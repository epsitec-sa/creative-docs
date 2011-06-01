//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Helpers
{
	internal abstract class FormatToken
	{
		public FormatToken(string format)
		{
			this.format = format;
		}
		
		public static IEnumerable<FormatToken> Items
		{
			get
			{
				return FormatToken.items;
			}
		}
		
		public abstract bool Matches(FormatterHelper formatter, string format, int pos);
		
		public abstract int Format(FormatterHelper formatter, System.Text.StringBuilder buffer);
		
		static FormatToken()
		{
			FormatToken.items = new List<FormatToken> (FormatterHelper.GetTokens ().OrderByDescending (x => x.format.Length));
		}
		
		private readonly static List<FormatToken> items;
		
		protected readonly string format;
	}
}