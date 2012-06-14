//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	/// <summary>
	/// Ce widget affiche des onglets, ainsi que quelques widgets quelconques (généralement des boutons) à leurs
	/// droite. Les onglets peuvent être renommés, supprimés et déplacés (par drag & drop). S'il y a trop d'onglets
	/// par rapport à la largeur disponible, un dernier onglet "v" affiche un menu avec les onglets surnuméraires.
	/// </summary>
	public class TabsPane : FrameBox, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public TabsPane()
		{
			this.tabs          = new List<Tab> ();
			this.showedIndexes = new List<int> ();
			this.hiddenIndexes = new List<int> ();
			this.leftWidgets   = new List<Widget> ();
			this.rightWidgets  = new List<Widget> ();

			this.renameField = new TextField
			{
				Parent     = this,
				Anchor     = AnchorStyles.BottomLeft,
				Visibility = false,
			};

			this.selectedIndex  = -1;
			this.hilitedIndex   = -1;
			this.gapHilitedRank = -1;

			this.TabLookStyle   = TabLook.OneNote;
			this.selectionColor = Color.FromName ("White");
			this.IconSize       = 20;

			ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
		}


		public TabLook TabLookStyle
		{
			get
			{
				return this.tabLook;
			}
			set
			{
				if (this.tabLook != value)
				{
					this.tabLook = value;
					this.Invalidate ();
				}
			}
		}

		public Color SelectionColor
		{
			get
			{
				return this.selectionColor;
			}
			set
			{
				if (this.selectionColor != value)
				{
					this.selectionColor = value;
					this.Invalidate ();
				}
			}
		}

		public bool IsDragSource
		{
			get;
			set;
		}

		public double IconSize
		{
			get;
			set;
		}


		public void Clear()
		{
			this.tabs.Clear ();
			this.selectedIndex = -1;

			this.dirtyLayout = true;
			this.Invalidate ();
		}

		public int Count
		{
			get
			{
				return this.tabs.Count;
			}
		}

		public void Add(TabItem item)
		{
			this.Insert (this.tabs.Count, item);
		}

		public void Insert(int index, TabItem item)
		{
			var tab = new Tab (this.IconSize)
			{
				TabItem = item,
			};

			this.tabs.Insert (index, tab);

			this.dirtyLayout = true;
			this.Invalidate ();
		}

		public void RemoveAt(int index)
		{
			this.tabs.RemoveAt (index);

			this.dirtyLayout = true;
			this.Invalidate ();
		}


		public TabItem Get(int index)
		{
			return this.tabs[index].TabItem;
		}

		public void Set(int index, TabItem item)
		{
			this.tabs[index].TabItem = item;

			this.dirtyLayout = true;
			this.Invalidate ();
		}


		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}
			set
			{
				if (this.selectedIndex != value)
				{
					this.selectedIndex = value;

					this.dirtyLayout = true;
					this.Invalidate ();
				}
			}
		}


		public void ClearLeftWidgets()
		{
			//	Supprime tous les widgets additionnels à gauche des onglets.
			this.leftWidgets.Clear ();
			this.Children.Clear ();

			this.dirtyLayout = true;
			this.Invalidate ();
		}

		public void AddLeftWidget(Widget widget)
		{
			//	Ajoute un widget à gauche des onglets.
			this.leftWidgets.Add (widget);

			widget.Parent = this;
			widget.Anchor = AnchorStyles.TopLeft;

			this.dirtyLayout = true;
			this.Invalidate ();
		}


		public void ClearRightWidgets()
		{
			//	Supprime tous les widgets additionnels à droite des onglets.
			this.rightWidgets.Clear ();
			this.Children.Clear ();

			this.dirtyLayout = true;
			this.Invalidate ();
		}

		public void AddRightWidget(Widget widget)
		{
			//	Ajoute un widget à droite des onglets.
			this.rightWidgets.Add (widget);

			widget.Parent = this;
			widget.Anchor = AnchorStyles.TopRight;

			this.dirtyLayout = true;
			this.Invalidate ();
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.IsDragSource)
			{
				//	Le widget source d'un déplacement (placé dans une fenêtre flottante) doit être totalement inerte.
				return;
			}

			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					this.MouseMove (message, pos);
					break;

				case MessageType.MouseDown:
					this.MouseDown (message, pos);
					return;

				case MessageType.MouseUp:
					this.MouseUp (message, pos);
					break;

				case MessageType.MouseLeave:
					this.MouseLeave (message, pos);
					break;

				case MessageType.KeyDown:
					this.KeyDown (message, pos);
					break;
			}

			base.ProcessMessage (message, pos);
		}

		private void MouseMove(Message message, Point pos)
		{
			int index, rank;

			index = this.GetDetectedIndex (pos);

			if (this.isDragging)
			{
				rank = this.GetDetectedGapRank (pos);
				if (this.gapHilitedRank != rank)
				{
					this.gapHilitedRank = rank;
					this.Invalidate ();
				}

				this.DragMove ();
			}
			else
			{
				if (this.mouseDown && !this.isDragging && !this.fixedTab && Point.Distance (pos, this.draggingStartPos) >= 5)
				{
					index = this.GetDetectedIndex (pos);
					if (index != -1)
					{
						this.isDragging = true;
						this.dirtyLayout = true;
						this.draggingStartIndex = index;
						this.gapHilitedRank = this.GetDetectedGapRank (pos);
						this.Invalidate ();
						this.DragBegin ();
					}
				}
				else
				{
					if (this.hilitedIndex != index)
					{
						this.hilitedIndex = index;
						this.Invalidate ();
					}
				}
			}
		}

		private void MouseDown(Message message, Point pos)
		{
			int index;

			this.mouseDown = true;
			this.draggingStartPos = pos;

			this.fixedTab = true;
			index = this.GetDetectedIndex (pos);
			if (index != -1)
			{
				var tab = this.GetShowedTabFromIndex (index);
				if (tab != null && tab.TabItem.MoveVisibility)
				{
					this.fixedTab = false;
				}
			}

			if (this.isRename)
			{
				this.StopRename ();
			}

			message.Captured = true;
			message.Consumer = this;
		}

		private void MouseUp(Message message, Point pos)
		{
			int index;

			this.mouseDown = false;

			if (this.isDragging)
			{
				if (this.gapHilitedRank < this.showedIndexes.Count)
				{
					index = this.showedIndexes[this.gapHilitedRank];
				}
				else
				{
					index = this.showedIndexes.Count;
				}

				this.OnDraggingDoing (this.draggingStartIndex, index);

				this.isDragging = false;
				this.dirtyLayout = true;
				this.gapHilitedRank = -1;
				this.Invalidate ();
				this.DragEnd ();
			}
			else if (message.IsRightButton)
			{
				this.ShowContextMenu ();
			}
			else
			{
				index = this.GetDetectedIndex (pos);

				if (index == TabsPane.menuIndex)
				{
					this.ShowHiddenMenu ();
				}
				else
				{
					var tab = this.GetShowedTabFromIndex (this.selectedIndex);
					if (this.selectedIndex == index && tab != null && tab.TabItem.RenameEnable)  // clic sur l'onglet déjà sélectionné ?
					{
						this.menuTabIndex = this.selectedIndex;
						this.StartRename ();
					}
					else
					{
						this.selectedIndex = index;
						this.OnSelectedIndexChanged ();
						this.Invalidate ();
					}
				}
			}
		}

		private void MouseLeave(Message message, Point pos)
		{
			if (!this.isDragging)
			{
				this.mouseDown = false;
				this.hilitedIndex = -1;
				this.isDragging = false;
				this.dirtyLayout = true;
				this.gapHilitedRank = -1;
				this.Invalidate ();
				this.DragEnd ();
			}
		}

		private void KeyDown(Message message, Point pos)
		{
			if (message.KeyCode == KeyCode.Return)
			{
				if (this.isRename)
				{
					this.AcceptRename ();
				}
			}

			if (message.KeyCode == KeyCode.Escape)
			{
				if (this.isRename)
				{
					this.StopRename ();
				}
			}
		}

		private int GetDetectedIndex(Point pos)
		{
			//	Retourne l'index de l'onglet visé, ou -1.
			this.UpdateIndexes ();

			foreach (var rank in this.RanksForDetection)
			{
				var rect = this.GetTextRect (rank);

				if (!rect.IsEmpty)
				{
					var path = this.GetTabPath (rect);

					if (path.SurfaceContainsPoint (pos.X, pos.Y, 1))
					{
						var index = this.showedIndexes[rank];
						return index;
					}
				}
			}

			return -1;
		}

		private int GetDetectedGapRank(Point pos)
		{
			//	Retourne le rang (à ne pas confondre avec l'index) de l'espace inter-onglet visé.
			//	Si on est entre les onglets 12 et 13, retourne 12.
			for (int rank = 0; rank < this.showedIndexes.Count; rank++ )
			{
				var rect = this.GetTextRect (rank);

				if (!rect.IsEmpty)
				{
					if (pos.X < rect.Center.X)
					{
						return rank;
					}
				}
			}

			return this.showedIndexes.Count;
		}

		private void DragBegin()
		{
			this.DragEnd ();

			var tab = this.GetShowedTabFromIndex (this.draggingStartIndex);
			this.dragInfo = new DragInfo (tab, this);

			this.ShowHideAdditionnalWidgets (false);
			this.Invalidate ();

			this.DragMove ();
		}

		private void DragMove()
		{
			if (this.dragInfo != null)
			{
				var pos = new Point (this.GapMarkPos, this.ActualHeight-1);
				this.dragInfo.Update (this.MapClientToScreen (pos), this.IsNopDrag);
			}
		}

		private void DragEnd()
		{
			if (this.dragInfo != null)
			{
				this.dragInfo.Dispose ();
				this.dragInfo = null;

				this.ShowHideAdditionnalWidgets (true);
				this.Invalidate ();
			}
		}

		private void ShowHideAdditionnalWidgets(bool show)
		{
			foreach (var widget in this.leftWidgets)
			{
				widget.Visibility = show;
			}

			foreach (var widget in this.rightWidgets)
			{
				widget.Visibility = show;
			}

			this.dirtyLayout = true;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);
			this.PaintTabs (graphics);
			this.PaintGapMark (graphics);
		}

		private void PaintTabs(Graphics graphics)
		{
			//	Dessine tous les onglets.
			this.UpdateIndexes ();

			foreach (var rank in this.RanksForDetection.Reverse ())
			{
				this.PaintTab (graphics, rank);
			}
		}

		private void PaintTab(Graphics graphics, int rank)
		{
			//	Dessine un onglet.
			var index = this.showedIndexes[rank];
			var rect = this.GetTextRect (rank);

			var state = TabState.Normal;

			if (index == this.hilitedIndex)
			{
				state = TabState.Hilited;
			}

			if (index == this.selectedIndex)
			{
				state = TabState.Selected;
			}

			if (this.menuOpened && index == this.menuTabIndex)
			{
				state = TabState.MenuOpened;
			}

			if (this.isDragging && index == this.draggingStartIndex)
			{
				state = TabState.StartDragging;
			}

			if (this.IsDragSource)
			{
				state = TabState.Floating;
			}

			if (!rect.IsEmpty)
			{
				if (index == TabsPane.menuIndex)  // onglet 'v' du menu ?
				{
					this.PaintTabMenu (graphics, rect, state);
				}
				else
				{
					//	Dessine le cadre et le fond.
					this.PaintTabFrame (graphics, rect, state, this.GetBackColor (index));

					//	Dessine le texte.
					var color = Color.FromBrightness (0);

					if (state == TabState.StartDragging)
					{
						if (this.IsNopDrag)
						{
							color = Color.FromAlphaColor (0.2, color);
						}
						else
						{
							color = Color.FromAlphaColor (0.0, color);
						}
					}

					rect.Inflate (1, -2);

#if false
					if (state == TabState.Selected || state == TabState.MenuOpened)
					{
						rect.Offset (0, -2);
					}
#endif

					var pos = rect.BottomLeft;
					var tab = this.GetShowedTabFromRank (rank);
					tab.UpdateTextLayout (state == TabState.Selected || state == TabState.Hilited);
					tab.TextLayout.LayoutSize = rect.Size;
					tab.TextLayout.DefaultColor = color;
					tab.TextLayout.Paint (pos, graphics);
				}
			}
		}

		private void PaintTabMenu(Graphics graphics, Rectangle rect, TabState state)
		{
			//	Dessine l'onglet 'v' pour le menu.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			this.PaintTabFrame (graphics, rect, state, Color.Empty);

			//	Dessine le triangle 'v'.
			var c = rect.Center;
			var d = rect.Height*0.2;

			var path = new Path ();
			path.MoveTo (c.X, c.Y-d);
			path.LineTo (c.X-d, c.Y);
			path.LineTo (c.X+d, c.Y);
			path.Close ();

			graphics.AddFilledPath (path);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		private void PaintTabFrame(Graphics graphics, Rectangle rect, TabState state, Color backColor)
		{
			//	Dessine le cadre et le fond d'un onglet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			bool selected = (state == TabState.Selected || state == TabState.MenuOpened);
			var path = this.GetTabPath (rect, selected);
			var colorFrame = adorner.ColorBorder;

			//	Dessine le fond.
			if (state == TabState.Hilited)
			{
				if (backColor.IsEmpty)
				{
					backColor = Color.FromBrightness (1.0);
				}

				if (this.tabLook == TabLook.Simple)
				{
					graphics.AddFilledPath (path);
					graphics.RenderSolid (backColor);
				}
				else
				{
					graphics.AddFilledPath (path);
					graphics.RenderSolid (adorner.ColorBorder);

					var p = this.GetTabPath (rect, new Margins (1.5, 1.5, 1.5, 0), selected);
					graphics.AddFilledPath (p);
					graphics.RenderSolid (backColor);
				}
			}
			else if (state == TabState.Selected)
			{
				if (backColor.IsEmpty)
				{
					graphics.AddFilledPath (path);
					graphics.RenderSolid (this.selectionColor);
				}
				else
				{
					graphics.AddFilledPath (path);
					graphics.RenderSolid (backColor);
				}
			}
			else if (state == TabState.MenuOpened)
			{
				if (backColor.IsEmpty)
				{
					backColor = Color.FromBrightness (1.0);
				}

				graphics.AddFilledPath (path);
				graphics.RenderSolid (backColor);
			}
			else if (state == TabState.StartDragging)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromAlphaColor (0.4, Color.FromBrightness (1.0)));

				colorFrame = Color.FromAlphaColor (0.5, colorFrame);
			}
			else if (state == TabState.Floating)
			{
				if (backColor.IsEmpty)
				{
					backColor = Color.FromBrightness (1.0);
				}

				graphics.AddFilledPath (path);
				graphics.RenderSolid (backColor);
			}
			else
			{
				if (this.tabLook == TabLook.Simple)
				{
					return;
				}

				if (backColor.IsEmpty)
				{
					var pc = this.Parent.BackColor;
					backColor = Color.FromHsv (pc.Hue, pc.Saturation, pc.Value*0.95);
				}
				else
				{
					backColor = Color.FromHsv (backColor.Hue, backColor.Saturation*0.5, backColor.Value-0.1);
				}

				graphics.AddFilledPath (path);
				graphics.RenderSolid (backColor);
			}

			//	Dessine le cadre.
			graphics.AddPath (path);
			graphics.RenderSolid (colorFrame);
		}

		private Path GetTabPath(Rectangle rect, bool selected = false)
		{
			return this.GetTabPath (rect, Margins.Zero, selected);
		}

		private Path GetTabPath(Rectangle rect, Margins margins, bool selected = false)
		{
			//	Retourne le chemin pour dessiner/détecter un onglet.
			var path = new Path ();

			rect.Deflate (margins);
			rect.Deflate (0.5);

			if (this.tabLook == TabLook.Trapeze)
			{
				var p1 = new Point (rect.Left-TabsPane.tabMargin, rect.Bottom);
				var p2 = new Point (rect.Left, rect.Top);
				var p3 = new Point (rect.Right, rect.Top);
				var p4 = new Point (rect.Right+TabsPane.tabMargin, rect.Bottom);

				double d = rect.Height * 0.25;

				var p21 = Point.Move (p2, p1, d);
				var p23 = Point.Move (p2, p3, d);
				var p32 = Point.Move (p3, p2, d);
				var p34 = Point.Move (p3, p4, d);

				if (p23.X < p32.X)
				{
					path.MoveTo (p1);
					path.LineTo (p21);
					path.CurveTo (p2, p23);
					path.LineTo (p32);
					path.CurveTo (p3, p34);
					path.LineTo (p4);
				}
				else
				{
					path.MoveTo (p1);
					path.LineTo (p2);
					path.LineTo (p3);
					path.LineTo (p4);
				}
			}
			else if (this.tabLook == TabLook.OneNote)
			{
				var p1 = new Point (rect.Left-TabsPane.tabMargin*0.5, rect.Bottom);
				var p2 = new Point (rect.Left-TabsPane.tabMargin*0.5, rect.Top);
				var p3 = new Point (rect.Right-TabsPane.tabMargin*0.5, rect.Top);
				var p4 = new Point (rect.Right+TabsPane.tabMargin*1.8, rect.Bottom);

				double d = rect.Height * 0.2;

				var p21 = Point.Move (p2, p1, d);
				var p23 = Point.Move (p2, p3, d);

				if (selected)
				{
					var bounds = this.Client.Bounds;

					path.MoveTo (new Point (bounds.Left+0.5, bounds.Bottom));
					path.LineTo (new Point (bounds.Left+0.5, p1.Y));

					path.LineTo (p1);
					path.LineTo (p21);
					path.CurveTo (p2, p23);
					path.LineTo (p3);
					path.LineTo (p4);

					path.LineTo (new Point (bounds.Right-0.5, p4.Y));
					path.LineTo (new Point (bounds.Right-0.5, bounds.Bottom));
				}
				else
				{
					path.MoveTo (p1);
					path.LineTo (p21);
					path.CurveTo (p2, p23);
					path.LineTo (p3);
					path.LineTo (p4);
				}
			}
			else
			{
				double x1 = System.Math.Floor (rect.Left -TabsPane.tabMargin*0.7) - 0.5;
				double x2 = System.Math.Floor (rect.Right+TabsPane.tabMargin*0.7) + 0.5;

				var p1 = new Point (x1, rect.Bottom);
				var p2 = new Point (x1, rect.Top);
				var p3 = new Point (x2, rect.Top);
				var p4 = new Point (x2, rect.Bottom);

				double d = rect.Height * 0.15;

				var p21 = Point.Move (p2, p1, d);
				var p23 = Point.Move (p2, p3, d);
				var p32 = Point.Move (p3, p2, d);
				var p34 = Point.Move (p3, p4, d);

				if (selected)
				{
					var bounds = this.Client.Bounds;

					path.MoveTo (new Point (bounds.Left+0.5, bounds.Bottom));
					path.LineTo (new Point (bounds.Left+0.5, p1.Y));

					path.LineTo (p1);
					path.LineTo (p21);
					path.CurveTo (p2, p23);
					path.LineTo (p32);
					path.CurveTo (p3, p34);
					path.LineTo (p4);

					path.LineTo (new Point (bounds.Right-0.5, p4.Y));
					path.LineTo (new Point (bounds.Right-0.5, bounds.Bottom));
				}
				else
				{
					path.MoveTo (p1);
					path.LineTo (p21);
					path.CurveTo (p2, p23);
					path.LineTo (p32);
					path.CurveTo (p3, p34);
					path.LineTo (p4);
				}
			}

			if (this.IsDragSource)
			{
				path.Close ();  // on dessine le trait horizontal inférieur
			}

			return path;
		}


		private void PaintGapMark(Graphics graphics)
		{
			//	Dessine le marqueur entre deux onglets, pendant un déplacement d'onglet.
			if (this.gapHilitedRank == -1)
			{
				return;
			}

			var bounds = this.Client.Bounds;

			if (this.IsNopDrag)
			{
				var c = new Point (this.GapMarkPos, bounds.Center.Y);
				var d = 11;
				var h = 8;
				var m = 2;

				var path = new Path ();

				path.MoveTo (c.X-m-d, c.Y);
				path.LineTo (c.X-m, c.Y+h);
				path.LineTo (c.X-m, c.Y-h);
				path.Close ();

				path.MoveTo (c.X+m+d, c.Y);
				path.LineTo (c.X+m, c.Y+h);
				path.LineTo (c.X+m, c.Y-h);
				path.Close ();

				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
			else
			{
				var c = new Point (this.GapMarkPos, bounds.Top);
				var d = 11;
				var l = (this.gapHilitedRank == 0) ? 0 : d;

				var path = new Path ();
				path.MoveTo (c.X, c.Y-d-4);
				path.LineTo (c.X-l, c.Y);
				path.LineTo (c.X+d, c.Y);
				path.Close ();

				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}

		private double GapMarkPos
		{
			get
			{
				if (this.IsNopDrag)
				{
					int rank = this.showedIndexes.IndexOf (this.draggingStartIndex);
					var rect = this.GetTextRect (rank);
					return rect.Center.X;
				}
				else if (this.gapHilitedRank < this.showedIndexes.Count)
				{
					var rect = this.GetTextRect (this.gapHilitedRank);
					return rect.Left - TabsPane.tabMargin*0.5 + ((this.gapHilitedRank == 0) ? 0 : 1);
				}
				else
				{
					var rect = this.GetTextRect (this.gapHilitedRank-1);
					return rect.Right + TabsPane.tabMargin*0.5 + 1;
				}
			}
		}

		private bool IsNopDrag
		{
			get
			{
				int rank = this.showedIndexes.IndexOf (this.draggingStartIndex);
				return this.gapHilitedRank == rank || this.gapHilitedRank == rank+1;
			}
		}

		private Color GetBackColor(int index)
		{
			switch (this.tabs[index].TabItem.Color)
			{
				case TabColor.Red:
					return UIBuilder.TabsPaneRedBack;

				case TabColor.Green:
					return UIBuilder.TabsPaneGreenBack;

				case TabColor.Blue:
					return UIBuilder.TabsPaneBlueBack;

				default:
					return Color.Empty;
			}
		}


		private IEnumerable<int> RanksForDetection
		{
			//	Retourne les rangs dans l'ordre pour la détection.
			//	Il faut utiliser l'ordre inverse pour le dessin.
			get
			{
				int menuIndex = -1;

				//	Retourne toujours l'onglet 'v' du menu en premier, si le menu est ouvert.
				if (this.menuOpened)
				{
					yield return this.showedIndexes.IndexOf (this.menuTabIndex);
				}

				//	Retourne toujours l'onglet sélectionné en premier.
				if (this.selectedIndex != -1)
				{
					yield return this.showedIndexes.IndexOf (this.selectedIndex);
				}

				//	Retourne ensuite tous les autres onglets.
				for (int rank = 0; rank < this.showedIndexes.Count; rank++)
				{
					int index = this.showedIndexes[rank];

					if (index != this.selectedIndex && index != menuIndex)
					{
						yield return rank;
					}
				}
			}
		}

		private Rectangle GetTextRect(int rank)
		{
			//	Retourne le rectangle du texte d'un onglet. Le cadre de l'onglet déborde ce rectangle.
			double x = this.LeftWidgetsWidth + TabsPane.tabMargin;

			for (int r = 0; r < rank; r++)
			{
				x += this.GetShowedWidth (r);
				x += TabsPane.tabMargin;
			}

			if (this.IsDragSource)
			{
				return new Rectangle (x, 0, this.GetShowedWidth (rank), this.ActualHeight-2);
			}
			else
			{
				return new Rectangle (x, 2, this.GetShowedWidth (rank), this.ActualHeight-2);
			}
		}

		private double GetShowedWidth(int rank)
		{
			//	Retourne la largeur du texte contenu dans un onglet.
			var tab = this.GetShowedTabFromRank (rank);

			if (tab == null)
			{
				return TabsPane.menuWidth;
			}
			else
			{
				return tab.CurrentWidth;
			}
		}

		private Tab GetShowedTabFromIndex(int index)
		{
			int rank = this.showedIndexes.IndexOf (index);
			return this.GetShowedTabFromRank (rank);
		}

		private Tab GetShowedTabFromRank(int rank)
		{
			//	Retourne le contenu d'un onglet. L'onglet 'v' du menu retourne null.
			if (rank >= 0 && rank < this.showedIndexes.Count)
			{
				int index = this.showedIndexes[rank];

				if (index >= 0 && index < this.tabs.Count)
				{
					return this.tabs[index];
				}
			}

			return null;
		}


		private void UpdateIndexes()
		{
			//	Met à jour les index devant être dessinés et ceux qui sont cachés, dans l'ordre de visibilité
			//	de gauche à droite. A ne pas confondre avec l'ordre de détection/dessin.
			if (!this.dirtyLayout && this.lastWidth == this.ActualWidth)
			{
				return;
			}

			this.dirtyLayout = false;
			this.lastWidth = this.ActualWidth;

			if (this.isDragging)
			{
				this.UpdateIndexesStretch ();
			}
			else
			{
				this.UpdateIndexesNormal ();
			}

			//	Met à jour la liste des index invisibles.
			this.hiddenIndexes.Clear ();

			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (!this.showedIndexes.Contains (i))
				{
					this.hiddenIndexes.Add (i);
				}
			}
		}

		private void UpdateIndexesNormal()
		{
			this.showedIndexes.Clear ();

			if (this.tabs.Count == 0)
			{
				return;
			}

			//	Initialise les largeurs courantes des onglets, en diminuant la largeur si l'onglet est
			//	vraiment trop grand.
			int n = this.tabs.Count;
			n = System.Math.Max (n, 1);
			n = System.Math.Min (n, 4);

			double actualWidth = this.ActualWidth - this.LeftWidgetsWidth - this.RightWidgetsWidth;
			double max = System.Math.Floor (actualWidth/n);

			foreach (var tab in this.tabs)
			{
				tab.CurrentWidth = System.Math.Min (tab.TextWidth, max);
			}

			//	Génère la table des positions droites des onglets.
			var rigths = new List<double> ();
			double x = TabsPane.tabMargin;
			for (int i = 0; i < this.tabs.Count; i++)
			{
				var tab = this.tabs[i];
				x += tab.CurrentWidth;
				x += TabsPane.tabMargin;

				rigths.Add (x);
			}

			if (rigths[this.tabs.Count-1] <= actualWidth-TabsPane.tabMargin*1.8)  // assez de place ?
			{
				//	Cas où on a assez de place pour tout mettre normalement.
				for (int i = 0; i < tabs.Count; i++)
				{
					this.showedIndexes.Add (i);
				}
			}
			else
			{
				max = actualWidth-TabsPane.menuWidth-TabsPane.tabMargin*2.8;

				if (this.selectedIndex == -1 || rigths[this.selectedIndex] <= max)
				{
					//	Cas où il manque de la place, mais où l'onglet sélectionné est visible normalement.
					for (int i = 0; i < this.tabs.Count; i++)
					{
						if (rigths[i] > max)
						{
							break;
						}

						this.showedIndexes.Add (i);
					}
				}
				else
				{
					//	Cas où il manque de la place et où l'onglet sélectionné est invisible normalement.
					max -= this.tabs[this.selectedIndex].CurrentWidth;

					for (int i = 0; i < this.tabs.Count; i++)
					{
						if (i != this.selectedIndex)
						{
							if (rigths[i] > max)
							{
								break;
							}

							this.showedIndexes.Add (i);
						}
					}

					//	L'onglet sélectionné vient en dernier à droite.
					this.showedIndexes.Add (this.selectedIndex);
				}

				//	On ajoute l'index de l'onglet 'v' du menu.
				this.showedIndexes.Add (TabsPane.menuIndex);
			}

			//	Garde-fou. Si l'onglet sélectionné n'y est pas, on l'ajoute !
			if (this.selectedIndex != -1)
			{
				if (!this.showedIndexes.Contains (this.selectedIndex))
				{
					this.showedIndexes.Add (this.selectedIndex);
				}
			}

			this.UpdateLeftWidgetsLayout ();
			this.UpdateRightWidgetsLayout ();
		}

		private void UpdateIndexesStretch()
		{
			this.showedIndexes.Clear ();

			double actualWidth = this.ActualWidth - this.LeftWidgetsWidth - this.RightWidgetsWidth;
			double totalWidth = 0;

			foreach (var tab in this.tabs)
			{
				if (tab.TabItem.MoveVisibility)
				{
					totalWidth += tab.TextWidth + TabsPane.tabMargin*2;
				}
			}

			for (int i = 0; i < this.tabs.Count; i++)
			{
				var tab = this.tabs[i];

				if (tab.TabItem.MoveVisibility)
				{
					if (totalWidth > actualWidth)
					{
						tab.CurrentWidth = System.Math.Floor (tab.TextWidth * actualWidth / totalWidth);
					}
					else
					{
						tab.CurrentWidth = tab.TextWidth;
					}

					this.showedIndexes.Add (i);
				}
			}
		}


		private double LeftWidgetsWidth
		{
			//	Retourne la largeur de l'ensemble des widgets additionnels de gauche.
			get
			{
				double width = 0;

				foreach (var widget in this.leftWidgets)
				{
					if (widget.Visibility)
					{
						width += widget.ActualWidth;
					}
				}

				return width;
			}
		}

		private double RightWidgetsWidth
		{
			//	Retourne la largeur de l'ensemble des widgets additionnels de droite.
			get
			{
				double width = 0;

				foreach (var widget in this.rightWidgets)
				{
					if (widget.Visibility)
					{
						width += widget.ActualWidth;
					}
				}

				return width;
			}
		}

		private void UpdateLeftWidgetsLayout()
		{
			//	Met à jour les positions des widgets additionnels de gauche.
			double x = 0;

			foreach (var widget in this.rightWidgets)
			{
				widget.Margins = new Margins (x, 0, 0, 0);
				x += widget.PreferredWidth;
			}
		}

		private void UpdateRightWidgetsLayout()
		{
			//	Met à jour les positions des widgets additionnels de droite.
			double x = 0;

			foreach (var widget in this.rightWidgets)
			{
				widget.Margins = new Margins (0, x, 0, 0);
				x += widget.PreferredWidth;
			}
		}


		private void StartRename()
		{
			if (this.menuTabIndex != -1)
			{
				int rank = this.showedIndexes.IndexOf (this.menuTabIndex);
				if (rank != -1)
				{
					var tab = this.GetShowedTabFromRank (rank);
					if (tab != null)
					{
						var rect = this.GetTextRect (rank);
						double h = 20;
						double y = System.Math.Floor ((this.ActualHeight-h)/2);

						this.renameField.Margins = new Margins (rect.Left, 0, 0, y);
						this.renameField.PreferredSize = new Size (rect.Width+6, h);
						this.renameField.Visibility = true;
						this.renameField.FormattedText = tab.TabItem.FormattedText;

						this.isRename = true;

						Application.QueueAsyncCallback
						(
							delegate
							{
								this.renameField.SelectAll ();
								this.renameField.Focus ();
							}
						);
					}
				}
			}
		}

		private void AcceptRename()
		{
			this.OnRenameDoing (this.menuTabIndex, this.renameField.FormattedText);
			this.StopRename ();
		}

		private void StopRename()
		{
			this.isRename = false;
			this.renameField.Visibility = false;
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			int index = this.GetDetectedIndex (pos);
			if (index != -1)
			{
				var tab = this.GetShowedTabFromIndex (index);
				if (tab != null)
				{
					if (!tab.TabItem.Tooltip.IsNullOrEmpty)  // existe-t-il un tooltip spécifique ?
					{
						return tab.TabItem.Tooltip;
					}

					if (tab.TextWidth > tab.CurrentWidth)  // manque-t-il de la place pour le texte ?
					{
						return tab.TabItem.FormattedText;
					}
				}
			}

			return null;
		}
		#endregion


		#region Menus
		private void ShowHiddenMenu()
		{
			//	Affiche le menu sous l'onglet 'v', qui permet d'accèder aux onglets cachés.
			var menu = new VMenu ();

			foreach (var index in this.hiddenIndexes)
			{
				var text = this.tabs[index].TabItem.FormattedText;
				if (text.IsNullOrEmpty)
				{
					text = this.tabs[index].TabItem.Tooltip;
				}

				var item = new MenuItem
				{
					IconUri       = UIBuilder.GetResourceIconUri (this.tabs[index].TabItem.Icon),
					FormattedText = text,
					TabIndex      = index,  // il est étrange d'utiliser TabIndex, mais cela fonctionne !
				};

				menu.Items.Add (item);

				item.Clicked += delegate
				{
					this.selectedIndex = item.TabIndex;
					this.OnSelectedIndexChanged ();
				};
			}

			this.menuOpened = true;
			this.menuTabIndex = TabsPane.menuIndex;
			this.Invalidate ();

			menu.Accepted += delegate
			{
				this.menuOpened = false;
			};

			menu.Rejected += delegate
			{
				this.menuOpened = false;
			};

			var x = this.GetTextRect (this.showedIndexes.IndexOf (TabsPane.menuIndex)).Left - TabsPane.tabMargin*0.5;

			menu.AutoDispose = true;
			menu.ShowAsComboList (this, this.MapClientToScreen (new Point (x, 0)), this);
		}

		private void ShowContextMenu()
		{
			var tab = this.GetShowedTabFromIndex (this.hilitedIndex);
			if (tab == null)
			{
				return;
			}

			var menu = new VMenu ();

			if (tab.TabItem.RenameVisibility)
			{
				this.AddToContextMenu (menu, "Edit.Rename", "Renommer", tab.TabItem.RenameEnable, delegate
				{
					this.StartRename ();
				});
			}

			if (tab.TabItem.DuplicateVisibility)
			{
				this.AddToContextMenu (menu, "Edit.Duplicate", "Dupliquer", tab.TabItem.DuplicateEnable, delegate
				{
					this.OnDuplicateDoing (this.menuTabIndex);
				});
			}

			if (tab.TabItem.DeleteVisibility)
			{
				this.AddToContextMenu (menu, "Edit.Delete", "Supprimer", tab.TabItem.DeleteEnable, delegate
				{
					this.OnDeleteDoing (this.menuTabIndex);
				});
			}

			if (tab.TabItem.ReloadVisibility || tab.TabItem.SaveVisibility)
			{
				menu.Items.Add (new MenuSeparator ());

				if (tab.TabItem.ReloadVisibility)
				{
					this.AddToContextMenu (menu, "ViewSettings.Reload", "Réutiliser les réglages initiaux", tab.TabItem.ReloadEnable, delegate
					{
						this.OnReloadDoing (this.menuTabIndex);
					});
				}

				if (tab.TabItem.SaveVisibility)
				{
					this.AddToContextMenu (menu, "ViewSettings.Save", "Enregistrer les réglages actuels", tab.TabItem.SaveEnable, delegate
					{
						this.OnSaveDoing (this.menuTabIndex);
					});
				}
			}

			if (tab.TabItem.ColorVisibility)
			{
				menu.Items.Add (new MenuSeparator ());
				this.AddColorToContextMenu (menu, tab.TabItem, TabColor.None);
				this.AddColorToContextMenu (menu, tab.TabItem, TabColor.Red);
				this.AddColorToContextMenu (menu, tab.TabItem, TabColor.Green);
				this.AddColorToContextMenu (menu, tab.TabItem, TabColor.Blue);
			}

			if (tab.TabItem.MoveVisibility)
			{
				menu.Items.Add (new MenuSeparator ());

				this.AddToContextMenu (menu, "Edit.Tab.Begin", "Déplacer en tête", tab.TabItem.MoveBeginEnable, delegate
				{
					this.OnDraggingDoing (this.selectedIndex, this.BeginIndex);
				});

				this.AddToContextMenu (menu, "Edit.Tab.End", "Déplacer en queue", tab.TabItem.MoveEndEnable, delegate
				{
					this.OnDraggingDoing (this.selectedIndex, this.EndIndex);
				});
			}

			if (!menu.Items.Any ())
			{
				return;
			}

			this.menuOpened = true;
			this.menuTabIndex = this.hilitedIndex;
			this.Invalidate ();

			menu.Accepted += delegate
			{
				this.menuOpened = false;
			};

			menu.Rejected += delegate
			{
				this.menuOpened = false;
			};

			var x = this.GetTextRect (this.showedIndexes.IndexOf (this.hilitedIndex)).Left - TabsPane.tabMargin*0.5;

			menu.AutoDispose = true;
			menu.ShowAsComboList (this, this.MapClientToScreen (new Point (x, 0)), this);
		}

		private void AddColorToContextMenu(VMenu menu, TabItem item, TabColor color)
		{
			string icon = UIBuilder.GetRadioStateIconUri (item.Color == color);
			FormattedText text;

			switch (color)
			{
				case TabColor.Red:
					text = "Rouge";
					break;

				case TabColor.Green:
					text = "Vert";
					break;

				case TabColor.Blue:
					text = "Bleu";
					break;

				default:
					text = "Normal";
					break;
			}

			this.AddToContextMenu (menu, icon, text, true, delegate
			{
				this.OnColorChanging (this.menuTabIndex, color);
			});
		}

		private void AddToContextMenu(VMenu menu, string icon, FormattedText text, bool enable, System.Action action)
		{
			var item = new MenuItem
			{
				IconUri       = UIBuilder.GetResourceIconUri (icon),
				FormattedText = text,
				Enable        = enable,
			};

			item.Clicked += delegate
			{
				action ();
			};

			menu.Items.Add (item);
		}

		private int BeginIndex
		{
			get
			{
				for (int i = 0; i < this.tabs.Count; i++)
				{
					var tab = this.tabs[i];
					if (tab.TabItem.MoveVisibility)
					{
						return i;
					}
				}

				return this.selectedIndex;
			}
		}

		private int EndIndex
		{
			get
			{
				for (int i = this.tabs.Count-1; i >= 0; i--)
				{
					var tab = this.tabs[i];
					if (tab.TabItem.MoveVisibility)
					{
						return i+1;
					}
				}

				return this.selectedIndex;
			}
		}
		#endregion


		private class DragInfo : System.IDisposable
		{
			public DragInfo(Tab tab, Widget parent)
			{
				var floatingItem = new TabItem
				{
					Icon          = tab.TabItem.Icon,
					FormattedText = tab.TabItem.FormattedText,
					Color         = tab.TabItem.Color,
				};

				double width = tab.TextWidth + TabsPane.tabMargin*2;

				var floatingTabPane = new TabsPane
				{
					IsDragSource    = true,
					PreferredWidth  = width + 20,
					PreferredHeight = parent.ActualHeight,
				};

				floatingTabPane.Add (floatingItem);

				this.origin = new Point (-width/2, 0);

				this.window = new DragWindow ();
				this.window.Alpha = 1.0;
				this.window.DefineWidget (floatingTabPane, floatingTabPane.PreferredSize, Margins.Zero);
				this.window.Owner = parent.Window;
				this.window.FocusWidget (floatingTabPane);
				this.window.Show ();
			}

			public void Update(Point pos, bool isNopDrag)
			{
				this.window.WindowLocation = this.origin + pos;
				this.window.Alpha = isNopDrag ? 0.0 : 1.0;
			}

			public void DissolveAndDispose()
			{
				if (this.window != null)
				{
					this.window.DissolveAndDisposeWindow ();
					this.window = null;
				}

				this.Dispose ();
			}

			#region IDisposable Members
			public void Dispose()
			{
				if (this.window != null)
				{
					this.window.Hide ();
					this.window.Dispose ();
				}

				this.window = null;
			}
			#endregion

			private DragWindow		window;
			private Point			origin;
		}

	
		private class Tab
		{
			public Tab(double iconSize)
			{
				this.IconSize = iconSize;

				this.textLayout = new TextLayout
				{
					Alignment = ContentAlignment.MiddleCenter,
					BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				};
			}

			public double IconSize
			{
				get;
				private set;
			}

			public TextLayout TextLayout
			{
				get
				{
					return this.textLayout;
				}
			}

			public void UpdateTextLayout(bool selected)
			{
				string style = selected ? "Active" : null;

				if (string.IsNullOrEmpty (this.tabItem.Icon))
				{
					this.textLayout.FormattedText = this.tabItem.FormattedText;
				}
				else if (this.tabItem.FormattedText.IsNullOrEmpty)
				{
					this.textLayout.FormattedText = UIBuilder.GetIconTag (this.tabItem.Icon, iconSize: this.IconSize, style: style);
				}
				else
				{
					this.textLayout.FormattedText = UIBuilder.GetIconTag (this.tabItem.Icon, iconSize: this.IconSize, style: style) + " " + this.tabItem.FormattedText;
				}

				this.textWidth = this.textLayout.GetSingleLineSize ().Width;
			}

			public TabItem TabItem
			{
				get
				{
					return this.tabItem;
				}
				set
				{
					this.tabItem = value;
					this.UpdateTextLayout (false);
				}
			}

			public double TextWidth
			{
				//	Largeur requise pour afficher la totalité du texte.
				get
				{
					return System.Math.Floor (this.textWidth + TabsPane.textMargin*2) + 1;
				}
			}

			public double CurrentWidth
			{
				//	Largeur utilisée pour l'onglet.
				get;
				set;
			}

			private readonly TextLayout			textLayout;
			private TabItem						tabItem;
			private double						textWidth;
		}


		private enum TabState
		{
			Normal,				// onglet non sélectionné
			Selected,			// onglet sélectionné
			Hilited,			// onglet survolé par la souris
			MenuOpened,			// onglet 'v' du menu, avec le menu ouvert
			StartDragging,		// onglet en cours de déplacement
			Floating,			// onglet déplacé, dans la fenêtre flottante
		}


		private void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged (this);
			}
		}

		public event EventHandler SelectedIndexChanged;


		private void OnRenameDoing(int index, FormattedText text)
		{
			if (this.RenameDoing != null)
			{
				this.RenameDoing (this, index, text);
			}
		}

		public delegate void RenameEventHandler(object sender, int index, FormattedText text);
		public event RenameEventHandler RenameDoing;


		private void OnDuplicateDoing(int index)
		{
			if (this.DuplicateDoing != null)
			{
				this.DuplicateDoing (this, index);
			}
		}

		public delegate void DuplicateEventHandler(object sender, int index);
		public event DuplicateEventHandler DuplicateDoing;


		private void OnDeleteDoing(int index)
		{
			if (this.DeleteDoing != null)
			{
				this.DeleteDoing (this, index);
			}
		}

		public delegate void DeleteEventHandler(object sender, int index);
		public event DeleteEventHandler DeleteDoing;


		private void OnReloadDoing(int index)
		{
			if (this.ReloadDoing != null)
			{
				this.ReloadDoing (this, index);
			}
		}

		public delegate void ReloadEventHandler(object sender, int index);
		public event ReloadEventHandler ReloadDoing;


		private void OnSaveDoing(int index)
		{
			if (this.SaveDoing != null)
			{
				this.SaveDoing (this, index);
			}
		}

		public delegate void SaveEventHandler(object sender, int index);
		public event SaveEventHandler SaveDoing;


		private void OnDraggingDoing(int srcIndex, int dstIndex)
		{
			if (this.DraggingDoing != null)
			{
				this.DraggingDoing (this, srcIndex, dstIndex);
			}
		}

		public delegate void DraggingEventHandler(object sender, int srcIndex, int dstIndex);
		public event DraggingEventHandler DraggingDoing;


		private void OnColorChanging(int index, TabColor color)
		{
			if (this.ColorChanging != null)
			{
				this.ColorChanging (this, index, color);
			}
		}

		public delegate void ColorEventHandler(object sender, int index, TabColor color);
		public event ColorEventHandler ColorChanging;


		private static readonly double				tabMargin  = 8;
		private static readonly double				textMargin = 2;
		private static readonly double				menuWidth  = 20;
		private static readonly int					menuIndex  = 999;

		private readonly List<Tab>					tabs;
		private readonly List<int>					showedIndexes;
		private readonly List<int>					hiddenIndexes;
		private readonly List<Widget>				leftWidgets;
		private readonly List<Widget>				rightWidgets;
		private readonly TextField					renameField;

		private TabLook								tabLook;
		private Color								selectionColor;
		private int									selectedIndex;
		private int									hilitedIndex;
		private int									menuTabIndex;
		private int									draggingStartIndex;
		private Point								draggingStartPos;
		private int									gapHilitedRank;
		private bool								mouseDown;
		private bool								menuOpened;
		private bool								isDragging;
		private bool								isRename;
		private bool								fixedTab;
		private bool								dirtyLayout;
		private double								lastWidth;
		private DragInfo							dragInfo;
	}
}
