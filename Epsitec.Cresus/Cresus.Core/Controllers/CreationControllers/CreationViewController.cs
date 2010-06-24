//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public abstract class CreationViewController<T> : EntityViewController<T>, ICreationStatus
		where T : AbstractEntity, new ()
	{
		protected CreationViewController(string name, T entity)
			: base (name, entity)
		{
		}


		#region ICreationStatus Members

		public CreationStatus CreationStatus
		{
			get
			{
				return this.GetCreationStatus ();
			}
		}

		#endregion

		protected void ValidateCreation()
		{
			System.Diagnostics.Debug.Assert (this.CreationStatus == CreationStatus.Ready);

			var orchestrator = this.Orchestrator;
			var controller   = EntityViewController.CreateEntityViewController (this.Name, this.Entity, ViewControllerMode.Summary, orchestrator);

			controller.DataContext = orchestrator.DataContext;

			orchestrator.ReplaceView (this, controller);
		}

		protected virtual CreationStatus GetCreationStatus()
		{
			return CreationStatus.Unknown;
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
			this.ValidateCreation ();
		}
	}
}