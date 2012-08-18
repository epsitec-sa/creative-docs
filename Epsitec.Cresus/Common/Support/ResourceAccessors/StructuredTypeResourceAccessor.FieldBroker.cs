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
		private sealed class FieldBroker : IDataBroker
		{
			public FieldBroker(StructuredTypeResourceAccessor accessor)
			{
				this.accessor = accessor;
			}

			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
				StructuredData data = new StructuredData (Res.Types.Field);
				CultureMapSource source = container.Source;

				if (this.accessor.BasedOnPatchModule)
				{
					source = CultureMapSource.PatchModule;
				}

				data.SetValue (Res.Fields.Field.TypeId, Druid.Empty);
				data.SetValue (Res.Fields.Field.CaptionId, Druid.Empty);
				data.SetValue (Res.Fields.Field.Relation, FieldRelation.None);
				data.SetValue (Res.Fields.Field.Membership, FieldMembership.Local);
				data.SetValue (Res.Fields.Field.CultureMapSource, source);
				data.SetValue (Res.Fields.Field.Source, FieldSource.Value);
				data.SetValue (Res.Fields.Field.Options, FieldOptions.None);
				data.SetValue (Res.Fields.Field.Expression, "");
				data.SetValue (Res.Fields.Field.DefiningTypeId, Druid.Empty);
				data.SetValue (Res.Fields.Field.DeepDefiningTypeId, Druid.Empty);
				data.LockValue (Res.Fields.Field.DefiningTypeId);
				data.LockValue (Res.Fields.Field.DeepDefiningTypeId);
				
				return data;
			}

			#endregion

			private StructuredTypeResourceAccessor accessor;
		}
	}
}
