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
	public class StructuredTypeResourceAccessor : CaptionResourceAccessor
	{
		public StructuredTypeResourceAccessor()
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
			Caption     caption = base.GetCaptionFromData (data, name);
			StructuredType type = this.GetTypeFromData (data);

			AbstractType.SetComplexType (caption, type);
			
			return caption;
		}

		private StructuredType GetTypeFromData(StructuredData data)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		protected override void FillDataFromCaption(CultureMap item, Types.StructuredData data, Caption caption)
		{
			base.FillDataFromCaption (item, data, caption);

			StructuredType type = AbstractType.GetComplexType (caption) as StructuredType;

			if (type != null)
			{
				this.FillDataFromType (item, data, type);
			}
		}

		private void FillDataFromType(CultureMap item, StructuredData data, StructuredType type)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		protected override bool FilterField(ResourceBundle.Field field)
		{
			return base.FilterField (field);
		}


		private class FieldBroker : IDataBroker
		{
			#region IDataBroker Members

			public StructuredData CreateData(CultureMap container)
			{
			}

			#endregion
		}
}
}
