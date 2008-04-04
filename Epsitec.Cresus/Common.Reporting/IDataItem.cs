//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public interface IDataItem
	{
		string Value
		{
			get;
		}

		DataItemClass Class
		{
			get;
		}

		DataItemType Type
		{
			get;
		}
	}
}
