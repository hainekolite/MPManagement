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

        private readonly RelayCommand _refrigeratorIdBoxEnterCommand;
        public RelayCommand RefrigeratorIdBoxEnterCommand => _refrigeratorIdBoxEnterCommand;

        private readonly RelayCommand _cartridgeIdEnterCommand;
        public RelayCommand CartridgeIdEnterCommand => _cartridgeIdEnterCommand;

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

        private bool _cartridgeIdBoxBoxEnabled;
        public bool CartridgeIdBoxEnabled
        {
            get
            {
                return (_cartridgeIdBoxBoxEnabled);
            }
            set
            {
                _cartridgeIdBoxBoxEnabled = value;
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

        private string _cartridgeIdBox;
        public string CartridgeIdBox
        {
            get
            {
                return (_cartridgeIdBox);
            }
            set
            {
                _cartridgeIdBox = value;
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
            _refrigeratorIdBoxEnterCommand = new RelayCommand(RefrigeratorIdBoxEnterKey);
            _cartridgeIdEnterCommand = new RelayCommand(CartridgeIdBoxEnterKey);

            _employeeNameBoxEnabled = true;
            _refrigeratorIdBoxEnabled = false;
            _cartridgeIdBoxBoxEnabled = false;

            SideBarItems = new ObservableCollection<SideBarItemVM>();

            SideBarItems.Add(new SideBarItemVM("Entrada", new RelayCommand(() => UpdateUI(new MagneticPasteIn() { DataContext = new MagneticPasteInVM() }))));
            SideBarItems.Add(new SideBarItemVM("Salida", new RelayCommand(() => UpdateUI(new MagneticPasteOut() { DataContext = new MagneticPasteOutVM() }))));
            SideBarItems.Add(new SideBarItemVM("Bitacora", new RelayCommand(() => UpdateUI(new MagneticPasteBinnacle() { DataContext = new MagneticPasteBinnacleVM() }))));

            SideBarItems[0].Command.Execute();
        }

        public void UpdateUI(ContentControl view) => CurrentView = view;

        private void EmployeeBoxEnterKey(object box)
        {
            RefrigeratorIdBoxEnabled = true;
            EmployeeNameBoxEnabled = false;
            CartridgeIdBoxEnabled = false;
        }

        private void RefrigeratorIdBoxEnterKey()
        {
            EmployeeNameBoxEnabled = false;
            RefrigeratorIdBoxEnabled = false;
            CartridgeIdBoxEnabled = true;
        }

        private void CartridgeIdBoxEnterKey()
        {
            //do some action
        }

        private void ClearAllBoxes(object boxes)
        {
            var values = boxes as object[];
            TextBox EmployeeBox = values[0] as TextBox;
            TextBox RefrigeratorBox = values[1] as TextBox;
            TextBox CartridgeBox = values[2] as TextBox;

            EmployeeBox.Text = string.Empty;
            RefrigeratorBox.Text = string.Empty;
            CartridgeBox.Text = string.Empty;

            EmployeeBox.IsEnabled = true;
            RefrigeratorBox.IsEnabled = false;
            CartridgeBox.IsEnabled = false;

            EmployeeBox.Focus();
        }
    }
}
