//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The <c>IStructuredData</c> interface provides a <see cref="Binding"/>
	/// compatible way of accessing structured data (i.e. records, graphs, etc.)
	/// </summary>
	public interface IStructuredData
	{
		/// <summary>
		/// Attaches a listener to the specified structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void AttachListener(string id, PropertyChangedEventHandler handler);

		/// <summary>
		/// Detaches a listener from the specified structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void DetachListener(string id, PropertyChangedEventHandler handler);

		/// <summary>
		/// Gets the collection of identifiers used to define the structured values.
		/// </summary>
		/// <returns>The collection of identifiers.</returns>
		IEnumerable<string> GetValueIds();

		/// <summary>
		/// Gets the structured value with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <returns>The value, or <see cref="UndefinedValue.Instance"/> if no
		/// value exists in the structured data record.</returns>
		object GetValue(string id);

		/// <summary>
		/// Sets the structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="value">The value to store into the structure record;
		/// specifying <see cref="UndefinedValue.Instance"/> clears the value.</param>
		void SetValue(string id, object value);
	}
}
