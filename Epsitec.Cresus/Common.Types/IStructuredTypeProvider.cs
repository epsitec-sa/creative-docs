//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
