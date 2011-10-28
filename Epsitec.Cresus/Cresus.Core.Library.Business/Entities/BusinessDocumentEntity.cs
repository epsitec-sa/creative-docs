//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class BusinessDocumentEntity : ICopyableEntity<BusinessDocumentEntity>
	{
		public bool IsExcludingTax
		{
			get
			{
				return this.BillingMode == BillingMode.ExcludingTax;
			}
		}

		public BillingMode BillingMode
		{
			get
			{
				if (this.PriceGroup.IsNotNull ())
				{
					return this.PriceGroup.BillingMode;
				}
				else
				{
					return Business.Finance.BillingMode.None;
				}
			}
		}


		public override FormattedText GetSummary()
		{
			var billToMailContact = this.BillToMailContact;
			var shipToMailContact = this.ShipToMailContact;

			FormattedText billing  = BusinessDocumentEntity.GetShortMailContactSummary (billToMailContact);
			FormattedText shipping = BusinessDocumentEntity.GetShortMailContactSummary (shipToMailContact);

			FormattedText addresses;
			if (billToMailContact == shipToMailContact || (!billToMailContact.IsNotNull () && !shipToMailContact.IsNotNull ()))
			{
				addresses = FormattedText.Concat ("\n<b>• Adresse de facturation et de livraison:</b>\n", billing);
			}
			else
			{
				addresses = FormattedText.Concat ("\n<b>• Adresse de facturation:</b>\n", billing, "\n\n<b>• Adresse de livraison:</b>\n", shipping);
			}

			return TextFormatter.FormatText
				(
					this.BillingDate, ", ",
					InvoiceDocumentHelper.GetTotalPriceTTC (this), TextFormatter.FormatCommand ("#price()"), "\n",
					addresses
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("???");//"Facture n°", this.IdA);
		}


		private static FormattedText GetShortMailContactSummary(MailContactEntity x)
		{
			if (x.IsNotNull ())
			{
				return TextFormatter.FormatText
					(
						x.LegalPerson.Name, "\n",
						x.NaturalPerson.Firstname, x.NaturalPerson.Lastname, "\n",
						x.Address.Street.StreetName, "\n",
						x.Address.Location.PostalCode, x.Address.Location.Name
					);
			}
			else
			{
				return TextFormatter.FormatText ("Pas encore défini").ApplyItalic ();
			}
		}


#if false
		public MailContactEntity FinalShipToMailContact
		{
			//	Retourne l'adresse de livraison à utiliser. Si aucune adresse n'est directement définie,
			//	on utilise l'adresse définie dans l'éventuel chantier.
			get
			{
				var shipToMailContact = this.ShipToMailContact;

				if (shipToMailContact.IsNull ())
				{
					// TODO: ATTENTION, il n'est pas autorisé d'obtenir le BusinessContext ainsi !!!
					var businessContext = Logic.Current.GetComponent<BusinessContext> ();

					var affair = this.GetAffair (businessContext);
					if (affair != null)
					{
						shipToMailContact = affair.AssociatedSite.Person.Contacts.OfType<MailContactEntity> ().FirstOrDefault ();
					}
				}

				return shipToMailContact;
			}
		}

		public AffairEntity GetAffair(BusinessContext businessContext)
		{
			//	Retourne l'affaire à laquelle appartient l'entité.
			var metadata = this.GetDocumentMetadata (businessContext);

			if (metadata == null)
			{
				return null;
			}

			var example = new AffairEntity ();
			example.Documents.Add (metadata);

			return businessContext.DataContext.GetByExample<AffairEntity> (example).FirstOrDefault ();
		}

		private DocumentMetadataEntity GetDocumentMetadata(BusinessContext businessContext)
		{
			//	Retourne le DocumentMetadataEntity auquel appartient l'entité.
			var example = new DocumentMetadataEntity ();
			example.BusinessDocument = this;

			return businessContext.DataContext.GetByExample<DocumentMetadataEntity> (example).FirstOrDefault ();
		}
#endif


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

		partial void OnPriceRefDateChanged(Date oldValue, Date newValue)
		{
			this.MarkAllArticleLinesDirty ();
		}

		partial void OnPriceGroupChanged(PriceGroupEntity oldValue, PriceGroupEntity newValue)
		{
			this.MarkAllArticleLinesDirty ();
		}

		private void MarkAllArticleLinesDirty()
		{
			foreach (var articleItem in this.Lines.OfType<ArticleDocumentItemEntity> ())
			{
				articleItem.ArticleAttributes |= ArticleDocumentItemAttributes.Dirty | ArticleDocumentItemAttributes.Reset;
			}
		}

		#region Concise lines algorithm

		public IList<AbstractDocumentItemEntity> GetConciseLines()
		{
			//	Retourne la liste des lignes d'un document commercial expurgée de toutes les
			//	lignes redondantes, telles que les sous-totaux inutiles.
			//	Le résultat n'est à utiliser que pour la production de documents !
			//	En effet, il n'est pas compatible avec la calculateur de prix, qui rajouterait
			//	certains sous-totaux enlevés !
			var conciseLines = new List<AbstractDocumentItemEntity> (this.Lines);

			//	Supprime tous les sous-totaux à double.
			int i = 0;
			while (i < conciseLines.Count)
			{
				var line0 = conciseLines[i] as SubTotalDocumentItemEntity;
				var line1 = BusinessDocumentEntity.GetNextActiveLine (conciseLines, i, 1) as SubTotalDocumentItemEntity;

				if (BusinessDocumentEntity.Similar (line0, line1))
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
				var line2 = BusinessDocumentEntity.GetNextActiveLine (conciseLines, i, -2);
				var line1 = BusinessDocumentEntity.GetNextActiveLine (conciseLines, i, -1);
				var line0 = conciseLines[i];

				//	Article unique suivi d'un sous-total superflu ?
				if ((line2 == null || !(line2 is ArticleDocumentItemEntity)) &&
						line1 is ArticleDocumentItemEntity &&
						line0 is SubTotalDocumentItemEntity &&
						BusinessDocumentEntity.HasEmptyDiscount (line0 as SubTotalDocumentItemEntity))
				{
					conciseLines.RemoveAt (i);  // supprime le sous-total
					continue;
				}

				i++;
			}

			return conciseLines;
		}

		private static AbstractDocumentItemEntity GetNextActiveLine(IList<AbstractDocumentItemEntity> lines, int index, int direction)
		{
			//	Cherche une ligne en avant ou en arrière, en ignorant les lignes de texte.
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

		private static bool Similar(SubTotalDocumentItemEntity line1, SubTotalDocumentItemEntity line2)
		{
			//	Retourne true si les 2 lignes de sous-totaux sont redondantes et équivalentes.
			if ((line1.IsNull ()) ||
				(line2.IsNull ()))
			{
				return false;
			}

			return BusinessDocumentEntity.HasEmptyDiscount (line1) &&
				   BusinessDocumentEntity.HasEmptyDiscount (line2) &&
				   line1.PriceBeforeTax2 == line2.PriceBeforeTax2 &&
				   line1.PriceAfterTax2  == line2.PriceAfterTax2;
		}

		private static bool HasEmptyDiscount(SubTotalDocumentItemEntity line)
		{
			//	Retourne true si la ligne de sous-total est redondante, c'est-à-dire si elle n'a pas de rabais.

			if (line.Discount.IsNull ())
			{
				return true;
			}

			if ((line.Discount.HasDiscountRate) ||
				(line.Discount.HasValue))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		#region ICopyableEntity<BusinessDocumentEntity> Members

		void ICopyableEntity<BusinessDocumentEntity>.CopyTo(IBusinessContext businessContext, BusinessDocumentEntity copy)
		{
			copy.VariantId             = this.VariantId;

			copy.BaseDocumentCode      = this.Code;
			copy.BillToMailContact     = this.BillToMailContact;
			copy.ShipToMailContact     = this.ShipToMailContact;
			copy.OtherPartyRelation    = this.OtherPartyRelation;
			copy.OtherPartyBillingMode = this.OtherPartyBillingMode;
			copy.OtherPartyTaxMode     = this.OtherPartyTaxMode;

			//	PaymentTransactions is not copied, since it is really specific to one invoice, and
			//	should not be shared between different invoices.

			copy.BillingStatus         = this.BillingStatus;
			copy.BillingDate           = this.BillingDate;
			copy.CurrencyCode          = this.CurrencyCode;
			copy.PriceRefDate          = this.PriceRefDate;
			copy.PriceGroup            = this.PriceGroup;
			copy.DebtorBookAccount     = this.DebtorBookAccount;

			//	The lines must be copied last, or else, the fact that we set the price group or the
			//	price reference date above would invalidate them all :
			
			copy.Lines.AddRange (this.Lines.Select (x => x.CloneEntity (businessContext)));
		}

		#endregion
	}
}
