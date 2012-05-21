//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Controllers
{
	public class AiderWarningController : CoreDataComponent, System.IDisposable
	{
		public AiderWarningController(CoreData data)
			: base (data)
		{
			this.pool = data.GetComponent<BusinessContextPool> ();
			this.pool.Changed += this.HandleBusinessContextPoolChanged;
		}

		
		public static AiderWarningController	Current
		{
			get
			{
				var coreData  = CoreApp.FindCurrentAppSessionComponent<CoreData> ();
				var component = coreData.GetComponent<AiderWarningController> ();

				return component;
			}
		}


		public IEnumerable<TWarning> GetWarnings<TWarning>(IAiderWarningExampleFactoryGetter source)
			where TWarning : AbstractEntity, IAiderWarning
		{
			var factory = source.GetWarningExampleFactory ();
			var context = this.pool.FindContext (source as AbstractEntity);

			return factory.GetWarnings<TWarning> (context, source as AbstractEntity);
		}


		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return data.ContainsComponent<BusinessContextPool> ();
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new AiderWarningController (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (AiderWarningController);
			}

			#endregion
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.pool.Changed -= this.HandleBusinessContextPoolChanged;
		}

		#endregion


		private void ClearCache()
		{
		}


		private void HandleBusinessContextPoolChanged(object sender, BusinessContextEventArgs e)
		{
			switch (e.Operation)
			{
				case BusinessContextOperation.Add:
					this.HandleBusinessContextAdded (e.Context);
					break;
				case BusinessContextOperation.Remove:
					this.HandleBusinessContextRemoved (e.Context);
					break;
			}
		}

		private void HandleBusinessContextAdded(BusinessContext context)
		{
		}

		private void HandleBusinessContextRemoved(BusinessContext context)
		{
			this.ClearCache ();
		}


		private readonly BusinessContextPool	pool;
	}
}
