//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public class GlobalSettings
	{
		public GlobalSettings(UndoManager undoManager)
		{
			this.undoManager = undoManager;

			this.CopyNameStrategy = CopyNameStrategy.NameBracketCopy;
		}


		public string							MandatFilename = "C:\\Users\\Daniel\\Documents\\toto.assets";
		public CopyNameStrategy					CopyNameStrategy;


		private readonly UndoManager			undoManager;
	}
}
