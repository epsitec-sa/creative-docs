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
			this.visibleQuickButtons = new List<Button> ();

			this.quickButtonAddNegative = new GlyphButton ()
			{
				PreferredWidth = 22,
				PreferredHeight = 22,
				ButtonStyle = ButtonStyle.Icon,
				GlyphShape = GlyphShape.Minus,
				Parent = this.ScrollList.Parent,
				Anchor = AnchorStyles.BottomLeft
			};

			this.quickButtonAddPositive = new GlyphButton ()
			{
				PreferredWidth = 22,
				PreferredHeight = 22,
				ButtonStyle = ButtonStyle.Icon,
				GlyphShape = GlyphShape.Plus,
				Parent = this.ScrollList.Parent,
				Anchor = AnchorStyles.BottomLeft
			};

			this.quickButtonSum = new Button ()
			{
				PreferredWidth = 22,
				PreferredHeight = 22,
				ButtonStyle = ButtonStyle.Icon,
				Text = "Σ",
				Parent = this.ScrollList.Parent,
				Anchor = AnchorStyles.BottomLeft
			};

			this.quickButtonSum.Clicked += this.HandleButton2Clicked;
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
			this.UpdateVisibleButtons ();
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
				this.selection = e;
				this.UpdateVisibleButtons ();
			}
		}

		private void HandleMultiSelectionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.OnChanged ();

			int index = this.ScrollList.SelectedIndex;

			if ((this.ScrollList.IsItemSelected (index)) &&
				(this.ScrollList.SelectionCount == 1))
			{
				this.selection = new MultiSelectEventArgs (index, index);
			}
			else
			{
				this.selection = null;
			}

			this.UpdateVisibleButtons ();
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
				(this.selection != null) &&
				(this.selection.Count > 0))
			{
				var graphics  = e.Graphics;
				var bounds    = this.ScrollList.GetRowBounds (index);

				if ((bounds.IsEmpty) ||
					(this.visibleQuickButtons.Count == 0))
				{
					this.HideQuickButtons ();
				}
				else
				{
					var rectangle = this.selection == null ? bounds : this.ScrollList.GetRowBounds (this.selection.BeginIndex, this.selection.Count);

					double arrowLength = 6;
					double zoneWidth = arrowLength + (this.visibleQuickButtons.Count * 24) + 2;

					using (Path path = new Path ())
					{
						double ox = bounds.Right - zoneWidth + 1;
						double oy = rectangle.Bottom;
						double cy = bounds.Center.Y;

						path.MoveTo (ox, oy);
						path.LineTo (ox, cy-arrowLength);
						path.LineTo (ox+arrowLength, cy);
						path.LineTo (ox, cy+arrowLength);
						path.LineTo (ox, rectangle.Top);
						path.LineTo (rectangle.TopRight);
						path.LineTo (rectangle.BottomRight);
						path.Close ();

						Color background = Color.FromName ("ActiveCaption");

						graphics.Color = Color.Mix (background, Color.FromBrightness (1), 0.20);

						graphics.PaintSurface (path);
						graphics.RenderSolid ();
					}

					using (Path path = new Path ())
					{
						double ox = bounds.Right - zoneWidth + 0.5;
						double oy = rectangle.Bottom;
						double cy = bounds.Center.Y;

						path.MoveTo (ox, oy);
						path.LineTo (ox, cy-arrowLength);
						path.LineTo (ox+arrowLength, cy);
						path.LineTo (ox, cy+arrowLength);
						path.LineTo (ox, rectangle.Top);

						graphics.LineWidth = 1.0;
						graphics.LineCap = CapStyle.Butt;
						graphics.LineJoin = JoinStyle.Miter;
						graphics.Color = Color.FromBrightness (1);

						graphics.PaintOutline (path);
						graphics.RenderSolid ();
					}

					this.UpdateQuickButtons (new Rectangle (bounds.Right - zoneWidth + arrowLength, bounds.Bottom, zoneWidth - arrowLength, bounds.Height));
				}
			}
			else
			{
				this.HideQuickButtons ();
			}
		}

		private void UpdateQuickButtons(Rectangle frame)
		{
			System.Diagnostics.Debug.Assert (frame.IsEmpty == false);

			if (this.quickButtonsFrame == frame)
			{
				return;
			}

			this.quickButtonsFrame = frame;
			this.UpdateQuickButtons ();
		}

		private void HideQuickButtons()
		{
			this.quickButtonsFrame = Rectangle.Empty;
			this.visibleQuickButtons.Clear ();
			this.UpdateQuickButtons ();
		}

		private void UpdateQuickButtons()
		{
			double x = this.quickButtonsFrame.Left;
			double y = this.quickButtonsFrame.Bottom + this.ScrollList.ActualLocation.Y;

			foreach (var button in this.QuickButtons)
			{
				if (this.visibleQuickButtons.Contains (button))
				{
					button.Enable = true;
					button.Show ();
					button.Margins = new Margins (x, 0, 0, y);

					x += button.PreferredWidth + 2;
				}
				else
				{
					if (button.IsFocused)
					{
						this.ScrollList.Focus ();
					}

					button.Enable = false;
					button.Hide ();
				}
			}
		}

		private void UpdateVisibleButtons()
		{
			int selectionCount = (this.selection == null) ? 0 : this.selection.Count;

			if (this.selectionCount != selectionCount)
			{
				this.selectionCount = selectionCount;
				this.visibleQuickButtons.Clear ();

				if (selectionCount == 1)
				{
					this.visibleQuickButtons.Add (this.quickButtonAddNegative);
					this.visibleQuickButtons.Add (this.quickButtonAddPositive);
				}
				else if (selectionCount > 1)
				{
					this.visibleQuickButtons.Add (this.quickButtonSum);
				}
			}
		}

		private void HandleButton2Clicked(object sender, MessageEventArgs e)
		{
			this.HideQuickButtons ();
			this.selection = null;

			IEnumerable<int> selection = this.ScrollList.GetSortedSelection ();

			if (this.SumSeries != null)
			{
				this.SumSeries (selection);
			}
			
			this.ScrollList.Invalidate ();
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


		private IEnumerable<Button> QuickButtons
		{
			get
			{
				yield return this.quickButtonAddNegative;
				yield return this.quickButtonAddPositive;
				yield return this.quickButtonSum;
			}
		}

		
		public event EventHandler Changed;
		public System.Action<IEnumerable<int>> SumSeries;

		private int hotRowIndex;
		private MultiSelectEventArgs selection;
		private Rectangle quickButtonsFrame;
		private int selectionCount;

		readonly private Button quickButtonAddNegative;
		readonly private Button quickButtonAddPositive;
		readonly private Button quickButtonSum;
		readonly private List<Button> visibleQuickButtons;
	}
}
