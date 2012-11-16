//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public abstract class Mortar<TSource> : Mortar
	{
		public static implicit operator Mortar<TSource>(string value)
		{
			return new Mortar<TSource, string> (value);
		}
		
		public static implicit operator Mortar<TSource>(FormattedText value)
		{
			return new Mortar<TSource, FormattedText> (value.ToString ());
		}
	}
}
