using System.Threading.Tasks;
using VA.Candidate.Reviewer.Features.CandidateSelector.DataAccess;
using VA.Candidate.Reviewer.Features.Common.DataAccess;
using VA.Candidate.Reviewer.Frameworks;

namespace VA.Candidate.Reviewer
{
  public class MainUseCases
  {
    private IDbConnection _connection;
    private ICandidateSelectionRepository _repository;
    private IAppNavigation _navigation;

    public MainUseCases(ICandidateSelectionRepository repository, IAppNavigation navigation, IDbConnection connection)
    {
      _repository = repository;
      _navigation = navigation;
      _connection = connection;
    }

    public async void OnStart()
    {
      await Task.Run(async () =>
      {
        _connection.Initialize();
        await _repository.Initialize();
      });
      _navigation.SetAsRoot<MainPage>();
    }
  }
}