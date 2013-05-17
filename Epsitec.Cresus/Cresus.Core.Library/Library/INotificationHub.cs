//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library
{
	public interface INotificationHub
	{
		void NotifyAll(NotificationMessage message,bool onConnect);
		void Notify(string userName, NotificationMessage message,bool onConnect);
		void WarnUser(string userName, NotificationMessage message, bool onConnect);
	}
}

