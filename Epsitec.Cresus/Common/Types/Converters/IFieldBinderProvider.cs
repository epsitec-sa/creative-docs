//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>IFieldBinderProvider</c> interface tries to find a <see cref="IFieldBinder"/> for
	/// the specified <see cref="INamedType"/>.
	/// </summary>
	public interface IFieldBinderProvider
	{
		/// <summary>
		/// Gets the field binder for the specified named type.
		/// </summary>
		/// <param name="namedType">The named type.</param>
		/// <returns>The field binder if one can be found; otherwise, <c>null</c>.</returns>
		IFieldBinder GetFieldBinder(INamedType namedType);
	}
}
