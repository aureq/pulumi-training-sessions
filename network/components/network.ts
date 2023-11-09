import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import { Netmask } from "netmask";

interface VirtualNetworkArgs {
    resourceGroup: azure.core.ResourceGroup,
    cidrBlock: pulumi.Input<string>,
    subnetMask: pulumi.Input<string>,
};

export class VirtualNetwork extends pulumi.ComponentResource {
    private readonly name: string;
    private readonly args: VirtualNetworkArgs;

    public readonly virtualNetwork: azure.network.VirtualNetwork;
    public readonly subnet: azure.network.Subnet;

    constructor(name: string, args: VirtualNetworkArgs, opts?: pulumi.ComponentResourceOptions) {
        super("custom:resource:VirtualNetwork", name, {}, opts);
        this.name = name;
        this.args = args;

        const virtualNetworkCidr = new Netmask(this.args.cidrBlock.toString());
        const subnetCidr = new Netmask(`${virtualNetworkCidr.base}/${this.args.subnetMask}`);

        this.virtualNetwork = new azure.network.VirtualNetwork("network", {
            resourceGroupName: this.args.resourceGroup.name,
            location: this.args.resourceGroup.location,
            addressSpaces: [this.args.cidrBlock],
        }, { parent: this });

        this.subnet = new azure.network.Subnet("subnet", {
            resourceGroupName: this.args.resourceGroup.name,
            virtualNetworkName: this.virtualNetwork.name,
            addressPrefixes: [`${subnetCidr.base}/${subnetCidr.bitmask}`],
        }, { parent: this.virtualNetwork });
    }
}