using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    //I added option to refuse any unsupported format (in content negotiation stage)
    options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters(); //but here I enabled XML support

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
