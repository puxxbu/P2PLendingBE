using DAL.DTO.Req;
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
    public class LoanServices : ILoanServices
    {

        private readonly PeerlendingContext _peerlendingContext;

        public LoanServices(PeerlendingContext peerlendingContext)
        {
            _peerlendingContext = peerlendingContext;
        }

        public async Task<List<ResListLoanDto>> BorrowerLoanList(string status, string id)
        {
            var borrower = _peerlendingContext.MstUsers.FirstOrDefault(x => x.Id == id);
            if (borrower == null) {
                throw new Exception("Borrower id not found!");
            }

            var loans = await _peerlendingContext.MstLoans.
                Include(l => l.User).
                Select(x => new ResListLoanDto
                {
                    LoanId = x.Id,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).OrderBy(x => x.CreatedAt)
                .Where(x => x.BorrowerName == borrower.Name && (string.IsNullOrEmpty(status) || x.Status == status))
                .ToListAsync();

            return loans;
        }

        public async Task<string> CreateLoan(ReqLoanDto loan, string borrowerId)
        {
           var borrower = _peerlendingContext.MstUsers.FirstOrDefault(x => x.Id == borrowerId);

            if (borrower == null)
            {
                throw new Exception("Borrower id not found!");
            }

            if(loan.InterestRate < 0 || loan.InterestRate > 100)
            {
                throw new Exception("Interest rate must be between 0 and 100!");
            }

            if(loan.InterestRate == null)
            {
                loan.InterestRate = (decimal)2.5;
            }

            var newLoan = new MstLoans
            {
                BorrowerId = borrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = 12,

            };

            await _peerlendingContext.MstLoans.AddAsync(newLoan);
            await _peerlendingContext.SaveChangesAsync();


            return newLoan.BorrowerId;
        }

        public async Task<ResListLoanDto> GetLoanById(string id)
        {
            var loan = await _peerlendingContext.MstLoans.
                Include(l => l.User).
                Select(x => new ResListLoanDto
                {
                    LoanId = x.Id,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).FirstOrDefaultAsync(x => x.LoanId == id);

            var interestRate = (double)loan.InterestRate / 100;

            var returnAmount = interestRate * (double)loan.Amount / (1 - Math.Pow((1 + interestRate), (-12)));

            double amount = (double)loan.Amount;

            loan.ReturnAmount = (decimal?)Math.Round(returnAmount + amount, 2);
            loan.ReturnInterest = Math.Round((decimal)(loan.ReturnAmount - loan.Amount), 2);

            return loan;
        }

        public async Task<List<ResListLoanDto>> LoanList(string status)
        {

            var loans = await _peerlendingContext.MstLoans.
                Include(l => l.User).
                Select(x => new ResListLoanDto
                {
                    LoanId = x.Id,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).OrderBy(x => x.CreatedAt)
            .Where(x => string.IsNullOrEmpty(status) || x.Status == status)
            .ToListAsync();


            return loans;


        }

        public async Task<string> UpdateStatusLoan(ReqLoanStatusDto loan, string id)
        {
            var borrower = _peerlendingContext.MstLoans.FirstOrDefault(x => x.Id == id);

            if (borrower == null)
            {
                throw new Exception("Loan id not found!");
            }

            borrower.Status = loan.Status;
            borrower.UpdatedAt = DateTime.UtcNow;

            _peerlendingContext.MstLoans.Update(borrower);
            _peerlendingContext.SaveChanges();

            return id;

        }

        // Lender & Borrowers

        
    }
}
