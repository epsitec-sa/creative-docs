using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
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

	/// <summary>
	/// La classe PrintInfo comtient tous les réglages secondaires pour l'impression.
	/// </summary>
	[System.Serializable()]
	public class PrintInfo : ISerializable
	{
		public PrintInfo(Document document)
		{
			this.document = document;
			this.Initialise();
		}

		protected void Initialise()
		{
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


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Rev", 1);
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
		}

		// Constructeur qui désérialise les réglages.
		protected PrintInfo(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.Initialise();

			int rev = 0;
			if ( Support.Serialization.Helper.FindElement(info, "Rev") )
			{
				rev = info.GetInt32("Rev");
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
		}
		#endregion

		
		protected Document				document;
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
	}
}
