//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		/// <summary>
		/// Gets the structured type class for this instance. The default is
		/// simply <c>StructuredTypeClass.None</c>.
		/// </summary>
		/// <returns>The structured type class to which this instance belongs.</returns>
		StructuredTypeClass GetClass();
	}
}
