using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VA.Candidate.Reviewer.Features.Common.DataAccess;
using VA.Candidate.Reviewer.Features.Common.Entities;

namespace VA.Candidate.Reviewer.Features.CandidateSelector.DataAccess
{
  public interface ICandidateSelectionRepository
  {
    Task Initialize();
    
    Task<IReadOnlyList<Technology>> GetTechnologies();
    
    Task<IReadOnlyList<CandidateEntity>> GetCandidates(FilterState state);
    
    Task SetCandidateApproved(CandidateEntity candidate);
    
    Task SetCandidateRejected(CandidateEntity candidate);
  }

  public class CandidateSelectionRepository : ICandidateSelectionRepository
  {
    private readonly ICandidateSelectionBackendInteractor _backend;
    private readonly IDbStorage _db;

    public CandidateSelectionRepository(ICandidateSelectionBackendInteractor backend, IDbStorage db)
    {
      _backend = backend;
      _db = db;
    }

    public async Task Initialize()
    {
      var technologies = await _backend.GetTechnologies().ConfigureAwait(false);
      _db.SaveTechnologies(technologies);

      var candidates = await _backend.GetCandidates(technologies).ConfigureAwait(false);
      _db.SaveCandidates(candidates);
    }

    public Task<IReadOnlyList<Technology>> GetTechnologies() => Task.FromResult(_db.GetTechnologies());

    public Task<IReadOnlyList<CandidateEntity>> GetCandidates(FilterState state)
    {
      var filtered = _db.GetNotReviewedCandidates().Where(c =>
          c.Experiences.Any(e => (state.Selected is null || e.Technology == state.Selected) && e.Years >= state.OverYearsOfExperience))
        .ToList();
      return Task.FromResult((IReadOnlyList<CandidateEntity>)filtered);
    }

    public Task SetCandidateApproved(CandidateEntity candidate) => Task.Run(() => _db.SetCandidateApproved(candidate));

    public Task SetCandidateRejected(CandidateEntity candidate) => Task.Run(() => _db.SetCandidateRejected(candidate));
  }
}