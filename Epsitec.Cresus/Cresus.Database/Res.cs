﻿//	Automatically generated by ResGenerator, on 03.04.2007
//	Do not edit manually.

namespace Epsitec.Cresus.Database
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			public static class Num
			{
				public static readonly Epsitec.Common.Types.LongIntegerType KeyId = (Epsitec.Common.Types.LongIntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 4));
				public static readonly Epsitec.Common.Types.IntegerType KeyStatus = (Epsitec.Common.Types.IntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 5));
				public static readonly Epsitec.Common.Types.LongIntegerType NullableKeyId = (Epsitec.Common.Types.LongIntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 7));
				public static readonly Epsitec.Common.Types.IntegerType ReqExecState = (Epsitec.Common.Types.IntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 6));
			}
			
			public static class Other
			{
				public static readonly Epsitec.Common.Types.DateTimeType DateTime = (Epsitec.Common.Types.DateTimeType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 13));
				public static readonly Epsitec.Common.Types.BinaryType ReqData = (Epsitec.Common.Types.BinaryType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 12));
			}
			
			public static class Str
			{
				public static readonly Epsitec.Common.Types.StringType InfoXml = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 9));
				public static readonly Epsitec.Common.Types.StringType Name = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 8));
				public static class Dict
				{
					public static readonly Epsitec.Common.Types.StringType Key = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 10));
					public static readonly Epsitec.Common.Types.StringType Value = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 11));
				}
			}
		}
		
		//	Code mapping for 'String' resources
		
		public static class Strings
		{
			public static string New { get { return Epsitec.Cresus.Database.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (0)); } }
			
			public static string GetString(params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			
			#region Internal Support Code
			private static string GetText(string bundle, params string[] path)
			{
				string field = string.Join (".", path);
				return _stringsBundle[field].AsString;
			}
			private static string GetText(Epsitec.Common.Support.Druid druid)
			{
				return _stringsBundle[druid].AsString;
			}
			private static Epsitec.Common.Support.ResourceBundle _stringsBundle = Res._manager.GetBundle ("Strings");
			#endregion
		}
		
		static Res()
		{
			Res.Initialize (typeof (Res), "Cresus.Database");
		}

		public static void Initialize()
		{
		}

		private static void Initialize(System.Type type, string name)
		{
			Res._manager = new Epsitec.Common.Support.ResourceManager (type);
			Res._manager.DefineDefaultModuleName (name);
		}
		
		public static Epsitec.Common.Support.ResourceManager Manager
		{
			get { return Res._manager; }
		}
		
		public static int ModuleId
		{
			get { return _moduleId; }
		}
		
		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
		private const int _moduleId = 20;
	}
}
