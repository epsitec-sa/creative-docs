//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStructuredTree</c> interface makes the metadata about a structured
	/// tree accessible.
	/// </summary>
	public interface IStructuredTree
	{
		/// <summary>
		/// Gets the type object for the named field.
		/// </summary>
		/// <param name="name">The field name.</param>
		/// <returns>The type object (see <c>TypeRosetta</c>).</returns>
		object GetFieldTypeObject(string name);

		/// <summary>
		/// Gets the field names.
		/// </summary>
		/// <returns>An array of sorted field names.</returns>
		string[] GetFieldNames();
	}
}
