//	Copyright © 2012-2017, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Labels
{
	[LabelTextFactoryId (1)]
	public sealed class AiderMailingParticipantLabelTextFactory1 : LabelTextFactory<AiderMailingParticipantEntity>
	{
		public override FormattedText GetLabelText(AiderMailingParticipantEntity entity)
		{
			var contact   = entity.Contact;
			var recipient = entity.CustomRecipient;

			switch (entity.Mailing.GroupMode)
			{
				case Enumerations.MailingGroupMode.ByHouseholdUsingDesc:
				case Enumerations.MailingGroupMode.ByHouseholdUsingParticipants:
					var rows = recipient.Split ('\n');
					rows[0] = "Aux parents de";  // replace Monsieur by a fixed text...
					recipient = string.Join ("\n", rows);
					break;
				default:
					break;
			}
			
			return contact.GetCustomAddressLabelText (recipient);
		}

		public override FormattedText GetSenderText()
		{
			var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
			
			return user.GetSenderAddressLabelText ();
		}
	}
}
