using System;

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

			EervMainDataLoader.FreezeData (groupDefinitions);

			return new EervMainData(groupDefinitions);
		}


		internal static IEnumerable<EervGroupDefinition> LoadEervGroupDefinitions(FileInfo inputFile)
		{
			// Skip the 4 first lines of the file as they are titles.
			var records = RecordHelper.GetRecords (inputFile, 45).Skip (4);

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

				var parent = parents.Peek ();

				if (parent != null)
				{
					parent.Children.Add (groupDefinition);
					groupDefinition.Parent = parent;
				}

				if (!EervMainDataLoader.IsGroupDefinitionToDiscard (groupDefinition))
				{
					parents.Push (groupDefinition);

					yield return groupDefinition;
				}
			}
		}


		private static int GetEervGroupDefinitionLevel(ReadOnlyCollection<string> record)
		{
			for (int i = 0; i < GroupDefinitionIndex.Names.Count; i++)
			{
				var name = RecordHelper.GetString (record, GroupDefinitionIndex.Names[i]);

				if (!string.IsNullOrEmpty (name))
				{
					return i;
				}
			}

			throw new FormatException ("Invalid group definition level");
		}


		private static EervGroupDefinition GetEervGroupDefinition(ReadOnlyCollection<string> record, int groupLevel)
		{
			var id = RecordHelper.GetString (record, GroupDefinitionIndex.Id);
			var name = RecordHelper.GetString (record, GroupDefinitionIndex.Names[groupLevel]);

			return new EervGroupDefinition (id, name);
		}


		private static bool IsGroupDefinitionToDiscard(EervGroupDefinition groupDefinition)
		{
			return groupDefinition.Name.Contains ("n1, n2, n3");
		}


		private static void FreezeData(List<EervGroupDefinition> groupDefinitions)
		{
			foreach (var groupDefinition in groupDefinitions)
			{
				groupDefinition.Freeze ();
			}
		}


		private static class GroupDefinitionIndex
		{


			public static readonly int Id = 2;
			public static readonly int NameLevel1 = 4;
			public static readonly int NameLevel2 = 6;
			public static readonly int NameLevel3 = 8;
			public static readonly int NameLevel4 = 10;
			public static readonly int NameLevel5 = 12;


			public static readonly ReadOnlyCollection<int> Names = new ReadOnlyCollection<int>
			(
				new List<int> ()
				{
					GroupDefinitionIndex.NameLevel1,
					GroupDefinitionIndex.NameLevel2,
					GroupDefinitionIndex.NameLevel3,
					GroupDefinitionIndex.NameLevel4,
					GroupDefinitionIndex.NameLevel5,
				}
			);


		}


	}


}
