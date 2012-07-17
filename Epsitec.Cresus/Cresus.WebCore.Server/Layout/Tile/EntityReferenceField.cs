using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.Tile
{


	internal sealed class EntityReferenceField : AbstractField
	{


		public string Value
		{
			get;
			set;
		}


		public IList<Tuple<string, string>> PossibleValues
		{
			get
			{
				return this.possibleValues;
			}
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var fieldDictionary = base.ToDictionary ();

			fieldDictionary["value"] = this.Value;
			fieldDictionary["xtype"] = "epsitec.entity";
			fieldDictionary["store"] = this.PossibleValues.Select (t => new object[] { t.Item1, t.Item2 }).ToList ();

			return fieldDictionary;
		}


		private readonly IList<Tuple<string, string>> possibleValues = new List<Tuple<string, string>> ();


	}


}

