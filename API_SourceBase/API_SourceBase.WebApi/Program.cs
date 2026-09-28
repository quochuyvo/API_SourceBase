using API_SourceBase.Application.Mapper;
using API_SourceBase.Application.Services;
using API_SourceBase.Data.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm Controllers
builder.Services.AddControllers();

// 2. Cấu hình DbContext (mặc định dùng In-Memory Database để chạy thử nghiệm độc lập)
builder.Services.AddDbContext<MainDbContext>(options =>
{
    options.UseInMemoryDatabase("API_SourceBase_Db");
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 3. Đăng ký AutoMapper Profile
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 4. Đăng ký Dependency Injection cho Services
builder.Services.AddScoped<IS_Department, S_Department>();
builder.Services.AddScoped<IS_Position, S_Position>();

// 5. Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API SourceBase - Department & Position",
        Version = "v1",
        Description = "Source Base chuẩn hóa kiến trúc Layered 5 project (S_CompanyEvent pattern)"
    });
});

// 6. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Tự động seed dữ liệu mẫu vào bộ nhớ khi khởi chạy
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    context.SeedData();
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API SourceBase v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
