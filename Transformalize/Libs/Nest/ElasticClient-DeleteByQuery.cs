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
		public IDeleteResponse DeleteByQuery<T>(Func<DeleteByQueryDescriptor<T>, DeleteByQueryDescriptor<T>> deleteByQuerySelector) where T : class
		{
			return this.Dispatch<DeleteByQueryDescriptor<T>, DeleteByQueryRequestParameters, DeleteResponse>(
				deleteByQuerySelector,
				(p, d) => this.RawDispatch.DeleteByQueryDispatch<DeleteResponse>(p, d)
			);
		}

		/// <inheritdoc />
		public IDeleteResponse DeleteByQuery(IDeleteByQueryRequest deleteByQueryRequest) 
		{
			return this.Dispatch<IDeleteByQueryRequest, DeleteByQueryRequestParameters, DeleteResponse>(
				deleteByQueryRequest,
				(p, d) => this.RawDispatch.DeleteByQueryDispatch<DeleteResponse>(p, d)
			);
		}

		/// <inheritdoc />
		public Task<IDeleteResponse> DeleteByQueryAsync<T>(Func<DeleteByQueryDescriptor<T>, DeleteByQueryDescriptor<T>> deleteByQuerySelector) where T : class
		{
			return this.DispatchAsync<DeleteByQueryDescriptor<T>, DeleteByQueryRequestParameters, DeleteResponse, IDeleteResponse>(
				deleteByQuerySelector,
				(p, d) => this.RawDispatch.DeleteByQueryDispatchAsync<DeleteResponse>(p, d)
			);
		}

		/// <inheritdoc />
		public Task<IDeleteResponse> DeleteByQueryAsync(IDeleteByQueryRequest deleteByQueryRequest) 
		{
			return this.DispatchAsync<IDeleteByQueryRequest, DeleteByQueryRequestParameters, DeleteResponse, IDeleteResponse>(
				deleteByQueryRequest,
				(p, d) => this.RawDispatch.DeleteByQueryDispatchAsync<DeleteResponse>(p, d)
			);
		}

	}
}