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
	public class TabsPane : FrameBox
	{
		public TabsPane()
		{
			this.tabs               = new List<Tab> ();
			this.showedIndexes      = new List<int> ();
			this.hiddenIndexes      = new List<int> ();
			this.additionnalWidgets = new List<Widget> ();

			this.selectedIndex = -1;
			this.hilitedIndex  = -1;
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

		public void Add(FormattedText text)
		{
			this.Insert (this.tabs.Count, text);
		}

		public void Insert(int index, FormattedText text)
		{
			var tab = new Tab
			{
				Text = text,
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
			int index;

			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					index = this.Detect (pos);
					if (this.hilitedIndex != index)
					{
						this.hilitedIndex = index;
						this.Invalidate ();
					}
					break;

				case MessageType.MouseLeave:
					index = -1;
					if (this.hilitedIndex != index)
					{
						this.hilitedIndex = index;
						this.Invalidate ();
					}
					break;

				case MessageType.MouseDown:
					index = this.Detect (pos);
					if (index != -1)
					{
						if (index == TabsPane.menuIndex)
						{
							this.ShowMenu ();
						}
						else
						{
							this.selectedIndex = index;
							this.OnSelectedIndexChanged ();
						}
					}
					break;

				case MessageType.MouseUp:
					break;
			}

			base.ProcessMessage (message, pos);
		}

		private int Detect(Point pos)
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


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);
			this.PaintTabs (graphics);
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

			if (index == this.selectedIndex)
			{
				state = TabState.Selected;
			}

			if (index == this.hilitedIndex)
			{
				state = TabState.Hilited;
			}

			if (this.menuOpened && index == TabsPane.menuIndex)
			{
				state = TabState.MenuOpened;
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
					rect.Inflate (1, -2);
					var pos = rect.BottomLeft;
					var tab = this.GetShowedTab (rank);
					tab.TextLayout.LayoutSize = rect.Size;
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

			//	Dessine le fond.
			if (state == TabState.Hilited)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (adorner.ColorBorder);

				var p = this.GetTabPath (rect, new Margins (2.5, 2.5, 2.5, 0));
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
			else
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromBrightness (1.0));

				graphics.AddFilledPath (path);
				graphics.PaintVerticalGradient (rect, Color.FromAlphaColor (0.5, adorner.ColorBorder), Color.FromAlphaColor (0.0, adorner.ColorBorder));
			}

			//	Dessine le cadre.
			graphics.AddPath (path);
			graphics.RenderSolid (adorner.ColorBorder);
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

			return path;
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
					menuIndex = TabsPane.menuIndex;
					yield return this.showedIndexes.IndexOf (menuIndex);
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
			var tab = this.GetShowedTab (rank);

			if (tab == null)
			{
				return TabsPane.menuWidth;
			}
			else
			{
				return tab.CurrentWidth;
			}
		}

		private Tab GetShowedTab(int rank)
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
			if (!this.dirtyLayout && this.lastWidth == this.ActualWidth)
			{
				return;
			}

			this.dirtyLayout = false;
			this.lastWidth = this.ActualWidth;

			//	Met à jour les index devant être dessinés et ceux qui sont cachés, dans l'ordre de visibilité
			//	de gauche à droite. A ne pas confondre avec l'ordre de détection/dessin.
			this.showedIndexes.Clear ();
			this.hiddenIndexes.Clear ();

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
			for (int i = 0; i < tabs.Count; i++)
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

			//	Met à jour la liste des index invisibles.
			for (int i = 0; i < this.tabs.Count; i++)
			{
				if (!this.showedIndexes.Contains (i))
				{
					this.hiddenIndexes.Add (i);
				}
			}

			double offset = this.GetTextRect (this.showedIndexes.Count-1).Right + TabsPane.tabMargin;
			this.UpdateAdditionnalWidgetsLayout (offset);
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


		private void ShowMenu()
		{
			//	Affiche le menu sous l'onglet 'v'.
			var menu = new VMenu ();

			foreach (var index in this.hiddenIndexes)
			{
				var item = new MenuItem
				{
					FormattedText = this.tabs[index].Text,
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

			public FormattedText Text
			{
				get
				{
					return this.textLayout.FormattedText;
				}
				set
				{
					this.textLayout.FormattedText = value;
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
			private double						textWidth;
		}


		private enum TabState
		{
			Normal,			// onglet non sélectionné
			Selected,		// onglet sélectionné
			Hilited,		// onglet survolé par la souris
			MenuOpened,		// onglet 'v' du menu, avec le menu ouvert
		}


		private void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged (this);
			}
		}

		public event EventHandler SelectedIndexChanged;


		private static readonly double				tabMargin  = 8;
		private static readonly double				textMargin = 2;
		private static readonly double				menuWidth  = 20;
		private static readonly int					menuIndex  = 999;

		private readonly List<Tab>					tabs;
		private readonly List<int>					showedIndexes;
		private readonly List<int>					hiddenIndexes;
		private readonly List<Widget>				additionnalWidgets;

		private int									selectedIndex;
		private int									hilitedIndex;
		private bool								menuOpened;
		private bool								dirtyLayout;
		private double								lastWidth;
	}
}
