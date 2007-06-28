using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Identity;
using Epsitec.Common.Identity.UI;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Identity correspond aux commandes de choix d'identité.
	/// </summary>
	public class Identity : Abstract
	{
		public Identity(MainWindow mainWindow)
			: base (mainWindow)
		{
			this.Title = "Identité";
			this.PreferredWidth = 8 + 48;

			this.widget = new IdentityCardWidget (this);
			this.widget.Dock = DockStyle.Fill;
			this.widget.IdentityCard = this.mainWindow.Settings.IdentityCard;
			this.widget.Clicked +=
				delegate
				{
					IdentityCardSelectorDialog dialog = new IdentityCardSelectorDialog (IdentityRepository.Default.IdentityCards);
					dialog.Owner = this.mainWindow.Window;
					dialog.ActiveIdentityCard = this.widget.IdentityCard;
					dialog.OpenDialog ();
					if (dialog.Result == Epsitec.Common.Dialogs.DialogResult.Accept)
					{
						IdentityCard card = dialog.ActiveIdentityCard;
						
						this.mainWindow.Settings.IdentityCard = card;
						this.widget.IdentityCard = card;
					}
				};
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		private IdentityCardWidget widget;
	}
}
