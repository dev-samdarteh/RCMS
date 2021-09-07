using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    //[Table("ChurchAttend_Attendees")]
    public partial class ChurchAttendAttendee
    {
        /// <summary>
        /// Individual Member Attendance records
        /// 
        /// </summary>
        
        [Key]
        public int Id { get; set; }

        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateAttended { get; set; }
        public int? ChurchEventDetailId { get; set; }   /// specific daily event attended ... cud be week conference but which of the the days was attended..

        [StringLength(100)]
        public string AttendEventDesc { get; set; }  // additional info... or in case event is not yet registered  on the calendar

        [StringLength(1)]
        public string AttendeeType { get; set; }  // Member (M), New Convert (N)... or Guest (G), Visitor (V)

        public int? ChurchMemberId { get; set; }  // link Member profile to their attendances


        // Guest Attendance details
        /// <summary>
        /// Visitors are prospective MEMBERS in the short/long run... align details to Members Bio account
        /// </summary>
        


        [StringLength(15)]
        public string Title { get; set; }
        [StringLength(30)]
        public string FirstName { get; set; }
        [StringLength(30)]
        public string MiddleName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }
                
        public int? AgeBracketId { get; set; }  // pick from the standard age bracket defined... NVP

        [StringLength(3)]
        public string NationalityId { get; set; }  //CtryAlpha3Code e.g. GHA         

        [StringLength(50)]
        public string ResidenceLoc { get; set; }

        [Phone]
        public string MobilePhone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
         
        public int? ChurchMemStatusId { get; set; }

        public int? ChurchRankId { get; set; }

        public int? ChurchMemTypeId { get; set; }   /// Visitor (V), Guest (G), /// Congregant (C) ///


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateOfBirth { get; set; }

        // [Required]
        [StringLength(1)]
        public string MaritalStatus { get; set; } // Single, Married, Separated, Divorced, Widowed, Other

        public bool? isPersFirstimer { get; set; }   /// validate frequency...

        // public int? VisitReasonId { get; set; }

        //[StringLength(1)]
        //public string VisistorAttendeeType { get; set; }  // First-timer, Regular Visitor, Prospect

        [StringLength(1)]
        public string EnrollMode { get; set; }  // How did the person join the church ... transfer, marriage, relocation

        [StringLength(100)]
        public string VisitReason { get; set; }
        ///
        // Guest Attendance details ... vitals
         
        ///
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TempCelc { get; set; }  //  ... check against | range in NVP 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PersKgWt { get; set; }  // weight in kg   ... check against | range in NVP obesse/anaemic

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PersBPMin { get; set; }  // blood pressure... diastolic   diastolic is less than 80... check against | range in NVP  ...

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PersBPMax { get; set; }  // blood pressure... systolic  is less than 120 and my 


        [StringLength(200)]
        public string Notes { get; set; }   // in case the temperature exceeds limit or some other notable comment


        [StringLength(1)]
        public string SharingStatus { get; set; }  /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                   /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                   /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped]
       // [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }

        [NotMapped]  // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(ChurchMemberId))]
        public virtual ChurchMember ChurchMember { get; set; }


        [ForeignKey(nameof(AgeBracketId))] 
        public virtual AppUtilityNVP AgeBracket { get; set; }
               
        [ForeignKey(nameof(ChurchEventDetailId))] 
        public virtual ChurchCalendarEventDetail ChurchEventDetail { get; set; }
              
        [ForeignKey(nameof(NationalityId))] //CtryAlpha3Code
        public virtual Country Nationality { get; set; }   // 

        //[ForeignKey(nameof(VisitReasonId))] 
        //public virtual AppUtilityNVP VisitReason { get; set; }

        [ForeignKey(nameof(ChurchRankId))]
        public virtual AppUtilityNVP ChurchRank_NVP { get; set; }   // ChurchRank

        [ForeignKey(nameof(ChurchMemStatusId))]
        public virtual AppUtilityNVP ChurchMemStatus_NVP { get; set; }   // ChurchMemStatus

    }
}
