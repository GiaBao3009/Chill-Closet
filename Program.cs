using Chill_Closet.Data;
using Chill_Closet.Models;
using Chill_Closet.Repository;
using Chill_Closet.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ChillStoreConnection") ?? throw new InvalidOperationException("Connection string 'ChillStoreConnection' not found.");

// Đăng ký DbContext với chuỗi kết nối
builder.Services.AddDbContext<ChillClosetContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ChillClosetContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    // Policy này yêu cầu người dùng phải có vai trò "Admin"
    options.AddPolicy("RequireAdminRole",
        policy => policy.RequireRole("Admin"));
});

// Đăng ký các dịch vụ Repository
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IOrderRepository, EFOrderRepository>();
builder.Services.AddScoped<IVoucherRepository, EFVoucherRepository>();
builder.Services.AddScoped<INotificationRepository, EFNotificationRepository>();
builder.Services.AddScoped<IReturnRequestRepository, EFReturnRequestRepository>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting(); 
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// =======================================================
// SEEDING ROLES VÀ ADMIN USER - ĐẶT Ở ĐÂY (TRƯỚC app.Run())
// =======================================================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roleNames = { "Admin", "Customer" };
    IdentityResult roleResult;

    // Tạo Roles
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Tạo tài khoản Admin (cũng phải nằm bên trong scope)
    var adminEmail = builder.Configuration["AdminUser:Email"];
    var adminPassword = builder.Configuration["AdminUser:Password"];

    if (adminEmail != null && adminPassword != null)
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = builder.Configuration["AdminUser:FullName"],
                EmailConfirmed = true // Xác thực email luôn cho admin
            };
            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, "Admin"); // Gán vai trò Admin
        }
    }
}
// =======================================================
// KẾT THÚC PHẦN SEEDING
// =======================================================


// app.Run() là dòng cuối cùng
app.Run();