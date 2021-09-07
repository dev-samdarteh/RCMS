using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public class ChurchAttendanceModel
    {

        public ChurchAttendanceModel() { }

        public AppGlobalOwner oAppGlobalOwner { get; set; }
        public ChurchBody oChurchBody { get; set; }  //RequestCongregation
        public int? oAppGloOwnId { get; set; }
        public int? oChurchBodyId { get; set; }
        public ChurchBody oLoggedChurchBody { get; set; }
        public ChurchMember oChurchMember { get; set; }
        public int? oChurchMemberId { get; set; }
        public int? oAttendRefId { get; set; }
        public int? f_oAttendRefId { get; set; }

        public int? oAppGloOwnId_Logged { get; set; }
        public int? oAppGloOwnId_Logged_MSTR { get; set; }
        public int? oChurchBodyId_Logged { get; set; }
        public int? oChurchBodyId_Logged_MSTR { get; set; }
        public int? oCurrMemberId_Logged { get; set; } 
        public int? oMemberId_Logged { get; set; }
        public int? oUserId_Logged { get; set; }
        public string oUserRole_Logged { get; set; }

        public bool isUserRoleAdmin_Logged { get; set; }


        public ChurchAttendAttendee oChurchAttendee { get; set; }
        public ChurchAttendHeadCount oChurchAttend_HC { get; set; }

        // public ChurchAttend_Attendees oAttend { get; set; }
        public int? oAttend_Id { get; set; }

        public int currAttendTask { get; set; }
        public int currAttnVw { get; set; }
        public string strCurrTaskDesc { get; set; }
        public string strDescChkinRaw { get; set; }


        //list
        public List<ChurchBody> lsCurrSubChurchUnits { get; set; } /// CurrSubChurchUnits { get; set; }
        public List<ChurchAttendAttendee> lsChurchAttendeesList { get; set; }  /// oChurchAttendeesList { get; set; }
        //  public List<ChurchAttendanceModel> oChurchAttendanceModelList { get; set; }
        public List<ChurchCalendarModel> lsChurchCalendarModels_Attend { get; set; }
        public List<ChurchAttendanceModel> lsChurchAttendanceModels_Hdr { get; set; }
        public List<ChurchAttendanceModel> lsChurchAttendanceModels_VisHdr { get; set; }
        public List<MemberBioModel> lsChurchMemberModel_MemHdr { get; set; }
        public List<ChurchAttendanceModel> lsChurchAttendanceModels { get; set; }   /// oChurchAttendees_VMList   
        public List<ChurchAttendanceModel> lsChurchAttendanceModels_MultiEdit { get; set; }    ///  oChurchAttendeesEdit_VMList

        public List<ChurchAttendanceModel> lsChurchAttendanceModels_HC { get; set; }  /// oCongHeadcount_VMList { get; set; }
        public List<ChurchAttendanceModel> lsChurchAttendanceModels_HCSumm { get; set; }   /// oCongHeadcount_VMList_Summ { get; set; }
        public List<ChurchAttendanceModel> lsChurchAttendanceModels_HCEdit { get; set; }  /// oCongHeadcountEdit_VMList { get; set; }

        public List<ChurchAttendanceModel> lsChurchAttendanceModels_AttendFilter { get; set; } /// oFilter_ChurchAttendees_VMList { get; set; }

        public string strAttendeeName { get; set; }
        public string strAttendeeNameExtnd { get; set; }
        public string strMemGeneralStatus { get; set; }
        public string strGender { get; set; }
        public string strGenderDesc { get; set; }
        public string strPhotoUrl { get; set; }
        public string strAgeGroup { get; set; }
        public string strCountCategory { get; set; }
        public int? CountCategoryId { get; set; }
        public int? MinAge { get; set; }
        public DateTime? dtEventFrom { get; set; }
        public DateTime? dtEventTo { get; set; }
        public string strDateAttended { get; set; }
        public string strTempRec { get; set; }
        public decimal? decTempRec { get; set; }

        public string strPersWt { get; set; }
        public decimal? decPersWt { get; set; }

        public string strPersBPMin { get; set; }
        public decimal? decPersBPMin { get; set; }

        public string strPersBPMax { get; set; }
        public decimal? decPersBPMax { get; set; }

        public string strChurchBody { get; set; }
        public string strChurchEventDesc_Hdr { get; set; }
        public string strChurchEventDesc { get; set; }
        public string strCountBatchNo { get; set; }
        public string strPhone { get; set; }
        public string strResidenceLoc { get; set; }
        public string strNationality { get; set; }
        public string strAttendeeTypeDesc { get; set; }
        public string strCountTypeDesc { get; set; }

        // public string strChurchEvent { get; set; }
        public string strCountDate { get; set; }

        public string strCongregation { get; set; }
        public string strAttnLongevity { get; set; }  //H-istory, T-oday or C-urrent
        public string f_strAttendeeTypeCode { get; set; }  // V-isitor, C-congregant (member)


        public long CHCF_TotCong { get; set; }
        public long CHCF_TotAttend_MemOrVis { get; set; }
        //  public long CHCF_TotAttend_Today { get; set; }

        public long CHCF_TotAttend_M { get; set; }
        public long CHCF_TotAttend_F { get; set; }
        public long CHCF_TotAttend_O { get; set; }

        public long CHCF_TotAttend_C { get; set; }
        public long CHCF_TotAttend_Y { get; set; }
        public long CHCF_TotAttend_YA { get; set; }
        public long CHCF_TotAttend_MA { get; set; }
        public long CHCF_TotAttend_AA { get; set; }


        // public long CHCF_TotVis { get; set; } 
        public int? f_ChurchEventDetailId { get; set; } 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? f_DateAttended { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? f_DateAttendedMin { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? f_DateAttendedMax { get; set; }
        public bool f_ChkMemAttend { get; set; }
        public int? oEventCLId { get; set; }
        public int? f_oEventCLId { get; set; }
        public int? oEventCBId { get; set; }
        public int? f_oEventCBId { get; set; }

        //to add new data
        public int? numCountGroupId { get; set; }
        public long numCHCF_TotAttend_M { get; set; }
        public long numCHCF_TotAttend_F { get; set; }
        public long numCHCF_TotAttend_O { get; set; }

        public long numCHCF_TotAttend_C { get; set; }
        public long numCHCF_TotAttend_Y { get; set; }
        public long numCHCF_TotAttend_YA { get; set; }
        public long numCHCF_TotAttend_MA { get; set; }
        public long numCHCF_TotAttend_AA { get; set; }

        public long num_f_CHCF_TotAttend_Yr { get; set; }
        public long num_f_CHCF_TotAttend_Sem { get; set; }
        public long num_f_CHCF_TotAttend_Qtr { get; set; }
        public long num_f_CHCF_TotAttend_Mon { get; set; }
        public long num_f_CHCF_TotAttend_Wk { get; set; }
        public long num_f_CHCF_TotAttend_Today { get; set; }

        public long m_CHCF_TotVis { get; set; }

        //head count
        public string strCountTypeCode { get; set; }  // A-ll cong, G-roups


        [Display(Name = "Church Event")]
        public int? m_ChurchEventId_HC { get; set; }

        [Display(Name = "Date Attended")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? m_DateAttended_HC { get; set; }

        //public bool mc_IsCongregation_Summ { get; set; }
        //public bool mc_IsGenChurchGroup_Summ { get; set; }
        //public bool mc_IsInterGenChurchGroup_Summ { get; set; }

          
        [Display(Name = "Filter From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MMM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime dtFilter_AttendFrom { get; set; } 

        [Display(Name = "Filter To")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MMM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime dtFilter_AttendTo { get; set; }
         
        [Display(Name = "Filter by Attendee's name")]
        public string strFilter_AttendeeName { get; set; } 

        [Display(Name = "Filter by church event")]
        public string strFilter_ChurchEventDesc { get; set; } 

        [Display(Name = "Filter by Attendee's phone")]
        public string strFilter_AttendeePhone { get; set; }
        
        public bool blFilter_AttendeeTypeMem { get; set; }
        
        public bool blFilter_AttendeeTypeVis { get; set; }
         
        //[Display(Name = "Filter From")]
        //[DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:ddd d-MMM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime dtEventFilterDate1 { get; set; }
         
        //[Display(Name = "Filter To")]
        //[DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:ddd d-MMM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime dtEventFilterDate2 { get; set; }
         
        public string strEventSearchString { get; set; }
















        //lookups
        // public List<SelectListItem> lkpChurchEvents { set; get; }
        public List<SelectListItem> lkpAttendeeTypes { set; get; }
        public List<SelectListItem> lkpChuCalEvents { set; get; }
        public List<SelectListItem> lkpChurchLevels { set; get; }
        public List<SelectListItem> lkpEnrollModes { set; get; }
        public List<SelectListItem> lkpCountries { set; get; }
        public List<SelectListItem> lkpChurchMembers_Local { set; get; }
        public List<SelectListItem> lkpVisitorReasons { set; get; }
        public List<SelectListItem> lkpVisitorAgeBracket { set; get; }
        public List<SelectListItem> lkpVis_Statuses { set; get; }
        public List<SelectListItem> lkpMaritalStatuses { set; get; }
        public List<SelectListItem> lkpGenderTypes { set; get; }
        public List<SelectListItem> lkpAgeGroupTypes { set; get; }
        public List<SelectListItem> lkpPersTitles { set; get; }
        public List<SelectListItem> lkpCountGroups { set; get; }
        public List<SelectListItem> lkpChurchRanks  { set; get; }
        public List<SelectListItem> lkpChurchMemStatuses { set; get; }

    }
}
