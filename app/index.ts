import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import * as tls from "@pulumi/tls";

const config = new pulumi.Config();
const ownerEmail = config.get("ownerEmail") || "unassigned@acmecorp.com";

// TODO: 10. programmatically determine the current stack name
//       We assume the network project uses the same stack name
const stack = pulumi.getStack();

export = async () => {

    const baseName = "app";

    const org = config.require("org");
    const netStackRef = new pulumi.StackReference(`${org}/network/${stack}`)

    // TODO: 3. Use stack references to retrieve stack outputs from the network/dev stack
    const regions: Array<string> = await netStackRef.requireOutputValue("regions");
    const vpcs: Array<string> = await netStackRef.requireOutputValue("vpcs");
    const publicSubnets: Array<string> = await netStackRef.requireOutputValue("publicSubnets");

    const hostnames: pulumi.Output<string>[] = [];

    const sshPrivateKey = new tls.PrivateKey(`${baseName}-private-key`, {
        algorithm: "ED25519"
    });

    const userData = pulumi.interpolate`#cloud-config
repo_update: true
repo_upgrade: all

users:
  - default:
    ssh_authorized_keys:
      - ${sshPrivateKey.publicKeyOpenssh}
`;

    // TODO: 4. Iterate over each AWS region from the network/dev stack
    for (let x = 0; x < regions.length; x++) {
        const regionName = <aws.Region>regions[x];
        const vpcId = vpcs[x];
        const subnetId = publicSubnets[x];
        const name = `${baseName}-${regionName}`

        // TODO: 3. Create a per-region provider
        const awsProvider = new aws.Provider(`${name}-provider`, {
            region: regionName,
        });


        const securityGroup = new aws.ec2.SecurityGroup(`${name}-sg`, {
            vpcId: vpcId,
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
        }, {
            deleteBeforeReplace: true,
            provider: awsProvider,
        });

        const debianAmi = aws.ec2.getAmi({
            filters: [{
                name: "name",
                values: ["debian-11-amd64-*"]
            }],
            owners: ["136693071363"],
            mostRecent: true,
        }, {
            provider: awsProvider,
        });

        const vm = new aws.ec2.Instance(`${name}-vm`, {
            ami: (await debianAmi).imageId,
            instanceType: "t3.micro",
            subnetId: subnetId,
            vpcSecurityGroupIds: [securityGroup.id],
            userDataReplaceOnChange: true,
            userData: userData,
            tags: {
                owner: ownerEmail,
            },
        }, {
            provider: awsProvider,
        });

        hostnames[x] = vm.publicDns;
    }

    // TODO: 5. Stack outputs
    return {
        hostnames: hostnames,
        sshPrivKey: sshPrivateKey.privateKeyOpenssh,
        sshPubKey: sshPrivateKey.publicKeyOpenssh,
        sshKeyFingerprint: sshPrivateKey.publicKeyFingerprintSha256,
    }
}
