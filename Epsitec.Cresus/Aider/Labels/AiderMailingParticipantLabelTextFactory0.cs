using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderMailingParticipantLabelTextFactory0 : LabelTextFactory<AiderMailingParticipantEntity>
	{


		public override FormattedText GetLabelText(AiderMailingParticipantEntity entity)
		{
			if (entity.Houshold.IsNotNull ())
				return entity.Houshold.GetAddressLabelText ();		
			else
				return entity.Contact.GetAddressLabelText ();
		}
		
	}


}
