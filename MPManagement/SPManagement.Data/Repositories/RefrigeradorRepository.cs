using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPManagement.Data.Repositories
{
    public class RefrigeradorRepository : GenericRepository<Refrigerador>
    {
        public RefrigeradorRepository(SPManagementDbContext context) : base(context)
        {
        }
    }
}
