using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ChurchCalendarEventDetail
    {
        public ChurchCalendarEventDetail()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        // public int? OwnedByChurchBodyId { get; set; }


        [StringLength(100)]
        public string EventDescription { get; set; }   /// Prayer Conference 2021: Day 1 - Aug 26 2021, Prayer Conference 2021: Day 2 - Aug 27 2021, Prayer Conference 2021: Day 3 - Aug 28 2021,

        [StringLength(1)]
        public string EventSessionCode { get; set; }  /// Session 1 - Morning Session, Session 2  - Afternoon Session, Session 3  - Evening Session etc. with their respective times of meeting [start /closure]
      
        [StringLength(50)]
        public string CustomSessionDesc { get; set; }  /// Session 1 - Morning Session, Session 2  - Evening Session etc. with their respective times of meeting [start /closure]
         
        public int? ChurchCalendarEventId { get; set; }  /// Prayer Conference 2021: Aug 26-28 2021
      ///  public int? ChurchEventCategoryId { get; set; }
        public bool IsChurchServiceEvent { get; set; }  /// @detail level ... ensure the activity picked up.. is also Church service
        public int? ChurchBodyServiceId { get; set; }   // @detail level ... if not.. null

        public int? ChurchlifeActivityId { get; set; }    // @detail level ... you can pick particular activity to do on the day 1 [Prayer Teachings], 2 [Prayer Bazaar] or 3 [Prayer Warfare]  ... link to desc


        [DisplayFormat(ConvertEmptyStringToNull = true)] /// , NullDisplayText = "None")]
        [StringLength(100)]
        public string Venue { get; set; }       /// @detail level ... if not.. keep at the main
        public bool IsFullDay { get; set; }     /// @detail level ... if not.. keep at the main

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy hh:mm a}", ApplyFormatInEditMode = false)]    /// dd-MMM-yy hh:mm a  --- daily! so only the time may differ here
        public DateTime? EventFrom { get; set; }    //  time included...  merge :- @ui

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yy hh:mm a}", ApplyFormatInEditMode = false)]   /// dd-MMM-yy hh:mm a  --- daily! so only the time may differ here
        public DateTime? EventTo { get; set; }   //  time included...  merge :- @ui


        //[StringLength(50)]
        //public string ThemeColor { get; set; }  // get the color palettes  
        public bool IsEventActive { get; set; }  // can be deactivated ...   @detail level ... if not.. keep at the main


        public bool IsCurrentEvent { get; set; }  /// thus.. today !

        [StringLength(1)]
        public string EventStatus { get; set; }   //  P-Open/Pending, I-In Progress, D-Deactive, On-Hold (H), Cancelled (X), Complete (C)

        [StringLength(1)]
        public string SharingStatus { get; set; }    /// @detail level ... if not.. keep at the main
                                                     /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                     /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                     /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 
         


       public string PhotoUrl { get; set; }    /// @detail level ... if not.. keep at the main



        [StringLength(300)]
        public string Notes { get; set; }


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


        [ForeignKey(nameof(ChurchCalendarEventId))]
        public virtual ChurchCalendarEvent ChurchCalendarEvent { get; set; }


        [ForeignKey(nameof(ChurchlifeActivityId))]
        public virtual AppUtilityNVP ChurchlifeActivity_NVP { get; set; }  //ChurchlifeActivity



        [NotMapped]
        public string strEventFullDesc { get; set; }
    }
}






