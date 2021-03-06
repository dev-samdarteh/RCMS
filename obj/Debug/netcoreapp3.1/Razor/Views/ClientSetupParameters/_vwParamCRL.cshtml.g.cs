#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5933ee25da944238bee76e3ce86fc306852d1f1b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ClientSetupParameters__vwParamCRL), @"mvc.1.0.view", @"/Views/ClientSetupParameters/_vwParamCRL.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5933ee25da944238bee76e3ce86fc306852d1f1b", @"/Views/ClientSetupParameters/_vwParamCRL.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_ClientSetupParameters__vwParamCRL : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.vm_cl.ChurchRoleModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
 if (Model != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral("    <table id=\"tabData_CRL\" class=\"table table-bordered table-striped\">\r\n        <thead>\r\n            <tr>\r\n                <th> Church Role </th>\r\n                <th> Role Type </th>\r\n");
            WriteLiteral(@"                <th> Apply-to-Unit </th>
                <th width=""50px"" style=""text-align:center; vertical-align: middle""> Scope </th>
                <th width=""50px"" style=""text-align:center; vertical-align: middle"">Status</th>
                <th class=""p-1 justify-content-center"" style=""vertical-align: middle; text-align: center; width: 60px "">
");
            WriteLiteral(@"                    <a class=""btn btn-default text-success border-0"" id=""btnAddEdit_CRL"" role=""button"" data-remote=""false""
                       data-backdrop=""static"" onclick=""ReloadCurrPage_CRL(16, 1)"">
                        <i class=""fa fa-refresh text-secondary""></i>
                    </a>
                </th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 27 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
             foreach (var item in Model.lsChurchRoleModels)
            {
                if (item.oChurchRole != null)
                {
                    var strCRL_Par = !string.IsNullOrEmpty(item.strParentRole) ? item.strParentRole + " /" : item.strParentRole;
                    var strDesc = '"' + strCRL_Par + item.strChurchRole + '"';
                    var strParDesc = '"' + item.strChurchRole + '"';


#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td style=\"vertical-align: middle\">\r\n");
#nullable restore
#line 37 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                             if (item.bl_IsParentRole)
                            {
                                var _strRole = item.strChurchRole + " (" + item.numSubRolesCount + ")";

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <a href=\"#section-crl\" data-backdrop=\"static\"");
            BeginWriteAttribute("onclick", " onclick=\"", 2034, "\"", 2104, 6);
            WriteAttributeValue("", 2044, "ReloadCurrPage_CRL(16,", 2044, 22, true);
            WriteAttributeValue(" ", 2066, "1,", 2067, 3, true);
#nullable restore
#line 40 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 2069, item.oChurchRole.Id, 2070, 20, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2090, ",", 2090, 1, true);
#nullable restore
#line 40 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 2091, strParDesc, 2092, 11, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 2103, ")", 2103, 1, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                    ");
#nullable restore
#line 41 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                               Write(_strRole);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            WriteLiteral("                                </a>\r\n");
#nullable restore
#line 44 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                            }
                            else
                            {
                                

#line default
#line hidden
#nullable disable
#nullable restore
#line 47 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                           Write(item.strChurchRole);

#line default
#line hidden
#nullable disable
#nullable restore
#line 47 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                   
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </td>\r\n                        <td style=\"vertical-align: middle\">\r\n                            ");
#nullable restore
#line 51 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                       Write(item.strOrgType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n");
            WriteLiteral("                        <td style=\"vertical-align: middle\">\r\n");
#nullable restore
#line 57 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                             if (item.oChurchRole.IsApplyToMainstreamUnit)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("<i class=\"fas fa-church fa-xs text-secondary mr-1\"></i>");
#nullable restore
#line 58 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            ");
#nullable restore
#line 59 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                       Write(item.strApplyToChurchUnit);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td width=\"50px\" style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 62 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                             if (item.strOwnershipCode == "O")
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge badge-primary text-center text-light font-weight-normal border\"> <i class=\"fas fa-cog\"></i>  ");
#nullable restore
#line 63 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                        Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 63 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                                                              }
                        else if (item.strOwnershipCode == "I")
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-primary text-center text-light font-weight-normal border\"> <i class=\"fas fa-download\"></i>  ");
#nullable restore
#line 65 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                          Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 65 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                                                                }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-light text-center font-weight-normal border\"> <i class=\"fas fa-info-circle\"></i> ");
#nullable restore
#line 67 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                           Write(item.strOwnershipStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>");
#nullable restore
#line 67 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </td>\r\n                        <td style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 70 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                             if (item.bl_IsActivated)

#line default
#line hidden
#nullable disable
#nullable restore
#line 70 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                    
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge badge-success text-center text-light font-weight-normal border\"> <i class=\"fas fa-check fa-sm text-light\"></i> ");
#nullable restore
#line 71 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                                          Write(item.strStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span> ");
#nullable restore
#line 71 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                                                                       }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-secondary text-center text-light font-weight-normal border\"> <i class=\"fas fa-times fa-sm\"></i> ");
#nullable restore
#line 73 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                              Write(item.strStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>");
#nullable restore
#line 73 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                                                                                                                                                          }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                        </td>
                        <td class=""p-1"" style=""vertical-align: middle; text-align:center "">

                            <button type=""button"" class=""btn btn-outline-light btn-sm dropdown-toggle border-0"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                                <i class=""fa fa-ellipsis-v text-gray text-lg""></i>
                            </button>
                            <div class=""dropdown-menu p-1"">
                                <ul class=""m-0 p-0"" style=""list-style-type: none;"">
                                    <li>
                                        <a role=""button"" id=""btnAddEdit"" data-remote=""false"" class=""btn btn-light text-left w-100 border-0"" data-backdrop=""static""");
            BeginWriteAttribute("onclick", "\r\n                                           onclick=\"", 5228, "\"", 5401, 12);
            WriteAttributeValue("", 5282, "AddEditCurrData_CRL(", 5282, 20, true);
#nullable restore
#line 84 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue("", 5302, item.oChurchRole.Id, 5302, 20, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5322, ",", 5322, 1, true);
            WriteAttributeValue(" ", 5323, "-1,", 5324, 4, true);
#nullable restore
#line 84 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 5327, Model.oAppGloOwnId_Logged, 5328, 26, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5354, ",", 5354, 1, true);
#nullable restore
#line 84 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 5355, Model.oChurchBodyId_Logged, 5356, 27, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5383, ",", 5383, 1, true);
#nullable restore
#line 84 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 5384, strDesc, 5385, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5393, ",", 5393, 1, true);
            WriteAttributeValue(" ", 5394, "16,", 5395, 4, true);
            WriteAttributeValue(" ", 5398, "0)", 5399, 3, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                            <i class=\"fa fa-eye\"></i><span class=\"text-md-left ml-3\"> View </span>\r\n                                        </a>\r\n                                    </li>\r\n");
#nullable restore
#line 88 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                     if (item.oChurchRole.OwnedByChurchBodyId == Model.oChurchBodyId_Logged)
                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        <li>\r\n                                            <a role=\"button\" id=\"btnAddEdit\" data-remote=\"false\" class=\"btn btn-light text-left w-100 border-0\" data-backdrop=\"static\"");
            BeginWriteAttribute("onclick", "\r\n                                               onclick=\"", 5971, "\"", 6166, 12);
            WriteAttributeValue("", 6029, "AddEditCurrData_CRL(", 6029, 20, true);
#nullable restore
#line 92 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue("", 6049, item.oChurchRole.Id, 6049, 20, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6069, ",", 6069, 1, true);
            WriteAttributeValue(" ", 6070, "-1,", 6071, 4, true);
#nullable restore
#line 92 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 6074, item.oChurchRole.AppGlobalOwnerId, 6075, 34, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6109, ",", 6109, 1, true);
#nullable restore
#line 92 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 6110, item.oChurchRole.OwnedByChurchBodyId, 6111, 37, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6148, ",", 6148, 1, true);
#nullable restore
#line 92 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue(" ", 6149, strDesc, 6150, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6158, ",", 6158, 1, true);
            WriteAttributeValue(" ", 6159, "16,", 6160, 4, true);
            WriteAttributeValue(" ", 6163, "2)", 6164, 3, true);
            EndWriteAttribute();
            WriteLiteral(@">
                                                <i class=""fa fa-edit""></i><span class=""text-md-left ml-3""> Edit </span>
                                            </a>
                                        </li>
                                        <li>
                                            <a role=""button"" class=""btn btn-light text-danger text-left w-100 border-0""");
            BeginWriteAttribute("onclick", "\r\n                                               onclick=\"", 6553, "\"", 6662, 5);
            WriteAttributeValue("", 6611, "DeleteCurrData_CRL(", 6611, 19, true);
#nullable restore
#line 98 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
WriteAttributeValue("", 6630, item.oChurchRole.Id, 6630, 20, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6650, ",", 6650, 1, true);
            WriteAttributeValue(" ", 6651, "false,", 6652, 7, true);
            WriteAttributeValue(" ", 6658, "\'\')", 6659, 4, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                                <i class=\"fa fa-trash\"></i><span class=\"text-md-left ml-3\" style=\"color:#000\"> Delete </span>\r\n                                            </a>\r\n                                        </li>\r\n");
#nullable restore
#line 102 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                </ul>\r\n                            </div>                             \r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 107 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 111 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCRL.cshtml"

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.vm_cl.ChurchRoleModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
