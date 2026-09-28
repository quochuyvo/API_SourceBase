using API_SourceBase.Application.Services;
using API_SourceBase.Models.Common;
using API_SourceBase.Models.Request;
using API_SourceBase.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace API_SourceBase.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PositionController : ControllerBase
    {
        private readonly IS_Position _s_Position;

        public PositionController(IS_Position s_Position)
        {
            _s_Position = s_Position;
        }

        [HttpPost]
        public async Task<IActionResult> Create(MReq_Position request)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseData<MRes_Position>(0, 400, DataAnnotationExtensionMethod.GetErrorMessage(ModelState)));
            var response = await _s_Position.Create(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(MReq_Position request)
        {
            if (!ModelState.IsValid)
                return Ok(new ResponseData<MRes_Position>(0, 400, DataAnnotationExtensionMethod.GetErrorMessage(ModelState)));
            var response = await _s_Position.Update(request);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateList(List<MReq_Position> requests)
        {
            var response = await _s_Position.UpdateList(requests);
            return Ok(response);
        }

        /// <summary>
        /// Xóa mềm / Đổi trạng thái (status = -1 là xóa mềm)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int id, short status, int updatedBy)
        {
            var response = await _s_Position.UpdateStatus(id, status, updatedBy);
            return Ok(response);
        }

        /// <summary>
        /// Xóa vật lý bản ghi khỏi cơ sở dữ liệu
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _s_Position.Delete(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _s_Position.GetById(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByStatus(short? status)
        {
            var response = await _s_Position.GetListByStatus(status);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByDepartmentId(int departmentId, short? status)
        {
            var response = await _s_Position.GetListByDepartmentId(departmentId, status);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByFullParam(string? keyword, int? departmentId, short? status)
        {
            var response = await _s_Position.GetListByFullParam(keyword, departmentId, status);
            return Ok(response);
        }
    }
}
