﻿using RhemaCMS.Models.Adhoc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.MSTRModels
{
    public partial class UserRolePermission
    {
        public UserRolePermission()
        {  }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
        public int? UserRoleId { get; set; }
        public int? UserPermissionId { get; set; }
        [StringLength(1)]
        public string Status { get; set; }  
        //
        public bool ViewPerm { get; set; }  // Read
        public bool CreatePerm { get; set; }   //Add
        public bool EditPerm { get; set; }   //Modify
        public bool DeletePerm { get; set; }   //Remove
        public bool ManagePerm { get; set; }   //Full control
        //
        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }

        [NotMapped] //    [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }

        [NotMapped]   // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }

        [ForeignKey(nameof(ChurchBodyId))] 
        public virtual MSTRChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual MSTRAppGlobalOwner AppGlobalOwner { get; set; } 


        [ForeignKey(nameof(UserPermissionId))] 
        public virtual UserPermission UserPermission { get; set; } //public virtual UserPermission UserPermissionNavigation { get; set; }
        [ForeignKey(nameof(UserRoleId))] 
        public virtual UserRole UserRole { get; set; } 

        public virtual List <UserPermission> UserPermissions { get; set; }


        [NotMapped]
        public string strRoleName { get; set; }
        [NotMapped]
        public string strPermName { get; set; }
        //[NotMapped]
        //public string strSTRT { get; set; }
        //[NotMapped]
        //public string strEXPR { get; set; }
        //[NotMapped]
        //public bool isRoleAssigned { get; set; }
    }
}
