//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>ColumnRef</c> class is used to attach a value of type <typeparamref name="T"/> to
	/// a column, by using the ID of the column to reference it.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public class ColumnRef<T> : ColumnRef
		where T : IXmlNodeClass
	{
		public ColumnRef(string columnId, T value)
			: base (columnId)
		{
			this.value    = value;
		}

		
		public T								Value
		{
			get
			{
				return this.value;
			}
		}


		protected override XElement SaveValue(string xmlNodeName)
		{
			return ColumnRef<T>.saveFunc (this.value, xmlNodeName);
		}

		public static T RestoreValue(XElement xml)
		{
			return ColumnRef<T>.restoreFunc (xml);
		}

		static ColumnRef()
		{
			var restoreMethod = typeof (T).GetMethod ("Restore", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.InvokeMethod);
			var saveMethod    = typeof (T).GetMethod ("Save", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod);

			ColumnRef<T>.saveFunc = (that, xmlNodeName) => (XElement) saveMethod.Invoke (that, new object[] { xmlNodeName });
			ColumnRef<T>.restoreFunc = xml => (T) restoreMethod.Invoke (null, new object[] { xml });
		}

		private static readonly System.Func<T, string, XElement> saveFunc;
		private static readonly System.Func<XElement, T> restoreFunc;

		private readonly T value;
	}
}
