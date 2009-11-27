﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Cresus.Graph.ImportConverters;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.UI;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Dialogs;
using System.Collections;

namespace Epsitec.Cresus.Graph
{
	public partial class GraphApplication : Application
	{
		public GraphApplication()
		{
			this.graphCommands = new GraphCommands (this);
			this.persistenceManager = new Core.UI.PersistenceManager ();
			
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
				return Res.Strings.Application.Name.ToSimpleText ();
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
				return this.versionChecker;
			}
		}


		private GraphDataCube ImportCube(string text, string sourcePath)
		{
			string[] lines = text.Split (new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
			char[] columnSeparators = new char[] { '\t', ';' };

			foreach (var columnSeparator in columnSeparators)
			{
				var graphDataCube = this.TryImportCube (lines, columnSeparator, sourcePath);

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
			using (var stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.Cresus.Graph.Resources.futuramc.ttf"))
			{
				Font.RegisterDynamicFont (stream);
			}

			this.mainWindowController.SetupUI ();
			this.workspaceController.SetupUI ();

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
			this.connectorServer.Open (this.ProcessSendData);
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
				this.ImportCube (this.lastPasteTextData, null),
				GraphDataCube.LoadPathClipboard, System.Text.Encoding.UTF8);
		}

		internal void ExecuteImport(string path, System.Text.Encoding encoding)
		{
			this.Document.ImportCube (
				this.ImportCube (System.IO.File.ReadAllText (path, encoding), path),
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
				string arg  = args[i];
				string verb = arg.Split ('=')[0];

				switch (verb)
				{
					case "-loadcube":
						if (i+1 < args.Count)
                        {
							string path = Utilities.StringSimplify (args[++i]);
							Application.QueueAsyncCallback (() => this.ExecuteLoadCube (path));
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
					
					Core.UI.RestoreWindowPositions (store.Element ("windowPositions"));
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
						Core.UI.SaveWindowPositions ("windowPositions"),
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

			var args = System.Environment.GetCommandLineArgs ();

#if false
			if (!System.Diagnostics.Debugger.IsAttached)
            {
				System.Diagnostics.Debugger.Launch ();
			}
#endif

			if (args.Length == 3)
            {
				string verb = args[1];
				string path = args[2];

				if (verb == "-open")
				{
					try
					{
						var doc = this.CreateDocument ();
						doc.LoadDocument (path);
						return;
					}
					catch
					{
						System.Environment.Exit (1);
					}
				}
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

		private GraphDataCube TryImportCube(IEnumerable<string> lines, char columnSeparator, string sourcePath)
		{
			try
			{
				var headColumns = ImportConverter.QuotedSplit (lines.First (), columnSeparator);

				AbstractImportConverter converter;
				GraphDataCube           cube;

				var lineColumns = lines.Skip (1).Select (x => ImportConverter.QuotedSplit (x, columnSeparator));

				if (ImportConverter.ConvertToCube (headColumns, lineColumns, sourcePath, out converter, out cube))
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


		private bool ProcessSendData(System.IntPtr handle, string path, string meta, string data)
		{
			var doc = this.Document;

			if (this.ProcessClipboardText (data))
			{
				Application.QueueAsyncCallback (() => doc.ImportCube (this.ImportCube (data, path), path, null));
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

		private readonly Core.UI.PersistenceManager persistenceManager;

		private const int PasteTickCountTimeout = 5000;	//	[ms]
		
		private GraphDocument activeDocument;
		private long lastPasteChecksum;
		private string lastPasteTextData;
		private int lastPasteTickCount;
		private ConnectorServer connectorServer;
		private VersionChecker versionChecker;
	}
}
