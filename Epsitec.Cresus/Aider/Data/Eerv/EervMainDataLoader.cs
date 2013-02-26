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
			var groupDefinitions = EervMainDataLoader.LoadEervGroupDefinitions (groupDefinitionFile)
				.Where (g => g.GroupClassification != GroupClassification.None)
				.ToList ();

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

				var parent = parents.Peek ();

				if (parent == null)
				{
					groupDefinition.Parent = null;
				}
				else
				{
					parent.Children.Add (groupDefinition);
					groupDefinition.Parent = parent;
				}

				parents.Push (groupDefinition);

				EervMainDataLoader.HandleGroupDefinitionFunctions (functions, record, groupDefinition);

				yield return groupDefinition;
			}
		}


		private static void HandleGroupDefinitionFunctions(Dictionary<int, EervGroupDefinition> functions, Dictionary<GroupDefinitionHeader, string> record, EervGroupDefinition groupDefinition)
		{
			if (EervMainDataLoader.IsFunctionDefinition (groupDefinition))
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


		private static bool IsFunctionDefinition(EervGroupDefinition groupDefinition)
		{
			return groupDefinition.GroupClassification == GroupClassification.Function
				&& groupDefinition.GroupLevel == 2;
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

			return new EervGroupDefinition (id, name, isLeaf, groupLevel);
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
