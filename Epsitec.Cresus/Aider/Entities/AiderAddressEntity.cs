using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderAddressEntity
	{


		public override FormattedText GetSummary()
		{
			var parts = new List<FormattedText> ()
			{
				this.GetTypeText (),
				this.GetAddressText (),
				this.GetPhonesText (),
				this.GetEmailText (),
				this.GetDescriptionText (),
			};

			var texts = parts.Where (p => !p.IsNullOrWhiteSpace);

			return TextFormatter.Join (FormattedText.HtmlBreak + FormattedText.HtmlBreak, texts);
		}


		public override FormattedText GetCompactSummary()
		{
			var part1 = this.Town.Name
				?? this.Phone1
				?? this.Phone2
				?? this.Email;

			var part2 = this.GetTypeText (this.Type);

			if (part1 == null)
			{
				return null;
			}

			if (part2 == null)
			{
				return part1;
			}

			var text = part1 + " (" + part2 + ")";

			return FormattedText.FromSimpleText (text);
		}


		private FormattedText GetTypeText()
		{
			return this.GetLabeledText ("Type d'adresse", this.GetTypeText (this.Type));
		}


		private FormattedText GetAddressText()
		{
			return this.GetLabeledText ("Addresse", this.GetAddressLines ());
		}


		private IEnumerable<string> GetAddressLines()
		{
			yield return this.AddressLine1;
			yield return string.Join (" ", this.Street, this.HouseNumber, this.HouseNumberComplement);
			yield return this.PostBox;
			yield return string.Join (" ", string.Join ("-", this.Town.Country.IsoCode, this.Town.SwissZipCode), this.Town.Name);
			yield return this.Town.Country.Name;
		}


		private FormattedText GetPhonesText()
		{
			return this.GetLabeledText ("Téléphones", this.GetPhoneLines ());
		}


		private IEnumerable<string> GetPhoneLines()
		{
			yield return this.Phone1;
			yield return this.Phone2;
		}


		private FormattedText GetEmailText()
		{
			return this.GetLabeledText ("Addresse email", this.Email);
		}


		private FormattedText GetDescriptionText()
		{
			return this.GetLabeledText ("Description", this.Description);
		}


		private FormattedText GetLabeledText(string label, IEnumerable<string> lines)
		{
			var text = lines
				   .Where (l => !l.IsNullOrWhiteSpace ())
				   .Join ("\n");

			if (text.IsNullOrWhiteSpace ())
			{
				return null;
			}

			return this.GetLabeledText (label, text);
		}


		private FormattedText GetLabeledText(string label, string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				return null;
			}

			var formattedLabel = TextFormatter.FormatText (label).ApplyBold ();

			return TextFormatter.FormatText(formattedLabel, "\n", text);
		}


		private string GetTypeText(AddressType type)
		{
			switch (type)
			{
				case AddressType.Default:
					return null;

				case AddressType.Private:
					return "Privé";

				case AddressType.Professional:
					return "Professionnel";

				case AddressType.Secondary:
					return "Secondaire";

				default:
					return null;
			}
		}


	}


}
