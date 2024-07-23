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

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// This class is used to normalize expressions, so that two similar expression will map to the
    /// same normalized form. If we have (Person p) => p.Address and (Person x) => x.Address, these
    /// expressions are logically the same, even if their text is different. As we use this text
    /// to check if two expressions are the same, we must have a way to normalize them before.
    /// </summary>
    /// <remarks>
    /// For now, this class only normalizes the parameter names. If we wanted more advanced
    /// normalization, we could get insipration from this article:
    /// http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries
    /// Interrestingly, this article does not normalizes the parameter names.
    /// </remarks>
    public sealed class ExpressionNormalizer : ExpressionVisitor
    {
        private ExpressionNormalizer()
        {
            this.mapping = new Dictionary<string, string>();
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var newName = this.GetParameterName(node);

            return Expression.Parameter(node.Type, newName);
        }

        private string GetParameterName(ParameterExpression node)
        {
            var oldName = node.Name;

            string newName;

            if (!this.mapping.TryGetValue(oldName, out newName))
            {
                newName = "x" + mapping.Count;

                this.mapping[oldName] = newName;
            }

            return newName;
        }

        /// <summary>
        /// This method takes an expression and normalizes it so that it would become similar to
        /// another one with the same structure.
        /// </summary>
        public static Expression Normalize(Expression expression)
        {
            var normalizer = new ExpressionNormalizer();

            return normalizer.Visit(expression);
        }

        private readonly Dictionary<string, string> mapping;
    }
}
