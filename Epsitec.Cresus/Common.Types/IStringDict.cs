//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IStringDict</c> interface gives access to a string based dictionary.
	/// </summary>
	public interface IStringDict
	{
		/// <summary>
		/// Gets the keys of the known values.
		/// </summary>
		/// <value>The keys.</value>
		string[] Keys
		{
			get;
		}


		/// <summary>
		/// Gets or sets the value with the specified key.
		/// </summary>
		/// <value>The <c>string</c> value.</value>
		string this[string key]
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the number of known values.
		/// </summary>
		/// <value>The number of known values.</value>
		int Count
		{
			get;
		}

		/// <summary>
		/// Adds the specified key/value pair to the dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void Add(string key, string value);

		/// <summary>
		/// Removes the specified key/value pair from the dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the key was successfully removed; otherwise, <c>false</c>.
		/// </returns>
		bool Remove(string key);

		/// <summary>
		/// Clears the dictionary.
		/// </summary>
		void Clear();

		/// <summary>
		/// Determines whether the dictionary contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the dictionary contains the specified key; otherwise, <c>false</c>.
		/// </returns>
		bool ContainsKey(string key);
	}
}
