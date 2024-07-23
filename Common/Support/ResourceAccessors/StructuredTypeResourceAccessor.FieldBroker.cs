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
        private sealed class FieldBroker : IDataBroker
        {
            public FieldBroker(StructuredTypeResourceAccessor accessor)
            {
                this.accessor = accessor;
            }

            #region IDataBroker Members

            public StructuredData CreateData(CultureMap container)
            {
                StructuredData data = new StructuredData(Res.Types.Field);
                CultureMapSource source = container.Source;

                if (this.accessor.BasedOnPatchModule)
                {
                    source = CultureMapSource.PatchModule;
                }

                data.SetValue(Res.Fields.Field.TypeId, Druid.Empty);
                data.SetValue(Res.Fields.Field.CaptionId, Druid.Empty);
                data.SetValue(Res.Fields.Field.Relation, FieldRelation.None);
                data.SetValue(Res.Fields.Field.Membership, FieldMembership.Local);
                data.SetValue(Res.Fields.Field.CultureMapSource, source);
                data.SetValue(Res.Fields.Field.Source, FieldSource.Value);
                data.SetValue(Res.Fields.Field.Options, FieldOptions.None);
                data.SetValue(Res.Fields.Field.Expression, "");
                data.SetValue(Res.Fields.Field.DefiningTypeId, Druid.Empty);
                data.SetValue(Res.Fields.Field.DeepDefiningTypeId, Druid.Empty);
                data.LockValue(Res.Fields.Field.DefiningTypeId);
                data.LockValue(Res.Fields.Field.DeepDefiningTypeId);

                return data;
            }

            #endregion

            private StructuredTypeResourceAccessor accessor;
        }
    }
}
