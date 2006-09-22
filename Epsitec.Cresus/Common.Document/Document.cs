using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using Epsitec.Common.IO;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Document
{
	public enum DocumentType
	{
		Graphic,		// document graphique
		Pictogram,		// ic�ne
		Text,			// document texte
	}

	public enum DocumentMode
	{
		ReadOnly,		// document uniquement affichable
		Modify,			// document modifiable
		Clipboard,		// document servant uniquement de bloc-notes
		Samples,		// document servant uniquement pour dessiner les miniatures
	}

	public enum InstallType
	{
		Demo,			// version demo
		Full,			// version pleine valide
		Expired,		// version pleine �chue
		Freeware,		// version freeware
	}

	public enum DebugMode
	{
		Release,		// pas de debug
		DebugCommands,	// toutes les commandes de debug
	}

	public enum StyleCategory
	{
		//	Attention: les membres de cette �num�ration sont utilis�s comme index
		//	dans certaines tables; il ne faut donc pas changer leur num�rotation.
		//	cf. Containers.Styles.SelectorName, par exemple.
		
		None		= -1,
		
		Graphic		= 0,	// style graphique
		Paragraph	= 1,	// style de paragraphe
		Character	= 2,	// style de caract�re
		
		Count				// nombre d'�l�ments dans l'�num�ration (3)
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

		public enum FontIncludeMode
		{
			//	Mode d'inclusion des polices dans le document crdoc/zip.
			//	Ne pas changer les valeurs � cause des s�rialisations existantes !

			None	= 0,	// n'inclut aucune police
			Used	= 1,	// inclut les polices utilis�es
			All		= 2,	// inclut les polices utilis�es plus toutes les polices d�finies
		}

		public enum ImageIncludeMode
		{
			//	Mode d'inclusion des images dans le document crdoc/zip.
			//	Ne pas changer les valeurs � cause des s�rialisations existantes !

			None	= 0,	// n'inclut aucune image
			Defined	= 1,	// inclut les images selons leurs d�finitions
			All		= 2,	// inclut toutes les images d�finies
		}


		public Document(DocumentType type, DocumentMode mode, InstallType installType, DebugMode debugMode, Settings.GlobalSettings globalSettings, CommandDispatcher commandDispatcher, CommandContext commandContext)
		{
			//	Cr�e un nouveau document vide.
			this.UniqueIDCreate();
			this.type = type;
			this.mode = mode;
			this.installType = installType;
			this.debugMode = debugMode;
			this.fontIncludeMode = FontIncludeMode.Used;
			this.imageIncludeMode = ImageIncludeMode.Defined;
			this.globalSettings = globalSettings;
			this.commandDispatcher = commandDispatcher;
			this.commandContext = commandContext;
			this.initializationInProgress = true;

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
			this.uniqueParagraphStyleId = 0;
			this.uniqueCharacterStyleId = 0;
			this.containOldText = false;
			this.fontList = null;

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

			if ( this.mode == DocumentMode.Samples )
			{
				this.modifier  = new Modifier(this);
				this.wrappers  = new Wrappers(this);
				this.notifier  = new Notifier(this);
			}

			//	Il ne faut pas cr�er de ImageCache lorsqu'on cr�e un document
			//	juste pour une ic�ne !
			if ( this.mode == DocumentMode.Modify )
			{
				this.imageCache = new ImageCache();
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

		public void Dispose()
		{
			if ( this.modifier != null )
			{
				this.modifier.Dispose();
				this.modifier = null;
			}

			if ( this.wrappers != null )
			{
				this.wrappers.Dispose();
				this.wrappers = null;
			}

			if ( this.notifier != null )
			{
				this.notifier.Dispose();
				this.notifier = null;
			}

			if ( this.imageCache != null )
			{
				this.imageCache = null;
			}

			if ( this.dialogs != null )
			{
				this.dialogs.Dispose();
				this.dialogs = null;
			}

			if ( this.settings != null )
			{
				this.settings.Dispose();
				this.settings = null;
			}

			if ( this.printer != null )
			{
				this.printer.Dispose();
				this.printer = null;
			}

			if ( this.exportPDF != null )
			{
				this.exportPDF.Dispose();
				this.exportPDF = null;
			}
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

		public FontIncludeMode FontIncludeModeValue
		{
			//	Mode d'inclusion des polices.
			get { return this.fontIncludeMode; }
			set { this.fontIncludeMode = value; }
		}

		public ImageIncludeMode ImageIncludeModeValue
		{
			//	Mode d'inclusion des polices.
			get { return this.imageIncludeMode; }
			set { this.imageIncludeMode = value; }
		}

		public Settings.GlobalSettings GlobalSettings
		{
			//	R�glages globaux.
			get { return this.globalSettings; }
		}
		
		public CommandDispatcher CommandDispatcher
		{
			//	CommandDispatcher de l'�diteur.
			get { return this.commandDispatcher; }
		}

		public CommandContext CommandContext
		{
			get { return this.commandContext; }
		}
		
		public string Name
		{
			//	Nom du document.
			get { return this.name; }
			set { this.name = value; }
		}

		public Document Clipboard
		{
			//	Bloc-notes associ�.
			get { return this.clipboard; }
			set { this.clipboard = value; }
		}

		public bool InitializationInProgress
		{
			//	Initialisation du document en cours.
			//	Voir le commentaire dans DocumentEditor.cs, InitializationInProgress.
			get { return this.initializationInProgress; }
			set { this.initializationInProgress = value; }
		}

		#region ForSamples
		public Document DocumentForSamples
		{
			//	Donne le document sp�cial servant � dessiner les �chantillons.
			get
			{
				if ( this.documentForSamples == null )
				{
					this.documentForSamples = new Document(DocumentType.Graphic, DocumentMode.Samples, InstallType.Full, DebugMode.Release, this.GlobalSettings, null, null);
					this.documentForSamples.TextContext = this.TextContext;
				}

				return this.documentForSamples;
			}
		}

		public Objects.TextBox2 ObjectForSamplesParagraph
		{
			//	Donne l'objet TextBox2 servant � dessiner les �chantillons des styles de paragraphe.
			get
			{
				if ( this.objectForSamplesParagraph == null )
				{
					this.objectForSamplesParagraph = new Objects.TextBox2(this.DocumentForSamples, null);
					this.objectForSamplesParagraph.CreateForSample();
					this.objectForSamplesParagraph.EditInsertText(Res.Strings.Sample.Paragraph, "", "");
				}

				return this.objectForSamplesParagraph;
			}
		}

		public Objects.TextLine2 ObjectForSamplesCharacter
		{
			//	Donne l'objet TextLine2 servant � dessiner les �chantillons des styles de caract�re.
			get
			{
				if ( this.objectForSamplesCharacter == null )
				{
					this.objectForSamplesCharacter = new Objects.TextLine2(this.DocumentForSamples, null);
					this.objectForSamplesCharacter.CreateForSample();
					this.objectForSamplesCharacter.EditInsertText(Res.Strings.Sample.Character, "", "");
				}

				return this.objectForSamplesCharacter;
			}
		}
		#endregion

		public CommandState GetCommandState(string command)
		{
			CommandContext context = this.CommandContext;
			CommandState state = context.GetCommandState (command);

			return state;
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
					this.CreateDefaultTextContext();
				}
				return this.textContext;
			}
			set
			{
				this.textContext = value;
			}
		}

		public Widgets.HRuler HRuler
		{
			//	R�gle horizontale.
			get { return this.hRuler; }
			set { this.hRuler = value; }
		}

		public Widgets.VRuler VRuler
		{
			//	R�gle verticale.
			get { return this.vRuler; }
			set { this.vRuler = value; }
		}


		public UndoableList PropertiesAuto
		{
			//	Liste des propri�t�s automatiques de ce document.
			get { return this.propertiesAuto; }
		}

		public UndoableList PropertiesSel
		{
			//	Liste des propri�t�s s�lectionn�es de ce document.
			get { return this.propertiesSel; }
		}

		public UndoableList Aggregates
		{
			//	Liste des aggr�gats de ce document.
			get { return this.aggregates; }
		}

		public UndoableList TextFlows
		{
			//	Liste des flux de textes de ce document.
			get { return this.textFlows; }
		}


		public Settings.Settings Settings
		{
			//	R�glages de ce document.
			get { return this.settings; }
		}

		public Modifier Modifier
		{
			//	Modificateur �ventuel pour ce document.
			get
			{
				return this.modifier;
			}
		}

		public ImageCache ImageCache
		{
			//	Cache des images de ce document.
			get { return this.imageCache; }
		}

		public Wrappers Wrappers
		{
			//	Wrappers �ventuel pour ce document.
			get { return this.wrappers; }
		}

		public Notifier Notifier
		{
			//	Notificateur �ventuel pour ce document.
			get { return this.notifier; }
		}

		public Dialogs Dialogs
		{
			//	Dialogues �ventuels pour ce document.
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
			//	Rotation sp�ciale pour calculer SurfaceAnchor en cours.
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
			//	Angle de la rotation sp�ciale pour calculer SurfaceAnchor.
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

		public Canvas.IconKey[] IconKeys
		{
			//	Donne les cl�s pour les ic�nes de toutes les pages.
			get
			{
				Canvas.IconKey[] keys = new Canvas.IconKey[this.objects.Count];
				for ( int i=0 ; i<this.objects.Count ; i++ )
				{
					Objects.Page page = this.objects[i] as Objects.Page;
					
					Canvas.IconKey key = new Canvas.IconKey();
					key.Size     = this.GetPageSize(page);
					key.Language = page.Language;
					key.Style    = page.PageStyle;
					key.PageRank = i;

					keys[i] = key;
				}
				return keys;
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
			//	Nom du fichier associ�.
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
			//	Nom du dossier d'exportation associ�.
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
			//	Nom du fichier (sans dossier) d'exportation associ�.
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
			//	Type du fichier d'exportation associ�.
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
			//	Indique si la s�rialisation est n�cessaire.
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
				ZipFile zip = new ZipFile();
				string err = "";

				if (!Misc.IsExtension(filename, ".icon") && zip.TryLoadFile(filename))
				{
					// Fichier CrDoc au format ZIP, charg� avec succ�s.
					using (MemoryStream stream = new MemoryStream(zip["document.data"].Data))
					{
						err = this.Read(stream, System.IO.Path.GetDirectoryName(filename), zip);
					}
				}
				else
				{
					// D�s�rialisation standard; c'est un ancien fichier CrDoc.
					using (Stream stream = File.OpenRead(filename))
					{
						err = this.Read(stream, System.IO.Path.GetDirectoryName(filename), null);
					}
				}

				if (err == "")
				{
					if (Misc.IsExtension(filename, ".crdoc")||
						Misc.IsExtension(filename, ".icon"))
					{
						this.Filename = filename;
						this.globalSettings.LastFilenameAdd(filename);
						this.IsDirtySerialize = false;
					}
					if (Misc.IsExtension(filename, ".crmod"))
					{
						this.globalSettings.LastModelAdd(filename);
					}
				}
				else
				{
					this.IsDirtySerialize = false;
				}
				return err;
			}
			catch ( System.Exception e )
			{
				this.IsDirtySerialize = false;
				return e.Message;
			}
		}

		public string Read(Stream stream, string directory)
		{
			//	Ouvre un document s�rialis�, soit parce que l'utilisateur veut ouvrir
			//	explicitement un fichier, soit par Engine.
			return this.Read(stream, directory, null);
		}

		protected string Read(Stream stream, string directory, ZipFile zip)
		{
			//	Ouvre un document s�rialis�, zipp� ou non.
			this.ioDirectory = directory;
			this.readWarnings = new System.Collections.ArrayList();

			IOType type = Document.ReadIdentifier(stream);
			if ( type == IOType.Unknow )
			{
				return Res.Strings.Error.BadFile;
			}

			//	Initialise la variable statique permettant � tous les constructeurs
			//	de conna�tre le pointeur au document.
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
					if (zip == null)
					{
						string compressorName;
						using (Stream compressor = IO.Decompression.CreateStream(stream, out compressorName))
						{
							doc = (Document) formatter.Deserialize(compressor);
							doc.imageCache = new ImageCache();
						}
					}
					else
					{
						doc = (Document) formatter.Deserialize(stream);
						doc.FontReadAll(zip);
						doc.ImageCacheAll(zip, doc.imageIncludeMode);
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

			if ( doc.textContext != null )
			{
				doc.textContext.StyleList.StyleRedefined -= new Support.EventHandler(doc.HandleStyleListStyleRedefined);

				if ( doc.wrappers != null )
				{
					doc.wrappers.TextContextChangedDisconnect();
				}
			}
			
			this.objects = doc.objects;
			this.propertiesAuto = doc.propertiesAuto;
			this.aggregates = doc.aggregates;
			this.textFlows = doc.textFlows;
			this.textContext = doc.textContext; 
			this.uniqueObjectId = doc.uniqueObjectId;
			this.uniqueAggregateId = doc.uniqueAggregateId;
			this.uniqueParagraphStyleId = doc.uniqueParagraphStyleId;
			this.uniqueCharacterStyleId = doc.uniqueCharacterStyleId;
			this.imageCache = doc.imageCache;
			this.fontIncludeMode = doc.fontIncludeMode;
			this.imageIncludeMode = doc.imageIncludeMode;
			
			if ( this.textContext != null )
			{
				this.textContext.StyleList.StyleRedefined += new Support.EventHandler(this.HandleStyleListStyleRedefined);

				if ( this.wrappers != null )
				{
					this.wrappers.TextContextChangedConnect();
				}
			}
			
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
					this.Modifier.ObjectMemory     = doc.readObjectMemory;
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
			//	Retourne la liste �ventuelle des warnings de lecture.
			get
			{
				return this.readWarnings;
			}
		}

		
		#region VersionDeserializationBinder Class
		sealed class VersionDeserializationBinder : Common.IO.GenericDeserializationBinder
		{
			public VersionDeserializationBinder()
			{
			}
			
			public override System.Type BindToType(string assemblyName, string typeName) 
			{
				//	Retourne un type correspondant � l'application courante, afin
				//	d'accepter de d�s�rialiser un fichier g�n�r� par une application
				//	ayant un autre num�ro de r�vision.
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
		#endregion

		//	Utilis� par les constructeurs de d�s�rialisation du genre:
		//	protected Toto(SerializationInfo info, StreamingContext context)
		public static Document ReadDocument = null;
		public static long ReadRevision = 0;

		protected void ReadFinalize()
		{
			//	Adapte tous les objets apr�s une d�s�rialisation.
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
			foreach ( Objects.Abstract obj in this.Deep(null) )
			{
				obj.ReadFinalize();
				obj.ReadCheckWarnings(this.readWarnings);

				if ( (obj is Objects.TextBox) || (obj is Objects.TextLine) )
				{
					this.containOldText = true;
				}
			}

			if ( this.type != DocumentType.Pictogram )
			{
				TextFlow.ReadCheckWarnings(this.textFlows, this.readWarnings);
			}

			foreach ( TextFlow flow in this.textFlows )
			{
				flow.ReadFinalizeTextObj();
			}
			
#if false
			System.Diagnostics.Debug.WriteLine("Unused tab tags:");
			System.Diagnostics.Debug.WriteLine("  " + string.Join("\n  ", this.textContext.TabList.GetUnusedTabTags()));
#endif
			
			if ( this.textContext != null )
			{
				this.textContext.TabList.ClearUnusedTabTags();
				this.textContext.GeneratorList.ClearUnusedGenerators();

				Text.TextStyle[] list = this.textContext.StyleList.StyleMap.GetSortedStyles();
				if ( list.Length == 0 )
				{
					//	Le document n'a aucun style. Il faut rechercher si par hasard, il
					//	y en a quand-m�me certains qui seraient utilisables.
					
					Text.TextStyle paraStyle = this.textContext.StyleList["Default", Text.TextStyleClass.Paragraph];
					Text.TextStyle charStyle = this.textContext.StyleList["Default", Text.TextStyleClass.Text];
					
					if ( paraStyle == null )
					{
						paraStyle = this.textContext.DefaultParagraphStyle;
					}
					
					//	Tous les documents qui contenaient du texte ont toujours au moins
					//	d�fini un style nomm� 'Default'. Si �a ne devait pas �tre le cas,
					//	crash garanti.
					
					if ( paraStyle == null )  throw new System.ArgumentNullException("No default paragraph style found");
					
					this.textContext.DefaultParagraphStyle = paraStyle;
					this.textContext.StyleList.StyleMap.SetRank(null, paraStyle, 0);
					this.textContext.StyleList.StyleMap.SetCaption(null, paraStyle, Res.Strings.Style.Paragraph.Base);

					//	Cherche un style de caract�re; si aucun n'est trouv�, on en cr�e
					//	un nouveau de toutes pi�ces (c'est facile). Toute une s�rie de
					//	documents cr��s avec des versions de d�cembre 2005 n'avaient pas
					//	de style de caract�re par d�faut.
					
					if ( charStyle == null )
					{
						charStyle = this.textContext.DefaultTextStyle;
					}
					if ( charStyle == null )
					{
						charStyle = this.textContext.StyleList.NewTextStyle(null, "Default", Text.TextStyleClass.Text);
					}
					
					System.Diagnostics.Debug.Assert(charStyle != null);
					
					this.textContext.DefaultTextStyle = charStyle;
					this.textContext.StyleList.StyleMap.SetRank(null, charStyle, 0);
					this.textContext.StyleList.StyleMap.SetCaption(null, charStyle, Res.Strings.Style.Character.Base);
				}
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
			//	Adapte les anciens styles en agr�gats.
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

				if (this.type == DocumentType.Pictogram)
				{
					using (Stream stream = File.OpenWrite(filename))
					{
						Document.WriteIdentifier(stream, this.ioType);

						if (this.ioType == IOType.BinaryCompress)
						{
							//?Stream compressor = IO.Compression.CreateBZip2Stream(stream);
							Stream compressor = IO.Compression.CreateDeflateStream(stream, 1);
							BinaryFormatter formatter = new BinaryFormatter();
							formatter.Serialize(compressor, this);
							compressor.Close();
						}
						else if (this.ioType == IOType.SoapUncompress)
						{
							SoapFormatter formatter = new SoapFormatter();
							formatter.Serialize(stream, this);
						}
					}
				}
				else
				{
					this.FontUpdate();
					this.ImageFlushUnused();
					this.imageCache.GenerateShortNames();
					this.ImageUpdate();

					byte[] data;
					using (MemoryStream stream = new MemoryStream())
					{
						Document.WriteIdentifier(stream, this.ioType);

						BinaryFormatter formatter = new BinaryFormatter();
						formatter.Serialize(stream, this);
						data = stream.ToArray();
					}

					ZipFile zip = new ZipFile();
					zip.AddEntry("document.data", data);
					this.WriteMiniature(zip);
					this.WriteStatistics(zip);
					this.imageCache.WriteData(zip, this.imageIncludeMode);
					this.FontWriteAll(zip);
					zip.CompressionLevel = 6;
					zip.SaveFile(filename);
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
			DocumentCache.Remove(filename);
			return "";
		}

		protected static IOType ReadIdentifier(Stream stream)
		{
			//	Lit les 8 bytes d'en-t�te et v�rifie qu'ils contiennent bien "<?icon?>".
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
			//	Ecrit les 8 bytes d'en-t�te "<?icon?>".
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


		public static string DisplayOriginalSamples
		{
			//	Retourne le nom � afficher pour le dossier contenant les exemples originaux.
			get
			{
				return Res.Strings.Directory.OriginalSamples;
			}
		}

		public static string DisplayMySamples
		{
			//	Retourne le nom � afficher pour le dossier contenant les exemples personnels.
			get
			{
				return Res.Strings.Directory.MySamples;
			}
		}

		public static string DirectoryOriginalSamples
		{
			//	Retourne le nom du dossier contenant les exemples originaux.
			get
			{
				return string.Concat(Common.Support.Globals.Directories.Executable, "\\", Document.DisplayOriginalSamples);
			}
		}

		public static string DirectoryMySamples
		{
			//	Retourne le nom du dossier contenant les exemples personnels.
			get
			{
				//	Attention, on re�oit:
				//	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Cr�sus Documents\2.0.2.0
				//	'Cr�sus Documents' au lieu de 'Cr�sus documents' qui est le vrai nom !
				string path = Common.Support.Globals.Directories.UserAppData;
				int i = path.LastIndexOf("\\");
				if (i > 0)
				{
					path = path.Substring(0, i);  // supprime le dossier "1.0.0.0" � la fin
				}
				return string.Concat(path, "\\", Document.DisplayMySamples);
			}
		}

		public static bool RedirectionFilename(ref string filename)
		{
			//	Redirige un nom de fichier de 'Exemples originaux' vers 'Mes exemples', si n�cessaire.
			if (string.IsNullOrEmpty(filename))
			{
				return false;
			}

			string dir = System.IO.Path.GetDirectoryName(filename);
			if (string.Equals(dir, Document.DirectoryOriginalSamples, System.StringComparison.OrdinalIgnoreCase))
			{
				string file = System.IO.Path.GetFileName(filename);
				filename = string.Concat(Document.DirectoryMySamples, "\\", file);
				return true;
			}

			return false;
		}

		public static bool RedirectionDirectory(ref string directory)
		{
			//	Redirige un nom de dossier de 'Exemples originaux' vers 'Mes exemples', si n�cessaire.
			if (string.IsNullOrEmpty(directory))
			{
				return false;
			}

			if (string.Equals(directory, Document.DirectoryOriginalSamples, System.StringComparison.OrdinalIgnoreCase))
			{
				directory = Document.DirectoryMySamples;
				return true;
			}

			return false;
		}

		
		#region Serialization
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise le document.
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

				byte[] textContextData = this.textContext == null ? null : this.textContext.Serialize();
				info.AddValue("TextContextData", textContextData);

				info.AddValue("TextFlows", this.textFlows);
				info.AddValue("FontList", this.fontList);
				info.AddValue("FontIncludeMode", this.fontIncludeMode);
				info.AddValue("ImageIncludeMode", this.imageIncludeMode);
			}

			info.AddValue("UniqueObjectId", this.uniqueObjectId);
			info.AddValue("UniqueAggregateId", this.uniqueAggregateId);
			info.AddValue("UniqueParagraphStyleId", this.uniqueParagraphStyleId);
			info.AddValue("UniqueCharacterStyleId", this.uniqueCharacterStyleId);
			info.AddValue("Objects", this.objects);
			info.AddValue("Properties", this.propertiesAuto);
			info.AddValue("Aggregates", this.aggregates);
		}

		protected Document(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui d�s�rialise le document.
			this.type = (DocumentType) info.GetValue("Type", typeof(DocumentType));
			this.name = info.GetString("Name");

			if ( this.type == DocumentType.Pictogram )
			{
				this.size = (Size) info.GetValue("Size", typeof(Size));
				this.hotSpot = (Point) info.GetValue("HotSpot", typeof(Point));

				this.textContext = null;
				this.textFlows = new UndoableList(this, UndoableListType.TextFlows);
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

				if ( this.IsRevisionGreaterOrEqual(1,2,3) )
				{
					byte[] textContextData = (byte[]) info.GetValue("TextContextData", typeof(byte[]));
				
					if ( textContextData != null )
					{
						this.CreateDefaultTextContext();
						this.textContext.Deserialize(textContextData);
					}

					this.textFlows = (UndoableList) info.GetValue("TextFlows", typeof(UndoableList));
				}
				else
				{
					this.textFlows = new UndoableList(this, UndoableListType.TextFlows);
				}

				if (this.IsRevisionGreaterOrEqual(2, 0, 1))
				{
					this.fontList = (List<OpenType.FontName>) info.GetValue("FontList", typeof(List<OpenType.FontName>));
				}
				else
				{
					this.fontList = null;
				}

				if (this.IsRevisionGreaterOrEqual(2, 0, 2))
				{
					this.fontIncludeMode = (FontIncludeMode) info.GetValue("FontIncludeMode", typeof(FontIncludeMode));
					this.imageIncludeMode = (ImageIncludeMode) info.GetValue("ImageIncludeMode", typeof(ImageIncludeMode));
				}
				else
				{
					this.fontIncludeMode = FontIncludeMode.Used;
					this.imageIncludeMode = ImageIncludeMode.Defined;
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

			if ( this.IsRevisionGreaterOrEqual(1,6,2) )
			{
				this.uniqueParagraphStyleId = info.GetInt32("UniqueParagraphStyleId");
				this.uniqueCharacterStyleId = info.GetInt32("UniqueCharacterStyleId");
			}
			else
			{
				this.uniqueParagraphStyleId = 0;
				this.uniqueCharacterStyleId = 0;
			}
		}

		public string IoDirectory
		{
			//	Retourne le nom du dossier en cours de lecture/�criture.
			get { return this.ioDirectory; }
		}

		public bool IsRevisionGreaterOrEqual(int revision, int version, int subversion)
		{
			//	Indique si un fichier est compatible avec une r�vision/version.
			long r = ((long)revision<<32) + ((long)version<<16) + (long)subversion;
			return ( Document.ReadRevision >= r );
		}
		#endregion


		#region Miniature
		protected void WriteMiniature(ZipFile zip)
		{
			//	Ecrit la miniature de la premi�re page dans le fichier zip.
			string filename;
			byte[] data;
			if (this.printer.Miniature(new Size(100, 100), out filename, out data))
			{
				zip.AddEntry(filename, data);
			}
		}
		#endregion


		#region Statistics
		[System.Serializable()]
		public class Statistics : ISerializable
		{
			public Statistics()
			{
			}

			public Size PageSize
			{
				//	Dimensions d'une page en unit�s internes.
				get
				{
					return this.pageSize;
				}
				set
				{
					this.pageSize = value;
				}
			}

			public string PageFormat
			{
				//	Format d'une page en clair ("A4" ou "123 x 456").
				get
				{
					return this.pageFormat;
				}
				set
				{
					this.pageFormat = value;
				}
			}

			public int PagesCount
			{
				//	Nombre total de pages.
				get
				{
					return this.pagesCount;
				}
				set
				{
					this.pagesCount = value;
				}
			}

			public int LayersCount
			{
				//	Nombre total de calques.
				get
				{
					return this.layersCount;
				}
				set
				{
					this.layersCount = value;
				}
			}

			public int ObjectsCount
			{
				//	Nombre total d'objets.
				get
				{
					return this.objectsCount;
				}
				set
				{
					this.objectsCount = value;
				}
			}

			public int ComplexesCount
			{
				//	Nombre total d'objets d�grad�s ou transparents.
				get
				{
					return this.complexesCount;
				}
				set
				{
					this.complexesCount = value;
				}
			}

			public int FontsCount
			{
				//	Nombre total de polices.
				get
				{
					return this.fontsCount;
				}
				set
				{
					this.fontsCount = value;
				}
			}

			public int ImagesCount
			{
				//	Nombre total d'images bitmap.
				get
				{
					return this.imagesCount;
				}
				set
				{
					this.imagesCount = value;
				}
			}


			public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				//	S�rialise l'objet.
				info.AddValue("Version", 2);
				info.AddValue("PageSize", this.pageSize);
				info.AddValue("PageFormat", this.pageFormat);
				info.AddValue("PagesCount", this.pagesCount);
				info.AddValue("LayersCount", this.layersCount);
				info.AddValue("ObjectsCount", this.objectsCount);
				info.AddValue("ComplexesCount", this.complexesCount);
				info.AddValue("FontsCount", this.fontsCount);
				info.AddValue("ImagesCount", this.imagesCount);
			}

			protected Statistics(SerializationInfo info, StreamingContext context)
			{
				//	Constructeur qui d�s�rialise l'objet.
				int version = info.GetInt32("Version");

				this.pageSize = (Size) info.GetValue("PageSize", typeof(Size));
				this.pageFormat = info.GetString("PageFormat");
				this.pagesCount = info.GetInt32("PagesCount");
				this.layersCount = info.GetInt32("LayersCount");
				this.objectsCount = info.GetInt32("ObjectsCount");
				this.complexesCount = info.GetInt32("ComplexesCount");

				if (version >= 2)
				{
					this.fontsCount = info.GetInt32("FontsCount");
					this.imagesCount = info.GetInt32("ImagesCount");
				}
			}


			protected Size					pageSize;
			protected string				pageFormat;
			protected int					pagesCount;
			protected int					layersCount;
			protected int					objectsCount;
			protected int					complexesCount;
			protected int					fontsCount;
			protected int					imagesCount;
		}

		protected void WriteStatistics(ZipFile zip)
		{
			//	Ecrit la description du document dans le fichier zip.
			Statistics stat = new Statistics();
			stat.PageSize = this.PageSize;

			string format = Dialogs.PaperFormat(stat.PageSize);
			if (format == null)
			{
				stat.PageFormat = string.Format("{0} x {1}", this.modifier.RealToString(stat.PageSize.Width), this.modifier.RealToString(stat.PageSize.Height));
			}
			else
			{
				stat.PageFormat = format;
			}

			stat.PagesCount = this.modifier.StatisticTotalPages();
			stat.LayersCount = this.modifier.StatisticTotalLayers();
			stat.ObjectsCount = this.modifier.StatisticTotalObjects();
			stat.ComplexesCount = this.modifier.StatisticTotalComplex();

			List<OpenType.FontName> fontList = new List<OpenType.FontName>();
			TextFlow.StatisticFonts(fontList, this.TextFlows);
			this.modifier.StatisticFonts(fontList);
			stat.FontsCount = fontList.Count;

			System.Collections.ArrayList imageList = this.modifier.StatisticImages();
			stat.ImagesCount = imageList.Count;

			byte[] data = Serialization.SerializeToMemory(stat);
			zip.AddEntry("statistics.data", data);
		}
		#endregion


		#region Fonts
		protected void FontUpdate()
		{
			//	Met � jour la liste de toutes les polices utilis�es dans le document.
			this.fontList = new List<OpenType.FontName>();

			TextFlow.StatisticFonts(this.fontList, this.textFlows);
			this.modifier.StatisticFonts(this.fontList);

			if (this.fontIncludeMode == FontIncludeMode.All)
			{
				//	Fouille tous les styles � la recherche des polices d�finies.
				Text.TextStyle[] list = this.TextContext.StyleList.StyleMap.GetSortedStyles();
				foreach (Text.TextStyle style in list)
				{
					Text.Property[] properties = style.FindProperties(Text.Properties.WellKnownType.Font);
					foreach (Text.Property property in properties)
					{
						Text.Properties.FontProperty font = property as Text.Properties.FontProperty;
						OpenType.FontName f = new Epsitec.Common.OpenType.FontName(font.FaceName, font.StyleName);
						if (!this.fontList.Contains(f))
						{
							this.fontList.Add(f);
						}
					}
				
				}
			}

			this.fontList.Sort();
		}

		protected void FontWriteAll(ZipFile zip)
		{
			//	Ecrit sur disque tous les fichiers des polices utilis�es dans le document.
			if (this.fontList == null || this.fontIncludeMode == FontIncludeMode.None)
			{
				return;
			}

			for (int i=0; i<this.fontList.Count; i++)
			{
				OpenType.FontName fontName = this.fontList[i];

				if (Document.IsStandardFont(fontName.FaceName))
				{
					continue;
				}

				OpenType.Font font = TextContext.GetFont(fontName.FaceName, fontName.StyleName);
				System.Diagnostics.Debug.Assert(font != null);

				string name = Document.GetFontFilename(fontName, i);
				
				zip.AddEntry(name, font.FontData.Data.Array);
			}
		}

		protected void FontReadAll(ZipFile zip)
		{
			//	Lit sur disque tous les fichiers des polices utilis�es dans le document.
			if (this.fontList == null || this.fontIncludeMode == FontIncludeMode.None)
			{
				return;
			}

			for (int i=0; i<this.fontList.Count; i++)
			{
				OpenType.FontName fontName = this.fontList[i];

				if (Document.IsStandardFont(fontName.FaceName))
				{
					continue;
				}

				string name = Document.GetFontFilename(fontName, i);
				byte[] data = zip[name].Data;  // lit les donn�es dans le fichier zip
				if (data != null)
				{
					Drawing.Font.RegisterDynamicFont(data);
				}
			}
		}

		protected static bool IsStandardFont(string faceName)
		{
			//	Indique s'il s'agit d'une police courante dont il est certain qu'elle
			//	existe sur tous les ordinateurs.
			return (faceName == "Arial"           ||
					faceName == "Times New Roman" ||
					faceName == "Courier New"     );
		}

		protected static string GetFontFilename(OpenType.FontName fontName, int rank)
		{
			//	Retourne le nom de fichier � utiliser pour une police donn�e.
			//	Un nom tr�s simple 'n.font' est tout � fait suffisant.
			return string.Format("fonts/{0}.font", rank.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		#endregion

		#region Images
		protected void ImageFlushUnused()
		{
			//	Supprime toutes les images inutilis�es du cache des images.
			List<string> filenames = new List<string>();
			foreach (Objects.Abstract obj in this.Deep(null))
			{
				Properties.Image propImage = obj.PropertyImage;
				if (propImage != null)
				{
					if (!filenames.Contains(propImage.Filename))
					{
						filenames.Add(propImage.Filename);
					}
				}
			}

			this.imageCache.FlushUnused(filenames);
		}

		protected void ImageUpdate()
		{
			//	Met � jour les informations ShortName et InsideDoc.
			//	ShortName est mis � jour dans les propri�t�s des objets Image du document.
			//	InsideDoc est mis � jour dans le cache des images.
			this.imageCache.ClearInsideDoc();

			foreach (Objects.Abstract obj in this.Deep(null))
			{
				Properties.Image propImage = obj.PropertyImage;
				if (propImage != null)
				{
					ImageCache.Item item = this.imageCache.Get(propImage.Filename);

					if (item != null)
					{
						propImage.ShortName = item.ShortName;

						if (propImage.InsideDoc)
						{
							item.InsideDoc = true;
						}
					}
				}
			}
		}

		protected void ImageCacheAll(ZipFile zip, ImageIncludeMode imageIncludeMode)
		{
			//	Cache toutes les donn�es pour les objets Images du document.
			this.imageCache = new ImageCache();

			foreach (Objects.Abstract obj in this.Deep(null))
			{
				Properties.Image propImage = obj.PropertyImage;
				if (propImage != null)
				{
					this.imageCache.ReadData(zip, imageIncludeMode, propImage);
				}
			}
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
						if ( obj.IsHide )  continue;  // objet cach� ?
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

						if ( obj.IsHide )  // objet cach� ?
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
		protected void CreateDefaultTextContext()
		{
			//	Cr�e le TextContext et les styles par d�faut.
			this.textContext = new Text.TextContext();
			this.textContext.IsDegradedLayoutEnabled = true;
			this.textContext.IsPropertiesPropertyEnabled = false;

			string black = RichColor.ToString(RichColor.FromBrightness(0));
			string red = RichColor.ToString(RichColor.FromRgb(1.0, 0.0, 0.0));
			string green = RichColor.ToString(RichColor.FromRgb(0.0, 1.0, 0.0));
			string indentAttribute = Text.TabList.PackToAttribute(System.Globalization.RegionInfo.CurrentRegion.IsMetric ? "LevelMultiplier:10 mm" : "LevelMultiplier:0.5 in");
			double fontSize = (this.type == DocumentType.Pictogram) ? 1.2 : 12.0;

			System.Collections.ArrayList properties = new System.Collections.ArrayList();
			properties.Add(new Text.Properties.FontProperty("Arial", Misc.DefaultFontStyle("Arial"), "kern", "liga"));
			properties.Add(new Text.Properties.FontSizeProperty(fontSize*Modifier.FontSizeScale, Text.Properties.SizeUnits.Points, 0.0));
			properties.Add(new Text.Properties.MarginsProperty(0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True, 0, indentAttribute));
			properties.Add(new Text.Properties.FontColorProperty(black));
			properties.Add(new Text.Properties.LanguageProperty(System.Globalization.CultureInfo.CurrentCulture.Name, 1.0));
			properties.Add(new Text.Properties.LeadingProperty(1.0, Text.Properties.SizeUnits.PercentNotCombining, 0.0, Text.Properties.SizeUnits.Points, 0.0, Text.Properties.SizeUnits.Points, Text.Properties.AlignMode.None));
			properties.Add(new Text.Properties.KeepProperty(2, 2, Text.Properties.ParagraphStartMode.Anywhere, Text.Properties.ThreeState.False, Text.Properties.ThreeState.False));
			Text.TextStyle paraStyle = this.textContext.StyleList.NewTextStyle(null, "Default", Text.TextStyleClass.Paragraph, properties);
			Text.TextStyle charStyle = this.textContext.StyleList.NewTextStyle(null, "Default", Text.TextStyleClass.Text);
			
			#region Experimental Code
#if false
			Text.TextStyle[] baseStyles = new Text.TextStyle[] { paraStyle };
			
			Text.Generator generator1 = this.textContext.GeneratorList.NewGenerator("bullet-1");
			Text.Generator generator2 = this.textContext.GeneratorList.NewGenerator("num-1");
			Text.Generator generator3 = this.textContext.GeneratorList.NewGenerator("alpha-1");
			Text.Generator generator4 = this.textContext.GeneratorList.NewGenerator("num-2");
			
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant, "", "", Text.Generator.Casing.Default, "\u25CF"));
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant, "", "", Text.Generator.Casing.Default, "\u25CB", true));
			generator1.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Constant, "", "", Text.Generator.Casing.Default, "-", true));
			
			generator1[1].ValueProperties = new Text.Property[] { new Text.Properties.FontColorProperty(red) };
			generator1[2].ValueProperties = new Text.Property[] { new Text.Properties.FontColorProperty(green) };
			
			generator2.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Numeric, "", "."));
			
			generator3.GlobalPrefix = "";
			generator3.GlobalSuffix = ")";
			generator3.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Alphabetic, "", "", Text.Generator.Casing.Lower));
			generator3.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Numeric,    "-", ""));
			generator3.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Roman,      "(", "", Text.Generator.Casing.Lower, null, true));
			
			generator4.GlobalSuffix = " ";
			generator4.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Numeric,  "", "", Text.Generator.Casing.Default));
			generator4.Add(Text.Generator.CreateSequence(Text.Generator.SequenceType.Numeric, ".", "", Text.Generator.Casing.Default));
			
			Text.ParagraphManagers.ItemListManager.Parameters items1 = new Text.ParagraphManagers.ItemListManager.Parameters();
			Text.ParagraphManagers.ItemListManager.Parameters items2 = new Text.ParagraphManagers.ItemListManager.Parameters();
			Text.ParagraphManagers.ItemListManager.Parameters items3 = new Text.ParagraphManagers.ItemListManager.Parameters();
			Text.ParagraphManagers.ItemListManager.Parameters items4 = new Text.ParagraphManagers.ItemListManager.Parameters();

			Text.TabList tabs = this.textContext.TabList;

			items1.Generator = generator1;
			items1.TabItem   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.5, null, TabPositionMode.LeftRelative,       TabList.PackToAttribute ("Em:1"));
			items1.TabBody   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.LeftRelativeIndent, TabList.PackToAttribute ("Em:2"));
			items1.Font      = new Text.Properties.FontProperty ("Arial", "Regular");
			
			items2.Generator = generator2;
			items2.TabItem   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 1.0, null, TabPositionMode.Force,       TabList.PackToAttribute ("LevelMultiplier:150 %", "Em:1.5"));
			items2.TabBody   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.ForceIndent, TabList.PackToAttribute ("LevelMultiplier:150 %", "Em:2"));
			
			items3.Generator = generator3;
			items3.TabItem   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Force,       TabList.PackToAttribute ("LevelMultiplier:100 %", "Em:0.5"));
			items3.TabBody   = tabs.NewTab(Text.TabList.GenericSharedName, 0.0, Text.Properties.SizeUnits.Points, 0.0, null, TabPositionMode.ForceIndent, TabList.PackToAttribute ("LevelMultiplier:150 %", "Em:2"));
			
			items4.Generator = generator4;
			
			Text.Properties.ManagedParagraphProperty itemList1 = new Text.Properties.ManagedParagraphProperty("ItemList", items1.Save());
			Text.Properties.ManagedParagraphProperty itemList2 = new Text.Properties.ManagedParagraphProperty("ItemList", items2.Save());
			Text.Properties.ManagedParagraphProperty itemList3 = new Text.Properties.ManagedParagraphProperty("ItemList", items3.Save());
			Text.Properties.ManagedParagraphProperty itemList4 = new Text.Properties.ManagedParagraphProperty("ItemList", items4.Save());
			
			Text.Properties.ManagedInfoProperty contInfo = new Epsitec.Common.Text.Properties.ManagedInfoProperty("ItemList", "cont");
			
			properties.Clear ();
			properties.Add(new Text.Properties.FontProperty("Times New Roman", "Italic", "kern", "liga"));
			properties.Add(new Text.Properties.FontSizeProperty(fontSize*Modifier.FontSizeScale*1.5, Text.Properties.SizeUnits.Points));
			properties.Add(new Text.Properties.KeepProperty(1, 1, Text.Properties.ParagraphStartMode.Anywhere, Text.Properties.ThreeState.False, Text.Properties.ThreeState.True));
			properties.Add(new Text.Properties.MarginsProperty(0, 0, 0, 0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True, 0, Text.TabList.PackToAttribute("LevelMultiplier:0 mm")));
			
			Text.TextStyle title = this.textContext.StyleList.NewTextStyle(null, "Title", Text.TextStyleClass.Paragraph, properties, baseStyles);
			
			Text.Properties.MarginsProperty ml = new Text.Properties.MarginsProperty(2*fontSize*Modifier.FontSizeScale, 2*fontSize*Modifier.FontSizeScale, 0.0, 0.0, Text.Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Text.Properties.ThreeState.True, 0, Text.TabList.PackToAttribute("LevelMultiplier:150 %"));
			
			Text.Properties.MarginsProperty mt1 = new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined, 0, Text.TabList.PackToAttribute("LevelMultiplier:0 mm"));
			Text.Properties.MarginsProperty mt2 = new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined, 1, Text.TabList.PackToAttribute("LevelMultiplier:0 mm"));
			Text.Properties.MarginsProperty mt3 = new Text.Properties.MarginsProperty(double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.SizeUnits.None, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, Text.Properties.ThreeState.Undefined, 2, Text.TabList.PackToAttribute("LevelMultiplier:0 mm"));
			
			Text.TextStyle l1 = this.textContext.StyleList.NewTextStyle(null, "BulletRound",   Text.TextStyleClass.Paragraph, new Text.Property[] { itemList1 }, baseStyles);
			Text.TextStyle l2 = this.textContext.StyleList.NewTextStyle(null, "BulletNumeric", Text.TextStyleClass.Paragraph, new Text.Property[] { itemList2, ml }, baseStyles);
			Text.TextStyle l3 = this.textContext.StyleList.NewTextStyle(null, "BulletAlpha",   Text.TextStyleClass.Paragraph, new Text.Property[] { itemList3, ml }, baseStyles);
			
			Text.TextStyle t1 = this.textContext.StyleList.NewTextStyle(null, "TitleNum1",  Text.TextStyleClass.Paragraph, new Text.Property[] { itemList4, mt1, contInfo }, new Text.TextStyle[] { title } );
			Text.TextStyle t2 = this.textContext.StyleList.NewTextStyle(null, "TitleNum2",  Text.TextStyleClass.Paragraph, new Text.Property[] { itemList4, mt2, contInfo }, new Text.TextStyle[] { title } );
			Text.TextStyle t3 = this.textContext.StyleList.NewTextStyle(null, "TitleNum3",  Text.TextStyleClass.Paragraph, new Text.Property[] { itemList4, mt3, contInfo }, new Text.TextStyle[] { title } );
			
			int rank = 1;
			
			this.textContext.StyleList.SetNextStyle(null, title, paraStyle);
			this.textContext.StyleList.StyleMap.SetRank(null, title, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, title, "P.Titre");
			
			this.textContext.StyleList.SetNextStyle(null, t1, paraStyle);
			this.textContext.StyleList.StyleMap.SetRank(null, t1, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, t1, "P.Titre 1");
			
			this.textContext.StyleList.SetNextStyle(null, t2, paraStyle);
			this.textContext.StyleList.StyleMap.SetRank(null, t2, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, t2, "P.Titre 2");
			
			this.textContext.StyleList.SetNextStyle(null, t3, paraStyle);
			this.textContext.StyleList.StyleMap.SetRank(null, t3, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, t3, "P.Titre 3");
			
			this.textContext.StyleList.StyleMap.SetRank(null, l1, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, l1, "P.Liste � puces");

			this.textContext.StyleList.StyleMap.SetRank(null, l2, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, l2, "P.Liste 1./2./...");

			this.textContext.StyleList.StyleMap.SetRank(null, l3, rank++);
			this.textContext.StyleList.StyleMap.SetCaption(null, l3, "P.Liste a)/b)/...");
#endif	
			#endregion
			
			this.textContext.DefaultParagraphStyle = paraStyle;
			this.textContext.StyleList.StyleMap.SetRank(null, paraStyle, 0);
			this.textContext.StyleList.StyleMap.SetCaption(null, paraStyle, Res.Strings.Style.Paragraph.Base);

			this.textContext.DefaultTextStyle = charStyle;
			this.textContext.StyleList.StyleMap.SetRank(null, charStyle, 0);
			this.textContext.StyleList.StyleMap.SetCaption(null, charStyle, Res.Strings.Style.Character.Base);

			this.textContext.StyleList.StyleRedefined += new Support.EventHandler(this.HandleStyleListStyleRedefined);
		}

		
		private void HandleStyleListStyleRedefined(object sender)
		{
			//	Appel� quand un TextStyle est modifi� dans StyleList.
			this.textContext.StyleList.UpdateTextStyles();
		}
		
		public Text.TextStyle[] TextStyles(StyleCategory category)
		{
			//	Liste des styles de paragraphe ou de caract�re de ce document.
			System.Diagnostics.Debug.Assert(category == StyleCategory.Paragraph || category == StyleCategory.Character);
			Text.TextStyle[] list = this.TextContext.StyleList.StyleMap.GetSortedStyles();
			int total = 0;
			foreach ( Text.TextStyle style in list )
			{
				if ( category == StyleCategory.Paragraph && style.TextStyleClass == Text.TextStyleClass.Paragraph )  total ++;
				if ( category == StyleCategory.Character && style.TextStyleClass == Text.TextStyleClass.Text      )  total ++;
			}

			Text.TextStyle[] extract = new Text.TextStyle[total];
			int i = 0;
			foreach ( Text.TextStyle style in list )
			{
				if ( category == StyleCategory.Paragraph && style.TextStyleClass == Text.TextStyleClass.Paragraph )  extract[i++] = style;
				if ( category == StyleCategory.Character && style.TextStyleClass == Text.TextStyleClass.Text      )  extract[i++] = style;
			}

			this.SetSelectedTextStyle(category, System.Math.Min(this.GetSelectedTextStyle(category), extract.Length-1));

			return extract;
		}

		public int GetSelectedTextStyle(StyleCategory category)
		{
			//	Donne le style de paragraphe ou de caract�re s�lectionn�.
			System.Diagnostics.Debug.Assert(category == StyleCategory.Paragraph || category == StyleCategory.Character);
			if ( category == StyleCategory.Paragraph )  return this.selectedParagraphStyle;
			if ( category == StyleCategory.Character )  return this.selectedCharacterStyle;
			return -1;
		}

		public void SetSelectedTextStyle(StyleCategory category, int rank)
		{
			//	Modifie le style de paragraphe ou de caract�re s�lectionn�.
			System.Diagnostics.Debug.Assert(category == StyleCategory.Paragraph || category == StyleCategory.Character);
			
			if ( this.modifier.OpletQueue.IsActionDefinitionInProgress )
			{
				this.modifier.OpletQueue.Insert(new SetSelectedTextStyleOplet(this, category));
			}
			
			if ( category == StyleCategory.Paragraph )  this.selectedParagraphStyle = rank;
			if ( category == StyleCategory.Character )  this.selectedCharacterStyle = rank;
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
			//	Retourne le prochain identificateur unique pour les noms d'agr�gats.
			return ++this.uniqueAggregateId;
		}

		public int GetNextUniqueParagraphStyleId()
		{
			//	Retourne le prochain identificateur unique pour les noms de style de paragraphe.
			return ++this.uniqueParagraphStyleId;
		}

		public int GetNextUniqueCharacterStyleId()
		{
			//	Retourne le prochain identificateur unique pour les noms de style de caract�re.
			return ++this.uniqueCharacterStyleId;
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

		//	Enum�rateur permettant de parcourir � plat l'arbre des objets.
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
				//	Impl�mentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Impl�mentation de IEnumerator:
				this.index = -1;
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets s�lectionn�s ?
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

		//	Enum�rateur permettant de parcourir � plat depuis la fin l'arbre des objets.
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
				//	Impl�mentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Impl�mentation de IEnumerator:
				this.index = this.list.Count;
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets s�lectionn�s ?
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

		//	Enum�rateur permettant de parcourir en profondeur l'arbre des objets.
		//	En mode onlySelected, seuls les objets s�lectionn�s du premier niveau
		//	sont concern�s. Un objet fils d'un objet s�lectionn� du premier niveau
		//	est toujours consid�r� comme s�lectionn�, bien qu'il ne le soit pas
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
				//	Impl�mentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Impl�mentation de IEnumerator:
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
					if ( this.onlySelected )  // seulement les objets s�lectionn�s ?
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
				if ( this.onlySelected )  // seulement les objets s�lectionn�s ?
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

		//	Enum�rateur permettant de parcourir en profondeur l'arbre des objets.
		//	L'objet rendu est de type DeepBranchEntry. Ceci permet de savoir si
		//	l'on est ou non � l'int�rieur d'une branche quelconque.
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
				//	Impl�mentation de IEnumerable:
				return this;
			}

			public void Reset()
			{
				//	Impl�mentation de IEnumerator:
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
			//	Retourne une ressource string d'apr�s son nom.
			return Res.Strings.GetString(name);
		}
		#endregion

		#region SetSelectedTextStyleOplet Class
		private class SetSelectedTextStyleOplet : Common.Support.AbstractOplet
		{
			public SetSelectedTextStyleOplet(Document document, StyleCategory category)
			{
				this.document = document;
				this.category = category;
				
				switch ( this.category )
				{
					case StyleCategory.Paragraph:
						this.rank = this.document.selectedParagraphStyle;
						break;
					case StyleCategory.Character:
						this.rank = this.document.selectedCharacterStyle;
						break;
				}
			}
			

			public override Common.Support.IOplet Undo()
			{
				int oldRank = -1;
				int newRank = this.rank;
				
				switch ( this.category )
				{
					case StyleCategory.Paragraph:
						oldRank = this.document.selectedParagraphStyle;
						this.document.selectedParagraphStyle = newRank;
						break;
					
					case StyleCategory.Character:
						oldRank = this.document.selectedCharacterStyle;
						this.document.selectedCharacterStyle = newRank;
						break;
				}
				
				this.rank = oldRank;
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			
			private Document					document;
			private StyleCategory				category;
			private int							rank;
		}
		#endregion

		#region UniqueID
		protected void UniqueIDCreate()
		{
			//	Assigne un num�ro unique � ce document.
			this.uniqueID = Document.uniqueIDGenerator++;
		}

		public string UniqueName
		{
			//	Retourne un nom unique pour ce document.
			get
			{
				return string.Concat("Document-", this.uniqueID.ToString(System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		protected static int					uniqueIDGenerator = 0;
		protected int							uniqueID;
		#endregion


		protected DocumentType					type;
		protected DocumentMode					mode;
		protected InstallType					installType;
		protected DebugMode						debugMode;
		protected FontIncludeMode				fontIncludeMode;
		protected ImageIncludeMode				imageIncludeMode;
		protected bool							initializationInProgress;
		protected Settings.GlobalSettings		globalSettings;
		protected CommandDispatcher				commandDispatcher;
		protected CommandContext				commandContext;
		protected string						name;
		protected Document						clipboard;
		protected Document						documentForSamples;
		protected Objects.TextBox2				objectForSamplesParagraph;
		protected Objects.TextLine2				objectForSamplesCharacter;
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
		protected ImageCache					imageCache;
		protected Wrappers						wrappers;
		protected Notifier						notifier;
		protected Printer						printer;
		protected Common.Dialogs.Print			printDialog;
		protected PDF.Export					exportPDF;
		protected Dialogs						dialogs;
		protected string						ioDirectory;
		protected System.Collections.ArrayList	readWarnings;
		protected List<OpenType.FontName>		fontList;
		protected IOType						ioType;
		protected Objects.Memory				readObjectMemory;
		protected Objects.Memory				readObjectMemoryText;
		protected System.Collections.ArrayList	readRootStack;
		protected bool							isSurfaceRotation;
		protected double						surfaceRotationAngle;
		protected int							uniqueObjectId = 0;
		protected int							uniqueAggregateId = 0;
		protected int							uniqueParagraphStyleId = 0;
		protected int							uniqueCharacterStyleId = 0;
		protected int							selectedParagraphStyle = 0;
		protected int							selectedCharacterStyle = 0;
		protected Text.TextContext				textContext;
		protected Widgets.HRuler				hRuler;
		protected Widgets.VRuler				vRuler;
		protected bool							containOldText;
	}
}
