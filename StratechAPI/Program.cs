using Microsoft.EntityFrameworkCore;
using StratechAPI;
using StratechAPI.Model;
using StratechAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<RdsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PSQL"));
});
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddScoped<AudienceService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();
