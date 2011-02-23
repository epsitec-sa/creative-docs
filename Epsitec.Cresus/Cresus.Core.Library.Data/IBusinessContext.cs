//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core
{
	public interface IBusinessContext
	{
		CoreData Data
		{
			get;
		}

		System.IDisposable AutoLock<T>(T entity)
			where T : AbstractEntity;

		T CreateEntity<T>()
			where T : AbstractEntity, new ();

		T GetLocalEntity<T>(T entity)
			where T : AbstractEntity, new ();
		
		void SaveChanges();
	}
}
