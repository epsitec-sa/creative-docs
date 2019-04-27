using System;
using Epsitec.Aider.Entities;

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderContactLabelTextFactory0 : LabelTextFactory<AiderContactEntity>
	{


		public override FormattedText GetLabelText(AiderContactEntity entity)
		{
			return entity.GetAddressLabelText ();
		}

		public override FormattedText GetSenderText()
		{
			var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
			return user.GetSenderAddressLabelText ();
		}
	}


}
