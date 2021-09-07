using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberType
    {
        public MemberType() { }


        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }    /// cud be by transfer...
        public int? OwnedByChurchBodyId { get; set; }    /// Created it... 
        ///
        public int? ChurchMemberId { get; set; }

       // public int? ChurchMemTypeId { get; set; }

        [StringLength(1)]
        public string MemberTypeCode { get; set; }      ///Clergy, Lay Minister, Church Leader,  Congregant [C] ... In-Transit (T) ... New Convert [N] ... Affiliate \\\  --seasonal-- [A], Guest/Visitor [G/V] ...

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FromDate { get; set; }

        public bool IsCurrent { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToDate { get; set; }

        [StringLength(30)]
        public string Reason { get; set; }

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

        [NotMapped]//  [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]//[ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }


        //[ForeignKey(nameof(ChurchMemTypeId))] 
        //public virtual AppUtilityNVP ChurchMemType_NVP { get; set; }  // ChurchMemType

        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }

        [NotMapped]
        public string strChurchMemType { get; set; }
        [NotMapped]
        public string strAssigned { get; set; }
        [NotMapped]
        public string strUntil { get; set; }
    }
}
