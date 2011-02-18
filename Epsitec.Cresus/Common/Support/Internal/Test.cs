//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.Internal
{
	public static class Test
	{
		public static CultureMap CreateCultureMap(IResourceAccessor owner, Druid id, CultureMapSource source)
		{
			return new CultureMap (owner, id, source);
		}
	}
}
