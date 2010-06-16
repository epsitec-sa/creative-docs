//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.CommandHandlers
{
	public class CoreCommandHandler : ICommandHandler
	{
		public CoreCommandHandler(CoreCommandDispatcher commandDispatcher)
		{
			this.commandDispatcher = commandDispatcher;
		}


		[Command (Res.CommandIds.Edition.SaveRecord)]
		public void ProcessSaveRecord(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			this.commandDispatcher.DispatchGenericCommand (e.Command);
		}

		[Command (Core.Res.CommandIds.Test.Crash)]
		public void TestCrash()
		{
			throw new System.Exception ("Crashing the application on purpose");
		}

		private readonly CoreCommandDispatcher commandDispatcher;
	}

}
