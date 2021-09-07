using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public class ChurchBodyModel
    {

        public ChurchBodyModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public AppGlobalOwner oAppGlobalOwn { get; set; }
        // public ChurchLevel oChurchLevel { get; set; }
        // public ChurchBody oChurchBody { get; set; }  // grace
        //
        //public int? oAppGloOwnId_MSTR { get; set; }
        //public int? oChurchBodyId_MSTR { get; set; }
        //public int? oParentChurchBodyId_MSTR { get; set; }
        //public int? oChurchLevelId_MSTR { get; set; }
        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        public int? oCurrUserId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }

        [Display(Name = "Church logo")]
        public IFormFile ChurchLogoFile { get; set; }

        public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }
        // public ChurchMember oCurrLoggedMember { get; set; }
        public MSTRModels.UserProfile oChurchAdminProfile { get; set; }
        public int setIndex { get; set; }
        public int subSetIndex { get; set; }
        public int pageIndex { get; set; }

        //
        public List<ChurchBodyModel> lsChurchBodyModels { get; set; }
        public List<ChurchBody> lsChurchBodies { get; set; }
        public ChurchBody oChurchBody { get; set; }
        public ChurchBody oChurchBody_par { get; set; }
        public ChurchBody oChurchBody_sup { get; set; }
        //
        public List<ChurchBodyModel> lsSubChurchBodyModels { get; set; }
        public List<ChurchBody> lsSubChurchBodies { get; set; }
        public ChurchBody oSubChurchBody { get; set; }
        //

        public long CH_TotSubUnits { get; set; }
        public long CH_TotMem { get; set; }
        public long CH_TotNewMem { get; set; }
        public long CH_TotMaleMem { get; set; }
        public long CH_TotFemMem { get; set; }
        public long CH_TotOtherMem { get; set; }
        //

        public string strAppGlobalOwn { get; set; }
        public string strChurchBody { get; set; }
        public string strCBLevel { get; set; }
        public string strParentCBLevel { get; set; }
        public string strCBLevel_sup { get; set; }
        //  public string strOwnerChurchBody { get; set; }
        public string strParentChurchBody { get; set; }
        public int? numParentChurchBodyId { get; set; }
        public int? numSupervisedByCBId { get; set; }
        public string strSupervisedByChurchBody { get; set; }

        public List<object> arrRootChurchIds_sup { get; set; }  // top - down    
        public List<string> arrRootChurchCodes_sup { get; set; }  // top - down  
        public List<object> arrRootChurchIds { get; set; }  // top - down
        public List<string> arrRootChurchCodes { get; set; }  // top - down  
        public List<object> arrRootChurchIds_par { get; set; }  // top - down   
        public List<string> arrRootChurchCodes_par { get; set; }  // top - down          

        public string strCountry { get; set; }
        public string strCountryDesc { get; set; }
        public string strCountryRegion { get; set; }
        public string strContactDetail { get; set; }
        public string strFaithTypeCategory { get; set; }   // get the faith category stuff from AGO        
        public string strOrgType { get; set; }
        public string strChurchLevel { get; set; }
        public int numChurchLevel_Index { get; set; }
        public string strCH_InCharge { get; set; }
        public string strStatus { get; set; } // Active, Blocked, Deactive  
        public string strSharingStatus { get; set; } // All share, Child share, Not share  
        public string strChurchWorkStatus { get; set; }      //   Operationalized - O, Structure only - S 
        public string strOwnershipStatus { get; set; }   // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody 

        public string strOwnershipCode { get; set; }
        public string strOwnedByChurchBody { get; set; }
        public string strChurchLevel_OwnedByCB { get; set; }

        public string strDateFormed { get; set; }
        public string strDateInnaug { get; set; }
        public string strDateDeactive { get; set; }
        //
        public bool bl_IsActivated { get; set; }
        //public bool bl_IsFullAutonomy { get; set; }

        public string strCongLoc { get; set; }
        public string strCongLoc2 { get; set; }
        public string strChurchLogo { get; set; }
        public string strParentCB_HeaderDesc { get; set; }

        public DateTime? dtCreated { get; set; }

        public int? oCurrAppGloId_Filter5 { get; set; }
        public int? oCurrChuCategId_Filter5 { get; set; }
        public bool oCurrShowAllCong_Filter5 { get; set; }
        //         
        public int oCBLevelCount { get; set; }
        public int oCBLevelCount_sup { get; set; }
        //  public int numCLIndex { get; set; }
        public int? numSupervisedByBodyCLId { get; set; }
        public int? numParentBodyCLId { get; set; }

        public string strChurchLevel_1 { get; set; }
        public string strChurchLevel_2 { get; set; }
        public string strChurchLevel_3 { get; set; }
        public string strChurchLevel_4 { get; set; }
        public string strChurchLevel_5 { get; set; }
        public string strChurchLevel_6 { get; set; }
        public string strChurchLevel_7 { get; set; }
        public string strChurchLevel_8 { get; set; }
        public string strChurchLevel_9 { get; set; }
        public string strChurchLevel_10 { get; set; }

        public int? ChurchBodyId_1 { get; set; }
        public int? ChurchBodyId_2 { get; set; }
        public int? ChurchBodyId_3 { get; set; }
        public int? ChurchBodyId_4 { get; set; }
        public int? ChurchBodyId_5 { get; set; }
        public int? ChurchBodyId_6 { get; set; }
        public int? ChurchBodyId_7 { get; set; }
        public int? ChurchBodyId_8 { get; set; }
        public int? ChurchBodyId_9 { get; set; }
        public int? ChurchBodyId_10 { get; set; }
        //
        //  public int? oChurchBodyId_1 { set; get; }
        public string strChurchBody_1 { set; get; }
        public string strChurchBody_2 { set; get; }
        public string strChurchBody_3 { set; get; }
        public string strChurchBody_4 { set; get; }
        public string strChurchBody_5 { set; get; }
        public string strChurchBody_6 { set; get; }
        public string strChurchBody_7 { set; get; }
        public string strChurchBody_8 { set; get; }
        public string strChurchBody_9 { set; get; }
        public string strChurchBody_10 { set; get; }
        //
        public List<ChurchBody> lsChurchBody { get; set; }
        //
        public List<SelectListItem> lkp_ChurchBodies_1 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_2 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_3 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_4 { set; get; }
        public List<SelectListItem> lkp_ChurchBodies_5 { set; get; }

        public List<SelectListItem> lkpAppGlobalOwns { set; get; }
        public List<SelectListItem> lkpChurchCategories { set; get; }
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpCountryRegions { set; get; }
        public List<SelectListItem> lkpContactDetails { set; get; }
        public List<SelectListItem> lkpChurchBodies { set; get; }
        public List<SelectListItem> lkpChurchLevels { set; get; }
        public List<SelectListItem> lkpParentChurchBodies { set; get; }
        public List<SelectListItem> lkpOrgTypes { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }
        public List<SelectListItem> lkpOwnershipStatuses { set; get; }
        public List<SelectListItem> lkpSharingStatuses { set; get; }
        public List<SelectListItem> lkpChurchWorkStatuses { set; get; }

    }
     
    public class CBNetworkModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public string state { get; set; }
        public bool opened { get; set; }
        public bool disabled { get; set; }
        public bool selected { get; set; }
        public string li_attr { get; set; }
        public string a_attr { get; set; }


        public IList<CBNetworkModel> lsCBNetworkModels { get; set; }

        // public JsTreeModel oCBNetworkModel { get; set; }

        ///////
        ///
        public int? oAppGloOwnId { get; set; }
        //public int? oChurchBodyId { get; set; }
        //public AppGlobalOwner oAppGlobalOwn { get; set; }
        //public ChurchBody oChurchBody { get; set; }  // grace
        //public ChurchLevel oChurchLevel { get; set; }

        //
        public int? oAppGloOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oCurrMemberId_Logged { get; set; }
        // public int? oCurrUserId_Logged { get; set; }
        //public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        //public string oUserRole_Logged { get; set; }


        public int setIndex
        {
            get; set;

        } 
    }

     
    public class TreeViewNode
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
    }
}