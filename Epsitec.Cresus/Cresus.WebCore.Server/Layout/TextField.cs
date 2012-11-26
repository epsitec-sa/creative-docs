using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class TextField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		public bool IsPassword
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "textField";
		}


		protected override object GetValue()
		{
			return ValueConverter.ConvertFieldToClientForText (this.Value);
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["isPassword"] = this.IsPassword;

			return brick;
		}


	}


}

