using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.ViewModels.vm_cl.vm_ch_life
{
    public class ChurchAttendanceModel
    {

        public ChurchAttendanceModel() { }

        public AppGlobalOwner oAppGlobalOwner { get; set; }
        public ChurchBody oChurchBody { get; set; }  //RequestCongregation
        public int? oChurchBodyId { get; set; }
        public ChurchBody oLoggedChurchBody { get; set; }
        public ChurchMember oChurchMember { get; set; }
        public int? oChurchMemberId { get; set; }

        public ChurchAttendAttendee oChurchAttend { get; set; }
        public ChurchAttendHeadCount oChurchAttend_HC { get; set; }

        // public ChurchAttend_Attendees oAttend { get; set; }
        public int? oAttend_Id { get; set; }

        public int currAttendTask { get; set; }
        public string strCurrTaskDesc { get; set; }

        //list
        public List<ChurchBody> CurrSubChurchUnits { get; set; }
        public List<ChurchAttendAttendee> oChurchAttendeesList { get; set; }
        //  public List<ChurchAttendanceModel> oChurchAttendanceModelList { get; set; }
        public List<ChurchAttendanceModel> oChurchAttendees_VMList { get; set; }
        public List<ChurchAttendanceModel> oChurchAttendeesEdit_VMList { get; set; }

        public List<ChurchAttendanceModel> oCongHeadcount_VMList { get; set; }
        public List<ChurchAttendanceModel> oCongHeadcount_VMList_Summ { get; set; }
        public List<ChurchAttendanceModel> oCongHeadcountEdit_VMList { get; set; }

        public List<ChurchAttendanceModel> oFilter_ChurchAttendees_VMList { get; set; }

        public string strAttendeeName { get; set; }
        public string strGender { get; set; }
        public string strAgeGroup { get; set; }
        public string strCountCategory { get; set; }
        public int? CountCategoryId { get; set; }
        public int? MinAge { get; set; }
        public DateTime? dtEventFrom { get; set; }
        public DateTime? dtEventTo { get; set; }
        public string strDateAttended { get; set; }
        public string strTempRec { get; set; }
        public decimal? decTempRec { get; set; }
        public string strChurchEventDesc { get; set; }
        public string strCountBatchNo { get; set; }
        public string strPhone { get; set; }
        public string strResidenceLoc { get; set; }
        public string strNationality { get; set; }
        public string strAttendeeType { get; set; }
        public string strCountTypeDesc { get; set; }

        // public string strChurchEvent { get; set; }
        public string strCountDate { get; set; }

        public string strCongregation { get; set; }
        public string strAttnLongevity { get; set; }  //H-istory, T-oday or C-urrent
        public string strAttendeeTypeCode { get; set; }  // V-isitor, M-ember


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

        [Display(Name = "Church Event")]
        public int? m_ChurchEventId { get; set; }

        [Display(Name = "Date Attended")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? m_DateAttended { get; set; }
        public bool m_ChkMemAttend { get; set; }

        //to add new data
        public int? m_CountGroupId { get; set; }
        public long m_CHCF_TotAttend_M { get; set; }
        public long m_CHCF_TotAttend_F { get; set; }
        public long m_CHCF_TotAttend_O { get; set; }

        public long m_CHCF_TotAttend_C { get; set; }
        public long m_CHCF_TotAttend_Y { get; set; }
        public long m_CHCF_TotAttend_YA { get; set; }
        public long m_CHCF_TotAttend_MA { get; set; }
        public long m_CHCF_TotAttend_AA { get; set; }


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


        //lookups
        // public List<SelectListItem> lkpChurchEvents { set; get; }
        public List<SelectListItem> lkpAttendeeTypes { set; get; }
        public List<SelectListItem> lkpChuCalEvents { set; get; }
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


    }
}
