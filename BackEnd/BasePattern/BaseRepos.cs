namespace BackEnd.BasePattern;

public abstract class BaseRepos
{
    protected readonly ILogger<BaseRepos> _logger;
    protected readonly EFContext _ef;

    protected BaseRepos(EFContext ef, ILogger<BaseRepos> logger)
    {
        _logger = logger;
        _ef = ef;
    }
}
