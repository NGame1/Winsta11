using InstagramApiSharp.Classes.Models;
using Mapster;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Media
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class CoAuthorUserProviderUC : StackPanel
    {
        public static readonly DependencyProperty NavigateToUserCommandProperty = DependencyProperty.Register(
          nameof(NavigateToUserCommand),
          typeof(RelayCommand),
          typeof(CoAuthorUserProviderUC),
          new PropertyMetadata(null));

        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          nameof(Media),
          typeof(InstaMedia),
          typeof(CoAuthorUserProviderUC),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnMediaChanged))]
        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        public RelayCommand<InstaUser> NavigateToUserCommand { get; set; }

        public CoAuthorUserProviderUC()
        {
            this.InitializeComponent();
        }

        void OnMediaChanged()
        {
            this.Children.Add(new HyperlinkButton()
            {
                CommandParameter = Media.User,
                Command = NavigateToUserCommand,
                Content = Media.User.UserName,
                FontWeight = FontWeights.SemiBold,
                Margin = new(0, 0, 5, 0),
                Padding = new(0)
            });
            this.Children.Add(new TextBlock()
            {
                Text = "and",
                Margin = new(0, 0, 5, 0)
            });
            for (int i = 0; i < Media.CoAuthorsProducers.Count; i++)
            {
                var coAuthor = Media.CoAuthorsProducers.ElementAtOrDefault(i);
                this.Children.Add(new HyperlinkButton()
                {
                    CommandParameter = coAuthor.Adapt<InstaUser>(),
                    Command = NavigateToUserCommand,
                    Content = coAuthor.UserName,
                    FontWeight = FontWeights.SemiBold,
                    Margin = new(0, 0, 5, 0),
                    Padding = new(0)
                });
            }
        }
    }
}
