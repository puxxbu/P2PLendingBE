using DAL.DTO.Req.Loan;
using DAL.DTO.Res.Loan;
using DAL.DTO.Res.Repayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentServices
    {
        Task<string> RepayLoan(ReqRepaymentDto req);

        Task<ResRepaymentDetailDto> GetPaymentDetail(string id);

        Task<List<ResPayScheduleDto>> GetRepaymentSchedule(string loanId);
    }
}
