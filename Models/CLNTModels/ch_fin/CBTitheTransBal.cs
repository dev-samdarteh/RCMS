  
using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public class CBTitheTransBal
    {
        public CBTitheTransBal() { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [Display(Name = "Account Period")]
        public int? ChurchPeriodId { get; set; }


        [StringLength(3)]
        public string Curr3LISOSymbol { get; set; }

        [StringLength(3)]
        public string CtryAlpha3Code { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotAmtCol { get; set; }  // collected   ... 10, 000

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotAmtOut { get; set; }  // paid to next level ... 4, 000  ... disbursed [ per set %]

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotAmtNet { get; set; }  // net received  ... 6, 000

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = false)]
        public DateTime? DatePaid { get; set; }
         

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

        [ForeignKey(nameof(CtryAlpha3Code))]
        public virtual Country CurrencyCountry { get; set; }
    }
}





