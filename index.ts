import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import { Netmask } from "netmask";
import * as tls from "@pulumi/tls";

const config = new pulumi.Config();

export = async () => {
    const baseName = "app";

    const ownerEmail = config.get("ownerEmail") || "unassigned@acmecorp.com";

    const cidrBlock = config.require("cidrBlock");
    const vpcCidr = new Netmask(cidrBlock.toString());
    const availabilityZone = config.require("availablityZone");

    const subnetMask = config.require("netmask");
    const subnetCidr = new Netmask(`${vpcCidr.base}/${subnetMask}`);

    // TODO: 2. Create VPC
    const vpc = new aws.ec2.Vpc( `${baseName}-vpc`, {
        cidrBlock: cidrBlock,
        instanceTenancy: "default",
        enableDnsHostnames: true,
        enableDnsSupport: true,
        tags: {
            owner: ownerEmail,
        }
    });

    // TODO: 2. Create Internet Gateway
    const igw = new aws.ec2.InternetGateway(`${baseName}-igw`, {
        vpcId: vpc.id,
        tags: {
            // TODO: 8. Resource tagged
            owner: ownerEmail,
        }
    },{
        // TODO: 2. Resource has a parent
        parent: vpc
    });

    // TODO: 2. Create route table
    const routeTable = new aws.ec2.RouteTable(`${baseName}-rt`, {
        vpcId: vpc.id,
        routes: [
            {
                cidrBlock: '0.0.0.0/0',
                gatewayId: igw.id
            }
        ],
        tags: {
            // TODO: 8. Resource tagged
            owner: ownerEmail,
        }
    }, {
        // TODO: 2. Resource has a parent
        parent: vpc
    });

    // TODO: 5. Use a 3rd pary module to compute subnets CIDR
    const subnetCidrBlock = `${subnetCidr.base}/${subnetCidr.bitmask}`;

    // TODO: 2. Create public subnet
    const pubSubnet = new aws.ec2.Subnet(`${baseName}-subnet`, {
        vpcId: vpc.id,
        cidrBlock: subnetCidrBlock,
        assignIpv6AddressOnCreation: false,
        mapPublicIpOnLaunch: true,
        availabilityZone: availabilityZone,
        tags: {
            // TODO: 8. Resource tagged
            owner: ownerEmail,
        }
    }, {
        // TODO: 2. Resource has a parent
        parent: vpc
    });

    // TODO: 2. Create route table association
    const routeTableAssoc = new aws.ec2.RouteTableAssociation(`${baseName}-public-rt-assoc-${availabilityZone}`, {
        routeTableId: routeTable.id,
        subnetId: pubSubnet.id,
    }, {
        // TODO: 2. Resource has a parent
        parent: routeTable
    });

    // TODO: 2. Create security group
    const securityGroup = new aws.ec2.SecurityGroup(`${baseName}-sg`, {
        vpcId: vpc.id,
        description: 'Allow SSH inbound traffic',
        ingress: [{
            cidrBlocks: ['0.0.0.0/0'],
            fromPort: 22,
            toPort: 22,
            protocol: 'tcp',
            description: 'SSH into VPC'
        }],
        egress: [{
            cidrBlocks: ['0.0.0.0/0'],
            fromPort: 0,
            toPort: 0,
            protocol: '-1'
        }],
        tags: {
            // TODO: 8. Resource tagged
            owner: ownerEmail,
        },
    }, {
        // TODO: 2. Resource has a parent
        parent: vpc,
        deleteBeforeReplace: true
    });

    // TODO: 3. Create a SSH key
    const sshPrivateKey = new tls.PrivateKey(`${baseName}-private-key`, {
        // TODO: 6. Generate a SSH key pair (ed25519)
        algorithm: "ED25519"
    });

    // TODO: 3. Retrieve the most recent Debian 11 AMI for amd64
    const debianAmi = aws.ec2.getAmi({
        filters: [{
            name: "name",
            values: ["debian-11-amd64-*"]
        }],
        owners: ["136693071363"],
        mostRecent: true,
    });

    // TODO: 6. Generate user data (aka cloud-init configuration)
    // NOTE: This needs to be a valid YAML
    const userData = pulumi.interpolate`#cloud-config
repo_update: true
repo_upgrade: all

users:
  - default:
    ssh_authorized_keys:
      - ${sshPrivateKey.publicKeyOpenssh}
`;

    const vm = new aws.ec2.Instance(`${baseName}-vm`, {
        ami: (await debianAmi).imageId,
        instanceType: "t3.micro",
        subnetId: pubSubnet.id,
        vpcSecurityGroupIds: [securityGroup.id],
        userDataReplaceOnChange: true,
        // TODO: 6. pass it to the VM using user-data
        userData: userData,
        tags: {
            // TODO: 8. Resource tagged
            owner: ownerEmail,
        },
    }, {
        parent: vpc
    });

    return {
        vpcId: vpc.id,
        sshPrivKey: sshPrivateKey.privateKeyOpenssh,
        sshPubKey: sshPrivateKey.publicKeyOpenssh,
        sshKeyFingerprint: sshPrivateKey.publicKeyFingerprintSha256,
        hostname: vm.publicDns
    }
}