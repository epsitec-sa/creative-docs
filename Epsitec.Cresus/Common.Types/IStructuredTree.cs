//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// The IStructuredTree interface makes the metadata about the tree accessible.
	/// </summary>
	public interface IStructuredTree
	{
		object GetFieldTypeObject(string name);
		
		string[] GetFieldNames();
		string[] GetFieldPaths(string path);
	}
}
