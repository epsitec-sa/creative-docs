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
			this.Initialise();
		}

		protected void Initialise()
		{
			this.conditionName = "";
			this.conditionState = false;

			switch ( this.name )
			{
				case "DefaultUnit":
					this.text = "Unité de travail";
					this.minValue = 0;
					this.maxValue = 100;
					this.step = 1;
					break;

				case "PrintCentring":
					this.text = "Position dans la page";
					this.conditionName = "PrintAutoZoom";
					this.conditionState = true;
					this.minValue = 0;
					this.maxValue = 100;
					this.step = 1;
					break;

				case "ImageDepth":
					this.text = "Nombre de couleurs";
					this.minValue = 0;
					this.maxValue = 100;
					this.step = 1;
					break;

				case "ImageCompression":
					this.text = "Type de compression";
					this.minValue = 0;
					this.maxValue = 100;
					this.step = 1;
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

					case "PrintCentring":
						return (int) this.document.Settings.PrintInfo.Centring;

					case "ImageDepth":
						return this.document.Printer.ImageDepth;

					case "ImageCompression":
						return (int) this.document.Printer.ImageCompression;
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

					case "PrintCentring":
						this.document.Settings.PrintInfo.Centring = (PrintCentring) value;
						break;

					case "ImageDepth":
						this.document.Printer.ImageDepth = value;
						break;

					case "ImageCompression":
						this.document.Printer.ImageCompression = (ImageCompression) value;
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
					if ( unit == RealUnitType.DimensionMillimeter )  return "Millimètres";
					if ( unit == RealUnitType.DimensionCentimeter )  return "Centimètres";
					if ( unit == RealUnitType.DimensionInch       )  return "Pouces";
					break;

				case "PrintCentring":
					PrintCentring pc = (PrintCentring) type;
					if ( pc == PrintCentring.BottomLeft   )  return "Calé en bas à gauche";
					if ( pc == PrintCentring.BottomCenter )  return "Centré en bas";
					if ( pc == PrintCentring.BottomRight  )  return "Calé en bas à droite";
					if ( pc == PrintCentring.MiddleLeft   )  return "Centré à gauche";
					if ( pc == PrintCentring.MiddleCenter )  return "Centré";
					if ( pc == PrintCentring.MiddleRight  )  return "Centré à droite";
					if ( pc == PrintCentring.TopLeft      )  return "Calé en haut à gauche";
					if ( pc == PrintCentring.TopCenter    )  return "Centré en haut";
					if ( pc == PrintCentring.TopRight     )  return "Calé en haut à droite";
					break;

				case "ImageDepth":
					if ( type ==  2 )  return "Noir et blanc (1 bit)";
					if ( type ==  8 )  return "256 (8 bits)";
					if ( type == 16 )  return "65'000 (16 bits)";
					if ( type == 24 )  return "16 millions (24 bits)";
					if ( type == 32 )  return "Avec transparence (32 bits)";
					break;

				case "ImageCompression":
					ImageCompression ic = (ImageCompression) type;
					if ( ic == ImageCompression.None      )  return "Aucune";
					if ( ic == ImageCompression.Lzw       )  return "LZW";
					if ( ic == ImageCompression.Rle       )  return "RLE";
					if ( ic == ImageCompression.FaxGroup3 )  return "Fax Group 3";
					if ( ic == ImageCompression.FaxGroup4 )  return "Fax Group 4";
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
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
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
