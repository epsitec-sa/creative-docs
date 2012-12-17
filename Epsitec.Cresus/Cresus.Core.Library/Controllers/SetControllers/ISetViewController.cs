using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using System;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Controllers.SetControllers
{
	public interface ISetViewController : IDisposable
	{
		FormattedText GetTitle();

		string GetIcon();

		Druid GetDisplayDataSetId();

		Druid GetPickDataSetId();

		DataSetAccessor GetDisplayDataSetAccessor(DataSetGetter dataSetGetter, DataStoreMetadata dataStoreMetadata);

		DataSetAccessor GetPickDataSetAccessor(DataSetGetter dataSetGetter, DataStoreMetadata dataStoreMetadata);

		void AddItems(IEnumerable<AbstractEntity> entitiesToAdd);

		void RemoveItems(IEnumerable<AbstractEntity> entitiesToRemove);
	}
}