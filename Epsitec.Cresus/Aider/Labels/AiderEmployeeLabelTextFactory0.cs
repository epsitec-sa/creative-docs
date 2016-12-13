using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Labels;

using System;


namespace Epsitec.Aider.Labels
{


	[LabelTextFactoryId (0)]
	public sealed class AiderEmployeeLabelTextFactory0 : LabelTextFactory<AiderEmployeeEntity>
	{


		public override FormattedText GetLabelText(AiderEmployeeEntity entity)
		{
			return entity.PersonContact.GetAddressLabelText (PostalAddressType.Default);
		}


	}


}
