//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Core.Library
{
	public interface IStatusBarHub
	{
		void AddToBar(string type, string text, string iconClass, string statusId, When when);
		void RemoveFromBar(string statusId, When when);
		IEnumerable<string> GetStatusEntitiesId();
	}
}