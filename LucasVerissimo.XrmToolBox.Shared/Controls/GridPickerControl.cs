using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.Shared.Controls
{
    public partial class GridPickerControl : UserControl
    {
        private readonly GridPickerPopup popup;
        private readonly ToolStripDropDown dropDown;
        private readonly ToolStripControlHost popupHost;
        private GridPickerConfiguration configuration;
        private object selectedItem;
        private bool updatingValueText;

        public GridPickerControl()
        {
            InitializeComponent();

            configuration = new GridPickerConfiguration();
            popup = new GridPickerPopup();
            popup.ItemAccepted += PopupItemAccepted;

            popupHost = new ToolStripControlHost(popup)
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };

            dropDown = new ToolStripDropDown
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = new Padding(1),
            };
            dropDown.Items.Add(popupHost);
        }

        public event EventHandler SelectedItemChanged;

        public object SelectedItem
        {
            get { return selectedItem; }
        }

        public void Configure(GridPickerConfiguration pickerConfiguration)
        {
            if (pickerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(pickerConfiguration));
            }

            pickerConfiguration.Validate();
            configuration = pickerConfiguration;
            valueTextBox.ReadOnly = !configuration.SearchEnabled;
            popup.Configure(configuration);
            ClearItems();
        }

        public T GetSelectedItem<T>()
            where T : class
        {
            return selectedItem as T;
        }

        public void SetItems<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var availableItems = items.Where(item => item != null).Cast<object>().ToList();
            popup.SetItems(availableItems);

            if (selectedItem != null && !ContainsSelectedItem(availableItems))
            {
                SetSelectedItem(null);
            }

            if (selectedItem != null)
            {
                UpdateValueText(configuration.DisplayTextSelector(selectedItem));
                return;
            }

            UpdateValueText(
                availableItems.Count == 0
                    ? "No " + configuration.ItemName + " loaded"
                    : "Select from " + configuration.ItemName + "..."
            );
        }

        public void ClearItems()
        {
            popup.SetItems(Array.Empty<object>());
            SetSelectedItem(null);
            UpdateValueText("Load " + configuration.ItemName + " to begin");
        }

        protected override void OnEnabledChanged(EventArgs eventArguments)
        {
            base.OnEnabledChanged(eventArguments);
            valueTextBox.BackColor = Enabled ? SystemColors.Window : SystemColors.Control;
        }

        private bool ContainsSelectedItem(IEnumerable<object> availableItems)
        {
            var selectedIdentity = configuration.IdentitySelector(selectedItem);
            return availableItems.Any(item =>
                Equals(configuration.IdentitySelector(item), selectedIdentity)
            );
        }

        private void OpenButtonClick(object sender, EventArgs eventArguments)
        {
            ShowItemList();
        }

        private void ValueTextBoxClick(object sender, EventArgs eventArguments)
        {
            ShowItemList();
        }

        private void ValueTextBoxTextChanged(object sender, EventArgs eventArguments)
        {
            if (updatingValueText || !configuration.SearchEnabled)
            {
                return;
            }

            if (selectedItem != null)
            {
                selectedItem = null;
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            }

            popup.SetSearchText(valueTextBox.Text);
            ShowItemList(false);
        }

        private void ValueTextBoxKeyDown(object sender, KeyEventArgs eventArguments)
        {
            if (
                eventArguments.KeyCode != Keys.Enter
                && (!eventArguments.Alt || eventArguments.KeyCode != Keys.Down)
            )
            {
                return;
            }

            eventArguments.Handled = true;
            eventArguments.SuppressKeyPress = true;
            ShowItemList();
        }

        private void ShowItemList(bool focusSearch = true)
        {
            if (!Enabled || dropDown.Visible)
            {
                return;
            }

            var popupSize = new Size(Math.Max(680, Width), 340);
            popup.Size = popupSize;
            popupHost.Size = popupSize;
            dropDown.Size = new Size(popupSize.Width + 2, popupSize.Height + 2);
            popup.SelectItem(selectedItem);
            dropDown.Show(this, new Point(0, Height));

            if (focusSearch && configuration.SearchEnabled)
            {
                popup.FocusSearch();
            }
            else
            {
                valueTextBox.Focus();
            }
        }

        private void PopupItemAccepted(object sender, EventArgs eventArguments)
        {
            if (popup.SelectedItem == null)
            {
                return;
            }

            SetSelectedItem(popup.SelectedItem);
            dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
        }

        private void SetSelectedItem(object item)
        {
            if (SameIdentity(selectedItem, item))
            {
                return;
            }

            selectedItem = item;
            UpdateValueText(
                item == null
                    ? "Select from " + configuration.ItemName + "..."
                    : configuration.DisplayTextSelector(item)
            );
            valueTextBox.SelectionStart = 0;
            valueTextBox.SelectionLength = 0;
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool SameIdentity(object first, object second)
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            return Equals(
                configuration.IdentitySelector(first),
                configuration.IdentitySelector(second)
            );
        }

        private void UpdateValueText(string text)
        {
            updatingValueText = true;
            try
            {
                valueTextBox.Text = text;
            }
            finally
            {
                updatingValueText = false;
            }
        }
    }
}
