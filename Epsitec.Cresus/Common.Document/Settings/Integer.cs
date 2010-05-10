using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Integer contient un réglage numérique.
	/// </summary>
	[System.Serializable()]
	public class Integer : Abstract
	{
		public Integer(Document document, string name) : base(document, name)
		{
			this.Initialize();
		}

		protected void Initialize()
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

				case "PrintImageFilterA":
					this.text = Res.Strings.Dialog.Integer.ImageFilterA.Text;
					break;

				case "PrintImageFilterB":
					this.text = Res.Strings.Dialog.Integer.ImageFilterB.Text;
					break;

				case "ImageCrop":
				case "ExportICOCrop":
					this.text = Res.Strings.Dialog.Integer.ImageCrop.Text;
					break;

				case "ImageDepth":
					this.text = Res.Strings.Dialog.Integer.ImageDepth.Text;
					break;

				case "ImageCompression":
					this.text = Res.Strings.Dialog.Integer.ImageCompression.Text;
					break;

				case "ImageFilterA":
					this.text = Res.Strings.Dialog.Integer.ImageFilterA.Text;
					break;

				case "ImageFilterB":
					this.text = Res.Strings.Dialog.Integer.ImageFilterB.Text;
					break;

				case "ExportPDFColorConversion":
					this.text = Res.Strings.Dialog.Integer.ExportPDFColorConversion.Text;
					break;

				case "ExportPDFImageCompression":
					this.text = Res.Strings.Dialog.Integer.ExportPDFImageCompression.Text;
					break;

				case "ExportPDFImageFilterA":
					this.text = Res.Strings.Dialog.Integer.ImageFilterA.Text;
					break;

				case "ExportPDFImageFilterB":
					this.text = Res.Strings.Dialog.Integer.ImageFilterB.Text;
					break;

                case "ExportICOFormat":
                    this.text = Res.Strings.Dialog.Integer.ExportICOFormat.Text;
                    break;

                case "ConstrainAngle":
                    this.text = Res.Strings.Dialog.Integer.ConstrainAngle.Text;
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

					case "PrintImageFilterA":
						return Properties.Image.FilterNameToIndex(this.document.Settings.PrintInfo.GetImageNameFilter(0));

					case "PrintImageFilterB":
						return Properties.Image.FilterNameToIndex(this.document.Settings.PrintInfo.GetImageNameFilter(1));

					case "ImageCrop":
					case "ExportICOCrop":
						return (int) this.document.Printer.ImageCrop;

					case "ImageDepth":
						return this.document.Printer.ImageDepth;

					case "ImageCompression":
						return (int) this.document.Printer.ImageCompression;

					case "ImageFilterA":
						return Properties.Image.FilterNameToIndex(this.document.Printer.GetImageNameFilter(0));

					case "ImageFilterB":
						return Properties.Image.FilterNameToIndex(this.document.Printer.GetImageNameFilter(1));

					case "ExportPDFColorConversion":
						return (int) this.document.Settings.ExportPDFInfo.ColorConversion;

					case "ExportPDFImageCompression":
						return (int) this.document.Settings.ExportPDFInfo.ImageCompression;

					case "ExportPDFImageFilterA":
						return Properties.Image.FilterNameToIndex(this.document.Settings.ExportPDFInfo.GetImageNameFilter(0));

					case "ExportPDFImageFilterB":
						return Properties.Image.FilterNameToIndex(this.document.Settings.ExportPDFInfo.GetImageNameFilter(1));

                    case "ExportICOFormat":
                        return (int) this.document.Settings.ExportICOInfo.Format;

                    case "ConstrainAngle":
						return (int) this.document.Modifier.ActiveViewer.DrawingContext.ConstrainAngle;
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

					case "PrintImageFilterA":
						this.document.Settings.PrintInfo.SetImageNameFilter(0, Properties.Image.FilterIndexToName(value));
						break;

					case "PrintImageFilterB":
						this.document.Settings.PrintInfo.SetImageNameFilter(1, Properties.Image.FilterIndexToName(value));
						break;

					case "ImageCrop":
					case "ExportICOCrop":
						this.document.Printer.ImageCrop = (ExportImageCrop) value;
						break;

					case "ImageDepth":
						this.document.Printer.ImageDepth = value;
						break;

					case "ImageCompression":
						this.document.Printer.ImageCompression = (ImageCompression) value;
						break;

					case "ImageFilterA":
						this.document.Printer.SetImageNameFilter(0, Properties.Image.FilterIndexToName(value));
						break;

					case "ImageFilterB":
						this.document.Printer.SetImageNameFilter(1, Properties.Image.FilterIndexToName(value));
						break;

					case "ExportPDFColorConversion":
						this.document.Settings.ExportPDFInfo.ColorConversion = (PDF.ColorConversion) value;
						break;

					case "ExportPDFImageCompression":
						this.document.Settings.ExportPDFInfo.ImageCompression = (PDF.ImageCompression) value;
						break;

					case "ExportPDFImageFilterA":
						this.document.Settings.ExportPDFInfo.SetImageNameFilter(0, Properties.Image.FilterIndexToName(value));
						break;

					case "ExportPDFImageFilterB":
						this.document.Settings.ExportPDFInfo.SetImageNameFilter(1, Properties.Image.FilterIndexToName(value));
						break;

                    case "ExportICOFormat":
                        this.document.Settings.ExportICOInfo.Format = (ICOFormat) value;
                        break;

                    case "ConstrainAngle":
						this.document.Modifier.ActiveViewer.DrawingContext.ConstrainAngle = (ConstrainAngle) value;
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
				combo.SelectedItemIndex = 0;
			}
			else
			{
				int sel = this.TypeToRank(this.Value);
				if ( sel == -1 )
				{
					this.Value = this.RankToType(0);
					sel = this.TypeToRank(this.Value);
				}
				combo.SelectedItemIndex = sel;
			}

			combo.Enable = (combo.Items.Count > 1);
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

				case "ImageCrop":
				case "ExportICOCrop":
					ExportImageCrop xic = (ExportImageCrop) type;
					if ( xic == ExportImageCrop.Page      )  return Res.Strings.Dialog.Integer.ImageCrop.Page;
					if ( xic == ExportImageCrop.Objects   )  return Res.Strings.Dialog.Integer.ImageCrop.Objects;
					if ( xic == ExportImageCrop.Selection )  return Res.Strings.Dialog.Integer.ImageCrop.Selection;
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
					if ( cc == PDF.ColorConversion.ToRgb  )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.ToRgb;
					if ( cc == PDF.ColorConversion.ToCmyk )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.ToCmyk;
					if ( cc == PDF.ColorConversion.ToGray )  return Res.Strings.Dialog.Integer.ExportPDFColorConversion.ToGray;
					break;

				case "ExportPDFImageCompression":
					PDF.ImageCompression imc = (PDF.ImageCompression) type;
					if ( imc == PDF.ImageCompression.None )  return Res.Strings.Dialog.Integer.ExportPDFImageCompression.None;
					if ( imc == PDF.ImageCompression.ZIP  )  return Res.Strings.Dialog.Integer.ExportPDFImageCompression.ZIP;
					if ( imc == PDF.ImageCompression.JPEG )  return Res.Strings.Dialog.Integer.ExportPDFImageCompression.JPEG;
					break;

				case "PrintImageFilterA":
				case "PrintImageFilterB":
				case "ImageFilterA":
				case "ImageFilterB":
				case "ExportPDFImageFilterA":
				case "ExportPDFImageFilterB":
					return Properties.Image.FilterNameToText(Properties.Image.FilterIndexToName(type));

				case "ExportICOFormat":
					ICOFormat icf = (ICOFormat) type;
                    if ( icf == ICOFormat.XP    )  return Res.Strings.Dialog.Integer.ExportICOFormat.XP;
                    if ( icf == ICOFormat.Vista )  return Res.Strings.Dialog.Integer.ExportICOFormat.Vista;
				    break;

				case "ConstrainAngle":
					ConstrainAngle ca = (ConstrainAngle) type;
                    if ( ca == ConstrainAngle.None    )  return Res.Strings.Dialog.Integer.ConstrainAngle.None;
                    if ( ca == ConstrainAngle.Quarter )  return Res.Strings.Dialog.Integer.ConstrainAngle.Quarter;
                    if ( ca == ConstrainAngle.Sixth   )  return Res.Strings.Dialog.Integer.ConstrainAngle.Sixth;
                    if ( ca == ConstrainAngle.Eight   )  return Res.Strings.Dialog.Integer.ConstrainAngle.Eight;
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

				case "ImageCrop":
				case "ExportICOCrop":
					if ( rank == 0 )  return (int) ExportImageCrop.Page;
					if ( rank == 1 )  return (int) ExportImageCrop.Objects;
					if ( rank == 2 )  return (int) ExportImageCrop.Selection;
					return -1;

				case "ImageDepth":
					return this.RankToTypeImageDepth(rank);

				case "ImageCompression":
					return this.RankToTypeImageCompression(rank);

				case "ExportPDFColorConversion":
					if ( rank == 0 )  return (int) PDF.ColorConversion.None;
					if ( rank == 1 )  return (int) PDF.ColorConversion.ToRgb;
					if ( rank == 2 )  return (int) PDF.ColorConversion.ToCmyk;
					if ( rank == 3 )  return (int) PDF.ColorConversion.ToGray;
					return -1;

				case "ExportPDFImageCompression":
					if ( rank == 0 )  return (int) PDF.ImageCompression.None;
					if ( rank == 1 )  return (int) PDF.ImageCompression.ZIP;
					if ( rank == 2 )  return (int) PDF.ImageCompression.JPEG;
					return -1;

				case "PrintImageFilterA":
				case "PrintImageFilterB":
				case "ImageFilterA":
				case "ImageFilterB":
				case "ExportPDFImageFilterA":
				case "ExportPDFImageFilterB":
					return Properties.Image.FilterIndexToName(rank) == null ? -1 : rank;

				case "ExportICOFormat":
					if ( rank == 0 )  return (int) ICOFormat.XP;
					if ( rank == 1 )  return (int) ICOFormat.Vista;
					return -1;

				case "ConstrainAngle":
					if ( rank == 0 )  return (int) ConstrainAngle.None;
					if ( rank == 1 )  return (int) ConstrainAngle.Quarter;
					if ( rank == 2 )  return (int) ConstrainAngle.Sixth;
					if ( rank == 3 )  return (int) ConstrainAngle.Eight;
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
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le réglage.
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		protected Integer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise le réglage.
			if ( this.name == "DefaultUnit" )
			{
				RealUnitType unit = (RealUnitType) info.GetInt32("Value");
				this.document.Modifier.SetRealUnitDimension(unit, false);
			}
			else
			{
				this.Value = info.GetInt32("Value");
			}

			this.Initialize();
		}
		#endregion


		protected int				minValue;
		protected int				maxValue;
		protected int				step;
	}
}
