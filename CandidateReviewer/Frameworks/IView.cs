namespace VA.Candidate.Reviewer.Frameworks
{
  public interface IView<TState>
  {
    TState State { get; }
    
    void Update(TState state);
  }
}