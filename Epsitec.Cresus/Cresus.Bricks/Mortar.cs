//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public abstract class Mortar
	{
		public virtual string GetString()
		{
			return null;
		}

		public virtual Expression GetExpression()
		{
			return null;
		}
	}
}
