using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	internal sealed class EntityCollectionField : AbstractField
	{


		public string TypeName
		{
			get;
			set;
		}


		public IList<EntityValue> Values
		{
			get;
			set;
		}


		protected override string GetEditionTilePartType()
		{
			return "entityCollectionField";
		}


		protected override object GetValue()
		{
			return this.Values.Select (v => v.ToDictionary ()).ToList ();
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["entityName"] = this.TypeName;

			return brick;
		}


	}


}

