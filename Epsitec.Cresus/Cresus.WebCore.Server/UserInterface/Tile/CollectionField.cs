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


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			// Here we don't set the readOnly property on the container directly but in the
			// individual check boxes.
			fieldDictionary.Remove ("readOnly");

			// Here we don't set the PanelFieldAccessorId on the container directly but in the
			// individual check boxes.

			fieldDictionary["xtype"] = "epsitec.checkboxes";
			fieldDictionary["defaultType"] = "checkboxfield";
			fieldDictionary["labelAlign"] = "left";
			fieldDictionary["items"] = this.CheckBoxFields.Select (c => c.ToDictionary (this.PanelFieldAccessorId, this.IsReadOnly)).ToList ();

			return fieldDictionary;
		}


		private readonly IList<CheckBoxField> checkBoxFields = new List<CheckBoxField> ();


	}


}

