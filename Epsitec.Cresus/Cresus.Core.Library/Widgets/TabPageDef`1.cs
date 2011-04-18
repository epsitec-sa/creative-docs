//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	public class TabPageDef<T> : TabPageDef
	{
		public TabPageDef(T id, string name, FormattedText text, System.Action action)
			: base (name, text, action)
		{
			this.id = id;
		}

		public T Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly T id;
	}
}
