//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public static class Isr
	{
		/// <summary>
		/// Gets a new reference number for an ISR.
		/// </summary>
		/// <param name="data">The <see cref="CoreData"/>.</param>
		/// <param name="subscriberNumber">The (compact) subscriber number.</param>
		/// <param name="bankPrefix">The optional bank prefix (6-12 digits).</param>
		/// <returns>A unique reference number for an ISR slip.</returns>
		public static string GetNewReferenceNumber(CoreData data, string subscriberNumber, string bankPrefix = null)
		{
			if (!Isr.IsCompactSubscriberNumber (subscriberNumber))
			{
				throw new System.ArgumentException ("Subscriber number is invalid");
			}

			var generator  = data.RefIdGeneratorPool.GetGenerator (Isr.GeneratorNamePrefix + subscriberNumber);
			var nextLongId = generator.GetNextId ();

			var refNumber  = nextLongId.ToString (System.Globalization.NumberFormatInfo.InvariantInfo);
			var refPrefix  = bankPrefix ?? "";
			var refZeroes  = new string ('0', 26 - (refNumber.Length + refPrefix.Length));

			var refLine    = string.Concat (refPrefix, refZeroes, refNumber);
			var checksum   = Isr.ComputeCheckDigit (refLine);

			return refLine + checksum;
		}

		public static string GetNewReferenceNumber(CoreData data, IsrDefinitionEntity isrDefinition)
		{
			if (isrDefinition.IsNull ())
			{
				return null;
			}
			else
			{
				return Isr.GetNewReferenceNumber (data, isrDefinition.SubscriberNumber, isrDefinition.BankReferenceNumberPrefix);
			}
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


		/// <summary>
		/// Computes the control key of <paramref name="number"/> using the modulo 10 recursive
		/// algorithm. The reference of this algorithm can be found in the DTA document at the
		/// paragraph C9.4.
		/// </summary>
		/// <param name="number">The number whose control key to compute.</param>
		/// <returns>The control key of number.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="number"/> is empty or contains non alpha-numeric characters.</exception>
		public static char ComputeCheckDigit(string number)
		{
			if (string.IsNullOrWhiteSpace (number))
			{
				throw new System.ArgumentException ("The provided string is empty.");
			}

			int report = 0;

			foreach (char digit in number)
			{
				if (digit.IsDigit ())
				{
					report = Isr.checkDigitTable[report, digit-'0'];
				}
			}

			return Isr.checkDigits[report];
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
		/// Gets the formatted subscriber number, such as <c>"01-069444-3"</c>.
		/// </summary>
		/// <param name="number">The compact number.</param>
		/// <returns>The formatted subscriber number.</returns>
		public static string GetFormattedSubscriberNumber(string number)
		{
			if (!Isr.IsCompactSubscriberNumber (number))
			{
				return number;
			}

			string s1 = number.Substring (0, 2);
			string s2 = number.Substring (2, 6);
			string s3 = number.Substring (8, 1);

			return string.Concat (s1, "-", s2, "-", s3);
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


		#region ISR ComputeCheckDigit Constants

		/// <summary>
		/// This bidimensional <see cref="Array"/> is the table used by the modulo 10 recursive
		/// algorithm to find the next report digit at each step.
		/// </summary>
		private static readonly int[,] checkDigitTable = new int[,]
		{
			{0, 9, 4, 6, 8, 2, 7, 1, 3, 5},
			{9, 4, 6, 8, 2, 7, 1, 3, 5, 0},
			{4, 6, 8, 2, 7, 1, 3, 5, 0, 9},
			{6, 8, 2, 7, 1, 3, 5, 0, 9, 4},
			{8, 2, 7, 1, 3, 5, 0, 9, 4, 6},
			{2, 7, 1, 3, 5, 0, 9, 4, 6, 8},
			{7, 1, 3, 5, 0, 9, 4, 6, 8, 2},
			{1, 3, 5, 0, 9, 4, 6, 8, 2, 7},
			{3, 5, 0, 9, 4, 6, 8, 2, 7, 1},
			{5, 0, 9, 4, 6, 8, 2, 7, 1, 3},
		};

		/// <summary>
		/// This <see cref="Array"/> contains the control keys used by the modulo 10 recursive
		/// algorithm.
		/// </summary>
		private static readonly char[] checkDigits = new char[]
		{
			'0', '9', '8', '7', '6', '5', '4', '3', '2', '1'
		};

		#endregion

		private static bool IsDigit(this char c)
		{
			return (c >= '0') && (c <= '9');
		}

		private static readonly string GeneratorNamePrefix = "ISR.Ref.";
	}
}
