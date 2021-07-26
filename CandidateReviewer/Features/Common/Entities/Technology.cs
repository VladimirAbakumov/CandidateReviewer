using System;

namespace VA.Candidate.Reviewer.Features.Common.Entities
{
  public sealed record Technology(Guid Id, string Name);
  
  public sealed record Experience(Technology Technology, int Years);
}