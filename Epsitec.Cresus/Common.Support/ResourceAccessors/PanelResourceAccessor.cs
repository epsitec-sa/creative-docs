//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo = System.Globalization.CultureInfo;

	public class PanelResourceAccessor : AbstractFileResourceAccessor
	{
		public PanelResourceAccessor()
		{
		}

		protected override string GetResourceFileType()
		{
			return Resources.PanelTypeName;
		}

		protected override IStructuredType GetResourceType()
		{
			return Res.Types.ResourcePanel;
		}

		protected override void FillDataFromBundle(StructuredData data, ResourceBundle bundle)
		{
			ResourceBundle.Field panelSourceField = bundle[Strings.XmlSource];
			ResourceBundle.Field panelSizeField   = bundle[Strings.DefaultSize];

			string panelSource = panelSourceField.IsValid ? panelSourceField.AsString : null;
			string panelSize   = panelSizeField.IsValid   ? panelSizeField.AsString   : null;

			data.SetValue (Res.Fields.ResourcePanel.XmlSource, panelSource);
			data.SetValue (Res.Fields.ResourcePanel.DefaultSize, panelSize);
		}

		protected override void FillData(StructuredData data)
		{
			data.SetValue (Res.Fields.ResourcePanel.DefaultSize, "");
			data.SetValue (Res.Fields.ResourcePanel.XmlSource, "");
		}

		protected override void SetupBundleFields(ResourceBundle bundle)
		{
			ResourceBundle.Field field;

			field = bundle.CreateField (ResourceFieldType.Data);
			field.SetName (Strings.XmlSource);
			bundle.Add (field);

			field = bundle.CreateField (ResourceFieldType.Data);
			field.SetName (Strings.DefaultSize);
			bundle.Add (field);
		}

		protected override void SetBundleFields(ResourceBundle bundle, StructuredData data)
		{
			string xmlSource   = data.GetValue (Res.Fields.ResourcePanel.XmlSource) as string;
			string defaultSize = data.GetValue (Res.Fields.ResourcePanel.DefaultSize) as string;
			
			bundle[Strings.XmlSource].SetXmlValue (xmlSource);
			bundle[Strings.DefaultSize].SetStringValue (defaultSize);
		}

		private static class Strings
		{
			public const string XmlSource = "Panel";
			public const string DefaultSize = "DefaultSize";
		}
	}
}
