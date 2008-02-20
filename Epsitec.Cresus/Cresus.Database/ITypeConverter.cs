//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>ITypeConverter</c> is used to convert types between the public
	/// raw type and the internal reprsentation used by the ADO.NET provider.
	/// </summary>
	public interface ITypeConverter
	{
		/// <summary>
		/// Verifies that the raw type is supported by the ADO.NET provider. The
		/// <c>DbRawType.Guid</c> for instance could not be supported by some
		/// providers.
		/// </summary>
		/// <param name="type">Raw type to check.</param>
		/// <returns>
		/// 	<c>true</c> if the raw type is supported natively; otherwise, <c>false</c>.
		/// </returns>
		bool CheckNativeSupport(DbRawType type);
		
		/// <summary>
		/// Find the matching type converter.
		/// </summary>
		/// <param name="type">Raw type for which to get the converter.</param>
		/// <param name="converter">The converter or <c>null</c> if the raw type is supported natively.</param>
		/// <returns><c>true</c> if a converter exists; otherwise, <c>false</c>.</returns>
		bool GetRawTypeConverter(DbRawType type, out IRawTypeConverter converter);
		
		/// <summary>
		/// Converts an object managed by ADO.NET to the specified simple type.
		/// Never call this method if the raw type is not supported natively.
		/// This usually maps to <c>TypeConverter.ConvertToSimpleType</c>.
		/// </summary>
		/// <param name="value">Raw object provided by ADO.NET.</param>
		/// <param name="simpleType">Expected simple type.</param>
		/// <param name="numDef">Expected numeric format.</param>
		/// <returns>Converted object.</returns>
		object ConvertToSimpleType(object value, DbSimpleType simpleType, DbNumDef numDef);
		
		/// <summary>
		/// Converts an object from a specified simple type to an ADO.NET compatible
		/// object. Never call this method if the raw type is not supported natively.
		/// This usually maps to <c>TypeConverter.ConvertFromSimpleType</c>.
		/// </summary>
		/// <param name="value">Object to convert.</param>
		/// <param name="simpleType">Simple type.</param>
		/// <param name="numDef">Numeric format.</param>
		/// <returns>Converted object.</returns>
		object ConvertFromSimpleType(object value, DbSimpleType simpleType, DbNumDef numDef);
	}
}
