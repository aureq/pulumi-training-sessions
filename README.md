# Pulumi training using C# (4th session)

Exercises to learn how to use Pulumi (4th session)

## Introduction

This exercise starts where we finished the previous session (a Virtual Network and a VM) but dives deeper on Pulumi reusability patterns at scale. You'll learn how to create your own Component Resource and use stack references along the way.

For a better learning experience, you way want to start with your own code from the previous training session.

## Content

### The `network` project

1. Switch to the [`network/`](./network/) folder and create a new stack (`pulumi new azure-csharp`)
2. Deploy a fully functional virtual network as a component resource
   * Ensure the component resource is easily portable
3. Determine the necessary stack outputs for the `webserver` project to use

### The `webserver` project

4. Switch to the [`webserver/`](./webserver/) folder and create a new stack (`pulumi new azure-csharp`)
5. Use stack references to retrieve the necessary dependencies
6. Use the `Get*()` functions to retrieve existing resources
7. Deploy a VM you can SSH into in the previously created subnet (Component Resource)
   * Use stack references
   * Use Ubuntu 20.04 LTS (latest version)
   * ensure you can SSH into each VM
8. Create stack outputs for:
   * the VMs hostname
   * the username
   * the password

### Bonus/Challenges

9. Use Pulumi runtime functions to determine the current stack name
10. Your component resource doesn't rely on pulumi.Config()

### Resources

* Pulumi [examples](https://github.com/pulumi/examples)

### Answers

You will find all the answers on the azure-cs/session-4-solution branch.
