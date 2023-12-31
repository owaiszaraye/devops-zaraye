﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Domain.Orders;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Return request service interface
    /// </summary>
    public partial interface IReturnRequestService
    {
        /// <summary>
        /// Updates a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateReturnRequestAsync(ReturnRequest returnRequest);

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteReturnRequestAsync(ReturnRequest returnRequest);

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request
        /// </returns>
        Task<ReturnRequest> GetReturnRequestByIdAsync(int returnRequestId);

        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; 0 to load all entries</param>
        /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
        /// <param name="customNumber">Custom number; null or empty to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return requests
        /// </returns>
        Task<IPagedList<ReturnRequest>> SearchReturnRequestsAsync(int storeId = 0, int customerId = 0,
            int orderItemId = 0, string customNumber = "", ReturnRequestStatus? rs = null, DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Delete a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteReturnRequestReasonAsync(ReturnRequestReason returnRequestReason);

        /// <summary>
        /// Gets all return request reasons
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reasons
        /// </returns>
        Task<IList<ReturnRequestReason>> GetAllReturnRequestReasonsAsync();

        /// <summary>
        /// Gets a return request reason
        /// </summary>
        /// <param name="returnRequestReasonId">Return request reason identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the return request reason
        /// </returns>
        Task<ReturnRequestReason> GetReturnRequestReasonByIdAsync(int returnRequestReasonId);

        /// <summary>
        /// Inserts a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertReturnRequestReasonAsync(ReturnRequestReason returnRequestReason);

        /// <summary>
        /// Updates the  return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateReturnRequestReasonAsync(ReturnRequestReason returnRequestReason);

        Task InsertReturnRequestAsync(ReturnRequest returnRequest);
    }

}
