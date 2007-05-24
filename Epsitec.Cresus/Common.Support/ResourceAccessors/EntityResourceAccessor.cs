//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.ResourceAccessors
{
	using CultureInfo=System.Globalization.CultureInfo;
	
	/// <summary>
	/// The <c>EntityResourceAccessor</c> is used to access entity resources,
	/// stored in the <c>Captions</c> resource bundle and which have a field
	/// name prefixed with <c>"Typ."</c>.
	/// </summary>
	public class EntityResourceAccessor : CaptionResourceAccessor
	{
		public EntityResourceAccessor()
		{
		}

		public override IDataBroker GetDataBroker(StructuredData container, string fieldId)
		{
			return base.GetDataBroker (container, fieldId);
		}

		protected override string Prefix
		{
			get
			{
				return "Typ.";
			}
		}

		protected override IStructuredType GetStructuredType()
		{
			return Res.Types.ResourceEntity;
		}

		protected override Caption GetCaptionFromData(Types.StructuredData data, string name)
		{
			Caption caption = base.GetCaptionFromData (data, name);

			//	TODO: hook structured type with caption
			
			return caption;
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			base.FillDataFromCaption (item, data, caption);

			//	TODO: extract structured type information from caption
		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			return base.FilterField (field);
		}
	}
}
