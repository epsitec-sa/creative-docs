//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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
		/// Gets the field identifiers.
		/// </summary>
		/// <returns>An array of field identifiers.</returns>
		IEnumerable<string> GetFieldIds();
	}
}
