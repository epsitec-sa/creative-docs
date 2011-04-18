//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TabPageDef&lt;T&gt;</c> class represents a tab associated with an id
	/// of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the ids.</typeparam>
	public class TabPageDef<T> : TabPageDef
	{
		public TabPageDef(T id, string name, FormattedText text, System.Action selectAction)
			: base (name, text, selectAction)
		{
			this.id = id;
		}

		
		public T								Id
		{
			get
			{
				return this.id;
			}
		}

		
		private readonly T id;
	}
}
