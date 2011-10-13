//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;
using Epsitec.Cresus.Graph.ImportConverters;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Graph
{
	public partial class GraphApplication : Application
	{
		public GraphApplication()
		{
			Services.SetApplication (this);

			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");

			this.graphCommands = new GraphCommands (this);
			this.persistenceManager = new PersistenceManager ();
			
			this.mainWindowController = new Controllers.MainWindowController (this);
			this.workspaceController = new Controllers.WorkspaceController (this);

			GraphActions.DocumentReload = Actions.Factory.New (this.DocumentReload);
			GraphActions.DocumentSelectDataSource = Actions.Factory.New (this.DocumentSelectDataSource);
			GraphActions.DocumentIncludeFilterCategory = Actions.Factory.New (this.DocumentIncludeFilterCategory);
			GraphActions.DocumentExcludeFilterCategory = Actions.Factory.New (this.DocumentExcludeFilterCategory);
			GraphActions.DocumentAddSeriesToOutput = Actions.Factory.New (this.DocumentAddSeriesToOutput);
			GraphActions.DocumentRemoveSeriesFromOutput = Actions.Factory.New (this.DocumentRemoveSeriesFromOutput);
			GraphActions.DocumentSetSeriesOutputIndex = Actions.Factory.New<string, int> (this.DocumentSetSeriesOutputIndex);
			GraphActions.DocumentHideSnapshot = Actions.Factory.New (this.DocumentHideSnapshot);
			GraphActions.DocumentDefineColor = Actions.Factory.New<int, string> (this.DocumentDefineColor);

//			this.loadDataSetAction = Actions.Factory.New (this.LoadDataSet);
//			this.removeSeriesFromGraphAction = Actions.Factory.New (this.RemoveFromChart);

			Clipboard.DataChanged += this.HandleClipboardDataChanged;
		}

		
		public bool IsReady
		{
			get;
			private set;
		}

		
		public override string ShortWindowTitle
		{
			get
			{
				string version = VersionChecker.Format ("#.#.###", GraphUpdate.GetInstalledVersion ());
				string mode    = GraphSerial.LicensingFriendlyName;
				return string.Format (Res.Strings.Application.Name.ToSimpleText (), mode, version);
			}
		}

		public override string ApplicationIdentifier
		{
			get
			{
				return "CresusGrapheMainEp";
			}
		}


		public GraphDocument Document
		{
			get
			{
				return this.activeDocument;
			}
		}

		public IEnumerable<GraphDocument> OpenDocuments
		{
			get
			{
				if (this.activeDocument != null)
				{
					yield return this.activeDocument;
				}
			}
		}

		public VersionChecker VersionChecker
		{
			get
			{
				return this.versionChecker ?? VersionChecker.Default;
			}
		}


		private GraphDataCube ImportCube(string text, string sourcePath, IDictionary<string, string> meta)
		{
			string[] lines = text.Split (new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
			char[] columnSeparators = new char[] { '\t', ';' };

			foreach (var columnSeparator in columnSeparators)
			{
				var graphDataCube = this.TryImportCube (lines, columnSeparator, sourcePath, meta);

				if (graphDataCube != null)
				{
					return graphDataCube;
				}
			}

			return null;
		}
		
		internal Controllers.MainWindowController MainWindowController
		{
			get
			{
				return this.mainWindowController;
			}
		}

		internal Controllers.WorkspaceController WorkspaceController
		{
			get
			{
				return this.workspaceController;
			}
		}


		internal void SetupUI()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			
			Font.RegisterResourceFont (assembly, "Epsitec.Cresus.Graph.Resources.calibri.ttf");
			Font.RegisterResourceFont (assembly, "Epsitec.Cresus.Graph.Resources.futuramc.ttf");

			this.mainWindowController.SetupUI ();
			this.workspaceController.SetupUI ();

//-			System.Diagnostics.Debugger.Launch ();

			this.Window = this.mainWindowController.Window;

			this.SetEnable (ApplicationCommands.Save, false);
			this.SetEnable (ApplicationCommands.SaveAs, false);
			
			this.RestoreApplicationState ();
			this.IsReady = true;
		}

		internal void SetupDefaultDocument()
		{
			if (this.activeDocument == null)
			{
				this.CreateDocument ();
			}
		}

		internal void SetupConnectorServer()
		{
			this.connectorServer = new ConnectorServer (false);
			this.connectorServer.Open (this.ProcessConnectorData);
		}

		internal void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (() => this.SaveApplicationState (false));
		}

		internal void RegisterDocument(GraphDocument document)
		{
			System.Diagnostics.Debug.Assert (this.activeDocument == null);

			this.SetActiveDocument (document);
		}

		internal void SetActiveDocument(GraphDocument document)
		{
			if (this.activeDocument != document)
			{
				this.activeDocument = document;
				this.NotifyDocumentChanged ();
			}
		}

		internal void ExecutePaste()
		{
			if (this.lastPasteTextData == null)
			{
				//	Pasting for the first time and cut/copy was done before the application
				//	started, fetch the data now from the clipboard (if any) :

				if (!this.ReadClipboardTextData (Clipboard.GetData ()))
				{
					return;
				}
			}

			this.Document.ImportCube (
				this.ImportCube (this.lastPasteTextData, null, null),
				GraphDataCube.LoadPathClipboard, System.Text.Encoding.UTF8);
		}

		internal void ExecuteImport(string path, System.Text.Encoding encoding)
		{
			this.Document.ImportCube (
				this.ImportCube (System.IO.File.ReadAllText (path, encoding), path, null),
				path, encoding);
		}

		internal void ExecuteLoadCube(string path)
		{
			var cube = GraphDocument.LoadCubeData (path);
			
			this.Document.LoadCube (cube);
			this.Document.RefreshDataSet ();
		}

		internal void ProcessCommandLine()
		{
			var args = Application.GetCommandLineArguments ();

			for (int i = 0; i < args.Count; i++)
			{
				var arg  = Utilities.StringSimplify (args[i]);
				int pos = arg.IndexOf ('=');
				var verb = pos < 0 ? arg : arg.Substring (0, pos);
				var param = pos < 0 ? "" : arg.Substring (pos+1);

				switch (verb)
				{
					case "-loadcube":
						Application.QueueAsyncCallback (() => this.ExecuteLoadCube (param));
						break;

					case "-open":
						try
						{
							this.activeDocument.LoadDocument (param);
						}
						catch
						{
						}
						break;

					case "-connector":
						if (param == "SendData")
						{
//							System.Diagnostics.Debugger.Break ();
							this.SetupConnectorServer ();
						}
						break;
				}
			}
		}

		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var dirtyDocs = from doc in this.OpenDocuments
							where doc.IsDirty
							select doc;

			foreach (var doc in dirtyDocs)
			{
				var dialog = new Dialogs.QuestionDialog (Res.Captions.Message.Quit.Question,
					Res.Captions.Message.Quit.Option1Save,
					Res.Captions.Message.Quit.Option2DoNotSave,
					Res.Captions.Message.Quit.Option3Cancel)
				{
					OwnerWindow = this.Window
				};

				dialog.OpenDialog ();

				switch (dialog.Result)
				{
					case DialogResult.Answer1:
						if (!this.graphCommands.Save (doc, false))
						{
							e.Executed = true;
							return;
						}
						break;
					
					case DialogResult.Answer2:
						break;

					case DialogResult.Answer3:
						e.Executed = true;
						return;
				}
			}
			
			this.SaveApplicationState (true);
			base.ExecuteQuit (dispatcher, e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.connectorServer != null)
				{
					this.connectorServer.Dispose ();
					this.connectorServer = null;
				}
			}
			
			base.Dispose (disposing);
		}

		private void RestoreApplicationState()
		{
			if (System.IO.File.Exists (GraphApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (GraphApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

				var version = (string) store.Attribute ("version");
				var status  = (string) store.Attribute ("status");

				if (version == "1")
				{
//-					this.stateManager.RestoreStates (store.Element ("stateManager"));

					if (Application.GetCommandLineArguments ().Count == 0)
					{
						if (status != "done")
						{
							this.RestoreDocumentSettings (store.Element ("documents"));
						}
					}
					
					Services.RestoreWindowPositions (store.Element ("windowPositions"));
					this.persistenceManager.Restore (store.Element ("uiSettings"));
				}
			}

			this.persistenceManager.DiscardChanges ();
			this.persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();
		}
		
		private void SaveApplicationState(bool saveStatus)
		{
			if (this.IsReady)
			{
				System.DateTime now = System.DateTime.Now.ToUniversalTime ();
				string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

				XDocument doc = new XDocument (
					new XDeclaration ("1.0", "utf-8", "yes"),
					new XComment ("Saved on " + timeStamp),
					new XElement ("store",
						new XAttribute ("version", "1"),
						new XAttribute ("status", saveStatus ? "done" : "open"),
//						this.StateManager.SaveStates ("stateManager"),
						this.SaveOpenDocumentSettings ("documents"),
						Services.SaveWindowPositions ("windowPositions"),
						this.persistenceManager.Save ("uiSettings")));

				doc.Save (GraphApplication.Paths.SettingsPath);
			}
		}

		private XElement SaveOpenDocumentSettings(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
					from doc in this.OpenDocuments
					select doc.SaveSettings (new XElement ("doc")));
		}

		private void RestoreDocumentSettings(XElement xml)
		{
			if (xml == null)
			{
				return;
			}

			foreach (XElement node in xml.Elements ())
			{
				System.Diagnostics.Debug.Assert (node.Name == "doc");

				var doc = this.CreateDocument ();
				doc.RestoreSettings (node);
			}
		}

		private GraphDocument CreateDocument()
		{
			var doc = new GraphDocument (this)
			{
			};

			return doc;
		}

		private GraphDataCube TryImportCube(IEnumerable<string> lines, char columnSeparator, string sourcePath, IDictionary<string, string> meta)
		{
			try
			{
				var headColumns = ImportConverter.QuotedSplit (lines.First (), columnSeparator);

				AbstractImportConverter converter;
				GraphDataCube           cube;

				var lineColumns = lines.Skip (1).Select (x => ImportConverter.QuotedSplit (x, columnSeparator));

				if (ImportConverter.ConvertToCube (headColumns, lineColumns, sourcePath, meta, out converter, out cube))
				{
					string path = System.IO.Path.GetTempFileName ();

					using (var stream = new System.IO.StreamWriter (path, false, System.Text.Encoding.UTF8))
					{
						cube.Save (stream);
					}

					var dimensionNames = cube.NaturalTableDimensionNames;

					cube.SliceDimA = dimensionNames[0];
					cube.SliceDimB = dimensionNames[1];
					cube.ConverterName = converter.Name;
					cube.ConverterMeta = converter.FlatMeta;
					cube.Title = converter.DataTitle;
					
					return cube;
				}
			}
			catch
			{
			}

			return null;
		}

		internal void NotifyDocumentChanged()
		{
			var handler = this.ActiveDocumentChanged;

			if (handler != null)
			{
				Application.QueueAsyncCallback (() => handler (this));
			}
		}

		internal void NotifyNewerVersion(VersionChecker versionChecker)
		{
			this.versionChecker = versionChecker;

			if (versionChecker.FoundNewerVersion)
			{
				this.SetEnable (Res.Commands.General.DownloadUpdate, true);
			}
			else
			{
				this.SetEnable (Res.Commands.General.DownloadUpdate, false);
				this.Window.Root.FindCommandWidget (Res.Commands.General.DownloadUpdate).Visibility = false;
			}
		}


		private void NotifyClipboardDataChanged()
		{
			var handler = this.ClipboardDataChanged;

			if (handler != null)
			{
				handler (this, new ClipboardDataEventArgs (this.lastPasteTextData));
			}
		}

		private void HandleClipboardDataChanged(object sender, ClipboardDataChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (string.Join ("/", e.Data.NativeFormats));
			System.Diagnostics.Debug.WriteLine (string.Join ("/", e.Data.AllPossibleFormats));

			var clipboard = e.Data;

			if (this.ReadClipboardTextData (clipboard))
			{
				this.NotifyClipboardDataChanged ();
			}
		}


		private bool ProcessConnectorData(ConnectorData connectorData)
		{
			var doc = this.Document;

			if ((this.lastConnectorData == null) ||
				(this.lastConnectorData.Checksum != connectorData.Checksum) ||
				(this.lastConnectorData.Ticks < connectorData.Ticks + GraphApplication.PasteTickCountTimeout))
			{
				this.lastConnectorData = connectorData;

				string tempPath = System.IO.Path.GetTempFileName ();
				System.IO.File.WriteAllText (tempPath, connectorData.Data);

				System.Diagnostics.Debug.WriteLine ("Saved connector data to " + tempPath);

				Application.QueueAsyncCallback (() => doc.ImportCube (this.ImportCube (connectorData.Data, connectorData.Path, connectorData.Meta), connectorData.Path, null));

				return true;
			}
			else
			{
				return false;
			}
		}


		private bool ReadClipboardTextData(ClipboardReadData clipboard)
		{
			if (!clipboard.IsCompatible (ClipboardDataFormat.Text))
			{
				return false;
			}

			return this.ProcessClipboardText(clipboard.ReadText ());
		}

		private bool ProcessClipboardText(string textData)
		{
			if (string.IsNullOrEmpty (textData))
			{
				return false;
			}

			var checksum  = string.IsNullOrEmpty (textData) ? 0 : Epsitec.Common.IO.Checksum.ComputeAdler32 (x => x.UpdateValue (textData));
			var tickCount = System.Environment.TickCount;
			var timeout   = this.lastPasteTickCount + GraphApplication.PasteTickCountTimeout;

			this.lastPasteTickCount = tickCount;

			if ((checksum == this.lastPasteChecksum) &&
				(tickCount < timeout))
			{
				return false;
			}

			this.lastPasteChecksum  = checksum;
			this.lastPasteTextData  = textData;

			return true;
		}


		private void DocumentSelectDataSource(string name)
		{
			this.Document.SelectDataSource (name);
		}

		public void DocumentReload()
		{
			this.Document.ResetDefaultColors ();
			this.Document.ClearData ();
			this.Document.ReloadDataSet ();

			//	Make all snapshots visible again; this is useful when actions get run again
			//	in order to execute an undo/redo operation; snapshots will be hidden by the
			//	'HideSnapshot' actions :

			//	TODO: do this only while rewinding the actions, not when executing other
			//	"reload" actions in the history...

			this.Document.ChartSnapshots.ForEach (x => x.Visibility = true);
		}

		private void DocumentIncludeFilterCategory(string name)
		{
			this.Document.IncludeFilterCategory (name);
		}

		private void DocumentExcludeFilterCategory(string name)
		{
			this.Document.ExcludeFilterCategory (name);
		}

		private void DocumentAddSeriesToOutput(string id)
		{
			this.Document.AddOutput (this.Document.FindSeries (id));
			this.Document.RefreshUI ();
		}

		private void DocumentRemoveSeriesFromOutput(string id)
		{
			this.Document.RemoveOutput (this.Document.FindSeries (id));
			this.Document.RefreshUI ();
		}

		private void DocumentSetSeriesOutputIndex(string id, int index)
		{
			this.Document.SetOutputIndex (this.Document.FindSeries (id), index);
			this.Document.RefreshUI ();
		}

		private void DocumentDefineColor(int index, string color)
		{
			this.Document.DefaultColorStyle.DefineColor (index, Color.Parse (color));
			this.Document.RefreshUI ();
		}

		private void DocumentHideSnapshot(string guid)
		{
			var snapshot = this.Document.ChartSnapshots.Find (x => x.GuidName == guid);
			
			if (snapshot != null)
			{
				snapshot.Visibility = false;
				this.Document.RefreshUI ();
			}
		}
		
		public event EventHandler ActiveDocumentChanged;
		public event EventHandler<ClipboardDataEventArgs> ClipboardDataChanged;


		
		private readonly GraphCommands graphCommands;
		
		private readonly Controllers.MainWindowController mainWindowController;
		private readonly Controllers.WorkspaceController workspaceController;

		private readonly PersistenceManager		persistenceManager;

		private const int PasteTickCountTimeout = 5000;	//	[ms]
		
		private GraphDocument					activeDocument;
		private long							lastPasteChecksum;
		private string							lastPasteTextData;
		private int								lastPasteTickCount;
		private ConnectorServer					connectorServer;
		private ConnectorData					lastConnectorData;
		private VersionChecker					versionChecker;
	}
}
