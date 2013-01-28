using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class RelatedDataLoader
	{


		public RelatedDataLoader(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}


		public void LoadRelatedData(IEnumerable<AbstractEntity> entities, IEnumerable<LambdaExpression> expressions)
		{
			// We make the requests by small batches because we dont want to make a giant IN clause
			// with thousands of entity ids.
			var batchSize = 100;

			var entityList = entities.ToList ();

			// The chains are the representations of the properties that we want to load, like
			// Contact->Person
			// Contact->Person->Sex
			// Contact->Address
			// Contact->Address->Town
			// We sort them by size so that we compute the result of shorter chains before longer
			// chains. This way we can use the result of say Contact->Address to compute the result
			// of Contact->Address->Town.
			// We need to store the type with the field, because we cannot retreive this information
			// via the StructuredTypeField. We should be able, but we cannot because the defining
			// type id is always empty. I don't know if this is a bug or something and I don't have
			// the time to investigate that now.
			var chains = this.GetPropertyChains (expressions)
				.OrderBy (c => c.Length)
				.ToList ();

			for (int i = 0; i < entityList.Count; i += batchSize)
			{
				var batch = entityList.Skip (i).Take (batchSize);

				this.LoadData (batch, chains);
			}
		}


		private void LoadData(IEnumerable<AbstractEntity> entities, IEnumerable<Tuple<Type, StructuredTypeField>[]> chains)
		{
			var comparer = ArrayEqualityComparer<Tuple<Type, StructuredTypeField>>.Instance;
			var results = new Dictionary<Tuple<Type, StructuredTypeField>[], List<Constant>> (comparer);

			results[new Tuple<Type, StructuredTypeField>[0]] = this.GetIds (entities).ToList ();

			foreach (var chain in chains)
			{
				var subChain = chain.Take (chain.Length - 1).ToArray ();
				
				List<Constant> subChainIds;

				if (results.TryGetValue (subChain, out subChainIds) && subChainIds.Count > 0)
				{
					results[chain] = this.LoadData (chain.Last (), subChainIds);
				}
			}
		}


		private IEnumerable<Tuple<Type, StructuredTypeField>[]> GetPropertyChains(IEnumerable<LambdaExpression> expressions)
		{
			return expressions
				.SelectMany (e => this.GetPropertyChains (e))
				.Distinct (ArrayEqualityComparer<Tuple<Type, StructuredTypeField>>.Instance);
		}

		private IEnumerable<Tuple<Type, StructuredTypeField>[]> GetPropertyChains(LambdaExpression expression)
		{
			var chain = this.GetPropertyChain (expression);

			for (int i = 0; i < chain.Count; i++)
			{
				yield return chain.Take (i + 1).ToArray ();
			}
		}

		private List<Tuple<Type, StructuredTypeField>> GetPropertyChain(LambdaExpression expression)
		{
			var fields = ExpressionAnalyzer
				.ExplodeLambda (expression)
				.Select (p => Tuple.Create (p.DeclaringType, EntityInfo.GetStructuredTypeField (p)))
				.ToList ();

			if (fields.Count > 0 && fields.Last ().Item2.Relation != FieldRelation.Reference)
			{
				fields.RemoveAt (fields.Count - 1);
			}

			for (int i = 0; i < fields.Count; i++)
			{
				if (fields[i].Item2.Options.HasFlag (FieldOptions.Virtual))
				{
					fields.RemoveRange (i, fields.Count - i);

					break;
				}
			}

			return fields;
		}


		private List<Constant> LoadData(Tuple<Type, StructuredTypeField> fieldData, List<Constant> entityIds)
		{
			var fieldFefiningType = fieldData.Item1;
			var field = fieldData.Item2;

			var requestedEntityType = EntityInfo.GetType (field.Type.CaptionId);
			var requestedEntity = (AbstractEntity) Activator.CreateInstance (requestedEntityType);

			var rootEntityType = fieldFefiningType;
			var rootEntity = (AbstractEntity) Activator.CreateInstance (rootEntityType);
			rootEntity.SetField (field.CaptionId.ToResourceId (), requestedEntity);

			var request = new Request ()
			{
				RootEntity = rootEntity,
				RequestedEntity = requestedEntity,
				Distinct = true,
			};

			request.Conditions.Add
			(
				new ValueSetComparison
				(
					InternalField.CreateId (rootEntity),
					SetComparator.In,
					entityIds
				)
			);
			
			var result = this.dataContext.GetByRequest (request);

			return this.GetIds (result).ToList ();
		}


		private IEnumerable<Constant> GetIds(IEnumerable<AbstractEntity> entities)
		{
			return from entity in entities
				   where entity != null
				   let key = dataContext.GetNormalizedEntityKey (entity)
				   where key.HasValue
				   let value = key.Value.RowKey.Id.Value
				   select new Constant (value);
		}


		private readonly DataContext dataContext;


	}


}
