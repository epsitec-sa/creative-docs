namespace Epsitec.Common.Document.Settings
{
    public class DrawingSettings
    {
        public DrawingSettings(Document document)
        {
            this.document = document;
        }

        public bool GridActive
        {
            get => gridActive;
            set
            {
                gridActive = value;
                if (this.document.Notifier != null)
                {
                    this.document.Notifier.NotifyGridChanged();
                    this.SetDocumentDirtySerialize();
                }
            }
        }
        public bool GridShow
        {
            get => gridShow;
            set
            {
                gridShow = value;
                this.TryNotifyGridChanged();
            }
        }
        public Drawing.Point GridStep
        {
            get => gridStep;
            set
            {
                gridStep = value;
                this.TryNotifyGridChanged();
            }
        }
        public Drawing.Point GridSubdiv
        {
            get => gridSubdiv;
            set
            {
                gridSubdiv = value;
                this.TryNotifyGridChanged();
            }
        }
        public Drawing.Point GridOffset
        {
            get => gridOffset;
            set
            {
                gridOffset = value;
                this.TryNotifyGridChanged();
            }
        }
        public bool TextGridShow
        {
            get => textGridShow;
            set
            {
                textGridShow = value;
                this.TryNotifyGridChanged();
            }
        }
        public double TextGridStep
        {
            get => textGridStep;
            set
            {
                textGridStep = value;
                this.UpdateAllTextForTextGrid();
                this.TryNotifyGridChanged(allViewers: true);
            }
        }
        public double TextGridSubdiv
        {
            get => textGridSubdiv;
            set
            {
                textGridSubdiv = value;
                this.TryNotifyGridChanged();
            }
        }
        public double TextGridOffset
        {
            get => textGridOffset;
            set
            {
                textGridOffset = value;
                this.UpdateAllTextForTextGrid();
                this.TryNotifyGridChanged(allViewers: true);
            }
        }
        public bool TextFontFilter
        {
            get => textFontFilter;
            set
            {
                textFontFilter = value;
                this.SetDocumentDirtySerialize();
            }
        }
        public bool TextFontSampleAbc
        {
            get => textFontSampleAbc;
            set
            {
                textFontSampleAbc = value;
                this.SetDocumentDirtySerialize();
            }
        }
        public double TextFontSampleHeight
        {
            get => textFontSampleHeight;
            set
            {
                textFontSampleHeight = value;
                this.SetDocumentDirtySerialize();
            }
        }
        public bool TextShowControlCharacters
        {
            get => textShowControlCharacters;
            set
            {
                textShowControlCharacters = value;
                this.TryNotifyGridChanged(allViewers: true);
            }
        }
        public bool GuidesActive
        {
            get => guidesActive;
            set
            {
                guidesActive = value;
                this.SetDocumentDirtySerialize();
            }
        }
        public bool GuidesShow
        {
            get => guidesShow;
            set
            {
                guidesShow = value;
                this.TryNotifyGridChanged();
            }
        }
        public bool GuidesMouse
        {
            get => guidesMouse;
            set
            {
                guidesMouse = value;
                this.SetDocumentDirtySerialize();
            }
        }
        public bool MagnetActive
        {
            get => magnetActive;
            set
            {
                magnetActive = value;

                if (this.document.Notifier != null)
                {
                    this.document.Notifier.NotifyMagnetChanged();
                }
                this.SetDocumentDirtySerialize();
            }
        }
        public bool ConstrainActive
        {
            get => constrainActive;
            set => constrainActive = value;
        }
        public bool RulersShow
        {
            get => rulersShow;
            set
            {
                rulersShow = value;
                this.TryNotifyGridChanged();
            }
        }

        public bool PreviewActive
        {
            get => previewActive;
            set
            {
                previewActive = value;
                if (this.document.Notifier != null)
                {
                    this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
                    this.document.Notifier.NotifyPreviewChanged();
                }
            }
        }
        public bool LabelsShow
        {
            get => labelsShow;
            set
            {
                labelsShow = value;
                this.TryNotifyGridChanged();
            }
        }

        public bool AggregatesShow
        {
            get => aggregatesShow;
            set
            {
                aggregatesShow = value;
                this.TryNotifyGridChanged();
            }
        }
        public ConstrainAngle ConstrainAngle { get; set; }

        private void SetDocumentDirtySerialize()
        {
            Viewer viewer = this.document.Modifier.ActiveViewer;
            if (viewer != null && !viewer.IsMiniature)
            {
                this.document.SetDirtySerialize(CacheBitmapChanging.None);
            }
        }

        private void TryNotifyGridChanged(bool allViewers = false)
        {
            if (this.document.Notifier == null)
            {
                return;
            }
            if (allViewers)
            {
                this.document.Notifier.NotifyArea();
            }
            else
            {
                this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
            }
            this.document.Notifier.NotifyGridChanged();
            this.SetDocumentDirtySerialize();
        }

        protected void UpdateAllTextForTextGrid()
        {
            //	Met à jour tous les pavés du document lorsque les lignes magnétiques ont changé.
            foreach (TextFlow flow in this.document.TextFlows)
            {
                foreach (Objects.AbstractText obj in flow.Chain)
                {
                    Text.ITextFrame frame = obj.TextFrame as Text.ITextFrame;
                    if (frame != null)
                    {
                        obj.UpdateTextGrid(true);
                    }
                }
            }
        }

        private Document document;

        private bool gridActive;
        private bool gridShow;
        private Drawing.Point gridStep;
        private Drawing.Point gridSubdiv;
        private Drawing.Point gridOffset;
        private bool textGridShow;
        private double textGridStep;
        private double textGridSubdiv;
        private double textGridOffset;
        private bool textFontFilter;
        private bool textFontSampleAbc;
        private double textFontSampleHeight;
        private bool textShowControlCharacters;
        private bool guidesActive;
        private bool guidesShow;
        private bool guidesMouse;
        private bool magnetActive;
        private bool constrainActive;
        private bool rulersShow;
        private bool previewActive;
        private bool labelsShow;
        private bool aggregatesShow;
    }
}
