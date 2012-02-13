using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.Tile
{


	internal sealed class EntityField : AbstractField
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


		protected override Dictionary<string, object> GetFieldDictionary()
		{
			var fieldDictionary = base.GetFieldDictionary ();

			fieldDictionary["value"] = this.Value;
			fieldDictionary["xtype"] = "epsitec.entity";
			fieldDictionary["store"] = this.PossibleValues.Select (t => new object[] { t.Item1, t.Item2 }).ToList ();

			return fieldDictionary;
		}


		private readonly IList<Tuple<string, string>> possibleValues = new List<Tuple<string, string>> ();


	}


}

