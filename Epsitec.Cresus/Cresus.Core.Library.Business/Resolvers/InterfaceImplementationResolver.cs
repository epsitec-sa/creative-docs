//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

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
		/// <summary>
		/// Gets one instance for every class which implements <typeparamref name="TInterface"/>.
		/// The instances are only created once for every thread; it would be best if they were
		/// immutable.
		/// </summary>
		/// <returns>The collection of classes implementing <typeparamref name="TInterface"/>.</returns>
		public static IEnumerable<TInterface> GetInstances()
		{
			if (InterfaceImplementationResolver<TInterface>.instances == null)
			{
				InterfaceImplementationResolver<TInterface>.instances = new List<TInterface> (InterfaceImplementationResolver<TInterface>.CreateInstances ());
			}

			return InterfaceImplementationResolver<TInterface>.instances;
		}

		private static IEnumerable<System.Type> FindSystemTypes()
		{
			var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies ()
						from type in assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract && type.GetConstructor (noTypes) != null && type.GetInterfaces ().Any (x => x == typeof (TInterface))
						select type;

			return types;
		}

		private static IEnumerable<TInterface> CreateInstances()
		{
			var types = InterfaceImplementationResolver<TInterface>.FindSystemTypes ();

			return from type in types
				   select System.Activator.CreateInstance (type, InterfaceImplementationResolver<TInterface>.noArguments) as TInterface;
		}


		[System.ThreadStatic]
		private static List<TInterface> instances;
		private static readonly System.Type[] noTypes = new System.Type[] { };
		private static readonly object[] noArguments = new object[] { };
	}
}
