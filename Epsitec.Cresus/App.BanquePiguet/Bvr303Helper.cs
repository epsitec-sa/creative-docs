//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epsitec.App.BanquePiguet
{

	class Bvr303Helper
	{

		/// <summary>
		/// This bidimensional array is the table used by the modulo 10 recursive algorithm
		/// to find the next report digit at each step.
		/// </summary>
		protected static int[,] table = new int[,]
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
		protected static char[] keys = new char[]
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
				throw new ArgumentException ("The provided string contains non numeric characters");
			}

			if (number.Length == 0)
			{
				throw new ArgumentException ("The provided string is empty");
			}

			int report = 0;

			while (number.Length > 0)
			{
				int digit = Int32.Parse (number.Substring (0, 1), CultureInfo.InvariantCulture);
				report = Bvr303Helper.table[report, digit];
				number = number.Substring (1);
			}

			return Bvr303Helper.keys[report];
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
			bool check	=  Bvr303Helper.CheckBeneficiaryIban (iban)
						&& Bvr303Helper.CheckReferenceClientNumber (reference);

			return check;
		}

		public static bool CheckClearingLine(string constant, string clearing, string key)
		{
			bool check	=  Bvr303Helper.CheckClearingConstant (constant)
						&& Bvr303Helper.CheckClearingBank (clearing)
						&& Bvr303Helper.CheckClearingBankKey (key);

			return check;
		}

		public static bool CheckBvr303(Bvr303Widget bvr303Widget)
		{
			bool check	=  Bvr303Helper.CheckBankAddress (bvr303Widget.BankAddress)
						&& Bvr303Helper.CheckBeneficiaryIban (bvr303Widget.BeneficiaryIban)
						&& Bvr303Helper.CheckBeneficiaryAddress (bvr303Widget.BeneficiaryAddress)
						&& Bvr303Helper.CheckBankAccount (bvr303Widget.BankAccount)
						&& Bvr303Helper.CheckLayoutCode (bvr303Widget.LayoutCode)
						&& Bvr303Helper.CheckReason (bvr303Widget.Reason)
						&& Bvr303Helper.CheckReferenceClientNumber (bvr303Widget.ReferenceClientNumber)
						&& Bvr303Helper.CheckClearingConstant (bvr303Widget.ClearingConstant)
						&& Bvr303Helper.CheckClearingBank (bvr303Widget.ClearingBank)
						&& Bvr303Helper.CheckClearingBankKey (bvr303Widget.ClearingBankKey)
						&& Bvr303Helper.CheckCcpNumber (bvr303Widget.CcpNumber);

			return check;
		}

		public static string BuildReferenceLine(string iban, string reference)
		{
			if (!Bvr303Helper.CheckReferenceLine (iban, reference))
			{
				string message = String.Format ("One of the provided argument is not valid. Iban: {0}. Reference: {1}.", iban, reference);
				throw new ArgumentException (message);
			}

			string line = String.Format ("{0}{1}{2}", reference, "0000", Regex.Replace (iban, @"\s", "").Substring (9, 12));
			return String.Format ("{0}{1}+", line, Bvr303Helper.Compute (line));
		}

		public static string BuildClearingLine(string constant, string clearing, string key)
		{
			if (!Bvr303Helper.CheckClearingLine (constant, clearing, key))
			{
				string message = String.Format ("One of the provided argument is not valid. Constant: {0}. Clearing: {1}. Key: {2}.", constant, clearing, key);
				throw new ArgumentException (message);
			}

			string line = String.Format ("{0}{1}{2}", constant, clearing, key);
			return String.Format ("{0}{1}>", line, Bvr303Helper.Compute (line));
		}

		public static string BuildCcpNumberLine(string ccp)
		{
			if (!Bvr303Helper.CheckCcpNumber (ccp))
			{
				throw new ArgumentException (String.Format ("The provided value is not valid: {0}", ccp));
			}

			return String.Format ("{0}>", ccp);
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
