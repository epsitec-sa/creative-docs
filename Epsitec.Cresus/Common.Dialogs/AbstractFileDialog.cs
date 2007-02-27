//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	public class AbstractFileDialog
	{
		static AbstractFileDialog()
		{
			Res.Initialize ();
		}

		public static readonly string NewEmptyDocument = "#NewEmptyDocument#";
	}
}
