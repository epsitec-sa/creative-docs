using Epsitec.Common.Support;

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


	}


}
