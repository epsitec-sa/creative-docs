//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStructuredTypeProviderId</c> interface is used to resolve an id
	/// into a <see cref="StructuredType"/> instance.
	/// </summary>
	public interface IStructuredTypeProviderId
	{
		/// <summary>
		/// Gets the structured type associated with the specified caption id.
		/// </summary>
		/// <param name="id">The caption id.</param>
		/// <returns>The structured type.</returns>
		StructuredType GetStructuredType(Druid id);
	}
}
