using Mango.ServiceBus;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

builder.Services.AddHttpClient<IBaseService,BaseService>();
builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddHttpClient<IMessageBus, MessageBus>();
builder.Services.AddScoped<IBaseService, BaseService>();

builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
Mango.Web.Utility.SD.CouponAPIBaseUrl = builder.Configuration["ServiceUrls:CouponAPI"];
Mango.Web.Utility.SD.AuthAPIBaseUrl = builder.Configuration["ServiceUrls:AuthAPI"];
Mango.Web.Utility.SD.ProductAPIBaseUrl = builder.Configuration["ServiceUrls:ProductAPI"];
Mango.Web.Utility.SD.ShoppingCartAPIBaseUrl = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
Mango.Web.Utility.SD.OrderAPIBaseUrl = builder.Configuration["ServiceUrls:OrderAPI"];
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  options.Cookie.HttpOnly = true;
                  options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                  options.LoginPath = "/Auth/Login";
                  options.AccessDeniedPath = "/Auth/AccessDenied";
                  options.SlidingExpiration = true;
              });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
