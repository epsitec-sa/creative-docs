﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public abstract class GenericTile : Tile
	{
		public GenericTile()
		{
			this.Padding = new Margins (GenericTile.leftRightGap, 0, 0, 0);
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

		public override Controllers.ITileController Controller
		{
			get;
			set;
		}

		public CoreViewController SubViewController
		{
			get
			{
				return this.subViewController;
			}
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

		public override TileArrow TileArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors   (this.OutlineColors);
				arrow.SetThicknessColors (this.ThicknessColors);
				arrow.SetSurfaceColors   (this.SurfaceColors);
				arrow.MouseHilite = this.MouseHilite;

				return arrow;
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
					if (this.IsEntered || this.Hilited)
					{
						return Widgets.TileArrowMode.Visible;
					}
				}

				if (this.IsSelected)
				{
					return Widgets.TileArrowMode.Visible;
				}
			}

			if (!this.IsReadOnly)
			{
				if (this.IsSelected)
				{
					return Widgets.TileArrowMode.Visible;
				}
			}

			return Widgets.TileArrowMode.None;
		}

		private bool MouseHilite
		{
			get
			{
				return Misc.ColorsCompare (this.SurfaceColors, Tile.SurfaceHilitedColors);
			}
		}

		private IEnumerable<Color> SurfaceColors
		{
			get
			{
				if (this.IsReadOnly == false)
				{
					return Tile.SurfaceEditingColors;
				}
				else if (this.IsCompact)
				{
					if (this.AutoHilite)
					{
						if (this.IsEntered || this.Hilited)
						{
							return Tile.SurfaceHilitedColors;
						}
					}

					if (this.IsSelected)
					{
						return Tile.SurfaceSelectedContainerColors;
					}
				}

				return null;
			}
		}

		private IEnumerable<Color> OutlineColors
		{
			get
			{
				if (this.IsCompact && this.IsReadOnly)
				{
					if (this.AutoHilite && (this.IsEntered || this.Hilited))
					{
						return Tile.BorderColors;
					}

					if (this.IsSelected)
					{
						return Tile.BorderColors;
					}
				}

				return null;
			}
		}

		private IEnumerable<Color> ThicknessColors
		{
			get
			{
				return null;
			}
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


		public  static readonly double leftRightGap = 4;

		private CoreViewController subViewController;
	}
}
