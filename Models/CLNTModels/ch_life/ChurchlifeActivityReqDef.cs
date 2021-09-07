using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ChurchlifeActivityReqDef
    {
        public ChurchlifeActivityReqDef()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }

        public int? ChurchlifeActivityId { get; set; }
        [StringLength(50)]
        public string RequirementDesc { get; set; }

        public int? OrderIndex { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TargetOccurences { get; set; }    // counts .. e.g. 3months pre-marital counselling
        [StringLength(1)]
        public string OccurFreqCode { get; set; }   // d-Daily, w-Weekly, m-Monthly, s-Semesterly, y-Yearly
        public bool IsRequired { get; set; }       
        public bool IsSequenced { get; set; }

       
        [StringLength(1)]
        public string SharingStatus { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]//[ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]//[ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

          [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }


        [ForeignKey(nameof(ChurchlifeActivityId))] 
        public virtual ChurchlifeActivity ChurchlifeActivity { get; set; }
        
       // public virtual List<EventActivityReqLog> EventActivityReqLog { get; set; }
    }
}
