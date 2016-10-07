using MPManagement.Contracts;
using MPManagement.ViewModels.Commands;
using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MPManagement.ViewModels
{
    public class MagneticPasteInOutVM : ViewModelBase, IDisposeDataContext
    {
        #region Properties

        private const int ZERO_STATE = 0;
        private const int ONE_STATE = 1;
        private const int TWO_STATE = 2;

        //Hacerlo un instance para que lo compartan In y Out y no cargar dos viewModels
        private readonly RefrigeradorBusiness refrigeradorBusiness;
        private readonly CartuchoBusiness cartuchoBusiness;
        private Refrigerador refrigerator;
        private Cartucho cartridge;

        private readonly ParameterCommand _solderingPasteInCommand;
        public ParameterCommand SolderingPasteInCommand => _solderingPasteInCommand;

        private readonly ParameterCommand _solderingPasteOutCommand;
        public ParameterCommand SolderingPasteOutCommand => _solderingPasteOutCommand;

        private readonly ParameterCommand _solderingPasteOptionCommand;
        public ParameterCommand SolderingPasteOptionCommand => _solderingPasteOptionCommand;

        private readonly ParameterCommand _clearBoxesCommand;
        public ParameterCommand ClearBoxesCommand => _clearBoxesCommand;

        private readonly ParameterCommand _employeeNameEnterCommand;
        public ParameterCommand EmployeeNameEnterCommand => _employeeNameEnterCommand;

        private readonly ParameterCommand _refrigeratorIdBoxEnterCommand;
        public ParameterCommand RefrigeratorIdBoxEnterCommand => _refrigeratorIdBoxEnterCommand;

        private readonly ParameterCommand _cartridgeIdEnterCommand;
        public ParameterCommand CartridgeIdEnterCommand => _cartridgeIdEnterCommand;

        private ICollection<Cartucho> _cartridgeList;

        public ObservableCollection<Cartucho> CartridgeList { get; set; }

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

        private readonly bool isInOrOut;
        private readonly bool isOutOrReturn;
        private bool _option;
        public bool Option
        {
            get
            {
                return (_option);
            }
            set
            {
                _option = value;
                OnPropertyChanged();
            }
        }
        DispatcherTimer dispatcherTimer;


        #endregion Properties

        #region Constructor

        public MagneticPasteInOutVM()
        {
            Option = true;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();

            _solderingPasteInCommand = new ParameterCommand(SolderPasteIn);
            _solderingPasteOutCommand = new ParameterCommand(SolderPasteOut);
            _solderingPasteOptionCommand = new ParameterCommand(SolderPasteManagementConfiguration);
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

            UpdateList();
        }

        #endregion Constructor

        #region UpdatingList

        private void UpdateList()
        {
            RefrigeratorList = refrigeradorBusiness.GetAllRefrigeratorsByIQueryable().ToList();
            if (RefrigeratorList != null)
            {
                for (int i = 0; i < RefrigeratorList.Count; i++)
                {
                    if (i == 0)
                        _cartridgeList = RefrigeratorList.ElementAt(i).Cartuchos.ToList();
                    else
                        _cartridgeList = _cartridgeList.Concat(RefrigeratorList.ElementAt(i).Cartuchos.ToList()).ToList();
                }
                CartridgeList = new ObservableCollection<Cartucho>(_cartridgeList.ToList().OrderByDescending(c => c.FechaSalida));
                OnPropertyChanged("CartridgeList");
            }
        }

        #endregion UpdatingList

        #region SQLRawValidations

        private bool isEmployeeValid()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["NexusPCSDbContext"].ConnectionString; ;
            string query = "SELECT * FROM Users";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = (reader["Name"].ToString() + " " + reader["LastName"].ToString()).ToUpper();
                if (Regex.Replace(EmployeeName, " ", string.Empty).ToUpper().Equals(Regex.Replace(name, " ", string.Empty)))
                {
                    connection.Close();
                    return (true);
                }
            }
            connection.Close();
            return (false);
        }

        #endregion SQLRawValidations

        #region EnterOnBoxes

        private void EmployeeBoxEnterKey(object box)
        {            
            TextBox refrigeratorIdBox = box as TextBox;

            if (isEmployeeValid())
            {
                RefrigeratorIdBoxEnabled = true;
                EmployeeNameBoxEnabled = false;
                CartridgeIdBoxEnabled = false;
                refrigeratorIdBox.Focus();
            }
            else
                MessageBox.Show("Nombre de Empleado no Valido favor de consultar con su supervisor", "ERROR");
            
        }

        private void RefrigeratorIdBoxEnterKey(object box)
        {
            TextBox cartridgeIdBox = box as TextBox;
            var query = refrigeradorBusiness.GetRefrigeradorByIQueriable(Regex.Replace(RefrigeratorId, " ", string.Empty));
            if (query != null)
            {
                refrigerator = query;
                EmployeeNameBoxEnabled = false;
                RefrigeratorIdBoxEnabled = false;
                CartridgeIdBoxEnabled = true;
                cartridgeIdBox.Focus();
            }
            else
            {
                MessageBox.Show("Refrigerador no identificado");
                refrigerator = null;
            }
        }

        private void CartridgeIdBoxEnterKey(object box)
        {
            TextBox employeeNameBox = box as TextBox;

            if (Option)
                InsertCartridgeToRefrigerator(ref employeeNameBox);
            else
                UpdateCartridgeState(ref employeeNameBox, ZERO_STATE, ONE_STATE);
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

            refrigerator = null;
            cartridge = null;

            EmployeeBox.Focus();
        }

        #endregion EnterOnBoxes

        #region ButtonsForMainFunctions

        private void SolderPasteIn(object boxes)
        {
            var values = boxes as object[];
            TextBox EmployeeBox = values[0] as TextBox;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            refrigerator = null;
            cartridge = null;

            Option = true;

            EmployeeBox.Focus();
        }

        private void SolderPasteOut(object boxes)
        {
            var values = boxes as object[];
            TextBox EmployeeBox = values[0] as TextBox;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            refrigerator = null;
            cartridge = null;

            Option = false;

            EmployeeBox.Focus();
        }

        private void SolderPasteManagementConfiguration(object boxes)
        {
            var values = boxes as object[];
            TextBox EmployeeBox = values[0] as TextBox;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            refrigerator = null;
            cartridge = null;

            EmployeeBox.Focus();
        }



        #endregion ButtonsForMainFunctions

        #region InsertUpdateCartridgeMethods

        private void InsertCartridgeToRefrigerator(ref TextBox box)
        {
            cartridge = new Cartucho()
            {
                NumeroDeCartucho = CartridgeId,
                FechaEntrada = DateTime.Now,
                FechaSalida = DateTime.MaxValue,
                FechaRecepcion = DateTime.MaxValue,
                Estado = ZERO_STATE,
                RefrigeradorId = refrigerator.Id
            };
            var query = cartuchoBusiness.GetAllCartuchosByRefrigeradorIdByIQueryable(refrigerator.Id).Where(c => c.NumeroDeCartucho == cartridge.NumeroDeCartucho).FirstOrDefault();
            if (query == null)
            {
                CartridgeList.Add(cartridge);
                cartuchoBusiness.InsertCartucho(cartridge);
                EmployeeNameBoxEnabled = true;
                RefrigeratorIdBoxEnabled = false;
                CartridgeIdBoxEnabled = false;

                EmployeeName = string.Empty;
                RefrigeratorId = string.Empty;
                CartridgeId = string.Empty;

                box.Focus();
                MessageBox.Show("Cartucho introducido exitosamente");
            }
            else
                MessageBox.Show("El cartucho ya se encuentra registrado dentro del refrigerador especificado");
        }


        private void UpdateCartridgeState(ref TextBox box, int previousState, int newState)
        {
            var query = cartuchoBusiness.GetAllCartuchosByRefrigeradorIdByIQueryable(refrigerator.Id).ToList().Where(c => c.NumeroDeCartucho == CartridgeId).FirstOrDefault();
            if (query != null)
            {
                if (query.Estado == previousState)
                {
                    var cartridge = CartridgeList.Where(c => c.Id == query.Id).ToList().FirstOrDefault();
                
                    if (newState == ZERO_STATE)
                    {
                        cartridge.FechaSalida = DateTime.MaxValue;
                        cartridge.FechaEntrada = DateTime.Now;
                        query.FechaSalida = DateTime.MaxValue;
                        query.FechaEntrada = DateTime.Now;
                    }
                    else
                    {
                        query.FechaSalida = DateTime.Now;
                        cartridge.FechaSalida = DateTime.Now;
                    }

                    cartridge.Estado = newState;
                    query.Estado = newState;

                    cartuchoBusiness.UpdateCartucho(query);
                    EmployeeNameBoxEnabled = true;
                    RefrigeratorIdBoxEnabled = false;
                    CartridgeIdBoxEnabled = false;

                    EmployeeName = string.Empty;
                    RefrigeratorId = string.Empty;
                    CartridgeId = string.Empty;

                    box.Focus();
                }
                else
                    MessageBox.Show("El cartucho seleccionado no se encuentra en alguno de los refrigeradores", "ERROR");
            }
            else
                MessageBox.Show("El cartucho seleccionado no se encuentra relacionado al refrigerador seleccionado o no se encuentra registrado", "ERROR");
        }

        #endregion InsertUpdateCartridgeMethods

        #region TimerForUpdate

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            UpdateList();
        }

        #endregion

        #region DisposeableItems


        public void Dispose()
        {
            this.dispatcherTimer.Stop();
        }

        #endregion DisposeableItems

    }
}
