using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	/// <summary>
	/// The <c>ValueSynchronizationJob</c> class describes the modifications that have been made to
	/// a value field of an <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ValueSynchronizationJob : AbstractFieldSynchronizationJob
	{
		

		/// <summary>
		/// Creates a new <c>ValueSynchronizationJob</c>.
		/// </summary>
		/// <param name="dataContextId">The unique id of the <see cref="DataContext"/> that is creating the <c>ValueSynchronizationJob</c>.</param>
		/// <param name="entityKey">The <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/> targeted by the <c>ValueSynchronizationJob</c>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field targeted by the <c>ValueSynchronizationJob</c></param>
		/// <param name="newValue">The new value of the field targeted by the <c>ValueSynchronizationJob</c></param>
		/// <exception cref="System.ArgumentException">If <paramref name="entityKey"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		
		public ValueSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId, object newValue)
			: base (dataContextId, entityKey, fieldId)
		{
			this.NewValue = newValue;
		}


		/// <summary>
		/// The new value of the field of the <see cref="AbstractEntity"/> targeted by this
		/// instance.
		/// </summary>
		public object NewValue
		{
			get;
			private set;
		}


	}


}
