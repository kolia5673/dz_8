using Amazon.S3;
using Amazon.S3.Model;

namespace MyHRExample.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3, IConfiguration configuration)
        {
            _s3 = s3;
            _bucketName = configuration["AWS:BucketName"]!;
        }

        public async Task UploadAsync(Stream stream, string key, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType
            };

            await _s3.PutObjectAsync(request);
        }

        public async Task<string> GetPresignedUrlAsync(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(10)
            };

            return await _s3.GetPreSignedURLAsync(request);
        }

        public async Task DeleteAsync(string key)
        {
            await _s3.DeleteObjectAsync(_bucketName, key);
        }
    }
}