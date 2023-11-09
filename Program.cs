using Pulumi;
using Pulumi.AzureNative.Compute;
using Pulumi.AzureNative.Compute.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.Random;
using System;
using System.Collections.Generic;
using System.Net;

return await Pulumi.Deployment.RunAsync(() =>
{
    var config = new Pulumi.Config();
    var cidrBlock = config.Require("cidrBlock");
    var subnetMask = config.Require("netmask");

    var username = config.Require("username");
    // TODO 3: Generate a random password
    var password = new RandomPassword                                       // FIXME

    var resourceGroup = new ResourceGroup("resourceGroup");

    // TODO 5: 3rd pary module to compute subnets CIDR
    IPNetwork ipnetwork = IPNetwork.Parse(/* FIXME */);                     // FIXME

    // TODO 2: Virtual Network
    var network = new VirtualNetwork("network", new VirtualNetworkArgs      // FIXME
    {
        /* FIXME */
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        /* FIXME */
    });

    // TODO 2: public Subnet (/20)
    // TODO 5: 3rd pary module to compute subnets CIDR
    var subnetBytes = IPNetwork.ToCidr( /* FIXME */);                       // FIXME
    IPNetworkCollection subnets = ipnetwork.Subnet(subnetBytes);
    var subnet = new Subnet("subnet",                                       // FIXME
        /* FIXME */
    );

    // TODO 2: public IP address
    var publicIp = new PublicIPAddress("public-ip",                         // FIXME
    {
        /* FIXME */
    });

    // TODO 2: network security group
    var networkSG = new NetworkSecurityGroup("network-sg",                  // FIXME
    {
        /* FIXME */
    });

    // TODO 2: network security rule
    var sgRule = new SecurityRule("rule",                                   // FIXME
    {
        /* FIXME */
    });

    // TODO 2: network interface
    var networkInterface = new NetworkInterface("vm-nic",                   // FIXME
    {
        /* FIXME */
    });

    // TODO 3: Create a virtual machine
    var virtualMachine = new VirtualMachine("vm",                           // FIXME
    {
        /* FIXME */
    });

    // TODO 4: Create stack outputs
    return new Dictionary<string, object?>                                  // FIXME
    {
        /* FIXME */
    };
});