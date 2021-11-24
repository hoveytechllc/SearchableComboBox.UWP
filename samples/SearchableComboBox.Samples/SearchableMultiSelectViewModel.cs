using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SearchableComboBox.Samples
{
    public class SearchableMultiSelectViewModel : BaseViewModel
    {
        private readonly bool _useDelay;
        private readonly Action _selectedItemsChanged;

        private readonly IList<Person> _initialItems;

        public SearchableMultiSelectViewModel(bool useDelay = true,
            Action selectedItemsChanged = null)
        {
            _useDelay = useDelay;
            _selectedItemsChanged = selectedItemsChanged;

            _initialItems = new List<Person>()
                {
                    new Person() { FirstName = "Amya", LastName = "Vazquez" },
                    new Person() { FirstName = "Charlotte", LastName = "Serrano" },
                    new Person() { FirstName = "Brennen", LastName = "Bowman" },
                    new Person() { FirstName = "Arely", LastName = "Anderson" },
                    new Person() { FirstName = "Aron", LastName = "Barry" },
                    new Person() { FirstName = "Janelle", LastName = "Tanner" },
                    new Person() { FirstName = "Coleman", LastName = "Yates" },
                    new Person() { FirstName = "Zackary", LastName = "Sparks" },
                    new Person() { FirstName = "Boston", LastName = "Pitts" },
                    new Person() { FirstName = "Miley", LastName = "Haley" },
                    new Person() { FirstName = "Madelynn", LastName = "Frey" },
                    new Person() { FirstName = "Danna", LastName = "Snyder" }
                }
                .OrderBy(x => x.LastName)
                .ToList();

            foreach (var item in _initialItems)
                List.Add(item);

            _selectedEntities = new List<Person>()
            {
                _initialItems.ElementAt(0),
                _initialItems.ElementAt(1),
            };
        }

        public ObservableCollection<Person> List { get; } = new ObservableCollection<Person>();

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
                                                 || x.IsMatch(filterText))
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

        public Type ItemType => typeof(Person);

        private IList<Person> _selectedEntities;
        public virtual IList<Person> SelectedEntities
        {
            get => _selectedEntities;
            set
            {
                _selectedEntities = value;
                RaisePropertyChanged();
                _selectedItemsChanged?.Invoke();
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
    }
}
