using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RhemaCMS.Controllers.con_adhc;
using RhemaCMS.Models;
using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using RhemaCMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        // private readonly ChurchModelContext _context;
        // private readonly string _clientDBConnString;

      //  private readonly MSTR_DbContext _masterContext;
        // private readonly ChurchModelContext _clientDBContext;

        // private readonly MSTR_DbContext _masterContextLog;

        // private string _clientDBConnString;

        private readonly MSTR_DbContext _masterContext;
        private ChurchModelContext _context;

      //  private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        ///
     //   private string _strClientConn;
       /// private string _clientDBConnString;
        private UserProfile _oLoggedUser;
        // private UserRole _oLoggedRole;
        private MSTRChurchBody _oLoggedCB_MSTR;
        private MSTRAppGlobalOwner _oLoggedAGO_MSTR;

        /// localized
        private ChurchBody _oLoggedCB;
        private AppGlobalOwner _oLoggedAGO;


        // this attr is used by most of the models... save in memory [class var]
        private string strCountryCode_dflt = (string)null;
        private string strCountryName_dflt = "";
        private string strCountryCURR1_dflt = "";
        private string strCountryCURR2_dflt = "";
         
        private CountryCustom oCTRYDefault;
        private ChurchPeriod oCPRDefault;
        private CurrencyCustom oCURRDefault;

        //private bool isCurrValid = false;
        private UserSessionPrivilege oUserLogIn_Priv = null;
        //  private UserProfile oUserProfile_Logged  = null;


         private readonly IConfiguration _configuration;  
         private string _clientDBConn;  


        public HomeController( MSTR_DbContext masterContext,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, ChurchModelContext clientCtx, 
            IConfiguration configuration) //ChurchModelContext context ,, ILogger<HomeController> logger )
        {
            try
            {
                // _context = context;
                _masterContext = masterContext; 
                _configuration = configuration;  ///  private readonly IConfiguration _configuration; /// IConfiguration configuration


                //  _masterContextLog = new MSTR_DbContext();
                //  _logger = logger;  

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
                    this._oLoggedCB = this.oUserLogIn_Priv.ChurchBody_CLNT; //
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

                    //_context = GetClientDBContext();

                //else
                //{
                //    var cs =  clientCtx.Database.GetDbConnection().ConnectionString;
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
                //           /// this._clientDBConnString = clientCtx.Database.GetDbConnection().ConnectionString;
                //            _clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));
                //        }
                //   // }
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
            }
            catch (Exception ex)
            {
                RedirectToAction("LoginUserAcc", "UserLogin");
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
        //                { RedirectToAction("LoginUserAcc", "UserLogin"); return null; }
        //            }
        //            else
        //            { RedirectToAction("LoginUserAcc", "UserLogin"); return null; }
        //        }
        //    }


        //    //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == this.oUserLogIn_Priv.UserProfile.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //    ////var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == 4 && c.Status == "A").FirstOrDefault();
        //    //if (oClientConfig != null)
        //    //{
        //    //    //// get and mod the conn
        //    //    //var _clientDBConnString = "";
        //    //    //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //    //    //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
        //    //    //_clientDBConnString = conn.ConnectionString;

        //    //    //// test the NEW DB conn
        //    //    //var _clientContext = new ChurchModelContext(_clientDBConnString);

        //    //    // var _clientDBConnString = "";
        //    //    var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
        //    //    conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
        //    //    conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //    //    conn.IntegratedSecurity = false; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

        //    //    this._clientDBConnString = conn.ConnectionString;

        //    //    // test the NEW DB conn
        //    //    var _clientContext = new ChurchModelContext(_clientDBConnString);

        //    //    if (!_clientContext.Database.CanConnect())
        //    //        RedirectToAction("LoginUserAcc", "UserLogin");

        //    //    //// _oLoggedRole = oUserLogIn_Priv.UserRole; 
        //    //    //this._oLoggedUser = this.oUserLogIn_Priv.UserProfile;
        //    //    //this._oLoggedCB_MSTR = this.oUserLogIn_Priv.ChurchBody;
        //    //    //this._oLoggedAGO_MSTR = this.oUserLogIn_Priv.AppGlobalOwner;
        //    //    //this._oLoggedUser.strChurchCode_AGO = this._oLoggedAGO_MSTR != null ? this._oLoggedAGO_MSTR.GlobalChurchCode : "";
        //    //    //this._oLoggedUser.strChurchCode_CB = this._oLoggedCB_MSTR != null ? this._oLoggedCB_MSTR.GlobalChurchCode : "";

        //    //    ///// synchronize AGO, CL, CB, CTRY  or @login 
        //    //    //// this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

        //    //    ///// get the localized data... using the MSTR data
        //    //    //this._oLoggedAGO = _clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
        //    //    //this._oLoggedCB = _clientContext.ChurchBody.AsNoTracking().Include(t => t.ChurchLevel).Where(c => c.MSTR_AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId &&
        //    //    //                        c.MSTR_ChurchBodyId == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();


        //    //    // load the dash b/f
        //    //    // LoadClientDashboardValues();

        //    //    return _clientContext;
        //    //}

        //    //
        //    // return null;
        //}





        private bool userAuthorized = false;
        //private void _SetUserLogged()
        //{
        //    if (TempData.ContainsKey("UserLogIn_oUserPrivCol"))
        //    {
        //        var tempPrivList = TempData["UserLogIn_oUserPrivCol"] as string;
        //        if (string.IsNullOrEmpty(tempPrivList)) RedirectToAction("LoginUserAcc", "UserLogin");
        //        // De serialize the string to object
        //        this.oUserLogIn_Priv = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
        //        //
        //        isCurrValid = oUserLogIn_Priv.UserSessionPermList?.Count > 0;
        //        if (isCurrValid)
        //        {
        //            //ViewBag.oAppGloOwnLogged = oUserLogIn_Priv.AppGlobalOwner;
        //            //ViewBag.oChuBodyLogged = oUserLogIn_Priv.ChurchBody;
        //            //ViewBag.oUserLogged = oUserLogIn_Priv.UserProfile;

        //            // check permission for Core life...  given the sets of permissions
        //            userAuthorized = true;  // oUserLogIn_Priv.Count > 0; //(oUserLogIn_Priv.Find(x => x.PermissionName == "_A0__System_Administration" || x.PermissionName == "xxx") != null);
        //        }
        //    }

        //    else RedirectToAction("LoginUserAcc", "UserLogin");
        //}


        //private async Task LogUserActivity_AppMainUserAuditTrail (UserAuditTrail oUserTrail) 
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail!=null)
        //    { 
        //        _masterContextLog.UserAuditTrail.Add(oUserTrail);
        //        await _masterContextLog.SaveChangesAsync();
        //    }           
        //}


        //private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail)
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail != null)
        //    {
        //        using (var logCtx = new MSTR_DbContext())
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


        //private ChurchModelContext GetCL_DBContext()
        //{
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
        //                { RedirectToAction("LoginUserAcc", "UserLogin"); return null; }
        //            }
        //            else
        //            { RedirectToAction("LoginUserAcc", "UserLogin"); return null; }
        //        }
        //    }
        //}

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
                    using (var logCtx = new ChurchModelContext(_connstr_CL )) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
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

         
        private async Task LoadClientDashboardValues()  //string strTempConn = "") //, UserProfile oLoggedUser)string clientDBConnString, 
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
            ViewData["bl_IsModAccessVAA4"] = this.oUserLogIn_Priv.IsModAccessVAA4;
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
            ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 5); //res[0].cnt_tdb);
            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_a);
            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d);
            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d); 
            ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);

        }

        private async Task<HomeDashboardVM> LoadClientDashboardValues(HomeDashboardVM oHomeDash, UserProfile oLoggedUser )
        {
            // using (var dashContext = new ChurchModelContext(clientDBConnString))
           
            var _connstr_CL = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId); ///  this.GetCL_DBConnString();
            if (!string.IsNullOrEmpty(_connstr_CL))
            {
                using (var clientContext = new ChurchModelContext(_connstr_CL)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
                {
                    if (clientContext.Database.CanConnect() == false)
                        clientContext.Database.OpenConnection();
                    else if (clientContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                        clientContext.Database.OpenConnection();

                    //get Currency
                    // var curr = clientContext.Currency.Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == oLoggedUser.ChurchBodyId && c.IsBaseCurrency == true).FirstOrDefault();

                    // this attr is used by most of the models... save in memory [class var]
                    oCTRYDefault = clientContext.CountryCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == oLoggedUser.ChurchBodyId && c.IsDefaultCountry == true).FirstOrDefault();

                    ViewData["CB_CurrUsed"] = "GHS";
                    if (oCTRYDefault != null)
                        if (oCTRYDefault.Country != null)
                            ViewData["CB_CurrUsed"] = oCTRYDefault.Country != null ? oCTRYDefault.Country.Curr3LISOSymbol : ""; // curr != null ? curr.Acronym : ""; // "GHS"


                    var oCPR_List_1 = clientContext.ChurchPeriod.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.OwnedByChurchBody).ThenInclude(t=> t.ChurchLevel)
                         .Where(c => c.AppGlobalOwnerId == this._oLoggedAGO.Id).ToList();
                    oCPR_List_1 = oCPR_List_1.Where(c =>
                                                 (c.OwnedByChurchBodyId == this._oLoggedCB.Id ||
                                                 (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "C" && c.OwnedByChurchBodyId == _oLoggedCB.ParentChurchBodyId) ||
                                                 (c.OwnedByChurchBodyId != this._oLoggedCB.Id && c.SharingStatus == "A" && AppUtilties.IsAncestor_ChurchBody(c.OwnedByChurchBody, this._oLoggedCB)))).ToList();

                    var oCPR_List = oCPR_List_1;
                    if (oCPR_List.Count > 0) 
                        oCPR_List = oCPR_List.OrderBy(c=> c.OwnedByChurchBody != null ? (c.OwnedByChurchBody.ChurchLevel != null ? c.OwnedByChurchBody.ChurchLevel.LevelIndex : (int?)null ) : (int?)null ).ToList();
                    var oCPR_Def = oCPR_List_1.FirstOrDefault();  //_context.AppUtilityNVP.Where(c => c.AppGlobalOwnerId == oCurrNVP.AppGlobalOwnerId && c.ChurchBodyId == oCurrNVP.ChurchBodyId &&  c.NVPCode == oCurrNVP.NVPCode && c.Id != oCurrNVP.Id)
                                                                  //        .OrderBy(c => c.EventFrom)
                                                                  //        .ThenBy(c => c.EventTo)
                                                                  //        .ThenBy(c => c.Subject)
                                                                  //        .Select(c => new SelectListItem()
                                                                  //        {
                                                                  //            Value = c.Id.ToString(),
                                                                  //            Text = c.Subject + " [" + (c.EventFrom != null ? String.Format("{0:dddd, MMMM d, yyyy}", c.EventFrom.Value) : "") + "]"  // add the To_date
                                                                  //})
                                                                  //        .ToList();

                    ViewData["strACurrChYear"] = oCPR_Def != null ? oCPR_Def.Year.ToString() : DateTime.Now.Year.ToString() ;     /// "2021/2022";


                    var clientAGO = clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                    var clientCB = clientContext.ChurchBody.AsNoTracking().Include(t=> t.ChurchLevel).Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId == oLoggedUser.ChurchBodyId && c.Status == "A").FirstOrDefault();
                    ///
                    var qrySuccess = false;
                    if (clientAGO != null && clientCB != null)
                    {
                        if (clientCB.OrgType == "CR" || clientCB.OrgType == "CH")
                        {
                            var clientCB_SubCL = clientContext.ChurchLevel.AsNoTracking()
                                .Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.LevelIndex == clientCB.ChurchLevel.LevelIndex - 1).FirstOrDefault();
                            ///
                            oHomeDash.strCL_SubCong = clientCB_SubCL != null ? clientCB_SubCL.CustomName : "Congregations";
                        }
                         
                        var res = await (from dummyRes in new List<string> { "X" }
                                         join tcb_sb in clientContext.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN") &&
                                                             c.AppGlobalOwnerId == clientAGO.Id && c.ParentChurchBodyId == clientCB.Id) on 1 equals 1 into _tcb_sb

                                         // join tcb in clientContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
                                         // join tsr in clientContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr

                                         join tcm in clientContext.CBMemberRollBal.AsNoTracking().Where(c => c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
                                         join ttt in clientContext.CBTitheTransBal.AsNoTracking().Where(c => c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _ttt

                                         //join tcm in clientContext.ChurchMember.Where(c => c.Status == "A" &&
                                         //                    c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
                                         // join tms in clientContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
                                         // join tsubs in clientContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
                                         // join ttc in clientContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
                                         // join tdb in clientContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb

                                         select new
                                         {
                                             cnt_tcb_sb = _tcb_sb.Count(),
                                             cnt_tcm = _tcm.Count() > 0 ? _tcm.ToList()[0].TotRoll : 0,
                                             cnt_tcm_M = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_M : 0,
                                             cnt_tcm_F = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_F : 0,
                                             cnt_tcm_O = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_O : 0,
                                             cnt_tcm_C = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_C : 0,
                                             cnt_tcm_Y = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_Y : 0,
                                             cnt_tcm_YA = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_YA : 0,
                                             cnt_tcm_MA = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_MA : 0,
                                             cnt_tcm_AA = _tcm.Count() > 0 ? _tcm.ToList()[0].Tot_AA : 0,
                                             //
                                             cnt_tcm_OA = _tcm.Count() > 0 ? (_tcm.ToList()[0].Tot_MA + _tcm.ToList()[0].Tot_AA) : 0,
                                             cnt_tcm_GA = _tcm.Count() > 0 ? (_tcm.ToList()[0].Tot_YA + _tcm.ToList()[0].Tot_MA + _tcm.ToList()[0].Tot_AA) : 0,
                                             ///

                                             cnt_TolCol = _ttt.Count() > 0 ? _ttt.ToList()[0].TotAmtCol : 0,
                                             cnt_TolOut = _ttt.Count() > 0 ? _ttt.ToList()[0].TotAmtOut : 0,
                                             cnt_TolNet = _ttt.Count() > 0 ? _ttt.ToList()[0].TotAmtNet : 0,

                                             //cnt_tms = _tms.Count(),
                                             //cnt_tsubs = _tsubs.Count(),
                                             //cnt_tdb = _tdb.Count(),
                                             // cnt_ttc = _ttc.Count(),
                                             //cnt_tcln_d = _tcln_d.Count(),
                                             //cnt_tcln_a = _tcln_a.Count()
                                         })
                                .ToList().ToListAsync();

                        ///
                        if (res.Count() > 0)
                        {
                            qrySuccess = true;
                            ViewData["CB_SubCongCount"] = String.Format("{0:N0}", res[0].cnt_tcb_sb);
                            ViewData["CB_MemListCount"] = String.Format("{0:N0}", res[0].cnt_tcm);
                            ///
                            ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 0); // res[0].cnt_tsubs);
                            ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tdb);
                            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_a);
                            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d);
                            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d); 

                            oHomeDash.strCB_SubCongCount = String.Format("{0:N0}", res[0].cnt_tcb_sb);
                             
                            ///
                            oHomeDash.strCB_MemListCount = String.Format("{0:N0}", res[0].cnt_tcm);
                            oHomeDash.strCB_MemListCount_M = String.Format("{0:N0}", res[0].cnt_tcm_M);
                            oHomeDash.strCB_MemListCount_MPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_M / (double)res[0].cnt_tcm) * 100);   // 0:0.0%
                            oHomeDash.strCB_MemListCount_F = String.Format("{0:N0}", res[0].cnt_tcm_F);
                            oHomeDash.strCB_MemListCount_FPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_F / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_O = String.Format("{0:N0}", res[0].cnt_tcm_O);
                            oHomeDash.strCB_MemListCount_OPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_O / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_C = String.Format("{0:N0}", res[0].cnt_tcm_C);
                            oHomeDash.strCB_MemListCount_CPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_C / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_Y = String.Format("{0:N0}", res[0].cnt_tcm_Y);
                            oHomeDash.strCB_MemListCount_YPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_Y / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_YA = String.Format("{0:N0}", res[0].cnt_tcm_YA);
                            oHomeDash.strCB_MemListCount_YAPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_YA / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_MA = String.Format("{0:N0}", res[0].cnt_tcm_MA);
                            oHomeDash.strCB_MemListCount_MAPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_MA / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_AA = String.Format("{0:N0}", res[0].cnt_tcm_AA);
                            oHomeDash.strCB_MemListCount_AAPc = String.Format("{0:N1}", ((double)res[0].cnt_tcm_AA / (double)res[0].cnt_tcm) * 100);
                            ///
                            oHomeDash.strCB_MemListCount_GA = String.Format("{0:N0}", res[0].cnt_tcm_GA);
                            oHomeDash.strCB_MemListCount_GAPc = String.Format("{0:0.0}", ((double)res[0].cnt_tcm_GA / (double)res[0].cnt_tcm) * 100);
                            oHomeDash.strCB_MemListCount_OA = String.Format("{0:N0}", res[0].cnt_tcm_OA);
                            oHomeDash.strCB_MemListCount_OAPc = String.Format("{0:0.0}", ((double)res[0].cnt_tcm_OA / (double)res[0].cnt_tcm) * 100);
                            ///
                            oHomeDash.strToDateChuPer_TotColTithes = String.Format("{0:N0}", res[0].cnt_TolCol);
                            oHomeDash.strToDateChuPer_TotOutTithes = String.Format("{0:N0}", res[0].cnt_TolOut);
                            oHomeDash.strToDateChuPer_TotNetTithes = String.Format("{0:N0}", res[0].cnt_TolNet);
                        }


                        var resAudits = _masterContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date);
                        // var cnt_ttc = resAudits.Count();
                        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", resAudits.Count());


                        ////String.Format(1234 % 1 == 0 ? "{0:N0}" : "{0:N2}", 1234);
                        //var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId_Logged && c.ChurchBodyId == oChuBodyId_Logged && c.IsBaseCurrency == true).FirstOrDefault(); 
                        //oHomeDash.strCurrUsed = curr != null ? curr.Acronym : ""; // "GHS";
                        //oHomeDash.SupCongCount = String.Format("{0:N0}", 25);
                        //oHomeDash.MemListCount = String.Format("{0:N0}", 4208); ViewBag.MemListCount = oHomeDash.MemListCount;
                        //oHomeDash.NewMemListCount = String.Format("{0:N0}", 17); ViewBag.NewMemListCount = oHomeDash.NewMemListCount;
                        //oHomeDash.NewConvertsCount = String.Format("{0:N0}", 150); ViewBag.NewConvertsCount = oHomeDash.NewConvertsCount;
                        //oHomeDash.VisitorsCount = String.Format("{0:N0}", 9); ViewBag.VisitorsCount = oHomeDash.VisitorsCount;
                        //oHomeDash.ReceiptsAmt = String.Format("{0:N2}", 1700);
                        //oHomeDash.PaymentsAmt = String.Format("{0:N2}", 105.491); 
                    }

                    if (!qrySuccess)
                    {
                        ViewData["CB_SubCongCount"] = String.Format("{0:N0}", 0);
                        ViewData["CB_MemListCount"] = String.Format("{0:N0}", 0);
                        ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
                        ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
                        ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
                        ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
                        ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
                        ///
                        ViewData["Today_AuditCount"] = String.Format("{0:N0}", 0);


                        oHomeDash.strCB_SubCongCount = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_M = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_MPc = String.Format("{0:N1}", 0);   // 0:0.0%
                        oHomeDash.strCB_MemListCount_F = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_FPc = String.Format("{0:N1}", 0);
                        oHomeDash.strCB_MemListCount_O = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_OPc = String.Format("{0:N1}", 0);
                        oHomeDash.strCB_MemListCount_C = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_CPc = String.Format("{0:N1}", 0);
                        oHomeDash.strCB_MemListCount_Y = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_YPc = String.Format("{0:N1}", 0);
                        oHomeDash.strCB_MemListCount_YA = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_YAPc = String.Format("{0:N1}", 0);
                        oHomeDash.strCB_MemListCount_MA = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_MAPc = String.Format("{0:N1}", 0);
                        oHomeDash.strCB_MemListCount_AA = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_AAPc = String.Format("{0:N1}", 0);
                        ///
                        oHomeDash.strCB_MemListCount_GA = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_GAPc = String.Format("{0:0.0}", 0);
                        oHomeDash.strCB_MemListCount_OA = String.Format("{0:N0}", 0);
                        oHomeDash.strCB_MemListCount_OAPc = String.Format("{0:0.0}", 0);
                        ///
                        oHomeDash.strToDateChuPer_TotColTithes = String.Format("{0:0.0}", 0);
                        oHomeDash.strToDateChuPer_TotOutTithes = String.Format("{0:0.0}", 0);
                        oHomeDash.strToDateChuPer_TotNetTithes = String.Format("{0:0.0}", 0);
                    }

                    ///// initialize Model values
                    //oHomeDash.strCB_SubCongCount = ViewData["CB_SubCongCount"] as string;
                    //oHomeDash.strCB_MemListCount = ViewData["CB_MemListCount"] as string;
                    //oHomeDash.strCBWeek_NewMemListCount = ViewData["CBWeek_NewMemListCount"] as string;
                    //oHomeDash.strCBWeek_NewConvertsCount = ViewData["CBWeek_NewConvertsCount"] as string;
                    //oHomeDash.strCBWeek_VisitorsCount = ViewData["CBWeek_VisitorsCount"] as string;
                    //oHomeDash.strCBWeek_ReceiptsAmt = ViewData["CBWeek_ReceiptsAmt"] as string;
                    //oHomeDash.strCBWeek_PaymentsAmt = ViewData["CBWeek_PaymentsAmt"] as string;
                    //oHomeDash.strTodaysAuditCount = ViewData["TodaysAuditCount"] as string;
                    //oHomeDash.strCB_CurrUsed = ViewData["CB_CurrUsed"] as string;

                    // change later... pick set curr
                    ViewData["CB_CurrUsed"] = "GHS";

                    oHomeDash.strCBWeek_NewMemListCount = ViewData["CBWeek_NewMemListCount"] as string;
                    oHomeDash.strCBWeek_NewConvertsCount = ViewData["CBWeek_NewConvertsCount"] as string;
                    oHomeDash.strCBWeek_VisitorsCount = ViewData["CBWeek_VisitorsCount"] as string;
                    oHomeDash.strCBWeek_ReceiptsAmt = ViewData["CBWeek_ReceiptsAmt"] as string;
                    oHomeDash.strCBWeek_PaymentsAmt = ViewData["CBWeek_PaymentsAmt"] as string;
                    oHomeDash.strTodaysAuditCount = ViewData["TodaysAuditCount"] as string;
                    oHomeDash.strCB_CurrUsed = ViewData["CB_CurrUsed"] as string;

                    // close connection
                    clientContext.Database.CloseConnection();
                }

            }


            ///
            oHomeDash.strChurchType = this._oLoggedCB.OrgType;
            oHomeDash.oChurchBodyId = this._oLoggedCB.Id;
            oHomeDash.strChuBodyLoggedDash = !string.IsNullOrEmpty(this._oLoggedAGO.Acronym) && !string.IsNullOrEmpty(this._oLoggedCB.Name) ?
                                            this._oLoggedAGO.Acronym + " - " + this._oLoggedCB.Name : this._oLoggedAGO.Acronym + this._oLoggedCB.Name;
            return oHomeDash;

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

        //private async Task LoadDashboardValues()
        //{ 
        //    using (var dashContext = new MSTR_DbContext())
        //    {
        //        var res = await (from dummyRes in new List<string> { "X" }
        //                   join tago in dashContext.AppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
        //                   join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //                   join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
        //                   join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
        //                   join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //                   join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //                   join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //                   join tdb in dashContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb
        //                   join tcln_a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A") on 1 equals 1 into _tcln_a
        //                   join tcln_d in (from a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
        //                                   from b in dashContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
        //                                   select a) on 1 equals 1 into _tcln_d
        //                   select new
        //                   {
        //                       cnt_tago = _tago.Count(),
        //                       cnt_tcb = _tcb.Count(),
        //                       cnt_tsr = _tsr.Count(),
        //                       cnt_tsp = _tsp.Count(),
        //                       cnt_tms = _tms.Count(),
        //                       cnt_tsubs = _tsubs.Count(),
        //                       cnt_tdb = _tdb.Count(),
        //                       cnt_ttc = _ttc.Count(),
        //                       cnt_tcln_d = _tcln_d.Count(),
        //                       cnt_tcln_a = _tcln_a.Count()
        //                   })
        //                    .ToList().ToListAsync();

        //        ///
        //        ViewBag.TotalSubsDenom = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tago : 0));  
        //        ViewBag.TotalSubsCong = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcb: 0));   
        //        ViewBag.TotalSysPriv = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsp: 0));   
        //        ViewBag.TotalSysRoles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsr: 0));   
        //        ViewBag.TotSysProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tms: 0));  
        //        ViewBag.TotSubscribers = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsubs: 0));  
        //        ViewBag.TotDbaseCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tdb: 0));    
        //        ViewBag.TodaysAuditCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_ttc: 0));   
        //        ViewBag.TotClientProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_a: 0));   
        //        ViewBag.TotClientProfiles_Admins = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_d: 0));   

        //        // Index value... main dashboard...
        //        //
        //        //oHomeDash.TotalSubsDenom = String.Format("{0:N0}", res.cnt_tago); ViewBag.TotalSubsDenom = oHomeDash.TotalSubsDenom;
        //        //oHomeDash.TotalSubsCong = String.Format("{0:N0}", res.cnt_tcb); ViewBag.TotalSubsCong = oHomeDash.TotalSubsCong;
        //        //oHomeDash.TotalSysPriv = String.Format("{0:N0}", res.cnt_tsp); ViewBag.TotalSysPriv = oHomeDash.TotalSysPriv;
        //        //oHomeDash.TotalSysRoles = String.Format("{0:N0}", res.cnt_tsr); ViewBag.TotalSysRoles = oHomeDash.TotalSysRoles;
        //        //oHomeDash.TotSysProfiles = String.Format("{0:N0}", res.cnt_tms); ViewBag.TotSysProfiles = oHomeDash.TotSysProfiles;
        //        //oHomeDash.TotSubscribers = String.Format("{0:N0}", res.cnt_tsubs); ViewBag.TotSubscribers = oHomeDash.TotSubscribers;
        //        //oHomeDash.TotDbaseCount = String.Format("{0:N0}", res.cnt_tdb); ViewBag.TotDbaseCount = oHomeDash.TotDbaseCount;
        //        //oHomeDash.TodaysAuditCount = String.Format("{0:N0}", res.cnt_ttc); ViewBag.TodaysAuditCount = oHomeDash.TodaysAuditCount;
        //        //oHomeDash.TotClientProfiles = String.Format("{0:N0}", res.cnt_tcln_a); ViewBag.TotClientProfiles = oHomeDash.TotClientProfiles;
        //        //oHomeDash.TotClientProfiles_Admins = String.Format("{0:N0}", res.cnt_tcln_d); ViewBag.TotClientProfiles_Admins = oHomeDash.TotClientProfiles_Admins;
        //    }

        //}


        private async Task LoadVendorDashboardValues()  ///string strTempConn = "")
        {
            // using (var dashContext = new MSTR_DbContext())
            // var cs = _masterContext.Database.GetDbConnection().ConnectionString;

            // var _cs = this._configuration["ConnectionStrings:DefaultConnection"]; /// _masterContext.Database.GetDbConnection().ConnectionString

            //var _cs1 = _configuration.GetConnectionString("DefaultConnection");
            //var _cs2 = this._configuration["ConnectionStrings:DefaultConnection"];

            // Server=RHEMA-SDARTEH;Database=DBRCMS_MS_PROD;User Id=sa;Password=sadmin;MultipleActiveResultSets=true
            // Server=RHEMA-SDARTEH;Database=DBRCMS_MS_PROD;User Id=sa;Password=sadmin;MultipleActiveResultSets=true

            //var _cs = strTempConn;  /// 
            //if (string.IsNullOrEmpty(_cs)) 
            //    _cs = _configuration.GetConnectionString("DefaultConnection");   /// this._configuration["ConnectionStrings:DefaultConnection"];


            var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  /// 

            using (var dashContext = new MSTR_DbContext(_cs)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
            {
                if (dashContext.Database.CanConnect() == false) dashContext.Database.OpenConnection();
                else if (dashContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) dashContext.Database.OpenConnection();

                var oAGO = dashContext.MSTRAppGlobalOwner.ToList();

                var _tm = DateTime.Now.Date;
                var res = await (from dummyRes in new List<string> { "X" }
                                 join tago in dashContext.MSTRAppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
                                 join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
                                 join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
                                 join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
                                 join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
                                 join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
                                 join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == _tm) on 1 equals 1 into _ttc
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



                // close connection
                dashContext.Database.CloseConnection();
            }










            //ViewData["TotalSubsDenom"] = String.Format("{0:N0}", 0);
            //ViewData["TotalSubsCong"] = String.Format("{0:N0}", 0);
            //ViewData["TotalSysPriv"] = String.Format("{0:N0}", 0);
            //ViewData["TotalSysRoles"] = String.Format("{0:N0}", 0);
            //ViewData["TotSysProfiles"] = String.Format("{0:N0}", 0);
            //ViewData["TotSubscribers"] = String.Format("{0:N0}", 0);
            //ViewData["TotDbaseCount"] = String.Format("{0:N0}", 0);
            //ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);
            //ViewData["TotClientProfiles"] = String.Format("{0:N0}", 0);
            //ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", 0);

            //using (var dashContext = new MSTR_DbContext())
            //{
            //    var res = await (from dummyRes in new List<string> { "X" }
            //                     join tago in dashContext.AppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
            //                     join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
            //                     join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
            //                     join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
            //                     join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
            //                     join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
            //                     join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
            //                     join tdb in dashContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb
            //                     join tcln_a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A") on 1 equals 1 into _tcln_a
            //                     join tcln_d in (from a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
            //                                     from b in dashContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
            //                                     select a) on 1 equals 1 into _tcln_d
            //                     select new
            //                     {
            //                         cnt_tago = _tago.Count(),
            //                         cnt_tcb = _tcb.Count(),
            //                         cnt_tsr = _tsr.Count(),
            //                         cnt_tsp = _tsp.Count(),
            //                         cnt_tms = _tms.Count(),
            //                         cnt_tsubs = _tsubs.Count(),
            //                         cnt_tdb = _tdb.Count(),
            //                         cnt_ttc = _ttc.Count(),
            //                         cnt_tcln_d = _tcln_d.Count(),
            //                         cnt_tcln_a = _tcln_a.Count()
            //                     })
            //                .ToList().ToListAsync();

            //    ///
            //    if (res.Count > 0)
            //    {
            //        ViewData["TotalSubsDenom"] = String.Format("{0:N0}", res[0].cnt_tago );
            //        ViewData["TotalSubsCong"] = String.Format("{0:N0}", res[0].cnt_tcb );
            //        ViewData["TotalSysPriv"] = String.Format("{0:N0}", res[0].cnt_tsp );
            //        ViewData["TotalSysRoles"] = String.Format("{0:N0}", res[0].cnt_tsr );
            //        ViewData["TotSysProfiles"] = String.Format("{0:N0}", res[0].cnt_tms );
            //        ViewData["TotSubscribers"] = String.Format("{0:N0}", res[0].cnt_tsubs );
            //        ViewData["TotDbaseCount"] = String.Format("{0:N0}", res[0].cnt_tdb );
            //        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", res[0].cnt_ttc );
            //        ViewData["TotClientProfiles"] = String.Format("{0:N0}", res[0].cnt_tcln_a );
            //        ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}",  res[0].cnt_tcln_d );
            //    }

            //    else
            //    {
            //        ViewData["TotalSubsDenom"] = String.Format("{0:N0}", 0);
            //        ViewData["TotalSubsCong"] = String.Format("{0:N0}", 0);
            //        ViewData["TotalSysPriv"] = String.Format("{0:N0}", 0);
            //        ViewData["TotalSysRoles"] = String.Format("{0:N0}", 0);
            //        ViewData["TotSysProfiles"] = String.Format("{0:N0}", 0);
            //        ViewData["TotSubscribers"] = String.Format("{0:N0}", 0);
            //        ViewData["TotDbaseCount"] = String.Format("{0:N0}", 0);
            //        ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);
            //        ViewData["TotClientProfiles"] = String.Format("{0:N0}", 0);
            //        ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", 0);
            //    } 
            //}
        }    
        
        //private async Task _LoadClientDashboardValues(string clientDBConnString, UserProfile oLoggedUser, string strTempConn = "")
        //{
        //    // using (var dashContext = new ChurchModelContext(clientDBConnString))

        //    var _cs = strTempConn;
        //    if (string.IsNullOrEmpty(_cs)) 
        //        _cs = this._configuration["ConnectionStrings:DefaultConnection"];

        //    using (var clientContext = new ChurchModelContext(_cs)) // ("Server=RHEMA-SDARTEH;Database=DBRCMS_MS_TEST;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true") ) // AppUtilties.GetNewDBContext_MS(_context, "DBRCMS_CL_TEST"))  // MSTR_DbContext()) //
        //    {
        //        if (clientContext.Database.CanConnect() == false) 
        //            clientContext.Database.OpenConnection();
        //        else if (clientContext.Database.GetDbConnection().State != System.Data.ConnectionState.Open) 
        //            clientContext.Database.OpenConnection();

        //        ////get Currency
        //        //var curr = clientContext.Currency.Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == oLoggedUser.ChurchBodyId && c.IsBaseCurrency == true).FirstOrDefault();
        //        //ViewData["CB_CurrUsed"] = curr != null ? curr.Acronym : ""; // "GHS"

        //        // this attr is used by most of the models... save in memory [class var]
        //        oCTRYDefault = _context.CountryCustom.AsNoTracking().Include(t => t.Country).Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.ChurchBodyId == oLoggedUser.ChurchBodyId && c.IsDefaultCountry == true).FirstOrDefault();

        //        ViewData["CB_CurrUsed"] = oCTRYDefault.Country != null ? oCTRYDefault.Country.Curr3LISOSymbol : ""; // curr != null ? curr.Acronym : ""; // "GHS"


        //        var clientAGO = clientContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
        //        var clientCB = clientContext.ChurchBody.Where(c => c.MSTR_AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.MSTR_ChurchBodyId==oLoggedUser.ChurchBodyId && c.Status == "A").FirstOrDefault();
        //        ///
        //        var qrySuccess = false;
        //        if (clientAGO != null && clientCB != null)
        //        {
        //            var res = await (from dummyRes in new List<string> { "X" }
        //                             join tcb_sb in clientContext.ChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN") &&
        //                                                 c.AppGlobalOwnerId== clientAGO.Id && c.ParentChurchBodyId == clientCB.Id) on 1 equals 1 into _tcb_sb
        //                             // join tcb in clientContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
        //                             // join tsr in clientContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr

        //                             join tcm in _context.CBMemberRollBal.AsNoTracking().Where(c => c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm

        //                             //join tcm in clientContext.ChurchMember.Where(c => c.Status == "A" &&
        //                             //                    c.AppGlobalOwnerId == clientAGO.Id && c.ChurchBodyId == clientCB.Id) on 1 equals 1 into _tcm
        //                             // join tms in clientContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
        //                             // join tsubs in clientContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
        //                             // join ttc in clientContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
        //                             // join tdb in clientContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb

        //                             select new
        //                             {
        //                                 cnt_tcb_sb = _tcb_sb.Count(), 
        //                                 cnt_tcm = _tcm.Count() > 0 ? _tcm.ToList()[0].TotRoll : 0,
        //                                 ///
        //                                 //cnt_tms = _tms.Count(),
        //                                 //cnt_tsubs = _tsubs.Count(),
        //                                 //cnt_tdb = _tdb.Count(),
        //                                // cnt_ttc = _ttc.Count(),
        //                                 //cnt_tcln_d = _tcln_d.Count(),
        //                                 //cnt_tcln_a = _tcln_a.Count()
        //                             })
        //                    .ToList().ToListAsync();

        //            ///
        //            if (res.Count() > 0)
        //            {
        //                qrySuccess = true;
        //                ViewData["CB_SubCongCount"] = String.Format("{0:N0}", res[0].cnt_tcb_sb);
        //                ViewData["CB_MemListCount"] = String.Format("{0:N0}", res[0].cnt_tcm); 

        //                ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 0); // res[0].cnt_tsubs);
        //                ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tdb);
        //                ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_a);
        //                ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d);
        //                ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0); //res[0].cnt_tcln_d); 
        //            }

        //            var resAudits = _masterContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date);
        //           // var cnt_ttc = resAudits.Count();
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

        //        if(!qrySuccess)
        //        {
        //            ViewData["CB_SubCongCount"] = String.Format("{0:N0}", 0);
        //            ViewData["CB_MemListCount"] = String.Format("{0:N0}", 0);
        //            ViewData["CBWeek_NewMemListCount"] = String.Format("{0:N0}", 0);
        //            ViewData["CBWeek_NewConvertsCount"] = String.Format("{0:N0}", 0);
        //            ViewData["CBWeek_VisitorsCount"] = String.Format("{0:N0}", 0);
        //            ViewData["CBWeek_ReceiptsAmt"] = String.Format("{0:N0}", 0);
        //            ViewData["CBWeek_PaymentsAmt"] = String.Format("{0:N0}", 0);
        //            ///
        //            ViewData["Today_AuditCount"] = String.Format("{0:N0}", 0);
        //        }

        //        // close connection
        //        clientContext.Database.CloseConnection();
        //    } 
        //}

        private string GetDefaultCountryInfo()   // GHA--Ghana--GHC--GHS
        {
            try
            {

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
                return "";
            }
        }

      

        public async Task<IActionResult> Index()
        {
            // init the client DB   
           // LoadClientDashboardValues(this._clientDBConnString, this._oLoggedUser);

         //   SetUserLogged();

            //if (!isCurrValid) { ViewData["strUserLoginFailMess"] = "Client user profile validation unsuccessful."; return RedirectToAction("LoginUserAcc", "UserLogin"); }
            //else
            //{

            if (oUserLogIn_Priv.UserProfile == null)
            { 
                ViewData["strUserLoginFailMess"] = "Client user profile not found. Please try again or contact System Admin";
                return RedirectToAction("LoginUserAcc", "UserLogin"); 
            }


            // check permission 
            var _oUserPrivilegeCol = oUserLogIn_Priv;
            var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
            TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();

                //
                //if (!this.userAuthorized) return View(new HomeDashboardVM()); //retain view    
                //if (oUserLogIn_Priv == null) return View(new HomeDashboardVM());

                //  if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) return View(new HomeDashboardVM());
                // var oLoggedUser = oUserLogIn_Priv.UserProfile;

                //var oLoggedRole = oUserLogIn_Priv.UserRole;
                ////
                //var oUserId_Logged = oLoggedUser.Id;
                //var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
                //int? oAppGloOwnId_Logged = null; int? oChuBodyId_Logged = null;
                //if (oChuBody_Logged != null)
                //{
                //    oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
                //    oChuBodyId_Logged = oChuBody_Logged.Id;
                //}


                //// Get the client database details.... db connection string
                //var _clientDBConnString = "";
                //var conn = new SqlConnectionStringBuilder(_masterContext.Database.GetDbConnection().ConnectionString);
                ////  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
                ////conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

                //var oClientConfig = _masterContext.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                //if (oClientConfig == null) { ViewData["strUserLoginFailMess"] = "Client database details not found. Please try again or contact System Admin"; return RedirectToAction("LoginUserAcc", "UserLogin"); }
                /////
                //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName; conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                //_clientDBConnString = conn.ConnectionString;

                //// test the NEW DB conn
                //var _context = new ChurchModelContext(_clientDBConnString = conn.ConnectionString);
                //if (!_context.Database.CanConnect()) { ViewData["strUserLoginFailMess"] = "Failed to connect client database. Please try again or contact System Admin"; return RedirectToAction("LoginUserAcc", "UserLogin"); }  // give appropriate user prompts

                //// connection SUCESS! initialize main context... this._context  -->>> to inject subsequent instances.  REM: not using the Setup otopn now. Setup only connects the Master DB
                //// this._context = testConn;



                //SetUserLogged();
                //if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
                //else
                //{
                //    // check permission 
                //    var _oUserPrivilegeCol = oUserLogIn_Priv;
                //    var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                //    TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                //    //
                //    // check permission 
                //    if (!this.userAuthorized) return View(new HomeDashboardVM()); //retain view    
                //    if (oUserLogIn_Priv == null) return View(new HomeDashboardVM());
                //    if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner == null || oUserLogIn_Priv.ChurchBody == null) return View(new HomeDashboardVM()); 
                //    var oLoggedUser = oUserLogIn_Priv.UserProfile;
                //    var oChuBodyLogOn = oUserLogIn_Priv.ChurchBody;
                //    var oAppGloOwn = oUserLogIn_Priv.AppGlobalOwner;


                ///                                
                //Actually... loggedUser has already been authenticated @Login -->> exists, subscribed, active etc.
                ///
                var oHomeDash = new HomeDashboardVM();
            //var oLoggedRole = oUserLogIn_Priv.UserRoles;
            //var oLoggedGroup = oUserLogIn_Priv.UserGroups;
            //var oLoggedPerm = oUserLogIn_Priv.UserPermissions;

            //var oLoggedUserSessionPermList = oUserLogIn_Priv.UserSessionPermList;
            //var oLoggedUser = oUserLogIn_Priv.UserProfile;
            //var oLoggedCB = oUserLogIn_Priv.ChurchBody;
            //var oAppGloOwn = _masterContext.MSTRAppGlobalOwner.Find(oLoggedUser.AppGlobalOwnerId);
            //var oChuBodyLogOn = _masterContext.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
            //                            .Where(c => c.AppGlobalOwnerId == oLoggedUser.AppGlobalOwnerId && c.Id == oLoggedUser.ChurchBodyId).FirstOrDefault();


            // var _clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);

            TempData["_strCLConnStr"] = ""; TempData.Keep();
            ///
            var _strAppName = this._configuration["AppSettingVals:AppName"];
            _strAppName = !string.IsNullOrEmpty(_strAppName) ? _strAppName : "RhemaCMS";

            ViewData["strAppName"] = _strAppName;   ///  "RhemaCMS";

            var strAppLogo = this._configuration["AppSettingVals:AppLogoFilename"];
            strAppLogo = !string.IsNullOrEmpty(strAppLogo) ? strAppLogo : "df_rhema.jpg";
            ViewData["strAppLogo_Filename"] = strAppLogo;

         //   ViewData["strAppName"] = "RhemaCMS";
              //  ViewData["strAppLogo_Filename"] = "df_rhema.jpg"; // "~/img_db/df_rhema.jpg"; // oAppGloOwn?.ChurchLogo;

                ViewData["strAppNameMod"] = "Church Dashboard";
                ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(this._oLoggedUser.UserDesc) ? this._oLoggedUser.UserDesc : "[Current user]";
                ///
                ViewData["oAppGloOwnId_Logged_MSTR"] = this._oLoggedUser.AppGlobalOwnerId;
                ViewData["oChurchBodyId_Logged_MSTR"] = this._oLoggedUser.ChurchBodyId;
                ViewData["oAppGloOwnId_Logged"] = this._oLoggedAGO.Id;
                ViewData["oChurchBodyId_Logged"] = this._oLoggedCB.Id;
                ViewData["oCBOrgType_Logged"] = this._oLoggedCB.OrgType;  // CR, CH, CN ...but subscriber may come from other units like Church Office or Church Group HQ [CR]
                ///
                ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
                ViewData["bl_IsModAccessVAA0"] = oUserLogIn_Priv.IsModAccessVAA0;
                ViewData["bl_IsModAccessVAA4"] = oUserLogIn_Priv.IsModAccessVAA4;
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

                // ViewData["strAppCurrUser_ChRole"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRolesDesc);  //oUserLogIn_Priv.strAssignedRole; // oLoggedRole != null ? oLoggedRole.RoleDesc : ""; // "System Adminitrator";
                //// List<string> arrRoles = new List<string>();  foreach (var r in oUserLogIn_Priv.UserRoles) arrRoles.Add(r.RoleName); //String.Join(", ", oLoggedRole)               
                // ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);  // oLoggedRole != null ? oLoggedRole.RoleName : "" ; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
                // ViewData["strAppCurrUser_ChGrp"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc); // oUserLogIn_Priv.strAssignedGroup; //oLoggedGroup != null ? oLoggedGroup.GroupDesc : ""; // "System Adminitrator";
                // ViewData["strAppCurrUser_GrpCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames); // oLoggedGroup != null ? oLoggedGroup.GroupName : ""; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST

                ViewData["strAppCurrUserPhoto_Filename"] = this._oLoggedUser.UserPhoto;
                // ViewData["strAppLogo_Filename"] = this._oLoggedUser.UserPhoto;

                ///
                ViewData["strClientLogo_Filename"] = this._oLoggedAGO?.ChurchLogo;
                // ViewData["strAppLogo_Filename"] = "~/frontend/dist/img/rhema_logo.png"; // oAppGloOwn?.ChurchLogo;
                

                ViewData["strClientChurchName"] = this._oLoggedAGO.OwnerName;
                ViewData["strClientBranchName"] = this._oLoggedCB.Name;
                ViewData["strClientChurchLevel"] = !string.IsNullOrEmpty(this._oLoggedCB.ChurchLevel?.CustomName) ? this._oLoggedCB.ChurchLevel?.CustomName : this._oLoggedCB.ChurchLevel?.Name;  // Assembly, Presbytery etc
                ViewData["strClientBranchParentName"] = this._oLoggedCB.ParentChurchBody != null ? this._oLoggedCB.ParentChurchBody.Name : "";

           

            var _clientCon = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId); ///  GetClientDBContext(); // (this._oLoggedUser);
                if (_clientCon == null)
                {
                    ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
                    // return RedirectToAction("LoginUserAcc", "UserLogin"); 
                    ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
                    return View(oHomeDash);
                }


            //var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];   // , _context, _connstr

            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");

             

            //successfull login... audit!
            var tm = DateTime.Now;

                //this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
                //                "L", null, null, "Logged in successfully into RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));

                // record... @vendor...
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
                                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id) );

                // record ... @client
                _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
                                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id) );


           
            ///
            oHomeDash = await LoadClientDashboardValues(oHomeDash, this._oLoggedUser );

            //var _cs = strTempConn;
            //if (string.IsNullOrEmpty(_cs))
            //    _cs = this._configuration["ConnectionStrings:DefaultConnection"];




            //int? oAppGloOwnId_Logged = null;
            //string strChuBodyDenom_Logged = "";
            //int? oChuBodyId_Logged = null;
            //int? oUserId_Logged = null;
            //string strChuBodyType_Logged = "";                
            //string strChuBodyCong_Logged = "";
            //string strUserDesc_Logged = "";
            //string strUserPhoto_Logged = ""; 

            //if (oChuBodyLogOn != null)
            //{
            //    oAppGloOwnId_Logged = oChuBodyLogOn.AppGlobalOwnerId;
            //    strChuBodyDenom_Logged =  oAppGloOwn?.OwnerName;// "The Church of Pentecost";
            //    oChuBodyId_Logged = oChuBodyLogOn.Id;
            //    strChuBodyType_Logged = oChuBodyLogOn.OrgType.ToUpper();  //ChurchType.ToUpper();
            //    strChuBodyCong_Logged =  oChuBodyLogOn.Name; //  "La Assembly, Accra";                 
            //    oUserId_Logged = oLoggedUser.Id;
            //    strUserDesc_Logged = oLoggedUser.UserDesc; // "Apostle Sam Darteh";
            //    strUserPhoto_Logged = oLoggedUser.UserPhoto;//"2020_dev_sam.jpg"; 

            //    //if (oCurrChuBodyId == null) { oCurrChuBodyId = oChuBodyLogOn.Id; }
            //    //if (oAppGloOwnId == null) { oAppGloOwnId = oChuBodyLogOn.AppGlobalOwnerId; }
            //    //else if (oCurrChuBodyId != oCurrChuBodyLogOn.Id) oCurrChuBodyId = oCurrChuBodyLogOn.Id;  //reset to logon...
            //    //
            //    // oAppGloOwnId = oCurrChuBodyLogOn.AppGlobalOwnerId;
            //}


            //int? oCurrChuMemberId_LogOn = null;
            //ChurchMember oCurrChuMember_LogOn = null;

            //var currChurchMemberLogged = _context.ChurchMember.Where(c => c.ChurchBodyId == oChuBodyId_Logged && c.Id == oLoggedUser.ChurchMemberId).FirstOrDefault();
            //if (currChurchMemberLogged != null) //return View(oCurrMdl);
            //{
            //    oCurrChuMemberId_LogOn = currChurchMemberLogged.Id;
            //    oCurrChuMember_LogOn = currChurchMemberLogged;
            //}



            //oHomeDash.oAppGlolOwnId = oAppGloOwnId_Logged; ViewBag.oAppGloOwnId_Logged = oHomeDash.oAppGloOwnId_Logged;
            //oHomeDash.oChurchBodyId = oChuBodyId_Logged; ViewBag.oChuBodyId_Logged = oHomeDash.oChurchBodyId;
            ////
            //oHomeDash.oUserId_Logged = oUserId_Logged; ViewBag.oUserId_Logged = oHomeDash.oUserId_Logged;
            //oHomeDash.oChurchBody_Logged = oChuBodyLogOn; ViewBag.oChurchBody_Logged = oHomeDash.oChurchBody_Logged;
            //oHomeDash.oAppGloOwnId_Logged = oAppGloOwnId_Logged; ViewBag.oAppGloOwnId_Logged = oHomeDash.oAppGloOwnId_Logged;
            //oHomeDash.oMemberId_Logged = oCurrChuMemberId_LogOn; ViewBag.oMemberId_Logged = oHomeDash.oMemberId_Logged;

            ////
            ////
            //oHomeDash.strChurchLevelDown = "Assemblies";
            //oHomeDash.strAppName = "RhemaCMS"; ViewBag.strAppName = oHomeDash.strAppName;
            //oHomeDash.strAppCurrUser = strUserDesc_Logged; ViewBag.strAppCurrUser = oHomeDash.strAppCurrUser;
            //oHomeDash.strChurchType = strChuBodyType_Logged; ViewBag.strChurchType = oHomeDash.strChurchType;//"CH"
            //oHomeDash.strChuBodyDenomLogged =  strChuBodyDenom_Logged; ViewBag.strChuBodyDenomLogged = oHomeDash.strChuBodyDenomLogged;//"Rhema Global Church"
            //oHomeDash.strChuBodyLogged =  strChuBodyCong_Logged; ViewBag.strChuBodyLogged = oHomeDash.strChuBodyLogged;//"Rhema Comm Chapel"

            ////           
            //ViewBag.strAppCurrUser_ChRole = "Pastor-in-Charge";
            //ViewBag.strAppCurrUser_RoleCateg = "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
            //ViewBag.strAppCurrUserPhoto_Filename = strUserPhoto_Logged; //   "2020_dev_sam.jpg"; //
            //ViewBag.strAppCurrChu_LogoFilename = "14dc86a7-81ae-462c-b73e-4581bd4ee2b2_church-of-pentecost.png";
            //ViewBag.strUserSessionDura = "Logged: 1 hour ago";


            ////String.Format(1234 % 1 == 0 ? "{0:N0}" : "{0:N2}", 1234);
            //var curr = _context.Currency.Where(c => c.AppGlobalOwnerId == oAppGloOwnId_Logged && c.ChurchBodyId == oChuBodyId_Logged && c.IsBaseCurrency == true).FirstOrDefault(); 
            //oHomeDash.strCurrUsed = curr != null ? curr.Acronym : ""; // "GHS";
            //oHomeDash.SupCongCount = String.Format("{0:N0}", 25);
            //oHomeDash.MemListCount = String.Format("{0:N0}", 4208); ViewBag.MemListCount = oHomeDash.MemListCount;
            //oHomeDash.NewMemListCount = String.Format("{0:N0}", 17); ViewBag.NewMemListCount = oHomeDash.NewMemListCount;
            //oHomeDash.NewConvertsCount = String.Format("{0:N0}", 150); ViewBag.NewConvertsCount = oHomeDash.NewConvertsCount;
            //oHomeDash.VisitorsCount = String.Format("{0:N0}", 9); ViewBag.VisitorsCount = oHomeDash.VisitorsCount;
            //oHomeDash.ReceiptsAmt = String.Format("{0:N2}", 1700);
            //oHomeDash.PaymentsAmt = String.Format("{0:N2}", 105.491);


            // var ls = _context.ChurchBody.ToList();
            //var _oUserPrivilegeCol = oUserLogIn_Priv;
            //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
            //TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();


            return View(oHomeDash);

           // }
            
        }

        public IActionResult Index_MR()
        {

             if (this._context == null)
            { 
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId); /// GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }

            if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            { RedirectToAction("LoginUserAcc", "UserLogin"); }


            var oHomeDash = new HomeDashboardVM();
            ///

            //var _clientCon = GetClientDBContext(this._oLoggedUser);
            //if (_clientCon == null)
            //{
            //    ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
            //    // return RedirectToAction("LoginUserAcc", "UserLogin"); 
            //    ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
            //    return View(oHomeDash);
            //}


            //var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];   // , _context, _connstr

            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");


            //_ = this.LoadClientDashboardValues(_connstr_CL);// this._clientDBConnString
             
             
            ///// query  .... 
            //// var strClientConn = _clientCon.Database.GetDbConnection().ConnectionString;

            ////successfull login... audit!
            //var tm = DateTime.Now;
            ////this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
            ////                "L", null, null, "Logged in successfully into RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));

            //// record... @vendor...
            //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //    );

            //// record ... @client
            //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //               );


            //// refreshValues...
            //// oHomeDash = await LoadClientDashboardValues(oHomeDash, this._clientDBConnString, this._oLoggedUser);


            return View("Index_MR", oHomeDash);

            // }

        }
         public IActionResult Index_CL()
        {

             if (this._context == null)
            { 
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);///  GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }

            if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            { RedirectToAction("LoginUserAcc", "UserLogin"); }


            var oHomeDash = new HomeDashboardVM();
            ///

            //var _clientCon = GetClientDBContext(this._oLoggedUser);
            //if (_clientCon == null)
            //{
            //    ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
            //    // return RedirectToAction("LoginUserAcc", "UserLogin"); 
            //    ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
            //    return View(oHomeDash);
            //}

            //var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];   // , _context, _connstr

            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");

            ////_ = this.LoadClientDashboardValues(_connstr_CL);
             


            ///// query  .... 
            //// var strClientConn = _clientCon.Database.GetDbConnection().ConnectionString;

            ////successfull login... audit!
            //var tm = DateTime.Now;
            ////this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
            ////                "L", null, null, "Logged in successfully into RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));

            //// record... @vendor...
            //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //    );

            //// record ... @client
            //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //              );


            //// refreshValues...
            //// oHomeDash = await LoadClientDashboardValues(oHomeDash, this._clientDBConnString, this._oLoggedUser);


            return View("Index_CL", oHomeDash);

            // }

        }
         public IActionResult Index_CA()
        {

             if (this._context == null)
            {
                
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);  /// GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }

            if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            { RedirectToAction("LoginUserAcc", "UserLogin"); }


            var oHomeDash = new HomeDashboardVM();
            ///

            //var _clientCon = GetClientDBContext(this._oLoggedUser);
            //if (_clientCon == null)
            //{
            //    ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
            //    // return RedirectToAction("LoginUserAcc", "UserLogin"); 
            //    ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
            //    return View(oHomeDash);
            //}


            //var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];   // , _context, _connstr

            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");


            //_ = this.LoadClientDashboardValues();
              

            ///// query  .... 
            //// var strClientConn = _clientCon.Database.GetDbConnection().ConnectionString;

            ////successfull login... audit!
            //var tm = DateTime.Now;
            ////this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
            ////                "L", null, null, "Logged in successfully into RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));

            //// record... @vendor...
            //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //   );

            //// record ... @client
            //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //   );


            //// refreshValues...
            //// oHomeDash = await LoadClientDashboardValues(oHomeDash, this._clientDBConnString, this._oLoggedUser);


            return View("Index_CA", oHomeDash);

            // }

        }
         public IActionResult Index_CF()
        {

             if (this._context == null)
            {
                
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId);  /// GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }

            if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            { RedirectToAction("LoginUserAcc", "UserLogin"); }


            var oHomeDash = new HomeDashboardVM();
            ///

            //var _clientCon = GetClientDBContext(this._oLoggedUser);
            //if (_clientCon == null)
            //{
            //    ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
            //    // return RedirectToAction("LoginUserAcc", "UserLogin"); 
            //    ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
            //    return View(oHomeDash);
            //}

            //var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];   // , _context, _connstr

            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");

           
            //_ = this.LoadClientDashboardValues();
             

            ///// query  .... 
            //// var strClientConn = _clientCon.Database.GetDbConnection().ConnectionString;

            ////successfull login... audit!
            //var tm = DateTime.Now;
            ////this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
            ////                "L", null, null, "Logged in successfully into RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));

            //// record... @vendor...
            //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //               );

            //// record ... @client
            //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //               );


            //// refreshValues...
            //// oHomeDash = await LoadClientDashboardValues(oHomeDash, this._clientDBConnString, this._oLoggedUser);


            return View("Index_CF", oHomeDash);

            // }

        }
         public IActionResult Index_RA()
        {

             if (this._context == null)
            {
               
                this._context = AppUtilties.GetNewDBCtxConn_CL(_masterContext, _configuration, this._oLoggedUser.AppGlobalOwnerId); /// GetClientDBContext();
                if (this._context == null)
                {
                    RedirectToAction("LoginUserAcc", "UserLogin");

                    // should not get here... Response.StatusCode = 500; 
                    return View("_ErrorPage");
                }
            }

            if (this._oLoggedAGO_MSTR == null || this._oLoggedCB_MSTR == null || this._oLoggedAGO == null || this._oLoggedCB == null)
            { RedirectToAction("LoginUserAcc", "UserLogin"); }


            var oHomeDash = new HomeDashboardVM();
            ///

            //var _clientCon = GetClientDBContext(this._oLoggedUser);
            //if (_clientCon == null)
            //{
            //    ViewData["strUserLoginFailMess"] = "Client database connection unsuccessful. Please try again or contact System Admin";
            //    // return RedirectToAction("LoginUserAcc", "UserLogin"); 
            //    ModelState.AddModelError("", "Client database connection unsuccessful. Please try again or contact System Admin");
            //    return View(oHomeDash);
            //}

            //var _connstr = this._configuration["ConnectionStrings:DefaultConnection"];   // , _context, _connstr

            //// refreshValues...
            //var _connstr_CL = this.GetCL_DBConnString();
            //if (string.IsNullOrEmpty(_connstr_CL)) RedirectToAction("LoginUserAcc", "UserLogin");

            //_ = this.LoadClientDashboardValues();
              

            ///// query  .... 
            //// var strClientConn = _clientCon.Database.GetDbConnection().ConnectionString;

            ////successfull login... audit!
            //var tm = DateTime.Now;
            ////this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
            ////                "L", null, null, "Logged in successfully into RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));

            //// record... @vendor...
            //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //            );

            //// record ... @client
            //_ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, this._oLoggedUser.AppGlobalOwnerId, this._oLoggedUser.ChurchBodyId, "N",
            //                 "RCMS-Client: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
            //              );


            //// refreshValues...
            //// oHomeDash = await LoadClientDashboardValues(oHomeDash, this._clientDBConnString, this._oLoggedUser);


            return View("Index_RA", oHomeDash);

            // }

        }

         


        public async Task<IActionResult> Index_sa()
        {
            //SetUserLogged();
            //if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");
            //else
            //{

                if (oUserLogIn_Priv.UserProfile == null)
                {
                    ViewData["strUserLoginFailMess"] = "Client user profile not found. Please try again or contact System Admin";
                    return RedirectToAction("LoginUserAcc", "UserLogin");
                }


                // check permission 
                var _oUserPrivilegeCol = oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();


                //// check permission 
                //var _oUserPrivilegeCol = oUserLogIn_Priv;
                //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                //TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();

                //
                //if (!this.userAuthorized) return View(new HomeDashboardVM()); //retain view    
                //if (oUserLogIn_Priv == null) return View(new HomeDashboardVM());
                //if (oUserLogIn_Priv.UserProfile == null || oUserLogIn_Priv.AppGlobalOwner != null || oUserLogIn_Priv.ChurchBody != null) return View(new HomeDashboardVM());
                //var oLoggedUser = oUserLogIn_Priv.UserProfile;

              //  var oLoggedRole = String.Join(", ", oUserLogIn_Priv.arrAssignedRolesDesc);  //oUserLogIn_Priv.UserRole;
              //  var oLoggedGroup = oUserLogIn_Priv.UserGroup;
               
               // var oUserId_Logged = oLoggedUser.Id;
               // var oChuBody_Logged = oUserLogIn_Priv.ChurchBody;
               //// int? oAppGloOwnId_Logged = null; int? oChuBodyId_Logged = null;
               // if (oChuBody_Logged != null)
               // {
               //     oAppGloOwnId_Logged = oChuBody_Logged.AppGlobalOwnerId;
               //     oChuBodyId_Logged = oChuBody_Logged.Id;
               // }
                 
                
                //  successfull login... audit!
                var tm = DateTime.Now;

                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                 "RCMS-Admin: Home-Dashboard", AppUtilties.GetRawTarget(HttpContext.Request), "Launched into Admin Palette Home-Dashboard", tm, this._oLoggedUser.Id, tm, tm, this._oLoggedUser.Id, this._oLoggedUser.Id)
                    );


                var oHomeDashAdmin = new HomeDashboardVM();
            //  oHomeDashAdmin.strChurchLevelDown = "Assemblies"; 
            ///
            var _strAppName = this._configuration["AppSettingVals:AppName"];
            _strAppName = !string.IsNullOrEmpty(_strAppName) ? _strAppName : "RhemaCMS";

                ViewData["strAppName"] = _strAppName;   ///  "RhemaCMS";
                ViewData["strAppNameMod"] = "Admin Palette";
                ViewData["strAppCurrUser"] = !string.IsNullOrEmpty(this._oLoggedUser.UserDesc) ? this._oLoggedUser.UserDesc : "[Current user]";
                ///
                ViewData["oAppGloOwnId_Logged"] = (int?)null;
                ViewData["oChuBodyId_Logged"] = (int?)null;

                // ViewData["oCBOrgType_Logged"] = oLoggedCB.OrgType;  // CH, CN but subscriber may come from oth

                ViewData["strModCodes"] = String.Join(", ", oUserLogIn_Priv.arrAssignedModCodes);
                ViewData["bl_IsModAccessVAA0"] = oUserLogIn_Priv.IsModAccessVAA0;
                ViewData["bl_IsModAccessVAA4"] = oUserLogIn_Priv.IsModAccessVAA4;
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

                //ViewData["strAppCurrUser_ChRole"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRolesDesc);  //oUserLogIn_Priv.strAssignedRole; // oLoggedRole != null ? oLoggedRole.RoleDesc : ""; // "System Adminitrator";
                //                                                                                             // List<string> arrRoles = new List<string>();  foreach (var r in oUserLogIn_Priv.UserRoles) arrRoles.Add(r.RoleName); //String.Join(", ", oLoggedRole)               
                //ViewData["strAppCurrUser_RoleCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedRoleCodes);  // oLoggedRole != null ? oLoggedRole.RoleName : "" ; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
                //ViewData["strAppCurrUser_ChGrp"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupsDesc); // oUserLogIn_Priv.strAssignedGroup; //oLoggedGroup != null ? oLoggedGroup.GroupDesc : ""; // "System Adminitrator";
                //ViewData["strAppCurrUser_GrpCateg"] = String.Join(", ", oUserLogIn_Priv.arrAssignedGroupNames); // oLoggedGroup != null ? oLoggedGroup.GroupName : ""; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST


                ViewData["strAppCurrUserPhoto_Filename"] = this._oLoggedUser.UserPhoto;

                // ViewData["strAppLogo_Filename"] = oLoggedUser.UserPhoto;
              //  ViewData["strAppLogo_Filename"] = "df_rhema.jpg"; //"~/img_db/df_rhema.jpg"; //"~/frontend/dist/img/rhema_logo.png"; // oAppGloOwn?.ChurchLogo;

            var strAppLogo = this._configuration["AppSettingVals:AppLogoFilename"];
            strAppLogo = !string.IsNullOrEmpty(strAppLogo) ? strAppLogo : "df_rhema.jpg";
            ViewData["strAppLogo_Filename"] = strAppLogo;

            ///
            //ViewData["strAppCurrChu_LogoFilename"] = oLoggedUser.UserPhoto;
            //ViewData["strChuBodyDenomLogged"] = oLoggedUser.UserPhoto;
            //ViewData["strChuBodyLogged"] = oLoggedUser.UserPhoto;
            ///

            //oHomeDashAdmin.strAppName = "RhemaCMS"; ViewBag.strAppName = oHomeDashAdmin.strAppName;
            //oHomeDashAdmin.strAppNameMod = "Admin Palette"; ViewBag.strAppNameMod = oHomeDashAdmin.strAppNameMod;
            //oHomeDashAdmin.strAppCurrUser = oLoggedUser.UserDesc; ViewBag.strAppCurrUser = oHomeDashAdmin.strAppCurrUser;  // "Dan Abrokwa"
            //                                                                                                     //oHomeDashAdmin.strChurchType = "CH"; ViewBag.strChurchType = oHomeDashAdmin.strChurchType;
            //oHomeDashAdmin.strChuBodyDenomLogged = "Rhema Global Church"; ViewBag.strChuBodyDenomLogged = oHomeDashAdmin.strChuBodyDenomLogged;
            //oHomeDashAdmin.strChuBodyLogged = "Rhema Comm Chapel"; ViewBag.strChuBodyLogged = oHomeDashAdmin.strChuBodyLogged;

            //           
            //ViewBag.oAppGloOwnId_Logged = oAppGloOwnId_Logged;
            //ViewBag.oChuBodyId_Logged = oChuBodyId_Logged;
            //ViewBag.strAppCurrUser_ChRole = oLoggedRole.RoleDesc; // "System Adminitrator";
            //ViewBag.strAppCurrUser_RoleCateg = oLoggedRole.RoleName; // "SUP_ADMN";  // CH_ADMN | CF_ADMN | CH_RGTR | CF_RGTR | CH_ACCT | CF_ACCT | CH_CUST | CH_CUST
            //ViewBag.strAppCurrUserPhoto_Filename = oLoggedUser.UserPhoto; // "2020_dev_sam.jpg";
            // ViewBag.strAppCurrChu_LogoFilename = "14dc86a7-81ae-462c-b73e-4581bd4ee2b2_church-of-pentecost.png";
            // ViewBag.strUserSessionDura = "Logged: 10 minutes ago";



            // refreshValues...
            await LoadVendorDashboardValues();


                //ViewData["TotalSubsDenom"] = String.Format("{0:N0}", 0);
                //ViewData["TotalSubsCong"] = String.Format("{0:N0}", 0);
                //ViewData["TotalSysPriv"] = String.Format("{0:N0}", 0);
                //ViewData["TotalSysRoles"] = String.Format("{0:N0}", 0);
                //ViewData["TotSysProfiles"] = String.Format("{0:N0}", 0);
                //ViewData["TotSubscribers"] = String.Format("{0:N0}", 0);
                //ViewData["TotDbaseCount"] = String.Format("{0:N0}", 0);
                //ViewData["TodaysAuditCount"] = String.Format("{0:N0}", 0);
                //ViewData["TotClientProfiles"] = String.Format("{0:N0}", 0);
                //ViewData["TotClientProfiles_Admins"] = String.Format("{0:N0}", 0);


                //using (var dashContext = new MSTR_DbContext())
                //{
                //    var res = await (from dummyRes in new List<string> { "X" }
                //                     join tago in dashContext.AppGlobalOwner.Where(c => c.Status == "A") on 1 equals 1 into _tago
                //                     join tcb in dashContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")) on 1 equals 1 into _tcb
                //                     join tsr in dashContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null) on 1 equals 1 into _tsr
                //                     join tsp in dashContext.UserPermission.Where(c => c.PermStatus == "A") on 1 equals 1 into _tsp
                //                     join tms in dashContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A") on 1 equals 1 into _tms
                //                     join tsubs in dashContext.AppSubscription.Where(c => c.Slastatus == "A") on 1 equals 1 into _tsubs
                //                     join ttc in dashContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date) on 1 equals 1 into _ttc
                //                     join tdb in dashContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct() on 1 equals 1 into _tdb
                //                     join tcln_a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A") on 1 equals 1 into _tcln_a
                //                     join tcln_d in (from a in dashContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
                //                                           from b in dashContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
                //                                           select a) on 1 equals 1 into _tcln_d
                //                     select new
                //                     {
                //                         cnt_tago = _tago.Count(),
                //                         cnt_tcb = _tcb.Count(),
                //                         cnt_tsr = _tsr.Count(),
                //                         cnt_tsp = _tsp.Count(),
                //                         cnt_tms = _tms.Count(),
                //                         cnt_tsubs = _tsubs.Count(),
                //                         cnt_tdb = _tdb.Count(),
                //                         cnt_ttc = _ttc.Count(),
                //                         cnt_tcln_d = _tcln_d.Count(),
                //                         cnt_tcln_a = _tcln_a.Count()
                //                     })
                //                .ToList().ToListAsync();

                //    // Index value... main dashboard included [model]...  
                //    ViewBag.TotalSubsDenom = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tago : 0));
                //    ViewBag.TotalSubsCong = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcb : 0));
                //    ViewBag.TotalSysPriv = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsp : 0));
                //    ViewBag.TotalSysRoles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsr : 0));
                //    ViewBag.TotSysProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tms : 0));
                //    ViewBag.TotSubscribers = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsubs : 0));
                //    ViewBag.TotDbaseCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tdb : 0));
                //    ViewBag.TodaysAuditCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_ttc : 0));
                //    ViewBag.TotClientProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_a : 0));
                //    ViewBag.TotClientProfiles_Admins = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_d : 0));

                //    oHomeDashAdmin.TotalSubsDenom = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tago : 0)); ViewBag.TotalSubsDenom = oHomeDashAdmin.TotalSubsDenom;
                //    oHomeDashAdmin.TotalSubsCong = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcb : 0)); ViewBag.TotalSubsCong = oHomeDashAdmin.TotalSubsCong;
                //    oHomeDashAdmin.TotalSysPriv = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsp : 0)); ViewBag.TotalSysPriv = oHomeDashAdmin.TotalSysPriv;
                //    oHomeDashAdmin.TotalSysRoles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsr : 0)); ViewBag.TotalSysRoles = oHomeDashAdmin.TotalSysRoles;
                //    oHomeDashAdmin.TotSysProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tms : 0)); ViewBag.TotSysProfiles = oHomeDashAdmin.TotSysProfiles;
                //    oHomeDashAdmin.TotSubscribers = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tsubs : 0)); ViewBag.TotSubscribers = oHomeDashAdmin.TotSubscribers;
                //    oHomeDashAdmin.TotDbaseCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tdb : 0)); ViewBag.TotDbaseCount = oHomeDashAdmin.TotDbaseCount;
                //    oHomeDashAdmin.TodaysAuditCount = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_ttc : 0)); ViewBag.TodaysAuditCount = oHomeDashAdmin.TodaysAuditCount;
                //    oHomeDashAdmin.TotClientProfiles = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_a : 0)); ViewBag.TotClientProfiles = oHomeDashAdmin.TotClientProfiles;
                //    oHomeDashAdmin.TotClientProfiles_Admins = String.Format("{0:N0}", (res.Count > 0 ? res[0].cnt_tcln_d : 0)); ViewBag.TotClientProfiles_Admins = oHomeDashAdmin.TotClientProfiles_Admins;
                //}


                //oHomeDashAdmin.TotalSubsDenom = String.Format("{0:N0}", 0); ViewBag.TotalSubsDenom = oHomeDashAdmin.TotalSubsDenom;
                //oHomeDashAdmin.TotalSubsCong = String.Format("{0:N0}", 0); ViewBag.TotalSubsCong = oHomeDashAdmin.TotalSubsCong;
                //oHomeDashAdmin.TotalSysPriv = String.Format("{0:N0}", 0); ViewBag.TotalSysPriv = oHomeDashAdmin.TotalSysPriv;
                //oHomeDashAdmin.TotalSysRoles = String.Format("{0:N0}", 0); ViewBag.TotalSysRoles = oHomeDashAdmin.TotalSysRoles;
                //oHomeDashAdmin.TotSysProfiles = String.Format("{0:N0}", 0); ViewBag.TotSysProfiles = oHomeDashAdmin.TotSysProfiles;
                //oHomeDashAdmin.TotSubscribers = String.Format("{0:N0}", 0); ViewBag.TotSubscribers = oHomeDashAdmin.TotSubscribers;
                //oHomeDashAdmin.TotDbaseCount = String.Format("{0:N0}", 0); ViewBag.TotDbaseCount = oHomeDashAdmin.TotDbaseCount;
                //oHomeDashAdmin.TodaysAuditCount = String.Format("{0:N0}", 0); ViewBag.TodaysAuditCount = oHomeDashAdmin.TodaysAuditCount;
                //oHomeDashAdmin.TotClientProfiles = String.Format("{0:N0}", 0); ViewBag.TotClientProfiles = oHomeDashAdmin.TotClientProfiles;
                //oHomeDashAdmin.TotClientProfiles_Admins = String.Format("{0:N0}", 0); ViewBag.TotClientProfiles_Admins = oHomeDashAdmin.TotClientProfiles_Admins;


                // var ls = _context.ChurchBody.ToList();
                //_oUserPrivilegeCol = oUserLogIn_Priv;
                //privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                //TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();


               // TempData.Keep();

                ///
                return View(oHomeDashAdmin);
            //}                         
        }


        //public ActionResult ReRouteDbase()
        //{
        //    string[] args = new string[] { };
        //    Program.CreateHostBuilder2(args).Build().Run();
        //}


        //public  async Task<IActionResult> GetDashboardValues()
        //{
            //var res = new
            //{
            //    tago = await _masterContext.AppGlobalOwner.Where(c => c.Status == "A").ToListAsync(),
            //    tcb = await _masterContext.MSTRChurchBody.Where(c => c.Status == "A" && (c.OrgType == "CH" || c.OrgType == "CN")).ToListAsync(), //.Result.Count(),
            //    tsr = await _masterContext.UserRole.Where(c => c.RoleStatus == "A" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null).ToListAsync(), //.Result.Count(),
            //    tsp = await _masterContext.UserPermission.Where(c => c.PermStatus == "A").ToListAsync(), //.Result.Count(), //,
            //    tms = await _masterContext.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A").ToListAsync(), //.Result.Count(), //,
            //    tcl_d = await (from a in _masterContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A")
            //             from b in _masterContext.UserProfileRole.Where(c => c.UserRole.RoleType == "CH_ADMN" || c.UserRole.RoleType == "CF_ADMN")
            //             select a).ToListAsync(), //.Result.Count(),
            //    tcl_a = await _masterContext.UserProfile.Where(c => c.ProfileScope == "C" && c.UserStatus == "A").ToListAsync(), //.Result.Count(),
            //    tsubs = await _masterContext.AppSubscription.Where(c => c.Slastatus == "A").ToListAsync(), //.Result.Count(),
            //    tdb = await _masterContext.ClientAppServerConfig.Select(c => c.DbaseName).Distinct().ToListAsync(), //.Result.Count(),
            //    ttc = await _masterContext.UserAuditTrail.Where(c => c.EventDate.Date == DateTime.Now.Date).ToListAsync() //.Result.Count()
            //};

        //    return ""; // != null;
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
