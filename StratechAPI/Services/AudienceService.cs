using Microsoft.EntityFrameworkCore;
using StratechAPI.Model;

namespace StratechAPI.Services
{
    public class AudienceService
    {
        private readonly RdsDbContext _context;
        private readonly FileService _fileService;
        private readonly EmailService _emailService;

        public AudienceService(RdsDbContext context, FileService fileService, EmailService emailService)
        {
            _context = context;
            _fileService = fileService;
            _emailService = emailService;
        }

        /**
         * Finds audience data for a specified year and quarter,
         * generates a CSV, uploads it to S3, and emails the link.
         */
        public async Task<List<AudienceRecord>> FindAudienceData(string year, string quarter, string email)
        {
            int yearValue = ValidateYear(year);

            (DateTime startDate, DateTime endDate) = GetQuarterDates(yearValue, quarter);

            var query = _context.AudienceRecord
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate);

            var  data =  await query.ToListAsync();

            var csvStream = _fileService.GenerateCsv(data);
            var fileName = $"AudienceData_{Guid.NewGuid()}.csv";

            var preSignedUrl = await _fileService.UploadCsvToS3Async(csvStream, fileName);

            await _emailService.SendEmailAsync(email, "Audience Data", preSignedUrl);
            return data;
        }

        /**
         * Validates and parses the year input, 
         * throwing an exception if the year is invalid.
         */
        private int ValidateYear(string year)
        {
            if (string.IsNullOrEmpty(year) || !int.TryParse(year, out int yearValue))
            {
                throw new ArgumentException("Invalid year provided.");
            }
            return yearValue;
        }
        /**
        * Gets the start and end dates for the specified quarter, 
        * based on the provided year.
        */
        private (DateTime startDate, DateTime endDate) GetQuarterDates(int year, string quarter)
        {
            return quarter.ToUpper() switch
            {
                "Q1" => (new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(year, 3, 31, 23, 59, 59, DateTimeKind.Utc)),
                "Q2" => (new DateTime(year, 4, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(year, 6, 30, 23, 59, 59, DateTimeKind.Utc)),
                "Q3" => (new DateTime(year, 7, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(year, 9, 30, 23, 59, 59, DateTimeKind.Utc)),
                "Q4" => (new DateTime(year, 10, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc)),
                _ => throw new ArgumentException("Invalid quarter provided.")
            };
        }
    }
}
