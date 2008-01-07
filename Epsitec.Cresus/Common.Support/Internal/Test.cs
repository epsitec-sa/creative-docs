//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
