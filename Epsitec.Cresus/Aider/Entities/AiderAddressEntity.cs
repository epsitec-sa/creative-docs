//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;


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
				this.GetWebsiteText ()
			};

			var texts = parts.Where (p => !p.IsNullOrWhiteSpace);

			return TextFormatter.Join (FormattedText.HtmlBreak + FormattedText.HtmlBreak, texts);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type);
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				//a.Accumulate (this.IdA.GetEntityStatus ());
				//a.Accumulate (this.IdB.GetEntityStatus ().TreatAsOptional ());
				//a.Accumulate (this.IdC.GetEntityStatus ().TreatAsOptional ());
				//a.Accumulate (this.Affairs.Select (x => x.GetEntityStatus ()));
				//a.Accumulate (this.MainRelation, EntityStatusAccumulationMode.NoneIsPartiallyCreated);
				//a.Accumulate (this.SalesRepresentative.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		public IEnumerable<FormattedText> GetAddressLines()
		{
			yield return this.AddressLine1;
			yield return StringUtils.Join (" ", this.Street, this.HouseNumber, this.HouseNumberComplement);
			yield return this.PostBox;
			yield return StringUtils.Join (" ", StringUtils.Join ("-", this.Town.Country.IsoCode, this.Town.ZipCode), this.Town.Name);
			yield return this.Town.Country.Name;
		}


		public IEnumerable<FormattedText> GetPhones()
		{
			if (!this.Phone1.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Phone1);
			}

			if (!this.Phone2.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Phone2);
			}

			if (!this.Mobile.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Mobile);
			}

			if (!this.Fax.IsNullOrWhiteSpace ())
			{
				yield return TextFormatter.FormatField (() => this.Fax);
			}
		}


		private FormattedText GetAddressText()
		{
			return this.GetLabeledText ("Addresse postale", this.GetAddressLines ());
		}


		private FormattedText GetPhonesText()
		{
			return this.GetLabeledText ("Numéros de téléphones", this.GetPhones ());
		}


		private FormattedText GetEmailText()
		{
			return this.GetLabeledText ("Addresse éléctronique", this.Email);
		}


		private FormattedText GetWebsiteText()
		{
			return this.GetLabeledText ("Site internet", this.Web);
		}


		private FormattedText GetLabeledText(string label, IEnumerable<FormattedText> lines)
		{
			var text = TextFormatter.Join ("<br/>", lines.Where (l => !l.IsNullOrWhiteSpace));

			if (text.IsNullOrWhiteSpace)
			{
				return null;
			}

			return this.GetLabeledText (label, text);
		}


		private FormattedText GetLabeledText(string label, FormattedText text)
		{
			if (text.IsNullOrWhiteSpace)
			{
				return null;
			}

			var formattedLabel = TextFormatter.FormatText (label).ApplyBold ();

			return TextFormatter.FormatText (formattedLabel, "\n", text);
		}
	}
}
