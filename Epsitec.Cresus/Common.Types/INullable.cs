//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INullable</c> interface can be used to check whether a value type
	/// encodes the <c>null</c> value.
	/// </summary>
	public interface INullable
	{
		/// <summary>
		/// Gets a value indicating whether this value is null.
		/// </summary>
		/// <value><c>true</c> if this value is null; otherwise, <c>false</c>.</value>
		bool IsNull
		{
			get;
		}
	}
}
