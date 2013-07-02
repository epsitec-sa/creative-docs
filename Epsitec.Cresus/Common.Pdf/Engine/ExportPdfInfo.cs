//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

using System;

namespace Epsitec.Common.Pdf.Engine
{
	/// <summary>
	/// La classe ExportPDFInfo contient tous les réglages pour l'exportation PDF.
	/// </summary>
	public class ExportPdfInfo
	{
		public ExportPdfInfo()
		{
			this.Initialize ();
		}

		protected void Initialize()
		{
			this.pageFrom         = null;
			this.pageTo           = null;
			this.pageSize         = new Size (2100.0, 2970.0);  // A4 vertical
			this.bleedMargin      = 0.0;  // débord
			this.cropMarks        = false;
			this.cropMarksLength  = 100.0;  // 10mm
			this.cropMarksWidth   = 1.0;    // 0.1mm
			this.cropMarksOffset  = 50.0;   // 5mm
			this.textToCurve      = false;
			this.colorConversion  = ColorConversion.None;
			this.imageCompression = ImageCompression.ZipDefault;
			this.jpegQuality      = 0.7;
			this.imageMinDpi      = 0.0;
			this.imageMaxDpi      = 300.0;

			this.imageNameFilters = new string[2];
			this.imageNameFilters[0] = "Blackman";
			this.imageNameFilters[1] = "Bicubic";
		}

		public int? PageFrom
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

		public int? PageTo
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

		public bool TextToCurve
		{
			get
			{
				return this.textToCurve;
			}
			set
			{
				this.textToCurve = value;
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

		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
			}
		}

		public string Author
		{
			get
			{
				return this.author;
			}
			set
			{
				this.author = value;
			}
		}

		public string Creator
		{
			get
			{
				return this.creator;
			}
			set
			{
				this.creator = value;
			}
		}

		public string Producer
		{
			get
			{
				return this.producer;
			}
			set
			{
				this.producer = value;
			}
		}

		public DateTime CreationDate
		{
			get
			{
				return this.creationDate;
			}
			set
			{
				this.creationDate = value;
			}
		}


		private int?					pageFrom;
		private int?					pageTo;
		private Size					pageSize;
		private double					bleedMargin;
		private bool					cropMarks;
		private double					cropMarksLength;
		private double?					cropMarksLengthX, cropMarksLengthY;
		private double					cropMarksWidth;
		private double					cropMarksOffset;
		private double?					cropMarksOffsetX, cropMarksOffsetY;
		private bool					textToCurve;
		private ColorConversion			colorConversion;
		private ImageCompression		imageCompression;
		private double					jpegQuality;
		private double					imageMinDpi;
		private double					imageMaxDpi;
		private string[]				imageNameFilters;
		private Margins					bleedEvenMargins;
		private Margins					bleedOddMargins;
		private string					title;
		private string					author;
		private string					creator;
		private string					producer;
		private DateTime				creationDate;
	}
}
