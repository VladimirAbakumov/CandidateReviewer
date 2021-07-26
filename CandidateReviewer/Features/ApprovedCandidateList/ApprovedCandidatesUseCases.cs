using System.Threading.Tasks;
using VA.Candidate.Reviewer.Frameworks;

namespace VA.Candidate.Reviewer.Features.ApprovedCandidateList
{
  public class ApprovedCandidatesUseCases : IUseCase<ApprovedCandidateListState>
  {
    private readonly IApprovedCandidatesRepository _repository;

    public ApprovedCandidatesUseCases(IApprovedCandidatesRepository repository) => _repository = repository;

    public Task ExecuteAction(IView<ApprovedCandidateListState> view, IAction action)
    {
      return action switch
      {
        LoadAction => LoadList(view),
        _ => Task.CompletedTask
      };
    }

    private async Task LoadList(IView<ApprovedCandidateListState> view)
    {
      var approvedCandidates = await Task.Run(() => _repository.Get());
      view.Update(view.State with { Candidates = approvedCandidates });
    }
  }
}