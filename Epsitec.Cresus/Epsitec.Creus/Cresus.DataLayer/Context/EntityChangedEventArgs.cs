using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Context
{


	/// <summary>
	/// The <c>EntityEventArgs</c> is a container for the data describing an event related to an
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	public class EntityChangedEventArgs : EventArgs
	{


		/// <summary>
		/// Creates a new <c>EntityEventArgs.</c>
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the event.</param>
		/// <param name="eventType">The type of the event.</param>
		/// <param name="eventSource">The source of the event.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public EntityChangedEventArgs(AbstractEntity entity, EntityChangedEventType eventType, EntityChangedEventSource eventSource)
			: base ()
		{
			entity.ThrowIfNull ("entity");

			this.Entity = entity;
			this.EventType = eventType;
			this.EventSource = eventSource;
		}


		/// <summary>
		/// The <see cref="AbstractEntity"/> concerned by the event.
		/// </summary>
		public AbstractEntity Entity
		{
			get;
			private set;
		}


		/// <summary>
		/// The type of the event.
		/// </summary>
		public EntityChangedEventType EventType
		{
			get;
			set;
		}


		/// <summary>
		/// The source of the event.
		/// </summary>
		public EntityChangedEventSource EventSource
		{
			get;
			private set;
		}


	}


}
