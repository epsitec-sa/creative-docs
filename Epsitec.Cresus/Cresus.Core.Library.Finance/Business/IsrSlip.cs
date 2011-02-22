//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>IsrSlip</c> class implements the ISR slip coding zone
	/// (in French: ligne de codage des justificatifs BVR).
	/// </summary>
	/// <remarks>
	/// Spec. in French:  http://www.postfinance.ch/medialib/pf/fr/doc/consult/manual/dlserv/inpayslip_isr_man.Par.0001.File.pdf
	/// Spec. in English: http://www.postfinance.ch/medialib/pf/en/doc/consult/manual/dlserv/inpayslip_isr_man.Par.0001.File.pdf
	/// </remarks>
	public class IsrSlip
	{
		public IsrSlip(BillingDetailEntity billingDetails, bool optionalAmount = false)
		{
			var amount   = billingDetails.AmountDue.Amount;
			var currency = billingDetails.AmountDue.Currency.CurrencyCode;

			this.subscriberNumber = billingDetails.IsrDefinition.SubscriberNumber;
			this.referenceNumber  = billingDetails.IsrReferenceNumber;
			
			this.amount   = Isr.GetRoundedAmount (amount, currency);
			this.currency = currency;
			this.optionalAmount = optionalAmount;

			System.Diagnostics.Debug.Assert (this.currency == CurrencyCode.Chf || this.currency == CurrencyCode.Eur);
			
			System.Diagnostics.Debug.Assert (Isr.IsCompactSubscriberNumber (this.SubscriberNumber));
			System.Diagnostics.Debug.Assert (Isr.IsCompactReferenceNumber (this.ReferenceNumber));
			
			//	01-xxx-x => ISR in CHF
			//	03-xxx-x => ISR in EUR
			
			System.Diagnostics.Debug.Assert ((this.subscriberNumber.StartsWith ("01") && this.currency == CurrencyCode.Chf) ||
				/**/						 (this.subscriberNumber.StartsWith ("03") && this.currency == CurrencyCode.Eur)); 
		}
		
		public string							SubscriberNumber
		{
			get
			{
				return this.subscriberNumber;
			}
		}

		public string							ReferenceNumber
		{
			get
			{
				return this.referenceNumber;
			}
		}

		public decimal							Amount
		{
			get
			{
				return this.amount;
			}
		}

		public CurrencyCode						Currency
		{
			get
			{
				return this.currency;
			}
		}

		public IsrType							IsrType
		{
			get
			{
				return Isr.GetIsrType (this.currency, this.optionalAmount);
			}
		}

		public string GetFormattedReferenceNumber()
		{
			return Isr.FormatReferenceNumber (this.referenceNumber);
		}

		public string GetFormattedSubscriberNumber()
		{
			string s1 = this.subscriberNumber.Substring (0, 2);
			string s2 = this.subscriberNumber.Substring (2, 6);
			string s3 = this.subscriberNumber.Substring (8, 1);

			string s2clean = new string (s2.ToCharArray ().SkipWhile (c => c == '0').ToArray ());

			return string.Concat (s1, "-", s2clean, "-", s3);
		}

		public string GetInvalidCodingZone()
		{
			string spaces = new string (' ', 13+1+27+2);
			return string.Concat (spaces, this.SubscriberNumber, ">");
		}

		public string GetCodingZone()
		{
			//	Retourne le numéro complet au format "0100000106201>100000001668190000043332147+ 010619511>"

			string referenceNumber  = this.ReferenceNumber;
			string subscriberNumber = this.SubscriberNumber;

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			decimal value = this.amount;
			IsrType type  = this.IsrType;
			
			if (value <= 0)
			{
				string space10 = new string (' ', 10);
				buffer.Append (string.Format ("{0}{1:00}", space10, (int) type));
			}
			else
			{
				buffer.Append (string.Format ("{0:00}{1:0000000000}", (int) type, (long) (value * 100)));
			}

			buffer.Append (Isr.ComputeCheckDigit (buffer.ToString ()));		//	13
			buffer.Append (">");											//	 1
			buffer.Append (referenceNumber);								//	27
			buffer.Append ("+ ");											//	 2
			buffer.Append (subscriberNumber);								//	 9
			buffer.Append (">");											//	 1

			string line = buffer.ToString ();

			System.Diagnostics.Debug.Assert (line.Length == 13+1+27+2+9+1);

			return line;
		}

		
		private readonly string					subscriberNumber;
		private readonly string					referenceNumber;
		private readonly decimal				amount;
		private readonly CurrencyCode			currency;
		private readonly bool					optionalAmount;
	}
}
