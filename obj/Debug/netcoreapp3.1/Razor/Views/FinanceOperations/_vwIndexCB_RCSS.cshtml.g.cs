#pragma checksum "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "c93589eb9e0f655132fe093301537a15bf11278b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_FinanceOperations__vwIndexCB_RCSS), @"mvc.1.0.view", @"/Views/FinanceOperations/_vwIndexCB_RCSS.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"c93589eb9e0f655132fe093301537a15bf11278b", @"/Views/FinanceOperations/_vwIndexCB_RCSS.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0b312f2952edfd28900dda7f5874117e7c09329e", @"/Views/_ViewImports.cshtml")]
    public class Views_FinanceOperations__vwIndexCB_RCSS : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<RhemaCMS.Models.ViewModels.vm_cl.CBTitheBalModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 4 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
 if (Model != null)
{

#line default
#line hidden
#nullable disable
            WriteLiteral(@"    <table id=""tabDataCB_RCSS"" class=""table table-bordered table-striped"" style=""font-family:Verdana, Geneva, Tahoma, sans-serif; font-size: small"">
        <thead>
            <tr>
                <th style=""vertical-align: middle; ""> Congregation </th>

                
                <th style=""vertical-align: middle; text-align:center""> Total Receipts </th>
                <th style=""vertical-align: middle; text-align:center""> Total Disbursed </th>
                <th style=""vertical-align: middle; text-align:center""> Total Net </th> 

            </tr>
        </thead>
        <tbody>
");
#nullable restore
#line 19 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
             foreach (var item in Model.lsCBTitheBalModels)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\r\n                    <td style=\"vertical-align: middle\">\r\n");
#nullable restore
#line 23 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
                           var strDesc = '"' + item.oChurchBody?.Name + '"'; 

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <a href=\"#\" data-backdrop=\"static\"");
            BeginWriteAttribute("onclick", " onclick=\"", 989, "\"", 1086, 7);
            WriteAttributeValue("", 999, "ReloadCurrPageCB_RCSS(", 999, 22, true);
#nullable restore
#line 24 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
WriteAttributeValue("", 1021, item.oCBTitheTransBal?.ChurchBodyId, 1021, 36, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1057, ",", 1057, 1, true);
#nullable restore
#line 24 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
WriteAttributeValue(" ", 1058, Model.taskIndex, 1059, 16, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1075, ",", 1075, 1, true);
#nullable restore
#line 24 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
WriteAttributeValue(" ", 1076, strDesc, 1077, 8, false);

#line default
#line hidden
#nullable disable
            WriteAttributeValue("", 1085, ")", 1085, 1, true);
            EndWriteAttribute();
            WriteLiteral(">\r\n                            ");
#nullable restore
#line 25 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
                       Write(item.strChurchBody);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                        </a>\r\n                    </td>\r\n\r\n                   \r\n                    <td style=\"vertical-align: middle; text-align:center\">\r\n                        ");
#nullable restore
#line 31 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
                   Write(item.strTotCol);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td style=\"vertical-align: middle; text-align:center\">\r\n                        ");
#nullable restore
#line 34 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
                   Write(item.strTotOut);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>\r\n                    <td style=\"vertical-align: middle; text-align:center\">\r\n                        ");
#nullable restore
#line 37 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
                   Write(item.strTotNet);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                    </td>  \r\n                </tr>\r\n");
#nullable restore
#line 40 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        </tbody>
        <tfoot>
            <tr class=""text-bold"">
                <td style=""vertical-align: middle"">
                    Grand Total
                </td>  
                <td style=""vertical-align: middle; text-align:center"">
                    ");
#nullable restore
#line 48 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
               Write(Model.strGrandTotCol);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td style=\"vertical-align: middle; text-align:center\">\r\n                    ");
#nullable restore
#line 51 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
               Write(Model.strGrandTotOut);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td style=\"vertical-align: middle; text-align:center\">\r\n                    ");
#nullable restore
#line 54 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"
               Write(Model.strGrandTotNet);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td> \r\n            </tr>\r\n        </tfoot>\r\n    </table>\r\n");
#nullable restore
#line 59 "D:\dev_projects\web\church project\RhemaCMS\Views\FinanceOperations\_vwIndexCB_RCSS.cshtml"

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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<RhemaCMS.Models.ViewModels.vm_cl.CBTitheBalModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
