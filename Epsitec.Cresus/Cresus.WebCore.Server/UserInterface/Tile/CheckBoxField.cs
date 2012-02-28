using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class CheckBoxField
	{


		public string Label
		{
			get;
			set;
		}


		public string InputValue
		{
			get;
			set;
		}


		public int Index
		{
			get;
			set;
		}


		public bool Checked
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary(string propertyAccessorId, bool isReadOnly)
		{
			var item = new Dictionary<string, object> ();

			item["boxLabel"] = this.Label;
			item["name"] = FormCollectionEmbedder.GetFieldName (propertyAccessorId, this.Index);
			item["inputValue"] = this.InputValue;
			item["checked"] = this.Checked;
			item["readOnly"] = isReadOnly;

			// We want to return "nothing" when nothing is checked, but we want to return something.
			item["uncheckedValue"] = "";

			return item;
		}


	}


}

