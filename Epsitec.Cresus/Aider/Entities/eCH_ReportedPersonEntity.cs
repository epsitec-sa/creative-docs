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

		partial void GetMembers(ref IList<eCH_PersonEntity> value)
		{
			value = this.GetEChMembers ().AsReadOnlyCollection ();
		}

		private IList<eCH_PersonEntity> GetEChMembers()
		{
			if (this.membersCache == null)
			{
				var heads = new List<eCH_PersonEntity> ();

				heads.Add (this.Adult1);
				if (this.Adult2.IsNotNull ())
				{
					heads.Add (this.Adult2);
				}			
				var children = this.Children.OrderBy (x => x.PersonDateOfBirth);

				this.membersCache = heads.Concat (children).ToList ();
			}

			return this.membersCache;
		}

		private IList<eCH_PersonEntity>		membersCache;
	}
}
