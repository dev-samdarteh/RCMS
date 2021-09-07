using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberChurchlifeEventTask
    {
        public MemberChurchlifeEventTask()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }
        ///
        public int? ChurchMemberId { get; set; }
      //  public int ChurchCalendarId { get; set; }
        //public int? ChurchEventId { get; set; }  //ex. Wedding event
        public int? MemberChurchlifeActivityId { get; set; }  //ex. Sam-Naming 1, Sam-Naming 2 event
        public int? RequirementDefId { get; set; }  // from setup.. expected to be met...  can be 1 [requirement step] to many [reality steps]
        public int? MemberChurchRoleId { get; set; }  //thus the leader... ex. counsellor in pre-marital counseling session.. [may access, do the scoring/comment] tie responsibility to person-role [ cos he/she will be performing some function ]
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateCommenced { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateCompleted { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(100)]
        public string Details { get; set; }

        //public bool IsTaskCompleted { get; set; }

        public bool IsCurrentTask { get; set; }

        public string PhotoUrl { get; set; }

        [StringLength(200)]
        public string Notes { get; set; }
        
        [StringLength(1)]
        public string TaskStatus { get; set; }   //  P-Pending, I-In Progress, D-Deactive, W-Waived, Complete

        [StringLength(1)]
        public string SharingStatus { get; set; }  /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                   /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                   /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }
        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
        public virtual MSTRModels.UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }


        [ForeignKey(nameof(RequirementDefId))]
        public virtual AppUtilityNVP ActivityRequirementDef { get; set; }  //code: CLARD ...ChurchlifeActivityReqDef


        //[ForeignKey(nameof(ChurchEventId))]
        //public virtual ChurchCalendarEvent ChurchEvent { get; set; }

        [ForeignKey(nameof(MemberChurchlifeActivityId))]
        public virtual MemberChurchlifeActivity MemberChurchlifeActivity { get; set; }


        [ForeignKey(nameof(ChurchMemberId))]
        public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(MemberChurchRoleId))]
        public virtual MemberChurchRole MemberChurchRole { get; set; }
    }
}
