//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Affiche un message.
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


		public static void ShowAssetsDeleteEventWarning(Widget target)
		{
			var popup = new MessagePopup ("Avertissement", "Il n'est pas possible de supprimer un événement verrouillé.");
			popup.Create (target);
		}


		private const int popupWidth  = 250;
		private const int popupHeight = 100;

		private readonly string					title;
		private readonly string					message;
	}
}