//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.PlugIns
{
	/// <summary>
	/// The <c>IPlugInAttribute</c> generic interface must be implemented by the
	/// attribute classes which interact with the <see cref="PlugInFactory"/>.
	/// </summary>
	/// <typeparam name="TId">The type of the id.</typeparam>
	public interface IPlugInAttribute<TId>
	{
		/// <summary>
		/// Gets the id of the class described by this attribute.
		/// </summary>
		/// <value>The id.</value>
		TId Id
		{
			get;
		}

		/// <summary>
		/// Gets the type of the class described by this attribute.
		/// </summary>
		/// <value>The type.</value>
		System.Type Type
		{
			get;
		}
	}
}
