using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderGroupParticipantLabelTextFactory0 : LabelTextFactory<AiderGroupParticipantEntity>
	{


		public override FormattedText GetLabelText(AiderGroupParticipantEntity entity)
		{
			return entity.Contact.GetAddressLabelText ();
		}


	}


}
