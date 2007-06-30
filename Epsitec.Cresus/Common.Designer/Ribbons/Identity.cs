using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Identity;
using Epsitec.Common.Identity.UI;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Identity correspond aux commandes de choix d'identité.
	/// </summary>
	public class Identity : Abstract
	{
		public Identity(MainWindow mainWindow) : base(mainWindow)
		{
			this.Title = "Identité";
			this.PreferredWidth = 8 + 48;

			this.widget = new IdentityCardWidget(this);
			this.widget.Dock = DockStyle.Fill;
			this.widget.IdentityCard = this.mainWindow.Settings.IdentityCard;
			this.widget.Clicked += new MessageEventHandler(this.HandleIdentityClicked);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.widget.Clicked -= new MessageEventHandler(this.HandleIdentityClicked);
			}
			
			base.Dispose(disposing);
		}


		private void HandleIdentityClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour changer d'identité a été cliqué.
			IdentityCard nullCard = new IdentityCard("-", -1, null);
			List<IdentityCard> cards = new List<IdentityCard>(IdentityRepository.Default.IdentityCards);
			cards.Add(nullCard);
			
			IdentityCardSelectorDialog dialog = new IdentityCardSelectorDialog(cards);
			dialog.Owner = this.mainWindow.Window;
			dialog.ActiveIdentityCard = this.widget.IdentityCard ?? nullCard;
			dialog.OpenDialog();
			if (dialog.Result == Common.Dialogs.DialogResult.Accept)
			{
				IdentityCard card = dialog.ActiveIdentityCard;

				if (card == nullCard)
				{
					card = null;
				}
				
				this.mainWindow.Settings.IdentityCard = card;
				this.widget.IdentityCard = card;
			}
		}
		

		private IdentityCardWidget widget;
	}
}
