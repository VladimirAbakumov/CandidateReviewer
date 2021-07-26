using DryIoc;
using VA.Candidate.Reviewer.Features.ApprovedCandidateList;
using VA.Candidate.Reviewer.Features.CandidateSelector;
using VA.Candidate.Reviewer.Features.CandidateSelector.DataAccess;
using VA.Candidate.Reviewer.Features.Common.DataAccess;
using VA.Candidate.Reviewer.Frameworks;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace VA.Candidate.Reviewer
{
  public partial class App
  {
    public static Container Container { get; } = new(Rules.Default.WithUseInterpretation());
    
    public App()
    {
      InitializeComponent();
      RegisterDependencies();

      MainPage = new SplashPage();
            
      Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }

    private void RegisterDependencies()
    {
      Container.Register<MainUseCases>();
      Container.Register<MainPage>();
      Container.Register<IAppNavigation, Navigation>();
      Container.Register<IDbConnection, DbConnection>(Reuse.Singleton);
      Container.Register<IDbStorage, DbStorage>();
      Container.Register<IHttpClientFactory, HttpClientFactory>(Reuse.Singleton);

      Container.Register<ICandidateSelectionRepository, CandidateSelectionRepository>();
      Container.Register<ICandidateSelectionBackendInteractor, CandidateSelectionBackendInteractor>();
      Container.Register<CandidateSelectorUseCases>();
      
      Container.Register<ApprovedCandidatesUseCases>();
      Container.Register<ApprovedCandidatesViewModel>();
      Container.Register<IApprovedCandidatesRepository, ApprovedCandidatesRepository>();
    }

    protected override void OnStart()
    {
      Container.Resolve<MainUseCases>().OnStart();
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}

namespace System.Runtime.CompilerServices {
  public class IsExternalInit {}
}