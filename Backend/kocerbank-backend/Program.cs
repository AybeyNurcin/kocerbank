using kocerbank_backend.DataAccess;
using kocerbank_backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<SubeRepository>();
builder.Services.AddScoped<SubeService>();
builder.Services.AddScoped<MusteriRepository>();
builder.Services.AddScoped<MusteriService>();
builder.Services.AddScoped<PersonelRepository>();
builder.Services.AddScoped<PersonelService>();
builder.Services.AddScoped<MusteriIletisimRepository>();
builder.Services.AddScoped<MusteriIletisimService>();
builder.Services.AddScoped<HesapRepository>();
builder.Services.AddScoped<HesapService>();
builder.Services.AddScoped<HesapHareketiRepository>();
builder.Services.AddScoped<HesapHareketiService>();
builder.Services.AddScoped<ParaTransferRepository>();
builder.Services.AddScoped<ParaTransferServis>();
builder.Services.AddScoped<DovizKuruServis>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:59718",
                "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
