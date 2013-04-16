var index = (function ($, ko) {
    var rootUrl;

    function ajaxRequest(method, params, callback) {
        $.ajax({
            dataType: "json",
            url: rootUrl + '?m=' + method,
            data: params,
            success: callback,
            cache: false
        });
    }

    function PeformanceCounterRowViewModel(categoryName, counterHelp, counterName, counterType, instanceName, hash) {
        // data
        var self = this;
        self.counterName = counterName;
        self.counterHelp = counterHelp;
        self.counterName = counterName;
        self.counterType = counterType;
        self.instanceName = instanceName;
        self.hash = hash;
        self.value = ko.observable();
    }

    function PeformanceCounterViewModel() {
        // data
        var self = this;
        self.rows = ko.observableArray();
        self.hashToRowIndex = {};
        self.showGraph = ko.observable(false);
    }

    function RootObjectRowViewModel(name, doc, value) {
        // data
        var self = this;
        self.name = name;
        self.doc = doc;
        self.value = value;
    }

    function RootObjectViewModel(id) {
        // data
        var self = this;
        self.id = id;
        self.rows = ko.observableArray();
    }

    function NavigationViewModel(rootObjectIds) {
        // data
        var self = this;
        self.rootObjects = [];
        self.chosenRootObject = ko.observable();
        self.performanceCounter = new PeformanceCounterViewModel();
                
        // behaviours
        self.goToRootObject = function (rootObject) {
            location.hash = rootObject.id;
        };

        self.showDashbord = function () {
            location.hash = "#";
        }

        for (var i = 0; i < rootObjectIds.length; i++) {
            self.rootObjects.push(new RootObjectViewModel(rootObjectIds[i]));
        }
        self.chosenRootObject(self.rootObjects[0]);
    }

    return {
        init: function (rootObjectIds, currentRootUrl) {
            rootUrl = currentRootUrl;

            $(document).ajaxError(function (event, jqxhr, settings, exception) {
                toastr.error(exception, "Faild to fetch data");
            });

            var navigationViewModel = new NavigationViewModel(rootObjectIds);

            ko.applyBindings(navigationViewModel);

            // TODO:    should be set from local storage.
            // navigationViewModel.performanceCounter.showGraph(false);

            // client-side routes    
            var sammyApp = Sammy(function () {
                this.get('#', function () {
                    // TODO:    start sending perf counters.
                    navigationViewModel.chosenRootObject(navigationViewModel.rootObjects[0]);
                });

                this.get('#:rootObjectId', function () {
                    var rootObjectId = this.params.rootObjectId;
                    for (var i = 0; i < navigationViewModel.rootObjects.length; i++) {
                        var rootObject = navigationViewModel.rootObjects[i];

                        // only make request when correct root id. so #FooBar will never trigger.
                        if (rootObject.id == rootObjectId) {
                            navigationViewModel.chosenRootObject(rootObject);

                            if (!rootObject.rows().length) {
                                ajaxRequest("loadRootObject", { id: rootObjectId }, function (data) {
                                    for (var j = 0; j < data.length; j++) {
                                        rootObject.rows.push(new RootObjectRowViewModel(data[j].Name, data[j].Doc, data[j].Value));
                                    }
                                });
                            }
                            // TODO:    stop sending perf counters.
                            return;
                        }
                    }

                    // redirect to dashboard if no root object was found.
                    this.redirect('#');
                });

                // show dashboard by default or use client-side hash URL.
                this.get('', function ()
                {
                    // TODO:    the default # dosen't show anymore.
                    this.app.runRoute('get', '#');
                });
            }).run();

            $('.footer-nav').click(function (e) {
                sammyApp.unload();
            });

            $(document).ajaxSend(function (event, request, settings) {
                $('#ajax-spinner').css('visibility', 'visible');
            });
                 
            $(document).ajaxComplete(function (event, request, settings) {
                $('#ajax-spinner').css('visibility', 'hidden');
            });

            var diagDashHub = $.connection.diagDashHub;
            var hubInitDone = false;

            var graphData = [], graphMaxPoints = 10, graphMaxX = graphMaxPoints - 1;
            var graph = $.plot('#perf-counter-placeholder', [], {
                series: {
                    shadowSize: 0 // Drawing is faster without shadows
                },
                yaxis: {
                    min: 0,
                    max: 100
                },
                xaxis: {
                    show: false
                }
            });
            diagDashHub.client.updatePerformanceCounters = function (counterSnapShots) {
                if (!hubInitDone) {
                    return;
                }

                var newGraphPoints = [],
                    i, j,
                    row;

                for (i = 0; i < counterSnapShots.length; i++) {
                    var rowIndex = navigationViewModel.performanceCounter.hashToRowIndex[counterSnapShots[i].Hash];
                    var snapShotValue = counterSnapShots[i].Value;
                    
                    navigationViewModel.performanceCounter.rows()[rowIndex].value(snapShotValue);
                    // add new points at max x.
                    newGraphPoints[rowIndex] = [graphMaxX, snapShotValue];
                }

                // remove oldest if we have more then max points now.
                if (graphData.length > 10) {
                    graphData = graphData.slice(1);
                }
                // move old points -1 in x-axis.
                for (i = 0; i < graphData.length; i++) {
                    row = graphData[i];

                    for (j = 0; j < row.length; j++) {
                        row[j][0] -= 1;
                    }
                }
                // append new points last.
                graphData.push(newGraphPoints);

                // redraw if graph is visible.
                if (navigationViewModel.performanceCounter.showGraph()) {
                    graph.setData(graphData);
                    // Since the axes don't change, we don't need to call graph.setupGrid()
                    graph.draw();
                }
            };

            $.connection.hub.start().fail(function () {
                toastr.error("Faild to start the hub", "Signalr error");
            }).done(function () {
                diagDashHub.server.getPerformanceCounters([]).done(function (perfCounters) {
                    if (!perfCounters || !perfCounters.length) {
                        return;
                    }

                    for (var i = 0; i < perfCounters.length; i++) {
                        navigationViewModel.performanceCounter.rows.push(
                            new PeformanceCounterRowViewModel(perfCounters[i].CategoryName,
                                                              perfCounters[i].CounterHelp,
                                                              perfCounters[i].CounterName,
                                                              perfCounters[i].CounterType,
                                                              perfCounters[i].InstanceName,
                                                              perfCounters[i].Hash));
                        navigationViewModel.performanceCounter.hashToRowIndex[perfCounters[i].Hash] = i;
                    }

                    hubInitDone = true;
                }); 
            });
        }
    }
}(jQuery, ko));