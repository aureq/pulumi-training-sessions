import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";
import { runInThisContext } from "vm";

interface WebServerArgs {
    resourceGroup: azure.core.ResourceGroup | azure.core.GetResourceGroupResult,
    subnet: azure.network.Subnet | azure.network.GetSubnetResult,
    userName: pulumi.Input<string>,
};

export class WebServer extends pulumi.ComponentResource {
    private readonly name: string;
    private readonly args: WebServerArgs;

    public readonly publicIp: azure.network.PublicIp;
    public readonly networkInterface: azure.network.NetworkInterface;
    public readonly networkSecurityGroup: azure.network.NetworkSecurityGroup;
    public readonly networkSecurityRule: azure.network.NetworkSecurityRule;
    public readonly password: random.RandomPassword;
    public readonly virtualMachine: azure.compute.VirtualMachine;


    constructor(name: string, args: WebServerArgs, opts?: pulumi.ComponentResourceOptions) {
        super("custom:resource:WebServer", name, {}, opts);
        this.name = name;
        this.args = args;

        this.publicIp = new azure.network.PublicIp("ip", {
            location: this.args.resourceGroup.location,
            resourceGroupName: this.args.resourceGroup.name,
            allocationMethod: "Static",
            sku: "Standard",
        }, { parent: this });

        this.networkInterface = new azure.network.NetworkInterface("nic", {
            resourceGroupName: this.args.resourceGroup.name,
            ipConfigurations: [{
                name: "webserveripcfg",
                subnetId: this.args.subnet.id,
                privateIpAddressAllocation: "Dynamic",
                publicIpAddressId: this.publicIp.id,
            }],
        }, { parent: this.publicIp });

        this.networkSecurityGroup = new azure.network.NetworkSecurityGroup("sg", {
            resourceGroupName: this.args.resourceGroup.name,
            location: this.args.resourceGroup.location,
        }, { parent: this });

        this.networkSecurityRule = new azure.network.NetworkSecurityRule("sgrule", {
            priority: 100,
            direction: "Inbound",
            access: "Allow",
            protocol: "Tcp",
            sourcePortRange: "*",
            destinationPortRange: "22",
            sourceAddressPrefix: "*",
            destinationAddressPrefix: "*",
            resourceGroupName: this.args.resourceGroup.name,
            networkSecurityGroupName: this.networkSecurityGroup.name,
        }, { parent: this });

        new azure.network.NetworkInterfaceSecurityGroupAssociation("sg-nic-assoc", {
            networkInterfaceId: this.networkInterface.id,
            networkSecurityGroupId: this.networkSecurityGroup.id,
        }, { parent: this });

        this.password = new random.RandomPassword('password', {
            length: 33,
            special: false,
        }, { parent: this });

        this.virtualMachine = new azure.compute.VirtualMachine("vm", {
            resourceGroupName: this.args.resourceGroup.name,
            networkInterfaceIds: [this.networkInterface.id],
            vmSize: "Standard_A0",
            deleteDataDisksOnTermination: true,
            deleteOsDiskOnTermination: true,
            osProfile: {
                computerName: "hostname",
                adminUsername: this.args.userName,
                adminPassword: this.password.result,
            },
            osProfileLinuxConfig: {
                disablePasswordAuthentication: false,
            },
            storageOsDisk: {
                createOption: "FromImage",
                name: "osdisk1",
            },
            storageImageReference: {
                publisher: "Canonical",
                offer: "0001-com-ubuntu-server-focal",
                sku: "20_04-lts",
                version: "latest",
            },
        }, { parent: this });
    }
}