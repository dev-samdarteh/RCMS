#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0be332a577ede2171f0df6609605dbcff49a15d2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ClientSetupParameters__vwParamCURR), @"mvc.1.0.view", @"/Views/ClientSetupParameters/_vwParamCURR.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0be332a577ede2171f0df6609605dbcff49a15d2", @"/Views/ClientSetupParameters/_vwParamCURR.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_ClientSetupParameters__vwParamCURR : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.vm_cl.CurrencyCustomModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", "hidden", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("_hdnAppGloOwnId_BLK"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("_hdnAppChurchBodyId_BLK"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div");
            BeginWriteAttribute("class", " class=\"", 67, "\"", 75, 0);
            EndWriteAttribute();
            WriteLiteral(">\r\n");
#nullable restore
#line 4 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
     if (Model.pageIndex == 1)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"input-group mb-3\">\r\n            <button type=\"button\" class=\"btn btn-light text-secondary border border-info ml-1\" onclick=\"ReloadCurrPage_CURR(7, 3, 1)\">\r\n                <i class=\"fa fa-refresh\"></i>\r\n            </button>\r\n");
            WriteLiteral("            <button type=\"button\" class=\"btn btn-info border ml-1\" onclick=\"ReloadCurrPage_CURR(7, 3, 2)\">\r\n                <i class=\"fas fa-edit\"></i><span class=\"text-sm\"> Modify </span>\r\n            </button>\r\n        </div>\r\n");
#nullable restore
#line 23 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
    }
    else if (Model.pageIndex == 2)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div class=""input-group mb-3"">
            <button type=""button"" class=""btn btn-light text-secondary  border border-info mr-1"" onclick=""ReloadCurrPage_CURR(7, 3, 1)"">
                <i class=""fa fa-arrow-left""></i> Back
            </button>
            <button type=""button"" class=""btn btn-light text-secondary  border border-info mr-1"" onclick=""ReloadCurrPage_CURR(7, 3, 2)"">
                <i class=""fa fa-refresh""></i>
            </button>
            <button id=""btnSaveChanges_CURR_BLK"" type=""button"" class=""btn btn-info border"">
                <i class=""fas fa-save mr-1""></i><span > Save changes </span>
            </button>
        </div>
");
#nullable restore
#line 37 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n\r\n\r\n\r\n");
#nullable restore
#line 42 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
 if (Model.pageIndex == 1)
{

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <table id=""tabData_CURR"" class=""table table-bordered table-striped"">
        <thead>
            <tr>
                <th> Currency </th>
                <th> SMBL </th>
                <th> ISO </th>
                <th> Country </th>
                <th class=""text-center""><i class=""fas fa-check-double fa-sm""></i> </th> ");
            WriteLiteral("\r\n                <th class=\"text-center\"><i class=\"fas fa-home fa-sm\"></i></th> ");
            WriteLiteral("\r\n                <th> Base Rate </th>\r\n            </tr>\r\n        </thead>\r\n        <tbody>\r\n\r\n");
#nullable restore
#line 58 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
             foreach (var item in Model.lsCurrencyCustomModels)
            {
                if (item.oCountry != null)
                {   //var strDesc = "'" + item.strCurrEngName + "'";

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\r\n                        <td>\r\n                            ");
#nullable restore
#line 64 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                       Write(item.strCurrEngName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td style=\"text-align:center; vertical-align: middle\">\r\n                            ");
#nullable restore
#line 67 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                       Write(item.strCurrSymbol);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td style=\"text-align:center; vertical-align: middle\">\r\n                            ");
#nullable restore
#line 70 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                       Write(item.strCurr3LISOSymbol);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td>\r\n                            ");
#nullable restore
#line 73 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                       Write(item.strCountry);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                        <td style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 76 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                             if (item.bl_IsCustomDisplay)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("<i class=\"fa fa-check fa-primary\"></i> ");
#nullable restore
#line 77 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                                    }
                            else
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral(" <i class=\"fa fa-times fa-secondary\"></i>");
#nullable restore
#line 79 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                                      }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </td>\r\n                        <td style=\"text-align:center; vertical-align: middle\">\r\n");
#nullable restore
#line 82 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                             if (item.bl_IsBaseCurrency)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("<span class=\"badge bg-info text-sm text-center text-light font-weight-normal border-dark \"> Default </span>");
#nullable restore
#line 83 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                                                                                                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </td>\r\n                        <td style=\"text-align: right; vertical-align: middle \">\r\n                            ");
#nullable restore
#line 86 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                       Write(item.strBaseRate);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n");
#nullable restore
#line 89 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </tbody>\r\n    </table>\r\n");
#nullable restore
#line 93 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
}
else if (Model.pageIndex == 2)
{
    var _oAGOId = -1; if (Model.oAppGlobalOwn_Logged != null) { _oAGOId = (int)Model.oAppGloOwnId_Logged; };
    using (Html.BeginForm("AddOrEdit_CURR_BLK", "ClientSetupParameters", FormMethod.Post, new { @id = "currForm_CURR_BLK" }))
    {
        // var test = Model.oAppGloOwnId;

#line default
#line hidden
#nullable disable
            WriteLiteral("        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d212312", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 100 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.oAppGloOwnId);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d214060", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 101 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.oAppGloOwnId_Logged);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d215732", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 102 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.oChurchBodyId);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d217481", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 103 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.oChurchBodyId_Logged);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d219154", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 104 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.pageIndex);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d220816", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 105 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.filterIndex);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d222480", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 106 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.setIndex);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d224141", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 107 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.subSetIndex);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "0be332a577ede2171f0df6609605dbcff49a15d225805", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 108 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.oUserId_Logged);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 109 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
        // var strL = "'" + Model.strAttnLongevity + "'";


#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"p-3 rounded justify-content-center card-footer clearfix\">\r\n");
            WriteLiteral(@"            <br />
            <table id=""tabData_CURR_BLK"" class=""table table-bordered table-striped"" style=""border: 1px solid deepskyblue"">
                <thead>
                    <tr style=""text-align: left;"">
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>

                        <th> Currency </th>
                        <th> SMBL </th>
                        <th> ISO-Symbol </th>
                        <th> Country </th>
                        <th class=""text-center""><i class=""fas fa-check-double fa-sm""></i> </th> ");
            WriteLiteral("\r\n                        <th class=\"text-center\"><i class=\"fas fa-home fa-sm\"></i> </th> ");
            WriteLiteral("\r\n                        <th> Base Rate </th>\r\n                    </tr>\r\n                </thead>\r\n                <tbody id=\"tabIdCTRYBody_BLK\">\r\n");
#nullable restore
#line 134 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                     for (int i = 0; i < Model.lsCurrencyCustomModels.Count - 43; i++)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <tr id=\"row_template_CURR\" style=\"text-align:left\">\r\n                            <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 137 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                             Write(Html.HiddenFor(modelItem => Model.lsCurrencyCustomModels[i].oCountry.CtryAlpha3Code));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 138 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                             Write(Html.HiddenFor(modelItem => Model.lsCurrencyCustomModels[i].numBaseRate));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 139 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                             Write(Html.HiddenFor(modelItem => Model.lsCurrencyCustomModels[i].oCountry.Created));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 140 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                             Write(Html.HiddenFor(modelItem => Model.lsCurrencyCustomModels[i].oCountry.LastMod));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 141 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                             Write(Html.HiddenFor(modelItem => Model.lsCurrencyCustomModels[i].oCountry.CreatedByUserId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                            <td class=\"row-hide\" width=\"0\" hidden>");
#nullable restore
#line 142 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                                                             Write(Html.HiddenFor(modelItem => Model.lsCurrencyCustomModels[i].oCountry.LastModByUserId));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n\r\n                            <td style=\"vertical-align: middle\">\r\n                                ");
#nullable restore
#line 145 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.DisplayFor(modelItem => Model.lsCurrencyCustomModels[i].strCurrEngName, new { @class = "form-control text-center font-weight-normal" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center; vertical-align: middle\">\r\n                                ");
#nullable restore
#line 148 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.DisplayFor(modelItem => Model.lsCurrencyCustomModels[i].strCurrSymbol, new { @class = "form-control font-weight-normal" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center; vertical-align: middle\">\r\n                                ");
#nullable restore
#line 151 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.DisplayFor(modelItem => Model.lsCurrencyCustomModels[i].strCurr3LISOSymbol, new { @class = "form-control font-weight-normal" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center; vertical-align: middle\">\r\n                                ");
#nullable restore
#line 154 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.DisplayFor(modelItem => Model.lsCurrencyCustomModels[i].strCountry, new { @class = "form-control font-weight-normal" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center; vertical-align: middle\">\r\n                                ");
#nullable restore
#line 157 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.CheckBoxFor(modelItem => Model.lsCurrencyCustomModels[i].bl_IsCustomDisplay, new { @class = "text-sm" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align:center; vertical-align: middle\">\r\n                                ");
#nullable restore
#line 160 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.CheckBoxFor(modelItem => Model.lsCurrencyCustomModels[i].bl_IsBaseCurrency, new { @class = "text-sm" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                            <td style=\"text-align: right; vertical-align: middle \">\r\n                                ");
#nullable restore
#line 163 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                           Write(Html.TextBoxFor(modelItem => Model.lsCurrencyCustomModels[i].numBaseRate, new { @class = "form-control font-weight-normal", @type = "number" }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </td>\r\n                        </tr>\r\n");
#nullable restore
#line 166 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                </tbody>
                <tfoot>
                    <tr class=""bg-white "" style=""font-weight: bold "">
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>
                        <th class=""row-hide"" width=""0"" hidden></th>

                        <th style=""vertical-align: middle""> ");
            WriteLiteral("  </th>\r\n                        <th align=\"right\" style=\"vertical-align: middle\"> ");
            WriteLiteral("  </th>\r\n                        <th align=\"right\" style=\"vertical-align: middle\"> ");
            WriteLiteral("  </th>\r\n                        <th align=\"right\" style=\"vertical-align: middle\"> ");
            WriteLiteral("  </th>\r\n                        <th align=\"right\" style=\"vertical-align: middle\"> ");
            WriteLiteral("  </th>\r\n                        <th align=\"right\" style=\"vertical-align: middle\"> ");
            WriteLiteral(" </th>\r\n                        <th align=\"right\" style=\"vertical-align: middle\"> ");
            WriteLiteral(" </th>\r\n                    </tr>\r\n                </tfoot>\r\n            </table>\r\n        </div>\r\n");
#nullable restore
#line 189 "D:\dev_projects\web\church project\RhemaCMS\Views\ClientSetupParameters\_vwParamCURR.cshtml"
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.vm_cl.CurrencyCustomModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
