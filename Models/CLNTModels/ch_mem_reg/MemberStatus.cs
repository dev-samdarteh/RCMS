using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberStatus
    {
        public MemberStatus() { }


        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; } 
        public int? ChurchBodyId { get; set; }
     //   public int? OwnedByChurchBodyId { get; set; }
        ///
        public int? ChurchMemberId { get; set; }
        public int? ChurchMemStatusId { get; set; }

        public bool IsCurrent { get; set; }

        // Where is this member now.. in case MOVED [migration]
        public int? ToChurchBodyId { get; set; }  // Tema Comm 2 Congregation ... allow member details to be tracked

        [StringLength(50)]
        public string ToLocation { get; set; }   // From Taifa .. Tema


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToDate { get; set; }

        [StringLength(30)]
        public string Reason { get; set; }  // Transfer, relocation

        [StringLength(100)]
        public string Notes { get; set; } 

        [StringLength(1)]
        public string SharingStatus { get; set; }  /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                   /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                   /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 


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


        [ForeignKey(nameof(ChurchMemStatusId))] 
        public virtual AppUtilityNVP ChurchMemStatus_NVP { get; set; }   // ChurchMemStatus

        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }

         
        [NotMapped]
        public string strSince { get; set; }
        [NotMapped]
        public string strUntil { get; set; }
        [NotMapped]
        public string strChurchMemStatus { get; set; }

    }
}
