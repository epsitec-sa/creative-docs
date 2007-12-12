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
			ResourceBundle.Field panelSourceField   = bundle[Strings.XmlSource];
			ResourceBundle.Field panelEntityIdField = bundle[Strings.RootEntityId];
			ResourceBundle.Field panelSizeField     = bundle[Strings.DefaultSize];

			string panelSource   = panelSourceField.IsValid ? panelSourceField.AsString : null;
			string panelSize     = panelSizeField.IsValid ? panelSizeField.AsString : null;
			Druid  panelEntityId = AbstractFileResourceAccessor.ToDruid (panelEntityIdField);

			data.SetValue (Res.Fields.ResourceForm.DefaultSize, panelSize);
			data.SetValue (Res.Fields.ResourceForm.XmlSource, panelSource);
			data.SetValue (Res.Fields.ResourceForm.RootEntityId, panelEntityId);
		}

		protected override void FillData(StructuredData data)
		{
			data.SetValue (Res.Fields.ResourceForm.DefaultSize, "");
			data.SetValue (Res.Fields.ResourceForm.XmlSource, "");
			data.SetValue (Res.Fields.ResourceForm.RootEntityId, Druid.Empty);
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

			field = bundle.CreateField (ResourceFieldType.Data);
			field.SetName (Strings.RootEntityId);
			bundle.Add (field);
		}

		protected override void SetBundleFields(ResourceBundle bundle, StructuredData data)
		{
			string xmlSource    = data.GetValue (Res.Fields.ResourceForm.XmlSource) as string;
			string defaultSize  = data.GetValue (Res.Fields.ResourceForm.DefaultSize) as string;
			Druid  rootEntityId = StructuredTypeResourceAccessor.ToDruid (data.GetValue (Res.Fields.ResourceForm.RootEntityId));

			bundle[Strings.XmlSource].SetXmlValue (xmlSource);
			bundle[Strings.DefaultSize].SetStringValue (defaultSize);
			bundle[Strings.RootEntityId].SetStringValue (rootEntityId.IsValid ? rootEntityId.ToString () : "");
		}

		public static class Strings
		{
			public const string XmlSource		= "Source";
			public const string DefaultSize		= "DefaultSize";
			public const string RootEntityId	= "RootEntityId";
		}
	}
}
