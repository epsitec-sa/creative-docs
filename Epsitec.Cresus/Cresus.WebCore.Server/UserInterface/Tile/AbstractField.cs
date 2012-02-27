using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal abstract class AbstractField : AbstractEditionTilePart
	{


		public string Title
		{
			get;
			set;
		}


		public string PanelFieldAccessorId
		{
			get;
			set;
		}


		public bool IsReadOnly
		{
			get;
			set;
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();
			
			fieldDictionary["fieldLabel"] = this.Title;
			fieldDictionary["name"] = this.PanelFieldAccessorId;
			fieldDictionary["readOnly"] = this.IsReadOnly;

			return fieldDictionary;
		}


	}


}
