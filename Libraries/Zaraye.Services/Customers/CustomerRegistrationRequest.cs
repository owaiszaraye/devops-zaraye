using Zaraye.Core.Domain.Customers;

namespace Zaraye.Services.Customers
{
    /// <summary>
    /// Customer registration request
    /// </summary>
    public partial class CustomerRegistrationRequest
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="email">Email</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="passwordFormat">Password format</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="isApproved">Is approved</param>
        public CustomerRegistrationRequest(Customer customer, string email, string username,
            string password,
            PasswordFormat passwordFormat,
            int storeId,
            bool isApproved = true,
            bool isSupplier = false,
            bool isBuyer = false,
            bool isBooker = false)
        {
            Customer = customer;
            Email = email;
            Username = username;
            Password = password;
            PasswordFormat = passwordFormat;
            StoreId = storeId;
            IsApproved = isApproved;
            IsSupplier = isSupplier;
            IsBuyer = isBuyer;
            IsBooker = isBooker;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Password format
        /// </summary>
        public PasswordFormat PasswordFormat { get; set; }

        /// <summary>
        /// Store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Is approved
        /// </summary>
        public bool IsApproved { get; set; }

        public bool IsSupplier { get; set; }

        public bool IsBuyer { get; set; }

        public bool IsBooker { get; set; }
    }
}
