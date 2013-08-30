//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Entities;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class eCH_ReportedPersonEntity
	{
		public int GetMembersCount()
		{
            var count = 1;

            if (this.Adult2.IsNotNull())
            {
                count++;
            }

            count += this.Children.Count;

            return count;
		}
	}
}
