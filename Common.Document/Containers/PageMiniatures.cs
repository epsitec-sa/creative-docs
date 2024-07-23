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

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Containers
{
    /// <summary>
    /// Cette classe gère le panneau des vues des pages miniatures.
    /// </summary>
    public class PageMiniatures
    {
        public PageMiniatures(Document document)
        {
            this.document = document;

            this.pages = new List<int>();
            this.pagesToRegenerate = new List<int>();
        }

        public bool IsPanelShowed
        {
            //	Indique si le panneau des minuatures est visible ou caché.
            get { return this.isPanelShowed; }
            set { this.isPanelShowed = value; }
        }

        public void CreateInterface(FrameBox parentPanel)
        {
            //	Crée l'interface nécessaire pour les pages miniatures dans un panneau dédié.
            this.parentPanel = parentPanel;

            //	Crée puis peuple la barre d'outils.
            VToolBar toolbar = new VToolBar(this.parentPanel);
            toolbar.Dock = DockStyle.Left;
            toolbar.Margins = new Margins(0, -1, 0, 0);

            this.slider = new VSlider();
            this.slider.MinValue = 1M;
            this.slider.MaxValue = 4M;
            this.slider.SmallChange = 1M;
            this.slider.LargeChange = 1M;
            this.slider.Resolution = (this.document.Type == DocumentType.Pictogram) ? 1.0M : 0.01M;
            this.slider.Value = 2M;
            this.slider.PreferredHeight = 60;
            //?this.slider.PreferredWidth = 22-4-4;
            this.slider.Margins = new Margins(4, 4, 0, 0);
            this.slider.Dock = DockStyle.Bottom;
            this.slider.ValueChanged += this.HandleSliderValueChanged;
            ToolTip.Default.SetToolTip(
                this.slider,
                Res.Strings.Container.PageMiniatures.Slider.Tooltip
            );
            toolbar.Items.Add(this.slider);

            //	Crée la zone contenant les miniatures.
            this.scrollable = new Scrollable(this.parentPanel);
            this.scrollable.Dock = DockStyle.Fill;
            this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.HideAlways;
            this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways;
            this.scrollable.Viewport.IsAutoFitting = true;
            this.scrollable.PaintViewportFrame = true;
            this.scrollable.ViewportFrameMargins = new Margins(0, 0, 0, 1);
            this.scrollable.ViewportPadding = new Margins(-1);
        }

        public void RegenerateAll()
        {
            //	Tout devra être régénéré lorsque le timer arrivera à échéance.
            int total = this.TotalPages;
            for (int page = 0; page < total; page++)
            {
                if (!this.pagesToRegenerate.Contains(page))
                {
                    this.pagesToRegenerate.Add(page);
                }
            }

            this.pagesToRegenerate.Sort();
        }

        public void AddPageToRegenerate(int page)
        {
            //	Ajoute une page dans la liste des choses à régénérer lorsque le timer arrivera à échéance.
            if (!this.pagesToRegenerate.Contains(page))
            {
                this.pagesToRegenerate.Add(page);
                this.pagesToRegenerate.Sort();
            }
        }

        public bool TimeElapsed()
        {
            //	Appelé lorsque le timer arrive à échéance.
            //	On régénère alors une partie de ce que la liste indique.
            //	On retourne false s'il n'y avait plus rien à régénérer.
            int currentPage = this.CurrentPage;
            if (this.pagesToRegenerate.Contains(currentPage))
            {
                this.RegeneratePage(currentPage);
                return true;
            }

            if (this.pagesToRegenerate.Count > 0)
            {
                this.RegeneratePage(this.pagesToRegenerate[0]);
                return true;
            }

            return false;
        }

        protected void RegeneratePage(int pageRank)
        {
            //	Régénère une page donnée.
            Objects.Abstract page = this.GetPage(pageRank);

            if (page != null)
            {
                page.CacheBitmapDirty();
                this.Redraw(pageRank);
            }

            this.pagesToRegenerate.Remove(pageRank);
        }

        public void UpdatePageAfterChanging()
        {
            //	Adapte les miniatures après un changement de page (création d'une
            //	nouvelle page, suppression d'une page, etc.).
            this.pages.Clear();

            int total = this.TotalPages;
            for (int i = 0; i < total; i++)
            {
                this.pages.Add(i);
            }

            this.Create();
        }

        public void Redraw(int page)
        {
            //	Redessine une page qui a changé.
            foreach (Viewer viewer in this.document.Modifier.Viewers)
            {
                if (
                    viewer.IsMiniature
                    && !viewer.IsLayerMiniature
                    && viewer.DrawingContext.CurrentPage == page
                )
                {
                    viewer.Invalidate();
                }
            }
        }

        private void HandleSliderValueChanged(object sender)
        {
            //	Appelé lorsque la taille des miniatures a changé.
            this.Create();
        }

        protected void Create()
        {
            //	Crée toutes les pages miniatures.
            if (this.parentPanel.Window == null) // initialisation du logiciel ?
            {
                return;
            }

            double offsetX = this.scrollable.ViewportOffsetX;
            this.Clear(); // supprime les miniatures existantes

            if (!this.isPanelShowed) // panneau caché ?
            {
                return;
            }

            double zoom = (double)this.slider.Value;
            int currentPage = this.CurrentPage;
            double posX = 0;
            double requiredHeight = 0;
            List<Viewer> viewers = new List<Viewer>();

            foreach (int page in this.pages) // page = [0..n-1]
            {
                Objects.Page objectPage = this.GetPage(page);
                if (objectPage == null)
                {
                    continue;
                }

                Size pageSize = this.document.GetPageSize(page);
                double w,
                    h;
                if (this.document.Type == DocumentType.Pictogram)
                {
                    w = System.Math.Ceiling(zoom * pageSize.Width) + 2;
                    h = System.Math.Ceiling(zoom * pageSize.Height) + 2;
                }
                else
                {
                    w = System.Math.Ceiling(zoom * pageSize.Width * 50 / 2970);
                    h = System.Math.Ceiling(zoom * pageSize.Height * 50 / 2970);
                }

                if (this.document.Type == DocumentType.Pictogram)
                {
                    objectPage.CacheBitmapSize = new Size(pageSize.Width + 2, pageSize.Height + 2);
                }
                else
                {
                    objectPage.CacheBitmapSize = new Size(w, h);
                }

                string pageName = this.PageName(page);

                Widgets.MiniatureFrame box = new Widgets.MiniatureFrame(this.scrollable.Viewport);
                box.IsLeftRightPlacement = true;
                box.Index = page;
                box.PreferredSize = new Size(w + 4, h + 4 + PageMiniatures.labelHeight);
                box.Padding = new Margins(2, 2, 2, 2);
                box.Anchor = AnchorStyles.TopLeft;
                box.Margins = new Margins(posX, 0, 0, 0);
                box.Clicked += this.HandlePageBoxClicked;
                box.DragAndDropDoing += this.HandleDragAndDropDoing;
                ToolTip.Default.SetToolTip(
                    box,
                    string.Format(Res.Strings.Container.PageMiniatures.Box.Tooltip, pageName)
                );

                //	Crée la vue de la page miniature.
                Viewer viewer = new Viewer(this.document);
                viewer.SetParent(box);
                viewer.Dock = DockStyle.Fill;
                viewer.PreferredSize = new Size(w, h);
                if (this.document.Type == DocumentType.Pictogram)
                {
                    viewer.PictogramMiniatureZoom = zoom;
                }
                viewer.IsMiniature = true;
                viewer.IsLayerMiniature = false;
                viewer.PaintPageFrame = false;
                viewer.DrawingContext.LayerDrawingMode = LayerDrawingMode.ShowInactive;
                viewer.DrawingContext.InternalPageLayer(page, 0);
                viewer.DrawingContext.PreviewActive = true;
                viewer.DrawingContext.GridShow = false;
                viewer.DrawingContext.GuidesShow = false;
                viewer.DrawingContext.TextShowControlCharacters = false;

                //	Crée la légende en bas, avec le numéro+nom de la page.
                StaticText label = new StaticText(box);
                label.PreferredSize = new Size(w, PageMiniatures.labelHeight);
                label.Dock = DockStyle.Bottom;
                label.ContentAlignment = ContentAlignment.TopCenter;
                label.Text = Misc.FontSize(pageName, 0.75);

                if (page == currentPage)
                {
                    viewer.DrawingContext.PreviewActive = false;
                    box.BackColor = viewer.DrawingContext.HiliteOutlineColor;
                    viewer.DrawingContext.PreviewActive = true;
                }

                posX += w + 2;
                requiredHeight = System.Math.Max(requiredHeight, h);
                viewers.Add(viewer);
            }

            this.scrollable.ViewportOffsetX = offsetX;
            this.parentPanel.PreferredHeight = requiredHeight + PageMiniatures.labelHeight + 23 + 4;
            this.parentPanel.Window.ForceLayout();

            foreach (Viewer viewer in viewers)
            {
                viewer.DrawingContext.ZoomPageAndCenter();
                this.document.Modifier.AttachViewer(viewer);
                this.document.Notifier.NotifyArea(viewer);
            }
        }

        public static Viewer GetViewer(Widgets.MiniatureFrame box)
        {
            //	Retourne la vue fille d'une miniature.
            System.Diagnostics.Debug.Assert(box.Children.Count == 2);
            System.Diagnostics.Debug.Assert(box.Children[0] is Viewer);
            return box.Children[0] as Viewer;
        }

        public static StaticText GetLabel(Widgets.MiniatureFrame box)
        {
            //	Retourne le texte fixe fils d'une miniature.
            System.Diagnostics.Debug.Assert(box.Children.Count == 2);
            System.Diagnostics.Debug.Assert(box.Children[1] is StaticText);
            return box.Children[1] as StaticText;
        }

        protected void Clear()
        {
            //	Supprime toutes les miniatures des pages.
            List<Viewer> viewers = new List<Viewer>();
            foreach (Viewer viewer in this.document.Modifier.Viewers)
            {
                if (viewer.IsMiniature && !viewer.IsLayerMiniature) // miniature d'une page ?
                {
                    viewers.Add(viewer);
                }
            }

            foreach (Viewer viewer in viewers)
            {
                this.document.Modifier.DetachViewer(viewer);
            }

            this.scrollable.Viewport.Children.Clear();
        }

        private void HandlePageBoxClicked(object sender, MessageEventArgs e)
        {
            //	Une page miniature a été cliquée.
            Widgets.MiniatureFrame box = sender as Widgets.MiniatureFrame;
            this.CurrentPage = box.Index; // rend la page cliquée active
        }

        private void HandleDragAndDropDoing(object sender, Widgets.MiniatureFrameEventArgs dst)
        {
            //	Une page a été draggée sur une autre.
            Widgets.MiniatureFrame src = sender as Widgets.MiniatureFrame;

            int page = dst.Frame.Index;

            if (!dst.Frame.IsBefore)
            {
                page++;
            }

            if (!src.IsDuplicate && dst.Frame.Index > src.Index)
            {
                page--;
            }

            if (src.IsDuplicate)
            {
                this.document.Modifier.PageDuplicate(src.Index, page, "");
            }
            else
            {
                this.document.Modifier.PageSwap(src.Index, page);
            }

            this.CurrentPage = page;
        }

        protected string PageName(int rank)
        {
            //	Retourne le nom le plus complet possible d'une page, constitué du numéro
            //	suivi de son éventuel nom.
            Objects.Page page = this.GetPage(rank);
            if (page == null)
            {
                return null;
            }

            string longName = page.LongName;
            if (string.IsNullOrEmpty(longName))
            {
                return string.Concat("<b>", page.ShortName, "</b>");
            }
            else
            {
                return string.Concat("<b>", page.ShortName, "</b> ", longName);
            }
        }

        protected int TotalPages
        {
            //	Retourne le nombre total de pages.
            get { return this.document.Modifier.ActiveViewer.DrawingContext.TotalPages(); }
        }

        protected int CurrentPage
        {
            //	Retourne le rang de la page courante.
            get { return this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage; }
            set { this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage = value; }
        }

        protected Objects.Page GetPage(int rank)
        {
            //	Retourne un objet Page du document.
            NewUndoableList doc = this.document.DocumentObjects;

            if (rank < doc.Count)
            {
                return doc[rank] as Objects.Page;
            }
            else
            {
                return null;
            }
        }

        protected static readonly double labelHeight = 12;

        protected Document document;
        protected List<int> pages;
        protected List<int> pagesToRegenerate;
        protected FrameBox parentPanel;
        protected VSlider slider;
        protected Scrollable scrollable;
        protected bool isPanelShowed;
    }
}
