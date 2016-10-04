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
        public ObservableCollection<SideBarItemVM> SideBarItems { get; private set; }
       
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
            SideBarItems = new ObservableCollection<SideBarItemVM>();

            SideBarItems.Add(new SideBarItemVM("Entrada", new RelayCommand(() => UpdateUI(new MagneticPasteInOut() { DataContext = new MagneticPasteInOutVM() }))));
            SideBarItems.Add(new SideBarItemVM("Salida", new RelayCommand(() => UpdateUI(new MagneticPasteInOut() { DataContext = new MagneticPasteInOutVM() }))));
            SideBarItems.Add(new SideBarItemVM("Bitacora", new RelayCommand(() => UpdateUI(new MagneticPasteBinnacle() { DataContext = new MagneticPasteBinnacleVM() }))));

            SideBarItems[0].Command.Execute();
        }

        public void UpdateUI(ContentControl view) => CurrentView = view;

    }
}
