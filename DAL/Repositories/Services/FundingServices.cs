using DAL.DTO.Res.Funding;
using DAL.DTO.Res.Loan;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class FundingServices : IFundingServices
    {
        private readonly PeerlendingContext _peerlendingContext;

        public FundingServices(PeerlendingContext peerlendingContext)
        {
            _peerlendingContext = peerlendingContext;
        }
        public async Task<ResFundingDto> LendMoney(string loanId, string lenderId)
        {
            var loan = _peerlendingContext.MstLoans.Include(l => l.User).SingleOrDefault(x => x.Id == loanId);
            var lender = _peerlendingContext.MstUsers.SingleOrDefault(x => x.Id == lenderId);

            if (loan == null)
            {
                throw new Exception("Loan not found!");
            }

            if (lender == null)
            {
                throw new Exception("lender not found");
            }

            if (loan.Status != "requested")
            {
                throw new Exception("Loan is funded/repaid!");
            }

            if (lender.Balance < loan.Amount)
            {
                throw new Exception("Saldo Tidak cukup !");
            }


            var timeNow = DateTime.UtcNow;

            var funding = new TrnFunding
            {
                LoanId = loan.Id,
                LenderId = lenderId,
                Amount = loan.Amount,
                FundedAt = timeNow
            };

            loan.Status = "funded";
            loan.UpdatedAt = timeNow;

            lender.Balance -= loan.Amount;
            loan.User.Balance += loan.Amount;

            _peerlendingContext.TrnFundings.Add(funding);
            _peerlendingContext.MstLoans.Update(loan);
            _peerlendingContext.MstUsers.Update(lender);
            _peerlendingContext.SaveChanges();

            return new ResFundingDto
            {
                Id = funding.Id,
                LoanId = funding.LoanId,
                LenderId = funding.LenderId,
                Amount = funding.Amount,
                FundedAt = funding.FundedAt
            };
        }

        public async Task<List<ResListLoanDto>> FundingHistory(string lenderId)
        {

            var loans = await _peerlendingContext.TrnFundings.
                Where(x => x.LenderId == lenderId).
                Include(l => l.Loan).
                Select(x => new ResListLoanDto
                {
                    LoanId = x.LoanId,
                    BorrowerName = x.Loan.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.Loan.InterestRate,
                    Duration = x.Loan.Duration,
                    Status = x.Loan.Status,
                    CreatedAt = x.Loan.CreatedAt,
                    UpdatedAt = x.Loan.UpdatedAt
                }).OrderBy(x => x.CreatedAt)
            .ToListAsync();


            return loans;
        }
    }
}
