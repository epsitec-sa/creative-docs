//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataValue donne acc�s � une donn�e dans un IDataGraph.
	/// </summary>
	public interface IDataValue : IDataItem
	{
		INamedType		DataType		{ get; }
		IDataConstraint	DataConstraint	{ get; }
		bool			IsValueValid	{ get; }
		
		event Support.EventHandler	Changed;
		
		object ReadValue();
		void WriteValue(object value);
		
		void NotifyInvalidData();
	}
}

