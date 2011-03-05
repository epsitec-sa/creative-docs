using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	/// <summary>
	/// The <c>AbstractFieldSynchronizationJob</c> class is the base class that describes the
	/// modification to the fields of an <see cref="AbstractEntity"/>.
	/// </summary>
	internal abstract class AbstractFieldSynchronizationJob : AbstractSynchronizationJob
	{

		
		/// <summary>
		/// Creates a new <c>AbstractFieldSynchronizationJob</c>.
		/// </summary>
		/// <param name="dataContextId">The unique id of the <see cref="DataContext"/> that is creating the <c>AbstractFieldSynchronizationJob</c>.</param>
		/// <param name="entityKey">The <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/> targeted by the <c>AbstractFieldSynchronizationJob</c>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field targeted by the <c>AbstractFieldSynchronizationJob</c>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="entityKey"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		protected AbstractFieldSynchronizationJob(long dataContextId, EntityKey entityKey, Druid fieldId)
			: base (dataContextId, entityKey)
		{
			fieldId.ThrowIf (f => f.IsEmpty, "fieldId cannot be empty");
			
			this.FieldId = fieldId;
		}


		/// <summary>
		/// Gets the <see cref="Druid"/> of the field of the <see cref="AbstractEntity"/> targeted
		/// by this instance.
		/// </summary>
		public Druid FieldId
		{
			get;
			private set;
		}


	}


}
