using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderLegalPersonTextFactory0 : LabelTextFactory<AiderLegalPersonEntity>
	{


		public override FormattedText GetLabelText(AiderLegalPersonEntity entity)
		{
			return entity.GetAddressLabelText ();
		}


	}


}
