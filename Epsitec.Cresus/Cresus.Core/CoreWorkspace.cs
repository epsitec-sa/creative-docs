//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public abstract class CoreWorkspace
	{
		public CoreWorkspace()
		{
		}

		public CoreApplication Application
		{
			get
			{
				return this.application;
			}
			internal set
			{
				System.Diagnostics.Debug.Assert (value != null);
				System.Diagnostics.Debug.Assert (this.application == null);

				this.application = value;
			}
		}

		
		public abstract void CreateUserInterface();


		private CoreApplication application;
	}
}
