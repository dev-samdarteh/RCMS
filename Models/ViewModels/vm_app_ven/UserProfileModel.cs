using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace RhemaCMS.Models.ViewModels.vm_app_ven
{
    public class UserProfileModel
    {
        public UserProfileModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        public string strAppCurrUser_RoleCateg { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oAppGloOwnId_CLNT { get; set; }
        public int? oChurchBodyId { get; set; }
        //public int? oChurchBodyId_CLNT { get; set; }
        public MSTRAppGlobalOwner oAppGlobalOwn { get; set; }
       // public CLNTModels.AppGlobalOwner oAppGlobalOwn_CLNT { get; set; }
        // public ChurchLevel oChurchLevel { get; set; }
        public MSTRChurchBody oChurchBody { get; set; }  // grace
      //  public CLNTModels.ChurchBody oChurchBody_CLNT { get; set; }  // grace
        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oAppGloOwnId_Logged_CLNT { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oChurchBodyId_Logged_CLNT { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oCurrUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }
        public int? oUserProfileLevel_Logged { get; set; }
        public string oUserProfileScope_Logged { get; set; }

        public UserProfile  oUserProfile_Logged { get; set; }
        public string oUserRoleLevel_Logged { get; set; }
        public string arrUserRoleCodes_Logged { get; set; }
        public string arrUserPermCodes_Logged { get; set; }

        public MSTRAppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public CLNTModels.AppGlobalOwner oAppGlobalOwn_Logged_CLNT { get; set; }
        public MSTRChurchBody oChurchBody_Logged { get; set; }
        public CLNTModels.ChurchBody oChurchBody_Logged_CLNT { get; set; }
        public RhemaCMS.Models.CLNTModels.ChurchMember oCurrLoggedMember { get; set; }
        public UserProfile oChurchAdminProfile { get; set; }
        public int setIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; } 
                
        //
        public List<UserProfileModel> lsUserProfileModels { get; set; }
        public List<UserProfile> lsUserProfiles { get; set; }
        public UserProfile oUserProfile { get; set; }
        //
        //// public List<UserRoleModel> lsUserRoleModels { get; set; }
        //public List<UserRole> lsUserRoles { get; set; }
        //public UserRole oUserRole { get; set; }
        ////
        //// public List<UserPermissionModel> lsUserPermissionModels { get; set; }
        //public List<UserPermission> lsUserPermissions { get; set; }
        //public UserPermission oUserPermission { get; set; }

        //public List<UserAuditTrail> lsUserAuditTrails { get; set; }
        //public UserAuditTrail oUserAuditTrail { get; set; }
        //
              
        public List<UserProfileRole> lsUserProfileRoles { get; set; }
        public List<UserProfileRole> lsUserProfileRoles_All { get; set; }
        public List<UserRole> lsUserRoles { get; set; }
        public List<UserRole> lsUserRolesAll { get; set; }

        public List<UserProfileGroup> lsUserProfileGroupsAll { get; set; }
        public List<UserProfileGroup> lsUserProfileGroups { get; set; }
        public List<UserGroup> lsUserGroups { get; set; }
        public List<UserGroup> lsUserGroupsAll { get; set; }
        public List<UserGroupRole> lsUserGroupRoles { get; set; }

        public List<UserRolePermission> lsUserRolePermissions { get; set; }
        public List<UserPermission> lsUserPermissions { get; set; }
        public List<UserPermission> lsUserPermissionsAll { get; set; }
        public List<UserSessionPerm> lsUserSessionPerms { get; set; }  // mapping or user role perm and user group perm

        public List<UserAuditTrail> lsUserAuditTrails { get; set; }
        public List<UserSessionPerm> lsUserSessionPermList { get; set; }

        //  public List<UserProfileRoleModel> lsUserProfileRoleModels { get; set; }

        // public UserProfileRole oUserProfileRole { get; set; }
        //public UserProfile oUserProfile { get; set; }
        // public UserRole oUserRole { get; set; }
        //
       // public UserProfile oUserProfile { get; set; }         
        

       // public List<UserGroup> lsUserGroups { get; set; }
      //  public List<UserRole> lsUserRoles { get; set; }
       // public List<UserPermission> lsUserPermissions { get; set; }
        //        
        public List<string> arrAssignedModCodes { get; set; }  //  module(s)
        public List<string> arrAssignedRoleCodes { get; set; }  //  collection of user role (s) 
        public List<string> arrAssignedRoleNames { get; set; }  //  collection of user role (s) 
       public List<string> arrAssignedGroupCodes { get; set; }  // collection of user group (s)
        public List<string> arrAssignedGroupNames { get; set; }  // collection of user group (s)
        public List<string> arrAssignedPermCodes { get; set; }   // collection of user group (s)
         

        [StringLength(1)]
        public string profileScope { get; set; }
        [StringLength(1)]
        public string subScope { get; set; }

        //    
        public List<SelectListItem> lkpDenominations { set; get; }
        public List<SelectListItem> lkpChurchMembers { set; get; }
        public List<SelectListItem> lkpOwnerUsers { set; get; }
        public List<SelectListItem> lkpCongregations { set; get; }
        public List<SelectListItem> lkpTargetCongregations { set; get; }
        public List<SelectListItem> lkpCongNextCategory { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }
        public List<SelectListItem> lkpUserProfileLevels { set; get; }
        public List<SelectListItem> lkpUserTypes { set; get; }
        public List<SelectListItem> lkpPwdSecQueList { set; get; }
        public List<SelectListItem> lkpPwdSecAnsList { set; get; }

        public List<SelectListItem> lkp_CongNextCategory { set; get; }
        public List<SelectListItem> lkp_ToCongregations { set; get; }  // of same denomination except curr cong

        public int numCLIndex { get; set; }
        public int oCBLevelCount { get; set; }

        public string strChurchLevel_1 { get; set; }
        public string strChurchLevel_2 { get; set; }
        public string strChurchLevel_3 { get; set; }
        public string strChurchLevel_4 { get; set; }
        public string strChurchLevel_5 { get; set; }
        public string strChurchLevel_6 { get; set; }
        public int? ChurchBodyId_1 { get; set; }
        public int? ChurchBodyId_2 { get; set; }
        public int? ChurchBodyId_3 { get; set; }
        public int? ChurchBodyId_4 { get; set; }
        public int? ChurchBodyId_5 { get; set; }
        public int? ChurchBodyId_6 { get; set; }

        //
        //  public int? oChurchBodyId_1 { set; get; }
        public string strChurchBody_1 { set; get; }
        //
        public List<SelectListItem> lkp_ChurchBodies_1 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_2 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_3 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_4 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_5 { set; get; }



        public string strChurchBody { get; set; }
        public string strRootChurchBodyCode { get; set; }
        public string strCBLevel { get; set; }

        public string strAppGlobalOwn { get; set; }
        public string strChurchLevel { get; set; }
        public string strUserProfile { get; set; }
        // public string strDenomination { get; set; }
        public string strChurchMember { get; set; }
        public string strOwnerUser { get; set; }
        public string strUserStatus { get; set; }
        public bool isVendorOwned { get; set; }  // cannot be deleted by client
        public string strSTRT { get; set; }  
        public string strEXPR { get; set; }  

        [Display(Name = "User Photo")]
        public IFormFile UserPhotoFile { get; set; }
        public string strUserPhoto { get; set; }
         

        public List<SelectListItem> lkpAppGlobalOwns { get; set; }
        public List<SelectListItem> lkpChurchLevels { get; set; }
        public List<SelectListItem> lkpUserProfiles { get; set; }
        public List<SelectListItem> lkpUserRoles { set; get; }
        public List<SelectListItem> lkpUserGroups { set; get; }
        public List<SelectListItem> lkpUserPermissions { set; get; }
        public string strUserRoleName { get; set; }
        public int? numUserRoleLevel { get; set; }
        public string strUserGroupName { get; set; }
        public string strUserPermission { get; set; }
    }


    //public class UserProfileRoleModel
    //{
    //    public UserProfileRoleModel() { }

    //    public string strAppName { get; set; }
    //    public string strAppNameMod { get; set; }
    //    public string strAppCurrUser { get; set; }
    //    public string strCurrTask { get; set; }
    //    public string strAppCurrUser_RoleCateg { get; set; }
    //    // 

    //    public int? oAppGloOwnId { get; set; }
    //    public int? oAppGloOwnId_CLNT { get; set; }
    //    public int? oChurchBodyId { get; set; }
    //    //public int? oChurchBodyId_CLNT { get; set; }
    //    public MSTRAppGlobalOwner oAppGlobalOwn { get; set; }
    //    // public CLNTModels.AppGlobalOwner oAppGlobalOwn_CLNT { get; set; }
    //    // public ChurchLevel oChurchLevel { get; set; }
    //    public MSTRChurchBody oChurchBody { get; set; }  // grace
    //                                                     //  public CLNTModels.ChurchBody oChurchBody_CLNT { get; set; }  // grace
    //                                                     //
    //    public int? oAppGloOwnId_Logged { get; set; }
    //    public int? oAppGloOwnId_Logged_CLNT { get; set; }
    //    public int? oChurchBodyId_Logged { get; set; }
    //    public int? oChurchBodyId_Logged_CLNT { get; set; }
    //    public int? oCurrMemberId_Logged { get; set; }
    //    public int? oUserId_Logged { get; set; }
    //    //public int? oMemberId_Logged { get; set; }
    //    //public int? oCurrUserId_Logged { get; set; }
    //    //public string oUserRole_Logged { get; set; }
    //    //public int? oUserProfileLevel_Logged { get; set; }

    //    public MSTRAppGlobalOwner oAppGlobalOwn_Logged { get; set; }
    //    public CLNTModels.AppGlobalOwner oAppGlobalOwn_Logged_CLNT { get; set; }
    //    public MSTRChurchBody oChurchBody_Logged { get; set; }
    //    public CLNTModels.ChurchBody oChurchBody_Logged_CLNT { get; set; }
    //    public RhemaCMS.Models.CLNTModels.ChurchMember oCurrLoggedMember { get; set; }
    //    //public UserProfile oChurchAdminProfile { get; set; }
    //    public int setIndex { get; set; }
    //    public int subSetIndex { get; set; }
    //    public int pageIndex { get; set; }

    //    //  public int numCLIndex { get; set; }


    //    //
    //    public List<UserProfileRoleModel> lsUserProfileRoleModels { get; set; }
    //    public List<UserProfileRole> lsUserProfileRoles { get; set; }
    //    public UserProfileRole oUserProfileRole { get; set; }
    //    public UserProfile oUserProfile { get; set; }
    //    public UserRole oUserRole { get; set; }
    //    //
    //    public UserProfile UserProfile { get; set; }
    //    public List<UserRole> UserRoles { get; set; }
    //    public List<UserGroup> UserGroups { get; set; }
    //    public List<UserPermission> UserPermissions { get; set; }
    //    public List<UserSessionPerm> UserSessionPermList { get; set; }
    //    public List<UserAuditTrail> lsUserAuditTrails { get; set; }

    //    public List<UserProfileGroup> lsUserProfileGroups { get; set; }
    //   // public List<UserProfileRole> lsUserProfileRoles { get; set; }
    //    public List<UserRolePermission> lsUserRolePermissions { get; set; }
    //    public List<UserGroupPermission> lsUserGroupPermissions { get; set; }
    //    public List<UserGroup> lsUserGroups { get; set; }
    //    public List<UserSessionPrivilege> lsUserPrivileges { get; set; }
    //    public UserGroup oUserGroup { get; set; }

    //   // public UserProfileRole oUserProfileRole { get; set; }
    //    public UserProfileGroup oUserProfileGroup { get; set; }
    //    public UserGroupPermission oUserGroupPermission { get; set; }
    //    public UserRolePermission oUserRolePermission { get; set; }
    //    //
    //    public List<string> arrAssignedModCodes { get; set; }  //  collection of user role (s) 
    //    public List<string> arrAssignedRoleCodes { get; set; }  //  collection of user role (s) 
    //    public List<string> arrAssignedRolesDesc { get; set; }  //  collection of user role (s) 
    //    public List<string> arrAssignedGroupsDesc { get; set; }  // collection of user group (s)
    //    public List<string> arrAssignedGroupNames { get; set; }  // collection of user group (s)
    //    public List<string> arrAssignedPermCodes { get; set; }  // collection of user group (s)



    //    [StringLength(1)]
    //    public string profileScope { get; set; }
    //    [StringLength(1)]
    //    public string subScope { get; set; }

    //    //    
    //    public List<SelectListItem> lkpDenominations { set; get; }
    //    public List<SelectListItem> lkpChurchMembers { set; get; }
    //    public List<SelectListItem> lkpOwnerUsers { set; get; }
    //    public List<SelectListItem> lkpCongregations { set; get; }
    //    public List<SelectListItem> lkpTargetCongregations { set; get; }
    //    public List<SelectListItem> lkpCongNextCategory { set; get; }
    //    public List<SelectListItem> lkpStatuses { set; get; }
    //    public List<SelectListItem> lkpUserTypes { set; get; }
    //    public List<SelectListItem> lkpPwdSecQueList { set; get; }
    //    public List<SelectListItem> lkpPwdSecAnsList { set; get; }

    //    public List<SelectListItem> lkp_CongNextCategory { set; get; }
    //    public List<SelectListItem> lkp_ToCongregations { set; get; }  // of same denomination except curr cong


    //    public string strChurchBody { get; set; }
    //    public string strAppGlobalOwn { get; set; }
    //    public string strChurchLevel { get; set; }
    //    public string strUserProfile { get; set; } 
    //    public string strChurchMember { get; set; }
    //    public string strOwnerUser { get; set; }
    //    public string strUserStatus { get; set; }
    //    public bool isVendorOwned { get; set; }  // cannot be deleted by client

    //    [Display(Name = "User Photo")]
    //    public IFormFile UserPhotoFile { get; set; }
    //    public string strUserPhoto { get; set; }
    //    // 

    //    public List<SelectListItem> lkpAppGlobalOwns { get; set; }
    //    public List<SelectListItem> lkpChurchLevels { get; set; }
    //    public List<SelectListItem> lkpUserProfiles { get; set; }
    //    public List<SelectListItem> lkpUserRoles { set; get; }
    //    public List<SelectListItem> lkpUserGroups { set; get; }
    //    public List<SelectListItem> lkpUserPermissions { set; get; }
    //    public string strUserRole { get; set; }
    //    public int? numUserRoleLevel { get; set; }
    //    public string strUserGroup { get; set; }
    //    public string strUserPermission { get; set; }
    //}

    public class UserProfileRoleModel
    {
        public UserProfileRoleModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        public string strAppCurrUser_RoleCateg { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oAppGloOwnId_CLNT { get; set; }
        public int? oChurchBodyId { get; set; }
        //public int? oChurchBodyId_CLNT { get; set; }
        public MSTRAppGlobalOwner oAppGlobalOwn { get; set; }
        // public CLNTModels.AppGlobalOwner oAppGlobalOwn_CLNT { get; set; }
        // public ChurchLevel oChurchLevel { get; set; }
        public MSTRChurchBody oChurchBody { get; set; }  // grace
                                                         //  public CLNTModels.ChurchBody oChurchBody_CLNT { get; set; }  // grace
                                                         //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oAppGloOwnId_Logged_CLNT { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oChurchBodyId_Logged_CLNT { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oCurrUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }
        public int? oUserProfileLevel_Logged { get; set; }
        public string oUserProfileScope_Logged { get; set; }

        public UserProfile oUserProfile_Logged { get; set; }
        public string oUserRoleLevel_Logged { get; set; }
        public string arrUserRoleCodes_Logged { get; set; }
        public string arrUserPermCodes_Logged { get; set; }

        public MSTRAppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public CLNTModels.AppGlobalOwner oAppGlobalOwn_Logged_CLNT { get; set; }
        public MSTRChurchBody oChurchBody_Logged { get; set; }
        public CLNTModels.ChurchBody oChurchBody_Logged_CLNT { get; set; }
        public RhemaCMS.Models.CLNTModels.ChurchMember oCurrLoggedMember { get; set; }
        public UserProfile oChurchAdminProfile { get; set; }
        public int setIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; }


        //
        public List<UserProfileRoleModel> lsUserProfileRoleModels { get; set; }
        public List<UserProfileRole> lsUserProfileRoles { get; set; }
        public UserProfileRole oUserProfileRole { get; set; }
        public UserProfile oUserProfile { get; set; }
   
        //
         
        public List<UserRole> lsUserRoles { get; set; }
        public List<UserRole> lsUserRolesAll { get; set; } 

        public List<UserRolePermission> lsUserRolePermissions { get; set; }
        public List<UserPermission> lsUserPermissions { get; set; }
        public List<UserPermission> lsUserPermissionsAll { get; set; }
        public List<UserSessionPerm> lsUserSessionPerms { get; set; }  // mapping or user role perm and user group perm

        public List<UserAuditTrail> lsUserAuditTrails { get; set; }
        public List<UserSessionPerm> lsUserSessionPermList { get; set; }

        //  public List<UserProfileRoleModel> lsUserProfileRoleModels { get; set; }

        // public UserProfileRole oUserProfileRole { get; set; }
        //public UserProfile oUserProfile { get; set; }
        // public UserRole oUserRole { get; set; }
        //
        // public UserProfile oUserProfile { get; set; }         


        // public List<UserGroup> lsUserGroups { get; set; }
        //  public List<UserRole> lsUserRoles { get; set; }
        // public List<UserPermission> lsUserPermissions { get; set; }
        //        
        public List<string> arrAssignedModCodes { get; set; }  //  module(s)
        public List<string> arrAssignedRoleCodes { get; set; }  //  collection of user role (s) 
        public List<string> arrAssignedRoleNames { get; set; }  //  collection of user role (s) 
                                                                // public List<string> arrAssignedGroupsDesc { get; set; }  // collection of user group (s)
        public List<string> arrAssignedGroupNames { get; set; }  // collection of user group (s)
        public List<string> arrAssignedPermCodes { get; set; }   // collection of user group (s)


        [StringLength(1)]
        public string profileScope { get; set; }
        [StringLength(1)]
        public string subScope { get; set; }

        //    

        public List<SelectListItem> lkpUserRoles { set; get; }  
        public List<SelectListItem> lkpUserPermissions { set; get; }
        public string strUserRoleName { get; set; } 
        public string strUserPermission { get; set; }
    }


}
