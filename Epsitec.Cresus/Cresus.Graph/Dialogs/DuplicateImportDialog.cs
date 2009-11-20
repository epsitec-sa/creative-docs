using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Graph.Dialogs
{
	class DuplicateImportDialog
	{
		public static ImportOperation AskHowToImportDuplicateSource(GraphDataCube cube)
		{
			var dialog = new QuestionDialog (Res.Captions.Message.DataImport.WhatToDo,
				Res.Captions.Message.DataImport.WhatToDoMerge,
				Res.Captions.Message.DataImport.WhatToDoAdd,
				Res.Captions.Message.DataImport.WhatToDoCancel);

			dialog.OpenDialog ();

			switch (dialog.Result)
			{
				case Epsitec.Common.Dialogs.DialogResult.Answer1:
					return ImportOperation.Merge;
				case Epsitec.Common.Dialogs.DialogResult.Answer2:
					return ImportOperation.Add;
				case Epsitec.Common.Dialogs.DialogResult.Answer3:
					return ImportOperation.Cancel;
			}

			throw new System.NotImplementedException ();
		}


	}
}
