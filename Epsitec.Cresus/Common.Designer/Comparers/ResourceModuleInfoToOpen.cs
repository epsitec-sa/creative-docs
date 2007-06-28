using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux ResourceModuleInfoToOpen.
	/// </summary>
	public class ResourceModuleInfoToOpen : IComparer<ResourceModuleInfo>
	{
		public int Compare(ResourceModuleInfo obj1, ResourceModuleInfo obj2)
		{
			string s1 = obj1.FullId.Path;
			string s2 = obj2.FullId.Path;

			return s1.CompareTo(s2);
		}
	}
}
