# DiscountService

### Open powershell to Test SignalR
-- Generate
```
$client = New-Object System.Net.Sockets.TcpClient("localhost",5000)
$stream = $client.GetStream()
$writer = New-Object System.IO.StreamWriter($stream)
$writer.AutoFlush = $true
$reader = New-Object System.IO.StreamReader($stream)
$writer.Write('{"Type":"Generate","Count":50,"Length":8}')
$response = $reader.ReadToEnd()
$response
```
-- UseCode
```
$client = New-Object System.Net.Sockets.TcpClient("localhost",5000)
$stream = $client.GetStream()
$writer = New-Object System.IO.StreamWriter($stream)
$writer.AutoFlush = $true
$reader = New-Object System.IO.StreamReader($stream)
$writer.Write('{"Type":"UseCode","Code": "8N87584R"}')
$response = $reader.ReadToEnd()
$response
```

- Install grpcurl using powershell
```
$grpcurlVersion = "1.9.1"
$grpcurlUrl = "https://github.com/fullstorydev/grpcurl/releases/download/v$grpcurlVersion/grpcurl_${grpcurlVersion}_windows_x86_64.zip"
$zipPath = "$env:TEMP\grpcurl.zip"
$destPath = "$env:ProgramFiles\grpcurl"
# Download
Invoke-WebRequest -Uri $grpcurlUrl -OutFile $zipPath
# Create folder
New-Item -ItemType Directory -Force -Path $destPath | Out-Null
# Extract
Expand-Archive -Path $zipPath -DestinationPath $destPath -Force
# Add to PATH for current session
$env:Path += ";$destPath"
Write-Host "grpcurl installed to $destPath. Run 'grpcurl --version' to check."
```

### GRPC Test

open cmp "C:\Program Files\grpcurl" to test grpc
```
grpcurl -insecure localhost:7183 list
grpcurl -insecure -d "{\"count\": 5, \"length\": 8 }" localhost:7183 discountCode.DiscountService/Generate
grpcurl -insecure -d "{\"code\":\"LB8A5N59\"}" localhost:7183 discountCode.DiscountService/UseCode
```


