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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>IValidator</c> interface is used to find out if a widget contains
    /// valid data or not.
    /// </summary>
    public interface IValidator : IValidationResult
    {
        /// <summary>
        /// Validates the associated data and updates the <c>State</c>.
        /// </summary>
        void Validate();

        /// <summary>
        /// Marks the validator as dirty, which means that the <c>State</c> will have
        /// to be revalidated.
        /// </summary>
        /// <param name="deep">If set to <c>true</c>, mark all children validators as dirty too.</param>
        void MakeDirty(bool deep);

        /// <summary>
        /// Occurs when the validator state became dirty, i.e. <c>State</c> changed to
        /// <c>ValidationState.Dirty</c>.
        /// </summary>
        event Support.EventHandler BecameDirty;
    }
}
