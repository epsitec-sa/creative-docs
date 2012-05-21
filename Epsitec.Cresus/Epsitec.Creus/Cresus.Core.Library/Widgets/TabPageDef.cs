//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TabPageDef</c> class defines a tab managed by the <see cref="TileTabBook"/>
	/// class. When the tab gets activated, the associated action will be executed.
	/// </summary>
	public abstract class TabPageDef
	{
		protected TabPageDef(string name, FormattedText text, System.Action selectAction)
		{
			this.name = name;
			this.text = text;
			this.selectAction = selectAction;
			this.pageWidgets = new List<Widget> ();
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public FormattedText					Text
		{
			get
			{
				return this.text;
			}
		}

		public IList<Widget>					PageWidgets
		{
			get
			{
				return this.pageWidgets;
			}
		}

		public void ExecuteSelectAction()
		{
			if (this.selectAction != null)
			{
				this.selectAction ();
			}
		}

		public void SetPageWidgetsVisibility(bool visibility)
		{
			this.pageWidgets.ForEach (widget => widget.Visibility = visibility);
		}


		public static TabPageDef<T> Create<T>(T id, FormattedText text, System.Action action = null)
		{
			return new TabPageDef<T> (id, id.ToString (), text, action);
		}

		public static TabPageDef<T> Create<T>(T id, FormattedText text, System.Action<T> action)
		{
			if (action == null)
			{
				return new TabPageDef<T> (id, id.ToString (), text, null);
			}
			else
			{
				return new TabPageDef<T> (id, id.ToString (), text, () => action (id));
			}
		}

		private readonly string					name;
		private readonly FormattedText			text;
		private readonly System.Action			selectAction;
		private readonly List<Widget>			pageWidgets;
	}
}
