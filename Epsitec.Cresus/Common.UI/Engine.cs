//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe Engine permet de lier une interface graphique avec une structure
	/// de donn�es.
	/// </summary>
	public class Engine
	{
		private Engine()
		{
		}
		
		static Engine()
		{
			Epsitec.Common.Widgets.Widget.Initialise ();
			Epsitec.Common.Support.ObjectBundler.RegisterAssembly (typeof (Engine).Assembly);
		}
		
		
		public static void Initialise()
		{
			//	En appelant cette m�thode statique, on peut garantir que le constructeur
			//	statique de Engine a bien �t� ex�cut�.
		}
		
		
		public static void BindWidgets(Types.IDataGraph graph, Common.Widgets.Widget root)
		{
			//	Attache tous les widgets, � partir de la racine.
			
			WidgetBinder binder = new WidgetBinder (graph);
			
			//	WalkChildren r�alise une descente dans tous les enfants, mais il ne faut pas
			//	oublier de traiter aussi la racine :
			
			binder.Process (root);
			
			root.WalkChildren (new Common.Widgets.WalkWidgetCallback (binder.Process));
		}
		
		
		#region WidgetBinder Class
		private class WidgetBinder
		{
			public WidgetBinder(Types.IDataGraph graph)
			{
				this.graph = graph;
			}
			
			
			public bool Process(Common.Widgets.Widget widget)
			{
				string binding = widget.BindingInfo;
				
				if (binding != null)
				{
					Engine.BindWidget (this.graph, widget, binding);
				}
				
				return true;
			}
			
			
			private Types.IDataGraph			graph;
		}
		#endregion
		
		public static void BindWidget(Types.IDataGraph graph, Common.Widgets.Widget widget, string binding)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.Load (new System.IO.StringReader ("<xml>" + binding + "</xml>"));
			
			Engine.BindWidget (graph, widget, doc.DocumentElement.FirstChild);
		}
		
		public static void BindWidget(Types.IDataGraph graph, Common.Widgets.Widget widget, System.Xml.XmlNode binding)
		{
			//	L'information de binding permet de r�aliser le lien entre un champ contenu
			//	dans IDataGraph et un widget. La syntaxe doit �tre du style :
			//
			//	  <bind path="chemin"/>
			//
			//	o� :
			//
			//	- path d�finit le chemin d'acc�s � l'objet IDataValue dans IDataGraph.
			
			System.Xml.XmlAttribute x_path = binding.Attributes["path"];
			
			if (x_path == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; no path found in binding information ({0}).", binding.OuterXml), "binding");
			}
			
			string           path   = x_path.Value;
			Types.IDataValue source = graph.Navigate (path) as Types.IDataValue;
			
			//	NB: pour l'instant, on ne peut pas r�aliser de binding sur un IDataFolder, mais uniquement
			//	    sur un IDataValue. C'est contestable, et peut-�tre faudra-t-il un jour modifier cela.
			
			if (source == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; invalid path found in binding information ({0}).", path), "binding");
			}
			
			Types.INamedType type = source.DataType;
			
			if (type == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; path ({0}) points to typeless data.", path), "binding");
			}
			
			if (Engine.BindWidget(source, widget) == false)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; path ({0}) points to data of unknown type ({1}).", path, type.Name), "binding");
			}
		}
		
		
		public static bool BindWidget(Types.IDataValue source, Common.Widgets.Widget widget)
		{
			Widgets.ISelfBindingWidget self_binding_widget = widget as Widgets.ISelfBindingWidget;
			
			if (self_binding_widget != null)
			{
				return self_binding_widget.BindWidget (source);
			}
			
			Types.IDataConstraint   constraint = source.DataConstraint;
			Binders.DataValueBinder binder     = new Binders.DataValueBinder (source);
			
			Types.IString str_type  = source.DataType as Types.IString;
			Types.INum    num_type  = source.DataType as Types.INum;
			Types.IEnum   enum_type = source.DataType as Types.IEnum;
			
			if (str_type != null)
			{
				Adapters.StringAdapter adapter = new Adapters.StringAdapter (binder);
				
				new Controllers.WidgetTextController (adapter, widget, constraint);
				
				return true;
			}
			
			if (num_type != null)
			{
				Adapters.DecimalAdapter adapter = new Adapters.DecimalAdapter (binder);
				
				//	Si le widget supporte l'interface INumValue, utilise la propri�t� Value
				//	du widget pour �changer des donn�es; sinon, part du principe que c'est
				//	l'�tat du widget qui doit �tre exploit� (sous la forme d'un bool�en
				//	ou d'un index dans le cas d'un groupe de boutons radio) :
				
				if (widget is Support.Data.INumValue)
				{
					new Controllers.WidgetValueController (adapter, widget, constraint, num_type);
				}
				else
				{
					new Controllers.WidgetStateController (adapter, widget, source.Caption, num_type);
				}
				
				return true;
			}
			
			if ((enum_type != null) &&
				(enum_type.IsCustomizable))
			{
				//	Le type est une �num�ration acceptant des valeurs en dehors de la
				//	liste des valeurs d�finies. Il faut donc utiliser une interface de
				//	type 'texte' :
				
				Adapters.StringAdapter adapter = new Adapters.StringAdapter (binder);
				
				if (constraint == null)
				{
					constraint = enum_type as Types.IDataConstraint;
				}
				
				if (widget is Support.Data.INamedStringSelection)
				{
					new Controllers.WidgetNamedSelController (adapter, widget, constraint, enum_type);
				}
				else
				{
					new Controllers.WidgetTextController (adapter, widget, constraint);
				}
				
				return true;
			}
			
			if (enum_type != null)
			{
				Adapters.DecimalAdapter adapter = new Adapters.DecimalAdapter (binder);
				
				if ((widget is Common.Widgets.CheckButton) &&
					(widget.Index != 0))
				{
					//	C'est un cas particulier: un bouton de type "check box" permet de d�finir
					//	l'�tat d'un bit d'une �num�ration repr�sentant un bitset. Dans un tel cas,
					//	l'index du bouton d�termine la valeur du bit.
					
					new Controllers.WidgetFlagController (adapter, widget, null, enum_type);
				}
				else if (widget is Support.Data.INamedStringSelection)
				{
					new Controllers.WidgetNamedSelController (adapter, widget, null, enum_type);
				}
				else
				{
					new Controllers.WidgetStateController (adapter, widget, source.Caption, enum_type);
				}
				
				return true;
			}
			
			return false;
		}
		
		public static bool BindWidget(Types.IDataFolder folder, Common.Widgets.Widget widget)
		{
			Binders.DataFolderBinder   binder  = new Binders.DataFolderBinder (folder);
			Adapters.DataFolderAdapter adapter = new Adapters.DataFolderAdapter (binder);
			
			new Controllers.WidgetDataFolderController (adapter, widget);
			
			return true;
		}
		
		
		public static Data.Representation FindDefaultRepresentation(Types.IDataValue source)
		{
			if (source == null)
			{
				//	On ne peut pas d�duire de repr�sentation si aucune source de donn�es
				//	n'a �t� sp�cifi�e :
					
				return Data.Representation.None;
			}
				
			Types.INamedType source_type = source.DataType;
				
			if (source_type is Types.IString)
			{
				return Data.Representation.TextField;
			}
			if (source_type is Types.INum)
			{
				return Data.Representation.NumericUpDown;
			}
				
			Types.IEnum enum_type = source_type as Types.IEnum;
				
			if (enum_type != null)
			{
				if (enum_type.IsCustomizable)
				{
					return Data.Representation.ComboEditableList;
				}
				else
				{
					return Data.Representation.ComboConstantList;
				}
			}
				
			return Data.Representation.None;
		}
		
		
		public static string MakeBindingDefinition(string path)
		{
			//	Produit le code XML qui permet de d�finir un "binding" avec une donn�e
			//	correspondant au chemin sp�cifi�. C'est ce code XML qui sera consomm�
			//	par les m�thodes BindWidget(...)
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (@"<bind path=""");
			buffer.Append (System.Utilities.TextToXml (path));
			buffer.Append (@"""/>");
			
			return buffer.ToString ();
		}
	}
}
