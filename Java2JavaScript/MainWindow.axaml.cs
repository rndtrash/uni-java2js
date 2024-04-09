using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Java2JavaScript;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    static readonly IReadOnlyList<FilePickerFileType> javaFileTypes =
        new[]
        {
            new FilePickerFileType("Исходный код Java")
                { Patterns = new[] { "*.java" }, MimeTypes = new[] { "text/plain" } }
        };

    private async void OpenFile(object? sender, RoutedEventArgs e)
    {
        // Start async operation to open the dialog.
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Открыть файл Java",
            AllowMultiple = false,
            FileTypeFilter = javaFileTypes
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            // Reads all the content of file as a text.
            var fileContent = await streamReader.ReadToEndAsync();
            SourceTextBox.Text = fileContent;
        }
    }

    private void SourceTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ProcessedTextBox.IsEnabled = false;
    }

    private void Tokenize(object? sender, RoutedEventArgs e)
    {
        var tokenizer = new JavaTokenizer();
        var tokens = tokenizer.Tokenize(SourceTextBox.Text ?? "");
        ProcessedTextBox.Text = string.Join(' ', tokens);
        ProcessedTextBox.IsEnabled = true;
    }
}