//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// La classe Editor donne accès à l'éditeur de scripts via l'interface IEditorEngine.
	/// Le chargement de l'assembly "Developer" se fait à la demande.
	/// </summary>
	public class Editor
	{
		private Editor()
		{
		}
		
		
		public static IEditorEngine				Engine
		{
			get
			{
				if (Editor.engine == null)
				{
					Editor.LoadEditorEngine ();
				}
				
				return Editor.engine;
			}
			set
			{
				Editor.engine = value;
			}
		}
		
		
		private static void LoadEditorEngine()
		{
			//	On connaît l'identité de la classe qui nous intéresse, mais comme on ne peut pas avoir
			//	de références circulaires (et que le moteur d'édition n'est chargé qu'à la demande), on
			//	utilise une création dynamique :
			
			object engine = System.Activator.CreateInstance ("Common.Script.Developer", "Epsitec.Common.Script.Developer.EditorEngine").Unwrap ();
			Editor.engine = engine as IEditorEngine;
		}
		
		
		private static IEditorEngine			engine;
	}
}
