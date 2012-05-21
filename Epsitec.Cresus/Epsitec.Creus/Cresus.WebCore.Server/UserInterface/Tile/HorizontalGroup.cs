using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal class HorizontalGroup : AbstractEditionTilePart
	{


		public string Title
		{
			get;
			set;
		}


		public IList<AbstractField> Fields
		{
			get
			{
				return this.fields;
			}
		}


		public override IEnumerable<Dictionary<string, object>> ToDictionaries()
		{
			var item = new Dictionary<string, object> ();

			item["xtype"] = "fieldset";
			item["layout"] = "column";
			item["title"] = this.Title;

			var fieldItems = this.Fields.Select (f => f.ToDictionary ()).ToList ();
			var fieldWidth = 1.0 / fieldItems.Count;
				
			foreach (var fieldItem in fieldItems)
			{
				fieldItem["columnWidth"] = fieldWidth;
				fieldItem["margin"] = "0 5 0 0";
				fieldItem.Remove ("fieldLabel");
			}

			item["items"] = fieldItems;

			yield return item;
		}


		private readonly IList<AbstractField> fields = new List<AbstractField> ();


	}


}
