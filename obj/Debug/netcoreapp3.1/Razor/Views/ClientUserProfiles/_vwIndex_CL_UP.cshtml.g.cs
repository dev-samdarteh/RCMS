#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "419d28bf117124c281ed9c5ed79591c660acdf0a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ClientUserProfiles__vwIndex_CL_UP), @"mvc.1.0.view", @"/Views/ClientUserProfiles/_vwIndex_CL_UP.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"419d28bf117124c281ed9c5ed79591c660acdf0a", @"/Views/ClientUserProfiles/_vwIndex_CL_UP.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_ClientUserProfiles__vwIndex_CL_UP : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.vm_app_ven.UserProfileModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
 if (Model != null)
{
    int? _oAGOId = -1; if (Model.oAppGloOwnId != null) { _oAGOId = (int)Model.oAppGloOwnId; };
    int? _oCBId = -1; if (Model.oChurchBodyId != null) { _oCBId = (int)Model.oChurchBodyId; }


#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <table id=""tabData_CL_UP"" class=""table table-bordered table-striped"">
        <thead>
            <tr>
                <th class=""pl-2"" style=""text-align:left; vertical-align: middle"">
                    User Description
                </th>
                <th style=""vertical-align: middle"">
                    Username
                </th>
                <th style=""vertical-align: middle"">
                    Assigned Role
                </th>
                <th style=""vertical-align: middle"">
                    User Group
                </th>
");
            WriteLiteral("                <th style=\"vertical-align: middle\">\r\n                    Status\r\n                </th>\r\n                <th style=\"text-align: center; width: auto; vertical-align: middle\">\r\n");
            WriteLiteral(@"                    <a class=""btn btn-default text-success border-0"" id=""btnAddEdit_CPR"" role=""button"" data-remote=""false""
                       data-backdrop=""static"" onclick=""ReloadCurrPage_CL_UP(0, 0, true)"">
                        <i class=""fa fa-refresh fa-secondary""></i>
                    </a>
                </th>
            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 43 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
             foreach (var item in Model.lsUserProfileModels)
            {
                var strDesc = '"' + item.oUserProfile.UserDesc + '"';

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td class=\"pl-2\" align=\"left\" style=\"vertical-align: middle\">\r\n");
            WriteLiteral("\r\n");
            WriteLiteral("                        <a role=\"button\"");
            BeginWriteAttribute("onclick", " onclick=\"", 2983, "\"", 3088, 9);
            WriteAttributeValue("", 2993, "ReloadCurrPage_CL_UPO(", 2993, 22, true);
#nullable restore
#line 56 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue("", 3015, item.oUserProfile.Id, 3015, 21, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 3036, ",", 3036, 1, true);
#nullable restore
#line 56 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 3037, item.oAppGloOwnId, 3038, 18, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 3056, ",", 3056, 1, true);
#nullable restore
#line 56 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 3057, item.oChurchBodyId, 3058, 19, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 3077, ",", 3077, 1, true);
            WriteAttributeValue(" ", 3078, "1,", 3079, 3, true);
            WriteAttributeValue(" ", 3081, "false)", 3082, 7, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                            ");
#nullable restore
#line 57 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                       Write(item.oUserProfile.UserDesc);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </a>\r\n                        \r\n                    </td>\r\n                    <td style=\"vertical-align: middle\">\r\n                        ");
#nullable restore
#line 62 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                   Write(item.oUserProfile.Username);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                   <td style=\"vertical-align: middle\">\r\n                        ");
#nullable restore
#line 65 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                   Write(item.strUserRoleName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                   <td style=\"vertical-align: middle\">\r\n                        ");
#nullable restore
#line 68 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                   Write(item.strUserGroupName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n");
            WriteLiteral("                <td style=\"vertical-align: middle; text-align:center\">\r\n");
#nullable restore
#line 74 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                     if (item.oUserProfile.UserStatus == "A")
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge badge-success text-center text-light font-weight-normal border\"> <i class=\"fas fa-check fa-sm text-light\"></i> ");
#nullable restore
#line 75 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                                                                                                                                  Write(item.strUserStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral("  </span> ");
#nullable restore
#line 75 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                                                                                                                                                                    }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <span class=\"badge badge-light text-center text-secondary font-weight-normal border\"> <i class=\"fas fa-times fa-sm\"></i> ");
#nullable restore
#line 77 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                                                                                                                          Write(item.strUserStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral(" </span>");
#nullable restore
#line 77 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                                                                                                                                                          }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </td>\r\n\r\n                    <td style=\" vertical-align: middle ; text-align:center\">   \r\n");
#nullable restore
#line 81 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                              _oAGOId = item.oUserProfile.AppGlobalOwnerId; _oCBId = item.oUserProfile.ChurchBodyId;
                              _oAGOId = _oAGOId == null ? -1 : _oAGOId; _oCBId = _oCBId == null ? -1 : _oCBId;
                          

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            <div class=\"btn-group\">\r\n");
            WriteLiteral(@"                                <button type=""button"" class=""btn btn-light border-0 bg-transparent"" data-toggle=""dropdown"" data-offset=""-52"">
                                    <i class=""fas fa-ellipsis-h text-secondary""></i>
                                </button>
                                <div class=""dropdown-menu"" role=""menu"">
                                    <a role=""button"" class=""dropdown-item""");
            BeginWriteAttribute("onclick", " onclick=\"", 5200, "\"", 5300, 11);
            WriteAttributeValue("", 5210, "AddEditCurrData_CL_UP(", 5210, 22, true);
#nullable restore
#line 91 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue("", 5232, item.oUserProfile.Id, 5232, 21, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5253, ",", 5253, 1, true);
#nullable restore
#line 91 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 5254, _oAGOId, 5255, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5263, ",", 5263, 1, true);
#nullable restore
#line 91 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 5264, _oCBId, 5265, 7, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5272, ",", 5272, 1, true);
#nullable restore
#line 91 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 5273, strDesc, 5274, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5282, ",", 5282, 1, true);
#nullable restore
#line 91 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 5283, Model.setIndex, 5284, 15, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 5299, ")", 5299, 1, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                        <i class=\"fa fa-pencil fa-sm \"></i> &nbsp; Edit\r\n                                    </a>\r\n");
#nullable restore
#line 94 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                     if (item.isVendorOwned == false)

#line default
#line hidden
#nullable disable
#nullable restore
#line 94 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                                                                                                                                                                                                                                                                                                                                                                                                   
                                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                        <a role=\"button\" class=\"dropdown-item text-danger\"");
            BeginWriteAttribute("onclick", " onclick=\"", 5969, "\"", 6033, 5);
            WriteAttributeValue("", 5979, "DeleteCurrData_CL_UP(", 5979, 21, true);
#nullable restore
#line 96 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue("", 6000, item.oUserProfile.Id, 6000, 21, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6021, ",", 6021, 1, true);
            WriteAttributeValue(" ", 6022, "false,", 6023, 7, true);
            WriteAttributeValue(" ", 6029, "\'\')", 6030, 4, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                                            <i class=\"fa fa-trash fa-sm\"></i> &nbsp; Delete\r\n                                        </a>\r\n");
#nullable restore
#line 99 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
                                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <div class=\"dropdown-divider\"></div>\r\n                                    <a role=\"button\" class=\"dropdown-item\"");
            BeginWriteAttribute("onclick", " onclick=\"", 6363, "\"", 6468, 9);
            WriteAttributeValue("", 6373, "ReloadCurrPage_CL_UPO(", 6373, 22, true);
#nullable restore
#line 101 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue("", 6395, item.oUserProfile.Id, 6395, 21, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6416, ",", 6416, 1, true);
#nullable restore
#line 101 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 6417, item.oAppGloOwnId, 6418, 18, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6436, ",", 6436, 1, true);
#nullable restore
#line 101 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
WriteAttributeValue(" ", 6437, item.oChurchBodyId, 6438, 19, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 6457, ",", 6457, 1, true);
            WriteAttributeValue(" ", 6458, "1,", 6459, 3, true);
            WriteAttributeValue(" ", 6461, "false)", 6462, 7, true);
            EndWriteAttribute();
            WriteLiteral("> \r\n                                        <i class=\"fa fa-folder-open fa-sm\"></i> &nbsp; Open Profile \r\n                                    </a>\r\n\r\n                                </div>\r\n                            </div>\r\n\r\n");
            WriteLiteral("                    </td>\r\n                </tr>\r\n");
#nullable restore
#line 126 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 131 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UP.cshtml"

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.vm_app_ven.UserProfileModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
