//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDocument
	{
		public GraphDocument()
		{
			this.graphPanels  = new List<GraphPanelController> ();
			this.chartSeries  = new List<ChartSeries> ();
			this.dataSet      = new GraphDataSet ();
			this.undoRecorder = new Actions.Recorder ();
			this.redoRecorder = new Actions.Recorder ();

			this.undoRecorder.ActionCreated += sender => this.redoRecorder.Clear ();

			this.undoRecorder.Changed += sender => this.UpdateUndoRedo ();
			this.redoRecorder.Changed += sender => this.UpdateUndoRedo ();
			
			this.dataSet.Changed += x => this.Clear ();

			this.CreateUI ();
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


		internal XElement SaveSettings(XElement xml)
		{
			xml.Add (new XAttribute ("path", this.path ?? ""));
			xml.Add (new XElement ("undoActions",
				new XText (this.undoRecorder.SaveToString ())));
			
			return xml;
		}

		internal void RestoreSettings(XElement xml)
		{
			this.path = (string) xml.Attribute ("path");

			var undoActionsXml = xml.Element ("undoActions");

			if (undoActionsXml != null)
			{
				this.undoRecorder.RestoreFromString (undoActionsXml.Value);
				this.undoRecorder.ForEach (x => x.PlayBack ());
			}
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
		
		
		private void UpdateUndoRedo()
		{
			GraphProgram.Application.SetEnable (ApplicationCommands.Undo, this.undoRecorder.Count > 1);
			GraphProgram.Application.SetEnable (ApplicationCommands.Redo, this.redoRecorder.Count > 0);
		}
		
		private void ProcessDocumentChanged()
		{
			foreach (var panel in this.graphPanels)
			{
				panel.Refresh ();
			}
		}


		private void CreateUI()
		{
			if (this.window == null)
			{
				this.window = new Window ()
				{
					Text = "Document",
					ClientSize = new Size (960, 600)
				};

				var frame = new FrameBox ()
				{
					Dock = DockStyle.Fill,
					Margins = new Margins (4, 4, 8, 8),
					Parent = this.window.Root
				};

				var panel = new GraphPanelController (frame, this);

				this.graphPanels.Add (panel);
				this.window.Show ();
			}
		}

		
		private readonly List<GraphPanelController> graphPanels;
		private readonly List<ChartSeries> chartSeries;
		private readonly GraphDataSet dataSet;
		private readonly Actions.Recorder undoRecorder;
		private readonly Actions.Recorder redoRecorder;

		private Window window;
		private string path;
	}
}
