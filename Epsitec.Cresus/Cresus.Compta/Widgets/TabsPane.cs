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
			this.tabs               = new List<Tab> ();
			this.showedIndexes      = new List<int> ();
			this.hiddenIndexes      = new List<int> ();
			this.additionnalWidgets = new List<Widget> ();

			this.renameField = new TextField
			{
				Parent     = this,
				Anchor     = AnchorStyles.BottomLeft,
				Visibility = false,
			};

			this.selectedIndex  = -1;
			this.hilitedIndex   = -1;
			this.gapHilitedRank = -1;

			ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
		}


		public bool IsDragSource
		{
			get;
			set;
		}


		public void Clear()
		{
			this.tabs.Clear ();

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
			var tab = new Tab
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


		public void ClearAdditionnalWidgets()
		{
			//	Supprime tous les widgets à droite des onglets.
			this.additionnalWidgets.Clear ();
			this.Children.Clear ();

			this.dirtyLayout = true;
			this.Invalidate ();
		}

		public void AddAdditionnalWidget(Widget widget)
		{
			//	Ajoute un widget à droite des onglets.
			this.additionnalWidgets.Add (widget);

			widget.Parent = this;
			widget.Anchor = AnchorStyles.TopLeft;

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
			int index, rank;

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

			this.DragMove ();

			foreach (var widget in this.additionnalWidgets)
			{
				widget.Visibility = false;
			}
		}

		private void DragMove()
		{
			if (this.dragInfo != null)
			{
				var pos = new Point (this.GapMarkPos, this.ActualHeight);
				this.dragInfo.Update (this.MapClientToScreen (pos), this.IsNopDrag);
			}
		}

		private void DragEnd()
		{
			if (this.dragInfo != null)
			{
				this.dragInfo.Dispose ();
				this.dragInfo = null;
			}
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
					this.PaintTabFrame (graphics, rect, state);

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
					var pos = rect.BottomLeft;
					var tab = this.GetShowedTabFromRank (rank);
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

			this.PaintTabFrame (graphics, rect, state);

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

		private void PaintTabFrame(Graphics graphics, Rectangle rect, TabState state)
		{
			//	Dessine le cadre et le fond d'un onglet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			var path = this.GetTabPath (rect);
			var colorFrame = adorner.ColorBorder;

			//	Dessine le fond.
			if (state == TabState.Hilited)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (adorner.ColorBorder);

				var p = this.GetTabPath (rect, new Margins (1.5, 1.5, 1.5, 0));
				graphics.AddFilledPath (p);
				graphics.PaintVerticalGradient (rect, Color.FromAlphaColor (0.2, Color.FromBrightness (1.0)), Color.FromBrightness (1.0));
			}
			else if (state == TabState.Selected)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (UIBuilder.SelectionColor);

				var p = this.GetTabPath (rect, new Margins (2.5, 2.5, 2.5, 0));
				graphics.AddFilledPath (p);
				graphics.RenderSolid (Color.FromBrightness (1.0));
			}
			else if (state == TabState.MenuOpened)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (1.0));
			}
			else if (state == TabState.StartDragging)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromAlphaColor (0.4, Color.FromBrightness (1.0)));

				colorFrame = Color.FromAlphaColor (0.5, colorFrame);
			}
			else if (state == TabState.Floating)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (1.0));
			}
			else
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (1.0));

				graphics.AddFilledPath (path);
				graphics.PaintVerticalGradient (rect, Color.FromAlphaColor (0.5, adorner.ColorBorder), Color.FromAlphaColor (0.0, adorner.ColorBorder));
			}

			//	Dessine le cadre.
			graphics.AddPath (path);
			graphics.RenderSolid (colorFrame);
		}

		private Path GetTabPath(Rectangle rect)
		{
			return this.GetTabPath (rect, Margins.Zero);
		}

		private Path GetTabPath(Rectangle rect, Margins margins)
		{
			//	Retourne le chemin pour dessiner/détecter un onglet.
			var path = new Path ();

			rect.Deflate (margins);
			rect.Deflate (0.5);

#if false
			//	Onglet en trapèze.
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
#else
			//	Onglet style "OneNote".
			var p1 = new Point (rect.Left-TabsPane.tabMargin*0.5, rect.Bottom);
			var p2 = new Point (rect.Left-TabsPane.tabMargin*0.5, rect.Top);
			var p3 = new Point (rect.Right-TabsPane.tabMargin*0.5, rect.Top);
			var p4 = new Point (rect.Right+TabsPane.tabMargin*1.8, rect.Bottom);

			double d = rect.Height * 0.2;

			var p21 = Point.Move (p2, p1, d);
			var p23 = Point.Move (p2, p3, d);

			path.MoveTo (p1);
			path.LineTo (p21);
			path.CurveTo (p2, p23);
			path.LineTo (p3);
			path.LineTo (p4);
#endif

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
				path.MoveTo (c.X, c.Y-d-6);
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
			double x = TabsPane.tabMargin;

			for (int r = 0; r < rank; r++)
			{
				x += this.GetShowedWidth (r);
				x += TabsPane.tabMargin;
			}

			return new Rectangle (x, 0, this.GetShowedWidth (rank), this.ActualHeight-2);
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

			//	Initialise les largeurs courantes des onglets, en diminuant la largeur si l'onglet est
			//	vraiment trop grand.
			int n = this.tabs.Count;
			n = System.Math.Max (n, 1);
			n = System.Math.Min (n, 4);

			double actualWidth = this.ActualWidth - this.GetAdditionnalWidgetsWidth;
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

			double offset = this.GetTextRect (this.showedIndexes.Count-1).Right + TabsPane.tabMargin;
			this.UpdateAdditionnalWidgetsLayout (offset);
		}

		private void UpdateIndexesStretch()
		{
			this.showedIndexes.Clear ();

			double actualWidth = this.ActualWidth - this.GetAdditionnalWidgetsWidth;
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


		private double GetAdditionnalWidgetsWidth
		{
			//	Retourne la largeur de l'ensemble des widgets additionnels.
			get
			{
				double width = 0;

				foreach (var widget in this.additionnalWidgets)
				{
					width += widget.PreferredWidth;
				}

				return width;
			}
		}

		private void UpdateAdditionnalWidgetsLayout(double offset)
		{
			//	Met à jour les positions des widgets additionnels.
			double x = 0;

			foreach (var widget in this.additionnalWidgets)
			{
				widget.Margins = new Margins (offset+x, 0, 0, 0);
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
			menu.ShowAsComboList (this, this.MapClientToScreen (new Point (x, 1)), this);
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

			if (tab.TabItem.DeleteVisibility)
			{
				this.AddToContextMenu (menu, "Edit.Delete", "Supprimer", tab.TabItem.DeleteEnable, delegate
				{
					this.OnDeleteDoing (this.menuTabIndex);
				});
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
			menu.ShowAsComboList (this, this.MapClientToScreen (new Point (x, 1)), this);
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
			public Tab()
			{
				this.textLayout = new TextLayout
				{
					Alignment = ContentAlignment.MiddleCenter,
					BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				};
			}

			public TextLayout TextLayout
			{
				get
				{
					return this.textLayout;
				}
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

					if (string.IsNullOrEmpty (this.tabItem.Icon))
					{
						this.textLayout.FormattedText = this.tabItem.FormattedText;
					}
					else if (this.tabItem.FormattedText.IsNullOrEmpty)
					{
						this.textLayout.FormattedText = UIBuilder.GetTextIconUri (this.tabItem.Icon, iconSize: 20);
					}
					else
					{
						this.textLayout.FormattedText = UIBuilder.GetTextIconUri (this.tabItem.Icon, iconSize: 20) + " " + this.tabItem.FormattedText;
					}

					this.textWidth = this.textLayout.GetSingleLineSize ().Width;
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


		private void OnDeleteDoing(int index)
		{
			if (this.DeleteDoing != null)
			{
				this.DeleteDoing (this, index);
			}
		}

		public delegate void DeleteEventHandler(object sender, int index);
		public event DeleteEventHandler DeleteDoing;


		private void OnDraggingDoing(int srcIndex, int dstIndex)
		{
			if (this.DraggingDoing != null)
			{
				this.DraggingDoing (this, srcIndex, dstIndex);
			}
		}

		public delegate void DraggingEventHandler(object sender, int srcIndex, int dstIndex);
		public event DraggingEventHandler DraggingDoing;


		private static readonly double				tabMargin  = 8;
		private static readonly double				textMargin = 2;
		private static readonly double				menuWidth  = 20;
		private static readonly int					menuIndex  = 999;

		private readonly List<Tab>					tabs;
		private readonly List<int>					showedIndexes;
		private readonly List<int>					hiddenIndexes;
		private readonly List<Widget>				additionnalWidgets;
		private readonly TextField					renameField;

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
