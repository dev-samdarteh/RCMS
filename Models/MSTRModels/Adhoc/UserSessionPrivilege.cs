using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.MSTRModels
{ 
    public class UserSessionPrivilege
    {

        public UserSessionPrivilege()
        { }

        public MSTRAppGlobalOwner AppGlobalOwner { get; set; }
        public MSTRChurchBody ChurchBody { get; set; }
        public RhemaCMS.Models.CLNTModels.AppGlobalOwner AppGlobalOwner_CLNT { get; set; }
        public RhemaCMS.Models.CLNTModels.ChurchBody ChurchBody_CLNT { get; set; }
        public UserProfile UserProfile { get; set; } 
        public List<UserRole> UserRoles { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<UserPermission> UserPermissions { get; set; }
        public List<UserSessionPerm> UserSessionPermList { get; set; }

        // public string logUserDesc { get; set; }
        //public string PermissionCode { get; set; }
        //public string PermissionName { get; set; }
        //public bool PermissionValue { get; set; }

        public List<string> arrAssignedModCodes { get; set; }  //  collection of user role (s) 
        public List<string> arrAssignedRoleCodes { get; set; }  //  collection of user role (s) 
        public List<string> arrAssignedRoleNames { get; set; }  //  collection of user role (s) 
        public List<string> arrAssignedGroupCodes { get; set; }  // collection of user group (s)
        public List<string> arrAssignedGroupNames { get; set; }  // collection of user group (s)
        public List<string> arrAssignedPermCodes { get; set; }  // collection of user group (s)
        public string strChurchCode_AGO { get; set; }
        public string strChurchCode_CB { get; set; }

        //public bool ViewPerm { get; set; }
        //public bool CreatePerm { get; set; }
        //public bool EditPerm { get; set; }
        //public bool DeletePerm { get; set; }
        //public bool ManagePerm { get; set; }

        //  public List<UserSessionPrivilege> oUserLogIn_Permissions { get; set; }        
        public bool IsModAccessVAA0 { get; set; }  // A0 -- Vendor Admin Module        
        public bool IsModAccessVAA4 { get; set; }  // A0 -- Vendor Admin Assist        
        public bool IsModAccessDS00 { get; set; }  // 00 -- Dashboard Module        
        public bool IsModAccessAC01 { get; set; }  // 01 -- App Config Module -- Admin         
        public bool IsModAccessMR02 { get; set; }  // 02 -- Mem Reg Module        
        public bool IsModAccessCL03 { get; set; }  // 03 -- Chu Life Module        
        public bool IsModAccessCA04 { get; set; }  // 04 -- Chu Admin Module        
        public bool IsModAccessFM05 { get; set; }  // 05 -- Fin Module        
        public bool IsModAccessRA06 { get; set; }  // 06 -- Reports and Analytics     
         
    }

    public class UserSessionPerm
    {
        public UserSessionPerm()
        { }

        public UserRole UserRole { get; set; }
        public UserGroup UserGroup { get; set; }
        public UserPermission UserPermission { get; set; }

        public int? oAppGlobalOwnerId { get; set; }
        public int? oChurchBodyId { get; set; }

        public int? oUserPermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public string strPermission { get; set; }

        //  public bool PermissionValue { get; set; }  // ??? i have forgotten what i really used this for... OMG!

        public int? oUserRoleId { get; set; }  // URP
        public int? oUserGroupId { get; set; } // URP 

        public DateTime? oCreated { get; set; }
        public int? oCreatedByUserId { get; set; }
        public DateTime? oLastMod  { get; set; }
        public int? oLastModByUserId { get; set; }

        public bool ViewPerm { get; set; }
        public bool CreatePerm { get; set; }
        public bool EditPerm { get; set; }
        public bool DeletePerm { get; set; }
        public bool ManagePerm { get; set; }

        public string strRoleName { get; set; }
        public string strRoleCode { get; set; }
        public string strGroupName { get; set; } 
        public string strGroupCode { get; set; } 

        public string strPermModule { get; set; } 
    }
    



    //public class DiscreteLookup
    //{
    //    public DiscreteLookup() { }

    //    public string Val { get; set; }
    //    public string Desc { get; set; }
    //    public string Category { get; set; }

    //    public List<DiscreteLookup> EntityStatusList { get; set; }
    //}

    //public class NumberDiscreteLookup
    //{
    //    public NumberDiscreteLookup() { }

    //    public decimal Val { get; set; }
    //    public string Desc { get; set; }
    //    public string Category { get; set; }

    //    public List<NumberDiscreteLookup> EntityStatusList { get; set; }
    //}
}

 