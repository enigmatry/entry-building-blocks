---
name: azure-devops-pipelines
description: Best practices for Azure DevOps Pipeline YAML files in the Enigmatry Entry Blueprint project. Use this when creating, editing, or reviewing Azure DevOps CI/CD pipeline YAML.
---

# Blueprint Azure DevOps Pipelines

## Pipeline files

All pipeline YAML lives in `Pipelines/`:

| File | Purpose |
|------|---------|
| `azure-pipelines.yml` | Main CI/CD pipeline — build → deploy |
| `run-all-tests.yml` | Runs all test suites |
| `code-analysis.yml` | Static analysis |
| `build-publish-nuget.yml` | NuGet package publishing |
| `deploy-to-stage.yml` | Reusable deployment job template |
| `variables/` | Per-environment variable files |

Shared pipeline templates are consumed from the `enigmatry-azure-pipelines-templates` repository via a `resources.repositories` reference — do not inline template logic that already exists there.

## Key variables

```yaml
variables:
  artifactName: 'enigmatry-entry-blueprint-template'
  dbContextName: 'AppDbContext'
  nodeVersion: '22.17.1'         # keep in sync with .github/workflows/copilot-setup-steps.yml
  projectNameAngularApp: enigmatry-entry-blueprint-app
  projectNamePrefix: Enigmatry.Entry.Blueprint
  majorMinorVersion: 1.0
```

## Build stage

The build uses the shared `build-angular-app-and-dotnet-api.yml` template:

```yaml
- template: build-angular-app-and-dotnet-api.yml@templates
  parameters:
    artifactName: $(artifactName)
    nodeVersion: $(nodeVersion)
    projectNameAngularApp: $(projectNameAngularApp)
    projectNamePrefix: $(projectNamePrefix)
    runAngularTests: true
    useSlnx: true
    dbContextNames:
    - $(dbContextName)
```

## Deployment stage

Deployments use the `deploy-to-stage.yml` template and are gated on branch conditions:

```yaml
- stage: Deploy_Test
  dependsOn: ci_build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/master'))
  variables:
  - template: variables/variables.test.yml
  jobs:
  - template: deploy-to-stage.yml
    parameters:
      environment: test
      serviceConnection: 'Enigmatry - Entry Template (Test)'
```

## Rules

- Use `variables/variables.<env>.yml` files for environment-specific config — never inline environment values.
- Never hardcode secrets or connection strings in YAML — use variable groups or Azure Key Vault references.
- Keep `nodeVersion` in sync with `package.json` engines and `copilot-setup-steps.yml`.
- When adding a new `dbContext`, add it to the `dbContextNames` list in the build template call.
- Use `batch: true` on triggers to avoid redundant builds for rapid pushes.

