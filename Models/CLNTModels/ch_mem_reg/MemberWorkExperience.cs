using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberWorkExperience
    {
        public MemberWorkExperience() { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }
        ///
        public int? ChurchMemberId { get; set; }
       // [Required]
        [StringLength(50)]
        public string WorkPlace { get; set; }

        [StringLength(3)]
        public string CtryAlpha3Code { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        //[Required]
        [StringLength(50)]
        public string WorkRole { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToDate { get; set; }

        [StringLength(100)]
        public string Reason { get; set; } 
        public bool IsCurrentWork { get; set; }
        
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }
         

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }
         

        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }


        [ForeignKey(nameof(CtryAlpha3Code))]
        public virtual Country Country { get; set; }

        //[NotMapped]
        //public string strStarted { get; set; }
        //[NotMapped]
        //public string strEnded { get; set; }
    }
}
