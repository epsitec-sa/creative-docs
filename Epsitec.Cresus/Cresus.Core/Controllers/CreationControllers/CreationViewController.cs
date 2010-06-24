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
		where T : AbstractEntity
	{
		protected CreationViewController(string name, T entity)
			: base (name, entity)
		{
		}

		protected override void AboutToCloseUI()
		{
			base.AboutToCloseUI ();

			if (this.CreationStatus == CreationStatus.Empty)
			{
				this.DataContext.DeleteEntity (this.Entity);
			}
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

			controller.DataContext = this.DataContext;

			orchestrator.ReplaceView (this, controller);
		}

		protected virtual CreationStatus GetCreationStatus()
		{
			return CreationStatus.Unknown;
		}
	}
}