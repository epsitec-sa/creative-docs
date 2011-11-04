//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

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

		public abstract BrickWall CreateBrickWallForInspection();

		public abstract void BuildBricksForInspection(BrickWall wall);

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
	}
}