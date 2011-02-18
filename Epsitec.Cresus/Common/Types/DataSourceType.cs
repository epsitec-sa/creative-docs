//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DataSourceType</c> enumeration describes the binding type used
	/// by <see cref="T:BindingExpression"/> to attach to the data source.
	/// </summary>
	public enum DataSourceType : byte
	{
		/// <summary>
		/// The data source is not defined.
		/// </summary>
		None,
		
		/// <summary>
		/// The data source is defined as a <c>DependencyProperty</c> on a <c>DependencyObject</c>.
		/// </summary>
		PropertyObject,
		
		/// <summary>
		/// The data source is defined as a field on a <c>IStructuredData</c> object.
		/// </summary>
		StructuredData,
		
		/// <summary>
		/// The data source is defined as the binding source itself.
		/// </summary>
		SourceItself,
		
		/// <summary>
		/// The data source is defined as a resource provided by <c>IResourceProvider</c>.
		/// </summary>
		Resource
	}
}
