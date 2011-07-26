//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class BusinessDocumentEntity : ICopyableEntity<BusinessDocumentEntity>
	{
		public override FormattedText GetSummary()
		{
			FormattedText billing  = BusinessDocumentEntity.GetShortMailContactSummary (this.BillToMailContact);
			FormattedText shipping = BusinessDocumentEntity.GetShortMailContactSummary (this.ShipToMailContact);

			FormattedText addresses;
			if (this.BillToMailContact == this.ShipToMailContact || (!this.BillToMailContact.IsNotNull () && !this.ShipToMailContact.IsNotNull ()))
			{
				addresses = FormattedText.Concat ("\n<b>• Adresse de facturation et de livraison:</b>\n", billing);
			}
			else
			{
				addresses = FormattedText.Concat ("\n<b>• Adresse de facturation:</b>\n", billing, "\n\n<b>• Adresse de livraison:</b>\n", shipping);
			}

			return TextFormatter.FormatText (
				this.BillingDate, ", ",
				InvoiceDocumentHelper.GetTotalPriceTTC (this), TextFormatter.FormatCommand ("#price()"), "\n",
				addresses);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("???");//"Facture n°", this.IdA);
		}


		private static FormattedText GetShortMailContactSummary(MailContactEntity x)
		{
			if (x.IsNotNull ())
			{
				return TextFormatter.FormatText (x.LegalPerson.Name, "\n",
												 x.NaturalPerson.Firstname, x.NaturalPerson.Lastname, "\n",
												 x.Address.Street.StreetName, "\n",
												 x.Address.Location.PostalCode, x.Address.Location.Name);
			}
			else
			{
				return TextFormatter.FormatText ("Pas encore défini").ApplyItalic ();
			}
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				//a.Accumulate (this.IdA.GetEntityStatus ());
				//a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				//a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());

				//a.Accumulate (this.DocumentTitle.GetEntityStatus ());
				a.Accumulate (/*EntityStatus.Empty | */EntityStatus.Valid); // this.Description.GetEntityStatus ();
				a.Accumulate (this.Lines.Select (x => x.GetEntityStatus ()));
				//a.Accumulate (this.Comments.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
			}
		}


		public IList<AbstractDocumentItemEntity> ConciseLines
		{
			//	Retourne la liste des lignes d'un document commercial expurgée de toutes les
			//	lignes redondantes, telles que les sous-totaux inutiles.
			//	Le résultat n'est à utiliser que pour la production de documents !
			//	En effet, il n'est pas compatible avec la calculateur de prix, qui ajouterait
			//	certains sous-totaux enlevés !
			get
			{
				var conciseLines = new List<AbstractDocumentItemEntity> (this.Lines);

				//	Supprime tous les sous-totaux à double.
				int i = 0;
				while (i < conciseLines.Count)
				{
					var line1 = conciseLines[i] as SubTotalDocumentItemEntity;
					var line2 = BusinessDocumentEntity.GetNextActiveLine (conciseLines, i, 1) as SubTotalDocumentItemEntity;

					if (BusinessDocumentEntity.IsSimilarSubTotals (line1, line2))
					{
						conciseLines.RemoveAt (i);
						continue;
					}

					i++;
				}

				//	Supprime tous les sous-totaux inutiles. Par exemple, un sous-total sans rabais
				//	précédé d'un article unique est superflu.
				i = 0;
				while (i < conciseLines.Count)
				{
					var line1 = BusinessDocumentEntity.GetNextActiveLine (conciseLines, i, -1);
					var line2 = BusinessDocumentEntity.GetNextActiveLine (conciseLines, i, -2);
					var line3 = conciseLines[i];

					//	Article unique suivi d'un sous-total superflu ?
					if ((line1 == null || !(line1 is ArticleDocumentItemEntity)) &&
						line2 is ArticleDocumentItemEntity &&
						line3 is SubTotalDocumentItemEntity &&
						BusinessDocumentEntity.IsMiscSubTotal (line3 as SubTotalDocumentItemEntity))
					{
						conciseLines.RemoveAt (i);  // supprime le sous-total
						continue;
					}

					i++;
				}

				return conciseLines;
			}
		}

		private static bool IsSimilarSubTotals(SubTotalDocumentItemEntity line1, SubTotalDocumentItemEntity line2)
		{
			//	Retourne true si les 2 lignes de sous-totaux sont redondantes et équivalentes.
			if (line1 == null || line2 == null)
			{
				return false;
			}

			return BusinessDocumentEntity.IsMiscSubTotal (line1) &&
				   BusinessDocumentEntity.IsMiscSubTotal (line2) &&
				   line1.ResultingPriceBeforeTax.GetValueOrDefault (0) == line2.ResultingPriceBeforeTax.GetValueOrDefault (0) &&
				   line1.ResultingTax.GetValueOrDefault (0)            == line2.ResultingTax.GetValueOrDefault (0);
		}

		private static bool IsMiscSubTotal(SubTotalDocumentItemEntity line)
		{
			//	Retourne true si la ligne de sous-total est redondante, c'est-à-dire si elle n'a pas de rabais.
			if (line == null)
			{
				return false;
			}

			if (line.Discount.DiscountRate.HasValue ||
				line.Discount.Value.HasValue)
			{
				return false;
			}

			return true;
		}

		private static AbstractDocumentItemEntity GetNextActiveLine(IList<AbstractDocumentItemEntity> lines, int index, int direction)
		{
			while (true)
			{
				index += direction;

				if (index < 0 || index >= lines.Count ())
				{
					return null;
				}

				var line = lines[index];

				if (!(line is TextDocumentItemEntity))
				{
					return line;
				}
			}
		}


		#region ICopyableEntity<BusinessDocumentEntity> Members

		void ICopyableEntity<BusinessDocumentEntity>.CopyTo(IBusinessContext businessContext, BusinessDocumentEntity copy)
		{
			copy.BaseDocumentCode      = this.Code;
			copy.BillToMailContact     = this.BillToMailContact;
			copy.ShipToMailContact     = this.ShipToMailContact;
			copy.OtherPartyRelation    = this.OtherPartyRelation;
			copy.OtherPartyBillingMode = this.OtherPartyBillingMode;
			copy.OtherPartyTaxMode     = this.OtherPartyTaxMode;
			
			copy.Lines.AddRange (this.Lines.Select (x => x.CloneEntity (businessContext)));

			//	BillingDetails is not copied, since it is really specific to one invoice, and
			//	should not be shared between different invoices.

			copy.BillingStatus         = this.BillingStatus;
			copy.BillingDate           = this.BillingDate;
			copy.CurrencyCode          = this.CurrencyCode;
			copy.PriceRefDate          = this.PriceRefDate;
			copy.PriceGroup            = this.PriceGroup;
			copy.DebtorBookAccount     = this.DebtorBookAccount;
		}

		#endregion
	}
}
