using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuckatorService.BO
{
    public class Log
    {
        public int Id { get; set; }
        public int LogTypeId { get; set; }
        public string RequestDate { get; set; }
        public string ResponseData { get; set; }

        public string ErrorCode { get; set; } = "0";

        public DateTime CreatedDate { get; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public string CreatedBy { get; set; } = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

    }

}

