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

        public IQueryable<Cartucho> GetAllCartuchosByIQueryable() =>
            (unitOfWork.CartuchoRepository.GetQuery());

        public IQueryable<Cartucho> GetAllCartuchosByRefrigeradorIdByIQueryable(int refrigeratorId) =>
            (GetAllCartuchosByIQueryable().Where(c => c.RefrigeradorId == refrigeratorId));

        public void InsertCartucho(Cartucho cartucho)
        {
            unitOfWork.CartuchoRepository.Insert(cartucho);
            unitOfWork.CommitChanges();
        }

        public void UpdateCartucho(Cartucho cartucho)
        {
            unitOfWork.CartuchoRepository.Update(cartucho);
            unitOfWork.CommitChanges();
        }

    }
}
