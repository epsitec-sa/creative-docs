//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[TypeConverter (typeof (Binding.Converter))]
	public class Binding
	{
		public Binding()
		{
		}
		public Binding(object source) : this (BindingMode.TwoWay, source)
		{
		}
		public Binding(object source, string path) : this (BindingMode.TwoWay, source, path)
		{
		}
		public Binding(BindingMode mode, object source)
		{
			this.Mode = mode;
			this.Source = source;
		}
		public Binding(BindingMode mode, object source, string path)
		{
			this.Mode = mode;
			this.Source = source;
			this.Path = new DependencyPropertyPath (path);
		}

		
		public BindingMode						Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				if (this.mode != value)
				{
					this.NotifyBeforeChange ();
					this.mode = value;
					this.NotifyAfterChange ();
				}
			}
		}
		public object							Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.NotifyBeforeChange ();
					this.source = value;
					this.NotifyAfterChange ();
				}
			}
		}
		public DependencyPropertyPath			Path
		{
			get
			{
				return this.path;
			}
			set
			{
				if (this.path != value)
				{
					this.NotifyBeforeChange ();
					this.path = value;
					this.NotifyAfterChange ();
				}
			}
		}
		public string							ElementName
		{
			get
			{
				return this.elementName;
			}
			set
			{
				if (this.elementName != value)
				{
					this.NotifyBeforeChange ();
					this.elementName = value;
					this.NotifyAfterChange ();
				}
			}
		}

		public System.IDisposable DeferChanges()
		{
			return new DeferManager (this);
		}

		public static bool IsBindingTarget(BindingMode mode)
		{
			switch (mode)
			{
				case BindingMode.OneTime:
				case BindingMode.OneWay:
				case BindingMode.TwoWay:
					return true;

				case BindingMode.OneWayToSource:
				case BindingMode.None:
					return false;

				default:
					throw new System.ArgumentOutOfRangeException (string.Format ("BindingMode.{0} not supported", mode));
			}
		}
		public static bool IsBindingSource(BindingMode mode)
		{
			switch (mode)
			{
				case BindingMode.None:
				case BindingMode.OneTime:
				case BindingMode.OneWay:
					return false;

				case BindingMode.OneWayToSource:
				case BindingMode.TwoWay:
					return true;

				default:
					throw new System.ArgumentOutOfRangeException (string.Format ("BindingMode.{0} not supported", mode));
			}
		}

		internal void Add(BindingExpression expression)
		{
			this.expressions.Add (expression);
		}
		internal void Remove(BindingExpression expression)
		{
			this.expressions.Remove (expression);
		}

		private void NotifyBeforeChange()
		{
			this.DetachBeforeChanges ();
		}
		private void NotifyAfterChange()
		{
			if (this.deferCounter == 0)
			{
				this.AttachAfterChanges ();
			}
		}

		private void DetachBeforeChanges()
		{
			if (this.state == State.SourceAttached)
			{
				this.state = State.SourceDetached;
				
				foreach (BindingExpression expression in this.expressions)
				{
					expression.DetachFromSource ();
				}
			}
		}
		private void AttachAfterChanges()
		{
			if (this.state == State.SourceDetached)
			{
				this.state = State.SourceAttached;

				foreach (BindingExpression expression in this.expressions)
				{
					expression.AttachToSource ();
					expression.UpdateTarget (BindingUpdateMode.Reset);
				}
			}
		}

		#region Private DeferManager Class
		private struct DeferManager : System.IDisposable
		{
			public DeferManager(Binding binding)
			{
				this.binding = binding;
				System.Threading.Interlocked.Increment (ref this.binding.deferCounter);
			}

			#region IDisposable Members
			public void Dispose()
			{
				if (System.Threading.Interlocked.Decrement (ref this.binding.deferCounter) == 0)
				{
					this.binding.AttachAfterChanges ();
				}
			}
			#endregion
			
			private Binding						binding;
		}
		#endregion

		#region Private State Enumeration
		private enum State
		{
			Invalid,
			
			SourceAttached,
			SourceDetached
		}
		#endregion

		public class Converter : ITypeConverter
		{
			#region ITypeConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				Binding binding = value as Binding;
				return Serialization.MarkupExtension.BindingToString (context, binding);
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				return Serialization.MarkupExtension.BindingFromString (context, value);
			}

			#endregion
		}
		
		public static readonly object			DoNothing = new object ();	//	setting a value of DoNothing in BindingExpression does nothing

		private BindingMode						mode;
		private object							source;
		private DependencyPropertyPath			path;
		private string							elementName;
		private int								deferCounter;
		private State							state = State.SourceDetached;
		private List<BindingExpression>			expressions = new List<BindingExpression> ();
	}
}
