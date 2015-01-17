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
		public int								MembersCount
		{
			get
			{
				var count = 1;

				if (this.Adult2.IsNotNull ())
				{
					count++;
				}

				count += this.Children.Count;

				return count;
			}
		}


		public void RemoveDuplicates()
		{
			if (this.Adult1 == this.Adult2)
			{
				this.Adult2 = null;
				this.ClearCache ();
			}

			var ids  = new HashSet<string> ();
			var keep = new List<eCH_PersonEntity> ();

			foreach (var child in this.Children)
			{
				if (ids.Add (child.PersonId))
				{
					keep.Add (child);
				}
			}

			if (keep.Count < this.Children.Count)
			{
				this.Children.Clear ();
				this.Children.AddRange (keep);
				this.ClearCache ();
			}
		}

		private void ClearCache()
		{
			this.membersCache = null;
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

		private IList<eCH_PersonEntity>			membersCache;
	}
}
