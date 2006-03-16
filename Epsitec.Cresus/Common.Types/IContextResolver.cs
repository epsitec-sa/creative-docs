//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public interface IContextResolver
	{
		string ResolveToMarkup(object value);
		object ResolveFromMarkup(string id);
	}
}
