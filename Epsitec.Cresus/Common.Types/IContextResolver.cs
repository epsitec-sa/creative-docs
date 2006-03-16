//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public interface IContextResolver
	{
		string ResolveToId(object value);
		object ResolveFromId(string id);
	}
}
