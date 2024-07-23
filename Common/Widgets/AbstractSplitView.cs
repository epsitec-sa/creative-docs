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


using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>AbstractSplitView</c> class is the base class for the split views (see
    /// <see cref="VSplitView"/>).
    /// </summary>
    public abstract class AbstractSplitView : Widget
    {
        protected AbstractSplitView() { }

        public double Ratio
        {
            get { return this.ratio; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > 1)
                {
                    value = 1;
                }

                if (this.ratio != value)
                {
                    this.ratio = value;
                    this.UpdateRatioAndNotify();
                }
            }
        }

        public int CollapseThreshold
        {
            get { return this.collapseThreshold; }
            set { this.collapseThreshold = value; }
        }

        public abstract Widget Frame1 { get; }

        public abstract Widget Frame2 { get; }

        public abstract AbstractScroller Scroller1 { get; }

        public abstract AbstractScroller Scroller2 { get; }

        public abstract SplitViewFrameVisibility FrameVisibility { get; }

        protected void UpdateRatioAndNotify()
        {
            var oldValue = this.FrameVisibility;

            this.UpdateRatio();

            var newValue = this.FrameVisibility;

            if (oldValue != newValue)
            {
                this.OnFrameVisibilityChanged(
                    new SplitViewFrameVisibilityEventArgs(oldValue, newValue)
                );
            }
        }

        protected override void UpdateClientGeometry()
        {
            base.UpdateClientGeometry();
            this.UpdateRatioAndNotify();
        }

        protected abstract void UpdateRatio();

        protected virtual void OnFrameVisibilityChanged(SplitViewFrameVisibilityEventArgs e)
        {
            this.RaiseUserEvent<SplitViewFrameVisibilityEventArgs>(
                EventNames.FrameVisibilityChanged,
                e
            );
        }

        #region EventNames Class

        private new static class EventNames
        {
            public const string FrameVisibilityChanged = "FrameVisibilityChanged";
        }

        #endregion


        public event EventHandler<SplitViewFrameVisibilityEventArgs> FrameVisibilityChanged
        {
            add { this.AddUserEventHandler(EventNames.FrameVisibilityChanged, value); }
            remove { this.RemoveUserEventHandler(EventNames.FrameVisibilityChanged, value); }
        }

        private int collapseThreshold = 28;
        private double ratio;
    }
}
