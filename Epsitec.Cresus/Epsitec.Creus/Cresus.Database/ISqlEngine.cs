//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>ISqlEngine</c> interface can be used to execute SQL commands
	/// produced by <see cref="ISqlBuilder"/>.
	/// </summary>
	public interface ISqlEngine
	{
		/// <summary>
		/// Executes the specified command. Use this if you are expecting a scalar result.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="type">The command type.</param>
		/// <param name="commandCount">The number of commands.</param>
		/// <param name="result">The execution result.</param>
		void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out int result);

		/// <summary>
		/// Executes the specified command. Use this if you are expecting a single object
		/// as result.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="type">The command type.</param>
		/// <param name="commandCount">The number of commands.</param>
		/// <param name="simpleData">The execution result.</param>
		void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out object simpleData);

		/// <summary>
		/// Executes the specified command. Use this if your command uses output parameters.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="type">The command type.</param>
		/// <param name="commandCount">The number of commands.</param>
		/// <param name="outputValues">The values of the output parameters.</param>
		void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out IList<object> outputValues);

		/// <summary>
		/// Executes the specified command. Use this if you are expecting a full <c>DataSet</c>
		/// as result.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="type">The command type.</param>
		/// <param name="commandCount">The number of commands.</param>
		/// <param name="dataSet">The execution result.</param>
		void Execute(System.Data.IDbCommand command, DbCommandType type, int commandCount, out System.Data.DataSet dataSet);
	}
}
