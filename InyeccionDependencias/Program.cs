
using InyeccionDependencias.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddTransient<IGuidGenerator, GuidGenerator>();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseRouting();

// app.UseAuthorization();

// app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transient}/{action=TestTransient}/{id?}")
    .WithStaticAssets();


app.Run();
