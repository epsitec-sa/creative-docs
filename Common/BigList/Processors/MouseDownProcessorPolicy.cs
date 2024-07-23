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

namespace Epsitec.Common.BigList.Processors
{
    /// <summary>
    /// The <c>MouseDownProcessorPolicy</c> enumeration defines how the <see cref="MouseDownProcessor"/>
    /// behaves.
    /// </summary>
    public class MouseDownProcessorPolicy : EventProcessorPolicy
    {
        public MouseDownProcessorPolicy()
        {
            this.AutoFollow = true;
            this.AutoScroll = true;

            this.AutoScrollDelay = SystemInformation.InitialKeyboardDelay;
            this.AutoScrollRepeat = SystemInformation.KeyboardRepeatPeriod;

            this.SelectionPolicy = SelectionPolicy.OnMouseDown;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selection should automatically follow
        /// the mouse while the user is dragging.
        /// </summary>
        /// <value>
        ///   <c>true</c> to follow the mouse while dragging; otherwise, <c>false</c>.
        /// </value>
        public bool AutoFollow { get; set; }

        public bool AutoScroll { get; set; }

        public double AutoScrollDelay { get; set; }

        public double AutoScrollRepeat { get; set; }

        public SelectionPolicy SelectionPolicy { get; set; }
    }
}
