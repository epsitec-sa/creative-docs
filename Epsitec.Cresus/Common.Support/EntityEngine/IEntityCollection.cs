//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	public interface IEntityCollection
	{
		void ResetCopyOnWrite();
		void CopyOnWrite();
		
		bool UsesCopyOnWriteBehavior
		{
			get;
		}
	}
}
