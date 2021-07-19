﻿using HumanResources.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResources.CustomFunctions
{
    class MakerChecker
    {
        private static LeaveManagementSystemEntities _db = new LeaveManagementSystemEntities();
        /**
        * Function returns true if approval request is sent
        * @param _DocumentType | The record document type     
        * @param _DocumentNumber | The record record number

        * @return bool | return true if approval entry is created / return false if not created
        */
        public static string SendApprovalRequest(string _DocumentNumber)
        {
            string ApprovalStatus = null; //Created,Open,Canceled,Rejected,Approved
            string ApproverEmail = null;
            string ApproverName = null;
            string OpenApproverEmail = null;
            string status = null, message = null;

            bool ApprovalEntryCreated = false;

            var approvers = _db.ApprovalUsers.Where(a => a.DocumentType == "Leave").ToList();

            if (approvers != null)
            {
                //Get List of approval users then create Approval Entries (Make Reusable)

                //create approval entries for each of the Approvers, for first approver in the sequence, set to 'Open', the remaining, set to 'Created'

                //loop through approval users json array

                foreach (var ApprovalUser in approvers)
                {
                    string DocumentType = ApprovalUser.DocumentType;
                    string Approver = ApprovalUser.Approver;

                    int ApprovalSequence = ApprovalUser.ApprovalSequence;

                    if (ApprovalSequence == 1)
                    {
                        ApprovalStatus = "Open";
                        ApproverEmail = ApprovalUser.ApproverEmail;
                        OpenApproverEmail = ApproverEmail;
                        ApproverName = ApprovalUser.Approver;

                       
                    }
                    else
                    {
                        ApprovalStatus = "Created";
                    }
                    //create Approval Entry

                    ApprovalEntryCreated = CreateApprovalEntry(_DocumentNumber, ApprovalSequence, Approver, ApprovalStatus, ApprovalUser.Approver, "Derrick", DateTime.Now);                   
                }
                //Update Parent Table

                UpdateParentTableStatus(_DocumentNumber, "Pending Approval");

                //if sender has an approval entry approve it

                ApprovalEntryCreated = true;

                message = "An approval entry has been successfully created";
                status = "000";
            }
            else
            {
                ApprovalEntryCreated = false;
                status = "900";
            }

            var _ApprovalRequestResponse = new ApprovalRequestResponse
            {
                Status = status,
                Message = message,
                ApproverEmail = OpenApproverEmail
            };

            return JsonConvert.SerializeObject(_ApprovalRequestResponse);
        }

        private static bool UpdateParentTableStatus(string documentNumber, string ApprovalStatus)
        {
            bool status = false;
            try
            {

                using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
                {
                    var leaves = dbEntities.Leaves.Where(a => a.DocumentNo == documentNumber).FirstOrDefault();

                    if (leaves != null)
                    {
                        leaves.ApprovalStatus = ApprovalStatus;
                        dbEntities.SaveChanges();
                        status = true;
                    }
                }
            }
            catch (Exception es)
            {
                status = false;
            }
            return status;
        }

        private static bool CreateApprovalEntry(string documentNumber, int approvalSequence, string approver, string approvalStatus, string ApproverId, string SenderId, DateTime DateSent)
        {
            bool status = false;
            try
            {
                var approvalEntry = new ApprovalEntry { DocumentNo = documentNumber, SequenceNo = approvalSequence, Status = approvalStatus, ApproverId = ApproverId, DocumentType = "Leave", SenderId = SenderId, DateSent = DateSent };

                using (LeaveManagementSystemEntities dbEntities = new LeaveManagementSystemEntities())
                {
                    dbEntities.Configuration.ValidateOnSaveEnabled = false;
                    dbEntities.ApprovalEntries.Add(approvalEntry);
                    dbEntities.SaveChanges();
                    status = true;
                }
            }
            catch (Exception es)
            {
                status = false;
            }
            return status;
        }


        /**
        * Function returns true if approval entry record sequence is updated
        * @param EntryNumber | The record entry    
        * @param SequenceNumber | The record sequence number
        * @param _DocumentType | The record document type     
        * @param _DocumentNumber | The record's record number
        * @param Table | The record record's parent

        * @return bool | return true if approval entry is created / return false if not created
        */
        public static string ApproveAppovalRequest(int EntryNumber, int SequenceNumber, string _DocumentType, string _DocumentNumber)
        {
            bool ApprovalEntryCreated = false;
            string status = null, message = null, senderemail = null, SenderName = null;
            // Update status to 'Approved' for specified DocumentNumber and Document Type where Approver is loggedIn user
            bool IsRecordApproved = UpdateApprovalEntry(EntryNumber, _DocumentType, _DocumentNumber, "Approved", "Derrick");

            if (IsRecordApproved)
            {
                //check if there are approvers in sequence
                var PendingApprovals = _db.ApprovalEntries.Where(a => a.DocumentType == "Leave" && a.DocumentNo == _DocumentNumber && (a.Status == "Created" || a.Status == "Open")).ToList().Count();

                if (PendingApprovals > 0)
                {
                    //take current approval sequence and add 1 to get new approval sequence

                    SequenceNumber = SequenceNumber + 1;

                    //with the new approval sequence, update status to Open

                    ApprovalEntryCreated = UpdateApprovalEntrySequence(SequenceNumber, _DocumentType, _DocumentNumber, "Open");
                }
                else if (PendingApprovals == 0)
                {
                    //if no approver in sequence, update parent table status

                    ApprovalEntryCreated = true;

                    UpdateParentTableStatus(_DocumentNumber, "Approved");

                    UpdateApprovalEntry(EntryNumber, _DocumentType, _DocumentNumber, "Approved", "Derrick");

                    var SenderInfo = _db.ApprovalEntries.Where(a => a.DocumentNo == _DocumentNumber).FirstOrDefault();
                    var EmployeeRec = _db.Employees.Where(e => e.EmployeeNo == SenderInfo.SenderId).FirstOrDefault();

                    message = "An approval entry has been successfully approved";
                    status = "000";

                    senderemail = EmployeeRec.EMail;
                    SenderName = EmployeeRec.FirstName;
                }
            }
            var _ApprovalRequestResponse = new ApprovedRequestResponse
            {
                Status = status,
                Message = message,
                SenderEmail = senderemail,
                SenderName = SenderName,
                DocumentNo = _DocumentNumber
            };

            return JsonConvert.SerializeObject(_ApprovalRequestResponse);
        }
        private static bool UpdateApprovalEntrySequence(int SequenceNumber, string DocumentType, string DocumentNumber, string Status)
        {
            bool status = false;

            try
            {
                using (var db = new LeaveManagementSystemEntities())
                {
                    var approvalentry = db.ApprovalEntries.Where(x => x.SequenceNo == SequenceNumber && x.DocumentType == DocumentType && x.DocumentNo == DocumentNumber).SingleOrDefault();

                    if (approvalentry != null)
                    {
                        approvalentry.Status = Status;
                        db.SaveChanges();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        private static bool UpdateApprovalEntry(int EntryNumber, string DocumentType, string DocumentNumber, string Status, string ApproverId)
        {
            bool status = false;

            try
            {
                using (var db = new LeaveManagementSystemEntities())
                {
                    var approvalentry = db.ApprovalEntries.Where(x => x.EntryNumber == EntryNumber && x.DocumentType == DocumentType && x.DocumentNo == DocumentNumber && x.ApproverId == ApproverId).SingleOrDefault();

                    if (approvalentry == null)
                    {
                        status = false;
                    }
                    else
                    {
                        approvalentry.Status = Status;
                        db.SaveChanges();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
        /**
        * Function returns true if approval entry record sequence is rejected     
        * @param _DocumentType | The record document type     
        * @param _DocumentNumber | The record's record number

        * @return bool | return true if approval entry is rejected / return false if not rejected
        */
        public static bool RejectAppovalRequest(int EntryNumber, string _DocumentType, string _DocumentNumber, string Approver)
        {
            //set all approval entries record to Rejected
            bool IsRejected = UpdateApprovalEntry(EntryNumber, _DocumentType, _DocumentNumber, "Rejected", Approver);

            IsRejected = UpdateParentTableStatus(_DocumentNumber, "Rejected");

            return IsRejected;
        }
        /**
        * Function returns true if approval entry record sequence is delegated     
        * @param _DocumentType | The record document type     
        * @param _DocumentNumber | The record's record number

        * @return bool | return true if approval entry is delegated / return false if not delegated
        */
        public static bool DelegateAppovalRequest(int EntryNumber, int ApprovalSequence)
        {
            //Get Document approver substitute
            var approvalSubstitute = _db.ApprovalUsers.Where(x => x.DocumentType == "Leave" && x.ApprovalSequence == ApprovalSequence).FirstOrDefault();
            string substituteApprover = approvalSubstitute.SubstituteApprover;

            //Update Approval User 
            bool IsRecordDelegated = UpdateApprovalEntryApproverId(EntryNumber, substituteApprover);

            return IsRecordDelegated;
        }
        private static bool UpdateApprovalEntryApproverId(int entryNumber, string approverId)
        {
            bool status = false;

            try
            {
                using (var db = new LeaveManagementSystemEntities())
                {
                    var approvalentry = db.ApprovalEntries.Where(x => x.EntryNumber == entryNumber).SingleOrDefault();

                    if (approvalentry == null)
                    {
                        status = false;
                    }
                    else
                    {
                        approvalentry.ApproverId = approverId;
                        db.SaveChanges();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }
    }
}