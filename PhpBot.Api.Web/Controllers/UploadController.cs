using Microsoft.AspNetCore.Mvc;
using PhpBot.Api.Core.Auth.AccessService;
using PhpBot.Api.Core.Uploading.UploadService;

namespace PhpBot.Api.Web.Controllers
{
    [ApiController, Route("api/upload")]

    public class UploadController : ControllerBase
    {
        public UploadController(IUploadService uploadService, CurrentUser currentUser)
        {
            _uploadService = uploadService;
            _currentUser = currentUser;
        }

        [HttpPost, Route("upload")]
        public async Task<IActionResult> Upload([FromBody] UploadRequest uploadRequest)
        {
            if (!_currentUser.IsAuthorized)
            {
                return Unauthorized();
            }

            var uploadResult = await _uploadService.Upload(uploadRequest.AppName, uploadRequest.AppBundle, uploadRequest.SFTPHost,
                                                      uploadRequest.SFTPLogin, uploadRequest.SFTPPassword, uploadRequest.SFTPFilePath,
                                                      _currentUser.Id);

            if (uploadResult.Success)
            {
                return Ok(uploadResult);
            }
            else
            {
                return BadRequest(uploadResult);
            };
        }

        [HttpGet, Route("lastUploads")]
        public async Task<IActionResult> LastTenUploads()
        {
            if (!_currentUser.IsAuthorized)
            {
                return Unauthorized();
            }

            var lastTenUploads = await _uploadService.LastTenUploads(_currentUser.Id);
            return Ok(new LastUploadsResponse { Uploads = lastTenUploads });
        }

        private IUploadService _uploadService;
        private CurrentUser _currentUser;
    }
}