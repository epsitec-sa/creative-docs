using Epsitec.Common.Document;
using Epsitec.Common.Widgets;
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
			this.windowLocation = new Drawing.Point(0, 0);
			this.windowSize = new Drawing.Size(830, 580);
			this.isFullScreen = false;
			this.windowBounds = new System.Collections.Hashtable();

			this.screenDpi = 96.0;
			this.adorner = "LookMetal";
			this.defaultZoom = 1.5;
			this.mouseWheelAction = MouseWheelAction.Zoom;
			this.fineCursor = false;
			this.splashScreen = true;
			this.firstAction = FirstAction.OpenNewDocument;
			this.lastFilename = new System.Collections.ArrayList();
			this.lastFilenameMax = 10;
			this.labelProperties = true;

			// Suppose que le dossier des exemples est dans le même dossier
			// que l'application.
			string dir = Support.Globals.Directories.Executable;
			this.initialDirectory = dir + @"\Samples";

			this.colorCollection = new ColorCollection();
			this.colorCollectionDirectory = "";
			this.colorCollectionFilename = "";
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
			try
			{
				string[] list = System.IO.Directory.GetFiles(this.initialDirectory, ext);
				for ( int i=0 ; i<lastFilenameMax ; i++ )
				{
					if ( i >= list.Length )  break;
					this.LastFilenameAdd(list[i]);
				}
			}
			catch
			{
			}
		}


		// Fenêtre principale de l'application.
		public Drawing.Rectangle MainWindow
		{
			get
			{
				Drawing.Rectangle rect = new Rectangle(this.windowLocation, this.windowSize);
				if ( this.windowLocation.IsEmpty )
				{
					// Lors de la première exécution, met l'application au centre
					// de la fenêtre.
					ScreenInfo si = ScreenInfo.Find(new Drawing.Point(0,0));
					Rectangle area = si.WorkingArea;
					rect = new Rectangle(area.Center-rect.Size/2, area.Center+rect.Size/2);
				}
				return GlobalSettings.WindowClip(rect);
			}

			set
			{
				this.windowLocation = value.Location;
				this.windowSize = value.Size;
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


		// Ajoute une définition de fenêtre.
		public void SetWindowBounds(string name, Drawing.Point location, Drawing.Size size)
		{
			WindowBounds wb = this.windowBounds[name] as WindowBounds;
			if ( wb == null )
			{
				wb = new WindowBounds(location, size);
				this.windowBounds.Add(name, wb);
			}
			else
			{
				wb.Location = location;
				wb.Size = size;
			}
		}

		// Cherche une définition de fenêtre.
		public bool GetWindowBounds(string name, ref Drawing.Point location, ref Drawing.Size size)
		{
			WindowBounds wb = this.windowBounds[name] as WindowBounds;
			if ( wb == null )  return false;
			location = wb.Location;
			size = wb.Size;
			return true;
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

		public bool FineCursor
		{
			get
			{
				return this.fineCursor;
			}

			set
			{
				this.fineCursor = value;
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


		public bool LabelProperties
		{
			get
			{
				return this.labelProperties;
			}

			set
			{
				this.labelProperties = value;
			}
		}


		public Drawing.ColorCollection ColorCollection
		{
			get
			{
				return this.colorCollection;
			}

			set
			{
				this.colorCollection = value;
			}
		}

		public string ColorCollectionDirectory
		{
			get
			{
				return this.colorCollectionDirectory;
			}

			set
			{
				this.colorCollectionDirectory = value;
			}
		}

		public string ColorCollectionFilename
		{
			get
			{
				return this.colorCollectionFilename;
			}

			set
			{
				this.colorCollectionFilename = value;
			}
		}


		// Adapte un rectangle pour qu'il entre dans l'écran, si possible
		// sans modifier ses dimensions. Utilisé pour les dialogues.
		public static Drawing.Rectangle WindowClip(Drawing.Rectangle rect)
		{
			ScreenInfo si = ScreenInfo.Find(rect.Center);
			Rectangle area = si.WorkingArea;

			rect.Width  = System.Math.Min(rect.Width,  area.Width );
			rect.Height = System.Math.Min(rect.Height, area.Height);

			if ( rect.Left < area.Left )  // dépasse à gauche ?
			{
				rect.Offset(area.Left-rect.Left, 0);
			}

			if ( rect.Right > area.Right )  // dépasse à droite ?
			{
				rect.Offset(area.Right-rect.Right, 0);
			}

			if ( rect.Bottom < area.Bottom )  // dépasse en bas ?
			{
				rect.Offset(0, area.Bottom-rect.Bottom);
			}

			if ( rect.Top > area.Top )  // dépasse en haut ?
			{
				rect.Offset(0, area.Top-rect.Top);
			}

			return rect;
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
				case FirstAction.Nothing:          return Res.Strings.Dialog.Settings.FirstAction.Nothing;
				case FirstAction.OpenNewDocument:  return Res.Strings.Dialog.Settings.FirstAction.OpenNewDocument;
				case FirstAction.OpenLastFile:     return Res.Strings.Dialog.Settings.FirstAction.OpenLastFile;
				case FirstAction.OpenLastFiles:    return Res.Strings.Dialog.Settings.FirstAction.OpenLastFiles;
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
				case MouseWheelAction.Zoom:     return Res.Strings.Dialog.Settings.MouseWheelAction.Zoom;
				case MouseWheelAction.VScroll:  return Res.Strings.Dialog.Settings.MouseWheelAction.VScroll;
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
			info.AddValue("Version", 2);

			info.AddValue("WindowLocation", this.windowLocation);
			info.AddValue("WindowSize", this.windowSize);
			info.AddValue("IsFullScreen", this.isFullScreen);
			info.AddValue("WindowBounds", this.windowBounds);

			info.AddValue("ScreenDpi", this.screenDpi);
			info.AddValue("Adorner", this.adorner);
			info.AddValue("DefaultZoom", this.defaultZoom);
			info.AddValue("MouseWheelAction", this.mouseWheelAction);
			info.AddValue("FineCursor", this.fineCursor);
			info.AddValue("SplashScreen", this.splashScreen);
			info.AddValue("FirstAction", this.firstAction);
			info.AddValue("LastFilename", this.lastFilename);
			info.AddValue("InitialDirectory", this.initialDirectory);
			info.AddValue("LabelProperties", this.labelProperties);

			info.AddValue("ColorCollection", this.colorCollection);
			info.AddValue("ColorCollectionDirectory", this.colorCollectionDirectory);
			info.AddValue("ColorCollectionFilename", this.colorCollectionFilename);
		}

		// Constructeur qui désérialise les réglages.
		protected GlobalSettings(SerializationInfo info, StreamingContext context) : this()
		{
			int version = info.GetInt32("Version");

			this.windowLocation = (Drawing.Point) info.GetValue("WindowLocation", typeof(Drawing.Point));
			this.windowSize = (Drawing.Size) info.GetValue("WindowSize", typeof(Drawing.Size));
			this.isFullScreen = info.GetBoolean("IsFullScreen");
			this.windowBounds = (System.Collections.Hashtable) info.GetValue("WindowBounds", typeof(System.Collections.Hashtable));

			this.screenDpi = info.GetDouble("ScreenDpi");
			this.adorner = info.GetString("Adorner");
			this.defaultZoom = info.GetDouble("DefaultZoom");
			this.mouseWheelAction = (MouseWheelAction) info.GetValue("MouseWheelAction", typeof(MouseWheelAction));
			this.fineCursor = info.GetBoolean("FineCursor");
			this.splashScreen = info.GetBoolean("SplashScreen");
			this.firstAction = (FirstAction) info.GetValue("FirstAction", typeof(FirstAction));
			this.lastFilename = (System.Collections.ArrayList) info.GetValue("LastFilename", typeof(System.Collections.ArrayList));
			this.initialDirectory = info.GetString("InitialDirectory");

			if ( version >= 2 )
			{
				this.labelProperties = info.GetBoolean("LabelProperties");
			}

			this.colorCollection = (ColorCollection) info.GetValue("ColorCollection", typeof(ColorCollection));
			this.colorCollectionDirectory = info.GetString("ColorCollectionDirectory");
			this.colorCollectionFilename = info.GetString("ColorCollectionFilename");
		}
		#endregion


		protected Drawing.Point					windowLocation;
		protected Drawing.Size					windowSize;
		protected bool							isFullScreen;
		protected System.Collections.Hashtable	windowBounds;
		protected double						screenDpi;
		protected string						adorner;
		protected double						defaultZoom;
		protected MouseWheelAction				mouseWheelAction;
		protected bool							fineCursor;
		protected bool							splashScreen;
		protected FirstAction					firstAction;
		protected System.Collections.ArrayList	lastFilename;
		protected int							lastFilenameMax;
		protected string						initialDirectory;
		protected bool							labelProperties;
		protected Drawing.ColorCollection		colorCollection;
		protected string						colorCollectionDirectory;
		protected string						colorCollectionFilename;


		#region WindowBounds
		[System.Serializable()]
		protected class WindowBounds : ISerializable
		{
			public WindowBounds(Drawing.Point location, Drawing.Size size)
			{
				this.location = location;
				this.size = size;
			}

			public Drawing.Point Location
			{
				get { return this.location; }
				set { this.location = value; }
			}

			public Drawing.Size Size
			{
				get { return this.size; }
				set { this.size = value; }
			}

			// Sérialise les réglages.
			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("Location", this.location);
				info.AddValue("Size", this.size);
			}

			// Constructeur qui désérialise les réglages.
			protected WindowBounds(SerializationInfo info, StreamingContext context)
			{
				this.location = (Drawing.Point) info.GetValue("Location", typeof(Drawing.Point));
				this.size = (Drawing.Size) info.GetValue("Size", typeof(Drawing.Size));
			}
		
			protected Drawing.Point					location;
			protected Drawing.Size					size;
		}
		#endregion
	}
}
