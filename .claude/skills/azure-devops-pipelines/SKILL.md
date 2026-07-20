---
name: azure-devops-pipelines
description: Best practices for Azure DevOps Pipeline YAML files in the Enigmatry Entry Building Blocks project. Use this when creating, editing, or reviewing Azure DevOps CI/CD pipeline YAML.
---

# Entry Building Blocks Azure DevOps Pipelines

## Pipeline files

All pipeline YAML lives in `Pipelines/`:

| File | Purpose |
|------|---------|
| `azure-pipelines.yml` | Main CI pipeline — build → test → pack → publish to NuGet |
| `code-analysis.yml` | Daily scheduled SonarQube + build-report analysis |

Shared pipeline templates are consumed from the `enigmatry-azure-pipelines-templates` repository via a `resources.repositories` reference — do not inline template logic that already exists there.

## Key variables

```yaml
variables:
  artifactName: enigmatry-entry-building-blocks
  buildConfiguration: Release
  projectNamePrefix: Enigmatry.Entry
  NUGET_PACKAGES: $(Pipeline.Workspace)/.nuget/packages
```

## Main pipeline stages

### 1. `ci_build` — build, test, pack

```yaml
- stage: ci_build
  displayName: Build the packages
  jobs:
  - job: build_prerequisites        # installs minver-cli, sets build number from git tag
  - job: Build_Package              # restore → build → test (unit+smoke) → pack → publish artifact
```

The test step filters to fast categories only:

```yaml
- task: DotNetCoreCLI@2
  displayName: Run Unit Tests
  inputs:
    command: test
    projects: $(projectNamePrefix)**/*.Tests.csproj
    arguments: '--filter TestCategory=unit|TestCategory=smoke --configuration $(buildConfiguration)'
    nobuild: true
```

### 2. `publish_nuget` — publish to NuGet.org

Runs only on `master` or `release/*` branches, after `ci_build` succeeds:

```yaml
- stage: publish_nuget
  dependsOn: ci_build
  condition: and(succeeded(), or(eq(variables['Build.SourceBranch'], 'refs/heads/master'), startsWith(variables['Build.SourceBranch'], 'refs/heads/release/')))
  jobs:
  - deployment: Publish
    environment: Publish to NuGet
    strategy:
      runOnce:
        deploy:
          steps:
          - task: NuGetCommand@2
            inputs:
              command: push
              packagesToPush: '$(System.ArtifactsDirectory)/**/*.nupkg;!$(System.ArtifactsDirectory)/**/*.symbols.nupkg'
              nuGetFeedType: external
              publishFeedCredentials: nuget_org
```

## Code analysis pipeline

Runs on a daily schedule (`0 0 * * *`) against `master`. Uses shared templates:

```yaml
jobs:
- template: code-analysis.yml@templates
  parameters:
    projectName: 'Enigmatry - Entry Building Blocks'
    sonarProjectKey: 'AspNetCoreAngular_EnigmatryBlueprintBuildingBlocks'
    dotNetVersion: '10.0.x'

- template: build-report-job.yml@templates
  parameters:
    artifactName: $(artifactName)
    projectNamePrefix: $(projectNamePrefix)
    projectNameAngularApp: ''      # empty — this project has no Angular frontend
```

## Versioning

Version numbers come from `minver-cli` (reads the nearest git tag). The `build_prerequisites` job installs it as a local tool and sets `BUILD_BUILDNUMBER`:

```powershell
$version = dotnet minver -p preview
echo "##vso[build.updatebuildnumber]$version"
```

Pack tasks then consume `$(BUILD_BUILDNUMBER)` via `versioningScheme: byEnvVar`.

## NuGet caching

Always include the cache task before restore:

```yaml
- task: Cache@2
  inputs:
    key: 'nuget | "$(Agent.OS)" | **/packages.lock.json,!**/bin/**,!**/obj/**'
    restoreKeys: |
      nuget | "$(Agent.OS)"
      nuget
    path: $(NUGET_PACKAGES)
    cacheHitVar: NUGET_CACHE_RESTORED

- task: NuGetCommand@2
  condition: ne(variables.NUGET_CACHE_RESTORED, true)
  inputs:
    command: restore
    restoreSolution: '$(projectNamePrefix).sln'
```

## Rules

- Never hardcode secrets or NuGet API keys — use the `nuget_org` service connection.
- Do not add deployment stages — this project ships NuGet packages, not deployed services.
- Use `trigger: '*'` on `azure-pipelines.yml` (run on every push); `trigger: none` on `code-analysis.yml` (schedule-only).
- When adding a new test project, no pipeline changes are needed — the glob `$(projectNamePrefix)**/*.Tests.csproj` picks it up automatically.
- Lock files are enforced (`RestorePackagesWithLockFile=true`). After changing `Directory.Packages.props`, regenerate them with `dotnet restore --force-evaluate` before pushing.
