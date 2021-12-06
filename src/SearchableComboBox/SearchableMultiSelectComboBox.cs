using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using IList = System.Collections.IList;

namespace HoveyTech.SearchableComboBox
{
    public class SearchableMultiSelectComboBox : ItemsControl
    {
        private Border _popupBorder;
        private Button _popupButton;
        private TextBox _filterTextBox;
        private FrameworkElement _selectedItemControl;
        private ItemsPresenter _itemsPresenter;
        private Popup _popup;
        private TextBlock _placeholderTextBlock;
        private ProgressRing _progressRing;
        private TextBlock _noItemsTextBlock;
        private Button _dropdownIcon;
        private Grid _popupGrid;

        private bool _hasFocus;
        private bool _pointerOver;

        public const string PopupButtonName = "PopupButton";
        public const string PopupBorderName = "PopupBorder";
        public const string PlaceholderTextBlockName = "PlaceholderTextBlock";
        public const string SelectedItemControlName = "SelectedItemControl";
        public const string PopupName = "Popup";
        public const string FilterTextBoxName = "FilterTextBox";
        public const string ProgressRingName = "ProgressRing";
        public const string NoItemsTextBlockName = "NoItemsTextBlock";
        public const string ItemsPresenterName = "ItemsPresenter";

        public SearchableMultiSelectComboBox()
        {
            DefaultStyleKey = typeof(SearchableMultiSelectComboBox);
        }

        protected override void OnItemsChanged(object e)
        {
            UpdateItemsControlVisibility();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Height = double.MaxValue;

            _popupButton = GetTemplateChild(PopupButtonName) as Button;
            _placeholderTextBlock = GetTemplateChild(PlaceholderTextBlockName) as TextBlock;
            _popup = GetTemplateChild(PopupName) as Popup;
            _filterTextBox = GetTemplateChild(FilterTextBoxName) as TextBox;
            _selectedItemControl = GetTemplateChild(SelectedItemControlName) as FrameworkElement;
            _progressRing = GetTemplateChild(ProgressRingName) as ProgressRing;
            _noItemsTextBlock = GetTemplateChild(NoItemsTextBlockName) as TextBlock;
            _popupBorder = GetTemplateChild(PopupBorderName) as Border;
            _itemsPresenter = GetTemplateChild(ItemsPresenterName) as ItemsPresenter;
            _dropdownIcon = GetTemplateChild("DropdownIcon") as Button;
            _popupGrid = GetTemplateChild("PopupGrid") as Grid;
            
            IsEnabledChanged += OnIsEnabledChanged;
            Tapped += OnElementTapped;

            if (_filterTextBox != null)
            {
                //_filterTextBox.LostFocus += HandleLostFocus;
                _filterTextBox.KeyUp += FilterTextBoxOnKeyUp;
            }

            if (_selectedItemControl != null)
                _selectedItemControl.Tapped += OnElementTapped;
            if (_placeholderTextBlock != null)
                _placeholderTextBlock.Tapped += OnElementTapped;
            if (_dropdownIcon != null)
                _dropdownIcon.Tapped += DropdownIconOnTapped;
            if (_popup != null)
                _popup.Opened += PopupOnOpened;
            ClosePopup();
            UpdateItemsControlVisibility();
        }

        private void PopupOnOpened(object? sender, object e)
        {
            _popup.RequestedTheme = RequestedTheme;
        }

        private void DropdownIconOnTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;

            if (_popup.IsOpen)
            {
                ClosePopup();
            }
            else
            {
                ShowFilter();
            }
        }

        #region Dependency Properties

        public static readonly DependencyProperty ClearFilterTextOnSelectionProperty = DependencyProperty.Register(
            nameof(ClearFilterTextOnSelection), typeof(bool), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(
            nameof(SelectedItemTemplate), typeof(DataTemplate), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTypeProperty = DependencyProperty.Register(
            nameof(ItemType), typeof(Type), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(null));

        public static readonly DependencyProperty IsRefreshingItemsSourceProperty = DependencyProperty.Register(
           nameof(IsRefreshingItemsSource), typeof(bool), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(false, IsRefreshingItemsSourceChangedCallback));

        public static readonly DependencyProperty UseLoadingProgressRingProperty = DependencyProperty.Register(
           nameof(UseLoadingProgressRing), typeof(bool), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty IsItemsSourceReadyProperty = DependencyProperty.Register(
           nameof(IsItemsSourceReady), typeof(bool), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
           nameof(SelectedItems), typeof(object), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(null, SelectedItemsChangedCallback));

        public static readonly DependencyProperty FilterPlaceholderTextProperty = DependencyProperty.Register(
            nameof(FilterPlaceholderText), typeof(string), typeof(SearchableMultiSelectComboBox), new PropertyMetadata("Filter text..."));

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
           nameof(PlaceholderText), typeof(string), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(
           nameof(FilterText), typeof(string), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(null, FilterTextChangedCallback));

        public static readonly DependencyProperty ItemsSourceEmptyMessageProperty = DependencyProperty.Register(
            nameof(ItemsSourceEmptyMessage), typeof(string), typeof(SearchableMultiSelectComboBox), new PropertyMetadata("No items..."));

        public static readonly DependencyProperty FlyoutMaxHeightProperty = DependencyProperty.Register(
            nameof(FlyoutMaxHeight), typeof(int), typeof(SearchableMultiSelectComboBox), new PropertyMetadata(300));

        #endregion

        #region Properties

        public bool ClearFilterTextOnSelection
        {
            get => (bool)GetValue(ClearFilterTextOnSelectionProperty);
            set => SetValue(ClearFilterTextOnSelectionProperty, value);
        }

        public DataTemplate SelectedItemTemplate
        {
            get => (DataTemplate)GetValue(SelectedItemTemplateProperty);
            set => SetValue(SelectedItemTemplateProperty, value);
        }

        public Type ItemType
        {
            get => (Type)GetValue(ItemTypeProperty);
            set => SetValue(ItemTypeProperty, value);
        }

        public int FlyoutMaxHeight
        {
            get => (int)GetValue(FlyoutMaxHeightProperty);
            set => SetValue(FlyoutMaxHeightProperty, value);
        }

        public string ItemsSourceEmptyMessage
        {
            get => (string)GetValue(ItemsSourceEmptyMessageProperty);
            set => SetValue(ItemsSourceEmptyMessageProperty, value);
        }

        public bool IsRefreshingItemsSource
        {
            get { return (bool)GetValue(IsRefreshingItemsSourceProperty); }
            set { SetValue(IsRefreshingItemsSourceProperty, value); }
        }

        public bool UseLoadingProgressRing
        {
            get { return (bool)GetValue(UseLoadingProgressRingProperty); }
            set { SetValue(UseLoadingProgressRingProperty, value); }
        }

        public bool IsItemsSourceReady
        {
            get { return (bool)GetValue(IsItemsSourceReadyProperty); }
            set { SetValue(IsItemsSourceReadyProperty, value); }
        }

        public object SelectedItems
        {
            get { return GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public string FilterPlaceholderText
        {
            get { return (string)GetValue(FilterPlaceholderTextProperty); }
            set { SetValue(FilterPlaceholderTextProperty, value); }
        }

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        #endregion

        #region Overrides

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab
                || e.Key == VirtualKey.Shift
                || e.Key == VirtualKey.LeftShift
                || e.Key == VirtualKey.RightShift) return;

            e.Handled = true;
            ShowFilter();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            _pointerOver = true;
            UpdateStates();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            _pointerOver = false;
            UpdateStates();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            _pointerOver = true;
            UpdateStates();
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            _pointerOver = false;
            UpdateStates();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var control = (SearchableMultiSelectComboBoxItem) element;

            var selectedItems = (SelectedItems as IList) ?? new List<object>();

            control.Selected = selectedItems.IndexOf(item) > -1;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SearchableMultiSelectComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new SearchableMultiSelectComboBoxItem(this);
            item.FontSize = FontSize;
            return item;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_popupBorder != null)
                _popupBorder.Width = finalSize.Width;

            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #region Events

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            UpdateStates();
        }

        private void FilterTextBoxOnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
        {
            var key = keyRoutedEventArgs.Key;

            if (key == VirtualKey.Escape)
            {
                ClosePopup();
                return;
            }

            if (key != VirtualKey.Enter
                && key != VirtualKey.Down
                && key != VirtualKey.Up)
                return;

            var list = (IList)ItemsSource;
            if (list == null) return;
            if (list.Count == 0) return;

            if (key == VirtualKey.Enter)
            {
                if (list.Count == 1)
                {
                    var control = GetItemControlFromObject(list[0]);
                    control.Selected = !control.Selected;
                    UpdateSelectedItems();
                    return;
                }

                foreach (var item in list)
                {
                    var control = GetItemControlFromObject(item);

                    if (control == null)
                        continue;

                    if (control.IsPointerEntered)
                    {
                        control.Selected = !control.Selected;
                        UpdateSelectedItems();
                        return;
                    }
                }
            }
            else
            {
                if (list.Count < 2) return;

                int index = -1;
                SearchableMultiSelectComboBoxItem pointerEnteredControl = null;

                foreach (var item in list)
                {
                    var control = GetItemControlFromObject(item);

                    if (control == null)
                        continue;

                    if (control.IsPointerEntered)
                    {
                        index = list.IndexOf(item);
                        pointerEnteredControl = control;
                        break;
                    }
                }

                SearchableMultiSelectComboBoxItem nextItem;

                if (pointerEnteredControl != null)
                    pointerEnteredControl.IsPointerEntered = false;

                if (key == VirtualKey.Up)
                {
                    if (index < 1)
                        nextItem = GetItemControlFromIndex(list.Count - 1);
                    else
                        nextItem = GetItemControlFromIndex(index - 1);
                }
                else
                {
                    if (index == list.Count - 1 || index == -1)
                        nextItem = GetItemControlFromIndex(0);
                    else
                        nextItem = GetItemControlFromIndex(index + 1);
                }

                if (nextItem != null)
                    nextItem.IsPointerEntered = true;
            }
        }

        private static void IsRefreshingItemsSourceChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = (SearchableMultiSelectComboBox)dependencyObject;
            comboBox.UpdateItemsControlVisibility();
        }

        private static void FilterTextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as SearchableMultiSelectComboBox;

            if (control == null)
                return;

            var newFilterText = e?.NewValue as string;

            if (!string.IsNullOrEmpty(newFilterText))
            {
                if (control._placeholderTextBlock != null)
                    control._placeholderTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private static void SelectedItemsChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchableMultiSelectComboBox)dependencyObject;
            control.UpdateItemsControlVisibility();
        }

        private void OnElementTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            tappedRoutedEventArgs.Handled = true;
            ShowFilter();
        }

        private void HandleLostFocus(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        #endregion

        private void UpdateStates()
        {
            if (!IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", false);
            else if (_hasFocus)
                VisualStateManager.GoToState(this, "Focused", false);
            else if (_pointerOver)
                VisualStateManager.GoToState(this, "PointerOver", false);
            else
                VisualStateManager.GoToState(this, "Normal", false);
        }

        private void UpdateItemsControlVisibility()
        {
            var isItems = GetListInternal().Count > 0;
            var loadingItems = UseLoadingProgressRing && IsRefreshingItemsSource;

            if (_itemsPresenter != null)
                _itemsPresenter.Visibility = loadingItems ? Visibility.Collapsed : Visibility.Visible;
            if (_progressRing != null)
                _progressRing.Visibility = loadingItems ? Visibility.Visible : Visibility.Collapsed;
            if (_noItemsTextBlock != null)
            {
                _noItemsTextBlock.Visibility = isItems || loadingItems ? Visibility.Collapsed : Visibility.Visible;
                _noItemsTextBlock.Text = ItemsSourceEmptyMessage;
            }

            UpdateLayout();

            bool anySelected = false;

            if (SelectedItems is IList selectedItems)
            {
                if (ItemsSource is IList itemsSource)
                {
                    foreach (var item in itemsSource)
                    {
                        var control = GetItemControlFromObject(item);

                        if (control == null)
                            continue;

                        control.Selected = selectedItems.IndexOf(item) > -1;
                    }

                }

                anySelected = selectedItems.Count > 0;
            }

            if (_placeholderTextBlock != null)
            {
                _placeholderTextBlock.Visibility = anySelected ? Visibility.Collapsed : Visibility.Visible;
                _placeholderTextBlock.Text = (PlaceholderText ?? ItemsSourceEmptyMessage);
            }

            UpdateLayout();
        }

        private IList GetListInternal()
        {
            if (ItemsSource == null)
                return new List<object>();
            return (IList)ItemsSource;
        }

        private void ClosePopup()
        {
            _hasFocus = false;
            UpdateStates();

            _popup.IsOpen = false;
        }

        private void ShowFilter()
        {
            var hadFocus = _hasFocus;

            _hasFocus = true;
            UpdateStates();

            _popup.VerticalOffset = ActualHeight;
            _popup.IsOpen = true;
            _popupGrid.Width = ActualWidth;

            if (_popupBorder != null)
                _popupBorder.RequestedTheme = ActualTheme;
            if (_popupGrid != null)
                _popupGrid.RequestedTheme = ActualTheme;

            UpdateItemsControlVisibility();

            _filterTextBox.Focus(FocusState.Programmatic);
            _filterTextBox.Focus(FocusState.Keyboard);
        }

        public void OnSelectedItemChanged(SearchableMultiSelectComboBoxItem tappedItem)
        {
            UpdateSelectedItems();
        }

        private void UpdateSelectedItems()
        {
            if (ItemType == null)
                throw new Exception($"{nameof(ItemType)} property is required.");

            var selectedItems = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ItemType));

            if (SelectedItems != null)
            {
                var listType = typeof(IList<>).MakeGenericType(ItemType);
                var validType = listType.IsInstanceOfType(SelectedItems);

                if (!validType)
                    throw new Exception($"SelectItems must be derived from IList<{listType}>.");

                var existingSelectedItems = (IList)SelectedItems;

                foreach (var item in existingSelectedItems)
                    selectedItems.Add(item);
            }

            if (ItemsSource is IList itemsList)
            {
                foreach (var item in itemsList)
                {
                    var control = GetItemControlFromObject(item);

                    if (control == null)
                        continue;

                    var selectedItemIndex = selectedItems.IndexOf(item);

                    if (control.Selected && selectedItemIndex == -1)
                        selectedItems.Add(item);
                    else if (!control.Selected && selectedItemIndex > -1)
                        selectedItems.Remove(item);
                }
            }

            SelectedItems = selectedItems;
            UpdateLayout();
            _popup.VerticalOffset = ActualHeight;
            UpdateItemsControlVisibility();

            if (ClearFilterTextOnSelection && FilterText != null)
                FilterText = string.Empty;
        }

        public void ClearSelection()
        {
            var list = (IList)ItemsSource;
            if (list == null) return;

            foreach (var item in list)
            {
                SearchableMultiSelectComboBoxItem control = GetItemControlFromObject(item);
                if (control == null) continue;

                control.Selected = false;
            }

            var typedList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ItemType));
            SelectedItems = typedList;
        }

        private SearchableMultiSelectComboBoxItem GetItemControlFromIndex(int index)
        {
            return ContainerFromIndex(index) as SearchableMultiSelectComboBoxItem;
        }

        private SearchableMultiSelectComboBoxItem GetItemControlFromObject(object item)
        {
            return ContainerFromItem(item) as SearchableMultiSelectComboBoxItem;
        }
    }
}
