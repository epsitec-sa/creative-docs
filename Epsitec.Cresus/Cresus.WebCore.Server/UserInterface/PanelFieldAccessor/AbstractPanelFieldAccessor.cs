using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor
{


	internal abstract class AbstractPanelFieldAccessor
	{


		public AbstractPanelFieldAccessor(LambdaExpression lambda, string id)
		{
			this.id = id;
			this.type = lambda.ReturnType;
			this.fieldType = FieldTypeSelector.GetFieldType (this.type);

			this.getter = lambda.Compile ();

			var setterLambda = ExpressionAnalyzer.CreateSetter (lambda);

			if (setterLambda != null)
			{
				this.setter = setterLambda.Compile ();
			}
		}


		public string Id
		{
			get
			{
				return this.id;
			}
		}


		public Type Type
		{
			get
			{
				return this.type;
			}
		}


		public FieldType FieldType
		{
			get
			{
				return this.fieldType;
			}
		}


		protected Delegate Getter
		{
			get
			{
				return this.getter;
			}
		}


		protected Delegate Setter
		{
			get
			{
				return this.setter;
			}
		}


		public static AbstractPanelFieldAccessor Create(LambdaExpression lambda, string id)
		{
			var type = lambda.ReturnType;

			if (type.IsEntity ())
			{
				return new EntityPanelFieldAccessor (lambda, id);
			}
			else if (type.IsGenericIListOfEntities ())
			{
				return new EntityListPanelFieldAccessor (lambda, id);
			}
			else
			{
				return new StringPanelFieldAccessor (lambda, id);
			}
		}


		private readonly string id;


		private readonly Type type;


		private readonly FieldType fieldType;


		private readonly Delegate getter;


		private readonly Delegate setter;

	
	}


}
