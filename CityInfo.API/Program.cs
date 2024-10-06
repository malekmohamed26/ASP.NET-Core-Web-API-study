using CityInfo.API;
using CityInfo.API.DBContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Serilog;

//We'll use Serilog to configure creating file logs instead of console logs, lines from 5 to 9 configures it
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args); //CreateBuilder(args) sets the logging info automatically
//builder.Logging.ClearProviders(); //Clears all logging information
//builder.Logging.AddConsole (); //Shows all logging informaion

builder.Host.UseSerilog(); //No need for lines 12-13 since we'll create log files instead of console logs

// Add services to the container.

builder.Services.AddControllers(options =>
{
    //I added option to refuse any unsupported format (in content negotiation stage)
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson() //Helps in using PATCH action
  .AddXmlDataContractSerializerFormatters(); //but here I enabled XML support

/*I've commented these lines because it won't help us now, but I'll leave it to know how to manipulate the error 
responses*/

//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = context =>
//    {
//        context.ProblemDetails.Extensions.Add("additionalInfo", "Additional info example");
//        //Next line is used if we work with multi-server environment instead of local environment like we are now,
//        //which helps to know which server the error is coming from.
//        context.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//    };
//});

//But instead I'll use this line to configure how potential exceptions should be handled "for those who are building APIs "
//(i.e. I want to show a nice user-friendly interface for those who are working with APIs)
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
builder.Services.AddTransient<IMailService,LocalMailService>();
#else
builder.Services.AddTransient<IMailService,CloudMailService>();
#endif
builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddDbContext<CityInfoContext>(dbContextOptions
    => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));

//to register the repository
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//1. Configure Exception handler  to show any exception on running time before sending any requests
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

//2. Configure HTTP request for normal use
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.Run();
