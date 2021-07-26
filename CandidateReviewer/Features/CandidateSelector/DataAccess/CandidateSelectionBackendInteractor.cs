using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VA.Candidate.Reviewer.Features.Common.DataAccess;
using VA.Candidate.Reviewer.Features.Common.Entities;
using VA.Candidate.Reviewer.Frameworks;

namespace VA.Candidate.Reviewer.Features.CandidateSelector.DataAccess
{
  public interface ICandidateSelectionBackendInteractor
  {
    Task<IReadOnlyList<Technology>> GetTechnologies();
    
    Task<IReadOnlyList<CandidateEntity>> GetCandidates(IReadOnlyList<Technology> readOnlyList);
  }

  public class CandidateSelectionBackendInteractor : ICandidateSelectionBackendInteractor
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public CandidateSelectionBackendInteractor(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    public async Task<IReadOnlyList<Technology>> GetTechnologies()
    {
      var client = _httpClientFactory.Get();
      try
      {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, "https://app.ifs.aero/EternalBlue/api/technologies");
        var responseMsg = await client.SendAsync(requestMsg).ConfigureAwait(false);
        if (!responseMsg.IsSuccessStatusCode)
          return new List<Technology>(0);
        
        var responseString = await responseMsg.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = responseString != null
          ? JsonConvert.DeserializeObject<TechnologyDto[]>(responseString)?.Select(t => new Technology(t.Guid, t.Name)).ToArray()
          : new Technology[0];
        return result!;
      }
      finally
      {
        _httpClientFactory.Release(client);        
      }
    }

    public async Task<IReadOnlyList<CandidateEntity>> GetCandidates(IReadOnlyList<Technology> readOnlyList)
    {
      var technology = readOnlyList.ToDictionary(t => t.Id);
      
      var client = _httpClientFactory.Get();
      try
      {
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, "https://app.ifs.aero/EternalBlue/api/candidates");
        var responseMsg = await client.SendAsync(requestMsg).ConfigureAwait(false);
        if (!responseMsg.IsSuccessStatusCode)
          return new List<CandidateEntity>(0);
        
        var responseString = await responseMsg.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = responseString != null
          ? JsonConvert.DeserializeObject<CandidateDto[]>(responseString)?
            .Select(c => new CandidateEntity(c.CandidateId,
              c.FirstName,
              c.LastName,
              c.ProfilePicture,
              c.Email,
              c.Experience.Select(e => new Experience(technology[e.TechnologyId], e.YearsOfExperience)).ToList())).ToArray()
          : new CandidateEntity[0];
        return result!;
      }
      finally
      {
        _httpClientFactory.Release(client);        
      }
    }
  }
}