//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IContextResolver</c> can be used to map between objects and markup
	/// tags. This interface is implemented by <c>Serialization.Context</c>.
	/// </summary>
	public interface IContextResolver
	{
		/// <summary>
		/// Resolves the object to its corresponding markup.
		/// </summary>
		/// <param name="value">The object to convert to a markup string.</param>
		/// <returns>The markup string.</returns>
		string ResolveToMarkup(object value);

		/// <summary>
		/// Resolves the object from its corresponding markup.
		/// </summary>
		/// <param name="markup">The markup string to convert to an object.</param>
		/// <param name="type">The expected type.</param>
		/// <returns>The object.</returns>
		object ResolveFromMarkup(string markup, System.Type type);

		/// <summary>
		/// Gets the external map used to map referemces to external objects.
		/// </summary>
		/// <value>The external map.</value>
		Serialization.Generic.MapTag<object> ExternalMap
		{
			get;
		}
	}
}
