using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;
using Zaraye.Core;
using Zaraye.Data;

namespace Zaraye.Services.Common
{
    public partial class AwsS3FilesService : IAwsS3FilesService
    {
        #region Fileds
        private readonly IRepository<AwsS3Files> _awsS3FilesRepository;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor
        public AwsS3FilesService(IRepository<AwsS3Files> awsS3FilesRepository,
            IWorkContext workContext)
        {
            _awsS3FilesRepository = awsS3FilesRepository;
            _workContext = workContext;
        }
        #endregion

        #region Mehtod

        public virtual async Task InsertAwsS3FilesAsync(AwsS3Files awsS3Files)
        {
            await _awsS3FilesRepository.InsertAsync(awsS3Files);
        }

        public virtual async Task UpdateAwsS3FilesAsync(AwsS3Files awsS3Files)
        {
            await _awsS3FilesRepository.UpdateAsync(awsS3Files);
        }

        public virtual async Task<AwsS3Files> GetAwsS3FilesByIdAsync(int awsS3FilesId)
        {
            return await _awsS3FilesRepository.GetByIdAsync(awsS3FilesId);
        }

        public virtual async Task DeleteAwsS3FilesAsync(AwsS3Files awsS3Files)
        {
            await _awsS3FilesRepository.DeleteAsync(awsS3Files);
        }

        #endregion
    }
}
