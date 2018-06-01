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
        }

        public SearchableListViewModel Simple { get; }

        public SearchableListViewModel LoadingIndicator { get; }

        public SearchableListViewModel FlexibleFilterText { get; }
    }
}
