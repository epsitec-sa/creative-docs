using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Common.Support;


namespace Epsitec.Aider.Entities
{


	public partial class AiderAddressEntity
	{


		public override FormattedText GetSummary()
		{
			var parts = new List<FormattedText> ()
			{
				this.GetAddressText (),
				this.GetPhonesText (),
				this.GetEmailText (),
				this.GetWebsiteText (),
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

			var part2 = this.Type.AsText ();

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


		private FormattedText GetAddressText()
		{
			return this.GetLabeledText ("Addresse postale", this.GetAddressLines ());
		}


		public IEnumerable<string> GetAddressLines()
		{
			yield return this.AddressLine1;
			yield return StringUtils.Join (" ", this.Street, this.HouseNumber, this.HouseNumberComplement);
			yield return this.PostBox;
			yield return StringUtils.Join (" ", StringUtils.Join ("-", this.Town.Country.IsoCode, this.Town.ZipCode), this.Town.Name);
			yield return this.Town.Country.Name;
		}


		private FormattedText GetPhonesText()
		{
			return this.GetLabeledText ("Numéros de téléphones", this.GetPhones ());
		}


		public IEnumerable<string> GetPhones()
		{
			if (!this.Phone1.IsNullOrWhiteSpace ())
			{
				yield return this.Phone1;
			}

			if (!this.Phone2.IsNullOrWhiteSpace ())
			{
				yield return this.Phone2;
			}
		}


		private FormattedText GetEmailText()
		{
			return this.GetLabeledText ("Addresse éléctronique", this.Email);
		}


		private FormattedText GetWebsiteText()
		{
			return this.GetLabeledText ("Site internet", this.Web);
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
