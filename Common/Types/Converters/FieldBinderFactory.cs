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


using Epsitec.Common.Support;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
    /// <summary>
    /// The <c>FieldBinderFactory</c> class creates (or finds) a field binder for
    /// a specified <see cref="INamedType"/> instance. The type must specify a default
    /// controller named <c>FieldBinder</c> and a non-empty controller parameter.
    /// </summary>
    public static class FieldBinderFactory
    {
        /// <summary>
        /// Creates a field binder for the specified named type.
        /// </summary>
        /// <param name="namedType">The named type.</param>
        /// <returns>A matching field binder, if one can be found; otherwise, <c>null</c>.</returns>
        public static IFieldBinder Create(INamedType namedType)
        {
            if (
                (namedType == null)
                || (namedType.DefaultController != "FieldBinder")
                || (string.IsNullOrEmpty(namedType.DefaultControllerParameters))
            )
            {
                return null;
            }

            if (FieldBinderFactory.binders == null)
            {
                FieldBinderFactory.binders = new Dictionary<INamedType, IFieldBinder>();
            }

            IFieldBinder result;

            if (FieldBinderFactory.binders.TryGetValue(namedType, out result))
            {
                return result;
            }

            if (FieldBinderFactory.providers == null)
            {
                FieldBinderFactory.providers = new List<IFieldBinderProvider>(
                    InterfaceImplementationResolver<IFieldBinderProvider>.CreateInstances()
                );
            }

            var binders =
                from provider in FieldBinderFactory.providers
                let binder = provider.GetFieldBinder(namedType)
                where binder != null
                select binder;

            result = binders.FirstOrDefault();

            FieldBinderFactory.binders[namedType] = result;

            return result;
        }

        /// <summary>
        /// Gets a validator returning an <see cref="IValidationResult"> based on the UI text
        /// for the specified field binder.
        /// </summary>
        /// <param name="fieldValidator">The field binder.</param>
        /// <returns>The validator returning an <see cref="IValidationResult"/>.</returns>
        public static System.Func<string, IValidationResult> GetValidator(
            this IFieldBinder fieldValidator
        )
        {
            return text => fieldValidator.ValidateFromUI(text);
        }

        [System.ThreadStatic]
        private static Dictionary<INamedType, IFieldBinder> binders;

        [System.ThreadStatic]
        private static List<IFieldBinderProvider> providers;
    }
}
