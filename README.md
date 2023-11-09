# Pulumi training using C# (3rd session)

Answers to the 3rd training session

## Answers

1. `pulumi stack init loginName/dev`
2. See [Program.cs](Program.cs)
3. See [Program.cs](Program.cs)
4. See [Program.cs](Program.cs)
5. `dotnet add package ipnetwork2` and the references to `IPNetwork` in [Program.cs](Program.cs)
6. In [Program.cs](Program.cs)
   * all references to `Pulumi.Config()` and `config.Require()`
   * `pulumi config set username aurelien`
   * `pulumi config set cidrBlock 10.42.0.0/16`
   * `pulumi config set netmask 255.255.240.0`
