using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervMainDataLoader
	{


		public static EervMainData LoadEervData(FileInfo inputFile)
		{
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (inputFile).ToList ();

			EervMainDataLoader.FreezeData (groupDefinitions);

			return new EervMainData (groupDefinitions);
		}


		internal static IEnumerable<EervGroupDefinition> LoadEervGroupDefinitions(FileInfo inputFile)
		{
			var records = EervDataReader.ReadGroupDefinitions (inputFile);

			var parents = new Stack<EervGroupDefinition> ();

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

					yield return groupDefinition;
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

			throw new FormatException ("Invalid group definition level");
		}


		private static EervGroupDefinition GetEervGroupDefinition(Dictionary<GroupDefinitionHeader, string> record, int groupLevel)
		{
			var id = record[GroupDefinitionHeader.Id];
			var name = record[EervMainDataLoader.names[groupLevel]];

			return new EervGroupDefinition (id, name);
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
