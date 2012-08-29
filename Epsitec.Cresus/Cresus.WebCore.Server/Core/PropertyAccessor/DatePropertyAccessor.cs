using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System;

using System.Globalization;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class DatePropertyAccessor : AbstractStringPropertyAccessor
	{
		
		
		public DatePropertyAccessor(LambdaExpression lambda, int id)
			: base (lambda, id)
		{		
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.Date;
			}
		}


		protected override string ConvertValueToText(object value)
		{
			return this.Convert ((string) value);
		}


		protected override object ConvertTextToValue(string text)
		{
			return this.Convert (text);
		}


		private string Convert(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}

			var date = DateTime.Parse (text).Date;
			var format = "dd.MM.yyyy";
			var culture = CultureInfo.InvariantCulture;

			return date.ToString (format, culture);
		}


	}


}
