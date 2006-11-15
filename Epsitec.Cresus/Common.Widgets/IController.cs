//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>IController</c> interface is used by the <see cref="T:Placeholder"/>
	/// class to communicate with its controller.
	/// </summary>
	public interface IController
	{
		/// <summary>
		/// Creates the user interface. This populates the <c>Placeholder</c>
		/// with one or several widgets used to represent/edit its value.
		/// </summary>
		void CreateUserInterface();

		/// <summary>
		/// Disposes the user interface.
		/// </summary>
		void DisposeUserInterface();

		/// <summary>
		/// Refreshes the user interface. This method is called whenever the value
		/// represented in the <c>Placeholder</c> changes.
		/// </summary>
		void RefreshUserInterface(object oldValue, object newValue);

		/// <summary>
		/// Gets an object implementing interface <see cref="T:Layouts.IGridPermeable"/>
		/// for this controller.
		/// </summary>
		/// <returns>Object implementing interface <see cref="T:Layouts.IGridPermeable"/>
		/// for this controller, or <c>null</c>.</returns>
		Widgets.Layouts.IGridPermeable GetGridPermeableLayoutHelper();
		
		/// <summary>
		/// Gets or sets the placeholder associated with this controller.
		/// </summary>
		/// <value>The placeholder.</value>
		Placeholder Placeholder
		{
			get;
			set;
		}
	}
}
