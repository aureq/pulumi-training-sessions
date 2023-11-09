using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using System;
using System.Net;

// TODO 2: Ensure the component resource is easily portable (ie, input params and no hardcoded values)
// TODO 2: Use existing Resource Group instead of creating one inside the Component
public class NetworkComponentArgs : Pulumi.ResourceArgs
{
    public ResourceGroup? ResourceGroup { get; set; }

    public string? CidrBlock { get; set; }

    public string? SubnetMask { get; set; }

}

class NetworkComponent : ComponentResource
{
    private string Name { get; set; }
    private NetworkComponentArgs Args { get; set; }

    public VirtualNetwork VirtualNetwork { get; private set; }

    public Subnet Subnet { get; private set; }

    public NetworkSecurityGroup NetworkSecurityGroup { get; private set; }

    public NetworkComponent(string name, NetworkComponentArgs args, ComponentResourceOptions? opts = null) : base("pkg:index:NetworkComponent", name, opts)
    {
        this.Name = name;
        this.Args = args;

        if (this.Args.ResourceGroup == null)
        {
            throw new ArgumentNullException(nameof(this.Args.ResourceGroup), "Value must be provided.");
        }

        if (this.Args.CidrBlock == null || this.Args.CidrBlock == "")
        {
            throw new ArgumentNullException(nameof(this.Args.CidrBlock), "Value must be provided.");
        }

        if (this.Args.SubnetMask == null || this.Args.SubnetMask == "")
        {
            throw new ArgumentNullException(nameof(this.Args.SubnetMask), "Value must be provided.");
        }

        IPNetwork ipNetwork = IPNetwork.Parse(this.Args.CidrBlock);

        this.VirtualNetwork = new VirtualNetwork("network", new VirtualNetworkArgs
        {
            ResourceGroupName = this.Args.ResourceGroup.Name,
            Location = this.Args.ResourceGroup.Location,
            AddressSpace = new AddressSpaceArgs
            {
                AddressPrefixes = { ipNetwork.ToString() },
            },
        }, new CustomResourceOptions
        {
            Parent = this,
        });

        var subnetBytes = IPNetwork.ToCidr(IPAddress.Parse(this.Args.SubnetMask));
        IPNetworkCollection subnets = ipNetwork.Subnet(subnetBytes);
        this.Subnet = new Subnet("subnet", new Pulumi.AzureNative.Network.SubnetArgs
        {
            ResourceGroupName = this.Args.ResourceGroup.Name,
            VirtualNetworkName = this.VirtualNetwork.Name,
            AddressPrefix = subnets[0].ToString(),
        }, new CustomResourceOptions
        {
            Parent = this.VirtualNetwork
        });

        this.NetworkSecurityGroup = new NetworkSecurityGroup("network-sg", new Pulumi.AzureNative.Network.NetworkSecurityGroupArgs
        {
            ResourceGroupName = this.Args.ResourceGroup.Name,
        }, new CustomResourceOptions
        {
            Parent = this
        });

        this.RegisterOutputs();
    }
}