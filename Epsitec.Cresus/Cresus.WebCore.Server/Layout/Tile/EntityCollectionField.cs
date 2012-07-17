using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityCollectionField : AbstractField
	{


		public IList<EntityCollectionCheckBoxField> CheckBoxFields
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

			// Here we don't set the PropertyAccessorId on the container directly but in the
			// individual check boxes.

			fieldDictionary["xtype"] = "epsitec.entitycollectionfield";
			fieldDictionary["defaultType"] = "checkboxfield";
			fieldDictionary["labelAlign"] = "left";
			fieldDictionary["items"] = this.CheckBoxFields.Select (c => c.ToDictionary (this.PropertyAccessorId, this.IsReadOnly)).ToList ();

			return fieldDictionary;
		}


		private readonly IList<EntityCollectionCheckBoxField> checkBoxFields = new List<EntityCollectionCheckBoxField> ();


	}


}

