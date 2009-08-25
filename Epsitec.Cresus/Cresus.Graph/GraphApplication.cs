//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
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

namespace Epsitec.Cresus.Graph
{
	public partial class GraphApplication : Application
	{
		public GraphApplication()
		{
			this.graphCommands = new GraphCommands (this);
			this.persistenceManager = new Core.UI.PersistenceManager ();
			this.documents = new List<GraphDocument> ();

			this.loadDataSetAction = Actions.Factory.New (this.LoadDataSet);
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
				return "Crésus Graphe";
			}
		}


		public GraphDocument Document
		{
			get
			{
				if (this.activeDocument == null)
				{
					new GraphDocument ();
				}
				
				return this.activeDocument;
			}
		}

		internal void SetupDefaultDocument()
		{
			if (this.activeDocument == null)
			{
				new GraphDocument ();
				this.SetupDataSet ();
			}
		}

		internal void SetupDataSet()
		{
			this.loadDataSetAction ();
		}

		internal void SetupInterface()
		{
			Window window = new Window ()
			{
				Text = this.ShortWindowTitle,
				ClientSize = new Epsitec.Common.Drawing.Size (824, 400),
				Name = "Application"
			};

			window.Root.Padding = new Margins (4, 8, 8, 4);

			FrameBox bar = new FrameBox ()
			{
				Dock = DockStyle.Top,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Margins = new Margins (0, 0, 0, 4),
				Parent = window.Root
			};

			new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Undo,
				PreferredWidth = 32,
				Embedder = bar
			};

			new MetaButton ()
			{
				ButtonClass = ButtonClass.FlatButton,
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Redo,
				PreferredWidth = 32,
				Embedder = bar
			};

			new IconButton ()
			{
				Dock = DockStyle.Stacked,
				CommandObject = ApplicationCommands.Save,
				PreferredWidth = 32,
				Embedder = bar
			};

			this.SetEnable (ApplicationCommands.Save, false);
			

			FrameBox frame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Parent = window.Root
			};

			this.seriesPickerController = new Controllers.SeriesPickerController (frame);
			this.seriesPickerController.SumSeriesAction = Actions.Factory.New (this.SumRows);
			this.seriesPickerController.NegateSeriesAction = Actions.Factory.New (this.NegateRows);
			this.seriesPickerController.AddSeriesToGraphAction = Actions.Factory.New (this.AddToChart);

			
			this.Window = window;
			this.RestoreApplicationState ();
			this.IsReady = true;
		}

		internal void AsyncSaveApplicationState()
		{
			Application.QueueAsyncCallback (this.SaveApplicationState);
		}

		internal void RegisterDocument(GraphDocument graphDocument)
		{
			System.Diagnostics.Debug.Assert (this.documents.Contains (graphDocument) == false);

			this.documents.Add (graphDocument);

			if (this.activeDocument == null)
			{
				this.SetActiveDocument (graphDocument);
			}
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
			this.SaveApplicationState ();
			base.ExecuteQuit (dispatcher, e);
		}
		
		private void RestoreApplicationState()
		{
			if (System.IO.File.Exists (GraphApplication.Paths.SettingsPath))
			{
				XDocument doc = XDocument.Load (GraphApplication.Paths.SettingsPath);
				XElement store = doc.Element ("store");

//-				this.stateManager.RestoreStates (store.Element ("stateManager"));
				this.RestoreDocumentSettings (store.Element ("documents"));
				Core.UI.RestoreWindowPositions (store.Element ("windowPositions"));
				this.persistenceManager.Restore (store.Element ("uiSettings"));
			}

			this.persistenceManager.DiscardChanges ();
			this.persistenceManager.SettingsChanged += (sender) => this.AsyncSaveApplicationState ();
		}
		
		private void SaveApplicationState()
		{
			if (this.IsReady)
			{
				System.Diagnostics.Debug.WriteLine ("Saving application state.");
				System.DateTime now = System.DateTime.Now.ToUniversalTime ();
				string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

				XDocument doc = new XDocument (
					new XDeclaration ("1.0", "utf-8", "yes"),
					new XComment ("Saved on " + timeStamp),
					new XElement ("store",
//						this.StateManager.SaveStates ("stateManager"),
						this.SaveDocumentSettings ("documents"),
						Core.UI.SaveWindowPositions ("windowPositions"),
						this.persistenceManager.Save ("uiSettings")));

				doc.Save (GraphApplication.Paths.SettingsPath);
				System.Diagnostics.Debug.WriteLine ("Save done.");
			}
		}

		private XElement SaveDocumentSettings(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
					from doc in this.documents
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

				var doc = new GraphDocument ();

				doc.RestoreSettings (node);
			}
		}

		
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

			this.activeDocument.DataSet.DataTable.RemoveRows (rows);
			
			this.seriesPickerController.UpdateScrollListItems ();
			this.seriesPickerController.UpdateChartView ();
		}

		private void LoadDataSet()
		{
			System.Diagnostics.Debug.Assert (this.activeDocument != null);
			
			this.activeDocument.DataSet.LoadDataTable ();
			this.OnActiveDocumentChanged ();
		}


		private Controllers.SeriesPickerController seriesPickerController;

		private void OnActiveDocumentChanged()
		{
			var handler = this.ActiveDocumentChanged;

			if (handler != null)
			{
				handler (this);
			}
		}
		
		
		public event EventHandler ActiveDocumentChanged;

		private readonly System.Action loadDataSetAction;

		private GraphCommands graphCommands;
		private GraphDocument activeDocument;
		private readonly List<GraphDocument> documents;
		private readonly Core.UI.PersistenceManager persistenceManager;
	}
}
