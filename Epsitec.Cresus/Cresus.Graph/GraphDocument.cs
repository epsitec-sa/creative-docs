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
		public GraphDocument(GraphApplication application)
		{
			this.application = application;
			this.dataSources = new List<GraphDataSource> ();
			this.outputSeries = new List<GraphDataSeries> ();
			this.syntheticSeries = new List<GraphSyntheticDataSeries> ();
			this.groups = new List<GraphDataGroup> ();
			this.columnLabels = new List<string> ();
			
			this.groupSource = new GraphDataSource (null)
			{
				Name = "Groups"
			};

			this.guid   = System.Guid.NewGuid ();
//-			this.views  = new List<DocumentViewController> ();

			this.undoRedoManager = new Actions.UndoRedoManager ();

			
			
			this.application.RegisterDocument (this);
			this.application.SetActiveDocument (this);
		}


		public IEnumerable<GraphDataSource> DataSources
		{
			get
			{
				return this.dataSources;
			}
		}

		public int DataSourceCount
		{
			get
			{
				return this.dataSources.Count;
			}
		}

		public GraphDataSource ActiveDataSource
		{
			get
			{
				return this.activeDataSource;
			}
			set
			{
				if (this.activeDataSource != value)
				{
					this.activeDataSource = value;
					this.NotifyDataSourceChanged ();
				}
			}
		}

		public IList<GraphDataSeries> OutputSeries
		{
			get
			{
				return this.outputSeries.AsReadOnly ();
			}
		}

		public IList<GraphDataGroup> Groups
		{
			get
			{
				return this.groups.AsReadOnly ();
			}
		}

		public IEnumerable<GraphDataSeries> DataSeries
		{
			get
			{
				return this.activeDataSource;
			}
		}

		public IList<GraphSyntheticDataSeries> SyntheticSeries
		{
			get
			{
				return this.syntheticSeries.AsReadOnly ();
			}
		}

		public IList<string> ChartColumnLabels
		{
			get
			{
				return this.columnLabels.AsReadOnly ();
			}
		}

		public Actions.UndoRedoManager UndoRedo
		{
			get
			{
				return this.undoRedoManager;
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
//-					this.views.ForEach (view => view.RemoveSeriesFromGraphAction = value);
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
//-					this.views.ForEach (view => view.TitleSetterAction (value));
				}
			}
		}




		public GraphDataSeries AddOutput(GraphDataSeries series)
		{
			series.IsSelected = true;
			
			var output = new GraphDataSeries (series)
			{
				Label = series.Source == null ? "" : series.Source.Name,
				Title = null,												//	force title inheritance through parent series
				ColorIndex = series.ColorIndex,
			};

			var syn = series as GraphSyntheticDataSeries;

			if ((syn != null) &&
				(!string.IsNullOrEmpty (syn.FunctionName)))
			{
				output.Label = Functions.FunctionFactory.GetFunctionCaption (syn.FunctionName);
			}

			output.Index = this.outputSeries.Count;
			this.outputSeries.Add (output);

			return output;
		}

		public void RemoveOutput(GraphDataSeries series)
		{
			GraphDataSeries item = this.ResolveOutputSeries (series);

			System.Diagnostics.Debug.Assert (item != null);
			System.Diagnostics.Debug.Assert (item.Parent != null);

			item.Parent.IsSelected = false;

			this.outputSeries.Remove (item);
			this.RenumberSeries (this.outputSeries);
		}

		private void RenumberSeries(IEnumerable<GraphDataSeries> collection)
		{
			int index = 0;
			
			foreach (var item in collection)
			{
				item.Index = index++;
			}
		}

		public bool SetOutputIndex(GraphDataSeries series, int newIndex)
		{
			var item = this.ResolveOutputSeries (series);

			if (item == null)
			{
				return false;
			}

			int oldIndex = item.Index;

			if (oldIndex < newIndex)
			{
				newIndex--;
			}

			this.outputSeries.Remove (item);
			this.outputSeries.Insert (newIndex, item);
			this.RenumberSeries (this.outputSeries);

			return true;
		}

		public GraphDataSeries ResolveOutputSeries(GraphDataSeries series)
		{
			if (this.outputSeries.Contains (series))
			{
				return series;
			}
			else
			{
				return this.outputSeries.Find (x => x.Parent == series);
			}
		}


		public GraphDataGroup AddGroup(IEnumerable<GraphDataSeries> series)
		{
			var group = new GraphDataGroup ()
			{
				Source = this.groupSource
			};

			series.ForEach (x => group.Add (x));

			this.groups.Add (group);

			return group;
		}

		public void RemoveGroup(GraphDataGroup group)
		{
			System.Diagnostics.Debug.Assert (this.groups.Contains (group));

			group.Dispose ();
			this.groups.Remove (group);
		}

		public void InvalidateCache()
		{
			this.groups.ForEach (x => x.Invalidate ());
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
			xml.Add (new XElement ("undoActions", new XText (this.UndoRedo.UndoRecorder.SaveToString ())));
			xml.Add (new XElement ("redoActions", new XText (this.UndoRedo.RedoRecorder.SaveToString ())));
//-			xml.Add (new XElement ("views", this.views.Select (x => x.SaveSettings (new XElement ("view")))));

			if (this.cube != null)
			{
				xml.Add (new XElement ("cube",
					new XAttribute ("sliceDimA", this.cubeSliceDim1 ?? ""),
					new XAttribute ("sliceDimB", this.cubeSliceDim2 ?? ""),
					new XAttribute ("converter", this.converterName ?? "")));

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
			var redoActionsXml = xml.Element ("redoActions");
			var viewsXml = xml.Element ("views");
			var cubeXml = xml.Element ("cube");

			if ((cubeXml != null) &&
				(System.IO.File.Exists (this.path)))
			{
				this.cubeSliceDim1 = (string) cubeXml.Attribute ("sliceDimA");
				this.cubeSliceDim2 = (string) cubeXml.Attribute ("sliceDimB");
				this.converterName = (string) cubeXml.Attribute ("converter");

				using (var stream = new System.IO.StreamReader (this.path, System.Text.Encoding.UTF8))
				{
					this.cube = new DataCube ();
					this.cube.Restore (stream);
				}
			}

			if (undoActionsXml != null)
			{
				this.UndoRedo.UndoRecorder.RestoreFromString (undoActionsXml.Value);
				this.UndoRedo.UndoRecorder.ForEach (x => x.PlayBack ());
			}
			if (redoActionsXml != null)
            {
				this.UndoRedo.RedoRecorder.RestoreFromString (redoActionsXml.Value);
            }

			this.ReloadDataSet ();
			this.application.WorkspaceController.Refresh ();
#if false
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
#endif
		}


		internal void ExportImage()
		{
//-			this.views.First ().SaveMetafile (null);
		}

		internal void ExportImage(string path)
		{
			string ext = System.IO.Path.GetExtension (path).ToLowerInvariant ();

			switch (ext)
			{
				case ".emf":
//-					this.views.First ().SaveMetafile (path);
					break;

				case ".bmp":
				case ".png":
				case ".gif":
//-					this.views.First ().SaveBitmap (path);
					break;
			}
		}


		internal void ReloadDataSet()
		{
			this.dataSources.Clear ();

			foreach (string sourceName in this.cube.GetDimensionValues ("Source"))
			{
				GraphDataSet dataSet = new GraphDataSet ();

				var source = new GraphDataSource (this.converterName);

				if ((this.cube != null) &&
				(!string.IsNullOrEmpty (this.cubeSliceDim1)) &&
				(!string.IsNullOrEmpty (this.cubeSliceDim2)))
				{
					dataSet.LoadDataTable (this.cube.ExtractDataTable ("Source="+sourceName, this.cubeSliceDim1, this.cubeSliceDim2));
				}
				else
				{
					this.title = "Démo";
					dataSet.LoadDataTable (GraphDataSet.LoadComptaDemoData ());
				}


				int index = 0;
				source.Name = sourceName;

				dataSet.DataTable.RowSeries.ForEach (
					series =>
						source.Add (
							new GraphDataSeries (series)
							{
								Index = index++,
#if true
								Label = "",
								Title = series.Label,
#else
							Label = series.Label.Substring (0, series.Label.IndexOf (' ')+1).Trim (),
							Title = series.Label.Substring (series.Label.IndexOf (' ')+1).Trim ()
#endif
							}));

				source.UpdateCategories ();

				this.dataSources.Add (source);
				this.activeDataSource = source;
			}
			
			this.NotifyDataSourceChanged ();
		}

		private void NotifyDataSourceChanged()
		{
			this.application.WorkspaceController.Refresh ();
		}


		internal void DefineImportConverter(string importerName)
		{
			this.converterName = importerName;
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

		internal void UpdateSyntheticSeries()
		{
			this.syntheticSeries.Clear ();
			
			foreach (var group in this.groups)
			{
				this.syntheticSeries.AddRange (group.SyntheticDataSeries);
			}
		}

		
#if false
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
#endif

		#region Dependency Properties

		public static void SetDocument(DependencyObject o, GraphDocument value)
		{
			o.SetValue (GraphDocument.DocumentProperty, value);
		}

		public static GraphDocument GetDocument(DependencyObject o)
		{
			return (GraphDocument) o.GetValue (GraphDocument.DocumentProperty);
		}

		public static DependencyProperty DocumentProperty = DependencyProperty.RegisterAttached ("Document", typeof (GraphDocument), typeof (GraphDocument));

		#endregion

		private readonly GraphApplication application;
		private readonly List<GraphDataSource> dataSources;
		private readonly List<GraphDataSeries> outputSeries;
		private readonly List<GraphDataGroup> groups;
		private readonly List<string> columnLabels;
		private readonly List<GraphSyntheticDataSeries> syntheticSeries;

		private readonly Actions.UndoRedoManager undoRedoManager;
		

		private string path;
		private string title;
		private System.Action<IEnumerable<int>> removeSeriesFromGraphAction;
		private System.Guid guid;
		
		private DataCube cube;
		private string cubeSliceDim1;
		private string cubeSliceDim2;
		private string converterName;

		private GraphDataSource activeDataSource;
		private GraphDataSource groupSource;
	}
}
