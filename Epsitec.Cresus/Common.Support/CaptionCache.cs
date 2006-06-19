//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	public sealed class CaptionCache
	{
		private CaptionCache()
		{
		}

		public Caption GetCaption(ResourceManager manager, long id)
		{
			Druid druid = Druid.FromLong (id);
			return this.GetCaption (manager, druid, id);
		}

		public Caption GetCaption(ResourceManager manager, Druid druid)
		{
			long id = druid.ToLong ();
			return this.GetCaption (manager, druid, id);
		}

		private Caption GetCaption(ResourceManager manager, Druid druid, long id)
		{
			return null;
		}

		public static readonly CaptionCache Instance = new CaptionCache ();
	}
}
