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
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;

[assembly: DependencyClass(typeof(SearchBox))]

namespace Epsitec.Common.Widgets
{
    public class SearchBox : AbstractTextField, ISearchBox
    {
        public SearchBox()
        {
            this.searchPolicy = new SearchBoxPolicy();
            this.searchBehavior = new SearchBehavior(this);

            this.DefocusAction = DefocusAction.None;
            this.ButtonShowCondition = ButtonShowCondition.Always;

            this.SwallowReturnOnAcceptEdition = true;
        }

        public SearchBox(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        #region ISearchBox Members

        public SearchBoxPolicy Policy
        {
            get { return this.searchPolicy; }
        }

        void ISearchBox.NotifySearchClicked()
        {
            this.OnSearchClicked();
        }

        void ISearchBox.NotifyShowNextClicked()
        {
            this.OnShowNextClicked();
        }

        void ISearchBox.NotifyShowPrevClicked()
        {
            this.OnShowPrevClicked();
        }

        #endregion

        protected override bool CanStartEdition
        {
            get { return true; }
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            base.PaintBackgroundImplementation(graphics, clipRect);
        }

        protected override void UpdateButtonGeometry()
        {
            if (this.searchBehavior != null)
            {
                var current = this.margins;
                var right = this.searchBehavior.DefaultWidth;

                this.margins = new Margins(current.Left, right, current.Top, current.Bottom);
                this.searchBehavior.UpdateButtonGeometry();
            }

            base.UpdateButtonGeometry();
        }

        protected override void SetButtonVisibility(bool show)
        {
            if (this.searchBehavior == null)
            {
                return;
            }

            if (this.searchBehavior.IsVisible != show)
            {
                this.searchBehavior.SetVisible(show);

                Window window = this.Window;

                if (window != null)
                {
                    window.ForceLayout();
                }

                this.UpdateButtonGeometry();
                this.UpdateButtonEnable();
                this.UpdateTextLayout();
                this.UpdateMouseCursor(this.MapRootToClient(Message.CurrentState.LastPosition));
            }
        }

        protected void UpdateButtonEnable()
        {
            if (this.searchBehavior != null)
            {
                this.searchBehavior.SetSearchEnabled(this.IsValid);
            }
        }

        protected void OnSearchClicked()
        {
            this.SearchClicked.Raise(this);
        }

        protected void OnShowNextClicked()
        {
            this.ShowNextClicked.Raise(this);
        }

        protected void OnShowPrevClicked()
        {
            this.ShowPrevClicked.Raise(this);
        }

        protected override void OnEditionAccepted()
        {
            base.OnEditionAccepted();
            this.OnSearchClicked();
        }

        public event EventHandler SearchClicked;
        public event EventHandler ShowNextClicked;
        public event EventHandler ShowPrevClicked;

        private readonly SearchBehavior searchBehavior;
        private readonly SearchBoxPolicy searchPolicy;
    }
}
