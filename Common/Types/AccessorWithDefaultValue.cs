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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>AccessorWithDefaultValue</c> is a specialized <see cref="Accessor"/> which
    /// returns a default result when the value is empty.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class AccessorWithDefaultValue<TResult> : Accessor<TResult>
    {
        public AccessorWithDefaultValue(
            Accessor<TResult> accessor,
            TResult defaultResult,
            System.Predicate<TResult> isEmptyPredicate
        )
            : base(accessor.Getter)
        {
            this.defaultResult = defaultResult;
            this.isEmptyPredicate = isEmptyPredicate;
        }

        public override TResult ExecuteGetter()
        {
            TResult result = base.ExecuteGetter();

            if (this.isEmptyPredicate(result))
            {
                return this.defaultResult;
            }
            else
            {
                return result;
            }
        }

        private readonly TResult defaultResult;
        private readonly System.Predicate<TResult> isEmptyPredicate;
    }
}
