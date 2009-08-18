//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class MainScrollListController
	{
		public MainScrollListController(ScrollListMultiSelect list)
		{
			this.ScrollList = list;

			this.ScrollList.DragMultiSelectionStarted += this.HandleDragMultiSelectionStarted;
			this.ScrollList.DragMultiSelectionEnded   += this.HandleDragMultiSelectionEnded;
			this.ScrollList.MultiSelectionChanged     += this.HandleMultiSelectionChanged;
			this.ScrollList.MouseMove                 += this.HandleMouseMove;
			this.ScrollList.Exited                    += this.HandleMouseExited;

			this.ScrollList.PaintForeground += this.HandlePaintForeground;

			this.hotRowIndex = -1;
		}

		public ScrollListMultiSelect ScrollList
		{
			get;
			private set;
		}


		private void HandleDragMultiSelectionStarted(object sender, MultiSelectEventArgs e)
		{
			if (Message.CurrentState.IsControlPressed == false)
			{
				this.ScrollList.ClearSelection ();
			}

			this.selection = null;
		}

		private void HandleDragMultiSelectionEnded(object sender, MultiSelectEventArgs e)
		{
			if ((e.Count == 1) &&
				(this.ScrollList.IsItemSelected (e.BeginIndex)))
			{
				this.ScrollList.RemoveSelection (Enumerable.Range (e.BeginIndex, e.Count));
			}
			else
			{
				this.ScrollList.AddSelection (Enumerable.Range (e.BeginIndex, e.Count));
			}

			this.selection = e;
		}

		private void HandleMultiSelectionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.OnChanged ();
			this.selection = new MultiSelectEventArgs (this.ScrollList.SelectedIndex, this.ScrollList.SelectedIndex);
		}

		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			this.NotifyHotRow (-1);
		}

		private void HandleMouseMove(object sender, MessageEventArgs e)
		{
			int count = this.ScrollList.VisibleRowCount;
			int first = this.ScrollList.FirstVisibleRow;

			for (int i = 0; i < count; i++)
			{
				int index = first + i;
				var frame = this.ScrollList.GetRowBounds (index);

				if (frame.Contains (e.Point))
				{
					this.NotifyHotRow (index);
					return;
				}
			}
		}

		private void HandlePaintForeground(object sender, PaintEventArgs e)
		{
			int index = this.ScrollList.SelectedIndex;

			if ((index >= 0) &&
				(this.selection != null))
			{
				var graphics  = e.Graphics;
				var bounds    = this.ScrollList.GetRowBounds (index);
				var rectangle = this.selection == null ? bounds : this.ScrollList.GetRowBounds (this.selection.BeginIndex, this.selection.Count);

				double zoneWidth = 56;

				using (Path path = new Path ())
				{
					double ox = bounds.Right - zoneWidth + 1;
					double oy = rectangle.Bottom;
					double cy = bounds.Center.Y;

					path.MoveTo (ox, oy);
					path.LineTo (ox, cy-6);
					path.LineTo (ox+6, cy);
					path.LineTo (ox, cy+6);
					path.LineTo (ox, rectangle.Top);
					path.LineTo (rectangle.TopRight);
					path.LineTo (rectangle.BottomRight);
					path.Close ();

					Color background = Color.FromName ("ActiveCaption");

					graphics.Color = Color.Mix (background, Color.FromBrightness (1), 0.3333);

					graphics.PaintSurface (path);
					graphics.RenderSolid ();
				}

				using (Path path = new Path ())
				{
					double ox = bounds.Right - zoneWidth + 0.5;
					double oy = rectangle.Bottom;
					double cy = bounds.Center.Y;

					path.MoveTo (ox, oy);
					path.LineTo (ox, cy-6);
					path.LineTo (ox+6, cy);
					path.LineTo (ox, cy+6);
					path.LineTo (ox, rectangle.Top);

					graphics.LineWidth = 1.0;
					graphics.LineCap = CapStyle.Butt;
					graphics.LineJoin = JoinStyle.Miter;
					graphics.Color = Color.FromBrightness (1);

					graphics.PaintOutline (path);
					graphics.RenderSolid ();
				}
			}
		}

		
		private void NotifyHotRow(int index)
		{
			if (this.hotRowIndex != index)
			{
				//	TODO: handle hover over row in scroll list

				this.hotRowIndex = index;
			}
		}

		private void OnChanged()
		{
			var handler = this.Changed;

			if (handler != null)
			{
				handler (this);
			}
		}

		
		public event EventHandler Changed;

		private int hotRowIndex;
		private MultiSelectEventArgs selection;
	}
}
