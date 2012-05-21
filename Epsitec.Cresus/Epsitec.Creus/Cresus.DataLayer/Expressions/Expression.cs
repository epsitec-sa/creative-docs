using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>Expression</c> class is used to represent a logical expression such as
	/// ((a != b) and (c = 1)).
	/// </summary>
	public abstract class Expression
	{


		/// <summary>
		/// Creates an <see cref="SqlFunction"/> that corresponds to this instance.
		/// </summary>
		/// <param name="builder">The object used to build <see cref="SqlField"/> instances.</param>
		internal abstract SqlFunction CreateSqlCondition(SqlFieldBuilder builder);


		/// <summary>
		/// Checks that the fields contained by this instance are valid.
		/// </summary>
		/// <param name="checker">The <see cref="FieldChecker"/> instance used to check the fields.</param>
		internal abstract void CheckFields(FieldChecker checker);


	}


}
