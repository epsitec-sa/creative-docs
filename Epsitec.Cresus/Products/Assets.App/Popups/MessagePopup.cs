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
		public MessagePopup(string title, string message)
		{
			this.title   = title;
			this.message = message;
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size (MessagePopup.popupWidth, MessagePopup.popupHeight);
			}
		}

		public override void CreateUI()
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
		public static void ShowAssetsDeleteEventWarning(Widget target)
		{
			var popup = new MessagePopup (
				"Avertissement",
				"Il n'est pas possible de supprimer un événement verrouillé.");

			popup.Create (target);
		}

		public static void ShowAssetsPreviewEventWarning(Widget target)
		{
			var popup = new MessagePopup (
				"Avertissement",
				"Le verrouillage n'est pas possible, car il existe des préamortissements.");

			popup.Create (target);
		}

		public static void ShowTodo(Widget target)
		{
			var popup = new MessagePopup ("Erreur", "Cette fonction n'est pas encore implémentée.");

			popup.Create (target);
		}

		public static void ShowPasteError(Widget target)
		{
			var popup = new MessagePopup ("Erreur", "Les données sont incompatibles.");

			popup.Create (target);
		}

		public static void ShowError(Widget target, string message)
		{
			var popup = new MessagePopup ("Erreur", message);

			popup.Create (target);
		}

		public static void ShowMessage(Widget target, string title, string message)
		{
			var popup = new MessagePopup (title, message);

			popup.Create (target);
		}

		public static void ShowMessage(Widget target, string message)
		{
			var popup = new MessagePopup ("Message", message);

			popup.Create (target);
		}
		#endregion


		private const int popupWidth  = 250;
		private const int popupHeight = 130;

		private readonly string					title;
		private readonly string					message;
	}
}