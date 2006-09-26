//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStructuredType</c> interface makes the metadata about a structured
	/// tree accessible.
	/// </summary>
	public interface IStructuredType
	{
		/// <summary>
		/// Gets the type for the named field.
		/// </summary>
		/// <param name="name">The field name.</param>
		/// <returns>The <see cref="INamedType"/> type.</returns>
		INamedType GetFieldType(string name);

		/// <summary>
		/// Gets the field names.
		/// </summary>
		/// <returns>An array of sorted field names.</returns>
		string[] GetFieldNames();
	}
}
