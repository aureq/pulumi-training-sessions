using System;
using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.AzureNative.Compute;
using Pulumi.AzureNative.Compute.Inputs;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.Azure.Network;

public class WebServerComponentArgs
{
    public Output<GetResourceGroupResult>? ResourceGroup { get; set; }
    public Output<Pulumi.Azure.Network.GetSubnetResult>? Subnet { get; set; }
    public Output<Pulumi.Azure.Network.GetNetworkSecurityGroupResult>? NetworkSecurityGroup { get; set; }
    public string AdminUserName { get; set; } = "AcmeCorpAdmin";
    public Input<string>? AdminPassword { get; set; }
}

class WebServerComponent : ComponentResource
{
    private string Name { get; set; }
    private WebServerComponentArgs Args { get; set; }
    public PublicIPAddress PublicIPAddress { get; private set; }
    public SecurityRule SecurityRule { get; private set; }
    public Pulumi.AzureNative.Network.NetworkInterface NetworkInterface { get; private set; }

    public WebServerComponent(string name, WebServerComponentArgs args, ComponentResourceOptions? opts = null) : base("pkg:index:WebServerComponent", name, opts)
    {
        this.Name = name;
        this.Args = args;

        if (this.Args.ResourceGroup == null)
        {
            throw new ArgumentNullException(nameof(this.Args.ResourceGroup), "Value must be provided");
        }

        if (this.Args.Subnet == null)
        {
            throw new ArgumentNullException(nameof(this.Args.Subnet), "Value must be provided");
        }

        if (this.Args.NetworkSecurityGroup == null)
        {
            throw new ArgumentNullException(nameof(this.Args.NetworkSecurityGroup), "Value must be provided");
        }

        this.PublicIPAddress = new PublicIPAddress("public-ip", new Pulumi.AzureNative.Network.PublicIPAddressArgs
        {
            Location = this.Args.ResourceGroup.Apply(r => r.Location),
            ResourceGroupName = this.Args.ResourceGroup.Apply(r => r.Name),
            PublicIPAllocationMethod = IPAllocationMethod.Static,
            Sku = new Pulumi.AzureNative.Network.Inputs.PublicIPAddressSkuArgs
            {
                Tier = Pulumi.AzureNative.Network.PublicIPAddressSkuTier.Regional,
                Name = "Standard"
            },
        }, new CustomResourceOptions
        {
            Parent = this,
        });

        this.SecurityRule = new SecurityRule("rule", new Pulumi.AzureNative.Network.SecurityRuleArgs
        {
            ResourceGroupName = this.Args.ResourceGroup.Apply(r => r.Name),
            NetworkSecurityGroupName = this.Args.NetworkSecurityGroup.Apply(r => r.Name),
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
            Parent = this
        });

        this.NetworkInterface = new Pulumi.AzureNative.Network.NetworkInterface("vm-nic", new Pulumi.AzureNative.Network.NetworkInterfaceArgs
        {
            ResourceGroupName = this.Args.ResourceGroup.Apply(r => r.Name),
            IpConfigurations =
            {
                new NetworkInterfaceIPConfigurationArgs
                {
                    Name = "vmserveripcfg",
                    Subnet = new Pulumi.AzureNative.Network.Inputs.SubnetArgs
                    {
                        Id = this.Args.Subnet.Apply(r => r.Id),
                    },
                    PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
                    PublicIPAddress = new Pulumi.AzureNative.Network.Inputs.PublicIPAddressArgs
                    {
                        Id = this.PublicIPAddress.Id,
                    }
                }
            },
            NetworkSecurityGroup = new Pulumi.AzureNative.Network.Inputs.NetworkSecurityGroupArgs
            {
                Id = this.Args.NetworkSecurityGroup.Apply(r => r.Id),
            },
        }, new CustomResourceOptions
        {
            Parent = this
        });

        var virtualMachine = new VirtualMachine("vm", new VirtualMachineArgs
        {
            ResourceGroupName = this.Args.ResourceGroup.Apply(r => r.Name),
            NetworkProfile = new Pulumi.AzureNative.Compute.Inputs.NetworkProfileArgs
            {
                NetworkInterfaces =
                {
                    new NetworkInterfaceReferenceArgs
                    {
                        Id = this.NetworkInterface.Id,
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
                    Publisher = "Canonical",
                    Offer = "0001-com-ubuntu-server-focal",
                    Sku = "20_04-lts",
                    Version = "latest",
                }
            },
            OsProfile = new OSProfileArgs
            {
                ComputerName = "hostname",
                AdminPassword = this.Args.AdminPassword,
                AdminUsername = this.Args.AdminUserName,
            },
        }, new CustomResourceOptions
        {
            Parent = this.NetworkInterface,
        });

        this.RegisterOutputs();
    }
}