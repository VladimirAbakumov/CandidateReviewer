using DryIoc;
using VA.Candidate.Reviewer.Frameworks;
using Xamarin.Forms.Xaml;

namespace VA.Candidate.Reviewer.Features.ApprovedCandidateList
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ApprovedCandidatesPage : IOnActivatedHandler
  {
    private readonly ApprovedCandidatesViewModel _viewModel;

    public ApprovedCandidatesPage()
    {
      InitializeComponent();
      _viewModel = App.Container.Resolve<ApprovedCandidatesViewModel>();
      BindingContext = _viewModel;
    }

    public void OnActivated() => _viewModel.OnActivated();
  }
}