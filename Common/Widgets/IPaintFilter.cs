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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>IPaintFilter</c> interface is used to skip widgets completely,
    /// including their children, or just skip the widgets but process their
    /// children, when executing the <c>Widget.PaintHandler</c> method.
    /// </summary>
    public interface IPaintFilter
    {
        /// <summary>
        /// Determines whether the specified widget is fully discarded. A fully
        /// discarded widget won't be processed at all by <c>PaintHandler</c>;
        /// its children won't be processed either.
        /// </summary>
        /// <param name="widget">The widget to check.</param>
        /// <returns><c>true</c> if the specified widget fully discarded;
        /// otherwise, <c>false</c>.</returns>
        bool IsWidgetFullyDiscarded(Widget widget);

        /// <summary>
        /// Determines whether the specified widget should not be painted. Its
        /// children will get a chance to be painted.
        /// </summary>
        /// <param name="widget">The widget to check.</param>
        /// <returns><c>true</c> if the specified widget should not be painted;
        /// otherwise, <c>false</c>.</returns>
        bool IsWidgetPaintDiscarded(Widget widget);

        /// <summary>
        /// This gets called by <c>PaintHandler</c> before a widget's children
        /// get processed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        void NotifyAboutToProcessChildren(Widget sender, PaintEventArgs e);

        /// <summary>
        /// This gets called by <c>PaintHandler</c> after a widget's children
        /// have all been processed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
        void NotifyChildrenProcessed(Widget sender, PaintEventArgs e);
    }
}
