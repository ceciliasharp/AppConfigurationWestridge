$location = "swedencentral"
$random = Get-Random -Maximum 999999 -minimum 100000
$resourceGroupName = "rg-app-appconfigdemo"
$appServicePlanName = "asp-appconfigdemo-$random"
$webAppName = "app-appconfigdemo-$random"
$keyVaultName = "kvappconfigdemo$random"
$appConfigName = "app-appconfigdemo-$random"

az group create --name $resourceGroupName --location $location

az appservice plan create --name $appServicePlanName --resource-group $resourceGroupName
az webapp create --name $webAppName --resource-group $resourceGroupName --plan $appServicePlanName
$webappidentity =  az webapp identity assign --name $webAppName --resource-group $resourceGroupName | ConvertFrom-Json | Select-Object -ExpandProperty principalId
az webapp config appsettings set --name $webAppName --resource-group $resourceGroupName --settings "env=qa"

az keyvault create --name $keyVaultName --resource-group $resourceGroupName --location $location
az keyvault set-policy --name $keyVaultName --object-id $webappidentity --secret-permissions get
$kvSecret = az keyvault secret set --vault-name $keyVaultName --name "TheSecretKey" --value "RyMwniQ.!6pbjCowLxcbgJ_BVCpRyXhEwic2nUW" | ConvertFrom-Json | Select-Object -ExpandProperty id

$appconfig = az appconfig create --name $appConfigName --resource-group $resourceGroupName --location $location 
$appconfigId = $appconfig | ConvertFrom-Json | Select-Object -ExpandProperty id
az role assignment create --role "App Configuration Data Reader" --scope $appconfigId --assignee-object-id $webappidentity --assignee-principal-type ServicePrincipal
az appconfig kv set --name $appConfigName --key "Demo:title" --value "QA Azure" --label "qa" --yes
az appconfig kv set --name $appConfigName --key "Demo:title" --value "DEV Azure" --label "dev" --yes
az appconfig kv set --name $appConfigName --key "Demo:title" --value "PROD Azure" --label "prod" --yes
az appconfig kv set --name $appConfigName --key "Demo:text" --value "Text for QA env" --label "qa" --yes
az appconfig kv set --name $appConfigName --key "Demo:text" --value "Text for DEV env" --label "dev" --yes
az appconfig kv set --name $appConfigName --key "Demo:text" --value "Text for PROD" --label "prod" --yes
az appconfig kv set-keyvault --name $appConfigName --key "secret" --secret-identifier $kvSecret --yes
az appconfig feature set --name $appConfigName --feature showsecret --yes

#appconfig endpoint, update appsettings.json file with the endpoint
Write-Host ($appconfig | ConvertFrom-Json).endpoint

#run this to remove everything again
#az group delete --name $resourceGroupName --yes
