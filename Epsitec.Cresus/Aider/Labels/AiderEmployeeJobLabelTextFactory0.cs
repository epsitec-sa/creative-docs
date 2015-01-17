using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;

using System;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderEmployeeJobLabelTextFactory0 : LabelTextFactory<AiderEmployeeJobEntity>
	{


		public override FormattedText GetLabelText(AiderEmployeeJobEntity entity)
		{
			return entity.GetAddressLabelText ();
		}


	}


}
