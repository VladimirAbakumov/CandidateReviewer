using System;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace VA.Candidate.Reviewer.Frameworks
{
  public interface IAppNavigation
  {
    TNavigationElement SetAsRoot<TNavigationElement>() where TNavigationElement : class;
    
    Task<TNavigationElement> NavigateTo<TNavigationElement>() where TNavigationElement : class;
    
    Task<string> ShowActionSheet(string title, string? destruction, params string[] actions);
    
    Task<bool> ShowAlert(string title, string message, string ok, string cancel);
  }

  public class Navigation : IAppNavigation
  {
    private INavigation XamarinFormsNav => Application.Current.MainPage.Navigation;

    public TNavigationElement SetAsRoot<TNavigationElement>() where TNavigationElement : class
    {
      var page = CreatePage<TNavigationElement>();
      Application.Current.MainPage = page;
      return (page as TNavigationElement)!;
    }

    private static Page CreatePage<TNavigationElement>()
    {
      var page = App.Container.Resolve<TNavigationElement>();
      if (page is not Page p)
        throw new NotSupportedException("Navigation element must be a page type");
      return p;
    }

    public async Task<TNavigationElement> NavigateTo<TNavigationElement>() where TNavigationElement : class
    {
      var page = CreatePage<TNavigationElement>();
      await XamarinFormsNav.PushAsync(page);
      return (page as TNavigationElement)!;
    }

    public Task<string> ShowActionSheet(string title, string? destruction, params string[] actions)
    {
      var page = XamarinFormsNav.ModalStack.LastOrDefault() ?? XamarinFormsNav.NavigationStack.LastOrDefault();
      return page == null ? Task.FromResult("Cancel") : page.DisplayActionSheet(title, "Cancel", destruction, actions);
    }
    
    public Task<bool> ShowAlert(string title, string message, string ok, string cancel)
    {
      var page = XamarinFormsNav.ModalStack.LastOrDefault() ?? XamarinFormsNav.NavigationStack.LastOrDefault();

      return page == null ? Task.FromResult(false) : page.DisplayAlert(title, message, ok, cancel);
    }
  }
}