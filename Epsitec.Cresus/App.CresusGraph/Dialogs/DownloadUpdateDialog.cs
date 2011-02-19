//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Graph.Dialogs
{
	public static class DownloadUpdateDialog
	{
		public static bool AskDownloadConfirmation(VersionChecker checker)
		{
			var dialog = new QuestionDialog (Res.Captions.Message.Update.Question,
				Res.Captions.Message.Update.Option1DownloadAndInstall,
				Res.Captions.Message.Update.Option2Cancel);

			dialog.DefineArguments (
				VersionChecker.Format ("#.#.###", checker.CurrentVersion),
				VersionChecker.Format ("#.#.###", checker.NewerVersion),
				checker.NewerVersionUrl);

			dialog.OpenDialog ();

			switch (dialog.Result)
			{
				case Epsitec.Common.Dialogs.DialogResult.Answer1:
					return true;
				
				case Epsitec.Common.Dialogs.DialogResult.Answer2:
					return false;
			}

			throw new System.NotImplementedException ();
		}


	}
}
