using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class CheckBoxField
	{


		public string Label
		{
			get;
			set;
		}


		public string Name
		{
			get;
			set;
		}


		public string InputValue
		{
			get;
			set;
		}


		public bool Checked
		{
			get;
			set;
		}


		public Dictionary<string, object> ToDictionary()
		{
			var item = new Dictionary<string, object> ();

			item["boxLabel"] = this.Label;
			item["name"] = this.Name;
			item["inputValue"] = this.InputValue;
			item["checked"] = InvariantConverter.ToString (this.Checked);

			// We want to return "nothing" when nothing is checked (but we want to return something)
			item["uncheckedValue"] = "";

			return item;
		}


	}


}

