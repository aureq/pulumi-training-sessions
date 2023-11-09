using Pulumi;
using Pulumi.AzureNative.Resources;
using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();
    var cidrBlock = config.Require("cidrBlock");
    var subnetMask = config.Require("netmask");

    // TODO 2: Leave the resource group outside of the NetworkComponent
    var resourceGroup = new ResourceGroup("resourceGroup");

    // TODO 2: Deploy a fully functional virtual network as a component resource
    var networkComponent = new NetworkComponent("NetworkComponent", new NetworkComponentArgs
    {
        ResourceGroup = resourceGroup,
        SubnetMask = subnetMask,
        CidrBlock = cidrBlock,
    }, new ComponentResourceOptions
    {
        Parent = resourceGroup,
    });

    // TODO 3: Determine the necessary stack outputs for the `webserver` project to use
    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["virtualNetworkName"] = networkComponent.VirtualNetwork.Name,
        ["subnetName"] = networkComponent.Subnet.Name,
        ["networkSecurityGroupName"] = networkComponent.NetworkSecurityGroup.Name,
    };
});