//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epsitec.Cresus.Core.Business
{

	/// <summary>
	/// The class <c>IsrHelper</c> contains static methods used to check or build es values.
	/// </summary>
	static class IsrHelper
	{
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

				foreach (string s in IsrHelper.charConversionTable.Keys)
				{
					iban = iban.Replace (s, IsrHelper.charConversionTable[s]);
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
			return IsrHelper.CheckIban (iban);
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
		/// Checks that <paramref name="amount"/> is a valid amount.
		/// </summary>
		/// <param name="amount">The amount to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="amount"/> is valid or not.</returns>
		public static bool CheckAmount(string amount)
		{
			return (amount.Length == 0) || Regex.IsMatch (amount, @"^\d{1,8}[.,]\d{2}$");
		}

		/// <summary>
		/// Checks that <paramref name="payedBy"/> is a valid name and address.
		/// </summary>
		/// <param name="amount">The name and address to check.</param>
		/// <returns>A <see cref="bool"/> indicating if <paramref name="payedBy"/> is valid or not.</returns>
		public static bool CheckPayedBy(string payedBy)
		{
			string[] lines = payedBy.Split ('\n');

			return (lines.Length <= 4) && (lines.All (line => line.Length <= 30));
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
			return IsrHelper.CheckBeneficiaryIban (iban)
				&& IsrHelper.CheckReferenceClientNumber (reference);
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
			return IsrHelper.CheckClearingConstant (constant)
				&& IsrHelper.CheckClearingBank (clearing)
				&& IsrHelper.CheckClearingBankKey (key);
		}

		/// <summary>
		/// Gets the francs part of the amount.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <returns>The francs part of the amount</returns>
		/// <exception cref="System.ArgumentException">If the provided value is not valid.</exception>
		public static string BuildFrancPart(string amount)
		{
			if (!IsrHelper.CheckAmount (amount))
			{
				throw new System.ArgumentException (string.Format("the provided argument is not valid. Iban: {0}.", amount));
			}

			if (amount.Length > 0)
			{
				int index = amount.IndexOfAny (new char[] { ',', '.' });
				return amount.Substring (0, index);
			}
			else
			{
				return "";
			}
		}

		/// <summary>
		/// Gets the cents part of the amount.
		/// </summary>
		/// <param name="amount">The amount.</param>
		/// <returns>The cents part of the amount</returns>
		/// <exception cref="System.ArgumentException">If the provided value is not valid.</exception>
		public static string BuildCentPart(string amount)
		{
			if (!IsrHelper.CheckAmount (amount))
			{
				throw new System.ArgumentException (string.Format ("the provided argument is not valid. Iban: {0}.", amount));
			}

			if (amount.Length > 0)
			{
				int index = amount.IndexOfAny (new char[] { ',', '.' });
				return amount.Substring (index + 1);
			}
			else
			{
				return "";
			}
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
			if (!IsrHelper.CheckReferenceLine (iban, reference))
			{
				string message = string.Format ("One of the provided argument is not valid. Iban: {0}. Reference: {1}.", iban, reference);
				throw new System.ArgumentException (message);
			}

			iban =  Regex.Replace (iban, @"\s", "");
			string line = string.Format ("{0}{1}{2}", reference, "0000", iban.Substring (iban.Length - 12, 12));

			return string.Format ("{0}{1}+", line, Isr.ComputeCheckDigit (line));
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
		/// Normalizes <paramref name="reason"/> so that each line has at most 10 chars on it.
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

		/// <summary>
		/// Gets the error message for <paramref name="iban"/>.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>The error message for <paramref name="iban"/>.</returns>
		public static string GetErrorMessageForBenefeciaryIban(string iban)
		{
			string ibanNoSpace = iban.Replace (" ", "");

			bool validLength = ibanNoSpace.Length == 21;
			bool noLetters = validLength ? Regex.IsMatch (ibanNoSpace.Substring (ibanNoSpace.Length - 12, 12), "^[0-9]*$") : true;

			string error;

			if (!validLength)
			{
				error = "N°IBAN du bénéficiaire: doit contenir 21 caractères.";
			}
			else if (!noLetters)
			{
				error = "N°IBAN du bénéficiaire: doit se terminer par 12 chiffres.";
			}
			else if (!IsrHelper.CheckBeneficiaryIban (iban))
			{
				error = "N°IBAN du bénéficiaire: invalide.";
			}
			else
			{
				error = "";
			}

			return error;
		}

		/// <summary>
		/// Gets the error message for <paramref name="address"/>.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>The error message for <paramref name="address"/>.</returns>
		public static string GetErrorMessageForBeneficiaryAddress(string address)
		{
			string[] lines = address.Split ('\n');

			string error;

			if (address.Length == 0)
			{
				error = "Nom et adresse du bénéficiaire: doit être rempli.";
			}
			else if (lines.Count () > 4)
			{
				error = "Nom et adresse du bénéficiaire: ne peut contenir que 4 lignes.";
			}
			else if (System.Array.Exists (lines, line => line.Length > 27))
			{
				error = "Nom et adresse du bénéficiaire: chaque ligne ne peut contenir que 27 caractères.";
			}
			else if (!IsrHelper.CheckBeneficiaryAddress (address))
			{
				error = "Nom et adresse du bénéficiaire: invalide.";
			}
			else
			{
				error = "";
			}

			return error;
		}

		/// <summary>
		/// Gets the error message for <paramref name="amount"/>.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>The error message for <paramref name="amount"/>.</returns>
		public static string GetErrorMessageForAmount(string amount)
		{
			char[] period = new char[] { ',', '.' };

			string error;

			if (amount.IndexOfAny (period) > 8 || amount.LastIndexOfAny (period) < amount.Length - 3)
			{
				error = "Montant: ne peut contenir que 8 chiffres et 2 décimales.";
			}
			else if (System.Array.FindAll (amount.ToCharArray (), c => period.Contains (c)).Count () != 1)
			{
				error = "Montant: ne peut contenir qu'une seule virgule.";
			}
			else if (!System.Array.TrueForAll (amount.ToCharArray (), c => char.IsDigit (c) || period.Contains (c)))
			{
				error = "Montant: ne peut contenir que des chiffres et une virgule.";
			}
			else if (!IsrHelper.CheckAmount(amount))
			{
				error = "Montant: invalide.";
			}
			else
			{
				error = "";
			}

			return error;
		}

		/// <summary>
		/// Gets the error message for <paramref name="payedBy"/>.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>The error message for <paramref name="payedBy"/>.</returns>
		public static string GetErrorMessageForPayedBy(string payedBy)
		{
			string[] lines = payedBy.Split ('\n');

			string error;

			if (lines.Count () > 4)
			{
				error = "Versé par: ne peut contenir que 3 lignes.";
			}
			else if (System.Array.Exists (lines, line => line.Length > 30))
			{
				error = "Versé par: chaque ligne ne peut contenir que 30 caractères.";
			}
			else if (!IsrHelper.CheckPayedBy (payedBy))
			{
				error = "Versé par: invalide.";
			}
			else
			{
				error = "";
			}

			return error;
		}

		/// <summary>
		/// Gets the error message for <paramref name="reason"/>.
		/// </summary>
		/// <param name="reason">The reason.</param>
		/// <returns>The error message for <paramref name="reason"/>.</returns>
		public static string GetErrorMessageForReason(string reason)
		{
			string[] lines = reason.Split ('\n');
			bool valid = IsrHelper.CheckReason (reason);

			string error;

			if (!valid && lines.Count () > 3)
			{
				error = "Motif du versement: ne peut contenir que 3 lignes.";
			}
			else if (!valid && System.Array.Exists (lines, line => line.Length > 10))
			{
				error = "Motif du versement: chaque ligne ne peut contenir que 10 caractères.";
			}
			else if (!valid)
			{
				error = "Motif du versement: invalide.";
			}
			else
			{
				error = "";
			}

			return error;
		}

	}

}
