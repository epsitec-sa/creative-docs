using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Double contient un réglage numérique.
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

			switch ( this.name )
			{
				case "PrintDpi":
					this.text = "Résolution (dpi)";
					this.conditionName = "PrintDraft";
					this.conditionState = true;
					this.integer = true;
					this.factorMinValue = 150.0;
					this.factorMaxValue = 600.0;
					this.factorStep = 50.0;
					break;

				case "PrintMargins":
					this.text = "Marges de centrage";
					this.conditionName = "PrintAutoZoom";
					this.conditionState = true;
					this.factorMinValue = 0.0;
					this.factorMaxValue = 0.1;
					this.factorResolution = 0.1;
					this.factorStep = 1.0;
					break;

				case "PrintDebord":
					this.text = "Débord";
					this.conditionName = "PrintAutoZoom";
					this.conditionState = true;
					this.factorMinValue = 0.0;
					this.factorMaxValue = 0.1;
					this.factorResolution = 0.1;
					this.factorStep = 1.0;
					break;

				case "ImageDpi":
					this.text = "Résolution (dpi)";
					this.integer = true;
					this.info = true;
					this.factorMinValue = 10.0;
					this.factorMaxValue = 600.0;
					this.factorResolution = 0.1;
					this.factorStep = 0.1;
					break;

				case "ImageQuality":
					this.text = "Qualité de l'image";
					this.integer = true;
					this.info = true;
					this.factorMinValue = 0.0;
					this.factorMaxValue = 100.0;
					this.factorStep = 5.0;
					break;

				case "ImageAA":
					this.text = "Anti-crénelage";
					this.integer = true;
					this.info = true;
					this.factorMinValue = 0.0;
					this.factorMaxValue = 100.0;
					this.factorStep = 10.0;
					break;

				case "ArrowMoveMul":
					this.text = "Multiplicateur si Shift";
					this.integer = true;
					this.factorMinValue = 1.1;
					this.factorMaxValue = 20.0;
					this.factorStep = 1.0;
					this.factorResolution = 0.1;
					break;

				case "ArrowMoveDiv":
					this.text = "Diviseur si Ctrl";
					this.integer = true;
					this.factorMinValue = 1.1;
					this.factorMaxValue = 20.0;
					this.factorStep = 1.0;
					this.factorResolution = 0.1;
					break;

				case "ToLinePrecision":
					this.text = "Précision";
					this.integer = true;
					this.factorMinValue = 0.0;
					this.factorMaxValue = 100.0;
					this.factorStep = 10.0;
					break;
			}
		}

		public double Value
		{
			get
			{
				switch ( this.name )
				{
					case "PrintDpi":
						return this.document.Settings.PrintInfo.Dpi;

					case "PrintMargins":
						return this.document.Settings.PrintInfo.Margins;

					case "PrintDebord":
						return this.document.Settings.PrintInfo.Debord;

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
				}

				return 0.0;
			}

			set
			{
				switch ( this.name )
				{
					case "PrintDpi":
						this.document.Settings.PrintInfo.Dpi = value;
						break;

					case "PrintMargins":
						this.document.Settings.PrintInfo.Margins = value;
						break;

					case "PrintDebord":
						this.document.Settings.PrintInfo.Debord = value;
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


		public bool IsEnabled
		{
			get
			{
				bool enabled = true;

				if ( this.name == "ImageQuality" )
				{
					enabled = (this.document.Printer.ImageFormat == ImageFormat.Jpeg);
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

			if ( this.name == "ImageQuality" )
			{
				double quality = this.document.Printer.ImageQuality;
				if ( quality == 0.0 )
				{
					text = "Qualité minimale, taille faible";
				}
				else if ( quality < 0.3 )
				{
					text = "Basse qualité, taille faible";
				}
				else if ( quality < 0.7 )
				{
					text = "Qualité moyenne, taille standard";
				}
				else if ( quality < 1.0 )
				{
					text = "Bonne qualité, taille importante";
				}
				else
				{
					text = "Qualité maximale, taille importante";
				}
			}

			if ( this.name == "ImageAA" )
			{
				double aa = this.document.Printer.ImageAA;
				if ( aa == 0.0 )
				{
					text = "Escaliers importants (tout ou rien)";
				}
				else if ( aa < 0.3 )
				{
					text = "Escaliers prononcés";
				}
				else if ( aa < 0.7 )
				{
					text = "Adoucissement moyen";
				}
				else if ( aa < 1.0 )
				{
					text = "Adoucissement important";
				}
				else
				{
					text = "Adoucissement maximal (standard)";
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
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
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
	}
}
