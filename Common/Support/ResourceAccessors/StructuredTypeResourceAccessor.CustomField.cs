/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass(
    typeof(Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor)
)]

namespace Epsitec.Common.Support.ResourceAccessors
{
    public partial class StructuredTypeResourceAccessor : CaptionResourceAccessor
    {
        private sealed class CustomField : StructuredTypeField
        {
            public CustomField(
                Druid typeId,
                Druid captionId,
                int rank,
                FieldRelation relation,
                FieldMembership membership,
                FieldSource source,
                FieldOptions options,
                string expression,
                System.Func<Druid, INamedType> namedTypeResolver
            )
                : base(
                    null,
                    null,
                    captionId,
                    rank,
                    relation,
                    membership,
                    source,
                    options,
                    expression
                )
            {
                this.namedTypeResolver = namedTypeResolver;
                this.DefineTypeId(typeId);
            }

            public override INamedType Type
            {
                get
                {
                    if (this.type == null)
                    {
                        if (this.namedTypeResolver == null)
                        {
                            throw new System.InvalidOperationException(
                                "Trying to read inexistant type information"
                            );
                        }

                        this.type = this.namedTypeResolver(this.typeId);
                    }

                    return this.type;
                }
            }

            private readonly System.Func<Druid, INamedType> namedTypeResolver;
        }
    }
}
