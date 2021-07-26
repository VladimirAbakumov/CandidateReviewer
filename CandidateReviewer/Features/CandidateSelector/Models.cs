using System.Collections.Generic;
using VA.Candidate.Reviewer.Features.Common.Entities;
using VA.Candidate.Reviewer.Frameworks;

namespace VA.Candidate.Reviewer.Features.CandidateSelector
{
  public sealed record FilterState
  (
    IReadOnlyList<Technology> Technologies,
    Technology? Selected,
    int OverYearsOfExperience
  );

  public sealed record CandidateState(IReadOnlyList<CandidateEntity> Candidates, CandidateEntity? Current, FilterState FilterState, bool IsLoading);

  public readonly struct InitializeAction : IAction { }
  
  public sealed record SelectTechnologyAction(Technology Technology) : IAction { }

  public sealed record ChangeYearsOfExperienceTechnologyAction(int NewYears) : IAction;
  
  public sealed record ApproveAction : IAction;
  
  public sealed record RejectAction : IAction;
}