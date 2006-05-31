//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IContextResolver</c> can be used to map between objects and markup
	/// tags. This interface is implemented by <c>Serialization.Context</c>.
	/// </summary>
	public interface IContextResolver
	{
		string ResolveToMarkup(object value);
		object ResolveFromMarkup(string id);
	}
}
