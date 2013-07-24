using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// This class represents the action tiles, i.e. the layout that must be presented to the user
	/// when he decides to perform an action.
	/// </summary>
	internal sealed class ActionTile : AbstractEntityTile
	{


		public string Text
		{
			get;
			set;
		}


		public IList<AbstractField> Fields
		{
			get;
			set;
		}


		protected override string GetTileType()
		{
			return "action";
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var tile = base.ToDictionary ();

			tile["text"] = this.Text;
			tile["fields"] = this.Fields.Select (f => f.ToDictionary ()).ToList ();

			return tile;
		}


	}


}
