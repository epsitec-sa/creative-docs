//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStructuredTypeProvider</c> interface gives access to the
	/// <see cref="T:IStructuredType"/> interface.
	/// </summary>
	public interface IStructuredTypeProvider
	{
		/// <summary>
		/// Gets the structured type associated with this instance.
		/// </summary>
		/// <returns>The structured type.</returns>
		IStructuredType GetStructuredType();
	}
}
