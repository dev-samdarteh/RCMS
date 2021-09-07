﻿using RhemaCMS.Models.Adhoc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.MSTRModels
{
    public partial class UserRole
    {

        public UserRole()
        {  }

        public UserRole(int? id, int? appGlobalOwnId,  int? churchBodyId, string roleType, string roleName, string roleDesc, int roleLevel, string roleStatus, string ownedBy,
            DateTime? created, DateTime? lastMod, int? createdByUserId, int? lastModByUserId)
        {
            Id = (int)id ;
            AppGlobalOwnerId = appGlobalOwnId;
            ChurchBodyId = churchBodyId;
            RoleType = roleType;
            RoleDesc = roleDesc;
            RoleStatus = roleStatus;
            RoleLevel = roleLevel;
            RoleName = roleName ; 
            OwnedBy = ownedBy;
            Created = created ;
            LastMod = lastMod ;
            CreatedByUserId = createdByUserId ;
            LastModByUserId = lastModByUserId ;
        }


        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }

        [StringLength(50)]
        public string RoleName { get; set; }

        //[StringLength(50)]
        //public string CustomRoleName { get; set; }   // [CH_CUST | CF_CUST] Pastor, Minister, Catechist,. Secretary, Treasurer etc

        [StringLength(100)]
        public string RoleDesc { get; set; }

        [StringLength(15)]
        public string RoleType { get; set; }  // 

        [StringLength(1)]
        public string RoleStatus { get; set; }
        public int RoleLevel { get; set; }

        [StringLength(1)]
        public string OwnedBy { get; set; }  // SYS / CUS

        //public int? UserProfileId { get; set; }
        //public int? UserProfileRoleId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped]   //  [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]  // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }

        [ForeignKey(nameof(ChurchBodyId))] 
        public virtual MSTRChurchBody ChurchBody { get; set; }

        
        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual MSTRAppGlobalOwner AppGlobalOwner { get; set; }

        //[ForeignKey(nameof(UserProfileId))] 
        //public virtual UserProfile UserProfile { get; set; }

        //[ForeignKey(nameof(UserProfileRoleId))] 
        //public virtual UserProfileRole UserProfileRole { get; set; }

        //   public virtual List <UserProfileRole> UserProfileRoles { get; set; } 
        //  public virtual List <UserRolePermission> UserRolePermissions { get; set; }


        [NotMapped]
        public bool bl_IsRoleAssigned { get; set; }

        [NotMapped]
        public string strRoleName { get; set; }

        [NotMapped]
        public string strUserProfileRole { get; set; }
    }
}
