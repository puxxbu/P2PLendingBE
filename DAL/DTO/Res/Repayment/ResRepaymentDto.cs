﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res.Repayment
{
    public class ResRepaymentDto
    {
        public string Id { get; set; }

        public decimal Amount { get; set; }
        public DateTime RepaidAt { get; set; }
    }
}
