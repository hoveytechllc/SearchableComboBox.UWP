using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Core;

namespace SearchableComboBox.UWP.Sample.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        public MainViewModel(IMvxMainThreadDispatcher mvxMainThreadDispatcher)
        {
            Simple = new SearchableListViewModel(mvxMainThreadDispatcher, useDelay: false);
            LoadingIndicator = new SearchableListViewModel(mvxMainThreadDispatcher);
            FlexibleFilterText = new SearchableListViewModel(mvxMainThreadDispatcher);
            MultiSelect = new SearchableMultiSelectViewModel(mvxMainThreadDispatcher,
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

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            SelectedItemsChanged();
        }

        public SearchableListViewModel Simple { get; }

        public SearchableListViewModel LoadingIndicator { get; }

        public SearchableListViewModel FlexibleFilterText { get; }

        public SearchableMultiSelectViewModel MultiSelect { get; }

        public string ItemsSelected { get; set; }
    }
}
