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
		protected sealed class FieldListener : Listener
		{
			public FieldListener(StructuredTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public bool HasSnapshotData
			{
				get
				{
					return this.originalFields != null;
				}
			}

			public void CreateSnapshot()
			{
				if (this.SaveField (Res.Fields.ResourceStructuredType.Fields))
				{
					this.originalFields = new List<StructuredData> ();

					IList<StructuredData> fields = this.Data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

					foreach (StructuredData data in fields)
					{
						this.originalFields.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void HandleCollectionChanging(object sender)
			{
				this.CreateSnapshot ();
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalFields != null)
				{
					this.RestoreField (Res.Fields.ResourceStructuredType.Fields);

					ObservableList<StructuredData> fields = this.Data.GetValue (Res.Fields.ResourceStructuredType.Fields) as ObservableList<StructuredData>;
					List<Druid> fieldIds = new List<Druid> ();

					using (fields.DisableNotifications ())
					{
						int index = fields.Count - 1;
						
						while (index >= 0)
						{
							StructuredData data = fields[index];

							if (FieldListener.IsLocalField (data))
							{
								fieldIds.Add (StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.Field.CaptionId)));
							}
							
							fields.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (fields.Count == 0);

						foreach (StructuredData data in this.originalFields)
						{
							if (FieldListener.IsLocalField (data))
							{
								fieldIds.Remove (StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.Field.CaptionId)));
							}
							
							StructuredData copy = data.GetShallowCopy ();
							fields.Add (copy);
							this.Item.NotifyDataAdded (copy);
							copy.PromoteToOriginal ();
						}
					}

					StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;
					IResourceAccessor fieldAccessor = accessor.FieldAccessor;
					
					if (fieldIds.Count > 0)
					{
						//	Some fields got orphaned while resetting to the original fields
						//	collection. We have to remove them from the field accessor too,
						//	or else we will accumulate dead fields over the time :

						foreach (Druid id in fieldIds)
						{
							CultureMap fieldItem = fieldAccessor.Collection[id];

							if (fieldItem != null)
							{
								fieldAccessor.Collection.Remove (fieldItem);
							}
						}
					}

					accessor.RefreshItem (this.Item);
				}
			}

			private static bool IsLocalField(StructuredData data)
			{
				Druid fieldDefiningType = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.Field.DefiningTypeId));
				FieldMembership membership = StructuredTypeResourceAccessor.Simplify ((FieldMembership) data.GetValue (Res.Fields.Field.Membership));
				bool? interfaceDefinition = StructuredTypeResourceAccessor.ToBoolean (data.GetValue (Res.Fields.Field.IsInterfaceDefinition));

				if (((membership == FieldMembership.Local) && (fieldDefiningType.IsEmpty)) ||
					((membership == FieldMembership.Local) && (interfaceDefinition.HasValue) && (interfaceDefinition.Value == false)))
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			private List<StructuredData> originalFields;
		}
	}
}
