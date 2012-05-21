using System.Runtime.Serialization;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe ExportPDFInfo contient tous les réglages pour l'exportation PDF.
	/// </summary>
	[System.Serializable()]
	public class ExportPDFInfo : ISerializable
	{
		public ExportPDFInfo(Document document)
		{
			this.document = document;
			this.Initialize();
		}

		protected void Initialize()
		{
			this.pageRange = PrintRange.All;
			this.pageFrom = 1;
			this.pageTo = 10000;
			this.debord = 0.0;
			this.target = false;
			this.targetLength = 100.0;  // 10mm
			this.targetWidth = 1.0;  // 0.1mm
			this.targetOffset = 50.0;  // 5mm
			this.textCurve = false;
			this.execute = true;
			this.colorConversion = PDF.ColorConversion.None;
			this.imageCompression = PDF.ImageCompression.ZIP;
			this.jpegQuality = 0.7;
			this.imageMinDpi = 0.0;
			this.imageMaxDpi = 300.0;

			this.imageNameFilters = new string[2];
			this.imageNameFilters[0] = "Blackman";
			this.imageNameFilters[1] = "Bicubic";
		}

		public PrintRange PageRange
		{
			get { return this.pageRange; }
			set { this.pageRange = value; }
		}

		public int PageFrom
		{
			get { return this.pageFrom; }
			set { this.pageFrom = value; }
		}

		public int PageTo
		{
			get { return this.pageTo; }
			set { this.pageTo = value; }
		}

		public double BleedMargin
		{
			get { return this.debord; }
			set { this.debord = value; }
		}

		public Margins BleedEvenMargins
		{
			get { return this.bleedEvenMargins; }
			set { this.bleedEvenMargins = value; }
		}

		public Margins BleedOddMargins
		{
			get { return this.bleedOddMargins; }
			set { this.bleedOddMargins = value; }
		}

		public bool PrintCropMarks
		{
			get { return this.target; }
			set { this.target = value; }
		}

		public double CropMarksLength
		{
			get { return this.targetLength; }
			set { this.targetLength = value; }
		}

		public double CropMarksLengthX
		{
			get { return this.targetLengthX ?? this.targetLength; }
			set { this.targetLengthX = value; }
		}

		public double CropMarksLengthY
		{
			get { return this.targetLengthY ?? this.targetLength; }
			set { this.targetLengthY = value; }
		}

		public double CropMarksOffset
		{
			get { return this.targetOffset; }
			set { this.targetOffset = value; }
		}
		
		public double CropMarksOffsetX
		{
			get { return this.targetOffsetX ?? this.targetOffset; }
			set { this.targetOffsetX = value; }
		}

		public double CropMarksOffsetY
		{
			get { return this.targetOffsetY ?? this.targetOffset; }
			set { this.targetOffsetY = value; }
		}

		public double CropMarksWidth
		{
			get { return this.targetWidth; }
			set { this.targetWidth = value; }
		}

		public bool TextCurve
		{
			get { return this.textCurve; }
			set { this.textCurve = value; }
		}

		public bool Execute
		{
			get { return this.execute; }
			set { this.execute = value; }
		}

		public PDF.ColorConversion ColorConversion
		{
			get { return this.colorConversion; }
			set { this.colorConversion = value; }
		}

		public PDF.ImageCompression ImageCompression
		{
			get { return this.imageCompression; }
			set { this.imageCompression = value; }
		}

		public double JpegQuality
		{
			get { return this.jpegQuality; }
			set { this.jpegQuality = value; }
		}

		public double ImageMinDpi
		{
			get { return this.imageMinDpi; }
			set { this.imageMinDpi = value; }
		}

		public double ImageMaxDpi
		{
			get { return this.imageMaxDpi; }
			set { this.imageMaxDpi = value; }
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


		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise les réglages.
			info.AddValue("Rev", 8);
			info.AddValue("PageRange", this.pageRange);
			info.AddValue("PageFrom", this.pageFrom);
			info.AddValue("PageTo", this.pageTo);
			info.AddValue("Debord", this.debord);
			info.AddValue("Target", this.target);
			info.AddValue("TargetLength", this.targetLength);
			info.AddValue("TargetWidth", this.targetWidth);
			info.AddValue("TargetOffset", this.targetOffset);
			info.AddValue("TextCurve", this.textCurve);
			info.AddValue("Execute", this.execute);
			info.AddValue("ColorConversion", this.colorConversion);
			info.AddValue("ImageCompression", this.imageCompression);
			info.AddValue("JpegQuality", this.jpegQuality);
			info.AddValue("ImageMinDpi", this.imageMinDpi);
			info.AddValue("ImageMaxDpi", this.imageMaxDpi);
			info.AddValue("ImageNameFilters", this.imageNameFilters);
			info.AddValue("BleedEvenMargins", this.bleedEvenMargins);
			info.AddValue("BleedOddMargins", this.bleedOddMargins);
		}

		protected ExportPDFInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise les réglages.
			this.document = Document.ReadDocument;
			this.Initialize();

			int rev = 0;
			if ( Support.Serialization.Helper.FindElement(info, "Rev") )
			{
				rev = info.GetInt32("Rev");
			}

			this.pageRange = (PrintRange) info.GetValue("PageRange", typeof(PrintRange));
			this.pageFrom = info.GetInt32("PageFrom");
			this.pageTo = info.GetInt32("PageTo");

			if ( rev >= 2 )
			{
				this.debord = info.GetDouble("Debord");
				this.target = info.GetBoolean("Target");
				this.targetLength = info.GetDouble("TargetLength");
				this.targetWidth = info.GetDouble("TargetWidth");
				this.textCurve = info.GetBoolean("TextCurve");
				this.colorConversion = (PDF.ColorConversion) info.GetValue("ColorConversion", typeof(PDF.ColorConversion));
			}

			if ( rev >= 3 )
			{
				this.imageCompression = (PDF.ImageCompression) info.GetValue("ImageCompression", typeof(PDF.ImageCompression));
				this.jpegQuality = info.GetDouble("JpegQuality");
			}

			if ( rev >= 4 )
			{
				this.imageMinDpi = info.GetDouble("ImageMinDpi");
				this.imageMaxDpi = info.GetDouble("ImageMaxDpi");
			}

			if ( rev >= 5 )
			{
				this.imageNameFilters = (string[]) info.GetValue("ImageNameFilters", typeof(string[]));
			}

			if ( rev >= 6 )
			{
				this.bleedEvenMargins = (Margins) info.GetValue("BleedEvenMargins", typeof(Margins));
				this.bleedOddMargins  = (Margins) info.GetValue("BleedOddMargins", typeof(Margins));
			}
			else
			{
				this.bleedEvenMargins = Margins.Zero;
				this.bleedOddMargins  = Margins.Zero;
			}

			if (rev >= 7)
			{
				this.targetOffset = info.GetDouble ("TargetOffset");
			}

			if (rev >= 8)
			{
				this.execute = info.GetBoolean("Execute");
			}
		}
		#endregion

		
		protected Document					document;
		protected PrintRange				pageRange;
		protected int						pageFrom;
		protected int						pageTo;
		protected double					debord;
		protected bool						target;
		protected double					targetLength;
		protected double?					targetLengthX, targetLengthY;
		protected double					targetWidth;
		protected double					targetOffset;
		protected double?					targetOffsetX, targetOffsetY;
		protected bool						textCurve;
		protected bool						execute;
		protected PDF.ColorConversion		colorConversion;
		protected PDF.ImageCompression		imageCompression;
		protected double					jpegQuality;
		protected double					imageMinDpi;
		protected double					imageMaxDpi;
		protected string[]					imageNameFilters;
		protected Margins					bleedEvenMargins;
		protected Margins					bleedOddMargins;
	}
}
