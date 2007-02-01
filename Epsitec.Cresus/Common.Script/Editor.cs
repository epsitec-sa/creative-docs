//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// La classe Editor donne acc�s � l'�diteur de scripts via l'interface IEditorEngine.
	/// Le chargement de l'assembly "Developer" se fait � la demande.
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
			//	On conna�t l'identit� de la classe qui nous int�resse, mais comme on ne peut pas avoir
			//	de r�f�rences circulaires (et que le moteur d'�dition n'est charg� qu'� la demande), on
			//	utilise une cr�ation dynamique :
			
			object engine = System.Activator.CreateInstance ("Common.Script.Developer", "Epsitec.Common.Script.Developer.EditorEngine").Unwrap ();
			Editor.engine = engine as IEditorEngine;
		}
		
		
		private static IEditorEngine			engine;
	}
}
