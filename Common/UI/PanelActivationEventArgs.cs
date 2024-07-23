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

[assembly: DependencyClass(typeof(Panel))]

namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>Panel</c> class is used as the (local) root in a widget tree
    /// built by the dynamic user interface designer.
    /// </summary>
    public class PanelActivationEventArgs : Support.CancelEventArgs
    {
        public PanelActivationEventArgs(Panel panel, PanelStack stack, string focusWidgetName)
        {
            this.panel = panel;
            this.stack = stack;
            this.focusWidgetName = focusWidgetName;
        }

        public Panel Panel
        {
            get { return this.panel; }
        }

        public PanelStack PanelStack
        {
            get { return this.stack; }
        }

        public string FocusWidgetName
        {
            get { return this.focusWidgetName; }
        }

        private Panel panel;
        private PanelStack stack;
        private string focusWidgetName;
    }
}
