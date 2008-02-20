//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStructuredTypeResolver</c> interface is used to resolve an id
	/// into a <see cref="StructuredType"/> instance.
	/// </summary>
	public interface IStructuredTypeResolver
	{
		/// <summary>
		/// Gets the structured type for the specified id.
		/// </summary>
		/// <param name="id">The id for the structured type.</param>
		/// <returns>The structured type or <c>null</c>.</returns>
		StructuredType GetStructuredType(Druid id);
	}
}
