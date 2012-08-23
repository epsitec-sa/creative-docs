using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

using System.Text.RegularExpressions;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	///  The <c>Constant</c> class represents a constant value that can be used in an
	///  <see cref="Expression"/>.
	/// </summary>
	public sealed class Constant : Value
	{


		/// <summary>
		/// Builds a new <c>Constant</c>.
		/// </summary>
		/// <param name="value">The value of the <c>Constant</c>.</param>
		public Constant(object value)
		{
			this.Value = value;
		}
		
		
		/// <summary>
		/// The value of the <c>Constant</c>.
		/// </summary>
		public object Value
		{
			get;
			private set;
		}


		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			return builder.BuildConstantForField (this.Value);
		}


		internal override void CheckField(FieldChecker checker)
		{
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
		}


		/// <summary>
		/// Escapes a string so that it can be used with the <see cref="BinaryComparator"/> dealing
		/// with escaped strings.
		/// </summary>
		/// <param name="value">The string to escape.</param>
		/// <returns>The escaped string.</returns>
		public static string Escape(string value)
		{
			string escapeChar = DbSqlStandard.CompareLikeEscape;

			string pattern = "([%_" + escapeChar + "])";
			string replacement = escapeChar + "$1";

			return Regex.Replace (value, pattern, replacement);
		}


	}


}
