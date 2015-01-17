//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	public static class Change
	{
		public static Change<T> Create<T>(T oldValue, T newValue)
			where T : class
		{
			return new Change<T> (oldValue, newValue);
		}
	}
}

