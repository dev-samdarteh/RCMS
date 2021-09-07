using RhemaCMS.Models.Adhoc;
//using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class MemberChurchlifeActivity
    {
        public MemberChurchlifeActivity() { }

        [Key]
        public int Id { get; set; }
        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }
      //  public int? OwnedByChurchBodyId { get; set; }
        public int? ChurchMemberId { get; set; }

        public bool IsChurchEvent { get; set; }  // If churchEvent... link to the church event created... else spell out the details of the activity/ocasion ... connecting the member 
        public int? ChurchEventId { get; set; }  //ex. Wedding event  ... calendar

        // Activity NOT church event... but certainly a church-related activity... provide necessary details
        public int? ChurchlifeActivityId { get; set; }

        [DataType(DataType.Date)]    
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? EventDate { get; set; }   /// loads the events

         
        /// Who performed the activity on that day...
       // public bool IsHostedByChurchVenue { get; set; }   // Yes... then link to the congregation configured
        public string HostVenueScope { get; set; }   //L-local cong, C- Chu Associate..ext cong but within denom, E [not church fellow]
        public int? VenueChurchBodyId { get; set; }    /// denomination reach--- CBid
        [StringLength(100)]
        public string OfficiatingVenueExt { get; set; }  // HostVenueScope = E ... = F, Riis Chapel, Methodist Church of Ghana
                                                         //  public bool IsOfficiatedByChurchFellowCode { get; set; }   // Yes... then link to the pastor's profile

        [StringLength(1)]
        public string OfficiatedByScope { get; set; }   // L-local cong, C- Chu Associate..ext cong but within denom, E [not church fellow]
        public int? OfficiatedByChurchBodyId { get; set; }    // RelationScope == L /C
        public int? OfficiatedByChurchMemberId { get; set; }  // RelationScope == L /C
        public int? MemberChurchRoleId { get; set; }  // both member profile and role of the person... Dr Sam Darteh - Dis Min at East Legon Hills... pick using CBid and CMid

        // Else give the discrete details...
        [StringLength(100)]
        public string OfficiatedByNameExt { get; set; }  // OfficiatedByScope = E, Pastor Willie Bucks  but Ext.ernal to the system
        [StringLength(100)]
        public string OfficiatedByRoleExt { get; set; }  // IsOfficiatedByChurchFellow = F, Minister-in-charge, Hatso PCG
        
         
        ///
        public string EventPhotoUrl { get; set; }   // scanned documents or event pictures can be attached here [ jux 1 per event/activity ]... ex. Baptismal cards, Marriage certs, Citations, Awards etc. 

        [StringLength(300)]
        public string Notes { get; set; }

        [StringLength(1)]
        public string SharingStatus { get; set; }  /// church role can be both local or in higher /lower congregations ... actually: role cud be anywhere [history], plus current roles [curr CB or elsewhere within church]
                                                   /// N- Do NOT share, C-share with Child CB only [below], D- Share with all sub congregations [down, descendants],  P- Share with Parent congregation [above], 
                                                   /// H- Share with all parent congregations - oversee [up, ancestor, forefather, Head CB], R- Share with congregations on same ROUTE [line], A- Share with all congregations within denomination [<denom.name>] 


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped]// [ForeignKey(nameof(CreatedByUserId))]
        public virtual MSTRModels.UserProfile CreatedByUser { get; set; }

        [NotMapped]// [ForeignKey(nameof(LastModByUserId))]
        public virtual MSTRModels.UserProfile LastModByUser { get; set; }



        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner  AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }

        //[NotMapped] // [ForeignKey(nameof(OwnedByChurchBodyId))] 
        //public virtual ChurchBody OwnedByChurchBody { get; set; }


        [ForeignKey(nameof(VenueChurchBodyId))]
        public virtual ChurchBody VenueChurchBody { get; set; }

        [ForeignKey(nameof(ChurchEventId))]
        public virtual ChurchCalendarEvent ChurchEvent { get; set; }

        [ForeignKey(nameof(ChurchlifeActivityId))]
        public virtual AppUtilityNVP  ChurchlifeActivity { get; set; }  //ChurchlifeActivity  -- code: CLA

        [ForeignKey(nameof(ChurchMemberId))]
        public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(OfficiatedByChurchBodyId))]
        public virtual ChurchBody OfficiatedByChurchBody { get; set; }

        [ForeignKey(nameof(OfficiatedByChurchMemberId))]
        public virtual ChurchMember OfficiatedByChurchMember { get; set; }

        [ForeignKey(nameof(MemberChurchRoleId))]
        public virtual MemberChurchRole MemberChurchRole { get; set; }


    }
}
