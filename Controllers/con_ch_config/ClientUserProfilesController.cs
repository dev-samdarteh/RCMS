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
using System.Collections;
using System.Net.Mail;
using static RhemaCMS.Controllers.con_adhc.AppUtilties;
//using RhemaCMS.Models.ViewModels.vm_app_ven;

namespace RhemaCMS.Controllers.con_ch_config
{
    public class ClientUserProfilesController : Controller
    {
        private readonly MSTR_DbContext _masterContext;
        private ChurchModelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        ///
        //private string _strClientConn;
        //private UserProfile _oLoggedUser;
        // private UserRole _oLoggedRole;
        //  private MSTRChurchBody _oLoggedCB_MSTR;
        //  private MSTRAppGlobalOwner _oLoggedAGO_MSTR;


        //   private string _strClientConn;
        private string _clientDBConnString;
        private UserProfile _oLoggedUser;
        // private UserRole _oLoggedRole;
        //private MSTRChurchBody _oLoggedCB_MSTR;
        //private MSTRAppGlobalOwner _oLoggedAGO_MSTR;

        /// localized
        private ChurchBody _oLoggedCB;
        private MSTRChurchBody _oLoggedCB_MSTR;
        private AppGlobalOwner _oLoggedAGO;
        private MSTRAppGlobalOwner _oLoggedAGO_MSTR;  
        ///
        private string strCountryCode_dflt = (string)null;
        private string strCountryName_dflt = "";
        private string strCountryCURR1_dflt = "";
        private string strCountryCURR2_dflt = "";

        private bool isCurrValid = false;
        private UserSessionPrivilege oUserLogIn_Priv = null;
        ///        
        private List<DiscreteLookup> dlGenStatuses = new List<DiscreteLookup>();
        private List<DiscreteLookup> dlUserLevels = new List<DiscreteLookup>();



        private readonly IConfiguration _configuration;
        private string _clientDBConn;

        public ClientUserProfilesController(MSTR_DbContext masterContext, IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, ChurchModelContext clientCtx,
            IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _masterContext = masterContext;
            // _context = context;
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


            ////load the dash
            //LoadClientDashboardValues();


            ///
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "A", Desc = "Active" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "P", Desc = "Pending" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "B", Desc = "Blocked" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "D", Desc = "Deactive" });
            dlGenStatuses.Add(new DiscreteLookup() { Category = "GenStatus", Val = "E", Desc = "Expired" });


            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "1", Desc = "System" }); // 1
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "2", Desc = "Super Admin" }); // 2
            dlUserLevels.Add(new DiscreteLookup() { Category = "UserLevel", Val = "3", Desc = "System Admin" });  // 3
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

             
        }



        public static string GetStatusDesc(string oCode)
        {
            switch (oCode)
            {
                case "A": return "Active";
                case "B": return "Blocked";
                case "D": return "Deactive";
                case "P": return "Pending";
                case "E": return "Expired";

                default: return oCode;
            }
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



        //private ChurchModelContext GetClientDBContext() //UserProfile oUserLogged = null)
        //{
        //    var isAuth = this.oUserLogIn_Priv != null;
        //    if (!isAuth) isAuth = SetUserLogged();

        //    if (!isAuth)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (this.oUserLogIn_Priv == null)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (this.oUserLogIn_Priv.UserProfile == null)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //    //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == 4 && c.Status == "A").FirstOrDefault();
        //    if (oClientConfig != null)
        //    {
        //        //// get and mod the conn
        //        //var _clientDBConnString = "";
        //        //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //        //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //        //_clientDBConnString = conn.ConnectionString;

        //        //// test the NEW DB conn
        //        //var _clientContext = new ChurchModelContext(_clientDBConnString);

        //        // var _clientDBConnString = "";
        //        var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //        conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //        conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //        conn.IntegratedSecurity = false; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //        this._clientDBConnString = conn.ConnectionString;

        //        // test the NEW DB conn
        //        var _clientContext = new ChurchModelContext(_clientDBConnString);

        //        if (!_clientContext.Database.CanConnect())
        //            RedirectToAction("LoginUserAcc", "UserLogin");

        //        //// _oLoggedRole = oUserLogIn_Priv.UserRole; 
        //        //this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
        //        //this._oLoggedCB_MSTR = this.oUserLogIn_Priv.ChurchBody;
        //        //this._oLoggedAGO_MSTR = this.oUserLogIn_Priv.AppGlobalOwner;
        //        //this._oLoggedUser.strChurchCode_AGO = this._oLoggedAGO_MSTR != null ? this._oLoggedAGO_MSTR.GlobalChurchCode : "";
        //        //this._oLoggedUser.strChurchCode_CB = this._oLoggedCB_MSTR != null ? this._oLoggedCB_MSTR.GlobalChurchCode : "";

        //        ///// synchronize AGO, CL, CB, CTRY  or @login 
        //        //// this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

        //        ///// get the localized data... using the MSTR data
        //        //this._oLoggedAGO = _clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
        //        //this._oLoggedCB = _clientContext.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId &&
        //        //                        c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();


        //        // load the dash b/f
        //        // LoadClientDashboardValues();

        //        return _clientContext;
        //    }

        //    //
        //    return null;
        //}

        // private bool isUserAuthorized = false;  


        //private string GetCL_DBConnString()
        //{
        //    var isAuth = this.oUserLogIn_Priv != null;
        //    if (!isAuth) isAuth = SetUserLogged();

        //    if (!isAuth)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (this.oUserLogIn_Priv == null)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (this.oUserLogIn_Priv.UserProfile == null)
        //        RedirectToAction("LoginUserAcc", "UserLogin");


        //    if (string.IsNullOrEmpty(this._clientDBConn))
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));

        //    var _clientContext = new ChurchModelContext(this._clientDBConn);
        //    if (_clientContext.Database.CanConnect()) return this._clientDBConn;
        //    else
        //    {
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
        //        _clientContext = new ChurchModelContext(this._clientDBConn);
        //        if (_clientContext.Database.CanConnect()) return this._clientDBConn;
        //        else
        //        {
        //            var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //            if (oClientConfig != null)
        //            {
        //                var _cs = _configuration.GetConnectionString("DefaultConnection");
        //                // get and mod the conn                        
        //                var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
        //                conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //                conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //                /// conn.IntegratedSecurity = false; 
        //                conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //                this._clientDBConn = conn.ConnectionString;

        //                _clientContext = new ChurchModelContext(this._clientDBConn);
        //                if (_clientContext.Database.CanConnect()) return this._clientDBConn;
        //                else
        //                { return null; }
        //            }
        //            else
        //            { return null; }
        //        }
        //    }
        //}

        //private ChurchModelContext GetClientDBContext()  ///(UserProfile oUserLogged)
        //{
        //    var isAuth = this.oUserLogIn_Priv != null;
        //    if (!isAuth) isAuth = SetUserLogged();

        //    if (!isAuth)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (this.oUserLogIn_Priv == null)
        //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (this.oUserLogIn_Priv.UserProfile == null)
        //        RedirectToAction("LoginUserAcc", "UserLogin");


        //    if (string.IsNullOrEmpty(this._clientDBConn))
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));

        //    var _clientContext = new ChurchModelContext(this._clientDBConn);
        //    if (_clientContext.Database.CanConnect()) return _clientContext;
        //    else
        //    {
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
        //        _clientContext = new ChurchModelContext(this._clientDBConn);
        //        if (_clientContext.Database.CanConnect()) return _clientContext;
        //        else
        //        {
        //            var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //            if (oClientConfig != null)
        //            {
        //                var _cs = _configuration.GetConnectionString("DefaultConnection");
        //                // get and mod the conn                        
        //                var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
        //                conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //                conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //                /// conn.IntegratedSecurity = false; 
        //                conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //                this._clientDBConn = conn.ConnectionString;

        //                _clientContext = new ChurchModelContext(this._clientDBConn);
        //                if (_clientContext.Database.CanConnect()) return _clientContext;
        //                else
        //                { return null; }
        //            }
        //            else
        //            { return null; }
        //        }
        //    }

        //    //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == oUserLogged.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //    //if (oClientConfig != null)
        //    //{
        //    //    //// get and mod the conn
        //    //    //var _clientDBConnString = "";
        //    //    //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //    //    //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //    //    //_clientDBConnString = conn.ConnectionString;

        //    //    //// test the NEW DB conn
        //    //    //var _clientContext = new ChurchModelContext(_clientDBConnString);

        //    //    //var _clientDBConnString = "";
        //    //    var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //    //    conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //    //    conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //    //    /// conn.IntegratedSecurity = false; 
        //    //    conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //    //    this._clientDBConnString = conn.ConnectionString;

        //    //    // test the NEW DB conn
        //    //    var _clientContext = new ChurchModelContext(this._clientDBConnString);

        //    //    if (_clientContext.Database.CanConnect())
        //    //        return _clientContext;
        //    //}

        //    //
        //    // return null;
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

                var _connstr_CL = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId); /// this.GetCL_DBConnString();
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

        //private void __LoadClientDashboardValues()
        //{
        //    //get Currency
        //    var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.IsBaseCurrency == true).FirstOrDefault();
        //    ViewData["CB_CurrUsed"] = curr != null ? curr.Acronym : ""; // "GHS"

        //    var clientAGO = _context.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //    var clientCB = _context.ChurchBody.Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.Status == "A").FirstOrDefault();
        //    ///
        //    var qrySuccess = false;
        //    if (clientAGO != null && clientCB != null)
        //    {
        //        var res = (from dummyRes in new List<string> { "X" }
        //                   join tcb_sb in _context.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN") &&
        //                                       c.AppGlobalOwnerId == clientAGO.Id && c.ParentChurchBodyId == clientCB.Id) on 1 equals 1 into _tcb_sb
        //                   // join tcb in clientContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //                   // join tsr in clientContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
        //                   //join tcm in clientContext.ChurchMember.Where(c => c.Status == "A" &&
        //                   //                    c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
        //                   // join tms in clientContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //                   // join tsubs in clientContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //                   // join ttc in clientContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //                   // join tdb in clientContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb

        //                   select new
        //                   {
        //                       cnt_tcb_sb = _tcb_sb.Count(),
        //                       // cnt_tcm = _tcm.Count(),
        //                       ///
        //                       //cnt_tms = _tms.Count(),
        //                       //cnt_tsubs = _tsubs.Count(),
        //                       //cnt_tdb = _tdb.Count(),
        //                       // cnt_ttc = _ttc.Count(),
        //                       //cnt_tcln_d = _tcln_d.Count(),
        //                       //cnt_tcln_a = _tcln_a.Count()
        //                   })
        //                .ToList();
        //        //.ToListAsync();

        //        ///
        //        if (res.Count() > 0)
        //        {
        //            qrySuccess = true;
        //            ViewData["CB_SubCongCount"] = String.Format("{0:N0}", res[0].cnt_tcb_sb);
        //            ViewData["CB_MemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tcm); 
        //            ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tsubs);
        //            ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tdb);
        //            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_a);
        //            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d);
        //            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d); 
        //        }

        //        var resAudits = _masterContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date);
        //        // var cnt_ttc = resAudits.Count();
        //        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", resAudits.Count());


        //        ////String.Format(1234 % 1 == 0 ? "{0:N0}" : "{0:N2}", 1234);
        //        //var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId_Logged && c.ChurchBodyId == oChuBodyId_Logged && c.IsBaseCurrency == true).FirstOrDefault(); 
        //        //oHomeDash.strCurrUsed = curr != null ? curr.Acronym : ""; // "GHS";
        //        //oHomeDash.SupCongCount = String.Format("{0:N0}", 25);
        //        //oHomeDash.MemListCount = String.Format("{0:N0}", 4208); ViewBag.MemListCount = oHomeDash.MemListCount;
        //        //oHomeDash.NewMemListCount = String.Format("{0:N0}", 17); ViewBag.NewMemListCount = oHomeDash.NewMemListCount;
        //        //oHomeDash.NewConvertsCount = String.Format("{0:N0}", 150); ViewBag.NewConvertsCount = oHomeDash.NewConvertsCount;
        //        //oHomeDash.VisitorsCount = String.Format("{0:N0}", 9); ViewBag.VisitorsCount = oHomeDash.VisitorsCount;
        //        //oHomeDash.ReceiptsAmt = String.Format("{0:N2}", 1700);
        //        //oHomeDash.PaymentsAmt = String.Format("{0:N2}", 105.491); 
        //    }

        //    if (!qrySuccess)
        //    {
        //        ViewData["numCB_SubCongCount"] = String.Format("{0:N0}", 0);
        //        ViewData["numCB_MemListCount"] = String.Format("{0:N0}", 0);
        //        ViewData["numCBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
        //        ViewData["numCBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
        //        ViewData["numCBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
        //        ViewData["numCBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
        //        ViewData["numCBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
        //        ///
        //        ViewData["numCBToday_AuditCount"] = String.Format("{0:N0}", 0);
        //    }


        //    /// Load basic values...
        //    ///
        //    /// master control DB
        //    ViewData["strAppName"] = "Rhema-CMS";
        //    ViewData["strAppNameMod"] = "Church Dashboard";
        //    ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(this._oLoggedUser.UserDesc) ? this._oLoggedUser.UserDesc : "[Current user]";
        //    ViewData["oMSTR_AppGloOwnId_Logged"] = this._oLoggedUser.AppGlobalOwnerId;
        //    ViewData["oMSTR_ChurchBodyId_Logged"] = this._oLoggedUser.ChurchBodyId;

        //    ViewData["oCBOrgType_Logged"] = _oLoggedCB.OrgType;  // CH, CN but subscriber may come from other units like Church Office or Church Group HQ

        //    ViewData["strModCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedModCodes);
        //    ViewData["bl_IsModAccessVAA0"] = this.oUserLogIn_Priv.IsModAccessVAA0;
        //    ViewData["bl_IsModAccessDS00"] = this.oUserLogIn_Priv.IsModAccessDS00;
        //    ViewData["bl_IsModAccessAC01"] = this.oUserLogIn_Priv.IsModAccessAC01;
        //    ViewData["bl_IsModAccessMR02"] = this.oUserLogIn_Priv.IsModAccessMR02;
        //    ViewData["bl_IsModAccessCL03"] = this.oUserLogIn_Priv.IsModAccessCL03;
        //    ViewData["bl_IsModAccessCA04"] = this.oUserLogIn_Priv.IsModAccessCA04;
        //    ViewData["bl_IsModAccessFM05"] = this.oUserLogIn_Priv.IsModAccessFM05;
        //    ViewData["bl_IsModAccessRA06"] = this.oUserLogIn_Priv.IsModAccessRA06;
        //    ///                
        //    ViewData["strAssignedRoleCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleCodes);
        //    ViewData["strAssignedRoleNames"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleNames);
        //    ViewData["strAssignedGroupNames"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedGroupNames);
        //    //  ViewData["strAssignedGroupDesc"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedGroupsDesc);
        //    ViewData["strAssignedPermCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedPermCodes);


        //    //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRolesDesc);  //_oLoggedRole.RoleName; // "System Adminitrator";
        //    //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleCodes);  //_oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

        //    ViewData["strAppCurrUserPhoto_Filename"] = this._oLoggedUser.UserPhoto;
        //    ///
        //    /// client control DB
        //    ViewData["oAppGloOwnId_Logged"] = this._oLoggedAGO.Id;
        //    ViewData["oChurchBodyId_Logged"] = this._oLoggedCB.Id;
        //    ViewData["oChurchBodyOrgType_Logged"] = this._oLoggedCB.OrgType;
        //    ViewData["strClientLogo_Filename"] = this._oLoggedAGO?.ChurchLogo;
        //    ViewData["strAppLogo_Filename"] = "~/frontend/dist/img/rhema_logo.png"; // oAppGloOwn?.ChurchLogo;
        //    ViewData["strClientChurchName"] = this._oLoggedAGO.OwnerName;
        //    ViewData["strClientBranchName"] = this._oLoggedCB.Name;
        //    ViewData["strClientChurchLevel"] = !string.IsNullOrEmpty(this._oLoggedCB.ChurchLevel?.CustomName) ? this._oLoggedCB.ChurchLevel?.CustomName : this._oLoggedCB.ChurchLevel?.Name;  // Assembly, Presbytery etc
        //    ViewData["strClientBranchParentName"] = this._oLoggedCB.ParentChurchBody != null ? this._oLoggedCB.ParentChurchBody.Name : "";
        //}



        private async Task LoadClientDashboardValues()  //string clientDBConnString) //, UserProfile oLoggedUser)
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



            //using (var _clientContext = new ChurchModelContext(clientDBConnString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
            //{
            //    var result = _clientContext.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN") &&
            //                                             c.AppGlobalOwnerId == this._oLoggedAGO.Id //&& c.ParentChurchBodyId == this._oLoggedCB.Id
            //                                             ).ToList();

            //    //ViewData["CB_SubCongCount"] = String.Format("{0:N0}", (result != null ? result.Count : 999));
            //    ViewData["CB_MemListCount"] = String.Format("{0:N0}", (result != null ? result.Count : 999));

            //    //if (_clientContext.Database.CanConnect() == false) _clientContext.Database.OpenConnection();
            //    //else if (_clientContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) _clientContext.Database.OpenConnection();

            //    //get Currency
            //    //var curr = _clientContext.Currency.Where(c => c.AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.IsBaseCurrency == true).FirstOrDefault();
            //    //ViewData["CB_CurrUsed"] = curr != null ? curr.Acronym : ""; // "GHS"

            //    //var clientAGO = _clientContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
            //    //var clientCB = _clientContext.ChurchBody.Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.Status == "A").FirstOrDefault();
            //    ///
            //    //var qrySuccess = false;

            //    //if (clientAGO != null && clientCB != null)
            //    //{

            //    var res = await (from dummyRes in new List<string> { "X" }
            //                         join tcb_sb in _clientContext.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN") &&
            //                                             c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.ParentChurchBodyId == this._oLoggedCB.Id) on 1 equals 1 into _tcb_sb

            //                         // join tcb in clientContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
            //                         // join tsr in clientContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
            //                         //join tcm in clientContext.ChurchMember.Where(c => c.Status == "A" &&
            //                         //                    c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
            //                         // join tms in clientContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
            //                         // join tsubs in clientContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
            //                         // join ttc in clientContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
            //                         // join tdb in clientContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb

            //                         select new
            //                         {
            //                             cnt_tcb_sb = _tcb_sb.Count(),
            //                             // cnt_tcm = _tcm.Count(),
            //                             ///
            //                             //cnt_tms = _tms.Count(),
            //                             //cnt_tsubs = _tsubs.Count(),
            //                             //cnt_tdb = _tdb.Count(),
            //                             // cnt_ttc = _ttc.Count(),
            //                             //cnt_tcln_d = _tcln_d.Count(),
            //                             //cnt_tcln_a = _tcln_a.Count()
            //                         })
            //                .ToList().ToListAsync();
            //        //.ToListAsync();

            //        ///
            //        if (res.Count() > 0)
            //        {
            //           // qrySuccess = true;
            //            ViewData["CB_SubCongCount"] = String.Format("{0:N0}", res[0].cnt_tcb_sb);
            //           // ViewData["CB_MemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tcm); //
            //            ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tsubs);
            //         //   ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tdb);
            //            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_a);
            //            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d);
            //            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d); 
            //        }
            //        else
            //        {
            //           ViewData["numCB_SubCongCount"] = String.Format("{0:N0}", 0);
            //          // ViewData["numCB_MemListCount"] = String.Format("{0:N0}", 0);
            //           ViewData["numCBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
            //          //  ViewData["numCBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
            //            ViewData["numCBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
            //            ViewData["numCBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
            //            ViewData["numCBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
            //            ///
            //            ViewData["numCBToday_AuditCount"] = String.Format("{0:N0}", 0);
            //        }

            //       //// var resAudits = _masterContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date);
            //       // var resAudits = await _masterContext.UserAuditTrail.AsNoTracking().Where(c => c.EventDate.Date == DateTime.Now.Date).ToListAsync();

            //       // // var cnt_ttc = resAudits.Count();
            //       // ViewData["TodaysAuditCount"] = String.Format("{0:N0}", resAudits.Count());


            //        ////String.Format(1234 % 1 == 0 ? "{0:N0}" : "{0:N2}", 1234);
            //        //var curr = _clientContext.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId_Logged && c.ChurchBodyId == oChuBodyId_Logged && c.IsBaseCurrency == true).FirstOrDefault(); 
            //        //oHomeDash.strCurrUsed = curr != null ? curr.Acronym : ""; // "GHS";
            //        //oHomeDash.SupCongCount = String.Format("{0:N0}", 25);
            //        //oHomeDash.MemListCount = String.Format("{0:N0}", 4208); ViewBag.MemListCount = oHomeDash.MemListCount;
            //        //oHomeDash.NewMemListCount = String.Format("{0:N0}", 17); ViewBag.NewMemListCount = oHomeDash.NewMemListCount;
            //        //oHomeDash.NewConvertsCount = String.Format("{0:N0}", 150); ViewBag.NewConvertsCount = oHomeDash.NewConvertsCount;
            //        //oHomeDash.VisitorsCount = String.Format("{0:N0}", 9); ViewBag.VisitorsCount = oHomeDash.VisitorsCount;
            //        //oHomeDash.ReceiptsAmt = String.Format("{0:N2}", 1700);
            //        //oHomeDash.PaymentsAmt = String.Format("{0:N2}", 105.491); 
            //   // }

            //    //if (!qrySuccess)
            //    //{
            //    //    ViewData["numCB_SubCongCount"] = String.Format("{0:N0}", 0);
            //    //    ViewData["numCB_MemListCount"] = String.Format("{0:N0}", 0);
            //    //    ViewData["numCBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
            //    //    ViewData["numCBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
            //    //    ViewData["numCBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
            //    //    ViewData["numCBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
            //    //    ViewData["numCBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
            //    //    ///
            //    //    ViewData["numCBToday_AuditCount"] = String.Format("{0:N0}", 0);
            //    //}




            //    //// close connection
            //    //_clientContext.Database.CloseConnection();

            //}


            //using (var _masContext = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
            //{
            //    if (_masContext.Database.CanConnect() == false) _masContext.Database.OpenConnection();
            //    else if (_masContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) _masContext.Database.OpenConnection();

            //    var resAudits = await _masContext.UserAuditTrail.AsNoTracking().Where(c => c.EventDate.Date == DateTime.Now.Date).ToListAsync();
            //    // var cnt_ttc = resAudits.Count();
            //    ViewData["TodaysAuditCount"] = String.Format("{0:N0}", resAudits.Count());


            //    _masContext.Database.CloseConnection();
            //}



            //return new RhemaCMS.Models.ViewModels.vm_app_ven.UserProfileModel();
        }





        //private async Task LoadClientDashboardValues(string clientDBConnString) //, UserProfile oLoggedUser)
        //{
        //    using (var _clientContext = new ChurchModelContext(clientDBConnString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
        //    {
        //        if (_clientContext.Database.CanConnect() == false) _clientContext.Database.OpenConnection();
        //        else if (_clientContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) _clientContext.Database.OpenConnection();

        //        //get Currency
        //        var curr = _clientContext.Currency.Where(c => c.AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.IsBaseCurrency == true).FirstOrDefault();
        //        ViewData["CB_CurrUsed"] = curr != null ? curr.Acronym : ""; // "GHS"

        //        var clientAGO = _clientContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //        var clientCB = _clientContext.ChurchBody.Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.Status == "A").FirstOrDefault();
        //        ///
        //        var qrySuccess = false;
        //        if (clientAGO != null && clientCB != null)
        //        {
        //            var res = await (from dummyRes in new List<string> { "X" }
        //                       join tcb_sb in _clientContext.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN") &&
        //                                           c.AppGlobalOwnerId == clientAGO.Id && c.ParentChurchBodyId == clientCB.Id) on 1 equals 1 into _tcb_sb
        //                       // join tcb in clientContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //                       // join tsr in clientContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
        //                       // join tcm in clientContext.ChurchMember.Where(c => c.Status == "A" && c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
        //                       // join tms in clientContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //                       // join tsubs in clientContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //                       // join ttc in clientContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //                       // join tdb in clientContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb

        //                       select new
        //                       {
        //                           cnt_tcb_sb = _tcb_sb.Count(),
        //                           // cnt_tcm = _tcm.Count(),
        //                           ///
        //                           //cnt_tms = _tms.Count(),
        //                           //cnt_tsubs = _tsubs.Count(),
        //                           //cnt_tdb = _tdb.Count(),
        //                           // cnt_ttc = _ttc.Count(),
        //                           //cnt_tcln_d = _tcln_d.Count(),
        //                           //cnt_tcln_a = _tcln_a.Count()
        //                       })
        //                    .ToList().ToListAsync();
        //            //.ToListAsync();

        //            ///
        //            if (res.Count() > 0)
        //            {
        //                qrySuccess = true;
        //                ViewData["CB_SubCongCount"] = String.Format("{0:N0}", res[0].cnt_tcb_sb);
        //                ViewData["CB_MemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tcm); 
        //                ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tsubs);
        //                ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tdb);
        //                ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_a);
        //                ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d);
        //                ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d); 
        //            }


        //            ////String.Format(1234 % 1 == 0 ? "{0:N0}" : "{0:N2}", 1234);
        //            //var curr = _clientContext.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId_Logged && c.ChurchBodyId == oChuBodyId_Logged && c.IsBaseCurrency == true).FirstOrDefault(); 
        //            //oHomeDash.strCurrUsed = curr != null ? curr.Acronym : ""; // "GHS";
        //            //oHomeDash.SupCongCount = String.Format("{0:N0}", 25);
        //            //oHomeDash.MemListCount = String.Format("{0:N0}", 4208); ViewBag.MemListCount = oHomeDash.MemListCount;
        //            //oHomeDash.NewMemListCount = String.Format("{0:N0}", 17); ViewBag.NewMemListCount = oHomeDash.NewMemListCount;
        //            //oHomeDash.NewConvertsCount = String.Format("{0:N0}", 150); ViewBag.NewConvertsCount = oHomeDash.NewConvertsCount;
        //            //oHomeDash.VisitorsCount = String.Format("{0:N0}", 9); ViewBag.VisitorsCount = oHomeDash.VisitorsCount;
        //            //oHomeDash.ReceiptsAmt = String.Format("{0:N2}", 1700);
        //            //oHomeDash.PaymentsAmt = String.Format("{0:N2}", 105.491); 
        //        }

        //        if (!qrySuccess)
        //        {
        //            ViewData["numCB_SubCongCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCB_MemListCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
        //            ///
        //            ViewData["numCBToday_AuditCount"] = String.Format("{0:N0}", 0);
        //        }


        //        /// Load basic values...
        //        ///
        //        /// master control DB
        //        ViewData["strAppName"] = "Rhema-CMS";
        //        ViewData["strAppNameMod"] = "Church Dashboard";
        //        ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(this._oLoggedUser.UserDesc) ? this._oLoggedUser.UserDesc : "[Current user]";
        //        ViewData["oMSTR_AppGloOwnId_Logged"] = this._oLoggedUser.AppGlobalOwnerId;
        //        ViewData["oMSTR_ChurchBodyId_Logged"] = this._oLoggedUser.ChurchBodyId;

        //        ViewData["oCBOrgType_Logged"] = _oLoggedCB.OrgType;  // CH, CN but subscriber may come from other units like Church Office or Church Group HQ

        //        ViewData["strModCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedModCodes);
        //        ViewData["bl_IsModAccessVAA0"] = this.oUserLogIn_Priv.IsModAccessVAA0;
        //        ViewData["bl_IsModAccessDS00"] = this.oUserLogIn_Priv.IsModAccessDS00;
        //        ViewData["bl_IsModAccessAC01"] = this.oUserLogIn_Priv.IsModAccessAC01;
        //        ViewData["bl_IsModAccessMR02"] = this.oUserLogIn_Priv.IsModAccessMR02;
        //        ViewData["bl_IsModAccessCL03"] = this.oUserLogIn_Priv.IsModAccessCL03;
        //        ViewData["bl_IsModAccessCA04"] = this.oUserLogIn_Priv.IsModAccessCA04;
        //        ViewData["bl_IsModAccessFM05"] = this.oUserLogIn_Priv.IsModAccessFM05;
        //        ViewData["bl_IsModAccessRA06"] = this.oUserLogIn_Priv.IsModAccessRA06;
        //        ///                
        //        ViewData["strAssignedRoleCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleCodes);
        //        ViewData["strAssignedRoleNames"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleNames);
        //        ViewData["strAssignedGroupNames"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedGroupNames);
        //        //  ViewData["strAssignedGroupDesc"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedGroupsDesc);
        //        ViewData["strAssignedPermCodes"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedPermCodes);


        //        //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRolesDesc);  //_oLoggedRole.RoleName; // "System Adminitrator";
        //        //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", this.oUserLogIn_Priv.arrAssignedRoleCodes);  //_oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

        //        ViewData["strAppCurrUserPhoto_Filename"] = this._oLoggedUser.UserPhoto;
        //        ///
        //        /// client control DB
        //        ViewData["oAppGloOwnId_Logged"] = this._oLoggedAGO.Id;
        //        ViewData["oChurchBodyId_Logged"] = this._oLoggedCB.Id;
        //        ViewData["oChurchBodyOrgType_Logged"] = this._oLoggedCB.OrgType;
        //        ViewData["strClientLogo_Filename"] = this._oLoggedAGO?.ChurchLogo;
        //        ViewData["strAppLogo_Filename"] = "~/frontend/dist/img/rhema_logo.png"; // oAppGloOwn?.ChurchLogo;
        //        ViewData["strClientChurchName"] = this._oLoggedAGO.OwnerName;
        //        ViewData["strClientBranchName"] = this._oLoggedCB.Name;
        //        ViewData["strClientChurchLevel"] = !string.IsNullOrEmpty(this._oLoggedCB.ChurchLevel?.CustomName) ? this._oLoggedCB.ChurchLevel?.CustomName : this._oLoggedCB.ChurchLevel?.Name;  // Assembly, Presbytery etc

        //        // close connection
        //        _clientContext.Database.CloseConnection();
        //    }


        //    using (var _masContext = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
        //    {
        //        if (_masContext.Database.CanConnect() == false) _masContext.Database.OpenConnection();
        //        else if (_masContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) _masContext.Database.OpenConnection();

        //        var resAudits = await _masterContext.UserAuditTrail.AsNoTracking().Where(c => c.EventDate.Date == DateTime.Now.Date).ToListAsync();
        //        // var cnt_ttc = resAudits.Count();
        //        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", resAudits.Count());
        //    }

        //   // return new UserProfileModel();
        //}










        //private bool InitializeUserLogging()
        //{
        //    try
        //    {
        //        SetUserLogged();

        //        if (!isCurrValid)
        //        {
        //            ViewData["strUserLoginFailMess"] = "Client user profile validation unsuccessful.";
        //            //RedirectToAction("LoginUserAcc", "UserLogin"); 
        //            return false;
        //        }

        //        if (oUserLogIn_Priv.UserProfile == null)
        //        {
        //            ViewData["strUserLoginFailMess"] = "Client user profile not found. Please try again or contact System Admin";
        //            // RedirectToAction("LoginUserAcc", "UserLogin"); 
        //            return false;
        //        }

        //        // store login in session 
        //        var _oUserPrivilegeCol = oUserLogIn_Priv;
        //        var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
        //        TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();

        //        ///
        //       // _oLoggedRole = oUserLogIn_Priv.UserRole; 
        //        _oLoggedUser = oUserLogIn_Priv.UserProfile;
        //        _oLoggedCB_MSTR = oUserLogIn_Priv.ChurchBody;
        //        _oLoggedAGO_MSTR = oUserLogIn_Priv.AppGlobalOwner;
        //        _oLoggedUser.strChurchCode_AGO = _oLoggedAGO_MSTR != null ? _oLoggedAGO_MSTR.GlobalChurchCode : "";
        //        _oLoggedUser.strChurchCode_CB = _oLoggedCB_MSTR != null ? _oLoggedCB_MSTR.GlobalChurchCode : "";

        //        this._context = GetClientDBContext(_oLoggedUser);

        //        if (this._context == null)
        //        {
        //            ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
        //            // return RedirectToAction("LoginUserAcc", "UserLogin"); 
        //            ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
        //            ///
        //            return false;
        //            // RedirectToAction("Index", "Home");  //return View(oHomeDash);
        //        }

        //        this._strClientConn = _context.Database.GetDbConnection().ConnectionString;

        //        //// store ctx in session 
        //        //var _tempContext = this._context;
        //        //var _ctx = Newtonsoft.Json.JsonConvert.SerializeObject(_tempContext);
        //        //TempData["UserLogIn_oDBContext_Client"] = _ctx; TempData.Keep();

        //        /// synchronize AGO, CL, CB, CTRY  or @login 

        //        /// get the localized data... using the MSTR data
        //       _oLoggedAGO = _context.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == _oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == _oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
        //       _oLoggedCB = _context.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.MSTR_AppGlobalOwnerId == _oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == _oLoggedUser.ChurchBodyId &&
        //                                                c.GlobalChurchCode == _oLoggedUser.strChurchCode_CB).FirstOrDefault();

        //        if (_oLoggedAGO == null || _oLoggedCB == null)
        //        {
        //            ViewData["strUserLoginFailMess"] = "Client Church unit details could not be verified. Please try again or contact System Admin";
        //            ///
        //            // RedirectToAction("LoginUserAcc", "UserLogin"); 
        //            return false;
        //        }

        //        /// master control DB
        //        ViewData["strAppName"] = "Rhema-CMS";
        //        ViewData["strAppNameMod"] = "Church Dashboard";
        //        ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(_oLoggedUser.UserDesc) ? _oLoggedUser.UserDesc : "[Current user]";
        //        ViewData["oMSTR_AppGloOwnId_Logged"] = _oLoggedUser.AppGlobalOwnerId;
        //        ViewData["oMSTR_ChurchBodyId_Logged"] = _oLoggedUser.ChurchBodyId;

        //        ViewData["oCBOrgType_Logged"] = _oLoggedCB.OrgType;  // CH, CN but subscriber may come from other units like Church Office or Church Group HQ

        //        ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);                
        //        ViewData["bl_IsModAccessVAA0"] = oUserLogIn_Priv.IsModAccessVAA0;
        //        ViewData["bl_IsModAccessDS00"] = oUserLogIn_Priv.IsModAccessDS00;
        //        ViewData["bl_IsModAccessAC01"] = oUserLogIn_Priv.IsModAccessAC01;
        //        ViewData["bl_IsModAccessMR02"] = oUserLogIn_Priv.IsModAccessMR02;
        //        ViewData["bl_IsModAccessCL03"] = oUserLogIn_Priv.IsModAccessCL03;
        //        ViewData["bl_IsModAccessCA04"] = oUserLogIn_Priv.IsModAccessCA04;
        //        ViewData["bl_IsModAccessFM05"] = oUserLogIn_Priv.IsModAccessFM05;
        //        ViewData["bl_IsModAccessRA06"] = oUserLogIn_Priv.IsModAccessRA06;
        //        ///                
        //        ViewData["strAssignedRoleCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);
        //        ViewData["strAssignedRoleNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleNames);
        //        ViewData["strAssignedGroupNames"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames);
        //      //  ViewData["strAssignedGroupDesc"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc);
        //        ViewData["strAssignedPermCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedPermCodes);

        //        //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRolesDesc);  // _oLoggedRole.RoleName; // "System Adminitrator";
        //        //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);  //_oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

        //        ViewData["strAppCurrUserPhoto_Filename"] = _oLoggedUser.UserPhoto;                
        //        ViewData["strAppLogo_Filename"] = "df_rhema.jpg"; //"~/img_db/df_rhema.jpg"; // oAppGloOwn?.ChurchLogo;
        //        ///
        //        /// client control DB
        //        ViewData["oAppGloOwnId_Logged"] = _oLoggedAGO.Id;
        //        ViewData["oChurchBodyId_Logged"] = _oLoggedCB.Id;
        //        ViewData["oChurchBodyOrgType_Logged"] = _oLoggedCB.OrgType;
        //        ViewData["strClientLogo_Filename"] = _oLoggedAGO?.ChurchLogo;

        //        ViewData["strClientChurchName"] = _oLoggedAGO.OwnerName;
        //        ViewData["strClientBranchName"] = _oLoggedCB.Name;
        //        ViewData["strClientChurchLevel"] = !string.IsNullOrEmpty(_oLoggedCB.ChurchLevel?.CustomName) ? _oLoggedCB.ChurchLevel?.CustomName : _oLoggedCB.ChurchLevel?.Name;  // Assembly, Presbytery etc

        //        // refreshValues...
        //        // LoadClientDashboardValues(this._strClientConn, this._oLoggedUser);

        //        return true;
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private bool CheckCurrentClientDBContext()
        //{
        //    try
        //    {
        //        SetUserLogged();
        //        if (!isCurrValid)
        //        {
        //            ViewData["strUserLoginFailMess"] = "Client user profile validation unsuccessful.";
        //            //RedirectToAction("LoginUserAcc", "UserLogin"); 
        //            return false;
        //        }

        //        if (oUserLogIn_Priv.UserProfile == null)
        //        {
        //            ViewData["strUserLoginFailMess"] = "Client user profile not found. Please try again or contact System Admin";
        //            // RedirectToAction("LoginUserAcc", "UserLogin"); 
        //            return false;
        //        }

        //        if (TempData == null)
        //        {
        //            var httpContext = _httpContextAccessor.HttpContext;
        //            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

        //            if (tempData.ContainsKey("UserLogIn_oDBContext_Client"))
        //            {
        //                var _con = tempData["UserLogIn_oDBContext_Client"] as string;
        //                if (string.IsNullOrEmpty(_con)) RedirectToAction("LoginUserAcc", "UserLogin");
        //                // De serialize the string to object
        //                this._context = Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchModelContext>(_con);
        //                if (this._context == null)
        //                    this._context = GetClientDBContext(this._oLoggedUser);
        //            }

        //            else
        //            {
        //                this._context = GetClientDBContext(this._oLoggedUser);
        //            }
        //        }
        //        else
        //        {
        //            if (TempData.ContainsKey("UserLogIn_oDBContext_Client"))
        //            {
        //                var _con1 = TempData["UserLogIn_oDBContext_Client"] as string;
        //                if (string.IsNullOrEmpty(_con1)) RedirectToAction("LoginUserAcc", "UserLogin");
        //                // De serialize the string to object
        //                this._context = Newtonsoft.Json.JsonConvert.DeserializeObject<ChurchModelContext>(_con1);
        //                //
        //                if (this._context == null)
        //                    this._context = GetClientDBContext(this._oLoggedUser);
        //            }

        //            else
        //            {
        //                this._context = GetClientDBContext(this._oLoggedUser);
        //            }

        //        }

        //        //// store ctx in session 
        //        //var _tempContext = this._context; 
        //        //var _ctx = Newtonsoft.Json.JsonConvert.SerializeObject(_tempContext);
        //        //TempData["UserLogIn_oDBContext_Client"] = _ctx; TempData.Keep();

        //        if (this._context != null) this._strClientConn = _context.Database.GetDbConnection().ConnectionString;
        //        ///
        //        return (this._context != null);
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private bool userAuthorized = false;
        //private void SetUserLogged()
        //{
        //    if (TempData == null)
        //    {
        //        var httpContext = _httpContextAccessor.HttpContext;
        //        var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);

        //        if (tempData.ContainsKey("UserLogIn_oUserPrivCol"))
        //        {
        //            var tempPrivList = tempData["UserLogIn_oUserPrivCol"] as string;
        //            if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
        //            // De serialize the string to object
        //            this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
        //            //
        //            isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
        //            if (isCurrValid)
        //            {
        //                this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;

        //                //ViewBag.oAppGloOwnLogged = oUserLogIn_Priv.AppGlobalOwner;
        //                //ViewBag.oChuBodyLogged = oUserLogIn_Priv.ChurchBody;
        //                //ViewBag.oUserLogged = oUserLogIn_Priv.UserProfile;

        //                // check permission for Core life...  given the sets of permissions
        //                userAuthorized = oUserLogIn_Priv.UserSessionPermList.Count > 0; //(oUserLogIn_Priv.Find(x => x.PermissionName == "_A0__System_Administration" || x.PermissionName == "xxx") != null);
        //            }
        //        }

        //        else RedirectToAction("LoginUserAcc", "UserLogin");
        //    }
        //    else
        //    {
        //        if (TempData.ContainsKey("UserLogIn_oUserPrivCol"))
        //        {
        //            var tempPrivList = TempData["UserLogIn_oUserPrivCol"] as string;
        //            if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
        //            // De serialize the string to object
        //            this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
        //            //
        //            isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
        //            if (isCurrValid)
        //            {
        //                this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
        //                //ViewBag.oAppGloOwnLogged = oUserLogIn_Priv.AppGlobalOwner;
        //                //ViewBag.oChuBodyLogged = oUserLogIn_Priv.ChurchBody;
        //                //ViewBag.oUserLogged = oUserLogIn_Priv.UserProfile;

        //                // check permission for Core life...  given the sets of permissions
        //                userAuthorized = true; //oUserLogIn_Priv.Count > 0; //(oUserLogIn_Priv.Find(x => x.PermissionName == "_A0__System_Administration" || x.PermissionName == "xxx") != null);
        //            }
        //        }

        //        else RedirectToAction("LoginUserAcc", "UserLogin");
        //    }
        //}







        private string GetDefaultCountryInfo()   // GHA--Ghana--GHC--GHS
        {
            try
            {
                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return "";  /// View("_ErrorPage");
                    }
                }

                if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                { RedirectToAction("LoginUserAcc", "UserLogin"); return "";  }



                this.strCountryCode_dflt = (string)null; this.strCountryName_dflt = ""; this.strCountryCURR1_dflt = ""; this.strCountryCURR2_dflt = "";
                var strCTRYInfo = "";
                if (TempData.ContainsKey("oDefaultCountryInfo"))
                {
                    strCTRYInfo = TempData["oDefaultCountryInfo"] as string;
                    var isSuccess = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
                    if (isSuccess)
                    {
                        strCTRYInfo = this.strCountryCode_dflt + "--" + this.strCountryName_dflt + "--" + this.strCountryCURR1_dflt + "--" + this.strCountryCURR2_dflt;  // GHA--Ghana--GHC--GHS
                        TempData["oDefaultCountryInfo"] = strCTRYInfo; TempData.Keep();
                        return strCTRYInfo;  // GHA--Ghana--GHC--GHS
                    }
                }

                // country -- default if not specified
                var oCTRY_List = _context.CountryCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.ChurchBodyId == this._oLoggedCB.Id).ToList();
                var oCTRY_List_d = oCTRY_List.Where(c => c.IsDefaultCountry == true).FirstOrDefault();
                if (oCTRY_List_d == null && oCTRY_List.Count > 0) oCTRY_List_d = oCTRY_List[0];
                if (oCTRY_List_d != null)
                {
                    this.strCountryCode_dflt = oCTRY_List_d.CtryAlpha3Code;
                    this.strCountryName_dflt = oCTRY_List_d.Country != null ? (!string.IsNullOrEmpty(oCTRY_List_d.Country.EngName) ? oCTRY_List_d.Country.EngName : oCTRY_List_d.Country.CtryAlpha3Code) : "";
                    this.strCountryCURR1_dflt = oCTRY_List_d.Country != null ? oCTRY_List_d.Country.CurrSymbol : "";
                    this.strCountryCURR2_dflt = oCTRY_List_d.Country != null ? oCTRY_List_d.Country.Curr3LISOSymbol : "";
                }
                //strCountryCode_dflt = oCTRY_List_d != null ? oCTRY_List_d.CtryAlpha3Code : "";
                //strCountry_dflt = oCTRY_List_d != null ? (oCTRY_List_d.Country != null ? (!string.IsNullOrEmpty(oCTRY_List_d.Country.EngName) ? oCTRY_List_d.Country.EngName : oCTRY_List_d.Country.CtryAlpha3Code) : "") : "";

                //  ...
                strCTRYInfo = this.strCountryCode_dflt + "--" + this.strCountryName_dflt + "--" + this.strCountryCURR1_dflt + "--" + this.strCountryCURR2_dflt;  // GHA--Ghana--GHC--GHS
                TempData["oDefaultCountryInfo"] = strCTRYInfo; TempData.Keep();
                return strCTRYInfo;  // GHA--Ghana--GHC--GHS
            }

            catch (Exception ex)
            {
                throw;
            }
        }


        //private ChurchModelContext GetClientDBContext(UserProfile oUserLogged)
        //{
        //    var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == oUserLogged.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //    if (oClientConfig != null)
        //    {
        //        //// get and mod the conn
        //        //var _clientDBConnString = "";
        //        //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //        //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //        //_clientDBConnString = conn.ConnectionString;

        //        //// test the NEW DB conn
        //        //var _clientContext = new ChurchModelContext(_clientDBConnString);

        //        var _clientDBConnString = "";
        //        var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //        conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //        conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //        conn.IntegratedSecurity = false; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //        _clientDBConnString = conn.ConnectionString;

        //        // test the NEW DB conn
        //        var _clientContext = new ChurchModelContext(_clientDBConnString);

        //        if (_clientContext.Database.CanConnect())
        //            return _clientContext;
        //    }

        //    //
        //    return null;
        //}


        //private void LoadClientDashboardValues(string clientDBConnString, UserProfile oLoggedUser)
        //{
        //    // using (var dashContext = new ChurchModelContext(clientDBConnString))
        //    using (var clientContext = new ChurchModelContext(clientDBConnString)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
        //    {
        //        if (clientContext.Database.CanConnect() == false) clientContext.Database.OpenConnection();
        //        else if (clientContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) clientContext.Database.OpenConnection();

        //        //get Currency
        //        var curr = clientContext.Currency.Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == oLoggedUser.ChurchBodyId && c.IsBaseCurrency == true).FirstOrDefault();
        //        ViewData["CB_CurrUsed"] = curr != null ? curr.Acronym : ""; // "GHS"

        //        var clientAGO = clientContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //        var clientCB = clientContext.ChurchBody.Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == oLoggedUser.ChurchBodyId && c.Status == "A").FirstOrDefault();
        //        ///
        //        var qrySuccess = false;
        //        if (clientAGO != null && clientCB != null)
        //        {
        //            var res = (from dummyRes in new List<string> { "X" }
        //                       join tcb_sb in clientContext.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN") &&
        //                                           c.AppGlobalOwnerId == clientAGO.Id && c.ParentChurchBodyId == clientCB.Id) on 1 equals 1 into _tcb_sb
        //                       // join tcb in clientContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //                       // join tsr in clientContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
        //                       //join tcm in clientContext.ChurchMember.Where(c => c.Status == "A" &&
        //                       //                    c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
        //                       // join tms in clientContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //                       // join tsubs in clientContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //                       // join ttc in clientContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //                       // join tdb in clientContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb

        //                       select new
        //                       {
        //                           cnt_tcb_sb = _tcb_sb.Count(),
        //                           // cnt_tcm = _tcm.Count(),
        //                           ///
        //                           //cnt_tms = _tms.Count(),
        //                           //cnt_tsubs = _tsubs.Count(),
        //                           //cnt_tdb = _tdb.Count(),
        //                           // cnt_ttc = _ttc.Count(),
        //                           //cnt_tcln_d = _tcln_d.Count(),
        //                           //cnt_tcln_a = _tcln_a.Count()
        //                       })
        //                    .ToList();
        //            //.ToListAsync();

        //            ///
        //            if (res.Count() > 0)
        //            {
        //                qrySuccess = true;
        //                ViewData["CB_SubCongCount"] = String.Format("{0:N0}", res[0].cnt_tcb_sb);
        //                ViewData["CB_MemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tcm); 
        //                ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 100); // res[0].cnt_tsubs);
        //                ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tdb);
        //                ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_a);
        //                ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d);
        //                ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 100); //res[0].cnt_tcln_d); 
        //            }

        //            var resAudits = _masterContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date);
        //            // var cnt_ttc = resAudits.Count();
        //            ViewData["TodaysAuditCount"] = String.Format("{0:N0}", resAudits.Count());


        //            ////String.Format(1234 % 1 == 0 ? "{0:N0}" : "{0:N2}", 1234);
        //            //var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId_Logged && c.ChurchBodyId == oChuBodyId_Logged && c.IsBaseCurrency == true).FirstOrDefault(); 
        //            //oHomeDash.strCurrUsed = curr != null ? curr.Acronym : ""; // "GHS";
        //            //oHomeDash.SupCongCount = String.Format("{0:N0}", 25);
        //            //oHomeDash.MemListCount = String.Format("{0:N0}", 4208); ViewBag.MemListCount = oHomeDash.MemListCount;
        //            //oHomeDash.NewMemListCount = String.Format("{0:N0}", 17); ViewBag.NewMemListCount = oHomeDash.NewMemListCount;
        //            //oHomeDash.NewConvertsCount = String.Format("{0:N0}", 150); ViewBag.NewConvertsCount = oHomeDash.NewConvertsCount;
        //            //oHomeDash.VisitorsCount = String.Format("{0:N0}", 9); ViewBag.VisitorsCount = oHomeDash.VisitorsCount;
        //            //oHomeDash.ReceiptsAmt = String.Format("{0:N2}", 1700);
        //            //oHomeDash.PaymentsAmt = String.Format("{0:N2}", 105.491); 
        //        }

        //        if (!qrySuccess)
        //        {
        //            ViewData["numCB_SubCongCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCB_MemListCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
        //            ViewData["numCBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
        //            ///
        //            ViewData["numCBToday_AuditCount"] = String.Format("{0:N0}", 0);
        //        }

        //        // close connection
        //        clientContext.Database.CloseConnection();
        //    }
        //}

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
        private static bool IsAncestor_ChurchBody(ChurchBody oAncestorChurchBody, ChurchBody oCurrChurchBody)
        {
            if (oAncestorChurchBody == null || oCurrChurchBody == null) return false;
            //string ChurchCodeFullPath { get; set; }  //R0000-0000-0000-0000-0000-0000 

            if (oAncestorChurchBody.Id == oCurrChurchBody.ParentChurchBodyId) return true;
            if (string.IsNullOrEmpty(oAncestorChurchBody.RootChurchCode) || string.IsNullOrEmpty(oCurrChurchBody.RootChurchCode)) return false;
            if (string.Compare(oAncestorChurchBody.RootChurchCode, oCurrChurchBody.RootChurchCode) == 0) return true;

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





        private RhemaCMS.Models.ViewModels.vm_app_ven.UserProfileModel GetInitialClientSetupList()
        {
            try
            {
                var arrData = "";
                arrData = TempData.ContainsKey("oVmUPModel") ? TempData["oVmUPModel"] as string : arrData;
                var vm = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<RhemaCMS.Models.ViewModels.vm_app_ven.UserProfileModel>(arrData) : null;
                ///
                if (vm != null) return vm;


                /// once it gets to this point... then the details not found!
                return new RhemaCMS.Models.ViewModels.vm_app_ven.UserProfileModel();
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Index
      //  public ActionResult Index_CL_UP(int? setIndex = 0, int? subSetIndex = 0, bool loadSectionOnly = false)  //, int filterIndex = 1, int pageIndex = 1, int? numCodeCriteria_1 = (int?)null, string strCodeCriteria_2 = null)  // , int? subSetIndex = 0  int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        public IActionResult Index_CL_UP(int? setIndex = 0, int? subSetIndex = 0, bool loadSectionOnly = false)  //, int filterIndex = 1, int pageIndex = 1, int? numCodeCriteria_1 = (int?)null, string strCodeCriteria_2 = null)  // , int? subSetIndex = 0  int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            try
            {

                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return View("_ErrorPage");
                    }
                }

                if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                { RedirectToAction("LoginUserAcc", "UserLogin"); }


                if (!loadSectionOnly)
                    _ = this.LoadClientDashboardValues(); //// this._clientDBConnString);


                // SetUserLogged();

                //load the dash  ... main layout stuff
                //if (!loadSectionOnly)
                //    await LoadClientDashboardValues(this._clientDBConnString); //await this.LoadClientDashboardValues(this._clientDBConnString, this._oLoggedUser);  //LoadClientDashboardValues();


                //if (!InitializeUserLogging())
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                var oAGO_MSTR = _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId);
                var oCB_MSTR = _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.MSTR_AppGlobalOwnerId && c.Id == this._oLoggedCB.MSTR_ChurchBodyId).FirstOrDefault();
                if (oAGO_MSTR == null || oCB_MSTR == null)  // || oCU_Parent == null church units may be networked...
                { return PartialView("_ErrorPage"); }

                // var proScope = "C";
                //get all member from congregation
                var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id && c.ChurchBodyId == this._oLoggedCB.Id).ToList();

                // get all users from client-congregation  //.Include(t => t.ContactInfo)
                var oUP_List = (from t_up in _masterContext.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_MSTR.Id && c.ChurchBodyId == oCB_MSTR.Id && c.ProfileScope == "C")
                                select t_up).ToList();

                //oUP_List = (
                //             from t_up in oUP_List
                //             from t_cm in oCMList.Where(c => c.Id == t_up.ChurchMemberId).DefaultIfEmpty()).ToList();

                var oUPModel_List = (
                       from t_up in oUP_List
                       from t_ago in _masterContext.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_up.AppGlobalOwnerId)  ////.Include(t => t.AppGlobalOwner)  && c.Status == "A"
                       from t_cb in _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel)
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.Id == t_up.ChurchBodyId) //.DefaultIfEmpty()   
                       
                            //from t_upr in _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                       //     .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&   // ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                       //          ((c.ChurchBody.OrgType == "CH" && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                       //           (c.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15))).DefaultIfEmpty()   // ((proScope == "C" && subScope == "A" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST")) && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))                                     
                       //from t_upg in _masterContext.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                       //     .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       
                       from t_up_o in _masterContext.UserProfile.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.ProfileScope == "C" && c.Id == t_up.OwnerUserId).DefaultIfEmpty()
                       
                            //from t_cm in oCMList.Where(c => c.Id == t_up.ChurchMemberId).DefaultIfEmpty()
                       
                       from t_up_cbu in _masterContext.UserProfile.AsNoTracking() 
                            .Where(c => c.AppGlobalOwnerId==null && c.ChurchBodyId==null && c.Id == t_up.CreatedByUserId).DefaultIfEmpty()

                           //   from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                           //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                           //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                           //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                           //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()

                           //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                           //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                       select new Models.ViewModels.vm_app_ven.UserProfileModel()
                       {
                           oAppGloOwnId = t_cb.AppGlobalOwnerId,
                           oAppGlobalOwn = t_ago, //t_cb.AppGlobalOwner,
                           oChurchBodyId = t_cb.Id,
                           /// 
                           oChurchBody = t_cb,
                           strChurchBody = t_cb.Name,
                           strAppGlobalOwn = t_ago.OwnerName, // + (!string.IsNullOrEmpty(t_ago.OwnerName) ? (t_ci_ago != null ? (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "") : "") : ""),

                           oUserProfile = t_up,

                           isVendorOwned = t_up.CreatedByUserId == null || t_up_cbu != null,
                           strChurchLevel = t_cb.ChurchLevel != null ? (!string.IsNullOrEmpty(t_cb.ChurchLevel.CustomName) ? t_cb.ChurchLevel.CustomName : t_cb.ChurchLevel.Name) : "",

                          // strUserRoleName = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleName : "") : "",
                         //  numUserRoleLevel = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleLevel : (int?)null) : (int?)null,

                           /// strAppCurrUser_RoleCateg = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleType : "") : "",
                           //  strUserGroupName = t_upg != null ? (t_upg.UserGroup != null ? t_upg.UserGroup.GroupName : "") : "",

                           strOwnerUser = t_up_o != null ? t_up_o.UserDesc : "",
                           strUserProfile = t_up.UserDesc,
                           strUserStatus = GetStatusDesc(t_up.UserStatus),
                          
                           // strChurchMember = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc,
                           
                           strSTRT = t_up.Strt != null ? DateTime.Parse(t_up.Strt.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "",
                           strEXPR = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "",
                            
                           ///
                           
                           lsUserRoles = (from t_upr in _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&   // ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                                            (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                                             (c.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15))) //.DefaultIfEmpty()
                                          from t_ur in _masterContext.UserRole.AsNoTracking()
                                            .Where(c => c.AppGlobalOwnerId == t_upr.AppGlobalOwnerId && c.ChurchBodyId == t_upr.ChurchBodyId && c.Id == t_upr.UserRoleId)
                                          select t_ur ).ToList(),
                           lsUserGroups = (from t_upg in _masterContext.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id) //.DefaultIfEmpty()
                                           from t_ur in _masterContext.UserGroup.AsNoTracking()
                                            .Where(c => c.AppGlobalOwnerId == t_upg.AppGlobalOwnerId && c.ChurchBodyId == t_upg.ChurchBodyId && c.Id == t_upg.UserGroupId)
                                          select t_ur ).ToList()

                       })
                  .OrderBy(c => (c.oUserProfile != null ? c.oUserProfile.ProfileLevel : null))
                  .ThenBy(c => c.strUserRoleName).ThenBy(c => c.strUserProfile) // .Distinct()
                  .ToList();

                var oUPModel = new Models.ViewModels.vm_app_ven.UserProfileModel();

                // refine group / role list
                var arrRoleCodes = new ArrayList(); var arrGroupCodes = new ArrayList();
                foreach (var userPro in oUPModel_List)
                {
                    // refine role list
                    userPro.arrAssignedRoleNames = new List<string>();
                    foreach (var userRole in userPro.lsUserRoles)
                    { 
                        if (!arrRoleCodes.Contains(userRole.RoleType))
                        { 
                            userPro.arrAssignedRoleCodes.Add(userRole.RoleType);
                            userPro.arrAssignedRoleNames.Add(userRole.RoleName);
                        }
                        arrRoleCodes.Add(userRole.RoleType);
                    }

                    //refine group list
                    userPro.arrAssignedGroupNames = new List<string>();
                    foreach (var userGroup in userPro.lsUserGroups)
                    {
                        if (!arrGroupCodes.Contains(userGroup.GroupType))
                        {
                            userPro.arrAssignedGroupCodes.Add(userGroup.GroupType);
                            userPro.arrAssignedGroupNames.Add(userGroup.GroupName);
                        }
                        arrGroupCodes.Add(userGroup.GroupType);
                    }

                    userPro.strUserRoleName = String.Join(", ", userPro.arrAssignedRoleNames);
                    userPro.strUserGroupName = String.Join(", ", userPro.arrAssignedGroupNames);
                }

                  

                oUPModel.oAppGloOwnId = oAGO_MSTR.Id;
                oUPModel.oAppGlobalOwn = oAGO_MSTR;
                oUPModel.oChurchBodyId = oCB_MSTR.Id;
                oUPModel.oChurchBody = oCB_MSTR;
                ///
                oUPModel.oChurchBodyId_Logged = this._oLoggedCB.MSTR_ChurchBodyId;
                oUPModel.oChurchBodyId_Logged_CLNT = this._oLoggedCB.Id;  
                oUPModel.oAppGloOwnId_Logged = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                oUPModel.oAppGloOwnId_Logged_CLNT = this._oLoggedAGO.Id;
                oUPModel.oUserId_Logged = _oLoggedUser.Id;

                //oUPModel.pageIndex = 1;
                //oUPModel.filterIndex = filterIndex;
                oUPModel.setIndex = (int)setIndex;
               // oUPModel.subSetIndex = (int)subSetIndex;
                /// 
                oUPModel.lsUserProfileModels = oUPModel_List;
                ViewData["oUPModel_List"] = oUPModel.lsUserProfileModels;

                var strDesc = "User Profile";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                oUPModel.strCurrTask = strDesc;


                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUPModel);
                TempData["oVmCSPModel"] = _oUPModel; TempData.Keep();



                if (loadSectionOnly)
                    return PartialView("_vwIndex_CL_UP", oUPModel);

                else
                    return View("Index_CL_UP", oUPModel);
            }

            catch (Exception ex)
            {
                throw;
                ////page not found error
                //Response.StatusCode = 500;
                //return View("_ErrorPage");
            }
        }
         
        [HttpGet]  //     public IActionResult AddOrEdit_UP(int? oAppGloOwnId = null, int? oChurchBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId = null) //, int pageIndex = 1)
        public IActionResult AddOrEdit_CL_UP(int id = 0, int? oAppGloOwnId = null, int? oChurchBodyId = null, int? oUserId = null, int setIndex = 0)
        {

            if (this._context == null)
            {
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }

            if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            { RedirectToAction("LoginUserAcc", "UserLogin"); }


            //if (!InitializeUserLogging())
            //    return RedirectToAction("LoginUserAcc", "UserLogin");

            // Client
            if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
            if (oChurchBodyId == null) oChurchBodyId = this._oLoggedCB.Id;

            // MSTR
            if (oUserId == null) oUserId = this._oLoggedUser.Id;
            var oAGO_MSTR = _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId);
            var oCB_MSTR = _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.MSTR_AppGlobalOwnerId && c.Id == this._oLoggedCB.MSTR_ChurchBodyId).FirstOrDefault();
            if (oAGO_MSTR == null || oCB_MSTR == null)  // || oCU_Parent == null church units may be networked...
            { return PartialView("_ErrorPage"); }


            var _tm = DateTime.Now;

            //// check permission 
            //var _oUserPrivilegeCol = oUserLogIn_Priv;
            //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
            //TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
            //
  //TempData.Keep();  //  if (setIndex == 0) return oUPModel;

            //subSetIndex: 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
            // setIndex: 1-Vendor-any , 2-Client - CH/CF, 3- Client - any/all
            //var proScope = "C"; // setIndex == 1 ? "V" : "C";               // V - vendor, C - Client
            //var subScope = setIndex == 2 ? "D" : setIndex == 3 ? "A" : "";      // D - denomination, A - all

            //if (subSetIndex >= 1 && subSetIndex <= 5) { proScope = "V"; subScope = ""; }
            //else if (subSetIndex == 6 || subSetIndex == 11) { proScope = "C"; subScope = "D"; }
            //else if (subSetIndex >= 6 && subSetIndex <= 15) { proScope = "C"; subScope = "A"; }

            var strDesc = "User Profile";
            var _userTask = "Attempted accessing/modifying " + strDesc.ToLower();

            var oUPModel = new Models.ViewModels.vm_app_ven.UserProfileModel(); 
            if (id == 0)
            {   //create user and assign ROLE / GROUP --- with Privileges
                 
                var oUser = new UserProfile();
                oUser.AppGlobalOwnerId = oAGO_MSTR.Id; // oAppGloOwnId;
                oUser.ChurchBodyId = oCB_MSTR.Id; // oChurchBodyId;
                oUser.oCBChurchLevelId = oCB_MSTR.ChurchLevelId;
                oUser.Strt = _tm;
                oUser.ResetPwdOnNextLogOn = true;

                //oUP_MDL.oUserProfile.CountryId = oCurrCtryId;

                oUser.UserStatus = "A";   // A-ctive...D-eactive   
                oUser.ProfileScope = "C";   ;
                // oUser.ProfileLevel = subSetIndex;  //remember to change the profile level when the ROLE type is changed                
                oUser.UserScope = "I";

                //if (oUser.ProfileLevel >= 1 && oUser.ProfileLevel <= 5) // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 11-CF_ADMN
                //{
                //    oUser.UserScope = "E";  // I-internal, E-external
                //    if (oUser.ProfileLevel == 2) { oUser.Username = "supadmin"; oUser.UserDesc = "Super Admin"; }
                //}
                //else // I-internal, E-external [manually config]
                //{
                //    if (oAppGloOwnId == null) // || oCurrChuBodyId == null)
                //    { Response.StatusCode = 500; return PartialView("_ErrorPage"); }

                //    //var oAGO = _context.MSTRAppGlobalOwner.Find(oAppGloOwnId);
                //    //var oCB = _context.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();

                //    //if (oAppGloOwnId == null || oCurrChuBodyId == null)
                //    //{ Response.StatusCode = 500; return PartialView("_ErrorPage"); }

                //    oUser.UserScope = "I";
                //    oUPModel.numCLIndex = _context.MSTRChurchLevel.Count(c => c.AppGlobalOwnerId == oAppGloOwnId);  //use what's configured... not digit at AGO

                //    oUser.numCLIndex = oUPModel.numCLIndex; // _context.ChurchLevel.Count(c => c.AppGlobalOwnerId == oAppGloOwnId);
                //}

                _userTask = "Attempted creating new " + strDesc.ToLower(); // + ", " + oUPModel.oUserProfile.UserDesc; 
                 
                oUPModel.strChurchBody = oCB_MSTR.Name; //this._oLoggedCB.Name; 
                oUPModel.strChurchLevel = oCB_MSTR.ChurchLevel != null ? (!string.IsNullOrEmpty(oCB_MSTR.ChurchLevel.CustomName) ? oCB_MSTR.ChurchLevel.CustomName : oCB_MSTR.ChurchLevel.Name) : "";
                 
                oUPModel.oUserProfile = oUser;
            }

            else
            {
                // var proScope = "C";
                //get all member from congregation
                var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId).ToList();

                // get all users from client-congregation  //.Include(t => t.ContactInfo)
                var oUP_List = (from t_up in _masterContext.UserProfile.AsNoTracking().Where(c => c.Id == id && c.AppGlobalOwnerId == oAGO_MSTR.Id && c.ChurchBodyId == oCB_MSTR.Id && c.ProfileScope == "C")
                                select t_up).ToList();

                oUPModel = ( 
                       from t_up in oUP_List
                       from t_ago in _masterContext.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_up.AppGlobalOwnerId)  ////.Include(t => t.AppGlobalOwner)  && c.Status == "A"
                       from t_cb in _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel)
                           .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.Id == t_up.ChurchBodyId) //.DefaultIfEmpty()   
                       from t_upr in _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                           .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&   // ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                                (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel < 11) ||
                                 (c.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel < 15))).DefaultIfEmpty()   // ((proScope == "C" && subScope == "A" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST")) && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))                                     
                       from t_upg in _masterContext.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                           .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       from t_up_o in _masterContext.UserProfile.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.ProfileScope == "C" && c.Id == t_up.OwnerUserId).DefaultIfEmpty()
                       from t_cm in oCMList.Where(c => c.Id == t_up.ChurchMemberId).DefaultIfEmpty()
                       from t_up_cbu in _masterContext.UserProfile.AsNoTracking()
                             .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == t_up.CreatedByUserId).DefaultIfEmpty()

                           //   from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                           //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                           //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                           //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                           //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()

                           //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                           //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                       select new Models.ViewModels.vm_app_ven.UserProfileModel()
                       {
                           oAppGloOwnId = t_cb.AppGlobalOwnerId,
                           oAppGlobalOwn = t_ago, //t_cb.AppGlobalOwner,
                           oChurchBodyId = t_cb.Id,
                           /// 
                           oChurchBody = t_cb,
                           strChurchBody = t_cb.Name,
                           strAppGlobalOwn = t_ago.OwnerName, // + (!string.IsNullOrEmpty(t_ago.OwnerName) ? (t_ci_ago != null ? (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "") : "") : ""),

                           oUserProfile = t_up,

                           isVendorOwned = t_up.CreatedByUserId == null || t_up_cbu != null,
                           strChurchLevel = t_cb.ChurchLevel != null ? (!string.IsNullOrEmpty(t_cb.ChurchLevel.CustomName) ? t_cb.ChurchLevel.CustomName : t_cb.ChurchLevel.Name) : "",
                           strUserRoleName = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleName : "") : "",
                           // numUserRoleLevel = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleLevel : (int?)null) : (int?)null,
                           /// strAppCurrUser_RoleCateg = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleType : "") : "",
                           strUserGroupName = t_upg != null ? (t_upg.UserGroup != null ? t_upg.UserGroup.GroupName : "") : "",
                           strOwnerUser = t_up_o != null ? t_up_o.UserDesc : "",
                           strUserProfile = t_up.UserDesc,
                           strUserStatus = GetStatusDesc(t_up.UserStatus),

                           strChurchMember = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc,
                           strSTRT = t_up.Strt != null ? DateTime.Parse(t_up.Strt.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "",
                           strEXPR = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : ""
                       })
                   .FirstOrDefault();


                if (oUPModel == null)
                {
                    Response.StatusCode = 500;
                    return PartialView("_ErrorPage");
                }
            }

            //             
            oUPModel.oAppGloOwnId = oAGO_MSTR.Id;
            oUPModel.oAppGlobalOwn = oAGO_MSTR;
            oUPModel.oChurchBodyId = oCB_MSTR.Id;
            oUPModel.oChurchBody = oCB_MSTR;
            ///
            oUPModel.oChurchBodyId_Logged = this._oLoggedCB.MSTR_ChurchBodyId;
            oUPModel.oChurchBodyId_Logged_CLNT = this._oLoggedCB.Id;
            oUPModel.oAppGloOwnId_Logged = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
            oUPModel.oAppGloOwnId_Logged_CLNT = this._oLoggedAGO.Id;
            oUPModel.oUserId_Logged = _oLoggedUser.Id;


            /// load lookups 
            oUPModel = this.populateLookups_CL_UP(oUPModel, oUPModel.oChurchBody);  //setIndex, 
               
            var tm = DateTime.Now;
            _userTask = "Opened " + strDesc.ToLower() + ", " + oUPModel.oUserProfile.UserDesc + "[username: " + oUPModel.oUserProfile.Username + "]";


            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");



            // register @MSTR
            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                             "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id));

            //register @CLNT
            _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, null, null, "T",
                             "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
                 );

            var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUPModel);
            TempData["oVmCurrMod"] = _oUPModel; TempData.Keep();


            //
            if (setIndex == 0)
                return PartialView("_AddOrEdit_CL_UP", oUPModel);
            else    
                return PartialView("_vwAddOrEdit_CL_UP", oUPModel);
           

        }
         
        private Models.ViewModels.vm_app_ven.UserProfileModel populateLookups_CL_UP(Models.ViewModels.vm_app_ven.UserProfileModel vmLkp, MSTRChurchBody oChurchBody = null)  //int setIndex, 
        {
             if (vmLkp == null || oChurchBody == null) return vmLkp;
                // 
           vmLkp.lkpStatuses = new List<SelectListItem>();
                foreach (var dl in dlGenStatuses) { vmLkp.lkpStatuses.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc }); }


            vmLkp.lkpUserProfileLevels = new List<SelectListItem>();
            foreach (var dl in dlUserLevels)
            {
                if (oChurchBody.OrgType == "CR" || oChurchBody.OrgType == "CH")
                {
                    if (dl.Val == "6" || dl.Val == "7" || dl.Val == "8" || dl.Val == "9" || dl.Val == "10")  // CH Admin ...               
                        vmLkp.lkpUserProfileLevels.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });

                }
                else if (oChurchBody.OrgType == "CN")
                {
                    if (dl.Val == "11" || dl.Val == "12" || dl.Val == "13" || dl.Val == "14" || dl.Val == "15")  // CF Admin ...              
                        vmLkp.lkpUserProfileLevels.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });
                }
            }

            

            vmLkp.lkpUserRoles = _masterContext.UserRole
                .Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || 
                             (c.AppGlobalOwnerId != null && c.ChurchBodyId != null && c.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && c.ChurchBodyId == oChurchBody.Id)) && 
                              c.RoleStatus == "A" &&
                              (((oChurchBody.OrgType == "CR" || oChurchBody.OrgType == "CH") && c.RoleLevel >= 6 && c.RoleLevel < 11) ||
                              (oChurchBody.OrgType == "CN" && c.RoleLevel >= 11 && c.RoleLevel < 15)))
                   .OrderBy(c => c.RoleLevel)
                   .Select(c => new SelectListItem()
                   {
                       Value = c.Id.ToString(),
                       Text = c.RoleName.Trim()
                   })
                   // .OrderBy(c => c.Text)
                   .ToList();
            vmLkp.lkpUserRoles.Insert(0, new SelectListItem { Value = "", Text = "Select" });

            //
            vmLkp.lkpUserGroups = _masterContext.UserGroup   //.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A")
                                .Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) ||
                                         (c.AppGlobalOwnerId != null && c.ChurchBodyId != null && c.AppGlobalOwnerId == oChurchBody.AppGlobalOwnerId && c.ChurchBodyId == oChurchBody.Id)) &&
                                          c.Status == "A") // &&
                                         // ((oChurchBody.OrgType == "CH" && c.GroupLevel >= 6 && c.GroupLevel < 11) ||
                                          // (oChurchBody.OrgType == "CN" && c.GroupLevel >= 11 && c.GroupLevel < 15)))
                                  .OrderBy(c => c.UserGroupCategoryId).ThenBy(c => c.GroupName)
                                  .Select(c => new SelectListItem()
                                  {
                                      Value = c.Id.ToString(),
                                      Text = c.GroupName.Trim()
                                  })
                                  // .OrderBy(c => c.Text)
                                  .ToList();
            vmLkp.lkpUserGroups.Insert(0, new SelectListItem { Value = "", Text = "Select" });


            return vmLkp;
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
         public async Task<IActionResult> AddOrEdit_CL_UP(Models.ViewModels.vm_app_ven.UserProfileModel vm)
            {
                
                //if (!InitializeUserLogging())
                //return RedirectToAction("LoginUserAcc", "UserLogin");

                var strDesc = "User profile";
                // var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); 

                if (vm == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.pageIndex });
                if (vm.oUserProfile == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = vm.pageIndex });
                  
                UserProfile _oChanges = vm.oUserProfile;
                //   vmMod = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as Models.ViewModels.vm_app_ven.UserProfileModel : vmMod; TempData.Keep();

                var arrData = ""; var tm = DateTime.Now;
                arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
                var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<Models.ViewModels.vm_app_ven.UserProfileModel>(arrData) : vm;

             var oUPCopy = vmMod.oUserProfile ;
            //  oUP.ChurchBody = vmMod.oChurchBody;


            // confirm client admin
            if (oUPCopy.AppGlobalOwner == null || oUPCopy.ChurchBody == null)
            {
                oUPCopy.AppGlobalOwner = _masterContext.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
                oUPCopy.ChurchBody = _masterContext.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == _oChanges.ChurchBodyId).FirstOrDefault();
            }

            if (oUPCopy.AppGlobalOwner == null || oUPCopy.ChurchBody == null)
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specified denomination and church unit could not be retrieved. Please refresh and try again.", signOutToLogIn = false });

            if (string.IsNullOrEmpty(oUPCopy.ChurchBody.GlobalChurchCode))
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code not specified. The unique global church code of church unit is required. Please verify with System Admin and try again.", signOutToLogIn = false });

            //var clientUPRList = _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
            //                     .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
            //                           ((oUP.ChurchBody.OrgType == "CH" && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
            //                            (oUP.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15))).ToList();


            //if (clientUPRList.Count > 1 && _oChanges.Id > 0)
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one (1) default '" + (clientUPRList[0].UserRole != null ? clientUPRList[0].UserRole.RoleName : "[Administrator]") + "' permitted. " + clientUPRList.Count + " Admin users currently created for specified client. Please correct and try again. Hint: additional users can be created at the client side by client administrator", signOutToLogIn = false });
            //else if (clientUPRList.Count > 0 && _oChanges.Id == 0)
            //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Only one (1) default '" + (clientUPRList[0].UserRole != null ? clientUPRList[0].UserRole.RoleName : "[Administrator]") + "' permitted. Hint: additional users can be created at the client side by client administrator", signOutToLogIn = false });



            try
            {
                    ModelState.Remove("oUserProfile.AppGlobalOwnerId");
                    ModelState.Remove("oUserProfile.ChurchBodyId");
                    ModelState.Remove("oUserProfile.ChurchMemberId");
                    ModelState.Remove("oUserProfile.oCBChurchLevelId");
                    ModelState.Remove("oUserProfile.ContactInfoId");
                    ModelState.Remove("oUserProfile.CreatedByUserId");
                    ModelState.Remove("oUserProfile.LastModByUserId"); 
                    ModelState.Remove("oUserProfile.OwnerUserId");

                    // ChurchBody == null 

                    //finally check error state...
                    if (ModelState.IsValid == false)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", signOutToLogIn = false });

                    if (string.IsNullOrEmpty(_oChanges.Username)) // || string.IsNullOrEmpty(_oChanges.Pwd))  //Congregant... ChurcCodes required
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide username.", signOutToLogIn = false });
                    }
                     
                // trim leading /trailing spaces... 
                if (!string.IsNullOrEmpty(_oChanges.Username)) _oChanges.Username = _oChanges.Username.Trim();



                if (_oChanges.ProfileLevel == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please select minimum user role.", signOutToLogIn = false });

                if ((oUPCopy.ChurchBody.OrgType == "CR" || oUPCopy.ChurchBody.OrgType == "CH") && !(_oChanges.ProfileLevel >= 6 && _oChanges.ProfileLevel <=10))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Congregration head-unit cannot have role type specified. Change the user role type or contact system admin.", signOutToLogIn = false });

                if ((oUPCopy.ChurchBody.OrgType == "CN") && !(_oChanges.ProfileLevel >= 11 && _oChanges.ProfileLevel <= 15))
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Congregration cannot have role type specified. Change the user role type or contact system admin.", signOutToLogIn = false });
                                 

                //if (_oChanges.PwdSecurityQue != null && string.IsNullOrEmpty(_oChanges.PwdSecurityAns))  //Congregant... ChurcCodes required
                //{
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please provide the response to the security question specified.", signOutToLogIn = false });
                //}


                //confirm this is SYS acc   //check for the SYS acc
                //string strPwdHashedData = AppUtilties.ComputeSha256Hash(_oChanges.ChurchCode + _oChanges.Username.Trim().ToLower() + _oChanges.Password);
                //string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(vm.ChurchCode + _oChanges.Username.Trim().ToLower());
                // c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == strUserKeyHashedData && c.Pwd == strPwdHashedData
                ///

                //    var currLogUserInfo = (from t_up in _masterContext.UserProfile.Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && c.Id == vm.oUserId_Logged)
                //                           from t_upr in _masterContext.UserProfileRole.Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && 
                //                                            c.UserProfileId == t_up.Id && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                //                          from t_ur in _masterContext.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == t_upr.UserRoleId && c.RoleStatus == "A") // && c.RoleLevel == 1 && c.RoleType == "SYS")
                //                           select new
                //                           {
                //                               UserId = t_up.Id,
                //                              // UserRoleId = t_ur.Id,
                //                             //  UserType = t_ur.RoleType,
                //                              // UserRoleLevel = t_ur.RoleLevel,
                //                             //  UserStatus = t_up.strUserStatus == "A" && t_upr.ProfileRoleStatus == "A" && t_ur.RoleStatus == "A"
                //                           }
                //                     ).FirstOrDefault();


                //// checking admin has privileges to CRUD user...
                //    if (currLogUserInfo == null)
                //    { return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user not found! Please refresh and try again.", signOutToLogIn = false }); }

                //if (_oChanges.ProfileScope == "V")  //vendor admins ... SYS, SUP_ADMN, SYS_ADMN etc.
                //{
                //    if (currLogUserInfo.UserType == "SYS" && string.Compare(_oChanges.Username, "supadmin", true) != 0 && string.Compare(_oChanges.Username, "sys", true) != 0)
                //    {
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account can ONLY manage the [sys] or [supadmin] profile. Hint: Sign in with [supadmin] or other Admin account.", signOutToLogIn = false });
                //    }

                //    if (currLogUserInfo.UserType == "SUP_ADMN" && string.Compare(_oChanges.Username, "sys", true) == 0)
                //    {
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user does not have SYS role. SYS role required to manage SYS account.", signOutToLogIn = false });
                //    }

                //    if (currLogUserInfo.UserType != "SYS" && string.Compare(_oChanges.Username, "supadmin", true) == 0) // currLogUserInfo.UserType != "SUP_ADMN" && 
                //    {
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Current user does not have SYS role. SYS role required to manage SUP_ADMN account.", signOutToLogIn = false });
                //    }

                //    if (_oChanges.Id == 0)
                //    {
                //        if (string.Compare(_oChanges.Username, "sys", true) == 0)
                //        {
                //            var existUserRoles = (from upr in _masterContext.UserProfileRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                //                                  from ur in _masterContext.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SYS")
                //                                  select upr
                //                     );
                //            if (existUserRoles.Count() > 0)
                //            {
                //                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "SYS account cannot be created. Only one (1) SYS role allowed.", signOutToLogIn = false });
                //            }
                //        }

                //        if (string.Compare(_oChanges.Username, "supadmin", true) == 0)
                //        {
                //            var existUserRoles = (from upr in _masterContext.UserProfileRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileRoleStatus == "A" && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))
                //                                  from ur in _masterContext.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN")
                //                                  select upr
                //                     );
                //            if (existUserRoles.Count() > 0)
                //            {
                //                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Super Admin profile cannot be created. Only one (1) SUP_ADMN role allowed.", signOutToLogIn = false });
                //            }
                //        }
                //    }


                //    // check if client database has been created or can connect... thus if task is to create/manage a client admin profile
                //    // 1 - vendor admins, 2 - client admins, 3 - client users
                //    ///
                //    if (vm.setIndex == 2 || vm.setIndex == 3)
                //    {
                //        // Get the client database details.... db connection string                        
                //        var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                //        if (oClientConfig == null)
                //        {
                //            ViewData["strUserLoginFailMess"] = "Client database details not found. Please try again or contact System Admin";
                //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewData["strUserLoginFailMess"], signOutToLogIn = false });
                //        }

                //        // get and mod the conn
                //        var _clientDBConnString = "";
                //        var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
                //        conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                //        _clientDBConnString = conn.ConnectionString;

                //        // test the NEW DB conn
                //        var _clientContext = new ChurchModelContext(_clientDBConnString);
                //        if (!_clientContext.Database.CanConnect())
                //        {
                //            ViewData["strUserLoginFailMess"] = "Failed to connect client database. Please try again or contact System Admin";
                //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = ViewData["strUserLoginFailMess"], signOutToLogIn = false });
                //        }  // give appropriate user prompts

                //    }
                //}

                //else  //CLIENT ADMINs ... creating users for their churches /congregations
                //{


                //CLIENT ADMINs ... creating users for their churches /congregations
                if (_oChanges.AppGlobalOwnerId == null || _oChanges.ChurchBodyId == null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the denomination (church) and the congregation of user.", signOutToLogIn = false });

                //if (_oChanges.oCBChurchLevelId == null)
                //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the church level for the user profile.", signOutToLogIn = false });

                        //var oCBLevel = _masterContext.MSTRChurchLevel.Find(_oChanges.oCBChurchLevelId);
                        //if (oCBLevel == null)  // ... parent church level > church unit level
                        //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church unit level could not be found. Please refresh and try again", signOutToLogIn = false });

                        ///// get the parent id
                        ///// 
                        //var parDesc = "church unit";
                        //switch (vm.oCBLevelCount)
                        //{
                        //    case 1: _oChanges.ChurchBodyId = vm.ChurchBodyId_1; parDesc = vm.strChurchLevel_1; break;
                        //    case 2: _oChanges.ChurchBodyId = vm.ChurchBodyId_2; parDesc = vm.strChurchLevel_2; break;
                        //    case 3: _oChanges.ChurchBodyId = vm.ChurchBodyId_3; parDesc = vm.strChurchLevel_3; break;
                        //    case 4: _oChanges.ChurchBodyId = vm.ChurchBodyId_4; parDesc = vm.strChurchLevel_4; break;
                        //    case 5: _oChanges.ChurchBodyId = vm.ChurchBodyId_5; parDesc = vm.strChurchLevel_5; break;
                        //}


                        ////check availability of username... SYS /SUP_ADMN reserved
                        //if (string.Compare(_oChanges.Username, "sys", true) == 0 || string.Compare(_oChanges.Username, "supadmin", true) == 0)
                        //{
                        //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Username '" + _oChanges.Username  + "' used is system name. Please try different username.", signOutToLogIn = false });
                        //}

                        //// Denomination and ChurchBody cannot be null
                        //if (_oChanges.AppGlobalOwnerId == null || _oChanges.ChurchBodyId == null)
                        //{
                        //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specify the denomination and church unit", signOutToLogIn = false });
                        //}
                   // }


                    //check that username is unique within congregation
                    var existUserProfiles = _masterContext.UserProfile.Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && //... restrict within denomination as dbase is per denomination
                                                                            c.Id != _oChanges.Id && c.Username.Trim().ToLower() == _oChanges.Username.Trim().ToLower()).ToList();
                    if (existUserProfiles.Count() > 0)
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Username '" + _oChanges.Username + "' is not available. Try different username. [Recommendation: User's email]", signOutToLogIn = false });
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
                    if (string.IsNullOrEmpty(_oChanges.Email))  // == null))
                    {
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please specify email of user. Email is needed for password reset and login user verifications.", signOutToLogIn = false });
                    }
                    //check email availability and validity
                    else // (_oChanges.Email != null) //_oChanges.ChurchMemberId != null && 
                    {
                    // ??? ... check validity... REGEX
                        ///

                        if (!AppUtilties.IsValidEmail(_oChanges.Email))
                        {
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User email invalid. Check and try again.", signOutToLogIn = false });
                        }

                        // disallow impersonation within same CB... user can be at diff levels or congregations
                        var oUserEmailExist = _masterContext.UserProfile.Where(c => (_oChanges.Id == 0 || (_oChanges.Id > 0 && c.Id != _oChanges.Id)) && c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && 
                                                                                    c.Email == _oChanges.Email).FirstOrDefault();
                            if (oUserEmailExist != null)  // ModelState.AddModelError(_oChanges.Id.ToString(), "Email of member must be unique. >> Hint: Already used by another member: "  + GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName) + "[" + oCM.ChurchBody.Name + "]");
                                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User email must be unique. Email already used by another: [User: " + oUserEmailExist.UserDesc + "]", signOutToLogIn = false });

                        var oUserCIEmailExist = _masterContext.MSTRContactInfo.Where(c => (_oChanges.Id == 0 || (_oChanges.Id > 0 && c.RefUserId != _oChanges.Id)) && c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId && 
                                                                                    c.Email == _oChanges.Email).FirstOrDefault();
                        if (oUserCIEmailExist != null)  // ModelState.AddModelError(_oChanges.Id.ToString(), "Email of member must be unique. >> Hint: Already used by another member: "  + GetConcatMemberName(_oChanges.Title, _oChanges.FirstName, _oChanges.MiddleName, _oChanges.LastName) + "[" + oCM.ChurchBody.Name + "]");
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "User email must be unique. Email already used by another: [User: " + oUserEmailExist.UserDesc + "]", signOutToLogIn = false });


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





                    //// SYS control account check...
                    //var oAdminsCount = _masterContext.UserProfile.Count();
                    //if (string.Compare(_oChanges.Username, "sys", true) == 0 && oAdminsCount > 1)  // other users have been created....
                    //{
                    //    //check the SYS account...
                    //    var oSYSAcc = _masterContext.UserProfile.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Username.ToLower() == "SYS".ToLower() && c.UserStatus == "A").FirstOrDefault();
                    //    if (oSYSAcc == null)
                    //        return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile not found. SYS profile is a control account. Contact System Admin for help.", signOutToLogIn = false });
                    //    ///

                    //    if (oSYSAcc.PwdExpr == null)
                    //    {
                    //        return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile password not set. Reset password and try again.", signOutToLogIn = false });
                    //    }
                    //    else
                    //    {
                    //        if (oSYSAcc.PwdExpr.Value <= DateTime.Now.Date)
                    //            return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile password has expired. Reset password.", signOutToLogIn = false });
                    //    }

                    //    if (string.IsNullOrEmpty(oSYSAcc.Email))
                    //        return Json(new { taskSuccess = false, oCurrId = oSYSAcc.Id, userMess = "SYS profile email not configured. Email is needed for password reset and login user verifications.", signOutToLogIn = false });

                    //}



                    //if (_oChanges.ProfileScope == "V" && string.Compare(_oChanges.Username, "sys", true) == 0)
                    //{
                    //    var oAdminsCount = _masterContext.UserProfile.Count(); // (c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V");
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
                    _oChanges.LastModByUserId = vm.oUserId_Logged;
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

                  ///  var tm = DateTime.Now;
                    _oChanges.LastMod = tm;
                    _oChanges.CreatedByUserId = vm.oUserId_Logged;
                    // _oChanges.AppGlobalOwnerId = 
                    // _oChanges.ChurchBodyId = 

                    //validate...
                    var _userTask = "Attempted saving user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "");  //    _userTask = "Added new user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";  // _userTask = "Updated user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";

                //   using (var _userCtx = new MSTR_DbContext(_masterContext.Database.GetDbConnection().ConnectionString))
                //   {
                //if (_oChanges.Id == 0)
                //{
                //    var cc = "";
                //    if (_oChanges.AppGlobalOwnerId == null && _oChanges.ChurchBodyId == null && _oChanges.ProfileScope == "V")
                //    {
                //        cc = "000000";    //var churchCode = "000000"; _oChanges.Username = "SysAdmin"; _oChanges.Pwd = "$ys@dmin1";  
                //    }
                //    else  //client admins
                //    {
                //        var oAGO = _masterContext.MSTRAppGlobalOwner.Find(_oChanges.AppGlobalOwnerId);
                //        var oCB = _masterContext.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.Id == _oChanges.ChurchBodyId).FirstOrDefault();

                //        if (oAGO == null || oCB == null)
                //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specified denomination and church unit could not be retrieved. Please refresh and try again.", signOutToLogIn = false });
                //        if (string.IsNullOrEmpty(oCB.GlobalChurchCode))
                //            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code not specified. The unique global church code of church unit is required. Please verify with System Admin and try again.", signOutToLogIn = false });

                //        cc = oCB.GlobalChurchCode;

                //        // _oChanges.Pwd = AppUtilties.ComputeSha256Hash(_oChanges.Username + _oChanges.Pwd);
                //    }


                bool blSendUserEmail = false; bool isNewUsername = false; string strUserTempPwd = ""; // var cc = "";string strOldUsername = ""; 
                _oChanges.strChurchCode_CB = string.IsNullOrEmpty(_oChanges.strChurchCode_CB) ? oUPCopy.ChurchBody.GlobalChurchCode : _oChanges.strChurchCode_CB;
                var cc = _oChanges.strChurchCode_CB;

                // tracker error...
                if (_oChanges.AppGlobalOwner != null) _oChanges.AppGlobalOwner = null; if (_oChanges.ChurchBody != null) _oChanges.ChurchBody = null; 
                if (_oChanges.Id == 0)
                {
                    var tempPwd = CodeGenerator.GenerateCode();  //   const string tempPwd = "123456";
                   // cc = string.IsNullOrEmpty(_oChanges.strChurchCode_CB) ? oUP.ChurchBody.GlobalChurchCode : _oChanges.strChurchCode_CB;

                    _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());
                    _oChanges.Pwd = tempPwd; // "123456";  //temp pwd... to reset @ next login  
                            _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower() + _oChanges.Pwd);
                            _oChanges.Strt = tm;
                            _oChanges.Expr = (string.Compare(_oChanges.Username.Trim(), "sys", true) == 0 || string.Compare(_oChanges.Username.Trim(), "supadmin", true) == 0) ? (DateTime?)null : tm.AddDays(90);  //default to 90 days
                            _oChanges.ResetPwdOnNextLogOn = true;
                            _oChanges.PwdExpr = tm.AddDays(30);  //default to 30 days 
                            ///
                            _oChanges.Created = tm;
                            _oChanges.CreatedByUserId = vm.oUserId_Logged;


                    _masterContext.Add(_oChanges);

                            _userTask = "Added new user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";
                            ViewBag.UserMsg = "Saved user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully. Password must be changed on next logon";

                    /////
                    // created new account... send details to account holder!
                    blSendUserEmail = true;
                    strUserTempPwd = tempPwd;
                }
                else
                {
                    var existUsernameList = _masterContext.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&        //... restrict within denomination as dbase is per denomination
                                             c.Username.Trim().ToLower() == _oChanges.Username.Trim().ToLower()).ToList();

                    // username must exist ELSE it's changed!
                    isNewUsername = existUsernameList.Count == 0;
                    if (isNewUsername)
                    {
                        // strOldUsername = oUP.Username.Trim();

                        // RESET password... remember [cc + username + pwd] hashed together
                        _oChanges.ResetPwdOnNextLogOn = true;

                        // user key changes..
                        // if (string.Compare(_oChanges.UserKey, AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.ToLower()), true) != 0)
                        _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());

                        // changed username... send details to account holder!
                        blSendUserEmail = true;
                    }

                    // if RESET pwd... 
                    if (_oChanges.ResetPwdOnNextLogOn == true)
                    {
                        var tempPwd = CodeGenerator.GenerateCode();  //const string tempPwd = "123456";

                       // cc = string.IsNullOrEmpty(_oChanges.strChurchCode_CB) ? oUP.ChurchBody.GlobalChurchCode : _oChanges.strChurchCode_CB;

                        _oChanges.Pwd = tempPwd;  //temp pwd... to reset @ next login  
                        _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower() + _oChanges.Pwd);

                        if (string.Compare(_oChanges.UserKey, AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower()), true) != 0)
                            _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());


                        /////
                        // created new account... send details to account holder!
                        blSendUserEmail = true;
                        strUserTempPwd = tempPwd;
                    }

                    //// user key lost
                    //if (string.Compare(_oChanges.UserKey, AppUtilties.ComputeSha256Hash(_oChanges.strChurchCode_CB + _oChanges.Username.ToLower()), true) != 0)
                    //    _oChanges.UserKey = AppUtilties.ComputeSha256Hash(_oChanges.strChurchCode_CB + _oChanges.Username.ToLower());

                    //retain the pwd details... hidden fields
                    _masterContext.Update(_oChanges);

                    _userTask = "Updated user profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " successfully";
                    ViewBag.UserMsg = "User profile, " + (!string.IsNullOrEmpty(_oChanges.UserDesc) ? "[" + _oChanges.UserDesc + "]" : "") + " updated successfully.";
                }


                   //save user profile first... 
                   _masterContext.SaveChanges();
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
                    var userMess = "<span> Hello " + _oChanges.UserDesc + ",  </span><p>";
                    if (_oChanges.Id == 0)
                    {
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

                    userMess += "<span class='text-success'> Church code: " + _oChanges.strChurchCode_CB + "</span><p>";
                    userMess += "<span class='text-success'> Username: " + _oChanges.Username.Trim() + "</span><p>";
                    userMess += "<h3 class='text-success'> Password: " + strUserTempPwd + "</h3><br /><hr /><br />";

                    userMess += "<span class='text-info text-lg'> Please you will be required to RESET password at next logon. </span><p><p>"; 

                    userMess += "<span class='text-info'> Thanks </span>";
                    userMess += "<span class='text-info'> RHEMA-CMS Team (Ghana) </span>";

                    userMess = "<div class='text-center col-md border border-info' style='padding: 50px 0'>" + userMess + "</div>";

                    listToAddr.Add(new MailAddress(_oChanges.Email, _oChanges.UserDesc));
                    var res = AppUtilties.SendEmailNotification("RHEMA-CMS", msgSubject, userMess, listToAddr, listCcAddr, listBccAddr, null, true);

                }


                //audit...
                var _tm = DateTime.Now;
                await this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));


                    //check if role assigned... SUP_ADMN -- auto, others -- warn!
                    var _cs = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId); /// 


                if (!string.IsNullOrEmpty(_cs))
                {
                    using (var _roleCtx = new MSTR_DbContext(_cs))
                    {
                        if (_oChanges.ProfileLevel == 6 || _oChanges.ProfileLevel == 11)  //(vmMod.subSetIndex == 6 || vmMod.subSetIndex == 11) // SUP_ADMN role
                        {
                            var oChuAdminRole = _masterContext.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" &&
                                                                            (((oUPCopy.ChurchBody.OrgType == "CR" || oUPCopy.ChurchBody.OrgType == "CH") && c.RoleType == "CH_ADMN" && c.RoleLevel == 6) ||
                                                                             (oUPCopy.ChurchBody.OrgType == "CN" && c.RoleType == "CF_ADMN" && c.RoleLevel == 11)))
                                                                 .FirstOrDefault();

                            // jux 1 to execute... no unit to access except CH /CN
                            if (oChuAdminRole != null)
                            {
                                var existUserRoles = (from upr in _masterContext.UserProfileRole
                                                      .Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                                                      c.UserRoleId == oChuAdminRole.Id && c.ProfileRoleStatus == "A") // &&                                                                                                                                                                                                                           // ((c.Strt == null || c.Expr == null) || (c.Strt != null && c.Expr != null && c.Strt <= DateTime.Now && c.Expr >= DateTime.Now && c.Strt <= c.Expr)))
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
                                        CreatedByUserId = vm.oUserId_Logged,
                                        LastModByUserId = vm.oUserId_Logged
                                    };

                                    _roleCtx.Add(oUPR);

                                    //save user role...
                                    await _roleCtx.SaveChangesAsync();

                                    DetachAllEntities(_roleCtx);

                                    _userTask = "Added [" + oChuAdminRole.RoleType + "] role to user, " + _oChanges.Username;
                                    ViewBag.UserMsg += Environment.NewLine + " ~ [" + oChuAdminRole.RoleType + "] role added.";


                                    _tm = DateTime.Now;
                                    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                         "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));
                                    // } 
                                }
                                else
                                {
                                    oUPR = existUserRoles[0];  // user may have multiple roles .. CHECK
                                }

                                if (oChuAdminRole != null && oUPR != null)
                                {
                                    using (var _permCtx = new MSTR_DbContext(_cs))
                                    {
                                        // assign all privileges to the ADMN role 
                                        var existUserRolePerms = (from upr in _masterContext.UserRolePermission.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && // c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                                                                  c.Status == "A" && c.UserRoleId == oUPR.UserRoleId && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                                  select upr).ToList();
                                        //if (existUserRolePerms.Count() > 0)
                                        //{

                                        // get only [00_] Church level Admin permissions ... 
                                        var oUserPerms = (from upr in _masterContext.UserPermission.Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0"))  // c.PermissionCode.StartsWith("01"))                                                                                                                                                      // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                                                          select upr).ToList();

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
                                                        AppGlobalOwnerId = null, //_oChanges.AppGlobalOwnerId,
                                                        ChurchBodyId = null, //_oChanges.ChurchBodyId,
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
                                                        CreatedByUserId = vm.oUserId_Logged,
                                                        LastModByUserId = vm.oUserId_Logged
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
                                                        existUserRolePerm.CreatedByUserId = vm.oUserId_Logged;
                                                        existUserRolePerm.LastModByUserId = vm.oUserId_Logged;

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
                                                                  "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oUserId_Logged, _tm, _tm, vm.oUserId_Logged, vm.oUserId_Logged));
                                             
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                 
                }




                // oCM_NewConvert.Created = DateTime.Now;
                // _masterContext.Add(_oChanges);



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
                // await _masterContext.SaveChangesAsync();

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

        public IActionResult Delete_CL_UP(int? oAppGloOwnId, int? oCurrChuBodyId, int? loggedUserId, int id, int setIndex, int subSetIndex, bool forceDeleteConfirm = false)
            {
            // var strDesc = setIndex == 1 ? "System profile" : setIndex == 2 ? "Church admin profile" : "Church user profile";
            var strDesc = setIndex == 2 ? "Church admin profile" : "Church user profile"; // (setIndex == 1 ? "System admin profile" : (setIndex == 2 ? "Church admin profile" : "Church user profile"));
                var tm = DateTime.Now; var _tm = DateTime.Now; var _userTask = "Attempted saving  " + strDesc;
                //
                try
                {
                    var strUserDenom = "Vendor Admin";
                   // if (setIndex != 1)
                  //  {
                        if (oAppGloOwnId == null || oCurrChuBodyId == null)
                            return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Denomination/church of " + strDesc + " unknown. Please refesh and try again." });

                        var oAGO = _masterContext.MSTRAppGlobalOwner.Find(oAppGloOwnId);
                        var oCB = _masterContext.MSTRChurchBody.Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.Id == oCurrChuBodyId).FirstOrDefault();

                        if (oAGO == null || oCB == null)
                            return Json(new { taskSuccess = false, oCurrId = "", userMess = "Specified denomination and church unit could not be retrieved. Please refresh and try again.", signOutToLogIn = false });

                        strUserDenom = oCB.Name + (!string.IsNullOrEmpty(oAGO.Acronym) ? ", " + oAGO.Acronym : oAGO.OwnerName);
                        strUserDenom =(!string.IsNullOrEmpty(strUserDenom) ? "Denomination: " + strUserDenom : strUserDenom);// "--" + 
                 //   }


                    var oUser = _masterContext.UserProfile.Where(c => c.Id == id && // (setIndex == 1 && oAppGloOwnId == null && oCurrChuBodyId == null) ||  setIndex != 1 && 
                                                c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId).FirstOrDefault();// .Include(c => c.ChurchUnits)
                    if (oUser == null)
                    {
                        _userTask = "Attempted deleting " + strDesc.ToLower() + ", " + oUser.UserDesc + " [" + oUser.Username + "]" + strUserDenom;  // var _userTask = "Attempted saving  " + strDesc;
                        _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
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
                    //var oUPRs = _masterContext.UserProfileRole.Where(c => c.UserProfileId == oUser.Id).ToList();    // .... cascade delete together with the roles, groups / permissions assigned
                    //var UPGs = _masterContext.UserProfileGroup.Where(c => c.UserProfileId == oUser.Id).ToList();

                    var UATs = _masterContext.UserAuditTrail.Where(c => c.UserProfileId == oUser.Id).ToList();

                    //using (var _userCtx = new MSTR_DbContext(_masterContext.Database.GetDbConnection().ConnectionString))
                    //{
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
                            _masterContext.UserProfile.Remove(oUser);
                            _masterContext.SaveChanges();

                      //      DetachAllEntities(_userCtx);

                            //audit...
                            _userTask = "Deleted " + strDesc.ToLower() + ", " + oUser.UserDesc + " [" + oUser.Username + "]" + strUserDenom;
                            _tm = DateTime.Now;
                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS-Admin:  " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                            return Json(new { taskSuccess = true, tryForceDelete = false, oCurrId = oUser.Id, userMess = strDesc + " successfully deleted." });
                        }

                  // }


                    _userTask = "Attempted deleting " + strDesc.ToLower() + "," + oUser.UserDesc + "[" + oUser.Username + "]" + strUserDenom + " -- but FAILED. Data unavailable.";   // var _userTask = "Attempted saving " + strDesc;
                    _tm = DateTime.Now;
                    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));

                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "No " + strDesc.ToLower() + " data available to delete. Try again" });
                }

                catch (Exception ex)
                {
                    _userTask = "Attempted deleting " + strDesc.ToLower() + ", [ ID= " + id + "] FAILED. ERR: " + ex.Message;  // var _userTask = "Attempted saving " + strDesc;
                    _tm = DateTime.Now;
                    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));
                    //
                    return Json(new { taskSuccess = false, tryForceDelete = false, oCurrId = id, userMess = "Failed deleting " + strDesc.ToLower() + ". Err: " + ex.Message });
                }
            }

      //  public ActionResult Index_CL_UPO(int id = 0, int? oAppGloOwnId = null, int? oChurchBodyId = null, int? oUserId = null, int setIndex = 0, bool loadSectionOnly = false)  //, int filterIndex = 1, int pageIndex = 1, int? numCodeCriteria_1 = (int?)null, string strCodeCriteria_2 = null)  // , int? subSetIndex = 0  int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        public IActionResult Index_CL_UPO(int id = 0, int? oAppGloOwnId = null, int? oChurchBodyId = null, int? oUserId = null, int setIndex = 0, bool loadSectionOnly = false)  //, int filterIndex = 1, int pageIndex = 1, int? numCodeCriteria_1 = (int?)null, string strCodeCriteria_2 = null)  // , int? subSetIndex = 0  int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int setIndex = 0, int subSetIndex = 0) //, int? oParentId = null, int? id = null, int pageIndex = 1)             
        {
            try
            {
                if (this._context == null)
                {
                    this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);
                    if (this._context == null)
                    {
                        RedirectToAction("LoginUserAcc", "UserLogin");

                        // should not get here... Response.StatusCode = 500; 
                        return View("_ErrorPage");
                    }
                }

                if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
                { RedirectToAction("LoginUserAcc", "UserLogin"); }


                if (!loadSectionOnly)
                    _ = this.LoadClientDashboardValues(); // this._clientDBConnString);

                // SetUserLogged();

                //load the dash  ... main layout stuff
                // LoadClientDashboardValues(); 

                //if (!loadSectionOnly)
                //    _ = LoadClientDashboardValues(this._clientDBConnString); //await this.LoadClientDashboardValues(this._clientDBConnString, this._oLoggedUser);


                //if (!InitializeUserLogging())
                //    return RedirectToAction("LoginUserAcc", "UserLogin");

                // Client
                if (oAppGloOwnId == null) oAppGloOwnId = this._oLoggedAGO.Id;
                if (oChurchBodyId == null) oChurchBodyId = this._oLoggedCB.Id;

                // MSTR
                if (oUserId == null) oUserId = this._oLoggedUser.Id;
                var oAGO_MSTR = _masterContext.MSTRAppGlobalOwner.Find(this._oLoggedAGO.MSTR_AppGlobalOwnerId);
                var oCB_MSTR = _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.MSTR_AppGlobalOwnerId && c.Id == this._oLoggedCB.MSTR_ChurchBodyId).FirstOrDefault();
                if (oAGO_MSTR == null || oCB_MSTR == null)  // || oCU_Parent == null church units may be networked...
                { return PartialView("_ErrorPage"); }

                // var proScope = "C";
                //get all member from congregation
                var oCMList = _context.ChurchMember.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oChurchBodyId).ToList();

                // get all users from client-congregation  //.Include(t => t.ContactInfo)
                var oUP_List = (from t_up in _masterContext.UserProfile.AsNoTracking().Where(c => c.Id == id && c.AppGlobalOwnerId == oAGO_MSTR.Id && c.ChurchBodyId == oCB_MSTR.Id && c.ProfileScope == "C")
                                select t_up).ToList();

                var oUPModel = (
                       from t_up in oUP_List
                       from t_ago in _masterContext.MSTRAppGlobalOwner.AsNoTracking().Where(c => c.Id == t_up.AppGlobalOwnerId)  ////.Include(t => t.AppGlobalOwner)  && c.Status == "A"
                       from t_cb in _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel)
                           .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.Id == t_up.ChurchBodyId) //.DefaultIfEmpty()   
                       from t_upr in _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                           .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id &&   // ((proScope == "V" && (c.UserRole.RoleType == "SYS" || c.UserRole.RoleType == "SUP_ADMN" || c.UserRole.RoleType == "SYS_ADMN" || c.UserRole.RoleType == "SYS_CUST") && (c.UserRole.RoleLevel >= 1 && c.UserRole.RoleLevel <= 5)) ||
                                (((c.ChurchBody.OrgType == "CR" || c.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel < 11) ||
                                 (c.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel < 15))).DefaultIfEmpty()   // ((proScope == "C" && subScope == "A" && (c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CH_RGSTR" || c.UserRole.RoleType == "CH_ACCT" || c.UserRole.RoleType == "CH_CUST" || c.UserRole.RoleType == "CF_ADMN" || c.UserRole.RoleType == "CF_RGSTR" || c.UserRole.RoleType == "CF_ACCT" || c.UserRole.RoleType == "CF_CUST")) && (c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 15))                                     
                       from t_upg in _masterContext.UserProfileGroup.AsNoTracking().Include(t => t.UserGroup)
                           .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.UserProfileId == t_up.Id).DefaultIfEmpty()
                       from t_up_o in _masterContext.UserProfile.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == t_up.AppGlobalOwnerId && c.ChurchBodyId == t_up.ChurchBodyId && c.ProfileScope == "C" && c.Id == t_up.OwnerUserId).DefaultIfEmpty()
                       from t_cm in oCMList.Where(c => c.Id == t_up.ChurchMemberId).DefaultIfEmpty()
                       from t_up_cbu in _masterContext.UserProfile.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Id == t_up.CreatedByUserId).DefaultIfEmpty()

                           //   from t_ur in _context.UserRole.AsNoTracking().Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId &&
                           //                  (c.RoleType == "SYS" || c.RoleType == "SUP_ADMN" || c.RoleType == "SYS_ADMN" || c.RoleType == "SYS_CUST") && (c.RoleLevel > 0 && c.RoleLevel <= 5))
                           //             //   (c.RoleType != null && c.RoleLevel == roleLevel) || (roleLevel == null && c.RoleLevel > 0 && c.RoleLevel <= 5)))  //.DefaultIfEmpty()
                           //from t_urp in _context.UserRolePermission.AsNoTracking().Include(t => t.UserPermission)
                           //             .Where(c => c.ChurchBodyId == null && c.UserRoleId == t_upr.UserRoleId).DefaultIfEmpty()

                           //from t_ugp in _context.UserGroupPermission.AsNoTracking().Include(t => t.UserPermission)
                           //             .Where(c => c.ChurchBodyId == null && c.UserGroupId == t_upg.UserGroupId).DefaultIfEmpty()

                       select new Models.ViewModels.vm_app_ven.UserProfileModel()
                       {
                           oAppGloOwnId = t_cb.AppGlobalOwnerId,
                           oAppGlobalOwn = t_ago, //t_cb.AppGlobalOwner,
                           oChurchBodyId = t_cb.Id,
                           /// 
                           oChurchBody = t_cb,
                           strChurchBody = t_cb.Name,
                           strAppGlobalOwn = t_ago.OwnerName, // + (!string.IsNullOrEmpty(t_ago.OwnerName) ? (t_ci_ago != null ? (t_ci_ago.Country != null ? t_ci_ago.Country.EngName : "") : "") : ""),

                           oUserProfile = t_up,
                           
                           isVendorOwned = t_up.CreatedByUserId==null || t_up_cbu != null,
                           strChurchLevel = t_cb.ChurchLevel != null ? (!string.IsNullOrEmpty(t_cb.ChurchLevel.CustomName) ? t_cb.ChurchLevel.CustomName : t_cb.ChurchLevel.Name) : "",
                           strUserRoleName = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleName : "") : "",
                           //numUserRoleLevel = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleLevel : (int?)null) : (int?)null,
                           /// strAppCurrUser_RoleCateg = t_upr != null ? (t_upr.UserRole != null ? t_upr.UserRole.RoleType : "") : "",
                           strUserGroupName = t_upg != null ? (t_upg.UserGroup != null ? t_upg.UserGroup.GroupName : "") : "",
                           strOwnerUser = t_up_o != null ? t_up_o.UserDesc : "",
                           strUserProfile = t_up.UserDesc,
                           strUserStatus = GetStatusDesc(t_up.UserStatus),
                           strChurchMember = t_cm != null ? ((((!string.IsNullOrEmpty(t_cm.Title) ? t_cm.Title : "") + ' ' + t_cm.FirstName).Trim() + " " + t_cm.MiddleName).Trim() + " " + t_cm.LastName).Trim() : t_up.UserDesc,
                           strSTRT = t_up.Strt != null ? DateTime.Parse(t_up.Strt.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "",
                           strEXPR = t_up.Expr != null ? DateTime.Parse(t_up.Expr.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : ""
                       })
                   .FirstOrDefault();

                if (oUPModel == null)
                { 
                    return PartialView("_ErrorPage");
                }


                // var oUPModel = new Models.ViewModels.vm_app_ven.UserProfileModel();

                oUPModel.oAppGloOwnId = oAGO_MSTR.Id;
                oUPModel.oAppGlobalOwn = oAGO_MSTR;
                oUPModel.oChurchBodyId = oCB_MSTR.Id;
                oUPModel.oChurchBody = oCB_MSTR;
                ///
                oUPModel.oChurchBodyId_Logged = this._oLoggedCB.MSTR_ChurchBodyId;
                oUPModel.oChurchBodyId_Logged_CLNT = this._oLoggedCB.Id;
                oUPModel.oAppGloOwnId_Logged = this._oLoggedAGO.MSTR_AppGlobalOwnerId;
                oUPModel.oAppGloOwnId_Logged_CLNT = this._oLoggedAGO.Id;
                oUPModel.oUserId_Logged = _oLoggedUser.Id;

                //oUPModel.pageIndex = 1;
                //oUPModel.filterIndex = filterIndex;
                oUPModel.setIndex = (int)setIndex;
                // oUPModel.subSetIndex = (int)subSetIndex;
                /// 
                //oUPModel.lsUserProfileModels = oUPModel_List;
                //ViewData["oUPModel_List"] = oUPModel.lsUserProfileModels;


                // get other sub-modules 
                oUPModel.lsUserRolesAll = _masterContext.UserRole.AsNoTracking() 
                            .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus=="A" &&
                                 (((oUPModel.oChurchBody.OrgType == "CR" || oUPModel.oChurchBody.OrgType == "CH") && c.RoleLevel >= 6 && c.RoleLevel < 11) ||
                                  (oUPModel.oChurchBody.OrgType == "CN" && c.RoleLevel >= 11 && c.RoleLevel < 15))
                            )
                            .OrderBy(c=>c.RoleLevel).ThenBy(c => c.RoleName)
                            .ToList();

                //all roles
                oUPModel.lsUserProfileRoles = _masterContext.UserProfileRole.AsNoTracking().Include(t=>t.UserRole) //.Include(t=>t.UserProfile)
                            .Where(c => c.AppGlobalOwnerId == oUPModel.oAppGloOwnId && c.ChurchBodyId == oUPModel.oChurchBodyId && c.UserProfileId==oUPModel.oUserProfile.Id).ToList();

                var oUPR_All = new List<UserProfileRole>();
               // UserProfileRole oUPR = new UserProfileRole();
                foreach (var userRole in oUPModel.lsUserRolesAll)
                {
                    var oUPR = oUPModel.lsUserProfileRoles.Where(c => c.UserRoleId == userRole.Id).FirstOrDefault();
                    // create UPR ... 
                    oUPR_All.Add(
                        new UserProfileRole()
                        {
                            Id = oUPR != null ? oUPR.Id : 0,
                            AppGlobalOwnerId = oUPModel.oAppGloOwnId,
                            ChurchBodyId = oUPModel.oChurchBodyId,
                           
                            UserProfile = oUPModel.oUserProfile,
                            UserProfileId = oUPModel.oUserProfile.Id,
                            UserRole = userRole,
                            UserRoleId = userRole.Id,
                            isRoleAssigned = oUPR != null, //oUPModel.lsUserProfileRoles.Count(c => c.UserRoleId == oURA.Id) > 0
                            strRoleName = userRole.RoleName,
                            Strt = oUPR != null ? oUPR.Strt : (DateTime?)null,
                            strSTRT = oUPR != null ? (oUPR.Strt != null ? DateTime.Parse(oUPR.Strt.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "") : "",
                            strEXPR = oUPR != null ? (oUPR.Expr != null ? DateTime.Parse(oUPR.Expr.ToString()).ToString("d MMM yyyy", CultureInfo.InvariantCulture) : "") : "",

                            Created = oUPR != null ? oUPR.Created : (DateTime?)null,
                            CreatedByUserId = oUPR != null ? oUPR.CreatedByUserId : (int?)null,
                            LastMod = oUPR != null ? oUPR.LastMod : (DateTime?)null,
                            LastModByUserId = oUPR != null ? oUPR.LastModByUserId : (int?)null,
                        });

                    //userRole.strRoleName = userRole.RoleName; // (oURA.RoleLevel != 9 && oURA.RoleLevel != 14) ? oURA.RoleName : string.Concat(oURA.RoleName, "-" + oURA.CustomRoleName);
                    //userRole.strUserProfileRole = userRole.strRoleName;
                    //userRole.bl_IsRoleAssigned = oUPModel.lsUserProfileRoles.Count(c => c.UserRoleId == userRole.Id) > 0;
                }

                oUPR_All = oUPR_All.OrderByDescending(c => c.isRoleAssigned).ThenByDescending(c => c.Strt).ThenBy(c => c.strRoleName).ToList();
                oUPModel.lsUserProfileRoles_All = oUPR_All;

                //roles via roles -- direct config
                oUPModel.arrAssignedRoleCodes = new List<string>();
                oUPModel.arrAssignedRoleNames = new List<string>();
                foreach (var oUPR in oUPModel.lsUserProfileRoles)
                {
                    if (!oUPModel.arrAssignedRoleCodes.Contains(oUPR.UserRole.RoleType))
                    {
                        oUPModel.arrAssignedRoleCodes.Add(oUPR.UserRole.RoleType);
                        oUPModel.arrAssignedRoleNames.Add(oUPR.UserRole.RoleName);
                    }
                }                    
                oUPModel.strUserRoleName = String.Join(", ", oUPModel.arrAssignedRoleNames);

                // roles via group
                oUPModel.lsUserGroupsAll = _masterContext.UserGroup.AsNoTracking()
                            .Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A" &&
                                    (((oUPModel.oChurchBody.OrgType == "CR" || oUPModel.oChurchBody.OrgType == "CH") && c.GroupLevel >= 6 && c.GroupLevel < 16) ||
                                     (oUPModel.oChurchBody.OrgType == "CN" && c.GroupLevel >= 16 && c.GroupLevel < 25))
                            )
                            .OrderBy(c => c.GroupLevel).ThenBy(c => c.GroupName)
                            .ToList();

                oUPModel.lsUserProfileGroups = _masterContext.UserProfileGroup.AsNoTracking() //.Include(t => t.UserGroup)
                            .Where(c => c.AppGlobalOwnerId == oUPModel.oAppGloOwnId && c.ChurchBodyId == oUPModel.oChurchBodyId && c.UserProfileId == oUPModel.oUserProfile.Id).ToList();

                foreach (var oURG in oUPModel.lsUserGroupsAll)
                {
                    oURG.strGroupName = oURG.GroupName; // (oURG.GroupLevel != 9 && oURG.GroupLevel != 14) ? oURG.GroupName : string.Concat(oURG.GroupName, "-" + oURG.CustomGroupName); 
                    oURG.bl_IsGroupAssigned = oUPModel.lsUserProfileGroups.Count(c => c.UserGroupId == oURG.Id) > 0;
                }

                oUPModel.arrAssignedGroupNames = new List<string>();
                oUPModel.arrAssignedGroupNames = new List<string>();
                foreach (var oUPG in oUPModel.lsUserProfileGroups)
                {
                    if (!oUPModel.arrAssignedGroupNames.Contains(oUPG.UserGroup.GroupName))
                    {
                        oUPModel.arrAssignedGroupNames.Add(oUPG.UserGroup.GroupName);
                       // oUPModel.arrAssignedGroupNames.Add(oUPG.UserGroup.GroupName);
                    }
                }
                oUPModel.strUserGroupName = String.Join(", ", oUPModel.arrAssignedGroupNames);

                oUPModel.lsUserGroupRoles = new List<UserGroupRole>();
                foreach (var oUPG in oUPModel.lsUserProfileGroups)
                {
                    var oUGR_List = _masterContext.UserGroupRole.AsNoTracking().Include(t => t.UserGroup).Include(t => t.UserRole)
                            .Where(c => c.AppGlobalOwnerId == oUPModel.oAppGloOwnId && c.ChurchBodyId == oUPModel.oChurchBodyId && c.UserGroupId== oUPG.UserGroupId).ToList();
                    ///
                    oUPModel.lsUserGroupRoles.AddRange(oUGR_List);
                }

                // oUPModel.lsUserRolePermissions  = new List<UserRolePermission>(); 
                var oUR_SessionPermList = (  
                                    from a in oUPModel.lsUserProfileRoles  // note: UserRole included from previous list
                                    from b in _masterContext.UserRolePermission.Where(x => x.AppGlobalOwnerId == null && x.ChurchBodyId == null && x.UserRoleId == a.UserRoleId)
                                    from c in _masterContext.UserPermission.Where(x => x.Id == b.UserPermissionId)   

                                    select new UserSessionPerm()  // UserSessionPrivilege
                                    {
                                        oAppGlobalOwnerId = b.AppGlobalOwnerId,
                                        oChurchBodyId = b.ChurchBodyId,
                                        ///
                                        UserPermission = c,
                                        oUserPermissionId = c.Id,
                                        PermissionCode = c.PermissionCode,
                                        PermissionName = c.PermissionName,
                                        strPermission = AppUtilties.GetPermissionDesc_FromName(c.PermissionName),
                                        strPermModule = AppUtilties.GetPermModule(c.PermissionCode.Length > 1 ? c.PermissionCode.Substring(0, 2) : ""),
                                        //  PermissionValue = true,
                                        ///
                                        oUserRoleId = b.UserRoleId,
                                        oUserGroupId = b.UserRoleId,
                                        //
                                        oCreated = b.Created,
                                        oCreatedByUserId = b.CreatedByUserId,
                                        oLastMod = b.Created,
                                        oLastModByUserId = b.LastModByUserId,
                                        ///
                                        ViewPerm = b.ViewPerm,
                                        CreatePerm = b.CreatePerm,
                                        EditPerm = b.EditPerm,
                                        DeletePerm = b.DeletePerm,
                                        ManagePerm = b.ManagePerm,
                                        ///
                                        UserRole = a.UserRole,
                                        strRoleName = a.UserRole != null ? a.UserRole.RoleName : "",
                                        strRoleCode = a.UserRole != null ? a.UserRole.RoleType : "",
                                        ///
                                        //UserGroup = a.UserGroup,
                                        strGroupName = "N/A" //, a.UserGroup != null ? a.UserGroup.GroupName : "",
                                        //strGroupCode = a.UserGroup != null ? a.UserGroup.GroupType : ""


                                        //UserPermission = c,
                                        //oUserPermissionId = c.Id,
                                        //PermissionCode = c.PermissionCode,
                                        //PermissionName = c.PermissionName,
                                        ////PermissionValue = true, 
                                        /////
                                        //ViewPerm = b.ViewPerm,
                                        //CreatePerm = b.CreatePerm,
                                        //EditPerm = b.EditPerm,
                                        //DeletePerm = b.DeletePerm,
                                        //ManagePerm = b.ManagePerm,
                                        /////
                                        //UserRole = a.UserRole,
                                        //strRoleName = a.UserRole != null ? a.UserRole.RoleName : "",
                                        //strRoleCode = a.UserRole != null ? a.UserRole.RoleType : "",
                                          

                                    })
                                    .ToList();

                // Permission can also be config via User Groups...
                // var _oUserLog_GroupPerm = new List<UserPermission>();
                var oUG_SessionPermList = (
                                    from a in oUPModel.lsUserGroupRoles // note: UserGroup, UserRole included from previous list
                                    from b in _masterContext.UserRolePermission //.Include(t=>t.UserRole)
                                        .Where(x => x.AppGlobalOwnerId == null && x.ChurchBodyId == null && x.UserRoleId == a.UserRoleId)
                                    from c in _masterContext.UserPermission
                                        .Where(x => x.Id == b.UserPermissionId)

                                    select new UserSessionPerm()  // UserSessionPrivilege
                                    {
                                        oAppGlobalOwnerId = b.AppGlobalOwnerId,
                                        oChurchBodyId = b.ChurchBodyId,
                                        ///
                                        UserPermission = c,
                                        oUserPermissionId = c.Id,
                                        PermissionCode = c.PermissionCode,
                                        PermissionName = c.PermissionName,
                                        strPermission = AppUtilties.GetPermissionDesc_FromName(c.PermissionName),
                                        strPermModule = AppUtilties.GetPermModule(c.PermissionCode.Length > 1 ? c.PermissionCode.Substring(0, 2) : ""),
                                        //  PermissionValue = true,
                                        ///
                                        oUserRoleId = b.UserRoleId,
                                        oUserGroupId = b.UserRoleId,
                                        //
                                        oCreated = b.Created,
                                        oCreatedByUserId = b.CreatedByUserId,
                                        oLastMod = b.Created,
                                        oLastModByUserId = b.LastModByUserId, 
                                        ///
                                        ViewPerm = b.ViewPerm,
                                        CreatePerm = b.CreatePerm,
                                        EditPerm = b.EditPerm,
                                        DeletePerm = b.DeletePerm,
                                        ManagePerm = b.ManagePerm,
                                        ///
                                        UserRole = a.UserRole,
                                        strRoleName = a.UserRole != null ? a.UserRole.RoleName : "",
                                        strRoleCode = a.UserRole != null ? a.UserRole.RoleType : "",
                                        ///
                                        UserGroup = a.UserGroup,
                                        strGroupName = a.UserGroup != null ? a.UserGroup.GroupName : "",
                                        strGroupCode = a.UserGroup != null ? a.UserGroup.GroupType : "",
                                    })
                                    .ToList();


                var oPermAll = new List<UserSessionPerm>();
                oPermAll.AddRange(oUR_SessionPermList);
                oPermAll.AddRange(oUG_SessionPermList);
                oUPModel.lsUserSessionPermList = oPermAll
                                .OrderBy(c=> (c.PermissionCode.Length > 1 ? c.PermissionCode.Substring(0, 2) : "")).ThenBy(c=>c.PermissionCode)
                                .ToList();


                var strDesc = "User Profile Detail";
                var _userTask = "Viewed " + strDesc.ToLower() + " list";
                oUPModel.strCurrTask = strDesc;


                var tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, tm, _oLoggedUser.Id, tm, tm, _oLoggedUser.Id, _oLoggedUser.Id));

                ///
                var _oUPModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUPModel);
                TempData["oVmCSPModel"] = _oUPModel; TempData.Keep();

                if (loadSectionOnly)
                    return PartialView("_vwIndex_CL_UPO", oUPModel);
                else
                    return View("Index_CL_UP", oUPModel);
            }

            catch (Exception ex)
            {
                throw;
                ////page not found error
                //Response.StatusCode = 500;
                //return View("_ErrorPage");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
       //public async Task<IActionResult> AddMod_CL_UPR(List<UserRole> lsUserRoles)JsonResult
       public IActionResult AddMod_CL_UPR(List<UserProfileRole> lsUserProfileRoles)
        {
            //List<UserProfileRole> lsUserProfileRoles = null;
            var strDesc = "User profile roles";
            //if (!InitializeUserLogging())
            //    return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data unavailable. Please refresh and try again.", pageIndex = 0 });  //RedirectToAction("LoginUserAcc", "UserLogin");
            
            // var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); 

            if (lsUserProfileRoles == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = 0 });
            if (lsUserProfileRoles.Count == 0) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No data unavailable. Add " + strDesc+ " data and try again.", pageIndex = 0 });

            try
            {
                // UserProfile _oChanges = vm.oUserProfile;

                //using (CustomersEntities entities = new CustomersEntities())
                //{
                //Truncate Table to delete all old records.
                //_masterContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Customers]");


                // ADD ROLES...
                //add non-exist but checked, update... remove unchecked roles                 
                var rowsUpdated = 0; var rowsAdded = 0; var tm = DateTime.Now; var rowsDeleted = 0;
                UserProfile oUserProfile = null;
               // var masterUserRoleList = new List<UserRole>();
                var masterUserProfileRoleList = new List<UserProfileRole>();

                // lsUserProfileRoles = lsUserProfileRoles.Where(c => c.isRoleAssigned == true).ToList();
                if (lsUserProfileRoles.Count > 0)
                {
                    /// get all roles incl custom roles
                    var masterUserRoleList = _masterContext.UserRole.AsNoTracking()   //.Include(t => t.UserRole)
                        .Where(c => ((c.AppGlobalOwnerId==null && c.ChurchBodyId ==null) || (c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id))).ToList();

                    for (var i = 0; i < lsUserProfileRoles.Count; i++)
                    { 
                        lsUserProfileRoles[i].UserRole = masterUserRoleList.Where(c => c.Id == lsUserProfileRoles[i].UserRoleId).FirstOrDefault();
                        //userProRole.UserProfile = masterUserProfileRoleList.Where(c => c.UserRoleId == userProRole.UserRoleId).FirstOrDefault();
                        //load one-time
                        if (oUserProfile == null) oUserProfile = _masterContext.UserProfile.Include(t => t.ChurchBody)
                                .Where(c => c.AppGlobalOwnerId == lsUserProfileRoles[i].AppGlobalOwnerId && c.ChurchBodyId==lsUserProfileRoles[i].ChurchBodyId && 
                                            c.Id==lsUserProfileRoles[i].UserProfileId).FirstOrDefault();
                    }

                    // affirm to client roles... in case
                    lsUserProfileRoles = lsUserProfileRoles.Where(c => ((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                                                                       (oUserProfile.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15)).ToList();
                }

                //  masterUserRoleList = _masterContext.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).ToList();
                masterUserProfileRoleList = _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                            .Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).ToList();

                foreach (var oUPR in lsUserProfileRoles)
                {
                    var existUPR = masterUserProfileRoleList.Where(c=> c.UserRoleId == oUPR.UserRoleId && c.UserProfileId== oUPR.UserProfileId).FirstOrDefault();

                    if (existUPR == null && oUPR.isRoleAssigned)
                    {
                       // if (oUPR.isRoleAssigned == true)
                       // {
                            var oUPRAdded = new UserProfileRole
                            {
                                AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id,
                                ChurchBodyId = this._oLoggedCB_MSTR.Id,
                                UserRoleId = oUPR.UserRoleId,
                                UserProfileId = oUPR.UserProfileId,
                                Strt = tm,
                                Expr = (DateTime?)null,
                                ProfileRoleStatus = "A",
                                Created = tm,
                                LastMod = tm,
                                CreatedByUserId = this._oLoggedUser.Id,
                                LastModByUserId = this._oLoggedUser.Id,
                            };

                            _masterContext.Add(oUPRAdded);
                            rowsAdded++;
                        //}
                    }

                    else if (existUPR != null)
                    {
                        if (!oUPR.isRoleAssigned)
                        {
                            // unchecked.. delete
                            _masterContext.Remove(existUPR);
                            rowsDeleted++;
                        }

                        else
                        {
                            //existUPR.AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id;
                            //existUPR.ChurchBodyId = this._oLoggedCB_MSTR.Id;
                            existUPR.UserRoleId = oUPR.UserRoleId;
                            existUPR.UserProfileId = oUPR.UserProfileId;
                            //existUPR.Strt = oUPR.Strt;
                            //existUPR.Expr = oUPR.Expr;
                            //existUPR.ProfileRoleStatus = oUPR.ProfileRoleStatus;
                            //existUPR.Created = tm;
                            existUPR.LastMod = tm;
                            //existUPR.CreatedByUserId = this._oLoggedUser.Id;
                            existUPR.LastModByUserId = this._oLoggedUser.Id;

                            _masterContext.Update(existUPR);
                            rowsUpdated++;
                        }
                    }
                }

                //var oUserProfile = _masterContext.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).FirstOrDefault();
                //var strUserProfile = oUserProfile != null ? oUserProfile.UserDesc : "";

                if (oUserProfile != null)
                {
                    var _userTask = "user"; var strUserProfile = oUserProfile.UserDesc;

                    // prompt users
                    if (rowsAdded > 0)
                    {
                        _userTask = "Added " + rowsAdded + " user roles to [" + strUserProfile + "] profile";
                        ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user roles added.";
                    }
                    if (rowsUpdated > 0)
                    {
                        _userTask = "Updated " + rowsUpdated + " user roles on [" + strUserProfile + "] profile";
                        ViewBag.UserMsg += ". " + rowsUpdated + " user roles updated.";
                    }
                    if (rowsDeleted > 0)
                    {
                        _userTask = "Deleted " + rowsDeleted + " user roles from [" + strUserProfile + "] profile";
                        ViewBag.UserMsg += ". " + rowsDeleted + " user roles deleted.";
                    }

                    if ((rowsAdded + rowsUpdated + rowsDeleted) > 0)
                    {
                        //save changes... 
                        _masterContext.SaveChanges();

                       // DetachAllEntities(_permCtx);

                        var _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                          "RCMS-Admin: User Profile Role", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, this._oLoggedUser.Id, _tm, _tm, this._oLoggedUser.Id, this._oLoggedUser.Id));
                    }
                                         

                    //// ADD PERMISSIONS ON THE ASSIGNED ROLES
                    

                    /// 
                    //// get updated roles... and update permission mapping tab
                    //var lsAssignedRoles = (from t_upr in _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserProfile)
                    //                       .Where(c => c.AppGlobalOwnerId==oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId && c.UserProfileId==oUserProfile.Id)
                    //                       from t_ur in _masterContext.UserRole.AsNoTracking()
                    //                       .Where(c => ((c.AppGlobalOwnerId==null && c.ChurchBodyId==null) || (c.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId)) && 
                    //                                    c.Id == t_upr.UserRoleId && (((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.RoleLevel >= 6 && c.RoleLevel <= 10) || (oUserProfile.ChurchBody.OrgType == "CN" && c.RoleLevel >= 11 && c.RoleLevel <= 15))
                    //                       )
                    //                       select t_ur)
                    //                      .Distinct().ToList();

                    //var existUserPermissions = (from t_ur in lsAssignedRoles
                    //                           from t_urp in _masterContext.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRoleId == t_ur.Id)
                    //                           select t_urp)
                    //                           .Distinct().ToList();

                    //var masterCL_UserPermList = (from upm in _masterContext.UserPermission.Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0"))                                                                                                                                                       // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                    //                            select upm).ToList();

                    //////create the user permissions
                    ////var oUtil = new AppUtilties();
                    ////var permList = oUtil.GetSystem_Administration_Permissions();  // A0   ... SYS/SUP_ADMN/SYS_ADMN/SYS_CUST/SYS_???
                    ////var permList1 = oUtil.GetAppDashboard_Permissions();  // 00           ***
                    ////var permList2 = oUtil.GetAppConfigurations_Permissions();   // 01     ... CH_ADMN/CF_ADMN
                    ////var permList3 = oUtil.GetMemberRegister_Permissions();   // 02        ... CH_RGSTR/CF_RGSTR
                    ////var permList4 = oUtil.GetChurchlifeAndEvents_Permissions();   // 03   ... CH_RGSTR/CF_RGSTR
                    ////var permList5 = oUtil.GetChurchAdministration_Permissions();   // 04  ... CH_RGSTR/CF_RGSTR
                    ////var permList6 = oUtil.GetFinanceManagement_Permissions();   // 05     ... CH_ACCT/CF_ACCT
                    ////var permList7 = oUtil.GetReportsAnalytics_Permissions();   // 06      ***

                    //////var permList3 = oUtil.get();
                    ////permList = AppUtilties.CombineCollection(permList, permList1, permList2, permList3, permList4);
                    ////permList = AppUtilties.CombineCollection(permList, permList5, permList6, permList7);

                    //var permRowsUpd = 0; var permRowsAdd = 0; var permRowsDel = 0;
                    //foreach (var oUserRole in lsAssignedRoles)
                    //{
                    //    //var oChuAdminRole = _masterContext.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" &&
                    //    //                                                ((oUP.ChurchBody.OrgType == "CH" && c.RoleType == "CH_ADMN" && c.RoleLevel == 6) ||
                    //    //                                                 (oUP.ChurchBody.OrgType == "CN" && c.RoleType == "CF_ADMN" && c.RoleLevel == 11)))
                    //    //                                     .FirstOrDefault();


                    //    // assign all privileges 
                    //    //var existUserRolePerms = (from upr in _masterContext.UserRolePermission.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A" && 
                    //    //                          c.UserRoleId == oUserRole.UserRoleId && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                    //    //                          select upr).ToList();
                    //    //if (existUserRolePerms.Count() > 0)
                    //    //{

                    //    var arrPermKey = new List<string>();
                    //    arrPermKey.Add("00");  // dash
                    //    if (oUserRole.RoleType== "CH_ADMN" || oUserRole.RoleType == "CF_ADMN") arrPermKey.Add("01");  // admin
                    //    if (oUserRole.RoleType== "CH_RGSTR" || oUserRole.RoleType == "CF_RGSTR") { arrPermKey.Add("02"); arrPermKey.Add("03"); arrPermKey.Add("04"); }  // mem reg, churchlife, chu admin
                    //    if (oUserRole.RoleType== "CH_ACCT" || oUserRole.RoleType == "CF_ACCT") arrPermKey.Add("05");  // chu fin
                    //    arrPermKey.Add("06");  // reports/analytics

                         
                    //    // get only [00_] Church level Admin permissions ... 
                    //    //var oUserPerms = (from upr in _masterContext.UserPermission.Where(c => c.PermStatus == "A" && arrPermKey.Contains(c.PermissionCode.Length > 0 ? c.PermissionCode.Substring(0, 2) : null))  // c.PermissionCode.StartsWith("01")) 
                    //    //                  select upr)
                    //    //                  .Distinct().ToList();

                    //     var oUserPerms = (from upm in masterCL_UserPermList.Where(c => c.PermStatus == "A" && arrPermKey.Contains(c.PermissionCode.Length > 0 ? c.PermissionCode.Substring(0, 2) : null))  // c.PermissionCode.StartsWith("01")) 
                    //                      select upm)
                    //                      .Distinct().ToList();

                    //    if (oUserPerms.Count() > 0) //(existUserRolePerms.Count() < oUserPerms.Count())
                    //    {
                    //        var isRowUpd = false; 
                    //        foreach (var oURP in oUserPerms)
                    //        {
                    //            var existUserRolePerm = existUserPermissions.Where(c => c.UserPermissionId == oURP.Id).FirstOrDefault();
                    //            if (existUserRolePerm == null)
                    //            {
                    //                var oUserRolePerm = new UserRolePermission
                    //                {
                    //                    AppGlobalOwnerId = null, // oUserProfile.AppGlobalOwnerId,
                    //                    ChurchBodyId = null, // oUserProfile.ChurchBodyId,
                    //                    UserRoleId = oUserRole.Id,
                    //                    UserPermissionId = oURP.Id,
                    //                    ViewPerm = true,
                    //                    CreatePerm = true,
                    //                    EditPerm = true,
                    //                    DeletePerm = true,
                    //                    ManagePerm = true,
                    //                    Status = "A",
                    //                    Created = tm,
                    //                    LastMod = tm,
                    //                    CreatedByUserId = this._oLoggedUser.Id,
                    //                    LastModByUserId = this._oLoggedUser.Id
                    //                };

                    //                _masterContext.Add(oUserRolePerm);
                    //                permRowsAdd++;
                    //            }
                    //            else
                    //            {
                    //                isRowUpd = false;
                    //                if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = true; isRowUpd = true; }

                    //                if (isRowUpd)
                    //                {
                    //                    existUserRolePerm.Created = tm;
                    //                    existUserRolePerm.LastMod = tm;
                    //                    existUserRolePerm.CreatedByUserId = this._oLoggedUser.Id;
                    //                    existUserRolePerm.LastModByUserId = this._oLoggedUser.Id;

                    //                    _masterContext.Update(existUserRolePerm);
                    //                    permRowsUpd++;
                    //                }
                    //            }
                    //        } 
                    //    }
                    //}

                    //// prompt users
                    //if (permRowsAdd > 0)
                    //{
                    //    _userTask = "Added " + permRowsAdd + " user permissions to [" + strUserProfile + "] profile";
                    //    ViewBag.UserMsg += Environment.NewLine + " ~ " + permRowsAdd + " user permissions added.";
                    //}
                    //if (permRowsUpd > 0)
                    //{
                    //    _userTask = "Updated " + permRowsUpd + " user permissions on [" + strUserProfile + "] profile";
                    //    ViewBag.UserMsg += ". " + permRowsUpd + " user permissions updated.";
                    //}

                    //if ((permRowsAdd + permRowsUpd) > 0) // permRowsDel
                    //{
                    //    //save changes... 
                    //    _masterContext.SaveChanges();

                    //    // DetachAllEntities(_permCtx);

                    //    var _tm = DateTime.Now;
                    //    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                    //                      "RCMS-Admin: User Role Permission", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, this._oLoggedUser.Id, _tm, _tm, this._oLoggedUser.Id, this._oLoggedUser.Id));

                    //}



                    /// actually... client only access to the URP is via custom roles ONLY ... standard roles MUST BE SET BY THE VENDOR...
                    /// But for now... allow jux first timer roles in... and that's it!

                    var loggedUserId = this._oLoggedUser.Id;
                    var oUPRList = _masterContext.UserProfileRole.Include(t => t.UserRole).AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId && c.UserProfileId == oUserProfile.Id &&
                        (((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) || 
                         (oUserProfile.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15)))
                        .ToList();
                    var oUPMList_MSTR = _masterContext.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0")).ToList();
                    var oUPM_PermListCurr = new List<UserPermission>();
                    /// 
                    //var lsRows_UPR = oUPRList;    //  if (oUPRList.Count > 0) 
                    var cntURPChanges_Add = 0; var cntURPChanges_Upd = 0; var isRowUpd = false;
                    var totURPChanges_Add = 0; var totURPChanges_Upd = 0; var updRoleIdList = new List<string>();
                    foreach (var oUPR in oUPRList)
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
                                var oURPExist = _masterContext.UserRolePermission.AsNoTracking().Where(c => c.UserRoleId == oUPR.UserRoleId &&
                                                                            (c.UserPermissionId == oUPMCurr.Id || c.UserPermission.PermissionCode == oUPMCurr.PermissionCode)).FirstOrDefault();
                                if (oURPExist == null)
                                {
                                    var _allowCruD = (oUPR.UserRole.RoleLevel == 1 && oUPMCurr.PermissionCode != "A0_00") || (oUPR.UserRole.RoleLevel == 2 && oUPMCurr.PermissionCode != "A0_01") || (oUPR.UserRole.RoleLevel == 3 && oUPMCurr.PermissionCode != "A0_02") || (oUPR.UserRole.RoleLevel == 4 && oUPMCurr.PermissionCode != "A0_04") ||
                                                    ((oUPR.UserRole.RoleLevel >= 6 || oUPR.UserRole.RoleLevel <= 15) && !oUPMCurr.PermissionCode.StartsWith("A0"));
                                    ///
                                    _masterContext.Add(new UserRolePermission()
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

                                        _masterContext.Update(oURPExist);
                                        cntURPChanges_Upd++;
                                    }
                                }
                            }

                            // update....
                            if ((cntURPChanges_Add + cntURPChanges_Upd) > 0)
                            {

                                _masterContext.SaveChanges();
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
                        if (totURPChanges_Add > 0) ViewBag.UserMsg += Environment.NewLine + " ~ " + totURPChanges_Add + " user permissions added.";
                        if (totURPChanges_Upd > 0) ViewBag.UserMsg += ". " + totURPChanges_Upd + " user permissions updated.";
                        ///

                        var _tm = DateTime.Now;
                        _userTask = "Assigned " + totURPChanges_Add + ", updated [" + totURPChanges_Upd + "] permission(s) to [available] roles successfully"; _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, loggedUserId, _tm, _tm, loggedUserId, loggedUserId));
                    }

                    if (string.IsNullOrEmpty(_userTask)) _userTask = "No update made on user profile.";

                    // promtp user
                    return Json(new { taskSuccess = true, oCurrId = 0, userMess = ViewBag.UserMsg, signOutToLogIn = false });
                }

                return Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile roles. User profile not found.", signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile roles. Err: " + ex.Message, signOutToLogIn = false });
            }


           // //Check for NULL.
           // if (lsUserProfileRoles == null)
           //     {
           //     lsUserProfileRoles = new List<UserProfileRole>();
           //     }

           //     //Loop and insert records.
           //     foreach (var oUPR in lsUserProfileRoles)
           //     {
           //         _masterContext.UserProfileRole.Add(oUPR);
           //         //_masterContext.user.Add(oUPR);
           //     }

           //     int insertedRecords = _masterContext.SaveChanges();
           //     return Json(insertedRecords);
           ////}
           ///

        }


        private bool Configure_CL_UPR (List<UserProfileRole> lsUserProfileRoles)
        {
            //List<UserProfileRole> lsUserProfileRoles = null;
            var strDesc = "User profile roles";
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
                // ADD ROLES... add non-exist but checked, update... remove unchecked roles                 
                var rowsUpdated = 0; var rowsAdded = 0; var tm = DateTime.Now; var rowsDeleted = 0;
                UserProfile oUserProfile = null;
                
                var masterUserProfileRoleList = new List<UserProfileRole>(); 
                if (lsUserProfileRoles.Count > 0)
                {
                    /// get all roles incl custom roles
                    var masterUserRoleList = _masterContext.UserRole.AsNoTracking()   //.Include(t => t.UserRole)
                        .Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null) || (c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id))).ToList();

                    for (var i = 0; i < lsUserProfileRoles.Count; i++)
                    {
                        lsUserProfileRoles[i].UserRole = masterUserRoleList.Where(c => c.Id == lsUserProfileRoles[i].UserRoleId).FirstOrDefault();
                         
                        //load one-time
                        if (oUserProfile == null) oUserProfile = _masterContext.UserProfile.Include(t => t.ChurchBody)
                                .Where(c => c.AppGlobalOwnerId == lsUserProfileRoles[i].AppGlobalOwnerId && c.ChurchBodyId == lsUserProfileRoles[i].ChurchBodyId &&
                                            c.Id == lsUserProfileRoles[i].UserProfileId).FirstOrDefault();
                    }

                    // affirm to client roles... in case
                    lsUserProfileRoles = lsUserProfileRoles.Where(c => ((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                                                                       (oUserProfile.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15)).ToList();
                }
                 
                masterUserProfileRoleList = _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserRole)
                                            .Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).ToList();

                foreach (var oUPR in lsUserProfileRoles)
                {
                    var existUPR = masterUserProfileRoleList.Where(c => c.UserRoleId == oUPR.UserRoleId && c.UserProfileId == oUPR.UserProfileId).FirstOrDefault();

                    if (existUPR == null && oUPR.isRoleAssigned)
                    { 
                        var oUPRAdded = new UserProfileRole
                        {
                            AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id,
                            ChurchBodyId = this._oLoggedCB_MSTR.Id,
                            UserRoleId = oUPR.UserRoleId,
                            UserProfileId = oUPR.UserProfileId,
                            Strt = tm,
                            Expr = (DateTime?)null,
                            ProfileRoleStatus = "A",
                            Created = tm,
                            LastMod = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id,
                        };

                        _masterContext.Add(oUPRAdded);
                        rowsAdded++; 
                    }

                    else if (existUPR != null)
                    {
                        if (!oUPR.isRoleAssigned)
                        {
                            // unchecked.. delete
                            _masterContext.Remove(existUPR);
                            rowsDeleted++;
                        }

                        else
                        {
                            //existUPR.AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id;
                            //existUPR.ChurchBodyId = this._oLoggedCB_MSTR.Id;
                            existUPR.UserRoleId = oUPR.UserRoleId;
                            existUPR.UserProfileId = oUPR.UserProfileId;
                            //existUPR.Strt = oUPR.Strt;
                            //existUPR.Expr = oUPR.Expr;
                            //existUPR.ProfileRoleStatus = oUPR.ProfileRoleStatus;
                            //existUPR.Created = tm;
                            existUPR.LastMod = tm;
                            //existUPR.CreatedByUserId = this._oLoggedUser.Id;
                            existUPR.LastModByUserId = this._oLoggedUser.Id;

                            _masterContext.Update(existUPR);
                            rowsUpdated++;
                        }
                    }
                }
                 
                if (oUserProfile != null)
                {
                    var _userTask = "user"; var strUserProfile = oUserProfile.UserDesc;

                    // prompt users
                    if (rowsAdded > 0)
                    {
                        _userTask = "Added " + rowsAdded + " user roles to [" + strUserProfile + "] profile";
                        ViewBag.strResUPR_CL_Task += Environment.NewLine + " ~ " + rowsAdded + " user roles added.";
                    }
                    if (rowsUpdated > 0)
                    {
                        _userTask = "Updated " + rowsUpdated + " user roles on [" + strUserProfile + "] profile";
                        ViewBag.strResUPR_CL_Task += ". " + rowsUpdated + " user roles updated.";
                    }
                    if (rowsDeleted > 0)
                    {
                        _userTask = "Deleted " + rowsDeleted + " user roles from [" + strUserProfile + "] profile";
                        ViewBag.strResUPR_CL_Task += ". " + rowsDeleted + " user roles deleted.";
                    }

                    if ((rowsAdded + rowsUpdated + rowsDeleted) > 0)
                    {
                        //save changes... 
                        _masterContext.SaveChanges();

                        // DetachAllEntities(_permCtx);

                        var _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                          "RCMS-Admin: User Profile Role", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, this._oLoggedUser.Id, _tm, _tm, this._oLoggedUser.Id, this._oLoggedUser.Id));
                    }


                    //// ADD PERMISSIONS ON THE ASSIGNED ROLES 
                    /// 
                    //// get updated roles... and update permission mapping tab
                    //var lsAssignedRoles = (from t_upr in _masterContext.UserProfileRole.AsNoTracking().Include(t => t.UserProfile)
                    //                       .Where(c => c.AppGlobalOwnerId==oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId && c.UserProfileId==oUserProfile.Id)
                    //                       from t_ur in _masterContext.UserRole.AsNoTracking()
                    //                       .Where(c => ((c.AppGlobalOwnerId==null && c.ChurchBodyId==null) || (c.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId)) && 
                    //                                    c.Id == t_upr.UserRoleId && (((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.RoleLevel >= 6 && c.RoleLevel <= 10) || (oUserProfile.ChurchBody.OrgType == "CN" && c.RoleLevel >= 11 && c.RoleLevel <= 15))
                    //                       )
                    //                       select t_ur)
                    //                      .Distinct().ToList();

                    //var existUserPermissions = (from t_ur in lsAssignedRoles
                    //                           from t_urp in _masterContext.UserRolePermission.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserRoleId == t_ur.Id)
                    //                           select t_urp)
                    //                           .Distinct().ToList();

                    //var masterCL_UserPermList = (from upm in _masterContext.UserPermission.Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0"))                                                                                                                                                       // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                    //                            select upm).ToList();

                    //////create the user permissions
                    ////var oUtil = new AppUtilties();
                    ////var permList = oUtil.GetSystem_Administration_Permissions();  // A0   ... SYS/SUP_ADMN/SYS_ADMN/SYS_CUST/SYS_???
                    ////var permList1 = oUtil.GetAppDashboard_Permissions();  // 00           ***
                    ////var permList2 = oUtil.GetAppConfigurations_Permissions();   // 01     ... CH_ADMN/CF_ADMN
                    ////var permList3 = oUtil.GetMemberRegister_Permissions();   // 02        ... CH_RGSTR/CF_RGSTR
                    ////var permList4 = oUtil.GetChurchlifeAndEvents_Permissions();   // 03   ... CH_RGSTR/CF_RGSTR
                    ////var permList5 = oUtil.GetChurchAdministration_Permissions();   // 04  ... CH_RGSTR/CF_RGSTR
                    ////var permList6 = oUtil.GetFinanceManagement_Permissions();   // 05     ... CH_ACCT/CF_ACCT
                    ////var permList7 = oUtil.GetReportsAnalytics_Permissions();   // 06      ***

                    //////var permList3 = oUtil.get();
                    ////permList = AppUtilties.CombineCollection(permList, permList1, permList2, permList3, permList4);
                    ////permList = AppUtilties.CombineCollection(permList, permList5, permList6, permList7);

                    //var permRowsUpd = 0; var permRowsAdd = 0; var permRowsDel = 0;
                    //foreach (var oUserRole in lsAssignedRoles)
                    //{
                    //    //var oChuAdminRole = _masterContext.UserRole.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.RoleStatus == "A" &&
                    //    //                                                ((oUP.ChurchBody.OrgType == "CH" && c.RoleType == "CH_ADMN" && c.RoleLevel == 6) ||
                    //    //                                                 (oUP.ChurchBody.OrgType == "CN" && c.RoleType == "CF_ADMN" && c.RoleLevel == 11)))
                    //    //                                     .FirstOrDefault();


                    //    // assign all privileges 
                    //    //var existUserRolePerms = (from upr in _masterContext.UserRolePermission.Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.Status == "A" && 
                    //    //                          c.UserRoleId == oUserRole.UserRoleId && c.UserRole.RoleStatus == "A") // && (c.Strt == null || c.Strt <= DateTime.Now) && (c.Expr == null || c.Expr >= DateTime.Now))     // from up in permCtx.UserRole.Where(c => c.Id == upr.UserRoleId && c.ChurchBodyId == null && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                    //    //                          select upr).ToList();
                    //    //if (existUserRolePerms.Count() > 0)
                    //    //{

                    //    var arrPermKey = new List<string>();
                    //    arrPermKey.Add("00");  // dash
                    //    if (oUserRole.RoleType== "CH_ADMN" || oUserRole.RoleType == "CF_ADMN") arrPermKey.Add("01");  // admin
                    //    if (oUserRole.RoleType== "CH_RGSTR" || oUserRole.RoleType == "CF_RGSTR") { arrPermKey.Add("02"); arrPermKey.Add("03"); arrPermKey.Add("04"); }  // mem reg, churchlife, chu admin
                    //    if (oUserRole.RoleType== "CH_ACCT" || oUserRole.RoleType == "CF_ACCT") arrPermKey.Add("05");  // chu fin
                    //    arrPermKey.Add("06");  // reports/analytics


                    //    // get only [00_] Church level Admin permissions ... 
                    //    //var oUserPerms = (from upr in _masterContext.UserPermission.Where(c => c.PermStatus == "A" && arrPermKey.Contains(c.PermissionCode.Length > 0 ? c.PermissionCode.Substring(0, 2) : null))  // c.PermissionCode.StartsWith("01")) 
                    //    //                  select upr)
                    //    //                  .Distinct().ToList();

                    //     var oUserPerms = (from upm in masterCL_UserPermList.Where(c => c.PermStatus == "A" && arrPermKey.Contains(c.PermissionCode.Length > 0 ? c.PermissionCode.Substring(0, 2) : null))  // c.PermissionCode.StartsWith("01")) 
                    //                      select upm)
                    //                      .Distinct().ToList();

                    //    if (oUserPerms.Count() > 0) //(existUserRolePerms.Count() < oUserPerms.Count())
                    //    {
                    //        var isRowUpd = false; 
                    //        foreach (var oURP in oUserPerms)
                    //        {
                    //            var existUserRolePerm = existUserPermissions.Where(c => c.UserPermissionId == oURP.Id).FirstOrDefault();
                    //            if (existUserRolePerm == null)
                    //            {
                    //                var oUserRolePerm = new UserRolePermission
                    //                {
                    //                    AppGlobalOwnerId = null, // oUserProfile.AppGlobalOwnerId,
                    //                    ChurchBodyId = null, // oUserProfile.ChurchBodyId,
                    //                    UserRoleId = oUserRole.Id,
                    //                    UserPermissionId = oURP.Id,
                    //                    ViewPerm = true,
                    //                    CreatePerm = true,
                    //                    EditPerm = true,
                    //                    DeletePerm = true,
                    //                    ManagePerm = true,
                    //                    Status = "A",
                    //                    Created = tm,
                    //                    LastMod = tm,
                    //                    CreatedByUserId = this._oLoggedUser.Id,
                    //                    LastModByUserId = this._oLoggedUser.Id
                    //                };

                    //                _masterContext.Add(oUserRolePerm);
                    //                permRowsAdd++;
                    //            }
                    //            else
                    //            {
                    //                isRowUpd = false;
                    //                if (!existUserRolePerm.ViewPerm) { existUserRolePerm.ViewPerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.CreatePerm) { existUserRolePerm.CreatePerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.EditPerm) { existUserRolePerm.EditPerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.DeletePerm) { existUserRolePerm.DeletePerm = true; isRowUpd = true; }
                    //                if (!existUserRolePerm.ManagePerm) { existUserRolePerm.ManagePerm = true; isRowUpd = true; }

                    //                if (isRowUpd)
                    //                {
                    //                    existUserRolePerm.Created = tm;
                    //                    existUserRolePerm.LastMod = tm;
                    //                    existUserRolePerm.CreatedByUserId = this._oLoggedUser.Id;
                    //                    existUserRolePerm.LastModByUserId = this._oLoggedUser.Id;

                    //                    _masterContext.Update(existUserRolePerm);
                    //                    permRowsUpd++;
                    //                }
                    //            }
                    //        } 
                    //    }
                    //}

                    //// prompt users
                    //if (permRowsAdd > 0)
                    //{
                    //    _userTask = "Added " + permRowsAdd + " user permissions to [" + strUserProfile + "] profile";
                    //    ViewBag.UserMsg += Environment.NewLine + " ~ " + permRowsAdd + " user permissions added.";
                    //}
                    //if (permRowsUpd > 0)
                    //{
                    //    _userTask = "Updated " + permRowsUpd + " user permissions on [" + strUserProfile + "] profile";
                    //    ViewBag.UserMsg += ". " + permRowsUpd + " user permissions updated.";
                    //}

                    //if ((permRowsAdd + permRowsUpd) > 0) // permRowsDel
                    //{
                    //    //save changes... 
                    //    _masterContext.SaveChanges();

                    //    // DetachAllEntities(_permCtx);

                    //    var _tm = DateTime.Now;
                    //    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                    //                      "RCMS-Admin: User Role Permission", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, this._oLoggedUser.Id, _tm, _tm, this._oLoggedUser.Id, this._oLoggedUser.Id));

                    //}



                    /// actually... client only access to the URP is via custom roles ONLY ... standard roles MUST BE SET BY THE VENDOR...
                    /// But for now... allow jux first timer roles in... and that's it!

                    var loggedUserId = this._oLoggedUser.Id;
                    var oUPRList = _masterContext.UserProfileRole.Include(t => t.UserRole).AsNoTracking()
                        .Where(c => c.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserProfile.ChurchBodyId && c.UserProfileId == oUserProfile.Id &&
                        (((oUserProfile.ChurchBody.OrgType == "CR" || oUserProfile.ChurchBody.OrgType == "CH") && c.UserRole.RoleLevel >= 6 && c.UserRole.RoleLevel <= 10) ||
                         (oUserProfile.ChurchBody.OrgType == "CN" && c.UserRole.RoleLevel >= 11 && c.UserRole.RoleLevel <= 15)))
                        .ToList();
                    var oUPMList_MSTR = _masterContext.UserPermission.AsNoTracking().Where(c => c.PermStatus == "A" && !c.PermissionCode.StartsWith("A0")).ToList();
                    var oUPM_PermListCurr = new List<UserPermission>();
                    /// 
                    //var lsRows_UPR = oUPRList;    //  if (oUPRList.Count > 0) 
                    var cntURPChanges_Add = 0; var cntURPChanges_Upd = 0; var isRowUpd = false;
                    var totURPChanges_Add = 0; var totURPChanges_Upd = 0; var updRoleIdList = new List<string>();
                    foreach (var oUPR in oUPRList)
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
                                var oURPExist = _masterContext.UserRolePermission.AsNoTracking().Where(c => c.UserRoleId == oUPR.UserRoleId &&
                                                                            (c.UserPermissionId == oUPMCurr.Id || c.UserPermission.PermissionCode == oUPMCurr.PermissionCode)).FirstOrDefault();
                                if (oURPExist == null)
                                {
                                    var _allowCruD = (oUPR.UserRole.RoleLevel == 1 && oUPMCurr.PermissionCode != "A0_00") || (oUPR.UserRole.RoleLevel == 2 && oUPMCurr.PermissionCode != "A0_01") || (oUPR.UserRole.RoleLevel == 3 && oUPMCurr.PermissionCode != "A0_02") || (oUPR.UserRole.RoleLevel == 4 && oUPMCurr.PermissionCode != "A0_04") ||
                                                    ((oUPR.UserRole.RoleLevel >= 6 || oUPR.UserRole.RoleLevel <= 15) && !oUPMCurr.PermissionCode.StartsWith("A0"));
                                    ///
                                    _masterContext.Add(new UserRolePermission()
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

                                        _masterContext.Update(oURPExist);
                                        cntURPChanges_Upd++;
                                    }
                                }
                            }

                            // update....
                            if ((cntURPChanges_Add + cntURPChanges_Upd) > 0)
                            {

                                _masterContext.SaveChanges();
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
                    }

                    if (string.IsNullOrEmpty(_userTask)) _userTask = "No update made on user profile.";

                    // promtp user    ... ViewBag.strResUPR_CL_Task
                    return true;  /// Json(new { taskSuccess = true, oCurrId = 0, userMess = ViewBag.UserMsg, signOutToLogIn = false });
                }


                ///fail... 
                ViewBag.strResUPR_CL_Task = "Failed saving user profile roles. User profile not found.";
                return false;  /// Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile roles. User profile not found.", signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                ViewBag.strResUPR_CL_Task = "Failed saving user profile roles. Err: " + ex.Message;
                return false;  /// Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile roles. Err: " + ex.Message, signOutToLogIn = false });
            }


            // //Check for NULL.
            // if (lsUserProfileRoles == null)
            //     {
            //     lsUserProfileRoles = new List<UserProfileRole>();
            //     }

            //     //Loop and insert records.
            //     foreach (var oUPR in lsUserProfileRoles)
            //     {
            //         _masterContext.UserProfileRole.Add(oUPR);
            //         //_masterContext.user.Add(oUPR);
            //     }

            //     int insertedRecords = _masterContext.SaveChanges();
            //     return Json(insertedRecords);
            ////}
            ///

        }

        public JsonResult AddMod_CL_UPG(List<UserProfileGroup> lsUserProfileGroups)
        {
            var strDesc = "User profile groups";
            //if (!InitializeUserLogging())
            //    return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data unavailable. Please refresh and try again.", pageIndex = 0 });  //RedirectToAction("LoginUserAcc", "UserLogin");

            // var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); 

            if (lsUserProfileGroups == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = 0 });
            if (lsUserProfileGroups.Count == 0) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No data unavailable. Add " + strDesc + " data and try again.", pageIndex = 0 });

            try
            {
                // UserProfile _oChanges = vm.oUserProfile;

                //using (CustomersEntities entities = new CustomersEntities())
                //{
                //Truncate Table to delete all old records.
                //_masterContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Customers]");


                var rowsUpdated = 0; var rowsAdded = 0; var tm = DateTime.Now;
                foreach (var oUPG in lsUserProfileGroups)
                {
                    var existUPG = _masterContext.UserProfileGroup.Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).FirstOrDefault();
                    if (existUPG == null)
                    {
                        var oUPGAdded = new UserProfileGroup
                        {
                            AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id,
                            ChurchBodyId = this._oLoggedCB_MSTR.Id,
                            UserGroupId = oUPG.UserGroupId,
                            UserProfileId = oUPG.UserProfileId,
                            Strt = tm,
                            Expr = (DateTime?)null,
                            Status = "A", 
                            Created = tm, 
                            LastMod = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id,
                        };

                        _masterContext.Add(oUPGAdded);
                        rowsAdded++;
                    }
                    else
                    {
                        //existUPG.AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id;
                        //existUPG.ChurchBodyId = this._oLoggedCB_MSTR.Id;
                        existUPG.UserGroupId = oUPG.UserGroupId;
                        existUPG.UserProfileId = oUPG.UserProfileId;
                        existUPG.Strt = oUPG.Strt;
                        existUPG.Expr = oUPG.Expr;
                        existUPG.Status = oUPG.Status;
                        //existUPG.Created = tm;
                        existUPG.LastMod = tm;
                        //existUPG.CreatedByUserId = this._oLoggedUser.Id;
                        existUPG.LastModByUserId = this._oLoggedUser.Id;

                        _masterContext.Update(existUPG);
                        rowsUpdated++;
                    }
                }

                var oUserProfile = _masterContext.UserProfile.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).FirstOrDefault();
                var strUserProfile = oUserProfile != null ? oUserProfile.UserDesc : "";
                var _userTask = "user";
                // prompt users
                if (rowsAdded > 0)
                {
                    _userTask = "Added " + rowsAdded + " user groups to [" + strUserProfile + "] profile";
                    ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user groups added.";
                }
                if (rowsUpdated > 0)
                {
                    _userTask = "Updated " + rowsUpdated + " user groups on [" + strUserProfile + "] profile";
                    ViewBag.UserMsg += ". " + rowsUpdated + " user groups updated.";
                }

                if ((rowsAdded + rowsUpdated) > 0)
                {
                    //save changes... 
                    _masterContext.SaveChanges();

                    // DetachAllEntities(_permCtx);

                    var _tm = DateTime.Now;
                    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                      "RCMS-Admin: User Profile Group", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, this._oLoggedUser.Id, _tm, _tm, this._oLoggedUser.Id, this._oLoggedUser.Id));
                }

                // promtp user
                return Json(new { taskSuccess = true, oCurrId = 0, userMess = ViewBag.UserMsg, signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user profile groups. Err: " + ex.Message, signOutToLogIn = false });
            }
             

        }
        public JsonResult AddMod_CL_UGR(List<UserGroupRole> lsUserGroupRoles)
        {

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

            var strDesc = "User group roles";
            //if (!InitializeUserLogging())
            //    return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data unavailable. Please refresh and try again.", pageIndex = 0 });  //RedirectToAction("LoginUserAcc", "UserLogin");

            // var _userTask = "Attempted accessing/modifying " + strDesc.ToLower(); 

            if (lsUserGroupRoles == null) return Json(new { taskSuccess = false, oCurrId = "", userMess = strDesc + " data to update unavailable. Please refresh and try again.", pageIndex = 0 });
            if (lsUserGroupRoles.Count == 0) return Json(new { taskSuccess = false, oCurrId = "", userMess = "No data unavailable. Add " + strDesc + " data and try again.", pageIndex = 0 });

            try
            {
                // UserGroup _oChanges = vm.oUserGroup;

                //using (CustomersEntities entities = new CustomersEntities())
                //{
                //Truncate Table to delete all old records.
                //_masterContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Customers]");


                var rowsUpdated = 0; var rowsAdded = 0; var tm = DateTime.Now;
                foreach (var oUGR in lsUserGroupRoles)
                {
                    var existUGR = _masterContext.UserGroupRole.Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).FirstOrDefault();
                    if (existUGR == null)
                    {
                        var oUGRAdded = new UserGroupRole
                        {
                            AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id,
                            ChurchBodyId = this._oLoggedCB_MSTR.Id,
                            UserRoleId = oUGR.UserRoleId,
                            UserGroupId = oUGR.UserGroupId,
                            Strt = tm,
                            Expr = (DateTime?)null,
                            Status = "A", 
                            Created = tm,
                            LastMod = tm,
                            CreatedByUserId = this._oLoggedUser.Id,
                            LastModByUserId = this._oLoggedUser.Id,
                        };

                        _masterContext.Add(oUGRAdded);
                        rowsAdded++;
                    }
                    else
                    {
                        //existUGR.AppGlobalOwnerId = this._oLoggedAGO_MSTR.Id;
                        //existUGR.ChurchBodyId = this._oLoggedCB_MSTR.Id;
                        existUGR.UserRoleId = oUGR.UserRoleId;
                        existUGR.UserGroupId = oUGR.UserGroupId;
                        existUGR.Strt = oUGR.Strt;
                        existUGR.Expr = oUGR.Expr;
                        existUGR.Status = oUGR.Status;
                        //existUGR.Created = tm;
                        existUGR.LastMod = tm;
                        //existUGR.CreatedByUserId = this._oLoggedUser.Id;
                        existUGR.LastModByUserId = this._oLoggedUser.Id;

                        _masterContext.Update(existUGR);
                        rowsUpdated++;
                    }
                }

                var oUserGroup = _masterContext.UserGroup.AsNoTracking().Where(c => c.AppGlobalOwnerId == this._oLoggedAGO_MSTR.Id && c.ChurchBodyId == this._oLoggedCB_MSTR.Id).FirstOrDefault();
                var strUserGroup = oUserGroup != null ? oUserGroup.GroupDesc : "";
                var _userTask = "group";
                // prompt users
                if (rowsAdded > 0)
                {
                    _userTask = "Added " + rowsAdded + " user roles to [" + strUserGroup + "] group";
                    ViewBag.UserMsg += Environment.NewLine + " ~ " + rowsAdded + " user roles added.";
                }
                if (rowsUpdated > 0)
                {
                    _userTask = "Updated " + rowsUpdated + " user roles on [" + strUserGroup + "] group";
                    ViewBag.UserMsg += ". " + rowsUpdated + " user roles updated.";
                }

                if ((rowsAdded + rowsUpdated) > 0)
                {
                    //save changes... 
                    _masterContext.SaveChanges();

                    // DetachAllEntities(_permCtx);

                    var _tm = DateTime.Now;
                    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                      "RCMS-Admin: User Group Role", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, this._oLoggedUser.Id, _tm, _tm, this._oLoggedUser.Id, this._oLoggedUser.Id));
                }

                // promtp user
                return Json(new { taskSuccess = true, oCurrId = 0, userMess = ViewBag.UserMsg, signOutToLogIn = false });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = 0, userMess = "Failed saving user group roles. Err: " + ex.Message, signOutToLogIn = false });
            }


            // //Check for NULL.
            // if (lsUserGroupRoles == null)
            //     {
            //     lsUserGroupRoles = new List<UserGroupRole>();
            //     }

            //     //Loop and insert records.
            //     foreach (var oUGR in lsUserGroupRoles)
            //     {
            //         _masterContext.UserGroupRole.Add(oUGR);
            //         //_masterContext.user.Add(oUGR);
            //     }

            //     int insertedRecords = _masterContext.SaveChanges();
            //     return Json(insertedRecords);
            ////}
            ///

        }


    }
}
