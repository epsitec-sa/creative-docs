//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public abstract class CreationViewController<T> : EntityViewController<T>, ICreationController
		where T : AbstractEntity, new ()
	{
		protected CreationViewController(string name, T entity)
			: base (name, entity)
		{
			System.Diagnostics.Debug.Assert (this.Orchestrator != null);
			System.Diagnostics.Debug.Assert (this.Orchestrator.Data.IsDummyEntity (entity));
		}


		#region ICreationController Members

		void ICreationController.RegisterDisposeAction(System.Action disposeAction)
		{
			this.disposeAction = disposeAction;
		}

		void ICreationController.RegisterEntityCreator(System.Func<System.Action<BusinessContext, AbstractEntity>, AbstractEntity> entityCreator)
		{
			this.entityCreator = entityCreator;
		}

		#endregion

		
		internal void CreateRealEntity(System.Action<BusinessContext, T> initializer = null)
		{
			var orchestrator = this.Orchestrator;
			var business     = this.BusinessContext;

			System.Diagnostics.Debug.Assert (business != null);

			System.Action<BusinessContext, AbstractEntity> initAction;

			if (initializer == null)
			{
				initAction = (bc, x) =>
					{
					};
			}
			else
			{
				initAction = (bc, x) =>
					{
						initializer (bc, (T) x);
					};
			}
			
			T entity = this.CreateEntity (business, initAction);

//-			this.UpgradeController (entity);
		}

		private T CreateEntity(BusinessContext business, System.Action<BusinessContext, AbstractEntity> initializer)
		{
			T entity;

			if (this.entityCreator != null)
			{
				entity = this.entityCreator (initializer) as T;
			}
			else
			{
				entity = business.CreateEntity<T> ();
				initializer (business, entity);
			}
			
			return entity;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeAction != null)
				{
					this.disposeAction ();
				}
			}

			base.Dispose (disposing);
		}

		private void UpgradeController(T entity)
		{
			var orchestrator = this.Orchestrator;
			var replacementController = this.CreateReplacementController (entity);

			orchestrator.ReplaceView (this, replacementController);
		}

		private CoreViewController CreateReplacementController(T entity)
		{
			return EntityViewControllerFactory.Create (this.Name, entity, ViewControllerMode.Summary, this.Orchestrator);
		}


		private System.Action					disposeAction;
		private System.Func<System.Action<BusinessContext, AbstractEntity>, AbstractEntity>		entityCreator;
	}
}