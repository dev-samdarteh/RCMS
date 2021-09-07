
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RhemaCMS.Controllers.con_adhc;
using RhemaCMS.Models;
using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.CLNTModels;
// using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using RhemaCMS.Models.ViewModels.vm_app_ven;
using static RhemaCMS.Controllers.con_adhc.AppUtilties;
using static RhemaCMS.Models.ViewModels.vm_app_ven.AppVenAdminVM;
//using Microsoft.Extensions.Hosting;

namespace RhemaCMS.Controllers.con_app_va
{
    public class AppVenAdminController : Controller
    {
        private readonly MSTR_DbContext _context;
       // private readonly MSTR_DbContext _masterContextLog;
      //  private readonly ChurchModelContext _clientContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
     //   private UserProfile _oLoggedUser;

        private bool isCurrValid = false;
       // private UserSessionPrivilege oUserLogIn_Priv = null;
        
        private List<DiscreteLookup> dlCBDivOrgTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlShareStatus = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlOwnerStatus = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlGenStatuses = new List<DiscreteLookup>();
      // private List<DiscreteLookup> dlChurchType = new List<DiscreteLookup>();
       // private List<DiscreteLookup> dlChuWorkStat = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlChuWorkStats = new List<DiscreteLookup>();

        private List<DiscreteLookup> dlUserRoleTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlUserLevels = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlUserAuthTypes = new List<DiscreteLookup>();

        private readonly IConfiguration _configuration;
        private ChurchModelContext _clientDBContext;


        //private readonly MSTR_DbContext _masterContext;
        //private ChurchModelContext _context;

      //  private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        ///
        //private string _strClientConn;

        //  private string _clientDBConnString;
        private UserProfile _oLoggedUser;
        // private UserRole _oLoggedRole;
        private MSTRChurchBody _oLoggedCB_MSTR;
        private MSTRAppGlobalOwner _oLoggedAGO_MSTR;

        // private bool isCurrValid = false;
        private UserSessionPrivilege oUserLogIn_Priv = null;

        /// localized
        private ChurchBody _oLoggedCB;
        private AppGlobalOwner _oLoggedAGO;




        public AppVenAdminController(MSTR_DbContext context, IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, IConfiguration configuration) //ChurchModelContext ctx, 
        {
            _context = context;

          //  _masterContextLog = context; // new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString);

           // _clientContext = ctx ;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;




            //_httpContextAccessor = httpContextAccessor;
            //_tempDataDictionaryFactory = tempDataDictionaryFactory;


            //// get user logon info... authenticate else logout
            //if (!SetUserLogged())
            //    RedirectToAction("LoginUserAcc", "UserLogin");


            //// get user logon details... from memory
            //// _oLoggedRole = oUserLogIn_Priv.UserRole; 

            //var isAuth = this.oUserLogIn_Priv != null;
            ////  if (!isAuth) isAuth = SetUserLogged();

            //if (isAuth)
            //{
            //    this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
            //    this._oLoggedCB_MSTR = this.oUserLogIn_Priv.ChurchBody;
            //    this._oLoggedAGO_MSTR = this.oUserLogIn_Priv.AppGlobalOwner;
            //    this._oLoggedUser.strChurchCode_AGO = this._oLoggedAGO_MSTR != null ? this._oLoggedAGO_MSTR.GlobalChurchCode : "";
            //    this._oLoggedUser.strChurchCode_CB = this._oLoggedCB_MSTR != null ? this._oLoggedCB_MSTR.GlobalChurchCode : "";
            //    ///
            //    this._oLoggedCB = this.oUserLogIn_Priv.ChurchBody_CLNT;
            //    this._oLoggedAGO = this.oUserLogIn_Priv.AppGlobalOwner_CLNT;
            //}


            //if (this._oLoggedUser == null)
            //    RedirectToAction("LoginUserAcc", "UserLogin");

            //if (this._oLoggedUser.AppGlobalOwnerId == null || this._oLoggedUser.ChurchBodyId == null)
            //    RedirectToAction("LoginUserAcc", "UserLogin");


            //// _context = context;
            ////  this._context = clientCtx;

            //this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

            //if (clientCtx == null)
            //    _context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

            ////_context = GetClientDBContext();


            ///// synchronize AGO, CL, CB, CTRY  or @login 
            //// this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

            ///// get the localized data... using the MSTR data
            //if (this._context != null && (this._oLoggedAGO == null || this._oLoggedCB == null))
            //{
            //    this._oLoggedAGO = this._context.AppGlobalOwner.AsNoTracking()
            //                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
            //    this._oLoggedCB = this._context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
            //                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();
            //}



            ///// synchronize AGO, CL, CB, CTRY  or @login 
            //// this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

            ///// get the localized data... using the MSTR data
            //if (_context != null)
            //{
            //    this._oLoggedAGO = _context.AppGlobalOwner.AsNoTracking()
            //                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...

            //    this._oLoggedCB = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
            //                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();
            //}



             




            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "A", Desc = "Active" });            
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "P", Desc = "Pending" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "B", Desc = "Blocked" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "E", Desc = "Expired" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "D", Desc = "Deactive" });

            //SharingStatus { get; set; }  // A - Share with all sub-congregations, C - Share with child congregations only, N - Do not share
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "N", Desc = "Do not roll-down (share)" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "C", Desc = "Roll-down (share) for direct child congregations" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "A", Desc = "Roll-down (share) for all sub-congregations" });

            // OwnershipStatus { get; set; }  // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody
            dlOwnerStatus.Add(new DiscreteLookup() { Category = "OwnStat", Val = "O", Desc = "Originated" });
            dlOwnerStatus.Add(new DiscreteLookup() { Category = "OwnStat", Val = "I", Desc = "Inherited" });


            dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CN", Desc = "Congregation" });  //to look up congregation by church code [short or full path]  -- CB
            dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CH", Desc = "Congregation Head-unit" });  // oversight directly on congregations   -- CB
            dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CR", Desc = "Church Head (Apex)" }); // --  CB [church body]

            // dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "GB", Desc = "Governing Body" });              --  CB 
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CO", Desc = "Church Office" });                --   CB
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "DP", Desc = "Department" });  //Ministry        -- CSU [church sector unit]
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CG", Desc = "Church Grouping" });                -- CSU
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "SC", Desc = "Standing Committee" }); // Working Committee   -- CSU
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CE", Desc = "Church Enterprise" });              -- CB
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "TM", Desc = "Team" });   // Working Team .. group of roles/pos   -- CR  [church roles]
            ////dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "CP", Desc = "Church Position" });              -- CR
            //dlCBDivOrgTypes.Add(new DiscreteLookup() { Category = "CBDivOrgType", Val = "IB", Desc = "Independent Unit" });               -- CB
            ///

            /// 
            //dlChurchType.Add(new DiscreteLookup() { Category = "ChurchType", Val = "", Desc = "N/A" });
            //dlChurchType.Add(new DiscreteLookup() { Category = "ChurchType", Val = "CH", Desc = "Congregation Head-unit" });
            //dlChurchType.Add(new DiscreteLookup() { Category = "ChurchType", Val = "CF", Desc = "Congregation" });

            //dlChuWorkStat.Add(new DiscreteLookup() { Category = "ChuWorkStat", Val = "S", Desc = "Structure Only" });
            //dlChuWorkStat.Add(new DiscreteLookup() { Category = "ChuWChuWorkStatorkStat", Val = "O", Desc = "Operationalized" });

            dlChuWorkStats.Add(new DiscreteLookup() { Category = "", Val = "OP", Desc = "Operational" });
            dlChuWorkStats.Add(new DiscreteLookup() { Category = "ChuWorkStat", Val = "ST", Desc = "Structure only" });


            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "SYS", Desc = "System" }); // 0..
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "SUP_ADMN", Desc = "Super Admin" }); // 1
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "SYS_ADMN", Desc = "System Admin" });  // 2

            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_ADMN", Desc = "Church Administrator" }); // 6               
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_MGR", Desc = "Church Manager" }); // 7
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_EXC", Desc = "Church Executive" });// 8
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_RGSTR", Desc = "Church Registrar" }); // 9
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_ACCT", Desc = "Church Accountant" }); // 10
         // dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_CUST", Desc = "Church Custom" }); // 
          
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CF_ADMN", Desc = "Congregation Administrator" }); // 11                
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CF_MGR", Desc = "Congregation Manager" }); // 12
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CF_EXC", Desc = "Congregation Executive" });// 13
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CF_RGSTR", Desc = "Congregation Registrar" }); // 14
            dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CF_ACCT", Desc = "Congregation Accountant" }); // 15
         // dlUserRoleTypes.Add(new DiscreteLookup() { Category = "UserRolType", Val = "CH_CUST", Desc = "Church Custom" }); // 16
           

            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "1", Desc = "System" }); // 1
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "2", Desc = "Super Admin" }); // 2
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "3", Desc = "System Admin" });  // 3
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "4", Desc = "Client System Assistant" });  // 4
            // dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "4", Desc = "System Custom" });  // 4
            ///
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "6", Desc = "Church Administrator" }); // 6               
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "7", Desc = "Church Manager" }); // 7
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "8", Desc = "Church Executive" });// 8
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "9", Desc = "Church Registrar" }); // 9
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "10", Desc = "Church Accountant" }); // 10  
            // dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "CH_CUST", Desc = "Church Custom" }); // 
            ///
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "11", Desc = "Congregation Administrator" }); // 11                
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "12", Desc = "Congregation Manager" }); // 12
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "13", Desc = "Congregation Executive" });// 13
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "14", Desc = "Congregation Registrar" }); // 14
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "15", Desc = "Congregation Accountant" }); // 15 
            // dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "CH_CUST", Desc = "Church Custom" }); // 16


            dlUserAuthTypes.Add(new DiscreteLookup() { Category = "UserAuthType", Val = "1", Desc = "Two-way Authentication" });
            dlUserAuthTypes.Add(new DiscreteLookup() { Category = "UserAuthType", Val = "2", Desc = "Security Question Validation" });


            // The example displays the following output:
            //    Pattern                                  Result String
            //
            //    MM/dd/yyyy                               08/28/2014
            //    yyyy-MM-dd                               2014-08-28
            //    dddd, dd MMMM yyyy                       Thursday, 28 August 2014
            //    dddd, dd MMMM yyyy HH:mm                 Thursday, 28 August 2014 12:28
            //    dddd, dd MMMM yyyy hh:mm tt              Thursday, 28 August 2014 12:28 PM
            //    dddd, dd MMMM yyyy H:mm                  Thursday, 28 August 2014 12:28
            //    dddd, dd MMMM yyyy h:mm tt               Thursday, 28 August 2014 12:28 PM
            //    dddd, dd MMMM yyyy HH:mm:ss              Thursday, 28 August 2014 12:28:30
            //    MM/dd/yyyy HH:mm                         08/28/2014 12:28
            //    MM/dd/yyyy hh:mm tt                      08/28/2014 12:28 PM
            //    MM/dd/yyyy H:mm                          08/28/2014 12:28
            //    MM/dd/yyyy h:mm tt                       08/28/2014 12:28 PM
            //    yyyy-MM-dd HH:mm                         2014-08-28 12:28
            //    yyyy-MM-dd hh:mm tt                      2014-08-28 12:28 PM
            //    yyyy-MM-dd H:mm                          2014-08-28 12:28
            //    yyyy-MM-dd h:mm tt                       2014-08-28 12:28 PM
            //    MM/dd/yyyy HH:mm:ss                      08/28/2014 12:28:30
            //    yyyy-MM-dd HH:mm:ss                      2014-08-28 12:28:30
            //    MMMM dd                                  August 28
            //    MMMM dd                                  August 28
            //    yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK   2014-08-28T12:28:30.0000000
            //    yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK   2014-08-28T12:28:30.0000000
            //    ddd, dd MMM yyyy HH':'mm':'ss 'GMT'      Thu, 28 Aug 2014 12:28:30 GMT
            //    ddd, dd MMM yyyy HH':'mm':'ss 'GMT'      Thu, 28 Aug 2014 12:28:30 GMT
            //    yyyy'-'MM'-'dd'T'HH':'mm':'ss            2014-08-28T12:28:30
            //    HH:mm                                    12:28
            //    hh:mm tt                                 12:28 PM
            //    H:mm                                     12:28
            //    h:mm tt                                  12:28 PM
            //    HH:mm:ss                                 12:28:30
            //    yyyy'-'MM'-'dd HH':'mm':'ss'Z'           2014-08-28 12:28:30Z
            //    dddd, dd MMMM yyyy HH:mm:ss              Thursday, 28 August 2014 12:28:30
            //    yyyy MMMM                                2014 August
            //    yyyy MMMM                                2014 August

        }



        public int GetRoleTypeLevel(string oCode)
        {
            switch (oCode)
            {
                case "SYS": return 1;
                case "SUP_ADMN": return 2;
                case "SYS_ADMN": return 3;
                case "SYS_CUST": return 4;
                // case "SYS_CUST2": return 5;
                //
                case "CH_ADMN": return 6;
                case "CH_MGR": return 7;
                case "CH_EXC": return 8;
                case "CH_RGSTR": return 9;
                case "CH_ACCT": return 10;
                // case "CH_CUST2": return 10;
                //
                case "CF_ADMN": return 11;
                case "CF_MGR": return 12;
                case "CF_EXC": return 13;
                case "CF_RGSTR": return 14;
                case "CF_ACCT": return 15;
                // case "CF_CUST2": return 16; 
                //
                default: return 0;
            }
        }


        public static string GetStatusDesc(string oCode)
        { 
            switch (oCode)
            {
                case "A": return "Active";
                case "D": return "Deactive";
                case "P": return "Pending";
                case "E": return "Expired";


                default: return oCode;
            }
        }

        //AuditType -- TRANSACTIONAL = T, NAVIGATIONAL = N, LOGIN /LOGOUT = L 
        public static string GetAuditTypeDesc(string oCode)
        { 
            switch (oCode)
            {
                case "L": return "Login/Logout";
                case "N": return "Navigational";
                case "T": return "Transactional";

                default: return oCode;
            }
        }
         

        public static string GetChuOrgTypeDesc(string oCode)
        {
            switch (oCode)
            {
                case "CR": return "Church Head (Apex)";
                //case "GB": return "Governing Body";
                //case "CO": return "Church Office";
                //case "DP": return "Department";                
                //case "CG": return "Church Grouping";
                //case "SC": return "Standing Committee";
                //case "CE": return "Church Enterprise";
                //case "TM": return "Team";
                //// case "CP": return "Church Position";
                //case "IB": return "Independent Unit";
                case "CH": return "Congregation Head-unit";
                case "CN": return "Congregation";

                default: return oCode;
            }
        }

        public object GetChuOrgTypeDetail(string oCode, bool returnSetIndex)
        {
            switch (oCode)
            {
                case "CR": if (returnSetIndex) return 0; else return "Church Head (Apex)";
                //case "GB": if (returnSetIndex) return 1; else return "Governing Body";
                //case "CO": if (returnSetIndex) return 2; else return "Church Office";
                //case "DP": if (returnSetIndex) return 3; else return "Church Department";
                //case "CG": if (returnSetIndex) return 4; else return "Church Grouping";
                //case "SC": if (returnSetIndex) return 5; else return "Standing Committee";
                //case "CE": if (returnSetIndex) return 6; else return "Church Enterprise";
                //case "TM": if (returnSetIndex) return 7; else return "Team";
                ////case "CP": if (returnSetIndex) return 8; else return "Church Position";
               // case "IB": if (returnSetIndex) return 8; else return "Independent Unit";  // Independent Body e.g. Boards, Trustees
                case "CH": if (returnSetIndex) return 9; else return "Congregation Head-unit";
                case "CN": if (returnSetIndex) return 10; else return "Congregation";

                default: return oCode;
            }
        }

        public string GetChuOrgTypeCode(int setIndex)
        {
            switch (setIndex)
            {
                case 0: return "CR";
                //case 1: return "GB";
                //case 2: return "CO";
                //case 3: return "DP";
                //case 4: return "CG";
                //case 5: return "SC";
                //case 6: return "CE";
                //case 7: return "TM";
                //// case ?: return "CP";
                //case 8: return "IB";
                case 9: return "CH";
                case 10: return "CN";

                default: return "";
            }
        }


        public string GetConcatMemberName(string title, string fn, string mn, string ln, bool displayName = false)
        {
            if (displayName)
                return ((((!string.IsNullOrEmpty(title) ? title : "") + ' ' + fn).Trim() + " " + mn).Trim() + " " + ln).Trim();
            else
                return (((fn + ' ' + mn).Trim() + " " + ln).Trim() + " " + (!string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
        }

        //private async Task LogAction_UserAuditTrail(UserAuditTrail oUserTrail)
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail != null)
        //    {
        //        using (var logCtx = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
        //        {
        //            logCtx.UserAuditTrail.Add(oUserTrail);
        //            await logCtx.SaveChangesAsync();

        //            logCtx.Entry(oUserTrail).State = EntityState.Detached;
        //            ///
        //            //DetachAllEntities(logCtx);
        //            //logCtx.Dispose();
        //        }
        //    }
        //}


        private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail)  //, MSTR_DbContext currContext = null, string strTempConn = "")
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // MSTR_DbContext currContext = null, string strTempConn = ""
                //var _cs = strTempConn;
                //if (string.IsNullOrEmpty(_cs))

                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration.GetConnectionString("DefaultConnection"); //["ConnectionStrings:DefaultConnection"]; /// _masterContext.Database.GetDbConnection().ConnectionString

                if (!string.IsNullOrEmpty(_cs))
                {
                    using (var logCtx = new MSTR_DbContext(_cs)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
                    {
                        if (logCtx.Database.CanConnect() == false)
                            logCtx.Database.OpenConnection();
                        else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                            logCtx.Database.OpenConnection();

                        // var a = logCtx.Database.GetDbConnection().ConnectionString;
                        // var b = _masterContext.Database.GetDbConnection().ConnectionString;

                        /// 
                        logCtx.UserAuditTrail.Add(oUserTrail);
                        await logCtx.SaveChangesAsync();

                        //logCtx.SaveChanges();

                        logCtx.Entry(oUserTrail).State = EntityState.Detached;
                        ///
                        //DetachAllEntities(logCtx);

                        // close connection
                        logCtx.Database.CloseConnection();

                        //logCtx.Dispose();
                    }
                }

            }
        }

        //private async Task LogUserActivity_ClientUserAuditTrail(UserAuditTrail_CL oUserTrail)  //, string strTempConn = ""
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail != null)
        //    {
        //        // var tempCtx = _context;
        //        //  if (!string.IsNullOrEmpty(clientDBConnString))
        //        // {

        //        //// refreshValues... 
        //        //this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
        //        //_context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

        //        var _connstr_CL = AppUtilties.GetNewDBConnString_CL(_context, _configuration, this._oLoggedUser.AppGlobalOwnerId); /// this.GetCL_DBConnString();
        //        if (!string.IsNullOrEmpty(_connstr_CL))
        //        {
        //            using (var logCtx = new ChurchModelContext(_connstr_CL)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
        //            {
        //                //logCtx = _context;
        //                //var conn = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_context.Database.GetDbConnection().ConnectionString);
        //                ////  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
        //                //conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //                /////
        //                //logCtx.Database.GetDbConnection().ConnectionString = conn.ConnectionString;

        //                try
        //                {
        //                    if (logCtx.Database.CanConnect() == false)
        //                        logCtx.Database.OpenConnection();
        //                    else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
        //                        logCtx.Database.OpenConnection();

        //                    // var a = logCtx.Database.GetDbConnection().ConnectionString;
        //                    // var b = _masterContext.Database.GetDbConnection().ConnectionString;

        //                    /// 
        //                    logCtx.UserAuditTrail_CL.Add(oUserTrail);
        //                    await logCtx.SaveChangesAsync();

        //                    //logCtx.SaveChanges();

        //                    logCtx.Entry(oUserTrail).State = EntityState.Detached;
        //                    ///
        //                    //DetachAllEntities(logCtx);

        //                    // close connection
        //                    logCtx.Database.CloseConnection();

        //                    //logCtx.Dispose();

        //                }

        //                catch (Exception ex)
        //                {
        //                    throw;
        //                }
        //            }
        //        }


        //        //   }
        //    }
        //}


        public void DetachAllEntities(MSTR_DbContext ctx)
        {
            var changedEntriesCopy = ctx.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        public void DetachAllEntities_CL(ChurchModelContext ctx)
        {
            var changedEntriesCopy = ctx.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }


        //private async Task LoadDashboardValues()
        //{

        //    ViewData["TotalSubsDenom"] = String.Format("{0:N0}", 0);
        //    ViewData["TotalSubsCong"] = String.Format("{0:N0}", 0);
        //    ViewData["TotalSysPriv"] = String.Format("{0:N0}", 0);
        //    ViewData["TotalSysRoles"] = String.Format("{0:N0}", 0);
        //    ViewData["TotSysProfiles"] = String.Format("{0:N0}", 0);
        //    ViewData["TotSubscribers"] = String.Format("{0:N0}", 0);
        //    ViewData["TotDbaseCount"] = String.Format("{0:N0}", 0);
        //    ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);
        //    ViewData["TotClientProfiles"] = String.Format("{0:N0}", 0);
        //    ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", 0);


        //    //using (var dashContext = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
        //    //{
        //    //    var res = await (from dummyRes in new List<string> { "X" }
        //    //                     join tago in dashContext.AppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
        //    //                     join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //    //                     join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
        //    //                     join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
        //    //                     join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //    //                     join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //    //                     join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //    //                     join tdb in dashContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb
        //    //                     join tcln_a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A") on 1 equals 1 into _tcln_a
        //    //                     join tcln_d in (from a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
        //    //                                     from b in dashContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
        //    //                                     select a) on 1 equals 1 into _tcln_d
        //    //                     select new
        //    //                     {
        //    //                         cnt_tago = _tago.Count(),
        //    //                         cnt_tcb = _tcb.Count(),
        //    //                         cnt_tsr = _tsr.Count(),
        //    //                         cnt_tsp = _tsp.Count(),
        //    //                         cnt_tms = _tms.Count(),
        //    //                         cnt_tsubs = _tsubs.Count(),
        //    //                         cnt_tdb = _tdb.Count(),
        //    //                         cnt_ttc = _ttc.Count(),
        //    //                         cnt_tcln_d = _tcln_d.Count(),
        //    //                         cnt_tcln_a = _tcln_a.Count()
        //    //                     })
        //    //                .ToList().ToListAsync();

        //    //    ///
        //    //    ViewBag.TotalSubsDenom = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tago : 0));
        //    //    ViewBag.TotalSubsCong = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcb : 0));
        //    //    ViewBag.TotalSysPriv = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsp : 0));
        //    //    ViewBag.TotalSysRoles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsr : 0));
        //    //    ViewBag.TotSysProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tms : 0));
        //    //    ViewBag.TotSubscribers = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsubs : 0));
        //    //    ViewBag.TotDbaseCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tdb : 0));
        //    //    ViewBag.TodaysAuditCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_ttc : 0));
        //    //    ViewBag.TotClientProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_a : 0));
        //    //    ViewBag.TotClientProfiles_Admins = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_d : 0));
        //    //}




        //    //using (var dashContext = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
        //    //{
        //    //    var res = await (from dummyRes in new List<string> { "X" }
        //    //                     join tago in dashContext.AppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
        //    //                     join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //    //                     join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
        //    //                     join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
        //    //                     join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //    //                     join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //    //                     join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //    //                     join tdb in dashContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb
        //    //                     join tcln_a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A") on 1 equals 1 into _tcln_a
        //    //                     join tcln_d in (from a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
        //    //                                     from b in dashContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
        //    //                                     select a) on 1 equals 1 into _tcln_d
        //    //                     select new
        //    //                     {
        //    //                         cnt_tago = _tago.Count(),
        //    //                         cnt_tcb = _tcb.Count(),
        //    //                         cnt_tsr = _tsr.Count(),
        //    //                         cnt_tsp = _tsp.Count(),
        //    //                         cnt_tms = _tms.Count(),
        //    //                         cnt_tsubs = _tsubs.Count(),
        //    //                         cnt_tdb = _tdb.Count(),
        //    //                         cnt_ttc = _ttc.Count(),
        //    //                         cnt_tcln_d = _tcln_d.Count(),
        //    //                         cnt_tcln_a = _tcln_a.Count()
        //    //                     })
        //    //                .ToList().ToListAsync();

        //    //    ///
        //    //    if (res.Count > 0)
        //    //    {
        //    //        ViewData["TotalSubsDenom"] = String.Format("{0:N0}", res[0].cnt_tago);
        //    //        ViewData["TotalSubsCong"] = String.Format("{0:N0}", res[0].cnt_tcb);
        //    //        ViewData["TotalSysPriv"] = String.Format("{0:N0}", res[0].cnt_tsp);
        //    //        ViewData["TotalSysRoles"] = String.Format("{0:N0}", res[0].cnt_tsr);
        //    //        ViewData["TotSysProfiles"] = String.Format("{0:N0}", res[0].cnt_tms);
        //    //        ViewData["TotSubscribers"] = String.Format("{0:N0}", res[0].cnt_tsubs);
        //    //        ViewData["TotDbaseCount"] = String.Format("{0:N0}", res[0].cnt_tdb);
        //    //        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", res[0].cnt_ttc);
        //    //        ViewData["TotClientProfiles"] = String.Format("{0:N0}", res[0].cnt_tcln_a);
        //    //        ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", res[0].cnt_tcln_d);
        //    //    }

        //    //    else
        //    //    {
        //    //        ViewData["TotalSubsDenom"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotalSubsCong"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotalSysPriv"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotalSysRoles"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotSysProfiles"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotSubscribers"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotDbaseCount"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotClientProfiles"] = String.Format("{0:N0}", 0);
        //    //        ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", 0);
        //    //    }
        //    //}

        //}



        private async Task LoadDashboardValues () ///(string strTempConn = "")
        {
            var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);

           // var _cs = strTempConn;  /// string strTempConn = ""
            //if (string.IsNullOrEmpty(_cs))
            //    _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];

            if (!string.IsNullOrEmpty(_cs))
            {
                using (var dashContext = new MSTR_DbContext(_cs))
                {
                    var res = await (from dummyRes in new List<string> { "X" }
                                     join tago in dashContext.MSTRAppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
                                     join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
                                     join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
                                     join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
                                     join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
                                     join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
                                     join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
                                     join tdb in dashContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb
                                     join tcln_a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A") on 1 equals 1 into _tcln_a
                                     join tcln_d in (from a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
                                                     from b in dashContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
                                                     select a) on 1 equals 1 into _tcln_d

                                     select new
                                     {
                                         cnt_tago = _tago.Count(),
                                         cnt_tcb = _tcb.Count(),
                                         cnt_tsr = _tsr.Count(),
                                         cnt_tsp = _tsp.Count(),
                                         cnt_tms = _tms.Count(),
                                         cnt_tsubs = _tsubs.Count(),
                                         cnt_tdb = _tdb.Count(),
                                         cnt_ttc = _ttc.Count(),
                                         cnt_tcln_d = _tcln_d.Count(),
                                         cnt_tcln_a = _tcln_a.Count()
                                     })
                                .ToList().ToListAsync();

                    ///
                    if (res.Count > 0)
                    {
                        ViewData["TotalSubsDenom"] = String.Format("{0:N0}", res[0].cnt_tago);
                        ViewData["TotalSubsCong"] = String.Format("{0:N0}", res[0].cnt_tcb);
                        ViewData["TotalSysPriv"] = String.Format("{0:N0}", res[0].cnt_tsp);
                        ViewData["TotalSysRoles"] = String.Format("{0:N0}", res[0].cnt_tsr);
                        ViewData["TotSysProfiles"] = String.Format("{0:N0}", res[0].cnt_tms);
                        ViewData["TotSubscribers"] = String.Format("{0:N0}", res[0].cnt_tsubs);
                        ViewData["TotDbaseCount"] = String.Format("{0:N0}", res[0].cnt_tdb);
                        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", res[0].cnt_ttc);
                        ViewData["TotClientProfiles"] = String.Format("{0:N0}", res[0].cnt_tcln_a);
                        ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", res[0].cnt_tcln_d);
                    }

                    else
                    {
                        ViewData["TotalSubsDenom"] = String.Format("{0:N0}", 0);
                        ViewData["TotalSubsCong"] = String.Format("{0:N0}", 0);
                        ViewData["TotalSysPriv"] = String.Format("{0:N0}", 0);
                        ViewData["TotalSysRoles"] = String.Format("{0:N0}", 0);
                        ViewData["TotSysProfiles"] = String.Format("{0:N0}", 0);
                        ViewData["TotSubscribers"] = String.Format("{0:N0}", 0);
                        ViewData["TotDbaseCount"] = String.Format("{0:N0}", 0);
                        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);
                        ViewData["TotClientProfiles"] = String.Format("{0:N0}", 0);
                        ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", 0);
                    }
                }
            }
                   
        }



        private static bool IsAncestor_ChurchBody(MSTRChurchBody oAncestorChurchBody, MSTRChurchBody oCurrChurchBody)
        {
            if (oAncestorChurchBody == null || oCurrChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 
            if (oAncestorChurchBody.Id == oCurrChurchBody.ParentChurchBodyId) return true;
            if (string.IsNullOrEmpty(oCurrChurchBody.RootChurchCode)) return false;

            string[] arr = new string[] { oCurrChurchBody.RootChurchCode };
            if (oCurrChurchBody.RootChurchCode.Contains("--")) arr = oCurrChurchBody.RootChurchCode.Split("--");

            if (arr.Length > 0)
            {
                var ancestorCode = oAncestorChurchBody.RootChurchCode;
                var tempCode = oCurrChurchBody.RootChurchCode;
                var k = arr.Length - 1;
                for (var i = arr.Length - 1; i >= 0; i--)
                {
                    if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
                    if (string.Compare(ancestorCode, tempCode) == 0) return true;
                }
            }

            return false;
        }


        private static bool IsAncestor_ChurchBody(string strAncestorRootCode, string strCurrChurchBodyRootCode, int? strAncestorId = null, int? strCurrChurchBodyId = null)
        {
            // if (oAncestorChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

            if (strCurrChurchBodyId != null && strAncestorId == strCurrChurchBodyId) return true; 

            if (string.IsNullOrEmpty(strAncestorRootCode) || string.IsNullOrEmpty(strCurrChurchBodyRootCode)) return false;

            string[] arr = new string[] { strCurrChurchBodyRootCode };
            if (strCurrChurchBodyRootCode.Contains("--")) arr = strCurrChurchBodyRootCode.Split("--");

            if (arr.Length > 0)
            {
                var ancestorCode = strAncestorRootCode;
                var tempCode = strCurrChurchBodyRootCode;
                var k = arr.Length - 1;
                for (var i = arr.Length - 1; i >= 0; i--)
                {
                    if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
                    if (string.Compare(ancestorCode, tempCode) == 0) return true;
                }
            }

            return false;
        }


        private bool userAuthorized = false;
        //private void SetUserLogged()
        //{
        //    ////  oUserLogIn_Priv = TempData.Get<UserSessionPrivilege>("UserLogIn_oUserPrivCol");

        //    //UserSessionPrivilege oUserLogIn_Priv = TempData.ContainsKey("UserLogIn_oUserPrivCol") ?
        //    //                                                TempData["UserLogIn_oUserPrivCol"] as UserSessionPrivilege : null;

        //    if (TempData.ContainsKey("UserLogIn_oUserPrivCol"))
        //    {
        //        var tempPrivList = TempData["UserLogIn_oUserPrivCol"] as string;
        //        if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
        //        // De serialize the string to object
        //        oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);

        //        this.isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
        //        if (isCurrValid)
        //        {
        //            ViewBag.oAppGloOwnLogged = oUserLogIn_Priv.AppGlobalOwner;
        //            ViewBag.oChuBodyLogged = oUserLogIn_Priv.ChurchBody;
        //            ViewBag.oUserLogged = oUserLogIn_Priv.UserProfile;

        //            // check permission for Core life...  given the sets of permissions
        //            userAuthorized = true; //oUserLogIn_Priv.Count > 0; //(oUserLogIn_Priv.Find(x => x.PermissionName == "_A0__System_Administration" || x.PermissionName == "xxx") != null);
        //        }
        //    }
        //    else RedirectToAction("LoginUserAcc", "UserLogin");
                        
        //}

        private bool CheckUserLoggedIn()
        {
            ////  oUserLogIn_Priv = TempData.Get<UserSessionPrivilege>("UserLogIn_oUserPrivCol");

            //UserSessionPrivilege oUserLogIn_Priv = TempData.ContainsKey("UserLogIn_oUserPrivCol") ?
            //                                                TempData["UserLogIn_oUserPrivCol"] as UserSessionPrivilege : null;
            try
            {
                if (TempData.ContainsKey("UserLogIn_oUserPrivCol"))
                {
                    var tempPrivList = TempData["UserLogIn_oUserPrivCol"] as string;
                    if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
                    // De serialize the string to object
                    oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);

                    if (oUserLogIn_Priv != null)
                    {
                        this.isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
                        if (isCurrValid)
                        {
                            ViewBag.oAppGloOwnLogged = oUserLogIn_Priv.AppGlobalOwner;
                            ViewBag.oChuBodyLogged = oUserLogIn_Priv.ChurchBody;
                            ViewBag.oUserLogged = oUserLogIn_Priv.UserProfile;

                            // check permission for Core life...  given the sets of permissions
                            this.userAuthorized = true; //oUserLogIn_Priv.Count > 0; //(oUserLogIn_Priv.Find(x => x.PermissionName == "_A0__System_Administration" || x.PermissionName == "xxx") != null);
                        }
                    }
                    else
                        isCurrValid = false;
                }
                else 
                    isCurrValid = false; // RedirectToAction("LoginUserAcc", "UserLogin");


                return isCurrValid;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }


        private bool InitializeUserLogging(bool loadDash = false)
        {
            try
            {
               //   SetUserLogged();

                if (!CheckUserLoggedIn())
                {
                    ViewData["strUserLoginFailMess"] = "User profile validation unsuccessful.";
                    //RedirectToAction("LoginUserAcc", "UserLogin"); 
                    return false;
                }

                if (!loadDash) return true;
                ///

                if (oUserLogIn_Priv.UserProfile == null)
                {
                    ViewData["strUserLoginFailMess"] = "User profile not found. Please try again or contact System Admin";
                    // RedirectToAction("LoginUserAcc", "UserLogin"); 
                    return false;
                }

                // store login in session 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();

                ///
               // _oLoggedRole = oUserLogIn_Priv.UserRole;
                _oLoggedUser = oUserLogIn_Priv.UserProfile;

               // _oLoggedMSTR_CB = oUserLogIn_Priv.ChurchBody;
               // _oLoggedMSTR_AGO = oUserLogIn_Priv.AppGlobalOwner;
               // _oLoggedUser.strChurchCode_AGO = _oLoggedMSTR_AGO != null ? _oLoggedMSTR_AGO.GlobalChurchCode : "";
               // _oLoggedUser.strChurchCode_CB = _oLoggedMSTR_CB != null ? _oLoggedMSTR_CB.GlobalChurchCode : "";

               // this._context = GetClientDBContext(_oLoggedUser);

               // if (this._context == null)
               // {
               //     ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
               //     // return RedirectToAction("LoginUserAcc", "UserLogin"); 
               //     ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
               //     ///
               //     return false;
               //     // RedirectToAction("Index", "Home");  //return View(oHomeDash);
               // }

               // this._strClientConn = _context.Database.GetDbConnection().ConnectionString;

               // //// store ctx in session 
               // //var _tempContext = this._context;
               // //var _ctx = Newtonsoft.Json.JsonConvert.SerializeObject(_tempContext);
               // //TempData["UserLogIn_oDBContext_Client"] = _ctx; TempData.Keep();

               // /// synchronize AGO, CL, CB, CTRY  or @login 

               // /// get the localized data... using the MSTR data
               // _oLoggedAGO = _context.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == _oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == _oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
               // _oLoggedCB = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
               //                             .Where(c => c.MSTR_AppGlobalOwnerId == _oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == _oLoggedUser.ChurchBodyId &&
               //                                         c.GlobalChurchCode == _oLoggedUser.strChurchCode_CB).FirstOrDefault();

               // if (_oLoggedAGO == null || _oLoggedCB == null)
               // {
               //     ViewData["strUserLoginFailMess"] = "Client Church unit details could not be verified. Please try again or contact System Admin";
               //     ///
               //     // RedirectToAction("LoginUserAcc", "UserLogin"); 
               //     return false;
               // }

                /// master control DB
                ViewData["strAppName"] = "Rhema-CMS";
                ViewData["strAppNameMod"] = "Church Dashboard";
                ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(_oLoggedUser.UserDesc) ? _oLoggedUser.UserDesc : "[Current user]";
               // ViewData["oMSTR_AppGloOwnId_Logged"] = _oLoggedUser.AppGlobalOwnerId;
               // ViewData["oMSTR_ChurchBodyId_Logged"] = _oLoggedUser.ChurchBodyId;

               // ViewData["oCBOrgType_Logged"] = _oLoggedCB.OrgType;  // CH, CN but subscriber may come from other units like Church Office or Church Group HQ

                ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
                ViewData["bl_IsModAccessVAA0"] = oUserLogIn_Priv.IsModAccessVAA0;
                ViewData["bl_IsModAccessDS00"] = oUserLogIn_Priv.IsModAccessDS00;
                ViewData["bl_IsModAccessAC01"] = oUserLogIn_Priv.IsModAccessAC01;
                ViewData["bl_IsModAccessMR02"] = oUserLogIn_Priv.IsModAccessMR02;
                ViewData["bl_IsModAccessCL03"] = oUserLogIn_Priv.IsModAccessCL03;
                ViewData["bl_IsModAccessCA04"] = oUserLogIn_Priv.IsModAccessCA04;
                ViewData["bl_IsModAccessFM05"] = oUserLogIn_Priv.IsModAccessFM05;
                ViewData["bl_IsModAccessRA06"] = oUserLogIn_Priv.IsModAccessRA06;
                ///                
                ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
                ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
                ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
                //  ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc);
                ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

                //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRolesDesc);  // _oLoggedRole.RoleName; // "System Adminitrator";
                //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);  //_oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

                ViewData["strAppCurrUserPhoto_Filename"] = _oLoggedUser.UserPhoto;
                ViewData["strAppLogo_Filename"] = "df_rhema.jpg"; //"~/img_db/df_rhema.jpg"; // oAppGloOwn?.ChurchLogo;
                ///
                /// client control DB
                //  ViewData["oAppGloOwnId_Logged"] = _oLoggedAGO.Id;
                //  ViewData["oChurchBodyId_Logged"] = _oLoggedCB.Id;
                // ViewData["oChurchBodyOrgType_Logged"] = _oLoggedCB.OrgType;
                // ViewData["strClientLogo_Filename"] = _oLoggedAGO?.ChurchLogo;

                //  ViewData["strClientChurchName"] = _oLoggedAGO.OwnerName;
                //  ViewData["strClientBranchName"] = _oLoggedCB.Name;
                // ViewData["strClientChurchLevel"] = !string.IsNullOrEmpty(_oLoggedCB.ChurchLevel?.CustomName) ? _oLoggedCB.ChurchLevel?.CustomName : _oLoggedCB.ChurchLevel?.Name;  // Assembly, Presbytery etc


                // refreshValues...
                _ = LoadDashboardValues();



                return true;
            }

            catch (Exception)
            {
                throw;
            }
        }




        private List<ChurchFaithTypeModel> GetChurchFaithTypes(int categIndex)  
        {  //FS == Faith Sect like Catholism, Protestantism, Pentecostalism/Charismatism, FC == Faith Class like Presbyterian, Methodist, Catholic, Charismatic
            
            var dc = categIndex == 1  ? "FS" : categIndex == 2 ? "FC" : null;             
            var ls = (
                   from t_cft in _context.ChurchFaithType //.Include(t => t.FaithTypeClass).Include(t => t.SubFaithTypes)
                        .Where(c => c.Category == dc || dc == null)
                   from t_cft_c in _context.ChurchFaithType.AsNoTracking().Where(c => c.Id == t_cft.FaithTypeClassId).DefaultIfEmpty()
                   select new ChurchFaithTypeModel()
                   {
                       oChurchFaithType = t_cft,
                       strFaithTypeClass = t_cft_c != null ? t_cft_c.FaithDescription : ""
                   })
                   .OrderBy(c => c.oChurchFaithType.FaithTypeClassId).ThenBy(c => c.oChurchFaithType.FaithDescription)
                   .ToList();
            return ls;
        }

        private List<DenominationVM> GetDenominations()
        {   //return _context.MSTRAppGlobalOwner.ToList();
           return  (
                   from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Include(t => t.FaithTypeCategory).Include(t => t.ChurchLevels) 
                        //.Where(c=> oAppOwnId==null || (oAppOwnId != null && c.Id == oAppOwnId))                                                                                                                                       // from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c=> c.AppGlobalOwnerId==t_ago.Id ).DefaultIfEmpty()
                   from t_ft in _context.ChurchFaithType.AsNoTracking().Where(c => c.Id == t_ago.FaithTypeCategoryId).DefaultIfEmpty()
                   from t_ctr in _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == t_ago.CtryAlpha3Code).DefaultIfEmpty()
                   select new DenominationVM()
                   {
                       oDenomination = t_ago,
                       lsChurchLevels = t_ago.ChurchLevels.ToList(),
                       strAppGloOwn = t_ago != null ? t_ago.OwnerName : "",
                       strCountry = t_ctr != null ? t_ctr.EngName : "",
                       strFaithTypeCategory = t_ft != null ? t_ft.FaithDescription : "",
                   }
                   )
                   .OrderBy(c => c.strCountry).ThenBy(c => c.strFaithTypeCategory).ThenBy(c => c.strAppGloOwn).ToList(); 
        }

        private DenominationVM GetDenomination(int oAppOwnId)
        {   //return _context.MSTRAppGlobalOwner.ToList();
            return (
                    from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Include(t => t.FaithTypeCategory).Include(t => t.ChurchLevels)
                         .Where(c => c.Id == oAppOwnId)                                                                                                                                       // from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c=> c.AppGlobalOwnerId==t_ago.Id ).DefaultIfEmpty()
                    from t_ft in _context.ChurchFaithType.AsNoTracking().Where(c => c.Id == t_ago.FaithTypeCategoryId).DefaultIfEmpty()
                    from t_ctr in _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == t_ago.CtryAlpha3Code).DefaultIfEmpty()
                    select new DenominationVM()
                    {
                        oDenomination = t_ago,
                        lsChurchLevels = t_ago.ChurchLevels.ToList(),
                        strAppGloOwn = t_ago != null ? t_ago.OwnerName : "",
                        strCountry = t_ctr != null ? t_ctr.EngName : "",
                        strFaithTypeCategory = t_ft != null ? t_ft.FaithDescription : "",
                    }
                    ).FirstOrDefault();
        }

        private List<MSTRChurchLevel> GetChurchLevels(int? oAppOwnId)
        {
            return (
                   from t_cl in _context.MSTRChurchLevel.AsNoTracking().Include(t => t.AppGlobalOwner)
                        .Where(c => c.AppGlobalOwnerId == oAppOwnId)
                  // from t_cb_c in _context.MSTRChurchBody.AsNoTracking().Where(c => c.ParentChurchBodyId == t_cl.Id).DefaultIfEmpty()
                   select new MSTRChurchLevel()
                   {
                       Id = t_cl.Id,
                       AppGlobalOwnerId = t_cl.AppGlobalOwnerId,
                       Name = t_cl.Name,
                       CustomName = t_cl.CustomName,
                       LevelIndex = t_cl.LevelIndex,
                       Created = t_cl.Created,
                       LastMod = t_cl.LastMod,
                      // OwnedByChurchBodyId = t_cl.OwnedByChurchBodyId,
                       SharingStatus = t_cl.SharingStatus,
                       //
                       strAppGlobalOwner = t_cl.AppGlobalOwner != null ? t_cl.AppGlobalOwner.OwnerName : ""
                   }
                   ).OrderBy(c => c.AppGlobalOwnerId).ThenBy(c => c.LevelIndex)
                   .ToList();
        }

        private List<ChurchBodyVM> GetCongregations(int? oAppOwnId)  // , int? oParCongId, bool oShowAllCong)
        {
            return (
                   from t_cb in _context.MSTRChurchBody.AsNoTracking()
                       // .Include(t => t.AppGlobalOwner).Include(t => t.ParentChurchBody)
                        .Include(t => t.Country).Include(t => t.SubChurchUnits)
                        .Where(c => c.AppGlobalOwnerId == oAppOwnId) // && (c.OrgType=="GB" || c.OrgType == "CN" && c.ChurchWorkStatus== "O"))  //church unit must be (O) operationalized...
                     //   ((oParCongId == null && oShowAllCong) || (oShowAllCong == false && c.ParentChurchBodyId == null) || c.ParentChurchBodyId == oParCongId))
                   from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_cb.AppGlobalOwnerId).DefaultIfEmpty()
                   from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.Id == t_cb.ChurchLevelId).DefaultIfEmpty()
                   from t_cb_c in _context.MSTRChurchBody.AsNoTracking().Where(c => c.Id == t_cb.ParentChurchBodyId).DefaultIfEmpty()
                   select new ChurchBodyVM()
                   {
                       oChurchBody = t_cb,
                       strAppGloOwn = t_ago != null ? t_ago.OwnerName : "",
                       lsSubChurchBodies = t_cb.SubChurchUnits.ToList(),
                       strParentChurchBody = t_cb_c != null ? t_cb_c.Name : "",
                       strChurchLevel = t_cl != null ? !string.IsNullOrEmpty(t_cl.CustomName) ? t_cl.CustomName : t_cl.Name : "",
                       strCountry = t_cb.Country != null ? t_cb.Country.EngName : "",
                       strCountryRegion = t_cb_c != null ? t_cb_c.Name : "",
                     //  strChurchType = t_cb.ChurchType == "CH" ? "Hierarchy" : "Congregation",
                       /// strAssociationType = t_cb.AssociationType == "N" ? "Networked" : "Freelance"
                   }
                   ).OrderBy(c => c.oChurchBody.ParentChurchBodyId).ThenBy(c => c.oChurchBody.Name).ToList();
        }

        private List<MSTRCountry> GetCountries()
        {
            //return _context.MSTRAppGlobalOwner.ToList();
            return (
                   from t_ctr in _context.MSTRCountry.AsNoTracking() //.Include(t => t.CountryRegions)
                   from t_rgn in _context.MSTRCountryRegion.AsNoTracking().Where(c => c.CtryAlpha3Code == t_ctr.CtryAlpha3Code).DefaultIfEmpty()
                   select t_ctr 

                   //select new Country()
                   //{
                   //    oCountry = t_ctr,
                   //    lsCountryRegions = t_ctr.CountryRegions,
                   //    strCountry = t_ctr != null ? t_ctr.EngName : ""
                   //}

                   ).OrderBy(c => c.EngName).ToList();
        }

        private List<MSTRCountryRegion> GetCountryRegions(string oCtryId)  //(int? oCtryId)
        {  //  return _context.MSTRAppGlobalOwner.ToList();
            return (
                   from t_rgn in _context.MSTRCountryRegion.AsNoTracking().Include(t => t.Country).Where(c => c.CtryAlpha3Code == oCtryId)
                   select new MSTRCountryRegion()
                   {
                       Id = t_rgn.Id,
                       Name = t_rgn.Name,
                       CtryAlpha3Code = t_rgn.CtryAlpha3Code,
                       Created = t_rgn.Created,
                       LastMod = t_rgn.LastMod,
                       RegCode = t_rgn.RegCode,
                     //  OwnedByChurchBodyId = t_rgn.OwnedByChurchBodyId,
                       SharingStatus = t_rgn.SharingStatus,
                                              
                       oCountry = t_rgn != null ? t_rgn.Country : null,
                       strCountry = t_rgn != null ? t_rgn.Country != null ? t_rgn.Country.EngName : "" : ""
                   }
                   ).OrderBy(c => c.oCountry.EngName).ToList();
        }

        public JsonResult GetCountryRegionsByCountry(string ctryId, bool addEmpty = false) //(int? ctryId, bool addEmpty = false)
        {
            var countryList = _context.MSTRCountryRegion.AsNoTracking().Include(t => t.Country)
                .Where(c =>  c.CtryAlpha3Code == ctryId)  //c.Country.Display == true &&
                .OrderBy(c => c.Name)
                .ToList()
            .Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .OrderBy(c => c.Text)
            .ToList();

            /// if (addEmpty) countryList.Insert(0, new CountryRegion { Id = "", Name = "Select" });             
            //return Json(new SelectList(countryList, "Id", "Name"));  

            if (addEmpty) countryList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(countryList);
        }


        private static List<UserPermission> CombineCollection(List<UserPermission> list1, List<UserPermission> list2,
           List<UserPermission> list3 = null, List<UserPermission> list4 = null, List<UserPermission> list5 = null)
        {
            if (list1 != null)
            {
                if (list2 != null) list1.AddRange(list2);
                if (list3 != null) list1.AddRange(list3);
                if (list4 != null) list1.AddRange(list4);
                if (list5 != null) list1.AddRange(list5);
            }
            
            //
            return list1;
        }

        private List<UserProfileModel> _GetUserProfileList(int? oAppGloOwnId = null, int ? oChurchBodyId = null, string proScope = "V", string subScope = "")//, int? roleLevel = -1)    profileCode : -1 = V /SUP_ADMN | 0 = V /other users  || 1 = C /??_ADMN || 2 = C / other users
        {
           // var p = _context.UserProfile.AsNoTracking().Where(c => c.ChurchBodyId == null && c.ProfileScope == "V");

            //var profiles = new List<UserProfileVM>();

            // null CB means ... SUPER USER .. get all accounts at toplevel  
            // null CB means ... SUPER USER .. get all accounts at toplevel  
            var profiles = (
               from t_up in _context.UserProfile.Include(t => t.ContactInfo).AsNoTracking().Where(c => c.ProfileScope == proScope && ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) ||
                                                                              (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == null && c.AppGlobalOwnerId != null) ||
                                                                              (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId && c.AppGlobalOwnerId != null && c.ChurchBodyId != null)))  //.Include(t => t.ChurchMember)   "V"  proScope == "C" && subScope == "D"
                   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                                ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                                 ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                                 ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                                )).DefaultIfEmpty()

                       // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                       //from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                       //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                       //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                       //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                       //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                       //from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                       //             .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                       //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                   select new UserProfileModel()
               {
                       // oUserProfile = t_up,

                       oUserProfile = new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_up.ChurchBody,
                       // ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,
                       strChurchCode_AGO = t_cb != null ? (t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.GlobalChurchCode : "") : "",
                       strChurchCode_CB = t_cb != null ? t_cb.GlobalChurchCode : "",

                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc,
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                           // PhoneNum = t_up.ContactInfo != null ? t_up.ContactInfo.MobilePhone1 : "", //t_up.PhoneNum,
                           Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",  // yyyy-MM-dd hh:mm tt
                       OwnerUserId = t_up.OwnerUserId,
                       // UserId = t_up.UserId,
                       UserScope = t_up.UserScope,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)

                   },

                       //  lsUserGroups = t_upg.UserGroups,
                       // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                       // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                       strUserProfile = t_up.UserDesc,
                       strChurchBody = t_cb != null ? t_cb.Name : "",
                       strAppGlobalOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",

                       //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                       // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
                   }
               )
               //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
               .Distinct()
               .ToList(); 

            return profiles;
        }

        private List<UserProfileModel> GetUserProfileList_SysAdmin() //int? oAppGloOwnId = null, int? oChurchBodyId = null, string proScope = "V", string subScope = "" ... , int? roleLevel = -1)    profileCode : -1 = V /SUP_ADMN | 0 = V /other users  || 1 = C /??_ADMN || 2 = C / other users
        {
           // int? oAppGloOwnId = null; int? oChurchBodyId = null; 
            string proScope = "V"; 
            // null CB means ... SUPER USER .. get all accounts at toplevel...

            var profiles = (
               from t_up in _context.UserProfile.AsNoTracking().Include(t => t.ContactInfo).AsNoTracking().Where(c => c.ProfileScope == proScope && c.AppGlobalOwnerId == null && c.ChurchBodyId == null)
                                                                    // ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) ||
                                                                    //  (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == null && c.AppGlobalOwnerId != null) ||
                                                                    //  (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId && c.AppGlobalOwnerId != null && c.ChurchBodyId != null)))  //.Include(t => t.ChurchMember)   "V"  proScope == "C" && subScope == "D"
               from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
               from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                            .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                                        (proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5))                          
                            ).DefaultIfEmpty()

                   // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                   //from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                   //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                   //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                   //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                   //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                   //from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                   //             .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                   //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                   //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

               select new UserProfileModel()
               {
                   // oUserProfile = t_up,

                   oUserProfile = new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_up.ChurchBody,
                       // ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,
                       oUserRole = t_upr.UserRole,
                       strChurchCode_AGO = t_cb != null ? (t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.GlobalChurchCode : "") : "",
                       strChurchCode_CB = t_cb != null ? t_cb.GlobalChurchCode : "",

                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc,
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                       // PhoneNum = t_up.ContactInfo != null ? t_up.ContactInfo.MobilePhone1 : "", //t_up.PhoneNum,
                       Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                       OwnerUserId = t_up.OwnerUserId,
                    //   UserId = t_up.UserId,
                       UserScope = t_up.UserScope,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       ProfileLevel = t_up.ProfileLevel,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)

                   },

                   //  lsUserGroups = t_upg.UserGroups,
                   // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                   // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                   strUserProfile = t_up.UserDesc,
                   strChurchBody = t_cb != null ? t_cb.Name : "",
                   strAppGlobalOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",

                   //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                   // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
               }
               )
               //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
               .Distinct()
               .ToList();

            return profiles;
        }

        private List<UserProfileModel> GetUserProfileList_ChuAdmin(string subScope = "", int? oAppGloOwnId = null)//, int? roleLevel = -1)    profileCode : -1 = V /SUP_ADMN | 0 = V /other users  || 1 = C /??_ADMN || 2 = C / other users
        {
            var proScope = "C";

            // null CB means ... SUPER USER .. get all accounts at toplevel  
            // null CB means ... SUPER USER .. get all accounts at toplevel  
            var profiles = (
               from t_up in _context.UserProfile.AsNoTracking().Include(t => t.ContactInfo)
                    .Where(c => (oAppGloOwnId==null || (oAppGloOwnId != null && c.AppGlobalOwnerId== oAppGloOwnId)) && c.ProfileScope == proScope) 
               
               // && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId )
                                                                             // (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == null && c.AppGlobalOwnerId != null) ||
                                                                             // (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId && c.AppGlobalOwnerId != null && c.ChurchBodyId != null)))  //.Include(t => t.ChurchMember)   "V"  proScope == "C" && subScope == "D"
              
               from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
               from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&
                           // ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                             ((proScope == "C" && subScope == "D" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")) && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                             ((proScope == "C" && subScope == "A" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST")) && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                            ).DefaultIfEmpty()

                   // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                   //from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                   //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                   //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                   //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                   //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                   //from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                   //             .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                   //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                   //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

               select new UserProfileModel()
               {
                   // oUserProfile = t_up,

                   oUserProfile = new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_up.ChurchBody,
                       // ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,
                       oUserRole = t_upr.UserRole,
                       strChurchCode_AGO = t_cb != null ? (t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.GlobalChurchCode : "") : "",
                       strChurchCode_CB = t_cb != null ? t_cb.GlobalChurchCode : "",

                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc,
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                       // PhoneNum = t_up.ContactInfo != null ? t_up.ContactInfo.MobilePhone1 : "", //t_up.PhoneNum,
                       Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                       OwnerUserId = t_up.OwnerUserId,
                      // UserId = t_up.UserId,
                       UserScope = t_up.UserScope,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       ProfileLevel = t_up.ProfileLevel,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)

                   },

                   //  lsUserGroups = t_upg.UserGroups,
                   // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                   // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                   strUserProfile = t_up.UserDesc,
                   strChurchBody = t_cb != null ? t_cb.Name : "",
                   strAppGlobalOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",

                   //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                   // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
               }
               )
               //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
               .Distinct()
               .ToList();

            return profiles;
        }

        private List<UserProfileModel> GetUserProfileList_ClientChuAdmin(int? oAppGloOwnId, int? oChurchBodyId)//, string subScope = "", int? roleLevel = -1)    profileCode : -1 = V /SUP_ADMN | 0 = V /other users  || 1 = C /??_ADMN || 2 = C / other users
        {
            //checking users at the level of the subscriber...

            var proScope = "C"; var subScope = "A";

            // null CB means ... SUPER USER .. get all accounts at toplevel  
            // null CB means ... SUPER USER .. get all accounts at toplevel  
            var profiles = (
               from t_up in _context.UserProfile.AsNoTracking().Include(t => t.ContactInfo).AsNoTracking().Where(c => c.ProfileScope == proScope && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId)
                   // (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == null && c.AppGlobalOwnerId != null) ||
                   // (c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId && c.AppGlobalOwnerId != null && c.ChurchBodyId != null)))  //.Include(t => t.ChurchMember)   "V"  proScope == "C" && subScope == "D"
               from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
               from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&
                             // ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                             // ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                            
                            ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                            ).DefaultIfEmpty()

                   // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                   //from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                   //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                   //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                   //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                   //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                   //from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                   //             .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                   //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                   //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

               select new UserProfileModel()
               {
                   // oUserProfile = t_up,

                   oUserProfile = new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_up.ChurchBody,
                       // ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,

                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc,
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                       // PhoneNum = t_up.ContactInfo != null ? t_up.ContactInfo.MobilePhone1 : "", //t_up.PhoneNum,
                       Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                       OwnerUserId = t_up.OwnerUserId,
                     //  UserId = t_up.UserUserId,
                       UserScope = t_up.UserScope,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)

                   },

                   //  lsUserGroups = t_upg.UserGroups,
                   // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                   // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                   strUserProfile = t_up.UserDesc,
                   strChurchBody = t_cb != null ? t_cb.Name : "",
                   strAppGlobalOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",

                   //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                   // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
               }
               )
               //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
               .Distinct()
               .ToList();

            return profiles;
        }

        private List<UserProfileVM> GetUserProfiles(int? oDenomId = null, int ? oChurchBodyId = null, string proScope = "V", string subScope = "")//, int? roleLevel = -1)    profileCode : -1 = V /SUP_ADMN | 0 = V /other users  || 1 = C /??_ADMN || 2 = C / other users
        {
           // var p = _context.UserProfile.AsNoTracking().Where(c => c.ChurchBodyId == null && c.ProfileScope == "V");

            var profiles = new List<UserProfileVM>();

            // null CB means ... SUPER USER .. get all accounts at toplevel  
            profiles = (
                   from t_up in _context.UserProfile.Include(t=>t.ContactInfo).AsNoTracking().Where(c =>c.ProfileScope == proScope && ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || 
                                                                                (c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == null && c.AppGlobalOwnerId != null) || 
                                                                                (c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == oChurchBodyId && c.AppGlobalOwnerId != null && c.ChurchBodyId != null)))  //.Include(t => t.ChurchMember)   "V"  proScope == "C" && subScope == "D"
                   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                    .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                                    ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                                     ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                                     ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                                    )).DefaultIfEmpty()

                       // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                       //from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                       //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                       //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                       //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                       //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                       //from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                       //             .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                       //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                   select new UserProfileVM()
                   {
                       // oUserProfile = t_up,

                       oUserProfile = new UserProfile()
                       {
                           Id = t_up.Id,
                           AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                           ChurchBodyId = t_up.ChurchBodyId,
                           ChurchMemberId = t_up.ChurchMemberId,
                           ChurchBody = t_up.ChurchBody,
                           // ChurchMember = t_up.ChurchMember,
                           OwnerUser = t_up.OwnerUser,

                           Username = t_up.Username,
                           UserDesc = t_up.UserDesc,
                           Email = t_up.Email,
                           ContactInfo = t_up.ContactInfo,
                          // PhoneNum = t_up.ContactInfo != null ? t_up.ContactInfo.MobilePhone1 : "", //t_up.PhoneNum,
                           Pwd = t_up.Pwd,
                           PwdExpr = t_up.PwdExpr,
                           PwdSecurityQue = t_up.PwdSecurityQue,
                           PwdSecurityAns = t_up.PwdSecurityAns,
                           ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                           Strt = t_up.Strt,
                           strStrt = t_up.strStrt,
                           Expr = t_up.Expr,
                           strExpr = t_up.Expr != null ?
                                                                DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                           OwnerUserId = t_up.OwnerUserId,
                         //  UserId = t_up.UserId,
                           UserScope = t_up.UserScope,
                           UserPhoto = t_up.UserPhoto,
                           ProfileScope = t_up.ProfileScope,
                           Created = t_up.Created,
                           CreatedByUserId = t_up.CreatedByUserId,
                           LastMod = t_up.LastMod,
                           LastModByUserId = t_up.LastModByUserId,
                           UserStatus = t_up.UserStatus,
                           strUserStatus = GetStatusDesc(t_up.UserStatus)

                       },

                       //  lsUserGroups = t_upg.UserGroups,
                       // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                       // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                       strUserProfile = t_up.UserDesc,
                       strChurchBody = t_cb != null ? t_cb.Name : "",
                       strAppGloOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",

                       //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                       // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
                   }
                   )
                   //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
                   .Distinct()
                   .ToList();



            //if (proScope == "V")  // // null CB means ... SUPER USER .. get all accounts at toplevel  
            //{ 
            //    profiles = (
            //       from t_up in _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId==null && c.ChurchBodyId == null && c.ProfileScope == "V")  //.Include(t => t.ChurchMember)
            //       from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
            //       from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
            //                        .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id  &&
            //                        ((c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5))                                    
            //                        ).DefaultIfEmpty()

            //      // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
            //       //from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
            //       //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
            //       //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
            //       //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
            //       //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
            //       //from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
            //       //             .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
            //       //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
            //       //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

            //       select new UserProfileVM()
            //       {
            //          // oUserProfile = t_up,

            //           oUserProfile = new UserProfile()
            //           {
            //               Id = t_up.Id,
            //               AppGlobalOwnerId = t_up.AppGlobalOwnerId,
            //               ChurchBodyId = t_up.ChurchBodyId,
            //               ChurchMemberId = t_up.ChurchMemberId,
            //               ChurchBody = t_up.ChurchBody,
            //               ChurchMember = t_up.ChurchMember,
            //               Owner = t_up.Owner,

            //               Username = t_up.Username,
            //               UserDesc = t_up.UserDesc,
            //               Email = t_up.Email,
            //               PhoneNum = t_up.PhoneNum,
            //               Pwd = t_up.Pwd,
            //               PwdExpr = t_up.PwdExpr,
            //               PwdSecurityQue = t_up.PwdSecurityQue,
            //               PwdSecurityAns = t_up.PwdSecurityAns,
            //               ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
            //               Strt = t_up.Strt,
            //               strStrt = t_up.strStrt,
            //               Expr = t_up.Expr,
            //               strExpr = t_up.Expr != null ?
            //                                                    DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
            //               OwnerId = t_up.OwnerId,
            //               UserId = t_up.UserId,
            //               UserScope = t_up.UserScope,
            //               UserPhoto = t_up.UserPhoto,
            //               ProfileScope = t_up.ProfileScope,
            //               Created = t_up.Created,
            //               CreatedByUserId = t_up.CreatedByUserId,
            //               LastMod = t_up.LastMod,
            //               LastModByUserId = t_up.LastModByUserId,
            //               UserStatus = t_up.UserStatus,
            //               strUserStatus = GetStatusDesc(t_up.UserStatus)

            //           },

            //         //  lsUserGroups = t_upg.UserGroups,
            //         // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
            //         // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

            //             strUserProfile = t_up.UserDesc,
            //             strChurchBody = t_cb != null ? t_cb.Name : "",
            //             strAppGloOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",

            //           //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
            //           // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
            //       }
            //       )
            //       //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
            //       .Distinct()
            //       .ToList() ;
            //}

            //else if (proScope == "C" && subScope == "D")  //Administrative account
            //{
            //    profiles = (
            //      from t_up in _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId==oDenomId &&  c.ProfileScope == "C")  //.Include(t => t.ChurchMember)
            //       from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
            //       from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
            //           // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
            //       from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
            //                        (c.RoleType == "CH_ADMN" || c.RoleType == "CF_ADMN") && (c.RoleLevel >= 6 && c.RoleLevel <= 10))
            //           //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
            //       from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
            //                   .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
            //      from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
            //                   .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
            //      from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
            //                   .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

            //      select new UserProfileVM()
            //      {
            //           // oUserProfile = t_up,

            //           oUserProfile = new UserProfile()
            //          {
            //              Id = t_up.Id,
            //              AppGlobalOwnerId = t_up.AppGlobalOwnerId,
            //              ChurchBodyId = t_up.ChurchBodyId,
            //              ChurchMemberId = t_up.ChurchMemberId,
            //              ChurchBody = t_up.ChurchBody,
            //              ChurchMember = t_up.ChurchMember,
            //              Owner = t_up.Owner,

            //              Username = t_up.Username,
            //              UserDesc = t_up.UserDesc,
            //              Email = t_up.Email,
            //              PhoneNum = t_up.PhoneNum,
            //              Pwd = t_up.Pwd,
            //              PwdExpr = t_up.PwdExpr,
            //              PwdSecurityQue = t_up.PwdSecurityQue,
            //              PwdSecurityAns = t_up.PwdSecurityAns,
            //              ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
            //              Strt = t_up.Strt,
            //              strStrt = t_up.strStrt,
            //              Expr = t_up.Expr,
            //              strExpr = t_up.Expr != null ?
            //                                                   DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
            //              OwnerId = t_up.OwnerId,
            //              UserId = t_up.UserId,
            //              UserScope = t_up.UserScope,
            //              UserPhoto = t_up.UserPhoto,
            //              ProfileScope = t_up.ProfileScope,
            //              Created = t_up.Created,
            //              CreatedByUserId = t_up.CreatedByUserId,
            //              LastMod = t_up.LastMod,
            //              LastModByUserId = t_up.LastModByUserId,
            //              UserStatus = t_up.UserStatus,
            //              strUserStatus = GetStatusDesc(t_up.UserStatus)

            //          },

            //           //  lsUserGroups = t_upg.UserGroups,
            //           // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
            //           // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

            //           strUserProfile = t_up.UserDesc,

            //          strChurchBody = t_cb != null ? t_cb.Name : "",
            //          strAppGloOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
            //           //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
            //           // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
            //       }
            //      )
            //      //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
            //      .ToList();
            //}

            //else if (proScope == "C" && subScope == "A")   //all clients accounts... custom created
            //{
            //    profiles = (
            //     from t_up in _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == oDenomId && c.ProfileScope == "C")  //.Include(t => t.ChurchMember)
            //      from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
            //      from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
            //          // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
            //      from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
            //                       (c.RoleType == "CH_ADMN" || c.RoleType == "CH_RGSTR" || c.RoleType == "CH_ACCT" || c.RoleType == "CH_CUST" || c.RoleType == "CF_ADMN" || c.RoleType == "CF_RGSTR" || c.RoleType == "CF_ACCT" || c.RoleType == "CF_CUST") && (c.RoleLevel >= 6 && c.RoleLevel <= 10))
            //          //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
            //      from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
            //                  .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
            //     from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
            //                  .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
            //     from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
            //                  .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

            //     select new UserProfileVM()
            //     {
            //          // oUserProfile = t_up,

            //          oUserProfile = new UserProfile()
            //         {
            //             Id = t_up.Id,
            //             AppGlobalOwnerId = t_up.AppGlobalOwnerId,
            //             ChurchBodyId = t_up.ChurchBodyId,
            //             ChurchMemberId = t_up.ChurchMemberId,
            //             ChurchBody = t_up.ChurchBody,
            //             ChurchMember = t_up.ChurchMember,
            //             Owner = t_up.Owner,

            //             Username = t_up.Username,
            //             UserDesc = t_up.UserDesc,
            //             Email = t_up.Email,
            //             PhoneNum = t_up.PhoneNum,
            //             Pwd = t_up.Pwd,
            //             PwdExpr = t_up.PwdExpr,
            //             PwdSecurityQue = t_up.PwdSecurityQue,
            //             PwdSecurityAns = t_up.PwdSecurityAns,
            //             ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
            //             Strt = t_up.Strt,
            //             strStrt = t_up.strStrt,
            //             Expr = t_up.Expr,
            //             strExpr = t_up.Expr != null ?
            //                                                  DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
            //             OwnerId = t_up.OwnerId,
            //             UserId = t_up.UserId,
            //             UserScope = t_up.UserScope,
            //             UserPhoto = t_up.UserPhoto,
            //             ProfileScope = t_up.ProfileScope,
            //             Created = t_up.Created,
            //             CreatedByUserId = t_up.CreatedByUserId,
            //             LastMod = t_up.LastMod,
            //             LastModByUserId = t_up.LastModByUserId,
            //             UserStatus = t_up.UserStatus,
            //             strUserStatus = GetStatusDesc(t_up.UserStatus)

            //         },

            //          //  lsUserGroups = t_upg.UserGroups,
            //          // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
            //          // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

            //          strUserProfile = t_up.UserDesc,

            //         strChurchBody = t_cb != null ? t_cb.Name : "",
            //         strAppGloOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
            //          //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
            //          // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
            //      }
            //     )
            //     //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
            //     .ToList();
            //}

            //    //else
            //    //    profiles =  new List<UserProfileVM>();  //jux an empty list


                return profiles;
        }

       

        public ActionResult Index_CFT(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            // //SetUserLogged();

            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin");  // if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                // check permission 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                //
                if (!this.userAuthorized) return View(new ChurchFaithTypeModel()); //retain view    
                if (oUserLogIn_Priv == null) return View(new ChurchFaithTypeModel());
                if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) 
                    return View(new ChurchFaithTypeModel());
                var oLoggedUser = oUserLogIn_Priv.UserProfile;
               // var oLoggedRole = oUserLogIn_Priv.UserRole;

                //
                var strDesc = subSetIndex == 1 ? "Faith stream" : (subSetIndex == 2 ? "Faith category" : "All Faith type");
                var _userTask = "Viewed " + strDesc + " list";
                var oCFTModel = new ChurchFaithTypeModel(); //TempData.Keep();  
                                                            // int? oAppGloOwnId = null;
                var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
                //
                int? oAppGloOwnId_Logged = null;
                int? oChuBodyId_Logged = null;
                if (oChuBody_Logged != null)
                {
                    oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
                    oChuBodyId_Logged = oChuBody_Logged.Id;

                    if (oCurrChuBodyId == null) { oCurrChuBodyId = oChuBody_Logged.Id; }
                    if (oAppGloOwnId == null) { oAppGloOwnId = oChuBody_Logged.AppGlobalOwnerId; }
                    //else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...
                    //
                    // oAppGloOwnId = oCurrChuBodyLogOn.AppGlobalOwnerId;
                }

                //int? oCurrChuMemberId_LogOn = null;
                //ChurchMember oCurrChuMember_LogOn = null;

                //var currChurchMemberLogged = _clientContext.ChurchMember.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Id == oUserProfile.ChurchMemberId).FirstOrDefault();
                //if (currChurchMemberLogged != null) //return View(oCFTModel);
                //{
                //    oCurrChuMemberId_LogOn = currChurchMemberLogged.Id;
                //    oCurrChuMember_LogOn = currChurchMemberLogged;
                //}

                var oUserId_Logged = oLoggedUser.Id;

                oCFTModel.lsChurchFaithTypeModels = GetChurchFaithTypes(subSetIndex);
                oCFTModel.strCurrTask = strDesc;

                //                
                oCFTModel.oAppGloOwnId = oAppGloOwnId;
                oCFTModel.oChurchBodyId = oCurrChuBodyId;
                //
                oCFTModel.oUserId_Logged = oUserId_Logged;
                oCFTModel.oChurchBody_Logged = oChuBody_Logged;
                oCFTModel.oAppGloOwnId_Logged = oAppGloOwnId_Logged;

                // oCFTModel.oMemberId_Logged = oCurrChuMemberId_LogOn;
                //
                oCFTModel.setIndex = setIndex;
                oCFTModel.subSetIndex = subSetIndex;

                ////
                /////
                //ViewData["strAppName"] = "RhemaCMS";
                //ViewData["strAppNameMod"] = "Admin Palette";
                //ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(oLoggedUser.UserDesc) ? oLoggedUser.UserDesc : "[Current user]";
                /////
                //ViewData["oAppGloOwnId_Logged"] = oAppGloOwnId_Logged;
                //ViewData["oChuBodyId_Logged"] = oChuBodyId_Logged;
                //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);  // "System Adminitrator";
                //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);  // // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
                //ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
                /////

                //_ = LoadDashboardValues();


                var tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oLoggedUser.Id, tm, tm, oLoggedUser.Id, oLoggedUser.Id));

                return View(oCFTModel);
            }
        }

        [HttpGet]
        public IActionResult AddOrEdit_CFT(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0,
                                                 int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); // if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {

                var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                var strDesc = subSetIndex == 1 ? "Faith stream" : (subSetIndex == 2 ? "Faith category" : "All Faith type");
                var _userTask = "Attempted accessing/modifying "  + strDesc; // _userTask = "Attempted creating new Faith stream"; // _userTask = "Opened Faith stream-" + oCFT_MDL.oChurchFaithType.FaithDescription;
                if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                {
                    var oCFT_MDL = new ChurchFaithTypeModel();
                    if (id == 0)
                    {
                        //create user and init... 
                        oCFT_MDL.oChurchFaithType = new ChurchFaithType();
                        oCFT_MDL.oChurchFaithType.Level = subSetIndex;  //subSetIndex == 2 ? 1 : 2; // 1;
                        oCFT_MDL.oChurchFaithType.Category = subSetIndex == 1 ? "FS" : "FC";

                        //if (setIndex > 0) oCFT_MDL.oChurchFaithType.Category = setIndex == 1 ? "FS" : "FC";
                        _userTask = "Attempted creating new " + strDesc + ", " + oCFT_MDL.oChurchFaithType.FaithDescription;
                    }

                    else
                    {
                        oCFT_MDL = (
                             from t_cft in _context.ChurchFaithType.AsNoTracking() //.Include(t => t.FaithTypeClass)
                                 .Where(x => x.Id == id)
                             from t_cft_c in _context.ChurchFaithType.AsNoTracking().Where(c => c.Id == t_cft.FaithTypeClassId).DefaultIfEmpty()
                             select new ChurchFaithTypeModel()
                             {
                                 oChurchFaithType = t_cft,
                                 strFaithTypeClass = t_cft_c != null ? t_cft_c.FaithDescription : ""                                 
                             }) 
                             .FirstOrDefault();
                         

                        if (oCFT_MDL == null)
                        {
                            
                            return PartialView("_ErrorPage");
                        }

                        _userTask = "Opened " + strDesc + ", " + oCFT_MDL.oChurchFaithType.FaithDescription;
                    }
                                       
                    if (oCFT_MDL.oChurchFaithType == null) return null;

                    oCFT_MDL.setIndex = setIndex;
                    oCFT_MDL.subSetIndex = subSetIndex;
                    oCFT_MDL.oUserId_Logged = oUserId_Logged;   
                    oCFT_MDL.oAppGloOwnId_Logged = oAGOId_Logged;
                    oCFT_MDL.oChurchBodyId_Logged = oCBId_Logged;
                    //
                    oCFT_MDL.oAppGloOwnId = oAppGloOwnId;
                    oCFT_MDL.oChurchBodyId = oCurrChuBodyId;
                    var oCurrChuBody = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                    oCFT_MDL.oChurchBody = oCurrChuBody != null ? oCurrChuBody : null;

                    if (oCFT_MDL.subSetIndex == 2) // Denomination classes av church sects
                        oCFT_MDL = this.popLookups_CFT(oCFT_MDL, oCFT_MDL.oChurchFaithType);

                    var tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oUserId_Logged, tm, tm, oUserId_Logged, oUserId_Logged));

                    var _oCFT_MDL = Newtonsoft.Json.JsonConvert.SerializeObject(oCFT_MDL);
                    TempData["oVmCurrMod"] = _oCFT_MDL; TempData.Keep();
                      
                    return PartialView("_AddOrEdit_CFT", oCFT_MDL);                     
                }

                //page not found error
                
                return PartialView("_ErrorPage");
            }
        }

        public ChurchFaithTypeModel popLookups_CFT(ChurchFaithTypeModel vm, ChurchFaithType oCurrCFT)
        {
            if (vm != null)
            {
                vm.lkpFaithTypeClasses = _context.ChurchFaithType.AsNoTracking().Where(c => c.Id != oCurrCFT.Id && c.Category == "FS" && !string.IsNullOrEmpty(c.FaithDescription))
                                              .OrderBy(c => c.FaithDescription).ToList()
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = c.FaithDescription
                                              })
                                              .ToList();

                vm.lkpFaithTypeClasses.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            }

            return vm;
        }

        public ChurchFaithTypeModel Get_AddOrEdit_CFT(int? oDenomId = null, int? oCurrChuBodyId=null, int id = 0, int setIndex = 0, int subSetIndex = 0)
        {
            if (setIndex == 0) return null;  //oCFT_MDL; oCFT_MDL.setIndex = setIndex;                
                                             //
            //var oCurrChuBodyLogOn = oUserLogIn_Priv.ChurchBody;
            //var oUserProfile = oUserLogIn_Priv.UserProfile;
            //if (oCurrChuBodyLogOn == null) return null;   //prompt!
            //if (oCurrChuBodyId == null) oCurrChuBodyId = oCurrChuBodyLogOn.Id;
            //else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...

            //// check permission for Core life...
            //if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) //prompt!
            //    return null;

            //int? oCurrChuMemberId_LogOn = null;
            //ChurchMember oCurrChuMember_LogOn = null;
            //if (oUserProfile == null) return null;
            //if (oUserProfile.ChurchMember == null) return null;

            //oCurrChuMemberId_LogOn = oUserProfile.ChurchMember.Id;
            //oCurrChuMember_LogOn = oUserProfile.ChurchMember;

            var oCFT_MDL = new ChurchFaithTypeModel();
            if (id == 0)
            {
                //create user and init... 
                oCFT_MDL.oChurchFaithType = new ChurchFaithType();
                oCFT_MDL.oChurchFaithType.Level = subSetIndex;  //subSetIndex == 2 ? 1 : 2; // 1;
                oCFT_MDL.oChurchFaithType.Category = subSetIndex == 1 ? "FS" : "FC"; 

                //if (setIndex > 0) oCFT_MDL.oChurchFaithType.Category = setIndex == 1 ? "FS" : "FC";
            }

            else
            {
                oCFT_MDL = (
                     from t_cft in _context.ChurchFaithType.AsNoTracking().Include(t => t.FaithTypeClass)
                         .Where(x => x.Id == id)
                     select new ChurchFaithTypeModel()
                     {
                         oChurchFaithType = t_cft,
                         strFaithTypeClass = t_cft.FaithTypeClass == null ? t_cft.FaithTypeClass.FaithDescription : ""
                     }
                    ).FirstOrDefault();
            }

            if (oCFT_MDL.oChurchFaithType == null) return null;

            oCFT_MDL.setIndex = setIndex;
          //  oCFT_MDL.oCurrAppGlobalOwner = oCurrChuBodyLogOn.AppGlobalOwner;
          //  oCFT_MDL.oChurchBody = oCurrChuBodyLogOn;
          //  oCFT_MDL.oCurrLoggedMember = oCurrChuMember_LogOn;
          //  oCFT_MDL.oCurrLoggedMemberId = oCurrChuMemberId_LogOn;

            if (oCFT_MDL.setIndex == 2) // Denomination classes av church sects
                oCFT_MDL = this.popLookups_CFT(oCFT_MDL, oCFT_MDL.oChurchFaithType);


            var _oCFT_MDL = Newtonsoft.Json.JsonConvert.SerializeObject(oCFT_MDL);
            TempData["oVmCurrMod"] = _oCFT_MDL; TempData.Keep();


            //TempData["oVmCurr"] = oCFT_MDL;
            //TempData.Keep();


           // return PartialView("_AddOrEdit_CFT", oCFT_MDL);
             return oCFT_MDL;

        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_CFT(ChurchFaithTypeModel vmMod)
        {

            ChurchFaithType _oChanges = vmMod.oChurchFaithType;
            //   vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vmMod; TempData.Keep();

            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchFaithTypeModel>(arrData) : vmMod;

            var oCFT = vmMod.oChurchFaithType;
            // oCFT.ChurchBody = vmMod.oChurchBody;

           /// var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];

            try
            {
               // ModelState.Remove("oChurchFaithType.AppGlobalOwnerId");
               // ModelState.Remove("oChurchFaithType.ChurchBodyId");
                ModelState.Remove("oChurchFaithType.FaithTypeClassId");
                ModelState.Remove("oChurchFaithType.CreatedByUserId");
                ModelState.Remove("oChurchFaithType.LastModByUserId");
               // ModelState.Remove("oChurchFaithType.OwnerId");
               // ModelState.Remove("oChurchFaithType.UserId");

                // ChurchBody == null 

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", signOutToLogIn = false });

                var strDesc = _oChanges.Category == "FS" ? "Faith stream" : "Faith category";
                if (string.IsNullOrEmpty(_oChanges.FaithDescription)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide description for " + strDesc.ToLower(), signOutToLogIn = false });
                }
                if (_oChanges.Category == "FC" && _oChanges.FaithTypeClassId==null)  // you can create 'Others' to cater for non-category
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the church faith stream.", signOutToLogIn = false });
                }
                 
                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.LastModByUserId = vmMod.oUserId_Logged;

                var _reset = _oChanges.Id == 0;

                //validate...
                var _userTask = "Attempted saving " + strDesc.ToLower() + ", " + _oChanges.FaithDescription.ToUpper();  // _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.FaithDescription.ToUpper() + " successfully";   //  _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.FaithDescription.ToUpper() + " successfully";

                //using (var _cftCtx = new MSTR_DbContext(_cs))
                //{
                    if (_oChanges.Id == 0)
                    {
                        var existCFT = _context.ChurchFaithType.AsNoTracking().Where(c => c.FaithDescription.ToLower() == _oChanges.FaithDescription.ToLower() && c.Level == _oChanges.Level).ToList();
                        if (existCFT.Count() > 0)
                        {
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = strDesc + " [" + _oChanges.FaithDescription + "] already exist.", signOutToLogIn = false });
                        }

                        _oChanges.Created = tm;
                        _oChanges.CreatedByUserId = vmMod.oUserId_Logged;

                        _context.Add(_oChanges);

                        ViewBag.UserMsg = "Saved " + strDesc.ToLower() + ", " + (!string.IsNullOrEmpty(_oChanges.FaithDescription) ? " [" + _oChanges.FaithDescription + "]" : "") + " successfully.";
                        _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.FaithDescription.ToUpper() + " successfully";  
                    }

                    else
                    {
                        var existCFT = _context.ChurchFaithType.AsNoTracking().Where(c => c.Id != _oChanges.Id && c.FaithDescription.ToLower() == _oChanges.FaithDescription.ToLower() && c.Level == _oChanges.Level).ToList();
                        if (existCFT.Count() > 0)
                        {
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = strDesc + " [" + _oChanges.FaithDescription + "] already exist.", signOutToLogIn = false });
                        }

                        //retain the pwd details... hidden fields
                        _context.Update(_oChanges);

                        //var _strDesc = strDesc.Length > 0 ? strDesc.Substring(0, 1).ToUpper() + strDesc.Substring(1) : "Church faith type ";

                        ViewBag.UserMsg = strDesc + " updated successfully.";
                        _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.FaithDescription.ToUpper() + " successfully";
                    }

                    //save church faith type first... 
                    await _context.SaveChangesAsync();
                     

                //    DetachAllEntities(_cftCtx);
                //}

               

                //audit...
                var _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: Faith stream", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vmMod.oCurrUserId_Logged, _tm, _tm, vmMod.oCurrUserId_Logged, vmMod.oCurrUserId_Logged));


                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurr"] = _vmMod; TempData.Keep(); 
                 
                return Json(new { taskSuccess = true,  oCurrId = _oChanges.Id, resetNew = _reset, userMess = ViewBag.UserMsg, signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving church faith type details. Err: " + ex.Message, signOutToLogIn = false });
            }
        }

        public IActionResult Delete_CFT(int? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
        {
           /// var  _cs = this._configuration["ConnectionStrings:DefaultConnection"];
            var strDesc = subSetIndex == 1 ? "Faith stream" : "Faith category"; 
            // 
            var tm = DateTime.Now; var _tm = DateTime.Now; var _userTask = "Attempted saving " + strDesc;

            try
            {                
                var oCFT = _context.ChurchFaithType.AsNoTracking().Where(c => c.Id == id).FirstOrDefault(); // .Include(c => c.ChurchUnits)
                if (oCFT == null)
                {
                    _userTask = "Attempted deleting " + strDesc.ToLower() + "," + oCFT.FaithDescription;  // var _userTask = "Attempted saving " + strDesc;
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " to delete could not be retrieved." });
                }

                    var saveDelete = true;
                    // ensuring cascade delete where there's none!

                    //check CFC for the CFS
                    var oFaithCategories = _context.ChurchFaithType.AsNoTracking().Where(c => c.FaithTypeClassId == oCFT.Id).ToList();

                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  ///
                using (var _cftCtx = new MSTR_DbContext(_cs))
                { 

                    if (oFaithCategories.Count() > 0) //+ oCFT.ChurchLevels.Count + oCFT.AppSubscriptions.Count + oCFT.ChurchMembers.Count )
                    {
                        if (forceDeleteConfirm == false)
                        {
                        var strConnTabs = "Faith category";
                            saveDelete = false;
                        // check user privileges to determine... administrator rights
                        // log
                        _userTask = "Attempted deleting " + strDesc.ToLower() + "," + oCFT.FaithDescription;  // var _userTask = "Attempted saving " + strDesc;
                        _tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                        return Json(new
                            {
                                taskSuccess = false,
                                tryForceDelete = false,
                                oCurrId = id,
                                userMess = "Specified " + strDesc.ToLower() +
                                                            " to delete has been used elsewhere in the system [" + strConnTabs + "]. Delete cannot be done unless dependent-references are removed."
                            });

                                //super_admin task
                                //return Json(new { taskSuccess = false, tryForceDelete = true, oCurrId = id, userMess = "Specified " + strDesc.ToLower() + 
                                //       " has dependencies or links with other external data [Faith category]. Delete cannot be done unless child refeneces are removed. DELETE (cascade) anyway?" });
                        }


                        //to be executed only for higher privileges...
                        try
                        {
                            //check AGO... for each CFC 
                          foreach (var child in oFaithCategories.ToList())
                          {
                            // AGO cannot be DELETED indirectly...  do it directly:: has too many dependencies
                              var oAppGLoOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.FaithTypeCategoryId == child.Id).ToList();
                              if (oAppGLoOwns.Count() > 0)
                              {
                                  foreach (var grandchild in oAppGLoOwns.ToList())
                                  {
                                      grandchild.FaithTypeCategoryId = null;
                                      grandchild.LastMod = tm;
                                      grandchild.LastModByUserId = loggedUserId;
                                  }
                              }

                            //grandchild dependencies done... delete child
                            _cftCtx.ChurchFaithType.Remove(child); //counter check this too... before delete
                        }                              
                    }

                    catch (Exception ex)
                    {
                        _userTask = "Attempted deleting " + strDesc.ToLower() + "," + oCFT.FaithDescription + " but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                        _tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                        saveDelete = false; 
                        return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Error occured while deleting specified " + strDesc.ToLower() + ": " + ex.Message + ". Reload and try to delete again." });
                    }  
                }

                    //successful...
                    if (saveDelete)
                    {
                        _cftCtx.ChurchFaithType.Remove(oCFT);
                        _cftCtx.SaveChanges();  
                        
                        DetachAllEntities(_cftCtx);

                        _userTask = "Deleted " + strDesc.ToLower() + "," + oCFT.FaithDescription + " successfully";  // var _userTask = "Attempted saving " + strDesc;
                        _tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                        return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oCFT.Id, userMess = strDesc + " successfully deleted." });                       
                    } 
                }
                 
                    _userTask = "Attempted deleting " + strDesc.ToLower() + "," + oCFT.FaithDescription + " but FAILED. Data unavailable.";  // var _userTask = "Attempted saving " + strDesc;
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));
                    //
                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" });
                }

            catch (Exception ex)
            {
                _userTask = "Attempted deleting " + strDesc + " [ID=" + id + "] but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
            }
        }
  

        // AGO
        public ActionResult Index_AGO()  // int? setIndex = 1, int? subSetIndex = 0  int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            // SetUserLogged();
            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin"); // if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                //try
                //{
                    // check permission 
                    var _oUserPrivilegeCol = oUserLogIn_Priv;
                    var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                    TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                    //
                    if (!this.userAuthorized) return View(new MSTRAppGlobalOwnerModel()); //retain view    
                    if (oUserLogIn_Priv == null) return View(new MSTRAppGlobalOwnerModel());
                    if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) return View(new MSTRAppGlobalOwnerModel());
                    var oLoggedUser = oUserLogIn_Priv.UserProfile;
                   // var oLoggedRole = oUserLogIn_Priv.UserRole;

                    // 
                    var strDesc = "Denomination (Church)";
                    var _userTask = "Viewed " + strDesc.ToLower() + " list";
                    var oAGOModel = new MSTRAppGlobalOwnerModel(); //TempData.Keep();  
                                                               // int? oAppGloOwnId = null;
                    var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
                    //
                    int? oAppGloOwnId_Logged = null;
                    int? oChuBodyId_Logged = null;
                    if (oChuBody_Logged != null)
                    {
                        oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
                        oChuBodyId_Logged = oChuBody_Logged.Id;

                        // if (oCurrChuBodyId == null) { oCurrChuBodyId = oChuBody_Logged.Id; }
                        //if (oAppGloOwnId == null) { oAppGloOwnId = oChuBody_Logged.AppGlobalOwnerId; }
                        //else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...
                        //
                        // oAppGloOwnId = oCurrChuBodyLogOn.AppGlobalOwnerId;
                    }

                    //int? oCurrChuMemberId_LogOn = null;
                    //ChurchMember oCurrChuMember_LogOn = null;

                    //var currChurchMemberLogged = _clientContext.ChurchMember.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Id == oUserProfile.ChurchMemberId).FirstOrDefault();
                    //if (currChurchMemberLogged != null) //return View(oAGOModel);
                    //{
                    //    oCurrChuMemberId_LogOn = currChurchMemberLogged.Id;
                    //    oCurrChuMember_LogOn = currChurchMemberLogged;
                    //}

                    var oUserId_Logged = oLoggedUser.Id;
                    var lsAGOMdl = (
                        from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking() //.Include(t => t.ChurchLevels)
                        from t_cft in _context.ChurchFaithType.AsNoTracking().Where(c => c.Category == "FC" && c.Id == t_ago.FaithTypeCategoryId).DefaultIfEmpty()  //.Include(t => t.FaithTypeClass)
                        from t_ctry in _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == t_ago.CtryAlpha3Code).DefaultIfEmpty()

                        select new MSTRAppGlobalOwnerModel()
                        {
                            oAppGlobalOwn = t_ago,
                        // lsChurchLevels = t_ago.ChurchLevels,
                        //       
                            TotalChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == t_ago.Id),
                            TotalCongregations = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == t_ago.Id && c.Status == "A"),
                            // && c.IsActivated==true && c.ChurchWorkStatus=="O" &&    c.OrgType=="CN"),  //c.OrgType=="CH" && 
                            strAppGloOwn = t_ago.OwnerName,
                            strFaithCategory = t_cft != null ? t_cft.FaithDescription : "",
                            strCountry = t_ctry != null ? t_ctry.EngName : "",
                            strSlogan = t_ago.Slogan.Contains("*|*") ? (t_ago.Slogan.Substring(0, t_ago.Slogan.IndexOf("*|*"))).Replace("*|*", "") : t_ago.Slogan,
                            strSloganResponse = t_ago.Slogan.Contains("*|*") ? (t_ago.Slogan.Substring(t_ago.Slogan.IndexOf("*|*"))).Replace("*|*", "") : "",
                        //strChurchStream = t_cft.FaithTypeClass != null ? t_cft.FaithTypeClass.FaithDescription : "",
                        //   
                            blStatusActivated = t_ago.Status == "A",
                            strStatus = GetStatusDesc(t_ago.Status)
                        })
                        .OrderBy(c => c.strCountry).OrderBy(c => c.strAppGloOwn)
                        .ToList();

                    oAGOModel.lsAppGlobalOwnModels = lsAGOMdl;


                    oAGOModel.strCurrTask = strDesc;

                    //                
                    //oAGOModel.oAppGloOwnId = oAppGloOwnId;
                    //oAGOModel.oChurchBodyId = oCurrChuBodyId;
                    //
                    oAGOModel.oUserId_Logged = oUserId_Logged;
                    oAGOModel.oChurchBody_Logged = oChuBody_Logged;
                    oAGOModel.oAppGloOwnId_Logged = oAppGloOwnId_Logged;

               // // 
               // ///
               // ViewData["strAppName"] = "RhemaCMS";
               // ViewData["strAppNameMod"] = "Admin Palette";
               // ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(oLoggedUser.UserDesc) ? oLoggedUser.UserDesc : "[Current user]";
               // ///
               // ViewData["oAppGloOwnId_Logged"] = oAppGloOwnId_Logged;
               // ViewData["oChuBodyId_Logged"] = oChuBodyId_Logged;

               // ViewData["oCBOrgType_Logged"] = oChuBody_Logged.OrgType;  // CH, CN but subscriber may come from oth

               // ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
               // ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
               // ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
               // ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
               //// ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
               // ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

               // //ViewData["strAppCurrUser_ChRole"] = oLoggedRole.RoleDesc; // "System Adminitrator";
               // //ViewData["strAppCurrUser_RoleCateg"] = oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

               // ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
               // ///
                 

               // //refresh Dash values
               // _ = LoadDashboardValues();

                var tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oLoggedUser.Id, tm, tm, oLoggedUser.Id, oLoggedUser.Id));

                    return View("Index_AGO", oAGOModel);

                //}

                //catch (Exception ex)
                //{
                //    //page not found error
                //    
                //    return PartialView("_ErrorPage");
                //} 
            }
        }

        [HttpGet]
        public IActionResult AddOrEdit_AGO(int id = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) // (int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); // if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            { 
                try
                {
                    if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                    {
                        var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                        var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                        // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                        //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                        // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                        oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                        oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                        oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                        var strDesc = "Denomination (Church)";
                        var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();  // _userTask = "Attempted creating new denomination (church)"; // _userTask = "Opened denomination (church)-" + oCFT_MDL.oChurchFaithType.FaithDescription;
                        var oAGO_MDL = new MSTRAppGlobalOwnerModel();
                        if (id == 0)
                        {
                            //create user and init... 
                            oAGO_MDL.oAppGlobalOwn = new MSTRAppGlobalOwner();
                            oAGO_MDL.oAppGlobalOwn.TotalLevels = 1;
                            //oAGO_MDL.oAppGlobalOwn.Status = "A";
                            oAGO_MDL.blStatusActivated = true;

                            _userTask = "Attempted creating new " + strDesc.ToLower();
                        }

                        else
                        {
                            oAGO_MDL = (
                                 from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().AsNoTracking() //.Include(t => t.ChurchLevels) 
                                     .Where(x => x.Id == id)
                                 from t_cft in _context.ChurchFaithType.Include(t => t.FaithTypeClass).AsNoTracking().Where(c => c.Category == "FC" && c.Id == t_ago.FaithTypeCategoryId).DefaultIfEmpty()
                                 from t_ctry in _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == t_ago.CtryAlpha3Code).DefaultIfEmpty()

                                 select new MSTRAppGlobalOwnerModel()
                                 {
                                     oAppGlobalOwn = t_ago,
                                     lsChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_ago.Id).ToList(),
                                 //       
                                    TotalChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == t_ago.Id),
                                     TotalCongregations = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == t_ago.Id && c.Status == "A"),
                                 // && c.IsActivated==true && c.ChurchWorkStatus=="O" &&    c.OrgType=="CN"),  //c.OrgType=="CH" && 
                                    strAppGloOwn = t_ago.OwnerName,
                                     strFaithCategory = t_cft != null ? t_cft.FaithDescription : "",
                                     strCountry = t_ctry != null ? t_ctry.EngName : "",
                                     strSlogan = t_ago.Slogan.Contains("*|*") ? (t_ago.Slogan.Substring(0, t_ago.Slogan.IndexOf("*|*"))).Replace("*|*", "") : t_ago.Slogan,
                                     strSloganResponse = t_ago.Slogan.Contains("*|*") ? (t_ago.Slogan.Substring(t_ago.Slogan.IndexOf("*|*"))).Replace("*|*", "") : "",
                                     strChurchStream = t_cft.FaithTypeClass != null ? t_cft.FaithTypeClass.FaithDescription : "",
                                    //   
                                    blStatusActivated = t_ago.Status == "A",
                                    strStatus = GetStatusDesc(t_ago.Status)
                                 })
                                 .FirstOrDefault();

                            if (oAGO_MDL.oAppGlobalOwn == null)
                            {
                                
                                return PartialView("_ErrorPage");
                            }

                            //if (string.IsNullOrEmpty(oAGO_MDL.oAppGlobalOwn.PrefixKey))
                            //{
                            //    var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
                            //    var jsCode = GetNextCodePrefixByAcronym_jsonString(oAGO_MDL.oAppGlobalOwn.Acronym);  // string json1 = @"{'Name':'James'}";
                            //    var jsOut = JsonConvert.DeserializeAnonymousType(jsCode, template);

                            //    if (jsOut != null)
                            //        if (bool.Parse(jsOut.taskSuccess) == true)
                            //            oAGO_MDL.oAppGlobalOwn.PrefixKey = jsOut.strRes;
                            //}


                            if (string.IsNullOrEmpty(oAGO_MDL.oAppGlobalOwn.PrefixKey))
                                oAGO_MDL.oAppGlobalOwn.PrefixKey = oAGO_MDL.oAppGlobalOwn.Acronym; //GetNextCodePrefixByAcronym_jsonString(oAGO_MDL.oAppGlobalOwn.Acronym, "");

                            //church code  
                            if (string.IsNullOrEmpty(oAGO_MDL.oAppGlobalOwn.GlobalChurchCode) && !string.IsNullOrEmpty(oAGO_MDL.oAppGlobalOwn.PrefixKey))
                            {
                                oAGO_MDL.oAppGlobalOwn.GlobalChurchCode = oAGO_MDL.oAppGlobalOwn.PrefixKey + string.Format("{0:D3}", 0);
                                //oAGO_MDL.oAppGlobalOwn.RootChurchCode = oAGO_MDL.oAppGlobalOwn.GlobalChurchCode;
                            }

                            ////root church code  
                            //if (string.IsNullOrEmpty(oAGO_MDL.oAppGlobalOwn.RootChurchCode) && !string.IsNullOrEmpty(oAGO_MDL.oAppGlobalOwn.GlobalChurchCode))
                            //oAGO_MDL.oAppGlobalOwn.RootChurchCode = oAGO_MDL.oAppGlobalOwn.GlobalChurchCode; 


                            _userTask = "Opened " + strDesc.ToLower() + ", " + oAGO_MDL.oAppGlobalOwn.OwnerName;
                        }


                        // oAGO_MDL.setIndex = setIndex;
                        // oAGO_MDL.subSetIndex = subSetIndex;
                        oAGO_MDL.oUserId_Logged = oUserId_Logged;
                        oAGO_MDL.oAppGloOwnId_Logged = oAGOId_Logged;
                        oAGO_MDL.oChurchBodyId_Logged = oCBId_Logged;
                        //
                        // oAGO_MDL.oAppGloOwnId = oAppGloOwnId;
                        // oAGO_MDL.oChurchBodyId = oCurrChuBodyId;
                        //  var oCurrChuBody = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                        // oAGO_MDL.oChurchBody = oCurrChuBody != null ? oCurrChuBody : null;

                        //   if (oAGO_MDL.subSetIndex == 2) // Denomination classes av church sects

                        oAGO_MDL = this.popLookups_AGO(oAGO_MDL, oAGO_MDL.oAppGlobalOwn);

                        var tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oUserId_Logged, tm, tm, oUserId_Logged, oUserId_Logged));

                        var _oAGO_MDL = Newtonsoft.Json.JsonConvert.SerializeObject(oAGO_MDL);
                        TempData["oVmCurrMod"] = _oAGO_MDL; TempData.Keep();


                        return PartialView("_AddOrEdit_AGO", oAGO_MDL);
                    }

                    //page not found error
                    
                    return PartialView("_ErrorPage");
                }

                catch (Exception ex)
                {
                    //page not found error
                    
                    return PartialView("_ErrorPage");
                }
            }
        }

        public MSTRAppGlobalOwnerModel popLookups_AGO(MSTRAppGlobalOwnerModel vm, MSTRAppGlobalOwner oCurrAGO)
        {
            if (vm == null || oCurrAGO == null) return vm;
            //
            vm.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vm.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vm.lkpFaithCategories = _context.ChurchFaithType.AsNoTracking().Where(c => c.Category == "FC" && !string.IsNullOrEmpty(c.FaithDescription))
                                          .OrderBy(c => c.FaithDescription).ToList()
                                          .Select(c => new SelectListItem()
                                          {
                                              Value = c.Id.ToString(),
                                              Text = c.FaithDescription
                                          })
                                          .ToList();            
          //  vm.lkpFaithCategories.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vm.lkpCountries = _context.MSTRCountry.AsNoTracking().ToList()  //.Where(c => c.Display == true)
                                           .Select(c => new SelectListItem()
                                           {
                                               Value = c.CtryAlpha3Code, //.ToString(),
                                               Text = c.EngName
                                           })
                                           .OrderBy(c => c.Text)
                                           .ToList();
           // vm.lkpCountries.Insert(0, new SelectListItem { Value = "", Text = "Select" });

             
            return vm;
        }

        public JsonResult GetFaithStreamByCategory(int? categoryId )
        {
            var fs = _context.ChurchFaithType.AsNoTracking().Include(t=>t.FaithTypeClass)
                .Where(c => c.Category == "FC" && c.Id == categoryId).FirstOrDefault();

            var res = fs != null;
            var _strRes = fs != null ? (fs.FaithTypeClass != null ? fs.FaithTypeClass.FaithDescription : "") : "";
            return Json(new { taskSuccess = res, strRes = _strRes });

            //if (addEmpty) countryList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            //return Json(countryList);
        }

        public JsonResult GetNextAGOCodePrefixByAcronym(string strPrefix_AGO, string strPrefix_CL = "")
        {
            var strPrefix = strPrefix_AGO + (!string.IsNullOrEmpty(strPrefix_CL) ? "-" + strPrefix_CL : "");
            var tempCode = strPrefix.ToUpper(); // + tempCnt; //+ string.Format("{0:N0}", tempCnt);
            var fsCount = _context.MSTRAppGlobalOwner.AsNoTracking().Count(c => c.GlobalChurchCode == tempCode);
            if (fsCount == 0) return Json(new { taskSuccess = true, strRes = tempCode });
            else
            {
                var tempCnt = 1; tempCode = strPrefix.ToUpper() + tempCnt; //+ string.Format("{0:N0}", tempCnt);
                fsCount = _context.MSTRAppGlobalOwner.AsNoTracking().Count(c => c.GlobalChurchCode == tempCode);
                var res = false;
                while (fsCount > 0 && fsCount < 10)
                {
                    tempCnt++; tempCode = strPrefix.ToUpper() + tempCnt; //+ string.Format("{0:N0}", tempCnt);
                    fsCount = _context.MSTRAppGlobalOwner.AsNoTracking().Count(c => c.GlobalChurchCode == tempCode);
                    //
                    res = fsCount == 0;
                }

                return Json(new { taskSuccess = res, strRes = tempCode });
            }

            //var strPrefix = strPrefix_AGO + (strPrefix_CL.Length != 0 ? "-" + strPrefix_CL : "");
            //var tempCode = strPrefix.ToUpper(); // + tempCnt; //+ string.Format("{0:N0}", tempCnt);
            //var fsCount = _context.MSTRAppGlobalOwner.Count(c => c.RootChurchCode == tempCode);
            //if (fsCount == 0) return Json(new { taskSuccess = true, strRes = tempCode });
            //else
            //{
            //    var tempCnt = 1; tempCode = strPrefix.ToUpper() + tempCnt; //+ string.Format("{0:N0}", tempCnt);
            //    fsCount = _context.MSTRAppGlobalOwner.Count(c => c.RootChurchCode == tempCode);
            //    var res = false;
            //    while (fsCount > 0 && fsCount < 10)
            //    {
            //        tempCnt++; tempCode = strPrefix.ToUpper() + tempCnt; //+ string.Format("{0:N0}", tempCnt);
            //        fsCount = _context.MSTRAppGlobalOwner.Count(c => c.RootChurchCode == tempCode);
            //        //
            //        res = fsCount == 0;
            //    }

            //    return Json(new { taskSuccess = res, strRes = tempCode });
            //}
        }

        public string GetNextAGOCodePrefixByAcronym_jsonString(string strPrefix_AGO, string strPrefix_CL="")
        {
            var strPrefix = strPrefix_AGO + (!string.IsNullOrEmpty(strPrefix_CL) ? "-" + strPrefix_CL : "");
            var tempCode = strPrefix.ToUpper(); // + tempCnt; // string.Format("{0:N0}", tempCnt);
            var fsCount = _context.MSTRAppGlobalOwner.AsNoTracking().Count(c => c.GlobalChurchCode == tempCode);
            if (fsCount == 0) return tempCode; // @"{'taskSuccess' : " + true + ", strRes :'" + tempCode + "'}";
            else
            {
                var tempCnt = 1; tempCode = strPrefix.ToUpper() + tempCnt; // + string.Format("{0:N0}", tempCnt);
                fsCount = _context.MSTRAppGlobalOwner.AsNoTracking().Count(c => c.GlobalChurchCode == tempCode);
                var res = false;
                while (fsCount > 0 && fsCount < 10)
                {
                    tempCnt++; tempCode = strPrefix.ToUpper() + tempCnt; //+ string.Format("{0:N0}", tempCnt);
                    fsCount = _context.MSTRAppGlobalOwner.AsNoTracking().Count(c => c.GlobalChurchCode == tempCode);
                    //
                    res = fsCount == 0;
                }

                return tempCode;  // @"{'taskSuccess' : " + res + ", strRes :'" + tempCode + "'}";
            }

            //var strPrefix = strPrefix_AGO + (strPrefix_CL.Length != 0 ? "-" + strPrefix_CL : "");
            //var tempCode = strPrefix.ToUpper(); // + tempCnt; // string.Format("{0:N0}", tempCnt);
            //var fsCount = _context.MSTRAppGlobalOwner.Count(c => c.RootChurchCode == tempCode);
            //if (fsCount == 0) return tempCode; // @"{'taskSuccess' : " + true + ", strRes :'" + tempCode + "'}";
            //else
            //{
            //    var tempCnt = 1; tempCode = strPrefix.ToUpper() + tempCnt; // + string.Format("{0:N0}", tempCnt);
            //    fsCount = _context.MSTRAppGlobalOwner.Count(c => c.RootChurchCode == tempCode);
            //    var res = false;
            //    while (fsCount > 0 && fsCount < 10)
            //    {
            //        tempCnt++; tempCode = strPrefix.ToUpper() + tempCnt; //+ string.Format("{0:N0}", tempCnt);
            //        fsCount = _context.MSTRAppGlobalOwner.Count(c => c.RootChurchCode == tempCode);
            //        //
            //        res = fsCount == 0;
            //    }

            //    return tempCode;  // @"{'taskSuccess' : " + res + ", strRes :'" + tempCode + "'}";
            //}
        }

        public JsonResult GetNextGlobalChurchCodeByAcronym(int? oAppGloOwnId, string strPrefix_AGO, string strPrefix_CL)
        {
            var strPrefix = strPrefix_AGO + (!string.IsNullOrEmpty(strPrefix_CL) ? "-" + strPrefix_CL : "");

            var fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.OrgType != "CR");
            var tempCnt = fsCount + 1; var tempCode = strPrefix.ToUpper() + string.Format("{0:D3}", tempCnt);
            fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.GlobalChurchCode == tempCode);
            if (fsCount == 0) return Json(new { taskSuccess = true, strRes = tempCode });
            else
            {
                tempCnt++; tempCode = strPrefix.ToUpper() + string.Format("{0:D3}", tempCnt);
                fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.GlobalChurchCode == tempCode);
                var res = false;
                while (fsCount > 0 && fsCount < 10)
                {
                    tempCnt++; tempCode = strPrefix.ToUpper() + string.Format("{0:D3}", tempCnt);
                    fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.GlobalChurchCode == tempCode);
                    //
                    res = fsCount == 0;
                }

                return Json(new { taskSuccess = res, strRes = tempCode });
            }
        }

        public string GetNextGlobalChurchCodeByAcronym_jsonString(int? oAppGloOwnId, string strPrefix_AGO, string strPrefix_CL = "")
        {
            var strPrefix = strPrefix_AGO + (!string.IsNullOrEmpty(strPrefix_CL) ? "-" + strPrefix_CL : "");

            var fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.OrgType != "CR");
            var tempCnt = fsCount + 1; var tempCode = strPrefix.ToUpper() + string.Format("{0:D3}", tempCnt);
            fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.GlobalChurchCode == tempCode);
            if (fsCount == 0) return tempCode; // @"{'taskSuccess' : " + true + ", strRes :'" + tempCode + "'}"; 
            else
            {
                tempCnt++; tempCode = strPrefix.ToUpper() + string.Format("{0:D3}", tempCnt);
                fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.GlobalChurchCode == tempCode);
                var res = false;
                while (fsCount > 0 && fsCount < 10)
                {
                    tempCnt++; tempCode = strPrefix.ToUpper() + string.Format("{0:D3}", tempCnt);
                    fsCount = _context.MSTRChurchBody.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId && c.GlobalChurchCode == tempCode);
                    //
                    res = fsCount == 0;
                }

                return tempCode; // @"{'taskSuccess' : " + res + ", strRes :'" + tempCode + "'}";
            }
        }

        public JsonResult GetNextRootChurchCodeByParentCB(int? oAppGloOwnId, int? oParChurchBodyId, string strCBChurchCode, string strPrefix_AGO, string strPrefix_CL)
        {
           // var strPrefix = strPrefix_AGO + (strPrefix_CL.Length != 0 ? "-" + strPrefix_CL : "");

            //get the church code
            //get the church code
            if (string.IsNullOrEmpty(strCBChurchCode))
            {
                    var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
                    var jsCBChurchCode = GetNextGlobalChurchCodeByAcronym_jsonString(oAppGloOwnId, strPrefix_AGO, strPrefix_CL);  // string json1 = @"{'Name':'James'}";
                    var jsOut = JsonConvert.DeserializeAnonymousType(jsCBChurchCode, template); 
             
                    if (jsOut != null)
                        if (bool.Parse(jsOut.taskSuccess) == true)
                            strCBChurchCode = jsOut.strRes; 
            }         

            var oParCB = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id==oParChurchBodyId && c.Status == "A" ).FirstOrDefault();
            if (oParCB != null && !string.IsNullOrEmpty(strCBChurchCode))
            {
                var strRootCode = oParCB.RootChurchCode + (!string.IsNullOrEmpty(oParCB.RootChurchCode) ? "--" + strCBChurchCode : strCBChurchCode);
                return Json(new { taskSuccess = true, strRes = strRootCode });

                //if (!string.IsNullOrEmpty(oParCB.RootChurchCode))
                //    return Json(new { taskSuccess = true, strRes = oParCB.RootChurchCode + "--" + strCBChurchCode });
            }

            return Json(new { taskSuccess = false, strRes = strCBChurchCode });
        }

        public string GetNextRootChurchCodeByParentCB_jsonString( int? oAppGloOwnId, int? oParChurchBodyId, string strCBChurchCode, string strPrefix_AGO, string strPrefix_CL)
        {
           // var strPrefix = strPrefix_AGO + (strPrefix_CL.Length != 0 ? "-" + strPrefix_CL : "");

            //get the church code
            if (string.IsNullOrEmpty(strCBChurchCode))
                strCBChurchCode = GetNextGlobalChurchCodeByAcronym_jsonString(oAppGloOwnId, strPrefix_AGO, strPrefix_CL);

            // var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
            //  var jsCBChurchCode = GetNextGlobalChurchCodeByAcronym_jsonString(prefixCode, oAppGloOwnId);  // string json1 = @"{'Name':'James'}";
            //var jsOut = JsonConvert.DeserializeAnonymousType(jsCBChurchCode, template);

            //if (jsOut != null)
            //    if (bool.Parse(jsOut.taskSuccess) == true)
            //        strCBChurchCode = jsOut.strRes;

            var oParCB = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oParChurchBodyId && c.Status == "A").FirstOrDefault();
            if (oParCB != null && !string.IsNullOrEmpty(strCBChurchCode))
            {
                var strRootCode = oParCB.RootChurchCode + (!string.IsNullOrEmpty(oParCB.RootChurchCode) ? "--" + strCBChurchCode : strCBChurchCode);
                return strRootCode;

                //if (!string.IsNullOrEmpty(oParCB.RootChurchCode))
                //    return oParCB.RootChurchCode + "--" + strCBChurchCode; // @"{'taskSuccess' : " + true + ", strRes :'" + oParCB.RootChurchCode + "--" + strCBChurchCode + "'}"; 
            }

            return strCBChurchCode; // @"{'taskSuccess' : " + false + ", strRes :''}";
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_AGO(MSTRAppGlobalOwnerModel vm)
        {
            var strDesc = "Denomination (Church)";
            if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again." });
            if (vm.oAppGlobalOwn == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again." });

            MSTRAppGlobalOwner _oChanges = vm.oAppGlobalOwn;  // vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vmMod; TempData.Keep();
           /// var  _cs = this._configuration["ConnectionStrings:DefaultConnection"];

            //check if the configured levels <= total levels under AppGloOwn
            var lsCL = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.Id).ToList();
            var countCL = lsCL.Count();
            if (countCL > _oChanges.TotalLevels)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Total church levels allowed for denomination, " + _oChanges.OwnerName + " [" + _oChanges.TotalLevels + "] exceeded. Hint: You may adjust either way [Denomination or Church level details]" });

            foreach (var oCL in lsCL)
            {
                if (oCL.LevelIndex <= 0 || oCL.LevelIndex > _oChanges.TotalLevels)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please indicate correct level index. Hint: Must be within total church levels [" + _oChanges.TotalLevels + "]. Hint: You may adjust either way [Denomination or Church level details]" });
            }

            if (string.IsNullOrEmpty(_oChanges.OwnerName)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide name for " + strDesc.ToLower() });
            }

            //check if the configured levels <= total levels under AppGloOwn
            var lsCBs = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.Id).ToList();  

            if ((_oChanges.Id == 0 || (_oChanges.Id > 0 && lsCBs.Count() == 0)) && string.IsNullOrEmpty(_oChanges.PrefixKey) && string.IsNullOrEmpty(_oChanges.Acronym)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide acronym or church prefix for " + strDesc.ToLower() });
            }
            if (_oChanges.FaithTypeCategoryId == null)  // you can create 'Others' to cater for non-category
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the church faith stream or category." });
            }

            if (_oChanges.CtryAlpha3Code == null)  // you can create 'Others' to cater for non-category
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the base country." });
            }


            //validations done!
            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<MSTRAppGlobalOwnerModel>(arrData) : vm;

            var oAGO = vmMod.oAppGlobalOwn;
            // oAGO.ChurchBody = vmMod.oChurchBody;

            try
            {
                ModelState.Remove("oAppGlobalOwn.CtryAlpha3Code");
                ModelState.Remove("oAppGlobalOwn.FaithTypeCategoryId");
                ModelState.Remove("oAppGlobalOwn.CreatedByUserId");
                ModelState.Remove("oAppGlobalOwn.LastModByUserId");

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again." });                 
                 
                // church logo
                if (vm.ChurchLogoFile != null ) //&& _oChanges.ChurchLogo != null
                {
                    if (_oChanges.ChurchLogo != vm.ChurchLogoFile.FileName)
                    {
                        string strFilename = null;
                        if (vm.ChurchLogoFile != null && vm.ChurchLogoFile.Length > 0)
                        {
                            string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img_db");
                            strFilename = Guid.NewGuid().ToString() + "_" + vm.ChurchLogoFile.FileName;
                            string filePath = Path.Combine(uploadFolder, strFilename);
                            vm.ChurchLogoFile.CopyTo(new FileStream(filePath, FileMode.Create));
                        }
                        else
                        {
                            if (vm.oAppGlobalOwn.Id != 0) strFilename = vm.strChurchLogo;
                        }

                        _oChanges.ChurchLogo = strFilename;
                    }
                }
                                  
                //
                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.LastModByUserId = vm.oUserId_Logged;
                _oChanges.Status = vm.blStatusActivated ? "A" : "D";

                //
                _oChanges.Slogan = (!string.IsNullOrEmpty(vm.strSlogan) ? vm.strSlogan : "")  + 
                                                    (!string.IsNullOrEmpty(vm.strSlogan) && !string.IsNullOrEmpty(vm.strSloganResponse) ? "*|*" : "") + 
                                                                                (!string.IsNullOrEmpty(vm.strSloganResponse) ? vm.strSloganResponse : "");
                //
                //get the prefix, church code, root code from acronym
                //get the prefix code  
                if (string.IsNullOrEmpty(_oChanges.PrefixKey))
                    _oChanges.PrefixKey = _oChanges.Acronym;

                //{
                //    //var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
                //    //var jsCode = GetNextCodePrefixByAcronym_jsonString(_oChanges.Acronym);  // string json1 = @"{'Name':'James'}";
                //    //var jsOut = JsonConvert.DeserializeAnonymousType(jsCode, template);

                //    //if (jsOut != null)
                //    //    if (bool.Parse(jsOut.taskSuccess) == true)
                //    //        _oChanges.PrefixKey = jsOut.strRes;

                //    _oChanges.PrefixKey = GetNextCodePrefixByAcronym_jsonString(_oChanges.Acronym, "");
                //}

                //church code  
                if (string.IsNullOrEmpty(_oChanges.GlobalChurchCode) && !string.IsNullOrEmpty(_oChanges.PrefixKey))
                {
                    _oChanges.GlobalChurchCode = _oChanges.PrefixKey + string.Format("{0:D3}", 0);
                   // _oChanges.RootChurchCode = _oChanges.GlobalChurchCode;
                }


                //var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
                //jsCode = GetNextGlobalChurchCodeByAcronym_jsonString(_oChanges.PrefixKey, _oChanges.Id);  // string json1 = @"{'Name':'James'}";
                //jsOut = JsonConvert.DeserializeAnonymousType(jsCode, template);

                //if (jsOut != null)
                //    if (bool.Parse(jsOut.taskSuccess) == true)
                //        _oChanges.GlobalChurchCode = jsOut.strRes;



                ////root church code  
                //if (string.IsNullOrEmpty(_oChanges.RootChurchCode) && !string.IsNullOrEmpty(_oChanges.GlobalChurchCode))
                //{
                //    _oChanges.RootChurchCode = _oChanges.GlobalChurchCode;
                //}


                //jsCode = GetNextRootChurchCodeByParentCB_jsonString(_oChanges.PrefixKey, _oChanges.Id, null);  // string json1 = @"{'Name':'James'}";
                //jsOut = JsonConvert.DeserializeAnonymousType(jsCode, template);

                //if (jsOut != null)
                //    if (bool.Parse(jsOut.taskSuccess) == true)
                //        _oChanges.RootChurchCode = jsOut.strRes;

                if (string.IsNullOrEmpty(_oChanges.PrefixKey) || string.IsNullOrEmpty(_oChanges.GlobalChurchCode)) // || string.IsNullOrEmpty(_oChanges.RootChurchCode)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Prefix code, Church code and Root church code for " + strDesc.ToLower() + " must be specified" });
                }


                //validate...
                var _userTask = "Attempted saving " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper();  // _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";   //  _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                var _reset = _oChanges.Id == 0;


                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  ///
                if (string.IsNullOrEmpty(_cs))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Connection to central DB failed. Please refresh page to retry connection" });


                //using (var _agoCtx = new MSTR_DbContext(_cs))
                //{
                if (_oChanges.Id == 0)
                    {
                        var existAGO = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.OwnerName.ToLower() == _oChanges.OwnerName.ToLower()).ToList();
                        if (existAGO.Count() > 0)
                            { return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = strDesc + " -- " + _oChanges.OwnerName + " already exist." }); }

                        _oChanges.Created = tm;
                        _oChanges.CreatedByUserId = vm.oUserId_Logged;

                        _context.Add(_oChanges);

                        ViewBag.UserMsg = "Saved " + strDesc.ToLower() + " " + (!string.IsNullOrEmpty(_oChanges.OwnerName) ? " -- " + _oChanges.OwnerName : "") + " successfully.";
                        _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                    }

                    else
                    {
                        var existAGO = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id != _oChanges.Id && c.OwnerName.ToLower() == _oChanges.OwnerName.ToLower()).ToList();
                        if (existAGO.Count() > 0)
                        {
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = strDesc + " -- " + _oChanges.OwnerName + " already exist."  });
                        }

                        //retain the pwd details... hidden fields
                        _context.Update(_oChanges);
                        //var _strDesc = strDesc.Length > 0 ? strDesc.Substring(0, 1).ToUpper() + strDesc.Substring(1) : "Denomination ";

                        ViewBag.UserMsg = strDesc + " updated successfully.";
                        _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                    }

                    //save denomination first... 
                    await _context.SaveChangesAsync();
                     

                //    DetachAllEntities(_agoCtx);

                //}
                 

                var _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));

                 

                //auto-update the church levels
                using (var _clCtx = new MSTR_DbContext(_cs))
                {
                    var oChLevelCntAdd = 0; var oChLevelCntUpd = 0;
                    //  _userTask = "Attempted saving church level, " + _oChanges.ToUpper();  // _userTask = "Added new church level, " + _oChanges.OwnerName.ToUpper() + " successfully";   //  _userTask = "Updated church level, " + _oChanges.OwnerName.ToUpper() + " successfully";
                    if (vmMod.oAppGlobalOwn.Id == 0)
                    {
                        for (int i = 1; i <= _oChanges.TotalLevels; i++) // oAGO.TotalLevels; i++)
                        {
                            MSTRChurchLevel oCL = new MSTRChurchLevel()
                            {
                                Name = "Level_" + i,
                                CustomName = "Level " + i,
                                LevelIndex = i,
                                AppGlobalOwnerId = _oChanges.Id,
                                SharingStatus = "N",
                                Created = DateTime.Now,
                                LastMod = DateTime.Now,
                            };
                            //
                            oChLevelCntAdd++;
                            _clCtx.Add(oCL);
                        }

                        if (oChLevelCntAdd > 0)
                        {
                            _userTask = "Added new " + oChLevelCntAdd + " church levels for " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                            ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Created " + oChLevelCntAdd + " church levels. Customization may be necessary";
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= _oChanges.TotalLevels; i++)
                        {
                            var oExistCL = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.Id && c.Name == "Level_" + i).FirstOrDefault();
                            if (oExistCL == null && (countCL + oChLevelCntAdd ) < _oChanges.TotalLevels)  //add new ... the missing levels
                            {
                                MSTRChurchLevel oCL = new MSTRChurchLevel()
                                {
                                    Name = "Level_" + i,
                                    CustomName = "Level " + i,
                                    LevelIndex = i,
                                    AppGlobalOwnerId = _oChanges.Id,
                                    SharingStatus = "N",
                                    Created = DateTime.Now,
                                    LastMod = DateTime.Now,
                                };
                               
                                //
                                oChLevelCntAdd++;
                                _clCtx.Add(oCL);
                            }

                            // UPDATE unecessary!
                            //else if (oExistCL != null && (countCL + oChLevelCntAdd ) <= _oChanges.TotalLevels)  // upd
                            //{
                            //    oExistCL.Name = "Level_" + i;
                            //    oExistCL.CustomName = "Level " + i;
                            //    oExistCL.LevelIndex = i;
                            //    oExistCL.AppGlobalOwnerId = _oChanges.Id;
                            //    oExistCL.SharingStatus = "N";
                            //    oExistCL.LastMod = DateTime.Now;
                            //    //
                            //    oChLevelCntUpd++;
                            //    ctx.Update(oExistCL);
                            //}
                        }

                        if ((oChLevelCntAdd + oChLevelCntUpd) > 0)
                        {
                            if (oChLevelCntAdd > 0)
                            {
                                _userTask = "Added new " + oChLevelCntAdd + " church levels for " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                                ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Created " + oChLevelCntAdd + " church levels. Customization may be necessary";
                            }

                            if (oChLevelCntUpd > 0)
                            {
                                _userTask = !string.IsNullOrEmpty(_userTask) ? _userTask + ". " : "" + "Updated " + oChLevelCntUpd + " church levels for " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                                ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Denomination's " + oChLevelCntUpd + " church levels updated. Customization may be necessary.";
                            }
                        }
                    }

                    if ((oChLevelCntAdd + oChLevelCntUpd) > 0)
                    {
                        await _clCtx.SaveChangesAsync();

                        
                        DetachAllEntities(_clCtx);
                        
                        _tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: Church Level", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));
                    }

                }


                //auto-update the church root - church body : RCM000
                using (var _cbCtx = new MSTR_DbContext(_cs))
                {
                    var oCBCntAdd = 0; var oCBCntUpd = 0;
                    //  _userTask = "Attempted saving church level, " + _oChanges.ToUpper();  // _userTask = "Added new church level, " + _oChanges.OwnerName.ToUpper() + " successfully";   //  _userTask = "Updated church level, " + _oChanges.OwnerName.ToUpper() + " successfully";

                    var oCL_1 = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.Id && c.LevelIndex == 1).FirstOrDefault();
                    if (vmMod.oAppGlobalOwn.Id == 0)
                    {
                        MSTRChurchBody oCB = new MSTRChurchBody()
                        {
                            Name = _oChanges.OwnerName,
                            OrgType = "CR",
                            AppGlobalOwnerId = _oChanges.Id,
                            ChurchLevelId = oCL_1 != null ? oCL_1.Id : (int?)null,
                            // CountryId = _oChanges.CountryId,
                            CtryAlpha3Code = _oChanges.CtryAlpha3Code,
                            CountryRegionId = null,
                            GlobalChurchCode = _oChanges.GlobalChurchCode,
                            RootChurchCode = _oChanges.GlobalChurchCode,
                            //ChurchUnitLogo = _oChanges.ChurchLogo,
                            ParentChurchBodyId = null,
                            Status = "A", 
                            Created = DateTime.Now,
                            LastMod = DateTime.Now,
                            CreatedByUserId = _oChanges.CreatedByUserId,
                            LastModByUserId = _oChanges.LastModByUserId
                        };                        

                        oCBCntAdd++;
                        _cbCtx.Add(oCB);

                        if (oCBCntAdd > 0)
                        {
                            _userTask = "Added Church Root unit for " +  strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                            ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Created " + oCBCntAdd + " church root unit";
                        }
                    }
                    else
                    {
                        var oCBList = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.Id && c.OrgType == "CR" && c.Status == "A").ToList();
                        if (oCBList.Count() == 0)
                        {
                            MSTRChurchBody oCB = new MSTRChurchBody()
                            {
                                Name = _oChanges.OwnerName,
                                OrgType = "CR",
                                AppGlobalOwnerId = _oChanges.Id,
                                ChurchLevelId = oCL_1 != null ? oCL_1.Id : (int?)null,
                                CtryAlpha3Code = _oChanges.CtryAlpha3Code,
                                CountryRegionId = null,
                                GlobalChurchCode = _oChanges.GlobalChurchCode,
                                RootChurchCode = _oChanges.GlobalChurchCode,
                                //ChurchUnitLogo = _oChanges.ChurchLogo,
                                ParentChurchBodyId = null,
                                Status = "A",
                                Created = DateTime.Now,
                                LastMod = DateTime.Now,
                                CreatedByUserId = _oChanges.CreatedByUserId,
                                LastModByUserId = _oChanges.LastModByUserId
                            };

                            oCBCntAdd++;
                            _cbCtx.Add(oCB);
                        }
                        else
                        {
                            var recUpdated = false;
                            if (string.Compare(oCBList[0].Name, _oChanges.OwnerName, true) != 0) { oCBList[0].Name = _oChanges.OwnerName; recUpdated = true; }
                            if (oCBList[0].AppGlobalOwnerId != _oChanges.Id) { oCBList[0].AppGlobalOwnerId = _oChanges.Id; recUpdated = true; }
                            if (oCBList[0].ChurchLevelId != (oCL_1 != null ? oCL_1.Id : (int?)null)) { oCBList[0].ChurchLevelId = (oCL_1 != null ? oCL_1.Id : (int?)null); recUpdated = true; }
                            if (oCBList[0].ParentChurchBodyId != null) { oCBList[0].ParentChurchBodyId = null; recUpdated = true; }
                            if (oCBList[0].CtryAlpha3Code != _oChanges.CtryAlpha3Code) { oCBList[0].CtryAlpha3Code = _oChanges.CtryAlpha3Code; recUpdated = true; }
                            if (string.Compare(oCBList[0].GlobalChurchCode, _oChanges.GlobalChurchCode, true) != 0) { oCBList[0].GlobalChurchCode = _oChanges.GlobalChurchCode; recUpdated = true; }
                            if (string.Compare(oCBList[0].RootChurchCode, _oChanges.GlobalChurchCode, true) != 0) { oCBList[0].RootChurchCode = _oChanges.GlobalChurchCode; recUpdated = true; }

                            if (recUpdated)
                            {
                                oCBList[0].LastMod = DateTime.Now;
                                oCBList[0].LastModByUserId = _oChanges.LastModByUserId;
                                //
                                oCBCntUpd++;
                                _cbCtx.Update(oCBList[0]);
                            }
                        }

                        if ((oCBCntAdd + oCBCntUpd) > 0)
                        {
                            if (oCBCntAdd > 0)
                            {
                                _userTask = "Added Church Root unit for " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                                ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Created " + oCBCntAdd + " church root unit";
                            }

                            if (oCBCntUpd > 0)
                            {
                                _userTask = !string.IsNullOrEmpty(_userTask) ? _userTask + ". " : "" + "Updated Church Root unit for " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
                                ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Denomination's " + oCBCntUpd + " Church Root unit updated.";
                            }
                        }
                    }

                    if ((oCBCntAdd + oCBCntUpd) > 0)
                    {
                        await _cbCtx.SaveChangesAsync();

                        using (var agoCtx = new MSTR_DbContext(_cs))
                        
                         DetachAllEntities(_cbCtx); 

                        _tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: Church Unit", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));
                    }

                }



                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurr"] = _vmMod; TempData.Keep();
                 

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, resetNew = _reset, userMess = ViewBag.UserMsg });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving denomination (church) details. Err: " + ex.Message  });
            }
        }

        public IActionResult Delete_AGO(int? loggedUserId, int id, bool forceDeleteConfirm = false)  // (int? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
        {
            var strDesc =  "Denomination (Church)"; var _tm = DateTime.Now; var _userTask = "Attempted saving denomination (church)";
           /// var _cs  = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];
            //
            try
            {
                var tm = DateTime.Now;
                //
                var oAGO = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == id).FirstOrDefault(); // .Include(c => c.ChurchUnits)
                if (oAGO == null)
                {
                    _userTask = "Attempted deleting " + strDesc.ToLower() +  ", "  + oAGO.OwnerName;  // var _userTask = "Attempted saving denomination (church)";
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " to delete could not be retrieved." });
                }

                var saveDelete = true;
                // ensuring cascade delete where there's none!

                //check for the CL, CB, UP, CSC and others
                var oChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO.Id).ToList();
                var oChurchBodies = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO.Id).ToList();
                var oUserProfiles = _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO.Id).ToList();
                var oClientServerConfigs = _context.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO.Id).ToList();


                //using (var _agoCtx = new MSTR_DbContext(_cs))
                //{ 
                    if ((oChurchLevels.Count() + oChurchBodies.Count() + oUserProfiles.Count() + oClientServerConfigs.Count()) > 0)
                    {
                        var strConnTabs = oChurchLevels.Count() > 0 ? "Church level" : "";
                        strConnTabs = strConnTabs.Length > 0 ? strConnTabs + ", " : strConnTabs;
                        strConnTabs = oChurchLevels.Count() > 0 ? strConnTabs + "Church unit" : strConnTabs;
                        strConnTabs = oUserProfiles.Count() > 0 ? strConnTabs + ", User profile" : strConnTabs;
                        strConnTabs = oClientServerConfigs.Count() > 0 ? strConnTabs + ", Client server config" : strConnTabs;

                        if (forceDeleteConfirm == false)
                        {
                            saveDelete = false;
                            // check user privileges to determine... administrator rights
                            // log
                            _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oAGO.OwnerName;  // var _userTask = "Attempted saving denomination (church)";
                            _tm = DateTime.Now;
                            _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                            return Json(new
                            {
                                taskSuccess = false,
                                tryForceDelete = false,
                                oCurrId = id,
                                userMess = "Specified " + strDesc.ToLower() + " to delete has been used elsewhere in the system [" + strConnTabs + "]. Delete cannot be done unless dependent-references are removed."
                            });

                            //super_admin task
                            //return Json(new { taskSuccess = false, tryForceDelete = true, oCurrId = id, userMess = "Specified " + strDesc.ToLower() + 
                            //       " has dependencies or links with other external data [Faith category]. Delete cannot be done unless child refeneces are removed. DELETE (cascade) anyway?" });
                        }


                        //to be executed only for higher privileges...
                        try
                        {
                            //check AGO... for each CFC 
                            foreach (var child in oChurchLevels.ToList())
                            {
                                // CB cannot be DELETED indirectly...  do it directly:: has too many dependencies
                                var oCBs = _context.MSTRChurchBody.AsNoTracking().Where(c => c.ChurchLevelId == child.Id).ToList();
                                if (oCBs.Count() > 0)
                                {
                                    foreach (var grandchild in oCBs.ToList())
                                    {
                                        grandchild.ChurchLevelId = null;
                                        grandchild.LastMod = tm;
                                        grandchild.LastModByUserId = loggedUserId;
                                    }
                                }

                            //grandchild dependencies done... delete child
                            _context.MSTRChurchLevel.Remove(child); //counter check this too... before delete
                            }
                        }

                        catch (Exception ex)
                        {
                            _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oAGO.OwnerName + " but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                            _tm = DateTime.Now;
                            _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));


                            saveDelete = false;
                            return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Error occured while deleting specified " + strDesc.ToLower() + ": " + ex.Message + ". Reload and try to delete again." });
                        }
                    }

                    //successful...
                    if (saveDelete)
                    {
                        _context.MSTRAppGlobalOwner.Remove(oAGO);
                    _context.SaveChanges();


                      ///  DetachAllEntities(_agoCtx); 

                        _userTask = "Deleted " + strDesc.ToLower() + ", " + oAGO.OwnerName + " successfully";  // var _userTask = "Attempted saving " + strDesc;
                        _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                        return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oAGO.Id, userMess = strDesc + " successfully deleted." });
                    }

                    else
                    { 
                        //DetachAllEntities(_agoCtx); 
                        return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" }); 
                    }
                      
               // }
               
            }

            catch (Exception ex)
            {
                _userTask = "Attempted deleting " + strDesc.ToLower() + ", [ID=" + id + "] but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc , AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
            }
        }



        // CL
        public ActionResult Index_CL(int? oAppGloOwnId = null, int pageIndex = 1)  // , int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            // SetUserLogged();
            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin"); // if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                var strDesc = "Church Level"; 
                var _userTask = "Viewed " + strDesc.ToLower() + " list";

                // check permission 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                //
                if (!this.userAuthorized) return View(new MSTRChurchLevelModel()); //retain view    
                if (oUserLogIn_Priv == null) return View(new MSTRChurchLevelModel());
                if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) return View(new MSTRChurchLevelModel());
                var oLoggedUser = oUserLogIn_Priv.UserProfile;
               // var oLoggedRole = oUserLogIn_Priv.UserRole;

                //
                var oUserId_Logged = oLoggedUser.Id;
                var oChuBody_Logged = oUserLogIn_Priv.ChurchBody; 
                int? oAppGloOwnId_Logged = null; int? oChuBodyId_Logged = null;
                if (oChuBody_Logged != null)
                {
                    oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
                    oChuBodyId_Logged = oChuBody_Logged.Id;
                }
                                

                //
                var oCLModel = new MSTRChurchLevelModel(); //TempData.Keep();     // int? oAppGloOwnId = null;
                var lsCLModel = (
                        from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId) //.Include(t => t.AppGlobalOwner)
                        from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_cl.AppGlobalOwnerId).DefaultIfEmpty()
                        from t_ci_ago in _context.MSTRContactInfo.Include(t => t.Country).AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cl.AppGlobalOwnerId && c.ChurchBodyId == null && c.Id == t_ago.ContactInfoId).DefaultIfEmpty()  

                        select new MSTRChurchLevelModel()
                        {
                            oChurchLevel = t_cl,
                            strAppGloOwn = t_ago.OwnerName + (!string.IsNullOrEmpty(t_ago.OwnerName) && t_ci_ago.Country != null ? ", " + t_ci_ago.Country.EngName : (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "")),
                        }
                       )
                       .OrderBy(c => c.oChurchLevel.AppGlobalOwnerId).ThenBy(c => c.oChurchLevel.LevelIndex)
                       .ToList();

                oCLModel.lsChurchLevelModels = lsCLModel;
                oCLModel.strCurrTask = strDesc;

                //                
                oCLModel.oAppGloOwnId = oAppGloOwnId;
                oCLModel.PageIndex = pageIndex;
                //oCLModel.oChurchBodyId = oCurrChuBodyId;
                //
                oCLModel.oUserId_Logged = oUserId_Logged;
                oCLModel.oChurchBody_Logged = oChuBody_Logged;
                oCLModel.oAppGloOwnId_Logged = oAppGloOwnId_Logged;


                // dashboard lookups...
                oCLModel.strAppName = "RhemaCMS"; ViewBag.strAppName = oCLModel.strAppName;
                oCLModel.strAppNameMod = "Admin Palette"; ViewBag.strAppNameMod = oCLModel.strAppNameMod;
                oCLModel.strAppCurrUser = oLoggedUser.UserDesc; ViewBag.strAppCurrUser = oCLModel.strAppCurrUser;  // "Dan Abrokwa"
                                                                                                                   //oUPModel.strChurchType = "CH"; ViewBag.strChurchType = oUPModel.strChurchType;
                                                                                                                   //oUPModel.strChuBodyDenomLogged = "Rhema Global Church"; ViewBag.strChuBodyDenomLogged = oUPModel.strChuBodyDenomLogged;
                                                                                                                   //oUPModel.strChuBodyLogged = "Rhema Comm Chapel"; ViewBag.strChuBodyLogged = oUPModel.strChuBodyLogged;

                oCLModel.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Status == "A")
                                             .OrderBy(c => c.OwnerName).ToList()
                                             .Select(c => new SelectListItem()
                                             {
                                                 Value = c.Id.ToString(),
                                                 Text = c.OwnerName
                                             })
                                             .ToList();

                //           
               // ///
               // ViewData["strAppName"] = "RhemaCMS";
               // ViewData["strAppNameMod"] = "Admin Palette";
               // ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(oLoggedUser.UserDesc) ? oLoggedUser.UserDesc : "[Current user]";
               // ///
               // ViewData["oAppGloOwnId_Logged"] = oAppGloOwnId_Logged;
               // ViewData["oChuBodyId_Logged"] = oChuBodyId_Logged;

               // ViewData["oCBOrgType_Logged"] = oChuBody_Logged.OrgType;  // CH, CN but subscriber may come from oth

               // ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
               // ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
               // ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
               // ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
               //// ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc);
               // ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

               // //ViewData["strAppCurrUser_ChRole"] = oLoggedRole.RoleDesc; // "System Adminitrator";
               // //ViewData["strAppCurrUser_RoleCateg"] = oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

               // ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
               // ///


               // //refresh Dash values
               // _ = LoadDashboardValues();

                var tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + strDesc , AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oLoggedUser.Id, tm, tm, oLoggedUser.Id, oLoggedUser.Id));

                return View(oCLModel);
            }
        }

        [HttpGet]
        public IActionResult AddOrEdit_CL(int id = 0, int? oAppGloOwnId = null, int pageIndex = 1, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) // (int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0, int? oCLId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                {    
                    var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                    var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                    // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                    //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                    // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                    oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                    oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                    oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                    var strDesc = "Church level"; 
                    var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); // _userTask = "Attempted creating new denomination (church)"; // _userTask = "Opened denomination (church)-" + oCFT_MDL.oChurchFaithType.FaithDescription;
                    var oCLModel = new MSTRChurchLevelModel();
                    if (id == 0)
                    {
                        oCLModel.oChurchLevel = new MSTRChurchLevel();
                        var oCLIndex = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId).Count() + 1;
                        var oAppOwn = _context.MSTRAppGlobalOwner.Find(oAppGloOwnId);
                        if (oAppOwn == null)
                        {
                            
                            return PartialView("_ErrorPage");
                        }

                        oCLModel.oChurchLevel.Name = "Level_" + oCLIndex;
                        oCLModel.oChurchLevel.CustomName = "Level " + oCLIndex;
                        oCLModel.oChurchLevel.LevelIndex = oCLIndex;
                        oCLModel.oChurchLevel.AppGlobalOwnerId = oAppGloOwnId;
                        oCLModel.oChurchLevel.SharingStatus = "N"; 

                        oCLModel.oChurchLevel.Created = DateTime.Now;
                        oCLModel.oChurchLevel.LastMod = DateTime.Now;
                        //
                        oCLModel.strAppGloOwn = oAppOwn.OwnerName;

                        _userTask = "Attempted creating new " + strDesc .ToLower() + ", " + (oCLModel.oChurchLevel.CustomName != null ? oCLModel.oChurchLevel.CustomName : oCLModel.oChurchLevel.Name);
                    }

                    else
                    {
                        oCLModel = (
                             from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(x => x.Id == id)
                             from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_cl.AppGlobalOwnerId).DefaultIfEmpty()
                             from t_ci_ago in _context.MSTRContactInfo.Include(t => t.Country).AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cl.AppGlobalOwnerId && c.ChurchBodyId == null && c.Id == t_ago.ContactInfoId).DefaultIfEmpty()

                             select new MSTRChurchLevelModel()
                             {
                                 oChurchLevel = t_cl,
                                 strAppGloOwn = t_ago.OwnerName + (!string.IsNullOrEmpty(t_ago.OwnerName) && t_ci_ago.Country != null ? ", " + t_ci_ago.Country.EngName : (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "")),
                             }
                       )
                         .FirstOrDefault();

                        if (oCLModel == null)
                        {
                            
                            return PartialView("_ErrorPage");
                        }

                        _userTask = "Opened " + strDesc .ToLower() + ", " + (oCLModel.oChurchLevel.CustomName != null ? oCLModel.oChurchLevel.CustomName : oCLModel.oChurchLevel.Name);
                    }


                    // oCLModel.setIndex = setIndex;
                    // oCLModel.subSetIndex = subSetIndex;
                    oCLModel.oUserId_Logged = oUserId_Logged;
                    oCLModel.oAppGloOwnId_Logged = oAGOId_Logged;
                    oCLModel.oChurchBodyId_Logged = oCBId_Logged;
                    //
                     oCLModel.oAppGloOwnId = oAppGloOwnId;
                    // oCLModel.oChurchBodyId = oCurrChuBodyId;
                    oCLModel.PageIndex = pageIndex;

                    //  var oCurrChuBody = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                    // oCLModel.oChurchBody = oCurrChuBody != null ? oCurrChuBody : null;

                    if (oCLModel.subSetIndex == 2) // Church level classes av church sects
                        oCLModel = this.popLookups_CL(oCLModel, oCLModel.oChurchLevel);
                     
                    var tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oUserId_Logged, tm, tm, oUserId_Logged, oUserId_Logged));

                    var _oCLModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCLModel);
                    TempData["oVmCurrMod"] = _oCLModel; TempData.Keep();

                    return PartialView("_AddOrEdit_CL", oCLModel);
                }

                //page not found error
                
                return PartialView("_ErrorPage");
            }
        }

        public MSTRChurchLevelModel popLookups_CL(MSTRChurchLevelModel vm, MSTRChurchLevel oCurrCL)
        {
            if (vm == null || oCurrCL == null) return vm;
            //
            //vm.lkpStatuses = new List<SelectListItem>();
            //foreach (var dl in dlGenStatuses) { vm.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vm.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Status == "A")
                                              .OrderBy(c => c.OwnerName).ToList()
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = c.OwnerName
                                              })
                                              .ToList();

           // vm.lkpAppGlobalOwns.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            return vm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_CL(MSTRChurchLevelModel vm)
        {
            var strDesc = "Church level";
            if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.PageIndex });
            if (vm.oChurchLevel == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.PageIndex });

            MSTRChurchLevel _oChanges = vm.oChurchLevel;  // vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vmMod; TempData.Keep();
            /// var  _cs = this._configuration["ConnectionStrings:DefaultConnection"];


            //check if the configured levels <= total levels under AppGloOwn
            var countCL = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId);
            var oAGO = _context.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
            if ( oAGO == null)  
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify denomination (church)", pageIndex = vm.PageIndex });

            if ((_oChanges.Id == 0 && countCL >= oAGO.TotalLevels) || (_oChanges.Id > 0 && countCL > oAGO.TotalLevels))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Total " + strDesc.ToLower() + "s allowed for denomination, " + oAGO.OwnerName + " [" + oAGO.TotalLevels + "] reached.", pageIndex = vm.PageIndex });
                       
            if (_oChanges.LevelIndex <= 0 || _oChanges.LevelIndex > oAGO.TotalLevels)  
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please indicate correct level index. Hint: Must be within total church levels [" + oAGO.TotalLevels + "]", pageIndex = vm.PageIndex });

            if (string.IsNullOrEmpty(_oChanges.Name))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the " + strDesc.ToLower() + " name", pageIndex = vm.PageIndex });


            // validations done!
            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<MSTRChurchLevelModel>(arrData) : vm;

            var oCL = vmMod.oChurchLevel;
            // oCL.ChurchBody = vmMod.oChurchBody; 

            try
            {
                ModelState.Remove("oChurchLevel.AppGlobalOwnerId");
                ModelState.Remove("oChurchLevel.Name");
                //
                //ModelState.Remove("oChurchBody.Id");
                //ModelState.Remove("oChurchBody.Name");
                //ModelState.Remove("oChurchBody.ChurchType");
                //ModelState.Remove("oChurchBody.OrgType");

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", pageIndex = vm.PageIndex });
                 
                 
                //
                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.LastModByUserId = vm.oUserId_Logged;

                var _reset = _oChanges.Id == 0;
                var oCLDesc = strDesc + ", " + (_oChanges.CustomName != null ? _oChanges.CustomName : _oChanges.Name);

                //validate...
                var _userTask = "Attempted saving "  + oCLDesc;  // _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";   //  _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";
               

                //using (var _clCtx = new MSTR_DbContext(_cs))
                //{

                    if (_oChanges.Id == 0)
                        {
                            var existCL = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId &&  
                                                     (c.CustomName.ToLower() == _oChanges.CustomName.ToLower() || c.Name.ToLower() == _oChanges.Name.ToLower()));
                            if (existCL > 0)
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = oCLDesc + " already exist.", pageIndex = vm.PageIndex });

                            _oChanges.Created = tm;
                            _oChanges.CreatedByUserId = vm.oUserId_Logged;

                            _context.Add(_oChanges);

                            ViewBag.UserMsg = "Saved " + oCLDesc + " successfully.";
                            _userTask = "Added new " + oCLDesc + " successfully";
                        }

                        else
                        {
                            var existCL = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId==_oChanges.AppGlobalOwnerId && c.Id != _oChanges.Id && 
                                                    (c.CustomName.ToLower() == _oChanges.CustomName.ToLower() || c.Name.ToLower() == _oChanges.Name.ToLower())) ;
                            if (existCL > 0) 
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = oCLDesc + " already exist.", pageIndex = vm.PageIndex });
                
                            //retain the pwd details... hidden fields


                            _context.Update(_oChanges);
                            //var _strDesc = strDesc.Length > 0 ? strDesc.Substring(0, 1).ToUpper() + strDesc.Substring(1) : "Church level ";

                            ViewBag.UserMsg = oCLDesc + " updated successfully.";
                            _userTask = "Updated " + oCLDesc + " successfully";
                        }

                        //save denomination first... 
                        await _context.SaveChangesAsync();


                //    DetachAllEntities(_clCtx);
                //}

                 

                var _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));


                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                TempData["oVmCurr"] = _vmMod; TempData.Keep();

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, resetNew = _reset, userMess = ViewBag.UserMsg, pageIndex = vm.PageIndex });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving denomination (church) details. Err: " + ex.Message, pageIndex = vm.PageIndex });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditBLK_CL(MSTRChurchLevelModel vm, IFormCollection f) //ChurchMemAttendList oList)      
                                                                                                      // public IActionResult Index_Attendees(ChurchMemAttendList oList) //List<ChurchMember> oList)  //, int? reqChurchBodyId = null, string strAttendee="M", string strLongevity="C" ) //, char? filterIndex = null, int? filterVal = null)
        { 
            var strDesc = "Church level";
            if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = 2 });
            if (vm.lsChurchLevelModels == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No changes made to " + strDesc + " data.", pageIndex = vm.PageIndex });
            if (vm.lsChurchLevelModels.Count == 0) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No changes made to " + strDesc + " data.", pageIndex = vm.PageIndex });
            //    if (vm.oChurchLevel == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again." });

            //  MSTRChurchLevel _oChanges = vm.oChurchLevel;  // vm = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vm; TempData.Keep();

            var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];

            if (string.IsNullOrEmpty(_cs))
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Connecto to client DB failed. Please refresh to retry connection" });

            //check if the configured levels <= total levels under AppGloOwn 
            var oVal = f["oAppGloOwnId"].ToString();
            var oAGOId = !string.IsNullOrEmpty(oVal) ? int.Parse(oVal) : (int?)null;
            var countCL = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOId);
            var oAGO = _context.MSTRAppGlobalOwner.Find(oAGOId);
            if (oAGO == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Specify denomination (church)" });

            if (countCL > oAGO.TotalLevels)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Total " + strDesc.ToLower() + "s allowed for denomination, " + oAGO.OwnerName + " [" + oAGO.TotalLevels + "] exceeded.", pageIndex = vm.PageIndex });


            // return View(vm);
            if (ModelState.IsValid == false)
                return Json(new { taskSuccess = false, oCurrId = oAGOId, userMess = "Saving data failed. Please refresh and try again", pageIndex = vm.PageIndex });

            //if (vm == null)
            //    return Json(new { taskSuccess = false, userMess = "Data to update not found. Please refresh and try again", pageIndex = vm.PageIndex });

            //if (vm.lsChurchLevelModels == null)
            //    return Json(new { taskSuccess = false, userMess = "No changes made to attendance data.", pageIndex = vm.PageIndex });

            //if (vm.lsChurchLevelModels.Count == 0)
            //    return Json(new { taskSuccess = false, userMess = "No changes made to attendance data", pageIndex = vm.PageIndex });


            


            //if ((_oChanges.Id == 0 && countCL >= oAGO.TotalLevels) || (_oChanges.Id > 0 && countCL > oAGO.TotalLevels))
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Total " + strDesc.ToLower() + "s allowed for denomination, " + oAGO.OwnerName + " [" + oAGO.TotalLevels + "] reached." });

            //if (_oChanges.LevelIndex <= 0 || _oChanges.LevelIndex > oAGO.TotalLevels)
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please indicate correct level index. Hint: Must be within total church levels [" + oAGO.TotalLevels + "]" });

            //if (string.IsNullOrEmpty(_oChanges.Name))
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the " + strDesc.ToLower() + " name" });



            //get the global var
            // var oCbId = f["_hdnAppGloOwnId"].ToString();
            //var oCbId = f["oChurchBodyId"].ToString();
            //var oDt = f["m_DateAttended"].ToString();
            //var oEv = f["m_ChurchEventId"].ToString();

            // if (oCbId == null)
            //   return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Denomination (church) is required. Please specify denomination.", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });

            //var oCBId = int.Parse(oCbId);
            //var dtEv = DateTime.Parse(oDt);
            //var oEvId = int.Parse(oEv);

            var numErrAddExceedCnt = 0;
            var numErrUpdExceedCnt = 0;
            var numErrAddExistCnt = 0;
            var numErrUpdExistCnt = 0;


            using (var _clBulkCtx = new MSTR_DbContext(_cs))
            {
                var oChLevelCntAdd = 0; var oChLevelCntUpd = 0;
                foreach (var d in vm.lsChurchLevelModels)
                {
                    if (d.oChurchLevel != null)
                    {
                        if (d.oChurchLevel.Id > 0)  // update
                        {
                            var err = false;
                            if ((countCL + oChLevelCntAdd) > oAGO.TotalLevels)   // + oChLevelCntUpd
                            { numErrUpdExceedCnt++; err = true;   }

                            if (err == false)
                            {
                                var existCL = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == d.oChurchLevel.AppGlobalOwnerId && c.Id != d.oChurchLevel.Id &&
                                            (c.CustomName.ToLower() == d.oChurchLevel.CustomName.ToLower() || c.Name.ToLower() == d.oChurchLevel.Name.ToLower()));

                                if (existCL > 0) { numErrUpdExistCnt++; err = true; }
                            }

                            if (err==false )
                            {
                                var oCL = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == d.oChurchLevel.AppGlobalOwnerId && c.Id == d.oChurchLevel.Id).FirstOrDefault(); // c.Name == "Level_" + i).FirstOrDefault();
                                if (oCL != null)  // && (countCL + oChLevelCntAdd + oChLevelCntUpd ) < oAGO.TotalLevels
                                {
                                    oCL.Name = d.oChurchLevel.Name; // "Level_" + i;
                                    oCL.CustomName = d.oChurchLevel.CustomName;  //"Level " + i;
                                    oCL.LevelIndex = d.oChurchLevel.LevelIndex; //i;
                                    oCL.Acronym = d.oChurchLevel.Acronym;
                                    oCL.PrefixKey = d.oChurchLevel.PrefixKey;
                                    oCL.AppGlobalOwnerId = d.oChurchLevel.AppGlobalOwnerId;
                                    oCL.SharingStatus = "N";
                                    oCL.LastMod = DateTime.Now;
                                    //
                                    oChLevelCntUpd++;
                                    _clBulkCtx.Update(oCL);
                                }
                            }                            
                        }

                        else if (d.oChurchLevel.Id == 0)  //add
                        {
                            var err = false;
                            if ((countCL + oChLevelCntAdd) >= oAGO.TotalLevels)  // + oChLevelCntUpd
                            { numErrAddExceedCnt++; err = true;  }

                            if (err == false)
                            { 
                                var existCL = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == d.oChurchLevel.AppGlobalOwnerId && 
                                            (c.CustomName.ToLower() == d.oChurchLevel.CustomName.ToLower() || c.Name.ToLower() == d.oChurchLevel.Name.ToLower()));

                                if (existCL > 0) { numErrAddExistCnt++; err = true; }
                            }
                           
                            if (err==false)
                            {
                                MSTRChurchLevel oCL = new MSTRChurchLevel()
                                {
                                    Name = d.oChurchLevel.Name,
                                    CustomName = d.oChurchLevel.CustomName,
                                    LevelIndex = d.oChurchLevel.LevelIndex,
                                    Acronym = d.oChurchLevel.Acronym,
                                    PrefixKey = d.oChurchLevel.PrefixKey,
                                    AppGlobalOwnerId = d.oChurchLevel.AppGlobalOwnerId, // oAGO.Id,
                                    SharingStatus = "N",
                                    Created = DateTime.Now,
                                    LastMod = DateTime.Now,
                                };

                                //
                                oChLevelCntAdd++;
                                _clBulkCtx.Add(oCL);
                            }                           
                        }                        
                    }
                }


                var _userTask = "";
                if ((oChLevelCntAdd + oChLevelCntUpd) > 0)
                {
                    if (oChLevelCntAdd > 0)
                    {
                        var strErrAdd = numErrAddExceedCnt > 0 ? numErrAddExceedCnt + " church levels could not be added. Total levels [" + oAGO.TotalLevels + "] reached." : "";
                        strErrAdd += numErrAddExistCnt > 0 ? numErrAddExistCnt + " church levels could not be added. Church level name already exist" : "";
                           
                        _userTask = "Added new " + oChLevelCntAdd + " church levels for " + strDesc.ToLower() + ", " + oAGO.OwnerName.ToUpper() + " successfully. " + strErrAdd;
                        ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Created " + oChLevelCntAdd + " church levels. Customization may be necessary. " + strErrAdd;
                    }

                    if (oChLevelCntUpd > 0)
                    {
                        var strErrUpd = numErrUpdExceedCnt > 0 ? numErrUpdExceedCnt + " church levels could not be updated. Total levels [" + oAGO.TotalLevels + "] exceeded." : "";
                        strErrUpd += numErrUpdExistCnt > 0 ? numErrUpdExistCnt + " church levels could not be updated. Church level name already exist" : "";

                        _userTask = !string.IsNullOrEmpty(_userTask) ? _userTask + ". " : "" + "Updated " + oChLevelCntUpd + " church levels for " + strDesc.ToLower() + ", " + oAGO.OwnerName.ToUpper() + " successfully. " + strErrUpd;
                        ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Denomination's " + oChLevelCntUpd + " church levels updated. Customization may be necessary. " + strErrUpd;
                    }

                    //save all...
                    await _clBulkCtx.SaveChangesAsync();

 

                    DetachAllEntities(_clBulkCtx); 

                    var _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: Church Level", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));


                    return Json(new { taskSuccess = true, userMess = ViewBag.UserMsg, pageIndex = vm.PageIndex });
                }                 

            }
                         
            return Json(new { taskSuccess = false, userMess = "Saving data failed. Please refresh and try again.", pageIndex = vm.PageIndex });
        }

        public IActionResult Delete_CL(int? loggedUserId, int id, bool forceDeleteConfirm = false)  // (int? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
        {
            var strDesc = "Church level "; var _tm = DateTime.Now; var _userTask = "Attempted deleting " + strDesc.ToLower();
            
            //
            try
            {
                var tm = DateTime.Now;
              //  var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];
                //
                var oCL = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.Id == id).FirstOrDefault(); // .Include(c => c.ChurchUnits)
                if (oCL == null)
                { 
                    _userTask = "Attempted deleting " + strDesc.ToLower();
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " to delete could not be retrieved." });
                }

                var saveDelete = true;
                // ensuring cascade delete where there's none!

                //check for the AGO and CB
                var oChurchBodies = _context.MSTRChurchBody.AsNoTracking().Where(c => c.ChurchLevelId == oCL.Id).ToList();
                // var oMTs = _clientContext.TransferTypeChurchLevel.Where(c => c.ChurchLevelId == oCL.Id).ToList(); 


                //using (var _clBulkCtx = new MSTR_DbContext(_cs))
                //{                    
                    if (oChurchBodies.Count() > 0)
                    {
                        var strConnTabs = oChurchBodies.Count() > 0 ? "Church unit" : "";
                        //strConnTabs = strConnTabs.Length > 0 ? strConnTabs + ", " : strConnTabs;
                        //strConnTabs = oMTs.Count() > 0 ? strConnTabs + "Church transfer" : strConnTabs;

                        if (forceDeleteConfirm == false)
                        {
                            saveDelete = false;
                            // check user privileges to determine... administrator rights
                            // log
                            _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + (oCL.CustomName != null ? oCL.CustomName : oCL.Name);  // var _userTask = "Attempted saving denomination (church)";
                            _tm = DateTime.Now;
                            _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                            return Json(new
                            {
                                taskSuccess = false,
                                tryForceDelete = false,
                                oCurrId = id,
                                userMess = "Specified " + strDesc.ToLower() + " to delete has been used elsewhere in the system [" + strConnTabs + "]. Delete cannot be done unless dependent-references are removed."
                            });

                            //super_admin task
                            //return Json(new { taskSuccess = false, tryForceDelete = true, oCurrId = id, userMess = "Specified " + strDesc.ToLower() + 
                            //       " has dependencies or links with other external data [Faith category]. Delete cannot be done unless child refeneces are removed. DELETE (cascade) anyway?" });
                        }
                    }

                    //successful...
                    if (saveDelete)
                    {
                        _context.MSTRChurchLevel.Remove(oCL);
                        _context.SaveChanges(); 
                        
                       
                       // DetachAllEntities(_clBulkCtx);

                        _userTask = "Deleted " + strDesc.ToLower() + ", " + (oCL.CustomName != null ? oCL.CustomName : oCL.Name) + " successfully";  // var _userTask = "Attempted saving " + strDesc;
                        _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                        return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oCL.Id, userMess = strDesc + " successfully deleted." });
                    }

                    else
                    {  
                       // DetachAllEntities(_clBulkCtx); 
                        return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" }); 
                    }
            
              //  }

            }

            catch (Exception ex)
            {
                _userTask = "Attempted deleting " + strDesc.ToLower() + ", [ID=" + id + "] but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
            }
        }



        public ActionResult Index_CB(int? oAppGloOwnId = null, int pageIndex = 1)  // int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            // SetUserLogged();
            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                // check permission 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                //
                if (!this.userAuthorized) return View(new MSTRChurchBodyModel()); //retain view    
                if (oUserLogIn_Priv == null) return View(new MSTRChurchBodyModel());
                if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) return View(new MSTRChurchBodyModel());
                var oLoggedUser = oUserLogIn_Priv.UserProfile;
             //   var oLoggedRole = oUserLogIn_Priv.UserRole;

                //
                var strDesc = "Church Unit (Congregation)";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                var oCBModel = new MSTRChurchBodyModel(); //TempData.Keep();    // int? oAppGloOwnId = null;
                var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
                //
                int? oAppGloOwnId_Logged = null;
                int? oChuBodyId_Logged = null;
                if (oChuBody_Logged != null)
                {
                    oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
                    oChuBodyId_Logged = oChuBody_Logged.Id; 
                }
                 
                var oUserId_Logged = oLoggedUser.Id;

                var lsCBMdl = (
                    from t_cb in _context.MSTRChurchBody.Include(t => t.Country).Include(t => t.CountryRegion).AsNoTracking()
                            .Where(c=> c.AppGlobalOwnerId == oAppGloOwnId && (c.OrgType == "CR" || c.OrgType=="CH" || c.OrgType == "CN")) // // jux for structure
                    from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_cb.AppGlobalOwnerId)                   
                    from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb.ChurchLevelId).DefaultIfEmpty()
                    from t_cb_p in _context.MSTRChurchBody.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN") && c.Id==t_cb.ParentChurchBodyId).DefaultIfEmpty()
                    from t_cl_p in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb_p.ChurchLevelId).DefaultIfEmpty()
                    from t_ci in _context.MSTRContactInfo.AsNoTracking().Where(c => c.AppGlobalOwnerId==t_cb.AppGlobalOwnerId && c.ChurchBodyId==t_cb.Id && c.Id == t_cb.ContactInfoId).DefaultIfEmpty() 
                    from t_ci_ago in _context.MSTRContactInfo.Include(t => t.Country).AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cl.AppGlobalOwnerId && c.ChurchBodyId == null && c.Id == t_ago.ContactInfoId).DefaultIfEmpty()
                         
                    select new MSTRChurchBodyModel()
                    {
                        oAppGlobalOwn = t_ago,
                        oChurchBody = t_cb, 
                        strOrgType = GetChuOrgTypeDesc(t_cb.OrgType),
                        strParentChurchBody = t_cb_p.Name,
                        //
                        strCountry = t_cb.Country != null ? t_cb.Country.EngName : "",
                        strCountryRegion = t_cb.CountryRegion != null ? t_cb.CountryRegion.Name : "", 
                        strAppGloOwn = t_ago.OwnerName + (!string.IsNullOrEmpty(t_ago.OwnerName) && t_ci_ago.Country != null ? ", " + t_ci_ago.Country.EngName : (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "")),
                        strChurchBody = t_cb.Name,
                        strParentCB_HeaderDesc = !string.IsNullOrEmpty(t_cl.CustomName) ? t_cl_p.CustomName : "Parent Unit", 
                        strChurchLevel = (t_cb.ChurchLevelId == null && t_cb.OrgType == "CR") ? "Church Root" : (!string.IsNullOrEmpty(t_cl.CustomName) ? t_cl.CustomName : t_cl.Name),
                        numCLIndex = t_cl.LevelIndex, 
                       // strCongLoc = t_ci.Location + (!string.IsNullOrEmpty(t_ci.Location) && !string.IsNullOrEmpty(t_ci.City) ? ", " + t_ci.City : t_ci_ago.City),
                        strCongLoc = (!string.IsNullOrEmpty(t_ci.Location) && !string.IsNullOrEmpty(t_ci.City) ? t_ci.Location + ", " + t_ci.City : t_ci.Location + t_ci.City).Trim(),
                        strCongLoc2 = (t_cb.CountryRegion != null && t_cb.Country != null ? t_cb.CountryRegion.Name + ", " + t_cb.Country.EngName : t_cb.CountryRegion.Name + t_cb.Country.EngName).Trim(),
                        blStatusActivated = t_cb.Status == "A", 
                        dtCreated = t_cb.Created,
                        //   
                        strStatus = GetStatusDesc(t_cb.Status)
                    })
                    .OrderByDescending(c=>c.dtCreated) //.OrderBy(c => c.strCountry).OrderBy(c => c.numCLIndex).OrderBy(c => c.strChurchBody)
                    .ToList();
                 


                oCBModel.lsChurchBodyModels = lsCBMdl;
                oCBModel.strCurrTask = strDesc;

                //                
                oCBModel.oAppGloOwnId = oAppGloOwnId;
                oCBModel.PageIndex = pageIndex;
                //oCBModel.oChurchBodyId = oCurrChuBodyId;
                //
                oCBModel.oUserId_Logged = oUserId_Logged;
                oCBModel.oChurchBody_Logged = oChuBody_Logged;
                oCBModel.oAppGloOwnId_Logged = oAppGloOwnId_Logged;

                oCBModel.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Status == "A")
                                             .OrderBy(c => c.OwnerName).ToList()
                                             .Select(c => new SelectListItem()
                                             {
                                                 Value = c.Id.ToString(),
                                                 Text = c.OwnerName
                                             })
                                             .ToList();

                //  
              //  // dashboard lookups
              //  ///
              //  ViewData["strAppName"] = "RhemaCMS";
              //  ViewData["strAppNameMod"] = "Admin Palette";
              //  ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(oLoggedUser.UserDesc) ? oLoggedUser.UserDesc : "[Current user]";
              //  ///
              //  ViewData["oAppGloOwnId_Logged"] = oAppGloOwnId_Logged;
              //  ViewData["oChuBodyId_Logged"] = oChuBodyId_Logged;

              //  ViewData["oCBOrgType_Logged"] = oChuBody_Logged.OrgType;  // CH, CN but subscriber may come from oth

              //  ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
              //  ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
              //  ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
              //  ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
              ////  ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc);
              //  ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

              //  //ViewData["strAppCurrUser_ChRole"] = oLoggedRole.RoleDesc; // "System Adminitrator";
              //  //ViewData["strAppCurrUser_RoleCateg"] = oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
                                
              //  ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
              //  ///

              //  //refresh Dash values
              //  _ = LoadDashboardValues();

                var tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oLoggedUser.Id, tm, tm, oLoggedUser.Id, oLoggedUser.Id));

                return View(oCBModel);
            }
        }

        [HttpGet]
        // public IActionResult AddOrEdit_CB(int id = 0, int? oAppOwnId = null, int? oParentCBId = null, int? oUserId_Logged = null) // int? oAGOId_Logged = null, int? oCBId_Logged = null, (int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        public IActionResult AddOrEdit_CB(int id = 0, int? oAppGloOwnId = null, int pageIndex = 1, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                try
                {
                 if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                    {
                        var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                        var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                        // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                        //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                        // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                        oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                        oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                        oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                        //var strDesc = "Church level";
                        //var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); // _userTask = "Attempted creating new denomination (church)"; // _userTask = "Opened denomination (church)-" + oCFT_MDL.oChurchFaithType.FaithDescription;
                        //var oCLModel = new MSTRChurchLevelModel();

                    //    var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                    //var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                    //// int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                    ////int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                    //// int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                    //oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                    //int? oCBId_Logged = null; // = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                    //int? oAGOId_Logged = null; // oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                   if (oAppGloOwnId == null)
                    {
                                
                                return PartialView("_ErrorPage");
                            }

                            //create user and init... 
                    var oAGO = _context.MSTRAppGlobalOwner.Find(oAppGloOwnId);
                    if (oAGO == null)
                     {
                                
                                return PartialView("_ErrorPage");
                            }


                        var strDesc = "Church body (Oversight unit)";
                        var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();
                        var oCBModel = new MSTRChurchBodyModel();
                        var tm = DateTime.Now;


                        // var oCBLevelCount = _context.MSTRChurchLevel.Count(c => c.AppGlobalOwnerId == oAGO.Id);

                        if (id == 0)
                        {  
                            //get CB parent
                            // var oCBPar = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oParentCBId).FirstOrDefault();
                            // oCBModel.oChurchBody = oCBPar;

                            //if (oCBPar != null) oCBModel.strParentChurchBody = oCBPar.Name + " (" + GetChuOrgTypeDetail(oCBPar.OrgType, false).ToString() + ")";
                            //string parCBCode = oCBPar.RootChurchCode;  // get the parent church body ... CB to be created first by Vendor... and picked up by the subscribers at the ChurchStructure ... congregation
                            //var oCLParCB = _context.MSTRChurchLevel.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBPar.ChurchLevelId).FirstOrDefault();
                            //var oCLCB = oCLParCB != null ? _context.MSTRChurchLevel.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.LevelIndex == oCLParCB.LevelIndex + 1).FirstOrDefault() : (MSTRChurchLevel)null;


                            //create user and init... 
                            oCBModel.oChurchBody = new MSTRChurchBody();
                            oCBModel.oChurchBody.AppGlobalOwnerId = oAppGloOwnId;
                            oCBModel.numCLIndex = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId);  //use what's configured... not digit at AGO

                            //  oCBLevelCount = oCBLevelCount >= 2 ? oCBLevelCount - 2 : 0;  // start at the lowest CB level ... CN will need [Max_Lev - 1] head-units ie. less HQ .. requesting CB itself

                            //
                            //var currCnt = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").Count() + 1; // (c.OrgType == "GB" || c.OrgType == "CN")  && (c.ChurchType=="CH" || c.ChurchType == "CF"))

                            //// GlobalChurchCode ::>>  [ RootChurchCode /Acronym ] - [7-digit] ... RCM-0000000, RCM-0000001, PCG-1234567, COP-1000000, ICGC-9999999
                            //var strGloChuCode = (!string.IsNullOrEmpty(oAppOwn.Acronym) ? oAppOwn.Acronym.ToUpper() + "-" : "") + currCnt.ToString();  //add preceding zero's

                            //// RootChurchCode ::>> RCM-000001--RCM-000001--RCM-000001--RCM-000001                                               
                            //var strCBFullCode = !string.IsNullOrEmpty(parCBCode) ? parCBCode.ToUpper() + "--" : "" + strGloChuCode;                    // var strLocalChuCode = (!string.IsNullOrEmpty(oAppOwn.Acronym) ? oAppOwn.Acronym.ToUpper() : "") + strCBCode;  //add preceding zero's ... 


                            //oCBModel.oChurchBody.CountryId = oCurrCtryId; 

                            oCBModel.oAppGlobalOwn = oAGO;

                            ////church code  ... must be done after CL is known... cos it's used in the code generation
                            //if (!string.IsNullOrEmpty(oCBModel.oAppGlobalOwn.PrefixKey))
                            //{ 
                            //    var template = new { taskSuccess = String.Empty, strRes = String.Empty };   
                            //    var jsCode = GetNextGlobalChurchCodeByAcronym_jsonString(oCBModel.oChurchBody.AppGlobalOwnerId, oCBModel.oAppGlobalOwn.PrefixKey, "");  // string json1 = @"{'Name':'James'}";
                            //    oCBModel.oChurchBody.GlobalChurchCode = jsCode;

                            //    //var jsOut = JsonConvert.DeserializeAnonymousType(jsCode, template);

                            //    //if (jsOut != null)
                            //    //    if (bool.Parse(jsOut.taskSuccess) == true)
                            //    //        oCBModel.oChurchBody.GlobalChurchCode = jsOut.strRes;
                            //}

                            // oCBModel.oChurchBody.RootChurchCode = strCBFullCode;
                            // oCBModel.oChurchBody.ChurchLevelId = oCLCB != null ? oCLCB.Id : (int?)null;
                            // oCBModel.oChurchBody.OrgType = // "CH", "CN";
                            //  oCBModel.oChurchBody.ParentChurchBodyId = oCBPar != null ? oCBPar.Id : (int?)null;
                             
                            oCBModel.oChurchBody.CtryAlpha3Code = oCBModel.oAppGlobalOwn.CtryAlpha3Code;
                            // oCBModel.oChurchBody.CountryRegionId = oCBPar != null ? oCBPar.CountryRegionId : (int?)null;
                             
                            oCBModel.oChurchBody.Status = "A";
                            oCBModel.blStatusActivated = true;
                            //
                            oCBModel.oChurchBody.Created = tm;
                            oCBModel.oChurchBody.LastMod = tm;
                            oCBModel.strAppGloOwn = oCBModel.oAppGlobalOwn.OwnerName;

                            _userTask = "Attempted creating new " + strDesc.ToLower() + ", " + oCBModel.oChurchBody.Name;



                            /////
                            //var oCTRYDef = _context.CountryCustom.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCBid && c.IsDefaultCountry == true).FirstOrDefault();
                            //oChurchMember.NationalityId = oCTRYDef != null ? oCTRYDef.CtryAlpha3Code : null;


                            // oChurchMember.MemberScope = "I"; // congregant    
                            ///
                           // oCBModel.strMemberScope = GetMemClassDesc(oChurchMember.MemberScope);

                            oCBModel.oCBContactInfo = new MSTRContactInfo();
                            oCBModel.oCBContactInfo.AppGlobalOwnerId = oAppGloOwnId; 
                            oCBModel.oCBContactInfo.ChurchBodyId = oCBModel.oChurchBody.Id;
                            oCBModel.oCBContactInfo.CtryAlpha3Code = oCBModel.oChurchBody.CtryAlpha3Code;       /// oCTRYDef != null ? oCTRYDef.CtryAlpha3Code : null;

                            ///

                        }

                        else
                        {
                            oCBModel = (
                                         from t_cb in _context.MSTRChurchBody.Include(t => t.Country).Include(t => t.CountryRegion).AsNoTracking()
                                            .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id==id && (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN"))
                                         from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_cb.AppGlobalOwnerId)
                                         from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb.ChurchLevelId) //.DefaultIfEmpty()                                         
                                         from t_cb_p in _context.MSTRChurchBody.AsNoTracking()
                                                 .Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN") && c.Id == t_cb.ParentChurchBodyId).DefaultIfEmpty()
                                         from t_cl_p in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb_p.ChurchLevelId).DefaultIfEmpty()
                                         from t_ci in _context.MSTRContactInfo.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && (c.ChurchBodyId == t_cb.Id || c.Id == t_cb.ContactInfoId)).Take(1).DefaultIfEmpty()
                                         from t_ci_ago in _context.MSTRContactInfo.Include(t => t.Country).AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cl.AppGlobalOwnerId && c.ChurchBodyId == null && c.Id == t_ago.ContactInfoId).DefaultIfEmpty()


                                         select new MSTRChurchBodyModel()
                                         {
                                             oAppGlobalOwn = t_ago,
                                             oChurchBody = t_cb, 
                                             oChurchLevel = t_cl,
                                             strOrgType = GetChuOrgTypeDesc(t_cb.OrgType),
                                             strParentChurchBody = t_cb_p.Name,
                                             oCBContactInfo = t_ci != null ? t_ci : new MSTRContactInfo(),  /// each CB must av a CI
                                             ///
                                             strCountry = t_cb.Country != null ? t_cb.Country.EngName : "",
                                             strCountryRegion = t_cb.CountryRegion != null ? t_cb.CountryRegion.Name : "",
                                             strAppGloOwn = t_ago.OwnerName + (!string.IsNullOrEmpty(t_ago.OwnerName) && t_ci_ago.Country != null ? ", " + t_ci_ago.Country.EngName : (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "")),
                                             strChurchBody = t_cb.Name,
                                             strParentCB_HeaderDesc = t_cl_p != null ? t_cl_p.CustomName : "",
                                             strParentCBLevel = t_cb_p != null ? (t_cb_p.ChurchLevel != null ? (!string.IsNullOrEmpty(t_cb_p.ChurchLevel.CustomName) ? t_cb_p.ChurchLevel.CustomName : t_cb_p.ChurchLevel.Name) : "") : "",
                                             
                                             strChurchLevel = !string.IsNullOrEmpty(t_cl.CustomName) ? t_cl.CustomName : t_cl.Name,
                                             numCLIndex = t_cl.LevelIndex,
                                            // strCongLoc = t_ci.Location + (!string.IsNullOrEmpty(t_ci.Location) && !string.IsNullOrEmpty(t_ci.City) ? ", " + t_ci.City : t_ci_ago.City),
                                             strCongLoc = (!string.IsNullOrEmpty(t_ci.Location) && !string.IsNullOrEmpty(t_ci.City) ? t_ci.Location + ", " + t_ci.City : t_ci.Location + t_ci.City).Trim(),
                                             strCongLoc2 = (t_cb.CountryRegion != null && t_cb.Country != null ? t_cb.CountryRegion.Name + ", " + t_cb.Country.EngName : t_cb.CountryRegion.Name + t_cb.Country.EngName).Trim(),
                                             blStatusActivated = t_cb.Status == "A", 
                                             //   
                                             strStatus = GetStatusDesc(t_cb.Status)
                                         })
                                         .FirstOrDefault();

                            if (oCBModel == null)
                            { return PartialView("_ErrorPage"); }

                            //if (oCBModel.oAppGlobalOwn == null)
                            //{ return PartialView("_ErrorPage"); }

                            if (oCBModel.oChurchBody == null)
                            { return PartialView("_ErrorPage"); }


                            //church code  ... CL known!
                            if (string.IsNullOrEmpty(oCBModel.oChurchBody.GlobalChurchCode) && !string.IsNullOrEmpty(oCBModel.oAppGlobalOwn.PrefixKey))
                            {
                                //_oChanges.GlobalChurchCode = _oChanges.PrefixKey + string.Format("{0:D3}", 0); 
                                //_oChanges.RootChurchCode = _oChanges.GlobalChurchCode;

                                //var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
                                var jsCode = GetNextGlobalChurchCodeByAcronym_jsonString(oCBModel.oChurchBody.AppGlobalOwnerId, oCBModel.oAppGlobalOwn.PrefixKey, oCBModel.oChurchLevel.PrefixKey);  // string json1 = @"{'Name':'James'}";
                                oCBModel.oChurchBody.GlobalChurchCode = jsCode;
                            }

                            //root church code  
                            if (string.IsNullOrEmpty(oCBModel.oChurchBody.RootChurchCode) && !string.IsNullOrEmpty(oCBModel.oChurchBody.GlobalChurchCode))
                            {
                                // _oChanges.RootChurchCode = _oChanges.GlobalChurchCode;

                                //var template = new { taskSuccess = String.Empty, strRes = String.Empty };
                                var jsCode = GetNextRootChurchCodeByParentCB_jsonString(oCBModel.oChurchBody.AppGlobalOwnerId, oCBModel.oChurchBody.ParentChurchBodyId, oCBModel.oChurchBody.GlobalChurchCode, oCBModel.oAppGlobalOwn.PrefixKey, oCBModel.oChurchLevel.PrefixKey);
                                oCBModel.oChurchBody.RootChurchCode = jsCode;

                                //var jsOut = JsonConvert.DeserializeAnonymousType(jsCode, template);

                                //if (jsOut != null)
                                //    if (bool.Parse(jsOut.taskSuccess) == true)
                                //        oCBModel.oChurchBody.RootChurchCode = jsOut.strRes;
                            }

                           // oCBLevelCount = oCBModel.numCLIndex;  // oCBModel.numCLIndex != 0 ? oCBModel.numCLIndex - 1 : 0;
                            _userTask = "Opened " + strDesc.ToLower() + ", " + oCBModel.oChurchBody.Name;
                        }

                        
                        // oCBModel.setIndex = setIndex;

                        oCBModel.PageIndex = pageIndex ;
                        oCBModel.oUserId_Logged = oUserId_Logged;
                        oCBModel.oAppGloOwnId_Logged = oAGOId_Logged;
                        oCBModel.oChurchBodyId_Logged = oCBId_Logged;


                        /// set the lookups for the church bodies
                        ///  
                        //if (oCBLevels.Count > 0)
                        //{
                        //    ViewBag.Filter_fr_fn = ViewBag.Filter_fn = !string.IsNullOrEmpty(oCBLevels[0].CustomName) ? oCBLevels[0].CustomName : oCBLevels[6].Name;
                        //   



                        /// -----------  >>>>>..   use the Church Browser!
                        ///

                        //oCBModel.oCBLevelCount = oCBModel.numCLIndex - 1;  // oCBLevelCount -= 2;  // less requesting CB
                        //List<MSTRChurchLevel> oCBLevelList = _context.MSTRChurchLevel.AsNoTracking()
                        //    .Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && c.LevelIndex > 0 && c.LevelIndex < oCBModel.numCLIndex)
                        //    .ToList().OrderBy(c=>c.LevelIndex).ToList();
                        /////
                        //if (oCBModel.oCBLevelCount > 0 && oCBLevelList.Count > 0)
                        //{
                        //    oCBModel.strChurchLevel_1 = !string.IsNullOrEmpty(oCBLevelList[0].CustomName) ? oCBLevelList[0].CustomName : oCBLevelList[0].Name;
                        //    ViewBag.strChurchLevel_1 = oCBModel.strChurchLevel_1;
                        //    ///
                        //    var oCB_1 = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                        //                      .Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && // c.Status == "A" && 
                        //                            c.ChurchLevel.LevelIndex == 1 && c.OrgType == "CR") //c.ChurchLevelId == oCBLevelList[0].Id &&
                        //                      .FirstOrDefault();

                        //    if (oCB_1 != null)
                        //        { oCBModel.ChurchBodyId_1 = oCB_1.Id;  oCBModel.strChurchBody_1 = oCB_1.Name + " (Church Root)";  }

                        //    ViewBag.ChurchBodyId_1 = oCBModel.ChurchBodyId_1; 
                        //    ViewBag.strChurchBody_1 = oCBModel.strChurchBody_1;

                        //    ///
                        //    if (oCBModel.oCBLevelCount > 1)
                        //    {
                        //        oCBModel.strChurchLevel_2 = !string.IsNullOrEmpty(oCBLevelList[1].CustomName) ? oCBLevelList[1].CustomName : oCBLevelList[1].Name;
                        //        ViewBag.strChurchLevel_2 = oCBModel.strChurchLevel_2;
                        //        ///
                        //        var lsCB_2 = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && c.ChurchLevelId == oCBLevelList[1].Id).ToList();
                        //        var oCB_2 = lsCB_2.Where(c=> IsAncestor_ChurchBody(c.RootChurchCode, oCBModel.oChurchBody.RootChurchCode, c.Id, oCBModel.oChurchBody.ParentChurchBodyId)).ToList();
                        //        if (oCB_2 .Count() != 0)
                        //        { oCBModel.ChurchBodyId_2 = oCB_2[0].Id; }
                        //        ViewBag.ChurchBodyId_2 = oCBModel.ChurchBodyId_2; 

                        //        /// 
                        //        //oCBModel.lkp_ChurchBodies_2 = _context.MSTRChurchBody.Include(t=>t.ChurchLevel)
                        //        //                    .Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && c.Status == "A" && c.ChurchLevel.LevelIndex == 2 && //c.ChurchLevelId == oCBLevelList[0].Id &&
                        //        //                    (c.OrgType == "CH" || c.OrgType == "CN"))
                        //        //              .Select(c => new SelectListItem()
                        //        //              {
                        //        //                  Value = c.Id.ToString(),
                        //        //                  Text = c.Name
                        //        //              })
                        //        //              .OrderBy(c => c.Text)
                        //        //              .ToList();
                        //        // oCBModel.lkp_ChurchBodies_1.Insert(0, new SelectListItem { Value = "", Text = "Select" });   

                        //        if (oCBModel.oCBLevelCount > 2)
                        //        {
                        //            oCBModel.strChurchLevel_3 = !string.IsNullOrEmpty(oCBLevelList[2].CustomName) ? oCBLevelList[2].CustomName : oCBLevelList[2].Name;
                        //            ViewBag.strChurchLevel_3 = oCBModel.strChurchLevel_3;

                        //            var lsCB_3 = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && c.ChurchLevelId == oCBLevelList[2].Id).ToList();
                        //            var oCB_3 = lsCB_3.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oCBModel.oChurchBody.RootChurchCode, c.Id, oCBModel.oChurchBody.ParentChurchBodyId)).ToList();
                        //            if (oCB_3.Count() != 0)
                        //            { oCBModel.ChurchBodyId_3 = oCB_3[0].Id; }
                        //            ViewBag.ChurchBodyId_3 = oCBModel.ChurchBodyId_3;
                                     

                        //            if (oCBModel.oCBLevelCount > 3)
                        //            {
                        //                oCBModel.strChurchLevel_4 = !string.IsNullOrEmpty(oCBLevelList[3].CustomName) ? oCBLevelList[3].CustomName : oCBLevelList[3].Name;
                        //                ViewBag.strChurchLevel_4 = oCBModel.strChurchLevel_4;

                        //                var lsCB_4 = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && c.ChurchLevelId == oCBLevelList[3].Id).ToList();
                        //                var oCB_4 = lsCB_4.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oCBModel.oChurchBody.RootChurchCode, c.Id, oCBModel.oChurchBody.ParentChurchBodyId)).ToList();
                        //                if (oCB_4.Count() != 0)
                        //                { oCBModel.ChurchBodyId_4 = oCB_4[0].Id; }
                        //                ViewBag.ChurchBodyId_4 = oCBModel.ChurchBodyId_4;


                        //                if (oCBModel.oCBLevelCount > 4)
                        //                {
                        //                    oCBModel.strChurchLevel_5 = !string.IsNullOrEmpty(oCBLevelList[4].CustomName) ? oCBLevelList[4].CustomName : oCBLevelList[4].Name;
                        //                    ViewBag.strChurchLevel_5 = oCBModel.strChurchLevel_4;

                        //                    var lsCB_5 = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCBModel.oChurchBody.AppGlobalOwnerId && c.ChurchLevelId == oCBLevelList[4].Id).ToList();
                        //                    var oCB_5 = lsCB_5.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oCBModel.oChurchBody.RootChurchCode, c.Id, oCBModel.oChurchBody.ParentChurchBodyId)).ToList();
                        //                    if (oCB_5.Count() != 0)
                        //                    { oCBModel.ChurchBodyId_5 = oCB_5[0].Id; }
                        //                    ViewBag.ChurchBodyId_5 = oCBModel.ChurchBodyId_5;
                        //                }
                        //            }
                        //        }
                        //    }
                        //} 
                         







                        //
                        // oCBModel.oAppGloOwnId = oAppGloOwnId;
                        // oCBModel.oChurchBodyId = oCurrChuBodyId;
                        //  var oCurrChuBody = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                        // oCBModel.oChurchBody = oCurrChuBody != null ? oCurrChuBody : null;



                       oCBModel = this.popLookups_CB(oCBModel, oCBModel.oChurchBody);
                                                

                    ///   var tm = DateTime.Now;
                        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oUserId_Logged, tm, tm, oUserId_Logged, oUserId_Logged));


                        var _oCBModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCBModel);
                        TempData["oVmCurrMod"] = _oCBModel; TempData.Keep();

                        return PartialView("_AddOrEdit_CB", oCBModel);

                    }

                    //page not found error
                    
                    return PartialView("_ErrorPage");

                }

                catch (Exception ex)
                {
                    //page not found error
                    
                    return PartialView("_ErrorPage");
                } 
            }
        }
        public MSTRChurchBodyModel popLookups_CB(MSTRChurchBodyModel vm, MSTRChurchBody oCurrChurchBody )
        {
            if (vm == null || oCurrChurchBody == null) return vm;
            //
            vm.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vm.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vm.lkpOrgTypes = new List<SelectListItem>();  // CR, CH. CN
            foreach (var dl in dlCBDivOrgTypes) { vm.lkpOrgTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc  }); }
            

            vm.lkpChurchWorkStatuses = new List<SelectListItem>();
            foreach (var dl in dlChuWorkStats) { vm.lkpChurchWorkStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            //vm.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.Where(c => c.Status == "A")
            //                                   .OrderBy(c => c.OwnerName).ToList()
            //                                   .Select(c => new SelectListItem()
            //                                   {
            //                                       Value = c.Id.ToString(),
            //                                       Text = c.OwnerName
            //                                   })
            //                                   .ToList();

            //vm.lkpAppGlobalOwns.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vm.lkpChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId)
                                              .OrderByDescending(c => c.LevelIndex)
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name,
                                                  //Disabled = c.LevelIndex == 1  // the base unit is auto... allow but confirm only one root CR... to allow manual manipulations at vendor side
                                              }) 
                                              .ToList();

            //vm.lkpChurchLevels.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            //vm.lkpChurchCategories = _context.MSTRChurchBody.Include(t => t.ChurchLevel)
            //                .Where(c => c.Id != oCurrChurchBody.Id && //c.Id != oCurrChurchBody.ParentChurchBodyId &&
            //                                 c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId) // &&  c.ChurchType=="CH"  )
            //                                                                  //  (c.ChurchLevel.LevelIndex == oCurrChurchBody.ChurchLevel.LevelIndex + 1 || c.ChurchLevel.LevelIndex == oCurrChurchBody.ChurchLevel.LevelIndex - 1))
            //                                      .OrderBy(c => c.ChurchLevel.LevelIndex).ThenBy(c => c.Name).ToList()
            //                                      .Select(c => new SelectListItem()
            //                                      {
            //                                          Value = c.Id.ToString(),
            //                                          Text = c.Name
            //                                      })
            //                                      .ToList();

            //vm.lkpChurchCategories.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            vm.lkpCountries = _context.MSTRCountry.ToList()  //.Where(c => c.Display == true)
                                          .Select(c => new SelectListItem()
                                          {
                                              Value = c.CtryAlpha3Code, // .ToString(),
                                              Text = c.EngName
                                          })
                                          .OrderBy(c => c.Text)
                                          .ToList();
           // vm.lkpCountries.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            //vm.lkpCountryRegions = _context.MSTRCountryRegion.Include(t => t.Country).ToList()  //.Where(c => c.Country.Display == true)
            //                                   .Select(c => new SelectListItem()
            //                                   {
            //                                       Value = c.Id.ToString(),
            //                                       Text = c.Name
            //                                   })
            //                                   .OrderBy(c => c.Text)
            //                                   .ToList();
            //vm.lkpCountryRegions.Insert(0, new SelectListItem { Value = null, Text = "Select" });
             

            return vm;
        }
        public JsonResult GetCountryByParentChurchBody(int? oParentCBId, int? oAppGloOwnId)
        {
            var cb = _context.MSTRChurchBody.AsNoTracking()  // .Include(t => t.FaithTypeClass)
                .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oParentCBId).FirstOrDefault();

            var res = cb != null;
            var _strResId = cb != null ? cb.Id : (int?)null;
            var _strRes = cb != null ? cb.Name : "";
            return Json(new { taskSuccess = res, strResId = _strResId, strRes = _strRes });

            //if (addEmpty) countryList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            //return Json(countryList);
        }
        public JsonResult GetInitChurchBodyListByAppGloOwn(int? oAppGloOwnId,  bool addEmpty = false)
        {
            var oCBList = new List<SelectListItem>();
            ///
            oCBList = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                       .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchLevel.LevelIndex == 2 && // c.Status == "A" && 
                       c.OrgType == "CH")
                   .OrderBy(c => c.Name)
                   .ToList()
                   .Select(c => new SelectListItem()
                   {
                       Value = c.Id.ToString(),
                       Text = c.Name
                   })
                   .OrderBy(c => c.Text)
                   .ToList();
            ///
            if (addEmpty) oCBList.Insert(0, new SelectListItem { Value = "", Text = "Select..." });
            return Json(oCBList);
        } 
        public JsonResult GetChurchLevelIndexByChurchLevel(int? oChurchLevelId, int? oAppGloOwnId, bool addEmpty = false)
        {
            var oCBList = new List<SelectListItem>();
            ///
            var oCL = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oChurchLevelId).FirstOrDefault();
            var res = oCL != null;
            var _numResLev = oCL != null ? oCL.LevelIndex : (int?)null;
            // !string.IsNullOrEmpty(oCBLevelList[0].CustomName) ? oCBLevelList[0].CustomName : oCBLevelList[0].Name
            var _strRes = oCL != null ? (!string.IsNullOrEmpty(oCL.CustomName) ? oCL.CustomName : oCL.Name) : "";
            ///
            return Json(new { taskSuccess = res, numResLev = _numResLev, strRes = _strRes }); 
        }
        public JsonResult GetChurchLevelIndexesByChurchLevel(int? oChurchLevelId, int? oAppGloOwnId, bool addEmpty = false)
        {
            var oCBList = new List<SelectListItem>();
            ///
            var oCL = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oChurchLevelId).FirstOrDefault();
            var res = oCL != null;
            var _numResLev = oCL != null ? oCL.LevelIndex : (int?)null;
            /// 

            if (oCL != null)
            {
                var oCLs = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.LevelIndex <= oCL.LevelIndex).OrderBy(c=>c.LevelIndex).ToList();
                var _strRes = "";
                foreach (var oChuLev in  oCLs)
                {
                    var strRes = oChuLev != null ? (!string.IsNullOrEmpty(oChuLev.CustomName) ? oChuLev.CustomName : oChuLev.Name) : "";
                    _strRes += strRes + ",";
                }

                _strRes = _strRes.Contains(",") ? _strRes.Remove(_strRes.LastIndexOf(",")) : _strRes;

                //  get the first CB
                var oCB_1 = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                                 .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && // c.Status == "A" && 
                                       c.ChurchLevel.LevelIndex == 1 && c.OrgType == "CR") //c.ChurchLevelId == oCBLevelList[0].Id &&
                                 .FirstOrDefault();

                var _numChurchBodyId_1 = (int?)null; var _strChurchBody_1 = "";
                if (oCB_1 != null)
                {  _numChurchBodyId_1 = oCB_1.Id;  _strChurchBody_1 = oCB_1.Name + " (Church Root)"; } 

                ///
                return Json(new { taskSuccess = res, numResLev = _numResLev, strResList = _strRes, numChurchBodyId_1 = _numChurchBodyId_1, strChurchBody_1 = _strChurchBody_1 });
            }


              return Json(new { taskSuccess = res, numResLev = _numResLev, strResList = "" }); 
        }
        public JsonResult GetChurchBodyListByParentBody(int? oParentCBId, int? oAppGloOwnId, string strOrgType = null, bool addEmpty = false)
        {
            var oCBList = _context.MSTRChurchBody.AsNoTracking()  //.Include(t => t.ChurchLevel)
                .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ParentChurchBodyId == oParentCBId && // c.Status == "A" && 
                        (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN"))
                       // (c.OrgType == strOrgType || (strOrgType == null && (c.OrgType == "CH" || c.OrgType == "CN"))))
                .OrderBy(c => c.Name)
                .ToList()
            .Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .OrderBy(c => c.Text)
            .ToList(); 

            if (addEmpty) oCBList.Insert(0, new SelectListItem { Value = "", Text = "Select..." });
            return Json(oCBList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_CB(MSTRChurchBodyModel vm)
        {

            var strDesc = "Church unit";
            if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.PageIndex });
            if (vm.oChurchBody == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.PageIndex });

            MSTRChurchBody _oChanges = vm.oChurchBody;
            MSTRContactInfo _oChangesCI = vm.oCBContactInfo;


            var initCBNetwork = false;
            var configCBDefaultUsers = false;
            var isNewCB = false;

            /// server validations
            ///   
            if (string.IsNullOrEmpty(_oChanges.OrgType))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church unit organisation type [Congregation or Congregation Head-unit] is not specified", pageIndex = vm.PageIndex });
            strDesc = (_oChanges.OrgType == "CR" || _oChanges.OrgType == "CH") ? "Congregation Head-unit" : _oChanges.OrgType == "CN" ? "Congregation" : "Church unit";

            if (_oChanges.AppGlobalOwnerId == null)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the denomination (church).", pageIndex = vm.PageIndex });

            var oAGO = _context.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
            if (oAGO == null)  // let's know the denomination... for prefic code
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Denomination (church) for " + strDesc.ToLower() + " could not be found. Please refresh and try again", pageIndex = vm.PageIndex });

            // check...
            if (string.IsNullOrEmpty(oAGO.PrefixKey))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church prefix code has not been specified. Hint: configure via Denominations" });

            if (string.IsNullOrEmpty(_oChanges.Name))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the " + strDesc.ToLower() + " name", pageIndex = vm.PageIndex }); 

            if (_oChanges.ChurchLevelId == null)  
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the church level.", pageIndex = vm.PageIndex });

            var oCBLevel = _context.MSTRChurchLevel.AsNoTracking().Where(c=> c.AppGlobalOwnerId==_oChanges.AppGlobalOwnerId && c.Id == _oChanges.ChurchLevelId).FirstOrDefault();
            if (oCBLevel == null)  // ... parent church level > church unit level
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church unit level could not be found. Please refresh and try again", pageIndex = vm.PageIndex });
                        
            var oCBRootList = _context.MSTRChurchBody.AsNoTracking().Where(c=> c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.OrgType == "CR").ToList();
            var oCBRootCount = oCBRootList.Count();
            if (oCBRootCount == 0)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church Head (Apex) was not found. Manually create church root and try again.", pageIndex = vm.PageIndex });
            
            if (oCBRootCount > 1)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church Head (Apex) cannot be more than one. Modify church root and try again.", pageIndex = vm.PageIndex });

            if (_oChanges.Id == 0 && _oChanges.OrgType == "CR" && oCBRootCount > 0)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church Head (Apex) already created. Denomination can have one one church root, and then networked with lower level church units.", pageIndex = vm.PageIndex });

            if (_oChanges.Id > 0 && _oChanges.OrgType == "CR")
            {
                var oCBRootListExcl = oCBRootList.Where(c => c.Id != _oChanges.Id).ToList();
                if (oCBRootListExcl.Count > 0)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church Head (Apex) already created. Denomination can have one one church root. Hint: '" + oCBRootListExcl[0].Name + "' configured as Church Head", pageIndex = vm.PageIndex });
            }

            ///// get the parent id
            ///// 
            //var parDesc = "church unit";
            //switch (vm.oCBLevelCount)
            //{
            //    case 1: _oChanges.ParentChurchBodyId = vm.ChurchBodyId_1; parDesc = vm.strChurchLevel_1; break;
            //    case 2: _oChanges.ParentChurchBodyId = vm.ChurchBodyId_2; parDesc = vm.strChurchLevel_2; break;
            //    case 3: _oChanges.ParentChurchBodyId = vm.ChurchBodyId_3; parDesc = vm.strChurchLevel_3; break;
            //    case 4: _oChanges.ParentChurchBodyId = vm.ChurchBodyId_4; parDesc = vm.strChurchLevel_4; break;
            //    case 5: _oChanges.ParentChurchBodyId = vm.ChurchBodyId_5; parDesc = vm.strChurchLevel_5; break;
            //} 

            if (_oChanges.OrgType != "CR" && _oChanges.ParentChurchBodyId == null)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church structure is networked. Provide the parent body" , pageIndex = vm.PageIndex });  // + parDesc.ToLower()

            MSTRChurchBody oCBParent = null;
            if (_oChanges.OrgType != "CR") ///  && oCBParent != null)
            {
                oCBParent = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.Id == _oChanges.ParentChurchBodyId).FirstOrDefault();
                if (oCBParent == null)  //_oChanges.OrgType != "CR" &&  ... let's know the parent church unit... parent church level > church unit level
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Parent church body could not be found. Please refresh and try again", pageIndex = vm.PageIndex });

                if (oCBLevel.LevelIndex <= oCBParent.ChurchLevel.LevelIndex)  // ... parent church level > church unit level
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church body level cannot be higher or same as parent church unit. Please select the correct parent unit or change church unit level", pageIndex = vm.PageIndex });

                // check... parent code
                if (string.IsNullOrEmpty(oCBParent.GlobalChurchCode) || string.IsNullOrEmpty(oCBParent.RootChurchCode))   /// (oCBParent.OrgType != "CR" && )
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Parent body church code or root code not specified. Please check parent and try again", pageIndex = vm.PageIndex });

            }
                
            if (_oChanges.CtryAlpha3Code == null)   // auto-fill the country and regions using the parent details...
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the base country.", pageIndex = vm.PageIndex }); 
               

            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<MSTRChurchBodyModel>(arrData) : vm;
            var oCB = vmMod.oChurchBody;
             

            // contact info ...  direct config ... rarely wil this happen [ so you may av to provide manual option to add before save.... in case]
            if (_oChangesCI == null)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church contact info is not specified. Please contact details and try again.", signOutToLogIn = false });


            if (string.IsNullOrEmpty(_oChangesCI.Email))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church email is required please [for correspondences]. Hint: someone's personal email may be temporarily used.", signOutToLogIn = false });

                if (string.IsNullOrEmpty(_oChangesCI.MobilePhone1) && string.IsNullOrEmpty(_oChangesCI.MobilePhone2))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church phone contact is required please [for SMS notifications where necessary]. Hint: someone's phone may be temporarily used.", signOutToLogIn = false });

                if (string.IsNullOrEmpty(_oChangesCI.Location) && string.IsNullOrEmpty(_oChangesCI.DigitalAddress))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church location or digital address must be specified please.", signOutToLogIn = false });


                if (_oChangesCI.AppGlobalOwnerId == null) _oChangesCI.AppGlobalOwnerId = _oChanges.AppGlobalOwnerId;
                if (_oChangesCI.ChurchBodyId == null) _oChangesCI.ChurchBodyId = _oChanges.Id;

                //Email... must be REQUIRED -- for password reset!
                if (!string.IsNullOrEmpty(_oChangesCI.Email))
                {
                    //  ... check validity... REGEX 
                    if (!AppUtilties.IsValidEmail(_oChangesCI.Email))
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Email specified is invalid. Please check and try again.", signOutToLogIn = false });

                    //// use email to check for member duplication across denomination ... disallow impersonation within same CB... mem can be at diff levels or congregations but must NOT be Active [historic data]
                    //var oCIEmailMem = _context.ChurchMember.AsNoTracking().Include(t => t.ChurchBody)
                    //    .Where(c => c.AppGlobalOwnerId == _oChangesCI.AppGlobalOwnerId &&
                    //               (c.ChurchBodyId == _oChangesCI.ChurchBodyId || (c.ChurchBodyId != _oChangesCI.ChurchBodyId && c.Status == "A")) && // [ c.Status=="A" ] stat summary in sync with member_status [available - regular, invalid /not available - distant, virtual, past, passed]
                    //               (_oChanges.Id == 0 || (_oChanges.Id > 0 && c.Id != _oChanges.Id)) && c.ContactInfo.Email == _oChangesCI.Email).FirstOrDefault();
                    //if (oCIEmailMem != null)
                    //    return Json(new
                    //    {
                    //        taskSuccess = false,
                    //        oCurrId = _oChanges.Id,
                    //        userMess = "Email must be unique. Email already used by another: [ Member: " +
                    //        GetConcatMemberName(oCIEmailMem.Title, oCIEmailMem.FirstName, oCIEmailMem.MiddleName, oCIEmailMem.LastName, false, false, false, false, false) + (oCIEmailMem.ChurchBody != null ? " / " + oCIEmailMem.ChurchBody.Name : "") + " ]",
                    //        signOutToLogIn = false
                    //    });



                    //// temp.... uncomment later! or allow vendor only
                    //var oCIEmailExist = _context.MSTRContactInfo.AsNoTracking()  /// .Include(t => t.ChurchBody)
                    //            .Where(c => c.AppGlobalOwnerId == _oChangesCI.AppGlobalOwnerId && c.ChurchBodyId != _oChanges.Id &&
                    //            ///((_oChanges.Id == 0 || _oChangesCI.Id == 0) || (_oChanges.Id > 0 && c.Id != _oChangesCI.Id)) && 
                    //            c.Email == _oChangesCI.Email).FirstOrDefault();
                    //if (oCIEmailExist != null)
                    //{
                    //    var oCBExist_CI = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCIEmailExist.AppGlobalOwnerId && c.Id == oCIEmailExist.ChurchBodyId).FirstOrDefault();
                    //    return Json(new
                    //        {
                    //            taskSuccess = false,
                    //            oCurrId = _oChanges.Id,
                    //            userMess = "Email must be unique. Email already used by another: [Church: " +
                    //             (oCBExist_CI != null ? " / " + oCBExist_CI.Name : " <elsewhere>") + " ]",
                    //            signOutToLogIn = false
                    //        }); 
                    //}
                }
                      

                if (!string.IsNullOrEmpty(_oChangesCI.Website))
                {
                    //  ... check validity... REGEX 
                    if (!AppUtilties.IsValidURL(_oChangesCI.Website))
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Url of member website (or blog) invalid. Please check and try again.", signOutToLogIn = false });
                    }
                }

                if (_oChangesCI.ResAddrSameAsPostAddr) _oChangesCI.PostalAddress = _oChangesCI.ResidenceAddress;
            

            //else //if (_oChangesCI == null)  // check for other contact details available but not primary and link up... use the Member Id ref on Member CI table ... in all 3 instances
            //{
            //    var oCBCI = _context.MSTRContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && x.ChurchBodyId == _oChanges.Id && 
            //                                                                    x.Id == _oChanges.ContactInfoId).FirstOrDefault();

            //    if (oCBCI == null)
            //        oCBCI = _context.MSTRContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && x.ChurchBodyId == _oChanges.Id).FirstOrDefault();

            //    //if (oCBCI == null)
            //    //    oCBCI = _context.MSTRContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && x.ChurchBodyId == _oChanges.Id &&
            //    //                                                x.ChurchMemberId == _oChanges.Id).FirstOrDefault();

            //    /// found? .. assign  ... pass thru the usual validations else via Member Profile Pane
            //    if (oCBCI != null) _oChangesCI = oCBCI;
                 
            //}



            //   var oCL = vmMod.oChurchLevel;

            try
            {
                ModelState.Remove("oChurchBody.AppGlobalOwnerId");
                //ModelState.Remove("oChurchBody.CountryId");
                ModelState.Remove("oChurchBody.CtryAlpha3Code");
                ModelState.Remove("oChurchBody.CountryRegionId");
                ModelState.Remove("oChurchBody.ParentChurchBodyId");
                ModelState.Remove("oChurchBody.ContactInfoId");
                ModelState.Remove("oChurchBody.ChurchLevelId");
                ModelState.Remove("oChurchBody.OrgType");
                ModelState.Remove("oChurchBody.GlobalChurchCode");
                ModelState.Remove("oChurchBody.RootChurchCode"); 
              //  ModelState.Remove("oAppGlobalOwn.OwnerName");
                //
                //ModelState.Remove("oCurrChurchBody.Id");
                //ModelState.Remove("oCurrChurchBody.Name");
                //
                
                ModelState.Remove("oChurchBody.CreatedByUserId");
                ModelState.Remove("oChurchBody.LastModByUserId");

                ModelState.Remove("oCBContactInfo.AppGlobalOwnerId");
                ModelState.Remove("oCBContactInfo.ChurchBodyId");
                ModelState.Remove("oCBContactInfo.ChurchMemberId");
                ModelState.Remove("oCBContactInfo.CtryAlpha3Code");
                ModelState.Remove("oCBContactInfo.RegionId");

                ModelState.Remove("oCBContactInfo.CreatedByUserIdCI");
                ModelState.Remove("oCBContactInfo.LastModByUserId");


                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", pageIndex = vm.PageIndex });


                //church code  ... must be done after CL is known... cos it's used in the code generation
                if (string.IsNullOrEmpty(_oChanges.GlobalChurchCode)) // && !string.IsNullOrEmpty(oAGO.PrefixKey))
                { 
                    //var template = new { taskSuccess = String.Empty, strRes = String.Empty };   // var definition = new { Name = "" };
                    var jsCode = GetNextGlobalChurchCodeByAcronym_jsonString( _oChanges.AppGlobalOwnerId, oAGO.PrefixKey, oCBLevel.PrefixKey);  // string json1 = @"{'Name':'James'}";
                    _oChanges.GlobalChurchCode = jsCode;
                }

                // check...
                if (string.IsNullOrEmpty(_oChanges.GlobalChurchCode))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code for " + strDesc.ToLower() + " unknown. Please refresh and try again", pageIndex = vm.PageIndex });


                //root church code  
                var genNewRootCode = string.IsNullOrEmpty(_oChanges.RootChurchCode);
                if (!genNewRootCode && oCBParent != null) genNewRootCode =  !_oChanges.RootChurchCode.Contains(oCBParent.RootChurchCode); // parent path diff from child path

                if (genNewRootCode)    // (string.IsNullOrEmpty(_oChanges.RootChurchCode) && !string.IsNullOrEmpty(_oChanges.GlobalChurchCode))
                { 
                   // var template = new { taskSuccess = String.Empty, strRes = String.Empty };
                    var jsCode = GetNextRootChurchCodeByParentCB_jsonString(_oChanges.AppGlobalOwnerId, _oChanges.ParentChurchBodyId, _oChanges.GlobalChurchCode, oAGO.PrefixKey, oCBLevel.PrefixKey);
                    _oChanges.RootChurchCode = jsCode;
                }

                // check...
                if ( string.IsNullOrEmpty(_oChanges.GlobalChurchCode) || string.IsNullOrEmpty(_oChanges.RootChurchCode))  
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code and Root church code for " + strDesc.ToLower() + " must be specified", pageIndex = vm.PageIndex }); 
                                  

                //// church logo
                //if (_oChanges.ChurchUnitLogo.ToLower() != (Guid.NewGuid().ToString() + "_" + vm.ChurchLogoFile.FileName).ToLower())
                //{
                //    string strFilename = null;
                //    if (vm.ChurchLogoFile != null && vm.ChurchLogoFile.Length > 0)
                //    {
                //        string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                //        strFilename = Guid.NewGuid().ToString() + "_" + vm.ChurchLogoFile.FileName;
                //        string filePath = Path.Combine(uploadFolder, strFilename);
                //        vm.ChurchLogoFile.CopyTo(new FileStream(filePath, FileMode.Create));
                //    }
                //    else
                //    {
                //        if (vm.oChurchBody.Id != 0) strFilename = vm.strChurchLogo;
                //    }

                //    _oChanges.ChurchUnitLogo = strFilename;
                //}

                //
                var _tm = DateTime.Now;
                _oChanges.LastMod = _tm;
                _oChanges.LastModByUserId = vm.oUserId_Logged;
                //_oChanges.Status = vm.blStatusActivated ? "A" : "D";

                var _reset = _oChanges.Id == 0;

                //validate...
                var _userTask = "Attempted saving " + strDesc.ToLower() + ", " + _oChanges.Name.ToUpper();  // _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.Name.ToUpper() + " successfully";   //  _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.Name.ToUpper() + " successfully";
                /// var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];

                var blSendCBEmail = false;

                //using (var _cbCtx = new MSTR_DbContext(_cs))
                //{ 
                    if (_oChanges.Id == 0)
                    {
                        blSendCBEmail = true;
                         initCBNetwork = true;
                        configCBDefaultUsers = true;
                         isNewCB = true;


                        var oCBVal = _context.MSTRChurchBody.AsNoTracking()  //.Include(t=>t.ParentChurchBody)
                            .Where(c => c.AppGlobalOwnerId == oCB.AppGlobalOwnerId && c.ParentChurchBodyId == (oCBParent != null ? oCBParent.Id : (int?)null) && c.Name.ToUpper() == _oChanges.Name.ToUpper()).FirstOrDefault();   /// oCB.ParentChurchBodyId
                    if (oCBVal != null) return Json(new  {  taskSuccess = false, oCurrId = _oChanges.Id,  userMess = strDesc + ", " + _oChanges.Name + " already exists [under parent body specified].", pageIndex = vm.PageIndex });

                        oCBVal = _context.MSTRChurchBody.AsNoTracking()  //.Include(t => t.ParentChurchBody)
                            .Where(c => c.AppGlobalOwnerId == oCB.AppGlobalOwnerId &&
                                    (c.GlobalChurchCode == oCB.GlobalChurchCode //||  c.RootChurchCode == oCB.RootChurchCode || 
                                                                                // (oCB.ChurchCodeCustom != null && c.ChurchCodeCustom == _oChanges.ChurchCodeCustom)
                                   )).FirstOrDefault();

                        if (oCBVal != null) return Json(new { taskSuccess = false, oCurrId = _oChanges.Id,  userMess = "Church codes must be unique." + Environment.NewLine + _oChanges.Name +  " has same church code.", pageIndex = vm.PageIndex });
                      
                        _oChanges.Created = _tm;
                        _oChanges.CreatedByUserId = vm.oUserId_Logged;
                        _context.Add(_oChanges);

                        ViewBag.UserMsg = "Saved " + strDesc.ToLower() + (!string.IsNullOrEmpty(_oChanges.Name) ? ", " + _oChanges.Name : "") + " successfully.";
                        _userTask = "Added new " + strDesc.ToLower() + (!string.IsNullOrEmpty(_oChanges.Name) ? ", " + _oChanges.Name : "") + " successfully";
                    }

                    else
                    {
                        var oCBVal = _context.MSTRChurchBody.AsNoTracking() //.Include(t => t.ParentChurchBody)
                            .Where(c => c.Id != _oChanges.Id && c.AppGlobalOwnerId == oCB.AppGlobalOwnerId && c.ParentChurchBodyId == (oCBParent != null ? oCBParent.Id : (int?)null) && c.Name.ToUpper() == _oChanges.Name.ToUpper()).FirstOrDefault();
                        if (oCBVal != null) return Json(new { taskSuccess = false,  oCurrId = _oChanges.Id, userMess = strDesc + ", " + _oChanges.Name + " already exists [under parent body specified].", pageIndex = vm.PageIndex });

                        // oCBVal = _context.MSTRChurchBody.Include(t => t.ParentChurchBody).Where(c => c.Id != oCB.Id && c.AppGlobalOwnerId == oCB.AppGlobalOwnerId && c.ChurchCode == oCB.ChurchCode ).FirstOrDefault();
                        //if (oCBVal != null) return Json(new { taskSuccess = false, oCurrId = oCB.Id, userMess = "Church code must be unique." + Environment.NewLine + 
                        //        oCBVal.Name + (oCBVal.ParentChurchBody != null ? " of " + oCBVal.ParentChurchBody.Name : "") + " has  same code."});
                    
                    
                        oCBVal = _context.MSTRChurchBody.AsNoTracking()  //.Include(t => t.ParentChurchBody)
                            .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id != _oChanges.Id &&
                                   (c.GlobalChurchCode == _oChanges.GlobalChurchCode //|| c.RootChurchCode == oCB.RootChurchCode ||
                                                                               // (oCB.ChurchCodeCustom != null && c.ChurchCodeCustom == oCB.ChurchCodeCustom)
                                   )).FirstOrDefault();

                        if (oCBVal != null) return Json(new {  taskSuccess = false,  oCurrId = _oChanges.Id,  userMess = "Church codes must be unique." + Environment.NewLine + _oChanges.Name +   " has same church code.", pageIndex = vm.PageIndex });
                    

                        //retain the pwd details... hidden fields

                        _context.Update(_oChanges);
                        //var _strDesc = strDesc.Length > 0 ? strDesc.Substring(0, 1).ToUpper() + strDesc.Substring(1) : "Denomination ";

                        ViewBag.UserMsg = strDesc + (!string.IsNullOrEmpty(_oChanges.Name) ? ", " + _oChanges.Name : "") + " updated successfully.";
                        _userTask = "Updated " + strDesc.ToLower() + (!string.IsNullOrEmpty(_oChanges.Name) ? ", " + _oChanges.Name : "") + " successfully";
                    }

                    //save  first... 
                     _context.SaveChanges();


                //    DetachAllEntities(_cbCtx);
                //}


                


                // save... CI
                // tracker .. error!
                // var updChangesMade = false;
                if (_oChangesCI.AppGlobalOwner != null) _oChangesCI.AppGlobalOwner = null; 
                if (_oChangesCI.AppGlobalOwner != null) _oChangesCI.ChurchBody = null;
                if (_oChangesCI != null)
                {
                    _oChangesCI.AppGlobalOwnerId = _oChanges.AppGlobalOwnerId;
                    _oChangesCI.ChurchBodyId = _oChanges.Id; 
                    _oChangesCI.LastMod = _tm;
                    _oChangesCI.LastModByUserId = vm.oUserId_Logged;  ///vmMod.oUserId_Logged; 

                    if (_oChangesCI.Id == 0)
                    {
                        _oChangesCI.Created = _tm;
                        _oChangesCI.CreatedByUserId = vm.oUserId_Logged;  ///vmMod.oUserId_Logged;

                        _context.MSTRContactInfo.Add(_oChangesCI);

                        _userTask += Environment.NewLine + " Updated " + _oChanges.Name.ToLower() + ", -contact info successfully";
                        
                    }
                    else
                    {
                        _context.MSTRContactInfo.Update(_oChangesCI);

                        _userTask += Environment.NewLine + " Updated " + _oChanges.Name.ToLower() + ", -contact info successfully"; 
                    }

                    //save CI to get Id...  can do other tasks...
                    await _context.SaveChangesAsync();

                    // updChangesMade = false;

                    // ci id may change...
                    if (_oChanges.ContactInfoId != _oChangesCI.Id)
                    {
                        _oChanges.ContactInfoId = _oChangesCI.Id;
                        _context.MSTRChurchBody.Update(_oChanges);
                        /// updChangesMade = true;
                        /// 
                       await _context.SaveChangesAsync();
                    }
                }


                if (blSendCBEmail)
                {
                    //email recipients... applicant, church   ... specific e-mail content
                    MailAddressCollection listToAddr = new MailAddressCollection();
                    MailAddressCollection listCcAddr = new MailAddressCollection();
                    MailAddressCollection listBccAddr = new MailAddressCollection();
                    // string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

                    // var vCode = CodeGenerator.GenerateCode();
                    // oUserResetModel.SentVerificationCode = AppUtilties.ComputeSha256Hash(vCode); // "12345678";  // TempData["oVmAuthCode"] = vCode; TempData.Keep();

                    /// var msgSubject = "RHEMA-CMS: ";   // 
                    var _appName = this._configuration["AppSettingVals:AppName"];
                    var msgSubject = !string.IsNullOrEmpty(_appName) ? _appName : "RhemaCMS";
                    var userMess = "<span> Hello " + _oChanges.Name + ",  </span><p>";
                    //if (_oChanges.Id == 0)
                    //{
                        userMess += "<span> Please a new church body (" + (!string.IsNullOrEmpty(oCBLevel.CustomName) ? oCBLevel.CustomName : oCBLevel.Name) +
                            ") of denomination, '" + oAGO.OwnerName + "' has been successfully created and added to the " + _appName + " church network " + 
                            (oCBParent != null ? "under the supervision of the " + (oCBParent.ChurchLevel != null ? (!string.IsNullOrEmpty(oCBParent.ChurchLevel.CustomName) ? oCBParent.ChurchLevel.CustomName : oCBParent.ChurchLevel.Name).ToLower() : "parent body") + ", '" + oCBParent.Name + "'." : "") +                             
                            "</span><p>";
                        ///
                        //userMess += "<h2 class='text-success'> Username: " + _oChangesUP.Username + "</h2><p>";

                        msgSubject += ": New Church Body (Congregation/Church Head Unit)";
                    //}
                    //else
                    //{
                    //    userMess += "<span> User account updated successfully </span>";
                    //    if (isNewUsername)
                    //    {
                    //        userMess += "<span>[Username changed]. Please find logon details below:</span>";
                    //        // &nbsp;userMess += "<h2 class='text-success'> Username: " + _oChangesUP.Username + "</h2><p>";
                    //        msgSubject += "User Account Update";
                    //    }
                    //    else if (_oChangesUP.ResetPwdOnNextLogOn == true)
                    //    {
                    //        userMess += "<span> . User account password reset request successfully granted. </span>";
                    //        msgSubject += "Password Reset";
                    //    }

                    //    userMess += "<span> Please find logon details below to reset: </span><p><p>";
                    //    ///
                    //    //userMess += "<h2 class='text-success'> Username: " + _oChangesUP.Username + "</h2><p>";
                    //}

                    //userMess += "<span class='text-success'> Church code: " + _oChangesUP.strChurchCode_CB + "</span><p>";
                    //userMess += "<span class='text-success'> Username: " + _oChangesUP.Username.Trim() + "</span><p>";
                    //userMess += "<h3 class='text-success'> Password: " + strUserTempPwd + "</h3><br /><hr /><br />";

                    userMess += "<span class='text-info text-lg'> Please your login details will be configured and forwarded to you shortly. </span><p><p>";

                    userMess += "<span class='text-info'> Thank you </span>";
                    userMess += "<p class='text-info'> RHEMA-CMS Team (Ghana) </p>";

                    userMess = "<div class='text-center col-md border border-info' style='padding: 50px 0'>" + userMess + "</div>";

                    listToAddr.Add(new MailAddress(_oChangesCI.Email, _oChanges.Name));
                    var res = AppUtilties.SendEmailNotification(_appName, msgSubject, userMess, listToAddr, listCcAddr, listBccAddr, null, true);
                }





                //////   vendor tasks ----------------  synchronize CB to church network.................
                ///


                /// moved to vendor tasks.....
                /// 
                initCBNetwork = true;
                if (initCBNetwork)
                {
                    var _synchRes = await SynchCBNetwork(_oChanges, vm.oUserId_Logged, _oChanges.AppGlobalOwnerId, strDesc);

                    if (_synchRes == true)  ///.IsCompletedSuccessfully == true)
                        ViewBag.UserMsg += ". Church body successfully synchronized with church network.";

                    else
                    {
                        ViewBag.UserMsg += ". Church network sync unsuccessful. Would require admin's manual configuration.";
                        _ = ViewBag.InitCBNet_uTask;
                    }
                        

                    //else   //// manual config   .... ViewBag.InitCBNet_uTask != empty
                    //{
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code and Root church code for " + strDesc.ToLower() + " must be specified", pageIndex = vm.PageIndex });
                    //}
                }


                var strMess = ViewBag.UserMsg;

                /// update, new .... config default users, roles, perms
                configCBDefaultUsers = true;
                if (configCBDefaultUsers)
                {
                    var oDefaultUPNamesList = new List<string>();
                    var oDefaultUPDescList = new List<string>();
                    var oDefaultUPRoleCodeList = new List<string>();
                    var oDefaultUPLevelList = new List<int>();

                    /// get the roles config for CB...
                    var _masCBUserRoles = _context.UserProfileRole.AsNoTracking().Include(t=> t.UserRole)
                                            .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.Id && // c.UserProfileId == 
                                                            c.ProfileRoleStatus == "A").ToList();

                    ///church head unit level
                    if (_oChanges.OrgType == "CH" || _oChanges.OrgType == "CR")
                    {
                        /// user profiles 

                        if (isNewCB || _masCBUserRoles.Count(c=> c.UserRole.RoleType == "CH_ADMN") == 0)
                        {
                            oDefaultUPNamesList.Add("chuadmin");  oDefaultUPDescList.Add("CH Admin"); oDefaultUPRoleCodeList.Add("CH_ADMN"); oDefaultUPLevelList.Add(6);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CH_MGR") == 0)
                        {
                            oDefaultUPNamesList.Add("chumgr"); oDefaultUPDescList.Add("CH Manager");  oDefaultUPRoleCodeList.Add("CH_MGR"); oDefaultUPLevelList.Add(7);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CH_EXC") == 0)
                        {
                            oDefaultUPNamesList.Add("chuexec"); oDefaultUPDescList.Add("CH Executive"); oDefaultUPRoleCodeList.Add("CH_EXC"); oDefaultUPLevelList.Add(8);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CH_RGSTR") == 0)
                        {
                            oDefaultUPNamesList.Add("chureg"); oDefaultUPDescList.Add("CH Registrar"); oDefaultUPRoleCodeList.Add("CH_RGSTR"); oDefaultUPLevelList.Add(9);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CH_ACCT") == 0)
                        {
                            oDefaultUPNamesList.Add("chuacct"); oDefaultUPDescList.Add("CH Accountant"); oDefaultUPRoleCodeList.Add("CH_ACCT"); oDefaultUPLevelList.Add(10);
                        } 
                    }

                    /// cong level
                    if (_oChanges.OrgType == "CN")
                    {
                        /// user profiles 

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CF_ADMN") == 0)
                        {
                            oDefaultUPNamesList.Add("congadmin"); oDefaultUPDescList.Add("CNG Admin"); oDefaultUPRoleCodeList.Add("CF_ADMN"); oDefaultUPLevelList.Add(11);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CF_MGR") == 0)
                        {
                            oDefaultUPNamesList.Add("congmgr"); oDefaultUPDescList.Add("CNG Manager"); oDefaultUPRoleCodeList.Add("CF_MGR"); oDefaultUPLevelList.Add(12);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CF_EXC") == 0)
                        {
                            oDefaultUPNamesList.Add("congexec"); oDefaultUPDescList.Add("CNG Executive"); oDefaultUPRoleCodeList.Add("CF_EXC"); oDefaultUPLevelList.Add(13);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CF_RGSTR") == 0)
                        {
                            oDefaultUPNamesList.Add("congreg"); oDefaultUPDescList.Add("CNG Registrar"); oDefaultUPRoleCodeList.Add("CF_RGSTR"); oDefaultUPLevelList.Add(14);
                        }

                        if (isNewCB || _masCBUserRoles.Count(c => c.UserRole.RoleType == "CF_ACCT") == 0)
                        {
                            oDefaultUPNamesList.Add("congacct"); oDefaultUPDescList.Add("CNG Accountant"); oDefaultUPRoleCodeList.Add("CF_ACCT"); oDefaultUPLevelList.Add(15);
                        }
                         
                    }

                     

                    ///create UP ... loop oDefaultUPNamesList   
                    ///
                    var _cs = AppUtilties.GetNewDBConnString_MS(_configuration); /// 
                    var tm = DateTime.Now;
                    List<UserProfileRole> oDefaultUPRList = new List<UserProfileRole>();
                    UserProfile _oChangesUP = null;
                    bool blSendUserEmail = false;
                    string strUserTempPwd = "";
                    using (var _upCtx = new MSTR_DbContext(_cs))
                    {
                        for (var i = 0; i < oDefaultUPNamesList.Count; i++)
                        {
                            var oUPName = oDefaultUPNamesList[i];
                            ///

                            if (!isNewCB)
                            {
                                _oChangesUP = _context.UserProfile.AsNoTracking()  ///.Include(t => t.UserRole)
                                            .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.Id &&
                                                            c.UserStatus == "A" && c.Username == oUPName).FirstOrDefault();
                            }
                      

                            if (isNewCB || _oChangesUP == null) 
                            {
                                
                                var oUPDesc = _oChanges.GlobalChurchCode + ": " + oDefaultUPDescList[i];
                            
                                ///
                               _oChangesUP = new UserProfile();
                              //  var _userpass = "$123456"; // + oUPName.Trim() + "1";  /// $congadmin1, $congreg1 etc
                                ///
                                blSendUserEmail = false; // bool isNewUsername = false; // var cc = "";string strOldUsername = ""; 
                                strUserTempPwd = "";                                                                                       //_oChangesUP.strChurchCode_CB = string.IsNullOrEmpty(_oChangesUP.strChurchCode_CB) ? oUPCopy.ChurchBody.GlobalChurchCode : _oChangesUP.strChurchCode_CB;

                                _oChangesUP.strChurchCode_CB = _oChanges.GlobalChurchCode;
                                var cc = _oChangesUP.strChurchCode_CB;

                                // tracker error...
                                if (_oChangesUP.AppGlobalOwner != null) _oChangesUP.AppGlobalOwner = null; if (_oChangesUP.ChurchBody != null) _oChangesUP.ChurchBody = null;
                          
                                var tempPwd = "$123456";  ///--- force user to change pwd... CodeGenerator.GenerateCode();  //   const string tempPwd = "123456";
                                // cc = string.IsNullOrEmpty(_oChangesUP.strChurchCode_CB) ? oUP.ChurchBody.GlobalChurchCode : _oChangesUP.strChurchCode_CB;
                                
                                _oChangesUP.AppGlobalOwnerId = _oChanges.AppGlobalOwnerId;
                                _oChangesUP.ChurchBodyId = _oChanges.Id; 
                                _oChangesUP.UserDesc = oUPDesc;
                                _oChangesUP.Username = oUPName;
                                _oChangesUP.UserKey = AppUtilties.ComputeSha256Hash(cc + oUPName.Trim().ToLower());
                                _oChangesUP.Pwd = tempPwd; // "123456";  //temp pwd... to reset @ next login  
                                _oChangesUP.Pwd = AppUtilties.ComputeSha256Hash(cc + oUPName.Trim().ToLower() + tempPwd);
                                _oChangesUP.Strt = tm;
                                _oChangesUP.Expr = (DateTime?)null; /// tm.AddDays(90); ///  (string.Compare(oUPName.Trim(), "sys", true) == 0 || string.Compare(_oChangesUP.Username.Trim(), "supadmin", true) == 0) ? (DateTime?)null : tm.AddDays(90);  //default to 90 days
                                _oChangesUP.ResetPwdOnNextLogOn = true; ///--- force user to change pwd... 
                                _oChangesUP.PwdExpr = tm.AddDays(30);  //default to 30 days 
                                _oChangesUP.ProfileLevel = oDefaultUPLevelList[i];   /// _oChanges.OrgType == "CN" ? i + 11 : i + 6;  // client users atart at 6  ... 11 ... 15
                                _oChangesUP.UserScope = "I";   
                                _oChangesUP.ProfileScope = "C";   
                                _oChangesUP.Email = _oChangesCI.Email;   
                                _oChangesUP.UserStatus = "A";   
                                ///
                                _oChangesUP.Created = tm;
                                _oChangesUP.LastMod = tm;
                                _oChangesUP.CreatedByUserId = vm.oUserId_Logged;
                                _oChangesUP.LastModByUserId = vm.oUserId_Logged;

                                _upCtx.Add(_oChangesUP);

                                //save user profile first... 
                                _upCtx.SaveChanges();


                                _userTask = "Added new user profile, " + (!string.IsNullOrEmpty(_oChangesUP.UserDesc) ? "[" + _oChangesUP.UserDesc + "]" : "") + " successfully";
                                ViewBag.UserMsg = "Saved user profile, " + (!string.IsNullOrEmpty(_oChangesUP.UserDesc) ? "[" + _oChangesUP.UserDesc + "]" : "") + " successfully. Password must be changed on next logon";

                                ///
                                // created new account... send details to account holder!
                                blSendUserEmail = true;
                                strUserTempPwd = tempPwd;




                                //audit...
                                ///var _tm = DateTime.Now;
                                await this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                     "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));


                                ///
                                //  DetachAllEntities(_userCtx);
                                // }



                                if (blSendUserEmail)
                                {
                                    //email recipients... applicant, church   ... specific e-mail content
                                    MailAddressCollection listToAddr = new MailAddressCollection();
                                    MailAddressCollection listCcAddr = new MailAddressCollection();
                                    MailAddressCollection listBccAddr = new MailAddressCollection();
                                    // string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

                                    // var vCode = CodeGenerator.GenerateCode();
                                    // oUserResetModel.SentVerificationCode = AppUtilties.ComputeSha256Hash(vCode); // "12345678";  // TempData["oVmAuthCode"] = vCode; TempData.Keep();

                                    var msgSubject = "RHEMA-CMS: ";   // 
                                    var userMess = "<span> Hello " + _oChangesUP.UserDesc + ",  </span><p>";
                                    //if (_oChangesUP.Id == 0)
                                    //{
                                        userMess += "<span> New user account created successfully for you. Please find logon details below:</span><p>";
                                        ///
                                        //userMess += "<h2 class='text-success'> Username: " + _oChangesUP.Username + "</h2><p>";

                                        msgSubject += "New User Account";
                                    //}
                                    //else
                                    //{
                                    //    userMess += "<span> User account updated successfully </span>";
                                    //    if (isNewUsername)
                                    //    {
                                    //        userMess += "<span>[Username changed]. Please find logon details below:</span>";
                                    //        // &nbsp;userMess += "<h2 class='text-success'> Username: " + _oChangesUP.Username + "</h2><p>";
                                    //        msgSubject += "User Account Update";
                                    //    }
                                    //    else if (_oChangesUP.ResetPwdOnNextLogOn == true)
                                    //    {
                                    //        userMess += "<span> . User account password reset request successfully granted. </span>";
                                    //        msgSubject += "Password Reset";
                                    //    }

                                    //    userMess += "<span> Please find logon details below to reset: </span><p><p>";
                                    //    ///
                                    //    //userMess += "<h2 class='text-success'> Username: " + _oChangesUP.Username + "</h2><p>";
                                    //}

                                    userMess += "<span class='text-success'> Church code: " + _oChangesUP.strChurchCode_CB + "</span><p>";
                                    userMess += "<span class='text-success'> Username: " + _oChangesUP.Username.Trim() + "</span><p>";
                                    userMess += "<h3 class='text-success'> Password: " + strUserTempPwd + "</h3><br /><hr /><br />";

                                    userMess += "<span class='text-info text-lg'> Please you will be required to RESET password at next logon. </span><p><p>";

                                    userMess += "<span class='text-info'> Thanks </span>";
                                    userMess += "<span class='text-info'> RHEMA-CMS Team (Ghana) </span>";

                                    userMess = "<div class='text-center col-md border border-info' style='padding: 50px 0'>" + userMess + "</div>";

                               
                                    listToAddr.Add(new MailAddress(_oChangesCI.Email, _oChangesUP.UserDesc));
                                    var res = AppUtilties.SendEmailNotification("RHEMA-CMS", msgSubject, userMess, listToAddr, listCcAddr, listBccAddr, null, true);
                                }
                                 
                            }

                             

                            /// add the user role... 
                            /// UPR --------------------
                            var oUPRoleCode = oDefaultUPRoleCodeList[i];
                            using (var _roleCtx = new MSTR_DbContext(_cs))
                            {
                                //if (_oChanges.ProfileLevel == 6 || _oChanges.ProfileLevel == 11)  //(vmMod.subSetIndex == 6 || vmMod.subSetIndex == 11) // SUP_ADMN role
                                //{
                                    var oUPRole = _roleCtx.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleType == oUPRoleCode &&
                                                                    (((_oChanges.OrgType == "CR" || _oChanges.OrgType == "CH") && c.RoleLevel >= 6 && c.RoleLevel <= 10) || 
                                                                      (_oChanges.OrgType == "CN" && c.RoleLevel >= 11 && c.RoleLevel <= 15)))
                                                                    .FirstOrDefault();

                                    // jux 1 to execute... no unit to access except CH /CN
                                    if (oUPRole != null)
                                    {
                                        var existUserRoles = (from upr in _roleCtx.UserProfileRole.AsNoTracking()
                                                              .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.Id &&
                                                              c.UserRoleId == oUPRole.Id && c.ProfileRoleStatus == "A") // &&                                                                                                                                                                                                                           // ((c.Strt == null || c.Expr == null) || (c.Strt != null && c.Expr != null && c.Strt <= DateTime.Now && c.Expr >= DateTime.Now && c.Strt <= c.Expr)))
                                                                                                                              // from up in roleCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                                                              select upr
                                                     ).ToList();

                                        var oUPR = new UserProfileRole();
                                        //add SUP_ADMN role to SUP_ADMN user ... assign all privileges to the SUP_ADMN role
                                        if (existUserRoles.Count() == 0)
                                        {
                                            //var oSupAdminRole = roleCtx.UserRole.Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                                            //if (oSupAdminRole != null)
                                            //{        

                                            oUPR = new UserProfileRole
                                            {
                                                AppGlobalOwnerId = _oChanges.AppGlobalOwnerId,
                                                ChurchBodyId = _oChanges.Id,
                                                UserRoleId = oUPRole.Id,
                                                UserProfileId = _oChangesUP.Id,
                                                Strt = tm,
                                                // Expr = tm,
                                                ProfileRoleStatus = "A",
                                                Created = tm,
                                                LastMod = tm,
                                                CreatedByUserId = vm.oUserId_Logged,
                                                LastModByUserId = vm.oUserId_Logged
                                            };

                                            _roleCtx.Add(oUPR);

                                            //save user role...
                                            _roleCtx.SaveChanges();

                                            DetachAllEntities(_roleCtx);

                                        oDefaultUPRList.Add(oUPR);


                                            _userTask = "Added [" + oUPRole.RoleType + "] role to user, " + _oChangesUP.Username;
                                            ViewBag.UserMsg += Environment.NewLine + " ~ [" + oUPRole.RoleType + "] role added.";


                                            _tm = DateTime.Now;
                                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                 "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));
                                            // } 
                                        } 
                                    }
                              //  }

                            }

                        }
                    }


                    strMess += " Default user profiles, roles and permissions configured successfully:- " +
                                (oDefaultUPNamesList.Count > 0 ? oDefaultUPNamesList.Count + " profile(s)." : "");

                    /// configure the user role permissions --- 
                    ///
                    if (oDefaultUPRList.Count > 0)
                    {
                        ViewBag.UserMsg_UPR = " " + (oDefaultUPRList.Count > 0 ? oDefaultUPRList.Count + " roles(s). " : "");

                        var _configres = await Configure_CL_UPR(oDefaultUPRList, _oChanges.AppGlobalOwnerId, _oChanges.Id, vm.oUserId_Logged);

                        if (_configres)
                            strMess += ViewBag.UserMsg_UPR;

                        //else   //// manual config
                        //{
                        //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code and Root church code for " + strDesc.ToLower() + " must be specified", pageIndex = vm.PageIndex });
                        //}
                    }
                     
                }


                //var strMess = "Default user profiles, roles and permissions  configured successfully. " + 
                //    (oDefaultUPNamesList.Count > 0 ? oDefaultUPNamesList.Count + " profile(s)." : "" ) +



                // var _tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));


                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurr"] = _vmMod; TempData.Keep();

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, resetNew = _reset, userMess = strMess, pageIndex = vm.PageIndex });

                 
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving church body details. Err: " + ex.Message, pageIndex = vm.PageIndex });
                // show error page
            }
        }



        private async Task<bool> SynchCBNetwork( MSTRChurchBody _oChanges, int? oUserId_Logged, int? oAGOid,  string strDesc)
        {
            try
            {
                var _tm = DateTime.Now;

                //if (initCBNetwork)  // only admin profiles allowed to update params
                //{

                var _userTask = "";
                    var oMSTRCBid = _oChanges.Id;

                    //// Get the client database details.... db connection string                        
                    //var oClientConfig = _context.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                    //if (oClientConfig == null)
                    //{
                    //    var errMess = "Client database details not found. Please try again or contact System Administrator";
                    //    // ModelState.AddModelError("", "Client database details not found. Please try again or contact System Administrator"); //model.IsVal = 0; 
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg + ". " + errMess, pageIndex = 0 });
                    //}

                    //// get and mod the conn
                    //var _clientDBConnString = "";
                    //var cs = _context.Database.GetDbConnection().ConnectionString;
                    //var conn = new SqlConnectionStringBuilder(_cs);
                    //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
                    //conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
                    ///// conn.IntegratedSecurity = false; 
                    //conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

                    //_clientDBConnString = conn.ConnectionString;

                    //// test the NEW DB conn 

                    //var _clientContext = new ChurchModelContext(_clientDBConnString);

                    ////try
                    ////{
                    ////    var b = _clientContext.Database.CanConnect();
                    ////}
                    ////catch (Exception ex)
                    ////{
                    ////    throw;
                    ////}

                    //if (!_clientContext.Database.CanConnect())
                    //{
                    //    ViewBag.InitCBNet_uTask  = "Client database details not found. Please try again or contact System Administrator";
                    //    return false; // Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg + ". " + errMess, pageIndex = 0 });
                    //}

                    try
                    {   /// synchronize CTRY, AGO, CL, CB...  from MSTR to CLIENT
                        /// 
                    // initialize the COUNTRY  ... jux the country list standard countries ::: Use [CountryCustom] to config per denomination

                    this._clientDBContext = AppUtilties.GetNewDBCtxConn_CL(_context, _configuration, oAGOid);
                    var _cs = AppUtilties.GetNewDBConnString_CL(_context, _configuration, oAGOid);  
                     
                    var _addCount = 0; var _updCount = 0;  // var tm = DateTime.Now; var strDesc = "Country"; 
                        var oCTRYCount = _clientDBContext.Country.AsNoTracking().Count();
                        var oCtryAddList = new List<Country>();
                        ///
                         var countriesList = AppUtilties.GetMS_BaseCountries();

                        if (oCTRYCount != countriesList.Count() && countriesList.Count > 0)
                        {
                            foreach (var oCtry in countriesList)
                            {
                                var oCTRYExist = _clientDBContext.Country.AsNoTracking().Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code).FirstOrDefault();
                                if (oCTRYExist == null)
                                {
                                    //var checkCtryAddedList = oCtryAddList.Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code).ToList();
                                    var checkCtryAdded = oCtryAddList.Count(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code) > 0; // checkCtryAddedList.Count() > 0; // oCtryAddList.Count(c => c.CtryAlpha3Code == oCTRY.CtryAlpha3Code) == 0;
                                    if (!checkCtryAdded)
                                    {
                                        var oNewCtry = new Country()
                                        {
                                            CtryAlpha3Code = oCtry.CtryAlpha3Code,
                                            //AppGlobalOwnerId = oAGO.Id,
                                            // ChurchBodyId = 
                                            EngName = oCtry.EngName,
                                            CtryAlpha2Code = oCtry.CtryAlpha2Code,
                                            CurrEngName = oCtry.CurrEngName,
                                            CurrLocName = oCtry.CurrLocName,
                                            CurrSymbol = oCtry.CurrSymbol,
                                            Curr3LISOSymbol = oCtry.Curr3LISOSymbol,
                                            // SharingStatus = "N",
                                            Created = _tm,
                                            LastMod = _tm,
                                            CreatedByUserId = oUserId_Logged,
                                            LastModByUserId = oUserId_Logged
                                        };

                                        _clientDBContext.Add(oNewCtry);
                                        _updCount++;

                                        oCtryAddList.Add(oNewCtry);
                                    }
                                    //else
                                    //{
                                    //    checkCtryAdded = checkCtryAdded;
                                    //    checkCtryAddedList = checkCtryAddedList;
                                    //}
                                }
                                else  // update country data
                                {
                                    oCTRYExist.CtryAlpha3Code = oCtry.CtryAlpha3Code;
                                    oCTRYExist.EngName = oCtry.EngName;
                                    oCTRYExist.CtryAlpha2Code = oCtry.CtryAlpha2Code;
                                    oCTRYExist.CurrEngName = oCtry.CurrEngName;
                                    oCTRYExist.CurrLocName = oCtry.CurrLocName;
                                    oCTRYExist.CurrSymbol = oCtry.CurrSymbol;
                                    oCTRYExist.Curr3LISOSymbol = oCtry.Curr3LISOSymbol;
                                    //oCTRY. SharingStatus = "N";
                                    // oCTRY.Created = tm;
                                    oCTRYExist.LastMod = _tm;
                                    // oCTRY.CreatedByUserId = _oChanges.Id;
                                    oCTRYExist.LastModByUserId = oUserId_Logged;

                                    _clientDBContext.Update(oCTRYExist);
                                    _updCount++;
                                }
                            }

                            if (_updCount > 0)
                            {
                                await _clientDBContext.SaveChangesAsync();  // save first 
                              ///  DetachAllEntities_CL(_clientDBContext);

                            ///
                            /////update country of oAGO
                            //if (oAGO_MSTR != null)
                            //{
                            //    oAGO.CtryAlpha3Code = oAGO_MSTR.CtryAlpha3Code;
                            //    oAGO.LastMod = tm; oAGO.LastModByUserId = _oChanges.Id;
                            //    _clientDBContext.Update(oAGO);

                            //    /// save updated...
                            //    _clientDBContext.SaveChanges();
                            //}

                            /// update user trail
                            _userTask = "Created/updated " + _updCount + " countries."; // var _tm = DateTime.Now;
                                // record ... @client 
                                //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, _oChanges.AppGlobalOwnerId, _oChanges.Id, "T",
                                //                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                                //               );

                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                               "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                               );

                            // record @ MSTR

                        }
                        }


                        // initialize the AGO    ...var oClientAGOListCount = _clientDBContext.AppGlobalOwner.Count();
                         strDesc = "Denomination (Church)"; //tm = DateTime.Now; //  _updCount = 0;
                        var oAGO_MSTR = _context.MSTRAppGlobalOwner.AsNoTracking().Include(t => t.FaithTypeCategory).ThenInclude(t => t.FaithTypeClass)
                                            .Where(c => c.Id == _oChanges.AppGlobalOwnerId).FirstOrDefault();   //&& c.GlobalChurchCode==_oChanges.strChurchCode_AGO

                        if (oAGO_MSTR == null)
                        {
                        /// ModelState.AddModelError("", "Denomination (Church) of user could not be verified [by Vendor]. Please enter correct login credentials.");   // model.IsVal = 0; // isUserValidated = false;
                        ///var errMess = "Denomination (Church) of user could not be verified [by Vendor]. Please enter correct login credentials.";
                            ViewBag.InitCBNet_uTask = "Denomination (Church) of user could not be verified [by Vendor]. Please enter correct login credentials.";

                            return false; //  Task.FromResult(false) ; /// Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg + ". " + errMess, pageIndex = 0 });
                        }

                        //COPY/create THE DENOMINATION / CONTACT INFO FROM MSTR    //  || c.GlobalChurchCode == oAGO_MSTR.GlobalChurchCode
                        var oAGO_CLNT = _clientDBContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == oAGO_MSTR.Id).FirstOrDefault();  //)   // || c.OwnerName == oAGO_MSTR.OwnerName
                        if (oAGO_CLNT == null)  // create AGO and CI
                        {
                            var oAGONew = new AppGlobalOwner()
                            {
                                //Id = 0,
                                MSTR_AppGlobalOwnerId = oAGO_MSTR.Id,
                                OwnerName = oAGO_MSTR.OwnerName,
                                GlobalChurchCode = oAGO_MSTR.GlobalChurchCode,
                                RootChurchCode = oAGO_MSTR.GlobalChurchCode,
                                TotalLevels = oAGO_MSTR.TotalLevels,
                                Acronym = oAGO_MSTR.Acronym,
                                PrefixKey = oAGO_MSTR.PrefixKey,
                                Allias = oAGO_MSTR.Allias,
                                Motto = oAGO_MSTR.Motto,
                                Slogan = oAGO_MSTR.Slogan,
                                ChurchLogo = oAGO_MSTR.ChurchLogo,
                                Status = oAGO_MSTR.Status,
                                Comments = oAGO_MSTR.Comments,
                                // CountryId = 0,
                                CtryAlpha3Code = oAGO_MSTR.CtryAlpha3Code,
                                strFaithTypeCategory = oAGO_MSTR.FaithTypeCategory != null ? oAGO_MSTR.FaithTypeCategory.FaithDescription : "",
                                strFaithStream = oAGO_MSTR.FaithTypeCategory != null ? (oAGO_MSTR.FaithTypeCategory.FaithTypeClass != null ? oAGO_MSTR.FaithTypeCategory.FaithTypeClass.FaithDescription : "") : "",
                                // FaithTypeCategoryId = oAGO_MSTR.FaithTypeCategoryId, // jux keep the Id... get the [strFaithTypeCategory, strFaithTypeStream] ...from MSTR @queries                                         
                                //  ContactInfoId = oCI != null ? oCI.Id : (int?)null, // copy details and create this to the local CI                                            
                                ///
                                Created = _tm,
                                LastMod = _tm,
                                CreatedByUserId = oUserId_Logged,
                                LastModByUserId = oUserId_Logged
                            };

                            _clientDBContext.Add(oAGONew);

                            // _updCount++;


                            //if (_updCount > 0)
                            //{

                                _clientDBContext.SaveChanges();  // save first 
                              ///  DetachAllEntities_CL(_clientDBContext);


                            oAGO_CLNT = oAGONew;


                            // check for the CI from MSTR...
                            var oCI_MSTR = _context.MSTRContactInfo.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_MSTR.Id && c.Id == oAGO_MSTR.ContactInfoId).FirstOrDefault();
                            ContactInfo oCI = null;
                            if (oCI_MSTR != null)
                            {
                                oCI = new ContactInfo()
                                {
                                    //Id = 0,
                                    AppGlobalOwnerId = oAGONew.Id,

                                    // ChurchBodyId = oCB_MSTR.Id,
                                    // RefUserId = oCI_MSTR.RefUserId,
                                    //ContactInfoDesc

                                    ExtHolderName = oCI_MSTR.ContactName,
                                    IsPrimaryContact = true,
                                    //IsChurchFellow = false,
                                    ResidenceAddress = oCI_MSTR.ResidenceAddress,
                                    Location = oCI_MSTR.Location,
                                    City = oCI_MSTR.City,
                                    CtryAlpha3Code = oCI_MSTR.CtryAlpha3Code,
                                    //RegionId = oCI_MSTR.RegionId,
                                    ResAddrSameAsPostAddr = oCI_MSTR.ResAddrSameAsPostAddr,
                                    PostalAddress = oCI_MSTR.PostalAddress,
                                    DigitalAddress = oCI_MSTR.DigitalAddress,
                                    Telephone = oCI_MSTR.Telephone,
                                    MobilePhone1 = oCI_MSTR.MobilePhone1,
                                    MobilePhone2 = oCI_MSTR.MobilePhone2,
                                    Email = oCI_MSTR.Email,
                                    Website = oCI_MSTR.Website,
                                    ///
                                    Created = _tm,
                                    LastMod = _tm,
                                    CreatedByUserId = oUserId_Logged,
                                    LastModByUserId = oUserId_Logged
                                };

                                _clientDBContext.Add(oCI);

                                //update firsst... to det Id
                                _clientDBContext.SaveChanges();  // save first 
                              ///  DetachAllEntities_CL(_clientDBContext);
                        }


                            // do some update here...
                            if (oCI != null)
                            {
                                oAGONew.ContactInfoId = oCI.Id;
                                oAGONew.LastMod = _tm; oCI.LastModByUserId = oUserId_Logged;
                                ///
                                _clientDBContext.Update(oAGONew);
                                _clientDBContext.SaveChanges();
                            }

                            // record ... @client
                            _userTask = "Created " + _updCount + " " + strDesc.ToLower(); _tm = DateTime.Now;

                            //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, _oChanges.AppGlobalOwnerId, oMSTRCBid, "T",
                            //                     "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                            //                    );

                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                            "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                            );
                        //  }
                    }


                        /// some client UPDATE /sync! ...  check the localized data... using the MSTR data
                        else  // update AGO
                        {
                            if (oAGO_CLNT.MSTR_AppGlobalOwnerId != oAGO_MSTR.Id || string.Compare(oAGO_CLNT.GlobalChurchCode, oAGO_MSTR.GlobalChurchCode, true) != 0 ||
                                string.IsNullOrEmpty(oAGO_CLNT.OwnerName) || string.IsNullOrEmpty(oAGO_CLNT.strFaithTypeCategory) || string.IsNullOrEmpty(oAGO_CLNT.strFaithStream))
                            {
                                //var oAGO_MSTR = _context.MSTRAppGlobalOwner.AsNoTracking().Include(t => t.FaithTypeCategory).ThenInclude(t => t.FaithTypeClass)
                                //            .Where(c => c.Id == _oChanges.AppGlobalOwnerId).FirstOrDefault();

                                if (oAGO_CLNT.MSTR_AppGlobalOwnerId == null)
                                    oAGO_CLNT.MSTR_AppGlobalOwnerId = oAGO_MSTR.Id;

                                if (string.IsNullOrEmpty(oAGO_CLNT.GlobalChurchCode) || oAGO_CLNT.GlobalChurchCode != oAGO_MSTR.GlobalChurchCode)
                                    oAGO_CLNT.GlobalChurchCode = oAGO_MSTR.GlobalChurchCode;

                                if (string.IsNullOrEmpty(oAGO_CLNT.OwnerName) || oAGO_CLNT.OwnerName != oAGO_MSTR.OwnerName)
                                    oAGO_CLNT.OwnerName = oAGO_MSTR.OwnerName;

                                if (string.IsNullOrEmpty(oAGO_CLNT.strFaithTypeCategory))
                                    oAGO_CLNT.strFaithTypeCategory = oAGO_MSTR.FaithTypeCategory != null ? oAGO_MSTR.FaithTypeCategory.FaithDescription : "";

                                if (string.IsNullOrEmpty(oAGO_CLNT.strFaithStream))
                                    oAGO_CLNT.strFaithStream = oAGO_MSTR.FaithTypeCategory != null ? (oAGO_MSTR.FaithTypeCategory.FaithTypeClass != null ? oAGO_MSTR.FaithTypeCategory.FaithTypeClass.FaithDescription : "") : "";

                                _clientDBContext.Update(oAGO_CLNT);


                                _clientDBContext.SaveChanges();  // save first 
                              ///  DetachAllEntities_CL(_clientDBContext);

                            // ViewBag.UserMsg = strDesc + " updated successfully.";
                            ///
                            _userTask = "Updated " + strDesc.ToLower() + ", " + oAGO_CLNT.OwnerName.ToUpper() + " successfully";

                                //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail_CL(0, _oChanges.AppGlobalOwnerId, oMSTRCBid, "T",
                                //             "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                                //            );


                              _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                                );

                            }
                        }


                        // Get the denomination/church || c.GlobalChurchCode == oAGO_MSTR.GlobalChurchCode
                        //var oAGO = _clientDBContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId).FirstOrDefault();                                 
                        // oAGO_CLNT... use last created /updated

                        // initialize the CL                                
                        var oCL_MSTRList = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId);
                        var oCL_CLNTList = _clientDBContext.ChurchLevel.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId);
                        ///
                        // if (oCL_CLNTList.Count() != oCL_MSTRList.Count())
                        // {
                        strDesc = "Church Level";
                        _addCount = 0; _updCount = 0; _tm = DateTime.Now;
                        if (oCL_MSTRList.Count() > 0 && oAGO_CLNT != null)
                        {
                            foreach (var oCL_MSTR in oCL_MSTRList)
                            {
                                var oCLExist = oCL_CLNTList.Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId &&
                                                   (c.Name.ToLower() == oCL_MSTR.Name.ToLower() || c.CustomName.ToLower() == oCL_MSTR.CustomName.ToLower())).FirstOrDefault();
                                if (oCLExist == null)
                                {
                                    _clientDBContext.Add(new ChurchLevel()
                                    {
                                        //Id = 0,
                                        MSTR_AppGlobalOwnerId = oCL_MSTR.AppGlobalOwnerId,
                                        MSTR_ChurchLevelId = oCL_MSTR.Id,
                                        ///
                                        AppGlobalOwnerId = oAGO_CLNT.Id,
                                        Name = oCL_MSTR.Name,
                                        CustomName = oCL_MSTR.CustomName,
                                        LevelIndex = oCL_MSTR.LevelIndex,
                                        Acronym = oCL_MSTR.Acronym,
                                        SharingStatus = oCL_MSTR.SharingStatus,
                                        ///
                                        Created = _tm,
                                        LastMod = _tm,
                                        CreatedByUserId = oUserId_Logged,
                                        LastModByUserId = oUserId_Logged
                                    });

                                    _addCount++;
                                }
                                else if (oCLExist.MSTR_AppGlobalOwnerId != oCL_MSTR.AppGlobalOwnerId || oCLExist.MSTR_ChurchLevelId != oCL_MSTR.Id ||
                                         oCLExist.LevelIndex != oCL_MSTR.LevelIndex || string.Compare(oCLExist.Name, oCL_MSTR.Name, true) != 0)
                                {
                                    oCLExist.MSTR_AppGlobalOwnerId = oCL_MSTR.AppGlobalOwnerId;
                                    oCLExist.MSTR_ChurchLevelId = oCL_MSTR.Id;
                                    oCLExist.Name = oCL_MSTR.Name;
                                    oCLExist.LevelIndex = oCL_MSTR.LevelIndex;
                                    oCLExist.LastMod = _tm;
                                    oCLExist.LastModByUserId = oUserId_Logged;

                                    _clientDBContext.Update(oCLExist);
                                    _updCount++;
                                }
                            }


                            if ((_addCount + _updCount) > 0)
                            {
                                _clientDBContext.SaveChanges();  // save first 
                             ///   DetachAllEntities_CL(_clientDBContext);
                                ///
                                // record ... @client
                                _userTask = "Created " + _updCount + " " + strDesc.ToLower(); _tm = DateTime.Now;
                                    //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, _oChanges.AppGlobalOwnerId, oMSTRCBid, "T",
                                    //                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                                    //                );

                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                   "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                                   );
                            }
                        }
                        // }

                        // include the root to the top for the subscriber... but make them STructure only until logged in [thus have accounts created by vendor]
                        // initialize the CB  ... ONLY create the CB that subscribed even in the same Denomination [ 1 CB at a time ]                       
                        //var oAGO = _clientDBContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId || c.GlobalChurchCode == oAGO_MSTR.GlobalChurchCode).FirstOrDefault();
                        ///

                        var oUserCB_MSTR = _context.MSTRChurchBody.AsNoTracking()  //.Include(t => t.FaithTypeCategory).ThenInclude(t => t.FaithTypeClass)
                                             .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == oMSTRCBid).FirstOrDefault();
                        if (oUserCB_MSTR == null)
                        {
                           /// var errMess = "Subscribed church body (unit) of user could not be verified [by Vendor]. Please enter correct login credentials.";
                            ViewBag.InitCBNet_uTask = "Subscribed church body (unit) of user could not be verified [by Vendor]. Please enter correct login credentials.";

                        return false;  ///  Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg + ". " + errMess, pageIndex = 0 });
                    }

                        //**********************************
                        // for single subscription... until user logs in, CB is not created yet on Client server/DB............. // use subscription key /shared subscriptio keys for Multiple subsciption sync
                        //var oCB_MSTRList = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == oUserCB_MSTR.Id).ToList(); 
                        ///
                        var oCB_MSTRList = _context.MSTRChurchBody.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && oUserCB_MSTR.RootChurchCode.Contains(c.GlobalChurchCode)).ToList();

                        //var oCBClientListCount = _clientDBContext.ChurchBody.AsNoTracking().Count(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.MSTR_ChurchBodyId == oUserCB_MSTR.Id);
                        var oCB_CLNTList = _clientDBContext.ChurchBody.AsNoTracking()
                            .Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && oUserCB_MSTR.RootChurchCode.Contains(c.GlobalChurchCode)).ToList();

                        strDesc = "Church body (unit)";

                        ///                                
                        // if (oCB_CLNTList.Count() != oCB_MSTRList.Count())
                        // {
                        _addCount = 0; _updCount = 0; _tm = DateTime.Now;
                        if (oCB_MSTRList.Count() > 0 && oAGO_CLNT != null)
                        {
                            if (!string.IsNullOrEmpty(_cs))
                            {
                                using (var _dbCtx_CB_CLNT = new ChurchModelContext(_cs))
                                {
                                    foreach (var oCB_MSTR in oCB_MSTRList)
                                    {
                                        //var oCB_CLNTExist = _clientDBContext.ChurchBody.Where(c => (c.OrgType == "CH" || c.OrgType == "CN") && c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && 
                                        //               (c.MSTR_ChurchBodyId == oCBid || c.GlobalChurchCode == oCB_MSTR.GlobalChurchCode)).FirstOrDefault();

                                        // create all CBs not found in the root path of MSTR path ... at the client side.
                                        var oCB_CLNTExist = oCB_CLNTList.Where(c => (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN") &&
                                                    (c.MSTR_ChurchBodyId == oCB_MSTR.Id || c.GlobalChurchCode == oCB_MSTR.GlobalChurchCode)).FirstOrDefault();

                                        if (oCB_CLNTExist == null)
                                        {
                                            // Get Church level
                                            //  ChurchBody oCB_CLNTAdd = null;
                                            var oCL = _clientDBContext.ChurchLevel.AsNoTracking()
                                                                        .Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.MSTR_ChurchLevelId == oCB_MSTR.ChurchLevelId).FirstOrDefault();
                                            if (oCL != null)
                                            {
                                                var oCB_CLNTAdd = new ChurchBody()
                                                {
                                                    //Id = 0,
                                                    MSTR_AppGlobalOwnerId = oCB_MSTR.AppGlobalOwnerId,
                                                    MSTR_ChurchBodyId = oCB_MSTR.Id,
                                                    MSTR_ParentChurchBodyId = oCB_MSTR.ParentChurchBodyId,
                                                    MSTR_ChurchLevelId = oCB_MSTR.ChurchLevelId,     // cannot change for CH, CN types
                                                    ///
                                                    AppGlobalOwnerId = oAGO_CLNT.Id,
                                                    ChurchLevelId = oCL.Id,
                                                    Name = oCB_MSTR.Name,
                                                    IsFullAutonomy = true,
                                                    ChurchWorkStatus = oCB_MSTR.Id == oMSTRCBid ? "OP" : "ST",
                                                    Status = oCB_MSTR.Id == oMSTRCBid ? oCB_MSTR.Status : "P",  // P-Pending activation from vendor  //oCB_MSTR.Status,
                                                    IsSupervisedByParentBody = true,
                                                    ///
                                                    //Acronym = null, 
                                                    //BriefHistory = null, 
                                                    //ChurchBodyLogo = null, 
                                                    //ChurchCodeCustom = null, 
                                                    //CountryRegionId = (int?)null,  
                                                    //SupervisedByChurchBodyId = (int?)null,   // Ex. Preaching Points are typically under the supervision of other congregations
                                                    //DateFormed = (DateTime?)null, 
                                                    //DateInnaug = (DateTime?)null,       

                                                    //ParentChurchBodyId = (int?)null,   // get the parent code... via master parent // ParentChurchBodyId = null,  // update after first batch...   ***
                                                    ///
                                                    GlobalChurchCode = oCB_MSTR.GlobalChurchCode,
                                                    MSTR_RootChurchCode = oCB_MSTR.RootChurchCode,  // ONLY Vendor to change
                                                    RootChurchCode = oCB_MSTR.RootChurchCode,       // Client Admin may change but MUST be symmetrical to the Vendors. Ex. Grace cong must continue to be in the root path of Ga Presbytery unless Vendor so determines... tho client may alter [unaffected paths]
                                                    OrgType = oCB_MSTR.OrgType,  // cannot change for CR, CH, CN types
                                                    SubscriptionKey = oCB_MSTR.SubscriptionKey,
                                                    CtryAlpha3Code = oCB_MSTR.CtryAlpha3Code,  // country GHA, USA, GBR 
                                                    ///    
                                                    //  ContactInfoId = oCI != null ? oCI.Id : (int?)null,  // create from the MSTR CI data-values ***
                                                    Comments = oCB_MSTR.Comments,

                                                    ///
                                                    Created = _tm,
                                                    LastMod = _tm,
                                                    CreatedByUserId = oUserId_Logged,
                                                    LastModByUserId = oUserId_Logged
                                                };

                                            _dbCtx_CB_CLNT.Add(oCB_CLNTAdd);

                                            _dbCtx_CB_CLNT.SaveChanges();  // save first 

                                            DetachAllEntities_CL(_dbCtx_CB_CLNT);

                                            _addCount++;


                                                ///
                                                var oCI_MSTR = _context.MSTRContactInfo.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCB_MSTR.AppGlobalOwnerId && c.ChurchBodyId == oCB_MSTR.Id && c.Id == oCB_MSTR.ContactInfoId).FirstOrDefault();
                                                //  ContactInfo oCI = null;
                                                if (oCI_MSTR != null)
                                                {
                                                    var oCI = new ContactInfo()
                                                    {
                                                        //Id = 0,
                                                        AppGlobalOwnerId = oCB_CLNTAdd.AppGlobalOwnerId,
                                                        ChurchBodyId = oCB_CLNTAdd.Id,
                                                        //RefUserId = oCI_MSTR.RefUserId,
                                                        ExtHolderName = !string.IsNullOrEmpty(oCI_MSTR.ContactName) ? oCI_MSTR.ContactName : _oChanges.Name, 
                                                        //ChurchMemberId = null
                                                        IsPrimaryContact = true,
                                                        IsChurchBody = true,
                                                        // ContactInfoDesc = null,
                                                        // IsChurchFellow = false,
                                                        ResidenceAddress = oCI_MSTR.ResidenceAddress,
                                                        Location = oCI_MSTR.Location,
                                                        City = oCI_MSTR.City, 
                                                        CtryAlpha3Code = oCI_MSTR.CtryAlpha3Code,
                                                        RegionId = oCI_MSTR.RegionId,
                                                        ResAddrSameAsPostAddr = oCI_MSTR.ResAddrSameAsPostAddr,
                                                        PostalAddress = oCI_MSTR.PostalAddress,
                                                        DigitalAddress = oCI_MSTR.DigitalAddress,
                                                        Telephone = oCI_MSTR.Telephone,
                                                        MobilePhone1 = oCI_MSTR.MobilePhone1,
                                                        MobilePhone2 = oCI_MSTR.MobilePhone2,
                                                        Email = oCI_MSTR.Email,
                                                        Website = oCI_MSTR.Website,
                                                        ///
                                                        Created = _tm,
                                                        LastMod = _tm,
                                                        CreatedByUserId = oUserId_Logged,
                                                        LastModByUserId = oUserId_Logged
                                                    };

                                                _dbCtx_CB_CLNT.Add(oCI);

                                                _dbCtx_CB_CLNT.SaveChanges();  // save first 
                                               // DetachAllEntities_CL(_clientDBContext);

                                                // update CB
                                                oCB_CLNTAdd.ContactInfoId = oCI.Id;
                                                    oCB_CLNTAdd.LastMod = _tm;
                                                    oCB_CLNTAdd.LastModByUserId = oUserId_Logged;

                                                _dbCtx_CB_CLNT.Update(oCI);

                                                _dbCtx_CB_CLNT.SaveChanges(); // Async();  // save first before updating parents...
                                                  //  DetachAllEntities_CL(_clientDBContext);
                                                }
                                            }
                                        }

                                        else if (oCB_CLNTExist.MSTR_AppGlobalOwnerId != oCB_MSTR.AppGlobalOwnerId || oCB_CLNTExist.MSTR_ChurchLevelId != oCB_MSTR.ChurchLevelId ||
                                                 oCB_CLNTExist.MSTR_ChurchBodyId != oCB_MSTR.Id || oCB_CLNTExist.MSTR_ParentChurchBodyId != oCB_MSTR.ParentChurchBodyId ||
                                                 string.IsNullOrEmpty(oCB_CLNTExist.Name) ||
                                                 oCB_CLNTExist.ChurchWorkStatus != oCB_MSTR.ChurchWorkStatus || oCB_CLNTExist.Status != oCB_MSTR.Status)
                                        {
                                            oCB_CLNTExist.MSTR_AppGlobalOwnerId = oCB_MSTR.AppGlobalOwnerId;
                                            oCB_CLNTExist.MSTR_ChurchLevelId = oCB_MSTR.ChurchLevelId;
                                            oCB_CLNTExist.MSTR_ChurchBodyId = oCB_MSTR.Id;
                                            oCB_CLNTExist.MSTR_ParentChurchBodyId = oCB_MSTR.ParentChurchBodyId;
                                            oCB_CLNTExist.Name = oCB_MSTR.Name;
                                            oCB_CLNTExist.ChurchWorkStatus = oCB_MSTR.Id == oMSTRCBid ? "OP" : "ST";
                                            oCB_CLNTExist.Status = oCB_MSTR.Id == oMSTRCBid ? oCB_MSTR.Status : "P";  // P-Pending activation from vendor  //oCB_MSTR.Status,
                                            ///
                                            oCB_CLNTExist.LastMod = _tm;
                                            oCB_CLNTExist.LastModByUserId = oUserId_Logged;

                                        _dbCtx_CB_CLNT.Update(oCB_CLNTExist);

                                        _dbCtx_CB_CLNT.SaveChanges();  // save first before updating parents...
                                        DetachAllEntities_CL(_dbCtx_CB_CLNT);

                                        _updCount++;
                                        }
                                    }
     
                                }
                            }
                                                        

                            /// NEW only else... on-demand update ... so that this code is run jux once... NOT @ every login
                            if ((_addCount + _updCount) > 0)
                            {
                                //await _clientDBContext.SaveChangesAsync();  // save first before updating parents...
                                //DetachAllEntities_CL(_clientDBContext);

                                /// set Parent ChurchBody at client level ... reload client CB list UP path only...
                                // var oCBList = _clientDBContext.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_CLNT.Id);

                                var oCBList = _clientDBContext.ChurchBody.AsNoTracking()
                                        .Where(c => c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && oUserCB_MSTR.RootChurchCode.Contains(c.GlobalChurchCode)).ToList();
                                var oCBParList = oCBList; // make a copy to search for the parent CB ... _clientDBContext.ChurchBody.Where(c => c.AppGlobalOwnerId == oAGO.Id);
                                _updCount = 0; _tm = DateTime.Now;

                                if (oCBList.Count() > 0)
                                { 
                                    if (!string.IsNullOrEmpty(_cs))
                                    {
                                        using (var _dbCtx_CBPar = new ChurchModelContext(_cs))
                                        {
                                            foreach (var oCBItem in oCBList)
                                            {
                                                if (oCBItem.ParentChurchBodyId == null ||
                                                   (oCBItem.ParentChurchBodyId != null &&
                                                   _clientDBContext.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_CLNT.Id && c.Id == oCBItem.ParentChurchBodyId).FirstOrDefault() == null))
                                                {
                                                    var oCBPar = oCBParList.Where(c => c.AppGlobalOwnerId == oCBItem.AppGlobalOwnerId && c.MSTR_AppGlobalOwnerId == oCBItem.MSTR_AppGlobalOwnerId &&
                                                                                c.MSTR_ChurchBodyId == oCBItem.MSTR_ParentChurchBodyId).FirstOrDefault();  // c.GlobalChurchCode == oCBItem.GlobalChurchCode && 
                                                    if (oCBPar != null)
                                                    {
                                                        //if (oCBItem.ParentChurchBodyId != oCBPar.Id)
                                                        //{
                                                        oCBItem.ParentChurchBodyId = oCBPar.Id;
                                                        oCBItem.LastMod = _tm;
                                                        oCBItem.LastModByUserId = oUserId_Logged;
                                                        ///

                                                        if (oCBItem.ParentChurchBody != null) oCBItem.ParentChurchBody = null;
                                                        if (oCBItem.MSTRChurchBody != null) oCBItem.MSTRChurchBody = null;
                                                        if (oCBItem.MSTRParentChurchBody != null) oCBItem.MSTRParentChurchBody = null;
                                                        if (oCBItem.SupervisedByChurchBody != null) oCBItem.SupervisedByChurchBody = null;
                                                        if (oCBItem.OwnedByChurchBody != null) oCBItem.OwnedByChurchBody = null;

                                                        _dbCtx_CBPar.Update(oCBItem);
                                                        _updCount++;
                                                        //}
                                                    }
                                                }
                                            }

                                            /// save updated...
                                            if (_updCount > 0)
                                            {
                                                _dbCtx_CBPar.SaveChanges();  // save first before updating parents...
                                                DetachAllEntities_CL(_dbCtx_CBPar);
                                            }
                                        }
                                    }                                     
                                }


                            // record ... @client
                            _userTask = "Created/updated " + _updCount + " " + strDesc.ToLower() + "s"; _tm = DateTime.Now;

                                //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, _oChanges.AppGlobalOwnerId, oMSTRCBid, "T",
                                //                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                                //                 );

                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                               "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                               );
                            }
                        }
                        // }

                        var oCBClient = _clientDBContext.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel).Include(t => t.ParentChurchBody)
                                        .Where(c => c.MSTR_AppGlobalOwnerId == oUserCB_MSTR.AppGlobalOwnerId && (c.MSTR_ChurchBodyId == oUserCB_MSTR.Id || c.GlobalChurchCode == oUserCB_MSTR.GlobalChurchCode || (c.MSTR_ParentChurchBodyId == oUserCB_MSTR.ParentChurchBodyId && c.Name == oUserCB_MSTR.Name)))
                                        .FirstOrDefault();

                        // var _oCBClient = oCBClient;
                        //var oCBClient = _clientDBContext.ChurchBody.AsNoTracking().Where(c => (c.OrgType == "CH" || c.OrgType == "CN") && c.MSTR_AppGlobalOwnerId == _oChanges.AppGlobalOwnerId &&
                        //                                (c.MSTR_ChurchBodyId == oCBid || c.GlobalChurchCode == oCB_MSTR.GlobalChurchCode)).FirstOrDefault();


                        //// not new... save user info into session
                        //if (oCBClient != null)
                        //{
                        //    // save the client db
                        //    oUserPrivilegeCol.AppGlobalOwner_CLNT = oCBClient.AppGlobalOwner;
                        //    oUserPrivilegeCol.ChurchBody_CLNT = oCBClient;
                        //    _privList_CLNT = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                        //    TempData["UserLogIn_oUserPrivCol"] = _privList_CLNT; TempData.Keep();

                        //    //// get the church slogan... from CLIENT side
                        //    _strSlogan_CL = "";
                        //    // get the church slogan... from MSTR side
                        //    // Asomdwei nka wo|enka wo nso 
                        //    if (!string.IsNullOrEmpty(oCBClient.AppGlobalOwner?.Slogan))
                        //    {
                        //        _strSlogan_CL = oCBClient.AppGlobalOwner?.Slogan;
                        //        if (_strSlogan_CL.Contains("*|*"))
                        //        {
                        //            var _arrSlogan_CL = _strSlogan_CL.Split("*|*");
                        //            _strSlogan_CL = _arrSlogan_CL.Length > 0 ? _arrSlogan_CL[0] : _strSlogan_CL;
                        //        }
                        //    }

                        //    // var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan);
                        //    TempData["_strChurchSlogan"] = _strSlogan_CL; TempData.Keep();
                        //    model.strChurchSlogan = _strSlogan_CL;
                        //}


                        // nullify obj b/f save
                        if (_oChanges.AppGlobalOwner != null) _oChanges.AppGlobalOwner = null; // if (_oChanges.ChurchBody != null) _oChanges.ChurchBody = null;
                        if (oCBClient != null)
                        {
                            if (oCBClient.MSTR_AppGlobalOwnerId == null || oCBClient.MSTR_ChurchBodyId == null || oCBClient.MSTR_ChurchLevelId == null || string.IsNullOrEmpty(oCBClient.GlobalChurchCode) || string.IsNullOrEmpty(oCBClient.Name))
                            {
                                if (oCBClient.MSTR_AppGlobalOwnerId == null)
                                    oCBClient.MSTR_AppGlobalOwnerId = oUserCB_MSTR.AppGlobalOwnerId;

                                if (oCBClient.MSTR_ChurchBodyId == null)
                                    oCBClient.MSTR_ChurchBodyId = oUserCB_MSTR.Id;

                                if (oCBClient.MSTR_ChurchLevelId == null)
                                    oCBClient.MSTR_ChurchLevelId = oUserCB_MSTR.ChurchLevelId;

                                if (string.IsNullOrEmpty(oCBClient.GlobalChurchCode) || oAGO_CLNT.GlobalChurchCode != oUserCB_MSTR.GlobalChurchCode)
                                    oCBClient.GlobalChurchCode = oUserCB_MSTR.GlobalChurchCode;

                                if (string.IsNullOrEmpty(oCBClient.Name) || oCBClient.Name != oUserCB_MSTR.Name)
                                    oCBClient.Name = oUserCB_MSTR.Name;

                                // nullify obj b/f save
                                oCBClient.ChurchLevel = null;

                                ///
                                // nullify obj b/f save
                                if (oCBClient.AppGlobalOwner != null) oCBClient.AppGlobalOwner = null; if (oCBClient.ChurchLevel != null) oCBClient.ParentChurchBody = null;
                                ///
                                _clientDBContext.Update(oCBClient);

                            _clientDBContext.SaveChanges();  // save first before updating parents...
                            DetachAllEntities_CL(_clientDBContext);

                            // ViewBag.UserMsg = strDesc + " updated successfully.";
                            ViewBag.InitCBNet_uTask = "Updated " + strDesc.ToLower() + ", " + oCBClient.Name.ToUpper() + " successfully";
                            }
                        }


                        //// update master db-user
                        //_oChanges.IsCLNTInit = true;
                        //_oChanges.LastMod = tm;
                        //// _oChanges.LastModByUserId = oUserId_Logged;


                        //// nullify obj b/f save
                        //if (_oChanges.AppGlobalOwner != null) _oChanges.AppGlobalOwner = null; if (_oChanges.ChurchBody != null) _oChanges.ChurchBody = null;
                        //_context.Update(oUser_MSTR);

                        /////
                        //_context.SaveChanges();







                        //// not new... save user info into session
                        //if (_oCBClient != null)
                        //{    
                        //    // save the client db
                        //    oUserPrivilegeCol.AppGlobalOwner_CLNT = _oCBClient.AppGlobalOwner;
                        //    oUserPrivilegeCol.ChurchBody_CLNT = _oCBClient;
                        //    var _privList_CLNT = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                        //    TempData["UserLogIn_oUserPrivCol"] = _privList_CLNT; TempData.Keep();

                        //    //// get the church slogan... from CLIENT side
                        //    var _strSlogan_CL = "";
                        //    // get the church slogan... from MSTR side
                        //    // Asomdwei nka wo|enka wo nso 
                        //    if (!string.IsNullOrEmpty(_oCBClient.AppGlobalOwner?.Slogan))
                        //    {
                        //        _strSlogan_CL = _oCBClient.AppGlobalOwner?.Slogan;   
                        //        if (_strSlogan_CL.Contains("*|*"))
                        //        {
                        //            var _arrSlogan_CL = _strSlogan_CL.Split("*|*");
                        //            _strSlogan_CL = _arrSlogan_CL.Length > 0 ? _arrSlogan_CL[0] : _strSlogan_CL;
                        //        }
                        //    } 

                        //    // var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan);
                        //    TempData["_strChurchSlogan"] = _strSlogan_CL; TempData.Keep();
                        //    model.strChurchSlogan = _strSlogan_CL;
                        //}

                        
                        /// ViewBag.InitCBNet_uTask = "Church network successfully synchronized.";
                        ///// 
                        //ViewBag.InitCBNet_uTask = "";

                        //return true; // Task.FromResult(true); 
                    }

                    catch (Exception ex)
                    {
                    /// var errMess = "church body synchronization failed. Please reload page to continue or contact System Admin. cli Err: " + ex.ToString();
                    // ModelState.AddModelError("", "Account validation failed. Client working space initialization failed. Please reload page to continue or contact System Admin. cli Err: " + ex.ToString()); // : ");

                        ViewBag.InitCBNet_uTask = "Church network sync and initialization failed. Please reload page to continue or contact System Admin. cli Err: " + ex.Message;
                        return false; // Task.FromResult(false);  ///  Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg + " but " + errMess, pageIndex = 0 });
                    }



                ViewBag.InitCBNet_uTask = "";

                return true; // Task.FromResult(true); 
                             // }


                // return true;
            }
            catch (Exception ex)
            {
                ViewBag.InitCBNet_uTask = "Church network sync and initialization failed. Please reload page to continue or contact System Admin. cli Err: " + ex.Message;
                return false;
            }
        }



        public IActionResult Delete_CB(int? loggedUserId, int id, bool forceDeleteConfirm = false)  // attach oAppGloOwnId ... (int? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
        {
            var strDesc = "Church unit"; var _tm = DateTime.Now; var _userTask = "Attempted saving church unit";
            //
            try
            {
                var tm = DateTime.Now;
                //
                var oCB = _context.MSTRChurchBody.AsNoTracking().Where(c => c.Id == id).FirstOrDefault(); // .Include(c => c.ChurchUnits)
                strDesc = (oCB.OrgType == "CR" || oCB.OrgType == "CH") ? "Congregation Head-unit" : oCB.OrgType == "CN" ? "Congregation" : "Church Unit";
                if (oCB == null)
                {
                    _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oCB.Name;  // var _userTask = "Attempted saving church unit";
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " to delete could not be retrieved." });
                }

                var saveDelete = true;
                // ensuring cascade delete where there's none!

                //check for the UP and CSC, parent CB ... and many more! almost every table is connectedto CB
                var oUserProfiles = _context.UserProfile.AsNoTracking().Where(c => c.ChurchBodyId == oCB.Id).ToList();
                // var oClientServerConfigs = _context.ClientAppServerConfig.Where(c => c.ChurchBodyId == oCB.Id).ToList();
                var oParentCBs = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCB.AppGlobalOwnerId && c.ParentChurchBodyId == oCB.Id).ToList();


                //var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];
                //if (string.IsNullOrEmpty(_cs))
                //    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "User profile could not be deleted successfully. Failed connecting to central pool DB. Please contact System Admin for assistance" });


                //using (var _cbCtx = new MSTR_DbContext(_cs))
                //    { 

                        if ((oUserProfiles.Count() + oParentCBs.Count()) > 0)
                        {
                            var strConnTabs = oUserProfiles.Count() > 0 ? "User profile" : "";
                            strConnTabs = strConnTabs.Length > 0 ? strConnTabs + ", " : strConnTabs;
                           // strConnTabs = oClientServerConfigs.Count() > 0 ? strConnTabs + "Client server configuration" : strConnTabs;
                            strConnTabs = oParentCBs.Count() > 0 ? strConnTabs + "Church unit (parented other church units) " : strConnTabs;

                            if (forceDeleteConfirm == false)
                            {
                                saveDelete = false;
                                // check user privileges to determine... administrator rights
                                // log
                                _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oCB.Name;  // var _userTask = "Attempted saving church unit";
                                _tm = DateTime.Now;
                                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                                return Json(new
                                {
                                    taskSuccess = false,
                                    tryForceDelete = false,
                                    oCurrId = id,
                                    userMess = "Specified " + strDesc.ToLower() + " to delete has been used elsewhere in the system [" + strConnTabs + "]. Delete cannot be done unless dependent-references are removed."
                                });

                                //super_admin task
                                //return Json(new { taskSuccess = false, tryForceDelete = true, oCurrId = id, userMess = "Specified " + strDesc.ToLower() + 
                                //       " has dependencies or links with other external data [Faith category]. Delete cannot be done unless child refeneces are removed. DELETE (cascade) anyway?" });
                            } 

                            ///Else....
                        }

                        //successful...
                        if (saveDelete)
                        {
                            _context.MSTRChurchBody.Remove(oCB);
                            _context.SaveChanges();

                         
                           /// DetachAllEntities(_cbCtx);

                            _userTask = "Deleted " + strDesc.ToLower() + ", " + oCB.Name + " successfully";  // var _userTask = "Attempted saving " + strDesc;
                            _tm = DateTime.Now;
                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                            return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oCB.Id, userMess = strDesc + " successfully deleted." });
                        }

                        else
                        {   
                           /// DetachAllEntities(_cbCtx); 
                            return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" }); 
                        }
                     
                 ///   }
                                
            }

            catch (Exception ex)
            {
                _userTask = "Attempted deleting " + strDesc.ToLower() + ", [ID=" + id + "] but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

               
                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
            }
        }


        // CSC
        public ActionResult Index_CASC(int? oAppGloOwnId = null, int pageIndex = 1)  // , int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            // SetUserLogged();
            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                var strDesc = "Client server configuration";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";

                // check permission 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                //
                if (!this.userAuthorized) return View(new ClientAppServerConfigModel()); //retain view    
                if (oUserLogIn_Priv == null) return View(new ClientAppServerConfigModel());
                if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) return View(new ClientAppServerConfigModel());
                var oLoggedUser = oUserLogIn_Priv.UserProfile;
               // var oLoggedRole = oUserLogIn_Priv.UserRole;

                //
                var oCASCModel = new ClientAppServerConfigModel(); //TempData.Keep();  
                                                           // int? oAppGloOwnId = null;
                var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
                //
                int? oAppGloOwnId_Logged = null;
                int? oChuBodyId_Logged = null;
                if (oChuBody_Logged != null)
                {
                    oAppGloOwnId_Logged = oChuBody_Logged.ChurchLevelId;
                    oChuBodyId_Logged = oChuBody_Logged.Id;
                }

                var oUserId_Logged = oLoggedUser.Id;

                //
                var lsCLModel = (
                        from t_casc in _context.ClientAppServerConfig.AsNoTracking() //.Where(c => c.AppGlobalOwnerId == oAppGloOwnId) //.Include(t => t.AppGlobalOwner)
                        from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_casc.AppGlobalOwnerId)  

                        select new ClientAppServerConfigModel()
                        {
                            oCASConfig = t_casc,
                            strAppGloOwn = t_ago != null ? t_ago.OwnerName : "", 
                            strConfigDate = t_casc.ConfigDate != null ? DateTime.Parse(t_casc.ConfigDate.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "",
                        }
                       ).OrderByDescending(c => c.oCASConfig.ConfigDate).ThenBy(c => c.strAppGloOwn) 
                       .ToList();

                oCASCModel.lsCASConfigModels = lsCLModel;
                oCASCModel.strCurrTask = strDesc;

                //                
                oCASCModel.oAppGloOwnId = oAppGloOwnId;
                //oCASCModel.oChurchBodyId = oCurrChuBodyId;
                oCASCModel.PageIndex = pageIndex;
                //
                oCASCModel.oUserId_Logged = oUserId_Logged;
                oCASCModel.oChurchBody_Logged = oChuBody_Logged;
                oCASCModel.oAppGloOwnId_Logged = oAppGloOwnId_Logged;



                if (oCASCModel.PageIndex == 2) // Client server configuration classes av church sects
                    oCASCModel = this.popLookups_CASC(oCASCModel, oCASCModel.oCASConfig);



              //  //  dashboard stuff
              //  ///
              //  ViewData["strAppName"] = "RhemaCMS";
              //  ViewData["strAppNameMod"] = "Admin Palette";
              //  ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(oLoggedUser.UserDesc) ? oLoggedUser.UserDesc : "[Current user]";
              //  ///
              //  ViewData["oAppGloOwnId_Logged"] = oAppGloOwnId_Logged;
              //  ViewData["oChuBodyId_Logged"] = oChuBodyId_Logged;

              //  ViewData["oCBOrgType_Logged"] = oChuBody_Logged.OrgType;  // CH, CN but subscriber may come from oth

              //  ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
              //  ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
              //  ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
              //  ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
              ////  ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc);
              //  ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

              //  //ViewData["strAppCurrUser_ChRole"] = oLoggedRole.RoleName; // "System Adminitrator";
              //  //ViewData["strAppCurrUser_RoleCateg"] = oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
                               
              //  ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
              //  ///


              //  //refresh Dash values
              //  _ = LoadDashboardValues();

                var tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oLoggedUser.Id, tm, tm, oLoggedUser.Id, oLoggedUser.Id));

                return View("Index_CASC", oCASCModel);
            }
        }


        [HttpGet]
        public IActionResult AddOrEdit_CASC(int id = 0, int? oAppGloOwnId = null, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) // (int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0, int? oCASCId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                {
                    var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                    var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                    // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                    //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                    // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                    oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                    oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                    oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                    var strDesc = "Client server configuration";
                    var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); // _userTask = "Attempted creating new denomination (church)"; // _userTask = "Opened denomination (church)-" + oCFT_MDL.oChurchFaithType.FaithDescription;
                    var oCASCModel = new ClientAppServerConfigModel();
                    if (id == 0)
                    { 
                        //var oAppOwn = _context.MSTRAppGlobalOwner.Find(oAppGloOwnId);
                        //if (oAppOwn == null)
                        //{
                        //    
                        //    return PartialView("_ErrorPage");
                        //}

                        oCASCModel.oCASConfig = new ClientAppServerConfig();
                          
                       // oCASCModel.oCASConfig.AppGlobalOwnerId = (int)oAppGloOwnId;

                        oCASCModel.oCASConfig.ConfigDate = DateTime.Now;
                        oCASCModel.oCASConfig.DbaseName = "DBRCMS_CL_"; // + oAppOwn.Acronym.ToUpper(); //check uniqueness
                                               
                        oCASCModel.oCASConfig.Status = "A";

                        oCASCModel.oCASConfig.Created = DateTime.Now;
                        oCASCModel.oCASConfig.LastMod = DateTime.Now;
                        //
                        //oCASCModel.oCASConfig.AppGlobalOwner = oAppOwn;
                        //oCASCModel.strAppGloOwn = oAppOwn.OwnerName;

                        _userTask = "Attempted creating new " + strDesc.ToLower(); //, " + oAppOwn.OwnerName;
                    }

                    else
                    {
                        oCASCModel = (
                             from t_casc in _context.ClientAppServerConfig.AsNoTracking().Include(t=>t.AppGlobalOwner).Where(x => x.Id == id)
                             from t_ago in _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_casc.AppGlobalOwnerId)

                             select new ClientAppServerConfigModel()
                             {
                                 oCASConfig = t_casc,
                                 strAppGloOwn = t_ago != null ? t_ago.OwnerName : "",
                                 strConfigDate = t_casc.ConfigDate != null ? DateTime.Parse(t_casc.ConfigDate.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "",
                             }
                            )
                             .FirstOrDefault();

                        if (oCASCModel == null)
                        {
                            
                            return PartialView("_ErrorPage");
                        }

                        _userTask = "Opened " + strDesc.ToLower() + " -- [#Id: " + oCASCModel.oCASConfig.Id + "] for denomination, " + oCASCModel.strAppGloOwn;
                    }


                    // oCASCModel.setIndex = setIndex;
                    // oCASCModel.subSetIndex = subSetIndex;

                    oCASCModel.PageIndex = 2;

                    oCASCModel.oUserId_Logged = oUserId_Logged;
                    oCASCModel.oAppGloOwnId_Logged = oAGOId_Logged;
                    oCASCModel.oChurchBodyId_Logged = oCBId_Logged;
                    //
                    // oCASCModel.oAppGloOwnId = oAppGloOwnId;
                    // oCASCModel.oChurchBodyId = oCurrChuBodyId;
                    //  var oCurrChuBody = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                    // oCASCModel.oChurchBody = oCurrChuBody != null ? oCurrChuBody : null;

                  //  if (oCASCModel.subSetIndex == 2) // Client server configuration classes av church sects
                   
                    oCASCModel = this.popLookups_CASC(oCASCModel, oCASCModel.oCASConfig);

                    var tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oUserId_Logged, tm, tm, oUserId_Logged, oUserId_Logged));

                    var _oCASCModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCASCModel);
                    TempData["oVmCurrMod"] = _oCASCModel; TempData.Keep();

                    return PartialView("_AddOrEdit_CASC", oCASCModel);
                }

                //page not found error
                
                return PartialView("_ErrorPage");
            }
        }

        public ClientAppServerConfigModel popLookups_CASC(ClientAppServerConfigModel vm, ClientAppServerConfig oCurrCL)
        {
            if (vm == null ) return vm;   // || oCurrCL == null
            //
            vm.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vm.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vm.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Status == "A")
                                              .OrderBy(c => c.OwnerName).ToList()
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = c.OwnerName
                                              })
                                              .ToList();

            vm.lkpAppGlobalOwns.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            return vm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_CASC(ClientAppServerConfigModel vm)
        {
            var strDesc = "Client server configuration";
            if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.PageIndex });
            if (vm.oCASConfig == null) 
                return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.PageIndex });

            ClientAppServerConfig _oChanges = vm.oCASConfig;  // vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vmMod; TempData.Keep();
            ///var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  ///  this._configuration["ConnectionStrings:DefaultConnection"];

            if (_oChanges.AppGlobalOwnerId == null)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify denomination (church) to configure", pageIndex = vm.PageIndex });

            if (string.IsNullOrEmpty(_oChanges.ServerName))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the database server", pageIndex = vm.PageIndex });

            if (string.IsNullOrEmpty(_oChanges.DbaseName))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the database name", pageIndex = vm.PageIndex });

           // oCASCModel.oCASConfig.DbaseName = "DBRCMS_CL_" + oAppOwn.Acronym.ToUpper(); //check uniqueness
            if (_oChanges.DbaseName.StartsWith("DBRCMS_CL_") == false)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Database name must begin with 'DBRCMS_CL_'", pageIndex = vm.PageIndex });

            // database must not have been assigned...
            //var oCASCExist = _context.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.DbaseName.ToLower() == _oChanges.DbaseName.ToLower()).FirstOrDefault();
            //if (oCASCExist != null)
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Database name specified is not available. It's used by another denomination/church.", pageIndex = vm.PageIndex });

            if (string.IsNullOrEmpty(_oChanges.SvrUserId))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the server user", pageIndex = vm.PageIndex });

            if (string.IsNullOrEmpty(_oChanges.SvrPassword))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the server password", pageIndex = vm.PageIndex });

            //// check connection successful...  ??? 
            //// get and mod the conn
            //var _clientDBConnString = "";
            //var cs = _context.Database.GetDbConnection().ConnectionString;
            //var conn = new SqlConnectionStringBuilder(_cs);
            //conn.DataSource = _oChanges.ServerName; conn.InitialCatalog = _oChanges.DbaseName; conn.UserID = _oChanges.SvrUserId; conn.Password = _oChanges.SvrPassword; 
            //conn.IntegratedSecurity = false; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

            //_clientDBConnString = conn.ConnectionString;
            
            
           /// _clientDBConnString = AppUtilties.GetNewDBCtxConn_CL(_context, _configuration, _oChanges.AppGlobalOwnerId);
            this._clientDBContext = AppUtilties.GetNewDBCtxConn_CL(_context, _configuration, _oChanges.AppGlobalOwnerId);

            // test the NEW DB conn
            ///var _clientContext = new ChurchModelContext(_clientDBConnString);
           
            
            if (!this._clientDBContext.Database.CanConnect()) 
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to connect client database. Please try again or contact System Admin", pageIndex = vm.PageIndex }); 
             

            // validations done!
            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ClientAppServerConfigModel>(arrData) : vm;

            var oCASC = vmMod.oCASConfig;
            // oCASC.ChurchBody = vmMod.oChurchBody; 

            try
            {
                ModelState.Remove("oCASConfig.AppGlobalOwnerId"); 
                //
                ModelState.Remove("oCASConfig.CreatedByUserId");
                ModelState.Remove("oCASConfig.LastModByUserId");


                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", pageIndex = vm.PageIndex });


                //
                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.LastModByUserId = vm.oUserId_Logged;

                var _reset = _oChanges.Id == 0;
                var oAGO = _context.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
                var oCASCDesc = strDesc.ToLower(); // + " -- [#Id: " + _oChanges.Id + "] for denomination" + (oAGO != null ? ", " + oAGO.OwnerName : "");

                //validate...
                var _userTask = "Attempted saving " + oCASCDesc;  // _userTask = "Added new " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";   //  _userTask = "Updated " + strDesc.ToLower() + ", " + _oChanges.OwnerName.ToUpper() + " successfully";

                //using (var _cascCtx = new MSTR_DbContext(_cs))
                //{ 

                    if (_oChanges.Id == 0)
                    {
                        var existCASC = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                        if (existCASC != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Client server configuration already exist" + (existCASC.AppGlobalOwner != null ? ". Denomination (Church): " + existCASC.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });


                        var oCASC_Db = _context.ClientAppServerConfig.AsNoTracking().Include(t=>t.AppGlobalOwner).Where(c => c.DbaseName.ToLower().Equals(_oChanges.DbaseName.ToLower())).FirstOrDefault();
                        if (oCASC_Db != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess =  "Database name already used for another denomination" + (oCASC_Db.AppGlobalOwner != null ? ", " + oCASC_Db.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });

                        _oChanges.Created = tm;
                        _oChanges.CreatedByUserId = vm.oUserId_Logged;

                        _context.Add(_oChanges);

                   
                        ViewBag.UserMsg = "Saved " + oCASCDesc + " successfully.";
                        _userTask = "Added new " + oCASCDesc + " successfully";
                    }

                    else
                    {
                        var existCASC = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id != _oChanges.Id && c.Status == "A").FirstOrDefault(); 
                        if (existCASC != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Client server configuration already exist" + (existCASC.AppGlobalOwner != null ? ". Denomination (Church): " + existCASC.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });

                        var oCASC_Db = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id != _oChanges.Id && c.DbaseName.ToLower().Equals(_oChanges.DbaseName.ToLower())).FirstOrDefault();
                        if (oCASC_Db != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Database name already used for another denomination" + (oCASC_Db.AppGlobalOwner != null ? ", " + oCASC_Db.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });
                     
                        //retain the pwd details... hidden fields
                        _context.Update(_oChanges);
                        //var _strDesc = strDesc.Length > 0 ? strDesc.Substring(0, 1).ToUpper() + strDesc.Substring(1) : "Client server configuration ";

                        oCASCDesc += " -- [#Id: " + _oChanges.Id + "] for denomination" + (oAGO != null ? ", " + oAGO.OwnerName : "");
                        ViewBag.UserMsg = oCASCDesc + " updated successfully.";
                        _userTask = "Updated " + oCASCDesc + " successfully";
                    }

                    //save denomination first... 
                    await _context.SaveChangesAsync();

                //    DetachAllEntities(_cascCtx);
              //  }


                var _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));


                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurr"] = _vmMod; TempData.Keep();

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, resetNew = _reset, userMess = ViewBag.UserMsg, pageIndex = vm.PageIndex });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving denomination (church) details. Err: " + ex.Message, pageIndex = vm.PageIndex });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEditBLK_CASC(ClientAppServerConfigModel vm, IFormCollection f) //ChurchMemAttendList oList)      
                                                                                                     // public IActionResult Index_Attendees(ChurchMemAttendList oList) //List<ChurchMember> oList)  //, int? reqChurchBodyId = null, string strAttendee="M", string strLongevity="C" ) //, char? filterIndex = null, int? filterVal = null)
        {
            var strDesc = "Client server configuration";


            if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = 2 });
            if (vm.lsCASConfigModels == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No changes made to " + strDesc + " data.", pageIndex = vm.PageIndex });
            if (vm.lsCASConfigModels.Count == 0) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No changes made to " + strDesc + " data.", pageIndex = vm.PageIndex });
           
            //    if (vm.oClientAppServerConfig == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again." });

            //  ClientAppServerConfig _oChanges = vm.oClientAppServerConfig;  // vm = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vm; TempData.Keep();

            //check if the configured levels <= total levels under AppGloOwn 
            //var oVal = f["oAppGloOwnId"].ToString();
            //var oAGOId = !string.IsNullOrEmpty(oVal) ? int.Parse(oVal) : (int?)null;
            //var countCL = _context.ClientAppServerConfig.Count(c => c.AppGlobalOwnerId == oAGOId);
            //var oAGO = _context.MSTRAppGlobalOwner.Find(oAGOId);
            //if (oAGO == null)
            //    return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Specify denomination (church)" });

            //if (countCL > oAGO.TotalLevels)
            //    return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Total " + strDesc.ToLower() + "s allowed for denomination, " + oAGO.OwnerName + " [" + oAGO.TotalLevels + "] exceeded.", pageIndex = vm.PageIndex });


            // return View(vm);
            if (ModelState.IsValid == false)
                return Json(new { taskSuccess = false, oCurrId = "", userMess = "Saving data failed. Please refresh and try again", pageIndex = vm.PageIndex  });

            //if (vm == null)
            //    return Json(new { taskSuccess = false, userMess = "Data to update not found. Please refresh and try again", pageIndex = vm.PageIndex });

            //if (vm.lsCASConfigModels == null)
            //    return Json(new { taskSuccess = false, userMess = "No changes made to attendance data.", pageIndex = vm.PageIndex });

            //if (vm.lsCASConfigModels.Count == 0)
            //    return Json(new { taskSuccess = false, userMess = "No changes made to attendance data", pageIndex = vm.PageIndex });



            //if ((_oChanges.Id == 0 && countCL >= oAGO.TotalLevels) || (_oChanges.Id > 0 && countCL > oAGO.TotalLevels))
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Total " + strDesc.ToLower() + "s allowed for denomination, " + oAGO.OwnerName + " [" + oAGO.TotalLevels + "] reached." });

            //if (_oChanges.LevelIndex <= 0 || _oChanges.LevelIndex > oAGO.TotalLevels)
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please indicate correct level index. Hint: Must be within total Client server configurations [" + oAGO.TotalLevels + "]" });

            //if (string.IsNullOrEmpty(_oChanges.Name))
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the " + strDesc.ToLower() + " name" });



            //get the global var
            // var oCbId = f["_hdnAppGloOwnId"].ToString();
            //var oCbId = f["oChurchBodyId"].ToString();
            //var oDt = f["m_DateAttended"].ToString();
            //var oEv = f["m_ChurchEventId"].ToString();

            // if (oCbId == null)
            //   return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Denomination (church) is required. Please specify denomination.", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });

            //var oCBId = int.Parse(oCbId);
            //var dtEv = DateTime.Parse(oDt);
            //var oEvId = int.Parse(oEv);

           /// var  _cs = this._configuration["ConnectionStrings:DefaultConnection"];


            foreach (var d in vm.lsCASConfigModels)
            {
                if (d.oCASConfig != null)
                {
                    var _oChanges = d.oCASConfig;
                    if (_oChanges.AppGlobalOwnerId == null)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify denomination (church) to configure" , pageIndex = vm.PageIndex });

                    var oAGO = _context.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
                    var strAGO = oAGO != null ? oAGO.OwnerName : "";

                    if (string.IsNullOrEmpty(_oChanges.ServerName))
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the database server" + (!string.IsNullOrEmpty(strAGO) ? ". Denomination (Church): " + strAGO : "") , pageIndex = vm.PageIndex });

                    if (string.IsNullOrEmpty(_oChanges.DbaseName))
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the database name" + (!string.IsNullOrEmpty(strAGO) ? ". Denomination (Church): " + strAGO : ""), pageIndex = vm.PageIndex });

                    // oCASCModel.oCASConfig.DbaseName = "DBRCMS_CL_" + oAppOwn.Acronym.ToUpper(); //check uniqueness
                    if (_oChanges.DbaseName.StartsWith("DBRCMS_CL_") == false)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Database name must begin with 'DBRCMS_CL_'" + (!string.IsNullOrEmpty(strAGO) ? ". Denomination (Church): " + strAGO : ""), pageIndex = vm.PageIndex });

                    if (string.IsNullOrEmpty(_oChanges.SvrUserId))
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the server user" + (!string.IsNullOrEmpty(strAGO) ? ". Denomination (Church): " + strAGO : ""), pageIndex = vm.PageIndex });

                    if (string.IsNullOrEmpty(_oChanges.SvrPassword))
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide the server password" + (!string.IsNullOrEmpty(strAGO) ? ". Denomination (Church): " + strAGO : ""), pageIndex = vm.PageIndex });


                    if (d.oCASConfig.Id > 0)  // update
                    {
                        var existCASC = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id != _oChanges.Id && c.Status == "A").FirstOrDefault();
                        if (existCASC != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Client server configuration already exist" + (existCASC.AppGlobalOwner != null ? ". Denomination (Church): " + existCASC.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });


                        var oCASC_Db = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id != _oChanges.Id && c.DbaseName.ToLower().Equals(_oChanges.DbaseName.ToLower())).FirstOrDefault();
                        if (oCASC_Db != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Database name already used for another denomination" + (oCASC_Db.AppGlobalOwner != null ? ", " + oCASC_Db.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });

                    }

                    else if (d.oCASConfig.Id == 0)  //add
                    {
                        var existCASC = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                        if (existCASC != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Client server configuration already exist" + (existCASC.AppGlobalOwner != null ? ". Denomination (Church): " + existCASC.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });
                         
                        var oCASC_Db = _context.ClientAppServerConfig.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.DbaseName.ToLower().Equals(_oChanges.DbaseName.ToLower())).FirstOrDefault();
                        if (oCASC_Db != null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Database name already used for another denomination" + (oCASC_Db.AppGlobalOwner != null ? ", " + oCASC_Db.AppGlobalOwner.OwnerName : ""), pageIndex = vm.PageIndex });
                   
                    }
                }
            }


            // all clear.....
            var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// 

            using (var _cascCtx = new MSTR_DbContext(_cs))
            {
                var oCASC_CntAdd = 0; var oCASC_CntUpd = 0;
                foreach (var d in vm.lsCASConfigModels)
                {
                    if (d.oCASConfig != null)
                    {
                        if (d.oCASConfig.Id > 0)  // update
                        {
                            var oCASC = _context.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == d.oCASConfig.AppGlobalOwnerId && c.Id == d.oCASConfig.Id).FirstOrDefault();  
                            if (oCASC != null)  
                            {
                                oCASC.AppGlobalOwnerId = d.oCASConfig.AppGlobalOwnerId;  
                                oCASC.ServerName = d.oCASConfig.ServerName;  
                                oCASC.DbaseName  = d.oCASConfig.DbaseName;  
                                oCASC.SvrUserId = d.oCASConfig.SvrUserId;
                                oCASC.SvrPassword = d.oCASConfig.SvrPassword;
                                oCASC.ConfigDate = d.oCASConfig.ConfigDate;
                                oCASC.LastMod = DateTime.Now;
                                //
                                oCASC_CntUpd++;
                                _cascCtx.Update(oCASC);
                            }
                        }

                        else if (d.oCASConfig.Id == 0)  //add
                        {
                            ClientAppServerConfig oCASC = new ClientAppServerConfig()
                            {
                                AppGlobalOwnerId = d.oCASConfig.AppGlobalOwnerId,
                                ServerName = d.oCASConfig.ServerName,
                                DbaseName = d.oCASConfig.DbaseName,
                                SvrUserId = d.oCASConfig.SvrUserId,
                                SvrPassword = d.oCASConfig.SvrPassword,  
                                ConfigDate = d.oCASConfig.ConfigDate,
                                Created = DateTime.Now,
                                LastMod = DateTime.Now,
                            };

                            //
                            oCASC_CntAdd++;
                            _cascCtx.Add(oCASC);
                        }
                    }
                }


                var _userTask = "";
                if ((oCASC_CntAdd + oCASC_CntUpd) > 0)
                {
                    if (oCASC_CntAdd > 0)
                    { 
                        _userTask = "Added new " + oCASC_CntAdd + " Client server configurations for " + strDesc.ToLower() + " successfully.";
                        ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + "Created " + oCASC_CntAdd + " client server configurations." ;
                    }

                    if (oCASC_CntUpd > 0)
                    { 
                        _userTask = !string.IsNullOrEmpty(_userTask) ? _userTask + ". " : "" + "Updated " + oCASC_CntUpd + " Client server configurations for " + strDesc.ToLower() + " successfully.";
                        ViewBag.UserMsg = (!string.IsNullOrEmpty(ViewBag.UserMsg) ? ViewBag.UserMsg + ". " : "") + Environment.NewLine + Environment.NewLine + oCASC_CntUpd + " client server configurations updated.";
                    }

                    //save all...
                    await _cascCtx.SaveChangesAsync();


                    var _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: Client Server Configuration", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));


                    return Json(new { taskSuccess = true, userMess = ViewBag.UserMsg, pageIndex = vm.PageIndex });
                }

            }

            return Json(new { taskSuccess = false, userMess = "Saving data failed. Please refresh and try again.", pageIndex = vm.PageIndex });
        }
         

        public IActionResult Delete_CASC(int? loggedUserId, int id, bool forceDeleteConfirm = false)  // (int? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
        {
            var strDesc = "Client server configuration" ; var _tm = DateTime.Now; var _userTask = "Attempted deleting " + strDesc.ToLower(); 
            //
            try
            {
                var tm = DateTime.Now;
                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];
                //
                var oCASC = _context.ClientAppServerConfig.AsNoTracking().Include(t=>t.AppGlobalOwner).Where(c => c.Id == id).FirstOrDefault(); // .Include(c => c.ChurchUnits)
                if (oCASC == null)
                {
                    _userTask = "Attempted deleting " + strDesc.ToLower() + " -- [#Id: " + id + "]";
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " to delete could not be retrieved." });
                }

                var saveDelete = true;
                // ensuring cascade delete where there's none!

                //check for the AGO and CB
                // var oChurchBodies = _context.MSTRAppGlobalOwner.Where(c => c.Id == oCASC.AppGlobalOwnerId).ToList();
                // var oMTs = _CASCientContext.TransferTypeChurchLevel.Where(c => c.ChurchLevelId == oCASC.Id).ToList(); 

                using (var _cascCtx = new MSTR_DbContext(_cs))
                { 
                    //if (oChurchBodies.Count() > 0)
                    //{
                    //    var strConnTabs = oChurchBodies.Count() > 0 ? "Denomination (church)" : "";
                    //    //strConnTabs = strConnTabs.Length > 0 ? strConnTabs + ", " : strConnTabs;
                    //    //strConnTabs = oMTs.Count() > 0 ? strConnTabs + "Church transfer" : strConnTabs;

                    //    if (forceDeleteConfirm == false)
                    //    {
                    //        saveDelete = false;
                    //        // check user privileges to determine... administrator rights
                    //        // log 

                    //        _userTask = "Attempted deleting " + strDesc.ToLower() + " -- [#Id: " + id + "]" + (oCASC.AppGlobalOwner != null ? ". Denomination: " + oCASC.AppGlobalOwner.OwnerName : "");
                    //        _tm = DateTime.Now;
                    //        _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                    //                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    //        return Json(new
                    //        {
                    //            taskSuccess = false,
                    //            tryForceDelete = false,
                    //            oCurrId = id,
                    //            userMess = "Specified " + strDesc.ToLower() + " to delete has been used elsewhere in the system [" + strConnTabs + "]. Delete cannot be done unless dependent-references are removed."
                    //        });

                    //        //super_admin task
                    //        //return Json(new { taskSuccess = false, tryForceDelete = true, oCurrId = id, userMess = "Specified " + strDesc.ToLower() + 
                    //        //       " has dependencies or links with other external data [Faith category]. Delete cannot be done unless child refeneces are removed. DELETE (cascade) anyway?" });
                    //    }
                    //}

                    //successful...
                    if (saveDelete)
                    {
                        _cascCtx.ClientAppServerConfig.Remove(oCASC);
                        _cascCtx.SaveChanges(); 
                        
                        DetachAllEntities(_cascCtx);

                        _userTask = "Deleted " + strDesc.ToLower() + " -- [#Id: " + oCASC.Id + "]" + (oCASC.AppGlobalOwner != null ? ". Denomination: " + oCASC.AppGlobalOwner.OwnerName : "") + " successfully";  // var _userTask = "Attempted saving " + strDesc;
                        _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                        return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oCASC.Id, userMess = strDesc + " successfully deleted." });
                    }

                    else
                    {   DetachAllEntities(_cascCtx); return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" }); }
                   
                }

            }

            catch (Exception ex)
            {
                _userTask = "Attempted deleting " + strDesc.ToLower() + " -- [#Id: " + id + "] but FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
            }
        }


        /// <summary>
        /// CONTACT DETAILS --- for AGO, CB [cong amd cong unit levels]
        /// </summary>
        /// <returns></returns>






        ////////////////////////////
        private List<UserRole> GetUserRoles()  //(int? userRoleId = null, int? oCurrChuBodyId = null)
        {  //System roles ... oCurrChuBodyId == null
           // if (oCurrChuBodyId == null) return new List<UserPermission>();

            var userRoles = (
                        //from t_upr in _context.UserRolePermission.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Status == "A" && (userRoleId == null || (userRoleId != null && c.UserRoleId == userRoleId)))
                        from t_up in _context.UserRole.AsNoTracking()
                        select new UserRole()
                        {
                            Id = t_up.Id,
                            ChurchBodyId = t_up.ChurchBodyId,
                            RoleType = t_up.RoleType,
                            RoleDesc = t_up.RoleDesc,
                            RoleLevel = t_up.RoleLevel,
                            RoleStatus = t_up.RoleStatus,
                            RoleName = t_up.RoleName,
                            Created = t_up.Created,
                            CreatedByUserId = t_up.CreatedByUserId,
                            LastMod = t_up.LastMod,
                            LastModByUserId = t_up.LastModByUserId

                        })
                           .OrderBy(c => c.RoleLevel).ThenBy(c => c.RoleName)
                           .ToList();

            return userRoles;
        }
        private List<UserPermission> GetUserPermissions()  //(int? userRoleId = null, int? oCurrChuBodyId = null)
        {  //System roles ... oCurrChuBodyId == null
           // if (oCurrChuBodyId == null) return new List<UserPermission>();

            var userPerms = (
                        //from t_upr in _context.UserRolePermission.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Status == "A" && (userRoleId == null || (userRoleId != null && c.UserRoleId == userRoleId)))
                        from t_up in _context.UserPermission.AsNoTracking()
                        from t_up_c in _context.UserPermission.Where(c => c.Id == t_up.UserPermCategoryId).DefaultIfEmpty()
                        select new UserPermission()
                        {
                            Id = t_up.Id,
                            // ChurchBodyId = t_up.ChurchBodyId,
                            PermissionCode = t_up.PermissionCode,
                            PermissionName = t_up.PermissionName,
                            strPermDesc = AppUtilties.GetPermissionDesc_FromName(t_up.PermissionName),
                            PermStatus = t_up.PermStatus,
                            UserPermCategoryId = t_up.UserPermCategoryId,
                            strPermCategory = t_up_c != null ? AppUtilties.GetPermissionDesc_FromName(t_up_c.PermissionName) : "",
                            // Crud = t_up.Crud,
                            Created = t_up.Created,
                            CreatedByUserId = t_up.CreatedByUserId,
                            LastMod = t_up.LastMod,
                            LastModByUserId = t_up.LastModByUserId

                        }
                               )
                               .OrderBy(c => c.PermissionCode).ToList();

            return userPerms;
        }

        private List<UserAuditTrail> GetUserAuditTasks()  //(int? userRoleId = null, int? oCurrChuBodyId = null)
        {  //System roles ... oCurrChuBodyId == null
           // if (oCurrChuBodyId == null) return new List<UserPermission>();

            //  var userAudits = _context.UserAuditTrail.ToList().OrderByDescending(c => c.Created).ToList();

            var userAudits = (from t_uat in _context.UserAuditTrail.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchBody)
                              from t_up in _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_uat.AppGlobalOwnerId && c.ChurchBodyId == t_uat.ChurchBodyId && c.Id == t_uat.UserProfileId)
                              select new UserAuditTrail()
                              {
                                  Id = t_uat.Id,
                                  AppGlobalOwnerId = t_uat.AppGlobalOwnerId,
                                  ChurchBodyId = t_uat.ChurchBodyId,
                                  UserProfileId = t_uat.UserProfileId,
                                  EventDetail = t_uat.EventDetail,
                                  EventDate = t_uat.EventDate,
                                  AuditType = t_uat.AuditType,
                                  UI_Desc = t_uat.UI_Desc,
                                  Url = t_uat.Url,
                                  //
                                  Created = t_uat.Created,
                                  CreatedByUserId = t_uat.CreatedByUserId,
                                  LastMod = t_uat.LastMod,
                                  LastModByUserId = t_uat.LastModByUserId,
                                  //
                                  strEventDate = t_uat.EventDate != null ? DateTime.Parse(t_uat.EventDate.ToString()).ToString("ddd, dd MMM yyyy h:mm tt", CultureInfo.InvariantCulture) : "",
                                  strAuditType = GetAuditTypeDesc(t_uat.AuditType),
                                  strEventUser = t_up != null ? t_up.UserDesc : "",
                                  strSubscriber = (t_uat.AppGlobalOwner != null ? t_uat.AppGlobalOwner.OwnerName + " - " : "") + (t_uat.ChurchBody != null ? t_uat.ChurchBody.Name : ""),
                              })
                                 .OrderByDescending(c => c.Created).ToList();

            return userAudits;
        }

        private List<UserPermission> GetPermissionsByRole(int? userRoleId = null, int? oCurrChuBodyId = null)
        {  //System roles ... oCurrChuBodyId == null
           // if (oCurrChuBodyId == null) return new List<UserPermission>();

            var userPerms = (
                        from t_upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId==null && c.ChurchBodyId == null && c.Status == "A" && (userRoleId == null || (userRoleId != null && c.UserRoleId == userRoleId)))
                        from t_up in _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && c.Id == t_upr.UserRoleId)
                        select t_up
                               )
                               .OrderBy(c => c.PermissionCode).ToList();

            return userPerms;
        }

        public JsonResult GetPermissionsListByRole(int? userRoleId, int? oCurrChuBodyId = null)  //, bool addEmpty = false)
        {
            var userPerms = (
                        from t_upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A" && (userRoleId == null || (userRoleId != null && c.UserRoleId == userRoleId)))
                        from t_up in _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && c.Id == t_upr.UserRoleId)
                        select t_up
                               ).OrderBy(c => c.PermissionCode).ToList()
                                .Select(c => new SelectListItem()
                                {
                                    Value = c.Id.ToString(),
                                    Text = c.PermissionName
                                })
                                .OrderBy(c => c.Text)
                                .ToList();

            // if (addEmpty) userPerms.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(userPerms);
        }

        private List<UserPermission> GetPermissionsByGroup(int? userGroupId = null, int? oCurrChuBodyId = null)
        {
            if (oCurrChuBodyId == null) return new List<UserPermission>();

            var userPerms = (
                        from t_upr in _context.UserGroupPermission.AsNoTracking().Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Status == "A" && (userGroupId == null || (userGroupId != null && c.UserGroupId == userGroupId)))
                        from t_up in _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && c.Id == t_upr.UserGroupId)
                               .OrderBy(c => c.PermissionCode)
                        select t_up
                               )
                               .ToList();

            return userPerms;
        }

        public JsonResult GetPermissionsListByGroup(int? userGroupId, int? oCurrChuBodyId = null)  //, bool addEmpty = false)
        {
            var userPerms = (
                        from t_upr in _context.UserGroupPermission.AsNoTracking().Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Status == "A" && (userGroupId == null || (userGroupId != null && c.UserGroupId == userGroupId)))
                        from t_up in _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && c.Id == t_upr.UserGroupId)
                        select t_up
                               ).OrderBy(c => c.PermissionCode).ToList()
                                .Select(c => new SelectListItem()
                                {
                                    Value = c.Id.ToString(),
                                    Text = c.PermissionName
                                })
                                .OrderBy(c => c.Text)
                                .ToList();

            // if (addEmpty) userPerms.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(userPerms);
        }


        // GET: ChurchBodyConfig 
        public ActionResult Index(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0, int? oParentId = null, int? id = null, int pageIndex = 1) //, int? oChuCategId = null, bool oShowAllCong = true) //, int? currFilterVal = null) //, ChurchBodyConfigMDL oCurrCBConfig = null)
        {
            //Request.Headers.Add("entityId", "COPDatabase");
            //Request.Headers.TryGetValue("entityId", out var entityVal);
            //var entityValue =  entityVal;

            // SetUserLogged();
            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                // check permission 
                if (!this.userAuthorized) return View(new AppVenAdminVM()); //retain view                  
                var oLoggedUser = oUserLogIn_Priv.UserProfile;  // if (oCurrChuBodyLogOn == null) return View(oCBConVM);
                if (oLoggedUser == null) return View(new AppVenAdminVM());
                //
                var oCBConVM = new AppVenAdminVM(); //TempData.Keep();  
                                                    // int? oAppGloOwnId = null;
                var oChuBodyLogOn = oUserLogIn_Priv.ChurchBody;
                //
                int? oAppGloOwnId_Logged = null;
                int? oChuBodyId_Logged = null;
                int? oUserId_Logged = null;
                if (oChuBodyLogOn != null)
                {
                    oAppGloOwnId_Logged = oChuBodyLogOn.AppGlobalOwnerId;
                    oChuBodyId_Logged = oChuBodyLogOn.Id;
                    if (oCurrChuBodyId == null) { oCurrChuBodyId = oChuBodyLogOn.Id; }
                    if (oAppGloOwnId == null) { oAppGloOwnId = oChuBodyLogOn.AppGlobalOwnerId; }
                    //else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...
                    //
                    // oAppGloOwnId = oCurrChuBodyLogOn.AppGlobalOwnerId;
                }


                int? oCurrChuMemberId_LogOn = null;
                Models.CLNTModels.ChurchMember oCurrChuMember_LogOn = null;


                oUserId_Logged = oLoggedUser.Id;
                //var currChurchMemberLogged = _clientContext.ChurchMember.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Id == oLoggedUser.ChurchMemberId).FirstOrDefault();
                //if (currChurchMemberLogged != null) //return View(oCBConVM);
                //{
                //    oCurrChuMemberId_LogOn = currChurchMemberLogged.Id;
                //    oCurrChuMember_LogOn = currChurchMemberLogged;
                //}

                //                
                oCBConVM.oAppGloOwnId = oAppGloOwnId;
                oCBConVM.oChurchBodyId = oCurrChuBodyId;
                //
                oCBConVM.oUserId_Logged = oUserId_Logged;
                oCBConVM.oChurchBody_Logged = oChuBodyLogOn;
                oCBConVM.oAppGloOwnId_Logged = oAppGloOwnId_Logged;
                oCBConVM.oMemberId_Logged = oCurrChuMemberId_LogOn;
                //
                oCBConVM.setIndex = setIndex;
                oCBConVM.subSetIndex = subSetIndex;
                oCBConVM.pageIndex = pageIndex;
                //      

                //// var oHomeDash1 = new HomeDashboardVM();

                //// oCBConVM.strChurchLevelDown = "Assemblies";
                //oCBConVM.strAppName = "RhemaCMS"; ViewBag.strAppName = oCBConVM.strAppName;
                //oCBConVM.strAppNameMod = "Admin Palette"; ViewBag.strAppNameMod = oCBConVM.strAppNameMod;
                //oCBConVM.strAppCurrUser = "Dan Abrokwa"; ViewBag.strAppCurrUser = oCBConVM.strAppCurrUser;
                //// oHomeDash.strChurchType = "CH"; ViewBag.strChurchType = oHomeDash.strChurchType;
                //// oHomeDash.strChuBodyDenomLogged = "Rhema Global Church"; ViewBag.strChuBodyDenomLogged = oHomeDash.strChuBodyDenomLogged;
                ////  oHomeDash.strChuBodyLogged = "Rhema Comm Chapel"; ViewBag.strChuBodyLogged = oHomeDash.strChuBodyLogged;

                ////           
                //ViewBag.strAppCurrUser_ChRole = "System Adminitrator";
                //ViewBag.strAppCurrUser_RoleCateg = "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
                //ViewBag.strAppCurrUserPhoto_Filename = "2020_dev_sam.jpg";
                //// ViewBag.strAppCurrChu_LogoFilename = "14dc86a7-81ae-462c-b73e-4581bd4ee2b2_church-of-pentecost.png";
                //ViewBag.strUserSessionDura = "Logged: 10 minutes ago";

                //

                if (setIndex == 1) // user profiles
                {
                    var _strCurrTask = ""; var _proScope = ""; var _subScope = "";
                    if (subSetIndex >= 1 && subSetIndex <= 5)  // sys=1   sup_admin=2  sys_admin=3 sys_cust=4
                    {
                        _proScope = "V"; _subScope = "";
                        _strCurrTask = "System Admin Profiles";
                    }
                    else if (subSetIndex == 6 || subSetIndex == 11)  // CH_admin=6,CH_rgstr=7,CH_acct=8,CH_cust=9... CF_admin=11
                    {
                        _proScope = "C"; _subScope = "D";
                        _strCurrTask = "Church Admin Profiles";
                    }
                    else if (subSetIndex >= 6 && subSetIndex <= 15)  // CH_admin, CF_admin
                    {
                        _proScope = "C"; _subScope = "A";
                        _strCurrTask = "Church User Profiles";
                    }

                    if (pageIndex == 1) //read
                    {
                        //if (subSetIndex >= 1 && subSetIndex <= 5)  // sys   sup_admin  sys_admin sys_cust
                        //{
                        //    oCBConVM.lsUserProfiles = GetUserProfiles(null, null, "V", ""); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                        //    oCBConVM.strCurrTask = "System Admin Profiles";
                        //}
                        //else if (subSetIndex == 6 || subSetIndex == 11)  // CH_admin, CF_admin
                        //{
                        //    oCBConVM.lsUserProfiles = GetUserProfiles(null, null, "C", "D"); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                        //    oCBConVM.strCurrTask = "Church Admin Profiles";
                        //}
                        //else if (subSetIndex >= 6 && subSetIndex <= 15)  // CH_admin, CF_admin
                        //{
                        //    oCBConVM.lsUserProfiles = GetUserProfiles(null, null, "C", "A"); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                        //    oCBConVM.strCurrTask = "Church User Profiles";
                        //}

                        if (subSetIndex >= 1 && subSetIndex <= 15)
                        {
                            oCBConVM.lsUserProfiles = GetUserProfiles(oAppGloOwnId, oCurrChuBodyId, _proScope, _subScope);
                            oCBConVM.strCurrTask = _strCurrTask;
                        }
                        else if (subSetIndex == 21) //enlist all roles
                        {
                            oCBConVM.lsRoles = GetUserRoles(); // (null, null, "V", ""); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                            oCBConVM.strCurrTask = "System Roles";
                        }
                        else if (subSetIndex == 21) //enlist all privileges
                        {
                            oCBConVM.lsPermissions = GetUserPermissions(); // (null, null, "V", ""); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                            oCBConVM.strCurrTask = "System Privileges";
                        }
                    }

                    else if (pageIndex == 2 || pageIndex == 3)  //edit
                    {  //   
                        var oUserModel = AddOrEdit_UPR(oAppGloOwnId, oCurrChuBodyId, id, setIndex, subSetIndex, oAppGloOwnId_Logged, oChuBodyId_Logged, oUserId_Logged);
                        oCBConVM.oUserProfileModel = oUserModel;

                        // var oUserModel = new UserProfileModel();
                        // oUserModel.oUserProfile = new UserProfile(); //_context.UserProfile.FirstOrDefault();
                        // oUserModel.oCurrUserId_Logged = oUserProfile_Logged.Id; oUserModel.oAppGloOwnId_Logged = ; oUserModel.oChurchBodyId_Logged = oAppGloOwnId_Logged;
                        if (oUserModel != null)
                        {
                            var cc = "000000";
                            //get the roles, groups and privileges    .... _clientContext, 
                            oCBConVM.lsPermissions = AppUtilties.GetUserAssignedPermissions(_context, _proScope == "V" ? cc : oLoggedUser.ChurchBody?.GlobalChurchCode, oLoggedUser);
                            oCBConVM.lsProfileRoles = (from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.UserProfileId == oLoggedUser.Id && (c.ChurchBodyId == null || c.ChurchBodyId == oLoggedUser.ChurchBodyId)) //&& c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                                       from ur in _context.UserRole.AsNoTracking().Where(c => c.Id == upr.UserRoleId && (c.ChurchBodyId == null || c.ChurchBodyId == oLoggedUser.ChurchBodyId))
                                                       select upr).ToList();
                            oCBConVM.lsProfileGroups = (from upg in _context.UserProfileGroup.AsNoTracking().Where(c => c.UserProfileId == oLoggedUser.Id && (c.ChurchBodyId == null || c.ChurchBodyId == oLoggedUser.ChurchBodyId)) //&& c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                                        from ur in _context.UserGroup.AsNoTracking().Where(c => c.Id == upg.UserGroupId && (c.ChurchBodyId == null || c.ChurchBodyId == oLoggedUser.ChurchBodyId))
                                                        select upg).ToList();

                            // the admin profiles
                            oCBConVM.lsUserProfiles = GetUserProfiles(oLoggedUser.AppGlobalOwnerId, oLoggedUser.ChurchBodyId, _proScope, _subScope);
                            //oCBConVM.strCurrTask = _strCurrTask;
                        }

                        else
                        {
                            Response.StatusCode = 403;  //obj null
                            return PartialView("_ErrorPage");
                        }
                    }
                }

                else if (setIndex == 2) // church faith types
                {
                    //oCBConVM.lsFaithCategories = GetChurchFaithTypes(setIndex);

                    //if (subSetIndex == 0)
                    //{
                    //    oCBConVM.strCurrTask = "Faith Streams";
                    //}
                    //else if (subSetIndex == 1)
                    //{
                    //    oCBConVM.strCurrTask = "Faith Categories";
                    //}
                    //else
                    //{
                    //    oCBConVM.strCurrTask = "All Faith Types";
                    //}
                    //   // oCBConVM.strCurrTask = subSetIndex == 1 ? "Faith Category" : subSetIndex == 2 ? "Faith Sub-Category" : "All Faith Category";                  
                }

                else if (setIndex == 3)  //denominations
                {
                    if (oCBConVM.oCurrDenomVM != null)
                    {
                        oCBConVM.oCurrDenomVM.oAppGloOwnId = (int)oAppGloOwnId;

                        if (subSetIndex == 0)
                        {
                            oCBConVM.oCurrDenomVM = GetDenomination((int)oAppGloOwnId);
                            oCBConVM.strCurrTask = "Denominations";
                        }
                        else if (subSetIndex == 1)
                        {
                            oCBConVM.oCurrDenomVM.lsChurchLevels = GetChurchLevels((int)oAppGloOwnId);
                            oCBConVM.strCurrTask = "Church Levels";
                        }
                        else if (subSetIndex == 2)
                        {
                            oCBConVM.oCurrDenomVM.lsChurchBodies = GetCongregations((int)oAppGloOwnId);
                            oCBConVM.strCurrTask = "Congregations";
                        }

                        else if (subSetIndex == 3)
                        {
                            oCBConVM.lsUserProfiles = GetUserProfiles((int)oAppGloOwnId, (int)oCurrChuBodyId, "C", "D"); // -- GET ALL CH ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                            oCBConVM.strCurrTask = "Church Admin Profiles";
                        }

                        else if (subSetIndex == 4)
                        {
                            // oCBConVM.oCurrDenomVM.lsSubscriptions = Subscriptions((int)oAppGloOwnId, "C", "D", 6);
                            oCBConVM.strCurrTask = "Subcriptions";
                        }

                        //else if (subSetIndex == 5)
                        //{
                        //    oCBConVM.oCurrDenomVM.ChurchAdminProfiles = GetUserProfiles(); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                        //    oCBConVM.strCurrTask = "Vendor User Profiles";
                        //}

                        else   // subSetIndex == 0
                        {
                            oCBConVM.lsDenominations = GetDenominations();
                            oCBConVM.strCurrTask = "Denominations";
                        }
                    }

                    else   // subSetIndex == 0
                    {
                        oCBConVM.lsDenominations = GetDenominations();
                        oCBConVM.strCurrTask = "Denominations";
                    }
                }

                else if (setIndex == 4)  //all subsriptions
                {

                }

                else if (setIndex == 5)  // other app parameters
                {
                    oCBConVM.lsCountries = GetCountries();
                    oCBConVM.strCurrTask = "Countries";
                }


                //  if (setIndex == 6) { oCBConVM.lsAppSubscriptions = GetAppSubscriptions(); oCBConVM.strCurrTask = "App Subscriptions"; }
                //if (setIndex == 7) { oCBConVM.lsUserProfiles = GetUserProfiles(oCBConVM.oChurchBody.Id); oCBConVM.strCurrTask = "User Profiles"; }
                //if (setIndex == 10) { oCBConVM.lsCountries = GetCountries(); oCBConVM.strCurrTask = "Countries & Regions"; }
                //if (setIndex == 11) { oCBConVM.lsCountryRegions = GetCountryRegions(oParentId); oCBConVM.strCurrTask = "Country Regions"; }

                //
                // TempData.Put("oVmCB_CNFG", oCBConVM);
                TempData.Keep();
                return View(oCBConVM);
            }
        }

        public IActionResult AddOrEdit_CNFG(int? oDenomId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0,
                                                 int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)

        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                {
                    if (setIndex == 1) // vendor admins
                    {
                        var oUserModel = AddOrEdit_UPR(oDenomId, oCurrChuBodyId, id, setIndex, subSetIndex, oAGOId_Logged, oCBId_Logged, oUserId_Logged);
                        // var oUserModel = new UserProfileModel();
                        // oUserModel.oUserProfile = new UserProfile(); //_context.UserProfile.FirstOrDefault();
                        // oUserModel.oCurrUserId_Logged = oUserProfile_Logged.Id; oUserModel.oAppGloOwnId_Logged = ; oUserModel.oChurchBodyId_Logged = oAppGloOwnId_Logged;
                        if (oUserModel != null)
                            if (oUserModel.oUserProfile != null)
                                return PartialView("_AddOrEdit_UPR", oUserModel); //PartialView("_AddOrEdit_UPR", oMdl1); // break;
                            else
                            {
                                
                                return PartialView("_ErrorPage");
                            }
                        else
                        {
                            Response.StatusCode = 403;  //obj null
                            return PartialView("_ErrorPage");
                        }
                    }

                    //else if (setIndex == 2) // church faith types
                    //{
                    //    //var oCFTModel = AddOrEdit_CFT(oDenomId, oCurrChuBodyId, id, setIndex, subSetIndex); 
                    //    //if (oCFTModel != null)
                    //    //    if (oCFTModel.oChurchFaithType != null)
                    //    //        return PartialView("_AddOrEdit_CFT", oCFTModel); //PartialView("_AddOrEdit_UPR", oMdl1); // break;
                    //    //    else
                    //    //    {
                    //    //        
                    //    //        return PartialView("_ErrorPage");
                    //    //    }
                    //    //else
                    //    //{
                    //    //    Response.StatusCode = 403;  //obj null
                    //    //    return PartialView("_ErrorPage");
                    //    //}




                    //    //var oMdl1 = AddOrEdit_CFT(oDenomId, oCurrChuBodyId, id, setIndex, subSetIndex);
                    //    //if (oMdl1 != null) return PartialView("_AddOrEdit_CFT", oMdl1); // break;
                    //    //else
                    //    //{
                    //    //    Response.StatusCode = 403;  //obj null
                    //    //    return PartialView("_ErrorPage");
                    //    //}

                    //    //// oCBConVM.lsFaithCategories = GetChurchFaithTypes(setIndex);

                    //    //if (subSetIndex == 1)
                    //    //{
                    //    //     oCBConVM.strCurrTask = "Faith Types";
                    //    //}
                    //    //else if (subSetIndex == 2)
                    //    //{
                    //    //    oCBConVM.strCurrTask = "Faith Sub-Types";
                    //    //}
                    //    //else
                    //    //{
                    //    //    oCBConVM.strCurrTask = "All Faith Types";
                    //    //}
                    //    // oCBConVM.strCurrTask = subSetIndex == 1 ? "Faith Category" : subSetIndex == 2 ? "Faith Sub-Category" : "All Faith Category";                  
                    //}

                    //else if (setIndex == 3)  //denominations
                    //{
                    //    if (oDenomId != null)
                    //    {
                    //       // oCBConVM.oCurrDenomVM.oAppGloOwnId = (int)oDenomId;

                    //        if (subSetIndex == 0)
                    //        {
                    //            //oCBConVM.oCurrDenomVM = GetDenomination((int)oDenomId);
                    //            //oCBConVM.strCurrTask = "Denominations";
                    //        }
                    //        else if (subSetIndex == 1)
                    //        {
                    //            //oCBConVM.oCurrDenomVM.lsChurchLevels = GetChurchLevels((int)oDenomId);
                    //            //oCBConVM.strCurrTask = "Church Levels";
                    //        }
                    //        else if (subSetIndex == 2)
                    //        {
                    //           // oCBConVM.oCurrDenomVM.lsChurchBodies = GetCongregations((int)oDenomId);
                    //            //oCBConVM.strCurrTask = "Congregations";
                    //        }
                    //        else if (subSetIndex == 3)
                    //        {
                    //            //oCBConVM.oCurrDenomVM.lsChurchAdminProfiles = GetUserProfiles((int)oDenomId, "C", "D", 6);
                    //           // oCBConVM.strCurrTask = "Church Admin Profiles";
                    //        }

                    //        else if (subSetIndex == 4)
                    //        {
                    //            // oCBConVM.oCurrDenomVM.lsSubscriptions = Subscriptions((int)oDenomId, "C", "D", 6);
                    //            //oCBConVM.strCurrTask = "Subcriptions";
                    //        }

                    //        //else if (subSetIndex == 5)
                    //        //{
                    //        //    oCBConVM.oCurrDenomVM.ChurchAdminProfiles = GetUserProfiles(); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                    //        //    oCBConVM.strCurrTask = "Vendor User Profiles";
                    //        //}

                    //        else   // subSetIndex == 0
                    //        {
                    //           // oCBConVM.lsDenominations = GetDenominations();
                    //            //oCBConVM.strCurrTask = "Denominations";
                    //        }
                    //    }

                    //    else   // subSetIndex == 0
                    //    {
                    //       // oCBConVM.lsDenominations = GetDenominations();
                    //       // oCBConVM.strCurrTask = "Denominations";
                    //    }
                    //}

                    //else if (setIndex == 4)  //all subsriptions
                    //{

                    //}

                    //else if (setIndex == 5)  // other app parameters
                    //{
                    //    //oCBConVM.lsCountries = GetCountries();
                    //    //oCBConVM.strCurrTask = "Countries";
                    //    return View(); //clear line later
                    //} 

                    else
                        return View(); //clear line later
                                       //}
                }

                //page not found error
                
                return PartialView("_ErrorPage");
            }
        }

        public UserProfileModel AddOrEdit_UPR(int? oDenomId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0,
                                                                            int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) //, int pageIndex = 1)
        {

            var oUserModel = new UserProfileModel(); TempData.Keep();
            if (setIndex == 0) return oUserModel;

            // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
            var proScope = "V"; var subScope = "";
            if (subSetIndex >= 1 && subSetIndex <= 5) { proScope = "V"; subScope = ""; }
            else if (subSetIndex == 6 || subSetIndex == 11) { proScope = "C"; subScope = "D"; }
            else if (subSetIndex >= 6 && subSetIndex <= 15) { proScope = "C"; subScope = "A"; }

            if (id == 0)
            {   //create user and init... 
                //var existSUP_ADMNs = (
                //   from t_up in _context.UserProfile.AsNoTracking() //.Include(t => t.ChurchMember)
                //                .Where(c => c.Id == id &&
                //                (c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V") ||
                //                (c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C"))
                //   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId && c.AppGlobalOwnerId == t_up.AppGlobalOwnerId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                //   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                //                    .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                //                    ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                //                     ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                //                     ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                //                    )).DefaultIfEmpty()
                //   select t_up
                //   ).ToList();

                ////supadmin <creation> task.... but must have logged in as SYS
                //if (setIndex==1 && subSetIndex==2 && existSUP_ADMNs.Count > 0)
                //{ //prompt user sup_admin == 1 only
                //    oUserModel.oUserProfile = null;
                //    return oUserModel;
                //}

                var oUser = new UserProfile();
                oUser.ChurchBodyId = oCurrChuBodyId;
                oUser.Strt = DateTime.Now;
                oUser.ResetPwdOnNextLogOn = true;

                //oUPR_MDL.oUserProfile.CountryId = oCurrCtryId;

                oUser.UserStatus = "A";   // A-ctive...D-eactive   
                oUser.ProfileScope = proScope;

                if (subSetIndex >= 1 && subSetIndex <= 5) // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
                {
                    oUser.UserScope = "E";  // I-internal, E-external
                    if (subSetIndex == 2) { oUser.Username = "supadmin"; oUser.UserDesc = "Super Admin"; }
                }
                else // I-internal, E-external [manually config]
                { oUser.UserScope = "I"; }

                oUserModel.oUserProfile = oUser;
            }

            else
            {
                var oUser = (
                   from t_up in _context.UserProfile.Include(t => t.ContactInfo).AsNoTracking() //.Include(t => t.ChurchMember)
                                .Where(c => c.Id == id &&
                                (c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V") ||
                                (c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C"))
                   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId && c.AppGlobalOwnerId == t_up.AppGlobalOwnerId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                    .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                                    ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                                     ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                                     ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                                    )).DefaultIfEmpty()

                       //// from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == t_up.ChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()
                       // from t_ur in _context.UserRole.AsNoTracking().Where(c => c.Id == t_upr.UserRoleId &&
                       //              ((c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5)) ||
                       //              ((c.RoleType == "CH_ADMN" || c.RoleType == "CF_ADMN") && (c.RoleLevel >= 6 && c.RoleLevel <= 10))).DefaultIfEmpty()
                       // from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                       //              .Where(c => c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                       // from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                       //              .Where(c => c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       // from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                       //              .Where(c => c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                   select new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_up.ChurchBody,
                       //  ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,
                       //
                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc,
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                       // PhoneNum = t_up.PhoneNum,
                       Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                       //
                       OwnerUserId = t_up.OwnerUserId,
                     //  UserId = t_up.UserId,
                       UserScope = t_up.UserScope,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)
                   }
                   ).FirstOrDefault();

                oUserModel.oUserProfile = oUser;
                if (oUser != null)
                {
                    oUserModel.strUserProfile = oUser.UserDesc;
                    oUserModel.strChurchBody = oUser.ChurchBody != null ? oUser.ChurchBody.Name : "";
                    oUserModel.strAppGlobalOwn = oUser.AppGlobalOwner != null ? oUser.AppGlobalOwner.OwnerName : "";

                    //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                    // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc

                }
            }

            if (oUserModel.oUserProfile != null)
            {
                if (oUserModel.oUserProfile.AppGlobalOwnerId != null)
                {
                    List<MSTRChurchLevel> oCBLevels = _context.MSTRChurchLevel.AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oUserModel.oUserProfile.AppGlobalOwnerId).ToList().OrderBy(c => c.LevelIndex).ToList();

                    if (oCBLevels.Count() > 0)
                    {
                        ViewBag.Filter_ln = !string.IsNullOrEmpty(oCBLevels[oCBLevels.Count - 1].CustomName) ? oCBLevels[oCBLevels.Count - 1].CustomName : oCBLevels[6].Name;
                        ViewBag.Filter_1 = !string.IsNullOrEmpty(oCBLevels[0].CustomName) ? oCBLevels[0].CustomName : oCBLevels[0].Name;

                        if (oCBLevels.Count() > 1)
                        {
                            ViewBag.Filter_2 = ViewBag.Filter_2 = !string.IsNullOrEmpty(oCBLevels[1].CustomName) ? oCBLevels[1].CustomName : oCBLevels[1].Name;
                            if (oCBLevels.Count() > 2)
                            {
                                ViewBag.Filter_3 = ViewBag.Filter_3 = !string.IsNullOrEmpty(oCBLevels[2].CustomName) ? oCBLevels[2].CustomName : oCBLevels[2].Name;
                                if (oCBLevels.Count() > 3)
                                {
                                    ViewBag.Filter_4 = ViewBag.Filter_4 = !string.IsNullOrEmpty(oCBLevels[3].CustomName) ? oCBLevels[3].CustomName : oCBLevels[3].Name;
                                    if (oCBLevels.Count() > 4)
                                    {
                                        ViewBag.Filter_5 = ViewBag.Filter_5 = !string.IsNullOrEmpty(oCBLevels[4].CustomName) ? oCBLevels[43].CustomName : oCBLevels[4].Name;
                                        if (oCBLevels.Count() > 5)
                                        {
                                            ViewBag.Filter_6 = ViewBag.Filter_6 = !string.IsNullOrEmpty(oCBLevels[5].CustomName) ? oCBLevels[5].CustomName : oCBLevels[5].Name;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            oUserModel.setIndex = setIndex;
            oUserModel.subSetIndex = subSetIndex;
            oUserModel.oCurrUserId_Logged = oUserId_Logged;
            oUserModel.oAppGloOwnId_Logged = oAGOId_Logged;
            oUserModel.oChurchBodyId_Logged = oCBId_Logged;
            //
            oUserModel.oAppGloOwnId = oDenomId;
            oUserModel.oChurchBodyId = oCurrChuBodyId;
            //  oUserModel.oCurrUserId_Logged = oCurrUserId_Logged; 

            // ChurchBody oCB = null;
            // if (oCurrChuBodyId != null)  oCB = _context.MSTRChurchBody.Where(c=>c.Id == oCurrChuBodyId && c.AppGlobalOwnerId==oDenomId).FirstOrDefault();

            if (subSetIndex >= 1 && subSetIndex <= 5) // no SUP_ADMN or SYS as option
                oUserModel = this.populateLookups_UPR_MS(oUserModel, oDenomId, subSetIndex);

            else if ((oUserModel.profileScope == "V" && (subSetIndex == 6 || subSetIndex == 11)) || (oUserModel.profileScope == "C" && subSetIndex >= 6 && subSetIndex <= 15))
            {
                // var oCB = _context.MSTRChurchBody.Where(c => c.Id == oCurrChuBodyId && c.AppGlobalOwnerId == oDenomId).FirstOrDefault();
                oUserModel = this.populateLookups_UPR_CL(oUserModel, oCurrChuBodyId);
            }

            //oUPR_MDL.lkpStatuses = new List<SelectListItem>();
            //foreach (var dl in dlGenStatuses) { oUPR_MDL.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            //TempData["oVmCurr"] = oUserModel;
            //TempData.Keep();

            // var _oUserModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUserModel);
            // TempData["oVmCurr"] = _oUserModel; TempData.Keep();

            var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(oUserModel);
            TempData["oVmCurrMod"] = _vmMod; TempData.Keep();

            return oUserModel;
        }


        //public UserProfileVM AddOrEdit_UPR2(int? oDenomId = null, int? oCurrChuBodyId = null, int id = 0, int setIndex = 0, int subSetIndex = 0)
        public UserProfileModel AddOrEdit_UPR2(int? oDenomId = null, int? oCurrChuBodyId = null, int id = 0, int setIndex = 0, int subSetIndex = 0)
        {
            var oUPR_MDL = new UserProfileModel(); TempData.Keep();
            if (setIndex == 0) return oUPR_MDL;
            oUPR_MDL.setIndex = setIndex;
            oUPR_MDL.subSetIndex = subSetIndex;

            ////
            //var oCurrChuBodyLogOn = oUserLogIn_Priv.ChurchBody;
            //var oUserProfile = oUserLogIn_Priv.UserProfile;
            //if (oCurrChuBodyLogOn == null) return oUPR_MDL;

            //if (oCurrChuBodyId == null) oCurrChuBodyId = oCurrChuBodyLogOn.Id;
            //else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...

            //// check permission for Core life...
            //if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) //prompt!
            //    return oUPR_MDL;

            //int? oCurrChuMemberId_LogOn = null;
            //ChurchMember oCurrChuMember_LogOn = null;
            //if (oUserProfile == null) //prompt!
            //    return oUPR_MDL;
            //if (oUserProfile.ChurchMember == null) //prompt!
            //    return oUPR_MDL;

            //oCurrChuMemberId_LogOn = oUserProfile.ChurchMember.Id;
            //oCurrChuMember_LogOn = oUserProfile.ChurchMember;

            if (id == 0)
            {         //create user and init... 
                oUPR_MDL.oUserProfile = new UserProfile();
                oUPR_MDL.oUserProfile.ChurchBodyId = oCurrChuBodyId;
                //oUPR_MDL.oUserProfile.CountryId = oCurrCtryId;
                oUPR_MDL.oUserProfile.UserScope = "I"; // I-internal, E-external
                oUPR_MDL.oUserProfile.UserStatus = "A";   // A-ctive...D-eactive   

                if (setIndex == 1) // sys admin
                {
                    oUPR_MDL.oUserProfile.ProfileScope = "V"; // V-Vendor, C-Client
                }

                else if (setIndex == 3) // && subSetIndex==3) //church admin acc
                {
                    oUPR_MDL.oUserProfile.ProfileScope = "C";
                }
            }

            else
            {
                if (setIndex == 1) // sys admin
                {
                    oUPR_MDL = (
                    from t_up in _context.UserProfile.Include(t => t.ContactInfo).AsNoTracking().Where(c => c.Id == id && c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V")  //.Include(t => t.ChurchMember)
                    from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                    from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                        // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                    from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                                       (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                        //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                    from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                                  .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                    from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                                 .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                    from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                                 .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                    select new UserProfileModel()
                    {
                        // oUserProfile = t_up,

                        oUserProfile = new UserProfile()
                        {
                            Id = t_up.Id,
                            AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                            ChurchBodyId = t_up.ChurchBodyId,
                            ChurchMemberId = t_up.ChurchMemberId,
                            ChurchBody = t_up.ChurchBody,
                            // ChurchMember = t_up.ChurchMember,
                            OwnerUser = t_up.OwnerUser,

                            Username = t_up.Username,
                            UserDesc = t_up.UserDesc,
                            Email = t_up.Email,
                            ContactInfo = t_up.ContactInfo,
                            //PhoneNum = t_up.PhoneNum,
                            Pwd = t_up.Pwd,
                            PwdExpr = t_up.PwdExpr,
                            PwdSecurityQue = t_up.PwdSecurityQue,
                            PwdSecurityAns = t_up.PwdSecurityAns,
                            ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                            Strt = t_up.Strt,
                            strStrt = t_up.strStrt,
                            Expr = t_up.Expr,
                            strExpr = t_up.Expr != null ?
                                                                 DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                            OwnerUserId = t_up.OwnerUserId,
                          //  UserId = t_up.UserId,
                            UserScope = t_up.UserScope,
                            UserPhoto = t_up.UserPhoto,
                            ProfileScope = t_up.ProfileScope,
                            Created = t_up.Created,
                            CreatedByUserId = t_up.CreatedByUserId,
                            LastMod = t_up.LastMod,
                            LastModByUserId = t_up.LastModByUserId,
                            UserStatus = t_up.UserStatus,
                            strUserStatus = GetStatusDesc(t_up.UserStatus)

                        },

                        //  lsUserGroups = t_upg.UserGroups,
                        // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                        // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                        strUserProfile = t_up.UserDesc,

                        strChurchBody = t_cb != null ? t_cb.Name : "",
                        strAppGlobalOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                        //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                        // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
                    }
                    )
                    //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
                    .FirstOrDefault();
                }

                else if (setIndex == 3) // && subSetIndex==3) //church admin acc
                {

                    oUPR_MDL = (
                      from t_up in _context.UserProfile.Include(t => t.ContactInfo).AsNoTracking().Where(c => c.Id == id && c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C")  //.Include(t => t.ChurchMember)
                      from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                      from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                          // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == oChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()                   
                      from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                                       (c.RoleType == "CH_ADMN" || c.RoleType == "CF_ADMN") && (c.RoleLevel >= 6 && c.RoleLevel <= 10))
                          //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                      from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                              .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                      from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                                   .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                      from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                                   .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                      select new UserProfileModel()
                      {
                          // oUserProfile = t_up,

                          oUserProfile = new UserProfile()
                          {
                              Id = t_up.Id,
                              AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                              ChurchBodyId = t_up.ChurchBodyId,
                              ChurchMemberId = t_up.ChurchMemberId,
                              ChurchBody = t_up.ChurchBody,
                              // ChurchMember = t_up.ChurchMember,
                              OwnerUser = t_up.OwnerUser,

                              Username = t_up.Username,
                              UserDesc = t_up.UserDesc,
                              Email = t_up.Email,
                              ContactInfo = t_up.ContactInfo,
                              // PhoneNum = t_up.PhoneNum,
                              Pwd = t_up.Pwd,
                              PwdExpr = t_up.PwdExpr,
                              PwdSecurityQue = t_up.PwdSecurityQue,
                              PwdSecurityAns = t_up.PwdSecurityAns,
                              ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                              Strt = t_up.Strt,
                              strStrt = t_up.strStrt,
                              Expr = t_up.Expr,
                              strExpr = t_up.Expr != null ?  DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                              OwnerUserId = t_up.OwnerUserId,
                            //  UserId = t_up.UserId,
                              UserScope = t_up.UserScope,
                              UserPhoto = t_up.UserPhoto,
                              ProfileScope = t_up.ProfileScope,
                              Created = t_up.Created,
                              CreatedByUserId = t_up.CreatedByUserId,
                              LastMod = t_up.LastMod,
                              LastModByUserId = t_up.LastModByUserId,
                              UserStatus = t_up.UserStatus,
                              strUserStatus = GetStatusDesc(t_up.UserStatus)

                          },

                          //  lsUserGroups = t_upg.UserGroups,
                          // lsUserRoles = t_upr != null ? t_upr.UserRoles : null,
                          // lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),

                          strUserProfile = t_up.UserDesc,

                          strChurchBody = t_cb != null ? t_cb.Name : "",
                          strAppGlobalOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                          //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                          // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
                      }
                      )
                      //.OrderBy(c => c.oUserRole.RoleDesc).ThenBy(c => c.strUserProfile)
                      .FirstOrDefault();
                }


                //oUPR_MDL = (
                //      from t_up in _context.UserProfile.AsNoTracking().Include(t => t.ChurchMember)
                //      .Where(x => x.ChurchBodyId == oCurrChuBodyId && x.Id == id)
                //      from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(x => x.Id == oCurrChuBodyId && x.Id == t_up.ChurchBodyId)
                //      from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.Id == oCurrChuBodyId && x.Id == t_up.ChurchMemberId)
                //      from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRoles).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.UserProfileId == t_up.Id).DefaultIfEmpty()
                //      from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermissions).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                //      from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroups).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.UserProfileId == t_up.Id).DefaultIfEmpty()
                //      from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermissions).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()
                //      select new UserProfileVM()
                //      {
                //          oUserProfile = t_up,
                //          lsUserGroups = t_upg.UserGroups,
                //          lsUserRoles = t_upr.UserRoles,
                //          lsUserPermissions = CombineCollection(t_urp.UserPermissions, t_ugp.UserPermissions, null, null, null),
                //          strCongregation = t_cb != null ? t_cb.Name : "",
                //          strAppGloOwn = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                //          strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                //          strUserProfile = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim()
                //      }
                //    ).FirstOrDefault();
            }


            oUPR_MDL.oAppGloOwnId = oDenomId;
            oUPR_MDL.oChurchBodyId = oCurrChuBodyId;
            //oUPR_MDL.oCurrLoggedMember = oCurrChuMember_LogOn;
            //oUPR_MDL.oCurrLoggedMemberId = oCurrChuMemberId_LogOn;

            // ChurchBody oCB = null;
            // if (oCurrChuBodyId != null)  oCB = _context.MSTRChurchBody.Where(c=>c.Id == oCurrChuBodyId && c.AppGlobalOwnerId==oDenomId).FirstOrDefault();

            if (setIndex == 1 || (setIndex == 3 && subSetIndex == 3))
                oUPR_MDL = this.populateLookups_UPR_MS(oUPR_MDL, oDenomId, setIndex);

            else
            {
                // var oCB = _context.MSTRChurchBody.Where(c => c.Id == oCurrChuBodyId && c.AppGlobalOwnerId == oDenomId).FirstOrDefault();
                oUPR_MDL = this.populateLookups_UPR_CL(oUPR_MDL, oCurrChuBodyId);
            }


            //oUPR_MDL.lkpStatuses = new List<SelectListItem>();
            //foreach (var dl in dlGenStatuses) { oUPR_MDL.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            TempData["oVmCurr"] = oUPR_MDL;
            TempData.Keep();

            return oUPR_MDL;

        }


        /// ////////////////// 

        private UserProfileModel populateLookups_UPR_CL(UserProfileModel vmLkp, int? oChurchBodyId)  //AppGloOwnId   ChurchBody oCurrChuBody)
        {
            if (vmLkp == null || oChurchBodyId == null) return vmLkp;
            //
            vmLkp.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vmLkp.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkpUserRoles = _context.UserRole.AsNoTracking().Where(c => c.RoleStatus == "A" && c.RoleLevel >= 6 && c.ChurchBodyId == oChurchBodyId)
                               .OrderBy(c => c.RoleLevel)
                               .Select(c => new SelectListItem()
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.RoleName.Trim()
                               })
                               // .OrderBy(c => c.Text)
                               .ToList();
            //  vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vmLkp.lkpUserGroups = _context.UserGroup.AsNoTracking().Where(c => c.Status == "A" && c.ChurchBodyId == oChurchBodyId)
                               .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
                               .Select(c => new SelectListItem()
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.GroupName.Trim()
                               })
                               // .OrderBy(c => c.Text)
                               .ToList();


            vmLkp.lkpPwdSecQueList = _context.AppUtilityNVP.AsNoTracking().Where(c => c.NvpCode == "PWD_SEC_QUE")
                      .OrderBy(c => c.RequireUserCustom).ThenBy(c => c.OrderIndex).ThenBy(c => c.NvpValue)
                      .ToList()
                      .Select(c => new SelectListItem()
                      {
                          Value = c.Id.ToString(),
                          Text = c.NvpValue
                      })
                      // .OrderBy(c => c.Text)
                      .ToList();
            vmLkp.lkpPwdSecQueList.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            //vmLkp.lkpPwdSecAnsList = _context.AppUtilityNVP.Where(c => c.NvpCode == "PWD_SEC_ANS")
            //         .OrderBy(c => c.RequireUserCustom).ThenBy(c => c.OrderIndex).ThenBy(c => c.NvpValue)
            //         .ToList()
            //         .Select(c => new SelectListItem()
            //         {
            //             Value = c.Id.ToString(),
            //             Text = c.NvpValue
            //         })
            //         // .OrderBy(c => c.Text)
            //         .ToList();
            //vmLkp.lkpPwdSecAnsList.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            return vmLkp;
        }

        private UserProfileModel populateLookups_UPR_MS(UserProfileModel vmLkp, int? AppGloOwnId, int subSetIndex)
        {
            //if (vmLkp == null || oDenom == null) return vmLkp;
            // 
            vmLkp.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vmLkp.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            //vmLkp.lkpUserRoles = _context.UserRole.Where(c => AppGloOwnId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 0 && c.RoleType=="SUP_ADMN" )
            if (subSetIndex >= 2 && subSetIndex <= 3)  //   SYS .. 1-SUP_ADMN, 2-SYS_ADMN, 3-SYS_CUST
            {
                vmLkp.lkpUserRoles = _context.UserRole.AsNoTracking().Where(c => AppGloOwnId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel >= 2 && c.RoleLevel <= 5)
                    .OrderBy(c => c.RoleLevel)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.Id.ToString(),
                        Text = c.RoleName.Trim()
                    })
                    // .OrderBy(c => c.Text)
                    .ToList();
                //  vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

                vmLkp.lkpUserGroups = _context.UserGroup.AsNoTracking().Where(c => AppGloOwnId == null && c.ChurchBodyId == null && c.Status == "A")
                                   .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
                                   .Select(c => new SelectListItem()
                                   {
                                       Value = c.Id.ToString(),
                                       Text = c.GroupName.Trim()
                                   })
                                   // .OrderBy(c => c.Text)
                                   .ToList();
            }

            else if (subSetIndex == 6 || subSetIndex == 7)  //  6-CH_ADMN, 7-CF_ADMN
            {
                vmLkp.lkpUserRoles = _context.UserRole.AsNoTracking().Where(c => c.RoleStatus == "A" && c.RoleLevel >= 6 && c.RoleLevel <= 10 && c.ChurchBody.AppGlobalOwnerId == AppGloOwnId)
                    .OrderBy(c => c.RoleLevel)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.Id.ToString(),
                        Text = c.RoleName.Trim()
                    })
                    // .OrderBy(c => c.Text)
                    .ToList();
                //  vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

                vmLkp.lkpUserGroups = _context.UserGroup.AsNoTracking().Where(c => c.Status == "A" && c.ChurchBody.AppGlobalOwnerId == AppGloOwnId)
                                   .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
                                   .Select(c => new SelectListItem()
                                   {
                                       Value = c.Id.ToString(),
                                       Text = c.GroupName.Trim()
                                   })
                                   // .OrderBy(c => c.Text)
                                   .ToList();
            }

            return vmLkp;
        }
         

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_UPR(UserProfileModel vmMod)
        {

            UserProfile _oChanges = vmMod.oUserProfile;
            //   vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vmMod; TempData.Keep();

            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileModel>(arrData) : vmMod;

            var oCV = vmMod.oUserProfile;
            oCV.ChurchBody = vmMod.oChurchBody;

            try
            {
                ModelState.Remove("oUserProfile.AppGlobalOwnerId");
                ModelState.Remove("oUserProfile.ChurchBodyId");
                ModelState.Remove("oUserProfile.ChurchMemberId");
                ModelState.Remove("oUserProfile.CreatedByUserId");
                ModelState.Remove("oUserProfile.LastModByUserId");
                ModelState.Remove("oUserProfile.OwnerId");
                ModelState.Remove("oUserProfile.UserId");

                // ChurchBody == null 

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", signOutToLogIn = false });

                if (string.IsNullOrEmpty(_oChanges.Username)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide username and password.", signOutToLogIn = false });
                }
                //if (_oChanges.PwdSecurityQue != null && string.IsNullOrEmpty(_oChanges.PwdSecurityAns))  //Congregant... ChurcCodes required
                //{
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the response to the security question specified.", signOutToLogIn = false });
                //}



                //confirm this is SYS acc   //check for the SYS acc
                var currLogUserInfo = (from up in _context.UserProfile.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == vmMod.oCurrUserId_Logged)
                                       from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.UserProfileId == up.Id && c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                       from ur in _context.UserRole.AsNoTracking().Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SYS")
                                       select new
                                       {
                                           UserId = up.Id,
                                           UserRoleId = ur.Id,
                                           UserType = ur.RoleType,
                                           UserRoleLevel = ur.RoleLevel,
                                           UserStatus = up.strUserStatus == "A" && upr.ProfileRoleStatus == "A" && ur.RoleStatus == "A"
                                       }
                                 ).FirstOrDefault();

                if (currLogUserInfo == null)
                { return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user not found! Please refresh and try again.", signOutToLogIn = false }); }

                if (_oChanges.ProfileScope == "V")  //vendor admins ... SYS, SUP_ADMN, SYS_ADMN etc.
                {
                    if (currLogUserInfo.UserType == "SYS" && string.Compare(_oChanges.Username, "supadmin", true) != 0 && string.Compare(_oChanges.Username, "sys", true) == 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account can ONLY manage the [sys] or [supadmin] profile. Hint: Sign in with [supadmin] or other Admin account.", signOutToLogIn = false });
                    }

                    if (currLogUserInfo.UserType == "SUP_ADMN" && string.Compare(_oChanges.Username, "sys", true) == 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user does not have SYS role. SYS role required to manage SYS account.", signOutToLogIn = false });
                    }

                    if (currLogUserInfo.UserType != "SUP_ADMN" && currLogUserInfo.UserType != "SYS" && string.Compare(_oChanges.Username, "supadmin", true) == 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user does not have SUP_ADMN role. SUP_ADMN role required to manage SUP_ADMN account.", signOutToLogIn = false });
                    }

                    if (_oChanges.Id == 0)
                    {
                        if (string.Compare(_oChanges.Username, "sys", true) == 0)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                                  from ur in _context.UserRole.AsNoTracking().Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SYS")
                                                  select upr
                                     );
                            if (existUserRoles.Count() > 0)
                            {
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account is not available. Only one (1) SYS role allowed.", signOutToLogIn = false });
                            }
                        }

                        if (string.Compare(_oChanges.Username, "supadmin", true) == 0)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                                  from ur in _context.UserRole.AsNoTracking().Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                                                  select upr
                                     );
                            if (existUserRoles.Count() > 0)
                            {
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Super Admin profile is not available. Only one (1) SUP_ADMN role allowed.", signOutToLogIn = false });
                            }
                        }
                    }
                }

                else  //CLIENT ADMINs ... creating users for their churches /congregations
                {
                    //check availability of username... SYS /SUP_ADMN reserved
                    if (string.Compare(_oChanges.Username, "SYS", true) == 0 || string.Compare(_oChanges.Username, "supadmin", true) == 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Username 'supadmin' not available. Try different username.", signOutToLogIn = false });
                    }
                }


                //check that username is unique in all instances
                var existUserProfiles = _context.UserProfile.AsNoTracking().Where(c => (c.ChurchBodyId == null || c.ChurchBodyId == _oChanges.ChurchBodyId) && c.Id != _oChanges.Id && c.Username.Trim().ToLower() == _oChanges.Username.Trim().ToLower()).ToList();
                if (existUserProfiles.Count() > 0)
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Username '" + _oChanges.Username + "' not available. Try different username.", signOutToLogIn = false });
                }

                if (string.IsNullOrEmpty(_oChanges.UserDesc))  //Congregant... ChurchCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the user description or name of user.", signOutToLogIn = false });
                }

                if (_oChanges.Expr != null)  //allow historic
                {
                    if (_oChanges.UserStatus == "A" && _oChanges.Expr.Value <= DateTime.Now.Date)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please user account has expired. Activate account first.", signOutToLogIn = false });

                    if (_oChanges.Strt != null)
                        if (_oChanges.Strt.Value > _oChanges.Expr.Value)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Start date cannot be later than expiry date", signOutToLogIn = false });
                }

                //check email availability and validity
                if (_oChanges.Email != null) //_oChanges.ChurchMemberId != null && 
                {
                    var oExistUser = _context.MSTRContactInfo.AsNoTracking().Where(c => c.RefUserId != _oChanges.Id && c.Email == _oChanges.Email).FirstOrDefault();
                    if (oExistUser != null)  // ModelState.AddModelError(_oChanges.Id.ToString(), "Email of member must be unique. >> Hint: Already used by another member: "  + GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName) + "[" + oCM.ChurchBody.Name + "]");
                        return Json(new
                        {
                            taskSuccess = false,
                            oCurrId = _oChanges.Id,
                            userMess = "User email must be unique. >> Hint: Already used by another: [User: " + _oChanges.UserDesc + "]", //  GetConcatMemberName(_oChanges.ChurchMember.Title, _oChanges.ChurchMember.FirstName, _oChanges.ChurchMember.MiddleName, _oChanges.ChurchMember.LastName) + "[" + oCV.ChurchBody.Name + "]",
                            signOutToLogIn = false
                        });

                    //if (_oChanges == null)
                    //{
                    //    return Json(new
                    //    {
                    //        taskSuccess = false,
                    //        oCurrId = _oChanges.Id,
                    //        userMess = "Member status [ current state of the person - active, dormant, invalid, deceased etc. ] is required"
                    //    });
                    //}

                    ////member must be active, NOT deceased
                    //if (_oChanges_MS.ChurchMemStatusId == null)
                    //{
                    //    return Json(new
                    //    {
                    //        taskSuccess = false,
                    //        oCurrId = _oChanges.Id,
                    //        userMess = "Select the Member status [current state of the person - active, dormant, invalid, deceased etc.] as applied"
                    //    });
                    //}
                }


                _oChanges.LastMod = DateTime.Now;
                _oChanges.LastModByUserId = vmMod.oCurrUserId_Logged;
                string uniqueFileName = null;

                var oFormFile = vmMod.UserPhotoFile;
                if (oFormFile != null && oFormFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img_db");  //~/frontend/dist/img_db
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + oFormFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    oFormFile.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                else
                    if (_oChanges.Id != 0) uniqueFileName = _oChanges.UserPhoto;

                _oChanges.UserPhoto = uniqueFileName;

                //_oChanges.PwdSecurityQue = "What account is this?"; _oChanges.PwdSecurityAns = "Rhema-SYS";
                if (!string.IsNullOrEmpty((_oChanges.PwdSecurityQue + _oChanges.PwdSecurityAns).Trim()))
                    _oChanges.PwdSecurityAns = AppUtilties.ComputeSha256Hash(_oChanges.PwdSecurityQue + _oChanges.PwdSecurityAns);

                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.CreatedByUserId = vmMod.oCurrUserId_Logged;

                //validate...
                if (_oChanges.Id == 0)
                {
                    _oChanges.Pwd = "123456";  //temp pwd... to reset @ next login   
                    if (_oChanges.ProfileScope == "V")
                    {
                        var cc = "000000";    //var churchCode = "000000"; _oChanges.Username = "SysAdmin"; _oChanges.Pwd = "$ys@dmin1";                                        
                        _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username + _oChanges.Pwd);
                    }
                    else
                        _oChanges.Pwd = AppUtilties.ComputeSha256Hash(_oChanges.Username + _oChanges.Pwd);

                    _oChanges.Strt = tm;
                    _oChanges.Expr = tm.AddDays(90);  //default to 90 days
                    _oChanges.ResetPwdOnNextLogOn = true;
                    _oChanges.PwdExpr = tm.AddDays(30);  //default to 30 days 

                    _oChanges.Created = tm;
                    _oChanges.CreatedByUserId = vmMod.oCurrUserId_Logged;
                    _context.Add(_oChanges);

                    ViewBag.UserMsg = "Saved user profile " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully. Password must be changed on next logon";
                }
                else
                {
                    //retain the pwd details... hidden fields
                    _context.Update(_oChanges);
                    ViewBag.UserMsg = "User profile updated successfully.";
                }

                //save user profile first... 
                await _context.SaveChangesAsync();

                //check if role assigned... SUP_ADMN -- auto, others -- warn!

                if (vmMod.subSetIndex == 2) // SUP_ADMN role
                {
                    var oSupAdminRole = _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                    if (oSupAdminRole != null)
                    {
                        var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.UserRoleId == oSupAdminRole.Id && c.ProfileRoleStatus == "A") // && 
                                                                                                                                                                                        // ((c.Strt == null || c.Expr == null) || (c.Strt != null && c.Expr != null && c.Strt <= DateTime.Now && c.Expr >= DateTime.Now && c.Strt <= c.Expr)))
                                                                                                                                                                                        // from up in _context.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                                              select upr
                                     );

                        //add SUP_ADMN role to SUP_ADMN user ... assign all privileges to the SUP_ADMN role
                        if (existUserRoles.Count() == 0)
                        {
                            //var oSupAdminRole = _context.UserRole.Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                            //if (oSupAdminRole != null)
                            //{                             
                            var oUserRole = new UserProfileRole
                            {
                                ChurchBodyId = null,
                                UserRoleId = oSupAdminRole.Id,
                                UserProfileId = _oChanges.Id,
                                Strt = tm,
                                Expr = tm,
                                ProfileRoleStatus = "A",
                                Created = tm,
                                LastMod = tm,
                                CreatedByUserId = vmMod.oCurrUserId_Logged,
                                LastModByUserId = vmMod.oCurrUserId_Logged
                            };

                            _context.Add(oUserRole);
                            //save user role...
                            await _context.SaveChangesAsync();
                            ViewBag.UserMsg += Environment.NewLine + " ~ SUP_ADMN role added.";
                            // }

                            if (oSupAdminRole != null)
                            {
                                // assign all privileges to the SUP_ADMN role 
                                var existUserRolePerms = (from upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A" && c.UserRoleId == oSupAdminRole.Id && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in _context.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                          select upr);
                                //if (existUserRolePerms.Count() > 0)
                                //{
                                var oUserPerms = (from upr in _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A")                                                                                                                                                      // from up in _context.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                  select upr);

                                if (oUserPerms.Count() > 0) //(existUserRolePerms.Count() < oUserPerms.Count())
                                {
                                    var rowUpdated = false; var rowsUpdated = 0; var rowsAdded = 0;
                                    foreach (var oURP in oUserPerms)
                                    {
                                        var existUserRolePerm = existUserRolePerms.Where(c => c.UserPermissionId == oURP.Id).FirstOrDefault();
                                        if (existUserRolePerm == null)
                                        {
                                            var oUserRolePerm = new UserRolePermission
                                            {
                                                AppGlobalOwnerId = null,
                                                ChurchBodyId = null,
                                                UserRoleId = oSupAdminRole.Id,
                                                UserPermissionId = oURP.Id,
                                                ViewPerm = true,
                                                CreatePerm = true,
                                                EditPerm = true,
                                                DeletePerm = true,
                                                ManagePerm = true,
                                                Status = "A",
                                                Created = tm,
                                                LastMod = tm,
                                                CreatedByUserId = vmMod.oCurrUserId_Logged,
                                                LastModByUserId = vmMod.oCurrUserId_Logged
                                            };

                                            _context.Add(oUserRolePerm);
                                            rowsAdded++;
                                        }
                                        else
                                        {
                                            rowUpdated = false;
                                            if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; rowUpdated = true; }
                                            if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = true; rowUpdated = true; }
                                            if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; rowUpdated = true; }
                                            if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = true; rowUpdated = true; }
                                            if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = true; rowUpdated = true; }

                                            if (rowUpdated)
                                            {
                                                existUserRolePerm.Created = tm;
                                                existUserRolePerm.LastMod = tm;
                                                existUserRolePerm.CreatedByUserId = vmMod.oCurrUserId_Logged;
                                                existUserRolePerm.LastModByUserId = vmMod.oCurrUserId_Logged;

                                                _context.Add(existUserRolePerm);
                                                rowsUpdated++;
                                            }
                                        }
                                    }

                                    // prompt users
                                    if (rowsAdded > 0) ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user permissions added.";
                                    if (rowsUpdated > 0) ViewBag.UserMsg += ". " + rowsUpdated + " user permissions updated.";
                                }
                                //}
                            }
                        }
                    }


                }

                // oCM_NewConvert.Created = DateTime.Now;
                // _context.Add(_oChanges);


                //save changes... 
                await _context.SaveChangesAsync();


                //user roles / user groups and/or user permissions [ tick ... pick from the attendance concept]
                //var oMemRoles = currCtx.MemberChurchRole.Include(t => t.LeaderRole).ThenInclude(t => t.LeaderRoleCategory)
                //    .Where(c => c.LeaderRole.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrServing == true).ToList();
                //foreach (var oMR in oMemRoles)
                //{
                //    oMR.IsCurrServing = false;
                //    oMR.Completed = oChuTransf.TransferDate;
                //    oMR.CompletionReason = oChuTransf.TransferType;
                //    oMR.LastMod = DateTime.Now;
                //    //
                //    currCtx.Update(oMR);
                //}
                //ViewBag.UserMsg += " Church visitor added to church as New Convert successfully. Update of member details may however be required."  


                //save everything
                // await _context.SaveChangesAsync();

                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurr"] = _vmMod; TempData.Keep();


                //if (_oChanges.PwdExpr != null)
                //{
                //    if (_oChanges.PwdExpr.Value >= DateTime.Now.Date)
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please user password has expired. Check email/phone for confirm code to activate password.", signOutToLogIn = true  });
                //}

                // return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = false });
                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving user profile details. Err: " + ex.Message, signOutToLogIn = false });
            }
        }

        public async Task<IActionResult> Delete_UPR(int? id)
        {
            var res = false;
            var UserProfile = await _context.UserProfile.FindAsync(id);
            if (UserProfile != null)
            {
                //check all member related modules for references to deny deletion

                res = true;
                if (res)
                {
                    _context.UserProfile.Remove(UserProfile);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(UserProfile.Username, "Delete failed. User Profile data is referenced elsewhere in the Application.");
                    ViewBag.UserDelMsg = "Delete failed. User Profile data is referenced elsewhere in the Application.";
                }

                return Json(res);
            }

            else
                return Json(false);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit_SYS(UserProfileVM vmMod, string churchCode)
        {
            try
            {
                //UserProfile _oChanges = vmMod.oUserProfile;
                // vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileVM : vmMod; TempData.Keep();

                var _churchCode = churchCode;

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _churchCode, userMess = "Failed to create requested user. Please refresh and try again.", signOutToLogIn = false });


                //var tm = DateTime.Now;
                //_oChanges.LastMod = tm;
                //_oChanges.LastModByUserId = vmMod.oCurrLoggedUserId;

                if (!string.IsNullOrEmpty(_churchCode))
                {
                    var userProList = (from t_upx in _context.UserProfile.AsNoTracking().Where(c => _churchCode == "000000" && c.ChurchBodyId == null && c.ProfileScope == "V" && c.UserStatus == "A")
                                       from t_upr in _context.UserProfileRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.UserProfileId == t_upx.Id && c.ProfileRoleStatus == "A").DefaultIfEmpty()
                                       from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 0 && c.RoleType == "SYS").DefaultIfEmpty()
                                       select t_upx
                                 ).OrderBy(c => c.UserDesc).ToList();

                    if (userProList.Count > 0)
                        return Json(new { taskSuccess = false, oCurrId = _churchCode, userMess = "SYS account profile already created. There could only be one SYS account.", signOutToLogIn = false });

                }


                //create user and init...
                var _oChanges = new UserProfile();

                //_oChanges.AppGlobalOwnerId = null; // oCV.ChurchBody != null ? oCV.ChurchBody.AppGlobalOwnerId : null;
                //_oChanges.ChurchBodyId = null; //(int)oCV.ChurchBody.Id;
                //_oChanges.OwnerId =null; // (int)vmMod.oCurrLoggedUserId;

                var tm = DateTime.Now;
                _oChanges.Strt = tm;
                // ChurchBody == null

                //_oChanges.Expr = null; // tm.AddDays(90);  //default to 30 days
                //  oCurrVmMod.oUserProfile.UserId = oCurrChuMemberId_LogOn;
                //_oChanges.ChurchMemberId = null; // vmMod.oCurrLoggedMemberId;

                _oChanges.UserScope = "E"; // I-internal, E-external
                _oChanges.ProfileScope = "V"; // V-Vendor, C-Client
                _oChanges.ResetPwdOnNextLogOn = true;
                _oChanges.PwdSecurityQue = "What account is this?";
                _oChanges.PwdSecurityAns = "Rhema-SYS";
                _oChanges.Email = "samuel@rhema-systems.com";
                // _oChanges.PhoneNum = "233242188212";
                _oChanges.UserDesc = "Sys Profile";

                var cc = "000000"; _oChanges.Username = "Sys"; _oChanges.Pwd = "654321"; // [ get the raw data instead ]
                _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username + _oChanges.Pwd);

                _oChanges.PwdExpr = tm.AddDays(30);  //default to 90 days 
                _oChanges.UserStatus = "A"; // A-ctive...D-eactive

                _oChanges.Created = tm;
                _oChanges.LastMod = tm;
                _oChanges.CreatedByUserId = null; // (int)vmMod.oCurrLoggedUserId;
                _oChanges.LastModByUserId = null; // (int)vmMod.oCurrLoggedUserId;

                //_oChanges.UserPhoto = null;
                //_oChanges.UserId = null;
                //_oChanges.PhoneNum = null;
                //_oChanges.Email = null; 

                // 
                ViewBag.UserMsg = "Saved SYS account profile successfully. Sign-out and then sign-in to create Super Admin profile to perform the required settings /client configurations.";

                _context.Add(_oChanges);
                //save everything
                _context.SaveChanges();

                // TempData["oVmCurrMod"] = vmMod;
                // TempData.Keep();
                // return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = true });


                //succesful...  login required
                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = true });
                // return RedirectToAction("LoginUserAcc", "UserLogin");
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = churchCode, userMess = "Failed saving SYS account profile. Err: " + ex.Message, signOutToLogIn = false });
            }

        }




        // UP  --- STRICTLY FOR VENDOR ACCESS
        public ActionResult Index_UP(int setIndex, int pageIndex = 1) //int? oAGOId = null, , string proScope, string subScope = "")  // , string subScope="" int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            // SetUserLogged();
            if (!InitializeUserLogging(true)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {

                // check permission 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                //
                if (!this.userAuthorized) return View(new UserProfileModel()); //retain view    
                if (oUserLogIn_Priv == null) return View(new UserProfileModel());
                if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) 
                    return View(new UserProfileModel());

                var oLoggedUser = oUserLogIn_Priv.UserProfile;
                var arrRoles = oUserLogIn_Priv.arrAssignedRoleCodes;
                var arrPerms = oUserLogIn_Priv.arrAssignedPermCodes;

              //  var oLoggedRole = oUserLogIn_Priv.UserRole;

                //
                var oUPModel = new UserProfileModel(); //TempData.Keep();   // int? oAppGloOwnId = null;
               // var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
                //
                int? oAppGloOwnId_Logged = null;
                int? oChuBodyId_Logged = null;

                //if (oChuBody_Logged != null)
                //{
                //    oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
                //    oChuBodyId_Logged = oChuBody_Logged.Id;
                //}

                //if (oAGOId == null )
                //{ return PartialView("_ErrorPage"); }

                //var oAGO = _context.MSTRAppGlobalOwner.Find(oAGOId);
                //if (setIndex == 2 && oAGO == null)  // adding user to denomination ...  must be known
                //{ return PartialView("_ErrorPage"); }

                var oUserId_Logged = oLoggedUser.Id;

                // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
                var proScope = setIndex == 1 ? "V" : "C";
                var subScope = setIndex == 2 ? "D" : setIndex == 3 ? "A" : "";

                //var _subSetIndex = 0;
                //switch (oLoggedRole.RoleName.ToUpper())
                //{
                //    case "SYS": _subSetIndex = 1; break;
                //    case "SUP_ADMN": _subSetIndex = 2; break;
                //    case "SYS_ADMN": _subSetIndex = 3; break;
                //    case "SYS_CUST": _subSetIndex = 4; break;
                //    // case "SYS_???": _subSetIndex = 5; break;
                //    case "CH_ADMN": _subSetIndex = 6; break;
                //    case "CF_ADMN": _subSetIndex = 11; break;
                //}

                // var _strCurrTask = ""; var _proScope = ""; var _subScope = "";

                if (proScope == "V" && setIndex == 1) // && subSetIndex >= 1 && subSetIndex <= 5)  // subSetIndex >= 1 && subSetIndex <= 5)  // sys=1   sup_admin=2  sys_admin=3 sys_cust=4
                {   //_proScope = "V"; _subScope = "";
                    oUPModel.strCurrTask = "System Admin Profile";
                    oUPModel.lsUserProfileModels = GetUserProfileList_SysAdmin();
                }

                else if (proScope == "C" && (setIndex == 2 || setIndex == 3)) // ((subScope == "D" && setIndex == 2) || (subScope == "A" && setIndex == 3))) //  (proScope == "C" && ((subScope == "D" && (subSetIndex == 6 || subSetIndex == 11)) || (subScope == "A" && (subSetIndex >= 6 || subSetIndex <= 15))))  // && subScope == "D" ... D - aDmin users, A-All users, subSetIndex == 6 || subSetIndex == 11)  // CH_admin=6, CH_rgstr=7, CH_acct=8, CH_cust=9... CH_???=10
                {  //_proScope = "C"; _subScope = "D";   // 
                    oUPModel.strCurrTask = subScope == "D" ? "Church Admin Profile" : "Church User Profile";
                    oUPModel.lsUserProfileModels = GetUserProfileList_ChuAdmin(subScope);
                    ///
                    oUPModel.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Status == "A")
                                             .OrderBy(c => c.OwnerName).ToList()
                                             .Select(c => new SelectListItem()
                                             {
                                                 Value = c.Id.ToString(),
                                                 Text = c.OwnerName
                                             })
                                             .ToList();

                }

                //else if (proScope == "C" )  // subSetIndex >= 6 && subSetIndex <= 15)  // CF_admin=11, CF_rgstr=12, CF_acct=13, CF_cust=14... CF_???=15
                //{ // _proScope = "C"; _subScope = "A";
                //    oUPModel.strCurrTask = subScope == "A" ? "Church User Profiles" : "User Profiles"; 
                //    oUPModel.lsUserProfileModels = GetUserProfileList_ChuAdmin(subScope);
                //}

                //else if (proScope == "C" && subScope == "A")  // subSetIndex >= 6 && subSetIndex <= 15)  // CF_admin=11, CF_rgstr=12, CF_acct=13, CF_cust=14... CF_???=15
                //{ // _proScope = "C"; _subScope = "A";
                //    oUPModel.strCurrTask = "User Profiles";
                //    oUPModel.lsUserProfileModels = GetUserProfileList_ClientChuAdmin(oAppGloOwnId, oChurchBodyId);
                //}

                //if (subSetIndex >= 1 && subSetIndex <= 15)
                //{
                //    oUPModel.lsUserProfileModels = GetUserProfileList(oAppGloOwnId, oChurchBodyId, _proScope, _subScope);
                //    oUPModel.strCurrTask = _strCurrTask;
                //}

                else if (setIndex == 6) //enlist all roles
                {
                    oUPModel.lsUserRoles = GetUserRoles(); // (null, null, "V", ""); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                    oUPModel.strCurrTask = "System Role";
                }

                else if (setIndex == 7) //enlist all privileges
                {
                    oUPModel.lsUserPermissions = GetUserPermissions(); // (null, null, "V", ""); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                    oUPModel.strCurrTask = "System Privilege";
                }

                else if (setIndex == 8) //enlist all privileges
                {
                    oUPModel.lsUserAuditTrails = GetUserAuditTasks(); // (null, null, "V", ""); // -- GET ALL VENDOR ADMINS...  (null, "V", "", -1);  // Vendor Admin may need a seperate API for cross databases :: since every clients db differ by DBNAME
                    oUPModel.strCurrTask = "UserAudit Trail";
                }

                // oUPModel.strCurrTask = "Denominations (Churches)";

                //                
                //oUPModel.oAppGloOwnId = oAppGloOwnId;
                //oUPModel.oChurchBodyId = oCurrChuBodyId;
                //

                //oUPModel.oAppGloOwnId = oAGOId; 
                //oUPModel.oAppGlobalOwn = oAGO; 

                oUPModel.oUserId_Logged = oUserId_Logged;                
                oUPModel.oChurchBodyId_Logged = oChuBodyId_Logged; //oUPModel.oChurchBody_Logged = oChuBody_Logged;
                oUPModel.oAppGloOwnId_Logged = oAppGloOwnId_Logged;
                oUPModel.oUserProfile_Logged = oLoggedUser;
                oUPModel.arrAssignedRoleCodes = arrRoles;
                oUPModel.arrAssignedPermCodes = arrPerms;                 
                 oUPModel.oUserProfileLevel_Logged = oLoggedUser.ProfileLevel;   // sync this with the User_roles ... roles can be multiple
                 oUPModel.oUserProfileScope_Logged = oLoggedUser.ProfileScope;   // sync this with the User_roles ... roles can be multiple

                // oUPModel.oMemberId_Logged = oCurrChuMemberId_LogOn;
                //

                oUPModel.setIndex = setIndex;
               // oUPModel.subSetIndex = subSetIndex;   // unknown... yet
                oUPModel.pageIndex = pageIndex; // 1;
                oUPModel.profileScope = proScope;
                oUPModel.subScope = subScope;

               // // oUPModel.pageIndex = pageIndex;

               // ///
               // ViewData["strAppName"] = "RhemaCMS";
               // ViewData["strAppNameMod"] = "Admin Palette";
               // ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(oLoggedUser.UserDesc) ? oLoggedUser.UserDesc : "[Current user]";
               // ///
               // ViewData["oAppGloOwnId_Logged"] = oAppGloOwnId_Logged;
               // ViewData["oChuBodyId_Logged"] = oChuBodyId_Logged;

               //// ViewData["oCBOrgType_Logged"] =  oChuBody_Logged.OrgType;  // CH, CN but subscriber may come from oth

               // ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
               // ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
               // ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
               // ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
               //// ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
               // ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

               // //ViewData["strAppCurrUser_ChRole"] = oLoggedRole.RoleDesc; // "System Adminitrator";
               // //ViewData["strAppCurrUser_RoleCateg"] = oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

               //// ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
               //// ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
               // ///
               // ViewData["strAppCurrUserPhoto_Filename"] = oLoggedUser.UserPhoto;
               // // ViewData["strAppLogo_Filename"] = oLoggedUser.UserPhoto;
               // ViewData["strAppLogo_Filename"] = "~/frontend/dist/img/rhema_logo.png"; // oAppGloOwn?.ChurchLogo;   ...load from db


               // //refresh Dash values
               // _ = LoadDashboardValues();



                var _userTask = "Viewed " + oUPModel.strCurrTask.ToLower() + " list";
                var tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + oUPModel.strCurrTask, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oLoggedUser.Id, tm, tm, oLoggedUser.Id, oLoggedUser.Id));

                return View(oUPModel);
            }
        }
         

        [HttpGet]
        public IActionResult AddOrEdit_UP1(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int id = 0, int? oParentId = null, int setIndex = 0, int subSetIndex = 0,
                                                int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {
                var oCurrChuBodyLogOn_Logged = oUserLogIn_Priv.ChurchBody;
                var oUserProfile_Logged = oUserLogIn_Priv.UserProfile;
                // int? oAppGloOwnId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : (int?)null;
                //int? oChurchBodyId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : (int?)null;
                // int? oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : (int?)null;
                oUserId_Logged = oUserProfile_Logged != null ? oUserProfile_Logged.Id : oUserId_Logged;
                oCBId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.Id : oCBId_Logged;
                oAGOId_Logged = oCurrChuBodyLogOn_Logged != null ? oCurrChuBodyLogOn_Logged.AppGlobalOwnerId : oAGOId_Logged;

                if (StackAppUtilties.IsAjaxRequest(HttpContext.Request))
                {
                    var oUP_MDL = new ChurchFaithTypeModel();
                    if (id == 0)
                    {
                        //create user and init... 
                        oUP_MDL.oChurchFaithType = new ChurchFaithType();
                        oUP_MDL.oChurchFaithType.Level = subSetIndex;  //subSetIndex == 2 ? 1 : 2; // 1;
                        oUP_MDL.oChurchFaithType.Category = subSetIndex == 1 ? "FS" : "FC";

                        //if (setIndex > 0) oUP_MDL.oChurchFaithType.Category = setIndex == 1 ? "FS" : "FC";
                    }

                    else
                    {
                        oUP_MDL = (
                             from t_cft in _context.ChurchFaithType.AsNoTracking() //.Include(t => t.FaithTypeClass)
                                 .Where(x => x.Id == id)
                             from t_cft_c in _context.ChurchFaithType.AsNoTracking().Where(c => c.Id == t_cft.FaithTypeClassId).DefaultIfEmpty()
                             select new ChurchFaithTypeModel()
                             {
                                 oChurchFaithType = t_cft,
                                 strFaithTypeClass = t_cft_c != null ? t_cft_c.FaithDescription : ""
                             })
                             .FirstOrDefault();
                    }

                    if (oUP_MDL.oChurchFaithType == null) return null;

                    oUP_MDL.setIndex = setIndex;
                    oUP_MDL.subSetIndex = subSetIndex;
                    oUP_MDL.oUserId_Logged = oUserId_Logged;
                    oUP_MDL.oAppGloOwnId_Logged = oAGOId_Logged;
                    oUP_MDL.oChurchBodyId_Logged = oCBId_Logged;
                    //
                    oUP_MDL.oAppGloOwnId = oAppGloOwnId;
                    oUP_MDL.oChurchBodyId = oCurrChuBodyId;
                    var oCurrChuBody = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                    oUP_MDL.oChurchBody = oCurrChuBody != null ? oCurrChuBody : null;

                    if (oUP_MDL.subSetIndex == 2) // Denomination classes av church sects
                        oUP_MDL = this.popLookups_CFT(oUP_MDL, oUP_MDL.oChurchFaithType);

                    var _oUP_MDL = Newtonsoft.Json.JsonConvert.SerializeObject(oUP_MDL);
                    TempData["oVmCurrMod"] = _oUP_MDL; TempData.Keep();

                    //TempData["oVmCurr"] = oUP_MDL;
                    //TempData.Keep(); 

                    return PartialView("_AddOrEdit_UP", oUP_MDL);
                }

                //page not found error
                
                return PartialView("_ErrorPage");
            }
        }

        public ChurchFaithTypeModel popLookups_UP1(ChurchFaithTypeModel vm, ChurchFaithType oCurrUP)
        {
            if (vm != null)
            {
                vm.lkpFaithTypeClasses = _context.ChurchFaithType.AsNoTracking().Where(c => c.Id != oCurrUP.Id && c.Category == "FS" && !string.IsNullOrEmpty(c.FaithDescription))
                                              .OrderBy(c => c.FaithDescription).ToList()
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = c.FaithDescription
                                              })
                                              .ToList();

                vm.lkpFaithTypeClasses.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            }

            return vm;
        }


        [HttpGet]
        public IActionResult AddOrEdit_UP(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0,
                                                                           int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) //, int pageIndex = 1)
        {
            //// check permission 
            //var _oUserPrivilegeCol = oUserLogIn_Priv;
            //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
            //TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
            //
            var oUserModel = new UserProfileModel(); //TempData.Keep();
                                                     //  if (setIndex == 0) return oUserModel;

            //subSetIndex: 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
            // setIndex: 1-Vendor-any , 2-Client - CH/CF, 3- Client - any/all
            var proScope = setIndex == 1 ? "V" : "C";               // V - vendor, C - Client
            var subScope = setIndex == 2 ? "D" : setIndex == 3 ? "A" : "";      // D - denomination, A - all

            //if (subSetIndex >= 1 && subSetIndex <= 5) { proScope = "V"; subScope = ""; }
            //else if (subSetIndex == 6 || subSetIndex == 11) { proScope = "C"; subScope = "D"; }
            //else if (subSetIndex >= 6 && subSetIndex <= 15) { proScope = "C"; subScope = "A"; }

            var strDesc = "User Profile";
            var _userTask = "Attempted accessing/modifying " + strDesc.ToLower() ;
            if (id == 0)
            {   //create user and init... 

                //var existSUP_ADMNs = (
                //   from t_up in _context.UserProfile.AsNoTracking() //.Include(t => t.ChurchMember)
                //                .Where(c => c.Id == id &&
                //                (c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V") ||
                //                (c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C"))
                //   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId && c.AppGlobalOwnerId == t_up.AppGlobalOwnerId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                //   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                //                    .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                //                    ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                //                     ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                //                     ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                //                    )).DefaultIfEmpty()
                //   select t_up
                //   ).ToList();

                ////supadmin <creation> task.... but must have logged in as SYS
                //if (setIndex==1 && subSetIndex==2 && existSUP_ADMNs.Count > 0)
                //{ //prompt user sup_admin == 1 only
                //    oUserModel.oUserProfile = null;
                //    return oUserModel;
                //}

                var oUser = new UserProfile();
                oUser.AppGlobalOwnerId = oAppGloOwnId;
                oUser.ChurchBodyId = oCurrChuBodyId;
                oUser.Strt = DateTime.Now;
                oUser.ResetPwdOnNextLogOn = true;

                //oUP_MDL.oUserProfile.CountryId = oCurrCtryId;

                oUser.UserStatus = "A"; oUser.strUserStatus = "Active";  // A-ctive...D-eactive   
                oUser.ProfileScope = proScope;                 
                oUser.ProfileLevel = subSetIndex;  //remember to change the profile level when the ROLE type is changed              
                

                if (oUser.ProfileLevel >= 1 && oUser.ProfileLevel <= 5) // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 11-CF_ADMN
                {
                    oUser.UserScope = "E";  // I-internal, E-external
                    if (oUser.ProfileLevel == 2) { oUser.Username = "supadmin"; oUser.UserDesc = "Super Admin"; }
                }
                else // I-internal, E-external [manually config]
                {
                    if (oAppGloOwnId == null) // || oCurrChuBodyId == null)
                    { return PartialView("_ErrorPage"); }

                    var oAGO = _context.MSTRAppGlobalOwner.Find(oAppGloOwnId); 
                    if (oAGO == null )
                    { return PartialView("_ErrorPage"); }

                    oUserModel.strAppGlobalOwn = oAGO.OwnerName;

                    oUserModel.strChurchLevel = "Choose congregation";
                    var oCB = _context.MSTRChurchBody.AsNoTracking().Include(t=>t.ChurchLevel).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                    if (oCB  != null)
                    {
                        oUser.oCBChurchLevelId = oCB.ChurchLevelId;
                        oUserModel.strChurchBody = oCB.Name;
                        oUserModel.strChurchLevel = oCB.ChurchLevel != null ? (!string.IsNullOrEmpty(oCB.ChurchLevel.CustomName) ? oCB.ChurchLevel.CustomName : oCB.ChurchLevel.Name) : oUserModel.strChurchLevel;
                    }

                    oUser.UserScope = "I";

                    oUserModel.numCLIndex = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAppGloOwnId);  //use what's configured... not digit at AGO
                    oUser.numCLIndex = oUserModel.numCLIndex; // _context.ChurchLevel.Count(c => c.AppGlobalOwnerId == oAppGloOwnId);
                    
                }

                _userTask = "Attempted creating new " + strDesc.ToLower(); // + ", " + oUserModel.oUserProfile.UserDesc;  
                oUserModel.oUserProfile = oUser;

                //oUserModel.numUserRoleLevel = oUser.ProfileLevel;  // assume users have one core role [more privileged] and then add-on roles

            }

            else
            {
                var oUser = (
                   from t_up in _context.UserProfile.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ContactInfo) //.Include(t => t.ChurchMember)
                                .Where(c => c.Id == id &&
                                ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V") ||
                                (oAppGloOwnId != null && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C")))
                   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel)
                                .Where(c => c.Id == t_up.ChurchBodyId && c.AppGlobalOwnerId == t_up.AppGlobalOwnerId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                   from t_cl in _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.Id == t_cb.ChurchLevelId).DefaultIfEmpty()
                   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                 .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&
                                     (proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5) ||
                                     (proScope == "C" && subScope == "D" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                                     (proScope == "C" && subScope == "A" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15)
                                  ).DefaultIfEmpty()

                       //from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == t_up.ChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()
                       // from t_ur in _context.UserRole.AsNoTracking().Where(c => c.Id == t_upr.UserRoleId &&
                       //              ((c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5)) ||
                       //              ((c.RoleType == "CH_ADMN" || c.RoleType == "CF_ADMN") && (c.RoleLevel >= 6 && c.RoleLevel <= 10))).DefaultIfEmpty()
                       // from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                       //              .Where(c => c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                       // from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                       //              .Where(c => c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       // from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                       //              .Where(c => c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                   select new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwner = t_up.AppGlobalOwner,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_cb, // t_up.ChurchBody,
                       oUserRole = t_upr.UserRole,
                       strChurchCode_AGO = t_cb != null ? (t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.GlobalChurchCode : "") : "", 
                       strChurchCode_CB = t_cb != null ? t_cb.GlobalChurchCode : "",
                       // 
                       numCLIndex = t_cl.LevelIndex,
                       oCBChurchLevelId = t_cb.ChurchLevelId,
                       //  ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,
                       //
                       UserKey = t_up.UserKey,
                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc, 
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                       // PhoneNum = t_up.PhoneNum,
                       Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                       //
                       OwnerUserId = t_up.OwnerUserId,
                      // UserId = t_up.UserId,
                       UserScope = t_up.UserScope,
                       ProfileLevel = t_up.ProfileLevel,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)

                   }
                   ).FirstOrDefault();

                oUserModel.oUserProfile = oUser;
                if (oUser == null)
                {
                    
                    return PartialView("_ErrorPage");
                }

                    oUserModel.strUserProfile = oUser.UserDesc;
                    subSetIndex = oUser.ProfileLevel != null ? (int)oUser.ProfileLevel : 0;
                    oUserModel.strChurchBody = oUser.ChurchBody != null ? oUser.ChurchBody.Name : "";
                    oUserModel.strAppGlobalOwn = oUser.AppGlobalOwner != null ? oUser.AppGlobalOwner.OwnerName : "";
              //  oUserModel.numUserRoleLevel = oUser.ProfileLevel;  // assume users have one core role [more privileged] and then add-on roles


                if (oUser.ChurchBody!= null) 
                    oUserModel.strChurchLevel =  oUser.ChurchBody.ChurchLevel != null ? (!string.IsNullOrEmpty(oUser.ChurchBody.ChurchLevel.CustomName) ? oUser.ChurchBody.ChurchLevel.CustomName : oUser.ChurchBody.ChurchLevel.Name) : "Choose congregation";

                //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc
                _userTask = "Opened " + strDesc.ToLower() + ", " + oUserModel.oUserProfile.UserDesc;               
            }
                         
            oUserModel.setIndex = setIndex;
            oUserModel.subSetIndex = subSetIndex;
            oUserModel.profileScope = proScope;
            oUserModel.subScope = subScope;
            //
            oUserModel.oCurrUserId_Logged = oUserId_Logged;
            oUserModel.oAppGloOwnId_Logged = oAGOId_Logged;
            oUserModel.oChurchBodyId_Logged = oCBId_Logged;
            //
            oUserModel.oAppGloOwnId = oAppGloOwnId;
            oUserModel.oChurchBodyId = oCurrChuBodyId;
            


            //  oUserModel.oCurrUserId_Logged = oCurrUserId_Logged; 

            // ChurchBody oCB = null;
            // if (oCurrChuBodyId != null)  oCB = _context.MSTRChurchBody.Where(c=>c.Id == oCurrChuBodyId && c.AppGlobalOwnerId==oDenomId).FirstOrDefault();

            //if (setIndex == 1) // (subSetIndex >= 1 && subSetIndex <= 5) // no SUP_ADMN or SYS as option
            //    oUserModel = this.populateLookups_UP_MS(oUserModel, null, null, setIndex);

            //else if (setIndex == 2 || setIndex == 3) // ((oUserModel.profileScope == "V" && (subSetIndex == 6 || subSetIndex == 11)) || (oUserModel.profileScope == "C" && subSetIndex >= 6 && subSetIndex <= 15))
            //{
            //    // var oCB = _context.MSTRChurchBody.Where(c => c.Id == oCurrChuBodyId && c.AppGlobalOwnerId == oDenomId).FirstOrDefault();
            //    oUserModel = this.populateLookups_UP_CL(oUserModel, oCurrChuBodyId);
            //}


            //if (setIndex == 2 && (oUserModel.oUserProfile.ProfileLevel == 6 || oUserModel.oUserProfile.ProfileLevel == 11)) //CH admin, CF admin (oUserModel.oChurchBody != null)
            //{
            //    if (oUserModel.oAppGloOwnId != null)
            //    {
            //        var oUserCB = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.Id == oUserModel.oChurchBodyId).FirstOrDefault();
            //        ///
            //        oUserModel.oCBLevelCount = oUserModel.oUserProfile.numCLIndex; // - 1;        // oCBLevelCount -= 2;   
            //        List<MSTRChurchLevel> oCBLevelList = _context.MSTRChurchLevel.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.LevelIndex > 0 &&
            //                              c.LevelIndex <= oUserModel.oUserProfile.numCLIndex).ToList().OrderBy(c => c.LevelIndex).ToList();
            //        ///
            //        if (oUserModel.oCBLevelCount > 0 && oCBLevelList.Count > 0)
            //        {
            //            oUserModel.strChurchLevel_1 = !string.IsNullOrEmpty(oCBLevelList[0].CustomName) ? oCBLevelList[0].CustomName : oCBLevelList[0].Name;
            //            ViewData["strChurchLevel_1"] = oUserModel.strChurchLevel_1;
            //            ///

            //            if (oUserCB != null)
            //            {
            //                if (oUserModel.oCBLevelCount == 1 && oUserCB.ChurchLevelId == oCBLevelList[0].Id)  // at the last index [no parent]
            //                { 
            //                    oUserModel.ChurchBodyId_1 = oUserCB != null ? oUserCB.Id : (int?)null;
            //                    oUserModel.strChurchBody_1 = (oUserCB != null ? oUserCB.Name : "") + " (Church Root)";     // !string.IsNullOrEmpty(oUserModel.strAppGlobalOwn) ? oUserModel.strAppGlobalOwn + " (Church Root)" : "Choose congregation";
            //                    oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[0].Id;
            //                }
            //                else
            //                {
            //                    var oCB_1 = _context.MSTRChurchBody.Include(t => t.ChurchLevel)
            //                                  .Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && // c.Status == "A" && 
            //                                        c.ChurchLevel.LevelIndex == 1 && c.OrgType == "CR") //c.ChurchLevelId == oCBLevelList[0].Id &&
            //                                  .FirstOrDefault();

            //                    if (oCB_1 != null)
            //                    { oUserModel.ChurchBodyId_1 = oCB_1.Id; oUserModel.strChurchBody_1 = oCB_1.Name + " (Church Root)"; }
            //                } 
            //            }
            //            else
            //            {
            //                oUserCB = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.ChurchLevelId == oCBLevelList[0].Id).FirstOrDefault();

            //                oUserModel.ChurchBodyId_1 = (int?)null;
            //                oUserModel.strChurchBody_1 = (oUserCB != null ? oUserCB.Name : "") + " (Church Root)";
            //              //  ViewData["strChurchBody_1"] = "";
            //              //  ViewData["ChurchBodyId_1"] = 0;

            //                if (oUserModel.oCBLevelCount == 1) oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[0].Id;
            //            }
            //            ViewData["ChurchBodyId_1"] = oUserModel.ChurchBodyId_1;
            //            ViewData["strChurchBody_1"] = oUserModel.strChurchBody_1;

            //            ///
            //            if (oUserModel.oCBLevelCount > 1)
            //            {
            //                oUserModel.strChurchLevel_2 = !string.IsNullOrEmpty(oCBLevelList[1].CustomName) ? oCBLevelList[1].CustomName : oCBLevelList[1].Name;
            //                ViewData["strChurchLevel_2"] = oUserModel.strChurchLevel_2;
            //                ///                            
            //                if (oUserCB != null)
            //                {
            //                    if (oUserModel.oCBLevelCount == 2 && oUserCB.ChurchLevelId == oCBLevelList[1].Id)  // at the last index [no parent]
            //                    { oUserModel.ChurchBodyId_2 = oUserCB.Id; oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[1].Id; }
            //                    else
            //                    {
            //                        var lsCB_2 = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.ChurchLevelId == oCBLevelList[1].Id).ToList();
            //                        var oCB_2 = lsCB_2.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oUserCB.RootChurchCode, c.Id, oUserCB.ParentChurchBodyId)).ToList();
            //                        if (oCB_2.Count() > 0)
            //                        { oUserModel.ChurchBodyId_2 = oCB_2[0].Id; }
            //                    } 
            //                    ViewData["ChurchBodyId_2"] = oUserModel.ChurchBodyId_2;
            //                }
            //                else
            //                {
            //                    oUserModel.ChurchBodyId_2 = null;
            //                    ViewData["ChurchBodyId_2"] = 0;
            //                    if (oUserModel.oCBLevelCount == 2) oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[1].Id;
            //                }
                             
            //                if (oUserModel.oCBLevelCount > 2)
            //                {
            //                    oUserModel.strChurchLevel_3 = !string.IsNullOrEmpty(oCBLevelList[2].CustomName) ? oCBLevelList[2].CustomName : oCBLevelList[2].Name;
            //                    ViewData["strChurchLevel_3"] = oUserModel.strChurchLevel_3;
                                 
            //                    if (oUserCB != null)
            //                    {
            //                        if (oUserModel.oCBLevelCount == 3 && oUserCB.ChurchLevelId == oCBLevelList[2].Id)  // at the last index [no parent]
            //                        { oUserModel.ChurchBodyId_3 = oUserCB.Id; oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[2].Id; }
            //                        else
            //                        {
            //                            var lsCB_3 = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.ChurchLevelId == oCBLevelList[2].Id).ToList();
            //                            var oCB_3 = lsCB_3.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oUserCB.RootChurchCode, c.Id, oUserCB.ParentChurchBodyId)).ToList();
            //                            if (oCB_3.Count() > 0)
            //                            { oUserModel.ChurchBodyId_3 = oCB_3[0].Id; }
            //                        }                                    
            //                        ViewData["ChurchBodyId_3"] = oUserModel.ChurchBodyId_3;
            //                    }
            //                    else
            //                    {
            //                        oUserModel.ChurchBodyId_3 = null;
            //                        ViewData["ChurchBodyId_3"] = 0;
            //                        if (oUserModel.oCBLevelCount == 3) oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[2].Id;
            //                    }

            //                    if (oUserModel.oCBLevelCount > 3)
            //                    {
            //                        oUserModel.strChurchLevel_4 = !string.IsNullOrEmpty(oCBLevelList[3].CustomName) ? oCBLevelList[3].CustomName : oCBLevelList[3].Name;
            //                        ViewData["strChurchLevel_4"] = oUserModel.strChurchLevel_4;
                                     
            //                        if (oUserCB != null)
            //                        {
            //                            if (oUserModel.oCBLevelCount == 4 && oUserCB.ChurchLevelId == oCBLevelList[3].Id)  // at the last index [no parent]
            //                            { oUserModel.ChurchBodyId_4 = oUserCB.Id; oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[3].Id; }
            //                            else
            //                            {
            //                                var lsCB_4 = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.ChurchLevelId == oCBLevelList[3].Id).ToList();
            //                                var oCB_4 = lsCB_4.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oUserCB.RootChurchCode, c.Id, oUserCB.ParentChurchBodyId)).ToList();
            //                                if (oCB_4.Count() > 0)
            //                                { oUserModel.ChurchBodyId_4 = oCB_4[0].Id; } 
            //                            }                                        
            //                            ViewData["ChurchBodyId_4"] = oUserModel.ChurchBodyId_4;
            //                        }
            //                        else
            //                        {
            //                            oUserModel.ChurchBodyId_4 = null;
            //                            ViewData["ChurchBodyId_4"] = 0;
            //                            if (oUserModel.oCBLevelCount == 4) oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[3].Id;
            //                        }

            //                        if (oUserModel.oCBLevelCount > 4)
            //                        {
            //                            oUserModel.strChurchLevel_5 = !string.IsNullOrEmpty(oCBLevelList[4].CustomName) ? oCBLevelList[4].CustomName : oCBLevelList[4].Name;
            //                            ViewData["strChurchLevel_5"] = oUserModel.strChurchLevel_5;
                                         
            //                            if (oUserCB != null)
            //                            {
            //                                if (oUserModel.oCBLevelCount == 5 && oUserCB.ChurchLevelId == oCBLevelList[4].Id)  // at the last index [no parent]
            //                                { oUserModel.ChurchBodyId_5 = oUserCB.Id; oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[4].Id; }
            //                                else
            //                                {
            //                                    var lsCB_5 = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.ChurchLevelId == oCBLevelList[4].Id).ToList();
            //                                    var oCB_5 = lsCB_5.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oUserCB.RootChurchCode, c.Id, oUserCB.ParentChurchBodyId)).ToList();
            //                                    if (oCB_5.Count() > 0)
            //                                    { oUserModel.ChurchBodyId_5 = oCB_5[0].Id; }
            //                                } 
            //                                ViewData["ChurchBodyId_5"] = oUserModel.ChurchBodyId_5;
            //                            }
            //                            else
            //                            {
            //                                oUserModel.ChurchBodyId_5 = null;
            //                                ViewData["ChurchBodyId_5"] = 0;
            //                                if (oUserModel.oCBLevelCount == 5) oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[4].Id;
            //                            }
            //                        }

            //                        if (oUserModel.oCBLevelCount > 5)
            //                        {
            //                            oUserModel.strChurchLevel_6 = !string.IsNullOrEmpty(oCBLevelList[5].CustomName) ? oCBLevelList[5].CustomName : oCBLevelList[5].Name;
            //                            ViewData["strChurchLevel_6"] = oUserModel.strChurchLevel_6;

            //                            if (oUserCB != null)
            //                            {
            //                                if (oUserModel.oCBLevelCount == 6 && oUserCB.ChurchLevelId == oCBLevelList[5].Id)  // at the last index [no parent]
            //                                { oUserModel.ChurchBodyId_6 = oUserCB.Id; oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[5].Id; }
            //                                else
            //                                {
            //                                    var lsCB_6 = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oUserModel.oAppGloOwnId && c.ChurchLevelId == oCBLevelList[5].Id).ToList();
            //                                    var oCB_6 = lsCB_6.Where(c => IsAncestor_ChurchBody(c.RootChurchCode, oUserCB.RootChurchCode, c.Id, oUserCB.ParentChurchBodyId)).ToList();
            //                                    if (oCB_6.Count() > 0)
            //                                    { oUserModel.ChurchBodyId_6 = oCB_6[0].Id; }
            //                                }
            //                                ViewData["ChurchBodyId_6"] = oUserModel.ChurchBodyId_6;
            //                            }
            //                            else
            //                            {
            //                                oUserModel.ChurchBodyId_6 = null;
            //                                ViewData["ChurchBodyId_6"] = 0;
            //                                if (oUserModel.oCBLevelCount == 6) oUserModel.oUserProfile.oCBChurchLevelId = oCBLevelList[5].Id;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    } 
            //}

              
            oUserModel = this.populateLookups_UP_MS(oUserModel, setIndex,  oUserModel.oUserProfile.ChurchBody);
                              
            //oUP_MDL.lkpStatuses = new List<SelectListItem>();
            //foreach (var dl in dlGenStatuses) { oUP_MDL.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            //TempData["oVmCurr"] = oUserModel;
            //TempData.Keep();

            // var _oUserModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUserModel);
            // TempData["oVmCurr"] = _oUserModel; TempData.Keep();

            var _oUserModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUserModel);
            TempData["oVmCurrMod"] = _oUserModel; TempData.Keep();
            
            var tm = DateTime.Now;
            _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                             "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, oUserId_Logged, tm, tm, oUserId_Logged, oUserId_Logged));

            return PartialView("_AddOrEdit_UP", oUserModel);             
        }

        [HttpGet]
        public UserProfileModel AddOrEdit_UP_CL(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0,
                                                int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) //, int pageIndex = 1)
        {
            var oUserModel = new UserProfileModel(); TempData.Keep();
            if (setIndex == 0) return oUserModel;

            // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CL_ADMN , 5-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
            var proScope = setIndex == 1 ? "V" : "C";
            var subScope = setIndex == 2 ? "D" : setIndex == 3 ? "A" : "";

            //if (subSetIndex >= 1 && subSetIndex <= 5) { proScope = "V"; subScope = ""; }
            //else if (subSetIndex == 6 || subSetIndex == 11) { proScope = "C"; subScope = "D"; }
            //else if (subSetIndex >= 6 && subSetIndex <= 15) { proScope = "C"; subScope = "A"; }

            if (id == 0)
            {   //create user and init... 
                //var existSUP_ADMNs = (
                //   from t_up in _context.UserProfile.AsNoTracking() //.Include(t => t.ChurchMember)
                //                .Where(c => c.Id == id &&
                //                (c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V") ||
                //                (c.AppGlobalOwnerId == oDenomId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C"))
                //   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId && c.AppGlobalOwnerId == t_up.AppGlobalOwnerId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                //   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                //                    .Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id &&
                //                    ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                //                     ((proScope == "C" && subScope == "D" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                //                     ((proScope == "C" && subScope == "A" && c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))
                //                    )).DefaultIfEmpty()
                //   select t_up
                //   ).ToList();

                ////supadmin <creation> task.... but must have logged in as SYS
                //if (setIndex==1 && subSetIndex==2 && existSUP_ADMNs.Count > 0)
                //{ //prompt user sup_admin == 1 only
                //    oUserModel.oUserProfile = null;
                //    return oUserModel;
                //}

                var oUser = new UserProfile();
                oUser.ChurchBodyId = oCurrChuBodyId;
                oUser.Strt = DateTime.Now;
                oUser.ResetPwdOnNextLogOn = true;

                //oUP_MDL.oUserProfile.CountryId = oCurrCtryId;

                oUser.UserStatus = "A";   // A-ctive...D-eactive   
                oUser.ProfileScope = proScope;

                if (subSetIndex >= 1 && subSetIndex <= 5) // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
                {
                    oUser.UserScope = "E";  // I-internal, E-external
                    if (subSetIndex == 2) { oUser.Username = "supadmin"; oUser.UserDesc = "Super Admin"; }
                }
                else // I-internal, E-external [manually config]
                { oUser.UserScope = "I"; }

                oUserModel.oUserProfile = oUser;
            }

            else
            {
                var oUser = (
                   from t_up in _context.UserProfile.Include(t => t.ContactInfo).AsNoTracking() //.Include(t => t.ChurchMember)
                                .Where(c => c.Id == id &&
                                (c.AppGlobalOwnerId == null && c.ChurchBodyId == null) ||  //&& c.ProfileScope == "V"
                                (oAppGloOwnId != null && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == "C"))
                   from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_up.ChurchBodyId && c.AppGlobalOwnerId == t_up.AppGlobalOwnerId).DefaultIfEmpty()  //c.Id == oChurchBodyId && 
                   from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                 .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&
                                     (proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CL_ADMN" || c.UserRole.RoleType == "SYS_CUST") && c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5) ||
                                     (proScope == "C" && subScope == "D" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN") && (c.UserRole.RoleLevel == 6 || c.UserRole.RoleLevel == 11)) ||
                                     (proScope == "C" && subScope == "A" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15)
                                  ).DefaultIfEmpty()

                       //// from t_upr in _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole).Where(c => c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       // from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.Id == t_up.ChurchBodyId && c.Id == t_up.ChurchMemberId).DefaultIfEmpty()
                       // from t_ur in _context.UserRole.AsNoTracking().Where(c => c.Id == t_upr.UserRoleId &&
                       //              ((c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5)) ||
                       //              ((c.RoleType == "CH_ADMN" || c.RoleType == "CF_ADMN") && (c.RoleLevel >= 6 && c.RoleLevel <= 10))).DefaultIfEmpty()
                       // from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                       //              .Where(c => c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()
                       // from t_upg in _context.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                       //              .Where(c => c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       // from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                       //              .Where(c => c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                   select new UserProfile()
                   {
                       Id = t_up.Id,
                       AppGlobalOwnerId = t_up.AppGlobalOwnerId,
                       ChurchBodyId = t_up.ChurchBodyId,
                       ChurchMemberId = t_up.ChurchMemberId,
                       ChurchBody = t_up.ChurchBody,
                       //  ChurchMember = t_up.ChurchMember,
                       OwnerUser = t_up.OwnerUser,
                       //
                       Username = t_up.Username,
                       UserDesc = t_up.UserDesc,
                       Email = t_up.Email,
                       ContactInfo = t_up.ContactInfo,
                       // PhoneNum = t_up.PhoneNum,
                       Pwd = t_up.Pwd,
                       PwdExpr = t_up.PwdExpr,
                       PwdSecurityQue = t_up.PwdSecurityQue,
                       PwdSecurityAns = t_up.PwdSecurityAns,
                       ResetPwdOnNextLogOn = t_up.ResetPwdOnNextLogOn,
                       Strt = t_up.Strt,
                       strStrt = t_up.strStrt,
                       Expr = t_up.Expr,
                       strExpr = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                       //
                       OwnerUserId = t_up.OwnerUserId,
                      // UserId = t_up.UserId,
                       UserScope = t_up.UserScope,
                       UserPhoto = t_up.UserPhoto,
                       ProfileScope = t_up.ProfileScope,
                       Created = t_up.Created,
                       CreatedByUserId = t_up.CreatedByUserId,
                       LastMod = t_up.LastMod,
                       LastModByUserId = t_up.LastModByUserId,
                       UserStatus = t_up.UserStatus,
                       strUserStatus = GetStatusDesc(t_up.UserStatus)
                   }
                   ).FirstOrDefault();

                oUserModel.oUserProfile = oUser;
                if (oUser != null)
                {
                    oUserModel.strUserProfile = oUser.UserDesc;
                    oUserModel.strChurchBody = oUser.ChurchBody != null ? oUser.ChurchBody.Name : "";
                    oUserModel.strAppGlobalOwn = oUser.AppGlobalOwner != null ? oUser.AppGlobalOwner.OwnerName : "";

                    //  strChurchMember = t_cb.AppGlobalOwner != null ? t_cb.AppGlobalOwner.OwnerName : "",
                    // strUserProfile = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc

                }
            }

            //if (oUserModel.oUserProfile != null)
            //{
            //    if (oUserModel.oUserProfile.AppGlobalOwnerId != null)
            //    {
            //        List<MSTRChurchLevel> oCBLevels = _context.MSTRChurchLevel
            //            .Where(c => c.AppGlobalOwnerId == oUserModel.oUserProfile.AppGlobalOwnerId).ToList().OrderBy(c => c.LevelIndex).ToList();

            //        if (oCBLevels.Count() > 0)
            //        {
            //            ViewBag.Filter_ln = !string.IsNullOrEmpty(oCBLevels[oCBLevels.Count - 1].CustomName) ? oCBLevels[oCBLevels.Count - 1].CustomName : oCBLevels[6].Name;
            //            ViewBag.Filter_1 = !string.IsNullOrEmpty(oCBLevels[0].CustomName) ? oCBLevels[0].CustomName : oCBLevels[0].Name;

            //            if (oCBLevels.Count() > 1)
            //            {
            //                ViewBag.Filter_2 = ViewBag.Filter_2 = !string.IsNullOrEmpty(oCBLevels[1].CustomName) ? oCBLevels[1].CustomName : oCBLevels[1].Name;
            //                if (oCBLevels.Count() > 2)
            //                {
            //                    ViewBag.Filter_3 = ViewBag.Filter_3 = !string.IsNullOrEmpty(oCBLevels[2].CustomName) ? oCBLevels[2].CustomName : oCBLevels[2].Name;
            //                    if (oCBLevels.Count() > 3)
            //                    {
            //                        ViewBag.Filter_4 = ViewBag.Filter_4 = !string.IsNullOrEmpty(oCBLevels[3].CustomName) ? oCBLevels[3].CustomName : oCBLevels[3].Name;
            //                        if (oCBLevels.Count() > 4)
            //                        {
            //                            ViewBag.Filter_5 = ViewBag.Filter_5 = !string.IsNullOrEmpty(oCBLevels[4].CustomName) ? oCBLevels[43].CustomName : oCBLevels[4].Name;
            //                            if (oCBLevels.Count() > 5)
            //                            {
            //                                ViewBag.Filter_6 = ViewBag.Filter_6 = !string.IsNullOrEmpty(oCBLevels[5].CustomName) ? oCBLevels[5].CustomName : oCBLevels[5].Name;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            oUserModel.setIndex = setIndex;
            oUserModel.subSetIndex = subSetIndex;
            oUserModel.oCurrUserId_Logged = oUserId_Logged;
            oUserModel.oAppGloOwnId_Logged = oAGOId_Logged;
            oUserModel.oChurchBodyId_Logged = oCBId_Logged;
            //
            oUserModel.oAppGloOwnId = oAppGloOwnId;
            oUserModel.oChurchBodyId = oCurrChuBodyId;
            //  oUserModel.oCurrUserId_Logged = oCurrUserId_Logged; 

            // ChurchBody oCB = null;
            // if (oCurrChuBodyId != null)  oCB = _context.MSTRChurchBody.Where(c=>c.Id == oCurrChuBodyId && c.AppGlobalOwnerId==oDenomId).FirstOrDefault();

            //if (setIndex == 1) // (subSetIndex >= 1 && subSetIndex <= 5) // no SUP_ADMN or SYS as option
            //    oUserModel = this.populateLookups_UP_MS(oUserModel, null, null, setIndex);

            //else if (setIndex == 2 || setIndex == 3) // ((oUserModel.profileScope == "V" && (subSetIndex == 6 || subSetIndex == 11)) || (oUserModel.profileScope == "C" && subSetIndex >= 6 && subSetIndex <= 15))
            //{
            //    // var oCB = _context.MSTRChurchBody.Where(c => c.Id == oCurrChuBodyId && c.AppGlobalOwnerId == oDenomId).FirstOrDefault();
            //    oUserModel = this.populateLookups_UP_CL(oUserModel, oCurrChuBodyId);
            //}

            oUserModel = this.populateLookups_UP_CL(oUserModel, oAppGloOwnId, oCurrChuBodyId);

            //oUP_MDL.lkpStatuses = new List<SelectListItem>();
            //foreach (var dl in dlGenStatuses) { oUP_MDL.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            //TempData["oVmCurr"] = oUserModel;
            //TempData.Keep();

            // var _oUserModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUserModel);
            // TempData["oVmCurr"] = _oUserModel; TempData.Keep();


            var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(oUserModel);
            TempData["oVmCurrMod"] = _vmMod; TempData.Keep();

            return oUserModel;
        }
        public JsonResult GetChurchBodyLevelsByAppGloOwn(int? oAppGloOwnId, bool addEmpty = false)
        {
            var oCBList = new List<SelectListItem>();
            ///
            oCBList = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId)
                                              .OrderByDescending(c => c.LevelIndex).ToList()

                   .Select(c => new SelectListItem()
                   {
                       Value = c.Id.ToString(),
                       Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name
                   })
                   //.OrderBy(c => c.Text)
                   .ToList();
            ///
            if (addEmpty) oCBList.Insert(0, new SelectListItem { Value = "", Text = "Select Church level" });
            return Json(oCBList);
        }
        private UserProfileModel populateLookups_UP_CL(UserProfileModel vmLkp, int? AppGloOwnId, int? oChurchBodyId)  //AppGloOwnId   ChurchBody oCurrChuBody)
        {
            if (vmLkp == null || oChurchBodyId == null) return vmLkp;
            //
            vmLkp.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vmLkp.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkpUserRoles = _context.UserRole.AsNoTracking().Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || (c.AppGlobalOwnerId == AppGloOwnId &&  c.ChurchBodyId == oChurchBodyId)) && 
                                                                c.RoleStatus == "A" && c.RoleLevel >= 6 && c.RoleLevel <= 15)
                               .OrderBy(c => c.RoleLevel)
                               .Select(c => new SelectListItem()
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.RoleName.Trim()
                               })
                               // .OrderBy(c => c.Text)
                               .ToList();
            //  vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vmLkp.lkpUserGroups = _context.UserGroup.AsNoTracking().Where(c => c.Status == "A" && c.ChurchBodyId == oChurchBodyId)
                               .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
                               .Select(c => new SelectListItem()
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.GroupName.Trim()
                               })
                               // .OrderBy(c => c.Text)
                               .ToList();


            vmLkp.lkpPwdSecQueList = _context.AppUtilityNVP.AsNoTracking().Where(c => c.NvpCode == "PWD_SEC_QUE")
                      .OrderBy(c => c.RequireUserCustom).ThenBy(c => c.OrderIndex).ThenBy(c => c.NvpValue)
                      .ToList()
                      .Select(c => new SelectListItem()
                      {
                          Value = c.Id.ToString(),
                          Text = c.NvpValue
                      })
                      // .OrderBy(c => c.Text)
                      .ToList();

            vmLkp.lkpPwdSecQueList.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            //vmLkp.lkpPwdSecAnsList = _context.AppUtilityNVP.Where(c => c.NvpCode == "PWD_SEC_ANS")
            //         .OrderBy(c => c.RequireUserCustom).ThenBy(c => c.OrderIndex).ThenBy(c => c.NvpValue)
            //         .ToList()
            //         .Select(c => new SelectListItem()
            //         {
            //             Value = c.Id.ToString(),
            //             Text = c.NvpValue
            //         })
            //         // .OrderBy(c => c.Text)
            //         .ToList();
            //vmLkp.lkpPwdSecAnsList.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            return vmLkp;
        }

        private UserProfileModel populateLookups_UP_MS(UserProfileModel vmLkp, int setIndex, MSTRChurchBody oChurchBody = null)
        {
            if (vmLkp == null ) return vmLkp;
            //if (vmLkp.oAppGloOwnId == null ) return vmLkp;

            // 
            vmLkp.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vmLkp.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkpUserProfileLevels = new List<SelectListItem>();
            foreach (var dl in dlUserLevels) { 
                if (vmLkp.profileScope == "V")
                {
                    if (dl.Val == "1" || dl.Val == "2" || dl.Val == "3" || dl.Val == "4")  // SYS   - 1                 
                        vmLkp.lkpUserProfileLevels.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc, Disabled = dl.Val == "1" });                     
                }
                else if (vmLkp.profileScope == "C") //
                {
                    if (dl.Val == "6" || dl.Val == "11")                    
                        vmLkp.lkpUserProfileLevels.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });                     
                }                
            }



            ////vmLkp.lkpUserRoles = _context.UserRole.Where(c => AppGloOwnId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 0 && c.RoleType=="SUP_ADMN" )
            //if (setIndex == 1 ) //>= 2 && setIndex <= 3)  //   SYS .. 1-SUP_ADMN, 2-SYS_ADMN, 3-SYS_CUST
            //{
            //    //vmLkp.lkpUserRoles = _context.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel > 2 && c.RoleLevel <= 5)
            //    //    .OrderBy(c => c.RoleLevel)
            //    //    .Select(c => new SelectListItem()
            //    //    {
            //    //        Value = c.Id.ToString(),
            //    //        Text = c.RoleName.Trim()
            //    //    })
            //    //    // .OrderBy(c => c.Text)
            //    //    .ToList();
            //    ////  vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            //    //vmLkp.lkpUserGroups = _context.UserGroup.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A")
            //    //                   .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
            //    //                   .Select(c => new SelectListItem()
            //    //                   {
            //    //                       Value = c.Id.ToString(),
            //    //                       Text = c.GroupName.Trim()
            //    //                   })
            //    //                   // .OrderBy(c => c.Text)
            //    //                   .ToList();
            //}

            //else 

            if (setIndex == 2 || setIndex == 3) // == 6 || subSetIndex == 7)  //  6-CH_ADMN, 7-CF_ADMN
            {
                vmLkp.lkpAppGlobalOwns = _context.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Status == "A")
                                             .OrderBy(c => c.OwnerName).ToList()
                                             .Select(c => new SelectListItem()
                                             {
                                                 Value = c.Id.ToString(),
                                                 Text = c.OwnerName
                                             })
                                             .ToList();


                //if (oChurchBody != null)
                //{
                    vmLkp.lkpChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == vmLkp.oAppGloOwnId) //oChurchBody.AppGlobalOwnerId)
                                              .OrderByDescending(c => c.LevelIndex)
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name
                                              })
                                              .ToList();

                    //vm.lkpChurchLevels.Insert(0, new SelectListItem { Value = "", Text = "Select" });
                //}


                //if (setIndex == 2)
                //{
                //    vmLkp.lkpUserRoles = _context.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null &&
                //                                        c.RoleStatus == "A" && (c.RoleLevel == 6 || c.RoleLevel == 11))
                //        .OrderBy(c => c.RoleLevel)
                //        .Select(c => new SelectListItem()
                //        {
                //            Value = c.Id.ToString(),
                //            Text = c.RoleName.Trim()
                //        })
                //        // .OrderBy(c => c.Text)
                //        .ToList(); 
                //}

                //else if (setIndex == 3)
                //{
                //    vmLkp.lkpUserRoles = _context.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null &&
                //                                        c.RoleStatus == "A" && (c.RoleLevel >= 6 && c.RoleLevel <= 15))
                //        .OrderBy(c => c.RoleLevel)
                //        .Select(c => new SelectListItem()
                //        {
                //            Value = c.Id.ToString(),
                //            Text = c.RoleName.Trim()
                //        })
                //        // .OrderBy(c => c.Text)
                //        .ToList(); 
                //}

                //vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

                ////
                //vmLkp.lkpUserGroups = _context.UserGroup.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A")
                //                   .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
                //                   .Select(c => new SelectListItem()
                //                   {
                //                       Value = c.Id.ToString(),
                //                       Text = c.GroupName.Trim()
                //                   })
                //                   // .OrderBy(c => c.Text)
                //                   .ToList();
            }

            return vmLkp;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_UP(UserProfileModel vm)
        {
            UserProfile _oChanges = vm.oUserProfile;
            //   vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as UserProfileModel : vmMod; TempData.Keep();

            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileModel>(arrData) : vm;

            var oUP = vmMod.oUserProfile;  // oUP.ChurchBody = vmMod.oChurchBody;
            
            // confirm client admin
            if (!(_oChanges.AppGlobalOwnerId == null && _oChanges.ChurchBodyId == null && _oChanges.ProfileScope == "V"))
            { //client admins
              //if (string.IsNullOrEmpty(_oChanges.strChurchCode_CB))
              //{
              // var oAGO = oUP.AppGlobalOwner; var oCB = oUP.ChurchBody;

               if (_oChanges.AppGlobalOwnerId == null || _oChanges.ChurchBodyId == null)
                   return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specified denomination and church body for the user profile.", signOutToLogIn = false });

               if (oUP.AppGlobalOwner == null || oUP.ChurchBody == null)
               {
                    oUP.AppGlobalOwner = _context.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
                    oUP.ChurchBody = _context.MSTRChurchBody.AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == _oChanges.ChurchBodyId).FirstOrDefault();                        
               }

               if (oUP.AppGlobalOwner == null || oUP.ChurchBody == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specified denomination and church body could not be retrieved. Please refresh and try again.", signOutToLogIn = false });
                   
               if (string.IsNullOrEmpty(oUP.ChurchBody.GlobalChurchCode))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code not specified. The unique global church code of church unit is required. Please verify with System Admin and try again.", signOutToLogIn = false });


                if (oUP.ProfileLevel == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select the initial user role.", signOutToLogIn = false });

                if ((oUP.ChurchBody.OrgType=="CR" || oUP.ChurchBody.OrgType == "CH") && oUP.ProfileLevel != 6)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Congregration head-unit cannot have role type specified. Change the church body or user role type. Hint: 'Church administrator' applies to Congregration head-units and 'Congregation administrator' applies to congregations.", signOutToLogIn = false });

                if ((oUP.ChurchBody.OrgType == "CN") && oUP.ProfileLevel != 11)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Congregration cannot have role type specified. Change the church body or user role type. Hint: 'Church administrator' applies to Congregration head-units and 'Congregation administrator' applies to congregations.", signOutToLogIn = false });


                //var clientUPRList = _context.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                //                     .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                //                           (((oUP.ChurchBody.OrgType == "CR" || oUP.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel == 6 ) ||  // && c.UserRole.RoleLevel <= 10
                //                            (oUP.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel == 11 ))).ToList();  // && c.UserRole.RoleLevel <= 15


                //if (clientUPRList.Count > 1 && _oChanges.Id > 0)
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one (1) default '" + (clientUPRList[0].UserRole != null ? clientUPRList[0].UserRole.RoleName : "[Administrator]" ) +  "' permitted. " + clientUPRList.Count + " Admin users currently created for specified client. Please correct and try again. Hint: additional users can be created at the client side by client administrator", signOutToLogIn = false });
               
                //else if (clientUPRList.Count > 0 && _oChanges.Id == 0)
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one (1) default '" + (clientUPRList[0].UserRole != null ? clientUPRList[0].UserRole.RoleName : "[Administrator]") + "' permitted. Hint: additional users can be created at the client side by client administrator", signOutToLogIn = false });



                /// ONLY CLIENT SYS ASSIST AND SUPADMIN ROLES CAN CREATE ADDITIONAL UPR @ client
                /// SYS ADMIN CAN ONLY CREATE ADMIN ROLES @CLIENT

                //var adminCount = _context.UserProfile.AsNoTracking().Count(c =>
                //                                 c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                //                                 (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.ProfileLevel == 6) ||
                //                                  (c.ChurchBody.OrgType == "CN" && c.ProfileLevel == 11))); //(c.ProfileLevel == 6 || c.ProfileLevel == 11)   .ToList();

                //if ((_oChanges.Id == 0 && adminCount > 0) || (_oChanges.Id > 0 && adminCount > 1))
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one Administrator Profile allowed. Target client (" + oUP.ChurchBody.Name + ") has " + adminCount + " admin profile(s). Hint:- Additional user profiles can be created at the client side.", signOutToLogIn = false });


                var adminCount = _context.UserProfileRole.AsNoTracking().Count(c =>
                                                 c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                                                 (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel == 6) ||
                                                  (c.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel == 11))); //(c.ProfileLevel == 6 || c.ProfileLevel == 11)   .ToList();

                if ((_oChanges.Id == 0 && adminCount > 0) || (_oChanges.Id > 0 && adminCount > 1))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one Administrator Profile allowed. Target client (" + oUP.ChurchBody.Name + ") has " + adminCount + " admin profile(s). Hint:- Additional user profiles can be created at the client side.", signOutToLogIn = false });


                //oUP.AppGlobalOwner = oAGO;
                //oUP.ChurchBody = oCB;
                //  } 
            }


            try
            {
                ModelState.Remove("oUserProfile.AppGlobalOwnerId");
                ModelState.Remove("oUserProfile.ChurchBodyId");
                ModelState.Remove("oUserProfile.ChurchMemberId");
                ModelState.Remove("oUserProfile.CreatedByUserId");
                ModelState.Remove("oUserProfile.LastModByUserId");
                ModelState.Remove("oUserProfile.OwnerId");
                ModelState.Remove("oUserProfile.UserId");

                // ChurchBody == null 


                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", signOutToLogIn = false });

                if (string.IsNullOrEmpty(_oChanges.Username)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide username", signOutToLogIn = false });
                }



                // trim leading /trailing spaces... 
                if (!string.IsNullOrEmpty(_oChanges.Username)) _oChanges.Username = _oChanges.Username.Trim();  


                //if (_oChanges.PwdSecurityQue != null && string.IsNullOrEmpty(_oChanges.PwdSecurityAns))  //Congregant... ChurcCodes required
                //{
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the response to the security question specified.", signOutToLogIn = false });
                //}


                //confirm this is SYS acc   //check for the SYS acc
                //string strPwdHashedData = AppUtilties.ComputeSha256Hash(_oChanges.ChurchCode + _oChanges.Username.Trim().ToLower() + _oChanges.Password);
                //string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(vm.ChurchCode + _oChanges.Username.Trim().ToLower());
                // c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == strUserKeyHashedData && c.Pwd == strPwdHashedData


                // get data of UP
                var currLogUserInfo = (from t_up in _context.UserProfile.AsNoTracking().Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || (c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId)) && c.Id == vm.oCurrUserId_Logged)
                                       from t_upr in _context.UserProfileRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                       from t_ur in _context.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == t_upr.UserRoleId && c.RoleStatus == "A") // && c.RoleLevel == 1 && c.RoleType == "SYS")
                                       select new
                                       {
                                           UserId = t_up.Id,
                                           UserRoleId = t_ur.Id,
                                           UserType = t_ur.RoleType,
                                           UserRoleLevel = t_ur.RoleLevel,
                                           UserStatus = t_up.strUserStatus == "A" && t_upr.ProfileRoleStatus == "A" && t_ur.RoleStatus == "A"
                                       }
                                 ).FirstOrDefault();

                if (currLogUserInfo == null)
                { return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user not found! Please refresh and try again.", signOutToLogIn = false }); }


                //if (currLogUserInfo.UserType != "SYS" && string.IsNullOrEmpty(_oChanges.Email)) 
                //{
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Email is required to create user account. Hint: Email helps with 2-way authentication of user", signOutToLogIn = false });
                //}


                if (_oChanges.ProfileScope == "V")  //vendor admins ... SYS, SUP_ADMN, SYS_ADMN etc.
                {
                    if (currLogUserInfo.UserType == "SYS" && string.Compare(_oChanges.Username.Trim(), "supadmin", true) != 0 && string.Compare(_oChanges.Username.Trim(), "sys", true) != 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account can ONLY manage the [sys] or [supadmin] profile. Hint: Sign in with [supadmin] or other Admin account.", signOutToLogIn = false });
                    }

                    if (currLogUserInfo.UserType == "SUP_ADMN" && string.Compare(_oChanges.Username.Trim(), "SYS", true) == 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Logged user does not have enough role or permission to manage SYS account.", signOutToLogIn = false });
                    }

                    if (currLogUserInfo.UserType != "SYS" && currLogUserInfo.UserType != "SUP_ADMN" && 
                        string.Compare(_oChanges.Username.Trim(), "supadmin", true) == 0) // currLogUserInfo.UserType != "SUP_ADMN" && 
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Logged user does not have enough role or permission to manage SUP_ADMN account.", signOutToLogIn = false });
                    }


                    //if (_oChanges.Username == "SYS" && string.Compare(_oChanges.Username, "supadmin", true) != 0 && string.Compare(_oChanges.Username, "sys", true) != 0)
                    //{
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account can ONLY manage the [sys] or [supadmin] profile. Hint: Sign in with [supadmin] or other Admin account.", signOutToLogIn = false });
                    //}

                    //if (currLogUserInfo.UserType == "SUP_ADMN" && string.Compare(_oChanges.Username, "SYS", true) == 0)
                    //{
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user does not have SYS role. SYS role required to manage SYS account.", signOutToLogIn = false });
                    //}

                    //if (currLogUserInfo.UserType != "SYS" && string.Compare(_oChanges.Username, "supadmin", true) == 0) // currLogUserInfo.UserType != "SUP_ADMN" && 
                    //{
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user does not have SYS role. SYS role required to manage SUP_ADMN account.", signOutToLogIn = false });
                    //}



                    if (_oChanges.Id == 0)
                    {
                        if (string.Compare(_oChanges.Username.Trim(), "sys", true) == 0)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                                  from ur in _context.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SYS")
                                                  select upr
                                     );
                            if (existUserRoles.Count() > 0)
                            {
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account cannot be created. Only one (1) SYS role allowed.", signOutToLogIn = false });
                            }
                        }

                        if (string.Compare(_oChanges.Username.Trim(), "supadmin", true) == 0)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                                                  from ur in _context.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                                                  select upr
                                     );
                            if (existUserRoles.Count() > 0)
                            {
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Super Admin profile could not be created. Only one (1) SUP_ADMN role allowed.", signOutToLogIn = false });
                            }
                        }
                    }
                     


                }

                else  //CLIENT ADMINs ... creating users for their churches /congregations
                {

                    // check if client database has been created or can connect... thus if task is to create/manage a client admin profile
                    // 1 - vendor admins, 2 - client admins, 3 - client users
                    ///
                    if (vm.setIndex == 2 || vm.setIndex == 3)
                    {
                        // Get the client database details.... db connection string                        
                        var oClientConfig = _context.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                        if (oClientConfig == null)
                        {
                            ViewData["strUserLoginFailMess"] = "Client database details not found. Please try again or contact System Admin";
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewData["strUserLoginFailMess"], signOutToLogIn = false });
                        }

                        //// get and mod the conn
                        //var _clientDBConnString = "";
                        //var cs = _context.Database.GetDbConnection().ConnectionString;
                        //var _cs = this._configuration["ConnectionStrings:DefaultConnection"];
                        //var conn = new SqlConnectionStringBuilder(_cs);
                        //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
                        ///// conn.IntegratedSecurity = false; 
                        //conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                        //_clientDBConnString = conn.ConnectionString;

                        //// test the NEW DB conn
                        //var _clientContext = new ChurchModelContext(_clientDBConnString);

                        var _clientContext = AppUtilties.GetNewDBCtxConn_CL(_context, _configuration, _oChanges.AppGlobalOwnerId);
                        if (!_clientContext.Database.CanConnect())  // give appropriate user prompts
                        {
                            ViewData["strUserLoginFailMess"] = "Failed to connect client database. Please try again or contact System Admin";
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewData["strUserLoginFailMess"], signOutToLogIn = false });
                        }
                    }

                    /// ONLY CLIENT SYS ASSIST AND SUPADMIN ROLES CAN CREATE ADDITIONAL UPR @ client
                    /// SYS ADMIN CAN ONLY CREATE ADMIN ROLES @CLIENT

                    var adminCount = _context.UserProfileRole.AsNoTracking().Count(c =>
                                                 c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                                                 (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel == 6) ||
                                                  (c.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel == 11))); //(c.ProfileLevel == 6 || c.ProfileLevel == 11)   .ToList();

                    if ((_oChanges.Id == 0 && adminCount > 0) || (_oChanges.Id > 0 && adminCount > 1))
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one Administrator Profile allowed. Target client (" + oUP.ChurchBody.Name + ") has " + adminCount + " admin profile(s). Hint:- Additional user profiles can be created at the client side.", signOutToLogIn = false });



                    //var adminCount = _context.UserProfile.AsNoTracking().Count(c =>
                    //                                 c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                    //                                 (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.ProfileLevel == 6) ||
                    //                                 (c.ChurchBody.OrgType == "CN" && c.ProfileLevel == 11))); //(c.ProfileLevel == 6 || c.ProfileLevel == 11)   .ToList();

                    //if ((_oChanges.Id==0 && adminCount > 0) || (_oChanges.Id > 0 && adminCount > 1))
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one Administrator Profile allowed. Target client (" + oUP.ChurchBody.Name + ") has " + adminCount + " admin profile(s). Hint:- Additional user profiles can be created at the client side.", signOutToLogIn = false });





                    //if (_oChanges.AppGlobalOwnerId == null)
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the denomination/church.", signOutToLogIn = false });

                    //if (_oChanges.oCBChurchLevelId == null)
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the church level for the user profile.", signOutToLogIn = false });

                    //var oCBLevel = _context.MSTRChurchLevel.Find(_oChanges.oCBChurchLevelId);
                    //if (oCBLevel == null)  // ... parent church level > church unit level
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church unit level could not be found. Please refresh and try again", signOutToLogIn = false });




                    /// get the parent id
                    /// 
                    //var parDesc = "church unit";
                    //switch (vm.oCBLevelCount)
                    //{
                    //    case 1: _oChanges.ChurchBodyId = vm.ChurchBodyId_1; parDesc = vm.strChurchLevel_1; break;
                    //    case 2: _oChanges.ChurchBodyId = vm.ChurchBodyId_2; parDesc = vm.strChurchLevel_2; break;
                    //    case 3: _oChanges.ChurchBodyId = vm.ChurchBodyId_3; parDesc = vm.strChurchLevel_3; break;
                    //    case 4: _oChanges.ChurchBodyId = vm.ChurchBodyId_4; parDesc = vm.strChurchLevel_4; break;
                    //    case 5: _oChanges.ChurchBodyId = vm.ChurchBodyId_5; parDesc = vm.strChurchLevel_5; break;
                    //}


                    //check availability of username... SYS /SUP_ADMN reserved
                    if (string.Compare(_oChanges.Username.Trim(), "SYS", true) == 0 || string.Compare(_oChanges.Username.Trim(), "supadmin", true) == 0) 
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Username used is [System account]. Try different username.", signOutToLogIn = false });
                    

                    //// Denomination and ChurchBody cannot be null
                    //if (_oChanges.AppGlobalOwnerId==null || _oChanges.ChurchBodyId==null)
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the denomination and church unit", signOutToLogIn = false });

                    //// check if CB level == CL selected

                    //var oUP_CB = _context.MSTRChurchBody.AsNoTracking().Include(t=>t.ChurchLevel)
                    //    .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == _oChanges.ChurchBodyId).FirstOrDefault();
                    //if (oUP_CB == null)
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church body (subscribed congregation) selected could not be retrieved. Refresh and try again.", signOutToLogIn = false });

                    //if (oUP_CB.ChurchLevelId != _oChanges.oCBChurchLevelId)
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Target church level" +
                    //        (!string.IsNullOrEmpty(vm.strChurchLevel) ? " - " + vm.strChurchLevel : "")
                    //        + " is different from church level of the church body (subscribed congregation) specified" + 
                    //        (oUP_CB != null ? " - " + (!string.IsNullOrEmpty(oUP_CB.ChurchLevel.CustomName) ? oUP_CB.ChurchLevel.CustomName : oUP_CB.ChurchLevel.Name) : "")
                    //        + ". Specify correct congregation else change the church level", signOutToLogIn = false });                     
                    //    
                
                    
                }


                //check that username is unique in all instances
                var existUserProfiles = _context.UserProfile.AsNoTracking().Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || 
                                                                         (c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId)) && //  //... restrict within denomination as dbase is per denomination /cb
                                                                        c.Id != _oChanges.Id && c.Username.Trim().ToLower() == _oChanges.Username.Trim().ToLower()).ToList();
                if (existUserProfiles.Count() > 0) 
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Username '" + _oChanges.Username + "' not available. Try different username. [Hint: User's email is unique.]", signOutToLogIn = false });
                

                if (string.IsNullOrEmpty(_oChanges.UserDesc))  //Congregant... ChurchCodes required 
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the user description or name of user.", signOutToLogIn = false });
                
                if (_oChanges.Expr != null)  //allow historic
                {
                    if (_oChanges.UserStatus == "A" && _oChanges.Expr.Value <= DateTime.Now.Date)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please user account has expired. Activate account first.", signOutToLogIn = false });

                    if (_oChanges.Strt != null)
                        if (_oChanges.Strt.Value > _oChanges.Expr.Value)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Start date cannot be later than expiry date", signOutToLogIn = false });
                }

                //// Check password ... should be done @LOGIN instead. jux update:- keep as it is.
                //if (_oChanges.PwdExpr == null)
                //{
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User profile password not set. Reset password and try again.", signOutToLogIn = false });
                //}
                //else
                //{
                //    if (_oChanges.PwdExpr.Value <= DateTime.Now.Date)
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User profile password has expired. Reset password.", signOutToLogIn = false });
                //}

                //Email... must be REQUIRED -- for password reset!
                //if (currLogUserInfo.UserType != "SYS")
                if (string.Compare(_oChanges.Username.Trim(), "SYS", true) != 0 || _oChanges.ProfileLevel != 1) ///not sys account
                {
                    if (string.IsNullOrEmpty(_oChanges.Email)) //{
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify email of user. Email is needed for password reset and login user verifications.", signOutToLogIn = false });
                
                    //  }
                    //check email availability and validity
                   // else // (_oChanges.Email != null) //_oChanges.ChurchMemberId != null && 
                   // {
                        // ??? ... check validity... REGEX
                        ///

                        if (!AppUtilties.IsValidEmail(_oChanges.Email)) 
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User email invalid. Check and try again.", signOutToLogIn = false });
                        

                        var oUserEmailExist = _context.UserProfile.AsNoTracking().Where(c => c.Id != _oChanges.Id && c.ProfileScope == _oChanges.ProfileScope && c.Email == _oChanges.Email &&
                                                                            (
                                                                             (c.AppGlobalOwnerId == null && c.ChurchBodyId == null && _oChanges.ProfileScope == "V" && c.ProfileLevel == _oChanges.ProfileLevel) ||
                                                                             (c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && _oChanges.ProfileScope == "C" && 
                                                                                _oChanges.AppGlobalOwnerId != null && _oChanges.ChurchBodyId != null)
                                                                            )).FirstOrDefault();
                        if (oUserEmailExist != null)  // ModelState.AddModelError(_oChanges.Id.ToString(), "Email of member must be unique. >> Hint: Already used by another member: "  + GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName) + "[" + oCM.ChurchBody.Name + "]");
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User email must be unique. Email already used by another: [User: " + oUserEmailExist.UserDesc + "]", signOutToLogIn = false });


                    //var oUserCIEmailExist = _context.MSTRContactInfo.AsNoTracking().Where(c => c.RefUserId != _oChanges.Id && c.Email == _oChanges.Email &&
                    //                                                          ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && _oChanges.ProfileScope == "V") ||
                    //                                                           (c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && _oChanges.ProfileScope == "C" && 
                    //                                                                _oChanges.AppGlobalOwnerId != null && _oChanges.ChurchBodyId != null)) 
                    //                                                          ).FirstOrDefault();
                    //if (oUserCIEmailExist != null)  // ModelState.AddModelError(_oChanges.Id.ToString(), "Email of member must be unique. >> Hint: Already used by another member: "  + GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName) + "[" + oCM.ChurchBody.Name + "]");
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User email must be unique. Email already used by another: [User: " + oUserEmailExist.UserDesc + "]", signOutToLogIn = false });


                    //if (_oChanges == null)
                    //{
                    //    return Json(new
                    //    {
                    //        taskSuccess = false,
                    //        oCurrId = _oChanges.Id,
                    //        userMess = "Member status [ current state of the person - active, dormant, invalid, deceased etc. ] is required"
                    //    });
                    //}

                    ////member must be active, NOT deceased
                    //if (_oChanges_MS.ChurchMemStatusId == null)
                    //{
                    //    return Json(new
                    //    {
                    //        taskSuccess = false,
                    //        oCurrId = _oChanges.Id,
                    //        userMess = "Select the Member status [current state of the person - active, dormant, invalid, deceased etc.] as applied"
                    //    });
                    //}

                    //  }
                }


                // SYS control account check...
                var oAdminsCount = _context.UserProfile.AsNoTracking().Count(c=> c.UserStatus == "A" && c.ProfileScope == "V");
                if (string.Compare(_oChanges.Username.Trim(), "sys", true) == 0 && oAdminsCount > 1)  // other users have been created....
                {
                    //check the SYS account...
                    var oSYSAcc = _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && 
                                    c.Username.Trim().ToUpper() == "SYS" && c.UserStatus == "A").FirstOrDefault();

                    if (oSYSAcc == null)
                        return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile not found. SYS profile is a control account. Contact System Admin for help.", signOutToLogIn = false });
                    ///

                    if (oSYSAcc.PwdExpr != null)  
                    {
                    //    return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile password not set. Reset password and try again.", signOutToLogIn = false });
                    //}
                    //else
                    //{
                        if (oSYSAcc.PwdExpr.Value <= DateTime.Now.Date)
                            return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile password has expired. Reset password.", signOutToLogIn = false }); 
                    }

                    if (string.IsNullOrEmpty(oSYSAcc.Email))
                        return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile email not configured. Email is needed for password reset and login user verifications.", signOutToLogIn = false });

                }



                //if (_oChanges.ProfileScope == "V" && string.Compare(_oChanges.Username, "sys", true) == 0)
                //{
                //    var oAdminsCount = _context.UserProfile.Count(); // (c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V");
                //    if (oAdminsCount > 0 ) 
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "There are other users. SYS profile must have assigned email. Email is needed for password reset and login user verifications.", signOutToLogIn = false });
                //}

                //else
                //{
                //    if (_oChanges.Email != null)
                //    {
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify email of user. Email is needed for password reset and login user verifications.", signOutToLogIn = false });
                //    }
                //}
                 

                _oChanges.LastMod = DateTime.Now;
                _oChanges.LastModByUserId = vm.oCurrUserId_Logged;
                string uniqueFileName = null;

                var oFormFile = vm.UserPhotoFile;
                if (oFormFile != null && oFormFile.Length > 0)
                {
                    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img_db");  //~/frontend/dist/img_db
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + oFormFile.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    oFormFile.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                else
                    if (_oChanges.Id != 0) uniqueFileName = _oChanges.UserPhoto;

                _oChanges.UserPhoto = uniqueFileName;

                //_oChanges.PwdSecurityQue = "What account is this?"; _oChanges.PwdSecurityAns = "Rhema-SYS";
                if (!string.IsNullOrEmpty((_oChanges.PwdSecurityQue + _oChanges.PwdSecurityAns).Trim()))
                    _oChanges.PwdSecurityAns = AppUtilties.ComputeSha256Hash(_oChanges.PwdSecurityQue + _oChanges.PwdSecurityAns);

                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.CreatedByUserId = vm.oCurrUserId_Logged;
               // _oChanges.AppGlobalOwnerId = 
               // _oChanges.ChurchBodyId = 

                //validate...
                var _userTask = "Attempted saving user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "");  //    _userTask = "Added new user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";  // _userTask = "Updated user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";

              //  using (var _userCtx = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
              //  {
                
                bool blSendUserEmail = false; string strOldUsername = ""; bool isNewUsername = false; string strUserTempPwd = ""; var cc = "";
                var isNewUserProfile = _oChanges.Id == 0;
                if (_oChanges.Id == 0)
                {                         
                      //  const string tempPwd = "123456";
                        if (_oChanges.AppGlobalOwnerId == null && _oChanges.ChurchBodyId == null && _oChanges.ProfileScope == "V")
                        {
                            cc = "000000";    //var churchCode = "000000"; _oChanges.Username = "SysAdmin"; _oChanges.Pwd = "$ys@dmin1";  
                        }
                        else  //client admins
                        {
                            _oChanges.strChurchCode_CB = string.IsNullOrEmpty(_oChanges.strChurchCode_CB) ? oUP.ChurchBody.GlobalChurchCode : _oChanges.strChurchCode_CB;
                            cc = _oChanges.strChurchCode_CB;
                         
                        }

                    var tempPwd = CodeGenerator.GenerateCode();  // 

                    _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());
                        _oChanges.Pwd = tempPwd;  //temp pwd... to reset @ next login  
                        _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower() + _oChanges.Pwd);
                        _oChanges.Strt = tm;
                        _oChanges.Expr =  _oChanges.Expr != null ? _oChanges.Expr : ((string.Compare(_oChanges.Username.Trim(), "SYS", true) == 0 || string.Compare(_oChanges.Username.Trim(), "supadmin", true) == 0) ? (DateTime?)null : tm.AddDays(182));  //default to 90 days
                      //  _oChanges.ResetPwdOnNextLogOn = true;
                        _oChanges.PwdExpr = tm.AddDays(30);  //default to 30 days even if the default... u cannot use same pwd forever massa!
                        ///
                        _oChanges.Created = tm;
                        _oChanges.CreatedByUserId = vm.oCurrUserId_Logged;

                    //Profile level:  assigned the least role level to user until assigned role -- max role level. Ex. Registrar + Accountant = Registrar else check the permissions assigned if user can perform...

                        _context.Add(_oChanges);

                        _userTask = "Added new user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";
                        ViewBag.UserMsg = "Saved user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully. Password must be changed on next logon";

                        /////
                        // created new account... send details to account holder!
                        blSendUserEmail = true;
                        strUserTempPwd = tempPwd;
                }
                else
                {
                    // don't allow non-expiry of accounts UNLESS system accounts
                    _oChanges.Expr = _oChanges.Expr != null ? _oChanges.Expr : ((string.Compare(_oChanges.Username.Trim(), "SYS", true) == 0 || string.Compare(_oChanges.Username.Trim(), "supadmin", true) == 0) ? (DateTime?)null : tm.AddDays(182));  //default to 90 days
                     

                    if (oUP.AppGlobalOwnerId == null && oUP.ChurchBodyId == null && _oChanges.ProfileScope == "V") 
                        _oChanges.strChurchCode_CB = "000000";

                    else 
                        _oChanges.strChurchCode_CB = string.IsNullOrEmpty(_oChanges.strChurchCode_CB) ? oUP.ChurchBody?.GlobalChurchCode : _oChanges.strChurchCode_CB;
                    ///
                    cc = _oChanges.strChurchCode_CB;
                    var existUsernameList = _context.UserProfile.AsNoTracking().Where(c =>
                                           ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && _oChanges.ProfileScope == "V") ||
                                            (c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && _oChanges.ProfileScope == "C" && _oChanges.AppGlobalOwnerId != null && _oChanges.ChurchBodyId != null)) &&        //... restrict within denomination as dbase is per denomination
                                             c.Username.Trim().ToLower() == _oChanges.Username.Trim().ToLower()).ToList();

                    // username must exist ELSE it's changed!
                    isNewUsername = existUsernameList.Count == 0;
                    if (isNewUsername)
                    {
                        strOldUsername = oUP.Username.Trim();

                        // RESET password... remember [cc + username + pwd] hashed together
                        _oChanges.ResetPwdOnNextLogOn = true;

                        // user key changes..
                       // if (string.Compare(_oChanges.UserKey, AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.ToLower()), true) != 0)
                       _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());

                        // changed username... send details to account holder!
                        blSendUserEmail = true;
                    }


                    // if RESET pwd... 
                    if (_oChanges.ResetPwdOnNextLogOn==true)
                    {
                        if (oUP == null || string.IsNullOrEmpty(cc))
                        {
                            if (_oChanges.AppGlobalOwnerId == null && _oChanges.ChurchBodyId == null && _oChanges.ProfileScope == "V")
                            {
                                cc = "000000";    //var churchCode = "000000"; _oChanges.Username = "SysAdmin"; _oChanges.Pwd = "$ys@dmin1";  
                            }
                            else  //client admins
                            {                            
                                cc = _oChanges.strChurchCode_CB;

                                //cc = string.IsNullOrEmpty(_oChanges.strChurchCode_CB) ? oUP.ChurchBody.GlobalChurchCode : _oChanges.strChurchCode_CB; 
                            }
                        }
        

                        var tempPwd = CodeGenerator.GenerateCode();  // make it more unique... 000000-BZWER09J-20210405

                       // var uniqueCode = cc + "-" + tempPwd + "-" + DateTime.Now.Year + string.Format("{0:D2}", DateTime.Now.Month) + string.Format("{0:D2}", DateTime.Now.Day);
                       // var strHashVerifCode = AppUtilties.ComputeSha256Hash(uniqueCode); // "12345678"; // TempData["oVmAuthCode"] = vCode; TempData.Keep();
                       // oUserResetModel.SentVerificationCode = strHashVerifCode;
                       ///
                      //  const string tempPwd = vCode; // "123456";

                        _oChanges.Pwd = tempPwd;  //temp pwd... to reset @ next login  
                        _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower() + _oChanges.Pwd);

                        //UserKey ... lost
                        if (string.Compare(_oChanges.UserKey, AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower()), true) != 0)
                            _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());


                        /////
                        // resetPwd == true ... send details to account holder!
                        blSendUserEmail = true;
                        strUserTempPwd = tempPwd;
                    }
                                            

                    //retain the pwd details... hidden fields
                    _context.Update(_oChanges);

                    _userTask = "Updated user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";
                    ViewBag.UserMsg = "User profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + "updated successfully.";
                }


                    //save user profile first... 
                    // await _userCtx.SaveChangesAsync();

                 _context.SaveChanges();
                    ///
                   // DetachAllEntities(_userCtx);

                    if (blSendUserEmail)
                    {
                        //email recipients... applicant, church   ... specific e-mail content
                        MailAddressCollection listToAddr = new MailAddressCollection();
                        MailAddressCollection listCcAddr = new MailAddressCollection();
                        MailAddressCollection listBccAddr = new MailAddressCollection();
                    // string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

                    // var vCode = CodeGenerator.GenerateCode();
                    // oUserResetModel.SentVerificationCode = AppUtilties.ComputeSha256Hash(vCode); // "12345678";  // TempData["oVmAuthCode"] = vCode; TempData.Keep();
                    var msgSubject = "RHEMA-CMS: ";   // 
                    var userMess = "<span> Hello " + _oChanges.UserDesc + ",  </span><p>";
                        if (isNewUserProfile){
                            userMess += "<span> New user account created successfully for you. Please find logon details below:</span><p>";
                        ///
                        //userMess += "<h2 class='text-success'> Username: " + _oChanges.Username + "</h2><p>";

                            msgSubject += "New User Account";
                        }
                        else
                        {
                            userMess += "<span> User account updated successfully </span>";
                            if (isNewUsername)
                            {                                
                                userMess += "<span>[Username changed]. Please find logon details below:</span>";
                                // &nbsp;userMess += "<h2 class='text-success'> Username: " + _oChanges.Username + "</h2><p>";
                                msgSubject += "User Account Update";
                            }
                            else if (_oChanges.ResetPwdOnNextLogOn == true)
                            {
                                userMess += "<span> . User account password reset request successfully granted. </span>";
                                msgSubject += "Password Reset";
                            }
                                
                            userMess += "<span> Please find logon details below to reset: </span><p><p>";
                            ///
                            //userMess += "<h2 class='text-success'> Username: " + _oChanges.Username + "</h2><p>";
                                                        
                        }

                    userMess += "<span class='text-success'> Church code: " + cc + "</span><p>";
                    userMess += "<span class='text-success'> Username: " + _oChanges.Username.Trim() + "</span><p>";
                    userMess += "<h3 class='text-success'> Password: " + strUserTempPwd + "</h3><br /><hr /><br />";

                    userMess += "<span class='text-info text-lg'> Please you will be required to RESET password at next logon. </span><p><p>";
                    userMess += "<span class='text-info'> Thanks </span>";
                    userMess += "<span class='text-info'> RHEMA-CMS Team (Ghana) </span>";
                    userMess = "<div class='text-center col-md border border-info' style='padding: 50px 0'>" + userMess + "</div>";
                    
                    listToAddr.Add(new MailAddress(_oChanges.Email, _oChanges.UserDesc));
                    var res = AppUtilties.SendEmailNotification("RHEMA-CMS", msgSubject, userMess, listToAddr, listCcAddr, listBccAddr, null, true);

                    }
                // }


                //audit... 
                var _connstr = AppUtilties.GetNewDBConnString_MS(_configuration);  /// this._configuration["ConnectionStrings:DefaultConnection"];

                if (string.IsNullOrEmpty(_connstr))
                    return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg + ". But user roles /permissions update could not complete successfully. Hint: See Admin to handle it manually.", signOutToLogIn = false });


                var _tm = DateTime.Now;
                await this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged)
                     );




                // var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];
                ///   _configuration = configuration;  // private readonly IConfiguration _configuration;  // , IConfiguration configuration
                ///   
                //var _cs = strTempConn;
                //if (string.IsNullOrEmpty(_cs))
                //    _cs = this._configuration["ConnectionStrings:DefaultConnection"];


                /// check for some inconsistencies... UPM, UR, UG ...update first !
                /// 
                if (_oChanges.ProfileLevel == 1 || _oChanges.ProfileLevel == 2)
                {
                    // check and update the USER ROLES
                    //// UPDATE USER ROLES ----
                    var oUtil = new AppUtilties();

                    /// check roles
                    var roleList = oUtil.GetSystemDefaultRoles();
                    var _URListCopy = _context.UserRole.AsNoTracking().ToList();   //copy... 
                    ///  
                    /// check perms
                    var _UPMListCopy = _context.UserPermission.AsNoTracking().ToList();   //copy... 

                    //create the user permissions 
                    var permList = oUtil.GetSystem_Administration_Permissions();
                    var permList1 = oUtil.GetAppDashboard_Permissions();
                    var permList2 = oUtil.GetAppConfigurations_Permissions();
                    var permList3 = oUtil.GetMemberRegister_Permissions();
                    var permList4 = oUtil.GetChurchlifeAndEvents_Permissions();
                    var permList5 = oUtil.GetChurchAdministration_Permissions();
                    var permList6 = oUtil.GetFinanceManagement_Permissions();
                    var permList7 = oUtil.GetReportsAnalytics_Permissions();

                    //var permList3 = oUtil.get();
                    permList = AppUtilties.CombineCollection(permList, permList1, permList2, permList3, permList4);
                    permList = AppUtilties.CombineCollection(permList, permList5, permList6, permList7);
                     
                    ///
                    /// check  URP ...  copy
                    var oURPListCopy = _context.UserRolePermission.AsNoTracking()   ///.Include(t=> t.UserRole)
                                        .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null).ToList();     //copy... && c.Status == "A"   everything!
                    var oURPListCopy_SUPADMN = new List<UserRolePermission>();
                    if (oURPListCopy.Count != 0) 
                        oURPListCopy_SUPADMN = _context.UserRolePermission.AsNoTracking()  ///.Include(t => t.UserRole)
                                        .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRole.RoleType == "SUP_ADMN").ToList();

                    var isURPMismatched = (oURPListCopy.Count == 0 || oURPListCopy_SUPADMN.Count == 0);
                    ///
                    /// either role changes or perm changes ---
                    if ((roleList.Count != _URListCopy.Count() || _URListCopy.Count() == 0) || 
                        (permList.Count != _UPMListCopy.Count() || _UPMListCopy.Count() == 0) || 
                        (isURPMismatched == true))
                    {
                        // clear this list...
                         oURPListCopy_SUPADMN.Clear();

                        // if roles change
                        if (roleList.Count != _URListCopy.Count() || _URListCopy.Count() == 0)
                        {
                            /// keep the UR configured in UPR --- update! no need to clear UPR and UR tables 
                            /// only the newly added UR --- to be added to UPR.
                            /// 
                            var _roleChangesAdd = 0; var _roleChangesUpd = 0;
                            for (var i = 0; i < roleList.Count; i++)
                            {
                                var oURExist = _context.UserRole.AsNoTracking().Where(c => c.RoleType.ToLower().Equals(roleList[i].RoleType.ToLower())).FirstOrDefault();
                                if (oURExist == null)
                                {
                                    _context.Add(new UserRole()
                                    {
                                        ChurchBodyId = null,
                                        RoleName = roleList[i].RoleName,
                                        //CustomRoleName = roleList[i].CustomRoleName,
                                        RoleType = roleList[i].RoleType,
                                        RoleDesc = roleList[i].RoleDesc,
                                        RoleLevel = roleList[i].RoleLevel,
                                        RoleStatus = roleList[i].RoleStatus, //"A",
                                        OwnedBy = roleList[i].OwnedBy,
                                        Created = tm,
                                        LastMod = tm,
                                        CreatedByUserId = vm.oCurrUserId_Logged,
                                        LastModByUserId = vm.oCurrUserId_Logged
                                    });

                                    _roleChangesAdd++;
                                }
                                else
                                {
                                    oURExist.ChurchBodyId = null;
                                    oURExist.RoleName = roleList[i].RoleName;
                                    //oURExist.CustomRoleName = roleList[i].CustomRoleName;
                                    oURExist.RoleType = roleList[i].RoleType;
                                    oURExist.RoleDesc = roleList[i].RoleDesc;
                                    oURExist.RoleLevel = roleList[i].RoleLevel;
                                    oURExist.RoleStatus = roleList[i].RoleStatus; //"A",
                                    oURExist.OwnedBy = roleList[i].OwnedBy;
                                    oURExist.Created = tm;
                                    oURExist.LastMod = tm;
                                    oURExist.CreatedByUserId = vm.oCurrUserId_Logged;
                                    oURExist.LastModByUserId = vm.oCurrUserId_Logged;

                                    _context.Update(oURExist);
                                    _roleChangesUpd++;
                                }
                            }


                            // logoutCurrUser = _roleChanges > 0;
                            if ((_roleChangesAdd + _roleChangesUpd) > 0)
                            {
                                _context.SaveChanges();
                                ///
                                _userTask = "Created the default SYS role"; _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                     "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                            }
                        }




                        //// UPDATE USER PERMS ----
                        //var _UPMList = _context.UserPermission.AsNoTracking().ToList();   //copy... 

                        ////create the user permissions 
                        //var permList = oUtil.GetSystem_Administration_Permissions();
                        //var permList1 = oUtil.GetAppDashboard_Permissions();
                        //var permList2 = oUtil.GetAppConfigurations_Permissions();
                        //var permList3 = oUtil.GetMemberRegister_Permissions();
                        //var permList4 = oUtil.GetChurchlifeAndEvents_Permissions();
                        //var permList5 = oUtil.GetChurchAdministration_Permissions();
                        //var permList6 = oUtil.GetFinanceManagement_Permissions();
                        //var permList7 = oUtil.GetReportsAnalytics_Permissions();

                        ////var permList3 = oUtil.get();
                        //permList = AppUtilties.CombineCollection(permList, permList1, permList2, permList3, permList4);
                        //permList = AppUtilties.CombineCollection(permList, permList5, permList6, permList7);
                        // 



                        // mismatch!  --- roles change or perm change  --- at this point either of the 2 is TRUE....
                        ///
                        //if (permList.Count != _UPMList.Count() || _context.UserRolePermission.AsNoTracking().Count() == 0)
                        //{
                            // copy UPR ...
                            var oUPRListCopy = _context.UserProfileRole.Include(t => t.UserRole).AsNoTracking().ToList();     //copy...  everything!.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null)


                        //// get the yet to exist roles.... advance  ... handled by specific request for  ... UPR  >>> scroll down ...
                        //var oUPRList_NotExist = (from t_upr in oUPRListCopy  /// _context.UserProfileRole.AsNoTracking().ToList()
                        //                        from t_ur in _context.UserRole.AsNoTracking().Where(c => c.Id != t_upr.UserRoleId).ToList()
                        //                        select new UserProfileRole()
                        //                        {
                        //                            AppGlobalOwnerId = null,
                        //                            ChurchBodyId = null,
                        //                            UserRoleId = t_ur.Id,
                        //                            UserProfileId = null,
                        //                            ///
                        //                         UserRole = t_ur
                        //                        }
                        //                         ).ToList();

                        //// merge the Copy and NotExist  ... exist will restore [Add with previous details] and NotExist will be added too!
                        //oUPRListCopy.AddRange(oUPRList_NotExist);


                        //// copy URP ...
                        //var oURPListCopy = _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null).ToList();     //copy... && c.Status == "A"   everything!
                        // var lsRows_URP = oURPListCopy;

                        // clear URP ...
                        if (oURPListCopy.Count() > 0)
                            {
                                _context.UserRolePermission.RemoveRange(oURPListCopy);
                                _context.SaveChanges();

                                // RESEED... auto column Id to start from 1 again
                                var tabName = "UserRolePermission";
                                _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                            }

                            ///
                            _userTask = "Deleted " + oURPListCopy.Count() + " default user role permissions"; _tm = DateTime.Now;
                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                 "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));


                            // clear UPM table and insert new..
                            var lsRows_UPM = _UPMListCopy;
                            if (lsRows_UPM.Count() > 0)
                            {
                                _context.UserPermission.RemoveRange(lsRows_UPM);
                                _context.SaveChanges();

                                // RESEED... auto column Id to start from 1 again
                                var tabName = "UserPermission";
                                _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                            }

                            ///
                            _userTask = "Deleted " + lsRows_UPM.Count() + " default user permissions"; _tm = DateTime.Now;
                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                 "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));

                            ///and insert new... UPM
                            ///// recreation of UPM needed bcos of URP --- role or perm changing
                            var _permChanges = 0;
                            for (var i = 0; i < permList.Count; i++)
                            {
                                var checkExist = _context.UserPermission.AsNoTracking().Where(c => c.PermissionName.ToLower().Equals(permList[i].PermissionName.ToLower())).FirstOrDefault();
                                if (checkExist == null)
                                {
                                    _context.Add(new UserPermission()
                                    {
                                        PermissionName = permList[i].PermissionName,
                                        // UserPermCategoryId = null,
                                        PermissionCode = AppUtilties.GetPermissionCode_FromName(permList[i].PermissionName),
                                        PermStatus = "A",
                                        Created = tm,
                                        LastMod = tm,
                                        CreatedByUserId = vm.oCurrUserId_Logged,
                                        LastModByUserId = vm.oCurrUserId_Logged
                                    });

                                    _permChanges++;
                                }
                            }

                            // logoutCurrUser = _permChanges > 0;
                            if (_permChanges > 0)
                            {
                                _context.SaveChanges();
                                ///
                                _userTask = "Created " + _permChanges + " default user permissions"; _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                     "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));
                            }


                            // restore the URP mapping... the roles will reconnect to the mappings in UPR... in that order...
                            ///
                            // add perms to new role (s) created.. SYS role - SUP_ADMN perm  :: lsPerms.Add(new UserPermission(0, null, "A0_00", "_A0_00__Super_Admin_Account", null, "A", null, null, null, null, null));   // for SYS account only
                            // 1 role --- 2 perms
                            // var userRole = _context.UserRole.AsNoTracking().Where(c => c.RoleType == "SYS" && c.RoleStatus == "A").FirstOrDefault();
                            ///

                            //var userPermList = _context.UserPermission.AsNoTracking().Where(c => (c.PermissionCode == "A0_00" || c.PermissionCode == "A0_01") && c.PermStatus == "A").ToList();

                            var oUPMList_MSTR = _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A").ToList();
                            var oUPM_PermListCurr = new List<UserPermission>();
                            /// 
                            //var lsRows_UPR = oUPRList;    //  if (oUPRList.Count > 0) 
                            var cntURPChanges_Add = 0; var cntURPChanges_Upd = 0; var isRowUpd = false;
                            var totURPChanges_Add = 0; var totURPChanges_Upd = 0; var updRoleIdList = new List<string>();
                            foreach (var oUPR in oUPRListCopy)  /// do for the default roles and/or create the UPR when role is assigned to user --->> to cater for custome roles also... skip the UPR config in adv. fyn!
                            {
                                if (oUPR.UserRole != null && !updRoleIdList.Contains((oUPR.UserRole != null ? oUPR.UserRole.RoleType : null)))
                                {
                                    updRoleIdList.Add(oUPR.UserRole.RoleType);

                                    // get perm list based on role...   
                                    if (oUPR.UserRole.RoleLevel == 1 && oUPR.UserRole.RoleType == "SYS")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode == "A0_00" || c.PermissionCode == "A0_01").ToList();  // SYS [2]
                                    else if (oUPR.UserRole.RoleLevel == 2 && oUPR.UserRole.RoleType == "SUP_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => (c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00") || !c.PermissionCode.StartsWith("A0")).ToList();  // SUP_ADMN [12 + 69]
                                    else if (oUPR.UserRole.RoleLevel == 3 && oUPR.UserRole.RoleType == "SYS_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00" && c.PermissionCode != "A0_01" && c.PermissionCode != "A0_04").ToList();  // SYS_ADMN [9]
                                    else if (oUPR.UserRole.RoleLevel == 4 && oUPR.UserRole.RoleType == "SYS_CL_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => (c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00" && c.PermissionCode != "A0_01" && c.PermissionCode != "A0_02") || !c.PermissionCode.StartsWith("A0")).ToList();  // SYS_CL_ADMN [9 + 69]

                                ///
                                else if (oUPR.UserRole.RoleLevel == 6 && oUPR.UserRole.RoleType == "CH_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => !c.PermissionCode.StartsWith("A0")).ToList();  // CH_ADMN [6] ... 69
                                    else if (oUPR.UserRole.RoleLevel == 7 && oUPR.UserRole.RoleType == "CH_MGR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [7]
                                    else if (oUPR.UserRole.RoleLevel == 8 && oUPR.UserRole.RoleType == "CH_EXC")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [8]
                                    else if (oUPR.UserRole.RoleLevel == 9 && oUPR.UserRole.RoleType == "CH_RGSTR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [9]                                
                                    else if (oUPR.UserRole.RoleLevel == 10 && oUPR.UserRole.RoleType == "CH_ACCT")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("05") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [10] 
                                    /// 
                                    else if (oUPR.UserRole.RoleLevel == 11 && oUPR.UserRole.RoleType == "CF_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => !c.PermissionCode.StartsWith("A0")).ToList();  // CF_ADMN [11] ... 69
                                    else if (oUPR.UserRole.RoleLevel == 12 && oUPR.UserRole.RoleType == "CF_MGR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [7]
                                    else if (oUPR.UserRole.RoleLevel == 13 && oUPR.UserRole.RoleType == "CF_EXC")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [8]
                                    else if (oUPR.UserRole.RoleLevel == 14 && oUPR.UserRole.RoleType == "CF_RGSTR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [9]                                
                                    else if (oUPR.UserRole.RoleLevel == 15 && oUPR.UserRole.RoleType == "CF_ACCT")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("05") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [10] 


                                    /// 

                                    //get dash and RA
                                    // oUPM_PermListCurr.AddRange(oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00")).ToList());  // possible it might av been added via previous...
                                    //  oUPM_PermListCurr.AddRange(oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("06")).ToList());  // possible it might av been added via previous...

                                    foreach (var oUPMCurr in oUPM_PermListCurr) //var i = 0; i < userPermList.Count; i++)
                                    {
                                        var oURPExist = _context.UserRolePermission.AsNoTracking().Where(c => c.UserRoleId == oUPR.UserRoleId &&
                                                            (c.UserPermissionId == oUPMCurr.Id || c.UserPermission.PermissionCode == oUPMCurr.PermissionCode)).FirstOrDefault();
                                        if (oURPExist == null)
                                        {
                                            var _allowCruD = (oUPR.UserRole.RoleLevel == 1 && oUPMCurr.PermissionCode != "A0_00") || (oUPR.UserRole.RoleLevel == 2 && oUPMCurr.PermissionCode != "A0_01") || (oUPR.UserRole.RoleLevel == 3 && oUPMCurr.PermissionCode != "A0_02") || (oUPR.UserRole.RoleLevel == 4 && oUPMCurr.PermissionCode != "A0_04") ||
                                                            ((oUPR.UserRole.RoleLevel >= 6 || oUPR.UserRole.RoleLevel <= 15) && !oUPMCurr.PermissionCode.StartsWith("A0"));
                                            ///
                                            _context.Add(new UserRolePermission()
                                            {
                                                ChurchBodyId = null,
                                                UserPermissionId = oUPMCurr.Id,
                                                UserRoleId = oUPR.UserRoleId,
                                                Status = "A",
                                                ViewPerm = true,
                                                CreatePerm = _allowCruD,
                                                EditPerm = true,
                                                DeletePerm = _allowCruD,
                                                ManagePerm = true,
                                                Created = tm,
                                                LastMod = tm,
                                                CreatedByUserId = vm.oCurrUserId_Logged,
                                                LastModByUserId = vm.oCurrUserId_Logged
                                            });

                                            cntURPChanges_Add++;
                                        }
                                        else
                                        {  /// naffin to update... URP cleared!
                                            var _allowCruD = (oUPR.UserRole.RoleLevel == 1 && oUPMCurr.PermissionCode != "A0_00") || (oUPR.UserRole.RoleLevel == 2 && oUPMCurr.PermissionCode != "A0_01") || (oUPR.UserRole.RoleLevel == 3 && oUPMCurr.PermissionCode != "A0_02") || (oUPR.UserRole.RoleLevel == 4 && oUPMCurr.PermissionCode != "A0_04") ||
                                                            ((oUPR.UserRole.RoleLevel >= 6 || oUPR.UserRole.RoleLevel <= 15) && !oUPMCurr.PermissionCode.StartsWith("A0"));
                                            ///
                                            isRowUpd = false;
                                            if (!oURPExist.ViewPerm) { oURPExist.ViewPerm = true; isRowUpd = true; }
                                            if (!oURPExist.CreatePerm) { oURPExist.CreatePerm = _allowCruD; isRowUpd = true; }
                                            if (!oURPExist.EditPerm) { oURPExist.EditPerm = true; isRowUpd = true; }
                                            if (!oURPExist.DeletePerm) { oURPExist.DeletePerm = _allowCruD; isRowUpd = true; }
                                            if (!oURPExist.ManagePerm) { oURPExist.ManagePerm = true; isRowUpd = true; }

                                            if (isRowUpd)
                                            {
                                                oURPExist.Created = tm;
                                                oURPExist.LastMod = tm;
                                                oURPExist.CreatedByUserId = vm.oCurrUserId_Logged;
                                                oURPExist.LastModByUserId = vm.oCurrUserId_Logged;

                                                _context.Update(oURPExist);
                                                cntURPChanges_Upd++;
                                            }
                                        }
                                    }

                                    // update....
                                    if ((cntURPChanges_Add + cntURPChanges_Upd) > 0)
                                    {
                                        _context.SaveChanges();
                                        ///
                                        totURPChanges_Add += cntURPChanges_Add;
                                        totURPChanges_Upd += cntURPChanges_Upd;
                                    }

                                    ///
                                    // init...
                                    cntURPChanges_Add = 0; cntURPChanges_Upd = 0;
                                }
                            }


                            // logoutCurrUser = _URPChanges > 0;   
                            if ((totURPChanges_Add + totURPChanges_Upd) > 0)
                            {
                                // _context.SaveChanges();
                                ///
                                _userTask = "Assigned " + totURPChanges_Add + ", updated [" + totURPChanges_Upd + "] permission(s) to [available] roles successfully"; _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                        "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));
                            }
                      ///  }

                    }
                }


                // synched...
                ///
                //check if role assigned... SUP_ADMN -- auto, others -- warn!    var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];
                using (var _roleCtx = new MSTR_DbContext(_connstr))
                {     
                     
                    if (_oChanges.ProfileLevel == 2) //subSetIndex == 2) // SUP_ADMN role
                    {

                        var oSupAdminRole = _context.UserRole.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId== null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                        if (oSupAdminRole != null)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking()
                                                  .Where(c => c.AppGlobalOwnerId==null && c.ChurchBodyId == null && c.UserRoleId == oSupAdminRole.Id && c.ProfileRoleStatus == "A") // &&   // ((c.Strt == null || c.Expr == null) || (c.Strt != null && c.Expr != null && c.Strt <= DateTime.Now && c.Expr >= DateTime.Now && c.Strt <= c.Expr)))  // from up in roleCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                                                  select upr
                                         ).ToList();

                            //add SUP_ADMN role to SUP_ADMN user ... assign all privileges to the SUP_ADMN role
                            if (existUserRoles.Count() == 0)
                            {
                                //var oSupAdminRole = roleCtx.UserRole.Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                                //if (oSupAdminRole != null)
                                //{        

                                var oUserRole = new UserProfileRole
                                {
                                    AppGlobalOwnerId = null,
                                    ChurchBodyId = null,
                                    UserRoleId = oSupAdminRole.Id,
                                    UserProfileId = _oChanges.Id,
                                    Strt = tm,
                                    // Expr = tm,
                                    ProfileRoleStatus = "A",
                                    Created = tm,
                                    LastMod = tm,
                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                    LastModByUserId = vm.oCurrUserId_Logged
                                };

                                _roleCtx.Add(oUserRole);
                                //save user role...
                                await _roleCtx.SaveChangesAsync();

                                DetachAllEntities(_roleCtx);


                                _userTask = "Added SUP_ADMN role to user, " + _oChanges.Username.Trim() ;
                                ViewBag.UserMsg += Environment.NewLine + " ~ SUP_ADMN role added.";


                                _tm = DateTime.Now;
                                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));
                                // }


                                using (var _permCtx = new MSTR_DbContext(_connstr))
                                {
                                    // assign all privileges [vendor domain only] to the SUP_ADMN role 
                                    var existUserRolePerms = (from upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null &&
                                                              c.Status == "A" && c.UserRoleId == oSupAdminRole.Id && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                              select upr).ToList();
                                    //if (existUserRolePerms.Count() > 0)
                                    //{

                                    // get only [A0_] System Admin permissions ... except A0_00 and A0_01 [ view, edit ]
                                    // var _permList = new AppUtilties().GetSystem_Administration_Permissions()
                                    //                  .Where(c => c.PermStatus == "A" && c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00").ToList(); // && c.PermissionCode != "A0_01");

                                    // get dbase values instead... but Vendor Admin MUST BE ABLE TO REFRESH THE PERMS LIST manually.
                                    // get only [A0_] System Admin permissions ... except A0_00 and A0_01 [ view, edit ]
                                    var oUserPerms = (from upr in _context.UserPermission.AsNoTracking()
                                                      .Where(c => c.PermStatus == "A" && ((c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00") || !c.PermissionCode.StartsWith("A0")))                                                                                                                                                       // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                      select upr).ToList();


                                    ////get the user permissions
                                   // var oUtil = new AppUtilties();
                                   // var oUserPerms1 = oUtil.GetSystem_Administration_Permissions().Where(c => c.PermStatus == "A" && c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00").ToList();
                                    /////


                                    ////create the user permissions
                                    //var oUtil = new AppUtilties();
                                    //var permList = oUtil.GetSystem_Administration_Permissions();  // A0
                                    //var permList1 = oUtil.GetAppDashboard_Permissions();  // 00
                                    //var permList2 = oUtil.GetAppConfigurations_Permissions();   // 01
                                    //var permList3 = oUtil.GetMemberRegister_Permissions();   // 02
                                    //var permList4 = oUtil.GetChurchlifeAndEvents_Permissions();   // 03
                                    //var permList5 = oUtil.GetChurchAdministration_Permissions();   // 04
                                    //var permList6 = oUtil.GetFinanceManagement_Permissions();   // 05
                                    //var permList7 = oUtil.GetReportsAnalytics_Permissions();   // 06

                                    ////var permList3 = oUtil.get();
                                    //permList = AppUtilties.CombineCollection(permList, permList1, permList2, permList3, permList4);
                                    //permList = AppUtilties.CombineCollection(permList, permList5, permList6, permList7);


                                    if (oUserPerms.Count() > 0) //(existUserRolePerms.Count() < oUserPerms.Count())
                                    {
                                        var rowUpdated = false; var rowsUpdated = 0; var rowsAdded = 0;
                                        foreach (var oUPM in oUserPerms)
                                        {
                                            var existUserRolePerm = existUserRolePerms.Where(c => c.UserPermissionId == oUPM.Id).FirstOrDefault();
                                            if (existUserRolePerm == null)
                                            {
                                                var oUserRolePerm = new UserRolePermission
                                                {
                                                    AppGlobalOwnerId = null,
                                                    ChurchBodyId = null,
                                                    UserRoleId = oSupAdminRole.Id,
                                                    UserPermissionId = oUPM.Id,
                                                    ViewPerm = true,
                                                    CreatePerm = oUPM.PermissionCode != "A0_01",  //supadmin cannnot create itself... SYS-A0_00, SUPADMN-A0_01, SYS_ADMN-A0_02
                                                    EditPerm = true,
                                                    DeletePerm = oUPM.PermissionCode != "A0_01",  //supadmin cannnot delete itself
                                                    ManagePerm = oUPM.PermissionCode != "A0_01",  //supadmin manage create itself
                                                    Status = "A",
                                                    Created = tm,
                                                    LastMod = tm,
                                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                                    LastModByUserId = vm.oCurrUserId_Logged
                                                };

                                                _permCtx.Add(oUserRolePerm);
                                                rowsAdded++;
                                            }
                                            else
                                            {
                                                rowUpdated = false;
                                                if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = oUPM.PermissionCode != "A0_01"; rowUpdated = true; }
                                                if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = oUPM.PermissionCode != "A0_01"; rowUpdated = true; }
                                                if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = oUPM.PermissionCode != "A0_01"; rowUpdated = true; }

                                                if (rowUpdated)
                                                {
                                                    existUserRolePerm.Created = tm;
                                                    existUserRolePerm.LastMod = tm;
                                                    existUserRolePerm.CreatedByUserId = vm.oCurrUserId_Logged;
                                                    existUserRolePerm.LastModByUserId = vm.oCurrUserId_Logged;

                                                    _permCtx.Update(existUserRolePerm);
                                                    rowsUpdated++;
                                                }
                                            }
                                        }

                                        // prompt users
                                        if (rowsAdded > 0)
                                        {
                                            _userTask = "Added " + rowsAdded + " user permissions to SUP_ADMN role";
                                            ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user permissions added.";
                                        }
                                        if (rowsUpdated > 0)
                                        {
                                            _userTask = "Updated " + rowsUpdated + " user permissions on SUP_ADMN role";
                                            ViewBag.UserMsg += ". " + rowsUpdated + " user permissions updated.";
                                        }

                                        if ((rowsAdded + rowsUpdated) > 0)
                                        {
                                            //save changes... 
                                            await _permCtx.SaveChangesAsync();

                                            DetachAllEntities(_permCtx);

                                            _tm = DateTime.Now;
                                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                              "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));

                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (_oChanges.ProfileLevel == 3) //subSetIndex == 2) // SYS_ADMN role
                    {
                        var oAdminRole = _context.UserRole.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 3 && c.RoleType == "SYS_ADMN").FirstOrDefault();
                        if (oAdminRole != null)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking()
                                                  .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRoleId == oAdminRole.Id &&
                                                                c.UserProfileId == _oChanges.Id && c.ProfileRoleStatus == "A") 
                                                     
                                                  select upr
                                         ).ToList();

                            //add SUP_ADMN role to SUP_ADMN user ... assign all privileges to the SUP_ADMN role
                            if (existUserRoles.Count() == 0)
                            {
                                //var oSupAdminRole = roleCtx.UserRole.Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                                //if (oSupAdminRole != null)
                                //{        

                                var oUserRole = new UserProfileRole
                                {
                                    AppGlobalOwnerId = null,
                                    ChurchBodyId = null,
                                    UserRoleId = oAdminRole.Id,
                                    UserProfileId = _oChanges.Id,
                                    Strt = tm,
                                    // Expr = tm,
                                    ProfileRoleStatus = "A",
                                    Created = tm,
                                    LastMod = tm,
                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                    LastModByUserId = vm.oCurrUserId_Logged
                                };

                                _roleCtx.Add(oUserRole);
                                //save user role...
                                await _roleCtx.SaveChangesAsync();

                                DetachAllEntities(_roleCtx);


                                _userTask = "Added SYS_ADMN role to user, " + _oChanges.Username;
                                ViewBag.UserMsg += Environment.NewLine + " ~ SYS_ADMN role added.";


                                _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));
                                // }


                                using (var _permCtx = new MSTR_DbContext(_connstr))
                                {
                                    // assign all privileges [vendor domain only] to the SUP_ADMN role 
                                    var existUserRolePerms = (from upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null &&
                                                              c.Status == "A" && c.UserRoleId == oAdminRole.Id && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                              select upr).ToList();
                                    //if (existUserRolePerms.Count() > 0)
                                    //{

                                    // get only [A0_] System Admin permissions ... except A0_00 and A0_01 [ view, edit ]
                                    var oUserPerms = (from upr in _context.UserPermission.AsNoTracking()
                                                      .Where(c => c.PermStatus == "A" && c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00" && c.PermissionCode != "A0_01" && c.PermissionCode != "A0_04")                                                                                                                                                      // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                      select upr).ToList();

                                    if (oUserPerms.Count() > 0) //(existUserRolePerms.Count() < oUserPerms.Count())
                                    {
                                        var rowUpdated = false; var rowsUpdated = 0; var rowsAdded = 0;
                                        foreach (var oUPM in oUserPerms)
                                        {
                                            var existUserRolePerm = existUserRolePerms.Where(c => c.UserPermissionId == oUPM.Id).FirstOrDefault();
                                            if (existUserRolePerm == null)
                                            {
                                                var oUserRolePerm = new UserRolePermission
                                                {
                                                    AppGlobalOwnerId = null,
                                                    ChurchBodyId = null,
                                                    UserRoleId = oAdminRole.Id,
                                                    UserPermissionId = oUPM.Id,
                                                    ViewPerm = true,
                                                    CreatePerm = oUPM.PermissionCode != "A0_02",  //supadmin cannnot create itself
                                                    EditPerm = true,
                                                    DeletePerm = oUPM.PermissionCode != "A0_02",  //supadmin cannnot delete itself
                                                    ManagePerm = oUPM.PermissionCode != "A0_02",  //supadmin manage create itself
                                                    Status = "A",
                                                    Created = tm,
                                                    LastMod = tm,
                                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                                    LastModByUserId = vm.oCurrUserId_Logged
                                                };

                                                _permCtx.Add(oUserRolePerm);
                                                rowsAdded++;
                                            }
                                            else
                                            {
                                                rowUpdated = false;
                                                if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = oUPM.PermissionCode != "A0_02"; rowUpdated = true; }
                                                if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = oUPM.PermissionCode != "A0_02"; rowUpdated = true; }
                                                if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = oUPM.PermissionCode != "A0_02"; rowUpdated = true; }

                                                if (rowUpdated)
                                                {
                                                    existUserRolePerm.Created = tm;
                                                    existUserRolePerm.LastMod = tm;
                                                    existUserRolePerm.CreatedByUserId = vm.oCurrUserId_Logged;
                                                    existUserRolePerm.LastModByUserId = vm.oCurrUserId_Logged;

                                                    _permCtx.Update(existUserRolePerm);
                                                    rowsUpdated++;
                                                }
                                            }
                                        }

                                        // prompt users
                                        if (rowsAdded > 0)
                                        {
                                            _userTask = "Added " + rowsAdded + " user permissions to SYS_ADMN role";
                                            ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user permissions added.";
                                        }
                                        if (rowsUpdated > 0)
                                        {
                                            _userTask = "Updated " + rowsUpdated + " user permissions on SYS_ADMN role";
                                            ViewBag.UserMsg += ". " + rowsUpdated + " user permissions updated.";
                                        }

                                        if ((rowsAdded + rowsUpdated) > 0)
                                        {
                                            //save changes... 
                                            await _permCtx.SaveChangesAsync();

                                            DetachAllEntities(_permCtx);

                                            _tm = DateTime.Now;
                                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                              "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));

                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (_oChanges.ProfileLevel == 4) //subSetIndex == 2) // SYS_ADMN role
                    {
                        var oCLSysAdminRole = _context.UserRole.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 4 && c.RoleType == "SYS_CL_ADMN").FirstOrDefault();
                        if (oCLSysAdminRole != null)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking()
                                                  .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRoleId == oCLSysAdminRole.Id &&
                                                                c.UserProfileId == _oChanges.Id && c.ProfileRoleStatus == "A")

                                                  select upr
                                         ).ToList();

                            //add SUP_ADMN role to SUP_ADMN user ... assign all privileges to the SUP_ADMN role
                            if (existUserRoles.Count() == 0)
                            {
                                //var oSupAdminRole = roleCtx.UserRole.Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                                //if (oSupAdminRole != null)
                                //{        

                                var oUserRole = new UserProfileRole
                                {
                                    AppGlobalOwnerId = null,
                                    ChurchBodyId = null,
                                    UserRoleId = oCLSysAdminRole.Id,
                                    UserProfileId = _oChanges.Id,
                                    Strt = tm,
                                    // Expr = tm,
                                    ProfileRoleStatus = "A",
                                    Created = tm,
                                    LastMod = tm,
                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                    LastModByUserId = vm.oCurrUserId_Logged
                                };

                                _roleCtx.Add(oUserRole);
                                //save user role...
                                await _roleCtx.SaveChangesAsync();

                                DetachAllEntities(_roleCtx);


                                _userTask = "Added SYS_CL_ADMN role to user, " + _oChanges.Username;
                                ViewBag.UserMsg += Environment.NewLine + " ~ SYS_CL_ADMN role added.";


                                _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));
                                // }


                                using (var _permCtx = new MSTR_DbContext(_connstr))
                                {
                                    // assign all privileges [vendor domain only] to the SUP_ADMN role 
                                    var existUserRolePerms = (from upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null &&
                                                              c.Status == "A" && c.UserRoleId == oCLSysAdminRole.Id && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                              select upr).ToList();
                                    //if (existUserRolePerms.Count() > 0)
                                    //{

                                    // get only [A0_] System Admin permissions ... except A0_00 and A0_01 [ view, edit ]
                                    var oUserPerms = (from upr in _context.UserPermission.AsNoTracking()
                                                      .Where(c => c.PermStatus == "A" && ((c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00" && c.PermissionCode != "A0_01" && c.PermissionCode != "A0_02") || !c.PermissionCode.StartsWith("A0")))                                                                                                                                                      // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                      select upr).ToList();

                                    if (oUserPerms.Count() > 0) //(existUserRolePerms.Count() < oUserPerms.Count())
                                    {
                                        var rowUpdated = false; var rowsUpdated = 0; var rowsAdded = 0;
                                        foreach (var oUPM in oUserPerms)
                                        {
                                            var existUserRolePerm = existUserRolePerms.Where(c => c.UserPermissionId == oUPM.Id).FirstOrDefault();
                                            if (existUserRolePerm == null)
                                            {
                                                var oUserRolePerm = new UserRolePermission
                                                {
                                                    AppGlobalOwnerId = null,
                                                    ChurchBodyId = null,
                                                    UserRoleId = oCLSysAdminRole.Id,
                                                    UserPermissionId = oUPM.Id,
                                                    ViewPerm = true,
                                                    CreatePerm = oUPM.PermissionCode != "A0_04",  //supadmin cannnot create itself
                                                    EditPerm = true,
                                                    DeletePerm = oUPM.PermissionCode != "A0_04",  //supadmin cannnot delete itself
                                                    ManagePerm = oUPM.PermissionCode != "A0_04",  //supadmin manage create itself
                                                    Status = "A",
                                                    Created = tm,
                                                    LastMod = tm,
                                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                                    LastModByUserId = vm.oCurrUserId_Logged
                                                };

                                                _permCtx.Add(oUserRolePerm);
                                                rowsAdded++;
                                            }
                                            else
                                            {
                                                rowUpdated = false;
                                                if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = oUPM.PermissionCode != "A0_04"; rowUpdated = true; }
                                                if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = oUPM.PermissionCode != "A0_04"; rowUpdated = true; }
                                                if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = oUPM.PermissionCode != "A0_04"; rowUpdated = true; }

                                                if (rowUpdated)
                                                {
                                                    existUserRolePerm.Created = tm;
                                                    existUserRolePerm.LastMod = tm;
                                                    existUserRolePerm.CreatedByUserId = vm.oCurrUserId_Logged;
                                                    existUserRolePerm.LastModByUserId = vm.oCurrUserId_Logged;

                                                    _permCtx.Update(existUserRolePerm);
                                                    rowsUpdated++;
                                                }
                                            }
                                        }

                                        // prompt users
                                        if (rowsAdded > 0)
                                        {
                                            _userTask = "Added " + rowsAdded + " user permissions to SYS_CL_ADMN role";
                                            ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user permissions added.";
                                        }
                                        if (rowsUpdated > 0)
                                        {
                                            _userTask = "Updated " + rowsUpdated + " user permissions on SYS_CL_ADMN role";
                                            ViewBag.UserMsg += ". " + rowsUpdated + " user permissions updated.";
                                        }

                                        if ((rowsAdded + rowsUpdated) > 0)
                                        {
                                            //save changes... 
                                            await _permCtx.SaveChangesAsync();

                                            DetachAllEntities(_permCtx);

                                            _tm = DateTime.Now;
                                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                              "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));

                                        }
                                    }
                                }
                            }
                        }
                    }


                    else if (_oChanges.ProfileLevel == 6 || _oChanges.ProfileLevel == 11)  //(vmMod.subSetIndex == 6 || vmMod.subSetIndex == 11) // SUP_ADMN role
                    {
                        var oChuAdminRole = _context.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" && 
                                                                        (((oUP.ChurchBody.OrgType == "CR" || oUP.ChurchBody.OrgType == "CH") && c.RoleType == "CH_ADMN" && c.RoleLevel == 6) || 
                                                                         (oUP.ChurchBody.OrgType == "CN" && c.RoleType == "CF_ADMN" && c.RoleLevel == 11)))
                                                             .FirstOrDefault();

                        // jux 1 to execute... no unit to access except CH /CN
                        if (oChuAdminRole != null)
                        {
                            var existUserRoles = (from upr in _context.UserProfileRole.AsNoTracking()
                                                  .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && 
                                                  c.UserRoleId == oChuAdminRole.Id  && c.ProfileRoleStatus == "A") // && c.UserProfileId == _oChanges.Id -- ONLY ONE(1) Admin role per congregation (client)

                                                  // ((c.Strt == null || c.Expr == null) || (c.Strt != null && c.Expr != null && c.Strt <= DateTime.Now && c.Expr >= DateTime.Now && c.Strt <= c.Expr)))
                                                                                                                                                                                                                          // from up in roleCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                                                  select upr
                                         ).ToList();

                            var oUPR = new UserProfileRole();

                            //add SUP_ADMN role to SUP_ADMN user ... assign all privileges to the SUP_ADMN role
                            if (existUserRoles.Count() == 0)
                            {
                                //var oSupAdminRole = roleCtx.UserRole.Where(c => c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").FirstOrDefault();
                                //if (oSupAdminRole != null)
                                //{        

                                oUPR = new UserProfileRole
                                {
                                    AppGlobalOwnerId = _oChanges.AppGlobalOwnerId,
                                    ChurchBodyId = _oChanges.ChurchBodyId,
                                    UserRoleId = oChuAdminRole.Id,
                                    UserProfileId = _oChanges.Id,
                                    Strt = tm,
                                    // Expr = tm,
                                    ProfileRoleStatus = "A",
                                    Created = tm,
                                    LastMod = tm,
                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                    LastModByUserId = vm.oCurrUserId_Logged
                                };

                                _roleCtx.Add(oUPR);

                                //save user role...
                                await _roleCtx.SaveChangesAsync();

                                DetachAllEntities(_roleCtx);

                                _userTask = "Added [" + oChuAdminRole.RoleType + "] role to user, " + _oChanges.Username;
                                ViewBag.UserMsg += Environment.NewLine + " ~ [" + oChuAdminRole.RoleType + "] role added.";


                                _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));
                                // } 
                            }
                            else
                            {
                                oUPR = existUserRoles[0];  // user may have multiple roles .. CHECK
                            }


                            if (oChuAdminRole != null && oUPR != null)
                            {
                                using (var _permCtx = new MSTR_DbContext(_connstr))
                                {
                                    // assign all privileges to the ADMN role 
                                    var existUserRolePerms = (from upr in _context.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && // c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && 
                                                              c.Status == "A" && c.UserRoleId == oUPR.UserRoleId && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                              select upr).ToList();
                                    //if (existUserRolePerms.Count() > 0)
                                    //{

                                    // get only [00_] Church level Admin permissions ... 
                                    var oUserPerms = (from upr in _context.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0"))  // c.PermissionCode.StartsWith("01"))                                                                                                                                                      // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                      select upr).ToList();

                                    // check else make it on-demand by SupAdmin to update the [Role-Perm] --- defaulted pair CANNOT be deleted.
                                    // use the [RolePermRestrict] table to see less previleged roles.  
                                    if (oUserPerms.Count() > 0 && existUserRolePerms.Count() < oUserPerms.Count())
                                    {
                                        var rowUpdated = false; var rowsUpdated = 0; var rowsAdded = 0;
                                        foreach (var oURP in oUserPerms)
                                        {
                                            var existUserRolePerm = existUserRolePerms.Where(c => c.UserPermissionId == oURP.Id).FirstOrDefault();
                                            if (existUserRolePerm == null)
                                            {
                                                var oUserRolePerm = new UserRolePermission
                                                {
                                                    AppGlobalOwnerId = null, //_oChanges.AppGlobalOwnerId,   --- default [role-perm]  ... leave as configured ...Master
                                                    ChurchBodyId = null, // _oChanges.ChurchBodyId,          --- default [role-perm]  ... leave as configured ...Master
                                                    UserRoleId = oUPR.UserRoleId,
                                                    UserPermissionId = oURP.Id,
                                                    ViewPerm = true,
                                                    CreatePerm = true,
                                                    EditPerm = true,
                                                    DeletePerm = true,
                                                    ManagePerm = true,
                                                    Status = "A",
                                                    Created = tm,
                                                    LastMod = tm,
                                                    CreatedByUserId = vm.oCurrUserId_Logged,
                                                    LastModByUserId = vm.oCurrUserId_Logged
                                                };

                                                _permCtx.Add(oUserRolePerm);
                                                rowsAdded++;
                                            }
                                            else
                                            {
                                                rowUpdated = false;
                                                if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = true; rowUpdated = true; }
                                                if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = true; rowUpdated = true; }

                                                if (rowUpdated)
                                                {
                                                    existUserRolePerm.Created = tm;
                                                    existUserRolePerm.LastMod = tm;
                                                    existUserRolePerm.CreatedByUserId = vm.oCurrUserId_Logged;
                                                    existUserRolePerm.LastModByUserId = vm.oCurrUserId_Logged;

                                                    _permCtx.Update(existUserRolePerm);
                                                    rowsUpdated++;
                                                }
                                            }
                                        }

                                        // prompt users
                                        if (rowsAdded > 0)
                                        {
                                            _userTask = "Added " + rowsAdded + " user permissions to [" + oChuAdminRole.RoleType + "] role";
                                            ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user permissions added.";
                                        }
                                        if (rowsUpdated > 0)
                                        {
                                            _userTask = "Updated " + rowsUpdated + " user permissions on [" + oChuAdminRole.RoleType + "] role";
                                            ViewBag.UserMsg += ". " + rowsUpdated + " user permissions updated.";
                                        }

                                        if ((rowsAdded + rowsUpdated) > 0)
                                        {
                                            //save changes... 
                                            await _permCtx.SaveChangesAsync();

                                            DetachAllEntities(_permCtx);

                                            _tm = DateTime.Now;
                                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                              "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));

                                        }
                                    }
                                }
                            }
                        }
                    }
                 
                }
                          

         

                // oCM_NewConvert.Created = DateTime.Now;
                // _context.Add(_oChanges);

               

                //user roles / user groups and/or user permissions [ tick ... pick from the attendance concept]
                //var oMemRoles = currCtx.MemberChurchRole.Include(t => t.LeaderRole).ThenInclude(t => t.LeaderRoleCategory)
                //    .Where(c => c.LeaderRole.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrServing == true).ToList();
                //foreach (var oMR in oMemRoles)
                //{
                //    oMR.IsCurrServing = false;
                //    oMR.Completed = oChuTransf.TransferDate;
                //    oMR.CompletionReason = oChuTransf.TransferType;
                //    oMR.LastMod = DateTime.Now;
                //    //
                //    currCtx.Update(oMR);
                //}
                //ViewBag.UserMsg += " Church visitor added to church as New Convert successfully. Update of member details may however be required."  


                //save everything
                // await _context.SaveChangesAsync();

                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurr"] = _vmMod; TempData.Keep();

                

                //if (_oChanges.PwdExpr != null)
                //{
                //    if (_oChanges.PwdExpr.Value >= DateTime.Now.Date)
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please user password has expired. Check email/phone for confirm code to activate password.", signOutToLogIn = true  });
                //}

                // return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = false });
                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving user profile details. Contact System Admin for assistance. Err: " + ex.Message, signOutToLogIn = false });
            }
        }

         

        public IActionResult Delete_UP(int? oAppGloOwnId, int? oCurrChuBodyId, int ? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
        {
            // var strDesc = setIndex == 1 ? "System profile" : setIndex == 2 ? "Church admin profile" : "Church user profile";
            var strDesc = (setIndex == 1 ? "System admin profile" : (setIndex == 2 ? "Church admin profile" : "Church user profile"));
            var tm = DateTime.Now; var _tm = DateTime.Now; var _userTask = "Attempted saving  " + strDesc;

             ///this._configuration["ConnectionStrings:DefaultConnection"];
            //
            try
            {
                var strUserDenom = "Vendor Admin";
                if (setIndex != 1 )
                {
                    if (oAppGloOwnId == null || oCurrChuBodyId == null) 
                        return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Denomination/church of " + strDesc + " unknown. Please refesh and try again." });

                    var oAGO = _context.MSTRAppGlobalOwner.Find(oAppGloOwnId);
                    var oCB = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();

                    if (oAGO == null || oCB == null)
                        return Json(new { taskSuccess = false, oCurrId = "", userMess = "Specified denomination and church unit could not be retrieved. Please refresh and try again.", signOutToLogIn = false });

                    strUserDenom = oCB.Name + (!string.IsNullOrEmpty(oAGO.Acronym) ? ", " + oAGO.Acronym : oAGO.OwnerName);
                    strUserDenom = (!string.IsNullOrEmpty(strUserDenom) ? "Denomination: " + strUserDenom : strUserDenom);//"--" + 
                }
                    

                var oUser = _context.UserProfile.AsNoTracking().Where(c => c.Id == id && 
                                               ((setIndex == 1 && oAppGloOwnId == null && oCurrChuBodyId == null && c.ProfileScope=="V") || 
                                                (setIndex != 1 && c.AppGlobalOwnerId==oAppGloOwnId && c.ChurchBodyId==oCurrChuBodyId && c.ProfileScope=="C"))).FirstOrDefault();// .Include(c => c.ChurchUnits)
                if (oUser == null)
                {
                    _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oUser.UserDesc + " [" + oUser.Username + "]" + strUserDenom;  // var _userTask = "Attempted saving  " + strDesc;
                    _tm = DateTime.Now;
                    _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " to delete could not be retrieved." });
                }

                if (string.Compare(oUser.Username.Trim(), "sys", true) == 0)
                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " [SYS] cannot be deleted: it's a system profile." });

                if (string.Compare(oUser.Username.Trim(), "sys", true) == 0)
                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = strDesc + " [SYS] cannot be deleted: it's a system profile." });

                var saveDelete = true;
                // ensuring cascade delete where there's none!

                //check oUPRs, UPGs, UATs for this UP to delete
                //var oUPRs = _context.UserProfileRole.Where(c => c.UserProfileId == oUser.Id).ToList();    // .... cascade delete together with the roles, groups / permissions assigned
                //var UPGs = _context.UserProfileGroup.Where(c => c.UserProfileId == oUser.Id).ToList();

               var UATs = _context.UserAuditTrail.AsNoTracking().Where(c => c.UserProfileId == oUser.Id).ToList();  // clear the UAT log to allow deletion of user

                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);
                if (!string.IsNullOrEmpty(_cs))
                {
                    using (var _userCtx = new MSTR_DbContext(_cs))
                    {
                        if (UATs.Count() > 0) // + UPGs.Count() + oUPRs.Count() //+oUser.ChurchMembers.Count )
                        {
                            if (forceDeleteConfirm == false)
                            {
                                var strConnTabs = "User audit trail";  //User profile role, User profile group and 
                                saveDelete = false;

                                // check user privileges to determine... administrator rights
                                //log...
                                _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oUser.UserDesc + " [" + oUser.Username + "]" + strUserDenom;
                                _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                 "RCMS-Admin:  " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                                return Json(new
                                {
                                    taskSuccess = false,
                                    tryForceDelete = false,
                                    oCurrId = id,
                                    userMess = "Specified " + strDesc.ToLower() +
                                                    " to delete has been used elsewhere in the system [" + strConnTabs + "]. Delete cannot be done unless dependent-references are removed."
                                });
                            }

                            //to be executed only for higher privileges... // FORCE-DELETE...
                        }

                        //successful...
                        if (saveDelete)
                        {
                            _userCtx.UserProfile.Remove(oUser);
                            _userCtx.SaveChanges();

                            DetachAllEntities(_userCtx);

                            //audit...
                            _userTask = "Deleted " + strDesc.ToLower() + ", " + oUser.UserDesc + " [" + oUser.Username + "]" + strUserDenom;
                            _tm = DateTime.Now;
                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS-Admin:  " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                            return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oUser.Id, userMess = strDesc + " successfully deleted." });
                        }

                    }

                }


                _userTask = "Attempted deleting " + strDesc.ToLower() + "," + oUser.UserDesc + "[" + oUser.Username + "]" + strUserDenom + " -- but FAILED. Data unavailable.";   // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" });
            }

            catch (Exception ex)
            {
                _userTask = "Attempted deleting " + strDesc.ToLower() + ", [ ID= " + id + "] FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId)); 
                //
                return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
            }
        }




        private async Task<bool> Configure_CL_UPR(List<UserProfileRole> lsUserProfileRoles, int? oAGOid, int? oCBid, int? oLoggedUserId)
        {
            //List<UserProfileRole> lsUserProfileRoles = null;
            var strDesc = "User role permissions";
            //if (!InitializeUserLogging())
            //    return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data unavailable. Please refresh and try again.", pageIndex = 0 });  //RedirectToAction("LoginUserAcc", "UserLogin");

            // var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); 

            if (lsUserProfileRoles == null)
            {
                ViewBag.strResUPR_CL_Task = strDesc + " data to update unavailable. Please refresh and try again.";
                return false;
            }
            if (lsUserProfileRoles.Count == 0)
            {
                ViewBag.strResUPR_CL_Task = "No data unavailable. Add " + strDesc + " data and try again.";
                return false;
            }


            try
            {

                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);
                if (!string.IsNullOrEmpty(_cs))
                {
                    using (var _masCtx_UPR = new MSTR_DbContext(_cs))
                    {
                        var tm = DateTime.Now;
                        var _userTask = "";

                        //// ADD ROLES... add non-exist but checked, update... remove unchecked roles                 
                        //var rowsUpdated = 0; var rowsAdded = 0; var rowsDeleted = 0;
                        //UserProfile oUserProfile = null;

                        var masterUserProfileRoleList = new List<UserProfileRole>();
                        if (lsUserProfileRoles.Count > 0)
                        {
                            /// get all roles incl custom roles
                            var masterUserRoleList = _masCtx_UPR.UserRole.AsNoTracking()   //.Include(t => t.UserRole)
                                .Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || (c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid))).ToList();

                            for (var i = 0; i < lsUserProfileRoles.Count; i++)
                            {
                                lsUserProfileRoles[i].UserRole = masterUserRoleList.Where(c => c.Id == lsUserProfileRoles[i].UserRoleId).FirstOrDefault();

                                ////load one-time
                                //if (oUserProfile == null) oUserProfile = _masCtx_UPR.UserProfile.Include(t => t.ChurchBody)
                                //        .Where(c => c.AppGlobalOwnerId == lsUserProfileRoles[i].AppGlobalOwnerId && c.ChurchBodyId == lsUserProfileRoles[i].ChurchBodyId &&
                                //                    c.Id == lsUserProfileRoles[i].UserProfileId).FirstOrDefault();
                            }

                            //// affirm to client roles... in case
                            //lsUserProfileRoles = lsUserProfileRoles.Where(c => ((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") &&
                            //                                                    c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                            //                                                   (oUserProfile.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15)).ToList();
                        }

                        //masterUserProfileRoleList = _masCtx_UPR.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                        //                            .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid).ToList();

                        //foreach (var oUPR in lsUserProfileRoles)
                        //{
                        //    var existUPR = masterUserProfileRoleList.Where(c => c.UserRoleId == oUPR.UserRoleId && c.UserProfileId == oUPR.UserProfileId).FirstOrDefault();

                        //    if (existUPR == null && oUPR.isRoleAssigned)
                        //    {
                        //        var oUPRAdded = new UserProfileRole
                        //        {
                        //            AppGlobalOwnerId = oAGOid,
                        //            ChurchBodyId = oCBid,
                        //            UserRoleId = oUPR.UserRoleId,
                        //            UserProfileId = oUPR.UserProfileId,
                        //            Strt = tm,
                        //            Expr = (DateTime?)null,
                        //            ProfileRoleStatus = "A",
                        //            Created = tm,
                        //            LastMod = tm,
                        //            CreatedByUserId = oLoggedUserId,
                        //            LastModByUserId = oLoggedUserId,
                        //        };

                        //        _masCtx_UPR.Add(oUPRAdded);
                        //        rowsAdded++;
                        //    }

                        //    else if (existUPR != null)
                        //    {
                        //        if (!oUPR.isRoleAssigned)
                        //        {
                        //            // unchecked.. delete
                        //            _masCtx_UPR.Remove(existUPR);
                        //            rowsDeleted++;
                        //        }

                        //        else
                        //        {
                        //            //existUPR.AppGlobalOwnerId = oAGOid;
                        //            //existUPR.ChurchBodyId = oCBid;
                        //            existUPR.UserRoleId = oUPR.UserRoleId;
                        //            existUPR.UserProfileId = oUPR.UserProfileId;
                        //            //existUPR.Strt = oUPR.Strt;
                        //            //existUPR.Expr = oUPR.Expr;
                        //            //existUPR.ProfileRoleStatus = oUPR.ProfileRoleStatus;
                        //            //existUPR.Created = tm;
                        //            existUPR.LastMod = tm;
                        //            //existUPR.CreatedByUserId = oLoggedUserId;
                        //            existUPR.LastModByUserId = oLoggedUserId; /// oLoggedUserId;

                        //            _masCtx_UPR.Update(existUPR);
                        //            rowsUpdated++;
                        //        }
                        //    }
                        //}



                        //if (oUserProfile != null)
                        //{

                        //var _userTask = "user"; var strUserProfile = oUserProfile.UserDesc;

                        //// prompt users
                        //if (rowsAdded > 0)
                        //{
                        //    _userTask = "Added " + rowsAdded + " user roles to [" + strUserProfile + "] profile";
                        //    ViewBag.strResUPR_CL_Task += Environment.NewLine + " ~ " + rowsAdded + " user roles added.";
                        //}
                        //if (rowsUpdated > 0)
                        //{
                        //    _userTask = "Updated " + rowsUpdated + " user roles on [" + strUserProfile + "] profile";
                        //    ViewBag.strResUPR_CL_Task += ". " + rowsUpdated + " user roles updated.";
                        //}
                        //if (rowsDeleted > 0)
                        //{
                        //    _userTask = "Deleted " + rowsDeleted + " user roles from [" + strUserProfile + "] profile";
                        //    ViewBag.strResUPR_CL_Task += ". " + rowsDeleted + " user roles deleted.";
                        //}

                        //if ((rowsAdded + rowsUpdated + rowsDeleted) > 0)
                        //{
                        //    //save changes... 
                        //   // _masCtx_UPR.SaveChanges();

                        //    _masCtx_UPR.SaveChanges();

                        //    DetachAllEntities(_masCtx_UPR);

                        //    var _tm = DateTime.Now;
                        //    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                        //                      "RCMS-Admin: User Profile Role", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oLoggedUserId, _tm, _tm, oLoggedUserId, oLoggedUserId));
                        //}


                        //// ADD PERMISSIONS ON THE ASSIGNED ROLES 
                        ///  

                        /// actually... client only access to the URP is via custom roles ONLY ... standard roles MUST BE SET BY THE VENDOR...
                        /// But for now... allow jux first timer roles in... and that's it!

                        var loggedUserId = oLoggedUserId;

                            //var oUPRList = _masCtx_UPR.UserProfileRole.Include(t => t.UserRole).AsNoTracking()
                            //    .Where(c => c.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId && c.UserProfileId == oUserProfile.Id &&
                            //    (((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                            //     (oUserProfile.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15)))
                            //    .ToList();


                            var oUPMList_MSTR = _masCtx_UPR.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0")).ToList();
                            var oUPM_PermListCurr = new List<UserPermission>();
                            /// 
                            //var lsRows_UPR = oUPRList;    //  if (oUPRList.Count > 0) 
                            var cntURPChanges_Add = 0; var cntURPChanges_Upd = 0; var isRowUpd = false;
                            var totURPChanges_Add = 0; var totURPChanges_Upd = 0; var updRoleIdList = new List<string>();
                            foreach (var oUPR in lsUserProfileRoles)  /// oUPRList)
                            {
                                if (oUPR.UserRole != null && !updRoleIdList.Contains((oUPR.UserRole != null ? oUPR.UserRole.RoleType : null)))
                                {
                                    updRoleIdList.Add(oUPR.UserRole.RoleType);

                                    //// get perm list based on role...   
                                    //if (oUPR.UserRole.RoleLevel == 1 && oUPR.UserRole.RoleType == "SYS")
                                    //    oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode == "A0_00" || c.PermissionCode == "A0_01").ToList();  // SYS [2]
                                    //else if (oUPR.UserRole.RoleLevel == 2 && oUPR.UserRole.RoleType == "SUP_ADMN")
                                    //    oUPM_PermListCurr = oUPMList_MSTR.Where(c => (c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00") || !c.PermissionCode.StartsWith("A0")).ToList();  // SUP_ADMN [12 + 69]
                                    //else if (oUPR.UserRole.RoleLevel == 3 && oUPR.UserRole.RoleType == "SYS_ADMN")
                                    //    oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00" && c.PermissionCode != "A0_01" && c.PermissionCode != "A0_04").ToList();  // SYS_ADMN [9]
                                    //else if (oUPR.UserRole.RoleLevel == 4 && oUPR.UserRole.RoleType == "SYS_CL_ADMN")
                                    //    oUPM_PermListCurr = oUPMList_MSTR.Where(c => (c.PermissionCode.StartsWith("A0") && c.PermissionCode != "A0_00" && c.PermissionCode != "A0_01" && c.PermissionCode != "A0_02") || !c.PermissionCode.StartsWith("A0")).ToList();  // SYS_CL_ADMN [9 + 69]

                                    ///
                                    if (oUPR.UserRole.RoleLevel == 6 && oUPR.UserRole.RoleType == "CH_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => !c.PermissionCode.StartsWith("A0")).ToList();  // CH_ADMN [6] ...
                                    else if (oUPR.UserRole.RoleLevel == 7 && oUPR.UserRole.RoleType == "CH_MGR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [7]
                                    else if (oUPR.UserRole.RoleLevel == 8 && oUPR.UserRole.RoleType == "CH_EXC")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [8]
                                    else if (oUPR.UserRole.RoleLevel == 9 && oUPR.UserRole.RoleType == "CH_RGSTR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [9]                                
                                    else if (oUPR.UserRole.RoleLevel == 10 && oUPR.UserRole.RoleType == "CH_ACCT")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("05") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [10] 
                                    /// 
                                    else if (oUPR.UserRole.RoleLevel == 11 && oUPR.UserRole.RoleType == "CF_ADMN")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => !c.PermissionCode.StartsWith("A0")).ToList();  // CF_ADMN [11] ...
                                    else if (oUPR.UserRole.RoleLevel == 12 && oUPR.UserRole.RoleType == "CF_MGR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [7]
                                    else if (oUPR.UserRole.RoleLevel == 13 && oUPR.UserRole.RoleType == "CF_EXC")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("03") || c.PermissionCode.StartsWith("04") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [8]
                                    else if (oUPR.UserRole.RoleLevel == 14 && oUPR.UserRole.RoleType == "CF_RGSTR")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("02") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [9]                                
                                    else if (oUPR.UserRole.RoleLevel == 15 && oUPR.UserRole.RoleType == "CF_ACCT")
                                        oUPM_PermListCurr = oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00") || c.PermissionCode.StartsWith("05") || c.PermissionCode.StartsWith("06")).ToList();  // CH_RGSTR [10] 

                                    /// 

                                    //get dash and RA
                                    // oUPM_PermListCurr.AddRange(oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("00")).ToList());  // possible it might av been added via previous...
                                    //  oUPM_PermListCurr.AddRange(oUPMList_MSTR.Where(c => c.PermissionCode.StartsWith("06")).ToList());  // possible it might av been added via previous...

                                    foreach (var oUPMCurr in oUPM_PermListCurr) //var i = 0; i < userPermList.Count; i++)
                                    {
                                        var oURPExist = _masCtx_UPR.UserRolePermission.AsNoTracking().Where(c => c.UserRoleId == oUPR.UserRoleId &&
                                                                                    (c.UserPermissionId == oUPMCurr.Id || c.UserPermission.PermissionCode == oUPMCurr.PermissionCode)).FirstOrDefault();
                                        if (oURPExist == null)
                                        {
                                            var _allowCruD = (oUPR.UserRole.RoleLevel == 1 && oUPMCurr.PermissionCode != "A0_00") || (oUPR.UserRole.RoleLevel == 2 && oUPMCurr.PermissionCode != "A0_01") || (oUPR.UserRole.RoleLevel == 3 && oUPMCurr.PermissionCode != "A0_02") || (oUPR.UserRole.RoleLevel == 4 && oUPMCurr.PermissionCode != "A0_04") ||
                                                            ((oUPR.UserRole.RoleLevel >= 6 || oUPR.UserRole.RoleLevel <= 15) && !oUPMCurr.PermissionCode.StartsWith("A0"));
                                            ///
                                            _masCtx_UPR.Add(new UserRolePermission()
                                            {
                                                ChurchBodyId = null,
                                                UserPermissionId = oUPMCurr.Id,
                                                UserRoleId = oUPR.UserRoleId,
                                                Status = "A",
                                                ViewPerm = true,
                                                CreatePerm = _allowCruD,
                                                EditPerm = true,
                                                DeletePerm = _allowCruD,
                                                ManagePerm = true,
                                                Created = tm,
                                                LastMod = tm,
                                                CreatedByUserId = loggedUserId,
                                                LastModByUserId = loggedUserId
                                            });

                                            cntURPChanges_Add++;
                                        }
                                        else
                                        {
                                            var _allowCruD = (oUPR.UserRole.RoleLevel == 1 && oUPMCurr.PermissionCode != "A0_00") || (oUPR.UserRole.RoleLevel == 2 && oUPMCurr.PermissionCode != "A0_01") || (oUPR.UserRole.RoleLevel == 3 && oUPMCurr.PermissionCode != "A0_02") || (oUPR.UserRole.RoleLevel == 4 && oUPMCurr.PermissionCode != "A0_04") ||
                                                            ((oUPR.UserRole.RoleLevel >= 6 || oUPR.UserRole.RoleLevel <= 15) && !oUPMCurr.PermissionCode.StartsWith("A0"));
                                            ///
                                            isRowUpd = false;
                                            if (!oURPExist.ViewPerm) { oURPExist.ViewPerm = true; isRowUpd = true; }
                                            if (!oURPExist.CreatePerm) { oURPExist.CreatePerm = _allowCruD; isRowUpd = true; }
                                            if (!oURPExist.EditPerm) { oURPExist.EditPerm = true; isRowUpd = true; }
                                            if (!oURPExist.DeletePerm) { oURPExist.DeletePerm = _allowCruD; isRowUpd = true; }
                                            if (!oURPExist.ManagePerm) { oURPExist.ManagePerm = true; isRowUpd = true; }

                                            if (isRowUpd)
                                            {
                                                oURPExist.Created = tm;
                                                oURPExist.LastMod = tm;
                                                oURPExist.CreatedByUserId = loggedUserId;
                                                oURPExist.LastModByUserId = loggedUserId;

                                                _masCtx_UPR.Update(oURPExist);
                                                cntURPChanges_Upd++;
                                            }
                                        }
                                    }

                                    // update....
                                    if ((cntURPChanges_Add + cntURPChanges_Upd) > 0)
                                    {
                                         
                                        await _masCtx_UPR.SaveChangesAsync();

                                        DetachAllEntities(_masCtx_UPR);
                                        ///
                                        totURPChanges_Add += cntURPChanges_Add;
                                        totURPChanges_Upd += cntURPChanges_Upd;
                                    }

                                    ///
                                    // init...
                                    cntURPChanges_Add = 0; cntURPChanges_Upd = 0;
                                }
                            }


                            // logoutCurrUser = _URPChanges > 0;   
                            if ((totURPChanges_Add + totURPChanges_Upd) > 0)
                            {
                                if (totURPChanges_Add > 0)
                                    ViewBag.strResUPR_CL_Task += Environment.NewLine + " ~ " + totURPChanges_Add + " user permissions added.";
                                if (totURPChanges_Upd > 0)
                                    ViewBag.strResUPR_CL_Task += ". " + totURPChanges_Upd + " user permissions updated.";
                                ///

                                var _tm = DateTime.Now;
                                _userTask = "Assigned " + totURPChanges_Add + ", updated [" + totURPChanges_Upd + "] permission(s) to [available] roles successfully"; _tm = DateTime.Now;
                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                        "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                                
                                ViewBag.UserMsg_UPR  += " " + ((totURPChanges_Add + totURPChanges_Upd) > 0 ? (totURPChanges_Add + totURPChanges_Upd) + " permission(s). " : "");

                            }

                            /// if (string.IsNullOrEmpty(_userTask)) _userTask = "No update made on user profile."; 
                       /// } 
                    }




                    // promtp user    ... ViewBag.strResUPR_CL_Task
                    return true;  /// Json(new { taskSuccess = true, oCurrId = 0, userMess = ViewBag.UserMsg, signOutToLogIn = false });
                }


                ///fail... 
                ViewBag.strResUPR_CL_Task = "Failed saving user profile roles. Client DB connection unsuccessful.";
                return false;  /// Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile roles. User profile not found.", signOutToLogIn = false });

            }

            catch (Exception ex)
            {
                ViewBag.strResUPR_CL_Task = "Failed saving user profile roles. Err: " + ex.Message;
                return false;  /// Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile roles. Err: " + ex.Message, signOutToLogIn = false });
            } 
        }




        [HttpGet]
        public IActionResult GetTargetCB(int? oAppGloOwnId = null, int? oChurchBodyId = null, int? oTargetCBId = null,
                                        string strId_TCB = "", string strName_TCB = "", string strCLId_TCB = "", string strCLName_TCB = "", string strCLTag_TCB = "")
        {
            try
            {
                var oCBModel = new TargetCBModel();
                oCBModel.oAppGloOwnId = oAppGloOwnId;     // current AGO of request
                oCBModel.oChurchBodyId = oChurchBodyId;  // current CB of request 
                ///
                oCBModel.oTargetCBId = oTargetCBId;     // target CB of request == initial CB ...  
                var _oTargetCBId = oTargetCBId;

                oCBModel.oTargetCLId = null; // oTargetCLId;  oCBModel.oTargetCLId = oCBModel.oTargetCB.ChurchLevelId;

                ///
                oCBModel.strId_TCB = strId_TCB != null ? strId_TCB : "";
                oCBModel.strName_TCB = strName_TCB != null ? strName_TCB : "";
                oCBModel.strCLId_TCB = strCLId_TCB != null ? strCLId_TCB : "";
                oCBModel.strCLName_TCB = strCLName_TCB != null ? strCLName_TCB : "";
                oCBModel.strCLTag_TCB = strCLTag_TCB != null ? strCLTag_TCB : "Church Body";
                ///
                if (oCBModel.oAppGloOwnId == null)
                    return View("_ErrorPage");  // || oCBModel.oChurchBodyId == null current denom and cong cannot be null

                //if (oCBModel.oTargetCBId == null) oCBModel.oTargetCBId = oCBModel.oChurchBodyId;
                ///
                oCBModel.oTargetCB_MSTR = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel) //.Include(t => t.ParentChurchBody)
                                    .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.Id == oCBModel.oTargetCBId)
                                    .FirstOrDefault();

                //if (oCBModel.oTargetCB != null ) return View("_ErrorPage");
                oCBModel.numChurchLevel_Index = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId); //oCBModel.oTargetCB.ChurchLevel.LevelIndex;
                oCBModel.strTargetCB = "None. (choose church body)"; oCBModel.strTargetCL = "";
                oCBModel.arrRootCBIds = new List<object>();
                oCBModel.arrRootCBNames = new List<string>();
                for (int k = 0; k < 10; k++)
                {
                    oCBModel.arrRootCBIds.Add((int?)null); // initialize the template
                    oCBModel.arrRootCBNames.Add(""); // initialize the template 
                }

                //init... all
                oCBModel.strChurchLevel_1 = ""; oCBModel.strChurchLevel_2 = ""; oCBModel.strChurchLevel_3 = ""; oCBModel.strChurchLevel_4 = ""; oCBModel.strChurchLevel_5 = "";
                oCBModel.strChurchLevel_6 = ""; oCBModel.strChurchLevel_7 = ""; oCBModel.strChurchLevel_8 = ""; oCBModel.strChurchLevel_9 = ""; oCBModel.strChurchLevel_10 = "";
                ///
                ViewBag.strChurchLevel_1 = oCBModel.strChurchLevel_1; ViewBag.strChurchLevel_2 = oCBModel.strChurchLevel_2; ViewBag.strChurchLevel_3 = oCBModel.strChurchLevel_3; ViewBag.strChurchLevel_4 = oCBModel.strChurchLevel_4; ViewBag.strChurchLevel_5 = oCBModel.strChurchLevel_5;
                ViewBag.strChurchLevel_6 = oCBModel.strChurchLevel_6; ViewBag.strChurchLevel_7 = oCBModel.strChurchLevel_7; ViewBag.strChurchLevel_8 = oCBModel.strChurchLevel_8; ViewBag.strChurchLevel_9 = oCBModel.strChurchLevel_9; ViewBag.strChurchLevel_10 = oCBModel.strChurchLevel_10;

                ///// pre-load the list...
                //if (_oTargetCBId != null)
                //{
                //    oCBModel.strTargetCB = oCBModel.oTargetCB_MSTR != null ? oCBModel.oTargetCB_MSTR.Name : "";
                //    oCBModel.strTargetCL = !string.IsNullOrEmpty(oCBModel.oTargetCB_MSTR.ChurchLevel.CustomName) ? oCBModel.oTargetCB_MSTR.ChurchLevel.CustomName : oCBModel.oTargetCB_MSTR.ChurchLevel.Name;

                /// pre-load the list...
                if (_oTargetCBId != null && oCBModel.oTargetCB_MSTR != null)
                {
                    oCBModel.oTargetCLId = oCBModel.oTargetCB_MSTR.ChurchLevelId;
                    oCBModel.strTargetCB = oCBModel.oTargetCB_MSTR != null ? oCBModel.oTargetCB_MSTR.Name : ""; //oCBModel.oTargetCB.Name;
                    oCBModel.strTargetCL = !string.IsNullOrEmpty(oCBModel.oTargetCB_MSTR.ChurchLevel.CustomName) ? oCBModel.oTargetCB_MSTR.ChurchLevel.CustomName : oCBModel.oTargetCB_MSTR.ChurchLevel.Name;

                    oCBModel.arrRootCBCodes = oCBModel.oTargetCB_MSTR != null ? oCBModel.oTargetCB_MSTR.RootChurchCode.Split("--").ToList<string>() : new List<string>();
                    // oCBModel.oTargetCLId = currCB.ChurchLevelId;

                    //// get the CB path... use either ChurchCode [must av bn ordered! else trouble..] or CB Id to trace path...
                    //var masterCBList = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Include(t => t.ChurchLevel)
                    //                    .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId &&
                    //                    (oCBModel.oTargetCB_MSTR == null || (oCBModel.oTargetCB_MSTR != null && oCBModel.arrRootCBCodes.Contains(c.GlobalChurchCode))))
                    //                    .ToList();

                    var masterCBList = (from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                                                   .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId &&
                                                              (oCBModel.oTargetCB_MSTR == null || (oCBModel.oTargetCB_MSTR != null && oCBModel.arrRootCBCodes.Contains(c.GlobalChurchCode))))
                                        from t_cb_p in _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                                                   .Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb.ParentChurchBodyId).DefaultIfEmpty()
                                        select new MSTRChurchBody()
                                        {
                                            Id = t_cb.Id,
                                            AppGlobalOwnerId = t_cb.AppGlobalOwnerId,
                                            ChurchLevelId = t_cb.ChurchLevelId,
                                            Name = t_cb.Name,
                                            GlobalChurchCode = t_cb.GlobalChurchCode,
                                            RootChurchCode = t_cb.RootChurchCode,
                                            OrgType = t_cb.OrgType,
                                            ParentChurchBodyId = t_cb.ParentChurchBodyId,
                                            ContactInfoId = t_cb.ContactInfoId,
                                            IsWaiveSubscription = t_cb.IsWaiveSubscription,
                                            SubscriptionKey = t_cb.SubscriptionKey,
                                            LicenseKey = t_cb.LicenseKey,
                                            CtryAlpha3Code = t_cb.CtryAlpha3Code,
                                            CountryRegionId = t_cb.CountryRegionId,
                                            Comments = t_cb.Comments,
                                            ChurchWorkStatus = t_cb.ChurchWorkStatus,
                                            Status = t_cb.Status,
                                            ///
                                            ParentChurchBody = t_cb_p,
                                        })
                                        .ToList();

                    ///
                    var listCount = 0;
                    MSTRChurchBody oCBNextParent = oCBModel.oTargetCB_MSTR;   // initial CB
                    if (masterCBList.Count > 1)   // leave the root... to append later
                    {                        
                        if (oCBNextParent == null) oCBNextParent = masterCBList.Where(c => c.Id == oCBModel.oTargetCB_MSTR.Id && c.GlobalChurchCode == oCBModel.oTargetCB_MSTR.GlobalChurchCode).FirstOrDefault();
                                         
                        if (oCBNextParent != null)
                        {
                            if (oCBNextParent.ChurchLevel == null) oCBNextParent.ChurchLevel = _context.MSTRChurchLevel.AsNoTracking().Where(c=>c.AppGlobalOwnerId== oCBNextParent.AppGlobalOwnerId && c.Id == oCBNextParent.ChurchLevelId).FirstOrDefault();
                            ///
                            if (oCBNextParent.ChurchLevel != null)
                            {
                                listCount = oCBNextParent.ChurchLevel.LevelIndex; // (oCBNextParent.ChurchLevel != null ? oCBNextParent.ChurchLevel.LevelIndex : listCount);
                                listCount = listCount - 1;
                                oCBModel.arrRootCBIds[listCount] = oCBNextParent.Id;
                                oCBModel.arrRootCBNames[listCount] = oCBNextParent.Name;
                            } 
                        }
                    }

                    for (int i = listCount - 1; i > 0; i--)
                    {
                        if (oCBNextParent.ParentChurchBody == null)
                            oCBNextParent.ParentChurchBody = _context.MSTRChurchBody.AsNoTracking()
                                .Where(c => c.AppGlobalOwnerId == oCBNextParent.AppGlobalOwnerId && c.Id == oCBNextParent.ParentChurchBodyId).FirstOrDefault();

                        var oNextCC = "";
                        if (oCBNextParent.ParentChurchBody != null) oNextCC = oCBNextParent.ParentChurchBody.GlobalChurchCode;
                        ///
                        oCBNextParent = masterCBList.Where(c => c.Id == oCBNextParent.ParentChurchBodyId && c.GlobalChurchCode == oNextCC).FirstOrDefault();
                        if (oCBNextParent == null) break; // loop out
                       
                        oCBModel.arrRootCBIds[i] = oCBNextParent.Id;
                        oCBModel.arrRootCBNames[i] = oCBNextParent.Name;                        
                    }

                    // add the root
                    oCBNextParent = masterCBList.Where(c => c.OrgType == "CR" && c.ParentChurchBodyId == null).FirstOrDefault();
                    if (oCBNextParent != null)
                    {
                        oCBModel.arrRootCBIds[0] = oCBNextParent.Id;  // base CB usually the name of the church -- CR
                        oCBModel.arrRootCBNames[0] = oCBNextParent.Name + " (Church Head)";  // base CB usually the name of the church -- CR
                    }
                }

                else
                {
                    // add the root
                    var oCBNextParent = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)  // .Include(t => t.ParentChurchBody)
                                        .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.OrgType == "CR" && c.ParentChurchBodyId == null).FirstOrDefault();
                    if (oCBNextParent != null)
                    {
                        oCBModel.arrRootCBIds[0] = oCBNextParent.Id;  // base CB usually the name of the church -- CR
                        oCBModel.arrRootCBNames[0] = oCBNextParent.Name + " (Church Head)";  // base CB usually the name of the church -- CR
                    }
                }


                oCBModel.oCBLevelCount = oCBModel.numChurchLevel_Index; // - 1;        // oCBLevelCount -= 2;  // less requesting CB
                List<MSTRChurchLevel> oCBLevelList = _context.MSTRChurchLevel.AsNoTracking()
                    .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.LevelIndex > 0 && c.LevelIndex <= oCBModel.numChurchLevel_Index)
                    .ToList().OrderBy(c => c.LevelIndex).ToList();
                ///
                if (oCBModel.oCBLevelCount > 0 && oCBLevelList.Count > 0)
                {
                    oCBModel.strChurchLevel_1 = !string.IsNullOrEmpty(oCBLevelList[0].CustomName) ? oCBLevelList[0].CustomName : oCBLevelList[0].Name;
                    ViewBag.strChurchLevel_1 = oCBModel.strChurchLevel_1;

                    if (oCBModel.oCBLevelCount > 1)
                    {
                        oCBModel.strChurchLevel_2 = !string.IsNullOrEmpty(oCBLevelList[1].CustomName) ? oCBLevelList[1].CustomName : oCBLevelList[1].Name;
                        ViewBag.strChurchLevel_2 = oCBModel.strChurchLevel_2;

                        if (oCBModel.oCBLevelCount > 2)
                        {
                            oCBModel.strChurchLevel_3 = !string.IsNullOrEmpty(oCBLevelList[2].CustomName) ? oCBLevelList[2].CustomName : oCBLevelList[2].Name;
                            ViewBag.strChurchLevel_3 = oCBModel.strChurchLevel_3;

                            if (oCBModel.oCBLevelCount > 3)
                            {
                                oCBModel.strChurchLevel_4 = !string.IsNullOrEmpty(oCBLevelList[3].CustomName) ? oCBLevelList[3].CustomName : oCBLevelList[3].Name;
                                ViewBag.strChurchLevel_4 = oCBModel.strChurchLevel_4;

                                if (oCBModel.oCBLevelCount > 4)
                                {
                                    oCBModel.strChurchLevel_5 = !string.IsNullOrEmpty(oCBLevelList[4].CustomName) ? oCBLevelList[4].CustomName : oCBLevelList[4].Name;
                                    ViewBag.strChurchLevel_5 = oCBModel.strChurchLevel_5;

                                    if (oCBModel.oCBLevelCount > 5)
                                    {
                                        oCBModel.strChurchLevel_6 = !string.IsNullOrEmpty(oCBLevelList[5].CustomName) ? oCBLevelList[5].CustomName : oCBLevelList[5].Name;
                                        ViewBag.strChurchLevel_6 = oCBModel.strChurchLevel_6;

                                        if (oCBModel.oCBLevelCount > 6)
                                        {
                                            oCBModel.strChurchLevel_7 = !string.IsNullOrEmpty(oCBLevelList[6].CustomName) ? oCBLevelList[6].CustomName : oCBLevelList[6].Name;
                                            ViewBag.strChurchLevel_7 = oCBModel.strChurchLevel_7;

                                            if (oCBModel.oCBLevelCount > 7)
                                            {
                                                oCBModel.strChurchLevel_8 = !string.IsNullOrEmpty(oCBLevelList[7].CustomName) ? oCBLevelList[7].CustomName : oCBLevelList[7].Name;
                                                ViewBag.strChurchLevel_8 = oCBModel.strChurchLevel_8;

                                                if (oCBModel.oCBLevelCount > 8)
                                                {
                                                    oCBModel.strChurchLevel_9 = !string.IsNullOrEmpty(oCBLevelList[8].CustomName) ? oCBLevelList[8].CustomName : oCBLevelList[8].Name;
                                                    ViewBag.strChurchLevel_9 = oCBModel.strChurchLevel_9;

                                                    if (oCBModel.oCBLevelCount > 9)
                                                    {
                                                        oCBModel.strChurchLevel_10 = !string.IsNullOrEmpty(oCBLevelList[9].CustomName) ? oCBLevelList[9].CustomName : oCBLevelList[9].Name;
                                                        ViewBag.strChurchLevel_10 = oCBModel.strChurchLevel_10;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                //oCBModel.oUserId_Logged = this._oLoggedUser.Id;
                //oCBModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                //oCBModel.oChurchBodyId_Logged = this._oLoggedCB.Id;

                /// load lookups
                // oCUModel = this.popLookups_CU(oCUModel, oCUModel.oChurchUnit);

                //var tm = DateTime.Now;
                //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, null, null, "T",
                //                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
                //    , this._clientDBConnString);

                //var _oCUModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCUModel);
                //TempData["oVmCurrMod"] = _oCUModel; TempData.Keep();

                oCBModel.lkpChurchLevels = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId)
                                          .OrderByDescending(c => c.LevelIndex)
                                          .Select(c => new SelectListItem()
                                          {
                                              Value = c.Id.ToString(),
                                              Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name,
                                              // Disabled = (numCLIndex == (int?)null || c.LevelIndex < numCLIndex || oCurrChurchBody.OrgType == "CH" || oCurrChurchBody.OrgType == "CN")
                                          })
                                          .ToList();

                return PartialView("_GetTargetCB", oCBModel);

            }

            catch (Exception ex)
            {
                //page not found error
                
                return View("_ErrorPage");
            }
        }










        //[HttpGet]
        //public IActionResult GetTargetCB(int? oAppGloOwnId = null, int? oChurchBodyId = null, int? oTargetCBId = null,
        //                                        string strId_TCB = "", string strName_TCB = "", string strCLId_TCB = "", string strCLName_TCB = "")
        //{
        //    try
        //    {
        //        var oCBModel = new TargetCBModel();
        //        oCBModel.oAppGloOwnId = oAppGloOwnId;     // current AGO of request
        //        oCBModel.oChurchBodyId = oChurchBodyId;  // current CB of request 
        //        ///
        //        oCBModel.oTargetCBId = oTargetCBId;     // target CB of request == initial CB ...  
        //        var _oTargetCBId = oTargetCBId;
        //        ///
        //        oCBModel.strId_TCB = strId_TCB;
        //        oCBModel.strName_TCB = strName_TCB;
        //        oCBModel.strCLId_TCB = strCLId_TCB;
        //        oCBModel.strCLName_TCB = strCLName_TCB;
        //        ///
        //        if (oCBModel.oAppGloOwnId == null)
        //            return View("_ErrorPage");  // || oCBModel.oChurchBodyId == null current denom and cong cannot be null

        //        //if (oCBModel.oTargetCBId == null) oCBModel.oTargetCBId = oCBModel.oChurchBodyId;
        //        ///
        //        oCBModel.oTargetCB_MSTR = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel) //.Include(t => t.ParentChurchBody)
        //                            .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.Id == oCBModel.oTargetCBId)
        //                            .FirstOrDefault();

        //        //if (oCBModel.oTargetCB != null ) return View("_ErrorPage");
        //        oCBModel.numChurchLevel_Index = _context.MSTRChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId); //oCBModel.oTargetCB.ChurchLevel.LevelIndex;
        //        oCBModel.strTargetCB = "None. (choose church body)"; oCBModel.strTargetCL = "";
        //        oCBModel.arrRootCBIds = new List<object>();
        //        oCBModel.arrRootCBNames = new List<string>();
        //        for (int k = 0; k < 10; k++)
        //        {
        //            oCBModel.arrRootCBIds.Add((int?)null); // initialize the template
        //            oCBModel.arrRootCBNames.Add(null); // initialize the template
        //        }
        //        /// pre-load the list...
        //        if (_oTargetCBId != null)
        //        {
        //            oCBModel.strTargetCB = oCBModel.oTargetCB_MSTR.Name;
        //            oCBModel.strTargetCL = !string.IsNullOrEmpty(oCBModel.oTargetCB_MSTR.ChurchLevel.CustomName) ? oCBModel.oTargetCB_MSTR.ChurchLevel.CustomName : oCBModel.oTargetCB_MSTR.ChurchLevel.Name;

        //            oCBModel.arrRootCBCodes = oCBModel.oTargetCB_MSTR != null ? oCBModel.oTargetCB_MSTR.RootChurchCode.Split("--").ToList<string>() : new List<string>();
        //            // oCBModel.oTargetCLId = currCB.ChurchLevelId;

        //            //// get the CB path... use either ChurchCode [must av bn ordered! else trouble..] or CB Id to trace path...
        //            //var masterCBList = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Include(t => t.ChurchLevel)
        //            //                    .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId &&
        //            //                    (oCBModel.oTargetCB_MSTR == null || (oCBModel.oTargetCB_MSTR != null && oCBModel.arrRootCBCodes.Contains(c.GlobalChurchCode))))
        //            //                    .ToList();

        //            var masterCBList = (from t_cb in _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
        //                                           .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId &&
        //                                           (oCBModel.oTargetCB_MSTR == null || (oCBModel.oTargetCB_MSTR != null && oCBModel.arrRootCBCodes.Contains(c.GlobalChurchCode))))
        //                                from t_cb_p in _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
        //                                           .Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb.ParentChurchBodyId).DefaultIfEmpty()
        //                                select new MSTRChurchBody()
        //                                {
        //                                    Id = t_cb.Id,
        //                                    AppGlobalOwnerId = t_cb.AppGlobalOwnerId,
        //                                    ChurchLevelId = t_cb.ChurchLevelId,
        //                                    Name = t_cb.Name,
        //                                    GlobalChurchCode = t_cb.GlobalChurchCode,
        //                                    RootChurchCode = t_cb.RootChurchCode,
        //                                    OrgType = t_cb.OrgType,
        //                                    ParentChurchBodyId = t_cb.ParentChurchBodyId,
        //                                    ContactInfoId = t_cb.ContactInfoId,
        //                                    IsWaiveSubscription = t_cb.IsWaiveSubscription,
        //                                    SubscriptionKey = t_cb.SubscriptionKey,
        //                                    LicenseKey = t_cb.LicenseKey,
        //                                    CtryAlpha3Code = t_cb.CtryAlpha3Code,
        //                                    CountryRegionId = t_cb.CountryRegionId,
        //                                    Comments = t_cb.Comments,
        //                                    ChurchWorkStatus = t_cb.ChurchWorkStatus,
        //                                    Status = t_cb.Status,
        //                                    ///
        //                                    ParentChurchBody = t_cb_p,
        //                                })
        //                                .ToList();

        //            ///
        //            var oCBNextParent = masterCBList.Where(c => c.Id == oCBModel.oTargetCB_MSTR.Id && c.GlobalChurchCode == oCBModel.oTargetCB_MSTR.GlobalChurchCode).FirstOrDefault();
        //            var listCount = 0;
        //            if (oCBNextParent != null)
        //            {
        //                listCount = (oCBNextParent.ChurchLevel != null ? oCBNextParent.ChurchLevel.LevelIndex : listCount);
        //                listCount = listCount - 1;
        //                oCBModel.arrRootCBIds[listCount] = oCBNextParent.Id;
        //                oCBModel.arrRootCBNames[listCount] = oCBNextParent.Name;
        //            }
        //            for (int i = listCount - 1; i > 0; i--)
        //            {
        //                oCBNextParent = masterCBList
        //                    .Where(c => c.Id == oCBNextParent.ParentChurchBodyId && c.GlobalChurchCode == oCBNextParent.ParentChurchBody.GlobalChurchCode)
        //                    .FirstOrDefault();
        //                if (oCBNextParent != null)
        //                {
        //                    oCBModel.arrRootCBIds[i] = oCBNextParent.Id;
        //                    oCBModel.arrRootCBNames[i] = oCBNextParent.Name;
        //                }
        //            }

        //            // add the root
        //            oCBNextParent = masterCBList.Where(c => c.OrgType == "CR" && c.ParentChurchBodyId == null).FirstOrDefault();
        //            if (oCBNextParent != null)
        //            {
        //                oCBModel.arrRootCBIds[0] = oCBNextParent.Id;  // base CB usually the name of the church -- CR
        //                oCBModel.arrRootCBNames[0] = oCBNextParent.Name + " (Church Root)";  // base CB usually the name of the church -- CR
        //            }
        //        }

        //        else
        //        {
        //            // add the root
        //            var oCBNextParent = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)  // .Include(t => t.ParentChurchBody)
        //                                .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.OrgType == "CR" && c.ParentChurchBodyId == null).FirstOrDefault();
        //            if (oCBNextParent != null)
        //            {
        //                oCBModel.arrRootCBIds[0] = oCBNextParent.Id;  // base CB usually the name of the church -- CR
        //                oCBModel.arrRootCBNames[0] = oCBNextParent.Name + " (Church Root)";  // base CB usually the name of the church -- CR
        //            }
        //        }


        //        oCBModel.oCBLevelCount = oCBModel.numChurchLevel_Index; // - 1;        // oCBLevelCount -= 2;  // less requesting CB
        //        List<MSTRChurchLevel> oCBLevelList = _context.MSTRChurchLevel
        //            .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.LevelIndex > 0 && c.LevelIndex <= oCBModel.numChurchLevel_Index)
        //            .ToList().OrderBy(c => c.LevelIndex).ToList();
        //        ///
        //        if (oCBModel.oCBLevelCount > 0 && oCBLevelList.Count > 0)
        //        {
        //            oCBModel.strChurchLevel_1 = !string.IsNullOrEmpty(oCBLevelList[0].CustomName) ? oCBLevelList[0].CustomName : oCBLevelList[0].Name;
        //            ViewBag.strChurchLevel_1 = oCBModel.strChurchLevel_1;

        //            if (oCBModel.oCBLevelCount > 1)
        //            {
        //                oCBModel.strChurchLevel_2 = !string.IsNullOrEmpty(oCBLevelList[1].CustomName) ? oCBLevelList[1].CustomName : oCBLevelList[1].Name;
        //                ViewBag.strChurchLevel_2 = oCBModel.strChurchLevel_2;

        //                if (oCBModel.oCBLevelCount > 2)
        //                {
        //                    oCBModel.strChurchLevel_3 = !string.IsNullOrEmpty(oCBLevelList[2].CustomName) ? oCBLevelList[2].CustomName : oCBLevelList[2].Name;
        //                    ViewBag.strChurchLevel_3 = oCBModel.strChurchLevel_3;

        //                    if (oCBModel.oCBLevelCount > 3)
        //                    {
        //                        oCBModel.strChurchLevel_4 = !string.IsNullOrEmpty(oCBLevelList[3].CustomName) ? oCBLevelList[3].CustomName : oCBLevelList[3].Name;
        //                        ViewBag.strChurchLevel_4 = oCBModel.strChurchLevel_4;

        //                        if (oCBModel.oCBLevelCount > 4)
        //                        {
        //                            oCBModel.strChurchLevel_5 = !string.IsNullOrEmpty(oCBLevelList[4].CustomName) ? oCBLevelList[4].CustomName : oCBLevelList[4].Name;
        //                            ViewBag.strChurchLevel_5 = oCBModel.strChurchLevel_5;

        //                            if (oCBModel.oCBLevelCount > 5)
        //                            {
        //                                oCBModel.strChurchLevel_6 = !string.IsNullOrEmpty(oCBLevelList[5].CustomName) ? oCBLevelList[5].CustomName : oCBLevelList[5].Name;
        //                                ViewBag.strChurchLevel_6 = oCBModel.strChurchLevel_6;

        //                                if (oCBModel.oCBLevelCount > 6)
        //                                {
        //                                    oCBModel.strChurchLevel_7 = !string.IsNullOrEmpty(oCBLevelList[6].CustomName) ? oCBLevelList[6].CustomName : oCBLevelList[6].Name;
        //                                    ViewBag.strChurchLevel_7 = oCBModel.strChurchLevel_7;

        //                                    if (oCBModel.oCBLevelCount > 7)
        //                                    {
        //                                        oCBModel.strChurchLevel_8 = !string.IsNullOrEmpty(oCBLevelList[7].CustomName) ? oCBLevelList[7].CustomName : oCBLevelList[7].Name;
        //                                        ViewBag.strChurchLevel_8 = oCBModel.strChurchLevel_8;

        //                                        if (oCBModel.oCBLevelCount > 8)
        //                                        {
        //                                            oCBModel.strChurchLevel_9 = !string.IsNullOrEmpty(oCBLevelList[8].CustomName) ? oCBLevelList[8].CustomName : oCBLevelList[8].Name;
        //                                            ViewBag.strChurchLevel_9 = oCBModel.strChurchLevel_9;

        //                                            if (oCBModel.oCBLevelCount > 9)
        //                                            {
        //                                                oCBModel.strChurchLevel_10 = !string.IsNullOrEmpty(oCBLevelList[9].CustomName) ? oCBLevelList[9].CustomName : oCBLevelList[9].Name;
        //                                                ViewBag.strChurchLevel_10 = oCBModel.strChurchLevel_10;
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //        //oCBModel.oUserId_Logged = this._oLoggedUser.Id;
        //        //oCBModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
        //        //oCBModel.oChurchBodyId_Logged = this._oLoggedCB.Id;

        //        /// load lookups
        //        // oCUModel = this.popLookups_CU(oCUModel, oCUModel.oChurchUnit);

        //        //var tm = DateTime.Now;
        //        //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, null, null, "T",
        //        //                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
        //        //    , this._clientDBConnString);

        //        //var _oCUModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCUModel);
        //        //TempData["oVmCurrMod"] = _oCUModel; TempData.Keep();

        //        oCBModel.lkpChurchLevels = _context.MSTRChurchLevel.Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId)
        //                                  .OrderByDescending(c => c.LevelIndex)
        //                                  .Select(c => new SelectListItem()
        //                                  {
        //                                      Value = c.Id.ToString(),
        //                                      Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name,
        //                                      // Disabled = (numCLIndex == (int?)null || c.LevelIndex < numCLIndex || oCurrChurchBody.OrgType == "CH" || oCurrChurchBody.OrgType == "CN")
        //                                  })
        //                                  .ToList();

        //        return PartialView("_GetTargetCB", oCBModel);

        //    }

        //    catch (Exception ex)
        //    {
        //        //page not found error
        //        
        //        return View("_ErrorPage");
        //    }
        //}







        [HttpGet]   // public IActionResult AddOrEdit_UP_ChangePwd(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)  //(int userId = 0, int setIndex = 0) // int? oCurrChuBodyId = null, string profileScope = "C", int setIndex = 0)   // setIndex = 0 (SYS), setIndex = 1 (SUP_ADMN), = 2 (Create/update user), = 3 (reset Pwd) 
        public IActionResult AddOrEdit_UP_ChangePwd(int? id = 0, int setIndex = 0, int? oUserId_Logged = null, int pageIndex = 1)  //(int userId = 0, int setIndex = 0) // int? oCurrChuBodyId = null, string profileScope = "C", int setIndex = 0)   // setIndex = 0 (SYS), setIndex = 1 (SUP_ADMN), = 2 (Create/update user), = 3 (reset Pwd) 
        {
            // SetUserLogged();
            if (!InitializeUserLogging(false)) return RedirectToAction("LoginUserAcc", "UserLogin"); //  if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            else
            {  
                //
                var oUserResetModel = new ResetUserPasswordModel(); 
                // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
                var proScope = setIndex == 1 ? "V" : "C";
                var subScope = setIndex == 2 ? "D" : setIndex == 3 ? "A" : ""; 
                var _userTask = "Attempted changing user password";
                int? oAppGloOwnId = null; int? oCurrChuBodyId = null;
                int? oAGOId_Logged = null; int? oCBId_Logged = null;
                // 
                var oUser = _context.UserProfile.AsNoTracking()
                         .Where(c => c.Id == id && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == proScope).FirstOrDefault();

                if (oUser == null) return PartialView("_AddOrEdit_UP_ChangePwd", oUserResetModel);

                // 
                //oCurrVmMod.oChurchBody = oCurrChuBodyLogOn; 
               // oUserResetModel.setIndex = setIndex;
                oUserResetModel.oUserProfile = oUser;
                //
                oUserResetModel.Username = oUser.Username;
                oUserResetModel.CurrentPassword = null;
                oUserResetModel.NewPassword = null;
                oUserResetModel.RepeatPassword = null;
               // oCurrVmMod.SecurityQue = oUser.PwdSecurityQue;
               // oCurrVmMod.SecurityAns = null;
                oUserResetModel.VerificationCode = null; // via email, sms
                oUserResetModel.strLogUserDesc = oUser.UserDesc;
                oUserResetModel.AuthTypeUsed = null;
                  
                //var _oCurrVmMod = oCurrVmMod;
                //TempData["oVmCurrMod"] = _vmMod;
                //TempData.Keep();

                //var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(_oCurrVmMod);
                //TempData["oVmCurrMod"] = _vmMod; TempData.Keep();


                oUserResetModel.setIndex = setIndex;
                oUserResetModel.pageIndex = pageIndex;

               // oUserResetModel.subSetIndex = subSetIndex;
                oUserResetModel.profileScope = proScope;
                oUserResetModel.subScope = subScope;
                //
                oUserResetModel.oCurrUserId_Logged = oUserId_Logged;
                oUserResetModel.oAppGloOwnId_Logged = oAGOId_Logged;
                oUserResetModel.oChurchBodyId_Logged = oCBId_Logged;
                //
                oUserResetModel.oAppGloOwnId = oAppGloOwnId;
                oUserResetModel.oChurchBodyId = oCurrChuBodyId;


                // oUserModel = this.populateLookups_UP_MS(oUserModel, setIndex); 
                oUserResetModel.lkpAuthTypes = new List<SelectListItem>();
                foreach (var dl in dlUserAuthTypes) { oUserResetModel.lkpAuthTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                var _oUserResetModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUserResetModel);
                TempData["oVmCurrMod"] = _oUserResetModel; TempData.Keep();

                _userTask = "Modified user profile--change password";
                var _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged));
                  

                return PartialView("_AddOrEdit_UP_ChangePwd", oUserResetModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_ChangePwd(ResetUserPasswordModel vm)
        {
            UserProfile _oChanges;  // = vm .oUserProfile;
            // vm = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as ResetUserProfilePwdVM : vm; TempData.Keep();

            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ResetUserPasswordModel>(arrData) : vm;

            var oUP = _context.UserProfile.AsNoTracking().Include(t=>t.ChurchBody)
                            .Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && vm.profileScope == "V" && vm.oAppGloOwnId != null && vm.oChurchBodyId != null) || 
                                         (c.AppGlobalOwnerId == vm.oAppGloOwnId && c.ChurchBodyId == vm.oChurchBodyId && vm.profileScope == "C")) && 
                                   c.Username == vm.Username).FirstOrDefault();  // vm.oUserProfile;

            if (oUP == null)
                return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "User profile could not be loaded. Please check and try again.", signOutToLogIn = false });

            //oUP.ChurchBody = vm.oChurchBody;

            try
            {
                ModelState.Remove("oUserProfile.AppGlobalOwnerId");
                ModelState.Remove("oUserProfile.ChurchBodyId");
                ModelState.Remove("oUserProfile.ChurchMemberId");
                ModelState.Remove("oUserProfile.CreatedByUserId");
                ModelState.Remove("oUserProfile.LastModByUserId");
                ModelState.Remove("oUserProfile.OwnerId");

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Failed to load the data to save. Please refresh and try again.", signOutToLogIn = false });

                //var tm = DateTime.Now;
                //_oChanges.LastMod = tm;
                //_oChanges.LastModByUserId = vm.oCurrLoggedUserId;

                if (vm.profileScope != "V" && vm.ChurchCode != "000000") // Denomination and ChurchBody cannot be null
                {
                    if (vm.oAppGloOwnId == null || vm.oChurchBodyId == null)                    
                        return Json(new { taskSuccess = false, oCurrId = "", userMess = "Specify the denomination and church unit", signOutToLogIn = false });

                }

                // get the user profile...
                var userProList = (from t_upx in _context.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == vm.oAppGloOwnId && c.ChurchBodyId == vm.oChurchBodyId &&
                                                                                     c.ProfileScope == vm.profileScope && c.Id == oUP.Id)
                                       //  from t_upr in _context.UserProfileRole.Where(c => c.ChurchBodyId == t_upx.ChurchBodyId && c.UserProfileId == t_upx.Id).DefaultIfEmpty()
                                       //  from t_ur in _context.UserRole.Where(c => c.ChurchBodyId == t_upx.ChurchBodyId && c.Id == t_upr.UserRoleId && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").DefaultIfEmpty()
                                   select t_upx
                                  ).OrderBy(c => c.UserDesc).ToList();

                if (userProList.Count == 0)
                    return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "User account was not found. Please rfresh and try again.", signOutToLogIn = false });

                if (oUP.Expr != null)
                {
                    if (oUP.Expr.Value >= DateTime.Now.Date)
                        return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Please user account has expired. Activate account first.", signOutToLogIn = false });
                }
                if (vm.NewPassword != null)
                    return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Please provide user password (minimum 6-digit; use strong passwords:- UPPER and lower cases, digits (0-9) and $pecial characters)", signOutToLogIn = false });

                if (vm.NewPassword != vm.RepeatPassword)
                    return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Password mismatch. Provide user password again.", signOutToLogIn = false });

                if (vm.AuthTypeUsed == null)
                    return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Please indicate authentication type to confirm user profile.", signOutToLogIn = false });

                if (vm.AuthTypeUsed == 1)  //2-way  ... Compulsory for VENDOR  ... optional for clients
                {
                    if (vm.VerificationCode != "12345678") // to be sent to user's email, sms
                        return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Enter correct verification code.", signOutToLogIn = false });
                }
                else
                {
                    var _secAns = AppUtilties.ComputeSha256Hash(vm.SecurityQue + vm.SecurityAns);
                    if (vm.SecurityQue.ToLower().Equals(vm.SecurityQue.ToLower()) && vm.SecurityAns.Equals(_secAns))
                        return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Security answer provided is not correct.", signOutToLogIn = false });
                }

               // var  _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  ///this._configuration["ConnectionStrings:DefaultConnection"];

                var _userTask = "Attempted to changed user password.";
               // using (var _pwdCtx = new MSTR_DbContext(_cs))
               // {
                    //create user and init...
                    _oChanges = new UserProfile();
                    //_oChanges.AppGlobalOwnerId = null; // oUP.ChurchBody != null ? oUP.ChurchBody.AppGlobalOwnerId : null;
                    //_oChanges.ChurchBodyId = null; //(int)oUP.ChurchBody.Id;
                    //_oChanges.OwnerId =null; // (int)vm.oCurrLoggedUserId;
                     
                    var cc = "";
                    if (_oChanges.AppGlobalOwnerId == null && _oChanges.ChurchBodyId == null && _oChanges.ProfileScope == "V")
                    {
                        cc = "000000";    //var churchCode = "000000"; _oChanges.Username = "SysAdmin"; _oChanges.Pwd = "$ys@dmin1";  
                    }
                    else
                    {
                        var oAGO = _context.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
                        var oCB = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == _oChanges.ChurchBodyId).FirstOrDefault();

                        if (oAGO == null || oCB == null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specified denomination and church unit could not be retrieved. Please refresh and try again.", signOutToLogIn = false });
                        if (string.IsNullOrEmpty(oCB.GlobalChurchCode))
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code not specified. The unique global church code of church unit is required. Please verify with System Admin and try again.", signOutToLogIn = false });


                        cc = oCB.GlobalChurchCode;

                        // _oChanges.Pwd = AppUtilties.ComputeSha256Hash(_oChanges.Username + _oChanges.Pwd);
                    }

                    _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username);
                    _oChanges.Pwd = "123456";  //temp pwd... to reset @ next login   --- send temporal pwd to be changed within 24 hours... hash (today + cc + gen code) :: until close of day (midnight)
                    _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username + _oChanges.Pwd);


                    var tm = DateTime.Now;
                    _oChanges.Strt = tm;
                    //_oChanges.Expr = null; // tm.AddDays(90);  //default to 30 days
                    //  oCurrvm.oUserProfile.UserId = oCurrChuMemberId_LogOn;
                    //_oChanges.ChurchMemberId = null; // vm.oCurrLoggedMemberId;
                    // _oChanges.UserScope = "E"; // I-internal, E-external
                    //_oChanges.ProfileScope = "V"; // V-Vendor, C-Client

                    _oChanges.ResetPwdOnNextLogOn = false;
                    _oChanges.PwdExpr = tm.AddDays(30);  //default to 90 days 
                    _oChanges.UserStatus = "A"; // A-ctive...D-eactive

                    // _oChanges.Created = tm;
                    _oChanges.LastMod = tm;
                    //  _oChanges.CreatedByUserId = null; // (int)vm.oCurrLoggedUserId;
                    _oChanges.LastModByUserId = null; // (int)vm.oCurrLoggedUserId;

                  //  _oChanges.Pwd = AppUtilties.ComputeSha256Hash(vm.Username + vm.NewPassword);
                    //_oChanges.UserDesc = "Super Admin";
                    //_oChanges.UserPhoto = null;
                    //_oChanges.UserId = null;
                    //_oChanges.PhoneNum = null;
                    //_oChanges.Email = null; 

                    // 
                   
                    _userTask = "Changed user password successfully.";
                    ViewBag.UserMsg = "Password changed successfully.";

                //save everything
                _context.Update(_oChanges);

                    await _context.SaveChangesAsync();


                  //  DetachAllEntities(_pwdCtx);
               // }

               

                //audit...
                var _tm = DateTime.Now;
                _ =  this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));


                var _vm = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                TempData["oVmCurr"] = _vm; TempData.Keep();

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = true });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "Failed saving user profile details. Err: " + ex.Message, signOutToLogIn = false });
            }
        }
        
        
    }
}



