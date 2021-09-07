using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public class ChurchUnit
    {
        public ChurchUnit()
        { }
        /// <summary>
        /// CHURCH_UNIT handles all other entities in the church connected with some sort of groupings, depatmentally inclined... directly.
        /// Church Grouping ... Applies to only groupable units like... CO, CG, CE, SC, DP
        /// Somewhat a under control...  
        /// </summary>

        [Key]
        public int Id { get; set; }

        public int? AppGlobalOwnerId { get; set; }   // local copy synced
        public int? OwnedByChurchBodyId { get; set; }  // CB creating it...        
        public int? TargetChurchLevelId { get; set; }    // local copy synced ... at what level of the church will this Unit function --> national, district or congregation
        
        public int? ParentUnitId { get; set; }
        public int? ParentUnitCBId { get; set; }  // get the OrgType via here ...         

        //public int? ParentUnitChurchLevelId { get; set; }
        //public int? ParentUnitChurchBodyId { get; set; } 

        //[StringLength(2)]
        //public string ParentUnitOrgType { get; set; }

        [StringLength(2)]
        public string OrgType { get; set; }    // Church Root, [ GB--Government Body ], CU--Congregation Head-unit, CN--Congregation 

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(10)]
        public string Acronym { get; set; }    // Junior Youth -- JY, Young Adults Fellowship -- YAF

        [Display(Name = "Unit Code")]   // READ-ONLY --- GlobalChurchCode ::>>  [ RootChurchCode /Acronym ] - [7-digit] ... RCM0000000, RCM0000001, PCG1234567, COP1000000, ICGC9999999
        [StringLength(30)]
        public string GlobalUnitCode { get; set; }

        [Display(Name = "Relative Unit Code")]
        [StringLength(500)]
        public string RootUnitCode { get; set; }  // READ ONLY --- ChurchCodeFullPath ::>> RCM-000001--RCM-000001--RCM-000001--RCM-000001
        
        [Display(Name = "Custom Unit Code")]
        [StringLength(30)]
        public string UnitCodeCustom { get; set; }

        public int? ContactInfoId { get; set; }

        public bool IsFullAutonomy { get; set; }   // Groups may be under other groups... before innaug. Preaching Points are not full autonomy... they are supervised
        public bool IsSupervisedByParentUnit { get; set; }   // ideally so but sometimes differ... even with congregations
        public int? SupervisedByUnitId { get; set; }  
        public int? SupervisedByUnitCBId { get; set; } // get the OrgType via here ...
         

        [Display(Name = "Formed")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateFormed { get; set; }

        [Display(Name = "Innaugurated")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateInnaug { get; set; }

        [Display(Name = "Deactivated")]   // Dissolved, Ends, Completed
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateDeactive { get; set; }

        [StringLength(200)]
        public string CoreFunction { get; set; }   // portfolio -- for committee and related 

        [StringLength(500)]
        public string BriefHistory { get; set; }


        public double? OrderIndex { get; set; }  // jux to order them... authority index e.g. CS, JY, YPG, YAF, MF, WF, Aged

        [StringLength(1)]
        public string GenderStatus { get; set; }   // Male- M, Female -F, Mixed - M 

        public bool IsUnitGen { get; set; }   //  Generational -- age biased  .. can overlap /interlap
        public bool IsAgeBracketOverlaps { get; set; }   //  Y -- allow overlap, N - disallow

        public int? UnitMaxAge { get; set; }  // --- CG >=  CS - 0, JY - 12, YPG - 18, YAF - 30, WF/MF - 40   
        public int? UnitMinAge { get; set; }    // --- CG < CS - 12, JY - 18, YPG - 30, YAF - 40, WF/MF - ??

        public string UnitLogo { get; set; }
         
        [StringLength(100)]
        public string StatusReason { get; set; }  // If dissolved/deactivated... any comment/reason

        [StringLength(1)]
        public string Status { get; set; }  // A-ctive, B-locked [deactive], I-nnaugurated [also active], D-issolved [deactive]  // public bool? IsActivated { get; set; }

        [StringLength(1)]
        public string SharingStatus { get; set; }   // N-Not shared C-shared with Child CB unit only A-Shared with all  per parent body

        [StringLength(2)]
        public string ChurchWorkStatus { get; set; }   // OPerationalized - OP, STructure only - ST

        //[StringLength(1)]
        //public string OwnershipStatus { get; set; }  // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody

        [Display(Name = "Additional Comments")]
        [StringLength(200)]
        public string Comments { get; set; }


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }



        [NotMapped] //[ForeignKey(nameof(CreatedByUserId))]
        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }

        [NotMapped] // [ForeignKey(nameof(LastModByUserId))]
        public virtual MSTRModels.UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ParentUnitId))]
        public virtual ChurchUnit ParentChurchUnit { get; set; }

        [ForeignKey(nameof(ParentUnitCBId))]
        public virtual ChurchBody ParentUnitCB { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))]
        public virtual ChurchBody OwnedByChurchBody { get; set; }

        [ForeignKey(nameof(TargetChurchLevelId))] //[InverseProperty("OwnedByChurchBody_CL")]
        public virtual ChurchLevel TargetChurchLevel { get; set; }

        //[ForeignKey(nameof(ParentUnitChurchLevelId))]
        //public virtual ChurchLevel ParentUnitChurchLevel { get; set; }

        //[ForeignKey(nameof(ParentUnitChurchBodyId))]
        //public virtual ChurchBody ParentUnitChurchBody { get; set; }

        [ForeignKey(nameof(ContactInfoId))]
        public virtual ContactInfo ContactInfo { get; set; }

        [ForeignKey(nameof(SupervisedByUnitId))]
        public virtual ChurchUnit SupervisedByUnit { get; set; }
         
        [ForeignKey(nameof(SupervisedByUnitCBId))]
        public virtual ChurchBody SupervisedByUnitCB { get; set; }


        [NotMapped]
        public virtual List<ChurchBody> SubChurchUnits { get; set; }


        //[NotMapped]
        //public string strOwnerChurchBody { get; set; }
        //[NotMapped]
        //public string strAppGlobalOwn { get; set; }
        //[NotMapped]
        //public string strChurchUnit { get; set; }
        //[NotMapped]
        //public string strParentChurchUnit { get; set; }        
        //[NotMapped]
        //public string strOrgType { get; set; }
        //[NotMapped]
        //public string strDateFormed { get; set; }
        //[NotMapped]
        //public string strDateInnaug { get; set; }
        //[NotMapped]
        //public string strTargetChurchLevel { get; set; }
        //[NotMapped]
        //public string strChurchWorkStatus { get; set; }
        //[NotMapped]
        //public string strGenderStatus { get; set; } // Female, Male, Mixed 
        //[NotMapped]
        //public string strStatus { get; set; } // Active, Blocked, Deactive
        //[NotMapped]
        //public string strSharingStatus { get; set; }  // All share, Child share, Not share
        //[NotMapped]
        //public string strOwnershipStatus { get; set; }   // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody
        //[NotMapped] 
        //public bool? bl_IsUnitGen { get; set; }    
        //[NotMapped] 
        //public bool? bl_AgeBracketOverlaps { get; set; }
         
    }
}
