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
			this.tabs          = new List<Tab> ();
			this.showedIndexes = new List<int> ();
			this.hiddenIndexes = new List<int> ();

			this.selectedIndex = -1;
			this.hilitedIndex = -1;
		}


		public void Clear()
		{
			this.tabs.Clear ();
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

			this.Invalidate ();
		}

		public void RemoveAt(int index)
		{
			this.tabs.RemoveAt (index);

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
					this.Invalidate ();
				}
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			int i;

			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					i = this.Detect (pos);
					if (this.hilitedIndex != i)
					{
						this.hilitedIndex = i;
						this.Invalidate ();
					}
					break;

				case MessageType.MouseLeave:
					i = -1;
					if (this.hilitedIndex != i)
					{
						this.hilitedIndex = i;
						this.Invalidate ();
					}
					break;

				case MessageType.MouseDown:
					i = this.Detect (pos);
					if (i != -1)
					{
						this.selectedIndex = i;
						this.OnSelectedIndexChanged ();
					}
					break;

				case MessageType.MouseUp:
					break;
			}

			base.ProcessMessage (message, pos);
		}

		private int Detect(Point pos)
		{
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

			var selected = index == this.selectedIndex;
			var hilited  = index == this.hilitedIndex;
			var rect     = this.GetTextRect (rank);

			if (!rect.IsEmpty)
			{
				if (index == TabsPane.menuIndex)  // onglet 'v' du menu ?
				{
					this.PaintTabMenu (graphics, rect, selected, hilited);
				}
				else
				{
					//	Dessine le cadre et le fond.
					this.PaintTabFrame (graphics, rect, selected, hilited);

					//	Dessine le texte.
					rect.Inflate (1);
					var pos = rect.BottomLeft;
					var tab = this.GetShowedTab (rank);
					tab.TextLayout.LayoutSize = rect.Size;
					tab.TextLayout.Paint (pos, graphics);
				}
			}
		}

		private void PaintTabMenu(Graphics graphics, Rectangle rect, bool selected, bool hilited)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			this.PaintTabFrame (graphics, rect, selected, hilited);

			var c = rect.Center;
			var d = rect.Height*0.25;

			var path = new Path ();
			path.MoveTo (c.X, c.Y-d);
			path.LineTo (c.X-d, c.Y);
			path.LineTo (c.X+d, c.Y);
			path.Close ();

			graphics.AddFilledPath (path);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		private void PaintTabFrame(Graphics graphics, Rectangle rect, bool selected, bool hilited)
		{
			//	Dessine le cadre et le fond d'un onglet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			var path = this.GetTabPath (rect);

			//	Dessine le fond.
			if (hilited)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (adorner.ColorBorder);

				var p = this.GetTabPath (rect, new Margins (2.5, 2.5, 2.5, 0));
				graphics.AddFilledPath (p);
				graphics.PaintVerticalGradient (rect, Color.FromAlphaColor (0.2, Color.FromBrightness (1.0)), Color.FromBrightness (1.0));
			}
			else if (selected)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (UIBuilder.SelectionColor);

				var p = this.GetTabPath (rect, new Margins (2.5, 2.5, 2.5, 0));
				graphics.AddFilledPath (p);
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
				//	Retourne toujours l'onglet sélectionné en premier.
				if (this.selectedIndex != -1)
				{
					yield return this.showedIndexes.IndexOf (this.selectedIndex);
				}

				//	Retourne ensuite tous les autres onglets.
				for (int rank = 0; rank < this.showedIndexes.Count; rank++)
				{
					int index = this.showedIndexes[rank];

					if (index != this.selectedIndex)
					{
						yield return rank;
					}
				}
			}
		}

		private Rectangle GetTextRect(int rank)
		{
			double x = TabsPane.tabMargin;

			for (int r = 0; r < rank; r++)
			{
				x += this.GetShowedWidth (r);
				x += TabsPane.tabMargin;
			}

			return new Rectangle (x, 0, this.GetShowedWidth (rank), this.ActualHeight-5);
		}

		private double GetShowedWidth(int rank)
		{
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
			this.showedIndexes.Clear ();
			this.hiddenIndexes.Clear ();

			//	Initialise les largeurs courantes des onglets, en diminuant la largeur si l'onglet est
			//	vraiment trop grand.
			int n = this.tabs.Count;
			n = System.Math.Max (n, 1);
			n = System.Math.Min (n, 4);

			//?double max = System.Math.Floor (this.ActualWidth/n - TabsPane.tabMargin*n*2);
			double max = System.Math.Floor (this.ActualWidth/n);

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

			if (rigths[this.tabs.Count-1] <= this.ActualWidth-TabsPane.tabMargin*1.8)  // assez de place ?
			{
				//	Cas où on a assez de place pour tout mettre normalement.
				for (int i = 0; i < tabs.Count; i++)
				{
					this.showedIndexes.Add (i);
				}
			}
			else
			{
				max = this.ActualWidth-TabsPane.menuWidth-TabsPane.tabMargin*2;

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

			//	Garde-fou.
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
				get
				{
					return System.Math.Floor (this.textWidth + TabsPane.textMargin*2) + 1;
				}
			}

			public double CurrentWidth
			{
				get;
				set;
			}

			private readonly TextLayout			textLayout;
			private double						textWidth;
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
		private int									selectedIndex;
		private int									hilitedIndex;
	}
}
