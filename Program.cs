using AuthorizationWithCustomClaim.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var conn = builder.Configuration.GetConnectionString("connectionString");

builder.Services.ConfigureDbContext(conn);
builder.Services.ConfigureAppSetting(builder.Configuration);
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureDefaultIdentity();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
