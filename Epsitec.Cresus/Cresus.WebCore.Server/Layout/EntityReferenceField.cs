using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class EntityReferenceField : AbstractField
	{


		public string TypeName
		{
			get;
			set;
		}


		public EntityValue Value
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "entityReferenceField";
		}


		protected override object GetValue()
		{
			return this.Value.ToDictionary ();
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["entityName"] = this.TypeName;

			return brick;
		}


	}


}