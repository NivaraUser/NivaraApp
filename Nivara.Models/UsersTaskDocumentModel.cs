using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class UsersTaskDocumentModel
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public int UsersTaskId { get; set; }
    }
}
