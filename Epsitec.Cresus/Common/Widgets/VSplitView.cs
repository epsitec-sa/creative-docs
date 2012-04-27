//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass (typeof (VSplitView))]

namespace Epsitec.Common.Widgets
{
	public partial class VSplitView : AbstractSplitView
	{
		public VSplitView()
		{
			this.frame1Container = new Widget (this)
			{
				PreferredHeight = 0,
				Dock = DockStyle.Top,
			};

			this.separator = new FrameBox (this.frame1Container)
			{
				Dock = DockStyle.Bottom,
				PreferredHeight = 8,
				BackColor = Color.FromRgb (0, 0, 0.4),
				MouseCursor = MouseCursor.AsHSplit,
				MinHeight = 0,
			};

			this.frame2Container = new Widget (this)
			{
				Visibility = true,
				Dock = DockStyle.Fill,
			};

			this.frame1 = new Widget (this.frame1Container)
			{
				Dock = DockStyle.Fill,
			};

			this.scroller1 = new VScroller (this.frame1Container)
			{
				Dock = DockStyle.Right,
				MinHeight = 0,
			};

			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (this.separator, true);
			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (this.scroller1, true);

			this.frame2 = new Widget (this.frame2Container)
			{
				Dock = DockStyle.Fill,
			};

			var scrollerContainer = new Widget (this.frame2Container)
			{
				Dock = DockStyle.Right,
				PreferredWidth = AbstractScroller.DefaultBreadth,
			};

			this.dragButton = new GlyphButton (scrollerContainer)
			{
				Dock = DockStyle.Top,
				PreferredHeight = AbstractScroller.DefaultBreadth,
			};

			this.scroller2 = new VScroller (scrollerContainer)
			{
				Dock = DockStyle.Fill,
			};


			this.dragButton.Pressed += this.HandleDragButtonPressed;
			this.separator.Pressed  += this.HandleDragButtonPressed;
		}


		public override Widget					Frame1
		{
			get
			{
				return this.frame1;
			}
		}

		public override Widget					Frame2
		{
			get
			{
				return this.frame2;
			}
		}

		
		protected override void UpdateRatio()
		{
			this.frame1Container.PreferredHeight = System.Math.Max (0, System.Math.Floor (this.Client.Height * this.Ratio));
		}
		
		
		private void HandleDragButtonPressed(object sender, MessageEventArgs e)
		{
			var host = new DragProcessor (this, sender as Widget, e);
		}

		
		private readonly Widget					frame1Container;
		private readonly Widget					frame1;
		private readonly VScroller				scroller1;
		private readonly Widget					separator;
		private readonly Widget					frame2Container;
		private readonly Widget					frame2;
		private readonly GlyphButton			dragButton;
		private readonly VScroller				scroller2;
	}
}
