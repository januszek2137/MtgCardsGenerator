using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AiMagicCardsGenerator.Data;
using AiMagicCardsGenerator.Models;
using AiMagicCardsGenerator.Repositories;
using AiMagicCardsGenerator.Services;
using AiMagicCardsGenerator.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddHttpClient<IScryfallService, ScryfallService>(client => {
    client.BaseAddress = new Uri("https://api.scryfall.com/");
    client.DefaultRequestHeaders.Add("User-Agent", "MtgCardGenerator/1.0");
});
builder.Services.AddHttpClient<IGeneratorService, GeneratorService>();
builder.Services.AddScoped<ICardRenderService, CardRenderService>();
builder.Services.AddScoped<IGeneratedCardRepository, GeneratedCardRepository>();
builder.Services.AddHttpClient<IImageGeneratorService, ImageGeneratorService>(client => {
    client.Timeout = TimeSpan.FromSeconds(60); // 60 seconds for image generation
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRateLimiter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();