using SPManagement.Data;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Business
{
    public class TiempoBusiness
    {
        UnitOfWork unitOfWork;

        public TiempoBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<Tiempo> GetAll() => (unitOfWork.TiempoRepository.GetList());

        public IQueryable<Tiempo> GetAllTiemposByIQueryable() => unitOfWork.TiempoRepository.GetQuery();
    }
}
