using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ChurchCalendarEvent
    {
        public ChurchCalendarEvent()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

       // public int? OwnedByChurchBodyId { get; set; }

        [StringLength(100)]
        public string Subject { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? ChurchlifeActivityId { get; set; }
        public int? ChurchEventCategoryId { get; set; } 
        public bool IsChurchServiceEvent { get; set; }  // ensure the activity picked up.. is also Church service
        public int? ChurchBodyServiceId { get; set; }   // if not.. null

        [StringLength(100)]
        public string Venue { get; set; }
        public bool IsFullDay { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy hh:mm a}", ApplyFormatInEditMode = false)]  ///dd-MMM-yy hh:mm a
        public DateTime? EventFrom { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy hh:mm a}", ApplyFormatInEditMode = false)]
        public DateTime? EventTo { get; set; }   // include time...
        
        
        [StringLength(50)]
        public string ThemeColor { get; set; }  // get the color palettes  

        public bool IsEventActive { get; set; }  // can be deactivated ... 
         
        //[StringLength(1)]
        //public string Status { get; set; }   ///  Open (O) , In Progress (I), Completed (C), 


        [StringLength(1)]
        public string EventStatus { get; set; }   //  P-Open/Pending, I-In Progress, D-Deactive, On-Hold (H), Cancelled (X), Complete (C)

        [StringLength(1)]
        public string SharingStatus { get; set; }    /// @detail level ... if not.. keep at the main
                                                     /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                     /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                     /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 



        public string PhotoUrl { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]//[ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }


        //[NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        //public virtual ChurchBody OwnedByChurchBody { get; set; }


        [ForeignKey(nameof(ChurchBodyServiceId))] 
        public virtual ChurchBodyService ChurchBodyService { get; set; }

        [ForeignKey(nameof(ChurchEventCategoryId))] 
        public virtual AppUtilityNVP ChurchEventCategory_NVP { get; set; }

        [ForeignKey(nameof(ChurchlifeActivityId))] 
        public virtual AppUtilityNVP ChurchlifeActivity_NVP { get; set; }  //ChurchlifeActivity


        //public virtual List<ChurchAttendAttendees> ChurchAttendAttendees { get; set; } 
        //public virtual List<ChurchAttendHeadCount> ChurchAttendHeadCount { get; set; } 
        //public virtual List<ChurchAttendance> ChurchAttendance { get; set; } 
        //public virtual List<ChurchEventActor> ChurchEventActor { get; set; } 
        //public virtual List<EventActivityReqLog> EventActivityReqLog { get; set; }



        /// use modelviews instead...
        [NotMapped]
        public string strChurchlife { get; set; }
        [NotMapped]
        public string strActivityTypeCode { get; set; }

        [NotMapped]
        public string strEventFullDesc { get; set; }

        [NotMapped]
        public string strChurchBodyService { get; set; }

        [NotMapped]
        public string strChurchEventCategory { get; set; }
    }
}
