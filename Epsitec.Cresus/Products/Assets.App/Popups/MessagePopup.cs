//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Affiche un simple message. S'il est long, il est mise en page sur plusieurs lignes.
	/// </summary>
	public class MessagePopup : AbstractPopup
	{
		private MessagePopup(string title, string message, int? width, int? height)
		{
			this.title   = title;
			this.message = message;
			this.width   = width;
			this.height  = height;
		}


		protected override Size DialogSize
		{
			get
			{
				int w = this.width.HasValue  ? this.width.Value  : MessagePopup.popupWidth;
				int h = this.height.HasValue ? this.height.Value : MessagePopup.popupHeight;

				return new Size (w, h);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (this.title);
			this.CreateCloseButton ();
			this.CreateTreeTable ();
		}

		private void CreateTreeTable()
		{
			new StaticText
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
				Text   = this.message,
				Margins = new Margins (10),
			};
		}


		#region Helpers
		public static void ShowAssetsDeleteEventWarning(Widget target, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (
				Res.Strings.Popup.Message.WarningTitle.ToString (),
				Res.Strings.Popup.Message.DeleteEventWarning.Text.ToString (),
				width, height);

			popup.Create (target);
		}

		public static void ShowAssetsPreviewEventWarning(Widget target, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (
				Res.Strings.Popup.Message.WarningTitle.ToString (),
				Res.Strings.Popup.Message.PreviewEventWarning.Text.ToString (),
				width, height);

			popup.Create (target);
		}

		public static void ShowTodo(Widget target, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (
				Res.Strings.Popup.Message.ErrorTitle.ToString (),
				Res.Strings.Popup.Message.Todo.Text.ToString (),
				width, height);

			popup.Create (target);
		}

		public static void ShowPasteError(Widget target, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (
				Res.Strings.Popup.Message.ErrorTitle.ToString (),
				Res.Strings.Popup.Message.PasteError.Text.ToString (),
				width, height);

			popup.Create (target);
		}

		public static void ShowError(Widget target, string message, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (
				Res.Strings.Popup.Message.ErrorTitle.ToString (),
				message, width, height);

			popup.Create (target);
		}

		public static void ShowMessage(Widget target, string title, string message, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (title, message, width, height);

			popup.Create (target);
		}

		public static void ShowMessage(Widget target, string message, int? width = null, int? height = null)
		{
			var popup = new MessagePopup (
				Res.Strings.Popup.Message.MessageTitle.ToString (),
				message, width, height);

			popup.Create (target);
		}
		#endregion


		private const int popupWidth  = 250;
		private const int popupHeight = 130;

		private readonly string					title;
		private readonly string					message;
		private readonly int?					width;
		private readonly int?					height;
	}
}