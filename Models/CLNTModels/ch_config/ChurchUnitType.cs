//using RhemaCMS.Models.MSTRModels;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace RhemaCMS.Models.CLNTModels
//{
//    public partial class ChurchUnitType
//    {
//        public ChurchUnitType()
//        { }

//        [Key]
//        public int Id { get; set; }
//        public int? AppGlobalOwnerId { get; set; }
//        [Required]
//        [StringLength(50)]
//        public string Description { get; set; }
//        [StringLength(2)]
//        public string OrganisationType { get; set; }
//        public int? OrderIndex { get; set; }
//        [StringLength(1)]
//        public string Status { get; set; }
//        public int? OwnedByChurchBodyId { get; set; }
//        [StringLength(1)]
//        public string SharingStatus { get; set; }
//        public DateTime? Created { get; set; }
//        public DateTime? LastMod { get; set; }
//        public int? CreatedByUserId { get; set; }
//        public int? LastModByUserId { get; set; }


//        [NotMapped]//[ForeignKey(nameof(CreatedByUserId))]
//        public virtual UserProfile CreatedByUser { get; set; }
//        [NotMapped]//[ForeignKey(nameof(LastModByUserId))]
//        public virtual UserProfile LastModByUser { get; set; }

//        [ForeignKey(nameof(OwnedByChurchBodyId))] 
//        public virtual ChurchBody OwnedByChurchBody { get; set; } 
//        //public virtual List<ChurchBody> ChurchBodies { get; set; }
//        //public virtual List<ChurchFaithType> SubFaithTypes { get; set; }
//    }
//}
