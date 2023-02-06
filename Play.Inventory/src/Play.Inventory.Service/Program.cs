using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Entities;
using Keycloak.AuthServices.Authentication;
using Play.Inventory.Service.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddMongo()
       .AddMongoRepository<InventoryItem>("inventoryitems")
       .AddMongoRepository<CatalogItem>("catalogitems")
       .AddMassTransitWithRabbitMQ();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder => {
    builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddKeycloakAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options => {
    options.AddPolicy("MatchingUserId", 
        policy => policy.Requirements.Add(new MatchingUserIdRequirement()));
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corsapp");
// TODO: uncomment this after development
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();