using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Infrastructure;
using Zaraye.Core;
using Zaraye.Data;

namespace Zaraye.Services.CustomerTestimonial
{
    public class CustomerTestimonialService : ICustomerTestimonialService
    {

        public virtual async Task DeleteCustomerTestimonialsAsync(Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial customerTestimonial)
        {
            var _customerTestimonialRepository = EngineContext.Current.Resolve<IRepository<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>>();
            await _customerTestimonialRepository.DeleteAsync(customerTestimonial);
        }

        public virtual async Task<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial> GetCustomerTestimonialByIdAsync(int appSliderId)
        {
            var _customerTestimonialRepository = EngineContext.Current.Resolve<IRepository<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>>();
            return await _customerTestimonialRepository.GetByIdAsync(appSliderId, cache => default);
        }

        public virtual async Task<IPagedList<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>> GetAllCustomerTestimonialsAsync(bool? published = null,
            string title = "", bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var _customerTestimonialRepository = EngineContext.Current.Resolve<IRepository<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>>();
            var query = _customerTestimonialRepository.Table;
            query = query.Where(br => !br.Deleted);

            if (published.HasValue)
                query = query.Where(br => br.Published == published);

            //if (!showHidden)
            //    query = query.Where(br => br.Published);

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(br => br.Title.Contains(title));

            query = query.OrderBy(br => br.DisplayOrder).ThenBy(X => X.Title);

            var customerTestimonials = await query.ToPagedListAsync(pageIndex, pageSize);
            return customerTestimonials;
        }

        public virtual async Task InsertCustomerTestimonialsAsync(Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial customerTestimonial)
        {
            var _customerTestimonialRepository = EngineContext.Current.Resolve<IRepository<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>>();
            await _customerTestimonialRepository.InsertAsync(customerTestimonial);
        }

        public virtual async Task UpdateCustomerTestimonialsAsync(Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial customerTestimonial)
        {
            var _customerTestimonialRepository = EngineContext.Current.Resolve<IRepository<Zaraye.Core.Domain.CustomerTestimonial.CustomerTestimonial>>();
            await _customerTestimonialRepository.UpdateAsync(customerTestimonial);
        }
    }
}
