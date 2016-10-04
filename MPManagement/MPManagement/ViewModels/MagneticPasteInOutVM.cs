using MPManagement.ViewModels.Commands;
using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MPManagement.ViewModels
{
    public class MagneticPasteInOutVM : ViewModelBase
    {
        #region Properties

        private const int ZERO_STATE = 0;
        private const int ONE_STATE = 1;
        private const int TWO_STATE = 2;

        //Hacerlo un instance para que lo compartan In y Out y no cargar dos viewModels
        private readonly RefrigeradorBusiness refrigeradorBusiness;
        private readonly CartuchoBusiness cartuchoBusiness;
        private Refrigerador refrigerador;
        private Cartucho cartucho;

        private readonly ParameterCommand _clearBoxesCommand;
        public ParameterCommand ClearBoxesCommand => _clearBoxesCommand;

        private readonly ParameterCommand _employeeNameEnterCommand;
        public ParameterCommand EmployeeNameEnterCommand => _employeeNameEnterCommand;

        private readonly ParameterCommand _refrigeratorIdBoxEnterCommand;
        public ParameterCommand RefrigeratorIdBoxEnterCommand => _refrigeratorIdBoxEnterCommand;

        private readonly ParameterCommand _cartridgeIdEnterCommand;
        public ParameterCommand CartridgeIdEnterCommand => _cartridgeIdEnterCommand;

        private ICollection<Cartucho> _cartuchoList;
        public ObservableCollection<Cartucho> CartuchoStateZeroList { get; set; }
        public ObservableCollection<Cartucho> CartuchoStateOneList { get; set; }
        public ObservableCollection<Cartucho> CartuchoStateTwoList { get; set; }

        public ICollection<Refrigerador> RefrigeratorList { get; set; }

        private bool _employeeNameBoxEnabled;
        public bool EmployeeNameBoxEnabled
        {
            get
            {
                return (_employeeNameBoxEnabled);
            }
            set
            {
                _employeeNameBoxEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _refrigeratorIdBoxEnabled;
        public bool RefrigeratorIdBoxEnabled
        {
            get
            {
                return (_refrigeratorIdBoxEnabled);
            }
            set
            {
                _refrigeratorIdBoxEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _cartridgeIdBoxEnabled;
        public bool CartridgeIdBoxEnabled
        {
            get
            {
                return (_cartridgeIdBoxEnabled);
            }
            set
            {
                _cartridgeIdBoxEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _employeeName;
        public string EmployeeName
        {
            get
            {
                return (_employeeName);
            }
            set
            {
                _employeeName = value;
                OnPropertyChanged();
            }
        }

        private string _refrigeratorId;
        public string RefrigeratorId
        {
            get
            {
                return (_refrigeratorId);
            }
            set
            {
                _refrigeratorId = value;
                OnPropertyChanged();
            }
        }

        private string _cartridgeId;
        public string CartridgeId
        {
            get
            {
                return (_cartridgeId);
            }
            set
            {
                _cartridgeId = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        public MagneticPasteInOutVM()
        {
            _clearBoxesCommand = new ParameterCommand(ClearAllBoxes);

            _employeeNameEnterCommand = new ParameterCommand(EmployeeBoxEnterKey);
            _refrigeratorIdBoxEnterCommand = new ParameterCommand(RefrigeratorIdBoxEnterKey);
            _cartridgeIdEnterCommand = new ParameterCommand(CartridgeIdBoxEnterKey);

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;

            refrigeradorBusiness = new RefrigeradorBusiness();
            cartuchoBusiness = new CartuchoBusiness();

            RefrigeratorList = refrigeradorBusiness.GetAllRefrigeratorsByIQueryable().ToList();
            if (RefrigeratorList != null)
            {
                for (int i = 0; i<RefrigeratorList.Count; i++)
                {
                    if (i == 0)
                        _cartuchoList = RefrigeratorList.ElementAt(i).Cartuchos.ToList();
                    else
                        _cartuchoList = _cartuchoList.Concat(RefrigeratorList.ElementAt(i).Cartuchos.ToList()).ToList();
                }
                CartuchoStateZeroList = new ObservableCollection<Cartucho>(_cartuchoList.ToList().Where( c => c.Estado == ZERO_STATE));
                CartuchoStateOneList = new ObservableCollection<Cartucho>(_cartuchoList.ToList().Where(c => c.Estado == ONE_STATE));
                CartuchoStateTwoList = new ObservableCollection<Cartucho>(_cartuchoList.ToList().Where(c => c.Estado == TWO_STATE));
            }
        }

        private void EmployeeBoxEnterKey(object box)
        {            
            TextBox refrigeratorIdBox = box as TextBox;

            /*if ( EmployeeName existe en base de datos){

            }
            else
            {
                MessageBox.Show("Usuario no identificado");
            }*/


            RefrigeratorIdBoxEnabled = true;
            EmployeeNameBoxEnabled = false;
            CartridgeIdBoxEnabled = false;
            refrigeratorIdBox.Focus();
        }

        private void RefrigeratorIdBoxEnterKey(object box)
        {
            TextBox cartridgeIdBox = box as TextBox;
            var query = refrigeradorBusiness.GetRefrigeradorByIQueriable(Regex.Replace(RefrigeratorId, " ", string.Empty));
            if (query != null)
            {
                refrigerador = query;
                EmployeeNameBoxEnabled = false;
                RefrigeratorIdBoxEnabled = false;
                CartridgeIdBoxEnabled = true;
                cartridgeIdBox.Focus();
            }
            else
            {
                MessageBox.Show("Refrigerador no identificado");
                refrigerador = null;
            }
        }

        private void CartridgeIdBoxEnterKey(object box)
        {
            TextBox employeeNameBox = box as TextBox;

            /*if ( CartridgeId existe en base de datos){
                
            }
            else
            {
                MessageBox.Show("Cartucho no identificado");
            }*/

            //do some action for the datagrid

            cartucho = new Cartucho()
            {
                NumeroDeCartucho = CartridgeId,
                FechaEntrada = DateTime.Now,
                FechaSalida = DateTime.MaxValue,
                FechaRecepcion = DateTime.MaxValue,
                Estado = 0,
                RefrigeradorId = refrigerador.Id
            };
            /*cartuchoBusiness.InsertCartucho(cartucho);
            CartuchoList.Add(cartucho);*/
            //CartuchoList.ElementAt(2).Estado = 0;

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;

            employeeNameBox.Focus();

        }

        private void ClearAllBoxes(object boxes)
        {
            var values = boxes as object[];
            TextBox EmployeeBox = values[0] as TextBox;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            refrigerador = null;
            cartucho = null;

            EmployeeBox.Focus();
        }

    }
}
