//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;

	public class FormResourceAccessor : AbstractFileResourceAccessor
	{
		public FormResourceAccessor()
		{
		}

		protected override string GetResourceFileType()
		{
			return Resources.FormTypeName;
		}

		protected override IStructuredType GetResourceType()
		{
			return Res.Types.ResourceForm;
		}

		protected override void FillDataFromBundle(StructuredData data, ResourceBundle bundle)
		{
			ResourceBundle.Field panelSourceField = bundle[Strings.XmlSource];

			string panelSource = panelSourceField.IsValid ? panelSourceField.AsString : null;

			data.SetValue (Res.Fields.ResourceForm.XmlSource, panelSource);
		}

		protected override void FillData(StructuredData data)
		{
			data.SetValue (Res.Fields.ResourceForm.XmlSource, "");
		}

		protected override void SetupBundleFields(ResourceBundle bundle)
		{
			ResourceBundle.Field field;

			field = bundle.CreateField (ResourceFieldType.Data);
			field.SetName (Strings.XmlSource);
			bundle.Add (field);
		}

		protected override void SetBundleFields(ResourceBundle bundle, StructuredData data)
		{
			string xmlSource   = data.GetValue (Res.Fields.ResourceForm.XmlSource) as string;

			bundle[Strings.XmlSource].SetXmlValue (xmlSource);
		}

		private static class Strings
		{
			public const string XmlSource = "Source";
		}
	}
}
