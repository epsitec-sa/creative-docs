/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Platform;

[assembly: DependencyClass(typeof(VSplitView))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>VSplitView</c> class implements a vertical split view: the splitter can be
    /// moved vertically to display the first frame, which is usually hidden.
    /// </summary>
    public partial class VSplitView : AbstractSplitView
    {
        public VSplitView()
        {
            this.frame1Container = new Widget(this) { PreferredHeight = 0, Dock = DockStyle.Top, };

            this.separator = new FrameBox(this.frame1Container)
            {
                Dock = DockStyle.Bottom,
                PreferredHeight = 8,
                BackColor = Color.FromRgb(0, 0, 0.4),
                MouseCursor = MouseCursor.Default,
                MinHeight = 0,
            };

            this.frame2Container = new Widget(this) { Visibility = true, Dock = DockStyle.Fill, };

            var scrollerContainer1 = new Widget(this.frame1Container)
            {
                Dock = DockStyle.Right,
                PreferredWidth = AbstractScroller.DefaultBreadth,
                MinHeight = 0,
                PreferredHeight = 0,
            };

            var scrollerContainer2 = new Widget(this.frame2Container)
            {
                Dock = DockStyle.Right,
                PreferredWidth = AbstractScroller.DefaultBreadth,
                MinHeight = 0,
                PreferredHeight = 0,
            };

            this.frame1 = new Widget(this.frame1Container) { Dock = DockStyle.Fill, };

            this.scroller1 = new VScroller(scrollerContainer1)
            {
                Dock = DockStyle.Fill,
                IsInverted = true,
                MinHeight = 0,
                VisibleRangeRatio = 1.0M,
            };

            this.frame2 = new Widget(this.frame2Container) { Dock = DockStyle.Fill, };

            this.dragButton = new GlyphButton(scrollerContainer2)
            {
                Dock = DockStyle.Top,
                PreferredHeight = AbstractScroller.DefaultBreadth,
            };

            this.scroller2 = new VScroller(scrollerContainer2)
            {
                Dock = DockStyle.Fill,
                IsInverted = true,
                MinHeight = 0,
                VisibleRangeRatio = 1.0M,
            };

            Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure(this.separator, true);
            Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure(this.scroller1, true);
            Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure(this.scroller2, true);

            this.dragButton.Pressed += this.HandleDragButtonPressed;
            this.separator.Pressed += this.HandleDragButtonPressed;
        }

        public override Widget Frame1
        {
            get { return this.frame1; }
        }

        public override Widget Frame2
        {
            get { return this.frame2; }
        }

        public override SplitViewFrameVisibility FrameVisibility
        {
            get
            {
                return new SplitViewFrameVisibility(
                    frame1: this.Frame1.PreferredHeight > 0,
                    frame2: this.Frame2.PreferredHeight > 0
                );
            }
        }

        public override AbstractScroller Scroller1
        {
            get { return this.scroller1; }
        }

        public override AbstractScroller Scroller2
        {
            get { return this.scroller2; }
        }

        protected override void UpdateRatio()
        {
            double ratio = this.Ratio;

            System.Diagnostics.Debug.Assert(ratio >= 0);
            System.Diagnostics.Debug.Assert(ratio <= 1);

            this.frame1Container.PreferredHeight = System.Math.Floor(this.Client.Height * ratio);
            this.frame2Container.PreferredHeight = System.Math.Ceiling(
                this.Client.Height * (1 - ratio)
            );
        }

        private void HandleDragButtonPressed(object sender, MessageEventArgs e)
        {
            var host = new DragProcessor(this, sender as Widget, e);
        }

        private readonly Widget frame1Container;
        private readonly Widget frame1;
        private readonly VScroller scroller1;
        private readonly Widget separator;
        private readonly Widget frame2Container;
        private readonly Widget frame2;
        private readonly VScroller scroller2;
        private readonly GlyphButton dragButton;
    }
}
