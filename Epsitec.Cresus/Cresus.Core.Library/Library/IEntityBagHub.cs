//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library
{
	public interface IEntityBagHub
	{
		void AddToBag(string userName, string title, string summary, string entityId, NotificationTime when);
		void RemoveFromBag(string userName, string title, string summary, string entityId, NotificationTime when);
	}
}