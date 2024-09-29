using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Repayment
{
    public class ResPayScheduleDto
    {
        public DateTime RepaymentDate { get; set; }
        public bool IsPaid { get; set; }
    }
}
