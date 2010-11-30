//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlFieldType</c> enumeration defines all possible field types.
	/// </summary>
	public enum SqlFieldType
	{
		/// <summary>
		/// Unknown or undefined field type.
		/// </summary>
		Unknown,
			
		/// <summary>
		/// The NULL contant.
		/// </summary>
		Null,

		/// <summary>
		/// The * contant used by aggregates functions.
		/// </summary>
		All,

		/// <summary>
		/// The DEFAULT constant used by the INSERT INTO command.
		/// </summary>
		Default,
		
		/// <summary>
		/// A constant value compatible with <c>DbRawType</c>.
		/// </summary>
		Constant,
							
		/// <summary>
		/// Input parameter for a stored procedure; this is a constant.
		/// </summary>
		ParameterIn = Constant,

		/// <summary>
		/// Output parameter for a stored procedure.
		/// </summary>
		ParameterOut,

		/// <summary>
		/// Input and output parameter for a stored procedure.
		/// </summary>
		ParameterInOut,
		
		/// <summary>
		/// Result parameter for a stored procedure.
		/// </summary>
		ParameterResult,
			
		/// <summary>
		/// Simple name (column name, table name, procedure name, etc.)
		/// </summary>
		Name,
		
		/// <summary>
		/// Qualified name (table name followed by a column name).
		/// </summary>
		QualifiedName,
										
		/// <summary>
		/// Aggregate function.
		/// </summary>
		Aggregate,
		
		/// <summary>
		/// Not supported yet: an SQL variable.
		/// </summary>
		Variable,
		
		/// <summary>
		/// An SQL function.
		/// </summary>
		Function,

		/// <summary>
		/// Not supported yet: an SQL procedure.
		/// </summary>
		Procedure,
		
		/// <summary>
		/// A sub query (used by a SELECT command).
		/// </summary>
		SubQuery,

		/// <summary>
		/// A join (used by a SELECT command).
		/// </summary>
		Join,

		/// <summary>
		/// A raw sql query snippet.
		/// </summary>
		RawSql,

		/// <summary>
		/// A set of constants. 
		/// </summary>
		Set,

	}									
}
