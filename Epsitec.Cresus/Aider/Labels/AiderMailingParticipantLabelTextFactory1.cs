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
			switch (entity.Mailing.GroupMode)
			{
				case Enumerations.MailingGroupMode.ByHouseholdUsingDesc:
					return TextFormatter.FormatText ("Aux parents de\n" + entity.CustomRecipient);
				case Enumerations.MailingGroupMode.ByHouseholdUsingParticipants:
					return entity.Contact.GetCustomAddressLabelText ("Aux parents de\n" + entity.CustomRecipient);
				case Enumerations.MailingGroupMode.ByContact:
				default:
					return entity.Contact.GetAddressOfParentsLabelText ();
			}	
		}


	}


}
