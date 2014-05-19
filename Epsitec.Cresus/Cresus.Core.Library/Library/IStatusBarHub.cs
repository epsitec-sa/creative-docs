//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Core.Library
{
	public interface IStatusBarHub
	{
		void AddToBar(string userName, string title, FormattedText summary, string entityId, When when);
		void RemoveFromBar(string userName, string entityId, When when);
		void SetLoading(string userName,bool state);
		IEnumerable<string> GetStatusEntitiesId();
	}
}