using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe ExportPDFInfo comtient tous les réglages secondaires pour l'impression.
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
			this.textCurve = false;
			this.colorConversion = PDF.ColorConversion.None;
			this.imageCompression = PDF.ImageCompression.ZIP;
			this.jpegQuality = 0.7;
			this.imageMinDpi = 0.0;
			this.imageMaxDpi = 300.0;
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

		public double Debord
		{
			get { return this.debord; }
			set { this.debord = value; }
		}

		public bool Target
		{
			get { return this.target; }
			set { this.target = value; }
		}

		public double TargetLength
		{
			get { return this.targetLength; }
			set { this.targetLength = value; }
		}

		public double TargetWidth
		{
			get { return this.targetWidth; }
			set { this.targetWidth = value; }
		}

		public bool TextCurve
		{
			get { return this.textCurve; }
			set { this.textCurve = value; }
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


		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise les réglages.
			info.AddValue("Rev", 4);
			info.AddValue("PageRange", this.pageRange);
			info.AddValue("PageFrom", this.pageFrom);
			info.AddValue("PageTo", this.pageTo);
			info.AddValue("Debord", this.debord);
			info.AddValue("Target", this.target);
			info.AddValue("TargetLength", this.targetLength);
			info.AddValue("TargetWidth", this.targetWidth);
			info.AddValue("TextCurve", this.textCurve);
			info.AddValue("ColorConversion", this.colorConversion);
			info.AddValue("ImageCompression", this.imageCompression);
			info.AddValue("JpegQuality", this.jpegQuality);
			info.AddValue("ImageMinDpi", this.imageMinDpi);
			info.AddValue("ImageMaxDpi", this.imageMaxDpi);
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
		}
		#endregion

		
		protected Document					document;
		protected PrintRange				pageRange;
		protected int						pageFrom;
		protected int						pageTo;
		protected double					debord;
		protected bool						target;
		protected double					targetLength;
		protected double					targetWidth;
		protected bool						textCurve;
		protected PDF.ColorConversion		colorConversion;
		protected PDF.ImageCompression		imageCompression;
		protected double					jpegQuality;
		protected double					imageMinDpi;
		protected double					imageMaxDpi;
	}
}
