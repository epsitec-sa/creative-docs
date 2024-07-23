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
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass(typeof(SearchPicker))]

namespace Epsitec.Common.Widgets
{
    public class SearchPicker : FrameBox
    {
        public SearchPicker()
        {
            this.searchBox = new SearchBox()
            {
                Parent = this,
                Dock = DockStyle.Stacked,
                Margins = new Margins(0, 0, 0, 3),
                TabIndex = 1,
            };

            this.spacer = new StaticText() { Parent = this, Dock = DockStyle.Stacked, };

            this.scrollList = new ScrollList()
            {
                Parent = this,
                Dock = DockStyle.Stacked,
                Margins = new Margins(0, 0, 3, 0),
                PreferredHeight = 120,
                TabIndex = 2,
            };

            this.messageContainerStateEmpty = new FrameBox()
            {
                Parent = this.scrollList,
                Dock = DockStyle.Fill,
            };

            this.messageContainerStateError = new FrameBox()
            {
                Parent = this.scrollList,
                Dock = DockStyle.Fill,
            };

            this.progressIndicator = new ProgressIndicator()
            {
                Parent = this.scrollList,
                VerticalAlignment = Widgets.VerticalAlignment.Center,
                HorizontalAlignment = Widgets.HorizontalAlignment.Stretch,
                Dock = DockStyle.Fill,
                ProgressStyle = ProgressIndicatorStyle.UnknownDuration,
            };

            this.spacer.PaintForeground += this.HandleSpacerPaintForeground;
            this.searchBox.TextChanged += this.HandleSearchBoxTextChanged;
            this.searchBox.SearchClicked += this.HandleSearchBoxSearchClicked;

            ProgressIndicator.Animate(this.progressIndicator);
        }

        public SearchPickerState State
        {
            get { return this.state; }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.OnSearchPickerStateChanged();
                }
            }
        }

        public FrameBox MessageContainerStateEmpty
        {
            get { return this.messageContainerStateEmpty; }
        }

        public FrameBox MessageContainerStateError
        {
            get { return this.messageContainerStateError; }
        }

        public FormattedText SearchText
        {
            get { return this.searchBox.FormattedText; }
        }

        public ScrollList ScrollList
        {
            get { return this.scrollList; }
        }

        protected void OnSearchPickerStateChanged()
        {
            switch (this.State)
            {
                case SearchPickerState.Undefined:
                    break;

                case SearchPickerState.Empty:
                    this.scrollList.DisableListDisplay = true;
                    this.messageContainerStateEmpty.Visibility = true;
                    this.messageContainerStateError.Visibility = false;
                    this.progressIndicator.Visibility = false;
                    break;

                case SearchPickerState.Ready:
                    this.scrollList.DisableListDisplay = false;
                    this.messageContainerStateEmpty.Visibility = false;
                    this.messageContainerStateError.Visibility = false;
                    this.progressIndicator.Visibility = false;
                    break;

                case SearchPickerState.Error:
                    this.scrollList.DisableListDisplay = true;
                    this.messageContainerStateEmpty.Visibility = false;
                    this.messageContainerStateError.Visibility = true;
                    this.progressIndicator.Visibility = false;
                    break;

                case SearchPickerState.Busy:
                    this.scrollList.DisableListDisplay = true;
                    this.messageContainerStateEmpty.Visibility = false;
                    this.messageContainerStateError.Visibility = false;
                    this.progressIndicator.Visibility = true;
                    break;

                default:
                    throw new System.NotSupportedException(
                        string.Format("{0} not supported", this.State.GetQualifiedName())
                    );
            }
        }

        protected void OnSearchClicked()
        {
            this.SearchClicked.Raise(this);
        }

        protected void OnSearchTextChanged()
        {
            this.SearchTextChanged.Raise(this);
        }

        private void HandleSpacerPaintForeground(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var bounds = this.spacer.Client.Bounds;

            double h = bounds.Height;
            double ox = System.Math.Floor(bounds.Center.X);
            double oy = System.Math.Floor((bounds.Height - 12) / 2);

            Path path = new Path();
            path.MoveTo(ox + 0.5, oy + 0.5);
            path.LineToRelative(20, 5);
            path.LineToRelative(-10, 0);
            path.LineToRelative(0, 5);
            path.LineToRelative(-20, 0);
            path.LineToRelative(0, -5);
            path.LineToRelative(-10, 0);
            path.LineToRelative(20, -5);
            path.Close();

            graphics.Color = Color.FromRgb(0.8, 0.8, 1);
            graphics.PaintSurface(path);

            graphics.LineWidth = 1.0;
            graphics.Color = Color.FromRgb(0.5, 0.5, 0.8);
            graphics.PaintOutline(path);
        }

        private void HandleSearchBoxSearchClicked(object sender)
        {
            this.OnSearchClicked();
        }

        private void HandleSearchBoxTextChanged(object sender)
        {
            this.OnSearchTextChanged();
        }

        public event EventHandler SearchClicked;
        public event EventHandler SearchTextChanged;

        private readonly SearchBox searchBox;
        private readonly StaticText spacer;
        private readonly ScrollList scrollList;

        private readonly FrameBox messageContainerStateEmpty;
        private readonly FrameBox messageContainerStateError;
        private readonly ProgressIndicator progressIndicator;

        private SearchPickerState state;
    }
}
