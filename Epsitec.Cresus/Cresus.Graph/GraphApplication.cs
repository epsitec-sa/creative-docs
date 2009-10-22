//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		internal void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
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
				this.OnActiveDocumentChanged ();
			}
		}


		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var dirtyDocs = from doc in this.OpenDocuments
							where doc.IsDirty
							select doc;

			foreach (var doc in dirtyDocs)
			{
				var header = "<font size=\"120%\">Ce document contient des modifications.</font><br/>Que faut-il faire avant de quitter ?";
				var questions = new string[]
				{
					"<font size=\"120%\">Enregistrer ce document</font><br/>Vos modifications seront enregistrées avant de quitter.",
					"<font size=\"120%\">Ne pas enregistrer ce document</font><br/>Vos modifications ne seront pas enregistrées et elles seront perdues.",
				};
				var dialog = new ConfirmationDialog (this.ShortWindowTitle, header, questions, true);
				
				dialog.OwnerWindow = this.Window;
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

					case DialogResult.Cancel:
						e.Executed = true;
						return;
                }
			}
			
			this.SaveApplicationState ();
			base.ExecuteQuit (dispatcher, e);
		}

		private void RestoreApplicationState()
		{
			if (System.IO.File.Exists (GraphApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (GraphApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

				var version = (string) store.Attribute ("version");

				if (version == "1")
				{
//-					this.stateManager.RestoreStates (store.Element ("stateManager"));
					this.RestoreDocumentSettings (store.Element ("documents"));
					Core.UI.RestoreWindowPositions (store.Element ("windowPositions"));
					this.persistenceManager.Restore (store.Element ("uiSettings"));
				}
			}

			this.persistenceManager.DiscardChanges ();
			this.persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();
		}
		
		private void SaveApplicationState()
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

#if false
		private void SumRows(IEnumerable<int> rows)
		{
			var sum = this.activeDocument.DataSet.DataTable.SumRows (rows, this.seriesPickerController.GetRowSeries);

			if (sum == null)
			{
				return;
			}

			this.activeDocument.DataSet.DataTable.RemoveRows (rows);
			this.activeDocument.DataSet.DataTable.Insert (rows.First (), sum.Label.Replace ("+-", "-"), sum.Values);

			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();
		}

		private void NegateRows(IEnumerable<int> rows)
		{
			foreach (int row in rows)
			{
				var series = this.activeDocument.DataSet.DataTable.GetRowSeries (row);
				this.seriesPickerController.NegateSeries (series.Label);
			}

			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();
		}

		private void AddToChart(IEnumerable<int> rows)
		{
			var table = this.activeDocument.DataSet.DataTable;

			foreach (int row in rows)
			{
				this.activeDocument.Add (table.GetRowSeries (row));
			}

			table.RemoveRows (rows);
			
			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();
		}

		private void RemoveFromChart(IEnumerable<int> rows)
		{
			var table = this.activeDocument.DataSet.DataTable;
			var list  = new List<ChartSeries> (rows.Select (x => this.activeDocument.Find (x)));

			list.ForEach (series => this.activeDocument.Remove (series));
			list.ForEach (series => table.Add (series.Label, series.Values));
			
			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();

			this.seriesPickerController.SetSelectedItem (table.RowCount-1);
		}

		private void LoadDataSet()
		{
			System.Diagnostics.Debug.Assert (this.activeDocument != null);

			this.activeDocument.ReloadDataSet ();
			this.seriesPickerController.ClearNegatedSeries ();

			this.OnActiveDocumentChanged ();
		}
#endif

		private void OnActiveDocumentChanged()
		{
			var handler = this.ActiveDocumentChanged;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void HandleClipboardDataChanged(object sender, ClipboardDataChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (string.Join ("/", e.Data.NativeFormats));
			System.Diagnostics.Debug.WriteLine (string.Join ("/", e.Data.AllPossibleFormats));

			var clipboard = e.Data;

			if (clipboard.IsCompatible (ClipboardDataFormat.Text))
			{
				string text = clipboard.ReadText ();
				long checksum = Epsitec.Common.IO.Checksum.ComputeAdler32 (x => x.UpdateValue (text));

				if (checksum == this.lastPasteChecksum)
				{
					return;
				}

				this.lastPasteChecksum = checksum;

				string[] lines = text.Split (new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
				string[] headColumns = lines[0].Split ('\t');

				AbstractImportConverter converter;
				DataCube                cube;

				if (ImportConverter.ConvertToCube (headColumns, lines.Skip (1).Select (x => x.Split ('\t')), out converter, out cube))
				{
					string path = System.IO.Path.GetTempFileName ();

					using (var stream = new System.IO.StreamWriter (path, false, System.Text.Encoding.UTF8))
					{
						cube.Save (stream);
					}

					var document       = this.Document;
					var dimensionNames = cube.NaturalTableDimensionNames;

					document.DefineImportConverter (converter.Name);
					document.LoadCube (cube);
					document.SelectCubeSlice (dimensionNames[0], dimensionNames[1]);
					
					document.Title = converter.DataTitle;

					this.OnActiveDocumentChanged ();
				}
			}
		}

		
		private void DocumentSelectDataSource(string name)
		{
			this.Document.SelectDataSource (name);
		}

		private void DocumentReload()
		{
			this.Document.ReloadDataSet ();
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
		
		public event EventHandler ActiveDocumentChanged;


		
		private readonly GraphCommands graphCommands;
		
		private readonly Controllers.MainWindowController mainWindowController;
		private readonly Controllers.WorkspaceController workspaceController;

		private readonly Core.UI.PersistenceManager persistenceManager;
		private readonly System.Action<IEnumerable<int>> removeSeriesFromGraphAction;

		private GraphDocument activeDocument;
		private long lastPasteChecksum;
	}
}
