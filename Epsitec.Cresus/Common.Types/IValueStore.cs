//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IValueStore</c> interface provides the basic getter and setter
	/// used to access values in a generic data store.
	/// </summary>
	public interface IValueStore
	{
		/// <summary>
		/// Gets the value for the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <returns>The value, or either <see cref="UndefinedValue.Value"/> if the
		/// value is currently undefined or <see cref="UnknownValue.Value"/> if the
		/// identifier does not map to a known value.</returns>
		object GetValue(string id);

		/// <summary>
		/// Sets the value for the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="value">The value to store into the structure record;
		/// specifying <see cref="UndefinedValue.Value"/> clears the value.
		/// <see cref="UnknownValue.Value"/> may not be specified as a value.</param>
		/// <param name="mode">The set mode.</param>
		void SetValue(string id, object value, ValueStoreSetMode mode);
	}
}
