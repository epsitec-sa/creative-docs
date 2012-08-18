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
		private sealed class FieldUpdater
		{
			public FieldUpdater(StructuredTypeResourceAccessor host)
			{
				this.host         = host;
				this.ids          = new List<Druid> ();
				this.interfaceIds = new List<Druid> ();
				this.fields       = new List<StructuredData> ();
			}

			public IList<StructuredData> Fields
			{
				get
				{
					return this.fields;
				}
			}

			public IList<Druid> InterfaceIds
			{
				get
				{
					return this.interfaceIds;
				}
			}

			/// <summary>
			/// Includes the interface id in the scan.
			/// </summary>
			/// <param name="interfaceId">The interface id.</param>
			public void IncludeInterfaceId(Druid interfaceId)
			{
				if (this.interfaceIds.Contains (interfaceId))
				{
					//	Nothing to do, interface is already known
				}
				else
				{
					this.interfaceIds.Add (interfaceId);
				}
			}
			
			/// <summary>
			/// Includes all fields defined by the specified type, setting their
			/// membership accordingly.
			/// </summary>
			/// <param name="typeId">The type id.</param>
			/// <param name="membership">The top level field membership.</param>
			public void IncludeType(Druid typeId, FieldMembership membership)
			{
				this.IncludeType (typeId, typeId, membership, 0);
			}

			/// <summary>
			/// Includes all fields defined by the specified type, setting their
			/// membership accordingly.
			/// </summary>
			/// <param name="typeId">The type id.</param>
			/// <param name="definingTypeId">The type id of the defining type.</param>
			/// <param name="membership">The top level field membership.</param>
			/// <param name="depth">The recursion depth.</param>
			private void IncludeType(Druid typeId, Druid definingTypeId, FieldMembership membership, int depth)
			{
				StructuredData data = this.host.FindStructuredData (typeId);

				if (data == null)
				{
					return;
				}

				Druid                 baseId       = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceStructuredType.BaseType));
				IList<StructuredData> interfaceIds = data.GetValue (Res.Fields.ResourceStructuredType.InterfaceIds) as IList<StructuredData>;
				IList<StructuredData> fields       = data.GetValue (Res.Fields.ResourceStructuredType.Fields) as IList<StructuredData>;

				if (baseId.IsValid)
				{
					this.IncludeType (baseId, definingTypeId, FieldMembership.Inherited, depth+1);
				}

				if (interfaceIds != null)
				{
					foreach (StructuredData interfaceId in interfaceIds)
					{
						Druid id = StructuredTypeResourceAccessor.ToDruid (interfaceId.GetValue (Res.Fields.InterfaceId.CaptionId));

						this.IncludeType (id, definingTypeId, membership, depth+1);
						this.IncludeInterfaceId (id);
					}
				}

				if (fields != null)
				{
					foreach (StructuredData field in fields)
					{
						Druid fieldId = StructuredTypeResourceAccessor.ToDruid (field.GetValue (Res.Fields.Field.CaptionId));

						if (this.ids.Contains (fieldId))
						{
							continue;
						}

						StructuredData copy = new StructuredData (Res.Types.Field);

						copy.SetValue (Res.Fields.Field.TypeId,             field.GetValue (Res.Fields.Field.TypeId));
						copy.SetValue (Res.Fields.Field.CaptionId,          field.GetValue (Res.Fields.Field.CaptionId));
						copy.SetValue (Res.Fields.Field.Relation,           field.GetValue (Res.Fields.Field.Relation));
						copy.SetValue (Res.Fields.Field.Membership,         membership);
						copy.SetValue (Res.Fields.Field.CultureMapSource,   field.GetValue (Res.Fields.Field.CultureMapSource));
						copy.SetValue (Res.Fields.Field.Source,             field.GetValue (Res.Fields.Field.Source));
						copy.SetValue (Res.Fields.Field.Options,            field.GetValue (Res.Fields.Field.Options));
						copy.SetValue (Res.Fields.Field.Expression,         field.GetValue (Res.Fields.Field.Expression));
						copy.SetValue (Res.Fields.Field.DefiningTypeId,     definingTypeId);
						copy.SetValue (Res.Fields.Field.DeepDefiningTypeId, typeId);
						
						copy.LockValue (Res.Fields.Field.DefiningTypeId);
						copy.LockValue (Res.Fields.Field.DeepDefiningTypeId);

						this.ids.Add (fieldId);
						this.fields.Add (copy);
					}
				}
			}

			private StructuredTypeResourceAccessor host;
			private List<Druid> ids;
			private List<Druid> interfaceIds;
			private List<StructuredData> fields;
		}
	}
}
