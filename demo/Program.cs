using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();

var app = builder.Build();


builder.Configuration.AddAzureAppConfiguration(option =>
{
    var config = app.Configuration;
    var endpoint = config.GetValue<string>("appconfigendpoint");
    string env;
    if (app.Environment.IsDevelopment())
    {
        env = "dev";
        //option.Connect(new Uri(endpoint), new DefaultAzureCredential());
        //option.Connect(new Uri(endpoint), new AzureCliCredential());
        //option.Connect("[connectionstring]]");
    }
    else
    {
        env = config.GetValue<string>("env");
        option.Connect(new Uri(endpoint), new DefaultAzureCredential());
    }

    option.Select(KeyFilter.Any, LabelFilter.Null);
    option.Select("Demo:*");
    option.Select("Demo:*", env);
    option.ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()));
    option.UseFeatureFlags(o =>
    {
        o.CacheExpirationInterval = TimeSpan.FromSeconds(3);
    }
    );

});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAzureAppConfiguration();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
