//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core
{
	public partial class CoreApplication
	{
		internal static class Paths
		{
			public static readonly string SettingsPath = System.IO.Path.Combine (Globals.Directories.UserAppData, "Cresus Core settings.xml");
		}
	}
}
