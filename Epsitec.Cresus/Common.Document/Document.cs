using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Document
{
	public enum DocumentType
	{
		Graphic,		// document graphique
		Pictogram,		// icône
		Text,			// document texte
	}

	public enum DocumentMode
	{
		ReadOnly,		// document uniquement affichable
		Modify,			// document modifiable
		Clipboard,		// document servant uniquement de bloc-notes
	}

	/// <summary>
	/// Summary description for Document.
	/// </summary>
	[System.Serializable()]
	public class Document : ISerializable
	{
		public enum IOType
		{
			Unknow,				// format inconnu
			BinaryCompress,		// format standard
			SoapUncompress,		// format de debug
		}

		// Crée un nouveau document vide.
		public Document(DocumentType type, DocumentMode mode, Settings.GlobalSettings globalSettings, CommandDispatcher commandDispatcher)
		{
			this.type = type;
			this.mode = mode;
			this.globalSettings = globalSettings;
			this.commandDispatcher = commandDispatcher;

			if ( this.type == DocumentType.Pictogram )
			{
				this.size = new Size(20, 20);
			}
			else
			{
				this.size = new Size(2100, 2970);  // A4 vertical
			}

			this.hotSpot = new Point(0, 0);
			this.objects = new UndoableList(this, UndoableListType.ObjectsInsideDocument);
			this.propertiesAuto = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.propertiesSel = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.propertiesStyle = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.exportDirectory = "";
			this.exportFilename = "";
			this.exportFilter = 0;
			this.printerName = "";

			if ( this.mode == DocumentMode.Modify    ||
				 this.mode == DocumentMode.Clipboard )
			{
				this.modifier = new Modifier(this);
				this.notifier = new Notifier(this);
				this.dialogs  = new Dialogs(this);
				this.settings = new Settings.Settings(this);
				this.printer  = new Printer(this);
			}

			if ( this.mode == DocumentMode.Clipboard )
			{
				Viewer clipboardViewer = new Viewer(this);
				this.Modifier.ActiveViewer = clipboardViewer;
				this.Modifier.AttachViewer(clipboardViewer);
				this.Modifier.New();
			}

			this.ioType = IOType.BinaryCompress;
			//this.ioType = IOType.SoapUncompress;
		}

		// Type de ce document.
		public DocumentType Type
		{
			get { return this.type; }
		}

		// Mode de travail pour ce document.
		public DocumentMode Mode
		{
			get { return this.mode; }
		}

		// Réglages globaux.
		public Settings.GlobalSettings GlobalSettings
		{
			get { return this.globalSettings; }
		}
		
		// CommandDispatcher de l'éditeur.
		public CommandDispatcher CommandDispatcher
		{
			get { return this.commandDispatcher; }
		}
		
		// Nom du document.
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		// Bloc-notes associé.
		public Document Clipboard
		{
			get { return this.clipboard; }
			set { this.clipboard = value; }
		}

		// Liste des objets de ce document.
		public UndoableList GetObjects
		{
			get { return this.objects; }
		}


		// Liste des propriétés automatiques de ce document.
		public UndoableList PropertiesAuto
		{
			get { return this.propertiesAuto; }
		}

		// Liste des propriétés sélectionnées de ce document.
		public UndoableList PropertiesSel
		{
			get { return this.propertiesSel; }
		}

		// Liste des styles de ce document.
		public UndoableList PropertiesStyle
		{
			get { return this.propertiesStyle; }
		}


		// Réglages de ce document.
		public Settings.Settings Settings
		{
			get { return this.settings; }
		}

		// Modificateur éventuel pour ce document.
		public Modifier Modifier
		{
			get { return this.modifier; }
		}

		// Notificateur éventuel pour ce document.
		public Notifier Notifier
		{
			get { return this.notifier; }
		}

		// Dialogues éventuels pour ce document.
		public Dialogs Dialogs
		{
			get { return this.dialogs; }
		}

		// Imprimeur pour ce document.
		public Printer Printer
		{
			get { return this.printer; }
			set { this.printer = value; }
		}
		
		// Dialogue d'impression pour ce document.
		public Common.Dialogs.Print PrintDialog
		{
			get
			{
				if ( this.printDialog == null )
				{
					this.printDialog = new Common.Dialogs.Print();
					this.printDialog.Document.SelectPrinter(this.PrinterName);
					this.printDialog.AllowFromPageToPage = true;
					this.printDialog.AllowSelectedPages  = true;
				}
				return this.printDialog;
			}
		}


		// Taille du document.
		public Size Size
		{
			get
			{
				return this.size;
			}
			
			set
			{
				if ( this.size != value )
				{
					this.size = value;
					this.IsDirtySerialize = true;

					if ( this.Modifier != null )
					{
						this.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
					}

					if ( this.notifier != null )
					{
						this.Notifier.NotifyAllChanged();
					}
				}
			}
		}

		// Point chaud du document.
		public Point HotSpot
		{
			get
			{
				return this.hotSpot;
			}
			
			set
			{
				if ( this.hotSpot != value )
				{
					this.hotSpot = value;
					this.IsDirtySerialize = true;
				}
			}
		}

		// Nom du fichier associé.
		public string Filename
		{
			get
			{
				return this.filename;
			}

			set
			{
				if ( this.filename != value )
				{
					this.filename = value;

					if ( this.Notifier != null )
					{
						this.Notifier.NotifyDocumentChanged();
					}
				}
			}
		}

		// Nom du dossier d'exportation associé.
		public string ExportDirectory
		{
			get
			{
				return this.exportDirectory;
			}

			set
			{
				this.exportDirectory = value;
			}
		}

		// Nom du fichier (sans dossier) d'exportation associé.
		public string ExportFilename
		{
			get
			{
				return this.exportFilename;
			}

			set
			{
				this.exportFilename = value;
			}
		}

		// Type du fichier d'exportation associé.
		public int ExportFilter
		{
			get
			{
				return this.exportFilter;
			}

			set
			{
				this.exportFilter = value;
			}
		}

		// Nom de l'imprimante utilisée.
		public string PrinterName
		{
			get
			{
				return this.printerName;
			}

			set
			{
				if ( this.printerName != value )
				{
					this.printerName = value;
					
					if ( this.printDialog != null )
					{
						this.printDialog.Document.SelectPrinter(this.PrinterName);
					}
				}
			}
		}

		// Indique si la sérialisation est nécessaire.
		public bool IsDirtySerialize
		{
			get
			{
				return this.isDirtySerialize;
			}

			set
			{
				if ( this.isDirtySerialize != value )
				{
					this.isDirtySerialize = value;

					if ( this.Notifier != null )
					{
						this.Notifier.NotifySaveChanged();
					}
				}
			}
		}

		// Ouvre un document existant sur disque.
		public string Read(string filename)
		{
			if ( this.Modifier != null )
			{
				this.Modifier.New();
			}

			try
			{
				using ( Stream stream = File.OpenRead(filename) )
				{
					string err = this.Read(stream, System.IO.Path.GetDirectoryName(filename));
					if ( err == "" )
					{
						this.Filename = filename;
						this.globalSettings.LastFilenameAdd(filename);
						this.IsDirtySerialize = false;
					}
					else
					{
						this.IsDirtySerialize = false;
					}
					return err;
				}
			}
			catch ( System.Exception e )
			{
				this.IsDirtySerialize = false;
				return e.Message;
			}
		}

		// Ouvre un document sérialisé, soit parce que l'utilisateur veut ouvrir
		// explicitement un fichier, soit par Engine.
		public string Read(Stream stream, string directory)
		{
			this.ioDirectory = directory;
			this.readWarnings = new System.Collections.ArrayList();

			IOType type = Document.ReadIdentifier(stream);
			if ( type == IOType.Unknow )
			{
				return "Type de fichier incorrect.";
			}

			// Initialise la variable statique utilisée par VersionDeserializationBinder.
			// Exemple de contenu:
			// "Common.Document, Version=1.0.1777.18519, Culture=neutral, PublicKeyToken=7344997cc606b490"
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetAssembly(this.GetType());
			Document.AssemblyFullName = ass.FullName;

			// Initialise la variable statique permettant à tous les constructeurs
			// de connaître le pointeur au document.
			Document.ReadDocument = this;

			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = false;
			}

			Document doc = null;

			if ( type == IOType.BinaryCompress )
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Binder = new VersionDeserializationBinder();

				try
				{
					string compressorName;
					using ( Stream compressor = IO.Decompression.CreateStream(stream, out compressorName) )
					{
						doc = (Document) formatter.Deserialize(compressor);
					}
				}
				catch ( System.Exception e )
				{
					if ( this.Modifier != null )
					{
						this.Modifier.OpletQueueEnable = true;
					}
					return e.Message;
				}
			}
			if ( type == IOType.SoapUncompress )
			{
				SoapFormatter formatter = new SoapFormatter();
				formatter.Binder = new VersionDeserializationBinder();

				try
				{
					doc = (Document) formatter.Deserialize(stream);
				}
				catch ( System.Exception e )
				{
					if ( this.Modifier != null )
					{
						this.Modifier.OpletQueueEnable = true;
					}
					return e.Message;
				}
			}

			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = true;
			}

			this.objects = doc.objects;
			this.propertiesAuto = doc.propertiesAuto;
			this.propertiesStyle = doc.propertiesStyle;
			
			if ( this.type == DocumentType.Pictogram )
			{
				this.size = doc.size;
				this.hotSpot = doc.hotSpot;
			}
			else
			{
				this.settings = doc.settings;
				this.exportDirectory = doc.exportDirectory;
				this.exportFilename = doc.exportFilename;
				this.exportFilter = doc.exportFilter;
				this.printerName = doc.printerName;
			}

			this.ReadFinalize();

			if ( this.Notifier != null )
			{
				this.Notifier.NotifyAllChanged();
				this.Modifier.DirtyCounters();
			}
			return "";
		}

		// Retourne la liste éventuelle des warnings de lecture.
		public System.Collections.ArrayList ReadWarnings
		{
			get
			{
				return this.readWarnings;
			}
		}

		sealed class VersionDeserializationBinder : SerializationBinder 
		{
			// Retourne un type correspondant à l'application courante, afin
			// d'accepter de désérialiser un fichier généré par une application
			// ayant un autre numéro de révision.
			// Application courante: Version=1.0.1777.18519
			// Version dans le fichier: Version=1.0.1777.11504
			public override System.Type BindToType(string assemblyName, string typeName) 
			{
				System.Type typeToDeserialize;
				typeToDeserialize = System.Type.GetType(string.Format("{0}, {1}", typeName, Document.AssemblyFullName));
				return typeToDeserialize;
			}
		}

		// Utilisé par les constructeurs de désérialisation du genre:
		// protected Toto(SerializationInfo info, StreamingContext context)
		public static Document ReadDocument = null;
		protected static string AssemblyFullName = "";

		// Adapte tous les objets après une désérialisation.
		protected void ReadFinalize()
		{
			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = false;
			}

			Font.FaceInfo[] fonts = Font.Faces;
			foreach ( Objects.Abstract obj in this.Deep(null) )
			{
				obj.ReadFinalize();
				obj.ReadCheckWarnings(fonts, this.readWarnings);
			}

			if ( this.settings != null )
			{
				this.settings.ReadFinalize();
			}

			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = true;
			}
		}

		// Enregistre le document sur disque.
		public string Write(string filename)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			this.ioDirectory = System.IO.Path.GetDirectoryName(filename);

			if ( File.Exists(filename) )
			{
				File.Delete(filename);
			}

			try
			{
				using ( Stream stream = File.OpenWrite(filename) )
				{
					Document.WriteIdentifier(stream, this.ioType);

					if ( this.ioType == IOType.BinaryCompress )
					{
						Stream compressor = IO.Compression.CreateBZip2Stream(stream);
						BinaryFormatter formatter = new BinaryFormatter();
						formatter.Serialize(compressor, this);
						compressor.Close();
					}
					if ( this.ioType == IOType.SoapUncompress )
					{
						SoapFormatter formatter = new SoapFormatter();
						formatter.Serialize(stream, this);
					}
				}
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			this.Filename = filename;
			this.IsDirtySerialize = false;
			return "";
		}

		// Lit les 8 bytes d'en-tête et vérifie qu'ils contiennent bien "<?icon?>".
		protected static IOType ReadIdentifier(Stream stream)
		{
			byte[] buffer = new byte[8];
			stream.Read(buffer, 0, 8);
			if ( buffer[0] != (byte) '<' )  return IOType.Unknow;
			if ( buffer[1] != (byte) '?' )  return IOType.Unknow;
			if ( buffer[2] != (byte) 'i' )  return IOType.Unknow;
			if ( buffer[3] != (byte) 'c' )  return IOType.Unknow;
			if ( buffer[4] != (byte) 'o' )  return IOType.Unknow;
			if ( buffer[6] != (byte) '?' )  return IOType.Unknow;
			if ( buffer[7] != (byte) '>' )  return IOType.Unknow;

			if ( buffer[5] == (byte) 'n' )  return IOType.BinaryCompress;
			if ( buffer[5] == (byte) 'x' )  return IOType.SoapUncompress;
			return IOType.Unknow;
		}

		// Ecrit les 8 bytes d'en-tête "<?icon?>".
		protected static void WriteIdentifier(Stream stream, IOType type)
		{
			byte[] buffer = new byte[8];
			buffer[0] = (byte) '<';
			buffer[1] = (byte) '?';
			buffer[2] = (byte) 'i';
			buffer[3] = (byte) 'c';
			buffer[4] = (byte) 'o';
			buffer[5] = (byte) '-';
			if ( type == IOType.BinaryCompress ) buffer[5] = (byte) 'n';
			if ( type == IOType.SoapUncompress ) buffer[5] = (byte) 'x';
			buffer[6] = (byte) '?';
			buffer[7] = (byte) '>';
			stream.Write(buffer, 0, 8);
		}

		#region Serialization
		// Sérialise le document.
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			int revision = 1;
			int version  = 1;
			info.AddValue("Revision", revision);
			info.AddValue("Version", version);
			info.AddValue("Type", this.type);
			info.AddValue("Name", this.name);

			if ( this.type == DocumentType.Pictogram )
			{
				info.AddValue("Size", this.size);
				info.AddValue("HotSpot", this.hotSpot);
			}
			else
			{
				info.AddValue("Settings", this.settings);
				info.AddValue("ExportFilename", this.exportFilename);
				info.AddValue("ExportFilter", this.exportFilter);
				info.AddValue("PrinterName", this.printerName);
			}

			info.AddValue("UniqueObjectId", this.modifier.UniqueObjectId);
			info.AddValue("UniqueStyleId", this.modifier.UniqueStyleId);
			info.AddValue("Objects", this.objects);
			info.AddValue("Properties", this.propertiesAuto);
			info.AddValue("Styles", this.propertiesStyle);
		}

		// Constructeur qui désérialise le document.
		protected Document(SerializationInfo info, StreamingContext context)
		{
			this.readRevision = info.GetInt32("Revision");
			this.readVersion  = info.GetInt32("Version");
			this.type = (DocumentType) info.GetValue("Type", typeof(DocumentType));
			this.name = info.GetString("Name");

			if ( this.type == DocumentType.Pictogram )
			{
				this.size = (Size) info.GetValue("Size", typeof(Size));
				this.hotSpot = (Point) info.GetValue("HotSpot", typeof(Point));
			}
			else
			{
				this.settings = (Settings.Settings) info.GetValue("Settings", typeof(Settings.Settings));

				if ( this.IsRevisionGreaterOrEqual(1,1) )
				{
					this.exportDirectory = "";
					this.exportFilename = info.GetString("ExportFilename");
					this.exportFilter = info.GetInt32("ExportFilter");
					this.printerName = info.GetString("PrinterName");
				}
				else
				{
					this.exportDirectory = "";
					this.exportFilename = "";
					this.exportFilter = 0;
					this.printerName = "";
				}
			}

			if ( this.modifier != null )
			{
				this.modifier.UniqueObjectId = info.GetInt32("UniqueObjectId");
				this.modifier.UniqueStyleId = info.GetInt32("UniqueStyleId");
			}
			this.objects = (UndoableList) info.GetValue("Objects", typeof(UndoableList));
			this.propertiesAuto = (UndoableList) info.GetValue("Properties", typeof(UndoableList));
			this.propertiesStyle = (UndoableList) info.GetValue("Styles", typeof(UndoableList));
		}

		// Retourne le nom du dossier en cours de lecture/écriture.
		public string IoDirectory
		{
			get { return this.ioDirectory; }
		}

		// Indique si un fichier est compatible avec une révision/version.
		public bool IsRevisionGreaterOrEqual(int revision, int version)
		{
			if ( this.readRevision > revision )
			{
				return true;
			}

			if ( this.readRevision < revision )
			{
				return false;
			}

			return ( this.readVersion >= version );
		}
		#endregion

		
		// Dessine le document.
		public void Paint(Graphics graphics, DrawingContext drawingContext, Rectangle clipRect)
		{
			if ( drawingContext.RootStackIsEmpty )  return;

			if ( !clipRect.IsInfinite )
			{
				clipRect = drawingContext.Viewer.ScreenToInternal(clipRect);
			}

			Objects.Abstract page = drawingContext.RootObject(1);

			if ( drawingContext.PreviewActive )
			{
				Rectangle initialClip = Rectangle.Empty;
				if ( this.Modifier != null )
				{
					initialClip = graphics.SaveClippingRectangle();
					clipRect = Rectangle.Intersection(clipRect, this.Modifier.PageArea);
					Rectangle clip = drawingContext.Viewer.InternalToScreen(clipRect);
					clip = drawingContext.Viewer.MapClientToRoot(clip);
					graphics.SetClippingRectangle(clip);
				}

				foreach ( Objects.Layer layer in this.Flat(page) )
				{
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;

					Properties.ModColor modColor = layer.PropertyModColor;
					drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
					drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

					foreach ( Objects.Abstract obj in this.Deep(layer) )
					{
						if ( obj.IsHide )  continue;  // objet caché ?
						if ( !obj.BoundingBox.IntersectsWith(clipRect) )  continue;

						obj.DrawGeometry(graphics, drawingContext);
					}
				}

				if ( this.Modifier != null )
				{
					graphics.RestoreClippingRectangle(initialClip);
				}
			}
			else
			{
				bool isBase = drawingContext.RootStackIsBase;
				Objects.Abstract branch = drawingContext.RootObject();
				Objects.Abstract activLayer = drawingContext.RootObject(2);

				foreach ( Objects.Layer layer in this.Flat(page) )
				{
					bool dimmedLayer = false;
					if ( layer != activLayer )  // calque passif ?
					{
						if ( layer.Type == Objects.LayerType.Hide ||
							drawingContext.LayerDrawingMode == LayerDrawingMode.HideInactive )
						{
							continue;
						}

						if ( layer.Type == Objects.LayerType.Dimmed &&
							drawingContext.LayerDrawingMode == LayerDrawingMode.DimmedInactive )
						{
							dimmedLayer = true;
						}
					}

					Properties.ModColor modColor = layer.PropertyModColor;
					drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);

					foreach ( DeepBranchEntry entry in this.DeepBranch(layer, branch) )
					{
						Objects.Abstract obj = entry.Object;
						if ( !obj.BoundingBox.IntersectsWith(clipRect) )  continue;

						if ( obj.IsHide )  // objet caché ?
						{
							if ( !drawingContext.HideHalfActive )  continue;
							drawingContext.IsDimmed = true;
						}
						else
						{
							drawingContext.IsDimmed = dimmedLayer;
						}

						if ( !entry.IsInsideBranch &&
							(layer.Type != Objects.LayerType.Show || !isBase) )
						{
							drawingContext.IsDimmed = true;
						}

						obj.DrawGeometry(graphics, drawingContext);
					}
				}
			}
		}

		// Imprime le document.
		public void Print(Common.Dialogs.Print dp)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			this.printerName = dp.Document.PrinterSettings.PrinterName;
			this.printer.Print(dp);
		}

		// Exporte le document.
		public string Export(string filename)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			return this.printer.Export(filename);
		}


		#region Flat
		public System.Collections.IEnumerable Flat(Objects.Abstract root)
		{
			return new FlatEnumerable(this, root, false);
		}

		public System.Collections.IEnumerable Flat(Objects.Abstract root, bool onlySelected)
		{
			return new FlatEnumerable(this, root, onlySelected);
		}

		// Enumérateur permettant de parcourir à plat l'arbre des objets.
		protected class FlatEnumerable : System.Collections.IEnumerable,
										 System.Collections.IEnumerator
		{
			public FlatEnumerable(Document document, Objects.Abstract root, bool onlySelected)
			{
				this.document = document;
				this.onlySelected = onlySelected;

				if ( root == null )
				{
					this.list = this.document.GetObjects;
				}
				else
				{
					this.list = root.Objects;
				}

				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.index = -1;
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets sélectionnés ?
				{
					while ( true )
					{
						this.index ++;
						if ( this.index >= this.list.Count )  return false;

						Objects.Abstract obj = this.list[this.index] as Objects.Abstract;
						if ( obj.IsSelected )  return true;
					}
				}
				else	// tous les objets ?
				{
					this.index ++;
					return ( this.index < this.list.Count );
				}
			}

			public object Current
			{
				get
				{
					if ( this.index >= 0 && this.index < this.list.Count )
					{
						return this.list[this.index] as Objects.Abstract;
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected bool						onlySelected;
			protected UndoableList				list;
			protected int						index;
		}
		#endregion

		#region FlatReverse
		public System.Collections.IEnumerable FlatReverse(Objects.Abstract root)
		{
			return new FlatReverseEnumerable(this, root, false);
		}

		public System.Collections.IEnumerable FlatReverse(Objects.Abstract root, bool onlySelected)
		{
			return new FlatReverseEnumerable(this, root, onlySelected);
		}

		// Enumérateur permettant de parcourir à plat depuis la fin l'arbre des objets.
		protected class FlatReverseEnumerable : System.Collections.IEnumerable,
												System.Collections.IEnumerator
		{
			public FlatReverseEnumerable(Document document, Objects.Abstract root, bool onlySelected)
			{
				this.document = document;
				this.onlySelected = onlySelected;

				if ( root == null )
				{
					this.list = this.document.GetObjects;
				}
				else
				{
					this.list = root.Objects;
				}

				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.index = this.list.Count;
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets sélectionnés ?
				{
					while ( true )
					{
						this.index --;
						if ( this.index < 0 )  return false;

						Objects.Abstract obj = this.list[this.index] as Objects.Abstract;
						if ( obj.IsSelected )  return true;
					}
				}
				else	// tous les objets ?
				{
					this.index --;
					return ( this.index >= 0 );
				}
			}

			public object Current
			{
				get
				{
					if ( this.index >= 0 && this.index < this.list.Count )
					{
						return this.list[this.index] as Objects.Abstract;
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected bool						onlySelected;
			protected UndoableList				list;
			protected int						index;
		}
		#endregion

		#region Deep
		public System.Collections.IEnumerable Deep(Objects.Abstract root)
		{
			return new DeepEnumerable(this, root, false);
		}

		public System.Collections.IEnumerable Deep(Objects.Abstract root, bool onlySelected)
		{
			return new DeepEnumerable(this, root, onlySelected);
		}

		// Enumérateur permettant de parcourir en profondeur l'arbre des objets.
		// En mode onlySelected, seuls les objets sélectionnés du premier niveau
		// sont concernés. Un objet fils d'un objet sélectionné du premier niveau
		// est toujours considéré comme sélectionné, bien qu'il ne le soit pas
		// physiquement !
		protected class DeepEnumerable : System.Collections.IEnumerable,
										 System.Collections.IEnumerator
		{
			public DeepEnumerable(Document document, Objects.Abstract root, bool onlySelected)
			{
				this.document = document;
				this.onlySelected = onlySelected;
				this.root = root;
				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.stack = new System.Collections.Stack();

				UndoableList list = this.document.GetObjects;
				if ( this.root != null )
				{
					list = this.root.Objects;
				}
				this.stack.Push(new TreeInfo(list, -1));

				this.first = true;
				this.index = 0;
			}

			protected bool InternalMoveNext()
			{
				TreeInfo ti;

				if ( this.first )
				{
					this.first = false;
					ti = this.stack.Peek() as TreeInfo;
					return ( this.index < ti.List.Count );
				}

				if ( this.index == -1 )  return false;

				ti = this.stack.Peek() as TreeInfo;
				Objects.Abstract obj = ti.List[this.index] as Objects.Abstract;
				if ( obj.Objects != null && obj.Objects.Count > 0 )  // objet avec fils ?
				{
					if ( this.onlySelected )  // seulement les objets sélectionnés ?
					{
						if ( this.stack.Count > 1 || obj.IsSelected )
						{
							this.stack.Push(new TreeInfo(obj.Objects, this.index));
							this.index = 0;
							return true;
						}
					}
					else	// tous les objets ?
					{
						this.stack.Push(new TreeInfo(obj.Objects, this.index));
						this.index = 0;
						return true;
					}
				}

				while ( true )
				{
					this.index ++;
					if ( this.index < ti.List.Count )  return true;

					ti = this.stack.Pop() as TreeInfo;
					this.index = ti.Parent;
					if ( this.index == -1 )  return false;

					ti = this.stack.Peek() as TreeInfo;
				}
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets sélectionnés ?
				{
					while ( true )
					{
						if ( !this.InternalMoveNext() )  return false;
						if ( this.stack.Count > 1 )  return true;
						Objects.Abstract obj = this.Current as Objects.Abstract;
						if ( obj.IsSelected )  return true;
					}
				}
				else	// tous les objets ?
				{
					return this.InternalMoveNext();
				}
			}

			public object Current
			{
				get
				{
					TreeInfo ti = this.stack.Peek() as TreeInfo;

					if ( this.index < ti.List.Count )
					{
						return ti.List[this.index];
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected bool						onlySelected;
			protected Objects.Abstract	root;
			protected bool						first;
			protected int						index;
			protected System.Collections.Stack	stack;
		}
		#endregion

		#region DeepBranch
		public System.Collections.IEnumerable DeepBranch(Objects.Abstract root, Objects.Abstract branch)
		{
			return new DeepBranchEnumerable(this, root, branch);
		}

		// Enumérateur permettant de parcourir en profondeur l'arbre des objets.
		// L'objet rendu est de type DeepBranchEntry. Ceci permet de savoir si
		// l'on est ou non à l'intérieur d'une branche quelconque.
		protected class DeepBranchEnumerable : System.Collections.IEnumerable,
											   System.Collections.IEnumerator
		{
			public DeepBranchEnumerable(Document document, Objects.Abstract root, Objects.Abstract branch)
			{
				this.document = document;
				this.root = root;
				this.branch = branch;
				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.stack = new System.Collections.Stack();

				UndoableList list = this.document.GetObjects;
				if ( this.root != null )
				{
					list = this.root.Objects;
				}
				this.stack.Push(new TreeInfo(list, -1));

				this.first = true;
				this.index = 0;
				this.isInsideBranch = (this.root == this.branch);
			}

			public bool MoveNext()
			{
				TreeInfo ti;
				Objects.Abstract obj;

				if ( this.first )
				{
					this.first = false;
					ti = this.stack.Peek() as TreeInfo;
					return ( this.index < ti.List.Count );
				}

				if ( this.index == -1 )  return false;

				ti = this.stack.Peek() as TreeInfo;
				obj = ti.List[this.index] as Objects.Abstract;
				if ( obj.Objects != null && obj.Objects.Count > 0 )  // objet avec fils ?
				{
					if ( !this.isInsideBranch )
					{
						if ( obj == this.branch )  this.isInsideBranch = true;
					}
					this.stack.Push(new TreeInfo(obj.Objects, this.index));
					this.index = 0;
					return true;
				}

				while ( true )
				{
					this.index ++;
					if ( this.index < ti.List.Count )  return true;

					ti = this.stack.Pop() as TreeInfo;
					this.index = ti.Parent;
					if ( this.index == -1 )  return false;

					ti = this.stack.Peek() as TreeInfo;

					if ( this.isInsideBranch )
					{
						obj = ti.List[this.index] as Objects.Abstract;
						if ( obj == this.branch )  this.isInsideBranch = false;
					}
				}
			}

			public object Current
			{
				get
				{
					TreeInfo ti = this.stack.Peek() as TreeInfo;

					if ( this.index < ti.List.Count )
					{
						Objects.Abstract obj = ti.List[this.index] as Objects.Abstract;
						return new DeepBranchEntry(obj, this.isInsideBranch);
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected Objects.Abstract			root;
			protected Objects.Abstract			branch;
			protected bool						isInsideBranch;
			protected bool						first;
			protected int						index;
			protected System.Collections.Stack	stack;
		}
		#endregion

		#region DeepBranchEntry
		public class DeepBranchEntry
		{
			public DeepBranchEntry(Objects.Abstract obj, bool isInsideBranch)
			{
				this.obj = obj;
				this.isInsideBranch = isInsideBranch;
			}

			public Objects.Abstract Object
			{
				get { return this.obj; }
			}

			public bool IsInsideBranch
			{
				get { return this.isInsideBranch; }
			}

			protected Objects.Abstract	obj;
			protected bool						isInsideBranch;
		}
		#endregion

		#region TreeInfo
		protected class TreeInfo
		{
			public TreeInfo(UndoableList list, int parent)
			{
				this.list = list;
				this.parent = parent;
			}

			public UndoableList List
			{
				get { return this.list; }
			}

			public int Parent
			{
				get { return this.parent; }
			}

			protected UndoableList		list;
			protected int				parent;
		}
		#endregion


		protected DocumentType					type;
		protected DocumentMode					mode;
		protected Settings.GlobalSettings		globalSettings;
		protected CommandDispatcher				commandDispatcher;
		protected string						name;
		protected Document						clipboard;
		protected Size							size;
		protected Point							hotSpot;
		protected string						filename;
		protected string						exportDirectory;
		protected string						exportFilename;
		protected int							exportFilter;
		protected string						printerName;
		protected bool							isDirtySerialize;
		protected UndoableList					objects;
		protected UndoableList					propertiesAuto;
		protected UndoableList					propertiesSel;
		protected UndoableList					propertiesStyle;
		protected Settings.Settings				settings;
		protected Modifier						modifier;
		protected Notifier						notifier;
		protected Printer						printer;
		protected Common.Dialogs.Print			printDialog;
		protected Dialogs						dialogs;
		protected string						ioDirectory;
		protected int							readRevision;
		protected int							readVersion;
		protected System.Collections.ArrayList	readWarnings;
		protected IOType						ioType;
	}
}
