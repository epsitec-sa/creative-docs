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
using System;
using Epsitec.Common.Graph.Styles;

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
			this.filterCategories = new HashSet<GraphDataCategory> ();
			
			this.groupSource = new GraphDataSource (null)
			{
				Name = "Groups"
			};

			this.guid   = System.Guid.NewGuid ();
//-			this.views  = new List<DocumentViewController> ();

			this.undoRedoManager = new UndoRedoManager ();
			this.undoRedoManager.UndoRedoExecuted += sender => this.RefreshUI ();

			this.CreateDefaultColorStyle ();

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

		public IEnumerable<GraphDataCategory> ActiveFilterCategories
		{
			get
			{
				return this.filterCategories;
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
					this.RefreshUI ();
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

		public UndoRedoManager UndoRedo
		{
			get
			{
				return this.undoRedoManager;
			}
		}

		public string Title
		{
			get
			{
				if (this.cube == null)
				{
					return null;
				}
				else
				{
					return this.cube.Title;
				}
			}
		}

		public string SavePath
		{
			get;
			set;
		}

		public bool IsEmpty
		{
			get
			{
				return this.cube == null;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public ColorStyle DefaultColorStyle
		{
			get;
			private set;
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

			this.outputSeries.Add (output);

			GraphDocument.RenumberSeries (this.outputSeries);

			return output;
		}

		public void RemoveOutput(GraphDataSeries series)
		{
			GraphDataSeries item = this.ResolveOutputSeries (series);

			System.Diagnostics.Debug.Assert (item != null);
			System.Diagnostics.Debug.Assert (item.Parent != null);

			item.Parent.IsSelected = false;

			this.outputSeries.Remove (item);
			
			GraphDocument.RenumberSeries (this.outputSeries);
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
			
			GraphDocument.RenumberSeries (this.outputSeries);

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

		public void IncludeFilterCategory(string name)
		{
			var category = this.FindCategory (name);
			
			if (category.IsEmpty)
			{
				return;
			}
			
			if (this.filterCategories.Add (category))
			{
				this.RefreshUI ();
			}
		}

		public void ExcludeFilterCategory(string name)
		{
			var category = this.FindCategory (name);

			if (category.IsEmpty)
            {
				return;
            }

			if (this.filterCategories.Remove (category))
            {
				this.RefreshUI ();
            }
		}

		public void SelectDataSource(string name)
		{
			var source = this.FindSource (name);

			if (source == null)
            {
				return;
            }

			this.ActiveDataSource = source;
		}

		private GraphDataCategory FindCategory(string name)
		{
			if (this.ActiveDataSource == null)
            {
				return GraphDataCategory.Empty;
            }
			
			return this.ActiveDataSource.Categories.FirstOrDefault (x => x.Name == name);
		}

		private GraphDataSource FindSource(string name)
		{
			return this.DataSources.FirstOrDefault (x => x.Name == name);
		}

		public GraphDataGroup AddGroup(IEnumerable<GraphDataSeries> series)
		{
			var group = new GraphDataGroup ()
			{
				Source = this.groupSource
			};

			series.ForEach (x => group.Add (x));

			this.groups.Add (group);

			GraphDocument.RenumberGroups (this.groups);

			return group;
		}

		public void RemoveGroup(GraphDataGroup group)
		{
			System.Diagnostics.Debug.Assert (this.groups.Contains (group));

			group.Dispose ();
			this.groups.Remove (group);
			
			GraphDocument.RenumberGroups (this.groups);
		}


		public GraphDataSeries FindSeries(string id)
		{
			string[] args = GraphDocument.SplitSeriesId (id);

			string prefix = args[0];
			int    index;
			int    color;
			string name = args[3];

			if (!int.TryParse (args[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out index))
			{
				throw new System.ArgumentException ("Invalid id : index not a number");
			}
			if (!int.TryParse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out color))
			{
				throw new System.ArgumentException ("Invalid id : color index not a number");
			}

			GraphDataSeries series;

			switch (prefix)
			{
				case "I":
					series = this.FindSource (name)[index];
					break;

				case "F":
					series = this.FindGroup (index).GetSyntheticDataSeries (name);
					break;

				default:
					throw new System.ArgumentException ("Invalid id : prefix unknown");
			}

			if (series != null)
			{
				series.ColorIndex = color;
			}

			return series;
		}

		public string GetSeriesId(GraphDataSeries series)
		{
			return this.GetSeriesId (series, series.ColorIndex);
		}
		
		private string GetSeriesId(GraphDataSeries series, int colorIndex)
		{
			if (series.Parent != null)
			{
				return this.GetSeriesId (series.Parent, colorIndex);
			}

			var synthetic = series as GraphSyntheticDataSeries;

			if (synthetic == null)
			{
				var index  = series.Source.IndexOf (series);
				var source = series.Source.Name;

				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "I:{0}:{1}:{2}", index, colorIndex, source);
			}
			else
			{
				var index = synthetic.SourceGroup.Index;
				var func  = synthetic.FunctionName;

				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "F:{0}:{1}:{2}", index, colorIndex, func);
			}
		}
		
		public GraphDataGroup FindGroup(int index)
		{
			if ((index < 0) ||
				(index >= this.groups.Count))
			{
				return null;
			}
			else
			{
				return this.groups[index];
			}
		}

		
		private static void RenumberSeries(IEnumerable<GraphDataSeries> collection)
		{
			int index = 0;

			foreach (var item in collection)
			{
				item.Index = index++;
			}
		}

		private static void RenumberGroups(IEnumerable<GraphDataGroup> collection)
		{
			int index = 0;

			foreach (var item in collection)
			{
				item.Index = index++;
			}
		}

		private static string[] SplitSeriesId(string id)
		{
			int p1 = id.IndexOf (':');
			int p2 = id.IndexOf (':', p1+1);
			int p3 = id.IndexOf (':', p2+1);

			return new string[]
			{
				id.Substring (0, p1),
				id.Substring (p1+1, p2-p1-1),
				id.Substring (p2+1, p3-p2-1),
				id.Substring (p3+1)
			};
		}


		public void InvalidateCache()
		{
			this.groups.ForEach (x => x.Invalidate ());
		}

		public void NotifyNeedsSave(bool dirty)
		{
			this.application.SetEnable (ApplicationCommands.Save, dirty);
			this.application.SetEnable (ApplicationCommands.SaveAs, true);

			this.isDirty = dirty;
			
			this.application.AsyncSaveApplicationState ();
		}

		public void SaveDocument(string path)
		{
			System.DateTime now = System.DateTime.Now.ToUniversalTime ();
			string timeStamp = string.Concat (now.ToShortDateString (), " ", now.ToShortTimeString (), " UTC");

			XDocument doc = new XDocument (
				new XDeclaration ("1.0", "utf-8", "yes"),
				new XComment ("Saved on " + timeStamp),
				new XElement ("store",
					new XAttribute ("version", "1"),
					new XElement ("about",
						new XAttribute ("savePath", path),
						new XAttribute ("saveTime", timeStamp)),
					this.SaveSettings (new XElement ("doc"))));

			doc.Save (path);

			this.SavePath = path;
			this.NotifyNeedsSave (false);
		}

		public bool LoadDocument(string path)
		{
			var xmlDocument = XDocument.Load (path);
			var xmlStore    = xmlDocument.Element ("store");

			var version = (string) xmlStore.Attribute ("version");

			if (version == "1")
			{
				var element = xmlStore.Element ("doc");

				if (element != null)
				{
					this.RestoreSettings (element);
					this.SavePath = path;

					return true;
				}
			}

			return false;
		}

		public void ReloadDataSet()
		{
			this.dataSources.Clear ();
			this.outputSeries.Clear ();
			this.groups.Clear ();
			this.syntheticSeries.Clear ();
			this.filterCategories.Clear ();

			this.activeDataSource = null;

			if ((this.cube == null) ||
				(string.IsNullOrEmpty (this.cube.SliceDimA)) ||
				(string.IsNullOrEmpty (this.cube.SliceDimB)))
			{
				return;
			}

			foreach (string sourceName in this.cube.GetDimensionValues ("Source"))
			{
				var source  = new GraphDataSource (this.cube.ConverterName)
				{
					Name = sourceName,
				};

				var table = this.cube.ExtractDataTable ("Source="+sourceName, this.cube.SliceDimA, this.cube.SliceDimB);

				source.AddRange (from series in table.RowSeries
								 select new GraphDataSeries (series)
								 {
									 Label = "",
									 Title = series.Label,
								 });

				source.RenumberSeries ();
				source.UpdateCategories ();

				this.dataSources.Add (source);
				this.activeDataSource = source;
			}
			
			this.RefreshUI ();
		}

		public void RefreshUI()
		{
			Application.QueueAsyncCallback (this.application.WorkspaceController.Refresh);
		}


		internal void LoadCube(GraphDataCube cube)
		{
			GraphDocument.SaveCubeData (cube);
			this.cube = cube;
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

		internal XElement SaveSettings(XElement xml)
		{
			xml.Add (new XAttribute ("guid", this.guid));
			xml.Add (new XAttribute ("savePath", this.SavePath ?? ""));
			xml.Add (new XElement ("undoActions", new XText (this.UndoRedo.UndoRecorder.SaveToString ())));
			xml.Add (new XElement ("redoActions", new XText (this.UndoRedo.RedoRecorder.SaveToString ())));
			xml.Add (new XElement ("cubes", this.SaveCubeSettings ()));

			return xml;
		}

		internal void RestoreSettings(XElement xml)
		{
			this.guid = (System.Guid) xml.Attribute ("guid");
			this.SavePath = (string) xml.Attribute ("savePath");
			
			var undoActionsXml = xml.Element ("undoActions");
			var redoActionsXml = xml.Element ("redoActions");
			var viewsXml = xml.Element ("views");
			var cubesXml = xml.Element ("cubes");

			if (cubesXml != null)
			{
				this.RestoreCubeSettings (cubesXml.Elements ());
			}


			GraphActions.DocumentReload ();

			if (undoActionsXml != null)
			{
				this.UndoRedo.UndoRecorder.RestoreFromString (undoActionsXml.Value);
				this.UndoRedo.UndoRecorder.ForEach (x => x.PlayBack ());
			}
			if (redoActionsXml != null)
			{
				this.UndoRedo.RedoRecorder.RestoreFromString (redoActionsXml.Value);
			}

			this.NotifyNeedsSave (false);
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


		private void CreateDefaultColorStyle()
		{
			var colorStyle = new ColorStyle ("line-color");

			for (int hue = 0; hue < 360; hue += 36)
			{
				colorStyle.Add (Color.FromAlphaHsv (1.0, hue, 1.0, 1.0));
			}

			this.DefaultColorStyle = colorStyle;
		}

		private IEnumerable<XElement> SaveCubeSettings()
		{
			if (this.cube != null)
			{
				yield return GraphDocument.SaveCubeSettings (this.cube);
			}
		}

		private static XElement SaveCubeSettings(GraphDataCube cube)
		{
			GraphDocument.SaveCubeData (cube);

			return new XElement ("cube",
				new XAttribute ("guid", cube.Guid),
				new XAttribute ("sliceDimA", cube.SliceDimA ?? ""),
				new XAttribute ("sliceDimB", cube.SliceDimB ?? ""),
				new XAttribute ("converter", cube.ConverterName ?? ""),
				new XAttribute ("title", cube.Title ?? "")
				);
		}

		private static void SaveCubeData(GraphDataCube cube)
		{
			var cubeGuid = cube.Guid;
			var dataPath = GraphDocument.GetDataPath (cubeGuid);

			System.IO.Directory.CreateDirectory (System.IO.Path.GetDirectoryName (dataPath));

			using (var stream = new System.IO.StreamWriter (dataPath, false, System.Text.Encoding.UTF8))
			{
				cube.Save (stream);
			}
		}

		private void RestoreCubeSettings(IEnumerable<XElement> elements)
		{
			foreach (var cubeXml in elements)
			{
				if ((cubeXml != null) &&
					(cubeXml.Name == "cube"))
				{
					var cubeGuid = (System.Guid) cubeXml.Attribute ("guid");
					var dataPath = GraphDocument.GetDataPath (cubeGuid);

					if (System.IO.File.Exists (dataPath))
					{
						var cubeSliceDim1 = (string) cubeXml.Attribute ("sliceDimA");
						var cubeSliceDim2 = (string) cubeXml.Attribute ("sliceDimB");
						var converterName = (string) cubeXml.Attribute ("converter");
						var cubeTitle     = (string) cubeXml.Attribute ("title");

						using (var stream = new System.IO.StreamReader (dataPath, System.Text.Encoding.UTF8))
						{
							this.cube = new GraphDataCube ()
							{
								Guid = cubeGuid,
								SliceDimA = cubeSliceDim1,
								SliceDimB = cubeSliceDim2,
								ConverterName = converterName,
								Title = cubeTitle,
							};

							this.cube.Restore (stream);
						}
					}
				}
			}
		}

		private static string GetDataPath(Guid guid)
		{
			return System.IO.Path.Combine (GraphApplication.Paths.AutoSavePath, guid.ToString ("D") + ".crgraph-data");
		}


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
		private readonly HashSet<GraphDataCategory> filterCategories;

		private readonly UndoRedoManager undoRedoManager;
		

		private System.Guid guid;
		private bool isDirty;
		
		private GraphDataCube	cube;

		private GraphDataSource activeDataSource;
		private GraphDataSource groupSource;
	}
}
