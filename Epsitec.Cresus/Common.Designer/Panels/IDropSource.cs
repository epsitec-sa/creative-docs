//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// L'interface IDropSource est utile pour savoir quel widget a été
	/// déposé à la suite d'une opération de drag & drop.
	/// </summary>
	public interface IDropSource
	{
		Epsitec.Common.Widgets.Widget	DroppedWidget		{ get; }
	}
}
