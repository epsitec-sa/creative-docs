//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INullableType</c> interface can be used to check whether an object
	/// of a given type represents the <c>null</c> value.
	/// </summary>
	public interface INullableType
	{
		/// <summary>
		/// Gets a value indicating whether the value is null.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value represents the <c>null</c> value; otherwise, <c>false</c>.
		/// </returns>
		bool IsNullValue(object value);

		/// <summary>
		/// Gets a value indicating whether this type may represent <c>null</c> values.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this type may represent <c>null</c> values; otherwise, <c>false</c>.
		/// </value>
		bool IsNullable
		{
			get;
		}
	}
}
