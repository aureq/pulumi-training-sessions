import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";
import { Netmask } from "netmask";

const config = new pulumi.Config();

export = async () => {

    // Create an Azure Resource Group
    const resourceGroup = new azure.core.ResourceGroup("resourceGroup");

    const cidrBlock = /* */;
    const subnetMask = /* */;

    const virtualNetworkCidr = new Netmask(cidrBlock.toString());
    const subnetCidr = new Netmask(`${virtualNetworkCidr.base}/${subnetMask}`);

    const virtualNetwork = new azure.network.VirtualNetwork("network", {
        resourceGroupName: resourceGroup.name,
        location: resourceGroup.location,
        addressSpaces: [ /* */ ],
    });

    const subnet = new azure.network.Subnet("subnet", {
        resourceGroupName: resourceGroup.name,
        virtualNetworkName: /* */,
        addressPrefixes: /* */,
    });

    const pubIp = new azure.network.PublicIp("ip", {
        /* */
    });

    const nic = new azure.network.NetworkInterface("nic", {
        /* */
    });

    const sg = new azure.network.NetworkSecurityGroup("sg", {
        /* */
    });

    const sgrule = new azure.network.NetworkSecurityRule("sgrule", {
        /* */
    });

    const sgnicassoc = new azure.network.NetworkInterfaceSecurityGroupAssociation("sg-nic-assoc", {
        /* */
    });


    const password = new random.RandomPassword('password', {
        /* */
    })./* */;

    const vm = new azure.compute.VirtualMachine("vm", {
        /* */
        storageImageReference: {
            publisher: "Canonical",
            offer: "0001-com-ubuntu-server-focal",
            sku: "20_04-lts",
            version: "latest",
        },
    });

    return {
        /* */
    }

}
