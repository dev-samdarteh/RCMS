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

namespace RhemaCMS.Controllers.con_ch_fin
{
    public class FinanceOperationsController : Controller
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
         
        private List<DiscreteLookup> dlPeriodTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlIntervalFreqs = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlSemesters = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlQuarters = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlMonths = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlDays = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlActivityTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlGenderStatuses = new List<DiscreteLookup>(); 
        private List<DiscreteLookup> dlPmntModes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlTitheModes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlTitherScopes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlPostModes = new List<DiscreteLookup>();

        public FinanceOperationsController(MSTR_DbContext masterContext, IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, ChurchModelContext clientCtx)
        {
            try
            {
                _hostingEnvironment = hostingEnvironment;
                _masterContext = masterContext;

                _httpContextAccessor = httpContextAccessor;
                _tempDataDictionaryFactory = tempDataDictionaryFactory;


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


                // _context = context;
                //  this._context = clientCtx;
                if (clientCtx == null)
                    _context = GetClientDBContext();

                else
                {
                    var cs = clientCtx.Database.GetDbConnection().ConnectionString;
                    if (string.IsNullOrEmpty(cs))
                        _context = GetClientDBContext();
                    else
                    {
                        var conn = new SqlConnectionStringBuilder(cs);
                        if (conn.DataSource == "_BLNK" || conn.InitialCatalog == "_BLNK") // (string.IsNullOrEmpty(this._clientDBConnString) || this.oUserLogIn_Priv.UserProfile == null)
                            _context = GetClientDBContext();
                        else
                        {
                            _context = clientCtx;
                            this._clientDBConnString = clientCtx.Database.GetDbConnection().ConnectionString;
                        }
                    }
                }


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



                /// synchronize AGO, CL, CB, CTRY  or @login 
                // this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

                /// get the localized data... using the MSTR data
                if (_context != null)
                {
                    this._oLoggedAGO = _context.AppGlobalOwner.AsNoTracking()
                                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...

                    this._oLoggedCB = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                                        .Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();
                }


            }
            catch (Exception ex)
            {
                RedirectToAction("LoginUserAcc", "UserLogin");
            }



            ////load the dash
            //LoadClientDashboardValues();



            ///
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "A", Desc = "Active" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "P", Desc = "Pending" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "B", Desc = "Blocked" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "D", Desc = "Deactive" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "E", Desc = "Expired" });
             

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


            //dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "A", Desc = "Active" });
            //dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "S", Desc = "Draft" });  //Saved Copy
            //dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "D", Desc = "Deactive" });
            //dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "E", Desc = "Expired" });

            //SharingStatus { get; set; }  // A - Share with all sub-congregations, C - Share with child congregations only, N - Do not share
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "N", Desc = "Do not roll-down (share)" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "C", Desc = "Roll-down (share) for direct child congregations" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "A", Desc = "Roll-down (share) for all sub-congregations" });


            dlPmntModes.Add(new DiscreteLookup() { Category = "PmntMode", Val = "CSH", Desc = "Cash" });
            dlPmntModes.Add(new DiscreteLookup() { Category = "PmntMode", Val = "CHQ", Desc = "Cheque" });
            dlPmntModes.Add(new DiscreteLookup() { Category = "PmntMode", Val = "MM", Desc = "Mobile Money" });
            dlPmntModes.Add(new DiscreteLookup() { Category = "PmntMode", Val = "POS", Desc = "POS" });
            dlPmntModes.Add(new DiscreteLookup() { Category = "PmntMode", Val = "O", Desc = "Other" });

            // TitheMode { get; set; }   // M-Member-Specific, G-Church Group, E-External Body, A-Anonymous
            dlTitheModes.Add(new DiscreteLookup() { Category = "TitheMode", Val = "A", Desc = "Anonymous" });
            dlTitheModes.Add(new DiscreteLookup() { Category = "TitheMode", Val = "M", Desc = "Member-based" });
            dlTitheModes.Add(new DiscreteLookup() { Category = "TitheMode", Val = "G", Desc = "Corporate/Group" });
            dlTitheModes.Add(new DiscreteLookup() { Category = "TitheMode", Val = "O", Desc = "Open" });

            // TitherScope L-Tither from local congregation, D-Tither from denomination, E-Corporate /Group 
            dlTitherScopes.Add(new DiscreteLookup() { Category = "TitherScope", Val = "L", Desc = "Local congregation" });
            dlTitherScopes.Add(new DiscreteLookup() { Category = "TitherScope", Val = "D", Desc = "Denomination" });
            dlTitherScopes.Add(new DiscreteLookup() { Category = "TitherScope", Val = "E", Desc = "External" });

            ///O-pen P-osted to GL C-ancelled
            dlPostModes.Add(new DiscreteLookup() { Category = "PostMode", Val = "O", Desc = "Open" });
            dlPostModes.Add(new DiscreteLookup() { Category = "PostMode", Val = "P", Desc = "Posted" });
            dlPostModes.Add(new DiscreteLookup() { Category = "PostMode", Val = "R", Desc = "Reversed" });
        }


        public static string GetStatusDesc(string oCode)
        {
            switch (oCode)
            {
                case "A": return "Active";
                case "S": return "Draft";
                case "D": return "Deactive";
                case "P": return "Pending";
                case "E": return "Expired";
                case "C": return "Closed";


                default: return oCode;
            }
        }

        public static string GetPostStatusDesc(string oCode)
        {
            switch (oCode)
            {
                case "O": return "Open";
                case "P": return "Posted to GL";
                case "R": return "Revesed";
                // case "C": return "Cancelled";


                default: return oCode;
            }
        }

        public static string GetTitheModeDesc(string oCode)
        {
            switch (oCode)
            {
                case "A": return "Anonymous";
                case "M": return "Member-based";
                case "G": return "Corporate /Group";
                case "O": return "Open"; //Any stream
                // case "C": return "Cancelled";


                default: return oCode;
            }
        }

        public static string GetTitherScopeDesc(string oCode)
        {
            switch (oCode)
            {
                case "L": return "Local congregation";
                case "D": return "Denomination";
                case "E": return "External";


                default: return oCode;
            }
        }

        public string GetConcatMemberName(string title, string fn, string mn, string ln, bool displayName = false)
        {
            if (displayName)
                return ((((!string.IsNullOrEmpty(title) ? title : "") + ' ' + fn).Trim() + " " + mn).Trim() + " " + ln).Trim();
            else
                return (((fn + ' ' + mn).Trim() + " " + ln).Trim() + " " + (!string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
        }

        public static string StaticGetConcatMemberName(string title, string fn, string mn, string ln, bool displayName = false)
        {
            if (displayName)
                return ((((!string.IsNullOrEmpty(title) ? title : "") + ' ' + fn).Trim() + " " + mn).Trim() + " " + ln).Trim();
            else
                return (((fn + ' ' + mn).Trim() + " " + ln).Trim() + " " + (!string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
        }

        public static string GetDayOfWeeksDesc(string oCode, bool days = false)
        {
            switch (oCode)
            {
                case "Su": return "Sunday";
                case "Mo": if (days) return "Monday"; else return "Monthly";
                case "Tu": return "Tuesday";
                case "We": return "Wednesday";
                case "Th": return "Thursday";
                case "Fr": return "Friday";
                case "Sa": return "Saturday";

                case "Da": return "Daily";
                case "Wk": return "Weekly";
                case "Bw": return "Bi-Weekly";
                //case "Mo": return "Monthly";
                case "Bm": return "Bi-Monthly";
                case "Qt": return "Quarterly";
                case "Yr": return "Yearly";

                default: return oCode;
            }
        }
         
        public static string GetPeriodDesc(string oCode)
        {
            switch (oCode)
            {
                case "AP": return "Accounting Period";
                case "CP": return "Church Calendar Period";

                case "Y": return "Year";
                case "S": return "Semester";
                case "M": return "Month";
                case "W": return "Week";
                case "D": return "Day";

                case "S1": return "Semester-1";
                case "S2": return "Semester-2";
                case "Q1": return "Quarter-1";
                case "Q2": return "Quarter-2";
                case "Q3": return "Quarter-3";
                case "Q4": return "Quarter-4";
                case "M1": return "January";
                case "M2": return "February";
                case "M3": return "March";
                case "M4": return "April";
                case "M5": return "May";
                case "M6": return "June";
                case "M7": return "July";
                case "M8": return "August";
                case "M9": return "September";
                case "M10": return "October";
                case "M11": return "November";
                case "M12": return "December";

                default: return oCode;
            }
        }
         
        private ChurchModelContext GetClientDBContext() //UserProfile oUserLogged = null)
        {
            var isAuth = this.oUserLogIn_Priv != null;
            if (!isAuth) isAuth = SetUserLogged();
            //else
            //{
            if (!isAuth)
            {
                RedirectToAction("LoginUserAcc", "UserLogin"); return null;
            }
            else
            {
                if (this.oUserLogIn_Priv == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin"); return null;
                }
                else
                {
                    if (this.oUserLogIn_Priv.UserProfile == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin"); return null;
                    }
                    else
                    {
                        var oClientConfig = _masterContext.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                        //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == 4 && c.Status == "A").FirstOrDefault();
                        if (oClientConfig != null)
                        {
                            //// get and mod the conn
                            //var _clientDBConnString = "";
                            //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
                            //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                            //_clientDBConnString = conn.ConnectionString;

                            //// test the NEW DB conn
                            //var _clientContext = new ChurchModelContext(_clientDBConnString);

                            // var _clientDBConnString = "";
                            var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
                            conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
                            conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
                            conn.IntegratedSecurity = false; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

                            this._clientDBConnString = conn.ConnectionString;

                            // test the NEW DB conn
                            var _clientContext = new ChurchModelContext(_clientDBConnString);

                            if (!_clientContext.Database.CanConnect())
                                RedirectToAction("LoginUserAcc", "UserLogin");

                            //// _oLoggedRole = oUserLogIn_Priv.UserRole; 
                            //this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
                            //this._oLoggedCB_MSTR = this.oUserLogIn_Priv.ChurchBody;
                            //this._oLoggedAGO_MSTR = this.oUserLogIn_Priv.AppGlobalOwner;
                            //this._oLoggedUser.strChurchCode_AGO = this._oLoggedAGO_MSTR != null ? this._oLoggedAGO_MSTR.GlobalChurchCode : "";
                            //this._oLoggedUser.strChurchCode_CB = this._oLoggedCB_MSTR != null ? this._oLoggedCB_MSTR.GlobalChurchCode : "";

                            ///// synchronize AGO, CL, CB, CTRY  or @login 
                            //// this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

                            ///// get the localized data... using the MSTR data
                            //this._oLoggedAGO = _clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
                            //this._oLoggedCB = _clientContext.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId &&
                            //                        c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();


                            // load the dash b/f
                            // LoadClientDashboardValues();

                            return _clientContext;
                        }
                        else
                        { // db config not found             
                            RedirectToAction("LoginUserAcc", "UserLogin"); return null;
                        }
                    }
                }
            }
        }

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
        

        private async Task LoadClientDashboardValues(string clientDBConnString) //, UserProfile oLoggedUser)
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


            ViewData["CB_SubCongCount"] = String.Format("{0:N0}", 50);
            ViewData["CB_MemListCount"] = String.Format("{0:N0}", 50); // res[0].cnt_tcm); //
            ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 50); // res[0].cnt_tsubs);
            ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 75); //res[0].cnt_tdb);
            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 50); //res[0].cnt_tcln_a);
            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 50); //res[0].cnt_tcln_d);
            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 50); //res[0].cnt_tcln_d); 
            ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 50);
             
        }

        private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail)
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // var tempCtx = _context;
                using (var logCtx = new MSTR_DbContext(_masterContext.Database.GetDbConnection().ConnectionString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
                {
                    if (logCtx.Database.CanConnect() == false) logCtx.Database.OpenConnection();
                    else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open) logCtx.Database.OpenConnection();

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

        private async Task LogUserActivity_ClientUserAuditTrail(UserAuditTrail_CL oUserTrail, string clientDBConnString)
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // var tempCtx = _context;
                if (!string.IsNullOrEmpty(clientDBConnString))
                {
                    using (var logCtx = new ChurchModelContext(clientDBConnString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
                    {
                        //logCtx = _context;
                        //var conn = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_context.Database.GetDbConnection().ConnectionString);
                        ////  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
                        //conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                        /////
                        //logCtx.Database.GetDbConnection().ConnectionString = conn.ConnectionString;

                        if (logCtx.Database.CanConnect() == false) logCtx.Database.OpenConnection();
                        else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open) logCtx.Database.OpenConnection();

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
                }
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

        private static bool IsAncestor_ChurchBody(ChurchBody oAncestorChurchBody, ChurchBody oCurrChurchBody)  // Ancestor of ? ... Taifa -- Grace. swapped -->> Descendant of ?
        {
            if (oAncestorChurchBody == null || oCurrChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

            if (oAncestorChurchBody.Id == oCurrChurchBody.ParentChurchBodyId) return true;
            if (string.IsNullOrEmpty(oAncestorChurchBody.RootChurchCode) || string.IsNullOrEmpty(oCurrChurchBody.RootChurchCode)) return false;
            if (string.Compare(oAncestorChurchBody.RootChurchCode, oCurrChurchBody.RootChurchCode) == 0) return true;  // same CB .. owned

            string[] arr = new string[] { oCurrChurchBody.RootChurchCode };
            if (oCurrChurchBody.RootChurchCode.Contains("--")) arr = oCurrChurchBody.RootChurchCode.Split("--");  // else it should be the ROOT... and would not get this far

            if (arr.Length > 0)
            {
                var ancestorCode = oAncestorChurchBody.RootChurchCode;
                var tempCode = oCurrChurchBody.RootChurchCode;

                if (string.Compare(ancestorCode, tempCode) == 0) return true;
                var k = arr.Length - 1;
                for (var i = arr.Length - 1; i >= 0; i--)
                {
                    if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
                    if (string.Compare(ancestorCode, tempCode) == 0) return true;
                }
            }

            return false;
        }

        private static bool IsAncestor_ChurchBody(string strAncestorRootCode, string strCurrChurchBodyRootCode, int? ancestorChurchBodyId = null, int? currChurchBodyId = null)
        {
            // if (oAncestorChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

            if (currChurchBodyId != null && ancestorChurchBodyId == currChurchBodyId) return true;

            if (string.IsNullOrEmpty(strAncestorRootCode) || string.IsNullOrEmpty(strCurrChurchBodyRootCode)) return false;
            if (string.Compare(strAncestorRootCode, strCurrChurchBodyRootCode) == 0) return true;

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

        private static bool IsDescendant_ChurchBody(ChurchBody oDescendantChurchBody, ChurchBody oCurrChurchBody)  // Ancestor of ? ... Taifa -- Grace. swapped -->> Descendant of ?
        {
            if (oDescendantChurchBody == null || oCurrChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

            if (oDescendantChurchBody.ParentChurchBodyId == oCurrChurchBody.Id) return true;  // father /child
            if (string.IsNullOrEmpty(oDescendantChurchBody.RootChurchCode) || string.IsNullOrEmpty(oCurrChurchBody.RootChurchCode)) return false;
            if (string.Compare(oDescendantChurchBody.RootChurchCode, oCurrChurchBody.RootChurchCode) == 0) return true;  // same CB .. owned

            string[] arr = new string[] { oDescendantChurchBody.RootChurchCode };
            if (oDescendantChurchBody.RootChurchCode.Contains("--")) arr = oDescendantChurchBody.RootChurchCode.Split("--");  // else it should be the ROOT... and would not get this far

            if (arr.Length > 0)
            {
                var ancestorCode = oCurrChurchBody.RootChurchCode;
                var tempCode = oDescendantChurchBody.RootChurchCode;

                if (string.Compare(ancestorCode, tempCode) == 0) return true;   // same CB .. owned
                var k = arr.Length - 1;
                for (var i = arr.Length - 1; i >= 0; i--)
                {
                    if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
                    if (string.Compare(ancestorCode, tempCode) == 0) return true;
                }
            }

            return false;
        }

        private static bool IsDescendant_ChurchBody(string strDescendantRootCode, string strCurrChurchBodyRootCode, int? descendantChurchBodyId = null, int? currChurchBodyId = null)
        {
            // if (oAncestorChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

            if (currChurchBodyId != null && descendantChurchBodyId == currChurchBodyId) return true; // same CB

            if (string.IsNullOrEmpty(strDescendantRootCode) || string.IsNullOrEmpty(strCurrChurchBodyRootCode)) return false;
            if (string.Compare(strDescendantRootCode, strCurrChurchBodyRootCode) == 0) return true;  // same CB

            string[] arr = new string[] { strDescendantRootCode };
            if (strDescendantRootCode.Contains("--")) arr = strDescendantRootCode.Split("--");

            if (arr.Length > 0)
            {
                var ancestorCode = strCurrChurchBodyRootCode; // // same CB .. owned
                var tempCode = strDescendantRootCode;

                var k = arr.Length - 1;
                for (var i = arr.Length - 1; i >= 0; i--)
                {
                    if (tempCode.Contains("--" + arr[i])) tempCode = tempCode.Replace("--" + arr[i], "");
                    if (string.Compare(ancestorCode, tempCode) == 0) return true;
                }
            }

            return false;
        }

        public ActionResult Index_TT(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, bool loadLim = false)
        {
            try
            {
                if (this._context == null)
                {
                    this._context = GetClientDBContext();
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return View("_ErrorPage");
                    }
                }

                if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                { RedirectToAction("LoginUserAcc", "UserLogin"); }

                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);

                if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCurrChuBodyId != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


                var oTTModelList = (
                   from t_tt in _context.TitheTrans.AsNoTracking() //.Include(t => t.ChurchMember)
                                .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId)
                   from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_tt.ChurchBodyId && c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId)   //c.Id == oChurchBodyId && 
                   from t_ap in _context.ChurchPeriod.AsNoTracking().Include(t => t.AppGlobalOwner) .Where(c => c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId && c.Id == t_tt.ChurchPeriodId)   // from t_an in _context.AppUtilityNVP.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.TitheModeId).DefaultIfEmpty()
                   from t_cb_corp in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Where(c => c.Id == t_tt.Corporate_ChurchBodyId && c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId).DefaultIfEmpty()
                   from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.ChurchMemberId).DefaultIfEmpty()
                   // from t_curr in _context.Country.AsNoTracking().Where(c => c.Curr3LISOSymbol == t_tt.Curr3LISOSymbol).DefaultIfEmpty()

                   select new TitheModel()
                   {
                       oTitheTrans = t_tt,
                       oAppGlolOwnId = t_tt.AppGlobalOwnerId,
                       oAppGlolOwn = t_tt.AppGlobalOwner,
                       oChurchBodyId = t_tt.ChurchBodyId,
                       oChurchBody = t_tt.ChurchBody,
                       strChurchBody = t_tt.ChurchBody != null ? t_tt.ChurchBody.Name : "",
                       //                       
                       strTithedBy = t_tt.TitheMode == "M" ? (t_cm != null ? StaticGetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, true) : t_tt.TitherDesc) :
                                                                    (t_tt.TitheMode == "C" ? (t_cb_corp != null ? t_cb_corp.Name : t_tt.TitherDesc) : t_tt.TitherDesc),
                       strTitheMode = GetTitheModeDesc(t_tt.TitheMode),
                       strTitherScope = GetTitherScopeDesc(t_tt.TithedByScope),
                       // strRelatedEvent = t_cce != null ? t_cce.Subject : "",
                       strAccountPeriod = t_ap != null ? t_ap.PeriodDesc : "",
                       strCurrency = t_tt.Curr3LISOSymbol,  // t_curr != null ? t_curr.Curr3LISOSymbol : "",

                       //
                       strAmount = String.Format("{0:N2}", t_tt.TitheAmount),
                       dt_TitheDate = t_tt.TitheDate,
                       strTitheDate = t_tt.TitheDate != null ? String.Format("{0:d MMM yyyy}", t_tt.TitheDate.Value) : "",
                       strPostDate = t_tt.PostedDate != null ? String.Format("{0:d MMM yyyy}", t_tt.PostedDate.Value) : "",
                       strPostStatus = GetPostStatusDesc(t_tt.PostStatus),
                       strStatus = GetStatusDesc(t_tt.Status)
                   }
                   )
                  // .OrderByDescending(c => c.dt_TitheDate)
                   .ToList();

                if (oTTModelList.Count > 0) oTTModelList = oTTModelList.OrderByDescending(c => c.dt_TitheDate).ToList(); //.ThenBy(c => c.strMemberFullName)

                var oTTModel = new TitheModel();
                oTTModel.lsTitheModels = oTTModelList;

                //
                oTTModel.oAppGlolOwnId = oAppGloOwnId;
                oTTModel.oChurchBodyId = oCurrChuBodyId;
                //
                oTTModel.oUserId_Logged = _oLoggedUser.Id;
                oTTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                oTTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;

                //oTTModel.oMemberId_Logged = oCurrChuMemberId_LogOn;
                //
                oTTModel.setIndex = setIndex; 

                //oTTModel.oAppGloOwnId = this._oLoggedAGO.Id; oCMModel.oAppGlobalOwn = this._oLoggedAGO;
                //oTTModel.oChurchBodyId = oCBid; oCMModel.oChurchBody = oCB;
                //oTTModel.strChurchBody = oCB.Name;
                ///
                //oTTModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                //oTTModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                oTTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oTTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;


                oTTModel.oUserId_Logged = _oLoggedUser.Id;



                //var oCMModel = new ChurchMemberSummaryModel();

                //// get summary data -- total, males, females...
                //oCMModel.numTotalRoll = oCMList.Count;
                //oCMModel.numTotalRoll_M = oCMList.Count(c => c.oChurchMember.Gender == "M");
                //oCMModel.numTotalRoll_F = oCMList.Count(c => c.oChurchMember.Gender == "F");
                //oCMModel.numTotalRoll_O = oCMList.Count - (oCMModel.numTotalRoll_M + oCMModel.numTotalRoll_F);

                //oCMModel.oAppGloOwnId = this._oLoggedAGO.Id; oCMModel.oAppGlobalOwn = this._oLoggedAGO;
                //oCMModel.oChurchBodyId = oCBid; oCMModel.oChurchBody = oCB;
                //oCMModel.strChurchBody = oCB.Name;
                /////
                //oCMModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                //oCMModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                //oCMModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                //oCMModel.oChurchBodyId_Logged = this._oLoggedCB.Id;


                //oCMModel.oUserId_Logged = _oLoggedUser.Id;


                //oCMModel.pageIndex = 1;
                //oCMModel.filterIndex = filterIndex;
                // oCMModel.setIndex = (int)setIndex;
                // oCMModel.subSetIndex = (int)subSetIndex;

                /// 
                oTTModel.lsTitheModels = oTTModelList;
                ViewData["oCMModel_List"] = oTTModel.lsTitheModels;

                var strDesc = "Church Member";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                oTTModel.strCurrTask = strDesc;


                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oTTModel);
                TempData["oVmCSPModel"] = _oUPModel; TempData.Keep();

                if (loadLim)
                    return PartialView("_vwIndexTT", oTTModel);
                else
                    return View("IndexTT", oTTModel);
            }

            catch (Exception ex)
            {
                return View("_ErrorPage");
            }
        }


        [HttpGet]
        public IActionResult AddOrEdit_TT(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0,
                                             int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) //, int pageIndex = 1)
        {   //int subSetIndex = 0,

            try
            {
                if (this._context == null)
                {
                    this._context = GetClientDBContext();
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return View("_ErrorPage");
                    }
                }


                //if (!InitializeUserLogging())
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                // Client
                if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                // MSTR
                var oUserId = this._oLoggedUser.Id;
                var oAGO_MSTR = this._oLoggedAGO_MSTR; var oCB_MSTR = this._oLoggedCB_MSTR; // _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId); 
                var oAGO = this._oLoggedAGO;
                var oCB = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                if (oCB == null) oCB = this._oLoggedCB;

                if (oAGO_MSTR == null || oCB_MSTR == null || oAGO == null || oCB == null)   // || oCU_Parent == null church units may be networked...
                { return View("_ErrorPage"); }

                var strDesc = "Church Member";
                var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();



                var oTTModel = new TitheModel();

                //var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                //                    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                //oCP_List_1 = oCP_List_1.Where(c =>
                //                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                if (id == 0)
                {
                    var oTransTITHE = new TitheTrans();

                    // this attr is used by most of the models... save in memory [class var]
                    this.oCTRYDefault = _context.CountryCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsDefaultCountry == true).FirstOrDefault();
                    this.oCURRDefault = _context.CurrencyCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsBaseCurrency == true).FirstOrDefault();
                    ///
                    var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody).ThenInclude(t=> t.ChurchLevel) //
                                    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                    oCP_List_1 = oCP_List_1.Where(c =>
                                       (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    /// may be more... pick the one up-most ... override lower cong!    private ChurchPeriod oCPRDefault;
                    oCP_List_1 = oCP_List_1.OrderBy(c => c.OwnedByChurchBody?.ChurchLevel?.LevelIndex).ToList();
                    this.oCPRDefault = oCP_List_1.FirstOrDefault();// _context.ChurchPeriod.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.Status == "A").FirstOrDefault();  // c.PeriodType == "CP" && 

                    ///
                    oTransTITHE.AppGlobalOwnerId = oAppGloOwnId;
                    oTransTITHE.ChurchBodyId = oCurrChuBodyId;

                    oTransTITHE.ChurchPeriodId = this.oCPRDefault != null ? this.oCPRDefault.Id : (int?)null; // accPer != null ? accPer.Id : (int?)null;
                                                                                                              //var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsBaseCurrency == true).FirstOrDefault();
                    ///
                    oTransTITHE.Curr3LISOSymbol = this.oCURRDefault != null ? this.oCURRDefault.Country.Curr3LISOSymbol : null; // curr != null ? curr.Id : (int?)null;
                    oTransTITHE.CtryAlpha3Code = this.oCURRDefault != null ? this.oCURRDefault.Country.CtryAlpha3Code : null; // curr != null ? curr.Id : (int?)null;
                    oTransTITHE.TithedByScope = "L";     // Local cong            
                    oTransTITHE.TitheMode = "A";     //  Anonymous           
                    oTransTITHE.PostStatus = "O";     // O-Open, P-Posted to GL, R-Reversed            
                    oTransTITHE.Status = "A"; //Active 
                    oTransTITHE.TitheDate = DateTime.Now;
                    oTransTITHE.ReceivedByUserId = oUserId_Logged;
                    ///
                    oTTModel.strCurrency = this.oCURRDefault != null ? (this.oCURRDefault.Country != null ? this.oCURRDefault.Country.Curr3LISOSymbol : "") : ""; // curr != null ? curr.Acronym : "";
                    oTTModel.strPostStatus = GetPostStatusDesc(oTransTITHE.PostStatus);
                    if (oTransTITHE != null)
                    {
                        var oUser = _masterContext.UserProfile.Where(c => c.Id == oTransTITHE.ReceivedByUserId).FirstOrDefault();
                        if (oUser != null) oTTModel.strReceivedBy = oUser.UserDesc;
                    }

                    oTTModel.oTitheTrans = oTransTITHE;
                }

                else
                {
                    var TitheMdl = (
                       from t_tt in _context.TitheTrans.AsNoTracking() //.Include(t => t.ChurchMember)
                                    .Where(c => c.Id == id && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId)
                       from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
                                .Where(c => c.Id == t_tt.ChurchBodyId && c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId)   //c.Id == oChurchBodyId && 
                   from t_ap in _context.ChurchPeriod.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId && c.Id == t_tt.ChurchPeriodId).DefaultIfEmpty()   //_context.ChurchPeriod.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.AccountPeriodId) .DefaultIfEmpty()  //c.Id == oChurchBodyId &&   // from t_an in _context.AppUtilityNVP.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.TitheModeId).DefaultIfEmpty()
                   from t_cb_corp in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
                            .Where(c => c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId && c.Id == t_tt.Corporate_ChurchBodyId).DefaultIfEmpty()
                       from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.ChurchMemberId).DefaultIfEmpty()
                           // from t_curr in _context.Currency.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.CurrencyId).DefaultIfEmpty()

                   select new TitheModel()
                       {
                           oTitheTrans = t_tt,
                           oAppGlolOwnId = t_tt.AppGlobalOwnerId,
                           oAppGlolOwn = t_cb.AppGlobalOwner,
                           oChurchBodyId = t_tt.ChurchBodyId,
                           oChurchBody = t_cb,
                       //                      
                       strTithedBy = t_tt.TitheMode == "M" ? (t_cm != null ? StaticGetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, true) : t_tt.TitherDesc) :
                                                                        (t_tt.TitheMode == "C" ? (t_cb_corp != null ? t_cb_corp.Name : t_tt.TitherDesc) : t_tt.TitherDesc),
                           strTitheMode = GetTitheModeDesc(t_tt.TitheMode),
                           strTitherScope = GetTitherScopeDesc(t_tt.TithedByScope),
                       // strRelatedEvent = t_cce != null ? t_cce.Subject : "",
                       strAccountPeriod = t_ap != null ? t_ap.PeriodDesc : "",
                           strCurrency = t_tt.Curr3LISOSymbol, // t_curr != null ? t_curr.Acronym : "",
                                                               //
                       strAmount = String.Format("{0:N2}", t_tt.TitheAmount),
                           dt_TitheDate = t_tt.TitheDate,
                           strTitheDate = t_tt.TitheDate != null ? DateTime.Parse(t_tt.TitheDate.ToString()).ToString("ddd, dd MMM yyyy", CultureInfo.InvariantCulture) : "",
                           strPostDate = t_tt.PostedDate != null ? DateTime.Parse(t_tt.PostedDate.ToString()).ToString("ddd, dd MMM yyyy", CultureInfo.InvariantCulture) : "",
                           strPostStatus = GetPostStatusDesc(t_tt.PostStatus),
                           strStatus = GetPostStatusDesc(t_tt.Status)
                       }
                       ).FirstOrDefault();

                    if (TitheMdl != null)
                        if (TitheMdl.oTitheTrans != null)
                        {
                            var oUser = _masterContext.UserProfile.Where(c => c.Id == TitheMdl.oTitheTrans.ReceivedByUserId).FirstOrDefault();
                            if (oUser != null) TitheMdl.strReceivedBy = oUser.UserDesc;
                        }

                    oTTModel = TitheMdl;
                }

                oTTModel.setIndex = setIndex;
                oTTModel.oUserId_Logged = oUserId_Logged;
                oTTModel.oAppGloOwnId_Logged = oAGOId_Logged;
                oTTModel.oChurchBodyId_Logged = oCBId_Logged;
                //
                oTTModel.oAppGlolOwnId = oAppGloOwnId;
                oTTModel.oChurchBodyId = oCurrChuBodyId;
                var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                oTTModel.oChurchBody = oCurrChuBody;  // != null ? oCurrChuBody : null;


                // ChurchBody oChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                oTTModel = this.populateLookups_TITHE(oTTModel, oCurrChuBody);
                // oCurrMdl.strCurrTask = "Church Tithe";

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oTTModel);
                TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

                return PartialView("_AddOrEdit_TT", oTTModel);

            }
            catch (Exception ex)
            {
                return PartialView("_ErrorPage");
            }
        }
                         
        private TitheModel populateLookups_TITHE(TitheModel vmLkp, ChurchBody oCurrChuBody)
        {
            if (vmLkp == null || oCurrChuBody == null) return vmLkp;
            //
            vmLkp.lkpStatuses = new List<SelectListItem>();
            foreach (var dl in dlGenStatuses) { vmLkp.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkpPaymentModes = new List<SelectListItem>();
            foreach (var dl in dlPmntModes) { vmLkp.lkpPaymentModes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkpTitheModes = new List<SelectListItem>();
            foreach (var dl in dlTitheModes) { vmLkp.lkpTitheModes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkpTitherScopes = new List<SelectListItem>();
            foreach (var dl in dlTitherScopes) { vmLkp.lkpTitherScopes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }


            //vmLkp.lkpTitheModes = _context.AppUtilityNVP.Where(c => c.NvpCode == "TITHE_TYP")
            //           .OrderBy(c => c.RequireUserCustom).ThenBy(c => c.OrderIndex).ThenBy(c => c.NvpValue)
            //           .ToList()
            //           .Select(c => new SelectListItem()
            //           {
            //               Value = c.Id.ToString(),
            //               Text = c.NvpValue
            //           })
            //           // .OrderBy(c => c.Text)
            //           .ToList();
            //vmLkp.lkpTitheModes.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            var chMemList = (from t_cm in _context.ChurchMember 
                             .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.ChurchBodyId == oCurrChuBody.Id && c.Status == "A")  //  && c.MemberClass == "C" 
                                                                //from t_ms in _context.MemberStatus.Where(c => c.ChurchBodyId == oCurrChuBody.Id && c.ChurchMemberId == t_cm.Id &&  //.Include(t => t.ChurchMemStatus)
                                                                //                                           c.IsCurrent == true && c.ChurchMemStatus.Available == true && c.ChurchMemStatus.Deceased == false)
                             select new ChurchMember
                             {
                                 Id = t_cm.Id,
                                 //FirstName = t_cm.FirstName,
                                 //MiddleName = t_cm.MiddleName ,
                                 // LastName = t_cm.LastName,
                                 GlobalMemberCode = t_cm.GlobalMemberCode,
                                 strMemFullName = t_cm != null ? StaticGetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false) : "",
                                 //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim()
                             })

                             // .OrderBy(c => c.strMemberName) //((c.FirstName + ' ' + c.MiddleName).Trim() + " " + c.LastName).Trim()) //strMemberName
                             .ToList();

            //  var chMemList = _context.ChurchMember.ToList();
            vmLkp.lkpChurchMembers_Local = chMemList
                                           .Select(c => new SelectListItem()
                                           {
                                               Value = c.Id.ToString(),
                                               Text = (c.GlobalMemberCode + " -- " + c.strMemFullName).Trim()
                                           })
                                           // .OrderBy(c => c.Text)
                                           .ToList();
            vmLkp.lkpChurchMembers_Local.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            //var attnDate = DateTime.Now;
            //var dtv = attnDate != null ? ((DateTime)attnDate).Date : (DateTime?)null;
            //vmLkp.lkpChurchEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchBody)  //.Include(t => t.RelatedChurchlifeActivity)
            //    .Where(c => !string.IsNullOrEmpty(c.Subject) && c.EventActive == true && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId //&&   //c.ChurchBody.CountryId == oCurrChuBody.CountryId && 
            //                                                                                                                                            //((DateTime)c.EventFrom.Value).Date == dtv &&

            //        //(c.EventFrom.HasValue ? ((DateTime)c.EventFrom.Value).Date==dts : c.EventFrom==dts) &&
            //        //((oAttendVM.m_DateAttended.HasValue==false && c.EventFrom == null) || 
            //        //   (oAttendVM.m_DateAttended.HasValue==true && ((DateTime)c.EventFrom).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt)) &&
            //        //(c.OwnedByChurchBodyId == oCurrChuBody.Id ||
            //        //(c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            //        //(c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
            //        )
            //         .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
            //         .ToList()
            //                 .Select(c => new SelectListItem()
            //                 {
            //                     Value = c.Id.ToString(),
            //                     Text = c.Subject + ":- " +
            //                                         (c.IsFullDay == true ?
            //                                             (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "").Trim() :
            //                                             (c.EventFrom != null ? DateTime.Parse(c.EventFrom.ToString()).ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "").Trim() +
            //                                             (c.EventFrom != null && c.EventTo != null ? " -- " : "") +
            //                                             (c.EventTo != null ? DateTime.Parse(c.EventTo.ToString()).ToString("dd MMM yyyy", CultureInfo.InvariantCulture) : "").Trim()
            //                                          )
            //                 })
            //                 //.OrderBy(c => c.Text)
            //                 .ToList();

            //vmLkp.lkpChurchEvents.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            //vmLkp.lkpCurrencies = _context.Currency.OrderBy(c => c.ChurchBodyId == oCurrChuBody.Id && c.Status == "A").ToList()
            //                               .Select(c => new SelectListItem()
            //                               {
            //                                   Value = c.Id.ToString(),
            //                                   Text = c.Acronym
            //                               })
            //                               .OrderBy(c => c.Text)
            //                               .ToList();

            // vmLkp.lkpCurrencies.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            var oCURR_List_1 = (from t_cc in _context.CurrencyCustom.AsNoTracking() // .Include(t => t.Country)
                                .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.ChurchBodyId == oCurrChuBody.Id && (c.IsBaseCurrency == true || c.IsDisplay == true)).ToList()
                              from t_ctry in _context.Country.AsNoTracking() // .Include(t => t.Country)
                              .Where(c => c.CtryAlpha3Code == t_cc.CtryAlpha3Code).ToList()
                              select t_ctry
                                ).ToList();
                
            if (oCURR_List_1.Count == 0)
            {
                oCURR_List_1 = ( from t_ctry in _context.Country.AsNoTracking() // .Include(t => t.Country) .Where(c => c.CtryAlpha3Code == t_cc.CtryAlpha3Code).ToList()
                              select t_ctry ).ToList();
            }

            vmLkp.lkpCurrencies = oCURR_List_1
                                .Where(c => !string.IsNullOrEmpty(c.Curr3LISOSymbol))
                                .OrderByDescending(c => c.CurrEngName) //.ThenByDescending(c => c.ToDate).ThenBy(c => c.PeriodDesc).ToList()
                                           .Select(c => new SelectListItem()
                                           {
                                               Value = c.Curr3LISOSymbol.ToString(),
                                               Text = c.Curr3LISOSymbol
                                           })
                                           // .OrderBy(c => c.Text)
                                           .ToList();

            //// ... AccessStatus = Open (O), Blocked (B), Closed (C)  .. Longevity = Current (C), Previous (P), History (H)
            //vmLkp.lkpAccountPeriods = _context.AccountPeriod.OrderBy(c => c.ChurchBodyId == oCurrChuBody.Id && c.PeriodStatus == "O")
            //                                .OrderBy(c => c.PeriodIndex)
            //                                .ToList()
            //                              .Select(c => new SelectListItem()
            //                              {
            //                                  Value = c.Id.ToString(),
            //                                  Text = c.PeriodDesc,
            //                                  Selected = c.LongevityStatus == "C"  //Current
            //                              })
            //                              //.OrderBy(c => c.Text)
            //                              .ToList();
            ////  vmLkp.lkpAccountPeriods.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                                .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

            oCP_List_1 = oCP_List_1.Where(c =>
                               (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

            vmLkp.lkpAccountPeriods = oCP_List_1
                                .Where(c => !string.IsNullOrEmpty(c.PeriodDesc))
                                .OrderByDescending(c => c.FromDate).ThenByDescending(c => c.ToDate).ThenBy(c => c.PeriodDesc).ToList()
                                           .Select(c => new SelectListItem()
                                           {
                                               Value = c.Id.ToString(),
                                               Text = c.PeriodDesc
                                           })
                                           // .OrderBy(c => c.Text)
                                           .ToList();
            // vmLkp.lkpChuMemTypes.Insert(0, new SelectListItem { Value = "", Text = "Select" });




            return vmLkp;
        }


        public JsonResult GetTitheModeByScope(string scope) //, bool addEmpty = false)
        {
            //var userPerms = (
            //            from t_upr in _context.UserRolePermission.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.Status == "A" && (userRoleId == null || (userRoleId != null && c.UserRoleId == userRoleId)))
            //            from t_up in _context.UserPermission.Where(c => c.ChurchBodyId == oCurrChuBodyId && c.PermStatus == "A" && c.Id == t_upr.UserRoleId)
            //            select t_up
            //                   ).OrderBy(c => c.PermissionCode).ToList()
            //                    .Select(c => new SelectListItem()
            //                    {
            //                        Value = c.Id.ToString(),
            //                        Text = c.PermissionName
            //                    })
            //                    .OrderBy(c => c.Text)
            //                    .ToList();


            var ls = new List<SelectListItem>();
            if (scope == "L")
            {
                foreach (var dl in dlTitheModes)
                {
                    if (dl.Val == "M" || dl.Val == "A" || dl.Val == "O") ls.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });
                }
            }
            else if (scope == "D" || scope == "E")
            {
                foreach (var dl in dlTitheModes)
                {
                    if (dl.Val == "G" || dl.Val == "A" || dl.Val == "O") ls.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });
                }
            }
            //else if (scope == "E")
            //{
            //    foreach (var dl in dlTitheModes)
            //    {
            //        if (dl.Val == "G" || dl.Val == "A" || dl.Val == "O") ls.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });
            //    }
            //}

            // if (addEmpty) ls.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(ls);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit_TT(TitheModel vmMod)
        {

            if (this._context == null)
            {
                this._context = GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }


            TitheTrans _oChanges = vmMod.oTitheTrans;   //  vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as TitheModel : vmMod; TempData.Keep();

            var arrData = ""; // TempData["oVmCurrMod"] as string;
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<TitheModel>(arrData) : vmMod;
            //
            var oMdlData = vmMod.oTitheTrans;
            oMdlData.ChurchBody = vmMod.oChurchBody;

            try
            {
                ModelState.Remove("oTitheTrans.AppGlobalOwnerId");
                ModelState.Remove("oTitheTrans.ChurchBodyId");
                ModelState.Remove("oTitheTrans.ChurchMemberId");
                ModelState.Remove("oTitheTrans.Corporate_ChurchBodyId");
                ModelState.Remove("oTitheTrans.TitheModeId");
                ModelState.Remove("oTitheTrans.RelatedEventId");
                ModelState.Remove("oTitheTrans.AccountPeriodId");
                ModelState.Remove("oTitheTrans.CurrencyId");

                ModelState.Remove("oTitheTrans.CreatedByUserId");
                ModelState.Remove("oTitheTrans.LastModByUserId");
                ModelState.Remove("oUserId_Logged");

                // ChurchBody == null 

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again." });

                if (_oChanges.TitheAmount == null || _oChanges.TitheAmount == 0) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Enter the tithe amount" });
                }
                if ( string.IsNullOrEmpty(_oChanges.Curr3LISOSymbol)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select currency." });
                }
                if (_oChanges.ChurchPeriodId == null) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select account period." });
                }
                if (_oChanges.TitheMode == "M" && _oChanges.ChurchMemberId == null)  // 
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify member paying tithe else change the tithe mode [hint: try choosing 'Anonymous']." });
                }
                if ((_oChanges.TithedByScope == "D" && _oChanges.TitheMode == "G" && _oChanges.Corporate_ChurchBodyId == null) ||
                    (_oChanges.TithedByScope == "E" && _oChanges.TitheMode == "G" && string.IsNullOrEmpty(_oChanges.TitherDesc)))
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify group or corporate body paying tithe else change the tithe mode [hint: try choosing 'Anonymous']." });
                }
                if (_oChanges.TitheMode == "O" && string.IsNullOrEmpty(_oChanges.TitherDesc))  // 
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify tither else change the tithe mode [hint: try choosing 'Anonymous']." });
                }
                if (_oChanges.TitheDate == null)  //Congregant... ChurchCodes required
                {
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the date." });
                }

                if (_oChanges.TitheDate != null)
                {
                    if (_oChanges.TitheDate.Value > DateTime.Now.Date)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Select the correct date. Date cannot be later than today." });

                    if (_oChanges.PostedDate != null)
                        if (_oChanges.TitheDate.Value > _oChanges.PostedDate.Value)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Payment date cannot be later than posting date" });
                }

                var _reset = _oChanges.Id == 0;

                //   _oChanges.LastMod = DateTime.Now;
                //  _oChanges.LastModByUserId = vmMod.oCurrUserId_Logged;

                //save for documents...
                // string uniqueFileName = null;
                //var oFormFile = vmMod.UserPhotoFile;
                //if (oFormFile != null && oFormFile.Length > 0)
                //{
                //    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img_db");  //~/frontend/dist/img_db
                //    uniqueFileName = Guid.NewGuid().ToString() + "_" + oFormFile.FileName;
                //    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                //    oFormFile.CopyTo(new FileStream(filePath, FileMode.Create));
                //}

                //else
                //    if (_oChanges.Id != 0) uniqueFileName = _oChanges.UserPhoto;

                //_oChanges.UserPhoto = uniqueFileName;



                var tm = DateTime.Now;
                _oChanges.LastMod = tm;
                _oChanges.LastModByUserId = vmMod.oUserId_Logged;
                _oChanges.ReceivedByUserId = vmMod.oUserId_Logged;

                //validate...
                if (_oChanges.Id == 0)
                {
                    _oChanges.Created = tm;
                    _oChanges.CreatedByUserId = vmMod.oUserId_Logged;
                    _context.Add(_oChanges);

                    ViewBag.UserMsg = "Saved tithe data successfully.";
                }
                else
                {
                    //retain the pwd details... hidden fields 
                    _context.Update(_oChanges);
                    ViewBag.UserMsg = "User tithe data updated successfully.";
                }

                //save user profile first... 
                _context.SaveChanges();  // await ... Async();

                /// update the tithe balance summary table... when new m added or triggered by user or at refresh 
                /// add new, upd [stat, dob, gen, group ... upd roll] and delete /transfer
                // if (_reset) {
                var resRollUpd = UpdCBTitheBal(_oChanges.AppGlobalOwnerId, _oChanges.ChurchBodyId, this._oLoggedUser.Id, _oChanges.Curr3LISOSymbol, _oChanges.CtryAlpha3Code);
                if (resRollUpd < 0) ViewBag.UserMsg += ". Tithe balance summary update failed. Try update again later.";
                else if (resRollUpd == 0) ViewBag.UserMsg += ". Tithe balance summary update incomplete. Try update again later.";
                // }

                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurrMod"] = _vmMod; TempData.Keep();

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, resetNew = _reset, userMess = ViewBag.UserMsg, lastCodeUsed = _oChanges.Curr3LISOSymbol });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving tithe details. Err: " + ex.Message });
            }

        }


        public int UpdCBTitheBal(int? oAGOid, int? oCBid = null, int? oUserId_Logged = null, string oCURRCode = null, string oCURRCtryCode = null)  // -1 - fail, 0 = incomplete, 1 - done
        { // jux update with data passed and return... 
            try
            {
                if (this._context == null)
                {
                    this._context = GetClientDBContext();
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here...  
                        ViewData["strRes_UpdCBTitheBal"] = "Tithe balances update failed. Client database failed to connect. Try updating Tithe balances table later.";
                        return -1;
                    }
                }

                var strDesc = "Church Body Tithe balances";
                var _userTask = "";


                //// get curr CB 
                var oCBCurr = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody) //.Include(t => t.ChurchLevel)
                            .Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBid).FirstOrDefault();

                if (oCBCurr == null)
                { ViewData["strRes_UpdCBTitheBal"] = "Updating congregation's tithe balance summary. Balances update failed. Congregation details could not be retrieved. Try updating Tithe balances table later."; return -1; }


                var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                                .Where(c => c.AppGlobalOwnerId == oAGOid && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                oCP_List_1 = oCP_List_1.Where(c =>
                                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();
                ChurchPeriod oCPRDefault = oCP_List_1.FirstOrDefault(); 

                if (oCPRDefault == null)
                { ViewData["strRes_UpdCBTitheBal"] = "Updating congregation's tithe balance summary. Balances update failed. Current church period could not be retrieved. Try updating Tithe balances table later."; return -1; }

                var oCPRDefaultId = oCPRDefault.Id;  // prompt for error


                // get the default --- base curr  
                var oCURR_List_1 = (from t_cc in _context.CurrencyCustom.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.IsBaseCurrency == true)  // || c.IsDisplay == true)).ToList()
                                    from t_ctry in _context.Country.AsNoTracking().Where(c => c.CtryAlpha3Code == t_cc.CtryAlpha3Code).DefaultIfEmpty()
                                    select t_ctry 
                                    ).ToList();

                Country oCURRDefault = oCURR_List_1.FirstOrDefault();
                ///
                ///
                var oCURRDefaultId = oCURRDefault != null ? oCURRDefault.Curr3LISOSymbol : null; // oCURRCode;                
                var oCTRYDefaultId = oCURRDefault != null ? oCURRDefault.CtryAlpha3Code : null; //oCURRCtryCode;


                /// get the matching rate --- agaisnt base curr
                var oCURR_UsedList_1 = _context.CurrencyCustom.AsNoTracking() // .Include(t => t.Country)
                                            .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.CtryAlpha3Code == oCURRCtryCode).ToList();
                CurrencyCustom oCURR_Used = oCURR_UsedList_1.FirstOrDefault();
                var oCURR_UsedRate = oCURR_Used != null ? oCURR_Used.BaseRate : 1;


                // update curr CB 
                // get list of local members... and save first... then update table afterward
                var oTTList = _context.TitheTrans.AsNoTracking()
                    .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Status == "A" && c.ChurchPeriodId== oCPRDefaultId).ToList();

                //// update curr CB 
                //// get list of local members... and save first... then update table afterward
                //var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Status == "A").ToList();  // Active-Blocked-Deactive current members only ::- keep oCM.Status in sync with [MemberStatus] ...VIPPP
                //foreach (ChurchMember oCM in oCMList)
                //{
                //    oCM.numMemAge = oCM.DateOfBirth != null ? (int)AppUtilties.CalcDateDiff(oCM.DateOfBirth.Value, DateTime.Now.Date, false, true) : -1;
                //}

                ///
                var tm = DateTime.Now;

                //// get these value from the settings of the CB... may be inherited too! so check out boy
                ////var strNVPCode = "TTL";    // c.Id != oCurrNVP.Id && 
                //// c.NVPCode == "GEN_AGE_GRP_C" || "GEN_AGE_GRP_Y" || "GEN_AGE_GRP_YA" || "GEN_AGE_GRP_MA" || "GEN_AGE_GRP_AA"
                //var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.OwnedByChurchBody)
                //                                   .Where(c => c.AppGlobalOwnerId == oAGOid &&
                //                                   c.NVPCode.Contains("GEN_AGE_GRP")).ToList();
                //oNVP_List_1 = oNVP_List_1.Where(c =>
                //                   (c.OwnedByChurchBodyId == oCBid ||
                //                   (c.OwnedByChurchBodyId != oCBid && c.SharingStatus == "C" && c.OwnedByChurchBodyId == _oLoggedCB.ParentChurchBodyId) ||
                //                   (c.OwnedByChurchBodyId != oCBid && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCBCurr)))).ToList();

                
                //var oNVP_List_C = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_C").FirstOrDefault();
                //var oNVP_List_Y = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_Y").FirstOrDefault();
                //var oNVP_List_YA = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_YA").FirstOrDefault();
                //var oNVP_List_MA = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_MA").FirstOrDefault();
                //var oNVP_List_AA = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_AA").FirstOrDefault();
                //// var ageLim_C = 0; var ageLim_Y = 0; var ageLim_YA = 0; var ageLim_MA = 0; var ageLim_AA = 0;
                //var ageMin_C = 0; var ageMin_Y = 0; var ageMin_YA = 0; var ageMin_MA = 0; var ageMin_AA = 0;
                //var ageMax_C = 0; var ageMax_Y = 0; var ageMax_YA = 0; var ageMax_MA = 0; var ageMax_AA = 0;
                /////
                //if (oNVP_List_C != null)
                //{
                //    ageMin_C = (int)oNVP_List_C.NVPNumVal >= 0 ? (int)oNVP_List_C.NVPNumVal : 0;
                //    ageMax_C = (int)oNVP_List_C.NVPNumValTo >= (int)oNVP_List_C.NVPNumVal ? (int)oNVP_List_C.NVPNumValTo : (int)oNVP_List_C.NVPNumVal;
                //}
                //if (oNVP_List_Y != null)
                //{
                //    ageMin_Y = (int)oNVP_List_Y.NVPNumVal >= 0 ? (int)oNVP_List_Y.NVPNumVal : 0;
                //    ageMax_Y = (int)oNVP_List_Y.NVPNumValTo >= (int)oNVP_List_Y.NVPNumVal ? (int)oNVP_List_Y.NVPNumValTo : (int)oNVP_List_Y.NVPNumVal;
                //}
                //if (oNVP_List_YA != null)
                //{
                //    ageMin_YA = (int)oNVP_List_YA.NVPNumVal >= 0 ? (int)oNVP_List_YA.NVPNumVal : 0;
                //    ageMax_YA = (int)oNVP_List_YA.NVPNumValTo >= (int)oNVP_List_YA.NVPNumVal ? (int)oNVP_List_YA.NVPNumValTo : (int)oNVP_List_YA.NVPNumVal;
                //}
                //if (oNVP_List_MA != null)
                //{
                //    ageMin_MA = (int)oNVP_List_MA.NVPNumVal >= 0 ? (int)oNVP_List_MA.NVPNumVal : 0;
                //    ageMax_MA = (int)oNVP_List_MA.NVPNumValTo >= (int)oNVP_List_MA.NVPNumVal ? (int)oNVP_List_MA.NVPNumValTo : (int)oNVP_List_MA.NVPNumVal;
                //}
                //if (oNVP_List_AA != null)
                //{
                //    ageMin_AA = (int)oNVP_List_AA.NVPNumVal >= 0 ? (int)oNVP_List_AA.NVPNumVal : 0;
                //    ageMax_AA = (int)oNVP_List_AA.NVPNumValTo >= (int)oNVP_List_AA.NVPNumVal ? (int)oNVP_List_AA.NVPNumValTo : (int)oNVP_List_AA.NVPNumVal;
                //}
                /////


                ///
                var _countCBTitheBalAdd = 0; var _countCBTitheBalUpd = 0;
                var oCBTitheBal = _context.CBTitheTransBal.AsNoTracking()
                    .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.ChurchPeriodId == oCPRDefaultId).FirstOrDefault();
                if (oCBTitheBal != null)
                {
                    //oCBTitheBal.AppGlobalOwnerId = oAGOid;
                    //oCBTitheBal.ChurchBodyId = oCBid;
                    // oCBTitheBal.ChurchPeriodId = oCPRDefaultId;   // cannot change jux like that... this na money matters oo

                    //oCBTitheBal.Curr3LISOSymbol = oCURRDefaultId;
                    //oCBTitheBal.CtryAlpha3Code = oCTRYDefaultId;

                    oCBTitheBal.Created = tm;
                    oCBTitheBal.LastMod = tm;
                    oCBTitheBal.CreatedByUserId = oUserId_Logged;
                    oCBTitheBal.LastModByUserId = oUserId_Logged;

                    ///
                    oCBTitheBal.TotAmtCol = oTTList.Sum(x=> (x.TitheAmount != null ? x.TitheAmount.Value * oCURR_UsedRate : 0));  // already added
                    oCBTitheBal.TotAmtOut = 0;
                    oCBTitheBal.TotAmtNet = oCBTitheBal.TotAmtCol - oCBTitheBal.TotAmtOut; 

                    _context.CBTitheTransBal.Update(oCBTitheBal);
                    _countCBTitheBalUpd++;
                }
                else  // add CB Tithe balances to the table
                {
                    oCBTitheBal = new CBTitheTransBal()
                    {
                        AppGlobalOwnerId = oAGOid,
                        ChurchBodyId = oCBid,
                        ChurchPeriodId = oCPRDefaultId,

                        Curr3LISOSymbol = oCURRDefaultId,
                        CtryAlpha3Code = oCTRYDefaultId,                        
                        ///
                        Created = tm,
                        LastMod = tm,
                        CreatedByUserId = oUserId_Logged,
                        LastModByUserId = oUserId_Logged,
                        ///
                        TotAmtCol = oTTList.Sum(x => (x.TitheAmount != null ? x.TitheAmount.Value * oCURR_UsedRate : 0)),
                        TotAmtOut = 0,
                        // TotAmtNet = oTTList.Sum(x => (x.TitheAmount != null ? x.TitheAmount.Value : 0)) - 0
                    };

                    oCBTitheBal.TotAmtNet = oCBTitheBal.TotAmtCol - oCBTitheBal.TotAmtOut;

                    ///
                    _context.CBTitheTransBal.Add(oCBTitheBal);
                    _countCBTitheBalAdd++;
                }

                // save first before updating Tithe balances... up
                _context.SaveChanges();


                if (oCBCurr.ParentChurchBody == null)
                { ViewData["strRes_UpdCBTitheBal"] = "Updated congregation's tithe balance summary. Tithe balances update complete. No further update as congregation has no parent."; return 1; }


                //// start Tithe balances update with parent >> child CBs 
                var oCBPar = oCBCurr.ParentChurchBody;

                // create CB path up -- >> 
                ///
                var oCBTitheBal_List = new List<CBTitheTransBal>();  // update if CB exists, else add .... batch update
                var oNextCBTitheBal = new CBTitheTransBal();
                //var currCBid = oCBid;
                var _countCBTitheBalAdd_Par = 0; var _countCBTitheBalUpd_Par = 0;
                var _countCBTitheBalAdd_Temp = 0; var _countCBTitheBalUpd_Temp = 0;
                do
                {
                    // get the previous bal ... and add the new current bal.
                    // get these value feom the settings of the CB... may be inherited too! so check out boy
                    // ageLim_C = 0; var ageLim_Y = 0; var ageLim_YA = 0; var ageLim_MA = 0; var ageLim_OA = 0;
                    var oNextCBTitheBal_List = _context.CBTitheTransBal.AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBody.ParentChurchBodyId == oCBPar.Id && c.ChurchPeriodId == oCPRDefaultId).ToList();  // Active-Blocked-Deactive current members only ::- keep oCM.Status in sync with [MemberStatus] ...VIPPP
                    ///
                    oNextCBTitheBal = _context.CBTitheTransBal.AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBPar.Id && c.ChurchPeriodId == oCPRDefaultId).FirstOrDefault();  // for curr period

                    if (oNextCBTitheBal_List.Count > 0)
                    {
                        //for (int i = 0; i < oNextCBTitheBal_List.Count; i++)
                        //{
                        // oNextCBTitheBal = oNextCBTitheBal_List[i];
                        if (oNextCBTitheBal != null)
                        {
                            //oNextCBTitheBal.AppGlobalOwnerId = oAGOid;
                            //oNextCBTitheBal.ChurchBodyId = oCBid;
                            // ChurchPeriodId = oCPRDefaultId;

                            oNextCBTitheBal.Curr3LISOSymbol = oCURRCode;
                            oNextCBTitheBal.CtryAlpha3Code = oCURRCtryCode;

                            oNextCBTitheBal.Created = tm;
                            oNextCBTitheBal.LastMod = tm;
                            oNextCBTitheBal.CreatedByUserId = oUserId_Logged;
                            oNextCBTitheBal.LastModByUserId = oUserId_Logged;
                            ///
                            oNextCBTitheBal.TotAmtCol = oNextCBTitheBal_List.Sum(x => x.TotAmtCol);  // alreay converted
                            oNextCBTitheBal.TotAmtOut = oNextCBTitheBal_List.Sum(x => x.TotAmtOut);
                            oNextCBTitheBal.TotAmtNet = oNextCBTitheBal.TotAmtCol - oNextCBTitheBal.TotAmtOut;    //oNextCBTitheBal_List.Sum(x => x.TotAmtNet);  // 

                            _context.CBTitheTransBal.Update(oNextCBTitheBal);
                            _countCBTitheBalUpd_Temp++;
                        }
                        else  // add CB Tithe balances to the table
                        {
                            oNextCBTitheBal = new CBTitheTransBal()
                            {
                                AppGlobalOwnerId = oCBPar.AppGlobalOwnerId,
                                ChurchBodyId = oCBPar.Id,
                                ChurchPeriodId = oCPRDefaultId,
                                Curr3LISOSymbol = oCURRCode,
                                CtryAlpha3Code = oCURRCtryCode,
                                ///
                                Created = tm,
                                LastMod = tm,
                                CreatedByUserId = oUserId_Logged,
                                LastModByUserId = oUserId_Logged,
                                ///
                                TotAmtCol = oNextCBTitheBal_List.Sum(x => x.TotAmtCol),
                                TotAmtOut = oNextCBTitheBal_List.Sum(x => x.TotAmtOut),
                                TotAmtNet = oNextCBTitheBal_List.Sum(x => x.TotAmtNet)
                            };

                            _context.CBTitheTransBal.Add(oNextCBTitheBal);
                            _countCBTitheBalAdd_Temp++;
                        }

                        if ((_countCBTitheBalAdd_Temp + _countCBTitheBalUpd_Temp) > 0)
                        {
                            _context.SaveChanges();  // batch can't work... save ... so the list can be upadted!
                            ///
                            _countCBTitheBalAdd_Par += _countCBTitheBalAdd_Temp;
                            _countCBTitheBalUpd_Par += _countCBTitheBalUpd_Temp;

                            _countCBTitheBalAdd_Temp = 0; _countCBTitheBalUpd_Temp = 0;
                        }
                        //}
                    } 


                    //oCBPar becomes curr CB... then loop
                    oCBPar = _context.ChurchBody.AsNoTracking() //.Include(t => t.ParentChurchBody) //.Include(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBPar.ParentChurchBodyId).FirstOrDefault();

                    // stop once the chain breaks --- no parent!
                }
                while (oCBPar != null); // && oCBCurr.ChurchLevel.LevelIndex > 0)

                 
                if ((_countCBTitheBalAdd + _countCBTitheBalUpd + _countCBTitheBalAdd_Par + _countCBTitheBalUpd_Par) > 0)
                {
                    //_context.SaveChanges();
                    ///
                    _userTask = "Updated tithe balances for " + (_countCBTitheBalAdd + _countCBTitheBalUpd + _countCBTitheBalAdd_Par + _countCBTitheBalUpd_Par) + " congregations [ added:" + _countCBTitheBalAdd + _countCBTitheBalAdd_Par + "; modified: " + _countCBTitheBalAdd_Par + _countCBTitheBalUpd_Par + "] on same route successfully.";
                }


                //audit...
                var _tm = DateTime.Now;
                // register @MSTR
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged));

                //register @CLNT
                _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, null, null, "T",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged)
                    , this._clientDBConnString);

                ViewData["strRes_UpdCBTitheBal"] = _userTask;
                return 1;
            }

            catch (Exception ex)
            {
                ViewData["strRes_UpdCBTitheBal"] = "Failed updating tithe balance summary. Refresh data or contact system admin. Error: " + ex.Message;
                return -1;
            }
        }

        public ActionResult IndexCB_RCSS(int? oCBid = null, int t_ndx = 1, bool loadLim = false)  //, int filterIndex = 1, int pageIndex = 1, int? numCodeCriteria_1 = (int?)null, string strCodeCriteria_2 = null)  // , int? subSetIndex = 0  int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            try
            {
                if (this._context == null)
                {
                    this._context = GetClientDBContext();
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return View("_ErrorPage");
                    }
                }

                if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                { RedirectToAction("LoginUserAcc", "UserLogin"); }

                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                //if (!InitializeUserLogging())
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                // Client
                // var oAppGloOwnId = this._oLoggedAGO.Id;

                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCBid).FirstOrDefault();

                var isCBPar = oCB.OrgType == "CR" || oCB.OrgType == "CH";

                // MSTR
                //var oUserId = this._oLoggedUser.Id;
                //var oAGO_MSTR = this._oLoggedAGO_MSTR; var oCB_MSTR = this._oLoggedCB_MSTR;
                //var oAGO = this._oLoggedAGO; var oCB = this._oLoggedCB;

                // if (oAGO_MSTR == null || this._oLoggedCB_MSTR == null || oAGO == null || oCB == null) { return View("_ErrorPage"); }

                // var proScope = "C";
                //get all member from congregation
                //  var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.ChurchBodyId == this._oLoggedCB.Id).ToList();

                var oRCSSList = new List<CBTitheBalModel>();

                oRCSSList = (
                from t_mrb in _context.CBTitheTransBal.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id &&
                            ((!isCBPar && c.ChurchBodyId == oCBid) || (isCBPar && c.ChurchBody.ParentChurchBodyId == oCBid)))  // both main view, sub view
                from t_cb in _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_mrb.AppGlobalOwnerId && c.Id == t_mrb.ChurchBodyId)

                select new CBTitheBalModel()
                {
                    oAppGloOwnId = t_mrb.AppGlobalOwnerId,
                    oChurchBodyId = t_mrb.ChurchBodyId,
                    oChurchBody = t_cb,
                    strChurchBody = t_cb.Name,
                    oCBTitheTransBal = t_mrb,
                    ///
                    strTotCol = string.Format("{0:N2}", t_mrb.TotAmtCol),  // link to actual mem roll
                    strTotOut = string.Format("{0:N2}", t_mrb.TotAmtOut),
                    strTotNet = string.Format("{0:N2}", t_mrb.TotAmtNet),
                      
                })
                // .OrderBy(c => c.strChurchBody).ThenBy(c => c.strMemberFullName) 
                .ToList();
                if (oRCSSList.Count > 0) oRCSSList = oRCSSList
                    .OrderBy(c => (c.oChurchBody.ChurchLevel != null ? c.oChurchBody.ChurchLevel.LevelIndex : (int?)null))
                    .ThenBy(c => c.strChurchBody).ToList();




                var oRCSSModel = new CBTitheBalModel();

                oRCSSModel.oAppGloOwnId = this._oLoggedAGO.Id;
                oRCSSModel.oChurchBodyId = oCBid;
                oRCSSModel.oChurchBody = oCB;
                oRCSSModel.strChurchBody = oCB.Name;

                ///
                //oRCSSModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                //oRCSSModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                oRCSSModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oRCSSModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                oRCSSModel.oUserId_Logged = _oLoggedUser.Id;

                oRCSSModel.strGrandTotCol = string.Format("{0:N2}", oRCSSList.Sum(x => x.oCBTitheTransBal.TotAmtCol));
                oRCSSModel.strGrandTotOut = string.Format("{0:N2}", oRCSSList.Sum(x => x.oCBTitheTransBal.TotAmtOut));
                oRCSSModel.strGrandTotNet = string.Format("{0:N2}", oRCSSList.Sum(x => x.oCBTitheTransBal.TotAmtNet)); 

                /// 
                oRCSSModel.lsCBTitheBalModels = oRCSSList;
                ViewData["oRCSSModel_List"] = oRCSSModel.lsCBTitheBalModels;

                var strDesc = "Receipts Summary Sheet (Tithes)";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                oRCSSModel.strCurrTask = strDesc;
                oRCSSModel.taskIndex = t_ndx;


                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oRCSSModel);
                TempData["oVmCSPModel"] = _oUPModel; TempData.Keep();

                if (loadLim)
                    return PartialView("_vwIndexCB_RCSS", oRCSSModel);
                else
                    return View("IndexCB_RCSS", oRCSSModel);
            }

            catch (Exception ex)
            {
                return View("ErrorPage");
            }
        }



    }
}
