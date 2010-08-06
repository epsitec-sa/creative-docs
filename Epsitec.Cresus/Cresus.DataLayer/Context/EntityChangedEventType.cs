using Epsitec.Common.Support.EntityEngine;


namespace Epsitec.Cresus.DataLayer.Context
{
	

	/// <summary>
	/// The <c>EntityEventType</c> enum describes the possible types for the events related the the
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	public enum EntityChangedEventType
	{
		
		
		/// <summary>
		/// Indicates that the <see cref="AbstractEntity"/> has been created.
		/// </summary>
		Created,
		
		
		/// <summary>
		/// Indicates that the <see cref="AbstractEntity"/> has been deleted.
		/// </summary>
		Deleted,
		
		
		/// <summary>
		/// Indicates that a field of the <see cref="AbstractEntity"/> has been updated.
		/// </summary>
		Updated,
	
	
	}


}
