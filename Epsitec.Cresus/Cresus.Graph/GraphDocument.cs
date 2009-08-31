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


		public void Add(ChartSeries series)
		{
			this.chartSeries.Add (series);
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
			xml.Add (new XAttribute ("path", this.path ?? ""));
			xml.Add (new XElement ("undoActions", new XText (this.undoRecorder.SaveToString ())));
			xml.Add (new XElement ("views", this.views.Select (x => x.SaveSettings (new XElement ("view")))));
			
			return xml;
		}

		internal void RestoreSettings(XElement xml)
		{
			this.path = (string) xml.Attribute ("path");

			var undoActionsXml = xml.Element ("undoActions");
			var viewsXml = xml.Element ("views");

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
			this.views.First ().SaveMetafile (@"F:\graph.emf");
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
				TabTitle = "Document"
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
				});

			this.views.Add (panel);
			book.Items.Add (page);

			return panel;
		}


		private readonly List<DocumentViewController> views;
		private readonly List<ChartSeries> chartSeries;
		private readonly GraphDataSet dataSet;
		private readonly Actions.Recorder undoRecorder;
		private readonly Actions.Recorder redoRecorder;

		private string path;
	}
}
