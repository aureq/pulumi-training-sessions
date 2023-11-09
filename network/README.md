# Pulumi training using TypeScript (4th session)
Answers to the 3rd training 4th session

## Answers ##

1. `pulumi new aws-typescript`
2. See https://www.pulumi.com/blog/disable-default-providers/
3. pulumi config set --path regions[0] 'ap-southeast-1'
   pulumi config set --path regions[1] 'ap-southeast-2'
   pulumi config set --path cidrBlocks[0] '10.41.0.0/16'
   pulumi config set --path cidrBlocks[1] '10.42.0.0/16'
   pulumi config set --path netmasks[0] '255.255.240.0'
   pulumi config set --path netmasks[1] '255.255.240.0'
   pulumi config set --path availabilityZones[0] 'ap-southeast-1a'
   pulumi config set --path availabilityZones[1] 'ap-southeast-2b'
   See [index.ts](index.ts)