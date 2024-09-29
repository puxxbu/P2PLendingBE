using DAL.DTO.Req.Loan;
using DAL.DTO.Res.Loan;
using DAL.DTO.Res.Repayment;
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
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlendingContext _peerlendingContext;

        public RepaymentServices(PeerlendingContext peerlendingContext)
        {
            _peerlendingContext = peerlendingContext;
        }

        public async Task<string> RepayLoan(ReqRepaymentDto req)
        {
            var loan = await _peerlendingContext.MstLoans.FirstOrDefaultAsync(x => x.Id == req.LoanId);

            if (loan == null)
            {
                throw new Exception("Loan not found!");
            }

            if (loan.Status != "funded")
            {
                throw new Exception("Loan is not funded!");
            }

            var funding = await _peerlendingContext.TrnFundings.FirstOrDefaultAsync(x => x.LoanId == req.LoanId);

            if (funding == null)
            {
                throw new Exception("Funding not found!");
            }

            var lender = _peerlendingContext.MstUsers.SingleOrDefault(x => x.Id == funding.LenderId);

            double amount = (double)loan.Amount;
            var interestRate = (double)loan.InterestRate / 100;
            var returnAmount = interestRate * (double)loan.Amount / (1 - Math.Pow((1 + interestRate), (-12)));
          
            var repaymentDates = req.RepaidAt;

            returnAmount = (double)(decimal?)Math.Round(returnAmount + amount, 2);

            decimal totalRepaidAmount = 0;
            var repaymentAmount = (decimal)(returnAmount / 12);


            foreach (var repaidAt in repaymentDates)
            {
                if (await IsMonthPaid(req.LoanId, repaidAt))
                {
                    continue;
                }

                var lastRepayment = await _peerlendingContext.TrnRepayment
                    .Where(r => r.LoanId == req.LoanId)
                    .OrderByDescending(r => r.PaidAt)
                    .FirstOrDefaultAsync();

                var repayment = new TrnRepayment
                {
                    Id = Guid.NewGuid().ToString(), 
                    LoanId = req.LoanId,
                    Amount = (decimal)returnAmount,
                    RepaidAmount = repaymentAmount,
                    BalanceAmount = (decimal)returnAmount - repaymentAmount - totalRepaidAmount, 
                    PaidAt = repaidAt
                };

                if (repayment.BalanceAmount <= 0)
                {
                    repayment.RepaidStatus = "done";
                    lender.Balance += (decimal)returnAmount;

                    _peerlendingContext.MstUsers.Update(lender);
                }
                else
                {
                    repayment.RepaidStatus = "on repay";
                }

                totalRepaidAmount += repaymentAmount;

                _peerlendingContext.TrnRepayment.Add(repayment);
               

                if (lastRepayment != null && lastRepayment.BalanceAmount <= 0 )
                {
                    lastRepayment.RepaidStatus = "done";
                    

                    await _peerlendingContext.SaveChangesAsync();
                }

                await _peerlendingContext.SaveChangesAsync();
            }


            if (totalRepaidAmount >= funding.Amount)
            {
                loan.Status = "repaid";
                await _peerlendingContext.SaveChangesAsync();
            }

            return "Loan repayment successful";
        }


        public async Task<List<ResPayScheduleDto>> GetRepaymentSchedule(string loanId)
        {
            var funding = await _peerlendingContext.TrnFundings.FirstOrDefaultAsync(x => x.LoanId == loanId);
            if (funding == null)
            {
                throw new Exception("Funding not found!");
            }

            var repaymentSchedule = new List<ResPayScheduleDto>();
            var startDate = funding.FundedAt;

            for (int i = 0; i < 12; i++)
            {
                var repaymentDate = startDate.AddMonths(i);
                var isPaid = await IsMonthPaid(loanId, repaymentDate);

                repaymentSchedule.Add(new ResPayScheduleDto
                {
                    RepaymentDate = repaymentDate,
                    IsPaid = isPaid
                });
            }

            return repaymentSchedule;
        }

        private async Task<bool> IsMonthPaid(string loanId, DateTime repaidAt)
        {
            var existingRepayment = await _peerlendingContext.TrnRepayment
                .FirstOrDefaultAsync(r => r.LoanId == loanId && r.PaidAt.Month == repaidAt.Month && r.PaidAt.Year == repaidAt.Year);

            return existingRepayment != null;
        }

        public async Task<ResRepaymentDetailDto> GetPaymentDetail(string loanId)
        {

            var funding = await _peerlendingContext.TrnFundings.FirstOrDefaultAsync(x => x.LoanId == loanId);
            if (funding == null)
            {
                throw new Exception("Funding not found!");
            }

            var repaymentSchedule = new List<ResPayScheduleDto>();
            var startDate = funding.FundedAt;

            for (int i = 0; i < 12; i++)
            {
                var repaymentDate = startDate.AddMonths(i);
                var isPaid = await IsMonthPaid(loanId, repaymentDate);

                repaymentSchedule.Add(new ResPayScheduleDto
                {
                    RepaymentDate = repaymentDate,
                    IsPaid = isPaid
                });
            }

            var loan = await _peerlendingContext.MstLoans.
                Include(l => l.User).
                Select(x => new ResRepaymentDetailDto
                {
                    LoanId = x.Id,
                    BorrowerName = x.User.Name,
                    Amount = x.Amount,
                    InterestRate = x.InterestRate,
                    Duration = x.Duration,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).FirstOrDefaultAsync(x => x.LoanId == loanId);

            var interestRate = (double)loan.InterestRate / 100;

            var returnAmount = interestRate * (double)loan.Amount / (1 - Math.Pow((1 + interestRate), (-12)));

            double amount = (double)loan.Amount;

            loan.ReturnAmount = (decimal?)Math.Round(returnAmount + amount, 2);
            loan.ReturnInterest = Math.Round((decimal)(loan.ReturnAmount - loan.Amount), 2);

            loan.PaySchedules = repaymentSchedule.ToArray();

            return loan;


        }
    }


    



}
