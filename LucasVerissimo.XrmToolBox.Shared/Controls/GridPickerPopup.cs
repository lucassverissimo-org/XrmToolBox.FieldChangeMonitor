using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.Shared.Controls
{
    internal partial class GridPickerPopup : UserControl
    {
        private readonly List<object> items = new List<object>();
        private GridPickerConfiguration configuration = new GridPickerConfiguration();
        private int sortedColumnIndex = -1;
        private SortOrder sortOrder = SortOrder.None;

        public GridPickerPopup()
        {
            InitializeComponent();
        }

        public event EventHandler ItemAccepted;

        public object SelectedItem
        {
            get { return itemsGrid.CurrentRow?.Tag; }
        }

        public void Configure(GridPickerConfiguration pickerConfiguration)
        {
            configuration =
                pickerConfiguration ?? throw new ArgumentNullException(nameof(pickerConfiguration));
            searchLayout.Visible = configuration.SearchEnabled;
            rootLayout.RowStyles[0].Height = configuration.SearchEnabled ? 36F : 0F;
            BuildColumns();
        }

        public void SetItems(IEnumerable<object> availableItems)
        {
            if (availableItems == null)
            {
                throw new ArgumentNullException(nameof(availableItems));
            }

            items.Clear();
            items.AddRange(availableItems.Where(item => item != null));
            sortedColumnIndex = -1;
            sortOrder = SortOrder.None;
            searchTextBox.Clear();
            filterTimer.Stop();
            ApplyFilter();
        }

        public void SelectItem(object item)
        {
            itemsGrid.ClearSelection();
            if (item == null)
            {
                return;
            }

            var identity = configuration.IdentitySelector(item);
            foreach (DataGridViewRow row in itemsGrid.Rows)
            {
                if (Equals(configuration.IdentitySelector(row.Tag), identity))
                {
                    row.Selected = true;
                    itemsGrid.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        public void SetSearchText(string searchText)
        {
            if (!configuration.SearchEnabled)
            {
                return;
            }

            var value = searchText ?? string.Empty;
            if (!string.Equals(searchTextBox.Text, value, StringComparison.Ordinal))
            {
                searchTextBox.Text = value;
            }
        }

        public void FocusSearch()
        {
            if (!configuration.SearchEnabled)
            {
                return;
            }

            BeginInvoke(
                new Action(() =>
                {
                    searchTextBox.Focus();
                    searchTextBox.SelectAll();
                })
            );
        }

        private void BuildColumns()
        {
            itemsGrid.Columns.Clear();
            foreach (var definition in configuration.Columns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    AutoSizeMode = definition.AutoSizeMode,
                    FillWeight = definition.FillWeight,
                    HeaderText = definition.HeaderText,
                    ReadOnly = true,
                    SortMode =
                        configuration.SortingEnabled && definition.Sortable
                            ? DataGridViewColumnSortMode.Programmatic
                            : DataGridViewColumnSortMode.NotSortable,
                    Width = definition.Width > 0 ? definition.Width : 100,
                };
                itemsGrid.Columns.Add(column);
            }
        }

        private void SearchTextBoxTextChanged(object sender, EventArgs eventArguments)
        {
            filterTimer.Stop();
            filterTimer.Start();
        }

        private void FilterTimerTick(object sender, EventArgs eventArguments)
        {
            filterTimer.Stop();
            ApplyFilter();
        }

        private void SearchTextBoxKeyDown(object sender, KeyEventArgs eventArguments)
        {
            if (eventArguments.KeyCode != Keys.Down || itemsGrid.Rows.Count == 0)
            {
                return;
            }

            eventArguments.Handled = true;
            eventArguments.SuppressKeyPress = true;
            itemsGrid.Focus();
            itemsGrid.CurrentCell = itemsGrid.Rows[0].Cells[0];
            itemsGrid.Rows[0].Selected = true;
        }

        private void ItemsGridCellDoubleClick(
            object sender,
            DataGridViewCellEventArgs eventArguments
        )
        {
            if (eventArguments.RowIndex >= 0)
            {
                AcceptSelection();
            }
        }

        private void ItemsGridColumnHeaderMouseClick(
            object sender,
            DataGridViewCellMouseEventArgs eventArguments
        )
        {
            if (
                !configuration.SortingEnabled
                || eventArguments.ColumnIndex < 0
                || !configuration.Columns.ElementAt(eventArguments.ColumnIndex).Sortable
            )
            {
                return;
            }

            if (sortedColumnIndex == eventArguments.ColumnIndex)
            {
                sortOrder =
                    sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                sortedColumnIndex = eventArguments.ColumnIndex;
                sortOrder = SortOrder.Ascending;
            }

            ApplyFilter();
        }

        private void ItemsGridKeyDown(object sender, KeyEventArgs eventArguments)
        {
            if (eventArguments.KeyCode != Keys.Enter)
            {
                return;
            }

            eventArguments.Handled = true;
            eventArguments.SuppressKeyPress = true;
            AcceptSelection();
        }

        private void SelectButtonClick(object sender, EventArgs eventArguments)
        {
            AcceptSelection();
        }

        private void ApplyFilter()
        {
            var search = configuration.SearchEnabled ? searchTextBox.Text.Trim() : string.Empty;
            IEnumerable<object> filteredItems = items.Where(item => MatchesSearch(item, search));

            if (sortedColumnIndex >= 0)
            {
                var column = configuration.Columns.ElementAt(sortedColumnIndex);
                filteredItems =
                    sortOrder == SortOrder.Descending
                        ? filteredItems.OrderByDescending(
                            column.ValueSelector,
                            PickerValueComparer.Instance
                        )
                        : filteredItems.OrderBy(column.ValueSelector, PickerValueComparer.Instance);
            }

            PopulateGrid(filteredItems.ToList());
        }

        private bool MatchesSearch(object item, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return true;
            }

            if (configuration.SearchPredicate != null)
            {
                return configuration.SearchPredicate(item, search);
            }

            return configuration.Columns.Any(column =>
                Contains(
                    Convert.ToString(column.ValueSelector(item), CultureInfo.CurrentCulture),
                    search
                )
            );
        }

        private void PopulateGrid(IReadOnlyCollection<object> filteredItems)
        {
            itemsGrid.Rows.Clear();
            foreach (var item in filteredItems)
            {
                var values = configuration
                    .Columns.Select(column => column.ValueSelector(item))
                    .ToArray();
                var rowIndex = itemsGrid.Rows.Add(values);
                itemsGrid.Rows[rowIndex].Tag = item;
            }

            foreach (DataGridViewColumn column in itemsGrid.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (sortedColumnIndex >= 0)
            {
                itemsGrid.Columns[sortedColumnIndex].HeaderCell.SortGlyphDirection = sortOrder;
            }

            resultCountLabel.Text =
                filteredItems.Count + " of " + items.Count + " " + configuration.ItemName;
            selectButton.Enabled = filteredItems.Count > 0;

            if (itemsGrid.Rows.Count > 0)
            {
                itemsGrid.Rows[0].Selected = true;
            }
        }

        private void AcceptSelection()
        {
            if (SelectedItem != null)
            {
                ItemAccepted?.Invoke(this, EventArgs.Empty);
            }
        }

        private static bool Contains(string value, string search)
        {
            return value?.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private sealed class PickerValueComparer : IComparer<object>
        {
            public static readonly PickerValueComparer Instance = new PickerValueComparer();

            public int Compare(object first, object second)
            {
                if (ReferenceEquals(first, second))
                {
                    return 0;
                }

                if (first == null)
                {
                    return -1;
                }

                if (second == null)
                {
                    return 1;
                }

                if (first is IComparable comparable && first.GetType() == second.GetType())
                {
                    return comparable.CompareTo(second);
                }

                return StringComparer.CurrentCultureIgnoreCase.Compare(
                    Convert.ToString(first, CultureInfo.CurrentCulture),
                    Convert.ToString(second, CultureInfo.CurrentCulture)
                );
            }
        }
    }
}
