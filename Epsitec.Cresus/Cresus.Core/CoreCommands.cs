//	Copyright © 2008-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreCommands</c> class implements the application wide commands.
	/// </summary>
	public sealed partial class CoreCommands
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreCommands"/> class.
		/// </summary>
		/// <param name="application">The application.</param>
		public CoreCommands(CoreApplication application)
		{
			this.application = application;
			this.application.CommandDispatcher.RegisterController (this);
		}


		[Command (Core.Res.CommandIds.Test.Crash)]
		public void TestCrash()
		{
			throw new System.Exception ("Crashing the application on purpose");
		}

		private readonly CoreApplication application;
	}
}
