//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlAggregate décrit une fonction de type aggrégat SQL. La
	/// propriété Field peut faire référence à une colonne, une constante
	/// ou une fonction, mais pas à un autre aggrégat.
	/// </summary>
	public class SqlAggregate
	{
		public SqlAggregate(SqlAggregateType type, SqlField field)
		{
			switch (field.Type)
			{
				case SqlFieldType.All:
				case SqlFieldType.Constant:
				case SqlFieldType.QualifiedName:
				case SqlFieldType.Function:
				case SqlFieldType.Procedure:
					break;
				
				default:
					throw new System.NotSupportedException ("SqlField type not supported");
			}
			
			this.type = type;
			this.field = field;
		}
		
		public SqlAggregateType			Type
		{
			get { return this.type; }
		}
		
		public SqlField					Field
		{
			get { return this.field; }
		}
		
		
		protected SqlAggregateType		type	= SqlAggregateType.Unsupported;
		protected SqlField				field	= null;
	}
	
	
	public enum SqlAggregateType
	{
		Unsupported,
		
		Count,
		Min,
		Max,
		Average,
		Sum
	}
}
