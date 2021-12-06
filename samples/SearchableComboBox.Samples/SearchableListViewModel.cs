using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SearchableComboBox.Samples
{
    public class SearchableListViewModel : BaseViewModel
    {
        private readonly bool _useDelay;

        private readonly IList<string> _initialItems;

        public SearchableListViewModel(bool useDelay = true)
        {
            _useDelay = useDelay;

            _initialItems = new List<string>()
                {
                    "Amya Vazquez",
                    "Charlotte Serrano",
                    "Brennen Bowman",
                    "Arely Anderson",
                    "Aron Barry",
                    "Janelle Tanner",
                    "Coleman Yates",
                    "Zackary Sparks",
                    "Boston Pitts",
                    "Miley Haley",
                    "Madelynn Frey",
                    "Danna Snyder"
                }.OrderBy(x => x)
                .ToList();

            RaisePropertyChanged(() => List);
        }
        
        public ObservableCollection<string> List { get; } = new ObservableCollection<string>();

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                RaisePropertyChanged(() => FilterText);
                HandleChangedFilterText(_filterText);
            }
        }
        
        private void ExecuteFiltering(string filterText)
        {
            filterText = filterText?.Trim().ToLower();

            var items = _initialItems.Where(x => string.IsNullOrEmpty(filterText)
                                                 || x.ToLower().Contains(filterText))
                .Take(10)
                .ToList();

 MainThreadInvoker.Invoke(() =>
            {
                List.Clear();
                foreach (var item in items)
                    List.Add(item);

                IsSearching = false;
            });
        }

        private void HandleChangedFilterText(string filterText)
        {
            IsSearching = true;

            if (!_useDelay)
            {
                ExecuteFiltering(filterText);
                return;
            }

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                ExecuteFiltering(filterText);
            });
        }

        private string _selectedEntity;
        public virtual string SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                _selectedEntity = value;
                RaisePropertyChanged();
            }
        }

        private bool _isSearching;
        public virtual bool IsSearching
        {
            get => _isSearching;
            set
            {
                _isSearching = value;
                RaisePropertyChanged(() => IsSearching);
            }
        }

        public string NoItemsMessage => "No-one found...";
    }
}
