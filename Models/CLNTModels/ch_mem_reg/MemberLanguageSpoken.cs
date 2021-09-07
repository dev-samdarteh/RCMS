using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberLanguageSpoken
    {
        public MemberLanguageSpoken() { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; } 


        public int? ChurchMemberId { get; set; }
        public int? LanguageSpokenId { get; set; }         
        public int? ProficiencyLevel { get; set; }
        public bool IsPrimaryLanguage { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]//  [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]//  [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }


        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(LanguageSpokenId))] 
        public virtual AppUtilityNVP LanguageSpoken { get; set; }  //LanguageSpoken



    [NotMapped]
        public string strChurchMember { get; set; }
        [NotMapped]
        public string strLanguageSpoken { get; set; } 
        [NotMapped]
        public string strProficiencyLevel { get; set; } 
        [NotMapped]
        public string strPrimaryLanguage { get; set; }
    }
}
