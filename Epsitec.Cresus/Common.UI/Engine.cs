//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA, 27/04/2004

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe Engine...
	/// </summary>
	public class Engine
	{
		public Engine()
		{
		}
		
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
			
			if (source == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; invalid path found in binding information ({0}).", path), "binding");
			}
			
			Types.INamedType type = source.DataType;
			
			if (type == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot bind widget; path ({0}) points to typeless data.", path), "binding");
			}
			
			if (type is Types.IString)
			{
				Types.IDataConstraint   constraint = source.DataConstraint;
				Binders.DataValueBinder binder     = new Binders.DataValueBinder (source);
				Adapters.StringAdapter  adapter    = new Adapters.StringAdapter (binder);
				
				new Controllers.WidgetTextController (adapter, widget, constraint);
				
				return;
			}
			
			if (type is Types.INum)
			{
				Types.IDataConstraint   constraint = source.DataConstraint;
				Types.INum              num_type   = source.DataType as Types.INum;
				Binders.DataValueBinder binder     = new Binders.DataValueBinder (source);
				Adapters.DecimalAdapter adapter    = new Adapters.DecimalAdapter (binder);
				
				new Controllers.WidgetValueController (adapter, widget, constraint, num_type);
				
				return;
			}
			
			if (type is Types.IEnum)
			{
				//	TODO: attache le widget � cette �num�ration
			}
			
			throw new System.ArgumentException (string.Format ("Cannot bind widget; path ({0}) points to data of unknown type ({1}).", path, type.Name), "binding");
		}
	}
}
