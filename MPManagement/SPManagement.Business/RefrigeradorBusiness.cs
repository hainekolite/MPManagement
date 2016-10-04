using SPManagement.Data;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Business
{
    public class RefrigeradorBusiness
    {
         UnitOfWork unitOfWork;

        public RefrigeradorBusiness()
        {
            unitOfWork = new UnitOfWork();
        }

        public ICollection<Refrigerador> GetAll() => (unitOfWork.RefrigeradorRepository.GetList());

        public IQueryable<Refrigerador> GetAllRefrigeratorsByIQueryable() => 
            (unitOfWork.RefrigeradorRepository.GetQuery(null, null, includeProperties: GetIncludeProperties()));

        public Refrigerador GetRefrigeradorByIQueriable(string refrigeratorName) =>
            (unitOfWork.RefrigeradorRepository.GetQuery().Where(r => r.NumeroDeRefrigerador == refrigeratorName).ToList().FirstOrDefault());            

        public string[] GetIncludeProperties() => new[] { "Cartuchos" };
    }
}
