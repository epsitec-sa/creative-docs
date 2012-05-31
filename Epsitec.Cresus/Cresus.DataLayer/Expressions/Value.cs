//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public abstract class Value
	{
		
		
		/// <summary>
		/// Creates an <see cref="SqlField"/> that corresponds to this instance.
		/// </summary>
		/// <param name="builder">The object used to build <see cref="SqlField"/> instances.</param>
		internal abstract SqlField CreateSqlField(SqlFieldBuilder builder);

		
		/// <summary>
		/// Checks that the field contained by this instance is valid.
		/// </summary>
		/// <param name="checker">The <see cref="FieldChecker"/> instance used to check the field.</param>
		internal abstract void CheckField(FieldChecker checker);


		/// <summary>
		/// Adds the entities thate are referenced within this instance to the given set.
		/// </summary>
		/// <param name="entities">The set in which to add the entities.</param>
		internal abstract void AddEntities(HashSet<AbstractEntity> entities);
	
	
	}


}
