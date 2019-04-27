using Epsitec.Aider.Entities;

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Labels;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (1)]
	public sealed class AiderContactLabelTextFactory1 : LabelTextFactory<AiderContactEntity>
	{


		public override FormattedText GetLabelText(AiderContactEntity entity)
		{
			return entity.GetAddressOfParentsLabelText ();
		}

		public override FormattedText GetSenderText()
		{
			var user = UserManager.Current.AuthenticatedUser as AiderUserEntity;
			return user.GetSenderAddressLabelText ();
		}


	}


}
