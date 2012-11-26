using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class DateField : AbstractField
	{


		public Date? Value
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "dateField";
		}


		protected override object GetValue()
		{
			return ValueConverter.ConvertFieldToClientForDate (this.Value);
		}


	}


}

