using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Dialogs
{
	public class AbstractFileDialog
	{
		static AbstractFileDialog()
		{
			Res.Initialize (typeof (AbstractFileDialog), "Common.Dialogs");
		}

		public static readonly string NewEmptyDocument = "#NewEmptyDocument#";
	}
}
