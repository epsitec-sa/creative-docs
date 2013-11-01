//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class AiderMailingEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
			(
				this.Name
			);
		}


		public override FormattedText GetSummary()
		{
			return this.Name;
		}

		public void RefreshCache()
		{
			
		}

		public FormattedText GetRecipientsSummary()
		{
			var recipients = this.Recipients
				.Select (r => r.GetCompactSummary ())
				.CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), recipients);
		}

		public static AiderMailingEntity Create(BusinessContext context, SoftwareUserEntity creator, string name, bool isReady)
		{
			var mailing = context.CreateAndRegisterEntity<AiderMailingEntity> ();

			var aiderUserExample = new AiderUserEntity()
			{
				People = creator.People
			};
			mailing.Name = name;
			mailing.IsReady = isReady;
			mailing.CreatedBy = context.DataContext.GetByExample<AiderUserEntity> (aiderUserExample).FirstOrDefault ();

			return mailing;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingEntity mailing)
		{
			//TODO BEFORE DELETE?

			businessContext.DeleteEntity (mailing);
		}
	}
}
