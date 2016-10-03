using MPManagement.ViewModels;
using MPManagement.ViewModels.Commands;
using MPManagement.ViewModels.Controls;
using MPManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MPManagement
{
    public class MainWindowVM : ViewModelBase
    {
        #region Properties
        private static MainWindowVM _instance = new MainWindowVM();
        public static MainWindowVM Instance => _instance;

        private readonly ParameterCommand _clearBoxesCommand;
        public ParameterCommand ClearBoxesCommand => _clearBoxesCommand;

        private readonly ParameterCommand _employeeNameEnterCommand;
        public ParameterCommand EmployeeNameEnterCommand => _employeeNameEnterCommand;

        private readonly ParameterCommand _refrigeratorIdBoxEnterCommand;
        public ParameterCommand RefrigeratorIdBoxEnterCommand => _refrigeratorIdBoxEnterCommand;

        private readonly ParameterCommand _cartridgeIdEnterCommand;
        public ParameterCommand CartridgeIdEnterCommand => _cartridgeIdEnterCommand;

        public ObservableCollection<SideBarItemVM> SideBarItems { get; private set; }

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

        private ContentControl _currentView;
        public ContentControl CurrentView
        {
            get
            {
                return _currentView;
            }

            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }

        }

        #endregion Properties

        public MainWindowVM()
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

            SideBarItems = new ObservableCollection<SideBarItemVM>();

            SideBarItems.Add(new SideBarItemVM("Entrada", new RelayCommand(() => UpdateUI(new MagneticPasteInOut() { DataContext = new MagneticPasteInOutVM() }))));
            SideBarItems.Add(new SideBarItemVM("Salida", new RelayCommand(() => UpdateUI(new MagneticPasteInOut() { DataContext = new MagneticPasteInOutVM() }))));
            SideBarItems.Add(new SideBarItemVM("Bitacora", new RelayCommand(() => UpdateUI(new MagneticPasteBinnacle() { DataContext = new MagneticPasteBinnacleVM() }))));

            SideBarItems[0].Command.Execute();
        }

        public void UpdateUI(ContentControl view) => CurrentView = view;

        private void EmployeeBoxEnterKey(object box)
        {
            TextBox refrigeratorIdBox = box as TextBox;  
                      
            RefrigeratorIdBoxEnabled = true;
            EmployeeNameBoxEnabled = false;
            CartridgeIdBoxEnabled = false;
            refrigeratorIdBox.Focus();
        }

        private void RefrigeratorIdBoxEnterKey(object box)
        {
            TextBox cartridgeIdBox = box as TextBox;

            EmployeeNameBoxEnabled = false;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = true;
            cartridgeIdBox.Focus();
        }

        private void CartridgeIdBoxEnterKey(object box)
        {
            TextBox employeeNameBox = box as TextBox;

            //do some action for the datagrid

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

            EmployeeBox.Focus();
        }
    }
}
