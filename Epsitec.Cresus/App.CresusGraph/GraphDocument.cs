//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Styles;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass (typeof (Epsitec.Cresus.Graph.GraphDocument))]

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>GraphDocument</c> class represents a graph document (data, styles and
	/// settings).
	/// </summary>
	public class GraphDocument : DependencyObject
	{
		public GraphDocument (GraphApplication application)
		{
			this.application = application;
			this.dataSources = new List<GraphDataSource> ();
			this.outputSeries = new List<GraphDataSeries> ();
			this.syntheticSeries = new List<GraphSyntheticDataSeries> ();
			this.groups = new List<GraphDataGroup> ();
			this.columnLabels = new List<string> ();
			this.filterCategories = new HashSet<GraphDataCategory> ();
			this.chartSnapshots = new List<GraphChartSnapshot> ();
			this.cubes = new List<GraphDataCube> ();

			this.groupSource = new GraphDataSource (null, null)
			{
				Name = "Groups"
			};

			this.guid = System.Guid.NewGuid ();
			//-			this.views  = new List<DocumentViewController> ();

			this.undoRedoManager = new UndoRedoManager ();
			this.undoRedoManager.UndoRedoExecuted += sender => this.RefreshUI ();

			this.ResetDefaultColors ();

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

		public IList<GraphDataCube> Cubes
		{
			get
			{
				return this.cubes.AsReadOnly ();
			}
		}

		public GraphDataCube ActiveCube
		{
			get
			{
				return this.activeCube;
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
				if (this.activeCube == null)
				{
					return null;
				}
				else
				{
					return this.activeCube.Title;
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
				return this.activeCube == null;
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

		public List<GraphChartSnapshot> ChartSnapshots
		{
			get
			{
				return this.chartSnapshots;
			}
		}

		public bool FullDocumentSaveInProgress
		{
			get;
			set;
		}

		public GraphDataSeries AddOutput (GraphDataSeries series)
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

		public void RemoveOutput (GraphDataSeries series)
		{
			GraphDataSeries item = this.ResolveOutputSeries (series);

			System.Diagnostics.Debug.Assert (item != null);
			System.Diagnostics.Debug.Assert (item.Parent != null);

			item.Parent.IsSelected = false;

			this.outputSeries.Remove (item);

			GraphDocument.RenumberSeries (this.outputSeries);
		}

		public bool SetOutputIndex (GraphDataSeries series, int newIndex)
		{
			var item = this.ResolveOutputSeries (series);

			if (item == null)
			{
				return false;
			}

			int oldIndex = item.Index;

			System.Diagnostics.Debug.Assert (oldIndex > -1);

			if (oldIndex < newIndex)
			{
				newIndex--;
			}

			this.outputSeries.Remove (item);
			this.outputSeries.Insert (newIndex, item);

			GraphDocument.RenumberSeries (this.outputSeries);

			return true;
		}

		public bool SetCubeIndex (System.Guid guid, int newIndex)
		{
			var cube = this.cubes.Find (x => x.Guid == guid);

			if (cube == null)
			{
				return false;
			}

			int oldIndex = this.cubes.IndexOf (cube);

			System.Diagnostics.Debug.Assert (oldIndex > -1);

			if (oldIndex < newIndex)
			{
				newIndex--;
			}

			this.cubes.RemoveAt (oldIndex);
			this.cubes.Insert (newIndex, cube);

			return true;
		}

		public GraphDataSeries ResolveOutputSeries (GraphDataSeries series)
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

		public void IncludeFilterCategory (string name)
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

		public void ExcludeFilterCategory (string name)
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

		public void SelectDataSource (string name)
		{
			var source = this.FindSource (name);

			if (source == null)
			{
				return;
			}

			this.ActiveDataSource = source;
		}

		private GraphDataCategory FindCategory (string name)
		{
			if (this.ActiveDataSource == null)
			{
				return GraphDataCategory.Empty;
			}

			return this.ActiveDataSource.Categories.FirstOrDefault (x => x.Name == name);
		}

		private GraphDataSource FindSource (string name)
		{
			return this.DataSources.FirstOrDefault (x => x.Name == name);
		}

		public GraphDataGroup AddGroup (IEnumerable<GraphDataSeries> series)
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

		public void RemoveGroup (GraphDataGroup group)
		{
			System.Diagnostics.Debug.Assert (this.groups.Contains (group));

			group.Dispose ();
			this.groups.Remove (group);

			GraphDocument.RenumberGroups (this.groups);
		}

		public bool SelectCube (System.Guid guid)
		{
			var cube = this.cubes.Find (x => x.Guid == guid);

			if (cube == null)
			{
				return false;
			}

			this.PreserveActiveSeriesAndGroups ();
			this.activeCube = cube;
			this.RestoreActiveSeriesAndGroups ();
			this.RefreshDataSet ();
			//			this.ReloadDataSet ();

			this.application.NotifyDocumentChanged ();

			return true;
		}


		public GraphDataSeries FindSeries (string id)
		{
			string[] args = GraphDocument.SplitSeriesId (id);

			string prefix = args[0];
			int index;
			int color;
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

		public string GetSeriesId (GraphDataSeries series)
		{
			return this.GetSeriesId (series, series.ColorIndex);
		}

		private string GetSeriesId (GraphDataSeries series, int colorIndex)
		{
			if (series.Parent != null)
			{
				return this.GetSeriesId (series.Parent, colorIndex);
			}

			var synthetic = series as GraphSyntheticDataSeries;

			if (synthetic == null)
			{
				var index = series.Source.IndexOf (series);
				var source = series.Source.Name;

				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "I:{0}:{1}:{2}", index, colorIndex, source);
			}
			else
			{
				var index = synthetic.SourceGroup.Index;
				var func = synthetic.FunctionName;

				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "F:{0}:{1}:{2}", index, colorIndex, func);
			}
		}

		public GraphDataGroup FindGroup (int index)
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


		private static void RenumberSeries (IEnumerable<GraphDataSeries> collection)
		{
			int index = 0;

			foreach (var item in collection)
			{
				item.Index = index++;
			}
		}

		private static void RenumberGroups (IEnumerable<GraphDataGroup> collection)
		{
			int index = 0;

			foreach (var item in collection)
			{
				item.Index = index++;
			}
		}

		private static string[] SplitSeriesId (string id)
		{
			int p1 = id.IndexOf (':');
			int p2 = id.IndexOf (':', p1 + 1);
			int p3 = id.IndexOf (':', p2 + 1);

			return new string[]
			{
				id.Substring (0, p1),
				id.Substring (p1+1, p2-p1-1),
				id.Substring (p2+1, p3-p2-1),
				id.Substring (p3+1)
			};
		}


		public void InvalidateCache ()
		{
			this.groups.ForEach (x => x.Invalidate ());
		}

		public void NotifyNeedsSave (bool dirty)
		{
			this.application.SetEnable (ApplicationCommands.Save, dirty);
			this.application.SetEnable (ApplicationCommands.SaveAs, true);

			this.isDirty = dirty;

			this.application.AsyncSaveApplicationState ();
		}

		public void SaveDocument (string path)
		{
			try
			{
				this.FullDocumentSaveInProgress = true;

				System.DateTime now = System.DateTime.UtcNow;
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
			finally
			{
				this.FullDocumentSaveInProgress = false;
			}
		}

		public bool LoadDocument (string path)
		{
			try
			{
				var xmlDocument = XDocument.Load (path);
				var xmlStore = xmlDocument.Element ("store");

				var version = (string)xmlStore.Attribute ("version");

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
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("LoadDocument : " + ex.Message);
			}

			return false;
		}

		public void ResetDefaultColors ()
		{
			this.DefaultColorStyle = GetDefaultColorStyle ();
		}

		public void ReloadDataSet ()
		{
			if ((this.cube == null) &&
				(this.activeCube != null))
			{
				this.cube = new GraphDataCube ();
			}

			if ((this.cubes.Count == 0) ||
				(this.activeCube == null) ||
				(string.IsNullOrEmpty (this.activeCube.SliceDimA)) ||
				(string.IsNullOrEmpty (this.activeCube.SliceDimB)))
			{
				return;
			}

			this.cube.Clear ();

			this.cube.SliceDimA = this.cube.SliceDimA ?? this.activeCube.SliceDimA;
			this.cube.SliceDimB = this.cube.SliceDimB ?? this.activeCube.SliceDimB;
			this.cube.Title = this.cube.Title ?? this.activeCube.Title;
			this.cube.ConverterName = this.cube.ConverterName ?? this.activeCube.ConverterName;
			this.cube.ConverterMeta = this.cube.ConverterMeta ?? this.activeCube.ConverterMeta;

			this.cubes.ForEach (x => this.cube.AddCube (x));

			this.activeDataSource = null;

			foreach (string sourceName in this.cube.GetDimensionValues ("Source"))
			{
				var table = this.cube.ExtractDataTable ("Source=" + sourceName, this.cube.SliceDimA, this.cube.SliceDimB);

				if ((table.RowCount == 0) ||
					(table.ColumnCount == 0))
				{
					continue;
				}

				var source = new GraphDataSource (this.cube.ConverterName, this.cube.ConverterMeta)
				{
					Name = sourceName,
				};

				source.AddRange (from series in table.RowSeries
								 //-								 where series.Values.Any (value => value.Value != 0)
								 select new GraphDataSeries (series)
								 {
									 Label = "",
									 Title = series.Label,
								 });

				source.RenumberSeries ();
				source.UpdateCategories ();

				this.dataSources.Add (source);

				if (this.activeDataSource == null)
				{
					this.activeDataSource = source;
				}
			}

			if (this.activeDataSource != null)
			{
				foreach (var cat in this.activeDataSource.Categories)
				{
					if (cat.IsGeneric)
					{
						continue;
					}

					this.filterCategories.Add (cat);
				}
			}

			this.RefreshUI ();
		}

		public void RefreshDataSet ()
		{
			this.UndoRedo.PlayBackAll ();
		}

		public void RefreshUI ()
		{
			Application.QueueAsyncCallback (this.application.WorkspaceController.Refresh);
			Application.QueueAsyncCallback (this.application.WorkspaceController.RefreshSnapshots);
		}

		/// <summary>
		/// Imports the cube into the document.
		/// </summary>
		/// <param name="cube">The cube.</param>
		/// <param name="path">The path of the source data.</param>
		/// <param name="encoding">The encoding of the source data (or <c>null</c> if the
		/// source data cannot be used, i.e. because it has to be transformed by some
		/// external application).</param>
		internal void ImportCube (GraphDataCube cube, string path, System.Text.Encoding encoding)
		{
			if (cube != null)
			{
				cube.LoadPath = path;
				cube.LoadEncoding = encoding;

				var op = this.CheckCubeSources (cube);

				switch (op)
				{
					case ImportOperation.Add:
					case ImportOperation.Merge:
						this.LoadCube (cube);
						this.RefreshDataSet ();
						break;

					case ImportOperation.New:
						this.LoadCubeInNewInstance (cube);
						break;
				}
			}
		}

		internal void LoadCubeInNewInstance (GraphDataCube cube)
		{
			if (cube != null)
			{
				GraphDocument.SaveCubeData (cube);

				var cubeGuid = cube.Guid;
				var dataPath = GraphDocument.GetDataPath (cubeGuid);

				var startInfo = new System.Diagnostics.ProcessStartInfo (
					Globals.ExecutablePath,
					string.Concat (@"""", "-loadcube=", dataPath, @""""));

				var process = System.Diagnostics.Process.Start (startInfo);

				if (process != null)
				{
					process.WaitForInputIdle ();
				}
			}
		}

		internal void LoadCube (GraphDataCube cube)
		{
			if (cube != null)
			{
				GraphDocument.SaveCubeData (cube);

				this.activeCube = cube;
				this.cubes.Add (cube);

				this.application.WorkspaceController.SetPreferredGraphType (cube.GetPreferredGraphType ());
				this.application.NotifyDocumentChanged ();
			}
		}

		internal void UpdateSyntheticSeries ()
		{
			this.syntheticSeries.Clear ();

			foreach (var group in this.groups)
			{
				this.syntheticSeries.AddRange (group.SyntheticDataSeries);
			}
		}

		internal XElement SaveSettings (XElement xml)
		{
			xml.Add (new XAttribute ("guid", this.guid));
			xml.Add (new XAttribute ("savePath", this.SavePath ?? ""));
			xml.Add (new XElement ("undoActions", new XText (this.UndoRedo.UndoRecorder.SaveToString ())));
			xml.Add (new XElement ("redoActions", new XText (this.UndoRedo.RedoRecorder.SaveToString ())));
			xml.Add (new XElement ("cubes", this.SaveCubeSettings ()));
			xml.Add (new XElement ("snapshots", this.chartSnapshots.Select (snapshot => snapshot.SaveSettings (new XElement ("snapshot")))));

			return xml;
		}

		internal void RestoreSettings (XElement xml)
		{
			this.guid = (System.Guid)xml.Attribute ("guid");
			this.SavePath = (string)xml.Attribute ("savePath");

			var undoActionsXml = xml.Element ("undoActions");
			var redoActionsXml = xml.Element ("redoActions");
			var viewsXml = xml.Element ("views");
			var cubesXml = xml.Element ("cubes");
			var snapshotsXml = xml.Element ("snapshots");

			if (cubesXml != null)
			{
				this.RestoreCubeSettings (cubesXml.Elements ());
			}


			try
			{
				if (undoActionsXml != null)
				{
					this.UndoRedo.UndoRecorder.RestoreFromString (undoActionsXml.Value);
					this.UndoRedo.UndoRecorder.ForEach (x => x.PlayBack ());
				}
				if (redoActionsXml != null)
				{
					this.UndoRedo.RedoRecorder.RestoreFromString (redoActionsXml.Value);
				}
			}
			catch
			{
				//	In case of a deserialization issue, simply clear everything back to
				//	an empy document:

				this.UndoRedo.UndoRecorder.Clear ();
				this.UndoRedo.RedoRecorder.Clear ();

				this.ClearData ();
			}

			if ((this.UndoRedo.UndoRecorder.Count == 0) &&
				(this.UndoRedo.RedoRecorder.Count == 0))
			{
				GraphActions.DocumentReload ();
			}

			if (snapshotsXml != null)
			{
				this.chartSnapshots.AddRange (snapshotsXml.Elements ().Select (x => GraphChartSnapshot.FromXml (x)));
			}

			this.NotifyNeedsSave (false);
			this.RefreshUI ();
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


		private ImportOperation CheckCubeSources (GraphDataCube cube)
		{
			var dimensionValues = cube.GetDimensionValues ("Source");

			if (dimensionValues == null)
			{
				return ImportOperation.Cancel;
			}

			var sources = dimensionValues.ToArray ();
			var count = sources.Length;

			switch (count)
			{
				case 0:
					//	Problem : no source defined !
					//Res.Captions.Message.DataImport.Failure.NoSource;
					break;

				case 1:
					//	Single source
					return this.CheckSingleCube (cube, sources);

				default:
					//	Problem : multiple sources not supported (yet)
					//Res.Captions.Message.DataImport.Failure.MultipleSources;
					//	HACK: fix this to analyze all sources
					if (this.cubes.Count == 0)
					{
						return ImportOperation.Add;
					}
					else
					{
						return ImportOperation.New;
					}
			}

			return ImportOperation.Cancel;
		}

		private ImportOperation CheckSingleCube (GraphDataCube cube, string[] sources)
		{
			//	For PICCOLO (and below), do not allow more than one source at any
			//	given time...

			if (GraphSerial.LicensingInfo <= LicensingInfo.ValidPiccolo)
			{
				if (this.cubes.Count == 0)
				{
					return ImportOperation.Add;
				}
				else
				{
					return ImportOperation.New;
				}
			}

			//	Check whether there is a source collision with existing sources :

			if (this.cubes.Any (x => x.GetDimensionValues ("Source").FirstOrDefault () == sources[0]))
			{
				//	Found at least one other cube which has the same source
				//	name. Ask what to do.

				return Dialogs.DuplicateImportDialog.AskHowToImportDuplicateSource (cube);
			}
			else
			{
				return ImportOperation.Add;
			}
		}

		private void PreserveActiveSeriesAndGroups ()
		{
			if (this.activeCube != null)
			{
				var save = new GraphDataSettings ();

				save.FilterCategories.AddRange (this.filterCategories);
				save.Groups.AddRange (this.groups);
				save.OutputSeries.AddRange (this.outputSeries);
				save.DataSourceName = this.activeDataSource == null ? null : this.activeDataSource.Name;

				this.activeCube.SavedSettings = save;
			}
		}

		private void RestoreActiveSeriesAndGroups ()
		{
			this.ClearData ();

			if (this.activeCube != null)
			{
				var save = activeCube.SavedSettings;

				if (save != null)
				{
					this.filterCategories.UnionWith (save.FilterCategories);
					this.groups.AddRange (save.Groups);
					this.outputSeries.AddRange (save.OutputSeries);

					string name = save.DataSourceName;

					if (name != null)
					{
						this.activeDataSource = this.dataSources.Find (x => x.Name == name);
					}
				}
			}

			this.UpdateSyntheticSeries ();
		}

		public void ClearData ()
		{
			this.outputSeries.Clear ();
			this.groups.Clear ();

			//	Clear information which can or will be rebuilt automatically when the
			//	ReloadDataSet method will be called:

			this.dataSources.Clear ();
			this.syntheticSeries.Clear ();
			this.filterCategories.Clear ();

			this.activeDataSource = null;
		}

		public static ColorStyle GetDefaultColorStyle ()
		{
			var colorStyle = new ColorStyle ("line-color");

			for (int hue = 0; hue < 360; hue += 36)
			{
				colorStyle.Add (Color.FromAlphaHsv (1.0, hue, 1.0, 1.0));
			}

			return colorStyle;
		}

		private IEnumerable<XElement> SaveCubeSettings ()
		{
			return this.cubes.Select (cube => GraphDocument.SaveCubeSettings (cube, this.FullDocumentSaveInProgress));
		}

		private static XElement SaveCubeSettings (GraphDataCube cube, bool saveCubeData)
		{
			GraphDocument.SaveCubeData (cube);

			var xml = new XElement ("cube",
				new XAttribute ("guid", cube.Guid),
				new XAttribute ("sliceDimA", cube.SliceDimA ?? ""),
				new XAttribute ("sliceDimB", cube.SliceDimB ?? ""),
				new XAttribute ("converter", cube.ConverterName ?? ""),
				new XAttribute ("title", cube.Title ?? ""),
				new XAttribute ("loadPath", cube.LoadPath ?? ""),
				new XAttribute ("loadEncoding", cube.LoadEncoding == null ? "" : cube.LoadEncoding.WebName)
				);

			if (saveCubeData)
			{
				xml.Add (new XAttribute ("data", "embedded"));

				using (var stream = new System.IO.StringWriter ())
				{
					cube.Save (stream);
					xml.Add (new XCData (stream.ToString ()));
				}
			}

			return xml;
		}

		internal static void SaveCubeData (GraphDataCube cube)
		{
			var cubeGuid = cube.Guid;
			var dataPath = GraphDocument.GetDataPath (cubeGuid);

			System.IO.Directory.CreateDirectory (System.IO.Path.GetDirectoryName (dataPath));

			using (var stream = new System.IO.StreamWriter (dataPath, false, System.Text.Encoding.UTF8))
			{
				cube.Save (stream);
			}
		}

		internal static GraphDataCube LoadCubeData (string path)
		{
			if (System.IO.File.Exists (path))
			{
				using (var stream = new System.IO.StreamReader (path, System.Text.Encoding.UTF8))
				{
					GraphDataCube cube = new GraphDataCube ();
					cube.Restore (stream);
					return cube;
				}
			}

			return null;
		}

		/// <summary>
		/// Restore data from an XML tree.
		/// First tries to load them from the tree itself, which has to be done when loading a document.
		/// If data is not available, it may mean that we are currently restoring from a crashed application,
		/// tries to load data from the AutoSave folder. 
		/// If not available, does not load anything. 
		/// </summary>
		/// <param name="elements">Elements from the Cube XML tree</param>
		private void RestoreCubeSettings (IEnumerable<XElement> elements)
		{
			foreach (var cubeXml in elements)
			{
				if ((cubeXml != null) &&
					(cubeXml.Name == "cube"))
				{
					var cubeGuid = (System.Guid)cubeXml.Attribute ("guid");

					var cubeSliceDim1 = (string)cubeXml.Attribute ("sliceDimA");
					var cubeSliceDim2 = (string)cubeXml.Attribute ("sliceDimB");
					var converterName = (string)cubeXml.Attribute ("converter");
					var cubeTitle = (string)cubeXml.Attribute ("title");
					var loadPath = (string)cubeXml.Attribute ("loadPath");
					var loadEncoding = (string)cubeXml.Attribute ("loadEncoding");

					// New cube
					var cube = new GraphDataCube ()
					{
						Guid = cubeGuid,
						SliceDimA = cubeSliceDim1,
						SliceDimB = cubeSliceDim2,
						ConverterName = converterName,
						Title = cubeTitle,
						LoadPath = loadPath,
						LoadEncoding = string.IsNullOrEmpty (loadEncoding) ? null : System.Text.Encoding.GetEncoding (loadEncoding),
					};

					// Data reader
					TextReader reader = null;

					// Is there data available
					if (!string.IsNullOrEmpty (cubeXml.Value))
					{
						reader = new StringReader (cubeXml.Value);
					}
					else
					{
						// Try to restore from the AutoSave folder
						var dataPath = GraphDocument.GetDataPath (cubeGuid);

						if (File.Exists (dataPath))
						{
							reader = new StreamReader (dataPath, System.Text.Encoding.UTF8);
						}
					}

					// We have data , restore
					if (reader != null)
					{
						cube.Restore (reader);
						reader.Dispose ();

						this.LoadCube (cube);
					}
				}
			}
		}

		private static string GetDataPath (System.Guid guid)
		{
			return System.IO.Path.Combine (GraphApplication.Paths.AutoSavePath, guid.ToString ("D") + ".crgraph-data");
		}


		#region Dependency Properties

		public static void SetDocument (DependencyObject o, GraphDocument value)
		{
			o.SetValue (GraphDocument.DocumentProperty, value);
		}

		public static GraphDocument GetDocument (DependencyObject o)
		{
			return (GraphDocument)o.GetValue (GraphDocument.DocumentProperty);
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
		private readonly List<GraphChartSnapshot> chartSnapshots;
		private readonly List<GraphDataCube> cubes;

		private readonly UndoRedoManager undoRedoManager;


		private System.Guid guid;
		private bool isDirty;

		private GraphDataCube cube;
		private GraphDataCube activeCube;

		private GraphDataSource activeDataSource;
		private GraphDataSource groupSource;
	}
}
