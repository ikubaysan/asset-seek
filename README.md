# asset-seek
Search engine for Resonite assets



## Prerequisites
- Azure CLI installed
	* `winget install --exact --id Microsoft.AzureCLI`
	

- Make a copy of appsettings.json.sample and update ConnectionStrings
	
	



Need this so login goes to web browser. Otherwise I get error:

```
Retrieving tenants and subscriptions for the selection...
Authentication failed against tenant <guid> 'Default Directory': (pii). Status: Response_Status.Status_InteractionRequired, Error code: 3399614476, Tag: 557973645
If you need to access subscriptions in the following tenants, please use `az login --tenant TENANT_ID`.
```

`az config set core.enable_broker_on_windows=false`
`az login`