//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
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
			return InterfaceImplementationResolver<TInterface>.CreateInstances (constructorArgument).FirstOrDefault ();
		}

		/// <summary>
		/// Creates one instance for every class which implements <typeparamref name="TInterface"/>.
		/// </summary>
		/// <returns>
		/// The collection of classes implementing <typeparamref name="TInterface"/>.
		/// </returns>
		public static IEnumerable<TInterface> CreateInstances()
		{
			return InterfaceImplementationResolver<TInterface>.CreateInstances (InterfaceImplementationResolver<TInterface>.noTypes, InterfaceImplementationResolver<TInterface>.noArguments);
		}

		public static IEnumerable<TInterface> CreateInstances<T>(T constructorArgument)
		{
			return InterfaceImplementationResolver<TInterface>.CreateInstances (new System.Type[] { typeof (T) }, new object[] { constructorArgument });
		}
		
		/// <summary>
		/// Creates one instance for every class which implements <typeparamref name="TInterface"/>.
		/// </summary>
		/// <param name="constructorArgumentTypes">The constructor argument types.</param>
		/// <param name="constructorArguments">The constructor arguments.</param>
		/// <returns>
		/// The collection of classes implementing <typeparamref name="TInterface"/>.
		/// </returns>
		public static IEnumerable<TInterface> CreateInstances(System.Type[] constructorArgumentTypes, object[] constructorArguments)
		{
			var types = InterfaceImplementationResolver<TInterface>.FindSystemTypes (constructorArgumentTypes);

			return from type in types
				   select System.Activator.CreateInstance (type, constructorArguments) as TInterface;
		}

		private static IEnumerable<System.Type> FindSystemTypes(System.Type[] constructorArgumentTypes)
		{
			var types = from type in TypeEnumerator.Instance.GetAllTypes ()
						where type.IsClass
						   && !type.IsAbstract
						   && type.GetConstructor (constructorArgumentTypes) != null
						   && type.ContainsInterface<TInterface> ()
						select type;

			return types;
		}


		private static readonly System.Type[] noTypes = new System.Type[] { };
		private static readonly object[] noArguments = new object[] { };
	}
}
