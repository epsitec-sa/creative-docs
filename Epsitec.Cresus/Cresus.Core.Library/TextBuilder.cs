//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public class TextBuilder
	{
		public TextBuilder()
		{
			this.items = new List<object> ();
		}


		public void Append(object item)
		{
			this.items.Add (item);
		}

		public void Clear()
		{
			this.items.Clear ();
		}

		public FormattedText ToFormattedText()
		{
			return TextFormatter.FormatText (items.ToArray ());
		}


		private readonly List<object> items;
	}
}
