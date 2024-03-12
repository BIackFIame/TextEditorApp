using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using Avalonia.Markup.Xaml;
using System.Collections;
using Avalonia.LogicalTree;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace TextEditorAp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            OpenNewDocument();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            if (tabControl?.SelectedItem is TabItem selectedTab && selectedTab.Content is TextBox textBox)
            {
                if (selectedTab.Tag is string filePath && !string.IsNullOrEmpty(filePath))
                {
                    await File.WriteAllTextAsync(filePath, textBox.Text);
                }
                else
                {
                    var result = await this.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions());
                    if (result != null)
                    {
                        await File.WriteAllTextAsync(result.Path.LocalPath, textBox.Text);
                        selectedTab.Header = Path.GetFileName(result.Path.LocalPath);
                        selectedTab.Tag = result.Path.LocalPath;
                    }
                }
            }
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions());
            if (result != null && result.Count > 0)
            {
                var text = await File.ReadAllTextAsync(result[0].Path.LocalPath);
                OpenNewDocument(text, result[0].Path.LocalPath);
            }
        }

        private void OpenNewDocument(string text = "", string? filePath = null)
        {
            var textBox = new TextBox { AcceptsReturn = true, AcceptsTab = true, Text = text };
            var header = string.IsNullOrEmpty(filePath) ? "New Document" : Path.GetFileName(filePath);
            var tabItem = new TabItem { Header = header, Content = textBox, Tag = filePath };
            var tabControl = this.FindControl<TabControl>("DocumentsTab");
            if(tabControl != null)
            {            
                tabControl.Items.Add(tabItem);
                tabControl.SelectedItem = tabItem;
            }
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
{
    if (sender is Button button && button.TemplatedParent is TabItem tabItem)
    {
        var tabControl = this.FindControl<TabControl>("DocumentsTab");
        if (tabControl != null)
        {
            int index = tabControl.Items.IndexOf(tabItem);
            if (index >= 0)
            {
                tabControl.Items.RemoveAt(index);
            }
        }
    }
}
    }
}