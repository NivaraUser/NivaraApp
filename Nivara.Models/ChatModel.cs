using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class ChatModel
    {

        public int Id { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public int UsersTaskId{ get; set; }
        public string Message { get; set; }
        public int? CompanyId { get; set; }
        public int? EmployeeId { get; set; }
        public int? EndUserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsComment { get; set; }
        public int? ChatRefId { get; set; }
        public bool IsDocument { get; set; }
        public bool? IsDeleted { get; set; }
        public string Document { get; set; }
        public bool IsSent { get; set; }
        public IFormFile ChatDocument { get; set; }
    }
    public class ChatModelSenderReceiverDetail
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public int UsersTaskId { get; set; }
        public string ProjectName { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public string ProfilePiture { get; set; }
        public string LastMessage { get; set; }
        public string LastUserName { get; set; }
        public  string UserType { get; set; }
    }
}
