# Install.ps1
param($installPath, $toolsPath, $package, $project)

function Get-CookieSecret() {
    [int] $len = 20
    [string] $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_!@#$%^&*()_"
    $bytes = new-object "System.Byte[]" $len
    $rnd = new-object System.Security.Cryptography.RNGCryptoServiceProvider
    $rnd.GetBytes($bytes)
    $result = ""
    for( $i=0; $i -lt $len; $i++ ) {
        $result += $chars[ $bytes[$i] % $chars.Length ]	
    }
    $result
}

function Add-AppSettings() {

    $xml = New-Object xml

    # find the Web.config file
    $config = $project.ProjectItems | where {$_.Name -eq "Web.config"}

    if (!$config) {
        Write-Warning "Skipping '$($project.Name)', can't find Web.config. This package can only be used with Web Application/Web Site"
        return;
    }
    # find its path on the file system
    $localPath = $config.Properties | where {$_.Name -eq "LocalPath"}

    # load Web.config as XML
    $xml.Load($localPath.Value)

    $existing = $xml.SelectSingleNode("configuration/appSettings/add[@key = 'DiagDash.CookieSecret']")
    if ($existing) {
        # already have installed package, doing update. so keep value.
        return;
    }

    # select the node
    $node = $xml.SelectSingleNode("configuration/appSettings")
    
	if (!$node)	{
		Write-Warning "Skipping '$($project.Name)', can't find appSettings tag."
        return;
	}
	$xmlElt = $xml.CreateElement("add")
	$xmlAtt = $xml.CreateAttribute("key")
	$xmlAtt.Value = "DiagDash.CookieSecret"
	$xmlElt.Attributes.Append($xmlAtt)
	$xmlAtt2 = $xml.CreateAttribute("value")
	$xmlAtt2.Value = Get-CookieSecret
	$xmlElt.Attributes.Append($xmlAtt2)

	$node.AppendChild($xmlElt)

	# save the Web.config file
	$xml.Save($localPath.Value)
}

try {
    Add-AppSettings
    #Get-CookieSecret
} catch {
    Write_Warning $_
}