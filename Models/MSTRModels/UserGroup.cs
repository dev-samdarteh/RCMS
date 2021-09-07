using RhemaCMS.Models.Adhoc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.MSTRModels
{
    public partial class UserGroup
    {
        public UserGroup()
        {  }

        public UserGroup(int? id, int? appGlobalOwnId, int? churchBodyId, string groupType, string groupName, string groupDesc, int groupLevel, int? groupCategoryId, string status, string ownedBy,
            DateTime? created, DateTime? lastMod, int? createdByUserId, int? lastModByUserId)
        {
            Id = (int)id;
            AppGlobalOwnerId = appGlobalOwnId;
            ChurchBodyId = churchBodyId; 
            UserGroupCategoryId = groupCategoryId; 
            Status = status;  
            GroupName = groupName;
            GroupType = groupType;
            GroupLevel = groupLevel;
            GroupDesc = groupDesc;
            OwnedBy = ownedBy;
            Created = created;
            LastMod = lastMod;
            CreatedByUserId = createdByUserId;
            LastModByUserId = lastModByUserId;
        }

        [Key]
        public int Id { get; set; } 
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }

        //[StringLength(50)]
        //public string CustomGroupName { get; set; }   // [CH_CUST | CF_CUST] Tech-operators, Catechists,. Secretaries, Treasurers etc

        public int GroupLevel { get; set; }  // SADMN - 1, ... CH_ADMN-6, CH_RGSTR-7, CH_ACCT-8, CH_WKR-9, CH_CLG-10, CH_LAY-11, CH_CLD-12, CH_CUST-13 ..15
                                             // CF_ADMN-16, CF_RGSTR-17, CF_ACCT-18, CF_WKR-19, CF_CLG-20, CF_LAY-21, CF_CLD-22, CF_CUST-23 ... 25
        [StringLength(10)]
        public string GroupType { get; set; }

        [StringLength(100)]
        public string GroupDesc { get; set; }
        public int? UserGroupCategoryId { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [StringLength(1)]
        public string OwnedBy { get; set; }  // SYS / CUS

        //public int? UserProfileGroupId { get; set; }
        //public int? UserProfileId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]
       // [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]
        //[ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(ChurchBodyId))] 
        public virtual MSTRChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(UserGroupCategoryId))] 
        public virtual UserGroup UserGroupCategory { get; set; }
         

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual MSTRAppGlobalOwner AppGlobalOwner { get; set; }

        //[ForeignKey(nameof(UserProfileId))] 
        //public virtual UserProfile UserProfile { get; set; }
        //[ForeignKey(nameof(UserProfileGroupId))] 
        //public virtual UserProfileGroup UserProfileGroup { get; set; } 

        //public virtual List<UserGroup> InverseUserGroupCategory { get; set; } 
        //public virtual List<UserGroupPermission> UserGroupPermission { get; set; } 
        //public virtual List<UserProfileGroup> UserProfileGroupNavigation { get; set; }


        [NotMapped]
        public bool bl_IsGroupAssigned { get; set; }

        [NotMapped]
        public string strGroupName { get; set; }   // concat with custom
    }
}
