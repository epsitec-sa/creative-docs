//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public class GenericTile : Tile
	{
		public GenericTile()
		{
		}

		public GenericTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}



		/// <summary>
		/// Détermine si le widget est sensible au survol de la souris.
		/// </summary>
		/// <value><c>true</c> if [entered sensitivity]; otherwise, <c>false</c>.</value>
		public bool AutoHilite
		{
			get;
			set;
		}

		public bool AutoReverse
		{
			get;
			set;
		}

		public bool Hilited
		{
			get;
			set;
		}

		public bool IsCompact
		{
			get;
			set;
		}

		public Controllers.ITileController Controller
		{
			get;
			set;
		}

		
		public override TileArrowMode ArrowMode
		{
			get
			{
				return this.GetPaintingArrowMode ();
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		public override TileArrow DirectArrow
		{
			get
			{
				return new TileArrow ()
				{
					OutlineColor   = this.GetOutlineColor (),
					ThicknessColor = this.GetThicknessColor (),
					SurfaceColor   = this.GetSurfaceColor (),
				};
			}
		}

		public override TileArrow ReverseArrow
		{
			get
			{
				return new TileArrow ()
				{
					OutlineColor   = this.GetReverseOutlineColor (),
					ThicknessColor = this.GetReverseThicknessColor (),
					SurfaceColor   = this.GetReverseSurfaceColor (),
				};
			}
		}



		public void ToggleSubView(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController)
		{
			if (this.IsSelected)
			{
				//	If the tile was selected, deselect it by closing its sub-view:
				this.CloseSubView (orchestrator);
			}
			else
			{
				this.OpenSubView (orchestrator, parentController);
			}
		}

		public void CloseSubView(Orchestrators.DataViewOrchestrator orchestrator)
		{
			System.Diagnostics.Debug.Assert (this.subViewController != null);
			System.Diagnostics.Debug.Assert (orchestrator != null);

			orchestrator.CloseView (this.subViewController);
			
			System.Diagnostics.Debug.Assert (this.subViewController == null);
			System.Diagnostics.Debug.Assert (!this.IsSelected);
		}

		public void OpenSubView(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController)
		{
			var controller = this.CreateSubViewController (orchestrator);

			if (controller != null)
			{
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
					if (this.IsEntered || this.Hilited)
					{
						if (this.IsSelected && this.AutoReverse)
						{
							return Widgets.TileArrowMode.VisibleReverse;
						}
						else
						{
							return Widgets.TileArrowMode.VisibleDirect;
						}
					}
				}

				if (this.IsSelected)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}
			}

			return Widgets.TileArrowMode.None;
		}

		private Color GetSurfaceColor()
		{
			if (this.IsReadOnly == false)
			{
				return Tile.SurfaceEditingColor;
			}
			else if (this.IsCompact)
			{
				if (this.AutoHilite)
				{
					if (this.IsEntered || this.Hilited)
					{
						if (this.IsSelected)
						{
							return Tile.SurfaceHilitedColor;
						}
						else
						{
							return Tile.ThicknessHilitedColor;
						}
					}
				}

				if (this.IsSelected)
				{
					return Tile.SurfaceSelectedContainerColor;
				}
			}

			return Color.Empty;
		}

		private Color GetOutlineColor()
		{
			if (this.IsCompact && this.IsReadOnly)
			{
				if (this.AutoHilite && (this.IsEntered || this.Hilited))
				{
					return Tile.BorderColor;
				}

				if (this.IsSelected)
				{
					return Tile.BorderColor;
				}
			}

			return Color.Empty;
		}

		private Color GetThicknessColor()
		{
			return Color.Empty;
		}

		private Color GetReverseSurfaceColor()
		{
			if (this.IsReadOnly == false)
			{
				return Color.Empty;
			}
			else
			{
				return Tile.ThicknessHilitedColor;
			}
		}

		private Color GetReverseOutlineColor()
		{
			return Tile.BorderColor;
		}

		private Color GetReverseThicknessColor()
		{
			return Color.Empty;
		}


		private EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
		{
			if (this.Controller == null)
			{
				return null;
			}
			else
			{
				return this.Controller.CreateSubViewController (orchestrator);
			}
		}
		
		private void HandleSubViewControllerDisposing(object sender)
		{
			this.SetSelected (false);
			this.subViewController = null;
		}

		private CoreViewController subViewController;
	}
}
