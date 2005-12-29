using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
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

	public enum DebugMode
	{
		Release,		// pas de debug
		DebugCommands,	// toutes les commandes de debug
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

		public Document(DocumentType type, DocumentMode mode, InstallType installType, DebugMode debugMode, Settings.GlobalSettings globalSettings, CommandDispatcher commandDispatcher)
		{
			//	Crée un nouveau document vide.
			this.type = type;
			this.mode = mode;
			this.installType = installType;
			this.debugMode = debugMode;
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
			this.containOldText = false;

			this.printDialog = new Common.Dialogs.PrinterDocumentProperties();

			if ( this.mode == DocumentMode.Modify    ||
				 this.mode == DocumentMode.Clipboard )
			{
				this.modifier  = new Modifier(this);
				this.wrappers  = new Wrappers(this);
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

		public DocumentType Type
		{
			//	Type de ce document.
			get { return this.type; }
		}

		public DocumentMode Mode
		{
			//	Mode de travail pour ce document.
			get { return this.mode; }
		}

		public InstallType InstallType
		{
			//	Type d'installation du logiciel.
			get { return this.installType; }
			set { this.installType = value; }
		}

		public DebugMode DebugMode
		{
			//	Type de mise au point du logiciel.
			get { return this.debugMode; }
			set { this.debugMode = value; }
		}

		public Settings.GlobalSettings GlobalSettings
		{
			//	Réglages globaux.
			get { return this.globalSettings; }
		}
		
		public CommandDispatcher CommandDispatcher
		{
			//	CommandDispatcher de l'éditeur.
			get { return this.commandDispatcher; }
		}
		
		public string Name
		{
			//	Nom du document.
			get { return this.name; }
			set { this.name = value; }
		}

		public Document Clipboard
		{
			//	Bloc-notes associé.
			get { return this.clipboard; }
			set { this.clipboard = value; }
		}

		public UndoableList GetObjects
		{
			//	Liste des objets de ce document.
			get { return this.objects; }
		}

		public Text.TextContext TextContext
		{
			//	TextContext de ce document.
			get
			{
				if ( this.textContext == null )
				{
					this.DefaultTextContext();
				}
				return this.textContext;
			}
		}

		public Text.Wrappers.TextWrapper TextWrapper
		{
			//	Wrapper pour la fonte.
			get { return this.wrappers.TextWrapper; }
		}

		public Text.Wrappers.ParagraphWrapper ParagraphWrapper
		{
			//	Wrapper pour le paragraphe.
			get { return this.wrappers.ParagraphWrapper; }
		}

		public bool IsWrappersAttached
		{
			//	Indique si les wrappers sont attachés.
			get
			{
				return this.wrappers.IsWrappersAttached;
			}
		}

		public Widgets.HRuler HRuler
		{
			//	Règle horizontale.
			get { return this.hRuler; }
			set { this.hRuler = value; }
		}

		public Widgets.VRuler VRuler
		{
			//	Règle verticale.
			get { return this.vRuler; }
			set { this.vRuler = value; }
		}


		public UndoableList PropertiesAuto
		{
			//	Liste des propriétés automatiques de ce document.
			get { return this.propertiesAuto; }
		}

		public UndoableList PropertiesSel
		{
			//	Liste des propriétés sélectionnées de ce document.
			get { return this.propertiesSel; }
		}

		public UndoableList Aggregates
		{
			//	Liste des aggrégats de ce document.
			get { return this.aggregates; }
		}

		public UndoableList TextFlows
		{
			//	Liste des flux de textes de ce document.
			get { return this.textFlows; }
		}


		public Settings.Settings Settings
		{
			//	Réglages de ce document.
			get { return this.settings; }
		}

		public Modifier Modifier
		{
			//	Modificateur éventuel pour ce document.
			get { return this.modifier; }
		}

		public Wrappers Wrappers
		{
			//	Wrappers éventuel pour ce document.
			get { return this.wrappers; }
		}

		public Notifier Notifier
		{
			//	Notificateur éventuel pour ce document.
			get { return this.notifier; }
		}

		public Dialogs Dialogs
		{
			//	Dialogues éventuels pour ce document.
			get { return this.dialogs; }
		}

		public Printer Printer
		{
			//	Imprimeur pour ce document.
			get { return this.printer; }
			set { this.printer = value; }
		}
		
		public Common.Dialogs.Print PrintDialog
		{
			//	Dialogue d'impression pour ce document.
			get { return this.printDialog; }
		}


		public bool IsSurfaceRotation
		{
			//	Rotation spéciale pour calculer SurfaceAnchor en cours.
			get
			{
				return this.isSurfaceRotation;
			}
			
			set
			{
				this.isSurfaceRotation = value;
			}
		}

		public double SurfaceRotationAngle
		{
			//	Angle de la rotation spéciale pour calculer SurfaceAnchor.
			get
			{
				return this.surfaceRotationAngle;
			}
			
			set
			{
				this.surfaceRotationAngle = value;
			}
		}


		public Size DocumentSize
		{
			//	Taille globale de toutes les pages du document.
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

		public Size PageSize
		{
			//	Taille de la page courante du document.
			get
			{
				int pageNumber = -1;

				if ( this.modifier != null && this.modifier.ActiveViewer != null )
				{
					pageNumber = this.modifier.ActiveViewer.DrawingContext.CurrentPage;
				}

				return this.GetPageSize(pageNumber);
			}
		}

		public Size GetPageSize(int pageNumber)
		{
			//	Taille d'une page du document.
			Objects.Page page = null;

			if ( pageNumber != -1 )
			{
				page = this.GetObjects[pageNumber] as Objects.Page;
			}

			return this.GetPageSize(page);
		}

		public Size GetPageSize(Objects.Page page)
		{
			//	Taille d'une page du document.
			Size size = this.size;

			if ( page != null )
			{
				if ( page.PageSize.Width  != 0 )  size.Width  = page.PageSize.Width;
				if ( page.PageSize.Height != 0 )  size.Height = page.PageSize.Height;
			}

			return size;
		}

		public Size InternalSize
		{
			set
			{
				this.size = value;
			}
		}

		public Point HotSpot
		{
			//	Point chaud du document.
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

		public string Filename
		{
			//	Nom du fichier associé.
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

		public string ExportDirectory
		{
			//	Nom du dossier d'exportation associé.
			get
			{
				return this.exportDirectory;
			}

			set
			{
				this.exportDirectory = value;
			}
		}

		public string ExportFilename
		{
			//	Nom du fichier (sans dossier) d'exportation associé.
			get
			{
				return this.exportFilename;
			}

			set
			{
				this.exportFilename = value;
			}
		}

		public int ExportFilter
		{
			//	Type du fichier d'exportation associé.
			get
			{
				return this.exportFilter;
			}

			set
			{
				this.exportFilter = value;
			}
		}

		public bool IsDirtySerialize
		{
			//	Indique si la sérialisation est nécessaire.
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
				
				if ( value )
				{
					foreach ( TextFlow flow in this.textFlows )
					{
						flow.NotifyAboutToExecuteCommand();
					}
				}
			}
		}

		public string Read(string filename)
		{
			//	Ouvre un document existant sur disque.
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

		public string Read(Stream stream, string directory)
		{
			//	Ouvre un document sérialisé, soit parce que l'utilisateur veut ouvrir
			//	explicitement un fichier, soit par Engine.
			this.ioDirectory = directory;
			this.readWarnings = new System.Collections.ArrayList();

			IOType type = Document.ReadIdentifier(stream);
			if ( type == IOType.Unknow )
			{
				return Res.Strings.Error.BadFile;
			}

			//	Initialise la variable statique permettant à tous les constructeurs
			//	de connaître le pointeur au document.
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
			this.textFlows = doc.textFlows;
			this.textContext = doc.textContext; 
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

		public System.Collections.ArrayList ReadWarnings
		{
			//	Retourne la liste éventuelle des warnings de lecture.
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
			
			public override System.Type BindToType(string assemblyName, string typeName) 
			{
				//	Retourne un type correspondant à l'application courante, afin
				//	d'accepter de désérialiser un fichier généré par une application
				//	ayant un autre numéro de révision.
				//	Application courante: Version=1.0.1777.18519
				//	Version dans le fichier: Version=1.0.1777.11504
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

		//	Utilisé par les constructeurs de désérialisation du genre:
		//	protected Toto(SerializationInfo info, StreamingContext context)
		public static Document ReadDocument = null;
		public static long ReadRevision = 0;

		//	Adapte tous les objets après une désérialisation.
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

			foreach ( TextFlow flow in this.textFlows )
			{
				flow.ReadFinalizeTextStory();
			}
			
			this.containOldText = false;
			Font.FaceInfo[] fonts = Font.Faces;
			foreach ( Objects.Abstract obj in this.Deep(null) )
			{
				obj.ReadFinalize();
				obj.ReadCheckWarnings(fonts, this.readWarnings);

				if ( (obj is Objects.TextBox) || (obj is Objects.TextLine) )
				{
					this.containOldText = true;
				}
			}

			foreach ( TextFlow flow in this.textFlows )
			{
				flow.ReadFinalizeTextObj();
			}
			
#if false
			System.Diagnostics.Debug.WriteLine("Unused tab tags:");
			System.Diagnostics.Debug.WriteLine("  " + string.Join ("\n  ", this.textContext.TabList.GetUnusedTabTags()));
#endif
			
			if ( this.textContext != null )
			{
				this.textContext.TabList.ClearUnusedTabTags();
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

		public bool ContainOldText
		{
			//	Indique si le document contient un ou plusieurs anciens objets TextBox ou TextLine.
			get { return this.containOldText; }
		}

		#region OldStylesToAggregates
		protected void OldStylesToAggregates()
		{
			//	Adapte les anciens styles en agrégats.
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

		public string Write(string filename)
		{
			//	Enregistre le document sur disque.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			
			int undoCount = this.modifier.OpletQueue.UndoActionCount;
			
			try
			{
				this.Modifier.DeselectAll();

				this.ioDirectory = System.IO.Path.GetDirectoryName(filename);

				if ( File.Exists(filename) )
				{
					File.Delete(filename);
				}

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
					else if ( this.ioType == IOType.SoapUncompress )
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
			finally
			{
				while ( undoCount < this.modifier.OpletQueue.UndoActionCount )
				{
					this.modifier.OpletQueue.UndoAction();
				}
				
				this.modifier.OpletQueue.PurgeRedo();
			}

			if ( Misc.IsExtension(filename, ".crdoc") ||
				 Misc.IsExtension(filename, ".icon")  )
			{
				this.Filename = filename;
				this.IsDirtySerialize = false;
			}
			return "";
		}

		protected static IOType ReadIdentifier(Stream stream)
		{
			//	Lit les 8 bytes d'en-tête et vérifie qu'ils contiennent bien "<?icon?>".
			byte[] buffer = new byte[8];
			Common.IO.Reader.Read(stream, buffer, 0, 8);
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

		protected static void WriteIdentifier(Stream stream, IOType type)
		{
			//	Ecrit les 8 bytes d'en-tête "<?icon?>".
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
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le document.
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

			byte[] textContextData = this.textContext == null ? null : this.textContext.Serialize();
			info.AddValue("TextContextData", textContextData);

			info.AddValue("TextFlows", this.textFlows);
		}

		protected Document(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise le document.
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

			if ( this.IsRevisionGreaterOrEqual(1,2,3) )
			{
				byte[] textContextData = (byte[]) info.GetValue("TextContextData", typeof(byte[]));
				
				if ( textContextData != null )
				{
					this.DefaultTextContext();
					this.textContext.Deserialize(textContextData);
				}

				this.textFlows = (UndoableList) info.GetValue("TextFlows", typeof(UndoableList));
			}
			else
			{
				this.textFlows = new UndoableList(this, UndoableListType.TextFlows);
			}
		}

		public string IoDirectory
		{
			//	Retourne le nom du dossier en cours de lecture/écriture.
			get { return this.ioDirectory; }
		}

		public bool IsRevisionGreaterOrEqual(int revision, int version, int subversion)
		{
			//	Indique si un fichier est compatible avec une révision/version.
			long r = ((long)revision<<32) + ((long)version<<16) + (long)subversion;
			return ( Document.ReadRevision >= r );
		}
		#endregion

		
		public void Paint(Graphics graphics, DrawingContext drawingContext, Rectangle clipRect)
		{
			//	Dessine le document.
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
					graphics.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));
					graphics.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
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
					graphics.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));
					graphics.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));

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

		public void Print(Common.Dialogs.Print dp)
		{
			//	Imprime le document.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			this.printer.Print(dp);
		}

		public string Export(string filename)
		{
			//	Exporte le document.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			return this.printer.Export(filename);
		}

		public string ExportPDF(string filename)
		{
			//	Exporte le document.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			return this.exportPDF.FileExport(filename);
		}


		#region TextContext
		protected void DefaultTextContext()
		{
			//	Crée le TextContext et les styles par défaut.
			System.Collections.ArrayList properties;
			Text.TextStyle style;

			this.textContext = new Text.TextContext();
			this.textContext.IsDegradedLayoutEnabled = true;
			this.textContext.IsPropertiesPropertyEnabled = false;

			string black = RichColor.ToString(RichColor.FromBrightness(0));

			double fontSize = (this.type == DocumentType.Pictogram) ? 1.2 : 12.0;

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty("Arial", Misc.DefaultFontStyle("Arial")));
			properties.Add(new Text.Properties.FontSizeProperty(fontSize*Modifier.fontSizeScale, Text.Properties.SizeUnits.Points));
			properties.Add(new Text.Properties.MarginsProperty(0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True));
			properties.Add(new Text.Properties.FontColorProperty(black));
			properties.Add(new Text.Properties.LanguageProperty("fr-ch", 1.0));
			properties.Add(new Text.Properties.LeadingProperty(1.0, Text.Properties.SizeUnits.PercentNotCombining, 0.0, Text.Properties.SizeUnits.Points, 0.0, Text.Properties.SizeUnits.Points, Text.Properties.AlignMode.None));
			properties.Add(new Text.Properties.KeepProperty(1, 1, Text.Properties.ParagraphStartMode.Anywhere, Text.Properties.ThreeState.False, Text.Properties.ThreeState.False));
			style = this.textContext.StyleList.NewTextStyle("Default", Text.TextStyleClass.Paragraph, properties);
			this.textContext.DefaultStyle = style;

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Bold"));
			this.textContext.StyleList.NewMetaProperty("Bold", "X-Bold", 1, properties);

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Italic"));
			this.textContext.StyleList.NewMetaProperty("Italic", "X-Italic", 1, properties);
			
			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.UnderlineProperty(-5, Text.Properties.SizeUnits.Points, 1.0, Text.Properties.SizeUnits.Points, "underline", black));
			this.textContext.StyleList.NewMetaProperty("Underlined", "Underlined", 0, properties);
			
			this.textContext.StyleList.NewMetaProperty("Subscript", "SuperScript", new Text.Properties.FontXscriptProperty (0.6, -0.15));
			this.textContext.StyleList.NewMetaProperty("Superscript", "SuperScript", new Text.Properties.FontXscriptProperty (0.6, 0.25));

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Bold"));
			this.textContext.StyleList.NewMetaProperty("UserX", "UserX", 0, properties);

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty(null, "!Italic"));
			this.textContext.StyleList.NewMetaProperty("UserY", "UserY", 0, properties);

			properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.UnderlineProperty(-5, Text.Properties.SizeUnits.Points, 1.0, Text.Properties.SizeUnits.Points, "underline", black));
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

			Text.TabList tabs = this.textContext.TabList;

			items1.Generator = generator1;
			items1.TabItem   = tabs.NewTab("T1-item", 10.0, Text.Properties.SizeUnits.Points, 0, null, TabPositionMode.Absolute);
			items1.TabBody   = tabs.NewTab("T1-body", 60.0, Text.Properties.SizeUnits.Points, 0, null, TabPositionMode.Absolute);
			
			items2.Generator = generator2;
			items2.TabItem   = tabs.NewTab("T2-item", 10.0, Text.Properties.SizeUnits.Points, 0, null, TabPositionMode.Absolute);
			items2.TabBody   = tabs.NewTab("T2-body", 60.0, Text.Properties.SizeUnits.Points, 0, null, TabPositionMode.Absolute);
			
			items3.Generator = generator3;
			items3.TabItem   = tabs.NewTab("T3-item", 10.0, Text.Properties.SizeUnits.Points, 0, null, TabPositionMode.Absolute);
			items3.TabBody   = tabs.NewTab("T3-body", 60.0, Text.Properties.SizeUnits.Points, 0, null, TabPositionMode.Absolute);
			
			Text.Properties.ManagedParagraphProperty itemList1 = new Text.Properties.ManagedParagraphProperty("ItemList", items1.Save());
			Text.Properties.ManagedParagraphProperty itemList2 = new Text.Properties.ManagedParagraphProperty("ItemList", items2.Save());
			Text.Properties.ManagedParagraphProperty itemList3 = new Text.Properties.ManagedParagraphProperty("ItemList", items3.Save());
			
			this.textContext.StyleList.NewTextStyle("BulletRound",   Text.TextStyleClass.Paragraph, itemList1);
			this.textContext.StyleList.NewTextStyle("BulletNumeric", Text.TextStyleClass.Paragraph, itemList2);
			this.textContext.StyleList.NewTextStyle("BulletAlpha",   Text.TextStyleClass.Paragraph, itemList3);


			this.textContext.StyleList.NewMetaProperty("AlignLeft",   "Align", new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 0.0, 0.0, 0.0, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			this.textContext.StyleList.NewMetaProperty("AlignCenter", "Align", new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 0.0, 0.0, 0.5, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			this.textContext.StyleList.NewMetaProperty("AlignRight",  "Align", new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 0.0, 0.0, 1.0, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			this.textContext.StyleList.NewMetaProperty("AlignJustif", "Align", new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, 1.0, 0.0, 0.0, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined));
			
			this.textContext.StyleList.StyleRedefined += new Support.EventHandler(this.HandleStyleListStyleRedefined);
		}

		
		private void HandleStyleListStyleRedefined(object sender)
		{
			//	Appelé quand un TextStyle est modifié dans StyleList.
			this.textContext.StyleList.UpdateTextStyles();
		}
		
		public string SearchTabNextTag()
		{
			//	Cherche un tag unique pour le prochain tabulateur interactif à créer.
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

		protected int GetTabId(string tag)
		{
			//	Retourne l'identificateur d'un tabulateur interactif d'après son tag.
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

		public string GetTabTag(int id)
		{
			//	Retourne le tag d'un tabulateur interactif.
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Ti{0}", id);
		}

		public UndoableList TextStyles
		{
			//	Liste des styles de texte de ce document.
			get
			{
				this.modifier.OpletQueueEnable = false;
				UndoableList list = new UndoableList(this, UndoableListType.TextStylesInsideDocument);

				//	TODO: comment obtenir la liste de tous les styles ?
				Text.TextStyle defaultStyle = this.TextContext.StyleList["Default", Text.TextStyleClass.Paragraph];
				list.Add(defaultStyle);

				this.modifier.OpletQueueEnable = true;
				return list;
			}
		}
		#endregion

		#region UniqueId
		public int GetNextUniqueObjectId()
		{
			//	Retourne le prochain identificateur unique pour les objets.
			return ++this.uniqueObjectId;
		}

		public int GetNextUniqueAggregateId()
		{
			//	Retourne le prochain identificateur unique pour les noms d'agrégats.
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

		//	Enumérateur permettant de parcourir à plat l'arbre des objets.
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

			public System.Collections.IEnumerator GetEnumerator()
			{
				//	Implémentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Implémentation de IEnumerator:
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

		//	Enumérateur permettant de parcourir à plat depuis la fin l'arbre des objets.
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

			public System.Collections.IEnumerator GetEnumerator()
			{
				//	Implémentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Implémentation de IEnumerator:
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

		//	Enumérateur permettant de parcourir en profondeur l'arbre des objets.
		//	En mode onlySelected, seuls les objets sélectionnés du premier niveau
		//	sont concernés. Un objet fils d'un objet sélectionné du premier niveau
		//	est toujours considéré comme sélectionné, bien qu'il ne le soit pas
		//	physiquement !
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

			public System.Collections.IEnumerator GetEnumerator()
			{
				//	Implémentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Implémentation de IEnumerator:
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

		//	Enumérateur permettant de parcourir en profondeur l'arbre des objets.
		//	L'objet rendu est de type DeepBranchEntry. Ceci permet de savoir si
		//	l'on est ou non à l'intérieur d'une branche quelconque.
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

			public System.Collections.IEnumerator GetEnumerator()
			{
				//	Implémentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Implémentation de IEnumerator:
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
		public static string GetRes(string name)
		{
			//	Retourne une ressource string d'après son nom.
			return Res.Strings.GetString(name);
		}
		#endregion



		protected DocumentType							type;
		protected DocumentMode							mode;
		protected InstallType							installType;
		protected DebugMode								debugMode;
		protected Settings.GlobalSettings				globalSettings;
		protected CommandDispatcher						commandDispatcher;
		protected string								name;
		protected Document								clipboard;
		protected Size									size;
		protected Point									hotSpot;
		protected string								filename;
		protected string								exportDirectory;
		protected string								exportFilename;
		protected int									exportFilter;
		protected bool									isDirtySerialize;
		protected UndoableList							objects;
		protected UndoableList							propertiesAuto;
		protected UndoableList							propertiesSel;
		protected UndoableList							aggregates;
		protected UndoableList							textFlows;
		protected Settings.Settings						settings;
		protected Modifier								modifier;
		protected Wrappers								wrappers;
		protected Notifier								notifier;
		protected Printer								printer;
		protected Common.Dialogs.Print					printDialog;
		protected PDF.Export							exportPDF;
		protected Dialogs								dialogs;
		protected string								ioDirectory;
		protected System.Collections.ArrayList			readWarnings;
		protected IOType								ioType;
		protected Objects.Memory						readObjectMemory;
		protected Objects.Memory						readObjectMemoryText;
		protected System.Collections.ArrayList			readRootStack;
		protected bool									isSurfaceRotation;
		protected double								surfaceRotationAngle;
		protected int									uniqueObjectId = 0;
		protected int									uniqueAggregateId = 0;
		protected Text.TextContext						textContext;
		protected Widgets.HRuler						hRuler;
		protected Widgets.VRuler						vRuler;
		protected bool									containOldText;
	}
}
