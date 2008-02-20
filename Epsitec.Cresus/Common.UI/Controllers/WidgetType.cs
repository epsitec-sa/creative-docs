//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// The <c>WidgetType</c> enumeration is used to categorize the widgets used
	/// in a <see cref="Placeholder"/>.
	/// </summary>
	public enum WidgetType
	{
		/// <summary>
		/// This is a simple label.
		/// </summary>
		Label,
		
		/// <summary>
		/// The is an input element; the user can interact with it.
		/// </summary>
		Input,
		
		/// <summary>
		/// This is a read only content element. It provides the user with
		/// information, but cannot it does not offer any interaction.
		/// </summary>
		Content,
		
		/// <summary>
		/// This is a command element, usually a button.
		/// </summary>
		Command
	}
}
