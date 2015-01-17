﻿//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TileContainer</c> contains a stack of tiles. It paints a frame which
	/// takes into account the tile arrows. There is exactly one <c>TileContainer</c>
	/// for every column managed by the <see cref="ViewLayoutController"/>.
	/// </summary>
	public sealed class TileContainer : FrameBox, IWidgetUpdater
	{
		public TileContainer(CoreViewController controller)
		{
			this.controller = controller;
			this.widgetUpdaters = new List<IWidgetUpdater> ();

			this.TabNavigationMode = TabNavigationMode.ForwardTabActive | TabNavigationMode.ForwardToChildren;
			this.TabIndex = 1;
		}


		public EntityViewController				EntityViewController
		{
			get
			{
				return this.controller as EntityViewController;
			}
		}

		public TileContainerController			TileContainerController
		{
			get;
			set;
		}

		
		public void Add(IWidgetUpdater widgetUpdater)
		{
			this.widgetUpdaters.Add (widgetUpdater);
		}

		public void UpdateAllWidgets()
		{
			this.controller.Orchestrator.MainWindowController.Update ();
		}

		public double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return this.controller.GetPreferredWidth (columnCount, columnCount);
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			foreach (var updater in this.widgetUpdaters.ToArray ())
			{
				updater.Update ();
			}
		}

		#endregion

		protected override void MeasureMinMax(ref Size min, ref Size max)
		{
			//-base.MeasureMinMax (ref min, ref max);
		}

		protected override Rectangle GetFrameRectangle()
		{
			var margins = Widgets.Tiles.TileArrow.GetContainerPadding (Direction.Right) + new Margins (0.5);
			var frame   = Rectangle.Deflate (this.Client.Bounds, margins);
			
			return frame;
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if ((this.DrawFrameEdges & FrameEdges.Right) != 0)
			{
				var adorner = Common.Widgets.Adorners.Factory.Active;
				var gradientRect = new Rectangle (this.Client.Bounds.Right-Tiles.TileArrow.Breadth, this.Client.Bounds.Bottom, Tiles.TileArrow.Breadth*0.5, this.Client.Bounds.Height);

				graphics.AddFilledRectangle (gradientRect);
				graphics.PaintHorizontalGradient (gradientRect, Color.FromAlphaColor (0.3, adorner.ColorBorder), Color.FromAlphaColor (0.0, adorner.ColorBorder));
			}
		}

		protected override Widget TabNavigate(int index, TabNavigationDir dir, Widget[] siblings)
		{
			if (dir == TabNavigationDir.Backwards ||
				dir == TabNavigationDir.Forwards  )
			{
				var e = new TabNavigateEventArgs (dir);
				var window = this.Window;

				this.OnTabNavigating (e);

				if (e.Cancel)
				{
					//	If the navigation was canceled, this implies that the currently focused
					//	widget has to be kept focused (it might have been focused manually in the
					//	process of handling the TabNavigating event).
					
					return window.FocusedWidget ?? this;
				}

				var result = e.ReplacementWidget;

				if (result != null)
				{
					return result;
				}
			}
			
			return base.TabNavigate (index, dir, siblings);
		}

		private void OnTabNavigating(TabNavigateEventArgs e)
		{
			var handler = this.TabNavigating;

			if (handler != null)
			{
				handler (this, e);
			}
		}


		public event EventHandler<TabNavigateEventArgs>		TabNavigating;

		private readonly CoreViewController		controller;
		private readonly List<IWidgetUpdater>	widgetUpdaters;
	}
}