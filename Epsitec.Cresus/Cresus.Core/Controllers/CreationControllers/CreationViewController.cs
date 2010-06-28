//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer;

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
		}


		#region ICreationController Members

		public ViewControllerMode UpgradeControllerMode
		{
			get
			{
				return this.GetUpgradeControllerMode ();
			}
		}

		#endregion

		protected void UpgradeController()
		{
			var upgradeMode = this.UpgradeControllerMode;

			if (upgradeMode == ViewControllerMode.Creation)
			{
				//	Do not upgrade the controller: we are already the creation mode controller
			}
			else
			{
				var orchestrator = this.Orchestrator;
				var controller   = EntityViewController.CreateEntityViewController (this.Name, this.Entity, upgradeMode, orchestrator);

				controller.DataContext = orchestrator.DataContext;

				orchestrator.ReplaceView (this, controller);
			}
		}

		protected virtual ViewControllerMode GetUpgradeControllerMode()
		{
			switch (this.EditionStatus)
			{
				case EditionStatus.Empty:
				default:
					return ViewControllerMode.Creation;

				case EditionStatus.Valid:
					return ViewControllerMode.Summary;
			}
		}

		protected void CreateRealEntity(System.Action<DataContext, T> initializer = null)
		{
			var orchestrator = this.Orchestrator;
			var context      = orchestrator.DataContext;
			var entity       = context.CreateEmptyEntity<T> ();

			if (initializer != null)
			{
				initializer (context, entity);
			}

			this.ReplaceEntity (entity);
			this.UpgradeController ();
		}
	}
}