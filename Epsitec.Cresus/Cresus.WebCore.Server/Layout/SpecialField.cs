using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class SpecialField : AbstractField
	{


		public string EntityId
		{
			get;
			set;
		}


		public string ControllerName
		{
			get;
			set;
		}
		

		public string FieldName
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "specialField";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var field = base.ToDictionary ();

			field["entityId"] = this.EntityId;
			field["controllerName"] = this.ControllerName;
			field["fieldName"] = this.FieldName;

			return field;
		}


	}


}
