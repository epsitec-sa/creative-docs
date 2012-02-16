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

			// Here we don't set the readOnly property on the field directly but in the individual
			// checkboxes.
			fieldDictionary.Remove ("readOnly");

			fieldDictionary["xtype"]  = "epsitec.checkboxes";
			fieldDictionary["defaultType"] = "checkboxfield";
			fieldDictionary["labelAlign"] = "left";
			fieldDictionary["items"] = this.CheckBoxFields.Select (c => c.ToDictionary (this.IsReadOnly)).ToList ();

			return fieldDictionary;
		}


		private readonly IList<CheckBoxField> checkBoxFields = new List<CheckBoxField> ();


	}


}

