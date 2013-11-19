//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
namespace Epsitec.Cresus.Core.Library
{
	public interface IEntityBagHub
	{
		void AddToBag(string userName, string title, FormattedText summary, string entityId, When when);
		void RemoveFromBag(string userName, string entityId, When when);
		void SetLoading(string userName,bool state);
	}
}