using System;
using System.Collections.Generic;
using System.Linq;
using VA.Candidate.Reviewer.Features.Common.Entities;

namespace VA.Candidate.Reviewer.Features.Common.DataAccess
{
  public interface IDbStorage
  {
    IReadOnlyList<Technology> GetTechnologies();

    void SaveTechnologies(IReadOnlyList<Technology> technologies);
    
    IReadOnlyList<CandidateEntity> GetNotReviewedCandidates();

    void SaveCandidates(IReadOnlyList<CandidateEntity> candidates);

    void SetCandidateApproved(CandidateEntity candidate);
    
    void SetCandidateRejected(CandidateEntity candidate);
    
    IReadOnlyList<CandidateEntity> GetApprovedCandidates();
  }

  public class DbStorage : IDbStorage
  {
    private readonly IDbConnection _get;

    public DbStorage(IDbConnection get) => _get = get;

    public IReadOnlyList<Technology> GetTechnologies() =>
      _get.Connection.Table<TechnologyStorageModel>()
        .Select(t => new Technology(Guid.Parse(t.TechnologyId), t.Name))
        .ToList();

    public void SaveTechnologies(IReadOnlyList<Technology> technologies)
    {
      lock (_get)
      {
        _get.Connection.DeleteAll<TechnologyStorageModel>();
        _get.Connection.InsertAll(technologies.Select(t => new TechnologyStorageModel(t.Id.ToString(), t.Name)));
      }
    }

    public void SaveCandidates(IReadOnlyList<CandidateEntity> candidates)
    {
      lock (_get)
      {
        _get.Connection.BeginTransaction();
        try
        {
          _get.Connection.DeleteAll<CandidateStorageModel>();
          _get.Connection.InsertAll(candidates.Select(c =>
            new CandidateStorageModel(c.Id.ToString(), c.FirstName, c.LastName, c.ProfilePicture, c.Email)));
          
          foreach (var c in candidates)
          {
            foreach (var e in c.Experiences)
            {
              _get.Connection.InsertOrReplace(new ExperienceStorageModel(c.Id.ToString(),
                e.Technology.Id.ToString(),
                e.Years));
            }
          }
        }
        finally
        {
          _get.Connection.Commit();
        }
      }
    }

    public void SetCandidateApproved(CandidateEntity candidate)
    {
      lock (_get)
      {
        _get.Connection.InsertOrReplace(new ApprovalStorageModel(candidate.Id.ToString(), true));
      }
    }

    public void SetCandidateRejected(CandidateEntity candidate)
    {
      lock (_get)
      {
        _get.Connection.InsertOrReplace(new ApprovalStorageModel(candidate.Id.ToString(), false));
      }
    }

    public IReadOnlyList<CandidateEntity> GetNotReviewedCandidates() => GetCandidates((c, a) => !a.ContainsKey(c.Id));

    public IReadOnlyList<CandidateEntity> GetApprovedCandidates() =>
      GetCandidates((c, a) => a.ContainsKey(c.Id) && a[c.Id].IsApproved);

    private IReadOnlyList<CandidateEntity> GetCandidates(Func<CandidateStorageModel, IReadOnlyDictionary<string, ApprovalStorageModel>, bool> predicate)
    {
      var technologies = GetTechnologies().ToDictionary(t => t.Id);
      var approvals = _get.Connection.Table<ApprovalStorageModel>().ToDictionary(t => t.Id);
      var candidateSms = _get.Connection.Table<CandidateStorageModel>()
        .ToList()
        .Where(c => predicate(c, approvals))
        .ToList();

      var result = from c in candidateSms
        let exp = _get.Connection.Table<ExperienceStorageModel>()
          .Where(e => e.CandidateId == c.Id)
          .Select(e => new Experience(technologies[Guid.Parse(e.TechnologyId)], e.Years))
          .ToList()
        select new CandidateEntity(Guid.Parse(c.Id), c.FirstName, c.LastName, c.ProfilePicture, c.Email, exp);
      return result.ToList();
    }
  }
}