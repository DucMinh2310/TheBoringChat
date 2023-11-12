namespace TheBoringChat.Controllers;

public class StaticDataController(ILogger<BaseController> logger, ApplicationUser applicationUser, IMapper mapper) : BaseController(logger, applicationUser, mapper)
{
    [HttpPost(nameof(UploadImage))]
    public async Task<IActionResult> UploadImage(IFormFile file, EnumImageType type)
    {
        if (StaticDataHelper.IsImage(file))
        {
            string path = await StaticDataHelper.WriteFile(file, type);
            return Ok(ResponseResult.Success(path));
        }
        return NotFound();
    }
}
