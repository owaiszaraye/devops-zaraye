using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Forums;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial interface IMessageTokenProvider
    {
        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddStoreTokensAsync(IList<Token> tokens, Store store, EmailAccount emailAccount);

        /// <summary>
        /// Add order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order"></param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
       // Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId);

        /// <summary>
        /// Add refunded order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order">Order</param>
        /// <param name="refundedAmount">Refunded amount of order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddOrderRefundedTokensAsync(IList<Token> tokens, Order order, decimal refundedAmount);

        /// <summary>
        /// Add shipment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="shipment">Shipment item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddShipmentTokensAsync(IList<Token> tokens, Shipment shipment, int languageId);

        /// <summary>
        /// Add order note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="orderNote">Order note</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddOrderNoteTokensAsync(IList<Token> tokens, OrderNote orderNote);

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
       // Task AddRecurringPaymentTokensAsync(IList<Token> tokens, RecurringPayment recurringPayment);

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddCustomerTokensAsync(IList<Token> tokens, int customerId);

        /// <summary>
        /// Add customer tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddCustomerTokensAsync(IList<Token> tokens, Customer customer);

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription);

        /// <summary>
        /// Add product review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="productReview">Product review</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddProductReviewTokensAsync(IList<Token> tokens, ProductReview productReview);

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment);

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment);
        
        /// <summary>
        /// Add product tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="product">Product</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddProductTokensAsync(IList<Token> tokens, Product product, int languageId);

        /// <summary>
        /// Add product attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">Product attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddAttributeCombinationTokensAsync(IList<Token> tokens, ProductAttributeCombination combination, int languageId);

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddForumTokensAsync(IList<Token> tokens, Forum forum);

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost);

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage);

        /// <summary>
        /// Add tokens of BackInStock subscription
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">BackInStock subscription</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task AddBackInStockTokensAsync(IList<Token> tokens, BackInStockSubscription subscription);

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed (supported) message tokens for campaigns
        /// </returns>
        Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync();

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the collection of allowed message tokens
        /// </returns>
        Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null);

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);

        #region CustomMehtod

        Task AddOnlineLeadTokensAsync(IList<Token> tokens, OnlineLead onlineLead);
        Task AddFeedbackTokensAsync(IList<Token> tokens, int customerId, int feedbackId);


        Task AddFeedbackTokensAsync(IList<Token> tokens, Customer customer, AppFeedBack appFeedBack);

        Task AddBuyerTokensAsync(IList<Token> tokens, Customer buyer);

        Task AddSupplierTokensAsync(IList<Token> tokens, int supplierId);

        Task AddSupplierTokensAsync(IList<Token> tokens, Customer supplier);

        Task AddOnlineLeadRequestTokensAsync(IList<Token> tokens, Customer buyer, Request Request);


        #endregion

    }
}