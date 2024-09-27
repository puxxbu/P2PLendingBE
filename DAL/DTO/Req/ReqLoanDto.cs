using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqLoanDto
    {

        [Required(ErrorMessage = "Borrower Id is required")]
        public string BorrowerId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Interest Rate is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Interest Rate must be a positive value")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }



    }
}
