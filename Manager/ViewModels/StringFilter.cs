using System.IO.Ports;
using System.Windows.Input;
using Mvvm;
using Mvvm.Commands;

namespace Manager.ViewModels
{
    public class StringFilter : BindableBase
    {
        private bool _isEnabled = false;
        private string _searchString;
        private ICommand _searchCommand;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled ,value);
        }

        public string SearchString
        {
            get => _searchString;
            set => SetProperty(ref _searchString ,value);
        }

        public ICommand SearchCommand
        {
            get => _searchCommand;
            set => SetProperty(ref _searchCommand ,value);
        }

        public ICommand ClearCommand { get; set; }

        public bool IsMatch(string text)
        {
            if (!IsEnabled)
                return true;

            if (string.IsNullOrEmpty(SearchString))
                return true;

            var tempSearch = SearchString.ToLower();
            var tempName = text.ToLower();

            return tempName.Contains(tempSearch) 
                   || tempSearch.Contains(tempName);
        }

        public StringFilter(ICommand onSearchCommand)
        {
            SearchCommand = onSearchCommand;
            
            ClearCommand = new DelegateCommand(() =>
            {
                SearchString = null;
                SearchCommand?.Execute(SearchString);
            });
        }
    }
}