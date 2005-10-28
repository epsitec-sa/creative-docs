using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
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

	public enum InstallType
	{
		Demo,			// version demo
		Full,			// version pleine valide
		Expired,		// version pleine échue
		Freeware,		// version freeware
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
		public Document(DocumentType type, DocumentMode mode, InstallType installType, Settings.GlobalSettings globalSettings, CommandDispatcher commandDispatcher)
		{
			this.type = type;
			this.mode = mode;
			this.installType = installType;
			this.globalSettings = globalSettings;
			this.commandDispatcher = commandDispatcher;

			if ( this.type == DocumentType.Pictogram )
			{
				this.size = new Size(20, 20);
			}
			else
			{
				if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
				{
					this.size = new Size(2100, 2970);  // A4 vertical
				}
				else
				{
					this.size = new Size(2159, 2794);  // Letter US (8.5x11in)
				}
			}

			this.hotSpot = new Point(0, 0);
			this.objects = new UndoableList(this, UndoableListType.ObjectsInsideDocument);
			this.propertiesAuto = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.propertiesSel = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.aggregates = new UndoableList(this, UndoableListType.AggregatesInsideDocument);
			this.textFlows = new UndoableList(this, UndoableListType.TextFlows);
			this.exportDirectory = "";
			this.exportFilename = "";
			this.exportFilter = 0;
			this.isSurfaceRotation = false;
			this.surfaceRotationAngle = 0.0;
			this.uniqueObjectId = 0;
			this.uniqueAggregateId = 0;

			this.printDialog = new Common.Dialogs.PrinterDocumentProperties();

			if ( this.mode == DocumentMode.Modify    ||
				 this.mode == DocumentMode.Clipboard )
			{
				this.modifier  = new Modifier(this);
				this.notifier  = new Notifier(this);
				this.dialogs   = new Dialogs(this);
				this.settings  = new Settings.Settings(this);
				this.printer   = new Printer(this);
				this.exportPDF = new PDF.Export(this);
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

		// Type d'installation du logiciel.
		public InstallType InstallType
		{
			get { return this.installType; }
			set { this.installType = value; }
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

		// TextContext de ce document.
		public Text.TextContext TextContext
		{
			get
			{
				if ( this.textContext == null )
				{
					this.DefaultTextContext();
				}
				return this.textContext;
			}
		}

		// Règle horizontale.
		public Widgets.HRuler HRuler
		{
			get { return this.hRuler; }
			set { this.hRuler = value; }
		}

		// Règle verticale.
		public Widgets.VRuler VRuler
		{
			get { return this.vRuler; }
			set { this.vRuler = value; }
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

		// Liste des aggrégats de ce document.
		public UndoableList Aggregates
		{
			get { return this.aggregates; }
		}

		// Liste des flux de textes de ce document.
		public UndoableList TextFlows
		{
			get { return this.textFlows; }
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
			get { return this.printDialog; }
		}


		// Rotation spéciale pour calculer SurfaceAnchor en cours.
		public bool IsSurfaceRotation
		{
			get
			{
				return this.isSurfaceRotation;
			}
			
			set
			{
				this.isSurfaceRotation = value;
			}
		}

		// Angle de la rotation spéciale pour calculer SurfaceAnchor.
		public double SurfaceRotationAngle
		{
			get
			{
				return this.surfaceRotationAngle;
			}
			
			set
			{
				this.surfaceRotationAngle = value;
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
					if ( this.modifier != null && this.notifier != null )
					{
						this.modifier.OpletQueueBeginAction(Res.Strings.Action.DocumentSize, "ChangeDocSize");
						this.modifier.InsertOpletSize();
						this.size = value;
						this.IsDirtySerialize = true;
						this.modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
						this.notifier.NotifyAllChanged();
						this.modifier.OpletQueueValidateAction();
					}
					else
					{
						this.size = value;
						this.IsDirtySerialize = true;
					}
				}
			}
		}

		public Size InternalSize
		{
			set
			{
				this.size = value;
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
						if ( Misc.IsExtension(filename, ".crdoc") ||
							 Misc.IsExtension(filename, ".icon")  )
						{
							this.Filename = filename;
							this.globalSettings.LastFilenameAdd(filename);
							this.IsDirtySerialize = false;
						}
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
				return Res.Strings.Error.BadFile;
			}

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
			this.aggregates = doc.aggregates;
			//?this.textFlows = doc.textFlows;
			this.uniqueObjectId = doc.uniqueObjectId;
			this.uniqueAggregateId = doc.uniqueAggregateId;
			
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

				if ( this.Modifier != null && doc.readObjectMemory != null )
				{
					doc.readObjectMemory.PropertiesXferMemory(this.Modifier.ObjectMemory);
					doc.readObjectMemoryText.PropertiesXferMemory(this.Modifier.ObjectMemoryText);
					this.Modifier.ObjectMemory = doc.readObjectMemory;
					this.Modifier.ObjectMemoryText = doc.readObjectMemoryText;
				}

				if ( this.Modifier != null && doc.readRootStack != null )
				{
					this.Modifier.ActiveViewer.DrawingContext.RootStack = doc.readRootStack;
				}
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

		sealed class VersionDeserializationBinder : Common.IO.GenericDeserializationBinder
		{
			public VersionDeserializationBinder()
			{
			}
			
			// Retourne un type correspondant à l'application courante, afin
			// d'accepter de désérialiser un fichier généré par une application
			// ayant un autre numéro de révision.
			// Application courante: Version=1.0.1777.18519
			// Version dans le fichier: Version=1.0.1777.11504
			public override System.Type BindToType(string assemblyName, string typeName) 
			{
				if ( typeName == "Epsitec.Common.Document.Document" )
				{
					int i, j;
					string v;

					i = assemblyName.IndexOf("Version=");
					if ( i >= 0 )
					{
						i += 8;
						j = assemblyName.IndexOf(".", i);
						v = assemblyName.Substring(i, j-i);
						long r1 = System.Int64.Parse(v, System.Globalization.CultureInfo.InvariantCulture);

						i = j+1;
						j = assemblyName.IndexOf(".", i);
						v = assemblyName.Substring(i, j-i);
						long r2 = System.Int64.Parse(v, System.Globalization.CultureInfo.InvariantCulture);

						i = j+1;
						j = assemblyName.IndexOf(".", i);
						v = assemblyName.Substring(i, j-i);
						long r3 = System.Int64.Parse(v, System.Globalization.CultureInfo.InvariantCulture);

						Document.ReadRevision = (r1<<32) + (r2<<16) + r3;
					}
				}

				return base.BindToType(assemblyName, typeName);
			}
		}

		// Utilisé par les constructeurs de désérialisation du genre:
		// protected Toto(SerializationInfo info, StreamingContext context)
		public static Document ReadDocument = null;
		public static long ReadRevision = 0;

		// Adapte tous les objets après une désérialisation.
		protected void ReadFinalize()
		{
			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = false;
			}

			if ( !this.IsRevisionGreaterOrEqual(1,0,24) )
			{
				this.OldStylesToAggregates();
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
				this.Modifier.UpdatePageAfterChanging();
				this.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
				this.Modifier.OpletQueueEnable = true;
				this.Modifier.OpletQueuePurge();
			}
		}

		#region OldStylesToAggregates
		// Adapte les anciens styles en agrégats.
		protected void OldStylesToAggregates()
		{
			foreach ( Objects.Abstract obj in this.Deep(null) )
			{
				Properties.Type[] list = obj.PropertiesStyle();
				if ( list.Length == 0 )  continue;

				Properties.Aggregate agg = this.OldStylesSearchAggregate(obj, list);
				if ( agg == null )
				{
					agg = this.OldStylesCreateAggregate(obj, list);
				}

				this.OldStylesUseAggregate(obj, list, agg);
			}

			this.OldStylesSort();

			if ( this.modifier != null )
			{
				this.modifier.ObjectMemory.AggregateFree();
				this.modifier.ObjectMemoryText.AggregateFree();
			}
		}

		protected Properties.Aggregate OldStylesSearchAggregate(Objects.Abstract obj, Properties.Type[] list)
		{
			foreach ( Properties.Aggregate agg in this.aggregates )
			{
				if ( agg.Styles.Count != list.Length )  continue;

				int eq = 0;
				for ( int i=0 ; i<list.Length ; i++ )
				{
					Properties.Abstract p1 = agg.Styles[i] as Properties.Abstract;
					Properties.Abstract p2 = obj.Property(list[i]);

					if ( !p1.Compare(p2) )  break;
					eq ++;
				}

				if ( eq == list.Length )  return agg;
			}
			return null;
		}

		protected Properties.Aggregate OldStylesCreateAggregate(Objects.Abstract obj, Properties.Type[] list)
		{
			Properties.Aggregate agg = new Properties.Aggregate(Document.ReadDocument);

			string name = "";
			string lastName = "";

			foreach ( Properties.Type type in list )
			{
				Properties.Abstract property = Properties.Abstract.NewProperty(Document.ReadDocument, type);
				Properties.Abstract p = obj.Property(type);
				p.CopyTo(property);
				agg.Styles.Add(property);

				if ( p.OldStyleName != lastName )
				{
					if ( name.Length > 0 )  name += ", ";
					name += p.OldStyleName;
					lastName = p.OldStyleName;
				}
			}

			agg.AggregateName = name;

			this.aggregates.Add(agg);
			return agg;
		}

		protected void OldStylesUseAggregate(Objects.Abstract obj, Properties.Type[] list, Properties.Aggregate agg)
		{
			foreach ( Properties.Type type in list )
			{
				Properties.Abstract style = agg.Property(type);
				obj.ChangeProperty(style);
			}

			obj.Aggregates.Add(agg);
		}

		protected void OldStylesSort()
		{
			bool stop;
			do
			{
				stop = true;
				for ( int i=0 ; i<this.aggregates.Count-1 ; i++ )
				{
					Properties.Aggregate agg1 = this.aggregates[i+0] as Properties.Aggregate;
					Properties.Aggregate agg2 = this.aggregates[i+1] as Properties.Aggregate;
					int n1 = this.OldStylesGetSortValue(agg1);
					int n2 = this.OldStylesGetSortValue(agg2);
					if ( n1 > n2 )
					{
						this.aggregates[i+0] = agg2;
						this.aggregates[i+1] = agg1;
						stop = false;
					}
				}
			}
			while ( !stop );
		}

		protected int OldStylesGetSortValue(Properties.Aggregate agg)
		{
			int n = 0;
			foreach ( Properties.Abstract property in agg.Styles )
			{
				int v = (int) property.Type;
				n |= (1<<v);
			}
			return n;
		}
		#endregion

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
						//?Stream compressor = IO.Compression.CreateBZip2Stream(stream);
						Stream compressor = IO.Compression.CreateDeflateStream(stream, 1);
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

			if ( Misc.IsExtension(filename, ".crdoc") ||
				 Misc.IsExtension(filename, ".icon")  )
			{
				this.Filename = filename;
				this.IsDirtySerialize = false;
			}
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
			int version  = 2;
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

				info.AddValue("ObjectMemory", this.modifier.ObjectMemory);
				info.AddValue("ObjectMemoryText", this.modifier.ObjectMemoryText);

				info.AddValue("RootStack", this.modifier.ActiveViewer.DrawingContext.RootStack);
			}

			info.AddValue("UniqueObjectId", this.uniqueObjectId);
			info.AddValue("UniqueAggregateId", this.uniqueAggregateId);
			info.AddValue("Objects", this.objects);
			info.AddValue("Properties", this.propertiesAuto);
			info.AddValue("Aggregates", this.aggregates);
		}

		// Constructeur qui désérialise le document.
		protected Document(SerializationInfo info, StreamingContext context)
		{
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

				if ( this.IsRevisionGreaterOrEqual(1,0,5) )
				{
					this.exportDirectory = "";
					this.exportFilename = info.GetString("ExportFilename");
					this.exportFilter = info.GetInt32("ExportFilter");
				}
				else
				{
					this.exportDirectory = "";
					this.exportFilename = "";
					this.exportFilter = 0;
				}

				if ( this.IsRevisionGreaterOrEqual(1,0,7) )
				{
					this.readObjectMemory = (Objects.Memory) info.GetValue("ObjectMemory", typeof(Objects.Memory));
					this.readObjectMemoryText = (Objects.Memory) info.GetValue("ObjectMemoryText", typeof(Objects.Memory));
				}
				else
				{
					this.readObjectMemory = null;
					this.readObjectMemoryText = null;
				}

				if ( this.IsRevisionGreaterOrEqual(1,0,8) )
				{
					this.readRootStack = (System.Collections.ArrayList) info.GetValue("RootStack", typeof(System.Collections.ArrayList));
				}
				else
				{
					this.readRootStack = null;
				}
			}

			this.uniqueObjectId = info.GetInt32("UniqueObjectId");
			this.objects = (UndoableList) info.GetValue("Objects", typeof(UndoableList));
			this.propertiesAuto = (UndoableList) info.GetValue("Properties", typeof(UndoableList));

			if ( this.IsRevisionGreaterOrEqual(1,0,23) )
			{
				this.aggregates = (UndoableList) info.GetValue("Aggregates", typeof(UndoableList));
				this.uniqueAggregateId = info.GetInt32("UniqueAggregateId");
			}
			else
			{
				this.aggregates = new UndoableList(Document.ReadDocument, UndoableListType.AggregatesInsideDocument);
				this.uniqueAggregateId = 0;
			}
		}

		// Retourne le nom du dossier en cours de lecture/écriture.
		public string IoDirectory
		{
			get { return this.ioDirectory; }
		}

		// Indique si un fichier est compatible avec une révision/version.
		public bool IsRevisionGreaterOrEqual(int revision, int version, int subversion)
		{
			long r = ((long)revision<<32) + ((long)version<<16) + (long)subversion;
			return ( Document.ReadRevision >= r );
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

			if ( drawingContext.MasterPageList.Count > 0 )
			{
				foreach ( Objects.Page masterPage in drawingContext.MasterPageList )
				{
					int frontier = masterPage.MasterFirstFrontLayer;
					this.PaintPage(graphics, drawingContext, clipRect, masterPage, 0, frontier-1);
				}
			}

			Objects.Abstract page = drawingContext.RootObject(1);
			this.PaintPage(graphics, drawingContext, clipRect, page, 0, 10000);

			if ( drawingContext.MasterPageList.Count > 0 )
			{
				foreach ( Objects.Page masterPage in drawingContext.MasterPageList )
				{
					int frontier = masterPage.MasterFirstFrontLayer;
					this.PaintPage(graphics, drawingContext, clipRect, masterPage, frontier, 10000);
				}
			}
		}

		protected void PaintPage(Graphics graphics, DrawingContext drawingContext, Rectangle clipRect,
								 Objects.Abstract page, int firstLayer, int lastLayer)
		{
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

				int rankLayer = -1;
				foreach ( Objects.Layer layer in this.Flat(page) )
				{
					rankLayer ++;
					if ( rankLayer < firstLayer || rankLayer > lastLayer )  continue;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;

					Properties.ModColor modColor = layer.PropertyModColor;
					graphics.PushColorModifier(new ColorModifier(drawingContext.DimmedColor));
					graphics.PushColorModifier(new ColorModifier(modColor.ModifyColor));
					drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

					foreach ( Objects.Abstract obj in this.Deep(layer) )
					{
						if ( obj.IsHide )  continue;  // objet caché ?
						if ( !obj.BoundingBox.IntersectsWith(clipRect) )  continue;

						obj.DrawGeometry(graphics, drawingContext);
					}

					graphics.PopColorModifier();
					graphics.PopColorModifier();
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

				int rankLayer = -1;
				foreach ( Objects.Layer layer in this.Flat(page) )
				{
					rankLayer ++;
					if ( rankLayer < firstLayer || rankLayer > lastLayer )  continue;
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
					graphics.PushColorModifier(new ColorModifier(drawingContext.DimmedColor));
					graphics.PushColorModifier(new ColorModifier(modColor.ModifyColor));

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

					graphics.PopColorModifier();
					graphics.PopColorModifier();
				}
			}
		}

		// Imprime le document.
		public void Print(Common.Dialogs.Print dp)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			this.printer.Print(dp);
		}

		// Exporte le document.
		public string Export(string filename)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			return this.printer.Export(filename);
		}

		// Exporte le document.
		public string ExportPDF(string filename)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			return this.exportPDF.FileExport(filename);
		}


		#region TextContext
		// Crée le TextContext et les styles par défaut.
		protected void DefaultTextContext()
		{
			System.Collections.ArrayList properties;
			Text.TextStyle style;

			this.textContext = new Text.TextContext();
			this.textContext.IsDegradedLayoutEnabled = true;

			Text.TabList tabs = this.textContext.TabList;

			//?Text.Properties.TabProperty t1 = tabs.NewTab("T1", 100, Text.Properties.SizeUnits.Points, 0.0, null);
			//?Text.Properties.TabProperty t2 = tabs.NewTab("T2", 400, Text.Properties.SizeUnits.Points, 0.0, null);

			properties = new System.Collections.ArrayList();
			//?properties.Add(new Text.Properties.FontProperty("Palatino Linotype", "Italic", "liga", "dlig", "kern"));
			properties.Add(new Text.Properties.FontProperty("Arial", Misc.DefaultFontStyle("Arial")));
			properties.Add(new Text.Properties.FontSizeProperty(12.0*Modifier.fontSizeScale, Text.Properties.SizeUnits.Points));
			properties.Add(new Text.Properties.MarginsProperty(10, 10, 10, 10, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
			properties.Add(new Text.Properties.ColorProperty("Black"));
			properties.Add(new Text.Properties.LanguageProperty("fr-ch", 1.0));
			properties.Add(new Text.Properties.LeadingProperty(0.0, Text.Properties.SizeUnits.Points, 5.0*Modifier.fontSizeScale, Text.Properties.SizeUnits.Points, 5.0, Text.Properties.SizeUnits.Points, Text.Properties.AlignMode.None));
			//?aproperties.Add(new Text.Properties.TabsProperty(t1, t2));
			style = this.textContext.StyleList.NewTextStyle("Default", Text.TextStyleClass.Paragraph, properties);
			this.textContext.DefaultStyle = style;

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Bold"));
			this.textContext.StyleList.NewMetaProperty("Bold", "X-Bold", 1, properties);

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Italic"));
			this.textContext.StyleList.NewMetaProperty("Italic", "X-Italic", 1, properties);
			
//			this.textContext.StyleList.NewMetaProperty("Bold", "Bold", new Text.Properties.FontBoldProperty ());
//			this.textContext.StyleList.NewMetaProperty("Italic", "Italic", new Text.Properties.FontItalicProperty ());

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.UnderlineProperty(-5, Text.Properties.SizeUnits.Points, 1.0, Text.Properties.SizeUnits.Points, "underline", "Black"));
			this.textContext.StyleList.NewMetaProperty("Underlined", "Underlined", 0, properties);
			
			this.textContext.StyleList.NewMetaProperty("Subscript", "SuperScript", new Text.Properties.FontXScriptProperty (0.6, -0.15, null));
			this.textContext.StyleList.NewMetaProperty("Superscript", "SuperScript", new Text.Properties.FontXScriptProperty (0.6, 0.25, null));

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Bold"));
			this.textContext.StyleList.NewMetaProperty("UserX", "UserX", 0, properties);

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Italic"));
			this.textContext.StyleList.NewMetaProperty("UserY", "UserY", 0, properties);

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.UnderlineProperty(-5, Text.Properties.SizeUnits.Points, 1.0, Text.Properties.SizeUnits.Points, "underline", "Black"));
			this.textContext.StyleList.NewMetaProperty("UserZ", "UserZ", 0, properties);

			
			Text.Generator generator1 = this.textContext.GeneratorList.NewGenerator("bullet-1");
			Text.Generator generator2 = this.textContext.GeneratorList.NewGenerator("num-1");
			Text.Generator generator3 = this.textContext.GeneratorList.NewGenerator("alpha-1");
			
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant,   "", "", Text.Generator.Casing.Default, "\u25CF"));
			generator2.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Numeric,    "", "."));
			generator3.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Alphabetic, "", ")", Text.Generator.Casing.Upper));
			
			Text.ParagraphManagers.ItemListManager.Parameters items1 = new Text.ParagraphManagers.ItemListManager.Parameters();
			Text.ParagraphManagers.ItemListManager.Parameters items2 = new Text.ParagraphManagers.ItemListManager.Parameters();
			Text.ParagraphManagers.ItemListManager.Parameters items3 = new Text.ParagraphManagers.ItemListManager.Parameters();

			items1.Generator = generator1;
			items1.TabItem   = tabs.NewTab("T1-item", 10.0, Text.Properties.SizeUnits.Points, 0, null);
			items1.TabBody   = tabs.NewTab("T1-body", 60.0, Text.Properties.SizeUnits.Points, 0, null);
			
			items2.Generator = generator2;
			items2.TabItem   = tabs.NewTab("T2-item", 10.0, Text.Properties.SizeUnits.Points, 0, null);
			items2.TabBody   = tabs.NewTab("T2-body", 60.0, Text.Properties.SizeUnits.Points, 0, null);
			
			items3.Generator = generator3;
			items3.TabItem   = tabs.NewTab("T3-item", 10.0, Text.Properties.SizeUnits.Points, 0, null);
			items3.TabBody   = tabs.NewTab("T3-body", 60.0, Text.Properties.SizeUnits.Points, 0, null);
			
			Text.Properties.ManagedParagraphProperty itemList1 = new Text.Properties.ManagedParagraphProperty("ItemList", items1.Save());
			Text.Properties.ManagedParagraphProperty itemList2 = new Text.Properties.ManagedParagraphProperty("ItemList", items2.Save());
			Text.Properties.ManagedParagraphProperty itemList3 = new Text.Properties.ManagedParagraphProperty("ItemList", items3.Save());
			
			this.textContext.StyleList.NewTextStyle("BulletRound",   Text.TextStyleClass.Paragraph, itemList1);
			this.textContext.StyleList.NewTextStyle("BulletNumeric", Text.TextStyleClass.Paragraph, itemList2);
			this.textContext.StyleList.NewTextStyle("BulletAlpha",   Text.TextStyleClass.Paragraph, itemList3);


			this.textContext.StyleList.NewTextStyle("AlignLeft",   Text.TextStyleClass.Paragraph, new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 0.0, 0.0, 0.0, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			this.textContext.StyleList.NewTextStyle("AlignCenter", Text.TextStyleClass.Paragraph, new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 0.0, 0.0, 0.5, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			this.textContext.StyleList.NewTextStyle("AlignRight",  Text.TextStyleClass.Paragraph, new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 0.0, 0.0, 1.0, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			this.textContext.StyleList.NewTextStyle("AlignJustif", Text.TextStyleClass.Paragraph, new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 1.0, 0.0, 0.0, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
		}

		// Cherche un tag unique pour le prochain tabulateur interactif à créer.
		public string SearchTabNextTag()
		{
			int max = -1;
			Text.TabList list = this.textContext.TabList;
			string[] tags = list.GetTabTags();
			foreach ( string tag in tags )
			{
				int id = this.GetTabId(tag);
				if ( max < id )  max = id;
			}
			return this.GetTabTag(max+1);
		}

		// Retourne l'identificateur d'un tabulateur interactif d'après son tag.
		protected int GetTabId(string tag)
		{
			if ( !tag.StartsWith("Ti") )  return -1;

			try
			{
				return int.Parse(tag.Substring(2), System.Globalization.CultureInfo.InvariantCulture);
			}
			catch
			{
				return -1;
			}
		}

		// Retourne le tag d'un tabulateur interactif.
		public string GetTabTag(int id)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Ti{0}", id);
		}

		// Liste des styles de texte de ce document.
		public UndoableList TextStyles
		{
			get
			{
				this.modifier.OpletQueueEnable = false;
				UndoableList list = new UndoableList(this, UndoableListType.TextStylesInsideDocument);

				// TODO: comment obtenir la liste de tous les styles ?
				Text.TextStyle defaultStyle = this.TextContext.StyleList["Default", Text.TextStyleClass.Paragraph];
				list.Add(defaultStyle);

				this.modifier.OpletQueueEnable = true;
				return list;
			}
		}
		#endregion

		#region UniqueId
		// Retourne le prochain identificateur unique pour les objets.
		public int GetNextUniqueObjectId()
		{
			return ++this.uniqueObjectId;
		}

		// Retourne le prochain identificateur unique pour les noms d'agrégats.
		public int GetNextUniqueAggregateId()
		{
			return ++this.uniqueAggregateId;
		}
		#endregion

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
			protected Objects.Abstract			root;
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

		#region Ressources
		// Retourne une ressource string d'après son nom.
		public static string GetRes(string name)
		{
			return Res.Strings.GetString(name);
		}
		#endregion



		protected DocumentType					type;
		protected DocumentMode					mode;
		protected InstallType					installType;
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
		protected bool							isDirtySerialize;
		protected UndoableList					objects;
		protected UndoableList					propertiesAuto;
		protected UndoableList					propertiesSel;
		protected UndoableList					aggregates;
		protected UndoableList					textFlows;
		protected Settings.Settings				settings;
		protected Modifier						modifier;
		protected Notifier						notifier;
		protected Printer						printer;
		protected Common.Dialogs.Print			printDialog;
		protected PDF.Export					exportPDF;
		protected Dialogs						dialogs;
		protected string						ioDirectory;
		protected System.Collections.ArrayList	readWarnings;
		protected IOType						ioType;
		protected Objects.Memory				readObjectMemory;
		protected Objects.Memory				readObjectMemoryText;
		protected System.Collections.ArrayList	readRootStack;
		protected bool							isSurfaceRotation;
		protected double						surfaceRotationAngle;
		protected int							uniqueObjectId = 0;
		protected int							uniqueAggregateId = 0;
		protected Text.TextContext				textContext;
		protected Widgets.HRuler				hRuler;
		protected Widgets.VRuler				vRuler;
	}
}
