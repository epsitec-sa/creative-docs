//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Graph
{
	public partial class GraphApplication
	{
		internal static class Paths
		{
			public static readonly string SettingsPath = System.IO.Path.Combine (Globals.Directories.UserAppData, "Cresus.Graph.settings.xml");
			public static readonly string AutoSavePath = System.IO.Path.Combine (Globals.Directories.UserAppData, "AutoSave");
		}
	}
}
