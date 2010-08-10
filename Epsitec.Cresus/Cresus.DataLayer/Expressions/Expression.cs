using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>Expression</c> class is used to represent a logical expression such as
	/// ((a != b) and (c = 1)).
	/// </summary>
	public abstract class Expression
	{


		/// <summary>
		/// Builds a new <c>Expression</c>.
		/// </summary>
		protected Expression()
		{
		}


		/// <summary>
		/// Gets the sequence of field ids that are used in this instance.
		/// </summary>
		/// <returns>The sequence of field ids that are used in this instance.</returns>
		internal abstract IEnumerable<Druid> GetFields();


		/// <summary>
		/// Converts this instance to an equivalent <see cref="DbAbstractCondition"/>, using a
		/// resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		internal abstract DbAbstractCondition CreateDbCondition(System.Func<Druid, DbTableColumn> columnResolver);


	}


}
