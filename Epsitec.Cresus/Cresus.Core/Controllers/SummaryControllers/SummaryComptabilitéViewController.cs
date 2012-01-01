//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryComptabilitéViewController : SummaryViewController<ComptabilitéEntity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 200;
		}

		protected override void CreateBricks(BrickWall<ComptabilitéEntity> wall)
		{
			wall.AddBrick (x => x)
				.Icon ("Comptabilité")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController1)
				.Icon ("Comptabilité.Journal")
				.Title ("Journal des écritures")
				.Text (this.JournalTitle)
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController0)
				.Icon ("Comptabilité.PlanComptable")
				.Title ("Plan comptable")
				.Text (this.PlanComptableTitle)
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController2)
				.Icon ("Comptabilité.Balance")
				.Title ("Balance de vérification")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController3)
				.Icon ("Comptabilité.Compte")
				.Title ("Extrait de compte")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController4)
				.Icon ("Comptabilité.Bilan")
				.Title ("Bilan")
				.Text ("Actifs et passifs")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController5)
				.Icon ("Comptabilité.PP")
				.Title ("Pertes et Profits")
				.Text ("Charges et produits")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController6)
				.Icon ("Comptabilité.Exploitation")
				.Title ("Compte d'exploitation")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController7)
				.Icon ("Comptabilité.Budgets")
				.Title ("Budgets")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController8)
				.Icon ("Comptabilité.Change")
				.Title ("Différences de change")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController9)
				.Icon ("Comptabilité.RésuméPériodique")
				.Title ("Résumé périodique")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController10)
				.Icon ("Comptabilité.RésuméTVA")
				.Title ("Résumé TVA")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController11)
				.Icon ("Comptabilité.DécompteTVA")
				.Title ("Décompte TVA")
				.Text ("")
				;
		}

		private FormattedText PlanComptableTitle
		{
			get
			{
				int count = this.Entity.PlanComptable.Count;

				if (count == 0)
				{
					return TextFormatter.FormatText ("Aucun compte").ApplyItalic ();
				}
				else if (count == 1)
				{
					return "1 compte";
				}
				else
				{
					return TextFormatter.FormatText (count.ToString (), "comptes");
				}
			}
		}

		private FormattedText JournalTitle
		{
			get
			{
				int count = this.Entity.Journal.Count;

				if (count == 0)
				{
					return TextFormatter.FormatText ("Aucune écriture").ApplyItalic ();
				}
				else if (count == 1)
				{
					return "1 écriture";
				}
				else
				{
					return TextFormatter.FormatText (count.ToString (), "écritures");
				}
			}
		}
	}
}