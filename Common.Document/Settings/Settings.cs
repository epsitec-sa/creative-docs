using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Epsitec.Common.Support.Serialization;

namespace Epsitec.Common.Document.Settings
{
    /// <summary>
    /// La classe Settings contient tous les réglages.
    /// </summary>
    [System.Serializable()]
    public class Settings : ISerializable, Support.IXMLSerializable<Settings>
    {
        public Settings(Document document)
        {
            this.document = document;

            // bl-net8-cross refactor
            // Use a hashtable instead of a list for the settings
            this.settings = new();
            this.settings = new();
            this.CreateDefault();

            this.globalGuides = true;
            this.guides = new NewUndoableList(this.document, UndoableListType.Guides);

            this.quickFonts = new();
            Settings.DefaultQuickFonts(this.quickFonts);

            this.drawingSettings = new DrawingSettings(document);
            this.printInfo = new PrintInfo(document);
            this.exportPDFInfo = new ExportPDFInfo(document);
            this.exportICOInfo = new ExportICOInfo(document);
        }

        public void Dispose() { }

        protected void CreateDefault()
        {
            //	Crée tous les réglages par défaut, si nécessaire.
            //	Il est possible d'en ajouter de nouveaux tout en restant compatible
            //	avec les anciens fichiers sérialisés.
            this.owners = new System.Collections.Hashtable();

            this.CreateDefaultPoint("Settings", "PageSize");
            this.CreateDefaultDouble("Settings", "OutsideArea");

            this.CreateDefaultBool("Settings", "GridActive");
            this.CreateDefaultBool("Settings", "GridShow");
            this.CreateDefaultPoint("Settings", "GridStep");
            this.CreateDefaultPoint("Settings", "GridSubdiv");
            this.CreateDefaultPoint("Settings", "GridOffset");

            this.CreateDefaultBool("Settings", "TextGridShow");
            this.CreateDefaultDouble("Settings", "TextGridStep");
            this.CreateDefaultDouble("Settings", "TextGridSubdiv");
            this.CreateDefaultDouble("Settings", "TextGridOffset");
            this.CreateDefaultBool("Settings", Commands.TextShowControlCharacters);
            this.CreateDefaultBool("Settings", "TextFontFilter");
            this.CreateDefaultBool("Settings", "TextFontSampleAbc");
            this.CreateDefaultDouble("Settings", "TextFontSampleHeight");

            this.CreateDefaultBool("Settings", "GuidesActive");
            this.CreateDefaultBool("Settings", "GuidesShow");
            this.CreateDefaultBool("Settings", "GuidesMouse");

            this.CreateDefaultBool("", "PreviewActive");
            this.CreateDefaultBool("", "RulersShow");
            this.CreateDefaultBool("", "LabelsShow");
            this.CreateDefaultBool("", "MagnetActive");

            this.CreateDefaultPoint("Settings", "DuplicateMove");
            this.CreateDefaultBool("Settings", "RepeatDuplicateMove");
            this.CreateDefaultPoint("Settings", "ArrowMove");
            this.CreateDefaultDouble("Settings", "ArrowMoveMul");
            this.CreateDefaultDouble("Settings", "ArrowMoveDiv");
            this.CreateDefaultInteger("Settings", "ConstrainAngle");
            this.CreateDefaultDouble("Settings", "ToLinePrecision");
            this.CreateDefaultBool("Settings", "ImageAlphaCorrect");
            this.CreateDefaultBool("Settings", "ImageAlphaPremultiplied");
            this.CreateDefaultInteger("Settings", "DefaultUnit");
            this.CreateDefaultDouble("Settings", "DimensionScale");
            this.CreateDefaultDouble("Settings", "DimensionDecimal");

            this.CreateDefaultString("Print", "PrintName");
            this.CreateDefaultRange("Print", "PrintRange");
            this.CreateDefaultInteger("Print", "PrintArea");
            this.CreateDefaultDouble("Print", "PrintCopies");
            this.CreateDefaultBool("Print", "PrintCollate");
            this.CreateDefaultBool("Print", "PrintReverse");
            this.CreateDefaultBool("Print", "PrintToFile");
            this.CreateDefaultString("Print", "PrintFilename");
            this.CreateDefaultBool("Print", "PrintAutoLandscape");
            this.CreateDefaultBool("Print", "PrintAutoZoom");
            this.CreateDefaultBool("Print", "PrintDraft");
            this.CreateDefaultBool("Print", "PrintAA");
            this.CreateDefaultBool("Print", "PrintPerfectJoin");
            this.CreateDefaultBool("Print", "PrintDebugArea");
            this.CreateDefaultDouble("Print", "PrintDpi");
            this.CreateDefaultInteger("Print", "PrintCentring");
            this.CreateDefaultDouble("Print", "PrintMargins");
            this.CreateDefaultDouble("Print", "PrintDebord");
            this.CreateDefaultBool("Print", "PrintTarget");
            this.CreateDefaultInteger("Print", "PrintImageFilterA");
            this.CreateDefaultInteger("Print", "PrintImageFilterB");

            this.CreateDefaultBool("Export", "ImageOnlySelected");
            this.CreateDefaultInteger("Export", "ImageCrop");
            this.CreateDefaultDouble("Export", "ImageDpi");
            this.CreateDefaultInteger("Export", "ImageDepth");
            this.CreateDefaultInteger("Export", "ImageCompression");
            this.CreateDefaultDouble("Export", "ImageQuality");
            this.CreateDefaultDouble("Export", "ImageAA");
            this.CreateDefaultInteger("Export", "ImageFilterA");
            this.CreateDefaultInteger("Export", "ImageFilterB");

            this.CreateDefaultRange("ExportPDF", "ExportPDFRange");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleed");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedEvenTop");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedEvenBottom");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedEvenLeft");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedEvenRight");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedOddTop");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedOddBottom");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedOddLeft");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFBleedOddRight");
            this.CreateDefaultBool("ExportPDF", "ExportPDFTarget");
            this.CreateDefaultBool("ExportPDF", "ExportPDFTextCurve");
            this.CreateDefaultBool("ExportPDF", "ExportPDFExecute");
            this.CreateDefaultInteger("ExportPDF", "ExportPDFColorConversion");
            this.CreateDefaultInteger("ExportPDF", "ExportPDFImageCompression");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFJpegQuality");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFImageMinDpi");
            this.CreateDefaultDouble("ExportPDF", "ExportPDFImageMaxDpi");
            this.CreateDefaultInteger("ExportPDF", "ExportPDFImageFilterA");
            this.CreateDefaultInteger("ExportPDF", "ExportPDFImageFilterB");

            this.CreateDefaultInteger("ExportICO", "ExportICOFormat");
            this.CreateDefaultBool("ExportICO", "ExportICOOnlySelected");
            this.CreateDefaultInteger("ExportICO", "ExportICOCrop");
        }

        protected void CreateDefaultBool(string dialog, string name)
        {
            this.SetOwnerDialog(dialog, name);
            Bool sBool = this.Get(name) as Bool;
            if (sBool == null)
            {
                sBool = new Bool(this.document, name);
                this.settings.Add(sBool);
            }
        }

        protected void CreateDefaultInteger(string dialog, string name)
        {
            this.SetOwnerDialog(dialog, name);
            Integer sInteger = this.Get(name) as Integer;
            if (sInteger == null)
            {
                sInteger = new Integer(this.document, name);
                this.settings.Add(sInteger);
            }
        }

        protected void CreateDefaultDouble(string dialog, string name)
        {
            this.SetOwnerDialog(dialog, name);
            Double sDouble = this.Get(name) as Double;
            if (sDouble == null)
            {
                sDouble = new Double(this.document, name);
                this.settings.Add(sDouble);
            }
        }

        protected void CreateDefaultString(string dialog, string name)
        {
            this.SetOwnerDialog(dialog, name);
            String sString = this.Get(name) as String;
            if (sString == null)
            {
                sString = new String(this.document, name);
                this.settings.Add(sString);
            }
        }

        protected void CreateDefaultPoint(string dialog, string name)
        {
            this.SetOwnerDialog(dialog, name);
            Point sPoint = this.Get(name) as Point;
            if (sPoint == null)
            {
                sPoint = new Point(this.document, name);
                this.settings.Add(sPoint);
            }
        }

        protected void CreateDefaultRange(string dialog, string name)
        {
            this.SetOwnerDialog(dialog, name);
            Range sRange = this.Get(name) as Range;
            if (sRange == null)
            {
                sRange = new Range(this.document, name);
                this.settings.Add(sRange);
            }
        }

        protected void SetOwnerDialog(string dialog, string name)
        {
            //	Spécifie quel est le dialogue propriétaire d'un réglage.
            if (dialog == "")
                return;
            this.owners.Add(name, dialog);
        }

        public string GetOwnerDialog(string name)
        {
            //	Indique à quel dialogue appartient un réglage.
            string dialog = this.owners[name] as string;
            if (dialog == null)
                return "";
            return dialog;
        }

        public DrawingSettings DrawingSettings
        {
            get { return this.drawingSettings; }
        }

        public PrintInfo PrintInfo
        {
            //	Donne les réglages de l'impression.
            get { return this.printInfo; }
        }

        public ExportPDFInfo ExportPDFInfo
        {
            //	Donne les réglages de la publication PDF.
            get { return this.exportPDFInfo; }
        }

        public ExportICOInfo ExportICOInfo
        {
            //	Donne les réglages de la publication ICO.
            get { return this.exportICOInfo; }
        }

        public void Reset()
        {
            //	Remets tous les réglages par défaut.
            this.GuidesReset();
        }

        public int Count
        {
            //	Nombre total de réglages.
            get { return this.settings.Count; }
        }

        public Abstract Get(int index)
        {
            //	Donne un réglage d'après son index.
            return this.settings[index];
        }

        public Abstract Get(string name)
        {
            //	Donne un réglage d'après son nom.
            foreach (Abstract settings in this.settings)
            {
                if (settings.Name == name)
                    return settings;
            }
            return null;
        }

        #region Guides
        public bool GlobalGuides
        {
            //	Utilise le guides globaux ou locaux à la page courante.
            get { return this.globalGuides; }
            set
            {
                if (this.globalGuides != value)
                {
                    this.globalGuides = value;
                    this.document.Notifier.NotifyGuidesChanged();
                    this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
                }
            }
        }

        public int GuidesCount
        {
            //	Nombre total de guides.
            get { return this.GuidesList.Count; }
        }

        public int GuidesSelected
        {
            //	Guide sélectionné.
            get { return this.GuidesList.Selected; }
            set { this.GuidesList.Selected = value; }
        }

        public void GuidesReset()
        {
            //	Supprime tous les guides.
            if (!this.globalGuides || this.guides.Count > 0)
            {
                this.globalGuides = true;
                this.guides.Clear();
                this.document.Notifier.NotifyGuidesChanged();
                this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
                this.document.SetDirtySerialize(CacheBitmapChanging.None);
            }
        }

        public Guide GuidesGet(int index)
        {
            //	Donne un guide.
            return this.GuidesList[index] as Guide;
        }

        public int GuidesAdd(Guide guide)
        {
            //	Ajoute un nouveau guide.
            int index = this.GuidesList.Add(guide);
            this.document.Notifier.NotifyGuidesChanged();
            this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
            this.document.SetDirtySerialize(CacheBitmapChanging.None);
            return index;
        }

        public int GuidesAddOther(Guide guide)
        {
            //	Ajoute un nouveau guide dans l'autre (global/local) liste.
            int index = this.GuidesListOther.Add(guide);
            this.document.Notifier.NotifyGuidesChanged();
            this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
            this.document.SetDirtySerialize(CacheBitmapChanging.None);
            return index;
        }

        public void GuidesInsert(int index, Guide guide)
        {
            //	Ajoute un nouveau guide.
            this.GuidesList.Insert(index, guide);
            this.document.Notifier.NotifyGuidesChanged();
            this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
            this.document.SetDirtySerialize(CacheBitmapChanging.None);
        }

        public void GuidesRemoveAt(int index)
        {
            //	Supprime un guide.
            this.GuidesList.RemoveAt(index);
            this.document.Notifier.NotifyGuidesChanged();
            this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
            this.document.SetDirtySerialize(CacheBitmapChanging.None);
        }

        protected NewUndoableList GuidesList
        {
            //	Retourne la liste des repères.
            get
            {
                if (this.globalGuides)
                {
                    return this.guides;
                }
                else
                {
                    int cp = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
                    Objects.Page page = this.document.DocumentObjects[cp] as Objects.Page;
                    return page.Guides;
                }
            }
        }

        protected NewUndoableList GuidesListOther
        {
            //	Retourne l'autre (global/local) liste des repères.
            get
            {
                if (!this.globalGuides)
                {
                    return this.guides;
                }
                else
                {
                    int cp = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
                    Objects.Page page = this.document.DocumentObjects[cp] as Objects.Page;
                    return page.Guides;
                }
            }
        }

        public NewUndoableList GuidesListGlobal
        {
            //	Retourne la liste des repères globaux.
            get { return this.guides; }
        }
        #endregion


        #region QuickFonts
        public List<string> QuickFonts
        {
            //	Liste des polices rapides.
            get { return this.quickFonts; }
            set { this.quickFonts = value; }
        }

        public static void DefaultQuickFonts(List<string> list)
        {
            //	Donne la liste des polices rapides par défaut.
            list.Clear();

            list.Add("Arial");
            list.Add("Courier New");
            //?list.Add("Tahoma");
            list.Add("Times New Roman");
        }
        #endregion


        #region Serialization
        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            Settings otherSettings = (Settings)other;
            List<bool> checks =
            [
                otherSettings.settings.HasEquivalentData(this.settings),
                otherSettings.globalGuides == this.globalGuides,
                otherSettings.guides.HasEquivalentData(this.guides),
                otherSettings.quickFonts.SequenceEqual(this.quickFonts),
                otherSettings.printInfo.HasEquivalentData(this.printInfo),
                otherSettings.exportPDFInfo.HasEquivalentData(this.exportPDFInfo),
                otherSettings.exportICOInfo.HasEquivalentData(this.exportICOInfo)
            ];
            if (!checks.All(x => x))
            {
                return false;
            }
            return true;
        }

        public XElement ToXML()
        {
            return new XElement(
                "Settings",
                new XAttribute("GlobalGuides", this.globalGuides),
                new XElement("Settings", this.settings.Select(item => item.ToXML())),
                new XElement(
                    "QuickFonts",
                    this.quickFonts.Select(fontname => new XElement(
                        "Font",
                        new XAttribute("Name", fontname)
                    ))
                ),
                new XElement("Guides", this.guides.ToXML()),
                this.printInfo.ToXML(),
                this.exportPDFInfo.ToXML(),
                this.exportICOInfo.ToXML()
            );
        }

        public static Settings FromXML(XElement xml)
        {
            return new Settings(xml);
        }

        private Settings(XElement xml)
        {
            this.document = Document.ReadDocument;
            this.drawingSettings = new DrawingSettings(this.document);
            this.settings = xml.Element("Settings")
                .Elements()
                .Select(Settings.LoadSettingFromXML)
                .ToList();
            this.globalGuides = bool.Parse(xml.Attribute("GlobalGuides").Value);
            this.guides = NewUndoableList.FromXML(xml.Element("Guides"));
            this.quickFonts = xml.Element("QuickFonts")
                .Elements()
                .Select(item => item.Attribute("Name").Value)
                .ToList();
            this.printInfo = PrintInfo.FromXML(xml.Element("PrintInfo"));
            this.exportPDFInfo = ExportPDFInfo.FromXML(xml.Element("ExportPDFInfo"));
            this.exportICOInfo = ExportICOInfo.FromXML(xml.Element("ExportICOInfo"));
        }

        private static Abstract LoadSettingFromXML(XElement xml)
        {
            switch (xml.Name.LocalName)
            {
                case "Bool":
                    return Bool.FromXML(xml);
                case "Double":
                    return Double.FromXML(xml);
                case "Integer":
                    return Integer.FromXML(xml);
                case "Point":
                    return Point.FromXML(xml);
                case "Range":
                    return Range.FromXML(xml);
                case "String":
                    return String.FromXML(xml);
                default:
                    throw new System.ArgumentException(
                        $"Unknown Setting type '{xml.Name.LocalName}'"
                    );
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise les réglages.
            info.AddValue("Settings", this.settings);
            info.AddValue("GlobalGuides", this.globalGuides);
            info.AddValue("GuidesList", this.guides);
            info.AddValue("QuickFonts", this.quickFonts);
            info.AddValue("PrintInfo", this.printInfo);
            info.AddValue("ExportPDFInfo", this.exportPDFInfo);
            info.AddValue("ExportICOInfo", this.exportICOInfo);
        }

        protected Settings(SerializationInfo info, StreamingContext context)
        {
            //	Constructeur qui désérialise les réglages.
            this.document = Document.ReadDocument;
            this.drawingSettings = new DrawingSettings(this.document);
            System.Collections.ArrayList settings = (System.Collections.ArrayList)
                info.GetValue("Settings", typeof(System.Collections.ArrayList));
            this.settings = settings.Cast<Abstract>().ToList();
            this.oldguides = (UndoableList)info.GetValue("GuidesList", typeof(UndoableList));
            this.printInfo = (PrintInfo)info.GetValue("PrintInfo", typeof(PrintInfo));

            if (this.document.IsRevisionGreaterOrEqual(1, 0, 10))
            {
                this.globalGuides = info.GetBoolean("GlobalGuides");
            }
            else
            {
                this.globalGuides = true;
            }

            if (this.document.IsRevisionGreaterOrEqual(1, 2, 5))
            {
                System.Collections.ArrayList quickFonts = (System.Collections.ArrayList)
                    info.GetValue("QuickFonts", typeof(System.Collections.ArrayList));
                this.quickFonts = quickFonts.Cast<string>().ToList();
            }
            else
            {
                this.quickFonts = new();
                Settings.DefaultQuickFonts(this.quickFonts);
            }

            if (this.document.IsRevisionGreaterOrEqual(1, 0, 21))
            {
                this.exportPDFInfo = (ExportPDFInfo)
                    info.GetValue("ExportPDFInfo", typeof(ExportPDFInfo));
            }
            else
            {
                this.exportPDFInfo = new ExportPDFInfo(this.document);
            }

            if (this.document.IsRevisionGreaterOrEqual(2, 0, 13))
            {
                this.exportICOInfo = (ExportICOInfo)
                    info.GetValue("ExportICOInfo", typeof(ExportICOInfo));
            }
            else
            {
                this.exportICOInfo = new ExportICOInfo(this.document);
            }
        }

        public void ReadFinalize()
        {
            //	Adapte l'objet après une désérialisation.
            this.guides = NewUndoableList.FromOld(this.oldguides);
            this.CreateDefault();
        }
        #endregion


        protected Document document;
        protected List<Abstract> settings;
        protected System.Collections.Hashtable owners;
        protected bool globalGuides;

        protected UndoableList oldguides;
        protected NewUndoableList guides;

        protected List<string> quickFonts;
        protected DrawingSettings drawingSettings;
        protected PrintInfo printInfo;
        protected ExportPDFInfo exportPDFInfo;
        protected ExportICOInfo exportICOInfo;
    }
}
