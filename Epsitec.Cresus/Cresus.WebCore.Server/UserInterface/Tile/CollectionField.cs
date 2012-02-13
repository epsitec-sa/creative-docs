using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class CollectionField : AbstractField
	{


		public IList<CheckBoxField> CheckBoxFields
		{
			get
			{
				return this.checkBoxFields;
			}
		}


		protected override Dictionary<string, object> GetFieldDictionary()
		{
			var fieldDictionary = base.GetFieldDictionary ();

			fieldDictionary["xtype"]  = "epsitec.checkboxes";
			fieldDictionary["defaultType"] = "checkboxfield";
			fieldDictionary["labelAlign"] = "left";
			fieldDictionary["items"] = this.CheckBoxFields.Select (c => c.ToDictionary ()).ToList ();

			return fieldDictionary;
		}


		private readonly IList<CheckBoxField> checkBoxFields = new List<CheckBoxField> ();


	}


}

