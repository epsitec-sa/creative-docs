//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business.Finance;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public class IsrData
	{
		public IsrData(string subscriberNumber)
		{
			this.subscriberNumber  = Isr.GetCompactNumber (subscriberNumber);
			this.referenceNumber = IsrData.EmptyReferenceNumber;

			System.Diagnostics.Debug.Assert (Isr.IsCompactSubscriberNumber (this.SubscriberNumber));
		}

		public IsrData(string subscriberNumber, string referenceNumber)
		{
			this.subscriberNumber = Isr.GetCompactNumber (subscriberNumber);
			this.referenceNumber = Isr.GetCompactNumber (referenceNumber);

			System.Diagnostics.Debug.Assert (Isr.IsCompactSubscriberNumber (this.SubscriberNumber));
			System.Diagnostics.Debug.Assert (Isr.IsCompactReferenceNumber (this.ReferenceNumber));
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

		public bool								IsEmpty
		{
			get
			{
				return this.referenceNumber == IsrData.EmptyReferenceNumber;
			}
		}

		
		public static readonly string EmptyReferenceNumber = new string (' ', 27);

		
		public string GetFormattedReferenceNumber()
		{
			return Isr.GetFormattedReferenceNumber (this.referenceNumber);
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

		public string GetCodingZone(decimal amount, CurrencyCode currency, bool leaveAmountBlank = false)
		{
			//	Retourne le numéro complet au format "0100000106201>100000001668190000043332147+ 010619511>"

			string referenceNumber  = this.ReferenceNumber;
			string subscriberNumber = this.SubscriberNumber;

			if (this.IsEmpty)
			{
				return this.GetInvalidCodingZone ();
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			decimal value = Isr.GetRoundedAmount (amount, currency);
			IsrType type  = Isr.GetIsrType (currency, leaveAmountBlank);
			
			if (value <= 0)
			{
				string space10 = new string (' ', 10);
				buffer.Append (string.Format ("{0}{1:00}", space10, (int) type));
			}
			else
			{
				buffer.Append (string.Format ("{0:00}{1:0000000000}", (int) type, (long) (value * 100)));
			}

			buffer.Append (EsrHelper.ComputeControlKey (buffer.ToString ()));		//	13
			buffer.Append (">");													//	 1
			buffer.Append (referenceNumber);										//	27
			buffer.Append ("+ ");													//	 2
			buffer.Append (subscriberNumber);										//	 9
			buffer.Append (">");													//	 1

			string line = buffer.ToString ();

			System.Diagnostics.Debug.Assert (line.Length == 13+1+27+2+9+1);

			return line;
		}

		
		private readonly string subscriberNumber;
		private readonly string referenceNumber;
	}
}
