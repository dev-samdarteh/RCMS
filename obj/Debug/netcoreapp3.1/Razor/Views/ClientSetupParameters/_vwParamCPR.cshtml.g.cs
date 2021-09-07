#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0943e56d964ba2f7817992c19f8845a751298816"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ClientSetupParameters__vwParamCPR), @"mvc.1.0.view", @"/Views/ClientSetupParameters/_vwParamCPR.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0943e56d964ba2f7817992c19f8845a751298816", @"/Views/ClientSetupParameters/_vwParamCPR.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_ClientSetupParameters__vwParamCPR : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.vm_cl.ChurchPeriodModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
 if (Model != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <table id=""tabData_CPR"" class=""table table-bordered table-striped"">
        <thead>
            <tr>
                <th> Period Desc </th>
                <th> Year </th>
                <th> From </th>
                <th> To </th>
                <th width=""50px"" style=""text-align:center; vertical-align: middle""> Scope </th>
                <th width=""50px"" style=""text-align:center; vertical-align: middle"">Status</th>
                <th class=""p-1 justify-content-center"" style=""vertical-align: middle; text-align: center; width: 60px "">
                    <a class=""btn btn-default text-success border-0"" id=""btnAddEdit_CPR"" role=""button"" data-remote=""false""
                       data-backdrop=""static"" onclick=""ReloadCurrPage_CPR(9, 1)"">
                        <i class=""fa fa-refresh fa-secondary""></i>
                    </a>
                </th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 24 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
             foreach (var item in Model.lsChurchPeriodModels)
            {
                if (item.oChurchPeriod != null)
                {

                    var strDesc = '"' +  item.strChurchPeriod + '"';


#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td style=\"vertical-align: middle\">\r\n                    ");
#nullable restore
#line 33 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
               Write(item.strChurchPeriod);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td style=\"vertical-align: middle\">\r\n                    ");
#nullable restore
#line 36 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
               Write(item.strYear);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td style=\"vertical-align: middle\">\r\n                    ");
#nullable restore
#line 39 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
               Write(item.strFrom);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td style=\"vertical-align: middle\">\r\n                    ");
#nullable restore
#line 42 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
               Write(item.strTo);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td width=\"50px\" style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 45 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                     if (item.strOwnershipCode == "O")
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge badge-primary text-center text-light font-weight-normal border\"> <i class=\"fas fa-cog\"></i>  ");
#nullable restore
#line 46 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 46 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                                                      }
                else if (item.strOwnershipCode == "I")
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-primary text-center text-light font-weight-normal border\"> <i class=\"fas fa-download\"></i>  ");
#nullable restore
#line 48 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                      Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 48 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                                                            }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-light text-center font-weight-normal border\"> <i class=\"fas fa-info-circle\"></i> ");
#nullable restore
#line 50 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                           Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>");
#nullable restore
#line 50 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </td>\r\n                <td style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 53 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                     if (item.oChurchPeriod.Status == "A")
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge badge-success text-center text-light font-weight-normal border\"> <i class=\"fas fa-check fa-sm text-light\"></i> ");
#nullable restore
#line 54 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                                  Write(item.strStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 54 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                                                               }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-secondary text-center text-light font-weight-normal border\"> <i class=\"fas fa-times fa-sm\"></i> ");
#nullable restore
#line 56 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                          Write(item.strStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>");
#nullable restore
#line 56 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                                                                                                                                                      }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </td>
                <td class=""p-1"" style=""vertical-align: middle; text-align:center "">
                    

                    <div class=""btn-group "">
                        <button type=""button"" class=""btn btn-outline-light btn-sm dropdown-toggle border-0"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                            <i class=""fa fa-ellipsis-v text-gray text-lg""></i>
                        </button>
                        <div class=""dropdown-menu p-1"">
                            <ul class=""m-0 p-0"" style=""list-style-type: none;"">
                                <li>
                                    <a role=""button"" id=""btnAddEdit"" data-remote=""false"" class=""btn btn-light text-left w-100 border-0"" data-backdrop=""static""");
            BeginWriteAttribute("onclick", "\r\n                                       onclick=\"", 3774, "\"", 3880, 6);
            WriteAttributeValue("", 3824, "AddEditCurrData_CPR(", 3824, 20, true);
#nullable restore
#line 69 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
WriteAttributeValue("", 3844, item.oChurchPeriod.Id, 3844, 22, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 3866, ",", 3866, 1, true);
#nullable restore
#line 69 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
WriteAttributeValue(" ", 3867, strDesc, 3868, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 3876, ",", 3876, 1, true);
            WriteAttributeValue(" ", 3877, "0)", 3878, 3, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                        <i class=\"fa fa-eye\"></i><span class=\"text-md-left ml-3\"> View </span>\r\n                                    </a>\r\n                                </li>\r\n");
#nullable restore
#line 73 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                 if (item.oChurchPeriod.OwnedByChurchBodyId == Model.oChurchBodyId_Logged)
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <li>\r\n                                        <a role=\"button\" id=\"btnAddEdit\" data-remote=\"false\" class=\"btn btn-light text-left w-100 border-0\" data-backdrop=\"static\"");
            BeginWriteAttribute("onclick", "\r\n                                           onclick=\"", 4424, "\"", 4534, 6);
            WriteAttributeValue("", 4478, "AddEditCurrData_CPR(", 4478, 20, true);
#nullable restore
#line 77 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
WriteAttributeValue("", 4498, item.oChurchPeriod.Id, 4498, 22, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 4520, ",", 4520, 1, true);
#nullable restore
#line 77 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
WriteAttributeValue(" ", 4521, strDesc, 4522, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 4530, ",", 4530, 1, true);
            WriteAttributeValue(" ", 4531, "2)", 4532, 3, true);
            EndWriteAttribute();
            WriteLiteral(@">
                                            <i class=""fa fa-edit""></i><span class=""text-md-left ml-3""> Edit </span>
                                        </a>
                                    </li>
                                    <li>
                                        <a role=""button"" class=""btn btn-light text-danger text-left w-100 border-0""");
            BeginWriteAttribute("onclick", "\r\n                                           onclick=\"", 4901, "\"", 5008, 5);
            WriteAttributeValue("", 4955, "DeleteCurrData_CPR(", 4955, 19, true);
#nullable restore
#line 83 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
WriteAttributeValue("", 4974, item.oChurchPeriod.Id, 4974, 22, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 4996, ",", 4996, 1, true);
            WriteAttributeValue(" ", 4997, "false,", 4998, 7, true);
            WriteAttributeValue(" ", 5004, "\'\')", 5005, 4, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                            <i class=\"fa fa-trash\"></i><span class=\"text-md-left ml-3\" style=\"color:#000\"> Delete </span>\r\n                                        </a>\r\n                                    </li>\r\n");
#nullable restore
#line 87 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            </ul>\r\n                        </div> \r\n                    </div>\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 93 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 97 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCPR.cshtml"

}

#line default
#line hidden
#nullable disable
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.vm_cl.ChurchPeriodModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
