//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HumanResources.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Setting
    {
        public int Id { get; set; }
        public string AfricasTalkingAppName { get; set; }
        public string AfricasTalkingApiKey { get; set; }
        public string GoogleId { get; set; }
        public string GmailAppPassword { get; set; }
        public string GmailSenderName { get; set; }
        public string GmailUsername { get; set; }
        public string GmailPassword { get; set; }
        public string EmailSender { get; set; }
        public Nullable<int> SMTPPort { get; set; }
        public string SMTPHost { get; set; }
        public string LeaveNumbers { get; set; }
        public string LeaveRecallNumbers { get; set; }
    }
}
