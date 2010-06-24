//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationCustomerViewController : CreationViewController<CustomerEntity>
	{
		public CreationCustomerViewController(string name, Entities.CustomerEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(Widgets.TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreatePanelTitleTile ("Data.Customer", "Client à créer...");

				var button1 = new ConfirmationButton
				{
					Text = ConfirmationButton.FormatContent ("Personne privée", "Crée un client de type personne privée"),
					PreferredHeight = 52,
				};

				var button2 = new ConfirmationButton
				{
					Text = ConfirmationButton.FormatContent ("Entreprise", "Crée un client de type entreprise"),
					PreferredHeight = 52,
				};
				
				builder.Add (button1);
				builder.Add (button2);
				
				builder.EndPanelTitleTile ();
			}
		}
	}
}
