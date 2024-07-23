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


using System.Linq.Expressions;

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>AccessorBinding</c> class encapsulates a property setter for a given
    /// property. Accessor bindings are considered to be equal if they apply to the
    /// same property (based on its name).
    /// </summary>
    public sealed class AccessorBinding : System.IEquatable<AccessorBinding>
    {
        private AccessorBinding(string name, System.Action action)
        {
            this.name = name;
            this.action = action;
        }

        public void Execute()
        {
            this.action();
        }

        /// <summary>
        /// Creates an accessor binding for the specified property.
        /// </summary>
        /// <typeparam name="TResult">The return type of the accessor.</typeparam>
        /// <param name="accessor">The accessor (used to provide the source data).</param>
        /// <param name="getterExpression">The property getter expression (used to retrieve the name of the property which will be set).</param>
        /// <param name="setter">The property setter action.</param>
        /// <returns>The accessor binding.</returns>
        public static AccessorBinding Create<TResult>(
            Accessor<TResult> accessor,
            Expression<System.Func<FormattedText>> getterExpression,
            System.Action<TResult> setter
        )
        {
            var propertyInfo = ExpressionAnalyzer.GetLambdaPropertyInfo(getterExpression);

            if (propertyInfo == null)
            {
                throw new System.ArgumentException(
                    string.Format(
                        "The expression {0} does not map to a property getter",
                        getterExpression
                    ),
                    "getterExpression"
                );
            }

            return new AccessorBinding(propertyInfo.Name, () => setter(accessor.ExecuteGetter()));
        }

        #region IEquatable<AccessorBinding> Members

        public bool Equals(AccessorBinding other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return this.name == other.name;
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            return base.Equals(obj as AccessorBinding);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }

        private readonly string name;
        private readonly System.Action action;
    }
}
