using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;

using System;


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
