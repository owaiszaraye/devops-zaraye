using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Common;

namespace Zaraye.Services.Common
{
    public partial class AmazonS3BuketService : IAmazonS3BuketService
    {
        #region Fileds
        private readonly IAmazonS3 _s3Client;
        private readonly TransferUtility _transferUtility;
        private readonly CommonSettings _commonSettings;
        #endregion

        #region Ctor

        public AmazonS3BuketService(IAmazonS3 s3Client, CommonSettings commonSettings)
        {
            _s3Client = s3Client;
            _transferUtility = new TransferUtility(_s3Client);
            _commonSettings = commonSettings;
        }

        #endregion

        #region Mehtod

        public async Task<string> UploadBase64FileAsync(string base64Data, string bucketName, string keyName)
        {
            byte[] dataBytes = Convert.FromBase64String(base64Data);
            using (MemoryStream stream = new MemoryStream(dataBytes))
            {
                await _transferUtility.UploadAsync(stream, bucketName, keyName);
            }

            return $"{_commonSettings.S3CDNname}{keyName}";
            //return $"http://{bucketName}.s3.amazonaws.com/{keyName}";
        }

        public async Task<IEnumerable<string>> GetAllObjectKeysAsync(string bucketName)
        {
            try
            {
                ListObjectsV2Response response = await _s3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = bucketName
                });

                List<string> objectKeys = new List<string>();
                foreach (S3Object entry in response.S3Objects)
                {
                    objectKeys.Add(entry.Key);
                }

                return objectKeys;
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on listing objects from S3. Message: " + e.Message);
                // Handle the exception as needed
                return new List<string>();
            }
        }

        public async Task<byte[]> GetObjectByKeyAsync(string bucketName, string key)
        {
            try
            {
                GetObjectResponse response = await _s3Client.GetObjectAsync(bucketName, key);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on getting object from S3. Message: " + e.Message);
                // Handle the exception as needed
                return null;
            }
        }

        public string GeneratePreSignedUrl(string bucketName, string key, int days = 1)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Protocol = Protocol.HTTPS, // Adjust the expiration time as needed
                Expires = DateTime.Now.AddDays(days)
            };

            var preSignedUrl = _s3Client.GetPreSignedURL(request);
            return preSignedUrl;
        }

        #endregion
    }
}
