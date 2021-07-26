using System;

namespace VA.Candidate.Reviewer.Features.Common.DataAccess
{
  public class TechnologyDto
  {
    public Guid Guid { get; set; }
    public string Name { get; set; }
  }
  
  public class CandidateDto
  {
    public Guid CandidateId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePicture { get; set; }
    public string Email { get; set; }
    public ExperienceDto[] Experience { get; set; }
  }

  public class ExperienceDto
  {
    public Guid TechnologyId { get; set; }
    public int YearsOfExperience { get; set; }
  }
}