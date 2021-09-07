using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberChurchRole
    {
        public MemberChurchRole()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; } /// each CB should maintain their leadership history... ex. Moderator should exist only in the local CB  and not  at HQ of the leader... local should pick it up for view

       // public int? OwnedByChurchBodyId { get; set; }
        ///
        public int? ChurchMemberId { get; set; }
        public int? ChurchRoleId { get; set; }  // filter role [TM, CP] based on selected Church sector/body

        [StringLength(2)]
        public string OrgType { get; set; }    // role [TM, CP] can be... church body [CR, CH, CN] or church unit [ GB, IB, CO, CG, CE, SC, DP]
        public bool IsChurchUnit { get; set; }   // link to ChurchBody if NOT else to ChurchUnit  ... mainstream CB [thus host CB]
       // public int? AssociatedChurchBodyId { get; set; }   // Role in ... CR, CH, CF...  thus role is bn performed in the main church body like congregation [pastor], district [coord] or council [chair]
        public int? ChurchUnitId { get; set; }   // Role in ... CO, CG, CE, SC, DP  ... general :- President of Men's, Choirmaster of Choir, Chairperson/Member of Educ Committee, Organizer of Ladies Club etc
        
        public bool IsLeadRole { get; set; }  // primary/core/lead job ... ex. Presbyter/elder of congregation may be lead role compared to Secretary of a group
        public bool IsCurrentRole { get; set; }

        [StringLength(10)]
        public string BatchCode { get; set; }   // thus... batch of leaders e.g. 2020 Session, 2019 Executive Council etc.

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToDate { get; set; }

        [StringLength(50)]
        public string DepartReason { get; set; }

        [StringLength(200)]
        public string RoleProfile { get; set; }

        //[Required]
        //public string Discriminator { get; set; }
        //  public int? AttachedChurchBodyId { get; set; }

        [StringLength(1)]
        public string SharingStatus { get; set; }  /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                   /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                   /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 

        public string RolePhotoUrl { get; set; }   // historic photo


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

        //[ForeignKey(nameof(OwnedByChurchBodyId))] 
        //public virtual ChurchBody OwnedByChurchBody { get; set; }
         

        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(ChurchUnitId))] 
        public virtual ChurchUnit ChurchUnit { get; set; }

       
        //[ForeignKey(nameof(AssociatedChurchBodyId))]
        //public virtual ChurchBody AssociatedChurchBody { get; set; } 
        

        [ForeignKey(nameof(ChurchRoleId))] 
        public virtual ChurchRole ChurchRole { get; set; } 



        [NotMapped]
        public string strChurchRole { get; set; } 
        [NotMapped]
        public string strRoleUnit { get; set; } 
        [NotMapped]
        public string strCommenced { get; set; } 
        [NotMapped]
        public string strCompleted { get; set; } 


        //public virtual ApprovalActionStep ApprovalActionStepActionBy { get; set; } 
        //public virtual ApprovalActionStep ApprovalActionStepAction { get; set; }
        //public virtual ChurchTransfer ChurchTransfer_MemberChurchRole { get; set; }
        //public virtual ChurchTransfer ChurchTransfer_RequestorRole { get; set; }

        //public virtual List<ApprovalActionStep> ApprovalActionStepActionByMemberChurchRoles { get; set; } 
        //public virtual List<ApprovalActionStep> ApprovalActionStepMemberChurchRoles { get; set; } 
        //public virtual List<ChurchTransfer> ChurchTransferFromMemberChurchRoles { get; set; } 
        //public virtual List<ChurchTransfer> ChurchTransferRequestorRoles { get; set; } 
        //public virtual List<EventActivityReqLog> EventActivityReqLogs { get; set; } 
        //public virtual List<MemberChurchlifeActivity> MemberChurchlifeActivities { get; set; }
    }
}
