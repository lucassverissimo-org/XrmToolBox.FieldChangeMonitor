using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Models;

namespace LucasVerissimo.XrmToolBox.DataverseUsageExplorer.Controls
{
    internal sealed class SearchableMetadataComboBox : ComboBox
    {
        private List<MetadataListItem> source = new List<MetadataListItem>();
        private bool updating;

        [StructLayout(LayoutKind.Sequential)]
        private struct CursorInfo
        {
            public int Size;
            public int Flags;
            public IntPtr CursorHandle;
            public Point ScreenPosition;
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(ref CursorInfo cursorInfo);

        private const int CursorShowing = 1;

        public SearchableMetadataComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDown;
            AutoCompleteMode = AutoCompleteMode.None;
            IntegralHeight = false;
            DropDownHeight = 320;
            MaxDropDownItems = 20;
        }

        public void SetItems(IEnumerable<MetadataListItem> items)
        {
            source = (items ?? Enumerable.Empty<MetadataListItem>()).ToList();
            updating = true;
            Items.Clear();
            Items.AddRange(source.Cast<object>().ToArray());
            Text = string.Empty;
            SelectedIndex = -1;
            updating = false;
        }

        public void ClearItems()
        {
            SetItems(null);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);
            if (updating)
                return;

            var query = Text.Trim();
            var caret = SelectionStart;
            var matches = query.Length == 0 ? source : source.Where(x => Matches(x, query));

            updating = true;
            BeginUpdate();
            Items.Clear();
            Items.AddRange(matches.Cast<object>().ToArray());
            Text = query;
            SelectionStart = Math.Min(caret, Text.Length);
            SelectionLength = 0;
            EndUpdate();
            updating = false;

            if (Items.Count > 0 && !DroppedDown)
            {
                DroppedDown = true;
                SelectionStart = Text.Length;
            }
            BeginInvoke(new Action(EnsureMousePointerVisible));
        }

        protected override void OnDropDown(EventArgs e)
        {
            if (
                !updating
                && SelectedItem == null
                && Text.Trim().Length == 0
                && Items.Count != source.Count
            )
            {
                updating = true;
                BeginUpdate();
                Items.Clear();
                Items.AddRange(source.Cast<object>().ToArray());
                EndUpdate();
                updating = false;
            }
            base.OnDropDown(e);
        }

        private static void EnsureMousePointerVisible()
        {
            var info = new CursorInfo { Size = Marshal.SizeOf(typeof(CursorInfo)) };
            if (GetCursorInfo(ref info) && (info.Flags & CursorShowing) == 0)
                Cursor.Show();
        }

        private static bool Matches(MetadataListItem item, string query)
        {
            return Contains(item.DisplayName, query) || Contains(item.LogicalName, query);
        }

        private static bool Contains(string value, string query)
        {
            return !string.IsNullOrWhiteSpace(value)
                && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
