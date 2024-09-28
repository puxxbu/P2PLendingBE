using DAL.DTO.Req;
using DAL.DTO.Res.Funding;
using DAL.DTO.Res.Loan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan, string borrowerId);

        Task<string> UpdateStatusLoan(ReqLoanStatusDto loan , string id);

        Task<List<ResListLoanDto>> LoanList(string status);

        Task<ResListLoanDto> GetLoanById(string id);


    }
}
