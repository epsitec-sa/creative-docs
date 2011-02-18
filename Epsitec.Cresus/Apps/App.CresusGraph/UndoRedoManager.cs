//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Actions;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>UndoRedoManager</c> class encapsulates an undo recorder and a redo
	/// recorder (see <cref="Recorder"/>).
	/// </summary>
	public sealed class UndoRedoManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UndoRedoManager"/> class.
		/// </summary>
		public UndoRedoManager()
		{
			this.undoRecorder = new Recorder ();
			this.redoRecorder = new Recorder ();
			
			this.undoRecorder.Changed += sender => this.UpdateUndoRedo ();
			this.redoRecorder.Changed += sender => this.UpdateUndoRedo ();

			this.UpdateUndoRedo ();
		}


		/// <summary>
		/// Gets the undo recorder.
		/// </summary>
		/// <value>The undo recorder.</value>
		public Recorder UndoRecorder
		{
			get
			{
				return this.undoRecorder;
			}
		}

		/// <summary>
		/// Gets the redo recorder.
		/// </summary>
		/// <value>The redo recorder.</value>
		public Recorder RedoRecorder
		{
			get
			{
				return this.redoRecorder;
			}
		}

		/// <summary>
		/// Gets the active manager, based on the active document.
		/// </summary>
		/// <value>The active manager.</value>
		public static UndoRedoManager Active
		{
			get
			{
				var document = GraphProgram.Application.Document;

				if (document == null)
				{
					return null;
				}
				else
				{
					return document.UndoRedo;
				}
			}
		}



		/// <summary>
		/// Plays back every action from the beginning of time.
		/// </summary>
		internal void PlayBackAll()
		{
			if (this.undoRecorder.Count == 0)
			{
				//	No action at all -- add the initial action to the undo/redo
				//	history.

				GraphActions.DocumentReload ();
			}
			else
			{
				this.PlayBackUndoHistory ();
			}
		}

		/// <summary>
		/// Undo the last action found in the undo list, if any.
		/// </summary>
		internal void Undo()
		{
			if (this.undoRecorder.Count > 1)
			{
				this.redoRecorder.Push (this.undoRecorder.Pop ());
				this.PlayBackUndoHistory ();
				this.OnUndoRedoExecuted ();
			}
		}

		/// <summary>
		/// Redo the last action found in the redo list, if any.
		/// </summary>
		internal void Redo()
		{
			if (this.redoRecorder.Count > 0)
			{
				this.undoRecorder.Push (this.redoRecorder.Pop ()).PlayBack ();
				this.OnUndoRedoExecuted ();
			}
		}

		/// <summary>
		/// Updates the undo/redo command states.
		/// </summary>
        private void UpdateUndoRedo()
		{
			var document = GraphProgram.Application.Document;

			GraphProgram.Application.SetEnable (ApplicationCommands.Undo, this.undoRecorder.Count > 1);
			GraphProgram.Application.SetEnable (ApplicationCommands.Redo, this.redoRecorder.Count > 0);

			if (document != null)
            {
				document.NotifyNeedsSave (true);
			}
		}

		private void PlayBackUndoHistory()
		{
			this.undoRecorder.ForEach (x => x.PlayBack ());
		}

		private void OnUndoRedoExecuted()
		{
			var handler = this.UndoRedoExecuted;

			if (handler != null)
            {
				handler (this);
            }
		}


		public event EventHandler UndoRedoExecuted;
		
		
		private readonly Recorder undoRecorder;
		private readonly Recorder redoRecorder;
	}
}
