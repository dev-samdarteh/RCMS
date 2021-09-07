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
    public class ChurchCalendarController : Controller
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
          
        private List<DiscreteLookup> dlThemeColor = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlActivityTypes = new List<DiscreteLookup>();

        private readonly IConfiguration _configuration;
        private string _clientDBConn;

        public ChurchCalendarController(MSTR_DbContext masterContext, IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, ChurchModelContext clientCtx,
            IConfiguration configuration)
        {
            try
            {
                _hostingEnvironment = hostingEnvironment;
                _masterContext = masterContext;
                _configuration = configuration;


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

                this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));


                // _context = context;
                //  this._context = clientCtx;
                if (clientCtx == null)
                    _context = GetClientDBContext();

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





            //color pallette will be used actually... to pick
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Red".ToLower(), Desc = "Red" }); //1  -- [i - 1] * 4  -- [i * 4] - 1
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Blue".ToLower(), Desc = "Blue" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Green".ToLower(), Desc = "Green" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Yellow".ToLower(), Desc = "Yellow" });

            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Lime".ToLower(), Desc = "Lime" });  //5
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "LightYellow".ToLower(), Desc = "LightYellow" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Cyan".ToLower(), Desc = "Cyan" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Grey".ToLower(), Desc = "Grey" });

            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Whitesmoke".ToLower(), Desc = "Whitesmoke" }); //9
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "DeepSkyBlue".ToLower(), Desc = "DeepSkyBlue" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "LightSkyBlue".ToLower(), Desc = "LightSkyBlue" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "LightBlue".ToLower(), Desc = "LightBlue" });

            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Tomato".ToLower(), Desc = "Tomato" }); //15
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Coral".ToLower(), Desc = "Coral" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Brown".ToLower(), Desc = "Brown" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Brown".ToLower(), Desc = "DimGrey" });

            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Violet".ToLower(), Desc = "Violet" });//19
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "Pink".ToLower(), Desc = "Maroon" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "DeepPink".ToLower(), Desc = "Pink" });
            dlThemeColor.Add(new DiscreteLookup() { Category = "ThemeColor", Val = "DeepPink".ToLower(), Desc = "Fuchsia" });


            //GA-- General activ, ER-Event Role,  MC--Member Churchlife Activity related, EV-Church Event related
            dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_GA", Desc = "General Activity" });
            dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_EV", Desc = "Event or Program" });
            // dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_ER", Desc = "Event Role" });
            dlActivityTypes.Add(new DiscreteLookup() { Category = "ActvType", Val = "CLA_MR", Desc = "Member related" });


            //SharingStatus { get; set; }  // A - Share with all sub-congregations, C - Share with child congregations only, N - Do not share
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "N", Desc = "Do not roll-down (share)" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "C", Desc = "Roll-down (share) for direct child congregations" });
            dlShareStatus.Add(new DiscreteLookup() { Category = "ShrStat", Val = "A", Desc = "Roll-down (share) for all sub-congregations" });


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
             
             

 
        }




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

        private string GetCL_DBConnString()
        {
            var isAuth = this.oUserLogIn_Priv != null;
            if (!isAuth) isAuth = SetUserLogged();

            if (!isAuth)
                RedirectToAction("LoginUserAcc", "UserLogin");

            if (this.oUserLogIn_Priv == null)
                RedirectToAction("LoginUserAcc", "UserLogin");

            if (this.oUserLogIn_Priv.UserProfile == null)
                RedirectToAction("LoginUserAcc", "UserLogin");


            if (string.IsNullOrEmpty(this._clientDBConn))
                this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));

            var _clientContext = new ChurchModelContext(this._clientDBConn);
            if (_clientContext.Database.CanConnect()) return this._clientDBConn;
            else
            {
                this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                _clientContext = new ChurchModelContext(this._clientDBConn);
                if (_clientContext.Database.CanConnect()) return this._clientDBConn;
                else
                {
                    var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                    if (oClientConfig != null)
                    {
                        var _cs = _configuration.GetConnectionString("DefaultConnection");
                        // get and mod the conn                        
                        var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
                        conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
                        conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
                        /// conn.IntegratedSecurity = false; 
                        conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

                        this._clientDBConn = conn.ConnectionString;

                        _clientContext = new ChurchModelContext(this._clientDBConn);
                        if (_clientContext.Database.CanConnect()) return this._clientDBConn;
                        else
                        { return null; }
                    }
                    else
                    { return null; }
                }
            }
        }

        private ChurchModelContext GetClientDBContext()  ///(UserProfile oUserLogged)
        {
            var isAuth = this.oUserLogIn_Priv != null;
            if (!isAuth) isAuth = SetUserLogged();

            if (!isAuth)
                RedirectToAction("LoginUserAcc", "UserLogin");

            if (this.oUserLogIn_Priv == null)
                RedirectToAction("LoginUserAcc", "UserLogin");

            if (this.oUserLogIn_Priv.UserProfile == null)
                RedirectToAction("LoginUserAcc", "UserLogin");


            if (string.IsNullOrEmpty(this._clientDBConn))
                this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));

            var _clientContext = new ChurchModelContext(this._clientDBConn);
            if (_clientContext.Database.CanConnect()) return _clientContext;
            else
            {
                this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                _clientContext = new ChurchModelContext(this._clientDBConn);
                if (_clientContext.Database.CanConnect()) return _clientContext;
                else
                {
                    var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                    if (oClientConfig != null)
                    {
                        var _cs = _configuration.GetConnectionString("DefaultConnection");
                        // get and mod the conn                        
                        var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
                        conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
                        conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
                        /// conn.IntegratedSecurity = false; 
                        conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

                        this._clientDBConn = conn.ConnectionString;

                        _clientContext = new ChurchModelContext(this._clientDBConn);
                        if (_clientContext.Database.CanConnect()) return _clientContext;
                        else
                        { return null; }
                    }
                    else
                    { return null; }
                }
            }

            //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == oUserLogged.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
            //if (oClientConfig != null)
            //{
            //    //// get and mod the conn
            //    //var _clientDBConnString = "";
            //    //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
            //    //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
            //    //_clientDBConnString = conn.ConnectionString;

            //    //// test the NEW DB conn
            //    //var _clientContext = new ChurchModelContext(_clientDBConnString);

            //    //var _clientDBConnString = "";
            //    var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
            //    conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
            //    conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
            //    /// conn.IntegratedSecurity = false; 
            //    conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

            //    this._clientDBConnString = conn.ConnectionString;

            //    // test the NEW DB conn
            //    var _clientContext = new ChurchModelContext(this._clientDBConnString);

            //    if (_clientContext.Database.CanConnect())
            //        return _clientContext;
            //}

            //
            // return null;
        }


        private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail, MSTR_DbContext currContext = null, string strTempConn = "")
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // MSTR_DbContext currContext = null, string strTempConn = ""
                var _cs = strTempConn;
                if (string.IsNullOrEmpty(_cs))
                    _cs = this._configuration.GetConnectionString("DefaultConnection"); //["ConnectionStrings:DefaultConnection"]; /// _masterContext.Database.GetDbConnection().ConnectionString

                // var tempCtx = _context;
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

        private async Task LogUserActivity_ClientUserAuditTrail(UserAuditTrail_CL oUserTrail, ChurchModelContext currContext = null, string strTempConn = "")
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // var tempCtx = _context;
                //  if (!string.IsNullOrEmpty(clientDBConnString))
                // {

                //// MSTR_DbContext currContext = null, string strTempConn = ""
                //var _cs = strTempConn;
                //if (string.IsNullOrEmpty(_cs))
                //    _cs = this._configuration["ConnectionStrings:DefaultConnection"];

                using (var logCtx = new ChurchModelContext(strTempConn)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
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
                //   }
            }
        }



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

        //private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail)
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail != null)
        //    {
        //        // var tempCtx = _context;
        //        using (var logCtx = new MSTR_DbContext(_masterContext.Database.GetDbConnection().ConnectionString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
        //        {
        //            if (logCtx.Database.CanConnect() == false) logCtx.Database.OpenConnection();
        //            else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open) logCtx.Database.OpenConnection();

        //            // var a = logCtx.Database.GetDbConnection().ConnectionString;
        //            // var b = _masterContext.Database.GetDbConnection().ConnectionString;

        //            /// 
        //            logCtx.UserAuditTrail.Add(oUserTrail);
        //            await logCtx.SaveChangesAsync();

        //            //logCtx.SaveChanges();

        //            logCtx.Entry(oUserTrail).State = EntityState.Detached;
        //            ///
        //            //DetachAllEntities(logCtx);

        //            // close connection
        //            logCtx.Database.CloseConnection();

        //            //logCtx.Dispose();
        //        }
        //    }
        //}
        //private async Task LogUserActivity_ClientUserAuditTrail(UserAuditTrail_CL oUserTrail, string clientDBConnString)
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail != null)
        //    {
        //        // var tempCtx = _context;
        //        if (!string.IsNullOrEmpty(clientDBConnString))
        //        {
        //            using (var logCtx = new ChurchModelContext(clientDBConnString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
        //            {
        //                //logCtx = _context;
        //                //var conn = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_context.Database.GetDbConnection().ConnectionString);
        //                ////  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
        //                //conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //                /////
        //                //logCtx.Database.GetDbConnection().ConnectionString = conn.ConnectionString;

        //                if (logCtx.Database.CanConnect() == false) logCtx.Database.OpenConnection();
        //                else if (logCtx.Database.GetDbConnection().State != System.Data.ConnectionState.Open) logCtx.Database.OpenConnection();

        //                // var a = logCtx.Database.GetDbConnection().ConnectionString;
        //                // var b = _masterContext.Database.GetDbConnection().ConnectionString;

        //                ///
        //                logCtx.UserAuditTrail_CL.Add(oUserTrail);
        //                await logCtx.SaveChangesAsync();

        //                //logCtx.SaveChanges();

        //                logCtx.Entry(oUserTrail).State = EntityState.Detached;
        //                ///
        //                //DetachAllEntities(logCtx);

        //                // close connection
        //                logCtx.Database.CloseConnection();

        //                //logCtx.Dispose();
        //            }
        //        }
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

        
        public IActionResult Index(int currAttendTask = 1, int? reqChurchBodyId = null)
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

            var loadLim = false;
            if (!loadLim)
                _ = this.LoadClientDashboardValues(this._clientDBConnString);

            var oAppGloOwnId = this._oLoggedAGO.Id;
            var oCurrChuBodyId = reqChurchBodyId;

            //if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            //if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

            ChurchBody oCB = this._oLoggedCB;
            if (oCurrChuBodyId != this._oLoggedCB.Id)
                oCB = _context.ChurchBody.AsNoTracking().Include(t=>t.AppGlobalOwner).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.Id == oCurrChuBodyId).FirstOrDefault();


            //var UserLogIn_Priv = TempData.Get<List<UserSessionPrivilege>>("UserLogIn_oUserPrivCol");
            //var checkUser = UserLogIn_Priv?.Count > 0;

            var oCurrChuBody = oCB; // oUserLogIn_Priv[0].ChurchBody;
            var oRequestedChurchBody = oCB;

               // if (oCurrChuBody == null) return View();

                //ViewBag.strChuBodyLogged = oCurrChuBody.AppGlobalOwner.OwnerName.ToUpper() + " - " + oCurrChuBody.Name;
                ViewBag.strChuBodyLogged = this._oLoggedAGO.OwnerName.ToUpper() + " - " + oCurrChuBody.Name;

                //ChurchBody oRequestedChurchBody;
                //if (reqChurchBodyId > 0)  // != null)
                //    oRequestedChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.Id == reqChurchBodyId).FirstOrDefault();
                //else
                //{ reqChurchBodyId = oCurrChuBody.Id; oRequestedChurchBody = oCurrChuBody; }

                // check permission for Core life...
              //  if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) return View();

               //var oAttendVM = new ChurchCalendarModel();

                //if (currAttendTask == 1)
                //{
                //    oAttendVM.strCurrTaskDesc = oAttendVM.strAttendeeTypeCode == "M" ? oAttendVM.strAttnLongevity == "H" ? "Member Attendance Headcount -- History" : "Member Attendance Headcount (Today)" :
                //                         oAttendVM.strAttendeeTypeCode == "V" ? oAttendVM.strAttnLongevity == "H" ? "Visitors Headcount History (Attendance)" : "Visitor Attendance Headcount (Today)" :
                //                                                      oAttendVM.strAttnLongevity == "H" ? "Church Attendance Headcount -- History" : "Church Attendance Headcount(Today)";  // isMigrated == true ? "New Converts" : "New Coverts (Historic)" : isMigrated == true ? "Visitors" : "Visitors (Historic)";
                //}
                //else
                //{
                //    oAttendVM.strCurrTaskDesc = "Church Attendance -- Headcount";
                //}


                var oCCEModel = new ChurchCalendarModel();

                //  // List<ChurchCalendarModel> qry = new List<ChurchCalendarModel>();
                //  //qry = ReturnMemberProfileList();


                //  // var qry = ReturnMemberProfileList();

                //oCCEModel.oCalendarEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity).Include(t => t.ChurchBody).Include(t => t.OwnedByChurchBody)
                //    // .Include(c => c.ChurchBody).Include(c => c.ChurchEventCategory).Include(c => c.OwnedByChurchBody)
                //    .Where(c =>
                //                    (c.OwnedByChurchBodyId == oCurrChuBody.Id ||
                //                    (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                //                    (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
                //           )
                //    //.OrderByDescending(c=>c.EventActive)
                //    .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo).ThenBy(c => c.Subject) //.ThenByDescending(c=>c.(DateTime)EventTo)
                //    .ToList();


            var oCCE_List_All = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP).Include(t => t.ChurchBody)
                                        .ToList();

            oCCE_List_All = oCCE_List_All.Where(c =>
                                   (c.ChurchBodyId == this._oLoggedCB.Id ||
                                   (c.ChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.ChurchBodyId == _oLoggedCB.ParentChurchBodyId) ||
                                   (c.ChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.ChurchBody, this._oLoggedCB))))
                                .ToList();

            if (oCCE_List_All.Count > 0)
                oCCE_List_All = oCCE_List_All
                   //.OrderByDescending(c=>c.EventActive)
                   .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo).ThenBy(c => c.Subject) //.ThenByDescending(c=>c.(DateTime)EventTo)
                   .ToList();

            oCCEModel.oCalendarEvents = oCCE_List_All;

            long gloCount, parCount, locCount, uncatCount;
                gloCount = 0; parCount = 0; locCount = 0; uncatCount = 0;
                var oActiveList = oCCEModel.oCalendarEvents.Where(c => c.IsEventActive == true).ToList();
                foreach (var oEv in oActiveList)
                {
                    if (oEv.ChurchBodyId == oEv.ChurchBodyId) locCount++;
                    else if (oEv.ChurchBodyId == oEv.ChurchBody.ParentChurchBodyId) parCount++;
                    else if (IsAncestor_ChurchBody(oEv.ChurchBody, oEv.ChurchBody) == true) gloCount++; //  (oEv.OwnedByChurchBodyId == oEv.ChurchBodyId) 
                    else uncatCount++;
                }

                oCCEModel.ActivityCount_Local = locCount;
                oCCEModel.ActivityCount_Parent = parCount;
                oCCEModel.ActivityCount_Global = gloCount;
                oCCEModel.ActivityCount_Uncat = uncatCount;

                //oCurrChuBody ... church level names  ... this code must be part of login info.. load once...
                oCCEModel.strActivityCount_Local = "Global Church";
                oCCEModel.strActivityCount_Parent = "Parent Congregation";
                oCCEModel.strActivityCount_Global = "Local Congregation";

                if (oCurrChuBody.ChurchLevel != null)
                    oCCEModel.strActivityCount_Local = !string.IsNullOrEmpty(oCurrChuBody.ChurchLevel.CustomName) ? oCurrChuBody.ChurchLevel.CustomName : oCurrChuBody.ChurchLevel.Name;

                var oCBParent = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.Id == oCurrChuBody.ParentChurchBodyId).FirstOrDefault();
                if (oCBParent.ChurchLevel != null)
                    oCCEModel.strActivityCount_Parent = !string.IsNullOrEmpty(oCBParent.ChurchLevel.CustomName) ? oCBParent.ChurchLevel.CustomName : oCBParent.ChurchLevel.Name;

                var lowestCB = _context.ChurchLevel.Where(x => x.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && x.LevelIndex == _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId).Min(y => y.LevelIndex)).FirstOrDefault();
                if (lowestCB != null)
                {
                    var oCBGlobal = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.ChurchLevelId == lowestCB.Id
                                        ).FirstOrDefault();
                    oCCEModel.strActivityCount_Global = !string.IsNullOrEmpty(oCBGlobal.ChurchLevel.CustomName) ? oCBGlobal.ChurchLevel.CustomName : oCBGlobal.ChurchLevel.Name;
                }


                //  var qryListVM = new List<ChurchCalendarModel>();
                //  qryListVM.Add(oCCEModel);



                //  //if (!(filterIndex == null || filterVal == null))
                //  //    qry = GetMemberProfiles(qry, filterIndex, filterVal);

                //  var oCalendarEvents = AppUtilties_Static.ToListAsync<ChurchCalendarModel>(qryListVM);


                //  //set the counts... oMemProfiles.Result.ToList().OfType<MemberProfile>()
                //  //var maleCount = qry.ToList().Where(c => c.oPersonalData.Gender == "M").Count();   //oMemProfiles.Result.ToList().OfType<MemberProfile>()
                //  //var femaleCount = qry.Where(c => c.oPersonalData.Gender == "F").Count();

                //  //ViewBag.MaleCount = maleCount;
                //  //ViewBag.FemaleCount = femaleCount;
                //  //ViewBag.OtherCount = qry.Count - maleCount - femaleCount;    //oMemProfiles.Result

                // // var oMemProfiles = AppUtilties_Static.ToListAsync<MemberProfileVM>(qry);

                //  //ViewBag.oMemberPaneFilters = LoadMemberListFilters((int)currChuBodyId);
                //  // ViewBag.oMemProfiles_Init = qry;



                // // var oCLVM = populateLookups(oCCEModel, currChuBodyId);


                // // return View(oCLVM);
                oCCEModel.currAttendTask = currAttendTask;
                oCCEModel.oChurchBody = oRequestedChurchBody;  //current working CB
                oCCEModel.oChurchBodyId = oRequestedChurchBody.Id;
                oCCEModel.oLoggedChurchBody = oCurrChuBody;

            //var _vmMod = populateLookups(oCCEModel, oCurrChuBody);

            //TempData.Keep();
            //return View(_vmMod); 


            ///  

            var strDesc = "Church Member";
            var _userTask = "Viewed " + strDesc.ToLower() + " list";
           // oCCEModel.strCurrTask = strDesc;


            var tm = DateTime.Now;
            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                             "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

            ///
            var _oCCEModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCCEModel);
            TempData["oVmCSPModel"] = _oCCEModel; TempData.Keep();

            
                return View("Index_CCE", oCCEModel);

        }


        public IActionResult Index_CCE(int currAttendTask = 1, int? reqChurchBodyId = null, bool loadLim = false)
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

            // if (oCurrChuBody == null) return View();

            //ViewBag.strChuBodyLogged = oCurrChuBody.AppGlobalOwner.OwnerName.ToUpper() + " - " + oCurrChuBody.Name;
            ViewBag.strChuBodyLogged = this._oLoggedAGO.OwnerName.ToUpper() + " - " + oCurrChuBody.Name;

            //ChurchBody oRequestedChurchBody;
            //if (reqChurchBodyId > 0)  // != null)
            //    oRequestedChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.Id == reqChurchBodyId).FirstOrDefault();
            //else
            //{ reqChurchBodyId = oCurrChuBody.Id; oRequestedChurchBody = oCurrChuBody; }

            // check permission for Core life...
            //  if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) return View();

            //var oAttendVM = new ChurchCalendarModel();

            //if (currAttendTask == 1)
            //{
            //    oAttendVM.strCurrTaskDesc = oAttendVM.strAttendeeTypeCode == "M" ? oAttendVM.strAttnLongevity == "H" ? "Member Attendance Headcount -- History" : "Member Attendance Headcount (Today)" :
            //                         oAttendVM.strAttendeeTypeCode == "V" ? oAttendVM.strAttnLongevity == "H" ? "Visitors Headcount History (Attendance)" : "Visitor Attendance Headcount (Today)" :
            //                                                      oAttendVM.strAttnLongevity == "H" ? "Church Attendance Headcount -- History" : "Church Attendance Headcount(Today)";  // isMigrated == true ? "New Converts" : "New Coverts (Historic)" : isMigrated == true ? "Visitors" : "Visitors (Historic)";
            //}
            //else
            //{
            //    oAttendVM.strCurrTaskDesc = "Church Attendance -- Headcount";
            //}


            var oCCEModel = new ChurchCalendarModel();

            //  // List<ChurchCalendarModel> qry = new List<ChurchCalendarModel>();
            //  //qry = ReturnMemberProfileList();


            //  // var qry = ReturnMemberProfileList();

            //oCCEModel.oCalendarEvents = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity).Include(t => t.ChurchBody).Include(t => t.OwnedByChurchBody)
            //    // .Include(c => c.ChurchBody).Include(c => c.ChurchEventCategory).Include(c => c.OwnedByChurchBody)
            //    .Where(c =>
            //                    (c.OwnedByChurchBodyId == oCurrChuBody.Id ||
            //                    (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            //                    (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
            //           )
            //    //.OrderByDescending(c=>c.EventActive)
            //    .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo).ThenBy(c => c.Subject) //.ThenByDescending(c=>c.(DateTime)EventTo)
            //    .ToList();


            var oCCE_List_All = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP).Include(t => t.ChurchBody)
                                        .ToList();

            oCCE_List_All = oCCE_List_All.Where(c =>
                                   (c.ChurchBodyId == this._oLoggedCB.Id ||
                                   (c.ChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.ChurchBodyId == _oLoggedCB.ParentChurchBodyId) ||
                                   (c.ChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.ChurchBody, this._oLoggedCB))))
                                .ToList();

            if (oCCE_List_All.Count > 0)
                oCCE_List_All = oCCE_List_All
                   //.OrderByDescending(c=>c.EventActive)
                   .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo).ThenBy(c => c.Subject) //.ThenByDescending(c=>c.(DateTime)EventTo)
                   .ToList();

            oCCEModel.oCalendarEvents = oCCE_List_All;

            long gloCount, parCount, locCount, uncatCount;
            gloCount = 0; parCount = 0; locCount = 0; uncatCount = 0;
            var oActiveList = oCCEModel.oCalendarEvents.Where(c => c.IsEventActive == true).ToList();
            foreach (var oEv in oActiveList)
            {
                if (oEv.ChurchBodyId == oEv.ChurchBodyId) locCount++;
                else if (oEv.ChurchBodyId == oEv.ChurchBody.ParentChurchBodyId) parCount++;
                else if (IsAncestor_ChurchBody(oEv.ChurchBody, oEv.ChurchBody) == true) gloCount++; //  (oEv.OwnedByChurchBodyId == oEv.ChurchBodyId) 
                else uncatCount++;
            }

            oCCEModel.ActivityCount_Local = locCount;
            oCCEModel.ActivityCount_Parent = parCount;
            oCCEModel.ActivityCount_Global = gloCount;
            oCCEModel.ActivityCount_Uncat = uncatCount;

            //oCurrChuBody ... church level names  ... this code must be part of login info.. load once...
            oCCEModel.strActivityCount_Local = "Global Church";
            oCCEModel.strActivityCount_Parent = "Parent Congregation";
            oCCEModel.strActivityCount_Global = "Local Congregation";

            if (oCurrChuBody.ChurchLevel != null)
                oCCEModel.strActivityCount_Local = !string.IsNullOrEmpty(oCurrChuBody.ChurchLevel.CustomName) ? oCurrChuBody.ChurchLevel.CustomName : oCurrChuBody.ChurchLevel.Name;

            var oCBParent = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.Id == oCurrChuBody.ParentChurchBodyId).FirstOrDefault();
            if (oCBParent != null)
                if (oCBParent.ChurchLevel != null)
                    oCCEModel.strActivityCount_Parent = !string.IsNullOrEmpty(oCBParent.ChurchLevel.CustomName) ? oCBParent.ChurchLevel.CustomName : oCBParent.ChurchLevel.Name;

            var lowestCB = _context.ChurchLevel.Where(x => x.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && x.LevelIndex == _context.ChurchLevel.Where(y => y.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId).Min(y => y.LevelIndex)).FirstOrDefault();
            if (lowestCB != null)
            {
                var oCBGlobal = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.ChurchLevelId == lowestCB.Id
                                    ).FirstOrDefault();
                oCCEModel.strActivityCount_Global = !string.IsNullOrEmpty(oCBGlobal.ChurchLevel.CustomName) ? oCBGlobal.ChurchLevel.CustomName : oCBGlobal.ChurchLevel.Name;
            }


            //  var qryListVM = new List<ChurchCalendarModel>();
            //  qryListVM.Add(oCCEModel);



            //  //if (!(filterIndex == null || filterVal == null))
            //  //    qry = GetMemberProfiles(qry, filterIndex, filterVal);

            //  var oCalendarEvents = AppUtilties_Static.ToListAsync<ChurchCalendarModel>(qryListVM);


            //  //set the counts... oMemProfiles.Result.ToList().OfType<MemberProfile>()
            //  //var maleCount = qry.ToList().Where(c => c.oPersonalData.Gender == "M").Count();   //oMemProfiles.Result.ToList().OfType<MemberProfile>()
            //  //var femaleCount = qry.Where(c => c.oPersonalData.Gender == "F").Count();

            //  //ViewBag.MaleCount = maleCount;
            //  //ViewBag.FemaleCount = femaleCount;
            //  //ViewBag.OtherCount = qry.Count - maleCount - femaleCount;    //oMemProfiles.Result

            // // var oMemProfiles = AppUtilties_Static.ToListAsync<MemberProfileVM>(qry);

            //  //ViewBag.oMemberPaneFilters = LoadMemberListFilters((int)currChuBodyId);
            //  // ViewBag.oMemProfiles_Init = qry;



            // // var oCLVM = populateLookups(oCCEModel, currChuBodyId);


            // // return View(oCLVM);
            oCCEModel.currAttendTask = currAttendTask;
            oCCEModel.oChurchBody = oRequestedChurchBody;  //current working CB
            oCCEModel.oChurchBodyId = oRequestedChurchBody.Id;
            oCCEModel.oAppGloOwnId = oAppGloOwnId;
            oCCEModel.oLoggedChurchBody = oCurrChuBody;
            oCCEModel.strChurchBody = oCCEModel.oChurchBody != null ? oCCEModel.oChurchBody.Name : "";

            //var _vmMod = populateLookups(oCCEModel, oCurrChuBody);

            //TempData.Keep();
            //return View(_vmMod); 


            ///  

            var strDesc = "Church Member";
            var _userTask = "Viewed " + strDesc.ToLower() + " list";
            // oCCEModel.strCurrTask = strDesc;


            var tm = DateTime.Now;
            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                             "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

            ///
            var _oCCEModel = Newtonsoft.Json.JsonConvert.SerializeObject(oCCEModel);
            TempData["oVmCurrMod"] = _oCCEModel; TempData.Keep();

            if (loadLim)
                return PartialView("_vwIndex_CCE", oCCEModel);
            else
                return View("Index_CCE", oCCEModel);

        }



        [HttpGet]
        public IActionResult AddOrEdit_ProgEvent(int id = 0, int? oCurrChuBodyId = null, string strLongevity = "C")
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

                //if (!InitializeUserLogging())
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                // Client 
               // if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                var oAppGloOwnId = this._oLoggedAGO.Id;
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



                //SetUserLogged();
                //if (!isCurrValid) //prompt!
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                //var oCurrVmMod = TempData.Get<CoreChurchlifeVM>("oVmCurr"); TempData.Keep();
                //if (oCurrVmMod == null) oCurrVmMod = new CoreChurchlifeVM();
                ////
                //var oCurrChuBody_Logged = oUserLogIn_Priv[0].ChurchBody;
                //var oUserProfile = oUserLogIn_Priv[0].UserProfile;
                //var oCurrChuMember_Logged = oUserProfile.ChurchMember;
                //if (oCurrChuBody_Logged == null || oCurrChuMember_Logged == null) return null;

                //// check permission for Core life...
                //if (oUserLogIn_Priv.Find(x => x.PermissionName == "Manage_SuperAdmin_Priv" || x.PermissionName == "xxx") == null) //prompt!
                //    return null;


                var oCurrChuBody = oCB;
                var oCurrVmMod = new ChurchCalendarModel();

                //ChurchBody oCurrChuBody = null;
                //// var oCurrChuBody = _context.ChurchBody.Include(t=>t.AppGlobalOwner).Where(c=>c.Id == oCurrChuBodyId).FirstOrDefault();
                //if (oCurrChuBodyId != null) oCurrChuBody = _context.ChurchBody.Include(t => t.AppGlobalOwner).Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();
                //if (oCurrChuBody == null) oCurrChuBody = oCurrChuBody_Logged;

                oCurrVmMod.oAppGlobalOwner = oAGO; // oCurrChuMember_Logged.AppGlobalOwner;


                if (id == 0)
                {
                    //create user and init...
                    oCurrVmMod.oChurchEvent = new ChurchCalendarEvent();
                    oCurrVmMod.oChurchEvent.ChurchBodyId = oCurrChuBody.Id;
                   // oCurrVmMod.oChurchEvent.OwnedByChurchBodyId = oCurrChuBody.Id;
                    oCurrVmMod.oChurchEvent.SharingStatus = "N"; //do Not share unless otherwise indicated
                    oCurrVmMod.oChurchEvent.EventFrom = DateTime.Now;
                    oCurrVmMod.oChurchEvent.EventTo = DateTime.Now;
                    oCurrVmMod.oChurchEvent.IsEventActive = true;
                    oCurrVmMod.oChurchEvent.strActivityTypeCode = "";

                }

                else
                {
                    var oEvent = _context.ChurchCalendarEvent.Include(t => t.ChurchlifeActivity_NVP)
                                      .Where(c => c.ChurchBodyId == oCurrChuBody.Id && c.Id == id).FirstOrDefault();
                    if (oEvent != null)
                        if (oEvent.ChurchlifeActivity_NVP != null)
                            oEvent.strActivityTypeCode = oEvent.ChurchlifeActivity_NVP.NVPValue;

                    oCurrVmMod.oChurchEvent = oEvent;
                    // oCurrVmMod.oChurchEvent = _context.ChurchCalendarEvent.Find(id);
                }



                //  oCurrVmMod.oChurchBody = _context.ChurchBody.Find(currChuBodyId); ;

                // var _vmMod = populateLookups_ProgEvent(vmMod, currChuBodyId);


                //    TempData.Put("oVmCurr", _vmMod);
                //   TempData.Keep();

                //   return PartialView("_AddOrEdit_ProgEvent", _vmMod);


                oCurrVmMod.oChurchBody = oCurrChuBody;  //current working CB
                oCurrVmMod.oLoggedChurchBody = this._oLoggedCB;  // oCurrChuBody_Logged;
                oCurrVmMod.strEventLongevity = strLongevity;  // today (C), history (H) 

                //oCurrVmMod.oFilter_ChurchAttendees_VMList = new List<ChurchAttendanceVM>();

                if (oCurrVmMod == null) return PartialView("_vwErrorPage"); ; // PartialView("_AddOrEdit_ProgEvent", oCurrVmMod);

                //  TempData.Put("oCurrChuMemberPrvw", oCurrVmMod.oChurchAttend);
                var vmMod = populateLookups_ProgEvent(oCurrVmMod, oCurrChuBody); // populateLookups_Attend(oCurrVmMod, oCurrChuBody, oCurrVmMod.oChurchAttend.DateAttended);//, oCurrVmMod.oChurchAttend.Id);
             

                var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(vmMod);
                TempData["oVmCurrMod"] = vmMod; TempData.Keep();

                return PartialView("_AddOrEdit_CCE", vmMod);  // AddOrEdit_ProgEvent



            }
            catch (Exception ex)
            {
                return PartialView("_vwErrorPage");
            } 
        }


        [HttpGet]
        public IActionResult AddOrEdit_CCE(int id = 0, int? oCurrChuBodyId = null, string strLongevity = "C")  //(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) //, int pageIndex = 1)
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
                        return PartialView("_vwErrorPage");
                    }
                }


                //if (!InitializeUserLogging())
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                // Client
                // if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;

                var oAppGloOwnId = this._oLoggedAGO.Id;
                if (oCurrChuBodyId == null) oCurrChuBodyId = this._oLoggedCB.Id;

                // MSTR
                var oUserId = this._oLoggedUser.Id;
                var oAGO_MSTR = this._oLoggedAGO_MSTR; var oCB_MSTR = this._oLoggedCB_MSTR; // _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId); 
                var oAGO = this._oLoggedAGO;
                var oCB = _context.ChurchBody.AsNoTracking().Include(t => t.ParentChurchBody).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                if (oCB == null) oCB = this._oLoggedCB;

                if (oAGO_MSTR == null || oCB_MSTR == null || oAGO == null || oCB == null)   // || oCU_Parent == null church units may be networked...
                { return PartialView("_vwErrorPage"); }

                var strDesc = "Church Calendar";
                var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();



                var oCCEModel = new ChurchCalendarModel();

                //var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                //                    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                //oCP_List_1 = oCP_List_1.Where(c =>
                //                   (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                //                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                if (id == 0)
                {
                    var oCCE = new ChurchCalendarEvent();

                    // this attr is used by most of the models... save in memory [class var]
                    oCTRYDefault = _context.CountryCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsDefaultCountry == true).FirstOrDefault();
                    oCURRDefault = _context.CurrencyCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.IsBaseCurrency == true).FirstOrDefault();

                    var oCP_List_1 = _context.ChurchPeriod.AsNoTracking().Include(t => t.OwnedByChurchBody) //
                                    .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Status == "A").ToList();  // && c.PeriodType == "AP"

                    oCP_List_1 = oCP_List_1.Where(c =>
                                       (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                       (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    oCPRDefault = oCP_List_1.FirstOrDefault();// _context.ChurchPeriod.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.Status == "A").FirstOrDefault();  // c.PeriodType == "CP" && 

                    ///
                    oCCE.AppGlobalOwnerId = oAppGloOwnId;
                    oCCE.ChurchBodyId = oCurrChuBodyId; 
                    // oCCE.OwnedByChurchBodyId = oCurrChuBody.Id;
                    oCCE.SharingStatus = "N"; //do Not share unless otherwise indicated
                    oCCE.EventFrom = DateTime.Now;
                    oCCE.EventTo = DateTime.Now;
                    oCCE.IsEventActive = true;
                    oCCE.strActivityTypeCode = "";

                    oCCEModel.oChurchEvent = oCCE;
                }

                else
                {
                    oCCEModel = (
                       from t_cce in _context.ChurchCalendarEvent.AsNoTracking().Include(t => t.ChurchlifeActivity_NVP)
                                    .Where(c => c.Id == id && c.AppGlobalOwnerId == oAppGloOwnId)   //  && c.ChurchBodyId == oCurrChuBodyId
                       from t_cb in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
                                .Where(c => c.Id == t_cce.ChurchBodyId && c.AppGlobalOwnerId == t_cce.AppGlobalOwnerId)   //c.Id == oChurchBodyId && 

                       //from t_ap in _context.ChurchPeriod.AsNoTracking().Where(c => c.AppGlobalOwnerId == t_cce.AppGlobalOwnerId && c.Id == t_cce.ChurchPeriodId).DefaultIfEmpty()   //_context.ChurchPeriod.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_cce.AccountPeriodId) .DefaultIfEmpty()  //c.Id == oChurchBodyId &&   // from t_an in _context.AppUtilityNVP.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_cce.TitheModeId).DefaultIfEmpty()
                       //from t_cb_corp in _context.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner)
                       //         .Where(c => c.AppGlobalOwnerId == t_cce.AppGlobalOwnerId && c.Id == t_cce.Corporate_ChurchBodyId).DefaultIfEmpty()
                       //from t_cm in _context.ChurchMember.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_cce.ChurchMemberId).DefaultIfEmpty()
                           // from t_curr in _context.Currency.AsNoTracking().Where(c => c.ChurchBodyId == t_cb.Id && c.Id == t_cce.CurrencyId).DefaultIfEmpty()

                       select new ChurchCalendarModel()
                       {
                           oChurchEvent = t_cce,
                           oAppGloOwnId = t_cce.AppGlobalOwnerId,
                           oAppGlobalOwner = t_cb.AppGlobalOwner,
                           oChurchBodyId = t_cce.ChurchBodyId,
                           oChurchBody = t_cb,
                           //               
                       }
                       ).FirstOrDefault();

                     
                    if (oCCEModel != null)
                        if (oCCEModel.oChurchEvent != null)
                            if (oCCEModel.oChurchEvent.ChurchlifeActivity_NVP != null)
                                oCCEModel.oChurchEvent.strActivityTypeCode = oCCEModel.oChurchEvent.ChurchlifeActivity_NVP.NVPValue; 
                }

                //oCCEModel.setIndex = setIndex;
                //oCCEModel.oUserId_Logged = oUserId_Logged;
                //oCCEModel.oAppGlolOwnId_Logged = oAGOId_Logged;
                //oCCEModel.oChurchBodyId_Logged = oCBId_Logged;
                //
                oCCEModel.oAppGloOwnId = oAppGloOwnId;
                oCCEModel.oChurchBodyId = oCurrChuBodyId;
                var oCurrChuBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                oCCEModel.oChurchBody = oCurrChuBody;  // != null ? oCurrChuBody : null;


                // ChurchBody oChurchBody = _context.ChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();
                oCCEModel = this.populateLookups_CCE(oCCEModel, oCurrChuBody);
                // oCurrMdl.strCurrTask = "Church Tithe";

                var _oCurrMdl = Newtonsoft.Json.JsonConvert.SerializeObject(oCCEModel);
                TempData["oVmCurrMod"] = _oCurrMdl; TempData.Keep();

                return PartialView("_AddOrEdit_CCE", oCCEModel);

            }
            catch (Exception ex)
            {
                return PartialView("_ErrorPage");
            }
        }


        private ChurchCalendarModel populateLookups_CCE(ChurchCalendarModel vmLkp, ChurchBody oCurrChuBody)  // int? currChuBodyId)
        {

            if (vmLkp == null || oCurrChuBody == null || this._oLoggedAGO == null) return vmLkp;

            ChurchLevel churchLevel = oCurrChuBody.ChurchLevel;
            if (churchLevel == null)
                churchLevel = _context.ChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.Id == oCurrChuBody.ChurchLevelId).FirstOrDefault();

            var colorMin = 0;
            var colorMax = dlThemeColor.Count - 1;
            if (churchLevel != null)   //  -- [i - 1] * 4  -- [i * 4] - 1
            {
                colorMin = (churchLevel.LevelIndex - 1) * 4;
                colorMax = (churchLevel.LevelIndex * 4) - 1;
            }
                
            /// assign colors per church level  --- for easy identification
            vmLkp.lkp_ThemeColors = new List<SelectListItem>();
            for (var i = 0; i < dlThemeColor.Count; i++) 
            { if (i >= colorMin && i <= colorMax) vmLkp.lkp_ThemeColors.Add(new SelectListItem { Value = dlThemeColor[i].Val, Text = dlThemeColor[i].Desc }); }

            vmLkp.lkp_ActivityType = new List<SelectListItem>();
            foreach (var dl in dlActivityTypes) { vmLkp.lkp_ActivityType.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkp_ActivityType.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vmLkp.lkpSharingStatus = new List<SelectListItem>();
            foreach (var dl in dlShareStatus) { vmLkp.lkpSharingStatus.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }


            //vmLkp.lkp_ChurchEventCategory = _context.ChurchEventCategory.Where(c => c.ChurchBodyId == oCurrChuBody.Id)
            //                               .Select(c => new SelectListItem()
            //                               {
            //                                   Value = c.Id.ToString(),
            //                                   Text = c.EventName
            //                               })
            //                                .OrderBy(c => c.Text)
            //                               .ToList();

            //vmLkp.lkp_ChurchEventCategory.Insert(0, new SelectListItem { Value = "", Text = "Select" });
             
            var strNVPCode = "CLA";    // c.Id != oCurrNVP.Id && 
            var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                               .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode).ToList();

            oNVP_List_1 = oNVP_List_1.Where(c =>
                                (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

            vmLkp.lkp_ChurchLifeActivity = oNVP_List_1.ToList()
                                                    .Select(c => new SelectListItem()
                                                    {
                                                        Value = c.Id.ToString(),
                                                        Text = c.NVPValue.ToString()
                                                    })
                                                    .OrderBy(c => c.Text)
                                                    .ToList();

            //TempData.Keep();
            return vmLkp;
        }



        private ChurchCalendarModel populateLookups_ProgEvent(ChurchCalendarModel vmLkp, ChurchBody oCurrChuBody)  // int? currChuBodyId)
        {

            if (vmLkp == null || oCurrChuBody == null || this._oLoggedAGO == null) return vmLkp;

            //  vmLkp = populateLookups(vmLkp, currChuBodyId);

            //  if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vmLkp.lkp_ThemeColors = new List<SelectListItem>();
            foreach (var dl in dlThemeColor) { vmLkp.lkp_ThemeColors.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }

            vmLkp.lkp_ActivityType = new List<SelectListItem>();
            foreach (var dl in dlActivityTypes) { vmLkp.lkp_ActivityType.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }
            vmLkp.lkp_ActivityType.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            vmLkp.lkpSharingStatus = new List<SelectListItem>();
            foreach (var dl in dlShareStatus) { vmLkp.lkpSharingStatus.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }


            //vmLkp.lkp_ChurchEventCategory = _context.ChurchEventCategory.Where(c => c.ChurchBodyId == oCurrChuBody.Id)
            //                               .Select(c => new SelectListItem()
            //                               {
            //                                   Value = c.Id.ToString(),
            //                                   Text = c.EventName
            //                               })
            //                                .OrderBy(c => c.Text)
            //                               .ToList();

            //vmLkp.lkp_ChurchEventCategory.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            //vmLkp.lkp_ChurchLifeActivity = _context.ChurchLifeActivity  //.Include(t => t.)
            //                            .Where(c => !string.IsNullOrEmpty(c.Description) && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&
            //                                  // c.ChurchBody.CountryId == oCurrChuBody.CountryId && c.EventFrom.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt  &&
            //                                  (c.ActivityType != "ER") &&
            //                                   (c.OwnedByChurchBodyId == oCurrChuBody.Id ||
            //                                   (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
            //                                   (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
            //                               )
            //                                // .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)
            //                                .ToList()
            //                                        .Select(c => new SelectListItem()
            //                                        {
            //                                            Value = c.Id.ToString(),
            //                                            Text = c.Description
            //                                        })
            //                                        .OrderBy(c => c.Text)
            //                                        .ToList();

            var strNVPCode = "CLA";    // c.Id != oCurrNVP.Id && 
            var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                               .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode).ToList();
            oNVP_List_1 = oNVP_List_1.Where(c =>
                                (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

            vmLkp.lkp_ChurchLifeActivity = oNVP_List_1.ToList()
                                                    .Select(c => new SelectListItem()
                                                     {
                                                         Value = c.Id.ToString(),
                                                         Text = c.NVPValue.ToString()
                                                     })
                                                    .OrderBy(c => c.Text)
                                                    .ToList();

            //TempData.Keep();
            return vmLkp;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_ProgEvent(ChurchCalendarModel vm)
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


            //TitheTrans _oChanges = vmMod.oTitheTrans;   //  vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as TitheModel : vmMod; TempData.Keep();

            //var arrData = ""; // TempData["oVmCurrMod"] as string;
            //arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            //vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<TitheModel>(arrData) : vmMod;
            ////
            //var oMdlData = vmMod.oTitheTrans;
            //oMdlData.ChurchBody = vmMod.oChurchBody;


            //var _oVmCurr = TempData.Get<ChurchCalendarModel>("oVmCurr");
            var arrData = ""; // TempData["oVmCurrMod"] as string;
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var _oVmCurr = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchCalendarModel>(arrData) : vm;
            //

            ChurchBody currChuBody = null;
            if (_oVmCurr != null)
                currChuBody = _oVmCurr.oChurchBody;

            var oChurchEvent = new ChurchCalendarEvent();
            if (vm.oChurchEvent != null)
                oChurchEvent = vm.oChurchEvent;

            //dummy user... no role yet
            ModelState.Remove("oChurchEvent.ChurchEventCategoryId");
            ModelState.Remove("oChurchEvent.ChurchBodyId");

          //  ModelState.Remove("oChurchEvent.OwnedByChurchBodyId");

            //if (currMember == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Current member previewed not found");
            //    ViewBag.UserDelMsg = "Current member previewed not found";
            //}


            //finally check error state...
            if (ModelState.IsValid)
            {
                oChurchEvent.LastMod = DateTime.Now;
                oChurchEvent.ChurchBody = currChuBody;
               // oChurchEvent.OwnedByChurchBody = currChuBody;
                if (oChurchEvent.Id == 0)
                {
                    oChurchEvent.Created = DateTime.Now;
                    _context.Add(oChurchEvent);

                    ViewBag.UserMsg = "Saved church event successfully.";
                }

                else
                {
                    _context.Update(oChurchEvent);
                    ViewBag.UserMsg = "Updated church event successfully.";
                }


                //save details... locAddr
                try
                {
                    await _context.SaveChangesAsync();

                    if (_oVmCurr != null)
                        _oVmCurr.oChurchEvent = oChurchEvent;
                     
                    var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(_oVmCurr);
                    TempData["oVmCurr"] = _vmMod; TempData.Keep();

                    return Json(true);
                }

                catch (Exception ex)
                {
                    ViewBag.UserMsg = "Failed saving church event. Err: " + ex.ToString();
                }
            }

            return Json(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_CCE(ChurchCalendarModel vm)
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


            //TitheTrans _oChanges = vmMod.oTitheTrans;   //  vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as TitheModel : vmMod; TempData.Keep();

            //var arrData = ""; // TempData["oVmCurrMod"] as string;
            //arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            //vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<TitheModel>(arrData) : vmMod;
            ////
            //var oMdlData = vmMod.oTitheTrans;
            //oMdlData.ChurchBody = vmMod.oChurchBody;


            //var _oVmCurr = TempData.Get<ChurchCalendarModel>("oVmCurr");
            var arrData = ""; // TempData["oVmCurrMod"] as string;
            arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            var _oVmCurr = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchCalendarModel>(arrData) : vm;
            //

            ChurchBody currChuBody = null;
            if (_oVmCurr != null)
                currChuBody = _oVmCurr.oChurchBody;

            var oChurchEvent = new ChurchCalendarEvent();
            if (vm.oChurchEvent != null)
                oChurchEvent = vm.oChurchEvent;

            //dummy user... no role yet
            ModelState.Remove("oChurchEvent.ChurchEventCategoryId");
            ModelState.Remove("oChurchEvent.ChurchBodyId");

          //  ModelState.Remove("oChurchEvent.OwnedByChurchBodyId");

            //if (currMember == null)
            //{
            //    ModelState.AddModelError(string.Empty, "Current member previewed not found");
            //    ViewBag.UserDelMsg = "Current member previewed not found";
            //}


            //finally check error state...
            if (ModelState.IsValid)
            {
                oChurchEvent.LastMod = DateTime.Now;
                // oChurchEvent.ChurchBody = currChuBody;
                // oChurchEvent.OwnedByChurchBody = currChuBody;

                if (oChurchEvent.AppGlobalOwner != null) oChurchEvent.AppGlobalOwner = null;
                if (oChurchEvent.ChurchBody != null) oChurchEvent.ChurchBody = null;
                //if (oChurchEvent.ChurchlifeActivity != null) oChurchEvent.ChurchlifeActivity = null;
                ///
                var _reset = oChurchEvent.Id == 0;
                if (oChurchEvent.Id == 0)
                {
                    oChurchEvent.Created = DateTime.Now;
                    _context.Add(oChurchEvent);

                    ViewBag.UserMsg = "Saved church event successfully.";
                }

                else
                {
                    _context.Update(oChurchEvent);
                    ViewBag.UserMsg = "Updated church event successfully.";
                }


                //save details... locAddr
                try
                {
                    _context.SaveChanges();

                    if (_oVmCurr != null)
                        _oVmCurr.oChurchEvent = oChurchEvent;
                     
                    var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(_oVmCurr);
                    TempData["oVmCurr"] = _vmMod; TempData.Keep();

                   // return Json(true);

                    return Json(new { taskSuccess = true, oCurrId = oChurchEvent.Id, resetNew = _reset, userMess = ViewBag.UserMsg, lastCodeUsed = oChurchEvent.ChurchlifeActivityId });
                }

                catch (Exception ex)
                {
                    ViewBag.UserMsg = "Failed saving church event. Err: " + ex.ToString();
                }
            }

            return Json(false);
        }


        private ChurchCalendarModel populateLookups(ChurchCalendarModel vmLkp, ChurchBody oCurrChuBody)  // int? currChuBodyId)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return null; //// View("_ErrorPage");
                }
            }

            if (vmLkp == null || oCurrChuBody == null || this._oLoggedAGO == null) return vmLkp;

            //vmLkp.oChurchSectorCategories = (
            //    from sc in _context.ChurchSectorCategory.Where(c => c.ChurchBodyId == currChuBodyId) // && c.IsMainstream == false)
            //    select new ChurchSectorCategory
            //    {
            //        Id = sc.Id,
            //        Name = AppUtilties.Pluralize(sc.Name),
            //        ChurchBodyId = sc.ChurchBodyId,
            //        OwnedByChurchBodyId = sc.OwnedByChurchBodyId,
            //        IsMainstream = sc.IsMainstream
            //    }
            //    ).ToList();

            // vmLkp.oChurchLifeActivities = _context.ChurchLifeActivity.Where(c => c.ChurchBodyId == currChuBodyId && c.IsMainline == true).ToList();

            // vmLkp.oChurchLifeServices = _context.ChurchLifeActivity.Where(c => c.ChurchBodyId == currChuBodyId && c.IsService == true).ToList();

            //TempData.Keep();


            var strNVPCode = "CLA";    // c.Id != oCurrNVP.Id && 
            var oNVP_List_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                               .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode).ToList();
            oNVP_List_1 = oNVP_List_1.Where(c =>
                                (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                               (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

            vmLkp.oChurchLifeActivities = oNVP_List_1.ToList();  //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                        //.OrderBy(c => c.OrderIndex)
                                        //.ThenBy(c => c.NVPValue)
                                        //.Select(c => new SelectListItem()
                                        //{
                                        //    Value = c.Id.ToString(),
                                        //    Text = c.NVPValue
                                        //})
                                        //.ToList();

            return vmLkp;
        }



        public JsonResult GetChurchActivityByType(int oCurrChuBodyId, string typeCode = null, bool addEmpty = false)
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


            var eventList = new List<SelectListItem>();

            var oCurrChuBody = _context.ChurchBody.Include(t => t.ChurchLevel).Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();
            if (oCurrChuBody != null)
            {
                ////GA-- General activ, ER-Event Role,  MC--Member Churchlife Activity related, EV-Church Event related
                ///
                var strNVPCode = "CLA";
                var eventList_1 = _context.AppUtilityNVP.AsNoTracking().Include(t => t.OwnedByChurchBody) //.Include(t => t.AppGlobalOwner) //
                                               .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId && c.NVPCode == strNVPCode &&
                                               ((!string.IsNullOrEmpty(typeCode) && c.NVPSubCode == typeCode) || (string.IsNullOrEmpty(typeCode) && c.NVPSubCode != "CLA_ER"))
                                               ).ToList();

                eventList_1 = eventList_1.Where(c =>
                                    (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                   (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();


                eventList = eventList_1  // _context.ChurchlifeActivity  //.Include(t => t.)
                                 //.Where(c => !string.IsNullOrEmpty(c.Description) && c.ChurchBody.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId &&
                                 //      // c.ChurchBody.CountryId == oCurrChuBody.CountryId && c.EventFrom.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == dt  &&
                                 //      ((!string.IsNullOrEmpty(typeCode) && c.ActivityType == typeCode) || (string.IsNullOrEmpty(typeCode) && c.ActivityType != "CLA_ER")) &&
                                 //       (c.OwnedByChurchBodyId == oCurrChuBody.Id ||
                                 //       (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == oCurrChuBody.ParentChurchBodyId) ||
                                 //       (c.OwnedByChurchBodyId != oCurrChuBody.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.OwnedByChurchBody, oCurrChuBody)))
                                 //   )
                                     // .OrderByDescending(c => c.EventFrom).ThenByDescending(c => c.EventTo)

                                     .ToList()
                                             .Select(c => new SelectListItem()
                                             {
                                                 Value = c.Id.ToString(),
                                                 Text = c.NVPValue.ToString()
                                             })
                                             .OrderBy(c => c.Text)
                                             .ToList();
            }
            if (addEmpty) eventList.Insert(0, new SelectListItem { Value = "", Text = "Select" });
            return Json(eventList);
        }


        [HttpGet]
        public JsonResult GetEvents(int? oCurrChuBodyId = null)
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

            oCurrChuBodyId = oCurrChuBodyId == null ? this._oLoggedCB.Id : oCurrChuBodyId;
            var oCurrChuBody = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
                .Where(c => c.Id == oCurrChuBodyId).FirstOrDefault();
            var eventList = new List<ChurchCalendarEvent>();

            if (oCurrChuBody != null)
            {
                var events_1 = _context.ChurchCalendarEvent.AsNoTracking().Include(c => c.ChurchBody).ThenInclude(c => c.ChurchLevel)
                                               .Where(c => c.AppGlobalOwnerId == oCurrChuBody.AppGlobalOwnerId).ToList();

                events_1 = events_1.Where(c =>
                                    (c.ChurchBodyId == this._oLoggedCB.Id ||
                                   (c.ChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.ChurchBodyId == this._oLoggedCB.ParentChurchBodyId) ||
                                   (c.ChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && IsAncestor_ChurchBody(c.ChurchBody, this._oLoggedCB)))).ToList();

                eventList = events_1.ToList(); 

            }

            var strKey = "";
            foreach (var ev in eventList)
            {
                if (ev.ChurchBody != null)
                    if (ev.ChurchBody.ChurchLevel != null)
                        strKey = !string.IsNullOrEmpty(ev.ChurchBody.ChurchLevel.Acronym) ? ev.ChurchBody.ChurchLevel.Acronym : "L" + ev.ChurchBody.ChurchLevel.LevelIndex.ToString();

                ev.strEventFullDesc = !string.IsNullOrEmpty(strKey) ? (strKey + " : " + ev.Description) : ev.Description;
            }


            //var obj = new { events = events };
            return Json(eventList); //Json(_context.ChurchCalendarEvent.ToList());
        }



    }
}
