
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using RhemaCMS.Models.ViewModels.vm_adhc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks; 


namespace RhemaCMS.Controllers.con_adhc
{

    public static class AppUtilties_Static
    {
        public static Task<List<T>> ToListAsync<T>(this List<T> list) //IQueryable
        {
            return Task.Run(() => list.ToList());
        }
    }

    public static class StackAppUtilties
    { 
        public static bool IsAjaxRequest(this HttpRequest request)  //(this HttpRequest request)
        {
            if (request == null)
                return false; // new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return !string.IsNullOrEmpty(request.Headers["X-Requested-With"]) &&
                    string.Equals(
                        request.Headers["X-Requested-With"],
                        "XmlHttpRequest",
                        StringComparison.OrdinalIgnoreCase);

            return false;
        }
    }
 

    public class AppUtilties
    {
        public AppUtilties() { }

         

        public static ChurchModelContext GetNewDBContext_CL(MSTR_DbContext dbContext, string dbName, string dbServer = null)
        {
            var conn = new SqlConnectionStringBuilder(dbContext.Database.GetDbConnection().ConnectionString);
            conn.InitialCatalog = dbName;
            if (dbServer != null)
                conn.DataSource = dbServer;
            // dbContext.Database.GetDbConnection().ConnectionString = conn.ConnectionString;
            ///
            var customConn = new ChurchModelContext(conn.ConnectionString);
            return customConn;
        }

        //public static ChurchModelContext GetNewDBContext_CL(ChurchModelContext dbContext)
        //{
        //    var customConn = new ChurchModelContext(dbContext.Database.GetDbConnection().ConnectionString);
        //    return new ChurchModelContext(dbContext.Database.GetDbConnection().ConnectionString);
        //}
        public static MSTR_DbContext GetNewDBCtxConn_MS(Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            try
            {
                var _cs = _configuration["ConnectionStrings:DefaultConnection"];
                var _masterContext = new MSTR_DbContext(_cs);
                if (_masterContext.Database.CanConnect()) return _masterContext;


                return null;
            }
            catch (Exception ex)
            {
                return null;
            } 
        }

        public static string GetNewDBConnString_MS(Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            try
            {
                var _cs = _configuration["ConnectionStrings:DefaultConnection"];
                var _masterContext = new ChurchModelContext(_cs);
                if (_masterContext.Database.CanConnect()) return _cs;

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static string GetNewDBConnString_CL(  MSTR_DbContext dbContext_MSTR, /// ChurchModelContext dbContext, 
            Microsoft.Extensions.Configuration.IConfiguration _configuration, int? oAGOid)
        {
           // var customConn = new ChurchModelContext(dbContext_MSTR.Database.GetDbConnection().ConnectionString);
           // return new ChurchModelContext(dbContext.Database.GetDbConnection().ConnectionString);



            // Get the client database details.... db connection string                        
            var oClientConfig = dbContext_MSTR.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == oAGOid && c.Status == "A").FirstOrDefault();
            // ac1... if (oClientConfig == null) oClientConfig = _context.ClientAppServerConfig.AsNoTracking().Where(c => c. == oUser_MSTR.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
            ///
            if (oClientConfig == null)
            {
                //ModelState.AddModelError("", "Client database details not found. Please try again or contact System Administrator"); //model.IsVal = 0; 
                return null;
            }

            ///// first get the session value.... before
            ///// 
            //var arrData = "";
            //arrData = TempData.ContainsKey("_strCLConnStr") ? TempData["_strCLConnStr"] as string : arrData;
            //var vmMod = (!string.IsNullOrEmpty(arrData)) ? Newtonsoft.Json.JsonConvert.DeserializeObject<MemberBioModel>(arrData) : vm;

            //var _cs = ""; /// strTempConn;  /// , MSTR_DbContext currContext = null, string strTempConn = ""
            //if (string.IsNullOrEmpty(_cs))

             var  _cs = _configuration["ConnectionStrings:conn_CLNT"];

            //_cs = _configuration.GetConnectionString("DefaultConnection");


            // get and mod the conn
            var _clientDBConnString = "";
            var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
            conn.DataSource = oClientConfig.ServerName.Contains("\\\\") ? oClientConfig.ServerName.Replace("\\\\","\\") : oClientConfig.ServerName; 
            conn.InitialCatalog = oClientConfig.DbaseName;
            conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
            /// conn.IntegratedSecurity = false; 
            conn.MultipleActiveResultSets = true; //conn.TrustServerCertificate = true;

            _clientDBConnString = conn.ConnectionString;

            // test the NEW DB conn 

            var _clientContext = new ChurchModelContext(_clientDBConnString);

            //try
            //{
            //    var b = _clientContext.Database.CanConnect();
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            if (_clientContext.Database.CanConnect()) return _clientDBConnString;
            //{
            //    // ModelState.AddModelError("", "Client validation failed. Failed to connect client database. Please try again or contact System Administrator"); //model.IsVal = 0;
                 
            //}

            return null; 
        }
         
        public static ChurchModelContext GetNewDBCtxConn_CL(MSTR_DbContext dbContext_MSTR, /// ChurchModelContext dbContext, 
            Microsoft.Extensions.Configuration.IConfiguration _configuration, int? oMSTRAGOid)
        { 
            // Get the client database details.... db connection string                        
            var oClientConfig = dbContext_MSTR.ClientAppServerConfig.AsNoTracking().Where(c => c.AppGlobalOwnerId == oMSTRAGOid && c.Status == "A").FirstOrDefault();
            // ac1... if (oClientConfig == null) oClientConfig = _context.ClientAppServerConfig.AsNoTracking().Where(c => c. == oUser_MSTR.AppGlobalOwnerId && c.Status == "A").FirstOrDefault();
            ///
            if (oClientConfig == null) return null; 

            var _cs = _configuration["ConnectionStrings:conn_CLNT"];

            //_cs = _configuration.GetConnectionString("DefaultConnection");


            // get and mod the conn
            var _clientDBConnString = "";
            var conn = new SqlConnectionStringBuilder(_cs); /// this._configuration.GetConnectionString("DefaultConnection") _context.Database.GetDbConnection().ConnectionString
            conn.DataSource = oClientConfig.ServerName.Contains("\\\\") ? oClientConfig.ServerName.Replace("\\\\", "\\") : oClientConfig.ServerName;
            conn.InitialCatalog = oClientConfig.DbaseName;
            conn.UserID = oClientConfig.SvrUserId; conn.Password = oClientConfig.SvrPassword;
            /// conn.IntegratedSecurity = false; 
            conn.MultipleActiveResultSets = true; //conn.TrustServerCertificate = true;

            _clientDBConnString = conn.ConnectionString;

            // test the NEW DB conn 

            var _clientContext = new ChurchModelContext(_clientDBConnString);

            //try
            //{
            //    var b = _clientContext.Database.CanConnect();
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

            if (_clientContext.Database.CanConnect()) return _clientContext; 

            return null;
        }


        public static MSTR_DbContext GetNewDBContext_MS(MSTR_DbContext dbContext, string dbName, string dbServer = null)
        {
            var conn = new SqlConnectionStringBuilder(dbContext.Database.GetDbConnection().ConnectionString);
            //  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
            conn.InitialCatalog = dbName; // conn.DataSource = dbServer; conn.UserID = "sa"; conn.Password = "sa"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
            
            ///
            if (dbServer != null)
                conn.DataSource = dbServer;
            // dbContext.Database.GetDbConnection().ConnectionString = conn.ConnectionString;
            ///
            var customConn = new MSTR_DbContext(conn.ConnectionString);
            //  "DefaultConnection": "Server=RHEMA-SDARTEH;Database=DBRCMS_MS_DEV;User Id=sa;Password=sadmin;Trusted_Connection=True;MultipleActiveResultSets=true"
            // conn.InitialCatalog = "DBRCMS_CL_TEST";  conn.DataSource = "RHEMA-SDARTEH"; conn.UserID = "sa"; conn.Password = "sadmin"; conn.MultipleActiveResultSets = true; conn.TrustServerCertificate = true;
            ///
            customConn.Database.GetDbConnection().ConnectionString = conn.ConnectionString;
            return customConn;
        }

        public static MSTR_DbContext GetNewDBContext_MS(MSTR_DbContext dbContext)
        { 
            var customConn = new MSTR_DbContext(dbContext.Database.GetDbConnection().ConnectionString);
            return new MSTR_DbContext(dbContext.Database.GetDbConnection().ConnectionString);
        }

        public static bool IsAncestor_ChurchBody(ChurchBody oAncestorChurchBody, ChurchBody oCurrChurchBody)
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

        public static bool IsAncestor_ChurchBody(string strAncestorRootCode, string strCurrChurchBodyRootCode, int? ancestorChurchBodyId = null, int? currChurchBodyId = null)
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

        public static bool IsDescendant_ChurchBody(ChurchBody oDescendantChurchBody, ChurchBody oCurrChurchBody)  // Ancestor of ? ... Taifa -- Grace. swapped -->> Descendant of ?
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

        public static bool IsDescendant_ChurchBody(string strDescendantRootCode, string strCurrChurchBodyRootCode, int? descendantChurchBodyId = null, int? currChurchBodyId = null)
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

        public static string GetConcatMemberName(string title, string fn, string mn, string ln, bool displayName = false)
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

        public static string GetConcatLinkedEntities(string entitySub, string entityMain)
        {
            var strDesig = ((!string.IsNullOrEmpty(entitySub) && !string.IsNullOrEmpty(entityMain) ?
                                                        entitySub + ", " + entityMain : entitySub + entityMain).Trim());
            return strDesig;
        }

         

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        ///// <summary>	/// Gets the raw target of an HTTP request.	/// </summary>	/// <returns>Raw target of an HTTP request</returns>	/// <remarks>	/// ASP.NET Core manipulates the HTTP request parameters exposed to pipeline	/// components via the HttpRequest class. This extension method delivers an untainted	/// request target. https://tools.ietf.org/html/rfc7230#section-5.3	/// </remarks>
        //public static string GetRawTarget(this HttpRequest request)
        //{
        //    var httpRequestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>(); return httpRequestFeature.RawTarget;
        //}

        public static string GetRawTarget(HttpRequest request)
        {
            var a = request.Path;
            var b = request.Path.Value; 

            return request.QueryString.Value;

            //var httpRequestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>(); 
            //return httpRequestFeature.RawTarget;
        }

        public static string Pluralize(string strWord)
        {
            var strPlu = strWord;
            if (strPlu.Substring(strPlu.Length - 1, 1).ToLower() != "s" && strPlu.Substring(strPlu.Length - 3, 3).ToLower() != "es" && strPlu.Substring(strPlu.Length - 3, 3).ToLower() != "ies")
            {
                if (strPlu.Substring(strPlu.Length - 1, 1).ToLower() == "y") { strPlu += "ies"; } else if (strPlu.Substring(strPlu.Length - 1, 1).ToLower() == "ch") { strPlu += "es"; } else { strPlu += "s"; }
            }

            return strPlu;
        }

         

        public static bool CheckUserPrivilege(UserProfileRole oUserProRoleLog)
        {
            //var oUserProRoleLog = TempData.Get<ChurchProData.Models.Authentication.UserProfileRole>("oUserProRoleLogIn");

            if (oUserProRoleLog == null)
                return false;
            else
            {
                if (oUserProRoleLog.UserProfile == null)
                    return false;

                else
                {
                    if (oUserProRoleLog.UserRole == null)
                        return false;

                    else
                    {
                        //ViewBag.oUserLogged = oUserProRoleLog.UserProfile;
                        //ViewBag.oUserProRoleLogged = oUserProRoleLog.UserRole;
                        return true;
                    }
                }
            }
        }


        public static UserSessionPrivilege GetUserPrivilege( MSTR_DbContext _context,  ChurchModelContext _clientContext,  string churchCode, UserProfile oUserProfile)
        {
            //already authenticated... jux the use the user and churchCode... check Active though
            // var _oUserLog_RolePerm = new List<UserPermission>();

            var h_pwd = AppUtilties.ComputeSha256Hash(churchCode);
            // var oUserLog_RolePerm = (
            //                         from a in _context.UserProfile //.Include(u => u.ChurchBody)
            //                         .Where(x => (h_pwd == ac1 || (x.ChurchBody.GlobalChurchCode == churchCode && x.ChurchBodyId == oUserProfile.ChurchBodyId)) && x.Username.Trim().ToLower() == oUserProfile.Username.Trim().ToLower() && x.UserStatus == "A")
            //                         from b in _context.UserProfileRole//.Include(u => u.ChurchBody)
            //                         .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserProfileId == a.Id && x.ProfileRoleStatus == "A")
            //                         from c in _context.UserRolePermission//.Include(u => u.ChurchBody)
            //                         .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserRoleId == b.UserRoleId && x.Status == "A")
            //                         from d in _context.UserPermission//.Include(u => u.ChurchBody)
            //                         .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.Id == c.UserPermissionId && x.PermStatus == "A")

            //                         select d  // c.UserPermissions; 
            //                         ).ToList();


            //// var _oUserLog_GroupPerm = new List<UserPermission>();
            //  var oUserLog_GroupPerm = (
            //                             from a in _context.UserProfile//.Include(u => u.ChurchBody)
            //                             .Where(x => (h_pwd == ac1 || (x.ChurchBody.GlobalChurchCode == churchCode && x.ChurchBodyId==oUserProfile.ChurchBodyId)) && x.Username.Trim().ToLower() == oUserProfile.Username.Trim().ToLower() && x.UserStatus == "A")
            //                             from b in _context.UserProfileGroup//.Include(u => u.ChurchBody)
            //                             .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserProfileId==a.Id && x.Status == "A")   
            //                             from c in _context.UserGroupPermission//.Include(u => u.ChurchBody)
            //                             .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserGroupId== b.UserGroupId && x.Status == "A")
            //                             from d in _context.UserPermission//.Include(u => u.ChurchBody)
            //                             .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.Id == c.UserPermissionId && x.PermStatus == "A")

            //                             select d  // c.UserPermissions; 
            //                         ).ToList();


            // var oUserLogPermAll = new List<UserPermission>();
            // oUserLogPermAll.AddRange(oUserLog_RolePerm);
            // oUserLogPermAll.AddRange(oUserLog_GroupPerm);

         //   var oUserLogPermAll = GetUserAssignedPermissions(_context, _clientContext, churchCode, oUserProfile);
           

            var oUserPrivList = new UserSessionPrivilege(); 

            if (h_pwd == ac1 && // "000000") && 
                oUserProfile.Username == AppUtilties.ComputeSha256Hash("000000") && oUserProfile.Pwd == AppUtilties.ComputeSha256Hash("000000" + "000000" + "$000000"))          
            {  //if (oUserProfile.RootProfileCode == AppUtilties.ComputeSha256Hash(oUserProfile.Username + "RHEMA_Sup_Admn1"))
               // var p = ((JProperty)JObject.FromObject((new UserPermissionLog())._A0__System_Administration).Properties()).Name;
               //  JObject jsonPerm = JObject.FromObject((new UserPermissionLog())._A0__System_Administration);

                var oUserSessionPermList = new List<UserSessionPerm>();
                oUserSessionPermList.Add(new UserSessionPerm()
                {

                    oUserPermissionId = null,
                    PermissionCode = "A0",
                    PermissionName = "_A0__System_Administration",
                   // PermissionValue = true,
                    //
                    ViewPerm = true,
                    CreatePerm = true,
                    EditPerm = true,
                    DeletePerm = true,
                    ManagePerm = true
                     

                });

                //create permission on the fly... DO NOT STORE IN THE DB!           strAppCurrUser_RoleCateg
                oUserPrivList = new UserSessionPrivilege()
                {
                   // AppGlobalOwner = null,
                   // ChurchBody = null,

                    UserProfile = oUserProfile,
                   // UserRoles = null,
                   // UserGroups = null,
                   // UserPermissions = null,
                   // arrAssignedGroupNames = "", // System Administrators
                   // arrAssignedRoleDesc = "System_Administration",
                   // strChurchCode_AGO = "",
                  //  strChurchCode_CB = "",
                    ///
                    UserSessionPermList = oUserSessionPermList
                };  //PermissionName = ((JProperty)jsonPerm.Properties()).Name

                return oUserPrivList;                
            }

            else
            {
                var oUserLogPermAll = GetUserAssigned_SessionPrivileges(_context, churchCode, oUserProfile);            //_clientContext, 
                return oUserLogPermAll;

                //if (oUserLogPermAll.Count > 0)   //(oChurchBody != null && 
                //{
                //    var mainCode = "";
                //    if (churchCode.Contains("_")) mainCode = churchCode.Substring(0, churchCode.IndexOf("-"));
                //    var oAppGloOwn = _context.AppGlobalOwner.Where(x => x.RootChurchCode == mainCode).FirstOrDefault(); //PCG, ICGC, RCM1, RCM2 etc  // ... RCM-0000000, RCM-0000001, PCG-1234567, COP-1000000, ICGC-9999999
                //    var oChurchBody = _context.MSTRChurchBody.Where(x => x.GlobalChurchCode == churchCode).FirstOrDefault(); //PCG-000000  .Include(t=>t.ChurchLevel)  //.Include(t => t.AppGlobalOwner)  //ChurchBody MUST also have been subscribed or renewed...

                //    foreach (var oPerm in oUserLogPermAll)
                //    {
                //        oUserPrivList.Add(new UserSessionPrivilege
                //        {
                //            AppGlobalOwner = oAppGloOwn,
                //            ChurchBody = oChurchBody,
                //            UserProfile = oUserProfile,
                //            UserPermission = oPerm,
                //            PermissionName = oPerm.PermissionName,
                //            PermissionValue = true
                //        });
                //    }

                //    return oUserPrivList;
                //}
            }                   
        
            //all else...
           // return null;

            //UserProfileRole oUserProRoleLog = _context.UserProfileRole
            //        .Include(u => u.ChurchBody)
            //       //.Include(u => u.UserProfile)
            //       //.Include(u => u.UserRole)
            //       //.Include(t => t.UserProfile.ChurchBody)
            //       .Where(c => c.ChurchBody.ChurchCode == churchCode && c.UserProfileId == oUserProfile.Id && c.ProfileRoleStatus == "A").FirstOrDefault();

            //// UserProfile oUserLogged = oUserLog[0].oUser; UserProfileRole oUserRoleLogged = oUserLog[0].oUserRole;

            //if (oUserProRoleLog != null)
            //{
            //    UserRole oUserRoleLog = _context.UserRole.Where(c => c.Id == oUserProRoleLog.UserRoleId && c.RoleStatus == "A").FirstOrDefault();
            //    //HttpContext.Session.SetObjectAsJson("oUserLogged", oUserLog);
            //    //HttpContext.Session.SetObjectAsJson("oUserProRoleLogged", oUserProRoleLog);

            //    oUserProRoleLog.UserProfile = oUserLog;
            //    oUserProRoleLog.UserRole = oUserRoleLog;                
            //} 
        }

        public static UserSessionPrivilege GetUserPrivilege(MSTR_DbContext _context, string churchCode, UserProfile oUserProfile)
        {
            //already authenticated... jux the use the user and churchCode... check Active though
            // var _oUserLog_RolePerm = new List<UserPermission>();
            const string def_init_hash = "10c16e2d260b87e96096c18991b57d9233453ae4eb3125ed0e34ecde2af3fa36";
            const string def_initkey_hash = "d38e8e28f06fbd35e89e67ea132da62c976af6dff36e02877d2236b6a12961ca"; 

            var h_pwd = AppUtilties.ComputeSha256Hash(churchCode);
             
            var oUserLogPrivSYS = new UserSessionPrivilege();

            //var a = h_pwd == ac1;
            //var b = string.Compare(oUserProfile.Username, "sys", true) == 0;
            //var c = oUserProfile.UserKey == def_initkey_hash;
            //var d = oUserProfile.Pwd == def_init_hash;

            if (h_pwd == ac1 && string.Compare(oUserProfile.Username, "sys", true)==0 &&   //AppUtilties.ComputeSha256Hash("000000")
                oUserProfile.UserKey == def_initkey_hash && oUserProfile.Pwd == def_init_hash) //  AppUtilties.ComputeSha256Hash("000000" + "000000" + "$000000"))
            {  //if (oUserProfile.RootProfileCode == AppUtilties.ComputeSha256Hash(oUserProfile.Username + "RHEMA_Sup_Admn1"))
               // var p = ((JProperty)JObject.FromObject((new UserPermissionLog())._A0__System_Administration).Properties()).Name;
               //  JObject jsonPerm = JObject.FromObject((new UserPermissionLog())._A0__System_Administration);

                var oUtil = new AppUtilties();    // (c.PermissionCode == "A0_00" || c.PermissionCode == "A0_01")
                var oPermListSYS = oUtil.GetSystem_Administration_Permissions().Where(c=> c.PermissionCode == "A0_00" || c.PermissionCode == "A0_01").ToList();
                var oRoleSYS = oUtil.GetSystemDefaultRoles().Where(c => c.RoleType == "SYS").FirstOrDefault();
               // var oGroupSYS = oUtil.GetSystemDefaultGroups().Where(c => c.GroupType == "SYS").FirstOrDefault();
                ///
                if (oPermListSYS.Count==0 || oRoleSYS == null)
                    return null;

                var oUserSessionPermList = new List<UserSessionPerm>();
                foreach(var oPermSYS in oPermListSYS)
                {
                    oUserSessionPermList.Add(new UserSessionPerm()
                    {
                        oUserPermissionId = null,
                        PermissionCode = oPermSYS.PermissionCode, // "A0",
                        PermissionName = oPermSYS.PermissionName, //"_A0__System_Administration",
                       // PermissionValue = true,
                        //
                        UserRole = oRoleSYS,
                        // UserGroup = oGroupSYS,
                        UserPermission = oPermSYS,
                        //
                        ViewPerm = true,
                        CreatePerm = true,
                        EditPerm = true,
                        DeletePerm = true,
                        ManagePerm = true
                    });


                }
                

                //create permission on the fly... DO NOT STORE IN THE DB!           strAppCurrUser_RoleCateg
                //var arrMods = new List<string>();
                //arrMods.Add("A0");

                oUserLogPrivSYS = new UserSessionPrivilege()
                {
                    //AppGlobalOwner = null,
                    //ChurchBody = null,
                    UserProfile = oUserProfile,                    
                    IsModAccessVAA0 = true,

                    UserRoles = new List<UserRole>(),
                    UserGroups = new List<UserGroup>(),
                    UserPermissions = new List<UserPermission>(),

                    arrAssignedModCodes = new List<string>(),
                    arrAssignedRoleCodes = new List<string>(),
                    arrAssignedRoleNames = new List<string>(),
                    arrAssignedGroupCodes = new List<string>(),
                    arrAssignedGroupNames = new List<string>(),
                    arrAssignedPermCodes = new List<string>(),

                    //UserRoles = null,
                    //UserGroups = null,
                    //UserPermissions = null,
                    //arrAssignedGroupsDesc= "", // System Administrators
                    //arrAssignedRolesDesc = "System_Administration",
                    //strChurchCode_AGO = "",
                    //strChurchCode_CB = "",
                    ///
                    UserSessionPermList = oUserSessionPermList
                };  //PermissionName = ((JProperty)jsonPerm.Properties()).Name



                // add other user details
                oUserLogPrivSYS.arrAssignedModCodes.Add("A0");

                oUserLogPrivSYS.UserRoles.Add(oRoleSYS);  
                oUserLogPrivSYS.arrAssignedRoleCodes.Add(oRoleSYS.RoleType);
                oUserLogPrivSYS.arrAssignedRoleNames.Add(oRoleSYS.RoleName);

                //oUserLogPrivSYS.UserGroups.Add(oGroupSYS); oUserLogPrivSYS.arrAssignedGroupCodes.Add(oGroupSYS.GroupType); oUserLogPrivSYS.arrAssignedGroupNames.Add(oGroupSYS.GroupName);

                foreach (var oPermSYS in oPermListSYS)
                {
                    oUserLogPrivSYS.UserPermissions.Add(oPermSYS);
                    oUserLogPrivSYS.arrAssignedPermCodes.Add(oPermSYS.PermissionCode);
                }
                 

                return oUserLogPrivSYS;
            }

            else
            {
                var oUserLogPermAll = GetUserAssigned_SessionPrivileges(_context, churchCode, oUserProfile);


                return oUserLogPermAll;

                //if (oUserLogPermAll.Count > 0)   //(oChurchBody != null && 
                //{
                //    var mainCode = "";
                //    if (churchCode.Contains("_")) mainCode = churchCode.Substring(0, churchCode.IndexOf("-"));
                //    var oAppGloOwn = _context.AppGlobalOwner.Where(x => x.RootChurchCode == mainCode).FirstOrDefault(); //PCG, ICGC, RCM1, RCM2 etc  // ... RCM-0000000, RCM-0000001, PCG-1234567, COP-1000000, ICGC-9999999
                //    var oChurchBody = _context.MSTRChurchBody.Where(x => x.GlobalChurchCode == churchCode).FirstOrDefault(); //PCG-000000  .Include(t=>t.ChurchLevel)  //.Include(t => t.AppGlobalOwner)  //ChurchBody MUST also have been subscribed or renewed...

                //    foreach (var oPerm in oUserLogPermAll)
                //    {
                //        oUserPrivList.Add(new UserSessionPrivilege
                //        {
                //            AppGlobalOwner = oAppGloOwn,
                //            ChurchBody = oChurchBody,
                //            UserProfile = oUserProfile,
                //            UserPermission = oPerm,
                //            PermissionName = oPerm.PermissionName,
                //            PermissionValue = true
                //        });
                //    }

                //    return oUserPrivList;
                //}
            }

            //all else...
            // return null;

            //UserProfileRole oUserProRoleLog = _context.UserProfileRole
            //        .Include(u => u.ChurchBody)
            //       //.Include(u => u.UserProfile)
            //       //.Include(u => u.UserRole)
            //       //.Include(t => t.UserProfile.ChurchBody)
            //       .Where(c => c.ChurchBody.ChurchCode == churchCode && c.UserProfileId == oUserProfile.Id && c.ProfileRoleStatus == "A").FirstOrDefault();

            //// UserProfile oUserLogged = oUserLog[0].oUser; UserProfileRole oUserRoleLogged = oUserLog[0].oUserRole;

            //if (oUserProRoleLog != null)
            //{
            //    UserRole oUserRoleLog = _context.UserRole.Where(c => c.Id == oUserProRoleLog.UserRoleId && c.RoleStatus == "A").FirstOrDefault();
            //    //HttpContext.Session.SetObjectAsJson("oUserLogged", oUserLog);
            //    //HttpContext.Session.SetObjectAsJson("oUserProRoleLogged", oUserProRoleLog);

            //    oUserProRoleLog.UserProfile = oUserLog;
            //    oUserProRoleLog.UserRole = oUserRoleLog;                
            //} 
        }



        public static List<UserPermission> GetUserAssignedPermissions(MSTR_DbContext _context, string churchCode, UserProfile oUserProfile)   //ChurchModelContext _clientContext, 
        {
            //already authenticated... jux the use the user and churchCode... check Active though
            // var _oUserLog_RolePerm = new List<UserPermission>();

            var h_pwd = AppUtilties.ComputeSha256Hash(churchCode);
            var oUserLog_RolePerm = (
                                    from a in _context.UserProfile.AsNoTracking() //.Include(u => u.ChurchBody)
                                    .Where(x => (h_pwd == ac1 || (x.ChurchBody.GlobalChurchCode == churchCode && x.ChurchBodyId == oUserProfile.ChurchBodyId)) && x.Username.Trim().ToLower() == oUserProfile.Username.Trim().ToLower() && x.UserStatus == "A")
                                    from b in _context.UserProfileRole.AsNoTracking() //.Include(u => u.ChurchBody)
                                    .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserProfileId == a.Id && x.ProfileRoleStatus == "A")
                                    from c in _context.UserRolePermission.AsNoTracking() //.Include(u => u.ChurchBody)
                                    .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserRoleId == b.UserRoleId && x.Status == "A")
                                    from d in _context.UserPermission.AsNoTracking() //.Include(u => u.ChurchBody)
                                    .Where(x => x.Id == c.UserPermissionId && x.PermStatus == "A")  //&& (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId 

                                    select d  // c.UserPermissions; 
                                    ).ToList();


            // var _oUserLog_GroupPerm = new List<UserPermission>();
            var oUserLog_GroupPerm = (
                                       from a in _context.UserProfile.AsNoTracking() //.Include(u => u.ChurchBody)
                                       .Where(x => (h_pwd == ac1 || (x.ChurchBody.GlobalChurchCode == churchCode && x.ChurchBodyId == oUserProfile.ChurchBodyId)) && x.Username.Trim().ToLower() == oUserProfile.Username.Trim().ToLower() && x.UserStatus == "A")
                                       from b in _context.UserProfileGroup.AsNoTracking() //.Include(u => u.ChurchBody)
                                       .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserProfileId == a.Id && x.Status == "A")
                                       from c in _context.UserGroupPermission.AsNoTracking() //.Include(u => u.ChurchBody)
                                       .Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserGroupId == b.UserGroupId && x.Status == "A")
                                       from d in _context.UserPermission.AsNoTracking() //.Include(u => u.ChurchBody)
                                       .Where(x => x.Id == c.UserPermissionId && x.PermStatus == "A")   // (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) &&  x.ChurchBodyId == a.ChurchBodyId && 

                                       select d  // c.UserPermissions; 
                                   ).ToList();


            var oUserLogPermAll = new List<UserPermission>();
            oUserLogPermAll.AddRange(oUserLog_RolePerm);
            oUserLogPermAll.AddRange(oUserLog_GroupPerm);
             
            return oUserLogPermAll; 
        }

        public static UserSessionPrivilege GetUserAssigned_SessionPrivileges(MSTR_DbContext _context, string churchCode, UserProfile oUserProfile) // ChurchModelContext _clientContext, 
        {
            //already authenticated... jux the use the user and churchCode... check Active though
            // var _oUserLog_RolePerm = new List<UserPermission>();

            var h_pwd = AppUtilties.ComputeSha256Hash(churchCode);
            var oUR_SessionPermList = (
                                    from a in _context.UserProfile.AsNoTracking()  //.Include(u => u.AppGlobalOwner).Include(u => u.ChurchBody)
                                    .Where(x => (h_pwd == ac1 || (x.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && x.ChurchBody.GlobalChurchCode == churchCode && x.ChurchBodyId == oUserProfile.ChurchBodyId)) && x.Username.Trim().ToLower() == oUserProfile.Username.Trim().ToLower() && x.UserStatus == "A")
                                    from b in _context.UserProfileRole.AsNoTracking().Include(u => u.UserRole)
                                    .Where(x => (h_pwd == ac1 || (x.AppGlobalOwnerId == a.AppGlobalOwnerId && x.ChurchBodyId == a.ChurchBodyId)) && x.UserProfileId == a.Id && x.ProfileRoleStatus == "A")
                                    from c in _context.UserRolePermission.AsNoTracking() //.Include(u => u.ChurchBody)
                                    .Where(x => x.AppGlobalOwnerId == null && x.ChurchBodyId == null && x.UserRoleId == b.UserRoleId && x.Status == "A")   // (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && 
                                    from d in _context.UserPermission.AsNoTracking() //.Include(u => u.ChurchBody)
                                    .Where(x => x.Id == c.UserPermissionId && x.PermStatus == "A")  //&& (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId 

                                    // later: exclude the list of removed provileges per congregation/user  ... leave the [UserRolePermission] table as master ref.

                                    select new UserSessionPerm()  // UserSessionPrivilege
                                    {
                                       // AppGlobalOwner = a.AppGlobalOwner,
                                       // ChurchBody = a.ChurchBody,
                                      //  UserProfile = oUserProfile,
                                        UserRole = b.UserRole,
                                      //  RoleType = b.UserRole != null ? b.UserRole.RoleType : "",
                                        UserPermission = d,
                                        oUserPermissionId = d.Id,
                                        PermissionCode = d.PermissionCode,
                                        PermissionName = d.PermissionName,
                                      //  PermissionValue = true,
                                       // strChurchCode_AGO = a.AppGlobalOwner != null ? a.AppGlobalOwner.GlobalChurchCode : "",
                                       // strChurchCode_CB = a.ChurchBody != null ? a.ChurchBody.GlobalChurchCode : "",  // assumption that Unspecified CB ~~ Vendor [Church code is MANDATORY!]
                                        //
                                        ViewPerm = c.ViewPerm,
                                        CreatePerm = c.CreatePerm,
                                        EditPerm = c.EditPerm,
                                        DeletePerm = c.DeletePerm,
                                        ManagePerm = c.ManagePerm
                                    })
                                    .ToList();
              

            // Permission can also be config via User Groups...
            // var _oUserLog_GroupPerm = new List<UserPermission>();
            var oUG_SessionPermList = (
                                       from a in _context.UserProfile.AsNoTracking().Include(u => u.AppGlobalOwner).Include(u => u.ChurchBody)
                                       .Where(x => (h_pwd == ac1 || (x.AppGlobalOwnerId == oUserProfile.AppGlobalOwnerId && x.ChurchBody.GlobalChurchCode == churchCode && x.ChurchBodyId == oUserProfile.ChurchBodyId)) && x.Username.Trim().ToLower() == oUserProfile.Username.Trim().ToLower() && x.UserStatus == "A")
                                       from b in _context.UserProfileGroup.AsNoTracking().Include(u => u.UserGroup)
                                       .Where(x => (h_pwd == ac1 || (x.AppGlobalOwnerId == a.AppGlobalOwnerId && x.ChurchBodyId == a.ChurchBodyId)) && x.UserProfileId == a.Id && x.Status == "A")                                 
                                       from c in _context.UserGroupRole.AsNoTracking().Include(u => u.UserRole)
                                        .Where(x => (h_pwd == ac1 || (x.AppGlobalOwnerId == a.AppGlobalOwnerId && x.ChurchBodyId == a.ChurchBodyId)) && x.UserGroupId == b.UserGroupId && x.Status == "A")
                                       from d in _context.UserRolePermission.AsNoTracking()
                                        .Where(x => x.AppGlobalOwnerId==null && x.ChurchBodyId == null && x.UserRoleId == c.UserRoleId && x.Status == "A")   // (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && 
                                       from e in _context.UserPermission.AsNoTracking()
                                       .Where(x => x.Id == d.UserPermissionId && x.PermStatus == "A")  //&& (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId 

                                       select new UserSessionPerm()  //UserSessionPrivilege
                                       {
                                          // AppGlobalOwner = a.AppGlobalOwner,
                                          // ChurchBody = a.ChurchBody,
                                          // UserProfile = oUserProfile,
                                           UserGroup = b.UserGroup,
                                           UserRole = c.UserRole,
                                          // GroupName = b.UserGroup != null ? b.UserGroup.GroupName : "",
                                           UserPermission = e,
                                           oUserPermissionId = d.Id,
                                           PermissionCode = e.PermissionCode,
                                           PermissionName = e.PermissionName,
                                          // PermissionValue = true,
                                          // strChurchCode_AGO = a.AppGlobalOwner != null ? a.AppGlobalOwner.GlobalChurchCode : "",
                                          // strChurchCode_CB = a.ChurchBody != null ? a.ChurchBody.GlobalChurchCode : "",  // assumption that Unspecified CB ~~ Vendor [Church code is MANDATORY!]
                                           //
                                           ViewPerm = d.ViewPerm,
                                           CreatePerm = d.CreatePerm,
                                           EditPerm = d.EditPerm,
                                           DeletePerm = d.DeletePerm,
                                           ManagePerm = d.ManagePerm
                                       })
                                        .ToList();


            //from c in _context.UserGroupPermission//.Include(u => u.ChurchBody)
            //.Where(x => (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) && x.ChurchBodyId == a.ChurchBodyId && x.UserGroupId == b.UserGroupId && x.Status == "A")
            //from d in _context.UserPermission//.Include(u => u.ChurchBody)
            //.Where(x => x.Id == c.UserPermissionId && x.PermStatus == "A")   // (h_pwd == ac1 || x.ChurchBody.GlobalChurchCode == churchCode) &&  x.ChurchBodyId == a.ChurchBodyId && 


            var oPermAll = new List<UserSessionPerm>();
            oPermAll.AddRange(oUR_SessionPermList);
            oPermAll.AddRange(oUG_SessionPermList);

            var oUserLogPrivAll = new UserSessionPrivilege();
            oUserLogPrivAll.AppGlobalOwner = oUserProfile.AppGlobalOwner;  
            oUserLogPrivAll.ChurchBody = oUserProfile.ChurchBody;  
            oUserLogPrivAll.UserProfile = oUserProfile;  
            oUserLogPrivAll.strChurchCode_AGO = oUserProfile.AppGlobalOwner != null ? oUserProfile.AppGlobalOwner.GlobalChurchCode : "";  
            oUserLogPrivAll.strChurchCode_CB = oUserProfile.ChurchBody != null ? oUserProfile.ChurchBody.GlobalChurchCode : ""; 
            oUserLogPrivAll.UserSessionPermList = oPermAll;  // both group and role permissions...
                        
            // add other user details
            oUserLogPrivAll.UserRoles = new List<UserRole>(); oUserLogPrivAll.arrAssignedRoleCodes = new List<string>();oUserLogPrivAll.arrAssignedRoleNames = new List<string>();
            oUserLogPrivAll.UserGroups = new List<UserGroup>(); oUserLogPrivAll.arrAssignedGroupCodes = new List<string>(); oUserLogPrivAll.arrAssignedGroupNames = new List<string>();
            oUserLogPrivAll.UserPermissions = new List<UserPermission>(); oUserLogPrivAll.arrAssignedPermCodes = new List<string>();
            oUserLogPrivAll.arrAssignedModCodes = new List<string>();

            ///
            var arrRoleCodes = new ArrayList();  
            var arrGroupCodes = new ArrayList(); 
            var arrPermCodes = new ArrayList(); 
            var arrModCodes = new ArrayList(); 

            foreach (var oPerm in oPermAll)  /// IsModAccessDS00IsModAccessVAA4
            {
                oUserLogPrivAll.IsModAccessVAA0 = !oUserLogPrivAll.IsModAccessVAA0 ? oPerm.PermissionCode.StartsWith("A0") : oUserLogPrivAll.IsModAccessVAA0 ;
                oUserLogPrivAll.IsModAccessVAA4 = !oUserLogPrivAll.IsModAccessVAA4 ? oPerm.PermissionCode.StartsWith("A0_04") : oUserLogPrivAll.IsModAccessVAA4 ;
                ///
                oUserLogPrivAll.IsModAccessDS00 = !oUserLogPrivAll.IsModAccessDS00 ? oPerm.PermissionCode.StartsWith("00") : oUserLogPrivAll.IsModAccessDS00 ;
                oUserLogPrivAll.IsModAccessAC01 = !oUserLogPrivAll.IsModAccessAC01 ? oPerm.PermissionCode.StartsWith("01") : oUserLogPrivAll.IsModAccessAC01 ;
                oUserLogPrivAll.IsModAccessMR02 = !oUserLogPrivAll.IsModAccessMR02 ? oPerm.PermissionCode.StartsWith("02") : oUserLogPrivAll.IsModAccessMR02 ;
                oUserLogPrivAll.IsModAccessCL03 = !oUserLogPrivAll.IsModAccessCL03 ? oPerm.PermissionCode.StartsWith("03") : oUserLogPrivAll.IsModAccessCL03 ;
                oUserLogPrivAll.IsModAccessCA04 = !oUserLogPrivAll.IsModAccessCA04 ? oPerm.PermissionCode.StartsWith("04") : oUserLogPrivAll.IsModAccessCA04 ;
                oUserLogPrivAll.IsModAccessFM05 = !oUserLogPrivAll.IsModAccessFM05 ? oPerm.PermissionCode.StartsWith("05") : oUserLogPrivAll.IsModAccessFM05 ;
                oUserLogPrivAll.IsModAccessRA06 = !oUserLogPrivAll.IsModAccessRA06 ? oPerm.PermissionCode.StartsWith("06") : oUserLogPrivAll.IsModAccessRA06;
                ///
                if (oPerm.PermissionCode.Length >= 2)
                {
                    if (!arrModCodes.Contains(oPerm.PermissionCode.Substring(0, 2)))
                    {
                        oUserLogPrivAll.arrAssignedModCodes.Add(oPerm.PermissionCode.Substring(0, 2));
                    }
                    arrModCodes.Add(oPerm.PermissionCode.Substring(0, 2));
                }
                
                // oUserLogPrivAll.strAssignedRole = !arrRoleNames.Contains(oPerm.UserRole?.RoleName) ? oPerm.UserRole?.RoleDesc + ", " : "";
                if (oPerm.UserRole != null && !arrRoleCodes.Contains(oPerm.UserRole?.RoleType))
                { 
                    oUserLogPrivAll.UserRoles.Add(oPerm.UserRole);  
                    oUserLogPrivAll.arrAssignedRoleCodes.Add(oPerm.UserRole.RoleType); 
                    oUserLogPrivAll.arrAssignedRoleNames.Add(oPerm.UserRole.RoleName); 
                }
                arrRoleCodes.Add(oPerm.UserRole?.RoleType);
                ///
               // oUserLogPrivAll.strAssignedGroup = !arrGroupNames.Contains(oPerm.UserGroup?.GroupName) ? oPerm.UserGroup?.GroupDesc + ", " : "";
                if (oPerm.UserGroup != null && !arrGroupCodes.Contains(oPerm.UserGroup?.GroupType))
                { 
                    oUserLogPrivAll.UserGroups.Add(oPerm.UserGroup); 
                    oUserLogPrivAll.arrAssignedGroupCodes.Add(oPerm.UserGroup.GroupType); 
                    oUserLogPrivAll.arrAssignedGroupNames.Add(oPerm.UserGroup.GroupName); 
                }
                arrGroupCodes.Add(oPerm.UserGroup?.GroupType);
                ///
                if (oPerm.UserPermission != null && !arrPermCodes.Contains(oPerm.UserPermission?.PermissionCode))
                { 
                    oUserLogPrivAll.UserPermissions.Add(oPerm.UserPermission); 
                    oUserLogPrivAll.arrAssignedPermCodes.Add(oPerm.UserPermission.PermissionCode); 
                }
                arrPermCodes.Add(oPerm.UserPermission?.PermissionCode);
            }

          //  if (oUserLogPrivAll.strAssignedRole.Contains(",")) oUserLogPrivAll.strAssignedRole.Remove(oUserLogPrivAll.strAssignedRole.LastIndexOf(","));
          //  if (oUserLogPrivAll.strAssignedGroup.Contains(",")) oUserLogPrivAll.strAssignedGroup.Remove(oUserLogPrivAll.strAssignedGroup.LastIndexOf(","));

 
            return oUserLogPrivAll; 
        }
 

        public static List<UserPermission> CombineCollection(List<UserPermission> list1, List<UserPermission> list2,
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

        public static string  GetPermissionDesc_FromName(string permName )
        {
            //[ ____ : (, ___ : ), ______ : - , _____ : &]
            if (permName.Contains("__"))
            {
                //var pn = ((((permName.Replace("______", " - ")).Replace("_____", " & ")).Replace("____", " (")).Replace("___", ") ")).Trim();  //   .Replace("__", "")).Replace("_", " ")).Trim();

                var pn1 = permName.Replace("______", "-");
                var pn2 = pn1.Replace("_____", " & ");
                var pn3 = pn2.Replace("____", " (");
                var pn4 = pn3.Replace("___", ") ");  //   .Replace("__", "")).Replace("_", " ")).Trim();
                var codeStr = pn4.Split("__"); 
                return ((codeStr[1].Replace("__", "")).Replace("_", " ")).Trim();
            }

            return permName; 
        }

        public static string GetPermissionCode_FromName(string permName)
        {
            if (permName.Contains("__"))
            {
                var codeStr = permName.Split("__");

                if (codeStr.Length > 1)
                    return codeStr[0].Substring(1).Trim();
                else
                    return codeStr[0].Trim();
            }

            return "";
        }

        public static string GetPermModule(string oCode)
        {
            switch (oCode)
            {
                case "A0": return "System Administration";
                case "00": return "Dashboard";
                case "01": return "App Configuration";
                case "02": return "Member Register";
                case "03": return "Churchlife & Events";
                case "04": return "Church Administration";
                case "05": return "Finance Management";
                case "06": return "Reports & Analytics";

                default: return string.Empty;
            }
        }

        public List<UserPermission> GetSystem_Administration_Permissions()
        {
            var lsPerms = new List<UserPermission>(); //10
            //
            lsPerms.Add(new UserPermission(0, null, "A0", "_A0__System_Administration", "A", null, null, null, null));   // main Ven module
            lsPerms.Add(new UserPermission(0, null, "A0_00", "_A0_00__SYS_Account", "A", null, null, null, null));   // // SYS manages itself and superadmin
            lsPerms.Add(new UserPermission(0, null, "A0_01", "_A0_01__Super_Admin_Account", "A", null, null, null, null));   // manages itself and other system admin account only + church tasks /admins
            lsPerms.Add(new UserPermission(0, null, "A0_02", "_A0_02__System_Admin_Account", "A", null, null, null, null));   // manages itself and church tasks /admins only
            lsPerms.Add(new UserPermission(0, null, "A0_04", "_A0_04__Manage_CL_Config_Ignitio", "A", null, null, null, null));  // manages itself and  ... manages CL config issues and system fxns
            ///
            lsPerms.Add(new UserPermission(0, null, "A0_06", "_A0_06__Church_Faith_Types", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "A0_07", "_A0_07__Denominations", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "A0_08", "_A0_08__Church_Levels", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "A0_09", "_A0_09__Congregations", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "A0_10", "_A0_10__Subscribers_Unit_Church_Structure", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "A0_11", "_A0_11__Church_Administrator_Accounts", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "A0_12", "_A0_12__User_Subscriptions", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "A0_13", "_A0_13__Admin_Dashboard", "A", null, null, null, null));
            //
            return lsPerms;
        }

        public List<UserPermission> GetAppDashboard_Permissions()
        {
            var lsPerms = new List<UserPermission>(); //8
            //
            lsPerms.Add(new UserPermission(0, null, "00", "_00__Dashboard", "A", null, null, null, null));  
            lsPerms.Add(new UserPermission(0, null, "00_01", "_00_01__Oversight_Congregations", "A", null, null, null, null));   // CH for CH account only
            lsPerms.Add(new UserPermission(0, null, "00_02", "_00_02__Members", "A", null, null, null, null));  //  XX CH_v
            lsPerms.Add(new UserPermission(0, null, "00_03", "_00_03__New_Converts", "A", null, null, null, null)); // XX CH v
            lsPerms.Add(new UserPermission(0, null, "00_04", "_00_04__Receipts____Income___", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "00_05", "_00_05__Payments____Expense___", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "00_06", "_00_06__Church_Attendance_Trend", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "00_07", "_00_07__To______Do_List", "A", null, null, null, null)); // XX
            //
            return lsPerms;
        }

        public List<UserPermission> GetAppConfigurations_Permissions()
        {
            var lsPerms = new List<UserPermission>();//9
            //
            lsPerms.Add(new UserPermission(0, null, "00", "_01__App_Configurations", "A", null, null, null, null)); // 
            lsPerms.Add(new UserPermission(0, null, "01_01", "_01_01__Church_Parameters____Configurations___", "A", null, null, null, null));    
            lsPerms.Add(new UserPermission(0, null, "01_02", "_01_02__General_Parameters", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "01_03", "_01_03__Church_Units_Structure____Organisational_Chart___", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "01_04", "_01_04__Internal_User_Accounts", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "01_05", "_01_05__User_Preferences", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "01_06", "_01_06__Upload_Configuration", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "01_07", "_01_07__Custom_Import_Configuration", "A", null, null, null, null));
            //
            return lsPerms;
        }

        public List<UserPermission> GetMemberRegister_Permissions()
        {
            var lsPerms = new List<UserPermission>();//9
            //
            lsPerms.Add(new UserPermission(0, null, "02", "_02__Member_Register", "A", null, null, null, null));  // XX CH-v
            lsPerms.Add(new UserPermission(0, null, "02_01", "_02_01__Member_Explorer", "A", null, null, null, null));   // XX for SYS account only
            lsPerms.Add(new UserPermission(0, null, "02_01_01", "_02_01_01__Member_Profile", "A", null, null, null, null)); // XX CH-v
            lsPerms.Add(new UserPermission(0, null, "02_01_02", "_02_01_02__Member_Church______life", "A", null, null, null, null));  // XX CH-v
            lsPerms.Add(new UserPermission(0, null, "02_02", "_02_02__New_Members", "A", null, null, null, null)); // XX CH-v
            lsPerms.Add(new UserPermission(0, null, "02_03", "_02_03__Past_Membership", "A", null, null, null, null)); // XX CH-v
            lsPerms.Add(new UserPermission(0, null, "02_04", "_02_04__Profile_Card", "A", null, null, null, null)); // XX CH-v
            lsPerms.Add(new UserPermission(0, null, "02_05", "_02_05__Lookup_Member", "A", null, null, null, null)); // XX CH-v unless within CH/CN or parents
            lsPerms.Add(new UserPermission(0, null, "02_06", "_02_06__Member_History", "A", null, null, null, null)); /// XX CH-v unless within CH/CN or parents
            //
            return lsPerms;
        }

        public List<UserPermission> GetChurchlifeAndEvents_Permissions()
        {
            var lsPerms = new List<UserPermission>();//12
            //
            lsPerms.Add(new UserPermission(0, null, "03", "_03__Church______life_____Events", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "03_01", "_03_01__Church_Calendar____Almanac___", "A", null, null, null, null));  // XX for SYS account only
            lsPerms.Add(new UserPermission(0, null, "03_02", "_03_02__Events_Countdown", "A", null, null, null, null));  // XX
            lsPerms.Add(new UserPermission(0, null, "03_03", "_03_03__Church_Service_Line______up", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "03_04", "_03_04__Order_of_Service", "A", null, null, null, null));  // XX
            lsPerms.Add(new UserPermission(0, null, "03_05", "_03_05__Preaching_Plan", "A", null, null, null, null)); // XX similar to calendar
            lsPerms.Add(new UserPermission(0, null, "03_06", "_03_06__Church_Activity_Roster", "A", null, null, null, null));  //  // XX
            lsPerms.Add(new UserPermission(0, null, "03_07", "_03_07__Minister_Schedule", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "03_08", "_03_08__Church_Core_Activities", "A", null, null, null, null));  // CN
            lsPerms.Add(new UserPermission(0, null, "03_09", "_03_09__Member______Activity_Checklist", "A", null, null, null, null));   // CN
            lsPerms.Add(new UserPermission(0, null, "03_10", "_03_10__My_Calendar", "A", null, null, null, null));  //  XX
            lsPerms.Add(new UserPermission(0, null, "03_11", "_03_11__To______Do_List", "A", null, null, null, null));  // XX
            //
            return lsPerms;
        }

        public List<UserPermission> GetChurchAdministration_Permissions()
        {
            var lsPerms = new List<UserPermission>();   //19
            //
            lsPerms.Add(new UserPermission(0, null, "04", "_04__Church_Administration", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "04_01", "_04_01__Congregations", "A", null, null, null, null));    // XX CH-v unless within CH/CN or parents
            lsPerms.Add(new UserPermission(0, null, "04_02", "_04_02__Church_Units", "A", null, null, null, null));  // XX
            lsPerms.Add(new UserPermission(0, null, "04_03", "_04_03__Leadership_Pool", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "04_04", "_04_04__Church_Projects", "A", null, null, null, null)); // XX sponsor must have access
            lsPerms.Add(new UserPermission(0, null, "04_05", "_04_05__Church_Attendance", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "04_06", "_04_06__Church_Visitors", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "04_07", "_04_07__New_Converts", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "04_08", "_04_08__Church_Transfers", "A", null, null, null, null));  // XX
            lsPerms.Add(new UserPermission(0, null, "04_08_01", "_04_08_01__Member_Transfers", "A", null, null, null, null));  // CN but approvals can go up
            lsPerms.Add(new UserPermission(0, null, "04_08_02", "_04_08_02__Clergy_Transfers", "A", null, null, null, null)); // XX but top -- down
            lsPerms.Add(new UserPermission(0, null, "04_08_03", "_04_08_03__Role_Transfers", "A", null, null, null, null));   // XX both lay /clergy
            lsPerms.Add(new UserPermission(0, null, "04_08_04", "_04_08_04__Transfer_Requests_Approval", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "04_09", "_04_09__Promotions_____Demotions", "A", null, null, null, null)); // CH from top
            lsPerms.Add(new UserPermission(0, null, "04_10", "_04_10__Notices_____Announcements", "A", null, null, null, null));  // XX
            lsPerms.Add(new UserPermission(0, null, "04_11", "_04_11__Internal______Communication", "A", null, null, null, null));  // XX
            lsPerms.Add(new UserPermission(0, null, "04_11_01", "_04_11_01__Broadcast_Notifications", "A", null, null, null, null)); // XX
            lsPerms.Add(new UserPermission(0, null, "04_11_02", "_04_11_02__Send_Birthday_Anniversary_Messages", "A", null, null, null, null));  //XX
            lsPerms.Add(new UserPermission(0, null, "04_12", "_04_12__Assets_Register", "A", null, null, null, null));  // XX per CB
            //
            return lsPerms;
        }

        public List<UserPermission> GetFinanceManagement_Permissions()
        {
            var lsPerms = new List<UserPermission>();   //9
            //
            lsPerms.Add(new UserPermission(0, null, "05", "_05__Finance_Management", "A", null, null, null, null));
            lsPerms.Add(new UserPermission(0, null, "05_01", "_05_01__Receipts____Income___", "A", null, null, null, null));   // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_02", "_05_02__Payments____Expense___", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_03", "_05_03__Offertory____Collection___", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_04", "_05_04__Tithes", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_05", "_05_05__Donations", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_06", "_05_06__Trial_Balance", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_07", "_05_07__Financial_Reports", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "05_08", "_05_08__Sync_Capitalized_Assets", "A", null, null, null, null));  // XX aggregate up

            //
            return lsPerms;
        }

        public List<UserPermission> GetReportsAnalytics_Permissions()
        {
            var lsPerms = new List<UserPermission>();   //4
            //
            lsPerms.Add(new UserPermission(0, null, "06", "_06__Reports_____Analytics", "A", null, null, null, null));   // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "06_01", "_06_01__Church_Statistics", "A", null, null, null, null));  // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "06_02", "_06_02__Growth_Trends", "A", null, null, null, null));   // XX aggregate up
            lsPerms.Add(new UserPermission(0, null, "06_03", "_06_03__Adhoc_Analysis", "A", null, null, null, null));   // XX aggregate up
            //
            return lsPerms;
        }


        public List<UserRole> GetSystemDefaultRoles()
        {
            var lsRoles = new List<UserRole>();
            //
            lsRoles.Add(new UserRole(0, null, null, "SYS", "System Account", "System Account", 1, "A", "S", null, null, null, null));
            lsRoles.Add(new UserRole(0, null, null, "SUP_ADMN", "Super Admin", "Super Administrator", 2, "A", "S", null, null, null, null));  //all +  incl. CL profiles and admin fxns
            lsRoles.Add(new UserRole(0, null, null, "SYS_ADMN", "System Admin", "System Administrator", 3, "A", "S", null, null, null, null));            
            lsRoles.Add(new UserRole(0, null, null, "SYS_CL_ADMN", "Client System Assist", "Client System Assistant", 4, "A", "S", null, null, null, null)); /// can access CL profiles and admin fxns
            lsRoles.Add(new UserRole(0, null, null, "SYS_CUST", "System Custom", "System Custom", 5, "A", "S", null, null, null, null));
            //                               null,
            lsRoles.Add(new UserRole(0, null, null, "CH_ADMN", "Church Administrator", "Church Administrator", 6, "A", "S", null, null, null, null));   ///  ... all mdl  :: 1st created by Vendor    
            lsRoles.Add(new UserRole(0, null, null, "CH_MGR", "Church Manager", "Church Manager", 7, "A", "S", null, null, null, null));    /// ... MR, CL, CA  [+ DS, RA]
            lsRoles.Add(new UserRole(0, null, null, "CH_EXC", "Church Executive", "Church Executive", 8, "A", "S", null, null, null, null));  /// ... CL, CA      [+ DS, RA]
            lsRoles.Add(new UserRole(0, null, null, "CH_RGSTR", "Church Registrar", "Church Registrar", 9, "A", "S", null, null, null, null));  /// ... MR          [+ DS, RA]
            lsRoles.Add(new UserRole(0, null, null, "CH_ACCT", "Church Accountant", "Church Accountant", 10, "A", "S", null, null, null, null));    /// ... CF          [+ DS, RA]
           // lsRoles.Add(new UserRole(null, null,null, "CH_CUST2", "Church Custom", "A", 10, "CH_CUS", "S",null, null, null, null));  
            //                               null,
            lsRoles.Add(new UserRole(0, null, null, "CF_ADMN", "Congregation Administrator", "Congregation Administrator", 11, "A", "S", null, null, null, null));   // 1st created by Vendor  
            lsRoles.Add(new UserRole(0, null, null, "CF_MGR", "Congregation Manager", "Congregation Manager", 12, "A", "S", null, null, null, null));
            lsRoles.Add(new UserRole(0, null, null, "CF_EXC", "Congregation Executive", "Congregation Executive", 13, "A", "S", null, null, null, null));
            lsRoles.Add(new UserRole(0, null, null, "CF_RGSTR", "Congregation Registrar", "Congregation Registrar", 14, "A", "S", null, null, null, null));
            lsRoles.Add(new UserRole(0, null, null, "CF_ACCT", "Congregation Accountant", "Congregation Accountant", 15, "A", "S", null, null, null, null));
            // lsRoles.Add(new UserRole(null, null,null, "CF_CUST2", "Congregation Custom", "A", 15, "CF_CUS", "S",null, null, null, null));  
            //
            return lsRoles;
        }


        //public List<UserRole> GetSystemDefaultRoles()
        //{
        //    var lsRoles = new List<UserRole>();
        //    //
        //    lsRoles.Add(new UserRole(0, null,null, "SYS", "System Account", "System Account", 1, "A", "S", null, null, null, null));     
        //    lsRoles.Add(new UserRole(0, null,null, "SUP_ADMN", "Super Admin", "Super Administrator", 2, "A", "S", null, null, null, null));     
        //    lsRoles.Add(new UserRole(0, null,null, "SYS_ADMN", "System Admin", "System Administrator", 3, "A", "S", null, null, null, null));    
        //    lsRoles.Add(new UserRole(0, null,null, "SYS_CUST", "System Custom", "System Custom",  4, "A","S", null, null, null, null));
        //    // lsRoles.Add(new UserRole(null, null,null, "SYS_CUST2", "System Custom", "A", 5, "SYS_CUST", "S",null, null, null, null));    
        //    //                               null,
        //    lsRoles.Add(new UserRole(0, null,null, "CH_ADMN", "Church Admin", "Church Administrator", 6, "A", "S", null, null, null, null));  // 1st created by Vendor   
        //    lsRoles.Add(new UserRole(0, null,null, "CH_RGSTR", "Church Registrar", "Church Registrar", 7, "A", "S", null, null, null, null));     
        //    lsRoles.Add(new UserRole(0, null,null, "CH_ACCT", "Church Accountant", "Church Accountant", 8, "A","S", null, null, null, null));     
        //    lsRoles.Add(new UserRole(0, null,null, "CH_CUST", "Church Custom", "Church Custom", 9, "A", "S", null, null, null, null));
        //    // lsRoles.Add(new UserRole(null, null,null, "CH_CUST2", "Church Custom", "A", 10, "CH_CUS", "S",null, null, null, null));  
        //    //                               null,
        //    lsRoles.Add(new UserRole(0, null,null, "CF_ADMN", "Congregation Administrator", "Congregation Administrator", 11, "A", "S", null, null, null, null));   // 1st created by Vendor  
        //    lsRoles.Add(new UserRole(0, null,null, "CF_RGSTR", "Congregation Registrar", "Congregation Registrar", 12, "A", "S", null, null, null, null));     
        //    lsRoles.Add(new UserRole(0, null,null, "CF_ACCT", "Congregation Accountant", "Congregation Accountant", 13, "A", "S", null, null, null, null));     
        //    lsRoles.Add(new UserRole(0, null,null, "CF_CUST", "Congregation Custom",  "Congregation Custom", 14, "A", "S", null, null, null, null));
        //    // lsRoles.Add(new UserRole(null, null,null, "CF_CUST2", "Congregation Custom", "A", 15, "CF_CUS", "S",null, null, null, null));  
        //    //
        //    return lsRoles;
        //}


        //public List<UserGroup> GetSystemDefaultGroups()
        //{
        //    var lsGroups = new List<UserGroup>();
        //    //
        //    lsGroups.Add(new UserGroup(0, null, null, "SYS", "SYS", "System Account", 1, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "SUP_ADMN", "SUP_ADMN", "Super Administrator", 2, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "SYS_ADMN", "SYS_ADMN", "System Administrator", 3, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "SYS_CUST", "SYS_CUST", "System Custom", 4, null, "A", "S", null, null, null, null));
        //    // lsGroups.Add(new UserGroup(null, null,null, "SYS_CUST2", "System Custom", "A", 5, "SYS_CUST", "S",null, null, null, null));    
        //    //                               
        //    lsGroups.Add(new UserGroup(0, null, null, "CH_ADMN", "CH_ADMN", "Church Administrator", 6, null, "A", "S", null, null, null, null));  // 1st created by Vendor   
        //    lsGroups.Add(new UserGroup(0, null, null, "CH_RGSTR", "CH_RGSTR", "Church Registrar", 7, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "CH_ACCT", "CH_ACCT", "Church Accountant", 8, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "CH_CUST", "CH_CUST", "Church Custom", 9, null, "A", "S", null, null, null, null));
        //    // lsGroups.Add(new UserGroup(null, null,null, "CH_CUST2", "Church Custom", "A", 10, null,  "CH_CUS", "S",null, null, null, null));  
        //    //                               null,
        //    lsGroups.Add(new UserGroup(0, null, null, "CF_ADMN", "CF_ADMN", "Congregation Administrator", 11, null, "A", "S", null, null, null, null));   // 1st created by Vendor  
        //    lsGroups.Add(new UserGroup(0, null, null, "CF_RGSTR", "CF_RGSTR", "Congregation Registrar", 12, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "CF_ACCT", "CF_ACCT", "Congregation Accountant", 13, null, "A", "S", null, null, null, null));
        //    lsGroups.Add(new UserGroup(0, null, null, "CF_CUST", "CF_CUST", "Congregation Custom", 14, null, "A", "S", null, null, null, null));
        //    // lsGroups.Add(new UserGroup(null, null,null, "CF_CUST2", "Congregation Custom", "A", 15, null,  "CF_CUS", "S",null, null, null, null));  
        //    //
        //    return lsGroups;
        //}


        //public static List<RelationshipType> GetCustomRelationTypes(ChurchMember oChurchMember, RhemaCMS.Models.CLNTModels.ChurchModelContext _context)
        //{
        //    //List<RelationshipType> exclRelConfig
        //   // List<RelationshipType> oRelConfAvail = new List<RelationshipType>();

        //    if (oChurchMember != null)
        //    {
        //        var exclRelConf = (from a in _context.RelationshipType
        //                           join
        //                           b in _context.MemberRelation.Where(c => c.ChurchMemberId == oChurchMember.Id) on a.Id equals b.RelationshipId
        //                           select a)
        //                      .ToList();

        //        //filter lists...            
        //        return _context.RelationshipType
        //                            .Where(tpl => !exclRelConf.Contains(tpl))
        //                            .OrderBy(tpl => tpl.RelationIndex).ThenBy(tpl => tpl.Name)
        //                            .ToList(); 
        //    }
        //    else
        //        return  _context.RelationshipType 
        //                            .OrderBy(c => c.RelationIndex).ThenBy(c=>c.Name)
        //                            .ToList(); ;


        //    ////config per member... member config with husband should not show up next time to be picked up
        //    //List < RelationshipType > oRelTypes = new List<RelationshipType>();

        //    ////get list saved already...
        //    //var oRelOptions = _context.RelationshipType                               
        //    //                    .OrderBy(c => c.RelationIndex)
        //    //                    .ToList();
        //    //foreach (var oRelItem in oRelOptions)
        //    //{
        //    //    var oRelFound = false;
        //    //    if (exclRelConfig != null)
        //    //    {
        //    //        foreach (var oExcl in exclRelConfig) // int i = 0; i < exclRelTypes.Count; i++)
        //    //        {
        //    //            if (oExcl.Name.Equals(oRelItem.Name) && oExcl.RelationIndex.Equals(oRelItem.RelationIndex))
        //    //                oRelFound = true; break;
        //    //        }
        //    //    }

        //    //    if (!oRelFound)
        //    //        oRelTypes.Add(oRelItem);
        //    //}

        //    //return oRelTypes;
        //}


        public static List<RelationshipType> GetGenericRelationTypes(List<RelationshipType> exclRelCustom)
        {
            //config per saved dbase... member config with list <> should not show up next time to be saved
            List<RelationshipType> oRelTypes = new List<RelationshipType>();
            var oRelOptions = GetRelationOptions();
            foreach (var oRelItem in oRelOptions)   //for(int i=0; i<oRelOptions.Count; i++) 
            {
               // var oRelItem = oRelOptions[i];

                var oRelFound = false;
                if (exclRelCustom != null)
                {
                    foreach (var oExcl in exclRelCustom) // int i = 0; i < exclRelTypes.Count; i++)
                    {
                        if (oExcl.Name.Equals(oRelItem[0]) && oExcl.LevelIndex.Equals(oRelItem[1]) && oExcl.RelationCode.Equals(oRelItem[2]))
                            oRelFound = true; break;
                    }
                }

                if (!oRelFound)
                {
                    var newRelType = new RelationshipType();
                    newRelType.Name =  oRelItem[0].ToString();
                    newRelType.LevelIndex = int.Parse(oRelItem[1].ToString()); 
                    newRelType.RelationCode = int.Parse(oRelItem[2].ToString());
                    //
                    var oMalePair = GetRelationshipTypePair('M', newRelType.RelationCode, newRelType.Name);
                    newRelType.RelationshipTypeMalePairCode = oMalePair != null ? oMalePair.RelationCode : (int?)null;
                    var oFemalePair = GetRelationshipTypePair('F', newRelType.RelationCode, newRelType.Name);
                    newRelType.RelationshipTypeFemalePairCode = oFemalePair != null ? oFemalePair.RelationCode : (int?)null;
                    var oGenericPair = GetRelationshipTypePair('X', newRelType.RelationCode, newRelType.Name);
                    newRelType.RelationshipTypeGenericPairCode = oGenericPair != null ? oGenericPair.RelationCode : (int?)null;
                    /// 
                    newRelType.IsSpouse = newRelType.RelationCode == 21 || newRelType.RelationCode == 22 || newRelType.RelationCode == 23;
                    newRelType.IsChild = newRelType.RelationCode == 31 || newRelType.RelationCode == 32 || newRelType.RelationCode == 33;
                    newRelType.IsParent = newRelType.RelationCode == 41 || newRelType.RelationCode == 42 || newRelType.RelationCode == 43;

                    oRelTypes.Add(newRelType);
                }                    
            }
            
            return oRelTypes;
        }


        public static RelationshipType GetRelationshipType(int oRelationCode, string oRelationName = "")
        {
            //config per saved dbase... member config with list <> should not show up next time to be saved
            List<RelationshipType> oRelTypes = new List<RelationshipType>();
            var oRelOptions = GetRelationOptions();
            foreach (var oRelItem in oRelOptions)   //for(int i=0; i<oRelOptions.Count; i++) 
            {  
                if (oRelationCode == int.Parse(oRelItem[2].ToString()) || oRelationName.Equals(oRelItem[0]))
                {
                    var newRelType = new RelationshipType();
                    newRelType.Name = oRelItem[0].ToString();
                    newRelType.LevelIndex = int.Parse(oRelItem[1].ToString());
                    newRelType.RelationCode = int.Parse(oRelItem[2].ToString());
                    //
                    
                    return newRelType; 
                }                 
            }

            return null;
        }

        public static RelationshipType GetRelationshipTypePair(char pairGender, int oRelationCode, string oRelationName = "")
        {
            return GetRelationPair(pairGender, oRelationCode, oRelationName);
        }

        private static RelationshipType GetRelationPair(char pairGender, int oRelationCode, string oRelationName) // 'M, F, X'
        {
            switch (oRelationCode)
            {
                case 11 : break; // return null; // next-of-kin 

                // spouse, husband, wife - 
                case 21: case 22: case 23: if (pairGender == 'M') return GetRelationshipType(22); else if (pairGender == 'F') return GetRelationshipType(23); else return GetRelationshipType(21); // (gender == 'X') 

                // child, son, daughter - parent, father, mother
                case 31: case 32: case 33: if (pairGender == 'M') return GetRelationshipType(41); else if (pairGender == 'F') return GetRelationshipType(42); else return GetRelationshipType(43); // (gender == 'X') 

                // father, mother, parent  -- child, son, daughter - 
                case 41: case 42: case 43: if (pairGender == 'M') return GetRelationshipType(32); else if (pairGender == 'F') return GetRelationshipType(33); else return GetRelationshipType(31); // (gender == 'X') 

                // brother, sister, sibling
                case 51: case 52: if (pairGender == 'M') return GetRelationshipType(51); else if (pairGender == 'F') return GetRelationshipType(52); else return GetRelationshipType(55); // (gender == 'X') 

                // step-brother, step-sister, sibling
                case 53: case 54: if (pairGender == 'M') return GetRelationshipType(53); else if (pairGender == 'F') return GetRelationshipType(54); else return GetRelationshipType(55); // (gender == 'X') 
                case 55: return GetRelationshipType(55);

                // Great grandfather, Great grandmother, Great grandparent -- Great grandchild, Great gandson, Great ganddaughter
                case 61: case 62: case 63: if (pairGender == 'M') return GetRelationshipType(142); else if (pairGender == 'F') return GetRelationshipType(143); else return GetRelationshipType(141); // (gender == 'X') 

                // Great grandchild, Great gandson, Great ganddaughter -- Great grandfather, Great grandmother , Great grandparent
                case 141: case 142: case 143: if (pairGender == 'M') return GetRelationshipType(61); else if (pairGender == 'F') return GetRelationshipType(62); else return GetRelationshipType(63); // (gender == 'X')

                //  grandfather,  grandmother,  grandparent --  grandchild,  gandson,  ganddaughter
                case 71: case 72: case 73: if (pairGender == 'M') return GetRelationshipType(92); else if (pairGender == 'F') return GetRelationshipType(93); else return GetRelationshipType(91); // (gender == 'X') 

                //  grandchild,  gandson,  ganddaughter  --- grandfather,  grandmother,  grandparent 
                case 91: case 92: case 93: if (pairGender == 'M') return GetRelationshipType(71); else if (pairGender == 'F') return GetRelationshipType(72); else return GetRelationshipType(73); // (gender == 'X') 

                //  Step father,  Step mother , Step parent  -- Step child, Step son, Step daughter
                case 81: case 82: case 85: if (pairGender == 'M') return GetRelationshipType(134); else if (pairGender == 'F') return GetRelationshipType(135); else return GetRelationshipType(133); // (gender == 'X') 

                //  Step child, Step son, Step daughter  -- Step father,  Step mother , Step parent  
                case 133: case 134: case 135: if (pairGender == 'M') return GetRelationshipType(81); else if (pairGender == 'F') return GetRelationshipType(82); else return GetRelationshipType(85); // (gender == 'X') 

                //  Uncle, Auntie, -- Nephew,  Niece 
                case 83: case 84: if (pairGender == 'M') return GetRelationshipType(131); else if (pairGender == 'F') return GetRelationshipType(132); else break; //else return GetRelationshipType(132); // (gender == 'X') 

                //  -- Nephew,  Niece   -- Uncle, Auntie, 
                case 131: case 132: if (pairGender == 'M') return GetRelationshipType(83); else if (pairGender == 'F') return GetRelationshipType(84); else break;

                //  Godfather, Godmother, Godparent  -- Godchild,  Godson, Goddaughter
                case 101: case 102: case 103: if (pairGender == 'M') return GetRelationshipType(115); else if (pairGender == 'F') return GetRelationshipType(116); else return GetRelationshipType(114); // (gender == 'X') 

                //  Godchild,  Godson, Goddaughter -- Godfather, Godmother, Godparent  
                case 114: case 115: case 116: if (pairGender == 'M') return GetRelationshipType(101); else if (pairGender == 'F') return GetRelationshipType(102); else return GetRelationshipType(103); // (gender == 'X') 

                //  Father-in-law,  Mother-in-law, Parent-in-law -- Son-in-law, Daughter-in-law, Child-in-law  
                case 104: case 105: case 106: if (pairGender == 'M') return GetRelationshipType(111); else if (pairGender == 'F') return GetRelationshipType(112); else return GetRelationshipType(113); // (gender == 'X')

                //  Son-in-law, Daughter-in-law, Child-in-law  -- Father-in-law,  Mother-in-law, Parent-in-law  
                case 111: case 112: case 113: if (pairGender == 'M') return GetRelationshipType(104); else if (pairGender == 'F') return GetRelationshipType(105); else return GetRelationshipType(106); // (gender == 'X')

                //  -- Fiance,  Fiancee   -- Fiancee, Fiance, 
                case 122: case 123: if (pairGender == 'M') return GetRelationshipType(122); else if (pairGender == 'F') return GetRelationshipType(123); else break;

                // Cousin
                case 121: return GetRelationshipType(121);

                // Bestie /Close|Best friend / Friend
                case 124: return GetRelationshipType(124);

                // Associate /Acquaintance
                case 125: return GetRelationshipType(125);
            }


            //once it reaches here... match not found
            return null;

        }

        private static List<ArrayList> GetRelationOptions()
        {
            List<ArrayList> oRelations = new List<ArrayList>();
            //var arr = new ArrayList();  
            // [relation name - relation level - rela code]

            // Next-of-Kin = 1  [can be paired up with any other relation as add-on],           
            var arr1 = new ArrayList(); arr1.Add("Next-of-kin"); arr1.Add(1); arr1.Add(11); oRelations.Add(arr1);  
            
            // Spouse = Wife = Husband = 2, ... No Polygamy! (.:.) no stupic LGBTQI.whatever  nonsense!
            var arr2 = new ArrayList(); arr2.Clear(); arr2.Add("Spouse"); arr2.Add(2); arr2.Add(21); oRelations.Add(arr2);  /// N-uclear
            var arr3 = new ArrayList(); arr3.Clear(); arr3.Add("Husband"); arr3.Add(2); arr3.Add(22); oRelations.Add(arr3);  /// N-uclear
            var arr4 = new ArrayList(); arr4.Clear(); arr4.Add("Wife"); arr4.Add(2); arr4.Add(23); oRelations.Add(arr4);  /// N-uclear

            //Child = Son = Daughter = 3             
            var arr5 = new ArrayList(); arr5.Clear(); arr5.Add("Child"); arr5.Add(3); arr5.Add(31); oRelations.Add(arr5);  /// N-uclear
            var arr6 = new ArrayList(); arr6.Clear(); arr6.Add("Son"); arr6.Add(3); arr6.Add(32); oRelations.Add(arr6);  /// N-uclear
            var arr7 = new ArrayList(); arr7.Clear(); arr7.Add("Daughter"); arr7.Add(3); arr7.Add(33); oRelations.Add(arr7);  /// N-uclear

            // Father = Mother = 4
            var arr43 = new ArrayList(); arr43.Add("Parent"); arr43.Add(4); arr43.Add(43); oRelations.Add(arr43);  /// N-uclear
            var arr8 = new ArrayList(); arr8.Add("Father"); arr8.Add(4); arr8.Add(41); oRelations.Add(arr8);  /// N-uclear
            var arr9 = new ArrayList(); arr9.Add("Mother"); arr9.Add(4); arr9.Add(42); oRelations.Add(arr9);  /// N-uclear

            // Sister = Brother = 5 
            var arr10 = new ArrayList(); arr10.Add("Brother"); arr10.Add(5); arr10.Add(51); oRelations.Add(arr10);  /// N-uclear
            var arr11 = new ArrayList(); arr11.Add("Sister"); arr11.Add(5); arr11.Add(52); oRelations.Add(arr11);  /// N-uclear
            var arr12 = new ArrayList(); arr12.Add("Step Brother"); arr12.Add(5); arr12.Add(53); oRelations.Add(arr12);
            var arr13 = new ArrayList(); arr13.Add("Step Sister"); arr13.Add(5); arr13.Add(54); oRelations.Add(arr13);
            var arr44 = new ArrayList(); arr44.Add("Sibling"); arr44.Add(5); arr44.Add(55); oRelations.Add(arr44);

            // Great grandfather = Great grandmother = 6,
            var arr14 = new ArrayList(); arr14.Add("Great grandfather"); arr14.Add(6); arr14.Add(61); oRelations.Add(arr14);
            var arr15 = new ArrayList(); arr15.Add("Great grandmother"); arr15.Add(6); arr15.Add(62); oRelations.Add(arr15);
            var arr45 = new ArrayList(); arr45.Add("Great grandparent"); arr45.Add(6); arr45.Add(63); oRelations.Add(arr45);

            // Grandfather = Grandmother = 7, 
            var arr16 = new ArrayList(); arr16.Add("Grandfather"); arr16.Add(7); arr16.Add(71); oRelations.Add(arr16);
            var arr17 = new ArrayList(); arr17.Add("Grandmother"); arr17.Add(7); arr17.Add(72); oRelations.Add(arr17);
            var arr46 = new ArrayList(); arr46.Add("Grandparent"); arr46.Add(7); arr46.Add(73); oRelations.Add(arr46);

            // Uncle = Auntie = Step mother = Step father = 8
            var arr18 = new ArrayList(); arr18.Add("Step father"); arr18.Add(8); arr18.Add(81); oRelations.Add(arr18);
            var arr19 = new ArrayList(); arr19.Add("Step mother"); arr19.Add(8); arr19.Add(82); oRelations.Add(arr19);
            var arr20 = new ArrayList(); arr20.Add("Uncle"); arr20.Add(8); arr20.Add(83); oRelations.Add(arr20);
            var arr21 = new ArrayList(); arr21.Add("Auntie"); arr21.Add(8); arr21.Add(84); oRelations.Add(arr21);
            var arr47 = new ArrayList(); arr47.Add("Step parent"); arr47.Add(8); arr47.Add(85); oRelations.Add(arr47);

            // Grandchild = Granddaughter = Granddaughter = 9,
            var arr22 = new ArrayList(); arr22.Add("Grandchild"); arr22.Add(9); arr22.Add(91); oRelations.Add(arr22);
            var arr23 = new ArrayList(); arr23.Add("Grandson"); arr23.Add(9); arr23.Add(92); oRelations.Add(arr23);
            var arr24 = new ArrayList(); arr24.Add("Granddaughter"); arr24.Add(9); arr24.Add(93); oRelations.Add(arr24);

            //Father-in-law = Mother-in-law = Godfather = Godmother = 10, 
            var arr25 = new ArrayList(); arr25.Add("Godfather"); arr25.Add(10); arr25.Add(101); oRelations.Add(arr25);
            var arr26 = new ArrayList(); arr26.Add("Godmother"); arr26.Add(10); arr26.Add(102); oRelations.Add(arr26);
            var arr48 = new ArrayList(); arr48.Add("Godparent"); arr48.Add(10); arr48.Add(103); oRelations.Add(arr48);
            var arr27 = new ArrayList(); arr27.Add("Father-in-law"); arr27.Add(10); arr27.Add(104); oRelations.Add(arr27);
            var arr28 = new ArrayList(); arr28.Add("Mother-in-law"); arr28.Add(10); arr28.Add(105); oRelations.Add(arr28);
            var arr52 = new ArrayList(); arr52.Add("Parent-in-law"); arr52.Add(10); arr52.Add(106); oRelations.Add(arr52);

            // Son-in-law = Daughter-in-law = 11, 
            var arr29 = new ArrayList(); arr29.Add("Son-in-law"); arr29.Add(11); arr29.Add(111); oRelations.Add(arr29);
            var arr30 = new ArrayList(); arr30.Add("Daughter-in-law"); arr30.Add(11); arr30.Add(112); oRelations.Add(arr30);
            var arr53 = new ArrayList(); arr53.Add("Child-in-law"); arr53.Add(11); arr53.Add(113); oRelations.Add(arr53);
            var arr49 = new ArrayList(); arr49.Add("Godchild"); arr49.Add(10); arr49.Add(114); oRelations.Add(arr49);
            var arr50 = new ArrayList(); arr50.Add("Godson"); arr50.Add(10); arr50.Add(115); oRelations.Add(arr50);
            var arr51 = new ArrayList(); arr51.Add("Goddaughter"); arr51.Add(10); arr51.Add(116); oRelations.Add(arr51);

            // Cousin = Fiance = Fiancee = Best friend = 12,
            var arr31 = new ArrayList(); arr31.Add("Cousin"); arr31.Add(12); arr31.Add(121); oRelations.Add(arr31);
            var arr32 = new ArrayList(); arr32.Add("Fiance"); arr32.Add(12); arr32.Add(122); oRelations.Add(arr32);
            var arr33 = new ArrayList(); arr33.Add("Fiancee"); arr33.Add(12); arr33.Add(123); oRelations.Add(arr33);
            var arr34 = new ArrayList(); arr34.Add("Bestie /Friend"); arr34.Add(12); arr34.Add(124); oRelations.Add(arr34);
            var arr54 = new ArrayList(); arr54.Add("Associate /Acquaintance"); arr54.Add(12); arr54.Add(125); oRelations.Add(arr54);

            // Nephew = Niece = Step child = Step son = Step daughter = 13, 
            var arr35 = new ArrayList(); arr35.Add("Nephew"); arr35.Add(13); arr35.Add(131); oRelations.Add(arr35);
            var arr36 = new ArrayList(); arr36.Add("Niece"); arr36.Add(13); arr36.Add(132); oRelations.Add(arr36);
            var arr37 = new ArrayList(); arr37.Add("Step child"); arr37.Add(13); arr37.Add(133); oRelations.Add(arr37);
            var arr38 = new ArrayList(); arr38.Add("Step son"); arr38.Add(13); arr38.Add(134); oRelations.Add(arr38);
            var arr39 = new ArrayList(); arr39.Add("Step daughter"); arr39.Add(13); arr39.Add(135); oRelations.Add(arr39);

            // Great grandchild = Great grandson = Great ganddaughter = 14 
            var arr40 = new ArrayList(); arr40.Add("Great grandchild"); arr40.Add(14); arr40.Add(141); oRelations.Add(arr40);
            var arr41 = new ArrayList(); arr41.Add("Great grandson"); arr41.Add(14); arr41.Add(142); oRelations.Add(arr41);
            var arr42 = new ArrayList(); arr42.Add("Great ganddaughter"); arr42.Add(14); arr42.Add(143); oRelations.Add(arr42);

            return oRelations;
        }
         

        public static List<MSTRCountry> GetMS_Countries()
        {
            List<MSTRCountry> lsCountries = new List<MSTRCountry>();
            List<string> lsCountryList = new List<string>();

            CultureInfo[] arrCInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo oCInfo in arrCInfoList)
            {
                RegionInfo oCtry = new RegionInfo(oCInfo.LCID);
                if (!(lsCountryList.Contains(oCtry.ThreeLetterISORegionName))) //EnglishName
                {
                    lsCountryList.Add(oCtry.ThreeLetterISORegionName);//EnglishName

                    var ctry = new MSTRCountry();
                    ctry.EngName = oCtry.EnglishName;
                    ctry.CtryAlpha3Code = oCtry.ThreeLetterISORegionName;
                    ctry.CtryAlpha2Code = oCtry.TwoLetterISORegionName;
                    ctry.CurrEngName = oCtry.CurrencyEnglishName;
                    ctry.CurrLocName = oCtry.CurrencyNativeName;
                    ctry.CurrSymbol = oCtry.CurrencySymbol;
                    ctry.Curr3LISOSymbol = oCtry.ISOCurrencySymbol;

                    lsCountries.Add(ctry);
                }
            }

          //  CountryList.Sort(); 
            return lsCountries;
        }


        public static List<MSTRCountry> GetMS_BaseCountries()
        {
            List<MSTRCountry> lsCountries = new List<MSTRCountry>();

            try
            { 
                List<string> lsCountryList = new List<string>();
                List<BaseCountryModel> countryModels = new List<BaseCountryModel>();

                //CultureInfo[] arrCInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                //foreach (CultureInfo oCInfo in arrCInfoList)
                //{
                //    RegionInfo oCtry = new RegionInfo(oCInfo.LCID);
                //    if (!(lsCountryList.Contains(oCtry.ThreeLetterISORegionName))) //EnglishName
                //    {
                //        lsCountryList.Add(oCtry.EnglishName);

                //        var ctry = new MSTRCountry();
                //        ctry.EngName = oCtry.EnglishName;
                //        ctry.CtryAlpha3Code = oCtry.ThreeLetterISORegionName;
                //        ctry.CtryAlpha2Code = oCtry.TwoLetterISORegionName;
                //        ctry.CurrEngName = oCtry.CurrencyEnglishName;
                //        ctry.CurrLocName = oCtry.CurrencyNativeName;
                //        ctry.CurrSymbol = oCtry.CurrencySymbol;
                //        ctry.Curr3LISOSymbol = oCtry.ISOCurrencySymbol;

                //        lsCountries.Add(ctry);
                //    }
                //}

                /////////////////////////

                try
                {
                    string url = "https://restcountries.eu/rest/v1/all";

                    // Web Request with the given url.
                    WebRequest request = WebRequest.Create(url);

                    if (request != null)
                    {
                        request.Credentials = CredentialCache.DefaultCredentials;

                        WebResponse response = request.GetResponse();

                        if (response != null)
                        {
                            Stream dataStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);

                            string jsonResponse = null;

                            if (reader != null)
                            {
                                // Store the json response into jsonResponse variable.
                                jsonResponse = reader.ReadLine();

                                if (jsonResponse != null)
                                {
                                    // Deserialize the jsonRespose object to the CountryModel. You're getting a JSON array [].
                                    countryModels = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseCountryModel>>(jsonResponse);

                                    // Set the List Item with the countries.
                                    //IEnumerable<SelectListItem> countries = countryModels.Select(x => new SelectListItem() { Value = x.name, Text = x.name });

                                    // Create a ViewBag property with the final content.
                                    //ViewBag.Countries = countries;
                                }
                                //return View();
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    return lsCountries;
                }


                if (countryModels.Count() > 0)
                {
                    foreach (BaseCountryModel baseCountry in countryModels)
                    {
                        // RegionInfo oCtry = new RegionInfo(oCInfo.LCID);
                        if (!(lsCountryList.Contains(baseCountry.alpha3Code))) //EnglishName
                        {
                            lsCountryList.Add(baseCountry.alpha3Code);

                            var ctry = new MSTRCountry();
                            ctry.EngName = baseCountry.name;
                            ctry.CapitalCity = baseCountry.capital;
                            ctry.CtryAlpha3Code = baseCountry.alpha3Code;
                            ctry.CtryAlpha2Code = baseCountry.alpha2Code;
                            ctry.CallingCode = baseCountry.callingCodes.Count > 0 ? baseCountry.callingCodes[0] : null;
                            ctry.CurrEngName = baseCountry.currencies.Count > 0 ? baseCountry.currencies[0] : "";
                            // ctry.CurrLocName = baseCountry.currencies ;
                            ctry.CurrSymbol = baseCountry.currencies.Count > 0 ? baseCountry.currencies[0] : "";
                            ctry.Curr3LISOSymbol = baseCountry.currencies.Count > 0 ? (baseCountry.currencies[0].Length >= 3 ? baseCountry.currencies[0].Substring(0, 3) : baseCountry.currencies[0]) : "";

                            lsCountries.Add(ctry);
                        }
                    }
                }


                //  CountryList.Sort(); 
                return lsCountries;
            }
            catch (Exception ex)
            {
                return lsCountries;
            }    
           
        }


      
        public static List<Country> GetClientCountries()
        {
            List<Country> oCountries = new List<Country>();
            List<string> CountryList = new List<string>();

            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo oCtry = new RegionInfo(CInfo.LCID);
                if (!(CountryList.Contains(oCtry.EnglishName)))
                {
                    CountryList.Add(oCtry.EnglishName);

                    var ctry = new Country();
                    ctry.EngName = oCtry.EnglishName;
                    ctry.CtryAlpha3Code = oCtry.ThreeLetterISORegionName;
                    ctry.CtryAlpha2Code = oCtry.TwoLetterISORegionName;
                    ctry.CurrEngName = oCtry.CurrencyEnglishName;
                    ctry.CurrLocName = oCtry.CurrencyNativeName;
                    ctry.CurrSymbol = oCtry.CurrencySymbol;
                    ctry.Curr3LISOSymbol = oCtry.ISOCurrencySymbol;
                    ///
                    // ctry.IsChurchCountry = true;
                    // ctry.IsDisplay = true;   

                    oCountries.Add(ctry);
                }
            }

            //  CountryList.Sort(); 
            return oCountries;
        }
         
       

        private static List<char> GetUpperCaseChars(int count)
        {
            List<char> result = new List<char>();
            Random random = new Random();

            for (int index = 0; index < count; index++)
            {
                result.Add(Char.ToUpper(Convert.ToChar(random.Next(97, 122))));
            }

            return result;
        }

        private static List<char> GetLowerCaseChars(int count)
        {
            List<char> result = new List<char>();

            Random random = new Random();

            for (int index = 0; index < count; index++)
            {
                result.Add(Char.ToLower(Convert.ToChar(random.Next(97, 122))));
            }

            return result;
        }

        private static List<char> GetNumericChars(int count)
        {
            List<char> result = new List<char>();

            Random random = new Random();

            for (int index = 0; index < count; index++)
            {
                result.Add(Convert.ToChar(random.Next(0, 9).ToString()));
            }

            return result;
        }

        private static string GenerateCodeFromList(List<char> chars)
        {
            string result = string.Empty;

            Random random = new Random();

            while (chars.Count > 0)
            {
                int randomIndex = random.Next(0, chars.Count);
                result += chars[randomIndex];
                chars.RemoveAt(randomIndex);
            }

            return result;
        }


       public static object CalcDateDiff(DateTime _dt1, DateTime _dt2, bool castToString = false, bool isReqAge = false,
           bool res_yr = true, bool res_mo = false, bool res_da = false, bool res_hr = false, bool res_mi = false, bool res_se = false, bool res_ms = false)  // dt1 <= dt2  in yyyymmdd format
        {
            try
            {
                // get exact time of birthday [dt1] ref today [dt2] //_dt2 = DateTime.Now;
                if (isReqAge) 
                    _dt1 = new DateTime(_dt1.Year, _dt1.Month, _dt1.Day, _dt2.Hour, _dt2.Minute, _dt2.Second, _dt2.Millisecond);  


                //var ms = _dt1.Millisecond;
                //var s = _dt1.ToString();
                //_dt1 = _dt2.AddDays(7);

                DateTime dt1, dt2;
                DateTime.TryParse(_dt1.ToString(), out dt1);    //"02/18/2008"
                DateTime.TryParse(_dt2.ToString(), out dt2);    //"02/18/2008"
                //DateTime currentDate = dt2; // DateTime.Now;
                //DateTime previousDate = dt2; // DateTime.Now;


                if (dt2 < dt1)
                    if (castToString)
                        return "N/A";
                    else
                        return 0;

                TimeSpan tdiff = dt2.Subtract(dt1); // : -1 * dt1.Subtract(dt2);

                // This is to convert the timespan to datetime object  
              //  var age = DateTime.MinValue + tdiff;  // DateTime age = DateTime.MinValue + difference; 
                var exactAge = DateTime.MinValue.AddDays(tdiff.Days).AddHours(tdiff.Hours).AddMinutes(tdiff.Minutes).AddSeconds(tdiff.Seconds).AddMilliseconds(tdiff.Milliseconds);

                // Min value is 01/01/0001  
                // Actual age is say 24 yrs, 9 months and 3 days represented as timespan  
                // Min Valye + actual age = 25 yrs , 10 months and 4 days.  
                // subtract our addition or 1 on all components to get the actual date.  


                ////if ((today.getMonth() == birthday.getMonth()) && (today.getDate() == birthday.getDate())) {   
                ////    alert("Happy B'day!!!");
                ////}

                int ageInYears = exactAge.Year > 0 ? exactAge.Year - 1 : exactAge.Year;  // exactAge.Month - 1; 
                int ageInMonths = exactAge.Month > 0 ? exactAge.Month - 1 : exactAge.Month;  // exactAge.Month - 1; week...
                int ageInDays = exactAge.Day > 0 ? exactAge.Day - 1 : exactAge.Day;  // exactAge.Day - 1;
                int ageInHrs = exactAge.Hour > 0 ? exactAge.Hour - 1 : exactAge.Hour;  // exactAge.Hour - 1;
                int ageInMin = exactAge.Minute > 0 ? exactAge.Minute - 1 : exactAge.Minute;  // exactAge.Minute - 1;
                int ageInSec = exactAge.Second > 0 ? exactAge.Second - 1 : exactAge.Second;  // exactAge.Second - 1;
                int ageInMilSec = exactAge.Millisecond > 0 ? exactAge.Millisecond - 1 : exactAge.Millisecond;  // exactAge.Millisecond - 1;

                if (castToString)
                {
                    var ageDesc = "";
                    if (res_yr && ageInYears != 0) ageDesc = ageInYears.ToString() + " years";
                    if (res_mo && ageInMonths != 0) ageDesc += (!string.IsNullOrEmpty(ageDesc.ToString()) ? ", " : "") + ageInMonths.ToString() + " months";
                    if (res_da && ageInDays != 0) ageDesc += (!string.IsNullOrEmpty(ageDesc.ToString()) ? ", " : "") + ageInDays.ToString() + " days";
                    if (res_hr) ageDesc += (!string.IsNullOrEmpty(ageDesc.ToString()) ? ", " : "") + ageInHrs.ToString() + " hours";
                    if (res_mi) ageDesc += (!string.IsNullOrEmpty(ageDesc.ToString()) ? ", " : "") + ageInMin.ToString() + " minutes";
                    if (res_se) ageDesc += (!string.IsNullOrEmpty(ageDesc.ToString()) ? ", " : "") + ageInSec.ToString() + " seconds";
                    if (res_ms) ageDesc += (!string.IsNullOrEmpty(ageDesc.ToString()) ? ", " : "") + ageInMilSec.ToString() + " milliseconds";
                    else
                        ageDesc = ageInYears.ToString() + " years";
                    ///
                    return ageDesc; // (ageInYears != null ? (ageInYears.ToString() + " yrs") : "");
                }                     
                else
                    return ageInYears;



                //// var mdate = dt1.ToString();
                //var yearThen = dt1.Year; // int.Parse(mdate.Substring(0, 4)); //, 10);
                //var monthThen = dt1.Month; // int.Parse(mdate.Substring(5, 2)); //, 10);
                //var dayThen = dt1.Day; // int.Parse(mdate.Substring(7, 2)); //, 10);

                ////var dt2 = new Date();
                //var birthday = new DateTime(yearThen, monthThen - 1, dayThen);
                //var birthday1 = new DateTime(yearThen, monthThen, dayThen- 1);
                //var birthday2 = new DateTime(yearThen, monthThen, dayThen);

                ////System.DateTime dtTodayNoon = new System.DateTime(2018, 9, 13, 12, 0, 0);
                ////System.DateTime dtYestMidnight = new System.DateTime(2018, 9, 12, 0, 0, 0);
                ////System.TimeSpan diffResult = dtTodayNoon - dtYestMidnight;

                //var yearNow = dt2.Year; 
                //var monthNow = dt2.Month;
                //var dayNow = dt2.Day; // 

                //var today = new DateTime(yearNow, monthNow, dayNow);

                //System.TimeSpan diffResult = today - birthday;
                //System.TimeSpan diffResult1 = today - birthday1;
                //System.TimeSpan diffResult2 = today - birthday2;

                //var differenceInMilisecond = diffResult.TotalMilliseconds;  //dt2.valueOf() - .valueOf();
                //var differenceInMilisecond1 = diffResult1.TotalMilliseconds;  //dt2.valueOf() - .valueOf();
                //var differenceInMilisecond2 = diffResult2.TotalMilliseconds;  //dt2.valueOf() - .valueOf();

                //var year_age = Math.Floor(differenceInMilisecond / 31536000000);  // 31536000000 
                //var year_age1 = Math.Floor(differenceInMilisecond1 / 31536000000);
                //var year_age2 = Math.Floor(differenceInMilisecond2 / 31536000000);
                ////  var day_age = Math.floor((differenceInMilisecond % 31536000000) / 86400000);

                ////if ((today.getMonth() == birthday.getMonth()) && (today.getDate() == birthday.getDate())) {   
                ////    alert("Happy B'day!!!");
                ////}

                //int? ageDiff = null;
                //if (year_age >= 0)
                //{

                //    ageDiff = !double.IsNaN(year_age) ? (int)year_age : ageDiff;

                //    //var month_age = Math.floor(day_age / 30);

                //    //day_age = day_age % 30;

                //    //if ( double.IsNaN(year_age))
                //    //{ // || isNaN(month_age) || isNaN(day_age)) {
                //    //    strDiff = "N/A";

                //    //    // $("#_strMemberAge").text("Invalid birthday - Please try again!");
                //    //}
                //    //else
                //    //{
                //    //    strDiff = year_age + " years";

                //    //    //$("#exact_age").html("You are<br/><span id=\"age\">" + year_age + " years " + month_age + " months " + day_age + " days</span> old");
                //    //}
                //}

                //if (castToString)
                //    return (ageDiff != null ? (ageDiff.ToString() + " yrs") : "");
                //else
                //    return ageDiff;

                // alert('age: ' + strAge);
                //$("#_strMemberAge").val(strAge);
                //}
            }
            catch (Exception)
            {
                return "N/A"; // error
            }
             
        }


        public static object CalcDateDiff_PeriodLeft(DateTime _dt1, DateTime _dt2, bool castToString = false, bool isReqAge = false,
           bool res_yr = false, bool res_mo = false, bool res_da = true, bool res_hr = false, bool res_mi = false, bool res_se = false, bool res_ms = false)  // dt1 <= dt2  in yyyymmdd format
        {
            try
            {
                // get exact time of birthday [dt1] ref today [dt2] //_dt2 = DateTime.Now;
                if (isReqAge)
                    _dt1 = new DateTime(_dt1.Year, _dt1.Month, _dt1.Day, _dt2.Hour, _dt2.Minute, _dt2.Second, _dt2.Millisecond);


                //var ms = _dt1.Millisecond;
                //var s = _dt1.ToString();
                //_dt1 = _dt2.AddDays(7);

                DateTime dt1, dt2;
                DateTime.TryParse(_dt1.ToString(), out dt1);    //"02/18/2008"
                DateTime.TryParse(_dt2.ToString(), out dt2);    //"02/18/2008"
                //DateTime currentDate = dt2; // DateTime.Now;
                //DateTime previousDate = dt2; // DateTime.Now;


                if (dt2 < dt1)
                    if (castToString)
                        return "N/A";
                    else
                        return 0;

                TimeSpan tdiff = dt2.Subtract(dt1); // : -1 * dt1.Subtract(dt2);

                // This is to convert the timespan to datetime object  
                //  var age = DateTime.MinValue + tdiff;  // DateTime age = DateTime.MinValue + difference; 
                var exactAge = DateTime.MinValue.AddDays(tdiff.Days).AddHours(tdiff.Hours).AddMinutes(tdiff.Minutes).AddSeconds(tdiff.Seconds).AddMilliseconds(tdiff.Milliseconds);

                // Min value is 01/01/0001  
                // Actual age is say 24 yrs, 9 months and 3 days represented as timespan  
                // Min Valye + actual age = 25 yrs , 10 months and 4 days.  
                // subtract our addition or 1 on all components to get the actual date.  


                ////if ((today.getMonth() == birthday.getMonth()) && (today.getDate() == birthday.getDate())) {   
                ////    alert("Happy B'day!!!");
                ////}

                int remTSpan_Years = exactAge.Year > 0 ? exactAge.Year - 1 : exactAge.Year;  // exactAge.Month - 1; 
                int remTSpan_Months = exactAge.Month > 0 ? exactAge.Month - 1 : exactAge.Month;  // exactAge.Month - 1;
                int remTSpan_Days = exactAge.Day > 0 ? exactAge.Day - 1 : exactAge.Day;  // exactAge.Day - 1;
                int remTSpan_Hrs = exactAge.Hour > 0 ? exactAge.Hour - 1 : exactAge.Hour;  // exactAge.Hour - 1;
                int remTSpan_Min = exactAge.Minute > 0 ? exactAge.Minute - 1 : exactAge.Minute;  // exactAge.Minute - 1;
                int remTSpan_Sec = exactAge.Second > 0 ? exactAge.Second - 1 : exactAge.Second;  // exactAge.Second - 1;
                int remTSpan_MilSec = exactAge.Millisecond > 0 ? exactAge.Millisecond - 1 : exactAge.Millisecond;  // exactAge.Millisecond - 1;

                if (castToString)
                {
                    var remTSpanDesc = "";
                    if (res_yr) remTSpanDesc = remTSpan_Years.ToString() + " years";
                    if (res_mo) remTSpanDesc += (!string.IsNullOrEmpty(remTSpanDesc.ToString()) ? ", " : "") + remTSpan_Months.ToString() + " months";
                    if (res_da) remTSpanDesc += (!string.IsNullOrEmpty(remTSpanDesc.ToString()) ? ", " : "") + remTSpan_Days.ToString() + " days";
                    if (res_hr) remTSpanDesc += (!string.IsNullOrEmpty(remTSpanDesc.ToString()) ? ", " : "") + remTSpan_Hrs.ToString() + " hours";
                    if (res_mi) remTSpanDesc += (!string.IsNullOrEmpty(remTSpanDesc.ToString()) ? ", " : "") + remTSpan_Min.ToString() + " minutes";
                    if (res_se) remTSpanDesc += (!string.IsNullOrEmpty(remTSpanDesc.ToString()) ? ", " : "") + remTSpan_Sec.ToString() + " seconds";
                    if (res_ms) remTSpanDesc += (!string.IsNullOrEmpty(remTSpanDesc.ToString()) ? ", " : "") + remTSpan_MilSec.ToString() + " milliseconds";
                    else 
                        remTSpanDesc = remTSpan_Days.ToString() + " days";   //default

                    ///
                    return remTSpanDesc; // (ageInYears != null ? (ageInYears.ToString() + " yrs") : "");
                }
                else
                {
                    if (res_yr == true) return remTSpan_Years;
                    else if (res_mo == true) return remTSpan_Months;
                    else if (res_da == true) return remTSpan_Days;
                    else if (res_hr == true) return remTSpan_Hrs;
                    else if (res_mi == true) return remTSpan_Min;
                    else if (res_se == true) return remTSpan_Sec;
                    else if (res_ms == true) return remTSpan_MilSec;

                    else return remTSpan_Days; // null;
                }             
            }
            catch (Exception)
            {
                return "N/A"; // error
            }

        }


        public static object _CalcDateDiff(DateTime dt1, DateTime dt2, bool castToString = false)  // dt1 <= dt2  in yyyymmdd format
        {
            try
            {
                var s = dt1.ToString();
                DateTime dateOfBirth;
                DateTime.TryParse(dt1.ToString(), out dateOfBirth);//"02/18/2008"
                DateTime currentDate = dt2; // DateTime.Now;

                TimeSpan difference = currentDate.Subtract(dateOfBirth);

                // This is to convert the timespan to datetime object  
                DateTime age = DateTime.MinValue + difference;

                // Min value is 01/01/0001  
                // Actual age is say 24 yrs, 9 months and 3 days represented as timespan  
                // Min Valye + actual age = 25 yrs , 10 months and 4 days.  
                // subtract our addition or 1 on all components to get the actual date.  

                int ageInYears = age.Year - 1;
                int ageInMonths = age.Month - 1;
                int ageInDays = age.Day - 1;

                 
                int? ageDiff = null;
                if (ageInYears >= 0)
                {

                    ageDiff = !double.IsNaN(ageInYears) ? (int)ageInYears : ageDiff;

                    //var month_age = Math.floor(day_age / 30);

                    //day_age = day_age % 30;

                    //if ( double.IsNaN(year_age))
                    //{ // || isNaN(month_age) || isNaN(day_age)) {
                    //    strDiff = "N/A";

                    //    // $("#_strMemberAge").text("Invalid birthday - Please try again!");
                    //}
                    //else
                    //{
                    //    strDiff = year_age + " years";

                    //    //$("#exact_age").html("You are<br/><span id=\"age\">" + year_age + " years " + month_age + " months " + day_age + " days</span> old");
                    //}
                }

                if (castToString)
                    return (ageDiff != null ? (ageDiff.ToString() + " yrs") : "");
                else
                    return ageDiff;

                // alert('age: ' + strAge);
                //$("#_strMemberAge").val(strAge);
                //}
            }
            catch (Exception)
            {
                return "";
            }

        }














        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <param name="_context"></param>
        /// <param name="churchCode"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// 
        private const string ac1 = "91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203";
        //private const string ac2 = "f7b11509f4d675c3c44f0dd37ca830bb02e8cfa58f04c46283c4bfcbdce1ff45";
        //private const string ac3 = "78415a1535ca0ef885aa7c0278a4de274b85d0c139932cc138ba6ee5cac4a00b";

        public static UserSessionPrivilege ValidateUser(MSTR_DbContext _context, ChurchModelContext _clientDBContext, string churchCode, string username, string password)
        {
            try
            {
                //  var temp = AppUtilties.ComputeSha256Hash("dabrokwah" + "$rhemacloud");
                string strPwdHashedData = AppUtilties.ComputeSha256Hash(churchCode + username + password);
                UserProfile oUser = null;
                //if (username == "test" || username == "test1")
                //{
                //    //oUser = _context.UserProfile //.Include(u => u.ChurchBody).Include(u => u.ChurchMember)
                //    //                .Where(c => c.ChurchBody.ChurchCode == churchCode && c.ChurchMember.IsCurrent && c.Username == username).FirstOrDefault(); // && c.Pwd.Trim() == strPwdHashedData.Trim() && c.UserStatus == "A").FirstOrDefault();

                //    oUser = (from t_up in _context.UserProfile.Include(t => t.ChurchBody) //.Include(t => t.ChurchMember)
                //                       .Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.Username == username)
                //             from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                //             select t_up
                //                      ).FirstOrDefault();
                //}

                if (AppUtilties.ComputeSha256Hash(churchCode) == ac1) //&& AppUtilties.ComputeSha256Hash(churchCode + username) == ac2 && AppUtilties.ComputeSha256Hash(churchCode + username + password) == ac3)  //6-digit vendor code 6-digit code for churches ... [church code: 0000000000 + ?? userid + ?? pwd] + no existing SUPADMIN user ... pop up SUPADMIN for new SupAdmin()
                {
                    // string strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode); //church code ...0x6... 91b4d142823f7d20c5f08df69122de43f35f057a988d9619f6d3138485c9a203
                    //  string _strRootCode = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username);  // user  ...0x6... f7b11509f4d675c3c44f0dd37ca830bb02e8cfa58f04c46283c4bfcbdce1ff45
                    //  string strRootCode0 = AppUtilties.ComputeSha256Hash(model.ChurchCode + model.Username + model.Password);  // pwd ...$0x6... 78415a1535ca0ef885aa7c0278a4de274b85d0c139932cc138ba6ee5cac4a00b

                    //var userList = (from t_up in _context.UserProfile.Where(c => c.ProfileScope == "V" && c.UserStatus == "A")
                    //                from t_upr in _context.UserProfileRole.Where(c => c.UserProfileId == t_up.Id && c.ProfileRoleStatus == "A")
                    //                from t_ur in _context.UserRole.Where(c => c.Id == t_upr.UserRoleId && c.RoleStatus == "A" && c.RoleLevel == 1 && c.RoleType == "SUP_ADMN")
                    //                select t_up
                    //                ).ToList();

                    ////if no SUP_ADMIN... create one and only 1... then create other users
                    //if (userList.Count == 0)
                    //{
                    //    //...
                    //}
                    //else
                    //{

                    oUser = (from t_up in _context.UserProfile.AsNoTracking().Include(t => t.ChurchBody)  //.Include(t => t.ChurchMember)
                                  .Where(c => c.ProfileScope == "V" && c.UserStatus == "A" && c.Username == username && c.Pwd.Trim() == strPwdHashedData.Trim())
                                 // from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                             select t_up
                                 ).FirstOrDefault();

                    //}
                }

                else
                {
                    //  oUser = _context.UserProfile
                    //.Include(u => u.ChurchBody).Include(u => u.ChurchMember)
                    //.Where(c => c.ChurchBody.ChurchCode == churchCode && c.ChurchMember.IsCurrent && c.Username == username && c.Pwd.Trim() == strPwdHashedData.Trim() && c.UserStatus == "A").FirstOrDefault();


                    //oUser = (from t_up in _context.UserProfile.Include(t => t.ChurchBody)  //.Include(t => t.ChurchMember)
                    //                   .Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.ProfileScope == "C" && c.Username == username && c.Pwd.Trim() == strPwdHashedData.Trim() && c.UserStatus == "A")
                    //         from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                    //         select t_up
                    //                  ).FirstOrDefault();

                    oUser = (from t_up in _context.UserProfile.AsNoTracking()  //.Include(t => t.ChurchBody)  //.Include(t => t.ChurchMember)
                                       .Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.ProfileScope == "C" && c.UserStatus == "A" && c.Username == username && c.Pwd.Trim() == strPwdHashedData.Trim())
                             //from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == t_up.ChurchMemberId)
                             select t_up
                                      ).FirstOrDefault();

                    //if (oUser != null) {
                    //    var chkUserMemExist = (
                    //        from t_ms in _clientDBContext.MemberStatus.Where(c => c.ChurchBody.GlobalChurchCode == churchCode && c.IsCurrent == true && c.ChurchMemberId == oUser.ChurchMemberId)
                    //        select t_ms
                    //                 ).FirstOrDefault();

                    //    //making sure user is active member of the church  ... might not be compulsory anyway... cos church may employ persons from other faiths
                    //    oUser = chkUserMemExist != null ? oUser : null; 
                    //}
                    
                }


                //var oUser = new UserProfile();
                //var oUserRole = new UserProfileRole();           

                if (oUser != null)
                { 
                        var oLoggedPrivileges = AppUtilties.GetUserPrivilege(_context, _clientDBContext, churchCode, oUser);

                        if (oLoggedPrivileges?.UserSessionPermList?.Count > 0) //i.e. at least have a permission == from groups and roles
                        {
                            // TempData.Put("oUserProRoleLogIn", oUserProRoleLog);
                            return oLoggedPrivileges;
                        }                   
                }

                return null; //ViewBag.UserPromptMsg = "Invalid credentials provided. Enter right username and password.";



                //// This is a simple single-user system
                //if (username == "keyvan" && password == "pas$word")
                //    return true;
                //return false;

            }
            catch (Exception ex)
            {
                return null; // ex;
            }
        }


        public static string GetUserPhone(MSTR_DbContext _context, string username)
        {
            UserProfile oUserLog = _context.UserProfile.AsNoTracking().Include(t=>t.ContactInfo).Where(c => c.Username == username
            //&& c.Pwd.Trim() == strPwdHashedData.Trim() 
            && c.UserStatus == "A")
                .FirstOrDefault();

            if (oUserLog != null)
            {
                if (oUserLog.ContactInfo != null)
                {
                    if (!string.IsNullOrEmpty(oUserLog.ContactInfo.MobilePhone1))
                        return oUserLog.ContactInfo.MobilePhone1;
                    else
                        return oUserLog.ContactInfo.MobilePhone2;
                } 
            }
            // This is a simple single-user system
            // if (username == "keyvan") return "+1YOURPHONE";

            return string.Empty;
        }

        public static string ReadValidationCode(string username)
        {
            // string path = HttpContext.Current.Server.MapPath("~/App_Data/usercodes.xml");
            // string path = _context.Request.PathBase
            //XDocument doc = XDocument.Load(path);
            //string code = (from u in doc.Element("Users").Descendants("User")
            //               where u.Attribute("name").Value == username
            //               select u.Attribute("code").Value).SingleOrDefault();

            string code = "";

            return code;
        }

        public static void StoreValidationCode(string username, string code)
        {
            //string path = HttpContext.Current.Server.MapPath("~/App_Data/usercodes.xml");

            //XDocument doc = XDocument.Load(path);
            //XElement user = (from u in doc.Element("Users").Descendants("User")
            //                 where u.Attribute("name").Value == username
            //                 select u).SingleOrDefault();

            //if (user != null)
            //{
            //    user.Attribute("code").SetValue(code);
            //}
            //else
            //{
            //    XElement newUser = new XElement("User");
            //    newUser.SetAttributeValue("name", username);
            //    newUser.SetAttributeValue("code", code);
            //    doc.Element("Users").Add(newUser);
            //}
            //doc.Save(path); 
        }




        public static class TrustedClients
        {
            public static bool ValidateClient(string username, string ip, string useragent)
            {
                //string path = HttpContext.Current.
                //      Server.MapPath("~/App_Data/trustedclients.xml");

                //XDocument doc = XDocument.Load(path);
                //var client = (from c in doc.Element("Clients").Descendants("Client")
                //              where
                //              ((c.Attribute("username").Value == username) &&
                //              (c.Attribute("ip").Value == ip) &&
                //              (c.Attribute("useragent").Value == useragent))
                //              select c).SingleOrDefault();

                //if (client != null)
                //    return true;

                return false;
            }

            public static void AddClient(string username, string ip, string useragent)
            {
                //string path = HttpContext.Current.
                //    Server.MapPath("~/App_Data/trustedclients.xml");

                //XDocument doc = XDocument.Load(path);
                //XElement newClient = new XElement("Client");

                //newClient.SetAttributeValue("username", username);
                //newClient.SetAttributeValue("ip", ip);
                //newClient.SetAttributeValue("useragent", useragent);

                //doc.Element("Clients").Add(newClient);

                //doc.Save(path);
            }
 

        }





        //using Twilio;
        //public static class TwilioMessenger
        //{
        //    public static void SendTextMessage(string number, string message)
        //    {
        //        TwilioRestClient twilioClient =
        //            new TwilioRestClient("acccountSid", "authToken");
        //        twilioClient.SendSmsMessage("+12065696562", number, message);
        //    }
        //}



        public static bool SendEmailNotification(string senderId, string strSubject, string strMailMess,
            MailAddressCollection lsToAddr, MailAddressCollection lsCcAddr, MailAddressCollection lsBccAddr, string docAttachFilePath, bool _isBodyHtml = false)
        //HttpPostedFileBase fileUploader) SendMailwithAttachment.Models.MailModel objModelMail
        {
            try
            {
                string strFrom = "rhemaadmin@rhema-systems.com"; // "samdartgroup@gmail.com"; //example:- sourabh9303@gmail.com  //  var fromAddr = new MailAddress(from, "RHEMACLOUD"); 
                string strPwdFrom = "Rj45td852#"; // "Sdgh1284";
                using (MailMessage mail = new MailMessage()) // strFrom, senderId))
                {
                    mail.From = new MailAddress(strFrom, senderId);
                    mail.Subject = strSubject;
                    mail.Body = strMailMess;
                    foreach (var oTo in lsToAddr) mail.To.Add(oTo);
                    foreach (var oCc in lsCcAddr) mail.To.Add(oCc);
                    foreach (var oBcc in lsBccAddr) mail.To.Add(oBcc);

                    //if (fileUploader != null)
                    //{
                    //    string fileName = Path.GetFileName(fileUploader.FileName);
                    //    mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
                    //}

                    mail.IsBodyHtml = _isBodyHtml;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.Office365.com"; // "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential networkCredential = new NetworkCredential(strFrom, strPwdFrom);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = networkCredential;
                    smtp.Port = 587;
                    smtp.Send(mail);
                    // ViewBag.Message = "Sent";

                    return true;  // View("Index", objModelMail);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
         
      
        public static bool SendSMSNotification(string strPhone, string strPostalCode, string strMsg)
        {
            if (strPhone == null || strPhone == "") return false;

            if (strPhone.Length <= 10 && !strPhone.StartsWith(strPostalCode))
                strPhone = strPostalCode + strPhone.Substring(1, strPhone.Length - 1);

            var oSendSMS = new AppUtility_SMS();

            //pick SMS Sender ID from db
            bool resMsg = oSendSMS.sendMessage("RHEMAChurch", strPhone, strMsg, "sdarteh", "Sdgh?2020"); // ChurchLITE
            return resMsg;

            //if (resMsg)
            //    MessageBox.Show("Message sent successfully")
            //    else
            //    MessageBox.Show("Failed sending message to some recipient(s).");
            //End If
        }

        public static bool SendSMSNotification(List<string> oMSISDN_List, List<string> strMsgList, bool customMsg = false)
        {
            try
            {
                if (customMsg)
                { if (strMsgList.Count == 0) return false; }
                else
                { if (strMsgList.Count != oMSISDN_List.Count) return false; }

                var successCount = 0; var msgResponse = false;
                for (var i = 0; i < oMSISDN_List.Count; i++)
                {
                    //var oSendSMS = new AppUtility_SMS();
                    //bool msgResponse = oSendSMS.sendMessage("RHEMAChurch", oMSISDNList[i], customMsg ? strMsgList[i] : strMsgList[0], "sdarteh", "Sdgh?2020");

                    //string strFrom = "rhemaadmin@rhema-systems.com"; // "samdartgroup@gmail.com"; //example:- sourabh9303@gmail.com  //  var fromAddr = new MailAddress(from, "RHEMACLOUD"); 
                    //string strPwdFrom = "Rj45td852#"; // "Sdgh1284";

                    if (oMSISDN_List[i] != null && strMsgList[i] != null)   // "RHEMAChurch"
                        msgResponse = new AppUtility_SMS().sendMessage("RHEMAChurch", oMSISDN_List[i], customMsg ? strMsgList[i] : strMsgList[0], "sdarteh", "Sdgh?2020");

                    if (msgResponse) successCount++;
                }

                //write fail to logs...
                return successCount >= 0;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsValidEmail(string _email)
        {
            if (string.IsNullOrWhiteSpace(_email))
                return false;

            try
            {
                // Normalize the domain
                _email = Regex.Replace(_email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(_email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool IsValidURL(string _url)
        {
            if (string.IsNullOrWhiteSpace(_url))
                return false;

            try
            {
                string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
                Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return Rgx.IsMatch(_url);
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }
        }




        public static class CodeGenerator
        {
            public static string GenerateCode(int digitLen = 4, int alphaLen = 4, bool option1Alpha = true, bool option2Digit = true)
            {
                List<char> chars = new List<char>();

                if (option1Alpha) chars.AddRange(GetUpperCaseChars(alphaLen)); else chars.AddRange(GetNumericChars(digitLen));
                if (option2Digit) chars.AddRange(GetNumericChars(digitLen)); else chars.AddRange(GetUpperCaseChars(alphaLen));               

                return GenerateCodeFromList(chars);
            }

            private static List<char> GetUpperCaseChars(int count)
            {
                List<char> result = new List<char>();
                Random random = new Random();

                for (int index = 0; index < count; index++)
                {
                    result.Add(Char.ToUpper(Convert.ToChar(random.Next(97, 122))));
                }

                return result;
            }

            private static List<char> GetLowerCaseChars(int count)
            {
                List<char> result = new List<char>();

                Random random = new Random();

                for (int index = 0; index < count; index++)
                {
                    result.Add(Char.ToLower(Convert.ToChar(random.Next(97, 122))));
                }

                return result;
            }

            private static List<char> GetNumericChars(int count)
            {
                List<char> result = new List<char>();

                Random random = new Random();

                for (int index = 0; index < count; index++)
                {
                    result.Add(Convert.ToChar(random.Next(0, 9).ToString()));
                }

                return result;
            }

            private static string GenerateCodeFromList(List<char> chars)
            {
                string result = string.Empty;

                Random random = new Random();

                while (chars.Count > 0)
                {
                    int randomIndex = random.Next(0, chars.Count);
                    result += chars[randomIndex];
                    chars.RemoveAt(randomIndex);
                }

                return result;
            }
        }



    }
}
