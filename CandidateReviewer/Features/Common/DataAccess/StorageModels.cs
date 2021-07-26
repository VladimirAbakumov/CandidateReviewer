using SQLite;

#nullable disable

namespace VA.Candidate.Reviewer.Features.Common.DataAccess
{
  public class CandidateStorageModel
  {
    [PrimaryKey]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePicture { get; set; }
    public string Email { get; set; }

    public CandidateStorageModel(string id, string firstName, string lastName, string profilePicture, string email)
    {
      Id = id;
      FirstName = firstName;
      LastName = lastName;
      ProfilePicture = profilePicture;
      Email = email;
    }

    public CandidateStorageModel()
    {
    }
  }
  
  public class ExperienceStorageModel
  {
    [PrimaryKey]
    public string Id { get; set; }
    public string CandidateId { get; set; }
    public string TechnologyId { get; set; }
    public int Years { get; set; }

    public ExperienceStorageModel()
    {
    }

    public ExperienceStorageModel(string candidateId, string technologyId, int years)
    {
      Id = $"{candidateId}-{technologyId}";
      CandidateId = candidateId;
      TechnologyId = technologyId;
      Years = years;
    }
  }
  
  public class TechnologyStorageModel
  {
    [PrimaryKey]
    public string TechnologyId { get; set; }
    public string Name { get; set; }

    public TechnologyStorageModel(string technologyId, string name)
    {
      TechnologyId = technologyId;
      Name = name;
    }

    public TechnologyStorageModel()
    {
    }
  }
  
  public class ApprovalStorageModel
  {
    [PrimaryKey]
    public string Id { get; set; }
    public bool IsApproved { get; set; }

    public ApprovalStorageModel(string candidateId, bool isApproved)
    {
      Id = candidateId;
      IsApproved = isApproved;
    }

    public ApprovalStorageModel()
    {
    }
  }
}