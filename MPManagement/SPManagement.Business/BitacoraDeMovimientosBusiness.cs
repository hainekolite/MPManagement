using SPManagement.Data;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Business
{
    public class BitacoraDeMovimientosBusiness
    {
        UnitOfWork unitOfWork;

        public BitacoraDeMovimientosBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<BitacoraDeMovimientos> GetAll() => (unitOfWork.BitacoraDeMovimientosRepository.GetList());

        public IQueryable<BitacoraDeMovimientos> GetAllBitacorasByIQueryable() => unitOfWork.BitacoraDeMovimientosRepository.GetQuery();

        public void InsertMovimiento(BitacoraDeMovimientos Bitacora)
        {
            unitOfWork.BitacoraDeMovimientosRepository.Insert(Bitacora);
            unitOfWork.CommitChanges();
        }

    }
}
