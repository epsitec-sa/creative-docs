//	Copyright © 2012-2017, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Labels
{
	[LabelTextFactoryId (0)]
	public sealed class AiderMailingParticipantLabelTextFactory0 : LabelTextFactory<AiderMailingParticipantEntity>
	{
		public override FormattedText GetLabelText(AiderMailingParticipantEntity entity)
		{
			var contact   = entity.Contact;
			var recipient = entity.CustomRecipient;
			
			return contact.GetCustomAddressLabelText (recipient);
		}

		public override FormattedText GetSenderText()
		{
			var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
			
			return user.GetSenderAddressLabelText ();
		}
	}
}
