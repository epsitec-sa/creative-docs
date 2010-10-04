//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class SoftwareUserGroupEntity
	{
		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.Code.GetEntityStatus ();
			var s2 = this.Name.GetEntityStatus ();
			var s3 = this.Description.GetEntityStatus ().TreatAsOptional ();
			var s4 = this.Roles.Select (x => x.GetEntityStatus ()).ToArray ();

			return Helpers.EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4);
		}
	}
}
