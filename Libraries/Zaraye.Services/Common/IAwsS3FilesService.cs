using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Services.Common
{
    public partial interface IAwsS3FilesService
    {
        Task InsertAwsS3FilesAsync(AwsS3Files awsS3Files);

        Task UpdateAwsS3FilesAsync(AwsS3Files awsS3Files);

        Task<AwsS3Files> GetAwsS3FilesByIdAsync(int awsS3FilesId);

        Task DeleteAwsS3FilesAsync(AwsS3Files awsS3Files);

    }
}
