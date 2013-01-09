using System.Runtime.Serialization;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf
{
	/// <summary>
	/// La classe ExportPDFInfo contient tous les réglages pour l'exportation PDF.
	/// </summary>
	public class ExportPdfInfo
	{
		public ExportPdfInfo()
		{
			this.Initialize();
		}

		protected void Initialize()
		{
			this.pageFrom         = 1;
			this.pageTo           = 1000;
			this.pageSize         = new Size (2100.0, 2970.0);  // A4 vertical
			this.bleedMargin      = 0.0;  // débord
			this.cropMarks        = false;
			this.cropMarksLength  = 100.0;  // 10mm
			this.cropMarksWidth   = 1.0;    // 0.1mm
			this.cropMarksOffset  = 50.0;   // 5mm
			this.textCurve        = false;
			this.execute          = true;
			this.colorConversion  = ColorConversion.None;
			this.imageCompression = ImageCompression.ZIP;
			this.jpegQuality      = 0.7;
			this.imageMinDpi      = 0.0;
			this.imageMaxDpi      = 300.0;

			this.imageNameFilters = new string[2];
			this.imageNameFilters[0] = "Blackman";
			this.imageNameFilters[1] = "Bicubic";
		}

		public int PageFrom
		{
			get
			{
				return this.pageFrom;
			}
			set
			{
				this.pageFrom = value;
			}
		}

		public int PageTo
		{
			get
			{
				return this.pageTo;
			}
			set
			{
				this.pageTo = value;
			}
		}

		public Size PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				this.pageSize = value;
			}
		}

		public double BleedMargin
		{
			get
			{
				return this.bleedMargin;
			}
			set
			{
				this.bleedMargin = value;
			}
		}

		public Margins BleedEvenMargins
		{
			get
			{
				return this.bleedEvenMargins;
			}
			set
			{
				this.bleedEvenMargins = value;
			}
		}

		public Margins BleedOddMargins
		{
			get
			{
				return this.bleedOddMargins;
			}
			set
			{
				this.bleedOddMargins = value;
			}
		}

		public bool PrintCropMarks
		{
			get
			{
				return this.cropMarks;
			}
			set
			{
				this.cropMarks = value;
			}
		}

		public double CropMarksLength
		{
			get
			{
				return this.cropMarksLength;
			}
			set
			{
				this.cropMarksLength = value;
			}
		}

		public double CropMarksLengthX
		{
			get
			{
				return this.cropMarksLengthX ?? this.cropMarksLength;
			}
			set
			{
				this.cropMarksLengthX = value;
			}
		}

		public double CropMarksLengthY
		{
			get
			{
				return this.cropMarksLengthY ?? this.cropMarksLength;
			}
			set
			{
				this.cropMarksLengthY = value;
			}
		}

		public double CropMarksOffset
		{
			get
			{
				return this.cropMarksOffset;
			}
			set
			{
				this.cropMarksOffset = value;
			}
		}

		public double CropMarksOffsetX
		{
			get
			{
				return this.cropMarksOffsetX ?? this.cropMarksOffset;
			}
			set
			{
				this.cropMarksOffsetX = value;
			}
		}

		public double CropMarksOffsetY
		{
			get
			{
				return this.cropMarksOffsetY ?? this.cropMarksOffset;
			}
			set
			{
				this.cropMarksOffsetY = value;
			}
		}

		public double CropMarksWidth
		{
			get
			{
				return this.cropMarksWidth;
			}
			set
			{
				this.cropMarksWidth = value;
			}
		}

		public bool TextCurve
		{
			get
			{
				return this.textCurve;
			}
			set
			{
				this.textCurve = value;
			}
		}

		public bool Execute
		{
			get
			{
				return this.execute;
			}
			set
			{
				this.execute = value;
			}
		}

		public ColorConversion ColorConversion
		{
			get
			{
				return this.colorConversion;
			}
			set
			{
				this.colorConversion = value;
			}
		}

		public ImageCompression ImageCompression
		{
			get
			{
				return this.imageCompression;
			}
			set
			{
				this.imageCompression = value;
			}
		}

		public double JpegQuality
		{
			get
			{
				return this.jpegQuality;
			}
			set
			{
				this.jpegQuality = value;
			}
		}

		public double ImageMinDpi
		{
			get
			{
				return this.imageMinDpi;
			}
			set
			{
				this.imageMinDpi = value;
			}
		}

		public double ImageMaxDpi
		{
			get
			{
				return this.imageMaxDpi;
			}
			set
			{
				this.imageMaxDpi = value;
			}
		}


		#region ImageNameFilter
		public string GetImageNameFilter(int rank)
		{
			//	Donne le nom d'un filtre pour l'image.
			System.Diagnostics.Debug.Assert(rank >= 0 && rank < this.imageNameFilters.Length);
			return this.imageNameFilters[rank];
		}

		public void SetImageNameFilter(int rank, string name)
		{
			//	Modifie le nom d'un filtre pour l'image.
			System.Diagnostics.Debug.Assert(rank >= 0 && rank < this.imageNameFilters.Length);
			this.imageNameFilters[rank] = name;
		}
		#endregion


		private int						pageFrom;
		private int						pageTo;
		private Size					pageSize;
		private double					bleedMargin;
		private bool					cropMarks;
		private double					cropMarksLength;
		private double?					cropMarksLengthX, cropMarksLengthY;
		private double					cropMarksWidth;
		private double					cropMarksOffset;
		private double?					cropMarksOffsetX, cropMarksOffsetY;
		private bool					textCurve;
		private bool					execute;
		private ColorConversion			colorConversion;
		private ImageCompression		imageCompression;
		private double					jpegQuality;
		private double					imageMinDpi;
		private double					imageMaxDpi;
		private string[]				imageNameFilters;
		private Margins					bleedEvenMargins;
		private Margins					bleedOddMargins;
	}
}
