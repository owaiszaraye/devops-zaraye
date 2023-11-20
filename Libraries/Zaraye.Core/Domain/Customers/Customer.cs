﻿using System;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Tax;

namespace Zaraye.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity,StoreEntity, ISoftDeletedEntity, DefaultColumns, IActiveActivityLogEntity
    {
        public Customer()
        {
            CustomerGuid = Guid.NewGuid();
        }

        public bool IsGuestCustomer { get; set; }

        /// <summary>
        /// Gets or sets the customer GUID
        /// </summary>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the gender
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the date of birth
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the street address
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the street address 2
        /// </summary>
        public string StreetAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the county
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets the country id
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state province id
        /// </summary>
        public int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string Phone { get; set; }

        public string Cnic { get; set; }

        /// <summary>
        /// Gets or sets the BookerType
        /// </summary>
        public string BookerType { get; set; }

        /// <summary>
        /// Gets or sets the fax
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets the vat number
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the vat number status id
        /// </summary>
        public int VatNumberStatusId { get; set; }

        /// <summary>
        /// Gets or sets the time zone id
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the custom attributes
        /// </summary>
        public string CustomCustomerAttributesXML { get; set; }

        /// <summary>
        /// Gets or sets the currency id
        /// </summary>
        public int? CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the language id
        /// </summary>
        public int? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the tax display type id
        /// </summary>
        public int? TaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets the email that should be re-validated. Used in scenarios when a customer is already registered and wants to change an email address.
        /// </summary>
        public string EmailToRevalidate { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this customer has some products in the shopping cart
        /// <remarks>The same as if we run ShoppingCartItems.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load "ShoppingCartItems" navigation property for each page load
        /// It's used only in a couple of places in the presenation layer
        /// </remarks>
        /// </summary>
        public bool HasShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is required to re-login
        /// </summary>
        public bool RequireReLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating number of failed login attempts (wrong password)
        /// </summary>
        public int FailedLoginAttempts { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which a customer cannot login (locked out)
        /// </summary>
        public DateTime? CannotLoginUntilDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        public bool IsAppActive { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the customer account is system
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the customer system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        ///  Gets or sets the store identifier in which customer registered
        /// </summary>
        public int RegisteredInStoreId { get; set; }

        /// <summary>
        /// Gets or sets the billing address identifier
        /// </summary>
        public int? BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the shipping address identifier
        /// </summary>
        public int? ShippingAddressId { get; set; }
        
        public int IndustryId { get; set; }

        public int AreaId { get; set; }

        public string Gst { get; set; }

        public int UserTypeId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool IsZarayeTransporter { get; set; }

        public bool IsPoc { get; set; }

        public int StoreId { get; set; }

        #region Custom properties

        public decimal CommissionRate { get; set; }

        public bool SendWelcomeMessage { get; set; }

        public string FullName { get; set; }

        public string Source { get; set; }

        public int CreatedBy { get; set; }
        public int SupportAgentId { get; set; }
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the vat number status
        /// </summary>
        public VatNumberStatus VatNumberStatus
        {
            get => (VatNumberStatus)VatNumberStatusId;
            set => VatNumberStatusId = (int)value;
        }

        /// <summary>
        /// Gets or sets the tax display type
        /// </summary>
        public TaxDisplayType? TaxDisplayType
        {
            get => TaxDisplayTypeId.HasValue ? (TaxDisplayType)TaxDisplayTypeId : null;
            set => TaxDisplayTypeId = value.HasValue ? (int)value : null;
        }

        #endregion
    }
}