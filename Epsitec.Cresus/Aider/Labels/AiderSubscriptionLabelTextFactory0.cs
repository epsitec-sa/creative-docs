using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Labels;

using System;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderSubscriptionLabelTextFactory0 : LabelTextFactory<AiderSubscriptionEntity>
	{


		public override FormattedText GetLabelText(AiderSubscriptionEntity entity)
		{
			return entity.GetAddressLabelText ();
		}

		public override FormattedText GetSenderText()
		{
            var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
            return user.GetSenderAddressLabelText();
        }
	}


}
