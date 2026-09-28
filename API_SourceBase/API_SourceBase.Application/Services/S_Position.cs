using AutoMapper;
using API_SourceBase.Data.EF;
using API_SourceBase.Data.Entities;
using API_SourceBase.Models.Common;
using API_SourceBase.Models.Request;
using API_SourceBase.Models.Response;
using API_SourceBase.Utilities.Constants;
using Microsoft.EntityFrameworkCore;

namespace API_SourceBase.Application.Services
{
    public interface IS_Position
    {
        Task<ResponseData<MRes_Position>> Create(MReq_Position request);
        Task<ResponseData<MRes_Position>> Update(MReq_Position request);
        Task<ResponseData<List<MRes_Position>>> UpdateList(List<MReq_Position> requests);
        Task<ResponseData<MRes_Position>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Position>> GetById(int id);
        Task<ResponseData<List<MRes_Position>>> GetListByStatus(short? status);
        Task<ResponseData<List<MRes_Position>>> GetListByDepartmentId(int departmentId, short? status);
        Task<ResponseData<List<MRes_Position>>> GetListByFullParam(string? keyword, int? departmentId, short? status);
    }

    public class S_Position : IS_Position
    {
        private readonly DemoDbContext _context;
        private readonly IMapper _mapper;

        public S_Position(DemoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Create / Update / Delete

        public async Task<ResponseData<MRes_Position>> Create(MReq_Position request)
        {
            var res = new ResponseData<MRes_Position>();
            try
            {
                #region 1. Validate phòng ban tồn tại và trùng lặp mã chức vụ
                var deptExists = await _context.Departments.AnyAsync(x => x.Id == request.DepartmentId && x.Status != -1);
                if (!deptExists)
                {
                    res.error.code = 400;
                    res.error.message = "Phòng ban không tồn tại trong hệ thống!";
                    return res;
                }

                request.Code = request.Code?.Trim() ?? string.Empty;
                var isExist = await _context.Positions.AnyAsync(x => x.Code == request.Code && x.Status != -1);
                if (isExist)
                {
                    res.error.code = 400;
                    res.error.message = "Mã chức vụ đã tồn tại trong hệ thống!";
                    return res;
                }
                #endregion

                #region 2. Map và lưu DB
                var data = new Position();
                data.DepartmentId = request.DepartmentId;
                data.Name = request.Name?.Trim() ?? string.Empty;
                data.Type = request.Type;
                data.Role = request.Role;
                data.Code = request.Code;
                data.Sort = request.Sort;
                data.Status = request.Status == 0 ? (short)1 : request.Status;
                data.CreatedAt = DateTime.Now;
                data.CreatedBy = request.CreatedBy;

                _context.Positions.Add(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.EXCEPTION_DO_NOT_CREATE;
                    return res;
                }
                #endregion

                #region 3. Re-fetch dữ liệu
                var getById = await GetById(data.Id);
                res.data = getById.data;
                res.result = 1;
                res.error.code = 201;
                res.error.message = MessageErrorConstants.CREATE_SUCCESS;
                #endregion
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<MRes_Position>> Update(MReq_Position request)
        {
            var res = new ResponseData<MRes_Position>();
            try
            {
                var data = await _context.Positions.FindAsync(request.Id);
                if (data == null)
                {
                    res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                    return res;
                }

                #region Validate phòng ban tồn tại và trùng mã chức vụ
                var deptExists = await _context.Departments.AnyAsync(x => x.Id == request.DepartmentId && x.Status != -1);
                if (!deptExists)
                {
                    res.error.code = 400;
                    res.error.message = "Phòng ban không tồn tại trong hệ thống!";
                    return res;
                }

                request.Code = request.Code?.Trim() ?? string.Empty;
                var isExist = await _context.Positions.AnyAsync(x => x.Code == request.Code && x.Id != request.Id && x.Status != -1);
                if (isExist)
                {
                    res.error.code = 400;
                    res.error.message = "Mã chức vụ đã tồn tại trên bản ghi khác!";
                    return res;
                }
                #endregion

                data.DepartmentId = request.DepartmentId;
                data.Name = request.Name?.Trim() ?? string.Empty;
                data.Type = request.Type;
                data.Role = request.Role;
                data.Code = request.Code;
                data.Sort = request.Sort;
                data.Status = request.Status;
                data.UpdatedAt = DateTime.Now;
                data.UpdatedBy = request.UpdatedBy;

                _context.Update(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE;
                    return res;
                }

                var getById = await GetById(data.Id);
                res.data = getById.data;
                res.result = 1;
                res.error.message = MessageErrorConstants.UPDATE_SUCCESS;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<List<MRes_Position>>> UpdateList(List<MReq_Position> requests)
        {
            var res = new ResponseData<List<MRes_Position>>();
            try
            {
                if (requests == null || !requests.Any())
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.REQUEST_DATA_INVALID;
                    return res;
                }

                var ids = requests.Select(x => x.Id).ToList();
                var dbList = await _context.Positions.Where(x => ids.Contains(x.Id)).ToListAsync();
                var dbDict = dbList.ToDictionary(x => x.Id);

                var datas = new List<Position>();
                foreach (var request in requests)
                {
                    if (!dbDict.TryGetValue(request.Id, out var data))
                    {
                        res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                        return res;
                    }

                    data.DepartmentId = request.DepartmentId;
                    data.Name = request.Name?.Trim() ?? string.Empty;
                    data.Type = request.Type;
                    data.Role = request.Role;
                    data.Code = request.Code?.Trim() ?? string.Empty;
                    data.Sort = request.Sort;
                    data.Status = request.Status;
                    data.UpdatedAt = DateTime.Now;
                    data.UpdatedBy = request.UpdatedBy;

                    _context.Update(data);
                    datas.Add(data);
                }

                var save = await _context.SaveChangesAsync();
                if (save == 0 && datas.Any())
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE;
                    return res;
                }

                res.data = _mapper.Map<List<MRes_Position>>(datas);
                res.result = 1;
                res.error.message = MessageErrorConstants.UPDATE_SUCCESS;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<MRes_Position>> UpdateStatus(int id, short status, int updatedBy)
        {
            var res = new ResponseData<MRes_Position>();
            try
            {
                var data = await _context.Positions.FindAsync(id);
                if (data == null)
                {
                    res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                    return res;
                }

                data.Status = status;
                data.UpdatedAt = DateTime.Now;
                data.UpdatedBy = updatedBy;

                _context.Update(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.EXCEPTION_DO_NOT_UPDATE;
                    return res;
                }

                var getById = await GetById(data.Id);
                res.data = getById.data;
                res.result = 1;
                res.error.message = MessageErrorConstants.UPDATE_SUCCESS;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<int>> Delete(int id)
        {
            var res = new ResponseData<int>();
            try
            {
                var data = await _context.Positions.FindAsync(id);
                if (data == null)
                {
                    res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                    return res;
                }

                _context.Positions.Remove(data);
                var save = await _context.SaveChangesAsync();
                if (save == 0)
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.EXCEPTION_DO_NOT_DELETE;
                    return res;
                }

                res.data = save;
                res.result = 1;
                res.error.message = MessageErrorConstants.DELETE_SUCCESS;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        #endregion

        #region Query / Get

        public async Task<ResponseData<MRes_Position>> GetById(int id)
        {
            var res = new ResponseData<MRes_Position>();
            try
            {
                var data = await (
                    from p in _context.Positions.AsNoTracking()
                    join d in _context.Departments.AsNoTracking() on p.DepartmentId equals d.Id into dGroup
                    from d in dGroup.DefaultIfEmpty()
                    join ca in _context.Accounts.AsNoTracking() on p.CreatedBy equals ca.Id into caGroup
                    from ca in caGroup.DefaultIfEmpty()
                    join ua in _context.Accounts.AsNoTracking() on p.UpdatedBy equals ua.Id into uaGroup
                    from ua in uaGroup.DefaultIfEmpty()
                    where p.Id == id
                    select new MRes_Position
                    {
                        Id = p.Id,
                        DepartmentId = p.DepartmentId,
                        DepartmentName = d != null ? d.Name : null,
                        Name = p.Name,
                        Type = p.Type,
                        Role = p.Role,
                        Code = p.Code,
                        Sort = p.Sort,
                        Status = p.Status,
                        CreatedAt = p.CreatedAt,
                        CreatedBy = p.CreatedBy,
                        UpdatedAt = p.UpdatedAt,
                        UpdatedBy = p.UpdatedBy,
                        CreatedByObj = ca == null ? null : new MRes_Account_Info_Custom
                        {
                            Id = ca.Id,
                            UserName = ca.UserName,
                            FirstName = ca.FirstName,
                            LastName = ca.LastName
                        },
                        UpdatedByObj = ua == null ? null : new MRes_Account_Info_Custom
                        {
                            Id = ua.Id,
                            UserName = ua.UserName,
                            FirstName = ua.FirstName,
                            LastName = ua.LastName
                        }
                    }).FirstOrDefaultAsync();

                if (data == null)
                {
                    res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                    return res;
                }

                res.data = data;
                res.result = 1;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<List<MRes_Position>>> GetListByStatus(short? status)
        {
            var res = new ResponseData<List<MRes_Position>>();
            try
            {
                var query = _context.Positions.AsNoTracking();
                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }
                else
                {
                    query = query.Where(x => x.Status != -1);
                }

                var list = await query.OrderBy(x => x.Sort).ThenBy(x => x.Name).ToListAsync();
                res.data = _mapper.Map<List<MRes_Position>>(list);
                res.result = 1;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<List<MRes_Position>>> GetListByDepartmentId(int departmentId, short? status)
        {
            var res = new ResponseData<List<MRes_Position>>();
            try
            {
                var query = _context.Positions.AsNoTracking().Where(x => x.DepartmentId == departmentId);
                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }
                else
                {
                    query = query.Where(x => x.Status != -1);
                }

                var list = await query.OrderBy(x => x.Sort).ThenBy(x => x.Name).ToListAsync();
                res.data = _mapper.Map<List<MRes_Position>>(list);
                res.result = 1;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        public async Task<ResponseData<List<MRes_Position>>> GetListByFullParam(string? keyword, int? departmentId, short? status)
        {
            var res = new ResponseData<List<MRes_Position>>();
            try
            {
                var query = _context.Positions.AsNoTracking();
                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }
                else
                {
                    query = query.Where(x => x.Status != -1);
                }

                if (departmentId.HasValue && departmentId.Value > 0)
                {
                    query = query.Where(x => x.DepartmentId == departmentId.Value);
                }

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
                }

                var list = await (
                    from p in query
                    join d in _context.Departments.AsNoTracking() on p.DepartmentId equals d.Id into dGroup
                    from d in dGroup.DefaultIfEmpty()
                    join ca in _context.Accounts.AsNoTracking() on p.CreatedBy equals ca.Id into caGroup
                    from ca in caGroup.DefaultIfEmpty()
                    join ua in _context.Accounts.AsNoTracking() on p.UpdatedBy equals ua.Id into uaGroup
                    from ua in uaGroup.DefaultIfEmpty()
                    orderby p.Sort, p.Name
                    select new MRes_Position
                    {
                        Id = p.Id,
                        DepartmentId = p.DepartmentId,
                        DepartmentName = d != null ? d.Name : null,
                        Name = p.Name,
                        Type = p.Type,
                        Role = p.Role,
                        Code = p.Code,
                        Sort = p.Sort,
                        Status = p.Status,
                        CreatedAt = p.CreatedAt,
                        CreatedBy = p.CreatedBy,
                        UpdatedAt = p.UpdatedAt,
                        UpdatedBy = p.UpdatedBy,
                        CreatedByObj = ca == null ? null : new MRes_Account_Info_Custom
                        {
                            Id = ca.Id,
                            UserName = ca.UserName,
                            FirstName = ca.FirstName,
                            LastName = ca.LastName
                        },
                        UpdatedByObj = ua == null ? null : new MRes_Account_Info_Custom
                        {
                            Id = ua.Id,
                            UserName = ua.UserName,
                            FirstName = ua.FirstName,
                            LastName = ua.LastName
                        }
                    }).ToListAsync();

                res.data = list;
                res.result = 1;
            }
            catch (Exception ex)
            {
                res.result = -1;
                res.error.code = 500;
                res.error.message = $"Exception: {ex.Message}\r\n{ex.InnerException?.Message}";
            }
            return res;
        }

        #endregion
    }
}
