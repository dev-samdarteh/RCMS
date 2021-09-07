
using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.Adhoc
{
    public class GenUtility
    {
        public GenUtility() 
        { }

       // public ChurchBody ChurchBody { get; set; }
        //public UserProfile UserProfile { get; set; }
        //// public string logUserDesc { get; set; }
        //public string PermissionName { get; set; }
        //public bool PermissionValue { get; set; }
    }


    public class DiscreteLookup
    {
        public DiscreteLookup() { }

        public string Val { get; set; }
        public string Desc { get; set; }
        public string Category { get; set; }

        public List<DiscreteLookup> EntityStatusList { get; set; }
    }

    public class NumberDiscreteLookup
    {
        public NumberDiscreteLookup() { }

        public decimal Val { get; set; }
        public string Desc { get; set; }
        public string Category { get; set; }

        public List<NumberDiscreteLookup> EntityStatusList { get; set; }
    }


    public class TargetCBModel
    {

        public TargetCBModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        // CLIENT SIDE
        public ChurchBody oChurchBody { get; set; }  // grace
        public ChurchBody oTargetCB { get; set; }  // grace


        //MSTR SIDE
        public MSTRModels.MSTRChurchBody oChurchBody_MSTR { get; set; }  // grace
        public MSTRModels.MSTRChurchBody oTargetCB_MSTR { get; set; }  // grace

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public int? oTargetCBId { get; set; }
        public int? oTargetCLId { get; set; }
        public string strTargetCB { get; set; }
        public string strTargetCL { get; set; }
        public int numChurchLevel_Index { get; set; }

        public string strId_TCB { get; set; }   
        public string strName_TCB { get; set; }
        public string strCLId_TCB { get; set; }
        public string strCLName_TCB { get; set; }
        public string strCLTag_TCB { get; set; }

        public List<object> arrRootCBIds { get; set; }  // top - down
        public List<string> arrRootCBCodes { get; set; }  // top - down  
        public List<string> arrRootCBNames { get; set; }  // top - down  


        public int oCBLevelCount { get; set; }

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

        public List<SelectListItem> lkpChurchLevels { set; get; }



        // public AppGlobalOwner oAppGlobalOwn { get; set; }
        // public ChurchLevel oChurchLevel { get; set; }



        //
        //public int? oAppGloOwnId_MSTR { get; set; }
        //public int? oChurchBodyId_MSTR { get; set; }
        //public int? oParentChurchBodyId_MSTR { get; set; }
        //public int? oChurchLevelId_MSTR { get; set; }
        //
        //public int? oAppGloOwnId_Logged { get; set; }
        //public int? oChurchBodyId_Logged { get; set; }
        //public int? oCurrMemberId_Logged { get; set; }
        //public int? oCurrUserId_Logged { get; set; }
        //public int? oMemberId_Logged { get; set; }
        //public int? oUserId_Logged { get; set; }
        //public string oUserRole_Logged { get; set; }

        //[Display(Name = "Church logo")]
        //public IFormFile ChurchLogoFile { get; set; }

        //public AppGlobalOwner oAppGlobalOwn_Logged { get; set; }
        //public ChurchBody oChurchBody_Logged { get; set; }
        //// public ChurchMember oCurrLoggedMember { get; set; }
        //public MSTRModels.UserProfile oChurchAdminProfile { get; set; }
        //public int setIndex { get; set; }
        //public int subSetIndex { get; set; }
        //public int pageIndex { get; set; }

        ////
        //public List<ChurchBodyModel> lsChurchBodyModels { get; set; }
        //public List<ChurchBody> lsChurchBodies { get; set; }
        //public ChurchBody oChurchBody { get; set; }
        //public ChurchBody oChurchBody_par { get; set; }
        //public ChurchBody oChurchBody_sup { get; set; }
        ////
        //public List<ChurchBodyModel> lsSubChurchBodyModels { get; set; }
        //public List<ChurchBody> lsSubChurchBodies { get; set; }
        //public ChurchBody oSubChurchBody { get; set; }
        ////

        //public long CH_TotSubUnits { get; set; }
        //public long CH_TotMem { get; set; }
        //public long CH_TotNewMem { get; set; }
        //public long CH_TotMaleMem { get; set; }
        //public long CH_TotFemMem { get; set; }
        //public long CH_TotOtherMem { get; set; }
        ////




        //public string strParentCBLevel { get; set; }
        //public string strCBLevel_sup { get; set; }
        ////  public string strOwnerChurchBody { get; set; }
        //public string strParentChurchBody { get; set; }
        //public int? numParentChurchBodyId { get; set; }
        //public int? numSupervisedByCBId { get; set; }
        //public string strSupervisedByChurchBody { get; set; }

        //public List<object> arrRootChurchCBIds_sup { get; set; }  // top - down    
        //public List<string> arrRootChurchCBCodes_sup { get; set; }  // top - down  



        //public List<object> arrRootChurchCBIds_par { get; set; }  // top - down   
        //public List<string> arrRootChurchCBCodes_par { get; set; }  // top - down          

        //public string strCountry { get; set; }
        //public string strCountryDesc { get; set; }
        //public string strCountryRegion { get; set; }
        //public string strContactDetail { get; set; }
        //public string strFaithTypeCategory { get; set; }   // get the faith category stuff from AGO        
        //public string strOrgType { get; set; }
        //public string strChurchLevel { get; set; }
        //public int numChurchLevel_Index { get; set; }
        //public string strCH_InCharge { get; set; }
        //public string strStatus { get; set; } // Active, Blocked, Deactive  
        //public string strSharingStatus { get; set; } // All share, Child share, Not share  
        //public string strChurchWorkStatus { get; set; }      //   Operationalized - O, Structure only - S 
        //public string strOwnershipStatus { get; set; }   // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody 

        //public string strOwnershipCode { get; set; }
        //public string strOwnedByChurchBody { get; set; }
        //public string strChurchLevel_OwnedByCB { get; set; }

        //public string strDateFormed { get; set; }
        //public string strDateInnaug { get; set; }
        //public string strDateDeactive { get; set; }
        ////
        //public bool bl_IsActivated { get; set; }
        ////public bool bl_IsFullAutonomy { get; set; }

        //public string strCongLoc { get; set; }
        //public string strCongLoc2 { get; set; }
        //public string strChurchLogo { get; set; }
        //public string strParentCB_HeaderDesc { get; set; }

        //public DateTime? dtCreated { get; set; }

        //public int? oCurrAppGloId_Filter5 { get; set; }
        //public int? oCurrChuCategId_Filter5 { get; set; }
        //public bool oCurrShowAllCong_Filter5 { get; set; }
        //         


        //public int oCBLevelCount_sup { get; set; }
        ////  public int numCLIndex { get; set; }
        //public int? numSupervisedByBodyCLId { get; set; }
        //public int? numParentBodyCLId { get; set; }

        //public int? ChurchBodyId_1 { get; set; }
        //public int? ChurchBodyId_2 { get; set; }
        //public int? ChurchBodyId_3 { get; set; }
        //public int? ChurchBodyId_4 { get; set; }
        //public int? ChurchBodyId_5 { get; set; }
        //public int? ChurchBodyId_6 { get; set; }
        //public int? ChurchBodyId_7 { get; set; }
        //public int? ChurchBodyId_8 { get; set; }
        //public int? ChurchBodyId_9 { get; set; }
        //public int? ChurchBodyId_10 { get; set; }


        //  public int? oChurchBodyId_1 { set; get; }
        //public string strChurchBody_1 { set; get; }
        //public string strChurchBody_2 { set; get; }
        //public string strChurchBody_3 { set; get; }
        //public string strChurchBody_4 { set; get; }
        //public string strChurchBody_5 { set; get; }
        //public string strChurchBody_6 { set; get; }
        //public string strChurchBody_7 { set; get; }
        //public string strChurchBody_8 { set; get; }
        //public string strChurchBody_9 { set; get; }
        //public string strChurchBody_10 { set; get; }




        //
        // public List<ChurchBody> lsChurchBody { get; set; }
        //
        //
        //public List<SelectListItem> lkp_ChurchBodies_1 { set; get; }
        //public List<SelectListItem> lkp_ChurchBodies_2 { set; get; }
        //public List<SelectListItem> lkp_ChurchBodies_3 { set; get; }
        //public List<SelectListItem> lkp_ChurchBodies_4 { set; get; }
        //public List<SelectListItem> lkp_ChurchBodies_5 { set; get; } 
        //public List<SelectListItem> lkpAppGlobalOwns { set; get; }
        //public List<SelectListItem> lkpChurchCategories { set; get; }
        //public List<SelectListItem> lkpCountries { set; get; }
        //public List<SelectListItem> lkpCountryRegions { set; get; }
        //public List<SelectListItem> lkpContactDetails { set; get; }
        //public List<SelectListItem> lkpChurchBodies { set; get; }
        //public List<SelectListItem> lkpChurchLevels { set; get; }
        //public List<SelectListItem> lkpParentChurchBodies { set; get; }
        //public List<SelectListItem> lkpOrgTypes { set; get; }
        //public List<SelectListItem> lkpStatuses { set; get; }
        //public List<SelectListItem> lkpOwnershipStatuses { set; get; }
        //public List<SelectListItem> lkpSharingStatuses { set; get; }
        //public List<SelectListItem> lkpChurchWorkStatuses { set; get; }

    }

    public class TargetCMModel
    {

        public TargetCMModel() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        // CLIENT SIDE
        public ChurchBody oChurchBody { get; set; }  // grace
        public ChurchBody oTargetCB { get; set; }  // grace


        //MSTR SIDE
        public MSTRModels.MSTRChurchBody oChurchBody_MSTR { get; set; }  // grace
        public MSTRModels.MSTRChurchBody oTargetCB_MSTR { get; set; }  // grace

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public int? oTargetCBId { get; set; }
        public int? oTargetCLId { get; set; }
        public string strTargetCB { get; set; }
        public string strTargetCL { get; set; }
        public int numChurchLevel_Index { get; set; }

        public string strTargetFilter { get; set; }
        public int? oChurchMemberId { get; set; }
        public ChurchMember oChurchMember { get; set; }
        public List<ChurchMember> oMemberResList { get; set; }


        public string strId_TCM { get; set; }
        public string strName_TCM { get; set; }
        public string strCLId_TCM { get; set; }
        public string strCLName_TCM { get; set; }
        public string strCLTag_TCM { get; set; } 

        public List<object> arrRootCBIds { get; set; }  // top - down
        public List<string> arrRootCBCodes { get; set; }  // top - down  
        public List<string> arrRootCBNames { get; set; }  // top - down  


        public int oCBLevelCount { get; set; }

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

        public List<SelectListItem> lkpChurchLevels { set; get; }
        public List<SelectListItem> lkpChurchMembers { set; get; }


    }

    public class _TargetCBModel_MSTR
    {

        public _TargetCBModel_MSTR() { }

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }
        // 

        //// CLIENT SIDE
        //public ChurchBody oChurchBody { get; set; }  // grace
        //public ChurchBody oTargetCB { get; set; }  // grace


        //MSTR SIDE
        public MSTRModels.MSTRChurchBody oChurchBody_MSTR { get; set; }  // grace
        public MSTRModels.MSTRChurchBody oTargetCB_MSTR { get; set; }  // grace

        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public int? oTargetCBId { get; set; }
        public string strTargetCB { get; set; }
        public string strTargetCL { get; set; }
        public int numChurchLevel_Index { get; set; }

        public string strId_TCB { get; set; }
        public string strName_TCB { get; set; }
        public string strCLId_TCB { get; set; }
        public string strCLName_TCB { get; set; }

        public List<object> arrRootCBIds { get; set; }  // top - down
        public List<string> arrRootCBCodes { get; set; }  // top - down  
        public List<string> arrRootCBNames { get; set; }  // top - down  


        public int oCBLevelCount { get; set; }

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

        public List<SelectListItem> lkpChurchLevels { set; get; }
         
    }


}
