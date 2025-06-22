Chưa thêm nuget đâu tạo database đã rồi thêm nuget nhá ae

Các nuget cần có
microsoft.aspnetcore.identity.entityframeworkcore\8.0.0\
microsoft.aspnetcore.identity.ui\8.0.0\
microsoft.entityframeworkcore\8.0.0\
microsoft.entityframeworkcore.design\8.0.0\
microsoft.entityframeworkcore.sqlserver\8.0.0\
microsoft.entityframeworkcore.tools\8.0.0\
microsoft.visualstudio.web.codegeneration.design\8.0.7\

Lệnh chạy nuget

Add-Migration Initial
Update-Database

dotnet tool install -g dotnet-aspnet-codegenerator --version 8.0.0

dotnet aspnet-codegenerator identity -dc Chill_Closet.Data.ChillClosetContext --files "Account.Register;Account.Login;Account.Logout;Account.Manage.Index;Account.Manage.Email;Account.Manage.ChangePassword" (ở bước này nhớ cd vào folder project kiểu ntn cd "C:\Users\baold\source\repos\Chill Closet\Chill Closet")(nếu lối)

Add-Migration AddIdentitySchemaToDB

Update-Database
