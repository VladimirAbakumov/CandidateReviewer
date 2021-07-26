using System.Collections.Generic;
using VA.Candidate.Reviewer.Features.Common.DataAccess;
using VA.Candidate.Reviewer.Features.Common.Entities;

namespace VA.Candidate.Reviewer.Features.ApprovedCandidateList
{
  public interface IApprovedCandidatesRepository
  {
    IReadOnlyList<CandidateEntity> Get();
  }

  public class ApprovedCandidatesRepository : IApprovedCandidatesRepository
  {
    private readonly IDbStorage _db;

    public ApprovedCandidatesRepository(IDbStorage db) => _db = db;

    public IReadOnlyList<CandidateEntity> Get() => _db.GetApprovedCandidates();
  }
}