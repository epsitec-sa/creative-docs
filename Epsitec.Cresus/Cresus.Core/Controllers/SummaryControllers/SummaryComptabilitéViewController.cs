//	Copyright � 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class SummaryComptabilit�ViewController : SummaryViewController<Comptabilit�Entity>
	{
		public override double GetPreferredWidth(int columnIndex, int columnCount)
		{
			return 200;
		}

		protected override void CreateBricks(BrickWall<Comptabilit�Entity> wall)
		{
			wall.AddBrick (x => x)
				.Icon ("Comptabilit�")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController1)
				.Icon ("Comptabilit�.Journal")
				.Title ("Journal des �critures")
				.Text (this.JournalTitle)
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController0)
				.Icon ("Comptabilit�.PlanComptable")
				.Title ("Plan comptable")
				.Text (this.PlanComptableTitle)
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController2)
				.Icon ("Comptabilit�.Balance")
				.Title ("Balance de v�rification")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController3)
				.Icon ("Comptabilit�.Compte")
				.Title ("Extrait de compte")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController4)
				.Icon ("Comptabilit�.Bilan")
				.Title ("Bilan")
				.Text ("Actifs et passifs")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController5)
				.Icon ("Comptabilit�.PP")
				.Title ("Pertes et Profits")
				.Text ("Charges et produits")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController6)
				.Icon ("Comptabilit�.Exploitation")
				.Title ("Compte d'exploitation")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController7)
				.Icon ("Comptabilit�.Budgets")
				.Title ("Budgets")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController8)
				.Icon ("Comptabilit�.Change")
				.Title ("Diff�rences de change")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController9)
				.Icon ("Comptabilit�.R�sum�P�riodique")
				.Title ("R�sum� p�riodique")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController10)
				.Icon ("Comptabilit�.R�sum�TVA")
				.Title ("R�sum� TVA")
				.Text ("")
				;
			wall.AddBrick ()
				.Attribute (BrickMode.SpecialController11)
				.Icon ("Comptabilit�.D�compteTVA")
				.Title ("D�compte TVA")
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
					return TextFormatter.FormatText ("Aucune �criture").ApplyItalic ();
				}
				else if (count == 1)
				{
					return "1 �criture";
				}
				else
				{
					return TextFormatter.FormatText (count.ToString (), "�critures");
				}
			}
		}
	}
}