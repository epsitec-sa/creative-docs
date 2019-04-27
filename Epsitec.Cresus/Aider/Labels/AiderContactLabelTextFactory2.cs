using Epsitec.Aider.Entities;

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Labels;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (2)]
	public sealed class AiderContactLabelTextFactory2 : LabelTextFactory<AiderContactEntity>
	{


		public override FormattedText GetLabelText(AiderContactEntity entity)
		{
			return entity.Household.GetAddressLabelText ();
		}

		public override FormattedText GetSenderText()
		{
			var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
			return user.GetSenderAddressLabelText ();
		}


	}


}
