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

        public RelayCommand InputFilesViewCommand { get; set; }
        
        public RelayCommand DictionaryViewCommand { get; set; }

        public InputFilesViewModel InputVM { get; set; }

        public DictionaryViewModel DictionaryVM { get; set; }

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
            DictionaryVM = new DictionaryViewModel();

            CurrentView = InputVM;

            InputFilesViewCommand = new RelayCommand(o =>
            {
                CurrentView = InputVM;
            }); 

            DictionaryViewCommand = new RelayCommand(o =>
            {
                CurrentView = DictionaryVM;
            });
        }
    }
}
