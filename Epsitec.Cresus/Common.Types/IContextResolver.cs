//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The IContextResolver can be used to map between objects and markup
	/// tags. This interface is implemented by Serialization.Context.
	/// </summary>
	public interface IContextResolver
	{
		string ResolveToMarkup(object value);
		object ResolveFromMarkup(string id);
	}
}
