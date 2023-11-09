import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as webserver from "./components/webserver";

const config = new pulumi.Config();

export = async () => {

    const username = config.require("username");

    const org = config.require("org");
    const stack = pulumi.getStack();
    const netStackRef = new pulumi.StackReference(`${org}/network/${stack}`);

    const resourceGroup = await azure.core.getResourceGroup({
        name: await netStackRef.requireOutputValue("resourceGroupName"),
    });

    const virtualNetwork = await azure.network.getVirtualNetwork({
        resourceGroupName: resourceGroup.name,
        name: await netStackRef.requireOutputValue("virtualNetworkName"),
    })

    const subnet = await azure.network.getSubnet({
        resourceGroupName: resourceGroup.name,
        virtualNetworkName: virtualNetwork.name,
        name: await netStackRef.requireOutputValue("subnetName"),
    });

    const ws = new webserver.WebServer("ws", {
        resourceGroup: resourceGroup,
        subnet: subnet,
        userName: username,
    });

    return {
        username: username,
        password: ws.password.result,
        ip: ws.publicIp.ipAddress,
    }
}
