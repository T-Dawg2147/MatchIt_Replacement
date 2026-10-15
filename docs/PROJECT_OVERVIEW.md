# MatchIt Replacement: Project and System Guide

This repository contains three .NET projects:

| Project | Role | Target |
|---|---|---|
| `MatchIt.MatchReplacement.Core` | Data import, normalization, matching, processing pipeline, diagnostics, and export services | .NET 10 class library |
| `MatchIt.MatchReplacement.Host` | Command-line entry point that runs Core and prints results | .NET 10 console application |
| `MatchIt.MatchReplacement.Wpf` | Windows desktop interface for selecting inputs, running a process, and previewing an output | .NET 10 WPF application for Windows |

The Host and WPF projects both reference Core. Core has no dependency on either front end. The repository currently includes no tests or solution file.

## System at a glance

Both front ends call `IMatchReplacementService.RunAsync`. The service validates the request, resolves a named profile, creates a per-run context, then gives the profile's stage definition to `ProcessEngine`. The engine runs stages sequentially, reports start/completion/failure events to an optional progress observer, and propagates failures to the caller. Core returns a response with status, metrics, diagnostics counts, and the named output files.

The only built-in profile is **Monthly Numbers**. It creates this nine-stage pipeline:

1. **Load Inputs** reads suppressions and contacts from positional pipe-delimited files without headers.
2. **Normalize Records** populates normalized values and match keys on both record types.
3. **Suppression Internal Analysis** finds potential duplicate suppression pairs for diagnostics; it does not remove suppressions.
4. **Contact Internal Analysis** finds potential duplicate contact pairs for diagnostics; it does not remove contacts.
5. **Suppression Overlap Purge** finds contact/suppression overlaps, records match details, and removes matched contacts from the downstream contact set.
6. **Site Level Dedupe** finds duplicate contact groups in the remaining set, records diagnostics, and retains a representative record per group.
7. **Export Outputs** writes a suppression snapshot and the suppression-cleaned and site-deduplicated contact outputs.
8. **Export Diagnostics** writes the four diagnostic files.
9. **Print Summary** prints run metrics and the named-output map.

Stages receive a shared `ProcessRunContext`. It holds input paths, configuration, records, intermediate collections, diagnostic rows, metrics, export settings, progress reporting, and output paths. A custom stage can read and update this context; a custom pipeline is assembled in the profile.

## Core project

Core separates application APIs, process orchestration, matching, data processing, I/O, and reporting. Its only explicit NuGet dependency is CsvHelper 33.0.1.

### Inputs and record mapping

`CsvReaderService` reads files with `|` as delimiter, no header row, blank-line ignoring, and whitespace trimming. A contact row is mapped by column position to 27 fields, in this order: `Cid`, `AccNo`, `Office`, `Prefix`, `Forenames`, `Surname`, `JobTitle`, `Dept`, `Company`, `Address1`–`Address5`, `Postcode`, `Email`, `CCreated`, `LastTypeCr`, `LastDateCr`, `CRecency`, `Bdm`, `Hierarchy`, `RollupLoy`, `RollupAcc`, `ActType`, `AccSupp`, and `Rem`. A suppression row is mapped to 10 fields: `Other1`, `Company`, `Address1`–`Address5`, `Postcode`, `Other2`, and `Other3`.

The reader does not validate business-required columns or reject short rows: an unavailable field is returned as `null`. The input files therefore need the expected ordering and delimiter, but they must not include a header row. Extra fields are not mapped.

### Normalization and keys

The normalization stage composes `TextNormalizer`, `PostcodeNormalizer`, `CompanyNormalizer`, `AddressNormalizer`, and `PhoneticEncoder` through `KeyBuilder`. It derives values used by the matchers, including normalized and outward postcodes, normalized company/address values, phonetic forms, and composite address keys.

`DefaultBuiltInKeyCatalog` describes the keys exposed through the authoring query API:

| Key ID | Meaning | Applies to |
|---|---|---|
| `normalized-postcode` | Normalized full postcode | Contacts and suppressions |
| `outward-postcode` | Outward postcode segment | Contacts and suppressions |
| `surname-phonetic` | Phonetic surname | Contacts |
| `company-first-word-phonetic` | Phonetic first company-name token | Contacts and suppressions |
| `street-phonetic` | Phonetic address/street text | Contacts and suppressions |
| `address-phonetic-key` | Composite phonetic address key | Contacts and suppressions |
| `add-key` | Compact normalized address key | Contacts and suppressions |
| `normalized-company` | Normalized company name | Contacts and suppressions |

These catalog entries describe available keys; the current profile's stages and matchers perform their own fixed operations. The key lists in editable-profile DTOs do not dynamically reconfigure matching.

### Matching and processing semantics

Matchers build candidate pairs/groups and attach scores and reasons to matches. The configured thresholds decide which candidate matches are accepted. Default weights and thresholds are defined in `ScoringWeights` and `MatchingThresholds`; Monthly Numbers explicitly sets the five thresholds below and uses default weights:

| Configuration | Monthly Numbers |
|---|---:|
| Suppression overlap threshold | 75 |
| Strong suppression overlap threshold | 90 |
| Site dedupe threshold | 70 |
| Contact dedupe threshold | 65 |
| Suppression internal dedupe threshold | 70 |

The default common weights are exact postcode 35, outward postcode 15, ADD_KEY 30, normalized street 10, street phonetic 12, and address phonetic key 18. Business weights are normalized company 25, company-first-word phonetic 12, company/street phonetic combination 35, and outward-postcode/company-phonetic combination 45. Person weights are exact email 20, surname phonetic 12, surname/street phonetic combination 45, and outward-postcode/surname-phonetic combination 45.

The current matching classes are:

- **Contact internal duplicates:** `ContactDedupMatcher` compares contact records using postcode, person, email, company, and street evidence against the contact threshold. The analysis stage produces diagnostic rows only.
- **Suppression internal duplicates:** `SuppressionInternalDuplicateMatcher` compares suppressions using postcode, company/address, and ADD_KEY evidence against its threshold. This stage also produces diagnostic rows only.
- **Suppression overlap:** `SuppressionOverlapMatcher` compares contacts to suppressions using postcode, company/address, and ADD_KEY evidence. With the default `RequirePostcodeFamilyForSuppressionOverlap` setting, candidates must share a full or outward postcode family. The best qualifying match per contact is recorded, and matched contacts are removed from the active contact set.
- **Site dedupe:** `SiteDedupMatcher` compares the post-suppression contacts using postcode, address, and company evidence. Qualifying pairs form connected groups; `SiteLevelActiveProcessor` selects the retained representative and marks other records as removed/duplicate candidates.

The full score construction and candidate bucketing rules are implemented by the matcher classes; threshold and weight values are configuration, not editable-profile DTO settings. `PreferExactPostcodeMatches` is part of configuration and is enabled by default.

### Outputs and formats

For the built-in pipeline, output paths are created in the requested output folder. The export-format extension is `.csv` for CSV, `.txt` for pipe-delimited, and `.json` for JSON; an unknown enum value falls back to `.dat`.

| Logical output name | File stem | Contents |
|---|---|---|
| `SuppressionSnapshot` | `suppressions.normalized.snapshot` | Imported suppression records, including normalized properties populated before export |
| `Output2` | `2 LAST DATE CR` | Contacts after suppression-overlap removal |
| `Output3` | `3 SITE LEVEL DATA ACTIVE` | Site-level active contacts after deduplication |
| `DiagSuppressionInternal` | `diag.suppression.internal.duplicates` | Potential suppression-to-suppression matches |
| `DiagContactInternal` | `diag.contact.internal.duplicates` | Potential contact-to-contact matches |
| `DiagSuppressionOverlap` | `diag.suppression.overlaps` | Contact-to-suppression matches used by the purge |
| `DiagSiteDedup` | `diag.site.dedup` | Potential site-level duplicate matches |

`Output2` and `Output3` are written through `ContactExportProjection`, which exports the 27 contact source fields using lower-case/snake-case column names and does not include the extra normalized or match fields. The suppression snapshot and diagnostics are serialized from their respective records/rows. Export providers support comma-separated CSV, pipe-delimited text, and JSON. `ExportOptions` controls header inclusion and quoting; both front ends currently request headers and quote-all-fields.

### Public application and extension APIs

- `IMatchReplacementService` / `MatchReplacementService` execute a named profile. `RunProcessRequest` carries the profile name, contacts path, suppression path, output folder, export options, and observer. `RunProcessResponse` contains success/status, metrics, diagnostic counts, and output file name/path pairs.
- `IMatchReplacementQueryService` lists profiles and retrieves a profile descriptor.
- `IProfileAuthoringQueryService` returns an editable-profile DTO. The current default implementation supplies fixed stage/key descriptions and an empty custom-key list; it is a query only, not a persistence or profile-update API.
- `IKeyCatalogQueryService`, `IExportOptionsQueryService`, and `IExportMetadataQueryService` expose built-in keys and export-format information.
- `IResultPreviewService` previews rows from a generated CSV, pipe-delimited, or JSON file.
- `IProcessProgressObserver` receives timestamped stage updates. The null observer is the default; the delegating observer adapts callbacks.
- `IProcessProfile` supplies a name, match configuration, and process definition. `IProcessProfileMetadata` supplies a UI/query descriptor.
- `IProcessStage` supplies a stage name and asynchronous operation.
- `IExportProvider` writes records for an export format; the default resolver selects CSV, pipe-delimited, or JSON providers.
- Matching, normalizers, key catalog, and processing stages are also separable components; see the file map below.

`MatchReplacementService` validates nonempty profile/input/output values and that both input files exist, canonicalizes paths, creates the output directory, then executes the resolved profile. It does not catch stage exceptions or roll back files already written. The console host translates an uncaught exception to exit code 1; the WPF application displays it and marks the run failed.

### Core source map

| Folder | What it contains |
|---|---|
| `Application/` | Service interfaces/implementations, request/response DTOs, profile and export query services, progress contracts, preview services, and diagnostics summaries |
| `Config/` | Match thresholds, scoring weights, and profile configuration |
| `Diagnostics/` | Diagnostic row models and conversion from matcher results |
| `Engine/` | Stage contract, process definition, process engine, and run context |
| `Exports/` | Export-provider contract/resolver/writer, CSV/pipe/JSON providers, and export filename handling |
| `IO/` | Pipe-delimited input reader, CSV/diagnostic writers, contact export projection, and projection-based export |
| `Keys/` | Built-in key metadata contract/catalog |
| `Matching/Common/` | Shared score and match context |
| `Matching/ContactDedup/` | Contact-internal matching and match result |
| `Matching/SiteDedup/` | Site dedupe matching and match result |
| `Matching/Suppression/` | Suppression-internal and contact/suppression matchers and results |
| `Models/` | Contact/suppression records, active site record, match candidate/result, and duplicate group |
| `Normalization/` | Text, company, address, postcode, phonetic, and composite-key normalization |
| `Processing/` | Import analysis, suppression purge, and site-level active-record selection |
| `Profiles/` | Profile interfaces, profile resolver, stage type definitions, and Monthly Numbers implementation |
| `Reporting/` | Run metrics, process result, and metrics collection/proxy |
| `Stages/` | The nine built-in pipeline stage implementations |

## Host project

Host is a small console application with three source files:

- `Program.cs` parses arguments, invokes `HostRunner`, returns 0 on success, and prints an uncaught exception to standard error with exit code 1.
- `RunOptions.cs` expects `<contactsCsvPath> <suppressionCsvPath> <outputFolder> [processProfileName] [exportFormat]`. The first three arguments are required. Profile defaults to `Monthly Numbers`; format defaults to CSV. Format parsing is case-insensitive and an unrecognized format also falls back to CSV. Input paths are checked for existence and the output folder is created.
- `HostRunner.cs` constructs the Core services directly (no dependency-injection container), displays available profile/export/key metadata, runs the selected profile with progress printed to the console, then prints status, diagnostics, metrics, and output paths. If `Output2` exists, it previews up to five rows.

Example:

```text
MatchIt.MatchReplacement.Host <contacts.psv> <suppressions.psv> <output-folder> "Monthly Numbers" Json
```

Host references Core and does not define its own matching or file-processing behavior.

## WPF project

WPF is the Windows desktop front end. `App.xaml` starts `MainWindow`; the window constructs `MainWindowViewModel` as its data context. Its primary files are:

- `MainWindow.xaml`: window layout and bindings for profile selection, contacts/suppression paths, output folder, export format, run status/progress, output files, and preview grid.
- `MainWindow.xaml.cs`: window initialization and view-model hookup.
- `MainWindowViewModel.cs`: owns selection and form state, loads profile metadata, runs the Core service, logs progress, and requests previews.
- `RelayCommand.cs`: basic `ICommand` implementation with optional `CanExecute`.
- `UiOutputFile.cs`: UI display wrapper for Core output-file results.
- `App.xaml.cs` and `AssemblyInfo.cs`: application shell and WPF theme metadata.

The view-model instantiates Core services directly. Its browse commands select files/folders; Run checks that a profile and all three paths have been provided, clears previous results, and calls Core with CSV/pipe/JSON export options, headers enabled, and quote-all enabled. It shows process progress and completion/failure status, and lists generated files. Selecting an output and invoking preview asks Core to read up to 50 rows, which populate the data grid. The current UI does not edit or save profile definitions; the profile combo box selects from the profiles Core reports.

## Behaviors and boundaries to be aware of

- The project names and metadata describe a configurable profile model, but the current runtime has one built-in profile and the default resolver is where profiles are registered.
- `EditableProcessProfileDto` and its query service describe stages and keys; there is no save/update endpoint that turns edited DTOs into executable profiles.
- A profile's `CreateDefinition()` controls what actually executes. Skipping or reordering stages can leave context data unpopulated or outputs absent; stage dependencies are not automatically inferred.
- Output filenames are generated from the built-in stage stems and format. The descriptor's output metadata is descriptive and does not itself change the files produced.
- The process is sequential. Cancellation is checked by the engine between stages and passed to the stage operations; not every synchronous inner loop checks cancellation.
- Partial output files can remain if a later stage fails.
