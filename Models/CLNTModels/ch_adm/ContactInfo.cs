﻿using RhemaCMS.Models.CLNTModels;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ContactInfo
    {
        public ContactInfo()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }


        [StringLength(50)]  // Id or sth
        public string ContactInfoDesc { get; set; }
        public bool IsChurchBody { get; set; }   // if church unit... contact info CB
        public bool IsChurchUnit { get; set; }   // if church unit... contact info for departments etc.
        public bool IsChurchFellow { get; set; }   // if church member... memId
        public int? ChurchMemberId { get; set; }   // unmap!  ...jux for referencing ... to allow for multiple contact details
        ///
        public int? ChurchUnitId { get; set; }  ///  jux ref to the unit bearing the CI

        [StringLength(100)]
        public string ExtHolderName { get; set; }  // not church member

        [StringLength(100)]
        public string ResidenceAddress { get; set; }
        [StringLength(30)]
        public string Location { get; set; }
        [StringLength(30)]
        public string City { get; set; }
        public int? RegionId { get; set; }


        [StringLength(3)]
        public string CtryAlpha3Code { get; set; }
        //   public int? CountryId { get; set; }
        public bool ResAddrSameAsPostAddr { get; set; }
        [StringLength(30)]
        public string PostalAddress { get; set; }
        [StringLength(30)]
        public string DigitalAddress { get; set; }
        [StringLength(15)]
        public string Telephone { get; set; }
        [StringLength(15)]
        public string MobilePhone1 { get; set; }
        [StringLength(15)]
        public string MobilePhone2 { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public bool IsPrimaryContact { get; set; }  // to allow for multiple addresses... later

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }



        [NotMapped] // [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped] //[ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }


       //[NotMapped] // [ForeignKey(nameof(ChurchMemberId))]  
       // public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(CtryAlpha3Code))] 
        public virtual Country Country { get; set; }

        [ForeignKey(nameof(RegionId))] 
        public virtual CountryRegion CountryRegion { get; set; }

        //[InverseProperty("ContactInfo")]
        //public virtual List<ChurchBody> ChurchBodies { get; set; }
         
         
    }
}
