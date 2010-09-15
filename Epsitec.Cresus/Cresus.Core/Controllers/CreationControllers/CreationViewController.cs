//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.BusinessLogic;
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

		/* no member */

		#endregion

		
		internal void CreateRealEntity(System.Action<BusinessContext, T> initializer = null)
		{
			var orchestrator = this.Orchestrator;
			var business     = this.BusinessContext;

			System.Diagnostics.Debug.Assert (business != null);

			var entity = business.CreateEntity<T> ();

			if (initializer != null)
			{
				initializer (business, entity);
			}

			this.UpgradeController (entity);
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
	}
}