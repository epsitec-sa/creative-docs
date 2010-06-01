using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Widgets.Tiles;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public interface ICollectionTemplate
	{
		string NamePrefix
		{
			get;
		}
		void BindCreateItem(SummaryData data, ICollectionAccessor collectionAccessor);
		AbstractEntity CreateItem();
		void DeleteItem(AbstractEntity item);
	}
}
