var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews(); // สำหรับ MVC
// builder.Services.AddSwaggerGen(); // ไม่ต้องใช้ Swagger

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// กำหนด default route ของ MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
