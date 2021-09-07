﻿using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public class TitheTrans
    {
        [Key]
        public long Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [Display(Name = "Account Period")]
        public int? ChurchPeriodId { get; set; }

        [StringLength(3)]
        public string Curr3LISOSymbol { get; set; }    

        [StringLength(3)]
        public string CtryAlpha3Code { get; set; }  

        //public int? CurrencyId { get; set; }
        //[Display(Name = "Tithe Amount")]
        //[Column(TypeName = "decimal(18, 2)")]
        public decimal? TitheAmount { get; set; }   //this is the total amount paid...
        [Display(Name = "Tithe Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? TitheDate { get; set; }

        //  public int? RelatedEventId { get; set; }
        [Display(Name = "Tithe Mode")]
        [StringLength(1)]
        public string TitheMode { get; set; }   // M-Member-Specific, G-Church Group, E-External Body, A-Anonymous

        //
        [Display(Name = "Tither Scope")]
        [StringLength(1)]
        public string TithedByScope { get; set; }   // L-Local cong, D-Denomination, E-External Body 
        [Display(Name = "Church Member")]
        public int? ChurchMemberId { get; set; }
        [Display(Name = "Group /Corporate Body")]
        public int? Corporate_ChurchBodyId { get; set; }   //groups, associations etc.

        [Display(Name = "Tithed By")]
        [StringLength(100)]
        public string TitherDesc { get; set; }  // use member details based in the Tithe mode

        [Display(Name = "Received By")]
        public int? ReceivedByUserId { get; set; }

        //[Display(Name = "Payment Mode")]
        //[StringLength(3)]
        //public string PmntMode { get; set; }   // CSH, CHQ, MMO
        //[Display(Name = "Payment Ref No")]
        //[StringLength(15)]
        //public string PmntModeRefNo { get; set; }   // CSH, CHQ, MMO 

        [Display(Name = "Date Posted")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy}", ApplyFormatInEditMode = false)]        
        public DateTime? PostedDate { get; set; }
        [Display(Name = "Posting")]
        public string PostStatus { get; set; }  //Open Posted Cancelled
         

        [StringLength(100)]
        public string Comments { get; set; }   
        
        [StringLength(1)]
        public string Status { get; set; }  // A - Active, D - Deactive  

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

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

        [ForeignKey(nameof(ChurchMemberId))]
        public virtual ChurchMember ChurchMember { get; set; } 

        [ForeignKey(nameof(Corporate_ChurchBodyId))]
        public virtual ChurchMember Corporate_ChurchBody { get; set; }

        //[ForeignKey(nameof(RelatedEventId))]
        //public virtual ChurchCalendarEvent RelatedEvent { get; set; }

        [NotMapped]// [ForeignKey(nameof(ReceivedByUserId))]
        public virtual UserProfile ReceivedByUser { get; set; }
    }
}


