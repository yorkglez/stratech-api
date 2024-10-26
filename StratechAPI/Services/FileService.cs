using Amazon.S3.Model;
using Amazon.S3;
using CsvHelper;
using StratechAPI.Model;
using System.Globalization;
using Amazon.Runtime;
using Amazon;

namespace StratechAPI.Services
{
    public class FileService
    {
        private readonly string _bucketName;
        private readonly AmazonS3Client _s3Client;

        public FileService(IConfiguration configuration)
        {
            _bucketName = configuration["AWS:BucketName"];

            var credentials = new BasicAWSCredentials(
                configuration["AWS:AccessKey"],
                configuration["AWS:SecretKey"]
            );

            var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

            _s3Client = new AmazonS3Client(credentials, region);
        }
        /** 
         * Generates a CSV file from a list of AudienceRecord objects 
         * and returns it as a MemoryStream.
         */
        public Stream GenerateCsv(List<AudienceRecord> records)
        {
            try
            {
                var stream = new MemoryStream();
                using (var writer = new StreamWriter(stream, leaveOpen: true))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
                stream.Position = 0;
                return stream;
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred while generating the CSV file: {e.Message}", e);
            }
        }

        /** 
         * Uploads a CSV file stream to the specified S3 bucket 
         * and returns a pre-signed URL for temporary access.
         */
        public async Task<string> UploadCsvToS3Async(Stream csvStream, string fileName)
        {
            try
            {
                var uploadRequest = new PutObjectRequest
                {
                    InputStream = csvStream,
                    BucketName = _bucketName,
                    Key = fileName
                };
                await _s3Client.PutObjectAsync(uploadRequest);
            }catch (Exception e)
            {
                throw new Exception($"An error occurred while uploading the file to S3: {e.Message}");
            }

            try
            {
                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Expires = DateTime.Now.AddHours(1)
                };

                return _s3Client.GetPreSignedURL(urlRequest);
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred while generating the pre-signed URL: {e.Message}");
            }          
        }
    }
}
