using System.Threading.Tasks;

namespace VA.Candidate.Reviewer.Frameworks
{
  public interface IUseCase<TState>
  {
    Task ExecuteAction(IView<TState> view, IAction action);
  }
}