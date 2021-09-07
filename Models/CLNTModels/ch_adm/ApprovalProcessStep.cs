using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ApprovalProcessStep
    {
        public ApprovalProcessStep()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }
        public int? ChurchBodyId { get; set; }


        public int ApprovalProcessId { get; set; }
        public int StepIndex { get; set; }
        [StringLength(7)]
        public string ProcessStepName { get; set; }
        [StringLength(100)]
        public string StepDesc { get; set; }
        
        public bool IsConcurrentWithOther { get; set; }
        public int? ConcurrProcessStepId { get; set; }

       //// public int? ApproverChurchRoleId { get; set; }  // tie to the person, not role :- role may be empty!
         
       // public int? Approver1ChurchBodyId { get; set; }     /// principal
       // public int? Approver1ChurchMemberId { get; set; }    /// subsidiary 
       // public int? Approver1ChurchRoleId { get; set; }
       // public int? Approver1MemberChurchRoleId { get; set; }  // both member profile and role of the person... Dr Sam Darteh - Dis Min at East Legon Hills... pick using CBid and CMid

       // public int? Approver2ChurchBodyId { get; set; }
       // public int? Approver2ChurchMemberId { get; set; }
       // public int? Approver2ChurchRoleId { get; set; }
       // public int? Approver2MemberChurchRoleId { get; set; }  // both member profile and role of the person... Dr Sam Darteh - Dis Min at East Legon Hills... pick using CBid and CMid
       
       // public int? EscalChurchBodyId { get; set; }
       // public int? EscalChurchMemberId { get; set; }
       // public int? EscalChurchRoleId { get; set; }
       // public int? EscalMemberChurchRoleId { get; set; }  // both member profile and role of the person... Dr Sam Darteh - Dis Min at East Legon Hills... pick using CBid and CMid
           

        [StringLength(1)]
        public string StepStatus { get; set; } 

        [StringLength(1)]
        public string SharingStatus { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]  //  [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped] // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(ApprovalProcessId))]
        public virtual ApprovalProcess ApprovalProcess { get; set; }
         

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(OwnedByChurchBodyId))]
        public virtual ChurchBody OwnedByChurchBody { get; set; }

        [ForeignKey(nameof(ConcurrProcessStepId))]
         public virtual ApprovalProcessStep ConcurrProcessStep { get; set; }

        // public virtual List<ApprovalProcessStep> InverseConcurrProcessStep { get; set; }

        //[ForeignKey(nameof(ApproverChurchRoleId))]
        //public virtual ChurchRole ApproverChurchRole { get; set; }


        //[ForeignKey(nameof(Approver1ChurchBodyId))]
        //public virtual ChurchBody Approver1ChurchBody { get; set; }

        //[ForeignKey(nameof(Approver1ChurchMemberId))]
        //public virtual ChurchMember Approver1ChurchMember { get; set; }

        //[ForeignKey(nameof(Approver1ChurchRoleId))]
        //public virtual ChurchRole Approver1ChurchRole { get; set; }

        //[ForeignKey(nameof(Approver1MemberChurchRoleId))]
        //public virtual MemberChurchRole Approver1MemberChurchRole { get; set; }


        //[ForeignKey(nameof(Approver2ChurchBodyId))]
        //public virtual ChurchBody Approver2ChurchBody { get; set; }

        //[ForeignKey(nameof(Approver2ChurchMemberId))]
        //public virtual ChurchMember Approver2ChurchMember { get; set; }

        //[ForeignKey(nameof(Approver2ChurchRoleId))]
        //public virtual ChurchRole Approver2ChurchRole { get; set; }

        //[ForeignKey(nameof(Approver2MemberChurchRoleId))]
        //public virtual MemberChurchRole Approver2MemberChurchRole { get; set; }


        //[ForeignKey(nameof(EscalChurchBodyId))]
        //public virtual ChurchBody EscalChurchBody { get; set; }

        //[ForeignKey(nameof(EscalChurchMemberId))]
        //public virtual ChurchMember EscalChurchMember { get; set; }

        //[ForeignKey(nameof(EscalChurchRoleId))]
        //public virtual ChurchRole EscalChurchRole { get; set; }

        //[ForeignKey(nameof(EscalMemberChurchRoleId))]
        //public virtual MemberChurchRole EscalMemberChurchRole { get; set; }
    }
}
