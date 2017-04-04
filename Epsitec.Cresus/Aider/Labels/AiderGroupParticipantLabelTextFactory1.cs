using System;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (1)]
	public sealed class AiderGroupParticipantLabelTextFactory1 : LabelTextFactory<AiderGroupParticipantEntity>
	{


		public override FormattedText GetLabelText(AiderGroupParticipantEntity entity)
		{
			return entity.Contact.GetAddressOfParentsLabelText ();
		}

		public override FormattedText GetSenderText()
		{
            var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
            return user.GetSenderAddressLabelText();
        }
	}


}
