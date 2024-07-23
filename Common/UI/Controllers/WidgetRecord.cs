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


using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI.Controllers
{
    /// <summary>
    /// The <c>WidgetRecord</c> structure associates a <see cref="Widget"/> with
    /// a <see cref="WidgetType"/> description.
    /// </summary>
    public struct WidgetRecord : System.IEquatable<WidgetRecord>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetRecord"/> struct.
        /// </summary>
        /// <param name="widget">The widget.</param>
        /// <param name="widgetType">Type of the widget.</param>
        public WidgetRecord(Widget widget, WidgetType widgetType)
        {
            this.widget = widget;
            this.widgetType = widgetType;
        }

        /// <summary>
        /// Gets the widget.
        /// </summary>
        /// <value>The widget.</value>
        public Widget Widget
        {
            get { return this.widget; }
        }

        /// <summary>
        /// Gets the type of the widget.
        /// </summary>
        /// <value>The type of the widget.</value>
        public WidgetType WidgetType
        {
            get { return this.widgetType; }
        }

        #region IEquatable<WidgetRecord> Members

        public bool Equals(WidgetRecord other)
        {
            return this.widgetType == other.WidgetType && this.widget == other.widget;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is WidgetRecord)
            {
                return this.Equals((WidgetRecord)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)this.widgetType ^ (this.widget == null ? 0 : this.widget.GetHashCode());
        }

        public override string ToString()
        {
            return string.Concat(
                this.widgetType.ToString(),
                ", ",
                this.widget == null ? "<null>" : this.widget.ToString()
            );
        }

        private Widget widget;
        private WidgetType widgetType;
    }
}
