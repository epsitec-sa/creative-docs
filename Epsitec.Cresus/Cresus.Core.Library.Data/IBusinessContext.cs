//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Repositories;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public interface IBusinessContext : System.IDisposable
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

		void DeleteEntity(AbstractEntity entity);

		T GetLocalEntity<T>(T entity)
			where T : AbstractEntity, new ();

		T GetMasterEntity<T>()
			where T : AbstractEntity, new ();

		Repository<T> GetRepository<T>()
			where T : AbstractEntity, new ();

		void Register(AbstractEntity entity);
		
		void SaveChanges();

		void Discard();

		Data.GlobalLock GlobalLock
		{
			get;
			set;
		}
	}
}
