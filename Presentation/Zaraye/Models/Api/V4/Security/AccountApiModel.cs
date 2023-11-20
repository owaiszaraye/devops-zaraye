using Zaraye.Models.Api.V4.Common;
using System;
using System.Collections.Generic;
using static Zaraye.Models.Api.V4.Buyer.BuyerInfoAddressApiModel;

namespace Zaraye.Models.Api.V4.Security
{
    public partial class AccountApiModel : BaseApiModel
    {
        public class LoginApiModel
        {
            public string Email { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }

        public class RegisterApiModel
        {
            public RegisterApiModel()
            {
                Roles = new List<string>();
            }

            public string Phone { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string? Address { get; set; }
            public int CountryId { get; set; }
            public int StateId { get; set; }
            public string userType { get; set; }

            public IList<string> Roles { get; set; }

            public BuyerInfoAddressPinLocationApiModel BuyerPinLocation { get; set; }
        }

        public class BuyerRegisterApiModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }

            public string Address { get; set; }
            public string Address2 { get; set; }
            public int CountryId { get; set; }
            public int StateProvinceId { get; set; }
            public int AreaId { get; set; }
            public string Phone { get; set; }
            public int IndustryId { get; set; }
            public string BuyerType { get; set; }

            public BuyerLocationModel BuyerPinLocation { get; set; }

            public class BuyerLocationModel
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
                public string Location { get; set; }
            }
        }

        public class BookerBuyerRegisterApiModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public int CountryId { get; set; }
            public int StateProvinceId { get; set; }
            public string Phone { get; set; }
            public int IndustryId { get; set; }
            public int BuyerType { get; set; }

            public string BuyerNTN { get; set; }
            public string BuyerGST { get; set; }

            public BookerBuyerLocationModel BookerCurrentLocation { get; set; }
            public BookerBuyerLocationModel BuyerPinLocation { get; set; }

            public class BookerBuyerLocationModel
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
                public string Location { get; set; }
            }
        }

        public class SellerRegisterApiModel
        {
            public SellerRegisterApiModel()
            {
                ProductIds = new List<int>();
            }

            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public int IndustryId { get; set; }
            public int CountryId { get; set; }
            public int StateProvinceId { get; set; }
            public int AreaId { get; set; }
            public string Phone { get; set; }
            public string SupplierType { get; set; }

            public List<int> ProductIds { get; set; }

            public SupplierLocationModel SellerPinLocation { get; set; }

            public class SupplierLocationModel
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
                public string Location { get; set; }
            }
        }

        public class BookerSellerRegisterApiModel
        {
            public BookerSellerRegisterApiModel()
            {
                ProductIds = new List<int>();
            }

            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public int IndustryId { get; set; }
            public int CountryId { get; set; }
            public int StateProvinceId { get; set; }
            public string Phone { get; set; }
            public int SupplierType { get; set; }

            public string SupplierNTN { get; set; }
            public string SupplierGST { get; set; }

            public List<int> ProductIds { get; set; }

            public BookerSupplierLocationModel BookerCurrentLocation { get; set; }
            public BookerSupplierLocationModel SellerPinLocation { get; set; }

            public class BookerSupplierLocationModel
            {
                public string Latitude { get; set; }
                public string Longitude { get; set; }
                public string Location { get; set; }
            }
        }

        public class ChangePasswordApiModel
        {
            public string NewPassword { get; set; }
            public string OldPassword { get; set; }
        }

        public class RequestPasswordApiModel
        {
            public string Email { get; set; }
        }
        public class EmployeeApiModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserType { get; set; }
        }
    }
}