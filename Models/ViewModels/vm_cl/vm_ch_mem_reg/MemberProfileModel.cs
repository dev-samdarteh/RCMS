//using Microsoft.AspNetCore.Mvc.Rendering;
//using RhemaCMS.Models.CLNTModels;
//using System;
//using System.Collections.Generic;

//namespace RhemaCMS.Models.ViewModels.vm_cl
//{
//    public abstract class MemberProfileModel
//    {
//        public MemberProfileModel()  { }

//        public string strAppName { get; set; }
//        public string strAppNameMod { get; set; }
//        public string strAppCurrUser { get; set; }
//        public string strCurrTask { get; set; }
//        // 

//        public int? oAppGloOwnId { get; set; }
//        public int? oChurchBodyId { get; set; }
//        public AppGlobalOwner oAppGlobalOwn { get; set; }
//        public ChurchBody oChurchBody { get; set; }  // grace
//                                                     //public ChurchLevel oChurchLevel { get; set; }

//        //
//        public int? oAppGloOwnId_Logged { get; set; }
//        public int? oChurchBodyId_Logged { get; set; }
//        public int? oCurrMemberId_Logged { get; set; }
//        // public int? oCurrUserId_Logged { get; set; }
//        public int? oMemberId_Logged { get; set; }
//        public int? oUserId_Logged { get; set; }
//        public string oUserRole_Logged { get; set; }

//        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
//        public ChurchBody oChurchBody_Logged { get; set; }
//        public ChurchMember oCurrLoggedMember { get; set; }
//        public MSTRModels.UserProfile oChurchAdminProfile { get; set; }
//        public int setIndex { get; set; }
//        public int subSetIndex { get; set; }
//        public int pageIndex { get; set; }
//        public int filterIndex { get; set; }
//    }

//    public class MemberBioModel : MemberProfileModel   //for CTRY and CURR
//    {
//        public MemberBioModel() { }
//        // 
//        public List<MemberBioModel> lsCountryModels { get; set; }
//        public List<ChurchMember> lsChurchMembers { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        public string strMemberDisplayName { get; set; }  // DARTEH, Samuel Jr (Rev. Dr.)
//        public string strMemberFullName { get; set; }   // with Title... Rev. Dr. Samuel Darteh Jr.
//        public string strNationality { get; set; }
//        public string strMotherTongue { get; set; }
//        public string strIdType { get; set; }
//        public string strHometownRegion { get; set; }
//        public string strMaritalStatus { get; set; }
//        public string strMarriageType { get; set; }
//        public string strGender { get; set; }
//        public string strMemberClass { get; set; }  // New Convert [N], Affiliated [A], Congregant [C], Guest/Visitor [G/V] ... Keep Guest records in the attendance details instead
//        public string strCurrMemberType { get; set; }
//        public string strDateOfBirth { get; set; }
//        public string strContactInfo { get; set; }


//        public List<SelectListItem> lkpCountries { set; get; }
//        public List<SelectListItem> lkpCountryRegions { set; get; }
//        public List<SelectListItem> lkpLanguages { set; get; }
//        public List<SelectListItem> lkpMemberTypes { set; get; }
//        public List<SelectListItem> lkpMarriageTypes { set; get; }
//        public List<SelectListItem> lkpMaritalStatuses { set; get; }
//        public List<SelectListItem> lkpStatuses { set; get; }
//        public List<SelectListItem> lkpIdTypes { set; get; }
//        public List<SelectListItem> lkpMemberClasses { set; get; }

//    }

//    public class MemberLanguageSpokenModel : MemberProfileModel    
//    {
//        public MemberLanguageSpokenModel() { }
//        // 
//        public List<MemberLanguageSpokenModel> lsMemberLanguageSpokenModels { get; set; }
//        public List<MemberLanguageSpoken> lsMemberLanguagesSpoken { get; set; }
//        public MemberLanguageSpoken oMemberLanguageSpoken { get; set; } 
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public LanguageSpoken PrimaryLanguage { get; set; }
//        public string strLanguageSpoken { get; set; }

//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }

//    public class MemberLocationAddressModel : MemberProfileModel    
//    {
//        public MemberLocationAddressModel() { }
//        // 
//        public List<MemberLocationAddressModel> lsMemberLocationAddressModels { get; set; }
//        public List<ContactInfo> lsMemberLocationAddresses { get; set; }
//        public ContactInfo oMemberLocationAddress { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strLanguageSpoken { get; set; }
//        //
//        public List<SelectListItem> lkpCountries { set; get; }

//    }

//    public class MemberEducHistoryModel : MemberProfileModel
//    {
//        public MemberEducHistoryModel() { }
//        // 
//        public List<MemberEducHistoryModel> lsMemberEducHistoryModels { get; set; }
//        public List<MemberEducHistory> lsMemberEducHistories { get; set; }
//        public MemberEducHistory oMemberEducHistory { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strEducHistory { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }

//    public class MemberRelationModel : MemberProfileModel
//    {
//        public MemberRelationModel() { }
//        // 
//        public List<MemberRelationModel> lsMemberRelationModels { get; set; }
//        public List<MemberRelation> lsMemberRelations { get; set; }
//        public MemberRelation oMemberRelation { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strRelation { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }

//    public class MemberContactModel : MemberProfileModel
//    {
//        public MemberContactModel() { }
//        // 
//        public List<MemberContactModel> lsMemberContactModels { get; set; }
//        public List<MemberContact> lsMemberContacts { get; set; }
//        public MemberContact oMemberContact { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strContact { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//    public class MemberProfessionBrandModel : MemberProfileModel
//    {
//        public MemberProfessionBrandModel() { }
//        // 
//        public List<MemberProfessionBrandModel> lsMemberProfessionBrandModels { get; set; }
//        public List<MemberProfessionBrand> lsMemberProfessionBrands { get; set; }
//        public MemberProfessionBrand oMemberProfessionBrand { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strProfessionBrand { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }



//    public class MemberWorkExperienceModel : MemberProfileModel
//    {
//        public MemberWorkExperienceModel() { }
//        // 
//        public List<MemberWorkExperienceModel> lsMemberWorkExperienceModels { get; set; }
//        public List<MemberWorkExperience> lsMemberWorkExperiences { get; set; }
//        public MemberWorkExperience oMemberWorkExperience { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//    /// CHURCH - LIFE related models

//    public class MemberTypeModel : MemberProfileModel  // Member Movements & Migration
//    {
//        public MemberTypeModel() { }
//        // 
//        public List<MemberTypeModel> lsMemberTypeModels { get; set; }
//        public List<MemberType> lsMemberTypes { get; set; }
//        public MemberType oMemberType { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }

     
//    public class MemberStatusModel : MemberProfileModel    // Member Movements & Migration
//    {
//        public MemberStatusModel() { }
//        // 
//        public List<MemberStatusModel> lsMemberStatusModels { get; set; }
//        public List<MemberStatus> lsMemberStatuses { get; set; }
//        public MemberStatus oMemberStatus { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strFromCongLocation { get; set; }  // Grace congregation, Taifa  [combine 2 fields] ... globally within the church will give members All-time movements/ migration
//        public string strToCongLocation { get; set; }   // Tema Comm 2 congregation, Tema
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }
     
//    public class MemberRankModel : MemberProfileModel    // Member Movements & Migration
//    {
//        public MemberRankModel() { }
//        // 
//        public List<MemberRankModel> lsMemberRankModels { get; set; }
//        public List<MemberRank> lsMemberRanks { get; set; }
//        public MemberRank oMemberRank { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//    public class MemberChurchlifeModel : MemberProfileModel
//    {
//        public MemberChurchlifeModel() { }
//        // 
//        public List<MemberChurchlifeModel> lsMemberChurchlifeModels { get; set; }
//        public List<MemberChurchlife> lsMemberChurchlife { get; set; }  //skip pluralize
//        public MemberChurchlife oMemberChurchlife { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//    public class MemberChurchlifeActivityModel : MemberProfileModel
//    {
//        public MemberChurchlifeActivityModel() { }
//        // 
//        public List<MemberChurchlifeActivityModel> lsMemberChurchlifeActivityModels { get; set; }
//        public List<MemberChurchlifeActivity> lsMemberChurchlifeActivities { get; set; }
//        public MemberChurchlifeActivity oMemberChurchlifeActivity { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; } 

//    }


//    public class MemberChurchUnitModel : MemberProfileModel  // Church Grouping ... Applies to only groupable units like... CO, CG, CE, SC, DP
//    {
//        public MemberChurchUnitModel() { }
//        // 
//        public List<MemberChurchUnitModel> lsMemberChurchUnitModels { get; set; }
//        public List<MemberChurchUnit> lsMemberChurchUnits { get; set; }
//        public MemberChurchUnit oMemberChurchUnit { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }



//    public class MemberChurchRoleModel : MemberProfileModel   // Church roles, positions, designations
//    {
//        public MemberChurchRoleModel() { }
//        // 
//        public List<MemberChurchRoleModel> lsMemberChurchRoleModels { get; set; }
//        public List<MemberChurchRole> lsMemberChurchRoles { get; set; }
//        public MemberChurchRole oMemberChurchRole { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//    public class MemberRegistrationModel : MemberProfileModel  
//    {
//        public MemberRegistrationModel() { }
//        // 
//        public List<MemberRegistrationModel> lsMemberRegistrationModels { get; set; }
//        public List<MemberRegistration> lsMemberRegistrations { get; set; }
//        public MemberRegistration oMemberRegistration { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//    public class MemberAttendanceModel : MemberProfileModel
//    {
//        public MemberAttendanceModel() { }
//        // 
//        public List<MemberAttendanceModel> lsMemberAttendanceModels { get; set; }
//        public List<ChurchAttendAttendee> lsChurchAttendAttendees { get; set; }
//        public ChurchAttendAttendee oChurchAttendAttendee { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        public DateTime dtFirstVisitDate { get; set; }
//        public string strFirstVisitDate { get; set; }
//        public int? FirstChurchEventId { get; set; }
//        public string strFirstEventDesc { get; set; }
//        public DateTime dtLastVisitDate { get; set; }
//        public string strLastVisitDate { get; set; }
//        public string strLastEventDesc { get; set; }
//        public int? LastChurchEventId { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }

//    public class MemberTransferModel : MemberProfileModel
//    {
//        public MemberTransferModel() { }
//        // 
//        public List<MemberTransferModel> lsMemberTransferModels { get; set; }
//        public List<ChurchTransfer> lsChurchTransfers { get; set; }
//        public ChurchTransfer oChurchTransfer { get; set; }
//        public ChurchMember oChurchMember { get; set; }
//        //
//        public string strWorkExperience { get; set; }
//        //
//        public List<SelectListItem> lkpLanguages { set; get; }

//    }


//}
