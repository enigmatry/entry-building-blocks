# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = 1)][scriptblock]$cmd,
        [Parameter(Position = 1, Mandatory = 0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0)
    {
        throw ("Exec: " + $errorMessage)
    }
}

$artifacts = ".\artifacts"

if (Test-Path $artifacts)
{
    Remove-Item $artifacts -Force -Recurse
}

# if minver is missing install it using: dotnet tool install --global minver-cli
$version = exec { & minver }

exec { & dotnet clean -c Release }

exec { & dotnet build -c Release -p:Version=$version }

exec { & dotnet test -c Release --no-build -l trx --verbosity=normal --filter "Category=unit|Category=smoke" }

$projects = Get-ChildItem -Recurse -Filter *.csproj

foreach ($project in $projects)
{
    $content = Get-Content $project.FullName
    if ($content -match '<IsPackable>true</IsPackable>')
    {
        exec { & dotnet pack $project.FullName -c Release -o $artifacts --no-build -p:Version=$version }
    }
}



