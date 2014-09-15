﻿using System;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest
{
	public partial class ElasticClient
	{
		/// <inheritdoc />
		public IBulkResponse Bulk(IBulkRequest bulkRequest)
		{
			return this.Dispatch<IBulkRequest, BulkRequestParameters, BulkResponse>(
				bulkRequest,
				(p, d) =>
				{
					var json = Serializer.SerializeBulkDescriptor(d);
					return this.RawDispatch.BulkDispatch<BulkResponse>(p, json);
				}
			);
		}
		
		/// <inheritdoc />
		public IBulkResponse Bulk(Func<BulkDescriptor, BulkDescriptor> bulkSelector)
		{
			return this.Dispatch<BulkDescriptor, BulkRequestParameters, BulkResponse>(
				bulkSelector,
				(p, d) =>
				{
					var json = Serializer.SerializeBulkDescriptor(d);
					return this.RawDispatch.BulkDispatch<BulkResponse>(p, json);
				}
			);
		}

		/// <inheritdoc />
		public Task<IBulkResponse> BulkAsync(IBulkRequest bulkRequest)
		{
			return this.DispatchAsync<IBulkRequest, BulkRequestParameters, BulkResponse, IBulkResponse>(
				bulkRequest,
				(p, d) =>
				{
					var json = Serializer.SerializeBulkDescriptor(d);
					return this.RawDispatch.BulkDispatchAsync<BulkResponse>(p, json);
				}
			);
		}

		/// <inheritdoc />
		public Task<IBulkResponse> BulkAsync(Func<BulkDescriptor, BulkDescriptor> bulkSelector)
		{
			return this.DispatchAsync<BulkDescriptor, BulkRequestParameters, BulkResponse, IBulkResponse>(
				bulkSelector,
				(p, d) =>
				{
					var json = Serializer.SerializeBulkDescriptor(d);
					return this.RawDispatch.BulkDispatchAsync<BulkResponse>(p, json);
				}
			);
		}
	}
}