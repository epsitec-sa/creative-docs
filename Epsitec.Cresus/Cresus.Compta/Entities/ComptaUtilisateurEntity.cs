//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaUtilisateurEntity
	{
		public UserAccess UserAccess
		{
			get
			{
				return (UserAccess) this.DroitsDaccès;
			}
			set
			{
				this.DroitsDaccès = (int) value;
			}
		}
	}
}
