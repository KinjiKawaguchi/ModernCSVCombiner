using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModernCSVCombiner.Core;

namespace ModernCSVCombiner.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public InputFilesViewModel InputVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            InputVM = new InputFilesViewModel();
            CurrentView = InputVM;
        }
    }
}
