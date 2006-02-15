//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataValue donne accès à une donnée dans un IDataGraph.
	/// </summary>
	public interface IDataValue : IDataItem
	{
		INamedType		DataType		{ get; }
		IDataConstraint	DataConstraint	{ get; }
		
		event Support.EventHandler	Changed;
		
		object ReadValue();
		void WriteValue(object value);
	}
}

