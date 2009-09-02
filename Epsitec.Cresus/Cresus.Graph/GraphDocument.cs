//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

[assembly:DependencyClass (typeof (Epsitec.Cresus.Graph.GraphDocument))]

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>GraphDocument</c> class represents a graph document (data, styles and
	/// settings).
	/// </summary>
	public class GraphDocument : DependencyObject
	{
		public GraphDocument()
		{
			this.guid   = System.Guid.NewGuid ();
			this.views  = new List<DocumentViewController> ();
			this.chartSeries  = new List<ChartSeries> ();
			this.dataSet      = new GraphDataSet ();
			this.undoRecorder = new Actions.Recorder ();
			this.redoRecorder = new Actions.Recorder ();

			this.undoRecorder.ActionCreated += sender => this.redoRecorder.Clear ();

			this.undoRecorder.Changed += sender => this.UpdateUndoRedo ();
			this.redoRecorder.Changed += sender => this.UpdateUndoRedo ();
			
			this.dataSet.Changed += x => this.Clear ();

			this.CreateUI (GraphProgram.Application.MainWindowController.DocTabBook);
			this.ProcessDocumentChanged ();

			GraphProgram.Application.RegisterDocument (this);
			GraphProgram.Application.SetActiveDocument (this);
			GraphProgram.Application.SetupDataSet ();
		}


		public GraphDataSet DataSet
		{
			get
			{
				return this.dataSet;
			}
		}

		public IEnumerable<ChartSeries> ChartSeries
		{
			get
			{
				return this.chartSeries;
			}
		}

		public Actions.Recorder Recorder
		{
			get
			{
				return this.undoRecorder;
			}
		}

		public System.Action<IEnumerable<int>> RemoveSeriesFromGraphAction
		{
			get
			{
				return this.removeSeriesFromGraphAction;
			}
			set
			{
				if (this.removeSeriesFromGraphAction != value)
				{
					this.removeSeriesFromGraphAction = value;
					this.views.ForEach (view => view.RemoveSeriesFromGraphAction = value);
				}
			}
		}

		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				if (this.title != value)
				{
					this.title = value;
					this.views.ForEach (view => view.TitleSetterAction (value));
				}
			}
		}

		
		public ChartSeries Find(int index)
		{
			if ((index < 0) ||
				(index >= this.chartSeries.Count))
			{
				return null;
			}
			else
			{
				return this.chartSeries[index];
			}
		}

		public void Add(ChartSeries series)
		{
			this.chartSeries.Add (series);
			this.ProcessDocumentChanged ();
		}

		public void Remove(ChartSeries series)
		{
			this.chartSeries.Remove (series);
			this.ProcessDocumentChanged ();
		}

		public void Clear()
		{
			this.chartSeries.Clear ();
			this.ProcessDocumentChanged ();
		}

		
		public void MakeVisible()
		{
			this.views.ForEach (x => x.MakeVisible ());
		}


		internal XElement SaveSettings(XElement xml)
		{
			if (string.IsNullOrEmpty (this.path))
			{

				this.path = System.IO.Path.Combine (GraphApplication.Paths.AutoSavePath, this.guid.ToString ("D") + ".crgraph");
			}

			xml.Add (new XAttribute ("guid", this.guid));
			xml.Add (new XAttribute ("path", this.path));
			xml.Add (new XAttribute ("title", this.title ?? ""));
			xml.Add (new XElement ("undoActions", new XText (this.undoRecorder.SaveToString ())));
			xml.Add (new XElement ("views", this.views.Select (x => x.SaveSettings (new XElement ("view")))));

			if (this.cube != null)
			{
				xml.Add (new XElement ("cube",
					new XAttribute ("sliceDimA", this.cubeSliceDim1 ?? ""),
					new XAttribute ("sliceDimB", this.cubeSliceDim2 ?? "")));

				System.IO.Directory.CreateDirectory (GraphApplication.Paths.AutoSavePath);
				
				using (var stream = new System.IO.StreamWriter (this.path, false, System.Text.Encoding.UTF8))
				{
					this.cube.Save (stream);
				}
			}
			
			return xml;
		}

		internal void RestoreSettings(XElement xml)
		{
			this.guid = (System.Guid) xml.Attribute ("guid");
			this.path = (string) xml.Attribute ("path");
			this.title = (string) xml.Attribute ("title");

			var undoActionsXml = xml.Element ("undoActions");
			var viewsXml = xml.Element ("views");
			var cubeXml = xml.Element ("cube");

			if ((cubeXml != null) &&
				(System.IO.File.Exists (this.path)))
			{
				this.cubeSliceDim1 = (string) cubeXml.Attribute ("sliceDimA");
				this.cubeSliceDim2 = (string) cubeXml.Attribute ("sliceDimB");

				using (var stream = new System.IO.StreamReader (this.path, System.Text.Encoding.UTF8))
				{
					this.cube = new DataCube ();
					this.cube.Restore (stream);
				}
			}

			if (undoActionsXml != null)
			{
				this.undoRecorder.RestoreFromString (undoActionsXml.Value);
				this.undoRecorder.ForEach (x => x.PlayBack ());
			}
			
			if ((viewsXml != null) &&
				(viewsXml.Elements ("view").Any ()))
			{
				this.views.ToArray ().ForEach (x => x.Dispose ());

				System.Diagnostics.Debug.Assert (this.views.Count == 0);

				foreach (XElement node in viewsXml.Elements ())
				{
					var panel = this.CreateUI (GraphProgram.Application.MainWindowController.DocTabBook);

					panel.RestoreSettings (node);
				}
			}

			this.ProcessDocumentChanged ();
		}


		internal void Undo()
		{
			if (this.undoRecorder.Count > 1)
			{
				this.redoRecorder.Push (this.undoRecorder.Pop ());
				this.undoRecorder.ForEach (x => x.PlayBack ());
			}
		}

		internal void Redo()
		{
			if (this.redoRecorder.Count > 0)
			{
				this.undoRecorder.Push (this.redoRecorder.Pop ()).PlayBack ();
			}
		}

		internal void ExportImage()
		{
			this.views.First ().SaveMetafile (null);
		}

		internal void ExportImage(string path)
		{
			string ext = System.IO.Path.GetExtension (path).ToLowerInvariant ();

			switch (ext)
			{
				case ".emf":
					this.views.First ().SaveMetafile (path);
					break;

				case ".bmp":
				case ".png":
				case ".gif":
					this.views.First ().SaveBitmap (path);
					break;
			}
		}


		internal void ReloadDataSet()
		{
			if ((this.cube != null) &&
				(!string.IsNullOrEmpty (this.cubeSliceDim1)) &&
				(!string.IsNullOrEmpty (this.cubeSliceDim2)))
			{
				this.dataSet.LoadDataTable (this.cube.ExtractDataTable (this.cubeSliceDim1, this.cubeSliceDim2));
			}
			else
			{
				this.title = "Démo";
				this.dataSet.LoadDataTable (GraphDataSet.LoadComptaDemoData ());
			}
		}
		
		internal void LoadCube(DataCube cube)
		{
			this.cube = cube;
			this.ReloadDataSet ();
		}

		internal void SelectCubeSlice(string dim1, string dim2)
		{
			this.cubeSliceDim1 = dim1;
			this.cubeSliceDim2 = dim2;
			this.ReloadDataSet ();
		}

		
		private void UpdateUndoRedo()
		{
			GraphProgram.Application.SetEnable (ApplicationCommands.Undo, this.undoRecorder.Count > 1);
			GraphProgram.Application.SetEnable (ApplicationCommands.Redo, this.redoRecorder.Count > 0);
		}
		
		private void ProcessDocumentChanged()
		{
			foreach (var panel in this.views)
			{
				panel.Refresh ();
			}
		}


		private DocumentViewController CreateUI(TabBook book)
		{
			var page = new TabPage ()
			{
				TabTitle = this.title ?? "Document",
				Rank = 1
			};

			var frame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Padding = new Margins (0, 0, 0, 0),
				Parent = page,
				BackColor = Epsitec.Common.Widgets.Adorners.Factory.Active.ColorTabBackground
			};

			var panel = new DocumentViewController (frame, this, x => book.ActivePage = page,
				x =>
				{
					book.Items.Remove (page);
					page.Dispose ();
					this.views.Remove (x);
				})
			{
				RemoveSeriesFromGraphAction = this.RemoveSeriesFromGraphAction,
				TitleSetterAction = title => page.TabTitle = title
			};

			GraphDocument.SetDocument (page, this);

			this.views.Add (panel);
			book.Items.Add (page);

			return panel;
		}


		private readonly List<DocumentViewController> views;
		private readonly List<ChartSeries> chartSeries;
		private readonly GraphDataSet dataSet;
		private readonly Actions.Recorder undoRecorder;
		private readonly Actions.Recorder redoRecorder;

		public static void SetDocument(DependencyObject o, GraphDocument value)
		{
			o.SetValue (GraphDocument.DocumentProperty, value);
		}

		public static GraphDocument GetDocument(DependencyObject o)
		{
			return (GraphDocument) o.GetValue (GraphDocument.DocumentProperty);
		}

		public static DependencyProperty DocumentProperty = DependencyProperty.RegisterAttached ("Document", typeof (GraphDocument), typeof (GraphDocument));

		private string path;
		private string title;
		private System.Action<IEnumerable<int>> removeSeriesFromGraphAction;
		private System.Guid guid;
		
		private DataCube cube;
		private string cubeSliceDim1;
		private string cubeSliceDim2;
	}
}
