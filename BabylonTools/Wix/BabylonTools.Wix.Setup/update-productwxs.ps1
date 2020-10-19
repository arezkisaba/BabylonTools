$basePath = $PsScriptRoot
$artifactFolderPath = "$basePath\Artifacts"
$wxsTemplatePath = "$basePath\ProductTemplate.wxs"
$wxsPath = "$basePath\Product.wxs"
$items = Get-ChildItem $artifactFolderPath
$xml = ""

foreach ($item in $items) {
	if (!$item.PSIsContainer) {
		$relativePath = "$artifactFolderPath\$($item.Name)"
		$guid = (New-Guid).Guid;
		$otherGuid = $guid.Replace("-", "")
		$row = "`t`t`t<Component Id=`"$($item.Name).$otherGuid`" Guid=`"$guid`">`r`n`t`t`t`t<File Id=`"$($item.Name).$otherGuid`" Name=`"$($item.Name)`" Source=`"$relativePath`"/>`r`n`t`t`t</Component>`r`n"
		$xml += "$row`n"
	}
}

$content = (Get-Content $wxsTemplatePath)
$content = $content -replace "##TOREPLACE##", $xml
Set-Content $wxsPath $content
