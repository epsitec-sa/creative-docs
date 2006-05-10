//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The IStructuredData interface provides a DependencyObject compatible way
	/// of accessing structured data (i.e. records, graphs, etc.)
	/// </summary>
	public interface IStructuredData
	{
		void AttachListener(string path, PropertyChangedEventHandler handler);
		void DetachListener(string path, PropertyChangedEventHandler handler);

		object GetValue(string path);
		void SetValue(string path, object value);

		bool HasImmutableRoots
		{
			get;
		}
	}
}
