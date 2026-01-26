# .NET 10.0 Upgrade Plan

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Migration Strategy](#migration-strategy)
3. [Detailed Dependency Analysis](#detailed-dependency-analysis)
4. [Project-by-Project Plans](#project-by-project-plans)
5. [Risk Management](#risk-management)
6. [Testing & Validation Strategy](#testing--validation-strategy)
7. [Complexity & Effort Assessment](#complexity--effort-assessment)
8. [Source Control Strategy](#source-control-strategy)
9. [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade the Enigmatry Entry Building Blocks solution from **.NET 9** to **.NET 10.0 LTS**. This solution provides a comprehensive set of reusable libraries for building enterprise applications, including core utilities, ASP.NET Core extensions, Entity Framework support, and various infrastructure components.

### Scope

**Projects Affected**: 43 projects across multiple functional areas
- **Current State**: All projects targeting .NET 9
- **Target State**: All projects targeting .NET 10.0

**Discovery Metrics**:
- **Total Issues**: 212 (55 mandatory, 157 potential, 0 optional)
- **Affected Files**: 78 source files
- **Dependency Depth**: 6 levels (Level 0 through Level 5)
- **Security Vulnerabilities**: None detected ?
- **Circular Dependencies**: None detected ?

### Complexity Assessment

**Classification: Complex Solution**

**Justification**:
- ? 43 projects exceeds 15-project threshold for complex solutions
- ? Dependency depth of 6 levels exceeds 4-level threshold
- ? Multiple high-risk projects identified:
  - **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** (25 issues)
  - **Enigmatry.Entry.BlobStorage** (19 issues, API breaking changes)
  - **Enigmatry.Entry.AzureSearch** (14 issues, API breaking changes)
  - **Enigmatry.Entry.Scheduler** (12 issues, API breaking changes)
- ? 212 total issues requiring careful assessment and validation

### Critical Issues

**API Breaking Changes**:
- **Binary Incompatible (Mandatory)**: 10 occurrences across 6 projects
  - Affects: HealthChecks, AzureSearch, EmailClient, BlobStorage, MediatR.Tests, GraphApi
- **Source Incompatible (Potential)**: 35 occurrences across 8 projects
  - Requires code changes for compilation
- **Behavioral Changes (Potential)**: 69 occurrences across 13 projects
  - Runtime behavior may change without compilation errors

**Package Updates**:
- **Recommended Updates**: 53 package update recommendations
- **Framework-Included**: 2 packages now included in framework (require removal)
  - Affects: Core, Randomness

**Central Package Management**:
- Solution uses **Directory.Packages.props** for centralized package version management
- **DotnetVersion** property controls .NET-related package versions
- Updates must maintain this architecture

### Recommended Approach

**Strategy**: **Bottom-Up (Dependency-First) Incremental Migration**

**Rationale**:
1. **Dependency Depth**: With 6 levels of dependencies, bottom-up ensures each tier builds on stable, upgraded foundations
2. **Risk Management**: High-risk projects handled individually with thorough validation
3. **Clear Validation Points**: Each level completion provides a natural checkpoint
4. **Minimal Multi-Targeting**: Avoid complexity of multi-targeting by upgrading dependencies before consumers
5. **Incremental Benefits**: Lower tiers unlock new .NET 10 features for higher tiers progressively

**Iteration Strategy**: 
- **Phase 1**: Foundation projects (Levels 0-1) - Critical shared libraries
- **Phase 2**: High-risk projects individually (BlobStorage, AzureSearch, Scheduler, AspNetCore.Tests.NewtonsoftJson)
- **Phase 3**: Medium complexity projects in batches (Levels 2-3)
- **Phase 4**: Top-level test applications and final integrations (Levels 4-5)
- **Phase 5**: Validation and completion

**Expected Iterations**: 7-9 detail iterations following foundation setup

---

## Migration Strategy

### Approach Selection: Bottom-Up Incremental Migration

**Chosen Strategy**: **Bottom-Up (Dependency-First) with Risk-Prioritized Execution**

#### Why Bottom-Up?

**Solution Characteristics Favor This Approach**:
1. **Large Scale**: 43 projects would be risky to upgrade simultaneously
2. **Deep Hierarchy**: 6 dependency levels require careful ordering
3. **Mixed Complexity**: High-risk projects (25 issues) alongside simple ones (1 issue)
4. **Stable Foundation Needed**: Core library used by 15 projects must be rock-solid first
5. **Testing Requirements**: Each tier can be validated before proceeding to dependent tiers

**Advantages for This Solution**:
- ? **Lowest Risk**: Each tier builds on already-validated, upgraded dependencies
- ? **No Multi-Targeting**: Consumers always reference same-framework dependencies
- ? **Clear Validation Points**: Test each tier independently before moving up
- ? **Easier Debugging**: Issues isolated to current tier's changes
- ? **Learning Curve**: Early tier lessons (Core) apply to later tiers
- ? **Parallel Work Possible**: Within tiers, independent projects can be batched

**Challenges Accepted**:
- ?? Longer timeline than all-at-once (but safer)
- ?? More coordination between phases required
- ?? Benefits realized late (applications upgraded last)

#### Dependency-Based Ordering

**Tier Progression**:
```
Tier 1 (Level 0) ? Tier 2 (Level 1) ? Tier 3 (Level 2) ? Tier 4 (Level 3) ? Tier 5 (Levels 4-5)
```

**Ordering Rules**:
1. **Strict Tier Sequence**: Cannot start Tier N+1 until Tier N fully validated
2. **Within-Tier Flexibility**: Projects in same tier can be done in parallel or batched by complexity
3. **Critical Path First**: Within each tier, prioritize projects with most consumers
4. **Risk-Based Batching**: High-risk projects get individual attention, low-risk can be batched

#### Parallel vs Sequential Execution

**Sequential Execution Required**:
- ? **Cross-Tier**: Must wait for full tier completion before starting next
- ? **High-Risk Projects**: Handle individually with full validation cycles
- ? **Critical Path**: Core must complete before its 15 consumers can start

**Parallel Execution Possible**:
- ? **Within-Tier Low-Risk Projects**: Can batch 5-10 simple projects together
- ? **Independent Branches**: Csv, MediatR, Randomness, Swagger (Level 0) can be done concurrently
- ? **Test Projects**: Tests for same-tier projects can be batched together

**Example Parallelization - Tier 1 Low-Risk Batch**:
```
Can upgrade together (all depend only on Core):
- AspNetCore
- Core.EntityFramework
- SmartEnums
- Infrastructure
- HealthChecks
```

### Phase Definitions

#### **Phase 1: Foundation Establishment**
**Scope**: Tier 1 (Level 0) - 5 projects  
**Strategy**: Sequential for Core (critical), parallel for others

**Execution**:
1. **Enigmatry.Entry.Core** (individual) - Must complete first, validates in isolation
2. **Parallel Batch**: Csv, MediatR, Randomness, Swagger - All independent, can upgrade together

**Completion Criteria**:
- All 5 projects build successfully on .NET 10
- All tests pass
- No package dependency conflicts
- Core validated by its own tests before consumers can reference it

---

#### **Phase 2: High-Risk Core Extensions**
**Scope**: Tier 2 High-Risk (Level 1) - 3 projects  
**Strategy**: Individual, sequential with full validation per project

**Execution Order**:
1. **Enigmatry.Entry.Scheduler** (12 issues, 4 mandatory)
   - Binary + Source + Behavioral breaking changes
   - Full test suite validation required
2. **Enigmatry.Entry.BlobStorage** (19 issues, 2 mandatory)
   - Highest issue count in Tier 2
   - Azure Storage SDK changes
3. **Enigmatry.Entry.AzureSearch** (14 issues, 2 mandatory)
   - Azure Cognitive Search SDK changes

**Completion Criteria (per project)**:
- Project builds without errors
- All unit tests pass
- Integration tests with Azure services validated
- Breaking changes documented
- Behavioral changes verified

---

#### **Phase 3: Medium & Low-Risk Core Extensions**
**Scope**: Tier 2 Medium/Low-Risk (Level 1) - 14 projects  
**Strategy**: Batched by complexity, with critical-path projects prioritized

**Batch 1 - Medium Risk** (3 projects):
- Enigmatry.Entry.Email (7 issues, 3M)
- Enigmatry.Entry.GraphApi (6 issues, 2M)
- Enigmatry.Entry.AspNetCore.Tests.Utilities (9 issues, 1M)

**Batch 2 - Low Risk, High Impact** (3 projects - these have consumers):
- **Enigmatry.Entry.AspNetCore** (used by 3 projects)
- **Enigmatry.Entry.Core.EntityFramework** (used by 2 projects)
- **Enigmatry.Entry.SmartEnums** (used by 4 projects)

**Batch 3 - Low Risk Libraries** (5 projects):
- Enigmatry.Entry.TemplatingEngine.Fluid
- Enigmatry.Entry.TemplatingEngine.Razor
- Enigmatry.Entry.HealthChecks
- Enigmatry.Entry.Infrastructure

**Batch 4 - Level 1 Tests** (3 projects):
- Enigmatry.Entry.Core.Tests
- Enigmatry.Entry.Csv.Tests
- Enigmatry.Entry.MediatR.Tests

**Completion Criteria**:
- All 14 projects build successfully
- All project tests pass
- No regressions in Tier 1 projects

---

#### **Phase 4: Integration Extensions & Tests**
**Scope**: Tier 3 (Level 2) - 16 projects  
**Strategy**: High-risk individual, others batched

**Individual - Very High Risk**:
- **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** (25 issues)
  - Highest issue count in entire solution
  - Newtonsoft.Json serialization behavioral changes
  - Extensive validation required

**Batch 1 - High-Risk Test Projects** (4 projects):
- Enigmatry.Entry.BlobStorage.Tests (19 issues)
- Enigmatry.Entry.AzureSearch.Tests (11 issues)
- Enigmatry.Entry.Scheduler.Tests (10 issues)
- Enigmatry.Entry.AspNetCore.Tests.SystemTextJson (9 issues)

**Batch 2 - Low-Risk Extensions** (11 projects):
- Enigmatry.Entry.AspNetCore.Authorization (used by 2 - prioritize)
- Enigmatry.Entry.EntityFramework (used by 1 - prioritize)
- SmartEnums extensions (4 projects): EntityFramework, Swagger, SystemTextJson, VerifyTests
- Test projects (5 projects): Email.Tests, HealthChecks.Tests, Infrastructure.Tests, TemplatingEngine.Fluid.Tests, TemplatingEngine.Razor.Tests

**Completion Criteria**:
- All 16 projects build successfully
- All tests pass, especially high-issue-count projects
- No regressions in Tiers 1-2

---

#### **Phase 5: Application & Top-Level Integration**
**Scope**: Tiers 4-5 (Levels 3-5) - 6 projects  
**Strategy**: Batched by level

**Batch 1 - Level 3** (3 projects):
- Enigmatry.Entry.AspNetCore.Tests.SampleApp
- Enigmatry.Entry.AspNetCore.Authorization.Tests
- Enigmatry.Entry.EntityFramework.Tests

**Batch 2 - Levels 4-5** (3 projects):
- Enigmatry.Entry.AspNetCore.Tests (Level 4)
- Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests (Level 5)
- Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests (Level 5)

**Completion Criteria**:
- All 6 projects build successfully
- Full integration test suite passes
- End-to-end scenarios validated
- Sample application runs correctly

---

#### **Phase 6: Final Validation & Documentation**
**Scope**: Entire solution  
**Strategy**: Comprehensive validation

**Activities**:
1. Full solution build (all 43 projects)
2. Run complete test suite (all test projects)
3. Validate no package conflicts
4. Verify no security vulnerabilities remain
5. Performance validation (compare .NET 9 vs .NET 10 baselines if available)
6. Update documentation (README, migration notes, breaking changes catalog)
7. Generate changelog

**Completion Criteria**:
- ? All 43 projects target .NET 10.0
- ? All builds succeed without errors or warnings
- ? All tests pass (no regressions)
- ? No package dependency conflicts
- ? No security vulnerabilities
- ? Documentation updated

### Between-Phase Validation

**After Each Phase**:
1. **Build Validation**: All projects in phase + all lower phases build successfully
2. **Test Validation**: All tests in phase + affected lower-phase tests pass
3. **Regression Check**: Re-run tests from previous phases to ensure no breakage
4. **Package Audit**: Verify no dependency conflicts introduced
5. **Commit Checkpoint**: Commit phase completion before proceeding

**Rollback Strategy**: If phase validation fails, fix issues in current phase before proceeding. Do not advance to next phase with known failures.

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a clear bottom-up dependency structure with **6 hierarchical levels** (Level 0 through Level 5). No circular dependencies detected, enabling clean incremental migration.

```
Level 5: [Tests.NewtonsoftJson.Tests] [Tests.SystemTextJson.Tests]
         ?
Level 4: [AspNetCore.Tests]
         ?
Level 3: [AspNetCore.Authorization.Tests] [AspNetCore.Tests.SampleApp] [EntityFramework.Tests]
         ?
Level 2: Multiple test projects + integrations (16 projects)
         ?
Level 1: Core extensions + specialized libraries (17 projects)
         ?
Level 0: Foundation libraries (5 projects: Core, Csv, MediatR, Randomness, Swagger)
```

### Project Groupings by Migration Phase

#### **Tier 1: Foundation (Level 0 - 5 projects)**
**No dependencies on other projects in solution**

| Project | Issues | Risk | Notes |
|---------|--------|------|-------|
| **Enigmatry.Entry.Core** | 3 (2M) | **HIGH** | Used by 15 projects - critical foundation |
| Enigmatry.Entry.Csv | 1 (1M) | Low | Simple library, minimal issues |
| Enigmatry.Entry.MediatR | 2 (1M) | Low | Independent MediatR integration |
| Enigmatry.Entry.Randomness | 2 (2M) | Medium | Package removal required (NuGet.0003) |
| Enigmatry.Entry.Swagger | 2 (1M) | Low | Independent Swagger utilities |

**Critical Path**: **Enigmatry.Entry.Core** must complete first - it's the foundation for 15 other projects.

#### **Tier 2: Core Extensions (Level 1 - 17 projects)**
**Depend only on Tier 1 projects**

**High-Risk Projects (handle individually)**:
| Project | Issues | Risk | Dependencies | Notes |
|---------|--------|------|--------------|-------|
| **Enigmatry.Entry.Scheduler** | 12 (4M) | **HIGH** | Core | Binary + Source + Behavioral breaking changes |
| **Enigmatry.Entry.BlobStorage** | 19 (2M) | **HIGH** | Core | Most issues in solution, API breaking changes |
| **Enigmatry.Entry.AzureSearch** | 14 (2M) | **HIGH** | Core | Multiple API breaking changes |

**Medium-Risk Projects (batch 1)**:
- Enigmatry.Entry.Email (7 issues, 3M) - Dependencies: Core
- Enigmatry.Entry.GraphApi (6 issues, 2M) - Dependencies: Core
- Enigmatry.Entry.AspNetCore.Tests.Utilities (9 issues, 1M) - Dependencies: Core

**Low-Risk Projects (batch 2)**:
- Enigmatry.Entry.AspNetCore (3 issues, 1M) - Dependencies: Core | **Used by 3 projects**
- Enigmatry.Entry.Core.EntityFramework (2 issues, 1M) - Dependencies: Core | **Used by 2 projects**
- Enigmatry.Entry.SmartEnums (1 issue, 1M) - Dependencies: Core | **Used by 4 projects**
- Enigmatry.Entry.TemplatingEngine.Fluid (4 issues, 1M) - Dependencies: Core
- Enigmatry.Entry.TemplatingEngine.Razor (3 issues, 1M) - Dependencies: Core
- Enigmatry.Entry.HealthChecks (2 issues, 2M) - Dependencies: Core
- Enigmatry.Entry.Infrastructure (1 issue, 1M) - Dependencies: Core

**Test Projects (batch 3)**:
- Enigmatry.Entry.Core.Tests (1 issue, 1M) - Dependencies: Core
- Enigmatry.Entry.Csv.Tests (1 issue, 1M) - Dependencies: Csv
- Enigmatry.Entry.MediatR.Tests (2 issues, 2M) - Dependencies: MediatR

#### **Tier 3: Integrations & Extensions (Level 2 - 16 projects)**
**Depend on Tiers 1-2**

**High-Risk Project**:
| Project | Issues | Risk | Dependencies | Notes |
|---------|--------|------|--------------|-------|
| **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** | 25 (1M) | **VERY HIGH** | AspNetCore.Tests.Utilities | Highest issue count in solution |

**Medium-Risk Projects (batch 1)**:
- Enigmatry.Entry.BlobStorage.Tests (19 issues, 1M) - Dependencies: BlobStorage
- Enigmatry.Entry.AzureSearch.Tests (11 issues, 1M) - Dependencies: AzureSearch
- Enigmatry.Entry.Scheduler.Tests (10 issues, 1M) - Dependencies: Scheduler
- Enigmatry.Entry.AspNetCore.Tests.SystemTextJson (9 issues, 1M) - Dependencies: AspNetCore.Tests.Utilities

**Low-Risk Projects (batch 2)**:
- Enigmatry.Entry.AspNetCore.Authorization (1 issue, 1M) - Dependencies: AspNetCore | **Used by 2 projects**
- Enigmatry.Entry.EntityFramework (4 issues, 1M) - Dependencies: Core.EntityFramework | **Used by 1 project**
- Enigmatry.Entry.SmartEnums.EntityFramework (2 issues, 1M) - Dependencies: Core.EntityFramework + SmartEnums
- Enigmatry.Entry.SmartEnums.Swagger (1 issue, 1M) - Dependencies: SmartEnums
- Enigmatry.Entry.SmartEnums.SystemTextJson (1 issue, 1M) - Dependencies: SmartEnums
- Enigmatry.Entry.SmartEnums.VerifyTests (1 issue, 1M) - Dependencies: SmartEnums
- Enigmatry.Entry.Email.Tests (3 issues, 1M) - Dependencies: Email
- Enigmatry.Entry.HealthChecks.Tests (1 issue, 1M) - Dependencies: HealthChecks
- Enigmatry.Entry.Infrastructure.Tests (3 issues, 1M) - Dependencies: Infrastructure
- Enigmatry.Entry.TemplatingEngine.Fluid.Tests (3 issues, 1M) - Dependencies: TemplatingEngine.Fluid
- Enigmatry.Entry.TemplatingEngine.Razor.Tests (5 issues, 1M) - Dependencies: TemplatingEngine.Razor + Core

#### **Tier 4: Application Layer (Level 3 - 3 projects)**
**Depend on Tiers 1-3**

| Project | Issues | Risk | Dependencies | Notes |
|---------|--------|------|--------------|-------|
| Enigmatry.Entry.AspNetCore.Tests.SampleApp | 2 (1M) | Low | HealthChecks, AspNetCore, Swagger, Authorization | Sample/test application |
| Enigmatry.Entry.AspNetCore.Authorization.Tests | 1 (1M) | Low | AspNetCore.Authorization | Test project |
| Enigmatry.Entry.EntityFramework.Tests | 3 (1M) | Low | EntityFramework | Test project |

#### **Tier 5: Integration Tests (Level 4-5 - 3 projects)**
**Depend on all lower tiers**

| Project | Issues | Risk | Dependencies | Notes |
|---------|--------|------|--------------|-------|
| Enigmatry.Entry.AspNetCore.Tests | 7 (1M) | Medium | AspNetCore, SampleApp | Integration test suite |
| Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests | 2 (1M) | Low | AspNetCore.Tests, NewtonsoftJson | JSON serialization tests |
| Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests | 1 (1M) | Low | AspNetCore.Tests, SystemTextJson | JSON serialization tests |

### Critical Path Identification

**Must-Upgrade-First (blocks everything)**:
1. **Enigmatry.Entry.Core** (Level 0) - Foundation for 15 projects

**High-Impact Projects (block multiple consumers)**:
2. **Enigmatry.Entry.AspNetCore** (Level 1) - Used by 3 projects
3. **Enigmatry.Entry.SmartEnums** (Level 1) - Used by 4 projects
4. **Enigmatry.Entry.Core.EntityFramework** (Level 1) - Used by 2 projects
5. **Enigmatry.Entry.AspNetCore.Authorization** (Level 2) - Used by 2 projects

### Migration Order Principles

1. **Tier-by-Tier**: Complete each tier fully before starting next tier
2. **Within-Tier Priority**:
   - High-risk projects individually with full validation
   - Medium-risk projects in small batches (3-5 per batch)
   - Low-risk projects in larger batches (5-10 per batch)
   - Test projects together when their dependencies are complete
3. **Validation Gates**: Each tier completion requires full build + test pass
4. **No Multi-Targeting**: Dependencies always on same or newer framework than consumers

---

## Project-by-Project Plans

### Central Package Management Note

**Critical**: This solution uses **Directory.Packages.props** for centralized package version management with a **DotnetVersion** property.

**Implications**:
- Package version updates must be made in **Directory.Packages.props**, not individual project files
- The **DotnetVersion** property controls all .NET-related package versions (e.g., Microsoft.Extensions.*, Microsoft.EntityFrameworkCore.*)
- Update **DotnetVersion** property from `9.0.12` to `10.0.2` to cascade to all .NET packages
- Individual project files contain only `<PackageReference Include="..." />` without versions

---

### Tier 1: Foundation Projects (Level 0)

#### 1. Enigmatry.Entry.Core

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Files**: 37 source files
- **Dependencies**: None (foundation library)
- **Used By**: 15 projects (critical foundation)
- **Issues**: 3 (2 mandatory, 1 potential)
- **Risk Level**: **HIGH** (blocks all dependent projects)

**Packages**:
- FluentValidation 12.1.1 ?
- JetBrains.Annotations 2025.2.4 ?
- MediatR 12.4.1 ?
- Microsoft.Extensions.Logging.Abstractions 9.0.12 ? 10.0.2 (via DotnetVersion)
- Serilog 4.3.0 ?
- ~~System.ComponentModel.Annotations 5.0.0~~ ? **REMOVE** (included in framework)

**Target State**:
- **Target Framework**: net10.0
- **Packages**: 6 (remove 1)

**Migration Steps**:

1. **Prerequisites**
   - ? No project dependencies (can start immediately)
   - ? Verify .NET 10 SDK installed

2. **Update Project File**
   - Update `Enigmatry.Entry.Core.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

3. **Update Directory.Packages.props**
   - **Critical First Step**: Update DotnetVersion property
     ```xml
     <DotnetVersion>10.0.2</DotnetVersion>
     ```
   - Remove System.ComponentModel.Annotations entry:
     ```xml
     <!-- Remove this line -->
     <PackageVersion Include="System.ComponentModel.Annotations" Version="5.0.0" />
     ```

4. **Remove Package Reference from Project**
   - Update `Enigmatry.Entry.Core.csproj`, remove:
     ```xml
     <PackageReference Include="System.ComponentModel.Annotations" />
     ```

5. **Build and Validate**
   - `dotnet build Enigmatry.Entry.Core\Enigmatry.Entry.Core.csproj`
   - Expected: No errors
   - Verify no compilation errors from missing ComponentModel annotations (framework provides them)

6. **Run Tests**
   - `dotnet test Enigmatry.Entry.Core.Tests\Enigmatry.Entry.Core.Tests.csproj`
   - Expected: All tests pass

7. **Validation Checklist**
   - [ ] Project builds without errors
   - [ ] Project builds without warnings
   - [ ] Core.Tests passes completely
   - [ ] System.ComponentModel.Annotations functionality still works (framework-provided)
   - [ ] No package version conflicts

**Expected Breaking Changes**: None (package removal is safe, functionality in framework)

**Behavioral Changes**: None expected

---

#### 2. Enigmatry.Entry.Csv

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: None
- **Used By**: Csv.Tests
- **Issues**: 1 (1 mandatory)
- **Risk Level**: Low

**Target State**:
- **Target Framework**: net10.0

**Migration Steps**:

1. **Update Project File**
   - Update `Enigmatry.Entry.Csv.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

2. **Build and Validate**
   - `dotnet build Enigmatry.Entry.Csv\Enigmatry.Entry.Csv.csproj`

3. **Validation Checklist**
   - [ ] Builds without errors or warnings
   - [ ] Csv.Tests passes (when upgraded)

---

#### 3. Enigmatry.Entry.MediatR

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: None
- **Used By**: MediatR.Tests
- **Issues**: 2 (1 mandatory, 1 potential)
- **Risk Level**: Low

**Packages**:
- MediatR 12.4.1 ?
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.12 ? 10.0.2 (via DotnetVersion)

**Target State**:
- **Target Framework**: net10.0

**Migration Steps**:

1. **Update Project File**
   - Update `Enigmatry.Entry.MediatR.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

2. **Build and Validate**
   - `dotnet build Enigmatry.Entry.MediatR\Enigmatry.Entry.MediatR.csproj`
   - Microsoft.Extensions.DependencyInjection.Abstractions auto-updates via DotnetVersion

3. **Validation Checklist**
   - [ ] Builds without errors or warnings
   - [ ] MediatR.Tests passes (when upgraded)

---

#### 4. Enigmatry.Entry.Randomness

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: None
- **Used By**: None (standalone library)
- **Issues**: 2 (2 mandatory)
- **Risk Level**: Medium (package removal required)

**Packages**:
- JetBrains.Annotations 2025.2.4 ?
- ~~System.Security.Cryptography.Algorithms 4.3.1~~ ? **REMOVE** (included in framework)

**Target State**:
- **Target Framework**: net10.0
- **Packages**: 1 (remove 1)

**Migration Steps**:

1. **Update Project File**
   - Update `Enigmatry.Entry.Randomness.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

2. **Update Directory.Packages.props**
   - Remove System.Security.Cryptography.Algorithms entry:
     ```xml
     <!-- Remove this line -->
     <PackageVersion Include="System.Security.Cryptography.Algorithms" Version="4.3.1" />
     ```

3. **Remove Package Reference from Project**
   - Update `Enigmatry.Entry.Randomness.csproj`, remove:
     ```xml
     <PackageReference Include="System.Security.Cryptography.Algorithms" />
     ```

4. **Build and Validate**
   - `dotnet build Enigmatry.Entry.Randomness\Enigmatry.Entry.Randomness.csproj`
   - Verify cryptography APIs still work (framework provides them)

5. **Validation Checklist**
   - [ ] Builds without errors or warnings
   - [ ] Cryptography functionality intact (framework-provided)

**Expected Breaking Changes**: None (framework provides functionality)

---

#### 5. Enigmatry.Entry.Swagger

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: None
- **Used By**: AspNetCore.Tests.SampleApp
- **Issues**: 2 (1 mandatory, 1 potential)
- **Risk Level**: Low

**Packages**:
- NSwag.AspNetCore 14.6.3 ?
- Microsoft.Extensions.Configuration.Abstractions 9.0.12 ? 10.0.2 (via DotnetVersion)

**Target State**:
- **Target Framework**: net10.0

**Migration Steps**:

1. **Update Project File**
   - Update `Enigmatry.Entry.Swagger.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

2. **Build and Validate**
   - `dotnet build Enigmatry.Entry.SwaggerSecurity\Enigmatry.Entry.Swagger.csproj`
   - Microsoft.Extensions.Configuration.Abstractions auto-updates via DotnetVersion

3. **Validation Checklist**
   - [ ] Builds without errors or warnings
   - [ ] Swagger UI functionality validated when SampleApp upgraded

---

### Tier 1 Completion Criteria

- ? All 5 foundation projects build successfully
- ? All project files updated to `net10.0`
- ? DotnetVersion property updated to `10.0.2` in Directory.Packages.props
- ? System.ComponentModel.Annotations removed from Core
- ? System.Security.Cryptography.Algorithms removed from Randomness
- ? Core.Tests, Csv.Tests, MediatR.Tests pass
- ? No package conflicts
- ? **Enigmatry.Entry.Core validated thoroughly** (critical dependency)

---

### Tier 2: High-Risk Core Extensions (Level 1)

#### 6. Enigmatry.Entry.Scheduler ?? HIGH RISK

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: Enigmatry.Entry.Core
- **Used By**: Scheduler.Tests
- **Issues**: 12 (4 mandatory, 8 potential)
- **Risk Level**: **HIGH** - Binary + Source + Behavioral incompatibilities

**Key Breaking Changes**:
- **Api.0001 (Binary Incompatible)**: 3 occurrences
  - `ConfigurationBinder.Get<T>()` - Method signature changed
  - `OptionsConfigurationServiceCollectionExtensions.Configure<T>()` - Overload changes
- **Api.0002 (Source Incompatible)**: 5 occurrences
  - `System.Configuration.ConfigurationErrorsException` - Type removed/moved
  - Constructor signatures changed
- **Api.0003 (Behavioral Change)**: 1 occurrence
  - `ActivitySource.StartActivity()` - Behavior changed in .NET 10

**Migration Steps**:

1. **Prerequisites**
   - ? Enigmatry.Entry.Core upgraded to .NET 10
   - ? Ensure Quartz 3.15.1 compatible with .NET 10

2. **Update Project File**
   - Update `Enigmatry.Entry.Scheduler.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

3. **Package Updates** (via DotnetVersion in Directory.Packages.props)
   - Microsoft.Extensions.Options.ConfigurationExtensions: 9.0.12 ? 10.0.2 ?

4. **Fix Binary Incompatible APIs**

   **File: `JobConfiguration.cs` (Line 41)**
   - **Old**: `var value = section.Get<T>();`
   - **New**: `var value = section.Get<T>(options => { });` or use `Bind()` method
   - **Reason**: `Get<T>()` now requires configuration options parameter

   **File: `ConfigurationExtensions.cs` (Line 16)**
   - **Old**: `var settings = jobSection.Get<JobSettings>();`
   - **New**: `var settings = jobSection.Get<JobSettings>(options => { });`

   **File: `ServiceCollectionExtensions.cs` (Line 23)**
   - **Old**: `services.Configure<QuartzOptions>(configuration.GetSchedulingHostSection());`
   - **New**: Verify signature still valid or adjust overload
   - **Action**: Likely requires adding configuration binder options

5. **Fix Source Incompatible APIs**

   **ConfigurationErrorsException Replacement** (5 occurrences in 2 files):
   - **Files**: `JobConfiguration.cs` (Lines 24, 29), `ConfigurationExtensions.cs` (Line 26)
   - **Old**: `throw new ConfigurationErrorsException("message");`
   - **New**: `throw new InvalidOperationException("message");` or use `ConfigurationException` from Microsoft.Extensions.Configuration
   - **Reason**: `System.Configuration.ConfigurationErrorsException` removed in .NET Core/5+
   - **Alternative**: Create custom `ConfigurationException` or use `InvalidOperationException`

6. **Validate Behavioral Changes**

   **ActivitySource.StartActivity()** (`OpenTelemetryJobListener.cs`, Line 24):
   - **Current**: `using var activity = _activitySource.StartActivity(jobName, ActivityKind.Server);`
   - **Change**: Behavior of activity creation may differ in .NET 10
   - **Action**: Test that activity creation, propagation, and telemetry work correctly
   - **Validation**: Run OpenTelemetry integration tests, verify traces captured

7. **Build and Validate**
   - `dotnet build Enigmatry.Entry.Scheduler\Enigmatry.Entry.Scheduler.csproj`
   - Expected: All compilation errors resolved

8. **Run Tests**
   - `dotnet test Enigmatry.Entry.Scheduler.Tests\Enigmatry.Entry.Scheduler.Tests.csproj`
   - **Critical**: Validate job scheduling, execution, configuration loading
   - Test recurring jobs, cron expressions, manual triggers

9. **Integration Testing**
   - Schedule sample job, verify execution
   - Test exception handling, ensure errors caught properly
   - Validate OpenTelemetry traces captured

10. **Validation Checklist**
    - [ ] Builds without errors
    - [ ] All API breaking changes resolved
    - [ ] ConfigurationErrorsException replaced appropriately
    - [ ] ConfigurationBinder.Get<T> calls updated
    - [ ] Job scheduling works correctly
    - [ ] OpenTelemetry activities traced
    - [ ] Scheduler.Tests passes completely
    - [ ] No warnings about obsolete APIs

**Expected Breaking Changes**:
- ConfigurationErrorsException removed ? Replace with InvalidOperationException
- ConfigurationBinder.Get<T>() signature changed ? Add options parameter

**Behavioral Changes**:
- ActivitySource.StartActivity() behavior ? Verify telemetry works

---

#### 7. Enigmatry.Entry.BlobStorage ?? HIGH RISK

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: Enigmatry.Entry.Core
- **Used By**: BlobStorage.Tests
- **Issues**: 19 (2 mandatory, 17 potential)
- **Risk Level**: **HIGH** - Highest issue count in Tier 2, Azure SDK changes

**Key Breaking Changes**:
- **Api.0001 (Binary Incompatible)**: 1 occurrence - Azure.Storage.Blobs SDK API changes
- **Api.0003 (Behavioral Changes)**: 17 occurrences - Azure SDK behavioral differences

**Migration Steps**:

1. **Prerequisites**
   - ? Enigmatry.Entry.Core upgraded to .NET 10
   - ? Azure.Storage.Blobs 12.27.0 compatible with .NET 10 (verify)

2. **Update Project File**
   - Update `Enigmatry.Entry.BlobStorage.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

3. **Package Updates** (via Directory.Packages.props)
   - Azure.Storage.Blobs 12.27.0 ? (already compatible)
   - Microsoft.Extensions.* packages: 9.0.12 ? 10.0.2 ?

4. **Fix Binary Incompatible APIs**
   - Identify specific Azure SDK API that changed (requires compilation to surface)
   - Common changes: BlobClient methods, upload/download signatures
   - **Action**: Let compiler identify, then update based on error messages

5. **Validate Behavioral Changes** (17 occurrences)
   - **Azure SDK behavioral changes** may include:
     - Different default retry policies
     - Changed exception types for error conditions
     - Modified timeout behaviors
     - Updated metadata handling
   - **Action**: Thorough testing required, compare behavior .NET 9 vs .NET 10

6. **Build and Validate**
   - `dotnet build Enigmatry.Entry.BlobStorage\Enigmatry.Entry.BlobStorage.csproj`
   - Fix any compilation errors

7. **Run Tests**
   - `dotnet test Enigmatry.Entry.BlobStorage.Tests\Enigmatry.Entry.BlobStorage.Tests.csproj`
   - **Critical**: Validate against Azure Storage Emulator or test account

8. **Integration Testing**
   - Upload blob, download blob, verify content matches
   - List blobs, verify metadata correct
   - Delete blob, verify deletion
   - Test edge cases: large files, concurrent uploads, error conditions

9. **Validation Checklist**
    - [ ] Builds without errors
    - [ ] Binary incompatible API resolved
    - [ ] Upload/download operations tested
    - [ ] Blob listing works
    - [ ] Metadata handling correct
    - [ ] Error handling unchanged (or updated intentionally)
    - [ ] BlobStorage.Tests passes completely
    - [ ] No Azure SDK deprecation warnings

**Expected Breaking Changes**:
- Azure SDK API signature changes (specific API TBD during compilation)

**Behavioral Changes**:
- Azure SDK retry policies, timeout behavior, exception types (validate via testing)

---

#### 8. Enigmatry.Entry.AzureSearch ?? HIGH RISK

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: Enigmatry.Entry.Core
- **Used By**: AzureSearch.Tests
- **Issues**: 14 (2 mandatory, 12 potential)
- **Risk Level**: **HIGH** - Multiple API breaking changes, Azure SDK

**Key Breaking Changes**:
- **Api.0001 (Binary Incompatible)**: 1 occurrence - Azure.Search.Documents SDK
- **Api.0003 (Behavioral Changes)**: 12 occurrences - Search query behavior, indexing

**Migration Steps**:

1. **Prerequisites**
   - ? Enigmatry.Entry.Core upgraded to .NET 10
   - ? Azure.Search.Documents 11.7.0 compatible with .NET 10 (verify)

2. **Update Project File**
   - Update `Enigmatry.Entry.AzureSearch.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

3. **Package Updates** (via Directory.Packages.props)
   - Azure.Search.Documents 11.7.0 ?
   - Microsoft.Extensions.* packages: 9.0.12 ? 10.0.2 ?

4. **Fix Binary Incompatible APIs**
   - Identify specific Azure Search SDK API changes
   - Common areas: SearchClient, SearchIndexClient, query methods
   - **Action**: Compile to identify, update per error guidance

5. **Validate Behavioral Changes** (12 occurrences)
   - **Search query behavior**: Result ranking, scoring may differ
   - **Indexing operations**: Batch behavior, error handling
   - **Faceting/filtering**: Query syntax edge cases
   - **Action**: Compare search results .NET 9 vs .NET 10 for sample queries

6. **Build and Validate**
   - `dotnet build Enigmatry.Entry.AzureSearch\Enigmatry.Entry.AzureSearch.csproj`

7. **Run Tests**
   - `dotnet test Enigmatry.Entry.AzureSearch.Tests\Enigmatry.Entry.AzureSearch.Tests.csproj`

8. **Integration Testing**
   - Execute search queries, validate results
   - Test indexing operations, verify documents indexed
   - Test facets, filters, sorting
   - Validate suggestions, autocomplete functionality

9. **Validation Checklist**
    - [ ] Builds without errors
    - [ ] Binary incompatible API resolved
    - [ ] Search queries return expected results
    - [ ] Indexing operations work
    - [ ] Facets and filters correct
    - [ ] Error handling preserved
    - [ ] AzureSearch.Tests passes completely

**Expected Breaking Changes**:
- Azure Search SDK API signature changes (specific API TBD)

**Behavioral Changes**:
- Search ranking, query behavior, indexing operations (validate via comparison testing)

---

#### 9. Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson ?? VERY HIGH RISK

**Current State**:
- **Target Framework**: net9.0
- **Project Type**: ClassLibrary
- **Dependencies**: Enigmatry.Entry.AspNetCore.Tests.Utilities
- **Used By**: AspNetCore.Tests.NewtonsoftJson.Tests
- **Issues**: 25 (1 mandatory, 24 potential)
- **Risk Level**: **VERY HIGH** - Highest issue count in entire solution

**Key Breaking Changes**:
- **Api.0002 (Source Incompatible)**: 7 occurrences
- **Api.0003 (Behavioral Changes)**: 17 occurrences - JSON serialization behavior

**Migration Steps**:

1. **Prerequisites**
   - ? AspNetCore.Tests.Utilities upgraded to .NET 10
   - ? Newtonsoft.Json 13.0.4 compatible ?

2. **Update Project File**
   - Update `Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.csproj`:
     ```xml
     <TargetFramework>net10.0</TargetFramework>
     ```

3. **Package Updates** (via Directory.Packages.props)
   - Newtonsoft.Json 13.0.4 ? (compatible)
   - Microsoft.AspNetCore.Mvc.NewtonsoftJson: 9.0.12 ? 10.0.2 ?

4. **Fix Source Incompatible APIs** (7 occurrences)
   - **Action**: Compilation will identify specific source incompatibilities
   - Likely areas: ASP.NET Core MVC/API patterns, controller method signatures
   - Update code per compiler guidance

5. **Validate Behavioral Changes** (17 occurrences)
   - **JSON Serialization behavioral changes**:
     - Date format handling may differ
     - Null value serialization
     - Polymorphic deserialization
     - Enum serialization
     - Circular reference handling
   - **Critical Action**: Compare serialization output .NET 9 vs .NET 10
     - Create sample objects
     - Serialize to JSON in both versions
     - Compare JSON output character-by-character
     - Document any differences

6. **Build and Validate**
   - `dotnet build Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson\...csproj`
   - Fix all compilation errors

7. **Run Tests**
   - `dotnet test Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests\...csproj`
   - **Expect some test failures** due to behavioral changes
   - **Action**: Determine if failures are:
     - Expected behavioral changes ? Update test assertions
     - Actual bugs ? Fix code

8. **Behavioral Comparison Testing**
   - Create test suite comparing .NET 9 vs .NET 10 serialization
   - Test cases:
     - Simple objects
     - Complex nested objects
     - Collections (lists, dictionaries)
     - Dates and DateTimeOffset
     - Enums
     - Null values
     - Polymorphic types
   - Document any differences

9. **Validation Checklist**
    - [ ] Builds without errors
    - [ ] Source incompatibilities resolved
    - [ ] JSON serialization tested thoroughly
    - [ ] Behavioral changes documented
    - [ ] Test assertions updated for new behavior
    - [ ] AspNetCore.Tests.NewtonsoftJson.Tests passes
    - [ ] Serialization differences documented

**Expected Breaking Changes**:
- ASP.NET Core API source incompatibilities (specific APIs TBD)

**Behavioral Changes**:
- JSON serialization behavior differences (date formats, null handling, etc.)
- **Critical**: Allocate extra time for this project due to complexity

---

### Tier 2 High-Risk Completion Criteria

- ? All 4 high-risk projects build successfully
- ? Scheduler: API changes resolved, job scheduling validated
- ? BlobStorage: Azure SDK changes resolved, storage operations tested
- ? AzureSearch: Search SDK changes resolved, queries validated
- ? AspNetCore.Tests.NewtonsoftJson: JSON serialization changes documented
- ? All breaking changes documented
- ? All behavioral changes tested and validated
- ? Associated test projects pass
- ? No regressions in Tier 1

---

### Tier 2: Medium/Low-Risk Core Extensions (Level 1) - Batched

#### Batch 1: Medium-Risk Projects (3 projects)

**Common Migration Pattern**:
1. Update `<TargetFramework>net10.0</TargetFramework>` in .csproj
2. Packages auto-update via DotnetVersion in Directory.Packages.props
3. Build and resolve any compilation errors
4. Run tests and validate functionality

---

**10. Enigmatry.Entry.Email** (7 issues, 3M)
- **Dependencies**: Core
- **Issues**: Binary incompatible (1), Package updates (3)
- **Packages**: MailKit 4.14.0 ?, MimeKit 4.14.0 ?, Microsoft.Extensions.* ? 10.0.2
- **Expected Changes**: Email sending API changes (binary incompatible)
- **Validation**: Test email sending/receiving, SMTP integration
- **Test Project**: Email.Tests

**11. Enigmatry.Entry.GraphApi** (6 issues, 2M)
- **Dependencies**: Core
- **Issues**: Binary incompatible (1), Behavioral changes (4)
- **Packages**: Microsoft.Graph 5.100.0 ?, Azure.Identity 1.17.1 ?, Microsoft.Extensions.* ? 10.0.2
- **Expected Changes**: Microsoft Graph SDK API changes, authentication behavior
- **Validation**: Test Graph API calls (users, groups, etc.), auth flow
- **Test Project**: None (standalone)

**12. Enigmatry.Entry.AspNetCore.Tests.Utilities** (9 issues, 1M)
- **Dependencies**: Core
- **Issues**: Behavioral changes (8), Package updates (1)
- **Packages**: Microsoft.AspNetCore.Mvc.Testing ? 10.0.2, FakeItEasy 9.0.0 ?
- **Expected Changes**: Test helper behavioral changes (ASP.NET Core test framework)
- **Validation**: Run tests that use these utilities (AspNetCore.Tests.NewtonsoftJson, SystemTextJson)
- **Test Project**: Used by other test projects

**Batch Validation**:
- ? All 3 projects build without errors
- ? Email.Tests passes
- ? GraphApi manually tested (if credentials available)
- ? Test utilities validated by dependent tests

---

#### Batch 2: Low-Risk, High-Impact Projects (3 projects) - These have consumers

**13. Enigmatry.Entry.AspNetCore** (3 issues, 1M)
- **Dependencies**: Core
- **Used By**: 3 projects (AspNetCore.Tests, AspNetCore.Tests.SampleApp, AspNetCore.Authorization)
- **Issues**: Behavioral changes (2), Package updates (1)
- **Packages**: Microsoft.AspNetCore.* ? 10.0.2
- **Expected Changes**: ASP.NET Core middleware/extension behavioral changes
- **Validation**: Build, ensure no breaking changes for consumers
- **Priority**: High-impact, upgrade early in batch

**14. Enigmatry.Entry.Core.EntityFramework** (2 issues, 1M)
- **Dependencies**: Core
- **Used By**: 2 projects (EntityFramework, SmartEnums.EntityFramework)
- **Issues**: Package updates (1)
- **Packages**: Microsoft.EntityFrameworkCore ? 10.0.2
- **Expected Changes**: EF Core 10 features, minimal breaking
- **Validation**: Build, ensure consumers can reference

**15. Enigmatry.Entry.SmartEnums** (1 issue, 1M)
- **Dependencies**: Core
- **Used By**: 4 projects (SmartEnums.EntityFramework, Swagger, SystemTextJson, VerifyTests)
- **Issues**: TFM update only
- **Packages**: Ardalis.SmartEnum 8.2.0 ?
- **Expected Changes**: None (simple TFM update)
- **Validation**: Build, consumers reference correctly

**Batch Validation**:
- ? All 3 projects build without errors
- ? No breaking changes introduced for consumers
- ? AspNetCore behavioral changes documented

---

#### Batch 3: Low-Risk Libraries (5 projects)

**16. Enigmatry.Entry.TemplatingEngine.Fluid** (4 issues, 1M)
- **Dependencies**: Core
- **Packages**: Fluid.Core 2.31.0 ?, Microsoft.Extensions.* ? 10.0.2
- **Validation**: Test template rendering
- **Test Project**: TemplatingEngine.Fluid.Tests

**17. Enigmatry.Entry.TemplatingEngine.Razor** (3 issues, 1M)
- **Dependencies**: Core
- **Packages**: Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation ? 10.0.2
- **Validation**: Test Razor template compilation and rendering
- **Test Project**: TemplatingEngine.Razor.Tests

**18. Enigmatry.Entry.HealthChecks** (2 issues, 2M)
- **Dependencies**: Core
- **Issues**: Binary incompatible (2)
- **Packages**: AspNetCore.HealthChecks.System 9.0.0 ?
- **Expected Changes**: Health check API changes
- **Validation**: Test health check endpoints
- **Test Project**: HealthChecks.Tests

**19. Enigmatry.Entry.Infrastructure** (1 issue, 1M)
- **Dependencies**: Core
- **Issues**: TFM update only
- **Validation**: Simple build validation
- **Test Project**: Infrastructure.Tests

**Batch Validation**:
- ? All 5 projects build
- ? Templating engines tested with sample templates
- ? HealthChecks API changes resolved

---

#### Batch 4: Level 1 Test Projects (3 projects)

**20. Enigmatry.Entry.Core.Tests** (1 issue, 1M)
- **Dependencies**: Core
- **Validation**: All Core functionality tested

**21. Enigmatry.Entry.Csv.Tests** (1 issue, 1M)
- **Dependencies**: Csv
- **Validation**: CSV parsing/writing tested

**22. Enigmatry.Entry.MediatR.Tests** (2 issues, 2M)
- **Dependencies**: MediatR
- **Issues**: Binary incompatible (1)
- **Expected Changes**: MediatR or test framework API change
- **Validation**: MediatR pipeline tested

**Batch Validation**:
- ? All test projects pass
- ? Tier 1 projects fully validated

---

### Tier 2 Medium/Low-Risk Completion Criteria

- ? All 14 projects build successfully
- ? Batch 1 (Medium): Email, GraphApi, AspNetCore.Tests.Utilities validated
- ? Batch 2 (High-Impact): AspNetCore, Core.EntityFramework, SmartEnums upgraded (no consumer breakage)
- ? Batch 3 (Libraries): Templating engines, HealthChecks, Infrastructure validated
- ? Batch 4 (Tests): All Tier 1 tests pass
- ? No regressions in Tier 1
- ? All Tier 1 + Tier 2 (29 projects total) build together

---

### Tier 3: Integration Extensions & Tests (Level 2)

#### Batch 1: High-Risk Test Projects (4 projects)

**23. Enigmatry.Entry.BlobStorage.Tests** (19 issues, 1M) - ?? HIGH
- **Dependencies**: BlobStorage
- **Issues**: Behavioral changes (18) - mirrors BlobStorage's Azure SDK changes
- **Validation**: Comprehensive Azure Storage testing
- **Critical**: Validate all behavioral changes documented in BlobStorage project

**24. Enigmatry.Entry.AzureSearch.Tests** (11 issues, 1M) - ?? HIGH
- **Dependencies**: AzureSearch
- **Issues**: Source incompatible (10), Behavioral changes (1)
- **Expected Changes**: Test code needs updates for AzureSearch API changes
- **Validation**: Search operations thoroughly tested

**25. Enigmatry.Entry.Scheduler.Tests** (10 issues, 1M) - ?? HIGH
- **Dependencies**: Scheduler
- **Issues**: Source incompatible (5), Behavioral changes (5)
- **Expected Changes**: Test code mirrors Scheduler breaking changes
- **Validation**: Job scheduling and execution tested

**26. Enigmatry.Entry.AspNetCore.Tests.SystemTextJson** (9 issues, 1M)
- **Dependencies**: AspNetCore.Tests.Utilities
- **Issues**: Behavioral changes (8) - System.Text.Json serialization
- **Expected Changes**: JSON serialization behavioral differences
- **Validation**: Compare JSON output .NET 9 vs .NET 10

**Batch Validation**:
- ? All 4 high-risk test projects pass
- ? Behavioral changes in tests validated
- ? Source incompatibilities in test code resolved

---

#### Batch 2: Low-Risk Extensions & Tests (11 projects)

**High-Impact Projects** (upgrade first in batch):

**27. Enigmatry.Entry.AspNetCore.Authorization** (1 issue, 1M)
- **Dependencies**: AspNetCore
- **Used By**: 2 projects (SampleApp, Authorization.Tests)
- **Issues**: TFM update only
- **Validation**: Authorization policies tested

**28. Enigmatry.Entry.EntityFramework** (4 issues, 1M)
- **Dependencies**: Core.EntityFramework
- **Used By**: EntityFramework.Tests
- **Issues**: Package updates (EF Core ? 10.0.2)
- **Validation**: EF Core migrations, queries tested

**SmartEnums Extensions** (4 projects, 1 issue each):

**29. Enigmatry.Entry.SmartEnums.EntityFramework** (2 issues, 1M)
- **Dependencies**: Core.EntityFramework, SmartEnums
- **Packages**: EF Core ? 10.0.2, Ardalis.SmartEnum.EFCore 8.2.0 ?

**30. Enigmatry.Entry.SmartEnums.Swagger** (1 issue, 1M)
- **Dependencies**: SmartEnums
- **Packages**: NSwag.Generation.AspNetCore 14.6.3 ?

**31. Enigmatry.Entry.SmartEnums.SystemTextJson** (1 issue, 1M)
- **Dependencies**: SmartEnums
- **Packages**: Ardalis.SmartEnum.SystemTextJson 8.1.0 ?

**32. Enigmatry.Entry.SmartEnums.VerifyTests** (1 issue, 1M)
- **Dependencies**: SmartEnums
- **Packages**: Verify 31.9.4 ?, Verify.NUnit 31.9.4 ?

**Test Projects**:

**33. Enigmatry.Entry.Email.Tests** (3 issues, 1M)
- **Dependencies**: Email
- **Validation**: Email functionality tested

**34. Enigmatry.Entry.HealthChecks.Tests** (1 issue, 1M)
- **Dependencies**: HealthChecks
- **Validation**: Health check functionality tested

**35. Enigmatry.Entry.Infrastructure.Tests** (3 issues, 1M)
- **Dependencies**: Infrastructure
- **Issues**: Source incompatible (1)
- **Expected Changes**: Test code API update

**36. Enigmatry.Entry.TemplatingEngine.Fluid.Tests** (3 issues, 1M)
- **Dependencies**: TemplatingEngine.Fluid
- **Validation**: Fluid template rendering tested

**37. Enigmatry.Entry.TemplatingEngine.Razor.Tests** (5 issues, 1M)
- **Dependencies**: TemplatingEngine.Razor, Core
- **Issues**: Source incompatible (5)
- **Expected Changes**: Razor template test code updates
- **Validation**: Razor compilation and rendering tested

**Batch Validation**:
- ? All 11 projects build
- ? Authorization, EntityFramework validated (high-impact)
- ? SmartEnums extensions (4 projects) validated
- ? All test projects pass

---

### Tier 3 Completion Criteria

- ? All 15 projects build successfully (excludes AspNetCore.Tests.NewtonsoftJson already done in Tier 2)
- ? High-risk test projects validated: BlobStorage.Tests, AzureSearch.Tests, Scheduler.Tests, SystemTextJson
- ? Low-risk extensions upgraded: Authorization, EntityFramework, SmartEnums extensions (4)
- ? All test projects pass: Email.Tests, HealthChecks.Tests, Infrastructure.Tests, Templating.Tests (2)
- ? No regressions in Tiers 1-2
- ? All Tiers 1-3 (44 projects total) build together

---

### Tier 4: Application Layer (Level 3) - 3 projects

#### Batch 1: Level 3 Projects

**38. Enigmatry.Entry.AspNetCore.Tests.SampleApp** (2 issues, 1M)
- **Dependencies**: HealthChecks, AspNetCore, Swagger, AspNetCore.Authorization
- **Project Type**: AspNetCore (web application)
- **Issues**: Package updates (1)
- **Packages**: Microsoft.AspNetCore.* ? 10.0.2, Swashbuckle/NSwag ?
- **Migration**:
  1. Update `<TargetFramework>net10.0</TargetFramework>`
  2. Build project
  3. Run application: `dotnet run --project Enigmatry.Entry.AspNetCore.Tests.SampleApp`
- **Validation**:
  - ? Application starts successfully
  - ? Swagger UI accessible at /swagger
  - ? Health checks respond at /health
  - ? Authorization policies work
  - ? No runtime exceptions

**39. Enigmatry.Entry.AspNetCore.Authorization.Tests** (1 issue, 1M)
- **Dependencies**: AspNetCore.Authorization
- **Issues**: TFM update only
- **Validation**: Authorization test suite passes

**40. Enigmatry.Entry.EntityFramework.Tests** (3 issues, 1M)
- **Dependencies**: EntityFramework
- **Issues**: Package updates (EF Core ? 10.0.2)
- **Validation**: EF Core functionality tested

**Batch Validation**:
- ? All 3 projects build
- ? SampleApp runs successfully
- ? Authorization.Tests, EntityFramework.Tests pass

---

### Tier 5: Integration Tests (Levels 4-5) - 3 projects

#### Batch 1: Top-Level Integration Tests

**41. Enigmatry.Entry.AspNetCore.Tests** (7 issues, 1M) - Level 4
- **Dependencies**: AspNetCore, AspNetCore.Tests.SampleApp
- **Issues**: Behavioral changes (6), Package updates (1)
- **Package**: Microsoft.AspNetCore.Mvc.Testing ? 10.0.2
- **Expected Changes**: ASP.NET Core test framework behavioral changes
- **Migration**:
  1. Update `<TargetFramework>net10.0</TargetFramework>`
  2. Build and fix compilation errors
  3. Run integration test suite
  4. Update test assertions for behavioral changes
- **Validation**:
  - ? Full integration test suite passes
  - ? Web API testing validated
  - ? Middleware pipeline tested
  - ? No false failures due to behavioral changes

**42. Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests** (2 issues, 1M) - Level 5
- **Dependencies**: AspNetCore.Tests, AspNetCore.Tests.NewtonsoftJson
- **Issues**: Package updates (1)
- **Validation**: Newtonsoft.Json integration tests pass

**43. Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests** (1 issue, 1M) - Level 5
- **Dependencies**: AspNetCore.Tests, AspNetCore.Tests.SystemTextJson
- **Issues**: TFM update only
- **Validation**: System.Text.Json integration tests pass

**Batch Validation**:
- ? All 3 top-level test projects pass
- ? Full integration test coverage validated
- ? JSON serialization (both Newtonsoft and System.Text.Json) tested end-to-end

---

### Tiers 4-5 Completion Criteria

- ? All 6 projects build successfully
- ? SampleApp runs and responds correctly
- ? Full integration test suite (AspNetCore.Tests) passes
- ? JSON serialization integration tests pass
- ? No regressions across entire solution
- ? **All 43 projects** build together successfully

---

## Complete Project Migration Summary

### Migration Sequence Overview

| Tier | Level | Projects | Batches | Strategy | Key Focus |
|------|-------|----------|---------|----------|-----------|
| **Tier 1** | 0 | 5 | 2 | Foundation-first | Core (critical), package removals |
| **Tier 2 (High)** | 1 | 4 | Individual | Risk-first | Scheduler, BlobStorage, AzureSearch, NewtonsoftJson |
| **Tier 2 (Med/Low)** | 1 | 14 | 4 batches | Complexity-based | AspNetCore, EF, SmartEnums, tests |
| **Tier 3** | 2 | 16 | 2 batches | High-risk tests + extensions | Test projects, integrations |
| **Tier 4** | 3 | 3 | 1 batch | Applications | SampleApp, tests |
| **Tier 5** | 4-5 | 3 | 1 batch | Integration tests | Full test suite |
| **TOTAL** | 0-5 | **43** | **11 batches** | Bottom-up | Dependency-first, risk-aware |

### Package Update Summary

**Central Package Management**:
- **DotnetVersion**: `9.0.12` ? `10.0.2` (applies to all Microsoft.* packages)
- **Location**: `Directory.Packages.props`

**Packages to Update** (via DotnetVersion property):
- Microsoft.AspNetCore.Mvc.NewtonsoftJson
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- Microsoft.AspNetCore.Mvc.Testing
- Microsoft.EntityFrameworkCore (+ .InMemory, .SqlServer)
- Microsoft.Extensions.Configuration (+ .Abstractions, .Binder, .EnvironmentVariables, .UserSecrets)
- Microsoft.Extensions.DependencyInjection (+ .Abstractions)
- Microsoft.Extensions.FileProviders.Physical
- Microsoft.Extensions.Hosting.Abstractions
- Microsoft.Extensions.Logging (+ .Abstractions, .Console)
- Microsoft.Extensions.Options (+ .ConfigurationExtensions)
- System.Net.Http.Json
- System.Text.Json

**Packages to Remove**:
- System.ComponentModel.Annotations (from Core) ? Framework-included
- System.Security.Cryptography.Algorithms (from Randomness) ? Framework-included

**Packages Remaining Compatible** (no changes):
- Third-party packages: Ardalis.SmartEnum, Autofac, AutoMapper, Azure.*, CsvHelper, FluentValidation, Fluid.Core, MailKit, MediatR, Newtonsoft.Json, NSwag, NUnit, Quartz, Serilog, etc.

### Breaking Changes Catalog

**Total Issues**: 212 (55 mandatory, 157 potential)

**By Category**:
1. **Project.0002** - TFM Updates: 43 occurrences (all projects)
2. **Api.0003** - Behavioral Changes: 69 occurrences (13 projects)
3. **NuGet.0002** - Package Updates: 53 occurrences (via DotnetVersion)
4. **Api.0002** - Source Incompatible: 35 occurrences (8 projects)
5. **Api.0001** - Binary Incompatible: 10 occurrences (6 projects)
6. **NuGet.0003** - Framework-Included: 2 occurrences (2 projects)

**Critical Breaking Changes by Project**:
- **Scheduler**: ConfigurationErrorsException removed, ConfigurationBinder.Get<T> signature changed
- **BlobStorage**: Azure Storage SDK API changes (1 binary incompatible + 17 behavioral)
- **AzureSearch**: Azure Search SDK API changes (1 binary incompatible + 12 behavioral)
- **AspNetCore.Tests.NewtonsoftJson**: JSON serialization behavioral changes (17 occurrences)
- **Email, GraphApi, HealthChecks, MediatR.Tests**: Various binary incompatible API changes

### Estimated Timeline by Complexity

**Note**: No real-time estimates provided. Relative complexity only.

| Phase | Complexity | Relative Effort | Notes |
|-------|------------|-----------------|-------|
| Phase 1: Foundation | Medium | 15% | Core is critical, package removals |
| Phase 2: High-Risk | Very High | 35% | 4 projects individually, extensive validation |
| Phase 3: Medium/Low | Medium | 25% | Batched, but many projects (14) |
| Phase 4: Integrations | High | 15% | High-risk test projects |
| Phase 5: Applications | Low-Medium | 5% | Top-level, mostly integration |
| Phase 6: Validation | Medium | 5% | Full solution validation |

**Total Effort Distribution**: 
- **50%** effort on 7 high/very-high complexity projects
- **50%** effort on 36 low/medium complexity projects

---

---

## Risk Management

### High-Risk Changes

| Project | Risk Level | Issues | Description | Mitigation |
|---------|-----------|--------|-------------|------------|
| **AspNetCore.Tests.NewtonsoftJson** | VERY HIGH | 25 (1M) | Highest issue count; behavioral changes in JSON serialization | Individual upgrade with extensive testing; compare serialization outputs before/after |
| **BlobStorage** | HIGH | 19 (2M) | Binary + behavioral changes in Azure Storage SDK | Individual upgrade; validate against Azure Storage emulator before production |
| **AzureSearch** | HIGH | 14 (2M) | Binary + behavioral changes in Azure Cognitive Search SDK | Individual upgrade; test search queries thoroughly |
| **Scheduler** | HIGH | 12 (4M) | Binary + source + behavioral changes in Quartz.NET integration | Individual upgrade; validate job scheduling and execution |
| **BlobStorage.Tests** | HIGH | 19 (1M) | Tests for high-risk BlobStorage | Thorough validation of Azure SDK behavioral changes |
| **AzureSearch.Tests** | HIGH | 11 (1M) | Tests for high-risk AzureSearch | Validate search functionality with behavioral changes |
| **Scheduler.Tests** | HIGH | 10 (1M) | Tests for high-risk Scheduler | Validate job scheduling behavioral changes |
| **AspNetCore.Tests.Utilities** | MEDIUM | 9 (1M) | Behavioral changes affecting test infrastructure | Batch with medium-risk; validate test helper behavior |
| **AspNetCore.Tests.SystemTextJson** | MEDIUM | 9 (1M) | Behavioral changes in System.Text.Json serialization | Batch with medium-risk; validate JSON serialization |
| **Email** | MEDIUM | 7 (3M) | Binary incompatible changes in email functionality | Batch with medium-risk; test email sending/receiving |
| **AspNetCore.Tests** | MEDIUM | 7 (1M) | Integration test suite with behavioral changes | Validate after all dependencies upgraded |
| **GraphApi** | MEDIUM | 6 (2M) | Binary + behavioral changes in Microsoft Graph integration | Batch with medium-risk; test Graph API calls |

### Security Vulnerabilities

**Status**: ? **No security vulnerabilities detected**

All packages are up-to-date or have secure versions available in the upgrade path.

### Breaking Changes by Category

#### Binary Incompatible (Api.0001) - 10 occurrences
**Severity**: Mandatory - Requires recompilation

**Affected Projects**:
1. **Enigmatry.Entry.HealthChecks** (2 occurrences)
2. **Enigmatry.Entry.AzureSearch** (1 occurrence)
3. **Enigmatry.Entry.Email** (1 occurrence)
4. **Enigmatry.Entry.BlobStorage** (1 occurrence)
5. **Enigmatry.Entry.MediatR.Tests** (1 occurrence)
6. **Enigmatry.Entry.GraphApi** (1 occurrence)

**Impact**: These APIs have changed signatures or been removed. Recompilation required; code changes may be needed.

**Mitigation**:
- Identify specific API changes during compilation
- Consult .NET 10 breaking changes documentation
- Use IDE quick fixes and suggestions where available
- Test thoroughly after changes

---

#### Source Incompatible (Api.0002) - 35 occurrences
**Severity**: Potential - Requires code changes to compile

**Affected Projects**:
1. **Enigmatry.Entry.AspNetCore** (2 occurrences)
2. **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** (7 occurrences)
3. **Enigmatry.Entry.Infrastructure.Tests** (1 occurrence)
4. **Enigmatry.Entry.Scheduler.Tests** (5 occurrences)
5. **Enigmatry.Entry.TemplatingEngine.Razor.Tests** (5 occurrences)
6. **Enigmatry.Entry.AzureSearch.Tests** (10 occurrences)
7. **Enigmatry.Entry.Scheduler** (5 occurrences)

**Impact**: Code requires changes to compile successfully (method signature changes, obsolete API usage, namespace changes).

**Mitigation**:
- Allow compilation errors to surface
- Use compiler guidance to identify issues
- Update code incrementally, test after each change
- Document patterns for reuse across similar occurrences

---

#### Behavioral Changes (Api.0003) - 69 occurrences
**Severity**: Potential - Runtime behavior may differ

**Affected Projects** (top 10 by occurrence count):
1. **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** (17 occurrences)
2. **Enigmatry.Entry.BlobStorage.Tests** (18 occurrences)
3. **Enigmatry.Entry.BlobStorage** (17 occurrences)
4. **Enigmatry.Entry.AspNetCore.Tests.Utilities** (8 occurrences)
5. **Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests** (2 occurrences)
6. **Enigmatry.Entry.AspNetCore.Tests** (6 occurrences)
7. **Enigmatry.Entry.AzureSearch** (12 occurrences)
8. **Enigmatry.Entry.GraphApi** (4 occurrences)
9. **Enigmatry.Entry.Scheduler** (6 occurrences)
10. **Enigmatry.Entry.AspNetCore** (1 occurrence)

**Impact**: Code compiles but may behave differently at runtime (changed default values, different exception handling, modified algorithm behavior).

**Mitigation**:
- Comprehensive test coverage essential
- Compare behavior before/after upgrade where possible
- Review .NET 10 behavioral change documentation for affected APIs
- Manual testing of critical paths
- Performance benchmarking for algorithms that may have changed

---

#### Framework-Included Packages (NuGet.0003) - 2 occurrences
**Severity**: Mandatory - Must remove packages

**Packages to Remove**:
1. **System.ComponentModel.Annotations** (version 5.0.0)
   - Affected: **Enigmatry.Entry.Core**
   - Action: Remove package reference; functionality included in .NET 10 framework
2. **System.Security.Cryptography.Algorithms** (version 4.3.1)
   - Affected: **Enigmatry.Entry.Randomness**
   - Action: Remove package reference; functionality included in .NET 10 framework

**Mitigation**:
- Update **Directory.Packages.props** to remove these entries
- Verify no compilation errors after removal (framework provides functionality)
- Test affected functionality to ensure framework version behaves identically

### Contingency Plans

#### Blocking Issues During Upgrade

**Scenario**: A project fails to build after framework upgrade due to breaking changes

**Response**:
1. **Isolate**: Roll back only the failing project, keep completed projects on .NET 10
2. **Research**: Consult .NET 10 migration guide and breaking changes documentation
3. **Alternative APIs**: Identify replacement APIs or patterns
4. **Workaround**: If no immediate solution, document and escalate; consider temporary compatibility shims
5. **Timeline**: Set maximum 2 days for resolution before considering phase rollback

---

#### Performance Regression

**Scenario**: Upgraded project shows significant performance degradation

**Response**:
1. **Baseline**: Capture performance metrics from .NET 9 version
2. **Profile**: Use .NET profiling tools to identify bottleneck
3. **Compare**: Check .NET 10 release notes for known performance changes in affected areas
4. **Optimize**: Apply .NET 10-specific optimizations if available
5. **Escalate**: If regression >20% and no solution in 3 days, consider filing issue with .NET team

---

#### Test Failures

**Scenario**: Tests pass on .NET 9 but fail on .NET 10

**Response**:
1. **Categorize**: Behavioral change vs actual bug
2. **Behavioral Changes**: Update tests to reflect new expected behavior (if correct)
3. **Bugs**: Fix code to work correctly with .NET 10 semantics
4. **False Failures**: Update test infrastructure if test assumptions changed
5. **Block**: Do not proceed to next tier with failing tests

---

#### Package Dependency Conflicts

**Scenario**: Package version conflicts after updating to .NET 10 versions

**Response**:
1. **Identify**: Use `dotnet list package` to find conflicts
2. **Consolidate**: Update **Directory.Packages.props** to use single compatible version
3. **Research**: Check package compatibility matrices
4. **Alternatives**: Consider different package if conflicts unresolvable
5. **Vendor Support**: Contact package maintainer if package doesn't support .NET 10

### Rollback Strategy

#### Per-Phase Rollback

If a phase fails validation:
1. **Do Not Proceed**: Stop before starting next phase
2. **Fix in Place**: Address issues in current phase
3. **Re-validate**: Run full phase validation again
4. **Document**: Record issues and resolutions for future reference

#### Full Rollback

If critical blocker prevents continuation:
1. **Branch Preservation**: Original .NET 9 code remains on source branch
2. **Selective Merge**: Can cherry-pick completed, validated phases if needed
3. **Documentation**: Capture all learnings and blockers for retry planning

**Rollback is per-phase, not per-project**: The bottom-up approach ensures each tier is stable before proceeding, minimizing need for full rollback.

---

## Testing & Validation Strategy

### Phase-by-Phase Testing Requirements

#### Phase 1: Foundation Testing
**After upgrading Tier 1 (Level 0) projects**

**Smoke Tests**:
- ? **Enigmatry.Entry.Core** builds without errors or warnings
- ? Core.Tests passes completely
- ? Csv, MediatR, Randomness, Swagger build successfully
- ? Csv.Tests, MediatR.Tests pass completely
- ? No package version conflicts in solution

**Comprehensive Validation**:
- ? All 5 Tier 1 projects build in clean solution
- ? All tests in Tier 1 test projects (Core.Tests, Csv.Tests, MediatR.Tests) pass
- ? Verify package removals: System.ComponentModel.Annotations, System.Security.Cryptography.Algorithms
- ? No compilation warnings related to obsolete APIs
- ? Run solution build to verify no downstream impacts (should fail on Tier 2+ as expected)

---

#### Phase 2: High-Risk Core Extensions Testing
**After upgrading Scheduler, BlobStorage, AzureSearch (individually)**

**Per-Project Validation (repeat for each)**:

**Enigmatry.Entry.Scheduler**:
- ? Builds without errors or warnings
- ? All binary incompatible API changes resolved
- ? All source incompatible API changes resolved
- ? Job scheduling and execution tested manually
- ? Behavioral changes documented and verified as acceptable
- ? Integration with Quartz.NET validated

**Enigmatry.Entry.BlobStorage**:
- ? Builds without errors or warnings
- ? All API breaking changes resolved
- ? Upload/download operations tested against Azure Storage emulator
- ? Behavioral changes in Azure SDK verified
- ? Connection string handling validated
- ? BlobStorage.Tests passes completely

**Enigmatry.Entry.AzureSearch**:
- ? Builds without errors or warnings
- ? All API breaking changes resolved
- ? Search query operations tested
- ? Index management validated
- ? Behavioral changes in Azure Cognitive Search SDK verified
- ? AzureSearch.Tests passes completely

**Phase Completion**:
- ? All 3 high-risk projects build together
- ? All associated tests pass
- ? No regressions in Tier 1 projects (re-run Tier 1 tests)

---

#### Phase 3: Medium/Low-Risk Core Extensions Testing
**After upgrading Tier 2 batches**

**Per-Batch Smoke Tests**:
- ? All projects in batch build without errors
- ? No package conflicts introduced
- ? Quick validation of key functionality per project

**Batch 1 (Medium-Risk) Validation**:
- ? Email, GraphApi, AspNetCore.Tests.Utilities build together
- ? Email.Tests passes (validate email sending)
- ? GraphApi tested against Microsoft Graph (if available)
- ? Test utilities validated by running dependent tests

**Batch 2 (High-Impact Low-Risk) Validation**:
- ? AspNetCore, Core.EntityFramework, SmartEnums build together
- ? Verify these projects don't introduce breaking changes for consumers
- ? Run quick integration tests with Tier 1

**Batch 3 (Low-Risk Libraries) Validation**:
- ? All 5 projects build together
- ? Templating engines tested with sample templates
- ? HealthChecks tested with sample health check scenarios

**Batch 4 (Tests) Validation**:
- ? All Tier 1 test projects pass

**Phase Completion**:
- ? All 14 Tier 2 projects build together
- ? All Tier 1 + Tier 2 projects build in clean solution (29 projects)
- ? All tests in Tiers 1-2 pass
- ? No regressions

---

#### Phase 4: Integration Extensions & Tests Testing
**After upgrading Tier 3 (Level 2) projects**

**Very High-Risk Individual Validation**:

**Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson**:
- ? Builds without errors or warnings
- ? All behavioral changes in JSON serialization documented
- ? Compare serialization output .NET 9 vs .NET 10 for sample objects
- ? Validate JSON schema compatibility
- ? Test edge cases: nulls, complex objects, polymorphism, date formats
- ? AspNetCore.Tests.NewtonsoftJson.Tests passes completely
- ?? **Critical**: Allocate extra time for this project

**Batch 1 (High-Risk Tests) Validation**:
- ? BlobStorage.Tests, AzureSearch.Tests, Scheduler.Tests, AspNetCore.Tests.SystemTextJson build together
- ? All tests pass
- ? Verify behavioral changes in test assertions updated appropriately

**Batch 2 (Low-Risk Extensions) Validation**:
- ? All 11 projects build together
- ? Authorization functionality tested
- ? EntityFramework integration tested with sample DbContext
- ? SmartEnums extensions validated
- ? All test projects pass

**Phase Completion**:
- ? All 16 Tier 3 projects build together
- ? All Tiers 1-3 projects build in clean solution (45 projects total)
- ? All tests in Tiers 1-3 pass
- ? No regressions in lower tiers

---

#### Phase 5: Application & Top-Level Integration Testing
**After upgrading Tiers 4-5 (Levels 3-5) projects**

**Batch 1 (Level 3) Validation**:
- ? AspNetCore.Tests.SampleApp runs successfully
- ? Sample app health checks respond correctly
- ? Sample app Swagger UI accessible
- ? AspNetCore.Authorization.Tests, EntityFramework.Tests pass

**Batch 2 (Levels 4-5) Validation**:
- ? AspNetCore.Tests (full integration suite) passes
- ? NewtonsoftJson.Tests, SystemTextJson.Tests pass
- ? End-to-end scenarios validated through integration tests

**Phase Completion**:
- ? All 6 top-level projects build together
- ? All 43 projects build in clean solution
- ? All tests across entire solution pass

---

### Comprehensive Validation (Phase 6)

#### Full Solution Build

```bash
# Clean and rebuild entire solution
dotnet clean Enigmatry.Entry.sln
dotnet build Enigmatry.Entry.sln --configuration Release
```

**Success Criteria**:
- ? 0 errors
- ? 0 warnings (or only acceptable warnings documented)
- ? All 43 projects build successfully

---

#### Complete Test Suite

```bash
# Run all tests in solution
dotnet test Enigmatry.Entry.sln --configuration Release --logger "console;verbosity=detailed"
```

**Success Criteria**:
- ? All tests pass (0 failures)
- ? 0 skipped tests (unless intentional)
- ? Test execution time comparable to .NET 9 baseline (±10%)

---

#### Package Validation

```bash
# List all packages and check for conflicts
dotnet list package --include-transitive
dotnet list package --vulnerable
dotnet list package --deprecated
```

**Success Criteria**:
- ? No package version conflicts
- ? No vulnerable packages
- ? No deprecated packages (or documented exceptions)
- ? All Microsoft.* packages at version 10.0.2 (via DotnetVersion property)

---

#### Code Quality Checks

**Compilation Warnings**:
- ? No new warnings introduced vs .NET 9 baseline
- ? Obsolete API warnings addressed or documented as acceptable

**Analyzer Warnings**:
- ? Run Roslyn analyzers, verify no new issues
- ? Code quality maintained or improved

---

#### Performance Validation (Optional but Recommended)

**Benchmarks** (if baseline exists):
- Compare key operation performance .NET 9 vs .NET 10
- Acceptable: ±10% variance
- Flag: >10% degradation for investigation

**Memory**:
- Monitor memory usage for long-running operations
- Verify no memory leaks introduced

---

#### Documentation Validation

- ? README.md updated with .NET 10 requirement
- ? Breaking changes documented
- ? Migration notes created
- ? CHANGELOG.md updated
- ? Package dependency matrix updated

---

### Testing Tools & Commands

**Build Validation**:
```bash
# Per-project build
dotnet build <project-path> --configuration Release

# Solution build
dotnet build Enigmatry.Entry.sln --configuration Release
```

**Test Execution**:
```bash
# Per-project tests
dotnet test <test-project-path> --configuration Release

# Solution tests
dotnet test Enigmatry.Entry.sln --configuration Release

# With coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

**Package Analysis**:
```bash
# List packages
dotnet list package

# Check for vulnerabilities
dotnet list package --vulnerable

# Check for updates
dotnet list package --outdated
```

**Cleanup**:
```bash
# Clean solution
dotnet clean Enigmatry.Entry.sln

# Remove bin/obj folders
Get-ChildItem -Recurse -Directory -Filter bin | Remove-Item -Recurse -Force
Get-ChildItem -Recurse -Directory -Filter obj | Remove-Item -Recurse -Force
```

---

### Regression Testing Strategy

**After Each Phase**:
1. **Re-run previous phase tests**: Verify no regressions in completed tiers
2. **Integration smoke tests**: Quick validation of cross-tier integration
3. **Build all completed tiers**: Ensure backward compatibility

**Red Flags** (stop and investigate):
- ? Previously passing tests now fail
- ? New compilation errors in previously upgraded projects
- ? Package conflicts introduced
- ? Performance degradation >20%

**Resolution**:
- Do not proceed to next phase with regressions
- Isolate issue to specific change
- Fix or roll back problematic change
- Re-validate before proceeding

---

## Complexity & Effort Assessment

### Per-Project Complexity

**Complexity Rating Methodology**:
- **Low**: 1-3 issues, mostly mandatory TFM updates, no breaking changes
- **Medium**: 4-9 issues, some API changes, moderate testing required
- **High**: 10-19 issues, multiple breaking changes, extensive validation needed
- **Very High**: 20+ issues, complex behavioral changes, thorough testing critical

#### Tier 1 (Level 0) - Foundation Projects

| Project | Issues | Complexity | Dependencies (Internal) | Risk Factors |
|---------|--------|------------|------------------------|--------------|
| **Enigmatry.Entry.Core** | 3 (2M) | **Medium** | None | Foundation for 15 projects; package removal required |
| Enigmatry.Entry.Csv | 1 (1M) | Low | None | Simple TFM update |
| Enigmatry.Entry.MediatR | 2 (1M) | Low | None | Simple TFM update + package update |
| Enigmatry.Entry.Randomness | 2 (2M) | Medium | None | Package removal required |
| Enigmatry.Entry.Swagger | 2 (1M) | Low | None | Simple TFM update + package update |

#### Tier 2 (Level 1) - Core Extensions

| Project | Issues | Complexity | Dependencies (Internal) | Risk Factors |
|---------|--------|------------|------------------------|--------------|
| **Enigmatry.Entry.Scheduler** | 12 (4M) | **High** | Core | Binary + source + behavioral changes |
| **Enigmatry.Entry.BlobStorage** | 19 (2M) | **High** | Core | Highest issue count in tier; Azure SDK changes |
| **Enigmatry.Entry.AzureSearch** | 14 (2M) | **High** | Core | Multiple API breaking changes |
| Enigmatry.Entry.Email | 7 (3M) | Medium | Core | Binary incompatible changes |
| Enigmatry.Entry.GraphApi | 6 (2M) | Medium | Core | Binary + behavioral changes |
| Enigmatry.Entry.AspNetCore.Tests.Utilities | 9 (1M) | Medium | Core | Test infrastructure behavioral changes |
| Enigmatry.Entry.AspNetCore | 3 (1M) | Low | Core | Used by 3 projects |
| Enigmatry.Entry.Core.EntityFramework | 2 (1M) | Low | Core | Used by 2 projects |
| Enigmatry.Entry.SmartEnums | 1 (1M) | Low | Core | Used by 4 projects |
| Enigmatry.Entry.TemplatingEngine.Fluid | 4 (1M) | Low | Core | Package updates |
| Enigmatry.Entry.TemplatingEngine.Razor | 3 (1M) | Low | Core | Package updates |
| Enigmatry.Entry.HealthChecks | 2 (2M) | Low | Core | Binary incompatible changes |
| Enigmatry.Entry.Infrastructure | 1 (1M) | Low | Core | TFM update only |
| Enigmatry.Entry.Core.Tests | 1 (1M) | Low | Core | Test project |
| Enigmatry.Entry.Csv.Tests | 1 (1M) | Low | Csv | Test project |
| Enigmatry.Entry.MediatR.Tests | 2 (2M) | Low | MediatR | Binary incompatible changes |

#### Tier 3 (Level 2) - Integrations & Extensions

| Project | Issues | Complexity | Dependencies (Internal) | Risk Factors |
|---------|--------|------------|------------------------|--------------|
| **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** | 25 (1M) | **Very High** | AspNetCore.Tests.Utilities | Highest issue count in solution; JSON behavioral changes |
| **Enigmatry.Entry.BlobStorage.Tests** | 19 (1M) | **High** | BlobStorage | Tests for high-risk project |
| **Enigmatry.Entry.AzureSearch.Tests** | 11 (1M) | **High** | AzureSearch | Tests for high-risk project; source incompatibilities |
| **Enigmatry.Entry.Scheduler.Tests** | 10 (1M) | **High** | Scheduler | Tests for high-risk project; source incompatibilities |
| Enigmatry.Entry.AspNetCore.Tests.SystemTextJson | 9 (1M) | Medium | AspNetCore.Tests.Utilities | JSON serialization behavioral changes |
| Enigmatry.Entry.TemplatingEngine.Razor.Tests | 5 (1M) | Medium | TemplatingEngine.Razor, Core | Source incompatibilities |
| Enigmatry.Entry.EntityFramework | 4 (1M) | Low | Core.EntityFramework | Package updates |
| Enigmatry.Entry.Email.Tests | 3 (1M) | Low | Email | Test project |
| Enigmatry.Entry.Infrastructure.Tests | 3 (1M) | Low | Infrastructure | Source incompatibility |
| Enigmatry.Entry.EntityFramework.Tests | 3 (1M) | Low | EntityFramework | Test project |
| Enigmatry.Entry.TemplatingEngine.Fluid.Tests | 3 (1M) | Low | TemplatingEngine.Fluid | Test project |
| Enigmatry.Entry.SmartEnums.EntityFramework | 2 (1M) | Low | Core.EntityFramework, SmartEnums | Package updates |
| Enigmatry.Entry.AspNetCore.Authorization | 1 (1M) | Low | AspNetCore | Used by 2 projects |
| Enigmatry.Entry.SmartEnums.Swagger | 1 (1M) | Low | SmartEnums | TFM update only |
| Enigmatry.Entry.SmartEnums.SystemTextJson | 1 (1M) | Low | SmartEnums | TFM update only |
| Enigmatry.Entry.SmartEnums.VerifyTests | 1 (1M) | Low | SmartEnums | TFM update only |
| Enigmatry.Entry.HealthChecks.Tests | 1 (1M) | Low | HealthChecks | Test project |

#### Tier 4 (Level 3) - Application Layer

| Project | Issues | Complexity | Dependencies (Internal) | Risk Factors |
|---------|--------|------------|------------------------|--------------|
| Enigmatry.Entry.AspNetCore.Tests.SampleApp | 2 (1M) | Low | HealthChecks, AspNetCore, Swagger, Authorization | Sample application |
| Enigmatry.Entry.AspNetCore.Authorization.Tests | 1 (1M) | Low | AspNetCore.Authorization | Test project |
| Enigmatry.Entry.EntityFramework.Tests | 3 (1M) | Low | EntityFramework | Test project (duplicate in Tier 3?) |

#### Tier 5 (Levels 4-5) - Integration Tests

| Project | Issues | Complexity | Dependencies (Internal) | Risk Factors |
|---------|--------|------------|------------------------|--------------|
| Enigmatry.Entry.AspNetCore.Tests | 7 (1M) | Medium | AspNetCore, SampleApp | Integration test suite with behavioral changes |
| Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson.Tests | 2 (1M) | Low | AspNetCore.Tests, NewtonsoftJson | Test project |
| Enigmatry.Entry.AspNetCore.Tests.SystemTextJson.Tests | 1 (1M) | Low | AspNetCore.Tests, SystemTextJson | Test project |

---

### Phase Complexity Assessment

| Phase | Projects | Total Issues | Complexity | Estimated Effort (Relative) | Notes |
|-------|----------|--------------|------------|----------------------------|-------|
| **Phase 1: Foundation** | 5 | 10 | Medium | Medium | Core is critical; package removals required |
| **Phase 2: High-Risk Core** | 3 | 45 | Very High | High | Individual attention per project; 12-19 issues each |
| **Phase 3: Medium/Low Core** | 14 | 52 | Medium | High | Batched execution; moderate issues per project |
| **Phase 4: Integrations** | 16 | 90 | High | Very High | Includes very high-risk project (25 issues) |
| **Phase 5: Applications** | 6 | 15 | Low-Medium | Medium | Top-level integration testing |
| **Phase 6: Validation** | All (43) | - | Medium | Medium | Full solution validation |

**Total Complexity**: **High** - Requires careful phased approach with extensive validation

---

### Resource Requirements

#### Skill Levels Required

**Technical Skills**:
- ? **.NET 10 Breaking Changes Knowledge**: Understanding of API changes, behavioral differences
- ? **Azure SDK Experience**: For BlobStorage, AzureSearch migrations
- ? **Entity Framework Core**: For EF-related projects
- ? **JSON Serialization Expertise**: For NewtonsoftJson/System.Text.Json behavioral changes
- ? **Testing Frameworks**: NUnit, FakeItEasy for test project updates
- ? **Central Package Management**: Understanding of Directory.Packages.props

**Recommended Team Structure**:
- **Lead Developer**: Oversees entire migration, handles high-risk projects
- **Core/Infrastructure Developer**: Tier 1 & 2 foundation projects
- **Azure Specialist**: BlobStorage, AzureSearch projects
- **Testing Specialist**: Test project validation, behavioral change verification

#### Parallel Work Capacity

**Maximum Parallel Streams**: 2-3 developers can work concurrently within same tier on independent projects

**Examples**:
- **Tier 1**: One developer on Core (critical path), another on Csv/MediatR/Randomness/Swagger
- **Tier 2 Batch 2**: Three developers can split 7 low-risk projects (2-3 projects each)
- **Tier 3 Batch 2**: Two developers can handle 11 low-risk projects (5-6 each)

**Sequential Bottlenecks**:
- Core must complete before any Tier 2 work begins
- High-risk projects (BlobStorage, AzureSearch, Scheduler, AspNetCore.Tests.NewtonsoftJson) should be handled sequentially for focused attention

---

### Relative Effort Summary

**Complexity Distribution**:
- **Very High Complexity**: 1 project (2% of total, 12% of issues)
- **High Complexity**: 6 projects (14% of total, 35% of issues)
- **Medium Complexity**: 9 projects (21% of total, 25% of issues)
- **Low Complexity**: 27 projects (63% of total, 28% of issues)

**Effort Concentration**:
- **80% of effort** will be in high and very-high complexity projects (7 projects, 47% of issues)
- **20% of effort** across low-complexity projects (27 projects, simple TFM updates)

**Critical Path Focus**:
1. **Enigmatry.Entry.Core** - Blocks all other work
2. **High-risk trio** (BlobStorage, AzureSearch, Scheduler) - Require most validation effort
3. **Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson** - Single most complex project

**Recommendation**: Allocate most experienced developers to critical path and high-risk projects; junior developers can handle low-complexity batches with supervision.

---

## Source Control Strategy

### Branching Strategy

**Working Branch**: `features/BP-1565-add-net10-support` (current branch)

**Approach**: Direct work on feature branch (as requested by user)

**No New Branches**: User has already created and switched to the target branch; all upgrade work happens here.

---

### Commit Strategy

#### Commit Frequency

**Per-Phase Commits**: Commit at the completion of each major phase

**Recommended Commit Points**:
1. After Phase 1 completion (Tier 1 - Foundation)
2. After each high-risk project in Phase 2 (Scheduler, BlobStorage, AzureSearch)
3. After Phase 3 completion (Tier 2 - Medium/Low Risk)
4. After Phase 4 completion (Tier 3 - Integrations)
5. After Phase 5 completion (Tiers 4-5 - Applications)
6. After Phase 6 validation (Final)

**Checkpoint Commits**: If a phase takes multiple days, commit at end of day with WIP (Work In Progress) marker if incomplete

---

#### Commit Message Format

**Template**:
```
[.NET 10 Upgrade] <Phase/Scope>: <Summary>

<Details>

Phase: <Phase Number>
Projects: <Count> (<Names if few>)
Validation: <Status>
```

**Examples**:

```
[.NET 10 Upgrade] Phase 1: Foundation projects upgraded to .NET 10

Upgraded 5 foundation projects (Level 0):
- Enigmatry.Entry.Core
- Enigmatry.Entry.Csv
- Enigmatry.Entry.MediatR
- Enigmatry.Entry.Randomness
- Enigmatry.Entry.Swagger

Removed framework-included packages:
- System.ComponentModel.Annotations (Core)
- System.Security.Cryptography.Algorithms (Randomness)

Phase: 1/6
Projects: 5/43
Validation: All builds pass, all tests pass ?
```

```
[.NET 10 Upgrade] Phase 2: Scheduler - High-risk project migrated

Upgraded Enigmatry.Entry.Scheduler with 12 issues (4 mandatory):
- Resolved 4 binary incompatible API changes
- Updated Quartz.NET integration for .NET 10
- Validated job scheduling and execution

Breaking changes documented in migration notes.

Phase: 2/6 (1 of 3 high-risk)
Projects: 6/43
Validation: Builds pass, tests pass, behavioral changes verified ?
```

```
[.NET 10 Upgrade] Phase 6: Final validation and documentation

Completed migration of all 43 projects to .NET 10.0 LTS.

- All projects build successfully
- All tests pass (0 failures)
- No package conflicts or vulnerabilities
- Documentation updated
- Migration notes finalized

Phase: 6/6 COMPLETE
Projects: 43/43 ?
Validation: Full solution validated ?
```

---

### Central Package Management Updates

**Directory.Packages.props Changes**:

All package version updates must be committed together with their consuming projects to maintain consistency.

**Key Update**:
```xml
<!-- Update DotnetVersion property -->
<DotnetVersion>10.0.2</DotnetVersion>
```

**Commit Approach**:
- Update DotnetVersion property in **first commit** (Phase 1) alongside TFM updates
- This cascades to all Microsoft.Extensions.*, Microsoft.EntityFrameworkCore.*, etc.
- Individual package version additions/removals committed as needed per phase

**Example Directory.Packages.props Commit**:
```
[.NET 10 Upgrade] Update DotnetVersion to 10.0.2 and remove framework-included packages

Updated Directory.Packages.props:
- DotnetVersion: 9.0.12 ? 10.0.2
- Removed: System.ComponentModel.Annotations (included in framework)
- Removed: System.Security.Cryptography.Algorithms (included in framework)

This cascades to all Microsoft.* package references across solution.

Phase: 1/6
```

---

### Review and Merge Process

#### Self-Review Checklist (Before Completing Phase)

- [ ] All projects in phase build without errors
- [ ] All projects in phase build without warnings (or warnings documented)
- [ ] All tests pass
- [ ] No package conflicts introduced
- [ ] Breaking changes documented
- [ ] Commit message descriptive and follows format
- [ ] No regressions in previous phases (re-run tests)

#### Pull Request (When Complete)

**PR Title**: `.NET 10 Upgrade - All 43 Projects Migrated`

**PR Description Template**:
```markdown
## Summary
Upgraded Enigmatry Entry Building Blocks solution from .NET 9 to .NET 10.0 LTS.

## Scope
- **Projects Upgraded**: 43/43
- **Issues Addressed**: 212 (55 mandatory, 157 potential)
- **Security Vulnerabilities**: 0 (none detected)

## Migration Approach
- **Strategy**: Bottom-up incremental migration (dependency-first)
- **Phases**: 6 phases, tier-by-tier progression
- **High-Risk Projects**: Handled individually with extensive validation
  - BlobStorage (19 issues)
  - AzureSearch (14 issues)
  - Scheduler (12 issues)
  - AspNetCore.Tests.NewtonsoftJson (25 issues)

## Changes

### Framework Updates
- All projects: .NET 9 ? .NET 10.0 (`TargetFramework` in .csproj files)

### Package Updates
- **DotnetVersion property**: `9.0.12` ? `10.0.2` (Directory.Packages.props)
- **Removed** (framework-included):
  - System.ComponentModel.Annotations
  - System.Security.Cryptography.Algorithms
- **Updated**: All Microsoft.* packages via DotnetVersion property

### Breaking Changes Addressed
- **Binary Incompatible**: 10 occurrences resolved
- **Source Incompatible**: 35 occurrences resolved
- **Behavioral Changes**: 69 occurrences validated and documented

### Testing
- ? Full solution build succeeds (0 errors, 0 warnings)
- ? All tests pass (0 failures)
- ? No package conflicts
- ? No security vulnerabilities

## Validation Evidence
```bash
dotnet build Enigmatry.Entry.sln --configuration Release
# Output: Build succeeded. 0 Warning(s). 0 Error(s).

dotnet test Enigmatry.Entry.sln --configuration Release
# Output: Passed! - <Total Tests> passed
```

## Documentation
- [x] Migration notes created
- [x] Breaking changes documented
- [x] README.md updated
- [x] CHANGELOG.md updated

## Review Focus Areas
1. **High-risk projects**: BlobStorage, AzureSearch, Scheduler, AspNetCore.Tests.NewtonsoftJson
2. **Directory.Packages.props**: Verify DotnetVersion and package removals
3. **Behavioral changes**: Especially in JSON serialization (NewtonsoftJson, System.Text.Json)
4. **Test coverage**: Ensure no tests were disabled or skipped inappropriately

## Rollback Plan
Original .NET 9 code preserved on source branch if rollback needed.
```

#### Review Criteria

**Reviewers should verify**:
- [ ] All 43 projects build successfully
- [ ] All tests pass
- [ ] Directory.Packages.props correctly updated (DotnetVersion, removed packages)
- [ ] No package version conflicts
- [ ] No new security vulnerabilities
- [ ] Breaking changes appropriately addressed
- [ ] High-risk projects (4) validated thoroughly
- [ ] Documentation complete

**Merge Criteria**:
- ? At least one approval from senior developer
- ? CI/CD pipeline passes (if configured)
- ? All review comments addressed
- ? No unresolved conversations

---

### Merge Strategy

**Target Branch**: Main/master branch (after review approval)

**Merge Method**: 
- **Squash merge** if commit history is verbose (many WIP commits)
- **Merge commit** if commits are clean and follow structure above

**Post-Merge**:
- Tag release: `v<version>-net10` (e.g., `v2.0.0-net10`)
- Create release notes from PR description
- Update any downstream consumers documentation

---

## Success Criteria

### Technical Criteria

#### Framework Migration

- ? **All 43 projects** target .NET 10.0
  - Verification: Check `<TargetFramework>net10.0</TargetFramework>` in all .csproj files
  - Tool: `grep -r "TargetFramework" --include="*.csproj"`

- ? **All project files** successfully updated
  - No projects remaining on .NET 9
  - No multi-targeting unless explicitly required

---

#### Package Updates

- ? **DotnetVersion property** updated to `10.0.2` in Directory.Packages.props
  - Cascades to all Microsoft.Extensions.*, Microsoft.EntityFrameworkCore.*, etc.

- ? **All recommended package updates** applied (23 packages)
  - Microsoft.AspNetCore.Mvc.NewtonsoftJson: 9.0.12 ? 10.0.2
  - Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation: 9.0.12 ? 10.0.2
  - Microsoft.AspNetCore.Mvc.Testing: 9.0.12 ? 10.0.2
  - Microsoft.EntityFrameworkCore.*: 9.0.12 ? 10.0.2
  - Microsoft.Extensions.*: 9.0.12 ? 10.0.2
  - System.Text.Json: 9.0.12 ? 10.0.2
  - System.Net.Http.Json: 9.0.12 ? 10.0.2

- ? **Framework-included packages removed** (2 packages)
  - System.ComponentModel.Annotations (from Core)
  - System.Security.Cryptography.Algorithms (from Randomness)

- ? **No package version conflicts**
  - Verification: `dotnet list package --include-transitive` shows no conflicts

---

#### Build Success

- ? **All 43 projects build without errors**
  ```bash
  dotnet build Enigmatry.Entry.sln --configuration Release
  # Expected: Build succeeded. 0 Error(s).
  ```

- ? **All 43 projects build without warnings**
  - Or warnings documented as acceptable and non-blocking
  - No obsolete API warnings unaddressed

- ? **Clean solution build succeeds**
  ```bash
  dotnet clean && dotnet build Enigmatry.Entry.sln
  # Expected: Build succeeded.
  ```

---

#### Test Success

- ? **All tests pass** (0 failures)
  ```bash
  dotnet test Enigmatry.Entry.sln --configuration Release
  # Expected: Passed! - All tests passed
  ```

- ? **No skipped tests** (unless documented)
  - All test projects execute successfully
  - Test count comparable to .NET 9 baseline

- ? **Test execution time** within acceptable range
  - ±10% of .NET 9 baseline (if measured)

---

#### Dependency & Security

- ? **No package dependency conflicts**
  ```bash
  dotnet list package --include-transitive
  # Expected: No warnings about version conflicts
  ```

- ? **No security vulnerabilities**
  ```bash
  dotnet list package --vulnerable
  # Expected: No vulnerable packages found
  ```

- ? **No deprecated packages** (or documented exceptions)
  ```bash
  dotnet list package --deprecated
  # Expected: No deprecated packages found
  ```

---

### Quality Criteria

#### Code Quality

- ? **Breaking changes addressed**
  - All mandatory issues (55) resolved
  - Binary incompatible APIs (10) updated
  - Source incompatible APIs (35) resolved or documented
  - Behavioral changes (69) validated and documented

- ? **Code compiles cleanly**
  - No suppressed warnings without justification
  - No commented-out code from migration attempts

- ? **Roslyn analyzers satisfied**
  - No new analyzer warnings introduced
  - Code quality maintained or improved

---

#### Testing Quality

- ? **Test coverage maintained**
  - No tests removed or disabled inappropriately
  - Coverage metrics comparable to .NET 9 baseline (if measured)

- ? **Behavioral changes validated**
  - Especially in high-risk projects:
    - BlobStorage behavioral changes verified
    - AzureSearch behavioral changes verified
    - Scheduler behavioral changes verified
    - AspNetCore.Tests.NewtonsoftJson JSON serialization changes verified
    - System.Text.Json behavioral changes verified

- ? **Integration tests pass**
  - AspNetCore.Tests full suite passes
  - Sample app (AspNetCore.Tests.SampleApp) runs correctly
  - End-to-end scenarios validated

---

#### Documentation

- ? **README.md updated**
  - .NET 10 requirement documented
  - Installation/setup instructions current

- ? **Migration notes created**
  - Breaking changes catalog
  - Behavioral changes documented
  - Lessons learned captured

- ? **CHANGELOG.md updated**
  - Version bump noted
  - .NET 10 migration noted
  - Breaking changes listed

- ? **Package dependency matrix updated**
  - If maintained, reflects .NET 10 package versions

---

### Process Criteria

#### Bottom-Up Strategy Adherence

- ? **Tier-by-tier completion**
  - Tier 1 (Level 0): 5 projects ?
  - Tier 2 (Level 1): 17 projects ?
  - Tier 3 (Level 2): 16 projects ?
  - Tier 4 (Level 3): 3 projects ?
  - Tier 5 (Levels 4-5): 3 projects ?

- ? **Dependencies upgraded before consumers**
  - No consumer upgraded before its dependencies
  - No multi-targeting used

- ? **Validation gates honored**
  - Each tier validated before proceeding to next
  - No regressions carried forward

---

#### High-Risk Project Handling

- ? **High-risk projects upgraded individually** (4 projects)
  - Enigmatry.Entry.Scheduler ?
  - Enigmatry.Entry.BlobStorage ?
  - Enigmatry.Entry.AzureSearch ?
  - Enigmatry.Entry.AspNetCore.Tests.NewtonsoftJson ?

- ? **Each high-risk project validated thoroughly**
  - Dedicated testing performed
  - Behavioral changes documented
  - API breaking changes resolved

---

#### Source Control

- ? **Clean commit history**
  - Phase completions marked with commits
  - Commit messages follow format
  - WIP commits squashed (if merge strategy chosen)

- ? **All changes on feature branch**
  - Branch: `features/BP-1565-add-net10-support`
  - Ready for PR/merge to main

- ? **Directory.Packages.props updates committed**
  - DotnetVersion property updated
  - Package removals committed
  - Consistency maintained throughout

---

### Final Validation Checklist

**Before considering migration complete, verify**:

#### Build & Test
- [ ] `dotnet clean Enigmatry.Entry.sln` succeeds
- [ ] `dotnet build Enigmatry.Entry.sln --configuration Release` succeeds with 0 errors, 0 warnings
- [ ] `dotnet test Enigmatry.Entry.sln --configuration Release` passes with 0 failures
- [ ] `dotnet list package` shows no conflicts
- [ ] `dotnet list package --vulnerable` shows no vulnerabilities

#### Project Files
- [ ] All 43 .csproj files have `<TargetFramework>net10.0</TargetFramework>`
- [ ] No .csproj files have explicit package versions (managed by Directory.Packages.props)
- [ ] Directory.Packages.props has `<DotnetVersion>10.0.2</DotnetVersion>`
- [ ] System.ComponentModel.Annotations removed from Core
- [ ] System.Security.Cryptography.Algorithms removed from Randomness

#### Functionality
- [ ] High-risk projects validated individually:
  - [ ] Scheduler: Job scheduling works
  - [ ] BlobStorage: Upload/download operations work
  - [ ] AzureSearch: Search queries work
  - [ ] AspNetCore.Tests.NewtonsoftJson: JSON serialization correct
- [ ] Sample app runs: `dotnet run --project Enigmatry.Entry.AspNetCore.Tests.SampleApp`
- [ ] Integration tests pass: All AspNetCore.Tests.* test projects

#### Documentation
- [ ] README.md mentions .NET 10
- [ ] Migration notes document breaking changes
- [ ] CHANGELOG.md updated
- [ ] Behavioral changes documented (especially JSON, Azure SDKs, Scheduler)

#### Source Control
- [ ] All changes committed to `features/BP-1565-add-net10-support`
- [ ] Commit messages descriptive
- [ ] No uncommitted changes (or documented if intentional)

---

### Success Declaration

**The .NET 10 upgrade is successfully complete when**:

? **All technical criteria met**: All 43 projects on .NET 10, building, and testing successfully  
? **All quality criteria met**: Code quality maintained, behavioral changes validated  
? **All process criteria met**: Bottom-up strategy followed, high-risk projects handled appropriately  
? **Final validation checklist complete**: All items checked and verified

**At this point**: Solution is ready for merge to main branch, release, and deployment.
