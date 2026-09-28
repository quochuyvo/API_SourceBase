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
    public interface IS_Department
    {
        Task<ResponseData<MRes_Department>> Create(MReq_Department request);
        Task<ResponseData<MRes_Department>> Update(MReq_Department request);
        Task<ResponseData<List<MRes_Department>>> UpdateList(List<MReq_Department> requests);
        Task<ResponseData<MRes_Department>> UpdateStatus(int id, short status, int updatedBy);
        Task<ResponseData<int>> Delete(int id);
        Task<ResponseData<MRes_Department>> GetById(int id);
        Task<ResponseData<List<MRes_Department>>> GetListByStatus(short? status);
        Task<ResponseData<List<MRes_Department>>> GetListByFullParam(string? keyword, short? status);
    }

    public class S_Department : IS_Department
    {
        private readonly MainDbContext _context;
        private readonly IMapper _mapper;

        public S_Department(MainDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Create / Update / Delete

        public async Task<ResponseData<MRes_Department>> Create(MReq_Department request)
        {
            var res = new ResponseData<MRes_Department>();
            try
            {
                #region 1. Validate trùng lặp mã phòng ban
                request.Code = request.Code?.Trim() ?? string.Empty;
                var isExist = await _context.Departments.AnyAsync(x => x.Code == request.Code && x.Status != -1);
                if (isExist)
                {
                    res.error.code = 400;
                    res.error.message = "Mã phòng ban đã tồn tại trong hệ thống!";
                    return res;
                }
                #endregion

                #region 2. Map và lưu DB
                var data = new Department();
                data.FarmId = request.FarmId;
                data.FactoryId = request.FactoryId;
                data.WorkerId = request.WorkerId;
                data.Code = request.Code;
                data.Name = request.Name?.Trim() ?? string.Empty;
                data.Sort = request.Sort;
                data.Status = request.Status == 0 ? (short)1 : request.Status;
                data.CreatedAt = DateTime.Now;
                data.CreatedBy = request.CreatedBy;

                _context.Departments.Add(data);
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

        public async Task<ResponseData<MRes_Department>> Update(MReq_Department request)
        {
            var res = new ResponseData<MRes_Department>();
            try
            {
                var data = await _context.Departments.FindAsync(request.Id);
                if (data == null)
                {
                    res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                    return res;
                }

                #region Validate trùng mã
                request.Code = request.Code?.Trim() ?? string.Empty;
                var isExist = await _context.Departments.AnyAsync(x => x.Code == request.Code && x.Id != request.Id && x.Status != -1);
                if (isExist)
                {
                    res.error.code = 400;
                    res.error.message = "Mã phòng ban đã tồn tại trên bản ghi khác!";
                    return res;
                }
                #endregion

                data.FarmId = request.FarmId;
                data.FactoryId = request.FactoryId;
                data.WorkerId = request.WorkerId;
                data.Code = request.Code;
                data.Name = request.Name?.Trim() ?? string.Empty;
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

        public async Task<ResponseData<List<MRes_Department>>> UpdateList(List<MReq_Department> requests)
        {
            var res = new ResponseData<List<MRes_Department>>();
            try
            {
                if (requests == null || !requests.Any())
                {
                    res.error.code = 400;
                    res.error.message = MessageErrorConstants.REQUEST_DATA_INVALID;
                    return res;
                }

                var ids = requests.Select(x => x.Id).ToList();
                var dbList = await _context.Departments.Where(x => ids.Contains(x.Id)).ToListAsync();
                var dbDict = dbList.ToDictionary(x => x.Id);

                var datas = new List<Department>();
                foreach (var request in requests)
                {
                    if (!dbDict.TryGetValue(request.Id, out var data))
                    {
                        res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                        return res;
                    }

                    data.FarmId = request.FarmId;
                    data.FactoryId = request.FactoryId;
                    data.WorkerId = request.WorkerId;
                    data.Code = request.Code?.Trim() ?? string.Empty;
                    data.Name = request.Name?.Trim() ?? string.Empty;
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

                res.data = _mapper.Map<List<MRes_Department>>(datas);
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

        public async Task<ResponseData<MRes_Department>> UpdateStatus(int id, short status, int updatedBy)
        {
            var res = new ResponseData<MRes_Department>();
            try
            {
                var data = await _context.Departments.FindAsync(id);
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
                var data = await _context.Departments.FindAsync(id);
                if (data == null)
                {
                    res.error.message = MessageErrorConstants.DO_NOT_FIND_DATA;
                    return res;
                }

                _context.Departments.Remove(data);
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

        public async Task<ResponseData<MRes_Department>> GetById(int id)
        {
            var res = new ResponseData<MRes_Department>();
            try
            {
                var data = await (
                    from d in _context.Departments.AsNoTracking()
                    join ca in _context.Accounts.AsNoTracking() on d.CreatedBy equals ca.Id into caGroup
                    from ca in caGroup.DefaultIfEmpty()
                    join ua in _context.Accounts.AsNoTracking() on d.UpdatedBy equals ua.Id into uaGroup
                    from ua in uaGroup.DefaultIfEmpty()
                    where d.Id == id
                    select new MRes_Department
                    {
                        Id = d.Id,
                        FarmId = d.FarmId,
                        FactoryId = d.FactoryId,
                        WorkerId = d.WorkerId,
                        Code = d.Code,
                        Name = d.Name,
                        Sort = d.Sort,
                        Status = d.Status,
                        CreatedAt = d.CreatedAt,
                        CreatedBy = d.CreatedBy,
                        UpdatedAt = d.UpdatedAt,
                        UpdatedBy = d.UpdatedBy,
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

        public async Task<ResponseData<List<MRes_Department>>> GetListByStatus(short? status)
        {
            var res = new ResponseData<List<MRes_Department>>();
            try
            {
                var query = _context.Departments.AsNoTracking();
                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }
                else
                {
                    query = query.Where(x => x.Status != -1);
                }

                var list = await query.OrderBy(x => x.Sort).ThenBy(x => x.Name).ToListAsync();
                res.data = _mapper.Map<List<MRes_Department>>(list);
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

        public async Task<ResponseData<List<MRes_Department>>> GetListByFullParam(string? keyword, short? status)
        {
            var res = new ResponseData<List<MRes_Department>>();
            try
            {
                var query = _context.Departments.AsNoTracking();
                if (status.HasValue)
                {
                    query = query.Where(x => x.Status == status.Value);
                }
                else
                {
                    query = query.Where(x => x.Status != -1);
                }

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.Trim().ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(keyword) || x.Code.ToLower().Contains(keyword));
                }

                var list = await (
                    from d in query
                    join ca in _context.Accounts.AsNoTracking() on d.CreatedBy equals ca.Id into caGroup
                    from ca in caGroup.DefaultIfEmpty()
                    join ua in _context.Accounts.AsNoTracking() on d.UpdatedBy equals ua.Id into uaGroup
                    from ua in uaGroup.DefaultIfEmpty()
                    orderby d.Sort, d.Name
                    select new MRes_Department
                    {
                        Id = d.Id,
                        FarmId = d.FarmId,
                        FactoryId = d.FactoryId,
                        WorkerId = d.WorkerId,
                        Code = d.Code,
                        Name = d.Name,
                        Sort = d.Sort,
                        Status = d.Status,
                        CreatedAt = d.CreatedAt,
                        CreatedBy = d.CreatedBy,
                        UpdatedAt = d.UpdatedAt,
                        UpdatedBy = d.UpdatedBy,
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
