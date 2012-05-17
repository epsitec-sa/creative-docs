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
			this.tabs = new List<Tab> ();

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

			this.UpdateTabs ();
			this.Invalidate ();
		}

		public void RemoveAt(int index)
		{
			this.tabs.RemoveAt (index);

			this.UpdateTabs ();
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

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			foreach (var index in this.Indexes.Reverse ())
			{
				this.PaintTab (graphics, index);
			}
		}


		private int Detect(Point pos)
		{
			foreach (var index in this.Indexes)
			{
				var path = this.GetTabPath (index);

				if (path.SurfaceContainsPoint (pos.X, pos.Y, 1))
				{
					return index;
				}
			}

			return -1;
		}

		private IEnumerable<int> Indexes
		{
			//	Retourne les index dans l'ordre pour la détection.
			//	Il faut utiliser l'ordre inverse pour le dessin.
			get
			{
				if (this.selectedIndex != -1)
				{
					yield return this.selectedIndex;
				}

				for (int i = 0; i < this.tabs.Count; i++)
				{
					if (i != this.selectedIndex)
					{
						yield return i;
					}
				}
			}
		}

		private void PaintTab(Graphics graphics, int index)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			var tab      = this.tabs[index];
			var selected = index == this.selectedIndex;
			var hilited  = index == this.hilitedIndex;
			var rect     = this.GetTabRect (index);
			var path     = this.GetTabPath (index);

			var color = hilited ? UIBuilder.SelectionColor : Color.FromBrightness (1.0);

			graphics.AddFilledPath (path);
			graphics.RenderSolid (color);

			if (!selected)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromAlphaColor (0.2, adorner.ColorBorder));
			}

			graphics.AddPath (path);
			graphics.RenderSolid (adorner.ColorBorder);

			rect.Inflate (1);
			var pos = rect.BottomLeft;
			tab.TextLayout.LayoutSize = rect.Size;
			tab.TextLayout.Paint (pos, graphics);
		}

		private Path GetTabPath(int index)
		{
			var path = new Path ();

			var rect = this.GetTabRect (index);
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

		private Rectangle GetTabRect(int index)
		{
			double x = TabsPane.tabMargin;

			for (int i = 0; i < index; i++)
			{
				var tab = this.tabs[i];
				x += tab.TextWidth;
				x += TabsPane.tabMargin;
			}

			//?double h = (index == this.selectedIndex) ? this.ActualHeight : this.ActualHeight-5;
			double h = this.ActualHeight-5;

			return new Rectangle (x, 0, this.tabs[index].TextWidth, h);
		}


		private void UpdateTabs()
		{
			foreach (var tab in this.tabs)
			{
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

		private readonly List<Tab>					tabs;
		private int									selectedIndex;
		private int									hilitedIndex;
	}
}
