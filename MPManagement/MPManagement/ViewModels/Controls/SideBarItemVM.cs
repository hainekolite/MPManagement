using MPManagement.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPManagement.ViewModels.Controls
{
    public sealed class SideBarItemVM : ViewModelBase
    {
        private readonly string _text;
        public string Text => _text;

        private readonly RelayCommand _command;
        public RelayCommand Command => _command;

        public SideBarItemVM(string text, RelayCommand command)
        {
            _text = text;
            _command = command;
        }

    }
}
