var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
//common services: carter, mediatr, fluentvalidation
var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;
var orderingModule = typeof(OrderingModule).Assembly;

//common services: carter, mediatr, fluentvalidation

builder.Services
    .AddCarterWithAssemblies(catalogAssembly, basketAssembly,orderingModule);

builder.Services
    .AddMediatRWithAssemblies(catalogAssembly, basketAssembly,orderingModule);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services
    .AddMassTransitWithAssemblies(builder.Configuration, 
        catalogAssembly, 
        basketAssembly,
        orderingModule);

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

//module services: catalog, basket, ordering
builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddOrderingModule(builder.Configuration);

builder.Services.AddControllers();

builder.Services
    .AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(options => { });
app.UseAuthentication();
app.UseAuthorization();

app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

app.Run();