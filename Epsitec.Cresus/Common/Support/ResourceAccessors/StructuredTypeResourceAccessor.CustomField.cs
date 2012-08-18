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
		private sealed class CustomField : StructuredTypeField
		{
			public CustomField(Druid typeId, Druid captionId, int rank, FieldRelation relation, FieldMembership membership, FieldSource source, FieldOptions options, string expression, System.Func<Druid, INamedType> namedTypeResolver)
				: base (null, null, captionId, rank, relation, membership, source, options, expression)
			{
				this.namedTypeResolver = namedTypeResolver;
				this.DefineTypeId (typeId);
			}

			public override INamedType Type
			{
				get
				{
					if (this.type == null)
					{
						if (this.namedTypeResolver == null)
						{
							throw new System.InvalidOperationException ("Trying to read inexistant type information");
						}

						this.type = this.namedTypeResolver (this.typeId);
					}

					return this.type;
				}
			}

			private readonly System.Func<Druid, INamedType> namedTypeResolver;
		}
	}
}
