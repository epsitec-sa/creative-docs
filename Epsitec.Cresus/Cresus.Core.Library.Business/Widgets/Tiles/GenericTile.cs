//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public abstract class GenericTile : ControllerTile
	{
		protected GenericTile()
			: base (Direction.Right)
		{
			this.Padding = new Margins (GenericTile.LeftRightGap, 0, 0, 0);
		}

		
		public bool								AutoHilite
		{
			get;
			set;
		}

		public bool								IsCompact
		{
			get;
			set;
		}

		public sealed override ITileController	Controller
		{
			get;
			set;
		}

		public CoreViewController				SubViewController
		{
			get
			{
				return this.subViewController;
			}
		}
		
		public sealed override TileArrowMode	ArrowMode
		{
			get
			{
				return this.GetPaintingArrowMode ();
			}
			set
			{
				throw new System.InvalidOperationException ("GenericTile.ArrowMode is read-only");
			}
		}

		public sealed override TileArrow		Arrow
		{
			get
			{
				this.UpdateTileArrow ();
				return this.tileArrow;
			}
		}


		public void ToggleSubView(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController, NavigationPathElement navigationPathElement = null)
		{
			if (this.IsSelected)
			{
				//	If the tile was selected, deselect it by closing its sub-view:
				this.CloseSubView (orchestrator);
			}
			else
			{
				this.OpenSubView (orchestrator, parentController, navigationPathElement: navigationPathElement);
			}
		}

		public void CloseSubView(Orchestrators.DataViewOrchestrator orchestrator)
		{
			if (this.subViewController != null)
			{
				System.Diagnostics.Debug.Assert (orchestrator != null);

				orchestrator.CloseView (this.subViewController);

				System.Diagnostics.Debug.Assert (this.subViewController == null);
				System.Diagnostics.Debug.Assert (!this.IsSelected);
			}
		}

		public void OpenSubView(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController, CoreViewController subViewController = null, NavigationPathElement navigationPathElement = null)
		{
			var controller = subViewController ?? this.CreateSubViewController (orchestrator, navigationPathElement);

			if (controller != null)
			{
				System.Diagnostics.Debug.Assert (controller.NavigationPathElement != null);

				this.subViewController = controller;
				this.subViewController.Disposing += this.HandleSubViewControllerDisposing;

				orchestrator.ShowSubView (parentController, this.subViewController);

				this.SetSelected (true);

				System.Diagnostics.Debug.Assert (this.subViewController != null);
				System.Diagnostics.Debug.Assert (this.IsSelected);
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.subViewController = null;
			}

			base.Dispose (disposing);
		}

		protected virtual TileArrowMode GetPaintingArrowMode()
		{
			if (this.IsReadOnly && this.IsCompact)
			{
				if (this.AutoHilite)
				{
					if (this.IsEntered)
					{
						return Tiles.TileArrowMode.Selected;
					}
				}

				if (this.IsSelected)
				{
					return Tiles.TileArrowMode.Selected;
				}
			}

			if (!this.IsReadOnly)
			{
				if (this.IsSelected)
				{
					return Tiles.TileArrowMode.Selected;
				}
			}

			return Tiles.TileArrowMode.Normal;
		}

		protected virtual bool GetMouseHilite()
		{
			return Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedColors);
		}

		protected virtual IEnumerable<Color> GetSurfaceColors()
		{
			if (this.IsReadOnly == false)
			{
				return TileColors.SurfaceEditingColors;
			}
			else
			{
				if (this.IsCompact)
				{
					if (this.AutoHilite)
					{
						if (this.IsEntered)
						{
							return TileColors.SurfaceHilitedColors;
						}
					}
					if (this.IsSelected)
					{
						return TileColors.SurfaceSelectedContainerColors;
					}
				}
			}
			return null;
		}

		protected virtual IEnumerable<Color> GetOutlineColors()
		{
			if (this.IsCompact && this.IsReadOnly)
			{
				if (this.AutoHilite && this.IsEntered)
				{
					return TileColors.BorderColors;
				}
				if (this.IsSelected)
				{
					return TileColors.BorderColors;
				}
			}
			return null;
		}


		private void UpdateTileArrow()
		{
			this.tileArrow.SetOutlineColors (this.GetOutlineColors ());
			this.tileArrow.SetSurfaceColors (this.GetSurfaceColors ());
			this.tileArrow.MouseHilite = this.GetMouseHilite ();
		}

		private EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement)
		{
			if (this.Controller == null)
			{
				return null;
			}
			else
			{
				return this.Controller.CreateSubViewController (orchestrator, navigationPathElement);
			}
		}
		
		private void HandleSubViewControllerDisposing(object sender)
		{
			this.SetSelected (false);

			if (sender == this.subViewController)
			{
				this.subViewController = null;
			}
		}


		public static readonly double LeftRightGap = 4;

		
		private CoreViewController				subViewController;
	}
}
