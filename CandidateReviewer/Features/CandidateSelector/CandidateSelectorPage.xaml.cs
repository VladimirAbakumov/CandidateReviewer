using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using VA.Candidate.Reviewer.Features.Common.Entities;
using VA.Candidate.Reviewer.Frameworks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace VA.Candidate.Reviewer.Features.CandidateSelector
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class CandidateSelectorPage : IView<CandidateState>
  {
    private readonly IUseCase<CandidateState> _useCase;
    private IReadOnlyList<Technology> _technologies;
    
    public CandidateSelectorPage()
    {
      InitializeComponent();
      State = new CandidateState(new CandidateEntity[0], null, new FilterState(new Technology[0], null, 0), false);
      _useCase = App.Container.Resolve<CandidateSelectorUseCases>();
      _useCase.ExecuteAction(this, new InitializeAction());
      _technologies = new List<Technology>();
    }

    public CandidateState State { get; private set; }
    
    public void Update(CandidateState state)
    {
      State = state;

      if (!ReferenceEquals(_technologies, state.FilterState.Technologies))
      {
        _technologies = state.FilterState.Technologies;
        TechnologyPicker.ItemsSource = state.FilterState.Technologies.Select(t => t.Name).ToList();
      }

      TechnologyPicker.SelectedIndex =
        state.FilterState.Selected != null ? _technologies.IndexOf(state.FilterState.Selected) : -1;
      YearsInput.Text = state.FilterState.OverYearsOfExperience.ToString();

      if (state.Current is null)
      {
        EmptyLabel.IsVisible = true;
        ProfileImage.IsVisible = CandidateName.IsVisible = CandidateInfo.IsVisible = false;
        return;
      }
      
      EmptyLabel.IsVisible = false;
      ProfileImage.IsVisible = CandidateName.IsVisible = CandidateInfo.IsVisible = true;

      ProfileImage.Source = state.Current.ProfilePicture;
      CandidateName.Text = $"{state.Current.FirstName} {state.Current.LastName}";

      CandidateInfo.Text = "";
      foreach (var (t, y) in state.Current.Experiences)
      {
        CandidateInfo.Text += $"{t.Name} ({y}) ";
      }
    }

    private void OnRejecting(object sender, SwipedEventArgs e) => _useCase.ExecuteAction(this, new RejectAction());
    
    private void OnApproving(object sender, SwipedEventArgs e) => _useCase.ExecuteAction(this, new ApproveAction());

    private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
      _useCase.ExecuteAction(this, new SelectTechnologyAction(_technologies[TechnologyPicker.SelectedIndex]));
    }

    private void OnYearsTextChanged(object sender, TextChangedEventArgs e)
    {
      if (int.TryParse(e.NewTextValue, out var newValue))
        _useCase.ExecuteAction(this, new ChangeYearsOfExperienceTechnologyAction(newValue));
      else if (e.NewTextValue != string.Empty)
        YearsInput.Text = e.OldTextValue;
    }

    private void OnYearsUnfocused(object sender, FocusEventArgs e)
    {
      if (YearsInput.Text == string.Empty)
        YearsInput.Text = "0";
    }
  }
}