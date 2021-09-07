//using RhemaCMS.Models.Adhoc;
////using RhemaCMS.Models.MSTRModels;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace RhemaCMS.Models.CLNTModels
//{
//    public partial class MemberChurchlifeEvent
//    {
//        [Key]
//        public int Id { get; set; }
//        public int? AppGlobalOwnerId { get; set; }
//        public int? ChurchBodyId { get; set; }
//        public int? OwnedByChurchBodyId { get; set; }
//        public int? ChurchMemberId { get; set; }

//        public bool IsChurchEvent { get; set; }  // If churchEvent... link to the church event created... else spell out the details of the activity/ocasion ... connecting the member
//        public int? ChurchEventId { get; set; }      
//        public int? ChurchlifeId { get; set; }

//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
//        public DateTime? EventDate { get; set; }

//        [StringLength(50)]
//        public string Venue { get; set; }
//        public int? MemberChurchRoleId { get; set; }

//        [StringLength(100)]
//        public string OfficiatedByName { get; set; }  // Pastor Willie
//        [StringLength(100)]
//        public string OfficiatedByRole { get; set; }  // Minister in charge

//        [StringLength(100)]
//        public string ActivityVenue { get; set; }
        

//        public bool IsOfficiatedByChurchFellow { get; set; }

//        /// 
//        public int? VenueChurchBodyId { get; set; } 
//        [StringLength(100)]
//        public string CongregationExt { get; set; } 
//        public bool IsOfficiatedByExt { get; set; } 

//        [StringLength(100)]
//        public string OfficiatedByCongExt { get; set; } 
//        [StringLength(100)]
//        public string OfficiatedByNameExt { get; set; } 
//        [StringLength(100)]
//        public string OfficiatedByRoleExt { get; set; }

//        ///
//        public string PhotoUrl { get; set; }

//        [StringLength(300)]
//        public string Comments { get; set; }

//        public DateTime? Created { get; set; }
//        public DateTime? LastMod { get; set; }
//        public int? CreatedByUserId { get; set; }
//        public int? LastModByUserId { get; set; }

//        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
//        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }

//        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
//        public virtual MSTRModels.UserProfile LastModByUser { get; set; }



//        [ForeignKey(nameof(AppGlobalOwnerId))]
//        public virtual AppGlobalOwner CountryAppGlobalOwner { get; set; }

//        [ForeignKey(nameof(ChurchBodyId))]
//        public virtual ChurchBody ChurchBody { get; set; }

//        [NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
//        public virtual ChurchBody OwnedByChurchBody { get; set; }


//        [ForeignKey(nameof(VenueChurchBodyId))]
//        public virtual ChurchBody VenueChurchBody { get; set; }

//        [ForeignKey(nameof(ChurchEventId))]
//        public virtual ChurchCalendarEvent ChurchEvent { get; set; }

//        [ForeignKey(nameof(ChurchlifeId))]
//        public virtual Churchlife Churchlife { get; set; }

//        [ForeignKey(nameof(ChurchMemberId))]
//        public virtual ChurchMember ChurchMember { get; set; }

//        [ForeignKey(nameof(MemberChurchRoleId))]
//        public virtual MemberChurchRole MemberChurchRole { get; set; }
//    }
//}
