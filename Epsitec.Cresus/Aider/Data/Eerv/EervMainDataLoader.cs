//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

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
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (groupDefinitionFile).ToList ();

			EervMainDataLoader.MoveParishGroupDefinition (groupDefinitions);
			EervMainDataLoader.FreezeData (groupDefinitions);

			return new EervMainData (groupDefinitions);
		}


		internal static IEnumerable<EervGroupDefinition> LoadEervGroupDefinitions(FileInfo inputFile)
		{
			var records = EervDataReader.ReadGroupDefinitions (inputFile);

			var parents = new Stack<EervGroupDefinition> ();
			var functions = new Dictionary<int, EervGroupDefinition> ();

			parents.Push (null);

			foreach (var record in records)
			{
				var level = EervMainDataLoader.GetEervGroupDefinitionLevel (record);

				while (level <= parents.Count - 2)
				{
					parents.Pop ();
				}

				var groupDefinition = EervMainDataLoader.GetEervGroupDefinition (record, level);
				
				if (!EervMainDataLoader.DiscardGroupDefinition (groupDefinition))
				{
					EervMainDataLoader.Process (parents, functions, record, groupDefinition);
					yield return groupDefinition;
				}

				parents.Push (groupDefinition);
			}
		}


		private static void Process(Stack<EervGroupDefinition> parents, Dictionary<int, EervGroupDefinition> functions, Dictionary<GroupDefinitionHeader, string> record, EervGroupDefinition groupDefinition)
		{
			var parent = parents.Peek ();

			if (parent != null && !EervMainDataLoader.DiscardGroupDefinition (parent))
			{
				parent.Children.Add (groupDefinition);
				groupDefinition.Parent = parent;
				groupDefinition.GroupLevel = parent.GroupLevel + 1;
			}
			else
			{
				groupDefinition.GroupLevel = 0;
			}

			if (groupDefinition.IsFunctionDefinition ())
			{
				var functionId = int.Parse (groupDefinition.Id.Substring (4, 2));
				functions[functionId] = groupDefinition;
			}
			else
			{
				var functionCode = record[GroupDefinitionHeader.Function];

				if (functionCode != null)
				{
					var functionId = int.Parse (functionCode);
					groupDefinition.FunctionGroup = functions[functionId];
				}
			}
		}


		private static bool DiscardGroupDefinition(EervGroupDefinition groupDefinition)
		{
			// This is not a real group definition, so we discard it and it is as it did not exist
			// at all in the file.

			return groupDefinition.Id == "0100000000";
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
			var isLeaf = record[GroupDefinitionHeader.IsLeaf] == "x";

			return new EervGroupDefinition (id, name, isLeaf);
		}


		private static void MoveParishGroupDefinition(List<EervGroupDefinition> groupDefinitions)
		{
			// We move the parish group definition within the region group definition because this
			// group definition is really a child of the region group definition, even if this is
			// not the case in the excel file.

			var regionGroupDefinition = groupDefinitions
				.Where (g => g.GroupLevel == 0)
				.Where (g => g.GroupClassification == GroupClassification.Region)
				.Single ();

			var parishGroupDefinition = groupDefinitions
				.Where (g => g.GroupLevel == 0)
				.Where (g => g.GroupClassification == GroupClassification.Parish)
				.Single ();

			regionGroupDefinition.Children.Add (parishGroupDefinition);
			parishGroupDefinition.Parent = regionGroupDefinition;

			EervMainDataLoader.ChangeGroupLevel (parishGroupDefinition, 1);
		}

		private static void ChangeGroupLevel(EervGroupDefinition groupDefinition, int delta)
		{
			groupDefinition.GroupLevel += delta;

			foreach (var child in groupDefinition.Children)
			{
				EervMainDataLoader.ChangeGroupLevel (child, delta);
			}
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


	}


}
