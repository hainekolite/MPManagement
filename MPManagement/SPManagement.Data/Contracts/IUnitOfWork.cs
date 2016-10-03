using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Data.Contracts
{
    public interface IUnitOfWork
    {
        IRepository<Refrigerador> RefrigeradorRepository { get; set; }
        IRepository<Cartucho> CartuchoRepository { get; set; }

        void CommitChanges();
        void Dispose();
    }
}
