﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Danirl ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	public interface IAttachedDialog
	{
		IEnumerable<AbstractEntity> Entities
		{
			get;
		}

		void Update();
	}
}
