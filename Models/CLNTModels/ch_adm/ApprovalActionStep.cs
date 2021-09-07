using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ApprovalActionStep
    {
        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; } 
        public int? OwnedByChurchBodyId { get; set; }
       public int? ChurchBodyId { get; set; }

        public int? ApprovalActionId { get; set; }
       
        [StringLength(100)]
        public string ActionStepDesc { get; set; }

        public int ApprovalStepIndex { get; set; }

        [StringLength(1)]
        public string ActionStepStatus { get; set; }

        [StringLength(100)]
        public string Comments { get; set; }
        public bool IsCurrentStep { get; set; }

        // public int? MemberChurchRoleId { get; set; }

        public int? ApprovalProcessStepRefId { get; set; }  /// ref to the Step defined...

        public int? ApproverChurchBodyId { get; set; }     /// principal
        public int? ApproverChurchMemberId { get; set; }    /// subsidiary 
        public int? ApproverChurchRoleId { get; set; }
        public int? ApproverMemberChurchRoleId { get; set; }
        
        [StringLength(1)]
        public string Status { get; set; }

        // public int? ActionByMemberChurchRoleId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ActionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StepRequestDate { get; set; }

        [StringLength(1)]
        public string CurrentScope { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped] // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


       // [ForeignKey(nameof(ActionByMemberChurchRoleId))] 
       //// [InverseProperty("ApprovalActionStepActionBy")]
       // public virtual MemberChurchRole ActionByMemberChurchRole { get; set; }   // Approver

        [ForeignKey(nameof(ApprovalActionId))] 
        public virtual ApprovalAction ApprovalAction { get; set; }
         

        [ForeignKey(nameof(ChurchBodyId))] 
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))]
        public virtual ChurchBody OwnedByChurchBody { get; set; }

       // [ForeignKey(nameof(MemberChurchRoleId))]
       //// [InverseProperty("ApprovalActionStepAction")]
       // public virtual MemberChurchRole MemberChurchRole { get; set; }


        [ForeignKey(nameof(ApprovalProcessStepRefId))]
        public virtual ApprovalProcessStep ApprovalProcessStepRef { get; set; }


        [ForeignKey(nameof(ApproverChurchBodyId))]
        public virtual ChurchBody ApproverChurchBody { get; set; }

        [ForeignKey(nameof(ApproverChurchMemberId))]
        public virtual ChurchMember ApproverChurchMember { get; set; }

        [ForeignKey(nameof(ApproverChurchRoleId))]
        public virtual ChurchRole ApproverChurchRole { get; set; }

        [ForeignKey(nameof(ApproverMemberChurchRoleId))]
        public virtual MemberChurchRole ApproverMemberChurchRole { get; set; }
         

    }
}
