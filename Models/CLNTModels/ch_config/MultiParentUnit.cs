//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace RhemaCMS.Models.CLNTModels
//{
//    public class MultiParentUnit
//    {

//        public MultiParentUnit()
//        { }


//        [Key]
//        public int Id { get; set; }

//        public int? AppGlobalOwnerId { get; set; }   // local copy synced
//        public int? OwnedByChurchBodyId { get; set; }
//        public int? TargetChurchLevelId { get; set; }    // District Pastor at district, Min in charge at congregation

//        public int? ParentUnitCBId { get; set; }    
//        public int? ParentUnitId { get; set; }    

         
//        [StringLength(2)]
//        public string ParentOrgType { get; set; }    // Team -- TM, Position -- CP 
         

//        [StringLength(1)]
//        public string Status { get; set; }  // A-ctive, B-locked [deactive], I-nnaugurated [also active], D-issolved [deactive]  // public bool? IsActivated { get; set; }

//        [StringLength(1)]
//        public string SharingStatus { get; set; }   // N-Not shared C-shared with Child CB unit only A-Shared with all  per parent body

//        [StringLength(2)]
//        public string ChurchWorkStatus { get; set; }   // OPerationalized - O, STructure only - S   ... Directors [structure only], District Evangelism Coordinators [Operationalized]

//        [StringLength(1)]
//        public string OwnershipStatus { get; set; }  // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody

//        [Display(Name = "Additional Comments")]
//        [StringLength(100)]
//        public string Comments { get; set; }


//        public DateTime? Created { get; set; }
//        public DateTime? LastMod { get; set; }
//        public int? CreatedByUserId { get; set; }
//        public int? LastModByUserId { get; set; }


//        [NotMapped] //[ForeignKey(nameof(CreatedByUserId))]
//        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }

//        [NotMapped] // [ForeignKey(nameof(LastModByUserId))]
//        public virtual MSTRModels.UserProfile LastModByUser { get; set; }


//        [ForeignKey(nameof(AppGlobalOwnerId))]
//        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

//        [ForeignKey(nameof(OwnedByChurchBodyId))]
//        public virtual ChurchBody OwnedByChurchBody { get; set; }

//        [ForeignKey(nameof(TargetChurchLevelId))]
//        public virtual ChurchLevel TargetChurchLevel { get; set; }
 

//        [ForeignKey(nameof(ParentUnitCBId))]
//        public virtual ChurchBody ParentUnitCB { get; set; }

//        [ForeignKey(nameof(ParentUnitId))]
//        public virtual ChurchUnit ParentUnit { get; set; }
 
         
//    }
//}

