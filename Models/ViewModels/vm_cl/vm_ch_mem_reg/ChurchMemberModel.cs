using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using System;
using System.Collections.Generic;

namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public abstract class AbsChurchMemberModel
    {
        public AbsChurchMemberModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public int? oChurchMemberId { get; set; }
        public string strGlobalMemberCode { get; set; }

        public AppGlobalOwner oAppGlobalOwn { get; set; }
        public ChurchBody oChurchBody { get; set; }  // grace ... //public ChurchLevel oChurchLevel { get; set; }
        public ChurchMember oChurchMember { get; set; }  // BioModel

        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oAppGloOwnId_Logged_MSTR { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oChurchBodyId_Logged_MSTR { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        // public int? oCurrUserId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }

        public bool isUserRoleAdmin_Logged { get; set; }
        


        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        public ChurchMember oCurrLoggedMember { get; set; }
        public MSTRModels.UserProfile oChurchAdminProfile { get; set; }
        public int setIndex { get; set; }
        public int taskIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; }
        public int filterIndex { get; set; }

        
        public string strParentCBLevel { get; set; }

        ///
        /// list all members of the church/congregation: Name [Sam Darteh], Sex [M], Location [Taifa, Accra], Member Type [Church Leader], Status [Regular], Joined [22-Aug-2000], Departed [***]
        
        public string strChurchBody { get; set; }
        public string strChurchBodyDetail { get; set; }
        public string strMemDisplayName { get; set; }
        public string strMemFullName { get; set; }
        public string strMemFullNameExtnd { get; set; }

        public string strCongLocation { get; set; }  // Grace, Taifa-North
        public string strLeadRole { get; set; }
        public string strPrimaryGroup { get; set; }
        public string strMemLongevity_Yrs { get; set; }
        
        /// 
        ///
        //public List<MemberLanguageSpokenModel> lsMemberLanguageSpokenModels { get; set; }
        //public List<MemberLanguageSpoken> lsMemberLanguagesSpoken { get; set; }
        //public List<MemContactInfoModel> lsMemContactInfoModels { get; set; }
        //public List<ContactInfo> lsMemContactInfoes { get; set; }
        //public List<MemberEducationModel> lsMemberEducationModels { get; set; }
        //public List<MemberEducation> lsMemberEducHistories { get; set; }
        //public List<MemberRelationModel> lsMemberRelationModels { get; set; }
        //public List<MemberRelation> lsMemberRelations { get; set; }
        //public List<MemberContactModel> lsMemberContactModels { get; set; }
        //public List<MemberContact> lsMemberContacts { get; set; }
        //public List<MemberProfessionBrandModel> lsMemberProfessionBrandModels { get; set; }
        //public List<MemberProfessionBrand> lsMemberProfessionBrands { get; set; }
        //public List<MemberWorkExperienceModel> lsMemberWorkExperienceModels { get; set; }
        //public List<MemberWorkExperience> lsMemberWorkExperiences { get; set; }
    }


    public class ChurchMemberModel
    {
        public ChurchMemberModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public AppGlobalOwner oAppGlobalOwn { get; set; }
        public ChurchBody oChurchBody { get; set; }  // grace ... //public ChurchLevel oChurchLevel { get; set; }
        public ChurchMember oChurchMember { get; set; }  // BioModel

        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oAppGloOwnId_Logged_MSTR { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oChurchBodyId_Logged_MSTR { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        // public int? oCurrUserId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }

        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        public ChurchMember oCurrLoggedMember { get; set; }
        public MSTRModels.UserProfile oChurchAdminProfile { get; set; }
        public int setIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; }
        public int filterIndex { get; set; }

        ///
        /// list all members of the church/congregation: Name [Sam Darteh], Sex [M], Location [Taifa, Accra], Member Type [Church Leader], Status [Regular], Joined [22-Aug-2000], Departed [***]
        public string strChurchBody { get; set; }
        public int? oChurchMemberId { get; set; }
        public string strMemberFullName { get; set; }
        public string strNameSortBy { get; set; }
        public string strLocation { get; set; }
        public string strMemberType { get; set; }
        public string strMemberStatus { get; set; }
        public string strDesignation { get; set; }  // current role, unit
        public string strDateJoined { get; set; }  
        public string strDateDeparted { get; set; }  
        public string strGender { get; set; }  
        public string strXStatus { get; set; }  
        public string strNationality { get; set; }  

        /// 
        ///
        //public List<MemberLanguageSpokenModel> lsMemberLanguageSpokenModels { get; set; }
        //public List<MemberLanguageSpoken> lsMemberLanguagesSpoken { get; set; }
        //public List<MemContactInfoModel> lsMemContactInfoModels { get; set; }
        //public List<ContactInfo> lsMemContactInfoes { get; set; }
        //public List<MemberEducationModel> lsMemberEducationModels { get; set; }
        //public List<MemberEducation> lsMemberEducHistories { get; set; }
        //public List<MemberRelationModel> lsMemberRelationModels { get; set; }
        //public List<MemberRelation> lsMemberRelations { get; set; }
        //public List<MemberContactModel> lsMemberContactModels { get; set; }
        //public List<MemberContact> lsMemberContacts { get; set; }
        //public List<MemberProfessionBrandModel> lsMemberProfessionBrandModels { get; set; }
        //public List<MemberProfessionBrand> lsMemberProfessionBrands { get; set; }
        //public List<MemberWorkExperienceModel> lsMemberWorkExperienceModels { get; set; }
        //public List<MemberWorkExperience> lsMemberWorkExperiences { get; set; }
    }


    // list roll
    public class ChurchMemberSummaryModel
    {
        public ChurchMemberSummaryModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        public string strMdlHdr { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public AppGlobalOwner oAppGlobalOwn { get; set; }
        public ChurchBody oChurchBody { get; set; }  // grace ... //public ChurchLevel oChurchLevel { get; set; }
        public ChurchMember oChurchMember { get; set; }  // BioModel 
         
        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oAppGloOwnId_Logged_MSTR { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oChurchBodyId_Logged_MSTR { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        // public int? oCurrUserId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }
        public bool isUserRoleAdmin_Logged { get; set; }

        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        public ChurchMember oCurrLoggedMember { get; set; }
        public MSTRModels.UserProfile oChurchAdminProfile { get; set; }
        public int setIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; }
        public int filterIndex { get; set; }

        ///
        /// list all members of the church/congregation: Name [Sam Darteh], Sex [M], Location [Taifa, Accra], Member Type [Church Leader], Status [Regular], Joined [22-Aug-2000], Departed [***]
        public string strChurchBody { get; set; }
        public string strMemberFullName { get; set; }
        public string strMemDisplayName { get; set; }
        public string strLocation { get; set; }
        public string strPhone { get; set; }
        public string strMemType { get; set; }
        public string strMemTypeCode { get; set; }
        public string strMemRank { get; set; }
        public string strMemStatus { get; set; }
        public string strMemGender { get; set; }
        public string strMemUnit { get; set; }
        public string strMemRole { get; set; }
        public string strMemUnitRole { get; set; }
        public string strMemRoleStatus { get; set; } 

        public MemberChurchRole oMemberChurchRole { get; set; }
        public ContactInfo oMemContactInfo { get; set; }
        public MemberChurchlife oMemberChurchlife { get; set; }
        public MemberWorkExperience oMemberWorkExperience { get; set; }
        public MemberEducation oMemberEducation { get; set; }

        public string strAccountStatus { get; set; } 
        public string strDesignation { get; set; }  // current role, unit
        public string strDateJoined { get; set; }
        public string strDateDeparted { get; set; }
        public string strMemLongevity_Yrs { get; set; }
        public bool bl_MemberStatusAvail { get; set; }

        ///summary sheet
        public long numTotalRoll { get; set; }
        public long numTotalRoll_M { get; set; }
        public long numTotalRoll_F { get; set; }
        public long numTotalRoll_O { get; set; } 
        public long numTotalRoll_CHL { get; set; } 
        public long numTotalRoll_LDR { get; set; } 
        public long numTotalRoll_CLG { get; set; } 


        public List<ChurchMemberSummaryModel> lsChurchMemberSummaryModels { get; set; }
        public List<ChurchMember> lsChurchMembers { get; set; } 

        public List<SelectListItem> lkpPrebuitFilterOptions { set; get; }
    }


    // open member profile -- together

    public class MemberProfileComponentModel : AbsChurchMemberModel
    {
        public MemberProfileComponentModel() { }
        // 

        public List<MemberProfileComponentModel> lsMemberProfileComponentModels { get; set; } 
        public List<MemberPersonalProfileModel> lsMemberPersonalProfileModels { get; set; } 
        public List<MemberChurchlifeProfileModel> lsMemberChurchlifeProfileModels { get; set; }


        /// pers
        public MemberBioModel oMemberBioModel { get; set; }
        public MemberContactInfoModel oMemberContactInfoModel { get; set; }
      //  public List<MemberContactInfoModel> lsMemContactInfoModels { get; set; }
        public MemberLanguageSpokenModel oMemberLanguageSpokenModel { get; set; }  /// has the list     
        public MemberEducationModel oMemberEducationModel { get; set; }
        public MemberFamilyRelationModel oMemberRelationModel { get; set; }
        public MemberContactPersonModel oMemberContactModel { get; set; }
        public MemberProfessionBrandModel oMemberProfessionBrandModel { get; set; }
        public MemberWorkExperienceModel oMemberWorkExperienceModel { get; set; }
         

        // churchlife
        public MemberChurchMovementModel oMemberChurchMovementModel { get; set; } // lsMemType, lsRank, lsMemStat List<MemberTypeModel>  List<MemberStatusModel>  List<MemberRankModel> 
        public MemberChurchlifeAllModel oMemberChurchlifeAllModel { get; set; }  // has List<MemberChurchlifeEvent> List<MemberChurchlifeEventTask>
        //  public List<MemberChurchlifeModel> lsMemberChurchlifeActivityModels { get; set; } 
        public MemberChurchGroupingModel oMemberGroupingModel { get; set; }
        public MemberRoleDesigModel oMemberRoleDesigModel { get; set; }
        public MemberRegistrationModel oMemberRegistrationModel { get; set; }
        public MemberChurchAttendanceModel oMemberChurchAttendanceModel { get; set; }
        public MemberChurchTransferModel oMemberChurchTransferModel { get; set; }
        public MemberChurchPaymentModel oMemberChurchPaymentModel { get; set; }   // all payments to the church... tithes, donations, welfare contri etc. usually via online platforms or direct cash



        // public string strTaskDesc { get; set; }
        public string strMaritalStat { get; set; }
        public string strMemberAge { get; set; }
        public string strAgeDesc { get; set; }
        public string strMemberClass { get; set; }
        public string strBirthday { get; set; }
        public string strLangProfiency { get; set; }
        public string strPrimLang { get; set; }

        public string strFamRelaDesc { get; set; }   
        public string strMemSpouse { get; set; }
        public string strMemChildren { get; set; }
        public string strMemNextOfKin { get; set; }

        public string strEducLevelDesc { get; set; }   //pick current from Education history : MSc from Harvard 
        public string strProBrandDesc { get; set; }  // Accountant or Multiple: Lawyer, Accountant, Auditor, Administrator 
        public string strJobStatusDesc { get; set; }   //incl WorkPlace @Work History...  :: Chief Director, Ministry of Finance 

        public string strNativity { get; set; }
        public string strNationality { get; set; }
        public string strNationIDType { get; set; }
        public string strMotherTongue { get; set; }
        public string strHometownReg { get; set; }

        public string strJoined { get; set; }
        public string strLongevity { get; set; }
        public string strLongevityDesc { get; set; }
        public string strBirthdayTag { get; set; }
        public string strDeparted { get; set; }
        public string strMemRankAssigned { get; set; }
        public string strMemRankLongevity { get; set; }
        public string strMemRankLongevityDesc { get; set; }

        public string strMemStatAssigned { get; set; }
        public string strMemStatLongevity { get; set; }
        public string strMemStatLongevityDesc { get; set; }

        //public string strMemRankAssigned { get; set; }
        //public string strMemRankLongevity { get; set; }
        //public string strMemRankLongevityDesc { get; set; }

        ////public string strMemRolesDesc { get; set; }
        //public string strMemUnitRole { get; set; }
        //public string strMemRole { get; set; }
        //public string strMemUnit { get; set; }

        public string strMemGroupsDesc { get; set; }
        public string strMemRolesDesc { get; set; }
        public string strMemChActvDesc { get; set; }

        //public string strMemberRankDesc { get; set; }

        public string strBaptised { get; set; }
        public string strConfirmed { get; set; }
        public string strCommunicant { get; set; }
        public string strCurrPastDeceased { get; set; }

        public string strMemberType { get; set; }
        public string strMemTypeAssigned { get; set; }
        public string strMemTypeLongevity { get; set; }
        public string strMemTypeLongevityDesc { get; set; }

        public string strEducLevel { get; set; }

    }


    public class MemberPersonalProfileModel : AbsChurchMemberModel  
    {
        public MemberPersonalProfileModel() { }
        // 
        public List<MemberPersonalProfileModel> lsMemberPersonalProfileModels { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //public string strCurrentMemberType { get; set; }
        //public ChurchMemType oCurrentMemberTypeId { get; set; }
        //public string strEducationLevel { get; set; }
        //public InstitutionType oEducationLevel { get; set; }  // highest... Basic ... Tertairy /PostGraduate

        public MemberBioModel oMemberBioModel { get; set; }        
        public List<MemberLanguageSpokenModel> lsMemberLanguageSpokenModels { get; set; }
        public List<MemberContactInfoModel> lsMemContactInfoModels { get; set; }
        public List<MemberEducationModel> lsMemberEducationModels { get; set; }
        public List<MemberFamilyRelationModel> lsMemberRelationModels { get; set; }
        public List<MemberContactPersonModel> lsMemberContactModels { get; set; }
        public List<MemberProfessionBrandModel> lsMemberProfessionBrandModels { get; set; }
        public List<MemberWorkExperienceModel> lsMemberWorkExperienceModels { get; set; }

    }

    public class MemberChurchlifeProfileModel : AbsChurchMemberModel
    {
        public MemberChurchlifeProfileModel() { }
        // 
        public List<MemberChurchlifeProfileModel> lsMemberChurchlifeProfileModels { get; set; }
        // public ChurchMember oChurchMember { get; set; } 
        public List<MemberChurchMovementModel> lsMemberMovementModels { get; set; } // List<MemberTypeModel>  List<MemberStatusModel>  List<MemberRankModel> 
        public List<MemberChurchlifeModel> lsMemberChurchlifeModels { get; set; }  // has List<MemberChurchlifeEvent> List<MemberChurchlifeEventTask>
        //  public List<MemberChurchlifeModel> lsMemberChurchlifeActivityModels { get; set; } 
        public List<MemberChurchGroupingModel> lsMemberGroupingModels { get; set; } 
        public List<MemberRoleDesigModel> lsMemberRoleDesigModels { get; set; } 
        public List<MemberRegistrationModel> lsMemberRegistrationModels { get; set; } 
        public List<MemberChurchAttendanceModel> lsMemberAttendanceModels { get; set; } 
        public List<MemberChurchTransferModel> lsMemberTransferModels { get; set; } 
        public List<MemberChurchPaymentModel> lsMemberChurchPaymentModels { get; set; } 

    }



    /// CHURCH - PERS related models  -- inidvidual edits/saves/delete
    public class MemberBioModel : AbsChurchMemberModel   //for CTRY and CURR
    {
        public MemberBioModel() { }
        // 
        //public List<MemberBioModel> lsMemberBioModels { get; set; }
        //public List<ChurchMember> lsChurchMembers { get; set; }
        //  // public ChurchMember oChurchMember { get; set; }

        public ChurchMember oChurchMemberBio { get; set; }  // BioModel
        public ContactInfo oMemContactInfo { get; set; }  // BioModel
        public string strEducationLevel { get; set; }
        public InstitutionType oEducationLevel { get; set; }  // highest... Basic ... Tertairy /PostGraduate
        public string strCurrentMemberType { get; set; }
        public ChurchMemType oCurrentMemberTypeId { get; set; }

       // public string strDisplayName    { get; set; }  // DARTEH, Samuel Jr (Rev. Dr.)
        //public string strMemFullName { get; set; }   // with Title... Rev. Dr. Samuel Darteh Jr.
        public string strNationality    { get; set; }
        public string strMotherTongue   { get; set; }
        public string strIdType         { get; set; }
        public string strHometown  { get; set; }
        public string strHometownRegion { get; set; }
        public string strMemType { get; set; }
        public string strMemRank { get; set; }
        public string strMemStatus { get; set; }
        public string strMemGeneralStatus { get; set; }
       // public string strStatus { get; set; }
        public string strAccountStatus  { get; set; }  // thus member account profile -- active, pending, blocked, deactive...  different from membership status -- regua;lr, distant, past etc
        public string strMaritalStatus  { get; set; }
        public string strMarriageType   { get; set; }
        public string strGender         { get; set; }
        public string strMemberScope { get; set; }  // Internal, External New Convert [N], Affiliated [A], Congregant [C], Guest/Visitor [G/V] ... Keep Guest records in the attendance details instead
      //  public string strMemberType { get; set; }
      //  public string strMemberStat { get; set; }
        public string strDateOfBirth    { get; set; }
        public string strBirthday    { get; set; }
       // public string strContactInfo    { get; set; }
        public string strPhone    { get; set; }
        public string strLocation    { get; set; }
        public string strMemberAge { get; set; }
        public bool bl_MemberStatusAvail { get; set; }
        public bool bl_AUT_GN { get; set; }
        public bool isCBOwned { get; set; }
        public bool isCBCurr { get; set; }

        public string strMemUnitRole { get; set; }
        public string strMemRole { get; set; }
        public string strMemUnit { get; set; }
        public string strEnrollModeCode { get; set; }
        public string strEnrollReason { get; set; }
        public DateTime? dtEnrollDate { get; set; }
        public string strMemTypeCode { get; set; }
        public string _strMemTypeCodeBck { get; set; }
        public int? numMemRankId { get; set; }
        public int? numMemStatusId { get; set; }
        public bool isMemberBirthdayToday { get; set; }
        public int numDaysToNextBirthday { get; set; }
        public string strDaysToNextBirthday { get; set; }

        public string strPhotoUrl { get; set; }
        public string strNameSortBy { get; set; }

        public List<SelectListItem> lkpMemberTypes { set; get; }  
        public List<SelectListItem> lkpStatuses { set; get; } 
        public List<SelectListItem> lkpEnrollModes { set; get; }


       // [Display(Name = "Church logo")]
        public IFormFile UserPhotoFile { get; set; }

        //Lookups Lists... 
        public List<SelectListItem> lkpChurchMembers { set; get; }
        public List<SelectListItem> lkpChurchMembers_Local { set; get; }
        public List<SelectListItem> lkpChurchMembers_Denom { set; get; }
        public List<SelectListItem> lkpChurchAssociates { set; get; }
        public List<SelectListItem> lkpActivStatus { set; get; }
        public List<SelectListItem> lkpGenderBaseTypes { set; get; }
        public List<SelectListItem> lkpGenderTypes { set; get; }
        public List<SelectListItem> lkpMaritalStatuses { set; get; }
      //  public List<SelectListItem> lkpChurchMemStatuses { set; get; }
      //  public List<SelectListItem> lkpChuMemTypes { set; get; }
      //  public List<SelectListItem> lkpChuMemRanks { set; get; }
       // public List<SelectListItem> lkpChuPositions { set; get; }
        public List<SelectListItem> lkpMarriageTypes { set; get; }
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpCountryRegions { get; set; }
        public List<SelectListItem> lkpEduLevels { set; get; }
        public List<SelectListItem> lkpLanguages { set; get; }
       // public List<SelectListItem> lkpChurchServices { set; get; }
        public List<SelectListItem> lkpPersonIdTypes { set; get; }
        public List<SelectListItem> lkpPersTitles { set; get; }
       // public List<SelectListItem> lkpCertificates { set; get; }
       // public List<SelectListItem> lkpInstitutionTypes { set; get; }
       // public List<SelectListItem> lkpChurchFellowOptions { set; get; }
        public List<SelectListItem> lkpContactInfoList { set; get; }

        // public List<SelectListItem> lkpRelationshipTypes { set; get; }
        // public List<SelectListItem> lkpRelationTypes { set; get; }
        // public List<SelectListItem> lkpMemRelStatuses { set; get; }
       // public List<SelectListItem> lkpAffiliateStatuses { set; get; }
        // public List<SelectListItem> lkpChuRoles { set; get; }
        // public List<SelectListItem> lkpChuSectors { set; get; }
        // public List<SelectListItem> lkpSectorScopes { set; get; }
        // public List<SelectListItem> lkpChuPeriods { set; get; }
        // public List<SelectListItem> lkpChuLifeActivities { set; get; }
        // public List<SelectListItem> lkpClergyList { set; get; }
        public List<SelectListItem> lkpLangProfLevels { set; get; }
       // public List<SelectListItem> lkpChurchUnits { set; get; }
       // public List<SelectListItem> lkpChurchUnitTypes { set; get; }
       // public List<SelectListItem> lkpChurchGroupingTypes { set; get; }
       // public List<SelectListItem> lkpFaithTypes { set; get; }

        public List<SelectListItem> lkpChurchMemTypes { set; get; }
        public List<SelectListItem> lkpChurchRanks { set; get; }
        public List<SelectListItem> lkpChurchMemStatuses { set; get; }

    }

    public class MemberContactInfoModel : AbsChurchMemberModel
    {
        public MemberContactInfoModel() { }
        // 
        //public List<MemberContactInfoModel> lsMemberContactInfoModels { get; set; }
        // public List<ContactInfo> lsMemContactInfo { get; set; }

        public ContactInfo oMemContactInfo { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        public string strLocation { get; set; }
        public string strRegion { get; set; }
        public string strCountry { get; set; }
        public string strContactInfo { get; set; }
        public string strHolderName { get; set; }  // if church mem pick... else extholdername
        //
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpCountryRegions { set; get; }

    }

    public class MemberLanguageSpokenModel : AbsChurchMemberModel
    {
        public MemberLanguageSpokenModel() { }
        // 
        public List<MemberLanguageSpokenModel> lsMemberLanguageSpokenModels { get; set; }
        public List<MemberLanguageSpoken> lsMemberLanguagesSpoken { get; set; }
        public MemberLanguageSpoken oMemberLanguageSpoken { get; set; }
       // // public ChurchMember oChurchMember { get; set; }
        //
        public LanguageSpoken PrimaryLanguage { get; set; }
        public string strLanguageSpoken { get; set; }
        public string strLanguageCountry { get; set; }
        public string strProficiencyLevel { get; set; }
        public bool bl_IsPrimaryLangSet { get; set; }
        public string strIsPrimaryLangMess { get; set; }   // >> Primary language set: English [change] // Not set yet! [choose now]
        public string arrLanguage_Country { get; set; }   // [id--lang--country] 1--English--UK, 2--Akan--Ghana, 4--Ga--Ghana, 7--French-- 

        //
        public List<SelectListItem> lkpLanguages { set; get; }
        public List<SelectListItem> lkpProficiencyLevels { set; get; }

    }

    public class MemberContactPersonModel : AbsChurchMemberModel
    {
        public MemberContactPersonModel() { }
        // 
        public List<MemberContactPersonModel> lsMemberContactModels { get; set; }
        public List<MemberContact> lsMemberContacts { get; set; }
        public MemberContact oMemberContact { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        // 
        public ContactInfo oMemContactInfo { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        public string strContactName { get; set; }   // link back to relation -- father/son, Uncle/niece etc
        public string strRelationCB_Scope { get; set; }   // link back to relation -- father/son, Uncle/niece etc
        public string strRelationship { get; set; }
        public string strScopeDesc { get; set; }  // Share congregation, Share Denomination, Share faith, Varied
        public string strLocation { get; set; }  // 
        public string strStatus { get; set; }  // 
        public string strContactDetails { get; set; }  // link to ContactInfo []

        public string strFaithAffiliate { get; set; }
        public string strFaithDenom { get; set; }
        public string strRelationCategory { get; set; }  //
        public string strPhotoUrl_MCP { get; set; }
        public string strContactChurchBody { get; set; } 

        public IFormFile PhotoFile_ExtCon { get; set; }
        ///
        public List<SelectListItem> lkpRelationshipTypes { set; get; }  
        public List<SelectListItem> lkpRelationScopes { set; get; } 
        public List<SelectListItem> lkpStatuses { set; get; }
        public List<SelectListItem> lkpChurchMembers_Local { set; get; }   // the Browse member utility
        public List<SelectListItem> lkpChurchMembers { set; get; }
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpCountryRegions { set; get; }
        public List<SelectListItem> lkpFaithCategories { set; get; }
    }   

    public class MemberFamilyRelationModel : AbsChurchMemberModel
    {
        public MemberFamilyRelationModel() { }
        // 
        public List<MemberFamilyRelationModel> lsMemberFamilyRelationModels { get; set; }
        public List<MemberRelation> lsMemberRelations { get; set; }
        public MemberRelation oMemberRelation { get; set; }
         public ContactInfo oRelationContactInfo { get; set; }
        //
        public int? numRelationCodePrev { get; set; }
        public string strRelationName { get; set; }   // link back to relation -- father/son, Uncle/niece etc
        public string strRelationCB_Scope { get; set; }   // link back to relation -- father/son, Uncle/niece etc
        public string strRelationship { get; set; }
        public string strScopeDesc { get; set; }  // Share congregation, Share Denomination, Share faith, Varied
        public string strLocation{ get; set; }  // 
        public string strRelationStatus { get; set; }  // 
        public string strRelationCategory { get; set; }  // 
        public string strContactDetails { get; set; }  // link to ContactInfo []
        

        public string strPhotoUrl_MFR { get; set; }
        public string strRelationChurchBody { get; set; }
        public string strFaithAffiliate { get; set; }
        public string strFaithDenom { get; set; }

        public IFormFile UserPhotoFile_Ext { get; set; }
        //
      //  public List<SelectListItem> lkpRelationCategories { set; get; }
       public List<SelectListItem> lkpRelationScopes { set; get; }
       public List<SelectListItem> lkpRelationshipTypes { set; get; }
       public List<SelectListItem> lkpRelationStatuses { set; get; }
       public List<SelectListItem> lkpChurchMembers_Local { set; get; }   // the Browse member utility
       public List<SelectListItem> lkpChurchMembers { set; get; }
       public List<SelectListItem> lkpCountries { set; get; }
       public List<SelectListItem> lkpCountryRegions { set; get; }
       public List<SelectListItem> lkpFaithCategories { set; get; }

    }

    public class MemberEducationModel : AbsChurchMemberModel
    {
        public MemberEducationModel() { }
        // 
        public List<MemberEducationModel> lsMemberEducationModels { get; set; }
        public List<MemberEducation> lsMemberEducation{ get; set; }
        public MemberEducation oMemberEducation { get; set; }
        public AppUtilityNVP oInstitutionType { get; set; }  // InstitutionType
        public AppUtilityNVP oCertificateType { get; set; }  // CertificateType

        public string strInstitutionType { get; set; } 
        public string strCountry { get; set; } 
        public string strInstitutionType_MaxEducLevel { get; set; } 
        public double numEducLevelIndex { get; set; } 
        public string strCertificateType { get; set; }
        public string strFromDate { get; set; } 
        public string strToDate { get; set; } 
        public string strDateDesc { get; set; } 
        
        public string strEducation { get; set; }   // Completed, On-going, Incomplete, Graduated 2 yrs ago
        public string strStatus { get; set; }   // Completed, On-going, Incomplete, Graduated 2 yrs ago

        public IFormFile CertPhotoFile { get; set; }

        public List<SelectListItem> lkpCountries { set; get; }
       public List<SelectListItem> lkpCertificateTypes { set; get; }
       public List<SelectListItem> lkpInstitutionTypes { set; get; }

    }

    public class MemberProfessionBrandModel : AbsChurchMemberModel
    {
        public MemberProfessionBrandModel() { }
        // 
        public List<MemberProfessionBrandModel> lsMemberProfessionBrandModels { get; set; }
        public List<MemberProfessionBrand> lsMemberProfessionBrands { get; set; }
        public MemberProfessionBrand oMemberProfessionBrand { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        public string strProfessionBrand { get; set; }

        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strDateDesc { get; set; }
        public string strStatus { get; set; }
        //
        // public List<SelectListItem> lkpLanguages { set; get; }

    }

    public class MemberWorkExperienceModel : AbsChurchMemberModel
    {
        public MemberWorkExperienceModel() { }
        // 
        public List<MemberWorkExperienceModel> lsMemberWorkExperienceModels { get; set; }
        public List<MemberWorkExperience> lsMemberWorkExperiences { get; set; }
        public MemberWorkExperience oMemberWorkExperience { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        public string strWorkExperience { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strDateDesc { get; set; }
        public string strLocationCountry { get; set; }
        public string strStatus { get; set; }
        //
       public List<SelectListItem> lkpCountries { set; get; }

    }


    /// CHURCH - LIFE related models
    public class MemberChurchMovementModel : AbsChurchMemberModel  // Member Movements & Migration
    {
        public MemberChurchMovementModel() { }
        // 
        public List<MemberChurchMovementModel> lsMemberChurchMovementModels { get; set; }
        ///
        
       // public List<MemberRankModel> lsMemberRankModels { get; set; }
        public MemberRankModel oMemberRankModel { get; set; }
        public List<MemberRank> lsMemberRanks { get; set; }
        public MemberRank oMemberRank { get; set; }


        ///
        //public List<MemberStatusModel> lsMemberStatusModels { get; set; }
        public MemberStatusModel oMemberStatusModel { get; set; }
        public List<MemberStatus> lsMemberStatuses { get; set; }
        public MemberStatus oMemberStatus { get; set; }


        ///
       // public List<MemberTypeModel> lsMemberTypeModels { get; set; }
        public MemberTypeModel oMemberTypeModel { get; set; }
        public List<MemberType> lsMemberTypes { get; set; }
        public MemberType oMemberType { get; set; }


        //public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
        //public List<SelectListItem> lkpChurchMemStatuses { set; get; }
        //public List<SelectListItem> lkpChurchMemTypes { set; get; }
        //public List<SelectListItem> lkpChurchRanks { set; get; }
    }

    public class MemberRankModel : AbsChurchMemberModel
    {
        public MemberRankModel() { }
        // 
        public List<MemberRankModel> lsMemberRankModels { get; set; } 
        public List<MemberRank> lsMemberRank { get; set; }
        public MemberRank oMemberRank { get; set; }
          
        public string strChurchRank { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strDateDesc { get; set; }
        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strAvailStatus { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared

        public List<SelectListItem> lkpSharingStatuses_Any { set; get; } 
        public List<SelectListItem> lkpChurchRanks { set; get; }
    }

    public class MemberTypeModel : AbsChurchMemberModel
    {
        public MemberTypeModel() { }
        // 
        public List<MemberTypeModel> lsMemberTypeModels { get; set; }
        public List<MemberType> lsMemberType { get; set; }
        public MemberType oMemberType { get; set; }
         
        public string strChurchMemType { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strDateDesc { get; set; }
        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strAvailStatus { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared
        public string strSharingStatus { get; set; }  // Owned or Shared

        public List<SelectListItem> lkpSharingStatuses_Any { set; get; } 
        public List<SelectListItem> lkpChurchMemTypes { set; get; } 
    }

    public class MemberStatusModel : AbsChurchMemberModel
    {
        public MemberStatusModel() { }
        // 
        public List<MemberStatusModel> lsMemberStatusModels { get; set; }
        public List<MemberStatus> lsMemberStatus { get; set; }
        public MemberStatus oMemberStatus { get; set; }
         
        public string strChurchMemStatus { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strDateDesc { get; set; }
        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strAvailStatus { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared

        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
        public List<SelectListItem> lkpChurchMemStatuses { set; get; } 
    }


    public class MemberChurchlifeAllModel : AbsChurchMemberModel
    {
        public MemberChurchlifeAllModel() { }
        // 
        public List<MemberChurchlifeAllModel> lsMemberChurchlifeAllModels { get; set; }
        ///  
        //public List<MemberChurchlifeModel> lsMemberChurchlifeModels { get; set; }
        public MemberChurchlifeModel oMemberChurchlifeModel { get; set; }
        /// 
        //public List<MemberChurchlifeActivityModel> lsMemberChurchlifeActivityModels { get; set; }
        public MemberChurchlifeActivityModel oMemberChurchlifeActivityModel { get; set; }
        /// 
        //public List<MemberChurchlifeEventTaskModel> lsMemberChurchlifeEventTaskModels { get; set; }
        public MemberChurchlifeEventTaskModel oMemberChurchlifeEventTaskModel { get; set; }


        ///
        //public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
        //public List<SelectListItem> lkpChurchBodyServices { set; get; }
        //public List<SelectListItem> lkpChurchlifeActivities { set; get; }
        //public List<SelectListItem> lkpRequirementDefList { set; get; }
        //public List<SelectListItem> lkpMemberChurchlifeActivities { set; get; }

    }

    public class MemberChurchlifeModel : AbsChurchMemberModel
    {
        public MemberChurchlifeModel() { }
        // 
        // public List<MemberChurchlifeModel> lsMemberChurchlifeModels { get; set; }
        ///

        public List<MemberChurchlife> lsMemberChurchlife { get; set; }  //skip pluralize
        public MemberChurchlife oMemberChurchlife { get; set; } 
         
        public string strChurchBodyService { get; set; }
        public string strHealthCondition { get; set; }
        public string strJoinedDate { get; set; }
        public string strDepartedDate { get; set; }
        public string strDateDesc { get; set; }

        public string strEnrollMode { get; set; }
        public string strDepartMode { get; set; }
        public string strAvailStatus { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared

        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public IFormFile PhotoFile_CL { get; set; }

        ///
        public List<SelectListItem> lkpChurchBodyServices { set; get; }
        public List<SelectListItem> lkpEnrollModes { set; get; }
        public List<SelectListItem> lkpDepartModes { set; get; }
        public List<SelectListItem> lkpHealthConditionStatuses { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
    }

    public class MemberChurchlifeActivityModel : AbsChurchMemberModel
    {
        public MemberChurchlifeActivityModel() { }
        /// 
        public List<MemberChurchlifeActivityModel> lsMemberChurchlifeActivityModels { get; set; }        
        /// 
        public List<MemberChurchlifeActivity> lsMemberChurchlifeActivities { get; set; }
        public MemberChurchlifeActivity oMemberChurchlifeActivity { get; set; }

        ///
        public List<MemberChurchlifeEventTaskModel> lsMemberChurchlifeEventTaskModels { get; set; }
        public List<MemberChurchlifeEventTask> lsMemberChurchlifeEventTasks { get; set; }
        public MemberChurchlifeEventTask oMemberChurchlifeEventTask { get; set; }
        public MemberChurchlifeEventTaskModel oMemberChurchlifeEventTaskModel { get; set; }

        //         
        public string strChurchlifeActivity { get; set; }
        public string strActivityDate { get; set; }
        public string strVenue { get; set; }
        public string strVenueChurchBody { get; set; }
        public string strOfficiatedByChurchBody { get; set; }
        public string strOfficiatedByRole { get; set; }
        public string strEvent { get; set; }

        public string strOwnStatus { get; set; }  // Owned or Shared

        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public bool isOpenChurchlifeActivity { get; set; }

        public IFormFile PhotoFile_CLA { get; set; }
        ///
        public List<SelectListItem> lkpChurchlifeActivities { set; get; }
        public List<SelectListItem> lkpChurchEvents { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
        public List<SelectListItem> lkpPersonScopes { set; get; }
        public List<SelectListItem> lkpChurchMembers { set; get; }
        public List<SelectListItem> lkpChurchRoles { set; get; }
    }

    public class MemberChurchlifeEventTaskModel : AbsChurchMemberModel//: SetupEntitiesModel
    {
        public MemberChurchlifeEventTaskModel() { }
        //
        public List<MemberChurchlifeEventTaskModel> lsMemberChurchlifeEventTaskModels { get; set; }
        public List<MemberChurchlifeEventTask> lsMemberChurchlifeEventTasks { get; set; }
        public MemberChurchlifeEventTask oMemberChurchlifeEventTask { get; set; }
         
        
        public AppUtilityNVP oChurchlifeActivity { get; set; }  // ChurchlifeActivity... jux the name of the activity
        public AppUtilityNVP oChurchlifeActivityReqDef { get; set; } //ChurchlifeActivityReqDef ... what req def /rule was been satisfied
        public ChurchCalendarEvent oChurchEvent { get; set; }  // this links to the calendar --- for What event was held at Where and When and by Who...
        public MemberChurchRole oMemberChurchRole { get; set; } //role assigned to ensure task was executed. ex. [Kojo Mensah, Counselor] performs [Premarital counseling] for would-be couples [member]  ... [oChurchEvent + oMemberChurchRole] >>> all persons mapped to this task ex. wedded couples [show names, link to profiles] ... batch process to copy same task to respective profiles

        public string strAppGloOwn { get; set; }
      //  public string strChurchBody { get; set; }
        public string strMemberChurchlifeActivity_Desc { get; set; }  // the pair of info: id:- [Sam - Wedding - 22 Dec 2013], [Sam - Naming]
        public string strChurchlifeActivity { get; set; }  //  [Wedding], [Naming] 
        public string strRequirementDefTask { get; set; }
        public string strMCLARoleDesc { get; set; }  // [Sam Darteh - Session Clerk], [Rev Amakye James, Minister in charge] ... generate, do not populate
        public string strDateCommenced { get; set; }
        public string strDateCompleted { get; set; }
        public string strDateDesc{ get; set; }
        public string strStatus { get; set; }

        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strOwnStatus { get; set; }  // Owned or Shared

        public IFormFile PhotoFile_CET { get; set; }

        public List<SelectListItem> lkpRequirementDefList { set; get; }
        public List<SelectListItem> lkpMemberChurchlifeActivities { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
        public List<SelectListItem> lkpTaskStatuses { set; get; }
    }

    public class MemberChurchGroupingModel : AbsChurchMemberModel  // Church Grouping ... Applies to only groupable units like... CO, CG, CE, SC, DP
    {
        public MemberChurchGroupingModel() { }
        // 
        public List<MemberChurchGroupingModel> lsMemberChurchGroupingModels { get; set; }
        public List<MemberChurchUnit> lsMemberChurchUnits { get; set; }
        public MemberChurchUnit oMemberChurchUnit { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        public string strChurchUnit { get; set; }
        public string strOrgType { get; set; }
        public string strJoinedDate { get; set; }
        public string strDepartedDate { get; set; }
        public string strDateDesc { get; set; }
        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }
         
        public string strAvailStatus { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared
        //
        public List<SelectListItem> lkpChurchOrgTypes { set; get; }
        public List<SelectListItem> lkpChurchUnits { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }

    }

    public class MemberRoleDesigModel : AbsChurchMemberModel   // Church roles, positions, designations
    {
        public MemberRoleDesigModel() { }
        // 
        public List<MemberRoleDesigModel> lsMemberRoleDesigModels { get; set; }
        public List<MemberChurchRole> lsMemberChurchRoles { get; set; }
        public MemberChurchRole oMemberChurchRole { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        public string strChurchRole { get; set; }
        public string strChurchUnit { get; set; }
        public string strOrgType { get; set; }
        public string strOrgType_CRL { get; set; }
        public string strFromDate { get; set; }
        public string strToDate { get; set; }
        public string strDateDesc { get; set; }
        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strAvailStatus { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared

        public IFormFile PhotoFile_CRL { get; set; }

        // public bool isRoleCurrent { get; set; }
        //
        public List<SelectListItem> lkpChurchOrgTypes { set; get; }
        public List<SelectListItem> lkpChurchOrgTypes_CRL { set; get; }
        public List<SelectListItem> lkpChurchUnits { set; get; }
        public List<SelectListItem> lkpChurchRoles { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }

    }

    public class MemberRegistrationModel : AbsChurchMemberModel
    {
        public MemberRegistrationModel() { }
        // 
        public List<MemberRegistrationModel> lsMemberRegistrationModels { get; set; }
        public List<MemberRegistration> lsMemberRegistrations { get; set; }
        public MemberRegistration oMemberRegistration { get; set; }
        // public ChurchMember oChurchMember { get; set; }

        public string strChurchYearFrom { get; set; }  
        public string strChurchYearTo { get; set; }  
        public string strRegYear { get; set; }  // churchperiod-year
        public string strRegCode { get; set; }
        public string strRegDate { get; set; }
        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strRegStatus { get; set; }  // Past or Current
        public string strRegStatusToDate { get; set; }  // Past or Current
        public string strOwnStatus { get; set; }  // Owned or Shared

        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
        public List<SelectListItem> lkpAccountPeriods { set; get; }
    }

    public class MemberChurchAttendanceModel : AbsChurchMemberModel
    {
        public MemberChurchAttendanceModel() { }
        // 
        public List<MemberChurchAttendanceModel> lsMemberChurchAttendanceModels { get; set; }
        public List<ChurchAttendAttendee> lsChurchAttendAttendees { get; set; }
        public ChurchAttendAttendee oChurchAttendAttendee { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
      //  public string strWorkExperience { get; set; }
        public DateTime dtFirstVisitDate { get; set; }
        public string strFirstVisitDate { get; set; }
        public int? FirstChurchEventId { get; set; }
        public string strFirstEventDesc { get; set; }
        public DateTime dtLastVisitDate { get; set; }
        public string strLastVisitDate { get; set; }
        public string strLastEventDesc { get; set; }
        public int? LastChurchEventId { get; set; }
          

        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }

        public string strRegAttended { get; set; }
        public string strOwnStatus { get; set; }  // Owned or Shared

        public List<SelectListItem> lkpSharingStatuses_Any { set; get; } 
        public List<SelectListItem> lkpChurchEvents { set; get; } 

    }

    public class MemberChurchTransferModel : AbsChurchMemberModel
    {
        public MemberChurchTransferModel() { }
        // 
        public List<MemberChurchTransferModel> lsMemberChurchTransferModels { get; set; }
        public List<ChurchTransfer> lsChurchTransfers { get; set; }
        public ChurchTransfer oChurchTransfer { get; set; }
        // public ChurchMember oChurchMember { get; set; }
        //
        //  public string strWorkExperience { get; set; }
        //

        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }
        public string strOwnStatus { get; set; }  // Owned or Shared

        //  public List<SelectListItem> lkpLanguages { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }
    }


    public class MemberChurchPaymentModel : AbsChurchMemberModel
    {
        public MemberChurchPaymentModel() { }
        // 
        public List<MemberChurchPaymentModel> lsMemberChurchPaymentModels { get; set; }
        //public List<TitheTrans> lsTitheTrans { get; set; }  // mem payments will land in the church's [Receipts journal]
        //public TitheTrans oTitheTrans { get; set; }

        // public ChurchMember oChurchMember { get; set; }
        //
        //  public string strWorkExperience { get; set; }
        //

        public string strOwnedByCB { get; set; }
        public string strOwnedByCBLevel { get; set; }
        public string strOwnStatus { get; set; }  // Owned or Shared

        //  public List<SelectListItem> lkpLanguages { set; get; }
        public List<SelectListItem> lkpSharingStatuses_Any { set; get; }

    }


     


    // other models -- extended

    public class CBMemRollModel
    {
        public CBMemRollModel() { }
        // 
        public List<CBMemRollModel> lsCBMemRollModels { get; set; }
        public List<CBMemberRollBal> lsCBMemberRollBals { get; set; }
        public CBMemberRollBal oCBMemberRollBal { get; set; }
        public ChurchBody oChurchBody { get; set; }


        ///
        public int? oChurchBodyId { get; set; }
        public int? oAppGloOwnId { get; set; }
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        public ChurchMember oCurrLoggedMember_Logged { get; set; }

        ///
        public string strTotRoll { get; set; }
        public string strTot_M { get; set; }  // Male
        public string strTot_F { get; set; }  // Female
        public string strTot_O { get; set; }  // Other
        public string strTot_C { get; set; }  // Child
        public string strTot_Y { get; set; }  // Youth .. get the settings for age-groupings :- this may be diff from the generational groupings aging
        public string strTot_YA { get; set; } // Young Adult ... 
        public string strTot_MA { get; set; } // Mid-aged Adult
        public string strTot_AA { get; set; } // [Aged] Adult 
        //
        public string strTot_GA { get; set; } // gEN Adult = [YA + MA + OA] 
        public string strTot_OA { get; set; } // Older [Aged] Adult   = [ MA + OA] 
        public string strTot_NA { get; set; } // Child/Youth [NonAdult]   = [ C + Y] 
        ///
        public string strTot_LDR { get; set; }   
        public string strTot_CLG { get; set; }


        ///
        public string strGrandTotRoll { get; set; }
        public string strGrandTot_M { get; set; }  // Male
        public string strGrandTot_F { get; set; }  // Female
        public string strGrandTot_O { get; set; }  // Other
        public string strGrandTot_C { get; set; }  // Child
        public string strGrandTot_Y { get; set; }  // Youth .. get the settings for age-groupings :- this may be diff from the generational groupings aging
        public string strGrandTot_YA { get; set; } // Young Adult ... 
        public string strGrandTot_MA { get; set; } // Mid-aged Adult
        public string strGrandTot_AA { get; set; } // [Aged] Adult 
        //
        public string strGrandTot_GA { get; set; } // gEN Adult = [YA + MA + OA] 
        public string strGrandTot_OA { get; set; } // Older [Aged] Adult   = [ MA + OA] 
        public string strGrandTot_NA { get; set; } // Child/Youth [NonAdult]   = [ C + Y] 
        ///
        public string strGrandTot_LDR { get; set; }
        public string strGrandTot_CLG { get; set; }
        ///
        public string strAppGlobalOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strCurrTask { get; set; }
        public int taskIndex { get; set; }
         
    }
     


    public class CUMemRollModel
    {
        public CUMemRollModel() { }
        // 
        public List<CUMemRollModel> lsCUMemRollModels { get; set; }
        public List<CUMemberRollBal> lsCUMemberRollBals { get; set; }
        public CUMemberRollBal oCUMemberRollBal { get; set; }


        ///
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; } 
        public int? oChurchMemberId_Logged { get; set; } 
        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        public ChurchMember oCurrLoggedMember_Logged  { get; set; }

        ///
        public string strTotRoll { get; set; }
        public string strTot_M { get; set; }  // Male
        public string strTot_F { get; set; }  // Female
        public string strTot_O { get; set; }  // Other
        public string strTot_C { get; set; }  // Child
        public string strTot_Y { get; set; }  // Youth .. get the settings for age-groupings :- this may be diff from the generational groupings aging
        public string strTot_YA { get; set; } // Young Adult ... Adult = [YA + MA + OA]
        public string strTot_MA { get; set; } // Mid-aged Adult
        public string strTot_OA { get; set; } // Older [Aged] Adult 

        ///
        public string strTot_Adult { get; set; } //  Adult = [YA + MA + OA]
        public string strTot_NonAdult { get; set; } // NA = [C + Y]

        ///
        public string strAppGlobalOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchUnit { get; set; } 
    }



    public class CBTitheBalModel
    {
        public CBTitheBalModel() { }
        // 
        public List<CBTitheBalModel> lsCBTitheBalModels { get; set; }
        public List<CBTitheTransBal> lsCBTitheTransBals { get; set; }
        public CBTitheTransBal oCBTitheTransBal { get; set; }
        public ChurchBody oChurchBody { get; set; }


        ///
        public int? oChurchBodyId { get; set; }
        public int? oAppGloOwnId { get; set; }
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        public ChurchMember oCurrLoggedMember_Logged { get; set; }

        ///
        public string strTotCol { get; set; }   // rec
        public string strTotOut { get; set; }  // disb
        public string strTotNet { get; set; }  // net
          
        ///
        public string strGrandTotCol { get; set; }
        public string strGrandTotOut { get; set; }  //  
        public string strGrandTotNet { get; set; }  //  
         
        ///
        public string strAppGlobalOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strCurrTask { get; set; }
        public int taskIndex { get; set; }

    }




}
