using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Funding
{
    public class ResFundingDto
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string LenderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime FundedAt { get; set; }
    }
}
