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
	public class IdentityRibbon : AbstractRibbon
	{
		public IdentityRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = "Identité";
			this.PreferredWidth = 8 + 48;

			this.widget = new IdentityCardWidget(this);
			this.widget.Dock = DockStyle.Fill;
			this.widget.IdentityCard = this.designerApplication.Settings.IdentityCard;
			this.widget.Clicked += this.HandleIdentityClicked;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.widget.Clicked -= this.HandleIdentityClicked;
			}
			
			base.Dispose(disposing);
		}


		private void HandleIdentityClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton pour changer d'identité a été cliqué.
			IdentityCard nullCard = new IdentityCard("Anonyme", -1, null);
			List<IdentityCard> cards = new List<IdentityCard>(IdentityRepository.Default.IdentityCards);
			cards.Add(nullCard);
			
			IdentityCardSelectorDialog dialog = new IdentityCardSelectorDialog(cards);
			dialog.OwnerWindow = this.designerApplication.Window;
			dialog.ActiveIdentityCard = this.widget.IdentityCard ?? nullCard;
			dialog.OpenDialog();
			if (dialog.Result == Common.Dialogs.DialogResult.Accept)
			{
				IdentityCard card = dialog.ActiveIdentityCard;

				if (card == nullCard)
				{
					card = null;
				}
				
				this.designerApplication.Settings.IdentityCard = card;
				this.widget.IdentityCard = card;

				this.designerApplication.UpdateCommandEditLocked();

				if (this.designerApplication.CurrentModule != null)
				{
					this.designerApplication.CurrentModule.Modifier.ActiveViewer.UpdateCommands();
					this.designerApplication.CurrentModule.Modifier.ActiveViewer.Update();
				}
			}
		}
		

		private IdentityCardWidget widget;
	}
}
