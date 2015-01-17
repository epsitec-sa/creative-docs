//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public abstract class AiderWarningExampleFactory
	{
		public abstract IEnumerable<T> GetWarnings<T>(BusinessContext context, AbstractEntity source);
	}
}

