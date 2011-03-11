//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataSetCollectionGetter</c> delegate is used to retrieve all items found
	/// in a data set as a collection of entities.
	/// </summary>
	/// <param name="dataContext">The data context.</param>
	/// <returns>The collection of entities.</returns>
	public delegate IEnumerable<AbstractEntity> DataSetCollectionGetter(DataContext dataContext);
}
