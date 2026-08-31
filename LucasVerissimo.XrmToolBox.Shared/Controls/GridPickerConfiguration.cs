using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LucasVerissimo.XrmToolBox.Shared.Controls
{
    public sealed class GridPickerConfiguration
    {
        public GridPickerConfiguration()
        {
            Columns = Array.Empty<GridPickerColumnDefinition>();
            DisplayTextSelector = item => item?.ToString() ?? string.Empty;
            IdentitySelector = item => item;
            ItemName = "items";
            SearchEnabled = true;
            SortingEnabled = true;
        }

        public IReadOnlyCollection<GridPickerColumnDefinition> Columns { get; set; }

        public Func<object, string> DisplayTextSelector { get; set; }

        public Func<object, object> IdentitySelector { get; set; }

        public string ItemName { get; set; }

        public Func<object, string, bool> SearchPredicate { get; set; }

        public bool SearchEnabled { get; set; }

        public bool SortingEnabled { get; set; }

        internal void Validate()
        {
            if (Columns == null || Columns.Count == 0)
            {
                throw new InvalidOperationException("At least one picker column is required.");
            }

            if (DisplayTextSelector == null)
            {
                throw new InvalidOperationException("A display text selector is required.");
            }

            if (IdentitySelector == null)
            {
                throw new InvalidOperationException("An identity selector is required.");
            }

            if (string.IsNullOrWhiteSpace(ItemName))
            {
                throw new InvalidOperationException("An item name is required.");
            }

            foreach (var column in Columns)
            {
                column.Validate();
            }
        }
    }

    public sealed class GridPickerColumnDefinition
    {
        public GridPickerColumnDefinition(string headerText, Func<object, object> valueSelector)
        {
            HeaderText = headerText;
            ValueSelector = valueSelector;
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            FillWeight = 100F;
            Sortable = true;
        }

        public DataGridViewAutoSizeColumnMode AutoSizeMode { get; set; }

        public float FillWeight { get; set; }

        public string HeaderText { get; }

        public bool Sortable { get; set; }

        public Func<object, object> ValueSelector { get; }

        public int Width { get; set; }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(HeaderText))
            {
                throw new InvalidOperationException("A picker column header is required.");
            }

            if (ValueSelector == null)
            {
                throw new InvalidOperationException(
                    "A value selector is required for column " + HeaderText + "."
                );
            }
        }
    }
}
