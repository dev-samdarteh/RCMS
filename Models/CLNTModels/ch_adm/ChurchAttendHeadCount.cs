using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    //[Table("ChurchAttend_HeadCount")]
    public partial class ChurchAttendHeadCount
    {
        /// <summary>
        /// Attendance Summary... head counts
        /// 
        /// </summary>
        
        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [StringLength(50)]
        public string CountEventDesc { get; set; }

        public DateTime? CountDate { get; set; }
        public int? ChurchEventId { get; set; }

        [StringLength(1)]
        public string CountType { get; set; }  // mainstream [together] or unit [segmented]        
        public int? ChurchUnitId { get; set; }   // Role in ... CO, CG, CE, SC, DP  ...  
        ///
        public long TotCount { get; set; }            
        public long TotM { get; set; }       
        public long TotF { get; set; }
        public long TotO { get; set; }
        ///
        public long Tot_C { get; set; }  // Child
        public long Tot_Y { get; set; }  // Youth .. get the settings for age-groupings :- this may be diff from the generational groupings aging
        public long Tot_YA { get; set; } // Young Adult ... Adult = [YA + MA + OA]
        public long Tot_MA { get; set; } // Mid-aged Adult
        public long Tot_AA { get; set; } // Aged Adult 


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]
       // [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]
       // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner CountryAppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))] 
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(ChurchEventId))] 
        public virtual ChurchCalendarEvent ChurchEvent { get; set; }        

        [ForeignKey(nameof(ChurchUnitId))]
        public virtual ChurchUnit ChurchUnit { get; set; }

    }
}
