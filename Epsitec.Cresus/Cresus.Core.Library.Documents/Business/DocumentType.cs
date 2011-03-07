//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum DocumentType
	{
		Unknown						=   -1,
		None						=    0,

		SalesQuote					=  100,				//	devis

		OrderConfiguration			=  110,				//	choix des options pour commande
		OrderBooking				=  120,				//	bon pour commande
		OrderConfirmation			=  130,				//	confirmation de commande

		ProductionOrder				=  200,				//	ordre de production
		ProductionChecklist			=  210,				//	check-liste de production

		ShipmentChecklist			=  300,				//	check-list d'expédition
		DeliveryNote				=  310,				//	bulletin de livraison


		Invoice						= 1000,				//	facture
		InvoiceProForma				= 1010,				//	facture pro forma

		PaymentReminder				= 1100,				//	rappel

		Receipt						= 1200,				//	reçu
		CreditMemo					= 1210,				//	note de crédit


		QuoteRequest				= 8000,				//	demande d'offre
		PurchaseOrder				= 8100,				//	confirmation de commande


		RelationSummary				= 10000,			//	résumé d'un client
		ArticleDefinitionSummary	= 10010,			//	résumé d'un article
		MailContactLabel			= 10100,			//	étiquette pour un client
	}
}