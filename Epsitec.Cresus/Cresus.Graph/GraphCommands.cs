//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public sealed class GraphCommands
	{
		public GraphCommands(GraphApplication application)
		{
			this.application  = application;
			this.undoRecorder = Actions.Recorder.Current;
			this.redoRecorder = new Actions.Recorder ();

			this.undoRecorder.RecordPushed += sender => this.redoRecorder.Clear ();
			this.undoRecorder.Changed += sender => this.UpdateUndoRedo ();
			this.redoRecorder.Changed += sender => this.UpdateUndoRedo ();

			application.CommandDispatcher.RegisterController (this);
		}


		private void UpdateUndoRedo()
		{
			this.application.CommandContext.GetCommandState (ApplicationCommands.Undo).Enable = this.undoRecorder.Count > 1;
			this.application.CommandContext.GetCommandState (ApplicationCommands.Redo).Enable = this.redoRecorder.Count > 0;
		}


		[Command(ApplicationCommands.Id.Undo)]
		private void UndoCommand()
		{
			if (this.undoRecorder.Count > 1)
			{
				this.redoRecorder.Push (this.undoRecorder.Pop ());
				
				foreach (var item in this.undoRecorder)
				{
					item.PlayBack ();
				}
			}
		}

		[Command (ApplicationCommands.Id.Redo)]
		private void RedoCommand()
		{
			if (this.redoRecorder.Count > 0)
			{
				this.undoRecorder.Push (this.redoRecorder.Pop ()).PlayBack ();
			}
		}

		private readonly Actions.Recorder undoRecorder;
		private readonly Actions.Recorder redoRecorder;

		private readonly GraphApplication application;
	}
}
