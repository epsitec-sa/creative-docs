using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (1)]
	public sealed class AiderMailingParticipantLabelTextFactory1 : LabelTextFactory<AiderMailingParticipantEntity>
	{


		public override FormattedText GetLabelText(AiderMailingParticipantEntity entity)
		{
			if (entity.Mailing.GroupMode != Enumerations.MailingGroupMode.ByContact)
			{
				return entity.Contact.GetCustomAddressLabelText ("Aux parents de\n" + entity.CustomRecipient);
			}
			else
			{
				return entity.Contact.GetAddressOfParentsLabelText ();
			}			
		}


	}


}
