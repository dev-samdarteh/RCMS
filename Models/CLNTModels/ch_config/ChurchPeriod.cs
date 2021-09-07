using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ChurchPeriod
    {
        public ChurchPeriod()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [StringLength(50)]
        public string PeriodDesc { get; set; }  // Year 2021, 2021-Q1
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToDate { get; set; }

        public int  Year { get; set; }  //  2021 , 2021
        public int? Semester { get; set; } //  1 - Jan... Jun, 2 -- Jun - Dec
        public int? Quarter { get; set; } //  1 - Jan... Mar
        public int? Month { get; set; } //  Jan - 1
        public int? Week { get; set; } //  1

        public int Interval { get; set; }
        [StringLength(1)]
        public string IntervalFreq { get; set; }  // Y-year, M-month, S-semester, Q-quarter, W-week, D-day    
        public int IntervalDays { get; set; }
       // public bool IsStartingPeriod { get; set; }  // add interval to the start date == From ... get the Month(int) to configure 12-mon IntervalDefinition table
        
        [StringLength(2)]
        public string PeriodType { get; set; }      //  AP--Accounting Period, CC -- Church Calendar    ....., Period Definition

        [StringLength(1)]
        public string Status { get; set; }   // D-IntervalDefinition, A-ctive, B-locked, Previous [= recent deactive], Deactive,  CURRENT YEAR ??

        [StringLength(1)]
        public string SharingStatus { get; set; } 

        public int? OwnedByChurchBodyId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]
        // [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped] // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }

        // public virtual List<MemberRegistration> MemberRegistration { get; set; }

        //[NotMapped]
        //public string strFrom { get; set; }
        //[NotMapped]
        //public string strTo { get; set; }
        //[NotMapped]
        //public string strStatus{ get; set; }
        //[NotMapped]
        //public string strSharingStatus { get; set; } 
        //[NotMapped]
        //[StringLength(1)]
        //public string strOwnerStatus { get; set; }  // I -- Inherited, O -- Originated   i.e. currChurchBody == OwnedByChurchBody
        //[NotMapped]
        //public string strOwnerStatusDesc { get; set; }
        //[NotMapped]
        //public string strOwnerChurchBody { get; set; }

    }
}
