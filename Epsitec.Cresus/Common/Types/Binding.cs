//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.Common.Types
{
	[SerializationConverter (typeof (Binding.SerializationConverter))]
	public class Binding : AbstractBinding
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
			this.Mode   = mode;
			this.Source = source;
		}
		public Binding(BindingMode mode, string path)
		{
			this.Mode = mode;
			this.Path = path;
		}
		public Binding(BindingMode mode, object source, string path)
		{
			this.Mode   = mode;
			this.Source = source;
			this.Path   = path;
		}


		/// <summary>
		/// Gets or sets the binding mode.
		/// </summary>
		/// <value>The binding mode.</value>
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

		/// <summary>
		/// Gets or sets the data source root.
		/// </summary>
		/// <value>The data source root.</value>
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
		
		/// <summary>
		/// Gets or sets the data source path.
		/// </summary>
		/// <value>The data source path, relative to the root.</value>
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

		/// <summary>
		/// Gets or sets a value indicating whether the binding should get and
		/// set values asynchronously.
		/// </summary>
		/// <value><c>true</c> if the binding should get and set values
		/// asynchronously; otherwise, <c>false</c>.</value>
		public bool								IsAsync
		{
			get
			{
				return this.isAsync;
			}
			set
			{
				this.isAsync = value;
			}
		}

		internal bool							IsAttached
		{
			get
			{
				return this.sourceState == SourceState.Attached;
			}
		}
#if false
		/// <summary>
		/// Gets or sets the name of the data source element.
		/// </summary>
		/// <value>The name of the data source element.</value>
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
#endif

		/// <summary>
		/// Gets or sets the converter for this binding.
		/// </summary>
		/// <value>The value converter.</value>
		public IValueConverter					Converter
		{
			get
			{
				return this.converter;
			}
			set
			{
				this.converter = value;
			}
		}

		/// <summary>
		/// Gets or sets the culture used by the binding converter.
		/// </summary>
		/// <value>The culture to use for conversions.</value>
		public System.Globalization.CultureInfo ConverterCulture
		{
			get
			{
				return this.converterCulture;
			}
			set
			{
				this.converterCulture = value;
			}
		}

		/// <summary>
		/// Gets or sets the optional converter parameter.
		/// </summary>
		/// <value>The optional converter parameter.</value>
		public object							ConverterParameter
		{
			get
			{
				return this.converterParameter;
			}
			set
			{
				this.converterParameter = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this binding has a converter.
		/// </summary>
		/// <value><c>true</c> if this binding has a converter; otherwise,
		/// <c>false</c>.</value>
		public bool								HasConverter
		{
			get
			{
				return this.converter != null;
			}
		}

		/// <summary>
		/// Determines whether the binding mode affects the target of a binding.
		/// </summary>
		/// <param name="mode">The binding mode.</param>
		/// <returns><c>true</c> if the binding mode affest the target of a binding;
		/// otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Determines whether the binding mode affects the source of a binding.
		/// </summary>
		/// <param name="mode">The binding mode.</param>
		/// <returns><c>true</c> if the binding mode affest the source of a binding;
		/// otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Finds the <c>ICollectionView</c> for a given object; the object must
		/// be a collection for this to work.
		/// </summary>
		/// <param name="collection">The probable collection object.</param>
		/// <param name="dataContext">The data context.</param>
		/// <returns>An <c>ICollectionView</c> which represents the collection.</returns>
		public static ICollectionView FindCollectionView(object collection, Binding dataContext)
		{
			if (dataContext == null)
			{
				return null;
			}

			if (Internal.CollectionViewResolver.IsCollectionViewCompatible (collection))
			{
				//	OK, there is a data context which is used as the source binding
				//	and the collection implements IList (the collection views are
				//	associated with the data context).

				return Internal.CollectionViewResolver.Default.GetCollectionView (dataContext, collection);
			}

			return null;
		}
		
		/// <summary>
		/// Updates the targets on which this binding has been attached.
		/// </summary>
		/// <param name="mode">The binding update mode.</param>
		public void UpdateTargets(BindingUpdateMode mode)
		{
			foreach (BindingExpression item in this.GetExpressions ())
			{
				item.UpdateTarget (mode);
			}
		}

		/// <summary>
		/// Evaluates the binding and returns the value of the source object.
		/// </summary>
		/// <param name="binding">The binding to evaluate.</param>
		/// <returns>The source object or an instance of <c>UndefinedValue</c>.</returns>
		public static object Evaluate(Binding binding)
		{
			if (binding == null)
			{
				return UndefinedValue.Value;
			}
			
			switch (binding.Mode)
			{
				case BindingMode.OneTime:
				case BindingMode.OneWay:
				case BindingMode.TwoWay:
					break;
				
				default:
					return UndefinedValue.Value;
			}

			Internal.BindingHelper helper = new Internal.BindingHelper ();

			helper.SetBinding (Internal.BindingHelper.ValueProperty, binding);
			helper.ClearBinding (Internal.BindingHelper.ValueProperty);

			return helper.GetValue (Internal.BindingHelper.ValueProperty);
		}
		
		internal object ConvertValue(object value, System.Type type)
		{
			if (type != null)
			{
				if ((value == null) ||
					(value.GetType () != type))
				{
					IValueConverter converter = this.converter ?? Converters.AutomaticValueConverter.Instance;
					CultureInfo     culture   = this.converterCulture ?? CultureInfo.CurrentCulture;
					object          parameter = this.converter == null ? null : this.converterParameter;
					
					value = converter.Convert (value, type, parameter, culture);
				}
			}
			
			return value;
		}

		internal object ConvertBackValue(object value, System.Type type)
		{
			if (Binding.IsRealValue (value))
			{
				if (type != null)
				{
					if ((value == null) ||
						(value.GetType () != type))
					{
						IValueConverter converter = this.converter ?? Converters.AutomaticValueConverter.Instance;
						CultureInfo     culture   = this.converterCulture ?? CultureInfo.CurrentCulture;
						object          parameter = this.converter == null ? null : this.converterParameter;

						value = converter.ConvertBack (value, type, parameter, culture);
					}
				}
			}

			return value;
		}

		protected override IEnumerable<BindingExpression> GetExpressions()
		{
			//	See RemoveExpression and AddExpression.

			Weak<BindingExpression> item;
			BindingExpression expression;
			Weak<BindingExpression>[] items;
			List<BindingExpression> expressions;
			bool cleanUp = false;
			
			lock (this)
			{
				switch (this.boundExpressionsType)
				{
					case BoundExpressions.None:
						expression  = null;
						expressions = null;
						break;

					case BoundExpressions.Single:
						item = (Weak<BindingExpression>) this.boundExpressions;
						
						expression  = item.Target;
						expressions = null;
						
						if (expression == null)
						{
							this.boundExpressionsType = BoundExpressions.None;
							this.boundExpressions = null;
							break;
						}

						break;

					case BoundExpressions.Several:
						items = (Weak<BindingExpression> []) this.boundExpressions;
						expressions = new List<BindingExpression> ();
						expression = null;

						for (int i = 0; i < items.Length; i++)
						{
							expression = items[i].Target;

							if (expression == null)
							{
								cleanUp = true;
							}
							else
							{
								expressions.Add (expression);
							}
						}

						if (expressions.Count == 0)
						{
							this.boundExpressionsType = BoundExpressions.None;
							this.boundExpressions = null;

							expression  = null;
							expressions = null;
							
							break;
						}
						else if (expressions.Count == 1)
						{
							expression  = expressions[0];
							expressions = null;
							
							this.boundExpressionsType = BoundExpressions.Single;
							this.boundExpressions = new Weak<BindingExpression> (expression);
						}
						else if (cleanUp)
						{
							items = new Weak<BindingExpression>[expressions.Count];

							for (int i = 0; i < expressions.Count; i++)
							{
								items[i] = new Weak<BindingExpression> (expressions[i]);
							}
							
							this.boundExpressionsType = BoundExpressions.Several;
							this.boundExpressions = items;
						}
						break;
					
					default:
						throw new System.InvalidOperationException ();
				}
			}

			if (expressions == null)
			{
				if (expression == null)
				{
					return new BindingExpression[0];
				}
				else
				{
					return new BindingExpression[1] { expression };
				}
			}
			else
			{
				return expressions;
			}
		}

		protected override void RemoveExpression(BindingExpression expression)
		{
			Weak<BindingExpression>[] expressions;

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
						expressions = (Weak<BindingExpression>[]) this.boundExpressions;

						if (expressions.Length > 2)
						{
							Weak<BindingExpression>[] copy = new Weak<BindingExpression>[expressions.Length-1];

							for (int i = 0, j = 0; i < expressions.Length; i++)
							{
								if (expressions[i].Target != expression)
								{
									copy[j++] = expressions[i];
								}
							}

							this.boundExpressions = copy;
						}
						else if (expressions.Length == 2)
						{
							if (expressions[0].Target == expression)
							{
								this.boundExpressions = expressions[1];
								this.boundExpressionsType = BoundExpressions.Single;
							}
							else if (expressions[1].Target == expression)
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

		protected override void AddExpression(BindingExpression expression)
		{
			Weak<BindingExpression>[] expressions;
			Weak<BindingExpression>[] copy;

			lock (this)
			{
				switch (this.boundExpressionsType)
				{
					case BoundExpressions.None:
						this.boundExpressions = new Weak<BindingExpression> (expression);
						this.boundExpressionsType = BoundExpressions.Single;
						break;

					case BoundExpressions.Single:
						this.boundExpressions = new Weak<BindingExpression>[2] { (Weak<BindingExpression>) this.boundExpressions, new Weak<BindingExpression> (expression) };
						this.boundExpressionsType = BoundExpressions.Several;
						break;

					case BoundExpressions.Several:
						expressions = (Weak<BindingExpression>[]) this.boundExpressions;
						copy = new Weak<BindingExpression>[expressions.Length+1];
						expressions.CopyTo (copy, 0);
						copy[expressions.Length] = new Weak<BindingExpression> (expression);
						this.boundExpressions = copy;
						break;

					default:
						throw new System.InvalidOperationException ();
				}
			}
		}

		protected override void DetachBeforeChanges()
		{
			if (this.sourceState == SourceState.Attached)
			{
				this.sourceState = SourceState.Detached;

				foreach (BindingExpression expression in this.GetExpressions ())
				{
					expression.DetachFromSource ();
				}
			}
		}
		
		protected override void AttachAfterChanges()
		{
			if (this.sourceState == SourceState.Detached)
			{
				this.AttachExpressionsToSource (Collection.ToArray (this.GetExpressions ()));
			}
			else if (this.sourceState == SourceState.AsyncAttaching)
			{
				System.Diagnostics.Debug.Fail ("Attaching while previous asynchronous attach is still running");
			}
		}

		internal static bool IsRealValue(object value)
		{
			if ((value != Binding.DoNothing) &&
				(!InvalidValue.IsInvalidValue (value)) &&
				(!UndefinedValue.IsUndefinedValue (value)) &&
				(!PendingValue.IsPendingValue (value)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		internal void AttachExpressionsToSource(params BindingExpression[] expressions)
		{
			if (this.IsAsync)
			{
				//	The binding is defined to be asynchronous : start a separate
				//	operation, running concurrently, which will walk the source
				//	tree and update the targets :

				this.sourceState = SourceState.AsyncAttaching;

				BindingAsyncOperation asyncOperation = new BindingAsyncOperation (this, expressions);

				asyncOperation.AttachToSourceAndUpdateTargets ();
			}
			else
			{
				this.sourceState = SourceState.Attached;

				foreach (BindingExpression expression in expressions)
				{
					expression.AttachToSource ();
					expression.UpdateTarget (BindingUpdateMode.Reset);
				}
			}
		}

		internal void NotifyAttachCompleted()
		{
			if (this.sourceState == SourceState.AsyncAttaching)
			{
				this.sourceState = SourceState.Attached;
			}
		}

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

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

		#region Private SourceState Enumeration

		private enum SourceState : byte
		{
			Invalid,
			
			Attached,
			Detached,
			
			AsyncAttaching
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

		public static readonly object			DoNothing = new object ();	//	setting a value of DoNothing in BindingExpression does nothing

		private BindingMode						bindingMode;
		private SourceState						sourceState = SourceState.Detached;
		private bool							isAsync;
		
		private object							source;
		private string							path;
#if false
		private string							elementName;
#endif
		
		private BoundExpressions				boundExpressionsType;
		private object							boundExpressions;
		
		private IValueConverter					converter;
		private CultureInfo						converterCulture;
		private object							converterParameter;
	}
}
