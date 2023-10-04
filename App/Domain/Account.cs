using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App
{
    public class Account
    {
        private INotificationService notificationService;

        public Account(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public const decimal PayInLimit = 4000m;

        public const decimal lowFundsLimit = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public void withdrawFunds(decimal amount)
        {
            var newBalance = this.Balance - amount;

            if (this.sufficientBalanceToWithdraw(amount))
            {

                this.sendLowFundsLimit(amount);

                Balance = Balance - amount;
                Withdrawn = Withdrawn - amount;
            }
        }

        public void payInFunds(decimal amount)
        {
            
            var paidIn = this.PaidIn + amount;
            if (paidIn > Account.PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (Account.PayInLimit - paidIn < 500m)
            {
                this.notificationService.NotifyApproachingPayInLimit(this.User.Email);
            }

            this.Balance = this.Balance + amount;
            this.PaidIn = this.PaidIn + amount;
        }

        public bool sufficientBalanceToWithdraw(decimal amount)
        {

            var newBalance = this.Balance - amount;
            if (newBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            return true;

        }

        public void sendLowFundsLimit(decimal amount)
        {
            var newBalance = this.Balance - amount;
            if (newBalance < lowFundsLimit)
            {
                this.notificationService.NotifyFundsLow(this.User.Email);
            }
        }






    }
}
