using MPManagement.Contracts;
using MPManagement.ViewModels.Commands;
using MPManagement.Views.dialogs;
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
    public class SolderingPasteInOutVM : ViewModelBase, IDisposeDataContext
    {
        #region Properties

        private const int ZERO_STATE = 0;
        private const int ONE_STATE = 1;
        private const int TWO_STATE = 2;
        private const int THIRD_STATE = 3;

        //Hacerlo un instance para que lo compartan In y Out y no cargar dos viewModels
        private readonly RefrigeradorBusiness refrigeradorBusiness;
        private readonly CartuchoBusiness cartuchoBusiness;
        private readonly TiempoBusiness tiempoBusiness;
        private readonly BitacoraDeMovimientosBusiness bitacoraDeMovimientosBusiness;

        private Refrigerador refrigerator;
        private Cartucho cartridge;
        private BitacoraDeMovimientos binnacleOfMovement;
        public Tiempo tiempo;
        private Object DBLock;

        private readonly ParameterCommand _solderingPasteInCommand;
        public ParameterCommand SolderingPasteInCommand => _solderingPasteInCommand;

        private readonly ParameterCommand _solderingPasteOutCommand;
        public ParameterCommand SolderingPasteOutCommand => _solderingPasteOutCommand;

        private readonly ParameterCommand _solderingPasteOptionCommand;
        public ParameterCommand SolderingPasteOptionCommand => _solderingPasteOptionCommand;

        private readonly ParameterCommand _clearBoxesCommand;
        public ParameterCommand ClearBoxesCommand => _clearBoxesCommand;

        private readonly ParameterCommand _updateButtonCommand;
        public ParameterCommand UpdateButtonCommand => _updateButtonCommand;

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
        private DispatcherTimer dispatcherTimer;

        #endregion Properties

        #region Constructor

        public SolderingPasteInOutVM()
        {
            Option = true;

            refrigeradorBusiness = new RefrigeradorBusiness();
            cartuchoBusiness = new CartuchoBusiness();
            tiempoBusiness = new TiempoBusiness();
            bitacoraDeMovimientosBusiness = new BitacoraDeMovimientosBusiness();

            tiempo = tiempoBusiness.GetAll().FirstOrDefault();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, tiempo.SegundosRefresco);
            dispatcherTimer.Start();

            _solderingPasteInCommand = new ParameterCommand(SolderPasteIn);
            _solderingPasteOutCommand = new ParameterCommand(SolderPasteOut);
            _solderingPasteOptionCommand = new ParameterCommand(SolderPasteManagementConfiguration);
            _clearBoxesCommand = new ParameterCommand(ClearAllBoxes);
            _updateButtonCommand = new ParameterCommand(UpdateButton);

            _employeeNameEnterCommand = new ParameterCommand(EmployeeBoxEnterKey);
            _refrigeratorIdBoxEnterCommand = new ParameterCommand(RefrigeratorIdBoxEnterKey);
            _cartridgeIdEnterCommand = new ParameterCommand(CartridgeIdBoxEnterKey);

            EmployeeNameBoxEnabled = true;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = false;

            EmployeeName = string.Empty;
            RefrigeratorId = string.Empty;
            CartridgeId = string.Empty;
            DBLock = new object();
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
                CartridgeList = new ObservableCollection<Cartucho>(_cartridgeList.Where(c=> c.Estado >= 0 && c.Estado <= 3).ToList().OrderBy(c => c.FechaEntrada));
                CartridgeList = new ObservableCollection<Cartucho>(CartridgeList.OrderByDescending(c => c.FechaSalida));
                SetTimeToEachElement();
                OnPropertyChanged("CartridgeList");
            }
        }

        private void SetTimeToEachElement()
        {
            for ( int i =0; i <CartridgeList.Count; i++)
                CartridgeList.ElementAt(i).Tiempo = tiempo;
        }

        #endregion UpdatingList

        #region SQLRawValidations

        private bool isEmployeeValid()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SPManagementDbContext"].ConnectionString;
            string query = "SELECT EmployeeNumber FROM Users u WHERE u.EmployeeNumber =" + "'" + Regex.Replace(EmployeeName," ",string.Empty) + "'";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string EmployeeNumber = (reader["EmployeeNumber"].ToString().ToUpper());
                if (Regex.Replace(EmployeeName, " ", string.Empty).ToUpper().Equals(Regex.Replace(EmployeeNumber, " ", string.Empty)))
                {
                    connection.Close();
                    return (true);
                }
            }
            connection.Close();
            return (false);
        }


        private bool isCartridgeValid()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MaterialManagementDbContext"].ConnectionString;
            string query = "SELECT SerialNumber FROM MaterialInstances m WHERE m.SerialNumber =" + "'" + Regex.Replace(CartridgeId, " ", string.Empty) + "'";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string CartridgeName = (reader["SerialNumber"].ToString().ToUpper());
                if (Regex.Replace(CartridgeId, " ", string.Empty).ToUpper().Equals(Regex.Replace(CartridgeName, " ", string.Empty)))
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
                MessageBox.Show("Nombre de Empleado no valido, favor de consultar con su supervisor", "ERROR");
            
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

        private async void CartridgeIdBoxEnterKey(object box)
        {
            TextBox employeeNameBox = box as TextBox;
            CartridgeIdBoxEnabled = false;
            await Task.Run(() =>
            {
                lock (DBLock)
                {
                    if (isCartridgeValid())
                    {
                        if (Option)
                        {
                            InsertCartridgeToRefrigerator();
                            UpdateList();
                        }
                        else
                        {
                            UpdateCartridgeStateForOut();
                            UpdateList();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cartucho no identificado");
                        cartridge = null;
                    }
                }
            });

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
            var configuration = new Configuration();
            configuration.DataContext = new ConfigurationVM(configuration.Close);
            configuration.ShowDialog();
        }

        private async void UpdateButton(object boxes)
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
            await Task.Run(() =>
            {
                lock (DBLock)
                {
                    CheckReturnedCartridges();
                    UpdateList();
                }
            });

            tiempo = tiempoBusiness.GetAll().FirstOrDefault();
            dispatcherTimer.Interval = new TimeSpan(0, 0, tiempo.SegundosRefresco);

            EmployeeBox.Focus();
        }

        #endregion ButtonsForMainFunctions

        #region InsertUpdateCartridgeMethods

        private void InsertCartridgeToRefrigerator()
        {
            string cartridgeName = Regex.Replace(CartridgeId, " ", string.Empty);
            var query = cartuchoBusiness.GetAllCartuchosByIQueryable().Where(c => c.NumeroDeCartucho == cartridgeName).FirstOrDefault();
            if (query == null)
            {
                cartridge = new Cartucho()
                {
                    NumeroDeCartucho = cartridgeName,
                    NombreRefrigerador = refrigerator.NumeroDeRefrigerador,
                    FechaEntrada = DateTime.Now,
                    FechaSalida = DateTime.MaxValue,
                    FechaRecepcion = DateTime.MaxValue,
                    FechaTerminacion = DateTime.MaxValue,
                    Estado = ZERO_STATE,
                    RefrigeradorId = refrigerator.Id
                };

                binnacleOfMovement = new BitacoraDeMovimientos()
                {
                    RefrigeradorId = refrigerator.Id,
                    UserId = Regex.Replace(EmployeeName, " ", string.Empty),
                    NumeroDeCartucho = Regex.Replace(CartridgeId, " ", string.Empty),
                    FechaMovimiento = DateTime.Now,
                    TipoMovimiento = 0
                };

                CartridgeList.Add(cartridge);
                cartuchoBusiness.InsertCartucho(cartridge);
                bitacoraDeMovimientosBusiness.InsertMovimiento(binnacleOfMovement);
                MessageBox.Show("Cartucho introducido exitosamente");
            }
            else
            {
                if (query.Estado.Equals(ONE_STATE) || query.Estado.Equals(TWO_STATE))
                {
                    var cartridge = CartridgeList.Where(c => c.Id == query.Id).ToList().FirstOrDefault();
                    if (cartridge != null)
                    {
                        query.FechaEntrada = DateTime.Now;
                        query.FechaSalida = DateTime.MaxValue;
                        query.FechaRecepcion = DateTime.MaxValue;
                        query.FechaTerminacion = DateTime.MaxValue;
                        query.Estado = THIRD_STATE;
                        query.NombreRefrigerador = refrigerator.NumeroDeRefrigerador;
                        cartuchoBusiness.UpdateCartucho(query);

                        cartridge.FechaEntrada = DateTime.Now;
                        cartridge.FechaSalida = DateTime.MaxValue;
                        cartridge.FechaRecepcion = DateTime.MaxValue;
                        cartridge.FechaTerminacion = DateTime.MaxValue;
                        cartridge.Estado = THIRD_STATE;
                        cartridge.NombreRefrigerador = refrigerator.NumeroDeRefrigerador;

                        binnacleOfMovement = new BitacoraDeMovimientos()
                        {
                            RefrigeradorId = refrigerator.Id,
                            UserId = Regex.Replace(EmployeeName, " ", string.Empty),
                            NumeroDeCartucho = Regex.Replace(CartridgeId, " ", string.Empty),
                            FechaMovimiento = DateTime.Now,
                            TipoMovimiento = 3
                        };

                        bitacoraDeMovimientosBusiness.InsertMovimiento(binnacleOfMovement);
                        binnacleOfMovement = null;
                    }
                    else
                        MessageBox.Show("El cartucho seleccionado existe pero la aplicación no se encuentra actualizada, favor de actualizar la aplicación antes de realizar el movimiento");
                }
                else
                    MessageBox.Show("No es posible insertar de nuevo ese cartucho, favor de actualizar su aplicación en caso de no verlo", "ERROR");
            }
        }


        private void UpdateCartridgeStateForOut()
        {
            var query = cartuchoBusiness.GetAllCartuchosByRefrigeradorIdByIQueryable(refrigerator.Id).ToList().Where(c => c.NumeroDeCartucho == CartridgeId).FirstOrDefault();
            if (query != null)
            {
                if (query.Estado.Equals(ZERO_STATE) || query.Estado.Equals(THIRD_STATE))
                {
                    var cartridge = CartridgeList.Where(c => c.Id == query.Id).ToList().FirstOrDefault();
                    if (cartridge != null)
                    {
                        var q = CartridgeList.Where(c => c.Estado == 0 || c.Estado == 3).ToList();
                        if (CartridgeList.Where(c => c.Estado == 0 || c.Estado == 3).FirstOrDefault().NumeroDeCartucho.Equals(cartridge.NumeroDeCartucho))
                        {
                            query.FechaSalida = DateTime.Now;
                            query.Estado = ONE_STATE;
                            cartridge.FechaSalida = DateTime.Now;
                            cartridge.Estado = ONE_STATE;
                            cartuchoBusiness.UpdateCartucho(query);

                            binnacleOfMovement = new BitacoraDeMovimientos()
                            {
                                RefrigeradorId = refrigerator.Id,
                                UserId = Regex.Replace(EmployeeName, " ", string.Empty),
                                NumeroDeCartucho = Regex.Replace(CartridgeId, " ", string.Empty),
                                FechaMovimiento = DateTime.Now,
                                TipoMovimiento = 1
                            };

                            bitacoraDeMovimientosBusiness.InsertMovimiento(binnacleOfMovement);
                            binnacleOfMovement = null;
                        }
                        else
                            MessageBox.Show("El cartucho seleccionado existe, pero no debe de ser el primero en salir. Favor de seleccionar el primer cartucho en lista, o pulsar el botón actualizar en busca de cambios");
                    }
                    else
                        MessageBox.Show("El cartucho seleccionado existe pero la aplicación no se encuentra actualizada, favor de actualizar la aplicación antes de realizar el movimiento");
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
            lock (DBLock)
            {
                CheckReturnedCartridges();
                tiempo = tiempoBusiness.GetAll().FirstOrDefault();
                dispatcherTimer.Interval = new TimeSpan(0, 0, tiempo.SegundosRefresco);
                UpdateList();
            }
        }

        private void CheckReturnedCartridges()
        {
            var query = cartuchoBusiness.GetAll().Where(c => c.Estado == 3);
            if (query != null)
            {
                var minTimeQuery = cartuchoBusiness.GetAll().Where(c => c.Estado == 0 || c.Estado == 3).OrderBy(c => c.FechaEntrada).FirstOrDefault();
                foreach (var cartridge in query)
                {
                    TimeSpan t = DateTime.Now.Subtract(cartridge.FechaEntrada);
                    if ((t.Hours + t.Days * 24) >= tiempo.HorasReposoTrasRetorno)
                    {
                        cartridge.FechaEntrada = minTimeQuery.FechaEntrada;
                        cartuchoBusiness.UpdateCartucho(cartridge);
                    }
                }
            }
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
