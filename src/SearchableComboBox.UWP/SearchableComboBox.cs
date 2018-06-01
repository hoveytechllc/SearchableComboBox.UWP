using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace HoveyTech.SearchableComboBox.UWP
{
    public class SearchableComboBox : ItemsControl
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

        public SearchableComboBox()
        {
            DefaultStyleKey = typeof(SearchableComboBox);
        }

        protected override void OnItemsChanged(object e)
        {
            UpdateItemsControlVisibility(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popupButton = GetTemplateChild(PopupButtonName) as Button;
            _placeholderTextBlock = GetTemplateChild(PlaceholderTextBlockName) as TextBlock;
            _popup = GetTemplateChild(PopupName) as Popup;
            _filterTextBox = GetTemplateChild(FilterTextBoxName) as TextBox;
            _selectedItemControl = GetTemplateChild(SelectedItemControlName) as FrameworkElement;
            _progressRing = GetTemplateChild(ProgressRingName) as ProgressRing;
            _noItemsTextBlock = GetTemplateChild(NoItemsTextBlockName) as TextBlock;
            _popupBorder = GetTemplateChild(PopupBorderName) as Border;
            _itemsPresenter = GetTemplateChild(ItemsPresenterName) as ItemsPresenter;

            IsEnabledChanged += OnIsEnabledChanged;
            Tapped += OnElementTapped;

            if (_filterTextBox != null)
            {
                _filterTextBox.LostFocus += HandleLostFocus;
                _filterTextBox.KeyUp += FilterTextBoxOnKeyUp;
            }

            if (_selectedItemControl != null)
                _selectedItemControl.Tapped += OnElementTapped;
            if (_placeholderTextBlock != null)
                _placeholderTextBlock.Tapped += OnElementTapped;

            ClosePopup();
        }

        #region Dependency Properties

        public static readonly DependencyProperty InputScopeProperty = DependencyProperty.Register(
           nameof(InputScope), typeof(object), typeof(SearchableComboBox), new PropertyMetadata(InputScopeNameValue.Text));

        public static readonly DependencyProperty IsRefreshingItemsSourceProperty = DependencyProperty.Register(
           nameof(IsRefreshingItemsSource), typeof(object), typeof(SearchableComboBox), new PropertyMetadata(false, IsRefreshingItemsSourceChangedCallback));

        public static readonly DependencyProperty UseLoadingProgressRingProperty = DependencyProperty.Register(
           nameof(UseLoadingProgressRing), typeof(object), typeof(SearchableComboBox), new PropertyMetadata(false));

        public static readonly DependencyProperty IsItemsSourceReadyProperty = DependencyProperty.Register(
           nameof(IsItemsSourceReady), typeof(object), typeof(SearchableComboBox), new PropertyMetadata(true));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
           nameof(SelectedItem), typeof(object), typeof(SearchableComboBox), new PropertyMetadata(null, SelectedItemChangedCallback));

        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
           nameof(PlaceholderText), typeof(string), typeof(SearchableComboBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty FilterTextProperty = DependencyProperty.Register(
           nameof(FilterText), typeof(string), typeof(SearchableComboBox), new PropertyMetadata(null, FilterTextChangedCallback));

        public static readonly DependencyProperty ItemsSourceEmptyMessageProperty = DependencyProperty.Register(
            nameof(ItemsSourceEmptyMessage), typeof(string), typeof(SearchableComboBox), new PropertyMetadata("No items..."));

        public static readonly DependencyProperty UseFilterTextAsUserInputProperty = DependencyProperty.Register(
            nameof(UseFilterTextAsUserInput), typeof(bool), typeof(SearchableComboBox), new PropertyMetadata(false));

        #endregion

        #region Properties

        public bool UseFilterTextAsUserInput
        {
            get => (bool)GetValue(UseFilterTextAsUserInputProperty);
            set => SetValue(UseFilterTextAsUserInputProperty, value);
        }

        public string ItemsSourceEmptyMessage
        {
            get => (string)GetValue(ItemsSourceEmptyMessageProperty);
            set => SetValue(ItemsSourceEmptyMessageProperty, value);
        }

        public object InputScope
        {
            get { return GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
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

        public object IsItemsSourceReady
        {
            get { return GetValue(IsItemsSourceReadyProperty); }
            set { SetValue(IsItemsSourceReadyProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
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

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SearchableComboBoxItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new SearchableComboBoxItem(this);
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
                Focus(FocusState.Programmatic);
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
                    SelectedItem = list[0];
                    ClosePopup();
                    this.Focus(FocusState.Programmatic);
                    return;
                }

                foreach (var item in list)
                {
                    var control = GetItemControlFromObject(item);

                    if (control == null)
                        continue;

                    if (control.IsPointerEntered)
                    {
                        SelectedItem = item;
                        ClosePopup();
                        this.Focus(FocusState.Programmatic);
                        return;
                    }
                }
            }
            else
            {
                if (list.Count < 2) return;

                int index = -1;
                SearchableComboBoxItem pointerEnteredControl = null;

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

                SearchableComboBoxItem nextItem;

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
            UpdateItemsControlVisibility((SearchableComboBox)dependencyObject);
        }

        private static void FilterTextChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchableComboBox)dependencyObject;

            var newFilterText = (string)e.NewValue;

            if (!string.IsNullOrEmpty(newFilterText))
            {
                control._placeholderTextBlock.Visibility = Visibility.Collapsed;

                if (control.UseFilterTextAsUserInput)
                    control.ClearSelection();
            }
        }

        private static void SelectedItemChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = (SearchableComboBox)dependencyObject;

            if (control._placeholderTextBlock == null || control._selectedItemControl == null)
                return;

            var newSelection = e.NewValue;

            if (newSelection != null && !string.IsNullOrEmpty(control.FilterText))
                control.FilterText = null;

            control.ClosePopup();
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

        private static void UpdateItemsControlVisibility(SearchableComboBox control)
        {
            if (control._itemsPresenter == null)
                return;

            var isItems = control.GetListInternal().Count > 0;
            var loadingItems = control.UseLoadingProgressRing && control.IsRefreshingItemsSource;

            control._itemsPresenter.Visibility = loadingItems ? Visibility.Collapsed : Visibility.Visible;
            control._progressRing.Visibility = loadingItems ? Visibility.Visible : Visibility.Collapsed;
            control._noItemsTextBlock.Visibility = isItems || loadingItems ? Visibility.Collapsed : Visibility.Visible;

            var placeHolderText = control.PlaceholderText ?? control.ItemsSourceEmptyMessage;

            control._noItemsTextBlock.Text = string.IsNullOrEmpty(control.FilterText) ? placeHolderText : control.ItemsSourceEmptyMessage;
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

            _selectedItemControl.Visibility = SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;

            if (UseFilterTextAsUserInput)
            {
                _placeholderTextBlock.Visibility = SelectedItem == null && string.IsNullOrEmpty(_filterTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
                _filterTextBox.Visibility = !string.IsNullOrEmpty(_filterTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                _placeholderTextBlock.Visibility = SelectedItem == null ? Visibility.Visible : Visibility.Collapsed;

                _filterTextBox.Text = string.Empty;
                _filterTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowFilter()
        {
            var hadFocus = _hasFocus;

            _hasFocus = true;
            UpdateStates();

            _placeholderTextBlock.Visibility = Visibility.Collapsed;
            _selectedItemControl.Visibility = Visibility.Collapsed;
            _filterTextBox.Visibility = Visibility.Visible;

            UpdateItemsControlVisibility(this);

            _popup.IsOpen = true;
            UpdateLayout();

            if (UseFilterTextAsUserInput
                && !string.IsNullOrEmpty(_filterTextBox.Text)
                && _filterTextBox.FocusState != FocusState.Keyboard)
            {
                _filterTextBox.SelectAll();
            }

            _filterTextBox.Focus(FocusState.Programmatic);
            _filterTextBox.Focus(FocusState.Keyboard);
        }

        public void NotifyItemTapped(SearchableComboBoxItem tappedItem)
        {
            var list = (IList)ItemsSource;
            if (list == null) return;

            foreach (var item in list)
            {
                SearchableComboBoxItem control = GetItemControlFromObject(item);
                if (control == null) continue;

                control.Selected = tappedItem == control;
            }

            SelectedItem = tappedItem.DataContext;
            ClosePopup();
            this.Focus(FocusState.Programmatic);
        }

        public void ClearSelection()
        {
            var list = (IList)ItemsSource;
            if (list == null) return;

            foreach (var item in list)
            {
                SearchableComboBoxItem control = GetItemControlFromObject(item);
                if (control == null) continue;

                control.Selected = false;
            }

            SelectedItem = null;
        }

        private SearchableComboBoxItem GetItemControlFromIndex(int index)
        {
            return ContainerFromIndex(index) as SearchableComboBoxItem;
        }

        private SearchableComboBoxItem GetItemControlFromObject(object item)
        {
            return ContainerFromItem(item) as SearchableComboBoxItem;
        }
    }
}
