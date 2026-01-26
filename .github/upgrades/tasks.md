# .NET 10 Upgrade - Execution Tasks

## Task Overview

**Scenario**: Upgrade Enigmatry Entry Building Blocks from .NET 9 to .NET 10.0 LTS  
**Strategy**: Bottom-Up Incremental Migration (Dependency-First)  
**Total Projects**: 43  
**Total Phases**: 6

---

## Phase 1: Foundation Establishment (Tier 1 - Level 0)

### [ ] TASK-001: Update Directory.Packages.props - DotnetVersion Property
**Priority**: CRITICAL - Blocks all Microsoft.* package updates  
**Scope**: Central package management

**Actions**:
- [ ] (1) Open `Directory.Packages.props`
- [ ] (2) Locate `<DotnetVersion>` property
- [ ] (3) Update value: `9.0.12` ? `10.0.2`
- [ ] (4) Save file
- [ ] (5) Verify: Property value is exactly `10.0.2`

**Validation**:
- File contains: `<DotnetVersion>10.0.2</DotnetVersion>`

---

### [ ] TASK-002: Remove Framework-Included Packages from Directory.Packages.props
**Priority**: HIGH - Required for Core and Randomness projects  
**Scope**: Central package management

**Actions**:
- [ ] (1) Open `Directory.Packages.props`
- [ ] (2) Remove line: `<PackageVersion Include="System.ComponentModel.Annotations" Version="5.0.0" />`
- [ ] (3) Remove line: `<PackageVersion Include="System.Security.Cryptography.Algorithms" Version="4.3.1" />`
- [ ] (4) Save file
- [ ] (5) Verify: Both package entries removed

**Validation**:
- File does NOT contain: `System.ComponentModel.Annotations`
- File does NOT contain: `System.Security.Cryptography.Algorithms`

---

### [ ] TASK-003: Upgrade Enigmatry.Entry.Core to .NET 10
**Priority**: CRITICAL - Foundation for 15 projects  
**Scope**: Single project (Core)

**Actions**:
- [ ] (1) Open `Enigmatry.Entry.Core\Enigmatry.Entry.Core.csproj`
- [ ] (2) Update `<TargetFramework>net9.0</TargetFramework>` ? `<TargetFramework>net10.0</TargetFramework>`
- [ ] (3) Remove `<PackageReference Include="System.ComponentModel.Annotations" />`
- [ ] (4) Save file
- [ ] (5) Build: `dotnet build Enigmatry.Entry.Core\Enigmatry.Entry.Core.csproj`
- [ ] (6) Verify: 0 errors, 0 warnings
- [ ] (7) Run tests: `dotnet test Enigmatry.Entry.Core.Tests\Enigmatry.Entry.Core.Tests.csproj`
- [ ] (8) Verify: All tests pass

**Validation**:
- Project file contains: `<TargetFramework>net10.0</TargetFramework>`
- Project file does NOT contain: `System.ComponentModel.Annotations`
- Build succeeds with 0 errors
- All Core.Tests pass

---

### [ ] TASK-004: Upgrade Tier 1 Independent Projects (Csv, MediatR, Swagger)
**Priority**: MEDIUM  
**Scope**: 3 projects (parallel-eligible)

**Actions**:
- [ ] (1) Update `Enigmatry.Entry.Csv\Enigmatry.Entry.Csv.csproj`:
  - Update `<TargetFramework>net10.0</TargetFramework>`
  - Save file
- [ ] (2) Update `Enigmatry.Entry.MediatR\Enigmatry.Entry.MediatR.csproj`:
  - Update `<TargetFramework>net10.0</TargetFramework>`
  - Save file
- [ ] (3) Update `Enigmatry.Entry.SwaggerSecurity\Enigmatry.Entry.Swagger.csproj`:
  - Update `<TargetFramework>net10.0</TargetFramework>`
  - Save file
- [ ] (4) Build all 3: `dotnet build <each-project-path>`
- [ ] (5) Verify: All 3 build successfully (0 errors)

**Validation**:
- All 3 projects target net10.0
- All 3 build without errors

---

### [ ] TASK-005: Upgrade Enigmatry.Entry.Randomness with Package Removal
**Priority**: MEDIUM  
**Scope**: Single project

**Actions**:
- [ ] (1) Open `Enigmatry.Entry.Randomness\Enigmatry.Entry.Randomness.csproj`
- [ ] (2) Update `<TargetFramework>net10.0</TargetFramework>`
- [ ] (3) Remove `<PackageReference Include="System.Security.Cryptography.Algorithms" />`
- [ ] (4) Save file
- [ ] (5) Build: `dotnet build Enigmatry.Entry.Randomness\Enigmatry.Entry.Randomness.csproj`
- [ ] (6) Verify: 0 errors (cryptography APIs work from framework)

**Validation**:
- Project targets net10.0
- Package reference removed
- Build succeeds

---

### [ ] TASK-006: Validate Phase 1 Completion
**Priority**: HIGH  
**Scope**: All Tier 1 projects

**Actions**:
- [ ] (1) Build all Tier 1 projects together
- [ ] (2) Run Tier 1 tests: Core.Tests, Csv.Tests, MediatR.Tests
- [ ] (3) Verify: All tests pass
- [ ] (4) Check for package conflicts: `dotnet list package`
- [ ] (5) Verify: No conflicts reported

**Validation**:
- All 5 Tier 1 projects build successfully
- All Tier 1 tests pass
- No package version conflicts

---

### [ ] TASK-007: Commit Phase 1 Changes
**Priority**: MEDIUM  
**Scope**: Source control

**Actions**:
- [ ] (1) Stage all changes: `git add .`
- [ ] (2) Commit with message:
  ```
  [.NET 10 Upgrade] Phase 1: Foundation projects upgraded to .NET 10

  Upgraded 5 foundation projects (Level 0):
  - Enigmatry.Entry.Core
  - Enigmatry.Entry.Csv
  - Enigmatry.Entry.MediatR
  - Enigmatry.Entry.Randomness
  - Enigmatry.Entry.Swagger

  Updated Directory.Packages.props:
  - DotnetVersion: 9.0.12 ? 10.0.2
  - Removed: System.ComponentModel.Annotations (Core)
  - Removed: System.Security.Cryptography.Algorithms (Randomness)

  Phase: 1/6
  Projects: 5/43
  Validation: All builds pass, all tests pass ?
  ```
- [ ] (3) Verify: Commit succeeded

**Validation**:
- Changes committed to branch `features/BP-1565-add-net10-support`

---

## Phase 2: High-Risk Core Extensions (Tier 2 - Individual Projects)

### [ ] TASK-008: Upgrade Enigmatry.Entry.Scheduler (High-Risk)
**Priority**: HIGH  
**Scope**: Single high-risk project (12 issues)

**Actions**:
- [ ] (1) Open `Enigmatry.Entry.Scheduler\Enigmatry.Entry.Scheduler.csproj`
- [ ] (2) Update `<TargetFramework>net10.0</TargetFramework>`
- [ ] (3) Save file
- [ ] (4) Build: `dotnet build Enigmatry.Entry.Scheduler\Enigmatry.Entry.Scheduler.csproj`
- [ ] (5) **EXPECT ERRORS**: Compilation will fail due to breaking changes
- [ ] (6) Fix Api.0001 - ConfigurationBinder.Get<T>() signature changes:
  - File: `JobConfiguration.cs` (Line 41)
  - Change: `var value = section.Get<T>();` ? `var value = section.Get<T>(options => { });`
  - File: `ConfigurationExtensions.cs` (Line 16)
  - Change: `var settings = jobSection.Get<JobSettings>();` ? `var settings = jobSection.Get<JobSettings>(options => { });`
- [ ] (7) Fix Api.0002 - ConfigurationErrorsException removed (5 occurrences):
  - File: `JobConfiguration.cs` (Lines 24, 29)
  - File: `ConfigurationExtensions.cs` (Line 26)
  - Change: `throw new ConfigurationErrorsException("message");` ? `throw new InvalidOperationException("message");`
  - Update using statements: Remove `System.Configuration`, add `System` if needed
- [ ] (8) Rebuild: `dotnet build Enigmatry.Entry.Scheduler\Enigmatry.Entry.Scheduler.csproj`
- [ ] (9) Verify: 0 errors
- [ ] (10) Run tests: `dotnet test Enigmatry.Entry.Scheduler.Tests\Enigmatry.Entry.Scheduler.Tests.csproj`
- [ ] (11) Verify: All tests pass
- [ ] (12) Manual validation: Test job scheduling, execution, OpenTelemetry traces

**Validation**:
- Project builds without errors
- All API breaking changes resolved
- Scheduler.Tests passes
- Job scheduling works correctly
- OpenTelemetry traces captured

---

### [ ] TASK-009: Upgrade Enigmatry.Entry.BlobStorage (High-Risk)
**Priority**: HIGH  
**Scope**: Single high-risk project (19 issues)

**Actions**:
- [ ] (1) Open `Enigmatry.Entry.BlobStorage\Enigmatry.Entry.BlobStorage.csproj`
- [ ] (2) Update `<TargetFramework>net10.0</TargetFramework>`
- [ ] (3) Save file
- [ ] (4) Build: `dotnet build Enigmatry.Entry.BlobStorage\Enigmatry.Entry.BlobStorage.csproj`
- [ ] (5) Fix any compilation errors (Azure SDK API changes)
- [ ] (6) Rebuild: Verify 0 errors
- [ ] (7) Run tests: `dotnet test Enigmatry.Entry.BlobStorage.Tests\Enigmatry.Entry.BlobStorage.Tests.csproj`
- [ ] (8) Verify: All tests pass
- [ ] (9) Integration testing: Validate upload/download operations against Azure Storage

**Validation**:
- Project builds without errors
- BlobStorage.Tests passes
- Upload/download operations work
- Azure SDK behavioral changes validated

---

### [ ] TASK-010: Upgrade Enigmatry.Entry.AzureSearch (High-Risk)
**Priority**: HIGH  
**Scope**: Single high-risk project (14 issues)

**Actions**:
- [ ] (1) Open `Enigmatry.Entry.AzureSearch\Enigmatry.Entry.AzureSearch.csproj`
- [ ] (2) Update `<TargetFramework>net10.0</TargetFramework>`
- [ ] (3) Save file
- [ ] (4) Build: `dotnet build Enigmatry.Entry.AzureSearch\Enigmatry.Entry.AzureSearch.csproj`
- [ ] (5) Fix any compilation errors (Azure Search SDK API changes)
- [ ] (6) Rebuild: Verify 0 errors
- [ ] (7) Run tests: `dotnet test Enigmatry.Entry.AzureSearch.Tests\Enigmatry.Entry.AzureSearch.Tests.csproj`
- [ ] (8) Verify: All tests pass
- [ ] (9) Integration testing: Validate search queries, indexing operations

**Validation**:
- Project builds without errors
- AzureSearch.Tests passes
- Search operations work correctly

---

### [ ] TASK-011: Upgrade Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson (Very High-Risk)
**Priority**: VERY HIGH  
**Scope**: Single very high-risk project (25 issues)

**Actions**:
- [ ] (1) Open `Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson\Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.csproj`
- [ ] (2) Update `<TargetFramework>net10.0</TargetFramework>`
- [ ] (3) Save file
- [ ] (4) Build: `dotnet build Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson\...csproj`
- [ ] (5) Fix source incompatibilities (7 occurrences - let compiler guide)
- [ ] (6) Rebuild: Verify 0 errors
- [ ] (7) Run tests: `dotnet test Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests\...csproj`
- [ ] (8) **EXPECT TEST FAILURES**: JSON serialization behavioral changes
- [ ] (9) Compare JSON serialization output .NET 9 vs .NET 10 for sample objects
- [ ] (10) Update test assertions for new JSON behavior (dates, nulls, enums)
- [ ] (11) Re-run tests: Verify all pass
- [ ] (12) Document behavioral changes

**Validation**:
- Project builds without errors
- All test assertions updated for .NET 10 behavior
- AspNetCore.Tests.NewtonsoftJson.Tests passes
- JSON serialization behavioral changes documented

---

### [ ] TASK-012: Commit Phase 2 Changes
**Priority**: MEDIUM  
**Scope**: Source control

**Actions**:
- [ ] (1) Stage all changes: `git add .`
- [ ] (2) Commit with message:
  ```
  [.NET 10 Upgrade] Phase 2: High-risk projects upgraded

  Upgraded 4 high-risk projects individually:
  - Enigmatry.Entry.Scheduler (12 issues resolved)
  - Enigmatry.Entry.BlobStorage (19 issues resolved)
  - Enigmatry.Entry.AzureSearch (14 issues resolved)
  - Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson (25 issues resolved)

  Breaking changes addressed:
  - Scheduler: ConfigurationErrorsException ? InvalidOperationException
  - Scheduler: ConfigurationBinder.Get<T>() signature updated
  - BlobStorage: Azure SDK API changes resolved
  - AzureSearch: Azure Search SDK API changes resolved
  - NewtonsoftJson: JSON serialization behavioral changes documented

  Phase: 2/6
  Projects: 9/43
  Validation: All builds pass, all tests pass, behavioral changes validated ?
  ```
- [ ] (3) Verify: Commit succeeded

**Validation**:
- Changes committed to branch

---

## Phase 3: Medium/Low-Risk Core Extensions (Tier 2)

### [ ] TASK-013: Upgrade Batch 1 - Medium-Risk Projects
**Priority**: MEDIUM  
**Scope**: 3 projects (Email, GraphApi, AspNetCore.Tests.Utilities)

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 3 projects:
  - `Enigmatry.Entry.EmailClient\Enigmatry.Entry.Email.csproj`
  - `Enigmatry.Entry.GraphApi\Enigmatry.Entry.GraphApi.csproj`
  - `Enigmatry.Entry.AspNetCore.Tests.Utilities\Enigmatry.Entry.AspNetCore.Tests.Utilities.csproj`
- [ ] (2) Build all 3 projects
- [ ] (3) Fix any compilation errors
- [ ] (4) Run Email.Tests
- [ ] (5) Verify: All build, Email.Tests passes

**Validation**:
- All 3 projects build successfully
- Email.Tests passes

---

### [ ] TASK-014: Upgrade Batch 2 - High-Impact Low-Risk Projects
**Priority**: HIGH  
**Scope**: 3 projects (AspNetCore, Core.EntityFramework, SmartEnums)

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 3 projects:
  - `Enigmatry.Entry.AspNetCore\Enigmatry.Entry.AspNetCore.csproj`
  - `Enigmatry.Entry.Core.EntityFramework\Enigmatry.Entry.Core.EntityFramework.csproj`
  - `Enigmatry.Entry.SmartEnums\Enigmatry.Entry.SmartEnums.csproj`
- [ ] (2) Build all 3 projects
- [ ] (3) Verify: 0 errors (these are used by many consumers)

**Validation**:
- All 3 projects build successfully
- No breaking changes for consumers

---

### [ ] TASK-015: Upgrade Batch 3 - Low-Risk Libraries
**Priority**: LOW  
**Scope**: 4 projects (Templating engines, HealthChecks, Infrastructure)

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 4 projects:
  - `Enigmatry.Entry.TemplatingEngine.Fluid\Enigmatry.Entry.TemplatingEngine.Fluid.csproj`
  - `Enigmatry.Entry.TemplatingEngine.Razor\Enigmatry.Entry.TemplatingEngine.Razor.csproj`
  - `Enigmatry.Entry.HealthChecks\Enigmatry.Entry.HealthChecks.csproj`
  - `Enigmatry.Entry.Infrastructure\Enigmatry.Entry.Infrastructure.csproj`
- [ ] (2) Build all 4 projects
- [ ] (3) Fix any errors (HealthChecks may have binary incompatible changes)

**Validation**:
- All 4 projects build successfully

---

### [ ] TASK-016: Upgrade Batch 4 - Level 1 Test Projects
**Priority**: LOW  
**Scope**: 3 test projects

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 3 projects:
  - `Enigmatry.Entry.Core.Tests\Enigmatry.Entry.Core.Tests.csproj`
  - `Enigmatry.Entry.Csv.Tests\Enigmatry.Entry.Csv.Tests.csproj`
  - `Enigmatry.Entry.MediatR.Tests\Enigmatry.Entry.MediatR.Tests.csproj`
- [ ] (2) Build all 3 projects
- [ ] (3) Run all 3 test projects
- [ ] (4) Verify: All tests pass

**Validation**:
- All test projects pass

---

### [ ] TASK-017: Validate Phase 3 Completion
**Priority**: HIGH  
**Scope**: All Tier 2 projects

**Actions**:
- [ ] (1) Build all Tier 1 + Tier 2 projects (29 total)
- [ ] (2) Run all tests in Tiers 1-2
- [ ] (3) Verify: All builds succeed, all tests pass
- [ ] (4) Check for regressions in Tier 1

**Validation**:
- All 29 projects (Tiers 1-2) build successfully
- All tests pass
- No regressions

---

### [ ] TASK-018: Commit Phase 3 Changes
**Priority**: MEDIUM  
**Scope**: Source control

**Actions**:
- [ ] (1) Stage all changes: `git add .`
- [ ] (2) Commit with message:
  ```
  [.NET 10 Upgrade] Phase 3: Tier 2 medium/low-risk projects upgraded

  Upgraded 14 Tier 2 projects in 4 batches:
  - Batch 1 (Medium): Email, GraphApi, AspNetCore.Tests.Utilities
  - Batch 2 (High-Impact): AspNetCore, Core.EntityFramework, SmartEnums
  - Batch 3 (Libraries): Templating engines, HealthChecks, Infrastructure
  - Batch 4 (Tests): Core.Tests, Csv.Tests, MediatR.Tests

  Phase: 3/6
  Projects: 23/43
  Validation: All Tiers 1-2 build together, all tests pass ?
  ```
- [ ] (3) Verify: Commit succeeded

**Validation**:
- Changes committed

---

## Phase 4: Integration Extensions & Tests (Tier 3)

### [ ] TASK-019: Upgrade Batch 1 - High-Risk Test Projects
**Priority**: HIGH  
**Scope**: 4 high-risk test projects

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 4 projects:
  - `Enigmatry.Entry.BlobStorage.Tests\Enigmatry.Entry.BlobStorage.Tests.csproj`
  - `Enigmatry.Entry.AzureSearch.Tests\Enigmatry.Entry.AzureSearch.Tests.csproj`
  - `Enigmatry.Entry.Scheduler.Tests\Enigmatry.Entry.Scheduler.Tests.csproj`
  - `Enigmatry.Entry.AspNetCore.Tests.SystemTextJson\Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.csproj`
- [ ] (2) Build all 4 projects
- [ ] (3) Fix compilation errors (source incompatibilities in test code)
- [ ] (4) Run all 4 test projects
- [ ] (5) Update test assertions for behavioral changes
- [ ] (6) Verify: All tests pass

**Validation**:
- All 4 projects build
- All tests pass

---

### [ ] TASK-020: Upgrade Batch 2 - Low-Risk Extensions & Tests
**Priority**: LOW  
**Scope**: 11 projects

**Actions**:
- [ ] (1) Update TFM to net10.0 for high-impact projects:
  - `Enigmatry.Entry.AspNetCore.Authorization\Enigmatry.Entry.AspNetCore.Authorization.csproj`
  - `Enigmatry.Entry.EntityFramework\Enigmatry.Entry.EntityFramework.csproj`
- [ ] (2) Update TFM for SmartEnums extensions (4 projects):
  - `Enigmatry.Entry.SmartEnums.EntityFramework\Enigmatry.Entry.SmartEnums.EntityFramework.csproj`
  - `Enigmatry.Entry.SmartEnums.Swagger\Enigmatry.Entry.SmartEnums.Swagger.csproj`
  - `Enigmatry.Entry.SmartEnums.SystemTextJson\Enigmatry.Entry.SmartEnums.SystemTextJson.csproj`
  - `Enigmatry.Entry.SmartEnums.VerifyTests\Enigmatry.Entry.SmartEnums.VerifyTests.csproj`
- [ ] (3) Update TFM for test projects (5 projects):
  - `Enigmatry.Entry.Email.Tests\Enigmatry.Entry.Email.Tests.csproj`
  - `Enigmatry.Entry.HealthChecks.Tests\Enigmatry.Entry.HealthChecks.Tests.csproj`
  - `Enigmatry.Entry.Infrastructure.Tests\Enigmatry.Entry.Infrastructure.Tests.csproj`
  - `Enigmatry.Entry.TemplatingEngine.Fluid.Tests\Enigmatry.Entry.TemplatingEngine.Fluid.Tests.csproj`
  - `Enigmatry.Entry.TemplatingEngine.Razor.Tests\Enigmatry.Entry.TemplatingEngine.Razor.Tests.csproj`
- [ ] (4) Build all 11 projects
- [ ] (5) Fix any compilation errors
- [ ] (6) Run all test projects
- [ ] (7) Verify: All tests pass

**Validation**:
- All 11 projects build
- All tests pass

---

### [ ] TASK-021: Validate Phase 4 Completion
**Priority**: HIGH  
**Scope**: All Tier 3 projects

**Actions**:
- [ ] (1) Build all Tiers 1-3 projects (44 total)
- [ ] (2) Run all tests in Tiers 1-3
- [ ] (3) Verify: All builds succeed, all tests pass
- [ ] (4) Check for regressions

**Validation**:
- All 44 projects build successfully
- All tests pass
- No regressions

---

### [ ] TASK-022: Commit Phase 4 Changes
**Priority**: MEDIUM  
**Scope**: Source control

**Actions**:
- [ ] (1) Stage all changes: `git add .`
- [ ] (2) Commit with message:
  ```
  [.NET 10 Upgrade] Phase 4: Tier 3 integration extensions & tests upgraded

  Upgraded 15 Tier 3 projects:
  - Batch 1 (High-Risk Tests): BlobStorage.Tests, AzureSearch.Tests, Scheduler.Tests, SystemTextJson
  - Batch 2 (Extensions): Authorization, EntityFramework, SmartEnums extensions (4), test projects (5)

  Phase: 4/6
  Projects: 38/43
  Validation: All Tiers 1-3 build together, all tests pass ?
  ```
- [ ] (3) Verify: Commit succeeded

**Validation**:
- Changes committed

---

## Phase 5: Application & Top-Level Integration (Tiers 4-5)

### [ ] TASK-023: Upgrade Tier 4 - Application Layer
**Priority**: MEDIUM  
**Scope**: 3 Level 3 projects

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 3 projects:
  - `Enigmatry.Entry.AspNetCore.Tests.SampleApp\Enigmatry.Entry.AspNetCore.Tests.SampleApp.csproj`
  - `Enigmatry.Entry.AspNetCore.Authorization.Tests\Enigmatry.Entry.AspNetCore.Authorization.Tests.csproj`
  - `Enigmatry.Entry.EntityFramework.Tests\Enigmatry.Entry.EntityFramework.Tests.csproj`
- [ ] (2) Build all 3 projects
- [ ] (3) Run SampleApp: `dotnet run --project Enigmatry.Entry.AspNetCore.Tests.SampleApp`
- [ ] (4) Verify: App starts, Swagger UI accessible, health checks work
- [ ] (5) Run test projects: Authorization.Tests, EntityFramework.Tests
- [ ] (6) Verify: All tests pass

**Validation**:
- All 3 projects build
- SampleApp runs successfully
- All tests pass

---

### [ ] TASK-024: Upgrade Tier 5 - Integration Tests
**Priority**: MEDIUM  
**Scope**: 3 top-level test projects

**Actions**:
- [ ] (1) Update TFM to net10.0 for all 3 projects:
  - `Enigmatry.Entry.AspNetCore.Tests\Enigmatry.Entry.AspNetCore.Tests.csproj`
  - `Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests\Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests.csproj`
  - `Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests\Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests.csproj`
- [ ] (2) Build all 3 projects
- [ ] (3) Fix any compilation errors (behavioral changes in test framework)
- [ ] (4) Run all 3 test projects
- [ ] (5) Update test assertions for behavioral changes if needed
- [ ] (6) Verify: All tests pass

**Validation**:
- All 3 projects build
- Full integration test suite passes

---

### [ ] TASK-025: Validate Phase 5 Completion
**Priority**: HIGH  
**Scope**: All 43 projects

**Actions**:
- [ ] (1) Build entire solution: `dotnet build Enigmatry.Entry.sln --configuration Release`
- [ ] (2) Verify: 0 errors, 0 warnings
- [ ] (3) Run all tests: `dotnet test Enigmatry.Entry.sln --configuration Release`
- [ ] (4) Verify: All tests pass (0 failures)

**Validation**:
- All 43 projects build successfully
- Complete test suite passes

---

### [ ] TASK-026: Commit Phase 5 Changes
**Priority**: MEDIUM  
**Scope**: Source control

**Actions**:
- [ ] (1) Stage all changes: `git add .`
- [ ] (2) Commit with message:
  ```
  [.NET 10 Upgrade] Phase 5: Application & integration tests upgraded

  Upgraded final 6 projects:
  - Tier 4 (Applications): SampleApp, Authorization.Tests, EntityFramework.Tests
  - Tier 5 (Integration): AspNetCore.Tests, NewtonsoftJson.Tests, SystemTextJson.Tests

  Phase: 5/6
  Projects: 43/43 ?
  Validation: Full solution builds, all tests pass ?
  ```
- [ ] (3) Verify: Commit succeeded

**Validation**:
- Changes committed

---

## Phase 6: Final Validation & Documentation

### [ ] TASK-027: Comprehensive Solution Validation
**Priority**: CRITICAL  
**Scope**: Entire solution

**Actions**:
- [ ] (1) Clean solution: `dotnet clean Enigmatry.Entry.sln`
- [ ] (2) Delete bin/obj folders:
  ```
  Get-ChildItem -Recurse -Directory -Filter bin | Remove-Item -Recurse -Force
  Get-ChildItem -Recurse -Directory -Filter obj | Remove-Item -Recurse -Force
  ```
- [ ] (3) Rebuild solution: `dotnet build Enigmatry.Entry.sln --configuration Release`
- [ ] (4) Verify: Build succeeded, 0 errors, 0 warnings
- [ ] (5) Run complete test suite: `dotnet test Enigmatry.Entry.sln --configuration Release --logger "console;verbosity=detailed"`
- [ ] (6) Verify: All tests passed, 0 failures, 0 skipped
- [ ] (7) Check packages: `dotnet list package --include-transitive`
- [ ] (8) Verify: No version conflicts
- [ ] (9) Check vulnerabilities: `dotnet list package --vulnerable`
- [ ] (10) Verify: No vulnerable packages
- [ ] (11) Check deprecated: `dotnet list package --deprecated`
- [ ] (12) Verify: No deprecated packages

**Validation**:
- Clean build succeeds (0 errors, 0 warnings)
- All tests pass (0 failures)
- No package conflicts
- No security vulnerabilities
- No deprecated packages

---

### [ ] TASK-028: Update Documentation
**Priority**: MEDIUM  
**Scope**: Documentation files

**Actions**:
- [ ] (1) Update README.md:
  - Update .NET version requirement to .NET 10.0
  - Update installation instructions if needed
- [ ] (2) Create/update CHANGELOG.md:
  - Add entry for .NET 10 upgrade
  - List breaking changes addressed
  - Document behavioral changes
- [ ] (3) Create migration notes document (optional):
  - Document all breaking changes encountered
  - Document behavioral changes validated
  - Lessons learned

**Validation**:
- README.md updated with .NET 10 requirement
- CHANGELOG.md updated

---

### [ ] TASK-029: Final Commit & Tag
**Priority**: HIGH  
**Scope**: Source control

**Actions**:
- [ ] (1) Stage all changes: `git add .`
- [ ] (2) Commit with message:
  ```
  [.NET 10 Upgrade] Phase 6: Final validation and documentation

  Completed migration of all 43 projects to .NET 10.0 LTS.

  Summary:
  - All 43 projects successfully upgraded from .NET 9 to .NET 10
  - 212 issues addressed (55 mandatory, 157 potential)
  - Directory.Packages.props: DotnetVersion updated to 10.0.2
  - Removed framework-included packages (2)
  - All breaking changes resolved
  - All tests passing (0 failures)
  - No package conflicts or vulnerabilities
  - Documentation updated

  Phase: 6/6 COMPLETE ?
  Projects: 43/43 ?
  Validation: Full solution validated ?
  ```
- [ ] (3) Verify: Commit succeeded
- [ ] (4) Tag release (optional): `git tag v<version>-net10`

**Validation**:
- Final commit completed
- Branch ready for PR/merge

---

## Completion Checklist

### Technical Validation
- [ ] All 43 projects target .NET 10.0
- [ ] DotnetVersion property set to 10.0.2 in Directory.Packages.props
- [ ] Framework-included packages removed (System.ComponentModel.Annotations, System.Security.Cryptography.Algorithms)
- [ ] All projects build without errors
- [ ] All projects build without warnings (or warnings documented)
- [ ] All tests pass (0 failures)
- [ ] No package version conflicts
- [ ] No security vulnerabilities
- [ ] No deprecated packages

### High-Risk Projects Validated
- [ ] Scheduler: ConfigurationErrorsException replaced, job scheduling works
- [ ] BlobStorage: Azure SDK changes resolved, storage operations tested
- [ ] AzureSearch: Search SDK changes resolved, queries validated
- [ ] AspNetCore.Tests.NewtonsoftJson: JSON serialization changes documented

### Documentation
- [ ] README.md updated with .NET 10 requirement
- [ ] CHANGELOG.md updated
- [ ] Migration notes created (optional)

### Source Control
- [ ] All changes committed to `features/BP-1565-add-net10-support`
- [ ] Commit history clean and descriptive
- [ ] Ready for PR/merge to main branch

---

## Success Declaration

**The .NET 10 upgrade is complete when all checkboxes above are checked (?).**

At that point, the solution is ready for:
- Code review
- Merge to main branch
- Release and deployment

---

**Last Updated**: Generated from plan.md  
**Branch**: features/BP-1565-add-net10-support  
**Strategy**: Bottom-Up Incremental Migration
