//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;

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

			for (int i = 0; i < this.tabs.Count; i++)
			{
				this.PaintTab (graphics, i);
			}
		}


		private int Detect(Point pos)
		{
			for (int i = 0; i < this.tabs.Count; i++)
			{
				var path = this.GetTabPath (i);

				if (path.SurfaceContainsPoint (pos.X, pos.Y, 1))
				{
					return i;
				}
			}

			return -1;
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

			path.MoveTo (rect.Left-TabsPane.tabMargin, rect.Bottom);
			path.LineTo (rect.Left, rect.Top);
			path.LineTo (rect.Right, rect.Top);
			path.LineTo (rect.Right+TabsPane.tabMargin, rect.Bottom);

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
					return this.textWidth + TabsPane.textMargin*2;
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


		private static readonly double				tabMargin  = 10;
		private static readonly double				textMargin = 6;

		private readonly List<Tab>					tabs;
		private int									selectedIndex;
		private int									hilitedIndex;
	}
}
