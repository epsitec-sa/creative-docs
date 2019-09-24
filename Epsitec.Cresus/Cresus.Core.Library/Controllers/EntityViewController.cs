//	Copyright © 2010-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

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


		public TileContainer					TileContainer
		{
			get;
			protected set;
		}

		
		public override void Focus()
		{
			Epsitec.Cresus.Core.Library.UI.Services.SetInitialFocus (this.TileContainer);
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public abstract AbstractEntity GetEntity();

		public virtual EntityViewController GetController()
		{
			return this;
		}

		
		protected virtual EntityStatus GetEditionStatus()
		{
			return EntityStatus.None;
		}

		protected abstract void CreateUI();

		
		internal EntityViewController NotifyAboutToCreateUI()
		{
			this.AboutToCreateUI ();
			return this;
		}

		internal abstract Bridge CreateBridgeAndBuildBricks();

		
		public abstract BrickWall BuildBrickWall();

		public abstract void BuildBricks(BrickWall wall);

		public T NotifyChildItemCreated<T>(T entity)
			where T : AbstractEntity
		{
			return entity;
		}

		public void NotifyChildItemDeleted<T>(T entity)
			where T : AbstractEntity
		{
		}


		/// <summary>
		/// Opens a linked sub-view (if any). This gets called by the <see cref="DataViewController"/>
		/// after this controller's view was successfully recorded in a data view column.
		/// </summary>
		public override void OpenLinkedSubView()
		{
			var summaryTiles   = this.TileContainer.Children.OfType<TitleTile> ().SelectMany (x => x.Items.OfType<SummaryTile> ());
			var defaultSummary = summaryTiles.FirstOrDefault ();

			if (defaultSummary != null)
			{
				defaultSummary.ToggleSubView (this.Orchestrator, this);
			}
		}


		/// <summary>
		/// Reopens the same view, or the sub-view specified by the navigation path elements.
		/// </summary>
		/// <param name="elements">The navigation path elements to the sub-view.</param>
		protected void ReopenSubView(params NavigationPathElement[] elements)
		{
			var navigator = this.Navigator;
			var history   = navigator.History;
			var path      = navigator.GetLeafNavigationPath ();

			path.AddRange (elements);

			using (history.SuspendRecording ())
			{
				history.NavigateInPlace (path);
			}
		}



		/// <summary>
		/// This method checks that the controller type can be used with the given entity type.
		/// </summary>
		public static bool AreCompatible(System.Type entityType, System.Type controllerType)
		{
			if (!typeof (AbstractEntity).IsAssignableFrom (entityType))
			{
				throw new System.ArgumentException ("Invalid entity type: " + entityType.FullName);
			}

			if (!typeof (EntityViewController).IsAssignableFrom (controllerType))
			{
				throw new System.ArgumentException ("Invalid controller type: " + controllerType.FullName);
			}

			var entityViewControllerType = typeof (EntityViewController<>);

			var entityTypes = entityType
				.GetBaseTypes ()
				.TakeWhile (t => t != typeof (AbstractEntity));

			foreach (var type in entityTypes)
			{
				var expectedControllerType = entityViewControllerType.MakeGenericType (type);

				if (expectedControllerType.IsAssignableFrom (controllerType))
				{
					return true;
				}
			}

			return false;
		}
	}
}
