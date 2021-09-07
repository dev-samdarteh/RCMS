using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public class ChurchTransferModel
    {
        public ChurchTransferModel() { }

        public int? oAppGloOwnId { get; set; }
        public AppGlobalOwner oAppGloOwn { get; set; }
        public ChurchBody oChurchBody { get; set; }  // grace 
        public int? oAppGloOwnId_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }

        //public ChurchMember oCurrLoggedMember { get; set; } 
        public int? oChurchBodyId { get; set; }

        public UserProfile oUserLogged { get; set; }

        public int setIndex { get; set; }
        //public int? oMemberId_Logged { get; set; }
        //public int? oUserId_Logged { get; set; }
        public string strChurchBody { get; set; }
        public string oUserRole_Logged { get; set; }


        public ChurchBody oRequestorChurchBody { get; set; }
        //public ChurchTransfer oMemberTransfer { get; set; }
        //public ChurchTransfer oClergyTransfer { get; set; }
        //public ChurchTransfer oRoleTransfer { get; set; }

        public int? oCurrApprovalProcessStepId { get; set; }
        public ApprovalProcessStep oCurrApprovalProcessStep { get; set; }

        public int? oCurrApproverChurchBodyId { get; set; }
        public int? oCurrApproverMemberChurchRoleId { get; set; }
        public int? oCurrApproverChurchRoleId { get; set; }
        public int? oCurrApproverChurchMemberId { get; set; }  // Approver1 or Approver2
        public ChurchMember oCurrApproverChurchMember { get; set; }

        public string strCurrApproverChurchBody { get; set; }
        public string strCurrApproverChurchMember { get; set; }
        public string strCurrApproverChurchRole { get; set; }

        public ApprovalActionStepModel oCurrApprovalActionStepModel { get; set; }
        public List<ApprovalActionStepModel> lsCurrApprActionStepModels { get; set; }
         
        public ApprovalActionStep oCurrApprovalActionStep { get; set; }
        public List<ApprovalActionStep> lsCurrApprActionSteps { get; set; }

        public int? oCurrApprovalActionStepId { get; set; }      
        public int? oCurrApprovalActionId { get; set; }
        public ApprovalAction oCurrApprovalAction { get; set; }
        public bool IsCurrentActionStepToApprove { get; set; }
        //  public string strApprovalStepStatus { get; set; }
        // public string strApprovalActionStatus { get; set; }
        public string strApproverComment { get; set; }


        public int? oChurchMemberId { get; set; }
        public string strEventLongevity { get; set; }  //H-istory, T-oday or C-urrent

        public string strAppName { get; set; }
        public string strAppNameMod { get; set; }
        public string strAppCurrUser { get; set; }
        public string strCurrTask { get; set; }

        // public int? oAppGlolOwnId_Logged { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public string strConfirmUserCode { get; set; }
        public bool isUserRoleAdmin_Logged { get; set; }


        public ChurchTransfer oChurchTransfer { get; set; }
        public List<ChurchTransfer> lsChurchTransfers { get; set; }
        public List<ChurchTransferModel> lsChurchTransferModels_OT { get; set; }
        public List<ChurchTransferModel> lsChurchTransferModels_IN { get; set; }



        public string strRequestorFullName { get; set; }
        public string strRequestorRole { get; set; }
        public int? numRequestorMemberId { get; set; }
        public int? numRequestorMemberLeaderRoleId { get; set; }

        public string strTransfMemberDesc { get; set; }
        public string strRequestDateDesc { get; set; }
        public string strTransferDateDesc { get; set; }
        public string strTransferType { get; set; }  // MT, RT, CT
        public string strTransferSubType { get; set; }


        // public string strRequestorCongregation { get; set; }
        public string strRequestorChurchBody { get; set; }
        public int? numRequestorChurchBodyId { get; set; }
        public string strRequestorChurchLevel { get; set; }


        //public string strMemTypeCode_FrCB { get; set; }   /// ... affirm from To-CB what's coming from From-CB
        //public string strMemTypeCode_ToCB { get; set; }   /// ... affirm from To-CB what's coming from From-CB
        //public int? numMemRankId { get; set; }/// ... affirm from To-CB what's coming from From-CB
        //public int? numMemStatusId_FrCB { get; set; }/// ... affirm from To-CB what's coming from From-CB
        //public int? numMemStatusId_ToCB { get; set; }/// ... affirm from To-CB what's coming from From-CB

        public string strFromChurchLevel { get; set; }
        public int? numFromChurchBodyId { get; set; }
        public int? numToChurchBodyId { get; set; }
        public string strFromChurchBody { get; set; }
        public string strFromChurchBodyDesc { get; set; }
        public string strFromMemberFullName { get; set; }
        ///
        public string strFromMemberStatus { get; set; }   // regular, distant, deactive etc  
        public string strFromMemberRank { get; set; }   // regular, distant, deactive etc  
        public string strFromMemberType { get; set; }   // regular, distant, deactive etc  
        ///
        public int? numFromMemberStatus { get; set; }   // regular, distant, deactive etc  
        public int? numFromMemberRank { get; set; }   // regular, distant, deactive etc  
        public string numFromMemberType { get; set; }   // regular, distant, deactive etc  
                  

        public string strTempMemStatusIdFrCB { get; set; }   // regular, distant, deactive etc  
        public string strTempMemRankIdFrCB { get; set; }   // elder, minister
        public string strTempMemTypeCodeFrCB { get; set; }   // guest, affiliated, ...member
        ///
        public string strTempMemStatusIdToCB { get; set; }   // regular, distant, deactive etc 
        public string strTempMemRankIdToCB { get; set; }   // elder, minister
        public string strTempMemTypeCodeToCB { get; set; }   // guest, affiliated, ...member
        ///
        public int? numTempMemStatusIdFrCB { get; set; }
        public int? numTempMemRankIdFrCB { get; set; }
        public string numTempMemTypeCodeFrCB { get; set; }
        ///
        public int? numTempMemStatusIdToCB { get; set; }
        public int? numTempMemRankIdToCB { get; set; }
        public string numTempMemTypeCodeToCB { get; set; }
         

        //  public string strFromMemberPhotoUrl { get; set; }
        public string strFromMemberPos { get; set; }
        public int? numFromChurchPositionId { get; set; }  // Rank...
        public int? numFromMemberLeaderRoleId { get; set; }
        public string strFromMemberAgeGroup { get; set; }
        public string strFromMemberRole { get; set; }
        public string strFromMemberLongevityDesc { get; set; }
        public string strFromCongLongevityDesc { get; set; }
        public string strFromMemberPhotoUrl { get; set; }

        public string strToChurchLevel { get; set; }
        public int? numToChurchLevelId { get; set; }
        public string strToChurchBody { get; set; }
        public string strToChurchBodyDesc { get; set; }

        public IList<string> strAffliliateChurchBodies { get; set; }
        public IList<ChurchBody> oAffliliateChurchBodies { get; set; }
        public string strApprovalActionStatus { get; set; }

        public string strCurrScope { get; set; }
        public string strReqStatus { get; set; }
        public string strReqStatusDetail { get; set; }
        public string strWorkSpanStatus { get; set; }
        //public string strAckStatus { get; set; }
        //public string strReqApprovalStatus { get; set; }

        public string strActionStatus { get; set; }
        public string strActionStatusCode { get; set; }
        public string strActionStepStatus { get; set; }
        public string strActionStepStatusCode { get; set; }

       // public string strMembershipStatus { get; set; }
        //public string strMovementStatus { get; set; }
        public string strReqStatusComments { get; set; }
        public string strApprovalStatus { get; set; }
        public string strApprovalStatusDetail { get; set; }
        public string strApprovalStatusComments { get; set; }
        public bool blRequireApproval { get; set; }
        public string strApprovers { get; set; }
        public string strTransMessage { get; set; }
        public string strReason { get; set; }
        public string strComments { get; set; }
        //public string strToMemberPos { get; set; }
        public string strToMemberRole { get; set; }
        public string strToRoleDept { get; set; }
        public string strFromRoleLongevityDesc { get; set; }
        public string strFromClergyLongevityDesc { get; set; }



        public int serviceTask { get; set; }  //church worker = 1, approver = 2, member self-service = 3
        public int numTransferDxn { get; set; }  // incoming = 1, outgoing = 2, 0 == both
        public string strTransferDxn { get; set; }   //OUTgoing or INcoming 
        public int userRequestTask { get; set; }
        public string strCurrUserTask { get; set; }
        public string strSelectCong { get; set; }


        public string strTransfMemberCongDesc { get; set; }
        public string strActionRequestDateDesc { get; set; }
        public string strStepRequestDateDesc { get; set; }
        public string strStepActionDateDesc { get; set; }
        public string strApproverDesc { get; set; }
        // public string strApproverRole { get; set; }


        public ApprovalActionStep oApprovalXActionStep { get; set; }
        public ApprovalAction oApprovalXAction { get; set; }
      //  public List<ApprovalActionStep> lsApprActionSteps { get; set; }

        public string strApprovalXChurchBody { get; set; }
        public string strApproverXChurchBody { get; set; }
        public string strApproverXChurchMember { get; set; }
        public string strApproverXChurchRole { get; set; }
        public string strApprovalXDate { get; set; }
        public string strApprovalXReqDate { get; set; }
        public string strApprovalXStepStatus { get; set; }





        //lookups

        public List<SelectListItem> lkp_ChurchMembers { set; get; }
        public List<SelectListItem> lkp_LeaderRoles { set; get; }
        public List<SelectListItem> lkp_SectorCategories { set; get; }  //groups, cttees, etc.
        public List<SelectListItem> lkp_RoleDepartments { set; get; }

        public List<SelectListItem> lkp_FromCongregations { set; get; }   // of same denomination
        public List<SelectListItem> lkp_CongNextCategory { set; get; }
        public List<SelectListItem> lkp_ToCongregations { set; get; }  // of same denomination except curr cong
        public List<SelectListItem> lkp_TransferTypes { set; get; }
        public List<SelectListItem> lkp_TransMessages { set; get; }
        //public List<SelectListItem> lkp_TransReasons { set; get; }
        //public List<SelectListItem> lkp_FilterMemberList { set; get; }

        public List<SelectListItem> lkpChurchMembers { set; get; }
        public List<SelectListItem> lkpSharingStatus { set; get; }
        public List<SelectListItem> lkpStatuses { set; get; }

        public List<SelectListItem> lkpChurchMemStatuses_IN { set; get; }  // status must be available
        public List<SelectListItem> lkpChurchMemStatuses_OT { set; get; }  // status must NOT be available but not deceased either
        public List<SelectListItem> lkpChurchMemTypes { set; get; }
        public List<SelectListItem> lkpChurchRanks { set; get; }




    }

    public class ApprovalActionStepModel
    {
        public ApprovalActionStepModel() { }

        public int? oAppGloOwnId { get; set; }
        public AppGlobalOwner oAppGloOwn { get; set; }
        public ChurchBody oChurchBody { get; set; }  // grace 
        public int? oAppGloOwnId_Logged { get; set; }
        public ChurchBody oChurchBody_Logged { get; set; }

        //public ChurchMember oCurrLoggedMember { get; set; } 
        public int? oChurchBodyId { get; set; }

        public UserProfile oUserLogged { get; set; }

        public int setIndex { get; set; }
        //public int? oMemberId_Logged { get; set; }
        //public int? oUserId_Logged { get; set; }
        public string strChurchBody { get; set; }
        public string oUserRole_Logged { get; set; }

        // public ApprovalActionStepModel oCurrApprovalActionStepModel { get; set; }
        public List<ApprovalActionStepModel> lsApprActionStepModels { get; set; }
        public ApprovalActionStep oApprovalActionStep { get; set; }
        public ApprovalAction oApprovalAction { get; set; }
        public List<ApprovalActionStep> lsApprActionSteps { get; set; }

        public string strApproverChurchBody { get; set; }
        public string strApproverChurchMember { get; set; }
        public string strApproverChurchRole { get; set; }
        public string strApprovalDate { get; set; }
        public string strApprovalReqDate { get; set; }
        public string strApprovalStepStatus { get; set; }
        public string strApprovalActionStatus { get; set; }


    }
}