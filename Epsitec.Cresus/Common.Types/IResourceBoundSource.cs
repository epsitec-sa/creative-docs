//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IResourceBoundSource</c> is use to retrieve resources efficiently
	/// through the data binding mechanisms.
	/// </summary>
	public interface IResourceBoundSource
	{
		/// <summary>
		/// Gets the resource with the specified identifier.
		/// </summary>
		/// <param name="id">The resource identifier.</param>
		/// <returns>The value stored in the resource.</returns>
		object GetValue(string id);
	}
}
