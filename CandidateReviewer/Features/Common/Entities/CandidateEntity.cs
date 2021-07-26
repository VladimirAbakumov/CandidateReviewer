using System;
using System.Collections.Generic;

namespace VA.Candidate.Reviewer.Features.Common.Entities
{
  public sealed record CandidateEntity(Guid Id,
    string FirstName,
    string LastName,
    string ProfilePicture,
    string Email,
    IReadOnlyList<Experience> Experiences);
}