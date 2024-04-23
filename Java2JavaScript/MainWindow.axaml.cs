using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Java2JavaScript;

public partial class MainWindow : Window
{
    private JavaTokenizer _javaTokenizer = new();

    static readonly IReadOnlyList<FilePickerFileType> javaFileTypes =
        new[]
        {
            new FilePickerFileType("Исходный код Java")
                { Patterns = new[] { "*.java" }, MimeTypes = new[] { "text/plain" } }
        };

    public MainWindow()
    {
        InitializeComponent();

        TokenTypeComboBox.SelectionChanged += (sender, args) => UpdateTokenTable();
        UpdateTokenTable();
    }

    private void UpdateTokenTable()
    {
        List<string> source;
        switch (TokenTypeComboBox.SelectedIndex)
        {
            // W
            case 0:
                source = JavaTokenizer.ReservedWords.ToList();
                break;

            // R
            case 1:
                source = JavaTokenizer.Separators.ToList();
                break;

            // O
            case 2:
                source = JavaTokenizer.Operations.ToList();
                break;

            // N
            case 3:
                source = _javaTokenizer.NumericConstants;
                break;

            // C
            case 4:
                source = _javaTokenizer.LiteralConstants;
                break;

            default:
                return;
        }

        var data = source
            .Select((token, index) => $"{token}\t{index + 1}");
        TokenListTextBox.Text = string.Join("\n", data);
    }

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
        _javaTokenizer = new JavaTokenizer();
        var tokens = _javaTokenizer.Tokenize(SourceTextBox.Text ?? "");
        ProcessedTextBox.Text = string.Join(' ', tokens);
        ProcessedTextBox.IsEnabled = true;
        UpdateTokenTable();
    }
}