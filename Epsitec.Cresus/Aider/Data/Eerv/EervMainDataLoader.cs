//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervMainDataLoader
	{


		public static EervMainData LoadEervData(FileInfo groupDefinitionFile)
		{
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (groupDefinitionFile)
				.Where (g => g.GroupClassification != Enumerations.GroupClassification.None)
				.ToList ();

			EervMainDataLoader.FreezeData (groupDefinitions);

			return new EervMainData (groupDefinitions);
		}


		internal static IEnumerable<EervGroupDefinition> LoadEervGroupDefinitions(FileInfo inputFile)
		{
			var records = EervDataReader.ReadGroupDefinitions (inputFile);

			var parents = new Stack<EervGroupDefinition> ();
			var functions = new Dictionary<string, EervGroupDefinition> ();

			parents.Push (null);

			foreach (var record in records)
			{
				var level = EervMainDataLoader.GetEervGroupDefinitionLevel (record);

				while (level <= parents.Count - 2)
				{
					parents.Pop ();
				}

				var groupDefinition = EervMainDataLoader.GetEervGroupDefinition (record, level);

				if (!EervMainDataLoader.IsGroupDefinitionToDiscard (groupDefinition))
				{
					var parent = parents.Peek ();

					if (parent != null)
					{
						parent.Children.Add (groupDefinition);
						groupDefinition.Parent = parent;
					}

					parents.Push (groupDefinition);

					EervMainDataLoader.HandleGroupDefinitionFunctions (functions, record, groupDefinition);

					yield return groupDefinition;
				}
			}
		}


		private static void HandleGroupDefinitionFunctions(Dictionary<string, EervGroupDefinition> functions, Dictionary<GroupDefinitionHeader, string> record, EervGroupDefinition groupDefinition)
		{
			string id = groupDefinition.Id;

			if (id.StartsWith ("0101"))
			{
				//	1.1.x describe not real groups, but transversal functions, which can be applied
				//	to any standard non-leaf group:

				if (id.EndsWith ("0000"))
				{
					var functionCode = id.Substring (4, 2);
					functions[functionCode] = groupDefinition;
				}
				else
				{
					groupDefinition.GroupType = Enumerations.GroupType.Leaf;
				}
			}
			else if ((id.StartsWith ("02")) ||	//	"Synodal"
				/**/ (id.StartsWith ("03")) ||	//	"Régional"
				/**/ (id.StartsWith ("04")) ||	//	"Paroissial"
				/**/ (id.StartsWith ("05")) ||	//	"Missions communes"
				/**/ (id.StartsWith ("06")))	//	"Relations extérieures"
			{
				var functionCode = record[GroupDefinitionHeader.Function];

				if (functionCode != null)
				{
					functionCode = ("00" + functionCode).SubstringEnd (2);
					groupDefinition.FunctionGroup = functions[functionCode];
				}
			}
		}

		private static int GetEervGroupDefinitionLevel(Dictionary<GroupDefinitionHeader, string> record)
		{
			for (int i = 0; i < EervMainDataLoader.names.Count; i++)
			{
				var name = record[EervMainDataLoader.names[i]];

				if (!string.IsNullOrEmpty (name))
				{
					return i;
				}
			}

			throw new System.FormatException ("Invalid group definition level");
		}


		private static EervGroupDefinition GetEervGroupDefinition(Dictionary<GroupDefinitionHeader, string> record, int groupLevel)
		{
			var id = record[GroupDefinitionHeader.Id];
			var name = record[EervMainDataLoader.names[groupLevel]];
			var type = string.IsNullOrEmpty (record[GroupDefinitionHeader.IsLeaf]) ? Enumerations.GroupType.Node : Enumerations.GroupType.Leaf;

			return new EervGroupDefinition (id, name, type, groupLevel);
		}


		private static bool IsGroupDefinitionToDiscard(EervGroupDefinition groupDefinition)
		{
			var name = groupDefinition.Name;

			return name.Contains ("n1")
				|| name.Contains ("n2")
				|| name.Contains ("n3")
				|| namesToDiscard.Contains (name);
		}


		private static void FreezeData(List<EervGroupDefinition> groupDefinitions)
		{
			foreach (var groupDefinition in groupDefinitions)
			{
				groupDefinition.Freeze ();
			}
		}


		private static readonly ReadOnlyCollection<GroupDefinitionHeader> names = new ReadOnlyCollection<GroupDefinitionHeader>
		(
			new List<GroupDefinitionHeader> ()
			{
				GroupDefinitionHeader.NameLevel1,
				GroupDefinitionHeader.NameLevel2,
				GroupDefinitionHeader.NameLevel3,
				GroupDefinitionHeader.NameLevel4,
				GroupDefinitionHeader.NameLevel5,
			}
		);


		private static readonly HashSet<string> namesToDiscard = new HashSet<string> ()
		{
			"Projets 1",
			"Projet 2",
			"Mandat 1, 2, 3",
			"Groupe pilotage 1",
			"Groupe de projets 1",
			"Groupe pilotage 2",
			"Groupe de projets 2",
			"Groupe pilotage 1, 2, 3",
			"Mandataires 1, 2, 3",
			"Archives 1, Archives 2, Archives 3…",
		};


	}


}
