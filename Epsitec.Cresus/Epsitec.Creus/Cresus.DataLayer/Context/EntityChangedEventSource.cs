using Epsitec.Common.Support.EntityEngine;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	/// <summary>
	/// The <c>EntityEventSource</c> enum describes the possible sources for an event related to an
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	public enum EntityChangedEventSource
	{
		
		
		/// <summary>
		/// The event has been fired because of an internal modification. Such an event occurs when
		/// the <see cref="DataContext"/> updates an <see cref="AbstractEntity"/> for consistency
		/// matters. For example, if an <see cref="AbstractEntity"/> which is still referenced by
		/// others in the same <see cref="DataContext"/> is deleted, these references will be removed
		/// an event with the <c>Internal</c> source parameter will be fired for each of them.
		/// </summary>
		Internal,
		
		
		/// <summary>
		/// The event has been fired because of an external modification. This typically happens when
		/// a field of an <see cref="AbstractEntity"/> is modified, and when an <see cref="AbstractEntity"/>
		/// is created or deleted.
		/// </summary>
		External,


		/// <summary>
		/// The event has been fired because the data of the <see cref="AbstractEntity"/> has been
		/// reloaded from the database, and its data was not the same as the reloaded data.
		/// </summary>
		Reload,
		
		
		/// <summary>
		/// The event has been fired because of synchronization modifications. If an
		/// <see cref="AbstractEntity"/> is modified in a <see cref="DataContext"/> and this
		/// <see cref="DataContext"/> is saved, these modifications will be propagated to the other
		/// <see cref="DataContext"/> that also manage the same <see cref="AbstractEntity"/>. For
		/// each of these modification, an event is fired with the <c>Synchronization</c> source
		/// parameter.
		/// </summary>
		Synchronization,
	
	
	}


}
