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
		partial void GetPersonWarnings(ref IList<AiderPersonWarningEntity> value)
		{
			value = this.Warnings.Cast<AiderPersonWarningEntity> ().ToList ();
		}
	}
}

