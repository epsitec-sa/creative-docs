using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;

using System;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderSubscriptionRefusalLabelTextFactory0 : LabelTextFactory<AiderSubscriptionRefusalEntity>
	{


		public override FormattedText GetLabelText(AiderSubscriptionRefusalEntity entity)
		{
			return entity.GetAddressLabelText ();
		}


	}


}
