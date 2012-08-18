//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor))]

namespace Epsitec.Common.Support.ResourceAccessors
{
	public partial class StructuredTypeResourceAccessor : CaptionResourceAccessor
	{
		private sealed class InterfaceIdBroker : IDataBroker
		{
			public InterfaceIdBroker(StructuredTypeResourceAccessor accessor)
			{
				this.accessor = accessor;
			}

			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.InterfaceId);
				CultureMapSource source = container.Source;

				if (this.accessor.BasedOnPatchModule)
				{
					source = CultureMapSource.PatchModule;
				}

				data.SetValue (Res.Fields.InterfaceId.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.InterfaceId.CultureMapSource, source);
				
				return data;
			}

			#endregion

			private StructuredTypeResourceAccessor accessor;
		}
	}
}
