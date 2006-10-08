using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Debug
{
	public static class Tracker
	{
		public static string Register(object obj)
		{
			string old = Tracker.Identify (obj);
			if (old != null) return old;
			Item item = new Item (string.Format ("{0}", Tracker.id++), obj);
			Tracker.items.Add (item);
			return item.Name;
		}
		
		public static string Register(string name, object obj)
		{
			string old = Tracker.Identify (obj);
			if (old != null) return old;
			Item item = new Item (string.Format ("{0}:{1}", Tracker.id++, name), obj);
			Tracker.items.Add (item);
			return item.Name;
		}

		public static string Identify(object obj)
		{
			foreach (Item item in Tracker.items)
			{
				if (System.Object.ReferenceEquals (item.Object, obj))
				{
					return item.Name;
				}
			}

			return null;
		}

		struct Item
		{
			public Item(string name, object obj)
			{
				this.name = name;
				this.weak = new WeakReference (obj);
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public object Object
			{
				get
				{
					return this.weak.Target;
				}
			}

			private string name;
			private System.WeakReference weak;
		}

		static private List<Item> items = new List<Item> ();
		static private int id = 0;
	}
}
