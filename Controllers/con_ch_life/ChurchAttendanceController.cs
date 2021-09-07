using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhemaCMS.Models;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using RhemaCMS.Models.MSTRModels;
using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Controllers.con_adhc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RhemaCMS.Models.Adhoc;
using Microsoft.AspNetCore.Hosting;
using RhemaCMS.Models.ViewModels.vm_cl;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using RhemaCMS.Models.ViewModels;

namespace RhemaCMS.Controllers.con_ch_life
{
    public class ChurchAttendanceController : Controller
    {

        private readonly MSTR_DbContext _masterContext;
        private ChurchModelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        ///
        //private string _strClientConn;

        private string _clientDBConnString;
        private UserProfile _oLoggedUser;
        // private UserRole _oLoggedRole;
        private MSTRChurchBody _oLoggedCB_MSTR;
        private MSTRAppGlobalOwner _oLoggedAGO_MSTR;

        // private bool isCurrValid = false;
        private UserSessionPrivilege oUserLogIn_Priv = null;

        /// localized
        private ChurchBody _oLoggedCB;
        private AppGlobalOwner _oLoggedAGO;


        ///   this attr is used by most of the models... save in memory [class var]
        private CountryCustom oCTRYDefault;
        private CurrencyCustom oCURRDefault;
        private ChurchPeriod oCPRDefault;

        private string strCountryCode_dflt = (string)null;
        private string strCountryName_dflt = "";
        private string strCountryCURR1_dflt = "";
        private string strCountryCURR2_dflt = "";

        // private bool isCurrValid = false;
        // private UserSessionPrivilege oUserLogIn_Priv = null;
        ///        
        private List<DiscreteLookup> dlGenStatuses = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlStatus = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlShareStatus = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlMemTypeCodes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlEnrollStatuses = new List<DiscreteLookup>();
         
        private List<DiscreteLookup> dlPeriodTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlIntervalFreqs = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlSemesters = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlQuarters = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlMonths = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlDays = new List<DiscreteLookup>();

        private List<DiscreteLookup> dlThemeColor = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlActivityTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlMaritalStatus = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlGenderType = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlEventSessionCodes = new List<DiscreteLookup>();

        private readonly IConfiguration _configuration;
        private string _clientDBConn;

        public ChurchAttendanceController(MSTR_DbContext masterContext, IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, ChurchModelContext clientCtx,
             IConfiguration configuration)
        {
            try
            {
                _hostingEnvironment = hostingEnvironment;
                _masterContext = masterContext;

                _httpContextAccessor = httpContextAccessor;
                _tempDataDictionaryFactory = tempDataDictionaryFactory;
                _configuration = configuration;



                // get user logon info... authenticate else logout
                if (!SetUserLogged())
                    RedirectToAction("LoginUserAcc", "UserLogin");


                // get user logon details... from memory
                // _oLoggedRole = oUserLogIn_Priv.UserRole; 

                var isAuth = this.oUserLogIn_Priv != null;
                //  if (!isAuth) isAuth = SetUserLogged();

                if (isAuth)
                {
                    this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
                    this._oLoggedCB_MSTR = this.oUserLogIn_Priv.ChurchBody;
                    this._oLoggedAGO_MSTR = this.oUserLogIn_Priv.AppGlobalOwner;
                    this._oLoggedUser.strChurchCode_AGO = this._oLoggedAGO_MSTR != null ? this._oLoggedAGO_MSTR.GlobalChurchCode : "";
                    this._oLoggedUser.strChurchCode_CB = this._oLoggedCB_MSTR != null ? this._oLoggedCB_MSTR.GlobalChurchCode : "";
                    ///
                    this._oLoggedCB = this.oUserLogIn_Priv.ChurchBody_CLNT;
                    this._oLoggedAGO = this.oUserLogIn_Priv.AppGlobalOwner_CLNT;
                }


                if (this._oLoggedUser == null)
                    RedirectToAction("LoginUserAcc", "UserLogin");

                if (this._oLoggedUser.AppGlobalOwnerId == null || this._oLoggedUser.ChurchBodyId == null)
                    RedirectToAction("LoginUserAcc", "UserLogin");


                // _context = context;
                //  this._context = clientCtx;

                this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

                if (clientCtx == null)
                    _context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

                //else
                //{
                //    var cs = clientCtx.Database.GetDbConnection().ConnectionString;
                //    if (string.IsNullOrEmpty(cs))
                //        _context = GetClientDBContext();
                //    else
                //    {
                //        var conn = new SqlConnectionStringBuilder(cs);
                //        if (conn.DataSource == "_BLNK" || conn.InitialCatalog == "_BLNK") // (string.IsNullOrEmpty(this._clientDBConnString) || this.oUserLogIn_Priv.UserProfile == null)
                //            _context = GetClientDBContext();
                //        else
                //        {
                //            _context = clientCtx;
                //            this._clientDBConnString = clientCtx.Database.GetDbConnection().ConnectionString;
                //        }
                //    }
                //}


                /// synchronize AGO, CL, CB, CTRY  or @login 
                // this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

                /// get the localized data... using the MSTR data
                if (this._context != null && (this._oLoggedAGO == null || this._oLoggedCB == null))
                {
                    this._oLoggedAGO = this._context.AppGlobalOwner.AsNoTracking()
                                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
                    this._oLoggedCB = this._context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();
                }


            }
            catch (Exception ex)
            {
                RedirectToAction("LoginUserAcc", "UserLogin");
            }



            ////load the dash
            //LoadClientDashboardValues();



            dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "S", Desc = "Single" });
            dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "M", Desc = "Married" });
            dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "X", Desc = "Separated" });
            dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "D", Desc = "Divorced" });
            dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "W", Desc = "Widowed" });
            //  dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "C", Desc = "Co-habit" });
            dlMaritalStatus.Add(new DiscreteLookup() { Category = "MarrStatus", Val = "O", Desc = "Other" });


            //GA-- General activ, ER-Event Role,  MC--Member Churchlife Activity related, EV-Church Event related
            dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_GA", Desc = "General Activity" });
            dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_EV", Desc = "Event or Program" });
            // dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_ER", Desc = "Event Role" });
            dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_MR", Desc = "Member related" });


            //SharingStatus { get; set; }  // A - Share with all sub-congregations, C - Share with child congregations only, N - Do not share
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "N", Desc = "Do not roll-down (share)" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "C", Desc = "Roll-down (share) for direct child congregations" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "A", Desc = "Roll-down (share) for all sub-congregations" });


            dlMemTypeCodes.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "V", Desc = "Visitor" }); /// Regular Visitor 
            dlMemTypeCodes.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "G", Desc = "Guest /Affiliate" });  //invited guests, visiting missionaries, workers can be assigned temporal membership status -- MBD, MCI, MCP, MCL
          // dlMemTypeCodes.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "N", Desc = "New Convert" }); // FULLY automate... MBD, MCI, MLS, MCP, MCL, MCLAc, MCET, MS [ MT, MS -- unassigned ], MCA, MTP    add New convert as *special Visitor [ on diff interface ]  ... [Member data] to be linked with [New Convert data] ... New Convert Class
          // dlMemTypeCodes.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "T", Desc = "In-Transit" }); // jux like the Congregant ... only that he has not reported physically yet --> result of TRANSFERS
            dlMemTypeCodes.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "C", Desc = "Congregant" }); // has all modules [21] -- MBD, MCI, MLS, MFR, MCP, MED, MPB, MWE --- MCL [MCL, MCLAc, MCET], MCM [MS, MT, MS], MCG, MCR, MRR, MCA, MCT, MTP, MCV [visits** when sick or regular checkups]
                                                                                                                  //dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "L", Desc = "Church Leader" });  /// Mainstream church leadership [elders abd the like]
                                                                                                                  //dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "P", Desc = "Lay Pastor" });   /// lay [P]astor
                                                                                                                  //dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "M", Desc = "Minister" });   // Ordained...

            ///
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "A", Desc = "Active" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "P", Desc = "Pending" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "B", Desc = "Blocked" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "D", Desc = "Deactive" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "E", Desc = "Expired" });


            dlGenderType.Add(new DiscreteLookup() { Category = "GenderType", Val = "M", Desc = "Male" });
            dlGenderType.Add(new DiscreteLookup() { Category = "GenderType", Val = "F", Desc = "Female" });
            dlGenderType.Add(new DiscreteLookup() { Category = "GenderType", Val = "O", Desc = "Other" });

            dlPeriodTypes.Add(new DiscreteLookup() { Category = "PeriodType", Val = "CP", Desc = "Church Calendar Period" });
            dlPeriodTypes.Add(new DiscreteLookup() { Category = "PeriodType", Val = "AP", Desc = "Accounting Period" });
            //  dlPeriodTypes.Add(new DiscreteLookup() { Category = "PeriodType", Val = "DF", Desc = "Default Period" });
            dlPeriodTypes.Add(new DiscreteLookup() { Category = "PeriodType", Val = "CS", Desc = "Custom Period" });
            dlPeriodTypes.Add(new DiscreteLookup() { Category = "PeriodType", Val = "DF", Desc = "Interval Definition" });

            dlIntervalFreqs.Add(new DiscreteLookup() { Category = "Interval", Val = "D", Desc = "Day" });
            dlIntervalFreqs.Add(new DiscreteLookup() { Category = "Interval", Val = "W", Desc = "Week" });
            dlIntervalFreqs.Add(new DiscreteLookup() { Category = "Interval", Val = "M", Desc = "Month" });
            dlIntervalFreqs.Add(new DiscreteLookup() { Category = "Interval", Val = "S", Desc = "Semester" });
            dlIntervalFreqs.Add(new DiscreteLookup() { Category = "Interval", Val = "Y", Desc = "Year" });

            // pair with the specific year... ex. 2021-Sem-1, 2021-Sem-2, 2023-Sem-2
            dlSemesters.Add(new DiscreteLookup() { Category = "CPR-Sem", Val = "S1", Desc = "Semester-1" });
            dlSemesters.Add(new DiscreteLookup() { Category = "CPR-Sem", Val = "S2", Desc = "Semester-2" });

            // pair with the specific year... ex. 2021-Qtr-1, 2022-Qtr-2, 2023-Qtr-4  // 2021-Quarter-1
            dlQuarters.Add(new DiscreteLookup() { Category = "CPR-Qtr", Val = "Q1", Desc = "Quarter-1" });
            dlQuarters.Add(new DiscreteLookup() { Category = "CPR-Qtr", Val = "Q2", Desc = "Quarter-2" });
            dlQuarters.Add(new DiscreteLookup() { Category = "CPR-Qtr", Val = "Q3", Desc = "Quarter-3" });
            dlQuarters.Add(new DiscreteLookup() { Category = "CPR-Qtr", Val = "Q4", Desc = "Quarter-4" });

            // pair with the specific year... ex. 2021-Jan, 2022-Feb, 2023-Dec  ... 2021-January
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M1", Desc = "Jan" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M2", Desc = "Feb" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M3", Desc = "Mar" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M4", Desc = "Apr" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M5", Desc = "May" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M6", Desc = "Jun" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M7", Desc = "Jul" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M8", Desc = "Aug" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M9", Desc = "Sep" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M10", Desc = "Oct" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M11", Desc = "Nov" });
            dlMonths.Add(new DiscreteLookup() { Category = "CPR-Mon", Val = "M12", Desc = "Dec" });
             

            dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "T", Desc = "Church Transfer" });
            dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "I", Desc = "Invite /Referral" });
            //dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "I", Desc = "Church Invite" });            
            //dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "I", Desc = "Group Invite" });
            //dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "I", Desc = "Member Invite" });
            dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "W", Desc = "Walk-in" });
            dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "P", Desc = "Through Parent" });
            dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "M", Desc = "Via Marriage" });
            dlEnrollStatuses.Add(new DiscreteLookup() { Category = "EnrollStatus", Val = "O", Desc = "other" });

            dlEventSessionCodes.Add(new DiscreteLookup() { Category = "EventSessionCode", Val = "1", Desc = "Session 1" });
            dlEventSessionCodes.Add(new DiscreteLookup() { Category = "EventSessionCode", Val = "2", Desc = "Session 2" });
            dlEventSessionCodes.Add(new DiscreteLookup() { Category = "EventSessionCode", Val = "2", Desc = "Session 3" });
        }




        public static string GetGenderDesc(string oCode)
        {
            switch (oCode)
            {
                case "M": return "Male";
                case "F": return "Female";
                case "X": return "Mixed";
                case "O": return "Gender (Other)";

                default: return oCode;
            }
        }

        public static string GetMemTypeDesc(string oCode)
        {
            switch (oCode)
            {
                case "M": return "Minister";   // Ordained...
                case "P": return "Lay Pastor";  /// lay [P]astor
                case "L": return "Church Leader";
                ///
                case "C": return "Member";   // Congregant
                case "T": return "In-Transit";      // Transfer states 
                case "N": return "New Convert";      // Transfer states 
                case "A": return "Affiliate";
                case "V": return "Guest /Visitor";  /// Regular Visitor

                default: return oCode;
            }
        }


        public static string GetEnrollStatusDesc(string oCode)
        {
            switch (oCode)
            {
                case "T": return "Church Transfer";
                case "I": return "Invite /Referral";
                case "W": return "Walk-in";
                case "P": return "Through Parent"; // Parent-led
                case "M": return "Via Marriage";
                case "O": return "Other";

                default: return oCode;
            }
        }


        public static string GetEventSessionDesc(string oCode)
        {
            switch (oCode)
            {
                case "1": return "Session 1";
                case "2": return "Session 2";
                case "3": return "Session 3";
                

                default: return oCode;
            }
        }



        //public static string GetConcatMemberName(string title, string fn, string mn, string ln, bool displayName = false)
        //{
        //    if (displayName)
        //        return ((((!string.IsNullOrEmpty(title) ? title : "") + ' ' + fn).Trim() + " " + mn).Trim() + " " + ln).Trim();
        //    else
        //        return (((fn + ' ' + mn).Trim() + " " + ln).Trim() + " " + (!string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
        //}



        //public static string GetConcatMemberName(string title, string fn, string mn, string ln, bool profileDispFormat = false, bool dispTitle = false, bool lnSTRT = false, bool capSTRT = false, bool capLn = false)
        //{
        //    if (!lnSTRT && capSTRT) fn = fn.ToUpper();
        //    else if (lnSTRT && capSTRT) ln = ln.ToUpper();

        //    if (capLn) ln = ln.ToUpper();

        //    if (lnSTRT)
        //    {
        //        if (profileDispFormat)
        //            return ((((dispTitle ? title : "") + " " + ln).Trim() + " " + mn).Trim() + " " + fn).Trim();
        //        else
        //            return (((ln + ' ' + mn).Trim() + " " + fn).Trim() + " " + (dispTitle && !string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
        //    }
        //    else
        //    {
        //        if (profileDispFormat)
        //            return ((((dispTitle ? title : "") + ' ' + fn).Trim() + " " + mn).Trim() + " " + ln).Trim();
        //        else
        //            return (((fn + ' ' + mn).Trim() + " " + ln).Trim() + " " + (dispTitle && !string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
        //    }
        //}


        //private ChurchModelContext GetClientDBContext() //UserProfile oUserLogged = null)
        //{
        //    var isAuth = this.oUserLogIn_Priv != null;
        //    if (!isAuth) isAuth = SetUserLogged();
        //    //else
        //    //{
        //    if (!isAuth)
        //    {
        //        RedirectToAction("LoginUserAcc", "UserLogin"); return null;
        //    }
        //    else
        //    {
        //        if (this.oUserLogIn_Priv == null)
        //        {
        //            RedirectToAction("LoginUserAcc", "UserLogin"); return null;
        //        }
        //        else
        //        {
        //            if (this.oUserLogIn_Priv.UserProfile == null)
        //            {
        //                RedirectToAction("LoginUserAcc", "UserLogin"); return null;
        //            }
        //            else
        //            {
        //                var oClientConfig = _masterContext.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //                //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == 4 && c.Status == "A").FirstOrDefault();
        //                if (oClientConfig != null)
        //                {
        //                    //// get and mod the conn
        //                    //var _clientDBConnString = "";
        //                    //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //                    //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //                    //_clientDBConnString = conn.ConnectionString;

        //                    //// test the NEW DB conn
        //                    //var _clientContext = new ChurchModelContext(_clientDBConnString);

        //                    // var _clientDBConnString = "";
        //                    var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //                    conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //                    conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //                    conn.IntegratedSecurity = false; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //                    this._clientDBConnString = conn.ConnectionString;

        //                    // test the NEW DB conn
        //                    var _clientContext = new ChurchModelContext(_clientDBConnString);

        //                    if (!_clientContext.Database.CanConnect())
        //                        RedirectToAction("LoginUserAcc", "UserLogin");

        //                    //// _oLoggedRole = oUserLogIn_Priv.UserRole; 
        //                    //this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
        //                    //this._oLoggedCB_MSTR = this.oUserLogIn_Priv.ChurchBody;
        //                    //this._oLoggedAGO_MSTR = this.oUserLogIn_Priv.AppGlobalOwner;
        //                    //this._oLoggedUser.strChurchCode_AGO = this._oLoggedAGO_MSTR != null ? this._oLoggedAGO_MSTR.GlobalChurchCode : "";
        //                    //this._oLoggedUser.strChurchCode_CB = this._oLoggedCB_MSTR != null ? this._oLoggedCB_MSTR.GlobalChurchCode : "";

        //                    ///// synchronize AGO, CL, CB, CTRY  or @login 
        //                    //// this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

        //                    ///// get the localized data... using the MSTR data
        //                    //this._oLoggedAGO = _clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
        //                    //this._oLoggedCB = _clientContext.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId &&
        //                    //                        c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();


        //                    // load the dash b/f
        //                    // LoadClientDashboardValues();

        //                    return _clientContext;
        //                }
        //                else
        //                { // db config not found             
        //                    RedirectToAction("LoginUserAcc", "UserLogin"); return null;
        //                }
        //            }
        //        }
        //    }
        //}

        // private bool isUserAuthorized = false;  

        private bool SetUserLogged()
        {
            var isUserAuthorized = false;
            if (TempData == null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

                if (tempData.ContainsKey("UserLogIn_oUserPrivCol"))
                {
                    var tempPrivList = tempData["UserLogIn_oUserPrivCol"] as string;
                    if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
                    // De-serialize the string to object
                    this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
                    isUserAuthorized = this.oUserLogIn_Priv != null;

                    if (!tempData.ContainsKey("_bckUserLogIn_oUserPrivCol"))
                    {
                        tempData["_bckUserLogIn_oUserPrivCol"] = tempData["UserLogIn_oUserPrivCol"];
                        tempData.Keep();
                    }

                    ////
                    //isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
                    //if (isCurrValid)
                    //{
                    //    this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;

                    //    // check permission for Core life...  given the sets of permissions
                    //     isUserAuthorized = true;
                    //}
                }

                else if (tempData.ContainsKey("_bckUserLogIn_oUserPrivCol"))
                {
                    var tempPrivList = tempData["_bckUserLogIn_oUserPrivCol"] as string;
                    if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
                    // De-serialize the string to object
                    this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
                    isUserAuthorized = this.oUserLogIn_Priv != null;
                    tempData["UserLogIn_oUserPrivCol"] = tempData["_bckUserLogIn_oUserPrivCol"];
                    tempData.Keep();
                }

                else isUserAuthorized = false; // RedirectToAction("LoginUserAcc", "UserLogin");
            }
            else
            {
                if (TempData.ContainsKey("UserLogIn_oUserPrivCol"))
                {
                    var tempPrivList = TempData["UserLogIn_oUserPrivCol"] as string;
                    if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
                    // De serialize the string to object
                    this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
                    isUserAuthorized = this.oUserLogIn_Priv != null;

                    if (!TempData.ContainsKey("_bckUserLogIn_oUserPrivCol"))
                    {
                        TempData["_bckUserLogIn_oUserPrivCol"] = TempData["UserLogIn_oUserPrivCol"];
                        TempData.Keep();
                    }

                    //
                    //isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
                    //if (isCurrValid)
                    //{
                    //    this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;

                    //    // check permission for Core life...  given the sets of permissions
                    //     isUserAuthorized = true; 
                    //}
                }

                else if (TempData.ContainsKey("_bckUserLogIn_oUserPrivCol"))
                {
                    var tempPrivList = TempData["_bckUserLogIn_oUserPrivCol"] as string;
                    if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
                    // De-serialize the string to object
                    this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
                    isUserAuthorized = this.oUserLogIn_Priv != null;
                }

                else isUserAuthorized = false; // RedirectToAction("LoginUserAcc", "UserLogin");
            }

            return isUserAuthorized;
        }


        private async Task LoadClientDashboardValues()  ///(string clientDBConnString) //, UserProfile oLoggedUser)
        {

            /// Load basic values...
            ///
            /// master control DB
            ViewData["strAppName"] = "Rhema-CMS";
            ViewData["strAppNameMod"] = "Church Dashboard";
            ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(this._oLoggedUser.UserDesc) ? this._oLoggedUser.UserDesc : "[Current user]";
            ViewData["oMSTR_AppGloOwnId_Logged"] = this._oLoggedUser.AppGlobalOwnerId;
            ViewData["oMSTR_ChurchBodyId_Logged"] = this._oLoggedUser.ChurchBodyId;

            ViewData["oCBOrgType_Logged"] = this._oLoggedCB.OrgType;  // CH, CN but subscriber may come from other units like Church Office or Church Group HQ

            ViewData["strModCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedModCodes);
            ViewData["bl_IsModAccessVAA0"] = this.oUserLogIn_Priv.IsModAccessVAA0;
            ViewData["bl_IsModAccessDS00"] = this.oUserLogIn_Priv.IsModAccessDS00;
            ViewData["bl_IsModAccessAC01"] = this.oUserLogIn_Priv.IsModAccessAC01;
            ViewData["bl_IsModAccessMR02"] = this.oUserLogIn_Priv.IsModAccessMR02;
            ViewData["bl_IsModAccessCL03"] = this.oUserLogIn_Priv.IsModAccessCL03;
            ViewData["bl_IsModAccessCA04"] = this.oUserLogIn_Priv.IsModAccessCA04;
            ViewData["bl_IsModAccessFM05"] = this.oUserLogIn_Priv.IsModAccessFM05;
            ViewData["bl_IsModAccessRA06"] = this.oUserLogIn_Priv.IsModAccessRA06;
            ///                
            ViewData["strAssignedRoleCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleCodes);
            ViewData["strAssignedRoleNames"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleNames);
            ViewData["strAssignedGroupNames"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedGroupNames);
            //  ViewData["strAssignedGroupDesc"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedGroupsDesc);
            ViewData["strAssignedPermCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedPermCodes);


            //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRolesDesc);  //_oLoggedRole.RoleName; // "System Adminitrator";
            //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleCodes);  //_oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

            ViewData["strAppCurrUserPhoto_Filename"] = this._oLoggedUser.UserPhoto;
            ///
            /// client control DB
            ViewData["oAppGloOwnId_Logged"] = this._oLoggedAGO.Id;
            ViewData["oChurchBodyId_Logged"] = this._oLoggedCB.Id;
            ViewData["oChurchBodyOrgType_Logged"] = this._oLoggedCB.OrgType;
            ViewData["strClientLogo_Filename"] = this._oLoggedAGO?.ChurchLogo;
            ViewData["strAppLogo_Filename"] = "~/frontend/dist/img/rhema_logo.png"; // oAppGloOwn?.ChurchLogo;
            ViewData["strClientChurchName"] = this._oLoggedAGO.OwnerName;
            ViewData["strClientBranchName"] = this._oLoggedCB.Name;
            ViewData["strClientChurchLevel"] = !string.IsNullOrEmpty(this._oLoggedCB.ChurchLevel?.CustomName) ? this._oLoggedCB.ChurchLevel?.CustomName : this._oLoggedCB.ChurchLevel?.Name;  // Assembly, Presbytery etc
            ViewData["strClientBranchParentName"] = this._oLoggedCB.ParentChurchBody != null ? this._oLoggedCB.ParentChurchBody.Name : "";


            ViewData["CB_SubCongCount"] = String.Format("{0:N0}", 0);
            ViewData["CB_MemListCount"] = String.Format("{0:N0}", 0); // res[0].cnt_tcm); //
            ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 0); // res[0].cnt_tsubs);
            ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tdb);
            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_a);
            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d);
            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d); 
            ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);

        }


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

        private async Task LogUserActivity_ClientUserAuditTrail(UserAuditTrail_CL oUserTrail)  //, string strTempConn = ""
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // var tempCtx = _context;
                //  if (!string.IsNullOrEmpty(clientDBConnString))
                // {

                //// refreshValues... 
                //this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                //_context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

                var _connstr_CL = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId); /// this.GetCL_DBConnString();
                if (!string.IsNullOrEmpty(_connstr_CL))
                {
                    using (var logCtx = new ChurchModelContext(_connstr_CL)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
                    {
                        //logCtx = _context;
                        //var conn = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_context.Database.GetDbConnection().ConnectionString);
                        ////  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
                        //conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                        /////
                        //logCtx.Database.GetDbConnection().ConnectionString = conn.ConnectionString;

                        try
                        {
                            if (logCtx.Database.CanConnect() == false)
                                logCtx.Database.OpenConnection();
                            else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                                logCtx.Database.OpenConnection();

                            // var a = logCtx.Database.GetDbConnection().ConnectionString;
                            // var b = _masterContext.Database.GetDbConnection().ConnectionString;

                            /// 
                            logCtx.UserAuditTrail_CL.Add(oUserTrail);
                            await logCtx.SaveChangesAsync();

                            //logCtx.SaveChanges();

                            logCtx.Entry(oUserTrail).State = EntityState.Detached;
                            ///
                            //DetachAllEntities(logCtx);

                            // close connection
                            logCtx.Database.CloseConnection();

                            //logCtx.Dispose();

                        }

                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }


                //   }
            }
        }




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
        public void DetachAllEntities(ChurchModelContext ctx)
        {
            var changedEntriesCopy = ctx.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        //private static bool IsAncestor_ChurchBody(ChurchBody oAncestorChurchBody, ChurchBody oCurrChurchBody)  // Ancestor of ? ... Taifa -- Grace. swapped -->> Descendant of ?
        //{
        //    if (oAncestorChurchBody == null || oCurrChurchBody == null) return false;
        //    //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

        //    if (oAncestorChurchBody.Id == oCurrChurchBody.ParentChurchBodyId) return true;
        //    if (string.IsNullOrEmpty(oAncestorChurchBody.RootChurchCode) || string.IsNullOrEmpty(oCurrChurchBody.RootChurchCode)) return false;
        //    if (string.Compare(oAncestorChurchBody.RootChurchCode, oCurrChurchBody.RootChurchCode) == 0) return true;  // same CB .. owned

        //    string[] arr = new string[] { oCurrChurchBody.RootChurchCode };
        //    if (oCurrChurchBody.RootChurchCode.Contains("--")) arr = oCurrChurchBody.RootChurchCode.Split("--");  // else it should be the ROOT... and would not get this far

        //    if (arr.Length > 0)
        //    {
        //        var ancestorCode = oAncestorChurchBody.RootChurchCode;
        //        var tempCode = oCurrChurchBody.RootChurchCode;

        //        if (string.Compare(ancestorCode, tempCode) == 0) return true;
        //        var k = arr.Length - 1;
        //        for (var i = arr.Length - 1; i >= 0; i--)
        //        {
        //            if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
        //            if (string.Compare(ancestorCode, tempCode) == 0) return true;
        //        }
        //    }

        //    return false;
        //}
        //private static bool IsAncestor_ChurchBody(string strAncestorRootCode, string strCurrChurchBodyRootCode, int? ancestorChurchBodyId = null, int? currChurchBodyId = null)
        //{
        //    // if (oAncestorChurchBody == null) return false;
        //    //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

        //    if (currChurchBodyId != null && ancestorChurchBodyId == currChurchBodyId) return true;

        //    if (string.IsNullOrEmpty(strAncestorRootCode) || string.IsNullOrEmpty(strCurrChurchBodyRootCode)) return false;
        //    if (string.Compare(strAncestorRootCode, strCurrChurchBodyRootCode) == 0) return true;

        //    string[] arr = new string[] { strCurrChurchBodyRootCode };
        //    if (strCurrChurchBodyRootCode.Contains("--")) arr = strCurrChurchBodyRootCode.Split("--");

        //    if (arr.Length > 0)
        //    {
        //        var ancestorCode = strAncestorRootCode;
        //        var tempCode = strCurrChurchBodyRootCode;

        //        var k = arr.Length - 1;
        //        for (var i = arr.Length - 1; i >= 0; i--)
        //        {
        //            if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
        //            if (string.Compare(ancestorCode, tempCode) == 0) return true;
        //        }
        //    }

        //    return false;
        //}

        //private static bool IsDescendant_ChurchBody(ChurchBody oDescendantChurchBody, ChurchBody oCurrChurchBody)  // Ancestor of ? ... Taifa -- Grace. swapped -->> Descendant of ?
        //{
        //    if (oDescendantChurchBody == null || oCurrChurchBody == null) return false;
        //    //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

        //    if (oDescendantChurchBody.ParentChurchBodyId == oCurrChurchBody.Id) return true;  // father /child
        //    if (string.IsNullOrEmpty(oDescendantChurchBody.RootChurchCode) || string.IsNullOrEmpty(oCurrChurchBody.RootChurchCode)) return false;
        //    if (string.Compare(oDescendantChurchBody.RootChurchCode, oCurrChurchBody.RootChurchCode) == 0) return true;  // same CB .. owned

        //    string[] arr = new string[] { oDescendantChurchBody.RootChurchCode };
        //    if (oDescendantChurchBody.RootChurchCode.Contains("--")) arr = oDescendantChurchBody.RootChurchCode.Split("--");  // else it should be the ROOT... and would not get this far

        //    if (arr.Length > 0)
        //    {
        //        var ancestorCode = oCurrChurchBody.RootChurchCode;
        //        var tempCode = oDescendantChurchBody.RootChurchCode;

        //        if (string.Compare(ancestorCode, tempCode) == 0) return true;   // same CB .. owned
        //        var k = arr.Length - 1;
        //        for (var i = arr.Length - 1; i >= 0; i--)
        //        {
        //            if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
        //            if (string.Compare(ancestorCode, tempCode) == 0) return true;
        //        }
        //    }

        //    return false;
        //}

        //private static bool IsDescendant_ChurchBody(string strDescendantRootCode, string strCurrChurchBodyRootCode, int? descendantChurchBodyId = null, int? currChurchBodyId = null)
        //{
        //    // if (oAncestorChurchBody == null) return false;
        //    //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

        //    if (currChurchBodyId != null && descendantChurchBodyId == currChurchBodyId) return true; // same CB

        //    if (string.IsNullOrEmpty(strDescendantRootCode) || string.IsNullOrEmpty(strCurrChurchBodyRootCode)) return false;
        //    if (string.Compare(strDescendantRootCode, strCurrChurchBodyRootCode) == 0) return true;  // same CB

        //    string[] arr = new string[] { strDescendantRootCode };
        //    if (strDescendantRootCode.Contains("--")) arr = strDescendantRootCode.Split("--");

        //    if (arr.Length > 0)
        //    {
        //        var ancestorCode = strCurrChurchBodyRootCode; // // same CB .. owned
        //        var tempCode = strDescendantRootCode;

        //        var k = arr.Length - 1;
        //        for (var i = arr.Length - 1; i >= 0; i--)
        //        {
        //            if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
        //            if (string.Compare(ancestorCode, tempCode) == 0) return true;
        //        }
        //    }

        //    return false;
        //}
         

        public JsonResult GetChurchGroupsByCategoryType(int? oCurrChuBodyId, bool isCong, bool isGen, bool isInterGen, bool addEmpty = false)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }



            var groupList = new List<SelectListItem>();
            var oCurrChuBody = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();
            if (oCurrChuBody == null) return Json(groupList);

            //groupList = _context.ChurchBody   //.Include(t => t.OwnedByChurchBody)
            //                           .Where(x => x.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && !string.IsNullOrEmpty(x.Name) &&
            //                                    ((isCong == true && x.OrgType == "CN" && x.Id == oCurrChuBody.Id) ||
            //                                    ((isGen == true && x.OrgType == "CG" && x.IsUnitGenerational == true) ||
            //                                     (isInterGen == true && x.OrgType == "CG" && x.IsUnitGenerational == false) &&
            //                            (x.OwnedByChurchBodyId == oCurrChuBody.Id ||
            //                            (x.OwnedByChurchBodyId != oCurrChuBody.Id && x.SharingStatus == "C" && x.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            //                            (x.OwnedByChurchBodyId != oCurrChuBody.Id && x.SharingStatus == "A" && IsAncestor_ChurchBody(x.OwnedByChurchBody, oCurrChuBody))))))
            //                   .Select(c => new SelectListItem()
            //                   {
            //                       Value = c.Id.ToString(),
            //                       Text = c.Name
            //                   })
            //                   .OrderBy(c => c.Text)
            //                   .ToList();


            var oCU_List_1 = _context.ChurchUnit.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                               .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId
                                              ).ToList();  //  && c.Status == "A"  // (c.OrgType != "CP") // || c.OrgType=="SC"
            oCU_List_1 = oCU_List_1.Where(c =>
                               (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

            groupList = oCU_List_1 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                        .OrderBy(c => c.OrderIndex)
                                        .ThenBy(c => c.Name)
                                        .Select(c => new SelectListItem()
                                        {
                                            Value = c.Id.ToString(),
                                            Text = c.Name
                                        })
                                        .ToList();

            /// if (addEmpty) countryList.Insert(0, new CountryRegion { Id = "", Name = "Select" });             
            //return Json(new SelectList(countryList, "Id", "Name"));  

            if (addEmpty) groupList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(groupList);
        }


        private List<MemberBioModel> GetMemBioSnapshotList(int? oAGOid, int? oCBid, int? id = null)
        {
            try
            {
                var oCMList_1 = (  /// members must [internal-current-active]
                                from t_cm in _context.ChurchMember.AsNoTracking()
                                     .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.MemberScope == "I" && (c.Status == "A" || c.Status == "T")) ///  && (!exclCurrCM || (exclCurrCM && c.Id != currCMid))).ToList()
                                from t_mcl in _context.MemberChurchlife.AsNoTracking()
                                     .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.ChurchMemberId == t_cm.Id &&
                                                c.IsDeceased == false && c.IsCurrentMember == true).DefaultIfEmpty()
                                from t_ci in _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == t_cm.AppGlobalOwnerId && x.ChurchBodyId == t_cm.ChurchBodyId &&
                                   (x.Id == t_cm.PrimContactInfoId || (t_cm.PrimContactInfoId == null && x.ChurchMemberId == t_cm.Id && x.IsPrimaryContact == true)))
                                 .Take(1).DefaultIfEmpty()
                                from t_mt in _context.MemberType.AsNoTracking().Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                        .Take(1).DefaultIfEmpty()//.OrderByDescending(y => y.ToDate).Take(1).ToList() : new List<MemberType>()
                                from t_mr in _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                              .Take(1).DefaultIfEmpty() // .FirstOrDefault() }
                                                                        //.OrderBy(y => y.ChurchRank_NVP != null ? y.ChurchRank_NVP.GradeLevel : null).Take(1).ToList() : new List<MemberRank>()
                                from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                              .Take(1).DefaultIfEmpty() //             .OrderBy(y => y.ChurchMemStatus_NVP != null ? y.ChurchMemStatus_NVP.GradeLevel : null).Take(1).ToList() : new List<MemberStatus>()


                                select new MemberBioModel()
                                {
                                    oChurchMember = t_cm,
                                    oChurchMemberId = t_cm.Id,
                                    strMemFullName = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, false, false, true),  /// + " (" + t_cm.GlobalMemberCode + ")",
                                    strMemFullNameExtnd = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, false, false, true) + " (" + t_cm.GlobalMemberCode + ")",
                                    strNameSortBy = t_cm.FirstName + t_cm.MiddleName + t_cm.LastName,
                                    strLocation = t_ci != null ? AppUtilties.GetConcatLinkedEntities(t_ci.Location.Trim(), t_ci.City.Trim()) : "",
                                    strNationality = t_cm.Nationality != null ? t_cm.Nationality.EngName : t_cm.NationalityId,
                                    strGender = GetGenderDesc(t_cm.Gender),
                                    strPhone = !string.IsNullOrEmpty(t_ci.MobilePhone1) ? t_ci.MobilePhone1.Trim() : t_ci.MobilePhone2.Trim(), /// t_ci != null ? AppUtilties.GetConcatLinkedEntities(t_ci.MobilePhone1.Trim(), t_ci.MobilePhone2.Trim()) : "",
                                    strMemGeneralStatus = string.Compare((t_mt != null ? t_mt.MemberTypeCode : null), "C", true) != 0 ?
                                                             (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned") :
                                                             (!string.IsNullOrEmpty(t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : null) : null) ?
                                                             (t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : "Unassigned") : "Unassigned") :
                                                             (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned")),
                                    strPhotoUrl = t_cm.PhotoUrl

                                })
                               .ToList();


                return oCMList_1;
            }
            catch (Exception ex)
            {
                return new List<MemberBioModel>();
            }
        }

        private ChurchAttendanceModel populateLookups_Attend(ChurchAttendanceModel vmLkp, ChurchBody oCurrChuBody, DateTime? attnDate)  //, int? currVisitor = null) // int? currChuBodyId)
        {
            try
            {
                if (vmLkp == null || oCurrChuBody == null) return vmLkp;
                if (vmLkp.oChurchAttendee == null) return vmLkp;

                //  oCurrChuBody = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.Id == oCurrChuBody.Id).FirstOrDefault();

                ///if (oCurrChuBody == null) return vmLkp;

                var oAGOid = oCurrChuBody.AppGlobalOwnerId;
                var oCBid = oCurrChuBody.Id;


                //_context.Configuration.LazyLoadingEnabled = false;   
                //vmLkp.lkpPersTitles = new List<SelectListItem>();
                //foreach (var dl in dlPersTitle) { vmLkp.lkpPersTitles.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                vmLkp.lkpAttendeeTypes = new List<SelectListItem>();
                foreach (var dl in dlMemTypeCodes) { vmLkp.lkpAttendeeTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc}); }  ///, Disabled = dl.Val != "V" 


                vmLkp.lkpChurchLevels = _context.ChurchLevel.Where(c => c.AppGlobalOwnerId == oAGOid)
                                              .OrderByDescending(c => c.LevelIndex)
                                              .Select(c => new SelectListItem()
                                              {
                                                  Value = c.Id.ToString(),
                                                  Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name,
                                                  // Disabled = (numCLIndex == (int?)null || c.LevelIndex < numCLIndex || oCurrChurchBody.OrgType == "CH" || oCurrChurchBody.OrgType == "CN")
                                              })
                                              .ToList();


                vmLkp.lkpAttendeeTypes.OrderBy(c => c.Value); 
                if (vmLkp.oChurchAttendee.AttendeeType == "C")  /// (vmLkp.f_strAttendeeTypeCode == "C")  /// not a member
                {
                    //var oCMList_1 = (  /// members must [internal-current-active]
                    //            from t_cm in _context.ChurchMember.AsNoTracking()
                    //                 .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.MemberScope == "I" && (c.Status == "A" || c.Status == "T")) ///  && (!exclCurrCM || (exclCurrCM && c.Id != currCMid))).ToList()
                    //            from t_mcl in _context.MemberChurchlife.AsNoTracking()
                    //                 .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.ChurchMemberId == t_cm.Id &&
                    //                            c.IsDeceased == false && c.IsCurrentMember == true).DefaultIfEmpty()
                    //            from t_ci in _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == t_cm.AppGlobalOwnerId && x.ChurchBodyId == t_cm.ChurchBodyId &&
                    //               (x.Id == t_cm.PrimContactInfoId || (t_cm.PrimContactInfoId == null && x.ChurchMemberId == t_cm.Id && x.IsPrimaryContact == true)))
                    //             .Take(1).DefaultIfEmpty()
                    //            from t_mt in _context.MemberType.AsNoTracking().Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                    //                    .Take(1).DefaultIfEmpty()//.OrderByDescending(y => y.ToDate).Take(1).ToList() : new List<MemberType>()
                    //            from t_mr in _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                    //                          .Take(1).DefaultIfEmpty() // .FirstOrDefault() }
                    //                                                    //.OrderBy(y => y.ChurchRank_NVP != null ? y.ChurchRank_NVP.GradeLevel : null).Take(1).ToList() : new List<MemberRank>()
                    //            from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                    //                          .Take(1).DefaultIfEmpty() //             .OrderBy(y => y.ChurchMemStatus_NVP != null ? y.ChurchMemStatus_NVP.GradeLevel : null).Take(1).ToList() : new List<MemberStatus>()


                    //            select new MemberBioModel()
                    //            {
                    //                oChurchMember = t_cm,
                    //                oChurchMemberId = t_cm.Id,
                    //                strMemFullName = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, false, false, true),  /// + " (" + t_cm.GlobalMemberCode + ")",
                    //                strMemFullNameExtnd = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, false, false, true) + " (" + t_cm.GlobalMemberCode + ")", 
                    //                strNameSortBy = t_cm.FirstName + t_cm.MiddleName + t_cm.LastName,
                    //                strLocation = t_ci != null ? AppUtilties.GetConcatLinkedEntities(t_ci.Location.Trim(), t_ci.City.Trim()) : "",
                    //                strNationality = t_cm.Nationality != null ? t_cm.Nationality.EngName : t_cm.NationalityId,
                    //                strGender = GetGenderDesc(t_cm.Gender),
                    //                strPhone = !string.IsNullOrEmpty(t_ci.MobilePhone1) ? t_ci.MobilePhone1.Trim() : t_ci.MobilePhone2.Trim(), /// t_ci != null ? AppUtilties.GetConcatLinkedEntities(t_ci.MobilePhone1.Trim(), t_ci.MobilePhone2.Trim()) : "",
                    //                strMemGeneralStatus = string.Compare((t_mt != null ? t_mt.MemberTypeCode : null), "C", true) != 0 ?
                    //                                         (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned") :
                    //                                         (!string.IsNullOrEmpty(t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : null) : null) ?
                    //                                         (t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : "Unassigned") : "Unassigned") :
                    //                                         (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned")),
                    //            })
                    //           .ToList();


                    var oCMList_1 = GetMemBioSnapshotList(oAGOid, oCBid);

                    if (oCMList_1.Count > 0)
                        oCMList_1 = oCMList_1.OrderBy(c => c.strNameSortBy).ToList();

                    vmLkp.lsChurchMemberModel_MemHdr = oCMList_1;

                    /////
                    //vmLkp.lkpChurchMembers_Local = oCMList_1
                    //                 .Select(c => new SelectListItem()
                    //                 {
                    //                     Value = c.oChurchMemberId.ToString(),  
                    //                     Text = c.strMemberFullName
                    //                 })
                    //                 .ToList();
                }
                else
                {
                    /// leave the lookup empty ... for performance! 
                    /// populate when user starts typing guest /visitor details [ filter by fn, mn, ln, mob1, mob2, email, loca, natl_id etc. ]
                    /// AJAX ...

                    vmLkp.lkpGenderTypes = new List<SelectListItem>();
                    foreach (var dl in dlGenderType) { vmLkp.lkpGenderTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                    vmLkp.lkpMaritalStatuses = new List<SelectListItem>();
                    foreach (var dl in dlMaritalStatus) { vmLkp.lkpMaritalStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                    vmLkp.lkpEnrollModes = new List<SelectListItem>();
                    foreach (var dl in dlEnrollStatuses) { vmLkp.lkpEnrollModes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }
             
                    vmLkp.lkpCountries = _context.Country.ToList()  //.Where(c => c.Display == true)
                                     .Select(c => new SelectListItem()
                                     {
                                         Value = c.CtryAlpha3Code, // .ToString(),
                                          Text = c.EngName
                                     })
                                     .OrderBy(c => c.Text)
                                     .ToList();

                    // vmLkp.lkpCtryRegions ... select with country


                    var strNVPCode = "TTL";    // c.Id != oCurrNVP.Id && 
                    var oNVP_List = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //Include(t => t.AppGlobalOwner).
                                                       .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode && c.NVPStatus == "A").ToList();
                    oNVP_List = oNVP_List.Where(c =>
                                       (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    vmLkp.lkpPersTitles = oNVP_List //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                .OrderBy(c => c.OrderIndex)
                                                .ThenBy(c => c.NVPValue)
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.NVPValue,
                                                    Text = c.NVPValue
                                                })
                                                .ToList();


                    var strNVPCode1 = "GEN_AGE_GRP";    // c.Id != oCurrNVP.Id && 
                    var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //Include(t => t.AppGlobalOwner).
                                                       .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode.StartsWith(strNVPCode1)).ToList();

                    oNVP_List_1 = oNVP_List_1.Where(c =>
                                       (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    vmLkp.lkpVisitorAgeBracket = oNVP_List_1 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                .OrderBy(c => c.OrderIndex)
                                                .ThenBy(c => c.NVPNumVal)
                                                .ThenBy(c => c.NVPNumValTo)
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.NVPValue,
                                                    Text = String.Format("{0:N0}", c.NVPNumVal) + " - " + String.Format("{0:N0}", c.NVPNumValTo) + " years"
                                                })
                                                .ToList();
                    vmLkp.lkpVisitorAgeBracket.Insert(0, new SelectListItem { Value = "", Text = "Select" });

             

                    var strNVPCode2 = "CMS";    // Driven by CLA
                    var oNVP_List_2 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                                       .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode2 && c.NVPStatus == "A").ToList();
                    oNVP_List_2 = oNVP_List_2.Where(c =>
                                        (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    vmLkp.lkpChurchMemStatuses = oNVP_List_2 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                .OrderBy(c => c.OrderIndex)
                                                .ThenBy(c => c.NVPValue)
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.NVPValue
                                                })
                                                .ToList();


                    var strNVPCode3 = "CR";    // Driven by CLA
                    var oNVP_List_3 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                                       .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode3 && c.NVPStatus == "A").ToList();
                    oNVP_List_3 = oNVP_List_3.Where(c =>
                                        (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    vmLkp.lkpChurchRanks = oNVP_List_3 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                .OrderBy(c => c.OrderIndex)
                                                .ThenBy(c => c.NVPValue)
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.NVPValue
                                                })
                                                .ToList();

                }

  




               // vmLkp.lkpChurchMembers_Local = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.ChurchBodyId == oCurrChuBody.Id && c.Status == "A" && c.MemberScope == "I")  //Internal [C, L, P, M] only
               //     .OrderBy(c => (c.FirstName + c.MiddleName + c.LastName)).ToList()

               // /// some level of security may be needed ... to allow access to CB membership from external ::: setting... Allow external access (with sister congregations) to relevant church info ...default: Yes

               // //(c.OwnedByChurchBodyId == oCurrChuBody.Id ||
               // // (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
               // // (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody))))

               // .Select(c => new SelectListItem()
               // {
               //     Value = c.Id.ToString(),
               //     Text = GetConcatMemberName(c.Title, c.FirstName, c.MiddleName, c.LastName, false, false, false, false, false) + " [" + c.GlobalMemberCode + "]", /// Dr. Sam ...
               //     //  GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, true, true, false, false, true),  // Sam Darteh
               // })
               // //.OrderBy(c => c.Text)
               // .ToList();

               // /// if (addEmpty) countryList.Insert(0, new CountryRegion { Id = "", Name = "Select" });             
               // //return Json(new SelectList(countryList, "Id", "Name"));  

               //// if (addEmpty) oCMList.Insert(0, new SelectListItem { Value = "", Text = "Select member" });
               // vmLkp.lkpChurchMembers_Local.Insert(0, new SelectListItem { Value = "", Text = "Select" });
             

                //var dtv = attnDate != null ? ((DateTime)attnDate).Date : (DateTime?)null;
                //var CCEList = _context.ChurchCalendarEvent.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ChurchlifeActivity_NVP)
                //    .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&   //c.ChurchBody.CountryId == oCurrChuBody.CountryId && 
                //           ((DateTime)c.EventFrom.Value).Date == dtv).ToList(); // &&

                //vmLkp.lkpChuCalEvents = CCEList.Where(c =>
                //            //(c.EventFrom.HasValue ? ((DateTime)c.EventFrom.Value).Date==dts : c.EventFrom==dts) &&
                //            //((oAttendModel.f_DateAttended.HasValue==false && c.EventFrom == null) || 
                //            //   (oAttendModel.f_DateAttended.HasValue==true && ((DateTime)c.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt)) &&
                //            (c.ChurchBodyId == oCurrChuBody.Id ||
                //            (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                //            (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
                //        )
                //         .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
                //         .ToList()
                //                 .Select(c => new SelectListItem()
                //                 {
                //                     Value = c.Id.ToString(),
                //                     Text = c.Subject + ":- " +
                //                                         (c.IsFullDay == true ?
                //                                             (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                //                                             (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                //                                             (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                //                                             (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                //                                          )
                //                 })
                //                 //.OrderBy(c => c.Text)
                //                 .ToList();

                //vmLkp.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });




                return vmLkp;
            }
            catch (Exception ex)
            {
                return vmLkp;
            }
            
        }



        public JsonResult GetMemberListByCB(int? oAGOid, int? oCBid, bool addEmpty = false, int? currCMid = null, bool exclCurrCM = false)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, oAGOid != null ? oAGOid : this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            var oCMList_1 = (  /// members must [internal-current-active]
                            from t_cm in _context.ChurchMember.AsNoTracking()
                                 .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.MemberScope == "I" && (c.Status == "A" || c.Status == "T") &&
                                 (!exclCurrCM || (exclCurrCM && c.Id != currCMid))).ToList()
                            from t_mcl in _context.MemberChurchlife.AsNoTracking()
                                 .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.ChurchMemberId == t_cm.Id &&
                                            c.IsDeceased == false && c.IsCurrentMember == true).DefaultIfEmpty()

                            select new ChurchMemberModel()
                            {
                                oChurchMember = t_cm,
                                oChurchMemberId = t_cm.Id,
                                strMemberFullName = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, false, false, false) + " [" + t_cm.GlobalMemberCode + "]",
                                strNameSortBy = t_cm.FirstName + t_cm.MiddleName + t_cm.LastName
                            })
                            .ToList();

            if (oCMList_1.Count > 0)
                oCMList_1 = oCMList_1.OrderBy(c => c.strNameSortBy).ToList();

            //var oCMList_1 = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Status == "A" && c.MemberScope == "I")  //Internal [C, L, P, M] only
            //    .OrderBy(c => (c.FirstName + c.MiddleName + c.LastName)).ToList();

            /// some level of security may be needed ... to allow access to CB membership from external ::: setting... Allow external access (with sister congregations) to relevant church info ...default: Yes

            //(c.OwnedByChurchBodyId == oCurrChuBody.Id ||
            // (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            // (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody))))

            var oCMList = oCMList_1
                .Select(c => new SelectListItem()
                {
                    Value = c.oChurchMemberId.ToString(),   //.Id.ToString(),
                    Text = c.strMemberFullName /// GetConcatMemberName(c.Title, c.FirstName, c.MiddleName, c.LastName, false, false, false, false, false) + " [" + c.GlobalMemberCode + "]" , /// Dr. Sam ...
                    //  GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, true, true, false, false, true),  // Sam Darteh
                })
                //.OrderBy(c => c.Text)
                .ToList();

            /// if (addEmpty) countryList.Insert(0, new CountryRegion { Id = "", Name = "Select" });             
            //return Json(new SelectList(countryList, "Id", "Name"));  

            if (addEmpty) oCMList.Insert(0, new SelectListItem { Value = "", Text = "Select member" });
            return Json(oCMList);
        }



        /// <summary>
        ///  [GetTargetCB] browses thru CBs
        ///  works with [GetChurchLevel_ListByChurchLevel]
        ///  always add the ... call the [triggerChurchLevel_TCB]... functions in the modal or form to auto fill....
        /// </summary>
        /// <param name="oChurchLevelId"></param>
        /// <param name="oAppGloOwnId"></param>
        /// <param name="isLowerLevel"></param>
        /// <param name="isInclusive"></param>
        /// <param name="addEmpty"></param>
        /// <returns></returns>
        /// 
        /// 
        /// Begin ----

        [HttpGet]
        public IActionResult GetTargetCB(int? oAppGloOwnId = null, int? oChurchBodyId = null, int? oTargetCBId = null,
                                       string strId_TCB = "", string strName_TCB = "", string strCLId_TCB = "", string strCLName_TCB = "", string strCLTag_TCB = "")
        {
            try
            {
                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return View("_ErrorPage");
                    }
                }


                var oCBModel = new TargetCBModel();
                oCBModel.oAppGloOwnId = oAppGloOwnId;     // current AGO of request
                oCBModel.oChurchBodyId = oChurchBodyId;  // current CB of request 
                ///
                // initialize
                var _oTargetCBId = oTargetCBId;
                oCBModel.oTargetCLId = null; // oTargetCLId; 
                ///
                if (oTargetCBId != null)
                {
                    oCBModel.oTargetCBId = oTargetCBId;     // target CB of request == initial CB ...  
                    // _oTargetCBId = oTargetCBId;
                }
                else
                {
                    oCBModel.oTargetCBId = this._oLoggedCB.Id;
                    _oTargetCBId = this._oLoggedCB.Id;
                }


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


                //if (oCBModel.oTargetCB != null ) return View("_ErrorPage");
                oCBModel.numChurchLevel_Index = _context.ChurchLevel.AsNoTracking().Count(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId); //oCBModel.oTargetCB.ChurchLevel.LevelIndex;
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


                ///
                oCBModel.oTargetCB = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel) //.Include(t => t.ParentChurchBody)
                                    .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.Id == oCBModel.oTargetCBId)
                                    .FirstOrDefault();

                /// pre-load the list...
                if (_oTargetCBId != null && oCBModel.oTargetCB != null)
                {
                    oCBModel.oTargetCLId = oCBModel.oTargetCB.ChurchLevelId;
                    oCBModel.strTargetCB = oCBModel.oTargetCB != null ? oCBModel.oTargetCB.Name : ""; //oCBModel.oTargetCB.Name;
                    oCBModel.strTargetCL = !string.IsNullOrEmpty(oCBModel.oTargetCB.ChurchLevel.CustomName) ? oCBModel.oTargetCB.ChurchLevel.CustomName : oCBModel.oTargetCB.ChurchLevel.Name;

                    oCBModel.arrRootCBCodes = oCBModel.oTargetCB != null ? oCBModel.oTargetCB.RootChurchCode.Split("--").ToList<string>() : new List<string>();
                    // oCBModel.oTargetCLId = currCB.ChurchLevelId;

                    // get the CB path... use either ChurchCode [must av bn ordered! else trouble..] or CB Id to trace path...
                    var masterCBList = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Include(t => t.ChurchLevel)
                                        .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId &&
                                        (oCBModel.oTargetCB == null || (oCBModel.oTargetCB != null && oCBModel.arrRootCBCodes.Contains(c.GlobalChurchCode))))
                                        .ToList();

                    //var masterCBList = (from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                    //                               .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId &&
                    //                               (oCBModel.oTargetCB == null || (oCBModel.oTargetCB != null && oCBModel.arrRootCBCodes.Contains(c.GlobalChurchCode))))
                    //                    from t_cb_p in _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                    //                               .Where(c => c.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && c.Id == t_cb.ParentChurchBodyId).DefaultIfEmpty()
                    //                    select new ChurchBody()
                    //                    {
                    //                        Id = t_cb.Id,
                    //                        AppGlobalOwnerId = t_cb.AppGlobalOwnerId,
                    //                        ChurchLevelId = t_cb.ChurchLevelId,
                    //                        Name = t_cb.Name,
                    //                        GlobalChurchCode = t_cb.GlobalChurchCode,
                    //                        RootChurchCode = t_cb.RootChurchCode,
                    //                        OrgType = t_cb.OrgType,
                    //                        ParentChurchBodyId = t_cb.ParentChurchBodyId,
                    //                        ContactInfoId = t_cb.ContactInfoId,
                    //                        IsWaiveSubscription = t_cb.IsWaiveSubscription,
                    //                        SubscriptionKey = t_cb.SubscriptionKey,
                    //                        LicenseKey = t_cb.LicenseKey,
                    //                        CtryAlpha3Code = t_cb.CtryAlpha3Code,
                    //                        CountryRegionId = t_cb.CountryRegionId,
                    //                        Comments = t_cb.Comments,
                    //                        ChurchWorkStatus = t_cb.ChurchWorkStatus,
                    //                        Status = t_cb.Status,
                    //                        ///
                    //                        ParentChurchBody = t_cb_p,
                    //                    })
                    //                    .ToList();

                    /////
                    //var oCBNextParent = masterCBList.Where(c => c.Id == oCBModel.oTargetCB.Id && c.GlobalChurchCode == oCBModel.oTargetCB.GlobalChurchCode).FirstOrDefault();
                    //var listCount = 0;
                    //if (oCBNextParent != null)
                    //{
                    //    listCount = (oCBNextParent.ChurchLevel != null ? oCBNextParent.ChurchLevel.LevelIndex : listCount);
                    //    listCount = listCount - 1;
                    //    oCBModel.arrRootCBIds[listCount] = oCBNextParent.Id;
                    //    oCBModel.arrRootCBNames[listCount] = oCBNextParent.Name;
                    //}

                    ///
                    var listCount = 0;
                    ChurchBody oCBNextParent = oCBModel.oTargetCB;   // initial CB
                    if (masterCBList.Count > 1)   // leave the root... to append later
                    {
                        if (oCBNextParent == null) oCBNextParent = masterCBList.Where(c => c.Id == oCBModel.oTargetCB.Id && c.GlobalChurchCode == oCBModel.oTargetCB.GlobalChurchCode).FirstOrDefault();

                        if (oCBNextParent != null)
                        {
                            if (oCBNextParent.ChurchLevel == null) oCBNextParent.ChurchLevel = _context.ChurchLevel.Where(c => c.AppGlobalOwnerId == oCBNextParent.AppGlobalOwnerId && c.Id == oCBNextParent.ChurchLevelId).FirstOrDefault();
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
                            oCBNextParent.ParentChurchBody = _context.ChurchBody
                                .Where(c => c.AppGlobalOwnerId == oCBNextParent.AppGlobalOwnerId && c.Id == oCBNextParent.ParentChurchBodyId).FirstOrDefault();

                        var oNextCC = "";
                        if (oCBNextParent.ParentChurchBody != null) oNextCC = oCBNextParent.ParentChurchBody.GlobalChurchCode;
                        ///
                        oCBNextParent = masterCBList.Where(c => c.Id == oCBNextParent.ParentChurchBodyId && c.GlobalChurchCode == oNextCC).FirstOrDefault();
                        if (oCBNextParent == null) break; // loop out

                        oCBModel.arrRootCBIds[i] = oCBNextParent.Id;
                        oCBModel.arrRootCBNames[i] = oCBNextParent.Name;
                    }


                    //for (int i = listCount - 1; i > 0; i--)
                    //{
                    //    oCBNextParent = masterCBList
                    //        .Where(c => c.Id == oCBNextParent.ParentChurchBodyId && c.GlobalChurchCode == oCBNextParent.ParentChurchBody.GlobalChurchCode)
                    //        .FirstOrDefault();
                    //    if (oCBNextParent != null)
                    //    {
                    //        oCBModel.arrRootCBIds[i] = oCBNextParent.Id;
                    //        oCBModel.arrRootCBNames[i] = oCBNextParent.Name;
                    //    }
                    //}


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
                    var oCBNextParent = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)  // .Include(t => t.ParentChurchBody)
                                        .Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId && c.OrgType == "CR" && c.ParentChurchBodyId == null).FirstOrDefault();
                    if (oCBNextParent != null)
                    {
                        oCBModel.arrRootCBIds[0] = oCBNextParent.Id;  // base CB usually the name of the church -- CR
                        oCBModel.arrRootCBNames[0] = oCBNextParent.Name + " (Church Head)";  // base CB usually the name of the church -- CR
                    }
                }


                oCBModel.oCBLevelCount = oCBModel.numChurchLevel_Index; // - 1;        // oCBLevelCount -= 2;  // less requesting CB
                List<ChurchLevel> oCBLevelList = _context.ChurchLevel
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

                oCBModel.lkpChurchLevels = _context.ChurchLevel.Where(c => c.AppGlobalOwnerId == oCBModel.oAppGloOwnId)
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

        public JsonResult GetChurchLevelIndexesByChurchLevel(int? oChurchLevelId, int? oAppGloOwnId, bool addEmpty = false)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, oAppGloOwnId != null ? oAppGloOwnId : this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            //var oCBList = new List<SelectListItem>(); 
            // if (_context == null)
            // if (!InitializeUserLogging()) return Json(new { taskSuccess = false, numResLev = (int?)null, strResList = string.Empty });

            ///
            var oCL = _context.ChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oChurchLevelId).FirstOrDefault();
            var res = oCL != null;
            var _numResLev = oCL != null ? oCL.LevelIndex : (int?)null;
            /// 

            if (oCL != null)
            {
                var oCLs = _context.ChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.LevelIndex <= oCL.LevelIndex).OrderBy(c => c.LevelIndex).ToList();
                var _strRes = "";
                foreach (var oChuLev in oCLs)
                {
                    var strRes = oChuLev != null ? (!string.IsNullOrEmpty(oChuLev.CustomName) ? oChuLev.CustomName : oChuLev.Name) : "";
                    _strRes += strRes + ",";
                }

                _strRes = _strRes.Contains(",") ? _strRes.Remove(_strRes.LastIndexOf(",")) : _strRes;

                //  get the first CB
                var oCB_1 = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                                 .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && // c.Status == "A" && 
                                       c.ChurchLevel.LevelIndex == 1 && c.OrgType == "CR") //c.ChurchLevelId == oCBLevelList[0].Id &&
                                 .FirstOrDefault();

                var _numChurchBodyId_1 = (int?)null; var _strChurchBody_1 = "";
                if (oCB_1 != null)
                { _numChurchBodyId_1 = oCB_1.Id; _strChurchBody_1 = oCB_1.Name + " [Church Root]"; }

                ///
                return Json(new { taskSuccess = res, numResLev = _numResLev, strResList = _strRes, numChurchBodyId_1 = _numChurchBodyId_1, strChurchBody_1 = _strChurchBody_1 });
            }


            return Json(new { taskSuccess = res, numResLev = _numResLev, strResList = "" });
        }

        public JsonResult GetInitChurchBodyListByAppGloOwn(int? oAppGloOwnId, bool addEmpty = false)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, oAppGloOwnId != null ? oAppGloOwnId : this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            var oCBList = new List<SelectListItem>();
            ///
            // if (_context == null)
            // if (!InitializeUserLogging()) return Json(oCBList);

            oCBList = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                       .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchLevel.LevelIndex == 2 && // c.Status == "A" && 
                       (c.OrgType == "CH" || c.OrgType == "CN"))  // c.OrgType == "CR" || 
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

        public JsonResult GetChurchBodyListByParentBody(int? oParentCBId, int? oAppGloOwnId, string strOrgType = null, bool addEmpty = false)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, oAppGloOwnId != null ? oAppGloOwnId : this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            var oCBList = new List<SelectListItem>();
            // if (_context == null)
            // if (!InitializeUserLogging()) return Json(oCBList);


            // list excludes the root -- CR -- Headquarters /Head office
            oCBList = _context.ChurchBody.AsNoTracking()  //.Include(t => t.ChurchLevel)
                .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ParentChurchBodyId == oParentCBId && // c.Status == "A" && 
                        (c.OrgType == "CH" || c.OrgType == "CN"))
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


        /// End -------- 
        ///





        public List<ChurchBody> GetChurchUnits_Attend(ChurchBody parentChurchBody, int? oParentId, string strAttendee, string strLongevity)  //, string OrgType 
        {
            if (parentChurchBody == null) parentChurchBody = _context.ChurchBody.Include(t => t.AppGlobalOwner).Where(c => c.Id == oParentId).FirstOrDefault();
            if (parentChurchBody == null) return new List<ChurchBody>();

            var churchUnits = (
                    from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)//.Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel)//.Include(t => t.ChurchDivisionType)
                                                                                               //.Include(t => t.ParentChurchBody).Include(t => t.SubChurchUnits)
                         .Where(c => c.AppGlobalOwnerId == parentChurchBody.AppGlobalOwnerId && (c.OrgType == "GB" || c.OrgType == "CH" || c.OrgType == "CN") && //(c.OrgType == "CH" || c.OrgType == "CF") &&
                                     ((oParentId == null && c.ParentChurchBodyId == null) || (oParentId != null && c.ParentChurchBodyId == oParentId)) //&&
                                                                                                                                                       //(c.OwnedByChurchBodyId == churchBody.Id ||
                                                                                                                                                       //(c.OwnedByChurchBodyId != churchBody.Id && c.SharingStatus == "C" && c.ParentChurchBodyId == oParentId) ||
                                                                                                                                                       //(c.OwnedByChurchBodyId != churchBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, churchBody)))
                         )
                    select new ChurchBody()
                    {
                        Id = t_cb.Id,
                        Name = t_cb.Name,
                        AppGlobalOwnerId = t_cb.AppGlobalOwnerId,
                        AppGlobalOwner = t_cb.AppGlobalOwner,
                        ParentChurchBodyId = t_cb.ParentChurchBodyId,
                        ParentChurchBody = t_cb.ParentChurchBody,
                        OrgType = t_cb.OrgType, 
                        //ChurchUnitTypeId = t_cb.ChurchUnitTypeId,
                        OwnedByChurchBodyId = t_cb.OwnedByChurchBodyId,
                        numChurchLevel_Index = t_cb.ChurchLevel.LevelIndex,

                        // CH_TotSubUnits = _context.ChurchBody.Where(y => y.ParentChurchBodyId == t_cb.Id && (y.OrgType == "GB" || y.OrgType == "CN") && (y.ChurchType=="CH" || y.ChurchType == "CF") && y.Status=="A").Count(), // Is_Activated is for COMMIT tasks  t_cb.SubChurchUnits,
                        CH_TotSubUnits = _context.ChurchBody
                                                  .Where(y => y.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && (y.OrgType == "GB" || y.OrgType == "CH" || y.OrgType == "CN") && // (y.ChurchType == "CH" || y.ChurchType == "CF") && 
                                                          y.Status == "A" && y.Id != t_cb.Id &&
                                                         (y.ParentChurchBodyId == t_cb.Id || y.RootChurchCode.Contains(t_cb.RootChurchCode))
                                                          ).Count(),

                        CH_TotList_Attendees = (
                                          from t_cb_1 in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == t_cb.AppGlobalOwnerId &&
                                                 x.OrgType == "CN" &&  //(x.OrgType == "GB" || x.OrgType == "CN") && (x.ChurchType == "CH" || x.ChurchType == "CF") &&
                                                 (x.Id == t_cb.Id || x.ParentChurchBodyId == t_cb.Id || x.RootChurchCode.Contains(t_cb.RootChurchCode)))
                                              //from t_cm in _context.ChurchAttendAttendee.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == t_cb_1.AppGlobalOwnerId && x.ChurchBodyId == t_cb_1.Id &&
                                          from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.ChurchEventDetail)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == t_cb_1.AppGlobalOwnerId &&
                                                   x.AttendeeType == strAttendee && (strLongevity == "H" || (strLongevity == "C" && x.DateAttended.Value.ToShortDateString() == DateTime.Now.Date.ToShortDateString())) &&
                                                   (x.ChurchBody.OrgType == "GB" || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CN") && // (x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                   (x.ChurchBodyId == t_cb_1.Id || x.ChurchBody.ParentChurchBodyId == t_cb_1.Id || x.ChurchBody.RootChurchCode.Contains(t_cb_1.RootChurchCode)))
                                          select t_caa
                                      ).ToList(),



                        CH_TotMaleMem = 0,
                        CH_TotFemMem = 0,
                        CH_TotOtherMem = 0,
                        oCH_InChargeMLR = null,
                        strCH_InCharge = "",
                        
                    }
                    )
                    .OrderBy(c => c.numChurchLevel_Index).ThenBy(c => c.OrgType) //.ThenBy(c => c.strChurchUnitType) //.ThenBy(c => c.strParentUnit)
                    .ThenBy(c => c.Name)
                    .ToList();

            return churchUnits;
        }

        public List<ChurchBody> GetChurchUnits_Headcount(ChurchBody parentChurchBody, int? oParentId, string strCountType, string strLongevity)  //, string OrgType 
        {
            if (parentChurchBody == null) parentChurchBody = _context.ChurchBody.Include(t => t.AppGlobalOwner).Where(c => c.Id == oParentId).FirstOrDefault();
            if (parentChurchBody == null) return new List<ChurchBody>();

            var churchUnits = (
                    from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)//.Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel)//.Include(t => t.ChurchDivisionType)
                                                                                               //.Include(t => t.ParentChurchBody).Include(t => t.SubChurchUnits)
                         .Where(c => c.AppGlobalOwnerId == parentChurchBody.AppGlobalOwnerId && (c.OrgType == "GB" || c.OrgType == "CH" || c.OrgType == "CN") && // (c.ChurchType == "CH" || c.ChurchType == "CF") &&
                                     ((oParentId == null && c.ParentChurchBodyId == null) || (oParentId != null && c.ParentChurchBodyId == oParentId)) //&&
                                                                                                                                                       //(c.OwnedByChurchBodyId == churchBody.Id ||
                                                                                                                                                       //(c.OwnedByChurchBodyId != churchBody.Id && c.SharingStatus == "C" && c.ParentChurchBodyId == oParentId) ||
                                                                                                                                                       //(c.OwnedByChurchBodyId != churchBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, churchBody)))
                         )
                    select new ChurchBody()
                    {
                        Id = t_cb.Id,
                        Name = t_cb.Name,
                        AppGlobalOwnerId = t_cb.AppGlobalOwnerId,
                        AppGlobalOwner = t_cb.AppGlobalOwner,
                        ParentChurchBodyId = t_cb.ParentChurchBodyId,
                        ParentChurchBody = t_cb.ParentChurchBody,
                        OrgType = t_cb.OrgType,
                       // ChurchUnitTypeId = t_cb.ChurchUnitTypeId,
                        OwnedByChurchBodyId = t_cb.OwnedByChurchBodyId,
                        numChurchLevel_Index = t_cb.ChurchLevel.LevelIndex,

                        // CH_TotSubUnits = _context.ChurchBody.Where(y => y.ParentChurchBodyId == t_cb.Id && (y.OrgType == "GB" || y.OrgType == "CN") && (y.ChurchType=="CH" || y.ChurchType == "CF") && y.Status=="A").Count(), // Is_Activated is for COMMIT tasks  t_cb.SubChurchUnits,
                        CH_TotSubUnits = _context.ChurchBody
                                                  .Where(y => y.AppGlobalOwnerId == t_cb.AppGlobalOwnerId && (y.OrgType == "GB" || y.OrgType == "CH" || y.OrgType == "CN") &&  // (y.ChurchType == "CH" || y.ChurchType == "CF") && 
                                                          y.Status == "A" && y.Id != t_cb.Id &&
                                                         (y.ParentChurchBodyId == t_cb.Id || y.RootChurchCode.Contains(t_cb.RootChurchCode))
                                                          ).Count(),

                        CH_TotList_Headcount = (
                                          from t_cb_1 in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == t_cb.AppGlobalOwnerId &&
                                                 x.OrgType == "CN" && //x.ChurchType == "CF" &&  //(x.OrgType == "GB" || x.OrgType == "CN") && (x.ChurchType == "CH" || x.ChurchType == "CF") &&
                                                 (x.Id == t_cb.Id || x.ParentChurchBodyId == t_cb.Id || x.RootChurchCode.Contains(t_cb.RootChurchCode)))
                                              //from t_cm in _context.ChurchAttendAttendee.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == t_cb_1.AppGlobalOwnerId && x.ChurchBodyId == t_cb_1.Id &&
                                          from t_caa in _context.ChurchAttendHeadCount.AsNoTracking().Include(t => t.ChurchEvent)  //.Include(t => t.ChurchMember)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == t_cb_1.AppGlobalOwnerId &&
                                                   x.CountType == strCountType && (strLongevity == "H" || (strLongevity == "C" && x.CountDate.Value.ToShortDateString() == DateTime.Now.Date.ToShortDateString())) &&
                                                   (x.ChurchBody.OrgType == "GB" || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CN") && //(x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                   (x.ChurchBodyId == t_cb_1.Id || x.ChurchBody.ParentChurchBodyId == t_cb_1.Id || x.ChurchBody.RootChurchCode.Contains(t_cb_1.RootChurchCode)))
                                          select t_caa
                                      ).ToList(),
                        CH_TotMaleMem = 0,
                        CH_TotFemMem = 0,
                        CH_TotOtherMem = 0,
                        oCH_InChargeMLR = null,
                        strCH_InCharge = ""
                    }
                    )
                    .OrderBy(c => c.numChurchLevel_Index).ThenBy(c => c.OrgType)  //.ThenBy(c => c.strChurchUnitType).ThenBy(c => c.strParentUnit)
                    .ThenBy(c => c.Name)
                    .ToList();

            return churchUnits;
        }



        public List<ChurchAttendanceModel> ReturnChurchAttendeesList_MemEdit(ChurchBody oChurchBody, int? churchEventId, DateTime? eventDate)  // later... customize ... create a class == merge Member + Attendance
        {   //Get member profile list      ... current members ... available

            // if (churchEventId == null) return new List<ChurchAttendanceModel>();
            try
            {

                var oChurchAttendVMList =

                (from m_data in
                       (
                            from t_cm in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo).Where(x => x.ChurchBodyId == oChurchBody.Id) //
                            join t_cbx in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId) on t_cm.ChurchBodyId equals t_cbx.Id into _tcb
                            from t_cb in _tcb

                                //join t_caax in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality).Include(t=>t.AgeBracket) 
                                //                   .Where(x => x.ChurchBodyId == oChurchBody.Id  && x.AttendeeType == "C" && x.Id == churchEventId) on t_cm.Id equals t_caax.ChurchMemberId into _tcaa
                                //from t_caa in _tcaa.DefaultIfEmpty()
                                //join t_ccex in _context.ChurchCalendarEvent.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id) on t_caa.ChurchEventId equals t_ccex.Id into _cce
                                //from t_cce in _cce.DefaultIfEmpty()
                                //member stuff

                            join t_mcux in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id && x.IsCurrUnit == true && x.ChurchUnit.IsUnitGen == true)
                                          on t_cm.Id equals t_mcux.ChurchMemberId into _tmcu
                            from t_mcu in _tmcu.DefaultIfEmpty()
                            join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.IsUnitGen == true) on t_mcu.ChurchUnitId equals t_cbgx.Id into _tcbg
                            from t_cbg in _tcbg.DefaultIfEmpty()
                            join t_mlax in _context.ContactInfo.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id) on t_cm.PrimContactInfoId equals t_mlax.Id into _tmla
                            from t_mla in _tmla.DefaultIfEmpty()

                            select new ChurchAttendanceModel
                            {
                                oChurchBody = t_cb,
                                oChurchBodyId = t_cb.Id,
                                strCongregation = t_cb.Name,
                                // oChurchAttend = t_caa, 
                                oChurchMember = t_cm,
                                oChurchMemberId = (int?)t_cm.Id,
                                strAttendeeName = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false),
                                //oChurchAttend = t_caa,
                                // oAttend_Id = t_caa != null ? t_caa.Id : (int?)null, 
                                // f_ChkMemAttend = t_caa != null, // no attend entry  made yet                                
                                strAttendeeTypeDesc = GetMemTypeDesc("C"),
                                //f_ChurchEventId = t_cce != null ? t_cce.Id : (int?)null,
                                //strChurchEventDesc = t_cce != null ? (t_cce.Subject + ":- " +
                                //                             (t_cce.IsFullDay == true ?
                                //                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                //                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                //                                (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                //                                (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                //                             )) : "",
                                strGender = t_cm.Gender,
                                strNationality = t_cm.Nationality != null ? t_cm.Nationality.EngName : "",
                                strResidenceLoc = t_cm.ContactInfo != null ? t_cm.ContactInfo.Location : "",
                                strAgeGroup = t_cbg != null ? t_cbg.Name : "" , //(!string.IsNullOrEmpty(t_cbg.Name) ? t_cbg.TagName : t_cbg.Name) : "",
                                strPhone = t_cm.ContactInfo != null ? t_cm.ContactInfo.MobilePhone1 : "",
                                // strDateAttended = t_caa.DateAttended != null ? DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                                // f_DateAttended = t_caa.DateAttended,
                                // decTempRec = t_caa != null ? (decimal)t_caa.TempRec : (decimal?)null,
                                //   strTempRec = t_caa != null ? t_caa.TempRec.Value.ToString("{0:#.#}") : ""

                            })
                               //.Distinct()
                               //  .OrderBy(x => x.strAttendeeName)
                               .ToList()

                 join t_caax in _context.ChurchAttendAttendee.AsNoTracking() //.Include(t => t.Nationality).Include(t => t.AgeBracket)
                                           .Where(x => x.ChurchBodyId == oChurchBody.Id && x.AttendeeType == "C" && x.ChurchEventDetailId == churchEventId && x.DateAttended.Value.ToShortDateString() == eventDate.Value.ToShortDateString()) on m_data.oChurchMemberId equals t_caax.ChurchMemberId into _tcaa
                 from t_caa in _tcaa.DefaultIfEmpty()
                     //join t_ccex in _context.ChurchCalendarEvent.AsNoTracking().Where(x => x.ChurchBodyId ==  oChurchBody.Id) on t_caa.ChurchEventId equals t_ccex.Id into _cce
                     //from t_cce in _cce //.DefaultIfEmpty()

                 select new ChurchAttendanceModel
                 {
                     oChurchBody = m_data.oChurchBody,
                     oChurchBodyId = m_data.oChurchBodyId,
                     strCongregation = m_data.strCongregation,
                     oChurchMember = m_data.oChurchMember,
                     oChurchMemberId = m_data.oChurchMemberId,
                     strAttendeeName = m_data.strAttendeeName,
                     strGender = m_data.strGender,
                     strNationality = m_data.strNationality,
                     strResidenceLoc = m_data.strResidenceLoc,
                     strAgeGroup = m_data.strAgeGroup,
                     strPhone = m_data.strPhone,
                     strAttendeeTypeDesc = m_data.strAttendeeTypeDesc,
                     //
                     oChurchAttendee = t_caa,
                     oAttend_Id = t_caa != null ? t_caa.Id : (int?)null,
                     //
                     f_ChkMemAttend = t_caa != null, // no attend entry  made yet  
                     f_ChurchEventDetailId = t_caa != null ? t_caa.ChurchEventDetailId : (int?)null,
                     f_DateAttended = t_caa != null ? t_caa.DateAttended : DateTime.Now,
                     //
                     strDateAttended = t_caa != null ? (t_caa.DateAttended != null ? DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "") : "",
                     decTempRec = t_caa != null ? (t_caa.TempCelc != null ? (decimal)t_caa.TempCelc : (decimal?)null) : (decimal?)null,
                     strTempRec = t_caa != null ? (t_caa.TempCelc != null ? t_caa.TempCelc.Value.ToString("{0:#.#}") : "") : "",

                     decPersWt = t_caa != null ? (t_caa.PersKgWt != null ? (decimal)t_caa.PersKgWt : (decimal?)null) : (decimal?)null,
                     strPersWt = t_caa != null ? (t_caa.PersKgWt != null ? t_caa.PersKgWt.Value.ToString("{0:#.#}") : "") : "",

                     decPersBPMin = t_caa != null ? (t_caa.PersBPMin != null ? (decimal)t_caa.PersBPMin : (decimal?)null) : (decimal?)null,
                     strPersBPMin = t_caa != null ? (t_caa.PersBPMin != null ? t_caa.PersBPMin.Value.ToString("{0:#.#}") : "") : "",

                     decPersBPMax = t_caa != null ? (t_caa.PersBPMax != null ? (decimal)t_caa.PersBPMax : (decimal?)null) : (decimal?)null,
                     strPersBPMax = t_caa != null ? (t_caa.PersBPMax != null ? t_caa.PersBPMax.Value.ToString("{0:#.#}") : "") : "",

                     //f_ChurchEventId = t_cce != null ? t_cce.Id : (int?)null,
                     //strChurchEventDesc = t_cce != null ? (t_cce.Subject + ":- " +
                     //                                          (t_cce.IsFullDay == true ?
                     //                                             (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                     //                                             (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                     //                                             (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                     //                                             (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                     //                                          )) : ""
                 })
                                 //.Distinct()
                                 //.OrderByDescending(x => x.oChurchAttend.DateAttended).ThenByDescending(x => x.dtEventFrom).ThenByDescending(x => x.dtEventTo).ThenBy(x => x.strAttendeeName)       
                                 .OrderBy(x => x.strAttendeeName)
                                .ToList();

            return oChurchAttendVMList;


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ChurchAttendanceModel> ReturnChurchAttendeesList_MemEditById(ChurchBody oChurchBody, int id)  // later... customize ... create a class == merge Member + Attendance
        {   //Get member profile list      ... current members ... available

            var oChurchAttendVMList = (
                             from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality).Include(t => t.AgeBracket)
                                                .Where(x => x.ChurchBodyId == oChurchBody.Id && x.AttendeeType == "C" && x.Id == id)
                             join t_cmx in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo).Where(x => x.ChurchBodyId == oChurchBody.Id)
                                            on t_caa.ChurchMemberId equals t_cmx.Id into _tcm
                             from t_cm in _tcm //DefaultIfEmpty()
                             join t_cbx in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId) on t_cm.ChurchBodyId equals t_cbx.Id into _tcb
                             from t_cb in _tcb
                             join t_ccedx in _context.ChurchCalendarEventDetail.AsNoTracking().Include(t=> t.ChurchCalendarEvent).Where(x => x.ChurchBodyId == oChurchBody.Id) on t_caa.ChurchEventDetailId equals t_ccedx.Id into _cced
                             from t_cced in _cced //.DefaultIfEmpty()
                                 //member stuff
                             join t_mcux in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id && x.IsCurrUnit == true && x.ChurchUnit.IsUnitGen == true)
                                            on t_cm.Id equals t_mcux.ChurchMemberId into _tmcu
                             from t_mcu in _tmcu.DefaultIfEmpty()
                             join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.IsUnitGen == true) on t_mcu.ChurchUnitId equals t_cbgx.Id into _tcbg
                             from t_cbg in _tcbg.DefaultIfEmpty()  //t_mcu.ChurchUnitI
                                                                   //join t_mlax in _context.ContactInfo.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id) on t_cm.ContactInfoId equals t_mlax.Id into _tmla
                                                                   //from t_mla in _tmla.DefaultIfEmpty()

                             select new ChurchAttendanceModel
                             {
                                 oChurchBody = t_cb,
                                 oChurchBodyId = t_cb.Id,
                                 // strCongregation = t_cb.Name,
                                 // oChurchAttend = t_caa, 
                                 oChurchMember = t_cm,
                                 oChurchMemberId = t_cm != null ? t_cm.Id : (int?)null,
                                 oChurchAttendee = t_caa,
                                 oAttend_Id = t_caa != null ? t_caa.Id : (int?)null,
                                 // f_ChkMemAttend = t_caa != null, // no attend entry  made yet
                                 strAttendeeName = AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false),
                                 strAttendeeTypeDesc = GetMemTypeDesc(t_caa.AttendeeType),
                                 // f_ChurchEventId = t_cce != null ? t_cce.Id : (int?)null,

                                 strChurchEventDesc_Hdr = t_cced.ChurchCalendarEvent != null ? t_cced.ChurchCalendarEvent.Subject + 
                                                            (t_caa.DateAttended != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", t_caa.DateAttended) : "") : "",

                                 //(t_cced.ChurchCalendarEvent.Subject + ":- " +
                                 //                                 (t_cced.ChurchCalendarEvent.IsFullDay == true ?
                                 //                                    (t_cced.ChurchCalendarEvent.EventFrom != null ? DateTime.Parse(t_cced.ChurchCalendarEvent.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                 //                                    (t_cced.ChurchCalendarEvent.EventFrom != null ? DateTime.Parse(t_cced.ChurchCalendarEvent.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                 //                                    (t_cced.ChurchCalendarEvent.EventFrom != null && t_cced.ChurchCalendarEvent.EventTo != null ? " -- " : "") +
                                 //                                    (t_cced.ChurchCalendarEvent.EventTo != null ? DateTime.Parse(t_cced.ChurchCalendarEvent.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                 //                                 )) : "",

                                 strChurchEventDesc = t_cced.EventDescription + (t_caa.DateAttended != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", t_caa.DateAttended) : ""),


                                 //t_cced.EventDescription + ":- " +
                                 //                                 (t_cced.IsFullDay == true ?
                                 //                                    (t_cced.EventFrom != null ? DateTime.Parse(t_cced.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                 //                                    (t_cced.EventFrom != null ? DateTime.Parse(t_cced.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                 //                                    (t_cced.EventFrom != null && t_cced.EventTo != null ? " -- " : "") +
                                 //                                    (t_cced.EventTo != null ? DateTime.Parse(t_cced.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                 //                                 ),

                                 strGender = t_caa.AttendeeType == "C" ? t_cm.Gender : t_caa.Gender,
                                 strNationality = t_caa.AttendeeType == "C" ?
                                                            (t_cm.Nationality != null ? t_cm.Nationality.EngName : "") :
                                                            t_caa.Nationality.EngName,
                                 strResidenceLoc = t_caa.AttendeeType == "C" ?
                                                            (t_cm.ContactInfo != null ? t_cm.ContactInfo.Location : "") :
                                                            t_caa.ResidenceLoc,
                                 strAgeGroup = t_caa.AttendeeType == "C" ?
                                                            (t_cbg != null ? t_cbg.Name : "") : // !string.IsNullOrEmpty(t_cbg.TagName) ? t_cbg.TagName : t_cbg.Name : "") :
                                                            (t_caa.AgeBracket != null ? t_caa.AgeBracket.NVPValue : ""),
                                 strPhone = t_caa.AttendeeType == "C" ?
                                                            (t_cm.ContactInfo != null ? t_cm.ContactInfo.MobilePhone1 : "") :
                                                            t_caa.MobilePhone,
                                 strDateAttended = t_caa.DateAttended != null ?
                                                            (DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture)) : "",

                                 f_ChkMemAttend = t_caa != null, // no attend entry  made yet  
                                 f_ChurchEventDetailId = t_caa.ChurchEventDetailId,
                                 f_DateAttended = t_caa.DateAttended,

                                 decTempRec = t_caa.TempCelc != null ? (decimal)t_caa.TempCelc : (decimal?)null,
                                 strTempRec = t_caa.TempCelc != null ? t_caa.TempCelc.Value.ToString("{0:#.#}") : "",

                                 decPersWt = t_caa.PersKgWt != null ? (decimal)t_caa.PersKgWt : (decimal?)null,
                                 strPersWt = t_caa.PersKgWt != null ? t_caa.PersKgWt.Value.ToString("{0:#.#}") : "",

                                 decPersBPMin = t_caa.PersBPMin != null ? (decimal)t_caa.PersBPMin : (decimal?)null,
                                 strPersBPMin = t_caa.PersBPMin != null ? t_caa.PersBPMin.Value.ToString("{0:#.#}") : "",

                                 decPersBPMax = t_caa.PersBPMax != null ? (decimal)t_caa.PersBPMax : (decimal?)null,
                                 strPersBPMax = t_caa.PersBPMax != null ? t_caa.PersBPMax.Value.ToString("{0:#.#}") : ""
                             })
                                //.Distinct()
                                .OrderByDescending(x => x.f_DateAttended).ThenBy(x => x.strAttendeeName)
                                .ToList();


            return oChurchAttendVMList;
        }


        public List<ChurchAttendanceModel> ReturnChurchAttendeesList(int? oAppGloOwnId, int? oCBid, string strAttendee,  ///string strLongevity,
                                                            int? eventId = null, DateTime? dtMinAttend = null, DateTime? dtMaxAttend = null)
        {   //Get member profile list      ... current members ... available
            try
            {
                if (dtMinAttend == null) dtMinAttend = DateTime.Now.Date.AddMonths(-1); ///.Date.AddDays(-28);
                if (dtMaxAttend == null) dtMaxAttend = DateTime.Now;

                var oChurchAttendVMList_1 = _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality)
                                                      .Where(x => x.ChurchBody.AppGlobalOwnerId == oAppGloOwnId && x.ChurchBodyId == oCBid && x.ChurchBody.OrgType == "CN" &&   /// for now restrict at CB base || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CR") && // (x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                      (x.AttendeeType == strAttendee || strAttendee == null) && (x.ChurchEventDetailId == eventId || eventId == null) &&        ///  || (x.AttendeeType == strAttendee)) &&   /// last week
                                                      (x.DateAttended >= dtMinAttend && x.DateAttended <= dtMaxAttend)                                                          /// eventId == null  == null || (x.AttendeeType == strAttendee)) &&   /// last week
                                                      ).ToList();                                                                                                              /// last week  strLongevity == "H" || (strLongevity == "C" && 
                                                                                                                                                                                /// (x.ChurchBodyId == oChurchBody.Id || x.ChurchBody.ParentChurchBodyId == oChurchBody.Id || x.ChurchBody.RootChurchCode.Contains(oChurchBody.RootChurchCode))
                                                            

                /// oChurchAttendVMList_1 = oChurchAttendVMList_1.Where(x => x.ChurchBodyId == oChurchBody.Id || x.ChurchBody.ParentChurchBodyId == oChurchBody.Id || x.ChurchBody.RootChurchCode.Contains(oChurchBody.RootChurchCode)).ToList();
                /// _oCPRDefault != null ? String.Format("{0:dddd, MMMM d, yyyy}", _oCPRDefault.FromDate.Value) : ""
                /// 
                var oChurchAttendVMList = (
                                 from t_caa in oChurchAttendVMList_1

                                 join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                                 from t_cb in _tcb                                
                                 join t_ccedx in _context.ChurchCalendarEventDetail.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ChurchCalendarEvent)
                                    on t_caa.ChurchEventDetailId equals t_ccedx.Id into _cced
                                 from t_cced in _cced   /// .DefaultIfEmpty()

                                 join t_cmx in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo) on t_caa.ChurchMemberId equals t_cmx.Id into _tcm
                                 from t_cm in _tcm.DefaultIfEmpty()
                                     //member stuff
                                 join t_mcux in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == oCBid && x.IsCurrUnit == true && x.ChurchUnit.IsUnitGen == true)
                                                on t_cm.Id equals t_mcux.ChurchMemberId into _tmcu
                                 from t_mcu in _tmcu.DefaultIfEmpty()
                                 join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAppGloOwnId && x.IsUnitGen == true) 
                                    on (t_mcu != null ? t_mcu.ChurchUnitId : (int?)null) equals t_cbgx.Id into _tcbg
                                 from t_cbg in _tcbg.DefaultIfEmpty()

                                 select new ChurchAttendanceModel
                                 {
                                     oChurchBody = t_cb,
                                     oChurchBodyId = t_cb.Id, 
                                     strCongregation = t_cb.Name,
                                     oChurchAttendee = t_caa,
                                     oAttend_Id = (int?)t_caa.Id,
                                     oChurchMember = t_cm,
                                     f_oAttendRefId = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.Id : (int?)null) : t_caa.Id,
                                     oChurchMemberId = t_caa.AttendeeType == "C" ? (t_cm != null ? (int?)t_cm.Id : (int?)null) : (int?)null,

                                     strAttendeeName = t_caa.AttendeeType == "C" ? (t_cm != null ? AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false) : "") :
                                                                       AppUtilties.GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false),
                                      
                                     f_ChurchEventDetailId = t_caa.ChurchEventDetailId,
                                     f_DateAttended = t_caa.DateAttended,
                                     f_strAttendeeTypeCode = t_caa.AttendeeType,

                                     //strAttendeeName = t_caa != null ? (t_caa.AttendeeType == "C" ? (t_cm == null ? (GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false)) : 
                                     //                                  ("")) : (GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false))) : (""),

                                     ////(t_caa.AttendeeType == "C" ? (t_cm ==null ? (GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false)) : "") : "") :
                                     ////                       t_caa != null ? t_cm == null ? GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false) : "": "",


                                     strAttendeeTypeDesc = GetMemTypeDesc(t_caa.AttendeeType),

                                     strChurchEventDesc_Hdr = t_cced.ChurchCalendarEvent != null ? t_cced.ChurchCalendarEvent.Subject +
                                                            (t_caa.DateAttended != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", t_caa.DateAttended) : "") : "", 
                                     strChurchEventDesc = t_cced.EventDescription + (t_caa.DateAttended != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", t_caa.DateAttended) : ""),

                                     dtEventFrom = t_cced != null ? t_cced.EventFrom : (DateTime?)null,
                                     dtEventTo = t_cced != null ? t_cced.EventTo : (DateTime?)null,

                                     strGender = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.Gender : "") : t_caa.Gender,
                                     strGenderDesc = GetGenderDesc(t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.Gender : "") : t_caa.Gender),

                                     strAgeGroup = t_caa.AttendeeType == "C" ? (t_cbg != null ? t_cbg.Name : "") : t_caa.AgeBracket != null ? t_caa.AgeBracket.NVPValue : "",
                                     strNationality = t_caa.AttendeeType == "C" ? (t_cbg != null ? t_cbg.Name : "") : t_caa.Nationality != null ? t_caa.Nationality.EngName : "",
                                     strResidenceLoc = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.ContactInfo != null ? t_cm.ContactInfo.Location : "" : "") : t_caa.ResidenceLoc,
                                     strPhone = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.ContactInfo != null ? t_cm.ContactInfo.MobilePhone1 : "" : "") : t_caa.MobilePhone,
                                     strDateAttended = t_caa.DateAttended != null ? String.Format("{0:ddd, d-MMM-yyyy}", t_caa.DateAttended) : "", // DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",

                                     decTempRec = t_caa.TempCelc != null ? (decimal)t_caa.TempCelc : (decimal?)null,
                                     strTempRec = t_caa.TempCelc != null ? t_caa.TempCelc.Value.ToString("{0:#.#}") : "",

                                     decPersWt = t_caa.PersKgWt != null ? (decimal)t_caa.PersKgWt : (decimal?)null,
                                     strPersWt = t_caa.PersKgWt != null ? t_caa.PersKgWt.Value.ToString("{0:#.#}") : "",

                                     decPersBPMin = t_caa.PersBPMin != null ? (decimal)t_caa.PersBPMin : (decimal?)null,
                                     strPersBPMin = t_caa.PersBPMin != null ? t_caa.PersBPMin.Value.ToString("{0:#.#}") : "",

                                     decPersBPMax = t_caa.PersBPMax != null ? (decimal)t_caa.PersBPMax : (decimal?)null,
                                     strPersBPMax = t_caa.PersBPMax != null ? t_caa.PersBPMax.Value.ToString("{0:#.#}") : ""

                                 })
                                    //.Distinct()
                                    //.OrderByDescending(x => x.oChurchAttendee.DateAttended).ThenByDescending(x => x.dtEventFrom).ThenByDescending(x => x.dtEventTo).ThenBy(x => x.strAttendeeName)
                                    .ToList();

                if (oChurchAttendVMList.Count > 0)
                    oChurchAttendVMList = oChurchAttendVMList
                        .OrderByDescending(x => x.oChurchAttendee.DateAttended).ThenByDescending(x => x.dtEventFrom).ThenByDescending(x => x.dtEventTo).ThenBy(x => x.strAttendeeName).ToList();

                return oChurchAttendVMList;
            }
            catch (Exception ex)
            {
                return null;
            }            
        }


        public List<ChurchAttendanceModel> ReturnChurchAttendeesListByFilters(ChurchBody oChurchBody, string strAttendee, string strLongevity,
                                        string strFirstName, string strMiddleName, string strLastName, string strAttendLoc, string strAttnPho)
        {   //Get member profile list      ... current members ... available
            // strLongevity = "H";

            if (string.IsNullOrEmpty(strFirstName) && string.IsNullOrEmpty(strMiddleName) && string.IsNullOrEmpty(strLastName) && string.IsNullOrEmpty(strAttendLoc) && string.IsNullOrEmpty(strAttnPho))
                return new List<ChurchAttendanceModel>();

            var oChurchAttendVMList = (
                             from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.AttendeeType == strAttendee &&
                                                   // (strLongevity == "H" || (strLongevity == "C" && x.DateAttended == DateTime.Now.Date)) &&
                                                   (x.ChurchBody.OrgType == "GB" || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CN") && //(x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                   (x.ChurchBodyId == oChurchBody.Id || x.ChurchBody.ParentChurchBodyId == oChurchBody.Id || x.ChurchBody.RootChurchCode.Contains(oChurchBody.RootChurchCode)))

                             join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                             from t_cb in _tcb
                             join t_cmx in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo) on t_caa.ChurchMemberId equals t_cmx.Id into _tcm
                             from t_cm in _tcm.DefaultIfEmpty()
                             join t_ccedx in _context.ChurchCalendarEventDetail.AsNoTracking() on t_caa.ChurchEventDetailId equals t_ccedx.Id into _cced
                             from t_cced in _cced ///.DefaultIfEmpty()

                                 //member stuff
                             join t_mcux in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id && x.IsCurrUnit == true && x.ChurchUnit.IsUnitGen == true)
                                            on t_cm.Id equals t_mcux.ChurchMemberId into _tmcu
                             from t_mcu in _tmcu.DefaultIfEmpty()
                             join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.IsUnitGen == true) on t_mcu.ChurchUnitId equals t_cbgx.Id into _tcbg
                             from t_cbg in _tcbg.DefaultIfEmpty()

                             select new ChurchAttendanceModel
                             {
                                 oChurchBody = t_cb,
                                 oChurchBodyId = t_cb.Id,
                                 strCongregation = t_cb.Name,
                                 oChurchAttendee = t_caa,
                                 oAttend_Id = (int?)t_caa.Id,

                                 ///
                                 strAttendeeName = t_caa.AttendeeType == "C" ? (t_cm != null ? AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false) : "") :
                                                                   AppUtilties.GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false),


                                 //strAttendeeName = t_caa != null ? (t_caa.AttendeeType == "C" ? (t_cm == null ? (GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false)) : 
                                 //                                  ("")) : (GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false))) : (""),

                                 ////(t_caa.AttendeeType == "C" ? (t_cm ==null ? (GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false)) : "") : "") :
                                 ////                       t_caa != null ? t_cm == null ? GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false) : "": "",
                                 ///
                                 oChurchMemberId = t_caa.AttendeeType == "C" ? (t_cm != null ? (int?)t_cm.Id : (int?)null) : (int?)null,
                                 strAttendeeTypeDesc = GetMemTypeDesc(t_caa.AttendeeType),

                                 strChurchEventDesc_Hdr = t_cced.ChurchCalendarEvent != null ? (t_cced.ChurchCalendarEvent.Subject + ":- " +
                                                                  (t_cced.ChurchCalendarEvent.IsFullDay == true ?
                                                                     (t_cced.ChurchCalendarEvent.EventFrom != null ? DateTime.Parse(t_cced.ChurchCalendarEvent.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                     (t_cced.ChurchCalendarEvent.EventFrom != null ? DateTime.Parse(t_cced.ChurchCalendarEvent.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                     (t_cced.ChurchCalendarEvent.EventFrom != null && t_cced.ChurchCalendarEvent.EventTo != null ? " -- " : "") +
                                                                     (t_cced.ChurchCalendarEvent.EventTo != null ? DateTime.Parse(t_cced.ChurchCalendarEvent.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                  )) : "",
                                 strChurchEventDesc = t_cced.EventDescription + ":- " +
                                                                  (t_cced.IsFullDay == true ?
                                                                     (t_cced.EventFrom != null ? DateTime.Parse(t_cced.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                     (t_cced.EventFrom != null ? DateTime.Parse(t_cced.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                     (t_cced.EventFrom != null && t_cced.EventTo != null ? " -- " : "") +
                                                                     (t_cced.EventTo != null ? DateTime.Parse(t_cced.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                  ),
                                 ///
                                 dtEventFrom = t_cced != null ? t_cced.EventFrom : (DateTime?)null,
                                 strGender = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.Gender : "") :
                                                               t_caa.Gender,
                                 strAgeGroup = t_caa.AttendeeType == "C" ? (t_cbg != null ? t_cbg.Name : "") :
                                                               t_caa.AgeBracket != null ? t_caa.AgeBracket.NVPValue : "",
                                 strNationality = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.Nationality != null ? t_cm.Nationality.EngName : "" : "") :
                                                               t_caa.Nationality != null ? t_caa.Nationality.EngName : "",
                                 strResidenceLoc = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.ContactInfo != null ? t_cm.ContactInfo.Location : "" : "") :
                                                               t_caa.ResidenceLoc,
                                 strPhone = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.ContactInfo != null ? t_cm.ContactInfo.MobilePhone1 : "" : "") :
                                                               t_caa.MobilePhone,

                                 strDateAttended = t_caa.DateAttended != null ? DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                                 decTempRec = t_caa.TempCelc != null ? (decimal)t_caa.TempCelc : (decimal?)null,
                                 strTempRec = t_caa.TempCelc != null ? t_caa.TempCelc.Value.ToString("{0:#.#}") : "",

                                 decPersWt = t_caa.PersKgWt != null ? (decimal)t_caa.PersKgWt : (decimal?)null,
                                 strPersWt = t_caa.PersKgWt != null ? t_caa.PersKgWt.Value.ToString("{0:#.#}") : "",

                                 decPersBPMin = t_caa.PersBPMin != null ? (decimal)t_caa.PersBPMin : (decimal?)null,
                                 strPersBPMin = t_caa.PersBPMin != null ? t_caa.PersBPMin.Value.ToString("{0:#.#}") : "",

                                 decPersBPMax = t_caa.PersBPMax != null ? (decimal)t_caa.PersBPMax : (decimal?)null,
                                 strPersBPMax = t_caa.PersBPMax != null ? t_caa.PersBPMax.Value.ToString("{0:#.#}") : ""
                             })
                                //.Distinct()
                                // .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenByDescending(x => x.dtEventFrom).ThenByDescending(x => x.dtEventTo).ThenBy(x => x.strAttendeeName)
                                .ToList();


            if (!string.IsNullOrEmpty(strFirstName))
            {
                oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strAttendeeName.ToLower().Contains(strFirstName.Trim().ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(strMiddleName))
            {
                oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strAttendeeName.ToLower().Contains(strMiddleName.Trim().ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(strLastName))
            {
                oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strAttendeeName.ToLower().Contains(strLastName.Trim().ToLower()))).ToList();
            }
            //if (!string.IsNullOrEmpty(strAttendLoc))
            //{
            //    oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strResidenceLoc.ToLower().Contains(strAttendLoc.Trim().ToLower()))).ToList();
            //} 
            if (!string.IsNullOrEmpty(strAttnPho))
            {
                oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strPhone.ToLower().StartsWith(strAttnPho.Trim().ToLower()))).ToList();
            }

            //if (!string.IsNullOrEmpty(strAttnPho))
            //{
            //    oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strPhone.ToLower().StartsWith(strAttnPho.Trim().ToLower()))).ToList();
            //}





            // lack of space... filter: take first 10
            //if (oChurchAttendVMList.Count > 10)
            //    oChurchAttendVMList = oChurchAttendVMList.Take(10).ToList();

            oChurchAttendVMList = oChurchAttendVMList
                              .OrderByDescending(x => x.oChurchAttendee.DateAttended).ThenByDescending(x => x.strAttendeeName)
                              .ToList();


            //if (!(string.IsNullOrEmpty(strFirstName) && string.IsNullOrEmpty(strAttnPho)))
            //{
            //    //oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strAttendeeName.ToLower().Contains(strAttnName.Trim().ToLower())))
            //    //               .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenByDescending(x => x.strAttendeeName)
            //    //               .ToList();

            //    oChurchAttendVMList = oChurchAttendVMList.Where(x => (string.IsNullOrEmpty(strFirstName) || x.strAttendeeName.ToLower().Contains(strFirstName.Trim().ToLower())) &&
            //                                                      (string.IsNullOrEmpty(strAttnPho) || x.strPhone.ToLower().Contains(strAttnPho.Trim().ToLower())))
            //                  .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenByDescending(x => x.strAttendeeName)
            //                  .ToList();
            //}
            //else
            //{
            //    oChurchAttendVMList = oChurchAttendVMList
            //                   .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenByDescending(x => x.strAttendeeName)
            //                   .ToList();
            //}

            //
            return oChurchAttendVMList;


        }

        public JsonResult GetAttendanceListByAttendeeFilters(int? oChurchBodyId, string strAttendee, string strLongevity, string strFirstName, string strMiddleName, string strLastName, string strAttendLoc, string strAttendPho)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            var oChurchBody = _context.ChurchBody.Where(c=> c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oChurchBodyId).FirstOrDefault();
            var oAttendList = ReturnChurchAttendeesListByFilters(oChurchBody, strAttendee, strLongevity, strFirstName, strMiddleName, strLastName, strAttendLoc, strAttendPho);

            // if (addEmpty) countryList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(oAttendList);
        }



        public ChurchAttendanceModel ReturnAttendVMTotals(ChurchAttendanceModel currVm, List<ChurchAttendanceModel> resList)
        {
            var oItem = (from ssi in resList
                             // here I choose each field I want to group by
                         group ssi by new { ssi.oChurchAttend_HC.CountDate, ssi.oChurchAttend_HC.ChurchEventId, ssi.oChurchAttend_HC.CountType } into g
                         // group ssi by new { ssi.strDateAttended, ssi.strChurchEventDesc, ssi.oChurchAttend_HC.CountType, ssi.CountCategoryId } into g
                         select new //ChurchAttendanceModel
                         {
                             oAttn = g.Select(x => x.oChurchAttendee).FirstOrDefault(),
                             //categId = g.Key.CountCategoryId,
                             //evtDesc = g.Key.strChurchEventDesc,
                             //attnDate = g.Key.strDateAttended,
                             tot_M = g.Sum(x => x.CHCF_TotAttend_M),
                             tot_F = g.Sum(x => x.CHCF_TotAttend_F),
                             tot_O = g.Sum(x => x.CHCF_TotAttend_O),
                             tot_Cnt = g.Sum(x => x.CHCF_TotAttend_M) + g.Sum(x => x.CHCF_TotAttend_F) + g.Sum(x => x.CHCF_TotAttend_O),
                             // Rec_Count = g.Count(),
                             //  oCBId = g.Min(x => x.oChurchBodyId),
                             // oAttnId = g.Min(x => x.oAttend_Id),
                             //  strCountCategory = g.Min(x => x.strCountCategory),
                             // MinAge = g.Min(x => x.MinAge)
                         }
                    )
                    .FirstOrDefault();

            // .OrderBy(x => x.MinAge).ThenBy(x => x.strCountTypeDesc)
            //   .ToList();

            if (oItem != null)
            {
                currVm.oChurchAttendee = oItem.oAttn;
                //currVm.CountCategoryId = oItem.categId;
                //currVm.strChurchEventDesc = oItem.evtDesc;
                //currVm.strDateAttended = oItem.attnDate;

                currVm.CHCF_TotAttend_M = oItem.tot_M;
                currVm.CHCF_TotAttend_F = oItem.tot_F;
                currVm.CHCF_TotAttend_O = oItem.tot_O;
                currVm.CHCF_TotAttend_MemOrVis = oItem.tot_Cnt;
            }


            return currVm;
        }



        public List<ChurchAttendanceModel> ReturnAttendVMTotals_Summ(List<ChurchAttendanceModel> resList)  //ChurchAttendanceModel currVm,
        {
            var oList = (from ssi in resList
                             // here I choose each field I want to group by
                         group ssi by new { ssi.oChurchAttend_HC.CountDate?.Date, ssi.oChurchAttend_HC.ChurchEventId, ssi.oChurchAttend_HC.CountType } into g
                         // group ssi by new { ssi.strDateAttended, ssi.strChurchEventDesc, ssi.oChurchAttend_HC.CountType, ssi.CountCategoryId } into g
                         select new ChurchAttendanceModel
                         {
                             // oAttn = g.Select(x => x.oChurchAttend).FirstOrDefault(),  /// use list instead

                             //categId = g.Key.CountCategoryId,
                             // evtDesc = g.Key.strChurchEventDesc,
                             //attnDate = g.Key.strDateAttended,
                             // tot_M = g.Sum(x => x.CHCF_TotAttend_M),
                             // tot_F = g.Sum(x => x.CHCF_TotAttend_F),
                             // tot_O = g.Sum(x => x.CHCF_TotAttend_O),
                             //
                             // strCntBatNo = g.Min(x=>x.strCountBatchNo),
                             // strEvtDesc = g.Min(x=>x.strChurchEventDesc),
                             // strEvtDate = g.Min(x=>x.strDateAttended),

                             // tot_Cnt = g.Sum(x => x.CHCF_TotAttend_M) + g.Sum(x => x.CHCF_TotAttend_F) + g.Sum(x => x.CHCF_TotAttend_O),

                             strCountBatchNo = g.Min(x => x.strCountBatchNo),
                             strChurchEventDesc = g.Min(x => x.strChurchEventDesc),
                             strDateAttended = g.Min(x => x.strDateAttended),
                             CHCF_TotAttend_MemOrVis = g.Sum(x => x.CHCF_TotAttend_M) + g.Sum(x => x.CHCF_TotAttend_F) + g.Sum(x => x.CHCF_TotAttend_O),
                         }
                    )

                   .ToList();

            // .OrderBy(x => x.MinAge).ThenBy(x => x.strCountTypeDesc)
            //   .ToList();

            //if (oItem != null)
            //{
            //    //currVm.oChurchAttend = oItem.oAttn;   //list of attend_counts  ... or use strCountBatchNo to fetch them
            //    //
            //    //currVm.CountCategoryId = oItem.categId;
            //    //currVm.strChurchEventDesc = oItem.evtDesc;
            //    //currVm.strDateAttended = oItem.attnDate;
            //    //
            //    //currVm.CHCF_TotAttend_M = oItem.tot_M ;
            //    //currVm.CHCF_TotAttend_F = oItem.tot_F;
            //    //currVm.CHCF_TotAttend_O = oItem.tot_O;
            //    //
            //    currVm.strCountBatchNo = oItem.strCntBatNo;
            //    currVm.strChurchEventDesc = oItem.strEvtDesc;
            //    currVm.strDateAttended = oItem.strEvtDate;
            //    currVm.CHCF_TotAttend_MemOrVis = oItem.tot_Cnt;
            //}         

            return oList;
        }

        public List<ChurchAttendanceModel> ReturnCongHeadcountList_Any(ChurchBody oChurchBody, int countBackDays) // string strLongevity, int? churchEventId, DateTime? attendDate)
        {   //Get member profile list      ... current members ... available
            // var dtv = attendDate != null ? ((DateTime)attendDate).Date : (DateTime?)null;

            var results = (
                             from t_caa in _context.ChurchAttendHeadCount.AsNoTracking().Include(t => t.ChurchBody)  //.Include(t => t.ChurchGroupChurchBody) //.Include(t => t.Nationality)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.CountDate >= DateTime.Now.Date.AddDays(-1 * countBackDays) &&
                                                   // (strLongevity == "H" || (strLongevity == "C" && x.CountDate.ToString() == DateTime.Now.Date.ToShortDateString())) &&
                                                   // x.ChurchEventId == churchEventId && ((DateTime)x.CountDate.Value).Date == dtv &&
                                                   // x.CountDate.Value.ToShortDateString() == attendDate.Value.ToShortDateString() &&
                                                   (x.ChurchBody.OrgType == "GB" || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CN") &&  // (x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                   (x.ChurchBodyId == oChurchBody.Id || x.ChurchBody.ParentChurchBodyId == oChurchBody.Id || x.ChurchBody.RootChurchCode.Contains(oChurchBody.RootChurchCode)))

                             join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                             from t_cb in _tcb
                             join t_ccex in _context.ChurchCalendarEvent.AsNoTracking() on t_caa.ChurchEventId equals t_ccex.Id into _cce
                             from t_cce in _cce.DefaultIfEmpty()
                             join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.OrgType == "CG") on t_caa.ChurchUnitId equals t_cbgx.Id into _tcbg
                             from t_cbg in _tcbg.DefaultIfEmpty()
                             select new ChurchAttendanceModel
                             {
                                 // oChurchBody = t_cb, 
                                 oChurchBodyId = (int)t_cb.Id,
                                 strCongregation = t_cb.Name,
                                 oChurchAttend_HC = t_caa,
                                 oAttend_Id = (int)t_caa.Id,
                                 strCountTypeDesc = t_caa.CountType == "C" ? "Congregation" : "Church Group",
                                 strCountTypeCode = t_caa.CountType,
                                 strChurchEventDesc = t_cce != null ?
                                                        t_cce.Subject + ":- " +
                                                                            (t_cce.IsFullDay == true ?
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                                (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                                                                (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                             ) : "",
                                 CHCF_TotAttend_M = t_caa.TotM,
                                 CHCF_TotAttend_F = t_caa.TotF,
                                 CHCF_TotAttend_O = t_caa.TotO,
                                 CHCF_TotAttend_MemOrVis = t_caa.TotCount,

                                 strCountCategory = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? t_cbg.Name : "") : "Congregants",
                                 //  (!string.IsNullOrEmpty(t_cb.TagName) ? t_cb.TagName : t_cb.Name),
                                 CountCategoryId = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? (int?)t_cbg.Id : (int?)null) : (int?)t_cb.Id,
                                 MinAge = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? (int?)t_cbg.UnitMinAge : (int?)null) : (int?)null,
                                 strDateAttended = t_caa.CountDate != null ?
                                                            DateTime.Parse(t_caa.CountDate.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : ""
                             })
                                //.Distinct()
                                // .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenBy(x => x.strAttendeeName)
                                .ToList();

            return results;
        }

        public List<ChurchAttendanceModel> ReturnCongHeadcountList(int? oAppGloOwnId, int? oCBid, string strLongevity, int? churchEventId, DateTime? attendDate)
        {   //Get member profile list      ... current members ... available
            var dtv = attendDate != null ? ((DateTime)attendDate).Date : (DateTime?)null;

            var results = (
                             from t_caa in _context.ChurchAttendHeadCount.AsNoTracking().Include(t => t.ChurchBody)  //.Include(t => t.ChurchGroupChurchBody) //.Include(t => t.Nationality)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == oAppGloOwnId &&
                                                   // (strLongevity == "H" || (strLongevity == "C" && x.CountDate.ToString() == DateTime.Now.Date.ToShortDateString())) &&
                                                    x.ChurchEventId == churchEventId && ((DateTime)x.CountDate.Value).Date == dtv &&
                                                   // x.CountDate.Value.ToShortDateString() == attendDate.Value.ToShortDateString() &&
                                                   (x.ChurchBody.OrgType == "CN" || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CR") && // (x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                   (x.ChurchBodyId == oCBid))  /// || x.ChurchBody.ParentChurchBodyId == oCBid || x.ChurchBody.RootChurchCode.Contains(oChurchBody.RootChurchCode)))

                             join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                             from t_cb in _tcb
                             join t_ccex in _context.ChurchCalendarEvent.AsNoTracking() on t_caa.ChurchEventId equals t_ccex.Id into _cce
                             from t_cce in _cce.DefaultIfEmpty()

                                 //member stuff
                                 //join t_mcux in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id && x.IsCurrUnit == true && x.ChurchUnit.IsUnitGenerational == true)
                                 //               on t_cm.Id equals t_mcux.ChurchMemberId into _tmcu
                                 //from t_mcu in _tmcu.DefaultIfEmpty()

                             join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAppGloOwnId && x.OrgType == "CG") on t_caa.ChurchUnitId equals t_cbgx.Id into _tcbg
                             from t_cbg in _tcbg.DefaultIfEmpty()

                                 //from t_caa in _context.ChurchAttend_HeadCount.AsNoTracking().Include(t => t.ChurchBody)
                                 //                                              .Where(x => x.ChurchBody.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.AttendeeType == strAttendee &&
                                 //                                            //  (strLongevity == "H" || (strLongevity == "C" && x.DateAttended == DateTime.Now.Date)) &&
                                 //                                              (x.ChurchBody.OrgType == "GB" || x.ChurchBody.OrgType == "CN") && (x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                 //                                              (x.ChurchBodyId == oChurchBody.Id || x.ChurchBody.ParentChurchBodyId == oChurchBody.Id || x.ChurchBody.ChurchCodeFullPath.Contains(oChurchBody.ChurchCodeFullPath)))

                                 //       join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                                 //       from t_cb in _tcb
                                 //       join t_cmx in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo) on t_caa.ChurchMemberId equals t_cmx.Id into _tcm
                                 //       from t_cm in _tcm.DefaultIfEmpty()
                                 //       join t_ccex in _context.ChurchCalendarEvent.AsNoTracking() on t_caa.ChurchEventId equals t_ccex.Id into _cce
                                 //       from t_cce in _cce.DefaultIfEmpty()

                                 //           //member stuff
                                 //       join t_mcux in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == oChurchBody.Id && x.IsCurrUnit == true && x.ChurchUnit.IsUnitGenerational == true)
                                 //                      on t_cm.Id equals t_mcux.ChurchMemberId into _tmcu
                                 //       from t_mcu in _tmcu.DefaultIfEmpty()
                                 //       join t_cbgx in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.IsUnitGenerational == true) on t_mcu.ChurchUnitId equals t_cbgx.Id into _tcbg
                                 //       from t_cbg in _tcbg.DefaultIfEmpty()


                             select new ChurchAttendanceModel
                             {
                                 // oChurchBody = t_cb, 
                                 oChurchBodyId = (int)t_cb.Id,
                                 strCongregation = t_cb.Name,
                                 oChurchAttend_HC = t_caa,
                                 oAttend_Id = (int)t_caa.Id,
                                 strCountTypeDesc = t_caa.CountType == "C" ? "Congregation" : "Church Group",
                                 strCountTypeCode = t_caa.CountType,
                                 strChurchEventDesc = t_cce != null ?
                                                        t_cce.Subject + ":- " +
                                                                            (t_cce.IsFullDay == true ?
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                                (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                                                                (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                             ) : "",
                                 CHCF_TotAttend_M = t_caa.TotM,
                                 CHCF_TotAttend_F = t_caa.TotF,
                                 CHCF_TotAttend_O = t_caa.TotO,
                                 CHCF_TotAttend_MemOrVis = t_caa.TotCount,

                                 strCountCategory = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? t_cbg.Name : "") : "Congregants",
                                 //  (!string.IsNullOrEmpty(t_cb.TagName) ? t_cb.TagName : t_cb.Name),
                                 CountCategoryId = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? (int?)t_cbg.Id : (int?)null) : (int?)t_cb.Id,
                                 MinAge = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? (int?)t_cbg.UnitMinAge : (int?)null) : (int?)null,
                                 strDateAttended = t_caa.CountDate != null ?
                                                            DateTime.Parse(t_caa.CountDate.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : ""
                             })
                                //.Distinct()
                                // .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenBy(x => x.strAttendeeName)
                                .ToList();

            //var oChurchHeadcountVMList = (from ssi in results
            //                  // here I choose each field I want to group by
            //              group ssi by new { ssi.strDateAttended, ssi.strChurchEventDesc, ssi.oChurchAttend_HC.CountType, ssi.CountCategoryId } into g
            //              select new ChurchAttendanceModel
            //              {
            //                  oChurchAttend = g.Select(x=>x.oChurchAttend).FirstOrDefault(),
            //                  CountCategoryId = g.Key.CountCategoryId,
            //                  strChurchEventDesc = g.Key.strChurchEventDesc,
            //                  strDateAttended = g.Key.strDateAttended,
            //                  CHCF_TotAttend_M = g.Sum(x => x.oChurchAttend_HC.Tot_M),
            //                  CHCF_TotAttend_F = g.Sum(x => x.oChurchAttend_HC.Tot_F),
            //                  CHCF_TotAttend_O = g.Sum(x => x.oChurchAttend_HC.Tot_O),
            //                  CHCF_TotAttend_MemOrVis = g.Sum(x => x.oChurchAttend_HC.Tot_M) + g.Sum(x => x.oChurchAttend_HC.Tot_F) + g.Sum(x => x.oChurchAttend_HC.Tot_O),
            //                  // Rec_Count = g.Count(),
            //                  oChurchBodyId = g.Min(x => x.oChurchBodyId),
            //                  oAttend_Id = g.Min(x => x.oAttend_Id),
            //                  strCountCategory = g.Min(x => x.strCountCategory),
            //                  MinAge = g.Min(x => x.MinAge)
            //              }
            //)
            //.OrderBy (x=>x.MinAge).ThenBy(x=>x.strCountTypeDesc)
            //.ToList();

            // return oChurchHeadcountVMList;


            return results;


            //var oChurchHeadcountVMList = (from ssi in results
            //                                  // here I choose each field I want to group by
            //                              group ssi by new { ssi.strDateAttended, ssi.strChurchEventDesc, ssi.oChurchAttend_HC.CountType, ssi.CountCategoryId } into g
            //                              select new
            //                              {
            //                                  CountCategory = g.Key.CountCategoryId,
            //                                  ChurchEvent = g.Key.strChurchEventDesc,
            //                                  DateAttended = g.Key.strDateAttended,
            //                                  M_Tot = g.Sum(x => x.oChurchAttend_HC.Tot_M),
            //                                  F_Tot = g.Sum(x => x.oChurchAttend_HC.Tot_F),
            //                                  O_Tot = g.Sum(x => x.oChurchAttend_HC.Tot_M) + g.Sum(x => x.oChurchAttend_HC.Tot_F),
            //                                  Rec_Count = g.Count(),
            //                                  oChurchBodyId = g.Min(x => x.oChurchBodyId),
            //                                  oAttend_Id = g.Min(x => x.oAttend_Id),
            //                                  CountCategoryDesc = g.Min(x => x.strCountCategory)
            //                              }
            //                ).ToList();
            //return oChurchHeadcountVMList;
        }



        public List<ChurchAttendanceModel> ReturnCongHeadcountList_EditByCategory(ChurchBody oChurchBody, string strCountType)  // later... customize ... create a class == merge Member + Attendance
        {   //Get member profile list      ... current members ... available

            //var oChurchAttendVMList = (
            //                 from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality).Include(t => t.AgeBracket)
            //                                    .Where(x => x.ChurchBodyId == oChurchBody.Id && x.AttendeeType == "C" && x.Id == id)

            var oChurchHeadcountVMList = (
                                from t_cbg in _context.ChurchUnit   //.Include(t => t.OwnedByChurchBody)
                                        .Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && !string.IsNullOrEmpty(x.Name) &&
                                                 ((strCountType == "C" && x.OrgType == "CN") ||
                                                 (strCountType == "G" && x.OrgType == "CG" && //x.IsUnitGenerational == true &&
                                         (x.OwnedByChurchBodyId == oChurchBody.Id ||
                                         (x.OwnedByChurchBodyId != oChurchBody.Id && x.SharingStatus == "C" && x.OwnedByChurchBodyId == oChurchBody.ParentChurchBodyId) ||
                                         (x.OwnedByChurchBodyId != oChurchBody.Id && x.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(x.OwnedByChurchBody, oChurchBody))))))
                                join t_caax in _context.ChurchAttendHeadCount.AsNoTracking()
                                                    .Where(x => x.ChurchBodyId == oChurchBody.Id && x.CountType == strCountType) on t_cbg.Id equals t_caax.ChurchUnitId into _tcaa
                                from t_caa in _tcaa.DefaultIfEmpty()
                                    //join t_caax_cb in _context.ChurchAttend_HeadCount.AsNoTracking()
                                    //                    .Where(x => x.ChurchBodyId == oChurchBody.Id && x.CountType == strCountType) on t_cbg.Id equals t_caax.ChurchGroupChurchBodyId into _tcaa
                                    //from t_caa in _tcaa.DefaultIfEmpty() 
                                join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                                from t_cb in _tcb
                                join t_ccex in _context.ChurchCalendarEvent.AsNoTracking() on t_caa.ChurchEventId equals t_ccex.Id into _cce
                                from t_cce in _cce.DefaultIfEmpty()

                                select new ChurchAttendanceModel
                                {
                                    // oChurchBody = t_cb, 
                                    oChurchBodyId = (int)t_cb.Id,
                                    strCongregation = t_cb.Name,
                                    //  oChurchAttend_HC = t_caa, 
                                    oAttend_Id = (int)t_caa.Id,
                                    strCountTypeDesc = t_caa.CountType == "C" ? "Congregation" : "Church Group",
                                    strChurchEventDesc = t_cce != null ?
                                                           t_cce.Subject + ":- " +
                                                                               (t_cce.IsFullDay == true ?
                                                                                   (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                                   (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                                   (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                                                                   (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                                ) : "",
                                    strCountCategory = t_caa.CountType == "G" ? (t_cbg != null ? t_cbg.Name : "") : t_cb != null ? t_cb.Name : "",
                                    CountCategoryId = t_caa.CountType == "G" ?
                                                               t_cbg != null ? (int?)t_cbg.Id : (int?)null :
                                                                t_cb != null ? (int?)t_cb.Id : (int?)null,
                                    strDateAttended = t_caa.CountDate != null ? DateTime.Parse(t_caa.CountDate.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                                    MinAge = t_caa.CountType == "G" ?
                                                               t_cbg != null ? (int?)t_cbg.UnitMinAge : (int?)null : (int?)null
                                })
                                .OrderBy(x => x.MinAge).ThenBy(x => x.strCountTypeDesc)
                            .ToList();


            return oChurchHeadcountVMList;
        }


        public List<ChurchAttendanceModel> ReturnCongHeadcountList_EditById(ChurchBody oChurchBody, int id)  // later... customize ... create a class == merge Member + Attendance
        {   //Get member profile list      ... current members ... available

            //var oChurchAttendVMList = (
            //                 from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality).Include(t => t.AgeBracket)
            //                                    .Where(x => x.ChurchBodyId == oChurchBody.Id && x.AttendeeType == "C" && x.Id == id)

            var oChurchHeadcountVMList = (
                             from t_caa in _context.ChurchAttendHeadCount.AsNoTracking()//.Include(t => t.Nationality)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.Id == id)
                             join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                             from t_cb in _tcb
                             join t_ccex in _context.ChurchCalendarEvent.AsNoTracking() on t_caa.ChurchEventId equals t_ccex.Id into _cce
                             from t_cce in _cce.DefaultIfEmpty()
                             join t_cbgx in _context.ChurchUnit.AsNoTracking().Where(x => x.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && x.OrgType == "CG") on t_caa.ChurchUnitId equals t_cbgx.Id into _tcbg
                             from t_cbg in _tcbg.DefaultIfEmpty()

                             select new ChurchAttendanceModel
                             {
                                 // oChurchBody = t_cb, 
                                 oChurchBodyId = (int)t_cb.Id,
                                 strCongregation = t_cb.Name,
                                 //  oChurchAttend_HC = t_caa, 
                                 oAttend_Id = (int)t_caa.Id,
                                 strCountTypeDesc = t_caa.CountType == "C" ? "Congregation" : "Church Group",
                                 strChurchEventDesc = t_cce != null ?
                                                        t_cce.Subject + ":- " +
                                                                            (t_cce.IsFullDay == true ?
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                                (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                                                                (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                             ) : "",
                                 strCountCategory = t_caa.CountType == "G" ?
                                                            (t_cbg != null ? t_cbg.Name : ""): t_cb != null ? t_cb.Name : "",
                                 CountCategoryId = t_caa.CountType == "G" ?
                                                            t_cbg != null ? (int?)t_cbg.Id : (int?)null :
                                                             t_cb != null ? (int?)t_cb.Id : (int?)null,
                                 strDateAttended = t_caa.CountDate != null ? DateTime.Parse(t_caa.CountDate.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "",
                                 MinAge = t_caa.CountType == "G" ?
                                                            t_cbg != null ? (int?)t_cbg.UnitMinAge : (int?)null : (int?)null
                             })
                                .OrderBy(x => x.MinAge).ThenBy(x => x.strCountTypeDesc)
                            .ToList();


            return oChurchHeadcountVMList;
        }


        //  public JsonResult GetAttendanceListByAttendeeFilters_AC(int? oChurchBodyId, string strAttendee, string strLongevity, string strAttendName, string strAttendPho)
        public JsonResult GetAttendListByFilters_AC(string strAttendName, int oCBid)   /// = 1029)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            //change or clear....this line
            var oChurchBody = _context.ChurchBody.Find(oCBid);
            //var oAttendList = ReturnChurchAttendeesListByFilters(oChurchBody, strAttendee, strLongevity, strAttendName, strAttendPho);
            var oAttendList = (from y in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality)
                                                   .Where(x => x.ChurchBody.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId &&
                                                   (x.FirstName + x.MiddleName + x.LastName).ToLower().Contains(strAttendName.Trim().ToLower()) &&
                                                   // x.AttendeeType == "V" && (x.DateAttended == DateTime.Now.Date)) &&
                                                   (x.ChurchBody.OrgType == "GB" || x.ChurchBody.OrgType == "CH" || x.ChurchBody.OrgType == "CN") && // (x.ChurchBody.ChurchType == "CH" || x.ChurchBody.ChurchType == "CF") &&
                                                   (x.ChurchBodyId == oChurchBody.Id || x.ChurchBody.ParentChurchBodyId == oChurchBody.Id || x.ChurchBody.RootChurchCode.Contains(oChurchBody.RootChurchCode))
                                                   )
                               select new { label = y.DateAttended.ToString() })
                                                   .ToList();

            // if (!(string.IsNullOrEmpty(strAttendName))) // && string.IsNullOrEmpty(strAttendPho)))
            //{
            //    //oChurchAttendVMList = oChurchAttendVMList.Where(x => (x.strAttendeeName.ToLower().Contains(strAttnName.Trim().ToLower())))
            //    //               .OrderByDescending(x => x.oChurchAttend.DateAttended).ThenByDescending(x => x.strAttendeeName)
            //    //               .ToList();

            //    oAttendList = oAttendList.Where(x => (string.IsNullOrEmpty(strAttendName) || (x.FirstName + x.MiddleName + x.LastName).ToLower().Contains(strAttendName.Trim().ToLower())) // &&
            //                                                    //  (string.IsNullOrEmpty(strAttendPho) || x.MobilePhone.ToLower().Contains(strAttendPho.Trim().ToLower()))
            //                                                      )
            //                  .OrderByDescending(x => x.DateAttended).ThenByDescending(x => (x.FirstName + x.MiddleName + x.LastName))
            //                  .ToList();
            //}
            //else
            //{
            //    oAttendList = oAttendList
            //                   .OrderByDescending(x => x.DateAttended).ThenByDescending(x => (x.FirstName + x.MiddleName + x.LastName))
            //                   .ToList();
            //}

            // if (addEmpty) countryList.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            return Json(oAttendList);
        }


        //public JsonResult GetChurchEventsByAttendDate(int oCurrChuBodyId, string strDate, bool addEmpty = false, ChurchModelContext currCtx = null)
        //{

        //    if (currCtx == null)
        //    {
        //        currCtx = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
        //        if (currCtx == null)
        //        {
        //            RedirectToAction("LoginUserAcc", "UserLogin");

        //            // should not get here... Response.StatusCode = 500; 
        //            return null; //// View("_ErrorPage");
        //        }
        //    }

        //    var eventList = new List<SelectListItem>();
        //    DateTime dtAttend;

        //    try
        //    {
        //        dtAttend = DateTime.Parse(strDate);
        //    }
        //    catch (Exception)
        //    {
        //        if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
        //        return Json(eventList);
        //    }

        //    var oCurrChuBody = currCtx.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();

        //    //  var dt = dtAttend.Date.ToShortDateString();
        //    var dtv = dtAttend != null ? ((DateTime)dtAttend).Date : (DateTime?)null;
        //    // var dts = dtAttend != null ? dtAttend.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;   


        //    var eventList_1 = currCtx.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP)
        //                     .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && 
        //                           ((DateTime)c.EventFrom.Value).Date == dtv).ToList();

        //    eventList_1 = eventList_1.Where(c =>
        //                       (c.ChurchBodyId == oCurrChuBody.Id ||
        //                       (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
        //                       (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))).ToList();


        //    //eventList = currCtx.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP)
        //    //                 .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&
        //    //                       // c.ChurchBody.CountryId == oCurrChuBody.CountryId && c.EventFrom.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt  &&
        //    //                       ((DateTime)c.EventFrom.Value).Date == dtv &&
        //    //                        (c.ChurchBodyId == oCurrChuBody.Id ||
        //    //                        (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
        //    //                        (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
        //    //                    )


        //    eventList = eventList_1
        //                         .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
        //                         .ToList()
        //                                 .Select(c => new SelectListItem()
        //                                 {
        //                                     Value = c.Id.ToString(),
        //                                     Text = c.Subject + ":- " +
        //                                                         (c.IsFullDay == true ?
        //                                                             (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
        //                                                             (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
        //                                                             (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
        //                                                             (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
        //                                                          )
        //                                 })
        //                                 .OrderBy(c => c.Text)
        //                                 .ToList();

        //    if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
        //    return Json(eventList);
        //}



        [HttpGet]
        public IActionResult Index_Attendance(int currAttnTask = 0, int? oCBid = null, string strAttnType = "C", /// string strLongevity = "C", /// int? id = null, 
                                                int? oEvCLid = null, int? eventId = null, DateTime? eventDateMin = null, DateTime? eventDateMax = null, bool loadLim = false)      //, char? filterIndex = null, int? filterVal = null)
        {
            try
            { 
                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return null; //// View("_ErrorPage");
                    }
                }

                //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                //{ RedirectToAction("LoginUserAcc", "UserLogin"); }

                if (!loadLim)
                    _ = this.LoadClientDashboardValues();  /// this._clientDBConnString);

                var oAppGloOwnId = this._oLoggedAGO.Id;
                /// var oCBid = oCBid;

                //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                //if (oCBid == null) oCBid = this._oLoggedCB.Id;

                oCBid = oCBid == null ? this._oLoggedCB.Id : oCBid;
                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();

                //var UserLogIn_Priv = TempData.Get<List<UserSessionPrivilege>>("UserLogIn_oUserPrivCol");
                //var checkUser = UserLogIn_Priv?.Count > 0;

                //var oCurrChuBody = oCB; // oUserLogIn_Priv[0].ChurchBody;
                //var oRequestedChurchBody = oCB;

                eventDateMin = eventDateMin != null ? eventDateMin.Value.Date : DateTime.Now.Date;
                eventDateMax= eventDateMax != null ? eventDateMax.Value.Date : DateTime.Now.Date;


                ///// var oEventCBid = oCurrChuBody.Id;
                ///// confirm the CL 
                //if (oEvCLid != null)
                //{
                //    /// diff CL events requested --- perhaps the parent CB events
                //    if (oCurrChuBody.ChurchLevelId != oEvCLid)
                //    {
                //        var oCBList_1 = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();
                //        oCurrChuBody = oCBList_1.Where(c => oCurrChuBody.RootChurchCode.Contains(c.RootChurchCode) && c.ChurchLevelId == oEvCLid).FirstOrDefault();

                //        if (oCurrChuBody != null) oCBid = oCurrChuBody.Id;
                //    }
                //}


                var oEvCBid = oCBid;
                /// confirm the CL 
                if (oEvCLid != null)
                {
                    /// diff CL events requested --- perhaps the parent CB events
                    if (oCB.ChurchLevelId != oEvCLid)
                    {
                        var oCBList_1 = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();
                        var oEvCB = oCBList_1.Where(c => oCB.RootChurchCode.Contains(c.RootChurchCode) && c.ChurchLevelId == oEvCLid).FirstOrDefault();

                        if (oEvCB != null)
                        {
                            oEvCBid = oEvCB.Id;
                            oEvCLid = oEvCB.ChurchLevelId;
                        }
                    }
                }



                var oAttendModel = new ChurchAttendanceModel();
                /// oAttendModel.strAttnLongevity = strLongevity;  // today (C), history (H)
                oAttendModel.f_strAttendeeTypeCode = strAttnType;  //else history
                oAttendModel.f_ChurchEventDetailId = eventId;  //else history
                oAttendModel.f_DateAttendedMin = eventDateMin; //!= null ? eventDateMin.Value.Date : DateTime.Now.Date;  //else history
                oAttendModel.f_DateAttendedMax = eventDateMax; // != null ? eventDateMax.Value.Date : DateTime.Now.Date;  //else history
                oAttendModel.strCurrTaskDesc = "Church Attendance";
                oAttendModel.f_oEventCLId = oEvCLid;
                oAttendModel.f_oEventCBId = oEvCBid;

                //if (currAttendTask == 1)
                //{
                //    oAttendModel.strCurrTaskDesc = oAttendModel.f_strAttendeeTypeCode == "C" ? oAttendModel.strAttnLongevity == "H" ? "Member Attendance Log -- History" : "Member Attendance (Past Week)" :
                //                         oAttendModel.f_strAttendeeTypeCode == "V" ? oAttendModel.strAttnLongevity == "H" ? "Visitors History (Attendance)" : "Visitor Attendance (Past Week)" :
                //                                                      oAttendModel.strAttnLongevity == "H" ? "Church Attendance Log -- History" : "Church Attendance (Past Week)";  // isMigrated == true ? "New Converts" : "New Coverts (Historic)" : isMigrated == true ? "Visitors" : "Visitors (Historic)";
                //} 
                //else
                //{
                //    oAttendModel.strCurrTaskDesc = "Church Attendance";
                //} 
                ////   oAttendModel.isCurrMigrateQry = isMigrated; 
                /// _oCPRDefault != null ? String.Format("{0:dddd, MMMM d, yyyy}", _oCPRDefault.FromDate.Value) : ""

                /// String.Format("{0:dddd, MMMM d, yyyy}", _oCPRDefault.FromDate.Value)

                var resList = _context.ChurchAttendAttendee.AsNoTracking().Include(t=> t.ChurchEventDetail)
                    .Where(x => x.AppGlobalOwnerId == oAppGloOwnId && x.ChurchBodyId == oCBid).ToList();

                if (resList.Count > 0)
                {
                    oAttendModel.lsChurchAttendanceModels_Hdr = (from ssi in resList
                                                                 group ssi by new { ssi.DateAttended.Value.Date, ssi.ChurchEventDetailId } into g

                                                                 select new ChurchAttendanceModel
                                                                 {
                                                                     oChurchAttendee = g.FirstOrDefault(),

                                                                     strCurrTaskDesc = g.Select(x => x.ChurchEventDetail).FirstOrDefault().EventDescription + ": " + String.Format("{0:ddd, MMM d, yyyy}", g.Key.Date),
                                                                     
                                                                     //(g.Select(x => x.ChurchEvent).FirstOrDefault().IsFullDay == true ?
                                                                     //    (g.Select(x => x.ChurchEvent).FirstOrDefault().EventFrom != null ? String.Format("{0:ddd, MMM d, yyyy}", g.Select(x => x.ChurchEvent).FirstOrDefault().EventFrom.Value) : "").Trim() :
                                                                     //    (g.Select(x => x.ChurchEvent).FirstOrDefault().EventFrom != null ? String.Format("{0:ddd, MMM d, yyyy}", g.Select(x => x.ChurchEvent).FirstOrDefault().EventFrom.Value) : "").Trim() +
                                                                     //    (g.Select(x => x.ChurchEvent).FirstOrDefault().EventFrom != null && g.Select(x => x.ChurchEvent).FirstOrDefault().EventTo != null ? " -- " : "") +
                                                                     //    (g.Select(x => x.ChurchEvent).FirstOrDefault().EventTo != null ? String.Format("{0:ddd, MMM d, yyyy}", g.Select(x => x.ChurchEvent).FirstOrDefault().EventTo.Value) : "").Trim()
                                                                     // ),

                                                                     f_ChurchEventDetailId = g.Key.ChurchEventDetailId,
                                                                     f_DateAttended = g.Key.Date
                                                                 }
                                         ).ToList();

                    if (oAttendModel.lsChurchAttendanceModels_Hdr.Count > 0)
                        oAttendModel.lsChurchAttendanceModels_Hdr
                            .OrderByDescending(c => c.oChurchAttendee.DateAttended)
                            .OrderBy(c => c.strCurrTaskDesc)
                            .ToList();
                }
                else
                    oAttendModel.lsChurchAttendanceModels_Hdr = new List<ChurchAttendanceModel>();


                /// in-person list 
                if (currAttnTask == 0 || currAttnTask == 1)  /// 1 - in-person
                    oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oAppGloOwnId, oCBid, strAttnType, eventId, eventDateMin, eventDateMax);   /// strLongevity

                ///// hc list
                //if (currAttnTask == 0 || currAttnTask == 2)  /// 1 - hc
                ///  oAttendModel.lsChurchAttendanceModels_HC = ReturnChurchAttendeesList(oAppGloOwnId, oCBid, strAttendee, strLongevity, eventId, eventDateMin, eventDateMax);

                ///// trends list
                //if (currAttnTask == 0 || currAttnTask == 3)  /// 1 - trends
                ///   oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oAppGloOwnId, oCBid, strAttendee, strLongevity, eventId, eventDateMin, eventDateMax);






                /// load lookups
                /// 

                oAttendModel.lkpAttendeeTypes = new List<SelectListItem>();
                dlMemTypeCodes.OrderBy(x => x.Val).ToList();

                foreach (var dl in dlMemTypeCodes)  
                    oAttendModel.lkpAttendeeTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });   ///, Disabled = dl.Val != "V" 

               ///oAttendModel.lkpAttendeeTypes.OrderBy(c=> c.Value).ToList();                 

                oAttendModel.lkpChurchLevels = _context.ChurchLevel.Where(c => c.AppGlobalOwnerId == oAppGloOwnId)
                              .OrderByDescending(c => c.LevelIndex)
                              .Select(c => new SelectListItem()
                              {
                                  Value = c.Id.ToString(),
                                  Text = !string.IsNullOrEmpty(c.CustomName) ? c.CustomName : c.Name,
                                              // Disabled = (numCLIndex == (int?)null || c.LevelIndex < numCLIndex || oCurrChurchBody.OrgType == "CH" || oCurrChurchBody.OrgType == "CN")
                                          })
                              .ToList();



                oAttendModel.currAttendTask = currAttnTask;
                oAttendModel.currAttnVw = 0;
                oAttendModel.oAppGloOwnId = oAppGloOwnId;
                oAttendModel.strChurchBody = oCB.Name;  //current working CB
                oAttendModel.oChurchBodyId = oCB.Id;                
                oAttendModel.oChurchBody = oCB;  //current working CB 

                ///
                oAttendModel.oLoggedChurchBody = this._oLoggedCB;    //logged by user     
                oAttendModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                oAttendModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                oAttendModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                oAttendModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oAttendModel.oUserId_Logged = _oLoggedUser.Id;




                ViewBag.promptUserMsg = "";
                //TempData.Put("oVmCurr", oAttendModel);
                //TempData.Keep();

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oAttendModel);
                TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

                /// return View("", oAttendModel);

                if (loadLim)
                    return PartialView("_vwIndex_Attendance_CAA", oAttendModel);
                else
                    return View("Index_Attendance", oAttendModel);
            }

            catch (Exception ex)
            {
                if (loadLim)
                    return PartialView("_vwErrorPage");
                else
                    return View("_ErrorPage");
            }







            //oAttendModel.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs
            //if (oCB.OrgType == "CH")
            //{
            //    List<ChurchBody> congList = new List<ChurchBody>();
            //    congList = GetChurchUnits_Attend(oRequestedChurchBody, oRequestedChurchBody.Id, strAttendee, strLongevity);
            //    //if (!(filterIndex == null || filterVal == null))
            //    //    congList = GetMemberProfiles(congList, filterIndex, filterVal);

            //    oAttendModel.CHCF_TotAttend_MemOrVis = 0;
            //    oAttendModel.CHCF_TotAttend_M = 0;
            //    oAttendModel.CHCF_TotAttend_F = 0;
            //    oAttendModel.CHCF_TotAttend_O = 0;
            //    foreach (var cb in congList)
            //    {
            //        var qry = cb.CH_TotList_Attendees; //GetCurrentMemberProfiles(cb); 
            //        var maleCount = qry.ToList().Where(c => c.Gender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
            //        var femaleCount = qry.Where(c => c.Gender == "F").Count();
            //        var newCount = qry.Where(c => (DateTime)c.Created.Value.Date == DateTime.Now.Date).Count();

            //        cb.CH_TotMem = cb.CH_TotList_Attendees.Count();
            //        cb.CH_TotNewMem = newCount;
            //        //
            //        oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
            //        oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
            //        // oAttendModel.CHCF_TotNewMem += newCount;
            //        oAttendModel.CHCF_TotAttend_M += maleCount;
            //        oAttendModel.CHCF_TotAttend_F += femaleCount;
            //        oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount;
            //    }

            //    //oAttendModel.oChurchBody = oRequestedChurchBody; 
            //    oAttendModel.lsCurrSubChurchUnits = congList;

            //}
            //else if (oRequestedChurchBody.OrgType == "CN")
            //{
            //    var qry = new List<ChurchAttendanceModel>();
            //    // if (currAttendTask == 0) currAttendTask = 1;   //view the detailed

            //    //  List<ChurchAttendanceModel> qry = new  List<ChurchAttendanceModel>();

            //    if (currAttendTask == 1)
            //    {
            //        oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oAppGloOwnId, oCBid, strAttendee, strLongevity, eventId, eventDateMin, eventDateMax);
            //        qry = oAttendModel.lsChurchAttendanceModels;
            //    }
            //    else
            //    {
            //        // editing  ... single (id), multiple (id= -1) 
            //        if (id > 0)
            //        {
            //            //  oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null;
            //            // oAttendModel.f_DateAttended = eventDate != null ? (DateTime)eventDate : DateTime.Now;
            //            // oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null; //oAttendModel.f_DateAttended = eventDate != null ? (DateTime)eventDate : DateTime.Now;

            //            oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id);

            //            //oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended); 
            //            //  qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;

            //        }
            //        // editing  ... New
            //        else   //(id==0 || id < 0 || id ==null) // ((eventId == null || eventId < 0) && eventDate == null)
            //        {
            //            if (eventDate != null) // && eventId > 0 )
            //            {
            //                oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null;
            //                oAttendModel.f_DateAttended = eventDate != null ? (DateTime)eventDate : DateTime.Now;

            //                oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);
            //                // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;
            //            }
            //            else
            //            {
            //                //var ev = _context.ChurchCalendarEvent.Where(c => c.OwnedByChurchBodyId == oCurrChuBody.Id &&
            //                //     c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
            //                //var ev_Id = ev != null ? ev.Id : (int?)null;
            //                //oAttendModel.f_ChurchEventId = ev != null ? ev.Id : (int?)null;
            //                //oAttendModel.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

            //                oAttendModel.f_DateAttended = DateTime.Now;
            //                oAttendModel.lsChurchAttendanceModels_MultiEdit = new List<ChurchAttendanceModel>();
            //                // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;
            //            }
            //        }


            //        qry = oAttendModel.lsChurchAttendanceModels_MultiEdit.Where(c => c.f_ChkMemAttend == true).ToList();


            //        //if (id == null || id == 0) oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);  //
            //        //else oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id);

            //        //lookups
            //        //  var dt = oAttendModel.f_DateAttended.HasValue==true ? oAttendModel.f_DateAttended.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;

            //        //var ls = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.RelatedChurchLifeActivity).ToList();

            //        //foreach (var d in ls)
            //        //{
            //        //    var t = ((DateTime)d.EventFrom.Value).Date ;
            //        //    var ts = t == dts;
            //        //    var s = ((DateTime)d.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            //        //}





            //        var dtv = oAttendModel.f_DateAttended.HasValue == true ? ((DateTime)oAttendModel.f_DateAttended).Date : (DateTime?)null;
            //        var oCCList = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.ChurchlifeActivity_NVP)
            //            .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&   //c.ChurchBody.CountryId == oCurrChuBody.CountryId && 
            //                   ((DateTime)c.EventFrom.Value).Date == dtv).ToList(); // &&


            //        oAttendModel.lkpChuCalEvents = oCCList.Where(c =>
            //                    //(c.EventFrom.HasValue ? ((DateTime)c.EventFrom.Value).Date==dts : c.EventFrom==dts) &&
            //                    //((oAttendModel.f_DateAttended.HasValue==false && c.EventFrom == null) || 
            //                    //   (oAttendModel.f_DateAttended.HasValue==true && ((DateTime)c.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt)) &&
            //                    (c.ChurchBodyId == oCurrChuBody.Id ||
            //                    (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            //                    (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
            //                )
            //                 .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
            //                 .ToList()
            //                         .Select(c => new SelectListItem()
            //                         {
            //                             Value = c.Id.ToString(),
            //                             Text = c.Subject + ":- " +
            //                                                 (c.IsFullDay == true ?   
            //                                                     (c.EventFrom != null ? String.Format("{0:dddd, MMMM d, yyyy}", c.EventFrom) : "").Trim() :
            //                                                     (c.EventFrom != null ? String.Format("{0:dddd, MMMM d, yyyy}", c.EventFrom) : "").Trim() +
            //                                                     (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
            //                                                     (c.EventTo != null ? String.Format("{0:dddd, MMMM d, yyyy}", c.EventTo) : "").Trim()
            //                                                  )
            //                         })
            //                         //  .OrderBy(c => c.Text)
            //                         .ToList();

            //        oAttendModel.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            //        //
            //        //var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();

            //        //if (savedAttendVM == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "C", strLongevity); }
            //        //else if (savedAttendVM.lsChurchAttendanceModels == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "C", strLongevity); }  //only members ... visitors will use the modal form

            //        oAttendModel.lsChurchAttendanceModels = oAttendModel.lsChurchAttendanceModels;






            //    }


            //    //set the counts... oAttendModel.Result.ToList().OfType<MemberProfile>()
            //    var maleCount = qry.ToList().Where(c => c.strGender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
            //    var femaleCount = qry.Where(c => c.strGender == "F").Count();
            //    // var newCount = qry.Where(c => (DateTime)c.oChurchAttend.Created.Value.Date == DateTime.Now.Date).Count();

            //    // oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
            //    oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
            //    // oAttendModel.CHCF_TotNewMem += newCount;
            //    oAttendModel.CHCF_TotAttend_M += maleCount;
            //    oAttendModel.CHCF_TotAttend_F += femaleCount;
            //    oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount;

            //    // oAttendModel.lsChurchAttendanceModels = qry;
            //}



        }



        [HttpGet]
        public IActionResult IndexAttendCheckinHC_CAA(int currAttendTask = 0, int? oCurrChuBodyId = null, string strLongevity = "C", int? id = null, 
                                                        int? eventId = null, DateTime? attendDate = null, bool loadLim = false)
        {
            try
            {
                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return null; //// View("_ErrorPage");
                    }
                }


                //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


                if (!loadLim)
                    _ = this.LoadClientDashboardValues();  /// this._clientDBConnString);

                var oAppGloOwnId = this._oLoggedAGO.Id;
                ///var oCurrChuBodyId = reqChurchBodyId;

                //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCurrChuBodyId != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


                //var UserLogIn_Priv = TempData.Get<List<UserSessionPrivilege>>("UserLogIn_oUserPrivCol");
                //var checkUser = UserLogIn_Priv?.Count > 0;

                var oCurrChuBody = oCB; // oUserLogIn_Priv[0].ChurchBody;
                var oRequestedChurchBody = oCB;


                var oAttendModel = new ChurchAttendanceModel();

                if (currAttendTask == 0)
                {
                    oAttendModel.strCurrTaskDesc = oAttendModel.f_strAttendeeTypeCode == "C" ? oAttendModel.strAttnLongevity == "H" ? "Member Attendance Headcount -- History" : "Member Attendance Headcount (Past Week)" :
                                         oAttendModel.f_strAttendeeTypeCode == "V" ? oAttendModel.strAttnLongevity == "H" ? "Visitors History (Attendance) Headcount" : "Visitor Attendance Headcount (Past Week)" :
                                                                      oAttendModel.strAttnLongevity == "H" ? "Church Attendance Headcount -- History" : "Church Attendance Headcount (Past Week)";
                }
                else if (currAttendTask == 1)
                {
                    oAttendModel.strCurrTaskDesc = oAttendModel.f_strAttendeeTypeCode == "C" ? oAttendModel.strAttnLongevity == "H" ? "Member Attendance Headcount -- History" : "Member Attendance Headcount (Today)" :
                                         oAttendModel.f_strAttendeeTypeCode == "V" ? oAttendModel.strAttnLongevity == "H" ? "Visitors Headcount History (Attendance)" : "Visitor Attendance Headcount (Today)" :
                                                                      oAttendModel.strAttnLongevity == "H" ? "Church Attendance Headcount -- History" : "Church Attendance Headcount(Today)";  // isMigrated == true ? "New Converts" : "New Coverts (Historic)" : isMigrated == true ? "Visitors" : "Visitors (Historic)";
                }
                else
                {
                    oAttendModel.strCurrTaskDesc = "Church Attendance -- Headcount";
                }

                //   oAttendModel.isCurrMigrateQry = isMigrated;

                oAttendModel.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs
                if (oRequestedChurchBody.OrgType == "CH")
                {
                    List<ChurchBody> congList = new List<ChurchBody>();
                    congList = GetChurchUnits_Headcount(oRequestedChurchBody, oRequestedChurchBody.Id, null, strLongevity);
                    //if (!(filterIndex == null || filterVal == null))
                    //    congList = GetMemberProfiles(congList, filterIndex, filterVal);

                    oAttendModel.CHCF_TotAttend_MemOrVis = 0;
                    oAttendModel.CHCF_TotAttend_M = 0;
                    oAttendModel.CHCF_TotAttend_F = 0;
                    oAttendModel.CHCF_TotAttend_O = 0;
                    foreach (var cb in congList)
                    {
                        var qry = cb.CH_TotList_Attendees; //GetCurrentMemberProfiles(cb); 
                        var maleCount = qry.ToList().Where(c => c.Gender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
                        var femaleCount = qry.Where(c => c.Gender == "F").Count();
                        var newCount = qry.Where(c => (DateTime)c.Created.Value.Date == DateTime.Now.Date).Count();

                        cb.CH_TotMem = cb.CH_TotList_Attendees.Count();
                        cb.CH_TotNewMem = newCount;
                        //
                        oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
                        oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
                        // oAttendModel.CHCF_TotNewMem += newCount;
                        oAttendModel.CHCF_TotAttend_M += maleCount;
                        oAttendModel.CHCF_TotAttend_F += femaleCount;
                        oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount;
                    }

                    //oAttendModel.oChurchBody = oRequestedChurchBody; 
                    oAttendModel.lsCurrSubChurchUnits = congList;

                }
                else if (oRequestedChurchBody.OrgType == "CN")
                {
                    //  var qry = new List<ChurchAttendanceModel>();
                    // if (currAttendTask == 0) currAttendTask = 1;   //view the detailed

                    //  List<ChurchAttendanceModel> qry = new  List<ChurchAttendanceModel>();


                    if (currAttendTask == 0)
                    {
                        //  oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, strAttendee, strLongevity);
                        // qry = oAttendModel.lsChurchAttendanceModels;

                        // oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null;
                        // oAttendModel.f_DateAttended = attendDate != null ? (DateTime)attendDate : DateTime.Now;

                        oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList_Any(oRequestedChurchBody, 7);
                        // qry = oAttendModel.lsChurchAttendanceModels_HC;   //, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended
                        oAttendModel.lsChurchAttendanceModels_HCSumm = ReturnAttendVMTotals_Summ(oAttendModel.lsChurchAttendanceModels_HC);   //oAttendModel, 
                    }

                    else if (currAttendTask == 1)
                    {
                        oAttendModel.f_ChurchEventDetailId = (eventId > 0) ? eventId : (int?)null;
                        oAttendModel.f_DateAttended = attendDate != null ? (DateTime)attendDate : DateTime.Now;
                        oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList(oAppGloOwnId, oCurrChuBodyId, strLongevity, eventId, attendDate);

                        // qry = oAttendModel.lsChurchAttendanceModels_HC;   //, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended
                        oAttendModel = ReturnAttendVMTotals(oAttendModel, oAttendModel.lsChurchAttendanceModels_HC);
                    }

                    else
                    {
                        // editing  ... single (id), multiple (id= -1) 
                        if (id > 0)
                        {
                            oAttendModel.lsChurchAttendanceModels_HCEdit = ReturnCongHeadcountList_EditById(oRequestedChurchBody, (int)id);
                            //ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id); 
                        }
                        // editing  ... New
                        else   //(id==0 || id < 0 || id ==null) // ((eventId == null || eventId < 0) && eventDate == null)
                        {
                            if (attendDate != null && eventId > 0)
                            {
                                oAttendModel.f_ChurchEventDetailId = (eventId > 0) ? eventId : (int?)null;
                                oAttendModel.f_DateAttended = attendDate != null ? (DateTime)attendDate : DateTime.Now;

                                oAttendModel.lsChurchAttendanceModels_HCEdit = ReturnCongHeadcountList(oAppGloOwnId, oCurrChuBodyId, strLongevity, eventId, attendDate);
                                //ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);
                                // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;

                                //ADD NEW REC...
                                if (id == 0)
                                {
                                    var oAttendHC = new ChurchAttendanceModel()
                                    {
                                        oAttend_Id = 0,
                                        oChurchBodyId = oCurrChuBody.Id,
                                        CountCategoryId = null,
                                        CHCF_TotAttend_M = 0,
                                        CHCF_TotAttend_F = 0,
                                        CHCF_TotAttend_O = 0,
                                        CHCF_TotAttend_MemOrVis = 0
                                    };

                                    oAttendModel.lsChurchAttendanceModels_HCEdit.Add(oAttendHC);
                                }
                            }
                            else
                            {
                                //var ev = _context.ChurchCalendarEvent.Where(c => c.OwnedByChurchBodyId == oCurrChuBody.Id &&
                                //     c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
                                //var ev_Id = ev != null ? ev.Id : (int?)null;
                                //oAttendModel.f_ChurchEventId = ev != null ? ev.Id : (int?)null;
                                //oAttendModel.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

                                oAttendModel.f_DateAttended = DateTime.Now;
                                oAttendModel.lsChurchAttendanceModels_HCEdit = new List<ChurchAttendanceModel>();
                                // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;
                            }
                        }

                        // qry = oAttendModel.lsChurchAttendanceModels_HCEdit; //.Where(c => c.f_ChkMemAttend == true).ToList();

                        //if (id == null || id == 0) oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);  //
                        //else oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id);

                        //lookups
                        //  var dt = oAttendModel.f_DateAttended.HasValue==true ? oAttendModel.f_DateAttended.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;


                        //var ls = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.RelatedChurchLifeActivity).ToList();

                        //foreach (var d in ls)
                        //{
                        //    var t = ((DateTime)d.EventFrom.Value).Date ;
                        //    var ts = t == dts;
                        //    var s = ((DateTime)d.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        //}


                        //
                        //var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
                        //if (savedAttendVM == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "M", strLongevity); }
                        //else if (savedAttendVM.lsChurchAttendanceModels == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "M", strLongevity); }  //only members ... visitors will use the modal form

                        oAttendModel.lsChurchAttendanceModels = oAttendModel.lsChurchAttendanceModels;

                        //get church groups lists... generationl is default
                        oAttendModel.lkpCountGroups = _context.ChurchUnit   //.Include(t => t.OwnedByChurchBody)
                                        .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && !string.IsNullOrEmpty(c.Name) &&
                                                    ((c.OrgType == "CN" && c.Id == oCurrChuBody.Id) || c.OrgType == "CG") //&& //c.IsUnitGenerational == true &&
                                                                                                                          //(c.OwnedByChurchBodyId == oCurrChuBody.Id ||
                                                                                                                          //(c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                                                                                                                          //(c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
                                )
                                 .OrderByDescending(c => c.OrgType).ThenByDescending(c => c.IsUnitGen).ThenBy(c => c.Description).ToList()
                             .Select(c => new SelectListItem()
                             {
                                 Value = c.Id.ToString(),
                                 Text = c.Id == oCurrChuBody.Id ? "All Congregants" : c.Name
                             })
                             .ToList();

                        oAttendModel.lkpCountGroups.Insert(0, new SelectListItem { Value = "", Text = "-- Select Count Area --" });
                        // oAttendModel.lkpCountGroups.Insert(1, new SelectListItem { Value = "-1", Text = "All Congregants" });
                        // oAttendModel.lkpCountGroups.Insert(2, new SelectListItem { Value = "-1", Text = "Visitors (Guests)" });

                        oAttendModel.lkpCountGroups.Add(new SelectListItem { Value = "-1", Text = "Visitors (Guests)" });

                        //oAttendModel.mc_IsCongregation_Summ = true;
                        //oAttendModel.mc_IsGenChurchGroup_Summ = false ;
                        //oAttendModel.mc_IsInterGenChurchGroup_Summ = false ;
                        //
                        //var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
                        //if (savedAttendVM == null) { oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList(oRequestedChurchBody, strLongevity, eventId, attendDate); }
                        //else if (savedAttendVM.lsChurchAttendanceModels_HC == null) { oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList(oRequestedChurchBody, strCountType, strLongevity); }  //only members ... visitors will use the modal form
                        //oAttendModel.lsChurchAttendanceModels_HC = savedAttendVM.lsChurchAttendanceModels_HC;
                    }

                    //var dtv = oAttendModel.f_DateAttended.HasValue == true ? ((DateTime)oAttendModel.f_DateAttended).Date : (DateTime?)null;
                    oAttendModel.lkpChuCalEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.ChurchlifeActivity_NVP)
                          .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&   //c.ChurchBody.CountryId == oCurrChuBody.CountryId && 
                                                                                                                                                                         //  ((DateTime)c.EventFrom.Value).Date == dtv &&

                                  //(c.EventFrom.HasValue ? ((DateTime)c.EventFrom.Value).Date==dts : c.EventFrom==dts) &&
                                  //((oAttendModel.f_DateAttended.HasValue==false && c.EventFrom == null) || 
                                  //   (oAttendModel.f_DateAttended.HasValue==true && ((DateTime)c.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt)) &&
                                  (c.ChurchBodyId == oCurrChuBody.Id ||
                                  (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                                  (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
                              )
                               .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
                               .ToList()
                                       .Select(c => new SelectListItem()
                                       {
                                           Value = c.Id.ToString(),
                                           Text = c.Subject + ":- " +
                                                               (c.IsFullDay == true ?
                                                                   (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                   (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                   (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                                                                   (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                )
                                       })
                                       //  .OrderBy(c => c.Text)
                                       .ToList();

                    oAttendModel.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });


                    // //set the counts... oAttendModel.Result.ToList().OfType<MemberProfile>()
                    // var maleCount = qry.ToList().Where(c => c.oChurchAttend.Gender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
                    // var femaleCount = qry.Where(c => c.oChurchAttend.Gender == "F").Count();
                    // var newCount = qry.Where(c => (DateTime)c.oChurchAttend.Created.Value.Date == DateTime.Now.Date).Count();

                    //// oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
                    // oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
                    // // oAttendModel.CHCF_TotNewMem += newCount;
                    // oAttendModel.CHCF_TotAttend_M += maleCount;
                    // oAttendModel.CHCF_TotAttend_F += femaleCount;
                    // oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount; 

                    // oAttendModel.lsChurchAttendanceModels = qry;
                }

                oAttendModel.currAttendTask = currAttendTask;
                oAttendModel.oChurchBody = oRequestedChurchBody;  //current working CB
                oAttendModel.oChurchBodyId = oRequestedChurchBody.Id;
                oAttendModel.oLoggedChurchBody = oCurrChuBody;    //logged by user                                                             

                ViewBag.promptUserMsg = "";
                //TempData.Put("oVmCurr", oAttendModel);
                //TempData.Keep();


                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oAttendModel);
                TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

                return View("_vwAttendCheckinHC_CAA", oAttendModel);

            }
            catch (Exception ex)
            {
                if (loadLim)
                    return PartialView("_vwErrorPage");
                else
                    return View("_ErrorPage");
            }
        }







        [HttpGet]
        public IActionResult Index_Attendees(int currAttendTask = 1, int? reqChurchBodyId = null, string strAttendee = "C", string strLongevity = "C", int? id = null, int? eventId = null, DateTime? eventDate = null, bool loadLim = false) //, char? filterIndex = null, int? filterVal = null)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


            if (!loadLim)
                _ = this.LoadClientDashboardValues();  /// this._clientDBConnString);

            var oAppGloOwnId = this._oLoggedAGO.Id;
            var oCurrChuBodyId = reqChurchBodyId;

            //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

            oCurrChuBodyId = oCurrChuBodyId == null ? this._oLoggedCB.Id : oCurrChuBodyId;
            ChurchBody oCB = this._oLoggedCB;
            if (oCurrChuBodyId != this._oLoggedCB.Id)
                oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


            //var UserLogIn_Priv = TempData.Get<List<UserSessionPrivilege>>("UserLogIn_oUserPrivCol");
            //var checkUser = UserLogIn_Priv?.Count > 0;

            var oCurrChuBody = oCB; // oUserLogIn_Priv[0].ChurchBody;
            var oRequestedChurchBody = oCB;

            var oAttendModel = new ChurchAttendanceModel();
            oAttendModel.strAttnLongevity = strLongevity;  // today (C), history (H)
            oAttendModel.f_strAttendeeTypeCode = strAttendee;  //else history
            if (currAttendTask == 1)
            {
                oAttendModel.strCurrTaskDesc = oAttendModel.f_strAttendeeTypeCode == "C" ? oAttendModel.strAttnLongevity == "H" ? "Member Attendance Log -- History" : "Member Attendance (Past Week)" :
                                     oAttendModel.f_strAttendeeTypeCode == "V" ? oAttendModel.strAttnLongevity == "H" ? "Visitors History (Attendance)" : "Visitor Attendance (Past Week)" :
                                                                  oAttendModel.strAttnLongevity == "H" ? "Church Attendance Log -- History" : "Church Attendance (Past Week)";  // isMigrated == true ? "New Converts" : "New Coverts (Historic)" : isMigrated == true ? "Visitors" : "Visitors (Historic)";
            }

            else
            {
                oAttendModel.strCurrTaskDesc = "Church Attendance";
            }

            //   oAttendModel.isCurrMigrateQry = isMigrated;

            oAttendModel.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs
            if (oCB.OrgType == "CH")
            {
                List<ChurchBody> congList = new List<ChurchBody>();
                congList = GetChurchUnits_Attend(oRequestedChurchBody, oRequestedChurchBody.Id, strAttendee, strLongevity);
                //if (!(filterIndex == null || filterVal == null))
                //    congList = GetMemberProfiles(congList, filterIndex, filterVal);

                oAttendModel.CHCF_TotAttend_MemOrVis = 0;
                oAttendModel.CHCF_TotAttend_M = 0;
                oAttendModel.CHCF_TotAttend_F = 0;
                oAttendModel.CHCF_TotAttend_O = 0;
                foreach (var cb in congList)
                {
                    var qry = cb.CH_TotList_Attendees; //GetCurrentMemberProfiles(cb); 
                    var maleCount = qry.ToList().Where(c => c.Gender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
                    var femaleCount = qry.Where(c => c.Gender == "F").Count();
                    var newCount = qry.Where(c => (DateTime)c.Created.Value.Date == DateTime.Now.Date).Count();

                    cb.CH_TotMem = cb.CH_TotList_Attendees.Count();
                    cb.CH_TotNewMem = newCount;
                    //
                    oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
                    oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
                    // oAttendModel.CHCF_TotNewMem += newCount;
                    oAttendModel.CHCF_TotAttend_M += maleCount;
                    oAttendModel.CHCF_TotAttend_F += femaleCount;
                    oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount;
                }

                //oAttendModel.oChurchBody = oRequestedChurchBody; 
                oAttendModel.lsCurrSubChurchUnits = congList;

            }
            else if (oRequestedChurchBody.OrgType == "CN")
            {
                var qry = new List<ChurchAttendanceModel>();
                // if (currAttendTask == 0) currAttendTask = 1;   //view the detailed

                //  List<ChurchAttendanceModel> qry = new  List<ChurchAttendanceModel>();

                if (currAttendTask == 1)
                {
                    oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oAppGloOwnId, oCurrChuBodyId, strAttendee);  //, strLongevity
                    qry = oAttendModel.lsChurchAttendanceModels;
                }
                else
                {
                    // editing  ... single (id), multiple (id= -1) 
                    if (id > 0)
                    {
                        //  oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null;
                        // oAttendModel.f_DateAttended = eventDate != null ? (DateTime)eventDate : DateTime.Now;
                        // oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null; //oAttendModel.f_DateAttended = eventDate != null ? (DateTime)eventDate : DateTime.Now;

                        oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id);

                        //oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended); 
                        //  qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;

                    }
                    // editing  ... New
                    else   //(id==0 || id < 0 || id ==null) // ((eventId == null || eventId < 0) && eventDate == null)
                    {
                        if (eventDate != null) // && eventId > 0 )
                        {
                            oAttendModel.f_ChurchEventDetailId = (eventId > 0) ? eventId : (int?)null;
                            oAttendModel.f_DateAttended = eventDate != null ? (DateTime)eventDate : DateTime.Now;

                            oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventDetailId, oAttendModel.f_DateAttended);
                            // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;
                        }
                        else
                        {
                            //var ev = _context.ChurchCalendarEvent.Where(c => c.OwnedByChurchBodyId == oCurrChuBody.Id &&
                            //     c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
                            //var ev_Id = ev != null ? ev.Id : (int?)null;
                            //oAttendModel.f_ChurchEventId = ev != null ? ev.Id : (int?)null;
                            //oAttendModel.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

                            oAttendModel.f_DateAttended = DateTime.Now;
                            oAttendModel.lsChurchAttendanceModels_MultiEdit = new List<ChurchAttendanceModel>();
                            // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;
                        }
                    }

                    qry = oAttendModel.lsChurchAttendanceModels_MultiEdit.Where(c => c.f_ChkMemAttend == true).ToList();

                    //if (id == null || id == 0) oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);  //
                    //else oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id);

                    //lookups
                    //  var dt = oAttendModel.f_DateAttended.HasValue==true ? oAttendModel.f_DateAttended.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;

                    //var ls = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.RelatedChurchLifeActivity).ToList();

                    //foreach (var d in ls)
                    //{
                    //    var t = ((DateTime)d.EventFrom.Value).Date ;
                    //    var ts = t == dts;
                    //    var s = ((DateTime)d.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    //}

                    var dtv = oAttendModel.f_DateAttended.HasValue == true ? ((DateTime)oAttendModel.f_DateAttended).Date : (DateTime?)null;
                    var oCCList = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.ChurchlifeActivity_NVP)
                        .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&   //c.ChurchBody.CountryId == oCurrChuBody.CountryId && 
                               ((DateTime)c.EventFrom.Value).Date == dtv).ToList(); // &&


                    oAttendModel.lkpChuCalEvents = oCCList.Where(c=> 
                                //(c.EventFrom.HasValue ? ((DateTime)c.EventFrom.Value).Date==dts : c.EventFrom==dts) &&
                                //((oAttendModel.f_DateAttended.HasValue==false && c.EventFrom == null) || 
                                //   (oAttendModel.f_DateAttended.HasValue==true && ((DateTime)c.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt)) &&
                                (c.ChurchBodyId == oCurrChuBody.Id ||
                                (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                                (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
                            )
                             .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
                             .ToList()
                                     .Select(c => new SelectListItem()
                                     {
                                         Value = c.Id.ToString(),
                                         Text = c.Subject + ":- " +
                                                             (c.IsFullDay == true ?
                                                                 (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                 (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                 (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                                                                 (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                              )
                                     })
                                     //  .OrderBy(c => c.Text)
                                     .ToList();

                    oAttendModel.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });
                    //
                    //var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();

                    //if (savedAttendVM == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "C", strLongevity); }
                    //else if (savedAttendVM.lsChurchAttendanceModels == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "C", strLongevity); }  //only members ... visitors will use the modal form
                    
                    oAttendModel.lsChurchAttendanceModels = oAttendModel.lsChurchAttendanceModels;
                }


                //set the counts... oAttendModel.Result.ToList().OfType<MemberProfile>()
                var maleCount = qry.ToList().Where(c => c.strGender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
                var femaleCount = qry.Where(c => c.strGender == "F").Count();
                // var newCount = qry.Where(c => (DateTime)c.oChurchAttend.Created.Value.Date == DateTime.Now.Date).Count();

                // oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
                oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
                // oAttendModel.CHCF_TotNewMem += newCount;
                oAttendModel.CHCF_TotAttend_M += maleCount;
                oAttendModel.CHCF_TotAttend_F += femaleCount;
                oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount;

                // oAttendModel.lsChurchAttendanceModels = qry;
            }

            oAttendModel.currAttendTask = currAttendTask;
            oAttendModel.oChurchBody = oRequestedChurchBody;  //current working CB
            oAttendModel.oChurchBodyId = oRequestedChurchBody.Id;
            oAttendModel.oLoggedChurchBody = oCurrChuBody;    //logged by user     
            ///
            oAttendModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
            oAttendModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
            oAttendModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
            oAttendModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
            oAttendModel.oUserId_Logged = _oLoggedUser.Id;




            ViewBag.promptUserMsg = "";
            //TempData.Put("oVmCurr", oAttendModel);
            //TempData.Keep();

            var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oAttendModel);
            TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

            return View(oAttendModel);

            //ViewBag.oMemberList = GetMemberList((int)oUserProRoleLog.UserProfile.ChurchBodyId);
            //AddOrEditPersData();

            //TempData.Keep();   
            //return View();
        }


       


        [HttpGet]
        // public IActionResult Index_Headcount(int currAttendTask = 1, int? reqChurchBodyId = null, string strCountType="C", string strLongevity="C", int? id = null) // Cong or Groups ... char? filterIndex = null, int? filterVal = null)
        public IActionResult Index_Headcount(int currAttendTask = 0, int? reqChurchBodyId = null, string strLongevity = "C", int? id = null, int? 
                                            eventId = null, DateTime? attendDate = null, bool loadLim = false)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


            if (!loadLim)
                _ = this.LoadClientDashboardValues();  /// this._clientDBConnString);

            var oAppGloOwnId = this._oLoggedAGO.Id;
            var oCurrChuBodyId = reqChurchBodyId;

            //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

            ChurchBody oCB = this._oLoggedCB;
            if (oCurrChuBodyId != this._oLoggedCB.Id)
                oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


            //var UserLogIn_Priv = TempData.Get<List<UserSessionPrivilege>>("UserLogIn_oUserPrivCol");
            //var checkUser = UserLogIn_Priv?.Count > 0;

            var oCurrChuBody = oCB; // oUserLogIn_Priv[0].ChurchBody;
            var oRequestedChurchBody = oCB;


            var oAttendModel = new ChurchAttendanceModel();

            if (currAttendTask == 0)
            {
                oAttendModel.strCurrTaskDesc = oAttendModel.f_strAttendeeTypeCode == "C" ? oAttendModel.strAttnLongevity == "H" ? "Member Attendance Headcount -- History" : "Member Attendance Headcount (Past Week)" :
                                     oAttendModel.f_strAttendeeTypeCode == "V" ? oAttendModel.strAttnLongevity == "H" ? "Visitors History (Attendance) Headcount" : "Visitor Attendance Headcount (Past Week)" :
                                                                  oAttendModel.strAttnLongevity == "H" ? "Church Attendance Headcount -- History" : "Church Attendance Headcount (Past Week)";
            }
            else if (currAttendTask == 1)
            {
                oAttendModel.strCurrTaskDesc = oAttendModel.f_strAttendeeTypeCode == "C" ? oAttendModel.strAttnLongevity == "H" ? "Member Attendance Headcount -- History" : "Member Attendance Headcount (Today)" :
                                     oAttendModel.f_strAttendeeTypeCode == "V" ? oAttendModel.strAttnLongevity == "H" ? "Visitors Headcount History (Attendance)" : "Visitor Attendance Headcount (Today)" :
                                                                  oAttendModel.strAttnLongevity == "H" ? "Church Attendance Headcount -- History" : "Church Attendance Headcount(Today)";  // isMigrated == true ? "New Converts" : "New Coverts (Historic)" : isMigrated == true ? "Visitors" : "Visitors (Historic)";
            }
            else
            {
                oAttendModel.strCurrTaskDesc = "Church Attendance -- Headcount";
            }

            //   oAttendModel.isCurrMigrateQry = isMigrated;

            oAttendModel.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs
            if (oRequestedChurchBody.OrgType == "CH")
            {
                List<ChurchBody> congList = new List<ChurchBody>();
                congList = GetChurchUnits_Headcount(oRequestedChurchBody, oRequestedChurchBody.Id, null, strLongevity);
                //if (!(filterIndex == null || filterVal == null))
                //    congList = GetMemberProfiles(congList, filterIndex, filterVal);

                oAttendModel.CHCF_TotAttend_MemOrVis = 0;
                oAttendModel.CHCF_TotAttend_M = 0;
                oAttendModel.CHCF_TotAttend_F = 0;
                oAttendModel.CHCF_TotAttend_O = 0;
                foreach (var cb in congList)
                {
                    var qry = cb.CH_TotList_Attendees; //GetCurrentMemberProfiles(cb); 
                    var maleCount = qry.ToList().Where(c => c.Gender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
                    var femaleCount = qry.Where(c => c.Gender == "F").Count();
                    var newCount = qry.Where(c => (DateTime)c.Created.Value.Date == DateTime.Now.Date).Count();

                    cb.CH_TotMem = cb.CH_TotList_Attendees.Count();
                    cb.CH_TotNewMem = newCount;
                    //
                    oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
                    oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
                    // oAttendModel.CHCF_TotNewMem += newCount;
                    oAttendModel.CHCF_TotAttend_M += maleCount;
                    oAttendModel.CHCF_TotAttend_F += femaleCount;
                    oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount;
                }

                //oAttendModel.oChurchBody = oRequestedChurchBody; 
                oAttendModel.lsCurrSubChurchUnits = congList;

            }
            else if (oRequestedChurchBody.OrgType == "CN")
            {
                //  var qry = new List<ChurchAttendanceModel>();
                // if (currAttendTask == 0) currAttendTask = 1;   //view the detailed

                //  List<ChurchAttendanceModel> qry = new  List<ChurchAttendanceModel>();


                if (currAttendTask == 0)
                {
                    //  oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, strAttendee, strLongevity);
                    // qry = oAttendModel.lsChurchAttendanceModels;

                    // oAttendModel.f_ChurchEventId = (eventId > 0) ? eventId : (int?)null;
                    // oAttendModel.f_DateAttended = attendDate != null ? (DateTime)attendDate : DateTime.Now;

                    oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList_Any(oRequestedChurchBody, 7);
                    // qry = oAttendModel.lsChurchAttendanceModels_HC;   //, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended
                    oAttendModel.lsChurchAttendanceModels_HCSumm = ReturnAttendVMTotals_Summ(oAttendModel.lsChurchAttendanceModels_HC);   //oAttendModel, 
                }

                else if (currAttendTask == 1)
                {
                    oAttendModel.f_ChurchEventDetailId = (eventId > 0) ? eventId : (int?)null;
                    oAttendModel.f_DateAttended = attendDate != null ? (DateTime)attendDate : DateTime.Now;
                    oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList(oAppGloOwnId, oCurrChuBodyId, strLongevity, eventId, attendDate);

                    // qry = oAttendModel.lsChurchAttendanceModels_HC;   //, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended
                    oAttendModel = ReturnAttendVMTotals(oAttendModel, oAttendModel.lsChurchAttendanceModels_HC);
                }

                else
                {
                    // editing  ... single (id), multiple (id= -1) 
                    if (id > 0)
                    {
                        oAttendModel.lsChurchAttendanceModels_HCEdit = ReturnCongHeadcountList_EditById(oRequestedChurchBody, (int)id);
                        //ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id); 
                    }
                    // editing  ... New
                    else   //(id==0 || id < 0 || id ==null) // ((eventId == null || eventId < 0) && eventDate == null)
                    {
                        if (attendDate != null && eventId > 0)
                        {
                            oAttendModel.f_ChurchEventDetailId = (eventId > 0) ? eventId : (int?)null;
                            oAttendModel.f_DateAttended = attendDate != null ? (DateTime)attendDate : DateTime.Now;

                            oAttendModel.lsChurchAttendanceModels_HCEdit = ReturnCongHeadcountList(oAppGloOwnId, oCurrChuBodyId, strLongevity, eventId, attendDate);
                            //ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);
                            // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;

                            //ADD NEW REC...
                            if (id == 0)
                            {
                                var oAttendHC = new ChurchAttendanceModel()
                                {
                                    oAttend_Id = 0,
                                    oChurchBodyId = oCurrChuBody.Id,
                                    CountCategoryId = null,
                                    CHCF_TotAttend_M = 0,
                                    CHCF_TotAttend_F = 0,
                                    CHCF_TotAttend_O = 0,
                                    CHCF_TotAttend_MemOrVis = 0
                                };

                                oAttendModel.lsChurchAttendanceModels_HCEdit.Add(oAttendHC);
                            }
                        }
                        else
                        {
                            //var ev = _context.ChurchCalendarEvent.Where(c => c.OwnedByChurchBodyId == oCurrChuBody.Id &&
                            //     c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
                            //var ev_Id = ev != null ? ev.Id : (int?)null;
                            //oAttendModel.f_ChurchEventId = ev != null ? ev.Id : (int?)null;
                            //oAttendModel.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

                            oAttendModel.f_DateAttended = DateTime.Now;
                            oAttendModel.lsChurchAttendanceModels_HCEdit = new List<ChurchAttendanceModel>();
                            // qry = oAttendModel.lsChurchAttendanceModels_MultiEdit;
                        }
                    }

                    // qry = oAttendModel.lsChurchAttendanceModels_HCEdit; //.Where(c => c.f_ChkMemAttend == true).ToList();

                    //if (id == null || id == 0) oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, oAttendModel.f_ChurchEventId, oAttendModel.f_DateAttended);  //
                    //else oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oRequestedChurchBody, (int)id);

                    //lookups
                    //  var dt = oAttendModel.f_DateAttended.HasValue==true ? oAttendModel.f_DateAttended.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;


                    //var ls = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.RelatedChurchLifeActivity).ToList();

                    //foreach (var d in ls)
                    //{
                    //    var t = ((DateTime)d.EventFrom.Value).Date ;
                    //    var ts = t == dts;
                    //    var s = ((DateTime)d.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    //}


                    //
                    //var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
                    //if (savedAttendVM == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "M", strLongevity); }
                    //else if (savedAttendVM.lsChurchAttendanceModels == null) { oAttendModel.lsChurchAttendanceModels = ReturnChurchAttendeesList(oRequestedChurchBody, "M", strLongevity); }  //only members ... visitors will use the modal form
                    
                    oAttendModel.lsChurchAttendanceModels = oAttendModel.lsChurchAttendanceModels;

                    //get church groups lists... generationl is default
                    oAttendModel.lkpCountGroups = _context.ChurchUnit   //.Include(t => t.OwnedByChurchBody)
                                    .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && !string.IsNullOrEmpty(c.Name) &&
                                                ((c.OrgType == "CN" && c.Id == oCurrChuBody.Id) || c.OrgType == "CG") //&& //c.IsUnitGenerational == true &&
                                                                                                                      //(c.OwnedByChurchBodyId == oCurrChuBody.Id ||
                                                                                                                      //(c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                                                                                                                      //(c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
                            )
                             .OrderByDescending(c => c.OrgType).ThenByDescending(c => c.IsUnitGen).ThenBy(c => c.Description).ToList()
                         .Select(c => new SelectListItem()
                         {
                             Value = c.Id.ToString(),
                             Text = c.Id == oCurrChuBody.Id ? "All Congregants" : c.Name
                         })
                         .ToList();

                    oAttendModel.lkpCountGroups.Insert(0, new SelectListItem { Value = "", Text = "-- Select Count Area --" });
                    // oAttendModel.lkpCountGroups.Insert(1, new SelectListItem { Value = "-1", Text = "All Congregants" });
                    // oAttendModel.lkpCountGroups.Insert(2, new SelectListItem { Value = "-1", Text = "Visitors (Guests)" });

                    oAttendModel.lkpCountGroups.Add(new SelectListItem { Value = "-1", Text = "Visitors (Guests)" });

                    //oAttendModel.mc_IsCongregation_Summ = true;
                    //oAttendModel.mc_IsGenChurchGroup_Summ = false ;
                    //oAttendModel.mc_IsInterGenChurchGroup_Summ = false ;
                    //
                    //var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
                    //if (savedAttendVM == null) { oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList(oRequestedChurchBody, strLongevity, eventId, attendDate); }
                    //else if (savedAttendVM.lsChurchAttendanceModels_HC == null) { oAttendModel.lsChurchAttendanceModels_HC = ReturnCongHeadcountList(oRequestedChurchBody, strCountType, strLongevity); }  //only members ... visitors will use the modal form
                    //oAttendModel.lsChurchAttendanceModels_HC = savedAttendVM.lsChurchAttendanceModels_HC;
                }

                var dtv = oAttendModel.f_DateAttended.HasValue == true ? ((DateTime)oAttendModel.f_DateAttended).Date : (DateTime?)null;
                oAttendModel.lkpChuCalEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchBody).Include(t => t.ChurchlifeActivity_NVP)
                      .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&   //c.ChurchBody.CountryId == oCurrChuBody.CountryId && 
                                                                                                                                                                   //  ((DateTime)c.EventFrom.Value).Date == dtv &&

                              //(c.EventFrom.HasValue ? ((DateTime)c.EventFrom.Value).Date==dts : c.EventFrom==dts) &&
                              //((oAttendModel.f_DateAttended.HasValue==false && c.EventFrom == null) || 
                              //   (oAttendModel.f_DateAttended.HasValue==true && ((DateTime)c.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt)) &&
                              (c.ChurchBodyId == oCurrChuBody.Id ||
                              (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                              (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
                          )
                           .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
                           .ToList()
                                   .Select(c => new SelectListItem()
                                   {
                                       Value = c.Id.ToString(),
                                       Text = c.Subject + ":- " +
                                                           (c.IsFullDay == true ?
                                                               (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                               (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                               (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                                                               (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                            )
                                   })
                                   //  .OrderBy(c => c.Text)
                                   .ToList();

                oAttendModel.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });


                // //set the counts... oAttendModel.Result.ToList().OfType<MemberProfile>()
                // var maleCount = qry.ToList().Where(c => c.oChurchAttend.Gender == "M").Count();   //oAttendModel.Result.ToList().OfType<MemberProfile>()
                // var femaleCount = qry.Where(c => c.oChurchAttend.Gender == "F").Count();
                // var newCount = qry.Where(c => (DateTime)c.oChurchAttend.Created.Value.Date == DateTime.Now.Date).Count();

                //// oAttendModel.CHCF_TotCong += cb.CH_TotSubUnits;
                // oAttendModel.CHCF_TotAttend_MemOrVis += qry.Count();
                // // oAttendModel.CHCF_TotNewMem += newCount;
                // oAttendModel.CHCF_TotAttend_M += maleCount;
                // oAttendModel.CHCF_TotAttend_F += femaleCount;
                // oAttendModel.CHCF_TotAttend_O += qry.Count - maleCount - femaleCount; 

                // oAttendModel.lsChurchAttendanceModels = qry;
            }

            oAttendModel.currAttendTask = currAttendTask;
            oAttendModel.oChurchBody = oRequestedChurchBody;  //current working CB
            oAttendModel.oChurchBodyId = oRequestedChurchBody.Id;
            oAttendModel.oLoggedChurchBody = oCurrChuBody;    //logged by user                                                             

            ViewBag.promptUserMsg = "";
            //TempData.Put("oVmCurr", oAttendModel);
            //TempData.Keep();


            var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oAttendModel);
            TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

            return View(oAttendModel);

            //ViewBag.oMemberList = GetMemberList((int)oUserProRoleLog.UserProfile.ChurchBodyId);
            //AddOrEditPersData();

            //TempData.Keep();
            //return View();
        }


        //[HttpGet]
        //public IActionResult Index_Attendees_MemEdit2(int? reqChurchBodyId = null, string strLongevity = "C", bool loadLim = false) // or History (H), char? filterIndex = null, int? filterVal = null)
        //{

        //    if (this._context == null)
        //    {
        //        this._context = GetClientDBContext();
        //        if (this._context == null)
        //        {
        //            RedirectToAction("LoginUserAcc", "UserLogin");

        //            // should not get here... Response.StatusCode = 500; 
        //            return View("_ErrorPage");
        //        }
        //    }

        //    if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
        //    { RedirectToAction("LoginUserAcc", "UserLogin"); }


        //    if (!loadLim)
        //        _ = this.LoadClientDashboardValues(this._clientDBConnString);

        //    var oAppGloOwnId = this._oLoggedAGO.Id;
        //    var oCurrChuBodyId = reqChurchBodyId;

        //    //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
        //    //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

        //    ChurchBody oCB = this._oLoggedCB;
        //    if (oCurrChuBodyId != this._oLoggedCB.Id)
        //        oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


        //    //var UserLogIn_Priv = TempData.Get<List<UserSessionPrivilege>>("UserLogIn_oUserPrivCol");
        //    //var checkUser = UserLogIn_Priv?.Count > 0;

        //    var oCurrChuBody = oCB; // oUserLogIn_Priv[0].ChurchBody;
        //    var oRequestedChurchBody = oCB;


        //    var oAttend = new ChurchAttendanceModel();

        //    oAttend.strAttnLongevity = strLongevity;  // today (C), history (H) 
        //    oAttend.strCurrTaskDesc = oAttend.strAttnLongevity == "H" ? "Member Attendance History" : "Member Attendance Log (Today)";

        //    //   ViewBag.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs

        //    // List<ChurchAttendanceModel> qry = new List<ChurchAttendanceModel>();
        //    var ev = _context.ChurchCalendarEvent.Where(c => c.OwnedByChurchBodyId == oCurrChuBody.Id &&
        //       c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
        //    var ev_Id = ev != null ? ev.Id : (int?)null;
        //    oAttend.f_DateAttended = ev != null ? ev.EventFrom : DateTime.Now;

        //    oAttend.lsChurchAttendanceModels = ReturnChurchAttendeesList_MemEdit(oRequestedChurchBody, ev_Id, oAttend.f_DateAttended);

        //    //lookups
        //    oAttend.lkpChuCalEvents = _context.ChurchCalendarEvent.Include(t => t.RelatedChurchLifeActivity)
        //                            .Where(c => !string.IsNullOrEmpty(c.Subject) && c.EventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&
        //                                 c.ChurchBody.CountryId == oCurrChuBody.CountryId &&
        //                                 (c.OwnedByChurchBodyId == oCurrChuBody.Id ||
        //                                 (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
        //                                 (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
        //                                 )
        //                                .OrderByDescending(c => c.EventTo).ThenByDescending(c => c.EventTo)
        //                                .ToList()
        //                                        .Select(c => new SelectListItem()
        //                                        {
        //                                            Value = c.Id.ToString(),
        //                                            Text = c.Subject + ":- " +
        //                                                                (c.IsFullDay == true ?
        //                                                                    (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
        //                                                                    (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
        //                                                                    (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
        //                                                                    (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
        //                                                                 )
        //                                        })
        //                                        .OrderBy(c => c.Text)
        //                                        .ToList();
        //    oAttend.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });

        //    oAttend.oChurchBody = oRequestedChurchBody;
        //    oAttend.oLoggedChurchBody = oCurrChuBody;

        //    ViewBag.promptUserMsg = "";
        //    TempData.Keep();
        //    return View(oAttend);
        //}


        [HttpGet]
        public IActionResult Index_Attendees_MemEdit(int currAttendTask = 2, int? reqChurchBodyId = null, string strLongevity = "C", int? id = null, bool loadLim = false) //ONLY members... or History (H), char? filterIndex = null, int? filterVal = null)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


            if (!loadLim)
                _ = this.LoadClientDashboardValues();// this._clientDBConnString);


            var oAppGloOwnId = this._oLoggedAGO.Id;
            var oCurrChuBodyId = reqChurchBodyId;

            //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

            ChurchBody oCB = this._oLoggedCB;
            if (oCurrChuBodyId != this._oLoggedCB.Id)
                oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


            var oAttendModel = new ChurchAttendanceModel();
            oAttendModel.strAttnLongevity = strLongevity;  // today (C), history (H) 
            oAttendModel.strCurrTaskDesc = oAttendModel.strAttnLongevity == "H" ? "Member Attendance History" : "Member Attendance Log (Today)";

            //   ViewBag.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs

            // List<ChurchMember> qry = new List<ChurchMember>();

            // editing
            var ev = _context.ChurchCalendarEvent.Where(c => c.ChurchBodyId == oCurrChuBodyId &&
               c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
            var ev_Id = ev != null ? ev.Id : (int?)null;
            oAttendModel.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

            if (id == null) oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEdit(oCB, ev_Id, oAttendModel.f_DateAttended);
            else oAttendModel.lsChurchAttendanceModels_MultiEdit = ReturnChurchAttendeesList_MemEditById(oCB, (int)id);

            //lookups
            var oCCEList1 = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP)
                                    .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCB.AppGlobalOwnerId &&
                                         c.ChurchBody.CtryAlpha3Code == oCB.CtryAlpha3Code).ToList(); // &&

            oAttendModel.lkpChuCalEvents = oCCEList1.Where( c=> 
                                         (c.ChurchBodyId == oCurrChuBodyId ||
                                         (c.ChurchBodyId != oCurrChuBodyId && c.SharingStatus == "C" && c.ChurchBodyId == oCB.ParentChurchBodyId) ||
                                         (c.ChurchBodyId != oCurrChuBodyId && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCB)))
                                         )
                                        .OrderByDescending(c => c.EventTo).ThenByDescending(c => c.EventTo)
                                        .ToList()
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.Subject + ":- " +
                                                                        (c.IsFullDay == true ?
                                                                            (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                            (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                            (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                                                                            (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                         )
                                                })
                                                .OrderBy(c => c.Text)
                                                .ToList();
            oAttendModel.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            //
            // var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();

            var arrData = "";  
            arrData = TempData.ContainsKey("oVmCurr") ? TempData["oVmCurr"] as string : arrData;
            var savedAttendVM = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData) : null;
             

            if (savedAttendVM == null) { savedAttendVM.lsChurchAttendanceModels = ReturnChurchAttendeesList(oAppGloOwnId, oCurrChuBodyId, "C"); }    /// strLongevity   //only members ... visitors will use the modal form

            var attendList = savedAttendVM.lsChurchAttendanceModels;
            //
            var maleCount = attendList.ToList().Where(c => c.oChurchAttendee.Gender == "M").Count();   //oAttend.Result.ToList().OfType<MemberProfile>()
            var femaleCount = attendList.Where(c => c.oChurchAttendee.Gender == "F").Count();
            var newCount = attendList.Where(c => (DateTime)c.oChurchAttendee.Created.Value.Date == DateTime.Now.Date).Count();
            oAttendModel.CHCF_TotAttend_MemOrVis += attendList.Count();
            // oAttend.CHCF_TotNewMem += newCount;
            oAttendModel.CHCF_TotAttend_M += maleCount;
            oAttendModel.CHCF_TotAttend_F += femaleCount;
            oAttendModel.CHCF_TotAttend_O += attendList.Count - maleCount - femaleCount;
            oAttendModel.lsChurchAttendanceModels = attendList;
            //
            oAttendModel.currAttendTask = currAttendTask;
            oAttendModel.oChurchBody = oCB;
            oAttendModel.oChurchBodyId = oCurrChuBodyId; // oRequestedChurchBody.Id;
            oAttendModel.oLoggedChurchBody = oCB;

            ViewBag.promptUserMsg = "";
            TempData.Keep(); 



            return View(oAttendModel);
        }


        [HttpGet]
        public IActionResult AddOrEdit_Attendees_MemEdit(int? reqChurchBodyId = null, string strLongevity = "C", bool loadLim = false) // or History (H), char? filterIndex = null, int? filterVal = null)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


            if (!loadLim)
                _ = this.LoadClientDashboardValues();  /// this._clientDBConnString);


            var oAppGloOwnId = this._oLoggedAGO.Id;
            var oCurrChuBodyId = reqChurchBodyId;

            //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

            ChurchBody oCB = this._oLoggedCB;
            if (oCurrChuBodyId != this._oLoggedCB.Id)
                oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();

            var oAttend = new ChurchAttendanceModel();

            oAttend.strAttnLongevity = strLongevity;  // today (C), history (H) 
            oAttend.strCurrTaskDesc = oAttend.strAttnLongevity == "H" ? "Member Attendance History" : "Member Attendance Log (Today)";

            //   ViewBag.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs

            // List<ChurchMember> qry = new List<ChurchMember>();
            var ev = _context.ChurchCalendarEvent.Where(c => c.ChurchBodyId == oCurrChuBodyId &&
               c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
            var ev_Id = ev != null ? ev.Id : (int?)null;
            oAttend.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

            oAttend.lsChurchAttendanceModels = ReturnChurchAttendeesList_MemEdit(oCB, ev_Id, oAttend.f_DateAttended);
            //lookups
            oAttend.lkpChuCalEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP)
                                    .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCB.AppGlobalOwnerId &&
                                         c.ChurchBody.CtryAlpha3Code == oCB.CtryAlpha3Code &&
                                         (c.ChurchBodyId == oCurrChuBodyId ||
                                         (c.ChurchBodyId != oCurrChuBodyId && c.SharingStatus == "C" && c.ChurchBodyId == oCB.ParentChurchBodyId) ||
                                         (c.ChurchBodyId != oCurrChuBodyId && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCB)))
                                         )
                                        .OrderByDescending(c => c.EventTo).ThenByDescending(c => c.EventTo)
                                        .ToList()
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.Subject + ":- " +
                                                                        (c.IsFullDay == true ?
                                                                            (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                            (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                            (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                                                                            (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                         )
                                                })
                                                .OrderBy(c => c.Text)
                                                .ToList();
            oAttend.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            oAttend.oChurchBody = oCB;
            oAttend.oLoggedChurchBody = oCB;

            ViewBag.promptUserMsg = "";
            TempData.Keep();

            return PartialView("_AddOrEdit_Attendees_MemEdit", oAttend); //View(lsChurchAttendanceModels); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index_Attendees(ChurchAttendanceModel vmMod, IFormCollection f) //ChurchMemAttendList oList)      
                                                                                                      // public IActionResult Index_Attendees(ChurchMemAttendList oList) //List<ChurchMember> oList)  //, int? reqChurchBodyId = null, string strAttendee="M", string strLongevity="C" ) //, char? filterIndex = null, int? filterVal = null)

        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            // return View(vmMod);

            if (ModelState.IsValid == false)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Saving data failed. Please refresh and try again." });

            if (vmMod == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Data to update not found. Please refresh and try again" });


            if (vmMod.lsChurchAttendanceModels_MultiEdit.Count == 0)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "No changes made to attendance data." });

            //get the global var
            var oCbId = f["oChurchBodyId"].ToString();
            var oDt = f["f_DateAttended"].ToString();
            var oEv = f["f_ChurchEventId"].ToString();

            if (oCbId == null && oDt == null & oEv == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Congregation, Church event and date attended required" });

            var oCBId = int.Parse(oCbId);
            var dtEv = DateTime.Parse(oDt);
            var oEvId = int.Parse(oEv);

            var atLeastOneChecked = false;
            foreach (var d in vmMod.lsChurchAttendanceModels_MultiEdit)
            {
                if (d.f_ChkMemAttend == true || (d.f_ChkMemAttend == false && d.oAttend_Id > 0)) // new attend data
                {   //consider multiple sessions per activity/event  ... ignore the time

                    if (d.oAttend_Id > 0)
                    {
                        var oCA = _context.ChurchAttendAttendee.Where(c => c.ChurchBodyId == d.oChurchBodyId && c.Id == d.oAttend_Id).FirstOrDefault();
                        if (oCA != null)
                        {
                            if (d.f_ChkMemAttend == true)
                            {
                                oCA.ChurchBodyId = oCBId; //vmMod.oChurchBodyId;                               
                                oCA.DateAttended = dtEv; //vmMod.f_DateAttended;
                                oCA.ChurchEventDetailId = oEvId; // vmMod.f_ChurchEventId;
                                oCA.ChurchMemberId = d.oChurchMemberId;
                                oCA.TempCelc = d.decTempRec;
                                oCA.PersKgWt = d.decPersWt;
                                oCA.PersBPMin = d.decPersBPMin;
                                oCA.PersBPMax = d.decPersBPMax;

                                // oCA.AttendeeType = "C";
                                //  oCA = d.oChurchAttend;
                                oCA.LastMod = DateTime.Now;

                                _context.Update(oCA);
                            }
                            else
                            {
                                _context.Remove(oCA);  //.ChurchAttendAttendee.
                            }

                            if (atLeastOneChecked == false) atLeastOneChecked = true;
                        }
                    }
                    else //if (d.oAttend_Id > 0) //update
                    {
                        var oCAExist = _context.ChurchAttendAttendee.Where(c => c.ChurchBodyId == oCBId && c.ChurchEventDetailId == oEvId && c.DateAttended.Value.ToShortDateString() == dtEv.ToShortDateString() &&
                                                c.ChurchMemberId == d.oChurchMemberId).FirstOrDefault();
                        if (oCAExist == null)
                        {
                            var oCA = new ChurchAttendAttendee()
                            {
                                ChurchBodyId = oCBId, //f.TryGetValue("oChurchBodyId", f["oChurchBodyId"]),  //  vmMod.oChurchBodyId, , // 
                                DateAttended = dtEv, //vmMod.f_DateAttended,
                                ChurchEventDetailId = oEvId, // vmMod.f_ChurchEventId,
                                ChurchMemberId = d.oChurchMemberId,
                                AttendeeType = "C", 
                                TempCelc = d.decTempRec, //decimal.Parse(d.strTempRec),
                                PersKgWt = null,
                                PersBPMax = null,
                                PersBPMin = null,
                                Created = DateTime.Now,
                                LastMod = DateTime.Now
                            };

                            _context.Add(oCA);
                            if (atLeastOneChecked == false) atLeastOneChecked = true;
                        }
                    }
                }
            }

            if (atLeastOneChecked == true)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    // ViewBag.promptUserMsg = "Member attendance recorded /updated successfully";

                    var userMessage = "Member attendance updated successfully.";
                    return Json(new { taskSuccess = true, userMess = userMessage, currTask = vmMod.currAttendTask, oCurrId = -1, evtId = oEvId, evtDate = dtEv });

                    //  ViewBag.u_Message = "Member attendance updated successfully";
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

           // var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();

            var arrData = "";
            arrData = TempData.ContainsKey("oVmCurr") ? TempData["oVmCurr"] as string : arrData;
            var savedAttendVM = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData) : null;

            if (savedAttendVM != null) savedAttendVM.lsChurchAttendanceModels_MultiEdit = vmMod.lsChurchAttendanceModels_MultiEdit;


            //var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oAttendModel);
            //TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();


            return View(savedAttendVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index_Headcount(ChurchAttendanceModel vmMod, IFormCollection f) //ChurchMemAttendList oList)      
                                                                                                      // public IActionResult Index_Attendees(ChurchMemAttendList oList) //List<ChurchMember> oList)  //, int? reqChurchBodyId = null, string strAttendee="M", string strLongevity="C" ) //, char? filterIndex = null, int? filterVal = null)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            // return View(vmMod);

            if (ModelState.IsValid == false)
                return Json(new { taskSuccess = false, userMess = "Saving data failed. Please refresh and try again.", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });

            if (vmMod == null)
                return Json(new { taskSuccess = false, userMess = "Data to update not found. Please refresh and try again", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });

            if (vmMod.lsChurchAttendanceModels_HCEdit == null)
                return Json(new { taskSuccess = false, userMess = "No changes made to attendance data.", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });

            if (vmMod.lsChurchAttendanceModels_HCEdit.Count == 0)
                return Json(new { taskSuccess = false, userMess = "No changes made to attendance data", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });

            //get the global var
            var oCbId = f["oChurchBodyId"].ToString();
            var oDt = f["f_DateAttended"].ToString();
            var oEv = f["f_ChurchEventId"].ToString();

            if (oCbId == null && oDt == null & oEv == null)
                return Json(new { taskSuccess = false, userMess = "Congregation, Church event and date attended required.", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = -1, evtDate = -1 });


            var oCBId = int.Parse(oCbId);
            var dtEv = DateTime.Parse(oDt);
            var oEvId = int.Parse(oEv);

            var atLeastOneChecked = false;
            foreach (var d in vmMod.lsChurchAttendanceModels_HCEdit)
            {
                // if (d.f_ChkMemAttend == true || (d.f_ChkMemAttend == false && d.oAttend_Id > 0)) // new attend data
                //{   //consider multiple sessions per activity/event  ... ignore the time

                if (d.oAttend_Id > 0)
                {
                    if (d.CountCategoryId != null)
                    {
                        var oCA = _context.ChurchAttendHeadCount.Where(c => c.ChurchBodyId == d.oChurchBodyId && c.Id == d.oAttend_Id).FirstOrDefault();
                        if (oCA != null)
                        {   //add more conditions... this not enough ... ids jux integers... they can be same by chance
                            var isCongCount = oCBId == d.CountCategoryId; // _context.ChurchBody.Where(c => c.AppGlobalOwnerId == vmMod.oChurchBody.AppGlobalOwnerId && c.OrgType == "CN" && c.Id == d.CountCategoryId).Count() > 0;

                            oCA.ChurchBodyId = oCBId; //f.TryGetValue("oChurchBodyId", f["oChurchBodyId"]),  //  vmMod.oChurchBodyId, , // 
                            oCA.CountDate = dtEv; //vmMod.f_DateAttended,
                            oCA.ChurchEventId = oEvId; // vmMod.f_ChurchEventId,
                            oCA.ChurchUnitId = d.CountCategoryId.ToString() != "-1" ? d.CountCategoryId : null;
                            oCA.CountType = isCongCount == true ? "C" : "G";  // d.strCountCategory.ToLower() == "Congregants".ToLower() ? "C" : "G";
                            oCA.TotM = d.CHCF_TotAttend_M;
                            oCA.TotF = d.CHCF_TotAttend_F;
                            oCA.TotO = d.CHCF_TotAttend_O;
                            oCA.TotCount = d.CHCF_TotAttend_M + d.CHCF_TotAttend_F + d.CHCF_TotAttend_O;
                            // oCA.Created = DateTime.Now;
                            oCA.LastMod = DateTime.Now;

                            _context.Update(oCA);
                            atLeastOneChecked = true;
                        }
                    }
                }
                else //if (d.oAttend_Id > 0) //update
                {
                    if (d.CountCategoryId != null)
                    {
                        //var oCAExist = _context.ChurchAttend_HeadCount.Where(c => c.ChurchBodyId == d.oChurchBodyId && c.ChurchEventId == oEvId && c.CountDate.Value.ToShortDateString() == dtEv.ToShortDateString() &&
                        //                              c.CountType == d.strCountTypeCode && c.ChurchGroupChurchBodyId == d.CountCategoryId).FirstOrDefault();
                        //if (oCAExist == null)
                        //{
                        var isCongCount = oCBId == d.CountCategoryId; //_context.ChurchBody.Where(c => c.AppGlobalOwnerId == oCBId && c.OrgType == "CN" && c.Id == d.CountCategoryId).Count() > 0;

                        var oCA = new ChurchAttendHeadCount()
                        {
                            ChurchBodyId = oCBId, //f.TryGetValue("oChurchBodyId", f["oChurchBodyId"]),  //  vmMod.oChurchBodyId, , // 
                            CountDate = dtEv, //vmMod.f_DateAttended,
                            ChurchEventId = oEvId, // vmMod.f_ChurchEventId,
                            ChurchUnitId = d.CountCategoryId.ToString() != "-1" ? d.CountCategoryId : null,
                        // ChurchGroupChurchBodyId = d.CountCategoryId.ToString() != "-1" ? d.CountCategoryId : null,
                            CountType = isCongCount == true ? "C" : "G", // d.strCountCategory.ToLower() == "Congregants".ToLower() ? "C" : "G",
                            TotM = d.CHCF_TotAttend_M,
                            TotF = d.CHCF_TotAttend_F,
                            TotO = d.CHCF_TotAttend_O,
                            TotCount = d.CHCF_TotAttend_M + d.CHCF_TotAttend_F + d.CHCF_TotAttend_O,
                            Created = DateTime.Now,
                            LastMod = DateTime.Now
                        };

                        _context.Add(oCA);
                        atLeastOneChecked = true;
                        // }
                    }
                }
                // }
            }

            if (atLeastOneChecked == true)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    // ViewBag.promptUserMsg = "Member attendance recorded /updated successfully";

                    var userMessage = "Member attendance headcount updated successfully.";
                    return Json(new { taskSuccess = true, userMess = userMessage, currTask = vmMod.currAttendTask, oCurrId = -1, evtId = oEvId, evtDate = dtEv });

                    //  ViewBag.u_Message = "Member attendance updated successfully";
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            // var savedAttendVM = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
            // if (savedAttendVM != null) savedAttendVM.lsChurchAttendanceModels_HCEdit = vmMod.lsChurchAttendanceModels_HCEdit;

            //  return View(savedAttendVM); 
            return Json(new { taskSuccess = false, userMess = "Saving data failed. Please refresh and try again.", currTask = vmMod.currAttendTask, oCurrId = -1, evtId = oEvId, evtDate = dtEv });
        }


        [HttpGet]
        public IActionResult AddFrom_AttendeesItem(int? oCurrChuBodyId, int currAttendId)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            var oAttendModel = new ChurchAttendanceModel();
            var oCurrChuBody = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();
            if (oCurrChuBody == null) { oAttendModel.oChurchAttendee = new ChurchAttendAttendee(); return PartialView("_AddOrEdit_Attendees", new ChurchAttendanceModel()); }

            oAttendModel = ReturnChurchAttendeesList_MemEditById(oCurrChuBody, currAttendId).FirstOrDefault();
            if (oAttendModel == null) { oAttendModel.oChurchAttendee = new ChurchAttendAttendee(); return PartialView("_AddOrEdit_Attendees", new ChurchAttendanceModel()); }
            if (oAttendModel.oChurchAttendee == null) { oAttendModel.oChurchAttendee = new ChurchAttendAttendee(); return PartialView("_AddOrEdit_Attendees", new ChurchAttendanceModel()); }

            var oAttend = oAttendModel.oChurchAttendee;
            oAttendModel.oChurchAttendee = new ChurchAttendAttendee()
            {
                ChurchBodyId = oAttend.ChurchBodyId,
                ChurchBody = oAttend.ChurchBody,
                DateAttended = DateTime.Now,
                ChurchEventDetailId = null,
                ChurchEventDetail = null,
                AttendEventDesc = "",
                AttendeeType = "V",

                TempCelc = 0,
                PersKgWt = 0,
                PersBPMin = 0,
                PersBPMax = 0,

                ChurchMemberId = null,
                ChurchMember = null,
                Title = oAttend.Title,
                FirstName = oAttend.FirstName,
                MiddleName = oAttend.MiddleName,
                LastName = oAttend.LastName,
                Gender = oAttend.Gender,
               
                //MaritalStatus = oAttend.MaritalStatus,
                AgeBracketId = oAttend.AgeBracketId,
                AgeBracket = oAttend.AgeBracket,
                NationalityId = oAttend.NationalityId,
                Nationality = oAttend.Nationality,
                ResidenceLoc = oAttend.ResidenceLoc,
                MobilePhone = oAttend.MobilePhone,
                Email = oAttend.Email,
               // VisitReasonId = null,
                VisitReason = null,
                //VisitReason_Other = "",
                Notes = "",
            };

            oAttendModel.oChurchBody = oCurrChuBody;  //current working CB
            //oAttendModel.oLoggedChurchBody = oCurrChuBody_Logged;
            oAttendModel.strAttnLongevity = "C";  // today (C), history (H)   
            oAttendModel.f_strAttendeeTypeCode = "V";  //else history

            oAttendModel.lsChurchAttendanceModels_AttendFilter = new List<ChurchAttendanceModel>();
            if (oAttendModel == null) return PartialView("_AddOrEdit_Attendees", oAttendModel);

            oAttendModel = populateLookups_Attend(oAttendModel, oCurrChuBody, oAttendModel.oChurchAttendee.DateAttended);//, oCurrVmMod.oChurchAttend.Id);

            var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oAttendModel);
            TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();


            //TempData.Put("oVmCurrMod", _vmMod);
            //TempData.Keep();

            return PartialView("_AddOrEdit_Attendees", oAttendModel);
        }

        public IActionResult AddOrEdit_AttendHistory(int? oChurchBodyId, string strAttendee, string strLongevity, string strFirstName, string strMiddleName, string strLastName, string strAttendLoc, string strAttendPho) 
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


            var oChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oChurchBodyId).FirstOrDefault();
            var oAttendList = ReturnChurchAttendeesListByFilters(oChurchBody, strAttendee, strLongevity, strFirstName, strMiddleName, strLastName, strAttendLoc, strAttendPho);

            return PartialView("_vwAttendPreviousVisit", oAttendList);
        }



        /// church event cud be at any level -- local, district, ... HQ  :::  oCurrChuBodyId is the < current CB >
        public JsonResult GetChurchEventsByAttendDate(int oCBid, string strDate1, string strDate2, int? oEvCBid = null, int? oCLid = null, bool addEmpty = false)
        {
            var eventList = new List<SelectListItem>();

            try
            {
                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return null; //// View("_ErrorPage");
                    }
                }


                var oAppGloOwnId = this._oLoggedUser?.AppGlobalOwnerId;

                // var dtAttendMin = DateTime.Parse(strDate1);
                var dtDateMin = DateTime.Parse(strDate1);
                var dtDateMax = DateTime.Parse(strDate2);

                var oCBList = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId==oAppGloOwnId && c.Id == oCBid || c.Id == oEvCBid).ToList();
                var oCurrChuBody = oCBList.Where(c => c.Id == oCBid).FirstOrDefault();  
                if (oCurrChuBody == null)
                {
                    if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
                    return Json(eventList);
                }

                // var oEventCBid = oCurrChuBody.Id;
                 
                /// EvCB specified...
                if (oEvCBid != null)
                {
                    var oEvCB = oCBList.Where(c => c.Id == oEvCBid).FirstOrDefault();
                    if (oEvCB != null && oCLid  != null)
                        if (oEvCB.ChurchLevelId != oCLid) oEvCBid = null;  /// force passed CL to be used...
                }

                /// else pick from the CB tree...   // confirm the CL
                if (oEvCBid == null && oCLid != null)
                {
                    /// diff CL events requested --- perhaps the parent CB events
                    oEvCBid = oCBid;
                    if (oCurrChuBody.ChurchLevelId != oCLid)
                    {
                        var oCBList_1 = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();
                        var oEventCB = oCBList_1.Where(c => oCurrChuBody.RootChurchCode.Contains(c.RootChurchCode) && c.ChurchLevelId == oCLid).FirstOrDefault();

                        if (oEventCB != null) oEvCBid = oEventCB.Id;
                    }
                }


                ///
                /// date must be [ when event was organised] or [today! ...now happening ]
                // var _tm = DateTime.Now.Date;
                /// var dtv = dtAttend != null ? ((DateTime)dtAttend).Date : _tm;

                if (dtDateMin != null) dtDateMin = dtDateMin.Date;
                if (dtDateMax != null) dtDateMax = dtDateMax.Date;
                var eventList_1 = _context.ChurchCalendarEventDetail.Include(t => t.ChurchCalendarEvent)
                                 .Where(c => c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.IsEventActive == true && c.ChurchBodyId == oEvCBid && 
                               ////  (c.EventFrom.Value != null ? c.EventFrom.Value.Date : (DateTime?)null) <= _tm  &&    /// ... validate @ saving ... query can be historical
                                      (
                                        (dtDateMin != null && dtDateMax != null && (c.EventFrom.Value != null ? c.EventFrom.Value.Date : (DateTime?)null) >= dtDateMin && (c.EventFrom.Value != null ? c.EventFrom.Value.Date : (DateTime?)null) <= dtDateMax) ||
                                        ((dtDateMin == null || dtDateMax == null) && (c.EventFrom.Value != null ? c.EventFrom.Value.Date : (DateTime?)null) >= dtDateMin || (c.EventFrom.Value != null ? c.EventFrom.Value.Date : (DateTime?)null) <= dtDateMax)
                                        //(c.EventFrom.Value != null ? c.EventFrom.Value.Date : (DateTime?)null) == dtv
                                      )

                                          /// c.EventFrom <= dtv && c.EventTo >= dtv

                                    // c.ChurchBody.CountryId == oCurrChuBody.CountryId && c.EventFrom.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt  &&
                                    ///((DateTime)c.EventFrom.Value).Date == dtv
                                    )
                                 /// .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
                                  .ToList();

                /// also get the events of higher CBs + local CB
                /// 
                //eventList_1 = eventList_1.Where(c =>
                //    (c.ChurchBodyId == oCurrChuBody.Id ||
                //    (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                //    (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)))
                //      )
                //    .ToList();

                /// 
                /// no inheriting of events necessary here!
                //// filter ...
                //if (inclExtCBData == true)
                //{
                //    eventList_1 = eventList_1.Where(c =>
                //          (c.ChurchBodyId == oCurrChuBody.Id ||
                //          (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.ChurchBodyId == oCurrChuBody.ParentChurchBodyId) || // share with child
                //          (c.ChurchBodyId != oCurrChuBody.Id && (c.SharingStatus == "D" || c.SharingStatus == "R") && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCurrChuBody)) ||  /// share with descendant | same route
                //                          (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "P" && c.ChurchBody.ParentChurchBodyId == oCurrChuBody.Id) ||  /// share with parent
                //                          (c.ChurchBodyId != oCurrChuBody.Id && (c.SharingStatus == "H" || c.SharingStatus == "R") && AppUtilties.IsDescendant_ChurchBody(c.ChurchBody, oCurrChuBody)) ||  /// share with ancestor (up-High) | same route
                //                          (c.ChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId)  // share with all
                //          ))
                //        .ToList();
                //}

                eventList = eventList_1 
                                     .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
                                     .ToList()
                                             .Select(c => new SelectListItem()
                                             {
                                                 Value = c.Id.ToString(),
                                                 Text = c.EventDescription + (c.EventSessionCode != null ? ": " + (!string.IsNullOrEmpty(c.CustomSessionDesc) ? "" : GetEventSessionDesc(c.EventSessionCode)) : "")
                                                                           + (c.EventFrom != null ? " - " + String.Format("{0:ddd, d-MMM-yyyy}", c.EventFrom) : "")
                                             })
                                             //.OrderBy(c => c.Text)
                                             .ToList();

                if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
                return Json(eventList);

            }

            catch (Exception ex)
            {
                if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
                return Json(eventList);
            } 
        }



        public IActionResult AddOrEditAttendCheckinOne_CAA(int id = 0, int? oCBid = null, string strAttnType = "C", int? oAttendPersRefId = null,
                                                    int? oEvCLid = null, int? eventId = null, DateTime? attendDate = null, bool dbload = true) //, string vStatus = "A")
        {  /// id === attend.id [new or exist], refid === member/memid or visitor/attend.id  ... visitor (V), guest (G), congregant (C) -- member, new convert, excomm, affiliate
           ///string strLongevity = "C", 
            try
            {

                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return null; //// View("_ErrorPage");
                    }
                }

                //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


                //if (!loadLim)
                //    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                var oAppGloOwnId = this._oLoggedAGO.Id;
                // var oCurrChuBodyId = reqChurchBodyId;

                //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCBid).FirstOrDefault();

                //var arrData = ""; // TempData["oVmCurrMod"] as string;
                //arrData = TempData.ContainsKey("oVmCurr") ? TempData["oVmCurr"] as string : arrData;
                //var oCurrVmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData) : null;

                //// var oCurrVmMod = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
                //if (oCurrVmMod == null) oCurrVmMod = new ChurchAttendanceModel();
                ////

                ChurchAttendanceModel oCurrVmMod = null;

                attendDate = attendDate != null ? attendDate.Value.Date : DateTime.Now.Date;

                var oEvCBid = oCBid;
                /// confirm the CL 
                if (oEvCLid != null)
                {
                    /// diff CL events requested --- perhaps the parent CB events
                    if (oCB.ChurchLevelId != oEvCLid)
                    {
                        var oCBList_1 = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();
                        var oEvCB = oCBList_1.Where(c => oCB.RootChurchCode.Contains(c.RootChurchCode) && c.ChurchLevelId == oEvCLid).FirstOrDefault();

                        if (oEvCB != null) {                             
                            oEvCBid = oEvCB.Id;
                            oEvCLid = oEvCB.ChurchLevelId; 
                        }
                    }
                }


                if (id == 0)
                {
                    //create user and init...
                    oCurrVmMod = new ChurchAttendanceModel();
                    oCurrVmMod.oChurchAttendee = new ChurchAttendAttendee();
                    oCurrVmMod.oChurchAttendee.ChurchBodyId = oCBid; // (int)oCurrChuBody.Id;

                    // var currCnt = _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == (int)oCurrChuBody.Id).Count() + 1;
                    // var strLocMemCode = (!string.IsNullOrEmpty(oCurrChuBody.GlobalChurchCode) ? oCurrChuBody.GlobalChurchCode + "-" : "") + currCnt.ToString();  //add preceding zero's

                    oCurrVmMod.oChurchAttendee.AttendeeType = strAttnType; /// (C)ember , (V)isitor   , (G)uest    
                    oCurrVmMod.oChurchAttendee.ChurchEventDetailId = eventId;
                    oCurrVmMod.oChurchAttendee.DateAttended = attendDate;/// DateTime.Now;

                   // oCurrVmMod.f_strAttendeeTypeCode = strAttnType;
                   /// oCurrVmMod.strAttendeeName = strAttnType == "C" ? "Congregant" : strAttnType == "G" ? "Guest" : strAttnType == "V" ? "Visitor" : "[Attendee: N/A]" ;
                   // oCurrVmMod.f_ChurchEventDetailId = eventId;
                   // oCurrVmMod.f_DateAttended = oCurrVmMod.oChurchAttendee.DateAttended;

                    oCurrVmMod.oEventCLId = oEvCLid; // oCurrVmMod.oChurchAttendee.DateAttended;
                    oCurrVmMod.oEventCBId = oEvCBid; // oCurrVmMod.oChurchAttendee.DateAttended;

                    //oCurrVmMod.oChurchAttendee.TempCelc = (decimal)36.5;    
                    //oCurrVmMod.oChurchAttendee.PersKgWt = 65;    
                    //oCurrVmMod.oChurchAttendee.PersBPMin = 70;    
                    //oCurrVmMod.oChurchAttendee.PersBPMax = 110;    


                    /// get event details
                    //var dtv = oAttendModel.f_DateAttended.HasValue == true ? ((DateTime)oAttendModel.f_DateAttended).Date : (DateTime?)null;
                    var eventItem = _context.ChurchCalendarEvent  /// .Include(t => t.ChurchBody).Include(t => t.ChurchlifeActivity_NVP)
                          .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == eventId).FirstOrDefault();

                    if (eventItem != null)
                    {
                        oCurrVmMod.strChurchEventDesc = eventItem.Subject + (attendDate != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", attendDate) : "");
                    }


                    /// if member --- get member details
                    /// 
                    if (oAttendPersRefId != null && strAttnType == "C")
                    {
                        var oCMList_1 = GetMemBioSnapshotList(oAppGloOwnId, oCBid, oAttendPersRefId);
                        var oCAA_Mem = oCMList_1.FirstOrDefault();
                        oCurrVmMod.oChurchMember = oCAA_Mem.oChurchMember;

                        //var oCAA_Mem = _context.ChurchMember.AsNoTracking().Include(t=> t.ContactInfo)
                        //    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCBid && c.Id == oAttendPersRefId).FirstOrDefault();

                        if (oCAA_Mem != null)
                        { 
                            oCurrVmMod.oChurchAttendee.ChurchMemberId = oAttendPersRefId; /// oCAA_Mem.Id;
                            oCurrVmMod.strAttendeeName = oCAA_Mem.strMemFullName; /// AppUtilties.GetConcatMemberName(oCAA_Mem.Title, oCAA_Mem.FirstName, oCAA_Mem.MiddleName, oCAA_Mem.LastName, false);
                            oCurrVmMod.strAttendeeNameExtnd = oCAA_Mem.strMemFullNameExtnd;   /// name + code
                            ///
                            oCurrVmMod.strGender = oCAA_Mem.strGender;  ///  oCAA_Mem.Nationality != null ? oCAA_Mem.Nationality.EngName : "";
                           /// oCurrVmMod.strGlobalMemberCode = oCAA_Mem.strGlobalMemberCode;  ///  oCAA_Mem.Nationality != null ? oCAA_Mem.Nationality.EngName : "";
                            oCurrVmMod.strNationality = oCAA_Mem.strNationality;  ///  oCAA_Mem.Nationality != null ? oCAA_Mem.Nationality.EngName : "";
                            oCurrVmMod.strPhotoUrl = oCAA_Mem.strPhotoUrl;  ///  oCAA_Mem.PhotoUrl;
                            oCurrVmMod.strResidenceLoc = oCAA_Mem.strLocation;  ///  oCAA_Mem.ContactInfo != null ? oCAA_Mem.ContactInfo.Location : "";
                            oCurrVmMod.strPhone = oCAA_Mem.strPhone;  ///  oCAA_Mem.ContactInfo != null ? oCAA_Mem.ContactInfo.MobilePhone1 : "";
                            oCurrVmMod.strDateAttended = oCurrVmMod.oChurchAttendee.DateAttended != null ? String.Format("{0:ddd, d-MMM-yyyy}", oCurrVmMod.oChurchAttendee.DateAttended) : "";
                            ///  
                            oCurrVmMod.strMemGeneralStatus = oCAA_Mem.strMemGeneralStatus;

                                //string.Compare((t_mt != null ? t_mt.MemberTypeCode : null), "C", true) != 0 ?
                                //                (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned") :
                                //                (!string.IsNullOrEmpty(t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : null) : null) ?
                                //                (t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : "Unassigned") : "Unassigned") :
                                //                (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned"));

                            /// oCurrVmMod.f_strAttendeeTypeCode = "C";
                            /// oCurrVmMod.strAttendeeName = "Congregant";  /// member, new convert, excomm or affiliate 

                        }
                    }

                    else if (oAttendPersRefId != null && (strAttnType == "V" || strAttnType == "G"))
                    {
                        /// if guest/visitor and not first timer... 
                        /// 
                        var oCAA_Attend = _context.ChurchAttendAttendee.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCBid && c.Id == oAttendPersRefId).FirstOrDefault();
                        if (oCAA_Attend != null)
                        {
                            oCurrVmMod.oChurchAttendee.ChurchMemberId = oCAA_Attend.Id;
                            oCurrVmMod.strAttendeeName = AppUtilties.GetConcatMemberName(oCAA_Attend.Title, oCAA_Attend.FirstName, oCAA_Attend.MiddleName, oCAA_Attend.LastName, false);
                           /// oCurrVmMod.f_strAttendeeTypeCode = "V";  /// cud be guest (G)
                           ///  oCurrVmMod.strAttendeeName = "Visitor";   
                        }
                    }

                   

                  ///  oCurrVmMod.f_strAttendeeTypeCode = t_caa.AttendeeType;

                    //oCurrVmMod.dtFilter_AttendFrom = DateTime.Now;
                    //oCurrVmMod.dtFilter_AttendTo = DateTime.Now;
                    
                    ///
                    oCurrVmMod.oAttendRefId = oAttendPersRefId;

                    oCurrVmMod.oChurchBody = oCB;
                    oCurrVmMod.oLoggedChurchBody = oCB;
                }

                else
                {
                    //fetch mem details
                    dbload = true;
                    if (dbload)
                    {
                        var oChurchAttendVM = (
                                     from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality)
                                                    .Where(x => x.AppGlobalOwnerId == oAppGloOwnId && x.ChurchBodyId == oCBid && x.Id == id) // && x.AttendeeType == strAttnType)
                                     join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                                     from t_cb in _tcb
                                     join t_cmx in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo) on t_caa.ChurchMemberId equals t_cmx.Id into _tcm
                                     from t_cm in _tcm.DefaultIfEmpty()
                                     join t_ccedx in _context.ChurchCalendarEventDetail.AsNoTracking().Include(t => t.ChurchBody) .Include(t => t.ChurchCalendarEvent) 
                                                on t_caa.ChurchEventDetailId equals t_ccedx.Id into _cced
                                     from t_cced in _cced ///.DefaultIfEmpty()   --- there MUST always be an event attended!
                                     ///
                                     from t_mt in _context.MemberType.AsNoTracking().Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                               .Take(1).DefaultIfEmpty() 
                                     from t_mr in _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                                   .Take(1).DefaultIfEmpty()   
                                     from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                                   .Take(1).DefaultIfEmpty() // 

                                     select new ChurchAttendanceModel
                                     {
                                         oChurchBody = t_cb,
                                         oChurchBodyId = t_cb.Id,
                                         oLoggedChurchBody = t_cb,
                                         strCongregation = t_cb.Name,

                                         //peers stuff of mem
                                         oChurchAttendee = t_caa, 

                                         oAttendRefId = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.Id : (int?)null) : t_caa.Id,
                                         oChurchMemberId = t_caa.AttendeeType == "C" ? (t_cm != null ? (int?)t_cm.Id : (int?)null) : (int?)null,                                          
                                         strAttendeeName = t_caa.AttendeeType == "C" ? (t_cm != null ? AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false) : "") :
                                                                                AppUtilties.GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false),
                                         strAttendeeTypeDesc = GetMemTypeDesc(t_caa.AttendeeType),
                                         strChurchEventDesc_Hdr = t_cced.ChurchCalendarEvent != null ? (t_cced.ChurchCalendarEvent.Subject + (attendDate != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", attendDate) : "")): "",
                                         strChurchEventDesc = t_cced.EventDescription + (attendDate != null ? ": " + String.Format("{0:ddd, d-MMM-yyyy}", attendDate) : ""),

                                         f_ChurchEventDetailId = t_caa.ChurchEventDetailId,
                                         f_DateAttended = t_caa.DateAttended,
                                         f_strAttendeeTypeCode = t_caa.AttendeeType,
                                         f_oEventCLId = t_cced.ChurchBody != null ? t_cced.ChurchBody.Id : (int?)null,

                                         //strChurchEventDesc = t_cce != null ?
                                         //                   t_cce.Subject + ":- " +
                                         //                                       (t_cce.IsFullDay == true ?
                                         //                                           (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                         //                                           (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                         //                                           (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                         //                                           (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                         //                                        ) : "",

                                         strNationality = t_caa.AttendeeType == "C" ? (t_cm != null ? (t_cm.Nationality != null ? t_cm.Nationality.EngName : "") : "") :
                                                                (t_caa.Nationality != null ? t_caa.Nationality.EngName : ""),
                                         strPhotoUrl = t_caa.AttendeeType == "C" ? (t_cm != null ? t_cm.PhotoUrl : "") : "",


                                         strResidenceLoc = t_caa.AttendeeType == "C" ? (t_cm != null ? (t_cm.ContactInfo != null ? t_cm.ContactInfo.Location : "") : "") :
                                                                 t_caa.ResidenceLoc,
                                         strPhone = t_caa.AttendeeType == "C" ? (t_cm != null ? (t_cm.ContactInfo != null ? t_cm.ContactInfo.MobilePhone1 : "") : "") :
                                                                t_caa.MobilePhone,
                                         strDateAttended = t_caa.DateAttended != null ? String.Format("{0:ddd, d-MMM-yyyy}", t_caa.DateAttended) : "",
                                                               ///  DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : ""
                                                               ///  
                                         strMemGeneralStatus = t_caa.AttendeeType == "C" ? 
                                                             (string.Compare((t_mt != null ? t_mt.MemberTypeCode : null), "C", true) != 0 ?
                                                             (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned") :
                                                             (!string.IsNullOrEmpty(t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : null) : null) ?
                                                             (t_mr != null ? (t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : "Unassigned") : "Unassigned") :
                                                             (t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "Unassigned"))) : "N/A",

                                         decTempRec = t_caa.TempCelc != null ? (decimal)t_caa.TempCelc : (decimal?)null,
                                         strTempRec = t_caa.TempCelc != null ? t_caa.TempCelc.Value.ToString("{0:#.#}") : "",

                                         decPersWt = t_caa.PersKgWt != null ? (decimal)t_caa.PersKgWt : (decimal?)null,
                                         strPersWt = t_caa.PersKgWt != null ? t_caa.PersKgWt.Value.ToString("{0:#.#}") : "",

                                         decPersBPMin = t_caa.PersBPMin != null ? (decimal)t_caa.PersBPMin : (decimal?)null,
                                         strPersBPMin = t_caa.PersBPMin != null ? t_caa.PersBPMin.Value.ToString("{0:#.#}") : "",

                                         decPersBPMax = t_caa.PersBPMax != null ? (decimal)t_caa.PersBPMax : (decimal?)null,
                                         strPersBPMax = t_caa.PersBPMax != null ? t_caa.PersBPMax.Value.ToString("{0:#.#}") : "" 

                                     })
                                     .FirstOrDefault();

                        oCurrVmMod = oChurchAttendVM;
                    }

                    //else     //get the saved... all the member_data get loaded when drilling down on member
                    //{
                    //    // oCurrVmMod = TempData.Get<ChurchAttendanceModel>("oVmCurr");

                    //    var arrData1 = ""; // TempData["oVmCurrMod"] as string;
                    //    arrData1 = TempData.ContainsKey("oVmCurr") ? TempData["oVmCurr"] as string : arrData1;
                    //    oCurrVmMod = (!string.IsNullOrEmpty(arrData1)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData1) : null;
                    //}



                    //if (oCurrVmMod.oChurchAttendee != null)
                    //{
                    //    oCurrVmMod.dtEventFilterDate1 = DateTime.Now;
                    //    oCurrVmMod.dtEventFilterDate2 = DateTime.Now;
                    //}
                }


                /// load totals...
                oCurrVmMod.num_f_CHCF_TotAttend_Yr = 0;
                oCurrVmMod.num_f_CHCF_TotAttend_Sem = 0;
                oCurrVmMod.num_f_CHCF_TotAttend_Qtr = 0;
                oCurrVmMod.num_f_CHCF_TotAttend_Mon = 0;
                oCurrVmMod.num_f_CHCF_TotAttend_Wk = 0;


                ////oCurrVmMod.oChurchBody = oCB;  //current working CB
                ///
                /// oCurrVmMod.oLoggedChurchBody = oCB;
                ////  oCurrVmMod.strAttnLongevity = strLongevity;  // today (C), history (H)
                ///
                ///  oCurrVmMod.f_strAttendeeTypeCode = strAttendee;  //else history


                //oCurrVmMod.lsChurchAttendanceModels_AttendFilter = new List<ChurchAttendanceModel>();

                //if (oCurrVmMod == null) return PartialView("_AddOrEdit_Attendees", oCurrVmMod);

                //  TempData.Put("oCurrChuMemberPrvw", oCurrVmMod.oChurchAttend);
                var _vmMod = populateLookups_Attend(oCurrVmMod, oCB, oCurrVmMod.oChurchAttendee.DateAttended);//, oCurrVmMod.oChurchAttend.Id);
                                                                                                              //TempData.Put("oVmCurrMod", _vmMod);
                                                                                                              //TempData.Keep();

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(_vmMod);
                TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

                
                
                return PartialView("_vwAttendCheckinOne_CAA", _vmMod);   /// 

            }
            catch (Exception ex)
            {
                return PartialView("_vwErrorPage");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditCAA_CheckinOne(ChurchAttendanceModel vm)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            ChurchAttendAttendee _oChangesCAA = vm.oChurchAttendee; //  .oChurchAttend;

            //
            // var oFormFile = vmMod.MemPhotoFile;
            //  vmMod = TempData.Get<ChurchAttendanceModel>("oVmCurrMod"); TempData.Keep();

            var arrData = ""; // TempData["oVmCurrMod"] as string;
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData) : vm;

            var oCV = vmMod.oChurchAttendee;
            oCV.ChurchBody = vmMod.oChurchBody;
            var _tm = DateTime.Now;


            /// check if person already checked in... same [date, session, event, person-mem id or ext pers [name, dob, pho, email]
            /// check attached CB
            if (_oChangesCAA.AppGlobalOwnerId == null) _oChangesCAA.AppGlobalOwnerId = this._oLoggedAGO.Id;
            if (_oChangesCAA.ChurchBodyId == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Congregation hosting (organizing) event for this attendance not specified." });

            var oCM_CB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.Id == _oChangesCAA.ChurchBodyId).FirstOrDefault();
            if (oCM_CB == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Specified congregation for attendance could not be verified. Please refresh and try again.", signOutToLogIn = false });

            if (_oChangesCAA.AttendeeType == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Attendee type must be indicated please. Who's attending the event: Church member, Guest or Visitor ?" });
          
            if (_oChangesCAA.ChurchEventDetailId == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Please event attended is required." });

             if (_oChangesCAA.DateAttended == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Please date of attendance is not specified. Date event attended is required." });

             if (_oChangesCAA.DateAttended.Value.Date > _tm.Date)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Please date of attendance of event specified cannot be later than today." });

            // in case of event sessions... each made an occrence [detail] 
            var oMCAList_Dup_1 = _context.ChurchAttendAttendee.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId && 
                                 c.ChurchEventDetailId == _oChangesCAA.ChurchEventDetailId && (c.DateAttended != null ? c.DateAttended.Value.Date : (DateTime?)null) == _oChangesCAA.DateAttended.Value.Date
                                  ).ToList();

            if (_oChangesCAA.AttendeeType == "C")  //Congregant... ChurcCodes required
            {
                if (_oChangesCAA.ChurchMemberId == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Please specify church member attending event. Hint: You may change the attendee type." });

                var oCAA_Mem = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId && c.Id == _oChangesCAA.ChurchMemberId).FirstOrDefault();
                if (oCAA_Mem == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Church member attendee could not be verfied. Please refresh and try again.", signOutToLogIn = false });

                /// member check-in once...
                ///
                if (_oChangesCAA.Id == 0)
                {
                    var oMCAList_Dup = oMCAList_Dup_1.Where(c => c.ChurchMemberId == _oChangesCAA.ChurchMemberId).ToList();

                    if ( oMCAList_Dup.Count > 0)
                        return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Church member already CHECKED-IN for the event (or session) and date specified." 
                        // AppUtilties.GetConcatMemberName(oCAA_Mem.Title, oCAA_Mem.FirstName, oCAA_Mem.MiddleName, oCAA_Mem.LastName, false) + " already attended the event on specified date ."
                    });
                }
       


                var oMCL_Mem = _context.MemberChurchlife.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId && c.ChurchMemberId == _oChangesCAA.ChurchMemberId).FirstOrDefault();

                if (_oChangesCAA.DateAttended != null)
                {
                    if (oCAA_Mem != null)
                        if (_oChangesCAA.DateAttended.Value < oCAA_Mem.DateOfBirth)
                            return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Attendance date cannot be earlier than member date of birth.", signOutToLogIn = false });

                    if (oMCL_Mem != null)
                    {
                        if (oMCL_Mem.DateJoined != null)
                            if (_oChangesCAA.DateAttended.Value < oMCL_Mem.DateJoined)
                                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Attendance date cannot be earlier than date member enrolled.", signOutToLogIn = false });

                        if (oMCL_Mem.DateDeparted != null)
                            if (_oChangesCAA.DateAttended.Value < oMCL_Mem.DateDeparted)
                                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Attendance date cannot be earlier than date member departed.", signOutToLogIn = false });
                    }
                }

            }

            /// Guest, Visitor....
            else
            {
                if ((string.IsNullOrEmpty(_oChangesCAA.FirstName) || string.IsNullOrEmpty(_oChangesCAA.LastName)))  //Congregant... ChurcCodes required 
                    return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Firstname and last name of visitor required." });

                if (!string.IsNullOrEmpty(_oChangesCAA.Email))
                {
                    //  ... check validity... REGEX 
                    if (!AppUtilties.IsValidEmail(_oChangesCAA.Email))
                        return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Email specified is invalid. Please check and try again.", signOutToLogIn = false });
                }

                var oMCAList_Dup = _context.ChurchAttendAttendee.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId &&
                                     c.ChurchEventDetailId == _oChangesCAA.ChurchEventDetailId && c.DateAttended == _oChangesCAA.DateAttended &&
                                    (c.FirstName + c.MiddleName + c.LastName).Contains(_oChangesCAA.FirstName) && (c.FirstName + c.MiddleName + c.LastName).Contains(_oChangesCAA.MiddleName) &&
                                    (c.FirstName + c.MiddleName + c.LastName).Contains(_oChangesCAA.LastName) && (c.MobilePhone == _oChangesCAA.MobilePhone || c.ResidenceLoc == _oChangesCAA.ResidenceLoc || c.Email == _oChangesCAA.Email)
                                    ).ToList();

                if (oMCAList_Dup.Count > 0)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Visitor, " + AppUtilties.GetConcatMemberName(_oChangesCAA.Title, _oChangesCAA.FirstName, _oChangesCAA.MiddleName, _oChangesCAA.LastName, true) + " is already checked-in for this event for same date specified." });

            }



            if (_oChangesCAA.TempCelc != 0 && _oChangesCAA.TempCelc <= 30 && _oChangesCAA.TempCelc >= 40)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Please check the body temperature again. Doesn't seem to be normal. Hint: Clear value to continue [0]" });

            if (_oChangesCAA.PersBPMin != 0 && _oChangesCAA.PersBPMax != 0 && _oChangesCAA.PersBPMin >= _oChangesCAA.PersBPMax)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Blood pressure values indicated is not correct. Diastolic [2nd] value must be less than the Systolic [1st] value. Hint: Clear values to continue [0]" });

             

            try
            {
                ModelState.Remove("oChurchAttendee.ChurchBodyId");
                ModelState.Remove("oChurchAttendee.ChurchMemberId");
                ModelState.Remove("oChurchAttendee.ChurchEventId");

                if (_oChangesCAA.AttendeeType == "V")
                {
                    ModelState.Remove("oChurchAttendee.AgeBracketId");
                    //ModelState.Remove("oChurchAttend.VisitReasonId");
                    ModelState.Remove("oChurchAttendee.NationalityId");
                    //ModelState.Remove("oChurchAttend.VisitReasonId");
                    ModelState.Remove("oChurchAttendee.Title");
                }


                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Failed to load the data to save. Please refresh and try again." });

                                

                _oChangesCAA.LastMod = _tm; 
                _oChangesCAA.LastModByUserId = this._oLoggedUser.Id; /// vmMod.oUserId_Logged;
                if (_oChangesCAA.SharingStatus == null) _oChangesCAA.SharingStatus = "A"; /// allow access to data
                //validate...
                var userMessage = "";
               // var newTaskIndex = _oChangesCAA.Id == 0;
                var _isNewACheckin = _oChangesCAA.Id == 0;
                if (_oChangesCAA.Id == 0)
                {
                    _oChangesCAA.Created = _tm;
                    _oChangesCAA.CreatedByUserId = this._oLoggedUser.Id;

                    _context.Add(_oChangesCAA);

                    userMessage = (_oChangesCAA.AttendeeType == "C" ? "Church member attendee" : "Church visitor /guest" ) + " successfully CHECKED-IN.";
                    //userMessage = _oChangesCAA.AttendeeType=="V" ? "Saved Church Prospect (Regular Visitor) successfully." : "Saved church attendance (attendee) details successfully.";
                }
                else
                {
                    _context.Update(_oChangesCAA);
                    userMessage = "Attendance details updated successfully for specified " + (_oChangesCAA.AttendeeType == "C" ? "church member." : "visitor/guest.");
                    //userMessage = _oChangesCAA.AttendeeType == "V" ? "Church Prospect (Regualar Visitor) updated successfully." : "Church attendance (attendee) details updated successfully."; 
                }

                //save everything
                _context.SaveChanges();

                //TempData.Put("oVmCurrMod", vmMod);
                //TempData.Keep();

                //var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                //TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

                ///
                /// update the attendance pool... incrementally
                /// 


                // AddOrEditCurrCAA_CheckinOne(res.currCBId, res.currId, res.currAttendPersRefId, res.currEvCLId, res.currEventId, res.currAttendDate, res.currAttnType,
                /// res._strItemName, 1, 2);
                /// 
                vm.oAttendRefId = _oChangesCAA.AttendeeType == "C" ? _oChangesCAA.ChurchMemberId : _oChangesCAA.Id;

                return Json(new { taskSuccess = true, userMess = userMessage, isNewACheckin = _isNewACheckin, currCBId = _oChangesCAA.ChurchBodyId, currId = _oChangesCAA.Id, currAttendPersRefId = vm.oAttendRefId,
                                currEvCLId = vm.oEventCLId, currEventId = _oChangesCAA.ChurchEventDetailId, currAttendDate = _oChangesCAA.DateAttended, 
                                currAttnType = _oChangesCAA.AttendeeType, _strItemName = vm.strDescChkinRaw });  ///, currTask = newTaskIndex ? 1 : 2
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Failed saving church attendance (attendee) details. Err: " + ex.Message });
            }
        }



        [HttpGet]
        public IActionResult AddOrEditAttendCheckinMulti_CAA(int? reqChurchBodyId = null, string strLongevity = "C", bool loadLim = false) // or History (H), char? filterIndex = null, int? filterVal = null)
        {
            try
            {

                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return null; //// View("_ErrorPage");
                    }
                }

                //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


                if (!loadLim)
                    _ = this.LoadClientDashboardValues();  /// this._clientDBConnString);


                var oAppGloOwnId = this._oLoggedAGO.Id;
                var oCurrChuBodyId = reqChurchBodyId;

                //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCurrChuBodyId != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();

                var oAttend = new ChurchAttendanceModel();

                oAttend.strAttnLongevity = strLongevity;  // today (C), history (H) 
                oAttend.strCurrTaskDesc = oAttend.strAttnLongevity == "H" ? "Member Attendance History" : "Member Attendance Log (Today)";

                //   ViewBag.CHCF_TotCong = 0;  //the requesting congregation is included... it's excluded when summing the subs

                // List<ChurchMember> qry = new List<ChurchMember>();
                var ev = _context.ChurchCalendarEvent.Where(c => c.ChurchBodyId == oCurrChuBodyId &&
                   c.EventFrom == _context.ChurchCalendarEvent.Where(y => y.ChurchBodyId == c.ChurchBodyId).Max(y => y.EventFrom)).FirstOrDefault();
                var ev_Id = ev != null ? ev.Id : (int?)null;
                oAttend.f_DateAttended = ev != null ? (DateTime)(ev.EventFrom.Value).Date : DateTime.Now;

                oAttend.lsChurchAttendanceModels = ReturnChurchAttendeesList_MemEdit(oCB, ev_Id, oAttend.f_DateAttended);
                //lookups
                oAttend.lkpChuCalEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP)
                                        .Where(c => !string.IsNullOrEmpty(c.Subject) && c.IsEventActive == true && c.ChurchBody.AppGlobalOwnerId == oCB.AppGlobalOwnerId &&
                                             c.ChurchBody.CtryAlpha3Code == oCB.CtryAlpha3Code &&
                                             (c.ChurchBodyId == oCurrChuBodyId ||
                                             (c.ChurchBodyId != oCurrChuBodyId && c.SharingStatus == "C" && c.ChurchBodyId == oCB.ParentChurchBodyId) ||
                                             (c.ChurchBodyId != oCurrChuBodyId && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.ChurchBody, oCB)))
                                             )
                                            .OrderByDescending(c => c.EventTo).ThenByDescending(c => c.EventTo)
                                            .ToList()
                                                    .Select(c => new SelectListItem()
                                                    {
                                                        Value = c.Id.ToString(),
                                                        Text = c.Subject + ":- " +
                                                                            (c.IsFullDay == true ?
                                                                                (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                                (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                                (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
                                                                                (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                             )
                                                    })
                                                    .OrderBy(c => c.Text)
                                                    .ToList();
                oAttend.lkpChuCalEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });

                oAttend.oChurchBody = oCB;
                oAttend.oLoggedChurchBody = oCB;

                ViewBag.promptUserMsg = "";
                TempData.Keep();


                return PartialView("_vwAttendCheckinMulti_CAA", oAttend); //View(lsChurchAttendanceModels); 


            }
            catch (Exception ex)
            {
                return PartialView("_vwErrorPage");
            }
        }





        [HttpGet]
        public IActionResult AddOrEdit_Attendees(string strAttendee = "V", string strLongevity = "C", int id = 0, int? oCurrChuBodyId = null, bool dbload = true) //, string vStatus = "A")
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            //if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            //{ RedirectToAction("LoginUserAcc", "UserLogin"); }


            //if (!loadLim)
            //    _ = this.LoadClientDashboardValues(this._clientDBConnString);


            var oAppGloOwnId = this._oLoggedAGO.Id;
            // var oCurrChuBodyId = reqChurchBodyId;

            //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

            ChurchBody oCB = this._oLoggedCB;
            if (oCurrChuBodyId != this._oLoggedCB.Id)
                oCB = _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();

            var arrData = ""; // TempData["oVmCurrMod"] as string;
            arrData = TempData.ContainsKey("oVmCurr") ? TempData["oVmCurr"] as string : arrData;
            var oCurrVmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData) : null;

            // var oCurrVmMod = TempData.Get<ChurchAttendanceModel>("oVmCurr"); TempData.Keep();
            if (oCurrVmMod == null) oCurrVmMod = new ChurchAttendanceModel();
            //
            //var oCurrChuBody_Logged = oUserLogIn_Priv[0].ChurchBody;
            //var oUserProfile = oUserLogIn_Priv[0].UserProfile;
            //var oCurrChuMember_Logged = oUserProfile.ChurchMember;
            //if (oCurrChuBody_Logged == null || oCurrChuMember_Logged == null) return null;

            //// check permission for Core life...
            //if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) //prompt!
            //    return null;

            //ChurchBody oCurrChuBody = null;
            //// var oCurrChuBody = _context.ChurchBody.Include(t=>t.AppGlobalOwner).Where(c=>c.Id == oCurrChuBodyId).FirstOrDefault();
            //if (oCurrChuBodyId != null) oCurrChuBody = _context.ChurchBody.Include(t => t.AppGlobalOwner).Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();
            //if (oCurrChuBody == null) oCurrChuBody = oCurrChuBody_Logged;

            //oCurrVmMod.oAppGlobalOwner = oCurrChuMember_Logged.AppGlobalOwner;

            if (id == 0)
            {
                //create user and init...
                oCurrVmMod.oChurchAttendee = new ChurchAttendAttendee();
                oCurrVmMod.oChurchAttendee.ChurchBodyId = oCurrChuBodyId; // (int)oCurrChuBody.Id;

                // var currCnt = _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == (int)oCurrChuBody.Id).Count() + 1;
                // var strLocMemCode = (!string.IsNullOrEmpty(oCurrChuBody.GlobalChurchCode) ? oCurrChuBody.GlobalChurchCode + "-" : "") + currCnt.ToString();  //add preceding zero's

                oCurrVmMod.oChurchAttendee.AttendeeType = strAttendee; //  "M";  //(M)ember , (V)isitor  
                oCurrVmMod.dtEventFilterDate1 = DateTime.Now;
                oCurrVmMod.dtEventFilterDate2 = DateTime.Now;
                oCurrVmMod.dtFilter_AttendFrom = DateTime.Now;
                oCurrVmMod.dtFilter_AttendTo = DateTime.Now;
                oCurrVmMod.oChurchAttendee.DateAttended = DateTime.Now; 

                oCurrVmMod.oChurchBody = oCB;
            }

            else
            {
                //fetch mem details
                dbload = true;
                if (dbload)
                {
                    var oChurchAttendVM = (
                                 from t_caa in _context.ChurchAttendAttendee.AsNoTracking().Include(t => t.Nationality)
                                                .Where(x => x.ChurchBodyId == oCurrChuBodyId && x.Id == id)
                                 join t_cbx in _context.ChurchBody.AsNoTracking() on t_caa.ChurchBodyId equals t_cbx.Id into _tcb
                                 from t_cb in _tcb
                                 join t_cmx in _context.ChurchMember.AsNoTracking().Include(t => t.Nationality).Include(t => t.ContactInfo) on t_caa.ChurchMemberId equals t_cmx.Id into _tcm
                                 from t_cm in _tcm.DefaultIfEmpty()
                                 join t_ccex in _context.ChurchCalendarEvent.AsNoTracking() on t_caa.ChurchEventDetailId equals t_ccex.Id into _cce
                                 from t_cce in _cce.DefaultIfEmpty()

                                 select new ChurchAttendanceModel
                                 {
                                     oChurchBody = t_cb,
                                     strCongregation = t_cb.Name,
                                     //peers stuff of mem
                                     oChurchAttendee = t_caa,
                                     strAttendeeName = t_caa.AttendeeType == "C" ? (t_cm != null ? AppUtilties.GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false) : "") :
                                                                            AppUtilties.GetConcatMemberName(t_caa.Title, t_caa.FirstName, t_caa.MiddleName, t_caa.LastName, false),
                                     strAttendeeTypeDesc = GetMemTypeDesc(t_caa.AttendeeType),
                                     strChurchEventDesc = t_cce != null ?
                                                        t_cce.Subject + ":- " +
                                                                            (t_cce.IsFullDay == true ?
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() :
                                                                                (t_cce.EventFrom != null ? DateTime.Parse(t_cce.EventFrom.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim() +
                                                                                (t_cce.EventFrom != null && t_cce.EventTo != null ? " -- " : "") +
                                                                                (t_cce.EventTo != null ? DateTime.Parse(t_cce.EventTo.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : "").Trim()
                                                                             ) : "",
                                     strNationality = t_caa.AttendeeType == "C" ? (t_cm != null ?
                                                            (t_cm.Nationality != null ? t_cm.Nationality.EngName : "") : "") :
                                                            (t_caa.Nationality != null ? t_caa.Nationality.EngName : ""),
                                     strResidenceLoc = t_caa.AttendeeType == "C" ? (t_cm != null ?
                                                             (t_cm.ContactInfo != null ? t_cm.ContactInfo.Location : "") : "") :
                                                             t_caa.ResidenceLoc,
                                     strPhone = t_caa.AttendeeType == "C" ? (t_cm != null ?
                                                            (t_cm.ContactInfo != null ? t_cm.ContactInfo.MobilePhone1 : "") : "") :
                                                            t_caa.MobilePhone,
                                     strDateAttended = t_caa.DateAttended != null ?
                                                             DateTime.Parse(t_caa.DateAttended.ToString()).ToString("d MMM, yyyy", CultureInfo.InvariantCulture) : ""
                                 })
                                 .FirstOrDefault();

                    oCurrVmMod = oChurchAttendVM;
                }

                else     //get the saved... all the member_data get loaded when drilling down on member
                {
                   // oCurrVmMod = TempData.Get<ChurchAttendanceModel>("oVmCurr");

                    var arrData1 = ""; // TempData["oVmCurrMod"] as string;
                    arrData1 = TempData.ContainsKey("oVmCurr") ? TempData["oVmCurr"] as string : arrData1;
                    oCurrVmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData1) : null;
                }
                    


                if (oCurrVmMod.oChurchAttendee != null)
                {
                    oCurrVmMod.dtEventFilterDate1 = DateTime.Now;
                    oCurrVmMod.dtEventFilterDate2 = DateTime.Now;
                }
            }


            oCurrVmMod.oChurchBody = oCB;  //current working CB
            oCurrVmMod.oLoggedChurchBody = oCB;
            oCurrVmMod.strAttnLongevity = strLongevity;  // today (C), history (H)
            oCurrVmMod.f_strAttendeeTypeCode = strAttendee;  //else history
                        

            oCurrVmMod.lsChurchAttendanceModels_AttendFilter = new List<ChurchAttendanceModel>();

            if (oCurrVmMod == null) return PartialView("_AddOrEdit_Attendees", oCurrVmMod);

            //  TempData.Put("oCurrChuMemberPrvw", oCurrVmMod.oChurchAttend);
            var _vmMod = populateLookups_Attend(oCurrVmMod, oCB, oCurrVmMod.oChurchAttendee.DateAttended);//, oCurrVmMod.oChurchAttend.Id);
            //TempData.Put("oVmCurrMod", _vmMod);
            //TempData.Keep();

            var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(_vmMod);
            TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

            return PartialView("_AddOrEdit_Attendees", _vmMod);
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit_Attendees(ChurchAttendanceModel vmMod)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            ChurchAttendAttendee _oChanges = vmMod.oChurchAttendee; //  .oChurchAttend;

            //
            // var oFormFile = vmMod.MemPhotoFile;
          //  vmMod = TempData.Get<ChurchAttendanceModel>("oVmCurrMod"); TempData.Keep();

            var arrData = ""; // TempData["oVmCurrMod"] as string;
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchAttendanceModel>(arrData) : vmMod;

            var oCV = vmMod.oChurchAttendee;
            oCV.ChurchBody = vmMod.oChurchBody;

            try
            {
                ModelState.Remove("oChurchAttendee.ChurchBodyId");
                ModelState.Remove("oChurchAttendee.ChurchMemberId");
                ModelState.Remove("oChurchAttendee.ChurchEventId");
                ModelState.Remove("oChurchAttendee.AgeBracketId");
                //ModelState.Remove("oChurchAttend.VisitReasonId");
                ModelState.Remove("oChurchAttendee.NationalityId");
                //ModelState.Remove("oChurchAttend.VisitReasonId");
                ModelState.Remove("oChurchAttendee.Title");

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again." });

                if (_oChanges.AttendeeType == "C" )  //Congregant... ChurcCodes required
                {
                    if (_oChanges.ChurchMemberId == null)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select member attendee." });

                    var oMCAList_Dup = _context.ChurchAttendAttendee.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && c.ChurchMemberId==_oChanges.ChurchMemberId &&
                                         c.ChurchEventDetailId == _oChanges.ChurchEventDetailId && c.DateAttended == _oChanges.DateAttended
                                        ).ToList();

                    if (oMCAList_Dup.Count > 0)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Member, " + AppUtilties.GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName, false) + " is already checked-in for this event for same date specified." });

                }
                else if (_oChanges.AttendeeType == "V") //  && (string.IsNullOrEmpty(_oChanges.FirstName) || string.IsNullOrEmpty(_oChanges.LastName)))  //Congregant... ChurcCodes required
                {
                    if ((string.IsNullOrEmpty(_oChanges.FirstName) || string.IsNullOrEmpty(_oChanges.LastName)))  //Congregant... ChurcCodes required 
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Firstname and last name of visitor required." });

                    if (_oChanges.TempCelc != 0 && _oChanges.TempCelc <= 30 && _oChanges.TempCelc >= 40)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please check the body temperature again. Doesn't seem to be normal. Hint: Clear value to continue [0]" });

                    if (_oChanges.PersBPMin != 0 && _oChanges.PersBPMax != 0 && _oChanges.PersBPMin >= _oChanges.PersBPMax)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Blood pressure values indicated is not correct. Diastolic [2nd] value must be less than the Systolic [1st] value. Hint: Clear values to continue [0]" });

                    if (!string.IsNullOrEmpty(_oChanges.Email))
                    {
                        //  ... check validity... REGEX 
                        if (!AppUtilties.IsValidEmail(_oChanges.Email))
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Email specified is invalid. Please check and try again.", signOutToLogIn = false });
                    }

                    var oMCAList_Dup = _context.ChurchAttendAttendee.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                                         c.ChurchEventDetailId == _oChanges.ChurchEventDetailId && c.DateAttended == _oChanges.DateAttended  &&
                                        (c.FirstName + c.MiddleName + c.LastName).Contains(_oChanges.FirstName) && (c.FirstName + c.MiddleName + c.LastName).Contains(_oChanges.MiddleName) &&
                                        (c.FirstName + c.MiddleName + c.LastName).Contains(_oChanges.LastName) && (c.MobilePhone==_oChanges.MobilePhone || c.ResidenceLoc == _oChanges.ResidenceLoc || c.Email==_oChanges.Email)
                                        ).ToList();

                    if (oMCAList_Dup.Count > 0)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Guest, " + AppUtilties.GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName, false) +  " is already checked-in for this event for same date specified." });

                }

                _oChanges.LastMod = DateTime.Now;

                //validate...
                var userMessage = "";
                var newTaskIndex = _oChanges.Id == 0;
                if (_oChanges.Id == 0)
                {
                    _oChanges.Created = DateTime.Now;
                    _context.Add(_oChanges);

                    userMessage = _oChanges.AttendeeType == "V" ? "Church visitor checked-in successfully." : "Church attendee checked-in successfully.";
                    //userMessage = _oChanges.AttendeeType=="V" ? "Saved Church Prospect (Regular Visitor) successfully." : "Saved church attendance (attendee) details successfully.";
                }
                else
                {
                    _context.Update(_oChanges);
                    userMessage = _oChanges.AttendeeType == "V" ? "Church visitor details updated successfully." : "Church attendance (attendee) details updated successfully.";
                    //userMessage = _oChanges.AttendeeType == "V" ? "Church Prospect (Regualar Visitor) updated successfully." : "Church attendance (attendee) details updated successfully."; 
                }

                //save everything
                _context.SaveChanges();

                //TempData.Put("oVmCurrMod", vmMod);
                //TempData.Keep();

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

                // DisplaySuccessInfo(res.userMess, res.currTask, res.oCurrId, currChuBodyId, _strAttendee, _strLongevity, res.evtId, res.evtDate, false);
                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = userMessage, evtId = _oChanges.ChurchEventDetailId, currTask = newTaskIndex ? 1 : 2 });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving church attendance (attendee) details. Err: " + ex.Message });
            }
        }
         
        public string GetGlobalMemberCodeByChurchCode_string(int? oAGOid, int? oCBid, string strPFX_CC, bool bl_LDZR = false, int varCode = 0)
        {
            if (varCode == 0) return "";

            //if (this._context == null)
            //{
            //    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
            //    if (this._context == null)
            //    {
            //        RedirectToAction("LoginUserAcc", "UserLogin");

            //        // should not get here... Response.StatusCode = 500; 
            //        return null; //// View("_ErrorPage");
            //    }
            //}


            //if (this._context == null)
            //    if (!InitializeUserLogging()) return string.Empty;

            //var oChurchBody = _context.ChurchBody.AsNoTracking().Include(t=>t.ParentChurchBody).Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBid).FirstOrDefault();
            //if (oChurchBody == null) return "";

            /// 
            var strPrefix = strPFX_CC + (!string.IsNullOrEmpty(strPFX_CC) ? "/" : "");  // PCG001/

            //var fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid);
            //var tempCnt = fsCount + 1; var tempCode = strPrefix.ToUpper() + (bl_LDZR ? string.Format("{0:D4}", tempCnt) : tempCnt.ToString());
            //fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.GlobalMemberCode == tempCode);


            // unique per denomination ... across congregations
            var tempCode = strPrefix.ToUpper() + (bl_LDZR ? string.Format("{0:D4}", varCode) : varCode.ToString());
            var fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.GlobalMemberCode == tempCode);
            if (fsCount == 0) return tempCode;
            else
            {
                //tempCnt++; tempCode = bl_LDZR ? string.Format("{0:D4}", tempCnt) : tempCnt.ToString();
                //tempCode = strPrefix.ToUpper() + tempCode;
                //fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.GlobalMemberCode == tempCode);
                //var res = false;
                //while (fsCount > 0 && fsCount < 10)
                //{
                //    tempCnt++; tempCode = bl_LDZR ? string.Format("{0:D4}", tempCnt) : tempCnt.ToString();
                //    tempCode = strPrefix.ToUpper() + tempCode;
                //    fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.GlobalMemberCode == tempCode);
                //    //
                //    res = fsCount == 0;
                //}

                return "";  // tempCode;  
            }
        }
         
        public string GetCustomMemberCodeByPRFX_SFX_string(int? oAGOid, int? oCBid, string strPFX, string strSFX, bool bl_LDZR = false, int varCode = 0)
        {
            if (varCode == 0) return "";

            //if (this._context == null)
            //{
            //    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
            //    if (this._context == null)
            //    {
            //        RedirectToAction("LoginUserAcc", "UserLogin");

            //        // should not get here... Response.StatusCode = 500; 
            //        return null; //// View("_ErrorPage");
            //    }
            //}


            //if (this._context == null)
            //    if (!InitializeUserLogging()) return string.Empty;

            //var oChurchBody = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBid).FirstOrDefault();
            //if (oChurchBody == null) return "";

            ///  PRX -- ??? --- SFX
            //var fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid);
            //var tempCnt = fsCount + 1; var tempCode = strPFX.ToUpper() + (bl_LDZR ? string.Format("{0:D4}", tempCnt) : tempCnt.ToString()) + strSFX;


            // unique per congregation ... not the denomination
            var tempCode = strPFX.ToUpper() + (bl_LDZR ? string.Format("{0:D4}", varCode) : varCode.ToString()) + strSFX;
            var fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.GlobalMemberCode == tempCode);
            if (fsCount == 0) return tempCode;
            else
            {
                //tempCnt++; tempCode = bl_LDZR ? string.Format("{0:D4}", tempCnt) : tempCnt.ToString();
                //tempCode = strPFX.ToUpper() + tempCode + strSFX;
                //fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.GlobalMemberCode == tempCode);
                //var res = false;
                //while (fsCount > 0 && fsCount < 10)
                //{
                //    tempCnt++; tempCode = bl_LDZR ? string.Format("{0:D4}", tempCnt) : tempCnt.ToString();
                //    tempCode = strPFX.ToUpper() + tempCode + strSFX;
                //    fsCount = _context.ChurchMember.AsNoTracking().Count(c => c.AppGlobalOwnerId == oAGOid && c.GlobalMemberCode == tempCode);
                //    //
                //    res = fsCount == 0;
                //}

                return ""; // tempCode;
            }
        }

         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit_AttendeesNC(ChurchAttendanceModel vm)  // int? oCBid, int? oAttendid)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            var oAppGloOwnId = this._oLoggedAGO.Id;
            var oCBid = vm.oChurchBodyId;
            var oChanges_NC = vm.oChurchAttendee;
            var oAttendid = oChanges_NC != null ? oChanges_NC.Id : (int?)null;


            if (oCBid == null)
                return Json(new { taskSuccess = false, oCurrId = oCBid, userMess = "Specified congregation of new convert could not be verified. Please refresh and try again.", signOutToLogIn = false });

            var oCM_CB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
            if (oCM_CB == null)
                return Json(new { taskSuccess = false, oCurrId = oCBid, userMess = "Specified congregation of new convert could not be verified. Please refresh and try again.", signOutToLogIn = false });

            if (string.IsNullOrEmpty(oCM_CB.GlobalChurchCode))
                return Json(new { taskSuccess = false, oCurrId = oCBid, userMess = "Church code for target congregation could not be verified. Please verify with System Admin and try again.", signOutToLogIn = false });


            ///
            ChurchAttendAttendee _oChangesCAA = _context.ChurchAttendAttendee.AsNoTracking().Include(t=> t.ChurchBody)
                .Where(c=> c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId==oCBid && c.Id== oAttendid).FirstOrDefault();
             
            if (_oChangesCAA == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "New Convert details not found. Please refresh and trya again." });

            if (string.IsNullOrEmpty(_oChangesCAA.FirstName) || string.IsNullOrEmpty(_oChangesCAA.FirstName))
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "First name and last name are required", signOutToLogIn = false });

            if (_oChangesCAA.DateOfBirth == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "New Convert's date of birth is not indicated. Please add date of birth [save] and try process again. Hint: Make some assumptions if necessary.", signOutToLogIn = false });

            if (_oChangesCAA.DateOfBirth != null)
            {
                if (_oChangesCAA.DateOfBirth != null)
                    if (_oChangesCAA.DateOfBirth.Value > DateTime.Now)
                        return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Date of birth cannot be later than today", signOutToLogIn = false });
            }
             
            if (_oChangesCAA.MaritalStatus == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "New Convert's marital status is not indicated. Please add marital status [save] and try process again." });

            if (_oChangesCAA.NationalityId == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "New Convert's nationality is not indicated. Please add nationality [save] and try process again." });

            if (_oChangesCAA.EnrollMode == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "How did New Convert join the congregation? Please inidicate the enroll mode [save] and try process again." });

            // member class, member type, member rank and member status --- all required!string.IsNullOrEmpty(_oChanges.MemberScope) || 
            if (_oChangesCAA.ChurchRankId == null || _oChangesCAA.ChurchMemStatusId == null)
            {
                string strStatErr = ""; 
                if (_oChangesCAA.ChurchRankId == null) strStatErr += "member rank, ";
                if (_oChangesCAA.ChurchMemStatusId == null) strStatErr += "member status, "; 
                if (strStatErr.Contains(",")) strStatErr = strStatErr.Remove(strStatErr.LastIndexOf(","));
                ///
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Statuses are flags that control the member state and behaviour. Please specify " + strStatErr });
            }


            /// check MS if any [vm.numMemStatusId] ... past, dead must mave Member Account Status = Closed
            var oMS_Unavail = _context.AppUtilityNVP.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId &&
                                                        c.NVPCode == "CMS" && c.Id == _oChangesCAA.ChurchMemStatusId).FirstOrDefault();
            if (oMS_Unavail == null)
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Member status specified could not be verified. Please check with administrator and retry" });

            /// UPDATE @member profile...biodata
            if (oMS_Unavail.IsAvailable == false) // _oChangesCAA.Status = "X"; /// Closed
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Member status specified is unavailable. New Convert cannot have the status: '" + oMS_Unavail.NVPValue + "'" });


            var strMemName = AppUtilties.GetConcatMemberName(_oChangesCAA.Title, _oChangesCAA.FirstName, _oChangesCAA.MiddleName, _oChangesCAA.LastName, true, true, false, false, true);

            var oCAA_CMList_Dup = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesCAA.AppGlobalOwnerId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId &&
                                       // c.ChurchEventId == _oChangesCAA.ChurchEventId && c.DateAttended == _oChangesCAA.DateAttended &&
                                       (c.FirstName + c.MiddleName + c.LastName).Contains(_oChangesCAA.FirstName) && (c.FirstName + c.MiddleName + c.LastName).Contains(_oChangesCAA.MiddleName) &&
                                       (c.FirstName + c.MiddleName + c.LastName).Contains(_oChangesCAA.LastName) && 
                                       ((c.NationalityId == _oChangesCAA.NationalityId && c.DateOfBirth == _oChangesCAA.DateOfBirth &&
                                        (c.ContactInfo.MobilePhone1 == _oChangesCAA.MobilePhone || c.ContactInfo.MobilePhone2 == _oChangesCAA.MobilePhone || 
                                         c.ContactInfo.ResidenceAddress == _oChangesCAA.ResidenceLoc || c.ContactInfo.Location == _oChangesCAA.ResidenceLoc)) ||
                                         c.ContactInfo.Email == _oChangesCAA.Email)
                                       ).ToList();

            if (oCAA_CMList_Dup.Count > 0)
                return Json(new { taskSuccess = false, oCurrId = oCAA_CMList_Dup[0].Id, userMess = "Guest, " + strMemName + " is already added in the member pool [as Affiliate, New Convert, In-Transit or Member]. Hint: Search for the member profile and do the necessary changes else contact admin." });


            try
            { 
                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Failed to load the data to save. Please refresh and try again." });

                var oCTRYDef = _context.CountryCustom.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCBid && c.IsDefaultCountry == true).FirstOrDefault();
                //oChurchMember.NationalityId = oCTRYDef != null ? oCTRYDef.CtryAlpha3Code : null;


               
                /// var _userTask = "";
                var tm = DateTime.Now;

                /// create the member bio -- assign member type - New Convert
                /// 
                var oCAA_CM = new ChurchMember() {
                    
                    AppGlobalOwnerId = oAppGloOwnId,
                    ChurchBodyId = oCBid,
                    FirstName = _oChangesCAA.FirstName,
                    MiddleName = _oChangesCAA.MiddleName,
                    LastName = _oChangesCAA.LastName,
                    NationalityId = _oChangesCAA.NationalityId,
                    /// GlobalMemberCode =  ???
                    Title = _oChangesCAA.Title,
                    DateOfBirth = _oChangesCAA.DateOfBirth,
                    MaritalStatus = _oChangesCAA.MaritalStatus,
                    MemberScope = "E", // external ... not yet in
                    Gender = _oChangesCAA.Gender,
                    Notes = _oChangesCAA.Notes, 
                    Status = "A",  // Active 
                    
                    ///
                    Created  = tm,
                    LastMod = tm,
                    CreatedByUserId = this._oLoggedUser.Id,
                    LastModByUserId = this._oLoggedUser.Id
                };

                _context.ChurchMember.Add(oCAA_CM);
               /// _userTask = "Added new church member, " + (!string.IsNullOrEmpty(strMemName) ? "[" + strMemName + "]" : "") + " -biodata successfully";

                //save church member first... 
                _context.SaveChanges();




                // get the unique id from db and attach to the CB code for unique --- Global code for member
                //// member code... auto/sys
                var updChangesMade = false;
                if (string.IsNullOrEmpty(oCAA_CM.GlobalMemberCode))  //if (_oChanges.MemberScope == "C")  ... thus only congregants to have global code
                {
                    oCAA_CM.GlobalMemberCode = GetGlobalMemberCodeByChurchCode_string(oCAA_CM.AppGlobalOwnerId, oCAA_CM.ChurchBodyId, oCM_CB.GlobalChurchCode, false, oCAA_CM.Id);  // oCM_BD.ChurchBody.GlobalChurchCode + "/" + _oChanges.Id; // 

                    _context.ChurchMember.Update(oCAA_CM);
                    updChangesMade = true;
                }

                // custom member code... auto/man
                if (string.IsNullOrEmpty(oCAA_CM.MemberCustomCode))
                {
                    var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.ChurchBody).ThenInclude(t => t.ChurchLevel)  //.Include(t => t.AppGlobalOwner) //.Include(t => t.OwnedByChurchBody)
                                         .Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.NVPCode == "MCCF").ToList();

                    oNVP_List_1 = oNVP_List_1.Where(c =>
                                       (c.OwnedByChurchBodyId == null || c.OwnedByChurchBodyId == oCAA_CM.ChurchBodyId ||
                                       (c.OwnedByChurchBodyId != oCAA_CM.ChurchBodyId && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCM_CB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != oCAA_CM.ChurchBodyId && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, oCM_CB)))).ToList();

                    var oNVP_MCCF = oNVP_List_1
                        .Where(c => c.ChurchBodyId == oCAA_CM.ChurchBodyId && c.NVPSubCode == "AUT_GN" && c.ChurchBody.ChurchLevel.LevelIndex == oNVP_List_1.Min(y => y.ChurchBody.ChurchLevel.LevelIndex)).FirstOrDefault();
                    if (oNVP_MCCF != null)
                    {
                        var bl_AUT_GN = oNVP_MCCF.NVPValue == "Y";
                        if (bl_AUT_GN)
                        {
                            var strPFX = ""; var strSFX = ""; var bl_LDZR = false;
                            var oNVP_PFX = oNVP_List_1.Where(c => c.NVPSubCode == "PFX" && c.ChurchBody.ChurchLevel.LevelIndex == oNVP_List_1.Min(y => y.ChurchBody.ChurchLevel.LevelIndex)).FirstOrDefault();
                            if (oNVP_PFX != null) strPFX = oNVP_PFX.NVPValue;
                            var oNVP_SFX = oNVP_List_1.Where(c => c.NVPSubCode == "SFX" && c.ChurchBody.ChurchLevel.LevelIndex == oNVP_List_1.Min(y => y.ChurchBody.ChurchLevel.LevelIndex)).FirstOrDefault();
                            if (oNVP_SFX != null) strSFX = oNVP_SFX.NVPValue;
                            var oNVP_LDZR = oNVP_List_1.Where(c => c.NVPSubCode == "LDZR" && c.ChurchBody.ChurchLevel.LevelIndex == oNVP_List_1.Min(y => y.ChurchBody.ChurchLevel.LevelIndex)).FirstOrDefault();
                            if (oNVP_LDZR != null) bl_LDZR = oNVP_LDZR.NVPValue == "Y";

                            // generate...
                            oCAA_CM.MemberCustomCode = GetCustomMemberCodeByPRFX_SFX_string(oCAA_CM.AppGlobalOwnerId, oCAA_CM.ChurchBodyId, strPFX, strSFX, bl_LDZR, oCAA_CM.Id);

                            _context.ChurchMember.Update(oCAA_CM);
                            updChangesMade = true;

                        }
                    }
                }
                else
                {   ////check that custom code is unique within congregation  ... new or upd
                    var existMember = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.ChurchBodyId == oCAA_CM.ChurchBodyId && //... restrict within denomination as dbase is per denomination
                                                               (oCAA_CM.Id == 0 || (oCAA_CM.Id > 0 && c.Id != oCAA_CM.Id)) && c.MemberCustomCode == oCAA_CM.MemberCustomCode).FirstOrDefault();
                    if (existMember != null)
                        return Json(new
                        {
                            taskSuccess = false,
                            oCurrId = oCAA_CM.Id,
                            userMess = "Custom member code '" + oCAA_CM.MemberCustomCode + "' must be unique within congregation. [Hint: Member with same code: " +
                            AppUtilties.GetConcatMemberName(existMember.Title, existMember.FirstName, existMember.MiddleName, existMember.LastName, false, false, false, false, false) + (existMember.ChurchBody != null ? " / " + existMember.ChurchBody.Name : "") + " ]",
                            signOutToLogIn = false
                        });
                }

                 

                /// add CI ...
                /// 
                if (!string.IsNullOrEmpty(_oChangesCAA.Email))
                {
                    //  ... check validity... REGEX 
                    if (!AppUtilties.IsValidEmail(_oChangesCAA.Email))
                        return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Prospective New Convert email invalid. Please check and try again.", signOutToLogIn = false });

                    // use email to check for member duplication across denomination ... disallow impersonation within same CB... mem can be at diff levels or congregations but must NOT be Active [historic data]
                    var oCIEmailMem = _context.ChurchMember.AsNoTracking().Include(t => t.ChurchBody)
                        .Where(c => c.AppGlobalOwnerId == oAppGloOwnId &&
                                   (c.ChurchBodyId == _oChangesCAA.ChurchBodyId || (c.ChurchBodyId != _oChangesCAA.ChurchBodyId && c.Status == "A")) && // [ c.Status=="A" ] stat summary in sync with member_status [available - regular, invalid /not available - distant, virtual, past, passed]
                                    c.ContactInfo.Email == _oChangesCAA.Email).FirstOrDefault();

                    if (oCIEmailMem != null)
                        return Json(new
                        {
                            taskSuccess = false,
                            oCurrId = _oChangesCAA.Id,
                            userMess = "Email must be unique. Email already used by another: [ Member: " +
                            AppUtilties.GetConcatMemberName(oCIEmailMem.Title, oCIEmailMem.FirstName, oCIEmailMem.MiddleName, oCIEmailMem.LastName, false, false, false, false, false) + (oCIEmailMem.ChurchBody != null ? " / " + oCIEmailMem.ChurchBody.Name : "") + " ]",
                            signOutToLogIn = false
                        });

                    var oCIEmailExist = _context.ContactInfo.AsNoTracking().Include(t => t.ChurchBody)
                                .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == _oChangesCAA.ChurchBodyId && c.Email == _oChangesCAA.Email).FirstOrDefault();

                    if (oCIEmailExist != null)
                        return Json(new
                        {
                            taskSuccess = false,
                            oCurrId = _oChangesCAA.Id,
                            userMess = "Email must be unique. Email already used by another: [Member: " +
                            oCIEmailExist.ContactInfoDesc + (oCIEmailMem.ChurchBody != null ? " / " + oCIEmailExist.ChurchBody.Name : " <elsewhere>") + " ]",
                            signOutToLogIn = false
                        });
                }



                /// Add MCLAv ... pick New Convert activity from settings (NVP) ... add the event task [with status: Pending]
                /// 
                /// add MT, MR = ??,  MS = Avail
                ///  

                var oCAA_CI = new ContactInfo()
                {
                    AppGlobalOwnerId = oAppGloOwnId,
                    ChurchBodyId = oCBid,
                    ChurchMemberId = oCAA_CM.Id, // unmapped ... cyclic redundancy! 1-1 for now... will be multi later
                    CtryAlpha3Code = oCAA_CM.NationalityId != null ? oCAA_CM.NationalityId : (this.oCTRYDefault != null ? this.oCTRYDefault.CtryAlpha3Code : null),
                    MobilePhone1 = _oChangesCAA.MobilePhone,
                    Email = _oChangesCAA.Email,
                    Location = _oChangesCAA.ResidenceLoc, 
                    ///
                    IsPrimaryContact = true,
                    IsChurchFellow = true,
                    Created = tm,
                    LastMod = tm,
                    CreatedByUserId = this._oLoggedUser.Id,
                    LastModByUserId = this._oLoggedUser.Id
                };


                _context.ContactInfo.Add(oCAA_CI);
                

                //save CI to get Id... 
                _context.SaveChanges();
                updChangesMade = false;

                // ci id may change...
                if (oCAA_CM.PrimContactInfoId != oCAA_CI.Id)
                {
                    oCAA_CM.PrimContactInfoId = oCAA_CI.Id;
                    _context.ChurchMember.Update(oCAA_CM);
                    updChangesMade = true;
                }
                 

                /// church life
                /// 
                var oMCL_BD = _context.MemberChurchlife.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.ChurchBodyId == oCAA_CM.ChurchBodyId && c.ChurchMemberId == oCAA_CM.Id).FirstOrDefault();
                if (oMCL_BD == null)
                {
                    oMCL_BD = new MemberChurchlife()
                    {
                        AppGlobalOwnerId = oCAA_CM.AppGlobalOwnerId,
                        ChurchBodyId = oCAA_CM.ChurchBodyId,
                        ChurchMemberId = (int)oCAA_CM.Id,
                        IsCurrentMember = true, 
                        EnrollMode = _oChangesCAA.EnrollMode,  /// walk-in  ... can confirm later
                        DateJoined = oCAA_CM.Created, 
                        //IsMemBaptized , IsMemConfirmed, IsMemCommunicant
                        SharingStatus = "A"
                    };

                    _context.MemberChurchlife.Add(oMCL_BD);
                    updChangesMade = true;
                }

                ///
                if (updChangesMade)
                {
                    _context.SaveChanges();
                    updChangesMade = false;
                }


                /// MCET
                //// church activity added!  ... auto create all the MCET for this activity and assign Pending status... [ P-I-W-C ] allow tasks to be waived anyways!
                ///
                List<MemberChurchlifeActivity> oMCLAcList = new List<MemberChurchlifeActivity>();
                List<MemberChurchlifeEventTask> oMCETList = new List<MemberChurchlifeEventTask>();

                var oCLAList = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                                    .Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId &&
                                                    c.NVPCode == "CLA" && c.ApplyToMemberStatus == "N").ToList();   /// attached to New Convert module

                oCLAList = oCLAList.Where(c =>
                                    (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))
                                   )
                                    .OrderBy(c => c.OrderIndex)
                                    .ToList();


                foreach (var oCLAc in oCLAList)
                {

                    var oCAA_MCLAc = new MemberChurchlifeActivity()
                    {
                        AppGlobalOwnerId = oCAA_CM.AppGlobalOwnerId,
                        ChurchBodyId = oCAA_CM.ChurchBodyId,
                        ChurchMemberId = oCAA_CM.Id,
                        ChurchlifeActivityId = oCLAc.Id,
                        IsChurchEvent = false,
                        SharingStatus = "A",
                        EventDate = DateTime.Now,
                        Created = tm,
                        LastMod = tm,
                        CreatedByUserId = this._oLoggedUser.Id,
                        LastModByUserId = this._oLoggedUser.Id
                    };


                    _context.MemberChurchlifeActivity.Add(oCAA_MCLAc);

                    // save to get oCAA_CLAc Id ... for the CETs
                    _context.SaveChanges();


                    /// the definition list to be picked from...  strNVPCode = "CLA";    // c.Id != oCurrNVP.Id  
                    var oCLARDList = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                                       .Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId &&
                                                       c.NVPCode == "CLARD" && c.NVPCategoryId == oCAA_MCLAc.ChurchlifeActivityId).ToList();   /// ex. New Convert Class

                    oCLARDList = oCLARDList.Where(c =>
                                        (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))
                                       )
                                        .OrderBy(c => c.OrderIndex)
                                        .ToList();
                     

                    //create approval action... at least  one approval level
                    if (oCLARDList.Count > 0)
                    {
                        //create oMCET steps
                        var stepIndexLowest = oCLARDList[0].OrderIndex;
                        foreach (AppUtilityNVP oCLARD_NVP in oCLARDList)
                        {
                            var oMCETList_Dup = _context.MemberChurchlifeEventTask.AsNoTracking()
                                                    .Include(t => t.ActivityRequirementDef)
                                                    .Include(t => t.MemberChurchlifeActivity).ThenInclude(t => t.ChurchlifeActivity)
                                                    .Include(t => t.ChurchBody).ThenInclude(t => t.ChurchLevel)
                                                .Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.ChurchBodyId == oCAA_CM.ChurchBodyId && c.ChurchMemberId == oCAA_CM.Id &&
                                                            c.RequirementDefId == oCLARD_NVP.Id && c.MemberChurchlifeActivityId == oCAA_MCLAc.Id).ToList();

                            if (oMCETList_Dup.Count == 0)
                            {
                                stepIndexLowest = oCLARD_NVP.OrderIndex < stepIndexLowest ? oCLARD_NVP.OrderIndex : stepIndexLowest;
                                ///
                                oMCETList.Add(

                                    new MemberChurchlifeEventTask()
                                    {
                                        AppGlobalOwnerId = oCAA_CM.AppGlobalOwnerId,
                                        ChurchBodyId = oCAA_CM.ChurchBodyId,
                                        ChurchMemberId = oCAA_CM.Id,
                                        MemberChurchlifeActivityId = oCAA_MCLAc.Id,
                                        TaskStatus = "P",   // Pending
                                        SharingStatus = "A",
                                        DateCommenced = DateTime.Now,
                                        // DateCompleted = DateTime.Now
                                        Created = tm,
                                        LastMod = tm,
                                        CreatedByUserId = this._oLoggedUser.Id,
                                        LastModByUserId = this._oLoggedUser.Id
                                    });
                            }
                        }

                        foreach (MemberChurchlifeEventTask oMCET in oMCETList)
                        {
                            oMCET.IsCurrentTask = oMCET.OrderIndex <= stepIndexLowest;  //concurrent will be handled  
                            _context.Add(oMCET);
                        }
                    }
                }

                // save the batch of MCET
                if (oMCLAcList.Count > 0 || oMCETList.Count > 0) 
                { 
                    _context.SaveChanges(); 
                    updChangesMade = false; 
                }
                 

                /// MCM..MT
                /// 
                var oMCM_MT_BD = _context.MemberType.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.ChurchBodyId == oCAA_CM.ChurchBodyId && c.ChurchMemberId == oCAA_CM.Id).FirstOrDefault();
                if (oMCM_MT_BD == null )
                {
                    //GET THE LOWEST TYPE...
                    oMCM_MT_BD = new MemberType()
                    {
                        AppGlobalOwnerId = oCAA_CM.AppGlobalOwnerId,
                        ChurchBodyId = oCAA_CM.ChurchBodyId,
                        ChurchMemberId = (int)oCAA_CM.Id,
                        MemberTypeCode = "N",  /// New Convert
                        IsCurrent = true,
                        SharingStatus = "A",
                        FromDate = DateTime.Now,
                        // ToDate = DateTime.Now
                        Created = tm,
                        CreatedByUserId = this._oLoggedUser.Id,
                        LastMod = tm,
                        LastModByUserId = this._oLoggedUser.Id,
                    };

                    _context.MemberType.Add(oMCM_MT_BD);
                    updChangesMade = true;
                }


                /// MCM..MR
                /// 
                var oMCM_MR_BD = _context.MemberRank.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.ChurchBodyId == oCAA_CM.ChurchBodyId && c.ChurchMemberId == oCAA_CM.Id).FirstOrDefault();
                if (oMCM_MR_BD == null )
                {
                    //GET THE LOWEST RANK...
                    oMCM_MR_BD = new MemberRank()
                    {
                        AppGlobalOwnerId = oCAA_CM.AppGlobalOwnerId,
                        ChurchBodyId = oCAA_CM.ChurchBodyId,
                        ChurchMemberId = (int)oCAA_CM.Id,
                        ChurchRankId = (int)_oChangesCAA.ChurchRankId,
                        IsCurrentRank = true,
                        SharingStatus = "A",
                        FromDate = DateTime.Now,
                        // ToDate = DateTime.Now
                        Created = tm,
                        CreatedByUserId = this._oLoggedUser.Id,
                        LastMod = tm,
                        LastModByUserId = this._oLoggedUser.Id,
                    };

                    _context.MemberRank.Add(oMCM_MR_BD);
                    updChangesMade = true;
                }


                /// MCM..MS
                /// 
                var oMCM_MS_BD = _context.MemberStatus.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCAA_CM.AppGlobalOwnerId && c.ChurchBodyId == oCAA_CM.ChurchBodyId && c.ChurchMemberId == oCAA_CM.Id).FirstOrDefault();
                if (oMCM_MS_BD == null )
                {
                    oMCM_MS_BD = new MemberStatus()
                    {
                        //GET THE MOST APPLICABLE STATUS...
                        AppGlobalOwnerId = oCAA_CM.AppGlobalOwnerId,
                        ChurchBodyId = oCAA_CM.ChurchBodyId,
                        ChurchMemberId = (int)oCAA_CM.Id,
                        ChurchMemStatusId = _oChangesCAA.ChurchMemStatusId,
                        IsCurrent = true,
                        SharingStatus = "A",
                        FromDate = DateTime.Now,
                        // ToDate = DateTime.Now
                        Created = tm,
                        CreatedByUserId = this._oLoggedUser.Id,
                        LastMod = tm,
                        LastModByUserId = this._oLoggedUser.Id,
                    };

                    _context.MemberStatus.Add(oMCM_MS_BD);
                    updChangesMade = true;
                }




                //save... all
                if (updChangesMade)
                    _context.SaveChanges();

                var strDesc = "New Converts";
                var _userTask = "Added New Convert, " + (!string.IsNullOrEmpty(strMemName) ? "[" + strMemName + "]" : "") + " successfully. Other church member modules have been configured alogside. Note: Final approval may be required after the attached activity process is complete.";

                //audit...
                var _tm = DateTime.Now;
                //await this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                //                 "RCMS-Client: Church Member", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vmMod.oCurrUserId_Logged, _tm, _tm, vmMod.oCurrUserId_Logged, vmMod.oCurrUserId_Logged));


                //_userTask = "Opened " + strDesc.ToLower() + " member biodata";
                //if (oMBModel.oChurchMember.Id > 0) _userTask += "[" + _strMemFullName + " | Member code: " + oMBModel.oChurchMember.GlobalMemberCode + "]";

                // register @MSTR
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id));

                //register @CLNT
                _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, null, null, "T",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id));
                 
                 
                 
                // DisplaySuccessInfo(res.userMess, res.currTask, res.oCurrId, currChuBodyId, _strAttendee, _strLongevity, res.evtId, res.evtDate, false);
                return Json(new { taskSuccess = true, oCurrId = _oChangesCAA.Id, userMess = _userTask });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChangesCAA.Id, userMess = "Failed saving church attendance (attendee) details. Err: " + ex.Message });
            }
        }


        public async Task<IActionResult> Delete_Attendees(int? id)
        {
            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser?.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }


            var res = false;
            var churchAttend = await _context.ChurchAttendAttendee.FindAsync(id);
            if (churchAttend != null)
            {
                //check all member related modules for references to deny deletion

                res = true;
                if (res)
                {
                    _context.ChurchAttendAttendee.Remove(churchAttend);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError(churchAttend.Id.ToString(), "Delete failed. Church attendance (attendee) data is referenced elsewhere in the Application.");
                    ViewBag.UserDelMsg = "Delete failed. Church attendance (attendee) data is referenced elsewhere in the Application.";
                }

                return Json(res);
            }

            else
                return Json(false);
        }




    }
}
