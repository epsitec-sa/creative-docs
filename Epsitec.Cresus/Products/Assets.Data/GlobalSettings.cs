//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public class GlobalSettings
	{
		public GlobalSettings(UndoManager undoManager)
		{
			this.undoManager = undoManager;

			FolderItem item = FileManager.GetFolderItem (FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
			var filename = System.IO.Path.Combine (item.FullPath, "default" + IOHelpers.Extension);
			this.MandatFilename = filename;

			this.SaveMandatMode = SaveMandatMode.SaveUI | SaveMandatMode.KeepUnzip;
			this.CopyNameStrategy = CopyNameStrategy.NameBracketCopy;
		}


		public string							MandatFilename;
		public SaveMandatMode					SaveMandatMode;
		public CopyNameStrategy					CopyNameStrategy;


		private readonly UndoManager			undoManager;
	}
}
