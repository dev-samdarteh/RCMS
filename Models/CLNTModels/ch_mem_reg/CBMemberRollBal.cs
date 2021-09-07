using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public class CBMemberRollBal
    {
        public CBMemberRollBal() { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [Display(Name = "Account Period")]
        public int? ChurchPeriodId { get; set; }
         
        public long TotRoll { get; set; }
        public long Tot_M { get; set; }  // Male
        public long Tot_F { get; set; }  // Female
        public long Tot_O { get; set; }  // Other
        ///
        public long Tot_C { get; set; }  // Child
        public long Tot_Y { get; set; }  // Youth .. get the settings for age-groupings :- this may be diff from the generational groupings aging
        public long Tot_YA { get; set; } // Young Adult ... Adult = [YA + MA + OA]
        public long Tot_MA { get; set; } // Mid-aged Adult
        public long Tot_AA { get; set; } // Aged Adult 
        ///
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }
        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
        public virtual MSTRModels.UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(ChurchPeriodId))]
        public virtual ChurchPeriod ChurchPeriod { get; set; }

    }
}





