using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents a special field that is used to edit a value.
	/// </summary>
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
