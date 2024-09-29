using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req.Loan
{
    public class ReqRepaymentDto
    {
        public string LoanId{ get; set; }
        public IEnumerable<DateTime> RepaidAt { get; set; }
    }
}
