﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The class <c>BvHelper</c> contains static methods used to check or build bv values.
	/// </summary>
	static class BvHelper
	{

		/// <summary>
		/// This bidimensional <see cref="Array"/> is the table used by the modulo 10 recursive
		/// algorithm to find the next report digit at each step.
		/// </summary>
		private static readonly int[,] controlKeyTable = new int[,]
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
		private static readonly char[] controlKeys = new char[]
		{
			'0', '9', '8', '7', '6', '5', '4', '3', '2', '1'
		};

		/// <summary>
		/// This <see cref="Dictionnary"/> is used to convert a <see cref="char"/> to an <see cref="int"/>
		/// during the iban validation process.
		/// </summary>
		private readonly static Dictionary<string, string> charConversionTable = new Dictionary<string, string>
		{
			{"A", "10"},
			{"B", "11"},
			{"C", "12"},
			{"D", "13"},
			{"E", "14"},
			{"F", "15"},
			{"G", "16"},
			{"H", "17"},
			{"I", "18"},
			{"J", "19"},
			{"K", "20"},
			{"L", "21"},
			{"M", "22"},
			{"N", "23"},
			{"O", "24"},
			{"P", "25"},
			{"Q", "26"},
			{"R", "27"},
			{"S", "28"},
			{"T", "29"},
			{"U", "30"},
			{"V", "31"},
			{"W", "32"},
			{"X", "33"},
			{"Y", "34"},
			{"Z", "35"},
		};


		/// <summary>
		/// Computes the control key of <paramref name="number"/> using the modulo 10 recursive
		/// algorithm. The reference of this algorithm can be found in the DTA document at the
		/// paragraph C9.4.
		/// </summary>
		/// <param name="number">The number whose control key to compute.</param>
		/// <returns>The control key of number.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="number"/> is empty or contains non alpha-numeric characters.</exception>
		public static char ComputeControlKey(string number)
		{
			if (!Regex.IsMatch (number, @"^[0-9]*$"))
			{
				throw new System.ArgumentException (string.Format("The provided string contains non alpah-numeric characters: {0}", number));
			}

			if (number.Length == 0)
			{
				throw new System.ArgumentException ("The provided string is empty.");
			}

			int report = 0;

			while (number.Length > 0)
			{
				int digit = int.Parse (number.Substring (0, 1), CultureInfo.InvariantCulture);
				report = BvHelper.controlKeyTable[report, digit];
				number = number.Substring (1);
			}

			return BvHelper.controlKeys[report];
		}

		/// <summary>
		/// Checks that <paramref name="iban"/> is a valid iban using the proper algorithm. The
		/// reference of this algorithm can be found in the DTA document at the paragraph C10.
		/// </summary>
		/// <param name="iban">The iban to check.</param>
		/// <returns>A <see cref="bool"/> indicating whether <paramref name="iban"/> is valid or not.</returns>
		public static bool CheckIban(string iban)
		{
			bool valid;
			
			iban = iban.Replace(" ", "");
			
			if (!Regex.IsMatch (iban, "^[A-Z0-9]*$") || iban.Length < 15 || iban.Length > 34)
			{
				valid = false;
			}
			else
			{
				iban = string.Format ("{0}{1}", iban.Substring (4), iban.Substring (0, 4));

				foreach (string s in BvHelper.charConversionTable.Keys)
				{
					iban = iban.Replace (s, BvHelper.charConversionTable[s]);
				}

				string remainder = "";

				while (iban.Length > 0)
				{
					int splitIndex = 9 - remainder.Length;
					string tmp;

					if (splitIndex < iban.Length)
					{
						tmp = string.Format ("{0}{1}", remainder, iban.Substring (0, splitIndex));
						iban = iban.Substring (splitIndex);
					}
					else
					{
						tmp = string.Format ("{0}{1}", remainder, iban);
						iban = "";
					}

					remainder = (int.Parse (tmp, CultureInfo.InvariantCulture) % 97).ToString ();
				}

				valid = (remainder == "1");
			}

			return valid;
		}

		/// <summary>
		/// Checks that <paramref name="address"/> is a valid bank address.
		/// </summary>
		/// <param name="address">The bank address to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="address"/> is valid or not.</returns>
		public static bool CheckBankAddress(string address)
		{
			string[] lines = address.Split ('\n');

			return lines.Length <= 2
				&& lines.All (line => line.Length <= 27);
		}

		/// <summary>
		/// Checks that <paramref name="iban"/> is a valid beneficiary iban.
		/// </summary>
		/// <remarks>
		/// You have to normalize the iban using the BuildNormalizedIban method.
		/// </remarks>
		/// <param name="iban">The beneficiary <paramref name="iban"/> to check.</param>
		/// <returns>A <see cref="bool"/> indicating if iban is valid or not.</returns>
		public static bool CheckBeneficiaryIban(string iban)
		{
			return BvHelper.CheckIban (iban);
		}

		/// <summary>
		/// Checks that <paramref name="address"/> is a valid beneficiary address.
		/// </summary>
		/// <param name="address">The beneficiary address to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="address"/> is valid or not.</returns>
		public static bool CheckBeneficiaryAddress(string address)
		{
			string[] lines = address.Split ('\n');

			return lines.Length <= 4
				&& lines.All (line => line.Length <= 27)
				&& address.Trim ().Length > 0;
		}

		/// <summary>
		/// Checks that <paramref name="account"/> is a valid bank account.
		/// </summary>
		/// <param name="account">The bank account to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="account"/> is valid or not.</returns>
		public static bool CheckBankAccount(string account)
		{
			return Regex.IsMatch (account, @"^\d+-\d+-\d$");
		}

		/// <summary>
		/// Checks that <paramref name="layoutCode"/> is a valid layout code.
		/// </summary>
		/// <param name="layoutCode">The layout code to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="layoutCode"/> is valid.</returns>
		public static bool CheckLayoutCode(string layoutCode)
		{
			return (layoutCode == "303");
		}

		/// <summary>
		/// Checks that <paramref name="reason"/> is a valid reason of transfer.
		/// </summary>
		/// <param name="reason">The reason to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="reason"/> is valid or not.</returns>
		public static bool CheckReason(string reason)
		{
			string[] lines = reason.Split ('\n');
			return lines.Length <= 3
				&& lines.All (line => line.Length <= 10);
		}

		/// <summary>
		/// Checks that <paramref name="number"/> is a valid reference client number.
		/// </summary>
		/// <param name="number">The reference client number to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="number"/> is valid or not.</returns>
		public static bool CheckReferenceClientNumber(string number)
		{
			return Regex.IsMatch (number, @"^\d{10}$");
		}

		/// <summary>
		/// Checks that <paramref name="constant"/> is a valid clearing constant.
		/// </summary>
		/// <param name="constant">The clearing constant to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="constant"/> is valid or not.</returns>
		public static bool CheckClearingConstant(string constant)
		{
			return (constant == "07");
		}

		/// <summary>
		/// Checks that <paramref name="clearing"/> is a valid bank clearing number.
		/// </summary>
		/// <param name="clearing">The bank clearing number to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="clearing"/> is valid or not.</returns>
		public static bool CheckClearingBank(string clearing)
		{
			return Regex.IsMatch (clearing, @"^\d{5}$");
		}

		/// <summary>
		/// Checks that <paramref name="key"/> is a valid bank clearing number control key.
		/// </summary>
		/// <param name="key">The bank clearing number key to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="key"/> is valid or not.</returns>
		public static bool CheckClearingBankKey(string key)
		{
			return Regex.IsMatch (key, @"^\d{1}$");
		}

		/// <summary>
		/// Checks that <paramref name="ccp"/> is a valid CCP number.
		/// </summary>
		/// <param name="ccp">The CCP number to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="ccp"/> is valid or not.</returns>
		public static bool CheckCcpNumber(string ccp)
		{
			return Regex.IsMatch (ccp, @"^\d{9}$");
		}

		/// <summary>
		/// Checks that the values required to build the reference line are valid.
		/// </summary>
		/// <param name="iban">The beneficiary iban.</param>
		/// <param name="reference">The reference client number.</param>
		/// <returns>A <see cref="bool"/> indicating if the values required to build the reference line are valid or not.</returns>
		public static bool CheckReferenceLine(string iban, string reference)
		{
			return BvHelper.CheckBeneficiaryIban (iban)
				&& BvHelper.CheckReferenceClientNumber (reference);
		}

		/// <summary>
		/// Checks that the values required to build the clearing line are valid.
		/// </summary>
		/// <param name="constant">The clearing constant.</param>
		/// <param name="clearing">The bank clearing number.</param>
		/// <param name="key">The bank clearing number control key.</param>
		/// <returns>A <see cref="bool"/> indicating if the values required to build the clearing line are valid or not.</returns>
		public static bool CheckClearingLine(string constant, string clearing, string key)
		{
			return BvHelper.CheckClearingConstant (constant)
				&& BvHelper.CheckClearingBank (clearing)
				&& BvHelper.CheckClearingBankKey (key);
		}

		/// <summary>
		/// Checks that all the values contained in <paramref name="bvWidget"/> are valid.
		/// </summary>
		/// <param name="bvWidget">The <see cref="BvWidget"/> whose values to check.</param>
		/// <returns>A <see cref="bool"/> indicating if all the values contained in <paramref name="bvWidget"/> are valid or not.</returns>
		public static bool CheckBv(BvWidget bvWidget)
		{
			return BvHelper.CheckBankAddress (bvWidget.BankAddress)
				&& BvHelper.CheckBeneficiaryIban (bvWidget.BeneficiaryIban)
				&& BvHelper.CheckBeneficiaryAddress (bvWidget.BeneficiaryAddress)
				&& BvHelper.CheckBankAccount (bvWidget.BankAccount)
				&& BvHelper.CheckLayoutCode (bvWidget.LayoutCode)
				&& BvHelper.CheckReason (bvWidget.Reason)
				&& BvHelper.CheckReferenceClientNumber (bvWidget.ReferenceClientNumber)
				&& BvHelper.CheckClearingConstant (bvWidget.ClearingConstant)
				&& BvHelper.CheckClearingBank (bvWidget.ClearingBank)
				&& BvHelper.CheckClearingBankKey (bvWidget.ClearingBankKey)
				&& BvHelper.CheckCcpNumber (bvWidget.CcpNumber);
		}

		/// <summary>
		/// Builds the text of the reference line based on the given values.
		/// </summary>
		/// <param name="iban">The beneficiary iban.</param>
		/// <param name="reference">The client reference number.</param>
		/// <returns>The text of the reference line.</returns>
		/// <exception cref="System.ArgumentException">If the provided values are not valid.</exception>
		public static string BuildReferenceLine(string iban, string reference)
		{
			if (!BvHelper.CheckReferenceLine (iban, reference))
			{
				string message = string.Format ("One of the provided argument is not valid. Iban: {0}. Reference: {1}.", iban, reference);
				throw new System.ArgumentException (message);
			}

			iban =  Regex.Replace (iban, @"\s", "");
			string line = string.Format ("{0}{1}{2}", reference, "0000", iban.Substring (iban.Length - 12, 12));

			return string.Format ("{0}{1}+", line, BvHelper.ComputeControlKey (line));
		}

		/// <summary>
		/// Builds the text of the clearing line based on the given values.
		/// </summary>
		/// <param name="constant">The clearing constant.</param>
		/// <param name="clearing">The bank clearing number.</param>
		/// <param name="key">The bank clearing number control key.</param>
		/// <returns>The text of the clearing line.</returns>
		/// <exception cref="System.ArgumentException">If the provided values are not valid.</exception>
		public static string BuildClearingLine(string constant, string clearing, string key)
		{
			if (!BvHelper.CheckClearingLine (constant, clearing, key))
			{
				string message = string.Format ("One of the provided argument is not valid. Constant: {0}. Clearing: {1}. Key: {2}.", constant, clearing, key);
				throw new System.ArgumentException (message);
			}

			string line = string.Format ("{0}{1}{2}", constant, clearing, key);
			return string.Format ("{0}{1}>", line, BvHelper.ComputeControlKey (line));
		}

		/// <summary>
		/// Builds the text of the CCP number line based on the given value.
		/// </summary>
		/// <param name="ccp">The CCP number.</param>
		/// <returns>The text of the CCP number line.</returns>
		/// <exception cref="System.ArgumentException">If the provided value is not valid.</exception>
		public static string BuildCcpNumberLine(string ccp)
		{
			if (!BvHelper.CheckCcpNumber (ccp))
			{
				throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", ccp));
			}

			return string.Format ("{0}>", ccp);
		}

		/// <summary>
		/// Normalizes <paramref name="iban"/> in the format "CH00 0000 0000 0000 0000 0".
		/// </summary>
		/// <param name="iban">The iban to normalize.</param>
		/// <returns>The normalized version of <paramref name="iban"/>.</returns>
		public static string BuildNormalizedIban(string iban)
		{
			string normalizedIban = Regex.Replace (iban, @"\s", "");

			for (int i = 4; i < normalizedIban.Length && i < 27; i += 5)
			{
				normalizedIban = normalizedIban.Insert (i, " ");
			}

			normalizedIban = normalizedIban.ToUpperInvariant ();

			return normalizedIban;
		}

		/// <summary>
		/// Normalizes <paramref name="reason"/> so that each line has at least 10 chars on it.
		/// </summary>
		/// <param name="reason">The reason to normalize.</param>
		/// <returns>The normalized vestion of <paramref name="reason"/>.</returns>
		public static string BuildNormalizedReason(string reason)
		{
			string normalizedReason = "";

			foreach (string line in reason.Split ('\n'))
			{
				for (int i = 0; i * 10 < line.Length; i++)
				{
					int index = i * 10;
					int length = System.Math.Min (10, line.Length - i * 10);

					normalizedReason = string.Format("{0}\n{1}", normalizedReason, line.Substring (index, length));
				}
			}

			return normalizedReason.Trim ();
		}


	}

}
