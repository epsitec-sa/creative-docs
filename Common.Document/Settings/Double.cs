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

using System.Runtime.Serialization;
using System.Xml.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Settings
{
    /// <summary>
    /// La classe Double contient un réglage numérique.
    /// </summary>
    [System.Serializable()]
    public class Double : Abstract, Support.IXMLSerializable<Double>
    {
        public Double(Document document, string name)
            : base(document, name)
        {
            this.Initialize();
        }

        protected void Initialize()
        {
            this.conditionName = "";
            this.conditionState = false;
            this.suffix = "";

            switch (this.name)
            {
                case "OutsideArea":
                    this.text = Res.Strings.Dialog.Double.OutsideArea;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 1.0;
                    this.factorResolution = 1.0;
                    this.factorStep = 1.0;
                    break;

                case "PrintCopies":
                    this.text = Res.Strings.Dialog.Double.PrintCopies;
                    this.integer = true;
                    this.factorMinValue = 1.0;
                    this.factorMaxValue = 100.0;
                    this.factorStep = 1.0;
                    break;

                case "PrintDpi":
                    this.text = Res.Strings.Dialog.Double.PrintDpi;
                    this.conditionName = "PrintDraft";
                    this.conditionState = true;
                    this.integer = true;
                    this.factorMinValue = 150.0;
                    this.factorMaxValue = 600.0;
                    this.factorStep = 50.0;
                    break;

                case "PrintMargins":
                    this.text = Res.Strings.Dialog.Double.PrintMargins;
                    this.conditionName = "PrintAutoZoom";
                    this.conditionState = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "PrintDebord":
                    this.text = Res.Strings.Dialog.Double.PrintDebord;
                    this.conditionName = "PrintAutoZoom";
                    this.conditionState = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleed":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleed;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedEvenTop":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedEvenTop;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedEvenBottom":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedEvenBottom;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedEvenLeft":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedEvenLeft;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedEvenRight":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedEvenRight;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedOddTop":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedOddTop;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedOddBottom":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedOddBottom;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedOddLeft":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedOddLeft;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFBleedOddRight":
                    this.text = Res.Strings.Dialog.Double.ExportPDFBleedOddRight;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 0.1;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFJpegQuality":
                    this.text = Res.Strings.Dialog.Double.ExportPDFJpegQuality;
                    this.integer = true;
                    this.info = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 100.0;
                    this.factorStep = 5.0;
                    this.suffix = "%";
                    break;

                case "ExportPDFImageMinDpi":
                    this.text = Res.Strings.Dialog.Double.ExportPDFImageMinDpi;
                    this.integer = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 2400.0;
                    this.factorStep = 1.0;
                    break;

                case "ExportPDFImageMaxDpi":
                    this.text = Res.Strings.Dialog.Double.ExportPDFImageMaxDpi;
                    this.integer = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 2400.0;
                    this.factorStep = 1.0;
                    break;

                case "ImageDpi":
                    this.text = Res.Strings.Dialog.Double.ImageDpi;
                    this.integer = true;
                    this.info = false;
                    this.factorMinValue = 10.0;
                    this.factorMaxValue =
                        (this.document.Type == DocumentType.Pictogram) ? 3000.0 : 600.0;
                    this.factorResolution = 0.01;
                    this.factorStep = 1.0;
                    break;

                case "ImageQuality":
                    this.text = Res.Strings.Dialog.Double.ImageQuality;
                    this.integer = true;
                    this.info = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 100.0;
                    this.factorStep = 5.0;
                    this.suffix = "%";
                    break;

                case "ImageAA":
                    this.text = Res.Strings.Dialog.Double.ImageAA;
                    this.integer = true;
                    this.info = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 100.0;
                    this.factorStep = 10.0;
                    this.suffix = "%";
                    break;

                case "ArrowMoveMul":
                    this.text = Res.Strings.Dialog.Double.ArrowMoveMul;
                    this.integer = true;
                    this.factorMinValue = 1.1;
                    this.factorMaxValue = 20.0;
                    this.factorStep = 1.0;
                    this.factorResolution = 1.0;
                    break;

                case "ArrowMoveDiv":
                    this.text = Res.Strings.Dialog.Double.ArrowMoveDiv;
                    this.integer = true;
                    this.factorMinValue = 1.1;
                    this.factorMaxValue = 20.0;
                    this.factorStep = 1.0;
                    this.factorResolution = 1.0;
                    break;

                case "ToLinePrecision":
                    this.text = Res.Strings.Dialog.Double.ToLinePrecision;
                    this.integer = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 200.0;
                    this.factorStep = 10.0;
                    this.suffix = "%";
                    break;

                case "DimensionScale":
                    this.text = Res.Strings.Dialog.Double.DimensionScale;
                    this.integer = true;
                    this.factorMinValue = 1.0;
                    this.factorMaxValue = 10000.0;
                    this.factorResolution = 0.1;
                    this.factorStep = 1.0;
                    this.suffix = "%";
                    break;

                case "DimensionDecimal":
                    this.text = Res.Strings.Dialog.Double.DimensionDecimal;
                    this.integer = true;
                    this.factorMinValue = 0.0;
                    this.factorMaxValue = 4.0;
                    this.factorStep = 1.0;
                    break;

                case "TextGridStep":
                    this.text = Res.Strings.TextPanel.Leading.Long.GridStep;
                    this.factorMinValue = 0.0001; // 0.1mm
                    this.factorMaxValue = 0.1; // 100mm
                    break;

                case "TextGridSubdiv":
                    this.text = Res.Strings.TextPanel.Leading.Long.GridSubdiv;
                    this.integer = true;
                    this.factorMinValue = 1.0;
                    this.factorMaxValue = 10.0;
                    break;

                case "TextGridOffset":
                    this.text = Res.Strings.TextPanel.Leading.Long.GridOffset;
                    this.factorMinValue = -0.1;
                    this.factorMaxValue = 0.1;
                    this.factorStep = 0.5;
                    break;

                case "TextFontSampleHeight":
                    this.text = Res.Strings.Dialog.Double.TextFontSampleHeight;
                    this.integer = true;
                    this.factorMinValue = 20;
                    this.factorMaxValue = 60;
                    break;
            }
        }

        public double Value
        {
            get
            {
                switch (this.name)
                {
                    case "OutsideArea":
                        return this.document.Modifier.OutsideArea;

                    case "PrintCopies":
                        //	Retourne toujours 1, afin que le dialogue d'impresion propose '1 copie'
                        //	chaque fois qu'il est ouvert. Ainsi, si l'utilisateur imprime 50 copies
                        //	une première fois, une deuxième impression remet le compteur à 1, afin
                        //	d'éviter un éventuel gaspillage de papier (idée de CN).
                        //return this.document.Settings.PrintInfo.Copies;
                        return 1;

                    case "PrintDpi":
                        return this.document.Settings.PrintInfo.Dpi;

                    case "PrintMargins":
                        return this.document.Settings.PrintInfo.Margins;

                    case "PrintDebord":
                        return this.document.Settings.PrintInfo.Debord;

                    case "ExportPDFBleed":
                        return this.document.Settings.ExportPDFInfo.BleedMargin;

                    case "ExportPDFBleedEvenTop":
                        return this.document.Settings.ExportPDFInfo.BleedEvenMargins.Top;

                    case "ExportPDFBleedEvenBottom":
                        return this.document.Settings.ExportPDFInfo.BleedEvenMargins.Bottom;

                    case "ExportPDFBleedEvenLeft":
                        return this.document.Settings.ExportPDFInfo.BleedEvenMargins.Left;

                    case "ExportPDFBleedEvenRight":
                        return this.document.Settings.ExportPDFInfo.BleedEvenMargins.Right;

                    case "ExportPDFBleedOddTop":
                        return this.document.Settings.ExportPDFInfo.BleedOddMargins.Top;

                    case "ExportPDFBleedOddBottom":
                        return this.document.Settings.ExportPDFInfo.BleedOddMargins.Bottom;

                    case "ExportPDFBleedOddLeft":
                        return this.document.Settings.ExportPDFInfo.BleedOddMargins.Left;

                    case "ExportPDFBleedOddRight":
                        return this.document.Settings.ExportPDFInfo.BleedOddMargins.Right;

                    case "ExportPDFJpegQuality":
                        return this.document.Settings.ExportPDFInfo.JpegQuality * 100.0;

                    case "ExportPDFImageMinDpi":
                        return this.document.Settings.ExportPDFInfo.ImageMinDpi;

                    case "ExportPDFImageMaxDpi":
                        return this.document.Settings.ExportPDFInfo.ImageMaxDpi;

                    case "ImageDpi":
                        return this.document.Printer.ImageDpi;

                    case "ImageQuality":
                        return this.document.Printer.ImageQuality * 100.0;

                    case "ImageAA":
                        return this.document.Printer.ImageAA * 100.0;

                    case "ArrowMoveMul":
                        return this.document.Modifier.ArrowMoveMul;

                    case "ArrowMoveDiv":
                        return this.document.Modifier.ArrowMoveDiv;

                    case "ToLinePrecision":
                        return this.document.Modifier.ToLinePrecision * 100.0;

                    case "DimensionScale":
                        return this.document.Modifier.DimensionScale * 100.0;

                    case "DimensionDecimal":
                        return this.document.Modifier.DimensionDecimal;

                    case "TextGridStep":
                        return this.document.Settings.DrawingSettings.TextGridStep;

                    case "TextGridSubdiv":
                        return this.document.Settings.DrawingSettings.TextGridSubdiv;

                    case "TextGridOffset":
                        return this.document.Settings.DrawingSettings.TextGridOffset;

                    case "TextFontSampleHeight":
                        return this.document.Settings.DrawingSettings.TextFontSampleHeight;
                }

                return 0.0;
            }
            set
            {
                switch (this.name)
                {
                    case "OutsideArea":
                        this.document.Modifier.OutsideArea = value;
                        break;

                    case "PrintCopies":
                        this.document.Settings.PrintInfo.Copies = (int)value;
                        break;

                    case "PrintDpi":
                        this.document.Settings.PrintInfo.Dpi = value;
                        break;

                    case "PrintMargins":
                        this.document.Settings.PrintInfo.Margins = value;
                        break;

                    case "PrintDebord":
                        this.document.Settings.PrintInfo.Debord = value;
                        break;

                    case "ExportPDFBleed":
                        this.document.Settings.ExportPDFInfo.BleedMargin = value;
                        this.document.Settings.ExportPDFInfo.CropMarksOffset = value + 10.0; // rajoute 1mm entre le débord et le trait de coupe
                        break;

                    case "ExportPDFBleedEvenTop":
                        this.document.Settings.ExportPDFInfo.BleedEvenMargins = new Margins(
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Left,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Right,
                            value,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Bottom
                        );
                        break;

                    case "ExportPDFBleedEvenBottom":
                        this.document.Settings.ExportPDFInfo.BleedEvenMargins = new Margins(
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Left,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Right,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Top,
                            value
                        );
                        break;

                    case "ExportPDFBleedEvenLeft":
                        this.document.Settings.ExportPDFInfo.BleedEvenMargins = new Margins(
                            value,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Right,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Top,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Bottom
                        );
                        break;

                    case "ExportPDFBleedEvenRight":
                        this.document.Settings.ExportPDFInfo.BleedEvenMargins = new Margins(
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Left,
                            value,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Top,
                            this.document.Settings.ExportPDFInfo.BleedEvenMargins.Bottom
                        );
                        break;

                    case "ExportPDFBleedOddTop":
                        this.document.Settings.ExportPDFInfo.BleedOddMargins = new Margins(
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Left,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Right,
                            value,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Bottom
                        );
                        break;

                    case "ExportPDFBleedOddBottom":
                        this.document.Settings.ExportPDFInfo.BleedOddMargins = new Margins(
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Left,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Right,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Top,
                            value
                        );
                        break;

                    case "ExportPDFBleedOddLeft":
                        this.document.Settings.ExportPDFInfo.BleedOddMargins = new Margins(
                            value,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Right,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Top,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Bottom
                        );
                        break;

                    case "ExportPDFBleedOddRight":
                        this.document.Settings.ExportPDFInfo.BleedOddMargins = new Margins(
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Left,
                            value,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Top,
                            this.document.Settings.ExportPDFInfo.BleedOddMargins.Bottom
                        );
                        break;

                    case "ExportPDFJpegQuality":
                        this.document.Settings.ExportPDFInfo.JpegQuality = value / 100.0;
                        break;

                    case "ExportPDFImageMinDpi":
                        this.document.Settings.ExportPDFInfo.ImageMinDpi = value;
                        break;

                    case "ExportPDFImageMaxDpi":
                        this.document.Settings.ExportPDFInfo.ImageMaxDpi = value;
                        break;

                    case "ImageDpi":
                        this.document.Printer.ImageDpi = value;
                        break;

                    case "ImageQuality":
                        this.document.Printer.ImageQuality = value / 100.0;
                        break;

                    case "ImageAA":
                        this.document.Printer.ImageAA = value / 100.0;
                        break;

                    case "ArrowMoveMul":
                        this.document.Modifier.ArrowMoveMul = value;
                        break;

                    case "ArrowMoveDiv":
                        this.document.Modifier.ArrowMoveDiv = value;
                        break;

                    case "ToLinePrecision":
                        this.document.Modifier.ToLinePrecision = value / 100.0;
                        break;

                    case "DimensionScale":
                        this.document.Modifier.DimensionScale = value / 100.0;
                        break;

                    case "DimensionDecimal":
                        this.document.Modifier.DimensionDecimal = value;
                        break;

                    case "TextGridStep":
                        this.document.Settings.DrawingSettings.TextGridStep = value;
                        break;

                    case "TextGridSubdiv":
                        this.document.Settings.DrawingSettings.TextGridSubdiv = value;
                        break;

                    case "TextGridOffset":
                        this.document.Settings.DrawingSettings.TextGridOffset = value;
                        break;

                    case "TextFontSampleHeight":
                        this.document.Settings.DrawingSettings.TextFontSampleHeight = value;
                        break;
                }
            }
        }

        public double FactorMinValue
        {
            get { return this.factorMinValue; }
        }

        public double FactorMaxValue
        {
            get { return this.factorMaxValue; }
        }

        public double FactorResolution
        {
            get { return this.factorResolution; }
        }

        public double FactorStep
        {
            get { return this.factorStep; }
        }

        public bool Integer
        {
            get { return this.integer; }
        }

        public bool Info
        {
            get { return this.info; }
        }

        public string Suffix
        {
            get { return this.suffix; }
        }

        public bool IsEnabled
        {
            get
            {
                bool enabled = true;

                if (this.name == "ImageQuality")
                {
                    enabled = (this.document.Printer.ImageFormat == ImageFormat.Jpeg);
                }

                if (this.name == "ExportPDFJpegQuality")
                {
                    enabled = (
                        this.document.Settings.ExportPDFInfo.ImageCompression
                        == PDF.ImageCompression.JPEG
                    );
                }

                return enabled;
            }
        }

        public string GetInfo()
        {
            string text = "";

            if (this.name == "ImageQuality" || this.name == "ExportPDFJpegQuality")
            {
                double quality =
                    (this.name == "ImageQuality")
                        ? this.document.Printer.ImageQuality
                        : this.document.Settings.ExportPDFInfo.JpegQuality;
                if (quality == 0.0)
                {
                    text = Res.Strings.Dialog.Double.ImageQuality1;
                }
                else if (quality < 0.3)
                {
                    text = Res.Strings.Dialog.Double.ImageQuality2;
                }
                else if (quality < 0.7)
                {
                    text = Res.Strings.Dialog.Double.ImageQuality3;
                }
                else if (quality < 1.0)
                {
                    text = Res.Strings.Dialog.Double.ImageQuality4;
                }
                else
                {
                    text = Res.Strings.Dialog.Double.ImageQuality5;
                }
            }

            if (this.name == "ImageAA")
            {
                double aa = this.document.Printer.ImageAA;
                if (aa == 0.0)
                {
                    text = Res.Strings.Dialog.Double.ImageAA1;
                }
                else if (aa < 0.3)
                {
                    text = Res.Strings.Dialog.Double.ImageAA2;
                }
                else if (aa < 0.7)
                {
                    text = Res.Strings.Dialog.Double.ImageAA3;
                }
                else if (aa < 1.0)
                {
                    text = Res.Strings.Dialog.Double.ImageAA4;
                }
                else
                {
                    text = Res.Strings.Dialog.Double.ImageAA5;
                }
            }

#if false
			if ( text != "" )
			{
				text = string.Format("<i>{0}</i>", text);
			}
#endif
            return text;
        }

        #region Serialization
        public new bool HasEquivalentData(Support.IXMLWritable other)
        {
            Double otherDouble = (Double)other;
            return base.HasEquivalentData(other) && this.Value == otherDouble.Value;
        }

        public override XElement ToXML()
        {
            return new XElement("Double", base.IterXMLParts(), new XAttribute("Value", this.Value));
        }

        public static Double FromXML(XElement xml)
        {
            return new Double(xml);
        }

        private Double(XElement xml)
            : base(xml)
        {
            this.Value = (double)xml.Attribute("Value");
            this.Initialize();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //	Sérialise le réglage.
            base.GetObjectData(info, context);
            info.AddValue("Value", this.Value);
        }

        protected Double(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //	Constructeur qui désérialise le réglage.
            this.Value = info.GetDouble("Value");
            this.Initialize();
        }
        #endregion


        protected double factorMinValue = -1.0;
        protected double factorMaxValue = 1.0;
        protected double factorResolution = 1.0;
        protected double factorStep = 1.0;
        protected bool integer = false;
        protected bool info = false;
        protected string suffix;
    }
}
