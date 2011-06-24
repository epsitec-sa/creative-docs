//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Repositories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>IBusinessContext</c> interface defines the methods which need to be
	/// accessed by users of the <c>BusinessContext</c> class, but without a direct
	/// reference to the <c>Core.Business</c> assembly.
	/// </summary>
	public interface IBusinessContext : System.IDisposable, ICoreManualComponent
	{
		CoreData Data
		{
			get;
		}

		DataContext DataContext
		{
			get;
		}

		System.IDisposable AutoLock<T>(T entity)
			where T : AbstractEntity;

		bool AcquireLock(out IList<Data.LockOwner> foreignLockOwners);

		T CreateEntity<T>()
			where T : AbstractEntity, new ();

		T CreateMasterEntity<T>()
			where T : AbstractEntity, new ();

		bool DeleteEntity(AbstractEntity entity);

		void AddMasterEntity(AbstractEntity masterEntity);
		
		void RemoveMasterEntity(AbstractEntity masterEntity);

		IEnumerable<T> GetAllEntities<T>()
					where T : AbstractEntity, new ();
		
		T GetLocalEntity<T>(T entity)
			where T : AbstractEntity, new ();

		T GetMasterEntity<T>()
			where T : AbstractEntity, new ();

		Repository<T> GetRepository<T>()
			where T : AbstractEntity, new ();

		T GetSpecificRepository<T>()
			where T : Repositories.Repository;

		void Register(AbstractEntity entity);
		
		void SaveChanges();

		void Discard();

		Logic CreateLogic(AbstractEntity entity);

		Data.GlobalLock GlobalLock
		{
			get;
			set;
		}

		void NotifyExternalChanges();

		event EventHandler<CancelEventArgs> SavingChanges;
	}
}
