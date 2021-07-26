using System.Collections.Generic;
using System.Linq;
using MvvmHelpers;
using NetDiff;
using VA.Candidate.Reviewer.Features.Common.Entities;
using VA.Candidate.Reviewer.Frameworks;

namespace VA.Candidate.Reviewer.Features.ApprovedCandidateList
{
  public sealed record CandidatePresentationModel(CandidateEntity Entity)
  {
    public string FullName => $"{Entity.FirstName} {Entity.LastName} ({Entity.Email})";
    public string Experience => Entity.Experiences.Aggregate("", (c, n) => c + " " + $"{n.Technology.Name} ({n.Years})  ");
    public string ProfilePicture => Entity.ProfilePicture;
  }
  
  public class ApprovedCandidatesViewModel : IView<ApprovedCandidateListState>
  {
    private readonly IUseCase<ApprovedCandidateListState> _useCase;
    private IReadOnlyList<CandidateEntity> _presentedList;

    public ApprovedCandidatesViewModel(ApprovedCandidatesUseCases useCase)
    {
      _useCase = useCase;
      _presentedList = new CandidateEntity[0];
      State = new ApprovedCandidateListState(new CandidateEntity[0]);
      Candidates = new ObservableRangeCollection<CandidatePresentationModel>();

      _useCase.ExecuteAction(this, new LoadAction());
    }

    public ApprovedCandidateListState State { get; private set; }
    
    public ObservableRangeCollection<CandidatePresentationModel> Candidates { get; private set; }
    
    public void Update(ApprovedCandidateListState state)
    {
      State = state;
      
      if (ReferenceEquals(_presentedList, state.Candidates))
        return;

      _presentedList = state.Candidates;
      Merge(state.Candidates.Select(c => new CandidatePresentationModel(c)).ToList());
    }

    private void Merge(IReadOnlyCollection<CandidatePresentationModel> newCandidates)
    {
      if (Candidates.Count == 0 || newCandidates.Count == 0)
      {
        Candidates.ReplaceRange(newCandidates);
        return;
      }

      var nonOptimizedDiff = DiffUtil.Diff(Candidates, newCandidates);
      var diff = DiffUtil.OptimizeCaseDeletedFirst(DiffUtil.OptimizeCaseInsertedFirst(nonOptimizedDiff)).ToArray();

      for (var i = 0; i < diff.Length; i++)
      {
        if (diff[i].Status == DiffStatus.Inserted)
        {
          if (i >= Candidates.Count)
            Candidates.Add(diff[i].Obj2);
          else
            Candidates.Insert(i, diff[i].Obj2);

          continue;
        }

        if (diff[i].Status == DiffStatus.Modified)
        {
          var index = Candidates.IndexOf(diff[i].Obj1);
          if (index == -1 || index >= Candidates.Count)
            Candidates.Add(diff[i].Obj2);
          Candidates[index] = diff[i].Obj2;
        }

        if(diff[i].Status == DiffStatus.Deleted)
        {
          Candidates.Remove(diff[i].Obj1);
        }
      }
    }

    public void OnActivated() => _useCase.ExecuteAction(this, new LoadAction());
  }
}