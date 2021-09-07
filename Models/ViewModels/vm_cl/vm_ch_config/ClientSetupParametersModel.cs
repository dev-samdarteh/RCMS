using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using System.Collections.Generic; 

namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public class ClientSetupParametersModel
    {
        public ClientSetupParametersModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        //  public int? oChurchBodyId { get; set; }
        //  public AppGlobalOwner oAppGlobalOwn { get; set; }
        //  public ChurchLevel oChurchLevel { get; set; }
        //  public ChurchBody oChurchBody { get; set; }  // grace

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

        //
        public List<AppGlobalOwnerModel> lsAppGlobalOwnModels { get; set; }
        public AppGlobalOwnerModel oAppGlobalOwnModel { get; set; }
        public List<ChurchBodyModel> lsChurchBodyModels { get; set; }
        public ChurchBodyModel oChurchBodyModel { get; set; }
        public List<ChurchBodyModel> lsChurchBodyAdhocModels { get; set; }
        public ChurchBodyModel oChurchBodyAdhocModel { get; set; }
        public List<ChurchLevelModel> lsChurchLevelModels { get; set; }
        public ChurchLevelModel oChurchLevel { get; set; } 
        
        public IList<CBNetworkModel> lsCBNetworkModels { get; set; }
        public CBNetworkModel oCBNetworkModel { get; set; }

        public List<AppUtilityNVPModel> lsAppUtilityNVPModels { get; set; }
        public AppUtilityNVPModel oAppUtilityNVPModel { get; set; }
        public List<CountryModel> lsCountryModels { get; set; }
        public CountryModel oCountryModel { get; set; }
        public List<CountryCustomModel> lsCountryCustomModels { get; set; }
        public CountryCustomModel oCountryCustomModel { get; set; }
        public List<CurrencyCustomModel> lsCurrencyCustomModels { get; set; }
        public CurrencyCustomModel oCurrencyCustomModel { get; set; }
        public List<CountryRegionModel> lsCountryRegionModels { get; set; }
        public CountryRegionModel oCountryRegionModel { get; set; }
        public List<CountryRegionCustomModel> lsCountryRegionCustomModels { get; set; }
        public CountryRegionCustomModel oCountryRegionCustomModel { get; set; }
        public List<LanguageSpokenModel> lsLanguageSpokenModels { get; set; }
        public LanguageSpokenModel oLanguageSpokenModel { get; set; }
        public List<ChurchPeriodModel> lsChurchPeriodModels { get; set; }
        public ChurchPeriodModel oChurchPeriodModel { get; set; }
        public List<ChurchPeriodModel> lsChurchPeriodModels_CY { get; set; }
        public ChurchPeriodModel oChurchPeriodModel_CY { get; set; } 
        public List<ChurchPeriodModel> lsChurchPeriodModels_AY { get; set; }
        public ChurchPeriodModel oChurchPeriodModel_AY { get; set; }
        public List<National_IdTypeModel> lsNational_IdTypeModels { get; set; }
        public National_IdTypeModel oNational_IdTypeModel { get; set; }
        public List<InstitutionTypeModel> lsInstitutionTypeModels { get; set; }
        public InstitutionTypeModel oInstitutionTypeModel { get; set; }
        public List<CertificateTypeModel> lsCertificateTypeModels { get; set; }
        public CertificateTypeModel oCertificateTypeModel { get; set; }
        public List<RelationshipTypeModel> lsRelationshipTypeModels { get; set; }
        public RelationshipTypeModel oRelationshipTypeModel { get; set; }
        public List<ChurchUnitModel> lsChurchUnitModels { get; set; }
        public ChurchUnitModel oChurchUnitModel { get; set; }
        public List<ChurchRankModel> lsChurchRankModels { get; set; }
        public ChurchRankModel oChurchRankModel { get; set; }
        public List<ChurchRoleModel> lsChurchRoleModels { get; set; }
        public ChurchRoleModel oChurchRoleModel { get; set; }
        public List<ChurchlifeActivityModel> lsChurchlifeActivityModels { get; set; }
        public ChurchlifeActivityModel oChurchlifeActivityModel { get; set; }
        public List<ChurchlifeActivityReqDefModel> lsChurchlifeActivityReqDefModels { get; set; }
        public ChurchlifeActivityReqDefModel oChurchlifeActivityReqDefModel { get; set; }
        public List<ChurchMemTypeModel> lsChurchMemTypeModels { get; set; }
        public ChurchMemTypeModel oChurchMemTypeModel { get; set; }
        public List<ChurchMemStatusModel> lsChurchMemStatusModels { get; set; }
        public ChurchMemStatusModel oChurchMemStatusModel { get; set; }
        public List<ChurchTransferSettingsModel> lsChurchTransferSettingsModels { get; set; }
        public ChurchTransferSettingsModel oChurchTransferSettingsModel { get; set; }
        public List<MemberCustomCodeFormatModel> lsMemberCustomCodeFormatModels { get; set; }
        public AdhocParameterModel oAdhocParameterModel { get; set; }
        public MemberCustomCodeFormatModel oMemberCustomCodeFormatModel { get; set; }

        //public ChurchLevel oChurchLevel { get; set; }

        //public int numChurchLevel { get; set; }
        //public string strChurchLevel { get; set; }
        //public string strAppGloOwn { get; set; }
        //// public string strStatus { get; set; }

        //public List<SelectListItem> lkpAppGlobalOwns { set; get; }
        //public List<SelectListItem> lkpStatuses { set; get; }

    }
}
