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
using System.Net.Mail;
using System.Net;

namespace RhemaCMS.Controllers.con_ch_life
{
    public class ChurchTransferController : Controller
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

        List<DiscreteLookup> dlTransferTypes = new List<DiscreteLookup>();
        List<DiscreteLookup> dlApprovalStepStatuses = new List<DiscreteLookup>();
        List<DiscreteLookup> dlApprovalStatuses = new List<DiscreteLookup>();

        private List<DiscreteLookup> dlShareStatus = new List<DiscreteLookup>();

        private List<DiscreteLookup> dlPeriodTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlIntervalFreqs = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlSemesters = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlQuarters = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlMonths = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlDays = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlActivityTypes = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlGenderStatuses = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlMemTypeCode = new List<DiscreteLookup>();

        //private List<DiscreteLookup> dlPmntModes = new List<DiscreteLookup>();
        //private List<DiscreteLookup> dlTitheModes = new List<DiscreteLookup>();
        //private List<DiscreteLookup> dlTitherScopes = new List<DiscreteLookup>();
        //private List<DiscreteLookup> dlPostModes = new List<DiscreteLookup>();

        public ChurchTransferController(MSTR_DbContext masterContext, IWebHostEnvironment hostingEnvironment,
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


            dlTransferTypes.Add(new DiscreteLookup() { Category = "TransferType", Val = "MT", Desc = "Member Transfer" });
            dlTransferTypes.Add(new DiscreteLookup() { Category = "TransferType", Val = "RT", Desc = "Role Transfer" });
            dlTransferTypes.Add(new DiscreteLookup() { Category = "TransferType", Val = "CT", Desc = "Clergy Transfer" });


            // N - N/A  P - ending[all steps Pending], I-n Progress[at least 1 step In Process], H--On hold, Completed[A - pproved][all steps Approved],  C = Forced Completed, F-ailed[D-enied /Declined][at least 1 step Failed], R= Recalled, X= Terminated
            //dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "N", Desc = "N/A" });

            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "P", Desc = "Pending" });
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "I", Desc = "In Progress" });
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "H", Desc = "On Hold" });
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "A", Desc = "Approved" });
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "C", Desc = "Force Complete" });
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "D", Desc = "Declined" });  //Denied
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "R", Desc = "Recalled" });
            dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "X", Desc = "Terminated" });
           // dlApprovalStepStatuses.Add(new DiscreteLookup() { Category = "ApprovalStepStatus", Val = "X", Desc = "Terminated" });

            //dlApprovalStatuses.Add(new DiscreteLookup() { Category = "ApprovalStatus", Val = "D", Desc = "Draft" });
            //dlApprovalStatuses.Add(new DiscreteLookup() { Category = "ApprovalStatus", Val = "P", Desc = "Pending" });
            //dlApprovalStatuses.Add(new DiscreteLookup() { Category = "ApprovalStatus", Val = "R", Desc = "Received" });
            //dlApprovalStatuses.Add(new DiscreteLookup() { Category = "ApprovalStatus", Val = "C", Desc = "Closed" });
            //dlApprovalStatuses.Add(new DiscreteLookup() { Category = "ApprovalStatus", Val = "X", Desc = "Terminated" });


       
        dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "G", Desc = "Guest" }); /// Regular Visitor 
            dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "A", Desc = "Affiliate" });  // visiting missionaries, workers can be assigned temporal membership status -- MBD, MCI, MCP, MCL
            dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "N", Desc = "New Convert" }); // FULLY automate... MBD, MCI, MLS, MCP, MCL, MCLAc, MCET, MS [ MT, MS -- unassigned ], MCA, MTP    add New convert as *special Visitor [ on diff interface ]  ... [Member data] to be linked with [New Convert data] ... New Convert Class
           /// dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "T", Desc = "In-Transit" }); // jux like the Congregant ... only that he has not reported physically yet --> result of TRANSFERS
            dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "C", Desc = "Member" }); // has all modules [21] -- MBD, MCI, MLS, MFR, MCP, MED, MPB, MWE --- MCL [MCL, MCLAc, MCET], MCM [MS, MT, MS], MCG, MCR, MRR, MCA, MCT, MTP, MCV [visits** when sick or regular checkups]
           
            //dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "L", Desc = "Church Leader" });  /// Mainstream church leadership [elders abd the like]
            //dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "P", Desc = "Lay Pastor" });   /// lay [P]astor
            //dlMemTypeCode.Add(new DiscreteLookup() { Category = "MemTypeCode", Val = "M", Desc = "Minister" });   // Ordained...
            
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
                // case "C": return "Terminated";


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
                // case "C": return "Terminated";


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



        public static string GetConcatMemberName(string title, string fn, string mn, string ln, bool profileDispFormat = false, bool dispTitle = false, bool lnSTRT = false, bool capSTRT = false, bool capLn = false)
        {
            if (!lnSTRT && capSTRT) fn = fn.ToUpper();
            else if (lnSTRT && capSTRT) ln = ln.ToUpper();

            if (capLn) ln = ln.ToUpper();

            if (lnSTRT)
            {
                if (profileDispFormat)
                    return ((((dispTitle ? title : "") + " " + ln).Trim() + " " + mn).Trim() + " " + fn).Trim();
                else
                    return (((ln + ' ' + mn).Trim() + " " + fn).Trim() + " " + (dispTitle && !string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
            }
            else
            {
                if (profileDispFormat)
                    return ((((dispTitle ? title : "") + ' ' + fn).Trim() + " " + mn).Trim() + " " + ln).Trim();
                else
                    return (((fn + ' ' + mn).Trim() + " " + ln).Trim() + " " + (dispTitle && !string.IsNullOrEmpty(title) ? "(" + title + ")" : "")).Trim();
            }
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


        public static string GetMemTypeDesc(string oCode)
        {
            switch (oCode)
            {
                case "M": return "Minister";   // Ordained...
                case "P": return "Lay Pastor";  /// lay [P]astor
                case "L": return "Church Leader";
                case "C": return "Member";
                //   case "T": return "In-Transit";      // Transfer states 
                case "N": return "New Convert";      // Transfer states 
                case "A": return "Affiliate";
                case "G": return "Guest";  /// Regular Visitor

                default: return oCode;
            }
        }


        private static string GetRequestProcessStatusDesc(string stat)
        {
            // N - N/A  P - ending[all steps Pending], I-n Progress[at least 1 step In Process], H--On hold, Completed[A - pproved][all steps Approved],  C = Forced Completed, F-ailed[D-enied /Declined][at least 1 step Failed], R= Recalled, X= Terminated
            //... link this to the ApprovalAction Status
            switch (stat)
            {
                //case "N": return "Draft";  //New

                case "P": return "Pending";
                case "I": return "In Progress";
                case "H": return "On Hold";  //with reason                
                case "A": return "Approved"; //C-ompleted   
                case "F": return "Force-Complete"; //C-ompleted
                case "D": return "Declined"; // "Denied" DECLINED;
                case "R": return "Recalled";
                case "X": return "Terminated";

                //case "C": return "Closed";
                //case "Z": return "Archived";

                //// case "I": return "Pending Approval"; //In Progress
                //case "T": return "In-transit";  //Transit
                //case "U": return "Incomplete";  //unsuccessful
                //case "Y": return "Transferred";  //Yes... Completed ... jux to make sorting simple  

                ////case "R": return "Role Transferred";  //
                //                                      // case "Y": return "In-transit";
                //case "M": return "Moved";

                //// N - None, P = Pending /Un-read, R = Received, X = Closed 
                //case "N": return "Recalled";
                //case "P": return "Terminated";
                //case "R": return "Closed";
                //case "Z": return "Archived";

                default: return "N/A"; //null
            }

            //return "";
        }

        //t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
        public static string GetRequestStatusDesc(string oCode, bool isDetailInfo = false)
        {
            switch (oCode)
            {
                ///   // A-ctive, --- [ N-Draft, P-Pending, I-In Progress, X-Terminated, R-Recalled ], [ C-Completed/, Declined /F-ailed ], [ T-In Transit, S-Transferred/Success ]
                ///   
                case "N": return "Draft";  //New case "N": return "Closed"; 
                // case "K": return "Acknowledged";  //Received Acknowledgement

                case "P": if (isDetailInfo) return "Submitted [Pending Acknowledgment]"; else return "Submitted";  //Request Sent
                case "K": if (isDetailInfo) return "Acknowledged [Pending Approval]"; else return "Acknowledged";
                case "H": if (isDetailInfo) return "On Hold [Pending Approval]"; else return "On Hold";
                case "I": if (isDetailInfo) return "In Progress [Pending Approval]"; else return "In Progress"; //

                // case "T": return "In-transit";  //Transit ...

                case "A": return "Approved"; //C-ompleted     case "F": return "Force-Complete"; //C-ompleted
                case "D": return "Declined"; // "Denied" DECLINED;
                case "R": return "Recalled";
                case "X": return "Terminated";

                // case "R": return "Role Transferred";  //
                // case "Y": return "In-transit";
                // case "M": return "Moved";
                // case "C": return "Closed"; 
                // case "N": return "None";
                // case "P": return "Unacknowledged"; // Pending Acknowledgement
                 
                case "U": return "Unsuccessful";  //unsuccessful 
                case "T": return "In-Transit"; // Awaiting member visit ... 
                case "Y": return "Transferred";  //Yes... Completed ... jux to make sorting simple ...
                case "C": return "Closed"; // manually closed by the ToCB... when member visits
                case "Z": return "Archived";  // manual... add fxn: Push to Archive, Push All Closed to Archive

                default: return "N/A";
            }
        }

        //public static string GetAckStatusDesc(string oCode)  ///  N - None, P = Pending /Un-read, R = Received, C = Closed 
        //{
        //    switch (oCode)
        //    {
        //        case "N": return "None";
        //        case "P": return "Unacknowledged"; // Pending Acknowledgement
        //        case "K": return "Received";  // Acknowledgement
        //                                              // case "Y": return "In-transit";
        //        case "C": return "Closed";

        //        default: return "N/A";
        //    }
        //}

        public static string GetTransferTypeDesc(string oCode)
        {
            switch (oCode)
            {
                case "MT": return "Member Transfer";
                case "CT": return "Clergy Transfer";  //Ministerial 
                case "RT": return "Role Transfer";

                //case "CA": return "Clergy Associate Transfer";
                //case "CL": return "Core Leader Transfer";
                //case "LL": return "Lay Leader Transfer";
                //case "CW": return "Church Worker Transfer";
                //case "CT": return "Committee Member Transfer";

                default: return "Church transfer";
            }
        }

        public static string GetTransferSubTypeDesc(string oCode)
        {
            switch (oCode)
            {
                case "M--": return "Membership only";
                case "-R-": return "Role only";
                case "--F": return "Family only";
                case "MR-": return "Membership with role";
                case "M-F": return "Membership and family";
                case "-RF": return "Role and family";
                case "MRF": return "Membership, role and family";

                //case "CA": return "Clergy Associate Transfer";
                //case "CL": return "Core Leader Transfer";
                //case "LL": return "Lay Leader Transfer";
                //case "CW": return "Church Worker Transfer";
                //case "CT": return "Committee Member Transfer";

                default: return oCode;
            }
        }





        private string GetApprovalActionStatus(List<ApprovalActionStep> oAASList, int statusIndx = 0, string currActionStatus = null)  //int actionScope = 2, StatIndx = 1: req stat, | 2: approval stat... scope 1:Internal, 2=External   //private string GetApprovalActionStatus(int oChurchBodyId, ApprovalAction oAA)
        { // statusIndx = 0 -- Approval Action Status, statusIndx = 1 -- Req Status @ Scope 1, statusIndx = 2 -- Req Status @ Scope 2 
            // N - N/A  P - ending[all steps Pending], I-n Progress[at least 1 step In Process], H--On hold, Completed[A - pproved][all steps Approved],  C = Forced Completed, F-ailed[D-enied /Declined][at least 1 step Failed], R= Recalled, X= Terminated
            //... link this to the ApprovalAction Status

            var pending = 0; var inProgress = 0; var onHold = 0; var approve = 0; var forceComplete = 0; var decline = 0; var recall = 0; var cancel = 0;

            //if ((actionScope == 2) || (actionScope == 1 && statIndx == 2))  //EXT, INT-AS
            //{

            if (currActionStatus != null)
            {
                switch (currActionStatus)
                {
                    case "I": return "I";  //only @acknowledge or external process
                    case "H": return statusIndx == 0 ? "H" : "I";
                    case "F": return statusIndx == 0 || statusIndx == 2 ? "A" : "I"; // thus end approval in that scope only
                    case "D": return "D"; //at least 1 DECLINE
                    case "R": return "R"; //at least 1 RECALL
                    case "X": return "X"; //at least 1 TERMINATE
                }
            }

            foreach (ApprovalActionStep oAA in oAASList)
            {
                switch (oAA.ActionStepStatus)
                {
                    case "P": pending++; break; //return "Pending";
                    case "I": inProgress++; break; // return "I";  // at least 1 In Progress, "On Hold"; 
                    case "H": onHold++; break; ////if (statIndx==2) return "H"; else break; //i.e. if it' app stat... then take back the H else I...  "I"; //at least 1 In Progress, "On Hold";               
                    case "A": approve++; break; //return "Approved"; 
                    case "F": forceComplete++; break; //return "Force-Complete"; 
                    case "D": decline++; break; // return "D"; //at least 1 DECLINE
                    case "R": recall++; break; //return "R"; //at least 1 RECALL
                    case "X": cancel++; break; //return "X"; //at least 1 TERMINATE
                }
            }

            if (pending == oAASList.Count) return statusIndx == 1 ? "I" : "P"; // return statIndx == 1 ? "I" : "P"; //"P"; //Pending // YES Pending, but still in Progress... yet to go to th To cong. 
            else if (approve + forceComplete == oAASList.Count) return statusIndx == 1 ? "I" : "A"; // return statIndx == 1 ? "I" : "A"; //"A"; //Approved  //YES Approved, but still in Progress... yet to go to th To cong.                 
                                                                                                    // else if (inProgress > 0 || onHold > 0) return "I"; //at least 1 In Progress, On-Hold
                                                                                                    // else if (approve + forceComplete < oAASList.Count) return "I";            
            else if (cancel > 0) return "X"; //at least 1 TERMINATE
            else if (recall > 0) return "R"; //at least 1 RECALL
            else if (decline > 0) return "D"; //at least 1 DECLINE
            else if (onHold > 0) return statusIndx == 0 ? "H" : "I"; // return "H"; //at least 1 RECALL
            else if (inProgress > 0 || (approve + forceComplete < oAASList.Count)) return "I"; //at least 1 RECALL


            //}

            //else if (actionScope == 1 && statIndx == 1) //INT, if  //REQ STAT
            //{
            //    foreach (ApprovalActionStep oAA in oAASList)
            //    {
            //        switch (oAA.ActionStepStatus)
            //        {
            //            case "P": pending++; break; //return "Pending";
            //            case "I": inProgress++; return "I";  // at least 1 In Progress, "On Hold"; 
            //            case "H": onHold++; return "I"; //at least 1 In Progress, "On Hold";               
            //            case "A": approve++; break; //return "Approved"; 
            //            case "F": forceComplete++; break; //return "Force-Complete"; 
            //            case "D": decline++; return "D"; //at least 1 DECLINE
            //            case "R": recall++; return "R"; //at least 1 RECALL
            //            case "X": cancel++; return "X"; //at least 1 TERMINATE
            //        }
            //    }

            //    if (pending == oAASList.Count) return "I"; // YES Pending, but still in Progress... yet to go to th To cong. 
            //    else if (approve + forceComplete == oAASList.Count) return "I"; //YES Approved, but still in Progress... yet to go to th To cong.                
            //    else if (inProgress > 0 || onHold > 0) return "I"; //at least 1 In Progress, On-Hold
            //    else if (recall > 0) return "R"; //at least 1 RECALL
            //    else if (cancel > 0) return "X"; //at least 1 TERMINATE
            //    else if (decline > 0) return "D"; //at least 1 DECLINE            
            //}

            return "";
        }

        private string GetApprovalActionStatus(int oChurchBodyId, int oApprovalActionId, int statusIndx = 0, string currActionStatus = null)  //int actionScope = 2, StatIndx = 1: req stat, 2: approval stat... scope 1:Internal, 2=External   
        {
            // N - N/A  P - ending[all steps Pending], I-n Progress[at least 1 step In Process], H--On hold, Completed[A - pproved][all steps Approved],  C = Forced Completed, F-ailed[D-enied /Declined][at least 1 step Failed], R= Recalled, X= Terminated
            //... link this to the ApprovalAction Status

            var oAASList = _context.ApprovalActionStep.Where(c => c.ChurchBodyId == oChurchBodyId && c.ApprovalActionId == oApprovalActionId).ToList();
            //
            var pending = 0; var inProgress = 0; var onHold = 0; var approve = 0; var forceComplete = 0; var decline = 0; var recall = 0; var cancel = 0;

            if (currActionStatus != null)
            {
                switch (currActionStatus)
                {
                    case "I": return "I";  //only @acknowledge or external process
                    case "H": return statusIndx == 0 ? "H" : "I";
                    case "F": return statusIndx == 0 || statusIndx == 2 ? "A" : "I"; // thus end approval in that scope only
                    case "D": return "D"; //at least 1 DECLINE
                    case "R": return "R"; //at least 1 RECALL
                    case "X": return "X"; //at least 1 TERMINATE
                }
            }

            foreach (ApprovalActionStep oAA in oAASList)
            {
                switch (oAA.ActionStepStatus)
                {
                    case "P": pending++; break; //return "Pending";
                    case "I": inProgress++; break; // return "I";  // at least 1 In Progress, "On Hold"; 
                    case "H": onHold++; break; ////if (statIndx==2) return "H"; else break; //i.e. if it' app stat... then take back the H else I...  "I"; //at least 1 In Progress, "On Hold";               
                    case "A": approve++; break; //return "Approved"; 
                    case "F": forceComplete++; break; //return "Force-Complete"; 
                    case "D": decline++; break; // return "D"; //at least 1 DECLINE
                    case "R": recall++; break; //return "R"; //at least 1 RECALL
                    case "X": cancel++; break; //return "X"; //at least 1 TERMINATE
                }
            }

            if (pending == oAASList.Count) return statusIndx == 1 ? "I" : "P"; // return statIndx == 1 ? "I" : "P"; //"P"; //Pending // YES Pending, but still in Progress... yet to go to th To cong. 
            else if (approve + forceComplete == oAASList.Count) return statusIndx == 1 ? "I" : "A"; // return statIndx == 1 ? "I" : "A"; //"A"; //Approved  //YES Approved, but still in Progress... yet to go to th To cong.                 
                                                                                                    // else if (inProgress > 0 || onHold > 0) return "I"; //at least 1 In Progress, On-Hold
                                                                                                    // else if (approve + forceComplete < oAASList.Count) return "I";            
            else if (cancel > 0) return "X"; //at least 1 TERMINATE
            else if (recall > 0) return "R"; //at least 1 RECALL
            else if (decline > 0) return "D"; //at least 1 DECLINE
            else if (onHold > 0) return statusIndx == 0 ? "H" : "I"; // return "H"; //at least 1 RECALL
            else if (inProgress > 0 || (approve + forceComplete < oAASList.Count)) return "I"; //at least 1 RECALL
            //             
            return "";
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




        private string GetApprovers(ApprovalAction oApprovalAction)
        {
            if (oApprovalAction == null) return "";
            var strApprovers = "N/A";
            //ChurchModelContext ctx = (new ChurchTransferController(_context, null))._context;

            using (var tran = new System.Transactions.TransactionScope())
            {
                Task.Factory.StartNew(() =>

                strApprovers = _GetApprovers(oApprovalAction)

                );
                // Wait all
                tran.Complete();
            }

            return strApprovers;
        }

        private string _GetApprovers(ApprovalAction oApprovalAction)
        {
            if (oApprovalAction == null) return "";
            //ChurchModelContext ctx = (new ChurchTransferController(_context, null))._context;

            var oActionSteps = _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember)
                .Where(c => c.ApprovalActionId == oApprovalAction.Id).ToList();
            var _strApprovers = "N/A";
            if (oActionSteps.Count > 0)
            {
                _strApprovers = "";
                if (oActionSteps[0].ApproverMemberChurchRole.ChurchMember != null)
                {
                    var oMem = oActionSteps[0].ApproverMemberChurchRole.ChurchMember;
                    if (oMem != null)
                    {
                        _strApprovers = ((((!string.IsNullOrEmpty(oMem.Title) ? oMem.Title : "") + ' ' + oMem.FirstName).Trim() + " " + oMem.MiddleName).Trim() + " " + oMem.LastName).Trim();

                        if (oActionSteps.Count > 1)
                        {
                            _strApprovers += " and " + (oActionSteps.Count - 1).ToString() + " other" + ((oActionSteps.Count - 1) > 1 ? "s" : "");
                        }
                    }
                }
            }

            return _strApprovers;

        }

        ////MEMBER TRANSFER = MT, CLERGY/Ministers = CM, CLERGY ASSOCIATES = CA e.g. Catechists/Assistant Pastors, CORE LEADERS = CL e.g. Council members/Session, LAY LEADERS e.g. Groups/Associations= LL, 
        /////CHURCH WORKERS = CW, COMMITTEE Member/LEADERS = CT... CHURCH PROGRAM LEADS = PL, OTHER /OPEN = [O]
        public List<MemberProfileVM> GetChurchMembersList(int oChurchBodyId, string oLeaderRoleType = null)
        {
            List<MemberProfileVM> chuMemberList = new List<MemberProfileVM>();
            if (oLeaderRoleType == null)
            {// Church Members
                chuMemberList = (from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == oChurchBodyId) //Include(t => t.ContactInfo).
                                 join t_ms in _context.MemberStatus.AsNoTracking().Where(c => c.ChurchBodyId == oChurchBodyId && c.IsCurrent == true && c.ChurchMemStatus_NVP.IsAvailable == true)
                                            on t_cm.Id equals t_ms.ChurchMemberId into ab
                                 from t_ms_v in ab //.DefaultIfEmpty()
                                                   // where t_ms_v.ChurchBodyId == oChurchBodyId
                                                   // orderby t.LastName ascending
                                 select new MemberProfileVM
                                 {
                                     MemberId = t_cm.Id,
                                     //strMemberFullName = t_cm.MemberCode + ' ' + (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                     strMemberFullName = (!string.IsNullOrEmpty(t_cm.GlobalMemberCode) ? t_cm.GlobalMemberCode + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                     oPersonalData = t_cm
                                 }
                            ).OrderBy(c => c.strMemberFullName).ToList();
            }
            else if (oLeaderRoleType == "CL")
            { //leader transfer... Church Leaders
                chuMemberList = (from t_mp in _context.MemberChurchRole.AsNoTracking().Where(c => c.ChurchBodyId == oChurchBodyId && c.IsCurrentRole == true)     // .Take(1)
                                 join b in _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == oChurchBodyId) on t_mp.ChurchMemberId equals b.Id into ab  // .Include(t => t.ContactInfo)
                                 from t_cm in ab //.DefaultIfEmpty()
                                 join c in _context.ChurchRole.AsNoTracking().Where(c => c.OwnedByChurchBodyId == oChurchBodyId) on t_mp.ChurchRoleId equals c.Id into abc // && c.LeaderRoleType == oTransferType  ie. bring all the leaders
                                 from t_lr in abc //.DefaultIfEmpty()
                                 join d in _context.MemberStatus.AsNoTracking().Where(c => c.ChurchBodyId == oChurchBodyId && c.IsCurrent == true && c.ChurchMemStatus_NVP.IsAvailable == true).ToList() on t_cm.Id equals d.ChurchMemberId into abcd   // Take(1)
                                 from t_ms in abcd //.DefaultIfEmpty()
                                 where t_cm.ChurchBodyId == oChurchBodyId
                                 // orderby t.LastName ascending
                                 select new MemberProfileVM
                                 {
                                     MemberId = t_cm.Id,
                                     //  strMemberFullName = t_cm.MemberGlobalId + ' ' + (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                     strMemberFullName = (!string.IsNullOrEmpty(t_cm.GlobalMemberCode) ? t_cm.GlobalMemberCode + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                     oPersonalData = t_cm
                                 }
                            ).ToList();

                chuMemberList = chuMemberList

                            .OrderBy(c => c.strMemberFullName).ToList();
            }
            else if (oLeaderRoleType == "CM" || oLeaderRoleType == "CT")
            { //clergy transfer... Clergy /Ministers
                // var oCB = _context.ChurchBody.Find(oChurchBodyId);
                //if (oCB != null)
                //{

                var oAGOid = this._oLoggedAGO.Id;
                chuMemberList = (from t_mlr in _context.MemberChurchRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oChurchBodyId && c.IsCurrentRole == true) //.Take(1)  .Include(t => t.ChurchBody)
                                 join t_cmx in _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oChurchBodyId) on t_mlr.ChurchMemberId equals t_cmx.Id into ab
                                 from t_cm in ab //.DefaultIfEmpty()
                                 join t_lrx in _context.ChurchRole.AsNoTracking()  //.Include(t => t.ChurchBody).Include(t => t.LeaderRoleCategory)  //// (bool)c.LeaderRoleCategory.IsCoreRole && (bool)c.IsOrdainedRole
                                    .Where(c => c.AppGlobalOwnerId == oAGOid && c.OwnedByChurchBodyId == oChurchBodyId && c.IsApplyToClergyOnly == true) on t_mlr.ChurchRoleId equals t_lrx.Id into abc // && c.LeaderRoleType == oTransferType  ie. bring all the leaders
                                 from t_lr in abc //.DefaultIfEmpty()

                                     // link the status direct to the mem (JOIN)... and make way for 
                                     //join t_msx in _context.MemberStatus.Include(t => t.ChurchBody).Include(t => t.ChurchMemStatus)
                                     //   .Where(c => c.ChurchBody.AppGlobalOwnerId == oCB.AppGlobalOwnerId && c.ActiveStatus == true && c.ChurchMemStatus.Available == true) on t_cm.Id equals t_msx.ChurchMemberId into abcd
                                     //from t_ms in abcd //.DefaultIfEmpty()  //  where t_cm.ChurchBodyId == oChurchBodyId

                                     // orderby t.LastName ascending
                                 select new MemberProfileVM
                                 {
                                     MemberId = t_cm.Id,
                                     //strMemberFullName = t_cm.MemberGlobalId + ' ' + (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                     strMemberFullName = (!string.IsNullOrEmpty(t_cm.GlobalMemberCode) ? t_cm.GlobalMemberCode + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                     oPersonalData = t_cm
                                 }
                      ).OrderBy(c => c.strMemberFullName).ToList();
                // }

            }



            ////get requestor detail
            //strRequestorFullName = (!string.IsNullOrEmpty(t_cm_r.MemberGlobalId) ? t_cm_r.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm_r.Title) ? t_cm_r.Title : "") + ' ' + t_cm_r.FirstName).Trim() + " " + t_cm_r.MiddleName).Trim() + " " + t_cm_r.LastName).Trim() , 
            //        var oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.RoleSector)
            //                .Where(c => c.ChurchBodyId == oCurrChuBodyId && c.ChurchMemberId == oMemTransf_MDL.oChurchTransfer.RequestorMemberId
            //                    && c.LeaderRoleId == oMemTransf_MDL.oChurchTransfer.RequestorRoleId).FirstOrDefault();
            //// oMemTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCurrChuMember_LogOn.MemberGlobalId) ? oCurrChuMember_LogOn.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(oCurrChuMember_LogOn.Title) ? oCurrChuMember_LogOn.Title : "") + ' ' + oCurrChuMember_LogOn.FirstName).Trim() + " " + oCurrChuMember_LogOn.MiddleName).Trim() + " " + oCurrChuMember_LogOn.LastName).Trim();
            //if (oReqMLR != null)
            //    oMemTransf_MDL.strRequestorFullName += oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.RoleSector != null ? ", " + oReqMLR.RoleSector.Name : "") + ")" : "";





            //index 0
            //chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });

            return chuMemberList; //Json(new SelectList(chuMemberList, "MemberId", "strMemberFullName"));
        }

        public JsonResult GetChurchMembers(int oChurchBodyId, string oTransferType)
        {
            List<MemberProfileVM> chuMemberList = GetChurchMembersList(oChurchBodyId, oTransferType == "MT" ? null : oTransferType);
            chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });
            return Json(new SelectList(chuMemberList, "MemberId", "strMemberFullName"));
        }

        public JsonResult FilterMemberList(int oChurchBodyId, string oTransferType, string strFilterVal)
        {
            //this list actually must be held in memory unti next reload... finetune later!
            List<MemberProfileVM> chuMemberList = GetChurchMembersList(oChurchBodyId, oTransferType == "MT" ? null : oTransferType);
            if (strFilterVal != null)
            {
                //  var f_ChuMemberList
                chuMemberList = chuMemberList.Where(c => c.strMemberFullName.ToUpper().Contains(strFilterVal.ToUpper()) || c.oPersonalData.GlobalMemberCode?.Contains(strFilterVal) == true || c.oPersonalData.MemberCustomCode?.Contains(strFilterVal) == true || c.oPersonalData.ContactInfo?.MobilePhone1?.Contains(strFilterVal) == true || c.oPersonalData.ContactInfo?.MobilePhone2?.Contains(strFilterVal) == true || c.oPersonalData.ContactInfo?.Email?.ToUpper()?.Contains(strFilterVal.ToUpper()) == true).ToList();
                // chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });
            }
            else
            {
                chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });
            }

            return Json(new SelectList(chuMemberList, "MemberId", "strMemberFullName"));
        }

        public JsonResult GetTransferMemberDetail(int memberId, int oChurchBodyId)
        {
            // oChurchBodyId = 1029;
            ChurchTransferModel memberTransferDetail = new ChurchTransferModel();
            memberTransferDetail = (from t_cm in _context.ChurchMember.Where(x => x.ChurchBodyId == oChurchBodyId && x.Id == memberId)
                                        //join t_cb in _context.ChurchBody.Where(x => x.Id == oChurchBodyId) on t.ChurchBodyId equals t_cb.Id into abc
                                        //from t_cb in abc
                                    join t_mpx in _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.IsCurrentRank == true) on t_cm.Id equals t_mpx.ChurchMemberId into abcd
                                    from t_mr in abcd.DefaultIfEmpty()
                                    join t_msx in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == oChurchBodyId && x.IsCurrent == true) on t_cm.Id equals t_msx.ChurchMemberId into abcde
                                    from t_ms in abcde.DefaultIfEmpty()
                                    join t_mcsx in _context.MemberChurchUnit.AsNoTracking().Include(t => t.ChurchUnit).Where(x => x.ChurchUnit.IsUnitGen == true && x.IsCurrUnit == true) on t_cm.Id equals t_mcsx.ChurchMemberId into abcdef // age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                    from t_mcs in abcdef.DefaultIfEmpty()
                                    join t_mlrx in _context.MemberChurchRole.AsNoTracking().Include(t => t.ChurchRole).Where(x => x.IsLeadRole == true) on t_cm.Id equals t_mlrx.ChurchMemberId into abcdefg   //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)
                                    from t_mlr in abcdefg.DefaultIfEmpty()
                                        // where t.ChurchBodyId == oChurchBodyId
                                        //  orderby t.LastName ascending
                                    select new ChurchTransferModel
                                    {
                                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "",
                                        strFromMemberPos = t_mr.ChurchRank_NVP != null ? t_mr.ChurchRank_NVP.NVPValue : "",
                                        strFromMemberStatus = t_ms.ChurchMemStatus_NVP != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                        strFromMemberAgeGroup = t_mcs.ChurchUnit != null ? t_mcs.ChurchUnit.Name : "",
                                        strFromMemberRole = t_mlr.ChurchRole != null ? t_mlr.ChurchRole.Name : "",
                                        numFromChurchPositionId = t_mr != null ? t_mr.ChurchRankId : -1,
                                        numFromMemberLeaderRoleId = t_mlr != null ? t_mlr.Id : -1
                                        // strMemberFullName = t.MemberCode + ' ' + (((t.FirstName + ' ' + t.MiddleName).Trim() + " " + t.LastName).Trim() + " " + (!string.IsNullOrEmpty(t.Title) ? "(" + t.Title + ")" : "")).Trim(),                                
                                    }
                            ).FirstOrDefault();

            //index 0
            // chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });


            if (memberTransferDetail != null)
            {
                var photoPath = "~/images/" + (memberTransferDetail.strFromMemberPhotoUrl ?? "df_user_p.png");

                return Json(new
                {
                    success = true,
                    fromMemberPhotoUrl = photoPath,
                    fromMemberPos = memberTransferDetail.strFromMemberPos,
                    fromMemberStatus = memberTransferDetail.strFromMemberStatus,
                    fromMemberAgeGroup = memberTransferDetail.strFromMemberAgeGroup,
                    fromMemberRole = memberTransferDetail.strFromMemberRole,
                    fromChurchPositionId = memberTransferDetail.numFromChurchPositionId,
                    fromMemberLeaderRoleId = memberTransferDetail.numFromMemberLeaderRoleId
                });
            }

            return Json(new { success = false });
        }

        public JsonResult GetChurchBodiesByCategory(int oAppOwnId, int? oCBCategoryId, int? oCurrChurchBodyId, int qryIndx, bool inclCurrLevel = false, bool addEmpty = false)
        {
            List<ChurchBody> chuBodyList = GetChurchBodySubCategoryList(oAppOwnId, oCBCategoryId, oCurrChurchBodyId, qryIndx, inclCurrLevel);

            //bool moreLvl = false;
            //if (qryIndx == 1 || qryIndx == 2) 
            //    moreLvl = chuBodyList.Count > 0; 

            // if (qryIndx == 3) this.strToChurchLevel = oCurrChurchBody.ChurchLevel.CustomName;

            if (addEmpty) chuBodyList.Insert(0, new ChurchBody { Id = 0, Name = "Select" });

            //return Json(new
            //{
            //    moreLevels = moreLvl, 
            //    selData = new SelectList(chuBodyList, "Id", "Name")
            //});


            //var vmMod = TempData.Get<ChurchTransferModel>("oVmCurr"); TempData.Keep();
            ////check for the current 
            //var oSelectList = new SelectList(chuBodyList, "Id", "Name");
            //foreach (var item in oSelectList)
            //    item.Selected = vmMod.strAffliliateChurchBodies.Contains(item.Value);


            // return Json(new { selList = oSelectList, currLevel = ViewBag.strChurchCategLevel });
            return Json(chuBodyList); //oSelectList return Json(new SelectList(chuBodyList, "Id", "Name"));                       
        }

        public List<ChurchBody> GetChurchBodySubCategoryList(int oAppOwnId, int? oCBCategoryId, int? oCurrChurchBodyId, int qryIndx, bool inclCurrLevel = false)
        {
            List<ChurchBody> chuBodyList = new List<ChurchBody>();
            // var oCurrCategChurchBody = _context.ChurchBody.Include(t => t.ChurchLevel).Where(x => x.Id == oCBCategoryId).FirstOrDefault();
            //if (oCurrCategChurchBody == null) { ViewBag.strChurchCategLevel = "Level Unknown"; return chuBodyList; }
            //else { ViewBag.strChurchCategLevel = oCurrCategChurchBody.ChurchLevel?.CustomName; }

            if (qryIndx == 1) // top categories... exclude top-most
            {  //get the index... root category
                if (inclCurrLevel)   //ie.  start from root index
                {
                    chuBodyList = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                        .Where(c => c.AppGlobalOwnerId == oAppOwnId && c.OrgType == "CH" && c.ParentChurchBodyId == null).ToList();
                }
                else
                {
                    var oCBCategory = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                        .Where(c => c.AppGlobalOwnerId == oAppOwnId && c.OrgType == "CH" && c.ParentChurchBodyId == null).FirstOrDefault();
                    if (oCBCategory != null)
                    {
                        oCBCategoryId = oCBCategory.Id;
                        chuBodyList = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Include(t => t.AppGlobalOwner)
                        .Where(c => c.AppGlobalOwnerId == oAppOwnId && c.OrgType == "CH" && c.ParentChurchBodyId == oCBCategoryId &&
                        c.ChurchLevel.LevelIndex == _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oAppOwnId).Min(y => y.LevelIndex) + 1 &&   //(inclCurrLevel ? 0 : 1)
                        c.ChurchLevel.LevelIndex < _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oAppOwnId).Max(y => y.LevelIndex))
                        .OrderBy(c => c.ParentChurchBodyId).ThenBy(c => c.Name)
                        .ToList();
                    }
                }

                if (chuBodyList.Count > 0) ViewBag.strChurchCategLevel = chuBodyList[0].ChurchLevel?.CustomName;
                else ViewBag.strChurchCategLevel = "Level Unknown";
            }
            else if (qryIndx == 2) // next sub categories
            {
                if (oCBCategoryId != null)
                {
                    chuBodyList = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Include(t => t.AppGlobalOwner)
                   .Where(c => c.AppGlobalOwnerId == oAppOwnId && c.OrgType == "CH" && c.ParentChurchBodyId == oCBCategoryId &&
                    c.ChurchLevel.LevelIndex < _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oAppOwnId).Max(y => y.LevelIndex))
                    .OrderBy(c => c.ParentChurchBodyId).ThenBy(c => c.Name)
                    .ToList();
                }

                if (chuBodyList.Count > 0) ViewBag.strChurchCategLevel = chuBodyList[0].ChurchLevel?.CustomName;
                else ViewBag.strChurchCategLevel = "Level Unknown";
            }
            else if (qryIndx == 3) // bottom categories... congregations
            {
                if (oCBCategoryId != null)
                {
                    chuBodyList = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Include(t => t.AppGlobalOwner)
                    .Where(c => c.AppGlobalOwnerId == oAppOwnId && c.OrgType == "CN" && c.ParentChurchBodyId == oCBCategoryId && c.Id != oCurrChurchBodyId &&
                    // c.ChurchLevel.LevelIndex == _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oAppOwnId).Min(y => y.LevelIndex) + 1 &&
                    c.ChurchLevel.LevelIndex == _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oAppOwnId).Max(y => y.LevelIndex))
                    .OrderBy(c => c.ParentChurchBodyId).ThenBy(c => c.Name)
                    .ToList();
                }

                if (chuBodyList.Count > 0) ViewBag.strChurchCategLevel = chuBodyList[0].ChurchLevel?.CustomName;
                else ViewBag.strChurchCategLevel = "Level Unknown";
            }

            return chuBodyList;
        }


        public List<ChurchBody> GetChurchBodyList(int oChurchBodyId)
        {
            var oChurchBody = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Include(t => t.AppGlobalOwner).Where(x => x.Id == oChurchBodyId).FirstOrDefault();
            //only Top leaders can perform Transfers == CM, CA, CL
            List<ChurchBody> chuBodyList = new List<ChurchBody>();
            if (oChurchBody == null) return chuBodyList;

            chuBodyList = _context.ChurchBody
                .Where(c => c.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId)
                .OrderBy(c => c.ParentChurchBodyId).ThenBy(c => c.Name)
                .ToList();

            //.Select(c => new SelectListItem()
            //{
            //    Value = c.Id.ToString(),
            //    Text = c.Name
            //})
            // .OrderBy(c => c.Text)
            //.ToList();

            //index 0
            //chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });

            return chuBodyList; //Json(new SelectList(chuMemberList, "MemberId", "strMemberFullName"));            
        }

        public List<ChurchBody> GetChurchBodyList(ChurchBody oCurrChurchBody)
        {
            List<ChurchBody> chuBodyList = new List<ChurchBody>();
            if (oCurrChurchBody == null) return chuBodyList;

            chuBodyList = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Include(t => t.AppGlobalOwner)
                .Where(c => c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId)
                .OrderBy(c => c.ParentChurchBodyId).ThenBy(c => c.Name)
                .ToList();

            //.Select(c => new SelectListItem()
            //{
            //    Value = c.Id.ToString(),
            //    Text = c.Name
            //})
            // .OrderBy(c => c.Text)
            //.ToList();

            //index 0
            //chuMemberList.Insert(0, new MemberProfileVM { MemberId = 0, strMemberFullName = "Select" });

            return chuBodyList; //Json(new SelectList(chuMemberList, "MemberId", "strMemberFullName"));            
        }

        public JsonResult GetChurchBodies(int oChurchBodyId)
        {
            List<ChurchBody> chuBodyList = GetChurchBodyList(oChurchBodyId);
            chuBodyList.Insert(0, new ChurchBody { Id = 0, Name = "Select" });
            return Json(new SelectList(chuBodyList, "Id", "Name"));
        }

        public JsonResult GetToChurchBodies(int oChurchBodyId) //, ChurchBody oChurchBody)
        {
            List<ChurchBody> chuBodyList = GetChurchBodyList(oChurchBodyId).Where(c => c.Id != oChurchBodyId).ToList();
            chuBodyList.Insert(0, new ChurchBody { Id = 0, Name = "Select" });
            return Json(new SelectList(chuBodyList, "Id", "Name"));
        }

        public JsonResult FilterChurchBodyList_BaseLevel(int oChurchBodyId, string strFilterVal)
        {
            //this list actually must be held in memory unti next reload... finetune later! 
            var oCBList = GetChurchBodyList(oChurchBodyId).Where(c => c.OrgType == "CN").ToList();  //, oTransferType == "MT" ? null : oTransferType);
            if (strFilterVal != null)
            {
                oCBList = oCBList.Where(c => c.Name.ToUpper().Contains(strFilterVal.ToUpper()) || c.GlobalChurchCode?.Contains(strFilterVal) == true).ToList();
            }
            oCBList.Insert(0, new ChurchBody { Id = -1, Name = "Select" });
            return Json(new SelectList(oCBList, "Id", "Name"));

            // this.strToChurchLevel = oCurrChurchBody.ChurchLevel.CustomName;
        }

        public List<ChurchRole> GetLeaderRolesList(int oChurchBodyId)
        {
            var oCurrChurchBody = _context.ChurchBody.AsNoTracking().Where(x => x.Id == oChurchBodyId).FirstOrDefault();
            List<ChurchRole> oRoleList = new List<ChurchRole>();
            if (oCurrChurchBody == null) return oRoleList;

            oRoleList = _context.ChurchRole.AsNoTracking().Include(t => t.ParentChurchRole)
                .Where(c => c.OwnedByChurchBodyId == oCurrChurchBody.Id && c.Status == "A" &&
                                                    (c.ParentChurchRole.IsApplyToClergyOnly == true || (c.ParentChurchRole.IsApplyToClergyOnly == false && c.ParentChurchRole.IsAdhocRole == false)))
                .OrderBy(c => c.RankIndex).ThenBy(c => c.Name).ToList();

            return oRoleList;
        }

        public JsonResult GetSectorLeaderRoles(int oChurchBodyId, int oLeadCategId)
        {
            List<ChurchRole> oRoleList = GetLeaderRolesList(oChurchBodyId)
                .Where(c => c.ParentRoleId != oLeadCategId).ToList();
            oRoleList.Insert(0, new ChurchRole { Id = 0, Name = "Select" });
            return Json(new SelectList(oRoleList, "Id", "Name"));
        }

        public JsonResult GetLeaderRoleBySector(int oChurchBodyId, int oSectorId)
        {
            List<ChurchRole> oSecRoleList = _context.ChurchRole.AsNoTracking().Include(t => t.ApplyToChurchUnit).Include(t => t.ParentChurchRole)
                .Where(c => c.OwnedByChurchBodyId == oChurchBodyId && c.ApplyToChurchUnitId == oSectorId &&
                (c.ParentChurchRole.IsApplyToClergyOnly == true || (c.ParentChurchRole.IsApplyToClergyOnly == false && c.ParentChurchRole.IsAdhocRole == false)))
                .OrderBy(c => c.RankIndex).ThenBy(c => c.Name).ToList();
            //oRoleList.Insert(0, new LeaderRole { Id = 0, RoleName = "Select" });
            return Json(new SelectList(oSecRoleList, "Id", "Name"));
        }

        public List<ChurchUnit> GetChurchSectorList(ChurchBody oCurrChurchBody)
        {
            List<ChurchUnit> oSectorList = new List<ChurchUnit>();
            if (oCurrChurchBody == null) return oSectorList;

            oSectorList = _context.ChurchUnit.AsNoTracking().Include(t => t.ParentChurchUnit).Where(c => c.OwnedByChurchBodyId == oCurrChurchBody.Id && c.Status == "A") // && c.ChurchSectorCategory.IsMainstream)
               .OrderBy(c => c.Name).ToList();

            return oSectorList;
        }

        public List<ChurchUnit> GetChurchSectorList(int oChurchBodyId, int? oAppOwnId = null)
        {
            var oCurrChurchBody = _context.ChurchBody.Where(x => x.Id == oChurchBodyId).FirstOrDefault();
            List<ChurchUnit> oSectorList = new List<ChurchUnit>();
            if (oCurrChurchBody == null) return oSectorList;

            oSectorList = _context.ChurchUnit.Include(t => t.ParentChurchUnit)
                .Where(c => (c.AppGlobalOwnerId == oAppOwnId || oAppOwnId == null) && c.OwnedByChurchBodyId == oCurrChurchBody.Id &&
                c.Status == "A") // && c.ChurchSectorCategory.IsMainstream)
                .OrderBy(c => c.Name).ToList();

            return oSectorList;
        }

        public JsonResult GetCategorySectors(int oChurchBodyId, int oSectorCategId, int? oAppOwnId = null)
        {
            List<ChurchUnit> oSectorList = GetChurchSectorList(oChurchBodyId, oAppOwnId).Where(c => c.ParentUnitId != oSectorCategId).ToList();
            oSectorList.Insert(0, new ChurchUnit { Id = 0, Name = "Select" });
            return Json(new SelectList(oSectorList, "Id", "Name"));
        }

        public JsonResult GetChurchSectorByCategory(int oChurchBodyId, int oSectorCategId, int? oAppOwnId = null)
        {
            List<ChurchUnit> oSectorList = GetChurchSectorList(oChurchBodyId, oAppOwnId)
                .Where(c => c.ParentUnitId == oSectorCategId).ToList();
            // oSectorList.Insert(0, new ChurchSector { Id = 0, Name = "Select" });
            return Json(new SelectList(oSectorList, "Id", "Name"));
        }
        public JsonResult GetChurchSectorCategory(int oChurchBodyId, int? oAppOwnId = null)
        {
            List<ChurchUnit> oCategList = new List<ChurchUnit>();
            var oCurrChurchBody = _context.ChurchBody.AsNoTracking().Where(x => x.Id == oChurchBodyId).FirstOrDefault();
            if (oCurrChurchBody != null)
            {
                oCategList = _context.ChurchUnit.AsNoTracking() //.Include(t => t.ChurchBody) 
                    .Where(c => (c.AppGlobalOwnerId == oAppOwnId || oAppOwnId == null) && //c.ChurchBodyId == oCurrChurchBody.Id 
               c.Status == "A") // && c.ChurchSectorCategory.IsMainstream)
              .OrderBy(c => c.Name).ToList();
                // oSectorList.Insert(0, new ChurchSectorCategory { Id = 0, Name = "Select" });
            }

            return Json(new SelectList(oCategList, "Id", "Name"));
        }



        public IActionResult OpenReqAppr_MT(int? oCBid, int id = 0)  //int dxn,   reqDxn = 1 : incoming, reqDxn = 2: outgoing  including Scope = I or E
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

                var loadLim = false; var setIndex = 0;
                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                var oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCBid).FirstOrDefault();


                var oCTModel = new ChurchTransferModel();
                oCTModel.oChurchBody = oCB;
                var oCurrChurchBody = oCB;


                //get the selected transfer... 
                oCTModel = GetMemberTransferApprovalRequestDetail(oCB, id, this._oLoggedUser.ChurchMemberId);



                //reassigning...
                // oCTModel.oCoreChurchlifeVM = oCoreChuLife;


                oCTModel.setIndex = setIndex;
                oCTModel.oUserId_Logged = this._oLoggedUser.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                /// oCTModel.oMemberId_Logged = this._oLoggedUser.Id;
                //
                oCTModel.oAppGloOwnId = oAppGloOwnId;
                oCTModel.oChurchBodyId = oCBid;
                // var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
                oCTModel.oChurchBody = oCurrChurchBody;  // != null ? oCurrChuBody : null;

                // oCTModel = this.populateLookups_CT(oCTModel, oCurrChurchBody); 

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

                return View("OpenReqAppr_MT", oCTModel);

            }

            catch (Exception ex)
            {
                return View("_ErrorPage");
            }

        }

        public IActionResult OpenReq_CT(int? oCBid, int dxn = 2, int svc_ndx = 1, int id = 0)
        {   //reqDxn = 1 : incoming, reqDxn = 2: outgoing /// including Scope = I or E
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

                var loadLim = false; var setIndex = 0;
                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                var oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCBid).FirstOrDefault();


                var oCTModel = new ChurchTransferModel();
                oCTModel.oChurchBody = oCB;
                var oCurrChurchBody = oCB;


                if (dxn == 1)   /// incoming 
                {
                    var oCTD = GetClergyTransferDetail_Incoming(oCurrChurchBody, id);
                    if (oCTD != null) oCTModel = oCTD;
                    else return View("_ErrorPage");
                }

                else if (dxn == 2)   /// outgoing
                {
                    var currTransf = (from t_cm in _context.ChurchTransfer
                                      .Where(x => x.FromChurchBodyId == oCBid && x.Id == id && x.TransferType == "CT" && // x.TransferSubType == oCLGTransfDet_MDL.oChurchTransfer.TransferSubType &&
                                      (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.ReqStatus != "U"))
                                      select t_cm).ToList();

                    ViewBag.IsPendingTransfer = currTransf.Count > 0;

                    var oCTD = GetClergyTransferDetail_Outgoing(oCurrChurchBody, id);  // GetMemberTransferDetail_Outgoing(oCurrChurchBody, id);//oCBid
                    if (oCTD != null) oCTModel = oCTD;
                    else return View("_ErrorPage");
                }
                else
                {
                    var oCTD = GetClergyTransferDetail_Outgoing(oCurrChurchBody, id); //oCBid
                    if (oCTD != null) oCTModel = oCTD;
                    else return View("_ErrorPage");
                }


                oCTModel.serviceTask = svc_ndx;    //church worker = 1, approver = 2, member self-service = 3
                oCTModel.strTransferDxn = dxn == 1 ? "Incoming" : "Outgoing";    // incoming = 1, outgoing = 2, 0 == both
                                                                                 //oCTModel.oCurrLoggedMemberId = oCurrChuMemberId_LogOn;
                                                                                 //oCTModel.oCurrLoggedMember = oCurrChuMember_LogOn;
                                                                                 //oChuTransfDet_MDL.dx = dxn;

                //reassing...
                // oCTModel.oCoreChurchlifeVM = oCoreChuLife;


                oCTModel.setIndex = setIndex;
                oCTModel.oUserId_Logged = this._oLoggedUser.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                /// oCTModel.oMemberId_Logged = this._oLoggedUser.Id;
                //
                oCTModel.oAppGloOwnId = oAppGloOwnId;
                oCTModel.oChurchBodyId = oCBid;
                // var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
                oCTModel.oChurchBody = oCurrChurchBody;  // != null ? oCurrChuBody : null;

                // oCTModel = this.populateLookups_CT(oCTModel, oCurrChurchBody); 

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

                return View("OpenReq_CT", oCTModel);

            }
            catch (Exception ex)
            {
                return View("_ErrorPage");
            }

        }

        public IActionResult OpenReqAppr_CT(int? oCBid, int id = 0)  //int dxn,   reqDxn = 1 : incoming, reqDxn = 2: outgoing  including Scope = I or E
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

                var loadLim = false; var setIndex = 0;
                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                var oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCBid).FirstOrDefault();


                var oCTModel = new ChurchTransferModel();
                oCTModel.oChurchBody = oCB;
                var oCurrChurchBody = oCB;


                //get the selected transfer... 
                oCTModel = GetClergyTransferApprovalRequestDetail(oCB, id, this._oLoggedUser.ChurchMemberId);


                //reassing...
                // oCTModel.oCoreChurchlifeVM = oCoreChuLife;


                oCTModel.setIndex = setIndex;
                oCTModel.oUserId_Logged = this._oLoggedUser.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                /// oCTModel.oMemberId_Logged = this._oLoggedUser.Id;
                //
                oCTModel.oAppGloOwnId = oAppGloOwnId;
                oCTModel.oChurchBodyId = oCBid;
                // var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
                oCTModel.oChurchBody = oCurrChurchBody;  // != null ? oCurrChuBody : null;

                // oCTModel = this.populateLookups_CT(oCTModel, oCurrChurchBody); 

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();

                return View("OpenApprReq_CT", oCTModel);

            }
            catch (Exception ex)
            {
                return View("_ErrorPage");
            }

        }


        private List<ChurchTransferModel> GetTransfers_All(ChurchBody oCurrChurchBody, string strTransType, int svc_ndx) //archive and co...
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody;


                var oChuTransfList = (
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody) //.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel) .Include(t => t.RequestorChurchMember).Include(t => t.ChurchMember)
                             .Where(x => x.TransferType == strTransType &&
                             (svc_ndx != 4 || (svc_ndx == 4 && x.ReqStatus == "Z" && x.RequestorChurchBodyId == oCurrChurchBody.Id)))
                        // x.ToChurchBodyId == oCurrChurchBody.Id && x.CurrentScope == "E")    // && x.RequestorMemberId == oCurrLoggedUser_MemberId)
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                    from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                       _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()

                        // from t_nvp in _context.AppUtilityNVP.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.ReasonId == x.Id).DefaultIfEmpty()
                        //from t_aa in _context.ApprovalAction.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.IApprovalActionId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)

                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oChurchBody = oCurrChurchBody,  // t_ct.RequestorChurchBody,
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,
                        strRequestorChurchLevel = t_cb_r.ChurchLevel != null ? t_cb_r.ChurchLevel.CustomName : "",  //t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strRequestorChurchBody = t_cb_r.Name,  // t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        strFromChurchLevel = t_cb_f.ChurchLevel != null ? t_cb_f.ChurchLevel.CustomName : "",  // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_cb_f.Id, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_cb_f.Name, //t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strToChurchLevel = t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "",  //t_cb_t != null ? t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_cb_t.Name, // t_cb_t != null ? t_cb_t.Name : "",
                        numToChurchBodyId = t_cb_t != null ? t_cb_t.Id : -1,

                        strReason = t_ct.TransferReason, //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        //strTransferDxn = "Incoming",
                        numTransferDxn = (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? 2 : t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? 1 : 0,
                        // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                        strTransMessage = t_ct.CustomTransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                        strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim()
                        strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                                                    ((t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? ". Request to " + t_cb_t.Name :
                                                        t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? ". Request from " + t_cb_f.Name : ""),
                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                        strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                        strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),

                        strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",
                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                        // strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                        // strMovementStatus = GetRequestStatusDesc(t_ct.Status)//,   // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                        // lsCongAffliliates = GetChurchAffiliates(t_ct.AttachedToChurchBodyList)

                        // Process Status -- Received. Approval in progress, Received. Approval complete [host congregation], Not Acknowleged [target congregation]
                        // Transfer Status -- In Transit, Transferred, In Complete etc
                    })
                              //.OrderBy(x => x.oChurchTransfer.Status)
                              //.ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                              .ToList();

                if (oChuTransfList.Count > 0)
                    oChuTransfList
                        .OrderBy(x => x.oChurchTransfer.ReqStatus)
                        .ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                        .ToList();

                foreach (var oCT in oChuTransfList)
                {
                    if (oCT.oChurchTransfer != null) //if (oChuTransfDet_VM.oChurchTransfer.RequireApproval)
                        if (oCT.oChurchTransfer.CurrentScope == "I")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.IApprovalAction);
                        else if (oCT.oChurchTransfer.CurrentScope == "E")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.EApprovalAction);

                    oCT.strTransferDxn = oCT.numTransferDxn == 1 ? "Incoming" : oCT.numTransferDxn == 2 ? "Outgoing" : "";
                }

                return oChuTransfList;

            }
            catch (Exception ex)
            {
                return null;
            }

        }



        private List<ChurchTransferModel> GetTransferList(ChurchBody oCurrChurchBody, int dxn = 2, int svc_ndx = 1, string strTransType = "MT",
           ChurchMember oCM_Logged = null, bool? blReqHis = false, int vw = 0, int? id = null)  //bool showOwnerOnly=false, 
        {   // dxn = 1 (incoming), dxn = 2 (outgoing) ; svc_ndx = 1 (church worker), svc_ndx = 2 (approver), svc_ndx = 3 (self-service) 
            try
            {   /// x.ReqStatus != "Z" -- archived ... history
                var oCMid = oCM_Logged != null ? oCM_Logged.Id : (int?)null; 
                int? oCBid = oCurrChurchBody != null ? oCurrChurchBody.Id : this._oLoggedCB.Id;
                int? oAGOid = oCurrChurchBody != null ? oCurrChurchBody.AppGlobalOwnerId : this._oLoggedAGO.Id;

                var tm = System.DateTime.Now.Date;   // only keep in the ToCB tray [incoming requests] ... not more than 3 days [72 hours]! -- X - cancelled/ terminated, R - recalled, Y - done. transferred, C - closed, Z - archived
                var oChuTransfList = ( //blReqHistory   ... oCurrLoggedUser_MemberId
                             from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody) //.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel) .Include(t => t.RequestorMember).Include(t => t.ChurchMember)
                                //.Include(t => t.TempMemTypeCodeFrCB).Include(t => t.TempMemRankToCB_NVP).Include(t => t.TempMemTypeFrCB_NVP)
                                .Where(x => x.TransferType == strTransType && (vw == 0 || (vw > 0 && x.Id == id)) &&
                                      (((dxn == 0 || dxn == 1) && (x.ReqStatus != "N" && x.ReqStatus != "K") && /// x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "Y" && x.ReqStatus != "C" && x.ReqStatus != "Z") || ((x.ReqStatus == "X" || x.ReqStatus == "R" || x.ReqStatus == "Y" || x.ReqStatus == "C" || x.ReqStatus == "Z") && x.StatusModDate >= tm.AddHours(-72).Date) &&
                                                                  (blReqHis == null || blReqHis == false) && x.WorkSpanStatus != "C" && x.ToChurchBodyId == oCBid && (svc_ndx == 1 || svc_ndx == 2 || (svc_ndx == 3 && x.RequestorMemberId == oCMid))) ||          // current incoming... Active /Deactive [null]
                                       ((dxn == 0 || dxn == 1) && (x.ReqStatus != "N" && x.ReqStatus != "K") && /// x.ReqStatus != "X" || x.ReqStatus != "R" || x.ReqStatus != "Y" || x.ReqStatus != "C" || x.ReqStatus != "Z") || ((x.ReqStatus == "X" || x.ReqStatus == "R" || x.ReqStatus == "Y" || x.ReqStatus == "C" || x.ReqStatus == "Z") && x.StatusModDate >= tm.AddHours(-72).Date) &&
                                                                  (blReqHis == null || blReqHis == true ) && x.WorkSpanStatus == "C" && x.ToChurchBodyId == oCBid && (svc_ndx == 1 || svc_ndx == 2 || (svc_ndx == 3 && x.RequestorMemberId == oCMid))) ||         // history incoming
                                       ((dxn == 0 || dxn == 2) && (blReqHis == null || blReqHis == false) && x.WorkSpanStatus != "C" && x.RequestorChurchBodyId == oCBid && (svc_ndx == 1 || svc_ndx == 2 || (svc_ndx == 3 && x.RequestorMemberId == oCMid))) || // current outgoing
                                       ((dxn == 0 || dxn == 2) && (blReqHis == null || blReqHis == true ) && x.WorkSpanStatus == "C" && x.RequestorChurchBodyId == oCBid && (svc_ndx == 1 || svc_ndx == 2 || (svc_ndx == 3 && x.RequestorMemberId == oCMid))))     // history outgoing
                                    )

                            /// pick the exact approver who has logged on to approve... NB:- the Approver can be anywhere, from any CB!
                             from t_aas in _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApprovalAction).Include(t => t.ApproverChurchMember)
                                    .Where(x => x.AppGlobalOwnerId == t_ct.AppGlobalOwnerId && svc_ndx == 2  && x.ApproverChurchBodyId == oCBid && x.ApproverChurchMemberId == oCMid &&
                                                x.ApprovalAction.CallerRefId == t_ct.Id && x.ApprovalAction.ProcessSubCode == strTransType && 
                                                (((dxn == 0 || dxn == 1) && x.ApprovalAction.ProcessCode=="TRF_IN") || ((dxn == 0 || dxn == 2) && x.ApprovalAction.ProcessCode == "TRF_OT"))).DefaultIfEmpty()
                                       
                                    
                                    /// || x.ApprovalActionId == oReqActionId))
                                 //    // .Where(x => x.RequestorChurchBodyId == oCurrChuBodyId && (showOwnerOnly == false || (showOwnerOnly == true && x.RequestorMemberId == oCurrLoggedUser_MemberId)))
                                 //    //.Where(x => x.RequestorChurchBodyId == oCurrChuBodyId && x.RequestorMemberId == oCurrLoggedUser_MemberId ) //x.CurrentScope == "I" && 

                             from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.Id == t_ct.FromChurchBodyId)
                             from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.Id == t_ct.RequestorChurchBodyId)
                             from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()  // RT -- role trans
                             from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                             from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                                 //from t_mt in _context.MemberType.AsNoTracking().Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                 //               .Take(1).DefaultIfEmpty() 
                                 //from t_mr in _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()  
                                 //from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()  
                                 //from t_mcu in _context.MemberChurchUnit.AsNoTracking().Include(t => t.ChurchUnit).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()                                                                                                                                                                // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                                 //from t_mcr in _context.MemberChurchRole.AsNoTracking().Include(t => t.ChurchRole).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()

                             from t_mt in _context.MemberType.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                           .Take(1).DefaultIfEmpty()
                             from t_mr in _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_ms in _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_mcu in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_mcr in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()

                             from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()
                             from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()

                                 // from t_nvp in _context.AppUtilityNVP.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.ReasonId == x.Id).DefaultIfEmpty()
                                 //from t_aa in _context.ApprovalAction.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.IApprovalActionId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                             {
                                 oChurchBody = oCurrChurchBody,  // t_ct.RequestorChurchBody,  //only outgoing requests possible here... requesting cong
                                 oRequestorChurchBody = t_ct.RequestorChurchBody,
                                 oChurchTransfer = t_ct,
                                 strRequestorChurchLevel = t_cb_r.ChurchLevel != null ? t_cb_r.ChurchLevel.CustomName : "",  //t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                                 strRequestorChurchBody = t_cb_r.Name,  // t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                                 strFromChurchLevel = t_cb_f.ChurchLevel != null ? t_cb_f.ChurchLevel.CustomName : "",  // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                                 numFromChurchBodyId = t_cb_f.Id, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                                 strFromChurchBody = t_cb_f.Name, //t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                                 strToChurchLevel = t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "",  //t_cb_t != null ? t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "" : "",
                                 strToChurchBody = t_cb_t != null ? t_cb_t.Name : "", // t_cb_t != null ? t_cb_t.Name : "",

                                 strFromChurchBodyDesc = t_cb_f != null ? (t_cb_f.ParentChurchBody != null ? (t_cb_f.ParentChurchBody.ChurchLevel != null ?
                                 (t_cb_f.ParentChurchBody.ChurchLevel.LevelIndex > 1 ? t_cb_f.Name + ", " + t_cb_f.ParentChurchBody.Name : t_cb_f.Name) : t_cb_f.Name) : t_cb_f.Name) : "",

                                 strToChurchBodyDesc = t_cb_t != null ? (t_cb_t.ParentChurchBody != null ? (t_cb_t.ParentChurchBody.ChurchLevel != null ?
                                 (t_cb_t.ParentChurchBody.ChurchLevel.LevelIndex > 1 ? t_cb_t.Name + ", " + t_cb_t.ParentChurchBody.Name : t_cb_t.Name) : t_cb_t.Name) : t_cb_t.Name) : "",

                                 strReason = t_ct.TransferReason, //t_ct.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                                 strTransMessage = t_ct.CustomTransMessage, // t_ct.TransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                                 strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                                 strTransferDxn = t_ct.RequestorChurchBodyId == oCBid ? "Outgoing" : t_ct.ToChurchBodyId == oCBid ? "Incoming" : "Indeterminate" , //  dxn == 1 ? "Incoming" : "Outgoing",
                                 numTransferDxn = t_ct.RequestorChurchBodyId == oCBid ? 2 : t_ct.ToChurchBodyId == oCBid ? 1 : 0,// dxn,
                                 // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                 strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                 strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),

                                 numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                                 //strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",

                                 strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",

                                 strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                                 strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",

                                 strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? GetConcatMemberName(t_mcr_r.ChurchMember.Title, t_mcr_r.ChurchMember.FirstName, t_mcr_r.ChurchMember.MiddleName, t_mcr_r.ChurchMember.LastName, false, false, true, true, false) : "" : "",

                                 strFromMemberFullName = GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, true, true, false),
                                 strTransfMemberDesc = GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, true, true, false) + (dxn == 1 ? (t_cb_f != null ? ". Request from: " + t_cb_f.Name : "") : (t_cb_t != null ? ". Request to: " + t_cb_t.Name : "")),

                                 // strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                 //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                 strFromMemberPos = t_mr != null ? t_mr.ChurchRank_NVP.NVPValue : "",

                                 strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                 strFromMemberRank = t_mr != null ? t_mr.ChurchRank_NVP.NVPValue : "",
                                 strFromMemberType = t_mt != null ? GetMemTypeDesc(t_mt.MemberTypeCode) : "",
                                 numFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.Id : (int?)null,
                                 numFromMemberRank = t_mr != null ? t_mr.ChurchRank_NVP.Id : (int?)null,
                                 numFromMemberType = t_mt != null ? t_mt.MemberTypeCode : null,
                                 ///
                                 numTempMemStatusIdFrCB = t_ct.TempMemStatusIdFrCB, 
                                 numTempMemRankIdFrCB = t_ct.TempMemRankIdFrCB,
                                 numTempMemTypeCodeFrCB = t_ct.TempMemTypeCodeFrCB,
                                 /////
                                 strTempMemStatusIdFrCB = t_ct.TempMemStatusFrCB_NVP != null ? t_ct.TempMemStatusFrCB_NVP.NVPValue : "",
                                 strTempMemRankIdFrCB = t_ct.TempMemRankFrCB_NVP != null ? t_ct.TempMemRankFrCB_NVP.NVPValue : "",
                                 strTempMemTypeCodeFrCB = GetMemTypeDesc(t_ct.TempMemTypeCodeFrCB),

                                 ///
                                 numTempMemStatusIdToCB = t_ct.TempMemStatusIdToCB,
                                 numTempMemRankIdToCB = t_ct.TempMemRankIdToCB,
                                 numTempMemTypeCodeToCB = t_ct.TempMemTypeCodeToCB,
                                 ///
                                 strTempMemStatusIdToCB = t_ct.TempMemStatusToCB_NVP != null ? t_ct.TempMemStatusToCB_NVP.NVPValue : "",
                                 strTempMemRankIdToCB = t_ct.TempMemRankToCB_NVP != null ? t_ct.TempMemRankToCB_NVP.NVPValue : "",
                                 strTempMemTypeCodeToCB = GetMemTypeDesc(t_ct.TempMemTypeCodeToCB),

                                 strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                                 strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                                 strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "",
                                 strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                                 // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                                 //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                 // strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),

                                 strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",

                                 // strMovementStatus = GetRequestStatusDesc(t_ct.Status),   // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                               //  strMembershipStatus = GetMemTypeDesc(t_ct.MembershipStatus),
                                 // lsCongAffliliates = GetChurchAffiliates(t_ct.AttachedToChurchBodyList)

                                 // appReqDate = t_ct.IApprovalAction != null ? t_ct.IApprovalAction.ActionRequestDate : null
                                 //appReqDate = t_ct.EApprovalAction != null ? t_ct.EApprovalAction.ActionRequestDate : null,

                                 strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus), // + (!string.IsNullOrEmpty(t_ct.ApprovalStatusComments) ? " " + t_ct.ApprovalStatusComments.ToLower() : ""),
                                 strApprovalStatusDetail = GetRequestProcessStatusDesc(t_ct.ApprovalStatus) + (!string.IsNullOrEmpty(t_ct.ApprovalStatusComments) ? " " + t_ct.ApprovalStatusComments.ToLower() : ""),
                                 strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false), // + (!string.IsNullOrEmpty(t_ct.StatusComments) ? " " + t_ct.StatusComments.ToLower() : ""),
                                 strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true) + (!string.IsNullOrEmpty(t_ct.ReqStatusComments) ? " " + t_ct.ReqStatusComments.ToLower() : ""),
                                 strWorkSpanStatus = GetStatusDesc(t_ct.WorkSpanStatus),
                                 // strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                                 oApprovalXActionStep = t_aas,
                                 oApprovalXAction = t_aas != null ? t_aas.ApprovalAction : null,
                                 ///
                                 strApprovalXChurchBody = t_aas != null ? t_aas.ChurchBody != null ? t_aas.ChurchBody.Name : "" : "",
                                 strApprovalXReqDate = t_aas != null ? t_aas.StepRequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.StepRequestDate) : "N/A" : "",
                                 strApprovalXDate = t_aas != null ? t_aas.ActionDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.ActionDate) : "N/A" : "",
                                 strApproverXChurchBody = t_aas != null ? t_aas.ApproverChurchBody != null ? t_aas.ApproverChurchBody.Name : "" : "",
                                 strApproverXChurchMember = t_aas != null ? t_aas.ApproverChurchMember != null ? GetConcatMemberName(t_aas.ApproverChurchMember.Title, t_aas.ApproverChurchMember.FirstName, t_aas.ApproverChurchMember.MiddleName, t_aas.ApproverChurchMember.LastName, false, false, true, true, false) : "" : "",
                                 strApproverXChurchRole = t_aas != null ? t_aas.ApproverChurchRole != null ? t_aas.ApproverChurchRole.Name : "N/A" : "",
                                 strApprovalXStepStatus = t_aas != null ? GetRequestProcessStatusDesc(t_aas.ActionStepStatus) : ""

                             })
                              //.OrderBy(x => x.oChurchTransfer.Status)
                              //.ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                              .ToList();

                if (vw == 0 && oChuTransfList.Count > 1)
                    oChuTransfList = oChuTransfList // .OrderBy(x => x.oChurchTransfer.ReqStatus) 
                              .OrderByDescending(x => x.oChurchTransfer.RequestDate)
                              .ThenBy(x => x.oChurchTransfer.TransferDate)
                              .ToList();

                //if (vw > 0)
                //{
                //    foreach (var oCT in oChuTransfList)
                //    {
                //        if (oCT.oChurchTransfer != null) //if (oChuTransfDet_VM.oChurchTransfer.RequireApproval)
                //            if (oCT.oChurchTransfer.CurrentScope == "I")
                //                oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.IApprovalAction);

                //            else if (oCT.oChurchTransfer.CurrentScope == "E")
                //                oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.EApprovalAction);
                //    }
                //}


                return oChuTransfList;

            }
            catch (Exception ex)
            {
                return null;
            }

        }



        private List<ChurchTransferModel> GetTransfers_Outgoing(ChurchBody oCurrChurchBody, string strTransType, int dxn = 2, int svc_ndx = 1,
            ChurchMember oCurrChuMember_LogOn = null, bool blReqHis = false)  //bool showOwnerOnly=false, 
        {
            try
            {  //int? oCurrChuBodyId

                var oCMid = oCurrChuMember_LogOn != null ? oCurrChuMember_LogOn.Id : (int?)null;
                var oCMGlobalCode = oCurrChuMember_LogOn != null ? oCurrChuMember_LogOn.GlobalMemberCode : null;

                int? oCurrChuBodyId = oCurrChurchBody != null ? oCurrChurchBody.Id : (int?)null;
                var oChuTransfList = ( //blReqHistory   ... oCurrLoggedUser_MemberId
                             from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody) //.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel) .Include(t => t.RequestorMember).Include(t => t.ChurchMember)
                              .Where(x => x.TransferType == strTransType && x.ReqStatus != "Z" &&
                                (blReqHis == false && x.RequestorChurchBodyId == oCurrChurchBody.Id && (svc_ndx == 1 || (svc_ndx == 3 && x.RequestorMemberId == oCMid))) ||
                                (blReqHis == true && x.ToChurchBodyId == oCurrChurchBody.Id && (svc_ndx == 3 && x.RequestorMember.GlobalMemberCode == oCMGlobalCode))
                              )
                                 //    // .Where(x => x.RequestorChurchBodyId == oCurrChuBodyId && (showOwnerOnly == false || (showOwnerOnly == true && x.RequestorMemberId == oCurrLoggedUser_MemberId)))
                                 //    //.Where(x => x.RequestorChurchBodyId == oCurrChuBodyId && x.RequestorMemberId == oCurrLoggedUser_MemberId ) //x.CurrentScope == "I" && 
                             from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                             from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                             from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                             from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                             from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)


                                 //from t_mt in _context.MemberType.AsNoTracking().Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                 //               .Take(1).DefaultIfEmpty() 
                                 //from t_mr in _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()  
                                 //from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()  
                                 //from t_mcu in _context.MemberChurchUnit.AsNoTracking().Include(t => t.ChurchUnit).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()                                                                                                                                                                // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                                 //from t_mcr in _context.MemberChurchRole.AsNoTracking().Include(t => t.ChurchRole).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId)
                                 //              .Take(1).DefaultIfEmpty()

                             from t_mr in _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_ms in _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_mcu in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_mcr in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()
                             from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId)
                                            .Take(1).DefaultIfEmpty()

                             from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()
                             from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()

                                 // from t_nvp in _context.AppUtilityNVP.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.ReasonId == x.Id).DefaultIfEmpty()
                                 //from t_aa in _context.ApprovalAction.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.IApprovalActionId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                             {
                                 oChurchBody = oCurrChurchBody,  // t_ct.RequestorChurchBody,  //only outgoing requests possible here... requesting cong
                                 oRequestorChurchBody = t_ct.RequestorChurchBody,
                                 oChurchTransfer = t_ct,
                                 strRequestorChurchLevel = t_cb_r.ChurchLevel != null ? t_cb_r.ChurchLevel.CustomName : "",  //t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                                 strRequestorChurchBody = t_cb_r.Name,  // t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                                 strFromChurchLevel = t_cb_f.ChurchLevel != null ? t_cb_f.ChurchLevel.CustomName : "",  // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                                 numFromChurchBodyId = t_cb_f.Id, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                                 strFromChurchBody = t_cb_f.Name, //t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                                 strToChurchLevel = t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "",  //t_cb_t != null ? t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "" : "",
                                 strToChurchBody = t_cb_t != null ? t_cb_t.Name : "", // t_cb_t != null ? t_cb_t.Name : "",

                                 strToChurchBodyDesc = t_cb_t != null ? (t_cb_t.ParentChurchBody != null ? (t_cb_t.ParentChurchBody.ChurchLevel != null ?
                                 (t_cb_t.ParentChurchBody.ChurchLevel.LevelIndex > 1 ? t_cb_t.Name + ", " + t_cb_t.ParentChurchBody.Name : t_cb_t.Name) : t_cb_t.Name) : t_cb_t.Name) : "",

                                 strReason = t_ct.TransferReason, //t_ct.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                                 strTransMessage = t_ct.CustomTransMessage, // t_ct.TransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                                 strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                                 strTransferDxn = "Outgoing",
                                 numTransferDxn = dxn,
                                 // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                 strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                 strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),

                                 numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                                 //strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",

                                 strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",

                                 strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                                 strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",

                                 strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? GetConcatMemberName(t_mcr_r.ChurchMember.Title, t_mcr_r.ChurchMember.FirstName, t_mcr_r.ChurchMember.MiddleName, t_mcr_r.ChurchMember.LastName, false, false, true, true, false) : "" : "",

                                 strFromMemberFullName = GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, true, true, false),
                                 strTransfMemberDesc = GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, true, true, false) + (t_cb_t != null ? ". Request to " + t_cb_t.Name : ""),

                                 // strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                 //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                 strFromMemberPos = t_mr != null ? t_mr.ChurchRank_NVP.NVPValue : "",
                                 strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                 strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                                 strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                                 strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "",
                                 strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                                 // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                                 //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                 // strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),
                                 strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",
                                 // strMovementStatus = GetRequestStatusDesc(t_ct.Status),   // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                                // strMembershipStatus = GetMemTypeDesc(t_ct.MembershipStatus),
                                 // lsCongAffliliates = GetChurchAffiliates(t_ct.AttachedToChurchBodyList)

                                 // appReqDate = t_ct.IApprovalAction != null ? t_ct.IApprovalAction.ActionRequestDate : null
                                 //appReqDate = t_ct.EApprovalAction != null ? t_ct.EApprovalAction.ActionRequestDate : null,

                                 strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),

                                 strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                                 strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true),
                                 // strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                             })
                              //.OrderBy(x => x.oChurchTransfer.Status)
                              //.ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                              .ToList();

                if (oChuTransfList.Count > 0)
                    oChuTransfList = oChuTransfList
                                .OrderBy(x => x.oChurchTransfer.ReqStatus)
                              .ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                              .ToList();

                foreach (var oCT in oChuTransfList)
                {
                    if (oCT.oChurchTransfer != null) //if (oChuTransfDet_VM.oChurchTransfer.RequireApproval)
                        if (oCT.oChurchTransfer.CurrentScope == "I")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.IApprovalAction);
                        else if (oCT.oChurchTransfer.CurrentScope == "E")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.EApprovalAction);
                }

                return oChuTransfList;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
         
        private List<ChurchTransferModel> GetTransfers_Incoming(ChurchBody oCurrChurchBody, string strTransType)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                var oChuTransfList = (
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody) //.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel) .Include(t => t.RequestorMember).Include(t => t.ChurchMember)
                             .Where(x => x.TransferType == strTransType &&
                             (strTransType == "MT" && x.ReqStatus != "Z" && x.ToChurchBodyId == oCurrChurchBody.Id) ||
                             (strTransType == "CT" && (x.ReqStatus == "C" || x.ReqStatus == "Y") && (x.ApprovalStatus == "A" || x.ApprovalStatus == "F") &&
                              (x.FromChurchBodyId == oCurrChurchBody.Id || x.ToChurchBodyId == oCurrChurchBody.Id)))    // && x.RequestorMemberId == oCurrLoggedUser_MemberId)
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                        //from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                        //                _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                        //from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                        //             _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                        //from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                        //              _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                        //from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                        //              _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                        //from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                        //            _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)

                    from t_mr in _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                   .Take(1).DefaultIfEmpty()
                    from t_ms in _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                   .Take(1).DefaultIfEmpty()
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId)
                                   .Take(1).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId)
                                   .Take(1).DefaultIfEmpty()
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId)
                                   .Take(1).DefaultIfEmpty()

                    from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()

                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oChurchBody = oCurrChurchBody, // t_ct.RequestorChurchBody,
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,
                        strRequestorChurchLevel = t_cb_r.ChurchLevel != null ? t_cb_r.ChurchLevel.CustomName : "",  //t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strRequestorChurchBody = t_cb_r.Name,  // t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        strFromChurchLevel = t_cb_f.ChurchLevel != null ? t_cb_f.ChurchLevel.CustomName : "",  // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_cb_f.Id, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_cb_f.Name, //t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strToChurchLevel = t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "",  //t_cb_t != null ? t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_cb_t != null ? t_cb_t.Name : "", // t_cb_t != null ? t_cb_t.Name : "",
                        numToChurchBodyId = t_cb_t != null ? t_cb_t.Id : -1,

                        strToChurchBodyDesc = t_cb_f != null ? (t_cb_f.ParentChurchBody != null ? (t_cb_f.ParentChurchBody.ChurchLevel != null ?
                                 (t_cb_f.ParentChurchBody.ChurchLevel.LevelIndex > 1 ? t_cb_f.Name + ", " + t_cb_f.ParentChurchBody.Name : t_cb_f.Name) : t_cb_f.Name) : t_cb_f.Name) : "",

                        strReason = t_ct.TransferReason, // .oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        strTransferDxn = "Incoming",
                        numTransferDxn = 1,
                        // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),

                        strTransMessage = t_ct.CustomTransMessage, // .oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                        // strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",

                        strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? GetConcatMemberName(t_mcr_r.ChurchMember.Title, t_mcr_r.ChurchMember.FirstName, t_mcr_r.ChurchMember.MiddleName, t_mcr_r.ChurchMember.LastName, false, false, true, true, false) : "" : "",

                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        //strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                        //                        ". Request from " + t_cb_f.Name,   //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim() 

                        strFromMemberFullName = GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, true, true, false),
                        strTransfMemberDesc = GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, false, false, true, true, false) + (t_cb_t != null ? ". Request to " + t_cb_t.Name : ""),

                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                        //strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mr != null ? t_mr.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "",   // df_user_p.png
                        strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                        strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",

                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                        //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        // strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),

                        //strMovementStatus = GetRequestStatusDesc(t_ct.Status)   // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"

                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                        strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true),
                        // strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                    })
                              //.OrderBy(x => x.oChurchTransfer.Status)
                              //.ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                              .ToList();

                if (oChuTransfList.Count > 0)
                    oChuTransfList = oChuTransfList
                              .OrderBy(x => x.oChurchTransfer.ReqStatus)
                              .ThenByDescending(x => x.oChurchTransfer.RequestDate).ThenBy(x => x.oChurchTransfer.ToRequestDate)
                              .ToList();

                foreach (var oCT in oChuTransfList)
                {
                    if (oCT.oChurchTransfer != null)  // if (oCT.oChurchTransfer.RequireApproval)
                        if (oCT.oChurchTransfer.CurrentScope == "I")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.IApprovalAction);
                        else if (oCT.oChurchTransfer.CurrentScope == "E")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.EApprovalAction);
                }

                return oChuTransfList;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private IList<string> GetChurchAffiliates(string strArr) //IDs
        {
            List<string> ls = new List<string>();
            if (strArr != null)
            {
                if (strArr.Length > 0)
                {
                    string[] arr = strArr.Split(',');
                    if (arr.Length > 0)
                        ls.AddRange(arr);
                }
            }

            return ls;
        }

        private IList<ChurchBody> GetChurchAffiliates_CB(string strArr, int? oAppOwnId)  //objects
        {
            List<ChurchBody> ls = new List<ChurchBody>();
            if (strArr != null)
            {
                if (strArr.Length > 0)
                {
                    string[] arr = strArr.Split(',');
                    if (arr.Length > 0)
                    {
                        foreach (var oCBId in arr)
                        {
                            var oCB = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppOwnId && c.Id == int.Parse(oCBId)).FirstOrDefault();
                            if (oCB != null) ls.Add(oCB);
                        }
                    }
                }
            }

            return ls;
        }

        private List<ChurchTransferModel> GetApprovalTransferRequests(ChurchBody oCurrChurchBody, string strTransType, int? oCurrLoggedUser_MemberId = null, bool blReqHistory = false, int? oReqActionId = null)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                var oChuTransfList = (
                    from t_aas in _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApprovalAction).Include(t => t.ApproverMemberChurchRole)
                        .Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId &&
                                (oReqActionId == null && x.ApproverMemberChurchRole.ChurchMemberId == oCurrLoggedUser_MemberId) || (x.ApprovalActionId == oReqActionId))
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody) //.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel) .Include(t => t.RequestorMember).Include(t => t.ChurchMember)
                             .Where(x => x.TransferType == strTransType && x.ReqStatus != "Z" &&
                             x.Id == t_aas.ApprovalAction.CallerRefId && ((x.RequestorChurchBodyId == oCurrChurchBody.Id) || (x.ToChurchBodyId == oCurrChurchBody.Id)) &&   //&& x.ReceivedDate != null
                                            ((blReqHistory == false && oReqActionId != null && (x.IApprovalActionId == t_aas.ApprovalActionId || x.EApprovalActionId == t_aas.ApprovalActionId)) ||
                                             (blReqHistory == true && oReqActionId == null && (x.ReqStatus == "U" || x.ReqStatus == "Y")) ||
                                             (blReqHistory == false && oReqActionId == null && (x.ReqStatus == null || x.ReqStatus == "I" || x.ReqStatus == "T"))))      //x.Status=="I"... first approver makes "I"
                                                                                                                                                                // && x.CurrentScope == "I" || x.CurrentScope == "E"
                    from t_mcr_a in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_aas.ApproverMemberChurchRoleId)
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && t_ct.RequestorMemberId == x.Id)
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && t_ct.ChurchMemberId == x.Id)

                    from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                       _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()

                     from t_aa in _context.ApprovalAction.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && t_ct.IApprovalActionId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)

                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oCurrApprovalActionStep = t_aas,
                        oCurrApprovalActionStepId = t_aas.Id,
                        oCurrApprovalAction = t_aas.ApprovalAction,
                        oCurrApprovalActionId = t_aas.ApprovalActionId,
                        strApproverDesc = (t_mcr_a.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_a.ChurchMember.Title) ? t_mcr_a.ChurchMember.Title : "") + " " + t_mcr_a.ChurchMember.FirstName).Trim() + " " + t_mcr_a.ChurchMember.MiddleName).Trim() + " " + t_mcr_a.ChurchMember.LastName).Trim() : "") +
                                            (t_mcr_a.ChurchRole != null ? (!string.IsNullOrEmpty(t_mcr_a.ChurchRole.Name) ? " (" + t_mcr_a.ChurchRole.Name + ")" : "") : "").TrimEnd(),

                        //(((t_mcr_a.ChurchMember.FirstName + ' ' + t_mcr_a.ChurchMember.MiddleName).Trim() + " " + t_mcr_a.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_mcr_a.ChurchMember.Title) ? "(" + t_mcr_a.ChurchMember.Title + ")" : "")).Trim(),
                        //+ " " + (t_mcr_a.ChurchRole != null, (!string.IsNullOrEmpty(t_mcr_a.ChurchRole.Name) ? "(" + t_mcr_a.ChurchRole.Name + ")" : ""), "").Trim(),
                        strCurrApproverChurchRole = t_mcr_a.ChurchRole.Name.Trim(),
                        strActionRequestDateDesc = t_aas.ApprovalAction.ActionRequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.ApprovalAction.ActionRequestDate) : "N/A",
                        strStepRequestDateDesc = t_aas.StepRequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.StepRequestDate) : "N/A",
                        strStepActionDateDesc = t_aas.ActionDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.ActionDate) : "N/A",
                        strActionStepStatus = GetRequestProcessStatusDesc(t_aas.ActionStepStatus),
                        strActionStatus = GetRequestProcessStatusDesc(t_aas.ApprovalAction.ActionStatus),
                        strApproverComment = t_aas.Comments,
                        //
                        oChurchBody = oCurrChurchBody,  // t_ct.RequestorChurchBody,
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,
                        strRequestorChurchLevel = t_cb_r.ChurchLevel != null ? t_cb_r.ChurchLevel.CustomName : "",  //t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strRequestorChurchBody = t_cb_r.Name,  // t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        strFromChurchLevel = t_cb_f.ChurchLevel != null ? t_cb_f.ChurchLevel.CustomName : "",  // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_cb_f.Id, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_cb_f.Name, //t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strToChurchLevel = t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "",  //t_cb_t != null ? t_cb_t.ChurchLevel != null ? t_cb_t.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_cb_t.Name, // t_cb_t != null ? t_cb_t.Name : "",
                        numToChurchBodyId = t_cb_t != null ? t_cb_t.Id : -1,

                        strReason = t_ct.TransferReason,  //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        // strTransferDxn = t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I" ? "Incoming" : "Outgoing",
                        numTransferDxn = (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? 2 : t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? 1 : 0,
                        // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                        strTransMessage = t_ct.CustomTransMessage, // t_ct.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                        strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                                                    ((t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? ". Request to " + t_cb_t.Name :
                                                      t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? ". Request from " + t_cb_f.Name : ""),
                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",  //until successful [Y] transfer use the strMovementStatus
                        strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                        strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                        
                        strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",
                        //  strMovementStatus = GetRequestStatusDesc(t_ct.Status)   // == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                         
                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),
                    })
                              //By(x => x.oChurchTransfer.ReqStatus).Then
                              //.OrderBy(x => x.oChurchTransfer.Status)
                              //.ThenByDescending(x => x.oCurrApprovalAction.ActionRequestDate).ThenBy(x => x.oCurrApprovalActionStep.ApprovalStepIndex) 
                              .ToList();

                foreach (var oCT in oChuTransfList)
                {
                    if (oCT.oChurchTransfer != null) //if (oCT.oChurchTransfer.RequireApproval)
                        if (oCT.oChurchTransfer.CurrentScope == "I")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.IApprovalAction);
                        else if (oCT.oChurchTransfer.CurrentScope == "E")
                            oCT.strApprovers = _GetApprovers(oCT.oChurchTransfer.EApprovalAction);

                    oCT.strTransferDxn = oCT.numTransferDxn == 1 ? "Incoming" : oCT.numTransferDxn == 2 ? "Outgoing" : "";
                }


                return oChuTransfList;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private ChurchTransferModel GetMemberTransferDetail_Outgoing(ChurchBody oCurrChurchBody, int id) //, bool showOwned=false, int? oCurrLoggedUser_MemberId=null)
        {
            try
            {   // 
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 


                int? oCurrChuBodyId = oCurrChurchBody != null ? oCurrChurchBody.Id : (int?)null;
                var oChuTransfDet_VM = (
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
                         .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorMember).Include(t => t.IApprovalAction).Include(t => t.EApprovalAction)
                             .Where(x => x.Id == id && (x.RequestorChurchBodyId == oCurrChuBodyId || x.ToChurchBodyId == oCurrChuBodyId)) // && (showOwned==false || (showOwned==true && x.RequestorMemberId == oCurrLoggedUser_MemberId)))  //&& x.CurrentScope == "I" 
                  
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                    from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId)
                                 .OrderBy(y => y.ChurchRank_NVP.GradeLevel).Take(1).ToList() : new List<MemberRank>()
                    from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId)
                                 .OrderBy(y => y.ChurchMemStatus_NVP.GradeLevel).Take(1).ToList() : new List<MemberStatus>()
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId)
                                  .OrderBy(y => y.IsCoreArea).Take(1).ToList() : new List<MemberChurchUnit>()                                                                                                                                                                               // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId)
                                  .OrderBy(y => y.ChurchRole.RankIndex).Take(1).ToList() : new List<MemberChurchRole>()
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                    _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId)
                                    .OrderBy(y=> y.ChurchRole.RankIndex).Take(1).ToList() : new List<MemberChurchRole>()   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    
                    from t_crl_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()


                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oChurchBody = oCurrChurchBody, // t_ct.RequestorChurchBody,   //Outgoing requests can only come from requesting congregation
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,

                        strReason = t_ct.TransferReason, // .oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        strTransferDxn = t_ct.RequestorChurchBodyId == oCurrChuBodyId ? "Outgoing" : "Incoming", //"Outgoing",
                        // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                        strTransMessage = t_ct.CustomTransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        numRequestorMemberId = t_cm_r != null ? t_mcr_r.ChurchMemberId : null,

                        strRequestorFullName = t_cm_r != null ? ((((!string.IsNullOrEmpty(t_cm_r.Title) ? t_cm_r.Title : "") + ' ' + t_cm_r.FirstName).Trim() + " " + t_cm_r.MiddleName).Trim() + " " + t_cm_r.LastName).Trim() : "",
                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + " " + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                                        ". Request to " + t_cb_t.Name,

                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                        strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                        strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                        numToChurchBodyId = t_cb_t != null ? t_cb_t.Id : -1,

                        strToMemberRole = t_crl_tr != null ? t_crl_tr.Name : "N/A",
                        strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",

                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                        //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        //strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),                      
                        //strMovementStatus = GetRequestStatusDesc(t_ct.Status) // == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                                                                              // lsCongAffliliates = GetChurchAffiliates(t_ct.AttachedToChurchBodyList)

                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                        strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true),
                        // strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                    }
                    ).FirstOrDefault();


                if (oChuTransfDet_VM != null)
                {
                    if (oChuTransfDet_VM.oChurchTransfer != null) //if (oChuTransfDet_VM.oChurchTransfer.RequireApproval)
                        if (oChuTransfDet_VM.oChurchTransfer.CurrentScope == "I")
                            oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.IApprovalAction);
                        else if (oChuTransfDet_VM.oChurchTransfer.CurrentScope == "E")
                            oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.EApprovalAction);

                    oChuTransfDet_VM.strTransferDxn = oChuTransfDet_VM.numTransferDxn == 1 ? "Incoming" : oChuTransfDet_VM.numTransferDxn == 2 ? "Outgoing" : "";
                }

                return oChuTransfDet_VM;
            }

            catch (Exception ex)
            {
                return null;
            }

        }
    
        private ChurchTransferModel GetClergyTransferDetail_Outgoing(ChurchBody oCurrChurchBody, int id) //, bool showOwned=false, int? oCurrLoggedUser_MemberId=null)
        {
            try
            {// 
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody;  

                int? oCurrChuBodyId = oCurrChurchBody != null ? oCurrChurchBody.Id : (int?)null;
                var oCTModel = (
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
                         .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorMember).Include(t => t.IApprovalAction)  //.Include(t => t.oRoleDesignations)
                             .Where(x => x.Id == id && (x.RequestorChurchBodyId == oCurrChuBodyId || x.ToChurchBodyId == oCurrChuBodyId)) // && (showOwned==false || (showOwned==true && x.RequestorMemberId == oCurrLoggedUser_MemberId)))  //&& x.CurrentScope == "I" 
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Include(t=> t.AppGlobalOwner).Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                    from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                       _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcu.ChurchBodyId && x.Id==t_mcu.ChurchUnitId && x.Generational == true).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()

                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oChurchBody = oCurrChurchBody, // t_ct.RequestorChurchBody,   //Outgoing requests can only come from requesting congregation
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,
                        oAppGloOwn = t_cb_f.AppGlobalOwner,

                        strReason = t_ct.TransferReason,  //t_ct.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        // strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        strTransferDxn = t_ct.RequestorChurchBodyId == oCurrChuBodyId ? "Outgoing" : "Incoming", //"Outgoing",
                        // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),

                        strTransMessage = t_ct.CustomTransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        //strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                        strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + " " + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                                        ". Request to " + t_cb_t.Name,
                        strRequestorChurchBody = t_cb_r.Name,
                        strRequestorChurchLevel = t_cb_r.ChurchLevel.CustomName,
                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                        strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                        strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                        numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,


                        strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                     
                        strCurrScope = t_ct.CurrentScope == "I" ? "Internal" : "External",
                       // strMovementStatus = GetRequestStatusDesc(t_ct.Status), // == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                        strAffliliateChurchBodies = GetChurchAffiliates(t_ct.AttachedToChurchBodyList),
                        // oAffliliateChurchBodies = GetChurchAffiliates_CB(t_ct.AttachedToChurchBodyList, oCurrChurchBody.AppGlobalOwnerId)

                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),

                        
                    }
                    ).FirstOrDefault();


                if (oCTModel != null)
                {
                    oCTModel.strApprovers = _GetApprovers(oCTModel.oChurchTransfer.IApprovalAction);
                    oCTModel.oAffliliateChurchBodies = GetChurchAffiliates_CB(oCTModel.oChurchTransfer.AttachedToChurchBodyList, oCurrChurchBody.AppGlobalOwnerId);

                    oCTModel.strTransferDxn = oCTModel.numTransferDxn == 1 ? "Incoming" : oCTModel.numTransferDxn == 2 ? "Outgoing" : "";

                    oCTModel.oChurchBodyId_Logged = this._oLoggedUser.ChurchMemberId;
                }

                return oCTModel;
            }

            catch (Exception ex)
            {
                return null;
            }

        }

        private ChurchTransferModel GetMemberTransferDetail_Incoming(ChurchBody oCurrChurchBody, int id = 0)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                var oAGOid = this._oLoggedAGO.Id; var oCBid = oCurrChurchBody.Id; 
                var oChuTransfDet_VM = (
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
                         .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.IApprovalAction).Include(t => t.EApprovalAction) //.Include(t => t.ChurchMember) .Include(t => t.RequestorMember)
                        .Where(x => x.Id == id && x.ToChurchBodyId == oCBid && x.CurrentScope == "E")    // && x.RequestorMemberId == oCurrLoggedUser_MemberId) 
                   
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                    from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                       _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcs.ChurchBodyId && x.Id==t_mcs.SectorId && x.Generational == true).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()


                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oChurchBody = oCurrChurchBody,  // t_ct.RequestorChurchBody,
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,

                        strReason = t_ct.TransferReason,  //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        strTransferDxn = "Outgoing",
                        // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                        strTransMessage = t_ct.CustomTransMessage,  //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,

                        strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                                        ". Request from " + t_cb_f.Name,

                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                        strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                        strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                        numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,

                        strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                        //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        //strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),  // t_ct.ReqStatus.Equals("D") ? "Draft" : t_ct.ReqStatus.Equals("P") ? "Pending" : t_ct.ReqStatus.Equals("R") ? "Received" : t_ct.ReqStatus.Equals("X") ? "Terminated" : "N/A"
                        //strMovementStatus = GetRequestStatusDesc(t_ct.Status)   // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"

                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                        strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true),

                        //  strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                    })
                             .FirstOrDefault();

                if (oChuTransfDet_VM != null)
                {
                    if (oChuTransfDet_VM.oChurchTransfer != null) //if (oChuTransfDet_VM.oChurchTransfer.RequireApproval)
                        if (oChuTransfDet_VM.oChurchTransfer.CurrentScope == "I")
                            oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.IApprovalAction);
                        else if (oChuTransfDet_VM.oChurchTransfer.CurrentScope == "E")
                            oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.EApprovalAction);

                    oChuTransfDet_VM.strTransferDxn = oChuTransfDet_VM.numTransferDxn == 1 ? "Incoming" : oChuTransfDet_VM.numTransferDxn == 2 ? "Outgoing" : "";
                    oChuTransfDet_VM.oChurchBodyId_Logged = this._oLoggedUser.ChurchMemberId;
                }

                return oChuTransfDet_VM;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
   
        private ChurchTransferModel GetClergyTransferDetail_Incoming(ChurchBody oCurrChurchBody, int id = 0)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                var oCTModel = (
                    from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)  // .Include(t => t.ChurchMember)
                         .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorMember).Include(t => t.IApprovalAction)//.Include(t => t.EApprovalAction)
                        .Where(x => x.Id == id && (x.FromChurchBodyId == oCurrChurchBody.Id || x.ToChurchBodyId == oCurrChurchBody.Id))    // && x.RequestorMemberId == oCurrLoggedUser_MemberId)
                    from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                    from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                    from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                    from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                    from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                    from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                       _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                 _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                    from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcs.ChurchBodyId && x.Id==t_mcs.SectorId && x.Generational == true).DefaultIfEmpty()
                    from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                    from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                    from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()


                    select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                    {
                        oChurchBody = oCurrChurchBody,  // t_ct.RequestorChurchBody,
                        oRequestorChurchBody = t_ct.RequestorChurchBody,
                        oChurchTransfer = t_ct,
                        oAppGloOwn  = oCurrChurchBody.AppGlobalOwner,

                        strReason = t_ct.TransferReason,   //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                        strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                        strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                        strTransferDxn = "Outgoing",
                        //  strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                        strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                        strTransMessage = t_ct.CustomTransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                        strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                        numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                        strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                        strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                        strTransfMemberDesc = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() +
                                        ". Request from " + t_cb_f.Name,
                        strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                        strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                        strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                        numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                        strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                        strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                        //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                        strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                        strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                        strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                        strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                        strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                        strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                        strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                        numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,

                        strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                        // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                         // strMovementStatus = GetRequestStatusDesc(t_ct.Status),  // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                        strAffliliateChurchBodies = GetChurchAffiliates(t_ct.AttachedToChurchBodyList),
                        //oAffliliateChurchBodies = GetChurchAffiliates_CB(t_ct.AttachedToChurchBodyList, oCurrChurchBody.AppGlobalOwnerId)
                        strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                        strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),  // t_ct.ReqStatus.Equals("D") ? "Draft" : t_ct.ReqStatus.Equals("P") ? "Pending" : t_ct.ReqStatus.Equals("R") ? "Received" : t_ct.ReqStatus.Equals("X") ? "Terminated" : "N/A"

                        oMemberId_Logged = this._oLoggedUser.ChurchMemberId
                    })
                        .FirstOrDefault();

                if (oCTModel != null)
                {
                    if (oCTModel.oChurchTransfer != null)
                    {
                        oCTModel.strApprovers = _GetApprovers(oCTModel.oChurchTransfer.IApprovalAction);
                        oCTModel.oAffliliateChurchBodies = GetChurchAffiliates_CB(oCTModel.oChurchTransfer.AttachedToChurchBodyList, oCTModel.oAppGloOwn?.Id);
                    }

                    oCTModel.strTransferDxn = oCTModel.numTransferDxn == 1 ? "Incoming" : oCTModel.numTransferDxn == 2 ? "Outgoing" : "";
                }

                return oCTModel;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private ChurchTransferModel GetMemberTransferApprovalRequestDetail(ChurchBody oCurrChurchBody, int id = 0, int? oCurrLoggedUser_MemberId = null)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                var oChuTransfDet_VM = (
                            from t_aas in _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApproverMemberChurchRole)   //.Include(t => t.ApprovalAction)
                                  .Where(x => x.Id == id && x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId) // && x.Approver.ChurchMemberId == oCurrLoggedUser_MemberId)
                            from t_aa in _context.ApprovalAction.AsNoTracking().Where(x => x.Id == t_aas.ApprovalActionId && x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId)
                                // from t_mcr_a in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_aas.MemberChurchRoleId).DefaultIfEmpty()
                            from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
                                .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorMember)
                                  .Where(x => x.Id == t_aa.CallerRefId && (x.RequestorChurchBodyId == oCurrChurchBody.Id || x.ToChurchBodyId == oCurrChurchBody.Id))
                                //&& x.CurrentScope == "I"    && x.CurrentScope == "E"
                            from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                            from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                            from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                            from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                            from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                            from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                    _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                            from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                         _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                            from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcs.ChurchBodyId && x.Id==t_mcs.SectorId && x.Generational == true).DefaultIfEmpty()
                            from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                            from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                        _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()


                            select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                            {
                                oCurrApprovalActionStep = t_aas,
                                oCurrApprovalActionStepId = t_aas.Id,
                                oCurrApprovalAction = t_aa, //t_aas.ApprovalAction,
                                oCurrApprovalActionId = t_aa.Id, // t_aas.ApprovalActionId,
                                strActionStepStatus = GetRequestProcessStatusDesc(t_aas.ActionStepStatus),
                                strActionStatus = GetRequestProcessStatusDesc(t_aas.ApprovalAction.ActionStatus),
                                //

                                oChurchBody = oCurrChurchBody,  // t_cb_r, // t_ct.RequestorChurchBody,
                                oRequestorChurchBody = t_cb_r, // t_ct.RequestorChurchBody,
                                oChurchTransfer = t_ct,
                                strReason = t_ct.TransferReason, //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                                strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                                strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                                //strTransferDxn = "Outgoing",
                                numTransferDxn = (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? 2 : t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? 1 : 0,
                                // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                                strTransMessage = t_ct.CustomTransMessage, //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                                strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                                numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                                strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                                strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                                strTransfMemberDesc = (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim() +
                                                        ((t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? ". Request to " + t_cb_t.Name :
                                                                t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? ". Request from " + t_cb_f.Name : ""),
                                strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                                strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                                strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                                numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                                strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                                strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                                strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                                strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                                strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                                strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                                strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                                numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,

                                strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                                // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                                //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                //strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),
                               // strMovementStatus = GetRequestStatusDesc(t_ct.Status)   // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                                
                                strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                                strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true),
                                //  strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                            })
                             .FirstOrDefault();

                if (oChuTransfDet_VM != null)
                {
                    if (oChuTransfDet_VM.oChurchTransfer != null) //if (oChuTransfDet_VM.oChurchTransfer.RequireApproval)
                        if (oChuTransfDet_VM.oChurchTransfer.CurrentScope == "I")
                            oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.IApprovalAction);
                        else if (oChuTransfDet_VM.oChurchTransfer.CurrentScope == "E")
                            oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.EApprovalAction);

                    oChuTransfDet_VM.strTransferDxn = oChuTransfDet_VM.numTransferDxn == 1 ? "Incoming" : oChuTransfDet_VM.numTransferDxn == 2 ? "Outgoing" : "";
                }

                return oChuTransfDet_VM;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
      
        private ChurchTransferModel GetClergyTransferApprovalRequestDetail(ChurchBody oCurrChurchBody, int id = 0, int? oCurrLoggedUser_MemberId = null)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                var oChuTransfDet_VM = (
                            from t_aas in _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApproverMemberChurchRole)   //.Include(t => t.ApprovalAction)
                                  .Where(x => x.Id == id && x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId) // && x.Approver.ChurchMemberId == oCurrLoggedUser_MemberId)
                            from t_aa in _context.ApprovalAction.AsNoTracking().Where(x => x.Id == t_aas.ApprovalActionId && x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId)
                                // from t_mcr_a in _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_aas.MemberChurchRoleId).DefaultIfEmpty()
                            from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
                                .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorMember)
                                  .Where(x => x.Id == t_aa.CallerRefId && (x.RequestorChurchBodyId == oCurrChurchBody.Id || x.ToChurchBodyId == oCurrChurchBody.Id))
                                //&& x.CurrentScope == "I"    && x.CurrentScope == "E"
                            from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
                            from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
                            from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
                            from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
                            from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)

                            from t_mp in _context.MemberRank.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                _context.MemberRank.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrentRank == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                            from t_ms in _context.MemberStatus.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId) > 0 ?
                                         _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_cm.Id == x.ChurchMemberId).Take(1).ToList() : null
                            from t_mcu in _context.MemberChurchUnit.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId) > 0 ?              /// .DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
                                  _context.MemberChurchUnit.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrUnit == true && t_cm.Id == x.ChurchMemberId).Take(1) : null                                                                                                                                                                              // from t_cs in _context.ChurchUnit.Where(x=> x.ChurchBodyId == t_mcs.ChurchBodyId && x.Id==t_mcs.SectorId && x.Generational == true).DefaultIfEmpty()
                            from t_mcr in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId) > 0 ?         ///.DefaultIfEmpty() //.Include(t => t.ChurchRole)  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                                  _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsLeadRole == true && t_cm.Id == x.ChurchMemberId).Take(1) : null
                            from t_mcr_r in _context.MemberChurchRole.AsNoTracking().Count(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId) > 0 ?
                                        _context.MemberChurchRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsLeadRole == true && t_cm_r.Id == x.ChurchMemberId).Take(1) : null   //.DefaultIfEmpty()//.Include(t => t.ChurchRole) //  && x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_cr_tr in _context.ChurchRole.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToChurchRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_cs_trs in _context.ChurchUnit.AsNoTracking().Where(x => x.OwnedByChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleUnitId == x.Id).DefaultIfEmpty()


                            select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                            {

                                oCurrApprovalActionStep = t_aas,
                                oCurrApprovalActionStepId = t_aas.Id,
                                oCurrApprovalAction = t_aa, //t_aas.ApprovalAction,
                                oCurrApprovalActionId = t_aa.Id, // t_aas.ApprovalActionId,
                                strActionStepStatus = GetRequestProcessStatusDesc(t_aas.ActionStepStatus),
                                strActionStatus = GetRequestProcessStatusDesc(t_aas.ApprovalAction.ActionStatus),
                                //
                                oAppGloOwn = oCurrChurchBody.AppGlobalOwner,
                                oChurchBody = oCurrChurchBody,  // t_cb_r, // t_ct.RequestorChurchBody,
                                oRequestorChurchBody = t_cb_r, // t_ct.RequestorChurchBody,
                                oChurchTransfer = t_ct,
                                strReason = t_ct.TransferReason, //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                                strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                                strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                                //strTransferDxn = "Outgoing",
                                numTransferDxn = (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? 2 : t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? 1 : 0,
                                strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),
                                strTransMessage = t_ct.CustomTransMessage,  //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",
                                strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                                numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                                strRequestorFullName = t_mcr_r != null ? t_mcr_r.ChurchMember != null ? ((((!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? t_mcr_r.ChurchMember.Title : "") + ' ' + t_mcr_r.ChurchMember.FirstName).Trim() + " " + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() : "" : "",
                                strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole != null ? t_mcr_r.ChurchRole.Name : "" : "",
                                strTransfMemberDesc = (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim() +
                                                        ((t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "I") || (t_ct.RequestorChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" && t_ct.ApprovalStatus == "P") ? ". Request to " + t_cb_t.Name :
                                                                t_ct.ToChurchBodyId == oCurrChurchBody.Id && t_ct.CurrentScope == "E" ? ". Request from " + t_cb_f.Name : ""),
                                strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                                strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",
                                strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                                numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                                strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                                strFromMemberFullName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                                strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                                strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                                strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",
                                strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                                strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                                numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,

                                strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                                // strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                            
                            //    strMovementStatus = GetRequestStatusDesc(t_ct.Status),  // t_ct.Status == "I" ? "Pending Approval" : t_ct.Status == "T" ? "In-transit" : t_ct.Status == "U" ? "Incomplete" : t_ct.Status == "Y" ? "Transferred" : "N/A"
                                strAffliliateChurchBodies = GetChurchAffiliates(t_ct.AttachedToChurchBodyList),
                                // oAffliliateChurchBodies = GetChurchAffiliates_CB(t_ct.AttachedToChurchBodyList, oCurrChurchBody.AppGlobalOwnerId)

                                strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus, false),
                                strReqStatusDetail = GetRequestStatusDesc(t_ct.ReqStatus, true),
                            })
                             .FirstOrDefault();

                if (oChuTransfDet_VM != null)
                {
                    if (oChuTransfDet_VM.oChurchTransfer != null)
                    {
                        oChuTransfDet_VM.strApprovers = _GetApprovers(oChuTransfDet_VM.oChurchTransfer.IApprovalAction);
                        oChuTransfDet_VM.oAffliliateChurchBodies = GetChurchAffiliates_CB(oChuTransfDet_VM.oChurchTransfer.AttachedToChurchBodyList, oChuTransfDet_VM.oAppGloOwn?.Id);
                    }

                    oChuTransfDet_VM.strTransferDxn = oChuTransfDet_VM.numTransferDxn == 1 ? "Incoming" : oChuTransfDet_VM.numTransferDxn == 2 ? "Outgoing" : "";
                }

                return oChuTransfDet_VM;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private ChurchTransferModel GetOutgoingTransferDetail(int? oCurrChuBodyId, int id = 0)
        {
            try
            {
                var oCTModelList =  _context.ChurchTransfer.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.ChurchMember).Include(t => t.ChurchMember)
                            .Where(x => x.Id == id && x.RequestorChurchBodyId == oCurrChuBodyId).ToList();

                if (oCTModelList.Count == 0) return null;
                if (oCTModelList.Count > 1) oCTModelList = oCTModelList.Take(1).ToList();

                var oAGOid = this._oLoggedAGO.Id; var oCBid = oCurrChuBodyId; var oCMid = oCTModelList[0].ChurchMemberId;
                ///
                //var t_mtList = _context.MemberType.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsCurrent == true).ToList();
                //if (t_mtList.Count > 1) t_mtList = t_mtList.Take(1).ToList();

                var t_mrList = _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsCurrentRank == true).ToList();
                if (t_mrList.Count > 1) t_mrList = t_mrList.Take(1).ToList();
                var t_msList = _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsCurrent == true).ToList();
                if (t_msList.Count > 1) t_msList = t_msList.Take(1).ToList();
                var t_mcuList = _context.MemberChurchUnit.Include(t => t.ChurchUnit).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.ChurchUnit.IsUnitGen == true && x.IsCurrUnit == true).ToList();
                if (t_mcuList.Count > 1) t_mcuList = t_mcuList.Take(1).ToList();
                var t_mcrList = _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsLeadRole == true).ToList();
                if (t_msList.Count > 1) t_mcrList = t_mcrList.Take(1).ToList();
                var t_mcr_rList = _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCTModelList[0].RequestorMemberId && x.IsLeadRole == true).ToList();
                if (t_mcr_rList.Count > 1) t_mcr_rList = t_mcr_rList.Take(1).ToList();


                var oCTModel = (
                             from t_ct in oCTModelList   // _context.ChurchTransfer.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel)
                                                         //.Include(t => t.ChurchMember).Include(t => t.ChurchMember)
                                                         //.Where(x => x.Id == id && x.RequestorChurchBodyId == oCBid)   

                             join t_cmx in _context.ChurchMember.Where(x => x.ChurchBodyId == oCBid) on t_ct.ChurchMemberId equals t_cmx.Id into abc
                            from t_cm in abc  
                            join t_cmx_r in _context.ChurchMember.Where(x => x.ChurchBodyId == oCBid) on t_ct.RequestorMemberId equals t_cmx_r.Id into abc_r
                            from t_cm_r in abc_r  

                            join t_mpx in t_mrList on 1 equals 1 into abcd // _context.MemberRank.Where(x => x.ChurchBodyId == oCBid && x.IsCurrentRank == true) on t_cm.Id equals t_mpx.ChurchMemberId into abcd
                             from t_mp in abcd.DefaultIfEmpty() 

                            join t_msx in t_msList on 1 equals 1 into abcde // _context.MemberStatus.Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == oCBid && x.IsCurrent == true) on t_cm.Id equals t_msx.ChurchMemberId into abcde
                             from t_ms in abcde.DefaultIfEmpty()  

                            join t_mcux in _context.MemberChurchUnit.Include(t => t.ChurchUnit).Where(x => x.ChurchBodyId == oCBid && x.ChurchUnit.IsUnitGen == true && x.IsCurrUnit == true).Take(1) on t_cm.Id equals t_mcux.ChurchMemberId into abcdef // age bracket/group: children, youth, adults as defined in config. OR auto-assign
                            from t_mcu in abcdef.DefaultIfEmpty() 

                            join t_mcrx in _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.ChurchBodyId == oCBid && x.IsLeadRole == true).Take(1) on t_cm.Id equals t_mcrx.ChurchMemberId into abcdefg   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_mcr in abcdefg.DefaultIfEmpty()

                            join t_mcrx_r in _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.ChurchBodyId == oCBid && x.IsLeadRole == true).Take(1) on t_cm_r.Id equals t_mcrx_r.ChurchMemberId into abcdefg_r   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_mcr_r in abcdefg_r.DefaultIfEmpty()

                             join t_crx_tr in _context.ChurchRole.Where(x => x.OwnedByChurchBodyId == oCBid) on t_ct.ToChurchRoleId equals t_crx_tr.Id into abcdefg_tr   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_cr_tr in abcdefg_tr.DefaultIfEmpty()

                             join t_csx_trs in _context.ChurchUnit.Where(x => x.OwnedByChurchBodyId == oCBid) on t_ct.ToRoleUnitId equals t_csx_trs.Id into abcdefg_trs   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_cs_trs in abcdefg_trs.DefaultIfEmpty()

                             join t_aax in _context.ApprovalAction.Where(x => x.ChurchBodyId == oCBid) on t_ct.IApprovalActionId equals t_aax.Id into abcdefgh   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                            from t_aa in abcdefgh.DefaultIfEmpty()

                                //join t_mcx in _context.MemberContact.Include(t => t.InternalContact).ThenInclude(t => t.ContactInfo).Include(t => t.ExternalContact).ThenInclude(t => t.ContactInfo).Where(x => x.CurrentContact == true).Take(1) on t_cm.Id equals t_mcx.ChurchMemberId into abcdefghi
                                //from t_mc in abcdefghi.DefaultIfEmpty()
                                //join t_mlsx in _context.MemberLanguageSpoken.Where(x => x.PrimaryLanguage == true).Take(1) on t_cm.Id equals t_mlsx.ChurchMemberId into abcdefghij
                                //from t_mls in abcdefghij.DefaultIfEmpty()
                                //join t_mehx in _context.MemberEducHistory.Where(x => x.InstitutionType.EduLevelIndex == _context.MemberEducHistory.Where(y => y.ChurchMemberId == id).Min(y => y.InstitutionType.EduLevelIndex)).Take(1) on t_cm.Id equals t_mehx.ChurchMemberId into abcdefghijk
                                //from t_meh in abcdefghijk.DefaultIfEmpty()

                            select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                            {
                                ////cong of member's membership... not his work or role     // //core dept or sector == group, cttee or the local church (e.g. Min) ...                                   
                                //oAppGlobalOwner = t_ct.FromChurchBody.AppGlobalOwner, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.AppGlobalOwner : null,
                                //numAppGlobalOwnerId = t_ct.FromChurchBody.AppGlobalOwnerId,
                                oChurchBody = t_ct.RequestorChurchBody,
                                 oChurchTransfer = t_ct,

                                 blRequireApproval = t_ct.RequireApproval,
                                 IsCurrentActionStepToApprove = false,
                                 oCurrApprovalActionStep = null,
                                oCurrApprovalActionStepId = null,

                                // numFromChurchPositionId = null,
                                //  numFromMemberChurchRoleId = null,
                                //   numRequestorChurchBodyId = null,
                                //    numRequestorMemberChurchRoleId = null,
                                //numToChurchLevelId = null,
                                //oChurchTransferModelList = null,
                                // oCoreChurchlifeVM = null,

                                strReqStatusComments = t_ct.ReqStatusComments,
                                 //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                 strApprovalStatusComments = t_ct.ApprovalStatusComments,
                                 strComments = t_ct.Comments,
                                 strReason = t_ct.TransferReason,  //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                                 strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                                 strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                                 strTransferDxn = "Outgoing",
                                // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),

                                 strTransMessage = t_ct.CustomTransMessage,  //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",

                                //strMemberDisplayName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                                 numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                                 strRequestorFullName = t_mcr_r == null ? "" : (((t_mcr_r.ChurchMember.FirstName + ' ' + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? "(" + t_mcr_r.ChurchMember.Title + ")" : "")).Trim(),
                                 strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole.Name : "",

                                 strTransfMemberDesc = (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim() + ", " + (t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "").Trim(),
                                 strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                                 strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",

                                 strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                                 numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                                 strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                                 strFromMemberFullName = (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                 strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                                 strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                 strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                                 strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                                 strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",

                                 strFromMemberLongevityDesc = "", // GetMemberChuLongevity(),  //check when member joined the church
                                strFromCongLongevityDesc = "", // GetMemberCongLongevity(),   //check when member joined the congregation
                                strFromRoleLongevityDesc = "", // GetMemberRoleLongevity(),   //check when member commenced the role
                                strFromClergyLongevityDesc = "", // GetMemberClergyLongevity(),   //check when member commenced ministry [Ordained]  ie. Ordination is an event under Church Events

                                strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                                 strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                                 numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,
                                // strToMemberPos = t_mp_t != null ? t_mp_t.ChurchPosition.PositionName : "N/A",
                                strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                                // strApprovalActionStatus = t_ct.RequireApproval == false ? "N/A" : GetActionStatus(t_ct.ApprovalStatus), // GetActionStatus(_context.ApprovalAction.Where(x => x.Id == t_ct.IApprovalActionId).FirstOrDefault().ActionStatus),
                                strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                                //  strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus)  //.Equals("D") ? "Draft" : t_ct.ReqStatus.Equals("P") ? "Pending" : t_ct.ReqStatus.Equals("R") ? "Received" : t_ct.ReqStatus.Equals("X") ? "Terminated" : "N/A"

                                strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus),
                              //  strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                            }).FirstOrDefault();


                return oCTModel;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private ChurchTransferModel GetIncomingTransferDetail(ChurchBody oCurrChurchBody, int id = 0)
        {
            try
            {
                oCurrChurchBody = oCurrChurchBody == null ? this._oLoggedCB : oCurrChurchBody; 

                //var oChuTransfDet_VM = (
                //            from t_ct in _context.ChurchTransfer.Include(t => t.FromChurchBody).Include(t => t.ToChurchBody)
                //            .Include(t => t.RequestorMember).Include(t => t.ToChurchBody).Include(t => t.FromChurchBody).Include(t => t.ChurchMember)
                //            .Where(x => x.Id == id && x.ToChurchBodyId == oCurrChurchBody.Id)  // && x.Status != "X")   //ie Closed


                var oCTModelList = _context.ChurchTransfer.Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).Include(t => t.RequestorMember).Include(t => t.ToChurchBody).Include(t => t.FromChurchBody).Include(t => t.ChurchMember)
                .Where(x => x.Id == id && x.ToChurchBodyId == oCurrChurchBody.Id).ToList();

                if (oCTModelList.Count == 0) return null;
                if (oCTModelList.Count > 1) oCTModelList = oCTModelList.Take(1).ToList();

                var oAGOid = this._oLoggedAGO.Id; var oCBid = oCurrChurchBody.Id; var oCMid = oCTModelList[0].ChurchMemberId;
                ///
                //var t_mtList = _context.MemberType.AsNoTracking().Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsCurrent == true).ToList();
                //if (t_mtList.Count > 1) t_mtList = t_mtList.Take(1).ToList();

                var t_mrList = _context.MemberRank.AsNoTracking().Include(t => t.ChurchRank_NVP).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsCurrentRank == true).ToList();
                if (t_mrList.Count > 1) t_mrList = t_mrList.Take(1).ToList();
                var t_msList = _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus_NVP).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsCurrent == true).ToList();
                if (t_msList.Count > 1) t_msList = t_msList.Take(1).ToList();
                var t_mcuList = _context.MemberChurchUnit.Include(t => t.ChurchUnit).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.ChurchUnit.IsUnitGen == true && x.IsCurrUnit == true).ToList();
                if (t_mcuList.Count > 1) t_mcuList = t_mcuList.Take(1).ToList();
                var t_mcrList = _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCMid && x.IsLeadRole == true).ToList();
                if (t_msList.Count > 1) t_mcrList = t_mcrList.Take(1).ToList();
                var t_mcr_rList = _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.AppGlobalOwnerId == oAGOid && x.ChurchBodyId == oCBid && x.ChurchMemberId == oCTModelList[0].RequestorMemberId && x.IsLeadRole == true).ToList();
                if (t_mcr_rList.Count > 1) t_mcr_rList = t_mcr_rList.Take(1).ToList();


                var oCTModel = (
                             from t_ct in oCTModelList   // _context.ChurchTransfer.Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel)
                                                         //.Include(t => t.ChurchMember).Include(t => t.ChurchMember)
                                                         //.Where(x => x.Id == id && x.RequestorChurchBodyId == oCurrChuBodyId)   

                             join t_cmx in _context.ChurchMember.Where(x => x.ChurchBodyId == oCBid) on t_ct.ChurchMemberId equals t_cmx.Id into abc
                             from t_cm in abc
                             join t_cmx_r in _context.ChurchMember.Where(x => x.ChurchBodyId == oCBid) on t_ct.RequestorMemberId equals t_cmx_r.Id into abc_r
                             from t_cm_r in abc_r

                             join t_mpx in t_mrList on 1 equals 1 into abcd // _context.MemberRank.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.IsCurrentRank == true) on t_cm.Id equals t_mpx.ChurchMemberId into abcd
                             from t_mp in abcd.DefaultIfEmpty()

                             join t_msx in t_msList on 1 equals 1 into abcde // _context.MemberStatus.Include(t => t.ChurchMemStatus_NVP).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.IsCurrent == true) on t_cm.Id equals t_msx.ChurchMemberId into abcde
                             from t_ms in abcde.DefaultIfEmpty()

                             join t_mcux in _context.MemberChurchUnit.Include(t => t.ChurchUnit).Where(x => x.ChurchBodyId == oCBid && x.ChurchUnit.IsUnitGen == true && x.IsCurrUnit == true).Take(1) on t_cm.Id equals t_mcux.ChurchMemberId into abcdef // age bracket/group: children, youth, adults as defined in config. OR auto-assign
                             from t_mcu in abcdef.DefaultIfEmpty()

                             join t_mcrx in _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.ChurchBodyId == oCBid && x.IsLeadRole == true).Take(1) on t_cm.Id equals t_mcrx.ChurchMemberId into abcdefg   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             from t_mcr in abcdefg.DefaultIfEmpty()

                             join t_mcrx_r in _context.MemberChurchRole.Include(t => t.ChurchRole).Where(x => x.ChurchBodyId == oCBid && x.IsLeadRole == true).Take(1) on t_cm_r.Id equals t_mcrx_r.ChurchMemberId into abcdefg_r   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             from t_mcr_r in abcdefg_r.DefaultIfEmpty()

                             join t_crx_tr in _context.ChurchRole.Where(x => x.OwnedByChurchBodyId == oCBid) on t_ct.ToChurchRoleId equals t_crx_tr.Id into abcdefg_tr   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             from t_cr_tr in abcdefg_tr.DefaultIfEmpty()

                             join t_csx_trs in _context.ChurchUnit.Where(x => x.OwnedByChurchBodyId == oCBid) on t_ct.ToRoleUnitId equals t_csx_trs.Id into abcdefg_trs   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             from t_cs_trs in abcdefg_trs.DefaultIfEmpty()

                             join t_aax in _context.ApprovalAction.Where(x => x.ChurchBodyId == oCBid) on t_ct.IApprovalActionId equals t_aax.Id into abcdefgh   //&& x.CurrServing && x.ChurchRole.AuthorityIndex == _context.MemberChurchRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.ChurchRole.AuthorityIndex)
                             from t_aa in abcdefgh.DefaultIfEmpty()

                                 //join t_mcx in _context.MemberContact.Include(t => t.InternalContact).ThenInclude(t => t.ContactInfo).Include(t => t.ExternalContact).ThenInclude(t => t.ContactInfo).Where(x => x.CurrentContact == true).Take(1) on t_cm.Id equals t_mcx.ChurchMemberId into abcdefghi
                                 //from t_mc in abcdefghi.DefaultIfEmpty()
                                 //join t_mlsx in _context.MemberLanguageSpoken.Where(x => x.PrimaryLanguage == true).Take(1) on t_cm.Id equals t_mlsx.ChurchMemberId into abcdefghij
                                 //from t_mls in abcdefghij.DefaultIfEmpty()
                                 //join t_mehx in _context.MemberEducHistory.Where(x => x.InstitutionType.EduLevelIndex == _context.MemberEducHistory.Where(y => y.ChurchMemberId == id).Min(y => y.InstitutionType.EduLevelIndex)).Take(1) on t_cm.Id equals t_mehx.ChurchMemberId into abcdefghijk
                                 //from t_meh in abcdefghijk.DefaultIfEmpty()

                             select new ChurchTransferModel()   //don't waste time loading anything except editing mode...
                             {
                                 ////cong of member's membership... not his work or role     // //core dept or sector == group, cttee or the local church (e.g. Min) ...                                   
                                 //oAppGlobalOwner = t_ct.FromChurchBody.AppGlobalOwner, // t_ct.FromChurchBody != null ? t_ct.FromChurchBody.AppGlobalOwner : null,
                                 //numAppGlobalOwnerId = t_ct.FromChurchBody.AppGlobalOwnerId,
                                 oChurchBody = t_ct.RequestorChurchBody,
                                 oChurchTransfer = t_ct,

                                 blRequireApproval = t_ct.RequireApproval,
                                 IsCurrentActionStepToApprove = false,
                                 oCurrApprovalActionStep = null,
                                 oCurrApprovalActionStepId = null,

                                 // numFromChurchPositionId = null,
                                 //  numFromMemberChurchRoleId = null,
                                 //   numRequestorChurchBodyId = null,
                                 //    numRequestorMemberChurchRoleId = null,
                                 //numToChurchLevelId = null,
                                 //oChurchTransferModelList = null,
                                 // oCoreChurchlifeVM = null,

                                 strReqStatusComments = t_ct.ReqStatusComments,
                                 //strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                 strApprovalStatusComments = t_ct.ApprovalStatusComments,
                                 strComments = t_ct.Comments,
                                 strReason = t_ct.TransferReason,  //.oNVP_Reason != null ? t_ct.oNVP_Reason.NVPValue : "",
                                 strRequestorChurchLevel = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.ChurchLevel != null ? t_ct.RequestorChurchBody.ChurchLevel.CustomName : "" : "",
                                 strToRoleDept = t_cs_trs != null ? t_cs_trs.Name : "N/A",
                                 strTransferDxn = "Outgoing",
                                 // strTransferType = t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                 strTransferType = GetTransferTypeDesc(t_ct.TransferType), //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                 strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType),

                                 strTransMessage = t_ct.CustomTransMessage,  //.oNVP_TransMessage != null ? t_ct.oNVP_TransMessage.NVPValue : "",

                                 //strMemberDisplayName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
                                 strRequestorChurchBody = t_ct.RequestorChurchBody != null ? t_ct.RequestorChurchBody.Name : "",
                                 numRequestorMemberId = t_mcr_r != null ? t_mcr_r.ChurchMemberId : null,
                                 strRequestorFullName = t_mcr_r == null ? "" : (((t_mcr_r.ChurchMember.FirstName + ' ' + t_mcr_r.ChurchMember.MiddleName).Trim() + " " + t_mcr_r.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_mcr_r.ChurchMember.Title) ? "(" + t_mcr_r.ChurchMember.Title + ")" : "")).Trim(),
                                 strRequestorRole = t_mcr_r != null ? t_mcr_r.ChurchRole.Name : "",

                                 strTransfMemberDesc = (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim() + ", " + (t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "").Trim(),
                                 strRequestDateDesc = t_ct.RequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.RequestDate) : "N/A",
                                 strTransferDateDesc = t_ct.TransferDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_ct.TransferDate) : "N/A",

                                 strFromChurchLevel = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.ChurchLevel != null ? t_ct.FromChurchBody.ChurchLevel.CustomName : "" : "",
                                 numFromChurchBodyId = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Id : -1,
                                 strFromChurchBody = t_ct.FromChurchBody != null ? t_ct.FromChurchBody.Name : "",
                                 strFromMemberFullName = (((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim(),
                                 strFromMemberPos = t_mp != null ? t_mp.ChurchRank_NVP.NVPValue : "",
                                 strFromMemberStatus = t_ms != null ? t_ms.ChurchMemStatus_NVP.NVPValue : "",
                                 strFromMemberAgeGroup = t_mcu.ChurchUnit != null ? t_mcu.ChurchUnit.Name : "",
                                 strFromMemberRole = t_mcr != null ? t_mcr.ChurchRole.Name : "",
                                 strFromMemberPhotoUrl = t_cm != null ? t_cm.PhotoUrl : "df_user_p.png",

                                 strFromMemberLongevityDesc = "", // GetMemberChuLongevity(),  //check when member joined the church
                                 strFromCongLongevityDesc = "", // GetMemberCongLongevity(),   //check when member joined the congregation
                                 strFromRoleLongevityDesc = "", // GetMemberRoleLongevity(),   //check when member commenced the role
                                 strFromClergyLongevityDesc = "", // GetMemberClergyLongevity(),   //check when member commenced ministry [Ordained]  ie. Ordination is an event under Church Events

                                 strToChurchLevel = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.ChurchLevel != null ? t_ct.ToChurchBody.ChurchLevel.CustomName : "" : "",
                                 strToChurchBody = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Name : "",
                                 numToChurchBodyId = t_ct.ToChurchBody != null ? t_ct.ToChurchBody.Id : -1,
                                 // strToMemberPos = t_mp_t != null ? t_mp_t.ChurchPosition.PositionName : "N/A",
                                 strToMemberRole = t_cr_tr != null ? t_cr_tr.Name : "N/A",
                                 // strApprovalActionStatus = t_ct.RequireApproval == false ? "N/A" : GetActionStatus(t_ct.ApprovalStatus), // GetActionStatus(_context.ApprovalAction.Where(x => x.Id == t_ct.IApprovalActionId).FirstOrDefault().ActionStatus),
                                 strApprovers = t_ct.RequireApproval == false ? "N/A" : GetApprovers(t_ct.IApprovalAction),
                                 // strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus)  //.Equals("D") ? "Draft" : t_ct.ReqStatus.Equals("P") ? "Pending" : t_ct.ReqStatus.Equals("R") ? "Received" : t_ct.ReqStatus.Equals("X") ? "Terminated" : "N/A"

                                 strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
                                 strReqStatus = GetRequestStatusDesc(t_ct.ReqStatus),
                                 //strAckStatus = GetAckStatusDesc(t_ct.ReqStatus),

                             })
                             .FirstOrDefault();

                return oCTModel;
            }

            catch (Exception ex)
            {
                return null; // ChurchTransferModel();
            } 
        }


        private bool PerformMemberTransfer(ChurchModelContext currCtx, ChurchTransfer oChuTransf, int? oCurrChuBodyId)   // ChurchTransferModel oCTModel, 
        {
            try
            {
                //the request cong confirms that To cong should go ahead and transfer the member
                if (oCurrChuBodyId != oChuTransf.RequestorChurchBodyId && oChuTransf.CurrentScope != "E" && oChuTransf.ReqStatus != "A" && oChuTransf.ApprovalStatus != "A") return false;

                //make member past member of from cong... add new member @ To cong
               // var oCurrChuMember = currCtx.ChurchMember.Find(oChuTransf.ChurchMemberId);

                var oCurrChuMember = currCtx.ChurchMember.AsNoTracking()
                    .Where(c=> c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId== oChuTransf.FromChurchBodyId && c.Id== oChuTransf.ChurchMemberId).FirstOrDefault();

                var tm = DateTime.Now;

                ///
                if (oCurrChuMember != null)
                {
                    var _strCTdesc = GetTransferTypeDesc(oChuTransf.TransferType) + " - " + GetTransferSubTypeDesc(oChuTransf.TransferType);
                    var strCTdesc = oChuTransf.TransferType + " - " + oChuTransf.TransferType;

                    // oChuTransf.TransferDate = DateTime.Now;   ... use what date came

                    //
                    //oNewChuMember.ActiveStatus = true;   //deleted                   
                    //oCurrChuMember.IsCurrent = false;
                    //oCurrChuMember.Departed = oChuTransf.TransferDate;
                    //oCurrChuMember.DepartReason = "Transferred";
                    ////oCurrChuMember.Enrolled = null;  keep as it is
                    ////oNewChuMember.EnrollReason = "Transferred";   keep as it is
                    //oCurrChuMember.IsDeceased = false;  

                    oCurrChuMember.ChurchBodyId = oChuTransf.ToChurchBodyId;  // To CB now takes over the CM ... but [OwnedByChurchBodyId] keeps the FromCB as the originating CB of the member [where Cm begun life in the church oChuTransf ...VIPPP !]
                    oCurrChuMember.Status = "T";  // account closed
                    oCurrChuMember.StatusReason = "Member transferred but yet to report [in-person] at new congregation";
                    oCurrChuMember.LastMod = DateTime.Now;
                    oCurrChuMember.LastModByUserId = this._oLoggedUser.Id;

                    // 
                    currCtx.Update(oCurrChuMember);


                    //// create new member... unique member id = churchBodyId + member id
                    //var oNewChuMember = new ChurchMember();
                    //// oNewChuMember = currCtx.ChurchMember.Find(oChuTransf.ChurchMemberId);  //oChuTransf.ChurchMemberTransf;
                    //// oNewChuMember.Id = 0;
                    ////oCurrChuMember.MemberGlobalId = oChuTransf.ChurchMemberTransf.MemberGlobalId;
                    //// oNewChuMember.OwnerChurchBodyId = (int)oChuTransf.ToChurchBodyId;
                    //// oNewChuMember.ChurchBody = oChuTransf.ToChurchBody;

                    //oNewChuMember.ChurchBodyId = (int)oChuTransf.ToChurchBodyId;
                    //oNewChuMember.GlobalMemberCode = oCurrChuMember.GlobalMemberCode;
                    //oNewChuMember.AppGlobalOwnerId = oCurrChuMember.AppGlobalOwnerId;
                    //// oNewChuMember.AppGlobalOwner = oCurrChuMember.AppGlobalOwner;
                    //oNewChuMember.MemberCustomCode = oCurrChuMember.MemberCustomCode;
                    //oNewChuMember.Title = oCurrChuMember.Title;
                    //oNewChuMember.FirstName = oCurrChuMember.FirstName;
                    //oNewChuMember.MiddleName = oCurrChuMember.MiddleName;
                    //oNewChuMember.LastName = oCurrChuMember.LastName;
                    //oNewChuMember.MaidenName = oCurrChuMember.MaidenName;
                    //oNewChuMember.Gender = oCurrChuMember.Gender;
                    //oNewChuMember.DateOfBirth = oCurrChuMember.DateOfBirth;
                    //oNewChuMember.MaritalStatus = oCurrChuMember.MaritalStatus;
                    //oNewChuMember.MarriageType = oCurrChuMember.MarriageType;
                    //oNewChuMember.MarriageRegNo = oCurrChuMember.MarriageRegNo;
                    //oNewChuMember.NationalityId = oCurrChuMember.NationalityId;
                    //// oNewChuMember.Nationality = oCurrChuMember.MemberCode;
                    //oNewChuMember.IDTypeId = oCurrChuMember.IDTypeId;
                    ////oNewChuMember.IDType = oCurrChuMember.IDType;
                    //oNewChuMember.NationalIDNum = oCurrChuMember.NationalIDNum;
                    //oNewChuMember.ContactInfoId = oCurrChuMember.ContactInfoId;
                    ////  oNewChuMember.ContactInfo = oCurrChuMember.ContactInfo;
                    //oNewChuMember.Hobbies = oCurrChuMember.Hobbies;
                    //oNewChuMember.Hometown = oCurrChuMember.Hometown;
                    //oNewChuMember.HometownRegionId = oCurrChuMember.HometownRegionId;
                    //// oNewChuMember.HometownRegion = oCurrChuMember.HometownRegion;
                    //oNewChuMember.MotherTongueId = oCurrChuMember.MotherTongueId;
                    //// oNewChuMember.MotherTongue = oCurrChuMember.MotherTongue;
                    //oNewChuMember.PhotoUrl = oCurrChuMember.PhotoUrl;
                    //oNewChuMember.OtherInfo = oCurrChuMember.OtherInfo;
                    ////oNewChuMember.EducationLevelId = oCurrChuMember.EducationLevelId;
                    //oNewChuMember.IsActivated = true;   //deletion only...flag until clean up or archive

                    ////
                    ////oNewChuMember.IsCurrent = false;   //requires activation from Target congregation                    
                    ////oNewChuMember.Enrolled = null; //oChuTransf.TransferDate;  //date activated  
                    //// oNewChuMember.EnrollReason = "Transferred";   ... check church-life rather /status
                    ////oNewChuMember.IsDeceased = false;
                    ////oNewChuMember.Departed = null; // oChuTransf.TransferDate;
                    ////oNewChuMember.DepartReason = null; // "Transferred";
                    //oNewChuMember.Created = DateTime.Now;
                    //oNewChuMember.LastMod = DateTime.Now;
                    ////
                    //currCtx.Add(oNewChuMember);


                    //CHECK ON CHURCH-LIFE DETAILS TO SEE IF... ANY MUST BE TRANSFERRED
                    /// MCL
                    var oMemCL = currCtx.MemberChurchlife.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                    c.ChurchMemberId == oChuTransf.ChurchMemberId ).FirstOrDefault();
                    if (oMemCL != null) {

                        var oNewMemCL = new MemberChurchlife()
                        {
                            //  oNewMemCL.ChurchBodyServiceId = null,
                            ChurchBodyId = oChuTransf.ToChurchBodyId,
                            // ChurchMemberId = oNewChuMember.Id,  // oNewChuMember.Id, // oMemPos.ChurchMemberId,                    
                            DateJoined = oChuTransf.TransferDate, // DateTime.Now,                                                
                            EnrollMode = "T", // Transfer check enrollmodes @Member Register   
                            EnrollReason = oChuTransf.TransferReason,
                            IsCurrentMember = true,
                            //IsDeceased = oMemCL.IsDeceased,
                            //DateDeparted = oMemCL.DateDeparted,
                            //DepartReason = oMemCL.DepartReason,
                            ///
                            IsMemBaptized = oMemCL.IsMemBaptized,
                            IsMemConfirmed = oMemCL.IsMemConfirmed,
                            IsMemCommunicant = oMemCL.IsMemCommunicant,
                            NonCommReason = oMemCL.NonCommReason,
                            //oNewMemCL.IsPioneer  = oMemCL.IsPioneer,
                            Created = tm,
                            LastMod = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id
                        };

                        currCtx.Add(oNewMemCL);

                        //update the old to past ...  
                        oMemCL.IsCurrentMember = false; //past             ... WORK  ON THIS CODE LATER
                        oMemCL.DateDeparted = oChuTransf.TransferDate; // oNewMemPos.Until;
                        oMemCL.DepartMode = "T"; // Transfer === as --- enrollmodes @Member Register  
                        oMemCL.DepartReason = oChuTransf.TransferReason;
                        oMemCL.LastMod = tm;
                        oMemCL.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oMemCL);
                    }


                    //update the old to past  ... MT
                    var oMemType = currCtx.MemberType.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId &&
                                                            c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrent == true).FirstOrDefault();
                    if (oMemType != null)
                    {
                        //get new Type... from the old  ...but get the Type on the approval/acknowledge form of To-CB [ receive member as .... regular, distant etc. ]  ... oCTModel
                        var oNewMemType = new MemberType()
                        {
                            ChurchBodyId = oChuTransf.ToChurchBodyId,
                            /// ChurchMemberId = oNewChuMember.Id, // same everywhere...
                            MemberTypeCode =  oChuTransf.TempMemTypeCodeToCB,  /// oMemType.MemberTypeCode, // as-is... to init @form /// ... let the To-CB choose the [available Typees] applicable... but not deceased
                            FromDate = oChuTransf.TransferDate, // DateTime.Now,
                            ToDate = null,
                            IsCurrent = true,
                            Notes = oChuTransf.TransferReason,                            
                            Created = tm, 
                            LastMod = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id
                        };

                        currCtx.Add(oNewMemType);


                        //update the old to past ...  // var oMemType = currCtx.MemberType.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveType == true).FirstOrDefault();
                        // oMemType.MemberTypeCode = oChuTransf.TempMemTypeCodeFrCB; // as-is...   past             ... let the From-CB choose the [unavailable Typees] applicable... but not deceased
                        oMemType.ToDate = oChuTransf.TransferDate;
                        oMemType.IsCurrent = false;
                        oMemType.Notes = oChuTransf.TransferReason;
                        oMemType.LastMod = tm;
                        oMemType.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oMemType);
                    };

                    //update church MR ... put new member on lowest Pos... except clergy transfer
                    var oMemRank = currCtx.MemberRank.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                        c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrentRank == true).FirstOrDefault();
                    if (oMemRank != null)
                    {
                        //get new status... from the old  ...else get the RANK on the approval/acknowledge form [ receive member as .... member, presbyter/elder etc. ]
                        var oNewMemPos = new MemberRank()
                        {
                            ChurchBodyId = oChuTransf.ToChurchBodyId,
                           // ChurchMemberId = oChuTransf.ChurchMemberId, // oMemPos.ChurchMemberId,                    
                            FromDate = oChuTransf.TransferDate, 
                            ToDate = null, 
                            ChurchRankId = oChuTransf.TempMemRankIdToCB, /// oMemRank.ChurchRankId, // as-is... to init @form/// oMemRank.ChurchRankId, // oCTModel.numMemRankId... assign member the last rank... assumption that To CB uses same config as From CB else [edit required]
                            IsCurrentRank = true, 
                            Notes = oChuTransf.TransferReason,
                            Created = tm,
                            LastMod = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id
                        };

                        currCtx.Add(oNewMemPos);

                        ///
                        //update the old to past ...  // var oMemPos = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
                        ///
                        oMemRank.IsCurrentRank = false;
                        // oMemRank.ChurchRankId = as is 
                        // oMemRank.FromDate = as is
                        oMemRank.ToDate = oChuTransf.TransferDate; // oNewMemPos.Until;
                        oMemRank.Notes = oChuTransf.TransferReason;
                        oMemRank.LastMod = tm;
                        oMemRank.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oMemRank);
                         
                        //// OR assign minimum rank
                        //if (oChuTransf.TransferType == "MT")
                        //{
                        //    var oMinPos = _context.ChurchPosition.Where(x => x.GradeLevel ==
                        //                    _context.ChurchPosition.Where(y => y.ChurchBody.AppGlobalOwnerId == oChuTransf.ToChurchBody.AppGlobalOwnerId)
                        //                    .Max(y => y.GradeLevel)).Take(1).FirstOrDefault();
                        //    if (oMinPos == null) return false;
                        //    oNewMemPos.ChurchPositionId = oMinPos.Id;
                        //}
                        //else if (oChuTransf.TransferType == "CT")
                        //{  //Pos unchanged
                        //    oNewMemPos.ChurchPositionId = oMemPos.Id;
                        //} 
                    }
                     
                     
                    //update the old to past
                    var oMemStatus = currCtx.MemberStatus.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                            c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrent == true).FirstOrDefault();
                    if (oMemStatus != null)                     
                    {
                        //get new status... from the old  ...but get the STATUS on the approval/acknowledge form of To-CB [ receive member as .... regular, distant etc. ]  ... oCTModel
                        var oNewMemStatus = new MemberStatus()
                        {
                            ChurchBodyId = oChuTransf.ToChurchBodyId,
                           /// ChurchMemberId = oNewChuMember.Id, // same everywhere...
                            ChurchMemStatusId = oChuTransf.TempMemStatusIdToCB,  /// ... let the To-CB choose the [available statuses] applicable... but not deceased
                            FromDate = oChuTransf.TransferDate, // DateTime.Now,
                            ToDate = null,
                            Notes = oChuTransf.TransferReason,
                            IsCurrent = true,
                            Created = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id,
                        };

                        currCtx.Add(oNewMemStatus);


                        //update the old to past ...  // var oMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
                        oMemStatus.ChurchMemStatusId = oChuTransf.TempMemStatusIdFrCB; //past    ... let the From-CB choose the [unavailable statuses] applicable... but not deceased
                        oMemStatus.ToDate = oChuTransf.TransferDate;
                        /// oMemStatus.IsCurrent = false;  /// as-is  ... thus the last state of the member in the church. [use the ChurchMemStatusId ... indicate availability -- regular, distant or past]
                        oMemStatus.Notes = oChuTransf.TransferReason;
                        oMemStatus.LastMod = tm;
                        oMemStatus.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oMemStatus);


                        // oNewMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault(); // oMemStatus;
                        //  oNewMemStatus.Id = 0;                         
                    };
                                       

                    //// groups and associations... make them all ... PAST 
                    var oMemSectors = currCtx.MemberChurchUnit.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                        c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrUnit == true).ToList();
                    foreach (var oMS in oMemSectors)
                    {
                        oMS.IsCurrUnit = false;
                        oMS.ToDate = oChuTransf.TransferDate;
                        oMS.DepartReason = oChuTransf.TransferReason;
                        oMS.LastMod = tm;
                        oMS.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oMS);
                    }

                    //// roles... make them all ... PAST .. newer roles will be added at to-CB if any  [thus ... member has no role in new CB, but former roles left in-tact as history data]
                    var oMemRoles = currCtx.MemberChurchRole.Include(t => t.ChurchRole).ThenInclude(t => t.ParentChurchRole)
                                                            .Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                                c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrentRole == true).ToList();
                    foreach (var oMR in oMemRoles)  
                    {
                        oMR.IsCurrentRole = false;
                        oMR.ToDate = oChuTransf.TransferDate;
                        oMR.DepartReason = oChuTransf.TransferType;
                        oMR.LastMod = tm;
                        oMR.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oMR);
                    }

                    ////CT move with roles
                    //if (oChuTransf.TransferType == "CT")
                    //{
                    //    var oMinRoleList = oMemRoles.Where(c => c.LeaderRole?.LeaderRoleCategory?.IsCoreRole == true && c.LeaderRole?.LeaderRoleCategory?.IsOrdainedRole == true);
                    //    foreach (var oMinRole in oMinRoleList)
                    //    {
                    //        //get new status... from the old
                    //        var oNewMemRole = new MemberLeaderRole();
                    //        oNewMemRole.ChurchBodyId = oChuTransf.ToChurchBodyId;
                    //        oNewMemRole.ChurchMemberId = oNewChuMember.Id; // oMemRole.ChurchMemberId;
                    //        oNewMemRole.LeaderRoleId = oMinRole.LeaderRoleId;
                    //        oNewMemRole.Commenced = oChuTransf.TransferDate;
                    //        oNewMemRole.Completed = null;
                    //        oNewMemRole.CompletionReason = oChuTransf.TransferType;
                    //        oNewMemRole.IsCurrServing = true;
                    //        oNewMemRole.Created = DateTime.Now;
                    //        oNewMemRole.LastMod = DateTime.Now;
                    //        //
                    //        currCtx.Add(oNewMemRole);
                    //    }
                    //}

                    //user profiles... deactivate
                    var oUserProfile = _masterContext.UserProfile.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                                    c.ChurchMemberId == oCurrChuMember.Id && c.UserStatus == "A").ToList();
                    foreach (var oUP in oUserProfile)
                    {   //user profile group... deactivate
                        var oUserProfileGroup = _masterContext.UserProfileGroup.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                                    c.UserProfileId == oUP.Id && c.Status == "A").ToList();
                        foreach (var oUPG in oUserProfileGroup)
                        {
                            oUPG.Status = "D";
                            oUPG.LastMod = tm;
                            oUPG.LastModByUserId = this._oLoggedUser.Id;
                            //
                            currCtx.Update(oUPG);
                        }

                        //user profile group... deactivate
                        var oUserProfileRole = _masterContext.UserProfileRole.Where(c => c.AppGlobalOwnerId == oChuTransf.AppGlobalOwnerId && c.ChurchBodyId == oChuTransf.FromChurchBodyId && 
                                                                                c.UserProfileId == oUP.Id && c.ProfileRoleStatus == "A").ToList();
                        foreach (var oUPR in oUserProfileRole)
                        {
                            oUPR.ProfileRoleStatus = "D";
                            oUPR.LastMod = tm;
                            oUPR.LastModByUserId = this._oLoggedUser.Id;
                            //
                            currCtx.Update(oUPR);
                        }

                        //finally the master...
                        oUP.UserStatus = "D";
                        oUP.LastMod = tm;
                        oUP.LastModByUserId = this._oLoggedUser.Id;
                        //
                        currCtx.Update(oUP);
                    }
                }


                // oChuTransf.ReqStatus = "C";     //   Closed... member transferred  ... but let member PHYSICALLY MOVE to new CB to finalize [CLOSE] transfer
                // oChuTransf.ApprovalStatus = "A";   // Assuming all the necessay steps/action steps have been approved.

                return true;
            }

            catch (Exception)
            {
                return false;
            }
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


        public JsonResult GetMemberListByCB(int? oAGOid, int? oCBid, bool addEmpty = false)
        {
            if (this._context == null)
            {
                this._context = GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null;
                }
            }

            if (oAGOid == null) oAGOid = this._oLoggedAGO.Id;

            var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Status == "A" && c.MemberScope == "I")  //Internal [C, L, P, M] only
                .OrderBy(c => (c.FirstName + c.MiddleName + c.LastName)).ToList()

            /// some level of security may be needed ... to allow access to CB membership from external ::: setting... Allow external access (with sister congregations) to relevant church info ...default: Yes

            //(c.OwnedByChurchBodyId == oCurrChuBody.Id ||
            // (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            // (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody))))

            .Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = GetConcatMemberName(c.Title, c.FirstName, c.MiddleName, c.LastName, false, false, false, false, false) + " [" + c.GlobalMemberCode + "]", /// Dr. Sam ...
                //  GetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, true, true, false, false, true),  // Sam Darteh
            })
            //.OrderBy(c => c.Text)
            .ToList();

            /// if (addEmpty) countryList.Insert(0, new CountryRegion { Id = "", Name = "Select" });             
            //return Json(new SelectList(countryList, "Id", "Name"));  

            if (addEmpty) oCMList.Insert(0, new SelectListItem { Value = "", Text = "Select member" });
            return Json(oCMList);
        }

         


        public IActionResult IndexReq_CT(int? oCBid, int dxn = 0, int svc_ndx = 1, string strTrxTP = "MT", bool blReqHis = false, bool loadLim = false)   //async Task<IActionResult>
        { // dxn = 1 (incoming), dxn = 2 (outgoing) ; svc_ndx = 1 (church worker), svc_ndx = 2 (approver), svc_ndx = 3 (self-service)  /// int? oCMid = null, 

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

                var setIndex = 0;
                if (!loadLim || dxn == 0)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                // var oAppGloOwnId = this._oLoggedAGO.Id;

                var oAGOid = this._oLoggedAGO.Id;
                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                
                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBid).FirstOrDefault();

                 
                //var oCMid = oCurrChuMember_LogOn != null ? oCurrChuMember_LogOn.Id : (int?)null;
                //var oCMGlobalCode = oCurrChuMember_LogOn != null ? oCurrChuMember_LogOn.GlobalMemberCode : null;
                 
                var oCTModel = new ChurchTransferModel();
                oCTModel.strTransferType = GetTransferTypeDesc(strTrxTP); //t_ct.TransferType == "MT" ? "Member Transfer" : t_ct.TransferType == "CT" ? "Clergy Transfer" : "Role Transfer",
                                                                          //  strTransferSubType = GetTransferSubTypeDesc(t_ct.TransferSubType);
                var lsCTModels = new List<ChurchTransferModel>();
                var oCMid = this._oLoggedUser.ChurchMemberId;
                ChurchMember oCM_Curr = null;
                ///

                //if (dxn == 0 || dxn == 1 || dxn == 2 ) // both
                //{
                //    if (svc_ndx == 1 )
                //        lsCTModels = GetTransferList(oCB, dxn, 1, strTrxTP, null, blReqHis);

                //    else if (svc_ndx == 2)
                //    { 
                //        var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Id == oCMid).FirstOrDefault();
                //        lsCTModels = GetTransferList(oCB, dxn, 2, strTrxTP, oCM_Curr, blReqHis);
                //    }

                //    else if (svc_ndx != 3)
                //    {
                //        var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Id == oCMid).FirstOrDefault();
                //        lsCTModels = GetTransferList(oCB, dxn, 3, strTrxTP, oCM_Curr, blReqHis);
                //    }  
                //}


                lsCTModels = GetTransferList(oCB, dxn, svc_ndx, strTrxTP, oCM_Curr, blReqHis);


                /// approvals ... limit to the approver
                if (svc_ndx == 2)
                {
                    lsCTModels = lsCTModels.Where(x => x.oApprovalXAction != null).ToList();
                } 


                if (dxn == 1)//incoming or both
                    oCTModel.lsChurchTransferModels_IN = lsCTModels.Where(x=> x.numTransferDxn == 1).ToList(); // GetTransferList(oCB, dxn, svc_ndx, strTrxTP, null, blReqHis); //((int)oCBid);

                if (dxn == 2) //outgoing  or both
                {
                    oCTModel.lsChurchTransferModels_OT = lsCTModels.Where(x => x.numTransferDxn == 2).ToList();

                    //if (svc_ndx == 1 || svc_ndx == 2)  // svc_ndx = 1 (church worker), svc_ndx = 2 (approver), svc_ndx = 3 (self-service)  
                    //    oCTModel.lsChurchTransferModels_OT  = lsCTModels.Where(x => x.numTransferDxn == 2).ToList(); // GetTransferList(oCB, dxn, svc_ndx, strTrxTP, null, blReqHis); 

                    //else if (svc_ndx == 3)// SELF SERVICE PORTAL 
                    //{
                    //    var oCMid = this._oLoggedUser.ChurchMemberId;
                    //    var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.Id == oCMid).FirstOrDefault();

                    //    oCTModel.lsChurchTransferModels_OT = GetTransferList(oCB, dxn, svc_ndx, strTrxTP, oCM_Curr, blReqHis);  
                    //}

                }
                else
                {
                    oCTModel.lsChurchTransferModels_IN = lsCTModels.Where(x => x.numTransferDxn == 1).ToList();
                    oCTModel.lsChurchTransferModels_OT = lsCTModels.Where(x => x.numTransferDxn == 2).ToList();
                }

                //ViewBag.IsPendingTransfer = false;

                //if (dxn == 0 || dxn == 1)//incoming or both
                //    oCTModel.lsChurchTransferModels_IN = GetTransfers_Incoming(oCB, strTrxTP); //((int)oCBid);

                //if (dxn == 0 || dxn == 2) //outgoing  or both
                //{ 
                //    if (svc_ndx == 1 || svc_ndx == 2)  // svc_ndx = 1 (church worker), svc_ndx = 2 (approver), svc_ndx = 3 (self-service) 
                //    {
                //        oCTModel.lsChurchTransferModels_OT = GetTransfers_Outgoing(oCB, strTrxTP, dxn, svc_ndx, oCM_Curr);        //(int)oCBid
                //        ViewBag.IsPendingTransfer = false;
                //    }

                //    else if (svc_ndx == 3)// SELF SERVICE PORTAL (int)oCBid                    
                //    {
                //        oCTModel.lsChurchTransferModels_OT = GetTransfers_Outgoing(oCB, strTrxTP, dxn, svc_ndx, oCM_Curr, blReqHis);  //oCurrChuMemberId_LogOn  true, 
                //        ///
                //        var currTransf = (from t_cm in _context.ChurchTransfer
                //                  .Where(x => x.TransferType == strTrxTP && x.FromChurchBodyId == oCBid && x.ChurchMemberId == oCMid &&
                //                 (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.ReqStatus != "U"))
                //                          select t_cm).ToList();

                //        ViewBag.IsPendingTransfer = currTransf.Count > 0;
                //    }


                //    //else
                //    //{
                //    //    oCTModel.lsChurchTransferModels = GetTransfers_Outgoing(oCB, strTransType, dxn, svc_ndx, oCM_Curr);//(int)oCBid
                //    //    ViewBag.IsPendingTransfer = false;

                //    //    //var currTransf = (from t_cm in _context.ChurchTransfer
                //    //    //          .Where(x => x.TransferType== strTransType && x.RequestorChurchBodyId == oCBid && //x.ChurchMemberId == oCurrChuMemberId_LogOn &&
                //    //    //         (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.Status != "U"))
                //    //    //                  select t_cm).ToList();
                //    //    //ViewBag.IsPendingTransfer = currTransf.Count > 0;
                //    //}
                //}




                //else //if (dxn == 2)//all transfers
                //{
                //    ViewBag.IsPendingTransfer = false;
                //    if (svc_ndx == 4)//all transfers
                //    {
                //        ViewBag.IsPendingTransfer = false;
                //        oCTModel.lsChurchTransferModels = GetTransfers_All(oCB, strTrxTP, svc_ndx);
                //    }
                //    else
                //        oCTModel.lsChurchTransferModels = GetTransfers_All(oCB, strTrxTP, -1);
                //}


                oCTModel.serviceTask = svc_ndx;
                oCTModel.numTransferDxn = dxn; 
                oCTModel.strTransferDxn = dxn == 1 ? "Incoming" : "Outgoing";  //there cannot be NEW incoming requests...
                 
                //
                oCTModel.oAppGloOwnId = oAGOid; 
                oCTModel.oChurchBodyId = oCBid;
                oCTModel.oChurchBody = oCB;
                //
                oCTModel.oUserId_Logged = _oLoggedUser.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;

                oCTModel.oMemberId_Logged = this._oLoggedUser.ChurchMemberId; 
                //
                oCTModel.setIndex = setIndex;

                //oCTModel.oAppGloOwnId = this._oLoggedAGO.Id; oCMModel.oAppGlobalOwn = this._oLoggedAGO;
                //oCTModel.oChurchBodyId = oCBid; oCMModel.oChurchBody = oCB;
                //oCTModel.strChurchBody = oCB.Name;
                ///
                //oCTModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                //oCTModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;


                oCTModel.oUserId_Logged = _oLoggedUser.Id;

                /// 
                //oCTModel.lsChurchTransferModels = oCTModelList;
                if (dxn == 1)
                    ViewData["oCMModel_List"] = oCTModel.lsChurchTransferModels_IN;
                else                
                    ViewData["oCMModel_List"] = oCTModel.lsChurchTransferModels_OT;


                var strDesc = "Church Member";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                oCTModel.strCurrTask = strDesc;


                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCSPModel"] = _oUPModel; TempData.Keep();


                //if (dxn == 1) // incoming
                //{
                //    if (loadLim)
                //        return PartialView("_vwIndexReq_CTIN", oCTModel);
                //    else
                //        return View("IndexReq_CTIN", oCTModel);
                //}
                //else if (dxn == 2) // outgoing
                //{
                //    if (loadLim)
                //        return PartialView("_vwIndexReq_CTOT", oCTModel);
                //    else
                //        return View("IndexReq_CTOT", oCTModel);
                //}
                //else
                //    return View("_ErrorPage");

                if (loadLim)
                {
                    //return PartialView("_vwIndexReq_CT", oCTModel);
                    if (dxn == 1)
                        return PartialView("_vwIndexReq_CTIN", oCTModel);

                    else if (dxn == 2)
                        return PartialView("_vwIndexReq_CTOT", oCTModel);

                }


                return View("IndexReq_CT", oCTModel);

            }
            catch (Exception ex)
            {
                return View("_ErrorPage");
            }
        }


        public IActionResult IndexReqAppr_CT(int? oCurrChuBodyId, int? oReqActionId = null, bool blReqHistory = false, string strTransType = "MT", int? oCMid = null)  //, int dxn)   //async Task<IActionResult>
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

                var loadLim = false; var setIndex = 0;
                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                var oAGOid = this._oLoggedAGO.Id;
                if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCurrChuBodyId != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


                var oCTModel = new ChurchTransferModel();
                oCTModel.oChurchBody = oCB;


               // var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCurrChuBodyId && c.Id == oCMid).FirstOrDefault();

                //History of requests, or currently pending requests
                if (blReqHistory == true)
                    oCTModel.strCurrUserTask = "Approval Requests History";
                else
                {
                    if (oReqActionId == null)
                        oCTModel.strCurrUserTask = "Approval Requests (Open)";
                    else
                        oCTModel.strCurrUserTask = "Parallel Approvals";
                }

                oCTModel.lsChurchTransferModels_OT = oReqActionId == null ? GetApprovalTransferRequests(oCB, strTransType, oCMid, blReqHistory) :
                                                                        GetApprovalTransferRequests(oCB, strTransType, oCMid, blReqHistory, oReqActionId);
                ViewBag.blApprViewHist = blReqHistory;
                oCTModel.strTransferType = GetTransferTypeDesc(strTransType);

                //oCTModel.strTransferType = strTransType == "CT" ? "Clergy/Ministerial Transfer" :
                //                                                    strTransType == "MT" ? "Member Transfer" :
                //                                                            strTransType == "RT" ? "Role Transfer" : "Church Transfer";
                oCTModel.oMemberId_Logged = oCMid; // oUserProfile.ChurchMember.Id;
                oCTModel.oCurrApprovalActionId = oReqActionId;

                // oCTModel.oCurrLoggedMember = oCurrChuMember_LogOn;
                //reassign
               // oCTModel.oCoreChurchlifeVM = oCoreChuLife;



                //
                oCTModel.oAppGloOwnId = oAGOid;
                oCTModel.oChurchBodyId = oCurrChuBodyId;
                //
                oCTModel.oUserId_Logged = _oLoggedUser.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;

                //oCTModel.oMemberId_Logged = oCurrChuMemberId_LogOn;
                //
                oCTModel.setIndex = setIndex;

                //oCTModel.oAppGloOwnId = this._oLoggedAGO.Id; oCMModel.oAppGlobalOwn = this._oLoggedAGO;
                //oCTModel.oChurchBodyId = oCBid; oCMModel.oChurchBody = oCB;
                //oCTModel.strChurchBody = oCB.Name;
                ///
                //oCTModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                //oCTModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;


                oCTModel.oUserId_Logged = _oLoggedUser.Id;

                /// 
                /// oCTModel.lsChurchTransferModels = oCTModelList;
                ViewData["oCMModel_List"] = oCTModel.lsChurchTransferModels_OT;

                var strDesc = "Church Member";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                oCTModel.strCurrTask = strDesc;


                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCSPModel"] = _oUPModel; TempData.Keep();



                if (loadLim)
                    return PartialView("_vwIndexReqAppr_CT", oCTModel);
                else
                    return View("IndexReqAppr_CT", oCTModel);


            }
            catch (Exception ex)
            {
                throw;
            }
        }


        ////churchtransfer/addoredit_mt?oCBid=7&oCMid=80&id=1
       /// public IActionResult AddOrEdit_MT(int? oCBid, int dxn = 2, int svc_ndx = 1, int id = 0)
        public IActionResult AddOrEdit_MT( int? oCBid, int dxn = 2, int svc_ndx = 1, string strTrxTP = "MT", int? oCMid = null, int vw = -1, int id = 0 )
        {  //int? oCMid = null, vw = 0 [view], 1[add], 2[edit], 3[del], 4[actionby---approve, deny etc.], ChurchTransferDetail_MDL oCurrMdl = null)
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


                var setIndex = 0; 
                //var loadLim = (vw != 1 && vw != 2); // modals -- add, edit                
                //if (!loadLim)
                //    _ = this.LoadClientDashboardValues(this._clientDBConnString);

                // Client
                var oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                // MSTR
                var oUserId = this._oLoggedUser.Id;
                var oAGO_MSTR = this._oLoggedAGO_MSTR; var oCB_MSTR = this._oLoggedCB_MSTR; // _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId); 
                var oAGO = this._oLoggedAGO;
                var oCB = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
                if (oCB == null) oCB = this._oLoggedCB;

                if (oAGO_MSTR == null || oCB_MSTR == null || oAGO == null || oCB == null)   // || oCU_Parent == null church units may be networked...
                { return View("_ErrorPage"); }

                var strDesc = "Church Transfer";
                var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();
                 

                var oCTModel = new ChurchTransferModel();

                //var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                //                    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                //oCP_List_1 = oCP_List_1.Where(c =>
                //                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();
                 
                if (id == 0)
                {
                    //create user and init... 
                    var oCT_NewMT = new ChurchTransfer();
                    oCT_NewMT.AppGlobalOwnerId = oAppGloOwnId;  // pick ...
                    oCT_NewMT.ChurchBodyId = oCBid;  // pick ...
                    //
                    //oCT_NewMT.ChurchMemberId =  oCMid  ;  // pick ...
                    oCT_NewMT.RequestorMemberId = this._oLoggedUser.ChurchMemberId;
                    // oCT_NewMT.RequestorRoleId = oReqMLR != null ? oReqMLR.Id : (int?)null; //requestMLRId; 
                    oCT_NewMT.RequestorChurchBodyId = oCBid;
                    oCT_NewMT.FromChurchBodyId = oCBid;

                    //oCT_NewMT.oChurchTransfer.FromChurchPositionId = null;
                    //oCT_NewMT.oChurchTransfer.FromMemberLeaderRoleId  = null;
                    oCT_NewMT.CurrentScope = "I";  //Internal cong. submission first

                    oCT_NewMT.WorkSpanStatus = "A"; //NEW... DRAFT  
                    oCT_NewMT.ReqStatus = "N"; //NEW... DRAFT  
                    oCT_NewMT.ApprovalStatus = null;

                    oCT_NewMT.TransferType = "MT"; 
                    oCT_NewMT.RequestDate = DateTime.Now;
                    oCT_NewMT.CustomTransMessage =  "Please assist with my transfer request to attached congregation. Thank you";
                    // oCT_NewMT.oChurchTransfer.TransferType  = "MT";  //Member Transfer... done at definition

                    //view purpose....
                    //oCT_NewMT.RequestorChurchBody = oCurrChuBodyLogOn;
                    //oCT_NewMT.RequestorChurchMember = oCurrChuMember_LogOn;
                    //oCT_NewMT.FromChurchBody = oCurrChuBodyLogOn;
                    //
                    //oCTModel.oAppGloOwn = oCurrChuBodyLogOn.AppGlobalOwner;
                    //oCTModel.oChurchBody = oCurrChuBodyLogOn;
                    //oCTModel.oRequestorChurchBody = oCurrChuBodyLogOn; 
                    //oCTModel.oChurchMemberId = oCurrChuMember_LogOn;

                    oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                    oCTModel.oMemberId_Logged = this._oLoggedUser.ChurchMemberId;
                    oCTModel.oUserId_Logged = this._oLoggedUser.Id;

                    //// 
                    //oCTModel.strFromMemberFullName = memFromInfo.strFromMemberFullName;
                    //oCTModel.strRequestorFullName = memFromInfo.strFromMemberFullName;
                    //oCTModel.strFromMemberPos = memFromInfo.strFromMemberPos;
                    //oCTModel.strFromMemberStatus = memFromInfo.strFromMemberStatus;
                    //oCTModel.strFromMemberAgeGroup = memFromInfo.strFromMemberAgeGroup;
                    //oCTModel.strFromMemberRole = memFromInfo.strFromMemberRole;
                    //oCTModel.strRequestorRole = memFromInfo.strFromMemberRole;

                    ////
                    //oCTModel.strRequestorChurchBody = oCurrChuBodyLogOn.Name;
                    //oCTModel.strRequestorChurchLevel = oCurrChuBodyLogOn.ChurchLevel.CustomName;
                    //oCTModel.strFromChurchBody = oCT_NewMT.strRequestorChurchBody;
                    //oCTModel.strFromChurchLevel = oCT_NewMT.strRequestorChurchLevel;
                    //// oCTModel.strRequestorRole = oReqMLR != null ? oReqMLR.LeaderRole.RoleName : "";

                    oCTModel.strCurrScope = oCT_NewMT.CurrentScope.Equals("E") ? "External" : "Internal";

                    ///
                    oCTModel.oChurchTransfer = oCT_NewMT;
                    oCTModel.numFromChurchBodyId = oCB.Id;
                    oCTModel.strFromChurchBody = oCB.Name;
                    oCTModel.strFromChurchLevel = oCB.ChurchLevel != null ? oCB.ChurchLevel.CustomName : "";

                    oCTModel.numRequestorChurchBodyId = oCB.Id;
                    oCTModel.strRequestorChurchBody = oCTModel.strFromChurchBody;
                    oCTModel.strRequestorChurchLevel = oCTModel.strFromChurchLevel;

                    oCTModel.strReqStatus = GetRequestStatusDesc(oCT_NewMT.ReqStatus); //"New Request /Draft";
                    oCTModel.strWorkSpanStatus = GetStatusDesc(oCT_NewMT.WorkSpanStatus); //"New Request /Draft";

                }

                else
                {

                    var oCTList = new List<ChurchTransferModel>();
                    if (svc_ndx != 3)
                        oCTList = GetTransferList(oCB, dxn, svc_ndx, strTrxTP, null, null, vw, id);

                    else
                    {
                        if (oCMid == null) oCMid = this._oLoggedUser.ChurchMemberId;
                        var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCBid && c.Id == oCMid).FirstOrDefault();

                        oCTList = GetTransferList(oCB, dxn, svc_ndx, strTrxTP, oCM_Curr, null, vw, id);   /// getting the transfer details via Self service
                    }

                    if (oCTList.Count == 0) return PartialView("_vwErrorPage");

                    oCTModel = oCTList[0]; // new ChurchTransferModel();
                    oCTModel.oChurchBody = oCB;
                    var oCurrChurchBody = oCB;


                   // oCTModel = GetMemberTransferDetail_Outgoing(oCB, id);
                     
                   // if (oCTModel == null) return View("_vwErrorPage"); //

                    ////get requestor detail
                    //var oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchUnit)
                    //        .Where(c => c.ChurchBodyId == oCBid && c.ChurchMemberId == oMemTransf_MDL.oChurchTransfer.RequestorMemberId
                    //            && c.LeaderRoleId == oMemTransf_MDL.oChurchTransfer.RequestorRoleId).FirstOrDefault();
                    //// oMemTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCurrChuMember_LogOn.MemberGlobalId) ? oCurrChuMember_LogOn.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(oCurrChuMember_LogOn.Title) ? oCurrChuMember_LogOn.Title : "") + ' ' + oCurrChuMember_LogOn.FirstName).Trim() + " " + oCurrChuMember_LogOn.MiddleName).Trim() + " " + oCurrChuMember_LogOn.LastName).Trim();
                    //if (oReqMLR != null)
                    //    oMemTransf_MDL.strRequestorFullName += oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.ChurchUnit != null ? ", " + oReqMLR.ChurchUnit.Name : "") + ")" : "";

                    // if (oMemTransf_MDL.oChurchTransfer == null) return View(oMemTransf_MDL); //



                }

                //
                oCTModel.oAppGloOwnId = oAppGloOwnId;
                oCTModel.oChurchBodyId = oCBid;
                //
                oCTModel.oUserId_Logged = _oLoggedUser.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;

                //oCTModel.oMemberId_Logged = oCurrChuMemberId_LogOn;
                //
                oCTModel.setIndex = setIndex;

                //oCTModel.oAppGloOwnId = this._oLoggedAGO.Id; oCMModel.oAppGlobalOwn = this._oLoggedAGO;
                //oCTModel.oChurchBodyId = oCBid; oCMModel.oChurchBody = oCB;
                //oCTModel.strChurchBody = oCB.Name;
                ///
                //oCTModel.oAppGloOwnId_Logged_MSTR = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                //oCTModel.oChurchBodyId_Logged_MSTR = this._oLoggedCB.MSTR_ChurchBodyId;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;


                oCTModel.oUserId_Logged = _oLoggedUser.Id;


                // ChurchBody oChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
                oCTModel = this.popLookups_ChurchTransfer_MT(oCTModel, oCB);
                // oCurrMdl.strCurrTask = "Church Tithe";
                 

                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oCTModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCurrMod"] = _oCTModel; TempData.Keep();

                 
                return PartialView("_AddOrEdit_MT", oCTModel);

            }
            catch (Exception ex)
            {
                return PartialView("_vwErrorPage");
            }
             
        }

        public IActionResult OpenReq_MT(int? oCBid, int dxn = 2, int svc_ndx = 1, string strTrxTP = "MT", int? oCMid = null, int vw = -1, int id = 0 )  // -1 :- view, 0 = list, 1 -add, 2-edit, 3-del
        {//, int? oCMid = null  reqDxn = 1 : incoming, reqDxn = 2: outgoing  including Scope = I or E
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

                var loadLim = true; 
                var setIndex = 0;
                if (!loadLim)
                    _ = this.LoadClientDashboardValues(this._clientDBConnString);


                var oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCBid == null) oCBid = this._oLoggedCB.Id;

                ChurchBody oCB = this._oLoggedCB;
                if (oCBid != this._oLoggedCB.Id)
                    oCB = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCBid).FirstOrDefault();

                 

                var oCTList = new List<ChurchTransferModel>();
                if (svc_ndx != 3)
                    oCTList = GetTransferList(oCB, dxn, svc_ndx, strTrxTP, null, null, vw, id);

                else
                {
                    if (oCMid == null) oCMid = this._oLoggedUser.ChurchMemberId;
                    var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCBid && c.Id == oCMid).FirstOrDefault();

                    oCTList = GetTransferList(oCB, dxn, svc_ndx, strTrxTP, oCM_Curr, null, vw, id);   /// getting the transfer details via Self service
                     
                }

                if (oCTList.Count == 0) return PartialView("_vwErrorPage");

                var oCTModel = oCTList[0]; // new ChurchTransferModel();
               
                var oCurrChurchBody = oCB;


                /// get the AAS if approvals started...
                 
                /// check No Approval action pending or in progress
               if (dxn == 1)
                {
                    var oAppAction_Curr = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                        .Where(c => c.AppGlobalOwnerId == oCTModel.oChurchTransfer.AppGlobalOwnerId &&
                                    c.ProcessCode == "TRF_IN" && c.ProcessSubCode == oCTModel.oChurchTransfer.TransferType && c.CallerRefId == oCTModel.oChurchTransfer.Id) /// && c.Status == "A"
                                   .FirstOrDefault();

                    if (oAppAction_Curr != null)
                    { /// oCurrApprovalActionStep
                        oCTModel.oCurrApprovalActionStepModel = new ApprovalActionStepModel()
                        {
                             lsApprActionStepModels = (from t_aas in _context.ApprovalActionStep.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ApproverChurchBody)
                                                        .Include(t => t.ApproverChurchMember).Include(t => t.ApproverChurchRole).Include(t => t.ApprovalAction)
                                                        .Where(c => c.AppGlobalOwnerId == oCTModel.oChurchTransfer.AppGlobalOwnerId && c.ApprovalActionId == oAppAction_Curr.Id) 
                                                       select new ApprovalActionStepModel()
                                                       {
                                                           oAppGloOwnId = oCTModel.oAppGloOwnId,
                                                           oChurchBodyId = oCTModel.oChurchBodyId,
                                                           oApprovalActionStep = t_aas,
                                                           oApprovalAction = t_aas.ApprovalAction,
                                                           strApprovalActionStatus = t_aas.ApprovalAction != null ? (GetRequestStatusDesc(t_aas.ApprovalAction.Status, false) + (!string.IsNullOrEmpty(t_aas.ApprovalAction.Comments) ? ": " + t_aas.ApprovalAction.Comments.ToLower() : "")) : "",

                                                           ///
                                                           strChurchBody = t_aas.ChurchBody != null ? t_aas.ChurchBody.Name : "",
                                                           strApprovalReqDate = t_aas.StepRequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.StepRequestDate) : "N/A",
                                                           strApprovalDate  = t_aas.ActionDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.ActionDate) : "N/A",
                                                           strApproverChurchBody = t_aas.ApproverChurchBody != null ? t_aas.ApproverChurchBody.Name : "",
                                                           strApproverChurchMember = t_aas.ApproverChurchMember != null ? GetConcatMemberName(t_aas.ApproverChurchMember.Title, t_aas.ApproverChurchMember.FirstName, t_aas.ApproverChurchMember.MiddleName, t_aas.ApproverChurchMember.LastName, false, false, true, true, false) : "",
                                                           strApproverChurchRole = t_aas.ApproverChurchRole != null ? t_aas.ApproverChurchRole.Name : "N/A",
                                                           strApprovalStepStatus = GetRequestProcessStatusDesc(t_aas.ActionStepStatus) + (!string.IsNullOrEmpty(t_aas.Comments) ? ": " + t_aas.Comments.ToLower() : ""),

                                                       }).ToList(),

                            oAppGloOwnId = oCTModel.oAppGloOwnId,
                            oChurchBodyId = oCTModel.oChurchBodyId 
                        };
                    } 
                }
                else
                {
                    var oAppAction_Curr = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                        .Where(c => c.AppGlobalOwnerId == oCTModel.oChurchTransfer.AppGlobalOwnerId &&
                                    c.ProcessCode == "TRF_OT" && c.ProcessSubCode == oCTModel.oChurchTransfer.TransferType && c.CallerRefId == oCTModel.oChurchTransfer.Id) /// && c.Status == "A"
                                   .FirstOrDefault();

                    if (oAppAction_Curr != null)
                    {
                        oCTModel.oCurrApprovalActionStepModel = new ApprovalActionStepModel()
                        {
                            lsApprActionStepModels = (from t_aas in _context.ApprovalActionStep.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ApproverChurchBody)
                                                       .Include(t => t.ApproverChurchMember).Include(t => t.ApproverChurchRole)
                                                       .Where(c => c.AppGlobalOwnerId == oCTModel.oChurchTransfer.AppGlobalOwnerId && c.ApprovalActionId == oAppAction_Curr.Id)
                                                      select new ApprovalActionStepModel()
                                                      {
                                                          oAppGloOwnId = oCTModel.oAppGloOwnId,
                                                          oChurchBodyId = oCTModel.oChurchBodyId,
                                                          oApprovalActionStep = t_aas,
                                                          oApprovalAction = t_aas.ApprovalAction,
                                                          strApprovalActionStatus = t_aas.ApprovalAction != null ? (GetRequestStatusDesc(t_aas.ApprovalAction.Status, false) + (!string.IsNullOrEmpty(t_aas.ApprovalAction.Comments) ? ": " + t_aas.ApprovalAction.Comments.ToLower() : "")) : "",

                                                          ///
                                                          strChurchBody = t_aas.ChurchBody != null ? t_aas.ChurchBody.Name : "",
                                                          strApprovalReqDate = t_aas.StepRequestDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.StepRequestDate) : "N/A",
                                                          strApprovalDate = t_aas.ActionDate != null ? String.Format("{0:d-MMM-yyyy}", (DateTime)t_aas.ActionDate) : "N/A",
                                                          strApproverChurchBody = t_aas.ApproverChurchBody != null ? t_aas.ApproverChurchBody.Name : "",
                                                          strApproverChurchMember = t_aas.ApproverChurchMember != null ? GetConcatMemberName(t_aas.ApproverChurchMember.Title, t_aas.ApproverChurchMember.FirstName, t_aas.ApproverChurchMember.MiddleName, t_aas.ApproverChurchMember.LastName, false, false, true, true, false) : "",
                                                          strApproverChurchRole = t_aas.ApproverChurchRole != null ? t_aas.ApproverChurchRole.Name : "N/A",
                                                          strApprovalStepStatus = GetRequestProcessStatusDesc(t_aas.ActionStepStatus) + (!string.IsNullOrEmpty(t_aas.Comments) ? ": " + t_aas.Comments.ToLower() : ""),

                                                      }).ToList(),

                            oAppGloOwnId = oCTModel.oAppGloOwnId,
                            oChurchBodyId = oCTModel.oChurchBodyId
                        };
                    } 
                }



                /// some details ...AAS
                if (oCTModel.oCurrApprovalActionStepModel != null)
                {
                    if (oCTModel.oCurrApprovalActionStepModel.lsApprActionStepModels.Count > 0)
                    {
                        var currStep = oCTModel.oCurrApprovalActionStepModel.lsApprActionStepModels.Where(x => x.oApprovalActionStep.IsCurrentStep == true).FirstOrDefault();
                        if (currStep != null)
                        {
                            oCTModel.strCurrApproverChurchMember = currStep.strApproverChurchMember;
                            oCTModel.strCurrApproverChurchRole = currStep.strApproverChurchRole;
                            ///
                            oCTModel.oCurrApprovalActionId = currStep.oApprovalActionStep.ApprovalActionId;
                            oCTModel.oCurrApproverChurchBodyId = currStep.oApprovalActionStep.ApproverChurchBodyId;
                            oCTModel.oCurrApproverChurchMemberId = currStep.oApprovalActionStep.ApproverChurchMemberId;
                            oCTModel.oCurrApproverChurchRoleId = currStep.oApprovalActionStep.ApproverChurchRoleId;
                            oCTModel.oCurrApproverMemberChurchRoleId = currStep.oApprovalActionStep.ApproverMemberChurchRoleId;

                            oCTModel.strActionStepStatusCode = currStep.oApprovalActionStep.ActionStepStatus;
                            oCTModel.strActionStepStatus = GetRequestProcessStatusDesc(currStep.oApprovalActionStep.ActionStepStatus); 
                        }
                    }
                }



                //if (dxn == 1)   /// incoming 
                //{
                //    var oCTD = GetMemberTransferDetail_Incoming(oCurrChurchBody, id);
                //    if (oCTD != null) oCTModel = oCTD;
                //    else return View("_ErrorPage");
                //}

                //else if (dxn == 2)   /// outgoing
                //{
                //    var currTransf = (from t_cm in _context.ChurchTransfer
                //                 .Where(x => x.RequestorChurchBodyId == oCBid && x.Id == id && // ((oCMid != null && x.ChurchMemberId == oCMid) || oCMid == null) &&
                //                            (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.ReqStatus != "U"))
                //                      select t_cm).ToList();

                //    ViewBag.IsPendingTransfer = currTransf.Count > 0;  /// ?????

                //    var oCTD = GetMemberTransferDetail_Outgoing(oCurrChurchBody, id);//oCBid
                //    if (oCTD != null) oCTModel = oCTD;
                //    else return View("_ErrorPage");
                //}
                //else
                //{
                //    var oCTD = GetMemberTransferDetail_Outgoing(oCurrChurchBody, id);//oCBid
                //    if (oCTD != null) oCTModel = oCTD;
                //    else return View("_ErrorPage");
                //}


                oCTModel.serviceTask = svc_ndx;    //church worker = 1, approver = 2, member self-service = 3
                oCTModel.strTransferDxn = dxn == 1 ? "Incoming" : "Outgoing";    // incoming = 1, outgoing = 2, 0 == both
                                                                                 //oCTModel.oCurrLoggedMemberId = oCurrChuMemberId_LogOn;
                                                                                 //oCTModel.oCurrLoggedMember = oCurrChuMember_LogOn;
                                                                                 //oChuTransfDet_MDL.dx = dxn;

                //reassing...
                // oCTModel.oCoreChurchlifeVM = oCoreChuLife;


                oCTModel.setIndex = setIndex;
                oCTModel.oUserId_Logged = this._oLoggedUser.Id;
                oCTModel.oAppGloOwnId_Logged = this._oLoggedAGO.Id;
                oCTModel.oChurchBodyId_Logged = this._oLoggedCB.Id;
                /// oCTModel.oMemberId_Logged = this._oLoggedUser.Id;
                //
                oCTModel.oAppGloOwnId = oAppGloOwnId;
                oCTModel.oChurchBodyId = oCBid;
                // var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();
                oCTModel.oChurchBody = oCurrChurchBody;  // != null ? oCurrChuBody : null;


                // ChurchBody oChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCBid).FirstOrDefault();

                oCTModel = this.popLookups_ChurchTransfer_MT(oCTModel, oCurrChurchBody);

                // oCurrMdl.strCurrTask = "Church Tithe";

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
                TempData["oVmCurr"] = _oCurrMdl; TempData.Keep();


                return PartialView("_vwOpenReq_MT", oCTModel); 
            }

            catch (Exception ex)
            {
                return PartialView("_vwErrorPage");
            }
        }


        public ChurchTransferModel popLookups_ChurchTransfer_MT(ChurchTransferModel vmLkp, ChurchBody oCurrChurchBody)
        {

            if (vmLkp == null || oCurrChurchBody == null) return vmLkp;

            if (vmLkp != null)
            {

                vmLkp.lkp_TransferTypes = new List<SelectListItem>();
                foreach (var dl in dlTransferTypes)
                {
                    if (dl.Val == "MT" || dl.Val == "RT") vmLkp.lkp_TransferTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });
                }


               
                    //var strNVPCode1 = "CMT";    // c.Id != oCurrNVP.Id && 
                    //var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                    //                                   .Where(c => c.AppGlobalOwnerId == oChurchMember.AppGlobalOwnerId && c.NVPCode == strNVPCode1).ToList();
                    //oNVP_List_1 = oNVP_List_1.Where(c =>
                    //                    (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                    //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                    //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    //vmLkp.lkpChurchMemTypes = oNVP_List_1 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                    //                            .OrderBy(c => c.OrderIndex)
                    //                            .ThenBy(c => c.NVPValue)
                    //                            .Select(c => new SelectListItem()
                    //                            {
                    //                                Value = c.Id.ToString(),
                    //                                Text = c.NVPValue
                    //                            })
                    //                            .ToList();

                    vmLkp.lkpChurchMemTypes = new List<SelectListItem>();   /// (dl.Val == "C" || dl.Val == "L" || dl.Val == "P" || dl.Val == "M") 
                    foreach (var dl in dlMemTypeCode) { vmLkp.lkpChurchMemTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc, Disabled = (dl.Val == "G" || dl.Val == "A" || dl.Val == "N") }); }

                    var strNVPCode2 = "CMS";    // Driven by CLA
                    var oNVP_List_2 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                                       .Where(c => c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && c.NVPCode == strNVPCode2).ToList();

                    oNVP_List_2 = oNVP_List_2.Where(c =>
                                        (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();


                var oNVP_List_IN = oNVP_List_2.Where(c => c.IsAvailable == true && c.IsDeceased == false).ToList();
                var oNVP_List_OT = oNVP_List_2.Where(c => c.IsAvailable == false && c.IsDeceased == false).ToList();

                vmLkp.lkpChurchMemStatuses_OT = oNVP_List_OT //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                .OrderBy(c => c.OrderIndex)
                                                .ThenBy(c => c.NVPValue)
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.NVPValue
                                                })
                                                .ToList();

                vmLkp.lkpChurchMemStatuses_IN = oNVP_List_IN //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
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
                                                       .Where(c => c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId && c.NVPCode == strNVPCode3).ToList();
                    oNVP_List_3 = oNVP_List_3.Where(c =>
                                        (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    vmLkp.lkpChurchRanks = oNVP_List_3 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                .OrderBy(c => c.OrderIndex)
                                                .ThenBy(c => c.NVPValue)
                                                .Select(c => new SelectListItem()
                                                {
                                                    Value = c.Id.ToString(),
                                                    Text = c.NVPValue
                                                })
                                                .ToList();
                




                //this.lkp_TransfApprovalStatuses = new List<SelectListItem>();
                //foreach (var dl in dlTransfApprovalStatuses) { this.lkp_TransfApprovalStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                //this.lkp_AckStatuses = new List<SelectListItem>();
                //foreach (var dl in dlTransfStatuses) { this.lkp_AckStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                //vm.lkp_TransMessages = _context.AppUtilityNVP.Where(c => c.ChurchBodyId == oCurrChurchBody.Id && c.NVPCode == "TRNF_MSG")
                //                              .OrderBy(c => c.NVPValue).ToList()
                //                              .Select(c => new SelectListItem()
                //                              {
                //                                  Value = c.Id.ToString(),
                //                                  Text = c.NVPValue
                //                              })
                //                              // .OrderBy(c => c.Text)
                //                              .ToList();


                //vm.lkp_TransReasons = _context.AppUtilityNVP.Where(c => c.ChurchBodyId == oCurrChurchBody.Id && c.NVPCode == "TRNF_RSN")
                //                              .OrderBy(c => c.NVPValue).ToList()
                //                              .Select(c => new SelectListItem()
                //                              {
                //                                  Value = c.Id.ToString(),
                //                                  Text = c.NVPValue
                //                              })
                //                              // .OrderBy(c => c.Text)
                //                              .ToList();

                //this.lkp_ToChurchLevels = _context.ChurchLevel.Where(c => c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId) // && c.ChurchLevelId == oChurchBody.ChurchLevelId)
                //    .OrderBy(c => c.LevelIndex).ThenBy(c => c.Name)
                //    .ToList()
                //                                .Select(c => new SelectListItem()
                //                                {
                //                                    Value = c.Id.ToString(),
                //                                    Text = c.CustomName
                //                                })
                //                                // .OrderBy(c => c.Text)
                //                                .ToList();

                //var qry = new ChurchTransferController(_context, null).GetChurchBodyList(oCurrChurchBody); // (int)oCurrChurchBody.Id);
                //vm.lkp_FromCongregations = qry.Select(c => new SelectListItem()
                //{
                //    Value = c.Id.ToString(),
                //    Text = c.Name
                //})
                //    .ToList();
                //vm.lkp_FromCongregations.Insert(0, new SelectListItem { Value = "", Text = "Select" });


                ////GetChurchBodySubCategoryList
                //if (oCurrChurchBody.AppGlobalOwner.TotalLevels > 2)
                //{
                //    var qry_1 = new ChurchTransferController(_context, null).GetChurchBodySubCategoryList((int)oCurrChurchBody.AppGlobalOwnerId, (int)oCurrChurchBody.Id, (int)oCurrChurchBody.Id, 1);  //(int)oCurrChurchBody.ParentChurchBodyId
                //    var ls_1 = qry_1.Select(c => new SelectListItem()
                //    {
                //        Value = c.Id.ToString(),
                //        Text = c.Name
                //    })
                //            .ToList();
                //    ls_1.Insert(0, new SelectListItem { Value = null, Text = "Select" });

                //    vm.lkp_CongNextCategory = ls_1;

                //    if (qry_1.Count > 0)
                //    {
                //        var tempCB = qry_1[0]; // ToChurchBody.ChurchCategory;  // next category
                //        vm.strToChurchLevel_1 = tempCB.ChurchLevel.CustomName;
                //        //  vm.ToChurchBodyId_Categ1 = tempCB.Id;
                //    }
                //}
                //else
                //{
                //    //...to be loaded @runtime
                //    var _qry = qry.Where(c => c.ChurchType == "CF" && c.Id != oCurrChurchBody.Id).ToList();
                //    var ls_1 = _qry.Select(c => new SelectListItem()
                //    {
                //        Value = c.Id.ToString(),
                //        Text = c.Name
                //    })
                //        .ToList();
                //    ls_1.Insert(0, new SelectListItem { Value = "", Text = "Select" });

                //    if (_qry.Count > 0)
                //    {
                //        var tempCB = _qry[0]; // ToChurchBody.ChurchCategory;  // next category
                //        vm.strToChurchLevel_1 = ""; // tempCB.ChurchLevel.CustomName;
                //        vm.ToChurchBodyId_Categ1 = null; // tempCB.Id;
                //    }

                //    vm.lkp_ToCongregations = ls_1;
                //    vm.strToChurchLevel = oCurrChurchBody.ChurchLevel.CustomName;
                //}

                //var qry_2 = new ChurchTransferController(_context, null).GetChurchMembersList((int)oCurrChurchBody.Id, null);  //Members = null, Church Leaders = CL, Clergy = CM
                //vm.lkp_ChurchMembers = qry_2.Select(c => new SelectListItem()
                //{
                //    Value = c.MemberId.ToString(),
                //    Text = c.strMemberFullName
                //}) //.OrderBy(c => c.Text)
                //.ToList();
                //vm.lkp_ChurchMembers.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            }

            return vmLkp;
        }
        public ChurchTransferModel popLookups_ChurchTransfer_CT(ChurchTransferModel vm, ChurchBody oCurrChurchBody)
        {
            if (vm != null)
            {

                vm.lkp_TransferTypes = new List<SelectListItem>();
                foreach (var dl in dlTransferTypes) { vm.lkp_TransferTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                //this.lkp_TransfApprovalStatuses = new List<SelectListItem>();
                //foreach (var dl in dlTransfApprovalStatuses) { this.lkp_TransfApprovalStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                //this.lkp_AckStatuses = new List<SelectListItem>();
                //foreach (var dl in dlTransfStatuses) { this.lkp_AckStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

                //vm.lkp_TransMessages = _context.AppUtilityNVP.Where(c => c.ChurchBodyId == oCurrChurchBody.Id && c.NVPCode == "TRNF_MSG_CLG")
                //                              .OrderBy(c => c.NVPValue).ToList()
                //                              .Select(c => new SelectListItem()
                //                              {
                //                                  Value = c.Id.ToString(),
                //                                  Text = c.NVPValue
                //                              })
                //                              // .OrderBy(c => c.Text)
                //                              .ToList();

                //vm.lkp_TransReasons = _context.AppUtilityNVP.Where(c => c.ChurchBodyId == oCurrChurchBody.Id && c.NVPCode == "TRNF_RSN_CLG")
                //                              .OrderBy(c => c.NVPValue).ToList()
                //                              .Select(c => new SelectListItem()
                //                              {
                //                                  Value = c.Id.ToString(),
                //                                  Text = c.NVPValue
                //                              })
                //                              // .OrderBy(c => c.Text)
                //                              .ToList();

                //this.lkp_ToChurchLevels = _context.ChurchLevel.Where(c => c.AppGlobalOwnerId == oCurrChurchBody.AppGlobalOwnerId) // && c.ChurchLevelId == oChurchBody.ChurchLevelId)
                //    .OrderBy(c => c.LevelIndex).ThenBy(c => c.Name)
                //    .ToList()
                //                                .Select(c => new SelectListItem()
                //                                {
                //                                    Value = c.Id.ToString(),
                //                                    Text = c.CustomName
                //                                })
                //                                // .OrderBy(c => c.Text)
                //                                .ToList();

                //var qry = new ChurchTransferController(_context, null).GetChurchBodyList(oCurrChurchBody); // (int)oCurrChurchBody.Id);
                //vm.lkp_FromCongregations = qry.Select(c => new SelectListItem()
                //{
                //    Value = c.Id.ToString(),
                //    Text = c.Name
                //})
                //    .ToList();
                //// vm.lkp_FromCongregations.Insert(0, new SelectListItem { Value = "", Text = "Select" });


                ////GetChurchBodySubCategoryList
                //if (oCurrChurchBody.AppGlobalOwner.TotalLevels > 2)
                //{
                //    var qry_1 = new ChurchTransferController(_context, null).GetChurchBodySubCategoryList((int)oCurrChurchBody.AppGlobalOwnerId, (int)oCurrChurchBody.Id, (int)oCurrChurchBody.Id, 1, true);  //(int)oCurrChurchBody.ParentChurchBodyId
                //    var ls_1 = qry_1.Select(c => new SelectListItem()
                //    {
                //        Value = c.Id.ToString(),
                //        Text = c.Name
                //    })
                //            .ToList();
                //    //  ls_1.Insert(0, new SelectListItem { Value = null, Text = "Select" });

                //    vm.lkp_CongNextCategory = ls_1;

                //    if (qry_1.Count > 0)
                //    {
                //        var tempCB = qry_1[0]; // ToChurchBody.ChurchCategory;  // next category
                //        vm.strToChurchLevel_1 = tempCB.ChurchLevel.CustomName;
                //        //  vm.ToChurchBodyId_Categ1 = tempCB.Id;
                //    }
                //}
                //else
                //{
                //    //...to be loaded @runtime
                //    var _qry = qry.Where(c => c.ChurchType == "CF" && c.Id != oCurrChurchBody.Id).ToList();
                //    var ls_1 = _qry.Select(c => new SelectListItem()
                //    {
                //        Value = c.Id.ToString(),
                //        Text = c.Name
                //    })
                //        .ToList();
                //    ls_1.Insert(0, new SelectListItem { Value = "", Text = "Select" });

                //    if (_qry.Count > 0)
                //    {
                //        var tempCB = _qry[0]; // ToChurchBody.ChurchCategory;  // next category
                //        vm.strToChurchLevel_1 = ""; // tempCB.ChurchLevel.CustomName;
                //        vm.ToChurchBodyId_Categ1 = null; // tempCB.Id;
                //                                         //
                //        vm.strFromChurchLevel_1 = "";
                //        vm.FromChurchBodyId_Categ1 = null;
                //    }

                //    vm.lkp_FromCongregations = ls_1;
                //    vm.strFromChurchLevel = oCurrChurchBody.ChurchLevel.CustomName;
                //    //
                //    vm.lkp_ToCongregations = ls_1;
                //    vm.strToChurchLevel = oCurrChurchBody.ChurchLevel.CustomName;
                //}


                //var qry_2 = new ChurchTransferController(_context, null).GetChurchMembersList((int)oCurrChurchBody.Id, "CM");  //Members = null, Church Leaders = CL, Clergy = CM
                //vm.lkp_ChurchMembers = qry_2.Select(c => new SelectListItem()
                //{
                //    Value = c.MemberId.ToString(),
                //    Text = c.strMemberFullName
                //}) //.OrderBy(c => c.Text)
                //.ToList();
                //vm.lkp_ChurchMembers.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            }

            return vm;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit_MT(ChurchTransferModel vm)  //save, int taskIndx
        { // 1.save-2.send-3.recall-4.cancel-5.resubmit-6.acknowledge-7.approve-8.suspend(hold)-9.decline-10.force complete(waive)-11.reopen-12.archive
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

                var taskIndx = vm.userRequestTask;
                var strDesc = "Member Transfer";
                var oCBid = vm.oChurchBodyId_Logged != null ? vm.oChurchBodyId_Logged : this._oLoggedCB.Id;
                

                if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again." });
                if (vm.oChurchTransfer == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again." });

                var arrData = ""; // TempData["oVmCurrMod"] as string;
                arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
                var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchTransferModel>(arrData) : vm;
                var oMTModel = vmMod != null ? vmMod : vm;

                // ChurchMember _oChanges = vm.oChurchMember;
                ChurchTransfer _oChangesMT = vm.oChurchTransfer;
                //ChurchTransfer _oChangesTempMT = _oChangesMT;  // vm.oChurchTransfer;                
                //ChurchTransfer _oChangesMTbkp = _oChangesTempMT;
                //oMTModel.oChurchTransfer = oMTModel.oChurchTransfer == null ? vm.oChurchTransfer : oMTModel.oChurchTransfer;
                               

                if (_oChangesMT == null)
                    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });

                /// get attached AGO
                if (_oChangesMT.AppGlobalOwnerId == null || _oChangesMT.AppGlobalOwnerId != this._oLoggedAGO.Id) _oChangesMT.AppGlobalOwnerId = this._oLoggedAGO.Id;
                var oAGO_MT = this._oLoggedAGO;


                /// check attached CB
                if (_oChangesMT.ChurchBodyId == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Requesting congregation could not be verified. Please refresh and try again.", signOutToLogIn = false });

                if (_oChangesMT.FromChurchBodyId == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Congregation of member to transfer could not be verified. Please refresh and try again.", signOutToLogIn = false });

                if (_oChangesMT.ToChurchBodyId == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Target congregation of member to transfer could not be verified. Please refresh and try again.", signOutToLogIn = false });

                if (_oChangesMT.RequestorChurchBodyId == _oChangesMT.ToChurchBodyId)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Congregation requesting member transfer cannot be same as the destination congregation. Please correct and try again.", signOutToLogIn = false });

                if (_oChangesMT.FromChurchBodyId == _oChangesMT.ToChurchBodyId)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Congregation of member to transfer cannot be same as the destination congregation. Please correct and try again.", signOutToLogIn = false });


                /// closed requests --- need to reopen
                if (_oChangesMT.WorkSpanStatus == "C" && taskIndx != 14) // closed (C) --- open (O)   ........ _oChangesMT.ReqStatus == "D" || _oChangesMT.ReqStatus == "U" ||  
                    return Json(new { taskSuccess = false, userMess = "Transfer request specified is closed. It must be re-opened before any operation can be performed on it." });   


                /// more server validaations

                var oCBReq_Curr = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.Id == _oChangesMT.ChurchBodyId).FirstOrDefault();
                if (oCBReq_Curr == null) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Requesting congregation could not be verified. Please refresh and try again.", signOutToLogIn = false });
                if (string.IsNullOrEmpty(oCBReq_Curr.GlobalChurchCode)) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Church code for congregation could not be verified. Please verify with System Admin and try again.", signOutToLogIn = false });
                ///
                var oCBFrom_Curr = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.Id == _oChangesMT.FromChurchBodyId).FirstOrDefault();
                if (oCBFrom_Curr == null) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Congregation of member to transfer could not be verified. Please refresh and try again.", signOutToLogIn = false });
                if (string.IsNullOrEmpty(oCBFrom_Curr.GlobalChurchCode)) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Church code for congregation could not be verified. Please verify with System Admin and try again.", signOutToLogIn = false });
                ///
                var oCBTo_Curr = _context.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.Id == _oChangesMT.ToChurchBodyId).FirstOrDefault();
                if (oCBTo_Curr == null) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Target congregation could not be verified. Please refresh and try again.", signOutToLogIn = false });
                if (string.IsNullOrEmpty(oCBTo_Curr.GlobalChurchCode)) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Church code for congregation could not be verified. Please verify with System Admin and try again.", signOutToLogIn = false });
                
                /// check attached CM
                if (_oChangesMT.ChurchMemberId == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Member to transfer could not be verfied. Please refresh and try again.", signOutToLogIn = false });

                var oCM_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.ChurchBodyId == _oChangesMT.ChurchBodyId && c.Id == _oChangesMT.ChurchMemberId).FirstOrDefault();
                if (oCM_Curr == null) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Attached member could not be verfied. Please refresh and try again.", signOutToLogIn = false });
                ///
                var oCMReq_Curr = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.ChurchBodyId == _oChangesMT.ChurchBodyId && c.Id == _oChangesMT.RequestorMemberId).FirstOrDefault();
                if (oCM_Curr == null) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Attached member could not be verfied. Please refresh and try again.", signOutToLogIn = false });

                /// member profile requied ... to ensure role for this task
                var oCM_Logged = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.ChurchBodyId == _oChangesMT.ChurchBodyId && c.Id == this._oLoggedUser.ChurchMemberId).FirstOrDefault();
                if (oCM_Logged == null) return Json(new { taskSuccess = false, oCurrId = _oChangesMT.Id, userMess = "Attached member profile of logged user could not be verfied. Authorization is required. Please refresh and try again.", signOutToLogIn = false });

                /// 
                //if (_oChangesMTbkp.RequestorChurchBody == null) _oChangesMTbkp.RequestorChurchBody = oCBReq_Curr;
                //if (_oChangesMTbkp.FromChurchBody == null) _oChangesMTbkp.FromChurchBody = oCBFrom_Curr;
                //if (_oChangesMTbkp.ToChurchBody == null) _oChangesMTbkp.ToChurchBody = oCBTo_Curr;
                //if (_oChangesMTbkp.ChurchMember == null) _oChangesMTbkp.ChurchMember = oCM_Curr;
                //if (_oChangesMTbkp.RequestorMember == null) _oChangesMTbkp.RequestorMember = oCMReq_Curr; 

                //ChurchTransfer _oChanges = vm.oChurchTransfer;   //  vm = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as ChurchTransferModel : vm; TempData.Keep();

                ///  

                var _oChangesMTbkp = new ChurchTransfer()
                {
                    RequestorChurchBody = oCBReq_Curr,
                    FromChurchBody = oCBFrom_Curr,
                    ToChurchBody = oCBTo_Curr,
                    ChurchMember = oCM_Curr,
                    RequestorMember = oCMReq_Curr
                };

                
                //List<ChurchMember> oChuMemNotifyEmailList = new List<ChurchMember>();
                // var _oChangesMT = vm.oChurchTransfer;  //  has updates 

                /// avoid... cyclic err
                if (_oChangesMT.RequestorChurchBody != null) _oChangesMT.RequestorChurchBody = null;
                if (_oChangesMT.FromChurchBody != null) _oChangesMT.FromChurchBody = null;
                if (_oChangesMT.ToChurchBody != null) _oChangesMT.ToChurchBody = null;
                if (_oChangesMT.ChurchMember != null) _oChangesMT.ChurchMember = null;
                if (_oChangesMT.RequestorMember != null) _oChangesMT.RequestorMember = null;

                
                var apprComment = vm.strApproverComment;
                var isTransferDone_MemMoved = false;

                // vm = TempData.Get<ChurchTransferModel>("oVmCurr"); TempData.Keep();

                if (_oChangesMT.FromChurchBodyId == _oChangesMT.ToChurchBodyId) 
                    return Json(new { taskSuccess = false, userMess = "Target congregation cannot be same as the congregation of the member" });
               
                if (string.IsNullOrEmpty(_oChangesMT.TransferReason)) 
                    return Json(new { taskSuccess = false, userMess = "Please provide the transfer reason. Hint: Reason helps both congregations to have history of what transpired." });
                                 
                if (taskIndx == 6 && _oChangesMT.CurrentScope == "I")  // Fr-CB -- only status
                {
                    if (_oChangesMT.TempMemStatusIdFrCB == null)
                        return Json(new { taskSuccess = false, userMess = "Please indicate member status [availability] after member is transferred." });
                }

                if (taskIndx == 6 && _oChangesMT.CurrentScope == "E")  // To-CB
                {
                    if (string.IsNullOrEmpty(_oChangesMT.TempMemTypeCodeToCB))
                        return Json(new { taskSuccess = false, userMess = "Please specify or affirm the member type of the would-be member of this congregation (" + _oChangesMTbkp.ToChurchBody.Name + "). Hint: Ideally member type is same across denomination" });

                    if (_oChangesMT.TempMemRankIdToCB == null)
                        return Json(new { taskSuccess = false, userMess = "Please specify or affirm the member rank [position/authority] of the would-be member of this congregation (" + _oChangesMTbkp.ToChurchBody.Name + "). Hint: Ideally member status is same across denomination" });

                    if (_oChangesMT.TempMemStatusIdToCB == null)
                        return Json(new { taskSuccess = false, userMess = "Please specify or affirm the member status [availability] of the would-be member of this congregation (" + _oChangesMTbkp.ToChurchBody.Name + "). Hint: Ideally member status is same across denomination" });

                }


                DetachAllEntities();                 

                ModelState.Remove("oChurchTransfer.AppGlobalOwnerId");
                ModelState.Remove("oChurchTransfer.ChurchMemberId");
                ModelState.Remove("oChurchTransfer.ChurchBodyId");
                ModelState.Remove("oChurchTransfer.AttachedToChurchBodyId");

                ModelState.Remove("oChurchTransfer.RequestorChurchBodyId");
                ModelState.Remove("oChurchTransfer.RequestorMemberId");
                ModelState.Remove("oChurchTransfer.RequestorRoleId");

                ModelState.Remove("oChurchTransfer.FromChurchBodyId");
                ModelState.Remove("oChurchTransfer.FromMemberChurchRoleId"); 
                ModelState.Remove("oChurchTransfer.FromChurchRankId");                

                ModelState.Remove("oChurchTransfer.ToChurchBodyId");
                ModelState.Remove("oChurchTransfer.ToChurchRoleId");
                ModelState.Remove("oChurchTransfer.ToRoleUnitId");
                
                ModelState.Remove("oChurchTransfer.IApprovalActionId");
                ModelState.Remove("oChurchTransfer.EApprovalActionId");
                ModelState.Remove("oChurchTransfer.TransMessageId");

                ModelState.Remove("oChurchTransfer.CreatedByUserId");
                ModelState.Remove("oChurchTransfer.LastModByUserId");
                

                //finally check error state...
                if (!ModelState.IsValid)
                    return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });


                // vm.oChurchTransfer = _oMT;
                vm.userRequestTask = taskIndx;

                //ChurchTransfer _oMemTransf = vm.oChurchTransfer; // vm.oChurchTransfer; // init...  
                //ChurchTransfer oMemTransf = vm.oChurchTransfer;

                var tm = DateTime.Now;
                var isNewREQ = false;
                //save details... locAddr
                List<ChurchMember> oChuMemNotifyList = new List<ChurchMember>();
                List<ChurchBody> oCBNotifyList = new List<ChurchBody>();
                List<ApprovalActionStep> oCurrApprovers = new List<ApprovalActionStep>();

                if (taskIndx == 1 || taskIndx == 2 || taskIndx == 5)  //SAVE, SEND, RESUBMIT
                { 
                    if (_oChangesMT.Id == 0)
                    {
                        if (!(taskIndx == 1 || taskIndx == 2))  //save... send  
                            return Json(new { taskSuccess = false, userMess = "Current request allows only 'Save as Draft, Send request' operation to be performed." });
                        

                        //if (_oChangesMT.ChurchMemberId == null || _oChangesMT.ToChurchBodyId == null)
                        //{ //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
                        //    return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
                        //}

                        if (taskIndx == 1) //...draft  [avoid unnecessary duplicates]
                        {
                            //check for pending.. unClosed requests OR successful closed requests
                            var currTransf = (from t_cm in _context.ChurchTransfer.AsNoTracking().Include(t=> t.ToChurchBody)
                                              .Where(x => x.RequestorChurchBodyId == _oChangesMT.RequestorChurchBodyId &&   //  && x.ToChurchBodyId == _oChangesMT.ToChurchBodyId 
                                              x.ChurchMemberId == _oChangesMT.ChurchMemberId && (x.ReqStatus == "N" || x.ReqStatus == null))
                                              select t_cm)
                                              .ToList();

                            if (currTransf.Count > 0)
                                return Json(new { taskSuccess = false, userMess = "Member transfer draft " + (currTransf[0].ToChurchBody != null ? " [To: " + currTransf[0].ToChurchBody.Name + "] " : "")  + "has already been initiated for this specified member. Hint: Use the saved Draft instead." });
                            
                        }
                        else if (taskIndx == 2 || taskIndx == 5) //... send /resubmit
                        {
                            //check for pending.. unClosed requests OR successful closed requests ... x.ReqStatus != "X" --terminated is Terminate... Cannot be undone   /..... (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "U"))
                            var currTransf = (from t_cm in _context.ChurchTransfer.AsNoTracking()
                                              .Where(x => x.RequestorChurchBodyId == _oChangesMT.RequestorChurchBodyId && x.ChurchMemberId == _oChangesMT.ChurchMemberId &&
                                                            x.TransferType == _oChangesMT.TransferType && x.Id == _oChangesMT.Id &&
                                              ((taskIndx == 2 && (x.ReqStatus == "N" && x.ReqStatus == "R")) || (taskIndx == 5 && (x.ReqStatus == "R" && x.ReqStatus == "D" && x.ReqStatus == "U"))))
                                              select t_cm)
                                              .ToList();

                            if (currTransf.Count == 0)
                                return Json(new { taskSuccess = false, userMess = "Member transfer already initiated or done for specified member. Hint: You can only Send draft or recalled requests; or Resubmit recalled, declined or unsuccessful requests." });
                        }

                        //var oMT = new ChurchTransfer
                        //{ //create user and init...

                        //    RequestorMemberId = _oChangesMT.RequestorMemberId,
                        //    RequestorRoleId = _oChangesMT.RequestorRoleId,
                        //    RequestorChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                        //    FromChurchBodyId = _oChangesMT.FromChurchBodyId,
                        //    CurrentScope = _oChangesMT.CurrentScope,
                        //    AckStatus = _oChangesMT.ReqStatus,
                        //    ApprovalStatus = _oChangesMT.ApprovalStatus,
                        //    TransferType = _oChangesMT.TransferType,
                        //    //ToRequestDate  = date request is forwarded to destination congregation
                        //    // RequestDate = tm,
                        //    Created = tm,  // _oChangesMT.Created,
                        //    LastMod = tm,
                        //    //
                        //    ChurchMemberId = _oChangesMT.ChurchMemberId,
                        //    ToChurchBodyId = _oChangesMT.ToChurchBodyId,
                        //    TransferReason = _oChangesMT.TransferReason,
                        //    TransMessageId = _oChangesMT.TransMessageId,
                        //    CustomTransMessage = _oChangesMT.CustomTransMessage,
                        //    Comments = _oChangesMT.Comments,
                        //    //
                        //    FromChurchRankId = _oChangesMT.FromChurchRankId <= 0 ? null : _oChangesMT.FromChurchRankId,
                        //    FromMemberChurchRoleId = _oChangesMT.FromMemberChurchRoleId <= 0 ? null : _oChangesMT.FromMemberChurchRoleId
                        //};
                        ////
                        //_context.Add(oMT);

                        try
                        { 
                            ///
                            _oChangesMT.TempMemTypeCodeFrCB = vm.numFromMemberType;
                           // _oChangesMT.TempMemTypeCodeToCB = vm.numTempMemTypeCodeToCB;
                            _oChangesMT.TempMemRankIdFrCB = vm.numFromMemberRank;
                            //_oChangesMT.TempMemRankIdToCB = vm.numTempMemRankIdToCB;
                            _oChangesMT.TempMemStatusIdFrCB = vm.numFromMemberStatus;
                            //_oChangesMT.TempMemStatusIdToCB = vm.numTempMemStatusIdToCB;

                            _oChangesMT.ReqStatusComments = "By " + (vm.serviceTask == 1 ? "requesting congregation" : "applicant");  // applicants have self service portal

                            _oChangesMT.Created = tm;
                            _oChangesMT.LastMod = tm;
                            _oChangesMT.CreatedByUserId = this._oLoggedUser.Id;
                            _oChangesMT.LastModByUserId = this._oLoggedUser.Id;
                            ///
                            _context.Add(_oChangesMT);

                            _context.SaveChanges();
                            isNewREQ = _oChangesMT.Id == 0;

                            //_oChangesMT = oMT; //update cache... 
                            // _oChangesMT.ChurchMemberTransf = __oChangesMT.ChurchMemberTransf;
                            // _oChangesMT.ToChurchBody = __oChangesMT.ToChurchBody;

                            //_oChangesMTbkp.RequestorMember = oMTModel.RequestorMember;
                            //oMTModel.RequestorChurchBody = __oChangesMT.RequestorChurchBody;
                            //oMTModel.FromChurchBody = __oChangesMT.FromChurchBody;
                        }
                        catch (Exception ex)
                        {
                            return Json(new { taskSuccess = false, userMess = "Attempt to save data failed. Please try again." });
                        }
                    }
                    else
                    {
                        //if (_oChangesMT == null)
                        //{// ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
                        //    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
                        //}

                        //if (_oChangesMT.ChurchMemberId == null || _oChangesMT.ToChurchBodyId == null || _oChangesMT.ChurchMemberId == null || _oChangesMT.ToChurchBody == null)
                        //{ //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
                        //    return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
                        //}

                        if (taskIndx == 2 || taskIndx == 5) //... send 
                        {
                            //check for pending.. unClosed requests OR successfulLY closed requests 
                            var currTransf = (from t_cm in _context.ChurchTransfer.AsNoTracking()
                                              .Where(x => x.RequestorChurchBodyId == _oChangesMT.RequestorChurchBodyId && x.ChurchMemberId == _oChangesMT.ChurchMemberId && 
                                                            x.TransferType == _oChangesMT.TransferType && x.Id == _oChangesMT.Id &&
                                              ((taskIndx == 2 && (x.ReqStatus == "N" || x.ReqStatus == "R")) || (taskIndx == 5 && (x.ReqStatus == "R" || x.ReqStatus == "D" || x.ReqStatus == "U")))) 
                                              select t_cm)
                                              .ToList();

                            if (currTransf.Count == 0)
                                return Json(new { taskSuccess = false, userMess = "Member transfer already initiated or done for specified member. Hint: You can only Send draft or recalled requests; or Resubmit recalled, declined or unsuccessful requests." });
                             
                        }

                        ////set... for view in cache 
                        //if (_oChangesMT.ChurchMemberId != null) _oChangesMT.ChurchMemberId = _oChangesMT.ChurchMemberId;
                        //if (_oChangesMT.ToChurchBodyId != null) _oChangesMT.ToChurchBodyId = _oChangesMT.ToChurchBodyId;
                        //if (_oChangesMT.ReasonId != null) _oChangesMT.ReasonId = _oChangesMT.ReasonId;
                        //if (_oChangesMT.TransMessageId != null) _oChangesMT.TransMessageId = _oChangesMT.TransMessageId;
                        //if (_oChangesMT.Comments != null) _oChangesMT.Comments = _oChangesMT.Comments;
                        //if (_oChangesMT.FromChurchPositionId != null) _oChangesMT.FromChurchPositionId = _oChangesMT.FromChurchPositionId <= 0 ? null : _oChangesMT.FromChurchPositionId;
                        //if (_oChangesMT.FromMemberLeaderRoleId != null) _oChangesMT.FromMemberLeaderRoleId = _oChangesMT.FromMemberLeaderRoleId <= 0 ? null : _oChangesMT.FromMemberLeaderRoleId;
                        //// _oChangesMT.RequestDate =  tm;

                        //
                        //already loaded at GET()
                        if (taskIndx == 5)
                        {
                            var oMT = new ChurchTransfer
                            { //create user and init...
                                RequestorMemberId = _oChangesMT.RequestorMemberId,
                                RequestorRoleId = _oChangesMT.RequestorRoleId,
                                RequestorChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                FromChurchBodyId = _oChangesMT.FromChurchBodyId,
                                CurrentScope = _oChangesMT.CurrentScope,
                                ReqStatus = _oChangesMT.ReqStatus,
                                ReqStatusComments = "By " + (vm.serviceTask == 1 ? "requesting congregation" : "applicant"), 
                                StatusModDate = tm,
                                ApprovalStatus = _oChangesMT.ApprovalStatus,
                                TransferType = _oChangesMT.TransferType,
                                //ActualRequestDate  = date request is forwarded to destination congregation
                                // RequestDate = _oChangesMT.RequestDate,
                                Created = tm,  // tm,  // _oChangesMT.Created,
                                LastMod = tm,  //tm,
                                CreatedByUserId = this._oLoggedUser.Id,
                                LastModByUserId = this._oLoggedUser.Id,
                                //
                                ChurchMemberId = _oChangesMT.ChurchMemberId,
                                ToChurchBodyId = _oChangesMT.ToChurchBodyId,
                                TransferReason = _oChangesMT.TransferReason,
                                TransMessageId = _oChangesMT.TransMessageId,
                                CustomTransMessage = _oChangesMT.CustomTransMessage,
                                
                                Comments = _oChangesMT.Comments,
                                //
                                FromChurchRankId = _oChangesMT.FromChurchRankId,
                                FromMemberChurchRoleId = _oChangesMT.FromMemberChurchRoleId,
                                ///
                                TempMemTypeCodeFrCB = vm.numFromMemberType,
                                //TempMemTypeCodeToCB = vm.numTempMemTypeCodeToCB,
                                TempMemRankIdFrCB = vm.numFromMemberRank,
                                //TempMemRankIdToCB = vm.numTempMemRankIdToCB,
                                TempMemStatusIdFrCB = vm.numFromMemberStatus,
                                //TempMemStatusIdToCB = vm.numTempMemStatusIdToCB 

                            };

                            //
                            _context.Add(oMT);

                            try
                            {  
                                _context.SaveChanges();

                                //
                                //oMemTransf = oMT; //update cache... 
                                //                  // _oChangesMT.ChurchMemberTransf = __oChangesMT.ChurchMemberTransf;
                                //                  // _oChangesMT.ToChurchBody = __oChangesMT.ToChurchBody;
                                //_oChangesMT.RequestorChurchMember = __oChangesMT.RequestorChurchMember;
                                //_oChangesMT.RequestorChurchBody = __oChangesMT.RequestorChurchBody;
                                //_oChangesMT.FromChurchBody = __oChangesMT.FromChurchBody;
                            }
                            catch (Exception ex)
                            {
                                return Json(new { taskSuccess = false, userMess = "Attempt to save data failed. Please try again." });
                            }
                        }

                        //_oChangesMT.ChurchMemberTransf = __oChangesMT.ChurchMemberTransf;
                        //_oChangesMT.ToChurchBody = __oChangesMT.ToChurchBody;
                        //_oChangesMT.RequestorChurchMember = __oChangesMT.RequestorChurchMember;
                        //_oChangesMT.RequestorChurchBody = __oChangesMT.RequestorChurchBody;
                        //_oChangesMT.FromChurchBody = __oChangesMT.FromChurchBody;

                        //if (taskIndx != 1 )
                        //{
                        //    if (_oChangesMT.ChurchMemberId == null || _oChangesMT.ToChurchBodyId == null || _oChangesMT.ChurchMemberTransf == null || _oChangesMT.ToChurchBody==null)
                        //    { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
                        //        return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
                        //    }

                        //    ////get member
                        //    //_oChangesMT.ChurchMemberTransf = _context.ChurchMember.Find(_oChangesMT.ChurchMemberId);

                        //    ////get member
                        //    //_oChangesMT.ToChurchBody = _context.ChurchBody.Find(_oChangesMT.ToChurchBodyId);
                        //}
                    }

                    ////get member
                    //if (_oChangesMT.ChurchMemberTransf == null)
                    //    _oChangesMT.ChurchMemberTransf = _context.ChurchMember.Find(_oChangesMT.ChurchMemberId);

                    ////get member
                    //if (_oChangesMT.ToChurchBody == null)
                    //    _oChangesMT.ToChurchBody = _context.ChurchBody.Find(_oChangesMT.ToChurchBodyId);

                }

                else
                {   // get the member transfer data... no updates except the approval/ action parts 
                    //if (_oChangesMT == null) 
                    //    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
                     
                    //if (_oChangesMT.ChurchMemberId == null || _oChangesMT.ToChurchBodyId == null || _oChangesMT.ChurchMemberTransf == null || _oChangesMT.ToChurchBody == null)
                    //{ //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
                    //    return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
                    //}


                    ////get member
                    //if (_oChangesMT.ChurchMemberTransf == null)
                    //    _oChangesMT.ChurchMemberTransf = _context.ChurchMember.Find(_oChangesMT.ChurchMemberId);

                    ////get member
                    //if (_oChangesMT.ToChurchBody == null)
                    //    _oChangesMT.ToChurchBody = _context.ChurchBody.Find(_oChangesMT.ToChurchBodyId);


                    if (taskIndx != 5 && taskIndx >= 3 && taskIndx <= 9)  // resubmit-recall-decline
                    {
                        vm.lsCurrApprActionSteps = _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApproverChurchMember) .Include(t => t.ApprovalAction)
                                                                        //         .Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                                                                        //         .Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchRole) 
                                  .Where(c => c.ApprovalActionId == vm.oCurrApprovalActionId && c.CurrentScope == _oChangesMT.CurrentScope &&
                                  c.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.RequestorChurchBody.AppGlobalOwnerId && c.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.RequestorChurchBody.AppGlobalOwnerId &&   //(c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId || c.ChurchBodyId == _oChangesMT.ToChurchBodyId) && // == 
                                  (c.ApprovalAction.ProcessCode == "TRF_IN" || c.ApprovalAction.ProcessCode == "TRF_OT") && c.ApprovalAction.ProcessSubCode == "MT" && c.ApprovalAction.Status == "A" && c.Status == "A")
                                  .ToList();
                    }
                    else if (taskIndx == 10) // force complete (waive)
                    {
                        vm.lsCurrApprActionSteps = _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApproverChurchMember) .Include(t => t.ApprovalAction)
                                                                                // .Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                                                                                // .Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchRole)                                                                                 
                             .Where(c => c.ApprovalActionId == vm.oCurrApprovalActionId && c.CurrentScope == _oChangesMT.CurrentScope &&
                                   c.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.RequestorChurchBody.AppGlobalOwnerId && c.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.RequestorChurchBody.AppGlobalOwnerId &&   //(c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId || c.ChurchBodyId == _oChangesMT.ToChurchBodyId) && // == 
                                   (c.ApprovalAction.ProcessCode == "TRF_IN" || c.ApprovalAction.ProcessCode == "TRF_OT") && c.ApprovalAction.ProcessSubCode == "MT" && c.ApprovalAction.Status == "A" && c.Status == "A" && c.ActionStepStatus != "A")
                             .ToList();
                    }
                }

                //if (oMemTransf == null)
                //    { //ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
                //    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
                //}

                 
                if (!isNewREQ)   // _oChangesMT.Id > 0)
                {
                    if (taskIndx >= 1 && taskIndx <= 5)  // SAVE-SEND-RECALL-TERMINATE-RESUBMIT
                    {
                        if (!(_oChangesMT.CurrentScope == "I" && (taskIndx >= 1 && taskIndx <= 5))) // || taskIndx == 2 || taskIndx == 5))) / update a 'Draft' or Send 'Draft' Req...  resubmit //recalled, terminated, declined// here!
                            return Json(new { taskSuccess = false, userMess = "Current request allows only 'Save as Draft, Send, Recall, Terminate, and Resubmit request' operations to be performed" });
                        
                        //Update... only Draft
                        if (taskIndx == 1)
                        {
                            if (_oChangesMT.ReqStatus != "N" && _oChangesMT.ReqStatus != "R")  //draft or recalled
                            {  // ModelState.AddModelError(string.Empty, "Request already sent. Update cannot be done. Hint: Try 'Recall request' to make necessary changes.");  return Json(false);  
                                return Json(new { taskSuccess = false, userMess = "Request already sent. Update cannot be done. Hint: Try 'Recall request' to make necessary changes." });
                            } // ViewBag.UserMsg = "Updated member family relations successfully.";
                        }
                        else if (taskIndx == 2)
                        {
                            if (_oChangesMT.ReqStatus != "N" && _oChangesMT.ReqStatus != "R")  //send saved 'draft'
                            { // ModelState.AddModelError(string.Empty, "Request already sent. Hint: Try 'Recall request' to make necessary changes or 'Resubmit' if it's recalled, terminated or declined.");   return Json(false);
                                return Json(new { taskSuccess = false, userMess = "Request already sent. Hint: Try 'Recall request' to make necessary changes or 'Resubmit' if it's terminated or declined." });
                            } // ViewBag.UserMsg = "Transfer request sent successfully.";
                        }
                        else if (taskIndx == 3)
                        {
                            if (!(_oChangesMT.ReqStatus == "P" || _oChangesMT.ReqStatus == "K" || _oChangesMT.ReqStatus == "I"))  //RECALL
                            {// ModelState.AddModelError(string.Empty, "Request must be 'Pending' or 'In Progress' to be recalled. Hint: Try 'Terminate and Resubmit request'.");  return Json(false);
                                return Json(new { taskSuccess = false, userMess = "Request must be 'Pending' or 'In Progress' to be recalled. Hint: Try 'Terminate request' instead'." });
                            } // ViewBag.UserMsg = "Transfer request recalled successfully.";
                        }
                        else if (taskIndx == 4)     /// (_oChangesMT.ReqStatus != "P" && _oChangesMT.ReqStatus != "I") 
                        {
                            if (_oChangesMT.ReqStatus == "X" || _oChangesMT.ReqStatus == "T" || _oChangesMT.ReqStatus == "Y" || _oChangesMT.ReqStatus == "U")  // || _oChangesMT.ReqStatus == "C" || _oChangesMT.ReqStatus == "Z")  //TERMINATE /Terminate should be possible as long as Transfer is not [Success, Unsuccessful, Closed or Archived]
                            {
                                //ModelState.AddModelError(string.Empty, "Request cannot be terminated. Request must be Pending or In Progress.  Hint: Try 'Resubmit request' if request is 'Closed'.");
                                return Json(new { taskSuccess = false, userMess = "Request cannot be terminated. Request must be Pending or In Progress.  Hint: Try 'Resubmit request' if request is 'Unsuccessful'." });
                            }
                        }
                        else if (taskIndx == 5)  //RESUBMIT... recalled, declined ...............|| _oChangesMT.ReqStatus == "X"
                        { //ProcessCode = "TRF_OT" 
                            if (!(_oChangesMT.ReqStatus == "R" || _oChangesMT.ReqStatus == "D" || _oChangesMT.ReqStatus == "U")) //|| (_oChangesMT.ApprovalStatus == "R" || _oChangesMT.ApprovalStatus == "X" || _oChangesMT.ApprovalStatus == "D"))) // resubmit // Recalled, Terminated, Declined //
                            { // ModelState.AddModelError(string.Empty, "Resubmission is possible ONLY for transfer requests that have been Recalled, Terminated or Declined.");   return Json(false); 
                                return Json(new { taskSuccess = false, userMess = "Re-submit is possible ONLY for transfer requests recalled, declined or unsuccessful." });
                            } // ViewBag.UserMsg = "Transfer Request resubmitted successfully.";
                        }
                    }

                    else // ACKNOWLEDGE-APROVE-DECLINE-SUSPEND-FORCE COMPLETE
                    {
                        if (taskIndx == 6)  //ACK
                        {
                            if (!((_oChangesMT.CurrentScope == "I" && _oChangesMT.ReqStatus == "P" && string.IsNullOrEmpty(_oChangesMT.ApprovalStatus)) ||
                                    (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F")) ||
                                    (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "A" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))))   //(_oChangesMT.ReqStatus != "P")  // && _oChangesMT.ReqStatus != "R")  //draft or recalled
                            {  //ModelState.AddModelError(string.Empty, "Request must be 'Pending' to be acknowledged.");  return Json(false);
                                return Json(new { taskSuccess = false, userMess = "Request must be 'Sent' or 'Approved' to be acknowledged." });
                            }
                        }

                        else if (taskIndx >= 7 && taskIndx <= 10)
                        {
                            if (!((_oChangesMT.ReqStatus == "I" && (_oChangesMT.ApprovalStatus == "P" || _oChangesMT.ApprovalStatus == "I" || _oChangesMT.ApprovalStatus == "H")) ||
                                    (_oChangesMT.ReqStatus == "A" || _oChangesMT.ReqStatus == "D")))
                            //(_oChangesMT.ReqStatus != "I")
                            {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
                                return Json(new { taskSuccess = false, userMess = "Request must first be acknowledged or be 'In Progress', 'On Hold' or re-opened for approver to take action. Hint: try to Acknowledge first." });
                            }
                        }

                        else if (taskIndx == 11) // close request   ... 
                        {
                             // if (!((_oChangesMT.ReqStatus == "C" || _oChangesMT.ReqStatus == "X" || _oChangesMT.ReqStatus == "R") && (_oChangesMT.ReqStatus == "U" || _oChangesMT.ReqStatus == "Y"))) // && _oChangesMT.ApprovalStatus == "D")) //not Archived (Z)
                            if (!(_oChangesMT.ReqStatus == "X" || _oChangesMT.ReqStatus == "Y" || _oChangesMT.ReqStatus == "R" || _oChangesMT.ReqStatus == "U" || _oChangesMT.ReqStatus == "D"))  // || _oChangesMT.ReqStatus == "T")) // && _oChangesMT.ApprovalStatus == "D")) //not Archived (Z)
                            {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
                                return Json(new { taskSuccess = false, userMess = "Only processed requests [successful or incomplete] can be closed [archived]." });
                            }
                        }

                        else if (taskIndx == 14) // reopen
                        {
                            if (!(_oChangesMT.WorkSpanStatus == "C")) //not Archived (Z)... _oChangesMT.ReqStatus == "D" || _oChangesMT.ReqStatus == "U" || 
                            {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
                                return Json(new { taskSuccess = false, userMess = "Only closed (successful or incomplete) requests can be re-opened." });   // Hint: Unsuccessful (declined) or terminated requests can be archived... Only unarchived, unsuccessful or closed requests can be re-opened.

                                /// why not... copy the data... DONT CHANGE THE PREVIOUS data!!! and link the 2 requests
                            }
                        }

                        //else if (taskIndx == 15)  // archive... Unarchive --> if successful:- Closed [can be re-open], else Unsuccessful [can be re-open]
                        //{
                        //   // if (!((_oChangesMT.ReqStatus == "C" || _oChangesMT.ReqStatus == "X" || _oChangesMT.ReqStatus == "R") && (_oChangesMT.ReqStatus == "U" || _oChangesMT.ReqStatus == "Y"))) // && _oChangesMT.ApprovalStatus == "D")) //not Archived (Z)
                        //    if (!(_oChangesMT.ReqStatus == "X" || _oChangesMT.ReqStatus == "U" || _oChangesMT.ReqStatus == "C")) // && _oChangesMT.ApprovalStatus == "D")) //not Archived (Z)
                        //    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
                        //        return Json(new { taskSuccess = false, userMess = "Only terminated, incomplete or closed requests can be archived." });
                        //    }
                        //} 
                    }
                }

                //Create the approval process...
                //this approval is for OUTGOING... but internal. EXTERNAL will be triggered by the Acknowledgment separately... NEW Send Req or DRAFTed and now Send Req... jux make sure user does not send twice
                if (taskIndx > 1)
                {
                    if (taskIndx == 2 || taskIndx == 5)
                    {
                         
                        //var oAppProcessList = _context.ApprovalProcess.AsNoTracking().Include(t => t.OwnedByChurchBody).ThenInclude(t=> t.ChurchLevel)  
                        //    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.TargetChurchLevelId == _oChangesMTbkp.RequestorChurchBody.ChurchLevelId &&
                        //        c.ProcessCode == "TRF_OT" && c.ProcessSubCode == "MT" && c.ProcessStatus == "A" && 
                        //        ((c.SharingStatus=="N" && c.OwnedByChurchBodyId== _oChangesMT.ChurchBodyId) || c.SharingStatus != "N"))
                        //    .ToList();

                        //oAppProcessList = oAppProcessList.Where(c =>
                        //                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                        //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                        //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                        //if (oAppProcessList.Count == 0)
                        //    return Json(new { taskSuccess = false, userMess = "Approval flow configurations not done. Transfer process cannot run. Please check with Administrator and try again. Hint: Settings done at higher courts of the church run down /override settings in the lower ranked congregations." });

                        //if (oAppProcessList.Count > 1)
                        //        oAppProcessList = oAppProcessList.OrderBy(c => c.OwnedByChurchBody.ChurchLevel.LevelIndex).ToList();  /// HQ -- Presbytery -- District -- Local

                        //var oAppProcess = oAppProcessList[0]; 

                        //var oAppProStepList = _context.ApprovalProcessStep.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
                        //    .Where(c => c.ChurchBody.AppGlobalOwnerId == oAppProcess.AppGlobalOwnerId && c.ApprovalProcessId == oAppProcess.Id && c.StepStatus == "A")
                        //    .ToList();


                        ////create approval action... at least  one approval level
                        //List<ApprovalAction> oAppActionList = new List<ApprovalAction>();
                        //if (oAppProStepList.Count > 0)
                        //{
                        //    oAppActionList.Add(
                        //        new ApprovalAction
                        //        {
                        //        //Id = 0,
                        //        ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                        //        // ChurchBody = _oChangesMT.RequestorChurchBody,
                        //        ApprovalActionDesc = "Outgoing Member Transfer",
                        //            ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
                        //        ProcessCode = "TRF",
                        //            ProcessSubCode = "MT",
                        //            ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
                        //            ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
                        //            CallerRefId = _oChangesMT.Id,  //reference to the Transfer details
                        //        CurrentScope = "I",
                        //            Status = "A",   //active
                        //        //ActionDate = null,
                        //        //Comments = "",
                        //        ActionRequestDate = tm,
                        //            Created = tm,
                        //            LastMod = tm
                        //        });

                        //    if (oAppActionList.Count > 0)
                        //        _context.Add(oAppActionList.FirstOrDefault());

                        //    //create approval action steps

                        //    // check the approvals config before....                            
                        //    int? oApproverChurchBodyId = null; int? oApproverChurchMemberId = null; int? oApproverChurchRoleId = null; int? oApproverMemberChurchRoleId = null;
                        //    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                        //    {
                        //        var oStepApprover = _context.ApprovalProcessApprover.Where(c => c.ChurchBodyId == oCBid && c.ApprovalProcessStepId == oAppProStep.Id).FirstOrDefault();
                        //        if (oStepApprover == null) 
                        //            return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Please verify and try again." });

                        //        if (oStepApprover.Approver1ChurchBodyId == null && oStepApprover.Approver1ChurchMemberId == null &&
                        //            oStepApprover.Approver2ChurchBodyId == null && oStepApprover.Approver2ChurchMemberId == null)
                        //            return Json(new { taskSuccess = false, userMess = "Approver details not available for configured approval flow step. Please verify and try again." });

                        //        oApproverChurchMemberId = oStepApprover.Approver1ChurchMemberId == null ? oStepApprover.Approver1ChurchMemberId : oStepApprover.Approver2ChurchMemberId;

                        //        var oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver1ChurchBodyId && c.Id == oStepApprover.Approver1ChurchMemberId && c.Status == "A").FirstOrDefault();

                        //        if (oApproverCM == null)
                        //            oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver2ChurchBodyId && c.Id == oStepApprover.Approver2ChurchMemberId && c.Status == "A").FirstOrDefault();

                        //        if (oApproverCM == null)
                        //            return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });
 
                        //        ///
                        //        oApproverChurchBodyId = oApproverCM.ChurchBodyId;
                        //        oApproverChurchMemberId = oApproverCM.Id;
                        //        oApproverChurchRoleId = oStepApprover.Approver1ChurchRoleId != null ? oStepApprover.Approver1ChurchRoleId : oStepApprover.Approver2ChurchRoleId;
                        //        oApproverMemberChurchRoleId = oStepApprover.Approver1MemberChurchRoleId != null ? oStepApprover.Approver1MemberChurchRoleId : oStepApprover.Approver2MemberChurchRoleId;

                        //    }

                        //    List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
                        //    var stepIndexLowest = oAppProStepList[0].StepIndex;
                        //    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                        //    {
                        //        //var oCurrApprover = _context.MemberChurchRole.AsNoTracking()
                        //        //    .Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId && c.ChurchRoleId == oAppProStep.ApproverChurchRoleId && c.IsCurrentRole == true && c.IsLeadRole == true)
                        //        //    .FirstOrDefault();

                        //        //if (oCurrApprover == null)
                        //        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });

                        //        stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
                        //        oAppActionStepList.Add(
                        //            new ApprovalActionStep
                        //            {
                        //                //Id = 0,
                        //                ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                        //                // ChurchBody = _oChangesMT.RequestorChurchBody,
                        //                // MemberChurchRoleId = oCurrApprover != null ? oCurrApprover.Id : (int?)null, 
                        //                ///
                        //                ApproverChurchBodyId = oApproverChurchBodyId,  //  oICurrApprover.Id,
                        //                ApproverChurchMemberId = oApproverChurchMemberId,
                        //                ApproverChurchRoleId = oApproverChurchRoleId,
                        //                ApproverMemberChurchRoleId = oApproverMemberChurchRoleId,
                        //                /// 
                        //                // ApproverMemberChurchRole = oCurrApprover,   
                        //                ActionStepDesc = oAppProStep.StepDesc,
                        //                ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
                        //                ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
                        //                ApprovalProcessStepRefId = oAppProStep.Id,
                        //                CurrentScope = "I",
                        //                StepRequestDate = tm,
                        //                //Comments="",
                        //                //CurrentStep= true,
                        //                //ApproverMemberChurchRoleId = null,  // actual approver @approval
                        //                //ActionDate = tm,
                        //                Status = "A", //Active
                        //                Created = tm,
                        //                LastMod = tm,
                        //            //
                        //            ApprovalActionId = oAppActionList.FirstOrDefault().Id,
                        //            // ApprovalAction = oAppActionList.FirstOrDefault()
                        //        });
                        //    }

                        //    foreach (ApprovalActionStep oAS in oAppActionStepList)
                        //    {
                        //        oAS.IsCurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled   // if (oAS.CurrentStep) oCurrApprovers.Add(oAS);
                        //        _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }                                                  
                        //    }


                            _oChangesMT.ReqStatus = "P"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                            _oChangesMT.RequestDate = tm;
                            _oChangesMT.StatusModDate = tm;


                        //_oChangesMT.ActualRequestDate = null;  // after approval or acknowledgment from the FROM congregation
                        //_oChangesMT.RequireApproval = oAppActionList.Count > 0; // _oMemTransf_VM.RequireApproval,
                        //_oChangesMT.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
                        //
                        //_oChangesMT.IApprovalActionId = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oMemTransf_VM.IApprovalActionId,
                        // _oChangesMT.IApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction,
                                                                                                                                   //_oChangesMTbkp.IApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;
                        //_oChangesMT.LastMod = tm;  
                        //_oChangesMT.LastModByUserId = this._oLoggedUser.Id;  

                    // }
                    }

                    else if (taskIndx == 3 || taskIndx == 4)   //Recall, Terminate
                    {
                        string strApprovalStatus = null;
                        var oActionStepList = vm.lsCurrApprActionSteps;  /// setup above...

                        //create approval action
                        if (oActionStepList.Count > 0)
                        {
                            if (oActionStepList[0].ApprovalAction != null)
                            {
                                var oAA = oActionStepList[0].ApprovalAction;
                             
                                foreach (ApprovalActionStep oAAStep in oActionStepList)
                                {
                                    oAAStep.ActionStepStatus = taskIndx == 3 ? "R" : "X";
                                    oAAStep.Comments = vm.strApproverComment;  //could also be reason from the user/applicant
                                    oAAStep.ActionDate = tm;

                                    oAAStep.LastMod = tm;
                                    oAAStep.LastModByUserId = this._oLoggedUser.Id;

                                    //add appproval notifiers
                                    // oCurrApprovers.Add(oAAStep);

                                    if (oAAStep.ApprovalAction != null) oAAStep.ApprovalAction = null;
                                    if (oAAStep.ApproverChurchMember != null) oAAStep.ApproverChurchMember = null;

                                    _context.Update(oAAStep);
                                }
                                                            
                                strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, taskIndx == 3 ? "R" : "X");   // this.GetApprovalActionStatus(oActionStepList, _oChangesMT.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
                                oAA.ActionStatus = strApprovalStatus; // this.GetApprovalActionStatus(oActionStepList);   //this.GetApprovalActionStatus(oActionStepList, _oChangesMT.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
                                oAA.Comments = vm.strApproverComment;   //could also be reason from the user/applicant
                                oAA.LastActionDate = tm;
                                oAA.Status = "D"; // Active-Deactive-Closed
                                oAA.Comments = "By " + (vm.serviceTask == 1 ? "requesting congregation" : "applicant"); // "Request " + GetRequestStatusDesc(oAA.Status).ToLower() + " by " + (vm.numTransferDxn == 1 ? "requesting congregation" : "applicant");
                                oAA.LastMod = tm;
                                oAA.LastModByUserId = this._oLoggedUser.Id;

                                _context.Update(oAA);
                            }                         
                        }

                        _oChangesMT.ReqStatus = taskIndx == 3 ? "R" : "X"; // GetApprovalActionStatus(oActionStepList, _oChangesMT.CurrentScope == "I" ? 1 : 2, taskIndx == 3 ? "R" : "X"); // taskIndx == 3 ? "R" : "X"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                        _oChangesMT.RequestDate = tm;
                        _oChangesMT.StatusModDate = tm;
                        _oChangesMT.ApprovalStatus = strApprovalStatus; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
                        _oChangesMT.ReqStatusComments = "By " + (vm.serviceTask == 1 ? "requesting congregation" : "applicant");
                        _oChangesMT.ApprovalStatusComments = "By " + (vm.serviceTask == 1 ? "requesting congregation" : "applicant");

                        // _oChangesMT.ReqStatus = "U"; // Unsuccessful
                        //if (strApprovalStatus == null)
                        //{
                        //    _oChangesMT.IApprovalActionId = null;
                        //    _oChangesMT.IApprovalAction = null;
                        //}

                        //// update the AA ...
                        //var oAppAction_Curr = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                        //            .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId &&
                        //                (c.ProcessCode == "TRF_OT" || c.ProcessCode == "TRF_IN") && c.ProcessSubCode == _oChangesMT.TransferType && c.CallerRefId == _oChangesMT.Id && c.Status == "A")
                        //            .FirstOrDefault();
                        //if (oAppAction_Curr != null)
                        //{
                        //    oAppAction_Curr.Status = "D"; // Active-Deactive-Closed
                        //    oAppAction_Curr.Comments = "Request " + GetRequestStatusDesc(_oChangesMT.ReqStatus).ToLower(); 
                        //    oAppAction_Curr.LastMod = tm;
                        //    oAppAction_Curr.LastModByUserId = this._oLoggedUser.Id;

                        //    _context.Update(oAppAction_Curr);
                        //} 
                    }


                    else if (taskIndx == 6)
                    {
                        if ((_oChangesMT.CurrentScope == "I" && _oChangesMT.ReqStatus == "P" && string.IsNullOrEmpty(_oChangesMT.ApprovalStatus)) ||  //not set yet... _
                                        (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F")))  // FrCB done approving, transfer done, --FrCB acknowledge ... to Close request
                        {  //first... @Pending... //@In Progress... acknowledge --> Pending of ToChurchBody approvers
                            //_oChangesMT.ReqStatus = "I"; // Leave all approval action/step in Pending until approver works on ...

                            if (_oChangesMT.CurrentScope == "I")  // acknowledged at the requesting CB...
                            {
                                /// check No Approval action pending or in progress
                                var oAppActionList = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && 
                                        c.ProcessCode == "TRF_OT" && c.ProcessSubCode == _oChangesMT.TransferType && c.CallerRefId== _oChangesMT.Id && c.Status == "A")
                                    .ToList();
                                if (oAppActionList .Count > 0)
                                    return Json(new { taskSuccess = false, userMess = "Transfer request specified [" + vm.strTransfMemberDesc +  "] is already queued for approval and still active." });

                                var oAppProcessList = _context.ApprovalProcess.AsNoTracking().Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.TargetChurchLevelId == _oChangesMTbkp.RequestorChurchBody.ChurchLevelId &&
                                        c.ProcessCode == "TRF_OT" && c.ProcessSubCode == "MT" && c.ProcessStatus == "A" &&
                                        ((c.SharingStatus == "N" && c.OwnedByChurchBodyId == _oChangesMT.ChurchBodyId) || c.SharingStatus != "N"))
                                    .ToList();

                                oAppProcessList = oAppProcessList.Where(c =>
                                                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                                if (oAppProcessList.Count == 0)
                                    return Json(new { taskSuccess = false, userMess = "Approval flow configurations not done. Transfer process cannot run. Please check with Administrator and try again. Hint: Settings done at higher courts of the church run down /override settings in the lower ranked congregations." });

                                if (oAppProcessList.Count > 1)
                                    oAppProcessList = oAppProcessList.OrderBy(c => c.OwnedByChurchBody.ChurchLevel.LevelIndex).ToList();  /// HQ -- Presbytery -- District -- Local

                                var oAppProcess = oAppProcessList[0];

                                var oAppProStepList = _context.ApprovalProcessStep.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
                                    .Where(c => c.ChurchBody.AppGlobalOwnerId == oAppProcess.AppGlobalOwnerId && c.ApprovalProcessId == oAppProcess.Id && c.StepStatus == "A")
                                    .ToList();


                                //create approval action... at least  one approval level
                                // List<ApprovalAction> oAppActionList = new List<ApprovalAction>();

                                ApprovalAction oAppAction_OT = null; // 
                                if (oAppProStepList.Count > 0)
                                {
                                    
                                       oAppAction_OT = new ApprovalAction
                                        {
                                            //Id = 0,
                                            AppGlobalOwnerId = _oChangesMT.AppGlobalOwnerId,
                                            ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                            // ChurchBody = _oChangesMT.RequestorChurchBody,
                                            ApprovalActionDesc = "Outgoing Member Transfer",
                                            ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
                                            ProcessCode = "TRF_OT",
                                            ProcessSubCode = "MT",
                                            ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
                                           // ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
                                            CallerRefId = _oChangesMT.Id,  //reference to the Transfer details
                                            CurrentScope = "I",
                                            Status = "A",   //Active, Deactive, Close
                                                            //ActionDate = null,
                                                            //Comments = "",
                                            ActionRequestDate = tm,
                                            Created = tm,
                                           CreatedByUserId = this._oLoggedUser.Id,
                                            LastMod = tm,
                                           LastModByUserId = this._oLoggedUser.Id
                                       };

                                    //if (oAppActionList.Count > 0)
                                    //    _context.Add(oAppActionList.FirstOrDefault());


                                    if (oAppAction_OT.ApprovalProcess != null) oAppAction_OT.ApprovalProcess = null;
                                    _context.Add(oAppAction_OT);                                    
                                    _context.SaveChanges();

                                    //create approval action steps

                                    // check the approvals config before....                            
                                    int ? oApproverChurchBodyId = null; int? oApproverChurchMemberId = null; int? oApproverChurchRoleId = null; int? oApproverMemberChurchRoleId = null;
                                    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                                    {
                                        var oStepApprover = _context.ApprovalProcessApprover.Where(c => c.ChurchBodyId == oCBid && c.ApprovalProcessStepId == oAppProStep.Id).FirstOrDefault();
                                        if (oStepApprover == null)
                                            return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Please verify and try again." });

                                        if (oStepApprover.Approver1ChurchBodyId == null && oStepApprover.Approver1ChurchMemberId == null &&
                                            oStepApprover.Approver2ChurchBodyId == null && oStepApprover.Approver2ChurchMemberId == null)
                                            return Json(new { taskSuccess = false, userMess = "Approver details not available for configured approval flow step. Please verify and try again." });

                                        oApproverChurchMemberId = oStepApprover.Approver1ChurchMemberId == null ? oStepApprover.Approver1ChurchMemberId : oStepApprover.Approver2ChurchMemberId;

                                        var oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver1ChurchBodyId && c.Id == oStepApprover.Approver1ChurchMemberId && c.Status == "A").FirstOrDefault();

                                        if (oApproverCM == null)
                                            oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver2ChurchBodyId && c.Id == oStepApprover.Approver2ChurchMemberId && c.Status == "A").FirstOrDefault();

                                        if (oApproverCM == null)
                                            return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });

                                        ///
                                        oApproverChurchBodyId = oApproverCM.ChurchBodyId;
                                        oApproverChurchMemberId = oApproverCM.Id;
                                        oApproverChurchRoleId = oStepApprover.Approver1ChurchRoleId != null ? oStepApprover.Approver1ChurchRoleId : oStepApprover.Approver2ChurchRoleId;
                                        oApproverMemberChurchRoleId = oStepApprover.Approver1MemberChurchRoleId != null ? oStepApprover.Approver1MemberChurchRoleId : oStepApprover.Approver2MemberChurchRoleId;

                                    }

                                    List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
                                    var stepIndexLowest = oAppProStepList[0].StepIndex;
                                    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                                    {
                                        //var oCurrApprover = _context.MemberChurchRole.AsNoTracking()
                                        //    .Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId && c.ChurchRoleId == oAppProStep.ApproverChurchRoleId && c.IsCurrentRole == true && c.IsLeadRole == true)
                                        //    .FirstOrDefault();

                                        //if (oCurrApprover == null)
                                        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });

                                        stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
                                        oAppActionStepList.Add(
                                            new ApprovalActionStep
                                            {
                                                //Id = 0,
                                                AppGlobalOwnerId = _oChangesMT.AppGlobalOwnerId,
                                                ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                                // ChurchBody = _oChangesMT.RequestorChurchBody,
                                                // MemberChurchRoleId = oCurrApprover != null ? oCurrApprover.Id : (int?)null, 
                                                
                                                ApprovalActionId = oAppAction_OT.Id,  // oAppActionList.FirstOrDefault().Id,
                                                // ApprovalAction = oAppActionList.FirstOrDefault()
                                                ///
                                                ApproverChurchBodyId = oApproverChurchBodyId,  //  oICurrApprover.Id,
                                                ApproverChurchMemberId = oApproverChurchMemberId,
                                                ApproverChurchRoleId = oApproverChurchRoleId,
                                                ApproverMemberChurchRoleId = oApproverMemberChurchRoleId,
                                                /// 
                                                // ApproverMemberChurchRole = oCurrApprover,   
                                                ActionStepDesc = oAppProStep.StepDesc,
                                                ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
                                                ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
                                                ApprovalProcessStepRefId = oAppProStep.Id,
                                                CurrentScope = "I",
                                                StepRequestDate = tm,
                                                //Comments="",
                                                //CurrentStep= true,
                                                //ApproverMemberChurchRoleId = null,  // actual approver @approval
                                                //ActionDate = tm,
                                                Status = "A", //Active
                                                Created = tm,
                                                CreatedByUserId = this._oLoggedUser.Id,
                                                LastMod = tm,
                                                LastModByUserId = this._oLoggedUser.Id,
                                                //
                                                
                                            });
                                    }

                                    foreach (ApprovalActionStep oAS in oAppActionStepList)
                                    {
                                        oAS.IsCurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled   // if (oAS.CurrentStep) oCurrApprovers.Add(oAS);
                                        if (oAS.ApproverChurchMember != null) oAS.ApproverChurchMember = null;
                                        _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }                                                  
                                    }


                                    _oChangesMT.ToReceivedDate = null;
                                    _oChangesMT.FrReceivedDate = tm;
                                    _oChangesMT.ReqStatus = "K"; //In Progress...  Approval Pending 
                                    _oChangesMT.StatusModDate = tm;

                                    //    _oChangesMT.ReqStatus = "P"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                                    //_oChangesMT.RequestDate = tm;

                                    //_oChangesMT.ActualRequestDate = null;  // after approval or acknowledgment from the FROM congregation
                                    _oChangesMT.RequireApproval = oAppAction_OT != null;  // oAppActionList.Count > 0; // _oMemTransf_VM.RequireApproval,
                                _oChangesMT.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
                                //
                                _oChangesMT.IApprovalActionId = oAppAction_OT != null ? oAppAction_OT.Id : (int?)null;  ///  oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oMemTransf_VM.IApprovalActionId,
                                                                                                                        ///_oChangesMT.IApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction,
                                _oChangesMTbkp.IApprovalAction = oAppAction_OT != null ? oAppAction_OT : null;  // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;

                                _oChangesMT.LastMod = tm;
                                _oChangesMT.LastModByUserId = this._oLoggedUser.Id;


                                    ////notify the approvers now... at the FrCB
                                    //var oAASList = _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApprovalAction)
                                    //                .Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                                    //                .Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId &&                      //(c.CurrentScope == "I" || (c.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P")) && 
                                    //                c.ApprovalActionId == _oChangesMT.IApprovalActionId && c.IsCurrentStep == true).ToList();

                                    //oCurrApprovers.AddRange(oAASList);


                                    ///// add  only the current approver... may be concurrent
                                    //oCurrApprovers.AddRange(oAppActionStepList.Where(x=> x.IsCurrentStep==true).ToList());

                                } 
                            }

                            else if (_oChangesMT.CurrentScope == "E")   // &&   // acknowledged at the target CB... (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))
                            {

                                /// check No Approval action pending or in progress
                                var oAppActionList = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId &&
                                        c.ProcessCode == "TRF_IN" && c.ProcessSubCode == _oChangesMT.TransferType && c.CallerRefId == _oChangesMT.Id && c.Status == "A")
                                    .ToList();
                                if (oAppActionList.Count > 0)
                                    return Json(new { taskSuccess = false, userMess = "Transfer request specified [" + vm.strTransfMemberDesc + "] is already queued for approval and still active." });

                                ///// can be inherited from --- higher CBs but not approvers MUST be localized @Setup : note the target CB Level
                                //var oAppProStepList = _context.ApprovalProcessStep.Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
                                //    .Where(c => c.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.ToChurchBody.AppGlobalOwnerId && c.ApprovalProcess.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.ToChurchBody.AppGlobalOwnerId &&
                                //             c.ApprovalProcess.TargetChurchLevelId == _oChangesMTbkp.ToChurchBody.ChurchLevelId &&
                                //        c.ApprovalProcess.ProcessCode == "TRF_IN" && c.ApprovalProcess.ProcessSubCode == "MT" && c.ApprovalProcess.ProcessStatus == "A" && c.StepStatus == "A")
                                //    .ToList();


                                var oAppProcessList = _context.ApprovalProcess.AsNoTracking().Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.TargetChurchLevelId == _oChangesMTbkp.ToChurchBody.ChurchLevelId &&
                                        c.ProcessCode == "TRF_IN" && c.ProcessSubCode == "MT" && c.ProcessStatus == "A" && c.ProcessStatus == "A" &&
                                        ((c.SharingStatus == "N" && c.OwnedByChurchBodyId == _oChangesMT.ChurchBodyId) || c.SharingStatus != "N"))
                                    .ToList();

                                oAppProcessList = oAppProcessList.Where(c =>
                                                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                                if (oAppProcessList.Count == 0)
                                    return Json(new { taskSuccess = false, userMess = "Approval flow configurations not done yet. Transfer process cannot run. Please check with Administrator and try again. Hint: Settings done at higher courts of the church run down /override settings in the lower ranked congregations." });

                                if (oAppProcessList.Count > 1)
                                    oAppProcessList = oAppProcessList.OrderBy(c => c.OwnedByChurchBody.ChurchLevel.LevelIndex).ToList();  /// HQ -- Presbytery -- District -- Local

                                var oAppProcess = oAppProcessList[0];

                                var oAppProStepList = _context.ApprovalProcessStep.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
                                    .Where(c => c.ChurchBody.AppGlobalOwnerId == oAppProcess.AppGlobalOwnerId && c.ApprovalProcessId == oAppProcess.Id && c.StepStatus == "A")
                                    .ToList();


                                //create approval action
                                // List<ApprovalAction> oAppActionList = new List<ApprovalAction>();

                                ApprovalAction oAppAction_IN = null; // new ApprovalAction();
                                if (oAppProStepList.Count > 0)
                                {
                                    oAppAction_IN = new ApprovalAction
                                    {
                                        //Id = 0,
                                        AppGlobalOwnerId = _oChangesMT.AppGlobalOwnerId,
                                        ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                        // ChurchBody = _oChangesMT.RequestorChurchBody,
                                        ApprovalActionDesc = "Incoming Member Transfer",
                                        ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
                                        ProcessCode = "TRF_IN",
                                        ProcessSubCode = "MT",
                                        ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
                                       // ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
                                        CallerRefId = _oChangesMT.Id,  //reference to the Transfer details
                                        CurrentScope = "E",
                                        Status = "A",
                                        //ActionDate = null,
                                        //Comments = "",
                                        ActionRequestDate = tm,
                                        Created = tm,
                                        CreatedByUserId = this._oLoggedUser.Id,
                                        LastMod = tm,
                                        LastModByUserId = this._oLoggedUser.Id
                                    };

                                    //oAppActionList.Add( );

                                    //if (oAppActionList.Count > 0)
                                    //    _context.Add(oAppActionList.FirstOrDefault());

                                    _context.Add(oAppAction_IN);

                                    // check the approvals config before....
                                    int? oApproverChurchBodyId = null; int? oApproverChurchMemberId = null; int? oApproverChurchRoleId = null; int? oApproverMemberChurchRoleId = null;
                                    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                                    {
                                        //var oICurrApprover = _context.MemberChurchRole.Where(c => c.ChurchBodyId == _oChangesMT.ToChurchBodyId && c.ChurchRoleId == oAppProStep.ApproverChurchRoleId &&
                                        //                                                    c.IsCurrentRole == true && c.IsLeadRole == true).FirstOrDefault();

                                        //if (oICurrApprover == null)
                                        //{ // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");  return Json(false);
                                        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
                                        //}

                                        var oStepApprover = _context.ApprovalProcessApprover.Where(c => c.ChurchBodyId == oCBid && c.ApprovalProcessStepId == oAppProStep.Id).FirstOrDefault();
                                        if (oStepApprover == null) return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Please verify and try again." });

                                        if (oStepApprover.Approver1ChurchBodyId == null && oStepApprover.Approver1ChurchMemberId == null &&
                                            oStepApprover.Approver2ChurchBodyId == null && oStepApprover.Approver2ChurchMemberId == null)
                                            return Json(new { taskSuccess = false, userMess = "Approver details not available for configured approval flow step. Please verify and try again." });

                                        oApproverChurchMemberId = oStepApprover.Approver1ChurchMemberId == null ? oStepApprover.Approver1ChurchMemberId : oStepApprover.Approver2ChurchMemberId;


                                        var oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver1ChurchBodyId && c.Id == oStepApprover.Approver1ChurchMemberId && c.Status == "A").FirstOrDefault();

                                        if (oApproverCM == null)
                                            oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver2ChurchBodyId && c.Id == oStepApprover.Approver2ChurchMemberId && c.Status == "A").FirstOrDefault();

                                        if (oApproverCM == null)
                                            return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });


                                        // the role is great BUT that cannot prevent the Member from approving the request....
                                        ///
                                        //var oApproverMCR = _context.MemberChurchRole.Where(c => c.ChurchBodyId == oAppProStep.Approver1ChurchBodyId && c.Id == oAppProStep.Approver1MemberChurchRoleId && c.IsCurrentRole == true).FirstOrDefault();

                                        //if (oApproverMCR == null)
                                        //    oApproverMCR = _context.MemberChurchRole.Where(c => c.ChurchBodyId == oAppProStep.Approver1ChurchBodyId && c.ChurchRoleId == oAppProStep.Approver1ChurchRoleId && c.ChurchMemberId == oAppProStep.Approver1ChurchMemberId && c.IsCurrentRole==true).FirstOrDefault();

                                        //if (oApproverMCR == null)
                                        //    oApproverMCR = _context.MemberChurchRole.Where(c => c.ChurchBodyId == oAppProStep.Approver2ChurchBodyId && c.Id == oAppProStep.Approver2MemberChurchRoleId).FirstOrDefault();

                                        //if (oApproverCM == null)
                                        //    return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });

                                        ///
                                        oApproverChurchBodyId = oApproverCM.ChurchBodyId;
                                        oApproverChurchMemberId = oApproverCM.Id;
                                        oApproverChurchRoleId = oStepApprover.Approver1ChurchRoleId != null ? oStepApprover.Approver1ChurchRoleId : oStepApprover.Approver2ChurchRoleId;
                                        oApproverMemberChurchRoleId = oStepApprover.Approver1MemberChurchRoleId != null ? oStepApprover.Approver1MemberChurchRoleId : oStepApprover.Approver2MemberChurchRoleId;

                                    }


                                    //create approval action steps
                                    List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
                                    var stepIndexLowest = 1;
                                    if (oAppProStepList.Count > 0) stepIndexLowest = oAppProStepList[0].StepIndex;
                                    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                                    {
                                        //var oICurrApprover = _context.MemberChurchRole.Where(c => c.ChurchBodyId == _oChangesMT.ToChurchBodyId && c.ChurchRoleId == oAppProStep.ApproverChurchRoleId &&
                                        //                                                    c.IsCurrentRole == true && c.IsLeadRole == true).FirstOrDefault();

                                        //if (oICurrApprover == null)
                                        //{ // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");  return Json(false);
                                        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
                                        //}

                                        //if (oAppProStep.Approver1ChurchBodyId == null && oAppProStep.Approver1ChurchMemberId == null &&
                                        //    oAppProStep.Approver2ChurchBodyId == null && oAppProStep.Approver2ChurchMemberId == null)
                                        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Please verify and try again." });

                                        //oApproverChurchMemberId = oAppProStep.Approver1ChurchMemberId == null ? oAppProStep.Approver1ChurchMemberId : oAppProStep.Approver2ChurchMemberId;

                                        //var oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oAppProStep.Approver1ChurchBodyId && c.Id == oAppProStep.Approver1ChurchMemberId && c.Status == "A").FirstOrDefault();

                                        //if (oApproverCM == null)
                                        //    oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oAppProStep.Approver2ChurchBodyId && c.Id == oAppProStep.Approver2ChurchMemberId && c.Status == "A").FirstOrDefault();

                                        //if (oApproverCM == null)
                                        //    return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });

                                        stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
                                        oAppActionStepList.Add(
                                            new ApprovalActionStep
                                            {
                                                //Id = 0,
                                                AppGlobalOwnerId = _oChangesMT.AppGlobalOwnerId,
                                                ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                                //ChurchBody = _oChangesMT.RequestorChurchBody,
                                                ///
                                                ApprovalActionId = oAppAction_IN.Id,  // oAppActionList.FirstOrDefault().Id,
                                                //ApprovalAction = oAppActionList.FirstOrDefault()
                                                ///
                                                ApproverChurchBodyId = oApproverChurchBodyId,  //  oICurrApprover.Id,
                                                ApproverChurchMemberId = oApproverChurchMemberId,
                                                ApproverChurchRoleId = oApproverChurchRoleId,
                                                ApproverMemberChurchRoleId = oApproverMemberChurchRoleId,
                                                ///
                                                // ApproverMemberChurchRole = oICurrApprover, 
                                                ActionStepDesc = oAppProStep.StepDesc,
                                                ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
                                                ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
                                                ApprovalProcessStepRefId = oAppProStep.Id,
                                                CurrentScope = "E",
                                                StepRequestDate = tm,
                                                // Comments="",
                                                // CurrentStep= true,
                                                // ApproverMemberChurchRoleId  = vm.oCurrLoggedMemberId,  // actual approver
                                                // ActionBy = vm.oCurrLoggedMember,
                                                // ActionDate = tm,
                                                Status = "A", //Active
                                                Created = tm,
                                                LastMod = tm,
                                                LastModByUserId = this._oLoggedUser.Id

                                            }); 
                                    }

                                    foreach (ApprovalActionStep oAS in oAppActionStepList)
                                    {
                                        oAS.IsCurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled 
                                        if (oAS.ApproverChurchMember != null) oAS.ApproverChurchMember = null;
                                        _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }
                                    }


                                    _oChangesMT.ToReceivedDate = tm; // _oChangesMT.Status = "I"; //Approval Pending

                                    //_oChangesMT.ReqStatus = "I"; //keep as it is == in prrogres. to the applicant.. Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                                    // _oChangesMT.RequestDate = tm;
                                    //_oChangesMT.ToRequestDate = tm;   // after approval or acknowledgment from the FROM congregation

                                    _oChangesMT.RequireApproval = oAppAction_IN != null; // oAppActionList.Count > 0; // _oMemTransf_VM.RequireApproval,
                                    _oChangesMT.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
                                    // _oChangesMT.CurrentScope = "E";
                                    _oChangesMT.EApprovalActionId = oAppAction_IN != null ? oAppAction_IN.Id : (int?)null; //  oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oMemTransf_VM.IApprovalActionId,
                                                                                                                                                 // _oChangesMT.EApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction, 
                                     // _oChangesMTbkp.EApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction, 

                                    _oChangesMTbkp.EApprovalAction = oAppAction_IN != null ? oAppAction_IN : null;


                                    ////notify the approvers now... 
                                    //var oAASList = _context.ApprovalActionStep.AsNoTracking().Include(t => t.ApproverChurchMember)  //.Include(t => t.ApprovalAction)
                                    //                      // .Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                                    //                .Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId &&    //     (c.CurrentScope == "E" && _oChangesMT.ApprovalStatus != "P" && 
                                    //                c.ApprovalActionId == _oChangesMT.EApprovalActionId && c.IsCurrentStep == true).ToList();

                                    //oCurrApprovers.AddRange(oAASList);


                                    ///// add  only the current approver... may be concurrent
                                    //oCurrApprovers.AddRange(oAppActionStepList.Where(x => x.IsCurrentStep == true).ToList());

                                } 
                            }
                        }

                        else if  (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "A" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))  // Force complete
                        {
                            //perform the transfer ...and then Close the request after 'success'.
                           // _oChangesMT.TransferDate = tm;    ///same date to be used as the Enrollment date /Departure date for member

                            //if (this.PerformMemberTransfer(_context, _oChangesMT, vm.oChurchBody.Id) == false)  // add _bkp to get other details
                            //{  //reverse action .....    ModelState.AddModelError(string.Empty, "Member transfer unsuccessful.");
                            //    return Json(new { taskSuccess = false, userMess = "Member transfer unsuccessful. Please try to resubmit or contact administrator and try again" });
                            //}

                            // update the member data pool
                            isTransferDone_MemMoved = true;

                            ////notify all appprovers  ... Approvers receive Notification ONLY at their Action
                            //var oAASList = _context.ApprovalActionStep   //.Include(t => t.ApproverChurchMember)  /// .Include(t => t.ApprovalAction)
                            /////.Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                            //.Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId &&
                            //           (c.ApprovalActionId == _oChangesMT.IApprovalActionId || c.ApprovalActionId == _oChangesMT.EApprovalActionId)).ToList();

                            //oCurrApprovers.AddRange(oAASList);

                            //update... you can reverse in case... WHEN member reports at To-CB... Member Account status will move from [In-Transit] to [Active] then the counts will pick else include In-Transit as members already ---
                            _oChangesMT.ReqStatus = "Y"; //Yes... Transferred done....T ... "C";     // closed... From or requaesting congregation MUST acknowledge                         
                            _oChangesMT.StatusModDate = tm;
                            //_oChangesMT.ReqStatus = "T"; //Transfer done but member has not moved physically yet! ...in Transit state... not here, not there either! but actually data moved [to go back to Fr-CB.. new request in reverse order]
                            _oChangesMT.ReqStatusComments = "Member transfer complete. Member has joined new congregation [in-person]. Church statistics updated accordingly";
                        }
                    }

                    else if (taskIndx >= 7 && taskIndx <= 9)  // Approve-Decline-Suspend-Force Complete    && vm.lsCurrApprActionSteps.Count > 0)
                    {
                        /// Approve -- resStat = I, H
                        /// Approve -- resStat = I, H
                        /// Approve -- resStat = I, H



                        string strApprovalStatus = null;
                        if (vm.oCurrApprovalActionStep == null || vm.oCurrApprovalAction == null || vm.lsCurrApprActionSteps.Count == 0)
                        {  //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again."); return Json(false);
                            return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
                        }

                        //update approval action step

                        //var oAAStepList = _context.ApprovalActionStep.Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                        //    .Where(c => c.Id == vm.currApprovalActionId).ToList();

                        //var oAAStep = _context.ApprovalActionStep.Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t=>t.ContactInfo)
                        //    .Where(c => c.Id == vm.currApprovalActionStepId).FirstOrDefault() ; // .Find(vm.currApprovalActionStepId);  //.oCurrApprovalActionStep.Id);
                         
                        if (vm.lsCurrApprActionSteps.Count > 0)
                        {
                            if (vm.lsCurrApprActionSteps[0].ApprovalAction != null)
                            {
                                var oActionStepList = vm.lsCurrApprActionSteps;
                                var _oAA = vm.lsCurrApprActionSteps[0].ApprovalAction;

                                var oAAStep = oActionStepList.Where(c => c.ChurchBodyId == _oAA.ChurchBodyId && c.Id == vm.oCurrApprovalActionStepId).FirstOrDefault();
                                if (oAAStep == null)  
                                    return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
                                
                                //add appproval notifiers  ... @by now... the approver MUST av logged in... oCM_Logged -- setup above
                                //oCurrApprovers.Add(oAAStep);

                                //var oApprover = _context.MemberChurchRole.Where(c => ((_oChangesMT.CurrentScope == "E" && c.ChurchBodyId == _oChangesMT.ToChurchBodyId) || 
                                //                                                    (_oChangesMT.CurrentScope == "I" && c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId)) &&
                                //                                                    c.ChurchMemberId == oCM_Logged.Id && c.IsCurrentRole == true && c.IsLeadRole == true).FirstOrDefault();


                                //if (oApprover == null)
                                //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });

                                oAAStep.ActionStepStatus = taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : taskIndx == 9 ? "H" : oAAStep.ActionStepStatus;
                                if (taskIndx == 8 || taskIndx == 9)
                                {
                                    oAAStep.Comments = apprComment;   //could also be reason from the user/applicant
                                }

                                oAAStep.ActionDate = tm;
                                oAAStep.ApproverChurchMemberId = oCM_Logged.Id;

                                // oAAStep.ApproverMemberChurchRoleId = oApprover.Id;
                                // oAAStep.ApproverMemberChurchRole = oApprover;
                                                       
                                //if step Approved.. get next step and notify approvers...
                                if (oAAStep.ActionStepStatus == "A")
                                {
                                    /// notify the APPROVER's action taken
                                    oCurrApprovers.Add(oAAStep);

                                    var oNextAASteps = oActionStepList.Where(c => c.ApprovalStepIndex == oAAStep.ApprovalStepIndex + 1).ToList();
                                    if (oNextAASteps.Count > 0)
                                    {
                                        oAAStep.IsCurrentStep = false;
                                        foreach (var oNextStep in oNextAASteps)  /// concurrent approvers > 1  ... oNextStep != oAAStep
                                        {
                                            oNextStep.IsCurrentStep = true;
                                           
                                            ///
                                            /// notify the NEXT APPROVER's action taken
                                            oCurrApprovers.Add(oNextStep);

                                            _context.Update(oNextStep);
                                        }
                                    }
                                } 
                                                                
                                //
                                vm.oCurrApprovalActionStep = oAAStep;
                                for (int i = 0; i < oActionStepList.Count; i++)
                                {
                                    if (oActionStepList[i].Id == oAAStep.Id)
                                    {
                                        oActionStepList[i] = oAAStep; break;
                                    }
                                }

                                oAAStep.LastMod = tm;
                                oAAStep.LastModByUserId = this._oLoggedUser.Id;

                                _context.Update(oAAStep);


                                //try
                                //{  // _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                                //    _context.Update(oAAStep);
                                //}
                                //catch (Exception ex)
                                //{ return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." }); }


                                ////approval action       
                                //var oAA = _context.ApprovalAction.Find(vm.oCurrApprovalActionId); //oCurrApprovalAction.Id);
                                //if (oAA == null)
                                //{ //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again.");  return Json(false);
                                //    return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
                                //}


                                /// update the AA ... 
                                strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : "H"); // taskIndx == 9 ? "H");
                                _oAA.ActionStatus = strApprovalStatus; // this.GetApprovalActionStatus(oActionStepList);  //strApprovalStatus;
                                _oAA.Comments = vm.strApproverComment;   //could also be reason from the user/applicant
                                _oAA.LastActionDate = tm;
                                 
                                //member transfer
                                // strApprovalStatus = this.GetApprovalActionStatus(oActionStepList);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
                                _oChangesMT.ReqStatus = this.GetApprovalActionStatus(oActionStepList, _oChangesMT.CurrentScope == "I" ? 1 : 2, taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : "H"); //, _oChangesMT.CurrentScope == "I" || taskIndx == 9 ? 1 : 2);// taskIndx == 3 ? "R" : "X"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                                                                            //_oChangesMT.RequestDate = tm;
                                _oChangesMT.StatusModDate = tm;
                                _oChangesMT.ApprovalStatus = strApprovalStatus;// this.GetApprovalActionStatus(oActionStepList);  // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,

                                _oChangesMT.ReqStatus = (strApprovalStatus == "F" || (strApprovalStatus == "A" && _oChangesMT.CurrentScope == "E")) ? "T" :
                                                         strApprovalStatus == "D" ? "U" : strApprovalStatus == "H" ? "I" : _oChangesMT.ReqStatus; //await ack... In transit else Unsuccessful    

                                if (_oChangesMT.ReqStatus == "U")
                                {
                                    // _oChangesMT.ReqStatus = "C";     //closed... can be re-Opened at current scope for reverse actions... Change "C" to "I" and await any more step... [approve, decline] for next possible status
                                    _oChangesMT.TransferDate = null;

                                    // update the AA ...
                                    _oAA.Status = "D";  //Active, Deactive, Close
                                    _oAA.Comments = "Request " + GetRequestStatusDesc(_oChangesMT.ReqStatus).ToLower();


                                    //var oAppAction_Curr = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                    //            .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId &&
                                    //                (c.ProcessCode == "TRF_OT" || c.ProcessCode == "TRF_IN") && c.ProcessSubCode == _oChangesMT.TransferType && c.CallerRefId == _oChangesMT.Id && c.Status == "A")
                                    //            .FirstOrDefault();
                                    //if (oAppAction_Curr != null)
                                    //{
                                    //    oAppAction_Curr.Status = "D";
                                    //    oAppAction_Curr.Comments = "Request " + GetRequestStatusDesc(_oChangesMT.ReqStatus).ToLower();
                                    //    oAppAction_Curr.LastMod = tm;
                                    //    oAppAction_Curr.LastModByUserId = this._oLoggedUser.Id;

                                    //    _context.Update(oAppAction_Curr);
                                    //}
                                }

                                _oAA.LastMod = tm;
                                _oAA.LastModByUserId = this._oLoggedUser.Id;


                                try
                                {
                                    _context.Update(_oAA);

                                }
                                catch (Exception ex)
                                {
                                    return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
                                }

                                //
                                //if (strApprovalStatus == "F" || (strApprovalStatus == "A" || _oChangesMT.CurrentScope == "E")) _oChangesMT.Status = "T";
                                //else if (strApprovalStatus == "D") _oChangesMT.Status = "U";
                                //else if (strApprovalStatus == "H") _oChangesMT.Status = "I";
                            }
                        }
                            

                        


                        //if Approved... and scope == internal and status==In Progress
                        if (taskIndx == 7 && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))  //(taskIndx == 7 && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))
                        {
                            //  _oChangesMT.Status = "T";strApprovalStatus == "A" || strApprovalStatus == "F" ? "T" : strApprovalStatus == "D" ? "U" : _oChangesMT.Status; //await ack... In transit else Unsuccessful    

                            if (_oChangesMT.CurrentScope == "I" && _oChangesMT.ReqStatus == "I") //&& (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))
                            {

                                /////// can be inherited from --- higher CBs but not approvers MUST be localized @Setup : note the target CB Level
                                ////var oAppProStepList = _context.ApprovalProcessStep.Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
                                ////    .Where(c => c.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.ToChurchBody.AppGlobalOwnerId && c.ApprovalProcess.ChurchBody.AppGlobalOwnerId == _oChangesMTbkp.ToChurchBody.AppGlobalOwnerId &&
                                ////             c.ApprovalProcess.TargetChurchLevelId == _oChangesMTbkp.ToChurchBody.ChurchLevelId &&
                                ////        c.ApprovalProcess.ProcessCode == "TRF_IN" && c.ApprovalProcess.ProcessSubCode == "MT" && c.ApprovalProcess.ProcessStatus == "A" && c.StepStatus == "A")
                                ////    .ToList();

                                 
                                //var oAppProcessList = _context.ApprovalProcess.AsNoTracking().Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                //    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId && c.TargetChurchLevelId == _oChangesMTbkp.ToChurchBody.ChurchLevelId &&
                                //        c.ProcessCode == "TRF_IN" && c.ProcessSubCode == "MT" && c.ProcessStatus == "A" && c.ProcessStatus == "A" &&
                                //        ((c.SharingStatus == "N" && c.OwnedByChurchBodyId == _oChangesMT.ChurchBodyId) || c.SharingStatus != "N"))
                                //    .ToList();

                                //oAppProcessList = oAppProcessList.Where(c =>
                                //                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                                //if (oAppProcessList.Count == 0)
                                //    return Json(new { taskSuccess = false, userMess = "Approval flow configurations not done yet. Transfer process cannot run. Please check with Administrator and try again. Hint: Settings done at higher courts of the church run down /override settings in the lower ranked congregations." });

                                //if (oAppProcessList.Count > 1)
                                //    oAppProcessList = oAppProcessList.OrderBy(c => c.OwnedByChurchBody.ChurchLevel.LevelIndex).ToList();  /// HQ -- Presbytery -- District -- Local

                                //var oAppProcess = oAppProcessList[0];

                                //var oAppProStepList = _context.ApprovalProcessStep.AsNoTracking().Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
                                //    .Where(c => c.ChurchBody.AppGlobalOwnerId == oAppProcess.AppGlobalOwnerId && c.ApprovalProcessId == oAppProcess.Id && c.StepStatus == "A")
                                //    .ToList();

                               
                                ////create approval action
                                //List<ApprovalAction> oAppActionList = new List<ApprovalAction>();
                                //if (oAppProStepList.Count > 0)
                                //{
                                //    oAppActionList.Add(
                                //    new ApprovalAction
                                //    {
                                //        //Id = 0,
                                //        ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                //        // ChurchBody = _oChangesMT.RequestorChurchBody,
                                //        ApprovalActionDesc = "Incoming Member Transfer",
                                //        ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
                                //        ProcessCode = "TRF",
                                //        ProcessSubCode = "MT",
                                //        ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
                                //        ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
                                //        CallerRefId = _oChangesMT.Id,  //reference to the Transfer details
                                //        CurrentScope = "E",
                                //        Status = "A",
                                //        //ActionDate = null,
                                //        //Comments = "",
                                //        ActionRequestDate = tm,
                                //        Created = tm,
                                //        LastMod = tm
                                //    });

                                //    if (oAppActionList.Count > 0)
                                //        _context.Add(oAppActionList.FirstOrDefault());


                                //    // check the approvals config before....
                                //    int? oApproverChurchBodyId = null; int? oApproverChurchMemberId = null; int? oApproverChurchRoleId = null; int? oApproverMemberChurchRoleId = null;
                                //    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                                //    {
                                //        //var oICurrApprover = _context.MemberChurchRole.Where(c => c.ChurchBodyId == _oChangesMT.ToChurchBodyId && c.ChurchRoleId == oAppProStep.ApproverChurchRoleId &&
                                //        //                                                    c.IsCurrentRole == true && c.IsLeadRole == true).FirstOrDefault();

                                //        //if (oICurrApprover == null)
                                //        //{ // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");  return Json(false);
                                //        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
                                //        //}

                                //        var oStepApprover = _context.ApprovalProcessApprover.Where(c => c.ChurchBodyId == oCBid && c.ApprovalProcessStepId == oAppProStep.Id).FirstOrDefault();
                                //        if (oStepApprover == null) return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Please verify and try again." });

                                //        if (oStepApprover.Approver1ChurchBodyId == null && oStepApprover.Approver1ChurchMemberId == null &&
                                //            oStepApprover.Approver2ChurchBodyId == null && oStepApprover.Approver2ChurchMemberId == null)
                                //            return Json(new { taskSuccess = false, userMess = "Approver details not available for configured approval flow step. Please verify and try again." });

                                //        oApproverChurchMemberId = oStepApprover.Approver1ChurchMemberId == null ? oStepApprover.Approver1ChurchMemberId : oStepApprover.Approver2ChurchMemberId;


                                //        var oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver1ChurchBodyId && c.Id == oStepApprover.Approver1ChurchMemberId && c.Status == "A").FirstOrDefault();

                                //        if (oApproverCM == null)
                                //            oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oStepApprover.Approver2ChurchBodyId && c.Id == oStepApprover.Approver2ChurchMemberId && c.Status == "A").FirstOrDefault();

                                //        if (oApproverCM == null)
                                //            return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });


                                //        // the role is great BUT that cannot prevent the Member from approving the request....
                                //        ///
                                //        //var oApproverMCR = _context.MemberChurchRole.Where(c => c.ChurchBodyId == oAppProStep.Approver1ChurchBodyId && c.Id == oAppProStep.Approver1MemberChurchRoleId && c.IsCurrentRole == true).FirstOrDefault();

                                //        //if (oApproverMCR == null)
                                //        //    oApproverMCR = _context.MemberChurchRole.Where(c => c.ChurchBodyId == oAppProStep.Approver1ChurchBodyId && c.ChurchRoleId == oAppProStep.Approver1ChurchRoleId && c.ChurchMemberId == oAppProStep.Approver1ChurchMemberId && c.IsCurrentRole==true).FirstOrDefault();

                                //        //if (oApproverMCR == null)
                                //        //    oApproverMCR = _context.MemberChurchRole.Where(c => c.ChurchBodyId == oAppProStep.Approver2ChurchBodyId && c.Id == oAppProStep.Approver2MemberChurchRoleId).FirstOrDefault();

                                //        //if (oApproverCM == null)
                                //        //    return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });

                                //        ///
                                //        oApproverChurchBodyId = oApproverCM.ChurchBodyId;
                                //        oApproverChurchMemberId = oApproverCM.Id;
                                //        oApproverChurchRoleId = oStepApprover.Approver1ChurchRoleId != null ? oStepApprover.Approver1ChurchRoleId : oStepApprover.Approver2ChurchRoleId;
                                //        oApproverMemberChurchRoleId = oStepApprover.Approver1MemberChurchRoleId != null ? oStepApprover.Approver1MemberChurchRoleId : oStepApprover.Approver2MemberChurchRoleId;
                                         
                                //    }



                                //    //create approval action steps
                                //    List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
                                //    var stepIndexLowest = 1; 
                                //    if (oAppProStepList.Count > 0) stepIndexLowest = oAppProStepList[0].StepIndex;
                                //    foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
                                //    {
                                //        //var oICurrApprover = _context.MemberChurchRole.Where(c => c.ChurchBodyId == _oChangesMT.ToChurchBodyId && c.ChurchRoleId == oAppProStep.ApproverChurchRoleId &&
                                //        //                                                    c.IsCurrentRole == true && c.IsLeadRole == true).FirstOrDefault();

                                //        //if (oICurrApprover == null)
                                //        //{ // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");  return Json(false);
                                //        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
                                //        //}

                                //        //if (oAppProStep.Approver1ChurchBodyId == null && oAppProStep.Approver1ChurchMemberId == null &&
                                //        //    oAppProStep.Approver2ChurchBodyId == null && oAppProStep.Approver2ChurchMemberId == null)
                                //        //    return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Please verify and try again." });

                                //        //oApproverChurchMemberId = oAppProStep.Approver1ChurchMemberId == null ? oAppProStep.Approver1ChurchMemberId : oAppProStep.Approver2ChurchMemberId;

                                //        //var oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oAppProStep.Approver1ChurchBodyId && c.Id == oAppProStep.Approver1ChurchMemberId && c.Status == "A").FirstOrDefault();

                                //        //if (oApproverCM == null)
                                //        //    oApproverCM = _context.ChurchMember.Where(c => c.ChurchBodyId == oAppProStep.Approver2ChurchBodyId && c.Id == oAppProStep.Approver2ChurchMemberId && c.Status == "A").FirstOrDefault();

                                //        //if (oApproverCM == null)
                                //        //    return Json(new { taskSuccess = false, userMess = "Approver membership could not be verified. Hint: Approver must be available. Please check configuration and try again." });
                                         
                                //        stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
                                //        oAppActionStepList.Add(
                                //            new ApprovalActionStep
                                //            {
                                //                //Id = 0,
                                //                ChurchBodyId = _oChangesMT.RequestorChurchBodyId,
                                //                //ChurchBody = _oChangesMT.RequestorChurchBody,
                                //                ///
                                //                ApproverChurchBodyId = oApproverChurchBodyId,  //  oICurrApprover.Id,
                                //                ApproverChurchMemberId = oApproverChurchMemberId,
                                //                ApproverChurchRoleId = oApproverChurchRoleId,
                                //                ApproverMemberChurchRoleId = oApproverMemberChurchRoleId,
                                //                ///
                                //                // ApproverMemberChurchRole = oICurrApprover, 
                                //                ActionStepDesc = oAppProStep.StepDesc,
                                //                ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
                                //                ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
                                //                ApprovalProcessStepRefId = oAppProStep.Id,
                                //                CurrentScope = "E",
                                //                StepRequestDate = tm,
                                //                // Comments="",
                                //                // CurrentStep= true,
                                //                // ApproverMemberChurchRoleId  = vm.oCurrLoggedMemberId,  // actual approver
                                //                // ActionBy = vm.oCurrLoggedMember,
                                //                // ActionDate = tm,
                                //                Status = "A", //Active
                                //                Created = tm,  
                                //                LastMod = tm,

                                //            //
                                //                ApprovalActionId = oAppActionList.FirstOrDefault().Id,
                                //                ApprovalAction = oAppActionList.FirstOrDefault()
                                //            }); ;
                                //    }

                                //    foreach (ApprovalActionStep oAS in oAppActionStepList)
                                //    {
                                //        oAS.IsCurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled 
                                //        _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }
                                //    }

                                //    //_oChangesMT.ReqStatus = "I"; //keep as it is == in prrogres. to the applicant.. Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                                //    // _oChangesMT.RequestDate = tm;

                                _oChangesMT.ToRequestDate = tm;   // after approval or acknowledgment from the FROM congregation

                                //    _oChangesMT.RequireApproval = oAppActionList.Count > 0; // _oMemTransf_VM.RequireApproval,
                                //    _oChangesMT.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
                            
                                _oChangesMT.CurrentScope = "E";   /// FrCB shud done this by now... @Acknowledgment
                                //    _oChangesMT.EApprovalActionId = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oMemTransf_VM.IApprovalActionId,
                                //                                                                                                                 // _oChangesMT.EApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction, 
                                //                                                                                                                 // _oChangesMTbkp.EApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction, 

                                _oChangesMT.LastMod = tm;
                                _oChangesMT.LastModByUserId = this._oLoggedUser.Id;                                
                                                               
                                
                                //}

                            }

                            //perform the transfers... last Approver approved!
                            else if (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "A" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))
                            {


                                //perform the transfer ...and then Close the request after 'success'.
                                _oChangesMT.TransferDate = tm;    ///same date to be used as the Enrollment date /Departure date for member

                                if (this.PerformMemberTransfer(_context, _oChangesMT, vm.oChurchBody.Id) == false)  // add _bkp to get other details
                                {  //reverse action .....    ModelState.AddModelError(string.Empty, "Member transfer unsuccessful.");
                                    return Json(new { taskSuccess = false, userMess = "Member transfer unsuccessful. Please try to resubmit or contact administrator and try again" });
                                }

                                // update the member data pool
                                //isTransferDone_MemMoved = true;

                                ////notify all appprovers  ... Approvers receive Notification ONLY at their Action
                                //var oAASList = _context.ApprovalActionStep   //.Include(t => t.ApproverChurchMember)  /// .Include(t => t.ApprovalAction)
                                /////.Include(t => t.ApproverMemberChurchRole).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
                                //.Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId &&
                                //           (c.ApprovalActionId == _oChangesMT.IApprovalActionId || c.ApprovalActionId == _oChangesMT.EApprovalActionId)).ToList();

                                //oCurrApprovers.AddRange(oAASList);

                                //update... you can reverse in case... WHEN member reports at To-CB... Member Account status will move from [In-Transit] to [Active] then the counts will pick else include In-Transit as members already ---
                                // _oChangesMT.ReqStatus = "Y"; //Yes... Transferred done....T ... "C";     // closed... From or requaesting congregation MUST acknowledge                         
                                _oChangesMT.ReqStatus = "T"; //Transfer done but member has not moved physically yet! ...in Transit state... not here, not there either! but actually data moved [to go back to Fr-CB.. new request in reverse order]
                                _oChangesMT.ReqStatusComments = "Transfer of membership done. Awaiting member's visit [in-person] at new congregation";
                                _oChangesMT.StatusModDate = tm;


                                //if (!this.PerformMemberTransfer(_context, oMemTransf, vm.oChurchBody.Id))
                                //{  //reverse action
                                //   // ModelState.AddModelError(string.Empty, "Member transfer unsuccessful.");
                                //    return Json(new { taskSuccess = false, userMess = "Member transfer unsuccessful. Failed operation reversing..." });
                                //}
                            }
                        }
                    }

                    else if (taskIndx == 10) // && vm.lsCurrApprActionSteps.Count > 0)  //FORCE-COMPLETE ...iirespective of the state
                    {
                        string strApprovalStatus = null;
                        var oActionStepList = vm.lsCurrApprActionSteps;

                        //create approval action...  force-complete rest of the steps  unapproved.. at least 1 step of approval
                        if (oActionStepList.Count > 0)
                        {
                            foreach (ApprovalActionStep oAAStep in oActionStepList)
                            {
                                oAAStep.ActionStepStatus = "F";
                                oAAStep.Comments = vm.strApproverComment;  //could also be reason from the user/applicant
                                oAAStep.ActionDate = tm;
                                oAAStep.LastMod = tm;
                                oAAStep.LastModByUserId = this._oLoggedUser.Id;

                                _context.Update(oAAStep);
                            }

                            var oAA = oActionStepList[0].ApprovalAction; // not null
                            strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, "F"); // this.GetApprovalActionStatus(oActionStepList, _oChangesMT.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
                            oAA.ActionStatus = strApprovalStatus;
                            oAA.Comments = vm.strApproverComment;   //could also be reason from the user/applicant
                            oAA.LastActionDate = tm;
                            oAA.LastMod = tm;
                            oAA.LastModByUserId = this._oLoggedUser.Id;

                            _context.Update(oAA);

                            _oChangesMT.ReqStatus = this.GetApprovalActionStatus(oActionStepList, _oChangesMT.CurrentScope == "I" ? 1 : 2, "F"); // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
                            _oChangesMT.RequestDate = tm;
                            _oChangesMT.ApprovalStatus = strApprovalStatus; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,   
                            _oChangesMT.ReqStatus = "T"; //await ack... In transit
                        }
                    }

                    else if (taskIndx == 11) //Close request
                    { //if (vm.lsCurrApprActionSteps.Count > 0)  //{ 

                        _oChangesMT.WorkSpanStatus = "C";  // any -Y, -U [D], -X ...can be closed
                        _oChangesMT.StatusModDate = tm;
                        //_oChangesMT. LastMod = tm;
                        //_oChangesMT. LastModByUserId = this._oLoggedUser.Id;                        
                        // _oChangesMT.Status = "I";  KEEP THE STATUS

                        // update the AA ...
                        var oAppAction_Curr = _context.ApprovalAction.AsNoTracking()  //.Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == _oChangesMT.AppGlobalOwnerId &&
                                        (c.ProcessCode == "TRF_OT" || c.ProcessCode == "TRF_IN") && c.ProcessSubCode == _oChangesMT.TransferType && c.CallerRefId == _oChangesMT.Id && c.Status == "A")
                                    .FirstOrDefault();
                        if (oAppAction_Curr != null)
                        {
                            oAppAction_Curr.Status = "C";
                            oAppAction_Curr.Comments = "Request " + GetRequestStatusDesc(_oChangesMT.ReqStatus).ToLower();
                            oAppAction_Curr.LastMod = tm;
                            oAppAction_Curr.LastModByUserId = this._oLoggedUser.Id;

                            _context.Update(oAppAction_Curr);
                        }
                    }

                    /// 12 /13 -- Return Request (I), Return Request (E) 

                    else if (taskIndx == 14) //  Re-open request
                    {
                        _oChangesMT.WorkSpanStatus = "A";    // only closed requests are re-opened... move from -C to -A
                        _oChangesMT.StatusModDate = tm;     // await ack... In transit  // }

                        //_oChangesMT. LastMod = tm;
                        //_oChangesMT. LastModByUserId = this._oLoggedUser.Id;  
                    }

                    //else if (taskIndx == 15) // Archive request
                    //{
                    //    _oChangesMT.ReqStatus = "Z";
                    //    _oChangesMT.StatusModDate = tm;
                    //    // _oChangesMT.Status = "I";  KEEP THE STATUS

                    //    //_oChangesMT. LastMod = tm;
                    //    //_oChangesMT. LastModByUserId = this._oLoggedUser.Id; 
                    //} 


                    /// Auto - Close requests [processed] older than 72 hours [3 days...]
                    ///
                    if (_oChangesMT.WorkSpanStatus != "C" && (_oChangesMT.ReqStatus == "X" || _oChangesMT.ReqStatus == "R" || _oChangesMT.ReqStatus == "Y" || _oChangesMT.ReqStatus == "U" || _oChangesMT.ReqStatus == "D") &&
                        _oChangesMT.StatusModDate >= tm.AddHours(-72).Date)
                    {
                        _oChangesMT.WorkSpanStatus = "C";
                        _oChangesMT.StatusModDate = tm;
                    }
                }

               

                try
                {
                    //if (oMemTransf == null)
                    //    { //ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
                    //    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
                    //}

                    if (taskIndx > 1) // new or update instances already dealt with up there
                    {
                        try
                        {
                            _oChangesMT.LastMod = tm;
                            _oChangesMT.LastModByUserId = this._oLoggedUser.Id; 

                            _context.Update(_oChangesMT);
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
                        }
                    }


                    //if (_oChangesMT.Id == 0)
                    //{
                    //    switch (taskIndx)
                    //    {
                    //        case 1:
                    //            oUserMsg = "Saved member transfer details successfully.";
                    //            break;

                    //        case 2:
                    //            oChuMemNotifyList.Add(_oChangesMT.ChurchMemberTransf);
                    //            oNotifMsg = "Member transfer request sent successfully. #Request_Id: " + _oChangesMT.Id;
                    //            oUserMsg = oNotifMsg;
                    //            break;
                    //    }
                    //}

                    /// Notify the CBs involved in the transfer...
                    if (taskIndx != 1)
                    {
                        oCBNotifyList.Add(_oChangesMTbkp.FromChurchBody);
                        oCBNotifyList.Add(_oChangesMTbkp.ToChurchBody);
                       
                    }

                    oChuMemNotifyList.Add(oCMReq_Curr);                     

                    var oNotifMsg = ""; var oNotifMsg_CTCB = ""; var oUserMsg = "";
                    switch (taskIndx)
                    {
                        case 1:
                            oUserMsg = "Saved member transfer details successfully.";
                            break;

                        case 2:  //send 
                            oChuMemNotifyList.Add(_oChangesMTbkp.ChurchMember);                            
                            ///
                            oNotifMsg = "Please your Member transfer request to " + _oChangesMTbkp.ToChurchBody.Name + " has been submitted successfully. You will be notified [via email or SMS] until the transfer process is through. #Request_Id: " + _oChangesMT.Id;
                            oNotifMsg_CTCB = "Member transfer request to " + _oChangesMTbkp.ToChurchBody.Name + " has been submitted successfully. Please the congregation will be notified [via Transfer tray, email or SMS] until the transfer process is through. #Request_Id: " + _oChangesMT.Id;
                            oUserMsg = oNotifMsg_CTCB;
                            break;

                        case 3:  //recall
                            { //check for steps available... sent notification to all approvers and the applicant, FromChurchBody, ToChurchBody
                                //foreach (ApprovalActionStep oStep in vm.lsCurrApprActionSteps)
                                //    if (oStep.ApproverChurchMember != null)
                                //        oChuMemNotifyList.Add(oStep.ApproverChurchMember);

                                //if (oStep.ApproverMemberChurchRole != null) if (oStep.ApproverMemberChurchRole.ChurchMember != null)
                                //        oChuMemNotifyList.Add(oStep.ApproverMemberChurchRole.ChurchMember);

                                oChuMemNotifyList.Add(_oChangesMTbkp.ChurchMember);  //requestor : [to], approvers [ to /from]
                                oNotifMsg = "Please your Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name + 
                                            " has been recalled [by requestor or congregation]." + Environment.NewLine + "#Request_Id: " + _oChangesMT.Id;
                                oNotifMsg_CTCB = "Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name +
                                                " has been recalled [by requestor or congregation]." + Environment.NewLine + "#Request_Id: " + _oChangesMT.Id;
                                oUserMsg = oNotifMsg_CTCB;
                            }
                            break;

                        case 4: //cancel
                            { //check for steps available... sent notification to all approvers and the applicant, FromChurchBody, ToChurchBody
                                //foreach (ApprovalActionStep oStep in vm.lsCurrApprActionSteps)
                                //    if (oStep.ApproverChurchMember != null)  
                                //        oChuMemNotifyList.Add(oStep.ApproverChurchMember);

                                //if (oStep.ApproverMemberChurchRole != null) 
                                //    if (oStep.ApproverMemberChurchRole.ChurchMember != null)
                                //        oChuMemNotifyList.Add(oStep.ApproverMemberChurchRole.ChurchMember);

                                oChuMemNotifyList.Add(_oChangesMTbkp.ChurchMember);
                                oNotifMsg = "Please your Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name +           //(_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? "to " + _oChangesMT.ToChurchBody.Name : "from " + _oChangesMT.FromChurchBody.Name) +
                                    " has been terminated [by requestor or congregation]. #Request_Id: " + _oChangesMT.Id;

                                oNotifMsg_CTCB = "Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name +           //(_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? "to " + _oChangesMT.ToChurchBody.Name : "from " + _oChangesMT.FromChurchBody.Name) +
                                    " has been terminated [by requestor or congregation]. #Request_Id: " + _oChangesMT.Id;

                                oUserMsg = oNotifMsg_CTCB;
                            }
                            break;

                        case 5: //resubmit
                            {
                                oChuMemNotifyList.Add(_oChangesMTbkp.ChurchMember);
                                oNotifMsg = "Please your Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name +                 // (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? "to " + _oChangesMT.ToChurchBody.Name : "from " + _oChangesMT.FromChurchBody.Name) +
                                    " has been resubmitted successfully. #Request_Id: " + _oChangesMT.Id;

                                oNotifMsg_CTCB = "Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name +                 // (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? "to " + _oChangesMT.ToChurchBody.Name : "from " + _oChangesMT.FromChurchBody.Name) +
                                   " has been resubmitted successfully. #Request_Id: " + _oChangesMT.Id;

                                oUserMsg = oNotifMsg_CTCB;
                            }
                            break;

                        case 6: //ack
                            {
                                oNotifMsg = "Please your Member transfer request ";
                                if (_oChangesMT.CurrentScope == "I" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P")
                                    oNotifMsg += "to " + _oChangesMTbkp.ToChurchBody.Name + " has been received and being processed.";
                                         
                                else if (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P")
                                    oNotifMsg += "from " + _oChangesMTbkp.FromChurchBody.Name + " duly acknowledged by target congregation, and pending approval.";
                                   
                                else if (_oChangesMT.CurrentScope == "E" && (_oChangesMT.ReqStatus == "A" || _oChangesMT.ReqStatus == "C") && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))
                                    oNotifMsg += "from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name + " successfully completed. Membership has been transferred; awaiting member [in-person] visit.";
                                                                  
                                oChuMemNotifyList.Add(_oChangesMTbkp.ChurchMember);
                                oNotifMsg += " #Request_Id: " + _oChangesMT.Id;
                                oNotifMsg_CTCB = "Member transfer request " +
                                    (_oChangesMT.CurrentScope == "I" ? "to " + _oChangesMTbkp.ToChurchBody.Name + " acknowledged." :
                                    (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "A" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F")) ? 
                                            "from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name + " acknowledged and successfully completed. Member has been transferred; awaiting member [in-person] visit." :
                                            "from " + _oChangesMTbkp.FromChurchBody.Name + " acknowledged.");
                                oNotifMsg_CTCB += " #Request_Id: " + _oChangesMT.Id;
                                oUserMsg = oNotifMsg_CTCB;
                            }
                            break;


                        //        (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "A" && (_oChangesMT.ApprovalStatus == "A" || _oChangesMT.ApprovalStatus == "F"))
                        //else if ((_oChangesMT.CurrentScope == "I" && _oChangesMT.ReqStatus == "P" && _oChangesMT.ApprovalStatus == "P") ||
                        //                (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))

                        case 7:
                            {
                                if (_oChangesMT.ReqStatus == "A")
                                { /// Notify only the APPROVER who did the action... and the new recipient to approve
                                    foreach (ApprovalActionStep oStep in vm.lsCurrApprActionSteps.Where(x=> x.IsCurrentStep == true).ToList())
                                        if (oStep.ApproverChurchMember != null)
                                            oChuMemNotifyList.Add(oStep.ApproverChurchMember);

                                    //if (oStep.ApproverMemberChurchRole != null) if (oStep.ApproverMemberChurchRole.ChurchMember != null)
                                    //        oChuMemNotifyList.Add(oStep.ApproverMemberChurchRole.ChurchMember);

                                    oNotifMsg = "Member transfer request " +
                                        (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                        "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name);
                                    if (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))
                                        oNotifMsg += " has been approved. Target congregation approval underway."; // and then forwarded to the target congregation.";
                                    else
                                        oNotifMsg += " successfully approved, pending acknowledgement from requesting congregation, " + _oChangesMT.FromChurchBody.Name + " to effect the transfer";
                                     
                                }

                                else
                                {
                                    int stepsLeft = 0;
                                    foreach (ApprovalActionStep oStep in vm.lsCurrApprActionSteps)
                                        if (oStep.ActionStepStatus != "A" && oStep.ActionStepStatus != "F")
                                            stepsLeft++;
                                    //
                                    ChurchMember t_cm = null; string strApproverName = "[Approver]";
                                    if (vm.oCurrApprovalActionStep.ApproverMemberChurchRole != null)
                                        if (vm.oCurrApprovalActionStep.ApproverMemberChurchRole.ChurchMember != null)
                                        {
                                            t_cm = vm.oCurrApprovalActionStep.ApproverMemberChurchRole.ChurchMember;
                                            // oChuMemNotifyList.Add(t_cm);
                                            strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
                                            //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim();
                                            var t_cr = vm.oCurrApprovalActionStep.ApproverMemberChurchRole.ChurchRole;
                                            strApproverName += (t_cr != null ? (!string.IsNullOrEmpty(t_cr.Name) ? " (" + t_cr.Name + ")" : "") : "").Trim();
                                        }

                                    oNotifMsg = "Member transfer request " + (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                        "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name);
                                    oNotifMsg += " is " + GetRequestProcessStatusDesc(_oChangesMT.ReqStatus) + ". " + Environment.NewLine + strApproverName + 
                                        " has approved " + (stepsLeft > 0 ? ". " + stepsLeft + " more approval" + (stepsLeft > 1 ? "s" : "").ToString() + " left to" : "");
                                    if (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))
                                        oNotifMsg += (_oChangesMT.ApprovalStatus == "A" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P")) ? 
                                            ". Request forwarded to target congregation." : " forward request to target congregation.";
                                    else
                                        oNotifMsg += _oChangesMT.ApprovalStatus == "A" ? ", pending acknowledgement from requesting congregation." : " complete.";
                                     
                                }

                                oNotifMsg_CTCB = oNotifMsg;
                                oNotifMsg = "Please your " + oNotifMsg;
                                ///
                                oChuMemNotifyList.Add(_oChangesMTbkp.ChurchMember);
                                oNotifMsg += " #Request_Id: " + _oChangesMT.Id;

                                oNotifMsg_CTCB = "Member transfer request " +
                                    (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                    "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name) +
                                    " approved.";

                                oUserMsg = oNotifMsg_CTCB;
                            }
                            break;

                        case 8:
                            {
                                //if (_oChangesMT.ReqStatus == "D")
                                //{ 
                                foreach (ApprovalActionStep oStep in vm.lsCurrApprActionSteps)
                                    if (oStep.ApproverChurchMember != null)
                                        oChuMemNotifyList.Add(oStep.ApproverChurchMember);

                                //if (oStep.ApproverMemberChurchRole != null) if (oStep.ApproverMemberChurchRole.ChurchMember != null)
                                //        oChuMemNotifyList.Add(oStep.ApproverMemberChurchRole.ChurchMember);

                                oNotifMsg = "Member transfer request " +
                                    (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                    "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name) +
                                    " has been ";
                                if (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))
                                    oNotifMsg += "declined by the local congregation.";
                                else
                                    oNotifMsg += "unfortunately declined by target congregation.";
                                //}

                                oChuMemNotifyList.Add(_oChangesMT.ChurchMember);
                                oNotifMsg += " Please check notes attached. #Request_Id: " + _oChangesMT.Id;

                                oNotifMsg_CTCB = oNotifMsg;
                                oNotifMsg = "Please your " + oNotifMsg;
                                ///

                                oNotifMsg_CTCB = "Member transfer request " +
                                    (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                    "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name) +
                                    " sucessfully declined.";

                                oUserMsg = oNotifMsg_CTCB;
                            }
                            break;

                        case 9:
                            {
                                //if (_oChangesMT.ReqStatus == "I")
                                //{
                                ChurchMember t_cm = null; string strApproverName = "[Approver]";
                                if (vm.oCurrApprovalActionStep.ApproverMemberChurchRole != null)
                                    if (vm.oCurrApprovalActionStep.ApproverMemberChurchRole.ChurchMember != null)
                                    {
                                        t_cm = vm.oCurrApprovalActionStep.ApproverMemberChurchRole.ChurchMember;      // oChuMemNotifyList.Add(t_cm);
                                        strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();            //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim();
                                        var t_cr = vm.oCurrApprovalActionStep.ApproverMemberChurchRole.ChurchRole;
                                        strApproverName += (t_cr != null ? (!string.IsNullOrEmpty(t_cr.Name) ? " (" + t_cr.Name + ")" : "") : "").Trim();
                                    }

                                oNotifMsg = "Member transfer request " +
                                    (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                    "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name) +
                                    " has been put 'On hold' tentative by " + strApproverName + ". Please check notes attached.";

                                //if (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))
                                //    oNotifMsg += "forward request to target congregation.";
                                //else
                                //    oNotifMsg += "successfuly effect the transfer to target congregation.";

                                oChuMemNotifyList.Add(_oChangesMT.ChurchMember);
                                oNotifMsg += " #Request_Id: " + _oChangesMT.Id;

                                oNotifMsg_CTCB = oNotifMsg;
                                oNotifMsg = "Please your " + oNotifMsg;
                                ///

                                oNotifMsg_CTCB = "Approval step suspended successfully for Member transfer request " +
                                    (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                    "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name);

                                oUserMsg = oNotifMsg_CTCB;
                                //}
                            }
                            break;

                        case 10:
                            //if (_oChangesMT.ReqStatus == "A")
                            //{
                            foreach (ApprovalActionStep oStep in vm.lsCurrApprActionSteps)
                                if (oStep.ApproverChurchMember != null)
                                    oChuMemNotifyList.Add(oStep.ApproverChurchMember);

                            //if (oStep.ApproverMemberChurchRole != null) if (oStep.ApproverMemberChurchRole.ChurchMember != null)
                            //        oChuMemNotifyList.Add(oStep.ApproverMemberChurchRole.ChurchMember);

                            oNotifMsg = "Member transfer request " +
                                (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ?
                                "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name); // + _oChangesMT.ToChurchBody.Name.ToUpper();

                            if (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P"))
                                oNotifMsg += " has been approved [force-completed] by local congregation and forwarded to the target congregation pending their approval."; // and then forwarded to the target congregation.";
                            else
                                oNotifMsg += " successfully approved [force-completed].";

                            oChuMemNotifyList.Add(_oChangesMT.ChurchMember);
                            oNotifMsg += " #Request_Id: " + _oChangesMT.Id;

                            oNotifMsg_CTCB = oNotifMsg;
                            oNotifMsg = "Please your " + oNotifMsg;
                            ///

                            oNotifMsg_CTCB = "Member transfer request " +
                                (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ? 
                                "to " + _oChangesMTbkp.ToChurchBody.Name : "from " + _oChangesMTbkp.FromChurchBody.Name) + " force-completed.";

                            oUserMsg = oNotifMsg_CTCB;

                            break;

                        case 11: //  oNotifMsg = "Member transfer request from " + _oChangesMT.FromChurchBody.Name + " successfully re-opened.";
                            oUserMsg = "Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name + " has been closed" +
                                    Environment.NewLine + "#Request_Id: " + _oChangesMT.Id; ;
                             
                            break;


                        /// 12 /13 -- Return Request (I), Return Request (E) 
                        /// 

                        case 14:      
                            oNotifMsg = "Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name + " successfully re-opened.";                             
                            oUserMsg = oNotifMsg;

                            oNotifMsg_CTCB = oNotifMsg;
                            oNotifMsg = "Please your " + oNotifMsg;

                            break;

                        case 15:        //  oNotifMsg = "Member transfer request from " + _oChangesMT.FromChurchBody.Name + " successfully re-opened.";
                            oUserMsg = "Member transfer request from " + _oChangesMTbkp.FromChurchBody.Name + " to " + _oChangesMTbkp.ToChurchBody.Name + " has been archived" +
                                    Environment.NewLine + "#Request_Id: " + _oChangesMT.Id;
                            break;

                    }



                    var strSalutation = "";
                    var strSenderId = "RHEMA-CMS [fr: " + (_oChangesMT.CurrentScope == "I" || (_oChangesMT.CurrentScope == "E" && _oChangesMT.ReqStatus == "I" && _oChangesMT.ApprovalStatus == "P") ?
                        _oChangesMTbkp.FromChurchBody.Name : _oChangesMTbkp.ToChurchBody.Name) + "]";


                    string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

                    //var strUrl1 =  HttpContext.Request.Query["ReturnUrl"];

                    // Asomdwei nka wo|enka wo nso 
                    if (!string.IsNullOrEmpty(oAGO_MT.Slogan))
                    {
                        strSalutation = oAGO_MT.Slogan;   //
                        if (strSalutation.Contains("*|*"))
                        {
                            var _arrSlogan = strSalutation.Split("*|*");
                            strSalutation = _arrSlogan.Length > 0 ? _arrSlogan[0] : strSalutation;
                        }
                    }
                    // in case --- 
                    if (string.IsNullOrEmpty(strSalutation))
                    {
                        var ts = tm.TimeOfDay;
                        if (ts.Hours >= 0 && ts.Hours < 12) strSalutation = "Good morning";
                        else if (ts.Hours <= 16) strSalutation = "Good afternoon";
                        else if (ts.Hours < 24) strSalutation = "Good evening";
                    }

                    ContactInfo oMemCI = null;
                    var strRequestor = ""; var _oNotifMsg = oNotifMsg;

                    //notify... applicant member
                    if (oChuMemNotifyList.Count > 0)
                    {
                        MailAddressCollection listToAddr = new MailAddressCollection();
                        MailAddressCollection listCcAddr = new MailAddressCollection();
                        MailAddressCollection listBccAddr = new MailAddressCollection();
                        //
                        List<string> oNotifPhone_List = new List<string>(); 
                        List<string> oNotifMessageList = new List<string>(); 

                        ///_context.ContactInfo.Where(c => c.ChurchBodyId == _oChangesMT.RequestorChurchBodyId || c.ChurchBodyId == _oChangesMT.FromChurchBodyId || c.ChurchBodyId == _oChangesMT.ToChurchBodyId).Load();

                        // _context.ContactInfo.Find(_oChangesMT.ChurchMemberTransf.ContactInfoId);   //searches the local cache loaded
                         

                        // for the member
                        foreach (var oCM in oChuMemNotifyList)
                        {
                            var num = "";
                            /*oMemCI = _context.ContactInfo.Find(_oChangesMT.ChurchMemberTransf.ContactInfoId);  */ //searches the local cache loaded
                            if (oCM.PrimContactInfoId != null)
                                oMemCI = _context.ContactInfo.AsNoTracking().Where(x=> x.AppGlobalOwnerId == oCM.AppGlobalOwnerId && x.ChurchBodyId == oCM.ChurchBodyId && 
                                            x.Id==oCM.PrimContactInfoId).FirstOrDefault();

                            if (oMemCI == null)
                                oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCM.AppGlobalOwnerId && x.ChurchBodyId == oCM.ChurchBodyId &&
                                            x.ChurchMemberId == oCM.Id).FirstOrDefault();

                            // check contact person of the member --- for some proxy CI --- only for the Member bn transferred!! Approvers av no proxies
                            if (oMemCI == null && oCM.ChurchBodyId == _oChangesMT.ChurchBodyId && oCM.Id == _oChangesMT.ChurchMemberId)
                            {
                                var oMCP = _context.MemberContact.AsNoTracking().Include(t=> t.ChurchMember).Where(x => x.AppGlobalOwnerId == oCM.AppGlobalOwnerId && x.ChurchBodyId == oCM.ChurchBodyId &&
                                           x.ChurchMemberId == oCM.Id).FirstOrDefault();
                                if (oMCP != null)
                                {
                                    if (oMCP.ChurchMember != null)
                                    {
                                        if (oMCP.ChurchMember.PrimContactInfoId != null)
                                            oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oMCP.ChurchMember.AppGlobalOwnerId && x.ChurchBodyId == oMCP.ChurchMember.ChurchBodyId &&
                                                        x.Id == oMCP.ChurchMember.PrimContactInfoId).FirstOrDefault();

                                        if (oMemCI == null)
                                            oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oMCP.ChurchMember.AppGlobalOwnerId && x.ChurchBodyId == oMCP.ChurchMember.ChurchBodyId &&
                                                        x.ChurchMemberId == oMCP.ChurchMember.Id).FirstOrDefault();
                                    } 
                                }
                            }
                               

                            if (oMemCI != null && oCM != null)
                            {  //get Custom salution... else use greeeting of the day: morning, afternoon, evening
                                //strSalutation = "Asomdwei nka wo!";
                                
                                // get the country-codes from the CI module...
                                if (oMemCI.MobilePhone1 != null)
                                { num = oMemCI.MobilePhone1; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
                                if (num.Length == 0)
                                    if (oMemCI.MobilePhone2 != null)
                                    { num = oMemCI.MobilePhone2; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
                                if (num.Length > 0)
                                {
                                    strRequestor = (((oCM.Title + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
                                    oNotifMsg = strRequestor + ", " + strSalutation + ". Please " + (_oChangesMT.ChurchMemberId == oCM.Id ? "your " : strRequestor + ".") + _oNotifMsg + ". Thank you.";
                                    //
                                    oNotifPhone_List.Add(num);
                                    oNotifMessageList.Add(oNotifMsg);

                                    //email recipients... applicant, church   ... specific e-mail content
                                    listToAddr.Add(new MailAddress(oMemCI.Email, strRequestor));

                                    var msgSubject = vm.strTransferType + ": " + vm.strTransfMemberDesc;
                                    var userMess = oNotifMsg + Environment.NewLine + "Open request: " + strUrl;
                                    var res = AppUtilties.SendEmailNotification(strSenderId, msgSubject, userMess, listToAddr, listCcAddr, listBccAddr, null, true);

                                    //SendEmailNotification(strSenderId, vm.strTransferType + ": " + vm.strTransfMemberDesc, oNotifMsg + Environment.NewLine + "Open request: " +  strUrl, 
                                    //    listToAddr, listCcAddr, listBccAddr, null);
                                }
                            }
                        }

                        //send notifications... sms, email
                        if (oNotifPhone_List.Count > 0 && oNotifPhone_List.Count == oNotifMessageList.Count)
                            AppUtilties.SendSMSNotification(oNotifPhone_List, oNotifMessageList, true);
                         
                    }


                    //notify... CB
                    oMemCI = null;
                    if (oCBNotifyList.Count > 0)
                    {
                        MailAddressCollection listToAddr_CTCB = new MailAddressCollection();
                        MailAddressCollection listCcAddr_CTCB = new MailAddressCollection();
                        MailAddressCollection listBccAddr_CTCB = new MailAddressCollection();
                        List<string> oNotifPhone_List_CTCB = new List<string>(); 
                        List<string> oNotifMessageList_CTCB = new List<string>();


                        foreach (var oCTCB in oCBNotifyList)
                        {
                           // var strApproverName = "";
                            //var t_cm = oAAS.ApproverMemberChurchRole.ChurchMember;
                            //var t_cm = oAAS.ApproverChurchMember;
                            //var oCM = _context.ChurchMember.AsNoTracking().Include(t => t.ContactInfo)
                            //    .Where(c => c.ChurchBodyId == oCB.Id && c.AppGlobalOwnerId == oCB.AppGlobalOwnerId).FirstOrDefault();
                            //if (oCM != null)
                            //{

                                //strApproverName = ((((!string.IsNullOrEmpty(oCM.Title) ?
                                //    oCM.Title : "") + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
                                //if (oCM.ContactInfo != null)
                                //    listToAddr_Appr.Add(new MailAddress(oCM.ContactInfo.Email, strApproverName));

                                var num = "";
                                /*oMemCI = _context.ContactInfo.Find(_oChangesMT.ChurchMemberTransf.ContactInfoId);  */ //searches the local cache loaded
                                if (oCTCB.ContactInfoId != null)
                                    oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCTCB.AppGlobalOwnerId && x.ChurchBodyId == oCTCB.Id &&
                                    x.IsChurchBody==true && x.IsChurchUnit==false && x.IsChurchFellow==false && x.ChurchMemberId == null  && x.ChurchUnitId == null && 
                                    x.Id == oCTCB.ContactInfoId).FirstOrDefault();

                                if (oMemCI == null)
                                    oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCTCB.AppGlobalOwnerId && x.ChurchBodyId == oCTCB.Id &&
                                                x.ChurchMemberId == null).FirstOrDefault();
                                 
                                if (oMemCI != null )
                                {  //get Custom salution... else use greeeting of the day: morning, afternoon, evening
                                   //strSalutation = "Asomdwei nka wo!";

                                    // get the country-codes from the CI module...
                                    if (oMemCI.MobilePhone1 != null)
                                    { num = oMemCI.MobilePhone1; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
                                    if (num.Length == 0)
                                        if (oMemCI.MobilePhone2 != null)
                                        { num = oMemCI.MobilePhone2; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
                                    if (num.Length > 0)
                                    {
                                    strRequestor = oCTCB.Name; // (((oCM.Title + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
                                        oNotifMsg = strRequestor + ", " + strSalutation + ". Please " + strRequestor + ". " + oNotifMsg_CTCB + ". Thank you.";
                                    //
                                    oNotifPhone_List_CTCB.Add(num);
                                    oNotifMessageList_CTCB.Add(oNotifMsg);

                                    //email recipients... applicant, church   ... specific e-mail content
                                    listToAddr_CTCB.Add(new MailAddress(oMemCI.Email, strRequestor));
                                    }
                                }
                            //}
                        }

                        //send notifications... email
                        if (listToAddr_CTCB.Count > 0)
                        {
                            var msgSubject = vm.strTransferType + ": " + vm.strTransfMemberDesc;
                            var userMess = vm.strTransMessage + Environment.NewLine + "Open request: " + strUrl;
                            var res = AppUtilties.SendEmailNotification(strSenderId, msgSubject, userMess, listToAddr_CTCB, listCcAddr_CTCB, listBccAddr_CTCB, null, true);
                        }
                        

                        //send notifications... sms
                        if (oNotifPhone_List_CTCB.Count > 0 && oNotifPhone_List_CTCB.Count == oNotifMessageList_CTCB.Count)
                            AppUtilties.SendSMSNotification(oNotifPhone_List_CTCB, oNotifMessageList_CTCB, true);
                    }


                    //notify... approvers
                    oMemCI = null;
                    if (oCurrApprovers.Count > 0)
                    {
                        MailAddressCollection listToAddr_Appr = new MailAddressCollection();
                        MailAddressCollection listCcAddr_Appr = new MailAddressCollection();
                        MailAddressCollection listBccAddr_Appr = new MailAddressCollection();
                        List<string> oNotifPhone_List_Appr = new List<string>();
                        List<string> oNotifMessageList_Appr = new List<string>();

                        List<string> oNotifPhone_List_CB = new List<string>();
                        List<string> oNotifMessageList_CB = new List<string>();

                        foreach (var oAAS in oCurrApprovers)
                        {
                            var strApproverName = "";
                            //var t_cm = oAAS.ApproverMemberChurchRole.ChurchMember;
                            //var t_cm = oAAS.ApproverChurchMember;
                            var oCM = _context.ChurchMember.AsNoTracking().Include(t => t.ContactInfo)
                                .Where(c => c.ChurchBodyId == oAAS.ApproverChurchBodyId && c.Id == oAAS.ApproverChurchMemberId).FirstOrDefault();
                            if (oCM != null)
                            {
                                strApproverName = ((((!string.IsNullOrEmpty(oCM.Title) ?
                                    oCM.Title : "") + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
                                if (oCM.ContactInfo != null)
                                    listToAddr_Appr.Add(new MailAddress(oCM.ContactInfo.Email, strApproverName));

                                var num = "";
                                /*oMemCI = _context.ContactInfo.Find(_oChangesMT.ChurchMemberTransf.ContactInfoId);  */ //searches the local cache loaded
                                if (oCM.PrimContactInfoId != null)
                                    oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCM.AppGlobalOwnerId && x.ChurchBodyId == oCM.ChurchBodyId &&
                                                x.Id == oCM.PrimContactInfoId).FirstOrDefault();

                                if (oMemCI == null)
                                    oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCM.AppGlobalOwnerId && x.ChurchBodyId == oCM.ChurchBodyId &&
                                                x.ChurchMemberId == oCM.Id).FirstOrDefault();

                                // check contact person of the member --- for some proxy CI --- only for the Member bn transferred!! Approvers av no proxies
                                if (oMemCI == null && oCM.ChurchBodyId == _oChangesMT.ChurchBodyId && oCM.Id == _oChangesMT.ChurchMemberId)
                                {
                                    var oMCP = _context.MemberContact.AsNoTracking().Include(t => t.ChurchMember).Where(x => x.AppGlobalOwnerId == oCM.AppGlobalOwnerId && x.ChurchBodyId == oCM.ChurchBodyId &&
                                                x.ChurchMemberId == oCM.Id).FirstOrDefault();
                                    if (oMCP != null)
                                    {
                                        if (oMCP.ChurchMember != null)
                                        {
                                            if (oMCP.ChurchMember.PrimContactInfoId != null)
                                                oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oMCP.ChurchMember.AppGlobalOwnerId && x.ChurchBodyId == oMCP.ChurchMember.ChurchBodyId &&
                                                            x.Id == oMCP.ChurchMember.PrimContactInfoId).FirstOrDefault();

                                            if (oMemCI == null)
                                                oMemCI = _context.ContactInfo.AsNoTracking().Where(x => x.AppGlobalOwnerId == oMCP.ChurchMember.AppGlobalOwnerId && x.ChurchBodyId == oMCP.ChurchMember.ChurchBodyId &&
                                                            x.ChurchMemberId == oMCP.ChurchMember.Id).FirstOrDefault();
                                        }
                                    }
                                }

                                if (oMemCI != null && oCM != null)
                                {  //get Custom salution... else use greeeting of the day: morning, afternoon, evening
                                   //strSalutation = "Asomdwei nka wo!";

                                    // get the country-codes from the CI module...
                                    if (oMemCI.MobilePhone1 != null)
                                    { num = oMemCI.MobilePhone1; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
                                    if (num.Length == 0)
                                        if (oMemCI.MobilePhone2 != null)
                                        { num = oMemCI.MobilePhone2; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
                                    if (num.Length > 0)
                                    {
                                        strRequestor = (((oCM.Title + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
                                        oNotifMsg = strRequestor + ", " + strSalutation + ". Please " + (_oChangesMT.ChurchMemberId == oCM.Id ? "your " : strRequestor + ".") + oNotifMsg_CTCB + ". Thank you.";
                                        //
                                        oNotifPhone_List_Appr.Add(num);
                                        oNotifMessageList_Appr.Add(oNotifMsg);

                                        //email recipients... applicant, church   ... specific e-mail content
                                        listToAddr_Appr.Add(new MailAddress(oMemCI.Email, strRequestor));
                                    }
                                }
                            }
                        }

                        //send notifications... email
                        if (listToAddr_Appr.Count > 0)
                        {
                            var msgSubject = vm.strTransferType + ": " + vm.strTransfMemberDesc;
                            var userMess = vm.strTransMessage + Environment.NewLine + "Open request: " + strUrl;
                            var res = AppUtilties.SendEmailNotification(strSenderId, msgSubject, userMess, listToAddr_Appr, listCcAddr_Appr, listBccAddr_Appr, null, true);
                        }


                        //send notifications... sms
                        if (oNotifPhone_List_Appr.Count > 0 && oNotifPhone_List_Appr.Count == oNotifMessageList_Appr.Count)
                            AppUtilties.SendSMSNotification(oNotifPhone_List_Appr, oNotifMessageList_Appr, true);
                    }



                    vm.oChurchTransfer = _oChangesMT;
                    var _oChangesMTjson = Newtonsoft.Json.JsonConvert.SerializeObject(_oChangesMT);
                    TempData["oVmCurrMod"] = _oChangesMTjson; TempData.Keep();

                    
                    ///  
                    //return Json(new { taskSuccess = true, userMess = oUserMsg });


                    /// after transfer UPDATE the Roll ... up til Apex
                    // if (_reset) {
                    ///
                    if (isTransferDone_MemMoved)
                    {
                        var resRollUpd = UpdCBMemRoll(_oChangesMT.AppGlobalOwnerId, _oChangesMT.ChurchBodyId, this._oLoggedUser.Id);
                        if (resRollUpd < 0) oUserMsg += ". Member roll summary update failed. Try update again later.";
                        else if (resRollUpd == 0) oUserMsg += ". Member roll summary update incomplete. Try update again later.";
                    }

                    ///
                    // }


                    //  ReloadCurrPageReq_CT(res.currCBId, res.dxn, res.svc_ndx, res.blReqHis, res.strTrxTP, res.oCMid);   // res.taskSuccess, currId, strItemName
                    return Json(new { taskSuccess = true, currId = _oChangesMT.Id, currCBId = _oChangesMT.ChurchBodyId, userMess = oUserMsg, signOutToLogIn = false, resetNew = false, 
                                        dxn = vm.numTransferDxn, svc_ndx = vm.serviceTask, blReqHis = false, strTrxTP = _oChangesMT.TransferType, oCMid  = _oChangesMT.ChurchMemberId, strItemName = oMTModel.strTransfMemberDesc });
                     

                }

                catch (Exception ex)
                {
                    //  ViewBag.UserMsg = "Requested action could not be performed successfully. Err: " + ex.ToString();
                    return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
            }
        }


        public int UpdCBMemRoll(int? oAGOid, int? oCBid = null, int? oUserId_Logged = null)  // -1 - fail, 0 = incomplete, 1 - done
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
                        ViewData["strRes_UpdCBMemRoll"] = "Member Roll update failed. Client database failed to connect. Try updating roll table later.";
                        return -1;
                    }
                }

                var strDesc = "Church Body Roll Bal";
                var _userTask = "";


                //// get curr CB 
                var oCBCurr = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody) //.Include(t => t.ChurchLevel)
                            .Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBid).FirstOrDefault();

                if (oCBCurr == null)
                { ViewData["strRes_UpdCBMemRoll"] = "Updated congregation's member roll summary. Roll update failed. Congregation details could not be retrieved. Try updating roll table later."; return -1; }

                /// get the current year
                var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                                .Where(c => c.AppGlobalOwnerId == oAGOid && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                oCP_List_1 = oCP_List_1.Where(c =>
                                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();
                ChurchPeriod oCPRDefault = oCP_List_1.FirstOrDefault();

                if (oCPRDefault == null)
                { ViewData["strRes_UpdCBMemRoll"] = "Updated congregation's member roll summary. Roll update failed. Church period could not be retrieved. Try updating roll table later."; return -1; }
                var oCPRDefaultId = oCPRDefault != null ? oCPRDefault.Id : (int?)null;  // prompt for error


                // update curr CB 
                // get list of local members... and save first... then update table afterward
                var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && (c.Status == "A" || c.Status == "T")).ToList();  // Active-In Transit-Blocked-Deactive current members only ::- keep oCM.Status in sync with [MemberStatus] ...VIPPP
                foreach (ChurchMember oCM in oCMList)
                {
                    oCM.numMemAge = oCM.DateOfBirth != null ? (int)AppUtilties.CalcDateDiff(oCM.DateOfBirth.Value, DateTime.Now.Date, false, true) : -1;
                }

                ///
                var tm = DateTime.Now;

                // get these value from the settings of the CB... may be inherited too! so check out boy
                //var strNVPCode = "TTL";    // c.Id != oCurrNVP.Id && 
                // c.NVPCode == "GEN_AGE_GRP_C" || "GEN_AGE_GRP_Y" || "GEN_AGE_GRP_YA" || "GEN_AGE_GRP_MA" || "GEN_AGE_GRP_AA"
                var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.OwnedByChurchBody)
                                                   .Where(c => c.AppGlobalOwnerId == oAGOid &&
                                                   c.NVPCode.Contains("GEN_AGE_GRP")).ToList();
                oNVP_List_1 = oNVP_List_1.Where(c =>
                                   (c.OwnedByChurchBodyId == oCBid ||
                                   (c.OwnedByChurchBodyId != oCBid && c.SharingStatus == "C" && c.OwnedByChurchBodyId == _oLoggedCB.ParentChurchBodyId) ||
                                   (c.OwnedByChurchBodyId != oCBid && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCBCurr)))).ToList();

                //vmLkp.lkpPersTitles = oNVP_List_1 //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                //                            .OrderBy(c => c.OrderIndex)
                //                            .ThenBy(c => c.NVPValue)
                //                            .Select(c => new SelectListItem()
                //                            {
                //                                Value = c.NVPValue,
                //                                Text = c.NVPValue
                //                            })
                //                            .ToList();

                var oNVP_List_C = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_C").FirstOrDefault();
                var oNVP_List_Y = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_Y").FirstOrDefault();
                var oNVP_List_YA = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_YA").FirstOrDefault();
                var oNVP_List_MA = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_MA").FirstOrDefault();
                var oNVP_List_AA = oNVP_List_1.Where(c => c.NVPCode == "GEN_AGE_GRP_AA").FirstOrDefault();
                // var ageLim_C = 0; var ageLim_Y = 0; var ageLim_YA = 0; var ageLim_MA = 0; var ageLim_AA = 0;
                var ageMin_C = 0; var ageMin_Y = 0; var ageMin_YA = 0; var ageMin_MA = 0; var ageMin_AA = 0;
                var ageMax_C = 0; var ageMax_Y = 0; var ageMax_YA = 0; var ageMax_MA = 0; var ageMax_AA = 0;
                ///
                if (oNVP_List_C != null)
                {
                    ageMin_C = (int)oNVP_List_C.NVPNumVal >= 0 ? (int)oNVP_List_C.NVPNumVal : 0;
                    ageMax_C = (int)oNVP_List_C.NVPNumValTo >= (int)oNVP_List_C.NVPNumVal ? (int)oNVP_List_C.NVPNumValTo : (int)oNVP_List_C.NVPNumVal;
                }
                if (oNVP_List_Y != null)
                {
                    ageMin_Y = (int)oNVP_List_Y.NVPNumVal >= 0 ? (int)oNVP_List_Y.NVPNumVal : 0;
                    ageMax_Y = (int)oNVP_List_Y.NVPNumValTo >= (int)oNVP_List_Y.NVPNumVal ? (int)oNVP_List_Y.NVPNumValTo : (int)oNVP_List_Y.NVPNumVal;
                }
                if (oNVP_List_YA != null)
                {
                    ageMin_YA = (int)oNVP_List_YA.NVPNumVal >= 0 ? (int)oNVP_List_YA.NVPNumVal : 0;
                    ageMax_YA = (int)oNVP_List_YA.NVPNumValTo >= (int)oNVP_List_YA.NVPNumVal ? (int)oNVP_List_YA.NVPNumValTo : (int)oNVP_List_YA.NVPNumVal;
                }
                if (oNVP_List_MA != null)
                {
                    ageMin_MA = (int)oNVP_List_MA.NVPNumVal >= 0 ? (int)oNVP_List_MA.NVPNumVal : 0;
                    ageMax_MA = (int)oNVP_List_MA.NVPNumValTo >= (int)oNVP_List_MA.NVPNumVal ? (int)oNVP_List_MA.NVPNumValTo : (int)oNVP_List_MA.NVPNumVal;
                }
                if (oNVP_List_AA != null)
                {
                    ageMin_AA = (int)oNVP_List_AA.NVPNumVal >= 0 ? (int)oNVP_List_AA.NVPNumVal : 0;
                    ageMax_AA = (int)oNVP_List_AA.NVPNumValTo >= (int)oNVP_List_AA.NVPNumVal ? (int)oNVP_List_AA.NVPNumValTo : (int)oNVP_List_AA.NVPNumVal;
                }
                ///

                ///
                var _countCBRollAdd = 0; var _countCBRollUpd = 0;
                var oCBMemRoll = _context.CBMemberRollBal.AsNoTracking()
                    .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBid && c.ChurchPeriodId == oCPRDefaultId).FirstOrDefault();
                if (oCBMemRoll != null)
                {
                    //oCBMemRoll.AppGlobalOwnerId = oAGOid;
                    //oCBMemRoll.ChurchBodyId = oCBid;
                    // oCBMemRoll.ChurchPeriodId = oCPRDefaultId;   // cannot change jux like that... this na money matters oo

                    oCBMemRoll.Created = tm;
                    oCBMemRoll.LastMod = tm;
                    oCBMemRoll.CreatedByUserId = oUserId_Logged;
                    oCBMemRoll.LastModByUserId = oUserId_Logged;

                    ///
                    oCBMemRoll.TotRoll = oCMList.Count();
                    oCBMemRoll.Tot_M = oCMList.Count(x => x.Gender == "M");
                    oCBMemRoll.Tot_F = oCMList.Count(x => x.Gender == "F");
                    oCBMemRoll.Tot_O = oCMList.Count(x => x.Gender == "O");
                    oCBMemRoll.Tot_C = oCMList.Count(x => x.numMemAge >= ageMin_C && x.numMemAge <= ageMax_C);
                    oCBMemRoll.Tot_Y = oCMList.Count(x => x.numMemAge >= ageMin_Y && x.numMemAge <= ageMax_Y);
                    oCBMemRoll.Tot_YA = oCMList.Count(x => x.numMemAge >= ageMin_YA && x.numMemAge <= ageMax_YA);
                    oCBMemRoll.Tot_MA = oCMList.Count(x => x.numMemAge >= ageMin_MA && x.numMemAge <= ageMax_MA);
                    oCBMemRoll.Tot_AA = oCMList.Count(x => x.numMemAge >= ageMin_AA && x.numMemAge <= ageMax_AA);

                    _context.CBMemberRollBal.Update(oCBMemRoll);
                    _countCBRollUpd++;
                }
                else  // add CB roll to the table
                {
                    oCBMemRoll = new CBMemberRollBal()
                    {
                        AppGlobalOwnerId = oAGOid,
                        ChurchBodyId = oCBid,
                        ChurchPeriodId = oCPRDefaultId,
                        ///
                        Created = tm,
                        LastMod = tm,
                        CreatedByUserId = oUserId_Logged,
                        LastModByUserId = oUserId_Logged,
                        ///
                        TotRoll = oCMList.Count(),
                        Tot_M = oCMList.Count(x => x.Gender == "M"),
                        Tot_F = oCMList.Count(x => x.Gender == "F"),
                        Tot_O = oCMList.Count(x => x.Gender == "O"),
                        Tot_C = oCMList.Count(x => x.numMemAge >= ageMin_C && x.numMemAge <= ageMax_C),
                        Tot_Y = oCMList.Count(x => x.numMemAge >= ageMin_Y && x.numMemAge <= ageMax_Y),
                        Tot_YA = oCMList.Count(x => x.numMemAge >= ageMin_YA && x.numMemAge <= ageMax_YA),
                        Tot_MA = oCMList.Count(x => x.numMemAge >= ageMin_MA && x.numMemAge <= ageMax_MA),
                        Tot_AA = oCMList.Count(x => x.numMemAge >= ageMin_AA && x.numMemAge <= ageMax_AA)
                    };

                    _context.CBMemberRollBal.Add(oCBMemRoll);
                    _countCBRollAdd++;
                }

                // save first before updating roll... up
                _context.SaveChanges();


                if (oCBCurr.ParentChurchBody == null)
                { ViewData["strRes_UpdCBMemRoll"] = "Updated congregation's member roll summary. Roll update complete. No further update as congregation has no parent."; return 1; }


                //// start roll update with parent >> child CBs 
                var oCBPar = oCBCurr.ParentChurchBody;

                // create CB path up -- >> 
                ///
                var oCBMemRoll_List = new List<CBMemberRollBal>();  // update if CB exists, else add .... batch update
                var oNextCBMemRoll = new CBMemberRollBal();
                //var currCBid = oCBid;
                var _countCBRollAdd_Par = 0; var _countCBRollUpd_Par = 0;
                var _countCBRollAdd_Temp = 0; var _countCBRollUpd_Temp = 0;
                do
                {
                    // get the previous bal ... and add the new current bal.
                    // get these value feom the settings of the CB... may be inherited too! so check out boy
                    // ageLim_C = 0; var ageLim_Y = 0; var ageLim_YA = 0; var ageLim_MA = 0; var ageLim_OA = 0;
                    var oNextCBMemRoll_List = _context.CBMemberRollBal.AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBody.ParentChurchBodyId == oCBPar.Id && c.ChurchPeriodId == oCPRDefaultId).ToList();  // Active-Blocked-Deactive current members only ::- keep oCM.Status in sync with [MemberStatus] ...VIPPP
                    ///
                    oNextCBMemRoll = _context.CBMemberRollBal.AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oAGOid && c.ChurchBodyId == oCBPar.Id && c.ChurchPeriodId == oCPRDefaultId).FirstOrDefault();  // for curr period

                    if (oNextCBMemRoll_List.Count > 0)
                    {
                        //for (int i = 0; i < oNextCBMemRoll_List.Count; i++)
                        //{
                        // oNextCBMemRoll = oNextCBMemRoll_List[i];
                        if (oNextCBMemRoll != null)
                        {
                            //oNextCBMemRoll.AppGlobalOwnerId = oAGOid;
                            //oNextCBMemRoll.ChurchBodyId = oCBid;
                            // ChurchPeriodId = oCPRDefaultId;

                            oNextCBMemRoll.Created = tm;
                            oNextCBMemRoll.LastMod = tm;
                            oNextCBMemRoll.CreatedByUserId = oUserId_Logged;
                            oNextCBMemRoll.LastModByUserId = oUserId_Logged;
                            ///
                            oNextCBMemRoll.TotRoll = oNextCBMemRoll_List.Sum(x => x.TotRoll);
                            oNextCBMemRoll.Tot_M = oNextCBMemRoll_List.Sum(x => x.Tot_M);
                            oNextCBMemRoll.Tot_F = oNextCBMemRoll_List.Sum(x => x.Tot_F);
                            oNextCBMemRoll.Tot_O = oNextCBMemRoll_List.Sum(x => x.Tot_O);
                            oNextCBMemRoll.Tot_C = oNextCBMemRoll_List.Sum(x => x.Tot_C);
                            oNextCBMemRoll.Tot_Y = oNextCBMemRoll_List.Sum(x => x.Tot_Y);
                            oNextCBMemRoll.Tot_YA = oNextCBMemRoll_List.Sum(x => x.Tot_YA);
                            oNextCBMemRoll.Tot_MA = oNextCBMemRoll_List.Sum(x => x.Tot_MA);
                            oNextCBMemRoll.Tot_AA = oNextCBMemRoll_List.Sum(x => x.Tot_AA);

                            _context.CBMemberRollBal.Update(oNextCBMemRoll);
                            _countCBRollUpd_Temp++;
                        }
                        else  // add CB roll to the table
                        {
                            oNextCBMemRoll = new CBMemberRollBal()
                            {
                                AppGlobalOwnerId = oCBPar.AppGlobalOwnerId,
                                ChurchBodyId = oCBPar.Id,
                                ChurchPeriodId = oCPRDefaultId,
                                ///
                                Created = tm,
                                LastMod = tm,
                                CreatedByUserId = oUserId_Logged,
                                LastModByUserId = oUserId_Logged,
                                ///
                                TotRoll = oNextCBMemRoll_List.Sum(x => x.TotRoll),
                                Tot_M = oNextCBMemRoll_List.Sum(x => x.Tot_M),
                                Tot_F = oNextCBMemRoll_List.Sum(x => x.Tot_F),
                                Tot_O = oNextCBMemRoll_List.Sum(x => x.Tot_O),
                                Tot_C = oNextCBMemRoll_List.Sum(x => x.Tot_C),
                                Tot_Y = oNextCBMemRoll_List.Sum(x => x.Tot_Y),
                                Tot_YA = oNextCBMemRoll_List.Sum(x => x.Tot_YA),
                                Tot_MA = oNextCBMemRoll_List.Sum(x => x.Tot_MA),
                                Tot_AA = oNextCBMemRoll_List.Sum(x => x.Tot_AA)
                            };

                            _context.CBMemberRollBal.Add(oNextCBMemRoll);
                            _countCBRollAdd_Temp++;
                        }

                        if ((_countCBRollAdd_Temp + _countCBRollUpd_Temp) > 0)
                        {
                            _context.SaveChanges();  // batch can't work... save ... so the list can be upadted!
                            ///
                            _countCBRollAdd_Par += _countCBRollAdd_Temp;
                            _countCBRollUpd_Par += _countCBRollUpd_Temp;

                            _countCBRollAdd_Temp = 0; _countCBRollUpd_Temp = 0;
                        }
                        //}
                    }
                    //else
                    //{
                    //oNextCBMemRoll = new CBMemberRollBal()
                    //{
                    //    AppGlobalOwnerId = oCBPar.AppGlobalOwnerId,
                    //    ChurchBodyId = oCBPar.Id,
                    //    Created = tm,
                    //    LastMod = tm,
                    //    CreatedByUserId = oUserId_Logged,
                    //    LastModByUserId = oUserId_Logged,
                    //    ///
                    //    TotRoll = 0, //oNextCBMemRoll_List.Sum(x => x.TotRoll),
                    //    Tot_M   = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_M),
                    //    Tot_F   = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_F),
                    //    Tot_O   = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_O),
                    //    Tot_C   = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_C),
                    //    Tot_Y   = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_Y),
                    //    Tot_YA  = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_YA),
                    //    Tot_MA  = 0, //oNextCBMemRoll_List.Sum(x => x.Tot_MA),
                    //    Tot_AA  = 0  //oNextCBMemRoll_List.Sum(x => x.Tot_AA)
                    //};

                    //_context.CBMemberRollBal.Add(oNextCBMemRoll);
                    //_countCBRollAdd_Temp++;

                    //if (_countCBRollAdd_Temp > 0)
                    //{
                    //    _context.SaveChanges();  // batch can't work... save ... so the list can be upadted!
                    //    ///
                    //    _countCBRollAdd_Par += _countCBRollAdd_Temp;
                    //    _countCBRollUpd_Par += _countCBRollUpd_Temp;

                    //    _countCBRollAdd_Par = 0; _countCBRollUpd_Par = 0;
                    //}
                    //}

                    //add to list
                    // oCBMemRoll_List.Add(oNextCBMemRoll);

                    //oCBPar becomes curr CB... then loop
                    oCBPar = _context.ChurchBody.AsNoTracking() //.Include(t => t.ParentChurchBody) //.Include(t => t.ChurchLevel)
                                    .Where(c => c.AppGlobalOwnerId == oAGOid && c.Id == oCBPar.ParentChurchBodyId).FirstOrDefault();

                    // stop once the chain breaks --- no parent!
                }
                while (oCBPar != null); // && oCBCurr.ChurchLevel.LevelIndex > 0)


                //// update ... with table built
                //var _countCBRollAdd_Par = 0;  var _countCBRollUpd_Par = 0; 
                //for (int i = 0; i < oCBMemRoll_List.Count; i++)
                //{
                //    var _oChanges = oCBMemRoll_List[i]; 
                //    _oChanges.LastMod = tm;
                //    _oChanges.LastModByUserId = oUserId_Logged;

                //    if (_oChanges.Id > 0)
                //    {
                //        _context.CBMemberRollBal.Update(_oChanges);
                //        _countCBRollUpd_Par++;
                //    }
                //    else
                //    {
                //        _oChanges.Created = tm;
                //        _oChanges.CreatedByUserId = oUserId_Logged;
                //        ///
                //        _context.CBMemberRollBal.Add(_oChanges);
                //        _countCBRollAdd_Par++; 
                //    }
                //}

                if ((_countCBRollAdd + _countCBRollUpd + _countCBRollAdd_Par + _countCBRollUpd_Par) > 0)
                {
                    //_context.SaveChanges();
                    ///
                    _userTask = "Updated member roll balance for " + (_countCBRollAdd + _countCBRollUpd + _countCBRollAdd_Par + _countCBRollUpd_Par) + " congregations [ added:" + _countCBRollAdd + _countCBRollAdd_Par + "; modified: " + _countCBRollAdd_Par + _countCBRollUpd_Par + "] on same route successfully.";
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

                ViewData["strRes_UpdCBMemRoll"] = _userTask;
                return 1;
            }

            catch (Exception ex)
            {
                ViewData["strRes_UpdCBMemRoll"] = "Failed updating member roll summary. Refresh data or contact system admin. Error: " + ex.Message;
                return -1;
            }
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddOrEdit_CT(ChurchTransferModel vmMod)
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


        //    TitheTrans _oChanges = vmMod.oTitheTrans;   //  vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as ChurchTransferModel : vmMod; TempData.Keep();

        //    var arrData = ""; // TempData["oVmCurrMod"] as string;
        //    arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
        //    vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchTransferModel>(arrData) : vmMod;
        //    //
        //    var oMdlData = vmMod.oTitheTrans;
        //    oMdlData.ChurchBody = vmMod.oChurchBody;

        //    try
        //    {
        //        ModelState.Remove("oTitheTrans.AppGlobalOwnerId");
        //        ModelState.Remove("oTitheTrans.ChurchBodyId");
        //        ModelState.Remove("oTitheTrans.ChurchMemberId");
        //        ModelState.Remove("oTitheTrans.Corporate_ChurchBodyId");
        //        ModelState.Remove("oTitheTrans.TitheModeId");
        //        ModelState.Remove("oTitheTrans.RelatedEventId");
        //        ModelState.Remove("oTitheTrans.AccountPeriodId");
        //        ModelState.Remove("oTitheTrans.CurrencyId");

        //        ModelState.Remove("oTitheTrans.CreatedByUserId");
        //        ModelState.Remove("oTitheTrans.LastModByUserId");
        //        ModelState.Remove("oUserId_Logged");

        //        // ChurchBody == null 

        //        //finally check error state...
        //        if (ModelState.IsValid == false)
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again." });

        //        if (_oChanges.TitheAmount == null || _oChanges.TitheAmount == 0) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Enter the tithe amount" });
        //        }
        //        if (string.IsNullOrEmpty(_oChanges.Curr3LISOSymbol)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select currency." });
        //        }
        //        if (_oChanges.ChurchPeriodId == null) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select account period." });
        //        }
        //        if (_oChanges.TitheMode == "M" && _oChanges.ChurchMemberId == null)  // 
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify member paying tithe else change the tithe mode [hint: try choosing 'Anonymous']." });
        //        }
        //        if ((_oChanges.TithedByScope == "D" && _oChanges.TitheMode == "G" && _oChanges.Corporate_ChurchBodyId == null) ||
        //            (_oChanges.TithedByScope == "E" && _oChanges.TitheMode == "G" && string.IsNullOrEmpty(_oChanges.TitherDesc)))
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify group or corporate body paying tithe else change the tithe mode [hint: try choosing 'Anonymous']." });
        //        }
        //        if (_oChanges.TitheMode == "O" && string.IsNullOrEmpty(_oChanges.TitherDesc))  // 
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify tither else change the tithe mode [hint: try choosing 'Anonymous']." });
        //        }
        //        if (_oChanges.TitheDate == null)  //Congregant... ChurchCodes required
        //        {
        //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the date." });
        //        }

        //        if (_oChanges.TitheDate != null)
        //        {
        //            if (_oChanges.TitheDate.Value > DateTime.Now.Date)
        //                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Select the correct date. Date cannot be later than today." });

        //            if (_oChanges.PostedDate != null)
        //                if (_oChanges.TitheDate.Value > _oChanges.PostedDate.Value)
        //                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Payment date cannot be later than posting date" });
        //        }

        //        var _reset = _oChanges.Id == 0;

        //        //   _oChanges.LastMod = DateTime.Now;
        //        //  _oChanges.LastModByUserId = vmMod.oCurrUserId_Logged;

        //        //save for documents...
        //        // string uniqueFileName = null;
        //        //var oFormFile = vmMod.UserPhotoFile;
        //        //if (oFormFile != null && oFormFile.Length > 0)
        //        //{
        //        //    string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img_db");  //~/frontend/dist/img_db
        //        //    uniqueFileName = Guid.NewGuid().ToString() + "_" + oFormFile.FileName;
        //        //    string filePath = Path.Combine(uploadFolder, uniqueFileName);
        //        //    oFormFile.CopyTo(new FileStream(filePath, FileMode.Create));
        //        //}

        //        //else
        //        //    if (_oChanges.Id != 0) uniqueFileName = _oChanges.UserPhoto;

        //        //_oChanges.UserPhoto = uniqueFileName;



        //        var tm = DateTime.Now;
        //        _oChanges.LastMod = tm;
        //        _oChanges.LastModByUserId = vmMod.oUserId_Logged;
        //        _oChanges.ReceivedByUserId = vmMod.oUserId_Logged;

        //        //validate...
        //        if (_oChanges.Id == 0)
        //        {
        //            _oChanges.Created = tm;
        //            _oChanges.CreatedByUserId = vmMod.oUserId_Logged;
        //            _context.Add(_oChanges);

        //            ViewBag.UserMsg = "Saved tithe data successfully.";
        //        }
        //        else
        //        {
        //            //retain the pwd details... hidden fields 
        //            _context.Update(_oChanges);
        //            ViewBag.UserMsg = "User tithe data updated successfully.";
        //        }

        //        //save user profile first... 
        //        _context.SaveChanges();  // await ... Async();

        //        /// update the tithe balance summary table... when new m added or triggered by user or at refresh 
        //        /// add new, upd [stat, dob, gen, group ... upd roll] and delete /transfer
        //        // if (_reset) {
        //        var resRollUpd = UpdCBTitheBal(_oChanges.AppGlobalOwnerId, _oChanges.ChurchBodyId, this._oLoggedUser.Id, _oChanges.Curr3LISOSymbol, _oChanges.CtryAlpha3Code);
        //        if (resRollUpd < 0) ViewBag.UserMsg += ". Tithe balance summary update failed. Try update again later.";
        //        else if (resRollUpd == 0) ViewBag.UserMsg += ". Tithe balance summary update incomplete. Try update again later.";
        //        // }

        //        var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
        //        TempData["oVmCurrMod"] = _vmMod; TempData.Keep();

        //        return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, resetNew = _reset, userMess = ViewBag.UserMsg, lastCodeUsed = _oChanges.Curr3LISOSymbol });
        //    }

        //    catch (Exception ex)
        //    {
        //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving tithe details. Err: " + ex.Message });
        //    }

        //}






        //[HttpGet]
        //public IActionResult AddOrEdit_MemTransf(int? oCurrChuBodyId, int id = 0)  //, ChurchTransferDetail_MDL oCurrMdl = null)
        //{
        //    try
        //    {
        //        if (this._context == null)
        //        {
        //            this._context = GetClientDBContext();
        //            if (this._context == null)
        //            {
        //                RedirectToAction("LoginUserAcc", "UserLogin");

        //                // should not get here... Response.StatusCode = 500; 
        //                return View("_ErrorPage");
        //            }
        //        }


        //        //if (!InitializeUserLogging())
        //        //    return RedirectToAction("LoginUserAcc", "UserLogin");

        //        // Client
        //        var oAppGloOwnId = this._oLoggedAGO.Id;
        //        if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

        //        // MSTR
        //        var oUserId = this._oLoggedUser.Id;
        //        var oAGO_MSTR = this._oLoggedAGO_MSTR; var oCB_MSTR = this._oLoggedCB_MSTR; // _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId); 
        //        var oAGO = this._oLoggedAGO;
        //        var oCB = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
        //        if (oCB == null) oCB = this._oLoggedCB;

        //        if (oAGO_MSTR == null || oCB_MSTR == null || oAGO == null || oCB == null)   // || oCU_Parent == null church units may be networked...
        //        { return View("_ErrorPage"); }

        //        var strDesc = "Church Member";
        //        var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();



        //        var oCTModel = new ChurchTransferModel();

        //        //var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
        //        //                    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

        //        //oCP_List_1 = oCP_List_1.Where(c =>
        //        //                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
        //        //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
        //        //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

        //        if (id == 0)
        //        {
        //            var oTransTITHE = new TitheTrans();

        //            // this attr is used by most of the models... save in memory [class var]
        //            this.oCTRYDefault = _context.CountryCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsDefaultCountry == true).FirstOrDefault();
        //            this.oCURRDefault = _context.CurrencyCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsBaseCurrency == true).FirstOrDefault();
        //            ///
        //            var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody).ThenInclude(t => t.ChurchLevel) //
        //                            .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

        //            oCP_List_1 = oCP_List_1.Where(c =>
        //                               (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
        //                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
        //                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

        //            /// may be more... pick the one up-most ... override lower cong!    private ChurchPeriod oCPRDefault;
        //            oCP_List_1 = oCP_List_1.OrderBy(c => c.OwnedByChurchBody?.ChurchLevel?.LevelIndex).ToList();
        //            this.oCPRDefault = oCP_List_1.FirstOrDefault();// _context.ChurchPeriod.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.Status == "A").FirstOrDefault();  // c.PeriodType == "CP" && 

        //            ///
        //            oTransTITHE.AppGlobalOwnerId = oAppGloOwnId;
        //            oTransTITHE.ChurchBodyId = oCurrChuBodyId;

        //            oTransTITHE.ChurchPeriodId = this.oCPRDefault != null ? this.oCPRDefault.Id : (int?)null; // accPer != null ? accPer.Id : (int?)null;
        //                                                                                                      //var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsBaseCurrency == true).FirstOrDefault();
        //            ///
        //            oTransTITHE.Curr3LISOSymbol = this.oCURRDefault != null ? this.oCURRDefault.Country.Curr3LISOSymbol : null; // curr != null ? curr.Id : (int?)null;
        //            oTransTITHE.CtryAlpha3Code = this.oCURRDefault != null ? this.oCURRDefault.Country.CtryAlpha3Code : null; // curr != null ? curr.Id : (int?)null;
        //            oTransTITHE.TithedByScope = "L";     // Local cong            
        //            oTransTITHE.TitheMode = "A";     //  Anonymous           
        //            oTransTITHE.PostStatus = "O";     // O-Open, P-Posted to GL, R-Reversed            
        //            oTransTITHE.Status = "A"; //Active 
        //            oTransTITHE.TitheDate = DateTime.Now;
        //            oTransTITHE.ReceivedByUserId = oUserId_Logged;
        //            ///
        //            oCTModel.strCurrency = this.oCURRDefault != null ? (this.oCURRDefault.Country != null ? this.oCURRDefault.Country.Curr3LISOSymbol : "") : ""; // curr != null ? curr.Acronym : "";
        //            oCTModel.strPostStatus = GetPostStatusDesc(oTransTITHE.PostStatus);
        //            if (oTransTITHE != null)
        //            {
        //                var oUser = _masterContext.UserProfile.Where(c => c.Id == oTransTITHE.ReceivedByUserId).FirstOrDefault();
        //                if (oUser != null) oCTModel.strReceivedBy = oUser.UserDesc;
        //            }

        //            oCTModel.oTitheTrans = oTransTITHE;
        //        }

        //        else
        //        {
        //            var TitheMdl = (
        //               from t_tt in _context.TitheTrans.AsNoTracking() //.Include(t => t.ChurchMember)
        //                            .Where(c => c.Id == id && c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId)
        //               from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
        //                        .Where(c => c.Id == t_tt.ChurchBodyId && c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId)   //c.Id == oChurchBodyId && 
        //               from t_ap in _context.ChurchPeriod.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId && c.Id == t_tt.ChurchPeriodId).DefaultIfEmpty()   //_context.ChurchPeriod.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.AccountPeriodId) .DefaultIfEmpty()  //c.Id == oChurchBodyId &&   // from t_an in _context.AppUtilityNVP.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.TitheModeId).DefaultIfEmpty()
        //               from t_cb_corp in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
        //                        .Where(c => c.AppGlobalOwnerId == t_tt.AppGlobalOwnerId && c.Id == t_tt.Corporate_ChurchBodyId).DefaultIfEmpty()
        //               from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.ChurchMemberId).DefaultIfEmpty()
        //                   // from t_curr in _context.Currency.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_tt.CurrencyId).DefaultIfEmpty()

        //               select new ChurchTransferModel()
        //               {
        //                   oTitheTrans = t_tt,
        //                   oAppGlolOwnId = t_tt.AppGlobalOwnerId,
        //                   oAppGlolOwn = t_cb.AppGlobalOwner,
        //                   oChurchBodyId = t_tt.ChurchBodyId,
        //                   oChurchBody = t_cb,
        //                   //                      
        //                   strTithedBy = t_tt.TitheMode == "M" ? (t_cm != null ? StaticGetConcatMemberName(t_cm.Title, t_cm.FirstName, t_cm.MiddleName, t_cm.LastName, true) : t_tt.TitherDesc) :
        //                                                                    (t_tt.TitheMode == "C" ? (t_cb_corp != null ? t_cb_corp.Name : t_tt.TitherDesc) : t_tt.TitherDesc),
        //                   strTitheMode = GetTitheModeDesc(t_tt.TitheMode),
        //                   strTitherScope = GetTitherScopeDesc(t_tt.TithedByScope),
        //                   // strRelatedEvent = t_cce != null ? t_cce.Subject : "",
        //                   strAccountPeriod = t_ap != null ? t_ap.PeriodDesc : "",
        //                   strCurrency = t_tt.Curr3LISOSymbol, // t_curr != null ? t_curr.Acronym : "",
        //                                                       //
        //                   strAmount = String.Format("{0:N2}", t_tt.TitheAmount),
        //                   dt_TitheDate = t_tt.TitheDate,
        //                   strTitheDate = t_tt.TitheDate != null ? DateTime.Parse(t_tt.TitheDate.ToString()).ToString("ddd, dd MMM yyyy", CultureInfo.InvariantCulture) : "",
        //                   strPostDate = t_tt.PostedDate != null ? DateTime.Parse(t_tt.PostedDate.ToString()).ToString("ddd, dd MMM yyyy", CultureInfo.InvariantCulture) : "",
        //                   strPostStatus = GetPostStatusDesc(t_tt.PostStatus),
        //                   strStatus = GetPostStatusDesc(t_tt.Status)
        //               }
        //               ).FirstOrDefault();

        //            if (TitheMdl != null)
        //                if (TitheMdl.oTitheTrans != null)
        //                {
        //                    var oUser = _masterContext.UserProfile.Where(c => c.Id == TitheMdl.oTitheTrans.ReceivedByUserId).FirstOrDefault();
        //                    if (oUser != null) TitheMdl.strReceivedBy = oUser.UserDesc;
        //                }

        //            oCTModel = TitheMdl;
        //        }



        //        oCTModel.setIndex = setIndex;
        //        oCTModel.oUserId_Logged = oUserId_Logged;
        //        oCTModel.oAppGloOwnId_Logged = oAGOId_Logged;
        //        oCTModel.oChurchBodyId_Logged = oCBId_Logged;
        //        //
        //        oCTModel.oAppGloOwnId = oAppGloOwnId;
        //        oCTModel.oChurchBodyId = oCurrChuBodyId;
        //        var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
        //        oCTModel.oChurchBody = oCurrChuBody;  // != null ? oCurrChuBody : null;


        //        // ChurchBody oChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
        //        oCTModel = this.populateLookups_CT(oCTModel, oCurrChuBody);
        //        // oCurrMdl.strCurrTask = "Church Tithe";

        //        var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oCTModel);
        //        TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

        //        return PartialView("_AddOrEdit_CT", oCTModel);

        //    }
        //    catch (Exception ex)
        //    {
        //        return PartialView("_ErrorPage");
        //    }





        //    //var oChuTransf_VM = new ChurchTransferDetail_MDL();
        //    SetUserLogged();
        //    if (!isCurrValid) //prompt!
        //        return RedirectToAction("LoginUserAcc", "UserLogin");
        //    else
        //    {
        //        var oMemTransf_MDL = new ChurchTransferModel(); // _context, oCurrChuBodyLogOn);  //use this option so all lookups are loaded same time alongside.... from the class
        //        //
        //        var oCurrChuBodyLogOn = oUserLogIn_Priv[0].ChurchBody;
        //        var oUserProfile = oUserLogIn_Priv[0].UserProfile;
        //        if (oCurrChuBodyLogOn == null) //prompt!
        //            return View(oMemTransf_MDL); // PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL); //return View(oMemTransf_MDL);
        //        if (oCurrChuBodyId == null) oCurrChuBodyId = oCurrChuBodyLogOn.Id;
        //        else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...

        //        // check permission for Core life...
        //        if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) //prompt!
        //            return View(oMemTransf_MDL); //PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL); //return View(oMemTransf_MDL);

        //        int? oCurrChuMemberId_LogOn = null;
        //        ChurchMember oCurrChuMember_LogOn = null;
        //        if (oUserProfile == null) //prompt!
        //            return View(oMemTransf_MDL); //PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL); //return View(oMemTransf_MDL);
        //        if (oUserProfile.ChurchMember == null) //prompt!
        //            return View(oMemTransf_MDL); //PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL); //return View(oMemTransf_MDL);

        //        oCurrChuMemberId_LogOn = oUserProfile.ChurchMember.Id;
        //        oCurrChuMember_LogOn = oUserProfile.ChurchMember;

        //        //check for pending.. unClosed requests OR successful closed requests... check again! not logged user but applicant
        //        var currTransf = (from t_cm in _context.ChurchTransfer
        //                          .Where(x => x.RequestorChurchBodyId == oCurrChuBodyId && x.ChurchMemberId == oCurrChuMemberId_LogOn &&
        //                         (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.Status != "U"))
        //                          select t_cm).ToList();
        //        ViewBag.IsPendingTransfer = currTransf.Count > 0;

        //        //if (currTransf.Count > 0)
        //        //{
        //        //    ViewBag.IsPendingTransfer
        //        //   // return Json(new { taskSuccess = false, userMess = "Member transfer already initiated or done for specified member." });
        //        //}

        //        // oMemTransf_MDL.oChurchBody = oCurrChuBodyLogOn;   //ToCongregation can never call this fxn... cannot create or send request...
        //        if (id == 0)
        //        {  //new MemberTransfer_VM(_context, oCurrChuBodyLogOn);

        //            //check for pending.. unClosed requests
        //            //var currTransf = (from t_cm in _context.ChurchTransfer.Where(x => x.RequestorChurchBodyId==oCurrChuBodyId && x.ChurchMemberId==oCurrChuMemberId_LogOn &&
        //            //                 x.ReqStatus != "C" && x.ReqStatus != "R" && x.ReqStatus != "X") select t_cm).ToList();

        //            //if (currTransf.Count > 0)//prompt! ...complete pending requests before NEW*
        //            //      return View(oMemTransf_MDL); //PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL);

        //            var memFromInfo = (
        //                from t_cm in _context.ChurchMember.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.Id == oCurrChuMemberId_LogOn)
        //                from t_mp in _context.MemberPosition.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.CurrentPos == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty()  // from t_mp in abcd
        //                from t_ms in _context.MemberStatus.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.IsCurrent == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty() // from t_ms in abcde
        //                from t_mcs in _context.MemberChurchSector.Include(t => t.Sector).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.CurrSector == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty()
        //                from t_cs in _context.ChurchSector.Where(x => x.Id == t_mcs.SectorId && x.ChurchBodyId == oCurrChuBodyId && x.Generational == true).DefaultIfEmpty()    // on t_mcs.SectorId equals t_cs.Id into temp_t_mcs
        //                                                                                                                                                                        //from t_mcsx in temp_t_mcs.DefaultIfEmpty()          
        //                from t_mlr in _context.MemberLeaderRole.Include(t => t.LeaderRole).Where(x => x.IsCoreRole == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty()  // from t_mlr in abcdefg  //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)

        //                select new ChurchTransferDetail_VM
        //                {
        //                    oChurchMemberTransf = t_cm,  // strRequestorFullName = (!string.IsNullOrEmpty(t_cm.MemberCode) ? t_cm.MemberCode + " - " : "") + ((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : ""),
        //                    strFromMemberFullName = (!string.IsNullOrEmpty(t_cm.MemberGlobalId) ? t_cm.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
        //                    strFromMemberPos = t_mp.ChurchPosition != null ? t_mp.ChurchPosition.PositionName : "",
        //                    strFromMemberStatus = t_ms.ChurchMemStatus != null ? t_ms.ChurchMemStatus.Name : "",
        //                    strFromMemberAgeGroup = t_cs != null ? t_cs.Name : "",
        //                    strFromMemberRole = t_mlr.LeaderRole != null ? (t_mlr.LeaderRole.RoleName + (t_mlr.ChurchUnit != null ? ", " + t_mlr.ChurchUnit.Name : "")) : "",  //strFromMemberRole = t_mlr.LeaderRole != null ? t_mlr.LeaderRole.RoleName : "",
        //                    numFromChurchPositionId = t_mp != null ? t_mp.ChurchPositionId : -1,
        //                    numFromMemberLeaderRoleId = t_mlr != null ? t_mlr.Id : -1  // strMemberFullName = t.MemberCode + ' ' + (((t.FirstName + ' ' + t.MiddleName).Trim() + " " + t.LastName).Trim() + " " + (!string.IsNullOrEmpty(t.Title) ? "(" + t.Title + ")" : "")).Trim(),                                
        //                }
        //                   ).FirstOrDefault();

        //            if (memFromInfo == null)   //prompt!
        //                return View(oMemTransf_MDL); //PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL); //return View(oMemTransf_MDL); member to transfer --details-- not found

        //            //get requestor detail
        //            var oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchUnit)
        //                    .Where(c => c.ChurchBodyId == oCurrChuBodyId && c.ChurchMemberId == oCurrChuMemberId_LogOn && c.IsCoreRole == true).Take(1).FirstOrDefault();
        //            oMemTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCurrChuMember_LogOn.MemberGlobalId) ? oCurrChuMember_LogOn.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(oCurrChuMember_LogOn.Title) ? oCurrChuMember_LogOn.Title : "") + ' ' + oCurrChuMember_LogOn.FirstName).Trim() + " " + oCurrChuMember_LogOn.MiddleName).Trim() + " " + oCurrChuMember_LogOn.LastName).Trim();
        //            if (oReqMLR != null)
        //                oMemTransf_MDL.strRequestorFullName += oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.ChurchUnit != null ? ", " + oReqMLR.ChurchUnit.Name : "") + ")" : "";
        //            // oReqMLR != null ? (oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.RoleSector != null ? ", " + oReqMLR.RoleSector.Name : "") + ")" : "") : "";


        //            //_context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchMember)
        //            //        .Where(x => x.ChurchMemberId == oCurrChuMemberId_LogOn && x.CoreRole == true).Take(1).FirstOrDefault();

        //            //if (oMemTransf_MDL.oChurchTransfer != null)
        //            //{
        //            //    var oCTR = oMemTransf_MDL.oChurchTransfer;
        //            //    if (oCTR.RequestorChurchMember != null)
        //            //        oMemTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCTR.RequestorChurchMember.MemberCode) ? oCTR.RequestorChurchMember.MemberCode + " - " : "") +
        //            //            ((((!string.IsNullOrEmpty(oCTR.RequestorChurchMember.Title) ? oCTR.RequestorChurchMember.Title : "") + ' ' + oCTR.RequestorChurchMember.FirstName).Trim() + " " + oCTR.RequestorChurchMember.MiddleName).Trim() + " " + oCTR.RequestorChurchMember.LastName).Trim();
        //            //    //oCTR.RequestorChurchMember.MemberCode + " - " + ((oCTR.RequestorChurchMember.FirstName + ' ' + oCTR.RequestorChurchMember.MiddleName).Trim() + " " + oCTR.RequestorChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCTR.RequestorChurchMember.Title) ? "(" + oCTR.RequestorChurchMember.Title + ")" : "");

        //            //    if (oCTR.RequestorRole != null)
        //            //    {
        //            //        //if (oCTR.RequestorRole.ChurchMember != null)
        //            //        //    oMemTransf_MDL.strRequestorFullName = oCTR.RequestorRole.ChurchMember.MemberCode + " - " + ((oCTR.RequestorRole.ChurchMember.FirstName + ' ' + oCTR.RequestorRole.ChurchMember.MiddleName).Trim() + " " + oCTR.RequestorRole.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCTR.RequestorRole.ChurchMember.Title) ? "(" + oCTR.RequestorRole.ChurchMember.Title + ")" : "");

        //            //        //if (oCTR.RequestorRole.LeaderRole != null )
        //            //        //{
        //            //            oMemTransf_MDL.strRequestorFullName += " (";
        //            //            oMemTransf_MDL.strRequestorFullName += oCTR.RequestorRole.LeaderRole.RoleName + (oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "") ;
        //            //            //if (oCTR.RequestorRole.LeaderRole != null) oMemTransf_MDL.strRequestorFullName += (oCTR.RequestorRole.LeaderRole != null ? oCTR.RequestorRole.LeaderRole.RoleName : "");
        //            //            //if (oCTR.RequestorRole.RoleSector != null)  oMemTransf_MDL.strRequestorFullName += oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "";
        //            //            oMemTransf_MDL.strRequestorFullName += ")";
        //            //        //}
        //            //        //oMemTransf_MDL.strRequestorFullName = oCTR.RequestorRole.ChurchMember.MemberCode + " - " + (((oCTR.RequestorRole.ChurchMember.FirstName + ' ' + oCTR.RequestorRole.ChurchMember.MiddleName).Trim() + " " + oCTR.RequestorRole.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCTR.RequestorRole.ChurchMember.Title) ? "(" + oCTR.RequestorRole.ChurchMember.Title + ")" : "") + " " + (oCTR.RequestorRole.LeaderRole != null ? " | " + oCTR.RequestorRole.LeaderRole.RoleName + (oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "") + "" : "")).Trim();
        //            //    }
        //            //}

        //            //if (oCTR.RequestorRole.LeaderRole != null || oCTR.RequestorRole.RoleSector != null)
        //            //{
        //            //    oMemTransf_MDL.strRequestorFullName += " (";
        //            //    if (oCTR.RequestorRole.LeaderRole != null)
        //            //        oMemTransf_MDL.strRequestorFullName += (oCTR.RequestorRole.LeaderRole != null ? oCTR.RequestorRole.LeaderRole.RoleName : "");
        //            //    if (oCTR.RequestorRole.RoleSector != null)
        //            //        oMemTransf_MDL.strRequestorFullName += oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "";
        //            //    oMemTransf_MDL.strRequestorFullName += ")";
        //            //}

        //            //int? requestMLRId = null;
        //            //MemberLeaderRole oReqMLR = null;
        //            //if (oCurrChuMemberId_LogOn != null)
        //            //{   //MemberLeaderRole reqRole = null;
        //            //    oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchMember)
        //            //        .Where(x => x.ChurchMemberId == oCurrChuMemberId_LogOn && x.CoreRole == true).Take(1).FirstOrDefault();
        //            //    if (oReqMLR != null) requestMLRId = ((MemberLeaderRole)oReqMLR).Id;  //.LeaderRoleId;
        //            //}

        //            //create user and init... 
        //            oMemTransf_MDL.oChurchTransfer = new ChurchTransfer();
        //            oMemTransf_MDL.oChurchTransfer.ChurchMemberId = memFromInfo.oChurchMemberTransf.Id;
        //            oMemTransf_MDL.oChurchTransfer.RequestorMemberId = oCurrChuMemberId_LogOn;
        //            oMemTransf_MDL.oChurchTransfer.RequestorRoleId = oReqMLR != null ? oReqMLR.Id : (int?)null; //requestMLRId; 
        //            oMemTransf_MDL.oChurchTransfer.RequestorChurchBodyId = oCurrChuBodyId;
        //            oMemTransf_MDL.oChurchTransfer.FromChurchBodyId = oCurrChuBodyId;
        //            //oMemTransf_MDL.oChurchTransfer.FromChurchPositionId = null;
        //            //oMemTransf_MDL.oChurchTransfer.FromMemberLeaderRoleId  = null;
        //            oMemTransf_MDL.oChurchTransfer.CurrentScope = "I";  //Internal cong. submission first
        //            oMemTransf_MDL.oChurchTransfer.ReqStatus = "N"; //NEW... DRAFT 
        //            oMemTransf_MDL.oChurchTransfer.Status = null; //NEW... DRAFT
        //            oMemTransf_MDL.oChurchTransfer.ApprovalStatus = null;
        //            oMemTransf_MDL.oChurchTransfer.TransferType = "MT";
        //            oMemTransf_MDL.oChurchTransfer.RequestDate = DateTime.Now;
        //            // oMemTransf_MDL.oChurchTransfer.TransferType  = "MT";  //Member Transfer... done at definition

        //            //view purpose....
        //            oMemTransf_MDL.oChurchTransfer.RequestorChurchBody = oCurrChuBodyLogOn;
        //            oMemTransf_MDL.oChurchTransfer.RequestorChurchMember = oCurrChuMember_LogOn;
        //            oMemTransf_MDL.oChurchTransfer.FromChurchBody = oCurrChuBodyLogOn;
        //            //
        //            oMemTransf_MDL.oAppGlobalOwner = oCurrChuBodyLogOn.AppGlobalOwner;
        //            oMemTransf_MDL.oChurchBody = oCurrChuBodyLogOn;
        //            oMemTransf_MDL.oRequestorChurchBody = oCurrChuBodyLogOn;
        //            oMemTransf_MDL.oCurrLoggedMember = oCurrChuMember_LogOn;
        //            oMemTransf_MDL.oCurrLoggedMemberId = oCurrChuMemberId_LogOn;

        //            // oMemTransf_MDL.strRequestorFullName = oReqMLR.ChurchMember.MemberCode + " - " + (((oReqMLR.ChurchMember.FirstName + ' ' + oReqMLR.ChurchMember.MiddleName).Trim() + " " + oReqMLR.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oReqMLR.ChurchMember.Title) ? "(" + oReqMLR.ChurchMember.Title + ")" : "") + " " + (oReqMLR.LeaderRole != null ? " | " + oReqMLR.LeaderRole.RoleName + (oReqMLR.RoleSector != null ? ", " + oReqMLR.RoleSector.Name : "") + "" : "")).Trim();

        //            //if (oReqMLR != null)
        //            //{
        //            //   // var oCTR = oMemTransf_MDL.oChurchTransfer;
        //            //    if (oReqMLR.ChurchMember != null)
        //            //        oMemTransf_MDL.strRequestorFullName = oReqMLR.ChurchMember.MemberCode + " - " + ((oReqMLR.ChurchMember.FirstName + ' ' + oReqMLR.ChurchMember.MiddleName).Trim() + " " + oReqMLR.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oReqMLR.ChurchMember.Title) ? "(" + oReqMLR.ChurchMember.Title + ")" : "");
        //            //    if (oReqMLR.LeaderRole != null)
        //            //        oMemTransf_MDL.strRequestorFullName += " " + (oReqMLR.LeaderRole != null ? " | " + oReqMLR.LeaderRole.RoleName : "");
        //            //    if (oReqMLR.RoleSector != null)
        //            //        oMemTransf_MDL.strRequestorFullName += oReqMLR.RoleSector != null ? ", " + oReqMLR.RoleSector.Name : "";
        //            //  }

        //            //  var jsonData = GetTransferMemberDetail((int)oCurrChuMemberId_LogOn, (int)oCurrChuBodyId); 



        //            // 
        //            oMemTransf_MDL.strFromMemberFullName = memFromInfo.strFromMemberFullName;
        //            oMemTransf_MDL.strRequestorFullName = memFromInfo.strFromMemberFullName;
        //            oMemTransf_MDL.strFromMemberPos = memFromInfo.strFromMemberPos;
        //            oMemTransf_MDL.strFromMemberStatus = memFromInfo.strFromMemberStatus;
        //            oMemTransf_MDL.strFromMemberAgeGroup = memFromInfo.strFromMemberAgeGroup;
        //            oMemTransf_MDL.strFromMemberRole = memFromInfo.strFromMemberRole;
        //            oMemTransf_MDL.strRequestorRole = memFromInfo.strFromMemberRole;

        //            //
        //            oMemTransf_MDL.strRequestorChurchBody = oCurrChuBodyLogOn.Name;
        //            oMemTransf_MDL.strRequestorChurchLevel = oCurrChuBodyLogOn.ChurchLevel.CustomName;
        //            oMemTransf_MDL.strFromChurchBody = oMemTransf_MDL.strRequestorChurchBody;
        //            oMemTransf_MDL.strFromChurchLevel = oMemTransf_MDL.strRequestorChurchLevel;
        //            // oMemTransf_MDL.strRequestorRole = oReqMLR != null ? oReqMLR.LeaderRole.RoleName : "";
        //            oMemTransf_MDL.strCurrScope = oMemTransf_MDL.oChurchTransfer.CurrentScope.Equals("E") ? "External" : "Internal";

        //            //oMemTransf_MDL.strToChurchLevel_1 = "";
        //            //oMemTransf_MDL.strToChurchLevel_2 = "";
        //            //oMemTransf_MDL.strToChurchLevel_3 = "";
        //            //oMemTransf_MDL.strToChurchLevel_4 = "";
        //            //oMemTransf_MDL.strToChurchLevel_5 = "";
        //            //oMemTransf_MDL.strToChurchLevel_6 = "";
        //            //oMemTransf_MDL.strToChurchLevel_7 = "";                    
        //            //oMemTransf_MDL.ToChurchBodyId_Categ1 = null;
        //            //oMemTransf_MDL.ToChurchBodyId_Categ2 = null;
        //            //oMemTransf_MDL.ToChurchBodyId_Categ3 = null;
        //            //oMemTransf_MDL.ToChurchBodyId_Categ4 = null;
        //            //oMemTransf_MDL.ToChurchBodyId_Categ5 = null;
        //            //oMemTransf_MDL.ToChurchBodyId_Categ6 = null;
        //            //oMemTransf_MDL.ToChurchBodyId_Categ7 = null;                     
        //        }

        //        else
        //        {  // oMemTransf_MDL = new MemberTransfer_VM(_context, oCurrChuBodyLogOn);

        //            oMemTransf_MDL = (
        //                 from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.ChurchMemberTransf).Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
        //                 .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorChurchMember)
        //                     .Where(x => x.Id == id && x.RequestorChurchBodyId == oCurrChuBodyId) // && (showOwned==false || (showOwned==true && x.RequestorMemberId == oCurrLoggedUser_MemberId)))  //&& x.CurrentScope == "I" 
        //                 from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.ChurchMemberId == x.Id)
        //                 from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.RequestorMemberId == x.Id)
        //                 from t_mlr_r in _context.MemberLeaderRole.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.ChurchUnit).Include(t => t.LeaderRole)
        //                                .Where(x => x.ChurchBodyId == oCurrChuBodyId && x.Id == t_ct.RequestorRoleId).DefaultIfEmpty()
        //                 from t_cb_r in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel).Where(x => x.Id == t_ct.RequestorChurchBodyId) //.DefaultIfEmpty()
        //                 from t_cb_f in _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(x => x.Id == t_ct.FromChurchBodyId).DefaultIfEmpty()
        //                 from t_cb_t in _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(x => x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
        //                 from t_mp in _context.MemberPosition.AsNoTracking().Include(t => t.ChurchPosition).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.ChurchMemberId == t_cm.Id && x.CurrentPos == true).DefaultIfEmpty()
        //                 from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchMemStatus).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.ChurchMemberId == t_cm.Id && x.IsCurrent == true).DefaultIfEmpty()
        //                 from t_mcs in _context.MemberChurchSector.AsNoTracking().Include(t => t.Sector).Where(x => x.ChurchBodyId == t_cm.ChurchBodyId && x.CurrSector == true && t_cm.Id == x.ChurchMemberId).Take(1).DefaultIfEmpty() // age bracket/group: children, youth, adults as defined in config. OR auto-assign
        //                                                                                                                                                                                                                                 // from t_cs in _context.ChurchSector.Where(x=> x.ChurchBodyId == t_mcs.ChurchBodyId && x.Id==t_mcs.SectorId && x.Generational == true).DefaultIfEmpty()
        //                                                                                                                                                                                                                                 // join t_mcsx in _context.MemberChurchSector.Include(t=>t.Sector).Where(x => x.Sector.Generational == true && x.CurrSector == true) on t.Id equals t_mcsx.ChurchMemberId into abcdef
        //                 from t_mlr in _context.MemberLeaderRole.AsNoTracking().Include(t => t.LeaderRole).Where(x => x.ChurchMemberId == t_cm.Id && x.IsCoreRole == true).DefaultIfEmpty()   //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)

        //                 select new ChurchTransferModel()
        //                 {
        //                     oCurrLoggedMember = oCurrChuMember_LogOn,
        //                     oCurrLoggedMemberId = oCurrChuMemberId_LogOn,
        //                     oAppGlobalOwner = oCurrChuBodyLogOn.AppGlobalOwner,

        //                     oChurchTransfer = t_ct,
        //                     oRequestorChurchBody = t_cb_r,
        //                     oChurchBody = t_cb_r,
        //                     numTransferDxn = t_cb_r.Id == oCurrChuBodyLogOn.Id ? 2 : t_cb_t.Id == oCurrChuBodyLogOn.Id ? 1 : 0,   //out, in
        //                     strTransferDxn = oMemTransf_MDL.numTransferDxn == 1 ? "Incoming" : oMemTransf_MDL.numTransferDxn == 2 ? "Outgoing" : "",
        //                     strFromChurchBody = t_cb_f.Name,
        //                     strFromChurchLevel = t_cb_f.ChurchLevel.CustomName,
        //                     strRequestorRole = t_mlr_r.LeaderRole != null ? t_mlr_r.LeaderRole.RoleName : "",
        //                     strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),
        //                     strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
        //                     strCurrScope = t_ct.CurrentScope.Equals("E") ? "External" : "Internal",
        //                     strRequestorFullName = (!string.IsNullOrEmpty(t_cm_r.MemberGlobalId) ? t_cm_r.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm_r.Title) ? t_cm_r.Title : "") + ' ' + t_cm_r.FirstName).Trim() + " " + t_cm_r.MiddleName).Trim() + " " + t_cm_r.LastName).Trim(),
        //                     //                          t_mlr_r.LeaderRole != null ? " (" + t_mlr_r.LeaderRole.RoleName + (t_mlr_r.RoleSector != null ? ", " + t_mlr_r.RoleSector.Name : "")+")" : "", 
        //                     strFromMemberPos = t_mp.ChurchPosition != null ? t_mp.ChurchPosition.PositionName : "",
        //                     strFromMemberStatus = t_ms.ChurchMemStatus != null ? t_ms.ChurchMemStatus.Name : "",
        //                     strFromMemberAgeGroup = t_mcs.Sector != null ? t_mcs.Sector.Name : "",
        //                     strFromMemberRole = t_mlr.LeaderRole != null ? t_mlr.LeaderRole.RoleName : "",
        //                     strFromMemberFullName = (!string.IsNullOrEmpty(t_cm.MemberGlobalId) ? t_cm.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
        //                     // strFromMemberFullName = t_cm.MemberCode + " - " + ((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : ""),
        //                     //numFromChurchPositionId = t_mp != null ? t_mp.ChurchPositionId : -1,
        //                     //numFromMemberLeaderRoleId = t_mlr != null ? t_mlr.Id : -1
        //                 }
        //                ).FirstOrDefault();

        //            if (oMemTransf_MDL == null) return View(oMemTransf_MDL); //

        //            //get requestor detail
        //            var oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchUnit)
        //                    .Where(c => c.ChurchBodyId == oCurrChuBodyId && c.ChurchMemberId == oMemTransf_MDL.oChurchTransfer.RequestorMemberId
        //                        && c.LeaderRoleId == oMemTransf_MDL.oChurchTransfer.RequestorRoleId).FirstOrDefault();
        //            // oMemTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCurrChuMember_LogOn.MemberGlobalId) ? oCurrChuMember_LogOn.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(oCurrChuMember_LogOn.Title) ? oCurrChuMember_LogOn.Title : "") + ' ' + oCurrChuMember_LogOn.FirstName).Trim() + " " + oCurrChuMember_LogOn.MiddleName).Trim() + " " + oCurrChuMember_LogOn.LastName).Trim();
        //            if (oReqMLR != null)
        //                oMemTransf_MDL.strRequestorFullName += oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.ChurchUnit != null ? ", " + oReqMLR.ChurchUnit.Name : "") + ")" : "";


        //            //if (oMemTransf_MDL != null)
        //            //{
        //            //  oMemTransf_MDL.strTransferDxn = oMemTransf_MDL.numTransferDxn == 1 ? "Incoming" : oMemTransf_MDL.numTransferDxn == 2 ? "Outgoing" : "";
        //            // oMemTransf_MDL.strRequestorFullName = oMemTransf_MDL.strFromMemberFullName;

        //            //
        //            //if (oMemTransf_MDL.oChurchTransfer.RequestorChurchBody == null) oMemTransf_MDL.oChurchTransfer.RequestorChurchBody = oMemTransf_MDL.oRequestorChurchBody;
        //            //if (oMemTransf_MDL.oChurchTransfer.RequestorChurchMember == null ) oMemTransf_MDL.oChurchTransfer.RequestorChurchMember = oMemTransf_MDL.oCurrLoggedMember;

        //            if (oMemTransf_MDL.oChurchTransfer == null) return View(oMemTransf_MDL); //

        //            //if (oMemTransf_MDL.oChurchTransfer != null)
        //            //{
        //            //var oCTR = oMemTransf_MDL.oChurchTransfer;
        //            //if (oCTR.RequestorChurchMember != null)
        //            //    oMemTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCTR.RequestorChurchMember.MemberCode) ? oCTR.RequestorChurchMember.MemberCode + " - " : "") +
        //            //        ((((!string.IsNullOrEmpty(oCTR.RequestorChurchMember.Title) ? oCTR.RequestorChurchMember.Title : "") + ' ' + oCTR.RequestorChurchMember.FirstName).Trim() + " " + oCTR.RequestorChurchMember.MiddleName).Trim() + " " + oCTR.RequestorChurchMember.LastName).Trim();
        //            ////oCTR.RequestorChurchMember.MemberCode + " - " + ((oCTR.RequestorChurchMember.FirstName + ' ' + oCTR.RequestorChurchMember.MiddleName).Trim() + " " + oCTR.RequestorChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCTR.RequestorChurchMember.Title) ? "(" + oCTR.RequestorChurchMember.Title + ")" : "");

        //            //if (oCTR.RequestorRole != null)
        //            //{
        //            //    //if (oCTR.RequestorRole.ChurchMember != null)
        //            //    //    oMemTransf_MDL.strRequestorFullName = oCTR.RequestorRole.ChurchMember.MemberCode + " - " + ((oCTR.RequestorRole.ChurchMember.FirstName + ' ' + oCTR.RequestorRole.ChurchMember.MiddleName).Trim() + " " + oCTR.RequestorRole.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCTR.RequestorRole.ChurchMember.Title) ? "(" + oCTR.RequestorRole.ChurchMember.Title + ")" : "");

        //            //    //if (oCTR.RequestorRole.LeaderRole != null )
        //            //    //{
        //            //    oMemTransf_MDL.strRequestorFullName += " (";
        //            //    oMemTransf_MDL.strRequestorFullName += oCTR.RequestorRole.LeaderRole.RoleName + (oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "");
        //            //    //if (oCTR.RequestorRole.LeaderRole != null) oMemTransf_MDL.strRequestorFullName += (oCTR.RequestorRole.LeaderRole != null ? oCTR.RequestorRole.LeaderRole.RoleName : "");
        //            //    //if (oCTR.RequestorRole.RoleSector != null)  oMemTransf_MDL.strRequestorFullName += oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "";
        //            //    oMemTransf_MDL.strRequestorFullName += ")";
        //            //    //}
        //            //    //oMemTransf_MDL.strRequestorFullName = oCTR.RequestorRole.ChurchMember.MemberCode + " - " + (((oCTR.RequestorRole.ChurchMember.FirstName + ' ' + oCTR.RequestorRole.ChurchMember.MiddleName).Trim() + " " + oCTR.RequestorRole.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCTR.RequestorRole.ChurchMember.Title) ? "(" + oCTR.RequestorRole.ChurchMember.Title + ")" : "") + " " + (oCTR.RequestorRole.LeaderRole != null ? " | " + oCTR.RequestorRole.LeaderRole.RoleName + (oCTR.RequestorRole.RoleSector != null ? ", " + oCTR.RequestorRole.RoleSector.Name : "") + "" : "")).Trim();
        //            //}
        //            //  }


        //            //oMemTransf_MDL.oChurchTransfer =_context.ChurchTransfer.Include(t => t.RequestorRole).ThenInclude(t => t.RoleSector).Include(t => t.RequestorRole).ThenInclude(t => t.LeaderRole).Include(t => t.RequestorRole).ThenInclude(t => t.ChurchMember)
        //            //    .Include(t => t.FromChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.FromChurchBody).ThenInclude(t => t.ChurchCategory).ThenInclude(t => t.ChurchLevel).Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel)
        //            //    .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchCategory).ThenInclude(t => t.ChurchLevel)
        //            //    .Where(c => c.FromChurchBodyId == oCurrChuBodyId && c.Id == id).FirstOrDefault();

        //            ////view purpose....
        //            //if (oMemTransf_MDL.oChurchTransfer != null)
        //            //{
        //            //    var oCT = oMemTransf_MDL.oChurchTransfer;
        //            //    oMemTransf_MDL.oAppGlobalOwner = oCurrChuBodyLogOn.AppGlobalOwner;
        //            //    oMemTransf_MDL.oChurchBody = oCurrChuBodyLogOn;
        //            //    oMemTransf_MDL.oCurrLoggedMember = oCurrChuMember_LogOn;
        //            //    if (oCT.RequestorRole != null)
        //            //    {
        //            //        if (oCT.RequestorRole.ChurchMember != null)
        //            //            oMemTransf_MDL.strRequestorFullName = oCT.RequestorRole.ChurchMember.MemberCode + " - " + ((oCT.RequestorRole.ChurchMember.FirstName + ' ' + oCT.RequestorRole.ChurchMember.MiddleName).Trim() + " " + oCT.RequestorRole.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCT.RequestorRole.ChurchMember.Title) ? "(" + oCT.RequestorRole.ChurchMember.Title + ")" : "");
        //            //        if (oCT.RequestorRole.LeaderRole != null)
        //            //            oMemTransf_MDL.strRequestorFullName += " " + (oCT.RequestorRole.LeaderRole != null ? " | " + oCT.RequestorRole.LeaderRole.RoleName : "");
        //            //        if (oCT.RequestorRole.RoleSector != null)
        //            //            oMemTransf_MDL.strRequestorFullName += oCT.RequestorRole.RoleSector != null ? ", " + oCT.RequestorRole.RoleSector.Name : "";
        //            //        //oMemTransf_MDL.strRequestorFullName = oCT.RequestorRole.ChurchMember.MemberCode + " - " + (((oCT.RequestorRole.ChurchMember.FirstName + ' ' + oCT.RequestorRole.ChurchMember.MiddleName).Trim() + " " + oCT.RequestorRole.ChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oCT.RequestorRole.ChurchMember.Title) ? "(" + oCT.RequestorRole.ChurchMember.Title + ")" : "") + " " + (oCT.RequestorRole.LeaderRole != null ? " | " + oCT.RequestorRole.LeaderRole.RoleName + (oCT.RequestorRole.RoleSector != null ? ", " + oCT.RequestorRole.RoleSector.Name : "") + "" : "")).Trim();
        //            //    }

        //            //    oMemTransf_MDL.strFromChurchBody = oCT.FromChurchBody.Name;
        //            //    oMemTransf_MDL.strFromChurchLevel = oCT.FromChurchBody.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.strRequestorRole = oCT.RequestorRole != null ? oCT.RequestorRole?.LeaderRole?.RoleName : "";
        //            //    oMemTransf_MDL.strReqStatus = GetRequestProcessStatusDesc(oCT.ReqStatus); //.Equals("C") ? "Closed" : oCT.ReqStatus.Equals("R") ? "Received" : oCT.ReqStatus.Equals("P") ? "Pending" : "Draft";
        //            //    oMemTransf_MDL.strApprovalStatus = GetRequestProcessStatusDesc(oCT.ApprovalStatus);
        //            //    oMemTransf_MDL.strCurrScope = oCT.CurrentScope.Equals("E") ? "External" : "Internal";
        //            //}

        //            //// ChurchBody tempCB = null;
        //            //if (oMemTransf_MDL.ToChurchBody.ChurchLevel.LevelIndex > 5)  //index = n - 2 
        //            //{
        //            //    var tempCB1 = oMemTransf_MDL.ToChurchBody.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_3 = tempCB1.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ3 = tempCB1.Id;

        //            //    var tempCB2 = tempCB1.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_2 = tempCB2.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ2 = tempCB2.Id;

        //            //    var tempCB3 = tempCB2.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_1 = tempCB3.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ1 = tempCB3.Id;

        //            //    var tempCB4 = tempCB3.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_1 = tempCB4.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ1 = tempCB4.Id;
        //            //}
        //            //else if (oMemTransf_MDL.ToChurchBody.ChurchLevel.LevelIndex > 4)  //index = n - 2 
        //            //{
        //            //    var tempCB1 = oMemTransf_MDL.ToChurchBody.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_3 = tempCB1.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ3 = tempCB1.Id;

        //            //    var tempCB2 = tempCB1.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_2 = tempCB2.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ2 = tempCB2.Id;

        //            //    var tempCB3 = tempCB2.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_1 = tempCB3.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ1 = tempCB3.Id;
        //            //}
        //            //else if (oMemTransf_MDL.ToChurchBody.ChurchLevel.LevelIndex > 3)  //index = n - 2 
        //            //{
        //            //    var tempCB1 = oMemTransf_MDL.ToChurchBody.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_2 = tempCB1.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ2 = tempCB1.Id;

        //            //    var tempCB2 = tempCB1.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_1 = tempCB2.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ1 = tempCB2.Id;
        //            //}
        //            //else if (oMemTransf_MDL.ToChurchBody.ChurchLevel.LevelIndex > 2)  //index = n - 2 
        //            //{
        //            //    var tempCB1 = oMemTransf_MDL.ToChurchBody.ChurchCategory;  // next category
        //            //    oMemTransf_MDL.strToChurchLevel_2 = tempCB1.ChurchLevel.CustomName;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ2 = tempCB1.Id;
        //            //}
        //            //else
        //            //{
        //            //    oMemTransf_MDL.strToChurchLevel_1 = "";  oMemTransf_MDL.strToChurchLevel_2 = ""; oMemTransf_MDL.strToChurchLevel_3 = ""; oMemTransf_MDL.strToChurchLevel_4 = "";
        //            //    oMemTransf_MDL.strToChurchLevel_5 = ""; oMemTransf_MDL.strToChurchLevel_6 = ""; oMemTransf_MDL.strToChurchLevel_7 = "";
        //            //    //
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ1 = null; oMemTransf_MDL.ToChurchBodyId_Categ2 = null; oMemTransf_MDL.ToChurchBodyId_Categ3 = null; oMemTransf_MDL.ToChurchBodyId_Categ4 = null;
        //            //    oMemTransf_MDL.ToChurchBodyId_Categ5 = null;oMemTransf_MDL.ToChurchBodyId_Categ6 = null;oMemTransf_MDL.ToChurchBodyId_Categ7 = null;
        //            //}                    
        //        }

        //        oMemTransf_MDL = this.popLookups_ChurchTransfer(oMemTransf_MDL, oCurrChuBodyLogOn);

        //        //  var _vmMod = populateLookups_ChuTransf(oMemTransf_MDL, oCurrChuBodyId, oCurrChuBodyLogOn);
        //        TempData.Put("oVmCurr", oMemTransf_MDL);
        //        TempData.Keep();

        //        return PartialView("_AddOrEdit_MemTransf", oMemTransf_MDL);
        //    }
        //}

        //[HttpGet]
        //public IActionResult AddOrEdit_CLGTransf(int? oCurrChuBodyId, int id = 0)
        //{
        //    SetUserLogged();
        //    if (!isCurrValid) //prompt!
        //        return RedirectToAction("LoginUserAcc", "UserLogin");
        //    else
        //    {
        //        var oCLGTransf_MDL = new ChurchTransferModel(); // _context, oCurrChuBodyLogOn);  //use this option so all lookups are loaded same time alongside.... from the class
        //        //
        //        var oCurrChuBodyLogOn = oUserLogIn_Priv[0].ChurchBody;
        //        var oUserProfile = oUserLogIn_Priv[0].UserProfile;
        //        if (oCurrChuBodyLogOn == null) //prompt!
        //            return View(oCLGTransf_MDL); // PartialView("_AddOrEdit_CLGTransf", oCLGTransf_MDL); //return View(oCLGTransf_MDL);
        //        if (oCurrChuBodyId == null) oCurrChuBodyId = oCurrChuBodyLogOn.Id;
        //        else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...

        //        // check permission for Core life...
        //        if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) //prompt!
        //            return View(oCLGTransf_MDL); //PartialView("_AddOrEdit_CLGTransf", oCLGTransf_MDL); //return View(oCLGTransf_MDL);

        //        int? oCurrChuMemberId_LogOn = null;
        //        ChurchMember oCurrChuMember_LogOn = null;
        //        if (oUserProfile == null) //prompt!
        //            return View(oCLGTransf_MDL); //PartialView("_AddOrEdit_CLGTransf", oCLGTransf_MDL); //return View(oCLGTransf_MDL);
        //        if (oUserProfile.ChurchMember == null) //prompt!
        //            return View(oCLGTransf_MDL); //PartialView("_AddOrEdit_CLGTransf", oCLGTransf_MDL); //return View(oCLGTransf_MDL);

        //        oCurrChuMemberId_LogOn = oUserProfile.ChurchMember.Id;
        //        oCurrChuMember_LogOn = oUserProfile.ChurchMember;


        //        //var currTransf = (from t_ct in _context.ChurchTransfer
        //        //                .Where(x => x.RequestorChurchBodyId == oCurrChuBodyId && x.ChurchMemberId == oCurrChuMemberId_LogOn &&
        //        //               (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.Status != "U"))
        //        //                  select t_ct).ToList();
        //        //ViewBag.IsPendingTransfer = currTransf.Count > 0;

        //        // oCLGTransf_MDL.oChurchBody = oCurrChuBodyLogOn;   //ToCongregation can never call this fxn... cannot create or send request...
        //        if (id == 0)
        //        {
        //            //var memFromInfo = (
        //            //    from t_cm in _context.ChurchMember.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.Id == oCurrChuMemberId_LogOn)
        //            //    from t_mp in _context.MemberPosition.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.CurrentPos == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty()  // from t_mp in abcd
        //            //    from t_ms in _context.MemberStatus.Where(x => x.ChurchBodyId == oCurrChuBodyId && x.ActiveStatus == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty() // from t_ms in abcde
        //            //    from t_mcs in _context.MemberChurchSector.Include(t => t.Sector).Where(x => x.ChurchBodyId == oCurrChuBodyId && x.CurrSector == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty()
        //            //    from t_cs in _context.ChurchSector.Where(x => x.Id == t_mcs.SectorId && x.ChurchBodyId == oCurrChuBodyId && x.Generational == true).DefaultIfEmpty()    // on t_mcs.SectorId equals t_cs.Id into temp_t_mcs
        //            //                                                                                                                                                            //from t_mcsx in temp_t_mcs.DefaultIfEmpty()          
        //            //    from t_mlr in _context.MemberLeaderRole.Include(t => t.LeaderRole).Where(x => x.CoreRole == true && x.ChurchMemberId == t_cm.Id).DefaultIfEmpty()  // from t_mlr in abcdefg  //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)

        //            //    select new ChurchTransferDetail_VM
        //            //    {
        //            //        oChurchMemberTransf = t_cm,  // strRequestorFullName = (!string.IsNullOrEmpty(t_cm.MemberCode) ? t_cm.MemberCode + " - " : "") + ((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : ""),
        //            //        strFromMemberFullName = (!string.IsNullOrEmpty(t_cm.MemberGlobalId) ? t_cm.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
        //            //        strFromMemberPos = t_mp.ChurchPosition != null ? t_mp.ChurchPosition.PositionName : "",
        //            //        strFromMemberStatus = t_ms.ChurchMemStatus != null ? t_ms.ChurchMemStatus.Name : "",
        //            //        strFromMemberAgeGroup = t_cs != null ? t_cs.Name : "",
        //            //        strFromMemberRole = t_mlr.LeaderRole != null ? (t_mlr.LeaderRole.RoleName + (t_mlr.RoleSector != null ? ", " + t_mlr.RoleSector.Name : "")) : "",  //strFromMemberRole = t_mlr.LeaderRole != null ? t_mlr.LeaderRole.RoleName : "",
        //            //        numFromChurchPositionId = t_mp != null ? t_mp.ChurchPositionId : -1,
        //            //        numFromMemberLeaderRoleId = t_mlr != null ? t_mlr.Id : -1  // strMemberFullName = t.MemberCode + ' ' + (((t.FirstName + ' ' + t.MiddleName).Trim() + " " + t.LastName).Trim() + " " + (!string.IsNullOrEmpty(t.Title) ? "(" + t.Title + ")" : "")).Trim(),                                
        //            //    }
        //            //       ).FirstOrDefault();

        //            //if (memFromInfo == null)   //prompt!
        //            //    return View(oCLGTransf_MDL); //PartialView("_AddOrEdit_CLGTransf", oCLGTransf_MDL); //return View(oCLGTransf_MDL); member to transfer --details-- not found

        //            //get requestor detail
        //            var oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchUnit)
        //                    .Where(c => c.ChurchBodyId == oCurrChuBodyId && c.ChurchMemberId == oCurrChuMemberId_LogOn && c.IsCoreRole == true).Take(1).FirstOrDefault();
        //            oCLGTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCurrChuMember_LogOn.MemberGlobalId) ? oCurrChuMember_LogOn.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(oCurrChuMember_LogOn.Title) ? oCurrChuMember_LogOn.Title : "") + ' ' + oCurrChuMember_LogOn.FirstName).Trim() + " " + oCurrChuMember_LogOn.MiddleName).Trim() + " " + oCurrChuMember_LogOn.LastName).Trim();
        //            if (oReqMLR != null)
        //                oCLGTransf_MDL.strRequestorFullName += oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.ChurchUnit != null ? ", " + oReqMLR.ChurchUnit.Name : "") + ")" : "";

        //            //create user and init... 
        //            oCLGTransf_MDL.oChurchTransfer = new ChurchTransfer();
        //            // oCLGTransf_MDL.oChurchTransfer.ChurchMemberId = memFromInfo.oChurchMemberTransf.Id;
        //            oCLGTransf_MDL.oChurchTransfer.RequestorMemberId = oCurrChuMemberId_LogOn;
        //            oCLGTransf_MDL.oChurchTransfer.RequestorRoleId = oReqMLR != null ? oReqMLR.Id : (int?)null; //requestMLRId; 
        //            oCLGTransf_MDL.oChurchTransfer.RequestorChurchBodyId = oCurrChuBodyId;
        //            // oCLGTransf_MDL.oChurchTransfer.FromChurchBodyId = oCurrChuBodyId;
        //            oCLGTransf_MDL.oChurchTransfer.FromChurchPositionId = null;
        //            oCLGTransf_MDL.oChurchTransfer.FromMemberLeaderRoleId = null;
        //            oCLGTransf_MDL.oChurchTransfer.CurrentScope = "I";  //Internal cong. submission first
        //            oCLGTransf_MDL.oChurchTransfer.ReqStatus = "N"; //NEW... DRAFT 
        //            oCLGTransf_MDL.oChurchTransfer.Status = null; //NEW... DRAFT
        //            oCLGTransf_MDL.oChurchTransfer.ApprovalStatus = null;
        //            oCLGTransf_MDL.oChurchTransfer.TransferType = "CT";
        //            oCLGTransf_MDL.oChurchTransfer.RequestDate = DateTime.Now;

        //            //view purpose....
        //            oCLGTransf_MDL.oChurchTransfer.RequestorChurchBody = oCurrChuBodyLogOn;
        //            oCLGTransf_MDL.oChurchTransfer.RequestorChurchMember = oCurrChuMember_LogOn;
        //            // oCLGTransf_MDL.oChurchTransfer.FromChurchBody = oCurrChuBodyLogOn;
        //            //
        //            oCLGTransf_MDL.oAppGlobalOwner = oCurrChuBodyLogOn.AppGlobalOwner;
        //            oCLGTransf_MDL.oChurchBody = oCurrChuBodyLogOn;
        //            oCLGTransf_MDL.oRequestorChurchBody = oCurrChuBodyLogOn;
        //            oCLGTransf_MDL.oCurrLoggedMember = oCurrChuMember_LogOn;
        //            oCLGTransf_MDL.oCurrLoggedMemberId = oCurrChuMemberId_LogOn;
        //            oCLGTransf_MDL.strAffliliateChurchBodies = new List<string>();  // GetChurchAffiliates(t_ct.AttachedToChurchBodyList);
        //            oCLGTransf_MDL.oAffliliateChurchBodies = new List<ChurchBody>();

        //            // 
        //            //oCLGTransf_MDL.strFromMemberFullName = memFromInfo.strFromMemberFullName;
        //            //oCLGTransf_MDL.strRequestorFullName = memFromInfo.strFromMemberFullName;
        //            //oCLGTransf_MDL.strFromMemberPos = memFromInfo.strFromMemberPos;
        //            //oCLGTransf_MDL.strFromMemberStatus = memFromInfo.strFromMemberStatus;
        //            //oCLGTransf_MDL.strFromMemberAgeGroup = memFromInfo.strFromMemberAgeGroup;
        //            //oCLGTransf_MDL.strFromMemberRole = memFromInfo.strFromMemberRole;
        //            //oCLGTransf_MDL.strRequestorRole = memFromInfo.strFromMemberRole;

        //            //
        //            oCLGTransf_MDL.strRequestorChurchBody = oCurrChuBodyLogOn.Name;
        //            oCLGTransf_MDL.strRequestorChurchLevel = oCurrChuBodyLogOn.ChurchLevel.CustomName;
        //            // oCLGTransf_MDL.strFromChurchBody = oCLGTransf_MDL.strRequestorChurchBody;
        //            // oCLGTransf_MDL.strFromChurchLevel = oCLGTransf_MDL.strRequestorChurchLevel;
        //            // oCLGTransf_MDL.strRequestorRole = oReqMLR != null ? oReqMLR.LeaderRole.RoleName : "";
        //            oCLGTransf_MDL.strCurrScope = oCLGTransf_MDL.oChurchTransfer.CurrentScope.Equals("E") ? "External" : "Internal";
        //            //oCLGTransf_MDL.strSelectCong = "";

        //        }

        //        else
        //        {  // oCLGTransf_MDL = new ClergyTransfer_VM(_context, oCurrChuBodyLogOn);

        //            oCLGTransf_MDL = (
        //                 from t_ct in _context.ChurchTransfer.AsNoTracking().Include(t => t.ChurchMemberTransf).Include(t => t.RequestorChurchBody).Include(t => t.FromChurchBody)
        //                 .Include(t => t.ToChurchBody).ThenInclude(t => t.ChurchLevel).Include(t => t.RequestorChurchMember).Include(t => t.RequestorRole) //.ThenInclude(t => t.LeaderRole)
        //                     .Where(x => x.Id == id && x.RequestorChurchBodyId == oCurrChuBodyId) // && (showOwned==false || (showOwned==true && x.RequestorMemberId == oCurrLoggedUser_MemberId)))  //&& x.CurrentScope == "I" 
        //                 from t_cb_f in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId)
        //                 from t_cb_r in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.Id == t_ct.RequestorChurchBodyId)
        //                 from t_cb_t in _context.ChurchBody.AsNoTracking().Where(x => x.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
        //                 from t_cm in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && t_ct.ChurchMemberId == x.Id)
        //                 from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && t_ct.RequestorMemberId == x.Id)
        //                 from t_mp in _context.MemberPosition.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.CurrentPos == true && t_ct.ChurchMemberId == x.ChurchMemberId).DefaultIfEmpty()
        //                 from t_ms in _context.MemberStatus.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCurrent == true && t_ct.ChurchMemberId == x.ChurchMemberId).DefaultIfEmpty()//.Include(t => t.ChurchMemStatus)
        //                 from t_mcs in _context.MemberChurchSector.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.CurrSector == true && t_ct.ChurchMemberId == x.ChurchMemberId).Take(1).DefaultIfEmpty()//.Include(t => t.Sector)// age bracket/group: children, youth, adults as defined in config. OR auto-assign
        //                                                                                                                                                                                                                         // from t_cs in _context.ChurchSector.Where(x=> x.ChurchBodyId == t_mcs.ChurchBodyId && x.Id==t_mcs.SectorId && x.Generational == true).DefaultIfEmpty()
        //                 from t_mlr in _context.MemberLeaderRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.FromChurchBodyId && x.IsCoreRole == true && t_ct.ChurchMemberId == x.ChurchMemberId).Take(1).DefaultIfEmpty() //.Include(t => t.LeaderRole)  //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)
        //                 from t_mlr_r in _context.MemberLeaderRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.RequestorChurchBodyId && x.IsCoreRole == true && t_ct.RequestorMemberId == x.ChurchMemberId).Take(1).DefaultIfEmpty()//.Include(t => t.LeaderRole) //  && x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)
        //                                                                                                                                                                                                                                 //  from t_lr_tr in _context.LeaderRole.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToLeaderRoleId == x.Id).DefaultIfEmpty()  //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)
        //                                                                                                                                                                                                                                 //  from t_cs_trs in _context.ChurchSector.AsNoTracking().Where(x => x.ChurchBodyId == t_ct.ToChurchBodyId && t_ct.ToRoleSectorId == x.Id).DefaultIfEmpty()   //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)




        //                     //from t_cm in _context.ChurchMember.AsNoTracking().Include(t => t.ChurchBody).ThenInclude(t => t.AppGlobalOwner).Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && t_ct.ChurchMemberId == x.Id)
        //                     //from t_cm_r in _context.ChurchMember.AsNoTracking().Where(x => x.ChurchBodyId == oCurrChuBodyId && t_ct.RequestorMemberId == x.Id)
        //                     //from t_mlr_r in _context.MemberLeaderRole.AsNoTracking().Include(t => t.ChurchMember).Include(t => t.RoleSector).Include(t => t.LeaderRole)
        //                     //                   .Where(x => x.ChurchBodyId == oCurrChuBodyId && x.Id == t_ct.RequestorRoleId).DefaultIfEmpty()
        //                     //from t_cb_r in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel).Where(x => x.Id == t_ct.RequestorChurchBodyId) //.DefaultIfEmpty()
        //                     //from t_cb_f in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel).Where(x => x.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.Id == t_ct.FromChurchBodyId).DefaultIfEmpty()
        //                     //from t_cb_t in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel).Where(x => x.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.Id == t_ct.ToChurchBodyId).DefaultIfEmpty()
        //                     //from t_mp in _context.MemberPosition.AsNoTracking().Include(t => t.ChurchBody).ThenInclude(t => t.AppGlobalOwner).Include(t => t.ChurchPosition).Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.ChurchMemberId == t_cm.Id && x.CurrentPos == true).Take(1).DefaultIfEmpty()
        //                     //from t_ms in _context.MemberStatus.AsNoTracking().Include(t => t.ChurchBody).ThenInclude(t => t.AppGlobalOwner).Include(t => t.ChurchMemStatus).Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.ChurchMemberId == t_cm.Id && x.ActiveStatus == true).Take(1).DefaultIfEmpty()
        //                     //from t_mcs in _context.MemberChurchSector.AsNoTracking().Include(t => t.ChurchBody).ThenInclude(t => t.AppGlobalOwner).Include(t => t.Sector).Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.CurrSector == true && t_cm.Id == x.ChurchMemberId).Take(1).DefaultIfEmpty() // age bracket/group: children, youth, adults as defined in config. OR auto-assign
        //                     //from t_mlr in _context.MemberLeaderRole.AsNoTracking().Include(t => t.ChurchBody).ThenInclude(t => t.AppGlobalOwner).Include(t => t.LeaderRole).Where(x => x.ChurchBody.AppGlobalOwnerId == oCurrChuBodyLogOn.AppGlobalOwnerId && x.ChurchMemberId == t_cm.Id && x.CoreRole == true).Take(1).DefaultIfEmpty()   //&& x.CurrServing && x.LeaderRole.AuthorityIndex == _context.MemberLeaderRole.Where(y => y.ChurchMemberId == x.ChurchMemberId).Min(y => y.LeaderRole.AuthorityIndex)

        //                 select new ChurchTransferModel()
        //                 {
        //                     oCurrLoggedMember = oCurrChuMember_LogOn,
        //                     oCurrLoggedMemberId = oCurrChuMemberId_LogOn,
        //                     oAppGlobalOwner = oCurrChuBodyLogOn.AppGlobalOwner,

        //                     oChurchTransfer = t_ct,
        //                     oRequestorChurchBody = t_cb_r,
        //                     oChurchBody = t_cb_r,
        //                     numTransferDxn = t_cb_r.Id == oCurrChuBodyLogOn.Id ? 2 : t_cb_t.Id == oCurrChuBodyLogOn.Id ? 1 : 0,   //out, in
        //                     strTransferDxn = oCLGTransf_MDL.numTransferDxn == 1 ? "Incoming" : oCLGTransf_MDL.numTransferDxn == 2 ? "Outgoing" : "",
        //                     strFromChurchBody = t_cb_f.Name,
        //                     strFromChurchLevel = t_cb_f.ChurchLevel.CustomName,
        //                     strRequestorChurchBody = t_cb_r.Name,
        //                     strRequestorChurchLevel = t_cb_r.ChurchLevel.CustomName,
        //                     strRequestorRole = t_mlr_r.LeaderRole != null ? t_mlr_r.LeaderRole.RoleName : "",
        //                     strReqStatus = GetRequestProcessStatusDesc(t_ct.ReqStatus),
        //                     strApprovalStatus = GetRequestProcessStatusDesc(t_ct.ApprovalStatus),
        //                     strCurrScope = t_ct.CurrentScope.Equals("E") ? "External" : "Internal",
        //                     strRequestorFullName = (!string.IsNullOrEmpty(t_cm_r.MemberGlobalId) ? t_cm_r.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm_r.Title) ? t_cm_r.Title : "") + ' ' + t_cm_r.FirstName).Trim() + " " + t_cm_r.MiddleName).Trim() + " " + t_cm_r.LastName).Trim(),
        //                     //  //                        t_mlr_r.LeaderRole != null ? " (" + t_mlr_r.LeaderRole.RoleName + (t_mlr_r.RoleSector != null ? ", " + t_mlr_r.RoleSector.Name : "")+")" : "", 
        //                     strFromMemberPos = t_mp.ChurchPosition != null ? t_mp.ChurchPosition.PositionName : "",
        //                     strFromMemberStatus = t_ms.ChurchMemStatus != null ? t_ms.ChurchMemStatus.Name : "",
        //                     strFromMemberAgeGroup = t_mcs.Sector != null ? t_mcs.Sector.Name : "",
        //                     strFromMemberRole = t_mlr.LeaderRole != null ? t_mlr.LeaderRole.RoleName : "",
        //                     strFromMemberFullName = (!string.IsNullOrEmpty(t_cm.MemberGlobalId) ? t_cm.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim(),
        //                     strAffliliateChurchBodies = GetChurchAffiliates(t_ct.AttachedToChurchBodyList),
        //                     // oAffliliateChurchBodies = GetChurchAffiliates_CB(t_ct.AttachedToChurchBodyList, oCurrChuMember_LogOn.AppGlobalOwnerId)
        //                 }
        //                ).FirstOrDefault();

        //            if (oCLGTransf_MDL.oChurchTransfer != null)
        //            {
        //                oCLGTransf_MDL.strApprovers = _GetApprovers(oCLGTransf_MDL.oChurchTransfer.IApprovalAction);
        //                oCLGTransf_MDL.oAffliliateChurchBodies = GetChurchAffiliates_CB(oCLGTransf_MDL.oChurchTransfer.AttachedToChurchBodyList, oCLGTransf_MDL.oAppGlobalOwner?.Id);
        //            }

        //            if (oCLGTransf_MDL == null) return View(oCLGTransf_MDL); //

        //            //get requestor detail
        //            var oReqMLR = _context.MemberLeaderRole.Include(t => t.LeaderRole).Include(t => t.ChurchUnit)
        //                    .Where(c => c.ChurchBodyId == oCLGTransf_MDL.oChurchTransfer.RequestorChurchBodyId && c.ChurchMemberId == oCLGTransf_MDL.oChurchTransfer.RequestorMemberId
        //                        && c.LeaderRoleId == oCLGTransf_MDL.oChurchTransfer.RequestorRole.LeaderRoleId).FirstOrDefault();

        //            // oCLGTransf_MDL.strRequestorFullName = (!string.IsNullOrEmpty(oCurrChuMember_LogOn.MemberGlobalId) ? oCurrChuMember_LogOn.MemberGlobalId + " - " : "") + ((((!string.IsNullOrEmpty(oCurrChuMember_LogOn.Title) ? oCurrChuMember_LogOn.Title : "") + ' ' + oCurrChuMember_LogOn.FirstName).Trim() + " " + oCurrChuMember_LogOn.MiddleName).Trim() + " " + oCurrChuMember_LogOn.LastName).Trim();
        //            if (oReqMLR != null)
        //                oCLGTransf_MDL.strRequestorFullName += oReqMLR.LeaderRole != null ? " (" + oReqMLR.LeaderRole.RoleName + (oReqMLR.ChurchUnit != null ? ", " + oReqMLR.ChurchUnit.Name : "") + ")" : "";

        //            if (oCLGTransf_MDL.oChurchTransfer == null) return View(oCLGTransf_MDL);

        //            //avaoid dups
        //            var currTransf = (from t_cm in _context.ChurchTransfer
        //                               .Where(x => x.FromChurchBodyId == oCLGTransf_MDL.oChurchTransfer.FromChurchBodyId && x.ChurchMemberId == oCLGTransf_MDL.oChurchTransfer.ChurchMemberId && x.TransferType == "CT" &&
        //                               x.TransferSubType == oCLGTransf_MDL.oChurchTransfer.TransferSubType &&
        //                               (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.Status != "U"))
        //                              select t_cm).ToList();
        //            ViewBag.IsPendingTransfer = currTransf.Count > 0;
        //        }


        //        List<ChurchLevel> oCBLevels = _context.ChurchLevel
        //                   .Where(c => c.AppGlobalOwnerId == oCLGTransf_MDL.oChurchBody.AppGlobalOwnerId).ToList().OrderBy(c => c.LevelIndex).ToList();
        //        //if (oCBLevels.Count > 0)
        //        //{
        //        //    ViewBag.Filter_fr_fn = ViewBag.Filter_fn = !string.IsNullOrEmpty(oCBLevels[0].CustomName) ? oCBLevels[0].CustomName : oCBLevels[6].Name;
        //        //   
        //        if (oCBLevels.Count() > 0)
        //        {
        //            ViewBag.Filter_fr_ln = ViewBag.Filter_ln = !string.IsNullOrEmpty(oCBLevels[oCBLevels.Count - 1].CustomName) ? oCBLevels[oCBLevels.Count - 1].CustomName : oCBLevels[6].Name;
        //            ViewBag.Filter_fr_1 = ViewBag.Filter_1 = !string.IsNullOrEmpty(oCBLevels[0].CustomName) ? oCBLevels[0].CustomName : oCBLevels[0].Name;
        //            if (oCBLevels.Count() > 1)
        //            {
        //                ViewBag.Filter_fr_2 = ViewBag.Filter_2 = !string.IsNullOrEmpty(oCBLevels[1].CustomName) ? oCBLevels[1].CustomName : oCBLevels[1].Name;
        //                if (oCBLevels.Count() > 2)
        //                {
        //                    ViewBag.Filter_fr_3 = ViewBag.Filter_3 = !string.IsNullOrEmpty(oCBLevels[2].CustomName) ? oCBLevels[2].CustomName : oCBLevels[2].Name;
        //                    if (oCBLevels.Count() > 3)
        //                    {
        //                        ViewBag.Filter_fr_4 = ViewBag.Filter_4 = !string.IsNullOrEmpty(oCBLevels[3].CustomName) ? oCBLevels[3].CustomName : oCBLevels[3].Name;
        //                        if (oCBLevels.Count() > 4)
        //                        {
        //                            ViewBag.Filter_fr_5 = ViewBag.Filter_5 = !string.IsNullOrEmpty(oCBLevels[4].CustomName) ? oCBLevels[43].CustomName : oCBLevels[4].Name;
        //                            if (oCBLevels.Count() > 5)
        //                            {
        //                                ViewBag.Filter_fr_6 = ViewBag.Filter_6 = !string.IsNullOrEmpty(oCBLevels[5].CustomName) ? oCBLevels[5].CustomName : oCBLevels[5].Name;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        //}                

        //        oCLGTransf_MDL = this.popLookups_ChurchTransfer(oCLGTransf_MDL, oCurrChuBodyLogOn);
        //        ViewBag.TransType = oCLGTransf_MDL.oChurchTransfer.TransferType;
        //        TempData.Put("oVmCurr", oCLGTransf_MDL);
        //        TempData.Keep();

        //        return PartialView("_AddOrEdit_CLGTransf", oCLGTransf_MDL);
        //    }
        //}


        public void DetachAllEntities()
        {
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit_MemTransf(ChurchTransferModel vmMod)  //save, int taskIndx
        //{
        //    try
        //    {
        //        List<ApprovalActionStep> oCurrApprovers = new List<ApprovalActionStep>();
        //        //List<ChurchMember> oChuMemNotifyEmailList = new List<ChurchMember>();
        //        var _oMTChanges = vmMod.oChurchTransfer;  //  has updates 
        //        var taskIndx = vmMod.userRequestTask;
        //        var apprComment = vmMod.strApproverComment;
        //        vmMod = TempData.Get<ChurchTransferModel>("oVmCurr"); TempData.Keep();
        //        if (vmMod == null)
        //        { //ModelState.AddModelError(string.Empty, "Data retrieval failed. Please refresh and try again."); return Json(false); 
        //            return Json(new { taskSuccess = false, userMess = "Data retrieval failed. Please refresh and try again." });
        //        }
        //        if (vmMod.oChurchTransfer == null)
        //        { //ModelState.AddModelError(string.Empty, "Data submitted failed for 'Update, Send request' operation to be performed."); return Json(false); 
        //            return Json(new { taskSuccess = false, userMess = "Data submitted failed for 'Update, Send request' operation to be performed." });
        //        }

        //        DetachAllEntities();

        //        // vmMod.oChurchTransfer = _oMT;
        //        vmMod.userRequestTask = taskIndx;
        //        ChurchTransfer _oMemTransf = vmMod.oChurchTransfer; // vm.oChurchTransfer; // init...  
        //        ChurchTransfer oMemTransf = vmMod.oChurchTransfer;
        //        if (taskIndx == 1 || taskIndx == 2 || taskIndx == 5)  //SAVE, SEND, RESUBMIT
        //        {
        //            ModelState.Remove("oAppGlobalOwner.OwnerName");
        //            ModelState.Remove("oChurchTransfer.ChurchMemberId");
        //            ModelState.Remove("oChurchTransfer.ToChurchBodyId");
        //            ModelState.Remove("ToChurchBodyId_Categ1");
        //            ModelState.Remove("ToChurchBodyId_Categ2"); //ModelState.Remove("ToChurchBodyId_Categ3"); 
        //            ModelState.Remove("oChurchBody.Name");
        //            ModelState.Remove("oChurchBody.AssociationType");
        //            ModelState.Remove("oChurchTransfer.FromChurchBodyId");

        //            //finally check error state...
        //            if (!ModelState.IsValid)
        //                return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });

        //            if (oMemTransf.Id == 0)
        //            {
        //                if (!(taskIndx == 1 || taskIndx == 2))  //save... send  
        //                { //ModelState.AddModelError(string.Empty, "Current request allows only 'Update, Send request' operation to be performed."); return Json(false);
        //                    return Json(new { taskSuccess = false, userMess = "Current request allows only 'Update, Send request' operation to be performed." });
        //                }

        //                if (_oMTChanges.ChurchMemberId == null || _oMTChanges.ToChurchBodyId == null)
        //                { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                    return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
        //                }

        //                if (taskIndx == 2) //... send 
        //                {
        //                    //check for pending.. unClosed requests OR successful closed requests
        //                    var currTransf = (from t_cm in _context.ChurchTransfer
        //                                      .Where(x => x.RequestorChurchBodyId == oMemTransf.RequestorChurchBodyId && x.ChurchMemberId == oMemTransf.ChurchMemberId &&
        //                                     (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.Status != "U"))
        //                                      select t_cm).ToList();
        //                    if (currTransf.Count > 0)
        //                    {
        //                        return Json(new { taskSuccess = false, userMess = "Member transfer already initiated or done for specified member." });
        //                    }
        //                }

        //                var oMT = new ChurchTransfer
        //                { //create user and init...

        //                    RequestorMemberId = oMemTransf.RequestorMemberId,
        //                    RequestorRoleId = oMemTransf.RequestorRoleId,
        //                    RequestorChurchBodyId = oMemTransf.RequestorChurchBodyId,
        //                    FromChurchBodyId = oMemTransf.FromChurchBodyId,
        //                    CurrentScope = oMemTransf.CurrentScope,
        //                    AckStatus = oMemTransf.ReqStatus,
        //                    ApprovalStatus = oMemTransf.ApprovalStatus,
        //                    TransferType = oMemTransf.TransferType,
        //                    //ToRequestDate  = date request is forwarded to destination congregation
        //                    // RequestDate = DateTime.Now,
        //                    Created = DateTime.Now,  // oMemTransf.Created,
        //                    LastMod = DateTime.Now,
        //                    //
        //                    ChurchMemberId = _oMTChanges.ChurchMemberId,
        //                    ToChurchBodyId = _oMTChanges.ToChurchBodyId,
        //                    ReasonId = _oMTChanges.ReasonId,
        //                    TransMessageId = _oMTChanges.TransMessageId,
        //                    Comments = _oMTChanges.Comments,
        //                    //
        //                    FromChurchPositionId = _oMTChanges.FromChurchPositionId <= 0 ? null : _oMTChanges.FromChurchPositionId,
        //                    FromMemberLeaderRoleId = _oMTChanges.FromMemberLeaderRoleId <= 0 ? null : _oMTChanges.FromMemberLeaderRoleId
        //                };
        //                //
        //                _context.Add(oMT);

        //                try
        //                {
        //                    await _context.SaveChangesAsync();
        //                    //
        //                    oMemTransf = oMT; //update cache... 
        //                                      // oMemTransf.ChurchMemberTransf = _oMemTransf.ChurchMemberTransf;
        //                                      // oMemTransf.ToChurchBody = _oMemTransf.ToChurchBody;
        //                    oMemTransf.RequestorChurchMember = _oMemTransf.RequestorChurchMember;
        //                    oMemTransf.RequestorChurchBody = _oMemTransf.RequestorChurchBody;
        //                    oMemTransf.FromChurchBody = _oMemTransf.FromChurchBody;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return Json(new { taskSuccess = false, userMess = "Attempt to save data failed. Please try again." });
        //                }
        //            }
        //            else
        //            {
        //                if (oMemTransf == null)
        //                {// ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
        //                    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
        //                }

        //                if (oMemTransf.ChurchMemberId == null || oMemTransf.ToChurchBodyId == null || oMemTransf.ChurchMemberTransf == null || oMemTransf.ToChurchBody == null)
        //                { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                    return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
        //                }

        //                if (taskIndx == 2 || taskIndx == 5) //... send 
        //                {
        //                    //check for pending.. unClosed requests OR successful closed requests 
        //                    var currTransf = (from t_cm in _context.ChurchTransfer
        //                                      .Where(x => x.RequestorChurchBodyId == oMemTransf.RequestorChurchBodyId && x.ChurchMemberId == oMemTransf.ChurchMemberId && x.TransferType == "MT" &&
        //                                     (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.Status != "U"))
        //                                      select t_cm).ToList();
        //                    if (currTransf.Count > 0)
        //                    {
        //                        return Json(new { taskSuccess = false, userMess = "Member transfer already initiated or done for specified member." });
        //                    }
        //                }

        //                //set... for view in cache 
        //                if (_oMTChanges.ChurchMemberId != null) oMemTransf.ChurchMemberId = _oMTChanges.ChurchMemberId;
        //                if (_oMTChanges.ToChurchBodyId != null) oMemTransf.ToChurchBodyId = _oMTChanges.ToChurchBodyId;
        //                if (_oMTChanges.ReasonId != null) oMemTransf.ReasonId = _oMTChanges.ReasonId;
        //                if (_oMTChanges.TransMessageId != null) oMemTransf.TransMessageId = _oMTChanges.TransMessageId;
        //                if (_oMTChanges.Comments != null) oMemTransf.Comments = _oMTChanges.Comments;
        //                if (_oMTChanges.FromChurchPositionId != null) oMemTransf.FromChurchPositionId = _oMTChanges.FromChurchPositionId <= 0 ? null : _oMTChanges.FromChurchPositionId;
        //                if (_oMTChanges.FromMemberLeaderRoleId != null) oMemTransf.FromMemberLeaderRoleId = _oMTChanges.FromMemberLeaderRoleId <= 0 ? null : _oMTChanges.FromMemberLeaderRoleId;
        //                // oMemTransf.RequestDate =  DateTime.Now;

        //                //
        //                //already loaded at GET()
        //                if (taskIndx == 5)
        //                {
        //                    var oMT = new ChurchTransfer
        //                    { //create user and init...
        //                        RequestorMemberId = oMemTransf.RequestorMemberId,
        //                        RequestorRoleId = oMemTransf.RequestorRoleId,
        //                        RequestorChurchBodyId = oMemTransf.RequestorChurchBodyId,
        //                        FromChurchBodyId = oMemTransf.FromChurchBodyId,
        //                        CurrentScope = oMemTransf.CurrentScope,
        //                        AckStatus = oMemTransf.ReqStatus,
        //                        ApprovalStatus = oMemTransf.ApprovalStatus,
        //                        TransferType = oMemTransf.TransferType,
        //                        //ActualRequestDate  = date request is forwarded to destination congregation
        //                        // RequestDate = oMemTransf.RequestDate,
        //                        Created = DateTime.Now,  // oMemTransf.Created,
        //                        LastMod = DateTime.Now,
        //                        //
        //                        ChurchMemberId = oMemTransf.ChurchMemberId,
        //                        ToChurchBodyId = oMemTransf.ToChurchBodyId,
        //                        ReasonId = oMemTransf.ReasonId,
        //                        TransMessageId = oMemTransf.TransMessageId,
        //                        Comments = oMemTransf.Comments,
        //                        //
        //                        FromChurchPositionId = oMemTransf.FromChurchPositionId,
        //                        FromMemberLeaderRoleId = oMemTransf.FromMemberLeaderRoleId
        //                    };
        //                    //
        //                    _context.Add(oMT);

        //                    try
        //                    {
        //                        await _context.SaveChangesAsync();
        //                        //
        //                        oMemTransf = oMT; //update cache... 
        //                                          // oMemTransf.ChurchMemberTransf = _oMemTransf.ChurchMemberTransf;
        //                                          // oMemTransf.ToChurchBody = _oMemTransf.ToChurchBody;
        //                        oMemTransf.RequestorChurchMember = _oMemTransf.RequestorChurchMember;
        //                        oMemTransf.RequestorChurchBody = _oMemTransf.RequestorChurchBody;
        //                        oMemTransf.FromChurchBody = _oMemTransf.FromChurchBody;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return Json(new { taskSuccess = false, userMess = "Attempt to save data failed. Please try again." });
        //                    }
        //                }

        //                //oMemTransf.ChurchMemberTransf = _oMemTransf.ChurchMemberTransf;
        //                //oMemTransf.ToChurchBody = _oMemTransf.ToChurchBody;
        //                //oMemTransf.RequestorChurchMember = _oMemTransf.RequestorChurchMember;
        //                //oMemTransf.RequestorChurchBody = _oMemTransf.RequestorChurchBody;
        //                //oMemTransf.FromChurchBody = _oMemTransf.FromChurchBody;

        //                //if (taskIndx != 1 )
        //                //{
        //                //    if (oMemTransf.ChurchMemberId == null || oMemTransf.ToChurchBodyId == null || oMemTransf.ChurchMemberTransf == null || oMemTransf.ToChurchBody==null)
        //                //    { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                //        return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
        //                //    }

        //                //    ////get member
        //                //    //oMemTransf.ChurchMemberTransf = _context.ChurchMember.Find(oMemTransf.ChurchMemberId);

        //                //    ////get member
        //                //    //oMemTransf.ToChurchBody = _context.ChurchBody.Find(oMemTransf.ToChurchBodyId);
        //                //}
        //            }

        //            //get member
        //            if (oMemTransf.ChurchMemberTransf == null)
        //                oMemTransf.ChurchMemberTransf = _context.ChurchMember.Find(oMemTransf.ChurchMemberId);

        //            //get member
        //            if (oMemTransf.ToChurchBody == null)
        //                oMemTransf.ToChurchBody = _context.ChurchBody.Find(oMemTransf.ToChurchBodyId);

        //        }

        //        else
        //        {   // get the member transfer data... no updates except the approval/ action parts 
        //            if (oMemTransf == null)
        //            {// ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
        //            }

        //            if (oMemTransf.ChurchMemberId == null || oMemTransf.ToChurchBodyId == null || oMemTransf.ChurchMemberTransf == null || oMemTransf.ToChurchBody == null)
        //            { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
        //            }


        //            //get member
        //            if (oMemTransf.ChurchMemberTransf == null)
        //                oMemTransf.ChurchMemberTransf = _context.ChurchMember.Find(oMemTransf.ChurchMemberId);

        //            //get member
        //            if (oMemTransf.ToChurchBody == null)
        //                oMemTransf.ToChurchBody = _context.ChurchBody.Find(oMemTransf.ToChurchBodyId);

        //            if (taskIndx != 5 && taskIndx >= 3 && taskIndx <= 9)
        //            {
        //                vmMod.lsCurrApprActionSteps = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                          .Where(c => c.ApprovalActionId == vmMod.currApprovalActionId && c.CurrentScope == oMemTransf.CurrentScope &&
        //                          c.ChurchBody.AppGlobalOwnerId == oMemTransf.RequestorChurchBody.AppGlobalOwnerId && c.ChurchBody.AppGlobalOwnerId == oMemTransf.RequestorChurchBody.AppGlobalOwnerId &&   //(c.ChurchBodyId == oMemTransf.RequestorChurchBodyId || c.ChurchBodyId == oMemTransf.ToChurchBodyId) && // == 
        //                          c.ApprovalAction.ProcessCode == "TRF" && c.ApprovalAction.ProcessSubCode == "MT" && c.ApprovalAction.Status == "A" && c.Status == "A")
        //                          .ToList();
        //            }
        //            else if (taskIndx == 10)
        //            {
        //                vmMod.lsCurrApprActionSteps = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                     .Where(c => c.ApprovalActionId == vmMod.currApprovalActionId && c.CurrentScope == oMemTransf.CurrentScope &&
        //                           c.ChurchBody.AppGlobalOwnerId == oMemTransf.RequestorChurchBody.AppGlobalOwnerId && c.ChurchBody.AppGlobalOwnerId == oMemTransf.RequestorChurchBody.AppGlobalOwnerId &&   //(c.ChurchBodyId == oMemTransf.RequestorChurchBodyId || c.ChurchBodyId == oMemTransf.ToChurchBodyId) && // == 
        //                           c.ApprovalAction.ProcessCode == "TRF" && c.ApprovalAction.ProcessSubCode == "MT" && c.ApprovalAction.Status == "A" && c.Status == "A" && c.ActionStepStatus != "A")
        //                     .ToList();
        //            }
        //        }

        //        //if (oMemTransf == null)
        //        //    { //ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
        //        //    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
        //        //}

        //        if (oMemTransf.Id > 0)
        //        {
        //            if (taskIndx >= 1 && taskIndx <= 5)  // SAVE-SEND-RECALL-TERMINATE-RESUBMIT
        //            {
        //                if (!(oMemTransf.CurrentScope == "I" && (taskIndx >= 1 && taskIndx <= 5))) // || taskIndx == 2 || taskIndx == 5))) / update a 'Draft' or Send 'Draft' Req...  resubmit //recalled, terminated, declined// here!
        //                { //ModelState.AddModelError(string.Empty, "Current request allows only 'Save, Send, Recall, TERMINATE, RESUBMIT request' operations to be performed.");   return Json(false); 
        //                    return Json(new { taskSuccess = false, userMess = "Current request allows only 'Save, Send, Recall, TERMINATE, RESUBMIT request' operations to be performed" });
        //                }

        //                //Update... only Draft
        //                if (taskIndx == 1)
        //                {
        //                    if (oMemTransf.ReqStatus != "N" && oMemTransf.ReqStatus != "R")  //draft or recalled
        //                    {  // ModelState.AddModelError(string.Empty, "Request already sent. Update cannot be done. Hint: Try 'Recall request' to make necessary changes.");  return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Request already sent. Update cannot be done. Hint: Try 'Recall request' to make necessary changes." });
        //                    } // ViewBag.UserMsg = "Updated member family relations successfully.";
        //                }
        //                else if (taskIndx == 2)
        //                {
        //                    if (oMemTransf.ReqStatus != "N")  //send saved 'draft'
        //                    { // ModelState.AddModelError(string.Empty, "Request already sent. Hint: Try 'Recall request' to make necessary changes or 'Resubmit' if it's recalled, terminated or declined.");   return Json(false);
        //                        return Json(new { taskSuccess = false, userMess = "Request already sent. Hint: Try 'Recall request' to make necessary changes or 'Resubmit' if it's recalled, terminated or declined." });
        //                    } // ViewBag.UserMsg = "Transfer request sent successfully.";
        //                }
        //                else if (taskIndx == 3)
        //                {
        //                    if (oMemTransf.ReqStatus != "P" && oMemTransf.ReqStatus != "I")  //RECALL
        //                    {// ModelState.AddModelError(string.Empty, "Request must be 'Pending' or 'In Progress' to be recalled. Hint: Try 'Terminate and Resubmit request'.");  return Json(false);
        //                        return Json(new { taskSuccess = false, userMess = "Request must be 'Pending' or 'In Progress' to be recalled. Hint: Try 'Terminate request' instead'." });
        //                    } // ViewBag.UserMsg = "Transfer request recalled successfully.";
        //                }
        //                else if (taskIndx == 4)
        //                {
        //                    if (oMemTransf.ReqStatus != "P" && oMemTransf.ReqStatus != "I")  //TERMINATE
        //                    {
        //                        //ModelState.AddModelError(string.Empty, "Request cannot be terminated. Request must be Pending or In Progress.  Hint: Try 'Resubmit request' if request is 'Closed'.");
        //                        return Json(new { taskSuccess = false, userMess = "Request cannot be terminated. Request must be Pending or In Progress.  Hint: Try 'Resubmit request' if request is 'Closed'." });
        //                    }
        //                }
        //                else if (taskIndx == 5)  //RESUBMIT... recalled, declined ...............|| oMemTransf.ReqStatus == "X"
        //                { //ProcessCode = "TRF_OT" 
        //                    if (!((oMemTransf.ReqStatus == "R" || oMemTransf.ReqStatus == "D"))) //|| (oMemTransf.ApprovalStatus == "R" || oMemTransf.ApprovalStatus == "X" || oMemTransf.ApprovalStatus == "D"))) // resubmit // Recalled, Terminated, Declined //
        //                    { // ModelState.AddModelError(string.Empty, "Resubmission is possible ONLY for transfer requests that have been Recalled, Terminated or Declined.");   return Json(false); 
        //                        return Json(new { taskSuccess = false, userMess = "Resubmission is possible ONLY for transfer requests recalled or declined." });
        //                    } // ViewBag.UserMsg = "Transfer Request resubmitted successfully.";
        //                }
        //            }

        //            else // ACKNOWLEDGE-APROVE-DECLINE-SUSPEND-FORCE COMPLETE
        //            {
        //                if (taskIndx == 6)  //ACK
        //                {
        //                    if (!((oMemTransf.CurrentScope == "I" && oMemTransf.ReqStatus == "P" && oMemTransf.ApprovalStatus == "P") ||
        //                            (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ||
        //                            (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "A" && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))))   //(oMemTransf.ReqStatus != "P")  // && oMemTransf.ReqStatus != "R")  //draft or recalled
        //                    {  //ModelState.AddModelError(string.Empty, "Request must be 'Pending' to be acknowledged.");  return Json(false);
        //                        return Json(new { taskSuccess = false, userMess = "Request must be 'Pending' or 'Approved' to be acknowledged." });
        //                    }
        //                }
        //                else if (taskIndx >= 7 && taskIndx <= 10)
        //                {
        //                    if (!((oMemTransf.ReqStatus == "I" && (oMemTransf.ApprovalStatus == "P" || oMemTransf.ApprovalStatus == "I" || oMemTransf.ApprovalStatus == "H")) ||
        //                            (oMemTransf.ReqStatus == "A" || oMemTransf.ReqStatus == "D")))
        //                    //(oMemTransf.ReqStatus != "I")
        //                    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Request must first be acknowledged or be 'in progress' or still open for approver to take action. Hint: try to Acknowledge first." });
        //                    }
        //                }
        //                else if (taskIndx == 11)
        //                {
        //                    if (!(oMemTransf.ReqStatus == "C" && oMemTransf.Status == "U" && oMemTransf.ApprovalStatus == "D")) //not Archived (Z)
        //                    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Only unarchived, declined and closed requests can be re-opened." });
        //                    }
        //                }
        //                else if (taskIndx == 12)
        //                {
        //                    if (!((oMemTransf.ReqStatus == "C" || oMemTransf.ReqStatus == "X" || oMemTransf.ReqStatus == "R") && (oMemTransf.Status == "U" || oMemTransf.Status == "Y"))) // && oMemTransf.ApprovalStatus == "D")) //not Archived (Z)
        //                    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Only closed or aborted requests can be archived." });
        //                    }
        //                }
        //            }
        //        }

        //        //Create the approval process...
        //        //this approval is for OUTGOING... but internal. EXTERNAL will be triggered by the Acknowledgment separately... NEW Send Req or DRAFTed and now Send Req... jux make sure user does not send twice

        //        if (taskIndx == 2 || taskIndx == 5)
        //        {
        //            List<ApprovalAction> oAppActionList = new List<ApprovalAction>();
        //            List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
        //            var oAppProStepList = _context.ApprovalProcessStep.Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
        //                .Where(c => c.ChurchBody.AppGlobalOwnerId == oMemTransf.RequestorChurchBody.AppGlobalOwnerId && // c.ApprovalProcess.ChurchBody.AppGlobalOwnerId == oMemTransf.RequestorChurchBody.AppGlobalOwnerId &&
        //                    c.ApprovalProcess.ChurchLevelId == oMemTransf.RequestorChurchBody.ChurchLevelId &&
        //                    c.ApprovalProcess.ProcessCode == "TRF_OT" && c.ApprovalProcess.ProcessSubCode == "MT" && c.ApprovalProcess.ProcessStatus == "A" && c.StepStatus == "A")
        //                .ToList();

        //            //create approval action... at least  one approval level
        //            if (oAppProStepList.Count > 0)
        //            {
        //                oAppActionList.Add(
        //                    new ApprovalAction
        //                    {
        //                        //Id = 0,
        //                        ChurchBodyId = oMemTransf.RequestorChurchBodyId,
        //                        ChurchBody = oMemTransf.RequestorChurchBody,
        //                        ApprovalActionDesc = "Outgoing Member Transfer",
        //                        ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
        //                        ProcessCode = "TRF",
        //                        ProcessSubCode = "MT",
        //                        ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
        //                        ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
        //                        CallerRefId = oMemTransf.Id,  //reference to the Transfer details
        //                        CurrentScope = "I",
        //                        Status = "A",
        //                        //ActionDate = null,
        //                        //Comments = "",
        //                        ActionRequestDate = DateTime.Now,
        //                        Created = DateTime.Now,
        //                        LastMod = DateTime.Now
        //                    });

        //                if (oAppActionList.Count > 0)
        //                    _context.Add(oAppActionList.FirstOrDefault());

        //                //create approval action steps
        //                var stepIndexLowest = oAppProStepList[0].StepIndex;
        //                foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
        //                {
        //                    var oCurrApprover = _context.MemberLeaderRole.Where(c => c.ChurchBodyId == oMemTransf.RequestorChurchBodyId && c.LeaderRoleId == oAppProStep.ApproverLeaderRoleId && c.IsCurrServing == true && c.IsCoreRole == true).FirstOrDefault();
        //                    if (oCurrApprover == null)
        //                    {
        //                        // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");
        //                        return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
        //                    }

        //                    stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
        //                    oAppActionStepList.Add(
        //                        new ApprovalActionStep
        //                        {
        //                            //Id = 0,
        //                            ChurchBodyId = oMemTransf.RequestorChurchBodyId,
        //                            ChurchBody = oMemTransf.RequestorChurchBody,
        //                            MemberLeaderRoleId = oCurrApprover != null ? oCurrApprover.Id : (int?)null,
        //                            Approver = oCurrApprover,
        //                            ActionStepDesc = oAppProStep.StepDesc,
        //                            ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
        //                            ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
        //                            ProcessStepRefId = oAppProStep.Id,
        //                            CurrentScope = "I",
        //                            StepRequestDate = DateTime.Now,
        //                            //Comments="",
        //                            //CurrentStep= true,
        //                            //ActionByLeaderRoleId=null,  // actual approver
        //                            //ActionDate = DateTime.Now,
        //                            Status = "A", //Active
        //                            Created = DateTime.Now,
        //                            LastMod = DateTime.Now,
        //                            //
        //                            ApprovalActionId = oAppActionList.FirstOrDefault().Id,
        //                            ApprovalAction = oAppActionList.FirstOrDefault()
        //                        });
        //                }

        //                foreach (ApprovalActionStep oAS in oAppActionStepList)
        //                {
        //                    oAS.CurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled   // if (oAS.CurrentStep) oCurrApprovers.Add(oAS);
        //                    _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }                                                  
        //                }

        //                oMemTransf.ReqStatus = "P"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //                oMemTransf.RequestDate = DateTime.Now;
        //                //oMemTransf.ActualRequestDate = null;  // after approval or acknowledgment from the FROM congregation
        //                oMemTransf.RequireApproval = oAppActionList.Count > 0; // _oMemTransf_VM.RequireApproval,
        //                oMemTransf.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
        //                                                 //
        //                oMemTransf.IApprovalActionId = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oMemTransf_VM.IApprovalActionId,
        //                oMemTransf.IApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction,
        //            }
        //        }
        //        if (taskIndx == 3 || taskIndx == 4)   //Recall, Terminate
        //        {
        //            string strApprovalStatus = null;
        //            var oActionStepList = vmMod.lsCurrApprActionSteps;

        //            //create approval action
        //            if (oActionStepList.Count > 0)
        //            {
        //                foreach (ApprovalActionStep oAAStep in oActionStepList)
        //                {
        //                    oAAStep.ActionStepStatus = taskIndx == 3 ? "R" : "X";
        //                    oAAStep.Comments = vmMod.strApproverComment;  //could also be reason from the user/applicant
        //                    oAAStep.ActionDate = DateTime.Now;
        //                    oAAStep.LastMod = DateTime.Now;

        //                    //add appproval notifiers
        //                    oCurrApprovers.Add(oAAStep);
        //                    _context.Update(oAAStep);
        //                }

        //                var oAA = oActionStepList[0].ApprovalAction;
        //                strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, taskIndx == 3 ? "R" : "X");   // this.GetApprovalActionStatus(oActionStepList, oMemTransf.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //                oAA.ActionStatus = strApprovalStatus; // this.GetApprovalActionStatus(oActionStepList);   //this.GetApprovalActionStatus(oActionStepList, oMemTransf.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //                oAA.Comments = vmMod.strApproverComment;   //could also be reason from the user/applicant
        //                oAA.LastActionDate = DateTime.Now;
        //                oAA.LastMod = DateTime.Now;

        //                _context.Update(oAA);
        //            }

        //            oMemTransf.ReqStatus = taskIndx == 3 ? "R" : "X"; // GetApprovalActionStatus(oActionStepList, oMemTransf.CurrentScope == "I" ? 1 : 2, taskIndx == 3 ? "R" : "X"); // taskIndx == 3 ? "R" : "X"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //            oMemTransf.RequestDate = DateTime.Now;
        //            oMemTransf.ApprovalStatus = strApprovalStatus; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
        //            oMemTransf.Status = "U"; // Unsuccessful
        //            //if (strApprovalStatus == null)
        //            //{
        //            //    oMemTransf.IApprovalActionId = null;
        //            //    oMemTransf.IApprovalAction = null;
        //            //}
        //        }
        //        if (taskIndx == 6)
        //        {
        //            if ((oMemTransf.CurrentScope == "I" && oMemTransf.ReqStatus == "P" && oMemTransf.ApprovalStatus == "P") ||
        //                            (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))
        //            {  //first... @Pending... //@In Progress... acknowledge --> Pending of ToChurchBody approvers
        //                oMemTransf.ReqStatus = "I"; // Leave all approval action/step in Pending until approver works on ...
        //                if (oMemTransf.CurrentScope == "I")
        //                {
        //                    oMemTransf.ToReceivedDate = null;
        //                    oMemTransf.ReceivedDate = DateTime.Now;
        //                    oMemTransf.Status = "I"; //Approval Pending 

        //                    //notify the approvers now... 
        //                    var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                        .Where(c => c.ChurchBodyId == oMemTransf.RequestorChurchBodyId &&
        //                                    //(c.CurrentScope == "I" || (c.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P")) && 
        //                                    c.ApprovalActionId == oMemTransf.IApprovalActionId && c.CurrentStep == true).ToList();
        //                    oCurrApprovers.AddRange(oAASList);
        //                }

        //                else if (oMemTransf.CurrentScope == "E")
        //                {
        //                    oMemTransf.ToReceivedDate = DateTime.Now; // oMemTransf.Status = "I"; //Approval Pending

        //                    //notify the approvers now... 
        //                    var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                        .Where(c => c.ChurchBodyId == oMemTransf.RequestorChurchBodyId &&
        //                                    //(c.CurrentScope == "E" && oMemTransf.ApprovalStatus != "P" && 
        //                                    c.ApprovalActionId == oMemTransf.EApprovalActionId && c.CurrentStep == true).ToList();
        //                    oCurrApprovers.AddRange(oAASList);
        //                }
        //            }

        //            else if (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "A" && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))
        //            {
        //                //perform the transfer ...and then Close the request after 'success'.
        //                oMemTransf.TransferDate = DateTime.Now;    ///same date to be used as the Enrollment date /Departure date for member

        //                if (this.PerformMemberTransfer(_context, oMemTransf, vmMod.oChurchBody.Id) == false)
        //                {  //reverse action .....    ModelState.AddModelError(string.Empty, "Member transfer unsuccessful.");
        //                    return Json(new { taskSuccess = false, userMess = "Member transfer unsuccessful. Failed operation reversing..." });
        //                }

        //                //notify all appprovers
        //                var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                    .Where(c => c.ChurchBodyId == oMemTransf.RequestorChurchBodyId &&
        //                                    (c.ApprovalActionId == oMemTransf.IApprovalActionId || c.ApprovalActionId == oMemTransf.EApprovalActionId)
        //                                    ).ToList();
        //                oCurrApprovers.AddRange(oAASList);

        //                //update... you can reverse in case...
        //                oMemTransf.ReqStatus = "C";     //closed... From or requaesting congregation MUST acknowledge                         
        //                oMemTransf.Status = "Y"; //Yes... Transfer done
        //            }
        //        }

        //        if (taskIndx >= 7 && taskIndx <= 9)  // Approve-Decline-Suspend-Force Complete    && vm.lsCurrApprActionSteps.Count > 0)
        //        {
        //            string strApprovalStatus = null;
        //            if (vmMod.oCurrApprovalActionStep == null || vmMod.oCurrApprovalAction == null || vmMod.lsCurrApprActionSteps.Count == 0)
        //            {  //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again."); return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
        //            }

        //            //update approval action step

        //            //var oAAStepList = _context.ApprovalActionStep.Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //            //    .Where(c => c.Id == vmMod.currApprovalActionId).ToList();

        //            //var oAAStep = _context.ApprovalActionStep.Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t=>t.ContactInfo)
        //            //    .Where(c => c.Id == vmMod.currApprovalActionStepId).FirstOrDefault() ; // .Find(vmMod.currApprovalActionStepId);  //.oCurrApprovalActionStep.Id);

        //            var oActionStepList = vmMod.lsCurrApprActionSteps;
        //            var oAAStep = oActionStepList.Where(c => c.Id == vmMod.currApprovalActionStepId).FirstOrDefault();
        //            if (oAAStep == null)
        //            { //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again."); return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
        //            }

        //            //add appproval notifiers
        //            //oCurrApprovers.Add(oAAStep);

        //            var oApprover = _context.MemberLeaderRole.Where(c => ((oMemTransf.CurrentScope == "E" && c.ChurchBodyId == oMemTransf.ToChurchBodyId) || (oMemTransf.CurrentScope == "I" && c.ChurchBodyId == oMemTransf.RequestorChurchBodyId)) &&
        //                            c.ChurchMemberId == vmMod.oCurrLoggedMemberId && c.IsCurrServing == true && c.IsCoreRole == true).FirstOrDefault();
        //            if (oApprover == null)
        //            { //ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again."); return Json(false); 
        //                return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
        //            }

        //            oAAStep.ActionStepStatus = taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : taskIndx == 9 ? "H" : oAAStep.ActionStepStatus;
        //            if (taskIndx == 8 || taskIndx == 9)
        //            {
        //                oAAStep.Comments = apprComment;   //could also be reason from the user/applicant
        //            }
        //            oAAStep.ActionDate = DateTime.Now;
        //            oAAStep.ActionByMemberLeaderRoleId = oApprover.Id;
        //            oAAStep.ActionBy = oApprover;
        //            oAAStep.LastMod = DateTime.Now;

        //            //if step Approved.. get next step and notify approvers...
        //            if (oAAStep.ActionStepStatus == "A")
        //            {
        //                var oNextAAStep = oActionStepList.Where(c => c.ApprovalStepIndex == oAAStep.ApprovalStepIndex + 1).ToList();
        //                if (oNextAAStep.Count > 0)
        //                {
        //                    oAAStep.CurrentStep = false;
        //                    foreach (var oNextStep in oNextAAStep)
        //                    {
        //                        oNextStep.CurrentStep = true;
        //                        oNextStep.LastMod = DateTime.Now;
        //                        //
        //                        oCurrApprovers.Add(oNextStep);
        //                        _context.Update(oNextStep);
        //                    }
        //                }
        //            }

        //            //
        //            vmMod.oCurrApprovalActionStep = oAAStep;
        //            for (int i = 0; i < oActionStepList.Count; i++)
        //            {
        //                if (oActionStepList[i].Id == oAAStep.Id) { oActionStepList[i] = oAAStep; break; }
        //            }

        //            _context.Update(oAAStep);


        //            //try
        //            //{  // _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //            //    _context.Update(oAAStep);
        //            //}
        //            //catch (Exception ex)
        //            //{ return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." }); }


        //            //approval action       
        //            var oAA = _context.ApprovalAction.Find(vmMod.currApprovalActionId); //oCurrApprovalAction.Id);
        //            if (oAA == null)
        //            { //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again.");  return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
        //            }

        //            strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : "H"); // taskIndx == 9 ? "H");
        //            oAA.ActionStatus = strApprovalStatus; // this.GetApprovalActionStatus(oActionStepList);  //strApprovalStatus;
        //            oAA.Comments = vmMod.strApproverComment;   //could also be reason from the user/applicant
        //            oAA.LastActionDate = DateTime.Now;
        //            oAA.LastMod = DateTime.Now;

        //            try
        //            {  // _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //                _context.Update(oAA);
        //            }
        //            catch (Exception ex)
        //            { return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." }); }

        //            //member transfer
        //            // strApprovalStatus = this.GetApprovalActionStatus(oActionStepList);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //            oMemTransf.ReqStatus = this.GetApprovalActionStatus(oActionStepList, oMemTransf.CurrentScope == "I" ? 1 : 2, taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : "H"); //, oMemTransf.CurrentScope == "I" || taskIndx == 9 ? 1 : 2);// taskIndx == 3 ? "R" : "X"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //            //oMemTransf.RequestDate = DateTime.Now;
        //            oMemTransf.ApprovalStatus = strApprovalStatus;// this.GetApprovalActionStatus(oActionStepList);  // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,

        //            oMemTransf.Status = (strApprovalStatus == "F" || (strApprovalStatus == "A" && oMemTransf.CurrentScope == "E")) ? "T" :
        //                                 strApprovalStatus == "D" ? "U" : strApprovalStatus == "H" ? "I" : oMemTransf.Status; //await ack... In transit else Unsuccessful    
        //            if (oMemTransf.Status == "U")
        //            {
        //                oMemTransf.ReqStatus = "C";     //closed... can be re-Opened at current scope for reverse actions... Change "C" to "I" and await any more step... [approve, decline] for next possible status
        //                oMemTransf.TransferDate = null;
        //            }

        //            //
        //            //if (strApprovalStatus == "F" || (strApprovalStatus == "A" || oMemTransf.CurrentScope == "E")) oMemTransf.Status = "T";
        //            //else if (strApprovalStatus == "D") oMemTransf.Status = "U";
        //            //else if (strApprovalStatus == "H") oMemTransf.Status = "I";


        //            //if Approved... and scope == internal and status==In Progress
        //            if (taskIndx == 7 && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))  //(taskIndx == 7 && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))
        //            {
        //                //  oMemTransf.Status = "T";strApprovalStatus == "A" || strApprovalStatus == "F" ? "T" : strApprovalStatus == "D" ? "U" : oMemTransf.Status; //await ack... In transit else Unsuccessful    

        //                if (oMemTransf.CurrentScope == "I" && oMemTransf.ReqStatus == "I") //&& (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))
        //                {
        //                    //
        //                    List<ApprovalAction> oAppActionList = new List<ApprovalAction>();
        //                    List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
        //                    var oAppProStepList = _context.ApprovalProcessStep.Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
        //                        .Where(c => c.ChurchBody.AppGlobalOwnerId == oMemTransf.ToChurchBody.AppGlobalOwnerId && c.ApprovalProcess.ChurchBody.AppGlobalOwnerId == oMemTransf.ToChurchBody.AppGlobalOwnerId &&
        //                                 c.ApprovalProcess.ChurchLevelId == oMemTransf.ToChurchBody.ChurchLevelId &&
        //                            c.ApprovalProcess.ProcessCode == "TRF_IN" && c.ApprovalProcess.ProcessSubCode == "MT" && c.ApprovalProcess.ProcessStatus == "A" && c.StepStatus == "A")
        //                        .ToList();

        //                    //create approval action
        //                    if (oAppProStepList.Count > 0)
        //                    {
        //                        oAppActionList.Add(
        //                        new ApprovalAction
        //                        {
        //                            //Id = 0,
        //                            ChurchBodyId = oMemTransf.RequestorChurchBodyId,
        //                            ChurchBody = oMemTransf.RequestorChurchBody,
        //                            ApprovalActionDesc = "Incoming Member Transfer",
        //                            ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
        //                            ProcessCode = "TRF",
        //                            ProcessSubCode = "MT",
        //                            ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
        //                            ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
        //                            CallerRefId = oMemTransf.Id,  //reference to the Transfer details
        //                            CurrentScope = "E",
        //                            Status = "A",
        //                            //ActionDate = null,
        //                            //Comments = "",
        //                            ActionRequestDate = DateTime.Now,
        //                            Created = DateTime.Now,
        //                            LastMod = DateTime.Now
        //                        });

        //                        if (oAppActionList.Count > 0)
        //                            _context.Add(oAppActionList.FirstOrDefault());

        //                        //create approval action steps
        //                        var stepIndexLowest = 1;
        //                        if (oAppProStepList.Count > 0) stepIndexLowest = oAppProStepList[0].StepIndex;
        //                        foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
        //                        {
        //                            var oICurrApprover = _context.MemberLeaderRole.Where(c => c.ChurchBodyId == oMemTransf.ToChurchBodyId && c.LeaderRoleId == oAppProStep.ApproverLeaderRoleId &&
        //                                                                                c.IsCurrServing == true && c.IsCoreRole == true).FirstOrDefault();
        //                            if (oICurrApprover == null)
        //                            { // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");  return Json(false);
        //                                return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
        //                            }

        //                            stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
        //                            oAppActionStepList.Add(
        //                                new ApprovalActionStep
        //                                {
        //                                    //Id = 0,
        //                                    ChurchBodyId = oMemTransf.RequestorChurchBodyId,
        //                                    ChurchBody = oMemTransf.RequestorChurchBody,
        //                                    MemberLeaderRoleId = oICurrApprover.Id,
        //                                    Approver = oICurrApprover,
        //                                    ActionStepDesc = oAppProStep.StepDesc,
        //                                    ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
        //                                    ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
        //                                    ProcessStepRefId = oAppProStep.Id,
        //                                    CurrentScope = "E",
        //                                    StepRequestDate = DateTime.Now,
        //                                    // Comments="",
        //                                    // CurrentStep= true,
        //                                    // ActionByMemberLeaderRoleId = vmMod.oCurrLoggedMemberId,  // actual approver
        //                                    // ActionBy = vmMod.oCurrLoggedMember,
        //                                    // ActionDate = DateTime.Now,
        //                                    Status = "A", //Active
        //                                    Created = DateTime.Now,
        //                                    LastMod = DateTime.Now,

        //                                    //
        //                                    ApprovalActionId = oAppActionList.FirstOrDefault().Id,
        //                                    ApprovalAction = oAppActionList.FirstOrDefault()
        //                                }); ;
        //                        }

        //                        foreach (ApprovalActionStep oAS in oAppActionStepList)
        //                        {
        //                            oAS.CurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled 
        //                            _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }
        //                        }

        //                        //oMemTransf.ReqStatus = "I"; //keep as it is == in prrogres. to the applicant.. Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //                        // oMemTransf.RequestDate = DateTime.Now;
        //                        oMemTransf.ToRequestDate = DateTime.Now;   // after approval or acknowledgment from the FROM congregation
        //                        oMemTransf.RequireApproval = oAppActionList.Count > 0; // _oMemTransf_VM.RequireApproval,
        //                        oMemTransf.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,
        //                        oMemTransf.CurrentScope = "E";
        //                        oMemTransf.EApprovalActionId = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oMemTransf_VM.IApprovalActionId,
        //                        oMemTransf.EApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oMemTransf_VM.IApprovalAction, 
        //                    }
        //                }

        //                ////perform the transfers
        //                //else if (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "A" && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))
        //                //{
        //                //    if (! this.PerformMemberTransfer(_context, oMemTransf, vmMod.oChurchBody.Id))
        //                //    {  //reverse action
        //                //       // ModelState.AddModelError(string.Empty, "Member transfer unsuccessful.");
        //                //        return Json(new { taskSuccess = false, userMess = "Member transfer unsuccessful. Failed operation reversing..." });
        //                //    }
        //                //}
        //            }
        //        }

        //        if (taskIndx == 10) // && vm.lsCurrApprActionSteps.Count > 0)  //FORCE-COMPLETE ...iirespective of the state
        //        {
        //            string strApprovalStatus = null;
        //            var oActionStepList = vmMod.lsCurrApprActionSteps;

        //            //create approval action...  force-complete rest of the steps  unapproved.. at least 1 step of approval
        //            if (oActionStepList.Count > 0)
        //            {
        //                foreach (ApprovalActionStep oAAStep in oActionStepList)
        //                {
        //                    oAAStep.ActionStepStatus = "F";
        //                    oAAStep.Comments = vmMod.strApproverComment;  //could also be reason from the user/applicant
        //                    oAAStep.ActionDate = DateTime.Now;
        //                    oAAStep.LastMod = DateTime.Now;

        //                    _context.Update(oAAStep);
        //                }

        //                var oAA = oActionStepList[0].ApprovalAction; // not null
        //                strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, "F"); // this.GetApprovalActionStatus(oActionStepList, oMemTransf.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //                oAA.ActionStatus = strApprovalStatus;
        //                oAA.Comments = vmMod.strApproverComment;   //could also be reason from the user/applicant
        //                oAA.LastActionDate = DateTime.Now;
        //                oAA.LastMod = DateTime.Now;

        //                _context.Update(oAA);

        //                oMemTransf.ReqStatus = this.GetApprovalActionStatus(oActionStepList, oMemTransf.CurrentScope == "I" ? 1 : 2, "F"); // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //                oMemTransf.RequestDate = DateTime.Now;
        //                oMemTransf.ApprovalStatus = strApprovalStatus; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oMemTransf_VM.ApprovalStatus,   
        //                oMemTransf.Status = "T"; //await ack... In transit
        //            }
        //        }

        //        if (taskIndx == 11) // Re-open request
        //        {
        //            //if (vmMod.lsCurrApprActionSteps.Count > 0)
        //            //{ 
        //            oMemTransf.ReqStatus = "D"; //only declined actions are re-opened
        //            oMemTransf.Status = "I"; //await ack... In transit
        //                                     // }
        //        }

        //        if (taskIndx == 12) // Archive request
        //        {
        //            oMemTransf.ReqStatus = "Z";
        //            // oMemTransf.Status = "I";  KEEP THE STATUS
        //        }

        //        //save details... locAddr
        //        List<ChurchMember> oChuMemNotifyList = new List<ChurchMember>();
        //        try
        //        {
        //            //if (oMemTransf == null)
        //            //    { //ModelState.AddModelError(string.Empty, "Member Transfer data not found! Please refresh and try again."); return Json(false);
        //            //    return Json(new { taskSuccess = false, userMess = "Member Transfer data not found! Please refresh and try again." });
        //            //}

        //            if (oMemTransf.Id > 0 && taskIndx > 1) // new or update instances already dealt with up there
        //            {
        //                try
        //                {
        //                    oMemTransf.LastMod = DateTime.Now;
        //                    _context.Update(oMemTransf);
        //                    await _context.SaveChangesAsync();
        //                }
        //                catch (Exception ex)
        //                {
        //                    return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
        //                }
        //            }


        //            //if (oMemTransf.Id == 0)
        //            //{
        //            //    switch (taskIndx)
        //            //    {
        //            //        case 1:
        //            //            oUserMsg = "Saved member transfer details successfully.";
        //            //            break;

        //            //        case 2:
        //            //            oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //            //            oNotifMsg = "Member transfer request sent successfully. #Request_Id: " + oMemTransf.Id;
        //            //            oUserMsg = oNotifMsg;
        //            //            break;
        //            //    }
        //            //}

        //            var oNotifMsg = ""; var oUserMsg = "";
        //            switch (taskIndx)
        //            {
        //                case 1:
        //                    oUserMsg = "Saved member transfer details successfully.";
        //                    break;

        //                case 2:  //send 
        //                    oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                    oNotifMsg = "Member transfer request to " + oMemTransf.ToChurchBody.Name + " sent successfully. #Request_Id: " + oMemTransf.Id;
        //                    oUserMsg = oNotifMsg;
        //                    break;

        //                case 3:  //recall
        //                    { //check for steps available... sent notification to all approvers and the applicant, FromChurchBody, ToChurchBody
        //                        foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                            if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                    oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);  //requestor : [to], approvers [ to /from]
        //                        oNotifMsg = "Member transfer request from " + oMemTransf.FromChurchBody.Name + " to " + oMemTransf.ToChurchBody.Name +
        //                            " has been recalled [by requestor/congregation]." +
        //                            Environment.NewLine + "#Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = oNotifMsg;
        //                    }
        //                    break;

        //                case 4: //cancel
        //                    { //check for steps available... sent notification to all approvers and the applicant, FromChurchBody, ToChurchBody
        //                        foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                            if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                    oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                        oNotifMsg = "Member transfer request from " + oMemTransf.FromChurchBody.Name + "to " + oMemTransf.ToChurchBody.Name +
        //                            //(oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                            " has been terminated [by requestor/congregation]. #Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = oNotifMsg;
        //                    }
        //                    break;

        //                case 5: //resubmit
        //                    {
        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                        oNotifMsg = "Member transfer request from " + oMemTransf.FromChurchBody.Name +
        //                            // (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                            " has been resubmitted successfully. #Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = oNotifMsg;
        //                    }
        //                    break;

        //                case 6: //ack
        //                    {
        //                        oNotifMsg = "Member transfer request ";
        //                        if (oMemTransf.CurrentScope == "I" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P")  // (oMemTransf.CurrentScope == "I") 
        //                            oNotifMsg += "to " + oMemTransf.ToChurchBody.Name + " has been received and being processed.";
        //                        //+ oMemTransf.FromChurchBody.Name.ToUpper() + " to " + oMemTransf.ToChurchBody.Name.ToUpper() + " has been received and being processed.";
        //                        else if (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P")
        //                            oNotifMsg += "from " + oMemTransf.FromChurchBody.Name + " duly acknowledged by target congregation, and pending approval.";
        //                        //oMemTransf.FromChurchBody.Name.ToUpper() + " to " + oMemTransf.ToChurchBody.Name.ToUpper() + " duly acknowledged by target congregation, and pending approval.";
        //                        else if (oMemTransf.CurrentScope == "E" && (oMemTransf.ReqStatus == "A" || oMemTransf.ReqStatus == "C") && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))
        //                            oNotifMsg += "from " + oMemTransf.FromChurchBody.Name + " to " + oMemTransf.ToChurchBody.Name + " successfully completed. Membership has been transferred.";

        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                        oNotifMsg += " #Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = "Member transfer request " +
        //                            (oMemTransf.CurrentScope == "I" ? "to " + oMemTransf.ToChurchBody.Name + " acknowledged." :
        //                            (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "A" && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F")) ? "from " + oMemTransf.FromChurchBody.Name + " to " + oMemTransf.ToChurchBody.Name + " acknowledged and successfully completed. Member has been transferred." :
        //                            "from " + oMemTransf.FromChurchBody.Name + " acknowledged.");
        //                    }
        //                    break;


        //                //        (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "A" && (oMemTransf.ApprovalStatus == "A" || oMemTransf.ApprovalStatus == "F"))
        //                //else if ((oMemTransf.CurrentScope == "I" && oMemTransf.ReqStatus == "P" && oMemTransf.ApprovalStatus == "P") ||
        //                //                (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))

        //                case 7:
        //                    {
        //                        if (oMemTransf.ReqStatus == "A")
        //                        {
        //                            foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                                if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                        oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                            oNotifMsg = "Member transfer request " +
        //                                (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name);
        //                            if (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))
        //                                oNotifMsg += " has been approved. Target congregation approval underway."; // and then forwarded to the target congregation.";
        //                            else
        //                                oNotifMsg += " successfully approved, pending acknowledgement from requesting congregation, " + oMemTransf.FromChurchBody.Name + " to effect the transfer";
        //                        }

        //                        else
        //                        {
        //                            int stepsLeft = 0;
        //                            foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                                if (oStep.ActionStepStatus != "A" && oStep.ActionStepStatus != "F")
        //                                    stepsLeft++;
        //                            //
        //                            ChurchMember t_cm = null; string strApproverName = "[Approver]";
        //                            if (vmMod.oCurrApprovalActionStep.Approver != null)
        //                                if (vmMod.oCurrApprovalActionStep.Approver.ChurchMember != null)
        //                                {
        //                                    t_cm = vmMod.oCurrApprovalActionStep.Approver.ChurchMember;
        //                                    // oChuMemNotifyList.Add(t_cm);
        //                                    strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
        //                                    //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim();
        //                                    var t_lr = vmMod.oCurrApprovalActionStep.Approver.LeaderRole;
        //                                    strApproverName += (t_lr != null ? (!string.IsNullOrEmpty(t_lr.RoleName) ? " (" + t_lr.RoleName + ")" : "") : "").Trim();
        //                                }

        //                            oNotifMsg = "Member transfer request " + (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name);
        //                            oNotifMsg += " is " + this.GetRequestProcessStatusDesc(oMemTransf.ReqStatus) + ". " + Environment.NewLine + strApproverName + " has approved " + (stepsLeft > 0 ? ". " + stepsLeft + " more approval" + (stepsLeft > 1 ? "s" : "").ToString() + " left to" : "");
        //                            if (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))
        //                                oNotifMsg += (oMemTransf.ApprovalStatus == "A" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P")) ? ". Request forwarded to target congregation." : " forward request to target congregation.";
        //                            else
        //                                oNotifMsg += oMemTransf.ApprovalStatus == "A" ? ", pending acknowledgement from requesting congregation." : " complete.";
        //                        }

        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                        oNotifMsg += " #Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = "Member transfer request " +
        //                            (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                            " approved.";
        //                    }
        //                    break;

        //                case 8:
        //                    {
        //                        //if (oMemTransf.ReqStatus == "D")
        //                        //{ 
        //                        foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                            if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                    oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                        oNotifMsg = "Member transfer request " +
        //                            (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                            " has been ";
        //                        if (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))
        //                            oNotifMsg += "declined by the local congregation.";
        //                        else
        //                            oNotifMsg += "unfortunately declined by target congregation.";
        //                        //}

        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                        oNotifMsg += " Please check notes attached. #Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = "Member transfer request " +
        //                            (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                            " sucessfully declined.";
        //                    }
        //                    break;

        //                case 9:
        //                    {
        //                        //if (oMemTransf.ReqStatus == "I")
        //                        //{
        //                        ChurchMember t_cm = null; string strApproverName = "[Approver]";
        //                        if (vmMod.oCurrApprovalActionStep.Approver != null)
        //                            if (vmMod.oCurrApprovalActionStep.Approver.ChurchMember != null)
        //                            {
        //                                t_cm = vmMod.oCurrApprovalActionStep.Approver.ChurchMember;      // oChuMemNotifyList.Add(t_cm);
        //                                strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
        //                                //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim();
        //                                var t_lr = vmMod.oCurrApprovalActionStep.Approver.LeaderRole;
        //                                strApproverName += (t_lr != null ? (!string.IsNullOrEmpty(t_lr.RoleName) ? " (" + t_lr.RoleName + ")" : "") : "").Trim();
        //                            }

        //                        oNotifMsg = "Member transfer request " +
        //                            (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                            " has been put 'On hold' tentative by " + strApproverName + ". Please check notes attached.";
        //                        //if (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))
        //                        //    oNotifMsg += "forward request to target congregation.";
        //                        //else
        //                        //    oNotifMsg += "successfuly effect the transfer to target congregation.";

        //                        oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                        oNotifMsg += " #Request_Id: " + oMemTransf.Id;
        //                        oUserMsg = "Approval step suspended successfully for Member transfer request " +
        //                            (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name);
        //                        //}
        //                    }
        //                    break;

        //                case 10:
        //                    //if (oMemTransf.ReqStatus == "A")
        //                    //{
        //                    foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                        if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                    oNotifMsg = "Member transfer request " +
        //                        (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                        oMemTransf.ToChurchBody.Name.ToUpper();
        //                    if (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P"))
        //                        oNotifMsg += " has been approved [force-completed] by local congregation and forwarded to the target congregation pending their approval."; // and then forwarded to the target congregation.";
        //                    else
        //                        oNotifMsg += " successfully approved [force-completed].";

        //                    oChuMemNotifyList.Add(oMemTransf.ChurchMemberTransf);
        //                    oNotifMsg += " #Request_Id: " + oMemTransf.Id;
        //                    oUserMsg = "Member transfer request " +
        //                        (oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? "to " + oMemTransf.ToChurchBody.Name : "from " + oMemTransf.FromChurchBody.Name) +
        //                        " force-completed.";
        //                    break;

        //                case 11:
        //                    oNotifMsg = "Member transfer request from " + oMemTransf.FromChurchBody.Name + " successfully re-opened.";
        //                    oUserMsg = oNotifMsg;
        //                    break;

        //                case 12:
        //                    //  oNotifMsg = "Member transfer request from " + oMemTransf.FromChurchBody.Name + " successfully re-opened.";
        //                    oUserMsg = "Member transfer request from " + oMemTransf.FromChurchBody.Name + " to " + oMemTransf.ToChurchBody.Name +
        //                            " has been archived" +
        //                            Environment.NewLine + "#Request_Id: " + oMemTransf.Id; ;
        //                    break;
        //            }

        //            //construct notification messages
        //            if (oChuMemNotifyList.Count > 0)
        //            {
        //                MailAddressCollection listToAddr = new MailAddressCollection();
        //                MailAddressCollection listCcAddr = new MailAddressCollection();
        //                MailAddressCollection listBccAddr = new MailAddressCollection();
        //                //
        //                List<string> oNotifPhone_List = new List<string>();
        //                List<string> oNotifMessageList = new List<string>();
        //                _context.ContactInfo.Where(c => c.ChurchBodyId == oMemTransf.RequestorChurchBodyId || c.ChurchBodyId == oMemTransf.FromChurchBodyId || c.ChurchBodyId == oMemTransf.ToChurchBodyId).Load();
        //                ContactInfo oMemCI = null;  // _context.ContactInfo.Find(oMemTransf.ChurchMemberTransf.ContactInfoId);   //searches the local cache loaded
        //                var strRequestor = ""; var strSalutation = "";
        //                var _oNotifMsg = oNotifMsg;
        //                var strSenderId = oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ?
        //                    oMemTransf.FromChurchBody.Name : oMemTransf.ToChurchBody.Name;


        //                string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

        //                //var strUrl1 =  HttpContext.Request.Query["ReturnUrl"];

        //                foreach (var oCM in oChuMemNotifyList)
        //                {
        //                    var num = "";
        //                    /*oMemCI = _context.ContactInfo.Find(oMemTransf.ChurchMemberTransf.ContactInfoId);  */ //searches the local cache loaded
        //                    if (oCM.ContactInfoId != null)
        //                        oMemCI = _context.ContactInfo.Find(oCM.ContactInfoId);

        //                    if (oMemCI != null && oCM != null)
        //                    {  //get Custom salution... else use greeeting of the day: morning, afternoon, evening
        //                        //strSalutation = "Asomdwei nka wo!";
        //                        if (string.IsNullOrEmpty(strSalutation))
        //                        {
        //                            var ts = DateTime.Now.TimeOfDay;
        //                            if (ts.Hours >= 0 && ts.Hours < 12) strSalutation = "Good morning";
        //                            else if (ts.Hours <= 16) strSalutation = "Good afternoon";
        //                            else if (ts.Hours < 24) strSalutation = "Good evening";
        //                        }
        //                        if (oMemCI.MobilePhone1 != null)
        //                        { num = oMemCI.MobilePhone1; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
        //                        if (num.Length == 0)
        //                            if (oMemCI.MobilePhone2 != null)
        //                            { num = oMemCI.MobilePhone2; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
        //                        if (num.Length > 0)
        //                        {
        //                            strRequestor = (((oCM.Title + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
        //                            oNotifMsg = strRequestor + ", " + strSalutation + ". Please " + (oMemTransf.ChurchMemberId == oCM.Id ? "your " : strRequestor + ".") + _oNotifMsg + ". Thank you.";
        //                            //
        //                            oNotifPhone_List.Add(num);
        //                            oNotifMessageList.Add(oNotifMsg);

        //                            //email recipients... applicant, church   ... specific e-mail content
        //                            listToAddr.Add(new MailAddress(oMemCI.Email, strRequestor));
        //                            SendEmailNotification(strSenderId, vmMod.strTransferType + ": " + vmMod.strTransfMemberDesc, oNotifMsg +
        //                                Environment.NewLine + "Open request: " + strUrl,
        //                                listToAddr, listCcAddr, listBccAddr, null);
        //                        }
        //                    }
        //                }

        //                //send notifications... sms, email
        //                if (oNotifPhone_List.Count > 0 && oNotifPhone_List.Count == oNotifMessageList.Count)
        //                    SendSMSNotification(oNotifPhone_List, oNotifMessageList, true);

        //                //email... approvers
        //                if (oCurrApprovers.Count > 0)
        //                {
        //                    MailAddressCollection listToAddr_Appr = new MailAddressCollection();
        //                    MailAddressCollection listCcAddr_Appr = new MailAddressCollection();
        //                    MailAddressCollection listBccAddr_Appr = new MailAddressCollection();

        //                    foreach (var oAAS in oCurrApprovers)
        //                    {
        //                        var strApproverName = ""; var t_cm = oAAS.Approver.ChurchMember;
        //                        if (t_cm != null)
        //                        {
        //                            strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
        //                            if (t_cm.ContactInfo != null)
        //                                listToAddr_Appr.Add(new MailAddress(t_cm.ContactInfo.Email, strApproverName));
        //                        }
        //                    }

        //                    if (listToAddr_Appr.Count > 0)
        //                    {  //var strSenderId = oMemTransf.CurrentScope == "I" || (oMemTransf.CurrentScope == "E" && oMemTransf.ReqStatus == "I" && oMemTransf.ApprovalStatus == "P") ? oMemTransf.FromChurchBody.Name : oMemTransf.ToChurchBody.Name;
        //                        SendEmailNotification(strSenderId, vmMod.strTransferType + ": " + vmMod.strTransfMemberDesc, vmMod.strTransMessage +
        //                            Environment.NewLine + "Open request: " + strUrl,
        //                            listToAddr, listCcAddr, listBccAddr, null);
        //                    }
        //                }
        //            }

        //            vmMod.oChurchTransfer = oMemTransf;
        //            TempData.Put("oVmCurr", vmMod);
        //            //  TempData.Keep();

        //            return Json(new { taskSuccess = true, userMess = oUserMsg });

        //            // return Json(true);
        //        }

        //        catch (Exception ex)
        //        {
        //            //  ViewBag.UserMsg = "Requested action could not be performed successfully. Err: " + ex.ToString();
        //            return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });
        //    }
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit_CLGTransf(ChurchTransferModel vmMod)  //save,, string checkedCong int taskIndx
        //{
        //    try
        //    { // Member only, 


        //        List<ApprovalActionStep> oCurrApprovers = new List<ApprovalActionStep>();
        //        //List<ChurchMember> oChuMemNotifyEmailList = new List<ChurchMember>();

        //        if (vmMod.oChurchTransfer.FromChurchBodyId == null)
        //            vmMod.oChurchTransfer.FromChurchBodyId = vmMod.numFromChurchBodyId;

        //        var _oMTChanges = vmMod.oChurchTransfer;  //  has updates 
        //        var taskIndx = vmMod.userRequestTask;
        //        //string[] arr = vmMod.strSelectCong.Split(','); // vmMod.strSelectCong;

        //        var apprComment = vmMod.strApproverComment;
        //        vmMod = TempData.Get<ChurchTransferModel>("oVmCurr"); TempData.Keep();
        //        if (vmMod == null)
        //        { //ModelState.AddModelError(string.Empty, "Data retrieval failed. Please refresh and try again."); return Json(false); 
        //            return Json(new { taskSuccess = false, userMess = "Data retrieval failed. Please refresh and try again." });
        //        }
        //        if (vmMod.oChurchTransfer == null)
        //        { //ModelState.AddModelError(string.Empty, "Data submitted failed for 'Update, Send request' operation to be performed."); return Json(false); 
        //            return Json(new { taskSuccess = false, userMess = "Data submitted failed for 'Update, Send request' operation to be performed." });
        //        }

        //        // DetachAllEntities();

        //        // vmMod.oChurchTransfer = _oMT;
        //        vmMod.userRequestTask = taskIndx;
        //        ChurchTransfer _oCLGTransf = vmMod.oChurchTransfer; // vm.oChurchTransfer; // init...  
        //        ChurchTransfer oCLGTransf = vmMod.oChurchTransfer;
        //        //

        //        ChurchTransfer oTempCT = _oCLGTransf; // vmMod.oChurchTransfer;

        //        //some validations
        //        if (taskIndx == 1)//|| taskIndx == 2)//|| taskIndx == 5
        //            oTempCT = _oMTChanges;

        //        if (string.IsNullOrEmpty(oTempCT.TransferSubType))
        //            return Json(new { taskSuccess = false, userMess = "Specify the task to perform." });

        //        if (oTempCT.TransferSubType.Contains("M"))
        //            oTempCT.ToChurchBodyId = oTempCT.AttachedToChurchBodyId;

        //        if (!oTempCT.TransferSubType.Contains("R"))
        //            oTempCT.DesigRolesList = ""; // vmMod.oChurchTransfer.AttachedToChurchBodyId; }

        //        if (!oTempCT.TransferSubType.Contains("M"))
        //            oTempCT.AttachedToChurchBodyId = null;

        //        if (oTempCT.ToChurchBodyId == null)
        //            return Json(new { taskSuccess = false, userMess = "Target congregation for transfer required." });

        //        if (oTempCT.TransferSubType.Contains("M") && oTempCT.AttachedToChurchBodyId == null)
        //            return Json(new { taskSuccess = false, userMess = "Minister-on transfer must be attached to congregation to transfer membership" });

        //        if (oTempCT.AttachedToChurchBodyList == null)
        //            if (vmMod.strSelectCong != null)
        //                oTempCT.AttachedToChurchBodyList = vmMod.strSelectCong;

        //        if (string.IsNullOrEmpty(oTempCT.AttachedToChurchBodyList))
        //            return Json(new { taskSuccess = false, userMess = "At least one oversight congregation for the transfer required." });

        //        if (oTempCT.TransferSubType.Contains("R"))
        //        {
        //            if (string.IsNullOrEmpty(oTempCT.DesigRolesList))
        //                return Json(new { taskSuccess = false, userMess = "The Minister on-transfer not designated with any role." });

        //            string[] _arr = oTempCT.DesigRolesList.Split(',');
        //            if (_arr.Length > 0)
        //            {
        //                foreach (var _arrId in _arr)
        //                {
        //                    string[] arr = _arrId.Split('|');  //ChurchBody | ChurchSector | LeaderRole
        //                    if (arr.Length > 2)
        //                    {
        //                        var oChurchBodyId = int.Parse(arr[0]);
        //                        // var oChurchSectorId = int.Parse(arr[1]);
        //                        // var oLeaderRoleId = int.Parse(arr[2]);

        //                        var countCheck = 0;
        //                        string[] arrTo = oTempCT.AttachedToChurchBodyList.Split(',');
        //                        foreach (var arrId in arrTo)
        //                        {
        //                            if (int.Parse(arrId) == oChurchBodyId) countCheck++;
        //                        }

        //                        if (countCheck == 0) // || countCheck != arrTo.Length)
        //                            return Json(new { taskSuccess = false, userMess = "Church sectors in the designated roles must be part of the oversight congregations." });
        //                    }
        //                }
        //            }
        //        }

        //        if (taskIndx == 1)//|| taskIndx == 2 )//|| taskIndx == 5
        //            _oMTChanges = oTempCT;
        //        else
        //        { oCLGTransf = oTempCT; _oCLGTransf = oTempCT; }

        //        //oCLGTransf.AttachedToChurchBodyId = oCLGTransf.ToChurchBodyId;
        //        //oCLGTransf.AttachedToChurchBody = oCLGTransf.ToChurchBody;
        //        //_oCLGTransf.AttachedToChurchBodyId = _oCLGTransf.ToChurchBodyId;
        //        //_oCLGTransf.AttachedToChurchBody = _oCLGTransf.ToChurchBody;
        //        //_oMTChanges.AttachedToChurchBodyId = _oMTChanges.ToChurchBodyId;
        //        //_oMTChanges.AttachedToChurchBody = _oMTChanges.ToChurchBody;

        //        if (taskIndx == 1 || taskIndx == 2 || taskIndx == 5)  //SAVE, SEND, RESUBMIT
        //        {
        //            ModelState.Remove("oAppGlobalOwner.OwnerName");
        //            ModelState.Remove("oChurchTransfer.ChurchMemberId");
        //            ModelState.Remove("oChurchTransfer.FromChurchBodyId");
        //            ModelState.Remove("oChurchTransfer.ToChurchBodyId");
        //            ModelState.Remove("oChurchTransfer.AttachedToChurchBodyId");
        //            ModelState.Remove("ToChurchBodyId_Categ1");
        //            ModelState.Remove("ToChurchBodyId_Categ2");
        //            ModelState.Remove("ToChurchBodyId_Categ3");
        //            ModelState.Remove("FromChurchBodyId_Categ1");
        //            ModelState.Remove("FromChurchBodyId_Categ2");
        //            ModelState.Remove("FromChurchBodyId_Categ3");
        //            ModelState.Remove("oChurchBody.Name");
        //            ModelState.Remove("oChurchBody.ChurchType");
        //            ModelState.Remove("oChurchBody.AssociationType");
        //            ModelState.Remove("oChurchTransfer.ToLeaderRoleId");
        //            ModelState.Remove("oChurchTransfer.ToRoleSectorId");
        //            ModelState.Remove("oChurchTransfer.ToRoleSector.ChurchBodyId");

        //            //finally check error state...
        //            if (!ModelState.IsValid)
        //                return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." });

        //            if (oCLGTransf.Id == 0)
        //            {
        //                if (!(taskIndx == 1 || taskIndx == 2))  //save... send  
        //                {
        //                    return Json(new { taskSuccess = false, userMess = "Current request allows only 'Update, Send request' operation to be performed." });
        //                }

        //                if (_oMTChanges.ChurchMemberId == null || _oMTChanges.AttachedToChurchBodyList.Length == 0)//_oMTChanges.ToChurchBodyId == null)
        //                {
        //                    return Json(new { taskSuccess = false, userMess = "The Minister to transfer or target congregation not provided." });
        //                }

        //                if (taskIndx == 2) //... send 
        //                {
        //                    //check for pending.. unClosed requests OR successful closed requests
        //                    var currTransf = (from t_cm in _context.ChurchTransfer
        //                                     .Where(x => x.FromChurchBodyId == oCLGTransf.FromChurchBodyId && x.ChurchMemberId == _oMTChanges.ChurchMemberId && x.TransferType == "CT" &&
        //                                     x.TransferSubType == _oMTChanges.TransferSubType &&
        //                                     (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.Status != "U"))
        //                                      select t_cm).ToList();
        //                    if (currTransf.Count > 0)
        //                    {
        //                        return Json(new { taskSuccess = false, userMess = "Clergy transfer (" + GetTransferSubTypeDesc(_oMTChanges.TransferSubType) + ") already initiated or done for specified Minister. Hint: Try archiving and attempt again." });
        //                    }
        //                }

        //                var oCT = new ChurchTransfer
        //                { //create user and init...

        //                    RequestorMemberId = oCLGTransf.RequestorMemberId,
        //                    RequestorRoleId = oCLGTransf.RequestorRoleId,
        //                    RequestorChurchBodyId = oCLGTransf.RequestorChurchBodyId,
        //                    // FromChurchBodyId = oCLGTransf.FromChurchBodyId,
        //                    CurrentScope = oCLGTransf.CurrentScope,
        //                    AckStatus = oCLGTransf.ReqStatus,
        //                    ApprovalStatus = oCLGTransf.ApprovalStatus,
        //                    TransferType = oCLGTransf.TransferType,
        //                    //ToRequestDate  = date request is forwarded to destination congregation
        //                    // RequestDate = DateTime.Now,
        //                    Created = DateTime.Now,  // oCLGTransf.Created,
        //                    LastMod = DateTime.Now,
        //                    //
        //                    TransferSubType = _oMTChanges.TransferSubType,
        //                    ChurchMemberId = _oMTChanges.ChurchMemberId,
        //                    FromChurchBodyId = _oMTChanges.FromChurchBodyId,
        //                    ToChurchBodyId = _oMTChanges.ToChurchBodyId,
        //                    AttachedToChurchBodyId = _oMTChanges.AttachedToChurchBodyId,
        //                    AttachedToChurchBodyList = _oMTChanges.AttachedToChurchBodyList,
        //                    DesigRolesList = _oMTChanges.DesigRolesList,
        //                    ReasonId = _oMTChanges.ReasonId,
        //                    TransMessageId = _oMTChanges.TransMessageId,
        //                    Comments = _oMTChanges.Comments,
        //                    //
        //                    FromChurchPositionId = _oMTChanges.FromChurchPositionId <= 0 ? null : _oMTChanges.FromChurchPositionId,
        //                    FromMemberLeaderRoleId = _oMTChanges.FromMemberLeaderRoleId <= 0 ? null : _oMTChanges.FromMemberLeaderRoleId
        //                };
        //                //
        //                _context.Add(oCT);

        //                try
        //                {
        //                    await _context.SaveChangesAsync();
        //                    //
        //                    oCLGTransf = oCT; //update cache... 
        //                                      // oCLGTransf.ChurchMemberTransf = _oCLGTransf.ChurchMemberTransf;
        //                                      // oCLGTransf.ToChurchBody = _oCLGTransf.ToChurchBody;
        //                                      //oCLGTransf.RequestorChurchMember = _oCLGTransf.RequestorChurchMember;
        //                                      //oCLGTransf.RequestorChurchBody = _oCLGTransf.RequestorChurchBody;
        //                                      //oCLGTransf.FromChurchBody = _oCLGTransf.FromChurchBody;
        //                }
        //                catch (Exception ex)
        //                {
        //                    return Json(new { taskSuccess = false, userMess = "Attempt to save data failed. Please try again: " + ex.Message });
        //                }
        //            }
        //            else
        //            {
        //                if (oCLGTransf == null)
        //                {// ModelState.AddModelError(string.Empty, "Clergy transfer data not found! Please refresh and try again."); return Json(false);
        //                    return Json(new { taskSuccess = false, userMess = "Clergy transfer data not found! Please refresh and try again." });
        //                }

        //                if (oCLGTransf.ChurchMemberId == null || oCLGTransf.AttachedToChurchBodyList.Length == 0 || oCLGTransf.ChurchMemberTransf == null) // || oCLGTransf.ToChurchBody == null)
        //                { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                    return Json(new { taskSuccess = false, userMess = "The Minister to transfer or target congregation not provided." });
        //                }

        //                //set... for view in cache 
        //                if (_oMTChanges.TransferSubType != null) oCLGTransf.TransferSubType = _oMTChanges.TransferSubType;
        //                if (_oMTChanges.ChurchMemberId != null) oCLGTransf.ChurchMemberId = _oMTChanges.ChurchMemberId;
        //                if (_oMTChanges.FromChurchBodyId != null) oCLGTransf.FromChurchBodyId = _oMTChanges.FromChurchBodyId;
        //                if (_oMTChanges.ToChurchBodyId != null) oCLGTransf.ToChurchBodyId = _oMTChanges.ToChurchBodyId;
        //                if (_oMTChanges.AttachedToChurchBodyId != null) oCLGTransf.AttachedToChurchBodyId = _oMTChanges.AttachedToChurchBodyId;
        //                if (_oMTChanges.AttachedToChurchBodyList != null) oCLGTransf.AttachedToChurchBodyList = _oMTChanges.AttachedToChurchBodyList;
        //                if (_oMTChanges.DesigRolesList != null) oCLGTransf.DesigRolesList = _oMTChanges.DesigRolesList;
        //                if (_oMTChanges.ReasonId != null) oCLGTransf.ReasonId = _oMTChanges.ReasonId;
        //                if (_oMTChanges.TransMessageId != null) oCLGTransf.TransMessageId = _oMTChanges.TransMessageId;
        //                if (_oMTChanges.Comments != null) oCLGTransf.Comments = _oMTChanges.Comments;
        //                if (_oMTChanges.FromChurchPositionId != null) oCLGTransf.FromChurchPositionId = _oMTChanges.FromChurchPositionId <= 0 ? null : _oMTChanges.FromChurchPositionId;
        //                if (_oMTChanges.FromMemberLeaderRoleId != null) oCLGTransf.FromMemberLeaderRoleId = _oMTChanges.FromMemberLeaderRoleId <= 0 ? null : _oMTChanges.FromMemberLeaderRoleId;

        //                // oCLGTransf.RequestDate =  DateTime.Now;

        //                if (taskIndx == 2 || taskIndx == 5) //... send 
        //                {
        //                    //check for pending.. unClosed requests OR successful closed requests 
        //                    var currTransf = (from t_cm in _context.ChurchTransfer
        //                                      .Where(x => x.FromChurchBodyId == oCLGTransf.FromChurchBodyId && x.ChurchMemberId == oCLGTransf.ChurchMemberId && x.TransferType == "CT" &&
        //                                      x.TransferSubType == oCLGTransf.TransferSubType &&
        //                                      (x.ReqStatus != "N" && x.ReqStatus != "X" && x.ReqStatus != "R" && x.ReqStatus != "D" && x.ReqStatus != "Z" && x.Status != "U"))
        //                                      select t_cm).ToList();
        //                    if (currTransf.Count > 0)
        //                    {
        //                        return Json(new { taskSuccess = false, userMess = "Clergy transfer (" + GetTransferSubTypeDesc(oCLGTransf.TransferSubType) + ") already initiated or done for specified Minister. Hint: Try archiving and attempt again." });
        //                    }
        //                }



        //                //
        //                //already loaded at GET()
        //                if (taskIndx == 5)
        //                {
        //                    var oMT = new ChurchTransfer
        //                    { //create user and init...
        //                        RequestorMemberId = oCLGTransf.RequestorMemberId,
        //                        RequestorRoleId = oCLGTransf.RequestorRoleId,
        //                        RequestorChurchBodyId = oCLGTransf.RequestorChurchBodyId,
        //                        CurrentScope = oCLGTransf.CurrentScope,
        //                        AckStatus = oCLGTransf.ReqStatus,
        //                        ApprovalStatus = oCLGTransf.ApprovalStatus,
        //                        TransferType = oCLGTransf.TransferType,
        //                        //ActualRequestDate  = date request is forwarded to destination congregation
        //                        // RequestDate = oCLGTransf.RequestDate,
        //                        Created = DateTime.Now,  // oCLGTransf.Created,
        //                        LastMod = DateTime.Now,
        //                        //
        //                        TransferSubType = oCLGTransf.TransferSubType,
        //                        ChurchMemberId = oCLGTransf.ChurchMemberId,
        //                        FromChurchBodyId = oCLGTransf.FromChurchBodyId,
        //                        ToChurchBodyId = oCLGTransf.ToChurchBodyId,
        //                        AttachedToChurchBodyId = oCLGTransf.AttachedToChurchBodyId,
        //                        AttachedToChurchBodyList = oCLGTransf.AttachedToChurchBodyList,
        //                        DesigRolesList = oCLGTransf.DesigRolesList,
        //                        ReasonId = oCLGTransf.ReasonId,
        //                        TransMessageId = oCLGTransf.TransMessageId,
        //                        Comments = oCLGTransf.Comments,
        //                        //
        //                        FromChurchPositionId = oCLGTransf.FromChurchPositionId,
        //                        FromMemberLeaderRoleId = oCLGTransf.FromMemberLeaderRoleId
        //                    };
        //                    //
        //                    _context.Add(oMT);

        //                    try
        //                    {
        //                        await _context.SaveChangesAsync();
        //                        //
        //                        oCLGTransf = oMT; //update cache... 
        //                                          // oCLGTransf.ChurchMemberTransf = _oCLGTransf.ChurchMemberTransf;
        //                                          // oCLGTransf.ToChurchBody = _oCLGTransf.ToChurchBody;
        //                                          //oCLGTransf.RequestorChurchMember = _oCLGTransf.RequestorChurchMember;
        //                                          //oCLGTransf.RequestorChurchBody = _oCLGTransf.RequestorChurchBody;
        //                                          //oCLGTransf.FromChurchBody = _oCLGTransf.FromChurchBody;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return Json(new { taskSuccess = false, userMess = "Attempt to save data failed. Please try again: " + ex.Message });
        //                    }
        //                }

        //                //oCLGTransf.ChurchMemberTransf = _oCLGTransf.ChurchMemberTransf;
        //                //oCLGTransf.ToChurchBody = _oCLGTransf.ToChurchBody;
        //                //oCLGTransf.RequestorChurchMember = _oCLGTransf.RequestorChurchMember;
        //                //oCLGTransf.RequestorChurchBody = _oCLGTransf.RequestorChurchBody;
        //                //oCLGTransf.FromChurchBody = _oCLGTransf.FromChurchBody;

        //                //if (taskIndx != 1 )
        //                //{
        //                //    if (oCLGTransf.ChurchMemberId == null || oCLGTransf.ToChurchBodyId == null || oCLGTransf.ChurchMemberTransf == null || oCLGTransf.ToChurchBody==null)
        //                //    { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                //        return Json(new { taskSuccess = false, userMess = "Member to transfer or target congregation not provided." });
        //                //    }

        //                //    ////get member
        //                //    //oCLGTransf.ChurchMemberTransf = _context.ChurchMember.Find(oCLGTransf.ChurchMemberId);

        //                //    ////get member
        //                //    //oCLGTransf.ToChurchBody = _context.ChurchBody.Find(oCLGTransf.ToChurchBodyId);
        //                //}
        //            }

        //            ////get member
        //            //if (oCLGTransf.ChurchMemberTransf == null)
        //            //    oCLGTransf.ChurchMemberTransf = _context.ChurchMember.Find(oCLGTransf.ChurchMemberId);

        //            ////get cong
        //            //if (oCLGTransf.AttachedToChurchBodyId != null)
        //            //    if (oCLGTransf.AttachedToChurchBody == null )
        //            //        oCLGTransf.AttachedToChurchBody = _context.ChurchBody.Find(oCLGTransf.AttachedToChurchBodyId);

        //            //if (oCLGTransf.ToChurchBodyId != null)
        //            //    if (oCLGTransf.ToChurchBody == null)
        //            //        oCLGTransf.ToChurchBody = _context.ChurchBody.Find(oCLGTransf.ToChurchBodyId);
        //        }

        //        else
        //        {   // get the Clergy transfer data... no updates except the approval/ action parts 
        //            if (oCLGTransf == null)
        //            {// ModelState.AddModelError(string.Empty, "Clergy transfer data not found! Please refresh and try again."); return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Clergy transfer data not found! Please refresh and try again." });
        //            }

        //            if (oCLGTransf.ChurchMemberId == null || oCLGTransf.AttachedToChurchBodyList.Length == 0) // || oCLGTransf.ChurchMemberTransf == null) // || oCLGTransf.ToChurchBody == null)
        //            { //ModelState.AddModelError(string.Empty, "Member to transfer or target congregation not provided."); return Json(false); 
        //                return Json(new { taskSuccess = false, userMess = "The Minister to transfer or target congregation not provided." });
        //            }


        //            //if (oCLGTransf.AttachedToChurchBodyId != null)
        //            //    if (oCLGTransf.AttachedToChurchBody == null)
        //            //        oCLGTransf.AttachedToChurchBody = _context.ChurchBody.Find(oCLGTransf.AttachedToChurchBodyId);

        //            //if (oCLGTransf.ToChurchBodyId != null)
        //            //    if (oCLGTransf.ToChurchBody == null)
        //            //        oCLGTransf.ToChurchBody = _context.ChurchBody.Find(oCLGTransf.ToChurchBodyId);


        //            if (taskIndx != 5 && taskIndx >= 3 && taskIndx <= 9) //
        //            {
        //                vmMod.lsCurrApprActionSteps = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                          .Where(c => c.ApprovalActionId == vmMod.currApprovalActionId && //c.CurrentScope == oCLGTransf.CurrentScope &&
        //                          c.ChurchBody.AppGlobalOwnerId == _oCLGTransf.RequestorChurchBody.AppGlobalOwnerId && c.ChurchBody.AppGlobalOwnerId == _oCLGTransf.RequestorChurchBody.AppGlobalOwnerId &&   //(c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId || c.ChurchBodyId == oCLGTransf.ToChurchBodyId) && // == 
        //                          c.ApprovalAction.ProcessCode == "TRF" && c.ApprovalAction.ProcessSubCode == "CT" && c.ApprovalAction.Status == "A" && c.Status == "A")
        //                          .ToList();
        //            }
        //            else if (taskIndx == 10) //force complete
        //            {
        //                vmMod.lsCurrApprActionSteps = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                     .Where(c => c.ApprovalActionId == vmMod.currApprovalActionId && //c.CurrentScope == oCLGTransf.CurrentScope &&
        //                           c.ChurchBody.AppGlobalOwnerId == _oCLGTransf.RequestorChurchBody.AppGlobalOwnerId && c.ChurchBody.AppGlobalOwnerId == _oCLGTransf.RequestorChurchBody.AppGlobalOwnerId &&   //(c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId || c.ChurchBodyId == oCLGTransf.ToChurchBodyId) && // == 
        //                           c.ApprovalAction.ProcessCode == "TRF" && c.ApprovalAction.ProcessSubCode == "CT" && c.ApprovalAction.Status == "A" && c.Status == "A" && c.ActionStepStatus != "A")
        //                     .ToList();
        //            }
        //        }

        //        //if (oCLGTransf == null)
        //        //    { //ModelState.AddModelError(string.Empty, "Clergy transfer data not found! Please refresh and try again."); return Json(false);
        //        //    return Json(new { taskSuccess = false, userMess = "Clergy transfer data not found! Please refresh and try again." });
        //        //}

        //        if (oCLGTransf.Id > 0)
        //        {
        //            if (taskIndx >= 1 && taskIndx <= 5)  // SAVE-SEND-RECALL-TERMINATE-RESUBMIT
        //            {
        //                if (!(oCLGTransf.CurrentScope == "I" && (taskIndx >= 1 && taskIndx <= 5))) // || taskIndx == 2 || taskIndx == 5))) / update a 'Draft' or Send 'Draft' Req...  resubmit //recalled, terminated, declined// here!
        //                { //ModelState.AddModelError(string.Empty, "Current request allows only 'Save, Send, Recall, TERMINATE, RESUBMIT request' operations to be performed.");   return Json(false); 
        //                    return Json(new { taskSuccess = false, userMess = "Current request allows only 'Save, Send, Recall, TERMINATE, RESUBMIT request' operations to be performed" });
        //                }

        //                //Update... only Draft
        //                if (taskIndx == 1)
        //                {
        //                    if (oCLGTransf.ReqStatus != "N" && oCLGTransf.ReqStatus != "R")  //draft or recalled
        //                    {  // ModelState.AddModelError(string.Empty, "Request already sent. Update cannot be done. Hint: Try 'Recall request' to make necessary changes.");  return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Request already sent. Update cannot be done. Hint: Try 'Recall request' to make necessary changes." });
        //                    } // ViewBag.UserMsg = "Updated member family relations successfully.";
        //                }
        //                else if (taskIndx == 2)
        //                {
        //                    if (oCLGTransf.ReqStatus != "N")  //send saved 'draft'
        //                    { // ModelState.AddModelError(string.Empty, "Request already sent. Hint: Try 'Recall request' to make necessary changes or 'Resubmit' if it's recalled, terminated or declined.");   return Json(false);
        //                        return Json(new { taskSuccess = false, userMess = "Request already sent. Hint: Try 'Recall request' to make necessary changes or 'Resubmit' if it's recalled, terminated or declined." });
        //                    } // ViewBag.UserMsg = "Transfer request sent successfully.";
        //                }
        //                else if (taskIndx == 3)
        //                {
        //                    if (oCLGTransf.ReqStatus != "P" && oCLGTransf.ReqStatus != "I")  //RECALL
        //                    {// ModelState.AddModelError(string.Empty, "Request must be 'Pending' or 'In Progress' to be recalled. Hint: Try 'Terminate and Resubmit request'.");  return Json(false);
        //                        return Json(new { taskSuccess = false, userMess = "Request must be 'Pending' or 'In Progress' to be recalled. Hint: Try 'Terminate and Resubmit request'." });
        //                    } // ViewBag.UserMsg = "Transfer request recalled successfully.";
        //                }
        //                else if (taskIndx == 4)
        //                {
        //                    if (oCLGTransf.ReqStatus != "P" && oCLGTransf.ReqStatus != "I")  //TERMINATE
        //                    {
        //                        //ModelState.AddModelError(string.Empty, "Request cannot be terminated. Request must be Pending or In Progress.  Hint: Try 'Resubmit request' if request is 'Closed'.");
        //                        return Json(new { taskSuccess = false, userMess = "Request cannot be terminated. Request must be Pending or In Progress. Hint: Try 'Resubmit request' if request is 'Closed'." });
        //                    }
        //                }
        //                else if (taskIndx == 5)  //RESUBMIT... recalled, declined ...............|| oCLGTransf.ReqStatus == "X"
        //                { //ProcessCode = "TRF_OT" 
        //                    if (!((oCLGTransf.ReqStatus == "R" || oCLGTransf.ReqStatus == "D"))) //|| (oCLGTransf.ApprovalStatus == "R" || oCLGTransf.ApprovalStatus == "X" || oCLGTransf.ApprovalStatus == "D"))) // resubmit // Recalled, Terminated, Declined //
        //                    { // ModelState.AddModelError(string.Empty, "Resubmission is possible ONLY for transfer requests that have been Recalled, Terminated or Declined.");   return Json(false); 
        //                        return Json(new { taskSuccess = false, userMess = "Resubmission is possible ONLY for transfer requests recalled or declined." });
        //                    } // ViewBag.UserMsg = "Transfer Request resubmitted successfully.";
        //                }
        //            }

        //            else // ACKNOWLEDGE-APROVE-DECLINE-SUSPEND-FORCE COMPLETE
        //            {
        //                if (taskIndx == 6)  //ACK... 1. ToCongregation (after I-approval) 2. FromCongregation (after I-approval)... remember here... no E-approval
        //                {
        //                    if (!((oCLGTransf.ReqStatus == "A" || oCLGTransf.ReqStatus == "C") && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F")))   //(oCLGTransf.ReqStatus != "P")  // && oCLGTransf.ReqStatus != "R")  //draft or recalled
        //                    {  //ModelState.AddModelError(string.Empty, "Request must be 'Pending' to be acknowledged.");  return Json(false);
        //                        return Json(new { taskSuccess = false, userMess = "Request must be 'Approved' to be acknowledged." });
        //                    }
        //                }
        //                else if (taskIndx >= 7 && taskIndx <= 10)
        //                {
        //                    if (!(((oCLGTransf.ReqStatus == "P" || oCLGTransf.ReqStatus == "I") && (oCLGTransf.ApprovalStatus == "P" || oCLGTransf.ApprovalStatus == "I" || oCLGTransf.ApprovalStatus == "H")) ||
        //                            (oCLGTransf.ReqStatus == "A" || oCLGTransf.ReqStatus == "D")))
        //                    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Request must first be acknowledged or be 'in progress' or still open for approver to take action. Hint: try to Acknowledge first." });
        //                    }
        //                }
        //                else if (taskIndx == 11)
        //                {
        //                    if (!(oCLGTransf.ReqStatus == "C" && oCLGTransf.Status == "U" && oCLGTransf.ApprovalStatus == "D")) //not Archived (Z)
        //                    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Only unarchived, declined and closed requests can be re-opened." });
        //                    }
        //                }
        //                else if (taskIndx == 12)
        //                {
        //                    if (!((oCLGTransf.ReqStatus == "C" || oCLGTransf.ReqStatus == "X" || oCLGTransf.ReqStatus == "R") && (oCLGTransf.Status == "U" || oCLGTransf.Status == "Y"))) // && oCLGTransf.ApprovalStatus == "D")) //not Archived (Z)
        //                    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                        return Json(new { taskSuccess = false, userMess = "Only closed, aborted or recalled requests can be archived." });
        //                    }
        //                }

        //                //else if (taskIndx == 13)
        //                //{
        //                //    if (!((oCLGTransf.AttachedToChurchBodyId == null && oCLGTransf.Status == "T" && oCLGTransf.ReqStatus == "A" && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F")))) // && oCLGTransf.ApprovalStatus == "D")) //not Archived (Z)
        //                //    {  // ModelState.AddModelError(string.Empty, "Request must be 'In Progress' for approver to take action [approve, suspend, decline or force-complete]. Hint: try to Acknowledge first.");   return Json(false);  
        //                //        return Json(new { taskSuccess = false, userMess = "On-ward tranfers can only be done for Ministers in-transit (yet to be attached to a congregation; not affiliation!)" });
        //                //    }

        //                //    if (oCLGTransf.AttachedToChurchBodyList.Contains(oCLGTransf.AttachedToChurchBodyId.ToString()) == false)
        //                //    {
        //                //        return Json(new { taskSuccess = false, userMess = "Target congregation is not specified" });
        //                //    }
        //                //}
        //            }
        //        }

        //        //Create the approval process...
        //        //this approval is for OUTGOING... but internal. EXTERNAL will be triggered by the Acknowledgment separately... NEW Send Req or DRAFTed and now Send Req... jux make sure user does not send twice

        //        if (taskIndx == 2 || taskIndx == 5)
        //        {
        //            List<ApprovalAction> oAppActionList = new List<ApprovalAction>();
        //            List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
        //            var oAppProStepList = _context.ApprovalProcessStep.Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
        //                .Where(c => c.ChurchBody.AppGlobalOwnerId == _oCLGTransf.RequestorChurchBody.AppGlobalOwnerId &&  // c.ApprovalProcess.ChurchBody.AppGlobalOwnerId == oCLGTransf.RequestorChurchBody.AppGlobalOwnerId &&
        //                            c.ApprovalProcess.ChurchLevelId == _oCLGTransf.RequestorChurchBody.ChurchLevelId &&
        //                            c.ApprovalProcess.ProcessCode == "TRF_OT" && c.ApprovalProcess.ProcessSubCode == "CT" && c.ApprovalProcess.ProcessStatus == "A" && c.StepStatus == "A")
        //                .ToList();

        //            //create approval action... at least  one approval level
        //            if (oAppProStepList.Count > 0)
        //            {
        //                oAppActionList.Add(
        //                    new ApprovalAction
        //                    {
        //                        //Id = 0,
        //                        ChurchBodyId = oCLGTransf.RequestorChurchBodyId,
        //                        // ChurchBody = oCLGTransf.RequestorChurchBody,
        //                        ApprovalActionDesc = "Outgoing Clergy transfer",
        //                        ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
        //                        ProcessCode = "TRF",
        //                        ProcessSubCode = "CT",
        //                        ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
        //                        ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
        //                        CallerRefId = oCLGTransf.Id,  //reference to the Transfer details
        //                        CurrentScope = "I",
        //                        Status = "A",
        //                        //ActionDate = null,
        //                        //Comments = "",
        //                        ActionRequestDate = DateTime.Now,
        //                        Created = DateTime.Now,
        //                        LastMod = DateTime.Now
        //                    });

        //                if (oAppActionList.Count > 0)
        //                    _context.Add(oAppActionList.FirstOrDefault());

        //                //create approval action steps
        //                var stepIndexLowest = oAppProStepList[0].StepIndex;
        //                foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
        //                {
        //                    var oCurrApprover = _context.MemberLeaderRole.Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId && c.LeaderRoleId == oAppProStep.ApproverLeaderRoleId && c.IsCurrServing == true && c.IsCoreRole == true).FirstOrDefault();
        //                    if (oCurrApprover == null)
        //                    {
        //                        // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");
        //                        return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
        //                    }

        //                    stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
        //                    oAppActionStepList.Add(
        //                        new ApprovalActionStep
        //                        {
        //                            //Id = 0,
        //                            ChurchBodyId = oCLGTransf.RequestorChurchBodyId,
        //                            //ChurchBody = oCLGTransf.RequestorChurchBody,
        //                            MemberLeaderRoleId = oCurrApprover != null ? oCurrApprover.Id : (int?)null,
        //                            Approver = oCurrApprover,
        //                            ActionStepDesc = oAppProStep.StepDesc,
        //                            ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
        //                            ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
        //                            ProcessStepRefId = oAppProStep.Id,
        //                            CurrentScope = "I",
        //                            StepRequestDate = DateTime.Now,
        //                            //Comments="",
        //                            //CurrentStep= true,
        //                            //ActionByLeaderRoleId=null,  // actual approver
        //                            //ActionDate = DateTime.Now,
        //                            Status = "A", //Active
        //                            Created = DateTime.Now,
        //                            LastMod = DateTime.Now,
        //                            //
        //                            ApprovalActionId = oAppActionList.FirstOrDefault().Id,
        //                            //ApprovalAction = oAppActionList.FirstOrDefault()
        //                        });
        //                }

        //                foreach (ApprovalActionStep oAS in oAppActionStepList)
        //                {
        //                    oAS.CurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled   // if (oAS.CurrentStep) oCurrApprovers.Add(oAS);
        //                    _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }                                                  
        //                }

        //                oCLGTransf.ReqStatus = "P"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //                oCLGTransf.RequestDate = DateTime.Now;
        //                //oCLGTransf.ActualRequestDate = null;  // after approval or acknowledgment from the FROM congregation
        //                oCLGTransf.RequireApproval = oAppActionList.Count > 0; // _oCLGTransf_VM.RequireApproval,
        //                oCLGTransf.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oCLGTransf_VM.ApprovalStatus,
        //                                                 //
        //                oCLGTransf.IApprovalActionId = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oCLGTransf_VM.IApprovalActionId,
        //                                                                                                                             // oCLGTransf.IApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oCLGTransf_VM.IApprovalAction,
        //            }
        //        }
        //        if (taskIndx == 3 || taskIndx == 4)   //Recall, Terminate
        //        {
        //            string strApprovalStatus = null;
        //            var oActionStepList = vmMod.lsCurrApprActionSteps;

        //            //create approval action
        //            if (oActionStepList.Count > 0)
        //            {
        //                foreach (ApprovalActionStep oAAStep in oActionStepList)
        //                {
        //                    oAAStep.ActionStepStatus = taskIndx == 3 ? "R" : "X";
        //                    oAAStep.Comments = vmMod.strApproverComment;  //could also be reason from the user/applicant
        //                    oAAStep.ActionDate = DateTime.Now;
        //                    oAAStep.LastMod = DateTime.Now;

        //                    //add appproval notifiers
        //                    oCurrApprovers.Add(oAAStep);
        //                    _context.Update(oAAStep);
        //                }

        //                var oAA = oActionStepList[0].ApprovalAction;
        //                strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, taskIndx == 3 ? "R" : "X");   // this.GetApprovalActionStatus(oActionStepList, oCLGTransf.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //                oAA.ActionStatus = strApprovalStatus; // this.GetApprovalActionStatus(oActionStepList);   //this.GetApprovalActionStatus(oActionStepList, oCLGTransf.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //                oAA.Comments = vmMod.strApproverComment;   //could also be reason from the user/applicant
        //                oAA.LastActionDate = DateTime.Now;
        //                oAA.LastMod = DateTime.Now;

        //                _context.Update(oAA);
        //            }

        //            oCLGTransf.ReqStatus = taskIndx == 3 ? "R" : "X"; // GetApprovalActionStatus(oActionStepList, oCLGTransf.CurrentScope == "I" ? 1 : 2, taskIndx == 3 ? "R" : "X"); // taskIndx == 3 ? "R" : "X"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //            oCLGTransf.RequestDate = DateTime.Now;
        //            oCLGTransf.ApprovalStatus = strApprovalStatus; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oCLGTransf_VM.ApprovalStatus,
        //            oCLGTransf.Status = "U"; // Unsuccessful
        //            //if (strApprovalStatus == null)
        //            //{
        //            //    oCLGTransf.IApprovalActionId = null;
        //            //    oCLGTransf.IApprovalAction = null;
        //            //}
        //        }
        //        if (taskIndx == 6)
        //        {//oCLGTransf.AttachedToChurchBodyId == null && 
        //            if (oCLGTransf.Status == "Y" && ((oCLGTransf.ReqStatus == "A" || oCLGTransf.ReqStatus == "C") && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F")))
        //            {  //first... @Pending... //@In Progress... acknowledge --> Pending of ToChurchBody approvers
        //               //oCLGTransf.ReqStatus = "I"; // Leave all approval action/step in Pending until approver works on ...

        //                //if (oCLGTransf.CurrentScope == "I")
        //                //{

        //                // if (oCLGTransf.FromChurchBodyId !=null)
        //                if (oCLGTransf.FromChurchBodyId == (int?)vmMod.oChurchBody.Id)  //ack does not change anything...
        //                    oCLGTransf.Fr_ReceivedDate = DateTime.Now;

        //                if (oCLGTransf.ToChurchBodyId != null)
        //                    if (oCLGTransf.ToChurchBodyId == (int?)vmMod.oChurchBody.Id)
        //                        oCLGTransf.ToReceivedDate = DateTime.Now;

        //                ////notify the approvers now... 
        //                //var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                //    .Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId &&
        //                //                //(c.CurrentScope == "I" || (c.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P")) && 
        //                //                c.ApprovalActionId == oCLGTransf.IApprovalActionId && c.CurrentStep == true).ToList();
        //                //oCurrApprovers.AddRange(oAASList);

        //                //}

        //                //else if (oCLGTransf.CurrentScope == "E")
        //                //{
        //                //    oCLGTransf.ToReceivedDate = DateTime.Now; // oCLGTransf.Status = "I"; //Approval Pending

        //                //    //notify the approvers now... 
        //                //    var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                //        .Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId &&
        //                //                    //(c.CurrentScope == "E" && oCLGTransf.ApprovalStatus != "P" && 
        //                //                    c.ApprovalActionId == oCLGTransf.EApprovalActionId && c.CurrentStep == true).ToList();
        //                //    oCurrApprovers.AddRange(oAASList);
        //                //}
        //            }

        //            //else if (oCLGTransf.AttachedToChurchBodyId != null && (oCLGTransf.ReqStatus == "A" || oCLGTransf.ReqStatus == "C") && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))
        //            //{
        //            //    //perform the transfer ...and then Close the request after 'success'.
        //            //    oCLGTransf.TransferDate = DateTime.Now;    // same date to be used as the Enrollment date /Departure date for member

        //            //    if (this.PerformMemberTransfer(_context, oCLGTransf, vmMod.oChurchBody.Id) == false)
        //            //    {  //reverse action .....    ModelState.AddModelError(string.Empty, "Clergy transfer unsuccessful.");
        //            //        return Json(new { taskSuccess = false, userMess = "Clergy transfer unsuccessful. Failed operation reversing..." });
        //            //    }

        //            //    //notify all appprovers
        //            //    var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //            //        .Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId &&
        //            //                        (c.ApprovalActionId == oCLGTransf.IApprovalActionId) // || c.ApprovalActionId == oCLGTransf.EApprovalActionId)
        //            //                        ).ToList();
        //            //    oCurrApprovers.AddRange(oAASList);

        //            //    ////update... you can reverse in case...
        //            //    //oCLGTransf.ReqStatus = "C";     //closed... @ last approval                       
        //            //    //oCLGTransf.Status = "Y"; //Yes... Transfer done
        //            //}
        //        }

        //        if (taskIndx >= 7 && taskIndx <= 9)  // Approve-Decline-Suspend-Force Complete    && vm.lsCurrApprActionSteps.Count > 0)
        //        {
        //            string strApprovalStatus = null;
        //            if (vmMod.oCurrApprovalActionStep == null || vmMod.oCurrApprovalAction == null || vmMod.lsCurrApprActionSteps.Count == 0)
        //            {  //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again."); return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
        //            }

        //            //update approval action step

        //            //var oAAStepList = _context.ApprovalActionStep.Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //            //    .Where(c => c.Id == vmMod.currApprovalActionId).ToList();

        //            //var oAAStep = _context.ApprovalActionStep.Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t=>t.ContactInfo)
        //            //    .Where(c => c.Id == vmMod.currApprovalActionStepId).FirstOrDefault() ; // .Find(vmMod.currApprovalActionStepId);  //.oCurrApprovalActionStep.Id);

        //            var oActionStepList = vmMod.lsCurrApprActionSteps;
        //            var oAAStep = oActionStepList.Where(c => c.Id == vmMod.currApprovalActionStepId).FirstOrDefault();
        //            if (oAAStep == null)
        //            { //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again."); return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
        //            }

        //            //add appproval notifiers
        //            //oCurrApprovers.Add(oAAStep);
        //            //(oCLGTransf.CurrentScope == "E" && c.ChurchBodyId == oCLGTransf.ToChurchBodyId) || 
        //            var oApprover = _context.MemberLeaderRole.Where(c => (c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId &&
        //                            c.ChurchMemberId == vmMod.oCurrLoggedMemberId && c.IsCurrServing == true && c.IsCoreRole == true)).FirstOrDefault();
        //            if (oApprover == null)
        //            { //ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again."); return Json(false); 
        //                return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
        //            }

        //            oAAStep.ActionStepStatus = taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : taskIndx == 9 ? "H" : oAAStep.ActionStepStatus;
        //            if (taskIndx == 8 || taskIndx == 9)
        //            {
        //                oAAStep.Comments = apprComment;   //could also be reason from the user/applicant
        //            }
        //            oAAStep.ActionDate = DateTime.Now;
        //            oAAStep.ActionByMemberLeaderRoleId = oApprover.Id;
        //            oAAStep.ActionBy = oApprover;
        //            oAAStep.LastMod = DateTime.Now;

        //            //if step Approved.. get next step and notify approvers...
        //            if (oAAStep.ActionStepStatus == "A")
        //            {
        //                var oNextAAStep = oActionStepList.Where(c => c.ApprovalStepIndex == oAAStep.ApprovalStepIndex + 1).ToList();
        //                if (oNextAAStep.Count > 0)
        //                {
        //                    oAAStep.CurrentStep = false;
        //                    foreach (var oNextStep in oNextAAStep)
        //                    {
        //                        oNextStep.CurrentStep = true;
        //                        oNextStep.LastMod = DateTime.Now;
        //                        //
        //                        oCurrApprovers.Add(oNextStep);
        //                        _context.Update(oNextStep);
        //                    }
        //                }
        //            }

        //            //
        //            vmMod.oCurrApprovalActionStep = oAAStep;
        //            for (int i = 0; i < oActionStepList.Count; i++)
        //            {
        //                if (oActionStepList[i].Id == oAAStep.Id) { oActionStepList[i] = oAAStep; break; }
        //            }

        //            _context.Update(oAAStep);


        //            //try
        //            //{  // _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //            //    _context.Update(oAAStep);
        //            //}
        //            //catch (Exception ex)
        //            //{ return Json(new { taskSuccess = false, userMess = "Error occured while processing request. Please refresh and try again." }); }


        //            //approval action       
        //            var oAA = _context.ApprovalAction.Find(vmMod.currApprovalActionId); //oCurrApprovalAction.Id);
        //            if (oAA == null)
        //            { //ModelState.AddModelError(string.Empty, "Approval details not found. Please refresh and try again.");  return Json(false);
        //                return Json(new { taskSuccess = false, userMess = "Approval details not found. Please refresh and try again." });
        //            }

        //            strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : "H"); // taskIndx == 9 ? "H");
        //            oAA.ActionStatus = strApprovalStatus; // this.GetApprovalActionStatus(oActionStepList);  //strApprovalStatus;
        //            oAA.Comments = vmMod.strApproverComment;   //could also be reason from the user/applicant
        //            oAA.LastActionDate = DateTime.Now;
        //            oAA.LastMod = DateTime.Now;

        //            try
        //            {  // _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //                _context.Update(oAA);
        //            }
        //            catch (Exception ex)
        //            { return Json(new { taskSuccess = false, userMess = "Error occured while processing request: " + ex.Message }); }

        //            //Clergy transfer
        //            // strApprovalStatus = this.GetApprovalActionStatus(oActionStepList);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //            oCLGTransf.ReqStatus = this.GetApprovalActionStatus(oActionStepList, 2, taskIndx == 7 ? "A" : taskIndx == 8 ? "D" : "H"); //, oCLGTransf.CurrentScope == "I" || taskIndx == 9 ? 1 : 2);// taskIndx == 3 ? "R" : "X"; // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //            //oCLGTransf.RequestDate = DateTime.Now;
        //            oCLGTransf.ApprovalStatus = strApprovalStatus;// this.GetApprovalActionStatus(oActionStepList);  // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oCLGTransf_VM.ApprovalStatus,

        //            if (oCLGTransf.ReqStatus != "A" && vmMod.lsCurrApprActionSteps.Count == 1) //1st time
        //            {
        //                oCLGTransf.ReceivedDate = DateTime.Now;
        //                // oCLGTransf.ReqStatus = "I";
        //                oCLGTransf.Status = "I"; //Approval Pending ... 
        //            }

        //            oCLGTransf.Status = (oCLGTransf.ApprovalStatus == "F" || oCLGTransf.ApprovalStatus == "A") ? "T" :
        //                                 oCLGTransf.ApprovalStatus == "D" ? "U" : oCLGTransf.ApprovalStatus == "H" ? "I" : oCLGTransf.Status; //await ack... In transit else Unsuccessful 

        //            if (oCLGTransf.Status == "U")
        //            {
        //                oCLGTransf.ReqStatus = "C";     //closed... can be re-Opened at current scope for reverse actions... Change "C" to "I" and await any more step... [approve, decline] for next possible status
        //                oCLGTransf.TransferDate = null;
        //            }

        //            //
        //            //if (strApprovalStatus == "F" || (strApprovalStatus == "A" || oCLGTransf.CurrentScope == "E")) oCLGTransf.Status = "T";
        //            //else if (strApprovalStatus == "D") oCLGTransf.Status = "U";
        //            //else if (strApprovalStatus == "H") oCLGTransf.Status = "I";




        //            //if Approved... and scope == internal and status==In Progress.... last approver acted
        //            if (taskIndx == 7 && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))  //(taskIndx == 7 && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))
        //            {  //  oCLGTransf.Status = "T";strApprovalStatus == "A" || strApprovalStatus == "F" ? "T" : strApprovalStatus == "D" ? "U" : oCLGTransf.Status; //await ack... In transit else Unsuccessful    

        //                if (oCLGTransf.ReqStatus == "A" && ((oCLGTransf.TransferSubType.Contains("M") && oCLGTransf.AttachedToChurchBodyId != null) ||
        //                        (oCLGTransf.TransferSubType.Contains("R") && !oCLGTransf.TransferSubType.Contains("M") && oCLGTransf.AttachedToChurchBodyId == null)))
        //                {
        //                    //perform the transfer ...and then Close the request after 'success'.
        //                    oCLGTransf.TransferDate = DateTime.Now;    // same date to be used as the Enrollment date /Departure date for member

        //                    if (this.PerformCLGMembershipTransfer(_context, oCLGTransf, vmMod.oChurchBody.Id) == false)
        //                    {  //reverse action .....    ModelState.AddModelError(string.Empty, "Clergy transfer unsuccessful.");   ...add Affiliation and pay attn to AttachedTo
        //                        return Json(new { taskSuccess = false, userMess = "Clergy transfer unsuccessful. Failed operation reversing..." });
        //                    }

        //                    oCLGTransf.ReqStatus = "C";     //closed... @ last approval... Transfer done                       
        //                    oCLGTransf.Status = "Y"; //Yes... Transfer done
        //                    if (oCLGTransf.TransferSubType.Contains("M"))   //included membership transfer
        //                        oCLGTransf.MembershipStatus = "M";  // Moved /transferred  || "T";  // in transit

        //                    //notify all appprovers
        //                    var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //                        .Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId &&
        //                                        (c.ApprovalActionId == oCLGTransf.IApprovalActionId) // || c.ApprovalActionId == oCLGTransf.EApprovalActionId)
        //                                        ).ToList();
        //                    oCurrApprovers.AddRange(oAASList);

        //                }
        //                //else if (oCLGTransf.AttachedToChurchBodyId == null || oCLGTransf.ReqStatus == "A")
        //                //{
        //                //    oCLGTransf.Status = "T";   //in transit... awaiting member to hit target congregation
        //                //    oCLGTransf.MembershipStatus = "T";  // in transit
        //                //}

        //                ////1st approval
        //                //else if (oCLGTransf.ReqStatus != "A" && vmMod.lsCurrApprActionSteps.Count==1)
        //                //{
        //                //    oCLGTransf.ReceivedDate = DateTime.Now;   
        //                //    oCLGTransf.ReqStatus = "I";                            
        //                //    oCLGTransf.Status = "I"; //Approval Pending ... 
        //                //}




        //                //if (oCLGTransf.CurrentScope == "I" && oCLGTransf.ReqStatus == "I") //&& (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))
        //                //{ 
        //                //    List<ApprovalAction> oAppActionList = new List<ApprovalAction>();
        //                //    List<ApprovalActionStep> oAppActionStepList = new List<ApprovalActionStep>();
        //                //    var oAppProStepList = _context.ApprovalProcessStep.Include(t => t.ChurchBody).Include(t => t.ApprovalProcess)
        //                //        .Where(c => c.ChurchBody.AppGlobalOwnerId == oCLGTransf.ToChurchBody.AppGlobalOwnerId && c.ApprovalProcess.ChurchBody.AppGlobalOwnerId == oCLGTransf.ToChurchBody.AppGlobalOwnerId &&
        //                //                    c.ApprovalProcess.ProcessCode == "TRF_IN" && c.ApprovalProcess.ProcessSubCode == "CT" && c.ApprovalProcess.ProcessStatus == "A" && c.StepStatus == "A")
        //                //        .ToList();

        //                //    //create approval action
        //                //    if (oAppProStepList.Count > 0)
        //                //    {
        //                //        oAppActionList.Add(
        //                //        new ApprovalAction
        //                //        {
        //                //            //Id = 0,
        //                //            ChurchBodyId = oCLGTransf.RequestorChurchBodyId,
        //                //            ChurchBody = oCLGTransf.RequestorChurchBody,
        //                //            ApprovalActionDesc = "Incoming Clergy transfer",
        //                //            ActionStatus = "P",  //Acknowledgement sets it into... ie. the 1st Step[i]... In Progress. leaves remaining Pending until successful Approval
        //                //            ProcessCode = "TRF",
        //                //            ProcessSubCode = "CT",
        //                //            ApprovalProcessId = oAppProStepList.FirstOrDefault().ApprovalProcessId,
        //                //            ApprovalProcess = oAppProStepList.FirstOrDefault().ApprovalProcess,
        //                //            CallerRefId = oCLGTransf.Id,  //reference to the Transfer details
        //                //            CurrentScope = "E",
        //                //            Status = "A",
        //                //            //ActionDate = null,
        //                //            //Comments = "",
        //                //            ActionRequestDate = DateTime.Now,
        //                //            Created = DateTime.Now,
        //                //            LastMod = DateTime.Now
        //                //        });

        //                //        if (oAppActionList.Count > 0)
        //                //            _context.Add(oAppActionList.FirstOrDefault());

        //                //        //create approval action steps
        //                //        var stepIndexLowest = 1;
        //                //        if (oAppProStepList.Count > 0) stepIndexLowest = oAppProStepList[0].StepIndex;
        //                //        foreach (ApprovalProcessStep oAppProStep in oAppProStepList)
        //                //        {
        //                //            var oICurrApprover = _context.MemberLeaderRole.Where(c => c.ChurchBodyId == oCLGTransf.ToChurchBodyId && c.LeaderRoleId == oAppProStep.ApproverLeaderRoleId &&
        //                //                                                                c.CurrServing == true && c.CoreRole == true).FirstOrDefault();
        //                //            if (oICurrApprover == null)
        //                //            { // ModelState.AddModelError(string.Empty, "Approver not available for configured approval flow step. Verify and try again.");  return Json(false);
        //                //                return Json(new { taskSuccess = false, userMess = "Approver not available for configured approval flow step. Verify and try again." });
        //                //            }

        //                //            stepIndexLowest = oAppProStep.StepIndex < stepIndexLowest ? oAppProStep.StepIndex : stepIndexLowest;
        //                //            oAppActionStepList.Add(
        //                //                new ApprovalActionStep
        //                //                {
        //                //                    //Id = 0,
        //                //                    ChurchBodyId = oCLGTransf.RequestorChurchBodyId,
        //                //                    ChurchBody = oCLGTransf.RequestorChurchBody,
        //                //                    MemberLeaderRoleId = oICurrApprover.Id,
        //                //                    Approver = oICurrApprover,
        //                //                    ActionStepDesc = oAppProStep.StepDesc,
        //                //                    ApprovalStepIndex = oAppProStep.StepIndex,  // stepIndex,
        //                //                    ActionStepStatus = "P", //Pending          // Comments ="",   //CurrentStep = false, // oAppProStep.StepIndex == stepIndexLowest,
        //                //                    ProcessStepRefId = oAppProStep.Id,
        //                //                    CurrentScope = "E",
        //                //                    StepRequestDate = DateTime.Now,
        //                //                    // Comments="",
        //                //                    // CurrentStep= true,
        //                //                    // ActionByMemberLeaderRoleId = vmMod.oCurrLoggedMemberId,  // actual approver
        //                //                    // ActionBy = vmMod.oCurrLoggedMember,
        //                //                    // ActionDate = DateTime.Now,
        //                //                    Status = "A", //Active
        //                //                    Created = DateTime.Now,
        //                //                    LastMod = DateTime.Now,

        //                //                    //
        //                //                    ApprovalActionId = oAppActionList.FirstOrDefault().Id,
        //                //                    ApprovalAction = oAppActionList.FirstOrDefault()
        //                //                }); ;
        //                //        }

        //                //        foreach (ApprovalActionStep oAS in oAppActionStepList)
        //                //        {
        //                //            oAS.CurrentStep = oAS.ApprovalStepIndex <= stepIndexLowest;  //concurrent will be handled 
        //                //            _context.Add(oAS);     // if (oAS.ApprovalStepIndex <= stepIndexLowest) { oAS.CurrentStep = true; break; }
        //                //        }

        //                //        //oCLGTransf.ReqStatus = "I"; //keep as it is == in prrogres. to the applicant.. Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //                //        // oCLGTransf.RequestDate = DateTime.Now;
        //                //        oCLGTransf.ToRequestDate = DateTime.Now;   // after approval or acknowledgment from the FROM congregation
        //                //        oCLGTransf.RequireApproval = oAppActionList.Count > 0; // _oCLGTransf_VM.RequireApproval,
        //                //        oCLGTransf.ApprovalStatus = "P"; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oCLGTransf_VM.ApprovalStatus,
        //                //        oCLGTransf.CurrentScope = "E";
        //                //        oCLGTransf.EApprovalActionId = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().Id : (int?)null;  //_oCLGTransf_VM.IApprovalActionId,
        //                //        oCLGTransf.EApprovalAction = oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault() : null;  //_oCLGTransf_VM.IApprovalAction, 
        //                //    }
        //                //}


        //                ////perform the transfers
        //                //else if (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "A" && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))
        //                //{
        //                //    if (! this.PerformClergyTransfer(_context, oCLGTransf, vmMod.oChurchBody.Id))
        //                //    {  //reverse action
        //                //       // ModelState.AddModelError(string.Empty, "Clergy transfer unsuccessful.");
        //                //        return Json(new { taskSuccess = false, userMess = "Clergy transfer unsuccessful. Failed operation reversing..." });
        //                //    }
        //                //}
        //            }
        //        }

        //        if (taskIndx == 10) // && vm.lsCurrApprActionSteps.Count > 0)  //FORCE-COMPLETE ...iirespective of the state
        //        {
        //            string strApprovalStatus = null;
        //            var oActionStepList = vmMod.lsCurrApprActionSteps;

        //            //create approval action...  force-complete rest of the steps  unapproved.. at least 1 step of approval
        //            if (oActionStepList.Count > 0)
        //            {
        //                foreach (ApprovalActionStep oAAStep in oActionStepList)
        //                {
        //                    oAAStep.ActionStepStatus = "F";
        //                    oAAStep.Comments = vmMod.strApproverComment;  //could also be reason from the user/applicant
        //                    oAAStep.ActionDate = DateTime.Now;
        //                    oAAStep.LastMod = DateTime.Now;

        //                    _context.Update(oAAStep);
        //                }

        //                var oAA = oActionStepList[0].ApprovalAction; // not null
        //                strApprovalStatus = this.GetApprovalActionStatus(oActionStepList, 0, "F"); // this.GetApprovalActionStatus(oActionStepList, oCLGTransf.CurrentScope == "I" ? 1 : 2);// taskIndx == 3 ? "R" : "X";   ...combination of statuses...
        //                oAA.ActionStatus = strApprovalStatus;
        //                oAA.Comments = vmMod.strApproverComment;   //could also be reason from the user/applicant
        //                oAA.LastActionDate = DateTime.Now;
        //                oAA.LastMod = DateTime.Now;

        //                _context.Update(oAA);

        //                oCLGTransf.ReqStatus = this.GetApprovalActionStatus(oActionStepList, oCLGTransf.CurrentScope == "I" ? 1 : 2, "F"); // Pending until acknowledged bcos Scope == Internal.   @External...  it still will be In progress while the Approval Status and Step status monnitor the actions on the requests   // oChuTransf.strReqStatus = "Pending";
        //                oCLGTransf.RequestDate = DateTime.Now;
        //                oCLGTransf.ApprovalStatus = strApprovalStatus; // oAppActionList.Count > 0 ? oAppActionList.FirstOrDefault().ActionStatus : null; // _oCLGTransf_VM.ApprovalStatus,   
        //                oCLGTransf.Status = "T"; //await ack... In transit
        //            }
        //        }

        //        if (taskIndx == 11) // Re-open request
        //        {
        //            //if (vmMod.lsCurrApprActionSteps.Count > 0)
        //            //{ 
        //            oCLGTransf.ReqStatus = "D"; //only declined actions are re-opened
        //            oCLGTransf.Status = "I"; //await ack... Pending
        //                                     // }
        //        }

        //        if (taskIndx == 12) // Archive request
        //        {
        //            oCLGTransf.ReqStatus = "Z";
        //            // oCLGTransf.Status = "I";  KEEP THE STATUS
        //        }

        //        //if (taskIndx == 13) // On-ward transfer... thus: forward already transferred minister... emphasis on PerformMembershipTransfer
        //        //{
        //        //    if (oCLGTransf.AttachedToChurchBodyId == null && oCLGTransf.Status == "T" && oCLGTransf.ReqStatus == "A" && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))
        //        //    {
        //        //        //perform the transfer ...and then Close the request after 'success'.
        //        //        oCLGTransf.TransferDate = DateTime.Now;    // same date to be used as the Enrollment date /Departure date for member

        //        //        if (this.PerformMemberTransfer(_context, oCLGTransf, vmMod.oChurchBody.Id) == false)
        //        //        {  //reverse action .....    ModelState.AddModelError(string.Empty, "Clergy transfer unsuccessful.");   ...add Affiliation and pay attn to AttachedTo
        //        //            return Json(new { taskSuccess = false, userMess = "Clergy transfer unsuccessful. Failed operation reversing..." });
        //        //        }

        //        //        oCLGTransf.ReqStatus = "C";     //closed... @ last approval... Transfer done                       
        //        //        oCLGTransf.Status = "Y"; //Yes... Transfer done
        //        //        oCLGTransf.MembershipStatus = "M";  // Moved /transferred  || "T";  // in transit

        //        //        //notify all appprovers
        //        //        var oAASList = _context.ApprovalActionStep.Include(t => t.ApprovalAction).Include(t => t.Approver).ThenInclude(t => t.ChurchMember).ThenInclude(t => t.ContactInfo)
        //        //            .Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId &&
        //        //                            (c.ApprovalActionId == oCLGTransf.IApprovalActionId) // || c.ApprovalActionId == oCLGTransf.EApprovalActionId)
        //        //                            ).ToList();
        //        //        oCurrApprovers.AddRange(oAASList);
        //        //    }
        //        //}

        //        //save details... locAddr
        //        List<ChurchMember> oChuMemNotifyList = new List<ChurchMember>();
        //        try
        //        {
        //            //if (oCLGTransf == null)
        //            //    { //ModelState.AddModelError(string.Empty, "Clergy transfer data not found! Please refresh and try again."); return Json(false);
        //            //    return Json(new { taskSuccess = false, userMess = "Clergy transfer data not found! Please refresh and try again." });
        //            //}

        //            if (oCLGTransf.Id > 0 && taskIndx > 1) // new or update instances already dealt with up there
        //            {
        //                try
        //                {
        //                    //var oViewCB_Fr = new ChurchBody(); oViewCB_Fr= oCLGTransf.FromChurchBody;
        //                    //var oViewCB_To = new ChurchBody(); oViewCB_To= oCLGTransf.ToChurchBody;
        //                    //var oViewCB_Attach = new ChurchBody(); oViewCB_Attach =oCLGTransf.AttachedToChurchBody;
        //                    ////
        //                    //oCLGTransf.FromChurchBody.Remove(oViewCB_Fr);
        //                    //oCLGTransf.ToChurchBody.Remove(oViewCB_To);
        //                    //oCLGTransf.AttachedToChurchBody.Remove(oViewCB_Attach);

        //                    oCLGTransf.LastMod = DateTime.Now;
        //                    _context.Update(oCLGTransf);
        //                    await _context.SaveChangesAsync();
        //                    //
        //                    //oCLGTransf.FromChurchBody = oViewCB_Fr;
        //                    //oCLGTransf.FromChurchBody = oViewCB_To;
        //                    //oCLGTransf.FromChurchBody = oViewCB_Attach;


        //                    oCLGTransf.RequestorChurchMember = _oCLGTransf.RequestorChurchMember;
        //                    oCLGTransf.RequestorChurchBody = _oCLGTransf.RequestorChurchBody;
        //                    oCLGTransf.FromChurchBody = _oCLGTransf.FromChurchBody;

        //                    //get member
        //                    if (oCLGTransf.ChurchMemberTransf == null)
        //                        oCLGTransf.ChurchMemberTransf = _context.ChurchMember.Find(oCLGTransf.ChurchMemberId);

        //                    //get cong
        //                    if (oCLGTransf.AttachedToChurchBodyId != null)
        //                        if (oCLGTransf.AttachedToChurchBody == null)
        //                            oCLGTransf.AttachedToChurchBody = _context.ChurchBody.Find(oCLGTransf.AttachedToChurchBodyId);

        //                    if (oCLGTransf.ToChurchBodyId != null)
        //                        if (oCLGTransf.ToChurchBody == null)
        //                            oCLGTransf.ToChurchBody = _context.ChurchBody.Find(oCLGTransf.ToChurchBodyId);

        //                    if (oCLGTransf.FromChurchBodyId != null)
        //                        if (oCLGTransf.FromChurchBody == null)
        //                            oCLGTransf.FromChurchBody = _context.ChurchBody.Find(oCLGTransf.FromChurchBodyId);

        //                }
        //                catch (Exception ex)
        //                {
        //                    return Json(new { taskSuccess = false, userMess = "Error occured while processing request: " + ex.Message });
        //                }
        //            }


        //            //if (oCLGTransf.Id == 0)
        //            //{
        //            //    switch (taskIndx)
        //            //    {
        //            //        case 1:
        //            //            oUserMsg = "Saved Clergy transfer details successfully.";
        //            //            break;

        //            //        case 2:
        //            //            oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //            //            oNotifMsg = "Clergy transfer request sent successfully. #Request_Id: " + oCLGTransf.Id;
        //            //            oUserMsg = oNotifMsg;
        //            //            break;
        //            //    }
        //            //}

        //            var oNotifMsg = ""; var oUserMsg = "";
        //            string strAttachedToChurchBody = "";
        //            if (oCLGTransf.AttachedToChurchBody != null)
        //                strAttachedToChurchBody = " to " + oCLGTransf.AttachedToChurchBody.Name;

        //            switch (taskIndx)
        //            {
        //                case 1:
        //                    oUserMsg = "Saved Clergy transfer details successfully.";
        //                    break;

        //                case 2:  //send  ... notify Minister after successful transfer
        //                    //oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                    oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " sent successfully. #Request_Id: " + oCLGTransf.Id;
        //                    oUserMsg = oNotifMsg;
        //                    break;

        //                case 3:  //recall
        //                    { //check for steps available... sent notification to all approvers and the applicant, FromChurchBody, ToChurchBody
        //                        foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                            if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                    oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                        // oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);  //requestor : [to], approvers [ to /from]
        //                        oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody +
        //                            " has been recalled by requestor." +
        //                            Environment.NewLine + "#Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = oNotifMsg;
        //                    }
        //                    break;

        //                case 4: //cancel
        //                    { //check for steps available... sent notification to all approvers and the applicant, FromChurchBody, ToChurchBody
        //                        foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                            if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                    oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                        // oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                        oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " has been terminated by requestor. #Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = oNotifMsg;
        //                    }
        //                    break;

        //                case 5: //resubmit
        //                    {
        //                        // oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                        oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody +
        //                            " has been resubmitted successfully. #Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = oNotifMsg;
        //                    }
        //                    break;

        //                case 6: //ack
        //                    {
        //                        oNotifMsg = "Clergy transfer from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody;

        //                        if (oCLGTransf.FromChurchBodyId == vmMod.oChurchBody.Id)
        //                            oNotifMsg += " duly acknowledged by current congregation of the Minister on-transfer.";
        //                        else //if (oCLGTransf.ToChurchBodyId == vmMod.oChurchBody.Id)
        //                            oNotifMsg += " duly acknowledged by target congregation.";

        //                        // oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                        oNotifMsg += " #Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = "Clergy transfer from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " acknowledged.";
        //                    }
        //                    break;


        //                //        (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "A" && (oCLGTransf.ApprovalStatus == "A" || oCLGTransf.ApprovalStatus == "F"))
        //                //else if ((oCLGTransf.CurrentScope == "I" && oCLGTransf.ReqStatus == "P" && oCLGTransf.ApprovalStatus == "P") ||
        //                //                (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P"))

        //                case 7:
        //                    {
        //                        if (oCLGTransf.ReqStatus == "A")
        //                        {
        //                            foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                                if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                        oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                            oNotifMsg = "Clergy transfer ";
        //                            if (oCLGTransf.ReqStatus == "Y")   //moved membership
        //                            {
        //                                if (oCLGTransf.AttachedToChurchBodyId != null && oCLGTransf.MembershipStatus == "M")
        //                                    oNotifMsg += "from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " successfully completed. Membership transferred alongside.";
        //                                else
        //                                    oNotifMsg += "from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " successfully completed.";
        //                                //
        //                                oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                            }
        //                            else
        //                            {
        //                                oNotifMsg += "from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " partialy completed. Approvals done but Minister not transferred yet.";
        //                            }
        //                        }

        //                        else
        //                        {
        //                            int stepsLeft = 0;
        //                            foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                                if (oStep.ActionStepStatus != "A" && oStep.ActionStepStatus != "F")
        //                                    stepsLeft++;
        //                            //
        //                            ChurchMember t_cm = null; string strApproverName = "[Approver]";
        //                            if (vmMod.oCurrApprovalActionStep.Approver != null)
        //                                if (vmMod.oCurrApprovalActionStep.Approver.ChurchMember != null)
        //                                {
        //                                    t_cm = vmMod.oCurrApprovalActionStep.Approver.ChurchMember;
        //                                    // oChuMemNotifyList.Add(t_cm);
        //                                    strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
        //                                    //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim();
        //                                    var t_lr = vmMod.oCurrApprovalActionStep.Approver.LeaderRole;
        //                                    strApproverName += (t_lr != null ? (!string.IsNullOrEmpty(t_lr.RoleName) ? " (" + t_lr.RoleName + ")" : "") : "").Trim();
        //                                }

        //                            oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody;
        //                            oNotifMsg += " is " + this.GetRequestProcessStatusDesc(oCLGTransf.ReqStatus) + ". " + Environment.NewLine + strApproverName + " has approved " + (stepsLeft > 0 ? ". " + stepsLeft + " more approval" + (stepsLeft > 1 ? "s" : "").ToString() + " left to complete" : "");

        //                        }

        //                        //oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                        oNotifMsg += " #Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " approved.";
        //                    }
        //                    break;

        //                case 8:
        //                    {
        //                        //if (oCLGTransf.ReqStatus == "D")
        //                        //{ 
        //                        foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                            if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                    oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                        oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " has been declined.";

        //                        //  oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                        oNotifMsg += " Please check notes attached. #Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " sucessfully declined.";
        //                    }
        //                    break;

        //                case 9:
        //                    {
        //                        //if (oCLGTransf.ReqStatus == "I")
        //                        //{
        //                        ChurchMember t_cm = null; string strApproverName = "[Approver]";
        //                        if (vmMod.oCurrApprovalActionStep.Approver != null)
        //                            if (vmMod.oCurrApprovalActionStep.Approver.ChurchMember != null)
        //                            {
        //                                t_cm = vmMod.oCurrApprovalActionStep.Approver.ChurchMember;      // oChuMemNotifyList.Add(t_cm);
        //                                strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
        //                                //(((t_cm.FirstName + ' ' + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() + " " + (!string.IsNullOrEmpty(t_cm.Title) ? "(" + t_cm.Title + ")" : "")).Trim();
        //                                var t_lr = vmMod.oCurrApprovalActionStep.Approver.LeaderRole;
        //                                strApproverName += (t_lr != null ? (!string.IsNullOrEmpty(t_lr.RoleName) ? " (" + t_lr.RoleName + ")" : "") : "").Trim();
        //                            }

        //                        oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody +
        //                            " has been put 'On hold' tentative by " + strApproverName + ". Please check notes attached.";

        //                        // oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                        oNotifMsg += " #Request_Id: " + oCLGTransf.Id;
        //                        oUserMsg = "Approval step suspended successfully for Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody;
        //                        //}
        //                    }
        //                    break;

        //                case 10:
        //                    //if (oCLGTransf.ReqStatus == "A")
        //                    //{
        //                    foreach (ApprovalActionStep oStep in vmMod.lsCurrApprActionSteps)
        //                        if (oStep.Approver != null) if (oStep.Approver.ChurchMember != null)
        //                                oChuMemNotifyList.Add(oStep.Approver.ChurchMember);

        //                    oNotifMsg = "Clergy transfer request " +
        //                        (oCLGTransf.CurrentScope == "I" || (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P") ? "to " + oCLGTransf.ToChurchBody.Name : "from " + oCLGTransf.FromChurchBody.Name) +
        //                        oCLGTransf.ToChurchBody.Name.ToUpper();
        //                    if (oCLGTransf.CurrentScope == "I" || (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P"))
        //                        oNotifMsg += " has been approved [force-completed] by local congregation and forwarded to the target congregation pending their acknowledgment/approval."; // and then forwarded to the target congregation.";
        //                    else
        //                        oNotifMsg += " successfully approved [force-completed].";

        //                    oChuMemNotifyList.Add(oCLGTransf.ChurchMemberTransf);
        //                    oNotifMsg += " #Request_Id: " + oCLGTransf.Id;
        //                    oUserMsg = "Clergy transfer request " +
        //                        (oCLGTransf.CurrentScope == "I" || (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P") ? "to " + oCLGTransf.ToChurchBody.Name : "from " + oCLGTransf.FromChurchBody.Name) +
        //                        " force-completed.";
        //                    break;

        //                case 11:
        //                    oNotifMsg = "Clergy transfer from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " successfully re-opened.";
        //                    oUserMsg = oNotifMsg;
        //                    break;

        //                case 12:
        //                    //  oNotifMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + " successfully re-opened.";
        //                    oUserMsg = "Clergy transfer request from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody +
        //                            " has been archived" +
        //                            Environment.NewLine + "#Request_Id: " + oCLGTransf.Id; ;
        //                    break;

        //                    //case 13:
        //                    //    oNotifMsg += "On-ward Clergy transfer from " + oCLGTransf.FromChurchBody.Name + strAttachedToChurchBody + " successfully completed. Membership transferred alongside. " +
        //                    //        Environment.NewLine + "#Request_Id: " + oCLGTransf.Id; ;
        //                    //    oUserMsg = oNotifMsg;
        //                    //    break;
        //            }

        //            //construct notification messages
        //            if (oChuMemNotifyList.Count > 0)
        //            {
        //                MailAddressCollection listToAddr = new MailAddressCollection();
        //                MailAddressCollection listCcAddr = new MailAddressCollection();
        //                MailAddressCollection listBccAddr = new MailAddressCollection();
        //                //
        //                List<string> oNotifPhone_List = new List<string>();
        //                List<string> oNotifMessageList = new List<string>();
        //                _context.ContactInfo.Where(c => c.ChurchBodyId == oCLGTransf.RequestorChurchBodyId || c.ChurchBodyId == oCLGTransf.FromChurchBodyId || c.ChurchBodyId == oCLGTransf.AttachedToChurchBodyId).Load();
        //                ContactInfo oMemCI = null;  // _context.ContactInfo.Find(oCLGTransf.ChurchMemberTransf.ContactInfoId);   //searches the local cache loaded
        //                var strRequestor = ""; var strSalutation = "";
        //                var _oNotifMsg = oNotifMsg;
        //                var strSenderId = oCLGTransf.CurrentScope == "I" || (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P") ?
        //                    oCLGTransf.FromChurchBody.Name : oCLGTransf.ToChurchBody.Name;


        //                string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

        //                //var strUrl1 =  HttpContext.Request.Query["ReturnUrl"];

        //                foreach (var oCM in oChuMemNotifyList)
        //                {
        //                    var num = "";
        //                    /*oMemCI = _context.ContactInfo.Find(oCLGTransf.ChurchMemberTransf.ContactInfoId);  */ //searches the local cache loaded
        //                    if (oCM.ContactInfoId != null)
        //                        oMemCI = _context.ContactInfo.Find(oCM.ContactInfoId);

        //                    if (oMemCI != null && oCM != null)
        //                    {  //get Custom salution... else use greeeting of the day: morning, afternoon, evening
        //                        //strSalutation = "Asomdwei nka wo!";
        //                        if (string.IsNullOrEmpty(strSalutation))
        //                        {
        //                            var ts = DateTime.Now.TimeOfDay;
        //                            if (ts.Hours >= 0 && ts.Hours < 12) strSalutation = "Good morning";
        //                            else if (ts.Hours <= 16) strSalutation = "Good afternoon";
        //                            else if (ts.Hours < 24) strSalutation = "Good evening";
        //                        }
        //                        if (string.IsNullOrEmpty(oMemCI.MobilePhone1))
        //                        { num = oMemCI.MobilePhone1; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
        //                        if (num.Length == 0)
        //                            if (string.IsNullOrEmpty(oMemCI.MobilePhone2))
        //                            { num = oMemCI.MobilePhone2; if (num.Length <= 10 && !num.StartsWith("233")) num = "233" + num.Substring(1, num.Length - 1); }
        //                        if (num.Length > 0)
        //                        {
        //                            strRequestor = (((oCM.Title + ' ' + oCM.FirstName).Trim() + " " + oCM.MiddleName).Trim() + " " + oCM.LastName).Trim();
        //                            oNotifMsg = strRequestor + ", " + strSalutation + ". Please " + (oCLGTransf.ChurchMemberId == oCM.Id ? "your " : strRequestor + ".") + _oNotifMsg + ". Thank you.";
        //                            //
        //                            oNotifPhone_List.Add(num);
        //                            oNotifMessageList.Add(oNotifMsg);

        //                            //email recipients... applicant, church   ... specific e-mail content
        //                            listToAddr.Add(new MailAddress(oMemCI.Email, strRequestor));
        //                            SendEmailNotification(strSenderId, vmMod.strTransferType + ": " + vmMod.strTransfMemberDesc, oNotifMsg +
        //                                Environment.NewLine + "Open request: " + strUrl,
        //                                listToAddr, listCcAddr, listBccAddr, null);
        //                        }
        //                    }
        //                }

        //                //send notifications... sms, email
        //                if (oNotifPhone_List.Count > 0 && oNotifPhone_List.Count == oNotifMessageList.Count)
        //                    SendSMSNotification(oNotifPhone_List, oNotifMessageList, true);

        //                //email... approvers
        //                if (oCurrApprovers.Count > 0)
        //                {
        //                    MailAddressCollection listToAddr_Appr = new MailAddressCollection();
        //                    MailAddressCollection listCcAddr_Appr = new MailAddressCollection();
        //                    MailAddressCollection listBccAddr_Appr = new MailAddressCollection();

        //                    foreach (var oAAS in oCurrApprovers)
        //                    {
        //                        var strApproverName = ""; var t_cm = oAAS.Approver.ChurchMember;
        //                        if (t_cm != null)
        //                        {
        //                            strApproverName = ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim();
        //                            if (t_cm.ContactInfo != null)
        //                                listToAddr_Appr.Add(new MailAddress(t_cm.ContactInfo.Email, strApproverName));
        //                        }
        //                    }

        //                    if (listToAddr_Appr.Count > 0)
        //                    {  //var strSenderId = oCLGTransf.CurrentScope == "I" || (oCLGTransf.CurrentScope == "E" && oCLGTransf.ReqStatus == "I" && oCLGTransf.ApprovalStatus == "P") ? oCLGTransf.FromChurchBody.Name : oCLGTransf.ToChurchBody.Name;
        //                        SendEmailNotification(strSenderId, vmMod.strTransferType + ": " + vmMod.strTransfMemberDesc, vmMod.strTransMessage +
        //                            Environment.NewLine + "Open request: " + strUrl,
        //                            listToAddr, listCcAddr, listBccAddr, null);
        //                    }
        //                }
        //            }

        //            vmMod.oChurchTransfer = oCLGTransf;
        //            TempData.Put("oVmCurr", vmMod);
        //            //  TempData.Keep();

        //            return Json(new { taskSuccess = true, userMess = oUserMsg });

        //            // return Json(true);
        //        }

        //        catch (Exception ex)
        //        {
        //            //  ViewBag.UserMsg = "Requested action could not be performed successfully. Err: " + ex.ToString();
        //            return Json(new { taskSuccess = false, userMess = "Error occured while processing request:" + ex.Message });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { taskSuccess = false, userMess = "Error occured while processing request:" + ex.Message });
        //    }
        //}

        //private bool SendSMSNotification(List<string> oMSISDN_List, List<string> strMsgList, bool customMsg = false)
        //{
        //    try
        //    {
        //        if (customMsg)
        //        { if (strMsgList.Count == 0) return false; }
        //        else
        //        { if (strMsgList.Count != oMSISDN_List.Count) return false; }

        //        var successCount = 0; var msgResponse = false;
        //        for (var i = 0; i < oMSISDN_List.Count; i++)
        //        {
        //            //var oSendSMS = new AppUtility_SMS();
        //            //bool msgResponse = oSendSMS.sendMessage("RHEMAChurch", oMSISDNList[i], customMsg ? strMsgList[i] : strMsgList[0], "sdarteh", "Sdgh?2020");
        //            if (oMSISDN_List[i] != null && strMsgList[i] != null)
        //                msgResponse = new AppUtility_SMS().sendMessage("RHEMAChurch", oMSISDN_List[i], customMsg ? strMsgList[i] : strMsgList[0], "sdarteh", "Sdgh?2020");

        //            if (msgResponse) successCount++;
        //        }

        //        //write fail to logs...
        //        return successCount >= 0;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}


        //public bool SendEmailNotification(string senderId, string strSubject, string strMailMess,
        //    MailAddressCollection lsToAddr, MailAddressCollection lsCcAddr, MailAddressCollection lsBccAddr, string docAttachFilePath)
        ////HttpPostedFileBase fileUploader) SendMailwithAttachment.Models.MailModel objModelMail
        //{
        //    try
        //    {
        //        string strFrom = "samdartgroup@gmail.com"; //example:- sourabh9303@gmail.com
        //                                                   //  var fromAddr = new MailAddress(from, "RHEMACLOUD"); 

        //        using (MailMessage mail = new MailMessage()) // strFrom, senderId))
        //        {
        //            mail.From = new MailAddress(strFrom, senderId);
        //            mail.Subject = strSubject;
        //            mail.Body = strMailMess;
        //            foreach (var oTo in lsToAddr) mail.To.Add(oTo);
        //            foreach (var oCc in lsCcAddr) mail.To.Add(oCc);
        //            foreach (var oBcc in lsBccAddr) mail.To.Add(oBcc);

        //            //if (fileUploader != null)
        //            //{
        //            //    string fileName = Path.GetFileName(fileUploader.FileName);
        //            //    mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
        //            //}

        //            mail.IsBodyHtml = false;
        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = "smtp.gmail.com";
        //            smtp.EnableSsl = true;
        //            NetworkCredential networkCredential = new NetworkCredential(strFrom, "Sdgh1284");
        //            smtp.UseDefaultCredentials = false;
        //            smtp.Credentials = networkCredential;
        //            smtp.Port = 587;
        //            smtp.Send(mail);
        //            // ViewBag.Message = "Sent";

        //            return true;  // View("Index", objModelMail);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }


        //}
        //private bool _SendSMSNotification(List<ChurchMember> oChurchMemberList, string strMsg)
        //{
        //    try
        //    {
        //        foreach (ChurchMember oChurchMember in oChurchMemberList)
        //        {
        //            if (oChurchMember != null)
        //            {
        //                var oContact = _context.ContactInfo.Where(c => c.ChurchBodyId == oChurchMember.ChurchBodyId && c.Id == oChurchMember.ContactInfoId).FirstOrDefault();   //.Find(oChurchMember.ContactInfoId);
        //                if (oContact != null)
        //                {
        //                    var num1 = oContact?.MobilePhone1;
        //                    var num2 = oContact?.MobilePhone2;
        //                    if (num1.Length <= 10 && !num1.StartsWith("233"))
        //                        num1 = "233" + num1.Substring(1, num1.Length - 1);
        //                    if (num2.Length <= 10 && !num2.StartsWith("233"))
        //                        num2 = "233" + num2.Substring(1, num2.Length - 1);

        //                    //If MessageBox.Show(Me, "Send member ( " & num & " ) SMS?", Me.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.OK Then
        //                    //    ' MsgBox(num)
        //                    var strMemberFullName = (((oChurchMember.FirstName + ' ' + oChurchMember.MiddleName).Trim() + " " + oChurchMember.LastName).Trim() + " " + (!string.IsNullOrEmpty(oChurchMember.Title) ? "(" + oChurchMember.Title + ")" : "")).Trim();

        //                    var oSendSMS = new AppUtility_SMS();
        //                    bool resMsg1 = oSendSMS.sendMessage("RHEMAChurch", num1, "Dear " + strMemberFullName + ", " + strMsg, "sdarteh", "Sdgh?2020");
        //                    bool resMsg2 = oSendSMS.sendMessage("RHEMAChurch", num2, "Dear " + strMemberFullName + ", " + strMsg, "sdarteh", "Sdgh?2020");

        //                    return (resMsg1 || resMsg1);
        //                }
        //            }
        //        }

        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> AcknowledgeTransferRequest(int? oCurrChuBodyId, int? oChurchTransferId)
        //{
        //    var blAckOnly = true;
        //    var oChuTransf = _context.ChurchTransfer.Where(c => c.Id == oChurchTransferId && ((c.RequestorChurchBodyId == oCurrChuBodyId && c.CurrentScope == "I") ||
        //                                                    (c.ToChurchBodyId == oCurrChuBodyId && c.CurrentScope == "E"))).FirstOrDefault();  //.Find(oChurchTransferId);
        //    if (oChuTransf != null)
        //    {
        //        if (oChuTransf.ReqStatus == "P") // && oChuTransf.RequireApproval == false)
        //        {
        //            oChuTransf.ReqStatus = "R";  //oChuTransf.CurrentScope == I or E

        //            if (oChuTransf.RequireApproval == false ||
        //                (oChuTransf.RequireApproval == true && oChuTransf.IApprovalAction?.Status == "A"))
        //            {
        //                if (oChuTransf.RequestorChurchBodyId == oCurrChuBodyId && oChuTransf.CurrentScope == "I")  // jux internal approvals... keep the approval history anyways
        //                {
        //                    oChuTransf.CurrentScope = "E";
        //                }
        //                else if (oChuTransf.ToChurchBodyId == oCurrChuBodyId && oChuTransf.CurrentScope == "E")   //sure now that member is moving out...
        //                {
        //                    //transfer member...
        //                    if (PerformMemberTransfer(_context, oChuTransf, oCurrChuBodyId) == false)
        //                    {
        //                        ModelState.AddModelError(string.Empty, "Member transfer incurred error!");
        //                        return Json(false);
        //                    }

        //                    blAckOnly = false;
        //                }
        //            }
        //        }

        //        try
        //        {
        //            _context.Update(oChuTransf);

        //            //Save the request
        //            await _context.SaveChangesAsync();

        //            var oCB = _context.ChurchBody.Find(oChuTransf.ToChurchBodyId);

        //            //send notifications... email and sms
        //            var msg = blAckOnly ? "Member transfer request to " + oCB?.Name + " acknowledged. However, request is yet to be approved to continue." : "Please your request for transfer to " + oCB?.Name + " has been received and being processed. Thank you";
        //            //  SendSMSNotification(oChuTransf.ChurchMemberTransf, msg);

        //            return Json(true);
        //        }

        //        catch (Exception ex)
        //        {
        //            return Json(false);
        //        }
        //    }
        //    else
        //    {
        //        return Json(false);
        //    }
        //}

        //private bool PerformMemberTransfer(ChurchModelContext currCtx, ChurchTransfer oChuTransf, int? oCurrChuBodyId)
        //{
        //    try
        //    {
        //        //the request cong confirms that To cong should go ahead and transfer the member
        //        if (oCurrChuBodyId != oChuTransf.RequestorChurchBodyId && oChuTransf.CurrentScope != "E" && oChuTransf.ReqStatus != "A" && oChuTransf.ApprovalStatus != "A") return false;

        //        //make member past member of from cong... add new member @ To cong
        //        var oCurrChuMember = currCtx.ChurchMember.Find(oChuTransf.ChurchMemberId);
        //        if (oCurrChuMember != null)
        //        {
        //            var _strCTdesc = GetTransferTypeDesc(oChuTransf.TransferType) + " - " + GetTransferSubTypeDesc(oChuTransf.TransferSubType);
        //            var strCTdesc = oChuTransf.TransferType + " - " + oChuTransf.TransferSubType;

        //            // oChuTransf.TransferDate = DateTime.Now;   ... use what date came

        //            //
        //            //oNewChuMember.ActiveStatus = true;   //deleted                   
        //            //oCurrChuMember.IsCurrent = false;
        //            //oCurrChuMember.Departed = oChuTransf.TransferDate;
        //            //oCurrChuMember.DepartReason = "Transferred";
        //            ////oCurrChuMember.Enrolled = null;  keep as it is
        //            ////oNewChuMember.EnrollReason = "Transferred";   keep as it is
        //            //oCurrChuMember.IsDeceased = false;  
        //            oCurrChuMember.LastMod = DateTime.Now;
        //            // 
        //            currCtx.Update(oCurrChuMember);

        //            // create new member... unique member id = churchBodyId + member id
        //            var oNewChuMember = new ChurchMember();
        //            // oNewChuMember = currCtx.ChurchMember.Find(oChuTransf.ChurchMemberId);  //oChuTransf.ChurchMemberTransf;
        //            // oNewChuMember.Id = 0;
        //            //oCurrChuMember.MemberGlobalId = oChuTransf.ChurchMemberTransf.MemberGlobalId;
        //            // oNewChuMember.OwnerChurchBodyId = (int)oChuTransf.ToChurchBodyId;
        //            // oNewChuMember.ChurchBody = oChuTransf.ToChurchBody;

        //            oNewChuMember.ChurchBodyId = (int)oChuTransf.ToChurchBodyId;
        //            oNewChuMember.MemberGlobalId = oCurrChuMember.MemberGlobalId;
        //            oNewChuMember.AppGlobalOwnerId = oCurrChuMember.AppGlobalOwnerId;
        //            // oNewChuMember.AppGlobalOwner = oCurrChuMember.AppGlobalOwner;
        //            oNewChuMember.MemberCode = oCurrChuMember.MemberCode;
        //            oNewChuMember.Title = oCurrChuMember.Title;
        //            oNewChuMember.FirstName = oCurrChuMember.FirstName;
        //            oNewChuMember.MiddleName = oCurrChuMember.MiddleName;
        //            oNewChuMember.LastName = oCurrChuMember.LastName;
        //            oNewChuMember.MaidenName = oCurrChuMember.MaidenName;
        //            oNewChuMember.Gender = oCurrChuMember.Gender;
        //            oNewChuMember.DateOfBirth = oCurrChuMember.DateOfBirth;
        //            oNewChuMember.MaritalStatus = oCurrChuMember.MaritalStatus;
        //            oNewChuMember.MarriageType = oCurrChuMember.MarriageType;
        //            oNewChuMember.MarriageRegNo = oCurrChuMember.MarriageRegNo;
        //            oNewChuMember.NationalityId = oCurrChuMember.NationalityId;
        //            // oNewChuMember.Nationality = oCurrChuMember.MemberCode;
        //            oNewChuMember.IDTypeId = oCurrChuMember.IDTypeId;
        //            //oNewChuMember.IDType = oCurrChuMember.IDType;
        //            oNewChuMember.NationalIDNum = oCurrChuMember.NationalIDNum;
        //            oNewChuMember.ContactInfoId = oCurrChuMember.ContactInfoId;
        //            //  oNewChuMember.ContactInfo = oCurrChuMember.ContactInfo;
        //            oNewChuMember.Hobbies = oCurrChuMember.Hobbies;
        //            oNewChuMember.Hometown = oCurrChuMember.Hometown;
        //            oNewChuMember.HometownRegionId = oCurrChuMember.HometownRegionId;
        //            // oNewChuMember.HometownRegion = oCurrChuMember.HometownRegion;
        //            oNewChuMember.MotherTongueId = oCurrChuMember.MotherTongueId;
        //            // oNewChuMember.MotherTongue = oCurrChuMember.MotherTongue;
        //            oNewChuMember.PhotoUrl = oCurrChuMember.PhotoUrl;
        //            oNewChuMember.OtherInfo = oCurrChuMember.OtherInfo;
        //            //oNewChuMember.EducationLevelId = oCurrChuMember.EducationLevelId;
        //            oNewChuMember.IsActivated = true;   //deletion only...flag until clean up or archive

        //            //
        //            //oNewChuMember.IsCurrent = false;   //requires activation from Target congregation                    
        //            //oNewChuMember.Enrolled = null; //oChuTransf.TransferDate;  //date activated  
        //            // oNewChuMember.EnrollReason = "Transferred";   ... check church-life rather /status
        //            //oNewChuMember.IsDeceased = false;
        //            //oNewChuMember.Departed = null; // oChuTransf.TransferDate;
        //            //oNewChuMember.DepartReason = null; // "Transferred";
        //            oNewChuMember.Created = DateTime.Now;
        //            oNewChuMember.LastMod = DateTime.Now;
        //            //
        //            currCtx.Add(oNewChuMember);


        //            //CHECK ON CHURCH-LIFE DETAILS TO SEE IF... ANY MUST BE TRANSFERRED
        //            var oMemCL = currCtx.MemberChurchLife.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id &&
        //                               c.IsCurrentMember == true).FirstOrDefault();
        //            if (oMemCL == null) return false;   //member must have Pos in the church

        //            var oNewMemCL = new MemberChurchLife();
        //            //  oNewMemCL.ChurchBodyServiceId = null;
        //            oNewMemCL.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //            oNewMemCL.ChurchMemberId = oNewChuMember.Id;  // oNewChuMember.Id; // oMemPos.ChurchMemberId;                    
        //            oNewMemCL.Joined = oChuTransf.TransferDate; // DateTime.Now;                                                
        //            oNewMemCL.EnrollReason = strCTdesc; // oChuTransf.TransferType;
        //            oNewMemCL.IsCurrentMember = true;
        //            // oNewMemCL.Departed = null;
        //            // oNewMemCL.DepartReason = null;
        //            oNewMemCL.IsDeceased = false;
        //            oNewMemCL.IsMemBaptized = oMemCL.IsMemBaptized;
        //            oNewMemCL.IsMemConfirmed = oMemCL.IsMemConfirmed;
        //            oNewMemCL.IsMemCommunicant = oMemCL.IsMemCommunicant;
        //            oNewMemCL.NonCommReason = oMemCL.NonCommReason;
        //            //oNewMemCL.IsPioneer  = oMemCL.IsPioneer;
        //            oNewMemCL.Created = DateTime.Now;
        //            oNewMemCL.LastMod = DateTime.Now;

        //            currCtx.Add(oNewMemCL);

        //            //update the old to past ...  
        //            oMemCL.IsCurrentMember = false; //past             ... WORK  ON THIS CODE LATER
        //            oMemCL.Departed = oChuTransf.TransferDate; // oNewMemPos.Until;
        //            oMemCL.DepartReason = strCTdesc; // oChuTransf.TransferType;
        //            oMemCL.LastMod = DateTime.Now;
        //            //
        //            currCtx.Update(oMemCL);



        //            //update church Pos/position... put new member on lowest Pos... except clergy transfer
        //            var oMemPos = currCtx.MemberPosition.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id &&
        //                                c.CurrentPos == true).FirstOrDefault();
        //            if (oMemPos == null) return false;   //member must have Pos in the church

        //            //get new status... from the old   
        //            var oNewMemPos = new MemberPosition();
        //            if (oChuTransf.TransferType == "MT")
        //            {
        //                var oMinPos = _context.ChurchPosition.Where(x => x.GradeLevel ==
        //                                _context.ChurchPosition.Where(y => y.ChurchBody.AppGlobalOwnerId == oChuTransf.ToChurchBody.AppGlobalOwnerId)
        //                                .Max(y => y.GradeLevel)).Take(1).FirstOrDefault();
        //                if (oMinPos == null) return false;
        //                oNewMemPos.ChurchPositionId = oMinPos.Id;
        //            }
        //            else if (oChuTransf.TransferType == "CT")
        //            {  //Pos unchanged
        //                oNewMemPos.ChurchPositionId = oMemPos.Id;
        //            }

        //            oNewMemPos.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //            oNewMemPos.ChurchMemberId = oNewChuMember.Id; // oMemPos.ChurchMemberId;                    
        //            oNewMemPos.DateAssigned = oChuTransf.TransferDate; // DateTime.Now;
        //            oNewMemPos.Until = null;
        //            oNewMemPos.Comments = oChuTransf.TransferType;
        //            oNewMemPos.CurrentPos = true;
        //            oNewMemPos.Created = DateTime.Now;
        //            oNewMemPos.LastMod = DateTime.Now;
        //            //
        //            currCtx.Add(oNewMemPos);

        //            //update the old to past ...  // var oMemPos = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
        //            oMemPos.CurrentPos = false; //past             ... WORK  ON THIS CODE LATER
        //            oMemPos.Until = oChuTransf.TransferDate; // oNewMemPos.Until;
        //            oMemPos.Comments = oChuTransf.TransferType;
        //            oMemPos.LastMod = DateTime.Now;
        //            //
        //            currCtx.Update(oMemPos);


        //            //update the old to past
        //            var oMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrent == true).FirstOrDefault();
        //            if (oMemStatus == null) return false;

        //            //get new status... from the old
        //            var oNewMemStatus = new MemberStatus();   // oNewMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault(); // oMemStatus;
        //            //  oNewMemStatus.Id = 0;
        //            oNewMemStatus.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //            oNewMemStatus.ChurchMemberId = oNewChuMember.Id; // oMemStatus.ChurchMemberId;
        //            oNewMemStatus.ChurchMemStatusId = 1; // Regular... oMemStatus.ChurchMemStatusId;   ... WORK  ON THIS CODE LATER
        //            oNewMemStatus.Since = oChuTransf.TransferDate; // DateTime.Now;
        //            oNewMemStatus.Until = null;
        //            oNewMemStatus.Comments = oChuTransf.TransferType;
        //            oNewMemStatus.IsCurrent = true;
        //            oNewMemStatus.Created = DateTime.Now;
        //            oNewMemStatus.LastMod = DateTime.Now;
        //            //
        //            currCtx.Add(oNewMemStatus);

        //            //update the old to past ...  // var oMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
        //            oMemStatus.ChurchMemStatusId = 4; //past             ... WORK  ON THIS CODE LATER
        //            oMemStatus.Until = oChuTransf.TransferDate;
        //            oMemStatus.Comments = oChuTransf.TransferType;
        //            oMemStatus.LastMod = DateTime.Now;
        //            //
        //            currCtx.Update(oMemStatus);

        //            ////sectors... 
        //            var oMemSectors = currCtx.MemberChurchSector.Where(c => c.Sector.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.CurrSector == true).ToList();
        //            foreach (var oMS in oMemSectors)
        //            {
        //                oMS.CurrSector = false;
        //                oMS.Departed = oChuTransf.TransferDate;
        //                oMS.LastMod = DateTime.Now;
        //                //
        //                currCtx.Update(oMS);
        //            }

        //            //... roles
        //            var oMemRoles = currCtx.MemberLeaderRole.Include(t => t.LeaderRole).ThenInclude(t => t.LeaderRoleCategory)
        //                .Where(c => c.LeaderRole.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrServing == true).ToList();
        //            foreach (var oMR in oMemRoles)
        //            {
        //                oMR.IsCurrServing = false;
        //                oMR.Completed = oChuTransf.TransferDate;
        //                oMR.CompletionReason = oChuTransf.TransferType;
        //                oMR.LastMod = DateTime.Now;
        //                //
        //                currCtx.Update(oMR);
        //            }

        //            //CT move with roles
        //            if (oChuTransf.TransferType == "CT")
        //            {
        //                var oMinRoleList = oMemRoles.Where(c => c.LeaderRole?.LeaderRoleCategory?.IsCoreRole == true && c.LeaderRole?.LeaderRoleCategory?.IsOrdainedRole == true);
        //                foreach (var oMinRole in oMinRoleList)
        //                {
        //                    //get new status... from the old
        //                    var oNewMemRole = new MemberLeaderRole();
        //                    oNewMemRole.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //                    oNewMemRole.ChurchMemberId = oNewChuMember.Id; // oMemRole.ChurchMemberId;
        //                    oNewMemRole.LeaderRoleId = oMinRole.LeaderRoleId;
        //                    oNewMemRole.Commenced = oChuTransf.TransferDate;
        //                    oNewMemRole.Completed = null;
        //                    oNewMemRole.CompletionReason = oChuTransf.TransferType;
        //                    oNewMemRole.IsCurrServing = true;
        //                    oNewMemRole.Created = DateTime.Now;
        //                    oNewMemRole.LastMod = DateTime.Now;
        //                    //
        //                    currCtx.Add(oNewMemRole);
        //                }
        //            }

        //            //user profiles... deactivate
        //            var oUserProfile = currCtx.UserProfile.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.UserStatus == "A").ToList();
        //            foreach (var oUP in oUserProfile)
        //            {
        //                //user profile group... deactivate
        //                var oUserProfileGroup = currCtx.UserProfileGroup.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.UserProfileId == oUP.Id && c.Status == "A").ToList();
        //                foreach (var oUPG in oUserProfileGroup)
        //                {
        //                    oUPG.Status = "D";
        //                    oUPG.LastMod = DateTime.Now;
        //                    //
        //                    currCtx.Update(oUPG);
        //                }

        //                //user profile group... deactivate
        //                var oUserProfileRole = currCtx.UserProfileRole.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.UserProfileId == oUP.Id && c.ProfileRoleStatus == "A").ToList();
        //                foreach (var oUPR in oUserProfileRole)
        //                {
        //                    oUPR.ProfileRoleStatus = "D";
        //                    oUPR.LastMod = DateTime.Now;
        //                    //
        //                    currCtx.Update(oUPR);
        //                }

        //                //finally the master...
        //                oUP.UserStatus = "D";
        //                oUP.LastMod = DateTime.Now;
        //                //
        //                currCtx.Update(oUP);
        //            }
        //        }

        //        // oChuTransf.ReqStatus = "C";     //Closed... member transferred
        //        // oChuTransf.ApprovalStatus = "A";   // Assuming all the necessay steps/action steps have been approved.

        //        return true;
        //    }

        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //private bool PerformCLGMembershipTransfer(ChurchModelContext currCtx, ChurchTransfer oChuTransf, int? oCurrChuBodyId)
        //{
        //    try
        //    {
        //        //the request cong confirms that To cong should go ahead and transfer the member
        //        //  if (oCurrChuBodyId != oChuTransf.RequestorChurchBodyId && oChuTransf.CurrentScope != "E" && oChuTransf.ReqStatus != "A" && oChuTransf.ApprovalStatus != "A") return false;


        //        //make member past member of from cong... add new member @ To cong
        //        var oCurrChuMember = currCtx.ChurchMember.Find(oChuTransf.ChurchMemberId);
        //        if (oCurrChuMember != null)
        //        {
        //            var _strCTdesc = GetTransferTypeDesc(oChuTransf.TransferType) + " - " + GetTransferSubTypeDesc(oChuTransf.TransferSubType);
        //            var strCTdesc = oChuTransf.TransferType + " - " + oChuTransf.TransferSubType;
        //            // oChuTransf.TransferDate = DateTime.Now;   ... use what date came

        //            if (oChuTransf.TransferSubType.Contains("M"))
        //            {  //
        //               //oNewChuMember.ActiveStatus = true;   //deleted                   
        //               //oCurrChuMember.IsCurrent = false;
        //               //oCurrChuMember.Departed = oChuTransf.TransferDate;
        //               //oCurrChuMember.DepartReason = strCTdesc; // GetTransferTypeDesc(oChuTransf.TransferType) + " - " + GetTransferSubTypeDesc(oChuTransf.TransferSubType); // "Transferred";
        //               ////oCurrChuMember.Enrolled = null;  keep as it is
        //               ////oNewChuMember.EnrollReason = "Transferred";   keep as it is
        //               //oCurrChuMember.IsDeceased = false;
        //                oCurrChuMember.LastMod = DateTime.Now;
        //                // 
        //                currCtx.Update(oCurrChuMember);

        //            }

        //            ChurchMember oNewChuMember_Congregant = null;
        //            ChurchMember _oNewChuMember_Congregant = null;
        //            //get the target congregations --oversight responsibility-- for affiliation
        //            if (oChuTransf.AttachedToChurchBodyList.Length > 0)
        //            {
        //                string[] arr = oChuTransf.AttachedToChurchBodyList.Split(',');
        //                foreach (var arrId in arr)
        //                {
        //                    //var oCurrChuMember = currCtx.ChurchMember.Find(int.Parse(arrId));

        //                    // create new member... unique member id = churchBodyId + member id
        //                    var oNewChuMem = new ChurchMember();

        //                    // oNewChuMem = currCtx.ChurchMember.Find(oChuTransf.ChurchMemberId);  //oChuTransf.ChurchMemberTransf;
        //                    // oNewChuMem.Id = 0;
        //                    //oCurrChuMember.MemberGlobalId = oChuTransf.ChurchMemberTransf.MemberGlobalId;
        //                    // oNewChuMem.OwnerChurchBodyId = (int)oChuTransf.ToChurchBodyId;
        //                    // oNewChuMem.ChurchBody = oChuTransf.ToChurchBody; 

        //                    oNewChuMem.MemberGlobalId = oCurrChuMember.MemberGlobalId;
        //                    oNewChuMem.ChurchBodyId = int.Parse(arrId); // (int)oChuTransf.ToChurchBodyId;
        //                    oNewChuMem.AppGlobalOwnerId = oCurrChuMember.AppGlobalOwnerId;
        //                    // oNewChuMem.AppGlobalOwner = oCurrChuMember.AppGlobalOwner;
        //                    oNewChuMem.MemberCode = oCurrChuMember.MemberCode;
        //                    oNewChuMem.Title = oCurrChuMember.Title;
        //                    oNewChuMem.FirstName = oCurrChuMember.FirstName;
        //                    oNewChuMem.MiddleName = oCurrChuMember.MiddleName;
        //                    oNewChuMem.LastName = oCurrChuMember.LastName;
        //                    oNewChuMem.MaidenName = oCurrChuMember.MaidenName;
        //                    oNewChuMem.Gender = oCurrChuMember.Gender;
        //                    oNewChuMem.DateOfBirth = oCurrChuMember.DateOfBirth;
        //                    oNewChuMem.MaritalStatus = oCurrChuMember.MaritalStatus;
        //                    oNewChuMem.MarriageType = oCurrChuMember.MarriageType;
        //                    oNewChuMem.MarriageRegNo = oCurrChuMember.MarriageRegNo;
        //                    oNewChuMem.NationalityId = oCurrChuMember.NationalityId;
        //                    // oNewChuMem.Nationality = oCurrChuMember.MemberCode;
        //                    oNewChuMem.IDTypeId = oCurrChuMember.IDTypeId;
        //                    //oNewChuMem.IDType = oCurrChuMember.IDType;
        //                    oNewChuMem.NationalIDNum = oCurrChuMember.NationalIDNum;
        //                    oNewChuMem.ContactInfoId = oCurrChuMember.ContactInfoId;
        //                    //  oNewChuMem.ContactInfo = oCurrChuMember.ContactInfo;
        //                    oNewChuMem.Hobbies = oCurrChuMember.Hobbies;
        //                    oNewChuMem.Hometown = oCurrChuMember.Hometown;
        //                    oNewChuMem.HometownRegionId = oCurrChuMember.HometownRegionId;
        //                    // oNewChuMem.HometownRegion = oCurrChuMember.HometownRegion;
        //                    oNewChuMem.MotherTongueId = oCurrChuMember.MotherTongueId;
        //                    // oNewChuMem.MotherTongue = oCurrChuMember.MotherTongue;
        //                    oNewChuMem.PhotoUrl = oCurrChuMember.PhotoUrl;
        //                    oNewChuMem.OtherInfo = oCurrChuMember.OtherInfo;
        //                    //  oNewChuMem.EducationLevelId= oCurrChuMember.EducationLevelId; 
        //                    oNewChuMem.IsActivated = true;   //deletion only...flag until clean up or archive
        //                                                     //oNewChuMem.MembershipStatus = int.Parse(arrId)==(int)oChuTransf.ToChurchBodyId ? "C" : "A"; //Congregant  .. A: Affiliate
        //                                                     //
        //                                                     //oNewChuMem.IsCurrent = false;   //requires activation from Target congregation                    
        //                                                     //oNewChuMem.Enrolled = null; //oChuTransf.TransferDate;  //date activated  
        //                                                     //  oNewChuMem.EnrollReason = strCTdesc; // "Transferred";
        //                                                     //oNewChuMem.IsDeceased = false;
        //                                                     //oNewChuMem.Departed = null; // oChuTransf.TransferDate;
        //                                                     //oNewChuMem.DepartReason = null; // "Transferred";
        //                    oNewChuMem.Created = DateTime.Now;
        //                    oNewChuMem.LastMod = DateTime.Now;
        //                    //
        //                    oNewChuMem.MemberClass = "A";//affiliate

        //                    if (oChuTransf.AttachedToChurchBodyId != null)
        //                    {
        //                        if (int.Parse(arrId) == (int)oChuTransf.AttachedToChurchBodyId)
        //                        {
        //                            oNewChuMem.MemberClass = "C";   //congregant 
        //                            _oNewChuMember_Congregant = oNewChuMem;
        //                        }
        //                    }

        //                    oNewChuMember_Congregant = oNewChuMem;
        //                    //
        //                    currCtx.Add(oNewChuMem);
        //                }
        //            }

        //            if (_oNewChuMember_Congregant != null)
        //                oNewChuMember_Congregant = _oNewChuMember_Congregant;

        //            if (oChuTransf.TransferSubType.Contains("M"))
        //            {
        //                //CHECK ON CHURCH-LIFE DETAILS TO SEE IF... ANY MUST BE TRANSFERRED
        //                var oMemCL = currCtx.MemberChurchLife.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id &&
        //                                   c.IsCurrentMember == true).FirstOrDefault();
        //                if (oMemCL == null) return false;   //member must have Pos in the church

        //                var oNewMemCL = new MemberChurchLife();
        //                //  oNewMemCL.ChurchBodyServiceId = null;
        //                oNewMemCL.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //                oNewMemCL.ChurchMemberId = oNewChuMember_Congregant.Id;  // oNewChuMember.Id; // oMemPos.ChurchMemberId;                    
        //                oNewMemCL.Joined = oChuTransf.TransferDate; // DateTime.Now;                                                
        //                oNewMemCL.EnrollReason = strCTdesc; // oChuTransf.TransferType;
        //                oNewMemCL.IsCurrentMember = true;
        //                // oNewMemCL.Departed = null;
        //                // oNewMemCL.DepartReason = null;
        //                oNewMemCL.IsDeceased = false;
        //                oNewMemCL.IsMemBaptized = oMemCL.IsMemBaptized;
        //                oNewMemCL.IsMemConfirmed = oMemCL.IsMemConfirmed;
        //                oNewMemCL.IsMemCommunicant = oMemCL.IsMemCommunicant;
        //                oNewMemCL.NonCommReason = oMemCL.NonCommReason;
        //                //oNewMemCL.IsPioneer  = oMemCL.IsPioneer;
        //                oNewMemCL.Created = DateTime.Now;
        //                oNewMemCL.LastMod = DateTime.Now;

        //                currCtx.Add(oNewMemCL);

        //                //update the old to past ...  
        //                oMemCL.IsCurrentMember = false; //past             ... WORK  ON THIS CODE LATER
        //                oMemCL.Departed = oChuTransf.TransferDate; // oNewMemPos.Until;
        //                oMemCL.DepartReason = strCTdesc; // oChuTransf.TransferType;
        //                oMemCL.LastMod = DateTime.Now;
        //                //
        //                currCtx.Update(oMemCL);


        //                //update church Pos/position... put new member on lowest Pos... except clergy transfer
        //                var oMemPos = currCtx.MemberPosition.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id &&
        //                                    c.CurrentPos == true).FirstOrDefault();
        //                if (oMemPos == null) return false;   //member must have Pos in the church

        //                //get new status... from the old   
        //                //var oNewMemPos = new MemberPosition();
        //                //if (oChuTransf.TransferType == "CT")  ...obvious
        //                //{  //Pos unchanged
        //                // oNewMemPos.ChurchPositionId = oMemPos.ChurchPositionId;
        //                //}
        //                //else if (oChuTransf.TransferType == "MT")
        //                //{  // put member in lowest position
        //                //    var oMinPos = _context.ChurchPosition.Where(x => x.AuthorityIndex ==
        //                //                    _context.ChurchPosition.Where(y => y.ChurchBody.AppGlobalOwnerId == oChuTransf.ToChurchBody.AppGlobalOwnerId)
        //                //                    .Max(y => y.AuthorityIndex)).Take(1).FirstOrDefault();
        //                //    if (oMinPos == null) return false;
        //                //    oNewMemPos.ChurchPositionId = oMinPos.Id;
        //                //}

        //                //
        //                var oNewMemPos = new MemberPosition();
        //                oNewMemPos.ChurchPositionId = oMemPos.ChurchPositionId;
        //                oNewMemPos.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //                oNewMemPos.ChurchMemberId = oNewChuMember_Congregant.Id;  // oNewChuMember.Id; // oMemPos.ChurchMemberId;                    
        //                oNewMemPos.DateAssigned = oChuTransf.TransferDate; // DateTime.Now;
        //                oNewMemPos.Until = null;
        //                oNewMemPos.Comments = strCTdesc; // oChuTransf.TransferType;
        //                oNewMemPos.CurrentPos = true;
        //                oNewMemPos.Created = DateTime.Now;
        //                oNewMemPos.LastMod = DateTime.Now;

        //                //
        //                currCtx.Add(oNewMemPos);

        //                //update the old to past ...  // var oMemPos = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
        //                oMemPos.CurrentPos = false; //past             ... WORK  ON THIS CODE LATER
        //                oMemPos.Until = oChuTransf.TransferDate; // oNewMemPos.Until;
        //                oMemPos.Comments = strCTdesc; // oChuTransf.TransferType;
        //                oMemPos.LastMod = DateTime.Now;
        //                //
        //                currCtx.Update(oMemPos);


        //                //update the old to past
        //                var oMemFr_Status = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrent == true).FirstOrDefault();
        //                if (oMemFr_Status == null) return false;

        //                //get new status... from the old
        //                var oNewFr_MemStatus = new MemberStatus();   // oNewMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault(); // oMemStatus;
        //                                                             //  oNewMemStatus.Id = 0;
        //                oNewFr_MemStatus.ChurchBodyId = oMemFr_Status.ChurchBodyId;
        //                oNewFr_MemStatus.ChurchMemberId = oMemFr_Status.ChurchMemberId; // oMemStatus.ChurchMemberId;
        //                oNewFr_MemStatus.ChurchMemStatusId = 4; // Past... oMemStatus.ChurchMemStatusId;   ... WORK  ON THIS CODE LATER
        //                oNewFr_MemStatus.Since = oChuTransf.TransferDate; // DateTime.Now;
        //                oNewFr_MemStatus.IsCurrent = true;
        //                //oNewFr_MemStatus.Until = oChuTransf.TransferDate;
        //                oNewFr_MemStatus.Comments = oChuTransf.TransferType;
        //                oNewFr_MemStatus.Created = DateTime.Now;
        //                oNewFr_MemStatus.LastMod = DateTime.Now;
        //                //
        //                currCtx.Add(oNewFr_MemStatus);

        //                //update the old to past ...  // var oMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
        //                // oMemStatus.ChurchMemStatusId = 4; //past             ... WORK  ON THIS CODE LATER
        //                oMemFr_Status.Until = oChuTransf.TransferDate;
        //                oMemFr_Status.Comments = oChuTransf.TransferType;
        //                oMemFr_Status.IsCurrent = false;
        //                oMemFr_Status.LastMod = DateTime.Now;
        //                //
        //                currCtx.Update(oMemFr_Status);

        //                //get new status... from the old
        //                var oNewTo_MemStatus = new MemberStatus();   // oNewMemStatus = currCtx.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault(); // oMemStatus;
        //                                                             //  oNewMemStatus.Id = 0;
        //                oNewTo_MemStatus.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //                oNewTo_MemStatus.ChurchMemberId = oNewChuMember_Congregant.Id; // oMemStatus.ChurchMemberId;
        //                oNewTo_MemStatus.ChurchMemStatusId = 1; // Regular... oMemStatus.ChurchMemStatusId;   ... WORK  ON THIS CODE LATER
        //                oNewTo_MemStatus.Since = oChuTransf.TransferDate; // DateTime.Now;
        //                oNewTo_MemStatus.Until = null;
        //                oNewTo_MemStatus.Comments = oChuTransf.TransferType;
        //                oNewTo_MemStatus.IsCurrent = true;
        //                oNewTo_MemStatus.Created = DateTime.Now;
        //                oNewTo_MemStatus.LastMod = DateTime.Now;
        //                //
        //                currCtx.Add(oNewTo_MemStatus);


        //                ////sectors... 
        //                var oMemSectors = currCtx.MemberChurchSector.Where(c => c.Sector.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.CurrSector == true).ToList();
        //                foreach (var oMS in oMemSectors)
        //                {
        //                    oMS.CurrSector = false;
        //                    oMS.Departed = oChuTransf.TransferDate;
        //                    oMS.LastMod = DateTime.Now;
        //                    //
        //                    currCtx.Update(oMS);
        //                }


        //                //... roles
        //                var oMemRoles = currCtx.MemberLeaderRole.Include(t => t.LeaderRole).ThenInclude(t => t.LeaderRoleCategory)
        //                    .Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.IsCurrServing == true).ToList();
        //                foreach (var oMR in oMemRoles)
        //                {
        //                    oMR.IsCurrServing = false;
        //                    oMR.Completed = oChuTransf.TransferDate;
        //                    oMR.CompletionReason = strCTdesc; // oChuTransf.TransferType;
        //                    oMR.LastMod = DateTime.Now;
        //                    //
        //                    currCtx.Update(oMR);

        //                    //if (oChuTransf.TransferType == "CT")
        //                    //{
        //                    //    var oMinRoleList = oMemRoles.Where(c => c.LeaderRole?.LeaderRoleCategory?.CoreRole == true && c.LeaderRole?.LeaderRoleCategory?.OrdainedRole == true);
        //                    //    var oMinRoleSectorList = currCtx.ChurchSector.Include(t => t.ChurchSectorCategory).Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.Status == "A" && c.ChurchSectorCategory.IsMainstream == true).ToList();
        //                    //    //
        //                    //    if (oMinRoleList.Count() == 0 || oMinRoleSectorList.Count() == 0) return false;
        //                    //    var oMinRole = oMinRoleList.FirstOrDefault();
        //                    //    var oMinRoleSector = oMinRoleSectorList.FirstOrDefault();
        //                    //    //foreach (var oMinRole in oMinRoleList)
        //                    //    //{
        //                    //    //get new status... from the old
        //                    //    var oNewMemRole = new MemberLeaderRole();
        //                    //    oNewMemRole.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //                    //    oNewMemRole.ChurchMemberId = oNewChuMember_Congregant.Id; // oMemRole.ChurchMemberId;
        //                    //    oNewMemRole.LeaderRoleId = oMinRole.LeaderRoleId;
        //                    //    oNewMemRole.RoleSectorId = oMinRoleSector.Id;

        //                    //    oNewMemRole.Commenced = oChuTransf.TransferDate;
        //                    //    oNewMemRole.Completed = null;
        //                    //    oNewMemRole.CompletionReason = oChuTransf.TransferType;
        //                    //    oNewMemRole.CurrServing = true;
        //                    //    oNewMemRole.Created = DateTime.Now;
        //                    //    oNewMemRole.LastMod = DateTime.Now;
        //                    //    //
        //                    //    currCtx.Add(oNewMemRole);
        //                    //    //}
        //                    //}
        //                }
        //            }

        //            //CT move with indicated roles 
        //            if (oChuTransf.TransferSubType.Contains("R"))
        //            {
        //                if (oChuTransf.DesigRolesList.Length > 0)
        //                {
        //                    string[] _arr = oChuTransf.DesigRolesList.Split(',');
        //                    if (_arr.Length > 0)
        //                    {
        //                        foreach (var _arrId in _arr)
        //                        {
        //                            string[] arr = _arrId.Split('|');  //ChurchBody | ChurchSector | LeaderRole
        //                            if (arr.Length > 2) //3 parts
        //                            {
        //                                var oChurchBodyId = int.Parse(arr[0]);
        //                                var oChurchSectorId = int.Parse(arr[1]);
        //                                var oLeaderRoleId = int.Parse(arr[2]);

        //                                //var oMinRoleList = oMemRoles.Where(c => c.LeaderRole?.LeaderRoleCategory?.CoreRole == true && c.LeaderRole?.LeaderRoleCategory?.OrdainedRole == true);
        //                                //var oMinRoleSectorList = currCtx.ChurchSector.Include(t => t.ChurchSectorCategory).Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.Status == "A" && c.ChurchSectorCategory.IsMainstream == true).ToList();
        //                                ////
        //                                //if (oMinRoleList.Count() == 0 || oMinRoleSectorList.Count() == 0) return false;
        //                                //var oMinRole = oMinRoleList.FirstOrDefault();
        //                                //var oMinRoleSector = oMinRoleSectorList.FirstOrDefault();
        //                                //foreach (var oMinRole in oMinRoleList)
        //                                //{

        //                                //get new status... from the old
        //                                var oNewMemRole = new MemberLeaderRole();
        //                                oNewMemRole.ChurchBodyId = oChurchBodyId; // oChuTransf.ToChurchBodyId;
        //                                oNewMemRole.ChurchMemberId = oNewChuMember_Congregant.Id; // oMemRole.ChurchMemberId;
        //                                oNewMemRole.LeaderRoleId = oLeaderRoleId; // oMinRole.LeaderRoleId;
        //                                oNewMemRole.ChurchUnitId = oChurchSectorId; // oMinRoleSector.Id;

        //                                oNewMemRole.Commenced = oChuTransf.TransferDate;
        //                                oNewMemRole.Completed = null;
        //                                oNewMemRole.CompletionReason = strCTdesc; // oChuTransf.TransferType;
        //                                oNewMemRole.IsCurrServing = true;
        //                                oNewMemRole.Created = DateTime.Now;
        //                                oNewMemRole.LastMod = DateTime.Now;
        //                                //
        //                                currCtx.Add(oNewMemRole);
        //                                //}
        //                            }
        //                        }
        //                    }
        //                }
        //            }



        //            if (oChuTransf.TransferSubType.Contains("M"))
        //            {
        //                //user profiles... deactivate
        //                var oUserProfile = currCtx.UserProfile.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.UserStatus == "A").ToList();
        //                foreach (var oUP in oUserProfile)
        //                {
        //                    //user profile group... deactivate
        //                    var oUserProfileGroup = currCtx.UserProfileGroup.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.UserProfileId == oUP.Id && c.Status == "A").ToList();
        //                    foreach (var oUPG in oUserProfileGroup)
        //                    {
        //                        oUPG.Status = "D";
        //                        oUPG.LastMod = DateTime.Now;
        //                        //
        //                        currCtx.Update(oUPG);
        //                    }

        //                    //user profile group... deactivate
        //                    var oUserProfileRole = currCtx.UserProfileRole.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.UserProfileId == oUP.Id && c.ProfileRoleStatus == "A").ToList();
        //                    foreach (var oUPR in oUserProfileRole)
        //                    {
        //                        oUPR.ProfileRoleStatus = "D";
        //                        oUPR.LastMod = DateTime.Now;
        //                        //
        //                        currCtx.Update(oUPR);
        //                    }

        //                    //finally the master...
        //                    oUP.UserStatus = "D";
        //                    oUP.LastMod = DateTime.Now;
        //                    //
        //                    currCtx.Update(oUP);
        //                }
        //            }

        //        }

        //        // oChuTransf.ReqStatus = "C";     //Closed... member transferred
        //        // oChuTransf.ApprovalStatus = "A";   // Assuming all the necessay steps/action steps have been approved.

        //        return true;
        //    }

        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}


        //[HttpPost]
        //public async Task<IActionResult> CloseTransferRequest(int? oCurrChuBodyId, int? oChurchTransferId)
        //{
        //    var oChuTransf = _context.ChurchTransfer.Where(c => c.FromChurchBodyId == oCurrChuBodyId && c.Id == oChurchTransferId).FirstOrDefault();  //.Find(oChurchTransferId);
        //    if (oChuTransf != null)
        //    {
        //        try
        //        {
        //            _context.Update(oChuTransf);
        //            oChuTransf.ReqStatus = "C";

        //            //Save the request
        //            await _context.SaveChangesAsync();

        //            var oCB = _context.ChurchBody.Find(oChuTransf.ToChurchBodyId);
        //            //  SendSMSNotification(oChuTransf.ChurchMemberTransf, "Please your request for transfer to " + oCB?.Name + " has been received and being processed. Thank you");
        //            return Json(true);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(false);
        //        }


        //        //if (oChuTransf.Status == "P") // && oChuTransf.RequireApproval == false)
        //        //{
        //        //    oChuTransf.Status = "R";

        //        //    if (oChuTransf.RequireApproval == false ||
        //        //        (oChuTransf.RequireApproval == true && oChuTransf.IApprovalAction?.Status == 'A'))
        //        //    {

        //        //        //make member past member of from cong... add new member @ To cong
        //        //        var oCurrChuMember = _context.ChurchMember.Find(oChuTransf.ChurchMemberId);
        //        //        if (oCurrChuMember != null)
        //        //        {
        //        //            oChuTransf.TransferDate = DateTime.Now;

        //        //            // create new member... unique member id = churchBodyId + member id
        //        //            var oNewChuMember = new ChurchMember();
        //        //            // oNewChuMember = _context.ChurchMember.Find(oChuTransf.ChurchMemberId);  //oChuTransf.ChurchMemberTransf;
        //        //            // oNewChuMember.Id = 0;
        //        //            oNewChuMember.OwnerChurchBodyId = (int)oChuTransf.ToChurchBodyId;
        //        //            // oNewChuMember.ChurchBody = oChuTransf.ToChurchBody;
        //        //            oNewChuMember.ChurchBodyId = (int)oChuTransf.ToChurchBodyId;
        //        //            oNewChuMember.AppGlobalOwnerId = oCurrChuMember.AppGlobalOwnerId;
        //        //            // oNewChuMember.AppGlobalOwner = oCurrChuMember.AppGlobalOwner;
        //        //            oNewChuMember.MemberCode = oCurrChuMember.MemberCode;
        //        //            oNewChuMember.Title = oCurrChuMember.Title;
        //        //            oNewChuMember.FirstName = oCurrChuMember.FirstName;
        //        //            oNewChuMember.MiddleName = oCurrChuMember.MiddleName;
        //        //            oNewChuMember.LastName = oCurrChuMember.LastName;
        //        //            oNewChuMember.MaidenName = oCurrChuMember.MaidenName;
        //        //            oNewChuMember.Gender = oCurrChuMember.Gender;
        //        //            oNewChuMember.DateOfBirth = oCurrChuMember.DateOfBirth;
        //        //            oNewChuMember.MaritalStatus = oCurrChuMember.MaritalStatus;
        //        //            oNewChuMember.MarriageType = oCurrChuMember.MarriageType;
        //        //            oNewChuMember.NationalityId = oCurrChuMember.NationalityId;
        //        //            // oNewChuMember.Nationality = oCurrChuMember.MemberCode;
        //        //            oNewChuMember.IDTypeId = oCurrChuMember.IDTypeId;
        //        //            //oNewChuMember.IDType = oCurrChuMember.IDType;
        //        //            oNewChuMember.NationalIDNum = oCurrChuMember.NationalIDNum;
        //        //            oNewChuMember.ContactInfoId = oCurrChuMember.ContactInfoId;
        //        //            //  oNewChuMember.ContactInfo = oCurrChuMember.ContactInfo;
        //        //            oNewChuMember.Hobbies = oCurrChuMember.Hobbies;
        //        //            oNewChuMember.Hometown = oCurrChuMember.Hometown;
        //        //            oNewChuMember.HometownRegionId = oCurrChuMember.HometownRegionId;
        //        //            // oNewChuMember.HometownRegion = oCurrChuMember.HometownRegion;
        //        //            oNewChuMember.MotherTongueId = oCurrChuMember.MotherTongueId;
        //        //            // oNewChuMember.MotherTongue = oCurrChuMember.MotherTongue;
        //        //            oNewChuMember.PhotoUrl = oCurrChuMember.PhotoUrl;
        //        //            oNewChuMember.OtherInfo = oCurrChuMember.OtherInfo;
        //        //            oNewChuMember.ActiveStatus = true;
        //        //            oNewChuMember.Created = DateTime.Now;
        //        //            oNewChuMember.LastMod = DateTime.Now;
        //        //            //
        //        //            _context.Add(oNewChuMember);


        //        //            //update the old to past
        //        //            var oMemStatus = _context.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();

        //        //            //get new status... from the old
        //        //            var oNewMemStatus = new MemberStatus();
        //        //            // oNewMemStatus = _context.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault(); // oMemStatus;
        //        //            //  oNewMemStatus.Id = 0;
        //        //            oNewMemStatus.ChurchBodyId = oChuTransf.ToChurchBodyId;
        //        //            oNewMemStatus.ChurchMemberId = oNewChuMember.Id; // oMemStatus.ChurchMemberId;
        //        //            oNewMemStatus.ChurchMemStatusId = oMemStatus.ChurchMemStatusId;
        //        //            oNewMemStatus.Since = DateTime.Now;
        //        //            oNewMemStatus.Until = null;
        //        //            oNewMemStatus.Reason = oChuTransf.TransferType;
        //        //            oNewMemStatus.ActiveStatus = true;
        //        //            oNewMemStatus.Created = DateTime.Now;
        //        //            oNewMemStatus.LastMod = DateTime.Now;

        //        //            _context.Add(oNewMemStatus);

        //        //            ////update the old to past
        //        //            // var oMemStatus = _context.MemberStatus.Where(c => c.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.ActiveStatus == true).FirstOrDefault();
        //        //            oMemStatus.ChurchMemStatusId = 4; //past
        //        //            oMemStatus.Reason = oChuTransf.TransferType;
        //        //            _context.Update(oMemStatus);

        //        //            ////sectors... 
        //        //            var oMemSectors = _context.MemberChurchSector.Where(c => c.Sector.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.CurrSector == true).ToList();
        //        //            foreach (var oMS in oMemSectors)
        //        //            {
        //        //                oMS.CurrSector = false;
        //        //                oMS.Departed = oChuTransf.TransferDate;

        //        //                _context.Update(oMS);
        //        //            }

        //        //            //... roles
        //        //            var oMemRoles = _context.MemberLeaderRole.Where(c => c.LeaderRole.ChurchBodyId == oChuTransf.FromChurchBodyId && c.ChurchMemberId == oCurrChuMember.Id && c.CurrServing == true).ToList();
        //        //            foreach (var oMR in oMemRoles)
        //        //            {
        //        //                oMR.CurrServing = false;
        //        //                oMR.Completed = oChuTransf.TransferDate;
        //        //                oMR.CompletionReason = oChuTransf.TransferType;

        //        //                _context.Update(oMR);
        //        //            }
        //        //        }
        //        //    }
        //        //}


        //    }
        //    else
        //    {
        //        return Json(false);
        //    }
        //}

        //private void TriggerApprovalRequest(ChurchTransfer oChuTransf)
        //{
        //    //Upon Receive /Prending... trigger Approval Process else go ahead and transfer the person

        //    //if RequireApproval == true ... trigger the Approval Process  ... ie. new request
        //    if (oChuTransf.Id == 0 && oChuTransf.RequireApproval == true)
        //    {


        //    }

        //    //existing, approval req, yet not triggered
        //    else if (oChuTransf.Id > 0 && oChuTransf.RequireApproval == true && oChuTransf.IApprovalActionId == null)
        //    {

        //    }
        //}







    }
}
