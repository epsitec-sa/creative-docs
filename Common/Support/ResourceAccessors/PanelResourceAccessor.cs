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
    public class PanelResourceAccessor : AbstractFileResourceAccessor
    {
        public PanelResourceAccessor() { }

        protected override string GetResourceFileType()
        {
            return Resources.PanelTypeName;
        }

        protected override IStructuredType GetResourceType()
        {
            return Res.Types.ResourcePanel;
        }

        protected override void FillDataFromBundle(
            CultureMapSource source,
            StructuredData data,
            ResourceBundle bundle,
            ResourceBundle auxBundle
        )
        {
            ResourceBundle.Field panelSourceField = bundle[Strings.XmlSource];
            ResourceBundle.Field panelSizeField = bundle[Strings.DefaultSize];
            ResourceBundle.Field panelEntityIdField = bundle[Strings.RootEntityId];

            string panelSource = panelSourceField.IsValid ? panelSourceField.AsString : null;
            string panelSize = panelSizeField.IsValid ? panelSizeField.AsString : null;
            Druid panelEntityId = AbstractFileResourceAccessor.ToDruid(panelEntityIdField);

            data.SetValue(Res.Fields.ResourcePanel.XmlSource, panelSource);
            data.SetValue(Res.Fields.ResourcePanel.DefaultSize, panelSize);
            data.SetValue(Res.Fields.ResourcePanel.RootEntityId, panelEntityId);
        }

        protected override void FillData(StructuredData data)
        {
            data.SetValue(Res.Fields.ResourcePanel.DefaultSize, "");
            data.SetValue(Res.Fields.ResourcePanel.XmlSource, "");
            data.SetValue(Res.Fields.ResourcePanel.RootEntityId, Druid.Empty);
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
            string xmlSource = data.GetValue(Res.Fields.ResourcePanel.XmlSource) as string;
            string defaultSize = data.GetValue(Res.Fields.ResourcePanel.DefaultSize) as string;
            Druid rootEntityId = StructuredTypeResourceAccessor.ToDruid(
                data.GetValue(Res.Fields.ResourcePanel.RootEntityId)
            );

            bundle[Strings.XmlSource].SetXmlValue(xmlSource);
            bundle[Strings.DefaultSize].SetStringValue(defaultSize);
            bundle[Strings.RootEntityId]
                .SetStringValue(rootEntityId.IsValid ? rootEntityId.ToString() : "");
        }

        public static class Strings
        {
            public const string XmlSource = "Panel";
            public const string DefaultSize = "DefaultSize";
            public const string RootEntityId = "RootEntityId";
        }
    }
}
