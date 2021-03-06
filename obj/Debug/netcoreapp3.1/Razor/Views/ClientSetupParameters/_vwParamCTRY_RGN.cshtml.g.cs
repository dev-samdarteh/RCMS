#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0424bd07544813ff2eb711255d41b0ca7aea4f44"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ClientSetupParameters__vwParamCTRY_RGN), @"mvc.1.0.view", @"/Views/ClientSetupParameters/_vwParamCTRY_RGN.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\dev_projects\web\church project\RhemaCMS\Views\_ViewImports.cshtml"
using RhemaCMS;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\dev_projects\web\church project\RhemaCMS\Views\_ViewImports.cshtml"
using RhemaCMS.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0424bd07544813ff2eb711255d41b0ca7aea4f44", @"/Views/ClientSetupParameters/_vwParamCTRY_RGN.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_ClientSetupParameters__vwParamCTRY_RGN : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.vm_cl.CountryRegionModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            WriteLiteral("\r\n");
            WriteLiteral("\r\n\r\n    <table id=\"tabData_CTRY_RGN\" class=\"table table-bordered table-striped \">\r\n        <thead>\r\n            <tr>\r\n                <th> Name of Region </th>\r\n");
            WriteLiteral(@"                <th width=""50px"" style=""text-align:center; vertical-align: middle""><i class=""fa fa-info text-dark""></i> </th>
                <th width=""50px"" style=""text-align:center; vertical-align: middle"" class=""btn-light text-secondary p-1""> 
                    <a href=""#"" class=""text-success"" data-backdrop=""static"" onclick=""ReloadCurrPage_CTRY_RGN(7, 2, '')"">
                        <i class=""fa fa-refresh text-success""></i>
                    </a>
                </th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 46 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
             foreach (var item in Model.lsCountryRegionModels)
            {
                if (item.oCountryRegion != null)
                {
                    var strDesc = '"' + item.strCountryRegion + '"';


#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    <tr>
                        <td>
                            <span>
                                <i class=""fas fa-ellipsis-v fa-sm""></i>
                                <i class=""fas fa-ellipsis-v fa-sm""></i>
                            </span>

                            <span class=""text font-weight-normal ml-1""> ");
#nullable restore
#line 59 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                   Write(item.strCountryRegion);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>\r\n                            <span class=\"badge badge-warning text-center ml-2\"> <i class=\"far fa-ellipsis-h\"></i> ");
#nullable restore
#line 60 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                             Write(item.oCountryRegion.RegCode);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>                             \r\n                        </td>\r\n\r\n");
            WriteLiteral("                    <td width=\"50px\" style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 67 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                         if (item.strOwnershipStatus == "Owned")
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge badge-success text-center  text-light font-weight-normal border\"> <i class=\"fas fa-cog fa-sm\"></i>  ");
#nullable restore
#line 68 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                                                           Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 68 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                                                                                                 }
                        else if (item.strOwnershipStatus == "Inherited")
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-secondary text-center text-light font-weight-normal border\"> <i class=\"fas fa-download fa-sm\"></i>  ");
#nullable restore
#line 70 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                                                                  Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 70 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                                                                                                        }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge text-center font-weight-normal border\"> <i class=\"fas fa-info fa-sm\"></i> ");
#nullable restore
#line 72 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                                  Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>");
#nullable restore
#line 72 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                                                                                                                                       }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    </td>
                    <td width=""70px"" class=""p-0"" style=""text-align:center; vertical-align: middle"">
                        <button type=""button"" class=""btn btn-outline-light btn-sm dropdown-toggle border-0"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                            <i class=""fa fa-ellipsis-v text-gray text-lg""></i>
                        </button>
                        <div class=""dropdown-menu p-1"">
                            <ul class=""m-0 p-0"" style=""list-style-type: none;"">
                                <li>
                                    <a role=""button"" id=""btnAddEdit"" data-remote=""false"" class=""btn btn-light text-left w-100 border-0"" data-backdrop=""static""");
            BeginWriteAttribute("onclick", "\r\n                                       onclick=\"", 5132, "\"", 5244, 6);
            WriteAttributeValue("", 5182, "AddEditCurrData_CTRY_RGN(", 5182, 25, true);
#nullable restore
#line 82 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
WriteAttributeValue("", 5207, item.oCountryRegion.Id, 5207, 23, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5230, ",", 5230, 1, true);
#nullable restore
#line 82 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
WriteAttributeValue(" ", 5231, strDesc, 5232, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5240, ",", 5240, 1, true);
            WriteAttributeValue(" ", 5241, "0)", 5242, 3, true);
            EndWriteAttribute();
            WriteLiteral("> \r\n                                        <i class=\"fa fa-eye\"></i><span class=\"text-md-left ml-3\"> View </span>\r\n                                    </a>\r\n                                </li>\r\n");
#nullable restore
#line 86 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                 if (item.oCountryRegion.OwnedByChurchBodyId == Model.oChurchBodyId_Logged)
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <li>\r\n                                        <a role=\"button\" id=\"btnAddEdit\" data-remote=\"false\" class=\"btn btn-light text-left w-100 border-0\" data-backdrop=\"static\"");
            BeginWriteAttribute("onclick", "\r\n                                           onclick=\"", 5790, "\"", 5906, 6);
            WriteAttributeValue("", 5844, "AddEditCurrData_CTRY_RGN(", 5844, 25, true);
#nullable restore
#line 90 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
WriteAttributeValue("", 5869, item.oCountryRegion.Id, 5869, 23, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5892, ",", 5892, 1, true);
#nullable restore
#line 90 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
WriteAttributeValue(" ", 5893, strDesc, 5894, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5902, ",", 5902, 1, true);
            WriteAttributeValue(" ", 5903, "2)", 5904, 3, true);
            EndWriteAttribute();
            WriteLiteral(@">
                                            <i class=""fa fa-edit""></i><span class=""text-md-left ml-3""> Edit </span>
                                        </a>
                                    </li>
                                    <li>
                                        <a role=""button"" class=""btn btn-light text-danger text-left w-100 border-0""");
            BeginWriteAttribute("onclick", "\r\n                                           onclick=\"", 6273, "\"", 6386, 5);
            WriteAttributeValue("", 6327, "DeleteCurrData_CTRY_RGN(", 6327, 24, true);
#nullable restore
#line 96 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
WriteAttributeValue("", 6351, item.oCountryRegion.Id, 6351, 23, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6374, ",", 6374, 1, true);
            WriteAttributeValue(" ", 6375, "false,", 6376, 7, true);
            WriteAttributeValue(" ", 6382, "\'\')", 6383, 4, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                            <i class=\"fa fa-trash\"></i><span class=\"text-md-left ml-3\" style=\"color:#000\"> Delete </span>\r\n                                        </a>\r\n                                    </li>\r\n");
#nullable restore
#line 100 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            </ul>\r\n                        </div> \r\n                    </td>\r\n                    </tr>\r\n");
#nullable restore
#line 105 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCTRY_RGN.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </tbody>\r\n    </table>\r\n\r\n\r\n");
            WriteLiteral("\r\n\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.vm_cl.CountryRegionModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
