using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberChurchlife
    {
        public MemberChurchlife() { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId  { get; set; }
        public int? ChurchBodyId { get; set; }
       // public int? OwnedByChurchBodyId { get; set; } 
        public int? ChurchMemberId { get; set; } 

        

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateJoined { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]         
        public DateTime? DateDeparted { get; set; }

        public int? ChurchBodyServiceId { get; set; }   // thus the preferred service
        public bool IsPioneer { get; set; }       
        public bool IsCurrentMember { get; set; }
        public bool IsMemBaptized { get; set; }        
        public bool IsMemConfirmed { get; set; }   // only for protestant and catholic churches -- check the faith type        
        public bool IsMemCommunicant { get; set; } 

        [StringLength(100)]
        public string NonCommReason { get; set; }

        [StringLength(100)]
        public string NonConfirmReason { get; set; }

        public bool IsDeceased { get; set; }

        [StringLength(1)]
        public string EnrollMode { get; set; }  // How did the person join the church ... walk-in, transfer, invite/refer, thru parent, via marriage, other

        [StringLength(100)]
        public string EnrollReason { get; set; } // How did the person leave the church ... marriage, relocation, conflict, travelled, death etc.

        [StringLength(1)]
        public string DepartMode { get; set; }  // How did the person join the church ... transfer, death, marriage, official request out, unannounced, other

        [StringLength(100)]
        public string DepartReason { get; set; } // How did the person leave the church ... marriage, relocation, conflict, travelled, death etc.

        [StringLength(1)]
        public string HealthConditionStatus { get; set; }   // A - Fit /Well (Active), R-Recuperating (Recovering), S-Sick (Indisposed), I-Invalid (long-term ailment)   -- change in status to be recorded too and the visits that came along.

        [StringLength(500)]
        public string MemberlifeSummary { get; set; }  // member related matters ONLY

        [StringLength(1)]
        public string SharingStatus { get; set; }  /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                   /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                   /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 
        public string ChurchlifePhotoUrl { get; set; }  // favorite churchlife picture


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }
        [NotMapped]//[ForeignKey(nameof(LastModByUserId))]
        public virtual MSTRModels.UserProfile LastModByUser { get; set; }



        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        //[NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        //public virtual ChurchBody OwnedByChurchBody { get; set; }


        [ForeignKey(nameof(ChurchBodyServiceId))] 
        public virtual ChurchBodyService ChurchBodyService { get; set; }

        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }
    }
}
