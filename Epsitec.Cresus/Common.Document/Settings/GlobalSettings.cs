using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	public enum FirstAction
	{
		Nothing         = 0,
		OpenNewDocument = 1,
		OpenLastFile    = 2,
		OpenLastFiles   = 3,
	}

	public enum MouseWheelAction
	{
		Zoom			= 0,
		VScroll			= 1,
	}

	/// <summary>
	/// La classe GlobalSettings mémorise les paramètres de l'application.
	/// </summary>
	[System.Serializable()]
	public class GlobalSettings : ISerializable
	{
		public GlobalSettings()
		{
			this.windowLocation = new Drawing.Point(100, 100);
			this.windowSize = new Drawing.Size(830, 580);
			this.isFullScreen = false;

			this.settingsLocation = new Drawing.Point(0, 0);
			this.settingsSize = new Drawing.Size(0, 0);
			
			this.infosLocation = new Drawing.Point(0, 0);
			this.infosSize = new Drawing.Size(0, 0);
			
			this.exportLocation = new Drawing.Point(0, 0);
			this.exportSize = new Drawing.Size(0, 0);
			
			this.aboutLocation = new Drawing.Point(0, 0);
			this.aboutSize = new Drawing.Size(0, 0);
			
			this.screenDpi = 96.0;
			this.adorner = "LookMetal";
			this.defaultZoom = 1.5;
			this.mouseWheelAction = MouseWheelAction.Zoom;
			this.splashScreen = true;
			this.firstAction = FirstAction.OpenNewDocument;
			this.lastFilename = new System.Collections.ArrayList();
			this.lastFilenameMax = 10;

			// Suppose que le dossier des exemples est dans le même dossier
			// que l'application.
			string dir = Support.Globals.Directories.Executable;
			this.initialDirectory = dir + @"\Samples";
		}

		// Met tous les fichiers d'exemples dans la liste des 10 premiers
		// fichiers récents.
		public void Initialise(DocumentType type)
		{
			string ext = "";
			if ( type == DocumentType.Pictogram )
			{
				ext = "*.icon";
			}
			else
			{
				ext = "*.crdoc";
			}
			string[] list = System.IO.Directory.GetFiles(this.initialDirectory, ext);
			for ( int i=0 ; i<lastFilenameMax ; i++ )
			{
				if ( i >= list.Length )  break;
				this.LastFilenameAdd(list[i]);
			}
		}


		public Drawing.Point WindowLocation
		{
			get
			{
				return this.windowLocation;
			}

			set
			{
				this.windowLocation = value;
			}
		}

		public Drawing.Size WindowSize
		{
			get
			{
				return this.windowSize;
			}

			set
			{
				this.windowSize = value;
			}
		}

		public bool IsFullScreen
		{
			get
			{
				return this.isFullScreen;
			}

			set
			{
				this.isFullScreen = value;
			}
		}


		public Drawing.Point SettingsLocation
		{
			get
			{
				return this.settingsLocation;
			}

			set
			{
				this.settingsLocation = value;
			}
		}

		public Drawing.Size SettingsSize
		{
			get
			{
				return this.settingsSize;
			}

			set
			{
				this.settingsSize = value;
			}
		}


		public Drawing.Point InfosLocation
		{
			get
			{
				return this.infosLocation;
			}

			set
			{
				this.infosLocation = value;
			}
		}

		public Drawing.Size InfosSize
		{
			get
			{
				return this.infosSize;
			}

			set
			{
				this.infosSize = value;
			}
		}


		public Drawing.Point ExportLocation
		{
			get
			{
				return this.exportLocation;
			}

			set
			{
				this.exportLocation = value;
			}
		}

		public Drawing.Size ExportSize
		{
			get
			{
				return this.exportSize;
			}

			set
			{
				this.exportSize = value;
			}
		}


		public Drawing.Point AboutLocation
		{
			get
			{
				return this.aboutLocation;
			}

			set
			{
				this.aboutLocation = value;
			}
		}

		public Drawing.Size AboutSize
		{
			get
			{
				return this.aboutSize;
			}

			set
			{
				this.aboutSize = value;
			}
		}


		public double ScreenDpi
		{
			get
			{
				return this.screenDpi;
			}

			set
			{
				this.screenDpi = value;
			}
		}


		public string Adorner
		{
			get
			{
				return this.adorner;
			}

			set
			{
				this.adorner = value;
			}
		}

		public double DefaultZoom
		{
			get
			{
				return this.defaultZoom;
			}

			set
			{
				this.defaultZoom = value;
			}
		}

		public MouseWheelAction MouseWheelAction
		{
			get
			{
				return this.mouseWheelAction;
			}

			set
			{
				this.mouseWheelAction = value;
			}
		}

		public bool SplashScreen
		{
			get
			{
				return this.splashScreen;
			}

			set
			{
				this.splashScreen = value;
			}
		}

		public FirstAction FirstAction
		{
			get
			{
				return this.firstAction;
			}

			set
			{
				this.firstAction = value;
			}
		}


		public int LastFilenameCount
		{
			get
			{
				return this.lastFilename.Count;
			}
		}

		public string LastFilenameGet(int index)
		{
			return this.lastFilename[index] as string;
		}

		public string LastFilenameGetShort(int index)
		{
			return Misc.ExtractName(this.LastFilenameGet(index));
		}

		public void LastFilenameAdd(string filename)
		{
			int index = this.LastFilenameSearch(filename);
			if ( index < 0 )
			{
				this.LastFilenameTrunc();
			}
			else
			{
				this.lastFilename.RemoveAt(index);
			}
			this.lastFilename.Insert(0, filename);
		}

		protected int LastFilenameSearch(string filename)
		{
			for ( int i=0 ; i<this.lastFilename.Count ; i++ )
			{
				string s = this.lastFilename[i] as string;
				if ( s == filename )  return i;
			}
			return -1;
		}

		protected void LastFilenameTrunc()
		{
			if ( this.lastFilename.Count < this.lastFilenameMax )  return;
			this.lastFilename.RemoveAt(this.lastFilename.Count-1);
		}


		public string InitialDirectory
		{
			get
			{
				return this.initialDirectory;
			}

			set
			{
				this.initialDirectory = value;
			}
		}


		#region FirstAction
		public static int FirstActionCount
		{
			get { return 3; }
		}

		public static string FirstActionString(FirstAction action)
		{
			switch ( action )
			{
				case FirstAction.Nothing:          return "Rien";
				case FirstAction.OpenNewDocument:  return "Ouvrir un document vide";
				case FirstAction.OpenLastFile:     return "Ouvrir le dernier document";
				case FirstAction.OpenLastFiles:    return "Ouvrir les derniers documents";
			}
			return "?";
		}

		public static FirstAction FirstActionType(int rank)
		{
			switch ( rank )
			{
				case 0:  return FirstAction.Nothing;
				case 1:  return FirstAction.OpenNewDocument;
				case 2:  return FirstAction.OpenLastFile;
			}
			return FirstAction.Nothing;
		}

		public static int FirstActionRank(FirstAction action)
		{
			for ( int i=0 ; i<GlobalSettings.FirstActionCount ; i++ )
			{
				if ( GlobalSettings.FirstActionType(i) == action )  return i;
			}
			return -1;
		}
		#endregion


		#region MouseWheelAction
		public static int MouseWheelActionCount
		{
			get { return 2; }
		}

		public static string MouseWheelActionString(MouseWheelAction action)
		{
			switch ( action )
			{
				case MouseWheelAction.Zoom:     return "Loupe";
				case MouseWheelAction.VScroll:  return "Défilement vertical";
			}
			return "?";
		}

		public static MouseWheelAction MouseWheelActionType(int rank)
		{
			switch ( rank )
			{
				case 0:  return MouseWheelAction.Zoom;
				case 1:  return MouseWheelAction.VScroll;
			}
			return MouseWheelAction.Zoom;
		}

		public static int MouseWheelActionRank(MouseWheelAction action)
		{
			for ( int i=0 ; i<GlobalSettings.MouseWheelActionCount ; i++ )
			{
				if ( GlobalSettings.MouseWheelActionType(i) == action )  return i;
			}
			return -1;
		}
		#endregion


		#region Serialization
		// Sérialise les réglages.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("WindowLocation", this.windowLocation);
			info.AddValue("WindowSize", this.windowSize);
			info.AddValue("IsFullScreen", this.isFullScreen);

			info.AddValue("SettingsLocation", this.settingsLocation);
			info.AddValue("SettingsSize", this.settingsSize);

			info.AddValue("InfosLocation", this.infosLocation);
			info.AddValue("InfosSize", this.infosSize);

			info.AddValue("ExportLocation", this.exportLocation);
			info.AddValue("ExportSize", this.exportSize);

			info.AddValue("AboutLocation", this.aboutLocation);
			info.AddValue("AboutSize", this.aboutSize);

			info.AddValue("ScreenDpi", this.screenDpi);
			info.AddValue("Adorner", this.adorner);
			info.AddValue("DefaultZoom", this.defaultZoom);
			info.AddValue("MouseWheelAction", this.mouseWheelAction);
			info.AddValue("SplashScreen", this.splashScreen);
			info.AddValue("FirstAction", this.firstAction);
			info.AddValue("LastFilename", this.lastFilename);
			info.AddValue("InitialDirectory", this.initialDirectory);
		}

		// Constructeur qui désérialise les réglages.
		protected GlobalSettings(SerializationInfo info, StreamingContext context) : this()
		{
			this.windowLocation = (Drawing.Point) info.GetValue("WindowLocation", typeof(Drawing.Point));
			this.windowSize = (Drawing.Size) info.GetValue("WindowSize", typeof(Drawing.Size));
			this.isFullScreen = info.GetBoolean("IsFullScreen");

			this.settingsLocation = (Drawing.Point) info.GetValue("SettingsLocation", typeof(Drawing.Point));
			this.settingsSize = (Drawing.Size) info.GetValue("SettingsSize", typeof(Drawing.Size));

			this.infosLocation = (Drawing.Point) info.GetValue("InfosLocation", typeof(Drawing.Point));
			this.infosSize = (Drawing.Size) info.GetValue("InfosSize", typeof(Drawing.Size));

			this.exportLocation = (Drawing.Point) info.GetValue("ExportLocation", typeof(Drawing.Point));
			this.exportSize = (Drawing.Size) info.GetValue("ExportSize", typeof(Drawing.Size));

			this.aboutLocation = (Drawing.Point) info.GetValue("AboutLocation", typeof(Drawing.Point));
			this.aboutSize = (Drawing.Size) info.GetValue("AboutSize", typeof(Drawing.Size));

			this.screenDpi = info.GetDouble("ScreenDpi");
			this.adorner = info.GetString("Adorner");
			this.defaultZoom = info.GetDouble("DefaultZoom");
			this.mouseWheelAction = (MouseWheelAction) info.GetValue("MouseWheelAction", typeof(MouseWheelAction));
			this.splashScreen = info.GetBoolean("SplashScreen");
			this.firstAction = (FirstAction) info.GetValue("FirstAction", typeof(FirstAction));
			this.lastFilename = (System.Collections.ArrayList) info.GetValue("LastFilename", typeof(System.Collections.ArrayList));
			this.initialDirectory = info.GetString("InitialDirectory");
		}
		#endregion


		protected Drawing.Point					windowLocation;
		protected Drawing.Size					windowSize;
		protected Drawing.Point					settingsLocation;
		protected Drawing.Size					settingsSize;
		protected Drawing.Point					infosLocation;
		protected Drawing.Size					infosSize;
		protected Drawing.Point					exportLocation;
		protected Drawing.Size					exportSize;
		protected Drawing.Point					aboutLocation;
		protected Drawing.Size					aboutSize;
		protected bool							isFullScreen;
		protected double						screenDpi;
		protected string						adorner;
		protected double						defaultZoom;
		protected MouseWheelAction				mouseWheelAction;
		protected bool							splashScreen;
		protected FirstAction					firstAction;
		protected System.Collections.ArrayList	lastFilename;
		protected int							lastFilenameMax;
		protected string						initialDirectory;
	}
}
