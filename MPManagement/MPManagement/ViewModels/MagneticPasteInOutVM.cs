using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPManagement.ViewModels
{
    public class MagneticPasteInOutVM : ViewModelBase
    {
        #region Properties

        private readonly RefrigeradorBusiness refrigeradorBusiness;

        private ICollection<Refrigerador> _refrigeratorList;
        public ICollection<Refrigerador> RefrigeratorList
        {
            get
            {
                return (_refrigeratorList);
            }
            set
            {
                _refrigeratorList = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        public MagneticPasteInOutVM()
        {
            refrigeradorBusiness = new RefrigeradorBusiness();
            RefrigeratorList = refrigeradorBusiness.GetAllRefrigeratorsByIQueryable().ToList();
        }

    }
}
