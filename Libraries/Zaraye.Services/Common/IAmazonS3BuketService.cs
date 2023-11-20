using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial interface IAmazonS3BuketService
    {

        Task<string> UploadBase64FileAsync(string base64Data, string bucketName, string keyName);
        Task<IEnumerable<string>> GetAllObjectKeysAsync(string bucketName);
        Task<byte[]> GetObjectByKeyAsync(string bucketName, string key);
        string GeneratePreSignedUrl(string bucketName, string key, int days = 1);

    }
}
