Param(
    [parameter(Mandatory=$true)][string]$tag
)


docker build --no-cache  -f  .\Dyw.Ordering.Api\Dockerfile -t dyw-ordering-api:$tag .
docker build --no-cache  -f  .\Dyw.Mobile.Gateway\Dockerfile -t dyw-gateway:$tag .
docker build --no-cache  -f  .\Dyw.Mobile.ApiAggregator\Dockerfile -t dyw-aggretator-api:$tag .

"Any key to exit"  ;
Read-Host | Out-Null ;
Exit