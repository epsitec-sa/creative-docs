//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour l'ensemble des réglages globaux.
	/// </summary>
	public class FeedbackDialog : CoreDialog, ISettingsDialog
	{
		public FeedbackDialog(CoreApp application)
			: base (application)
		{
		}


		#region ISettingsDialog Members

		CoreData ISettingsDialog.Data
		{
			get
			{
				return this.application.FindComponent<CoreData> ();
			}
		}

		Window ISettingsDialog.DefaultOwnerWindow
		{
			get
			{
				return this.application.Window;
			}
		}

		#endregion
		
		protected override void SetupWindow(Window window)
		{
			window.Text = "Envoyer un commentaire";
			window.ClientSize = new Size (600, 400);
		}

		protected override void SetupWidgets(Window window)
		{
			var frame = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
			};

			new StaticText
			{
				Parent = frame,
				Text = "Message ou remarque sur le logiciel, à l'attention d'EPSITEC :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 2),
			};

			this.feedbackField = new TextFieldMulti
			{
				Parent = frame,
				Dock = DockStyle.Fill,
			};

			this.cancelButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Cancel,
				Parent = footer,
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Ok,
				Parent = footer,
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

			this.feedbackField.Focus ();
		}


		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Cancel)]
		private void ExecuteCancelCommand()
		{
			if (this.cancelButton.Enable)
			{
				this.CloseAndRejectChanges ();
			}
		}

		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Ok)]
		private void ExecuteOkCommand()
		{
			if (this.acceptButton.Enable)
			{
				this.CloseAndAcceptChanges ();
			}
		}

		private void CloseAndAcceptChanges()
		{
			var text = this.feedbackField.FormattedText;

			Epsitec.Cresus.Core.Library.Business.CoreSnapshotService.NotifyUserMessage (text.ToSimpleText ());

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		private void CloseAndRejectChanges()
		{
			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}


		private TextFieldMulti					feedbackField;
		private Button							acceptButton;
		private Button							cancelButton;
	}
}
