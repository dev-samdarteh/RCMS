#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "733d784ab0841a53cba941f66be1669ad02071aa"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index_sa), @"mvc.1.0.view", @"/Views/Home/Index_sa.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"733d784ab0841a53cba941f66be1669ad02071aa", @"/Views/Home/Index_sa.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index_sa : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.HomeDashboardVM>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_LoadingPartial", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Home", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Index_sa", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("page-top"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("w-100 small"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
  
    var _strAppName = @ViewData["strAppName"] as string;
    var _pageTitle = @_strAppName + " - Dashboard";
    ///
    /// dashboard values
    var _TotalSubsDenom = @ViewData["TotalSubsDenom"] as string;
    var _TotalSubsCong = @ViewData["TotalSubsCong"] as string;
    var _TotalSysPriv = @ViewData["TotalSysPriv"] as string;
    var _TotalSysRoles = @ViewData["TotalSysRoles"] as string;
    var _TotSysProfiles = @ViewData["TotSysProfiles"] as string;
    var _TotSubscribers = @ViewData["TotSubscribers"] as string;
    var _TotDbaseCount = @ViewData["TotDbaseCount"] as string;
    var _TodaysAuditCount = @ViewData["TodaysAuditCount"] as string;
    var _TotClientProfiles = @ViewData["TotClientProfiles"] as string;
    var _TotClientProfiles_Admins = @ViewData["TotClientProfiles_Admins"] as string;

    ///
    ViewData["Title"] = _pageTitle; // Model.strAppName + " - Dashboard";
    Layout = "_Layout_sa";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "733d784ab0841a53cba941f66be1669ad02071aa6087", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "733d784ab0841a53cba941f66be1669ad02071aa7207", async() => {
                WriteLiteral(@"
    <div class=""se-pre-con""></div>

    <div class=""content-header"">
        <div class=""container-fluid"">
            <div class=""row mb-2"">
                <div class=""col-sm-6"">
                    <h1 class=""m-0 text-dark"">Dashboard</h1>
                </div><!-- /.col -->
                <div class=""col-sm-6"">
                    <ol class=""breadcrumb float-sm-right"">
                        <li class=""breadcrumb-item"">");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "733d784ab0841a53cba941f66be1669ad02071aa7921", async() => {
                    WriteLiteral("Home");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Controller = (string)__tagHelperAttribute_1.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
                __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_2.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral(@"</li>
                        <li class=""breadcrumb-item active"">Dashboard</li>
                    </ol>
                </div><!-- /.col Admin Pallete -->
            </div><!-- /.row -->
        </div><!-- /.container-fluid -->
    </div>
    <!-- Small boxes (Stat box) -->

    <div class=""row"">
        <div class=""col-lg col"">
            <!-- small box -->
            <div class=""small-box bg-info"">
                <div class=""inner"">
                    <h2>");
#nullable restore
#line 51 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TotalSubsDenom);

#line default
#line hidden
#nullable disable
                WriteLiteral(@" </h2>
                    <p>Denominations</p>
                </div>
                <div class=""icon"">
                    <i class=""fas fa-church""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>

        <!-- ./col -->
        <div class=""col-lg col"">
            <!-- small box -->
            <div class=""small-box bg-gradient-indigo"">
                <div class=""inner"">
                    <h2>");
#nullable restore
#line 66 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TotalSubsCong);

#line default
#line hidden
#nullable disable
                WriteLiteral("</h2>\r\n");
                WriteLiteral("\r\n                    <p> Congregations </p>\r\n                </div>\r\n                <div class=\"icon\">\r\n");
                WriteLiteral(@"                    <i class=""fas fa-place-of-worship""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class=""col-lg col"">
            <!-- small box -->
            <div class=""small-box bg-gradient-fuchsia"">
                <div class=""inner"">
                    <h2>");
#nullable restore
#line 83 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TotSubscribers);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</h2>
                    <p>Subscriptions</p>
                </div>
                <div class=""icon"">
                    <i class=""ion ion-person-add""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>

        <!-- ./col -->
    </div>

    <!-- /.row -->
    <div class=""row"">
        <!-- ./col -->
        <div class=""col-lg "">
            <!-- small box -->
            <div class=""small-box bg-secondary"">
                <div class=""inner"">
");
                WriteLiteral("                    <h2>");
#nullable restore
#line 104 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TotClientProfiles);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</h2>
                    <p>Users (Client Profiles)</p>
                </div>
                <div class=""icon"">
                    <i class=""fas fa-user-tag""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class=""col-lg "">
            <!-- small box -->
            <div class=""small-box bg-lightblue"">
                <div class=""inner"">
                    <h2>");
#nullable restore
#line 118 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TotSysProfiles);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</h2>
                    <p>System Accounts</p>
                </div>
                <div class=""icon"">
                    <i class=""fas fa-user-cog""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class=""col-lg "">
            <!-- small box -->
            <div class=""small-box bg-gradient-teal"">
                <div class=""inner"">
                    <h2>");
#nullable restore
#line 132 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TotDbaseCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</h2>
                    <p>Managed Databases</p>
                </div>
                <div class=""icon"">
                    <i class=""fas fa-database""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>
        <!-- ./col -->
        <div class=""col-lg "">
            <!-- small box -->
            <div class=""small-box bg-dark"">
                <div class=""inner"">
                    <h2>");
#nullable restore
#line 146 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
                   Write(_TodaysAuditCount);

#line default
#line hidden
#nullable disable
                WriteLiteral(@"</h2>
                    <p> Transactions Today </p>
                </div>
                <div class=""icon"">
                    <i class=""fas fa-table""></i>
                </div>
                <a href=""#"" class=""small-box-footer"">More info <i class=""fas fa-arrow-circle-right""></i></a>
            </div>
        </div>
    </div>

    <!-- Main row -->
    <!-- /.row (main row) -->
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n\r\n\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 165 "D:\dev_projects\web\church project\RhemaCMS\Views\Home\Index_sa.cshtml"
      await Html.RenderPartialAsync("_ValidationScriptsPartial");

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n    <script>\r\n        //<div class=\"se-pre-con\"></div>\r\n        $(document).ready(function () {\r\n            // Animate loader off screen\r\n            $(\".se-pre-con\").fadeOut(\"slow\");\r\n        });\r\n    </script>\r\n\r\n");
            }
            );
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.HomeDashboardVM> Html { get; private set; }
    }
}
#pragma warning restore 1591
