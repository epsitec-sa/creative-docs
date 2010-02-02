//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epsitec.App.BanquePiguet
{

	static class BvHelper
	{

		/// <summary>
		/// This bidimensional array is the table used by the modulo 10 recursive algorithm
		/// to find the next report digit at each step.
		/// </summary>
		private static int[,] table = new int[,]
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
		/// This array contains the control keys used by the modulo 10 recursive algorithm.
		/// </summary>
		private static char[] keys = new char[]
		{
			'0', '9', '8', '7', '6', '5', '4', '3', '2', '1'
		};


		/// <summary>
		/// Computes the control key of number using the modulo 10 recursive algorithm.
		/// </summary>
		/// <param name="number">The number whose control key to compute.</param>
		/// <returns>The control key of number.</returns>
		/// <exception cref="System.ArgumentException">Thrown when the given string is empty or contains non numeric characters.</exception>
		public static char Compute(string number)
		{
			if (!Regex.IsMatch (number, @"^\d*$"))
			{
				throw new System.ArgumentException ("The provided string contains non numeric characters");
			}

			if (number.Length == 0)
			{
				throw new System.ArgumentException ("The provided string is empty");
			}

			int report = 0;

			while (number.Length > 0)
			{
				int digit = int.Parse (number.Substring (0, 1), CultureInfo.InvariantCulture);
				report = BvHelper.table[report, digit];
				number = number.Substring (1);
			}

			return BvHelper.keys[report];
		}

		public static bool CheckBankAddress(string address)
		{
			string[] lines = address.Split ('\n');
			return (lines.Length <= 2) && lines.All (line => line.Length <= 27);
		}

		public static bool CheckBeneficiaryIban(string iban)
		{
			return Regex.IsMatch (iban, @"^CH\d{2} \d{4} \d{4} \d{4} \d{4} \d{1}$");
		}

		public static bool CheckBeneficiaryAddress(string address)
		{
			string[] lines = address.Split ('\n');
			return (lines.Length <= 4) && lines.All (line => line.Length <= 27);
		}

		public static bool CheckBankAccount(string account)
		{
			return Regex.IsMatch (account, @"^\d+-\d+-\d$");
		}

		public static bool CheckLayoutCode(string layoutCode)
		{
			return (layoutCode == "303");
		}

		public static bool CheckReason(string reason)
		{
			string[] lines = reason.Split ('\n');
			return (lines.Length <= 3) && lines.All (line => line.Length <= 10);
		}

		public static bool CheckReferenceClientNumber(string number)
		{
			return Regex.IsMatch (number, @"^\d{10}$");
		}

		public static bool CheckClearingConstant(string constant)
		{
			return (constant == "07");
		}

		public static bool CheckClearingBank(string clearing)
		{
			return Regex.IsMatch (clearing, @"^\d{5}$");
		}

		public static bool CheckClearingBankKey(string key)
		{
			return Regex.IsMatch (key, @"^\d{1}$");
		}

		public static bool CheckCcpNumber(string ccp)
		{
			return Regex.IsMatch (ccp, @"^\d{9}$");
		}

		public static bool CheckReferenceLine(string iban, string reference)
		{
			bool check	=  BvHelper.CheckBeneficiaryIban (iban)
						&& BvHelper.CheckReferenceClientNumber (reference);

			return check;
		}

		public static bool CheckClearingLine(string constant, string clearing, string key)
		{
			bool check	=  BvHelper.CheckClearingConstant (constant)
						&& BvHelper.CheckClearingBank (clearing)
						&& BvHelper.CheckClearingBankKey (key);

			return check;
		}

		public static bool CheckBv(BvWidget bvWidget)
		{
			bool check	=  BvHelper.CheckBankAddress (bvWidget.BankAddress)
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

			return check;
		}

		public static string BuildReferenceLine(string iban, string reference)
		{
			if (!BvHelper.CheckReferenceLine (iban, reference))
			{
				string message = string.Format ("One of the provided argument is not valid. Iban: {0}. Reference: {1}.", iban, reference);
				throw new System.ArgumentException (message);
			}

			string line = string.Format ("{0}{1}{2}", reference, "0000", Regex.Replace (iban, @"\s", "").Substring (9, 12));
			return string.Format ("{0}{1}+", line, BvHelper.Compute (line));
		}

		public static string BuildClearingLine(string constant, string clearing, string key)
		{
			if (!BvHelper.CheckClearingLine (constant, clearing, key))
			{
				string message = string.Format ("One of the provided argument is not valid. Constant: {0}. Clearing: {1}. Key: {2}.", constant, clearing, key);
				throw new System.ArgumentException (message);
			}

			string line = string.Format ("{0}{1}{2}", constant, clearing, key);
			return string.Format ("{0}{1}>", line, BvHelper.Compute (line));
		}

		public static string BuildCcpNumberLine(string ccp)
		{
			if (!BvHelper.CheckCcpNumber (ccp))
			{
				throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", ccp));
			}

			return string.Format ("{0}>", ccp);
		}

		public static string BuildNormalizedIban(string iban)
		{
			string normalzedIban = Regex.Replace (iban, @"\s", "");

			for (int i = 4; i < normalzedIban.Length && i < 27; i += 5)
			{
				normalzedIban = normalzedIban.Insert (i, " ");
			}

			return normalzedIban;
		}

	}

}
