//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The <c>IStructuredData</c> interface provides a <see cref="T:Binding"/>
	/// compatible way of accessing structured data (i.e. records, graphs, etc.)
	/// </summary>
	public interface IStructuredData
	{
		/// <summary>
		/// Attaches a listener to the specified structured value.
		/// </summary>
		/// <param name="name">The name of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void AttachListener(string name, PropertyChangedEventHandler handler);

		/// <summary>
		/// Detaches a listener from the specified structured value.
		/// </summary>
		/// <param name="name">The name of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void DetachListener(string name, PropertyChangedEventHandler handler);

		/// <summary>
		/// Gets the collection of the names used to define the structured values.
		/// </summary>
		/// <returns>The collection of names.</returns>
		IEnumerable<string> GetValueNames();

		/// <summary>
		/// Gets the structured value with the specified name.
		/// </summary>
		/// <param name="name">The name of the value.</param>
		/// <returns>The value, or <see cref="T:UndefinedValue.Instance"/> if no
		/// value exists in the structured data record.</returns>
		object GetValue(string name);

		/// <summary>
		/// Sets the structured value.
		/// </summary>
		/// <param name="name">The name of the value.</param>
		/// <param name="value">The value to store into the structure record;
		/// specifying <see cref="T:UndefinedValue.Instance"/> clears the value.</param>
		void SetValue(string name, object value);
	}
}
