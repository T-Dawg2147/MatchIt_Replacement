using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MatchIt.MatchReplacement.Core.Application;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using FormsDialogResult = System.Windows.Forms.DialogResult;

namespace MatchIt.MatchReplacement.Wpf;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IMatchReplacementService _matchService;
    private readonly IMatchReplacementQueryService _profileQueryService;
    private readonly IResultPreviewService _previewService;

    #region Private Fields for Properties

    private ProcessProfileDescriptor? _selectedProfile;
    private ExportFormat _selectedExportFormat;
    private string _contactsCsvPath = string.Empty;
    private string _suppressionCsvPath = string.Empty;
    private string _outputFolder = string.Empty;
    private string _progressLog = string.Empty;
    private string _statusText = "Ready";
    private UiOutputFile? _selectedOutputFile;

    #endregion

    public MainWindowViewModel()
    {
        _matchService = new MatchReplacementService();
        _profileQueryService = new DefaultMatchReplacementQueryService();
        _previewService = new ResultPreviewService();

        BrowseContactsCommand = new RelayCommand(BrowseContacts);
        BrowseSuppressionCommand = new RelayCommand(BrowseSuppression);
        BrowseOutputFolderCommand = new RelayCommand(BrowseOutputFolder);
        RunProcessCommand = new RelayCommand(async () => await RunProcessAsync());
        PreviewSelectedOutputCommand = new RelayCommand(async () => await PreviewSelectedOutputAsync(), () => SelectedOutputFile is not null);

        Profiles = new ObservableCollection<ProcessProfileDescriptor>();
        ExportFormats = new ObservableCollection<ExportFormat>(
            Enum.GetValues<ExportFormat>());

        OutputFiles = new ObservableCollection<UiOutputFile>();
        PreviewRows = new ObservableCollection<Dictionary<string, string?>>();

        _selectedExportFormat = ExportFormat.Csv;

        _ = LoadProfilesAsync();
    }

    #region Events and Commands

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<ProcessProfileDescriptor> Profiles { get; }
    public ObservableCollection<ExportFormat> ExportFormats { get; }
    public ObservableCollection<UiOutputFile> OutputFiles { get; }
    public ObservableCollection<Dictionary<string, string?>> PreviewRows { get; }

    public RelayCommand BrowseContactsCommand { get; }
    public RelayCommand BrowseSuppressionCommand { get; }
    public RelayCommand BrowseOutputFolderCommand { get; }
    public RelayCommand RunProcessCommand { get; }
    public RelayCommand PreviewSelectedOutputCommand { get; }

    #endregion

    #region  Properties

    public ProcessProfileDescriptor? SelectedProfile
    {
        get => _selectedProfile;
        set => SetProperty(ref _selectedProfile, value);
    }

    public ExportFormat SelectedExportFormat
    {
        get => _selectedExportFormat;
        set => SetProperty(ref _selectedExportFormat, value);
    }

    public string ContactsCsvPath
    {
        get => _contactsCsvPath;
        set => SetProperty(ref _contactsCsvPath, value);
    }

    public string SuppressionCsvPath
    {
        get => _suppressionCsvPath;
        set => SetProperty(ref _suppressionCsvPath, value);
    }

    public string OutputFolder
    {
        get => _outputFolder;
        set => SetProperty(ref _outputFolder, value);
    }

    public string ProgressLog
    {
        get => _progressLog;
        set => SetProperty(ref _progressLog, value);
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public UiOutputFile? SelectedOutputFile
    {
        get => _selectedOutputFile;
        set
        {
            if (SetProperty(ref _selectedOutputFile, value))
            {
                PreviewSelectedOutputCommand.RaiseCanExecuteChanged();
            }
        }
    }
    
    #endregion

    private async Task LoadProfilesAsync()
    {
        try
        {
            var response = await _profileQueryService.GetProfilesAsync();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Profiles.Clear();
                foreach (var profile in response.Profiles)
                {
                    Profiles.Add(profile);
                }

                SelectedProfile = Profiles.FirstOrDefault();
            });
        }
        catch (Exception ex)
        {
            AppendLog($"Failed to load profiles: {ex.Message}");
        }
    }

    private void BrowseContacts()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Data Files|*.csv;*.txt|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            ContactsCsvPath = dialog.FileName;
        }
    }

    private void BrowseSuppression()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog() 
        {
            Filter = "Data Files|*.csv;*.txt|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            SuppressionCsvPath = dialog.FileName;
        }
    }

    private void BrowseOutputFolder()
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == FormsDialogResult.OK)
        {
            OutputFolder = dialog.SelectedPath;
        }
    }

    private async Task RunProcessAsync()
    {
        if (SelectedProfile is null)
        {
            System.Windows.MessageBox.Show("Please select a process profile.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ContactsCsvPath) ||
            string.IsNullOrWhiteSpace(SuppressionCsvPath) ||
            string.IsNullOrWhiteSpace(OutputFolder))
        {
            System.Windows.MessageBox.Show("Please select all required input paths.");
            return;
        }

        try
        {
            StatusText = "Running...";
            ProgressLog = string.Empty;
            OutputFiles.Clear();
            PreviewRows.Clear();

            var request = new RunProcessRequest
            {
                ProcessProfileName = SelectedProfile.Name,
                ContactsCsvPath = ContactsCsvPath,
                SuppressionCsvPath = SuppressionCsvPath,
                OutputFolder = OutputFolder,
                ExportOptions = new ExportOptions
                {
                    Format = SelectedExportFormat,
                    IncludeHeaders = true,
                    QuoteAllFields = true
                },
                ProgressObserver = new DelegatingProcessProgressObserver((update, _) =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        AppendLog($"[{update.StageIndex}/{update.TotalStages}] {update.StageName} - {update.StageStatus} - {update.Message}");
                    });

                    return Task.CompletedTask;
                })
            };

            var response = await _matchService.RunAsync(request);

            foreach (var output in response.OutputFiles)
            {
                OutputFiles.Add(UiOutputFile.From(output));
            }

            StatusText = $"Completed ({response.Status})";
            AppendLog("Run complete.");
        }
        catch (Exception ex)
        {
            StatusText = "Failed";
            AppendLog($"ERROR: {ex}");
            System.Windows.MessageBox.Show(ex.Message, "Run Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task PreviewSelectedOutputAsync()
    {
        if (SelectedOutputFile is null)
        {
            return;
        }

        try
        {
            var preview = await _previewService.PreviewFileAsync(new PreviewFileRequest
            {
                FilePath = SelectedOutputFile.Path,
                Format = SelectedExportFormat,
                HasHeaderRow = true,
                MaxRows = 50
            });

            PreviewRows.Clear();

            foreach (var row in preview.Rows)
            {
                PreviewRows.Add(new Dictionary<string, string?>(row.Values, StringComparer.OrdinalIgnoreCase));
            }

            AppendLog($"Preview loaded for {SelectedOutputFile.Name}");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "Preview Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AppendLog(string message)
    {
        ProgressLog += $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
        OnPropertyChanged(nameof(ProgressLog));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}