﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiagDash
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Index.cshtml"
    using DiagDash;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.4.0")]
    public partial class Index : RazorGenerator.Templating.RazorTemplateBase
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");


WriteLiteral("\r\n<!DOCTYPE html>\r\n<html lang=\"en\">\r\n    <head>\r\n        <meta charset=\"utf-8\" />" +
"\r\n        <title>DiagDash</title>\r\n        <meta name=\"viewport\" content=\"width=" +
"device-width, initial-scale=1.0\" />\r\n        <link href=\"");


            
            #line 10 "..\..\Index.cshtml"
               Write(ResourceHelper.Url("Content/bootstrap.min.css"));

            
            #line default
            #line hidden
WriteLiteral("\" rel=\"stylesheet\" media=\"screen\" />\r\n        <link href=\"");


            
            #line 11 "..\..\Index.cshtml"
               Write(ResourceHelper.Url("Content/bootstrap-responsive.min.css"));

            
            #line default
            #line hidden
WriteLiteral("\" rel=\"stylesheet\" media=\"screen\" />\r\n        <link href=\"");


            
            #line 12 "..\..\Index.cshtml"
               Write(ResourceHelper.Url("Content/toastr.min.css"));

            
            #line default
            #line hidden
WriteLiteral(@""" rel=""stylesheet"" media=""screen"" />
    </head>
    <body style=""padding-top: 60px; padding-bottom: 40px;"">
        <div class=""navbar navbar-inverse navbar-fixed-top"">
            <div class=""navbar-inner"">
                <div class=""container-fluid"">
                    <button data-target="".nav-collapse"" data-toggle=""collapse"" class=""btn btn-navbar""
                        type=""button"">
                        <span class=""icon-bar""></span>
                        <span class=""icon-bar""></span>
                        <span class=""icon-bar""></span>
                    </button>
                    <img id=""ajax-spinner"" src=""");


            
            #line 24 "..\..\Index.cshtml"
                                           Write(ResourceHelper.Url("Content/images/ajax-loader.gif"));

            
            #line default
            #line hidden
WriteLiteral("\" width=\"16\" height=\"16\" style=\"visibility: hidden;\" />\r\n                    <div" +
" class=\"nav-collapse collapse\">\r\n                        <ul class=\"nav\" data-bi" +
"nd=\"foreach: rootObjects\">\r\n                            <li data-bind=\"css: { ac" +
"tive: $data.id == $root.chosenRootObject().id },\r\n                              " +
"             click: $root.showDashbord,\r\n                                       " +
"    if: $data.id == undefined\">\r\n                                <a href=\"#\" cla" +
"ss=\"brand\">DiagDash</a>\r\n                            </li>\r\n                    " +
"        <li data-bind=\"css: { active: $data.id == $root.chosenRootObject().id }," +
"\r\n                                           click: $root.goToRootObject,\r\n     " +
"                                      if: $data.id != undefined\">\r\n             " +
"                   <a data-bind=\"text: $data.id\" href=\"#\"></a>\r\n                " +
"            </li>\r\n                        </ul>\r\n                    </div>\r\n  " +
"              </div>\r\n            </div>            \r\n        </div>\r\n        <d" +
"iv class=\"container-fluid\">\r\n            <div class=\"row-fluid\">\r\n              " +
"  <div class=\"span12\">\r\n                    <div class=\"row-fluid\"  data-bind=\"v" +
"isible: chosenRootObject().id, with: chosenRootObject\">\r\n                       " +
" <table class=\"table table-bordered table-striped\">\r\n                           " +
" <thead>\r\n                                <tr>\r\n                                " +
"    <th>Name</th>\r\n                                    <th>Value</th>\r\n         " +
"                       </tr>\r\n                            </thead>\r\n            " +
"                <tbody data-bind=\"foreach: rows\">\r\n                             " +
"   <tr>\r\n                                    <td data-bind=\"text: name, attr: { " +
"title: doc }\"></td>\r\n                                    <td data-bind=\"text: va" +
"lue\"></td>\r\n                                </tr>\r\n                            <" +
"/tbody>\r\n                        </table>\r\n                    </div>\r\n         " +
"           <div class=\"row-fluid\"  data-bind=\"visible: !chosenRootObject().id, w" +
"ith: performanceCounter\">\r\n                        ");



WriteLiteral(@"
                        <table class=""table table-bordered table-striped"" style=""table-layout: fixed;"">
                            <thead>
                                <tr>
                                    <th style=""width: 50%;"">Name</th>
                                    <th style=""width: 50%;"">Value</th>
                                </tr>
                            </thead>
                            <tbody data-bind=""foreach: rows"">
                                <tr>
                                    <td data-bind=""text: counterName, attr: { title: counterHelp }""></td>
                                    <td data-bind=""text: value""></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        
            <hr />
            <footer>
                ");



WriteLiteral("\r\n                <a class=\"footer-nav\" href=\"");


            
            #line 84 "..\..\Index.cshtml"
                                       Write(DiagDashSettings.RootUrl);

            
            #line default
            #line hidden
WriteLiteral("\">Reload</a>|<a class=\"footer-nav\" href=\"/\">Leave!</a>\r\n            </footer>\r\n  " +
"      </div>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 87 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/jquery-1.9.1.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 88 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/bootstrap.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 89 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/knockout-2.2.1.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 90 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/sammy-0.7.4.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 91 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/toastr.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 92 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/jquery.signalR-1.0.1.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 93 "..\..\Index.cshtml"
                                       Write(ResourceHelper.SignalrUrl(includeHubs: true));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n        <script type=\"text/javascript\" src=\"");


            
            #line 94 "..\..\Index.cshtml"
                                       Write(ResourceHelper.Url("Scripts/app/index.min.js"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n\r\n        <script type=\"text/javascript\">\r\n            $(function ()" +
" {\r\n                // first one is a dummy. for perfcounters.\r\n                " +
"index.init([undefined, ");


            
            #line 99 "..\..\Index.cshtml"
                                  Write(String.Join(", ", RootObjectUtils.RootObjectIds.Select(x => String.Format("'{0}'", x))));

            
            #line default
            #line hidden
WriteLiteral("], \'");


            
            #line 99 "..\..\Index.cshtml"
                                                                                                                              Write(DiagDashSettings.RootUrl);

            
            #line default
            #line hidden
WriteLiteral("\');\r\n            });\r\n            \r\n        </script>\r\n    </body>\r\n</html>");


        }
    }
}
#pragma warning restore 1591
