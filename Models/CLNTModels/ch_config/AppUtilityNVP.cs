using RhemaCMS.Models.MSTRModels;
using System; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    [Table("AppUtilityNVP")]
    public partial class AppUtilityNVP
    {
        public AppUtilityNVP()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }

       
        [StringLength(30)]
        public string NVPCode { get; set; }
       
        [StringLength(15)]
        public string NVPSubCode { get; set; }    //  MA - Member Activity, GA - General Activity, EV - Event-related, MR - Member Related

        // [StringLength(2)]
        // public string NVPSubCategory { get; set; }   //  MA - Member Activity, GA - General Activity, EV - Event-related, MR - Member Related
               
        [StringLength(1)]
        public string ValueType { get; set; }   // Text(T), Number (N), Date (D)   

        [StringLength(10)]
        public string Acronym { get; set; }    //  CLA:- BAP, CNF, BRV, MRD, WED, etc.

        // public bool IsValNumeric { get; set; }
        [StringLength(100)]
        public string NVPValue { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? NVPNumVal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? NVPNumValTo { get; set; }  // thus for range items.. ex. Age groups:- Child [0-12],  Youth [13-29],  Adult [30-69],  Aged [70-150]

        // public bool IsValDate { get; set; } 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? NVPFromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? NVPToDate { get; set; }
         
        [StringLength(200)]
        public string NVPDesc{ get; set; }
         
        public int? NVPCategoryId { get; set; }  // don't self reference ... make them specific to entity   ... ex.  ChurchlifeActivityId for Req Def [parented]

        [StringLength(3)]
        public String CtryAlpha3Code { get; set; }   // GHA, USA etc use this as key standard globally
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? GradeLevel { get; set; }
        public bool ApplyToClergyOnly { get; set; }
        public bool IsDeceased { get; set; }
        public bool IsAvailable { get; set; }
        ///
        /// only for CLA -- 17
        public bool IsMainlineActivity { get; set; }  // Mainline Church Activity ... Congregation-owned program ie. groups have their own programs too..
        public bool IsChurchService { get; set; }  // Church service like Sunday services
        ///
        /// only for CLARD -- 18
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TargetOccurences { get; set; }    // counts .. e.g. 3months pre-marital counselling
        [StringLength(1)]
        public string OccurFreqCode { get; set; }   // d-Daily, w-Weekly, m-Monthly, s-Semesterly, y-Yearly
        public bool IsRequired { get; set; }
        public bool IsSequenced { get; set; }
        ///
        public int? OrderIndex { get; set; }
        public bool? RequireUserCustom { get; set; }

        //  public int? OwnedByChurchBodyId { get; set; }

        [StringLength(1)]
        public string ApplyToMemberStatus { get; set; }

        [StringLength(1)]
        public string NVPStatus { get; set; }

        [StringLength(1)]
        public string SharingStatus { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped] // [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }

        [NotMapped] // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; } 

        [ForeignKey(nameof(NVPCategoryId))] 
        public virtual AppUtilityNVP NVPCategory { get; set; }

        //  public virtual List<AppUtilityNVP> InverseNVPCategories { get; set; }



        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))]
        public virtual ChurchBody OwnedByChurchBody { get; set; }

        [ForeignKey(nameof(CtryAlpha3Code))]
        public virtual Country Country_NVP { get; set; }


        //[NotMapped]
        //public string strNVPTag { get; set; }

        //[NotMapped][StringLength(10)]
        //public string strNVPStatus { get; set; }

        //[NotMapped]
        //public string strNVPCategory { get; set; }
    }
}
