//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class UpdateManager
	{
		public UpdateManager(AbstractController controller)
		{
			this.controller = controller;
		}

		public void Clear()
		{
			this.UpdateArrayContent = false;
			this.UpdateArrayHeader = false;
		}

		public bool UpdateArrayContent
		{
			get;
			set;
		}

		public bool UpdateArrayHeader
		{
			get;
			set;
		}


		public void SetDirty()
		{
			this.controller.MainWindowController.SetDirty ();
		}


		private readonly AbstractController				controller;
	}
}
