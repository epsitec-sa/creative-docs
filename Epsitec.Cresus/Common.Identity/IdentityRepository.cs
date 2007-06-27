//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Identity;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (IdentityRepository))]

namespace Epsitec.Common.Identity
{
	public class IdentityRepository : DependencyObject
	{
		public IdentityRepository()
		{
		}

		
		public IList<IdentityCard> Identities
		{
			get
			{
				return this.identities;
			}
		}

		
		private static object GetIdentitiesValue(DependencyObject obj)
		{
			IdentityRepository that = (IdentityRepository) obj;
			return that.Identities;
		}

		public static readonly DependencyProperty IdentitiesProperty = DependencyProperty.RegisterReadOnly ("Identities", typeof (ICollection<IdentityCard>), typeof (IdentityRepository), new DependencyPropertyMetadata (IdentityRepository.GetIdentitiesValue).MakeReadOnlySerializable ());

		private List<IdentityCard> identities = new List<IdentityCard> ();
	}
}
