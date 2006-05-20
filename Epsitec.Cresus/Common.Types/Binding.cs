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
				return this.bindingMode;
			}
			set
			{
				if (this.bindingMode != value)
				{
					this.NotifyBeforeChange ();
					this.bindingMode = value;
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

		internal bool							Deferred
		{
			get
			{
				return this.deferCounter > 0;
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

		private WeakBindingExpression[] GetExpressions()
		{
			//	See RemoveExpression and AddExpression.
			
			WeakBindingExpression[] expressions;
			WeakBindingExpression[] copy;

			lock (this)
			{
				switch (this.boundExpressionsType)
				{
					case BoundExpressions.None:
						return new WeakBindingExpression[0];

					case BoundExpressions.Single:
						return new WeakBindingExpression[1] { (WeakBindingExpression) this.boundExpressions };

					case BoundExpressions.Several:
						expressions = (WeakBindingExpression []) this.boundExpressions;
						copy = new WeakBindingExpression[expressions.Length];
						expressions.CopyTo (copy, 0);
						return copy;
				}
			}
			
			throw new System.InvalidOperationException ();
		}

		private void RemoveExpression(WeakBindingExpression expression)
		{
			WeakBindingExpression[] expressions;

			lock (this)
			{
				switch (this.boundExpressionsType)
				{
					case BoundExpressions.None:
						break;

					case BoundExpressions.Single:
						this.boundExpressions = null;
						this.boundExpressionsType = BoundExpressions.None;
						break;

					case BoundExpressions.Several:
						expressions = (WeakBindingExpression[]) this.boundExpressions;

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

							this.boundExpressions = copy;
						}
						else if (expressions.Length == 2)
						{
							if (expressions[0] == expression)
							{
								this.boundExpressions = expressions[1];
								this.boundExpressionsType = BoundExpressions.Single;
							}
							else if (expressions[1] == expression)
							{
								this.boundExpressions = expressions[0];
								this.boundExpressionsType = BoundExpressions.Single;
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
		}
		
		private void AddExpression(WeakBindingExpression expression)
		{
			WeakBindingExpression[] expressions;
			WeakBindingExpression[] copy;

			lock (this)
			{
				switch (this.boundExpressionsType)
				{
					case BoundExpressions.None:
						this.boundExpressions = expression;
						this.boundExpressionsType = BoundExpressions.Single;
						break;

					case BoundExpressions.Single:
						this.boundExpressions = new WeakBindingExpression[2] { (WeakBindingExpression) this.boundExpressions, expression };
						this.boundExpressionsType = BoundExpressions.Several;
						break;

					case BoundExpressions.Several:
						expressions = (WeakBindingExpression[]) this.boundExpressions;
						copy = new WeakBindingExpression[expressions.Length+1];
						expressions.CopyTo (copy, 0);
						copy[expressions.Length] = expression;
						this.boundExpressions = copy;
						break;

					default:
						throw new System.InvalidOperationException ();
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
			if (this.sourceState == SourceState.Attached)
			{
				this.sourceState = SourceState.Detached;
				
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
			if (this.sourceState == SourceState.Detached)
			{
				this.sourceState = SourceState.Attached;

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

		#region Private SourceState Enumeration

		private enum SourceState : byte
		{
			Invalid,
			
			Attached,
			Detached
		}

		#endregion

		#region Private BoundExpressions Enumeration

		private enum BoundExpressions : byte
		{
			None=0,
			Single,
			Several
		}

		#endregion

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

		private BindingMode						bindingMode;
		private SourceState						sourceState = SourceState.Detached;
		
		private object							source;
		private string							path;
		private string							elementName;
		private int								deferCounter;
		
		private BoundExpressions				boundExpressionsType;
		private object							boundExpressions;
	}
}
