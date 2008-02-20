//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The <c>IStructuredData</c> interface provides a <see cref="Binding"/>
	/// compatible way of accessing structured data (i.e. records, graphs, etc.)
	/// </summary>
	public interface IStructuredData : IValueStore
	{
		/// <summary>
		/// Attaches a listener to the specified structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void AttachListener(string id, PropertyChangedEventHandler handler);

		/// <summary>
		/// Sets the value. See <see cref="IValueStore.SetValue"/> for additional
		/// details (the default mode will be used).
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="value">The value.</param>
		void SetValue(string id, object value);

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
	}
}
