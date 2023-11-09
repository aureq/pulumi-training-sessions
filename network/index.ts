import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as network from "./components/network"

const config = new pulumi.Config();

export = async () => {

    const resourceGroup = new azure.core.ResourceGroup("resourceGroup");
    const cidrBlock = config.require("cidrBlock");
    const subnetMask = config.require("netmask");

    const vnet = new network.VirtualNetwork('network-cr', {
        resourceGroup: resourceGroup,
        cidrBlock: cidrBlock,
        subnetMask: subnetMask,
    });

    return {
        resourceGroupName: resourceGroup.name,
        virtualNetworkName: vnet.virtualNetwork.name,
        subnetName: vnet.subnet.name,
    }
}
