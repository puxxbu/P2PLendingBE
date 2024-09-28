using DAL.DTO.Res.Funding;
using DAL.DTO.Res.Loan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IFundingServices
    {

        // Lender 

        Task<ResFundingDto> LendMoney(string loanId, string lenderId);

        Task<List<ResListLoanDto>> FundingHistory(string lenderId);


    }
}
