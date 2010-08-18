﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

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
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			if (this.replacementController != null)
            {
				yield return this.replacementController;
            }
		}

		public override bool InheritDataContext
		{
			get
			{
				return false;
			}
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

		public override CoreViewController GetReplacementController()
		{
			var upgradeMode = this.UpgradeControllerMode;

			if (upgradeMode == ViewControllerMode.Creation)
			{
				return base.GetReplacementController ();
			}
			else
			{
				if ((this.replacementController != null) &&
					(this.replacementController.Mode != upgradeMode))
				{
					this.replacementController.Dispose ();
					this.replacementController = null;
				}

				if (this.replacementController == null)
				{
					var orchestrator = this.Orchestrator;
					var controller   = EntityViewController.CreateEntityViewController (this.Name, this.Entity, upgradeMode, orchestrator) as EntityViewController<T>;
					var context      = orchestrator.DataContext;

					controller.DataContext = context;

					if (context.Contains (this.Entity) == false)
					{
						controller.ReplaceEntity (context.CreateEntity<T> ());
					}

					this.replacementController = controller;
				}
				
				return this.replacementController;
			}
		}

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
				orchestrator.ReplaceView (this, this);
			}
		}

		protected virtual ViewControllerMode GetUpgradeControllerMode()
		{
			switch (this.EditionStatus)
			{
				case EditionStatus.Empty:
					return ViewControllerMode.Creation;

				case EditionStatus.Invalid:
				case EditionStatus.Valid:
					return ViewControllerMode.Summary;

				case EditionStatus.Unknown:
				default:
					throw new System.InvalidOperationException (string.Format ("EditionStatus may not be set to {0}", this.EditionStatus));
			}
		}

		internal void CreateRealEntity(System.Action<DataContext, T> initializer = null)
		{
			var orchestrator = this.Orchestrator;
			var context      = orchestrator.DataContext;
			var entity       = context.CreateEntity<T> ();

			if (initializer != null)
			{
				initializer (context, entity);
			}

			this.ReplaceEntity (entity);
			this.UpgradeController ();
		}

		protected EntityViewController replacementController;
	}
}