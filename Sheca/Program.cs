using Hangfire;
using Microsoft.EntityFrameworkCore;
using Sheca.Dtos.Settings;
using Sheca.Extensions;
using Sheca.Middlewares;
using Sheca.Models;
using Sheca.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureCors();

// Add services to the container.
builder.Services.AddControllers().AddCustomOptions();

builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ConfigureSwaggerOptions();
    c.EnableAnnotations();
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddHangFireService(builder.Configuration);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext"));
}
);
builder.Services.ConfigureRepository();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfireDashboard();

BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.ConfigureExceptionHandler(app.Logger);
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

RecurringJob.AddOrUpdate("easyjob", ( )=> Console.WriteLine($"Test Schedule {DateTime.Now}"), "*/1 * * * *");
RecurringJob.AddOrUpdate<EmailJob>(emailJob => emailJob.SendEmail(), "55 23 * * *");

app.UseMiddleware<JwtMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});

app.Run();
