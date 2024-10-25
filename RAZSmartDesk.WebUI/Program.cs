using Microsoft.OpenApi.Models;
using RAZSmartDesk.DataAccess.DapperDBContext;
using RAZSmartDesk.DataAccess.Repositories;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DapperDbContext, DapperDbContext>();

builder.Services.AddTransient<IAppUserRepository, AppUserRepository>();
builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = ".NetCore API with Dapper by: BLugtu",
        Description = "Web Api with views and Swagger UI.",
        //TermsOfService = new Uri("https://example.com/terms"),
        //Contact = new OpenApiContact
        //{
        //    Name = "Company Contact",
        //    Url = new Uri("https://company.com/contact")
        //},
        //License = new OpenApiLicense
        //{
        //    Name = "Company License",
        //    Url = new Uri("https://company.com/license")
        //}
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
