#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "dd83d5715fe8ddb9500d7bc3c92d49d02d93a274"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ClientUserProfiles__vwIndex_CL_UPR), @"mvc.1.0.view", @"/Views/ClientUserProfiles/_vwIndex_CL_UPR.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"dd83d5715fe8ddb9500d7bc3c92d49d02d93a274", @"/Views/ClientUserProfiles/_vwIndex_CL_UPR.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_ClientUserProfiles__vwIndex_CL_UPR : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<RhemaCMS.Models.MSTRModels.UserProfileRole>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
 if (Model != null)
{ 
   // var _oAGOId = -1; if (Model.oAppGlobalOwn_Logged != null) { _oAGOId = (int)Model.oAppGloOwnId_Logged; };
    using (Html.BeginForm("AddMod_CL_UPR", "ClientUserProfiles", FormMethod.Post, new { @id = "currForm_UPR" }))
    {
        // var test = Model.oAppGloOwnId;
        

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                          
        // var strL = "'" + Model.strAttnLongevity + "'";

        

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <table id=""tabData_CL_UPR"" class=""table table-bordered table-striped""  >
                <thead>
                    <tr > 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 
                        <th class=""row-hide"" width=""0"" hidden></th> 

                        <th class=""text-center""> <input type=""checkbox"" id=""_chk_IsRoleAssignedHdr""> </th>
                        <th > Role Name </th> 
                        <th");
            WriteLiteral(" class=\"text-center\"> Since </th> \r\n                    </tr>\r\n                </thead>\r\n                <tbody id=\"tabBody_CL_UPR\" >\r\n");
#nullable restore
#line 42 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                     for (int i = 0; i < Model.Count; i++)
                    {  

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr id=\"row_template_CTRY\" style=\"text-align:left\">\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 45 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].Created));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 46 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].LastMod));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 47 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].CreatedByUserId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 48 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].LastModByUserId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 49 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].AppGlobalOwnerId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 50 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].ChurchBodyId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 51 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 52 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].UserProfileId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 53 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].UserRoleId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                        <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 54 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                                                         Write(Html.HiddenFor(modelItem => Model[i].ProfileRoleStatus));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n\r\n                        <td style=\"text-align:center; vertical-align: middle; width: 50px\">\r\n                            ");
#nullable restore
#line 57 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                       Write(Html.CheckBoxFor(modelItem => Model[i].isRoleAssigned, new { @class = "text-sm" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td style=\"vertical-align: middle\">\r\n                            ");
#nullable restore
#line 60 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                       Write(Html.DisplayFor(modelItem => Model[i].strRoleName, new { @class = "form-control font-weight-normal" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td style=\"text-align:right; vertical-align: middle middle; width: 100px\">\r\n                            ");
#nullable restore
#line 63 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                       Write(Html.DisplayFor(modelItem => Model[i].strSTRT, new { @class = "form-control font-weight-normal" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 66 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </tbody>\r\n               \r\n            </table>\r\n");
#nullable restore
#line 71 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientUserProfiles\_vwIndex_CL_UPR.cshtml"
         


    }
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<RhemaCMS.Models.MSTRModels.UserProfileRole>> Html { get; private set; }
    }
}
#pragma warning restore 1591
