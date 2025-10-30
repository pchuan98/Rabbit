### Getting Shell Extensibility Helper (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Retrieves the `ShellExtensibility` object from the `VisualStudioExtensibility` instance, providing access to methods for interacting with the Visual Studio shell, such as displaying user prompts.

```csharp
var shell = this.Extensibility.Shell();
```

--------------------------------

### Showing Error Confirmation Prompt with Custom Title (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Shows how to display a confirmation prompt with a single "OK" button and a default error icon using `PromptOptions.ErrorConfirm`. The example also demonstrates customizing the prompt's title using the `with` expression.

```csharp
if (string.IsNullOrEmpty(projectName))
{
    this.logger.TraceInformation("User did not provide project name.");

    // Show a confirmation prompt (one 'OK' button) with error icon and title.
    await shell.ShowPromptAsync(
        "Project name is required to proceed. Exiting the configuration process.",
        PromptOptions.ErrorConfirm with { Title = Title },
        cancellationToken);

    return;
}
```

--------------------------------

### Executing a Project Query in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/ProjectQueryAPIBrowser.md

Demonstrates how to build and execute a query to retrieve information about projects and their files using the Project Query API. It selects project name, path, files (with various file properties), and project GUID.

```csharp
var result = await queryableSpace.Projects
         .With(project => project.Name)
         .With(project => project.Path)
         .With(project => project.Files
            .With(file => file.ItemType)
            .With(file => file.ItemName)
            .With(file => file.Path)
            .With(file => file.LinkPath)
            .With(file => file.VisualPath))
         .With(project => project.Guid)
.ExecuteQueryAsync();
```

--------------------------------

### Querying Output Groups By Project (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Provides an example of querying information about output groups for all projects. It retrieves project names, active configuration names, and the names of output groups associated with those configurations.

```csharp
var result = await querySpace.QueryProjectsAsync(
	project => project.With(p => p.Name)
		.With(p => p.ActiveConfigurations
		.With(c => c.Name)
		.With(c => c.OutputGroups.With(g => g.Name))),
	cancellationToken);
```

--------------------------------

### Getting Active Text View (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/InsertGuid/README.md

Retrieves the currently active text view asynchronously using `context.GetActiveTextViewAsync` within the `ExecuteCommandAsync` method, providing access to the document, version, and selection state at the time of command execution.

```csharp
using var textView = await context.GetActiveTextViewAsync(cancellationToken);
```

--------------------------------

### Defining InsertGuidCommand Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/InsertGuid/README.md

Defines the `InsertGuidCommand` class, inheriting from `Command` and marked with the `[VisualStudioContribution]` attribute to register it with Visual Studio using its full type name as a unique identifier.

```csharp
[VisualStudioContribution]
internal class InsertGuidCommand : Command
{
```

--------------------------------

### Preparing to Edit Document in Visual Studio (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/EncodeDecodeBase64/README.md

This code shows the process of getting a snapshot of the text document and then initiating an edit operation using `Extensibility.Editor().EditAsync`. The `EditAsync` method takes a batch action where modifications can be applied to an editable version of the document obtained via `textView.Document.AsEditable(batch)`.

```csharp
ITextViewSnapshot textDocumentEditor = await textView.GetTextDocumentAsync(cancellationToken);
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        ITextDocumentEditor textDocumentEditor = textView.Document.AsEditable(batch);
        // [...]
    },
    cancellationToken);
```

--------------------------------

### Configuring InsertGuidCommand (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/InsertGuid/README.md

Configures the command's properties using the `CommandConfiguration` property, setting its display name, placement in the Extensions menu, icon using `ImageMoniker.KnownValues.OfficeWebExtension`, and visibility constraint to be active when any editor is open.

```csharp
public override CommandConfiguration CommandConfiguration => new("%InsertGuid.InsertGuidCommand.DisplayName%")
{
    Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
    Icon = new(ImageMoniker.KnownValues.OfficeWebExtension, IconSettings.    IconAndText),
    VisibleWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorContentType, ".+"),
};
```

--------------------------------

### Creating a File using Project Query API in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/ProjectQueryAPIBrowser.md

Shows how to perform an update operation using the Project Query API. This specific example filters for a project named 'ConsoleApp1', makes it updatable, and then creates a new file within that project.

```csharp
var result = await queryableSpace.Projects
    .Where(project => project.Name == "ConsoleApp1")
.AsUpdatable()
.CreateFile("FileName")
.ExecuteAsync();
```

--------------------------------

### Getting Queryable Space in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/ProjectQueryAPIBrowser.md

Obtains the IProjectModelQueryableSpace instance required to perform queries against the Visual Studio Project System API. This involves getting the IProjectSystemQueryService via the global service provider.

```csharp
var service = (IProjectSystemQueryService)await ServiceProvider.GetGlobalServiceAsync(typeof(IProjectSystemQueryService));
IProjectModelQueryableSpace queryableSpace = await service.GetProjectModelQueryableSpaceAsync();
```

--------------------------------

### Overriding Extension Setting in JSON (JSON)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SettingsSample/README.md

Provides an example of how to override a specific extension setting value by adding an entry to the `extensibility.settings.json` file, explaining that the key is the `FullId` formed by the category and setting IDs.

```json
/* Visual Studio Settings File */
{
  "settingsSample.textLength": 150
}
```

--------------------------------

### Mutating Text in Active View (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/InsertGuid/README.md

Edits the document associated with the active text view by first getting the document, then using `Extensibility.Editor().EditAsync` with an edit batch to replace the current selection's extent with a new GUID string.

```csharp
var document = await textView.GetTextDocumentAsync(cancellationToken);
await this.Extensibility.Editor().EditAsync(
    batch =>
    {
        document.AsEditable(batch).Replace(textView.Selection.Extent, newGuidString);
    },
    cancellationToken);
```

--------------------------------

### Getting Active Text View in Visual Studio (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/EncodeDecodeBase64/README.md

This snippet demonstrates how to obtain the active text view using the `GetActiveTextViewAsync` method from the provided `context` (likely an `IClientContext` or similar). This view provides access to the document, selections, and other editor-related information needed to interact with the open file.

```csharp
using var textView = await context.GetActiveTextViewAsync(cancellationToken);
```

--------------------------------

### Creating TextMarker Tags in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/TaggersSample/README.md

This method processes requested document ranges, converts them to line numbers, and creates TextMarkerTags for lines starting with '#'. It uses the built-in 'FindHighlight' marker type as a workaround. The method collects the processed ranges and generated tags, then updates the tagger by calling UpdateTagsAsync.

```C#
private async Task CreateTagsAsync(ITextDocumentSnapshot document, IEnumerable<TextRange> requestedRanges)
{
    List<TaggedTrackingTextRange<TextMarkerTag>> tags = new();
    List<TextRange> ranges = new();
    
    foreach (var lineNumber in requestedRanges.SelectMany(r =>
        {
            // Convert the requested range to line numbers.
            var startLine = r.Document.GetLineNumberFromPosition(r.Start);
            var endLine = r.Document.GetLineNumberFromPosition(r.End);
            return Enumerable.Range(startLine, endLine - startLine + 1);
        }).Distinct()) // Use Distinct to avoid processing the same line multiple times.
    {
        var line = document.Lines[lineNumber];
        if (line.Text.StartsWith("#"))
        {
            int len = line.Text.Length;
            if (len > 0)
            {
                // VisualStudio.Extensibility doesn't support defining new TextMarker types yet, so we use
                // the built-in FindHighlight TextMarker type.
                tags.Add(new(
                    new(document, line.Text.Start, len, TextRangeTrackingMode.ExtendForwardAndBackward),
                    new("MarkerFormatDefinition/FindHighlight")));
            }
        }
        
        // Add the range to the list of ranges we have calculated tags for. We add the range even if no tags
        // were created for it, this takes care of clearing any tags that were previously created for this
        // range and are not valid anymore.
        ranges.Add(new(document, line.TextIncludingLineBreak.Start, line.TextIncludingLineBreak.Length));
    }
    
    // Return the ranges we have calculated tags for and the tags themselves.
    await this.UpdateTagsAsync(ranges, tags, CancellationToken.None);
}
```

--------------------------------

### Get Editor Option Value (With Default) in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Shows a safer pattern for retrieving an editor option value using GetOptionValueAsync. This approach uses the .ValueOrDefault method, providing a fallback default value to prevent exceptions if the option is missing.

```cs
// Never throws
bool useSpaces = (await textView.GetOptionValueAsync(KnownDocumentOptions.ConvertTabsToSpacesOption, cancellationToken)).ValueOrDefault(defaultValue: false);
```

--------------------------------

### Get Editor Option Value (Potentially Throws) in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Demonstrates how to retrieve an editor option value using the GetOptionValueAsync method. This pattern accesses the .Value property directly, which may throw an exception if the option is not found or the value is null.

```cs
// Potentially throws
bool useSpaces = (await textView.GetOptionValueAsync(KnownDocumentOptions.ConvertTabsToSpacesOption, cancellationToken)).Value;
```

--------------------------------

### Defining VSImageId Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the structure for a Visual Studio image identifier, composed of a GUID and an integer ID. This type is used to uniquely reference specific image assets within the Visual Studio environment.

```typescript
export interface VSImageId {

    /**
     * Gets or sets the VSImageId._vs_guid component of the unique identifier.
    **/
    _vs_guid : Guid,

    /**
     * Gets or sets the integer component of the unique identifier.
    **/
    _vs_id : integer,
}
```

--------------------------------

### Accessing Group.Name via Reflection in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Demonstrates accessing the `Name` property of a `Group` object using reflection. It creates a delegate `Func<Group, string?>` to get the property value dynamically, which is necessary because the property is not part of the `netstandard2.0` API surface.

```csharp
private static readonly Func<Group, string?>? GetGroupName =
    (Func<Group, string?>?)typeof(Group).GetProperty("Name")?.GetGetMethod().CreateDelegate(typeof(Func<Group, string?>));

...

Name = $"[{GetGroupName?.Invoke(g) ?? i.ToString()}]"
```

--------------------------------

### Processing and Replacing Text Selections (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/EncodeDecodeBase64/README.md

This snippet accesses the selections from the active text view snapshot. It iterates through each selection, checks if it's empty, and if not, it gets the selected text, processes it using an `EncodeOrDecode` method (not shown), and replaces the original selection with the new text using the `textDocumentEditor`.

```csharp
ITextDocumentEditor textDocumentEditor = textView.Document.AsEditable(batch);
var selections = textView.Selections;

for (int i = 0; i < selections.Count; i++)
{
    var selection = selections[i];
    if (selection.IsEmpty)
    {
        continue;
    }

    string newText = this.EncodeOrDecode(selection.Extent.CopyToString());
    textDocumentEditor.Replace(selection.Extent, newText);
}
```

--------------------------------

### Defining a Valid VisualStudio.Extensibility Setting ID (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

This snippet demonstrates the correct format for defining a VisualStudio.Extensibility setting ID using the SettingCategory class. Valid IDs must start with a lowercase letter and meet minimum length requirements, as enforced by new validation rules.

```csharp
public static SettingCategory MySettingCategory => new("settingsSample", "Settings Sample")
```

--------------------------------

### Registering a Visual Studio Command (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Defines a class inheriting from `Command` and applies the `[VisualStudioContribution]` attribute to register it with Visual Studio, making it available for execution.

```csharp
[VisualStudioContribution]
internal class SampleCommand : Command
{
```

--------------------------------

### Set Solution Startup Projects - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Shows how to configure the startup projects for a solution. It uses UpdateSolutionAsync to target the solution and then calls SetStartupProjects with one or more project paths.

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(solution => solution.BaseName == solutionName),
    solution => solution.SetStartupProjects(projectPath1, projectPath2),
    cancellationToken);
```

--------------------------------

### Creating Language Server Connection (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RustLanguageServerProvider/README.md

Overrides `CreateServerConnectionAsync` to launch the `rust-analyzer.exe` process. It configures the process to redirect standard input/output and creates a `DuplexPipe` to communicate with the process streams, returning the pipe for Visual Studio to use.

```C#
public override Task<IDuplexPipe?> CreateServerConnectionAsync(CancellationToken cancellationToken)
{
    ProcessStartInfo info = new ProcessStartInfo();
    info.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, @"rust-analyzer.exe");
    info.RedirectStandardInput = true;
    info.RedirectStandardOutput = true;
    info.UseShellExecute = false;
    info.CreateNoWindow = true;

    Process process = new Process();
    process.StartInfo = info;

    if (process.Start())
    {
        return Task.FromResult<IDuplexPipe?>(new DuplexPipe(
            PipeReader.Create(process.StandardOutput.BaseStream),
            PipeWriter.Create(process.StandardInput.BaseStream)));
    }

    return Task.FromResult<IDuplexPipe?>(null);
}
```

--------------------------------

### Setting up VS Project Query API Service (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates how to obtain an instance of the WorkspacesExtensibility object, which serves as the entry point for interacting with the Visual Studio Project System Query API. This object is required before performing any queries or updates.

```csharp
WorkspacesExtensibility querySpace = this.Extensibility.Workspaces();
```

--------------------------------

### Handling Settings Changes with Observer (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SettingsSample/README.md

Demonstrates how to inject and use the generated `SettingsSampleCategoryObserver` in a tool window constructor and how to handle the `Changed` event to react to setting value updates, noting that the event is invoked at least once upon subscription.

```csharp
public MyToolWindowData(VisualStudioExtensibility extensibility, SettingsSampleCategoryObserver settingsObserver)
{
    ...
    this.settingsObserver = Requires.NotNull(settingsObserver);
    settingsObserver.Changed += this.SettingsObserver_ChangedAsync;
}

private Task SettingsObserver_ChangedAsync(Settings.SettingsSampleCategorySnapshot settingsSnapshot)
{
    this.ManualUpdate = !settingsSnapshot.AutoUpdateSetting.ValueOrDefault(defaultValue: true);
    ...
}
```

--------------------------------

### Showing Custom Prompt with Options (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Demonstrates using `shell.ShowPromptAsync` with `PromptOptions<T>` to present a set of custom choices to the user. Each choice is mapped to a value of type `T` (here, `TokenThemeResult`), which is returned upon selection. Also shows configuring the return value for dismissal and setting a default choice.

```csharp
// Custom prompt
var themeResult = await shell.ShowPromptAsync(
    "Which theme should be used for the generated output?",
    new PromptOptions<TokenThemeResult>
    {
        Choices =
        {
            { "Solarized Is Awesome", TokenThemeResult.Solarized },
            { "OneDark Is The Best", TokenThemeResult.OneDark },
            { "GruvBox Is Groovy", TokenThemeResult.GruvBox },
        },
        DismissedReturns = TokenThemeResult.None,
        DefaultChoiceIndex = 2,
    },
    ct);
```

--------------------------------

### Executing a Visual Studio Command to Show Tool Window (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Implements the command's execution logic. When the command is invoked, it uses the `Extensibility.Shell()` service to show the `MyToolWindow`. The `activate: true` parameter ensures the tool window receives focus upon being shown.

```C#
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    await this.Extensibility.Shell().ShowToolWindowAsync<MyToolWindow>(activate: true, cancellationToken);
}
```

--------------------------------

### Executing Command to Show a Dialog (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Implements the `ExecuteCommandAsync` method to handle command execution. It creates an instance of `MyDialogControl` and uses the `Shell().ShowDialogAsync` helper to display it as a dialog in Visual Studio.

```csharp
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    // Ownership of the RemoteUserControl is transferred to VisualStudio, so it should not be disposed by the extension
    #pragma warning disable CA2000 // Dispose objects before losing scope
    var control = new MyDialogControl(null);
    #pragma warning restore CA2000 // Dispose objects before losing scope

    await this.Extensibility.Shell().ShowDialogAsync(control, cancellationToken);
}
```

--------------------------------

### Initializing Data and Creating UI Content (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Overrides `InitializeAsync` and `GetContentAsync`. `InitializeAsync` is used for setting up the data context (`MyToolWindowData`). `GetContentAsync` creates and returns the remote user control (`MyToolWindowControl`) that provides the visual content for the tool window, passing the data context to it.

```C#
public override Task InitializeAsync(CancellationToken cancellationToken)
{
    this.dataContext = new MyToolWindowData(this.Extensibility);
    return Task.CompletedTask;
}
public override Task<IRemoteUserControl> GetContentAsync(CancellationToken cancellationToken)
{
    return Task.FromResult<IRemoteUserControl>(new MyToolWindowControl(this.dataContext));
}
```

--------------------------------

### Defining Visual Studio Extension Settings (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SettingsSample/README.md

Shows how to define a setting category and a boolean setting using `VisualStudioContribution` attribute and `SettingCategory` and `Setting.Boolean` classes. Explains the purpose of `GenerateObserverClass` for generating an observer class.

```csharp
[VisualStudioContribution]
internal static SettingCategory SettingsSampleCategory { get; } = new("settingsSample", "%SettingsSample.Settings.Category.DisplayName%")
{
    Description = "%SettingsSample.Settings.Category.Description%",
    GenerateObserverClass = true,
};

[VisualStudioContribution]
internal static Setting.Boolean AutoUpdateSetting { get; } = new("autoUpdate", "%SettingsSample.Settings.AutoUpdate.DisplayName%", SettingsSampleCategory, defaultValue: true)
{
    Description = "%SettingsSample.Settings.AutoUpdate.Description%",
};
```

--------------------------------

### Showing Simple OK/Cancel Prompt (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Displays a prompt to the user with "OK" and "Cancel" options using `shell.ShowPromptAsync`. If the user selects "Cancel", the method returns `false`, allowing the command execution to be halted. Uses `PromptOptions.OKCancel`.

```csharp
if (!await shell.ShowPromptAsync("Continue with executing the command?", PromptOptions.OKCancel, ct))
{
    return;
}
```

--------------------------------

### Defining Rust Language Server Provider Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RustLanguageServerProvider/README.md

Defines the main class for the language server provider, inheriting from `LanguageServerProvider`. The `[VisualStudioContribution]` attribute is essential for Visual Studio to discover and register this provider.

```C#
[VisualStudioContribution]
internal class RustLanguageServerProvider : LanguageServerProvider
{
```

--------------------------------

### Prompting User to Open Multiple Files (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/FilePickerSample/README.md

Illustrates how to use the ShowOpenMultipleFilesDialogAsync method to allow users to select one or more files. It shows configuring the dialog with FileDialogOptions, including setting an initial filename and defining file type filters.

```csharp
FileDialogOptions options = new()
{
    InitialFileName = "test.cs",
    Filters = new DialogFilters(
        new("Log Files", "*.txt", "*.log"),
        new("CSharp Files", "*.cs")),
};

IReadOnlyList<string>? filePaths = await this.Extensibility.Shell().ShowOpenMultipleFilesDialogAsync(options, ct);
```

--------------------------------

### Invoke Solution Build Action - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates how to invoke the BuildAsync action on the solution level using the Project Query API. This action initiates a build operation for the entire solution.

```csharp
var result = await querySpace.Solutions
            .BuildAsync(cancellationToken);
```

--------------------------------

### Initial Query for By Id Pattern (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Shows an initial query to retrieve basic project and configuration information (name, active configuration name) as a prerequisite for the "Querying By Id" pattern. The results of this query are then processed to perform subsequent queries on individual items.

```csharp
var result = await querySpace.QueryProjectsAsync(
	project => project.With(p => p.Name)
						.With(p => p.ActiveConfigurations.With(c => c.Name)),
	cancellationToken);
```

--------------------------------

### Prompting User to Open a Single File (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/FilePickerSample/README.md

Demonstrates how to use the ShowOpenFileDialogAsync method to display a dialog for selecting a single file. It shows configuring the dialog with FileDialogOptions, including setting an initial filename and defining file type filters with a default selection.

```csharp
FileDialogOptions options = new()
{
    InitialFileName = "test.cs",
    Filters = new DialogFilters([
        new("Log Files", "*.txt", "*.log"),
        new("CSharp Files", "*.cs"),
    ])
    {
        DefaultFilterIndex = 1,
    },
};

string? filePath = await this.Extensibility.Shell().ShowOpenFileDialogAsync(options, cancellationToken);
```

--------------------------------

### Reading Current Settings Snapshot (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SettingsSample/README.md

Shows how to read the current values of settings asynchronously using the `GetSnapshotAsync` method of the settings observer if relying solely on events is not desired.

```csharp
var settingsSnapshot = await this.settingsObserver.GetSnapshotAsync(cancellationToken);
```

--------------------------------

### Writing Extension Settings (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SettingsSample/README.md

Illustrates how to update setting values programmatically through `VisualStudioExtensibility.Settings()` by calling `WriteAsync`, which allows batching multiple settings updates in a single operation.

```csharp
this.extensibility.Settings().WriteAsync(
    batch =>
    {
        batch.WriteSetting(SettingDefinitions.AutoUpdateSetting, !value);
    },
    description: Resources.AutoUpdateSettingWriteDescription,
    CancellationToken.None);
```

--------------------------------

### Showing Prompt with Custom Title and Icon (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Illustrates how to customize a prompt displayed via `shell.ShowPromptAsync` by setting a custom title and specifying an icon using `ImageMoniker.KnownValues`. This allows for more informative and visually distinct prompts.

```csharp
bool confirmConfiguration = await shell.ShowPromptAsync(
    $"The selected system ({selectedSystem}) may require additional resources. Do you want to proceed?",
    PromptOptions.OKCancel with
    {
        Title = Title,
        Icon = ImageMoniker.KnownValues.StatusSecurityWarning,
    },
    cancellationToken);
```

--------------------------------

### Configuring Visual Studio Command Properties (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Configures the command's properties, including its display name, placement in the `Tools` menu, and the icon used (`ToolWindow`). This configuration is available to Visual Studio before the extension loads.

```C#
public override CommandConfiguration CommandConfiguration => new("%ToolWindowSample.MyToolWindowCommand.DisplayName%")
{
    Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    Icon = new(ImageMoniker.KnownValues.ToolWindow, IconSettings.IconAndText),
};
```

--------------------------------

### Configuring Resource Dictionary as Embedded Resource (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Demonstrates how to configure resource dictionary files (`.xaml`) in the project file (`.csproj`) to be included as embedded resources rather than standard pages, ensuring they are available at runtime.

```xml
<ItemGroup>
  <EmbeddedResource Include="Resources\MyResources.*xaml" />
  <Page Remove="Resources\MyResources.*xaml" />
</ItemGroup>
```

--------------------------------

### Handling Server Initialization Result (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RustLanguageServerProvider/README.md

Overrides `OnServerInitializationResultAsync` to check the result of the LSP initialization handshake. If initialization fails, it logs the failure (commented out) and disables the language server provider to prevent further activation attempts.

```C#
public override Task OnServerInitializationResultAsync(ServerInitializationResult serverInitializationResult, LanguageServerInitializationFailureInfo? initializationFailureInfo, CancellationToken cancellationToken)
{
    if (serverInitializationResult == ServerInitializationResult.Failed)
    {
        // Log telemetry for failure and disable the server from being activated again.
        this.Enabled = false;
    }

    return base.OnServerInitializationResultAsync(serverInitializationResult, initializationFailureInfo, cancellationToken);
}
```

--------------------------------

### Prompting User to Open a Folder (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/FilePickerSample/README.md

Explains how to use the ShowOpenFolderDialogAsync method to prompt the user to select a folder path instead of a file. It shows using a FolderDialogOptions object to configure the folder selection dialog.

```csharp
FolderDialogOptions options = new();
string? folderPath = await this.Extensibility.Shell().ShowOpenFolderDialogAsync(options, ct);
```

--------------------------------

### Configuring Command Properties (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SimpleRemoteCommandSample/README.md

This snippet defines the `CommandConfiguration` property, setting the command's display name, specifying its placement within the Visual Studio Tools menu, and assigning an icon using `ImageMoniker.KnownValues.Extension`.

```csharp
public override CommandConfiguration CommandConfiguration => new("%SimpleRemoteCommandSample.CommandHandler.DisplayName%")
{
    Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    Icon = new(ImageMoniker.KnownValues.Extension, IconSettings.IconAndText),
};
```

--------------------------------

### Prompting User to Save a File As (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/FilePickerSample/README.md

Shows how to use the ShowSaveAsFileDialogAsync method to prompt the user to choose a location and name for saving a file. It demonstrates setting the dialog's title and an initial filename using FileDialogOptions.

```csharp
 FileDialogOptions options = new()
{
    Title = "Save as File",
    InitialFileName = "result.txt",
};

string? filePath = await this.Extensibility.Shell().ShowSaveAsFileDialogAsync(options, ct);
```

--------------------------------

### Configuring Container Project for PkgDef and Codebase (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Configures the Container project's .csproj file to enable .pkgdef file generation, use codebases for this generation, and sign the assembly with a strong name. These settings are crucial for Visual Studio to load the assembly.

```XML
<GeneratePkgDefFile>true</GeneratePkgDefFile>
<UseCodebase>true</UseCodebase>
<SignAssembly>True</SignAssembly>
<AssemblyOriginatorKeyFile>sgKey.snk</AssemblyOriginatorKeyFile>
```

--------------------------------

### Configuring Visual Studio Command Properties (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Overrides the `CommandConfiguration` property to define metadata for the command, including its display name, tooltip text, and placement within the Visual Studio UI (e.g., the Tools menu).

```csharp
public override CommandConfiguration CommandConfiguration => new("%UserPromptSample.SampleCommand.DisplayName%")
{
    TooltipText = "%UserPromptSample.SampleCommand.ToolTip%",
    Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
};
```

--------------------------------

### Defining Localized Display Name (JSON)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RustLanguageServerProvider/README.md

A JSON snippet from `string-resources.json` that defines the localized string value for the language server's display name, referenced by the C# configuration.

```JSON
{
    "RustLspExtension.RustLanguageServerProvider.DisplayName": "Rust Analyzer LSP server"
}
```

--------------------------------

### Showing Input Prompt with Default Options (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

This C# snippet shows how to use shell.ShowPromptAsync with the default input prompt options (InputPromptOptions.Default). It uses the with expression to override only the Title property while keeping other default settings. The prompt asks the user to enter a project name.

```csharp
string? projectName = await shell.ShowPromptAsync(
    "Enter the name of the project to configure?",
    InputPromptOptions.Default with { Title = Title },
    cancellationToken);
```

--------------------------------

### Showing Prompt with Cancel as Default (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

Displays a confirmation prompt using `shell.ShowPromptAsync` with "OK" and "Cancel" options. The `WithCancelAsDefault()` method is used to make "Cancel" the default selected option, suitable for operations requiring extra caution.

```csharp
// Asking the user to confirm a dangerous operation.
if (!await shell.ShowPromptAsync("Continue with executing the command?", PromptOptions.OKCancel.WithCancelAsDefault(), ct))
{
    return;
}
```

--------------------------------

### Add Solution Configuration - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates how to add a new solution configuration. It uses UpdateSolutionAsync to target the solution and calls AddSolutionConfiguration with the new name, the base configuration name, and a propagation flag.

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(solution => solution.BaseName == solutionName),
    solution => solution.AddSolutionConfiguration("Foo", "Debug", false),
    cancellationToken);
```

--------------------------------

### Configuring Rust Language Server Provider (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RustLanguageServerProvider/README.md

Overrides the `LanguageServerProviderConfiguration` property to provide metadata about the server. It sets the display name using a localized string resource and specifies that the server applies to documents of the `RustDocumentType`.

```C#
public override LanguageServerProviderConfiguration LanguageServerProviderConfiguration => new(
    "%RustLspExtension.RustLanguageServerProvider.DisplayName%",
    new[] 
    { 
        DocumentFilter.FromDocumentType(RustDocumentType) 
    });
```

--------------------------------

### Defining a Visual Studio Command Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Defines a command class `MyToolWindowCommand` that is made available to Visual Studio using the `[VisualStudioContribution]` attribute. This class inherits from `Command`.

```C#
[VisualStudioContribution]
public class MyToolWindowCommand : Command
{
```

--------------------------------

### Define and Configure Visual Studio Toolbar - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommandParentingSample/README.md

Defines a static property `ToolBar` of type `ToolbarConfiguration` and marks it with `[VisualStudioContribution]` to register a new toolbar in Visual Studio. The configuration sets the toolbar's display name and adds the `SampleCommand` as a child element using `ToolbarChild.Command<SampleCommand>()`, parenting the command to this new toolbar.

```C#
    [VisualStudioContribution]
    public static ToolbarConfiguration ToolBar => new("%CommandParentingSample.ToolBar.DisplayName%")
    {
        Children =
        [
            ToolbarChild.Command<SampleCommand>(),
        ],
    };
}
```

--------------------------------

### Creating Clickable CodeLens Instance - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Implements the `TryCreateCodeLensAsync` method to determine if a Code Lens should be created for a given code element. It specifically creates a `ClickableCodeLens` instance if the element is identified as a method.

```csharp
public Task<CodeLens?> TryCreateCodeLensAsync(CodeElement codeElement, CodeElementContext codeElementContext, CancellationToken token)
{
    if (codeElement.Kind == CodeElementKind.KnownValues.Method)
    {
        return Task.FromResult<CodeLens?>(new ClickableCodeLens(codeElement, this.Extensibility));
    }

    return Task.FromResult<CodeLens?>(null);
}
```

--------------------------------

### Defining a Visual Studio Tool Window Toolbar Configuration (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Defines a static `ToolbarConfiguration` property using the `[VisualStudioContribution]` attribute. This configuration specifies the toolbar's display name and includes a single command, `MyToolbarCommand`, as a child element.

```C#
[VisualStudioContribution]
private static ToolbarConfiguration Toolbar => new("%ToolWindowSample.MyToolWindow.Toolbar.DisplayName%")
{
    Children = [ToolbarChild.Command<MyToolbarCommand>()],
};
```

--------------------------------

### Configuring CodeLens Provider - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Defines the configuration for the Code Lens provider, setting its display name ('Invokable CodeLens Sample Provider') and a priority of 500 to influence its placement relative to other Code Lenses.

```csharp
public CodeLensProviderConfiguration CodeLensProviderConfiguration =>
    new("Invokable CodeLens Sample Provider")
    {
        Priority = 500,
    };
```

--------------------------------

### Defining CommandHandler Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/SimpleRemoteCommandSample/README.md

This snippet defines the `CommandHandler` class, inheriting from `Command`, and applies the `VisualStudioContribution` attribute to register it with Visual Studio as a command using its full type name.

```csharp
[VisualStudioContribution]
internal class CommandHandler : Command
{
```

--------------------------------

### Configuring Tool Window Placement (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Overrides the `ToolWindowConfiguration` property to define settings for the tool window. This snippet sets the `Placement` property to `ToolWindowPlacement.DocumentWell`, causing the tool window to appear in the document well by default.

```C#
public override ToolWindowConfiguration ToolWindowConfiguration => new()
{
    Placement = ToolWindowPlacement.DocumentWell,
};
```

--------------------------------

### Configuring Visual Studio Command Properties (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/EncodeDecodeBase64/README.md

This code defines the `CommandConfiguration` property, which specifies how the command appears and behaves in the Visual Studio UI. It sets the display name, places the command in the Extensions menu, assigns an icon, and defines activation constraints for visibility (solution fully loaded) and enabled state (active editor is C#).

```csharp
    public override CommandConfiguration CommandConfiguration => new("%EncodeDecodeBase64.EncodeDecodeBase64Command.DisplayName%")
    {
        Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu],
        Icon = new(ImageMoniker.KnownValues.ConvertPartition, IconSettings.IconAndText),
        VisibleWhen = ActivationConstraint.SolutionState(SolutionState.FullyLoaded),
        EnabledWhen = ActivationConstraint.ClientContext(ClientContextKey.Shell.ActiveEditorContentType, "csharp"),
    };
```

--------------------------------

### Save Solution - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Illustrates how to save the current state of the solution using the SaveAsync method available on the solution level of the Project Query API.

```csharp
var result = await querySpace.Solutions.SaveAsync(cancellationToken);
```

--------------------------------

### Defining a Visual Studio Command Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Defines a C# class `MyDialogCommand` that inherits from `Command` and is marked with the `[VisualStudioContribution]` attribute, making it discoverable by Visual Studio.

```csharp
[VisualStudioContribution]
public class MyDialogCommand : Command
{
```

--------------------------------

### Invoke Project Build Action - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Illustrates how to invoke a build action (BuildAsync) on a specific project snapshot (IProjectSnapshot). This is used after querying for projects and selecting one to build.

```csharp
await result.First().BuildAsync(cancellationToken);
```

--------------------------------

### Defining Tool Window Content with XAML DataTemplate (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Shows the basic structure of the XAML `DataTemplate` used to define the visual content of the tool window. It includes necessary namespace declarations for WPF, XAML, Visual Studio Extensibility, and Visual Studio Shell styles/colors.

```XML
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:vs="http://schemas.microsoft.com/visualstudio/extensibility/2022/xaml"
              xmlns:styles="clr-namespace:Microsoft.VisualStudio.Shell;assembly=Microsoft.VisualStudio.Shell.15.0"
              xmlns:colors="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0">
    ...
</DataTemplate>
```

--------------------------------

### Configure CodeLensProvider Display and Priority (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Sets the display name for the Code Lens provider to 'Invokable CodeLens Sample Provider' and assigns a priority of 500. The priority determines the order in which Code Lenses are displayed when multiple providers are active.

```csharp
 public CodeLensProviderConfiguration CodeLensProviderConfiguration =>
    new("Invokable CodeLens Sample Provider")
    {
        Priority = 500,
    };
```

--------------------------------

### Create WordCountCodeLens Instance (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Implements the `TryCreateCodeLensAsync` method to conditionally create a `WordCountCodeLens` instance. A Code Lens is created only if the provided `codeElement` is a method or a type, otherwise, it returns null.

```csharp
public Task<CodeLens?> TryCreateCodeLensAsync(CodeElement codeElement, CodeElementContext codeElementContext, CancellationToken token)
{
    if (codeElement.Kind == CodeElementKind.KnownValues.Method || codeElement.Kind.IsOfKind(CodeElementKind.KnownValues.Type))
    {
        return Task.FromResult<CodeLens?>(new WordCountCodeLens(codeElement, codeElementContext, this.Extensibility, this));
    }

    return Task.FromResult<CodeLens?>(null);
}
```

--------------------------------

### Define Visual Studio Command Class - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommandParentingSample/README.md

Defines the `SampleCommand` class which inherits from `Command` and is marked with the `[VisualStudioContribution]` attribute to register it as a command within Visual Studio. This attribute makes the command available to the IDE using the class's full type name as its unique identifier.

```C#
[VisualStudioContribution]
internal class SampleCommand : Command
{
```

--------------------------------

### Implementing Simple Match Visualizer Logic (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

This snippet illustrates a simplified implementation of the visualizer configuration and data retrieval. It defines the visualizer's name and target type and attempts to fetch the Match object directly, assuming it was serializable.

```csharp
public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => ew("Regex Match visualizer", typeof(Match));

public override async Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget isualizerTarget, CancellationToken cancellationToken)
{
    var regexMatch = await visualizerTarget.ObjectSource.RequestDataAsync<Match>(jsonSerializer: null, cancellationToken);
    return new RegexMatchVisualizerUserControl(regexMatch);
}

```

--------------------------------

### Adding Embedded Resource Dictionary to Remote Control (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Shows the constructor for the `MyDialogControl` class. It calls the base constructor and then adds an embedded resource dictionary named "DialogSample.Resources.MyResources" to the control's resources.

```csharp
internal class MyDialogControl : RemoteUserControl
{
    public MyDialogControl(object? dataContext, SynchronizationContext? synchronizationContext = null)
        : base(dataContext, synchronizationContext)
    {
        this.ResourceDictionaries.AddEmbeddedResource("DialogSample.Resources.MyResources");
    }
}
```

--------------------------------

### Defining Visual Studio Command Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/EncodeDecodeBase64/README.md

This snippet shows the definition of the `EncodeDecodeBase64Command` class, inheriting from `Command`. The `[VisualStudioContribution]` attribute is used to register this class as a command within Visual Studio, making it discoverable and usable by the IDE.

```csharp
[VisualStudioContribution]
internal class EncodeDecodeBase64Command : Command
{
```

--------------------------------

### Showing Input Prompt with Default Text (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/UserPromptSample/README.md

This snippet demonstrates how to display a single-line input prompt using shell.ShowPromptAsync in C#. It uses InputPromptOptions to set a default text value ("Works as expected."), an icon, and a title for the prompt. The user's input or the default text is returned as a string, or null if dismissed.

```csharp
string? feedback = await shell.ShowPromptAsync(
    $"Thank you for configuring {projectName}. Do you have any feedback?",
    new InputPromptOptions
    {
        DefaultText = "Works as expected.",
        Icon = ImageMoniker.KnownValues.Feedback,
        Title = Title,
    },
    cancellationToken);
```

--------------------------------

### Defining Visual Studio Tool Window Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Defines the main class for the tool window extension. The `[VisualStudioContribution]` attribute registers the class with Visual Studio, making the tool window available. The class inherits from `ToolWindow`.

```C#
[VisualStudioContribution]
public class MyToolWindow : ToolWindow
{
```

--------------------------------

### Referencing Toolbar in Visual Studio Tool Window Configuration (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Configures the tool window's properties, including its placement in the document well (`ToolWindowPlacement.DocumentWell`). It references the previously defined `ToolbarConfiguration` to associate the toolbar with the tool window.

```C#
public override ToolWindowConfiguration ToolWindowConfiguration => new()
{
    Placement = ToolWindowPlacement.DocumentWell,
    Toolbar = new ToolWindowToolbar(Toolbar),
};
```

--------------------------------

### Defining a Tagger Provider in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/TaggersSample/README.md

This C# snippet defines a Tagger Provider class, `MarkdownTextMarkerTaggerProvider`, which is a Visual Studio extension part. It uses the `VisualStudioContribution` attribute to be discoverable, specifies that it applies only to 'vs-markdown' document types via `TextViewExtensionConfiguration`, and implements `CreateTaggerAsync` to instantiate the tagger for a given text view.

```C#
[VisualStudioContribution]
internal class MarkdownTextMarkerTaggerProvider : ExtensionPart, ITextViewTaggerProvider<TextMarkerTag>
{
    public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
    {
        AppliesTo = [DocumentFilter.FromDocumentType("vs-markdown")],
    };

    public Task<TextViewTagger<TextMarkerTag>> CreateTaggerAsync(ITextViewSnapshot textView, CancellationToken cancellationToken)
    {
        var tagger = new MarkdownTextMarkerTagger(this, textView.Document.Uri);
        return Task.FromResult<TextViewTagger<TextMarkerTag>>(tagger);
    }
}
```

--------------------------------

### Implementing Clickable CodeLens Behavior - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Shows the core logic for the `ClickableCodeLens`. `ExecuteAsync` increments a counter and invalidates the Code Lens to trigger a refresh. `GetLabelAsync` provides the text and tooltip displayed for the Code Lens, updating the text based on the click count.

```csharp
public override Task ExecuteAsync(CodeElementContext codeElementContext, IClientContext clientContext, CancellationToken cancelToken)
{
    this.clickCount++;
    this.Invalidate();
    return Task.CompletedTask;
}

public override Task<CodeLensLabel> GetLabelAsync(CodeElementContext codeElementContext, CancellationToken token)
{
    return Task.FromResult(new CodeLensLabel()
    {
        Text = this.clickCount == 0 ? "Click me" : $"Clicked {this.clickCount} times",
        Tooltip = "Invokable CodeLens Sample Tooltip",
    });
}
```

--------------------------------

### Performing Subsequent Query By Id (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Illustrates how to perform a follow-up query on an item obtained from a previous query result using AsQueryable(). This allows retrieving additional details (like output group names) for specific items identified in the initial query.

```csharp
await foreach (var project in result) 
{
	message.Append($"{project.Value.Name}\n");

	foreach (var config in project.Value.ActiveConfigurations) 
	{
		message.Append($" \t {config.Name}\n");

		foreach (var group in config.OutputGroups) 
		{
			// This is needed for byId:
			var newResult = await group.AsQueryable()
				.With(g => g.Name)
				.ExecuteQueryAsync();
		}
	}
}
```

--------------------------------

### Querying Project Information with VS Project Query API (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Shows how to use QueryProjectsAsync to retrieve specific information about projects within the solution. It uses the With keyword to select properties like project name, path, and file names.

```csharp
var result = await querySpace.QueryProjectsAsync(
	project => project.With(project => project.Name)
		.With(project => project.Path)
		.With(project => project.Files.With(file => file.FileName)),
	cancellationToken);
```

--------------------------------

### Configure Visual Studio Command Placements - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommandParentingSample/README.md

Overrides the `CommandConfiguration` property to define the command's display name using a resource string and specify its placements within the Visual Studio IDE. It uses `CommandPlacement.VsctParent` to add the command to the context menus for files, projects, and solutions in the Solution Explorer.

```C#
    public override CommandConfiguration CommandConfiguration => new("%CommandParentingSample.SampleCommand.DisplayName%")
    {
        Placements =
        [
            // File in project context menu
            CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 521, priority: 0),

            // Project context menu
            CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 518, priority: 0),

            // Solution context menu
            CommandPlacement.VsctParent(new Guid("{d309f791-903f-11d0-9efc-00a0c911004f}"), id: 537, priority: 0),
        ],
    };
```

--------------------------------

### Copying File to New Project using Project Query API (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Copies a file from a source path to a destination project using the `UpdateProjectsAsync` method. This is the first step in the workaround to move a file, adding the file to the target project 'ConsoleApp2' from the specified source file path.

```csharp
var result = await querySpace.UpdateProjectsAsync(
                project => project.Where(project => project.Name == "ConsoleApp2"),
                project => project.AddFileFromCopy(sourceFilePath, destinationProject),
                cancellationToken);
```

--------------------------------

### Defining Dialog Content with XAML DataTemplate (XAML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Defines the structure for the dialog's user interface using a XAML `DataTemplate`. It includes necessary XML namespace declarations for WPF, Visual Studio Extensibility, and shell/platform styles and colors.

```xaml
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:vs="http://schemas.microsoft.com/visualstudio/extensibility/2022/xaml"
              xmlns:styles="clr-namespace:Microsoft.VisualStudio.Shell;assembly=Microsoft.VisualStudio.Shell.15.0"
              xmlns:colors="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0">
  ...
</DataTemplate>
```

--------------------------------

### Delete Solution Configuration - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Shows how to remove an existing solution configuration. It uses UpdateSolutionAsync to target the solution and calls DeleteSolutionConfiguration with the name of the configuration to remove.

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(solution => solution.BaseName == solutionName),
    solution => solution.DeleteSolutionConfiguration("Foo"),
    cancellationToken);
```

--------------------------------

### Adding VS LSP Extension NuGet Packages to .NET Project

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

This XML snippet shows how to add the necessary NuGet package references for the .NET implementation of the Visual Studio Language Server Protocol extensions to a .NET project file (.csproj). It includes references to both the core LSP package and the VS extensions package.

```XML
  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.LanguageServer.Protocol" Version="17.2.8" />
    <PackageReference Include="Microsoft.VisualStudio.LanguageServer.Protocol.Extensions" Version="17.2.8" />
  </ItemGroup>
```

--------------------------------

### Configuring TextView Extension - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Defines the configuration for the text view extension, specifying that it applies to documents of type `DocumentType.KnownValues.Code`, which includes most text-based code files.

```csharp
/// <inheritdoc />
public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
{
    AppliesTo = new[]
    {
        DocumentFilter.FromDocumentType(DocumentType.KnownValues.Code),
    },
};
```

--------------------------------

### Unload Project from Solution - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Shows how to unload a specific project from a solution. It uses UpdateSolutionAsync to target the desired solution and then calls UnloadProject with the project's path.

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(solution => solution.BaseName == solutionName),
    solution => solution.UnloadProject(projectPath),
    cancellationToken);
```

--------------------------------

### Defining Regex Match Debugger Visualizer Provider (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

This C# snippet shows the basic definition of the DebuggerVisualizerProvider class for the Regex Match visualizer. It is marked with the VisualStudioContribution attribute to be discovered by the extensibility model.

```csharp
[VisualStudioContribution]
internal class RegexMatchDebuggerVisualizerProvider : DebuggerVisualizerProvider
{
    ...

```

--------------------------------

### Provide CodeLens Label and Visualization (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Implements the `GetLabelAsync` and `GetVisualizationAsync` methods for the `WordCountCodeLens`. `GetLabelAsync` calculates and returns the word count label, while `GetVisualizationAsync` returns a custom remote user control (`WordCountCodeLensVisual`) to be displayed when the Code Lens is clicked.

```csharp
public override Task<CodeLensLabel> GetLabelAsync(CodeElementContext codeElementContext, CancellationToken token)
{
    this.wordCountData.WordCount = CountWords(codeElementContext.Range);
    return Task.FromResult(new CodeLensLabel()
    {
        Text = $"Words: {this.wordCountData.WordCount}",
        Tooltip = "Number of words in this code element",
    });
}

public override Task<IRemoteUserControl> GetVisualizationAsync(CodeElementContext codeElementContext, IClientContext clientContext, CancellationToken token)
{
    return Task.FromResult<IRemoteUserControl>(new WordCountCodeLensVisual(this.wordCountData));
}
```

--------------------------------

### Configuring Visual Studio Command Properties (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Overrides the `CommandConfiguration` property to define metadata for the command, including its display name, placement in the `Tools` menu, and the icon to be used.

```csharp
public override CommandConfiguration CommandConfiguration => new("%DialogSample.MyDialogCommand.DisplayName%")
{
    Placements = [CommandPlacement.KnownPlacements.ToolsMenu],
    Icon = new(ImageMoniker.KnownValues.Dialog, IconSettings.IconAndText),
};
```

--------------------------------

### Rename Project - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates how to rename a project using the Project Query API. It queries for the project by name, makes it updatable, calls Rename with the new name, and executes the update.

```csharp
var result = await querySpace.Projects
    .Where(p => p.Name == "ConsoleApp1")
    .AsUpdatable()
    .Rename("NewProjectName")
    .ExecuteAsync(cancellationToken);
```

--------------------------------

### Defining Data Model Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Defines the data model class (`MyToolWindowData`) used as the data context for the tool window's UI. It requires the `[DataContract]` attribute for serialization and must derive from `NotifyPropertyChangedObject` to support property change notifications for data binding.

```C#
[DataContract]
internal class MyToolWindowData : NotifyPropertyChangedObject
{
```

--------------------------------

### Injecting Visual Studio SDK Services (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommentRemover/README.md

This C# constructor demonstrates how to use .NET dependency injection with `AsyncServiceProviderInjection` and `MefInjection` to consume Visual Studio SDK services like `DTE2`, `IVsTextManager`, `IBufferTagAggregatorFactoryService`, and `IVsEditorAdaptersFactoryService` within a `VisualStudio.Extensibility` command.

```csharp
public RemoveAllComments(
    TraceSource traceSource,
    AsyncServiceProviderInjection<DTE, DTE2> dte,
    MefInjection<IBufferTagAggregatorFactoryService> bufferTagAggregatorFactoryService,
    MefInjection<IVsEditorAdaptersFactoryService> editorAdaptersFactoryService,
    AsyncServiceProviderInjection<SVsTextManager, IVsTextManager> textManager)
    : base(traceSource, dte, bufferTagAggregatorFactoryService, editorAdaptersFactoryService, textManager)
{
}
```

--------------------------------

### Defining VisualStudio.Extensibility Menu Structure

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommentRemover/README.md

This C# snippet demonstrates how to create a custom menu and place commands within it using `MenuConfiguration`. Decorated with `[VisualStudioContribution]`, this static property defines the menu's display name, its placement within the Visual Studio UI (e.g., the Extensions menu), and its children, which can be commands or separators.

```CSharp
internal static class ExtensionCommandConfiguration
{
    [VisualStudioContribution]
    public static MenuConfiguration CommentRemoverMenu => new("%CommentRemover.CommentRemoverMenu.DisplayName%")
    {
        Placements =
        [
            CommandPlacement.KnownPlacements.ExtensionsMenu.WithPriority(0x01),
        ],
        Children =
        [
            MenuChild.Command<RemoveAllComments>(),
            MenuChild.Command<RemoveXmlDocComments>(),
            MenuChild.Command<RemoveAllExceptXmlDocComments>(),
            MenuChild.Separator,
            MenuChild.Command<RemoveTasks>(),
            MenuChild.Command<RemoveAllExceptTaskComments>(),
            MenuChild.Separator,
            MenuChild.Command<RemoveRegions>(),
        ],
    };
}
```

--------------------------------

### Include C# Interface in MSBuild Project (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

This XML snippet demonstrates how to include a C# source file (an interface definition) located in a different directory into the current project's compilation process using an MSBuild ItemGroup and Compile item. This makes the interface available for the out-of-process component to reference.

```xml
<ItemGroup>
  <Compile Include="..\CompositeExtension\IInProcService.cs" Link="IInProcService.cs" />
</ItemGroup>
```

--------------------------------

### Setting Tool Window Title in Constructor (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Constructor for the `MyToolWindow` class. It calls the base constructor and sets the `Title` property of the tool window, which is displayed in the tool window's title bar.

```C#
public MyToolWindow(VisualStudioExtensibility extensibility)
    : base(extensibility)
{
    this.Title = "My Tool Window";
}
```

--------------------------------

### Adding Confirmation Prompt and Progress Reporting in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommentRemover/README.md

This snippet demonstrates how to display a confirmation dialog using ShowPromptAsync and initiate a progress notification using StartProgressReportingAsync within the ExecuteCommandAsync method of a Visual Studio extension command. ShowPromptAsync returns a boolean indicating the user's choice (or false on dismissal), and StartProgressReportingAsync provides a disposable object to manage the progress notification lifecycle.

```CSharp
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    if (!await this.Extensibility.Shell().ShowPromptAsync(
        "All regions will be removed from the current document. Are you sure?",
        PromptOptions.OKCancel,
        cancellationToken))
    {
        return;
    }

    using var reporter = await this.Extensibility.Shell().StartProgressReportingAsync(
        "Removing comments",
        options: new(isWorkCancellable: false),
        cancellationToken);

}
```

--------------------------------

### Creating Remote User Control in C# Debugger Visualizer Provider

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

This method creates and returns the remote user control instance responsible for displaying the visualizer UI. It passes the `VisualizerTarget` to the user control, enabling it to interact with the visualizer object source asynchronously.

```csharp
public override Task<IRemoteUserControl> CreateVisualizerAsync(VisualizerTarget visualizerTarget, CancellationToken cancellationToken)
{
    return Task.FromResult<IRemoteUserControl>(new RegexMatchCollectionVisualizerUserControl(visualizerTarget));
}
```

--------------------------------

### Referencing Dynamic Resource in XAML (XAML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Shows how to reference a resource defined in an embedded resource dictionary within the XAML markup using the `DynamicResource` markup extension, allowing the text content to be retrieved from the resource.

```xaml
<TextBlock Text="{DynamicResource myDialogText}" />
```

--------------------------------

### Reload Project in Solution - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates how to reload a previously unloaded project within a solution. It uses UpdateSolutionAsync to find the solution and then calls ReloadProject with the project's path.

```csharp
await querySpace.UpdateSolutionAsync(
    solution => solution.Where(solution => solution.BaseName == solutionName),
    solution => solution.ReloadProject(projectPath),
    cancellationToken);
```

--------------------------------

### Adding ProvideCodeBase Attribute (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Adds the ProvideCodeBase assembly attribute to the Container project. This attribute is necessary for Visual Studio to locate and load the assembly correctly, enabling the use of codebases for pkgdef generation.

```C#
[assembly: ProvideCodeBase]
```

--------------------------------

### Include Visualizer Object Source DLL in VS Extension (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Configures the .csproj file to include the visualizer object source DLL as content, ensuring it's copied to the output directory. It also adds a project reference to guarantee the object source is built before the extension, while avoiding a direct assembly dependency.

```XML
  <ItemGroup>
    <Content Include="..\..\..\..\bin\samples\RegexMatchObjectSource\$(Configuration)\netstandard2.0\RegexMatchObjectSource.dll" Link="netstandard2.0\RegexMatchObjectSource.dll">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\RegexMatchObjectSource\RegexMatchObjectSource.csproj" ReferenceOutputAssembly="false" />
  </ItemGroup>
```

--------------------------------

### Defining Remote User Control Class (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ToolWindowSample/README.md

Defines the C# class (`MyToolWindowControl`) that corresponds to the XAML content (`MyToolWindowControl.xaml`). This class inherits from `RemoteUserControl` and provides the logic and data binding for the UI defined in the XAML file.

```C#
internal class MyToolWindowControl : RemoteUserControl
{
```

--------------------------------

### Modifying Project System by Adding File (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Illustrates how to modify the project system using UpdateProjectsAsync. It filters projects by name using Where and then uses AddFile to create a new file within the matched project.

```csharp
await querySpace.UpdateProjectsAsync(
	project => project.Where(project => project.Name == "ConsoleApp1"),
	project => project.AddFile("CreatedFile.txt"),
	cancellationToken);
```

--------------------------------

### Process Selected Files in RunLinterOnCurrentFileCommand (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/MarkdownLinter/README.md

This C# snippet demonstrates how a command handler retrieves the currently selected file path from the IDE context using `IClientContext`. It then iterates through the selected paths, filtering for files, and calls the `ProcessFileAsync` method on a `MarkdownDiagnosticsService` instance to run the linter on each selected file.

```C#
// Get the selected item URIs from IDE context that reprents the state when command was executed.
var selectedItemPaths = new Uri[] { await context.GetSelectedPathAsync(cancellationToken) };

// Enumerate through each selection and run linter on each selected item.
foreach (var selectedItem in selectedItemPaths.Where(p => p.IsFile))
{
    await this.diagnosticsProvider.ProcessFileAsync(selectedItem, cancellationToken);
}
```

--------------------------------

### Proffering In-Proc Brokered Service in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

Shows how to register an in-proc brokered service (InProcService) with the service collection during initialization using serviceCollection.ProfferBrokeredService. This makes the service available for consumption by other components, including out-of-proc components.

```C#
protected override void InitializeServices(IServiceCollection serviceCollection)
{
    serviceCollection.ProfferBrokeredService(
        InProcService.BrokeredServiceConfiguration,
        IInProcService.Configuration.ServiceDescriptor);
    base.InitializeServices(serviceCollection);
}
```

--------------------------------

### Referencing Out-of-Proc Project for Packaging in MSBuild XML

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

This XML snippet, added to the in-proc component's .csproj file, adds a project reference to the out-of-proc component. The settings ensure that the out-of-proc component's output is included in the in-proc component's VSIX package without directly referencing its assembly (due to different target frameworks) and specifies the target framework for building the dependency.

```XML
<ProjectReference Include="..\OutOfProcComponent\OutOfProcComponent.csproj">
  <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
  <SetTargetFramework>TargetFramework=net8.0-windows8.0</SetTargetFramework>
  <IncludeInVSIX>true</IncludeInVSIX>
  <IncludeOutputGroupsInVSIX>ExtensionFilesOutputGroup</IncludeOutputGroupsInVSIX>
</ProjectReference>
```

--------------------------------

### Including Extension extension.json in Container VSIX (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Configures the Container project's .csproj file to include the Extension project's generated extension.json file in the Container's VSIX package. Requires adjusting the path based on project structure.

```XML
<ItemGroup>
  <VSIXSourceItem Include="$(BaseOutputPath)..\Extension\$(Configuration)\net8.0-windows8.0\.vsextension\extension.json" VsixSubPath=".vsextension" />
</ItemGroup>
```

--------------------------------

### Configuring VisualStudio.Extensibility Command

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommentRemover/README.md

This C# snippet shows how to define the configuration for a VisualStudio.Extensibility command using the `CommandConfiguration` property within a class decorated with `[VisualStudioContribution]`. It sets the display name, icon, enabled state based on a condition, and keyboard shortcuts.

```CSharp
[VisualStudioContribution]
internal class RemoveAllComments : CommentRemoverCommand
{
    private const string CommandDescription = "%CommentRemover.RemoveAllComments.DisplayName%";

    /// <inheritdoc />
    public override CommandConfiguration CommandConfiguration => new(CommandDescription)
    {
        Icon = new(ImageMoniker.KnownValues.Uncomment, IconSettings.IconAndText),
        EnabledWhen = CommandEnabledWhen,
        Shortcuts = [new CommandShortcutConfiguration(ModifierKey.Control, Key.K, ModifierKey.Control, Key.Q)],
    };

    public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
    {

```

--------------------------------

### Configure TextViewEventListener for Markdown Files (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/MarkdownLinter/README.md

This C# snippet shows the configuration for a `TextViewEventListener` using the `TextViewExtensionConfiguration` property. The `AppliesTo` property is set to a list containing a `DocumentFilter` created with `FromGlobPattern("**/*.md", true)`, indicating that this listener should only be active for files with the `.md` extension.

```C#
public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
{
    AppliesTo =
    [
        DocumentFilter.FromGlobPattern("**/*.md", true),
    ],
};
```

--------------------------------

### Consuming Out-of-Proc Brokered Service in C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

Demonstrates how an in-proc component acquires a proxy to an out-of-proc brokered service (IOutOfProcService) using ServiceBroker.GetProxyAsync. It shows calling a method on the proxy (DoSomethingAsync) and properly disposing of the proxy instance in a finally block.

```C#
public override async Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
{
    var outOfProcService = await this.Extensibility.ServiceBroker.GetProxyAsync<IOutOfProcService>(
        IOutOfProcService.Configuration.ServiceDescriptor,
        cancellationToken);
    try
    {
        Assumes.NotNull(outOfProcService);
        await outOfProcService.DoSomethingAsync(cancellationToken);
    }
    finally
    {
        (outOfProcService as IDisposable)?.Dispose();
    }
}
```

--------------------------------

### Configure Document Selector for C# Tests

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DocumentSelectorSample/README.md

This snippet configures a text view extension to apply only to C# files located within any 'tests' directory using a glob pattern. It utilizes DocumentFilter.FromGlobPattern to define the matching criteria for documents.

```csharp
/// <inheritdoc />
public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
{
    AppliesTo =
    [
        DocumentFilter.FromGlobPattern("**/tests/*.cs", relativePath: false),
    ],
};
```

--------------------------------

### Reference Extension Project in Container CSPROJ (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Adds a `ProjectReference` to the container project's .csproj file, referencing the out-of-proc extension project. This configuration ensures the extension's output is included in the container's VSIX package without directly referencing its assembly, accommodating different target frameworks and packaging dependencies correctly.

```XML
<ProjectReference Include="..\Extension\Extension.csproj">
  <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
  <SkipGetTargetFrameworkProperties>true</SkipGetTargetFrameworkProperties>
  <IncludeInVSIX>true</IncludeInVSIX>
  <IncludeOutputGroupsInVSIX>ExtensionFilesOutputGroup</IncludeOutputGroupsInVSIX>
</ProjectReference>
```

--------------------------------

### Skip Projects in Query - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates how to skip a specified number of results when querying for projects. It uses QueryProjectsAsync and applies the Skip method to the query definition.

```csharp
var result = await querySpace.QueryProjectsAsync(
            project => project.With(p => p.Name)
            .Skip(1),
            cancellationToken);
```

--------------------------------

### Handling Text View Changes in a C# Tagger

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/TaggersSample/README.md

This C# method, `TextViewChangedAsync`, demonstrates how a tagger handles notifications about text view changes. It retrieves all previously requested ranges, translates the edited ranges to the current document snapshot, ensures edited ranges are not empty, and then calls `CreateTagsAsync` to generate tags for the intersection of requested and edited areas.

```C#
public async Task TextViewChangedAsync(ITextViewSnapshot textView, IReadOnlyList<TextEdit> edits, CancellationToken cancellationToken)
{
    var allRequestedRanges = await this.GetAllRequestedRangesAsync(textView.Document, cancellationToken);
    await this.CreateTagsAsync(
        textView.Document,
        allRequestedRanges.Intersect(
            edits.Select(e =>
                EnsureNotEmpty(
                    e.Range.TranslateTo(textView.Document, TextRangeTrackingMode.ExtendForwardAndBackward)))));
}
```

--------------------------------

### Reference Visualizer Object Source by Type (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Shows a simpler way to configure the DebuggerVisualizerProviderConfiguration by directly referencing the object source type using typeof(). This method is possible when the extension project *does* have a direct assembly dependency on the object source project.

```C#
    public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new("Regex Match visualizer", typeof(Match))
    {
        VisualizerObjectSourceType = new(typeof(RegexMatchObjectSource)),
    };
```

--------------------------------

### Deleting Original File using Project Query API (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Deletes the original source file after it has been copied to the new location. It obtains an updatable instance of the file snapshot and executes the delete action using `ExecuteAsync`.

```csharp
await sourceFile.AsUpdatable().Delete().ExecuteAsync();
```

--------------------------------

### Defining Remote User Control Class for Dialog (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/DialogSample/README.md

Defines the C# class `MyDialogControl` that inherits from `RemoteUserControl`, serving as the code-behind for the XAML-defined dialog content.

```csharp
internal class MyDialogControl : RemoteUserControl
{
```

--------------------------------

### Reference Visualizer Object Source by Assembly-Qualified Name (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Shows how to configure the DebuggerVisualizerProviderConfiguration to reference the visualizer object source type using its full assembly-qualified name. This method is used when the extension project does not have a direct assembly dependency on the object source project.

```C#
    public override DebuggerVisualizerProviderConfiguration DebuggerVisualizerProviderConfiguration => new("Regex Match visualizer", typeof(Match))
    {
        VisualizerObjectSourceType = new("Microsoft.VisualStudio.Gladstone.RegexMatchVisualizer.ObjectSource.RegexMatchObjectSource, RegexMatchObjectSource"),
    };
```

--------------------------------

### Recommended ModifierKey.ControlShift (Commands)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

A recommended alternative modifier key combination to use instead of ModifierKey.Shift for command shortcuts.

```C#
ModifierKey.ControlShift
```

--------------------------------

### Recommended ModifierKey.ControlShiftLeftAlt (Commands)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Another recommended alternative modifier key combination to use instead of ModifierKey.Shift for command shortcuts.

```C#
ModifierKey.ControlShiftLeftAlt
```

--------------------------------

### Querying Specific Output Groups By Name (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Demonstrates using OutputGroupsByName to filter output groups by specific names. This allows retrieving information only for the requested output groups, skipping any names that don't correspond to valid groups.

```csharp
var result = await querySpace.QueryProjectsAsync(
	project => project.With(p => p.Name)
		.With(p => p.ActiveConfigurations
		.With(c => c.Name)
		.With(c => c.OutputGroupsByName("Built", "XmlSerializer", "SourceFiles", "RandomNameShouldntBePickedUp")
		.With(g => g.Name))),
	cancellationToken);
```

--------------------------------

### Defining Localizable String Resources (JSON)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommentRemover/README.md

This JSON snippet shows the structure of a `string-resources.json` file used for localization in Visual Studio extensions. Keys are identifiers referenced in code (e.g., `%CommentRemover.Comments.DisplayName%`), and values are the default strings.

```json
{
  "CommentRemover.CommentRemoverMenu.DisplayName": "Comments",
  "CommentRemover.RemoveAllComments.DisplayName": "Remove All",
  "CommentRemover.RemoveAllExceptTaskComments.DisplayName": "Remove All Except Tasks",
  "CommentRemover.RemoveAllExceptXmlDocComments.DisplayName": "Remove All Except Xml Docs",
  "CommentRemover.RemoveRegions.DisplayName": "Remove Regions",
  "CommentRemover.RemoveTasks.DisplayName": "Remove Tasks",
  "CommentRemover.RemoveXmlDocComments.DisplayName": "Remove Xml Docs"
}
```

--------------------------------

### Disable Experimental API Warning in Project File

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/experimental_apis.md

This XML snippet adds a `<NoWarn>` element to the `.csproj` file to disable the build error for a specific experimental API (VSEXTPREVIEW_OUTPUTWINDOW) across the entire project. This is an alternative to disabling the warning in individual source files.

```XML
<NoWarn>$(NoWarn);VSEXTPREVIEW_OUTPUTWINDOW</NoWarn>
```

--------------------------------

### Setting Out-of-Proc Assembly Subpath in MSBuild XML

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

This XML snippet, added to the out-of-proc component's .csproj file, specifies a subfolder ('OutOfProc') within the final VSIX where the out-of-proc component's assemblies and dependencies will be placed. This helps avoid potential dependency conflicts with the in-proc component.

```XML
<AssemblyVSIXSubPath>OutOfProc</AssemblyVSIXSubPath>
```

--------------------------------

### Including Brokered Service Interface in In-Proc Component (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

Configures the project file to include the IOutOfProcService.cs file from the out-of-proc component project, making the interface definition available for compilation within the in-proc component. This allows the in-proc component to reference the interface type when consuming the brokered service proxy.

```XML
<ItemGroup>
  <Compile Include="..\OutOfProcComponent\IOutOfProcService.cs" Link="IOutOfProcService.cs" />
</ItemGroup>
```

--------------------------------

### Configuring XAML as Embedded Resource in MSBuild (.csproj)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

This XML snippet from a .csproj file shows how to configure a XAML file located in a subfolder as an embedded resource. It uses the LogicalName property to ensure the resource name matches the full name of the remote user control class, which is required for remote UI.

```xml
<ItemGroup>
    <Page Remove="RegexMatchVisualizerUserControl.xaml" />
    <EmbeddedResource Include="RegexMatch\RegexMatchVisualizerUserControl.xaml" LogicalName="$(RootNamespace).RegexMatchVisualizerUserControl.xaml" />
  </ItemGroup>
```

--------------------------------

### Asynchronously Loading MatchCollection Data in C# Debugger Visualizer UI

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Executed when the remote user control is loaded, this code asynchronously requests data from the visualizer object source using `RequestDataAsync` with an index as the message. It loops, incrementing the index, until the object source returns null, indicating the end of the collection.

```csharp
public override Task ControlLoadedAsync(CancellationToken cancellationToken)
{
    _ = Task.Run(async () =>
    {
        for (int i = 0; ; i++)
        {
            RegexMatch? regexMatch = await this.visualizerTarget.ObjectSource.RequestDataAsync<int, RegexMatch?>(message: i, jsonSerializer: null, CancellationToken.None);
            if (regexMatch is null)
            {
                break;
            }

            this.RegexMatches.Add(regexMatch);
        }
    });

    return Task.CompletedTask;
}
```

--------------------------------

### Obsolete SetBuildPropertyValue Method (Project Query)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

This method for setting build properties via the Project Query API has been marked as obsolete and is scheduled for future removal.

```C#
Microsoft.VisualStudio.ProjectSystem.Query.UpdateExtensions.SetBuildPropertyValue(this IAsyncUpdatable<IProjectConfigurationSnapshot> configurations, string name, string value, string storageType)
```

--------------------------------

### Track Project File Updates - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Illustrates how to track updates to the files within a project. It accesses the Files property of a project snapshot and calls TrackUpdatesAsync, providing an observer to receive change notifications.

```csharp
var unsubscriber = await singleProject
    .Files
    .With(f => f.FileName)
    .TrackUpdatesAsync(new TrackerObserver(), CancellationToken.None);
```

--------------------------------

### Configure TextViewMarginProviderConfiguration Placement (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/WordCountMargin/README.md

This C# snippet defines the TextViewMarginProviderConfiguration for the word count margin. It specifies that the margin should be placed in the BottomRightCorner container and positioned Before (to the left of) the built-in Visual Studio RowMargin (line number margin) using the Before property.

```csharp
/// <summary>
/// Configures the margin to be placed to the left of built-in Visual Studio line number margin.
/// </summary>
public TextViewMarginProviderConfiguration TextViewMarginProviderConfiguration =>
    new(marginContainer: ContainerMarginPlacement.KnownValues.BottomRightCorner)
    {
        Before = [MarginPlacement.KnownValues.RowMargin],
    };
```

--------------------------------

### Defining VSProjectContext Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the structure representing a project context within Visual Studio. It includes properties for a user-friendly label, a unique identifier, and a kind to determine its associated icon.

```typescript
export interface VSProjectContext {

    /**
     * Gets or sets the label for the project context.
    **/
    _vs_label : string,

    /**
     * Gets or sets the unique identifier of the project context.
    **/
    _vs_id : string,

    /**
     * Gets or sets the context kind of the project context which is used to determine its associated icon.
    **/
    _vs_kind : VSProjectKind,
}
```

--------------------------------

### Link Shared Source Files in VS Extension (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Configures the .csproj file to link source files (.cs) from the object source project into the extension project. This is necessary when avoiding a direct assembly dependency to share code definitions like RegexCapture, RegexGroup, and RegexMatch.

```XML
  <ItemGroup>
    <Compile Include="..\RegexMatchObjectSource\RegexGroup.cs" Link="SharedFiles\RegexGroup.cs" />
    <Compile Include="..\RegexMatchObjectSource\RegexCapture.cs" Link="SharedFiles\RegexCapture.cs" />
    <Compile Include="..\RegexMatchObjectSource\RegexMatch.cs" Link="SharedFiles\RegexMatch.cs" />
  </ItemGroup>
```

--------------------------------

### Extending LSP ServerCapabilities with VSServerCapabilities Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines a Visual Studio-specific extension to the standard LSP `ServerCapabilities` type. It allows a language server to indicate support for VS-specific features, such as providing project context information.

```typescript
export interface VSServerCapabilities extends ServerCapabilities {

    /**
     * Gets or sets a value indicating whether the server supports the
     * 'textDocument/_vs_getProjectContexts' request.
    **/
    _vs_projectContextProvider? : boolean,
}
```

--------------------------------

### Disable Experimental API Warning in C# Source

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/experimental_apis.md

This C# `#pragma` statement disables the build error for a specific experimental API (VSEXTPREVIEW_OUTPUTWINDOW) within the file where it is placed. It allows the use of the experimental feature while acknowledging its potential for future changes.

```C#
#pragma warning disable VSEXTPREVIEW_OUTPUTWINDOW // Type is for evaluation purposes only and is subject to change or removal in future updates.
```

--------------------------------

### Proffer Brokered Service in VS Extensibility Out-of-Proc Component (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

Registers a brokered service implementation (`OutOfProcService`) with the service collection in the out-of-proc component's Extension class. This makes the service available for consumption by other components (like the in-proc component) and also allows dependency injection within the out-of-proc component itself.

```csharp
protected override void InitializeServices(IServiceCollection serviceCollection)
{
    serviceCollection.ProfferBrokeredService<OutOfProcService>();
    base.InitializeServices(serviceCollection);
}
```

--------------------------------

### Defining VSMethods Namespace for LSP Extensions in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

A namespace containing string constants for Visual Studio-specific Language Server Protocol method names. This provides a centralized place to reference method names like 'textDocument/_vs_getProjectContexts'.

```typescript
export namespace VSMethods {

    /**
     * Method name for 'textDocument/_vs_getProjectContexts'.
     * The 'textDocument/_vs_getProjectContexts' request is sent from the client to the server to query
     * the list of project context associated with a document.
     * This method has a parameter of type VSGetProjectContextsParams and a return value of type
     * VSProjectContextList.
     * In order to enable the client to send the 'textDocument/_vs_getProjectContexts' requests, the server must
     * set the VSServerCapabilities._vs_projectContextProvider property.
    **/
    export const GetProjectContextsName : string = 'textDocument/_vs_getProjectContexts';
}
```

--------------------------------

### Specify VSIX Subpath for Extension Assembly (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Adds the `AssemblyVSIXSubPath` property to the extension's .csproj file. This property specifies a subfolder within the final VSIX package where the extension's assembly and its dependencies will be placed, helping to avoid dependency conflicts with the container project.

```XML
<AssemblyVSIXSubPath>OutOfProc</AssemblyVSIXSubPath>
```

--------------------------------

### Adding a Classification Tag (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ClassificationSample/README.md

This snippet demonstrates how to create and add a ClassificationTag to a list of tags. It associates a specific text range (TrackingTextRange) with a classification type (ClassificationType.KnownValues.Operator), used by a tagger to highlight or style text in the editor.

```C#
tags.Add(
    new TaggedTrackingTextRange<ClassificationTag>(
        new TrackingTextRange(
            document,
            tagStartPosition,
            tagLength,
            TextRangeTrackingMode.ExtendNone),
        new ClassificationTag(ClassificationType.KnownValues.Operator)));
```

--------------------------------

### Defining the VSGetProjectContextsParams Interface (TypeScript)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the `VSGetProjectContextsParams` interface, which specifies the structure of the parameters sent with the `textDocument/_vs_getProjectContexts` request, primarily containing the `TextDocumentItem` for which project contexts are requested.

```typescript
export interface VSGetProjectContextsParams {

    /**
     * Gets or sets the document for which project contexts are queried.
    **/
    _vs_textDocument : TextDocumentItem,
}
```

--------------------------------

### Extending LSP Location with VSLocation Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines a Visual Studio-specific extension to the standard LSP `Location` type. It adds optional properties for the project name and a friendly display path, useful for presenting location information in the VS UI.

```typescript
export interface VSLocation extends Location {

    /**
     * Gets or sets the project name to be displayed to user.
    **/
    _vs_projectName? : string,

    /**
     * Gets or sets the text value for the display path.
     * In case the actual path on disk would be confusing for users, this should be a friendly display name.
     * This doesn't have to correspond to a real file path, but must be parsable by the System.IO.Path.GetFileName(System.String) method.
    **/
    _vs_displayPath? : string,
}
```

--------------------------------

### Implementing GetData Method for RegexMatchObjectSource (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

Overrides the `GetData` method from `VisualizerObjectSource` to handle the target object. It checks if the target is a `Match`, converts it to a `RegexMatch`, and serializes the result as JSON to the `outgoingData` stream using `SerializeAsJson`.

```csharp
public class RegexMatchObjectSource : VisualizerObjectSource
{
    public override void GetData(object target, Stream outgoingData)
    {
        if (target is Match match)
        {
            RegexMatch result = Convert(match);
            SerializeAsJson(outgoingData, result);
        }
    }

    ...
}
```

--------------------------------

### Referencing Visual Studio Types in WPF XAML (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Adds XML namespace declarations to a WPF XAML file to allow referencing types from Visual Studio assemblies like Microsoft.VisualStudio.Shell.15.0 for styles and colors.

```XML
xmlns:styles="clr-namespace:Microsoft.VisualStudio.Shell;assembly=Microsoft.VisualStudio.Shell.15.0"
xmlns:colors="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0"
```

--------------------------------

### Configure TextViewExtension for Code Documents (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CodeLensSample/README.md

Configures the TextViewExtension to apply only to documents identified as code, using the `DocumentFilter.FromDocumentType` method with `DocumentType.KnownValues.Code`. This ensures the Code Lens provider is active only in relevant text views.

```csharp
/// <inheritdoc />
public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
{
    AppliesTo = new[]
    {
        DocumentFilter.FromDocumentType(DocumentType.KnownValues.Code),
    },
};
```

--------------------------------

### LSP Extensions NuGet Package Reference

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Reference to the Microsoft.VisualStudio.LanguageServer.Protocol.Extensions NuGet package, which needs to be recompiled against version 17.10 or newer due to changes in the VSDiagnosticTags enum.

```C#
Microsoft.VisualStudio.LanguageServer.Protocol.Extensions package
```

--------------------------------

### Obsolete BuildPropertiesByName Method (Project Query)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

This method for filtering build properties by name via the Project Query API has been marked as obsolete and is scheduled for future removal.

```C#
Microsoft.VisualStudio.ProjectSystem.Query.ProjectConfigurationPropertiesFilterExtensions.BuildPropertiesByName(this IProjectConfigurationSnapshot projectConfiguration, string storageType, params string[] buildPropertyNames)
```

--------------------------------

### Defining VSProjectContextList Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the structure for the response to the 'textDocument/_vs_getProjectContexts' request. It contains an array of `VSProjectContext` objects representing the document contexts and the index of the default context.

```typescript
export interface VSProjectContextList {

    /**
     * Gets or sets the document contexts associated with a text document.
    **/
    _vs_projectContexts : VSProjectContext[],

    /**
     * Gets or sets the index of the default entry of the VSProjectContext array.
    **/
    _vs_defaultIndex : integer,
}
```

--------------------------------

### Extending LSP SymbolInformation with VSSymbolInformation Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines a Visual Studio-specific extension to the standard LSP `SymbolInformation` type. It adds optional properties for an associated icon (using `VSImageId`), a description, and hint text to enhance symbol display in the VS UI.

```typescript
export interface VSSymbolInformation extends SymbolInformation {

    /**
     * Gets or sets the icon associated with the symbol. If specified, this icon is used instead of SymbolKind.
    **/
    _vs_icon? : VSImageId,

    /**
     * Gets or sets the description of the symbol.
    **/
    _vs_description? : string,

    /**
     * Gets or sets the hint text for the symbol.
    **/
    _vs_hintText? : string,
}
```

--------------------------------

### Disable VSIX Creation in Extension CSPROJ (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Adds the `CreateVsixContainer` property to the extension's .csproj file and sets it to `false`. This prevents the project from creating its own VSIX package, as it will be packaged by the container project.

```XML
<CreateVsixContainer>false</CreateVsixContainer>
```

--------------------------------

### Transferring Data Incrementally in C# Debugger Visualizer Object Source

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

This method is used instead of `GetData` when the target object might be large. It accepts an index via `incomingData` and serializes a single element from the `MatchCollection` at that index to `outgoingData`, allowing the visualizer UI to request data incrementally.

```csharp
public override void TransferData(object target, Stream incomingData, Stream outgoingData)
{
    var index = (int)DeserializeFromJson(incomingData, typeof(int))!;
    if (target is MatchCollection matchCollection && index < matchCollection.Count)
    {
        var result = RegexMatchObjectSource.Convert(matchCollection[index]);
        result.Name = $"[{index}]";
        SerializeAsJson(outgoingData, result);
    }
    else
    {
        SerializeAsJson(outgoingData, null);
    }
}
```

--------------------------------

### Rename File in Project - C#

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/VSProjectQueryAPISample/README.md

Shows how to rename a file within a specific project. It uses UpdateProjectsAsync to target the project by name and then calls RenameFile with the original file path and the new file name.

```csharp
var result =  await querySpace.UpdateProjectsAsync(
                project => project.Where(project => project.Name == "ConsoleApp1"),
                project => project.RenameFile(filePath, "newName.cs"),
                cancellationToken);
```

--------------------------------

### Calling Old GetChannelAsync Method (VS Extensibility)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Identifies code that calls the previous method for obtaining an output channel, indicating potential impact from the API redesign.

```C#
await Extensibility.Views().Output.GetChannelAsync(…)
```

--------------------------------

### Setting Custom Command Icon (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CommentRemover/README.md

This C# property snippet shows how to configure a custom icon for a command in VisualStudio.Extensibility. It uses `ImageMoniker.Custom` with a string identifier ('DeleteRegions') that corresponds to image files (PNG/XAML) placed in the 'Images' folder.

```csharp
public override CommandConfiguration CommandConfiguration => new(CommandDescription)
{
    Icon = new(ImageMoniker.Custom("DeleteRegions"), IconSettings.IconAndText),
    EnabledWhen = CommandEnabledWhen,
};
```

--------------------------------

### Reference to Old Output Window Type (VS Extensibility)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Identifies code that references the previous type used for interacting with the Output Window, indicating potential impact from the API redesign.

```C#
Microsoft.VisualStudio.Extensibility.Documents.OutputWindow
```

--------------------------------

### Merge Dependent extension.json in VS Extensibility (XML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

Configures the .csproj file to include a dependent extension.json file from another project (like an out-of-proc component) for merging during the build process. This ensures the composite VSIX includes contributions from the out-of-proc part. Adjust the path to match your project structure.

```xml
<ItemGroup>
  <DependentExtensionJson Include="$(BaseOutputPath)..\OutOfProcComponent\$(Configuration)\net8.0-windows8.0\.vsextension\extension.json" />
</ItemGroup>
```

--------------------------------

### Configure TextViewExtensionConfiguration for Text Documents (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/WordCountMargin/README.md

This C# snippet shows the configuration for the TextViewExtensionConfiguration property within the TextViewMarginProvider. It uses the AppliesTo property with a DocumentFilter to specify that the extension should only be active for documents identified as text-based (DocumentType.KnownValues.Text).

```csharp
/// <inheritdoc />
public TextViewExtensionConfiguration TextViewExtensionConfiguration => new()
{
    AppliesTo =
    [
        DocumentFilter.FromDocumentType(DocumentType.KnownValues.Text),
    ],
};
```

--------------------------------

### Obsolete Delete Build Property Method (Project Query)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

This method for deleting build properties via the Project Query API has been marked as obsolete and is scheduled for future removal.

```C#
Microsoft.VisualStudio.ProjectSystem.Query.UpdateExtensions.Delete(this IAsyncUpdatable<IBuildPropertySnapshot> buildProperties)
```

--------------------------------

### Defining the VSDiagnosticProjectInformation Interface (TypeScript)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the `VSDiagnosticProjectInformation` interface, used within `VSDiagnostic` to specify the project and build context where a diagnostic was generated, including human-readable and unique project identifiers.

```typescript
export interface VSDiagnosticProjectInformation {

    /**
     * Gets or sets a human-readable identifier for the project in which the diagnostic was generated.
    **/
    _vs_projectName? : string,

    /**
     * Gets or sets a human-readable identifier for the build context (e.g. Win32 or MacOS)
     * in which the diagnostic was generated.
    **/
    _vs_context? : string,

    /**
     * Gets or sets the unique identifier for the project in which the diagnostic was generated.
    **/
    _vs_projectIdentifier? : string,
}
```

--------------------------------

### Defining the VSDiagnostic Interface (TypeScript)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the `VSDiagnostic` interface, extending the standard `Diagnostic` with Visual Studio-specific properties like project information, expanded message, tooltip text, identifier, diagnostic type, rank, and output ID.

```typescript
export interface VSDiagnostic extends Diagnostic {

    /**
     * Gets or sets the project and context (e.g. Win32, MacOS, etc.) in which the diagnostic was generated.
    **/
    _vs_projects? : VSDiagnosticProjectInformation[],

    /**
     * Gets or sets an expanded description of the diagnostic.
    **/
    _vs_expandedMessage? : string,

    /**
     * Gets or sets a message shown when the user hovers over an error. If null, then Diagnostic.message
     * is used (use VSDiagnosticTags.SuppressEditorToolTip to prevent a tool tip from being shown).
    **/
    _vs_toolTip? : string,

    /**
     * Gets or sets a non-human-readable identier allowing consolidation of multiple equivalent diagnostics
     * (e.g. the same syntax error from builds targeting different platforms).
    **/
    _vs_identifier? : string,

    /**
     * Gets or sets a string describing the diagnostic types (e.g. Security, Performance, Style, etc.).
    **/
    _vs_diagnosticType? : string,

    /**
     * Gets or sets a rank associated with this diagnostic, used for the default sort.
     * VSDiagnosticRank.Default will be used if no rank is specified.
    **/
    _vs_diagnosticRank? : VSDiagnosticRank,

    /**
     * Gets or sets an ID used to associate this diagnostic with a corresponding line in the output window.
    **/
    _vs_outputId? : integer,
}
```

--------------------------------

### Defining VSTextDocumentIdentifier Interface in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

This snippet defines the TypeScript interface `VSTextDocumentIdentifier`. It extends the standard `TextDocumentIdentifier` from the Language Server Protocol and adds an optional property `_vs_projectContext` of type `VSProjectContext` to provide project context for the document.

```typescript
export interface VSTextDocumentIdentifier extends TextDocumentIdentifier {

    /**
     * Gets or sets the project context of the text document.
    **/
    _vs_projectContext? : VSProjectContext,
}
```

--------------------------------

### Updating Data Request for Serializable RegexMatch (C#)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/RegexMatchDebugVisualizer/README.md

This C# line updates the data retrieval call within the visualizer provider. It changes the target type from the non-serializable Match to a custom serializable RegexMatch class, which is necessary for transferring the data.

```csharp
var regexMatch = await visualizerTarget.ObjectSource.RequestDataAsync<RegexMatch>(jsonSerializer: null, cancellationToken);
```

--------------------------------

### Referencing WPF User Control in Remote UI XAML (XAML)

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/ExtensionWithTraditionalComponents/README.md

Demonstrates how to reference a WPF user control from the Container assembly within a Remote UI XAML DataTemplate. Requires a fully qualified assembly name and a strong-named assembly for the clr-namespace declaration.

```XAML
<DataTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
              xmlns:container="clr-namespace:Container;assembly=Container, Version=1.0.0.0, Culture=neutral, PublicKeyToken=ebbd189e9e224069">
    <!-- The xmlns:container above must use the fully qualified name of the assembly and the assembly must be strong named!  -->
    <container:MyUserControl />
</DataTemplate>
```

--------------------------------

### ClientContexts Property Type Change (Commands)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Code illustrating the change in the ClientContexts property type on BaseCommandConfiguration from string[]? to ClientContextCategory[]?.

```C#
/// <summary>
/// Gets or sets the client context categories requested by the command.
/// </summary>
-    public string[]? ClientContexts { get; set; }
+    public ClientContextCategory[]? ClientContexts { get; set; }
```

--------------------------------

### Disabling VSIX Creation for Out-of-Proc Component in MSBuild XML

Source: https://github.com/microsoft/vsextensibility/blob/main/New_Extensibility_Model/Samples/CompositeExtension/README.md

This XML snippet, added to the out-of-proc component's .csproj file, prevents the project from generating its own VSIX package. This is necessary because the out-of-proc component will be packaged as part of the in-proc component's VSIX.

```XML
<CreateVsixContainer>false</CreateVsixContainer>
```

--------------------------------

### Defining VSDiagnosticTags Namespace (TypeScript)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the `VSDiagnosticTags` namespace, containing Visual Studio-specific `DiagnosticTag` values used to categorize diagnostics based on their origin (Build, Intellisense), visibility (HiddenInErrorList, VisibleInErrorList, HiddenInEditor), tooltip behavior (SuppressEditorToolTip), or special handling (PotentialDuplicate, EditAndContinueError).

```typescript
export namespace VSDiagnosticTags {

    /**
     * A Diagnostic entry generated by the build.
    **/
    export const BuildError : DiagnosticTag = -1;

    /**
     * A Diagnostic entry generated by Intellisense.
    **/
    export const IntellisenseError : DiagnosticTag = -2;

    /**
     * A Diagnostic entry that could be generated from both builds
     * and Intellisense.
     * 
     * Diagnostic entries tagged with VSDiagnosticTags.PotentialDuplicate will be hidden
     * in the error list if the error list is displaying build and intellisense
     * errors.
    **/
    export const PotentialDuplicate : DiagnosticTag = -3;

    /**
     * A Diagnostic entry is never displayed in the error list.
    **/
    export const HiddenInErrorList : DiagnosticTag = -4;

    /**
     * The Diagnostic entry is always displayed in the error list.
    **/
    export const VisibleInErrorList : DiagnosticTag = -5;

    /**
     * The Diagnostic entry is never displayed in the editor.
    **/
    export const HiddenInEditor : DiagnosticTag = -6;

    /**
     * No tooltip is shown for the Diagnostic entry in the editor.
    **/
    export const SuppressEditorToolTip : DiagnosticTag = -7;

    /**
     * The Diagnostic entry is represented in the Editor as an Edit
     * and Continue error.
    **/
    export const EditAndContinueError : DiagnosticTag = -8;
}
```

--------------------------------

### BaseCommandConfiguration Type (Commands)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Reference to the BaseCommandConfiguration type, whose ClientContexts property type has changed.

```C#
Microsft.VisualStudio.Extensibility.Commands.BaseCommandConfiguration
```

--------------------------------

### Defining VSProjectKind Enum in TypeScript

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines an enumeration for different kinds of Visual Studio project contexts. This allows associating specific icons or behaviors with different project types like C++, C#, and Visual Basic.

```typescript
export enum VSProjectKind {

    /**
     * C++ project.
    **/
    CPlusPlus = 1,

    /**
     * C# project.
    **/
    CSharp = 2,

    /**
     * Visual Basic project.
    **/
    VisualBasic = 3,
}
```

--------------------------------

### VSDiagnosticTags Enum (LSP)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Reference to the VSDiagnosticTags enum, which contains Visual Studio-specific diagnostic tags used in the Language Server Protocol.

```C#
VSDiagnosticTags
```

--------------------------------

### Defining the VSDiagnosticRank Enum (TypeScript)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/lsp/lsp-extensions-specifications.md

Defines the `VSDiagnosticRank` enumeration, providing predefined integer values to represent the priority or rank of a `VSDiagnostic` object for sorting purposes, ranging from `Highest` to `Lowest`.

```typescript
export enum VSDiagnosticRank {

    /**
     * Highest priority.
    **/
    Highest = 100,

    /**
     * High priority.
    **/
    High = 200,

    /**
     * Default priority.
    **/
    Default = 300,

    /**
     * Low priority.
    **/
    Low = 400,

    /**
     * Lowest priority.
    **/
    Lowest = 500,
}
```

--------------------------------

### ModifierKey Enum (Commands)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

Reference to the ModifierKey enum used to define modifier keys for command shortcuts.

```C#
ModifierKey
```

--------------------------------

### Removed ModifierKey.Shift Value (Commands)

Source: https://github.com/microsoft/vsextensibility/blob/main/docs/breaking_changes.md

The ModifierKey.Shift enum value has been removed because Shift alone is not a valid modifier key for Visual Studio command shortcuts.

```C#
ModifierKey.Shift
```