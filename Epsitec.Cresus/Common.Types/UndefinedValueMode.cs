//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>UndefinedValueMode</c> enumeration defines what value should
	/// be used by <see cref="StructuredData.GetValue"/> when an undefined
	/// value is found.
	/// </summary>
	public enum UndefinedValueMode
	{
		/// <summary>
		/// Return <see cref="UndefinedValue.Instance"/>.
		/// </summary>
		Undefined,
		
		/// <summary>
		/// Return the default value.
		/// </summary>
		Default,

		/// <summary>
		/// Return the sample value.
		/// </summary>
		Sample
	}
}
