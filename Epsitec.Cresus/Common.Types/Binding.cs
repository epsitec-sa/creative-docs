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
			this.Path = path;
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
		public string							Path
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

		public void UpdateTargets(BindingUpdateMode mode)
		{
			WeakBindingExpression[] expressions = this.GetExpressions ();

			for (int i = 0; i < expressions.Length; i++)
			{
				BindingExpression item = expressions[i].Expression;

				if (item == null)
				{
					this.RemoveExpression (expressions[i]);
				}
				else
				{
					item.UpdateTarget (mode);
				}
			}
		}

		private WeakBindingExpression[] GetExpressions()
		{
			switch (this.count)
			{
				case BindingCount.None:
					return new WeakBindingExpression[0];
				case BindingCount.Single:
					return new WeakBindingExpression[1] { (WeakBindingExpression) this.expressions };
				case BindingCount.Several:
					return (WeakBindingExpression[]) this.expressions;
			}

			throw new System.InvalidOperationException ();
		}

		private void RemoveExpression(WeakBindingExpression expression)
		{
			WeakBindingExpression[] expressions;
			
			switch (this.count)
			{
				case BindingCount.None:
					break;
				
				case BindingCount.Single:
					this.expressions = null;
					this.count = BindingCount.None;
					break;
				
				case BindingCount.Several:
					expressions = (WeakBindingExpression[]) this.expressions;
					
					if (expressions.Length > 2)
					{
						WeakBindingExpression[] copy = new WeakBindingExpression[expressions.Length-1];

						for (int i = 0, j = 0; i < expressions.Length; i++)
						{
							if (expressions[i] != expression)
							{
								copy[j++] = expressions[i];
							}
						}
						
						this.expressions = copy;
					}
					else if (expressions.Length == 2)
					{
						if (expressions[0] == expression)
						{
							this.expressions = expressions[1];
							this.count = BindingCount.Single;
						}
						else if (expressions[1] == expression)
						{
							this.expressions = expressions[0];
							this.count = BindingCount.Single;
						}
						else
						{
							throw new System.InvalidOperationException ();
						}
					}
					else
					{
						throw new System.InvalidOperationException ();
					}
					
					break;
				
				default:
					throw new System.InvalidOperationException ();
			}
		}
		private void AddExpression(WeakBindingExpression expression)
		{
			WeakBindingExpression[] expressions;
			WeakBindingExpression[] copy;
			
			switch (this.count)
			{
				case BindingCount.None:
					this.expressions = expression;
					this.count = BindingCount.Single;
					break;
				
				case BindingCount.Single:
					this.expressions = new WeakBindingExpression[2] { (WeakBindingExpression) this.expressions, expression };
					this.count = BindingCount.Several;
					break;
				
				case BindingCount.Several:
					expressions = (WeakBindingExpression[]) this.expressions;
					copy = new WeakBindingExpression[expressions.Length+1];
					expressions.CopyTo (copy, 0);
					copy[expressions.Length] = expression;
					this.expressions = copy;
					break;

				default:
					throw new System.InvalidOperationException ();
			}
		}
		
		internal void Add(BindingExpression expression)
		{
			//	The binding expression is referenced through a weak binding
			//	by the binding itself, which allows for the expression to be
			//	garbage collected when its property dies.
			
			this.AddExpression (new WeakBindingExpression (expression));
		}
		internal void Remove(BindingExpression expression)
		{
			WeakBindingExpression[] expressions = this.GetExpressions ();
			
			for (int i = 0; i < expressions.Length; i++)
			{
				BindingExpression item = expressions[i].Expression;

				if ((item == null) ||
					(item == expression))
				{
					this.RemoveExpression (expressions[i]);
				}
			}
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
				
				foreach (WeakBindingExpression item in this.GetExpressions ())
				{
					BindingExpression expression = item.Expression;

					if (expression != null)
					{
						expression.DetachFromSource ();
					}
				}
			}
		}
		private void AttachAfterChanges()
		{
			if (this.state == State.SourceDetached)
			{
				this.state = State.SourceAttached;

				foreach (WeakBindingExpression item in this.GetExpressions ())
				{
					BindingExpression expression = item.Expression;

					if (expression != null)
					{
						expression.AttachToSource ();
						expression.UpdateTarget (BindingUpdateMode.Reset);
					}
				}
			}
		}

		#region Converter Class

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

		#endregion

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

		private enum State : byte
		{
			Invalid,
			
			SourceAttached,
			SourceDetached
		}

		#endregion

		private enum BindingCount : byte
		{
			None,
			Single,
			Several
		}

		#region Private WeakBindingExpression Class

		private class WeakBindingExpression : System.WeakReference
		{
			public WeakBindingExpression(BindingExpression expression) : base (expression)
			{
			}

			public BindingExpression Expression
			{
				get
				{
					return base.Target as BindingExpression;
				}
			}
		}

		#endregion

		public static readonly object			DoNothing = new object ();	//	setting a value of DoNothing in BindingExpression does nothing

		private BindingMode						mode;
		private object							source;
		private string							path;
		private string							elementName;
		private int								deferCounter;
		private State							state = State.SourceDetached;
		private BindingCount					count = BindingCount.None;
		private object							expressions;
	}
}
