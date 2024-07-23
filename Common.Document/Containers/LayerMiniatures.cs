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
    /// Cette classe gère le panneau des vues des calques miniatures.
    /// </summary>
    public class LayerMiniatures
    {
        public LayerMiniatures(Document document)
        {
            this.document = document;

            this.layers = new List<int>();
            this.layersToRegenerate = new List<int>();
        }

        public bool IsPanelShowed
        {
            //	Indique si le panneau des minuatures est visible ou caché.
            get { return this.isPanelShowed; }
            set { this.isPanelShowed = value; }
        }

        public void CreateInterface(FrameBox parentPanel)
        {
            //	Crée l'interface nécessaire pour les calques miniatures dans un panneau dédié.
            this.parentPanel = parentPanel;

            //	Crée puis peuple la barre d'outils.
            HToolBar toolbar = new HToolBar(this.parentPanel);
            toolbar.PreferredWidth = 10;
            toolbar.Dock = DockStyle.Top;
            toolbar.Margins = new Margins(0, 0, 0, -1);

            this.slider = new HSlider();
            this.slider.MinValue = 1M;
            this.slider.MaxValue = 4M;
            this.slider.SmallChange = 1M;
            this.slider.LargeChange = 1M;
            this.slider.Resolution = (this.document.Type == DocumentType.Pictogram) ? 1.0M : 0.01M;
            this.slider.Value = 2M;
            this.slider.PreferredWidth = 60;
            //?this.slider.PreferredHeight = 22-4-4;
            this.slider.Margins = new Margins(0, 0, 4, 4);
            this.slider.Dock = DockStyle.Right;
            this.slider.ValueChanged += this.HandleSliderValueChanged;
            ToolTip.Default.SetToolTip(
                this.slider,
                Res.Strings.Container.LayerMiniatures.Slider.Tooltip
            );
            toolbar.Items.Add(this.slider);

            //	Crée la zone contenant les miniatures.
            this.scrollable = new Scrollable(this.parentPanel);
            this.scrollable.Dock = DockStyle.Fill;
            this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
            this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
            this.scrollable.Viewport.IsAutoFitting = true;
            this.scrollable.PaintViewportFrame = true;
            this.scrollable.ViewportFrameMargins = new Margins(0, 1, 0, 0);
            this.scrollable.ViewportPadding = new Margins(-1);
        }

        public void RegenerateAll()
        {
            //	Tout devra être régénéré lorsque le timer arrivera à échéance.
            int total = this.TotalLayers;
            for (int layer = 0; layer < total; layer++)
            {
                if (!this.layersToRegenerate.Contains(layer))
                {
                    this.layersToRegenerate.Add(layer);
                }
            }

            this.layersToRegenerate.Sort();
        }

        public void AddLayerToRegenerate(int layer)
        {
            //	Ajoute un calque dans la liste des choses à régénérer lorsque le timer arrivera à échéance.
            if (!this.layersToRegenerate.Contains(layer))
            {
                this.layersToRegenerate.Add(layer);
                this.layersToRegenerate.Sort();
            }
        }

        public bool TimeElapsed()
        {
            //	Appelé lorsque le timer arrive à échéance.
            //	On régénère alors une partie de ce que la liste indique.
            //	On retourne false s'il n'y avait plus rien à régénérer.
            int currentLayer = this.CurrentLayer;
            if (this.layersToRegenerate.Contains(currentLayer))
            {
                this.RegenerateLayer(currentLayer);
                return true;
            }

            if (this.layersToRegenerate.Count > 0)
            {
                this.RegenerateLayer(this.layersToRegenerate[0]);
                return true;
            }

            return false;
        }

        protected void RegenerateLayer(int layerRank)
        {
            //	Régénère un calque donné.
            Objects.Abstract layer = this.GetLayer(layerRank);

            if (layer != null)
            {
                layer.CacheBitmapDirty();
                this.Redraw(layerRank);
            }

            this.layersToRegenerate.Remove(layerRank);
        }

        public void UpdateLayerAfterChanging()
        {
            //	Adapte les miniatures après un changement de calque (création d'un
            //	nouveau calque, suppression d'un calque, etc.).
            this.layers.Clear();

            int total = this.TotalLayers;
            for (int i = total - 1; i >= 0; i--)
            {
                this.layers.Add(i);
            }

            this.Create();
        }

        public void Redraw(int layer)
        {
            //	Redessine un calque qui a changé.
            foreach (Viewer viewer in this.document.Modifier.Viewers)
            {
                if (
                    viewer.IsMiniature
                    && viewer.IsLayerMiniature
                    && viewer.DrawingContext.CurrentLayer == layer
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
            //	Crée tous les calques miniatures.
            if (this.parentPanel.Window == null) // initialisation du logiciel ?
            {
                return;
            }

            double offsetY = this.scrollable.ViewportOffsetY;
            this.Clear(); // supprime les miniatures existantes

            if (!this.isPanelShowed) // panneau caché ?
            {
                return;
            }

            double zoom = (double)this.slider.Value;
            int currentPage = this.CurrentPage;
            int currentLayer = this.CurrentLayer;
            Size pageSize = this.document.GetPageSize(currentPage);
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
            double posY = 0;
            double requiredWidth = 0;
            List<Viewer> viewers = new List<Viewer>();

            foreach (int layer in this.layers) // layer = [n-1..0]
            {
                Objects.Layer objectLayer = this.GetLayer(layer);
                if (objectLayer == null)
                {
                    continue;
                }

                if (this.document.Type == DocumentType.Pictogram)
                {
                    objectLayer.CacheBitmapSize = new Size(pageSize.Width + 2, pageSize.Height + 2);
                }
                else
                {
                    objectLayer.CacheBitmapSize = new Size(w, h);
                }

                string layerName = this.LayerName(layer);

                Widgets.MiniatureFrame box = new Widgets.MiniatureFrame(this.scrollable.Viewport);
                box.IsLeftRightPlacement = false;
                box.Index = layer;
                box.PreferredSize = new Size(w + 4, h + 4 + LayerMiniatures.labelHeight);
                box.Padding = new Margins(2, 2, 2, 2);
                box.Anchor = AnchorStyles.TopLeft;
                box.Margins = new Margins(0, 0, posY, 0);
                box.Clicked += this.HandleLayerBoxClicked;
                box.DragAndDropDoing += this.HandleDragAndDropDoing;
                ToolTip.Default.SetToolTip(
                    box,
                    string.Format(Res.Strings.Container.LayerMiniatures.Box.Tooltip, layerName)
                );

                //	Crée la vue du calque miniature.
                Viewer viewer = new Viewer(this.document);
                viewer.SetParent(box);
                viewer.Dock = DockStyle.Fill;
                viewer.PreferredSize = new Size(w, h);
                if (this.document.Type == DocumentType.Pictogram)
                {
                    viewer.PictogramMiniatureZoom = zoom;
                }
                viewer.IsMiniature = true;
                viewer.IsLayerMiniature = true;
                viewer.PaintPageFrame = false;
                viewer.DrawingContext.LayerDrawingMode = LayerDrawingMode.HideInactive;
                viewer.DrawingContext.InternalPageLayer(currentPage, layer);
                viewer.DrawingContext.GridShow = false;
                viewer.DrawingContext.GuidesShow = false;
                viewer.DrawingContext.TextShowControlCharacters = false;

                //	Crée la légende en bas, avec le lettre+position du calque.
                StaticText label = new StaticText(box);
                label.PreferredSize = new Size(w, LayerMiniatures.labelHeight);
                label.Dock = DockStyle.Bottom;
                label.ContentAlignment = ContentAlignment.TopCenter;
                label.Text = Misc.FontSize(layerName, 0.75);

                if (layer == currentLayer)
                {
                    box.BackColor = viewer.DrawingContext.HiliteOutlineColor;
                }

                posY += h + LayerMiniatures.labelHeight + 2;
                requiredWidth = System.Math.Max(requiredWidth, w);
                viewers.Add(viewer);
            }

            this.scrollable.ViewportOffsetY = offsetY;
            this.parentPanel.PreferredWidth = requiredWidth + 3 + 23;
            this.parentPanel.Window.ForceLayout();

            foreach (Viewer viewer in viewers)
            {
                viewer.DrawingContext.ZoomPageAndCenter();
                this.document.Modifier.AttachViewer(viewer);
                this.document.Notifier.NotifyArea(viewer);
            }
        }

        protected void Clear()
        {
            //	Supprime toutes les miniatures des calques.
            List<Viewer> viewers = new List<Viewer>();
            foreach (Viewer viewer in this.document.Modifier.Viewers)
            {
                if (viewer.IsMiniature && viewer.IsLayerMiniature) // miniature d'un calque ?
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

        private void HandleLayerBoxClicked(object sender, MessageEventArgs e)
        {
            //	Un calque miniature a été cliqué.
            Widgets.MiniatureFrame box = sender as Widgets.MiniatureFrame;
            this.CurrentLayer = box.Index; // rend le calque cliqué actif
        }

        private void HandleDragAndDropDoing(object sender, Widgets.MiniatureFrameEventArgs dst)
        {
            //	Un calque a été draggé sur un autre.
            Widgets.MiniatureFrame src = sender as Widgets.MiniatureFrame;

            int layer = dst.Frame.Index;

            if (!dst.Frame.IsBefore)
            {
                layer++;
            }

            if (!src.IsDuplicate && dst.Frame.Index > src.Index)
            {
                layer--;
            }

            if (src.IsDuplicate)
            {
                this.document.Modifier.LayerDuplicate(src.Index, layer, "");
            }
            else
            {
                this.document.Modifier.LayerSwap(src.Index, layer);
            }

            this.CurrentLayer = layer;
        }

        protected string LayerName(int rank)
        {
            //	Retourne le nom le plus complet possible du calque, constitué de la lettre
            //	suivi de sa position ou de son nom.
            Objects.Layer layer = this.GetLayer(rank);
            if (layer == null)
            {
                return null;
            }

            string layerName = layer.Name;
            if (string.IsNullOrEmpty(layerName))
            {
                return string.Concat(
                    "<b>",
                    Objects.Layer.ShortName(rank),
                    "</b> ",
                    Objects.Layer.LayerPositionName(rank, this.TotalLayers)
                );
            }
            else
            {
                return string.Concat("<b>", Objects.Layer.ShortName(rank), "</b> ", layerName);
            }
        }

        protected int TotalLayers
        {
            //	Retourne le nombre total de calques.
            get { return this.document.Modifier.ActiveViewer.DrawingContext.TotalLayers(); }
        }

        protected int CurrentPage
        {
            //	Retourne le rang de la page courante.
            get { return this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage; }
        }

        protected int CurrentLayer
        {
            //	Retourne le rang du calque courant.
            get { return this.document.Modifier.ActiveViewer.DrawingContext.CurrentLayer; }
            set { this.document.Modifier.ActiveViewer.DrawingContext.CurrentLayer = value; }
        }

        protected Objects.Layer GetLayer(int rank)
        {
            //	Retourne un objet Layer du document.
            NewUndoableList doc = this.document.DocumentObjects;
            Objects.Page page = doc[this.CurrentPage] as Objects.Page;

            if (rank < page.Objects.Count)
            {
                return page.Objects[rank] as Objects.Layer;
            }
            else
            {
                return null;
            }
        }

        protected static readonly double labelHeight = 12;

        protected Document document;
        protected List<int> layers;
        protected List<int> layersToRegenerate;
        protected FrameBox parentPanel;
        protected HSlider slider;
        protected Scrollable scrollable;
        protected bool isPanelShowed;
    }
}
