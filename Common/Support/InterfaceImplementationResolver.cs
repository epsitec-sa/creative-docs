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


using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>InterfaceImplementationResolver</c> class can create instances of classes
    /// implementing the <typeparamref name="TInterface"/> interface (or base class).
    /// </summary>
    /// <typeparam name="TInterface">The interface type or of the base class.</typeparam>
    public static class InterfaceImplementationResolver<TInterface>
        where TInterface : class
    {
        public static TInterface CreateInstance<T>(T constructorArgument)
        {
            return InterfaceImplementationResolver<TInterface>
                .CreateInstances(constructorArgument)
                .FirstOrDefault();
        }

        /// <summary>
        /// Creates one instance for every class which implements <typeparamref name="TInterface"/>.
        /// </summary>
        /// <returns>
        /// The collection of classes implementing <typeparamref name="TInterface"/>.
        /// </returns>
        public static IEnumerable<TInterface> CreateInstances()
        {
            return InterfaceImplementationResolver<TInterface>.CreateInstances(
                InterfaceImplementationResolver<TInterface>.noTypes,
                InterfaceImplementationResolver<TInterface>.noArguments
            );
        }

        public static IEnumerable<TInterface> CreateInstances<T>(T constructorArgument)
        {
            return InterfaceImplementationResolver<TInterface>.CreateInstances(
                new System.Type[] { typeof(T) },
                new object[] { constructorArgument }
            );
        }

        public static IEnumerable<TInterface> CreateInstances<T1, T2>(
            T1 constructorArgument1,
            T2 constructorArgument2
        )
        {
            var types = new System.Type[] { typeof(T1), typeof(T2) };
            var args = new object[] { constructorArgument1, constructorArgument2 };

            return InterfaceImplementationResolver<TInterface>.CreateInstances(types, args);
        }

        /// <summary>
        /// Creates one instance for every class which implements <typeparamref name="TInterface"/>.
        /// </summary>
        /// <param name="constructorArgumentTypes">The constructor argument types.</param>
        /// <param name="constructorArguments">The constructor arguments.</param>
        /// <returns>
        /// The collection of classes implementing <typeparamref name="TInterface"/>.
        /// </returns>
        public static IEnumerable<TInterface> CreateInstances(
            System.Type[] constructorArgumentTypes,
            object[] constructorArguments
        )
        {
            return InterfaceImplementationResolver<TInterface>
                .FindSystemTypes(constructorArgumentTypes)
                .Select(type =>
                    System.Activator.CreateInstance(type, constructorArguments) as TInterface
                );
        }

        private static IEnumerable<System.Type> FindSystemTypes(
            System.Type[] constructorArgumentTypes
        )
        {
            //	Note: it is about 10x faster to check if a type contains an interface than to
            //	retrieve its constructor for a given set of arguments.

            var types =
                from type in TypeEnumerator.Instance.GetAllClassTypes()
                where
                    type.IsAbstract == false
                    && type.ContainsInterface<TInterface>()
                    && type.GetConstructor(constructorArgumentTypes) != null
                select type;

            return types;
        }

        private static readonly System.Type[] noTypes = new System.Type[] { };
        private static readonly object[] noArguments = new object[] { };
    }
}
