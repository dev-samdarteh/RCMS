using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class RelationshipType
    {
        public RelationshipType()
        {  }

        //[Key] 
        //public int Id { get; set; }


        [Key][Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RelationCode { get; set; }  // USE this as ref # instead of the arbitrary id -- unique gen

        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }     // Great grandfather, Grandfather, Father ...
        public int LevelIndex { get; set; }        
        public int? RelationshipTypeFemalePairCode { get; set; } 
        public int? RelationshipTypeGenericPairCode { get; set; } 
        public int? RelationshipTypeMalePairCode { get; set; }
        
        public bool IsChild { get; set; }  // son, daughter, child
        public bool IsSpouse { get; set; }  // wife, husband
        public bool IsParent { get; set; }  // father, mother

        //[StringLength(1)]
        //public string Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }

        // public virtual List<MemberContact> MemberContact { get; set; } 
        // public virtual List<MemberRelation> MemberRelation { get; set; }

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }

    }
}
