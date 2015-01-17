using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;

using System;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderHouseholdLabelTextFactory0 : LabelTextFactory<AiderHouseholdEntity>
	{


		public override FormattedText GetLabelText(AiderHouseholdEntity entity)
		{
			return entity.GetAddressLabelText ();
		}


	}


}
