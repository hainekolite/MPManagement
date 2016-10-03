using SPManagement.Data;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Business
{
    public class CartuchoBusiness
    {
        UnitOfWork unitOfWork;

        public CartuchoBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<Cartucho> GetAll() => (unitOfWork.CartuchoRepository.GetList());

        public IQueryable<Cartucho> GetAllUserByIQueryable() =>
            (unitOfWork.CartuchoRepository.GetQuery());
    }
}
