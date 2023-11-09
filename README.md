# Pulumi training using TypeScript (3rd session)

Answers to the 3rd training 3rd session

## Answers

1. `pulumi stack init loginName/dev`
2. See [index.ts](index.ts)
3. See [index.ts](index.ts)
4. See [index.ts](index.ts)
5. `npm install "@types/netmask" netmask` and the references to `Netmask` in [index.ts](index.ts)
6. In [index.ts](index.ts)
   * all references to `pulumi.Config()` and `config.require()`
   * `pulumi config set cidrBlock 10.42.0.0/16`
   * `pulumi config set netmask 255.255.240.0`
