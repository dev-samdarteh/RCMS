using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public class ChurchRole
    {

        public ChurchRole()
        { }

        [Key]
        public int Id { get; set; }

        public int? AppGlobalOwnerId { get; set; }   // local copy synced
        public int? OwnedByChurchBodyId { get; set; }        
        public int? TargetChurchLevelId { get; set; }    // District Pastor at district, Min in charge at congregation

        public int? ParentRoleId { get; set; }   // if any... IT/Tech team --> [ Media Operator, Sound Engineer, Audiovisual Specialist ]
        public int? ParentRoleCBId { get; set; }

        
        [StringLength(2)]
        public string OrgType { get; set; }    // Team -- TM, Position -- CP 

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(15)]
        public string Acronym { get; set; }    // District President -- DP, Superintendent Minister -- SM, 

        [StringLength(200)]
        public string Description { get; set; }

        [Display(Name = "Role Code")]   //CRL1234 READ-ONLY --- GlobalChurchCode ::>>  [ RootChurchCode /Acronym ] - [7-digit] ... RCM0000000, RCM0000001, PCG1234567, COP1000000, ICGC9999999
        [StringLength(20)]
        public string GlobalRoleCode { get; set; }

        [Display(Name = "Relative Role Code")]  // only where Position or Role is under another Position or Role ::-- CHURCH_ORG_CHART handles the rest
        [StringLength(200)]
        public string RootRoleCode { get; set; }  //CRL1234--CRL1234--CRL1234 ... READ ONLY --- ChurchCodeFullPath ::>> RCM-000001--RCM-000001--RCM-000001--RCM-000001
                                                  //[Display(Name = "Custom Code")]
                                                  //[StringLength(20)]
                                                  //public string ChurchCodeCustom { get; set; }
        [StringLength(1)]
        public string ApplyToGender { get; set; }   // Male- M, Female -F, Mixed - M 
        public int? RankIndex { get; set; }  // Grade the CP's  ... Minister is higher than Catechist, then Presyter, then Group Presidents...
        public bool IsApplyToClergyOnly { get; set; }    // CP :-- Minister, Catechist  
        public bool IsAdhocRole { get; set; }    // CP :-- Chapel keeper, Usher ... not actual 'recognized' positions

        public bool IsApplyToMainstreamUnit { get; set; }    // CP :-- Minister, Catechist, Session Clerk apply to the congregation mainstream
        public int? ApplyToChurchUnitId { get; set; }   // can be null... in that case will apply to the congregation or church body Ex. Minister, Catechist, Presbyter

        public int? MinNumAllowed { get; set; }    // ---  CP ... President - 1, Presbyter - 5
        public int? MaxNumAllowed { get; set; }  // --- CP ... President - 1, Presbyter - 13

        [Display(Name = "Term (of Office) Type")]
        [StringLength(1)]
        public string OfficeTermType { get; set; } // T-enure, Y = Person Age ...yrs

        [Display(Name = "Max Duration")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? OfficeTermMax_Yrs { get; set; }   //UOM ?.. Years

        [StringLength(200)]
        public string PrimaryFunction { get; set; }   // brief func of position

        //[Display(Name = "Formed")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        //public DateTime? DateFormed { get; set; }

        //[Display(Name = "Innaugurated")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        //public DateTime? DateInnaug { get; set; }

        //[Display(Name = "Deactivated")]   // Dissolved, Ends, Completed
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        //public DateTime? DateDeactive { get; set; }
         

        // public bool IsActivated { get; set; }

        [StringLength(1)]
        public string Status { get; set; }  // A-ctive, B-locked [deactive], I-nnaugurated [also active], D-issolved [deactive]  // public bool? IsActivated { get; set; }

        [StringLength(1)]
        public string SharingStatus { get; set; }   // N-Not shared C-shared with Child CB unit only A-Shared with all  per parent body

        [StringLength(2)]
        public string ChurchWorkStatus { get; set; }   // OPerationalized - O, STructure only - S   ... Directors [structure only], District Evangelism Coordinators [Operationalized]

        [StringLength(1)]
        public string OwnershipStatus { get; set; }  // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody

        [Display(Name = "Additional Comments")]
        [StringLength(100)]
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

        [ForeignKey(nameof(ParentRoleId))]
        public virtual ChurchRole ParentChurchRole { get; set; }

        [ForeignKey(nameof(ApplyToChurchUnitId))]
        public virtual ChurchUnit ApplyToChurchUnit { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))]
        public virtual ChurchBody OwnedByChurchBody { get; set; }

        [ForeignKey(nameof(ParentRoleCBId))]
        public virtual ChurchBody ParentRoleCB { get; set; }

        [ForeignKey(nameof(TargetChurchLevelId))] 
        public virtual ChurchLevel TargetChurchLevel { get; set; }

         
        //[NotMapped]
        //public string strOwnerChurchBody { get; set; }
        //[NotMapped]
        //public string strAppGlobalOwn { get; set; }
        //[NotMapped]
        //public string strParentChurchBody { get; set; }
      
        //[NotMapped]
        //public string strOrgType { get; set; }
        //[NotMapped]
        //public string strTargetChurchLevel { get; set; }
        //[NotMapped]
        //public string strChurchWorkStatus { get; set; }

        //[NotMapped]
        //public string strDateFormed { get; set; }
        //[NotMapped]
        //public string strDateInnaug { get; set; }
        //[NotMapped]
        //public string strDateDeactive { get; set; }

        //[NotMapped]
        //public bool bl_IsActivated { get; set; }
        //public bool bl_ApplyToClergyOnly { get; set; }

        //[NotMapped]
        //public string strStatus { get; set; }
        //[NotMapped]
        //public string strSharingStatus { get; set; }

        //[NotMapped]
        //public string strOwnershipStatus { get; set; }   // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody


    }
}
