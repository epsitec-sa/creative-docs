//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Widgets
{
	/// <summary>
	/// L'interface ISelfBindingWidget permet d'indiquer qu'un widget sait
	/// réaliser le "binding" soi-même, sans l'aide de UI.Engine.
	/// </summary>
	public interface ISelfBindingWidget
	{
		bool BindWidget(Types.IDataValue source);
	}
}
