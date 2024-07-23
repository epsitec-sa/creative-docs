/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Types;
using Epsitec.Common.UI;

[assembly: DependencyClass(typeof(ItemTableColumn))]

namespace Epsitec.Common.UI
{
    using GridLength = Epsitec.Common.Widgets.Layouts.GridLength;

    public class ItemTableColumn : DependencyObject
    {
        public ItemTableColumn() { }

        public ItemTableColumn(string fieldId)
            : this()
        {
            this.FieldId = fieldId;
        }

        public ItemTableColumn(string fieldId, double width)
            : this(fieldId, new GridLength(width)) { }

        public ItemTableColumn(DependencyProperty property, double width)
            : this(property.Name, width) { }

        public ItemTableColumn(string fieldId, GridLength width)
            : this(fieldId)
        {
            this.Width = width;
        }

        public ItemTableColumn(DependencyProperty property, GridLength width)
            : this(property.Name, width) { }

        public ItemTableColumn(string fieldId, double width, Support.PropertyComparer comparer)
            : this(fieldId, new GridLength(width))
        {
            this.comparer = comparer;
        }

        public ItemTableColumn(
            DependencyProperty property,
            double width,
            Support.PropertyComparer comparer
        )
            : this(property.Name, width, comparer) { }

        public string FieldId
        {
            get { return (string)this.GetValue(ItemTableColumn.FieldIdProperty); }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.ClearValue(ItemTableColumn.FieldIdProperty);
                }
                else
                {
                    this.SetValue(ItemTableColumn.FieldIdProperty, value);
                }
            }
        }

        public GridLength Width
        {
            get { return (GridLength)this.GetValue(ItemTableColumn.WidthProperty); }
            set
            {
                if (value.IsAuto)
                {
                    this.ClearValue(ItemTableColumn.WidthProperty);
                }
                else
                {
                    this.SetValue(ItemTableColumn.WidthProperty, value);
                }
            }
        }

        public Support.Druid CaptionId
        {
            get { return (Support.Druid)this.GetValue(ItemTableColumn.CaptionIdProperty); }
            set
            {
                if (value.IsEmpty)
                {
                    this.ClearValue(ItemTableColumn.CaptionIdProperty);
                }
                else
                {
                    this.SetValue(ItemTableColumn.CaptionIdProperty, value);
                }
            }
        }

        public Support.PropertyComparer Comparer
        {
            get { return this.comparer; }
            set { this.comparer = value; }
        }

        public Support.Druid TemplateId
        {
            get { return (Support.Druid)this.GetValue(ItemTableColumn.TemplateIdProperty); }
            set
            {
                if (value.IsEmpty)
                {
                    this.ClearValue(ItemTableColumn.TemplateIdProperty);
                }
                else
                {
                    this.SetValue(ItemTableColumn.TemplateIdProperty, value);
                }
            }
        }

        public ListSortDirection SortDirection
        {
            get { return (ListSortDirection)this.GetValue(ItemTableColumn.SortDirectionProperty); }
            set { this.SetValue(ItemTableColumn.SortDirectionProperty, value); }
        }

        public Drawing.ContentAlignment ContentAlignment
        {
            get
            {
                return (Drawing.ContentAlignment)
                    this.GetValue(ItemTableColumn.ContentAlignmentProperty);
            }
            set { this.SetValue(ItemTableColumn.ContentAlignmentProperty, value); }
        }

        public static void SetComparer(DependencyObject obj, Support.PropertyComparer comparer)
        {
            obj.SetValue(ItemTableColumn.ComparerProperty, comparer);
        }

        public static Support.PropertyComparer GetComparer(DependencyObject obj)
        {
            return (Support.PropertyComparer)obj.GetValue(ItemTableColumn.ComparerProperty);
        }

        public static readonly DependencyProperty FieldIdProperty = DependencyProperty.Register(
            "FieldId",
            typeof(string),
            typeof(ItemTableColumn)
        );
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(GridLength),
            typeof(ItemTableColumn),
            new DependencyPropertyMetadata(GridLength.Auto)
        );
        public static readonly DependencyProperty CaptionIdProperty = DependencyProperty.Register(
            "CaptionId",
            typeof(Support.Druid),
            typeof(ItemTableColumn),
            new DependencyPropertyMetadata(Support.Druid.Empty)
        );
        public static readonly DependencyProperty TemplateIdProperty = DependencyProperty.Register(
            "TemplateId",
            typeof(Support.Druid),
            typeof(ItemTableColumn),
            new DependencyPropertyMetadata(Support.Druid.Empty)
        );
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register(
                "SortDirection",
                typeof(ListSortDirection),
                typeof(ItemTableColumn),
                new DependencyPropertyMetadata(ListSortDirection.Descending)
            );
        public static readonly DependencyProperty ContentAlignmentProperty =
            DependencyProperty.Register(
                "ContentAlignment",
                typeof(Drawing.ContentAlignment),
                typeof(ItemTableColumn),
                new DependencyPropertyMetadata(Drawing.ContentAlignment.MiddleLeft)
            );

        public static readonly DependencyProperty ComparerProperty =
            DependencyProperty.RegisterAttached(
                "Comparer",
                typeof(Support.PropertyComparer),
                typeof(ItemTableColumn)
            );

        private Support.PropertyComparer comparer;
    }
}
