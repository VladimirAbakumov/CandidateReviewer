using System.Collections.Generic;
using VA.Candidate.Reviewer.Features.Common.Entities;

namespace VA.Candidate.Reviewer.Features.ApprovedCandidateList
{
  public sealed record ApprovedCandidateListState(IReadOnlyList<CandidateEntity> Candidates);
}