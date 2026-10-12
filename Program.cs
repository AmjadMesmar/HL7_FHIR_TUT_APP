using Hl7FhirWorkbench.Components;

var builder = WebApplication.CreateBuilder(args);

// Support dynamic cloud hosting port (e.g., Render, Railway, Fly.io, Cloud Run)
var cloudPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(cloudPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{cloudPort}");
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<Hl7FhirWorkbench.Models.Fhir.FhirService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
