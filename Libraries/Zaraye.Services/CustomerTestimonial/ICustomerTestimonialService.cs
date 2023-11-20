using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core;

namespace Zaraye.Services.CustomerTestimonial
{
    public interface ICustomerTestimonialService
    {
        Task DeleteCustomerTestimonialsAsync(Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial customerTestimonial);

        Task<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial> GetCustomerTestimonialByIdAsync(int customerTestimonialId);

        Task<IPagedList<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>> GetAllCustomerTestimonialsAsync(bool? published = null,
            string title = "", bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        Task InsertCustomerTestimonialsAsync(Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial customerTestimonial);

        Task UpdateCustomerTestimonialsAsync(Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial customerTestimonial);
    }
}
