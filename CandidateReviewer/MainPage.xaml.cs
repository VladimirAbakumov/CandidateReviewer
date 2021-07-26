using System;
using VA.Candidate.Reviewer.Frameworks;
using Xamarin.Forms;

namespace VA.Candidate.Reviewer
{
  public partial class MainPage : TabbedPage
  {
    public MainPage()
    {
      InitializeComponent();
    }

    private void OnCurrentPageChanged(object sender, EventArgs e)
    {
      if (CurrentPage is IOnActivatedHandler onActivatedHandler)
        onActivatedHandler.OnActivated();
    }
  }
}