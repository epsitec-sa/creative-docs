//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public static class Isr
	{
		public static string GetNewReferenceNumber()
		{
			return null;
		}


		public static bool IsCompactReferenceNumber(string number)
		{
			if ((number != null) &&
				(number.Length == 27) &&
				(number.ToCharArray ().All (c => c.IsDigit ())))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool IsCompactSubscriberNumber(string number)
		{
			if ((number != null) &&
				(number.Length == 9) &&
				(number.ToCharArray ().All (c => c.IsDigit ())))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string GetCompactNumber(string number)
		{
			if (number == null)
			{
				return number;
			}
			else
			{
				return new string (number.Where (c => c.IsDigit ()).ToArray ());
			}
		}

		private static bool IsDigit(this char c)
		{
			return (c >= '0') && (c <= '9');
		}

		/// <summary>
		/// Gets the type of the ISR.
		/// </summary>
		/// <param name="currency">The currency.</param>
		/// <param name="optionalAmount">If set to <c>true</c>, the amount may be left blank.</param>
		/// <returns></returns>
		public static IsrType GetIsrType(CurrencyCode currency, bool optionalAmount)
		{
			IsrType type;

			switch (currency)
			{
				case CurrencyCode.Chf:
					type  = optionalAmount ? IsrType.Code04_IsrPlus_Chf : IsrType.Code01_Isr_Chf;
					break;

				case CurrencyCode.Eur:
					type  = optionalAmount ? IsrType.Code31_IsrPlus_Eur : IsrType.Code21_Isr_Eur;
					break;

				default:
					type = IsrType.Invalid;
					break;
			}
			return type;
		}

		/// <summary>
		/// Gets the rounded amount, based on the currency. ISR must be rounded to 0.05 CHF
		/// for instance, whereas in EUR, there is no such requirement.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <param name="currency">The currency.</param>
		/// <returns>The rounded amount.</returns>
		public static decimal GetRoundedAmount(decimal amount, CurrencyCode currency)
		{
			switch (currency)
			{
				case CurrencyCode.Chf:
					return System.Math.Round (amount * 20) * 0.05M;

				case CurrencyCode.Eur:
					return System.Math.Round (amount * 100) * 0.01M;

				default:
					return amount;
			}
		}

		/// <summary>
		/// Gets the formatted reference number, such as <c>"96 13070 01000 02173 50356 73892"</c>.
		/// </summary>
		/// <param name="number">The compact number.</param>
		/// <returns>The formatted reference number.</returns>
		public static string GetFormattedReferenceNumber(string number)
		{
			System.Diagnostics.Debug.Assert (Isr.IsCompactReferenceNumber (number));

			string s1 = number.Substring (0, 2);
			string s2 = number.Substring (2, 5);
			string s3 = number.Substring (7, 5);
			string s4 = number.Substring (12, 5);
			string s5 = number.Substring (17, 5);
			string s6 = number.Substring (22, 5);

			return string.Concat (s1, " ", s2, " ", s3, " ", s4, " ", s5, " ", s6);
		}
	}
}
