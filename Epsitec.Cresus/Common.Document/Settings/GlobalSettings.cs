using Epsitec.Common.Document;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using System.Runtime.Serialization;

using System.Collections.Generic;
using System;

namespace Epsitec.Common.Document.Settings
{
	public enum FirstAction
	{
		Nothing         = 0,
		OpenNewDocument = 1,
		OpenLastFile    = 2,
		OpenLastFiles   = 3,
		OpenLastModel   = 4,
		OpenLastModels  = 5,
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
	public class GlobalSettings : ISerializable, Epsitec.Common.Dialogs.IFavoritesSettings
	{
		public GlobalSettings()
		{
			this.windowLocation = new Drawing.Point(0, 0);
			this.windowSize = new Drawing.Size(830, 620);
			this.isFullScreen = false;
			this.windowBounds = new System.Collections.Hashtable();

			this.screenDpi = 96.0;
			this.adorner = "LookMetal";
			this.defaultZoom = 1.5;
			this.mouseWheelAction = MouseWheelAction.Zoom;
			this.fineCursor = false;
			this.splashScreen = true;
			this.firstAction = FirstAction.OpenNewDocument;
			this.lastModel = new System.Collections.ArrayList();
			this.lastModelMax = 10;
			this.lastFilename = new System.Collections.ArrayList();
			this.lastFilenameMax = 10;
			this.favoritesList = new Epsitec.Common.Types.Collections.ObservableList<string> ();
			this.favoritesBig = true;
			this.labelProperties = true;
			this.fineCursor = false;
			this.fineCursor = false;
			this.quickCommands = GlobalSettings.DefaultQuickCommands();

			//	Suppose que le dossier des exemples est dans le même dossier
			//	que l'application.
			this.initialDirectory = Document.OriginalSamplesPath;
			this.newDocument      = Document.OriginalSamplesPath;

			this.colorCollection = new ColorCollection();
			this.colorCollectionDirectory = "";
			this.colorCollectionFilename = "";

			this.autoChecker = true;
			this.dateChecker = Common.Types.Date.Today;
			this.dateChecker = this.dateChecker.AddDays(-1);
		}

		public void Initialize(DocumentType type)
		{
			//	Met tous les fichiers d'exemples dans la liste des 10 premiers
			//	fichiers récents.
#if false
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
				for ( int i=0 ; i<this.lastFilenameMax ; i++ )
				{
					if ( i >= list.Length )  break;
					this.LastFilenameAdd(list[i]);
				}
			}
			catch
			{
			}

			if (type != DocumentType.Pictogram)
			{
				try
				{
					string[] list = System.IO.Directory.GetFiles(this.initialDirectory, "*.crmod");
					this.LastModelAdd(Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument);
					for (int i=0; i<this.lastModelMax-1; i++)
					{
						if (i >= list.Length)  break;
						this.LastModelAdd(list[i]);
					}
				}
				catch
				{
				}
			}

			if (type != DocumentType.Pictogram)
			{
				this.newDocument = this.initialDirectory;
			}
#endif
		}


		public Drawing.Rectangle MainWindowBounds
		{
			//	Fenêtre principale de l'application.
			get
			{
				Drawing.Rectangle rect = new Rectangle(this.windowLocation, this.windowSize);
				if ( this.windowLocation.IsZero )
				{
					//	Lors de la première exécution, met l'application au centre
					//	de la fenêtre.
					ScreenInfo si = ScreenInfo.Find(Message.CurrentState.LastScreenPosition);
					Rectangle area = si.WorkingArea;
					rect = new Rectangle(area.Center-rect.Size/2, area.Center+rect.Size/2);
				}
				return ScreenInfo.FitIntoWorkingArea(rect);
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


		public void SetWindowBounds(string name, Drawing.Point location, Drawing.Size size)
		{
			//	Ajoute une définition de fenêtre.
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

		public bool GetWindowBounds(string name, out Drawing.Point location, out Drawing.Size size)
		{
			//	Cherche une définition de fenêtre.
			WindowBounds wb = this.windowBounds[name] as WindowBounds;
			if (wb == null)
			{
				location = Drawing.Point.Zero;
				size = Drawing.Size.Zero;
				return false;
			}
			else
			{
				location = wb.Location;
				size = wb.Size;
				return true;
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

		public string NewDocument
		{
			//	Dossier à utiliser pour les fichiers modèles (*.crmod).
			get
			{
				return this.newDocument;
			}

			set
			{
				this.newDocument = value;
			}
		}


		public int LastModelCount
		{
			get
			{
				return this.lastModel.Count;
			}
		}

		public string LastModelGet(int index)
		{
			return this.lastModel[index] as string;
		}

		public string LastModelGetShort(int index)
		{
			string last = this.LastModelGet(index);
			if (last == Epsitec.Common.Dialogs.AbstractFileDialog.NewEmptyDocument)  // nouveau document vide ?
			{
				return Res.Strings.File.Model.Empty;
			}
			else
			{
				return Misc.ExtractName(last);
			}
		}

		public void LastModelAdd(string filename)
		{
			int index = this.LastModelSearch(filename);
			if ( index < 0 )
			{
				this.LastModelTrunc();
			}
			else
			{
				this.lastModel.RemoveAt(index);
			}
			this.lastModel.Insert(0, filename);

			FileManager.AddToRecentDocuments(filename);
		}

		protected int LastModelSearch(string filename)
		{
			for ( int i=0 ; i<this.lastModel.Count ; i++ )
			{
				string s = this.lastModel[i] as string;
				if ( s == filename )  return i;
			}
			return -1;
		}

		protected void LastModelTrunc()
		{
			if ( this.lastModel.Count < this.lastModelMax )  return;
			this.lastModel.RemoveAt(this.lastModel.Count-1);
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

			FileManager.AddToRecentDocuments(filename);
		}

		protected int LastFilenameSearch(string filename)
		{
			for (int i=0; i<this.lastFilename.Count; i++)
			{
				string s = this.lastFilename[i] as string;
				if (string.Equals (s, filename, System.StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		protected void LastFilenameTrunc()
		{
			if ( this.lastFilename.Count < this.lastFilenameMax )  return;
			this.lastFilename.RemoveAt(this.lastFilename.Count-1);
		}


		IList<string> Epsitec.Common.Dialogs.IFavoritesSettings.Items
		{
			get
			{
				return this.favoritesList;
			}
		}

		bool Epsitec.Common.Dialogs.IFavoritesSettings.UseLargeIcons
		{
			get
			{
				return this.favoritesBig;
			}
			set
			{
				this.favoritesBig = value;
			}
		}


		public string InitialDirectory
		{
			//	Dossier à utiliser pour les fichiers documents (*.crdoc).
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

		public bool AutoChecker
		{
			get
			{
				return this.autoChecker;
			}

			set
			{
				this.autoChecker = value;
			}
		}

		public Common.Types.Date DateChecker
		{
			get
			{
				return this.dateChecker;
			}

			set
			{
				this.dateChecker = value;
			}
		}


		#region FirstAction
		public static int FirstActionCount
		{
			get { return 4; }
		}

		public static string FirstActionString(FirstAction action)
		{
			switch ( action )
			{
				case FirstAction.Nothing:          return Res.Strings.Dialog.Settings.FirstAction.Nothing;
				case FirstAction.OpenNewDocument:  return Res.Strings.Dialog.Settings.FirstAction.OpenNewDocument;
				case FirstAction.OpenLastFile:     return Res.Strings.Dialog.Settings.FirstAction.OpenLastFile;
				case FirstAction.OpenLastFiles:    return Res.Strings.Dialog.Settings.FirstAction.OpenLastFiles;
				case FirstAction.OpenLastModel:    return Res.Strings.Dialog.Settings.FirstAction.OpenLastModel;
				case FirstAction.OpenLastModels:   return Res.Strings.Dialog.Settings.FirstAction.OpenLastModels;
			}
			return "?";
		}

		public static FirstAction FirstActionType(int rank)
		{
			switch ( rank )
			{
				case 0:  return FirstAction.Nothing;
				case 1:  return FirstAction.OpenNewDocument;
				case 2:  return FirstAction.OpenLastModel;
				case 3:  return FirstAction.OpenLastFile;
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


		#region QuickCommands
		public System.Collections.ArrayList QuickCommands
		{
			//	Donne la liste des commandes rapides sous la forme "US:Name".
			//	U -> '1' si utilisé
			//	S -> '1' si séparateur après
			//	Name -> nom de la commande
			get
			{
				return this.quickCommands;
			}

			set
			{
				this.quickCommands = value;
			}
		}

		public static System.Collections.ArrayList DefaultQuickCommands()
		{
			//	Donne la liste des commandes rapides par défaut.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			list.Add("10:New");
			list.Add("10:Open");
			list.Add("10:Save");
			list.Add("00:SaveAs");
			list.Add("11:Print");
			list.Add("00:Export");
			list.Add("00:CloseAll");
			list.Add("00:SaveModel");

			list.Add("10:Undo");
			list.Add("11:Redo");
			
			list.Add("10:Delete");
			list.Add("11:Duplicate");
			
			list.Add("10:Cut");
			list.Add("10:Copy");
			list.Add("11:Paste");
			
			list.Add("00:DeselectAll");
			list.Add("00:SelectAll");
			list.Add("00:SelectInvert");
			list.Add("00:HideSel");
			list.Add("00:HideRest");
			list.Add("00:HideCancel");
			list.Add("00:HideHalf");

			list.Add("00:OrderUpAll");
			list.Add("00:OrderDownAll");
			list.Add("00:OrderUpOne");
			list.Add("00:OrderDownOne");

			list.Add("00:Group");
			list.Add("00:Merge");
			list.Add("00:Extract");
			list.Add("00:Ungroup");
			list.Add("00:Inside");
			list.Add("00:Outside");

			list.Add("00:Rotate90");
			list.Add("00:Rotate180");
			list.Add("00:Rotate270");
			list.Add("00:MirrorH");
			list.Add("00:MirrorV");
			list.Add("00:ScaleDiv2");
			list.Add("00:ScaleMul2");

			list.Add("00:AlignLeft");
			list.Add("00:AlignCenterX");
			list.Add("00:AlignRight");
			list.Add("00:AlignTop");
			list.Add("00:AlignCenterY");
			list.Add("00:AlignBottom");
			list.Add("00:AlignGrid");
			list.Add("00:ShareLeft");
			list.Add("00:ShareCenterX");
			list.Add("00:ShareSpaceX");
			list.Add("00:ShareRight");
			list.Add("00:ShareTop");
			list.Add("00:ShareCenterY");
			list.Add("00:ShareSpaceY");
			list.Add("00:ShareBottom");
			list.Add("00:AdjustWidth");
			list.Add("00:AdjustHeight");

			list.Add("00:Combine");
			list.Add("00:Uncombine");
			list.Add("00:Reset");
			list.Add("00:ToBezier");
			list.Add("00:ToPoly");
			list.Add("00:Fragment");

			list.Add("00:BooleanOr");
			list.Add("00:BooleanAnd");
			list.Add("00:BooleanXor");
			list.Add("00:BooleanFrontMinus");
			list.Add("00:BooleanBackMinus");

			list.Add("00:ColorToRgb");
			list.Add("00:ColorToCmyk");
			list.Add("00:ColorToGray");

			list.Add("00:ParagraphLeadingPlus");
			list.Add("00:ParagraphLeadingMinus");
			list.Add("00:ParagraphIndentPlus");
			list.Add("00:ParagraphIndentMinus");
			list.Add("00:ParagraphClear");
			
			list.Add("00:" + Res.Commands.FontBold.CommandId);
			list.Add("00:" + Res.Commands.FontItalic.CommandId);
			list.Add("00:" + Res.Commands.FontUnderline.CommandId);
			list.Add("00:FontOverline");
			list.Add("00:FontStrikeout");
			list.Add("00:FontSizePlus");
			list.Add("00:FontSizeMinus");
			list.Add("00:FontClear");
			list.Add("00:TextShowControlCharacters");
			list.Add("00:Replace");
			list.Add("00:TextInsertNewFrame");
			list.Add("00:TextInsertNewPage");
			list.Add("00:TextInsertQuad");
			list.Add("00:Glyphs");
			
			list.Add("00:ZoomMin");
			list.Add("00:ZoomPage");
			list.Add("00:ZoomPageWidth");
			list.Add("00:ZoomDefault");
			list.Add("00:ZoomSel");
			list.Add("00:ZoomSelWidth");
			list.Add("00:ZoomPrev");
			list.Add("00:ZoomSub");
			list.Add("00:ZoomAdd");

			list.Add("10:Preview");
			list.Add("11:Grid");
			list.Add("00:Magnet");
			list.Add("00:MagnetLayer");
			list.Add("00:Rulers");
			list.Add("00:Labels");
			list.Add("00:Aggregates");
			list.Add("00:Constrain");

			list.Add("00:Settings");
			list.Add("00:Infos");
			list.Add("00:PageStack");

			return list;
		}

		protected void UpdateQuickCommands()
		{
			//	Met à jour la liste des commandes rapides en fonction d'éventuelles commandes qui n'y
			//	seraient pas encore.
			System.Collections.ArrayList all = GlobalSettings.DefaultQuickCommands();

			for ( int i=0 ; i<all.Count ; i++ )
			{
				string cmd = GlobalSettings.QuickCmd(all[i] as string);
				if ( this.SearchQuickList(cmd) != -1 )  continue;
				int index = this.IndexQuickExisting(all, i);
				this.quickCommands.Insert(index, GlobalSettings.QuickXcmd(false, false, cmd));
			}

			//	Supprime les commandes qui n'existent plus dans cette version du logiciel.
			System.Collections.ArrayList purged = new System.Collections.ArrayList();
			foreach ( string xcmd in this.quickCommands )
			{
				string cmd = GlobalSettings.QuickCmd(xcmd as string);
				if ( this.SearchQuickList(all, cmd) != -1 )
				{
					purged.Add(xcmd);
				}
			}
			this.quickCommands = purged;
		}

		protected int IndexQuickExisting(System.Collections.ArrayList all, int i)
		{
			while ( true )
			{
				if ( i == 0 )  return 0;
				i --;
				string cmd = GlobalSettings.QuickCmd(all[i] as string);
				int index = this.SearchQuickList(cmd);
				if ( index != -1 )  return index+1;
			}
		}

		protected int SearchQuickList(string cmd)
		{
			for ( int i=0 ; i<this.quickCommands.Count ; i++ )
			{
				string xcmd = this.quickCommands[i] as string;
				if ( cmd == GlobalSettings.QuickCmd(xcmd) )  return i;
			}
			return -1;
		}

		protected int SearchQuickList(System.Collections.ArrayList all, string cmd)
		{
			for ( int i=0 ; i<all.Count ; i++ )
			{
				string xcmd = all[i] as string;
				if ( cmd == GlobalSettings.QuickCmd(xcmd) )  return i;
			}
			return -1;
		}

		public static string QuickXcmd(bool used, bool sep, string cmd)
		{
			//	Donne la commande étendue à partir de ses composantes.
			return string.Format("{0}{1}:{2}", used?"1":"0", sep?"1":"0", cmd);
		}

		public static bool QuickUsed(string xcmd)
		{
			//	Indique si une commande est utilisée (visible dans la barre d'icônes).
			return xcmd[0] == '1';
		}

		public static bool QuickSep(string xcmd)
		{
			//	Indique si une commande est suivie d'un séparateur.
			return xcmd[1] == '1';
		}

		public static string QuickCmd(string xcmd)
		{
			//	Donne le nom d'une commande.
			int index = xcmd.IndexOf(":");
			if ( index == -1 )  return "";
			return xcmd.Substring(index+1);
		}
		#endregion


		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise les réglages.
			info.AddValue("Version", 8);

			info.AddValue("WindowLocation", this.windowLocation);
			info.AddValue("WindowSize", this.windowSize);
			info.AddValue("IsFullScreen", this.isFullScreen);
			info.AddValue("WindowBounds", this.windowBounds);

			System.Collections.ArrayList serializableFavoritesList = new System.Collections.ArrayList (this.favoritesList);

			info.AddValue("ScreenDpi", this.screenDpi);
			info.AddValue("Adorner", this.adorner);
			info.AddValue("DefaultZoom", this.defaultZoom);
			info.AddValue("MouseWheelAction", this.mouseWheelAction);
			info.AddValue("FineCursor", this.fineCursor);
			info.AddValue("SplashScreen", this.splashScreen);
			info.AddValue("FirstAction", this.firstAction);
			info.AddValue("NewDocument", this.newDocument);
			info.AddValue("LastModel", this.lastModel);
			info.AddValue("LastFilename", this.lastFilename);
			info.AddValue("InitialDirectory", this.initialDirectory);
			info.AddValue("FavoritesList", serializableFavoritesList);
			info.AddValue("FavoritesBig", this.favoritesBig);
			info.AddValue("LabelProperties", this.labelProperties);

			info.AddValue("ColorCollection", this.colorCollection);
			info.AddValue("ColorCollectionDirectory", this.colorCollectionDirectory);
			info.AddValue("ColorCollectionFilename", this.colorCollectionFilename);

			info.AddValue("AutoChecker", this.autoChecker);
			info.AddValue("DateChecker", this.dateChecker.Ticks);

			info.AddValue("QuickCommands", this.quickCommands);
		}

		protected GlobalSettings(SerializationInfo info, StreamingContext context) : this()
		{
			//	Constructeur qui désérialise les réglages.
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

			if ( version >= 3 )
			{
				this.autoChecker = info.GetBoolean("AutoChecker");
				this.dateChecker = new Types.Date(info.GetInt64("DateChecker"));
			}

			if ( version >= 4 )
			{
				this.quickCommands = (System.Collections.ArrayList) info.GetValue("QuickCommands", typeof(System.Collections.ArrayList));
				this.UpdateQuickCommands();
			}
			else
			{
				this.quickCommands = GlobalSettings.DefaultQuickCommands();
			}

			if ( version >= 6 )
			{
				this.newDocument = info.GetString("NewDocument");
			}
			else
			{
				this.newDocument = "";
			}

			if ( version >= 7 )
			{
				this.lastModel = (System.Collections.ArrayList) info.GetValue("LastModel", typeof(System.Collections.ArrayList));
			}
			else
			{
				this.lastModel = new System.Collections.ArrayList();
			}

			this.CleanUpLastFiles (this.lastFilename);
			this.CleanUpLastFiles (this.lastModel);

			if ( version >= 8 )
			{
				System.Collections.ArrayList serializableFavoritesList = (System.Collections.ArrayList) info.GetValue("FavoritesList", typeof(System.Collections.ArrayList));

				this.favoritesList = new Epsitec.Common.Types.Collections.ObservableList<string> ();
				this.favoritesBig = info.GetBoolean("FavoritesBig");

				foreach (string item in serializableFavoritesList)
				{
					this.favoritesList.Add (item);
				}
			}
			else
			{
				this.favoritesList = new Epsitec.Common.Types.Collections.ObservableList<string> ();
				this.favoritesBig = true;
			}
		}
		
		private void CleanUpLastFiles(System.Collections.ArrayList paths)
		{
			var alive = new List<string> ();
			
			foreach (string name in paths)
			{
				try
				{
					if (System.IO.File.Exists (name))
					{
						alive.Add (name);
					}
				}
				catch
				{
				}
			}

			paths.Clear ();
			paths.AddRange (alive);
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
		protected string						newDocument;
		protected System.Collections.ArrayList	lastModel;
		protected int							lastModelMax;
		protected System.Collections.ArrayList	lastFilename;
		protected int							lastFilenameMax;
		protected string						initialDirectory;
		protected Epsitec.Common.Types.Collections.ObservableList<string> favoritesList;
		protected bool							favoritesBig;
		protected bool							labelProperties;
		protected Drawing.ColorCollection		colorCollection;
		protected string						colorCollectionDirectory;
		protected string						colorCollectionFilename;
		protected bool							autoChecker;
		protected Common.Types.Date				dateChecker;
		protected System.Collections.ArrayList	quickCommands;


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

			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				//	Sérialise les réglages.
				info.AddValue("Location", this.location);
				info.AddValue("Size", this.size);
			}

			protected WindowBounds(SerializationInfo info, StreamingContext context)
			{
				//	Constructeur qui désérialise les réglages.
				this.location = (Drawing.Point) info.GetValue("Location", typeof(Drawing.Point));
				this.size = (Drawing.Size) info.GetValue("Size", typeof(Drawing.Size));
			}
		
			protected Drawing.Point					location;
			protected Drawing.Size					size;
		}
		#endregion
	}
}
