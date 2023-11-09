using Pulumi;
using Pulumi.Random;
using Pulumi.Azure.Core;
using System.Collections.Generic;
using System;
using Pulumi.Azure.Network;

return await Pulumi.Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();
    var username = config.Require("username");
    var password = new RandomPassword("password", new RandomPasswordArgs
    {
        Length = 33,
        Special = false,
    });

    // TODO 5: Use stack references to retrieve the necessary dependencies
    // TODO 9: Use Pulumi runtime functions to determine the current stack name
    var referenceStackName = $"{Pulumi.Deployment.Instance.OrganizationName}/network/{Pulumi.Deployment.Instance.StackName}";
    var stackRef = new Pulumi.StackReference(referenceStackName);

    var resourceGroupName = stackRef.RequireOutput("resourceGroupName").Apply(v => v.ToString() ?? throw new ArgumentNullException(nameof(v), "Unable to convert 'resourceGroupName' stack ref into a string"));
    // TODO 6: Use the `Get*()` functions to retrieve existing resources
    var resourceGroup = GetResourceGroup.Invoke(new GetResourceGroupInvokeArgs
    {
        Name = resourceGroupName,
    });

    var virtualNetworkName = stackRef.RequireOutput("virtualNetworkName").Apply(v => v.ToString() ?? throw new ArgumentNullException(nameof(v), "Unable to convert 'virtualNetworkName' stack ref into a string"));
    // TODO 6: Use the `Get*()` functions to retrieve existing resources
    var virtualNetwork = GetVirtualNetwork.Invoke(new GetVirtualNetworkInvokeArgs
    {
        Name = virtualNetworkName,
        ResourceGroupName = resourceGroupName,
    });

    var subnetName = stackRef.RequireOutput("subnetName").Apply(v => v.ToString() ?? throw new ArgumentNullException(nameof(v), "Unable to convert 'subnetName' stack ref into a string"));
    // TODO 6: Use the `Get*()` functions to retrieve existing resources
    var subnet = GetSubnet.Invoke(new GetSubnetInvokeArgs
    {
        Name = subnetName,
        ResourceGroupName = resourceGroupName,
        VirtualNetworkName = virtualNetworkName,
    });

    var networtkSecurityGroupName = stackRef.RequireOutput("networkSecurityGroupName").Apply(v => v.ToString() ?? throw new ArgumentNullException(nameof(v), "Unable to convert 'networkSecurityGroupName' stack ref into a string"));
    // TODO 6: Use the `Get*()` functions to retrieve existing resources
    var networkSecurityGroup = GetNetworkSecurityGroup.Invoke(new GetNetworkSecurityGroupInvokeArgs
    {
        Name = networtkSecurityGroupName,
        ResourceGroupName = resourceGroupName,
    });

    // TODO 7: Deploy a VM you can SSH into in the previously created subnet
    var webServerComponent = new WebServerComponent("ws", new WebServerComponentArgs
    {
        AdminUserName = username,
        AdminPassword = password.Result,
        ResourceGroup = resourceGroup,
        NetworkSecurityGroup = networkSecurityGroup,
        Subnet = subnet,
    }, new ComponentResourceOptions
    {
        Parent = null
    });

    // TODO 8: Create stack outputs
    return new Dictionary<string, object?>
    {
        ["username"] = username,
        ["password"] = password.Result,
        ["ip"] = webServerComponent.PublicIPAddress.IpAddress,
    };
});