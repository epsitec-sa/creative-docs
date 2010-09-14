//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController : CoreViewController
	{
		protected EntityViewController(string name)
			: base (name)
		{
		}



		/// <summary>
		/// Gets the edition status of the entity.
		/// </summary>
		/// <value>The edition status.</value>
		public EditionStatus EditionStatus
		{
			get
			{
				return this.GetEditionStatus ();
			}
		}

		public TileContainer TileContainer
		{
			get;
			protected set;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public abstract AbstractEntity GetEntity();

		protected virtual EditionStatus GetEditionStatus()
		{
			return EditionStatus.Unknown;
		}

		protected abstract void CreateUI();

		public T NotifyChildItemCreated<T>(T entity)
			where T : AbstractEntity
		{
			this.OnChildItemCreated (entity);
			return entity;
		}

		public void NotifyChildItemDeleted<T>(T entity)
			where T : AbstractEntity
		{
		}

		
		protected void ReopenSubView(params NavigationPathElement[] elements)
		{
			var orchestrator = this.Orchestrator;
			var navigator    = this.Navigator;
			var history      = navigator.History;
			var path         = navigator.GetLeafNavigationPath ();

			path.AddRange (elements);

			using (history.SuspendRecording ())
			{
				history.NavigateInPlace (path);
			}
		}


		
		public static EntityViewController CreateEntityViewController(string name, Marshaler marshaler, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement = null)
		{
			var entity = marshaler.GetValue<AbstractEntity> ();
			int index  = marshaler.GetCollectionIndex ();
			string path;

			if (index < 0)
			{
				path = marshaler.GetGetterExpression ().ToString ();
			}
			else
			{
				path = string.Concat (marshaler.GetGetterExpression ().ToString (), "[", marshaler.GetCollectionIndex ().ToString (System.Globalization.CultureInfo.InvariantCulture), "]");
			}

			System.Diagnostics.Debug.WriteLine ("EntityViewController --> " + path);

			return EntityViewController.CreateEntityViewController (name, entity, mode, orchestrator, navigationPathElement: navigationPathElement);
		}

		public static EntityViewController CreateEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, int controllerSubTypeId = -1, NavigationPathElement navigationPathElement = null)
		{
			if (entity == null)
			{
				return null;
			}

			var controller = EntityViewControllerResolver.Resolve (orchestrator, name, entity, mode, controllerSubTypeId, navigationPathElement);

			if (controller == null)
			{
				if (mode == ViewControllerMode.Creation)
				{
					return EntityViewController.CreateEntityViewController (name, entity, ViewControllerMode.Summary, orchestrator, controllerSubTypeId, navigationPathElement);
				}

				return null;
			}

			return controller;
		}


		protected virtual void OnChildItemCreated(AbstractEntity entity)
		{
		}
	}
}