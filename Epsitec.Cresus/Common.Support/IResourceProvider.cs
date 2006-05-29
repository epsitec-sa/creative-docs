//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>IResourceProvider</c> interface provides low level access to the
	/// resource data.
	/// </summary>
	public interface IResourceProvider
	{
		/// <summary>
		/// Gets the prefix of this resource provider.
		/// </summary>
		/// <value>The prefix of this resource provider (this is <c>"file"</c> for
		/// the file-based resource provider).</value>
		string Prefix
		{
			get;
		}

		/// <summary>
		/// Sets up the resource provider for the specified resource manager.
		/// This is called only once in the life time of the provider.
		/// </summary>
		/// <param name="resourceManager">The resource manager which will use this
		/// resource provider.</param>
		void Setup(ResourceManager resourceManager);

		/// <summary>
		/// Selects the specified module.
		/// </summary>
		/// <param name="module">The module to select.</param>
		/// <returns><c>true</c> if the module could be selected; otherwise,
		/// <c>false</c>.</returns>
		bool SelectModule(ResourceModuleInfo module);

		/// <summary>
		/// Selects the locale to be used.
		/// </summary>
		/// <param name="culture">The culture of the locale.</param>
		void SelectLocale(System.Globalization.CultureInfo culture);

		/// <summary>
		/// Validates the resource bundle identifier.
		/// </summary>
		/// <param name="id">The resource bundle identifier.</param>
		/// <returns><c>true</c> if the resource bundle identifier is valid; otherwise,
		/// <c>false</c>.</returns>
		bool ValidateId(string id);

		/// <summary>
		/// Determines whether the resource provider can locate the bundle with the
		/// specified identifier.
		/// </summary>
		/// <param name="id">The resource bundle identifier.</param>
		/// <returns><c>true</c> the resource provider can locate the specified resource
		/// bundle identifier; otherwise, <c>false</c>.</returns>
		bool Contains(string id);

		/// <summary>
		/// Gets the modules which can be accessed by this resource provider.
		/// </summary>
		/// <returns>An array of modules which can be accessed by this
		/// resource provider.</returns>
		ResourceModuleInfo[] GetModules();

		/// <summary>
		/// Gets the resource bundle identifiers matching several criteria.
		/// </summary>
		/// <param name="nameFilter">The name filter.</param>
		/// <param name="typeFilter">The type filter.</param>
		/// <param name="level">The localization level (all, default, localized, customized).</param>
		/// <param name="culture">The culture for the locale.</param>
		/// <returns>The resource bundle identifiers matching the criteria.</returns>
		string[] GetIds(string nameFilter, string typeFilter, ResourceLevel level, System.Globalization.CultureInfo culture);
		
		byte[] GetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
		bool SetData(string id, ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, ResourceSetMode mode);
		bool Remove(string id, ResourceLevel level, System.Globalization.CultureInfo culture);
	}
}
