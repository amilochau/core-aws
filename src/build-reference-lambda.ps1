$dockerfilePath = 'Dockerfile-reference-lambda'
$solutionPath = './Milochau.Core.Aws.ReferenceProjects.LambdaFunction.sln'
$bootstrapPath = "./Reference Projects/Milochau.Core.Aws.ReferenceProjects.LambdaFunction/bin/Release/net8.0/linux-x64/publish/bootstrap"

$sw = [Diagnostics.Stopwatch]::StartNew()
$dir = (Get-Location).Path
$imageTag = "reference-temp"

Write-Output "Build Docker image, used to build functions"
docker build --pull --rm -f "$dockerfilePath" "$dir" -t $imageTag

docker run --rm -v "$($dir):/src" -w /src $imageTag dotnet publish "$solutionPath" -c Release -r linux-x64 --sc true -p:BuildSource=AwsCmd -f net8.0

Write-Output "Creating compressed file"

if (Test-Path "./output") {
  Remove-Item -LiteralPath "./output" -Force -Recurse
}
if (Test-Path "./output-compressed") {
  Remove-Item -LiteralPath "./output-compressed" -Force -Recurse
}
New-Item -Path "./output" -ItemType Directory | Out-Null
New-Item -Path "./output-compressed" -ItemType Directory | Out-Null

$directoryDestinationPath = "$PWD/output"
Copy-Item -Path $bootstrapPath -Destination $directoryDestinationPath

$directoryDestinationPathCompressed = "$PWD/output-compressed"
$compressedFilePath = Join-Path $directoryDestinationPathCompressed "bootstrap.zip"
[System.IO.Compression.ZipFile]::CreateFromDirectory($directoryDestinationPath, $compressedFilePath)

$zipSize = [Math]::Round((Get-ChildItem -Path ./output-compressed/bootstrap.zip).Length / 1024 / 1024, 2)

Write-Output "Zipped bundle size: $zipSize MB"
