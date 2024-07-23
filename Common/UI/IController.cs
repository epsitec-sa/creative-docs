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


namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>IController</c> interface is used by the <see cref="Placeholder"/>
    /// class to communicate with its controller.
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// Creates the user interface. This populates the <c>Placeholder</c>
        /// with one or several widgets used to represent/edit its value.
        /// </summary>
        void CreateUserInterface();

        /// <summary>
        /// Disposes the user interface.
        /// </summary>
        void DisposeUserInterface();

        /// <summary>
        /// Refreshes the user interface. This method is called whenever the value
        /// represented in the <c>Placeholder</c> changes.
        /// </summary>
        void RefreshUserInterface(object oldValue, object newValue);

        /// <summary>
        /// Gets an object implementing interface <see cref="T:Layouts.IGridPermeable"/>
        /// for this controller.
        /// </summary>
        /// <returns>Object implementing interface <see cref="T:Layouts.IGridPermeable"/>
        /// for this controller, or <c>null</c>.</returns>
        Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper();

        /// <summary>
        /// Gets the placeholder associated with this controller.
        /// </summary>
        /// <value>The placeholder.</value>
        Placeholder Placeholder { get; }

        /// <summary>
        /// Defines the placeholder associated with this controller.
        /// </summary>
        /// <param name="placeholder">The placeholder.</param>
        void DefinePlaceholder(Placeholder placeholder);
    }
}
