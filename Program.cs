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
    var password = new RandomPassword("password", new RandomPasswordArgs
    {
        Length = 33,
        Special = false,
    });

    var resourceGroup = new ResourceGroup("resourceGroup");

    // TODO 5: 3rd pary module to compute subnets CIDR
    IPNetwork ipnetwork = IPNetwork.Parse(cidrBlock);

    // TODO 2: Virtual Network
    var network = new VirtualNetwork("network", new VirtualNetworkArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        AddressSpace = new AddressSpaceArgs
        {
            AddressPrefixes = { ipnetwork.ToString() },
        },
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = resourceGroup
    });

    // TODO 2: public Subnet (/20)
    // TODO 5: 3rd pary module to compute subnets CIDR
    var subnetBytes = IPNetwork.ToCidr(IPAddress.Parse(subnetMask));
    IPNetworkCollection subnets = ipnetwork.Subnet(subnetBytes);
    var subnet = new Subnet("subnet", new Pulumi.AzureNative.Network.SubnetArgs
    {
        ResourceGroupName = resourceGroup.Name,
        VirtualNetworkName = network.Name,
        AddressPrefix = subnets[0].ToString(),
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = network
    });

    // TODO 2: public IP address
    var publicIp = new PublicIPAddress("public-ip", new Pulumi.AzureNative.Network.PublicIPAddressArgs
    {
        Location = resourceGroup.Location,
        ResourceGroupName = resourceGroup.Name,
        PublicIPAllocationMethod = IPAllocationMethod.Static,
        Sku = new Pulumi.AzureNative.Network.Inputs.PublicIPAddressSkuArgs
        {
            Tier = Pulumi.AzureNative.Network.PublicIPAddressSkuTier.Regional,
            Name = "Standard"
        },
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = network
    });

    // TODO 2: network security group
    var networkSG = new NetworkSecurityGroup("network-sg", new Pulumi.AzureNative.Network.NetworkSecurityGroupArgs
    {
        ResourceGroupName = resourceGroup.Name,
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = resourceGroup
    });

    // TODO 2: network security rule
    var sgRule = new SecurityRule("rule", new Pulumi.AzureNative.Network.SecurityRuleArgs
    {
        ResourceGroupName = resourceGroup.Name,
        NetworkSecurityGroupName = networkSG.Name,
        Access = "Allow",
        Direction = "Inbound",
        Priority = 100,
        Protocol = "Tcp",
        DestinationPortRange = "22",
        SourcePortRange = "*",
        SourceAddressPrefix = "*",
        DestinationAddressPrefix = "*"
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = networkSG
    });

    // TODO 2: network interface
    var networkInterface = new NetworkInterface("vm-nic", new NetworkInterfaceArgs
    {
        ResourceGroupName = resourceGroup.Name,
        IpConfigurations =
        {
            new NetworkInterfaceIPConfigurationArgs
            {
                Name = "vmserveripcfg",
                Subnet = new Pulumi.AzureNative.Network.Inputs.SubnetArgs
                {
                    Id = subnet.Id
                },
                PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
                PublicIPAddress = new Pulumi.AzureNative.Network.Inputs.PublicIPAddressArgs
                {
                    Id = publicIp.Id,
                }
            }
        },
        NetworkSecurityGroup = new Pulumi.AzureNative.Network.Inputs.NetworkSecurityGroupArgs
        {
            Id = networkSG.Id,
        },
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = subnet
    });

    // TODO 3: Create a virtual machine
    var virtualMachine = new VirtualMachine("vm", new VirtualMachineArgs
    {
        ResourceGroupName = resourceGroup.Name,
        NetworkProfile = new Pulumi.AzureNative.Compute.Inputs.NetworkProfileArgs
        {
            NetworkInterfaces =
            {
                new NetworkInterfaceReferenceArgs
                {
                    Id = networkInterface.Id,
                    Primary = true,
                }
            }
        },
        HardwareProfile = new HardwareProfileArgs
        {
            VmSize = VirtualMachineSizeTypes.Standard_A0,
        },
        StorageProfile = new StorageProfileArgs
        {
            OsDisk = new OSDiskArgs
            {
                CreateOption = DiskCreateOptionTypes.FromImage,
                Name = "osdisk",
            },
            ImageReference = new ImageReferenceArgs
            {
                // TODO 3: Using the most recent Ubuntu 20.04 LTS
                Publisher = "Canonical",
                Offer = "0001-com-ubuntu-server-focal",
                Sku = "20_04-lts",
                Version = "latest",
            }
        },
        OsProfile = new OSProfileArgs
        {
            ComputerName = "hostname",
            // TODO 3: Use random password
            AdminPassword = password.Result,
            AdminUsername = username,
        },
    }, new CustomResourceOptions
    {
        // TODO 2: Nest resource under correct parent
        Parent = networkInterface
    });

    // TODO 4: Create stack outputs
    return new Dictionary<string, object?>
    {
        ["username"] = username,
        ["password"] = password.Result,
        ["ip"] = publicIp.IpAddress,
    };
});