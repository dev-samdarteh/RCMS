using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RhemaCMS.Controllers.con_adhc;
using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.CLNTModels;
// using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using RhemaCMS.Models.ViewModels.vm_app_ven;
using static RhemaCMS.Controllers.con_adhc.AppUtilties;
//using static RhemaCMS.Models.ViewModels.AppVenAdminVM;

namespace RhemaCMS.Controllers.con_app_va
{
    public class UserLoginController : Controller
    {
       // private readonly  IConfiguration _configuration;
        private readonly MSTR_DbContext _context;
        //private readonly MSTR_DbContext _masterContext;
        //private ChurchModelContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;


        // private string _strClientConn;
        //private string _clientDBConnString;

       private UserProfile _oLoggedUser;

        // private UserRole _oLoggedRole;
        private MSTRChurchBody _oLoggedCB_MSTR;
        private MSTRAppGlobalOwner _oLoggedAGO_MSTR;

        // private bool isCurrValid = false;
        private UserSessionPrivilege oUserLogIn_Priv = null;

        ///// localized
        //private ChurchBody _oLoggedCB;
        //private AppGlobalOwner _oLoggedAGO;


        //will be initialized at successful login
        // private readonly string _clientDBConnString;
        // private readonly MSTR_DbContext _masterContextLog;
         

        private List<DiscreteLookup> dlUserAuthTypes = new List<DiscreteLookup>();

        private readonly IConfiguration _configuration;
        private string _clientDBConn;
        private ChurchModelContext _clientDBContext;

        public UserLoginController(MSTR_DbContext context, IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory, IConfiguration configuration) //, ChurchModelContext clientDBContext) //, IConfiguration configuration)
        {
            // initialize DBs 
            //_configuration = configuration;

            _context = context;
            _hostingEnvironment = hostingEnvironment;
            //_masterContext = masterContext;
            _configuration = configuration;  /// 

            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;



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
            //    this._oLoggedCB = this.oUserLogIn_Priv.ChurchBody_CLNT; //
            //    this._oLoggedAGO = this.oUserLogIn_Priv.AppGlobalOwner_CLNT;
            //}



            //this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_masterContext, _configuration, (this._oLoggedUser != null ? this._oLoggedUser.AppGlobalOwnerId : (int?)null));

            //if (clientCtx == null)
            //    _context = GetClientDBContext();


            //_clientDBContext = clientDBContext;
            // _masterContextLog = new MSTR_DbContext();

            // var conn = new SqlConnectionStringBuilder(_context.Database.GetDbConnection().ConnectionString);
            //  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
            // conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

            //will be initialized at successful login
            // this._clientDBConnString = ""; //conn.ConnectionString;

            //var a = _configuration.GetConnectionString("DefaultConnection");

            //var bb = context.Database.CanConnect();
            //if (context.Database.GetDbConnection().State != System.Data.ConnectionState.Open) 
            //    context.Database.GetDbConnection().Open();
            //context.Database.GetDbConnection().ChangeDatabase("DBRCMS_CL_TEST");
            //var b = context.Database.GetDbConnection().ConnectionString;

            //var c = _configuration.GetConnectionString("DefaultConnection");



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
            //}


            //// _context = context;
            ////  this._context = clientCtx;
            //if (clientCtx == null)
            //    _context = GetClientDBContext();

            //else
            //{
            //    var conn = new SqlConnectionStringBuilder(clientCtx.Database.GetDbConnection().ConnectionString);
            //    if (conn.DataSource == "_BLNK" || conn.InitialCatalog == "_BLNK") // (string.IsNullOrEmpty(this._clientDBConnString) || this.oUserLogIn_Priv.UserProfile == null)
            //        _context = GetClientDBContext();
            //    else
            //        _context = clientCtx;
            //}


            /// synchronize AGO, CL, CB, CTRY  or @login 
            // this._clientDBConnString = _context.Database.GetDbConnection().ConnectionString;

            ///// get the localized data... using the MSTR data
            //if (_context != null)
            //{
            //    this._oLoggedAGO_MSTR = _context.MSTRAppGlobalOwner.AsNoTracking()
            //                        .Where(c => c.Id == this._oLoggedUser.AppGlobalOwnerId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_AGO).FirstOrDefault();  // one record table...
            //    this._oLoggedCB_MSTR = _context.MSTRChurchBody.AsNoTracking().Include(t => t.ChurchLevel)
            //                        .Where(c => c.AppGlobalOwnerId == this._oLoggedUser.AppGlobalOwnerId && c.Id == this._oLoggedUser.ChurchBodyId && c.GlobalChurchCode == this._oLoggedUser.strChurchCode_CB).FirstOrDefault();
            //}



            dlUserAuthTypes.Add(new DiscreteLookup() { Category = "UserAuthType", Val = "1", Desc = "Two-way Authentication" });
            dlUserAuthTypes.Add(new DiscreteLookup() { Category = "UserAuthType", Val = "2", Desc = "Security Question Validation" });
            //options => options
        }


        [HttpPost]
        public IActionResult InitializeRCMS()
        {
            /////  DEFAULT USERS....  CHECK AND CREATE --- ONLY FOR EMPTY DATABASE
            ///
            /// 

            //SYS account can only be 1... check if it exists .... verify if add roles, perms or sys user profiles first ... thus for SYS acc only ... once SUP_ADMN created... NEVER execute this code.... Restore to default can be done by SUP_ADMN unless SYS acc > SUP_ADMN acc
            //const string def_init_hash = "10c16e2d260b87e96096c18991b57d9233453ae4eb3125ed0e34ecde2af3fa36";
            const string def_initkey_hash = "d38e8e28f06fbd35e89e67ea132da62c976af6dff36e02877d2236b6a12961ca";
            const string def_initp_hash = "10c16e2d260b87e96096c18991b57d9233453ae4eb3125ed0e34ecde2af3fa36";

            var dbUsers = _context.UserProfile.AsNoTracking().ToList(); 
            var dbSYSUser = _context.UserProfile.AsNoTracking().Where(c=>c.Username.ToLower()=="SYS".ToLower() && c.UserStatus=="A" && c.UserKey==ac2).FirstOrDefault(); 
            if (dbUsers.Count() == 0 || (dbUsers.Count > 0 && dbSYSUser == null)) // (checkSYSAccOnlyAndNone <= 1 && AppUtilties.ComputeSha256Hash(model.ChurchCode) == ac1 && AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower()) == ac2 && AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower() + model.Password) == ac3)
            //6-digit vendor code 6-digit code for churches ... [church code: 0000000000 + ?? userid + ?? pwd] + no existing SUPADMIN user ... pop up SUPADMIN for new SupAdmin()
            {
                LogOnVM model = new LogOnVM();

                model.UserProfiles = _context.UserProfile.AsNoTracking().ToList();
                ViewData["strAppName"] = "RHEMA-CMS";
                model.ChurchCode = "000000"; //vendor code but only in ViewModel -- not in db

              //  var logoutCurrUser = false;
               // UserProfile oUser_MSTR = null;

                var _userTask = "Initializing RCMS setup"; var _tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "N",
                                     "RCMS System", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));

                try
                { 
                    var tm = DateTime.Now;
                    //if no SYS... create one and only 1... then create other users
                    var userList = (from t_up in _context.UserProfile.AsNoTracking().Where(c => model.ChurchCode == "000000" && c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.ProfileScope == "V" && c.UserStatus == "A" && c.Username.ToLower() == "SYS".ToLower())   //UserKey
                                    from t_upr in _context.UserProfileRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null && c.ChurchBodyId == null && c.UserProfileId == t_up.Id && c.ProfileRoleStatus == "A").DefaultIfEmpty()
                                    from t_ur in _context.UserRole.AsNoTracking().Where(c => c.AppGlobalOwnerId == null &&  c.ChurchBodyId == null && c.Id == t_upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SYS").DefaultIfEmpty()
                                    select t_up
                                    ).ToList();


                    //SYS acc exists... and more users... sign out
                    // if (userList.Count() > 1) return RedirectToAction("LoginUserAcc", "UserLogin");

                    // create SYS account and login again...
                    var _userChanges = 0;

                    var oSYSUser = new UserProfile();
                    int? oSYSUserId = null;
                    if (userList.Count() == 0)
                    {
                        //create the SYS acc...
                        //var oUserVm = new AppVenAdminVM.UserProfileVM();
                        //var upc = new UserProfileController(_context, _clientDBContext, null);
                        //var p = upc.AddOrEdit_SYS(oUserVm, model.ChurchCode) as JsonResult;
                        //var mes = p.Value.ToString();

                        //oSYSUser = new UserProfile();                        
                        //oSYSUser.AppGlobalOwnerId = null; // oCV.ChurchBody != null ? oCV.ChurchBody.AppGlobalOwnerId : null; //oSYSUser.ChurchBodyId = null; //(int)oCV.ChurchBody.Id; //oSYSUser.OwnerId =null; // (int)vmMod.oCurrLoggedUserId;

                        oSYSUser.Strt = tm;
                        // ChurchBody == null //oSYSUser.Expr = null; // tm.AddDays(90);  //default to 30 days //  oCurrVmMod.oUserProfile.UserId = oCurrChuMemberId_LogOn;  //oSYSUser.ChurchMemberId = null; // vmMod.oCurrLoggedMemberId;

                        oSYSUser.ProfileLevel = 1;
                        oSYSUser.UserScope = "E"; // I-internal, E-external
                        oSYSUser.ProfileScope = "V"; // V-Vendor, C-Client
                        oSYSUser.ResetPwdOnNextLogOn = false; // true;
                        oSYSUser.PwdSecurityQue = "What account is this?"; oSYSUser.PwdSecurityAns = "RHEMA-SYS";
                        oSYSUser.PwdSecurityAns = AppUtilties.ComputeSha256Hash(oSYSUser.PwdSecurityQue + oSYSUser.PwdSecurityAns);
                        //oSYSUser.Email =  "samuel@rhema-systems.com";  // ???   ... user unknown [ what email to use ? ]
                        // oSYSUser.PhoneNum = "233242188212";   // ???  ... user unknown [ what phone to use ? ]
                        oSYSUser.UserDesc = "SYS Profile";
                    ///                        
                        oSYSUser.Username = "SYS"; 
                        oSYSUser.Pwd = def_initp_hash; //var cc = "000000";  "65***1";  //10c16e2d260b87e96096c18991b57d9233453ae4eb3125ed0e34ecde2af3fa36 // [ get the raw hashed data instead ]
                        oSYSUser.UserKey = def_initkey_hash; // AppUtilties.ComputeSha256Hash(cc + oSYSUser.Username.Trim().ToLower());                            
                        //oSYSUser.Pwd = AppUtilties.ComputeSha256Hash(cc + oSYSUser.Username.Trim().ToLower() + oSYSUser.Pwd);

                       // oSYSUser.PwdExpr = tm.AddDays(30);  //default to 90 days ... DO NOT EXPIRE THE SYS ACC... whether pwd or acc itself.
                        oSYSUser.UserStatus = "A"; // A-ctive...D-eactive
                         
                        oSYSUser.Created = tm;
                        oSYSUser.LastMod = tm; ;
                        oSYSUser.CreatedByUserId = null; // (int)vmMod.oCurrLoggedUserId;
                        oSYSUser.LastModByUserId = null; // (int)vmMod.oCurrLoggedUserId;

                        //oSYSUser.UserPhoto = null; //oSYSUser.UserId = null; //oSYSUser.PhoneNum = null; //oSYSUser.Email = null;  
                        _context.Add(oSYSUser);

                        //save everything
                        _context.SaveChanges();

                        oSYSUserId = oSYSUser.Id;
                    ///
                        //_context.Dispose();
                        ///
                        _userTask = "Created SYS control profile"; _tm = DateTime.Now;
                    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                            "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));

                        //  logoutCurrUser = true;

                        _userChanges++;
                    }
                    //else    ....only new stuff here! Updates will go on-demand by admin
                    //{
                    //    oSYSUser = userList[0];
                    //    ///
                    //    _userChanges++;
                    //}



                    // logoutCurrUser = _permChanges > 0;
                    if (_userChanges > 0)
                    {
                        ////clear table...
                        ////_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [" + tabName + "]");
                        //var lsRows = _context.UserProfile.ToList();
                        //if (lsRows.Count() > 0)
                        //{
                        //    _context.UserProfile.RemoveRange(lsRows);
                        //    _context.SaveChanges();

                        //    // RESEED... auto column Id to start from 1 again
                        //    var tabName = "UserProfile";
                        //    _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                        //}

                        //_context.SaveChanges();
                        ///
                        _userTask = "Created " + _userChanges + " default user profiles"; _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                    }


                    // string strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode); //church code ...0x6... 91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203
                    //  string _strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username);  // user  ...0x6... f7b11509f4d675c3c44f0dd37ca830bb02e8cfa58f04c46283c4bfcbdce1ff45
                    //  string strRootCode0 = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username + model.Password);  // pwd ...$0x6... 78415a1535ca0ef885aa7c0278a4de274b85d0c139932cc138ba6ee5cac4a00b

                    //create the user permissions
                    var oUtil = new AppUtilties();
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
                    // 

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
                                CreatedByUserId = oSYSUserId, // oSYSUser.Id,
                                LastModByUserId = oSYSUserId, //oSYSUser.Id
                            });

                            _permChanges++;
                        }
                    }

                    // logoutCurrUser = _permChanges > 0;
                    if (_permChanges > 0)
                    {
                        //clear table...
                        //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [" + tabName + "]");
                        var lsRows = _context.UserPermission.ToList();
                        if (lsRows.Count() > 0)
                        {
                            _context.UserPermission.RemoveRange(lsRows);
                            _context.SaveChanges();

                            // RESEED... auto column Id to start from 1 again
                            var tabName = "UserPermission";
                            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                        }

                        _context.SaveChanges();
                        ///
                        _userTask = "Created " + _permChanges + " default user permissions"; _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                    }


                    //create the user roles 
                    var roleList = oUtil.GetSystemDefaultRoles();

                    var _roleChanges = 0;
                    for (var i = 0; i < roleList.Count; i++)
                    {
                        var checkExist = _context.UserRole.AsNoTracking().Where(c => c.RoleType.ToLower().Equals(roleList[i].RoleType.ToLower())).FirstOrDefault();
                        if (checkExist == null)
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
                                CreatedByUserId = oSYSUserId, //oSYSUser.Id,
                                LastModByUserId = oSYSUserId, //oSYSUser.Id
                            });

                            _roleChanges++;
                        }
                    }


                    ////create the user groups 
                    //var groupList = oUtil.GetSystemDefaultGroups();

                    //var _groupChanges = 0;
                    //for (var i = 0; i < groupList.Count; i++)
                    //{
                    //    var checkExist = _context.UserGroup.Where(c => c.GroupName.ToLower().Equals(groupList[i].GroupName.ToLower())).FirstOrDefault();
                    //    if (checkExist == null)
                    //    {
                    //        _context.Add(new UserGroup()
                    //        {
                    //            ChurchBodyId = null,
                    //            GroupName = groupList[i].GroupName,
                    //            //CustomGroupName = groupList[i].CustomGroupName,
                    //            GroupType = groupList[i].GroupType,
                    //            GroupDesc = groupList[i].GroupDesc,
                    //            GroupLevel = groupList[i].GroupLevel,
                    //            Status = groupList[i].GroupStatus, //"A",
                    //            UserGroupCategoryId = groupList[i].userGroupCategoryId,
                    //            OwnedBy = groupList[i].OwnedBy,
                    //            Created = tm,
                    //            LastMod = tm,
                    //            CreatedByUserId = oSYSUserId, //oSYSUser.Id,
                    //            LastModByUserId = oSYSUserId, //oSYSUser.Id
                    //        });

                    //        _groupChanges++;
                    //    }
                    //}


                    // logoutCurrUser = _roleChanges > 0;
                    if (_roleChanges > 0)
                    {
                        //clear table...
                        //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [" + tabName + "]");
                        var lsRows = _context.UserRole.ToList();
                        if (lsRows.Count() > 0)
                        {
                            _context.UserRole.RemoveRange(lsRows);
                            _context.SaveChanges();

                            // RESEED... auto column Id to start from 1 again
                            var tabName = "UserRole";
                            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                        }

                        _context.SaveChanges();
                        ///
                        _userTask = "Created the default SYS role"; _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                    }


                    //create user groups
                     

                    // add perms to new role (s) created.. SYS role - SUP_ADMN perm  :: lsPerms.Add(new UserPermission(0, null, "A0_00", "_A0_00__Super_Admin_Account", null, "A", null, null, null, null, null));   // for SYS account only
                    // 1 role --- 2 perms
                    var userRole = _context.UserRole.AsNoTracking().Where(c => c.RoleType == "SYS" && c.RoleStatus == "A").FirstOrDefault();
                    var userPermList = _context.UserPermission.AsNoTracking().Where(c => (c.PermissionCode == "A0_00" || c.PermissionCode == "A0_01") && c.PermStatus == "A").ToList();
                    var _URPChanges = 0;
                    if (userRole != null && userPermList.Count > 0)
                    {
                        foreach (var userPerm in userPermList) //var i = 0; i < userPermList.Count; i++)
                        {
                            var checkURPExist = _context.UserRolePermission.AsNoTracking().Where(c => c.UserPermissionId == userPerm.Id && c.UserRoleId == userRole.Id && c.Status == "A").FirstOrDefault();
                            if (checkURPExist == null)
                            {
                                _context.Add(new UserRolePermission()
                                {
                                    ChurchBodyId = null,
                                    UserPermissionId = userPerm.Id,
                                    UserRoleId = userRole.Id,
                                    Status = "A",
                                    ViewPerm = true,
                                    CreatePerm = true,
                                    EditPerm = true,
                                    DeletePerm = true,
                                    ManagePerm = true,
                                    Created = tm,
                                    LastMod = tm,
                                    CreatedByUserId = oSYSUserId, //oSYSUser.Id,
                                    LastModByUserId = oSYSUserId, //oSYSUser.Id
                                });

                                _URPChanges++;
                            }
                        }
                   }

                    // logoutCurrUser = _URPChanges > 0;
                    if (_URPChanges > 0)
                    {
                        //clear table...
                        //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [" + tabName + "]");
                        var lsRows = _context.UserRolePermission.ToList();
                        if (lsRows.Count() > 0)
                        {
                            _context.UserRolePermission.RemoveRange(lsRows);
                            _context.SaveChanges();

                            // RESEED... auto column Id to start from 1 again
                            var tabName = "UserRolePermission";
                            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                        }

                        _context.SaveChanges();
                     ///
                     _userTask = "Assigned " + _URPChanges + " permission(s) to default SYS role"; _tm = DateTime.Now;
                     _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                             "RCMS User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                    }
                   

                    // add role (s) for the new acc created.. SYS acc - SYS role - SYS permission : lsPerms.Add(new UserPermission(0, null, "A0_00", "_A0_00__Super_Admin_Account", null, "A", null, null, null, null, null));   // for SYS account only
                    //string strUserKeyHashedData = AppUtilties.ComputeSha256Hash("000000" + "SYS".Trim().ToLower()); 

                    //var userProfile = _context.UserProfile.AsNoTracking().Include(t=>t.AppGlobalOwner).Include(t=>t.ChurchBody)
                    //    .Where(c => c.AppGlobalOwnerId==null && c.ChurchBodyId==null && c.UserStatus == "A" && 
                    //                                        c.Username.Trim().ToLower() == "SYS".Trim().ToLower() && c.UserKey==def_initkey_hash).FirstOrDefault(); 
                    ///
                    // oUser_MSTR = userProfile;

                    //var userRole = _context.UserRole.Where(c => c.RoleName == "SYS" && c.RoleStatus == "A").FirstOrDefault();
                    var _UPRChanges = 0;
                    if (userRole != null && oSYSUser != null)
                    {
                        //for (var i = 0; i < permList.Count; i++)
                        //{
                        var checkUPRExist = _context.UserProfileRole.AsNoTracking().Where(c => c.UserProfileId == oSYSUserId && c.UserRoleId == userRole.Id && c.ProfileRoleStatus == "A").FirstOrDefault();
                        if (checkUPRExist == null)
                        {
                            _context.Add(new UserProfileRole()
                            {
                                ChurchBodyId = null,
                                UserProfileId = (int)oSYSUserId, //oSYSUser.Id,
                                UserRoleId = userRole.Id,
                                ProfileRoleStatus = "A",
                                Strt = tm,
                                Expr = null,
                                Created = tm,
                                LastMod = tm,
                                CreatedByUserId = oSYSUserId, //oSYSUser.Id,
                                LastModByUserId = oSYSUserId, //oSYSUser.Id
                            });

                            _UPRChanges++;
                        }
                    }
                    //}

                    //  logoutCurrUser = _UPRChanges > 0;
                    if (_UPRChanges > 0)
                    {
                        //clear table...
                        //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [" + tabName + "]");
                        var lsRows = _context.UserProfileRole.ToList();
                        if (lsRows.Count() > 0)
                        {
                            _context.UserProfileRole.RemoveRange(lsRows);
                            _context.SaveChanges();

                            // RESEED... auto column Id to start from 1 again
                            var tabName = "UserProfileRole";
                            _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                        }

                        _context.SaveChanges();
                        ///
                        _userTask = "Assigned " + _UPRChanges + " role(s) to SYS profile"; _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                    }


                    // initialize the COUNTRY
                    //var countriesList = AppUtilties.GetMS_BaseCountries(); //.GetMS_Countries();
                    //var _ctryCount = 0;
                    //if (countriesList.Count > 0)
                    //{
                    //    foreach (var oCtry in countriesList)
                    //    {
                    //        var checkCTRYExist = _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code ).FirstOrDefault();
                    //        if (checkCTRYExist == null)
                    //        {
                    //            _context.Add(new MSTRCountry()
                    //            {
                    //                CtryAlpha3Code = oCtry.CtryAlpha3Code,  //key
                    //                EngName = oCtry.EngName,                                   
                    //                CapitalCity = oCtry.CapitalCity,                                   
                    //                CtryAlpha2Code = oCtry.CtryAlpha2Code,
                    //                CurrEngName = oCtry.CurrEngName,
                    //                CurrLocName = oCtry.CurrLocName,
                    //                CurrSymbol = oCtry.CurrSymbol,
                    //                Curr3LISOSymbol = oCtry.Curr3LISOSymbol,
                    //              //  SharingStatus = "N", 
                    //                Created = tm,
                    //                LastMod = tm,
                    //                CreatedByUserId = oSYSUser.Id,
                    //                LastModByUserId = oSYSUser.Id
                    //            });

                    //            _ctryCount++;
                    //        }
                    //    }
                    //}


                    //if (_ctryCount > 0)
                    //{
                    //    _context.SaveChanges();
                    //    ///
                    //    _userTask = "Created " + _ctryCount + " countries."; _tm = DateTime.Now;
                    //    _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                    //                            "RCMS-Admin: Country", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                    //}

                    // var a = oUser_MSTR;
                    var countriesList = AppUtilties.GetMS_BaseCountries();
                    // var b = oUser_MSTR;
                    if (countriesList.Count > 0)
                    {
                        //var tm = DateTime.Now;
                        var _ctryNewCount = 0; var _ctryUpdCount = 0;
                        foreach (var oCtry in countriesList)
                        {
                            var oCTRYExist = _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code).FirstOrDefault();
                            if (oCTRYExist == null)
                            {
                                _context.Add(new MSTRCountry()
                                {
                                    CtryAlpha3Code = oCtry.CtryAlpha3Code,  //key
                                    EngName = oCtry.EngName,
                                    CtryAlpha2Code = oCtry.CtryAlpha2Code,
                                    CurrEngName = oCtry.CurrEngName,
                                    CurrLocName = oCtry.CurrLocName,
                                    CurrSymbol = oCtry.CurrSymbol,
                                    Curr3LISOSymbol = oCtry.Curr3LISOSymbol,
                                    CallingCode = oCtry.CallingCode,
                                    //  SharingStatus = "N", 
                                    Created = _tm,
                                    LastMod = _tm,
                                    CreatedByUserId = oSYSUserId, // oUser_MSTR.Id,
                                    LastModByUserId = oSYSUserId // oUser_MSTR.Id
                                });

                                _ctryNewCount++;
                            }
                            //else  ///... updates to be done on-demand by admin
                            //{
                            //    oCTRYExist.CtryAlpha3Code = oCtry.CtryAlpha3Code;
                            //    oCTRYExist.EngName = oCtry.EngName;
                            //    oCTRYExist.CtryAlpha2Code = oCtry.CtryAlpha2Code;
                            //    oCTRYExist.CurrEngName = oCtry.CurrEngName;
                            //    oCTRYExist.CurrLocName = oCtry.CurrLocName;
                            //    oCTRYExist.CurrSymbol = oCtry.CurrSymbol;
                            //    oCTRYExist.Curr3LISOSymbol = oCtry.Curr3LISOSymbol;
                            //    //oCTRY. SharingStatus = "N";
                            //    // oCTRY.Created = tm;
                            //    oCTRYExist.LastMod = _tm;
                            //    // oCTRY.CreatedByUserId = oUser_MSTR.Id;
                            //    oCTRYExist.LastModByUserId = oUser_MSTR.Id;

                            //    _context.MSTRCountry.Update(oCTRYExist);
                            //    _ctryUpdCount++;
                            //}
                        }


                        if ((_ctryNewCount + _ctryUpdCount) > 0)
                        {
                            //clear table...
                            //_context.Database.ExecuteSqlRaw("TRUNCATE TABLE [" + tabName + "]");
                            var lsRows = _context.MSTRCountry.ToList();
                            if (lsRows.Count() > 0)
                            {
                                _context.MSTRCountry.RemoveRange(lsRows);
                                _context.SaveChanges();

                                // RESEED... auto column Id to start from 1 again
                                var tabName = "MSTRCountry";
                                _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('" + tabName + "', RESEED, 0)");
                            }

                            _context.SaveChanges();
                            ///
                            _userTask = "Created/updated " + (_ctryNewCount + _ctryUpdCount) + " countries."; _tm = DateTime.Now;
                            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                    "RCMS-Admin: Country", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                        }
                    }

                    if (oSYSUser == null) oSYSUser = _context.UserProfile.Find(oSYSUserId);
                    if (oSYSUser != null)
                    {
                        // update master db-user   
                        oSYSUser.IsMSTRInit = true;
                        oSYSUser.LastMod = DateTime.Now;
                        // oUser_MSTR.LastModByUserId = oUser_MSTR.Id;
                        _context.Update(oSYSUser);
                        ///
                        _context.SaveChanges();

                        ////logout ...login to authenticate
                        //if (logoutCurrUser == true) return RedirectToAction("LoginUserAcc", "UserLogin");
                    }

                }

                catch (Exception ex)
                {
                    throw;
                }
            }

            return RedirectToAction("LoginUserAcc", "UserLogin");
        }



        [HttpGet]
        public IActionResult LoginUserAcc()
        {
            try
            {
                // SHOULD BE ON THE PAGE directly.... < modify this later > clear the cache, stored logging details... @refresh... redirect to login page
                // Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //


                HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Response.Headers.Add("Pragma", "no-cache");
                HttpContext.Response.Headers.Add("Expires", "0");
                HttpContext.Session.Clear();


                // HttpContext.Response.Cookies.Delete();
                ///
                //TempData.Clear();            

                ///
                LogOnVM model = new LogOnVM();
                model.ChurchCode = "";
                model.Username = "";
                model.Password = "";

                // ViewData["strAppName"] = "RHEMA-CMS";
                // ViewData["strAppName"] = "RhemaCMS";
                // ViewData["strAppLogo_Filename"] = "df_rhema.jpg"; // "~/img_db/df_rhema.jpg"; // oAppGloOwn?.ChurchLogo;


                ViewData["strAppName"] = "RhemaCMS";
                ViewData["strAppLogo_Filename"] = "df_rhema.jpg";

                /////  DEFAULT USERS....  CHECK AND CREATE --- ONLY FOR EMPTY DATABASE
                /// 

                var isDBUserNull = _context.UserProfile.AsNoTracking().Count() == 0;
                if (!isDBUserNull)
                {
                   /// model.UserProfiles = _context.UserProfile.AsNoTracking().ToList();
                    ViewData["VerificationCodeEnabled"] = false;

                    return View(model);
                }

                else
                {
                    /// this will be done ONLY at new, empty database
                    return InitializeRCMS();
                    // perform  --->>>>  DEFAULT USERS and ROLES
                    // return RedirectToAction("InitializeRCMS"); 
                    
                }

                /// return View("LoginUserAcc", model);

            }
            catch (Exception ex)
            {
                return View("_ErrorPage");
            } 
        }


        private const string ac1 = "91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203"; //  ChurchCode =  encrypt(cc)  //"91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203";
        private const string ac2 = "d38e8e28f06fbd35e89e67ea132da62c976af6dff36e02877d2236b6a12961ca"; //  UserKey = encrypt(cc + usr) //"f7b11509f4d675c3c44f0dd37ca830bb02e8cfa58f04c46283c4bfcbdce1ff45";
        private const string ac3 = "10c16e2d260b87e96096c18991b57d9233453ae4eb3125ed0e34ecde2af3fa36"; //  User Pwd = encrypt(cc + usr + pwd) "78415a1535ca0ef885aa7c0278a4de274b85d0c139932cc138ba6ee5cac4a00b"; check thru codes

        [HttpPost]
        [ValidateAntiForgeryToken]
        //  public  ActionResult LoginUserAcc([Bind(include: "Username,Pwd")] UserProfile userProfile) 
        public ActionResult LoginUserAcc(LogOnVM model, int strIsVal = 0, bool navB = false, bool navBClr = false) //, string returnUrl)
        
        {
            //clear the model state first before adding ... any more error
            //if (ModelState.ContainsKey("")) ModelState[""].Errors.Clear();
            //ModelState[""].ValidationState = ModelValidationState.Valid;

            //foreach (var key in ModelState.Keys)
            //{
            //    ModelState[key].Errors.Clear();
            //    ModelState[key].ValidationState = ModelValidationState.Valid;
            //}

            ViewData["strAppName"] = "RhemaCMS";
            ViewData["strAppLogo_Filename"] = "df_rhema.jpg";

            //if (strIsVal == null) strIsVal = "F";
            ViewData["VerificationCodeEnabled"] = false;

            var _userTask = "User attempt login to RCMS initiated"; 
            var _tm = DateTime.Now;
            _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "L",
                                     "RCMS Login"  , AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
            //

            model.IsVal = strIsVal;  
            if (navBClr == true)
            {
                model.ChurchCode = "";
                model.Username = "";
                model.Password = "";

                return View(model);
            }

            if (navB)
            {
                model.IsVal = 0; // invalidate user ....
                return View(model);
            }

            if (model == null)
            {
                model.IsVal = 0; // invalidate user ....
                ModelState.AddModelError("", "Failed to retrieve data. Please try again.");
                return View(model);
            }

            var err = false;
            if (model.ChurchCode == null) { err = true; ModelState.AddModelError("", "Enter Church code"); }
            if (model.Username == null) { err = true; ModelState.AddModelError("", "Enter username."); }
            if (model.Password == null) { err = true; ModelState.AddModelError("", "Enter user password."); }


            //trim trailing and leading spaces
            //if (!string.IsNullOrEmpty(model.Username)) model.Username = model.Username.Trim();

            if (err==true)
            {
                model.IsVal = 0;     // invalidate user ....            
                return View(model);
            }

            //model.IsVal = strIsVal == null ? 0 : strIsVal;  //strIsVal == null ? "F" : strIsVal;
                   

            //Id,ChurchBodyId,OwnerId,UserDesc,Username,Pwd,Email,PhoneNum,STRT,EXPR,UserStatus,Created,LastMod
            //ModelState.Remove("AppGlobalOwnerId");

            ModelState.Remove("ChurchBodyId");
            ModelState.Remove("Email");
            ModelState.Remove("PhoneNum");

            //foreach (var key in ModelState.Keys)
            //{
            //    ModelState.Remove(key);
            //    // ModelState.Remove("BasicCredentialsVerified");
            //    // ModelState[key].Errors.Clear();
            //}

            //   ModelState.Remove("BasicCredentialsVerified");
            ModelState.Remove("IsBasicVerified");

            //ViewBag.VerificationCodeEnabled = false;

            //  bool valid = false;
            var isUserValidated = false;
            UserSessionPrivilege oUserPrivilegeCol = null;

            // UserProfile oUser_MSTR = null;

            if ((model.IsVal != 1 && ModelState.IsValid) || (!ModelState.IsValid)) //model.Username == null || model.Password == null))
            //((model.BasicCredentialsVerified) || (model.BasicCredentialsVerified == false && ModelState.IsValid))
            {
                var _cc = "000000";   //"10c16e2d260b87e96096c18991b57d9233453ae4eb3125ed0e34ecde2af3fa36"
                string _ac1 = AppUtilties.ComputeSha256Hash(_cc);
                string _ac2 = AppUtilties.ComputeSha256Hash(_cc + model.Username.Trim().ToLower());
                string _ac3 = AppUtilties.ComputeSha256Hash(_cc + model.Username.Trim().ToLower() + model.Password);

                //string _strPwdHashedData1 = AppUtilties.ComputeSha256Hash(model.Username.Trim().ToLower() + model.Password);

                //string strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode); //church code ... 91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203
                //string _strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower());  // user  ... f7b11509f4d675c3c44f0dd37ca830bb02e8cfa58f04c46283c4bfcbdce1ff45
                //string strRootCode0 = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower() + model.Password);  // pwd ... 78415a1535ca0ef885aa7c0278a4de274b85d0c139932cc138ba6ee5cac4a00b

                // string strRootCode1 = AppUtilties.ComputeSha256Hash(model.Username + "$rhemacloud"); // lotto scam: 0557 58 38 46
                // string strRootCode = AppUtilties.ComputeSha256Hash(model.Username + "RHEMA_SYS1");

                //  string strRootCode1 = AppUtilties.ComputeSha256Hash("danwool" + "$rhemacloud");
                // string strRootCode1 = AppUtilties.ComputeSha256Hash("joe" + "$rhemacloud");
                //  string strRootCode = AppUtilties.ComputeSha256Hash(model.Username + "RHEMA_Sup_Admn1");
                //string strRootCode3 = AppUtilties.ComputeSha256Hash("joe" + "RHEMA_Sup_Admn1");
                //string strRootCode4 = AppUtilties.ComputeSha256Hash("dabrokwah" + "RHEMA_Sup_Admn1");
                //string strRootCode5 = AppUtilties.ComputeSha256Hash("test" + "RHEMA_Sup_Admn1");
                //string strRootCode6 = AppUtilties.ComputeSha256Hash("test" + "$rhemacloud");

                // bool userValidated = model.IsBasicVerified == "T";
                // if user is NOT valiadated...  ---- >>> validate

                
                if (model.IsVal != 1)
                {
                   // var logoutCurrUser = false;

                    UserProfile oUser_MSTR = null;

                    //SYS account can only be 1... check if it exists .... verify if add roles, perms or sys user profiles first ... thus for SYS acc only ... once SUP_ADMN created... NEVER execute this code.... Restore to default can be done by SUP_ADMN unless SYS acc > SUP_ADMN acc
                    //6-digit vendor code 6-digit code for churches ... [church code: 0000000000 + ?? userid + ?? pwd] + no existing SUPADMIN user ... pop up SUPADMIN for new SupAdmin()

                    // New DB -- setup --->> InitializeRCMS
                    var checkSYSAcc_Count = _context.UserProfile.AsNoTracking().Count();
                    if (checkSYSAcc_Count == 0 && // no user created yet; InitializeSetup should be called by now
                        AppUtilties.ComputeSha256Hash(model.ChurchCode) == ac1 && // church code
                        AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower()) == ac2 && // user key
                        AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower() + model.Password) == ac3) // pwd
                    {
                        /// InitializeRCMS ... is to be done at creation of new database... empty ---->> by vendor
                        ///

                        InitializeRCMS();

                        //try
                        //{
                        //    // string strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode); //church code ...0x6... 91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203
                        //    //  string _strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username);  // user  ...0x6... f7b11509f4d675c3c44f0dd37ca830bb02e8cfa58f04c46283c4bfcbdce1ff45
                        //    //  string strRootCode0 = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username + model.Password);  // pwd ...$0x6... 78415a1535ca0ef885aa7c0278a4de274b85d0c139932cc138ba6ee5cac4a00b

                        //    //create the user permissions
                        //    var oUtil = new AppUtilties();
                        //    var permList = oUtil.GetSystem_Administration_Permissions();
                        //    var permList1 = oUtil.GetAppDashboard_Permissions();
                        //    var permList2 = oUtil.GetAppConfigurations_Permissions();
                        //    var permList3 = oUtil.GetMemberRegister_Permissions();
                        //    var permList4 = oUtil.GetChurchlifeAndEvents_Permissions();
                        //    var permList5 = oUtil.GetChurchAdministration_Permissions();
                        //    var permList6 = oUtil.GetFinanceManagement_Permissions();
                        //    var permList7 = oUtil.GetReportsAnalytics_Permissions();

                        //    //var permList3 = oUtil.get();
                        //    permList = AppUtilties.CombineCollection(permList, permList1, permList2, permList3, permList4);
                        //    permList = AppUtilties.CombineCollection(permList, permList5, permList6, permList7);
                        //    //
                        //    var tm = DateTime.Now;
                        //    var _permChanges = 0;
                        //    for (var i = 0; i < permList.Count; i++)
                        //    {
                        //        var checkExist = _context.UserPermission.Where(c => c.PermissionName.ToLower().Equals(permList[i].PermissionName.ToLower())).FirstOrDefault();
                        //        if (checkExist == null)
                        //        {
                        //            _context.Add(new UserPermission()
                        //            { 
                        //                PermissionName = permList[i].PermissionName,
                        //                PermissionCode = AppUtilties.GetPermissionCode_FromName(permList[i].PermissionName),
                        //                PermStatus = "A",
                        //                Created = tm,
                        //                LastMod = tm,
                        //                CreatedByUserId = null,
                        //                LastModByUserId = null
                        //            });

                        //            _permChanges++;
                        //        }
                        //    }

                        //    logoutCurrUser = _permChanges > 0;
                        //    if (_permChanges > 0) _context.SaveChanges();


                        //    //create the user roles 
                        //    var roleList = oUtil.GetSystemDefaultRoles();

                        //    var _roleChanges = 0;
                        //    for (var i = 0; i < roleList.Count; i++)
                        //    {
                        //        var checkExist = _context.UserRole.Where(c => c.RoleName.ToLower().Equals(roleList[i].RoleName.ToLower())).FirstOrDefault();
                        //        if (checkExist == null)
                        //        {
                        //            _context.Add(new UserRole()
                        //            {
                        //                ChurchBodyId = null,
                        //                RoleName = roleList[i].RoleName,
                        //                RoleType = roleList[i].RoleType,
                        //                RoleDesc = roleList[i].RoleDesc,
                        //                RoleLevel = roleList[i].RoleLevel,
                        //                RoleStatus = "A",
                        //                Created = tm,
                        //                LastMod = tm,
                        //                CreatedByUserId = null,
                        //                LastModByUserId = null
                        //            });

                        //            _roleChanges++;
                        //        }
                        //    }

                        //    logoutCurrUser = _roleChanges > 0;
                        //    if (_roleChanges > 0) _context.SaveChanges();


                        //    //create user groups


                        //    //if no SYS... create one and only 1... then create other users
                        //    var userList = (from t_up in _context.UserProfile.Where(c => model.ChurchCode == "000000" && c.ChurchBodyId == null && c.ProfileScope == "V" && c.UserStatus == "A")
                        //                    from t_upr in _context.UserProfileRole.Where(c => c.ChurchBodyId == null && c.UserProfileId == t_up.Id && c.ProfileRoleStatus == "A").DefaultIfEmpty()
                        //                    from t_ur in _context.UserRole.Where(c => c.ChurchBodyId == null && c.Id == t_upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 0 && c.RoleType == "SYS").DefaultIfEmpty()
                        //                    select t_up
                        //                    ).ToList();


                        //    //SYS acc exists... and more users... sign out
                        //    if (userList.Count > 1) return RedirectToAction("LoginUserAcc", "UserLogin");
                            
                        //    // create SYS account and login again...
                        //    if (userList.Count == 0)
                        //    {
                        //        //create the SYS acc...
                        //        //var oUserVm = new AppVenAdminVM.UserProfileVM();
                        //        //var upc = new UserProfileController(_context, _clientDBContext, null);
                        //        //var p = upc.AddOrEdit_SYS(oUserVm, model.ChurchCode) as JsonResult;
                        //        //var mes = p.Value.ToString();

                        //        var _oChanges = new UserProfile();

                        //        //_oChanges.AppGlobalOwnerId = null; // oCV.ChurchBody != null ? oCV.ChurchBody.AppGlobalOwnerId : null; //_oChanges.ChurchBodyId = null; //(int)oCV.ChurchBody.Id; //_oChanges.OwnerId =null; // (int)vmMod.oCurrLoggedUserId;
                                 
                        //        _oChanges.Strt = tm;
                        //        // ChurchBody == null //_oChanges.Expr = null; // tm.AddDays(90);  //default to 30 days //  oCurrVmMod.oUserProfile.UserId = oCurrChuMemberId_LogOn;  //_oChanges.ChurchMemberId = null; // vmMod.oCurrLoggedMemberId;

                        //        _oChanges.UserScope = "I"; // I-internal, E-external
                        //        _oChanges.ProfileScope = "V"; // V-Vendor, C-Client
                        //        _oChanges.ResetPwdOnNextLogOn = false; // true;
                        //        _oChanges.PwdSecurityQue = "What account is this?"; _oChanges.PwdSecurityAns = "Rhema-SYS";
                        //        _oChanges.PwdSecurityAns = AppUtilties.ComputeSha256Hash(_oChanges.PwdSecurityQue + _oChanges.PwdSecurityAns);
                        //        //_oChanges.Email =  "samuel@rhema-systems.com";  // ???   ... user unknown [ what email to use ? ]
                        //       // _oChanges.PhoneNum = "233242188212";   // ???  ... user unknown [ what phone to use ? ]
                        //        _oChanges.UserDesc = "Sys Profile";

                        //        var cc = "000000"; _oChanges.Username = "Sys"; _oChanges.Pwd = "654321"; // [ get the raw data instead ]
                        //        _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower() + _oChanges.Pwd);

                        //        _oChanges.PwdExpr = tm.AddDays(30);  //default to 90 days 
                        //        _oChanges.UserStatus = "A"; // A-ctive...D-eactive

                        //        _oChanges.Created = tm;
                        //        _oChanges.LastMod = tm; ;
                        //        _oChanges.CreatedByUserId = null; // (int)vmMod.oCurrLoggedUserId;
                        //        _oChanges.LastModByUserId = null; // (int)vmMod.oCurrLoggedUserId;

                        //        //_oChanges.UserPhoto = null; //_oChanges.UserId = null; //_oChanges.PhoneNum = null; //_oChanges.Email = null;  
                        //        _context.Add(_oChanges);

                        //        //save everything
                        //        _context.SaveChanges();
                        //        logoutCurrUser = true ;
                        //    }

                        //    // add perms to new role (s) created.. SYS role - SUP_ADMN perm  :: lsPerms.Add(new UserPermission(0, null, "A0_00", "_A0_00__Super_Admin_Account", null, "A", null, null, null, null, null));   // for SYS account only

                        //    var userRole = _context.UserRole.Where(c => c.RoleName == "SYS" && c.RoleStatus == "A").FirstOrDefault();
                        //    var userPerm = _context.UserPermission.Where(c => c.PermissionCode == "A0_00" && c.PermStatus == "A").FirstOrDefault();
                        //    var _URPChanges = 0;
                        //    if (userRole != null && userPerm != null)
                        //    {
                        //        //for (var i = 0; i < permList.Count; i++)
                        //        //{
                        //        var checkURPExist = _context.UserRolePermission.Where(c => c.UserPermissionId == userPerm.Id && c.UserRoleId == userRole.Id && c.Status == "A").FirstOrDefault();
                        //        if (checkURPExist == null)
                        //        {
                        //            _context.Add(new UserRolePermission()
                        //            {
                        //                ChurchBodyId = null,
                        //                UserPermissionId = userPerm.Id,
                        //                UserRoleId = userRole.Id,
                        //                Status = "A",
                        //                ViewPerm = true,
                        //                CreatePerm = true,
                        //                EditPerm = true,
                        //                DeletePerm = true,
                        //                ManagePerm = true,
                        //                Created = tm,
                        //                LastMod = tm,
                        //                CreatedByUserId = null,
                        //                LastModByUserId = null
                        //            });

                        //            _URPChanges++;
                        //        }
                        //    }
                        //    //}

                        //    logoutCurrUser = _URPChanges > 0;
                        //    if (_URPChanges > 0) _context.SaveChanges();

                        //    // add role (s) for the new acc created.. SYS acc - SYS role - SYS permission : lsPerms.Add(new UserPermission(0, null, "A0_00", "_A0_00__Super_Admin_Account", null, "A", null, null, null, null, null));   // for SYS account only
                        //    var userProfile = _context.UserProfile.Where(c => c.Username.Trim().ToLower() == "SYS".Trim().ToLower() && c.UserStatus == "A").FirstOrDefault();
                        //    oUser_MSTR = userProfile;
                        //    //var userRole = _context.UserRole.Where(c => c.RoleName == "SYS" && c.RoleStatus == "A").FirstOrDefault();
                        //    var _UPRChanges = 0;
                        //    if (userRole != null && userProfile != null)
                        //    {
                        //        //for (var i = 0; i < permList.Count; i++)
                        //        //{
                        //        var checkUPRExist = _context.UserProfileRole.Where(c => c.UserProfileId == userProfile.Id && c.UserRoleId == userRole.Id && c.ProfileRoleStatus == "A").FirstOrDefault();
                        //        if (checkUPRExist == null)
                        //        {
                        //            _context.Add(new UserProfileRole()
                        //            {
                        //                ChurchBodyId = null,
                        //                UserProfileId = userProfile.Id,
                        //                UserRoleId = userRole.Id,
                        //                ProfileRoleStatus = "A",
                        //                Strt = tm,
                        //                Expr = null,
                        //                Created = tm,
                        //                LastMod = tm,
                        //                CreatedByUserId = null,
                        //                LastModByUserId = null
                        //            });

                        //            _UPRChanges++;
                        //        }
                        //    }
                        //    //}

                        //    logoutCurrUser = _UPRChanges > 0;
                        //    if (_UPRChanges > 0) _context.SaveChanges();


                        //    //logout ...login to authenticate
                        //    if (logoutCurrUser==true) return RedirectToAction("LoginUserAcc", "UserLogin");
                        //}

                        //catch (Exception ex)
                        //{
                        //    throw;
                        //} 
                    }
                     
                    // default users and roles done.... EXISTING USERS .... >>>>> GET the USER... account logged ---->>>>
                    else
                    {
                        try
                        {
                            //CHECK ACCOUNT EXIST, ACCOUNT ACTIVE      //authenticate app sys admins...  @vendor COMPULSORY -- admin users must confirm via Email or SMS                            
                            ///
                            var _ac1Ven = AppUtilties.ComputeSha256Hash(model.ChurchCode);
                            if (_ac1Ven == ac1) //AppUtilties.ComputeSha256Hash(model.ChurchCode) == ac1)
                            {
                                var cc = "000000";
                                string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(cc + model.Username.Trim().ToLower());
                                string strPwdHashedData = AppUtilties.ComputeSha256Hash(cc + model.Username.Trim().ToLower() + model.Password);                                
                                ///
                                oUser_MSTR = (from t_up in _context.UserProfile.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchBody)
                                                   .Where(c => c.ProfileScope == "V" && c.UserStatus == "A" &&
                                                   c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == strUserKeyHashedData && c.Pwd == strPwdHashedData)
                                             //from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                                         select t_up
                                                  ).FirstOrDefault();
                            }

                            //authenticate users... @client
                            else
                            {    
                                //string strPwdData = model.ChurchCode + model.Username.Trim().ToLower() + model.Password;
                                //string strPwdHashedData = AppUtilties.ComputeSha256Hash(strPwdData); // model.ChurchCode + model.Username.Trim().ToLower() + model.Password);
                                //string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower());

                                var cc = model.ChurchCode;  ///                               
                                string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(cc + model.Username.Trim().ToLower());
                                string strPwdHashedData = AppUtilties.ComputeSha256Hash(cc + model.Username.Trim().ToLower() + model.Password);

                                /// alternative user from vendor side --- client system assistant - admin
                                var _cc1CSA = "000000"; 
                                string _strUserKeyHashedData1CSA = AppUtilties.ComputeSha256Hash(_cc1CSA + model.Username.Trim().ToLower());
                                string _strPwdHashedData1CSA = AppUtilties.ComputeSha256Hash(_cc1CSA + model.Username.Trim().ToLower() + model.Password);

                                //string _strUserKeyHashedData = AppUtilties.ComputeSha256Hash("AGG000" + "ag-hqadmin".Trim().ToLower());
                                //string _strPwdData = "AGG000" + "ag-hqadmin".Trim().ToLower() + "E20CM86Q";
                                //string _strPwdHashedData = AppUtilties.ComputeSha256Hash(_strPwdData);  //string _strPwdHashedData = AppUtilties.ComputeSha256Hash(_strPwdData); // "PCG000" + "hqadmin".Trim().ToLower() + "U5K35I5X");

                                //var user = "ag-hqadmin".Trim().ToLower();
                                //var _user = model.Username.Trim().ToLower();

                                //var _compkey = _strUserKeyHashedData == strUserKeyHashedData; // "b3656ff0febfe2e6e6e55a6dde1587c60aab14f2a36e1b32af5b7e0f24a1d4b7";   
                                //var _comppwd = _strPwdHashedData == strPwdHashedData; // "96f394905990e390c835926e1575224a20b5cc4263de5f6539095522da1b4bed";
                                //var _compuser = _user == user; // " hqadmin".Trim().ToLower() == "hqadmin".Trim().ToLower();
                                //var _comp = _strPwdData == (cc + model.Username.Trim().ToLower() + model.Password);

                                ///
                                oUser_MSTR = (from t_up in _context.UserProfile.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchBody)
                                                   .Where(c => 
                                                           (c.ChurchBody.GlobalChurchCode == model.ChurchCode && c.ProfileScope == "C" && c.UserStatus == "A" && // (c.Expr == null || userAccExpr >= DateTime.Now.Date) &&
                                                            c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == strUserKeyHashedData && c.Pwd == strPwdHashedData) ||

                                                           (_ac1Ven != ac1 && c.ProfileScope == "V" && c.UserStatus == "A" &&
                                                            c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == _strUserKeyHashedData1CSA && c.Pwd == _strPwdHashedData1CSA))
                                             //from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                                                    select t_up
                                                  )
                                                  .FirstOrDefault();
                                 
                            }


                            //check for user....
                            if (oUser_MSTR == null)
                            {
                                ModelState.AddModelError("", "User credentials provided invalid");
                                model.IsVal = 0;
                                ///
                                return View(model);
                            }


                            // get the church slogan... from MSTR side
                            var _strSlogan = "";
                            if (AppUtilties.ComputeSha256Hash(model.ChurchCode) != ac1)
                            {                                
                                // Asomdwei nka wo|enka wo nso 
                                if (!string.IsNullOrEmpty(oUser_MSTR.AppGlobalOwner?.Slogan))
                                {
                                    _strSlogan = oUser_MSTR.AppGlobalOwner?.Slogan;   //
                                    if (_strSlogan.Contains("*|*"))
                                    {
                                        var _arrSlogan = _strSlogan.Split("*|*");
                                        _strSlogan = _arrSlogan.Length > 0 ? _arrSlogan[0] : _strSlogan; 
                                    }
                                } 
                            }

                            // var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan);
                            TempData["_strChurchSlogan"] = _strSlogan; TempData.Keep();
                            model.strChurchSlogan = _strSlogan;

                            //CHECK ... ACCOUNT EXPR 
                            var userAccExpr = oUser_MSTR.Expr != null ? oUser_MSTR.Expr.Value : oUser_MSTR.Expr;
                            userAccExpr = userAccExpr != null ? userAccExpr.Value : userAccExpr; //  oUser_MSTR.UserStatus != "A" || 
                            if (oUser_MSTR.UserStatus != "A" || (userAccExpr != null && userAccExpr < DateTime.Now.Date))
                            {
                                ModelState.AddModelError("", "User credentials provided invalid [account may be expired or not active]");
                                model.IsVal = 0;
                                ///
                                return View(model);
                            }
                             

                           //check if PWD-RESET OR PWD-EXPR
                           //check Pwd expiry / Acc expiry  --- RESET PWD
                           ///
                            var userPwdExpr = oUser_MSTR.Expr != null ? oUser_MSTR.PwdExpr.Value : oUser_MSTR.PwdExpr;
                            userPwdExpr = userPwdExpr != null ? userPwdExpr.Value : userPwdExpr;
                            ///
                            if (oUser_MSTR.ResetPwdOnNextLogOn == true || userPwdExpr < DateTime.Now.Date)
                            {//int userId = 0, int? oCurrChuBodyId = null, string profileScope = "C", int setIndex = 0

                                isUserValidated = false; model.IsVal = 0;
                                //if (oUser_MSTR != null)
                                //    oUserPrivilegeCol = AppUtilties.GetUserPrivilege(_context, model.ChurchCode, oUser_MSTR);

                                //var _privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                                //TempData["UserLogIn_oUserPrivCol"] = _privList; TempData.Keep(); 

                                //  return RedirectToAction("GetChangeUserPwdDetail", "UserLogin");

                                // public IActionResult AddOrEdit_ChangeUserPwd(string churchCode, string username, int setIndex) 
                                var routeValues = new RouteValueDictionary {
                                                          { "churchCode", model.ChurchCode },
                                                          { "username", model.Username },
                                                          { "setIndex", model.ChurchCode == "000000" ? 1 : 2 },
                                                          { "pageIndex", 2 }
                                                        };

                                return RedirectToAction("AddOrEdit_ChangeUserPwd", "UserLogin", routeValues);


                                //change pwd... afterward... sign out!
                                // public IActionResult AddOrEdit_UP_ChangePwd(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0,
                                // int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) 

                                //return RedirectToAction("AddOrEdit_UP_ChangePwd", "AppVenAdminController", new { oAppGloOwnId = (int?)null, oCurrChuBodyId = (int?)null, 
                                //                id = oUser_MSTR.Id, setIndex = 1, subSetIndex = 1, oAGOId_Logged = oUser_MSTR.AppGlobalOwnerId, oCBId_Logged = oUser_MSTR.ChurchBodyId, oUserId_Logged = oUser_MSTR.Id });


                                //return RedirectToAction("AddOrEdit_UP_ChangePwd", "UserProfile", new { userId = oUser_MSTR.Id, oCurrChuBodyId = oUser_MSTR.ChurchBodyId, profileScope = oUser_MSTR.ProfileScope, setIndex = 4 });

                                //return RedirectToAction( "Main", new RouteValueDictionary( new { controller = controllerName, action = "Main", Id = Id } ) );
                                //return RedirectToAction("Action", new { id = 99 });
                                // return RedirectToAction("AddOrEdit_UP_ChangePwd", "UserProfile", );
                            }
                             

                            // NO EXPIRY... ACC VALIDATED...
                            
                            if (oUser_MSTR != null)
                                oUserPrivilegeCol = AppUtilties.GetUserPrivilege(_context, model.ChurchCode, oUser_MSTR);

                            if (oUserPrivilegeCol == null)
                            {
                                ViewBag.ErrorMess = "User profile has no permissions assigned. Please contact System Administrator.";
                                ModelState.AddModelError("", "User profile has no permissions assigned. Please contact System Administrator");
                                return View(model);
                            }

                            if (oUserPrivilegeCol.UserSessionPermList.Count == 0)
                            {
                                ViewBag.ErrorMess = "User profile has no permissions assigned. Please contact System Administrator";
                                ModelState.AddModelError("", "User profile has no permissions assigned. Please contact System Administrator");
                                return View(model);
                            }

                            // check for min priv 
                            bool _bl_AppCurrUser_ModAccessMin = oUserPrivilegeCol.IsModAccessVAA0 || oUserPrivilegeCol.IsModAccessVAA4 || oUserPrivilegeCol.IsModAccessDS00 || oUserPrivilegeCol.IsModAccessAC01 || oUserPrivilegeCol.IsModAccessMR02 ||
                                                                oUserPrivilegeCol.IsModAccessCL03 || oUserPrivilegeCol.IsModAccessCA04 || oUserPrivilegeCol.IsModAccessFM05 || oUserPrivilegeCol.IsModAccessRA06;
                             
                            if (!_bl_AppCurrUser_ModAccessMin)
                            {
                                ViewBag.ErrorMess = "User profile has no permissions assigned. Please contact System Administrator";
                                ModelState.AddModelError("", "User profile has no permissions assigned. Please contact System Administrator");
                                return View(model);
                            }

                            // done ... we av user with priv
                            // isUserValidated = true; model.IsVal = 1;
                            var _privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                            TempData["UserLogIn_oUserPrivCol"] = _privList; TempData.Keep(); 
                            
                            //------------

                            //////AUTHENTICATE ACCOUNT PWD -- 
                            //////authenticate app sys admins...  @vendor COMPULSORY -- admin users must confirm via Email or SMS
                            ////var isUserPwdAuthenticated = false;
                            ////if (AppUtilties.ComputeSha256Hash(model.ChurchCode) == ac1) 
                            ////{
                            ////    var cc = "000000";
                            ////    string strPwdHashedData = AppUtilties.ComputeSha256Hash(cc + model.Username.Trim().ToLower() + model.Password);
                            ////    // string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(cc + model.Username.Trim().ToLower());

                            ////    isUserPwdAuthenticated = oUser_MSTR.Pwd == strPwdHashedData;

                            ////    //oUser_MSTR = (from t_up in _context.UserProfile  //.Include(t => t.ChurchBody)  //.Include(t => t.ChurchMember)
                            ////    //                   .Where(c =>  c.ProfileScope == "V" && c.UserStatus == "A" &&
                            ////    //                   c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == strUserKeyHashedData && c.Pwd == strPwdHashedData)
                            ////    //             //from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                            ////    //         select t_up
                            ////    //                  ).FirstOrDefault();
                            ////}

                            //////authenticate users... @client
                            ////else
                            ////{
                            ////    // string strPwdHashedData = AppUtilties.ComputeSha256Hash(model.Username.Trim().ToLower() + model.Password.Trim());
                            ////    string strPwdHashedData = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower() + model.Password);
                            ////    // string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username.Trim().ToLower());

                            ////    isUserPwdAuthenticated = oUser_MSTR.Pwd == strPwdHashedData;

                            ////    //oUser_MSTR = (from t_up in _context.UserProfile  //.Include(t => t.ChurchBody)  //.Include(t => t.ChurchMember)
                            ////    //                   .Where(c => c.ChurchBody.GlobalChurchCode == model.ChurchCode && c.ProfileScope == "C" && c.UserStatus == "A" &&  
                            ////    //                            c.Username.Trim().ToLower() == model.Username.Trim().ToLower() && c.UserKey == strUserKeyHashedData && c.Pwd == strPwdHashedData)
                            ////    //             //from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                            ////    //         select t_up
                            ////    //                  ).FirstOrDefault();
                            ////}


                            //////check for user....
                            ////if (oUser_MSTR == null || !isUserPwdAuthenticated)
                            ////{
                            ////    ModelState.AddModelError("", "User credentials [username or password] provided invalid"); 
                            ////    model.IsVal = 0;
                            ////    ///
                            ////    return View(model);
                            ////}


                            //if (oUser != null)
                            //{



                            // INITIALIZE... lookups! for one-time only
                            //check for user--member stuff 
                            ///
                            if (_ac1Ven == ac1 && oUser_MSTR.AppGlobalOwnerId == null && oUser_MSTR.ChurchBodyId == null && oUser_MSTR.ProfileScope == "V")   // vendor only
                            {
                                //// authenticated...
                                //isUserValidated = true; model.IsVal = 1;
                                //if (oUser_MSTR != null)
                                //    oUserPrivilegeCol = AppUtilties.GetUserPrivilege(_context, model.ChurchCode, oUser_MSTR);

                                //var _privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                                //TempData["UserLogIn_oUserPrivCol"] = _privList; TempData.Keep();


                                try
                                {
                                    if (!oUser_MSTR.IsMSTRInit)
                                    {
                                        // check... initialize the COUNTRY
                                        if (_context.MSTRCountry.AsNoTracking().Count() == 0)   // else UPDATE on-demand
                                        {
                                            var countriesList = AppUtilties.GetMS_BaseCountries();
                                            var _ctryNewCount = 0; var _ctryUpdCount = 0;
                                            if (countriesList.Count > 0)
                                            {
                                                //var tm = DateTime.Now;
                                                foreach (var oCtry in countriesList)
                                                {
                                                    var oCTRYExist = _context.MSTRCountry.AsNoTracking().Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code).FirstOrDefault();
                                                    if (oCTRYExist == null)
                                                    {
                                                        _context.Add(new MSTRCountry()
                                                        {
                                                            CtryAlpha3Code = oCtry.CtryAlpha3Code,  //key
                                                            EngName = oCtry.EngName,
                                                            CtryAlpha2Code = oCtry.CtryAlpha2Code,
                                                            CurrEngName = oCtry.CurrEngName,
                                                            CurrLocName = oCtry.CurrLocName,
                                                            CurrSymbol = oCtry.CurrSymbol,
                                                            Curr3LISOSymbol = oCtry.Curr3LISOSymbol,
                                                            //  SharingStatus = "N", 
                                                            Created = _tm,
                                                            LastMod = _tm,
                                                            CreatedByUserId = oUser_MSTR.Id,
                                                            LastModByUserId = oUser_MSTR.Id
                                                        });

                                                        _ctryNewCount++;
                                                    }
                                                    else
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
                                                        // oCTRY.CreatedByUserId = oUser_MSTR.Id;
                                                        oCTRYExist.LastModByUserId = oUser_MSTR.Id;

                                                        _context.MSTRCountry.Update(oCTRYExist);
                                                        _ctryUpdCount++;
                                                    }
                                                }
                                            }


                                            if ((_ctryNewCount + _ctryUpdCount) > 0)
                                            {
                                                _context.SaveChanges();
                                                ///
                                                _userTask = "Created/updated " + (_ctryNewCount + _ctryUpdCount) + " countries."; _tm = DateTime.Now;
                                                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                                                        "RCMS-Admin: Country", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, null, _tm, _tm, null, null));
                                            }
                                        }


                                        // update master db-user  
                                        oUser_MSTR.IsMSTRInit = true;
                                        oUser_MSTR.LastMod = _tm;
                                        // oUser_MSTR.LastModByUserId = oUser_MSTR.Id;
                                        _context.Update(oUser_MSTR);
                                        ///
                                        _context.SaveChanges();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", "Account validation failed. Vendor working space initialization failed. Please reload page to continue or contact System Admin"); //  Error: " + ex.Message);
                                    return View(model);
                                }
                                 
                            }

                            // clients only  ... load pre-data
                            else if (oUser_MSTR.AppGlobalOwnerId != null && oUser_MSTR.ChurchBodyId != null && oUser_MSTR.ProfileScope == "C")   
                            {

                                // false until proven true
                                // isUserValidated = false; model.IsVal = 0; 

                                try
                                {
                                    // Get the client database details.... db connection string                        
                                    var oClientConfig = _context.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                                    // ac1... if (oClientConfig == null) oClientConfig = _context.ClientAppServerConfig.AsNoTracking().Where(c => c. == oUser_MSTR.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
                                    ///
                                    if (oClientConfig == null)
                                    {
                                        ModelState.AddModelError("", "Client database details not found. Please try again or contact System Administrator"); //model.IsVal = 0; 
                                        return View(model);
                                    }


                                    //var _cs = ""; /// strTempConn;  /// , MSTR_DbContext currContext = null, string strTempConn = ""
                                    //if (string.IsNullOrEmpty(_cs))
                                    //    _cs = this._configuration.GetConnectionString("DefaultConnection");


                                    //// get and mod the conn
                                    //var _clientDBConnString = "";
                                    //var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
                                    //conn.DataSource = oClientConfig.ServerName; conn.InitialCatalog = oClientConfig.DbaseName;
                                    //conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
                                    ///// conn.IntegratedSecurity = false; 
                                    //conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;

                                    //_clientDBConnString = conn.ConnectionString;

                                    //// test the NEW DB conn 

                                    //var _clientContext = new ChurchModelContext(_clientDBConnString);


                                   /// var _clConn = this.GetCL_DBConnString(oUser_MSTR.AppGlobalOwnerId);
                                   /// 
                                   /// this._clientDBContext = this.GetClientDBContext(oUser_MSTR.AppGlobalOwnerId);
                                   /// 

                                    this._clientDBContext = AppUtilties.GetNewDBCtxConn_CL(_context, _configuration, oUser_MSTR.AppGlobalOwnerId);

                                    //var connstring = this._clientDBConn;
                                    //try
                                    //{
                                    //    var b = _clientContext.Database.CanConnect();
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    throw;
                                    //}

                                    if (!this._clientDBContext.Database.CanConnect())
                                    {
                                        ModelState.AddModelError("", "Client validation failed. Failed to connect client database. Please try again or contact System Administrator"); /// ven cn:- " + this._clientDBConn); //model.IsVal = 0;
                                        return View(model);
                                    }

                                    //if (this._clientDBContext.Database.CanConnect())
                                    //{
                                    //    ModelState.AddModelError("", "Client validation success. Connected to client database. ven err cn:- " + this._clientDBConn); //model.IsVal = 0;
                                    //    return View(model);
                                    //}



                                    //// verify AGO, CB, CL from client database of the user...
                                    //var oCLAGO_MSTRRef = _clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && c.Status=="A").FirstOrDefault();
                                    //if (oCLAGO_MSTRRef == null)
                                    //{
                                    //    ModelState.AddModelError("", "Subscribed church unit of user could not be verified. Please contact System Administrator."); // model.IsVal = 0;
                                    //    return View(model);  // or update...
                                    //}

                                    if (oUser_MSTR.IsChurchMember && oUser_MSTR.ChurchMemberId == null)
                                    {
                                        ModelState.AddModelError("", "User profile has no Church member attached yet. Member profile required to trace some peculiar user activities. Please enter correct login credentials."); // model.IsVal = 0;
                                        return View(model);
                                    }

                                    else if (oUser_MSTR.IsChurchMember && oUser_MSTR.ChurchMemberId != null)
                                    {   //making sure user is active member of the church  ... might not be compulsory anyway... cos church may employ persons from other faiths
                                        var chkUserMemExist = (
                                            from t_ms in this._clientDBContext.MemberStatus.AsNoTracking().Where(c => c.ChurchBody.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && 
                                                            c.ChurchBody.MSTR_ChurchBodyId == oUser_MSTR.ChurchBodyId && c.ChurchBody.GlobalChurchCode == model.ChurchCode && 
                                                                c.IsCurrent == true && c.ChurchMemberId == oUser_MSTR.ChurchMemberId)
                                            select t_ms
                                                     ).FirstOrDefault();
                                        ///
                                        if (chkUserMemExist == null)
                                        {
                                            ModelState.AddModelError("", "Church membership of user could not be verified [inactive or unavailable] in congregation specified. Please enter correct login credentials.");   // model.IsVal = 0; // isUserValidated = false;
                                            return View(model);
                                        }
                                    }


                                    // not new... thus already initialized.... CBs created
                                    //if (oUser_MSTR.IsCLNTInit)
                                    //{
                                        var _clientCB = this._clientDBContext.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ParentChurchBody)
                                                            .Where(c => 
                                                                (c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId &&
                                                                 c.MSTR_ChurchBodyId == oUser_MSTR.ChurchBodyId && c.GlobalChurchCode == model.ChurchCode) 
                                                                 ).FirstOrDefault();

                                        // if @vendor... ForceSyncClientInfo == true ... set oUser_MSTR.IsCLNTInit = 0 ... so that sync kicks!
                                        if (_clientCB == null)
                                        {
                                            ModelState.AddModelError("", "User profile's church body could not be verified. Please enter correct login credentials.");   // model.IsVal = 0; // isUserValidated = false;
                                            return View(model);
                                        }

                                        // if @vendor... ForceSyncClientInfo == true ... set oUser_MSTR.IsCLNTInit = 0 ... so that sync kicks!
                                        if (_clientCB.AppGlobalOwner == null)
                                        {
                                            ModelState.AddModelError("", "User profile's church (denomination) could not be verified. Please enter correct login credentials.");   // model.IsVal = 0; // isUserValidated = false;
                                            return View(model);
                                        }


                                        // save the client db
                                        oUserPrivilegeCol.AppGlobalOwner_CLNT = _clientCB.AppGlobalOwner;
                                        oUserPrivilegeCol.ChurchBody_CLNT = _clientCB;
                                        var _privList_CLNT = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                                        TempData["UserLogIn_oUserPrivCol"] = _privList_CLNT; TempData.Keep();


                                        //// Asomdwei nka wo|enka wo nso
                                        //var _strSlogan_CL = _clientCB.AppGlobalOwner?.Slogan;
                                        //var _arrSlogan_CL = _strSlogan_CL.Split("|");
                                        //_strSlogan_CL = _arrSlogan_CL.Length > 0 ? _arrSlogan_CL[0] : _strSlogan_CL;
                                        //// var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan); 
                                        //TempData["_strChurchSlogan"] = _strSlogan_CL; TempData.Keep();


                                        //// get the church slogan... from CLIENT side
                                        var _strSlogan_CL = "";
                                        // get the church slogan... from MSTR side
                                        // Asomdwei nka wo|enka wo nso 
                                        if (!string.IsNullOrEmpty(_clientCB.AppGlobalOwner?.Slogan))
                                        {
                                            _strSlogan_CL = _clientCB.AppGlobalOwner?.Slogan;   //
                                            if (_strSlogan_CL.Contains("*|*"))
                                            {
                                                var _arrSlogan_CL = _strSlogan_CL.Split("*|*");
                                                _strSlogan_CL = _arrSlogan_CL.Length > 0 ? _arrSlogan_CL[0] : _strSlogan_CL;
                                            }
                                        }


                                        // var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan);
                                        TempData["_strChurchSlogan"] = _strSlogan_CL; TempData.Keep();
                                        model.strChurchSlogan = _strSlogan_CL;
                                    //}
                                     
                                      


                                    ///// CL INIT. moved to vendor tasks.....

                                    //// initialize....  lookups! once--- per user account
                                    //if (!oUser_MSTR.IsCLNTInit && (oUser_MSTR.ProfileLevel == 6 || oUser_MSTR.ProfileLevel == 11))  // only admin profiles allowed to update params
                                    //{
                                    //    try
                                    //    {   /// synchronize CTRY, AGO, CL, CB...  from MSTR to CLIENT
                                    //        /// 
                                    //        // initialize the COUNTRY  ... jux the country list standard countries ::: Use [CountryCustom] to config per denomination
                                    //        var _addCount = 0; var _updCount = 0; var tm = DateTime.Now; var strDesc = "Country";
                                    //        var countriesList = AppUtilties.GetMS_BaseCountries();
                                    //        var oCTRYCount = _clientContext.Country.AsNoTracking().Count();
                                    //        var oCtryAddList = new List<Country>();
                                    //        ///
                                    //        if (oCTRYCount != countriesList.Count() && countriesList.Count > 0)
                                    //        {
                                    //            foreach (var oCtry in countriesList)
                                    //            {
                                    //                var oCTRYExist = _clientContext.Country.AsNoTracking().Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code).FirstOrDefault();
                                    //                if (oCTRYExist == null)
                                    //                {
                                    //                    //var checkCtryAddedList = oCtryAddList.Where(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code).ToList();
                                    //                    var checkCtryAdded = oCtryAddList.Count(c => c.CtryAlpha3Code == oCtry.CtryAlpha3Code) > 0; // checkCtryAddedList.Count() > 0; // oCtryAddList.Count(c => c.CtryAlpha3Code == oCTRY.CtryAlpha3Code) == 0;
                                    //                    if (!checkCtryAdded)
                                    //                    {
                                    //                        var oNewCtry = new Country()
                                    //                        {
                                    //                            CtryAlpha3Code = oCtry.CtryAlpha3Code,
                                    //                            //AppGlobalOwnerId = oAGO.Id,
                                    //                            // ChurchBodyId = 
                                    //                            EngName = oCtry.EngName,
                                    //                            CtryAlpha2Code = oCtry.CtryAlpha2Code,
                                    //                            CurrEngName = oCtry.CurrEngName,
                                    //                            CurrLocName = oCtry.CurrLocName,
                                    //                            CurrSymbol = oCtry.CurrSymbol,
                                    //                            Curr3LISOSymbol = oCtry.Curr3LISOSymbol,
                                    //                            // SharingStatus = "N",
                                    //                            Created = tm,
                                    //                            LastMod = tm,
                                    //                            CreatedByUserId = oUser_MSTR.Id,
                                    //                            LastModByUserId = oUser_MSTR.Id
                                    //                        };

                                    //                        _clientContext.Add(oNewCtry);
                                    //                        _updCount++;

                                    //                        oCtryAddList.Add(oNewCtry);
                                    //                    }
                                    //                    //else
                                    //                    //{
                                    //                    //    checkCtryAdded = checkCtryAdded;
                                    //                    //    checkCtryAddedList = checkCtryAddedList;
                                    //                    //}
                                    //                }
                                    //                else  // update country data
                                    //                {
                                    //                    oCTRYExist.CtryAlpha3Code = oCtry.CtryAlpha3Code;
                                    //                    oCTRYExist.EngName = oCtry.EngName;
                                    //                    oCTRYExist.CtryAlpha2Code = oCtry.CtryAlpha2Code;
                                    //                    oCTRYExist.CurrEngName = oCtry.CurrEngName;
                                    //                    oCTRYExist.CurrLocName = oCtry.CurrLocName;
                                    //                    oCTRYExist.CurrSymbol = oCtry.CurrSymbol;
                                    //                    oCTRYExist.Curr3LISOSymbol = oCtry.Curr3LISOSymbol;
                                    //                    //oCTRY. SharingStatus = "N";
                                    //                    // oCTRY.Created = tm;
                                    //                    oCTRYExist.LastMod = tm;
                                    //                    // oCTRY.CreatedByUserId = oUser_MSTR.Id;
                                    //                    oCTRYExist.LastModByUserId = oUser_MSTR.Id;

                                    //                    _clientContext.Update(oCTRYExist);
                                    //                    _updCount++;
                                    //                }
                                    //            }

                                    //            if (_updCount > 0)
                                    //            {
                                    //                 _clientContext.SaveChanges();

                                    //                ///
                                    //                /////update country of oAGO
                                    //                //if (oAGO_MSTR != null)
                                    //                //{
                                    //                //    oAGO.CtryAlpha3Code = oAGO_MSTR.CtryAlpha3Code;
                                    //                //    oAGO.LastMod = tm; oAGO.LastModByUserId = oUser_MSTR.Id;
                                    //                //    _clientContext.Update(oAGO);

                                    //                //    /// save updated...
                                    //                //    _clientContext.SaveChanges();
                                    //                //}

                                    //                /// update user trail
                                    //                _userTask = "Created/updated " + _updCount + " countries."; _tm = DateTime.Now;
                                    //                // record ... @client 
                                    //                _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, oUser_MSTR.AppGlobalOwnerId, oUser_MSTR.ChurchBodyId, "T",
                                    //                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUser_MSTR.Id, _tm, _tm, oUser_MSTR.Id, oUser_MSTR.Id)
                                    //                                , _clientDBConnString);

                                    //                // record @ MSTR

                                    //            }
                                    //        }


                                    //        // initialize the AGO    ...var oClientAGOListCount = _clientContext.AppGlobalOwner.Count();
                                    //        strDesc = "Denomination (Church)"; tm = DateTime.Now; //  _updCount = 0;
                                    //        var oAGO_MSTR = _context.MSTRAppGlobalOwner.AsNoTracking().Include(t => t.FaithTypeCategory).ThenInclude(t => t.FaithTypeClass)
                                    //                            .Where(c => c.Id == oUser_MSTR.AppGlobalOwnerId).FirstOrDefault();   //&& c.GlobalChurchCode==oUser_MSTR.strChurchCode_AGO

                                    //        if (oAGO_MSTR == null)
                                    //        {
                                    //            ModelState.AddModelError("", "Denomination (Church) of user could not be verified [by Vendor]. Please enter correct login credentials.");   // model.IsVal = 0; // isUserValidated = false;
                                    //            return View(model);
                                    //        }

                                    //        //COPY/create THE DENOMINATION / CONTACT INFO FROM MSTR    //  || c.GlobalChurchCode == oAGO_MSTR.GlobalChurchCode
                                    //        var oAGO_CLNT = _clientContext.AppGlobalOwner.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == oAGO_MSTR.Id).FirstOrDefault();  //)   // || c.OwnerName == oAGO_MSTR.OwnerName
                                    //        if (oAGO_CLNT == null)  // create AGO and CI
                                    //        {
                                    //            var oAGONew = new AppGlobalOwner()
                                    //            {
                                    //                //Id = 0,
                                    //                MSTR_AppGlobalOwnerId = oAGO_MSTR.Id,
                                    //                OwnerName = oAGO_MSTR.OwnerName,
                                    //                GlobalChurchCode = oAGO_MSTR.GlobalChurchCode,
                                    //                RootChurchCode = oAGO_MSTR.GlobalChurchCode,
                                    //                TotalLevels = oAGO_MSTR.TotalLevels,
                                    //                Acronym = oAGO_MSTR.Acronym,
                                    //                PrefixKey = oAGO_MSTR.PrefixKey,
                                    //                Allias = oAGO_MSTR.Allias,
                                    //                Motto = oAGO_MSTR.Motto,
                                    //                Slogan = oAGO_MSTR.Slogan,
                                    //                ChurchLogo = oAGO_MSTR.ChurchLogo,
                                    //                Status = oAGO_MSTR.Status,
                                    //                Comments = oAGO_MSTR.Comments,
                                    //                // CountryId = 0,
                                    //                CtryAlpha3Code = oAGO_MSTR.CtryAlpha3Code,
                                    //                strFaithTypeCategory = oAGO_MSTR.FaithTypeCategory != null ? oAGO_MSTR.FaithTypeCategory.FaithDescription : "",
                                    //                strFaithStream = oAGO_MSTR.FaithTypeCategory != null ? (oAGO_MSTR.FaithTypeCategory.FaithTypeClass != null ? oAGO_MSTR.FaithTypeCategory.FaithTypeClass.FaithDescription : "") : "",
                                    //                // FaithTypeCategoryId = oAGO_MSTR.FaithTypeCategoryId, // jux keep the Id... get the [strFaithTypeCategory, strFaithTypeStream] ...from MSTR @queries                                         
                                    //                //  ContactInfoId = oCI != null ? oCI.Id : (int?)null, // copy details and create this to the local CI                                            
                                    //                ///
                                    //                Created = tm,
                                    //                LastMod = tm,
                                    //                CreatedByUserId = oUser_MSTR.Id,
                                    //                LastModByUserId = oUser_MSTR.Id
                                    //            };

                                    //            _clientContext.Add(oAGONew);

                                    //            // _updCount++;


                                    //            //if (_updCount > 0)
                                    //            //{

                                    //            _clientContext.SaveChanges();
                                    //            oAGO_CLNT = oAGONew;


                                    //            // check for the CI from MSTR...
                                    //            var oCI_MSTR = _context.MSTRContactInfo.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_MSTR.Id && c.Id == oAGO_MSTR.ContactInfoId).FirstOrDefault();
                                    //            ContactInfo oCI = null;
                                    //            if (oCI_MSTR != null)
                                    //            {
                                    //                oCI = new ContactInfo()
                                    //                {
                                    //                    //Id = 0,
                                    //                    AppGlobalOwnerId = oAGONew.Id,

                                    //                    // ChurchBodyId = oCB_MSTR.Id,
                                    //                    // RefUserId = oCI_MSTR.RefUserId,
                                    //                    //ContactInfoDesc

                                    //                    ExtHolderName = oCI_MSTR.ContactName,
                                    //                    IsPrimaryContact = true,
                                    //                    //IsChurchFellow = false,
                                    //                    ResidenceAddress = oCI_MSTR.ResidenceAddress,
                                    //                    Location = oCI_MSTR.Location,
                                    //                    City = oCI_MSTR.City,
                                    //                    CtryAlpha3Code = oCI_MSTR.CtryAlpha3Code,
                                    //                    //RegionId = oCI_MSTR.RegionId,
                                    //                    ResAddrSameAsPostAddr = oCI_MSTR.ResAddrSameAsPostAddr,
                                    //                    PostalAddress = oCI_MSTR.PostalAddress,
                                    //                    DigitalAddress = oCI_MSTR.DigitalAddress,
                                    //                    Telephone = oCI_MSTR.Telephone,
                                    //                    MobilePhone1 = oCI_MSTR.MobilePhone1,
                                    //                    MobilePhone2 = oCI_MSTR.MobilePhone2,
                                    //                    Email = oCI_MSTR.Email,
                                    //                    Website = oCI_MSTR.Website,
                                    //                    ///
                                    //                    Created = tm,
                                    //                    LastMod = tm,
                                    //                    CreatedByUserId = oUser_MSTR.Id,
                                    //                    LastModByUserId = oUser_MSTR.Id
                                    //                };

                                    //                _clientContext.Add(oCI);

                                    //                //update firsst... to det Id
                                    //                _clientContext.SaveChanges();
                                    //            }


                                    //            // do some update here...
                                    //            if (oCI != null)
                                    //            {
                                    //                oAGONew.ContactInfoId = oCI.Id;
                                    //                oAGONew.LastMod = tm; oCI.LastModByUserId = oUser_MSTR.Id;
                                    //                ///
                                    //                _clientContext.Update(oAGONew);
                                    //                _clientContext.SaveChanges();
                                    //            }

                                    //            // record ... @client
                                    //            _userTask = "Created " + _updCount + " " + strDesc.ToLower(); _tm = DateTime.Now;

                                    //            _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, oUser_MSTR.AppGlobalOwnerId, oUser_MSTR.ChurchBodyId, "T",
                                    //                                 "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUser_MSTR.Id, _tm, _tm, oUser_MSTR.Id, oUser_MSTR.Id)
                                    //                                , _clientDBConnString);
                                    //            //  }
                                    //        }


                                    //        /// some client UPDATE /sync! ...  check the localized data... using the MSTR data
                                    //        else  // update AGO
                                    //        {
                                    //            if (oAGO_CLNT.MSTR_AppGlobalOwnerId != oAGO_MSTR.Id || string.Compare(oAGO_CLNT.GlobalChurchCode, oAGO_MSTR.GlobalChurchCode, true) != 0 ||
                                    //                string.IsNullOrEmpty(oAGO_CLNT.OwnerName) || string.IsNullOrEmpty(oAGO_CLNT.strFaithTypeCategory) || string.IsNullOrEmpty(oAGO_CLNT.strFaithStream))
                                    //            {
                                    //                //var oAGO_MSTR = _context.MSTRAppGlobalOwner.AsNoTracking().Include(t => t.FaithTypeCategory).ThenInclude(t => t.FaithTypeClass)
                                    //                //            .Where(c => c.Id == oUser_MSTR.AppGlobalOwnerId).FirstOrDefault();

                                    //                if (oAGO_CLNT.MSTR_AppGlobalOwnerId == null)
                                    //                    oAGO_CLNT.MSTR_AppGlobalOwnerId = oAGO_MSTR.Id;

                                    //                if (string.IsNullOrEmpty(oAGO_CLNT.GlobalChurchCode) || oAGO_CLNT.GlobalChurchCode != oAGO_MSTR.GlobalChurchCode)
                                    //                    oAGO_CLNT.GlobalChurchCode = oAGO_MSTR.GlobalChurchCode;

                                    //                if (string.IsNullOrEmpty(oAGO_CLNT.OwnerName) || oAGO_CLNT.OwnerName != oAGO_MSTR.OwnerName)
                                    //                    oAGO_CLNT.OwnerName = oAGO_MSTR.OwnerName;

                                    //                if (string.IsNullOrEmpty(oAGO_CLNT.strFaithTypeCategory))
                                    //                    oAGO_CLNT.strFaithTypeCategory = oAGO_MSTR.FaithTypeCategory != null ? oAGO_MSTR.FaithTypeCategory.FaithDescription : "";

                                    //                if (string.IsNullOrEmpty(oAGO_CLNT.strFaithStream))
                                    //                    oAGO_CLNT.strFaithStream = oAGO_MSTR.FaithTypeCategory != null ? (oAGO_MSTR.FaithTypeCategory.FaithTypeClass != null ? oAGO_MSTR.FaithTypeCategory.FaithTypeClass.FaithDescription : "") : "";

                                    //                _clientContext.Update(oAGO_CLNT);
                                    //                _clientContext.SaveChanges();

                                    //                ViewBag.UserMsg = strDesc + " updated successfully.";
                                    //                ///
                                    //                _userTask = "Updated " + strDesc.ToLower() + ", " + oAGO_CLNT.OwnerName.ToUpper() + " successfully";
                                    //                _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, oUser_MSTR.AppGlobalOwnerId, oUser_MSTR.ChurchBodyId, "T",
                                    //                             "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUser_MSTR.Id, _tm, _tm, oUser_MSTR.Id, oUser_MSTR.Id)
                                    //                            , _clientDBConnString);
                                    //            }
                                    //        }


                                    //        // Get the denomination/church || c.GlobalChurchCode == oAGO_MSTR.GlobalChurchCode
                                    //        //var oAGO = _clientContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId).FirstOrDefault();                                 
                                    //        // oAGO_CLNT... use last created /updated

                                    //        // initialize the CL                                
                                    //        var oCL_MSTRList = _context.MSTRChurchLevel.AsNoTracking().Where(c => c.AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId);
                                    //        var oCL_CLNTList = _clientContext.ChurchLevel.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId);
                                    //        ///
                                    //       // if (oCL_CLNTList.Count() != oCL_MSTRList.Count())
                                    //       // {
                                    //            strDesc = "Church Level";
                                    //            _addCount = 0; _updCount = 0; tm = DateTime.Now;
                                    //            if (oCL_MSTRList.Count() > 0 && oAGO_CLNT != null)
                                    //            {
                                    //                foreach (var oCL_MSTR in oCL_MSTRList)
                                    //                {
                                    //                    var oCLExist = oCL_CLNTList.Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId &&
                                    //                                       (c.Name.ToLower() == oCL_MSTR.Name.ToLower() || c.CustomName.ToLower() == oCL_MSTR.CustomName.ToLower())).FirstOrDefault();
                                    //                    if (oCLExist == null)
                                    //                    {
                                    //                        _clientContext.Add(new ChurchLevel()
                                    //                        {
                                    //                            //Id = 0,
                                    //                            MSTR_AppGlobalOwnerId = oCL_MSTR.AppGlobalOwnerId,
                                    //                            MSTR_ChurchLevelId = oCL_MSTR.Id,
                                    //                            ///
                                    //                            AppGlobalOwnerId = oAGO_CLNT.Id,
                                    //                            Name = oCL_MSTR.Name,
                                    //                            CustomName = oCL_MSTR.CustomName,
                                    //                            LevelIndex = oCL_MSTR.LevelIndex,
                                    //                            Acronym = oCL_MSTR.Acronym,
                                    //                            SharingStatus = oCL_MSTR.SharingStatus,
                                    //                            ///
                                    //                            Created = tm,
                                    //                            LastMod = tm,
                                    //                            CreatedByUserId = oUser_MSTR.Id,
                                    //                            LastModByUserId = oUser_MSTR.Id
                                    //                        });

                                    //                        _addCount++;
                                    //                    }
                                    //                    else if (oCLExist.MSTR_AppGlobalOwnerId != oCL_MSTR.AppGlobalOwnerId || oCLExist.MSTR_ChurchLevelId != oCL_MSTR.Id ||
                                    //                             oCLExist.LevelIndex != oCL_MSTR.LevelIndex || string.Compare(oCLExist.Name, oCL_MSTR.Name, true) != 0)
                                    //                    {
                                    //                        oCLExist.MSTR_AppGlobalOwnerId = oCL_MSTR.AppGlobalOwnerId;
                                    //                        oCLExist.MSTR_ChurchLevelId = oCL_MSTR.Id;
                                    //                        oCLExist.Name = oCL_MSTR.Name;
                                    //                        oCLExist.LevelIndex = oCL_MSTR.LevelIndex;
                                    //                        oCLExist.LastMod = tm;
                                    //                        oCLExist.LastModByUserId = oUser_MSTR.Id;

                                    //                        _clientContext.Update(oCLExist);
                                    //                        _updCount++;
                                    //                    }
                                    //                }


                                    //                if ((_addCount + _updCount) > 0)
                                    //                {
                                    //                    _clientContext.SaveChanges();
                                    //                    ///
                                    //                    // record ... @client
                                    //                    _userTask = "Created " + _updCount + " " + strDesc.ToLower(); _tm = DateTime.Now;
                                    //                    _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, oUser_MSTR.AppGlobalOwnerId, oUser_MSTR.ChurchBodyId, "T",
                                    //                                     "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUser_MSTR.Id, _tm, _tm, oUser_MSTR.Id, oUser_MSTR.Id)
                                    //                                    , _clientDBConnString);
                                    //                }
                                    //            }
                                    //       // }

                                    //        // include the root to the top for the subscriber... but make them STructure only until logged in [thus have accounts created by vendor]
                                    //        // initialize the CB  ... ONLY create the CB that subscribed even in the same Denomination [ 1 CB at a time ]                       
                                    //        //var oAGO = _clientContext.AppGlobalOwner.Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId || c.GlobalChurchCode == oAGO_MSTR.GlobalChurchCode).FirstOrDefault();
                                    //        ///
                                    //        var oUserCB_MSTR = _context.MSTRChurchBody.AsNoTracking()  //.Include(t => t.FaithTypeCategory).ThenInclude(t => t.FaithTypeClass)
                                    //                             .Where(c => c.AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && c.Id == oUser_MSTR.ChurchBodyId).FirstOrDefault();
                                    //        if (oUserCB_MSTR == null)
                                    //        {
                                    //            ModelState.AddModelError("", "Subscribed church body (unit) of user could not be verified [by Vendor]. Please enter correct login credentials.");   // model.IsVal = 0; // isUserValidated = false;
                                    //            return View(model);
                                    //        }

                                    //        //**********************************
                                    //        // for single subscription... until user logs in, CB is not created yet on Client server/DB............. // use subscription key /shared subscriptio keys for Multiple subsciption sync
                                    //        //var oCB_MSTRList = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && c.Id == oUserCB_MSTR.Id).ToList(); 
                                    //        ///
                                    //        var oCB_MSTRList = _context.MSTRChurchBody.AsNoTracking()
                                    //            .Where(c => c.AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && oUserCB_MSTR.RootChurchCode.Contains(c.GlobalChurchCode)).ToList();
                                    //        //var oCBClientListCount = _clientContext.ChurchBody.AsNoTracking().Count(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && c.MSTR_ChurchBodyId == oUserCB_MSTR.Id);
                                    //        var oCB_CLNTList = _clientContext.ChurchBody.AsNoTracking()
                                    //            .Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && oUserCB_MSTR.RootChurchCode.Contains(c.GlobalChurchCode)).ToList();

                                    //        strDesc = "Church body (unit)";

                                    //        ///                                
                                    //       // if (oCB_CLNTList.Count() != oCB_MSTRList.Count())
                                    //       // {
                                    //            _addCount = 0; _updCount = 0; tm = DateTime.Now;
                                    //            if (oCB_MSTRList.Count() > 0 && oAGO_CLNT != null)
                                    //            {
                                    //                foreach (var oCB_MSTR in oCB_MSTRList)
                                    //                {
                                    //                    //var oCB_CLNTExist = _clientContext.ChurchBody.Where(c => (c.OrgType == "CH" || c.OrgType == "CN") && c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && 
                                    //                    //               (c.MSTR_ChurchBodyId == oUser_MSTR.ChurchBodyId || c.GlobalChurchCode == oCB_MSTR.GlobalChurchCode)).FirstOrDefault();

                                    //                    // create all CBs not found in the root path of MSTR path ... at the client side.
                                    //                    var oCB_CLNTExist = oCB_CLNTList.Where(c => (c.OrgType == "CR" || c.OrgType == "CH" || c.OrgType == "CN") &&
                                    //                                (c.MSTR_ChurchBodyId == oCB_MSTR.Id || c.GlobalChurchCode == oCB_MSTR.GlobalChurchCode)).FirstOrDefault();

                                    //                    if (oCB_CLNTExist == null)
                                    //                    {
                                    //                        // Get Church level
                                    //                        //  ChurchBody oCB_CLNTAdd = null;
                                    //                        var oCL = _clientContext.ChurchLevel.AsNoTracking().Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && c.MSTR_ChurchLevelId == oCB_MSTR.ChurchLevelId).FirstOrDefault();
                                    //                        if (oCL != null)
                                    //                        {
                                    //                            var oCB_CLNTAdd = new ChurchBody()
                                    //                            {
                                    //                                //Id = 0,
                                    //                                MSTR_AppGlobalOwnerId = oCB_MSTR.AppGlobalOwnerId,
                                    //                                MSTR_ChurchBodyId = oCB_MSTR.Id,
                                    //                                MSTR_ParentChurchBodyId = oCB_MSTR.ParentChurchBodyId,
                                    //                                MSTR_ChurchLevelId = oCB_MSTR.ChurchLevelId,     // cannot change for CH, CN types
                                    //                                ///
                                    //                                AppGlobalOwnerId = oAGO_CLNT.Id,
                                    //                                ChurchLevelId = oCL.Id,
                                    //                                Name = oCB_MSTR.Name,
                                    //                                IsFullAutonomy = true,
                                    //                                ChurchWorkStatus = oCB_MSTR.Id == oUser_MSTR.ChurchBodyId ? "OP" : "ST",
                                    //                                Status = oCB_MSTR.Id == oUser_MSTR.ChurchBodyId ? oCB_MSTR.Status : "P",  // P-Pending activation from vendor  //oCB_MSTR.Status,
                                    //                                IsSupervisedByParentBody = true,
                                    //                                ///
                                    //                                //Acronym = null, 
                                    //                                //BriefHistory = null, 
                                    //                                //ChurchBodyLogo = null, 
                                    //                                //ChurchCodeCustom = null, 
                                    //                                //CountryRegionId = (int?)null,  
                                    //                                //SupervisedByChurchBodyId = (int?)null,   // Ex. Preaching Points are typically under the supervision of other congregations
                                    //                                //DateFormed = (DateTime?)null, 
                                    //                                //DateInnaug = (DateTime?)null,       

                                    //                                //ParentChurchBodyId = (int?)null,   // get the parent code... via master parent // ParentChurchBodyId = null,  // update after first batch...   ***
                                    //                                ///
                                    //                                GlobalChurchCode = oCB_MSTR.GlobalChurchCode,
                                    //                                MSTR_RootChurchCode = oCB_MSTR.RootChurchCode,  // ONLY Vendor to change
                                    //                                RootChurchCode = oCB_MSTR.RootChurchCode,       // Client Admin may change but MUST be symmetrical to the Vendors. Ex. Grace cong must continue to be in the root path of Ga Presbytery unless Vendor so determines... tho client may alter [unaffected paths]
                                    //                                OrgType = oCB_MSTR.OrgType,  // cannot change for CR, CH, CN types
                                    //                                SubscriptionKey = oCB_MSTR.SubscriptionKey,
                                    //                                CtryAlpha3Code = oCB_MSTR.CtryAlpha3Code,  // country GHA, USA, GBR 
                                    //                                ///    
                                    //                                //  ContactInfoId = oCI != null ? oCI.Id : (int?)null,  // create from the MSTR CI data-values ***
                                    //                                Comments = oCB_MSTR.Comments,

                                    //                                ///
                                    //                                Created = tm,
                                    //                                LastMod = tm,
                                    //                                CreatedByUserId = oUser_MSTR.Id,
                                    //                                LastModByUserId = oUser_MSTR.Id
                                    //                            };

                                    //                            _clientContext.Add(oCB_CLNTAdd);
                                    //                            _clientContext.SaveChanges();
                                    //                            _addCount++;


                                    //                            ///
                                    //                            var oCI_MSTR = _context.MSTRContactInfo.AsNoTracking().Where(c => c.AppGlobalOwnerId == oCB_MSTR.AppGlobalOwnerId && c.ChurchBodyId == oCB_MSTR.Id && c.Id == oCB_MSTR.ContactInfoId).FirstOrDefault();
                                    //                            //  ContactInfo oCI = null;
                                    //                            if (oCI_MSTR != null)
                                    //                            {
                                    //                                var oCI = new ContactInfo()
                                    //                                {
                                    //                                    //Id = 0,
                                    //                                    AppGlobalOwnerId = oCB_CLNTAdd.AppGlobalOwnerId,
                                    //                                    ChurchBodyId = oCB_CLNTAdd.Id,
                                    //                                    //RefUserId = oCI_MSTR.RefUserId,
                                    //                                    ExtHolderName = oCI_MSTR.ContactName,
                                    //                                    //ChurchMemberId = null
                                    //                                    IsPrimaryContact = true,
                                    //                                    // ContactInfoDesc = null,
                                    //                                    // IsChurchFellow = false,
                                    //                                    ResidenceAddress = oCI_MSTR.ResidenceAddress,
                                    //                                    Location = oCI_MSTR.Location,
                                    //                                    City = oCI_MSTR.City,
                                    //                                    CtryAlpha3Code = oCI_MSTR.CtryAlpha3Code,
                                    //                                    //RegionId = oCI_MSTR.RegionId,
                                    //                                    ResAddrSameAsPostAddr = oCI_MSTR.ResAddrSameAsPostAddr,
                                    //                                    PostalAddress = oCI_MSTR.PostalAddress,
                                    //                                    DigitalAddress = oCI_MSTR.DigitalAddress,
                                    //                                    Telephone = oCI_MSTR.Telephone,
                                    //                                    MobilePhone1 = oCI_MSTR.MobilePhone1,
                                    //                                    MobilePhone2 = oCI_MSTR.MobilePhone2,
                                    //                                    Email = oCI_MSTR.Email,
                                    //                                    Website = oCI_MSTR.Website,
                                    //                                    ///
                                    //                                    Created = tm,
                                    //                                    LastMod = tm,
                                    //                                    CreatedByUserId = oUser_MSTR.Id,
                                    //                                    LastModByUserId = oUser_MSTR.Id
                                    //                                };

                                    //                                _clientContext.Add(oCI);
                                    //                                _clientContext.SaveChanges();

                                    //                                // update CB
                                    //                                oCB_CLNTAdd.ContactInfoId = oCI.Id;
                                    //                                oCB_CLNTAdd.LastMod = tm;
                                    //                                oCB_CLNTAdd.LastModByUserId = oUser_MSTR.Id;

                                    //                                _clientContext.Update(oCI);
                                    //                                _clientContext.SaveChanges();
                                    //                            }
                                    //                        }
                                    //                    }

                                    //                    else if (oCB_CLNTExist.MSTR_AppGlobalOwnerId != oCB_MSTR.AppGlobalOwnerId || oCB_CLNTExist.MSTR_ChurchLevelId != oCB_MSTR.ChurchLevelId ||
                                    //                             oCB_CLNTExist.MSTR_ChurchBodyId != oCB_MSTR.Id || oCB_CLNTExist.MSTR_ParentChurchBodyId != oCB_MSTR.ParentChurchBodyId ||
                                    //                             string.IsNullOrEmpty(oCB_CLNTExist.Name) || 
                                    //                             oCB_CLNTExist.ChurchWorkStatus != oCB_MSTR.ChurchWorkStatus || oCB_CLNTExist.Status != oCB_MSTR.Status)
                                    //                        {
                                    //                             oCB_CLNTExist.MSTR_AppGlobalOwnerId = oCB_MSTR.AppGlobalOwnerId;
                                    //                             oCB_CLNTExist.MSTR_ChurchLevelId = oCB_MSTR.ChurchLevelId;
                                    //                             oCB_CLNTExist.MSTR_ChurchBodyId = oCB_MSTR.Id;
                                    //                             oCB_CLNTExist.MSTR_ParentChurchBodyId = oCB_MSTR.ParentChurchBodyId;
                                    //                             oCB_CLNTExist.Name = oCB_MSTR.Name;
                                    //                             oCB_CLNTExist.ChurchWorkStatus = oCB_MSTR.Id == oUser_MSTR.ChurchBodyId ? "OP" : "ST";
                                    //                             oCB_CLNTExist.Status = oCB_MSTR.Id == oUser_MSTR.ChurchBodyId ? oCB_MSTR.Status : "P";  // P-Pending activation from vendor  //oCB_MSTR.Status,
                                    //                            ///
                                    //                            oCB_CLNTExist.LastMod = tm;
                                    //                            oCB_CLNTExist.LastModByUserId = oUser_MSTR.Id;

                                    //                            _clientContext.Update(oCB_CLNTExist);
                                    //                            _updCount++;
                                    //                    }
                                    //                }

                                    //                /// NEW only else... on-demand update ... so that this code is run jux once... NOT @ every login
                                    //                if ((_addCount + _updCount) > 0)
                                    //                {
                                    //                    _clientContext.SaveChanges();  // save first before updating parents...

                                    //                    /// set Parent ChurchBody at client level ... reload client CB list UP path only...
                                    //                   // var oCBList = _clientContext.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_CLNT.Id);

                                    //                    var oCBList = _clientContext.ChurchBody.AsNoTracking()
                                    //                            .Where(c => c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId && oUserCB_MSTR.RootChurchCode.Contains(c.GlobalChurchCode)).ToList();
                                    //                    var oCBParList = oCBList; // make a copy to search for the parent CB ... _clientContext.ChurchBody.Where(c => c.AppGlobalOwnerId == oAGO.Id);
                                    //                    _updCount = 0; tm = DateTime.Now;

                                    //                    if (oCBList.Count() > 0)
                                    //                    {
                                    //                        foreach (var oCB in oCBList)
                                    //                        {
                                    //                            if (oCB.ParentChurchBodyId == null ||
                                    //                               (oCB.ParentChurchBodyId != null &&
                                    //                               _clientContext.ChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGO_CLNT.Id && c.Id == oCB.ParentChurchBodyId).FirstOrDefault() == null))
                                    //                            {
                                    //                                var oCBPar = oCBParList.Where(c => c.AppGlobalOwnerId == oCB.AppGlobalOwnerId && c.MSTR_AppGlobalOwnerId == oCB.MSTR_AppGlobalOwnerId &&
                                    //                                                            c.MSTR_ChurchBodyId == oCB.MSTR_ParentChurchBodyId).FirstOrDefault();  // c.GlobalChurchCode == oCB.GlobalChurchCode && 
                                    //                                if (oCBPar != null)
                                    //                                {
                                    //                                    //if (oCB.ParentChurchBodyId != oCBPar.Id)
                                    //                                    //{
                                    //                                    oCB.ParentChurchBodyId = oCBPar.Id;
                                    //                                    oCB.LastMod = tm;
                                    //                                    oCB.LastModByUserId = oUser_MSTR.Id;
                                    //                                    ///
                                    //                                    _clientContext.Update(oCB);
                                    //                                    _updCount++;
                                    //                                    //}
                                    //                                }
                                    //                            }
                                    //                        }

                                    //                        /// save updated...
                                    //                        if (_updCount > 0)
                                    //                            _clientContext.SaveChanges();
                                    //                    }


                                    //                    // record ... @client
                                    //                    _userTask = "Created/updated " + _updCount + " " + strDesc.ToLower() + "s"; _tm = DateTime.Now;
                                    //                    _ = this.LogUserActivity_ClientUserAuditTrail(new UserAuditTrail_CL(0, oUser_MSTR.AppGlobalOwnerId, oUser_MSTR.ChurchBodyId, "T",
                                    //                                     "RCMS-Client: " + strDesc, AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUser_MSTR.Id, _tm, _tm, oUser_MSTR.Id, oUser_MSTR.Id)
                                    //                                    , _clientDBConnString);
                                    //                }
                                    //            }
                                    //       // }

                                    //        var oCBClient = _clientContext.ChurchBody.AsNoTracking().Include(t => t.AppGlobalOwner).Include(t => t.ChurchLevel).Include(t => t.ParentChurchBody)
                                    //                        .Where(c => c.MSTR_AppGlobalOwnerId == oUserCB_MSTR.AppGlobalOwnerId && (c.MSTR_ChurchBodyId == oUserCB_MSTR.Id || c.GlobalChurchCode == oUserCB_MSTR.GlobalChurchCode || (c.MSTR_ParentChurchBodyId == oUserCB_MSTR.ParentChurchBodyId && c.Name == oUserCB_MSTR.Name)))
                                    //                        .FirstOrDefault();

                                    //        // var _oCBClient = oCBClient;
                                    //        //var oCBClient = _clientContext.ChurchBody.AsNoTracking().Where(c => (c.OrgType == "CH" || c.OrgType == "CN") && c.MSTR_AppGlobalOwnerId == oUser_MSTR.AppGlobalOwnerId &&
                                    //        //                                (c.MSTR_ChurchBodyId == oUser_MSTR.ChurchBodyId || c.GlobalChurchCode == oCB_MSTR.GlobalChurchCode)).FirstOrDefault();


                                    //        // not new... save user info into session
                                    //        if (oCBClient != null)
                                    //        {
                                    //            // save the client db
                                    //            oUserPrivilegeCol.AppGlobalOwner_CLNT = oCBClient.AppGlobalOwner;
                                    //            oUserPrivilegeCol.ChurchBody_CLNT = oCBClient;
                                    //            _privList_CLNT = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                                    //            TempData["UserLogIn_oUserPrivCol"] = _privList_CLNT; TempData.Keep();

                                    //            //// get the church slogan... from CLIENT side
                                    //            _strSlogan_CL = "";
                                    //            // get the church slogan... from MSTR side
                                    //            // Asomdwei nka wo|enka wo nso 
                                    //            if (!string.IsNullOrEmpty(oCBClient.AppGlobalOwner?.Slogan))
                                    //            {
                                    //                _strSlogan_CL = oCBClient.AppGlobalOwner?.Slogan;
                                    //                if (_strSlogan_CL.Contains("*|*"))
                                    //                {
                                    //                    var _arrSlogan_CL = _strSlogan_CL.Split("*|*");
                                    //                    _strSlogan_CL = _arrSlogan_CL.Length > 0 ? _arrSlogan_CL[0] : _strSlogan_CL;
                                    //                }
                                    //            }

                                    //            // var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan);
                                    //            TempData["_strChurchSlogan"] = _strSlogan_CL; TempData.Keep();
                                    //            model.strChurchSlogan = _strSlogan_CL;
                                    //        }


                                    //        // nullify obj b/f save
                                    //        if (oUser_MSTR.AppGlobalOwner != null) oUser_MSTR.AppGlobalOwner = null; if (oUser_MSTR.ChurchBody != null) oUser_MSTR.ChurchBody = null;
                                    //        if (oCBClient != null)
                                    //        {
                                    //            if (oCBClient.MSTR_AppGlobalOwnerId == null || oCBClient.MSTR_ChurchBodyId == null || oCBClient.MSTR_ChurchLevelId == null || string.IsNullOrEmpty(oCBClient.GlobalChurchCode) || string.IsNullOrEmpty(oCBClient.Name))
                                    //            { 
                                    //                if (oCBClient.MSTR_AppGlobalOwnerId == null)
                                    //                    oCBClient.MSTR_AppGlobalOwnerId = oUserCB_MSTR.AppGlobalOwnerId;

                                    //                if (oCBClient.MSTR_ChurchBodyId == null)
                                    //                    oCBClient.MSTR_ChurchBodyId = oUserCB_MSTR.Id;

                                    //                if (oCBClient.MSTR_ChurchLevelId == null)
                                    //                    oCBClient.MSTR_ChurchLevelId = oUserCB_MSTR.ChurchLevelId;

                                    //                if (string.IsNullOrEmpty(oCBClient.GlobalChurchCode) || oAGO_CLNT.GlobalChurchCode != oUserCB_MSTR.GlobalChurchCode)
                                    //                    oCBClient.GlobalChurchCode = oUserCB_MSTR.GlobalChurchCode;

                                    //                if (string.IsNullOrEmpty(oCBClient.Name) || oCBClient.Name != oUserCB_MSTR.Name)
                                    //                    oCBClient.Name = oUserCB_MSTR.Name;

                                    //                // nullify obj b/f save
                                    //                oCBClient.ChurchLevel = null;

                                    //                ///
                                    //                // nullify obj b/f save
                                    //                if (oCBClient.AppGlobalOwner != null) oCBClient.AppGlobalOwner = null; if (oCBClient.ChurchLevel != null) oCBClient.ParentChurchBody = null;  
                                    //                ///
                                    //                _clientContext.Update(oCBClient);
                                    //                _clientContext.SaveChanges();

                                    //                ViewBag.UserMsg = strDesc + " updated successfully.";
                                    //                _userTask = "Updated " + strDesc.ToLower() + ", " + oCBClient.Name.ToUpper() + " successfully";
                                    //            }
                                    //        }


                                    //        // update master db-user
                                    //        oUser_MSTR.IsCLNTInit = true;
                                    //        oUser_MSTR.LastMod = tm;
                                    //        // oUser_MSTR.LastModByUserId = oUser_MSTR.Id;


                                    //        // nullify obj b/f save
                                    //        if (oUser_MSTR.AppGlobalOwner != null) oUser_MSTR.AppGlobalOwner = null; if (oUser_MSTR.ChurchBody != null) oUser_MSTR.ChurchBody = null;
                                    //        _context.Update(oUser_MSTR);

                                    //        ///
                                    //        _context.SaveChanges();


                                    //        //// not new... save user info into session
                                    //        //if (_oCBClient != null)
                                    //        //{    
                                    //        //    // save the client db
                                    //        //    oUserPrivilegeCol.AppGlobalOwner_CLNT = _oCBClient.AppGlobalOwner;
                                    //        //    oUserPrivilegeCol.ChurchBody_CLNT = _oCBClient;
                                    //        //    var _privList_CLNT = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                                    //        //    TempData["UserLogIn_oUserPrivCol"] = _privList_CLNT; TempData.Keep();
                                                 
                                    //        //    //// get the church slogan... from CLIENT side
                                    //        //    var _strSlogan_CL = "";
                                    //        //    // get the church slogan... from MSTR side
                                    //        //    // Asomdwei nka wo|enka wo nso 
                                    //        //    if (!string.IsNullOrEmpty(_oCBClient.AppGlobalOwner?.Slogan))
                                    //        //    {
                                    //        //        _strSlogan_CL = _oCBClient.AppGlobalOwner?.Slogan;   
                                    //        //        if (_strSlogan_CL.Contains("*|*"))
                                    //        //        {
                                    //        //            var _arrSlogan_CL = _strSlogan_CL.Split("*|*");
                                    //        //            _strSlogan_CL = _arrSlogan_CL.Length > 0 ? _arrSlogan_CL[0] : _strSlogan_CL;
                                    //        //        }
                                    //        //    } 


                                    //        //    // var _strSlogan = Newtonsoft.Json.JsonConvert.SerializeObject(strSlogan);
                                    //        //    TempData["_strChurchSlogan"] = _strSlogan_CL; TempData.Keep();
                                    //        //    model.strChurchSlogan = _strSlogan_CL;
                                    //        //}
                                              
                                    //    }

                                    //    catch (Exception ex)
                                    //    {
                                    //        ModelState.AddModelError("", "Account validation failed. Client working space initialization failed. Please reload page to continue or contact System Admin. cli Err: " + ex.ToString()); // : ");
                                    //        return View(model);
                                    //    } 
                                    //} 
                                     

                                    /// //// end: CL INIT moved to vendor tasks-------------------------------

                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError("", "Account validation failed. Client database verification failed. Please reload page to continue or contact System Admin. ven Error: " + ex.Message); //);
                                    return View(model);
                                } 
                            }


                            // }

                            //if (!isUserValidated) //oUser_MSTR == null)
                            //{
                            //    model.IsVal = 0;
                            //    ViewBag.ErrorMess = "Account validation failed. Username or password provided is incorrect.";
                            //    ///
                            // //   ModelState.AddModelError("", "Account validation failed. Username or password provided is incorrect");
                            //    return View(model);
                            //}
                             
                        }

                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Account validation failed. Church code, username or password provided is incorrect. Please reload page to continue or contact System Admin. Error: " + ex.Message); // );
                            return View(model);
                        }
                    }
                      

                    ////??
                    //// RESET User password...... @pwd_reset, expired user, expired pwd ...
                    /////
                    //if (oUser_MSTR != null && isUserValidated)
                    //{
                    //    ////check Pwd expiry / Acc expiry  --- RESET PWD
                    //    /////
                    //    //var userAccExpr = oUser_MSTR.Expr != null ? oUser_MSTR.Expr.Value : oUser_MSTR.Expr;
                    //    //userAccExpr = userAccExpr != null ? userAccExpr.Value : userAccExpr; 
                    //    /////
                    //    //if (oUser_MSTR.ResetPwdOnNextLogOn == true || oUser_MSTR.UserStatus != "A" || userAccExpr <= DateTime.Now.Date )
                    //    //{//int userId = 0, int? oCurrChuBodyId = null, string profileScope = "C", int setIndex = 0

                    //    //    if (oUser != null)
                    //    //        oUserPrivilegeCol = AppUtilties.GetUserPrivilege(_context, model.ChurchCode, oUser);

                    //    //    var _privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                    //    //    TempData["UserLogIn_oUserPrivCol"] = _privList; TempData.Keep();

                    //    //    //  return RedirectToAction("GetChangeUserPwdDetail", "UserLogin");

                    //    //    // public IActionResult AddOrEdit_ChangeUserPwd(string churchCode, string username, int setIndex) 
                    //    //    var routeValues = new RouteValueDictionary {
                    //    //                                  { "churchCode", model.ChurchCode },
                    //    //                                  { "username", model.Username },
                    //    //                                  { "setIndex", model.ChurchCode == "000000" ? 1 : 2 }
                    //    //                                };

                    //    //    return RedirectToAction("AddOrEdit_ChangeUserPwd", "UserLogin", routeValues);


                    //    //    //change pwd... afterward... sign out!
                    //    //    // public IActionResult AddOrEdit_UP_ChangePwd(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0,
                    //    //    // int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null) 

                    //    //    //return RedirectToAction("AddOrEdit_UP_ChangePwd", "AppVenAdminController", new { oAppGloOwnId = (int?)null, oCurrChuBodyId = (int?)null, 
                    //    //    //                id = oUser_MSTR.Id, setIndex = 1, subSetIndex = 1, oAGOId_Logged = oUser_MSTR.AppGlobalOwnerId, oCBId_Logged = oUser_MSTR.ChurchBodyId, oUserId_Logged = oUser_MSTR.Id });


                    //    //    //return RedirectToAction("AddOrEdit_UP_ChangePwd", "UserProfile", new { userId = oUser_MSTR.Id, oCurrChuBodyId = oUser_MSTR.ChurchBodyId, profileScope = oUser_MSTR.ProfileScope, setIndex = 4 });

                    //    //    //return RedirectToAction( "Main", new RouteValueDictionary( new { controller = controllerName, action = "Main", Id = Id } ) );
                    //    //    //return RedirectToAction("Action", new { id = 99 });
                    //    //    // return RedirectToAction("AddOrEdit_UP_ChangePwd", "UserProfile", );
                    //    //}


                    //    // get the collection of permissions
                    //    oUserPrivilegeCol = AppUtilties.GetUserPrivilege(_context, model.ChurchCode, oUser_MSTR);


                    //    // oUserPrivilegeCol = AppUtilties.ValidateUser( _context, _clientDBContext, model.ChurchCode, model.Username, model.Password);
                    //    //TempData.Put("UserLogIn_oUserPrivCol", oUserPrivilegeCol); TempData.Keep();

                        
                                              
                    //    //var userDataList = Newtonsoft.Json.JsonConvert.SerializeObject(oUser);
                    //    //TempData["UserLogIn_oUserProLogged"] = userDataList; TempData.Keep();
                    //    ///
                    //    var privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);   // flag out later... keep for now cos of the errors
                    //    TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();
                    //    ///

                    //    //ViewBag.oAppGloOwnId_Logged = oUserPrivilegeCol[0].ChurchBody.AppGlobalOwnerId;
                    //    //ViewBag.oChuBodyId_Logged = oUserPrivilegeCol[0].ChurchBody.Id;
                    //    ////
                    //    //ViewBag.oUserId_Logged = oUserPrivilegeCol[0].UserProfile.Id;
                    //    //ViewBag.oMemberId_Logged = oUserPrivilegeCol[0].UserProfile.ChurchMemberId;



                    //    //TempData.Put("oModel", model); TempData.Keep();
                    //    //TempData.Put("oVerifUserPriv", oUserPrivilegeCol); TempData.Keep();

                    //    //model.currChurchCode = model.ChurchCode;
                    //    //model.currUsername = model.Username;
                    //    //model.currPassword = model.Password;

                    //    isUserValidated = oUserPrivilegeCol.UserSessionPermList.Count > 0; // != null;
                    //    if (!isUserValidated)
                    //    {
                    //        ViewBag.ErrorMess = "No permissions assigned to specified user account. Please contact Administrator to rectify issue and try again.";
                    //        ModelState.AddModelError("", "No permissions assigned to specified user account. Please contact Administrator to rectify issue and try again.");
                    //        return View(model);
                    //    }
                    //    // false if No permissions [ ROLE / GROUP] found!
                    //}

                }

                //else
                //{
                //    //// oUserPrivilegeCol = UserLogOnUtility.ValidateUser(_context, model.currChurchCode, model.currUsername, model.currPassword);                   
                //    //// oUserPrivilegeCol = TempData.Get<UserSessionPrivilege>("UserLogIn_oUserPrivCol");

                //    //var tempPrivList = TempData["UserLogIn_oUserPrivCol"] as string;
                //    //// De serialize the string to object
                //    //oUserPrivilegeCol = Newtonsoft.Json.JsonConvert.DeserializeObject<UserSessionPrivilege>(tempPrivList);
                //    ////isUserValidated = oUserPrivilegeCol.UserSessionPermList.Count > 0; //  != null;


                //    //isUserValidated = false;

                //    model.IsVal = 0;
                //    // ViewBag.ErrorMess = "Account validation failed. Username or password provided is incorrect.";
                //    ModelState.AddModelError("", ViewBag.ErrorMess != null ? ViewBag.ErrorMess : "Account validation failed. Enter correct credentials or verify if user account has permissions to log in.");
                //    return View(model);
                //}

                
              //  model.IsVal = isUserValidated ? "T" : "F";
                 
                //  var userValidated = true;
                // user verification ... 2 way check (sms / email) or security quetion -- token to be sent once per active session


                ///// trusted user --- or ---- 2-WAY VERIFICATION 

                //if (isUserValidated)
                //{
                //    // false until proven true
                //    model.IsVal = 0;


                //    // VERIFY ... user
                //    //oUser_MSTR = _context.UserProfile .Include(u => u.ChurchBody)
                //    //   .Where(c => c.ChurchBody.ChurchCode == model.ChurchCode && c.Username == model.Username && c.UserStatus == "A").FirstOrDefault();

                //    if (AppUtilties.TrustedClients.ValidateClient(model.Username, null, null) == false) //Request.UserHostAddress, Request.UserAgent))
                //    {
                //        model.IsVal = 1;  //    valid = true;
                //    }
                //    else

                //    {   //
                //        //clear this line when done... testing only!!!   AppUtilties.ComputeSha256Hash("12345678") 
                //        model.VerificationCode = "12345678" ;  TempData ["oVmLogin"] = AppUtilties.ComputeSha256Hash("12345678") ; TempData.Keep();     
                //        //

                //        if (string.IsNullOrEmpty(model.VerificationCode))
                //        {
                //            // string vCode = "12345678"; 
                //            string vCode = CodeGenerator.GenerateCode();
                //            string vCodeEncrypt = AppUtilties.ComputeSha256Hash(vCode);
                //            // UserLogOnUtility.StoreValidationCode(model.UserName, vCodeEncrypt);
                             
                //            //  TempData ["oVmLogin"] = vCodeEncrypt; TempData.Keep();

                //            //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(vCodeEncrypt);
                //            //TempData["oVmLogin"] = privList; TempData.Keep();

                //            TempData["oVmLogin"] = vCodeEncrypt; TempData.Keep();

                //            // AppUtilties.SendSMSNotification(UserLogOnUtility.GetUserPhone(_context, model.Username), "233", string.Format("Your account verification code is {0}.", vCode));
                //            ViewData["VerificationCodeEnabled"] = true;
                //            var tempDesc = model.Username;
                //            if (oUserPrivilegeCol.UserSessionPermList.Count != 0)
                //            {
                //                tempDesc = oUserPrivilegeCol.UserProfile.UserDesc;
                //                //
                //                // can be done after user gone in... and client identified
                //                //if (oUserPrivilegeCol[0].UserProfile.ChurchMemberId != null)
                //                //{
                //                //    var tcm = _clientDBContext.ChurchMember.Where(c => c.AppGlobalOwnerId == oUserPrivilegeCol[0].UserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserPrivilegeCol[0].UserProfile.ChurchBodyId && c.Id == oUserPrivilegeCol[0].UserProfile.ChurchMemberId).FirstOrDefault();

                //                //    if (!string.IsNullOrEmpty(tcm.FirstName))
                //                //        tempDesc = tcm.FirstName;
                //                //    else if (!string.IsNullOrEmpty(tcm.LastName))
                //                //        tempDesc = tcm.LastName;
                //                //    else if (!string.IsNullOrEmpty(tcm.MiddleName))
                //                //        tempDesc = tcm.MiddleName; 
                //                //}
                //                ////else
                //                ////    tempDesc = oUserPrivilegeCol[0].UserProfile.UserDesc;
                //            }            
                                    
                //            //
                //            model.strLogUserDesc = tempDesc;
                //            ModelState.ClearValidationState("");

                //            return View(model);
                //        }

                //        else
                //        {
                //            //var vCode = TempData.Get<string>("oVmLogin");
                //            var vCodeEncrypt = TempData["oVmLogin"] as string ;

                //            //var arrData = "";
                //            //arrData = TempData.ContainsKey("oVmLogin") ? TempData["oVmLogin"] as string : arrData;
                //            //var vCode = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<string>(arrData) : "";

                //            if (vCodeEncrypt != null)
                //            {   // if (UserLogOnUtility.ReadValidationCode(_context, model.UserName) == model.VerificationCode)
                //                if (vCodeEncrypt != AppUtilties.ComputeSha256Hash(model.VerificationCode))
                //                {   // TrustedClients.AddClient(model.UserName, Request.UserHostAddress, Request.UserAgent);
                //                    model.IsVal = 1; // valid = true;
                //                }
                //                else
                //                {
                //                    ModelState.AddModelError("", "The verification code is incorrect."); // isUserValidated = false;   
                //                    ViewData["VerificationCodeEnabled"] = true;
                //                    return View(model);
                //                }
                //            }
                //        }
                //    }
                //}

                //else
                //{
                //    model.IsVal = 0;
                //    // ViewBag.ErrorMess = "Account validation failed. Username or password provided is incorrect.";
                //    ModelState.AddModelError("", ViewBag.ErrorMess != null ? ViewBag.ErrorMess : "Account validation failed. Enter correct credentials or verify if user account has permissions to log in.");
                //    return View(model);
                //}



            }


            ///// trusted user --- or ---- 2-WAY VERIFICATION 

            //if (isUserValidated)
            //{
            //    // false until proven true
            //    model.IsVal = 0;


            //    // VERIFY ... user
            //    //oUser_MSTR = _context.UserProfile .Include(u => u.ChurchBody)
            //    //   .Where(c => c.ChurchBody.ChurchCode == model.ChurchCode && c.Username == model.Username && c.UserStatus == "A").FirstOrDefault();

            //    if (AppUtilties.TrustedClients.ValidateClient(model.Username, null, null) == false) //Request.UserHostAddress, Request.UserAgent))
            //    {
            //        model.IsVal = 1;  //    valid = true;
            //    }
            //    else

            //    {   //
            //        //clear this line when done... testing only!!!   AppUtilties.ComputeSha256Hash("12345678") 
            //        model.VerificationCode = "12345678"; TempData["oVmLogin"] = AppUtilties.ComputeSha256Hash("12345678"); TempData.Keep();
            //        //

            //        if (string.IsNullOrEmpty(model.VerificationCode))
            //        {
            //            // string vCode = "12345678"; 
            //            string vCode = CodeGenerator.GenerateCode();
            //            string vCodeEncrypt = AppUtilties.ComputeSha256Hash(vCode);
            //            // UserLogOnUtility.StoreValidationCode(model.UserName, vCodeEncrypt);

            //            //  TempData ["oVmLogin"] = vCodeEncrypt; TempData.Keep();

            //            //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(vCodeEncrypt);
            //            //TempData["oVmLogin"] = privList; TempData.Keep();

            //            TempData["oVmLogin"] = vCodeEncrypt; TempData.Keep();

            //            // AppUtilties.SendSMSNotification(UserLogOnUtility.GetUserPhone(_context, model.Username), "233", string.Format("Your account verification code is {0}.", vCode));
            //            ViewData["VerificationCodeEnabled"] = true;
            //            var tempDesc = model.Username;
            //            if (oUserPrivilegeCol.UserSessionPermList.Count != 0)
            //            {
            //                tempDesc = oUserPrivilegeCol.UserProfile.UserDesc;
            //                //
            //                // can be done after user gone in... and client identified
            //                //if (oUserPrivilegeCol[0].UserProfile.ChurchMemberId != null)
            //                //{
            //                //    var tcm = _clientDBContext.ChurchMember.Where(c => c.AppGlobalOwnerId == oUserPrivilegeCol[0].UserProfile.AppGlobalOwnerId && c.ChurchBodyId == oUserPrivilegeCol[0].UserProfile.ChurchBodyId && c.Id == oUserPrivilegeCol[0].UserProfile.ChurchMemberId).FirstOrDefault();

            //                //    if (!string.IsNullOrEmpty(tcm.FirstName))
            //                //        tempDesc = tcm.FirstName;
            //                //    else if (!string.IsNullOrEmpty(tcm.LastName))
            //                //        tempDesc = tcm.LastName;
            //                //    else if (!string.IsNullOrEmpty(tcm.MiddleName))
            //                //        tempDesc = tcm.MiddleName; 
            //                //}
            //                ////else
            //                ////    tempDesc = oUserPrivilegeCol[0].UserProfile.UserDesc;
            //            }

            //            //
            //            model.strLogUserDesc = tempDesc;
            //            ModelState.ClearValidationState("");

            //            return View(model);
            //        }

            //        else
            //        {
            //            //var vCode = TempData.Get<string>("oVmLogin");
            //            var vCodeEncrypt = TempData["oVmLogin"] as string;

            //            //var arrData = "";
            //            //arrData = TempData.ContainsKey("oVmLogin") ? TempData["oVmLogin"] as string : arrData;
            //            //var vCode = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<string>(arrData) : "";

            //            if (vCodeEncrypt != null)
            //            {   // if (UserLogOnUtility.ReadValidationCode(_context, model.UserName) == model.VerificationCode)
            //                if (vCodeEncrypt != AppUtilties.ComputeSha256Hash(model.VerificationCode))
            //                {   // TrustedClients.AddClient(model.UserName, Request.UserHostAddress, Request.UserAgent);
            //                    model.IsVal = 1; // valid = true;
            //                }
            //                else
            //                {
            //                    ModelState.AddModelError("", "The verification code is incorrect."); // isUserValidated = false;   
            //                    ViewData["VerificationCodeEnabled"] = true;
            //                    return View(model);
            //                }
            //            }
            //        }
            //    }
            //}



            // by thus far... no error!
            // done ... we av user with priv
            isUserValidated = true; model.IsVal = 1;

            // user authentication and authorization done... go to --->>> 1) Admin Profile or 2) Client Dashboard  ... by client data configurations
            if (model.IsVal == 1)  // valid==true
            {

                // if (ModelState.ContainsKey("")) ModelState[""].Errors.Clear();
                //ModelState.vali ModelState.Remove["{}"];
                // ModelState[""].ValidationState = ModelValidationState.Valid;

                //foreach (var key in ModelState.Keys)
                //{
                //    ModelState[key].Errors.Clear();
                //    ModelState[key].ValidationState = ModelValidationState.Valid;
                //}

                //ViewBag.vwCurrChurchBodyId = oUserPrivilegeCol[0].ChurchBody.Id;
                //ViewBag.vwCurrChurchBody = oUserPrivilegeCol[0].ChurchBody;

                if (oUserPrivilegeCol == null)
                {
                    ModelState.AddModelError("", ViewBag.ErrorMess != null ? ViewBag.ErrorMess : "Account validation failed. Please enter correct credentials or verify if user account has permissions to log in.");
                    return View(model);
                }

                if (TempData == null)
                { 
                    var _privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                    TempData["UserLogIn_oUserPrivCol"] = _privList; TempData.Keep();
                }
                else if (!TempData.ContainsKey("UserLogIn_oUserPrivCol"))
                {
                    var _privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                    TempData["UserLogIn_oUserPrivCol"] = _privList; TempData.Keep();
                }
                
                //var privList = Newtonsoft.Json.JsonConvert.SerializeObject(oUserPrivilegeCol);
                //TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();

                // return RedirectToAction("Index", "ChurchWorkbench");

                //authentication done...   //successfull login... audit!
                var tm = DateTime.Now;
                ViewData["strAppName"] = "RHEMA-CMS";

                //vendor home
                if (AppUtilties.ComputeSha256Hash(model.ChurchCode) == ac1)
                {
                    //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
                    //            "L", "RCMS-Admin: User Login", AppUtilties.GetRawTarget(HttpContext.Request), "Logged in successfully to RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));
                    //
                   // ModelState.ClearValidationState("");

                    return RedirectToAction("Index_sa", "Home"); 

                   // return RedirectToAction("Index_AGO", "AppVenAdmin");                     
                }                    

                //...client home
                else
                {
                    // get the connection... to client db
                    //var conn = new SqlConnectionStringBuilder(_context.Database.GetDbConnection().ConnectionString);
                    ////  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
                    //conn.DataSource = "RHEMA-SDARTEH"; conn.InitialCatalog = "DBRCMS_CL_TEST"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
                    //this._clientDBConnString = conn.ConnectionString;


                    //_ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, oUserPrivilegeCol[0].AppGlobalOwner != null ? oUserPrivilegeCol[0].AppGlobalOwner.Id : (int?)null, oUserPrivilegeCol[0].ChurchBody != null ? oUserPrivilegeCol[0].ChurchBody.Id : (int?)null,
                    //            "L", "RCMS Client: User Login", AppUtilties.GetRawTarget(HttpContext.Request), "Logged in successfully to RHEMA-CMS", tm, oUserPrivilegeCol[0].UserProfile.Id, tm, tm, oUserPrivilegeCol[0].UserProfile.Id, oUserPrivilegeCol[0].UserProfile.Id));
                    //
                   // ModelState.ClearValidationState("");

                    return RedirectToAction("Index", "Home");
                }
                   

                //// FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1
                //    && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                //{
                //    return Redirect(returnUrl);
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Home");
                //}
            }

            // If we got this far, something failed, redisplay form



            ModelState.AddModelError("", ViewBag.ErrorMess != null ? ViewBag.ErrorMess : "Account validation failed. Please enter correct credentials or verify if user account has permissions to log in.");
            // done ... we av user with priv
            isUserValidated = false; model.IsVal = 0;
            return View(model);

        }


        //private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail)
        //{ // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
        //    if (oUserTrail != null)
        //    {  
        //        _masterContextLog.UserAuditTrail.Add(oUserTrail);
        //        await _masterContextLog.SaveChangesAsync();
        //    }
        //}



        private async Task LogUserActivity_AppMainUserAuditTrail(UserAuditTrail oUserTrail)  //, MSTR_DbContext currContext = null, string strTempConn = "")
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // MSTR_DbContext currContext = null, string strTempConn = ""
                //var _cs = strTempConn;
                //if (string.IsNullOrEmpty(_cs))

                var _cs = AppUtilties.GetNewDBConnString_MS(_configuration);  ///  this._configuration.GetConnectionString("DefaultConnection"); //["ConnectionStrings:DefaultConnection"]; /// _masterContext.Database.GetDbConnection().ConnectionString

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
         

        private async Task LogUserActivity_ClientUserAuditTrail(UserAuditTrail_CL oUserTrail, int? oAGOid)  //, string strTempConn = ""
        { // var oUserTrail = _masterContext.UserAuditTrail.Where(c => ((c.AppGlobalOwnerId == null && c.ChurchBodyId == null && churchCode=="000000") || (c.AppGlobalOwnerId== oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId))
            if (oUserTrail != null)
            {
                // var tempCtx = _context;
                //  if (!string.IsNullOrEmpty(clientDBConnString))
                // {

                // refreshValues...
               // var _connstr_CL = this.GetCL_DBConnString(oAGOid);

                var _connstr_CL = AppUtilties.GetNewDBConnString_CL(_context, _configuration, oAGOid);
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
                 

        //private string GetCL_DBConnString(int? oAGOid)
        //{
        //    //var isAuth = this.oUserLogIn_Priv != null;
        //    //if (!isAuth) isAuth = SetUserLogged();

        //    //if (!isAuth)
        //    //    RedirectToAction("LoginUserAcc", "UserLogin");

        //    //if (this.oUserLogIn_Priv == null)
        //    //    RedirectToAction("LoginUserAcc", "UserLogin");

        //    //if (this.oUserLogIn_Priv.UserProfile == null)
        //    //    RedirectToAction("LoginUserAcc", "UserLogin");


        //    if (string.IsNullOrEmpty(this._clientDBConn))
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_context, _configuration, oAGOid);

        //    var _clientContext = new ChurchModelContext(this._clientDBConn);
        //    if (_clientContext.Database.CanConnect()) return this._clientDBConn;
        //    else
        //    {
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_context, _configuration, oAGOid);
        //        _clientContext = new ChurchModelContext(this._clientDBConn);
        //        if (_clientContext.Database.CanConnect()) return this._clientDBConn;
        //        else
        //        {
        //            var oClientConfig = _context.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == oAGOid && c.Status == "A").FirstOrDefault();
        //            if (oClientConfig != null)
        //            {
        //                var _cs = _configuration.GetConnectionString("DefaultConnection");
        //                // get and mod the conn                        
        //                var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
        //                conn.DataSource = oClientConfig.ServerName.Contains("\\\\") ? oClientConfig.ServerName.Replace("\\\\", "\\") : oClientConfig.ServerName;
        //                conn.InitialCatalog = oClientConfig.DbaseName;
        //                conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //                /// conn.IntegratedSecurity = false; 
        //                conn.MultipleActiveResultSets = true; //conn.TrustServerCertificate = true;

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

        //private ChurchModelContext GetClientDBContext(int? oAGOid)  ///(UserProfile oUserLogged)
        //{
        //    //var isAuth = this.oUserLogIn_Priv != null;
        //    //if (!isAuth) isAuth = SetUserLogged();

        //    //if (!isAuth)
        //    //    RedirectToAction("LoginUserAcc", "UserLogin");

        //    //if (this.oUserLogIn_Priv == null)
        //    //    RedirectToAction("LoginUserAcc", "UserLogin");

        //    //if (this.oUserLogIn_Priv.UserProfile == null)
        //    //    RedirectToAction("LoginUserAcc", "UserLogin");

        //    if (string.IsNullOrEmpty(this._clientDBConn))
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_context, _configuration, oAGOid);


        //    var _clientContext = new ChurchModelContext(this._clientDBConn);
        //    if (_clientContext.Database.CanConnect()) return _clientContext;
        //    else
        //    {
        //        this._clientDBConn = AppUtilties.GetNewDBConnString_CL(_context, _configuration, oAGOid);
        //        _clientContext = new ChurchModelContext(this._clientDBConn);
        //        if (_clientContext.Database.CanConnect()) return _clientContext;
        //        else
        //        {
        //            var oClientConfig = _context.ClientAppServerConfig.Where(c => c.AppGlobalOwnerId == oAGOid && c.Status == "A").FirstOrDefault();
        //            if (oClientConfig != null)
        //            {
        //                var _cs = _configuration.GetConnectionString("conn_CLNT");
        //                // get and mod the conn                        
        //                var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
        //                conn.DataSource = oClientConfig.ServerName.Contains("\\\\") ? oClientConfig.ServerName.Replace("\\\\", "\\") : oClientConfig.ServerName;
        //                conn.InitialCatalog = oClientConfig.DbaseName;
        //                conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
        //                /// conn.IntegratedSecurity = false; 
        //                conn.MultipleActiveResultSets = true; // conn.TrustServerCertificate = true;

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


        //private bool isCurrValid = false;
        //private UserSessionPrivilege oUserLogIn_Priv = null;
        //private bool userAuthorized = false;


        //private void SetUserLogged()
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








        [HttpGet]
        public IActionResult GetChangeUserPwdDetail(int pageIndex = 1)
        {
            // page index
            // 0 - done .. re-login
            // 1 - new user / pwd
            // 2 exist user ... change pwd
            var oUserResetModel = new ResetUserPasswordModel();
             
            // oUserResetModel.pageIndex = 1; 
             //
            return View("AddOrEdit_ChangeUserPwd", oUserResetModel);
        }


        [HttpGet]   // public IActionResult AddOrEdit_UP_ChangePwd(int? oAppGloOwnId = null, int? oCurrChuBodyId = null, int? id = 0, int setIndex = 0, int subSetIndex = 0, int? oAGOId_Logged = null, int? oCBId_Logged = null, int? oUserId_Logged = null)  //(int userId = 0, int setIndex = 0) // int? oCurrChuBodyId = null, string profileScope = "C", int setIndex = 0)   // setIndex = 0 (SYS), setIndex = 1 (SUP_ADMN), = 2 (Create/update user), = 3 (reset Pwd) 
        public IActionResult AddOrEdit_ChangeUserPwd(string churchCode, string username, int setIndex, int pageIndex = 1)  //, int? oUserId_Logged = null  (int userId = 0, int setIndex = 0) // int? oCurrChuBodyId = null, string profileScope = "C", int setIndex = 0)   // setIndex = 0 (SYS), setIndex = 1 (SUP_ADMN), = 2 (Create/update user), = 3 (reset Pwd) 
        {
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
            }

            // trim leading /trailing spaces...
            //if (churchCode == null) churchCode = ""; if (username == null) username = "";
            if (!string.IsNullOrEmpty(username)) username = username.Trim(); else username = "";
            if (!string.IsNullOrEmpty(churchCode)) churchCode = churchCode.Trim(); else churchCode = "";

            //if (!SetUserLogged())
            //    RedirectToAction("LoginUserAcc", "UserLogin");

            //SetUserLogged();
            //if (!isCurrValid) return RedirectToAction("LoginUserAcc", "UserLogin");

            //else
            //{

            // check permission 
            var _oUserPrivilegeCol = this.oUserLogIn_Priv;
                var privList = Newtonsoft.Json.JsonConvert.SerializeObject(_oUserPrivilegeCol);
                TempData["UserLogIn_oUserPrivCol"] = privList; TempData.Keep();

                //
                int? oAGOId_Logged = null;
                int? oCBId_Logged = null;
                int? oUserId_Logged = null;
                UserProfile oLoggedUser = null;
               // UserRole oLoggedRole = null;
                MSTRChurchBody oChuBody_Logged = null;
                //
               // if (!this.userAuthorized) return View(new UserProfileModel()); //retain view    
                if (this.oUserLogIn_Priv != null)
                {
                    oLoggedUser = this.oUserLogIn_Priv.UserProfile;
                   // oLoggedRole = this.oUserLogIn_Priv.UserRole; 
                    oChuBody_Logged = this.oUserLogIn_Priv.ChurchBody;

                    if (oLoggedUser != null) oUserId_Logged = oLoggedUser.Id;
                    if (oChuBody_Logged != null) { oCBId_Logged = oChuBody_Logged.Id; oAGOId_Logged = oChuBody_Logged.AppGlobalOwnerId;}
                    if (oAGOId_Logged == null && this.oUserLogIn_Priv.AppGlobalOwner != null) oAGOId_Logged = this.oUserLogIn_Priv.AppGlobalOwner.Id; 
                }
                 
                //
                var oUserResetModel = new ResetUserPasswordModel();
                // 1-SYS .. 2-SUP_ADMN, 3-SYS_ADMN, 4-SYS_CUST | 6-CH_ADMN, 7-CF_ADMN
                setIndex = churchCode == "000000" ? 1 : 2;
                var proScope = setIndex == 1 ? "V" : "C";
                var subScope = setIndex == 2 ? "D" : setIndex == 3 ? "A" : "";

                var _userTask = "Attempt to change password. Retrieved user details"; var _tm = DateTime.Now; var userDenom = "";
                //int? oAppGloOwnId = null; int? oCurrChuBodyId = null;
                //int? oAGOId_Logged = null; int? oCBId_Logged = null;
                // 
                //var oUser_MSTR = _context.UserProfile
                //         .Where(c => c.AppGlobalOwnerId == oAppGloOwnId && c.ChurchBodyId == oCurrChuBodyId && c.ProfileScope == proScope).FirstOrDefault();

                const string ac1 = "91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203";

            
             /// var h_pwd = ""; 
             /// if (!string.IsNullOrEmpty(churchCode)) 
             var h_pwd = AppUtilties.ComputeSha256Hash(churchCode.Trim());

            //string strUserKeyHashedData = "";
            //if (!string.IsNullOrEmpty(username)) username = username.Trim().ToLower();

            var strUserKeyHashedData = AppUtilties.ComputeSha256Hash(churchCode + username.ToLower()); 
            ///
                var oUser_MSTR = _context.UserProfile.AsNoTracking().Include(t=>t.AppGlobalOwner).Include(t=> t.ChurchBody)
                                .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ProfileScope == proScope && x.UserStatus == "A" &&
                                    x.Username.ToLower() == username.ToLower() && x.UserKey == strUserKeyHashedData).FirstOrDefault();
                
                if (oUser_MSTR == null) 
                {
                    if (pageIndex == 2)
                    {
                        userDenom = setIndex == 1 ? "Vendor Admin" : "Client";
                        _userTask = "Attempt to change password. User account could not be retrieved - " + username + " [" + userDenom + "]";
                        _tm = DateTime.Now;
                        _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                          setIndex == 1 ? "RCMS-Admin:" : "RCMS-Client:" + " User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged));
                        //


                        oUserResetModel.userMess = "Attempt to change password. User account could not be retrieved.";
                        ModelState.AddModelError("", oUserResetModel.userMess);
                    }

                oUserResetModel.pageIndex = 1;
                return View(oUserResetModel); // PartialView("_AddOrEdit_UP_ChangePwd", oUserResetModel);
                }
 
                // 
                //oCurrVmMod.oChurchBody = oCurrChuBodyLogOn;  
                oUserResetModel.oUserProfile = oUser_MSTR;
                //
                oUserResetModel.ChurchCode = churchCode;
                oUserResetModel.Username = oUser_MSTR.Username;
                oUserResetModel.strLogUserDesc = oUser_MSTR.UserDesc;
                ViewData["strAppName"] = "RHEMA-CMS";
                ///
                oUserResetModel.CurrentPassword = null;
                oUserResetModel.NewPassword = null;
                oUserResetModel.RepeatPassword = null;
                // oCurrVmMod.SecurityQue = oUser_MSTR.PwdSecurityQue;
                // oCurrVmMod.SecurityAns = null;
                oUserResetModel.VerificationCode = null; // via email, sms                
                oUserResetModel.AuthTypeUsed = null;

                //var _oCurrVmMod = oCurrVmMod;
                //TempData["oVmCurrMod"] = _vmMod;
                //TempData.Keep();

                //var _vmMod = Newtonsoft.Json.JsonConvert.SerializeObject(_oCurrVmMod);
                //TempData["oVmCurrMod"] = _vmMod; TempData.Keep();


                oUserResetModel.setIndex = setIndex;
                oUserResetModel.pageIndex = pageIndex;  ///2; // 
                oUserResetModel.profileScope = proScope;
                oUserResetModel.subScope = subScope;
                //
                oUserResetModel.oCurrUserId_Logged = oUserId_Logged;
                oUserResetModel.oAppGloOwnId_Logged = oAGOId_Logged;
                oUserResetModel.oChurchBodyId_Logged = oCBId_Logged;
                oUserResetModel.oMemberId_Logged = oUser_MSTR.ChurchMemberId; // oCurrChuBodyId;
                //
                oUserResetModel.oAppGloOwnId = oUser_MSTR.AppGlobalOwnerId; // oAppGloOwnId;
                oUserResetModel.oChurchBodyId = oUser_MSTR.ChurchBodyId; // oCurrChuBodyId;
                
                 
                oUserResetModel.lkpAuthTypes = new List<SelectListItem>();
                foreach (var dl in dlUserAuthTypes) 
                { 
                    oUserResetModel.lkpAuthTypes.Add(new SelectListItem { Value = dl.Val, Text = dl.Desc });
                    oUserResetModel.lkpAuthTypes.Insert(0, new SelectListItem { Value = "", Text = "Select authentication type", Disabled=true }); 
                }

                // send valiadation code to user... else promt user!            
                if (!string.IsNullOrEmpty(oUser_MSTR.Email))
                {
                    //email recipients... applicant, church   ... specific e-mail content
                    MailAddressCollection listToAddr = new MailAddressCollection();
                    MailAddressCollection listCcAddr = new MailAddressCollection();
                    MailAddressCollection listBccAddr = new MailAddressCollection();
                    // string strUrl = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);

                    // string.Format("{0:D3}", 0); String.Format("{0:N0}", 0)
                    var vCode = CodeGenerator.GenerateCode(4, 4, false, true);  // make it more unique... 000000-BZWER09J-20210405
                                       
                    var cc = oUser_MSTR.AppGlobalOwnerId == null && oUser_MSTR.ChurchBodyId == null ? "000000" : oUser_MSTR.ChurchBody?.GlobalChurchCode;
                var dt = DateTime.Now;
                var uniqueCode = cc + "-" + vCode + "-" + dt.Year + string.Format("{0:D2}", dt.Month) + string.Format("{0:D2}", dt.Day);
                    var strHashVerifCode = AppUtilties.ComputeSha256Hash(uniqueCode); // "12345678"; // TempData["oVmAuthCode"] = vCode; TempData.Keep();
                    oUserResetModel.SentVerificationCode = strHashVerifCode;
                    ///
                    var msgSubject = "RHEMA-CMS: User Account Verification";
                    var userMess = "<span> Hello " + oUser_MSTR.UserDesc + ",  </span><p>";
                userMess += "<span class='text-success'> Church code: " + cc + "</span><p>";
                userMess += "<span class='text-success'> Username: " + oUser_MSTR.Username + "</span><p><p>";

                userMess += "<span> Please find code for account verification below: </span><p><p>"; 
                    userMess += "<h2 class='text-success'>" + vCode + "</h2><br /><hr /><br />";

                //yyyy-MM-dd hh:mm tt
                   
                    var ds = new TimeSpan(1, 0, 0, 0);
                    dt = dt.Add(ds);
                //var ts = new TimeSpan(0, 0, 0);
                var ts = "12:00 a.m.";   // dt.ToShortTimeString();     String.Format("{0:h:mm tt}", tm)
                    userMess += "<span class='text-info text-md'> Please note: Code is only valid until midnight " + String.Format("{0:dddd dd MMM, yyyy}", dt) + " " + ts + " </span><p><p>";  // 12:00a.m.
                    //userMess += "<span class='text-info text-md'> Please note: Code is only valid until midnight " + dt.ToString("dddd dd MMM, yyyy", CultureInfo.InvariantCulture) + " 12:00a.m. </span><p><p>";  // 
                userMess += "<span class='text-info'> Thanks </span>";
                    userMess += "<span class='text-info'> RHEMA-CMS Team (Ghana) </span>";

                    listToAddr.Add(new MailAddress(oUser_MSTR.Email, oUser_MSTR.UserDesc));
                    var res = AppUtilties.SendEmailNotification("RHEMA-CMS", msgSubject, userMess, listToAddr, listCcAddr, listBccAddr, null, true);

                    //var res = AppUtilties.SendEmailNotification("RHEMA-CMS", "User Account Authentication", "Hello " + oUser_MSTR.UserDesc + ", " +
                    //    Environment.NewLine + "Please find Authentication Code for account confirmation: " + vCode, listToAddr, listCcAddr, listBccAddr, null);
                }

                userDenom = setIndex == 1 ? "Vendor Admin" : (oUser_MSTR.ChurchBody != null ? oUser_MSTR.ChurchBody.Name + "-" : "") + (oUser_MSTR.ChurchBody != null ? oUser_MSTR.AppGlobalOwner.OwnerName + "-" : "");
                _userTask = "Attempt to change pwd. Retrieved user account - " + oUser_MSTR.Username + " [" + userDenom + "]";
               _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                     setIndex == 1 ? "RCMS-Admin:" : "RCMS-Client:" + " User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, oUserId_Logged, _tm, _tm, oUserId_Logged, oUserId_Logged));
                //
                oUserResetModel.strLogUserDesc = (userDenom + ": " + oUser_MSTR.UserDesc).ToUpper();

                //
                //var _oUserResetModel = Newtonsoft.Json.JsonConvert.SerializeObject(oUserResetModel);
                //TempData["oVmCurrMod"] = _oUserResetModel; TempData.Keep();

                return View(oUserResetModel); //  PartialView("_AddOrEdit_UP_ChangePwd", oUserResetModel);
           // }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit_ChangeUserPwd(ResetUserPasswordModel vm)
        {
           // UserProfile _oChanges;  // = vm .oUserProfile;
            if (vm == null) 
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "User account was not found.", signOutToLogIn = false });

            // var vmSave = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as ResetUserPasswordModel : vm; TempData.Keep();
 

            if (string.IsNullOrEmpty(vm.ChurchCode) || string.IsNullOrEmpty(vm.Username) || string.IsNullOrEmpty(vm.NewPassword))
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "Please enter church code, username and password and try again.", signOutToLogIn = false });

            // trim leading /trailing spaces... 
            if (!string.IsNullOrEmpty(vm.Username)) vm.Username = vm.Username.Trim();
            if (!string.IsNullOrEmpty(vm.CurrentPassword)) vm.NewPassword = vm.NewPassword.Trim(); 
             

            // default (vendor) church code - ******
            const string ac1 = "91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203";
            /// compare...
            var h_pwd = AppUtilties.ComputeSha256Hash(vm.ChurchCode); // church code checked
            string strUserKeyHashedData = AppUtilties.ComputeSha256Hash(vm.ChurchCode + vm.Username.Trim().ToLower()); 
            var _oChanges = _context.UserProfile.AsNoTracking() //.Include(t => t.AppGlobalOwner).Include(t => t.ChurchBody)
                            .Where(x => ((h_pwd == ac1 && x.AppGlobalOwnerId==null && x.ChurchBodyId==null) || x.ChurchBody.GlobalChurchCode == vm.ChurchCode) && 
                                    x.ProfileScope == vm.profileScope && x.UserStatus == "A" &&
                                x.Username.Trim().ToLower() == vm.Username.Trim().ToLower() && x.UserKey==strUserKeyHashedData).FirstOrDefault();
            
            if (_oChanges == null)
                return Json(new { taskSuccess = false, oCurrId = -1, userMess = "User account was not found. Please refresh and try again.", signOutToLogIn = false });


            //// trim leading /trailing spaces...
            //if (!string.IsNullOrEmpty(_oChanges.Username)) _oChanges.Username = _oChanges.Username.Trim();


            //var arrData = "";
            //arrData = TempData.ContainsKey("oVmCurrMod") ? TempData["oVmCurrMod"] as string : arrData;
            //vm = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<ResetUserPasswordModel>(arrData) : vm;

            //var oUP = _context.UserProfile.Where(c => c.Username == vm.Username).FirstOrDefault();  // vm.oUserProfile;
            //oUP.ChurchBody = vm.oChurchBody;

            try
            { 
                ModelState.Remove("oAppGlolOwnId");
                ModelState.Remove("oChurchBodyId");
                ModelState.Remove("ChurchCode");
                ModelState.Remove("Username");
                ModelState.Remove("CurrentPassword");
                ModelState.Remove("NewPassword");
                ModelState.Remove("RepeatPassword");
                

                //finally check error state...
                if (ModelState.IsValid == false)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed to load the data to save. Please refresh and try again.", signOutToLogIn = false });

                //var userProList = (from t_upx in _context.UserProfile.Where(c => c.AppGlobalOwnerId == _oChanges.AppGlobalOwnerId && c.ChurchBodyId == _oChanges.ChurchBodyId &&
                //                                                                     c.ProfileScope == _oChanges.ProfileScope && c.Id == _oChanges.Id)
                //                       //  from t_upr in _context.UserProfileRole.Where(c => c.ChurchBodyId == t_upx.ChurchBodyId && c.UserProfileId == t_upx.Id).DefaultIfEmpty()
                //                       //  from t_ur in _context.UserRole.Where(c => c.ChurchBodyId == t_upx.ChurchBodyId && c.Id == t_upr.UserRoleId && c.RoleLevel == 2 && c.RoleType == "SUP_ADMN").DefaultIfEmpty()
                //                   select t_upx
                //                  ).OrderBy(c => c.UserDesc).ToList();

                //if (userProList.Count == 0)
                //    return Json(new { taskSuccess = false, oCurrId = oUP.Id, userMess = "User account was not found. Please refresh and try again.", signOutToLogIn = false });

                //if (vm.AuthTypeUsed == null)
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please indicate authentication type to confirm user profile.", signOutToLogIn = false });

                vm.AuthTypeUsed = 1;
                if (vm.AuthTypeUsed == 1)  //2-way
                {
                    if (vm.VerificationCode == null)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Verify account. Please enter the verification code received.", signOutToLogIn = false });

                   // var tempPwd = CodeGenerator.GenerateCode();  // make it more unique... 000000-BZWER09J-20210405
                    var uniqueCode = vm.ChurchCode + "-" + vm.VerificationCode + "-" + DateTime.Now.Year + string.Format("{0:D2}", DateTime.Now.Month) + string.Format("{0:D2}", DateTime.Now.Day);
                    var strHashVerifCode = AppUtilties.ComputeSha256Hash(uniqueCode); // "12345678"; // TempData["oVmAuthCode"] = vCode; TempData.Keep();
                   // oUserResetModel.SentVerificationCode = strHashVerifCode;
                     
                    if (vm.SentVerificationCode != strHashVerifCode) // AppUtilties.ComputeSha256Hash(vm.VerificationCode)) //"12345678") // latest code sent to user's email, sms
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Enter correct verification code. Hint: Code sent may have expired. Get new verification code to verify.", signOutToLogIn = false });


                    //if (vm.SentVerificationCode != AppUtilties.ComputeSha256Hash(vm.VerificationCode)) //"12345678") // latest code sent to user's email, sms
                    //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Enter correct verification code.", signOutToLogIn = false });
                }

                //else
                //{
                //    var _secAns = AppUtilties.ComputeSha256Hash(vm.SecurityQue + vm.SecurityAns);
                //    if (vm.SecurityQue.ToLower().Equals(vm.SecurityQue.ToLower()) && vm.SecurityAns.Equals(_secAns))
                //        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Security answer provided is not correct.", signOutToLogIn = false });
                //}


                if (_oChanges.Expr != null)
                {
                    if (_oChanges.Expr.Value <= DateTime.Now.Date)
                        return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please user account has expired. Activate account first.", signOutToLogIn = false });
                }

                //if (_oChanges.Pwd != AppUtilties.ComputeSha256Hash(vm.CurrentPassword))
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide correct current password", signOutToLogIn = false });

                //if (vm.CurrentPassword == null)
                //    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide current password", signOutToLogIn = false });

                if (vm.NewPassword == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Provide new password (Hint: minimum 6-charaters; use strong passwords:- UPPER and lower cases, digits (0-9) and $pecial characters)", signOutToLogIn = false });

                if (vm.NewPassword.Length < 6)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Password length must be 6 characters or more. (Hint: minimum 6-charaters; use strong passwords:- UPPER and lower cases, digits (0-9) and $pecial characters)", signOutToLogIn = false });

                if (vm.RepeatPassword == null)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Please repeat your password typed. (Hint: minimum 6-digit; use strong passwords:- UPPER and lower cases, digits (0-9) and $pecial characters)", signOutToLogIn = false });

                if (vm.NewPassword != vm.RepeatPassword)
                    return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Password mismatch. Please try again:  put in your new password.", signOutToLogIn = false });


                var _userTask = "Attempted to changed user password."; var strUserDenom = "Vendor Admin";
                //using (var _pwdCtx = new MSTR_DbContext(_context.Database.GetDbConnection().ConnectionString))
                //{
                    var cc = "";
                    if (vm.ChurchCode == "000000" && _oChanges.AppGlobalOwnerId == null && _oChanges.ChurchBodyId == null && _oChanges.ProfileScope == "V")
                    {
                        cc = "000000";    //var churchCode = "000000"; _oChanges.Username = "SysAdmin"; _oChanges.Pwd = "$ys@dmin1";  
                    }
                    else
                    {
                        var oAGO = _context.MSTRAppGlobalOwner.Find(vm.oAppGloOwnId);
                        var oCB = _context.MSTRChurchBody.AsNoTracking().Where(c => c.AppGlobalOwnerId == vm.oAppGloOwnId && c.Id == vm.oChurchBodyId).FirstOrDefault();

                        if (oAGO == null || oCB == null)
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Specified denomination and church unit could not be retrieved. Please refresh and try again.", signOutToLogIn = false });
                      
                        if (string.IsNullOrEmpty(oCB.GlobalChurchCode))
                            return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Church code not specified. The unique global church code of church unit is required. Please verify with System Admin and try again.", signOutToLogIn = false });

                       
                        strUserDenom = oCB.Name + (!string.IsNullOrEmpty(oAGO.Acronym) ? ", " + oAGO.Acronym : oAGO.OwnerName);
                        strUserDenom = (!string.IsNullOrEmpty(strUserDenom) ? "Denomination: " + strUserDenom : strUserDenom);  // "--" + 

                        cc = oCB.GlobalChurchCode;

                        // _oChanges.Pwd = AppUtilties.ComputeSha256Hash(_oChanges.Username + _oChanges.Pwd);
                    }
                     

                    //create user and init...
                    // _oChanges = new UserProfile();
                    //_oChanges.AppGlobalOwnerId = null; // oUP.ChurchBody != null ? oUP.ChurchBody.AppGlobalOwnerId : null;
                    //_oChanges.ChurchBodyId = null; //(int)oUP.ChurchBody.Id;
                    //_oChanges.OwnerId =null; // (int)vm.oCurrLoggedUserId;


                    var tm = DateTime.Now;
                    _oChanges.Strt = tm;
                    //_oChanges.Expr = null; // tm.AddDays(90);  //default to 30 days
                    //  oCurrvm.oUserProfile.UserId = oCurrChuMemberId_LogOn;
                    //_oChanges.ChurchMemberId = null; // vm.oCurrLoggedMemberId;
                    // _oChanges.UserScope = "E"; // I-internal, E-external
                    //_oChanges.ProfileScope = "V"; // V-Vendor, C-Client

                    _oChanges.UserKey = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower());
                    //_oChanges.Pwd = "123456";  //temp pwd... to reset @ next login 

                    _oChanges.Pwd = AppUtilties.ComputeSha256Hash(cc + _oChanges.Username.Trim().ToLower() + vm.NewPassword);

                    _oChanges.ResetPwdOnNextLogOn = false;
                    _oChanges.PwdExpr = tm.AddDays(30);  //default to 90 days 
                    _oChanges.UserStatus = "A"; // A-ctive...D-eactive

                    // _oChanges.Created = tm;
                    _oChanges.LastMod = tm; 
                    //  _oChanges.CreatedByUserId = null; // (int)vm.oCurrLoggedUserId;
                    _oChanges.LastModByUserId = null; // (int)vm.oCurrLoggedUserId;

                    //cc + model.Username.Trim().ToLower() + model.Password
                
                    //if (vm.ChurchCode == "000000")
                    //    _oChanges.Pwd = AppUtilties.ComputeSha256Hash(vm.ChurchCode + vm.Username.ToLower() + vm.NewPassword);
                    // else
                    //    _oChanges.Pwd = AppUtilties.ComputeSha256Hash(vm.Username + vm.NewPassword);
                 

                    if (vm.AuthTypeUsed == 2)
                    {
                        _oChanges.PwdSecurityQue = vm.SecurityQue;
                        _oChanges.PwdSecurityAns = vm.SecurityAns != null ? AppUtilties.ComputeSha256Hash(vm.SecurityAns) : vm.SecurityAns;
                    }

                    //_oChanges.UserDesc = "Super Admin";
                    //_oChanges.UserPhoto = null;
                    //_oChanges.UserId = null;
                    //_oChanges.PhoneNum = null;
                    //_oChanges.Email = null; 

                    //  

                    _userTask = "Changed user password successfully - " + _oChanges.Username + strUserDenom;
                   // var _userTask = "Changed user password successfully.";
                   ViewBag.UserMsg = "Password changed successfully.";


                    //save everything
                    //  await _context.SaveChangesAsync();

                    _context.UserProfile.Update(_oChanges);

                    //save everything
                    _context.SaveChanges(); // await _pwdCtx.SaveChangesAsync();


                //    DetachAllEntities(_pwdCtx);
                //}

                //audit...
                var _tm = DateTime.Now;
                _ = this.LogUserActivity_AppMainUserAuditTrail(new UserAuditTrail(0, null, null, "T",
                                 "RCMS-Admin: User Profile", AppUtilties.GetRawTarget(HttpContext.Request), _userTask, _tm, vm.oCurrUserId_Logged, _tm, _tm, vm.oCurrUserId_Logged, vm.oCurrUserId_Logged));


                var _vm = Newtonsoft.Json.JsonConvert.SerializeObject(vm);
                TempData["oVmCurr"] = _vm; TempData.Keep();

                return Json(new { taskSuccess = true, oCurrId = _oChanges.Id, userMess = ViewBag.UserMsg, signOutToLogIn = true });
            }

            catch (Exception ex)
            {
                return Json(new { taskSuccess = false, oCurrId = _oChanges.Id, userMess = "Failed saving user profile details. Contact System Admin for assistance. Err: " + ex.Message, signOutToLogIn = false }); 
            }
        }
         

    }
}
