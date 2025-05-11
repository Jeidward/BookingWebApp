using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Entities;

namespace Interfaces
{
    public interface IAccountHolderRepository
    {
        public void SaveAccount(AccountHolder accountHolder);

        public AccountHolder GetAccountHolder(int id);

        public AccountHolder GetAccountHolderByUserId(int id);

        public bool HasBookingForAccountHolder(int accountId);

        public int GetTotalAccountHolder();
    }
}
