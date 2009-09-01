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
			this.application = application;
			this.application.CommandDispatcher.RegisterController (this);
		}


		[Command(ApplicationCommands.Id.Undo)]
		private void UndoCommand()
		{
			this.application.Document.Undo ();
		}

		[Command (ApplicationCommands.Id.Redo)]
		private void RedoCommand()
		{
			this.application.Document.Redo ();
		}

		[Command (Res.CommandIds.File.ExportImage)]
		private void ExportImageCommand()
		{
			//-this.application.Document.ExportImage ();
		}

		[Command (ApplicationCommands.Id.Copy)]
		private void CopyCommand()
		{
			this.application.Document.ExportImage ();
		}


		private readonly GraphApplication application;
	}
}
