﻿using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RhemaCMS.Models.ViewModels.vm_app_ven
{
    public class LogOnVM
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Verification code")]
        public string VerificationCode { get; set; }
        public string SentVerificationCode { get; set; }

        [Required]
        [Display(Name = "Church code")]
        public string ChurchCode { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        // public int BasicCredentialsVerified { get; set; }
        public string strLogUserDesc { get; set; }
        public int IsVal { get; set; }   // 0 - false, 1 -true... T-true, F-false

        public string strChurchSlogan { get; set; }

        public string currChurchCode { get; set; }
        public string currUsername { get; set; }
        public string currPassword { get; set; }

        public IList<UserProfile> UserProfiles { get; set; }
    }
}
