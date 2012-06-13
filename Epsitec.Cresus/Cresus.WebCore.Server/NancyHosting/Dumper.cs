using Epsitec.Common.Support.Extensions;

using System;

using System.Collections;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{


	internal sealed class Dumper
	{


		public Dumper(bool isEnabled)
		{
			this.IsEnabled = isEnabled;
		}


		public bool IsEnabled
		{
			get;
			set;
		}


		public void Dump(object element)
		{
			if (this.IsEnabled)
			{
				var dump = Dumper.DumpElement (element);

				System.Diagnostics.Debug.WriteLine (dump);
			}
		}


		private static string Dump(IDictionary dictionary, int level)
		{
			Func<int, DictionaryEntry, string> headerDumper = (i, e) => e.Key.ToString ();
			Func<int, DictionaryEntry, object> contentDumper = (i, e) => e.Value;

			return Dumper.DumpElements (level, "Dictionary", dictionary.AsEntries (), headerDumper, contentDumper);
		}


		private static string Dump(IList list, int level)
		{
			Func<int, object, string> headerDumper = (i, e) => i.ToString ();
			Func<int, object, object> contentDumper = (i, e) => e;

			return Dumper.DumpElements (level, "List", list, headerDumper, contentDumper);
		}


		private static string Dump(Array array, int level)
		{
			Func<int, object, string> headerDumper = (i, e) => i.ToString ();
			Func<int, object, object> contentDumper = (i, e) => e;

			return Dumper.DumpElements (level, "Array", array, headerDumper, contentDumper);
		}


		private static string DumpElements<T>(int level, string title, IEnumerable elements, Func<int, T, string> headerGetter, Func<int, T, object> contentGetter)
		{
			string s = title + "\n";

			var padding = Dumper.GetPadding (level);

			int i = 0;

			foreach (var element in elements.Cast<T> ())
			{
				var header = headerGetter (i, element);
				var content = Dumper.DumpElement (contentGetter (i, element), level);

				s += padding + header + ": " + content + "\n";

				i++;
			}
			return s;
		}


		private static string DumpElement(object element, int level = 0)
		{
			var newLevel = level + 1;

			if (element == null)
			{
				return "null";
			}
			else if (element is IDictionary)
			{
				return Dumper.Dump ((IDictionary) element, newLevel);
			}
			else if (element is IList)
			{
				return Dumper.Dump ((IList) element, newLevel);
			}
			else if (element is Array)
			{
				return Dumper.Dump ((Array) element, newLevel);
			}
			else
			{
				return element.ToString ();
			}
		}


		private static string GetPadding(int level)
		{
			string padding = "";

			for (int i = 0; i < level; i++)
			{
				padding += "\t";
			}

			return padding;
		}


		public static Dumper Instance
		{
			get
			{
				return Dumper.instance;
			}
		}


		private static readonly Dumper instance = new Dumper (true);


	}


}
