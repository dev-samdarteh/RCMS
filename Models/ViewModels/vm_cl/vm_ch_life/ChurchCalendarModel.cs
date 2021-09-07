using Microsoft.AspNetCore.Mvc.Rendering;
using RhemaCMS.Models.CLNTModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.ViewModels.vm_cl
{
    public class ChurchCalendarModel
    {

        public ChurchCalendarModel() { }

        public int? oAppGloOwnId { get; set; }
        public AppGlobalOwner oAppGlobalOwner { get; set; }
        public ChurchBody oChurchBody { get; set; }  //RequestCongregation        
        public int? oChurchBodyId { get; set; }
        public ChurchBody oLoggedChurchBody { get; set; }
        //public ChurchMember oChurchMember { get; set; }
        public int? oChurchMemberId { get; set; }
        public string strEventLongevity { get; set; }  //H-istory, T-oday or C-urrent

        //   public int? oActivityTypeId { get; set; }

        public ChurchCalendarEvent oChurchEvent { get; set; }
        //public ChurchTransferVM oChurchTransferVM { get; set; }


        public string strChurchBody { get; set; }
        public int currAttendTask { get; set; }
        public string strCurrTaskDesc { get; set; }  /// event name -- date

        //lists    
        public List<AppUtilityNVP> oChurchLifeActivities { get; set; }  // code = CLA
        
        //public List<ChurchLifeActivity> oChurchLifeServices { get; set; }
        public List<ChurchCalendarEvent> oCalendarEvents { get; set; }
        //public List<ChurchSectorCategory> oChurchSectorCategories { get; set; }


        public long ActivityCount_Global { get; set; }
        public long ActivityCount_Parent { get; set; }
        public long ActivityCount_Local { get; set; }
        public long ActivityCount_Uncat { get; set; }

        public string strActivityCount_Global { get; set; }
        public string strActivityCount_Parent { get; set; }
        public string strActivityCount_Local { get; set; }

        //lookups
        public List<SelectListItem> lkp_ChurchEventCategory { set; get; }
        public List<SelectListItem> lkp_ThemeColors { set; get; }
        public List<SelectListItem> lkp_ActivityType { set; get; }
        public List<SelectListItem> lkp_ChurchSectorCategory { set; get; }

        public List<SelectListItem> lkp_ChurchLifeActivity { set; get; }

        public List<SelectListItem> lkpSharingStatus { set; get; }


    }
}
