using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderContactLabelTextFactory0 : LabelTextFactory<AiderContactEntity>
	{


		public override FormattedText GetLabelText(AiderContactEntity entity)
		{
			return entity.GetAddressLabelText ();
		}


	}


}
