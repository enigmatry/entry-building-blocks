$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Write-Output $scriptPath
Set-Location $scriptPath
Set-Location ..\
Get-ChildItem .\ -include bin,obj -Recurse | Where-Object { $_ -notlike "*node_modules*" } | ForEach-Object ($_) {
   Write-Output $_.FullName
   Remove-Item $_.FullName -Force -Recurse 
}
Set-Location $scriptPath