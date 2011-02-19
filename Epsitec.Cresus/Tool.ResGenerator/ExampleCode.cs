#if false

namespace Epsitec.App.DocumentEditor
{
	public sealed class Res
	{
		public static class Strings
		{
			// ...
		}


		//	Voici à quoi ressemble le code que tu vas devoir générer
		//	depuis le fichier Application.cs, dans la méthode statique
		//	nommée "GenerateCaptions" :
		
		public static class Captions
		{
			//	Admettons que l'on a des définitions de captions "Cap.Foo.Bar.Xyz", "Cap.Foo.Bar.Abc",
			//	"Cap.Zzz.Abc", il faut que le code génère la structure suivante, en codant à la place
			//	de 0, 1, 2 les numéros de DRUIDs définis par les champs respectifs.

			public static class Foo
			{
				public static class Bar
				{
					public static Epsitec.Common.Types.Caption Xyz { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 0))); } }
					public static Epsitec.Common.Types.Caption Abc { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 1))); } }
				}
				public static class Zzz
				{
					public static Epsitec.Common.Types.Caption Abc { get { return _manager.GetCaption (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 2))); } }
				}
			}
		}

		//	Voici à quoi ressemble le code que tu vas devoir générer
		//	depuis le fichier Application.cs, dans la méthode statique
		//	nommée "GenerateCommands" :

		public static class Commands
		{
			//	Supposons des commandes "Cmd.Foo.Xyz" et "Cmd.Foo.Bar.Abc"... Voici ce qu'il faut
			//	générer dans ce cas. L'accès se fait au moyen de propriétés "readonly" et pas
			//	d'accesseurs "get", afin de permettre de cacher les commandes.
			
			public static class Foo
			{
				//	Chaque classe (imbriquée ou non) doit contenir une méthode statique
				//	nommée _Initialize, avec un body vide :

				internal static void _Initialize() { }
				
				public static readonly Epsitec.Common.Widgets.Command Xyz = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 3)));
				
				public static class Bar
				{
					internal static void _Initialize() { }
					public static readonly Epsitec.Common.Widgets.Command Abc = Epsitec.Common.Widgets.Command.Get (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 4)));
				}
			}

			//	En fin de compte, quand toutes les classes qui encapsulent les noms
			//	des commandes ont été générées, il faut encore produire une méthode
			//	_Initialize globale qui appelle toutes les méthodes _Initialize des
			//	diverses classes et classes imbriquées :

			internal static void _Initialize()
			{
				Foo._Initialize ();
				Foo.Bar._Initialize ();
			}
		}

		
		//	Voici à quoi ressemble le code qui doit être généré pour les définitions
		//	de types :

		public static class Types
		{
			public static class Foo
			{
				public static readonly Epsitec.Common.Types.StringType Abc = (Epsitec.Common.Types.StringType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 5)));
				public static readonly Epsitec.Common.Types.IntegerType Xyz = (Epsitec.Common.Types.IntegerType) Epsitec.Common.Types.TypeRosetta.CreateTypeObject (Epsitec.Common.Support.Druid.FromLong (Epsitec.Common.Support.Druid.FromModuleDruid (_moduleId, 6)));
			}
		}

		// ...

		public static void Initialize(System.Type type, string name)
		{
			_manager = new Epsitec.Common.Support.ResourceManager (type);
			_manager.DefineDefaultModuleName (name);

			//	Compléter ici pour ajouter si besoin (c'est-à-dire si on a généré
			//	une classe "Commands") le code suivant :

			Commands._Initialize ();
		}

		// ...

		private static Epsitec.Common.Support.ResourceManager _manager = Epsitec.Common.Support.Resources.DefaultManager;
		private static int _moduleId = 1;
	}
}

#endif
