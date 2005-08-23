using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Integer contient un r�glage num�rique.
	/// </summary>
	[System.Serializable()]
	public class Integer : Abstract
	{
		public Integer(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			this.conditionName = "";
			this.conditionState = false;
			this.minValue = 0;
			this.maxValue = 100;
			this.step = 1;

			switch ( this.name )
			{
				case "DefaultUnit":
					this.text = Res.Strings.Dialog.Integer.DefaultUnit.Text;
					break;

				case "PrintArea":
					this.text = Res.Strings.Dialog.Integer.PrintArea.Text;
					break;

				case "PrintCentring":
					this.text = Res.Strings.Dialog.Integer.PrintCentring.Text;
					this.conditionName = "PrintAutoZoom";
					this.conditionState = true;
					break;

				case "ImageDepth":
					this.text = Res.Strings.Dialog.Integer.ImageDepth.Text;
					break;

				case "ImageCompression":
					this.text = Res.Strings.Dialog.Integer.ImageCompression.Text;
					break;

				case "ExportPDFColorConversion":
					this.text = Res.Strings.Dialog.Integer.ExportPDFColorConversion.Text;
					break;
			}
		}

		public int Value
		{
			get
			{
				switch ( this.name )
				{
					case "DefaultUnit":
						return (int) this.document.Modifier.RealUnitDimension;

					case "PrintArea":
						return (int) this.document.Settings.PrintInfo.PrintArea;

					case "PrintCentring":
						return (int) this.document.Settings.PrintInfo.Centring;

					case "ImageDepth":
						return this.document.Printer.ImageDepth;

					case "ImageCompression":
						return (int) this.document.Printer.ImageCompression;

					case "ExportPDFColorConversion":
						return (int) this.document.Settings.ExportPDFInfo.ColorConversion;
				}

				return 0;
			}

			set
			{
				switch ( this.name )
				{
					case "DefaultUnit":
						this.document.Modifier.RealUnitDimension = (RealUnitType) value;
						break;

					case "PrintArea":
						this.document.Settings.PrintInfo.PrintArea = (PrintArea) value;
						break;

					case "PrintCentring":
						this.document.Settings.PrintInfo.Centring = (PrintCentring) value;
						break;

					case "ImageDepth":
						this.document.Printer.ImageDepth = value;
						break;

					case "ImageCompression":
						this.document.Printer.ImageCompression = (ImageCompression) value;
						break;

					case "ExportPDFColorConversion":
						this.document.Settings.ExportPDFInfo.ColorConversion = (PDF.ColorConversion) value;
						break;
				}
			}
		}

		public int MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public int MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public int Step
		{
			get
			{
				return this.step;
			}
		}


		public void InitCombo(TextFieldCombo combo)
		{
			combo.Items.Clear();

			for ( int rank=0 ; rank<10 ; rank++ )
			{
				int type = this.RankToType(rank);
				if ( type == -1 )  break;
				combo.Items.Add(this.TypeToString(type));
			}

			if ( combo.Items.Count == 0 )
			{
				combo.SelectedIndex = 0;
			}
			else
			{
				int sel = this.TypeToRank(this.Value);
				if ( sel == -1 )
				{
					this.Value = this.RankToType(0);
					sel = this.TypeToRank(this.Value);
				}
				combo.SelectedIndex = sel;
			}

			combo.SetEnabled(combo.Items.Count > 1);
		}

		protected string TypeToString(int type)
		{
			switch ( this.name )
			{
				case "DefaultUnit":
					RealUnitType unit = (RealUnitType) type;
					if ( unit == RealUnitType.DimensionMillimeter )  return Res.Strings.Dialog.Integer.DefaultUnit.Millimeter;
					if ( unit == RealUnitType.DimensionCentimeter )  return Res.Strings.Dialog.Integer.DefaultUnit.Centimeter;
					if ( unit == RealUnitType.DimensionInch       )  return Res.Strings.Dialog.Integer.DefaultUnit.Inch;
					break;

				case "PrintArea":
					PrintArea pa = (PrintArea) type;
					if ( pa == PrintArea.All  )  return Res.Strings.Dialog.Integer.PrintArea.All;
					if ( pa == PrintArea.Even )  return Res.Strings.Dialog.Integer.PrintArea.Even;
					if ( pa == PrintArea.Odd  )  return Res.Strings.Dialog.Integer.PrintArea.Odd;
					break;

				case "PrintCentring":
					PrintCentring pc = (PrintCentring) type;
					if ( pc == PrintCentring.BottomLeft   )  return Res.Strings.Dialog.Integer.PrintCentring.BottomLeft;
					if ( pc == PrintCentring.BottomCenter )  return Res.Strings.Dialog.Integer.PrintCentring.BottomCenter;
					if ( pc == PrintCentring.BottomRight  )  return Res.Strings.Dialog.Integer.PrintCentring.BottomRight;
					if ( pc == PrintCentring.MiddleLeft   )  return Res.Strings.Dialog.Integer.PrintCentring.MiddleLeft;
					if ( pc == PrintCentring.MiddleCenter )  return Res.Strings.Dialog.Integer.PrintCentring.MiddleCenter;
					if ( pc == PrintCentring.MiddleRight  )  return Res.Strings.Dialog.Integer.PrintCentring.MiddleRight;
					if ( pc == PrintCentring.TopLeft      )  return Res.Strings.Dialog.Integer.PrintCentring.TopLeft;
					if ( pc == PrintCentring.TopCenter    )  return Res.Strings.Dialog.Integer.PrintCentring.TopCenter;
					if ( pc == PrintCentring.TopRight     )  return Res.Strings.Dialog.Integer.PrintCentring.TopRight;
					break;

				case "ImageDepth":
					if ( type ==  2 )  return Res.Strings.Dialog.Integer.ImageDepth.Bit2;
					if ( type ==  8 )  return Res.Strings.Dialog.Integer.ImageDepth.Bit8;
					if ( type == 16 )  return Res.Strings.Dialog.Integer.ImageDepth.Bit16;
					if ( type == 24 )  return Res.Strings.Dialog.Integer.ImageDepth.Bit24;
					if ( type == 32 )  return Res.Strings.Dialog.Integer.ImageDepth.Bit32;
					break;

				case "ImageCompression":
					ImageCompression ic = (ImageCompression) type;
					if ( ic == ImageCompression.None      )  return Res.Strings.Dialog.Integer.ImageCompression.None;
					if ( ic == ImageCompression.Lzw       )  return Res.Strings.Dialog.Integer.ImageCompression.Lzw;
					if ( ic == ImageCompression.Rle       )  return Res.Strings.Dialog.Integer.ImageCompression.Rle;
					if ( ic == ImageCompression.FaxGroup3 )  return Res.Strings.Dialog.Integer.ImageCompression.FaxGroup3;
					if ( ic == ImageCompression.FaxGroup4 )  return Res.Strings.Dialog.Integer.ImageCompression.FaxGroup4;
					break;

				case "ExportPDFColorConversion":
					PDF.ColorConversion cc = (PDF.ColorConversion) type;
					if ( cc == PDF.ColorConversion.None   )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.None;
					if ( cc == PDF.ColorConversion.ToRGB  )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.ToRGB;
					if ( cc == PDF.ColorConversion.ToCMYK )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.ToCMYK;
					if ( cc == PDF.ColorConversion.ToGray )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.ToGray;
					break;
			}
			return "";
		}

		public int TypeToRank(int type)
		{
			for ( int rank=0 ; rank<10 ; rank++ )
			{
				if ( this.RankToType(rank) == type )  return rank;
			}
			return -1;
		}

		public int RankToType(int rank)
		{
			switch ( this.name )
			{
				case "DefaultUnit":
					if ( rank == 0 )  return (int) RealUnitType.DimensionMillimeter;
					if ( rank == 1 )  return (int) RealUnitType.DimensionCentimeter;
					if ( rank == 2 )  return (int) RealUnitType.DimensionInch;
					return -1;

				case "PrintArea":
					if ( rank == 0 )  return (int) PrintArea.All;
					if ( rank == 1 )  return (int) PrintArea.Odd;
					if ( rank == 2 )  return (int) PrintArea.Even;
					return -1;

				case "PrintCentring":
					if ( rank == 0 )  return (int) PrintCentring.BottomLeft;
					if ( rank == 1 )  return (int) PrintCentring.BottomCenter;
					if ( rank == 2 )  return (int) PrintCentring.BottomRight;
					if ( rank == 3 )  return (int) PrintCentring.MiddleLeft;
					if ( rank == 4 )  return (int) PrintCentring.MiddleCenter;
					if ( rank == 5 )  return (int) PrintCentring.MiddleRight;
					if ( rank == 6 )  return (int) PrintCentring.TopLeft;
					if ( rank == 7 )  return (int) PrintCentring.TopCenter;
					if ( rank == 8 )  return (int) PrintCentring.TopRight;
					return -1;

				case "ImageDepth":
					return this.RankToTypeImageDepth(rank);

				case "ImageCompression":
					return this.RankToTypeImageCompression(rank);

				case "ExportPDFColorConversion":
					if ( rank == 0 )  return (int) PDF.ColorConversion.None;
					if ( rank == 1 )  return (int) PDF.ColorConversion.ToRGB;
					if ( rank == 2 )  return (int) PDF.ColorConversion.ToCMYK;
					if ( rank == 3 )  return (int) PDF.ColorConversion.ToGray;
					return -1;
			}
			return -1;
		}

		public int RankToTypeImageDepth(int rank)
		{
			switch ( this.document.Printer.ImageFormat )
			{
				case ImageFormat.Bmp:
					if ( rank == 0 ) return 24;
					break;

				case ImageFormat.Gif:
					if ( rank == 0 ) return 8;
					break;

				case ImageFormat.Tiff:
					if ( rank == 0 ) return 24;
					if ( rank == 1 ) return 32;
					break;

				case ImageFormat.Png:
					if ( rank == 0 ) return 24;
					if ( rank == 1 ) return 32;
					break;

				case ImageFormat.Jpeg:
					if ( rank == 0 ) return 24;
					break;
			}

			return -1;
		}

		public int RankToTypeImageCompression(int rank)
		{
			switch ( this.document.Printer.ImageFormat )
			{
				case ImageFormat.Bmp:
					if ( rank == 0 )  return (int) ImageCompression.None;
					break;

				case ImageFormat.Gif:
					break;

				case ImageFormat.Tiff:
					if ( rank == 0 )  return (int) ImageCompression.None;
					if ( rank == 1 )  return (int) ImageCompression.Lzw;
					//?if ( rank == 2 )  return (int) ImageCompression.Rle;
					//?if ( rank == 3 )  return (int) ImageCompression.FaxGroup3;
					//?if ( rank == 4 )  return (int) ImageCompression.FaxGroup4;
					break;

				case ImageFormat.Png:
					break;

				case ImageFormat.Jpeg:
					break;
			}

			return -1;
		}


		#region Serialization
		// S�rialise le r�glage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui d�s�rialise le r�glage.
		protected Integer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Value = info.GetInt32("Value");
			this.Initialise();
		}
		#endregion


		protected int				minValue;
		protected int				maxValue;
		protected int				step;
	}
}
