using System.Linq;

namespace SearchableComboBox.Samples
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Simple = new SearchableListViewModel(useDelay: false);
            LoadingIndicator = new SearchableListViewModel();
            FlexibleFilterText = new SearchableListViewModel();
            MultiSelect = new SearchableMultiSelectViewModel(
                useDelay: true,
                selectedItemsChanged: SelectedItemsChanged);

            SelectedItemsChanged();
        }

        private void SelectedItemsChanged()
        {
            if (MultiSelect.SelectedEntities != null && MultiSelect.SelectedEntities.Any())
            {
                var items = MultiSelect.SelectedEntities
                    .Select(x => x.FirstName)
                    .ToList();

                ItemsSelected = $"Selected: {string.Join(", ", items)}";
            }
            else
                ItemsSelected = "nothing selected.";

            RaisePropertyChanged(() => ItemsSelected);
        }
        
        public SearchableListViewModel Simple { get; }

        public SearchableListViewModel LoadingIndicator { get; }

        public SearchableListViewModel FlexibleFilterText { get; }

        public SearchableMultiSelectViewModel MultiSelect { get; }

        public string ItemsSelected { get; set; }
    }
}
