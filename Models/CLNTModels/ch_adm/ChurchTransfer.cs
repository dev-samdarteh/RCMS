using RhemaCMS.Models.Adhoc;
using RhemaCMS.Models.MSTRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhemaCMS.Models.CLNTModels
{
    public partial class ChurchTransfer
    {
        public ChurchTransfer()
        { }

        public int? AppGlobalOwnerId { get; set; }
        public int? ChurchBodyId { get; set; }  

        [Key]
        public int Id { get; set; }
        // [Column("I_ApprovalActionId")]

        public int? RequestorChurchBodyId { get; set; } //Grace congregation, PCG Headquarters   ie. cong making request
        public int? RequestorMemberId { get; set; }  // Sam Darteh , Rev. Dr. Linda Taylor  ie. representative... or self-service member
        public int? RequestorRoleId { get; set; } // Session Clerk, GA Clerk  ... who is the requestor

        public int? FromChurchBodyId { get; set; } //Grace congregation, Christ The King congregation-Osu   ... ie congregation of the member /person to transfer or whose role is bn transferred
        public int? ChurchMemberId { get; set; }  //Change to Minister @Clergy Transfer
        public int? FromChurchRankId { get; set; }   //current position:- Presbyter...
        public int? FromMemberChurchRoleId { get; set; }   // current role:-- Snr Presbyter, Minister-in-charge

        public int? ToChurchBodyId { get; set; }  //Ramseyer congregation, Peyer Memorial congregation-Bantama   
        public int? ToChurchRoleId { get; set; }  /// what's he going to do there ?? Role...
        public int? ToRoleUnitId { get; set; } /// taht's in case role is effected in a church unit
         
        [StringLength(2)]
        public string TransferType { get; set; }  //MEMBER = MT, CLERGY/Ministers = CT, RT-Role Transfer... 

        [StringLength(3)]
        public string TransferSubType { get; set; } 
                
        public bool RequireApproval { get; set; }

        [StringLength(100)]
        public string TransferReason { get; set; }

        public int? TransMessageId { get; set; }  // preset messages

        [StringLength(100)]
        public string CustomTransMessage { get; set; }

        //[StringLength(50)]
        //public string CustomReason { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? RequestDate { get; set; }   // date the request for transfer was made


        [StringLength(1)]
        public string FrAckStatus { get; set; }

        [StringLength(100)]
        public string FrAckStatusComments { get; set; }
        public DateTime? FrReceivedDate { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToRequestDate { get; set; }  /// date the request was sent to the To-CB

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? ToReceivedDate { get; set; }   // date request was received at To-CB

        //[StringLength(1)]
        //public string AckStatus { get; set; }   // ...ToAckStatus    // N - None, P = Pending /Un-read, R = Received, X = Closed 
        //public string AckStatusComments { get; set; }

        [StringLength(1)]
        public string ApprovalStatus { get; set; }  //...link this to the ApprovalAction Status... P = Pending, [H = On Hold], I - In Progress, A = Completed /Done , F = Forced Completed, D = Unsuccessful /Denied, X- Cancelled   

        [StringLength(100)]
        public string ApprovalStatusComments { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? TransferDate { get; set; }  // date transfer occured [process + person]
        
        [StringLength(1)]
        public string CurrentScope { get; set; }  // I- internal, E- external
        public int? IApprovalActionId { get; set; }  //Internal ... to determine the status of the approval
        public int? EApprovalActionId { get; set; }   //External... to determine the status of the approval

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        //public DateTime? FromReceivedDate { get; set; }   // date request was received at From-CB

        [StringLength(1)]
        public string ReqStatus { get; set; }  // Overall status -- closed, done, failed etc.
         
        [StringLength(100)]
        public string ReqStatusComments { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d MMM yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? StatusModDate { get; set; }  // last date status changed... usually after 72hrs [3 days] ... X, R, Y, C, Z ---> auto-leave ToCB tray!

        [StringLength(1)]
        public string WorkSpanStatus { get; set; }  // Overall status -- deactive (D) :- not yet processed, active (A) :- show in tray , ... closed (C) :- processed and closed

        [StringLength(200)]
        public string Comments { get; set; }

        //[StringLength(1)]
        //public string MembershipStatus { get; set; }

        public int? AttachedToChurchBodyId { get; set; }
        public string AttachedToChurchBodyList { get; set; } 
        public string DesigRolesList { get; set; }

        /// temp holders ...
        public string TempMemTypeCodeFrCB { get; set; }   /// ... affirm from To-CB what's coming from From-CB
        public string TempMemTypeCodeToCB { get; set; }   /// ... affirm from To-CB what's coming from From-CB
        public int?   TempMemRankIdFrCB   { get; set; }     /// ... affirm from To-CB what's coming from From-CB
        public int?   TempMemRankIdToCB   { get; set; }     /// ... affirm from To-CB what's coming from From-CB
        public int?   TempMemStatusIdFrCB { get; set; }     /// ... affirm from To-CB what's coming from From-CB
        public int?   TempMemStatusIdToCB { get; set; }     /// ... affirm from To-CB what's coming from From-CB


        public DateTime? Created { get; set; }
        public DateTime? LastMod { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? LastModByUserId { get; set; }


        [NotMapped]
      //  [ForeignKey(nameof(CreatedByUserId))]
        public virtual UserProfile CreatedByUser { get; set; }
        [NotMapped]
       // [ForeignKey(nameof(LastModByUserId))]
        public virtual UserProfile LastModByUser { get; set; }


        [ForeignKey(nameof(AppGlobalOwnerId))]
        public virtual AppGlobalOwner AppGlobalOwner { get; set; }

        [ForeignKey(nameof(ChurchBodyId))]
        public virtual ChurchBody ChurchBody { get; set; }


        [ForeignKey(nameof(AttachedToChurchBodyId))] 
        public virtual ChurchBody AttachedToChurchBody { get; set; }
        [ForeignKey(nameof(ChurchMemberId))] 
        public virtual ChurchMember ChurchMember { get; set; }

        [ForeignKey(nameof(EApprovalActionId))] 
        public virtual ApprovalAction EApprovalAction { get; set; }
        [ForeignKey(nameof(FromChurchBodyId))] 
        public virtual ChurchBody FromChurchBody { get; set; }

        [ForeignKey(nameof(FromChurchRankId))] 
        public virtual AppUtilityNVP FromChurchPosition_NVP { get; set; }

        [ForeignKey(nameof(FromMemberChurchRoleId))]
      //  [InverseProperty("ChurchTransfer_MemberChurchRole")]
        public virtual MemberChurchRole FromMemberChurchRole { get; set; }
        [ForeignKey(nameof(IApprovalActionId))] 
        public virtual ApprovalAction IApprovalAction { get; set; }
        //[ForeignKey(nameof(ReasonId))] 
        //public virtual AppUtilityNVP Reason { get; set; }
        [ForeignKey(nameof(RequestorChurchBodyId))] 
        public virtual ChurchBody RequestorChurchBody { get; set; }
        [ForeignKey(nameof(RequestorMemberId))] 
        public virtual ChurchMember RequestorMember { get; set; }
        [ForeignKey(nameof(RequestorRoleId))]
      //  [InverseProperty("ChurchTransfer_RequestorRole")]
        public virtual MemberChurchRole RequestorRole { get; set; }
        [ForeignKey(nameof(ToChurchBodyId))] 
        public virtual ChurchBody ToChurchBody { get; set; }
        [ForeignKey(nameof(ToChurchRoleId))] 
        public virtual ChurchRole ToChurchRole { get; set; }
        [ForeignKey(nameof(ToRoleUnitId))] 
        public virtual ChurchUnit ToRoleUnit { get; set; }

        [ForeignKey(nameof(TransMessageId))] 
        public virtual AppUtilityNVP TransMessage { get; set; }
        

        //[ForeignKey(nameof(TempMemTypeCodeFrCB))] 
        //public virtual AppUtilityNVP TempMemTypeFrCB_NVP { get; set; }

        [ForeignKey(nameof(TempMemRankIdFrCB))]
        public virtual AppUtilityNVP TempMemRankFrCB_NVP { get; set; }

        [ForeignKey(nameof(TempMemStatusIdFrCB))]
        public virtual AppUtilityNVP TempMemStatusFrCB_NVP { get; set; }

        //[ForeignKey(nameof(TempMemTypeCodeToCB))]
        //public virtual AppUtilityNVP TempMemTypeToCB_NVP { get; set; }

        [ForeignKey(nameof(TempMemRankIdToCB))]
        public virtual AppUtilityNVP TempMemRankToCB_NVP { get; set; }

        [ForeignKey(nameof(TempMemStatusIdToCB))]
        public virtual AppUtilityNVP TempMemStatusToCB_NVP { get; set; }
         

      //  public virtual List<ChurchTransferDesignation> ChurchTransferDesignations { get; set; }


        ////////////////////////////////////////////////////////
        ////////////////////////////////////////////


        //case "N": return "Draft";  //New case "N": return "Closed";  

        //case "P": return "Acknowledged [Pending Approval]";
        //case "H": return "On Hold [Pending Approval]";
        //case "I": return "In Progress [Pending Approval]"; //

        //case "A": return "Approved"; //C-ompleted     case "F": return "Force-Complete"; //C-ompleted
        //case "D": return "Declined"; // "Denied" DECLINED;
        //case "R": return "Recalled";
        //case "X": return "Cancelled";

        //case "U": return "Unsuccessful";  //incomplete 
        //case "T": return "In-Transit";
        //case "Y": return "Transferred";  //Yes... Completed ... Awaiting member visit ... jux to make sorting simple ...
        //case "C": return "Closed"; // manually closed by the ToCB... when member visits
        //case "Z": return "Archived";  // manual... add fxn: Push to Archive, Push All Closed to Archive



    }
}
