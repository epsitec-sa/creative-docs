using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Double contient un r�glage num�rique.
	/// </summary>
	[System.Serializable()]
	public class Double : Abstract
	{
		public Double(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			this.conditionName = "";
			this.conditionState = false;
			this.suffix = "";

			switch ( this.name )
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

				case "ExportPDFDebord":
					this.text = Res.Strings.Dialog.Double.ExportPDFDebord;
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
					this.factorMaxValue = 600.0;
					this.factorStep = 1.0;
					break;

				case "ExportPDFImageMaxDpi":
					this.text = Res.Strings.Dialog.Double.ExportPDFImageMaxDpi;
					this.integer = true;
					this.factorMinValue = 0.0;
					this.factorMaxValue = 600.0;
					this.factorStep = 1.0;
					break;

				case "ImageDpi":
					this.text = Res.Strings.Dialog.Double.ImageDpi;
					this.integer = true;
					this.info = true;
					this.factorMinValue = 10.0;
					this.factorMaxValue = 600.0;
					this.factorResolution = 0.1;
					this.factorStep = 0.1;
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
			}
		}

		public double Value
		{
			get
			{
				switch ( this.name )
				{
					case "OutsideArea":
						return this.document.Modifier.OutsideArea;

					case "PrintCopies":
						return this.document.Settings.PrintInfo.Copies;

					case "PrintDpi":
						return this.document.Settings.PrintInfo.Dpi;

					case "PrintMargins":
						return this.document.Settings.PrintInfo.Margins;

					case "PrintDebord":
						return this.document.Settings.PrintInfo.Debord;

					case "ExportPDFDebord":
						return this.document.Settings.ExportPDFInfo.Debord;

					case "ExportPDFJpegQuality":
						return this.document.Settings.ExportPDFInfo.JpegQuality*100.0;

					case "ExportPDFImageMinDpi":
						return this.document.Settings.ExportPDFInfo.ImageMinDpi;

					case "ExportPDFImageMaxDpi":
						return this.document.Settings.ExportPDFInfo.ImageMaxDpi;

					case "ImageDpi":
						return this.document.Printer.ImageDpi;

					case "ImageQuality":
						return this.document.Printer.ImageQuality*100.0;

					case "ImageAA":
						return this.document.Printer.ImageAA*100.0;

					case "ArrowMoveMul":
						return this.document.Modifier.ArrowMoveMul;

					case "ArrowMoveDiv":
						return this.document.Modifier.ArrowMoveDiv;

					case "ToLinePrecision":
						return this.document.Modifier.ToLinePrecision*100.0;

					case "DimensionScale":
						return this.document.Modifier.DimensionScale*100.0;

					case "DimensionDecimal":
						return this.document.Modifier.DimensionDecimal;
				}

				return 0.0;
			}

			set
			{
				switch ( this.name )
				{
					case "OutsideArea":
						this.document.Modifier.OutsideArea = value;
						break;

					case "PrintCopies":
						this.document.Settings.PrintInfo.Copies = (int) value;
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

					case "ExportPDFDebord":
						this.document.Settings.ExportPDFInfo.Debord = value;
						break;

					case "ExportPDFJpegQuality":
						this.document.Settings.ExportPDFInfo.JpegQuality = value/100.0;
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
						this.document.Printer.ImageQuality = value/100.0;
						break;

					case "ImageAA":
						this.document.Printer.ImageAA = value/100.0;
						break;

					case "ArrowMoveMul":
						this.document.Modifier.ArrowMoveMul = value;
						break;

					case "ArrowMoveDiv":
						this.document.Modifier.ArrowMoveDiv = value;
						break;

					case "ToLinePrecision":
						this.document.Modifier.ToLinePrecision = value/100.0;
						break;

					case "DimensionScale":
						this.document.Modifier.DimensionScale = value/100.0;
						break;

					case "DimensionDecimal":
						this.document.Modifier.DimensionDecimal = value;
						break;
				}
			}
		}

		public double FactorMinValue
		{
			get
			{
				return this.factorMinValue;
			}
		}

		public double FactorMaxValue
		{
			get
			{
				return this.factorMaxValue;
			}
		}

		public double FactorResolution
		{
			get
			{
				return this.factorResolution;
			}
		}

		public double FactorStep
		{
			get
			{
				return this.factorStep;
			}
		}

		public bool Integer
		{
			get
			{
				return this.integer;
			}
		}

		public bool Info
		{
			get
			{
				return this.info;
			}
		}

		public string Suffix
		{
			get
			{
				return this.suffix;
			}
		}


		public bool IsEnabled
		{
			get
			{
				bool enabled = true;

				if ( this.name == "ImageQuality" )
				{
					enabled = (this.document.Printer.ImageFormat == ImageFormat.Jpeg);
				}

				if ( this.name == "ExportPDFJpegQuality" )
				{
					enabled = (this.document.Settings.ExportPDFInfo.ImageCompression == PDF.ImageCompression.JPEG);
				}

				return enabled;
			}
		}

		public string GetInfo()
		{
			string text = "";

			if ( this.name == "ImageDpi" )
			{
				double dpi = this.document.Printer.ImageDpi;
				int dx = (int) ((this.document.Size.Width/10.0)*(dpi/25.4));
				int dy = (int) ((this.document.Size.Height/10.0)*(dpi/25.4));
				text = string.Format("{0} x {1} pixels", dx, dy);
			}

			if ( this.name == "ImageQuality"         ||
				 this.name == "ExportPDFJpegQuality" )
			{
				double quality = (this.name == "ImageQuality") ? this.document.Printer.ImageQuality : this.document.Settings.ExportPDFInfo.JpegQuality;
				if ( quality == 0.0 )
				{
					text = Res.Strings.Dialog.Double.ImageQuality1;
				}
				else if ( quality < 0.3 )
				{
					text = Res.Strings.Dialog.Double.ImageQuality2;
				}
				else if ( quality < 0.7 )
				{
					text = Res.Strings.Dialog.Double.ImageQuality3;
				}
				else if ( quality < 1.0 )
				{
					text = Res.Strings.Dialog.Double.ImageQuality4;
				}
				else
				{
					text = Res.Strings.Dialog.Double.ImageQuality5;
				}
			}

			if ( this.name == "ImageAA" )
			{
				double aa = this.document.Printer.ImageAA;
				if ( aa == 0.0 )
				{
					text = Res.Strings.Dialog.Double.ImageAA1;
				}
				else if ( aa < 0.3 )
				{
					text = Res.Strings.Dialog.Double.ImageAA2;
				}
				else if ( aa < 0.7 )
				{
					text = Res.Strings.Dialog.Double.ImageAA3;
				}
				else if ( aa < 1.0 )
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
		// S�rialise le r�glage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui d�s�rialise le r�glage.
		protected Double(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetDouble("Value");
			this.Initialise();
		}
		#endregion


		protected double			factorMinValue = -1.0;
		protected double			factorMaxValue = 1.0;
		protected double			factorResolution = 1.0;
		protected double			factorStep = 1.0;
		protected bool				integer = false;
		protected bool				info = false;
		protected string			suffix;
	}
}
