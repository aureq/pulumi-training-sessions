# Pulumi training using C# (3rd session)

Exercise to learn how to use Pulumi (3rd session)

## Introduction

In this session, you will learn how to create a fully functional VirtualNetwork and a virtual machine. You'll also learn how to use 3rd party library to perform tasks more efficiently and in a reliable way. You'll have another opportunity to use string concatenation on `Output<T>`.

## Content

1. Create a new stack and install the node modules (`dotnet restore`)
2. Create the following resources, and make sure they are all nested under the correct parent
   * A Virtual Network (10.42.0.0/16)
   * A public Subnet (/20)
   * A public IP address
   * A network interface
   * The necessarity network security group and rule
3. Create a virtual machine
   * Use a small virtual machine to limit/reduce cost
   * Using the most recent Ubuntu 20.04 LTS
   * Generate a random password and use it
   * ensure you can SSH into it
4. Create stack outputs for:
   * The VM host name or IP address
   * The random password
   * A configuration admin user name for the VM

## Bonus/Challenges

5. Use a 3rd pary module to compute subnets CIDR
6. Ensure the project can easily be configured (ie, no hardcoded values where possible)

## Resources

* Pulumi [examples](https://github.com/pulumi/examples)

## Answers

You will find all the answers on the `azure-cs/session-3-solution` branch.