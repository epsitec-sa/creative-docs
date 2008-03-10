using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.UI.MetaButton))]

namespace Epsitec.Common.Widgets
{
	[Types.DesignerVisible]
	public enum ButtonAspect
	{
		[Types.Hidden] None,
		DialogButton,	// bouton textuel pour dialogue (typiquement: "D'accord", "Annuler", etc.)
		IconButton,		// bouton automatique pour ruban, palette, etc.
	}

	public enum ButtonDisplayMode
	{
		TextOnly,		// texte seul
		Automatic,		// icône et/ou texte selon la taille disponible
	}

	public enum ButtonMarkDisposition
	{
		None,			// pas de marque
		Below,			// marque au dessous
		Above,			// marque au dessus
		Left,			// marque à gauche
		Right,			// marque à droite
	}
}
