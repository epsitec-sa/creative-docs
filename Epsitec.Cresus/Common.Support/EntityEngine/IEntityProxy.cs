//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Types;


namespace Epsitec.Common.Support.EntityEngine
{


	/// <summary>
	/// The <c>IEntityProxy</c> interface is used by the <see cref="IValueStore"/>
	/// implementer to translate between an entity proxy and its real instance.
	/// </summary>
	public interface IEntityProxy
	{
		
		
		/// <summary>
		/// Gets the real instance to be used when reading on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		object GetReadEntityValue(IValueStore store, string id);

		
		/// <summary>
		/// Gets the real instance to be used when writing on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		object GetWriteEntityValue(IValueStore store, string id);

		
		/// <summary>
		/// Checks if the write to the specified entity value should proceed
		/// normally or be discarded completely.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the value should be discarded; otherwise, <c>false</c>.</returns>
		bool DiscardWriteEntityValue(IValueStore store, string id, ref object value);


		/// <summary>
		/// Promotes the proxy to its real instance.
		/// </summary>
		/// <returns>The real instance.</returns>
		object PromoteToRealInstance();
	
	
	}


}
