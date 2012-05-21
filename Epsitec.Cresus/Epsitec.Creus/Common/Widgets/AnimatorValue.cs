//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	public abstract class AnimatorValue
	{
		protected AnimatorValue()
		{
		}


		public abstract object BeginValue
		{
			get;
		}

		public abstract object EndValue
		{
			get;
		}


		public abstract object Interpolate(double ratio);
	}
}
