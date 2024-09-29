using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Repayment
{
    public class ResRepaymentDetailDto
    {
        public string LoanId { get; set; }
        public string BorrowerName { get; set; }

        public decimal Amount { get; set; }

        public decimal InterestRate { get; set; }

        public int Duration { get; set; }

        public string Status { get; set; }

        public decimal? ReturnAmount { get; set; }

        public decimal? ReturnInterest { get; set; }


        public ResPayScheduleDto[] PaySchedules { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
