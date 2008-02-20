//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ISerializationConverterFilter</c> is used by the serialization
	/// engine to filter elements when converting members of a collection to
	/// strings.
	/// </summary>
	public interface ISerializationConverterFilter
	{
		/// <summary>
		/// Determines whether the specified value is serializable.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="context">The context.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is serializable; otherwise, <c>false</c>.
		/// </returns>
		bool IsSerializable(object value, IContextResolver context);
	}
}
