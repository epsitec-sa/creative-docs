using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum PrintCentring
	{
		BottomLeft   = 10,
		BottomCenter = 11,
		BottomRight  = 12,
		
		MiddleLeft   = 20,
		MiddleCenter = 21,
		MiddleRight  = 22,
		
		TopLeft      = 30,
		TopCenter    = 31,
		TopRight     = 32,
	}

	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum PrintRange
	{
		All     = 0,
		FromTo  = 1,
		Current = 2,
	}

	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum PrintArea
	{
		All  = 0,
		Even = 1,
		Odd  = 2,
	}

	/// <summary>
	/// La classe PrintInfo contient tous les réglages secondaires pour l'impression.
	/// </summary>
	[System.Serializable()]
	public class PrintInfo : ISerializable
	{
		public PrintInfo(Document document)
		{
			this.document = document;
			this.Initialize();
		}

		protected void Initialize()
		{
			this.printName = "";
			this.printRange = PrintRange.All;
			this.printArea = PrintArea.All;
			this.printFrom = 1;
			this.printTo = 10000;
			this.copies = 1;
			this.collate = false;
			this.reverse = false;
			this.printToFile = false;
			this.printFilename = "";
			this.dpi = 300.0;
			this.zoom = 1.0;  // 100%
			this.gamma = 0.0;  // pas d'AA
			this.autoLandscape = true;
			this.autoZoom = false;
			this.forceSimply = false;
			this.forceComplex = false;
			this.perfectJoin = false;
			this.centring = PrintCentring.MiddleCenter;
			this.margins = 100.0;
			this.debord = 50.0;
			this.target = false;
			this.debugArea = false;

			this.imageNameFilters = new string[2];
			this.imageNameFilters[0] = "Blackman";
			this.imageNameFilters[1] = "Bicubic";
		}

		public string PrintName
		{
			get
			{
				if ( this.printName == "" )
				{
					return this.document.PrintDialog.Document.PrinterSettings.PrinterName;
				}
				return this.printName;
			}
			
			set
			{
				this.printName = value;
			}
		}

		public PrintRange PrintRange
		{
			get { return this.printRange; }
			set { this.printRange = value; }
		}

		public PrintArea PrintArea
		{
			get { return this.printArea; }
			set { this.printArea = value; }
		}

		public int PrintFrom
		{
			get { return this.printFrom; }
			set { this.printFrom = value; }
		}

		public int PrintTo
		{
			get { return this.printTo; }
			set { this.printTo = value; }
		}

		public int Copies
		{
			get { return this.copies; }
			set { this.copies = value; }
		}

		public bool Collate
		{
			get { return this.collate; }
			set { this.collate = value; }
		}

		public bool Reverse
		{
			get { return this.reverse; }
			set { this.reverse = value; }
		}

		public bool PrintToFile
		{
			get { return this.printToFile; }
			set { this.printToFile = value; }
		}

		public string PrintFilename
		{
			get { return this.printFilename; }
			set { this.printFilename = value; }
		}

		public double Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		public double Gamma
		{
			get { return this.gamma; }
			set { this.gamma = value; }
		}

		public double Dpi
		{
			get { return this.dpi; }
			set { this.dpi = value; }
		}

		public bool AutoZoom
		{
			get
			{
				return this.autoZoom;
			}
			
			set
			{
				if ( this.autoZoom != value )
				{
					this.autoZoom = value;
					this.document.Notifier.NotifyArea();
				}
			}
		}

		public bool AutoLandscape
		{
			get { return this.autoLandscape; }
			set { this.autoLandscape = value; }
		}

		public bool ForceSimply
		{
			get { return this.forceSimply; }
			set { this.forceSimply = value; }
		}

		public bool ForceComplex
		{
			get { return this.forceComplex; }
			set { this.forceComplex = value; }
		}

		public bool PerfectJoin
		{
			get { return this.perfectJoin; }
			set { this.perfectJoin = value; }
		}

		public PrintCentring Centring
		{
			get { return this.centring; }
			set { this.centring = value; }
		}

		public double Margins
		{
			get { return this.margins; }
			set { this.margins = value; }
		}

		public double Debord
		{
			get
			{
				return this.debord;
			}
			
			set
			{
				if ( this.debord != value )
				{
					this.debord = value;
					this.document.Notifier.NotifyArea();
				}
			}
		}

		public bool Target
		{
			get
			{
				return this.target;
			}
			
			set
			{
				if ( this.target != value )
				{
					this.target = value;
					this.document.Notifier.NotifyArea();
				}
			}
		}

		public bool DebugArea
		{
			get { return this.debugArea; }
			set { this.debugArea = value; }
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
			info.AddValue("Rev", 4);
			info.AddValue("PrintName", this.printName);
			info.AddValue("PrintRange", this.printRange);
			info.AddValue("PrintArea", this.printArea);
			info.AddValue("PrintFrom", this.printFrom);
			info.AddValue("PrintTo", this.printTo);
			info.AddValue("Copies", this.copies);
			info.AddValue("Collate", this.collate);
			info.AddValue("Reverse", this.reverse);
			info.AddValue("PrintToFile", this.printToFile);
			info.AddValue("PrintFilename", this.printFilename);
			info.AddValue("Dpi", this.dpi);
			info.AddValue("Gamma", this.gamma);
			info.AddValue("Zoom", this.zoom);
			info.AddValue("AutoZoom", this.autoZoom);
			info.AddValue("AutoLandscape", this.autoLandscape);
			info.AddValue("ForceSimply", this.forceSimply);
			info.AddValue("ForceComplex", this.forceComplex);
			info.AddValue("Centring", this.centring);
			info.AddValue("Margins", this.margins);
			info.AddValue("Debord", this.debord);
			info.AddValue("Target", this.target);
			info.AddValue("ImageNameFilters", this.imageNameFilters);
		}

		protected PrintInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise les réglages.
			this.document = Document.ReadDocument;
			this.Initialize();

			int rev = 0;
			if ( Support.Serialization.Helper.FindElement(info, "Rev") )
			{
				rev = info.GetInt32("Rev");
			}

			if ( rev >= 2 )
			{
				this.printName = info.GetString("PrintName");
				this.printRange = (PrintRange) info.GetValue("PrintRange", typeof(PrintRange));
				this.printArea = (PrintArea) info.GetValue("PrintArea", typeof(PrintArea));
				this.printFrom = info.GetInt32("PrintFrom");
				this.printTo = info.GetInt32("PrintTo");
				//	On ne veut pas mémoriser le nombre de copies, mais toujours remettre 1 par défaut !
				//this.copies = info.GetInt32("Copies");
				this.copies = 1;
				this.collate = info.GetBoolean("Collate");
				this.reverse = info.GetBoolean("Reverse");
			}

			if ( rev >= 3 )
			{
				this.printToFile = info.GetBoolean("PrintToFile");
				this.printFilename = info.GetString("PrintFilename");
			}

			this.dpi = info.GetDouble("Dpi");
			this.gamma = info.GetDouble("Gamma");
			this.zoom = info.GetDouble("Zoom");
			this.autoZoom = info.GetBoolean("AutoZoom");
			this.autoLandscape = info.GetBoolean("AutoLandscape");
			this.forceSimply = info.GetBoolean("ForceSimply");
			this.forceComplex = info.GetBoolean("ForceComplex");

			if ( rev >= 1 )
			{
				this.centring = (PrintCentring) info.GetValue("Centring", typeof(PrintCentring));
				this.margins = info.GetDouble("Margins");
				this.debord = info.GetDouble("Debord");
				this.target = info.GetBoolean("Target");
			}

			if ( rev >= 4 )
			{
				this.imageNameFilters = (string[]) info.GetValue("ImageNameFilters", typeof(string[]));
			}
		}
		#endregion

		
		protected Document				document;
		protected string				printName;
		protected PrintRange			printRange;
		protected PrintArea				printArea;
		protected int					printFrom;
		protected int					printTo;
		protected int					copies;
		protected bool					collate;
		protected bool					reverse;
		protected bool					printToFile;
		protected string				printFilename;
		protected double				dpi;
		protected double				gamma;
		protected double				zoom;
		protected bool					autoZoom;
		protected bool					autoLandscape;
		protected bool					forceSimply;
		protected bool					forceComplex;
		protected bool					perfectJoin;
		protected PrintCentring			centring;
		protected double				margins;
		protected double				debord;
		protected bool					target;
		protected bool					debugArea;
		protected string[]				imageNameFilters;
	}
}
