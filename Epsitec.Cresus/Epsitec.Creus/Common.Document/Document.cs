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
		Pictogram,		// icône
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
		Expired,		// version pleine échue
		Freeware,		// version freeware
	}

	public enum DebugMode
	{
		Release,		// pas de debug
		DebugCommands,	// toutes les commandes de debug
	}

	public enum StyleCategory
	{
		//	Attention: les membres de cette énumération sont utilisés comme index
		//	dans certaines tables; il ne faut donc pas changer leur numérotation.
		//	cf. Containers.Styles.SelectorName, par exemple.
		
		None		= -1,
		
		Graphic		= 0,	// style graphique
		Paragraph	= 1,	// style de paragraphe
		Character	= 2,	// style de caractère
		
		Count				// nombre d'éléments dans l'énumération (3)
	}

	public enum CacheBitmapChanging
	{
		None,
		Local,
		All,
	}


	/// <summary>
	/// Summary description for Document.
	/// </summary>
	[System.Serializable()]
	public class Document : ISerializable
	{
		public enum IOType
		{
			Unknown,			// format inconnu
			BinaryCompress,		// format standard
			SoapUncompress,		// format de debug
		}

		public enum FontIncludeMode
		{
			//	Mode d'inclusion des polices dans le document crdoc/zip.
			//	Ne pas changer les valeurs à cause des sérialisations existantes !

			None	= 0,	// n'inclut aucune police
			Used	= 1,	// inclut les polices utilisées
			All		= 2,	// inclut les polices utilisées plus toutes les polices définies
		}

		public enum ImageIncludeMode
		{
			//	Mode d'inclusion des images dans le document crdoc/zip.
			//	Ne pas changer les valeurs à cause des sérialisations existantes !

			None	= 0,	// n'inclut aucune image
			Defined	= 1,	// inclut les images selons leurs définitions
			All		= 2,	// inclut toutes les images définies
		}

		public enum DocumentFileExtension
		{
			Unknown,		// extension inconnue
			CrDoc,			// extension *.crdoc
			CrMod,			// extension *.crmod
			Icon,			// extension *.icon
			IconMod,		// extension *.iconmod
		}

		public Document(DocumentType type, DocumentMode mode, InstallType installType, DebugMode debugMode, Settings.GlobalSettings globalSettings, CommandDispatcher commandDispatcher, CommandContext commandContext, Window mainWindow)
		{
			//	Crée un nouveau document vide.
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
			this.mainWindow = mainWindow;
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

			this.printDialog = new Common.Dialogs.PrinterDocumentPropertiesDialog();

			if ( this.mode == DocumentMode.Modify    ||
				 this.mode == DocumentMode.Clipboard )
			{
				this.modifier  = new Modifier(this);
				this.wrappers  = new Wrappers(this);
				this.notifier  = new Notifier(this);
				this.dialogs   = new DocumentDialogs(this);
				this.settings  = new Settings.Settings(this);
				this.printer   = new Printer(this);
				this.exportPdf = new PDF.Export(this);
			}

			if ( this.mode == DocumentMode.Samples )
			{
				this.modifier  = new Modifier(this);
				this.wrappers  = new Wrappers(this);
				this.notifier  = new Notifier(this);
			}

			//	Il ne faut pas créer de ImageCache lorsqu'on crée un document
			//	juste pour une icône !
			if ( this.mode == DocumentMode.Modify )
			{
				this.imageCache = new ImageCache(this);
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
			if (this.imageCache != null)
			{
				this.imageCache.Dispose ();
				this.imageCache = null;
			}

			if (this.ioDocumentManager != null)
			{
				this.ioDocumentManager.Dispose ();
				this.ioDocumentManager = null;
			}

			if (this.modifier != null)
			{
				this.modifier.Dispose();
				this.modifier = null;
			}

			if (this.wrappers != null)
			{
				this.wrappers.Dispose();
				this.wrappers = null;
			}

			if (this.notifier != null)
			{
				this.notifier.Dispose();
				this.notifier = null;
			}

			if (this.dialogs != null)
			{
				this.dialogs.Dispose();
				this.dialogs = null;
			}

			if (this.settings != null)
			{
				this.settings.Dispose();
				this.settings = null;
			}

			if (this.printer != null)
			{
				this.printer.Dispose();
				this.printer = null;
			}

			if (this.exportPdf != null)
			{
				this.exportPdf.Dispose();
				this.exportPdf = null;
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

		public PDF.Export GetExportPdf()
		{
			return this.exportPdf;
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
			//	Réglages globaux.
			get { return this.globalSettings; }
		}
		
		public CommandDispatcher CommandDispatcher
		{
			//	CommandDispatcher de l'éditeur.
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
			//	Bloc-notes associé.
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

		public Containers.PageMiniatures PageMiniatures
		{
			//	Gestionnaire des pages miniatures associé.
			get { return this.pageMiniatures; }
			set { this.pageMiniatures = value; }
		}

		public Containers.LayerMiniatures LayerMiniatures
		{
			//	Gestionnaire des calques miniatures associé.
			get { return this.layerMiniatures; }
			set { this.layerMiniatures = value; }
		}

		protected void MainWindowSetFrozen()
		{
			//	Gèle la fenêtre principale.
			if (this.mainWindow != null)
			{
				this.isMainWindowFrozen = this.mainWindow.Root.IsFrozen;
				this.mainWindow.Root.SetFrozen(true);
			}
		}

		protected void MainWindowClearFrozen()
		{
			//	Dégèle la fenêtre principale.
			if (this.mainWindow != null)
			{
				this.mainWindow.Root.SetFrozen(this.isMainWindowFrozen);
			}
		}

		public void Close()
		{
			//	Appelé juste avant le fermeture du document.
			if (this.modifier != null)
			{
				this.modifier.MiniaturesTimerStop();
			}
		}

		#region ForSamples
		public Document DocumentForSamples
		{
			//	Donne le document spécial servant à dessiner les échantillons.
			get
			{
				if ( this.documentForSamples == null )
				{
					this.documentForSamples = new Document(DocumentType.Graphic, DocumentMode.Samples, InstallType.Full, DebugMode.Release, this.GlobalSettings, null, null, null);
					this.documentForSamples.TextContext = this.TextContext;
				}

				return this.documentForSamples;
			}
		}

		public Objects.TextBox2 ObjectForSamplesParagraph
		{
			//	Donne l'objet TextBox2 servant à dessiner les échantillons des styles de paragraphe.
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
			//	Donne l'objet TextLine2 servant à dessiner les échantillons des styles de caractère.
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
			CommandState state = context.GetCommandState (Command.Get (command));

			return state;
		}
		
		public UndoableList DocumentObjects
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
			get
			{
				return this.modifier;
			}
		}

		public ImageCache ImageCache
		{
			//	Cache des images de ce document.
			get
			{
				if (this.imageCache == null)
				{
					this.imageCache = new ImageCache(this);
				}
				
				return this.imageCache;
			}
		}

		public DocumentManager DocumentManager
		{
			//	Retourne le gestionnaire de document associé; il est créé s'il n'existait
			//	pas avant.
			get
			{
				return this.ioDocumentManager;
			}
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

		public DocumentDialogs Dialogs
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
		
		public Common.Dialogs.PrintDialog PrintDialog
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
						this.SetDirtySerialize(CacheBitmapChanging.All);
						this.modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
						this.notifier.NotifyAllChanged();
						this.modifier.OpletQueueValidateAction();

						this.AdjustOutsideArea();
					}
					else
					{
						this.size = value;
						this.SetDirtySerialize(CacheBitmapChanging.All);
					}
				}
			}
		}

		public Canvas.IconKey[] IconKeys
		{
			//	Donne les clés pour les icônes de toutes les pages.
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
				page = this.DocumentObjects[pageNumber] as Objects.Page;
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
					this.SetDirtySerialize(CacheBitmapChanging.All);
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
		}

		public void ClearDirtySerialize()
		{
			//	Considère le document comme propre, c'est-à-dire à jour.
			if (this.isDirtySerialize)
			{
				this.isDirtySerialize = false;

				if (this.Notifier != null)
				{
					this.Notifier.NotifySaveChanged();
				}
			}
		}

		public void SetDirtySerialize(CacheBitmapChanging changing)
		{
			//	Considère le document comme sale, c'est-à-dire devant être mis à jour.
			if (!this.isDirtySerialize)
			{
				this.isDirtySerialize = true;

				if (this.Notifier != null)
				{
					this.Notifier.NotifySaveChanged();
				}
			}

			foreach (TextFlow flow in this.textFlows)
			{
				flow.NotifyAboutToExecuteCommand();
			}

			this.SetDirtyCacheBitmap(changing);
		}

		protected void SetDirtyCacheBitmap(CacheBitmapChanging changing)
		{
			//	Parcourt toutes les pages/calques du document pour invalider les images des miniatures
			//	du cache bitmap.
			//	CacheBitmapChanging.None	->	n'invalide rien
			//	CacheBitmapChanging.Local	->	invalide la page et le calque courant
			//	CacheBitmapChanging.All		->	invalide toutes les pages et tous les calques
			if (changing == CacheBitmapChanging.None || this.pageMiniatures == null || this.layerMiniatures == null)
			{
				return;
			}

			int currentPage = this.Modifier.ActiveViewer.DrawingContext.CurrentPage;
			int currentLayer = this.Modifier.ActiveViewer.DrawingContext.CurrentLayer;

			//	Si on effectue un changement local dans une page modèle, il faut invalider toutes les miniatures.
			if (changing == CacheBitmapChanging.Local)
			{
				Objects.Page page = this.DocumentObjects[currentPage] as Objects.Page;
				if (page.MasterType != Objects.MasterType.Slave)  // page modèle ?
				{
					changing = CacheBitmapChanging.All;
				}
			}

			this.modifier.MiniaturesTimerStop();

			if (changing == CacheBitmapChanging.All)
			{
				this.pageMiniatures.RegenerateAll();
				this.layerMiniatures.RegenerateAll();
			}

			if (changing == CacheBitmapChanging.Local)
			{
				this.pageMiniatures.AddPageToRegenerate(currentPage);
				this.layerMiniatures.AddLayerToRegenerate(currentLayer);
			}

			this.modifier.MiniaturesTimerStart(false);
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
				this.ioDocumentManager = new DocumentManager();
				this.ioDocumentManager.Open(filename);

				Stream sourceStream = this.ioDocumentManager.GetLocalFileStream(FileAccess.Read);

				string err = "";
				ZipFile zip = new ZipFile();
				zip.LoadFileName = this.ioDocumentManager.GetLocalFilePath();
				DocumentFileExtension ext = Document.GetDocumentFileExtension(filename);
				bool isCrDoc = (ext == DocumentFileExtension.CrDoc);
				bool isCrMod = (ext == DocumentFileExtension.CrMod);
				this.type = (isCrDoc || isCrMod) ? DocumentType.Graphic : DocumentType.Pictogram;

				if (ext != DocumentFileExtension.Icon &&
					zip.TryLoadFile(sourceStream, delegate(string entryName) { return entryName == "document.data" || entryName.StartsWith("fonts/"); }))
				{
					// Fichier CrDoc au format ZIP, chargé avec succès.
					using (MemoryStream stream = new MemoryStream(zip["document.data"].Data))
					{
						err = this.Read(stream, System.IO.Path.GetDirectoryName(filename), zip, isCrDoc);
					}
				}
				else
				{
					// Désérialisation standard; c'est un ancien fichier CrDoc.
					err = this.Read(sourceStream, System.IO.Path.GetDirectoryName(filename), null, isCrDoc);
				}

				sourceStream.Close();

				if (err == "")
				{
					if (ext == DocumentFileExtension.CrDoc ||
						ext == DocumentFileExtension.Icon)
					{
						this.Filename = filename;
						this.globalSettings.LastFilenameAdd(filename);
						this.ClearDirtySerialize();
					}
					if (ext == DocumentFileExtension.CrMod ||
						ext == DocumentFileExtension.IconMod)
					{
						this.globalSettings.LastModelAdd(filename);
					}
				}
				else
				{
					this.ClearDirtySerialize();
				}
				return err;
			}
			catch ( System.Exception e )
			{
				this.ioDocumentManager.Close();
				this.ClearDirtySerialize();
				return e.Message;
			}
		}

		public string Read(Stream stream, string directory)
		{
			//	Ouvre un document sérialisé, soit parce que l'utilisateur veut ouvrir
			//	explicitement un fichier, soit par Engine.
			return this.Read(stream, directory, null, false);
		}

		private GenericDeserializationBinder GetVersionDeserializationBinder()
		{
			return new GenericDeserializationBinder (
				(assemblyName, typeName) =>
				{
					//	Retourne un type correspondant à l'application courante, afin
					//	d'accepter de désérialiser un fichier généré par une application
					//	ayant un autre numéro de révision.
					//	Application courante: Version=1.0.1777.18519
					//	Version dans le fichier: Version=1.0.1777.11504
					if (typeName == "Epsitec.Common.Document.Document")
					{
						int i, j;
						string v;

						i = assemblyName.IndexOf ("Version=");
						if (i >= 0)
						{
							i += 8;
							j = assemblyName.IndexOf (".", i);
							v = assemblyName.Substring (i, j-i);
							long r1 = System.Int64.Parse (v, System.Globalization.CultureInfo.InvariantCulture);

							i = j+1;
							j = assemblyName.IndexOf (".", i);
							v = assemblyName.Substring (i, j-i);
							long r2 = System.Int64.Parse (v, System.Globalization.CultureInfo.InvariantCulture);

							i = j+1;
							j = assemblyName.IndexOf (".", i);
							v = assemblyName.Substring (i, j-i);
							long r3 = System.Int64.Parse (v, System.Globalization.CultureInfo.InvariantCulture);

							Document.ReadRevision = (r1<<32) + (r2<<16) + r3;
						}
					}

					return null;
				}
				);
		}

		private string Read(Stream stream, string directory, ZipFile zip, bool isCrDoc)
		{
			//	Ouvre un document sérialisé, zippé ou non.
			this.ioDirectory = directory;
			this.readWarnings = new System.Collections.ArrayList();

			IOType type = Document.ReadIdentifier(stream);
			if ( type == IOType.Unknown )
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
				formatter.Binder = this.GetVersionDeserializationBinder ();

				try
				{
					if (zip == null)
					{
						string compressorName;
						using (Stream compressor = IO.Decompression.CreateStream(stream, out compressorName))
						{
							doc = (Document) formatter.Deserialize(compressor);
							doc.ioDocumentManager = this.ioDocumentManager;
							//-doc.imageCache = new ImageCache();
							if (isCrDoc)
							{
								doc.ImageCacheReadAll(zip, doc.imageIncludeMode);
							}
						}
					}
					else
					{
						doc = (Document) formatter.Deserialize(stream);
						doc.ioDocumentManager = this.ioDocumentManager;
						doc.FontReadAll(zip);
						doc.ImageCacheReadAll(zip, doc.imageIncludeMode);
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
				formatter.Binder = this.GetVersionDeserializationBinder ();

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
				doc.textContext.StyleList.StyleRedefined -= doc.HandleStyleListStyleRedefined;

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

			if ((this.imageCache != null) &&
				(doc.imageCache != null))
			{
				this.imageCache.Document = null;	//	évite de vider le cache global qui vient d'être rempli
				this.imageCache.Dispose ();
				this.imageCache = null;
			}

			if (doc.imageCache != null)
			{
				this.imageCache = doc.imageCache;
			}

			if (this.imageCache != null)
			{
				this.imageCache.Document = this;
			}

			if (this.ioDocumentManager == doc.ioDocumentManager)
			{
				doc.ioDocumentManager = null;
			}
			
			this.fontIncludeMode = doc.fontIncludeMode;
			this.imageIncludeMode = doc.imageIncludeMode;
			
			if ( this.textContext != null )
			{
				this.textContext.StyleList.StyleRedefined += this.HandleStyleListStyleRedefined;

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
					int pageNumber = this.Modifier.PrintablePageRank(0);  // numéro de la première page non modèle du document
					this.Modifier.ActiveViewer.DrawingContext.InternalPageLayer(pageNumber, 0);
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

		
		//	Utilisé par les constructeurs de désérialisation du genre:
		//	protected Toto(SerializationInfo info, StreamingContext context)
		public static Document ReadDocument = null;
		public static long ReadRevision = 0;

		private void ReadFinalize()
		{
			//	Adapte tous les objets après une désérialisation.
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
					//	y en a quand-même certains qui seraient utilisables.
					
					Text.TextStyle paraStyle = this.textContext.StyleList["Default", Text.TextStyleClass.Paragraph];
					Text.TextStyle charStyle = this.textContext.StyleList["Default", Text.TextStyleClass.Text];
					
					if ( paraStyle == null )
					{
						paraStyle = this.textContext.DefaultParagraphStyle;
					}
					
					//	Tous les documents qui contenaient du texte ont toujours au moins
					//	défini un style nommé 'Default'. Si ça ne devait pas être le cas,
					//	crash garanti.
					
					if ( paraStyle == null )  throw new System.ArgumentNullException("No default paragraph style found");
					
					this.textContext.DefaultParagraphStyle = paraStyle;
					this.textContext.StyleList.StyleMap.SetRank(null, paraStyle, 0);
					this.textContext.StyleList.StyleMap.SetCaption(null, paraStyle, Res.Strings.Style.Paragraph.Base);

					//	Cherche un style de caractère; si aucun n'est trouvé, on en crée
					//	un nouveau de toutes pièces (c'est facile). Toute une série de
					//	documents créés avec des versions de décembre 2005 n'avaient pas
					//	de style de caractère par défaut.
					
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
			DocumentFileExtension ext = Document.GetDocumentFileExtension(filename);
			
			try
			{
				this.Modifier.DeselectAll();

				this.ioDirectory = System.IO.Path.GetDirectoryName(filename);

				if (this.ioDocumentManager == null)
				{
					this.ioDocumentManager = new DocumentManager();
				}

				if (this.type == DocumentType.Pictogram)
				{
					string err = this.PictogramCheckBeforeWrite();
					if (err != "")
					{
						return err;
					}

					this.ioDocumentManager.Save(filename,
						delegate (System.IO.Stream stream)
						{
							Document.WriteIdentifier(stream, this.ioType);

							if (this.ioType == IOType.BinaryCompress)
							{
								//?Stream compressor = IO.Compression.CreateBZip2Stream(stream);
								Stream compressor = IO.Compression.CreateDeflateStream(stream, 1);
								BinaryFormatter formatter = new BinaryFormatter();
								formatter.Serialize(compressor, this);
								compressor.Close();
								return true;
							}
							else if (this.ioType == IOType.SoapUncompress)
							{
								SoapFormatter formatter = new SoapFormatter();
								formatter.Serialize(stream, this);
								return true;
							}
							else
							{
								return false;
							}
						});
				}
				else
				{
					this.FontUpdate();
					this.ImageFlushUnused();
					if (this.imageCache != null)
					{
						this.imageCache.GenerateShortNames();
					}
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
					zip.AddEntry("document.data", data, 0);
					this.WriteMiniature(zip, 1, ext == DocumentFileExtension.CrMod);
					this.WriteStatistics(zip, 2);
					if (this.imageCache != null)
					{
						this.imageCache.WriteData(zip, this.imageIncludeMode);
					}
					this.FontWriteAll(zip);
					zip.CompressionLevel = 6;

					this.ioDocumentManager.Save(filename,
						delegate (System.IO.Stream stream)
						{
							zip.SaveFile(stream);
							return true;
						});
				}
			}
			catch (System.Exception e)
			{
				return e.Message;
			}
			finally
			{
				while (undoCount < this.modifier.OpletQueue.UndoActionCount)
				{
					this.modifier.OpletQueue.UndoAction();
				}
				
				this.modifier.OpletQueue.PurgeRedo();
			}

			if (ext == DocumentFileExtension.CrDoc ||
				ext == DocumentFileExtension.Icon)
			{
				this.Filename = filename;
				this.ClearDirtySerialize();
			}
			DocumentCache.Remove(filename);
			return "";
		}

		private string PictogramCheckBeforeWrite()
		{
			foreach (Objects.Abstract obj in this.Deep(null))
			{
				if (obj is Objects.TextBox  ||
					obj is Objects.TextBox2 ||
					obj is Objects.TextLine ||
					obj is Objects.TextLine2)
				{
					return Res.Strings.Error.ExistingText;
				}
			}

			return "";  // ok
		}

		private static IOType ReadIdentifier(Stream stream)
		{
			//	Lit les 8 bytes d'en-tête et vérifie qu'ils contiennent bien "<?icon?>".
			byte[] buffer = new byte[8];
			Common.IO.ReaderHelper.Read(stream, buffer, 0, 8);
			if ( buffer[0] != (byte) '<' )  return IOType.Unknown;
			if ( buffer[1] != (byte) '?' )  return IOType.Unknown;
			if ( buffer[2] != (byte) 'i' )  return IOType.Unknown;
			if ( buffer[3] != (byte) 'c' )  return IOType.Unknown;
			if ( buffer[4] != (byte) 'o' )  return IOType.Unknown;
			if ( buffer[6] != (byte) '?' )  return IOType.Unknown;
			if ( buffer[7] != (byte) '>' )  return IOType.Unknown;

			if ( buffer[5] == (byte) 'n' )  return IOType.BinaryCompress;
			if ( buffer[5] == (byte) 'x' )  return IOType.SoapUncompress;
			return IOType.Unknown;
		}

		private static void WriteIdentifier(Stream stream, IOType type)
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

		public static DocumentFileExtension GetDocumentFileExtension(string path)
		{
			if ( Misc.IsExtension(path, ".crdoc") )  return DocumentFileExtension.CrDoc;
			if ( Misc.IsExtension(path, ".crmod") )  return DocumentFileExtension.CrMod;
			if ( Misc.IsExtension(path, ".icon" ) )  return DocumentFileExtension.Icon;
			if ( Misc.IsExtension(path, ".iconmod") )return DocumentFileExtension.IconMod;

			return DocumentFileExtension.Unknown;
		}

		public static string OriginalSamplesDisplayName
		{
			//	Retourne le nom à afficher pour le dossier contenant les exemples originaux.
			get
			{
				return Res.Strings.Directory.OriginalSamples;
			}
		}

		public static string MySamplesDisplayName
		{
			//	Retourne le nom à afficher pour le dossier contenant les exemples personnels.
			get
			{
				return Res.Strings.Directory.MySamples;
			}
		}

		public static string OriginalSamplesPath
		{
			//	Retourne le nom du dossier contenant les exemples originaux.
			get
			{
				return string.Concat(Common.Support.Globals.Directories.Executable, "\\", Document.OriginalSamplesDisplayName);
			}
		}

		public static string MySamplesPath
		{
			//	Retourne le nom du dossier contenant les exemples personnels.
			get
			{
#if false
				//	Attention, on reçoit:
				//	C:\Documents and Settings\Daniel Roux\Application Data\Epsitec\Crésus Documents\2.0.2.0
				//	'Crésus Documents' au lieu de 'Crésus documents' qui est le vrai nom !
				string path = Common.Support.Globals.Directories.UserAppData;
				int i = path.LastIndexOf("\\");
				if (i > 0)
				{
					path = path.Substring(0, i);  // supprime le dossier "1.0.0.0" à la fin
				}
				return string.Concat(path, "\\", Document.DisplayMySamples);
#else
				FolderItem item = FileManager.GetFolderItem(FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
				string path = item.FullPath;
				return string.Concat(path, "\\", Res.Strings.Directory.MyDocumentsRoot, "\\", Document.MySamplesDisplayName);
#endif
			}
		}

		public static bool RedirectPath(ref string path)
		{
			//	Redirige un nom de dossier de 'Exemples originaux' vers 'Mes exemples', si nécessaire.
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			string directory;
			string file;
			
			if (System.IO.Directory.Exists (path))
			{
				directory = path;
				file      = "";
			}
			else
			{
				directory = System.IO.Path.GetDirectoryName (path);
				file      = System.IO.Path.GetFileName (path);
			}

			if (string.Equals (directory, Document.OriginalSamplesPath, System.StringComparison.OrdinalIgnoreCase))
			{
				directory = Document.MySamplesPath;
				path      = System.IO.Path.Combine (directory, file);
				
				return true;
			}
			else
			{
				return false;
			}
		}

		
		#region Serialization
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le document.
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

				info.AddValue("RootStack", this.modifier.ActiveViewer.DrawingContext.GetRootStack());

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
			//	Constructeur qui désérialise le document.
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

		#region Miniature
		protected void WriteMiniature(ZipFile zip, int priority, bool isModel)
		{
			//	Ecrit la miniature de la première page dans le fichier zip.
			string filename;
			byte[] data;
			if (this.printer.Miniature(new Size(100, 100), isModel, out filename, out data))
			{
				zip.AddEntry (filename, data, System.DateTime.Now, priority, false);
			}
		}
		#endregion


		#region DocumentInfo
		public static IDocumentInfo GetDocumentInfo(string path)
		{
			//	Extrait des informations pour le document spécifié.

			byte[] dataImage;
			byte[] dataDocInfo;

			Document.ReadDocumentData (path, out dataImage, out dataDocInfo);

			if (dataImage != null || dataDocInfo != null)
			{
				Image image = null;
				Statistics stat = new Statistics ();

				if (dataImage != null)
				{
					try
					{
						Image loaded = Bitmap.FromData (dataImage);
						image = loaded;
					}
					catch
					{
					}
				}

				if (dataDocInfo != null)
				{
					try
					{
						Statistics loaded = Serialization.DeserializeFromMemory (dataDocInfo) as Document.Statistics;
						stat = loaded;
					}
					catch
					{
					}
				}

				stat.SetThumbnail (image);

				return stat;
			}
			else
			{
				return null;
			}
		}

		private static void ReadDocumentData(string path, out byte[] dataImage, out byte[] dataDocInfo)
		{
			//	Lit les données (miniature et statistique) associées au fichier.
			ZipFile zip = new ZipFile ();

			dataImage = null;
			dataDocInfo = null;

			bool ok = zip.TryLoadFile (path,
				delegate (string entryName)
				{
					return (entryName == "preview.png")
						|| (entryName == "statistics.data");
				});

			if (ok)
			{
				try
				{
					dataImage = zip["preview.png"].Data;  // lit les données dans le fichier zip
				}
				catch
				{
					dataImage = null;
				}

				try
				{
					dataDocInfo = zip["statistics.data"].Data;  // lit les données dans le fichier zip
				}
				catch
				{
					dataDocInfo = null;
				}
			}
		}
		#endregion

		#region Statistics
		[System.Serializable]
		public class Statistics : DocumentInfo
		{
			public Statistics()
			{
			}

			public void SetThumbnail(Image thumbnail)
			{
				this.thumbnail = thumbnail;
			}

			protected Statistics(SerializationInfo info, StreamingContext context)
				: base (info, context)
			{
			}

			public override string GetDescription()
			{
				return this.ToString ();
			}

			public override Image GetThumbnail()
			{
				return this.thumbnail;
			}

			public override string ToString()
			{
				string format = Res.Strings.Dialog.File.Statistics;
				return string.Format (format, this.PageFormat, this.PageCount, this.LayerCount, this.ObjectCount, this.ComplexObjectCount, this.FontCount, this.ImageCount);
			}

			private Image thumbnail;
		}

		protected void WriteStatistics(ZipFile zip, int priority)
		{
			//	Ecrit la description du document dans le fichier zip.
			Statistics stat = new Statistics();
			stat.PageSize = this.PageSize;

			string format = DocumentDialogs.PaperFormat(stat.PageSize);
			if (format == null)
			{
				stat.PageFormat = string.Format("{0} × {1}", this.modifier.RealToString(stat.PageSize.Width), this.modifier.RealToString(stat.PageSize.Height));
			}
			else
			{
				stat.PageFormat = format;
			}

			stat.PageCount = this.modifier.StatisticTotalPages();
			stat.LayerCount = this.modifier.StatisticTotalLayers();
			stat.ObjectCount = this.modifier.StatisticTotalObjects();
			stat.ComplexObjectCount = this.modifier.StatisticTotalComplex();
			stat.DefineDocumentVersion (typeof (Document).Assembly);

			List<OpenType.FontName> fontList = new List<OpenType.FontName>();
			TextFlow.StatisticFonts (fontList, this.TextFlows, TextStats.FontNaming.Invariant);
			this.modifier.StatisticFonts(fontList);
			stat.FontCount = fontList.Count;

			System.Collections.ArrayList imageList = this.modifier.StatisticImages();
			stat.ImageCount = imageList.Count;

			byte[] data = Serialization.SerializeToMemory(stat);
			zip.AddEntry ("statistics.data", data, priority);
		}
		#endregion


		#region Fonts
		protected void FontUpdate()
		{
			//	Met à jour la liste de toutes les polices utilisées dans le document.
			this.fontList = new List<OpenType.FontName>();

			TextFlow.StatisticFonts (this.fontList, this.textFlows, TextStats.FontNaming.Invariant);
			this.modifier.StatisticFonts(this.fontList);

			if (this.fontIncludeMode == FontIncludeMode.All)
			{
				//	Fouille tous les styles à la recherche des polices définies.
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
			//	Ecrit sur disque tous les fichiers des polices utilisées dans le document.
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
				
				zip.AddEntry(name, font.FontData.Data.Array, true);
			}
		}

		protected void FontReadAll(ZipFile zip)
		{
			//	Lit sur disque tous les fichiers des polices utilisées dans le document.
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
				byte[] data = zip[name].Data;  // lit les données dans le fichier zip
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
			//	Retourne le nom de fichier à utiliser pour une police donnée.
			//	Un nom très simple 'n.font' est tout à fait suffisant.
			return string.Format("fonts/{0}.font", rank.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
		#endregion

		#region Images
		public IEnumerable<Properties.Image> ImageSearchInPage(int pageRank)
		{
			//	Cherche tous les noms de fichier des images de la page.
			Objects.Page page = this.objects[pageRank] as Objects.Page;
			foreach (Objects.Abstract obj in this.Deep(page))
			{
				if (obj is Objects.Image)
				{
					yield return obj.PropertyImage;
				}
			}
		}

		public void ImageLockInPage(int pageRank)
		{
			//	Verrouille les images de la page en cours, en déverrouillant
			//	toutes les autres.
			if (this.imageCache != null)
			{
				this.imageCache.UnlockAll ();
				foreach (Properties.Image image in this.ImageSearchInPage (pageRank))
				{
					this.imageCache.Lock (image.FileName, image.FileDate);
				}
			}
		}

		protected void ImageFlushUnused()
		{
			//	Supprime toutes les images inutilisées du cache des images.
			if (this.imageCache != null)
			{
				this.imageCache.ResetUsedFlags ();

				foreach (Objects.Abstract obj in this.Deep (null))
				{
					Properties.Image propImage = obj.PropertyImage;
					if (propImage != null)
					{
						this.imageCache.SetUsedFlag (propImage.FileName, propImage.FileDate);
					}
				}

				this.imageCache.FlushUnused ();
			}
		}

		protected void ImageUpdate()
		{
			//	Met à jour les informations ShortName et InsideDoc.
			//	ShortName est mis à jour dans les propriétés des objets Image du document.
			//	InsideDoc est mis à jour dans le cache des images.
			if (this.imageCache != null)
			{
				this.imageCache.ClearEmbeddedInDocument ();

				foreach (Objects.Abstract obj in this.Deep (null))
				{
					Properties.Image propImage = obj.PropertyImage;
					if (propImage != null)
					{
						this.imageCache.SyncImageProperty (propImage);
					}
				}
			}
		}

		protected void ImageCacheReadAll(ZipFile zip, ImageIncludeMode imageIncludeMode)
		{
			//	Cache toutes les données pour les objets Images du document.
			this.imageCache = new ImageCache(this);

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


		#region AdjustOutsideArea
		protected void AdjustOutsideArea()
		{
			//	Ajuste la marge autour de la page physique pour que tous les objets du document
			//	puisse être vus. Cette marge est éventuellement augmentée, mais jamais réduite.
			Rectangle bbox = this.GetGlobalObjectsBoundingBox();
			if (bbox.IsEmpty)
			{
				return;
			}

			Rectangle page = new Rectangle(Point.Zero, this.DocumentSize);
			page.Inflate(this.modifier.OutsideArea);
			if (page.Contains(bbox))  // zone hors page assez grande ?
			{
				return;
			}

			page = new Rectangle(Point.Zero, this.DocumentSize);
			double left   = -System.Math.Min(page.Left, bbox.Left);
			double right  =  System.Math.Max(page.Right,  bbox.Right ) - page.Right;
			double bottom = -System.Math.Min(page.Bottom, bbox.Bottom);
			double top    =  System.Math.Max(page.Top,    bbox.Top   ) - page.Top;

			double m = System.Math.Max(System.Math.Max(left, right), System.Math.Max(bottom, top));
			m = System.Math.Ceiling(m/10)*10;
			this.modifier.OutsideArea = m;
		}

		protected Rectangle GetGlobalObjectsBoundingBox()
		{
			//	Retourne la bbox qui englobe tous les objets du document.
			Rectangle bbox = Rectangle.Empty;

			foreach (Objects.Page page in this.objects)
			{
				foreach (Objects.Layer layer in page.Objects)
				{
					foreach (Objects.Abstract obj in this.Deep(layer))
					{
						bbox = Rectangle.Union(bbox, obj.BoundingBoxGeom);
					}
				}
			}

			return bbox;
		}
		#endregion

		public void Paint(Graphics graphics, DrawingContext drawingContext, Rectangle clipRect)
		{
			//	Dessine le document.
			if ( drawingContext.RootStackIsEmpty )  return;

			if (this.imageCache != null)
			{
				this.imageCache.SetResolution (ImageCacheResolution.Low);
			}

			if ( !clipRect.IsInfinite )
			{
				clipRect = drawingContext.Viewer.ScreenToInternal(clipRect);
			}

			Objects.Abstract page = drawingContext.RootObject(1);

			if ( drawingContext.MasterPageList.Count > 0 )
			{
				foreach ( Objects.Page masterPage in drawingContext.MasterPageList )
				{
					int frontier = masterPage.MasterFirstFrontLayer;
					this.PaintPage(graphics, drawingContext, clipRect, masterPage, 0, frontier-1);
				}
			}

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

		protected void PaintPage(Graphics graphics, DrawingContext drawingContext, Rectangle clipRect, Objects.Abstract page, int firstLayer, int lastLayer)
		{
			if ( drawingContext.PreviewActive )
			{
				Objects.Page p = page as Objects.Page;
				Rectangle initialClip = Rectangle.Empty;
				if (this.Modifier != null && !drawingContext.Viewer.IsMiniature)
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

				if (this.Modifier != null && !drawingContext.Viewer.IsMiniature)
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
							if ( drawingContext.LayerDrawingMode != LayerDrawingMode.ShowInactive )
							{
								drawingContext.IsDimmed = true;
							}
						}

						obj.DrawGeometry(graphics, drawingContext);
					}

					graphics.PopColorModifier();
					graphics.PopColorModifier();
				}
			}
		}

		public void Print(Common.Dialogs.PrintDialog dp)
		{
			//	Imprime le document.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			if (this.imageCache != null)
			{
				this.imageCache.SetResolution (ImageCacheResolution.High);
			}
			this.printer.Print(dp);
		}

		public string Export(string filename)
		{
			//	Exporte le document.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);

			//	MainWindowSetFrozen évite des appels à ImageCache.SetResolution pendant l'exportation,
			//	si la fenêtre doit être repeinte !
			this.MainWindowSetFrozen();

			if (this.imageCache != null)
			{
				this.imageCache.SetResolution(ImageCacheResolution.High);
			}
			string err = this.printer.Export(filename);

			this.MainWindowClearFrozen();
			return err;
		}

		public string ExportPdf(string filename, Common.Dialogs.IWorkInProgressReport report)
		{
			//	Exporte le document.

			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);

			//	Exécute sur le processus principal (celui qui a accès à la fenêtre d'application)
			this.mainWindow.Invoke (this.BeforeExportPdf);

			if (this.imageCache != null)
			{
				this.imageCache.SetResolution (ImageCacheResolution.High);
			}
			
			string err = this.exportPdf.ExportToFile(filename, report);
			
			//	Libérer toute la mémoire accumulée pendant l'exportation PDF est une bonne idée, car
			//	cela peut occuper pas loing d'un GB de RAM...
			GlobalImageCache.FreeEverything ();


			//	Exécute sur le processus principal (celui qui a accès à la fenêtre d'application)
			this.mainWindow.Invoke (this.AfterExportPdf);
			
			return err;
		}

		private void BeforeExportPdf()
		{
			System.Diagnostics.Debug.Assert (Application.IsRunningOnMainUIThread);

			this.Modifier.DeselectAll ();

			//	MainWindowSetFrozen évite des appels à ImageCache.SetResolution pendant l'exportation,
			//	si la fenêtre doit être repeinte !
			
			this.MainWindowSetFrozen ();
		}

		private void AfterExportPdf()
		{
			System.Diagnostics.Debug.Assert (Application.IsRunningOnMainUIThread);

			this.MainWindowClearFrozen ();
		}

		public string ExportICO(string filename)
		{
			//	Exporte le document.
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);

			//	MainWindowSetFrozen évite des appels à ImageCache.SetResolution pendant l'exportation,
			//	si la fenêtre doit être repeinte !
			this.MainWindowSetFrozen();

			if (this.imageCache != null)
			{
				this.imageCache.SetResolution(ImageCacheResolution.High);
			}
			string err = this.printer.ExportICO(filename);

			this.MainWindowClearFrozen();
			return err;
		}


		#region TextContext
		protected void CreateDefaultTextContext()
		{
			//	Crée le TextContext et les styles par défaut.
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
			this.textContext.StyleList.StyleMap.SetCaption(null, l1, "P.Liste à puces");

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

			this.textContext.StyleList.StyleRedefined += this.HandleStyleListStyleRedefined;
		}

		
		private void HandleStyleListStyleRedefined(object sender)
		{
			//	Appelé quand un TextStyle est modifié dans StyleList.
			this.textContext.StyleList.UpdateTextStyles();
		}
		
		public Text.TextStyle[] TextStyles(StyleCategory category)
		{
			//	Liste des styles de paragraphe ou de caractère de ce document.
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
			//	Donne le style de paragraphe ou de caractère sélectionné.
			System.Diagnostics.Debug.Assert(category == StyleCategory.Paragraph || category == StyleCategory.Character);
			if ( category == StyleCategory.Paragraph )  return this.selectedParagraphStyle;
			if ( category == StyleCategory.Character )  return this.selectedCharacterStyle;
			return -1;
		}

		public void SetSelectedTextStyle(StyleCategory category, int rank)
		{
			//	Modifie le style de paragraphe ou de caractère sélectionné.
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
			//	Retourne le prochain identificateur unique pour les noms d'agrégats.
			return ++this.uniqueAggregateId;
		}

		public int GetNextUniqueParagraphStyleId()
		{
			//	Retourne le prochain identificateur unique pour les noms de style de paragraphe.
			return ++this.uniqueParagraphStyleId;
		}

		public int GetNextUniqueCharacterStyleId()
		{
			//	Retourne le prochain identificateur unique pour les noms de style de caractère.
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
					this.list = this.document.DocumentObjects;
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
					this.list = this.document.DocumentObjects;
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

				UndoableList list = this.document.DocumentObjects;
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

				UndoableList list = this.document.DocumentObjects;
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
			return Res.Strings.GetString (name);
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
			//	Assigne un numéro unique à ce document.
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
		protected Window						mainWindow;
		protected bool							isMainWindowFrozen;
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
		protected Common.Dialogs.PrintDialog	printDialog;
		protected PDF.Export					exportPdf;
		protected DocumentDialogs				dialogs;
		protected string						ioDirectory;
		protected System.Collections.ArrayList	readWarnings;
		protected List<OpenType.FontName>		fontList;
		protected IOType						ioType;
		protected DocumentManager				ioDocumentManager;
		protected Objects.Memory				readObjectMemory;
		protected Objects.Memory				readObjectMemoryText;
		protected System.Collections.ArrayList	readRootStack;
		protected bool							isSurfaceRotation;
		protected double						surfaceRotationAngle;
		protected int							uniqueObjectId;
		protected int							uniqueAggregateId;
		protected int							uniqueParagraphStyleId;
		protected int							uniqueCharacterStyleId;
		protected int							selectedParagraphStyle;
		protected int							selectedCharacterStyle;
		protected Text.TextContext				textContext;
		protected Widgets.HRuler				hRuler;
		protected Widgets.VRuler				vRuler;
		protected bool							containOldText;
		protected Containers.PageMiniatures		pageMiniatures;
		protected Containers.LayerMiniatures	layerMiniatures;
	}
}
