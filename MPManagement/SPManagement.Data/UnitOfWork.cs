using SPManagement.Data.Contracts;
using SPManagement.Data.Repositories;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Data
{

    [NotMapped]
    public class UnitOfWork : IDisposable, IUnitOfWork
    {

        public SPManagementDbContext Context { get; set; }

        private bool disposed = false;

        #region Repositories

        public IRepository<Refrigerador> RefrigeradorRepository { get; set; }
        public IRepository<Cartucho> CartuchoRepository { get; set; }
        public IRepository<Tiempo> TiempoRepository { get; set; }
        public IRepository<BitacoraDeMovimientos> BitacoraDeMovimientosRepository { get; set; }
        #endregion Repositories

        public UnitOfWork()
        {
            Context = new SPManagementDbContext();
            this.Context.Database.CommandTimeout = 0;
            InitializeRepositories(this.Context);
        }

        private void InitializeRepositories(SPManagementDbContext context)
        {
            RefrigeradorRepository = new RefrigeradorRepository(context) as IRepository<Refrigerador>;
            CartuchoRepository = new CartuchoRepository(context) as IRepository<Cartucho>;
            TiempoRepository = new TiempoRepository(context) as IRepository<Tiempo>;
            BitacoraDeMovimientosRepository = new BitacoraDeMovimientosRepository(context) as IRepository<BitacoraDeMovimientos>;
        }

        public void CommitChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

    }
}
