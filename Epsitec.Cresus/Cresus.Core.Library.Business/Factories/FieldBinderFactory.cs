//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Binders;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>FieldBinderFactory</c> class creates (or finds) a field binder for
	/// a specified <see cref="INamedType"/> instance.
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
			if (namedType == null)
			{
				return null;
			}

			if (FieldBinderFactory.binders == null)
			{
				FieldBinderFactory.binders = new Dictionary<INamedType, IFieldBinder> ();
			}

			IFieldBinder result;

			if (FieldBinderFactory.binders.TryGetValue (namedType, out result))
			{
				return result;
			}

			var providers  = InterfaceImplementationResolver<IFieldBinderProvider>.GetInstances ();
			var binders    = from provider in providers
							 let binder = provider.GetFieldBinder (namedType)
							 where binder != null
							 select binder;

			result = binders.FirstOrDefault ();

			FieldBinderFactory.binders[namedType] = result;

			return result;
		}

		/// <summary>
		/// Gets a validator returning an <see cref="IValidationResult"> based on the UI text
		/// for the specified field binder.
		/// </summary>
		/// <param name="fieldValidator">The field binder.</param>
		/// <returns>The validator returning an <see cref="IValidationResult"/>.</returns>
		public static System.Func<string, IValidationResult> GetValidator(this IFieldBinder fieldValidator)
		{
			return text => fieldValidator.ValidateFromUI (text);
		}


		[System.ThreadStatic]
		private static Dictionary<INamedType, IFieldBinder> binders;
	}
}
