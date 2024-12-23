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

namespace Epsitec.Common.Support.ResourceAccessors
{
    public class FormResourceAccessor : AbstractFileResourceAccessor
    {
        public FormResourceAccessor() { }

        protected override string GetResourceFileType()
        {
            return Resources.FormTypeName;
        }

        protected override IStructuredType GetResourceType()
        {
            return Res.Types.ResourceForm;
        }

        protected override void FillDataFromBundle(
            CultureMapSource source,
            StructuredData data,
            ResourceBundle bundle,
            ResourceBundle auxBundle
        )
        {
            ResourceBundle.Field formSourceField = bundle[Strings.XmlSource];
            ResourceBundle.Field formEntityIdField = bundle[Strings.RootEntityId];
            ResourceBundle.Field formSizeField = bundle[Strings.DefaultSize];

            string formSource = formSourceField.IsValid ? formSourceField.AsString : null;
            string formSize = formSizeField.IsValid ? formSizeField.AsString : null;
            Druid formEntityId = AbstractFileResourceAccessor.ToDruid(formEntityIdField);
            string formAuxSource = null;

            if (auxBundle != null)
            {
                ResourceBundle.Field formAuxSourceField = auxBundle[Strings.XmlSource];
                formAuxSource = formAuxSourceField.IsValid ? formAuxSourceField.AsString : null;
            }

            data.SetValue(Res.Fields.ResourceForm.DefaultSize, formSize);
            data.SetValue(Res.Fields.ResourceForm.RootEntityId, formEntityId);

            if (this.manager.BasedOnPatchModule)
            {
                switch (source)
                {
                    case CultureMapSource.ReferenceModule:
                        data.SetValue(Res.Fields.ResourceForm.XmlSource, "");
                        data.SetValue(Res.Fields.ResourceForm.XmlSourceAux, formAuxSource);
                        break;

                    case CultureMapSource.PatchModule:
                        data.SetValue(Res.Fields.ResourceForm.XmlSource, formSource);
                        data.SetValue(Res.Fields.ResourceForm.XmlSourceAux, "");
                        break;

                    case CultureMapSource.DynamicMerge:
                        data.SetValue(Res.Fields.ResourceForm.XmlSource, formSource);
                        data.SetValue(Res.Fields.ResourceForm.XmlSourceAux, formAuxSource);
                        break;
                }
            }
            else
            {
                data.SetValue(Res.Fields.ResourceForm.XmlSource, formSource);
            }
        }

        protected override void FillData(StructuredData data)
        {
            data.SetValue(Res.Fields.ResourceForm.DefaultSize, "");
            data.SetValue(Res.Fields.ResourceForm.XmlSource, "");
            data.SetValue(Res.Fields.ResourceForm.XmlSourceAux, "");
            data.SetValue(Res.Fields.ResourceForm.RootEntityId, Druid.Empty);
        }

        protected override void SetupBundleFields(ResourceBundle bundle)
        {
            ResourceBundle.Field field;

            field = bundle.CreateField(ResourceFieldType.Data);
            field.SetName(Strings.XmlSource);
            bundle.Add(field);

            field = bundle.CreateField(ResourceFieldType.Data);
            field.SetName(Strings.DefaultSize);
            bundle.Add(field);

            field = bundle.CreateField(ResourceFieldType.Data);
            field.SetName(Strings.RootEntityId);
            bundle.Add(field);
        }

        protected override void SetBundleFields(ResourceBundle bundle, StructuredData data)
        {
            string xmlSource = data.GetValue(Res.Fields.ResourceForm.XmlSource) as string;
            string defaultSize = data.GetValue(Res.Fields.ResourceForm.DefaultSize) as string;
            Druid rootEntityId = StructuredTypeResourceAccessor.ToDruid(
                data.GetValue(Res.Fields.ResourceForm.RootEntityId)
            );

            if (this.ForceModuleMerge)
            {
                xmlSource = data.GetValue(Res.Fields.ResourceForm.XmlSourceMerge) as string;
            }

            bundle[Strings.XmlSource].SetXmlValue(xmlSource);
            bundle[Strings.DefaultSize].SetStringValue(defaultSize);
            bundle[Strings.RootEntityId]
                .SetStringValue(rootEntityId.IsValid ? rootEntityId.ToString() : "");
        }

        public static class Strings
        {
            public const string XmlSource = "Source";
            public const string DefaultSize = "DefaultSize";
            public const string RootEntityId = "RootEntityId";
        }

        public static Drawing.Size GetFormDefaultSize(ResourceBundle bundle)
        {
            if (bundle == null)
            {
                return Drawing.Size.Empty;
            }
            else
            {
                string size = bundle[Strings.DefaultSize].AsString;

                if (string.IsNullOrEmpty(size))
                {
                    size = "*";
                }

                return Drawing.Size.Parse(size);
            }
        }
    }
}
