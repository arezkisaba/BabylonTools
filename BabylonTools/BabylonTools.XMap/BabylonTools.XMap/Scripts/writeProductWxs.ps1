param(
	[string] $path
)

$items = Get-ChildItem $path
$xml = ""

foreach ($item in $items) {
	if (!$item.PSIsContainer) {
		$relativePath = $item.FullName.Replace("$path", "`$`(var.BabylonTools.XMap.TargetDir`)")
		#$xml += "<File Source=`"$relativePath`"/>`n"
		$guid = (New-Guid).Guid;
		$otherGuid = $guid.Replace("-", "")
		Write-Host "<Component Id=`"$($item.Name).$otherGuid`" Guid=`"$guid`"><File Id=`"$($item.Name).$otherGuid`" Name=`"$($item.Name)`" Source=`"$relativePath`"/></Component>"
	}
	
	#Write-Host $xml
}
