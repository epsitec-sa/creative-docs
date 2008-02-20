//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>UndefinedValueMode</c> enumeration defines what value should
	/// be used by <see cref="StructuredData.GetValue(string)"/> or
	/// <see cref="StructuredData.GetValue(Druid)"/> when an undefined
	/// value is found.
	/// </summary>
	public enum UndefinedValueMode
	{
		/// <summary>
		/// Return <see cref="UndefinedValue.Value"/>.
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
