# Pulumi training using TypeScript (4th session)
Answers to the 3rd training 4th session

## Answers ##

1. `pulumi new aws-typescript`
2. See https://www.pulumi.com/blog/disable-default-providers/
3. See [index.ts](index.ts)
4. See [index.ts](index.ts)
5. See [index.ts](index.ts)
   pulumi stack output --show-secrets sshPrivKey > id_ed25519
   pulumi stack output --show-secrets sshPubKey > id_ed25519.pub
   chmod 0600 id_ed25519 id_ed25519.pub
   ssh -v -i ./id_ed25519 -l admin $(pulumi stack output hostnames --json | jq -r '.[0]')
   ssh -v -i ./id_ed25519 -l admin $(pulumi stack output hostnames --json | jq -r '.[1]')
