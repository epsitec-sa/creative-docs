//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public interface IGroupedItem : IGroupedItemPosition
	{
		/// <summary>
		/// Gets the id of the group to which this item belongs.
		/// </summary>
		/// <returns>The id of the group.</returns>
		string GetGroupId();
	}
}
