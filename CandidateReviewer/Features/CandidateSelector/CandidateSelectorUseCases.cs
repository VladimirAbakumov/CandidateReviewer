using System.Linq;
using System.Threading.Tasks;
using VA.Candidate.Reviewer.Features.CandidateSelector.DataAccess;
using VA.Candidate.Reviewer.Features.Common.Entities;
using VA.Candidate.Reviewer.Frameworks;

namespace VA.Candidate.Reviewer.Features.CandidateSelector
{
  public class CandidateSelectorUseCases : IUseCase<CandidateState>
  {
    private readonly ICandidateSelectionRepository _repository;
    private readonly IAppNavigation _navigation;

    public CandidateSelectorUseCases(ICandidateSelectionRepository repository, IAppNavigation navigation)
    {
      _repository = repository;
      _navigation = navigation;
    }

    public Task ExecuteAction(IView<CandidateState> view, IAction action)
    {
      return action switch
      {
        InitializeAction => Initialize(view),
        SelectTechnologyAction selectTechnologyAction => SelectTechnology(view, selectTechnologyAction),
        ChangeYearsOfExperienceTechnologyAction changeYearsOfExperience => ChangeYearsOfExperience(view, changeYearsOfExperience),
        ApproveAction => ApproveCandidate(view),
        RejectAction => RejectCandidate(view),
        _ => Task.CompletedTask
      };
    }

    private async Task RejectCandidate(IView<CandidateState> view)
    {
      var current = view.State.Current;
      if (current == null)
        return;
      
      view.Update(view.State with { IsLoading = true });
      
      var candidates = await Task.Run(async () =>
      {
        await _repository.SetCandidateRejected(current).ConfigureAwait(false);
        var leftCandidates = view.State.Candidates.ToList();
        
        if (leftCandidates.Contains(current))
          leftCandidates.Remove(current);

        return leftCandidates;
      });
      
      view.Update(view.State with { IsLoading = false, Candidates = candidates, Current = candidates.FirstOrDefault() });
      await _navigation.ShowAlert("Reject", $"{current.FirstName} {current.LastName} has been rejected", "ok", string.Empty);
    }

    private async Task ApproveCandidate(IView<CandidateState> view)
    {
      var current = view.State.Current;
      if (current == null)
        return;
      
      view.Update(view.State with { IsLoading = true });
      
      var candidates = await Task.Run(async () =>
      {
        await _repository.SetCandidateApproved(current).ConfigureAwait(false);
        var leftCandidates = view.State.Candidates.ToList();
        
        if (leftCandidates.Contains(current))
          leftCandidates.Remove(current);

        return leftCandidates;
      });
      
      view.Update(view.State with { IsLoading = false, Candidates = candidates, Current = candidates.FirstOrDefault() });
      await _navigation.ShowAlert("Approve", $"{current.FirstName} {current.LastName} has been approved", "ok", string.Empty);
    }

    private async Task ChangeYearsOfExperience(IView<CandidateState> view, ChangeYearsOfExperienceTechnologyAction changeYearsOfExperience)
    {
      if (changeYearsOfExperience.NewYears == view.State.FilterState.OverYearsOfExperience)
        return;
      
      view.Update(view.State with
      {
        IsLoading = true, FilterState = view.State.FilterState with { OverYearsOfExperience = changeYearsOfExperience.NewYears }
      });

      await ApplyFilter(view);
    }

    private async Task ApplyFilter(IView<CandidateState> view)
    {
      var candidates = await Task.Run(() => _repository.GetCandidates(view.State.FilterState));
      view.Update(view.State with
      {
        Candidates = candidates, Current = candidates.FirstOrDefault(), IsLoading = false
      });
    }

    private async Task SelectTechnology(IView<CandidateState> view, SelectTechnologyAction selectTechnologyAction)
    {
      if (selectTechnologyAction.Technology == view.State.FilterState.Selected)
        return;
      
      view.Update(view.State with
      {
        IsLoading = true, FilterState = view.State.FilterState with { Selected = selectTechnologyAction.Technology }
      });

      await ApplyFilter(view);
    }

    private async Task Initialize(IView<CandidateState> view)
    {
      var loadingState = new CandidateState(new CandidateEntity[0], null, new FilterState(new Technology[0], null, 0), true);
      view.Update(loadingState);

      var loadedState = await Task.Run(async () =>
      {
        var technologies = await _repository.GetTechnologies().ConfigureAwait(false);
        var filterState = new FilterState(technologies, null, 0);
        var candidates = await _repository.GetCandidates(filterState).ConfigureAwait(false);
        return new CandidateState(candidates, candidates.FirstOrDefault(), filterState, false);
      });
      view.Update(loadedState); 
    }
  }
}