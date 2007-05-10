//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>CaptionResourceAccessor</c> is used to access text resources,
	/// stored in the <c>Strings</c> resource bundle.
	/// </summary>
	public class CaptionResourceAccessor : AbstractCaptionResourceAccessor
	{
		public CaptionResourceAccessor()
		{
		}

		protected override string GetNameFromFieldName(string fieldName)
		{
			System.Diagnostics.Debug.Assert (fieldName.StartsWith ("Cap."));
			return fieldName.Substring (4);
		}

		protected override string GetFieldNameFromName(StructuredData data, string name)
		{
			return "Cap." + name;
		}

		protected override Caption GetCaptionFromData(StructuredData data)
		{
			string description = data.GetValue (Res.Fields.ResourceCaption.Description) as string;
			string icon = data.GetValue (Res.Fields.ResourceCaption.Icon) as string;

			Caption caption = new Caption ();

			foreach (string label in data.GetValue (Res.Fields.ResourceCaption.Labels) as IEnumerable<string>)
			{
				caption.Labels.Add (label);
			}

			caption.Description = description;
			caption.Icon = icon;

			return caption;
		}

		protected override void FillDataFromCaption(StructuredData data, Caption caption)
		{
			List<string> labels = new List<string> ();
			labels.AddRange (caption.Labels);

			data.SetValue (Res.Fields.ResourceCaption.Labels, labels);

			if (caption.Description != null)
			{
				data.SetValue (Res.Fields.ResourceCaption.Description, caption.Description);
			}
			if (caption.Icon != null)
			{
				data.SetValue (Res.Fields.ResourceCaption.Icon, caption.Icon);
			}
		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			return (!string.IsNullOrEmpty (field.Name))
				&& (field.Name.StartsWith ("Cap."));
		}
	}
}
