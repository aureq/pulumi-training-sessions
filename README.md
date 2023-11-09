# Pulumi training using TypeScript (3rd session)
Answers to the 3rd training 3rd session

## Answers ##

1. `pulumi stack init loginName/dev`
2. See [index.ts](index.ts)
3. npm install --save "@pulumi/tls"
   See [index.ts](index.ts)
4. See [index.ts](index.ts)
5. npm install --save netmask @types/netmask
6. See [index.ts](index.ts)
   pulumi stack output --show-secrets sshPrivKey > id_ed25519 && chmod 0600 id_ed25519 id_ed25519.pub
   pulumi stack output --show-secrets sshPubKey > id_ed25519.pub
   ssh -v -i ./id_ed25519 -l admin $(pulumi stack output hostname)
7. pulumi config set cidrBlock 10.42.0.0/16
   pulumi config set netmask 255.255.240.0
   pulumi config set availablityZone ap-southeast-2a
   pulumi config set ownerEmail will.e.coyote@acmecorp.com
