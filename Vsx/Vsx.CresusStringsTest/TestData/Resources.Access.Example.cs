// Paste this code in App.CresusGraph/Controllers/WorkspaceController.cs for functional testing

		private static void FOO(params object[] values)
		{
		}

		private static void TEST()
		{
			var p1 = Res.Strings.Message /* joli message */
				.MoreThanPiccolo;
			var p2 = Res.Strings.Message.MoreThanPiccolo.ToString ();
			var p3 = Cresus.Res.Strings.Message.MoreThanPiccolo;
			var p4 = Epsitec.Cresus.Res.Strings.Message.MoreThanPiccolo;

			var w1 = Common.Widgets.Res.Strings.Dialog.Button.Cancel;
			var w2 = Epsitec.Common.Widgets.Res.Strings.Dialog.Button.Cancel;

			var w3 = Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Open.File;

			FOO (Res.Strings.Message.MoreThanPiccolo
				.ToString ());
			FOO (Res
				.Strings
				.Message
				.MoreThanPiccolo
				.ToString ());
			FOO (Res.Strings.Message.MoreThanPiccolo, 57);
		}
