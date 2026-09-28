# API SourceBase - .NET 8 Layered Architecture

Repository mã nguồn mẫu (Source Base) chuẩn hóa theo kiến trúc phân tầng 5 Project (Layered Architecture) bám sát quy chuẩn thực tế của `API_VrgMekong`, sử dụng 2 nghiệp vụ mẫu là **Department** (Phòng ban) và **Position** (Chức vụ).

---

## 1. Cấu trúc 5 Tầng Phân Minh

```text
API_SourceBase/
├── API_SourceBase.sln                 <-- File Solution tổng (mở 1 click là load đủ 5 project)
│
├── API_SourceBase.WebApi/             <-- [TẦNG HOST]
│   ├── Controllers/                   (DepartmentController, PositionController)
│   ├── Properties/                    (launchSettings.json - cấu hình port và auto mở Swagger)
│   ├── appsettings.json               (Cấu hình chuỗi kết nối)
│   └── Program.cs                     (Đăng ký DI, DbContext, AutoMapper, Middleware)
│
├── API_SourceBase.Application/        <-- [TẦNG BUSINESS LOGIC]
│   ├── Services/                      (S_Department, S_Position - interface & class chung 1 file)
│   └── Mapper/                        (AutoMapperProfile - cấu hình ánh xạ Entity <-> DTO)
│
├── API_SourceBase.Data/               <-- [TẦNG DATA ACCESS / EF CORE]
│   ├── Entities/                      (Department, Position, Account)
│   └── EF/                            (DemoDbContext - có sẵn SeedData mẫu để test ngay)
│
├── API_SourceBase.Models/             <-- [TẦNG DTO / CONTRACTS]
│   ├── Common/                        (ResponseData<T>, BaseModel.History, ExtensionMethods)
│   ├── Request/                       (MReq_Department, MReq_Position)
│   └── Response/                      (MRes_Department, MRes_Position, MRes_Account_Info_Custom)
│
└── API_SourceBase.Utilities/          <-- [TẦNG TIỆN ÍCH / DÙNG CHUNG]
    └── Constants/                     (MessageErrorConstants)
```

---

## 2. Hướng dẫn Chạy và Kiểm thử

1. Mở file **`API_SourceBase.sln`** bằng Visual Studio.
2. Nhấn **F5** (hoặc `dotnet run --project API_SourceBase.WebApi`).
3. Trình duyệt tự động mở trang **Swagger UI**: `https://localhost:7234/swagger`.
4. Mặc định dự án sử dụng **EF Core In-Memory Database** và đã nạp sẵn dữ liệu mẫu ban đầu, có thể test các API ngay lập tức.
