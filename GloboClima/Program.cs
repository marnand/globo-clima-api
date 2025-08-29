using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GloboClima.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

if (string.IsNullOrEmpty(builder.Configuration["APIKEY"]))
{
    builder.Configuration["APIKEY"] = Environment.GetEnvironmentVariable("WEATHER_API_KEY") ?? "";
}

var dynamoDbConfig = new AmazonDynamoDBConfig();
var dynamoDbEndpoint = builder.Configuration["DYNAMODB_ENDPOINT"];

if (!string.IsNullOrEmpty(dynamoDbEndpoint))
{
    dynamoDbConfig.ServiceURL = dynamoDbEndpoint;
    dynamoDbConfig.UseHttp = true;
    
    builder.Services.AddSingleton<IAmazonDynamoDB>(provider =>
    {
        return new AmazonDynamoDBClient("dummy", "dummy", dynamoDbConfig);
    });
}
else
{
    builder.Services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(dynamoDbConfig));
}

builder.Services.AddSingleton<IDynamoDBContext>(provider =>
{
    var client = provider.GetService<IAmazonDynamoDB>();
    return new DynamoDBContext(client);
});

var jwtSecret = builder.Configuration["JWT_SECRET"] ?? "chave-de-api-se-env-vazio";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddHttpClient<ICountryService, CountryService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});

app.MapHealthChecks("/health");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dynamoDbContext = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();
    var client = scope.ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
    
    try
    {
        var tableNames = await client.ListTablesAsync();
        if (!tableNames.TableNames.Contains("Users"))
        {
            await client.CreateTableAsync(new Amazon.DynamoDBv2.Model.CreateTableRequest
            {
                TableName = "Users",
                KeySchema = new List<Amazon.DynamoDBv2.Model.KeySchemaElement>
                {
                    new Amazon.DynamoDBv2.Model.KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = Amazon.DynamoDBv2.KeyType.HASH
                    }
                },
                AttributeDefinitions = new List<Amazon.DynamoDBv2.Model.AttributeDefinition>
                {
                    new Amazon.DynamoDBv2.Model.AttributeDefinition
                    {
                        AttributeName = "Id",
                        AttributeType = Amazon.DynamoDBv2.ScalarAttributeType.S
                    }
                },
                BillingMode = Amazon.DynamoDBv2.BillingMode.PAY_PER_REQUEST
            });
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Não foi possível criar/verificar a tabela Users no DynamoDB");
    }
}

app.Run();
