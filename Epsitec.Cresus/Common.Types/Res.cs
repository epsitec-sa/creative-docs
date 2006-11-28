﻿//	Automatically generated by ResGenerator, on 28.11.2006
//	Do not edit manually.

namespace Epsitec.Common.Types
{
	public static class Res
	{
		//	Code mapping for 'Caption' resources
		
		public static class Types
		{
			public static readonly Epsitec.Common.Types.EnumType BindingMode = (Epsitec.Common.Types.EnumType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 16));
			public static class Default
			{
				public static readonly Epsitec.Common.Types.BooleanType Boolean = (Epsitec.Common.Types.BooleanType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 3));
				public static readonly Epsitec.Common.Types.DateType Date = (Epsitec.Common.Types.DateType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 17));
				public static readonly Epsitec.Common.Types.DateTimeType DateTime = (Epsitec.Common.Types.DateTimeType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 19));
				public static readonly Epsitec.Common.Types.DecimalType Decimal = (Epsitec.Common.Types.DecimalType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 4));
				public static readonly Epsitec.Common.Types.DoubleType Double = (Epsitec.Common.Types.DoubleType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 5));
				public static readonly Epsitec.Common.Types.IntegerType Integer = (Epsitec.Common.Types.IntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 6));
				public static readonly Epsitec.Common.Types.LongIntegerType LongInteger = (Epsitec.Common.Types.LongIntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 7));
				public static readonly Epsitec.Common.Types.StringType String = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 8));
				public static readonly Epsitec.Common.Types.TimeType Time = (Epsitec.Common.Types.TimeType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 18));
				public static readonly Epsitec.Common.Types.VoidType Void = (Epsitec.Common.Types.VoidType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (_moduleId, 9));
			}
		}
		
		public static class Values
		{
			public static class BindingMode
			{
				public static Epsitec.Common.Types.Caption None { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 11)); } }
				public static Epsitec.Common.Types.Caption OneTime { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 12)); } }
				public static Epsitec.Common.Types.Caption OneWay { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 13)); } }
				public static Epsitec.Common.Types.Caption OneWayToSource { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 14)); } }
				public static Epsitec.Common.Types.Caption TwoWay { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (_moduleId, 15)); } }
			}
		}
		
		//	Code mapping for 'String' resources
		
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
