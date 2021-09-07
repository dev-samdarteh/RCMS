using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ChurchMember
    {
        public ChurchMember()
        { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? OwnedByChurchBodyId { get; set; }  // created the oCM ... will be fixed thru out lifecycle
        public int? ChurchBodyId { get; set; }  // to be changed per transfer and movement of Member within denomination
        ///

        //[StringLength(50)]
        //public string MemberCode { get; set; }  // RCM0000001 :- Member Global Code -- same across denomination... to allow transfers and retaining of member data

        //[Display(Name = "Relative Member Code")]
        //[StringLength(200)]
        //public string RootChurchCode { get; set; }  // READ ONLY --- ChurchCodeFullPath ::>> RCM-000001--RCM-000001--RCM-000001--RCM-000001--M0000001


        [Display(Name = "Member Code")]   // READ-ONLY --- GlobalChurchCode ::>>  [ RootChurchCode /Acronym ] - [7-digit] ... RCM0000000, RCM0000001, PCG1234567, COP1000000, ICGC9999999
        [StringLength(30)]
        public string GlobalMemberCode { get; set; }

        //[Display(Name = "Relative Member Code")]
        //[StringLength(200)]
        //public string RootMemberCode { get; set; }  // READ ONLY --- ChurchCodeFullPath + MemberCode ::>> RCM-000001/0011--RCM-000001/0012--RCM-000001/0013--RCM-000001/0014

       // [Display(Name = "Custom Member Code")]
        //[StringLength(20)]
        //public string MemberCodeCustom { get; set; }  // RCM0000001 :- Member Global Code -- same across denomination... to allow transfers and retaining of member data
          

        [StringLength(50)]
        public string MemberCustomCode { get; set; }

        //[StringLength(50)]
        //public string MemberGlobalId { get; set; }

        [StringLength(15)]
        public string Title { get; set; }

        [StringLength(30)]
        public string FirstName { get; set; }
        [StringLength(30)]
        public string MiddleName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(100)]
        public string MaidenName { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DateOfBirth { get; set; }

       // [Required]
        [StringLength(1)]
        public string MaritalStatus { get; set; } // Single, Married, Separated, Divorced, Widowed, Other

        [StringLength(1)]
        public string MarriageType { get; set; }  // A--Church-Ordinance, B--Court-Ordinance, C--Customary,  NULL --None 

        [StringLength(50)]
        public string MarriageRegNo { get; set; }

        public int? PrimContactInfoId { get; set; }  // Pho, email, website, loca etc.

        [StringLength(3)]
        public string NationalityId { get; set; }  //CtryAlpha3Code e.g. GHA

        public int? MotherTongueId { get; set; }

        [StringLength(50)]
        public string Hometown { get; set; }
        public int? HometownRegionId { get; set; }

        public int? IdTypeId { get; set; }
        [StringLength(50)]
        public string National_IdNum { get; set; } 

        [StringLength(50)]
        public string Hobbies { get; set; }

        public string PhotoUrl { get; set; }

        [StringLength(300)]
        public string Notes { get; set; }

        //  public bool IsActivated { get; set; }  

        [StringLength(1)]        
        public string MemberScope { get; set; } // member class/category -- [I]nternal members - M, P, L, C /// [E]xternal members - N, A, G


        [StringLength(1)]
        public string Status { get; set; } // A - Active, In Transit, H-On Hold/B-Blocked, D-Deactive, X-Closed...

        [StringLength(100)]
        public string StatusReason { get; set; } // why??? 
         

        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped] //[ForeignKey(nameof(CreatedByUserId))]
        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }

       [NotMapped]//[ForeignKey(nameof(LastModByUserId))]
        public virtual MSTRModels.UserProfile LastModByUser { get; set; }

        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        public virtual ChurchBody OwnedByChurchBody { get; set; }
                 
        [ForeignKey(nameof(HometownRegionId))] 
        public virtual CountryRegion HometownRegion { get; set; }

        [ForeignKey(nameof(IdTypeId))] 
        public virtual AppUtilityNVP IdType { get; set; }  // National_IdType

        [ForeignKey(nameof(MotherTongueId))] 
        public virtual AppUtilityNVP MotherTongue { get; set; } // LanguageSpoken

        [ForeignKey(nameof(NationalityId))] 
        public virtual Country Nationality { get; set; } 

        [ForeignKey(nameof(PrimContactInfoId))] 
        public virtual ContactInfo ContactInfo { get; set; } // collection of details


        [NotMapped]
        public int numMemAge { get; set; }


        [NotMapped]
        public string strMemFullName { get; set; }

        // [NotMapped]
        // public int? CurrMemberTypeId { get; set; }


        //[NotMapped]
        //public virtual List<ChurchAttendAttendees> ChurchAttendAttendees { get; set; }
        //[NotMapped]
        //public virtual List<ChurchBodyAssociate> ChurchBodyAssociates { get; set; }
        //[NotMapped]
        //public virtual List<ChurchEventActor> ChurchEventActors { get; set; }

        //[NotMapped] //  [InverseProperty("ChurchMember")]
        //public virtual List<ChurchTransfer> ChurchTransferChurchMembers { get; set; }
        //[NotMapped] // [InverseProperty("RequestorMember")]
        //public virtual List<ChurchTransfer> ChurchTransferRequestorMembers { get; set; }
        //[NotMapped]
        //public virtual List<ChurchVisitor> ChurchVisitors { get; set; }
        //[NotMapped]
        //public virtual List<ContactInfo> ContactInfos { get; set; }
        //[NotMapped]
        //public virtual List<EventActivityReqLog> EventActivityReqLogs { get; set; }
        //[NotMapped]
        //public virtual List<MemberChurchlife> MemberChurchlifes { get; set; }
        //[NotMapped]
        //public virtual List<MemberChurchlifeActivity> MemberChurchlifeActivities { get; set; }
        //[NotMapped]
        //public virtual List<MemberChurchSector> MemberChurchSectors { get; set; }
        //[NotMapped]
        //public virtual List<MemberChurchUnit> MemberChurchUnits { get; set; }
        //[NotMapped]  // [InverseProperty("ChurchMember")]
        //public virtual List<MemberContact> MemberContactChurchMembers { get; set; }
        //[NotMapped] // [InverseProperty("InternalContact")]
        //public virtual List<MemberContact> MemberContactInternalContacts { get; set; }
        //[NotMapped]
        //public virtual List<MemberEducHistory> MemberEducHistories { get; set; }
        //[NotMapped]
        //public virtual List<MemberLanguageSpoken> MemberLanguagesSpoken { get; set; }
        //[NotMapped]
        //public virtual List<MemberChurchRole> MemberChurchRoles { get; set; }
        //[NotMapped]
        //public virtual List<MemberPosition> MemberPositions { get; set; }
        //[NotMapped]
        //public virtual List<MemberProfessionBrand> MemberProfessionBrands { get; set; }
        //[NotMapped]
        //public virtual List<MemberRank> MemberRanks { get; set; }
        //[NotMapped]
        //public virtual List<MemberRegistration> MemberRegistrations { get; set; }
        //[NotMapped]   // [InverseProperty("ChurchMember")]
        //public virtual List<MemberRelation> MemberRelationChurchMembers { get; set; }
        //[NotMapped] // [InverseProperty("RelationChurchMember")]
        //public virtual List<MemberRelation> MemberRelationRelationChurchMembers { get; set; }
        //[NotMapped]
        //public virtual List<MemberStatus> MemberStatuses { get; set; }
        //[NotMapped]
        //public virtual List<MemberType> MemberTypes { get; set; }
        //[NotMapped]
        //public virtual List<MemberWorkExperience> MemberWorkExperiences { get; set; }
    }
}
