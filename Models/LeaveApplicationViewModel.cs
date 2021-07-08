﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HumanResources.Models
{
    public class LeaveApplicationViewModel
    {
        public string LeaveType { get; set; }
        public string LeaveDaysEntitled { get; set; }
        public string LeaveDaysTaken { get; set; }
        public string LeaveBalance { get; set; }
        public string LeaveAccruedDays { get; set; }
        public string LeaveOpeningBalance { get; set; }

        //selection
        public string SelectionType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveDates { get; set; }
        public string LeaveDaysApplied { get; set; }
        public string LeaveStartDate { get; set; }
        public string LeaveEndDate { get; set; }
        public string ReturnDate { get; set; }
    }
}