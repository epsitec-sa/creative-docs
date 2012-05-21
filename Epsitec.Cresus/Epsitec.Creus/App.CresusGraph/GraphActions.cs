//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


namespace Epsitec.Cresus.Graph
{
	public static class GraphActions
	{
		public static System.Action DocumentReload;
		public static System.Action<string> DocumentSelectDataSource;
		public static System.Action<string> DocumentIncludeFilterCategory;
		public static System.Action<string> DocumentExcludeFilterCategory;
		public static System.Action<string> DocumentAddSeriesToOutput;
		public static System.Action<string> DocumentRemoveSeriesFromOutput;
		public static System.Action<string, int> DocumentSetSeriesOutputIndex;
		public static System.Action<string> DocumentHideSnapshot;
		public static System.Action<int, string> DocumentDefineColor;
	}
}
