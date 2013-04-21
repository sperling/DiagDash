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
        self.resizeGraph = true;
        self.showGraph = ko.observable(false);
        self.showGraph.subscribe(function (newValue) {
            if (newValue) {
                self.resizeGraph = true;
            }
        });
        
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
                    navigationViewModel.performanceCounter.resizeGraph = true;
                });

                this.get('#:rootObjectId', function () {
                    var rootObjectId = this.params.rootObjectId;
                    for (var i = 0; i < navigationViewModel.rootObjects.length; i++) {
                        var rootObject = navigationViewModel.rootObjects[i];

                        // only make request when correct root id. so #FooBar will never trigger.
                        if (rootObject.id == rootObjectId) {
                            navigationViewModel.chosenRootObject(rootObject);
                            navigationViewModel.performanceCounter.resizeGraph = true;

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
            var graphData = [],                     // holds graphMaxPoints of [x, y] for each performance counter.
                graphDataSets = [],                 // holds a set(label, color, data) for each peformance counter.
                graphMaxPoints = 10,
                snapshotMilliseconds;
            var graph = $.plot('#perf-counter-placeholder', graphDataSets, {
                series: {
                    shadowSize: 0 // Drawing is faster without shadows
                },
                yaxis: {
                    min: 0,
                    max: 100
                },
                xaxis: {
                    mode: 'time'
                },
                legend: {
                    backgroundColor: '000000'
                }
            });
            
            diagDashHub.client.updatePerformanceCounters = function (timestamp, counterSnapShots) {
                if (!hubInitDone) {
                    return;
                }
                
                var newGraphPoints = [],
                    i, j,
                    row,
                    utcToLocalTimeDifference = new Date().getTimezoneOffset() * 60 * 1000;      // getTimezoneOffset is in minutes.

                for (i = 0; i < counterSnapShots.length; i++) {
                    var rowIndex = navigationViewModel.performanceCounter.hashToRowIndex[counterSnapShots[i].Hash];
                    var snapShotValue = counterSnapShots[i].Value;
                    var snapShotNormalizedValue = counterSnapShots[i].NormalizedValue;

                    navigationViewModel.performanceCounter.rows()[rowIndex].value(snapShotValue);
                    // add new points.
                    // timestamp is in UTC milliseconds.
                    newGraphPoints[rowIndex] = [timestamp - utcToLocalTimeDifference, snapShotNormalizedValue];
                }

                // remove oldest if we have more then max points now.
                if (graphData.length === graphMaxPoints) {
                    graphData = graphData.slice(1);
                }
                // append new points last.
                graphData.push(newGraphPoints);
                
                // redraw if graph is visible.
                if (navigationViewModel.performanceCounter.showGraph() && !navigationViewModel.chosenRootObject().id) {
                    // dump over data.
                    for (i = 0; i < graphDataSets.length; i++) {
                        graphDataSets[i].data = [];
                        if (graphData.length < graphMaxPoints) {
                            // if we have less then max points, fill up with dummy data.
                            // so x-axis will be correct.
                            for (j = 0; j < graphMaxPoints - graphData.length; j++) {
                                graphDataSets[i].data.push([(timestamp - utcToLocalTimeDifference) - ((graphMaxPoints - j) * snapshotMilliseconds), 0]);
                            }
                        }
                        for (j = 0; j < graphData.length; j++) {
                            graphDataSets[i].data.push(graphData[j][i]);
                        }
                    }

                    graph.setData(graphDataSets);
                    if (navigationViewModel.performanceCounter.resizeGraph) {
                        graph = $.plot('#perf-counter-placeholder', graphDataSets, {
                            series: {
                                shadowSize: 0 // Drawing is faster without shadows
                            },
                            yaxis: {
                                min: 0,
                                max: 100
                            },
                            xaxis: {
                                mode: 'time'
                            },
                            legend: {
                                backgroundColor: '000000'
                            }
                        });
                        graph.resize();
                        navigationViewModel.performanceCounter.resizeGraph = false;
                    }
                    
                    // need to setupGrid() for redrawing of x-axis.
                    graph.setupGrid();
                    graph.draw();
                }
            };

            $.connection.hub.start().fail(function () {
                toastr.error("Faild to start the hub", "Signalr error");
            }).done(function () {
                diagDashHub.server.getPerformanceCounters([]).done(function (data) {
                    if (!data || !data.perfCounters || !data.perfCounters.length) {
                        return;
                    }

                    for (var i = 0; i < data.perfCounters.length; i++) {
                        navigationViewModel.performanceCounter.rows.push(
                            new PeformanceCounterRowViewModel(data.perfCounters[i].CategoryName,
                                                              data.perfCounters[i].CounterHelp,
                                                              data.perfCounters[i].CounterName,
                                                              data.perfCounters[i].CounterType,
                                                              data.perfCounters[i].InstanceName,
                                                              data.perfCounters[i].Hash));
                        navigationViewModel.performanceCounter.hashToRowIndex[data.perfCounters[i].Hash] = i;
                        graphDataSets.push({ label: data.perfCounters[i].CounterName, color: i, data: [] })
                    }
                    snapshotMilliseconds = data.snapshotMilliseconds;
                    hubInitDone = true;
                }); 
            });
        }
    }
}(jQuery, ko));