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
		protected sealed class InterfaceListener : Listener
		{
			public InterfaceListener(StructuredTypeResourceAccessor accessor, CultureMap item, StructuredData data)
				: base (accessor, item, data)
			{
			}

			public bool HasSnapshotData
			{
				get
				{
					return this.originalInterfaceIds != null;
				}
			}

			public void CreateSnapshot()
			{
				if (this.SaveField (Res.Fields.ResourceStructuredType.InterfaceIds))
				{
					this.originalInterfaceIds = new List<StructuredData> ();

					IList<StructuredData> interfaceIds = this.Data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;

					foreach (StructuredData data in interfaceIds)
					{
						this.originalInterfaceIds.Add (data.GetShallowCopy ());
					}
				}
			}

			public override void HandleCollectionChanging(object sender)
			{
				this.CreateSnapshot ();
			}

			public override void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
			{
				StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;
				accessor.RefreshItem (this.Item);
				
				base.HandleCollectionChanged (sender, e);
			}

			public override void ResetToOriginalValue()
			{
				if (this.originalInterfaceIds != null)
				{
					this.RestoreField (Res.Fields.ResourceStructuredType.InterfaceIds);

					ObservableList<StructuredData> interfaceIds = this.Data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as ObservableList<StructuredData>;

					using (interfaceIds.DisableNotifications ())
					{
						int index = interfaceIds.Count - 1;

						while (index >= 0)
						{
							StructuredData data = interfaceIds[index];
							interfaceIds.RemoveAt (index--);
							this.Item.NotifyDataRemoved (data);
						}

						System.Diagnostics.Debug.Assert (interfaceIds.Count == 0);

						foreach (StructuredData data in this.originalInterfaceIds)
						{
							StructuredData copy = data.GetShallowCopy ();
							interfaceIds.Add (copy);
							this.Item.NotifyDataAdded (copy);
							copy.PromoteToOriginal ();
						}
					}

					StructuredTypeResourceAccessor accessor = this.Accessor as StructuredTypeResourceAccessor;
					accessor.RefreshItem (this.Item);
				}
			}

			private List<StructuredData> originalInterfaceIds;
		}
	}
}
