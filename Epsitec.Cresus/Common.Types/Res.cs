﻿//	Automatically generated by ResGenerator, on 11.10.2006 17:01:52
//	Do not edit manually.

namespace Epsitec.Common.Types
{
	public static class Res
	{
		public static class Types
		{
			public static class Default
			{
				public static readonly Epsitec.Common.Types.BooleanType Boolean = (Epsitec.Common.Types.BooleanType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 3)));
				public static readonly Epsitec.Common.Types.DecimalType Decimal = (Epsitec.Common.Types.DecimalType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 4)));
				public static readonly Epsitec.Common.Types.DoubleType Double = (Epsitec.Common.Types.DoubleType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 5)));
				public static readonly Epsitec.Common.Types.IntegerType Integer = (Epsitec.Common.Types.IntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 6)));
				public static readonly Epsitec.Common.Types.LongIntegerType LongInteger = (Epsitec.Common.Types.LongIntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 7)));
				public static readonly Epsitec.Common.Types.StringType String = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 8)));
				public static readonly Epsitec.Common.Types.VoidType Void = (Epsitec.Common.Types.VoidType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 9)));
			}
			
		}
		public static class Strings
		{
			public static string Empty { get { return Epsitec.Common.Types.Res.Strings.GetText (Epsitec.Common.Support.Druid.FromFieldId (0)); } }
			
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
			private static Epsitec.Common.Support.ResourceBundle _stringsBundle = _manager.GetBundle ("Strings");
			#endregion
		}
		
		public static void Initialize(System.Type type, string name)
		{
			_manager = new Epsitec.Common.Support.ResourceManager (type);
			_manager.DefineDefaultModuleName (name);
		}
		
		public static Epsitec.Common.Support.ResourceManager Manager
		{
			get { return _manager; }
		}
		
		public static int ModuleId
		{
			get { return _moduleId; }
		}
		
		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
		private static int _moduleId = 1;
	}
}
