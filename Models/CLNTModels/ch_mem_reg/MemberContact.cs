using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberContact
    {
        public MemberContact() { }

        [Key]
        public int Id { get; set; }

        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }
        public int? ChurchMemberId { get; set; }    
        ///
        public bool IsCurrentContact { get; set; }


        [StringLength(1)]
        public string RelationCategory { get; set; }  // N-uclear, E-xtended, F-aith Related, V-aried relation //// Associate, Acquintance

        [StringLength(1)]
        public string RelationScope { get; set; }   // L-local cong, C- Chu Associate..ext cong but within denom, E [not church fellow]

        public int? ContactChurchBodyId { get; set; }    // RelationScope == L /C
        public int? ContactChurchMemberId { get; set; }  // RelationScope == L /C

        public int? RelationshipCode { get; set; }   /// expeand the list... can be outside the family

        // [StringLength(1)]
        // public string ChurchFellowCode { get; set; }   // L-local cong, C- Chu Associate..ext cong but within denom, E [not church fellow]
        // public int? InternalContactChurchMemberId { get; set; }

        // public int? ExtConChurchAssociateChurchBodyId { get; set; }
        // public int? ExtConChurchAssociateMemberId { get; set; }

        //// public int? InternalContactId { get; set; }   


        ///

        // only when   // RelationScope == E
        

        [StringLength(100)]
        public string ContactNameExtCon { get; set; }
        [StringLength(30)]
        public string LocationExtCon { get; set; }
        [StringLength(100)]
        public string ResidenceAddressExtCon { get; set; }
        public bool ResAddrSameAsPostAddrExtCon { get; set; }
        [StringLength(30)]
        public string PostalAddressExtCon { get; set; }
        [StringLength(30)]
        public string DigitalAddressExtCon { get; set; }
        [StringLength(30)]
        public string CityExtCon { get; set; }
        public int? RegionIdExtCon { get; set; }

        [StringLength(3)]
        public string CtryAlpha3CodeExtCon { get; set; }        

        [StringLength(15)]
        public string MobilePhone1ExtCon { get; set; }
        [StringLength(15)]
        public string MobilePhone2ExtCon { get; set; }
        public string EmailExtCon { get; set; }


        ///
        public string FaithAffiliationExtCon { get; set; }  // C-Christian, X-Non-Christian ... MUS-uslim, ATR-African Traditional Religion, N - N/A [None]
        [StringLength(100)]
        public string DenominationExtCon { get; set; }

        public string PhotoUrlExtCon { get; set; }
        ///
        // only when   // RelationScope == E

        [StringLength(1)]
        public string Status { get; set; }  // Active, Blocked

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped]//  [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }

        [ForeignKey(nameof(ChurchMemberId))]
        public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(CtryAlpha3CodeExtCon))]
        public virtual Country CountryExtCon { get; set; }

        //[ForeignKey(nameof(FaithTypeCategoryIdExtCon))] 
        //public virtual ChurchFaithType FaithTypeCategoryIdExtConNavigation { get; set; }

        [ForeignKey(nameof(RegionIdExtCon))]
        public virtual CountryRegion RegionExtCon { get; set; }

        [ForeignKey(nameof(ContactChurchBodyId))]
        public virtual ChurchBody RelationChurchBody { get; set; }

        [ForeignKey(nameof(ContactChurchMemberId))]
        public virtual ChurchMember RelationChurchMember { get; set; }

        [ForeignKey(nameof(RelationshipCode))]
        public virtual RelationshipType RelationshipType { get; set; }



    }
}
