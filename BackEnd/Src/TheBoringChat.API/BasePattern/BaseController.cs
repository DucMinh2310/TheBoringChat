namespace TheBoringChat.BasePattern;

[CustomRoute]
[ApiController]
public abstract class BaseController<TRepos> : ControllerBase
{
    protected readonly ILogger<BaseController<TRepos>> _logger;
    protected readonly TRepos _repos;
    protected readonly IMapper _mapper;
    protected readonly ApplicationUser _applicationUser;

    protected BaseController(
        TRepos repos,
        ILogger<BaseController<TRepos>> logger,
        ApplicationUser applicationUser,
        IMapper mapper)
    {
        _logger = logger;
        _repos = repos;
        _mapper = mapper;
        _applicationUser = applicationUser;
    }
}

[CustomRoute]
[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger _logger;
    protected readonly IMapper _mapper;
    protected readonly ApplicationUser _applicationUser;

    protected BaseController(
        ILogger<BaseController> logger,
        ApplicationUser applicationUser,
        IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
        _applicationUser = applicationUser;
    }
}
