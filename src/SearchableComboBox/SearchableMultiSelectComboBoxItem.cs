using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace HoveyTech.SearchableComboBox
{
    public class SearchableMultiSelectComboBoxItem : ContentControl
    {
        private readonly SearchableMultiSelectComboBox _parent;

        public const string GridName = "LayoutRoot";
        public const string CheckboxName = "IsSelectedCheckbox";

        public const string NormalStateName = "Normal";
        public const string PointerOverStateName = "PointerOver";
        public const string DisabledStateName = "Disabled"; // not used
        public const string PressedStateName = "Pressed";
        public const string SelectedStateName = "Selected";
        public const string SelectedUnfocusedStateName = "SelectedUnfocused"; // not used
        public const string SelectedDisabledStateName = "SelectedDisabled"; // not used
        public const string SelectedPointerOverStateName = "SelectedPointerOver";
        public const string SelectedPressedStateName = "SelectedPressed";

        private Grid _grid;
        private bool _pressed;
        private CheckBox _checkBox;

        public SearchableMultiSelectComboBoxItem(SearchableMultiSelectComboBox parent)
        {
            _parent = parent;
            DefaultStyleKey = typeof(SearchableMultiSelectComboBoxItem);
        }

        private bool _pointerEntered;
        public bool IsPointerEntered
        {
            get { return _pointerEntered; }
            set
            {
                _pointerEntered = value;
                UpdateStates();
            }
        }

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (_checkBox != null)
                    _checkBox.IsChecked = value;

                UpdateStates();
            }
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            Selected = !Selected;

            _parent.OnSelectedItemChanged(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _grid = GetTemplateChild(GridName) as Grid;
            _checkBox = GetTemplateChild(CheckboxName) as CheckBox;
            
            if (_grid != null)
            {
                _grid.PointerEntered += GridOnPointerEntered;
                _grid.PointerExited += GridOnPointerExited;
                _grid.PointerPressed += GridOnPointerPressed;
                _grid.PointerReleased += GridOnPointerReleased;
            }

            if (_checkBox != null)
                _checkBox.IsChecked = Selected;

            UpdateStates();
        }
        

        private void GridOnPointerEntered(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _pointerEntered = true;
            UpdateStates();
        }

        private void GridOnPointerExited(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _pointerEntered = false;
            UpdateStates();
        }

        private void GridOnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _pressed = false;
            UpdateStates();
        }

        private void GridOnPointerPressed(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            _pressed = true;
            UpdateStates();
        }

        private void UpdateStates()
        {
            if (Selected)
            {
                if (_pointerEntered) GoToState(SelectedPointerOverStateName);
                else if (_pressed) GoToState(SelectedPressedStateName);
                else GoToState(SelectedStateName);
            }
            else
            {
                if (_pointerEntered) GoToState(PointerOverStateName);
                else if (_pressed) GoToState(PressedStateName);
                else GoToState(NormalStateName);
            }
        }

        private void GoToState(string name)
        {
            VisualStateManager.GoToState(this, name, true);
        }
    }
}
