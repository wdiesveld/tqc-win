$uri = "http://compile.tinyqueries.com/"
$pwd = ConvertTo-SecureString 'MyPassword' -AsPlainText -Force
$cred = New-Object Management.Automation.PSCredential ('myuser', $pwd)
$contentType = "multipart/form-data"
$body = @{
    "FileName" = Get-Content($uploadPath) -Raw
}
Invoke-WebRequest -Uri $uri -Method Post -ContentType $contentType -Body $body
