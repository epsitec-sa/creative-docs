//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	public class TabPageDef
	{
		public TabPageDef(string name, FormattedText text, System.Action action)
		{
			this.name = name;
			this.text = text;
			this.action = action;
			this.pageWidgets = new List<Widget> ();
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public FormattedText Text
		{
			get
			{
				return this.text;
			}
		}

		public IList<Widget> PageWidgets
		{
			get
			{
				return this.pageWidgets;
			}
		}

		public void ExecuteAction()
		{
			if (this.action != null)
			{
				this.action ();
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
		
		private readonly string name;
		private readonly FormattedText text;
		private readonly System.Action action;
		private readonly List<Widget> pageWidgets;
	}
	
	public class TabPageDef<T> : TabPageDef
	{
		public TabPageDef(T id, string name, FormattedText text, System.Action action)
			: base (name, text, action)
		{
			this.id = id;
		}

		public T Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly T id;
	}
}
