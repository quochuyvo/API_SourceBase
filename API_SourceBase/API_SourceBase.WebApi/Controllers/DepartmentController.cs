using API_SourceBase.Application.Services;
using API_SourceBase.Models.Common;
using API_SourceBase.Models.Request;
using API_SourceBase.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace API_SourceBase.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IS_Department _s_Department;

        public DepartmentController(IS_Department s_Department)
        {
            _s_Department = s_Department;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Department request)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseData<MRes_Department>(0, 400, DataAnnotationExtensionMethod.GetErrorMessage(ModelState)));
            var response = await _s_Department.Create(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Department request)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseData<MRes_Department>(0, 400, DataAnnotationExtensionMethod.GetErrorMessage(ModelState)));
            var response = await _s_Department.Update(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateList(List<MReq_Department> requests)
        {
            var response = await _s_Department.UpdateList(requests);
            return Ok(response);
        }

        /// <summary>
        /// Xóa mềm / Đổi trạng thái (status = -1 là xóa mềm)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status, int updatedBy)
        {
            var response = await _s_Department.UpdateStatus(id, status, updatedBy);
            return Ok(response);
        }

        /// <summary>
        /// Xóa vật lý bản ghi khỏi cơ sở dữ liệu
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _s_Department.Delete(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _s_Department.GetById(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByStatus(short? status)
        {
            var response = await _s_Department.GetListByStatus(status);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam(string? keyword, short? status)
        {
            var response = await _s_Department.GetListByFullParam(keyword, status);
            return Ok(response);
        }
    }
}
