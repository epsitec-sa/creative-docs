//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Entities
{
	public partial class AiderPersonWarningSourceEntity
	{
		public static AiderPersonWarningSourceEntity Create(BusinessContext context, System.DateTime date, FormattedText name, FormattedText description)
		{
			return AiderWarningSourceEntity.Create<AiderPersonWarningSourceEntity, AiderPersonWarningEntity> (context, date, name, description);
		}
	}
}

