//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Core.Library
{
	public interface IEntityBagHub
	{
		void AddToBag(string userName, string title, FormattedText summary, string entityId, When when);
		void RemoveFromBag(string userName, string entityId, When when);
		void SetLoading(string userName,bool state);
		IEnumerable<string> GetUserBagEntitiesId(string userName);
	}
}