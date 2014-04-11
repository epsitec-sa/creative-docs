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
			var popup = new MessagePopup ("Avertissement", "Il n'est pas possible de supprimer un événement verrouillé.");
			popup.Create (target);
		}
		#endregion


		private const int popupWidth  = 250;
		private const int popupHeight = 100;

		private readonly string					title;
		private readonly string					message;
	}
}