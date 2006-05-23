//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>IPaintFilter</c> interface is used to skip widgets completely,
	/// including their children, or just skip the widgets but process their
	/// children, when executing the <c>Widget.PaintHandler</c> method.
	/// </summary>
	public interface IPaintFilter
	{
		/// <summary>
		/// Determines whether the specified widget is fully discarded. A fully
		/// discarded widget won't be processed at all by <c>PaintHandler</c>;
		/// its children won't be processed either.
		/// </summary>
		/// <param name="widget">The widget to check.</param>
		/// <returns><c>true</c> if the specified widget fully discarded;
		/// otherwise, <c>false</c>.</returns>
		bool IsWidgetFullyDiscarded(Widget widget);

		/// <summary>
		/// Determines whether the specified widget should not be painted. Its
		/// children will get a chance to be painted.
		/// </summary>
		/// <param name="widget">The widget to check.</param>
		/// <returns><c>true</c> if the specified widget should not be painted;
		/// otherwise, <c>false</c>.</returns>
		bool IsWidgetPaintDiscarded(Widget widget);

		/// <summary>
		/// This gets called by <c>PaintHandler</c> before a widget's children
		/// get processed.
		/// </summary>
		void NotifyAboutToProcessChildren();

		/// <summary>
		/// This gets called by <c>PaintHandler</c> after a widget's children
		/// have all been processed.
		/// </summary>
		void NotifyChildrenProcessed();
	}
}
