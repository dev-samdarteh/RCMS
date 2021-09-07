using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using System;
using System.Collections.Generic;

namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public abstract class SetupEntitiesModel
    {
        public SetupEntitiesModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public AppGlobalOwner oAppGlobalOwn { get; set; }
        public ChurchBody oChurchBody { get; set; }  // grace
        //public ChurchLevel oChurchLevel { get; set; }
        
        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
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
        public int tempSetIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; } 
        public int filterIndex { get; set; } 
    }

    public class CountryModel : SetupEntitiesModel   //for CTRY and CURR
    {
        public CountryModel() { } 
        //
        public List<CountryModel> lsCountryModels { get; set; }
        public List<Country> lsCountries { get; set; }
       // public CountryModel oCountryModel { get; set; }
        public Country oCountry { get; set; }
        public string strCountry { get; set; }  
        public bool bl_IsCustomDisplay { get; set; } 
        public bool bl_IsCustomDefaultCountry { get; set; }
        public bool bl_IsCustomChurchCountry { get; set; }
    }
    public class CountryCustomModel : SetupEntitiesModel
    {
        public CountryCustomModel() { }
        //
        public List<CountryCustomModel> lsCountryCustomModels { get; set; }
        public List<CountryCustom> lsCountriesCustom { get; set; }
        public CountryCustom oCountryCustom { get; set; }

        public string strAppGloOwn { get; set; }
        public string strCountry { get; set; }

        public List<SelectListItem> lkpCountries { set; get; } 
    }

    public class CurrencyCustomModel : SetupEntitiesModel
    {
        public CurrencyCustomModel() { }
        //
        public List<CurrencyCustomModel> lsCurrencyCustomModels { get; set; }
        public List<CurrencyCustom> lsCountriesCustom { get; set; }
        public CurrencyCustom oCurrencyCustom { get; set; }
        public Country oCountry { get; set; }

        public string strAppGloOwn { get; set; }
        public string strCountry { get; set; }
        public string strCurrEngName { get; set; }
        public string strCurrSymbol { get; set; }
        public string strCurr3LISOSymbol { get; set; }
        public bool bl_IsCustomDisplay { get; set; }
       public bool bl_IsBaseCurrency { get; set; }
       public decimal numBaseRate { get; set; }
       public string strBaseRate { get; set; }

        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpCurrencies { set; get; }
    }


    public class CountryRegionModel : SetupEntitiesModel
    {
        public CountryRegionModel() { }
        //
        public List<CountryRegionModel> lsCountryRegionModels { get; set; }
        public List<CountryRegion> lsCountryRegions { get; set; }
        public CountryRegion oCountryRegion { get; set; }

        public string strAppGloOwn { get; set; }  
        public string strCountry { get; set; } 
        public string strSharingStatus { get; set; } 
        public string strOwnershipCode { get; set; } 
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
        public string strCountryRegion { get; set; }
        public bool bl_IsCustomDisplay { get; set; }
        public bool bl_IsCustomDefaultRegion { get; set; }
        public bool bl_IsCustomChurchRegion { get; set; }

        public string currCountryCode { get; set; }

        public List<SelectListItem> lkpCountries { set; get; } 
        public List<SelectListItem> lkpSharingStatuses { set; get; } 
    }


    public class CountryRegionCustomModel : SetupEntitiesModel
    {
        public CountryRegionCustomModel() { }
        //
        public List<CountryRegionCustomModel> lsCountryRegionCustomModels { get; set; }
        public List<CountryRegionCustom> lsCountryRegionsCustom { get; set; }
        public CountryRegionCustom oCountryRegionCustom { get; set; }

        public string strAppGloOwn { get; set; }
        public string strCountry { get; set; }
        public string strCountryRegion { get; set; }
        public string strCountryRegionCustom { get; set; }
        public bool bl_IsCustomDisplay { get; set; }

        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpCountryRegions { set; get; }
       // public List<SelectListItem> lkpCountryRegionsCustom { set; get; }
    }

    public class AppUtilityNVPModel : SetupEntitiesModel
    {
        public AppUtilityNVPModel() { }
        //
        public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }
        public List<AppUtilityNVPModel> lsAppUtilityNVPModels_All { get; set; }
        public List<AppUtilityNVP> lsAppUtilityNVPs { get; set; }
        public AppUtilityNVP oAppUtilityNVP { get; set; }

        public ChurchBody modOwnedByChurchBody { get; set; }

        public string strAppGloOwn { get; set; }
        public string strAppUtilityNVP { get; set; }
        //public string strNVPItem_Parent { get; set; }
        //public int? numNVPCode_Parent { get; set; }
        public string strNVPValue_Cumm { get; set; }
        public string strNVPValue { get; set; }
        public decimal? numAppUtilityNVP { get; set; }
        public decimal? numAppUtilityNVPTo { get; set; }
        public DateTime? dt1AppUtilityNVP { get; set; }
        public DateTime? dt2AppUtilityNVP { get; set; }
        public string strNVPTag { get; set; }
        public string strNVPSubTag { get; set; }

        public string strNVPCode { get; set; }
        public string strNVPSubCode { get; set; }

        public string strCurrNVPCode { get; set; }
        public int? numNVP_ParentCategoryId { get; set; }
        public int? numOrderIndex { get; set; }
        //public int? numAppUtilityNVP { get; set; }        
        public string strNVPCategory { get; set; }
      //  public int numTargetOccurences { get; set; }
        public string strOccurFrequency { get; set; }
                

        public string strNVPStatus { get; set; }
        public string strCountry { get; set; }
       // public bool bl_IsDeceased { get; set; }
       // public bool bl_IsAvailable { get; set; }
       // public bool bl_ApplyToClergyOnly { get; set; }
        public bool bl_NVPStatus_Active { get; set; } 
        //public bool bl_IsValNumeric { get; set; }
        //public bool bl_IsValDate { get; set; }

        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; } 

        public List<SelectListItem> lkpAppGlobalOwns { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }
        public List<SelectListItem> lkpAppParameterTags { set; get; }
        public List<SelectListItem> lkpCLAList { set; get; }
        public List<SelectListItem> lkpNVPCategories { set; get; }  // NVPSubCode -- pre-determined
        public List<SelectListItem> lkpAppValueTypes { set; get; }
        public List<SelectListItem> lkpAttendeeTypes { set; get; }
        public List<SelectListItem> lkpNVP_ParentCategories { set; get; }
        public List<SelectListItem> lkpIntervalFreqs { set; get; }
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }  // N - Do Not Share  , A - All Share  , C - Direct Child Share
    }

      

    public class LanguageSpokenModel : SetupEntitiesModel
    {
        public LanguageSpokenModel() { }
        //
        public List<LanguageSpokenModel> lsLanguageSpokenModels { get; set; }
        public List<LanguageSpoken> lsLanguageSpokens { get; set; }
        public LanguageSpoken oLanguageSpoken { get; set; }

        public string strAppGloOwn { get; set; }
        public string strLanguageSpoken { get; set; }
        public string strCountry { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
         
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }  // N - Do Not Share  , A - All Share  , C - Direct Child Share
    }

    public class LanguageSpokenCustomModel : SetupEntitiesModel
    {
        public LanguageSpokenCustomModel() { }
        //
        public List<LanguageSpokenCustomModel> lsLanguageSpokenCustomModels { get; set; }
        public List<LanguageSpokenCustom> lsLanguageSpokenCustoms { get; set; }
        public LanguageSpokenCustom oLanguageSpokenCustom { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strLanguageSpoken { get; set; }
        public bool bl_IsDisplay { get; set; }
        public bool bl_IsDefaultLanguage { get; set; }
        public bool bl_IsChurchLanguage { get; set; }

        public List<SelectListItem> lkpLanguages { set; get; } 
    }

    public class ChurchPeriodModel : SetupEntitiesModel
    {
        public ChurchPeriodModel() { }
        //
        public List<ChurchPeriodModel> lsChurchPeriodModels { get; set; }
        public List<ChurchPeriodModel> lsChurchPeriodModels_AY { get; set; }
        public List<ChurchPeriodModel> lsChurchPeriodModels_CY { get; set; }
        public List<ChurchPeriod> lsChurchPeriods { get; set; }
        public ChurchPeriod oChurchPeriod { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchPeriod { get; set; }
        public string strFrom { get; set; }
        public string strTo { get; set; }        
        public string strPeriodType { get; set; }
        public string strInterval { get; set; }
        public string strIntervalDays { get; set; }
        public string strIntervalFreq { get; set; }

        public string strYear { get; set; }
        public string strQuarter { get; set; }
        public string strSemester { get; set; }
        public string strMonth { get; set; }
        public string strWeek { get; set; } 

        public string strStatus { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
        public bool bl_IsStartingPeriod { get; set; }
        public bool bl_IsPeriodActive { get; set; }
        public bool bl_IsIntervalDefinition { get; set; }  

        public List<SelectListItem> lkpYears { set; get; }   // Year, Semester, Quarter, Month, Week, Day
        public List<SelectListItem> lkpIntervalFreqs { set; get; }   // Year, Semester, Quarter, Month, Week, Day
        public List<SelectListItem> lkpPeriodTypes { set; get; }  //  AP--Accounting Period, CC -- Church Calendar
        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }  // Open, Closed, Pending, Blocked
    }


    public class National_IdTypeModel : SetupEntitiesModel
    {
        public National_IdTypeModel() { }
        //
        public List<National_IdTypeModel> lsNational_IdTypeModels { get; set; }
        public List<National_IdType> lsNational_IdTypes { get; set; }
        public National_IdType oNational_IdType { get; set; }

        public string strAppGloOwn { get; set; }
        public string strNational_IdType { get; set; }
        public string strCountry { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }

        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }  // N - Do Not Share  , A - All Share  , C - Direct Child Share
    }


    public class InstitutionTypeModel : SetupEntitiesModel
    {
        public InstitutionTypeModel() { }
        //
        public List<InstitutionTypeModel> lsInstitutionTypeModels { get; set; }
        public List<InstitutionType> lsInstitutionTypes { get; set; }
        public InstitutionType oInstitutionType { get; set; }

        public string strAppGloOwn { get; set; }
        public string strInstitutionType { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }

        public List<SelectListItem> lkpSharingStatuses { set; get; }  // N - Do Not Share  , A - All Share  , C - Direct Child Share
    }


    public class CertificateTypeModel : SetupEntitiesModel
    {
        public CertificateTypeModel() { }
        //
        public List<CertificateTypeModel> lsCertificateTypeModels { get; set; }
        public List<CertificateType> lsCertificateTypes { get; set; }
        public CertificateType oCertificateType { get; set; }

        public string strAppGloOwn { get; set; }
        public string strCertificateType { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }

        public List<SelectListItem> lkpSharingStatuses { set; get; }  // N - Do Not Share  , A - All Share  , C - Direct Child Share
    }


    public class RelationshipTypeModel : SetupEntitiesModel
    {
        public RelationshipTypeModel() { }
        //
        public List<RelationshipTypeModel> lsRelationshipTypeModels { get; set; }
        public List<RelationshipType> lsRelationshipTypes { get; set; }
        public RelationshipType oRelationshipType { get; set; }

        public string strAppGloOwn { get; set; }
        public string strRelationshipType { get; set; }

        public string strRelationshipTypeFemalePairId { get; set; }
        public string strRelationshipTypeGenericPairId { get; set; }
        public string strRelationshipTypeMalePairId { get; set; }

        //public bool bl_IsChild { get; set; }  // son, daughter, child
        //public bool bl_IsSpouse { get; set; }  // wife, husband
        //public bool bl_IsParent { get; set; }  // father, mother, guardian

    }


    public class  ChurchUnitModel : SetupEntitiesModel
    {
        public ChurchUnitModel() { }
        //
        public List<ChurchUnitModel> lsChurchUnitModels { get; set; }
        public List<ChurchUnitModel> lsChurchUnitModels_All { get; set; }
        public List<ChurchUnit> lsChurchUnits { get; set; }
        //public ChurchBody oChurchBody { get; set; }
        public ChurchUnit oChurchUnit { get; set; }

       // [Display(Name = "Church unit logo")]
        public IFormFile UnitLogoFile { get; set; }
        public string strUnitLogo { get; set; }

        public string strAppGlobalOwn { get; set; }
        public string strChurchBody { get; set; }  
       // public string strChurchLevel_CB { get; set; }
        public string strSupervisedByUnit { get; set; }
       // public int? numSupervisedByChurchUnitId { get; set; }

        public string strChurchUnit { get; set; } 
        public string strParentChurchUnit { get; set; } 
        public string strRootChurchBodyCode { get; set; }
        
        public string strParentUnitOrgType { get; set; } 
        public string strParentUnitOrgTypeCode { get; set; } 
        public int? numParentUnitCLId { get; set; }     
        public int? numParentUnitId { get; set; }     
       // public int? numParentUnitCBId { get; set; }
        public string strParentUnitCB { get; set; } 
     
        public string strSupervisedByUnitOrgType { get; set; }
        public string strSupervisedByUnitOrgTypeCode { get; set; }
        public int? numSupervisedByUnitCLId { get; set; }
       // public int? numSupervisedByUnitCBId { get; set; }
        public string strSupervisedByUnitCB { get; set; } 
        
        
        public int numSubUnitsCount { get; set; }   
        // public bool bl_IsUnitStatusBlocked { get; set; }   
        public string strCBLevel { get; set; }        
        public string strCBLevel_par { get; set; }        
        public string strCBLevel_sup { get; set; }        
        public string strOrgType { get; set; }        
        public string strTargetChurchLevel { get; set; }     
        public string strDateFormed { get; set; }        
        public string strDateInnaug { get; set; }
        public string strDateDeactive { get; set; }
        public string strGenderStatus { get; set; } // Female, Male, Mixed                  
      //  public bool? bl_IsUnitGen { get; set; }        
       // public bool? bl_AgeBracketOverlaps { get; set; }
        public string strStatus { get; set; } // Active, Blocked, Deactive  
        public string strSharingStatus { get; set; } // All share, Child share, Not share  
        public string strChurchWorkStatus { get; set; }      //   Operationalized - O, Structure only - S 
        public string strOwnershipStatus { get; set; }   // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody 
        public string strOwnershipCode { get; set; }    
        public string strChurchLevel_OwnedByCB { get; set; }         
        public string strOwnedByChurchBody { get; set; }    
        //
        // public string strChurchLevel { get; set; }
        public int numChurchLevel_Index { get; set; }
       // public string strCH_InCharge { get; set; }  
       
       // public bool bl_IsActivated { get; set; }
        //public bool bl_IsUnitGen { get; set; }
        //public bool bl_IsAgeBracketOverlaps { get; set; } 

        
        public string strCongLoc { get; set; }
        public string strCongLoc2 { get; set; }
       // public string strUnitLogo { get; set; }
        public string strParentUnit_HeaderDesc { get; set; }

        public DateTime? dtCreated { get; set; }

        //public int? oCurrAppGloId_Filter5 { get; set; }
        //public int? oCurrChuCategId_Filter5 { get; set; }
        //public bool oCurrShowAllCong_Filter5 { get; set; }
        //         
        public int oCULevelCount { get; set; }

        public int? oCurrAppGloId_Filter5 { get; set; }
        public int? oCurrChuCategId_Filter5 { get; set; }
        public bool oCurrShowAllCong_Filter5 { get; set; }
        //         
        public int oCBLevelCount { get; set; }
        //  public int numCLIndex { get; set; }

        public string strChurchLevel_1 { get; set; }
        public string strChurchLevel_2 { get; set; }
        public string strChurchLevel_3 { get; set; }
        public string strChurchLevel_4 { get; set; }
        public string strChurchLevel_5 { get; set; }
        public int? ChurchBodyId_1 { get; set; }
        public int? ChurchBodyId_2 { get; set; }
        public int? ChurchBodyId_3 { get; set; }
        public int? ChurchBodyId_4 { get; set; }
        public int? ChurchBodyId_5 { get; set; }
        //
        //  public int? oChurchBodyId_1 { set; get; }
        public string strChurchBody_1 { set; get; }
        public string strChurchBody_2 { set; get; }
        public string strChurchBody_3 { set; get; }
        public string strChurchBody_4 { set; get; }
        public string strChurchBody_5 { set; get; }
        //
        public List<ChurchBody> lsChurchBody { get; set; }
        //
        public List<SelectListItem> lkp_ChurchBodies_1 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_2 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_3 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_4 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_5 { set; get; }


        public List<SelectListItem> lkpChurchBodies { set; get; }
        public List<SelectListItem> lkpChurchLevels { set; get; }
        public List<SelectListItem> lkpChurchLevels_CU { set; get; }
        public List<SelectListItem> lkpParentChurchUnits { set; get; }
        public List<SelectListItem> lkpParentChurchBodies { set; get; }
        public List<SelectListItem> lkpOrgTypes { set; get; }
        public List<SelectListItem> lkpOrgTypesAllUnits { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }
        public List<SelectListItem> lkpGenderStatuses { set; get; }
        public List<SelectListItem> lkpOwnershipStatuses { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpChurchWorkStatuses { set; get; }
    }


    public class ChurchRankModel : SetupEntitiesModel
    {
        public ChurchRankModel() { }
        //
        public List<ChurchRankModel> lsChurchRankModels { get; set; }
        //public List<ChurchRank> lsChurchRanks { get; set; }
        //public ChurchRank oChurchRank { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchRank { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }

        public List<SelectListItem> lkpSharingStatuses { set; get; }
    }


    public class ChurchRoleModel : SetupEntitiesModel
    {
        public ChurchRoleModel() { }
        //
        public List<ChurchRoleModel> lsChurchRoleModels { get; set; }
        public List<ChurchRole> lsChurchRoles { get; set; }
        public ChurchRole oChurchRole { get; set; }

        public string strAppGloOwn { get; set; }
       public string strChurchBody { get; set; }
        public string strChurchRole { get; set; } 
        public string strOwnedByChurchBody { get; set; }        
        public string strAppGlobalOwn { get; set; }        
        public string strParentRole { get; set; }        
        public string strParentRoleOrgTypeCode { get; set; }        
        public string strParentRoleOrgType { get; set; }        
        public string strOrgType { get; set; }        
        public string strApplyToChurchUnit { get; set; }        
        public string strApplyToChurchUnitOrgTypeCode { get; set; }        
        public string strApplyToChurchUnitOrgType { get; set; }        
        public string strCBLevel { get; set; }        
        public string strCBLevel_sup { get; set; }        
        public string strTargetChurchLevel { get; set; }   
        //public string strDateFormed { get; set; }        
        //public string strDateInnaug { get; set; }        
        //public string strDateDeactive { get; set; }   
        
        public bool bl_IsParentRole { get; set; }
        public int numSubRolesCount { get; set; }

        public bool bl_IsActivated { get; set; }
        public bool bl_ApplyToClergyOnly { get; set; }        
        public string strStatus { get; set; }
        public string strGenderStatus { get; set; }
        public string strChurchWorkStatus { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; } 
        public string strOwnershipStatus { get; set; }   // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody 
        public string strChurchLevel_OwnedByCB { get; set; }
        public string strParentRoleCB { get; set; }
        public string strParentUnit_HeaderDesc { get; set; }

        public string strRootChurchBodyCode { get; set; }

        public int? numParentRoleCLId { get; set; }
        public int numChurchLevel_Index { get; set; }

        public List<SelectListItem> lkpChurchBodies { set; get; }
        public List<SelectListItem> lkpChurchLevels { set; get; }
        public List<SelectListItem> lkpChurchLevels_CRL { set; get; }
        public List<SelectListItem> lkpParentChurchRoles { set; get; }
        //public List<SelectListItem> lkpOrgTypes { set; get; }
        public List<SelectListItem> lkpOrgTypes_ApplyToUnit { set; get; }
        public List<SelectListItem> lkpOrgTypes_CRL { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }
        public List<SelectListItem> lkpOwnershipStatuses { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpChurchWorkStatuses { set; get; }
        public List<SelectListItem> lkpOfficeTermTypes { set; get; }
      //  public List<SelectListItem> lkpParentChurchUnits { set; get; }
        public List<SelectListItem> lkpApplyToChurchUnits { set; get; }
        public List<SelectListItem> lkpParentChurchBodies { set; get; }
        public List<SelectListItem> lkpGenderStatuses { set; get; }

        public int oCBLevelCount { get; set; }

        public int? oCurrAppGloId_Filter5 { get; set; }
        public int? oCurrChuCategId_Filter5 { get; set; }
        public bool oCurrShowAllCong_Filter5 { get; set; }
        //          
        //  public int numCLIndex { get; set; }

        public string strChurchLevel_1 { get; set; }
        public string strChurchLevel_2 { get; set; }
        public string strChurchLevel_3 { get; set; }
        public string strChurchLevel_4 { get; set; }
        public string strChurchLevel_5 { get; set; }
        public int? ChurchBodyId_1 { get; set; }
        public int? ChurchBodyId_2 { get; set; }
        public int? ChurchBodyId_3 { get; set; }
        public int? ChurchBodyId_4 { get; set; }
        public int? ChurchBodyId_5 { get; set; }
        //
        //  public int? oChurchBodyId_1 { set; get; }
        public string strChurchBody_1 { set; get; }
        public string strChurchBody_2 { set; get; }
        public string strChurchBody_3 { set; get; }
        public string strChurchBody_4 { set; get; }
        public string strChurchBody_5 { set; get; }
        //
        public List<ChurchBody> lsChurchBody { get; set; }
        //
        public List<SelectListItem> lkp_ChurchBodies_1 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_2 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_3 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_4 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_5 { set; get; }


    }


    public class ChurchlifeActivityModel : SetupEntitiesModel
    {
        public ChurchlifeActivityModel() { }
        //
        public List<ChurchlifeActivityModel> lsChurchlifeActivityModels { get; set; }
        public List<ChurchlifeActivity> lsChurchlifeActivities { get; set; }
        public ChurchlifeActivity oChurchlifeActivity { get; set; }
        //
        public List<ChurchlifeActivityReqDefModel> lsChurchlifeActivityReqDefModels { get; set; }


        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchlifeActivity { get; set; }  
        public string strActivityType { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }

        //public bool bl_IsMainline { get; set; }
        //public bool bl_IsService { get; set; }
        ///
        //public string strAppGloOwn { get; set; }
        //public string strChurchBody { get; set; }
        //public string strChurchlifeActivityReqDef { get; set; }
        //public string strChurchlifeActivity { get; set; }
        //public string strFrequency { get; set; }
        //public string strSharingStatus_CLARD { get; set; }
        //public string strOwnershipCode_CLARD { get; set; }
        //public string strOwnershipStatus_CLARD { get; set; }
        //public string strOwnedByChurchBody_CLARD { get; set; }
        //public string strChurchLevel_OwnedByCB_CLARD { get; set; }
        //public bool bl_IsSequenced { get; set; }
        //public bool bl_IsRequired { get; set; }


        public List<SelectListItem> lkpActivityTypes { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }
        ///
        //public List<SelectListItem> lkpChurchlifeActivities { set; get; }
        //public List<SelectListItem> lkpFrequencies { set; get; }
        //public List<SelectListItem> lkpSharingStatuses_CLARD { set; get; }

    }


    public class ChurchlifeActivityReqDefModel : SetupEntitiesModel
    {
        public ChurchlifeActivityReqDefModel() { }
        //
        public List<ChurchlifeActivityReqDefModel> lsChurchlifeActivityReqDefModels { get; set; }
        public List<ChurchlifeActivityReqDef> lsChurchlifeActivityReqDefs { get; set; }
        public ChurchlifeActivityReqDef oChurchlifeActivityReqDef { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchlifeActivityReqDef { get; set; }
        public string strChurchlifeActivity { get; set; }
        public string strFrequency { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
       // public bool bl_IsSequenced { get; set; }
       // public bool bl_IsRequired { get; set; }
        
        public List<SelectListItem> lkpChurchlifeActivities { set; get; }
        public List<SelectListItem> lkpFrequencies { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }
    }


    public class ChurchMemTypeModel : SetupEntitiesModel
    {
        public ChurchMemTypeModel() { }
        //
        public List<ChurchMemTypeModel> lsChurchMemTypeModels { get; set; }
        public List<ChurchMemType> lsChurchMemTypes { get; set; }
        public ChurchMemType oChurchMemType { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchMemType { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
        public bool bl_ApplyToClergyOnly { get; set; }

        public List<SelectListItem> lkpSharingStatuses { set; get; }
    }


    public class ChurchMemStatusModel : SetupEntitiesModel
    {
        public ChurchMemStatusModel() { }
        //
        public List<ChurchMemStatusModel> lsChurchMemStatusModels { get; set; }
        //public List<ChurchMemStatus> lsChurchMemStatuss { get; set; }
        //public ChurchMemStatus oChurchMemStatus { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchMemStatus { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
        public bool bl_IsDeceased { get; set; }
        public bool bl_IsAvailable { get; set; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }
    }


    public class ChurchTransferSettingsModel : SetupEntitiesModel
    {
        public ChurchTransferSettingsModel() { }
        //
        public List<ChurchTransferSettingsModel> lsChurchTransferSettingsModels { get; set; }
        //public List<ChurchTransferSettings> lsChurchTransferSettingss { get; set; }
        //public ChurchTransferSettings oChurchTransferSettings { get; set; }

        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strChurchTransferSettings { get; set; }
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }
        public bool isDeceased { get; set; }
        public bool isAvailable { get; set; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }


        //public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }
        //public List<AppUtilityNVP> lsAppUtilityNVPs { get; set; }
        //public AppUtilityNVP oAppUtilityNVP { get; set; }

        //public string strAppGloOwn { get; set; }
        //public string strNVPTag { get; set; }
        //// public string strNVPCode { get; set; }
        //// public int numOrderIndex { get; set; }
        //// public int numAppUtilityNVP { get; set; }
        //// public string strAppUtilityNVPName { get; set; }    
        //public string strNVPCategory { get; set; }
        //public string strNVPStatus { get; set; }

        //public List<SelectListItem> lkpAppGlobalOwns { set; get; }
        //public List<SelectListItem> lkpStatuses { set; get; }
        //public List<SelectListItem> lkpAppParameterTags { set; get; }
        //public List<SelectListItem> lkpNVPCategories { set; get; }
    }


    public class AdhocParameterModel : SetupEntitiesModel
    {
        public AdhocParameterModel() { }
        //
        public List<MemberCustomCodeFormatModel> lsMemberCustomCodeFormatModels { get; set; }
        public List<ChurchPeriodDefinitionModel> lsChurchPeriodDefinitionModels { get; set; }
        public List<ChurchTransferSettingModel> lsChurchTransferSettingModels { get; set; }
        public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }  
        public AppUtilityNVP oAppUtilityNVP { get; set; }
        public MemberCustomCodeFormatModel oMemberCustomCodeFormatModel { get; set; }
        public ChurchPeriodDefinitionModel oChurchPeriodDefinitionModel { get; set; }
        public ChurchTransferSettingModel oChurchTransferSettingModel { get; set; }

        ///  
        //public int? modAppGlobalOwnerId { get; set; }
        //public int? modChurchBodyId { get; set; }


       // [System.ComponentModel.DataAnnotations.StringLength(10)]
        //public string modNVPCode { get; set; }
        //public string modNVPSubCode { get; set; }

        //  [System.ComponentModel.DataAnnotations.StringLength(1)]
        //public string modSharingStatus { get; set; }
        //public int? modOwnedByChurchBodyId { get; set; }
        //public ChurchBody modOwnedByChurchBody { get; set; }
        ///  
        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }

        //// for the table
        //public string strSettingDesc { get; set; }
        //public string strSettingValue { get; set; }        

        //// MCCF
        //public string strPrefix { get; set; }
        //public string strPrefixDelim { get; set; }
        //public string strSuffix { get; set; }
        //public string strSuffixDelim { get; set; }
        //public string strSampleTextLDZR { get; set; }
        //public bool bl_IsAutogenMCCFCode { get; set; }
        //public bool bl_IsLeadingZeros { get; set; } // 0123 or 123, 0012 or 12, 0001 or 1

        //

        //public string modSharingStatus { get; set; }
        //public int? modOwnedByChurchBodyId { get; set; }
        //public ChurchBody modOwnedByChurchBody { get; set; }

        //public string strSharingStatus { get; set; }
        //public string strOwnershipCode { get; set; }
        //public string strOwnershipStatus { get; set; }
        //public string strOwnedByChurchBody { get; set; }
        //public string strChurchLevel_OwnedByCB { get; set; }


        //// CPRD 
        //public string modNVPSubCode_AP { get; set; }
        //public decimal? numPeriodInterval_AP { get; set; }  // 356
        //public decimal? numPeriodIntervalDays_AP { get; set; }
        //public string strPeriodType_AP { get; set; }   // Yr, Mo, Wk, Da
        //public string strPeriodStartDate_AP { get; set; }
        //public DateTime? PeriodStartDate_AP { get; set; } // Accounting Period,    

        //public string modSharingStatus_AP { get; set; }
        //public int? modOwnedByChurchBodyId_AP { get; set; }
        //public ChurchBody modOwnedByChurchBody_AP { get; set; }

        /////
        //public string modNVPSubCode_CP { get; set; }
        //public decimal? numPeriodInterval_CP { get; set; }  // 356      
        //public decimal? numPeriodIntervalDays_CP { get; set; }
        //public string strPeriodType_CP { get; set; }   // Yr, Mo, Wk, Da
        //public string strPeriodStartDate_CP { get; set; }
        //public DateTime? PeriodStartDate_CP { get; set; } // church Calendar Period


        //public string modSharingStatus_CP { get; set; }
        //public int? modOwnedByChurchBodyId_CP { get; set; }
        //public ChurchBody modOwnedByChurchBody_CP { get; set; }

        //public bool bl_IsChuPeriodSameAsAccPeriod { get; set; }

        //// CT
        ////public int numPeriodInterval { get; set; }
        ////public string strPeriodType { get; set; }
        ////public DateTime strPeriodStartDate { get; set; }


        //public string strSharingStatus_AP { get; set; }
        //public string strOwnershipCode_AP { get; set; }
        //public string strOwnershipStatus_AP { get; set; }
        //public string strOwnedByChurchBody_AP { get; set; }        
        //public string strChurchLevel_OwnedByCB_AP { get; set; }

        //public string strSharingStatus_CP { get; set; }
        //public string strOwnershipCode_CP { get; set; }
        //public string strOwnershipStatus_CP { get; set; }
        //public string strOwnedByChurchBody_CP { get; set; }
        //public string strChurchLevel_OwnedByCB_CP { get; set; }


        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpPeriodTypes { set; get; }
        public List<SelectListItem> lkpIntervalFreqs { set; get; }
        public List<SelectListItem> lkpCPR_Years { set; get; }
        public List<SelectListItem> lkpCPR_Semesters { set; get; }
        public List<SelectListItem> lkpCPR_Quarters { set; get; }
        public List<SelectListItem> lkpCPR_Months { set; get; }
        public List<SelectListItem> lkpCPR_Weeks { set; get; }
        public List<SelectListItem> lkpCPR_Days { set; get; }

    }

    public class ChurchPeriodDefinitionModel : SetupEntitiesModel
    {
        public ChurchPeriodDefinitionModel() { }
        //
        public List<ChurchPeriodDefinitionModel> lsChurchPeriodDefinitionModels { get; set; }
        public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }
        public AppUtilityNVP oAppUtilityNVP { get; set; }

        ///  
        //public int? modAppGlobalOwnerId { get; set; }
        //public int? modChurchBodyId { get; set; }


        // [System.ComponentModel.DataAnnotations.StringLength(10)]
        public string modNVPCode { get; set; }
        public string modNVPSubCode { get; set; }

        //  [System.ComponentModel.DataAnnotations.StringLength(1)]
        //public string modSharingStatus { get; set; }
        //public int? modOwnedByChurchBodyId { get; set; }
        //public ChurchBody modOwnedByChurchBody { get; set; }
        ///  
        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }

        //// for the table
        //public string strSettingDesc { get; set; }
        //public string strSettingValue { get; set; }

        //// MCCF
        //public string strPrefix { get; set; }
        //public string strPrefixDelim { get; set; }
        //public string strSuffix { get; set; }
        //public string strSuffixDelim { get; set; }
        //public string strSampleTextLDZR { get; set; }
        //public bool bl_IsAutogenMCCFCode { get; set; }
        //public bool bl_IsLeadingZeros { get; set; } // 0123 or 123, 0012 or 12, 0001 or 1

        //

        //public string modSharingStatus { get; set; }
        //public int? modOwnedByChurchBodyId { get; set; }
        //public ChurchBody modOwnedByChurchBody { get; set; }

        //public string strSharingStatus { get; set; }
        //public string strOwnershipCode { get; set; }
        //public string strOwnershipStatus { get; set; }
        //public string strOwnedByChurchBody { get; set; }
        //public string strChurchLevel_OwnedByCB { get; set; }


        // CPRD 
        public string modNVPSubCode_AP { get; set; }
        public decimal? numPeriodInterval_AP { get; set; }  // 356
        public decimal? numPeriodIntervalDays_AP { get; set; }
        public string strIntervalFrequency_AP { get; set; }   // Yr, Mo, Wk, Da
        public string strPeriodStartDate_AP { get; set; }
        public DateTime? PeriodStartDate_AP { get; set; } // Accounting Period,    

        public string modSharingStatus_AP { get; set; }
        public int? modOwnedByChurchBodyId_AP { get; set; }
        public ChurchBody modOwnedByChurchBody_AP { get; set; }

        ///
        public string modNVPSubCode_CP { get; set; }
        public decimal? numPeriodInterval_CP { get; set; }  // 356      
        public decimal? numPeriodIntervalDays_CP { get; set; }
        public string strIntervalFrequency_CP { get; set; }   // Yr, Mo, Wk, Da
        public string strPeriodStartDate_CP { get; set; }
        public DateTime? PeriodStartDate_CP { get; set; } // church Calendar Period
        public int? numOrderIndex { get; set; } // church Calendar Period


        public string modSharingStatus_CP { get; set; }
        public int? modOwnedByChurchBodyId_CP { get; set; }
        public ChurchBody modOwnedByChurchBody_CP { get; set; }

        public bool bl_IsChuPeriodSameAsAccPeriod { get; set; }

        // CT
        //public int numPeriodInterval { get; set; }
        //public string strPeriodType { get; set; }
        //public DateTime strPeriodStartDate { get; set; }


        public string strSharingStatus_AP { get; set; }
        public string strOwnershipCode_AP { get; set; }
        public string strOwnershipStatus_AP { get; set; }
        public string strOwnedByChurchBody_AP { get; set; }
        public string strChurchLevel_OwnedByCB_AP { get; set; }

        public string strSharingStatus_CP { get; set; }
        public string strOwnershipCode_CP { get; set; }
        public string strOwnershipStatus_CP { get; set; }
        public string strOwnedByChurchBody_CP { get; set; }
        public string strChurchLevel_OwnedByCB_CP { get; set; }


        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpPeriodTypes { set; get; }
        public List<SelectListItem> lkpIntervalFrequencies { set; get; }
        public List<SelectListItem> lkpIntervals { set; get; }
        public List<SelectListItem> lkpCPR_Years { set; get; }
        public List<SelectListItem> lkpCPR_Semesters { set; get; }
        public List<SelectListItem> lkpCPR_Quarters { set; get; }
        public List<SelectListItem> lkpCPR_Months { set; get; }
        public List<SelectListItem> lkpCPR_Weeks { set; get; }
        public List<SelectListItem> lkpCPR_Days { set; get; }

    }

    public class MemberCustomCodeFormatModel : SetupEntitiesModel
    {
        public MemberCustomCodeFormatModel() { }
        //
        public List<MemberCustomCodeFormatModel> lsMemberCustomCodeFormatModels { get; set; }
        public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }
        //public List<ChurchTransferSettings> lsChurchTransferSettingss { get; set; } 
        public AppUtilityNVP oAppUtilityNVP { get; set; }

        ///  
        //public int? modAppGlobalOwnerId { get; set; }
        //public int? modChurchBodyId { get; set; }

        public int? modOwnedByChurchBodyId { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(30)]
        public string modNVPCode { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(1)]
        public string modSharingStatus { get; set; }

        public ChurchBody modOwnedByChurchBody { get; set; }
        ///  
        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }

        // for the table
        public string strSettingDesc { get; set; }
        public string strSettingValue { get; set; }

        //
        public string strPrefix { get; set; }
        public string strPrefixDelim { get; set; }
        public string strSuffix { get; set; }
        public string strSuffixDelim { get; set; }
        public string strSampleTextLDZR { get; set; }
        public bool bl_IsAutogenMCCFCode { get; set; }
        public bool bl_IsLeadingZeros { get; set; } // 0123 or 123, 0012 or 12, 0001 or 1

        //
        public string strSharingStatus { get; set; }
        public string strOwnershipCode { get; set; }
        public string strOwnershipStatus { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }



        public List<SelectListItem> lkpSharingStatuses { set; get; }

    }

    public class ChurchTransferSettingModel : SetupEntitiesModel
    {
        public ChurchTransferSettingModel() { }
        //
        public List<ChurchTransferSettingModel> lsChurchTransferSettingModels { get; set; }
        public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }
        public AppUtilityNVP oAppUtilityNVP { get; set; }

        ///  
        //public int? modAppGlobalOwnerId { get; set; }
        //public int? modChurchBodyId { get; set; }


        // [System.ComponentModel.DataAnnotations.StringLength(10)]
        public string modNVPCode { get; set; }
        public string modNVPSubCode { get; set; }

        //  [System.ComponentModel.DataAnnotations.StringLength(1)]
        //public string modSharingStatus { get; set; }
        //public int? modOwnedByChurchBodyId { get; set; }
        //public ChurchBody modOwnedByChurchBody { get; set; }
        ///  
        public string strAppGloOwn { get; set; }
        public string strChurchBody { get; set; }

        //// for the table
        //public string strSettingDesc { get; set; }
        //public string strSettingValue { get; set; }

        //// MCCF
        //public string strPrefix { get; set; }
        //public string strPrefixDelim { get; set; }
        //public string strSuffix { get; set; }
        //public string strSuffixDelim { get; set; }
        //public string strSampleTextLDZR { get; set; }
        //public bool bl_IsAutogenMCCFCode { get; set; }
        //public bool bl_IsLeadingZeros { get; set; } // 0123 or 123, 0012 or 12, 0001 or 1

        //

        //public string modSharingStatus { get; set; }
        //public int? modOwnedByChurchBodyId { get; set; }
        //public ChurchBody modOwnedByChurchBody { get; set; }

        //public string strSharingStatus { get; set; }
        //public string strOwnershipCode { get; set; }
        //public string strOwnershipStatus { get; set; }
        //public string strOwnedByChurchBody { get; set; }
        //public string strChurchLevel_OwnedByCB { get; set; }


        //// CPRD 
        //public string modNVPSubCode_AP { get; set; }
        //public decimal? numPeriodInterval_AP { get; set; }  // 356
        //public decimal? numPeriodIntervalDays_AP { get; set; }
        //public string strPeriodType_AP { get; set; }   // Yr, Mo, Wk, Da
        //public string strPeriodStartDate_AP { get; set; }
        //public DateTime? PeriodStartDate_AP { get; set; } // Accounting Period,    

        //public string modSharingStatus_AP { get; set; }
        //public int? modOwnedByChurchBodyId_AP { get; set; }
        //public ChurchBody modOwnedByChurchBody_AP { get; set; }

        /////
        //public string modNVPSubCode_CP { get; set; }
        //public decimal? numPeriodInterval_CP { get; set; }  // 356      
        //public decimal? numPeriodIntervalDays_CP { get; set; }
        //public string strPeriodType_CP { get; set; }   // Yr, Mo, Wk, Da
        //public string strPeriodStartDate_CP { get; set; }
        //public DateTime? PeriodStartDate_CP { get; set; } // church Calendar Period


        //public string modSharingStatus_CP { get; set; }
        //public int? modOwnedByChurchBodyId_CP { get; set; }
        //public ChurchBody modOwnedByChurchBody_CP { get; set; }

        //public bool bl_IsChuPeriodSameAsAccPeriod { get; set; }

        //// CT
        ////public int numPeriodInterval { get; set; }
        ////public string strPeriodType { get; set; }
        ////public DateTime strPeriodStartDate { get; set; }


        //public string strSharingStatus_AP { get; set; }
        //public string strOwnershipCode_AP { get; set; }
        //public string strOwnershipStatus_AP { get; set; }
        //public string strOwnedByChurchBody_AP { get; set; }
        //public string strChurchLevel_OwnedByCB_AP { get; set; }

        //public string strSharingStatus_CP { get; set; }
        //public string strOwnershipCode_CP { get; set; }
        //public string strOwnershipStatus_CP { get; set; }
        //public string strOwnedByChurchBody_CP { get; set; }
        //public string strChurchLevel_OwnedByCB_CP { get; set; }


        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpPeriodTypes { set; get; }
        public List<SelectListItem> lkpIntervals { set; get; }
        public List<SelectListItem> lkpCPR_Years { set; get; }
        public List<SelectListItem> lkpCPR_Semesters { set; get; }
        public List<SelectListItem> lkpCPR_Quarters { set; get; }
        public List<SelectListItem> lkpCPR_Months { set; get; }
        public List<SelectListItem> lkpCPR_Weeks { set; get; }
        public List<SelectListItem> lkpCPR_Days { set; get; }

    }

}



