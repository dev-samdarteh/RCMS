//using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.MSTRModels;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace RhemaCMS.Models.CLNTModels
//{
//    public partial class ChurchMemStatus
//    {
//        public ChurchMemStatus()
//        {  }

//        [Key]
//        public int Id { get; set; }
//        public int? AppGlobalOwnerId { get; set; }
//        public int? ChurchBodyId { get; set; }
//        public int? OwnedByChurchBodyId { get; set; }

//        [StringLength(15)]
//        public string Name { get; set; }
//        [StringLength(50)]
//        public string Description { get; set; }
//        [StringLength(4)]
//        public string Abbrev { get; set; }
//        public bool IsDeceased { get; set; }
//        public bool IsAvailable { get; set; }
//        public int? OrderIndex { get; set; } 

//        [StringLength(1)]
//        public string SharingStatus { get; set; }

//        public DateTime? Created { get; set; }
//        public DateTime? LastMod { get; set; }
//        public int? CreatedByUserId { get; set; }
//        public int? LastModByUserId { get; set; }

//        [NotMapped]
//       // [ForeignKey(nameof(CreatedByUserId))]
//        public virtual UserProfile CreatedByUser { get; set; }
//        [NotMapped]
//       // [ForeignKey(nameof(LastModByUserId))]
//        public virtual UserProfile LastModByUser { get; set; }


//        // public virtual List<MemberStatus> MemberStatus { get; set; }

//        [ForeignKey(nameof(AppGlobalOwnerId))]
//        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

//        [ForeignKey(nameof(ChurchBodyId))]
//        public virtual ChurchBody ChurchBody { get; set; }

//        [ForeignKey(nameof(OwnedByChurchBodyId))] 
//        public virtual ChurchBody OwnedByChurchBody { get; set; }
//    }
//}
