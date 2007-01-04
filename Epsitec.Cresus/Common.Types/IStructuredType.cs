//	Copyright � 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		/// Gets a collection of field identifiers.
		/// </summary>
		/// <returns>A collection of field identifiers.</returns>
		IEnumerable<string> GetFieldIds();

		/// <summary>
		/// Gets the field descriptor for the specified field identifier.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <returns>The matching field descriptor; otherwise, <c>null</c>.</returns>
		StructuredTypeField GetField(string fieldId);
	}
}
