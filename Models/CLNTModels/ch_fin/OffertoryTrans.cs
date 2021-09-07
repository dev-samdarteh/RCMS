using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public class OffertoryTrans
    {
        [Key]
        public long Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
     
        [Display(Name = "Account Period")]
        public int? ChurchPeriodId { get; set; }

        [Display(Name = "Currency")]
        //public int? CurrencyId { get; set; }
        [StringLength(3)]
        public string Curr3LISOSymbol { get; set; }

        [StringLength(3)]
        public string CtryAlpha3Code { get; set; }

      

        [Display(Name = "Offertory Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? OffertoryDate { get; set; }
        public int? OffertoryTypeId { get; set; }
        [Display(Name = "Church Event")]
        public int? RelatedEventId { get; set; }


        [Display(Name = "Amount Paid")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AmtPaid { get; set; }
       
        // [Display(Name = "Payment Mode")]
        //[StringLength(3)]
        //public string PmntMode { get; set; }   // CSH, CHQ, MMO
        //[Display(Name = "Payment Ref No")]
        //[StringLength(15)]
        //public string PmntModeRefNo { get; set; }   // CSH, CHQ, MMO


        [Display(Name = "Received By")]
        public int? ReceivedByUserId { get; set; }
        // [StringLength(100)]
        //  public string PmntRefPers { get; set; }

        [Display(Name = "Date Posted")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? PostedDate { get; set; }
        [Display(Name = "Posting")]
        [StringLength(1)]
        public string PostStatus { get; set; }  //O-pen P-osted to GL R-eversed


        [StringLength(100)]
        public string Comments { get; set; }

        [StringLength(1)]
        public string Status { get; set; }  // A - Active, D - Deactive  

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        //
        //
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

        //[ForeignKey(nameof(CurrencyId))]
        //public virtual Currency Currency { get; set; }  

        [ForeignKey(nameof(CtryAlpha3Code))]
        public virtual Country CurrencyCountry { get; set; }

         
        [ForeignKey(nameof(OffertoryTypeId))]
        public virtual AppUtilityNVP OffertoryType { get; set; }

        [ForeignKey(nameof(RelatedEventId))]
        public virtual ChurchCalendarEvent RelatedEvent { get; set; }

        [NotMapped]// [ForeignKey(nameof(ReceivedByUserId))]
        public virtual UserProfile ReceivedByUser { get; set; }

    }
}



